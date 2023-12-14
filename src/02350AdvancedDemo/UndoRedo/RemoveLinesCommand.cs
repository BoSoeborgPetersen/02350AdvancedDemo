namespace _02350AdvancedDemo.UndoRedo;

public record RemoveLinesCommand(ObservableCollection<LineViewModel> Lines, List<LineViewModel> LinesToRemove) : IUndoRedoCommand
{
    public void Do() => LinesToRemove.ForEach(l => Lines.Remove(l));
    public void Undo() => LinesToRemove.ForEach(Lines.Add);
}
