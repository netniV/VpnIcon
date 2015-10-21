using System.Windows;

namespace VpnIcon.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            SizeChanged += (o, e) =>
            {
                var r = SystemParameters.WorkArea;
                Left = r.Right - ActualWidth;
                Top = r.Bottom - ActualHeight;
            };

            Deactivated += (o, e) =>
            {
                if (IsVisible)
                    this.Close();
            };

            InitializeComponent();
        }
    }
}
