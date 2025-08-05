using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace track_widths.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int amperage;
        public int width;
        public int thikness;
        public int rise_t;
        public int аmbient_t;
        public int lenght;

        public class UnitItem
        {
            public string Name { get; set; } = " ";
            public double Multiplier { get; set; } = 0.0;
        }


        


        public MainWindow()
        {
            InitializeComponent();
            InitializeComboboxes();
           
        }


        public void InitializeComboboxes()
        {
            // Единицы тока
            amperageCombobox.ItemsSource = new UnitItem[]
            {
                new() {Name = "A", Multiplier = 1.0},
                new() {Name = "mA", Multiplier = 0.001},
                new() {Name = "mkA", Multiplier = 0.000001}
            };

            // Единицы ширины (для вывода)
            widthCombobox.ItemsSource = new UnitItem[]
            {
                new() {Name = "мил", Multiplier = 0.0254},
                new() {Name = "см", Multiplier = 10.0},
                new() {Name = "мм", Multiplier = 1.0},
                new() {Name = "мкм", Multiplier = 0.001},
                new() {Name = "дюйм", Multiplier = 25.4},
            };

            // Единицы толщины
            thicknessCombobox.ItemsSource = new UnitItem[]
            {
                new() {Name = "унция/фут²", Multiplier =  1.378},
                new() {Name = "мил", Multiplier = 0.0254},
                new() {Name = "мм", Multiplier = 1.0},
                new() {Name = "мкм", Multiplier = 0.001},
                new() {Name = "дюйм", Multiplier = 25.4}
            };

            // Единицы температуры
            var tempUnits = new UnitItem[]
            {
                new() {Name = "°C", Multiplier = 1.0},
                new() {Name = "°K", Multiplier = 1.0}
            };

            riseTempCombobox.ItemsSource = tempUnits;
            ambientTempCombobox.ItemsSource = tempUnits;

            // Единицы длины
            lengthCombobox.ItemsSource = new UnitItem[]
            {
                new() {Name = "мил", Multiplier = 0.0254},
                new() {Name = "см", Multiplier = 10.0},
                new() {Name = "мм", Multiplier = 1.0},
                new() {Name = "мкм", Multiplier = 0.001},
                new() {Name = "дюйм", Multiplier = 25.4},
            };

            // Установка значений по умолчанию
            amperageCombobox.SelectedIndex = 0;
            widthCombobox.SelectedIndex = 2;    // мм
            thicknessCombobox.SelectedIndex = 3; // мм
            riseTempCombobox.SelectedIndex = 0;
            ambientTempCombobox.SelectedIndex = 0;
            lengthCombobox.SelectedIndex = 2;    // мм
        }


        private void CountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение введенных значений
                double current = GetValue(amperageTextBox, amperageCombobox);
                double thickness = GetValue(thicknessTextBox, thicknessCombobox);
                double tempRise = GetValue(riseTempTextBox, riseTempCombobox);
                double ambientTemp = GetValue(ambientTempTextBox, ambientTempCombobox);
                double length = GetValue(lengthTextBox, lengthCombobox);

                // Обработка единиц температуры
                if (((UnitItem)ambientTempCombobox.SelectedItem).Name == "°K")
                    ambientTemp -= 273.15;

                if (((UnitItem)riseTempCombobox.SelectedItem).Name == "°K")
                    tempRise -= 273.15;

                // Конвертация толщины в милы
                if (thicknessCombobox.SelectedItem is UnitItem thicknessUnit)
                {
                    if (thicknessUnit.Name == "унция/фут²")
                        thickness *= 1.37; 
                    else
                        thickness /=  0.0254; 
                }



                // Расчет параметров
                var (extWidth, extResults) = CalculateLayer(current, ambientTemp ,thickness, tempRise, length, isExternal: true);
                var (intWidth, intResults) = CalculateLayer(current, ambientTemp, thickness, tempRise, length, isExternal: false);




                // Конвертация ширины в выбранные единицы
                double widthMultiplier = ((UnitItem)widthCombobox.SelectedItem).Multiplier;
                extWidth = extWidth * 0.0254 / widthMultiplier;
                intWidth = intWidth * 0.0254 / widthMultiplier;



                string result = $"ВНЕШНИЕ СЛОИ:\n" +
                               $"Ширина дорожки: {extWidth:F3} {((UnitItem)widthCombobox.SelectedItem).Name}\n" +
                               $"Температура: {extResults.Temp:F1} °C\n" +
                               $"Сопротивление: {extResults.Resistance:F4} Ом\n" +
                               $"Падение напряжения: {extResults.VoltageDrop:F4} В\n" +
                               $"Рассеиваемая мощность: {extResults.Power:F4} Вт\n\n" +

                               $"ВНУТРЕННИЕ СЛОИ:\n" +
                               $"Ширина дорожки: {intWidth:F3} {((UnitItem)widthCombobox.SelectedItem).Name}\n" +
                               $"Температура: {intResults.Temp:F1} °C\n" +
                               $"Сопротивление: {intResults.Resistance:F4} Ом\n" +
                               $"Падение напряжения: {intResults.VoltageDrop:F4} В\n" +
                               $"Рассеиваемая мощность: {intResults.Power:F4} Вт";

                // Открываем окно с результатами
                var resultsWindow = new ResultsWindow(result)
                {
                    Owner = this 
                };
                resultsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка расчета: {ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double GetValue(TextBox textBox, ComboBox comboBox)
        {
            if (!double.TryParse(textBox.Text, out double value))
                throw new ArgumentException("Некорректное числовое значение");

            if (comboBox.SelectedItem is UnitItem unit)
                return value * unit.Multiplier;

            return value;
        }

        private (double width, (double Temp, double Resistance, double VoltageDrop, double Power))
            CalculateLayer(double current, double ambientTemp, double thickness, double tempRise, double length, bool isExternal)
        {
            // Константы для расчетов по IPC-2221
            double k = isExternal ? 0.048 : 0.024;
            double b = 0.44;
            double c = 0.725;

            // Проверка нулевых значений
            if (tempRise <= 0) throw new ArgumentException("Повышение температуры должно быть положительным");
            if (thickness <= 0) throw new ArgumentException("Толщина должна быть положительной");

            // Расчет площади поперечного сечения (в милах²)
            double area = Math.Pow(current / (k * Math.Pow(tempRise, b)), 1.0 / c);

            // Расчет ширины (в милах)
            double width = area / thickness;

            // Расчет температуры дорожки
            double trackTemp = ambientTemp + tempRise;

            // Конвертация длины в метры
            double lengthM = length / 1000.0;

            // Расчет площади в мм²
            double areaMm2 = (width * 0.0254) * (thickness * 0.0254);

            // Удельное сопротивление меди (Ом·мм²/м)
            const double resistivity = 0.0175;

            // Расчет сопротивления
            double resistance = resistivity * lengthM / areaMm2;

            // Расчет падения напряжения
            double voltageDrop = current * resistance;

            // Расчет рассеиваемой мощности
            double power = current * current * resistance;

            return (width, (trackTemp, resistance, voltageDrop, power));
        }
    }
}