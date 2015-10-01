using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VpnIcon.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region · Property Changing/Changed Events ·

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null) =>
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion · Property Changing/Changed Events ·
    }
}
