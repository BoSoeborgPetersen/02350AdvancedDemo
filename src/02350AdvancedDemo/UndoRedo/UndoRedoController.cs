namespace _02350AdvancedDemo.UndoRedo;

public class UndoRedoController
{
    public static UndoRedoController Instance { get; } = new();

    readonly Stack<IUndoRedoCommand> undoStack = new();
    readonly Stack<IUndoRedoCommand> redoStack = new();

    UndoRedoController() { }

    public void AddAndExecute(IUndoRedoCommand command)
    {
        undoStack.Push(command);
        redoStack.Clear();
        command.Do();
    }

    public bool CanUndo(string steps) => undoStack.Count >= (steps == null ? 1 : int.Parse(steps));

    public void Undo(string steps)
    {
        if (!CanUndo(steps)) throw new InvalidOperationException();
        int s = steps == null ? 1 : int.Parse(steps);
        for (int i = 0; i < s; i++)
        {
            IUndoRedoCommand command = undoStack.Pop();
            redoStack.Push(command);
            command.Undo();
        }
    }

    public bool CanRedo(string steps) => redoStack.Count >= (steps == null ? 1 : int.Parse(steps));

    public void Redo(string steps)
    {
        if (!CanRedo(steps)) throw new InvalidOperationException();
        int s = steps == null ? 1 : int.Parse(steps);
        for (int i = 0; i < s; i++)
        {
            IUndoRedoCommand command = redoStack.Pop();
            undoStack.Push(command);
            command.Do();
        }
    }
}
