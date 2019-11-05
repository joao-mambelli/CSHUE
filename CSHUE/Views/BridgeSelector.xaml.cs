using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CSHUE.ViewModels;
using Q42.HueApi.Models.Bridge;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for BridgeSelector.xaml
    /// </summary>
    public partial class BridgeSelector
    {
        #region Fields

        public BridgeSelectorViewModel ViewModel = new BridgeSelectorViewModel();

        #endregion

        #region Initializers

        public BridgeSelector(List<BridgeInfoCellViewModel> list)
        {
            InitializeComponent();
            DataContext = ViewModel;

            list.ElementAt(0).IsChecked = true;

            ViewModel.List = new ObservableCollection<BridgeInfoCellViewModel>(list);
        }

        #endregion

        #region Properties

        public LocatedBridge SelectedBridge { get; set; }

        #endregion

        #region Events Handlers

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var c in ViewModel.List)
            {
                if (!c.IsChecked) continue;

                SelectedBridge = new LocatedBridge
                {
                    IpAddress = c.Ip,
                    BridgeId = c.Id
                };
                break;
            }

            DialogResult = true;
            Close();
        }

        #endregion
    }
}
