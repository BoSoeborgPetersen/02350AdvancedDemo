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
    }

    public void MouseMove(ShapeViewModel shape, MouseEventArgs e)
    {
        if (Mouse.Captured != null && !IsAddingLine)
        {
            shape.Position = initialShapePosition + (shape.Position + RelativeMousePosition(e) - initialMousePosition);
        }
    }

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

    static Vector RelativeMousePosition(MouseEventArgs e) => (Vector)Mouse.GetPosition(e.MouseDevice.Target);
}
