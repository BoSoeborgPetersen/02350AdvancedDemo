namespace _02350AdvancedDemo.Model;

[XmlInclude(typeof(DashLine))]
public class Line
{
    public int FromNumber { get; set; }
    public int ToNumber { get; set; }
    public string Label { get; set; }
}
