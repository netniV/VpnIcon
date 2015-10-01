using System.Collections.Generic;

namespace VpnIcon.ViewModels
{
    public class ConnectionsGroupViewModel : ContainerItemViewModel<ConnectionViewModel>
    {
        public ConnectionsGroupViewModel() : this(null, null, null)
        {

        }

        public ConnectionsGroupViewModel(string Name) : this(null, Name, null)
        {

        }

        public ConnectionsGroupViewModel(MenuItemViewModel parentViewModel, string Name, List<ConnectionViewModel> connections) : base(parentViewModel, Name, connections)
        {
            
        }
    }
}
