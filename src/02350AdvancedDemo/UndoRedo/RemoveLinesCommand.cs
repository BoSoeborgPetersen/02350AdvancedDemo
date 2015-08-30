using _02350AdvancedDemo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class RemoveLinesCommand : IUndoRedoCommand
    {
        private ObservableCollection<Line> lines;
        
        private List<Line> linesToRemove;
        
        public RemoveLinesCommand(ObservableCollection<Line> _lines, List<Line> _linesToRemove) 
        {
            lines = _lines;
            linesToRemove = _linesToRemove;
        }
        
        public void Execute()
        {
            linesToRemove.ForEach(x => lines.Remove(x));
        }
        
        public void UnExecute()
        {
            linesToRemove.ForEach(x => lines.Add(x));
        }
    }
}
