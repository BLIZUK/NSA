using System.Windows;


namespace track_widths.Desktop.Views.CalculateTrack
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new track_widths.Desktop.ViewModels.CalculateTrack.MainViewModel();
        }
    }
}