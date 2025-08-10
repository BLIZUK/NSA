namespace track_widths.Core.Models
{
    public record CalculationResult(
        double ExternalWidth,    // В милах (оригинальная единица расчета)
        double ExternalTemp,
        double ExternalResistance,
        double ExternalVoltageDrop,
        double ExternalPower,
        double InternalWidth,    // В милах (оригинальная единица расчета)
        double InternalTemp,
        double InternalResistance,
        double InternalVoltageDrop,
        double InternalPower
    )
    {
        public string ToString(string widthUnit)
        {
            return $"ВНЕШНИЕ СЛОИ:\n" +
                   $"Ширина дорожки: {ExternalWidth:F3} {widthUnit}\n" +
                   $"Температура: {ExternalTemp:F1} °C\n" +
                   $"Сопротивление: {ExternalResistance:F4} Ом\n" +
                   $"Падение напряжения: {ExternalVoltageDrop:F4} В\n" +
                   $"Рассеиваемая мощность: {ExternalPower:F4} Вт\n\n" +
                   $"ВНУТРЕННИЕ СЛОИ:\n" +
                   $"Ширина дорожки: {InternalWidth:F3} {widthUnit}\n" +
                   $"Температура: {InternalTemp:F1} °C\n" +
                   $"Сопротивление: {InternalResistance:F4} Ом\n" +
                   $"Падение напряжения: {InternalVoltageDrop:F4} В\n" +
                   $"Рассеиваемая мощность: {InternalPower:F4} Вт";
        }
    }
}