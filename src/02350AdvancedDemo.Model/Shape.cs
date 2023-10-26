namespace _02350AdvancedDemo.Model;

[XmlInclude(typeof(Circle))]
[XmlInclude(typeof(Square))]
public abstract class Shape
{
    private static int counter = 0;
    public int Number { get; set; } = ++counter;

    public Point Position { get; set; } = new(200, 200);
    public Size Size { get; set; } = new(100, 100);

    public List<string> Data { get; set; }

    public void NewNumber()
    {
        Number = ++counter;
    }

    public override string ToString() => Number.ToString();
}
