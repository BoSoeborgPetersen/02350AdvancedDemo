namespace _02350AdvancedDemo.UndoRedo;

public record RemoveLinesCommand(ObservableCollection<LineViewModel> Lines, List<LineViewModel> LinesToRemove) : IUndoRedoCommand
{
    public void Do() => LinesToRemove.ForEach(x => Lines.Remove(x));
    public void Undo() => LinesToRemove.ForEach(x => Lines.Add(x));
}
