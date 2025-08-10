using System.Globalization;
using System.Windows;
using track_widths.Desktop.ViewModels.Default;
using track_widths.Desktop.Views;
using track_widths.Core.Models;


namespace track_widths.Desktop.ViewModels.CalculateTrack
{
    public class MainViewModel : ViewModelBase
    {
        // Свойства для привязки данных
        private string _amperageError;
        public string AmperageError
        {
            get => _amperageError;
            set => SetField(ref _amperageError, value);
        }

        // Для каждого свойства добавляем валидацию
        private string _amperageText;
        public string AmperageText
        {
            get => _amperageText;
            set
            {
                SetField(ref _amperageText, value);
                ValidateAmperage(value);
            }
        }

        private void ValidateAmperage(string value)
        {
            if (value == "*")
            {
                AmperageError = "Поле обязательно для заполнения";
            }
            else if (!string.IsNullOrEmpty(value) &&
                    !double.TryParse(value.Replace('.', ','), out _))
            {
                AmperageError = "Некорректное числовое значение";
            }
            else
            {
                AmperageError = null;
            }
        }

        public string? ThicknessText { get; set; }
        public string? RiseTempText { get; set; }
        public string? AmbientTempText { get; set; }
        public string? LengthText { get; set; }

        // Свойства для выбранных единиц измерения
        public UnitItem? SelectedAmperageUnit { get; set; }
        public UnitItem? SelectedWidthUnits { get; set; }
        public UnitItem? SelectedThicknessUnits { get; set; }
        public UnitItem? SelectedLengthUnits { get; set; }

        // Отдельные свойства для температур
        public UnitItem? SelectedAmbientTempUnit { get; set; }
        public UnitItem? SelectedRiseTempUnit { get; set; }

        // Коллекции для ComboBox
        public List<UnitItem> AmperageUnits { get; } = [];
        public List<UnitItem> WidthUnits { get; } = [];
        public List<UnitItem> ThicknessUnits { get; } = [];
        public List<UnitItem> TempUnits { get; } = [];
        public List<UnitItem> LengthUnits { get; } = [];

        // Команда для расчета
        public RelayCommand CalculateCommand { get; }

        public MainViewModel()
        {
            InitializeUnits();
            CalculateCommand = new RelayCommand(_ => Calculate());
        }

        private void InitializeUnits()
        {
            
            AmperageUnits.AddRange([
                new UnitItem("A", 1.0),
                new UnitItem("mA", 0.001),
                new UnitItem("mkA", 0.000001)
            ]);

            WidthUnits.AddRange([
            
                new UnitItem ("мил", 0.0254),
                new UnitItem ("см", 10.0),
                new UnitItem ("мм",1.0),
                new UnitItem ("мкм", 0.001),
                new UnitItem("дюйм", 25.4)
            ]);

            ThicknessUnits.AddRange([

                new UnitItem("унция/фут²", 1.37),
                new UnitItem("мил", 1.0),
                new UnitItem("мм", 39.3701),    
                new UnitItem("мкм", 0.0393701), 
                new UnitItem("дюйм", 1000.0)    
            ]);

            TempUnits.AddRange([
                new UnitItem("°C", 1.0),
                new UnitItem("°K", 1.0)
            ]);

            LengthUnits.AddRange([

                new UnitItem ("мил", 0.0254),
                new UnitItem ("см", 10.0),
                new UnitItem ("мм",1.0),
                new UnitItem ("мкм", 0.001),
                new UnitItem("дюйм", 25.4)
            ]);

            // Установка значений по умолчанию
            SelectedAmperageUnit = AmperageUnits[0];
            SelectedWidthUnits = WidthUnits[2];
            SelectedThicknessUnits = ThicknessUnits[3];
            SelectedLengthUnits = LengthUnits[2];

            SelectedAmbientTempUnit = TempUnits[0]; 
            SelectedRiseTempUnit = TempUnits[0];    
        }


        private double GetValue(string text, UnitItem unit)
        {
            if (text == "*") return double.NaN;

            if (!double.TryParse(text.Replace('.', ','), NumberStyles.Any,
                CultureInfo.InvariantCulture, out double value))
            {
                throw new ArgumentException("Некорректное числовое значение");
            }

            return value * unit.Multiplier;
        }

        private void Calculate()
        {
            try
            {
                double current = GetValue(AmperageText, SelectedAmperageUnit);
                double ambientTemp = GetValue(AmbientTempText, SelectedAmbientTempUnit);
                double thickness = GetValue(ThicknessText, SelectedThicknessUnits);
                double tempRise = GetValue(RiseTempText, SelectedRiseTempUnit);
                double length = GetValue(LengthText, SelectedLengthUnits);

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

        private double HandleEmptyValue(string text, UnitItem unit, string fieldName)
        {
            if (text == "*")
                throw new ArgumentException($"{fieldName} не может быть пустым");

            return GetValue(text, unit);
        }


        private void ShowResults(CalculationResult result)
        {
            double widthMultiplier = SelectedWidthUnits.Multiplier;
            result = result with
            {
                ExternalWidth = result.ExternalWidth * 0.0254 / widthMultiplier,
                InternalWidth = result.InternalWidth * 0.0254 / widthMultiplier
            };

            var resultsWindow = new ResultsWindow(result.ToString(SelectedWidthUnits.Name))
            {
                Owner = Application.Current.MainWindow
            };
            resultsWindow.Show();
        }
    }
}