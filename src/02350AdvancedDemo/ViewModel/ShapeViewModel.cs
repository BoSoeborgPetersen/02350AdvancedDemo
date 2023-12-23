namespace _02350AdvancedDemo.ViewModel;

public abstract partial class ShapeViewModel : BaseViewModel
{
    readonly MouseManipulationService mouseManipulationService = MouseManipulationService.Instance;

    [ObservableProperty]
    int number;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanvasCenter))]
    Point position;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Center))]
    [NotifyPropertyChangedFor(nameof(CanvasCenter))]
    Size size;

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

    [RelayCommand]
    void Remove() => undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, [this], Lines.Where(l => Number == l.From.Number || Number == l.To.Number).ToList()));
    [RelayCommand]
    public void MouseDown(MouseButtonEventArgs e) => mouseManipulationService.MouseDown(this, e);
    [RelayCommand]
    public void MouseMove(MouseEventArgs e) => mouseManipulationService.MouseMove(this, e);
    [RelayCommand]
    public void MouseUp(MouseButtonEventArgs e) => mouseManipulationService.MouseUp(this, e);
}
