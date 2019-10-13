using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using CSHUE.Components.ButtonSpinner;

namespace CSHUE.Components.NumericUpDown
{
    public abstract class CommonNumericUpDown<T> : NumericUpDown<T?> where T : struct, IFormattable, IComparable<T>
    {
        protected delegate bool FromText(string s, NumberStyles style, IFormatProvider provider, out T result);
        protected delegate T FromDecimal(decimal d);

        #region Private Members

        private readonly FromText _fromText;
        private readonly FromDecimal _fromDecimal;
        private readonly Func<T, T, bool> _fromLowerThan;
        private readonly Func<T, T, bool> _fromGreaterThan;

        #endregion

        #region Properties

        #region IsInvalid

        internal static readonly DependencyProperty IsInvalidProperty = DependencyProperty.Register("IsInvalid", typeof(bool), typeof(CommonNumericUpDown<T>), new UIPropertyMetadata(false));
        internal bool IsInvalid
        {
            get => (bool)GetValue(IsInvalidProperty);
            private set => SetValue(IsInvalidProperty, value);
        }

        #endregion //IsInvalid

        #region ParsingNumberStyle

        public static readonly DependencyProperty ParsingNumberStyleProperty =
            DependencyProperty.Register("ParsingNumberStyle", typeof(NumberStyles), typeof(CommonNumericUpDown<T>), new UIPropertyMetadata(NumberStyles.Any));

        public NumberStyles ParsingNumberStyle
        {
            get => (NumberStyles)GetValue(ParsingNumberStyleProperty);
            set => SetValue(ParsingNumberStyleProperty, value);
        }

        #endregion //ParsingNumberStyle

        #endregion

        #region Constructors

        protected CommonNumericUpDown(FromText fromText, FromDecimal fromDecimal, Func<T, T, bool> fromLowerThan, Func<T, T, bool> fromGreaterThan)
        {
            _fromText = fromText ?? throw new ArgumentNullException(nameof(fromText));
            _fromDecimal = fromDecimal ?? throw new ArgumentNullException(nameof(fromDecimal));
            _fromLowerThan = fromLowerThan ?? throw new ArgumentNullException(nameof(fromLowerThan));
            _fromGreaterThan = fromGreaterThan ?? throw new ArgumentNullException(nameof(fromGreaterThan));
        }

        #endregion

        #region Internal Methods

        protected static void UpdateMetadata(Type type, T? increment, T? minValue, T? maxValue)
        {
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            UpdateMetadataCommon(type, increment, minValue, maxValue);
        }

        protected void TestInputSpecialValue(AllowedSpecialValues allowedValues, AllowedSpecialValues valueToCompare)
        {
            if ((allowedValues & valueToCompare) != valueToCompare)
            {
                switch (valueToCompare)
                {
                    case AllowedSpecialValues.NaN:
                        throw new InvalidDataException("Value to parse shouldn't be NaN.");
                    case AllowedSpecialValues.PositiveInfinity:
                        throw new InvalidDataException("Value to parse shouldn't be Positive Infinity.");
                    case AllowedSpecialValues.NegativeInfinity:
                        throw new InvalidDataException("Value to parse shouldn't be Negative Infinity.");
                }
            }
        }

        internal bool IsBetweenMinMax(T? value)
        {
            return !IsLowerThan(value, Minimum) && !IsGreaterThan(value, Maximum);
        }

        #endregion

        #region Private Methods

        private static void UpdateMetadataCommon(Type type, T? increment, T? minValue, T? maxValue)
        {
            IncrementProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(increment));
            MaximumProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(maxValue));
            MinimumProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(minValue));
        }

        private bool IsLowerThan(T? value1, T? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return _fromLowerThan(value1.Value, value2.Value);
        }

        private bool IsGreaterThan(T? value1, T? value2)
        {
            if (value1 == null || value2 == null)
                return false;

            return _fromGreaterThan(value1.Value, value2.Value);
        }

        private bool HandleNullSpin()
        {
            if (!Value.HasValue)
            {
                var forcedValue = DefaultValue ?? default(T);

                Value = CoerceValueMinMax(forcedValue);

                return true;
            }

            if (!Increment.HasValue)
            {
                return true;
            }

            return false;
        }

        private T? CoerceValueMinMax(T value)
        {
            if (IsLowerThan(value, Minimum))
                return Minimum;
            if (IsGreaterThan(value, Maximum))
                return Maximum;
            return value;
        }

        #endregion

        #region Base Class Overrides

        protected override void OnIncrement()
        {
            if (!HandleNullSpin())
            {
                // if UpdateValueOnEnterKey is true, 
                // Sync Value on Text only when Enter Key is pressed.
                if (UpdateValueOnEnterKey)
                {
                    var currentValue = ConvertTextToValue(TextBox.Text);
                    if (currentValue != null)
                    {
                        if (Increment != null)
                        {
                            var result = IncrementValue(currentValue.Value, Increment.Value);
                            var newValue = CoerceValueMinMax(result);
                            if (newValue != null) TextBox.Text = newValue.Value.ToString(FormatString, CultureInfo);
                        }
                    }
                }
                else
                {
                    if (Value != null)
                    {
                        if (Increment != null)
                        {
                            var result = IncrementValue(Value.Value, Increment.Value);
                            Value = CoerceValueMinMax(result);
                        }
                    }
                }
            }
        }

        protected override void OnDecrement()
        {
            if (!HandleNullSpin())
            {
                // if UpdateValueOnEnterKey is true, 
                // Sync Value on Text only when Enter Key is pressed.
                if (UpdateValueOnEnterKey)
                {
                    var currentValue = ConvertTextToValue(TextBox.Text);
                    if (currentValue != null)
                    {
                        if (Increment != null)
                        {
                            var result = DecrementValue(currentValue.Value, Increment.Value);
                            var newValue = CoerceValueMinMax(result);
                            if (newValue != null) TextBox.Text = newValue.Value.ToString(FormatString, CultureInfo);
                        }
                    }
                }
                else
                {
                    if (Value != null)
                    {
                        if (Increment != null)
                        {
                            var result = DecrementValue(Value.Value, Increment.Value);
                            Value = CoerceValueMinMax(result);
                        }
                    }
                }
            }
        }

        protected override void OnMinimumChanged(T? oldValue, T? newValue)
        {
            base.OnMinimumChanged(oldValue, newValue);

            if (Value.HasValue && ClipValueToMinMax)
            {
                Value = CoerceValueMinMax(Value.Value);
            }
        }

        protected override void OnMaximumChanged(T? oldValue, T? newValue)
        {
            base.OnMaximumChanged(oldValue, newValue);

            if (Value.HasValue && ClipValueToMinMax)
            {
                Value = CoerceValueMinMax(Value.Value);
            }
        }

        protected override T? ConvertTextToValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            // Since the conversion from Value to text using a FormartString may not be parsable,
            // we verify that the already existing text is not the exact same value.
            var currentValueText = ConvertValueToText();
            if (Equals(currentValueText, text))
            {
                IsInvalid = false;
                return Value;
            }

            var result = ConvertTextToValueCore(currentValueText, text);

            if (ClipValueToMinMax)
            {
                return GetClippedMinMaxValue(result);
            }

            ValidateDefaultMinMax(result);

            return result;
        }

        protected override string ConvertValueToText()
        {
            if (Value == null)
                return string.Empty;

            IsInvalid = false;

            //Manage FormatString of type "{}{0:N2} °" (in xaml) or "{0:N2} °" in code-behind.
            if (FormatString.Contains("{0"))
                return string.Format(CultureInfo, FormatString, Value.Value);

            return Value.Value.ToString(FormatString, CultureInfo);
        }

        protected override void SetValidSpinDirection()
        {
            var validDirections = ValidSpinDirections.None;

            // Null increment always prevents spin.
            if (Increment != null && !IsReadOnly)
            {
                if (IsLowerThan(Value, Maximum) || !Value.HasValue || !Maximum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Increase;

                if (IsGreaterThan(Value, Minimum) || !Value.HasValue || !Minimum.HasValue)
                    validDirections = validDirections | ValidSpinDirections.Decrease;
            }

            if (Spinner != null)
                Spinner.ValidSpinDirection = validDirections;
        }

        private bool IsPercent(string stringToTest)
        {
            var pIndex = stringToTest.IndexOf("P", StringComparison.Ordinal);
            if (pIndex >= 0)
            {
                //stringToTest contains a "P" between 2 "'", it's considered as text, not percent
                var isText = stringToTest.Substring(0, pIndex).Contains("'")
                              && stringToTest.Substring(pIndex, FormatString.Length - pIndex).Contains("'");

                return !isText;
            }
            return false;
        }

        private T? ConvertTextToValueCore(string currentValueText, string text)
        {
            T? result;

            if (IsPercent(FormatString))
            {
                result = _fromDecimal(ParsePercent(text, CultureInfo));
            }
            else
            {
                // Problem while converting new text
                if (!_fromText(text, ParsingNumberStyle, CultureInfo, out var outputValue))
                {
                    var shouldThrow = true;

                    // case 164198: Throw when replacing only the digit part of 99° through UI.
                    // Check if CurrentValueText is also failing => it also contains special characters. ex : 90°
                    if (!_fromText(currentValueText, ParsingNumberStyle, CultureInfo, out _))
                    {
                        // extract non-digit characters
                        var currentValueTextSpecialCharacters = currentValueText.Where(c => !char.IsDigit(c));
                        var valueTextSpecialCharacters = currentValueTextSpecialCharacters.ToList();
                        if (valueTextSpecialCharacters.Any())
                        {
                            var textSpecialCharacters = text.Where(c => !char.IsDigit(c));
                            // same non-digit characters on currentValueText and new text => remove them on new Text to parse it again.
                            var specialCharacters = textSpecialCharacters.ToList();
                            if (valueTextSpecialCharacters.Except(specialCharacters).ToList().Count == 0)
                            {
                                foreach (var character in specialCharacters)
                                {
                                    text = text.Replace(character.ToString(), string.Empty);
                                }
                                // if without the special characters, parsing is good, do not throw
                                if (_fromText(text, ParsingNumberStyle, CultureInfo, out outputValue))
                                {
                                    shouldThrow = false;
                                }
                            }
                        }
                    }

                    if (shouldThrow)
                    {
                        IsInvalid = true;
                        throw new InvalidDataException("Input string was not in a correct format.");
                    }
                }
                result = outputValue;
            }
            return result;
        }

        private T? GetClippedMinMaxValue(T? result)
        {
            if (IsGreaterThan(result, Maximum))
                return Maximum;
            if (IsLowerThan(result, Minimum))
                return Minimum;
            return result;
        }

        private void ValidateDefaultMinMax(T? value)
        {
            // DefaultValue is always accepted.
            if (Equals(value, DefaultValue))
                return;

            if (IsLowerThan(value, Minimum))
                throw new ArgumentOutOfRangeException(nameof(value), $@"Value must be greater than MinValue of {Minimum}");
            if (IsGreaterThan(value, Maximum))
                throw new ArgumentOutOfRangeException(nameof(value), $@"Value must be less than MaxValue of {Maximum}");
        }

        #endregion //Base Class Overrides

        #region Abstract Methods

        protected abstract T IncrementValue(T value, T increment);

        protected abstract T DecrementValue(T value, T increment);

        #endregion
    }
}