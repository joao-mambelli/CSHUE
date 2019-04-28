using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CSHUE.Helpers;
using CSHUE.ViewModels;
using Q42.HueApi;
using System.Windows.Media;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for LightSelector.xaml
    /// </summary>
    public partial class LightSelector
    {
        public LightSelector()
        {
            InitializeComponent();
        }

        public List<Light> AllLights { get; set; }

        public EventProperty Property { get; set; } = null;

        public EventBrightnessProperty BrightnessProperty { get; set; } = null;

        public List<LightSelectorViewModel> List
        {
            get => (List<LightSelectorViewModel>)GetValue(ListProperty);
            set => SetValue(ListProperty, value);
        }
        public static readonly DependencyProperty ListProperty =
            DependencyProperty.Register("List", typeof(List<LightSelectorViewModel>), typeof(LightSelector));

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (Property != null)
            {
                foreach (var l in Property.Lights)
                {
                    l.Brightness = List.Find(x => x.UniqueId == l.Id).Content.Brightness;
                    l.Color = List.Find(x => x.UniqueId == l.Id).Content.Color;
                }

                Property.SelectedLights = new List<string>();
                foreach (var c in List)
                {
                    if (!c.IsChecked) continue;

                    Property.SelectedLights.Add(c.UniqueId);
                }
            }
            else if (BrightnessProperty != null)
            {
                foreach (var l in BrightnessProperty.Lights)
                {
                    l.Brightness = List.Find(x => x.UniqueId == l.Id).Content.Brightness;
                    l.Color = List.Find(x => x.UniqueId == l.Id).Content.Color;
                    l.OnlyBrightness = List.Find(x => x.UniqueId == l.Id).Content.OnlyBrightness;
                }

                BrightnessProperty.SelectedLights = new List<string>();
                foreach (var c in List)
                {
                    if (!c.IsChecked) continue;
                    
                    BrightnessProperty.SelectedLights.Add(c.UniqueId);
                }
            }
            
            DialogResult = true;
            Close();
        }

        private bool _loadDone;
        private void LightSelector_OnLoaded(object sender, RoutedEventArgs e)
        {
            List = new List<LightSelectorViewModel>();

            foreach (var l in AllLights)
            {
                if (Property != null)
                {
                    List.Add(new LightSelectorViewModel
                    {
                        Content = new ColorChooser
                        {
                            Text = l.Name,
                            Color = Color.FromRgb(Property.Lights.Find(x => x.Id == l.UniqueId).Color.Red,
                                Property.Lights.Find(x => x.Id == l.UniqueId).Color.Green,
                                Property.Lights.Find(x => x.Id == l.UniqueId).Color.Blue),
                            Brightness = Property.Lights.Find(x => x.Id == l.UniqueId).Brightness
                        },
                        UniqueId = l.UniqueId,
                        IsChecked = Property.SelectedLights.Any(x => x == l.UniqueId),
                    });
                }
                else if (BrightnessProperty != null)
                {
                    var mainEvent = "";
                    var singleOption = false;
                    if (BrightnessProperty == Properties.Settings.Default.PlayerGetsKill)
                        mainEvent = Cultures.Resources.Current;
                    if (BrightnessProperty == Properties.Settings.Default.PlayerGetsKilled)
                        mainEvent = Cultures.Resources.Current;
                    if (BrightnessProperty == Properties.Settings.Default.FreezeTime)
                        mainEvent = Cultures.Resources.RoundStarts;
                    if (BrightnessProperty == Properties.Settings.Default.Warmup)
                        mainEvent = Cultures.Resources.RoundStarts;
                    if (BrightnessProperty == Properties.Settings.Default.BombExplodes)
                        mainEvent = Cultures.Resources.BombHasBeenPlanted;
                    if (BrightnessProperty == Properties.Settings.Default.BombBlink)
                    {
                        mainEvent = Cultures.Resources.BombHasBeenPlanted;
                        singleOption = true;
                    }

                    List.Add(new LightSelectorViewModel
                    {
                        Content = new ColorChooser
                        {
                            Text = l.Name,
                            Color = Color.FromRgb(BrightnessProperty.Lights.Find(x => x.Id == l.UniqueId).Color.Red,
                                BrightnessProperty.Lights.Find(x => x.Id == l.UniqueId).Color.Green,
                                BrightnessProperty.Lights.Find(x => x.Id == l.UniqueId).Color.Blue),
                            Brightness = BrightnessProperty.Lights.Find(x => x.Id == l.UniqueId).Brightness,
                            OnlyBrightness = BrightnessProperty.Lights.Find(x => x.Id == l.UniqueId).OnlyBrightness,
                            OnlyBrightnessVisibility = Visibility.Visible,
                            MainEventText = string.Format(Cultures.Resources.UseMainEventColor, (mainEvent != Cultures.Resources.Current ? "\"" : "") + mainEvent + (mainEvent != Cultures.Resources.Current ? "\"" : ""))
                        },
                        UniqueId = l.UniqueId,
                        IsChecked = BrightnessProperty.SelectedLights.Any(x => x == l.UniqueId),
                        SingleOptionVisibility = singleOption ? Visibility.Visible : Visibility.Collapsed
                    });
                }
            }

            _loadDone = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool _radioButtonOnChanged;
        private void RadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (((RadioButton) sender).IsChecked == true && !_radioButtonOnChanged && _loadDone)
            {
                ((RadioButton) sender).IsChecked = false;
            }

            _radioButtonOnChanged = false;
        }

        private void RadioButton_OnChanged(object sender, RoutedEventArgs e)
        {
            _radioButtonOnChanged = true;
        }
    }
}
