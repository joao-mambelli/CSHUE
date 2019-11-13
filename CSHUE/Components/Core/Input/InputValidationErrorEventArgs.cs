using System;

namespace CSHUE.Components.Core.Input
{
    public delegate void InputValidationErrorEventHandler(object sender, InputValidationErrorEventArgs e);

    public class InputValidationErrorEventArgs : EventArgs
    {
        public InputValidationErrorEventArgs(Exception e)
        {
            Exception = e;
        }

        public Exception Exception { get; }
        public bool ThrowException { get; set; }
    }
}