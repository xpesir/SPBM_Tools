using System;
using System.Windows;

namespace XML_SPXX
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var uri = new Uri("/PresentationFramework.AeroLite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35;component/themes/AeroLite.NormalColor.xaml", UriKind.Relative);
            App.Current.Resources.Source = uri;
            base.OnStartup(e);
        }
    }
}
