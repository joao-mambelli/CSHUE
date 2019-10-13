﻿using System.Windows;

namespace CSHUE.Components.ButtonSpinner
{
    /// <summary>
    /// Provides data for the Spinner.Spin event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class SpinEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the SpinDirection for the spin that has been initiated by the 
        /// end-user.
        /// </summary>
        public SpinDirection Direction { get; }

        /// <summary>
        /// Get or set whheter the spin event originated from a mouse wheel event.
        /// </summary>
        public bool UsingMouseWheel { get; }

        /// <summary>
        /// Initializes a new instance of the SpinEventArgs class.
        /// </summary>
        /// <param name="direction">Spin direction.</param>
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