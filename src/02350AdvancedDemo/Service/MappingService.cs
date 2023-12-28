namespace _02350AdvancedDemo.Service;

public class MappingService() // TODO: Switch to AutoMapper
{
    public static Diagram Map(IList<ShapeViewModel> shapes, IList<LineViewModel> lines) => new(
        shapes.Select<ShapeViewModel, Shape>(s => s is SquareViewModel ?
            new Square(new((int)s.Position.X, (int)s.Position.Y), new((int)s.Size.Width, (int)s.Size.Height), s.Data) :
            new Circle(new((int)s.Position.X, (int)s.Position.Y), new((int)s.Size.Width, (int)s.Size.Height), s.Data)).ToList(),
        lines.Select(l => l is DashLineViewModel ?
            new DashLine(l.From.Number, l.To.Number, "") :
            new Line(l.From.Number, l.To.Number, "")).ToList());

    public static (IList<ShapeViewModel>, IList<LineViewModel>) Unmap(Diagram diagram)
    {
        var shapes = diagram.Shapes.Select<Shape, ShapeViewModel>(s => s is Square ?
            new SquareViewModel() { Number = s.Number, Position = new(s.Position.X, s.Position.Y), Size = new(s.Size.Width, s.Size.Height), Data = s.Data } :
            new CircleViewModel() { Number = s.Number, Position = new(s.Position.X, s.Position.Y), Size = new(s.Size.Width, s.Size.Height), Data = s.Data }).ToList();
        var lines = diagram.Lines.Select(l => l is DashLine ?
            new DashLineViewModel() { From = shapes.Single(s => s.Number == l.FromNumber), To = shapes.Single(s => s.Number == l.ToNumber) } :
            new LineViewModel() { From = shapes.Single(s => s.Number == l.FromNumber), To = shapes.Single(s => s.Number == l.ToNumber) }).ToList();
        return (shapes, lines);
    }
}
