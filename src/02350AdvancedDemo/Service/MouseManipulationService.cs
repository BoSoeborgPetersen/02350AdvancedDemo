namespace _02350AdvancedDemo.Service;

public class MouseManipulationService
{
    readonly UndoRedoController undoRedoController = UndoRedoController.Instance;

    public static MouseManipulationService Instance { get; } = new();

    MouseManipulationService() { }

    public ObservableCollection<LineViewModel> Lines;
    bool isAddingLine;
    public bool IsAddingLine { get { return isAddingLine; } set { isAddingLine = value; WeakReferenceMessenger.Default.Send(new IsAddingLineMessage()); } }
    ShapeViewModel addingLineFrom;
    Point initialShapePosition;
    Point initialMousePosition;

    public void MouseDown(ShapeViewModel shape, MouseButtonEventArgs e)
    {
        if (!IsAddingLine)
        {
            initialShapePosition = shape.Position;
            initialMousePosition = shape.Position + RelativeMousePosition(e);

            e.MouseDevice.Target.CaptureMouse();
        }
        e.Handled = true;
    }

    //public void MouseDownShape(MouseButtonEventArgs e)
    //{
    //    if (!IsAddingLine)
    //    {
    //        //var shape = TargetShape(e);
    //        var mousePosition = RelativeMousePosition(e);

    //        initialMousePosition = mousePosition;

    //        var selectedShapes = Shapes.Where(x => x.IsMoveSelected);
    //        if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };

    //        foreach (var s in selectedShapes)
    //            initialShapePositions.Add(s.Number, s.Position);

    //        e.MouseDevice.Target.CaptureMouse();
    //    }
    //    e.Handled = true;
    //}

    public void MouseMove(ShapeViewModel shape, MouseEventArgs e)
    {
        if (Mouse.Captured != null && !IsAddingLine)
        {
            shape.Position = initialShapePosition + (shape.Position + RelativeMousePosition(e) - initialMousePosition);
        }
    }

    //public void MouseMoveShape(MouseEventArgs e)
    //{
    //    if (Mouse.Captured != null && !IsAddingLine)
    //    {
    //        var mousePosition = RelativeMousePosition(e);

    //        var selectedShapes = Shapes.Where(x => x.IsMoveSelected);
    //        if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };

    //        foreach (var s in selectedShapes)
    //        {
    //            var originalPosition = initialShapePositions[s.Number];
    //            s.Position = (originalPosition + (mousePosition - initialMousePosition));
    //        }

    //        e.Handled = true;
    //    }
    //}

    public void MouseUp(ShapeViewModel shape, MouseButtonEventArgs e)
    {
        if (!IsAddingLine)
        {
            var mousePosition = shape.Position + RelativeMousePosition(e);
            shape.Position = initialShapePosition;

            //undoRedoController.AddAndExecute(new MoveShapesCommand(shape, mousePosition - initialMousePosition));

            e.MouseDevice.Target.ReleaseMouseCapture();
        }
        else
        {
            if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
            else if (addingLineFrom.Number != shape.Number)
            {
                undoRedoController.AddAndExecute(new AddLineCommand(Lines, new() { From = addingLineFrom, To = shape }));
                addingLineFrom.IsSelected = false;
                IsAddingLine = false;
                WeakReferenceMessenger.Default.Send(new IsAddingLineMessage());
                addingLineFrom = null;
            }
        }
    }

    //public void MouseUpShape(MouseButtonEventArgs e)
    //{
    //    if (IsAddingLine)
    //    {
    //        var shape = TargetShape(e);

    //        if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
    //        else if (addingLineFrom.Number != shape.Number)
    //        {
    //            //LineViewModel lineToAdd = new(
    //            //    addingLineType == typeof(Line) ? new Line() :
    //            //    new DashLine()
    //            //)
    //            //{ From = addingLineFrom, To = shape };
    //            //undoRedoController.AddAndExecute(new AddLineCommand(Lines, lineToAdd));
    //            //addingLineFrom.IsSelected = false;
    //            //IsAddingLine = false;
    //            //addingLineType = null;
    //            //addingLineFrom = null;
    //        }
    //    }
    //    else
    //    {
    //        var mousePosition = RelativeMousePosition(e);

    //        var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
    //        if (!selectedShapes.Any()) selectedShapes = [TargetShape(e)];

    //        foreach (var s in selectedShapes)
    //        {
    //            var originalPosition = initialShapePositions[s.Number];
    //            s.Position = originalPosition;
    //        }
    //        undoRedoController.AddAndExecute(new MoveShapesCommand(selectedShapes, mousePosition - initialMousePosition));

    //        initialMousePosition = new();
    //        initialShapePositions.Clear();
    //        e.MouseDevice.Target.ReleaseMouseCapture();
    //    }
    //    e.Handled = true;
    //}

    static Vector RelativeMousePosition(MouseEventArgs e) => (Vector)Mouse.GetPosition(e.MouseDevice.Target);
}
