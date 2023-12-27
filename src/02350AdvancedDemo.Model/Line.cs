namespace _02350AdvancedDemo.Model;

[JsonDerivedType(typeof(DashLine))]
public class Line // TODO: Try to change to records.
{
    public int FromNumber { get; set; }
    public int ToNumber { get; set; }
    public string Label { get; set; }
}
