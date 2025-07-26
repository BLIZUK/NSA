using System;
using System.Windows;
using System.Windows.Controls;
using track_widths.Core.Interfaces;


namespace track_widths.Core.Services
{
    public class NavigationService: INavigationService
    {
        private ContentControl _contentHost;


        public NavigationService(ContentControl contentHost)
        {
            _contentHost = contentHost;
        }


        public void NavigateTo<T>() where T : UserControl
        {
            _contentHost.Content = Activator.CreateInstance<T>();
        }


        public void ShowWindow<T>() where T : Window, new()
        {
            var window = new T();
            window.Show();
        }
    }
}
