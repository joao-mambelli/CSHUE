namespace CSHUE.Components.Core.Input
{
    public interface IValidateInput
    {
        event InputValidationErrorEventHandler InputValidationError;

        bool CommitInput();
    }
}