namespace _02350AdvancedDemo.ViewModel;

public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly UndoRedoController undoRedoController = UndoRedoController.Instance;
    readonly LoadSaveService loadSaveService = LoadSaveService.Instance;
    readonly CopyPasteService copyPasteService = CopyPasteService.Instance;
    public DialogViews DialogVM { get; set; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModeOpacity))]
    bool isAddingLine;
    static Type addingLineType;
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
    Task OpenDiagram() => loadSaveService.OpenDiagram(Shapes, Lines);

    [RelayCommand]
    void SaveDiagram() => loadSaveService.SaveDiagram(Shapes, Lines);

    [RelayCommand]
    void Cut() => copyPasteService.Cut(Shapes, Lines);

    [RelayCommand]
    void Copy() => copyPasteService.Copy(Shapes, Lines);

    [RelayCommand]
    async Task Paste() => copyPasteService.Paste(Shapes, Lines);

    [RelayCommand]
    void Exit() => Application.Current.Shutdown();

    [RelayCommand]
    void AddCircle() => undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel()));

    [RelayCommand]
    void AddSquare() => undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new SquareViewModel()));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveShapeCommand))]
    ShapeViewModel selectedShape;

    bool CanRemoveShape(IList _shapes) => SelectedShape != null;

    [RelayCommand(CanExecute = nameof(CanRemoveShape))]
    void RemoveShape(IList _shapes) => undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, [SelectedShape], Lines.Where(l => SelectedShape.Number == l.From.Number || SelectedShape.Number == l.To.Number).ToList()));

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
    [NotifyCanExecuteChangedFor(nameof(RemoveLineCommand))]
    LineViewModel selectedLine;

    bool CanRemoveLine(IList _lines) => SelectedLine != null;

    [RelayCommand(CanExecute = nameof(CanRemoveLine))]
    void RemoveLine(IList _lines) => undoRedoController.AddAndExecute(new RemoveLinesCommand(Lines, [SelectedLine]));
}
