using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace VpnIcon.Handler
{
    public static class WindowHelper
    {
        #region Public Constructors

        static WindowHelper()
        {
            slideInAnimation = new DoubleAnimation() { From = 1.0, To = 1.0, Duration = new Duration(TimeSpan.FromSeconds(0.2)), AutoReverse = false };
            slideInStoryboard = new Storyboard() { Name = "slideIn" };
            slideInStoryboard.Children.Add(slideInAnimation);

            slideOutAnimation = new DoubleAnimation() { From = 1.0, To = 1.0, Duration = new Duration(TimeSpan.FromSeconds(0.2)), AutoReverse = false };
            slideOutStoryboard = new Storyboard() { Name = "slideOut" };
            slideOutStoryboard.Children.Add(slideOutAnimation);
        }

        #endregion Public Constructors

        #region Private Fields

        private static DoubleAnimation slideInAnimation;
        private static Storyboard slideInStoryboard;
        private static DoubleAnimation slideOutAnimation;
        private static Storyboard slideOutStoryboard;

        #endregion Private Fields

        #region Public Methods

        public static void SlideIn(this Window window,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Debug.WriteLine($"SlideIn({window.Name}) from {memberName} in {sourceFilePath} at line {sourceLineNumber}");
            if (GetStoryboardStatus(window, slideInStoryboard) != ClockState.Active)
            {
                slideInAnimation.From = window.Width;
                SetWindowBasics(window, slideInAnimation, slideInStoryboard, Visibility.Hidden);
            }
        }

        private static ClockState GetStoryboardStatus(Window window, Storyboard storyboard)
        {
            try
            {
                DependencyObject obj = Storyboard.GetTarget(storyboard);
                if (obj == null) return ClockState.Stopped;
                return storyboard.GetCurrentState(obj as FrameworkElement);
            }
            catch (Exception)
            {
                return ClockState.Stopped;
            }
        }

        public static void SlideOut(this Window window,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (GetStoryboardStatus(window, slideOutStoryboard) != ClockState.Active)
            {
                System.Diagnostics.Debug.WriteLine($"SlideOut({window.Name}) from {memberName} in {sourceFilePath} at line {sourceLineNumber}");
                slideOutAnimation.To = window.Width;
                SetWindowBasics(window, slideOutAnimation, slideOutStoryboard, Visibility.Visible);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void SetWindowBasics(Window window, DoubleAnimation animation, Storyboard storyboard, Visibility visibility)
        {
            window.Height = SystemParameters.WorkArea.Height;
            object layoutObject = window.FindName("LayoutRoot");
            FrameworkElement layoutRoot = layoutObject as FrameworkElement;

            if (visibility == Visibility.Visible)
            {
                window.Topmost = true;
                window.ShowInTaskbar = false;
                window.Deactivated += Window_Deactivated;
                window.Show();
            }

            storyboard.Completed += Storyboard_Completed;

            if (layoutRoot != null)
            {
                layoutRoot.Width = animation.From.Value;
                Storyboard.SetTarget(storyboard, layoutRoot);
                Storyboard.SetTarget(animation, layoutRoot);
                Storyboard.SetTargetProperty(animation, new PropertyPath(FrameworkElement.WidthProperty));
                storyboard.Begin(layoutRoot, true);
            }
        }

        private static void Storyboard_Completed(object sender, EventArgs e)
        {
            string storyboardName = ((ClockGroup)sender).Timeline.Name;
            System.Diagnostics.Debug.WriteLine($"Completed {storyboardName}, {sender}, {e}");
            Storyboard storyboard = null;
            switch (storyboardName)
            {
                case "slideIn":
                    storyboard = slideInStoryboard;
                    break;
                case "slideOut":
                    storyboard = slideOutStoryboard;
                    break;
            }

            if (storyboard != null)
            {
                DependencyObject obj = Storyboard.GetTarget(storyboard);
                if (obj != null)
                {
                    Window window = Window.GetWindow(obj);
                    if (window != null)
                    {
                        if (storyboard == slideInStoryboard)
                        {
                            window.Topmost = false;
                            window.Deactivated -= Window_Deactivated;
                            window.Hide();
                        }
                        else
                        {
                            window.Focus();
                            window.Activate();
                        }
                    }
                }
                storyboard.Completed -= Storyboard_Completed;
            }
        }

        private static void TrySlideIn(object sender)
        {
            Window window = (sender as Window);
            if (window != null)
                window.SlideIn();
        }

        private static void Window_Deactivated(object sender, EventArgs e)
        {
            TrySlideIn(sender);
        }

        private static void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            TrySlideIn(sender);
        }

        #endregion Private Methods
    }
}