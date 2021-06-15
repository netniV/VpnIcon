using System;
using System.Windows;
using VpnIcon.Handler;
using VpnIcon.Windows;

namespace VpnIcon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ProgramVersions history;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Window win = new Windows.MainWindow();
            App.Current.MainWindow = win;
            win.SlideIn();

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            //notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            //notifyIcon = new TaskbarIcon() { DataContext = new MainViewModel() };

        }

        public static ProgramVersions History
        {
            get
            {
                if (history == null)
                    history =
                        new ProgramVersions(
                            new ProgramVersion("2.1",
                                "Added system entry support"),
                            new ProgramVersion("2.0",
                                "Added Windows 10 AppBar Style",
                                "Added Connecting/Disconnecting statuses"
                                ),
                            new ProgramVersion("1.2",
                                "Added error window to display unhandled errors",
                                "Fixed issue where icon didn't show disconnected state",
                                "Fixed issue with connection status tooltip"),
                            new ProgramVersion("1.1",
                                "Added new icons for tray",
                                "Added program name/version info to menu",
                                "Fixed left click to show VPNs only",
                                "Fixed right click to show VPNs + extra options"),
                            new ProgramVersion("1.0",
                                "First edition to show VPN status and allow",
                                "Connection/Disconnection via tray")
                            );
                return history;
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            HandleException(ex);
        }

        void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            e.Handled = true;
            HandleException(ex);
        }

        static void HandleException(Exception ex)
        {
            if (ex != null)
            {
                //MessageBox.Show(ex.Message + Environment.NewLine + "Stack: " + ex.StackTrace, ex.Source);
                ExceptionWindow ew = new ExceptionWindow();
                ew.ExceptionObject = ex;
                ew.ShowDialog();
            }
        }


        protected override void OnExit(ExitEventArgs e)
        {
            //notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }

    }
}
