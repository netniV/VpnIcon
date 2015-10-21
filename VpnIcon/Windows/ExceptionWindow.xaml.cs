using System;
using System.Windows;

namespace VpnIcon.Windows
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window
    {
        public ExceptionWindow()
        {
            InitializeComponent();
        }




        public Type ExceptionType
        {
            get { return (Type)GetValue(ExceptionTypeProperty); }
            set { SetValue(ExceptionTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExceptionType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExceptionTypeProperty =
            DependencyProperty.Register("ExceptionType", typeof(Type), typeof(ExceptionWindow), new PropertyMetadata(null));


        public Exception ExceptionObject
        {
            get { return (Exception)GetValue(ExceptionObjectProperty); }
            set { SetValue(ExceptionObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExceptionObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExceptionObjectProperty =
            DependencyProperty.Register("ExceptionObject", typeof(Exception), typeof(ExceptionWindow), new PropertyMetadata(null, new PropertyChangedCallback(ExceptionObjectChanged)));

        private static void ExceptionObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ExceptionTypeProperty, e.NewValue?.GetType());
        }
    }
}
