using _02350AdvancedDemo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.Command
{
    // Undo/Redo command for removing Lines.
    public class RemoveLinesCommand : IUndoRedoCommand
    {
        // Fields.
        // The 'lines' field holds the current collection of lines, 
        //  and the reference points to the same collection as the one the MainViewModel point to, 
        //  therefore when this collection is changed in a object of this class, 
        //  it also changes the collection that the MainViewModel uses.
        // For a description of an ObservableCollection see the MainViewModel class.
        private ObservableCollection<Line> lines;

        // The 'linesToRemove' field holds a collection of existing lines, that are removed from the 'lines' collection, 
        //  and if undone, they are added to the collection.
        private List<Line> linesToRemove;

        // Constructor for saving and changing the current state of the diagram 
        //  (or at least the relevant parts).
        public RemoveLinesCommand(ObservableCollection<Line> _lines, List<Line> _linesToRemove) 
        {
            lines = _lines;
            linesToRemove = _linesToRemove;
        }

        // Methods.
        // This method is for doing and redoing the command.
        public void Execute()
        {
            // This is a lambda expression, that iterates the 'linesToRemove' collection, 
            //  and for each one removes it from the 'lines' collection.
            linesToRemove.ForEach(x => lines.Remove(x));
            // This is equivalent to the following in Java (can also be done like this in .NET):

            // foreach (Line line in linesToRemove)
            // { 
            //   lines.Remove(line);
            // }
        }

        // This method is for undoing the command.
        public void UnExecute()
        {
            // This is a lambda expression, that iterates the 'linesToRemove' collection, 
            //  and for each one adds it to the 'lines' collection.
            linesToRemove.ForEach(x => lines.Add(x));
            // This is equivalent to the following in Java (can also be done like this in .NET):

            // foreach (Line line in linesToRemove)
            // { 
            //   lines.Add(line);
            // }
        }
    }
}
