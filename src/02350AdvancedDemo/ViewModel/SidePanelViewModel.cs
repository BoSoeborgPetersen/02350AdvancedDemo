namespace _02350AdvancedDemo.ViewModel;

public partial class SidePanelViewModel(StateService state, UndoRedoController undoRedo) : ObservableObject
{
    public ObservableCollection<ShapeViewModel> Shapes => state.Shapes;
    public ObservableCollection<LineViewModel> Lines => state.Lines;

    [RelayCommand]
    void AddCircle() => undoRedo.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel()));

    [RelayCommand]
    void AddSquare() => undoRedo.AddAndExecute(new AddShapeCommand(Shapes, new SquareViewModel()));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveShapeCommand))]
    ShapeViewModel selectedShape;

    [RelayCommand(CanExecute = nameof(CanRemoveShape))]
    void RemoveShape() => undoRedo.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, [SelectedShape], Lines.Where(l => SelectedShape.Number == l.From.Number || SelectedShape.Number == l.To.Number).ToList()));
    bool CanRemoveShape() => SelectedShape != null;

    [RelayCommand]
    void AddLine() => state.AddingLineType = typeof(Line);

    [RelayCommand]
    void AddDashLine() => state.AddingLineType = typeof(DashLine);

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RemoveLineCommand))]
    LineViewModel selectedLine;

    [RelayCommand(CanExecute = nameof(CanRemoveLine))]
    void RemoveLine() => undoRedo.AddAndExecute(new RemoveLinesCommand(Lines, [SelectedLine]));
    bool CanRemoveLine() => SelectedLine != null;
}
