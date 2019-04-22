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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSHUE.Views
{
    /// <summary>
    /// Interação lógica para BridgeSelector.xam
    /// </summary>
    public partial class BridgeSelector : UserControl
    {
        public string ContentText
        {
            get => (string)GetValue(ContentTextProperty);
            set => SetValue(ContentTextProperty, value);
        }
        public static readonly DependencyProperty ContentTextProperty =
            DependencyProperty.Register("ContentText", typeof(string), typeof(BridgeSelector));

        public BridgeSelector()
        {
            InitializeComponent();
        }
    }
}
