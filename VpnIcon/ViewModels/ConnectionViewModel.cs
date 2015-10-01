using DotRas;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VpnIcon.ViewModels
{
    public class ConnectionViewModel : MenuItemViewModel
    {
        public ConnectionViewModel(MenuItemViewModel parentViewModel) : base(parentViewModel)
        {
        }

        private RasEntry mEntry;
        public RasEntry Entry
        {
            get { return mEntry; }
            set
            {
                if (value != mEntry)
                {
                    OnPropertyChanging("Entry");
                    mEntry = value;
                    GroupName = null;
                    if (value != null)
                    {
                        Regex nameMatch = new Regex("(.*?)\\((.*?)\\)");
                        var result = nameMatch.Match(value.Name);
                        if (result.Success)
                        {
                            Name = result.Groups[result.Groups.Count - 1].Value;
                            GroupName = result.Groups.Count < 3 ? null : result.Groups[1].Value;
                        }
                        else
                            Name = value.Name;
                    }
                    else
                    {
                        Name = "Unknown";
                    }

                    OnPropertyChanged("Entry");
                }
            }
        }

        public string FullName => Entry?.Name;

        public System.Guid Id => (Entry?.Id).GetValueOrDefault();
        public RasConnection ActiveConnection => RasConnection.GetActiveConnections().Where(x => x.EntryId == Id).FirstOrDefault();

        public bool IsChanging => !IsEnabled;

        public override bool IsEnabled
        {
            get
            {
                base.IsEnabled = Dialer == null;
                return base.IsEnabled;
            }

            set
            {
                base.IsEnabled = Dialer == null;
            }
        }

        public override bool IsConnected
        {
            get
            {
                return (ActiveConnection != null);
            }

            set
            {
                base.IsConnected = (ActiveConnection != null);
            }
        }
        public RasHandle Handle { get; private set; }

        private RasDialer Dialer { get; set; }

        private object DialerLock = new object();

        private string mGroupName;
        public string GroupName
        {
            get { return mGroupName; }
            private set
            {
                if (value != mGroupName)
                {
                    OnPropertyChanging("GroupName");
                    mGroupName = value;
                    OnPropertyChanged("GroupName");
                }
            }
        }

        private string mName;
        public string Name
        {
            get { return mName; }
            private set
            {
                if (value != mName)
                {
                    OnPropertyChanging("FullName");
                    OnPropertyChanging("Name");
                    mName = value;
                    Header = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("FullName");
                }
            }
        }


        public async Task Connect()
        {
            await Task.Run(() =>
            {
                if (IsConnected)
                    return;

                lock (DialerLock)
                {
                    if (IsChanging)
                        return;

                    Dialer = new RasDialer();
                }

                try
                {
                    SetupDialer(Dialer);
                    Handle = Dialer.DialAsync();
                }
                catch (Exception ex)
                {
                    Dialer_Error(this, new System.IO.ErrorEventArgs(ex));
                }
            });
        }

        public async Task Disconnect()
        {
            await Task.Run(() =>
            {
                if (!IsConnected)
                    return;

                lock (DialerLock)
                {
                    if (IsChanging)
                        return;

                    Dialer = new RasDialer();
                }

                try
                {
                    SetupDialer(Dialer);
                    ActiveConnection.HangUp();
                }
                catch (Exception ex)
                {
                    Dialer_Error(this, new System.IO.ErrorEventArgs(ex));
                }
            });
        }

        private void SetupDialer(RasDialer dialer)
        {
            Dialer.Error += Dialer_Error;
            Dialer.DialCompleted += Dialer_DialCompleted;
            Dialer.StateChanged += Dialer_StateChanged;
            Dialer.AllowUseStoredCredentials = true;
            Dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User);
            Dialer.EntryName = Entry.Name;
        }

        public event EventHandler<StateChangedEventArgs> StateChanged;
        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<DialCompletedEventArgs> DialCompleted;

        private void Dialer_Error(object sender, System.IO.ErrorEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        private void Dialer_StateChanged(object sender, StateChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        private void Dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            DialCompleted?.Invoke(this, e);
            lock (DialerLock)
            {
                if (Dialer != null)
                {
                    Dialer.Dispose();
                    Dialer = null;
                }
            }
            OnPropertyChanged(nameof(IsConnected));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (Dialer != null)
                    {
                        Dialer.Dispose();
                        Dialer = null;
                    }

                    if (Handle != null)
                    {
                        Handle.Dispose();
                        Handle = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Connection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}