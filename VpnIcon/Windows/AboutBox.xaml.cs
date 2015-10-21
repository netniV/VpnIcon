using System;
using System.Windows;
using System.Windows.Input;
using VpnIcon.Async;
using VpnIcon.Handler;

namespace VpnIcon.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutBox
    {
        public AboutBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }



        public string ProductTitle
        {
            get { return (string)GetValue(ProductTitleProperty); }
            set { SetValue(ProductTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductTitleProperty =
            DependencyProperty.Register("ProductTitle", typeof(string), typeof(AboutBox), new PropertyMetadata(RuntimeHelper.AssemblyProduct));

        public string ProductName
        {
            get { return (string)GetValue(ProductNameProperty); }
            set { SetValue(ProductNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductNameProperty =
            DependencyProperty.Register("ProductName", typeof(string), typeof(AboutBox), new PropertyMetadata(RuntimeHelper.AssemblyTitle));

        public string Version
        {
            get { return (string)GetValue(VersionProperty); }
            set { SetValue(VersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Version.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionProperty =
            DependencyProperty.Register("Version", typeof(string), typeof(AboutBox), new PropertyMetadata("v" + RuntimeHelper.Version));

        public string ProductVersion
        {
            get { return (string)GetValue(ProductVersionProperty); }
            set { SetValue(ProductVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductVersionProperty =
            DependencyProperty.Register("ProductVersion", typeof(string), typeof(AboutBox), new PropertyMetadata("Version " + RuntimeHelper.AssemblyVersion));

        public string ProductCopyright
        {
            get { return (string)GetValue(ProductCopyrightProperty); }
            set { SetValue(ProductCopyrightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductCopyright.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductCopyrightProperty =
            DependencyProperty.Register("ProductCopyright", typeof(string), typeof(AboutBox), new PropertyMetadata(RuntimeHelper.AssemblyCopyright));



        public string ProductDescription
        {
            get { return (string)GetValue(ProductDescriptionProperty); }
            set { SetValue(ProductDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductDescriptionProperty =
            DependencyProperty.Register("ProductDescription", typeof(string), typeof(AboutBox), new PropertyMetadata(RuntimeHelper.AssemblyDescription));


        public string CompanyName
        {
            get { return (string)GetValue(CompanyNameProperty); }
            set { SetValue(CompanyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CompanyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompanyNameProperty =
            DependencyProperty.Register("CompanyName", typeof(string), typeof(AboutBox), new PropertyMetadata(RuntimeHelper.AssemblyCompany));



        public Visibility VersionHistoryVisibility
        {
            get { return (Visibility)GetValue(VersionHistoryVisibilityProperty); }
            set { SetValue(VersionHistoryVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VersionHistoryVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionHistoryVisibilityProperty =
            DependencyProperty.Register("VersionHistoryVisibility", typeof(Visibility), typeof(AboutBox), new PropertyMetadata(Visibility.Visible));


        public string VersionHistory
        {
            get { return (string)GetValue(VersionHistoryProperty); }
            set { SetValue(VersionHistoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VersionHistory.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionHistoryProperty =
            DependencyProperty.Register("VersionHistory", typeof(string), typeof(AboutBox), new PropertyMetadata(AboutBox.MainVersionHistory, new PropertyChangedCallback(VersionHistoryChanged)));

        private static void VersionHistoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(VersionHistoryVisibilityProperty, String.IsNullOrWhiteSpace(e.NewValue as string) ? Visibility.Collapsed : Visibility.Visible);
        }



        public Visibility AdditionalContentVisiblity
        {
            get { return (Visibility)GetValue(AdditionalContentVisiblityProperty); }
            set { SetValue(AdditionalContentVisiblityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdditionalContentVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdditionalContentVisiblityProperty =
            DependencyProperty.Register("AdditionalContentVisiblity", typeof(Visibility), typeof(AboutBox), new PropertyMetadata(Visibility.Collapsed));




        public object AdditionalContent
        {
            get { return (object)GetValue(AdditionalContentProperty); }
            set { SetValue(AdditionalContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdditionalContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdditionalContentProperty =
            DependencyProperty.Register("AdditionalContent", typeof(object), typeof(AboutBox), new PropertyMetadata(null, new PropertyChangedCallback(AdditionalContentChanged)));

        private static void AdditionalContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(AdditionalContentVisiblityProperty, e.NewValue == null ? Visibility.Collapsed : Visibility.Visible);
        }

        public string AboutText
        {
            get { return (string)GetValue(AboutTextProperty); }
            set { SetValue(AboutTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AboutText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AboutTextProperty =
            DependencyProperty.Register("AboutText", typeof(string), typeof(AboutBox), new PropertyMetadata("About " + RuntimeHelper.AssemblyProduct + " ... "));



        #region " OK Command "
        private RelayCommand mOKCommand;

        public ICommand OKCommand
        {
            get
            {
                if (mOKCommand == null)
                    mOKCommand = new RelayCommand(doOKCommand, canOKCommand);

                return mOKCommand;
            }
        }

        public bool canOKCommand(object obj)
        {
            //TODO: Place code here to validate when command can run
            return true;
        }

        public void doOKCommand(object obj)
        {
            if (canOKCommand(obj))
            {
                //this.DialogResult = true;
            }
        }

        public Visibility showOKCommand
        {
            get
            {
                return canOKCommand(null) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region Assembly Attribute Accessors

        public static string MainVersionHistory
        {
            get
            {
                return App.History.ToString();
            }
        }
        #endregion
    }
}
