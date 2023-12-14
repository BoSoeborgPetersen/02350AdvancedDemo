using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace _02350AdvancedDemo.ViewModel;

public abstract partial class BaseViewModel : ObservableObject
{
    protected UndoRedoController undoRedoController = UndoRedoController.Instance;
    public DialogViews DialogVM { get; set; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModeOpacity))]
    bool isAddingLine;
    protected static Type addingLineType;
    protected static ShapeViewModel addingLineFrom;
    public double ModeOpacity => IsAddingLine ? 0.4 : 1.0;

    public static ObservableCollection<ShapeViewModel> Shapes { get; set; }
    public static ObservableCollection<LineViewModel> Lines { get; set; }

    bool CanUndo(string s) => undoRedoController.CanUndo(s == null ? 1 : int.Parse(s));

    [RelayCommand(CanExecute = nameof(CanUndo))]
    void Undo(string s) => undoRedoController.Undo(s == null ? 1 : int.Parse(s));

    bool CanRedo(string s) => undoRedoController.CanRedo(s == null ? 1 : int.Parse(s));

    [RelayCommand(CanExecute = nameof(CanRedo))]
    void Redo(string s) => undoRedoController.Redo(s == null ? 1 : int.Parse(s));

    [RelayCommand]
    void NewDiagram()
    {
        if (DialogViews.ShowNew())
        {
            Shapes.Clear();
            Lines.Clear();
        }
    }

    [RelayCommand]
    async Task OpenDiagram()
    {
        string path = DialogViews.ShowOpen();
        if (path != null)
        {
            var json = File.ReadAllText(path);
            var diagram = JsonSerializer.Deserialize<Diagram>(json);

            Shapes.Clear();
            diagram.Shapes.Select(x => x is Circle ? (ShapeViewModel)new CircleViewModel(x) : new SquareViewModel(x)).ToList().ForEach(x => Shapes.Add(x));
            Lines.Clear();
            diagram.Lines.Select(x => new LineViewModel(x)).ToList().ForEach(x => Lines.Add(x));

            // Reconstruct object graph.
            foreach (LineViewModel line in Lines)
            {
                line.From = Shapes.Single(s => s.Number == line.Line.FromNumber);
                line.To = Shapes.Single(s => s.Number == line.Line.ToNumber);
            }
        }
    }

    [RelayCommand]
    void SaveDiagram()
    {
        string path = DialogViews.ShowSave();
        if (path != null)
        {
            Diagram diagram = new() { Shapes = Shapes.Select(x => x.Shape).ToList(), Lines = Lines.Select(x => x.Line).ToList() };
            var json = JsonSerializer.Serialize(diagram);
            File.WriteAllText(path, json);
        }
    }

    [RelayCommand]
    async Task Cut()
    {
        var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = Lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, selectedShapes, Lines.Where(l => selectedShapes.Any(s => s.Number == l.From.Number || s.Number == l.To.Number)).ToList()));

        Diagram diagram = new() { Shapes = selectedShapes.Select(x => x.Shape).ToList(), Lines = selectedLines.Select(x => x.Line).ToList() };

        var json = JsonSerializer.Serialize(diagram);

        Clipboard.SetText(json);
    }

    [RelayCommand]
    async Task Copy()
    {
        var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = Lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        Diagram diagram = new() { Shapes = selectedShapes.Select(x => x.Shape).ToList(), Lines = selectedLines.Select(x => x.Line).ToList() };

        var json = JsonSerializer.Serialize(diagram);

        Clipboard.SetText(json);
    }

    [RelayCommand]
    async Task Paste()
    {
        var json = Clipboard.GetText();

        var diagram = JsonSerializer.Deserialize<Diagram>(json);

        var shapes = diagram.Shapes;
        var lines = diagram.Lines;

        // Unselect existing shapes.
        foreach (var s in Shapes)
            s.IsMoveSelected = false;

        // Change numbers for shapes if necessary and move them a little.
        foreach (var s in shapes)
            if (Shapes.Any(x => x.Number == s.Number))
            {
                var oldNumber = s.Number;
                s.NewNumber();
                s.Position += new Vector(50, 50);

                // change referenced number for lines.
                foreach (var l in lines.Where(x => x.FromNumber == oldNumber))
                    l.FromNumber = s.Number;
                foreach (var l in lines.Where(x => x.ToNumber == oldNumber))
                    l.ToNumber = s.Number;
            }

        // Add shapes and lines (TODO: Should use undo/redo command).
        //undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel(new Circle())));
        //undoRedoController.AddAndExecute(new AddLineCommand(Lines, lineToAdd));
        shapes.ForEach(s => Shapes.Add(s is Circle ? new CircleViewModel(s) { IsMoveSelected = true } : new SquareViewModel(s) { IsMoveSelected = true }));
        lines.ForEach(l => Lines.Add(new LineViewModel(l)));

        // Reconstruct object graph.
        foreach (LineViewModel line in Lines)
        {
            line.From = Shapes.Single(s => s.Number == line.Line.FromNumber);
            line.To = Shapes.Single(s => s.Number == line.Line.ToNumber);
        }
    }

    [RelayCommand]
    void Exit() => Application.Current.Shutdown();

    [RelayCommand]
    void AddCircle() => undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel(new Circle())));

    [RelayCommand]
    void AddSquare() => undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new SquareViewModel(new Square())));

    [RelayCommand]
    void AddLine()
    {
        IsAddingLine = true;
        addingLineType = typeof(Line);
    }

    [RelayCommand]
    void AddDashLine()
    {
        IsAddingLine = true;
        addingLineType = typeof(DashLine);
    }

    bool CanRemoveShapes(IList _shapes) => _shapes.Count == 1;

    [RelayCommand(CanExecute = nameof(CanRemoveShapes))]
    void RemoveShapes(IList _shapes) => undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, _shapes.Cast<ShapeViewModel>().ToList(), Lines.Where(l => _shapes.Cast<ShapeViewModel>().Any(s => s.Number == l.From.Number || s.Number == l.To.Number)).ToList()));

    bool CanRemoveLines(IList _edges) => _edges.Count >= 1;

    [RelayCommand(CanExecute = nameof(CanRemoveLines))]
    void RemoveLines(IList _lines) => undoRedoController.AddAndExecute(new RemoveLinesCommand(Lines, _lines.Cast<LineViewModel>().ToList()));
}
