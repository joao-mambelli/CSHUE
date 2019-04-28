using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for HubSelector.xaml
    /// </summary>
    public partial class HubSelector
    {
        public HubSelector()
        {
            InitializeComponent();
        }

        public string SelectedBridge { get; set; }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            foreach (var c in List)
            {
                if (!c.IsChecked) continue;

                SelectedBridge = c.Ip;
                break;
            }

            DialogResult = true;
            Close();
        }

        public List<HubSelectorViewModel> List
        {
            get => (List<HubSelectorViewModel>)GetValue(ListProperty);
            set => SetValue(ListProperty, value);
        }
        public static readonly DependencyProperty ListProperty =
            DependencyProperty.Register("List", typeof(List<HubSelectorViewModel>), typeof(HubSelector));

        private void HubSelector_OnLoaded(object sender, RoutedEventArgs e)
        {
            List.ElementAt(0).IsChecked = true;
        }
    }
}
