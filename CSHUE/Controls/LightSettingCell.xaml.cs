using System.Linq;
using System.Windows;
using System.Windows.Media;
using CSHUE.ViewModels;
using CSHUE.Views;

// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for LightSettingCell.xaml
    /// </summary>
    public partial class LightSettingCell
    {
        public LightSelectorViewModel LightSelectorViewModel = null;

        public LightSettingCell()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            LightSelectorViewModel.IsColorPickerOpened = true;
            var colorPicker = new ColorPicker
            {
                Text1 = Cultures.Resources.Cancel,
                Text2 = Cultures.Resources.Ok,
                Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(),
                Color = ((LightSettingCellViewModel)DataContext).Color,
                Index = ((LightSettingCellViewModel)DataContext).Index,
                Brightness = ((LightSettingCellViewModel)DataContext).Brightness
            };
            colorPicker.ShowDialog();
            LightSelectorViewModel.IsColorPickerOpened = false;

            if (colorPicker.DialogResult == true)
                ((LightSettingCellViewModel)DataContext).Color = colorPicker.Color;
        }
    }
}
