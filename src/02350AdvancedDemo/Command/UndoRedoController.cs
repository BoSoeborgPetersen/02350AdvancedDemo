using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.Command
{
    // Keeps track of the Undo/Redo commands.
    // This is a Singleton, which ensures there will only ever be one instance of the class.
    // There should never be more than one, otherwise problems could arise.
    public class UndoRedoController
    {
        // Part of the Singleton design pattern.
        // This holds the only instance of the class that will ever exist.
        // See (http://en.wikipedia.org/wiki/Singleton_pattern) for a description of the Singleton design pattern, 
        //  look under eager initialization for this version of the design pattern.
        private static UndoRedoController controller = new UndoRedoController();

        // The Undo stack, holding the Undo/Redo commands that have been executed.
        private readonly Stack<IUndoRedoCommand> undoStack = new Stack<IUndoRedoCommand>();
        // The Redo stack, holding the Undo/Redo commands that have been executed and then unexecuted (undone).
        private readonly Stack<IUndoRedoCommand> redoStack = new Stack<IUndoRedoCommand>();

        // Part of the Singleton design pattern.
        // This ensures that only the 'UndoRedoController' can instantiate itself.
        private UndoRedoController() { }

        // Part of the Singleton design pattern.
        // This is used by other objects to retrieve a reference to the single 'UndoRedoController' object.
        public static UndoRedoController GetInstance() { return controller; }

        // Used for adding the Undo/Redo command to the 'undoStack' and at the same time executing it.
        public void AddAndExecute(IUndoRedoCommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();
            command.Execute();
        }

        // This informs the View (GUI) when the Undo command can be used.
        public bool CanUndo()
        {
            // Lambda expression to check that the 'undoStack' collection is not empty.
            return undoStack.Any();
            // This is equivalent to the following in Java (can also be done like this in .NET):

            // return undoStack.Count > 0;
        }

        // Undoes the Undo/Redo command that was last executed, if possible.
        public void Undo()
        {
            if (!undoStack.Any()) throw new InvalidOperationException();
            IUndoRedoCommand command = undoStack.Pop();
            redoStack.Push(command);
            command.UnExecute();
        }

        // This informs the View (GUI) when the Redo command can be used.
        public bool CanRedo()
        {
            // Lambda expression to check that the 'redoStack' collection is not empty.
            return redoStack.Any();
            // This is equivalent to the following in Java (can also be done like this in .NET):

            // return redoStack.Count > 0;
        }

        // Redoes the Undo/Redo command that was last unexecuted (undone), if possible.
        public void Redo()
        {
            if (!redoStack.Any()) throw new InvalidOperationException();
            IUndoRedoCommand command = redoStack.Pop();
            undoStack.Push(command);
            command.Execute();
        }
    }
}
