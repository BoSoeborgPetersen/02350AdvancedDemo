namespace _02350AdvancedDemo.UndoRedo;

public interface IUndoRedoCommand
{
    void Do();
    void Undo();
}
