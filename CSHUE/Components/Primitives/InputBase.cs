using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CSHUE.Components.Primitives
{
    public abstract class InputBase : Control
    {
        #region Properties

        #region AllowTextInput

        public static readonly DependencyProperty AllowTextInputProperty = DependencyProperty.Register("AllowTextInput",
            typeof(bool), typeof(InputBase), new UIPropertyMetadata(true, OnAllowTextInputChanged));
        public bool AllowTextInput
        {
            get => (bool)GetValue(AllowTextInputProperty);
            set => SetValue(AllowTextInputProperty, value);
        }

        private static void OnAllowTextInputChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is InputBase inputBase)
                inputBase.OnAllowTextInputChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnAllowTextInputChanged(bool oldValue, bool newValue)
        {
            // ignored
        }

        #endregion

        #region CultureInfo

        public static readonly DependencyProperty CultureInfoProperty = DependencyProperty.Register("CultureInfo",
            typeof(CultureInfo), typeof(InputBase),
            new UIPropertyMetadata(CultureInfo.CurrentCulture, OnCultureInfoChanged));
        public CultureInfo CultureInfo
        {
            get => (CultureInfo)GetValue(CultureInfoProperty);
            set => SetValue(CultureInfoProperty, value);
        }

        private static void OnCultureInfoChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is InputBase inputBase)
                inputBase.OnCultureInfoChanged((CultureInfo)e.OldValue, (CultureInfo)e.NewValue);
        }

        protected virtual void OnCultureInfoChanged(CultureInfo oldValue, CultureInfo newValue)
        {
            // ignored
        }

        #endregion

        #region IsReadOnly

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly",
            typeof(bool), typeof(InputBase), new UIPropertyMetadata(false, OnReadOnlyChanged));
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private static void OnReadOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is InputBase inputBase)
                inputBase.OnReadOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnReadOnlyChanged(bool oldValue, bool newValue)
        {
            // ignored
        }

        #endregion

        #region IsUndoEnabled

        public static readonly DependencyProperty IsUndoEnabledProperty = DependencyProperty.Register("IsUndoEnabled",
            typeof(bool), typeof(InputBase), new UIPropertyMetadata(true, OnIsUndoEnabledChanged));
        public bool IsUndoEnabled
        {
            get => (bool)GetValue(IsUndoEnabledProperty);
            set => SetValue(IsUndoEnabledProperty, value);
        }

        private static void OnIsUndoEnabledChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is InputBase inputBase)
                inputBase.OnIsUndoEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsUndoEnabledChanged(bool oldValue, bool newValue)
        {
            // ignored
        }

        #endregion

        #region Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(InputBase),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnTextChanged, null, false, UpdateSourceTrigger.LostFocus));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is InputBase inputBase)
                inputBase.OnTextChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnTextChanged(string oldValue, string newValue)
        {
            // ignored
        }

        #endregion

        #region TextAlignment

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment",
            typeof(TextAlignment), typeof(InputBase), new UIPropertyMetadata(TextAlignment.Left));
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }


        #endregion

        #region Watermark

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(InputBase), new UIPropertyMetadata(null));
        public object Watermark
        {
            get => GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        #endregion

        #region WatermarkTemplate

        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(InputBase),
                new UIPropertyMetadata(null));
        public DataTemplate WatermarkTemplate
        {
            get => (DataTemplate)GetValue(WatermarkTemplateProperty);
            set => SetValue(WatermarkTemplateProperty, value);
        }

        #endregion

        #endregion
    }
}