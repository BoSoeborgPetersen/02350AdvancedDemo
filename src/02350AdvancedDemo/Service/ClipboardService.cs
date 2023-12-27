namespace _02350AdvancedDemo.Service;

public class ClipboardService(UndoRedoController undoRedo)
{
    public void Cut(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        Copy(shapes, lines);

        var selectedShapes = shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        undoRedo.AddAndExecute(new RemoveShapesCommand(shapes, lines, selectedShapes, lines.Where(l => selectedShapes.Any(s => s.Number == l.From.Number || s.Number == l.To.Number)).ToList()));
    }

    public void Copy(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        var selectedShapes = shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        Diagram diagram = MappingService.Map(selectedShapes, selectedLines);

        Clipboard.SetText(JsonSerializer.Serialize(diagram));
    }

    public void Paste(ObservableCollection<ShapeViewModel> Shapes, ObservableCollection<LineViewModel> Lines) // TODO: Reconstruct with new shape numbers. // TODO: Nudge shapes by changing positions.
    {
        // Unselect existing shapes.
        foreach (var s in Shapes)
            s.IsMoveSelected = false;

        var diagram = JsonSerializer.Deserialize<Diagram>(Clipboard.GetText());
        var (loadedShapes, loadedLines) = MappingService.Unmap(diagram);

        foreach (var shape in loadedShapes) Shapes.Add(shape);
        foreach (var line in loadedLines) Lines.Add(line);

        //var shapes = diagram.Shapes;
        //var lines = diagram.Lines;

        //// Change numbers for shapes if necessary and move them a little.
        //foreach (var s in shapes)
        //    if (Shapes.Any(x => x.Number == s.Number))
        //    {
        //        var oldNumber = s.Number;
        //        s.Number == ShapeViewModel.
        //        s.X += 50;
        //        s.Y += 50;

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
