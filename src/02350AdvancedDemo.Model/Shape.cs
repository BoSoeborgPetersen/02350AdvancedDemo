namespace _02350AdvancedDemo.Model;

[JsonDerivedType(typeof(Circle), nameof(Circle))]
[JsonDerivedType(typeof(Square), nameof(Square))]
public abstract record Shape(Point Position, Size Size, List<string> Data)
{
    static int counter;

    public int Number { get; } = ++counter;
}
