using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class UndoRedoController
    {
        private static UndoRedoController controller = new UndoRedoController();

        private readonly Stack<IUndoRedoCommand> undoStack = new Stack<IUndoRedoCommand>();
        private readonly Stack<IUndoRedoCommand> redoStack = new Stack<IUndoRedoCommand>();

        private UndoRedoController() { }

        public static UndoRedoController GetInstance() => controller;

        public void AddAndExecute(IUndoRedoCommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();
            command.Execute();
        }

        public bool CanUndo() => undoStack.Any();

        public void Undo()
        {
            if (!undoStack.Any()) throw new InvalidOperationException();
            IUndoRedoCommand command = undoStack.Pop();
            redoStack.Push(command);
            command.UnExecute();
        }

        public bool CanRedo() => redoStack.Any();

        public void Redo()
        {
            if (!redoStack.Any()) throw new InvalidOperationException();
            IUndoRedoCommand command = redoStack.Pop();
            undoStack.Push(command);
            command.Execute();
        }
    }
}
