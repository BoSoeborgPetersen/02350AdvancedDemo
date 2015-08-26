using _02350AdvancedDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.Command
{
    // Undo/Redo command for moving a Shape.
    public class MoveShapeCommand : IUndoRedoCommand
    {
        // Fields.
        // The 'shape' field holds an existing shape, 
        //  and the reference points to the same object, 
        //  as one of the objects in the MainViewModels 'Shapes' ObservableCollection.
        // This shape is moved by changing its coordinates (X and Y), 
        //  and if undone the coordinates are changed back to the original coordinates.
        private Shape shape;

        // The 'beforeX' field holds the X coordinate of the shape before it is moved.
        private int beforeX;
        // The 'beforeY' field holds the Y coordinate of the shape after it is moved.
        private int beforeY;
        // The 'afterX' field holds the X coordinate of the shape before it is moved.
        private int afterX;
        // The 'afterY' field holds the Y coordinate of the shape after it is moved.
        private int afterY;

        // Constructor for saving and changing the current state of the diagram 
        //  (or at least the relevant parts).
        public MoveShapeCommand(Shape _shape, int _beforeX, int _beforeY, int _afterX, int _afterY) 
        {
            shape = _shape;
            beforeX = _beforeX;
            beforeY = _beforeY;
            afterX = _afterX;
            afterY = _afterY;
        }

        // Methods.
        // This method is for doing and redoing the command.
        public void Execute()
        {
            shape.CanvasCenterX = afterX;
            shape.CanvasCenterY = afterY;
        }

        // This method is for undoing the command.
        public void UnExecute()
        {
            shape.CanvasCenterX = beforeX;
            shape.CanvasCenterY = beforeY;
        }
    }
}
