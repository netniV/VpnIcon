using System.Windows;
using System.Windows.Controls;
using VpnIcon.ViewModels;

namespace VpnIcon.Selectors
{
    public class MenuStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item == null || item is SeparatorViewModel)
            {
                return ((FrameworkElement)container).FindResource("stMenuSeparator") as Style;
            }
            return base.SelectStyle(item, container);
        }
    }
}
