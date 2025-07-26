using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using track_widths.Core.Interfaces;
using track_widths.Core.Services;


namespace track_widths.Desktop.ViewModels
{
    public class CalculateViewModel: INotifyPropertyChanged
    {
        private readonly INavigationService _navigation;

        public ICommand OpenSecondViewCommand { get; }

        public CalculateViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            OpenSecondViewCommand = new RelayCommand(OpenSecondView);
        }

        private void OpenSecondView()
        {
            _navigation.NavigateTo<OtherView>(); // Для смены UserControl
                                                 // ИЛИ для нового окна:
                                                 // _navigation.ShowWindow<SecondWindow>();
        }
    }
}