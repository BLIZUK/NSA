namespace track_widths.Core
{
    public class TrackCalculator
    {
        public CalculationResult Calculate(double current, double ambientTemp, double thickness,
                                          double tempRise, double length)
        {
            // Перенесенная логика из CalculateLayer
            var (extWidth, extResults) = CalculateLayer(current, ambientTemp, thickness, tempRise, length, true);
            var (intWidth, intResults) = CalculateLayer(current, ambientTemp, thickness, tempRise, length, false);

            return new CalculationResult(
                extWidth, extResults.Temp, extResults.Resistance, extResults.VoltageDrop, extResults.Power,
                intWidth, intResults.Temp, intResults.Resistance, intResults.VoltageDrop, intResults.Power
            );
        }

        private (double width, (double Temp, double Resistance, double VoltageDrop, double Power))
            CalculateLayer(double current, double ambientTemp, double thickness, double tempRise,
                          double length, bool isExternal)
        {
            double k = isExternal ? 0.048 : 0.024;
            double b = 0.44;
            double c = 0.725;

            // Проверка входных значений
            if (current <= 0) throw new ArgumentException("Ток должен быть положительным");
            if (thickness <= 0) throw new ArgumentException("Толщина должна быть положительной");
            if (tempRise <= 0 || tempRise > 100) throw new ArgumentException("Некорректное повышение температуры (0-100°C)");
            if (length <= 0) throw new ArgumentException("Длина должна быть положительной");

            // Расчет площади поперечного сечения (в милах²) по формуле IPC-2221
            double S = Math.Pow(current / (k * Math.Pow(tempRise, b)), 1.0 / c);

            // Расчет ширины (в милах)
            double W = S / thickness;

            // Расчет температуры дорожки
            double trackTemp = ambientTemp + tempRise;

            // Конвертация длины в метры
            double lengthM = length / 1000.0;

            // Расчет площади в мм² (1 мил = 0.0254 мм)
            double areaMm2 = (W * 0.0254) * (thickness * 0.0254);

            // Удельное сопротивление меди (Ом·мм²/м)
            const double resistivity = 0.01678; // Более точное значение

            // Расчет сопротивления
            double resistance = resistivity * lengthM / areaMm2;

            // Расчет падения напряжения
            double voltageDrop = current * resistance;

            // Расчет рассеиваемой мощности
            double power = current * voltageDrop;

            return (W, (trackTemp, resistance, voltageDrop, power));
        }
    }
}