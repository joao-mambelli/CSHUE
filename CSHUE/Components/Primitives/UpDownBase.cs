using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using CSHUE.Components.ButtonSpinner;
using CSHUE.Components.Core.Input;

namespace CSHUE.Components.Primitives
{
    [TemplatePart(Name = PartTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PartSpinner, Type = typeof(Spinner))]
    public abstract class UpDownBase<T> : InputBase, IValidateInput
    {
        #region Members

        /// <summary>
        /// Name constant for Text template part.
        /// </summary>
        internal const string PartTextBox = "PART_TextBox";

        /// <summary>
        /// Name constant for Spinner template part.
        /// </summary>
        internal const string PartSpinner = "PART_Spinner";

        internal bool IsTextChangedFromUi;

        /// <summary>
        /// Flags if the Text and Value properties are in the process of being sync'd
        /// </summary>
        private bool _isSyncingTextAndValueProperties;
        private bool _internalValueSet;

        #endregion //Members

        #region Properties

        protected Spinner Spinner
        {
            get;
            private set;
        }
        protected TextBox TextBox
        {
            get;
            private set;
        }

        #region AllowSpin

        public static readonly DependencyProperty AllowSpinProperty = DependencyProperty.Register("AllowSpin", typeof(bool), typeof(UpDownBase<T>),

    new UIPropertyMetadata(true));
        public bool AllowSpin
        {
            get => (bool)GetValue(AllowSpinProperty);
            set => SetValue(AllowSpinProperty, value);
        }

        #endregion //AllowSpin

        #region ButtonSpinnerLocation

        public static readonly DependencyProperty ButtonSpinnerLocationProperty = DependencyProperty.Register("ButtonSpinnerLocation", typeof(Location

    ), typeof(UpDownBase<T>), new UIPropertyMetadata(Location.Right));
        public Location ButtonSpinnerLocation
        {
            get => (Location)GetValue(ButtonSpinnerLocationProperty);
            set => SetValue(ButtonSpinnerLocationProperty, value);
        }

        #endregion //ButtonSpinnerLocation

        #region ClipValueToMinMax

        public static readonly DependencyProperty ClipValueToMinMaxProperty = DependencyProperty.Register("ClipValueToMinMax", typeof(bool), typeof(

    UpDownBase<T>), new UIPropertyMetadata(false));
        public bool ClipValueToMinMax
        {
            get => (bool)GetValue(ClipValueToMinMaxProperty);
            set => SetValue(ClipValueToMinMaxProperty, value);
        }

        #endregion //ClipValueToMinMax

        #region DisplayDefaultValueOnEmptyText

        public static readonly DependencyProperty DisplayDefaultValueOnEmptyTextProperty = DependencyProperty.Register("DisplayDefaultValueOnEmptyText",

    typeof(bool), typeof(UpDownBase<T>), new UIPropertyMetadata(false, OnDisplayDefaultValueOnEmptyTextChanged));
        public bool DisplayDefaultValueOnEmptyText
        {
            get => (bool)GetValue(DisplayDefaultValueOnEmptyTextProperty);
            set => SetValue(DisplayDefaultValueOnEmptyTextProperty, value);
        }

        private static void OnDisplayDefaultValueOnEmptyTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((UpDownBase<T>)source).OnDisplayDefaultValueOnEmptyTextChanged();
        }

        private void OnDisplayDefaultValueOnEmptyTextChanged()
        {
            if (IsInitialized && string.IsNullOrEmpty(Text))
            {
                SyncTextAndValueProperties(false, Text);
            }
        }

        #endregion //DisplayDefaultValueOnEmptyText

        #region DefaultValue

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(T), typeof(UpDownBase<T>

    ), new UIPropertyMetadata(default(T), OnDefaultValueChanged));
        public T DefaultValue
        {
            get => (T)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        private static void OnDefaultValueChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            ((UpDownBase<T>)source).OnDefaultValueChanged();
        }

        private void OnDefaultValueChanged()
        {
            if (IsInitialized && string.IsNullOrEmpty(Text))
            {
                SyncTextAndValueProperties(true, Text);
            }
        }

        #endregion //DefaultValue

        #region Maximum

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(T), typeof(UpDownBase<T>), new

    UIPropertyMetadata(default(T), OnMaximumChanged, OnCoerceMaximum));
        public T Maximum
        {
            get => (T)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is UpDownBase<T> upDown)
                upDown.OnMaximumChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnMaximumChanged(T oldValue, T newValue)
        {
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceMaximum(DependencyObject d, object baseValue)
        {
            if (d is UpDownBase<T> upDown)
                return upDown.OnCoerceMaximum((T)baseValue);

            return baseValue;
        }

        protected virtual T OnCoerceMaximum(T baseValue)
        {
            return baseValue;
        }

        #endregion //Maximum

        #region Minimum

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(T), typeof(UpDownBase<T>), new

    UIPropertyMetadata(default(T), OnMinimumChanged, OnCoerceMinimum));
        public T Minimum
        {
            get => (T)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is UpDownBase<T> upDown)
                upDown.OnMinimumChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnMinimumChanged(T oldValue, T newValue)
        {
            if (IsInitialized)
            {
                SetValidSpinDirection();
            }
        }

        private static object OnCoerceMinimum(DependencyObject d, object baseValue)
        {
            if (d is UpDownBase<T> upDown)
                return upDown.OnCoerceMinimum((T)baseValue);

            return baseValue;
        }

        protected virtual T OnCoerceMinimum(T baseValue)
        {
            return baseValue;
        }

        #endregion //Minimum

        #region MouseWheelActiveTrigger

        /// <summary>
        /// Identifies the MouseWheelActiveTrigger dependency property
        /// </summary>
        public static readonly DependencyProperty MouseWheelActiveTriggerProperty = DependencyProperty.Register("MouseWheelActiveTrigger", typeof(

    MouseWheelActiveTrigger), typeof(UpDownBase<T>), new UIPropertyMetadata(MouseWheelActiveTrigger.FocusedMouseOver));

        /// <summary>
        /// Get or set when the mouse wheel event should affect the value.
        /// </summary>
        public MouseWheelActiveTrigger MouseWheelActiveTrigger
        {
            get => (MouseWheelActiveTrigger)GetValue(MouseWheelActiveTriggerProperty);
            set => SetValue(MouseWheelActiveTriggerProperty, value);
        }

        #endregion //MouseWheelActiveTrigger

        #region MouseWheelActiveOnFocus

        [Obsolete("Use MouseWheelActiveTrigger property instead")]
        public static readonly DependencyProperty MouseWheelActiveOnFocusProperty = DependencyProperty.Register("MouseWheelActiveOnFocus", typeof(bool

    ), typeof(UpDownBase<T>), new UIPropertyMetadata(true, OnMouseWheelActiveOnFocusChanged));

        [Obsolete("Use MouseWheelActiveTrigger property instead")]
        public bool MouseWheelActiveOnFocus
        {
            get
            {
#pragma warning disable 618
                return (bool)GetValue(MouseWheelActiveOnFocusProperty);
#pragma warning restore 618
            }
            set
            {
#pragma warning disable 618
                SetValue(MouseWheelActiveOnFocusProperty, value);
#pragma warning restore 618
            }
        }

        private static void OnMouseWheelActiveOnFocusChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is UpDownBase<T> upDownBase)
                upDownBase.MouseWheelActiveTrigger = (bool)e.NewValue
                  ? MouseWheelActiveTrigger.FocusedMouseOver
                  : MouseWheelActiveTrigger.MouseOver;
        }

        #endregion //MouseWheelActiveOnFocus

        #region ShowButtonSpinner

        public static readonly DependencyProperty ShowButtonSpinnerProperty = DependencyProperty.Register("ShowButtonSpinner", typeof(bool), typeof(

    UpDownBase<T>), new UIPropertyMetadata(true));
        public bool ShowButtonSpinner
        {
            get => (bool)GetValue(ShowButtonSpinnerProperty);
            set => SetValue(ShowButtonSpinnerProperty, value);
        }

        #endregion //ShowButtonSpinner

        #region UpdateValueOnEnterKey

        public static readonly DependencyProperty UpdateValueOnEnterKeyProperty = DependencyProperty.Register("UpdateValueOnEnterKey", typeof(bool), typeof(UpDownBase<T>),
          new FrameworkPropertyMetadata(false, OnUpdateValueOnEnterKeyChanged));
        public bool UpdateValueOnEnterKey
        {
            get => (bool)GetValue(UpdateValueOnEnterKeyProperty);
            set => SetValue(UpdateValueOnEnterKeyProperty, value);
        }

        private static void OnUpdateValueOnEnterKeyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is UpDownBase<T> upDownBase)
                upDownBase.OnUpdateValueOnEnterKeyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnUpdateValueOnEnterKeyChanged(bool oldValue, bool newValue)
        {
        }

        #endregion //UpdateValueOnEnterKey

        #region Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(T), typeof(UpDownBase<T>),
          new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, OnCoerceValue, false, UpdateSourceTrigger.PropertyChanged));
        public T Value
        {
            get => (T)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private void SetValueInternal(T value)
        {
            _internalValueSet = true;
            try
            {
                Value = value;
            }
            finally
            {
                _internalValueSet = false;
            }
        }

        private static object OnCoerceValue(DependencyObject o, object basevalue)
        {
            return ((UpDownBase<T>)o).OnCoerceValue(basevalue);
        }

        protected virtual object OnCoerceValue(object newValue)
        {
            return newValue;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is UpDownBase<T> upDownBase)
                upDownBase.OnValueChanged((T)e.OldValue, (T)e.NewValue);
        }

        protected virtual void OnValueChanged(T oldValue, T newValue)
        {
            if (!_internalValueSet && IsInitialized)
            {
                SyncTextAndValueProperties(false, null, true);
            }

            SetValidSpinDirection();

            RaiseValueChangedEvent(oldValue, newValue);
        }

        #endregion //Value

        #endregion //Properties

        #region Constructors

        internal UpDownBase()
        {

            AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new RoutedEventHandler(HandleClickOutsideOfControlWithMouseCapture),

      true);
            IsKeyboardFocusWithinChanged += UpDownBase_IsKeyboardFocusWithinChanged;
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            TextBox?.Focus();

            base.OnAccessKey(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (TextBox != null)
            {
                TextBox.TextChanged -= TextBox_TextChanged;
                TextBox.RemoveHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(TextBox_PreviewMouseDown));
            }

            TextBox = GetTemplateChild(PartTextBox) as TextBox;

            if (TextBox != null)
            {
                TextBox.Text = Text;
                TextBox.TextChanged += TextBox_TextChanged;
                TextBox.AddHandler(Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(TextBox_PreviewMouseDown), true);
            }

            if (Spinner != null)
                Spinner.Spin -= OnSpinnerSpin;

            Spinner = GetTemplateChild(PartSpinner) as Spinner;

            if (Spinner != null)
                Spinner.Spin += OnSpinnerSpin;

            SetValidSpinDirection();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        // Commit Text on "Enter" to raise Error event 
                        var commitSuccess = CommitInput();
                        //Only handle if an exception is detected (Commit fails)
                        e.Handled = !commitSuccess;
                        break;
                    }
            }
        }

        protected override void OnTextChanged(string oldValue, string newValue)
        {
            if (IsInitialized)
            {
                // When text is typed, if UpdateValueOnEnterKey is true, 
                // Sync Value on Text only when Enter Key is pressed.
                if (UpdateValueOnEnterKey)
                {
                    if (!IsTextChangedFromUi)
                    {
                        SyncTextAndValueProperties(true, Text);
                    }
                }
                else
                {
                    SyncTextAndValueProperties(true, Text);
                }
            }
        }

        protected override void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            if (IsInitialized)
            {
                SyncTextAndValueProperties(false, null);
            }
        }

        protected override void OnReadOnlyChanged(bool oldValue, bool newValue)
        {
            SetValidSpinDirection();
        }

        #endregion //Base Class Overrides

        #region Event Handlers

        private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            if (MouseWheelActiveTrigger == MouseWheelActiveTrigger.Focused)
            {
                //Capture the spinner when user clicks on the control.
                if (!Equals(Mouse.Captured, Spinner))
                {
                    //Delay the capture to let the DateTimeUpDown select a new DateTime part.
                    Dispatcher?.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                  {
                      Mouse.Capture(Spinner);
                  }
                    ));
                }
            }
        }

        private void HandleClickOutsideOfControlWithMouseCapture(object sender, RoutedEventArgs e)
        {
            if (Mouse.Captured is Spinner)
            {
                //Release the captured spinner when user clicks away from spinner.
                Spinner.ReleaseMouseCapture();
            }
        }

        private void OnSpinnerSpin(object sender, SpinEventArgs e)
        {
            if (AllowSpin && !IsReadOnly)
            {
                var activeTrigger = MouseWheelActiveTrigger;
                var spin = !e.UsingMouseWheel;
                spin |= activeTrigger == MouseWheelActiveTrigger.MouseOver;
                spin |= TextBox != null && TextBox.IsFocused && activeTrigger == MouseWheelActiveTrigger.FocusedMouseOver;
                spin |= TextBox != null && TextBox.IsFocused && activeTrigger == MouseWheelActiveTrigger.Focused && Mouse.Captured is Spinner;

                if (spin)
                {
                    e.Handled = true;
                    OnSpin(e);
                }
            }
        }

        #endregion //Event Handlers

        #region Events

        public event InputValidationErrorEventHandler InputValidationError;

        public event EventHandler<SpinEventArgs> Spinned;

        #region ValueChanged Event

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UpDownBase<T>));
        public event RoutedEventHandler ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        #endregion

        #endregion //Events

        #region Methods

        protected virtual void OnSpin(SpinEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            // Raise the Spinned event to user
            var handler = Spinned;
            handler?.Invoke(this, e);

            if (e.Direction == SpinDirection.Increase)
                DoIncrement();
            else
                DoDecrement();
        }

        protected virtual void RaiseValueChangedEvent(T oldValue, T newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue) { RoutedEvent = ValueChangedEvent };
            RaiseEvent(args);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            // When both Value and Text are initialized, Value has priority.
            // To be sure that the value is not initialized, it should
            // have no local value, no binding, and equal to the default value.
            var updateValueFromText =
              ReadLocalValue(ValueProperty) == DependencyProperty.UnsetValue
              && BindingOperations.GetBinding(this, ValueProperty) == null
              && Equals(Value, ValueProperty.DefaultMetadata.DefaultValue);

            SyncTextAndValueProperties(updateValueFromText, Text, !updateValueFromText);
        }

        /// <summary>
        /// Performs an increment if conditions allow it.
        /// </summary>
        internal void DoDecrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
            {
                OnDecrement();
            }
        }

        /// <summary>
        /// Performs a decrement if conditions allow it.
        /// </summary>
        internal void DoIncrement()
        {
            if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
            {
                OnIncrement();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
                return;

            try
            {
                IsTextChangedFromUi = true;
                Text = ((TextBox)sender).Text;
            }
            finally
            {
                IsTextChangedFromUi = false;
            }
        }

        private void UpDownBase_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                CommitInput();
            }
        }

        private void RaiseInputValidationError(Exception e)
        {
            if (InputValidationError != null)
            {
                var args = new InputValidationErrorEventArgs(e);
                InputValidationError(this, args);
                if (args.ThrowException)
                {
                    throw args.Exception;
                }
            }
        }

        public virtual bool CommitInput()
        {
            return SyncTextAndValueProperties(true, Text);
        }

        protected bool SyncTextAndValueProperties(bool updateValueFromText, string text)
        {
            return SyncTextAndValueProperties(updateValueFromText, text, false);
        }

        private bool SyncTextAndValueProperties(bool updateValueFromText, string text, bool forceTextUpdate)
        {
            if (_isSyncingTextAndValueProperties)
                return true;

            _isSyncingTextAndValueProperties = true;
            var parsedTextIsValid = true;
            try
            {
                if (updateValueFromText)
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        // An empty input sets the value to the default value.
                        SetValueInternal(DefaultValue);
                    }
                    else
                    {
                        try
                        {
                            var newValue = ConvertTextToValue(text);
                            if (!Equals(newValue, Value))
                            {
                                SetValueInternal(newValue);
                            }
                        }
                        catch (Exception e)
                        {
                            parsedTextIsValid = false;

                            // From the UI, just allow any input.
                            if (!IsTextChangedFromUi)
                            {
                                // This call may throw an exception. 
                                // See RaiseInputValidationError() implementation.
                                RaiseInputValidationError(e);
                            }
                        }
                    }
                }

                // Do not touch the ongoing text input from user.
                if (!IsTextChangedFromUi)
                {
                    // Don't replace the empty Text with the non-empty representation of DefaultValue.
                    var shouldKeepEmpty = !forceTextUpdate && string.IsNullOrEmpty(Text) && Equals(Value, DefaultValue) && !DisplayDefaultValueOnEmptyText;
                    if (!shouldKeepEmpty)
                    {
                        var newText = ConvertValueToText();
                        if (!Equals(Text, newText))
                        {
                            Text = newText;
                        }
                    }

                    // Sync Text and textBox
                    if (TextBox != null)
                        TextBox.Text = Text;
                }

                if (IsTextChangedFromUi && !parsedTextIsValid)
                {
                    // Text input was made from the user and the text
                    // repesents an invalid value. Disable the spinner
                    // in this case.
                    if (Spinner != null)
                    {
                        Spinner.ValidSpinDirection = ValidSpinDirections.None;
                    }
                }
                else
                {
                    SetValidSpinDirection();
                }
            }
            finally
            {
                _isSyncingTextAndValueProperties = false;
            }
            return parsedTextIsValid;
        }

        #region Abstract

        /// <summary>
        /// Converts the formatted text to a value.
        /// </summary>
        protected abstract T ConvertTextToValue(string text);

        /// <summary>
        /// Converts the value to formatted text.
        /// </summary>
        /// <returns></returns>
        protected abstract string ConvertValueToText();

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Increase.
        /// </summary>
        protected abstract void OnIncrement();

        /// <summary>
        /// Called by OnSpin when the spin direction is SpinDirection.Descrease.
        /// </summary>
        protected abstract void OnDecrement();

        /// <summary>
        /// Sets the valid spin directions.
        /// </summary>
        protected abstract void SetValidSpinDirection();

        #endregion //Abstract

        #endregion //Methods
    }
}