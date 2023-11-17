namespace _02350AdvancedDemo.UndoRedo;

public class RemoveShapesCommand(ObservableCollection<ShapeViewModel> _shapes, ObservableCollection<LineViewModel> _lines, List<ShapeViewModel> _shapesToRemove) : IUndoRedoCommand
{
    private readonly List<LineViewModel> linesToRemove = _lines.Where(x => _shapesToRemove.Any(y => y.Number == x.From.Number || y.Number == x.To.Number)).ToList();

    public void Do()
    {
        linesToRemove.ForEach(x => _lines.Remove(x));
        _shapesToRemove.ForEach(x => _shapes.Remove(x));
    }
    
    public void Undo()
    {
        _shapesToRemove.ForEach(_shapes.Add);
        linesToRemove.ForEach(_lines.Add);
    }
}
