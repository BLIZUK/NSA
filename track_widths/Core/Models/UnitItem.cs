namespace track_widths.Core
{
    public class UnitItem
    {
        public string Name { get; }
        public double Multiplier { get; }

        public UnitItem(string name, double multiplier)
        {
            Name = name;
            Multiplier = multiplier;
        }

        public override string ToString() => Name;
    }
}