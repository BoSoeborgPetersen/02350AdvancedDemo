namespace _02350AdvancedDemo.UndoRedo;

public record RemoveShapesCommand(ObservableCollection<ShapeViewModel> _shapes, ObservableCollection<LineViewModel> _lines, List<ShapeViewModel> _shapesToRemove) : IUndoRedoCommand
{
    readonly List<LineViewModel> linesToRemove = _lines.Where(l => _shapesToRemove.Any(y => y.Number == l.From.Number || y.Number == l.To.Number)).ToList();

    public void Do()
    {
        linesToRemove.ForEach(l => _lines.Remove(l));
        _shapesToRemove.ForEach(s => _shapes.Remove(s));
    }
    
    public void Undo()
    {
        _shapesToRemove.ForEach(_shapes.Add);
        linesToRemove.ForEach(_lines.Add);
    }
}
