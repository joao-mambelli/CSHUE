using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSHUE.Styles
{
    public static class TextBox
    {
        public static DependencyProperty CueTextProperty =
            DependencyProperty.RegisterAttached("CueText",
                typeof(string),
                typeof(TextBox));

        public static void SetCueText(FrameworkElement target, string value)
        {
            target.SetValue(CueTextProperty, value);
        }

        public static string GetCueText(FrameworkElement target)
        {
            return (string)target.GetValue(CueTextProperty);
        }
    }
}
