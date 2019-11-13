using System;
using System.Windows;
using System.Windows.Controls;

namespace CSHUE.Components.ButtonSpinner
{
    public abstract class Spinner : Control
    {
        #region Properties

        public static readonly DependencyProperty ValidSpinDirectionProperty =
            DependencyProperty.Register("ValidSpinDirection", typeof(ValidSpinDirections), typeof(Spinner),
                new PropertyMetadata(ValidSpinDirections.Increase | ValidSpinDirections.Decrease,
                    OnValidSpinDirectionPropertyChanged));
        public ValidSpinDirections ValidSpinDirection
        {
            get => (ValidSpinDirections)GetValue(ValidSpinDirectionProperty);
            set => SetValue(ValidSpinDirectionProperty, value);
        }

        private static void OnValidSpinDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (Spinner)d;
            var oldvalue = (ValidSpinDirections)e.OldValue;
            var newvalue = (ValidSpinDirections)e.NewValue;
            source.OnValidSpinDirectionChanged(oldvalue, newvalue);
        }

        #endregion

        public event EventHandler<SpinEventArgs> Spin;

        #region Events

        public static readonly RoutedEvent SpinnerSpinEvent = EventManager.RegisterRoutedEvent("SpinnerSpin",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Spinner));
        public event RoutedEventHandler SpinnerSpin
        {
            add => AddHandler(SpinnerSpinEvent, value);
            remove => RemoveHandler(SpinnerSpinEvent, value);
        }

        #endregion

        protected virtual void OnSpin(SpinEventArgs e)
        {
            var valid = e.Direction == SpinDirection.Increase ? ValidSpinDirections.Increase : ValidSpinDirections.Decrease;

            if ((ValidSpinDirection & valid) == valid)
            {
                var handler = Spin;
                handler?.Invoke(this, e);
            }
        }

        protected virtual void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
            // ignored
        }
    }
}