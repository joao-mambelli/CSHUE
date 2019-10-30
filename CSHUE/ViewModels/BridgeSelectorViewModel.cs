using System.Collections.ObjectModel;

namespace CSHUE.ViewModels
{
    public class BridgeSelectorViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// List back field.
        /// </summary>
        private ObservableCollection<BridgeInfoCellViewModel> _list;
        /// <summary>
        /// List property.
        /// </summary>
        public ObservableCollection<BridgeInfoCellViewModel> List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
