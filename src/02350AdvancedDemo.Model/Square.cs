namespace _02350AdvancedDemo.Model;

public record Square(Point Position, Size Size, List<string> Data) : Shape(Position, Size, Data);
