using _02350AdvancedDemo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class AddShapeCommand : IUndoRedoCommand
    {
        private ObservableCollection<Shape> shapes;
        private Shape shape;

        public AddShapeCommand(ObservableCollection<Shape> _shapes, Shape _shape) 
        { 
            shapes = _shapes;
            shape = _shape;
        }

        // Methods.
        public void Execute()
        {
            shapes.Add(shape);
        }
        
        public void UnExecute()
        {
            shapes.Remove(shape);
        }
    }
}
