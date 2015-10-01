using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;

namespace VpnIcon.Handler
{
    public class BalloonTipEventArgs : EventArgs
    {
        public BalloonTipEventArgs()
        {

        }

        public string Title { get; set; }
        public string Message { get; set; }

        public BalloonIcon Icon { get; set; }
    }

    public static class BalloonHelper
    {
        public static readonly DependencyProperty StandardBalloonProperty =
            DependencyProperty.RegisterAttached("StandardBalloon", typeof(BalloonTipEventArgs), typeof(BalloonHelper), new PropertyMetadata(StandardBalloonChanged));

        private static void StandardBalloonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var taskbarIcon = d as TaskbarIcon;
            var tips = e.NewValue as BalloonTipEventArgs;
            if (taskbarIcon != null)
            {
                taskbarIcon.HideBalloonTip();
                if (e.NewValue != null)
                    taskbarIcon.ShowBalloonTip(tips.Title, tips.Message, tips.Icon);
            }
        }

        public static void SetStandardBalloon(TaskbarIcon target, BalloonTipEventArgs value)
        {
            target.SetValue(StandardBalloonProperty, value);
        }

        public static BalloonTipEventArgs GetStandardBalloon(TaskbarIcon target)
        {
            return target.GetValue(StandardBalloonProperty) as BalloonTipEventArgs;
        }

        public static readonly DependencyProperty CustomBalloonProperty =
            DependencyProperty.RegisterAttached("CustomBalloon", typeof(UIElement), typeof(BalloonHelper), new PropertyMetadata(CustomBalloonChanged));

        private static void CustomBalloonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var taskbarIcon = d as TaskbarIcon;

            if (taskbarIcon != null)
            {
                taskbarIcon.HideBalloonTip();
                if (e.NewValue as UIElement != null)
                    taskbarIcon.ShowCustomBalloon(e.NewValue as UIElement, System.Windows.Controls.Primitives.PopupAnimation.Fade, 500);
            }
        }

        public static void SetCustomBalloon(TaskbarIcon target, UIElement value)
        {
            target.SetValue(CustomBalloonProperty, value);
        }

        public static UIElement GetCustomBalloon(TaskbarIcon target)
        {
            return target.GetValue(CustomBalloonProperty) as UIElement;
        }

    }
}
