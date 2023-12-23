namespace _02350AdvancedDemo.Service;

public class LoadSaveService
{
    public static LoadSaveService Instance { get; } = new();

    LoadSaveService() { }

    public async Task OpenDiagram(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        string path = DialogViews.ShowOpen();
        if (path != null)
        {
            var diagram = JsonSerializer.Deserialize<Diagram>(File.ReadAllText(path));
            var (loadedShapes, loadedLines) = MappingService.Unmap(diagram);

            shapes.Clear();
            foreach (var shape in loadedShapes) shapes.Add(shape);
            //diagram.Shapes.Select(x => x is Circle ? (ShapeViewModel)new CircleViewModel(x) : new SquareViewModel(x)).ToList().ForEach(x => Shapes.Add(x));
            lines.Clear();
            foreach (var line in loadedLines) lines.Add(line);
            //diagram.Lines.Select(x => new LineViewModel(x)).ToList().ForEach(x => Lines.Add(x));

            //// Reconstruct object graph.
            //foreach (LineViewModel line in Lines)
            //{
            //    line.From = Shapes.Single(s => s.Number == line.From.Number);
            //    line.To = Shapes.Single(s => s.Number == line.To.Number);
            //}
        }
    }

    public void SaveDiagram(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        string path = DialogViews.ShowSave();
        if (path != null)
        {
            Diagram diagram = MappingService.Map(shapes, lines);
            //Diagram diagram = new() { Shapes = selectedShapes.Select(x => x.Shape).ToList(), Lines = selectedLines.Select(x => x.Line).ToList() };
            //Diagram diagram = new() { Shapes = Shapes.Select(s => s is SquareViewModel ? new Square() { Position = s.Position, Size = s.Size, Data = s.Data } : new Circle() { Position = s.Position, Size = s.Size, Data = s.Data }).ToList(), Lines = Lines.Select(x => x.Line).ToList() };
            File.WriteAllText(path, JsonSerializer.Serialize(diagram));
        }
    }
}
