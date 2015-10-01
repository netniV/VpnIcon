using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace VpnIcon
{
    public static class IconHandler
    {
        public static string DisconnectedFile => "TrayIcon Disconnected.ico";
        public static BitmapSource DisconnectedSource => ImageFromResource(DisconnectedFile);

        public static string CloseFile => "Close.png";
        public static BitmapSource CloseSource => ImageFromResource(CloseFile);

        public static string ConnectedFile => "TrayIcon Connected.ico";
        public static BitmapSource ConnectedSource => ImageFromResource(ConnectedFile);

        public static BitmapSource ImageFromResource(string fileName)
        {
            string assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri($"pack://application:,,,/{assemblyName};component/Resources/{fileName}");
            logo.EndInit();
            return logo;
        }
    }
}
