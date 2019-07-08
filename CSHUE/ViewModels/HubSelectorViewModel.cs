using System.Collections.ObjectModel;

namespace CSHUE.ViewModels
{
    public class HubSelectorViewModel : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// List back field.
        /// </summary>
        private ObservableCollection<HubInfoCellViewModel> _list;
        /// <summary>
        /// List property.
        /// </summary>
        public ObservableCollection<HubInfoCellViewModel> List
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
