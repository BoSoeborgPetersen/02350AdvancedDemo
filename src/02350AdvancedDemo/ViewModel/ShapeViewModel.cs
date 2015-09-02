using _02350AdvancedDemo.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _02350AdvancedDemo.ViewModel
{
    public abstract class ShapeViewModel : ViewModelBase
    {
        public Shape Shape { get; set; }

        public int Number { get { return Shape.Number; } set { Shape.Number = value; RaisePropertyChanged(); } }
        public int X { get { return Shape.X; } set { Shape.X = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterX); } }
        public int Y { get { return Shape.Y; } set { Shape.Y = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterY); } }
        public int Width { get { return Shape.Width; } set { Shape.Width = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterX); RaisePropertyChanged(() => CenterX); } }
        public int Height { get { return Shape.Height; } set { Shape.Height = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterY); RaisePropertyChanged(() => CenterY); } }
        public List<string> Data { get { return Shape.Data; } set { Shape.Data = value; } }
        public int CanvasCenterX { get { return X + Width / 2; } set { X = value - Width / 2; RaisePropertyChanged(() => X); } }
        public int CanvasCenterY { get { return Y + Height / 2; } set { Y = value - Height / 2; RaisePropertyChanged(() => Y); } }
        public int CenterX => Width / 2;
        public int CenterY => Height / 2;
        private bool isSelected;
        public bool IsSelected { get { return isSelected; } set { isSelected = value; RaisePropertyChanged(); RaisePropertyChanged(() => SelectedColor); } }
        public Brush SelectedColor => IsSelected ? Brushes.Red : Brushes.Yellow;

        public ShapeViewModel(Shape _shape)
        {
            Shape = _shape;
        }
        public override string ToString() => Number.ToString();
    }
}
