using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CSHUE.ViewModels;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Selector.xaml
    /// </summary>
    public partial class Selector : Window
    {
        public SelectorViewModel ViewModel = new SelectorViewModel();

        public string Ok
        {
            get => (string)GetValue(OkProperty);
            set => SetValue(OkProperty, value);
        }
        public static readonly DependencyProperty OkProperty =
            DependencyProperty.Register("Ok", typeof(string), typeof(Selector));

        public Selector()
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

        public List<SelectorViewModel> List
        {
            get => (List<SelectorViewModel>)GetValue(ListProperty);
            set => SetValue(ListProperty, value);
        }
        public static readonly DependencyProperty ListProperty =
            DependencyProperty.Register("List", typeof(List<SelectorViewModel>), typeof(Selector));

        private void Selector_OnLoaded(object sender, RoutedEventArgs e)
        {
            List.ElementAt(0).IsChecked = true;
        }
    }
}
