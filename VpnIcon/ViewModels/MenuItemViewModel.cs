using System.Collections.Generic;
using System.Windows.Input;

namespace VpnIcon.ViewModels
{
    public class MenuItemViewModel : BaseViewModel
    {
        public MenuItemViewModel() : this(null)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref=""MenuItemViewModel"">   
        ///  class.
        /// </see></summary>
        /// The parent view model.
        public MenuItemViewModel(MenuItemViewModel parentViewModel) : base()
        {
            ParentViewModel = parentViewModel;
        }

        /// <summary>
        /// Gets the child menu items.
        /// </summary>
        /// <value>The child menu items.</value>
        public List<MenuItemViewModel> ChildMenuItems
        {
            get;
            protected set;
        } = new List<MenuItemViewModel>();

        private string _header;
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged("Header");
                }
            }
        }


        private bool mIsCheckable;
        public bool IsCheckable
        {
            get { return mIsCheckable; }
            protected set
            {
                if (value != mIsCheckable)
                {
                    OnPropertyChanging("IsCheckable");
                    mIsCheckable = value;
                    OnPropertyChanged("IsCheckable");
                }
            }
        }

        private bool mIsConnected;
        public virtual bool IsConnected
        {
            get { return mIsConnected; }
            set
            {
                if (value != mIsConnected)
                {
                    OnPropertyChanging("IsConnected");
                    mIsConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }


        private bool? mIsEnabled;
        public virtual bool IsEnabled
        {
            get
            {
                System.Diagnostics.Debug.Print($"Get Header: {Header}, IsEnabled: {mIsEnabled}");
                return mIsEnabled.GetValueOrDefault();
            }
            set
            {
                if (value != mIsEnabled)
                {
                    OnPropertyChanging("IsEnabled");
                    System.Diagnostics.Debug.Print($"Set Header: {Header}, IsEnabled: {mIsEnabled}, NewValue: {value}");
                    mIsEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the parent view model.
        /// </summary>
        /// <value>The parent view model.</value>
        public MenuItemViewModel ParentViewModel { get; set; }

        public object CommandParameter
        {
            get { return this; }
        }


        public virtual List<MenuItemViewModel> LoadChildMenuItems()
        {
            return new List<MenuItemViewModel>();
        }

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        /// <value>The image source.</value>
        public object IconSource { get; set; }


        private object mTag;
        public object Tag
        {
            get { return mTag; }
            set
            {
                if (value != mTag)
                {
                    OnPropertyChanging("Tag");
                    mTag = value;
                    OnPropertyChanged("Tag");
                }
            }
        }

    }
}
