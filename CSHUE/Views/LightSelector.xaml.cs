using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CSHUE.Helpers;
using CSHUE.ViewModels;
using Q42.HueApi;
using CSHUE.Controls;

// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for LightSelector.xaml
    /// </summary>
    public partial class LightSelector
    {
        #region Fields

        public LightSelectorViewModel ViewModel = new LightSelectorViewModel();

        private bool _isWindowOpened = true;
        private bool _loadDone;
        private bool _radioButtonOnChanged;

        #endregion

        #region Initializers

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
                                ViewModel.SetLightsAsync();
                            });

                        Thread.Sleep(500);
                    }
                })
                { IsBackground = true }.Start();
        }

        private void LightSelector_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.List = new ObservableCollection<LightSettingCellViewModel>();

            for (var i = 0; i < AllLights.Count; i++)
            {
                if (Property != null)
                {
                    var lightSettingCellViewModel = new LightSettingCellViewModel
                    {
                        Text = AllLights.ElementAt(i).Name,
                        Color = Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId) == null
                            ? Colors.Black
                            : Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color,
                        Brightness = Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId) == null
                            ? (byte)255
                            : Property.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Brightness,
                        Index = i + 1,
                        UniqueId = AllLights.ElementAt(i).UniqueId,
                        IsChecked = Property.SelectedLights.Any(x => x == AllLights.ElementAt(i).UniqueId)
                    };
                    lightSettingCellViewModel.Content = new LightSettingCell
                    {
                        LightSelectorViewModel = ViewModel,
                        DataContext = lightSettingCellViewModel
                    };

                    ViewModel.List.Add(lightSettingCellViewModel);
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

                    var lightSettingCellViewModel = new LightSettingCellViewModel
                    {
                        Text = AllLights.ElementAt(i).Name,
                        Color = BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId) == null
                            ? Colors.Black
                            : BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Color,
                        Brightness = BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId) == null
                            ? (byte)255
                            : BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).Brightness,
                        Index = i + 1,
                        OnlyBrightness =
                            BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId) == null ||
                            BrightnessProperty.Lights.Find(x => x.Id == AllLights.ElementAt(i).UniqueId).OnlyBrightness,
                        OnlyBrightnessVisibility = Visibility.Visible,
                        MainEventText = string.Format(Cultures.Resources.UseMainEventColor,
                            (mainEvent != Cultures.Resources.Current ? "\"" : "") + mainEvent +
                            (mainEvent != Cultures.Resources.Current ? "\"" : "")),
                        UniqueId = AllLights.ElementAt(i).UniqueId,
                        IsChecked = BrightnessProperty.SelectedLights.Any(x => x == AllLights.ElementAt(i).UniqueId),
                        SingleOptionVisibility = singleOption ? Visibility.Visible : Visibility.Collapsed
                    };
                    lightSettingCellViewModel.Content = new LightSettingCell
                    {
                        LightSelectorViewModel = ViewModel,
                        DataContext = lightSettingCellViewModel
                    };

                    ViewModel.List.Add(lightSettingCellViewModel);
                }
            }

            _loadDone = true;
        }

        #endregion

        #region Properties

        public List<Light> AllLights { get; set; }

        public EventProperty Property { get; set; } = null;

        public EventBrightnessProperty BrightnessProperty { get; set; } = null;

        #endregion

        #region Events Handlers

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (Property != null)
            {
                foreach (var l in Property.Lights)
                {
                    l.Brightness = ViewModel.List.ToList().Find(x => x.UniqueId == l.Id).Brightness;
                    l.Color = ViewModel.List.ToList().Find(x => x.UniqueId == l.Id).Color;
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
                    l.Brightness = ViewModel.List.ToList().Find(x => x.UniqueId == l.Id).Brightness;
                    l.Color = ViewModel.List.ToList().Find(x => x.UniqueId == l.Id).Color;
                    l.OnlyBrightness = ViewModel.List.ToList().Find(x => x.UniqueId == l.Id).OnlyBrightness;
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

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _isWindowOpened = false;
        }

        private void RadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (((RadioButton)sender).IsChecked == true && !_radioButtonOnChanged && _loadDone)
            {
                ((RadioButton)sender).IsChecked = false;
            }

            _radioButtonOnChanged = false;
        }

        private void RadioButton_OnChanged(object sender, RoutedEventArgs e)
        {
            _radioButtonOnChanged = true;
        }

        #endregion
    }
}
