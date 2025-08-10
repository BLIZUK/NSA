using System.Globalization;
using System.Windows;
using static track_widths.Desktop.Views.MainWindow;
using track_widths.Desktop.ViewModels.Default;
using track_widths.Desktop.Views;
using track_widths.Core;

namespace track_widths.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Свойства для привязки данных
        private string _amperageText;
        public string AmperageText
        {
            get => _amperageText;
            set => SetField(ref _amperageText, value);
        }

        private string _thicknessText;
        public string ThicknessText
        {
            get => _thicknessText;
            set => SetField(ref _thicknessText, value);
        }

        // ... аналогично для других полей (RiseTempText, AmbientTempText, LengthText)

        // Свойства для выбранных единиц измерения
        private UnitItem _selectedAmperageUnit;
        public UnitItem SelectedAmperageUnit
        {
            get => _selectedAmperageUnit;
            set => SetField(ref _selectedAmperageUnit, value);
        }

        // ... аналогично для других единиц измерения

        // Коллекции для ComboBox
        public List<UnitItem> AmperageUnits { get; } = new();
        public List<UnitItem> WidthUnits { get; } = new();
        public List<UnitItem> ThicknessUnits { get; } = new();
        public List<UnitItem> TempUnits { get; } = new();
        public List<UnitItem> LengthUnits { get; } = new();

        // Команда для расчета
        public RelayCommand CalculateCommand { get; }

        public MainViewModel()
        {
            InitializeUnits();
            CalculateCommand = new RelayCommand(_ => Calculate());
        }

        private void InitializeUnits()
        {
            // Инициализация единиц измерения (как в вашем оригинальном коде)
            AmperageUnits.AddRange(new[] {
                new UnitItem("A", 1.0),
                new UnitItem("mA", 0.001),
                new UnitItem("mkA", 0.000001)
            });

            // ... инициализация других коллекций

            // Установка значений по умолчанию
            SelectedAmperageUnit = AmperageUnits[0];
            // ... аналогично для других
        }

        private void Calculate()
        {
            try
            {
                // Перенесенная логика из CountButton_Click
                double current = GetValue(AmperageText, SelectedAmperageUnit);
                // ... остальные значения

                // Выполнение расчетов
                var calculator = new TrackCalculator();
                var result = calculator.Calculate(current, ambientTemp, thickness, tempRise, length);

                // Отображение результатов
                ShowResults(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка расчета",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double GetValue(string text, UnitItem unit)
        {
            if (!double.TryParse(text.Replace('.', ','), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                throw new ArgumentException("Некорректное числовое значение");

            return value * unit.Multiplier;
        }

        private void ShowResults(CalculationResult result)
        {
            // Логика открытия окна результатов
            var resultsWindow = new ResultsWindow(result.ToString());
            resultsWindow.Show();
        }
    }
}