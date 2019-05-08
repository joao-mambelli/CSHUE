using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CSHUE.Helpers;
using CSHUE.ViewModels;
using Q42.HueApi;
using System.Windows.Media;
using CSHUE.Controls;

// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for LightSelector.xaml
    /// </summary>
    public partial class LightSelector
    {
        public LightSelectorViewModel ViewModel = new LightSelectorViewModel();

        private bool _isWindowOpened = true;
        public LightSelector(string title)
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.Title = title;

            new Thread(() =>
            {
                while (_isWindowOpened && Properties.Settings.Default.PreviewLights)
                {
                    if (!ViewModel.IsColorPickerOpened)
                        Dispatcher.Invoke(() =>
                        {
                            ViewModel.SetLightsAsync(ViewModel.List);
                        });

                    Thread.Sleep(500);
                }
            })
            { IsBackground = true }.Start();
        }

        public List<Light> AllLights { get; set; }

        public EventProperty Property { get; set; } = null;

        public EventBrightnessProperty BrightnessProperty { get; set; } = null;

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (Property != null)
            {
                foreach (var l in Property.Lights)
                {
                    l.Brightness = ViewModel.List.Find(x => x.UniqueId == l.Id).Content.Brightness;
                    l.Color = ViewModel.List.Find(x => x.UniqueId == l.Id).Content.Color;
                }

                Property.SelectedLights = new List<string>();
                foreach (var c in ViewModel.List)
                {
                    if (!c.IsChecked) continue;

                    Property.SelectedLights.Add(c.UniqueId);
                }
            }
            else if (BrightnessProperty != null)
            {
                foreach (var l in BrightnessProperty.Lights)
                {
                    l.Brightness = ViewModel.List.Find(x => x.UniqueId == l.Id).Content.Brightness;
                    l.Color = ViewModel.List.Find(x => x.UniqueId == l.Id).Content.Color;
                    l.OnlyBrightness = ViewModel.List.Find(x => x.UniqueId == l.Id).Content.OnlyBrightness;
                }

                BrightnessProperty.SelectedLights = new List<string>();
                foreach (var c in ViewModel.List)
                {
                    if (!c.IsChecked) continue;
                    
                    BrightnessProperty.SelectedLights.Add(c.UniqueId);
                }
            }
            
            DialogResult = true;
            _isWindowOpened = false;
            Close();
        }

        private bool _loadDone;
        private void LightSelector_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.List = new List<LightSettingCellViewModel>();

            for (var i = 0; i < AllLights.Count; i++)
            {
                if (Property != null)
                {
                    ViewModel.List.Add(new LightSettingCellViewModel
                    {
                        Content = new LightSettingCell
                        {
                            LightSelectorViewModel = ViewModel,
                            Text = AllLights.ElementAt(i).Name,
                            Color = Color.FromRgb(Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Red,
                                Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Green,
                                Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Blue),
                            Brightness = Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Brightness,
                            Index = i
                        },
                        UniqueId = AllLights.ElementAt(i).UniqueId,
                        IsChecked = Property.SelectedLights.Any(x => x == AllLights.ElementAt(i).UniqueId),
                        Index = i,
                        Color = Color.FromRgb(Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Red,
                            Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Green,
                            Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Blue),
                        Brightness = Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Brightness
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

                    ViewModel.List.Add(new LightSettingCellViewModel
                    {
                        Content = new LightSettingCell
                        {
                            LightSelectorViewModel = ViewModel,
                            Text = AllLights.ElementAt(i).Name,
                            Color = Color.FromRgb(BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Red,
                                BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Green,
                                BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Blue),
                            Brightness = BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Brightness,
                            Index = i,
                            OnlyBrightness = BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).OnlyBrightness,
                            OnlyBrightnessVisibility = Visibility.Visible,
                            MainEventText = string.Format(Cultures.Resources.UseMainEventColor, (mainEvent != Cultures.Resources.Current ? "\"" : "") + mainEvent + (mainEvent != Cultures.Resources.Current ? "\"" : ""))
                        },
                        UniqueId = AllLights.ElementAt(i).UniqueId,
                        IsChecked = BrightnessProperty.SelectedLights.Any(x => x == AllLights.ElementAt(i).UniqueId),
                        SingleOptionVisibility = singleOption ? Visibility.Visible : Visibility.Collapsed,
                        Index = i,
                        Color = Color.FromRgb(BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Red,
                            BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Green,
                            BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color.Blue),
                        Brightness = BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Brightness
                    });
                }
            }

            _loadDone = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _isWindowOpened = false;
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
