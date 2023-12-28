namespace _02350AdvancedDemo.Model;

public record Circle(Point Position, Size Size, List<string> Data) : Shape(Position, Size, Data);
