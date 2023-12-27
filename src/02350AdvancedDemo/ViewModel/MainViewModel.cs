
namespace _02350AdvancedDemo.ViewModel;

public partial class MainViewModel(ClipboardService clipboard, StateService state, SelectionBoxService selectionBoxService, UndoRedoController undoRedo, SidePanelViewModel sidePanelVM) : ObservableRecipient, IRecipient<UndoRedoChangedMessage>
{
    public SidePanelViewModel SidePanelVM { get; set; } = sidePanelVM;

    public ObservableCollection<ShapeViewModel> Shapes => state.Shapes;
    public ObservableCollection<LineViewModel> Lines => state.Lines;

    [RelayCommand(CanExecute = nameof(CanUndo))]
    void Undo(string s) => undoRedo.Undo(s.ParseOr(1));
    bool CanUndo(string s) => undoRedo.CanUndo(s.ParseOr(1));

    [RelayCommand(CanExecute = nameof(CanRedo))]
    void Redo(string s) => undoRedo.Redo(s.ParseOr(1));
    bool CanRedo(string s) => undoRedo.CanRedo(s.ParseOr(1));

    public void Receive(UndoRedoChangedMessage message) { UndoCommand.NotifyCanExecuteChanged(); RedoCommand.NotifyCanExecuteChanged(); }

    [RelayCommand]
    void New()
    {
        if (Dialogs.ShowNew()) { Shapes.Clear(); Lines.Clear(); }
    }

    [RelayCommand] void Load() => FileService.Load(Shapes, Lines);
    [RelayCommand] void Save() => FileService.Save(Shapes, Lines);
    [RelayCommand] void Cut() => clipboard.Cut(Shapes, Lines);
    [RelayCommand] void Copy() => clipboard.Copy(Shapes, Lines);
    [RelayCommand] void Paste() => clipboard.Paste(Shapes, Lines);
    [RelayCommand] void Exit() => Application.Current.Shutdown();

    [ObservableProperty]
    Rect selectionBox;

    [RelayCommand] void MouseDown(MouseButtonEventArgs e) => selectionBoxService.CanvasMouseDown(e);
    [RelayCommand] void MouseMove(MouseEventArgs e) => SelectionBox = selectionBoxService.CanvasMouseMove(e);
    [RelayCommand] void MouseUp(MouseButtonEventArgs e) => SelectionBox = selectionBoxService.CanvasMouseUp(Shapes, e);

    internal void Init()
    {
        IsActive = true;
        state.Init();
    }
}