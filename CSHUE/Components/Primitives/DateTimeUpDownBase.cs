using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CSHUE.Components.DateTimeUpDown;

namespace CSHUE.Components.Primitives
{
    public abstract class DateTimeUpDownBase<T> : UpDownBase<T>
    {
        #region Members

        internal List<DateTimeInfo> DateTimeInfoList = new List<DateTimeInfo>();
        internal DateTimeInfo SelectedDateTimeInfo;
        internal bool FireSelectionChangedEvent = true;
        internal bool ProcessTextChanged = true;

        #endregion //Members

        #region Properties

        #region CurrentDateTimePart

        public static readonly DependencyProperty CurrentDateTimePartProperty = DependencyProperty.Register("CurrentDateTimePart", typeof(DateTimePart)
          , typeof(DateTimeUpDownBase<T>), new UIPropertyMetadata(DateTimePart.Other, OnCurrentDateTimePartChanged));
        public DateTimePart CurrentDateTimePart
        {
            get => (DateTimePart)GetValue(CurrentDateTimePartProperty);
            set => SetValue(CurrentDateTimePartProperty, value);
        }

        private static void OnCurrentDateTimePartChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is DateTimeUpDownBase<T> dateTimeUpDownBase)
                dateTimeUpDownBase.OnCurrentDateTimePartChanged((DateTimePart)e.OldValue, (DateTimePart)e.NewValue);
        }

        protected virtual void OnCurrentDateTimePartChanged(DateTimePart oldValue, DateTimePart newValue)
        {
            Select(GetDateTimeInfo(newValue));
        }

        #endregion //CurrentDateTimePart

        #region Step

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(int)
          , typeof(DateTimeUpDownBase<T>), new UIPropertyMetadata(1, OnStepChanged));
        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        private static void OnStepChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is DateTimeUpDownBase<T> dateTimeUpDownBase)
                dateTimeUpDownBase.OnStepChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual void OnStepChanged(int oldValue, int newValue)
        {
        }

        #endregion //Step

        #endregion

        #region Constructors

        internal DateTimeUpDownBase()
        {
            Loaded += DateTimeUpDownBase_Loaded;
        }

        #endregion

        #region BaseClass Overrides

        public override void OnApplyTemplate()
        {
            if (TextBox != null)
            {
                TextBox.SelectionChanged -= TextBox_SelectionChanged;
            }

            base.OnApplyTemplate();

            if (TextBox != null)
            {
                TextBox.SelectionChanged += TextBox_SelectionChanged;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var selectionStart = SelectedDateTimeInfo?.StartPosition ?? 0;
            var selectionLength = SelectedDateTimeInfo?.Length ?? 0;

            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (!IsReadOnly)
                        {
                            FireSelectionChangedEvent = false;
                            var binding = BindingOperations.GetBindingExpression(TextBox, System.Windows.Controls.TextBox.TextProperty);
                            binding?.UpdateSource();
                            FireSelectionChangedEvent = true;
                        }
                        return;
                    }
                case Key.Add:
                    if (AllowSpin && !IsReadOnly)
                    {
                        DoIncrement();
                        e.Handled = true;
                    }
                    FireSelectionChangedEvent = false;
                    break;
                case Key.Subtract:
                    if (AllowSpin && !IsReadOnly)
                    {
                        DoDecrement();
                        e.Handled = true;
                    }
                    FireSelectionChangedEvent = false;
                    break;
                case Key.Right:
                    if (IsCurrentValueValid())
                    {
                        PerformKeyboardSelection(selectionStart + selectionLength);
                        e.Handled = true;
                    }
                    FireSelectionChangedEvent = false;
                    break;
                case Key.Left:
                    if (IsCurrentValueValid())
                    {
                        PerformKeyboardSelection(selectionStart > 0 ? selectionStart - 1 : 0);
                        e.Handled = true;
                    }
                    FireSelectionChangedEvent = false;
                    break;
                default:
                    {
                        FireSelectionChangedEvent = false;
                        break;
                    }
            }

            base.OnPreviewKeyDown(e);
        }

        #endregion

        #region Event Hanlders

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (FireSelectionChangedEvent)
                PerformMouseSelection();
            else
                FireSelectionChangedEvent = true;
        }

        private void DateTimeUpDownBase_Loaded(object sender, RoutedEventArgs e)
        {
            InitSelection();
        }

        #endregion

        #region Methods

        protected virtual bool IsCurrentValueValid()
        {
            return true;
        }

        protected virtual void PerformMouseSelection()
        {
            var dateTimeInfo = GetDateTimeInfo(TextBox.SelectionStart);
            /*if( (this.TextBox is MaskedTextBox) && ( dateTimeInfo != null) && (dateTimeInfo.Type == DateTimePart.Other) )
            {
              this.Dispatcher.BeginInvoke( DispatcherPriority.Background, new Action( () =>
              {
                // Select the next dateTime part
                this.Select( this.GetDateTimeInfo( dateTimeInfo.StartPosition + dateTimeInfo.Length ) );
              }
              ) );
              return;
            }     */

            Select(dateTimeInfo);
        }

        protected virtual bool IsLowerThan(T value1, T value2)
        {
            return false;
        }

        protected virtual bool IsGreaterThan(T value1, T value2)
        {
            return false;
        }

        internal DateTimeInfo GetDateTimeInfo(int selectionStart)
        {
            return DateTimeInfoList.FirstOrDefault(info =>
                                   info.StartPosition <= selectionStart && selectionStart < info.StartPosition + info.Length);
        }

        internal DateTimeInfo GetDateTimeInfo(DateTimePart part)
        {
            return DateTimeInfoList.FirstOrDefault(info => info.Type == part);
        }

        internal void Select(DateTimeInfo info)
        {
            if (info != null && !info.Equals(SelectedDateTimeInfo) && TextBox != null && !string.IsNullOrEmpty(TextBox.Text))
            {
                FireSelectionChangedEvent = false;
                TextBox.Select(info.StartPosition, info.Length);
                FireSelectionChangedEvent = true;
                SelectedDateTimeInfo = info;
                SetCurrentValue(CurrentDateTimePartProperty, info.Type);
            }
        }

        internal T CoerceValueMinMax(T value)
        {
            if (IsLowerThan(value, Minimum))
                return Minimum;
            if (IsGreaterThan(value, Maximum))
                return Maximum;
            return value;
        }

        internal void ValidateDefaultMinMax(T value)
        {
            // DefaultValue is always accepted.
            if (Equals(value, DefaultValue))
                return;

            if (IsLowerThan(value, Minimum))
                throw new ArgumentOutOfRangeException(nameof(value), $@"Value must be greater than MinValue of {Minimum}");
            if (IsGreaterThan(value, Maximum))
                throw new ArgumentOutOfRangeException(nameof(value), $@"Value must be less than MaxValue of {Maximum}");
        }

        internal T GetClippedMinMaxValue(T value)
        {
            if (IsGreaterThan(value, Maximum))
                return Maximum;
            if (IsLowerThan(value, Minimum))
                return Minimum;
            return value;
        }

        protected internal virtual void PerformKeyboardSelection(int nextSelectionStart)
        {
            TextBox.Focus();

            if (!UpdateValueOnEnterKey)
            {
                CommitInput();
            }

            var selectedDateStartPosition = SelectedDateTimeInfo?.StartPosition ?? 0;
            var direction = nextSelectionStart - selectedDateStartPosition;
            Select(direction > 0 ? GetNextDateTimeInfo(nextSelectionStart) : GetPreviousDateTimeInfo(nextSelectionStart - 1));
        }

        private DateTimeInfo GetNextDateTimeInfo(int nextSelectionStart)
        {
            var nextDateTimeInfo = GetDateTimeInfo(nextSelectionStart) ?? DateTimeInfoList.First();

            var initialDateTimeInfo = nextDateTimeInfo;

            while (nextDateTimeInfo.Type == DateTimePart.Other)
            {
                nextDateTimeInfo = GetDateTimeInfo(nextDateTimeInfo.StartPosition + nextDateTimeInfo.Length) ??
                                   DateTimeInfoList.First();
                if (Equals(nextDateTimeInfo, initialDateTimeInfo))
                    throw new InvalidOperationException("Couldn't find a valid DateTimeInfo.");
            }
            return nextDateTimeInfo;
        }

        private DateTimeInfo GetPreviousDateTimeInfo(int previousSelectionStart)
        {
            var previousDateTimeInfo = GetDateTimeInfo(previousSelectionStart);
            if (previousDateTimeInfo == null)
            {
                if (DateTimeInfoList.Count > 0)
                {
                    previousDateTimeInfo = DateTimeInfoList.Last();
                }
            }

            var initialDateTimeInfo = previousDateTimeInfo;

            while (previousDateTimeInfo != null && previousDateTimeInfo.Type == DateTimePart.Other)
            {
                previousDateTimeInfo = GetDateTimeInfo(previousDateTimeInfo.StartPosition - 1) ?? DateTimeInfoList.Last();
                if (Equals(previousDateTimeInfo, initialDateTimeInfo))
                    throw new InvalidOperationException("Couldn't find a valid DateTimeInfo.");
            }
            return previousDateTimeInfo;
        }

        private void InitSelection()
        {
            if (SelectedDateTimeInfo == null)
            {
                Select(CurrentDateTimePart != DateTimePart.Other ? GetDateTimeInfo(CurrentDateTimePart) : GetDateTimeInfo(0));
            }
        }

        #endregion
    }
}