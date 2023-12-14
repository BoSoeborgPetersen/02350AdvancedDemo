namespace _02350AdvancedDemo.ViewModel;

public abstract partial class ShapeViewModel : BaseViewModel
{
    public ICommand RemoveCommand { get; }

    public Shape Shape { get; set; }

    public int Number { get { return Shape.Number; } set { Shape.Number = value; OnPropertyChanged(); } }
    public Point Position { get { return Shape.Position; } set { Shape.Position = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanvasCenter)); } }
    public Size Size { get { return Shape.Size; } set { Shape.Size = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanvasCenter)); OnPropertyChanged(nameof(Center)); } }
    public List<string> Data { get { return Shape.Data; } set { Shape.Data = value; } }
    public Vector Center => new(Size.Width / 2, Size.Height / 2);
    public Point CanvasCenter { get { return Position + Center; } set { Position = value - Center; OnPropertyChanged(nameof(Position)); } }
    bool isSelected;
    public bool IsSelected { get { return isSelected; } set { isSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(SelectedColor)); } }
    public Brush SelectedColor => IsSelected ? Brushes.Red : Brushes.Yellow;
    bool isMoveSelected;
    public bool IsMoveSelected { get { return isMoveSelected; } set { isMoveSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(BackgroundColor)); } }
    public Brush BackgroundColor => IsMoveSelected ? Brushes.SkyBlue : Brushes.Navy;

    public ShapeViewModel(Shape _shape) : base()
    {
        Shape = _shape;

        RemoveCommand = new RelayCommand(Remove);
    }

    void Remove()
    {
        undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, [this]));
    }

    public override string ToString() => Number.ToString();
}
