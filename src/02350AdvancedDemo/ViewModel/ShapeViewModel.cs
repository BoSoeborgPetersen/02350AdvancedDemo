namespace _02350AdvancedDemo.ViewModel;

//public abstract partial class ShapeViewModel(StateService state, MouseService mouse, UndoRedoController undoRedo) : ObservableRecipient, IRecipient<IsAddingLineMessage>
public abstract partial class ShapeViewModel : ObservableRecipient, IRecipient<IsAddingLineMessage> // TODO: Dependency Injection.
{
    //readonly StateService state = StateService.Instance;
    //readonly UndoRedoController undoRedo = UndoRedoController.Instance;
    //readonly MouseService mouse = MouseService.Instance;

    StateService _state;
    StateService state => _state ??= Ioc.Default.GetService<StateService>();
    MouseService _mouse;
    MouseService mouse => _mouse ??= Ioc.Default.GetService<MouseService>();
    UndoRedoController _undoRedo;
    UndoRedoController undoRedo => _undoRedo ??= Ioc.Default.GetService<UndoRedoController>();

    protected ShapeViewModel() => IsActive = true;

    static int counter = 0;

    [ObservableProperty]
    int number = ++counter;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanvasCenter))]
    Point position = new(200 + (counter * 10), 200 + (counter * 10)); // TODO: Change to System.Drawing.Point

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Center))]
    [NotifyPropertyChangedFor(nameof(CanvasCenter))]
    Size size = new(100, 100); // TODO: Change to System.Drawing.Size

    [ObservableProperty]
    List<string> data;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedColor))]
    bool isSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BackgroundColor))]
    bool isMoveSelected;

    public Vector Center => (Vector)Size / 2;
    public Point CanvasCenter => Position + Center;
    public Brush SelectedColor => IsSelected ? Brushes.Red : Brushes.Yellow;
    public Brush BackgroundColor => IsMoveSelected ? Brushes.SkyBlue : Brushes.Navy;
    public double ModeOpacity => state?.IsAddingLine == true ? 0.4 : 1.0;

    public void Receive(IsAddingLineMessage message) => OnPropertyChanged(nameof(ModeOpacity));

    [RelayCommand]
    void Remove() => undoRedo.AddAndExecute(new RemoveShapesCommand(state.Shapes, state.Lines, [this], state.Lines.Where(l => Number == l.From.Number || Number == l.To.Number).ToList()));

    [RelayCommand]
    void MouseDown(MouseButtonEventArgs e) => mouse.ShapeMouseDown(this, e);
    [RelayCommand]
    void MouseMove(MouseEventArgs e) => mouse.ShapeMouseMove(this, e);
    [RelayCommand]
    void MouseUp(MouseButtonEventArgs e) => mouse.ShapeMouseUp(this, e);
}
