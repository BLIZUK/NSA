using System.Configuration;
using System.Data;
using System.Windows;
using track_widths.Desktop.Views;
using track_widths.Desktop.ViewModels;
using track_widths.Core.Services;


namespace track_widths
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            var navigationService = new NavigationService(mainWindow.MainContent);


            mainWindow.Resources.Add("NavigationService", navigationService);


            navigationService.NavigateTo<CalculateView>();
            mainWindow.Show();

        }
    }
}
