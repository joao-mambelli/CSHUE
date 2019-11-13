using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using CSHUE.Components.ButtonSpinner;
using CSHUE.Components.Core.Utilities;
using CSHUE.Components.Primitives;

namespace CSHUE.Components.DateTimeUpDown
{
    public class DateTimeUpDown : DateTimeUpDownBase<DateTime?>
    {
        #region Members

        private DateTime? _lastValidDate;
        private bool _setKindInternal;

        #endregion

        #region Properties

        #region AutoClipTimeParts

        public static readonly DependencyProperty AutoClipTimePartsProperty = DependencyProperty.Register("AutoClipTimeParts", typeof(bool), typeof(DateTimeUpDown), new UIPropertyMetadata(false));
        public bool AutoClipTimeParts
        {
            get => (bool)GetValue(AutoClipTimePartsProperty);
            set => SetValue(AutoClipTimePartsProperty, value);
        }

        #endregion

        #region Format

        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(DateTimeFormat), typeof(DateTimeUpDown), new UIPropertyMetadata(DateTimeFormat.FullDateTime, OnFormatChanged));
        public DateTimeFormat Format
        {
            get => (DateTimeFormat)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        private static void OnFormatChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is DateTimeUpDown dateTimeUpDown)
                dateTimeUpDown.OnFormatChanged((DateTimeFormat)e.OldValue, (DateTimeFormat)e.NewValue);
        }

        protected virtual void OnFormatChanged(DateTimeFormat oldValue, DateTimeFormat newValue)
        {
            FormatUpdated();
        }

        #endregion

        #region FormatString

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string), typeof(DateTimeUpDown), new UIPropertyMetadata(default(string), OnFormatStringChanged), IsFormatStringValid);
        public string FormatString
        {
            get => (string)GetValue(FormatStringProperty);
            set => SetValue(FormatStringProperty, value);
        }

        internal static bool IsFormatStringValid(object value)
        {
            try
            {
                var unused = CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MinSupportedDateTime.ToString((string)value, CultureInfo.CurrentCulture);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is DateTimeUpDown dateTimeUpDown)
                dateTimeUpDown.OnFormatStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnFormatStringChanged(string oldValue, string newValue)
        {
            FormatUpdated();
        }

        #endregion

        #region Kind

        public static readonly DependencyProperty KindProperty = DependencyProperty.Register("Kind", typeof(DateTimeKind), typeof(DateTimeUpDown),
          new FrameworkPropertyMetadata(DateTimeKind.Unspecified, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnKindChanged));
        public DateTimeKind Kind
        {
            get => (DateTimeKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        private static void OnKindChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is DateTimeUpDown dateTimeUpDown)
                dateTimeUpDown.OnKindChanged((DateTimeKind)e.OldValue, (DateTimeKind)e.NewValue);
        }

        protected virtual void OnKindChanged(DateTimeKind oldValue, DateTimeKind newValue)
        {
            if (!_setKindInternal
              && Value != null
              && IsInitialized)
            {
                Value = ConvertToKind(Value.Value, newValue);
            }
        }

        private void SetKindInternal(DateTimeKind kind)
        {
            _setKindInternal = true;
            try
            {
                SetCurrentValue(KindProperty, kind);
            }
            finally
            {
                _setKindInternal = false;
            }
        }

        #endregion

        #region ContextNow (Private)

        internal DateTime ContextNow => DateTimeUtilities.GetContextNow(Kind);

        #endregion

        #endregion

        #region Constructors

        static DateTimeUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeUpDown), new FrameworkPropertyMetadata(typeof(DateTimeUpDown)));
            MaximumProperty.OverrideMetadata(typeof(DateTimeUpDown), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MaxSupportedDateTime));
            MinimumProperty.OverrideMetadata(typeof(DateTimeUpDown), new FrameworkPropertyMetadata(CultureInfo.CurrentCulture.DateTimeFormat.Calendar.MinSupportedDateTime));
            UpdateValueOnEnterKeyProperty.OverrideMetadata(typeof(DateTimeUpDown), new FrameworkPropertyMetadata(true));
        }

        public DateTimeUpDown()
        {
            Loaded += DateTimeUpDown_Loaded;
        }

        #endregion

        #region Base Class Overrides

        public override bool CommitInput()
        {
            var isSyncValid = SyncTextAndValueProperties(true, Text);
            _lastValidDate = Value;
            return isSyncValid;
        }

        protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            FormatUpdated();
        }

        protected override void OnIncrement()
        {
            if (IsCurrentValueValid())
            {
                Increment(Step);
            }
        }

        protected override void OnDecrement()
        {
            if (IsCurrentValueValid())
            {
                Increment(-Step);
            }
        }

        protected override void OnTextChanged(string previousValue, string currentValue)
        {
            if (!ProcessTextChanged)
                return;

            base.OnTextChanged(previousValue, currentValue);
        }

        protected override DateTime? ConvertTextToValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            TryParseDateTime(text, out var result);

            if (Kind != DateTimeKind.Unspecified)
                result = ConvertToKind(result, Kind);

            if (ClipValueToMinMax)
                return GetClippedMinMaxValue(result);

            ValidateDefaultMinMax(result);

            return result;
        }

        protected override string ConvertValueToText()
        {
            if (Value == null)
                return string.Empty;

            return Value.Value.ToString(GetFormatString(Format), CultureInfo);
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

        protected override object OnCoerceValue(object newValue)
        {
            var value = (DateTime?)base.OnCoerceValue(newValue);

            if (value != null && IsInitialized)
                SetKindInternal(value.Value.Kind);

            return value;
        }

        protected override void OnValueChanged(DateTime? oldValue, DateTime? newValue)
        {
            var info = SelectedDateTimeInfo ?? (CurrentDateTimePart != DateTimePart.Other ? GetDateTimeInfo(CurrentDateTimePart) : DateTimeInfoList[0]);

            if (newValue != null)
                ParseValueIntoDateTimeInfo(Value);

            base.OnValueChanged(oldValue, newValue);

            if (!IsTextChangedFromUi)
            {
                _lastValidDate = newValue;
            }

            if (TextBox != null)
            {
                FireSelectionChangedEvent = false;
                TextBox.Select(info.StartPosition, info.Length);
                FireSelectionChangedEvent = true;
            }
        }

        protected override bool IsCurrentValueValid()
        {
            if (string.IsNullOrEmpty(TextBox.Text))
                return true;

            return TryParseDateTime(TextBox.Text, out _);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (Value != null)
            {
                var valueKind = Value.Value.Kind;

                if (valueKind != Kind)
                {
                    if (Kind == DateTimeKind.Unspecified)
                        SetKindInternal(valueKind);
                    else
                        Value = ConvertToKind(Value.Value, Kind);
                }
            }
        }

        protected override void PerformMouseSelection()
        {
            if (UpdateValueOnEnterKey)
            {
                ParseValueIntoDateTimeInfo(ConvertTextToValue(TextBox.Text));
            }
            base.PerformMouseSelection();
        }

        protected internal override void PerformKeyboardSelection(int nextSelectionStart)
        {
            if (UpdateValueOnEnterKey)
            {
                ParseValueIntoDateTimeInfo(ConvertTextToValue(TextBox.Text));
            }
            base.PerformKeyboardSelection(nextSelectionStart);
        }

        protected void InitializeDateTimeInfoList(DateTime? value)
        {
            DateTimeInfoList.Clear();
            SelectedDateTimeInfo = null;

            var format = GetFormatString(Format);

            if (string.IsNullOrEmpty(format))
                return;

            while (format.Length > 0)
            {
                var elementLength = GetElementLengthByFormat(format);
                DateTimeInfo info = null;

                switch (format[0])
                {
                    case '"':
                    case '\'':
                        {
                            var closingQuotePosition = format.IndexOf(format[0], 1);
                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Type = DateTimePart.Other,
                                Length = 1,
                                Content = format.Substring(1, Math.Max(1, closingQuotePosition - 1))
                            };
                            elementLength = Math.Max(1, closingQuotePosition + 1);
                            break;
                        }
                    case 'D':
                    case 'd':
                        {
                            var d = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                d = "%" + d;

                            if (elementLength > 2)
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = true,
                                    Type = DateTimePart.DayName,
                                    Length = elementLength,
                                    Format = d
                                };
                            else
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = false,
                                    Type = DateTimePart.Day,
                                    Length = elementLength,
                                    Format = d
                                };
                            break;
                        }
                    case 'F':
                    case 'f':
                        {
                            var f = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                f = "%" + f;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Millisecond,
                                Length = elementLength,
                                Format = f
                            };
                            break;
                        }
                    case 'h':
                        {
                            var h = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                h = "%" + h;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Hour12,
                                Length = elementLength,
                                Format = h
                            };
                            break;
                        }
                    case 'H':
                        {
                            var h = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                h = "%" + h;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Hour24,
                                Length = elementLength,
                                Format = h
                            };
                            break;
                        }
                    case 'M':
                        {
                            var m = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                m = "%" + m;

                            if (elementLength >= 3)
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = false,
                                    Type = DateTimePart.MonthName,
                                    Length = elementLength,
                                    Format = m
                                };
                            else
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = false,
                                    Type = DateTimePart.Month,
                                    Length = elementLength,
                                    Format = m
                                };
                            break;
                        }
                    case 'S':
                    case 's':
                        {
                            var s = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                s = "%" + s;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Second,
                                Length = elementLength,
                                Format = s
                            };
                            break;
                        }
                    case 'T':
                    case 't':
                        {
                            var t = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                t = "%" + t;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.AmPmDesignator,
                                Length = elementLength,
                                Format = t
                            };
                            break;
                        }
                    case 'Y':
                    case 'y':
                        {
                            var y = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                y = "%" + y;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Year,
                                Length = elementLength,
                                Format = y
                            };
                            break;
                        }
                    case '\\':
                        {
                            if (format.Length >= 2)
                            {
                                info = new DateTimeInfo
                                {
                                    IsReadOnly = true,
                                    Content = format.Substring(1, 1),
                                    Length = 1,
                                    Type = DateTimePart.Other
                                };
                                elementLength = 2;
                            }
                            break;
                        }
                    case 'g':
                        {
                            var g = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                g = "%" + g;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Type = DateTimePart.Period,
                                Length = elementLength,
                                Format = g
                            };
                            break;
                        }
                    case 'm':
                        {
                            var m = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                m = "%" + m;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = false,
                                Type = DateTimePart.Minute,
                                Length = elementLength,
                                Format = m
                            };
                            break;
                        }
                    case 'z':
                        {
                            var z = format.Substring(0, elementLength);
                            if (elementLength == 1)
                                z = "%" + z;

                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Type = DateTimePart.TimeZone,
                                Length = elementLength,
                                Format = z
                            };
                            break;
                        }
                    default:
                        {
                            elementLength = 1;
                            info = new DateTimeInfo
                            {
                                IsReadOnly = true,
                                Length = 1,
                                Content = format[0].ToString(),
                                Type = DateTimePart.Other
                            };
                            break;
                        }
                }

                DateTimeInfoList.Add(info);
                format = format.Substring(elementLength);
            }
        }

        protected override bool IsLowerThan(DateTime? value1, DateTime? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return value1.Value < value2.Value;
        }

        protected override bool IsGreaterThan(DateTime? value1, DateTime? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return value1.Value > value2.Value;
        }

        protected override void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue)
        {
            throw new NotSupportedException("DateTimeUpDown controls do not support modifying UpdateValueOnEnterKey property.");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SyncTextAndValueProperties(false, null);
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }


        #endregion

        #region Methods

        public void SelectAll()
        {
            FireSelectionChangedEvent = false;
            TextBox.SelectAll();
            FireSelectionChangedEvent = true;
        }

        private void FormatUpdated()
        {
            InitializeDateTimeInfoList(Value);

            if (Value != null)
                ParseValueIntoDateTimeInfo(Value);

            ProcessTextChanged = false;

            SyncTextAndValueProperties(false, null);

            ProcessTextChanged = true;

        }

        private static int GetElementLengthByFormat(string format)
        {
            for (var i = 1; i < format.Length; i++)
            {
                if (string.CompareOrdinal(format[i].ToString(), format[0].ToString()) != 0)
                {
                    return i;
                }
            }
            return format.Length;
        }

        private void Increment(int step)
        {
            FireSelectionChangedEvent = false;

            var currentValue = ConvertTextToValue(TextBox.Text);
            if (currentValue.HasValue)
            {
                var newValue = UpdateDateTime(currentValue, step);
                if (newValue == null)
                    return;
                TextBox.Text = newValue.Value.ToString(GetFormatString(Format), CultureInfo);
            }
            else
            {
                TextBox.Text = DefaultValue != null
                                    ? DefaultValue.Value.ToString(GetFormatString(Format), CultureInfo)
                                    : ContextNow.ToString(GetFormatString(Format), CultureInfo);
            }

            if (TextBox != null)
            {
                var info = SelectedDateTimeInfo ?? (CurrentDateTimePart != DateTimePart.Other ? GetDateTimeInfo(CurrentDateTimePart) : DateTimeInfoList[0]);

                ParseValueIntoDateTimeInfo(ConvertTextToValue(TextBox.Text));

                TextBox.Select(info.StartPosition, info.Length);
            }
            FireSelectionChangedEvent = true;

            SyncTextAndValueProperties(true, Text);
        }

        private void ParseValueIntoDateTimeInfo(DateTime? newDate)
        {
            var text = string.Empty;

            DateTimeInfoList.ForEach(info =>
           {
               if (info.Format == null)
               {
                   info.StartPosition = text.Length;
                   info.Length = info.Content.Length;
                   text += info.Content;
               }
               else if (newDate != null)
               {
                   var date = newDate.Value;
                   info.StartPosition = text.Length;
                   info.Content = date.ToString(info.Format, CultureInfo.DateTimeFormat);
                   info.Length = info.Content.Length;
                   text += info.Content;
               }
           });
        }

        internal string GetFormatString(DateTimeFormat dateTimeFormat)
        {
            switch (dateTimeFormat)
            {
                case DateTimeFormat.ShortDate:
                    return CultureInfo.DateTimeFormat.ShortDatePattern;
                case DateTimeFormat.LongDate:
                    return CultureInfo.DateTimeFormat.LongDatePattern;
                case DateTimeFormat.ShortTime:
                    return CultureInfo.DateTimeFormat.ShortTimePattern;
                case DateTimeFormat.LongTime:
                    return CultureInfo.DateTimeFormat.LongTimePattern;
                case DateTimeFormat.FullDateTime:
                    return CultureInfo.DateTimeFormat.FullDateTimePattern;
                case DateTimeFormat.MonthDay:
                    return CultureInfo.DateTimeFormat.MonthDayPattern;
                case DateTimeFormat.Rfc1123:
                    return CultureInfo.DateTimeFormat.RFC1123Pattern;
                case DateTimeFormat.SortableDateTime:
                    return CultureInfo.DateTimeFormat.SortableDateTimePattern;
                case DateTimeFormat.UniversalSortableDateTime:
                    return CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
                case DateTimeFormat.YearMonth:
                    return CultureInfo.DateTimeFormat.YearMonthPattern;
                case DateTimeFormat.Custom:
                    {
                        switch (FormatString)
                        {
                            case "d":
                                return CultureInfo.DateTimeFormat.ShortDatePattern;
                            case "t":
                                return CultureInfo.DateTimeFormat.ShortTimePattern;
                            case "T":
                                return CultureInfo.DateTimeFormat.LongTimePattern;
                            case "D":
                                return CultureInfo.DateTimeFormat.LongDatePattern;
                            case "f":
                                return CultureInfo.DateTimeFormat.LongDatePattern + " " + CultureInfo.DateTimeFormat.ShortTimePattern;
                            case "F":
                                return CultureInfo.DateTimeFormat.FullDateTimePattern;
                            case "g":
                                return CultureInfo.DateTimeFormat.ShortDatePattern + " " + CultureInfo.DateTimeFormat.ShortTimePattern;
                            case "G":
                                return CultureInfo.DateTimeFormat.ShortDatePattern + " " + CultureInfo.DateTimeFormat.LongTimePattern;
                            case "m":
                                return CultureInfo.DateTimeFormat.MonthDayPattern;
                            case "y":
                                return CultureInfo.DateTimeFormat.YearMonthPattern;
                            case "r":
                                return CultureInfo.DateTimeFormat.RFC1123Pattern;
                            case "s":
                                return CultureInfo.DateTimeFormat.SortableDateTimePattern;
                            case "u":
                                return CultureInfo.DateTimeFormat.UniversalSortableDateTimePattern;
                            default:
                                return FormatString;
                        }
                    }
                default:
                    throw new ArgumentException("Not a supported format");
            }
        }

        private DateTime? UpdateDateTime(DateTime? currentDateTime, int value)
        {
            var info = SelectedDateTimeInfo ?? (CurrentDateTimePart != DateTimePart.Other ? GetDateTimeInfo(CurrentDateTimePart) : DateTimeInfoList[0]);

            DateTime? result = null;

            try
            {
                switch (info.Type)
                {
                    case DateTimePart.Year:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddYears(value);
                            break;
                        }
                    case DateTimePart.Month:
                    case DateTimePart.MonthName:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddMonths(value);
                            break;
                        }
                    case DateTimePart.Day:
                    case DateTimePart.DayName:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddDays(value);
                            break;
                        }
                    case DateTimePart.Hour12:
                    case DateTimePart.Hour24:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddHours(value);
                            break;
                        }
                    case DateTimePart.Minute:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddMinutes(value);
                            break;
                        }
                    case DateTimePart.Second:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddSeconds(value);
                            break;
                        }
                    case DateTimePart.Millisecond:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddMilliseconds(value);
                            break;
                        }
                    case DateTimePart.AmPmDesignator:
                        {
                            if (currentDateTime != null) result = ((DateTime)currentDateTime).AddHours(value * 12);
                            break;
                        }
                }
            }
            catch
            {
                // ignored
            }

            return CoerceValueMinMax(result);
        }

        private bool TryParseDateTime(string text, out DateTime result)
        {
            bool isValid;
            result = ContextNow;

            var current = ContextNow;
            try
            {
                current = Value ?? DateTime.Parse(ContextNow.ToString(CultureInfo.InvariantCulture), CultureInfo.DateTimeFormat);

                isValid = DateTimeParser.TryParse(text, GetFormatString(Format), current, CultureInfo, AutoClipTimeParts, out result);
            }
            catch (FormatException)
            {
                isValid = false;
            }

            if (!isValid)
            {
                isValid = DateTime.TryParseExact(text, GetFormatString(Format), CultureInfo, DateTimeStyles.None, out result);
            }

            if (!isValid)
            {
                result = _lastValidDate ?? current;
            }

            return isValid;
        }

        private static DateTime ConvertToKind(DateTime dateTime, DateTimeKind kind)
        {
            if (kind == dateTime.Kind)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Unspecified || kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime, kind);

            return kind == DateTimeKind.Local
               ? dateTime.ToLocalTime()
               : dateTime.ToUniversalTime();
        }

        #endregion

        #region Event Handlers

        private void DateTimeUpDown_Loaded(object sender, RoutedEventArgs e)
        {
            if (Format == DateTimeFormat.Custom && string.IsNullOrEmpty(FormatString))
            {
                throw new InvalidOperationException("A FormatString is necessary when Format is set to Custom.");
            }
        }

        #endregion
    }
}