using System.Diagnostics;
using System.Windows;
// ReSharper disable InheritdocConsiderUsage

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox
    {
        #region Properties

        public string Path { get; set; }

        #endregion

        #region Dependency Properties

        public string Text1
        {
            get => (string)GetValue(Text1Property);
            set => SetValue(Text1Property, value);
        }
        public static readonly DependencyProperty Text1Property =
            DependencyProperty.Register("Text1", typeof(string), typeof(CustomMessageBox));

        public string Text2
        {
            get => (string)GetValue(Text2Property);
            set => SetValue(Text2Property, value);
        }
        public static readonly DependencyProperty Text2Property =
            DependencyProperty.Register("Text2", typeof(string), typeof(CustomMessageBox));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(CustomMessageBox));

        public Visibility Button1Button2Visibility
        {
            get => (Visibility)GetValue(Button1Button2VisibilityProperty);
            set => SetValue(Button1Button2VisibilityProperty, value);
        }
        public static readonly DependencyProperty Button1Button2VisibilityProperty =
            DependencyProperty.Register("Button1Button2Visibility", typeof(Visibility), typeof(CustomMessageBox));

        public Visibility Button1Visibility
        {
            get => (Visibility)GetValue(Button1VisibilityProperty);
            set => SetValue(Button1VisibilityProperty, value);
        }
        public static readonly DependencyProperty Button1VisibilityProperty =
            DependencyProperty.Register("Button1Visibility", typeof(Visibility), typeof(CustomMessageBox));

        public Visibility Button1OpenFolderVisibility
        {
            get => (Visibility)GetValue(Button1OpenFolderVisibilityProperty);
            set => SetValue(Button1OpenFolderVisibilityProperty, value);
        }
        public static readonly DependencyProperty Button1OpenFolderVisibilityProperty =
            DependencyProperty.Register("Button1OpenFolderVisibility", typeof(Visibility), typeof(CustomMessageBox));

        #endregion

        #region Events Handlers

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Path);
        }

        #endregion

        #region Initializers

        public CustomMessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void CustomMessageBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Text1 == Cultures.Resources.Ok && Text2 == null)
            {
                Button1Button2Visibility = Visibility.Hidden;
                Button1Visibility = Visibility.Visible;
                Button1OpenFolderVisibility = Visibility.Hidden;
            }
            else if (Text1 == Cultures.Resources.Ok &&
                     (Text2 == Cultures.Resources.OpenFolder ||
                      Text2 == Cultures.Resources.ShowInBrowser))
            {
                Button1Button2Visibility = Visibility.Hidden;
                Button1Visibility = Visibility.Hidden;
                Button1OpenFolderVisibility = Visibility.Visible;
            }
            else
            {
                Button1Button2Visibility = Visibility.Visible;
                Button1Visibility = Visibility.Hidden;
                Button1OpenFolderVisibility = Visibility.Hidden;
            }
        }

        #endregion
    }
}
