namespace track_widths.Core.Models
{
    public class UnitItem(string name, double multiplier)
    {
        public string Name { get; } = name;

        public double Multiplier { get; } = multiplier;

        public override string ToString() => Name;
    }
}