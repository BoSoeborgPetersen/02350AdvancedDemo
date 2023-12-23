namespace _02350AdvancedDemo.Service;

public class MappingService
{
    public static Diagram Map(IList<ShapeViewModel> shapes, IList<LineViewModel> lines) => new()
    {
        Shapes = shapes.Select<ShapeViewModel, Shape>(s => s is SquareViewModel ?
            new Square() { X = s.Position.X, Y = s.Position.Y, Width = s.Size.Width, Height = s.Size.Height, Data = s.Data } :
            new Circle() { X = s.Position.X, Y = s.Position.Y, Width = s.Size.Width, Height = s.Size.Height, Data = s.Data }).ToList(),
        Lines = lines.Select(l => l is DashLineViewModel ?
            new DashLine() { FromNumber = l.From.Number, ToNumber = l.To.Number } :
            new Line() { FromNumber = l.From.Number, ToNumber = l.To.Number }).ToList()
    };

    public static (IList<ShapeViewModel>, IList<LineViewModel>) Unmap(Diagram diagram)
    {
        var shapes = diagram.Shapes.Select<Shape, ShapeViewModel>(s => s is Square ?
            new SquareViewModel() { Number = s.Number, Position = new(s.X, s.Y), Size = new(s.Width, s.Height), Data = s.Data } :
            new CircleViewModel() { Number = s.Number, Position = new(s.X, s.Y), Size = new(s.Width, s.Height), Data = s.Data }).ToList();
        var lines = diagram.Lines.Select(l => l is DashLine ?
            new DashLineViewModel() { From = shapes.Single(s => s.Number == l.FromNumber) } :
            new LineViewModel() { From = shapes.Single(s => s.Number == l.FromNumber) }).ToList();
        return (shapes, lines);
    }
}
