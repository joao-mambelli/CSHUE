using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CSHUE.ViewModels;
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for HubSelector.xaml
    /// </summary>
    public partial class HubSelector
    {
        #region Fields

        public HubSelectorViewModel ViewModel = new HubSelectorViewModel();

        #endregion

        #region Initializers

        public HubSelector(List<HubInfoCellViewModel> list)
        {
            InitializeComponent();
            DataContext = ViewModel;

            list.ElementAt(0).IsChecked = true;

            ViewModel.List = new ObservableCollection<HubInfoCellViewModel>(list);
        }

        #endregion

        #region Properties

        public string SelectedBridge { get; set; }

        #endregion

        #region Events Handlers

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            foreach (var c in ViewModel.List)
            {
                if (!c.IsChecked) continue;

                SelectedBridge = c.Ip;
                break;
            }

            DialogResult = true;
            Close();
        }

        #endregion
    }
}
