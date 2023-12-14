namespace _02350AdvancedDemo.UndoRedo;

public class UndoRedoController
{
    readonly Stack<IUndoRedoCommand> undoStack = new();
    readonly Stack<IUndoRedoCommand> redoStack = new();

    public static UndoRedoController Instance { get; } = new();

    UndoRedoController() { }

    public void AddAndExecute(IUndoRedoCommand command)
    {
        undoStack.Push(command);
        redoStack.Clear();
        command.Do();
        SendUndoRedoChangedMessage();
    }

    public bool CanUndo(int count) => undoStack.Count >= count;

    public void Undo(int count)
    {
        if (CanUndo(count))
        {
            for(int i = 0; i < count; i++)
            {
                var command = undoStack.Pop();
                redoStack.Push(command);
                command.Undo();
                SendUndoRedoChangedMessage();
            }
        }
    }

    public bool CanRedo(int count) => redoStack.Count >= count;

    public void Redo(int count)
    {
        if (CanRedo(count))
        {
            for (int i = 0; i < count; i++)
            {
                var command = redoStack.Pop();
                undoStack.Push(command);
                command.Do();
                SendUndoRedoChangedMessage();
            }
        }
    }

    void SendUndoRedoChangedMessage() => WeakReferenceMessenger.Default.Send(new UndoRedoChangedMessage());
}
