using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _02350AdvancedDemo.Model
{
    [ImplementPropertyChanged]
    public class Shape
    {
        private static int counter = 0;
        public int Number { get; } = ++counter;

        public int X { get; set; } = 200;
        public int Y { get; set; } = 200;
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
        
        public int CanvasCenterX { get { return X + Width / 2; } set { X = value - Width / 2; } }
        public int CanvasCenterY { get { return Y + Height / 2; } set { Y = value - Height / 2; } }

        public int CenterX => Width / 2;
        public int CenterY => Height / 2;
        
        public bool IsSelected { get; set; }
        public Brush SelectedColor => IsSelected ? Brushes.Red : Brushes.Yellow;

        public override string ToString() => Number.ToString();
    }
}
