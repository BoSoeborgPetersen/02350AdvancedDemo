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

    Diagram Map(IList<ShapeViewModel> shapes, IList<LineViewModel> lines) => new()
        {
            Shapes = shapes.Select<ShapeViewModel, Shape>(s => s is SquareViewModel ?
                new Square() { X = s.Position.X, Y = s.Position.Y, Width = s.Size.Width, Height = s.Size.Height, Data = s.Data } :
                new Circle() { X = s.Position.X, Y = s.Position.Y, Width = s.Size.Width, Height = s.Size.Height, Data = s.Data }).ToList(),
            Lines = lines.Select(l => l is DashLineViewModel ? 
                new DashLine() { FromNumber = l.From.Number, ToNumber = l.To.Number } : 
                new Line() { FromNumber = l.From.Number, ToNumber = l.To.Number }).ToList()
        };
    (IList<ShapeViewModel>, IList<LineViewModel>) Unmap(Diagram diagram)
    {
        var shapes = diagram.Shapes.Select<Shape, ShapeViewModel>(s => s is Square ?
            new SquareViewModel() { Number = s.Number, Position = new(s.X, s.Y), Size = new(s.Width, s.Height), Data = s.Data } :
            new CircleViewModel() { Number = s.Number, Position = new(s.X, s.Y), Size = new(s.Width, s.Height), Data = s.Data }).ToList();
        var lines = diagram.Lines.Select(l => l is DashLine ?
            new DashLineViewModel() { From = shapes.Single(s => s.Number == l.FromNumber) } : 
            new LineViewModel() { From = shapes.Single(s => s.Number == l.FromNumber) }).ToList();
        return (shapes, lines);
    }

    string Serialize(Diagram diagram) => JsonSerializer.Serialize(diagram);
    Diagram Deserialize(string json) => JsonSerializer.Deserialize<Diagram>(json);

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
            var diagram = Deserialize(File.ReadAllText(path));
            var (shapes, lines) = Unmap(diagram);

            Shapes.Clear();
            foreach (var shape in shapes) Shapes.Add(shape);
            //diagram.Shapes.Select(x => x is Circle ? (ShapeViewModel)new CircleViewModel(x) : new SquareViewModel(x)).ToList().ForEach(x => Shapes.Add(x));
            Lines.Clear();
            foreach (var line in lines) Lines.Add(line);
            //diagram.Lines.Select(x => new LineViewModel(x)).ToList().ForEach(x => Lines.Add(x));

            //// Reconstruct object graph.
            //foreach (LineViewModel line in Lines)
            //{
            //    line.From = Shapes.Single(s => s.Number == line.From.Number);
            //    line.To = Shapes.Single(s => s.Number == line.To.Number);
            //}
        }
    }

    [RelayCommand]
    void SaveDiagram()
    {
        string path = DialogViews.ShowSave();
        if (path != null)
        {
            Diagram diagram = Map(Shapes, Lines);
            //Diagram diagram = new() { Shapes = selectedShapes.Select(x => x.Shape).ToList(), Lines = selectedLines.Select(x => x.Line).ToList() };
            //Diagram diagram = new() { Shapes = Shapes.Select(s => s is SquareViewModel ? new Square() { Position = s.Position, Size = s.Size, Data = s.Data } : new Circle() { Position = s.Position, Size = s.Size, Data = s.Data }).ToList(), Lines = Lines.Select(x => x.Line).ToList() };
            File.WriteAllText(path, Serialize(diagram));
        }
    }

    [RelayCommand]
    void Cut()
    {
        Copy();

        var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = Lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, selectedShapes, Lines.Where(l => selectedShapes.Any(s => s.Number == l.From.Number || s.Number == l.To.Number)).ToList()));
    }

    [RelayCommand]
    void Copy()
    {
        var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
        var selectedLines = Lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

        Diagram diagram = Map(selectedShapes, selectedLines);

        Clipboard.SetText(Serialize(diagram));
    }

    [RelayCommand]
    async Task Paste()
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

    [RelayCommand]
    void Exit() => Application.Current.Shutdown();

    [RelayCommand]
    void AddCircle() => undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel()));

    [RelayCommand]
    void AddSquare() => undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new SquareViewModel()));

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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveShapesCommand))]
    ShapeViewModel selectedShape;

    bool CanRemoveShapes(IList _shapes) => SelectedShape != null;

    [RelayCommand(CanExecute = nameof(CanRemoveShapes))]
    void RemoveShapes(IList _shapes) => undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, _shapes.Cast<ShapeViewModel>().ToList(), Lines.Where(l => _shapes.Cast<ShapeViewModel>().Any(s => s.Number == l.From.Number || s.Number == l.To.Number)).ToList()));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveLinesCommand))]
    LineViewModel selectedLine;

    bool CanRemoveLines(IList _lines) => SelectedLine != null;

    [RelayCommand(CanExecute = nameof(CanRemoveLines))]
    void RemoveLines(IList _lines) => undoRedoController.AddAndExecute(new RemoveLinesCommand(Lines, _lines.Cast<LineViewModel>().ToList()));
}
