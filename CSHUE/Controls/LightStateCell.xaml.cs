using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for LightStateCell.xaml
    /// </summary>
    public partial class LightStateCell
    {
        #region Properties

        /// <summary>
        /// UniqueId of the light.
        /// </summary>
        public string UniqueId { get; set; }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Text to be shown.
        /// </summary>
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LightStateCell));

        /// <summary>
        /// Property that indicates the light is on or off.
        /// </summary>
        public bool On
        {
            get => (bool) GetValue(OnProperty);
            set => SetValue(OnProperty, value);
        }
        public static readonly DependencyProperty OnProperty =
            DependencyProperty.Register("On", typeof(bool), typeof(LightStateCell));

        /// <summary>
        /// Property that represents the current color of the light.
        /// </summary>
        public Color Color
        {
            get => (Color) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(LightStateCell));

        /// <summary>
        /// Property that represents the current brightness of the light.
        /// </summary>
        public double Brightness
        {
            get => (double) GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(LightStateCell),
                new PropertyMetadata((double)0, OnBrightnessPropertyChanged));

        /// <summary>
        /// Auxiliary property to animate the brightness numeric indicator.
        /// </summary>
        public double BrightnessAnimated
        {
            get => (double) GetValue(BrightnessAnimatedProperty);
            set => SetValue(BrightnessAnimatedProperty, value);
        }
        public static readonly DependencyProperty BrightnessAnimatedProperty =
            DependencyProperty.Register("BrightnessAnimated", typeof(double), typeof(LightStateCell));

        #endregion

        #region Initializers

        /// <summary>
        /// Initializer.
        /// </summary>
        public LightStateCell()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Brightness property callback.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnBrightnessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((double) e.OldValue < (double) e.NewValue)
            {
                new Thread(() =>
                    {
                        for (var i = (double) e.OldValue; i < (double) e.NewValue; i = i + 0.05)
                        {
                            var index = i;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ((LightStateCell) d).BrightnessAnimated = index;
                            });
                            Thread.Sleep((int) Math.Round(400 / (((double) e.NewValue - (double) e.OldValue) * 20)));
                        }

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ((LightStateCell) d).BrightnessAnimated = (double) e.NewValue;
                        });
                    })
                { IsBackground = true }.Start();
            }
            else if ((double) e.OldValue > (double) e.NewValue)
            {
                new Thread(() =>
                    {
                        for (var i = (double) e.OldValue; i > (double) e.NewValue; i = i - 0.05)
                        {
                            var index = i;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ((LightStateCell) d).BrightnessAnimated = index;
                            });
                            Thread.Sleep((int) Math.Round(400 / (((double) e.OldValue - (double) e.NewValue) * 20)));
                        }

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ((LightStateCell) d).BrightnessAnimated = (double) e.NewValue;
                        });
                    })
                { IsBackground = true }.Start();
            }
        }

        #endregion
    }
}
