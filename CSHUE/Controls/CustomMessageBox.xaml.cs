using System.Diagnostics;
using System.Windows;

namespace CSHUE.Controls
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox
    {
        #region Dependency Properties

        /// <summary>
        /// First button.
        /// </summary>
        public CustomButton Button1
        {
            get => (CustomButton)GetValue(Button1Property);
            set => SetValue(Button1Property, value);
        }
        public static readonly DependencyProperty Button1Property =
            DependencyProperty.Register("Button1", typeof(CustomButton), typeof(CustomMessageBox));

        /// <summary>
        /// Second button.
        /// </summary>
        public CustomButton Button2
        {
            get => (CustomButton)GetValue(Button2Property);
            set => SetValue(Button2Property, value);
        }
        public static readonly DependencyProperty Button2Property =
            DependencyProperty.Register("Button2", typeof(CustomButton), typeof(CustomMessageBox));

        /// <summary>
        /// Third button.
        /// </summary>
        public CustomButton Button3
        {
            get => (CustomButton)GetValue(Button3Property);
            set => SetValue(Button3Property, value);
        }
        public static readonly DependencyProperty Button3Property =
            DependencyProperty.Register("Button3", typeof(CustomButton), typeof(CustomMessageBox));

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
        /// Property to control Button1 type visibility.
        /// </summary>
        public Visibility Button1Visibility
        {
            get => (Visibility) GetValue(Button1VisibilityProperty);
            set => SetValue(Button1VisibilityProperty, value);
        }
        public static readonly DependencyProperty Button1VisibilityProperty =
            DependencyProperty.Register("Button1Visibility", typeof(Visibility), typeof(CustomMessageBox),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Property to control Button2 type visibility.
        /// </summary>
        public Visibility Button2Visibility
        {
            get => (Visibility)GetValue(Button2VisibilityProperty);
            set => SetValue(Button2VisibilityProperty, value);
        }
        public static readonly DependencyProperty Button2VisibilityProperty =
            DependencyProperty.Register("Button2Visibility", typeof(Visibility), typeof(CustomMessageBox),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Property to control Button3 type visibility.
        /// </summary>
        public Visibility Button3Visibility
        {
            get => (Visibility)GetValue(Button3VisibilityProperty);
            set => SetValue(Button3VisibilityProperty, value);
        }
        public static readonly DependencyProperty Button3VisibilityProperty =
            DependencyProperty.Register("Button3Visibility", typeof(Visibility), typeof(CustomMessageBox),
                new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Events Handlers

        /// <summary>
        /// Button1 click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = Button1.DialogResult;

            if (!string.IsNullOrWhiteSpace(Button1.Path))
            {
                if (Button1.ShowInFolder)
                    Process.Start("explorer.exe", $"/select, \"{Button1.Path}\"");
                else
                    Process.Start(Button1.Path);
            }
        }

        /// <summary>
        /// Button2 click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            //DialogResult = Button2.DialogResult;

            if (!string.IsNullOrWhiteSpace(Button2.Path))
            {
                if (Button2.ShowInFolder)
                    Process.Start("explorer.exe", $"/select, \"{Button2.Path}\"");
                else
                    Process.Start(Button2.Path);
            }
        }

        /// <summary>
        /// Button3 click handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_OnClick(object sender, RoutedEventArgs e)
        {
            //DialogResult = Button3.DialogResult;

            if (!string.IsNullOrWhiteSpace(Button3.Path))
            {
                if (Button3.ShowInFolder)
                    Process.Start("explorer.exe", $"/select, \"{Button3.Path}\"");
                else
                    Process.Start(Button3.Path);
            }
        }

        /// <summary>
        /// View OnLoad handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Button1 != null && !string.IsNullOrWhiteSpace(Button1.Text))
                Button1Visibility = Visibility.Visible;

            if (Button2 != null && !string.IsNullOrWhiteSpace(Button2.Text))
                Button2Visibility = Visibility.Visible;

            if (Button3 != null && !string.IsNullOrWhiteSpace(Button3.Text))
                Button3Visibility = Visibility.Visible;
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

    public class CustomButton
    {
        public string Text { get; set; } = "";
        public string Path { get; set; } = "";
        public bool ShowInFolder { get; set; } = false;
        public bool? DialogResult { get; set; } = false;
    }
}
