using System.Linq;
using System.Windows;
using CSHUE.ViewModels;
using CSHUE.Views;

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for LightSettingCell.xaml
    /// </summary>
    public partial class LightSettingCell
    {
        #region Fields

        /// <summary>
        /// ViewModel.
        /// </summary>
        public LightSelectorViewModel LightSelectorViewModel = null;

        #endregion

        #region Initializers

        /// <summary>
        /// Initializer.
        /// </summary>
        public LightSettingCell()
        {
            InitializeComponent();
        }

        #endregion

        #region Events Handlers

        /// <summary>
        /// Button click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            LightSelectorViewModel.IsColorPickerOpened = true;
            var colorPicker = new ColorPicker
            {
                Text1 = Cultures.Resources.Cancel,
                Text2 = Cultures.Resources.Ok,
                Owner = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault(),
                Color = ((LightSettingCellViewModel) DataContext).Color,
                Id = ((LightSettingCellViewModel) DataContext).Id,
                Brightness = ((LightSettingCellViewModel) DataContext).Brightness,
                IsColorTemperature = ((LightSettingCellViewModel) DataContext).IsColorTemperature,
                ColorTemperature = ((LightSettingCellViewModel) DataContext).ColorTemperature
            };
            colorPicker.ShowDialog();
            LightSelectorViewModel.IsColorPickerOpened = false;

            ((LightSettingCellViewModel) DataContext).Color = colorPicker.Color;
            ((LightSettingCellViewModel) DataContext).ColorTemperature = colorPicker.ColorTemperature;
        }

        #endregion
    }
}
