using System.Collections.Generic;
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
        public HubSelectorViewModel ViewModel = new HubSelectorViewModel();

        public HubSelector(List<HubInfoCellViewModel> list)
        {
            InitializeComponent();
            DataContext = ViewModel;

            list.ElementAt(0).IsChecked = true;

            ViewModel.List = list;
        }

        public string SelectedBridge { get; set; }

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
    }
}
