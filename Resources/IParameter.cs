namespace WiM.Resources
{
    public interface IParameter
    {
        string Name { get; set; }
        string Description { get; set; }
        string Code { get; set; }
        IUnit Unit { get; set; }
        double? Value { get; set; }
    }
}
