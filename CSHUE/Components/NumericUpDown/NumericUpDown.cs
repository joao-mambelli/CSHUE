using System;
using System.Globalization;
using System.Windows;
using CSHUE.Components.Primitives;

namespace CSHUE.Components.NumericUpDown
{
    public abstract class NumericUpDown<T> : UpDownBase<T>
    {
        #region Properties

        #region AutoMoveFocus
        
        public static readonly DependencyProperty AutoMoveFocusProperty = DependencyProperty.Register("AutoMoveFocus",
            typeof(bool), typeof(NumericUpDown<T>), new UIPropertyMetadata(false));
        public bool AutoMoveFocus
        {
            get => (bool)GetValue(AutoMoveFocusProperty);
            set => SetValue(AutoMoveFocusProperty, value);
        }

        #endregion AutoMoveFocus

        #region FormatString

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString",
            typeof(string), typeof(NumericUpDown<T>), new UIPropertyMetadata(string.Empty, OnFormatStringChanged));
        public string FormatString
        {
            get => (string)GetValue(FormatStringProperty);
            set => SetValue(FormatStringProperty, value);
        }

        private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is NumericUpDown<T> numericUpDown)
                numericUpDown.OnFormatStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnFormatStringChanged(string oldValue, string newValue)
        {
            if (IsInitialized)
                SyncTextAndValueProperties(false, null);
        }

        #endregion

        #region Increment

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment",
            typeof(T), typeof(NumericUpDown<T>),
            new PropertyMetadata(default(T), OnIncrementChanged, OnCoerceIncrement));
        public T Increment
        {
            get => (T)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        private static void OnIncrementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is NumericUpDown<T> numericUpDown)
                numericUpDown.OnIncrementChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnIncrementChanged(T oldValue, T newValue)
        {
            if (IsInitialized)
                SetValidSpinDirection();
        }

        private static object OnCoerceIncrement(DependencyObject d, object baseValue)
        {
            if (d is NumericUpDown<T> numericUpDown)
                return numericUpDown.OnCoerceIncrement((T)baseValue);

            return baseValue;
        }

        protected virtual T OnCoerceIncrement(T baseValue)
        {
            return baseValue;
        }

        #endregion

        #region MaxLength

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(NumericUpDown<T>), new UIPropertyMetadata(0));
        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        #endregion

        #endregion

        #region Overrides


        #endregion

        #region Methods

        protected static decimal ParsePercent(string text, IFormatProvider cultureInfo)
        {
            var info = NumberFormatInfo.GetInstance(cultureInfo);

            text = text.Replace(info.PercentSymbol, null);

            var result = decimal.Parse(text, NumberStyles.Any, info) / 100;

            return result;
        }

        #endregion
    }
}