namespace _02350AdvancedDemo.Model;

[JsonDerivedType(typeof(Circle), nameof(Circle))]
[JsonDerivedType(typeof(Square), nameof(Square))]
public abstract class Shape
{
    static int counter = 0;
    public int Number { get; set; } = ++counter;

    public Point Position { get; set; } = new(200, 200);
    public Size Size { get; set; } = new(100, 100);

    public List<string> Data { get; set; }

    public void NewNumber() => Number = ++counter;

    public override string ToString() => Number.ToString();
}
