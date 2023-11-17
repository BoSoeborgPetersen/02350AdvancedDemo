namespace _02350AdvancedDemo.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    private Point initialMousePosition;
    private readonly Dictionary<int, Point> initialShapePositions = [];

    private Point SelectionBoxStart;

    [ObservableProperty]
    private double selectionBoxX;
    [ObservableProperty]
    private double selectionBoxY;
    [ObservableProperty]
    private double selectionBoxWidth;
    [ObservableProperty]
    private double selectionBoxHeight;

    public MainViewModel() : base()
    {
        Shapes = [
            new CircleViewModel(new Circle() { Position = new(30, 40), Size = new(80, 80), Data = ["text1", "text2", "text3"] }),
            new SquareViewModel(new Square() { Position = new(140, 230), Size = new(200, 100), Data = ["text1", "text2", "text3"] })
        ];

        Lines = [
            new LineViewModel(new Line() {  Label = "Line Text" }) { From = Shapes[0], To = Shapes[1] }
        ];
    }

    [RelayCommand]
    private void MouseDownShape(MouseButtonEventArgs e)
    {
        if (!IsAddingLine)
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

    [RelayCommand]
    private void MouseMoveShape(MouseEventArgs e)
    {
        if (Mouse.Captured != null && !IsAddingLine)
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

    [RelayCommand]
    private void MouseUpShape(MouseButtonEventArgs e)
    {
        if (IsAddingLine)
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
                IsAddingLine = false;
                addingLineType = null;
                addingLineFrom = null;
            }
        }
        else
        {
            var mousePosition = RelativeMousePosition(e);

            var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
            if (!selectedShapes.Any()) selectedShapes = [TargetShape(e)];

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

    [RelayCommand]
    private void MouseDownCanvas(MouseButtonEventArgs e)
    {
        if (!IsAddingLine)
        {
            SelectionBoxStart = Mouse.GetPosition(e.MouseDevice.Target);
            e.MouseDevice.Target.CaptureMouse();
        }
    }

    [RelayCommand]
    private void MouseMoveCanvas(MouseEventArgs e)
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
    private void MouseUpCanvas(MouseButtonEventArgs e)
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