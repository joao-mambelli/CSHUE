namespace CSHUE.ViewModels
{
    public class UpdateViewModel : BaseViewModel
    {
        #region Properties

        private string _link;
        public string Link
        {
            get => _link;
            set
            {
                _link = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// Main window viewmodel field.
        /// </summary>
        public MainWindowViewModel MainWindowViewModel = null;

        #endregion
    }
}
