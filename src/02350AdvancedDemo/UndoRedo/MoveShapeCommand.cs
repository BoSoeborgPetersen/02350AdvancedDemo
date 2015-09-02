using _02350AdvancedDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class MoveShapeCommand : IUndoRedoCommand
    {
        private ShapeViewModel shape;
        
        private int beforeX;
        private int beforeY;
        private int afterX;
        private int afterY;
        
        public MoveShapeCommand(ShapeViewModel _shape, int _beforeX, int _beforeY, int _afterX, int _afterY) 
        {
            shape = _shape;
            beforeX = _beforeX;
            beforeY = _beforeY;
            afterX = _afterX;
            afterY = _afterY;
        }
        
        public void Execute()
        {
            shape.CanvasCenterX = afterX;
            shape.CanvasCenterY = afterY;
        }
        
        public void UnExecute()
        {
            shape.CanvasCenterX = beforeX;
            shape.CanvasCenterY = beforeY;
        }
    }
}
