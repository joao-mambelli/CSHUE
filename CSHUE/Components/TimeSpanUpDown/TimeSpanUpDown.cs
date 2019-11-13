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

        #endregion

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

        #endregion

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

        #endregion

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

        #endregion

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

                    timeSpan = new TimeSpan(0, int.Parse(values[0].Replace("-", "")),
                        int.Parse(values[1].Replace("-", "")), ShowSeconds ? int.Parse(values[2].Replace("-", "")) : 0,
                        haveMs ? int.Parse(values.Last().Replace("-", "")) : 0);

                    if (text.StartsWith("-"))
                        timeSpan = timeSpan.Negate();
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

            var separators = currentValue.Where(x => x == ':' || x == '.').ToList();
            var values = currentValue.Split(':', '.');
            if (values.Length >= 2 && !values.Any(string.IsNullOrEmpty))
            {
                var haveDays = separators.First() == '.';
                var haveMs = separators.Count > 1 && separators.Last() == '.';

                var result = new TimeSpan(haveDays ? int.Parse(values[0]) : 0,
                    haveDays ? int.Parse(values[1]) : int.Parse(values[0]),
                    haveDays ? int.Parse(values[2]) : int.Parse(values[1]),
                    haveDays && ShowSeconds ? int.Parse(values[3]) : ShowSeconds ? int.Parse(values[2]) : 0,
                    haveMs ? int.Parse(values.Last()) : 0);

                currentValue = result.ToString();
            }

            if (IsTextChangedFromUi && !UpdateValueOnEnterKey || !IsTextChangedFromUi)
                SyncTextAndValueProperties(true, currentValue);
        }

        protected override void OnValueChanged(TimeSpan? oldValue, TimeSpan? newValue)
        {
            if (newValue != null)
            {
                var value = UpdateValueOnEnterKey
                    ? TextBox != null
                        ? ConvertTextToValue(TextBox.Text)
                        : null
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
                        if (hasDay && dayLength != lastDayInfo.Length && SelectedDateTimeInfo.Type != DateTimePart.Day)
                        {
                            FireSelectionChangedEvent = false;
                            TextBox.SelectionStart = Math.Max(0, TextBox.SelectionStart + (dayLength - lastDayInfo.Length));
                            FireSelectionChangedEvent = true;
                        }
                        else if (!hasDay)
                        {
                            FireSelectionChangedEvent = false;
                            TextBox.SelectionStart += dayLength + 1;
                            FireSelectionChangedEvent = true;
                        }
                    }
                }
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
                // ignored
            }

            result = CoerceValueMinMax(result);

            return result;
        }

        private void Increment(int step)
        {
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
                var pasteText = e.DataObject.GetData(typeof(string)) as string;
                var success = TimeSpan.TryParse(pasteText, out _);

                if (!success)
                    e.CancelCommand();
            }
        }

        #endregion
    }
}