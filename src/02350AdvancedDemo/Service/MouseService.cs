namespace _02350AdvancedDemo.Service;

public class MouseService(StateService state, UndoRedoController undoRedo)
{
    ShapeViewModel addingLineFrom;
    Dictionary<ShapeViewModel, Point> initialShapePositions = [];
    Point initialMousePosition;

    static Point MousePosition(Point offset, MouseEventArgs e) => offset + (Vector)Mouse.GetPosition(e.MouseDevice.Target);
    public IEnumerable<ShapeViewModel> SelectedShapes(ShapeViewModel shape) => state.Shapes.Any(x => x.IsMoveSelected) ? state.Shapes.Where(x => x.IsMoveSelected) : [shape];
    public Dictionary<ShapeViewModel, Point> ShapePositions(ShapeViewModel shape) => SelectedShapes(shape).ToDictionary(s => s, s => s.Position);

    public void ShapeMouseDown(ShapeViewModel shape, MouseButtonEventArgs e)
    {
        if (!state.IsAddingLine)
        {
            initialShapePositions = ShapePositions(shape);
            initialMousePosition = MousePosition(shape.Position, e);

            e.MouseDevice.Target.CaptureMouse();
        }
        e.Handled = true;
    }

    public void ShapeMouseMove(ShapeViewModel shape, MouseEventArgs e)
    {
        if (Mouse.Captured != null && !state.IsAddingLine)
        {
            foreach (var s in initialShapePositions.Select(kv => kv.Key))
                s.Position = initialShapePositions[s] + (MousePosition(shape.Position, e) - initialMousePosition);

            e.Handled = true;
        }
    }

    public void ShapeMouseUp(ShapeViewModel shape, MouseButtonEventArgs e)
    {
        if (!state.IsAddingLine)
        {
            var mousePosition = MousePosition(shape.Position, e);

            foreach (var s in initialShapePositions.Select(kv => kv.Key))
                s.Position = initialShapePositions[s];

            undoRedo.AddAndExecute(new MoveShapesCommand(initialShapePositions.Select(kv => kv.Key).ToList(), mousePosition - initialMousePosition));

            e.MouseDevice.Target.ReleaseMouseCapture();

            e.Handled = true;
        }
        else
        {
            if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
            else if (addingLineFrom.Number != shape.Number)
            {
                undoRedo.AddAndExecute(new AddLineCommand(state.Lines, 
                    state.AddingLineType == typeof(Line) ? 
                        new LineViewModel() { From = addingLineFrom, To = shape } : 
                        new DashLineViewModel() { From = addingLineFrom, To = shape }));
                addingLineFrom.IsSelected = false;
                state.AddingLineType = default;
                addingLineFrom = null;
            }
        }
    }
}
