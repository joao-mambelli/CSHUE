using System.Diagnostics;
using System.Windows;

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox
    {
        #region Properties

        /// <summary>
        /// Path to open when "Open Path" button is pressed.
        /// </summary>
        public string Path { get; set; }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// First button text.
        /// </summary>
        public string Text1
        {
            get => (string) GetValue(Text1Property);
            set => SetValue(Text1Property, value);
        }
        public static readonly DependencyProperty Text1Property =
            DependencyProperty.Register("Text1", typeof(string), typeof(CustomMessageBox));

        /// <summary>
        /// Second button text.
        /// </summary>
        public string Text2
        {
            get => (string) GetValue(Text2Property);
            set => SetValue(Text2Property, value);
        }
        public static readonly DependencyProperty Text2Property =
            DependencyProperty.Register("Text2", typeof(string), typeof(CustomMessageBox));

        /// <summary>
        /// Message to dislay in the message box.
        /// </summary>
        public string Message
        {
            get => (string) GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(CustomMessageBox));

        /// <summary>
        /// Property to control Button1Button2 type visibility.
        /// </summary>
        public Visibility Button1Button2Visibility
        {
            get => (Visibility) GetValue(Button1Button2VisibilityProperty);
            set => SetValue(Button1Button2VisibilityProperty, value);
        }
        public static readonly DependencyProperty Button1Button2VisibilityProperty =
            DependencyProperty.Register("Button1Button2Visibility", typeof(Visibility), typeof(CustomMessageBox));

        /// <summary>
        /// Property to control Button1 type visibility.
        /// </summary>
        public Visibility Button1Visibility
        {
            get => (Visibility) GetValue(Button1VisibilityProperty);
            set => SetValue(Button1VisibilityProperty, value);
        }
        public static readonly DependencyProperty Button1VisibilityProperty =
            DependencyProperty.Register("Button1Visibility", typeof(Visibility), typeof(CustomMessageBox));

        /// <summary>
        /// Property to control Button1OpenFolder type visibility.
        /// </summary>
        public Visibility Button1OpenFolderVisibility
        {
            get => (Visibility) GetValue(Button1OpenFolderVisibilityProperty);
            set => SetValue(Button1OpenFolderVisibilityProperty, value);
        }
        public static readonly DependencyProperty Button1OpenFolderVisibilityProperty =
            DependencyProperty.Register("Button1OpenFolderVisibility", typeof(Visibility), typeof(CustomMessageBox));

        #endregion

        #region Events Handlers

        /// <summary>
        /// Button1 click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Button2 click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// ButtonOpenFolder click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOpenFolder_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Path);
        }

        /// <summary>
        /// View OnLoad handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
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

        #region Initializers

        /// <summary>
        /// Initializer.
        /// </summary>
        public CustomMessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion
    }
}
