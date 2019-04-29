using System.Linq;
using System.Windows;
using System.Windows.Media;
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for LightSettingCell.xaml
    /// </summary>
    public partial class LightSettingCell
    {
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LightSettingCell));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(LightSettingCell));

        public byte Brightness
        {
            get => (byte)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(byte), typeof(LightSettingCell));

        public bool OnlyBrightness
        {
            get => (bool)GetValue(OnlyBrightnessProperty);
            set => SetValue(OnlyBrightnessProperty, value);
        }
        public static readonly DependencyProperty OnlyBrightnessProperty =
            DependencyProperty.Register("OnlyBrightness", typeof(bool), typeof(LightSettingCell));

        public Visibility OnlyBrightnessVisibility
        {
            get => (Visibility)GetValue(OnlyBrightnessVisibilityProperty);
            set => SetValue(OnlyBrightnessVisibilityProperty, value);
        }
        public static readonly DependencyProperty OnlyBrightnessVisibilityProperty =
            DependencyProperty.Register("OnlyBrightnessVisibility", typeof(Visibility), typeof(LightSettingCell),
                new UIPropertyMetadata(Visibility.Collapsed));

        public string MainEventText
        {
            get => (string)GetValue(MainEventTextProperty);
            set => SetValue(MainEventTextProperty, value);
        }
        public static readonly DependencyProperty MainEventTextProperty =
            DependencyProperty.Register("MainEventText", typeof(string), typeof(LightSettingCell));

        public LightSettingCell()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            new ColorPicker
            {
                Text1 = Cultures.Resources.Ok,
                Text2 = Cultures.Resources.Cancel,
                Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
            }.ShowDialog();
        }
    }
}
