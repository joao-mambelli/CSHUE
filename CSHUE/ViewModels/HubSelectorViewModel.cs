using System.Collections.ObjectModel;

namespace CSHUE.ViewModels
{
    public class HubSelectorViewModel : BaseViewModel
    {
        #region Properties

        private ObservableCollection<HubInfoCellViewModel> _list;
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
