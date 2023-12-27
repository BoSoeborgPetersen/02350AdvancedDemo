namespace _02350AdvancedDemo.Service;

public class SelectionBoxService(StateService state)
{
    Point start;
    Rect selectionBox;

    Point Position(MouseEventArgs e) => Mouse.GetPosition(e.MouseDevice.Target);
    Rect Box(MouseEventArgs e) => new(start, Position(e));

    public void CanvasMouseDown(MouseButtonEventArgs e)
    {
        if (!state.IsAddingLine)
        {
            start = Position(e);
            e.MouseDevice.Target.CaptureMouse();
        }
    }

    public Rect CanvasMouseMove(MouseEventArgs e)
    {
        if (Mouse.Captured != null && !state.IsAddingLine)
            selectionBox = Box(e);

        return selectionBox;
    }

    public Rect CanvasMouseUp(ObservableCollection<ShapeViewModel> shapes, MouseButtonEventArgs e)
    {
        if (!state.IsAddingLine)
        {
            foreach (var s in shapes)
                s.IsMoveSelected = Box(e).Contains(new Rect(s.Position, s.Size));

            e.MouseDevice.Target.ReleaseMouseCapture();
            selectionBox = new();
        }
        return selectionBox;
    }
}
