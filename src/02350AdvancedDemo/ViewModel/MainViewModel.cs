namespace _02350AdvancedDemo.ViewModel;

public class MainViewModel : BaseViewModel
{
    private Point initialMousePosition;
    private readonly Dictionary<int, Point> initialShapePositions = new();

    private Point SelectionBoxStart;

    public double SelectionBoxX { get; set; }
    public double SelectionBoxY { get; set; }
    public double SelectionBoxWidth { get; set; }
    public double SelectionBoxHeight { get; set; }

    public ICommand MouseDownShapeCommand { get; }
    public ICommand MouseMoveShapeCommand { get; }
    public ICommand MouseUpShapeCommand { get; }

    public ICommand MouseDownCanvasCommand { get; }
    public ICommand MouseMoveCanvasCommand { get; }
    public ICommand MouseUpCanvasCommand { get; }

    public MainViewModel() : base()
    {
        Shapes = new ObservableCollection<ShapeViewModel>() {
            new CircleViewModel(new Circle() { Position = new(30, 40), Size = new(80, 80), Data = new List<string> { "text1", "text2", "text3" } }),
            new SquareViewModel(new Square() { Position = new(140, 230), Size = new(200, 100), Data = new List<string> { "text1", "text2", "text3" } })
        };

        Lines = new ObservableCollection<LineViewModel>() {
            new LineViewModel(new Line() {  Label = "Line Text" }) { From = Shapes[0], To = Shapes[1] }
        };

        MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
        MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
        MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);

        MouseDownCanvasCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownCanvas);
        MouseMoveCanvasCommand = new RelayCommand<MouseEventArgs>(MouseMoveCanvas);
        MouseUpCanvasCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpCanvas);
    }

    private void MouseDownShape(MouseButtonEventArgs e)
    {
        if (!isAddingLine)
        {
            var shape = TargetShape(e);
            var mousePosition = RelativeMousePosition(e);

            initialMousePosition = mousePosition;

            var selectedShapes = Shapes.Where(x => x.IsMoveSelected);
            if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };

            foreach (var s in selectedShapes)
                initialShapePositions.Add(s.Number, s.CanvasCenter);

            e.MouseDevice.Target.CaptureMouse();
        }
        e.Handled = true;
    }

    private void MouseMoveShape(MouseEventArgs e)
    {
        if (Mouse.Captured != null && !isAddingLine)
        {
            var mousePosition = RelativeMousePosition(e);

            var selectedShapes = Shapes.Where(x => x.IsMoveSelected);
            if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };

            foreach (var s in selectedShapes)
            {
                var originalPosition = initialShapePositions[s.Number];
                s.CanvasCenter = (originalPosition + (mousePosition - initialMousePosition));
            }

            e.Handled = true;
        }
    }

    private void MouseUpShape(MouseButtonEventArgs e)
    {
        if (isAddingLine)
        {
            var shape = TargetShape(e);

            if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
            else if (addingLineFrom.Number != shape.Number)
            {
                LineViewModel lineToAdd = new(
                    addingLineType == typeof(Line) ? new Line() :
                    new DashLine()
                )
                { From = addingLineFrom, To = shape };
                undoRedoController.AddAndExecute(new AddLineCommand(Lines, lineToAdd));
                addingLineFrom.IsSelected = false;
                isAddingLine = false;
                addingLineType = null;
                addingLineFrom = null;
                OnPropertyChanged(nameof(ModeOpacity));
            }
        }
        else
        {
            var mousePosition = RelativeMousePosition(e);

            var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
            if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };

            foreach (var s in selectedShapes)
            {
                var originalPosition = initialShapePositions[s.Number];
                s.CanvasCenter = originalPosition;
            }
            undoRedoController.AddAndExecute(new MoveShapesCommand(selectedShapes, mousePosition - initialMousePosition));

            initialMousePosition = new();
            initialShapePositions.Clear();
            e.MouseDevice.Target.ReleaseMouseCapture();
        }
        e.Handled = true;
    }

    private void MouseDownCanvas(MouseButtonEventArgs e)
    {
        if (!isAddingLine)
        {
            SelectionBoxStart = Mouse.GetPosition(e.MouseDevice.Target);
            e.MouseDevice.Target.CaptureMouse();
        }
    }

    private void MouseMoveCanvas(MouseEventArgs e)
    {
        if (Mouse.Captured != null && !isAddingLine)
        {
            var SelectionBoxNow = Mouse.GetPosition(e.MouseDevice.Target);
            SelectionBoxX = Math.Min(SelectionBoxStart.X, SelectionBoxNow.X);
            SelectionBoxY = Math.Min(SelectionBoxStart.Y, SelectionBoxNow.Y);
            SelectionBoxWidth = Math.Abs(SelectionBoxNow.X - SelectionBoxStart.X);
            SelectionBoxHeight = Math.Abs(SelectionBoxNow.Y - SelectionBoxStart.Y);
            OnPropertyChanged(nameof(SelectionBoxX));
            OnPropertyChanged(nameof(SelectionBoxY));
            OnPropertyChanged(nameof(SelectionBoxWidth));
            OnPropertyChanged(nameof(SelectionBoxHeight));
        }
    }

    private void MouseUpCanvas(MouseButtonEventArgs e)
    {
        if (!isAddingLine)
        {
            var SelectionBoxEnd = Mouse.GetPosition(e.MouseDevice.Target);
            var smallX = Math.Min(SelectionBoxStart.X, SelectionBoxEnd.X);
            var smallY = Math.Min(SelectionBoxStart.Y, SelectionBoxEnd.Y);
            var largeX = Math.Max(SelectionBoxStart.X, SelectionBoxEnd.X);
            var largeY = Math.Max(SelectionBoxStart.Y, SelectionBoxEnd.Y);
            foreach (var s in Shapes)
                s.IsMoveSelected = s.CanvasCenter.X > smallX && s.CanvasCenter.X < largeX && s.CanvasCenter.Y > smallY && s.CanvasCenter.Y < largeY;

            SelectionBoxX = SelectionBoxY = SelectionBoxWidth = SelectionBoxHeight = 0;
            OnPropertyChanged(nameof(SelectionBoxX));
            OnPropertyChanged(nameof(SelectionBoxY));
            OnPropertyChanged(nameof(SelectionBoxWidth));
            OnPropertyChanged(nameof(SelectionBoxHeight));
            e.MouseDevice.Target.ReleaseMouseCapture();
        }
    }

    private ShapeViewModel TargetShape(MouseEventArgs e)
    {
        var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
        return (ShapeViewModel)shapeVisualElement.DataContext;
    }

    private Point RelativeMousePosition(MouseEventArgs e)
    {
        var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
        var canvas = FindParentOfType<Canvas>(shapeVisualElement);
        return Mouse.GetPosition(canvas);
    }

    private static T FindParentOfType<T>(DependencyObject o)
    {
        dynamic parent = VisualTreeHelper.GetParent(o);
        return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
    }
}