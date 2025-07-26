using System.Windows;
using System.Windows.Controls;


namespace track_widths.Core.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<T>() where T : UserControl;
        void ShowWindow<T>() where T : Window, new();
    }
}
