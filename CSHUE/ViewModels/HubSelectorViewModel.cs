using System.Collections.Generic;

namespace CSHUE.ViewModels
{
    public class HubSelectorViewModel : BaseViewModel
    {
        private List<HubInfoCellViewModel> _list;
        public List<HubInfoCellViewModel> List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged();
            }
        }
    }
}
