﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CSHUE.Controls;
using CSHUE.Core;
using CSHUE.Cultures;
using CSHUE.ViewModels;
using Microsoft.Win32;
using Q42.HueApi;
using SourceChord.FluentWPF;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        #region Fields

        public SettingsViewModel ViewModel = new SettingsViewModel();

        #endregion

        #region Initializers

        public Settings()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.MainWindowViewModel =
                Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.ViewModel;

            ViewModel.SelectedTheme = ViewModel.Themes.First(x => x.Index == Properties.Settings.Default.Theme);
            ViewModel.SelectedTransparency =
                ViewModel.Transparencies.First(x => x.Index == Properties.Settings.Default.Transparency);

            ComboBoxLanguage.SelectionChanged += ComboBoxLanguage_OnSelectionChanged;

            Properties.Settings.Default.PropertyChanged += Default_OnPropertyChanged;
        }

        #endregion

        #region Events Handlers

        private void TextBox_OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void Default_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ViewModel.UpdateGradient(e.PropertyName);
        }

        private static void ComboBoxLanguage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Properties.Settings.Default.Save();

            if (((ComboBox)sender).SelectedItem == null)
            {
                var culture = CultureResources.SupportedCultures.Contains(CultureInfo.InstalledUICulture)
                    ? CultureInfo.InstalledUICulture
                    : new CultureInfo("en-US");

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                CultureResources.ChangeCulture(culture);

                Properties.Settings.Default.LanguageName =
                    CultureResources.SupportedCultures.Find(x => Equals(x, culture)).NativeName;
            }
            else
            {
                var culture =
                    CultureResources.SupportedCultures.Find(x =>
                        x.NativeName == (string) ((ComboBox) sender).SelectedItem);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                Cultures.Resources.Culture = culture;

                Process.Start(Application.ResourceAssembly.Location, "-lang");

                Application.Current.Shutdown();
            }
        }

        private async void Default_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBox = new CustomMessageBox
            {
                Button1 = new CustomButton
                {
                    Text = Cultures.Resources.YesButton,
                    DialogResult = true
                },
                Button2 = new CustomButton
                {
                    Text = Cultures.Resources.NoButton
                },
                Message = Cultures.Resources.ResetMessage,
                Owner = Window.GetWindow(this)
            };
            messageBox.ShowDialog();

            if (messageBox.DialogResult != true)
                return;

            ComboBoxLanguage.SelectionChanged -= ComboBoxLanguage_OnSelectionChanged;
            Properties.Settings.Default.Reset();
            ViewModel.MainWindowViewModel.Config.ViewModel.CheckConfigFile();

            try
            {
                await MainWindowViewModel.SetDefaultLightsSettings();
            }
            catch
            {
                Properties.Settings.Default.AppKey = "";
            }

            Process.Start(Application.ResourceAssembly.Location, "-reset");
            Application.Current.Shutdown();
        }

        private void RunOnStartupCheckBox_OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.RunOnStartup)
                ViewModel.AddStartup(Properties.Settings.Default.RunOnStartupMinimized);
            else
                ViewModel.RemoveStartup();

            Save(sender, e);
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {
            List<Light> allLights;
            try
            {
                allLights = (await MainWindowViewModel.Client.GetLightsAsync()).ToList();
            }
            catch
            {
                allLights = null;
            }

            if (ViewModel.MainWindowViewModel.GlobalLightsBackup == null)
            {
                ViewModel.MainWindowViewModel.GlobalLightsBackup = allLights;
            }

            var unreachableLights = new List<Light>();
            if (allLights != null)
                foreach (var l in allLights)
                    if (l.State.IsReachable != true)
                        unreachableLights.Add(l);
            foreach (var l in unreachableLights)
                if (l.State.IsReachable != true)
                    allLights?.Remove(l);

            if (allLights == null || !allLights.Any())
            {
                new CustomMessageBox
                {
                    Button1 = new CustomButton
                    {
                        Text = Cultures.Resources.OkButton
                    },
                    Message = Cultures.Resources.NoLightsFoundMessage,
                    Owner = Window.GetWindow(this)
                }.ShowDialog();

                return;
            }

            var title = Cultures.Resources.LightSelectorTitle + " - ";
            EventProperty property = null;
            EventBrightnessProperty brightnessProperty = null;
            if (((Button)sender).Tag.ToString() == "MainMenu")
            {
                title += Cultures.Resources.MainMenuEvent;
                property = Properties.Settings.Default.MainMenu;
            }
            if (((Button)sender).Tag.ToString() == "PlayerGetsFlashed")
            {
                title += Cultures.Resources.PlayerGetsFlashedEvent;
                property = Properties.Settings.Default.PlayerGetsFlashed;
            }
            if (((Button)sender).Tag.ToString() == "TerroristsWin")
            {
                title += Cultures.Resources.TerroristsWinEvent;
                property = Properties.Settings.Default.TerroristsWin;
            }
            if (((Button)sender).Tag.ToString() == "CounterTerroristsWin")
            {
                title += Cultures.Resources.CounterTerroristsWinEvent;
                property = Properties.Settings.Default.CounterTerroristsWin;
            }
            if (((Button)sender).Tag.ToString() == "RoundStarts")
            {
                title += Cultures.Resources.RoundStartsEvent;
                property = Properties.Settings.Default.RoundStarts;
            }
            if (((Button)sender).Tag.ToString() == "BombPlanted")
            {
                title += Cultures.Resources.BombHasBeenPlantedEvent;
                property = Properties.Settings.Default.BombPlanted;
            }
            if (((Button)sender).Tag.ToString() == "PlayerGetsKill")
            {
                title += Cultures.Resources.PlayerGetsAKillEvent;
                brightnessProperty = Properties.Settings.Default.PlayerGetsKill;
            }
            if (((Button)sender).Tag.ToString() == "PlayerGetsKilled")
            {
                title += Cultures.Resources.PlayerGetsKilledEvent;
                brightnessProperty = Properties.Settings.Default.PlayerGetsKilled;
            }
            if (((Button)sender).Tag.ToString() == "FreezeTime")
            {
                title += Cultures.Resources.FreezeTimeEvent;
                brightnessProperty = Properties.Settings.Default.FreezeTime;
            }
            if (((Button)sender).Tag.ToString() == "Warmup")
            {
                title += Cultures.Resources.WarmupEvent;
                brightnessProperty = Properties.Settings.Default.Warmup;
            }
            if (((Button)sender).Tag.ToString() == "BombExplodes")
            {
                title += Cultures.Resources.BombExplodesEvent;
                brightnessProperty = Properties.Settings.Default.BombExplodes;
            }
            if (((Button)sender).Tag.ToString() == "BombBlink")
            {
                title += Cultures.Resources.BombBlinkEvent;
                brightnessProperty = Properties.Settings.Default.BombBlink;
            }

            ViewModel.LightsBackup = allLights;
            new LightSelector(title)
            {
                AllLights = allLights,
                Property = property,
                BrightnessProperty = brightnessProperty,
                Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()
            }.ShowDialog();
            ViewModel.RestoreLights();

            ViewModel.MainWindowViewModel.Settings.ViewModel.UpdateGradients();

            Save(sender, e);
        }

        private void ShowSystemTrayIconCheckBox_OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel.MainWindowViewModel != null)
                ViewModel.MainWindowViewModel.NotifyIconVisibility =
                    Properties.Settings.Default.ShowSystemTrayIcon
                        ? Visibility.Visible
                        : Visibility.Collapsed;
        }

        private void Save(object sender, RoutedEventArgs routedEventArgs)
        {
            if (ViewModel.MainWindowViewModel != null)
            {
                ViewModel.MainWindowViewModel.Config.ViewModel.CheckConfigFile();
            }

            Properties.Settings.Default.Save();
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ComboBoxLanguage.IsDropDownOpen)
            {
                ComboBoxLanguage.IsDropDownOpen = false;
                ComboBoxLanguage.IsDropDownOpen = true;
            }

            if (ComboBoxTheme.IsDropDownOpen)
            {
                ComboBoxTheme.IsDropDownOpen = false;
                ComboBoxTheme.IsDropDownOpen = true;
            }

            if (ComboBoxTransparency.IsDropDownOpen)
            {
                ComboBoxTransparency.IsDropDownOpen = false;
                ComboBoxTransparency.IsDropDownOpen = true;
            }
        }

        private void ComboBoxTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem == null || ((ComboBox)sender).SelectedIndex == 0)
                return;

            var item = (CustomComboBoxItem)((ComboBox)sender).SelectedItem;

            ViewModel.Themes.Remove(item);
            ViewModel.Themes = new ObservableCollection<CustomComboBoxItem>(ViewModel.Themes.OrderBy(x => x.Index));
            ViewModel.Themes.Insert(0, item);

            ViewModel.SelectedTheme = (CustomComboBoxItem)((ComboBox)sender).Items[0];

            Properties.Settings.Default.Theme = item.Index;

            ResourceDictionaryEx.GlobalTheme = ViewModel.SelectedTheme.Index == 0
                ? ElementTheme.Default
                : ViewModel.SelectedTheme.Index == 1
                    ? ElementTheme.Dark
                    : ElementTheme.Light;

            if (ResourceDictionaryEx.GlobalTheme == ElementTheme.Default)
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(
                        "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                    {
                        if (key == null)
                        {
                            if (Properties.Settings.Default.Transparency == 0 ||
                                Properties.Settings.Default.Transparency == 1)
                            {
                                ViewModel.MainWindowViewModel.BackgroundColor =
                                    (Color)FindResource("SystemAltLowColor");
                                ViewModel.MainWindowViewModel.IsModeDark = false;
                                ViewModel.MainWindowViewModel.IsTransparencyTrue = true;
                            }
                            else
                            {
                                ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(230, 230, 230);
                                ViewModel.MainWindowViewModel.IsModeDark = false;
                                ViewModel.MainWindowViewModel.IsTransparencyTrue = false;
                            }

                            return;
                        }

                        ViewModel.MainWindowViewModel.IsModeDark =
                            key.GetValue("AppsUseLightTheme") != null &&
                            !Convert.ToBoolean(key.GetValue("AppsUseLightTheme"));

                        if (Properties.Settings.Default.Transparency == 0)
                            ViewModel.MainWindowViewModel.IsTransparencyTrue =
                                key.GetValue("EnableTransparency") == null ||
                                Convert.ToBoolean(key.GetValue("EnableTransparency"));
                        else
                            ViewModel.MainWindowViewModel.IsTransparencyTrue =
                                Properties.Settings.Default.Transparency == 1;

                        if (ViewModel.MainWindowViewModel.IsTransparencyTrue == true)
                            ViewModel.MainWindowViewModel.BackgroundColor = (Color)FindResource("SystemAltLowColor");
                        else if (ViewModel.MainWindowViewModel.IsModeDark == true)
                            ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(31, 31, 31);
                        else
                            ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(230, 230, 230);
                    }
                }
                catch
                {
                    // ignored
                }
            else
            {
                ViewModel.MainWindowViewModel.IsModeDark = ResourceDictionaryEx.GlobalTheme == ElementTheme.Dark;

                switch (Properties.Settings.Default.Transparency)
                {
                    case 0:
                        try
                        {
                            using (var key = Registry.CurrentUser.OpenSubKey(
                                "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                            {
                                if (key == null)
                                {
                                    ViewModel.MainWindowViewModel.BackgroundColor =
                                        ResourceDictionaryEx.GlobalTheme == ElementTheme.Dark
                                            ? Color.FromArgb(51, 0, 0, 0)
                                            : Color.FromArgb(51, 255, 255, 255);

                                    ViewModel.MainWindowViewModel.IsTransparencyTrue = true;

                                    return;
                                }

                                ViewModel.MainWindowViewModel.IsTransparencyTrue =
                                    key.GetValue("EnableTransparency") == null ||
                                    Convert.ToBoolean(key.GetValue("EnableTransparency"));

                                if (ViewModel.MainWindowViewModel.IsTransparencyTrue == true)
                                {
                                    ViewModel.MainWindowViewModel.BackgroundColor =
                                        ViewModel.MainWindowViewModel.IsModeDark == true
                                            ? Color.FromArgb(51, 0, 0, 0)
                                            : Color.FromArgb(51, 255, 255, 255);
                                }
                                else if (ViewModel.MainWindowViewModel.IsModeDark == true)
                                    ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(31, 31, 31);
                                else
                                    ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(230, 230, 230);
                            }
                        }
                        catch
                        {
                            // ignored
                        }

                        break;
                    case 1 when ViewModel.MainWindowViewModel.IsModeDark == true:
                        ViewModel.MainWindowViewModel.BackgroundColor = Color.FromArgb(51, 0, 0, 0);
                        break;
                    case 1:
                        ViewModel.MainWindowViewModel.BackgroundColor = Color.FromArgb(51, 255, 255, 255);
                        break;
                    default:
                        ViewModel.MainWindowViewModel.BackgroundColor =
                            ViewModel.MainWindowViewModel.IsModeDark == true
                                ? Color.FromRgb(31, 31, 31)
                                : Color.FromRgb(230, 230, 230);
                        break;
                }
            }
        }

        private void ComboBoxTransparency_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem == null || ((ComboBox)sender).SelectedIndex == 0)
                return;

            var item = (CustomComboBoxItem)((ComboBox)sender).SelectedItem;

            ViewModel.Transparencies.Remove(item);
            ViewModel.Transparencies =
                new ObservableCollection<CustomComboBoxItem>(ViewModel.Transparencies.OrderBy(x => x.Index));
            ViewModel.Transparencies.Insert(0, item);

            ViewModel.SelectedTransparency = (CustomComboBoxItem)((ComboBox)sender).Items[0];

            Properties.Settings.Default.Transparency = item.Index;

            if (Properties.Settings.Default.Transparency == 0)
            {
                try
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(
                        "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                    {
                        if (key == null)
                        {
                            ViewModel.MainWindowViewModel.BackgroundColor =
                                ViewModel.MainWindowViewModel.IsModeDark == true
                                    ? Color.FromArgb(51, 0, 0, 0)
                                    : Color.FromArgb(51, 255, 255, 255);

                            ViewModel.MainWindowViewModel.IsTransparencyTrue = true;

                            return;
                        }

                        ViewModel.MainWindowViewModel.IsTransparencyTrue =
                            key.GetValue("EnableTransparency") == null ||
                            Convert.ToBoolean(key.GetValue("EnableTransparency"));

                        if (ViewModel.MainWindowViewModel.IsTransparencyTrue == true)
                            ViewModel.MainWindowViewModel.BackgroundColor =
                                ViewModel.MainWindowViewModel.IsModeDark == true
                                    ? Color.FromArgb(51, 0, 0, 0)
                                    : Color.FromArgb(51, 255, 255, 255);
                        else
                            ViewModel.MainWindowViewModel.BackgroundColor =
                                ViewModel.MainWindowViewModel.IsModeDark == true
                                    ? Color.FromRgb(31, 31, 31)
                                    : Color.FromRgb(230, 230, 230);
                    }
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                ViewModel.MainWindowViewModel.IsTransparencyTrue = Properties.Settings.Default.Transparency == 1;

                switch (Properties.Settings.Default.Theme)
                {
                    case 0:
                        try
                        {
                            using (var key = Registry.CurrentUser.OpenSubKey(
                                "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
                            {
                                if (key == null)
                                {
                                    ViewModel.MainWindowViewModel.BackgroundColor =
                                        ViewModel.MainWindowViewModel.IsTransparencyTrue == true
                                            ? Color.FromArgb(51, 0, 0, 0)
                                            : Color.FromRgb(230, 230, 230);

                                    ViewModel.MainWindowViewModel.IsModeDark = false;

                                    return;
                                }

                                ViewModel.MainWindowViewModel.IsModeDark =
                                    key.GetValue("AppsUseLightTheme") != null &&
                                    !Convert.ToBoolean(key.GetValue("AppsUseLightTheme"));

                                if (ViewModel.MainWindowViewModel.IsTransparencyTrue == true)
                                {
                                    ViewModel.MainWindowViewModel.BackgroundColor =
                                        ViewModel.MainWindowViewModel.IsModeDark == true
                                            ? Color.FromArgb(51, 0, 0, 0)
                                            : Color.FromArgb(51, 255, 255, 255);
                                }
                                else if (ViewModel.MainWindowViewModel.IsModeDark == true)
                                    ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(31, 31, 31);
                                else
                                    ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(230, 230, 230);
                            }
                        }
                        catch
                        {
                            // ignored
                        }

                        break;
                    case 1 when ViewModel.MainWindowViewModel.IsTransparencyTrue == true:
                        ViewModel.MainWindowViewModel.BackgroundColor = Color.FromArgb(51, 0, 0, 0);
                        break;
                    case 1:
                        ViewModel.MainWindowViewModel.BackgroundColor = Color.FromRgb(31, 31, 31);
                        break;
                    default:
                        ViewModel.MainWindowViewModel.BackgroundColor =
                            ViewModel.MainWindowViewModel.IsTransparencyTrue == true
                                ? Color.FromArgb(51, 255, 255, 255)
                                : Color.FromRgb(230, 230, 230);
                        break;
                }
            }
        }

        private void BrightnessModifier_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ViewModel.Formula = e.NewValue <= 100
                ? $"{Cultures.Resources.BrightnessPercentageFormula} * {e.NewValue / 100:0.00}"
                : $"{Cultures.Resources.BrightnessPercentageFormula} + (1 - {Cultures.Resources.BrightnessPercentageFormula}) * {e.NewValue / 100 - 1:0.00}";
        }

        #endregion
    }
}
