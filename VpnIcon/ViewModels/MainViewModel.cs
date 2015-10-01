using DotRas;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VpnIcon.Async;
using VpnIcon.Handler;
using VpnIcon.Windows;

namespace VpnIcon.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel() : base()
        {
            mPhoneBook = RasPhoneBook.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
            mIconSource = IconHandler.DisconnectedSource;

            Dispatcher = Dispatcher.CurrentDispatcher;
            PhoneBook = RasPhoneBook.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));

            Watcher = new RasConnectionWatcher() { EnableRaisingEvents = true };
            Watcher.Connected += Watcher_Connected;
            Watcher.Disconnected += Watcher_Disconnected;
            Watcher.Error += Watcher_Error;

            UpdateTrayIcon();
        }

        #region · Properties ·

        #region · Internal Properties ·

        protected Dispatcher Dispatcher { get; }
        protected RasConnectionWatcher Watcher { get; }

        #endregion

        #region · Public Properties ·


        private BalloonTipEventArgs mBalloonTip;
        public BalloonTipEventArgs BalloonTip
        {
            get { return mBalloonTip; }
            set
            {
                if (value != mBalloonTip)
                {
                    OnPropertyChanging("BalloonTip");
                    mBalloonTip = value;
                    OnPropertyChanged("BalloonTip");
                }
            }
        }

        private ConnectionStatusBalloon mBalloon;
        public ConnectionStatusBalloon Balloon
        {
            get { return mBalloon; }
            set
            {
                if (value != mBalloon)
                {
                    OnPropertyChanging("Balloon");
                    mBalloon = value;
                    OnPropertyChanged("Balloon");
                }
            }
        }

        private RasPhoneBook mPhoneBook;
        public RasPhoneBook PhoneBook
        {
            get { return mPhoneBook; }
            set
            {
                if (value != mPhoneBook)
                {
                    OnPropertyChanging("PhoneBook");
                    mPhoneBook = value;
                    Entries = mPhoneBook?.Entries;
                    OnPropertyChanged("PhoneBook");
                }
            }
        }

        private ObservableCollection<RasEntry> mEntries;
        public ObservableCollection<RasEntry> Entries
        {
            get { return mEntries; }
            set
            {
                if (value != mEntries)
                {
                    OnPropertyChanging("Entries");
                    if (mEntries != null)
                        mEntries.CollectionChanged -= Entries_CollectionChanged;
                    mEntries = value;
                    if (mEntries != null)
                        mEntries.CollectionChanged += Entries_CollectionChanged;
                    Entries_CollectionChanged(value, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
                    OnPropertyChanged("Entries");
                }
            }
        }

        private ImageSource mIconSource;
        public ImageSource IconSource
        {
            get { return mIconSource; }
            set
            {
                if (value != mIconSource)
                {
                    OnPropertyChanging("IconSource");
                    mIconSource = value;
                    OnPropertyChanged("IconSource");
                }
            }
        }

        private ConnectionsGroupViewModel mUngroupedMenuItems;
        public ConnectionsGroupViewModel UngroupedMenuItems
        {
            get { return mUngroupedMenuItems; }
            set
            {
                if (value != mUngroupedMenuItems)
                {
                    OnPropertyChanging("UngroupedMenuItems");
                    mUngroupedMenuItems = value;
                    OnPropertyChanged("UngroupedMenuItems");
                }
            }
        }

        private ConnectionsGroupsViewModel mGroupedMenuItems;
        public ConnectionsGroupsViewModel GroupedMenuItems
        {
            get { return mGroupedMenuItems; }
            set
            {
                if (value != mGroupedMenuItems)
                {
                    OnPropertyChanging("GroupedMenuItems");
                    mGroupedMenuItems = value;
                    OnPropertyChanged("GroupedMenuItems");
                }
            }
        }

        #endregion

        #endregion

        #region · Methods ·

        #region · Internal Methods ·

        private void ShowBalloon(string title, string detail)
        {
            ShowBalloon(title, detail, IconHandler.ConnectedSource);
        }
        private void ShowBalloon(string title, string detail, ImageSource imageSource = null, BalloonIcon icon = BalloonIcon.Info, bool useCustom = false)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (useCustom)
                    Balloon = new ConnectionStatusBalloon() { Title = title, Detail = detail, ImageSource = imageSource };
                else
                    BalloonTip = new BalloonTipEventArgs() { Icon = icon, Message = detail, Title = title };
            }));
        }

        private async Task CreateCollectionContext()
        {
            ConnectionsGroupsViewModel groupedItems = new ConnectionsGroupsViewModel();
            ConnectionsGroupViewModel ungroupedItems = await CreateConnections(Entries).ConfigureAwait(false);
            var groups = ungroupedItems.GroupBy(x => x.GroupName).OrderBy(x => x.Key);

            List<Task> groupTasks = new List<Task>();
            foreach (var group in groups)
                if (!string.IsNullOrWhiteSpace(group.Key))
                {
                    ConnectionsGroupViewModel groupedItem = new ConnectionsGroupViewModel(group.Key);
                    groupedItems.Add(groupedItem);

                    var t = UpdateConnectionGroups(groupedItem, ungroupedItems, group);
                    groupTasks.Add(t);
                }

            await Task.WhenAll(groupTasks.ToArray()).ConfigureAwait(false);

            await Task.Run(() =>
            {
                UngroupedMenuItems = ungroupedItems;
            });

            await Task.Run(() =>
            {
                GroupedMenuItems = groupedItems;
            });
        }

        private async Task UpdateConnectionGroups(ConnectionsGroupViewModel groupedItems, ConnectionsGroupViewModel ungroupedItems, IGrouping<string, ConnectionViewModel> group)
        {
            await Task.Run(() =>
            {
                var localItems = groupedItems;
                var localGroup = group;

                foreach (var connection in group)
                {
                    ungroupedItems.Remove(connection);
                    groupedItems.Add(connection);
                }
            });
        }

        private async Task<ConnectionsGroupViewModel> CreateConnections(ObservableCollection<RasEntry> entries)
        {
            return await Task.Run<ConnectionsGroupViewModel>(() =>
            {
                var localEntries = entries.OrderBy(x => x.Name);

                var localConnections = new ConnectionsGroupViewModel();
                if (localEntries != null)
                    foreach (var entry in localEntries)
                    {
                        var connection = new ConnectionViewModel(null) { Entry = entry };
                        connection.DialCompleted += Connection_DialCompleted;
                        connection.StateChanged += Connection_StateChanged;
                        connection.Error += Connection_Error;
                        localConnections.Add(connection);
                    }

                return localConnections;
            });

        }

        private void UpdateTrayIcon()
        {
            Dispatcher.BeginInvoke(new Action(() => { IconSource = RasConnection.GetActiveConnections()?.Count() > 0 ? IconHandler.ConnectedSource : IconHandler.DisconnectedSource; }));
        }

        #endregion

        #region · Event Handlers ·

        private void Connection_Error(object sender, System.IO.ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Connection_StateChanged(object sender, StateChangedEventArgs e)
        {
            ShowBalloon((sender as ConnectionViewModel).GroupName, e.State.ToString(), useCustom: true);
        }

        private void Connection_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            ConnectionViewModel cvm = sender as ConnectionViewModel;

            if (e.Error != null)
                ShowBalloon($"{cvm.FullName}", $"{e.Error.GetType().Name} + {e.Error.Message}", icon: BalloonIcon.Error);
            else if (e.TimedOut)
                ShowBalloon($"{cvm.FullName}", $"Connection timed out", icon: BalloonIcon.Warning);
            else if (e.Cancelled)
                ShowBalloon($"{cvm.FullName}", $"Connection was cancelled", icon: BalloonIcon.Warning);
        }

        private void Entries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Task t = Task.Run(() => CreateCollectionContext());
            if (t.Status == TaskStatus.Faulted)
            {
                System.Windows.MessageBox.Show(t.Exception?.Message, t.Exception?.GetType().FullName);
            }
        }

        private void Watcher_Disconnected(object sender, RasConnectionEventArgs e)
        {
            ShowBalloon("Connection closed", $"{e.Connection?.EntryName} has been closed and is no longer active.", icon: BalloonIcon.Warning);
        }

        private void Watcher_Connected(object sender, RasConnectionEventArgs e)
        {
            ShowBalloon("Connection closed", $"{e.Connection?.EntryName} has been dialed and is now active.");
            UpdateTrayIcon();
        }

        private void Watcher_Error(object sender, System.IO.ErrorEventArgs e)
        {
            Exception ex = e.GetException();
            ShowBalloon("Connection Error", $"{ex?.GetType().FullName} occured ('{ex?.Message}", icon: BalloonIcon.Error);
        }

        #endregion

        #region · Commands ·

        #region · Connection Command ·
        private RelayCommand mConnectionCommand;

        public ICommand ConnectionCommand
        {
            get
            {
                if (mConnectionCommand == null)
                    mConnectionCommand = new RelayCommand(doConnectionCommand, canConnectionCommand);

                return mConnectionCommand;
            }
        }

        public bool canConnectionCommand(object obj)
        {
            //TODO: Place code here to validate when command can run
            return !(((obj as ConnectionViewModel)?.IsChanging) ?? false);
        }

        public async void doConnectionCommand(object obj)
        {
            if (canConnectionCommand(obj))
            {
                ConnectionViewModel cvm = obj as ConnectionViewModel;
                if (cvm != null)
                    if (cvm.IsConnected)
                        await cvm.Disconnect();
                    else
                        await cvm.Connect();
            }
        }
        #endregion

        #region · ExitApplication Command ·
        private RelayCommand mExitApplicationCommand;

        public ICommand ExitApplicationCommand
        {
            get
            {
                if (mExitApplicationCommand == null)
                    mExitApplicationCommand = new RelayCommand(doExitApplicationCommand, canExitApplicationCommand);

                return mExitApplicationCommand;
            }
        }

        public bool canExitApplicationCommand(object obj)
        {
            //TODO: Place code here to validate when command can run
            return true;
        }

        public void doExitApplicationCommand(object obj)
        {
            if (canExitApplicationCommand(obj))
            {
                App.Current?.Shutdown();
            }
        }
        #endregion

        #endregion

        #endregion
    }
}

