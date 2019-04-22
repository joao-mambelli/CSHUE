using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Selector.xaml
    /// </summary>
    public partial class Selector : Window
    {
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

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public List<BridgeSelector> List
        {
            get => (List<BridgeSelector>)GetValue(ListProperty);
            set => SetValue(ListProperty, value);
        }
        public static readonly DependencyProperty ListProperty =
            DependencyProperty.Register("List", typeof(List<BridgeSelector>), typeof(Selector));
    }
}
