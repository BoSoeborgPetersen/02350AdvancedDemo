using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.Command
{
    // Custom interface for implementing Undo/Redo commands.
    public interface IUndoRedoCommand
    {
        // Methods (that has to be implemented).
        // This method is for doing and redoing the command.
        void Execute();
        // This method is for undoing the command.
        void UnExecute();
    }
}
