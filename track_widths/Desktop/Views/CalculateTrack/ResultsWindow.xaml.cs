using System.Windows;
using track_widths.Core.Models;

namespace track_widths.Desktop.Views
{
    public partial class ResultsWindow : Window
    {
        public ResultsWindow(string results)
        {
            InitializeComponent();
            ResultsTextBox.Text = results;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ResultsTextBox.Text);
            MessageBox.Show("Результаты скопированы в буфер обмена!",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}