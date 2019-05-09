using System.Windows;
using System.Windows.Media;

// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for LightStateCell.xaml
    /// </summary>
    public partial class LightStateCell
    {
        #region Properties

        public string UniqueId { get; set; }

        #endregion

        #region Dependency Properties

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LightStateCell));

        public bool On
        {
            get => (bool)GetValue(OnProperty);
            set => SetValue(OnProperty, value);
        }
        public static readonly DependencyProperty OnProperty =
            DependencyProperty.Register("On", typeof(bool), typeof(LightStateCell));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(LightStateCell));

        public double Brightness
        {
            get => (double)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(LightStateCell));

        #endregion

        #region Initializers

        public LightStateCell()
        {
            InitializeComponent();
        }

        #endregion
    }
}
