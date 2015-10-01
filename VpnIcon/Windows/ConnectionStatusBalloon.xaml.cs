using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace VpnIcon.Windows
{
    /// <summary>
    /// Interaction logic for ConnectionStatusBalloon.xaml
    /// </summary>
    public partial class ConnectionStatusBalloon : UserControl
    {
        public ConnectionStatusBalloon()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ConnectionStatusBalloon), new PropertyMetadata("Connecting..."));



        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set { SetValue(DetailProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Detail.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DetailProperty =
            DependencyProperty.Register("Detail", typeof(string), typeof(ConnectionStatusBalloon), new PropertyMetadata("Connecting to FLowtech (Live)"));




        public string StatusLeft
        {
            get { return (string)GetValue(StatusLeftProperty); }
            set { SetValue(StatusLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusLeftProperty =
            DependencyProperty.Register("StatusLeft", typeof(string), typeof(ConnectionStatusBalloon), new PropertyMetadata(DateTime.Now.ToLongDateString()));




        public string StatusRight
        {
            get { return (string)GetValue(StatusRightProperty); }
            set { SetValue(StatusRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StatusRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusRightProperty =
            DependencyProperty.Register("StatusRight", typeof(string), typeof(ConnectionStatusBalloon), new PropertyMetadata(DateTime.Now.ToLongTimeString()));


        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ConnectionStatusBalloon), new PropertyMetadata(null));

        private bool isClosing;

        /// <summary>
        /// Resolves the <see cref="TaskbarIcon"/> that displayed
        /// the balloon and requests a close action.
        /// </summary>
        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }

        /// <summary>
        /// By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent"/>
        /// and setting the "Handled" property to true, we suppress the popup
        /// from being closed in order to display the custom fade-out animation.
        /// </summary>
        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true; //suppresses the popup from being closed immediately
            isClosing = true;
        }

        /// <summary>
        /// If the users hovers over the balloon, we don't close it.
        /// </summary>
        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            //if we're already running the fade-out animation, do not interrupt anymore
            //(makes things too complicated for the sample)
            if (isClosing) return;

            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
        }


        /// <summary>
        /// Closes the popup once the fade-out animation completed.
        /// The animation was triggered in XAML through the attached
        /// BalloonClosing event.
        /// </summary>
        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            Popup pp = (Popup)Parent;
            pp.IsOpen = false;
        }


    }
}
