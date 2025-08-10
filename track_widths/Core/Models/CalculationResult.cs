namespace track_widths.Core
{
    public record CalculationResult(
        double ExternalWidth,
        double ExternalTemp,
        double ExternalResistance,
        double ExternalVoltageDrop,
        double ExternalPower,
        double InternalWidth,
        double InternalTemp,
        double InternalResistance,
        double InternalVoltageDrop,
        double InternalPower
    )
    {
        public override string ToString()
        {
            return $"ВНЕШНИЕ СЛОИ:\n" +
                   $"Ширина дорожки: {ExternalWidth:F3} мм\n" +
                   $"Температура: {ExternalTemp:F1} °C\n" +
                   $"Сопротивление: {ExternalResistance:F4} Ом\n" +
                   $"Падение напряжения: {ExternalVoltageDrop:F4} В\n" +
                   $"Рассеиваемая мощность: {ExternalPower:F4} Вт\n\n" +
                   $"ВНУТРЕННИЕ СЛОИ:\n" +
                   $"Ширина дорожки: {InternalWidth:F3} мм\n" +
                   $"Температура: {InternalTemp:F1} °C\n" +
                   $"Сопротивление: {InternalResistance:F4} Ом\n" +
                   $"Падение напряжения: {InternalVoltageDrop:F4} В\n" +
                   $"Рассеиваемая мощность: {InternalPower:F4} Вт";
        }
    }
}