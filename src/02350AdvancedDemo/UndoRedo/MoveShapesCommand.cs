namespace _02350AdvancedDemo.UndoRedo;

public record MoveShapesCommand(List<ShapeViewModel> Shapes, Vector Offset) : IUndoRedoCommand
{
    public void Do() => Shapes.ForEach(s => s.CanvasCenter += Offset);
    public void Undo() => Shapes.ForEach(s => s.CanvasCenter -= Offset);
}