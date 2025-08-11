using System.Globalization;
using System.Windows;
using track_widths.Desktop.ViewModels.Default;
using track_widths.Desktop.Views;
using track_widths.Core.Models;


namespace track_widths.Desktop.ViewModels.CalculateTrack
{
    public class MainViewModel : ViewModelBase
    {
        public string? AmperageText { get; set; }
        public string? ThicknessText { get; set; }
        public string? RiseTempText { get; set; }
        public string? AmbientTempText { get; set; }
        public string? LengthText { get; set; }

        // Свойства для выбранных единиц измерения
        public UnitItem SelectedAmperageUnit { get; set; } = null!;
        public UnitItem SelectedWidthUnits { get; set; } = null!;
        public UnitItem SelectedThicknessUnits { get; set; } = null!;
        public UnitItem SelectedAmbientTempUnit { get; set; } = null!;
        public UnitItem SelectedRiseTempUnit { get; set; } = null!;
        public UnitItem SelectedLengthUnits { get; set; } = null!;

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
            AmperageText = "*";
            ThicknessText = "*";
            RiseTempText = "*";
            AmbientTempText = "*";
            LengthText = "*";
        
            InitializeUnits();
            CalculateCommand = new RelayCommand(_ => Calculate());
        }


        private void InitializeUnits()
        {
            
            AmperageUnits.AddRange
            ([
                new UnitItem("A", 1.0),
                new UnitItem("mA", 0.001),
                new UnitItem("mkA", 0.000001)
            ]);

            WidthUnits.AddRange
            ([
                new UnitItem ("мил", 0.0254),
                new UnitItem ("см", 10.0),
                new UnitItem ("мм",1.0),
                new UnitItem ("мкм", 0.001),
                new UnitItem("дюйм", 25.4)
            ]);

            ThicknessUnits.AddRange
            ([
                new UnitItem("унция/фут²", 1.37),
                new UnitItem("мил", 1.0),
                new UnitItem("мм", 39.3701),    
                new UnitItem("мкм", 0.0393701), 
                new UnitItem("дюйм", 1000.0)    
            ]);

            LengthUnits.AddRange
            ([
                new UnitItem ("мил", 0.0254),
                new UnitItem ("см", 10.0),
                new UnitItem ("мм",1.0),
                new UnitItem ("мкм", 0.001),
                new UnitItem("дюйм", 25.4)
            ]);

            TempUnits.AddRange
            ([
                new UnitItem("°C", 1.0),
                new UnitItem("°K", 1.0)
            ]);

            SelectedAmperageUnit = AmperageUnits[0];
            SelectedWidthUnits = WidthUnits[2];
            SelectedThicknessUnits = ThicknessUnits[3];
            SelectedLengthUnits = LengthUnits[2];
            SelectedAmbientTempUnit = TempUnits[0]; 
            SelectedRiseTempUnit = TempUnits[0];    
        }


        private void Calculate()
        {
            try
            {
                if (string.IsNullOrEmpty(AmperageText) || AmperageText == "*")
                    throw new ArgumentException("Введите максимальный ток");
                if (string.IsNullOrEmpty(ThicknessText) || ThicknessText == "*")
                    throw new ArgumentException("Введите толщину дорожки");
                if (string.IsNullOrEmpty(RiseTempText) || RiseTempText == "*")
                    throw new ArgumentException("Введите повышение температуры");
                if (string.IsNullOrEmpty(AmbientTempText) || AmbientTempText == "*")
                    throw new ArgumentException("Введите температуру окружающей среды");
                if (string.IsNullOrEmpty(LengthText) || LengthText == "*")
                    throw new ArgumentException("Введите длину дорожки");

                double current = GetValue(AmperageText, SelectedAmperageUnit);
                double ambientTemp = GetValue(AmbientTempText, SelectedAmbientTempUnit);
                double thickness = GetValue(ThicknessText, SelectedThicknessUnits);
                double tempRise = GetValue(RiseTempText, SelectedRiseTempUnit);
                double length = GetValue(LengthText, SelectedLengthUnits);

                var calculator = new TrackCalculator();
                var result = calculator.Calculate(current, ambientTemp, thickness, tempRise, length);

                ShowResults(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка расчета",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private static double GetValue(string text, UnitItem unit)
        {
            if (string.IsNullOrEmpty(text) || text == "*")
                throw new ArgumentException("Поле не может быть пустым");

            string normalizedText = text.Replace(',', '.');

            if (!double.TryParse(normalizedText, NumberStyles.Any,
                                CultureInfo.InvariantCulture, out double value))
            {
                if (!double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
                {
                    throw new ArgumentException("Некорректное числовое значение");
                }
            }

            return value * unit.Multiplier;
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