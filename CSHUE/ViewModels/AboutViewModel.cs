namespace CSHUE.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// Main window viewmodel field.
        /// </summary>
        public MainWindowViewModel MainWindowViewModel = null;

        #endregion

        public string Version => Core.Utilities.Version.CurrentVersion;
    }
}
