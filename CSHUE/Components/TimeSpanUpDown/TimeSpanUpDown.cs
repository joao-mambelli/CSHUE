using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CSHUE.Components.ButtonSpinner;
using CSHUE.Components.DateTimeUpDown;
using CSHUE.Components.Primitives;

namespace CSHUE.Components.TimeSpanUpDown
{
    public class TimeSpanUpDown : DateTimeUpDownBase<TimeSpan?>
    {
        #region Constructors

        static TimeSpanUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(typeof(TimeSpanUpDown)));
            MaximumProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(TimeSpan.MaxValue));
            MinimumProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(TimeSpan.MinValue));
            DefaultValueProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(TimeSpan.Zero));
        }

        public TimeSpanUpDown()
        {
            DataObject.AddPastingHandler(this, OnPasting);
        }

        #endregion //Constructors

        #region Properties

        #region FractionalSecondsDigitsCount

        public static readonly DependencyProperty FractionalSecondsDigitsCountProperty = DependencyProperty.Register("FractionalSecondsDigitsCount", typeof(int), typeof(TimeSpanUpDown), new UIPropertyMetadata(0, OnFractionalSecondsDigitsCountChanged, OnCoerceFractionalSecondsDigitsCount));
        public int FractionalSecondsDigitsCount
        {
            get => (int)GetValue(FractionalSecondsDigitsCountProperty);
            set => SetValue(FractionalSecondsDigitsCountProperty, value);
        }

        private static object OnCoerceFractionalSecondsDigitsCount(DependencyObject o, object value)
        {
            if (o is TimeSpanUpDown)
            {
                var digitsCount = (int)value;
                if (digitsCount < 0 || digitsCount > 3)
                    throw new ArgumentException("Fractional seconds digits count must be between 0 and 3.");
            }
            return value;
        }

        private static void OnFractionalSecondsDigitsCountChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is TimeSpanUpDown timeSpanUpDown)
                timeSpanUpDown.OnFractionalSecondsDigitsCountChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void OnFractionalSecondsDigitsCountChanged(int oldValue, int newValue)
        {
            UpdateValue();
        }

        #endregion //FractionalSecondsDigitsCount

        #region ShowDays

        public static readonly DependencyProperty ShowDaysProperty = DependencyProperty.Register("ShowDays", typeof(bool), typeof(TimeSpanUpDown), new UIPropertyMetadata(true, OnShowDaysChanged));
        public bool ShowDays
        {
            get => (bool)GetValue(ShowDaysProperty);
            set => SetValue(ShowDaysProperty, value);
        }

        private static void OnShowDaysChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is TimeSpanUpDown timeSpanUpDown)
                timeSpanUpDown.OnShowDaysChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnShowDaysChanged(bool oldValue, bool newValue)
        {
            UpdateValue();
        }

        #endregion //ShowDays

        #region ShowSeconds

        public static readonly DependencyProperty ShowSecondsProperty = DependencyProperty.Register("ShowSeconds", typeof(bool), typeof(TimeSpanUpDown), new UIPropertyMetadata(true, OnShowSecondsChanged));
        public bool ShowSeconds
        {
            get => (bool)GetValue(ShowSecondsProperty);
            set => SetValue(ShowSecondsProperty, value);
        }

        private static void OnShowSecondsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is TimeSpanUpDown timeSpanUpDown)
                timeSpanUpDown.OnShowSecondsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnShowSecondsChanged(bool oldValue, bool newValue)
        {
            UpdateValue();
        }

        #endregion //ShowSeconds

        #endregion

        #region BaseClass Overrides

        protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            var value = UpdateValueOnEnterKey
                        ? TextBox != null ? ConvertTextToValue(TextBox.Text) : null
                        : Value;
            InitializeDateTimeInfoList(value);
        }

        protected override void SetValidSpinDirection()
        {
            var validDirections = ValidSpinDirections.None;

            if (!IsReadOnly)
            {
                if (IsLowerThan(Value, Maximum) || !Value.HasValue || !Maximum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Increase;

                if (IsGreaterThan(Value, Minimum) || !Value.HasValue || !Minimum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Decrease;
            }

            if (Spinner != null)
                Spinner.ValidSpinDirection = validDirections;
        }

        protected override void OnIncrement()
        {
            Increment(Step);
        }

        protected override void OnDecrement()
        {
            Increment(-Step);
        }

        protected override string ConvertValueToText()
        {
            if (Value == null)
                return string.Empty;

            return ParseValueIntoTimeSpanInfo(Value, true);
        }

        protected override TimeSpan? ConvertTextToValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            var timeSpan = TimeSpan.MinValue;
            if (ShowDays)
            {
                timeSpan = TimeSpan.Parse(text);
            }
            else
            {
                var separators = text.Where(x => x == ':' || x == '.').ToList();
                var values = text.Split(':', '.');
                if (values.Length >= 2 && !values.Any(string.IsNullOrEmpty))
                {
                    var haveMs = separators.Count > 1 && separators.Last() == '.';

                    timeSpan = new TimeSpan(0,  //Days
                                             int.Parse(values[0].Replace("-", "")),  //Hours
                                             int.Parse(values[1].Replace("-", "")),  //Minutes
                                             ShowSeconds ? int.Parse(values[2].Replace("-", "")) : 0,  //Seconds
                                             haveMs ? int.Parse(values.Last().Replace("-", "")) : 0);  //Milliseconds
                    if (text.StartsWith("-"))
                    {
                        timeSpan = timeSpan.Negate();
                    }
                }
            }

            if (ClipValueToMinMax)
            {
                return GetClippedMinMaxValue(timeSpan);
            }

            ValidateDefaultMinMax(timeSpan);

            return timeSpan;
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            e.Handled = !IsNumber(e.Text);
            base.OnPreviewTextInput(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnTextChanged(string previousValue, string currentValue)
        {
            if (!ProcessTextChanged)
                return;

            if (string.IsNullOrEmpty(currentValue))
            {
                if (!UpdateValueOnEnterKey)
                {
                    Value = null;
                }
                return;
            }

            // Validate when more than 60 seconds (or more than 60 minutes, or more than 24 hours) are entered.
            var separators = currentValue.Where(x => x == ':' || x == '.').ToList();
            var values = currentValue.Split(':', '.');
            if (values.Length >= 2 && !values.Any(string.IsNullOrEmpty))
            {
                var haveDays = separators.First() == '.';
                var haveMs = separators.Count > 1 && separators.Last() == '.';

                var result = new TimeSpan(haveDays ? int.Parse(values[0]) : 0,  //Days
                    haveDays ? int.Parse(values[1]) : int.Parse(values[0]),  //Hours
                    haveDays ? int.Parse(values[2]) : int.Parse(values[1]),  //Minutes
                    haveDays && ShowSeconds ? int.Parse(values[3]) : ShowSeconds ? int.Parse(values[2]) : 0,  //Seconds
                    haveMs ? int.Parse(values.Last()) : 0);

                currentValue = result.ToString();
            }

            // When text is typed, if UpdateValueOnEnterKey is true, 
            // Sync Value on Text only when Enter Key is pressed.
            if (IsTextChangedFromUi && !UpdateValueOnEnterKey
              || !IsTextChangedFromUi)
            {
                SyncTextAndValueProperties(true, currentValue);
            }
        }

        protected override void OnValueChanged(TimeSpan? oldValue, TimeSpan? newValue)
        {
            //whenever the value changes we need to parse out the value into out DateTimeInfo segments so we can keep track of the individual pieces
            //but only if it is not null
            if (newValue != null)
            {
                var value = UpdateValueOnEnterKey
                          ? TextBox != null ? ConvertTextToValue(TextBox.Text) : null
                          : Value;
                InitializeDateTimeInfoList(value);
            }
            base.OnValueChanged(oldValue, newValue);
        }

        protected override void PerformMouseSelection()
        {
            var value = UpdateValueOnEnterKey
                        ? TextBox != null ? ConvertTextToValue(TextBox.Text) : null
                        : Value;
            InitializeDateTimeInfoList(value);
            base.PerformMouseSelection();
        }

        protected void InitializeDateTimeInfoList(TimeSpan? value)
        {
            var lastDayInfo = DateTimeInfoList.FirstOrDefault(x => x.Type == DateTimePart.Day);
            var hasDay = lastDayInfo != null;
            var negInfo = DateTimeInfoList.FirstOrDefault(x => x.Type == DateTimePart.Other);
            var hasNegative = negInfo != null && negInfo.Content == "-";

            DateTimeInfoList.Clear();

            if (value.HasValue && value.Value.TotalMilliseconds < 0)
            {
                DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Other, Length = 1, Content = "-", IsReadOnly = true });
                // Negative has been added, move TextBox.Selection to keep it on current DateTimeInfo
                if (!hasNegative && TextBox != null)
                {
                    FireSelectionChangedEvent = false;
                    TextBox.SelectionStart++;
                    FireSelectionChangedEvent = true;
                }
            }

            if (ShowDays)
            {
                if (value.HasValue && value.Value.Days != 0)
                {
                    var dayLength = Math.Abs(value.Value.Days).ToString().Length;
                    DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Day, Length = dayLength, Format = "dd" });
                    DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Other, Length = 1, Content = ".", IsReadOnly = true });

                    if (TextBox != null)
                    {
                        //number of digits for days has changed when selection is not on date part, move TextBox.Selection to keep it on current DateTimeInfo
                        if (hasDay && dayLength != lastDayInfo.Length && SelectedDateTimeInfo.Type != DateTimePart.Day)
                        {
                            FireSelectionChangedEvent = false;
                            TextBox.SelectionStart = Math.Max(0, TextBox.SelectionStart + (dayLength - lastDayInfo.Length));
                            FireSelectionChangedEvent = true;
                        }
                        // Day has been added, move TextBox.Selection to keep it on current DateTimeInfo
                        else if (!hasDay)
                        {
                            FireSelectionChangedEvent = false;
                            TextBox.SelectionStart += dayLength + 1;
                            FireSelectionChangedEvent = true;
                        }
                    }
                }
                // Day has been removed, move TextBox.Selection to keep it on current DateTimeInfo
                else if (hasDay)
                {
                    FireSelectionChangedEvent = false;
                    if (TextBox != null)
                        TextBox.SelectionStart = Math.Max(hasNegative ? 1 : 0, TextBox.SelectionStart - (lastDayInfo.Length + 1));
                    FireSelectionChangedEvent = true;
                }
            }

            DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Hour24, Length = 2, Format = "hh" });
            DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Other, Length = 1, Content = ":", IsReadOnly = true });
            DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Minute, Length = 2, Format = "mm" });
            if (ShowSeconds)
            {
                DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Other, Length = 1, Content = ":", IsReadOnly = true });
                DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Second, Length = 2, Format = "ss" });
            }

            if (FractionalSecondsDigitsCount > 0)
            {
                DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Other, Length = 1, Content = ".", IsReadOnly = true });
                var fraction = new string('f', FractionalSecondsDigitsCount);
                //If the "f" custom format specifier is used alone, specify "%f" so that it is not misinterpreted as a standard format string.
                if (fraction.Length == 1)
                {
                    fraction = "%" + fraction;
                }
                DateTimeInfoList.Add(new DateTimeInfo { Type = DateTimePart.Millisecond, Length = FractionalSecondsDigitsCount, Format = fraction });
            }

            if (value.HasValue)
            {
                ParseValueIntoTimeSpanInfo(value, true);
            }
        }

        protected override bool IsLowerThan(TimeSpan? value1, TimeSpan? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return value1.Value < value2.Value;
        }

        protected override bool IsGreaterThan(TimeSpan? value1, TimeSpan? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return value1.Value > value2.Value;
        }


        #endregion

        #region Methods

        private string ParseValueIntoTimeSpanInfo(TimeSpan? value, bool modifyInfo)
        {
            var text = string.Empty;

            DateTimeInfoList.ForEach(info =>
           {
               if (info.Format == null)
               {
                   if (modifyInfo)
                   {
                       info.StartPosition = text.Length;
                       info.Length = info.Content.Length;
                   }
                   text += info.Content;
               }
               else
               {
                   var span = TimeSpan.Parse(value.ToString());
                   if (modifyInfo)
                   {
                       info.StartPosition = text.Length;
                   }

                   // Display days and hours or totalHours
                   var content = !ShowDays && span.Days != 0 && info.Format == "hh"
                       ? Math.Truncate(Math.Abs(span.TotalHours)).ToString(CultureInfo.InvariantCulture)
                       : span.ToString(info.Format, CultureInfo.DateTimeFormat);

                   if (modifyInfo)
                   {
                       if (info.Format == "dd")
                       {
                           content = Convert.ToInt32(content).ToString();
                       }
                       info.Content = content;
                       info.Length = info.Content.Length;
                   }
                   text += content;
               }
           });

            return text;
        }

        private TimeSpan? UpdateTimeSpan(TimeSpan? currentValue, int value)
        {
            var info = SelectedDateTimeInfo ?? (CurrentDateTimePart != DateTimePart.Other
                           ? GetDateTimeInfo(CurrentDateTimePart)
                           : DateTimeInfoList[0].Content != "-" ? DateTimeInfoList[0] : DateTimeInfoList[1]);

            //this only occurs when the user manually type in a value for the Value Property

            TimeSpan? result = null;

            try
            {
                switch (info.Type)
                {
                    case DateTimePart.Day:
                        if (currentValue != null) result = ((TimeSpan)currentValue).Add(new TimeSpan(value, 0, 0, 0, 0));
                        break;
                    case DateTimePart.Hour24:
                        if (currentValue != null) result = ((TimeSpan)currentValue).Add(new TimeSpan(0, value, 0, 0, 0));
                        break;
                    case DateTimePart.Minute:
                        if (currentValue != null) result = ((TimeSpan)currentValue).Add(new TimeSpan(0, 0, value, 0, 0));
                        break;
                    case DateTimePart.Second:
                        if (currentValue != null) result = ((TimeSpan)currentValue).Add(new TimeSpan(0, 0, 0, value, 0));
                        break;
                    case DateTimePart.Millisecond:
                        switch (FractionalSecondsDigitsCount)
                        {
                            case 1:
                                value = value * 100;
                                break;
                            case 2:
                                value = value * 10;
                                break;
                            default:
                                value = value * 1;
                                break;
                        }

                        if (currentValue != null) result = ((TimeSpan)currentValue).Add(new TimeSpan(0, 0, 0, 0, value));
                        break;
                }
            }
            catch
            {
                //this can occur if the date/time = 1/1/0001 12:00:00 AM which is the smallest date allowed.
                //I could write code that would validate the date each and everytime but I think that it would be more
                //efficient if I just handle the edge case and allow an exeption to occur and swallow it instead.
            }

            result = CoerceValueMinMax(result);

            return result;
        }

        private void Increment(int step)
        {
            // if UpdateValueOnEnterKey is true, 
            // Sync Value on Text only when Enter Key is pressed.
            if (UpdateValueOnEnterKey)
            {
                var currentValue = ConvertTextToValue(TextBox.Text);
                var newValue = currentValue.HasValue
                               ? UpdateTimeSpan(currentValue, step)
                               : DefaultValue ?? TimeSpan.Zero;

                if (newValue != null)
                {
                    InitializeDateTimeInfoList(newValue);
                    var selectionStart = TextBox.SelectionStart;
                    var selectionLength = TextBox.SelectionLength;

                    TextBox.Text = ParseValueIntoTimeSpanInfo(newValue, false);
                    TextBox.Select(selectionStart, selectionLength);
                }
            }
            else
            {
                if (Value.HasValue)
                {
                    var newValue = UpdateTimeSpan(Value, step);
                    if (newValue != null)
                    {
                        InitializeDateTimeInfoList(newValue);
                        var selectionStart = TextBox.SelectionStart;
                        var selectionLength = TextBox.SelectionLength;
                        Value = newValue;
                        TextBox.Select(selectionStart, selectionLength);
                    }
                }
                else
                {
                    Value = DefaultValue ?? TimeSpan.Zero;
                }
            }
        }

        private static bool IsNumber(string str)
        {
            foreach (var c in str)
            {
                if (!char.IsNumber(c))
                    return false;
            }

            return true;
        }

        private void UpdateValue()
        {
            var value = UpdateValueOnEnterKey
                        ? TextBox != null ? ConvertTextToValue(TextBox.Text) : null
                        : Value;
            InitializeDateTimeInfoList(value);
            SyncTextAndValueProperties(false, Text);
        }

        #endregion

        #region Event Handlers

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                // Allow pasting only TimeSpan values
                var pasteText = e.DataObject.GetData(typeof(string)) as string;
                var success = TimeSpan.TryParse(pasteText, out _);
                if (!success)
                {
                    e.CancelCommand();
                }
            }
        }

        #endregion
    }
}