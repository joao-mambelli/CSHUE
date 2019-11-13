using System.Windows;

namespace CSHUE.Components.ButtonSpinner
{
    public class SpinEventArgs : RoutedEventArgs
    {
        public SpinDirection Direction { get; }

        public bool UsingMouseWheel { get; }

        public SpinEventArgs(SpinDirection direction)
        {
            Direction = direction;
        }

        public SpinEventArgs(RoutedEvent routedEvent, SpinDirection direction) : base(routedEvent)
        {
            Direction = direction;
        }

        public SpinEventArgs(SpinDirection direction, bool usingMouseWheel)
        {
            Direction = direction;
            UsingMouseWheel = usingMouseWheel;
        }

        public SpinEventArgs(RoutedEvent routedEvent, SpinDirection direction, bool usingMouseWheel) : base(routedEvent)
        {
            Direction = direction;
            UsingMouseWheel = usingMouseWheel;
        }
    }
}