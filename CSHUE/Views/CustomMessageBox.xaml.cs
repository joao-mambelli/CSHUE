using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public string Yes
        {
            get => (string)GetValue(YesProperty);
            set => SetValue(YesProperty, value);
        }
        public static readonly DependencyProperty YesProperty =
            DependencyProperty.Register("Yes", typeof(string), typeof(CustomMessageBox));

        public string No
        {
            get => (string)GetValue(NoProperty);
            set => SetValue(NoProperty, value);
        }
        public static readonly DependencyProperty NoProperty =
            DependencyProperty.Register("No", typeof(string), typeof(CustomMessageBox));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(CustomMessageBox));

        public Visibility YesNoVisibility
        {
            get => (Visibility)GetValue(YesNoVisibilityProperty);
            set => SetValue(YesNoVisibilityProperty, value);
        }
        public static readonly DependencyProperty YesNoVisibilityProperty =
            DependencyProperty.Register("YesNoVisibility", typeof(Visibility), typeof(CustomMessageBox));

        public Visibility OkVisibility
        {
            get => (Visibility)GetValue(OkVisibilityProperty);
            set => SetValue(OkVisibilityProperty, value);
        }
        public static readonly DependencyProperty OkVisibilityProperty =
            DependencyProperty.Register("OkVisibility", typeof(Visibility), typeof(CustomMessageBox));

        public CustomMessageBox()
        {
            InitializeComponent();
        }

        private void buttonYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CustomMessageBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Yes) && !string.IsNullOrEmpty(No))
            {
                YesNoVisibility = Visibility.Visible;
                OkVisibility = Visibility.Hidden;
            }
            else
            {
                YesNoVisibility = Visibility.Hidden;
                OkVisibility = Visibility.Visible;
            }
        }
    }
}
