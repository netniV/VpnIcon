namespace VpnIcon.ViewModels
{
    public class SeparatorViewModel : MenuItemViewModel
    {
        public SeparatorViewModel() : base(null)
        {

        }
        public SeparatorViewModel(MenuItemViewModel parentViewModel) : base(parentViewModel)
        {
            Header = "-";
        }
    }
}
