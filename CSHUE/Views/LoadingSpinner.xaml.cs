using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace CSHUE.Views
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class LoadingSpinner : UserControl
    {
        public LoadingSpinner()
        {
            InitializeComponent();
        }
        
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(
                "IsLoading",
                typeof(bool),
                typeof(LoadingSpinner),
                new PropertyMetadata(default(bool), OnIsLoadingPropertyChanged));

        private static void OnIsLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState((LoadingSpinner)d, (bool)e.NewValue ? "Loading" : "Hanging", true);

            if ((bool) e.NewValue)
            {
                ((LoadingSpinner)d).Path.SetResourceReference(Shape.StrokeProperty, "SystemBaseHighColorBrush");
            }
            else
            {
                ((LoadingSpinner)d).Path.SetResourceReference(Shape.StrokeProperty, "SystemBaseMediumColorBrush");
            }
        }
    }
}
