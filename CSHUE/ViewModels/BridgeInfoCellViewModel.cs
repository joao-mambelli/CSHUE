namespace CSHUE.ViewModels
{
    public class BridgeInfoCellViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Text back field.
        /// </summary>
        private string _text;
        /// <summary>
        /// Text property.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Is checked back field.
        /// </summary>
        private bool _isChecked;
        /// <summary>
        /// Is checked property.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Ip back field.
        /// </summary>
        private string _ip;
        /// <summary>
        /// Ip property.
        /// </summary>
        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Id back field.
        /// </summary>
        private string _id;
        /// <summary>
        /// Id property.
        /// </summary>
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
