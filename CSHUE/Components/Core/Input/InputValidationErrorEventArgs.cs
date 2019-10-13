using System;

namespace CSHUE.Components.Core.Input
{
    public delegate void InputValidationErrorEventHandler(object sender, InputValidationErrorEventArgs e);

    public class InputValidationErrorEventArgs : EventArgs
    {
        #region Constructors

        public InputValidationErrorEventArgs(Exception e)
        {
            Exception = e;
        }

        #endregion

        #region Exception Property

        public Exception Exception { get; }

        #endregion

        #region ThrowException Property

        public bool ThrowException { get; set; }

        #endregion
    }
}