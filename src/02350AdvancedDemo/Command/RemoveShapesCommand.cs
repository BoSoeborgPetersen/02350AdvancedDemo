using _02350AdvancedDemo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.Command
{
    // Undo/Redo command for removing Shapes.
    // It also removes the Lines connected to the Shapes.
    public class RemoveShapesCommand : IUndoRedoCommand
    {
        // Fields.
        // The 'shapes' field holds the current collection of shapes, 
        //  and the reference points to the same collection as the one the MainViewModel point to, 
        //  therefore when this collection is changed in a object of this class, 
        //  it also changes the collection that the MainViewModel uses.
        // For a description of an ObservableCollection see the MainViewModel class.
        private ObservableCollection<Shape> shapes;

        // The 'lines' field holds the current collection of lines, 
        //  and the reference points to the same collection as the one the MainViewModel point to, 
        //  therefore when this collection is changed in a object of this class, 
        //  it also changes the collection that the MainViewModel uses.
        // For a description of an ObservableCollection see the MainViewModel class.
        private ObservableCollection<Line> lines;

        // The 'shapesToRemove' field holds a collection of existing shapes, that are removed from the 'shapes' collection, 
        //  and if undone, they are added to the collection.
        private List<Shape> shapesToRemove;

        // The 'linesToRemove' field holds a collection of existing lines, that are removed from the 'lines' collection, 
        //  and if undone, they are added to the collection.
        private List<Line> linesToRemove;

        // Constructor for saving and changing the current state of the diagram 
        //  (or at least the relevant parts).
        public RemoveShapesCommand(ObservableCollection<Shape> _shapes, ObservableCollection<Line> _lines, List<Shape> _shapesToRemove)
        {
            shapes = _shapes;
            lines = _lines;
            shapesToRemove = _shapesToRemove;
            // Lambda expression to sort the '_lines' collection, and save the result in the 'linesToRemove collection.
            // The result are the lines that are connected to a shape in the 'shapesToRemove' collection.
            linesToRemove = _lines.Where(x => _shapesToRemove.Any(y => y.Number == x.From.Number || y.Number == x.To.Number)).ToList();
            // This is equivalent to the following in Java (can also be done like this in .NET):

            // linesToRemove = new List<Line>();
            //
            // foreach (Line line in _lines)
            // { 
            //   bool inBothCollections;
            //
            //   foreach (Line lineToRemove in _shapesToRemove)
            //   {
            //     if(lineToRemove.Number == line.From.Number || lineToRemove.Number == line.To.Number)
            //     {
            //       inBothCollections = true;
            //     }
            //   }
            //
            //   linesToRemove.Add(line);
            // }
        }

        // Methods.
        // This method is for doing and redoing the command.
        public void Execute()
        {
            // See RemoveLinesCommand for an explanation of lambda expressions.
            // First the lines to and from the shapes have to be removed to keep the data model consistent.
            linesToRemove.ForEach(x => lines.Remove(x));
            // Then the shapes can be removed.
            shapesToRemove.ForEach(x => shapes.Remove(x));
        }

        // This method is for undoing the command.
        public void UnExecute()
        {
            // See RemoveLinesCommand for an explanation of lambda expressions.
            // First the shapes have to be added.
            shapesToRemove.ForEach(x => shapes.Add(x));
            // Then the lines to and from the shapes have to be added to keep the data model consistent.
            linesToRemove.ForEach(x => lines.Add(x));
        }
    }
}
