using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace CSHUE.Components.ButtonSpinner
{
    public enum Location
    {
        Left,
        Right
    }

    [TemplatePart(Name = PartIncreaseButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PartDecreaseButton, Type = typeof(ButtonBase))]
    [ContentProperty("Content")]
    public class ButtonSpinner : Spinner
    {
        private const string PartIncreaseButton = "PART_IncreaseButton";
        private const string PartDecreaseButton = "PART_DecreaseButton";

        #region Properties

        #region AllowSpin

        public static readonly DependencyProperty AllowSpinProperty = DependencyProperty.Register("AllowSpin",
            typeof(bool), typeof(ButtonSpinner), new UIPropertyMetadata(true, AllowSpinPropertyChanged));
        public bool AllowSpin
        {
            get => (bool)GetValue(AllowSpinProperty);
            set => SetValue(AllowSpinProperty, value);
        }

        private static void AllowSpinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as ButtonSpinner;
            source?.OnAllowSpinChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        #endregion

        #region ButtonSpinnerLocation

        public static readonly DependencyProperty ButtonSpinnerLocationProperty =
            DependencyProperty.Register("ButtonSpinnerLocation", typeof(Location), typeof(ButtonSpinner),
                new UIPropertyMetadata(Location.Right));
        public Location ButtonSpinnerLocation
        {
            get => (Location)GetValue(ButtonSpinnerLocationProperty);
            set => SetValue(ButtonSpinnerLocationProperty, value);
        }

        #endregion

        #region Content

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content",
            typeof(object), typeof(ButtonSpinner), new PropertyMetadata(null, OnContentPropertyChanged));
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as ButtonSpinner;

            source?.OnContentChanged(e.OldValue, e.NewValue);
        }

        #endregion

        #region DecreaseButton

        private ButtonBase _decreaseButton;
        private ButtonBase DecreaseButton
        {
            get => _decreaseButton;
            set
            {
                if (_decreaseButton != null)
                    _decreaseButton.Click -= OnButtonClick;

                _decreaseButton = value;

                if (_decreaseButton != null)
                    _decreaseButton.Click += OnButtonClick;
            }
        }

        #endregion

        #region IncreaseButton

        private ButtonBase _increaseButton;
        private ButtonBase IncreaseButton
        {
            get => _increaseButton;
            set
            {
                if (_increaseButton != null)
                    _increaseButton.Click -= OnButtonClick;

                _increaseButton = value;

                if (_increaseButton != null)
                    _increaseButton.Click += OnButtonClick;
            }
        }

        #endregion

        #region ShowButtonSpinner

        public static readonly DependencyProperty ShowButtonSpinnerProperty =
            DependencyProperty.Register("ShowButtonSpinner", typeof(bool), typeof(ButtonSpinner),
                new UIPropertyMetadata(true));
        public bool ShowButtonSpinner
        {
            get => (bool)GetValue(ShowButtonSpinnerProperty);
            set => SetValue(ShowButtonSpinnerProperty, value);
        }

        #endregion

        #endregion

        #region Constructors

        static ButtonSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonSpinner),
                new FrameworkPropertyMetadata(typeof(ButtonSpinner)));
        }

        #endregion

        #region Base Class Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IncreaseButton = GetTemplateChild(PartIncreaseButton) as ButtonBase;
            DecreaseButton = GetTemplateChild(PartDecreaseButton) as ButtonBase;

            SetButtonUsage();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            Point mousePosition;

            if (IncreaseButton != null && IncreaseButton.IsEnabled == false)
            {
                mousePosition = e.GetPosition(IncreaseButton);

                if (mousePosition.X > 0 && mousePosition.X < IncreaseButton.ActualWidth &&
                    mousePosition.Y > 0 && mousePosition.Y < IncreaseButton.ActualHeight)
                    e.Handled = true;
            }

            if (DecreaseButton != null && DecreaseButton.IsEnabled == false)
            {
                mousePosition = e.GetPosition(DecreaseButton);

                if (mousePosition.X > 0 && mousePosition.X < DecreaseButton.ActualWidth &&
                    mousePosition.Y > 0 && mousePosition.Y < DecreaseButton.ActualHeight)
                    e.Handled = true;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (AllowSpin)
                    {
                        OnSpin(new SpinEventArgs(SpinnerSpinEvent, SpinDirection.Increase));
                        e.Handled = true;
                    }

                    break;
                case Key.Down:
                    if (AllowSpin)
                    {
                        OnSpin(new SpinEventArgs(SpinnerSpinEvent, SpinDirection.Decrease));
                        e.Handled = true;
                    }

                    break;
                case Key.Enter:
                    if (IncreaseButton?.IsFocused == true || DecreaseButton?.IsFocused == true)
                        e.Handled = true;

                    break;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!e.Handled && AllowSpin && e.Delta != 0)
            {
                var spinnerEventArgs = new SpinEventArgs(SpinnerSpinEvent, e.Delta < 0 ? SpinDirection.Decrease : SpinDirection.Increase, true);

                OnSpin(spinnerEventArgs);

                e.Handled = spinnerEventArgs.Handled;
            }
        }

        protected override void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
            SetButtonUsage();
        }

        #endregion

        #region Event Handlers

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (AllowSpin)
            {
                var direction = Equals(sender, IncreaseButton) ? SpinDirection.Increase : SpinDirection.Decrease;
                OnSpin(new SpinEventArgs(SpinnerSpinEvent, direction));
            }
        }

        #endregion

        #region Methods

        protected virtual void OnContentChanged(object oldValue, object newValue)
        {
            // ignored
        }

        protected virtual void OnAllowSpinChanged(bool oldValue, bool newValue)
        {
            SetButtonUsage();
        }

        private void SetButtonUsage()
        {
            if (IncreaseButton != null)
                IncreaseButton.IsEnabled = AllowSpin && (ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase;

            if (DecreaseButton != null)
                DecreaseButton.IsEnabled = AllowSpin && (ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease;
        }

        #endregion
    }
}