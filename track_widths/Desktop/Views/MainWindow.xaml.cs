using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace track_widths.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Поля
        public double Amperage;
        public double TraceWidth;
        public double TraceThikness;
        public double TraceLenght;
        public double RiseTemperature;
        public double AmbientTemperature;

        public class UnitItem_OLD
        {
            public string Name { get; set; } = " ";
            public double Multiplier { get; set; } = 0.0;
        }
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            InitializeComboboxes();

        }


        public void InitializeComboboxes()
        {

            // Единицы тока
            amperageCombobox.ItemsSource = new UnitItem_OLD[]
            {
                new() {Name = "A", Multiplier = 1.0},
                new() {Name = "mA", Multiplier = 0.001},
                new() {Name = "mkA", Multiplier = 0.000001}
            };


            // Единицы ширины (для вывода)
            widthCombobox.ItemsSource = new UnitItem_OLD[]
            {
                new() {Name = "мил", Multiplier = 0.0254},
                new() {Name = "см", Multiplier = 10.0},
                new() {Name = "мм", Multiplier = 1.0},
                new() {Name = "мкм", Multiplier = 0.001},
                new() {Name = "дюйм", Multiplier = 25.4},
            };


            // Единицы толщины
            thicknessCombobox.ItemsSource = new UnitItem_OLD[]
            {
                new() {Name = "унция/фут²", Multiplier = 1.37},
                new() {Name = "мил", Multiplier = 1.0},
                new() {Name = "мм", Multiplier = 39.3701},    // 1 мм = 39.3701 мил
                new() {Name = "мкм", Multiplier = 0.0393701}, // 1 мкм = 0.0393701 мил
                new() {Name = "дюйм", Multiplier = 1000.0}    // 1 дюйм = 1000 мил
            };


            // Единицы температуры
            var tempUnits = new UnitItem_OLD[]
            {
                new() {Name = "°C", Multiplier = 1.0},
                new() {Name = "°K", Multiplier = 1.0}
            };


            riseTempCombobox.ItemsSource = tempUnits;
            ambientTempCombobox.ItemsSource = tempUnits;


            // Единицы длины
            lengthCombobox.ItemsSource = new UnitItem_OLD[]
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


        #region Валидация данных

        //Получение значений
        private double GetValue(TextBox textBox, ComboBox comboBox)
        {
            // Замена точки на запятую для поддержки локализации
            string input = textBox.Text.Replace('.', ',');

            if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                throw new ArgumentException($"Введите значение: {textBox.Name.Replace("TextBox", "")}");

            // Общая проверка на положительность (кроме температуры окружения)
            if (textBox != ambientTempTextBox && value <= 0)
                throw new ArgumentException("Значение должно быть положительным");

            // Специфические проверки
            if (textBox == ambientTempTextBox && (value < -100 || value > 100))
                throw new ArgumentException("Температура окружения должна быть от -100 до 100°C");

            if (textBox == riseTempTextBox && (value <= 0 || value > 100))
                throw new ArgumentException("Повышение температуры должно быть 1-100°C");

            if (comboBox.SelectedItem is UnitItem_OLD unit)
                return value * unit.Multiplier;

            return value;
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            Regex regex;

            // Для температуры окружения разрешаем знак минус
            if (textBox == ambientTempTextBox)
                regex = new Regex(@"^-?\d*[.,]?\d*$");
            else
                regex = new Regex(@"^\d*[.,]?\d*$");

            e.Handled = !regex.IsMatch(newText);
        }


        //Подстановка * в пустое при клике в другое поле
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "*";
            }
            else if (textBox.Text.Contains('.'))
            {
                textBox.Text = textBox.Text.Replace('.', ',');
            }
        }


        //Удаление * при клике на пустое поле
        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox.Text == "*")
            {
                textBox.Text = "";
            }
        }
        #endregion


        #region Расчет данных
        private void CountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполненности всех полей
                if (string.IsNullOrWhiteSpace(amperageTextBox.Text)) throw new ArgumentException("Введите значение тока");
                if (string.IsNullOrWhiteSpace(thicknessTextBox.Text)) throw new ArgumentException("Введите толщину дорожки");
                if (string.IsNullOrWhiteSpace(riseTempTextBox.Text)) throw new ArgumentException("Введите повышение температуры");
                if (string.IsNullOrWhiteSpace(ambientTempTextBox.Text)) throw new ArgumentException("Введите температуру окружения");
                if (string.IsNullOrWhiteSpace(lengthTextBox.Text)) throw new ArgumentException("Введите длину дорожки");

                // Получение введенных значений
                double GeyAmperage = GetValue(amperageTextBox, amperageCombobox);
                double GetThickness = GetValue(thicknessTextBox, thicknessCombobox);
                double GetRiseTemperature = GetValue(riseTempTextBox, riseTempCombobox);
                double GetAmbientTemperature = GetValue(ambientTempTextBox, ambientTempCombobox);
                double GetLength = GetValue(lengthTextBox, lengthCombobox);

                // Обработка единиц температуры
                if (((UnitItem_OLD)ambientTempCombobox.SelectedItem).Name == "°K")
                    GetAmbientTemperature -= 273.15;

                if (((UnitItem_OLD)riseTempCombobox.SelectedItem).Name == "°K")
                    GetRiseTemperature -= 273.15;

                // Расчет параметров
                var (extWidth, extResults) = CalculateLayer(GeyAmperage, GetAmbientTemperature, GetThickness, GetRiseTemperature, GetLength, isExternal: true);
                var (intWidth, intResults) = CalculateLayer(GeyAmperage, GetAmbientTemperature, GetThickness, GetRiseTemperature, GetLength, isExternal: false);

                // Конвертация ширины в выбранные единицы
                double widthMultiplier = ((UnitItem_OLD)widthCombobox.SelectedItem).Multiplier;
                extWidth = extWidth * 0.0254 / widthMultiplier;
                intWidth = intWidth * 0.0254 / widthMultiplier;

                string result = $"ВНЕШНИЕ СЛОИ:\n" +
                               $"Ширина дорожки: {extWidth:F3} {((UnitItem_OLD)widthCombobox.SelectedItem).Name}\n" +
                               $"Температура: {extResults.Temp:F1} °C\n" +
                               $"Сопротивление: {extResults.Resistance:F4} Ом\n" +
                               $"Падение напряжения: {extResults.VoltageDrop:F4} В\n" +
                               $"Рассеиваемая мощность: {extResults.Power:F4} Вт\n\n" +

                               $"ВНУТРЕННИЕ СЛОИ:\n" +
                               $"Ширина дорожки: {intWidth:F3} {((UnitItem_OLD)widthCombobox.SelectedItem).Name}\n" +
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


        private (double width, (double Temp, double Resistance, double VoltageDrop, double Power))
    CalculateLayer(double Amperage, double AmbientTemperature, double TraceThikness, double RiseTemperature, double TraceLength, bool isExternal)
        {
            double k = isExternal ? 0.048 : 0.024;
            double b = 0.44;
            double c = 0.725;

            // Проверка входных значений
            if (Amperage <= 0) throw new ArgumentException("Ток должен быть положительным");
            if (TraceThikness <= 0) throw new ArgumentException("Толщина должна быть положительной");
            if (RiseTemperature <= 0 || RiseTemperature > 100) throw new ArgumentException("Некорректное повышение температуры (0-100°C)");
            if (TraceLength <= 0) throw new ArgumentException("Длина должна быть положительной");

            // Расчет площади поперечного сечения (в милах²) по формуле IPC-2221
            double S = Math.Pow(Amperage / (k * Math.Pow(RiseTemperature, b)), 1.0 / c);

            // Расчет ширины (в милах)
            double W = S / TraceThikness;

            // Расчет температуры дорожки
            double trackTemp = AmbientTemperature + RiseTemperature;

            // Конвертация длины в метры
            double lengthM = TraceLength / 1000.0;

            // Расчет площади в мм² (1 мил = 0.0254 мм)
            double areaMm2 = (W * 0.0254) * (TraceThikness * 0.0254);

            // Удельное сопротивление меди (Ом·мм²/м)
            const double resistivity = 0.01678; // Более точное значение

            // Расчет сопротивления
            double resistance = resistivity * lengthM / areaMm2;

            // Расчет падения напряжения
            double voltageDrop = Amperage * resistance;

            // Расчет рассеиваемой мощности
            double power = Amperage * voltageDrop;

            return (W, (trackTemp, resistance, voltageDrop, power));
        }
        #endregion
    }
}