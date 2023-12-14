namespace _02350AdvancedDemo.UndoRedo;

public record RemoveShapesCommand(ObservableCollection<ShapeViewModel> Shapes, ObservableCollection<LineViewModel> Lines, List<ShapeViewModel> ShapesToRemove, List<LineViewModel> LinesToRemove) : IUndoRedoCommand
{
    public void Do() { LinesToRemove.ForEach(l => Lines.Remove(l)); ShapesToRemove.ForEach(s => Shapes.Remove(s)); }
    public void Undo() { ShapesToRemove.ForEach(Shapes.Add); LinesToRemove.ForEach(Lines.Add); }
}
