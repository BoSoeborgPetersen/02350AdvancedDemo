namespace _02350AdvancedDemo.Service;

public class CopyPasteService
{
    readonly UndoRedoController undoRedoController = UndoRedoController.Instance;

    public static CopyPasteService Instance { get; } = new();

    CopyPasteService() { }

    public void Cut(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        Copy(shapes, lines);

        var selectedShapes = shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        undoRedoController.AddAndExecute(new RemoveShapesCommand(shapes, lines, selectedShapes, lines.Where(l => selectedShapes.Any(s => s.Number == l.From.Number || s.Number == l.To.Number)).ToList()));
    }

    public void Copy(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        var selectedShapes = shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        Diagram diagram = MappingService.Map(selectedShapes, selectedLines);

        Clipboard.SetText(JsonSerializer.Serialize(diagram));
    }

    public async Task Paste(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        //var diagram = Deserialize(Clipboard.GetText());

        //var shapes = diagram.Shapes;
        //var lines = diagram.Lines;

        //// Unselect existing shapes.
        //foreach (var s in Shapes)
        //    s.IsMoveSelected = false;

        //// Change numbers for shapes if necessary and move them a little.
        //foreach (var s in shapes)
        //    if (Shapes.Any(x => x.Number == s.Number))
        //    {
        //        var oldNumber = s.Number;
        //        s.Position += new Vector(50, 50);

        //        // change referenced number for lines.
        //        foreach (var l in lines.Where(x => x.FromNumber == oldNumber))
        //            l.FromNumber = s.Number;
        //        foreach (var l in lines.Where(x => x.ToNumber == oldNumber))
        //            l.ToNumber = s.Number;
        //    }

        //// Add shapes and lines (TODO: Should use undo/redo command).
        ////undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel(new Circle())));
        ////undoRedoController.AddAndExecute(new AddLineCommand(Lines, lineToAdd));
        //shapes.ForEach(s => Shapes.Add(s is Circle ? new CircleViewModel(s) { IsMoveSelected = true } : new SquareViewModel(s) { IsMoveSelected = true }));
        //lines.ForEach(l => Lines.Add(new LineViewModel(l)));

        //// Reconstruct object graph.
        //foreach (LineViewModel line in Lines)
        //{
        //    line.From = Shapes.Single(s => s.Number == line.Line.FromNumber);
        //    line.To = Shapes.Single(s => s.Number == line.Line.ToNumber);
        //}
    }
}
