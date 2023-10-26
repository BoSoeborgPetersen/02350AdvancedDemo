namespace _02350AdvancedDemo.UndoRedo;

public class RemoveShapesCommand : IUndoRedoCommand
{
    private readonly ObservableCollection<ShapeViewModel> shapes;

    private readonly ObservableCollection<LineViewModel> lines;
    
    private readonly List<ShapeViewModel> shapesToRemove;
    
    private readonly List<LineViewModel> linesToRemove;

    public RemoveShapesCommand(ObservableCollection<ShapeViewModel> _shapes, ObservableCollection<LineViewModel> _lines, List<ShapeViewModel> _shapesToRemove)
    {
        shapes = _shapes;
        lines = _lines;
        shapesToRemove = _shapesToRemove;
        linesToRemove = _lines.Where(x => _shapesToRemove.Any(y => y.Number == x.From.Number || y.Number == x.To.Number)).ToList();
    }
    
    public void Do()
    {
        linesToRemove.ForEach(x => lines.Remove(x));
        shapesToRemove.ForEach(x => shapes.Remove(x));
    }
    
    public void Undo()
    {
        shapesToRemove.ForEach(x => shapes.Add(x));
        linesToRemove.ForEach(x => lines.Add(x));
    }
}
