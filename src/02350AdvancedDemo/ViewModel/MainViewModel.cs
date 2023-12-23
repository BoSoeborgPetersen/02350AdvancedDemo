namespace _02350AdvancedDemo.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    readonly MouseManipulationService mouseManipulationService = MouseManipulationService.Instance;
    Point initialMousePosition;
    readonly Dictionary<int, Point> initialShapePositions = [];

    Point SelectionBoxStart;

    [ObservableProperty]
    double selectionBoxX;
    [ObservableProperty]
    double selectionBoxY;
    [ObservableProperty]
    double selectionBoxWidth;
    [ObservableProperty]
    double selectionBoxHeight;

    public MainViewModel() : base()
    {
        Shapes = [
            new CircleViewModel() { Position = new(30, 40), Size = new(80, 80), Data = ["text1", "text2", "text3"] },
            new SquareViewModel() { Position = new(140, 230), Size = new(200, 100), Data = ["text1", "text2", "text3"] }
        ];

        Lines = [
            new LineViewModel() { From = Shapes[0], To = Shapes[1], Label = "Line Text" }
        ];
    }

    [RelayCommand]
    void MouseDownCanvas(MouseButtonEventArgs e)
    {
        if (!IsAddingLine)
        {
            SelectionBoxStart = Mouse.GetPosition(e.MouseDevice.Target);
            e.MouseDevice.Target.CaptureMouse();
        }
    }

    [RelayCommand]
    void MouseMoveCanvas(MouseEventArgs e)
    {
        if (Mouse.Captured != null && !IsAddingLine)
        {
            var SelectionBoxNow = Mouse.GetPosition(e.MouseDevice.Target);
            SelectionBoxX = Math.Min(SelectionBoxStart.X, SelectionBoxNow.X);
            SelectionBoxY = Math.Min(SelectionBoxStart.Y, SelectionBoxNow.Y);
            SelectionBoxWidth = Math.Abs(SelectionBoxNow.X - SelectionBoxStart.X);
            SelectionBoxHeight = Math.Abs(SelectionBoxNow.Y - SelectionBoxStart.Y);
        }
    }

    [RelayCommand]
    void MouseUpCanvas(MouseButtonEventArgs e)
    {
        if (!IsAddingLine)
        {
            var SelectionBoxEnd = Mouse.GetPosition(e.MouseDevice.Target);
            var smallX = Math.Min(SelectionBoxStart.X, SelectionBoxEnd.X);
            var smallY = Math.Min(SelectionBoxStart.Y, SelectionBoxEnd.Y);
            var largeX = Math.Max(SelectionBoxStart.X, SelectionBoxEnd.X);
            var largeY = Math.Max(SelectionBoxStart.Y, SelectionBoxEnd.Y);
            foreach (var s in Shapes)
                s.IsMoveSelected = s.CanvasCenter.X > smallX && s.CanvasCenter.X < largeX && s.CanvasCenter.Y > smallY && s.CanvasCenter.Y < largeY;

            SelectionBoxX = SelectionBoxY = SelectionBoxWidth = SelectionBoxHeight = 0;
            e.MouseDevice.Target.ReleaseMouseCapture();
        }
    }

    ShapeViewModel TargetShape(MouseEventArgs e)
    {
        var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
        return (ShapeViewModel)shapeVisualElement.DataContext;
    }

    Point RelativeMousePosition(MouseEventArgs e)
    {
        var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
        var canvas = FindParentOfType<Canvas>(shapeVisualElement);
        return Mouse.GetPosition(canvas);
    }

    static T FindParentOfType<T>(DependencyObject o)
    {
        dynamic parent = VisualTreeHelper.GetParent(o);
        return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
    }
}