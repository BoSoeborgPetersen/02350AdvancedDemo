namespace _02350AdvancedDemo.Model;

[JsonDerivedType(typeof(Circle), nameof(Circle))]
[JsonDerivedType(typeof(Square), nameof(Square))]
public abstract class Shape
{
    static int counter = 0;

    public int Number { get; } = ++counter;

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public List<string> Data { get; set; }
}
