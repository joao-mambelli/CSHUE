using System;
using System.Windows;
using System.Windows.Controls;

namespace CSHUE.Views
{
    /// <summary>
    /// Interação lógica para LoadingSpinner.xam
    /// </summary>
    public partial class LoadingSpinner : ContentControl
    {
        static LoadingSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingSpinner), new FrameworkPropertyMetadata(typeof(LoadingSpinner)));
        }

        public bool IsHanging
        {
            get { return (bool)GetValue(IsHangingProperty); }
            set { SetValue(IsHangingProperty, value); }
        }

        public static readonly DependencyProperty IsHangingProperty =
            DependencyProperty.Register("IsHanging", typeof(bool), typeof(LoadingSpinner), new UIPropertyMetadata(IsHangingPropertyChangedCallback));

        static void IsHangingPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as LoadingSpinner).OnIsHangingPropertyChanged(d, e);
        }

        private void OnIsHangingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Convert.ToBoolean(e.NewValue))
            {
                VisualStateManager.GoToState(this, "Hanging", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Loading", true);
            }
        }
    }
}
