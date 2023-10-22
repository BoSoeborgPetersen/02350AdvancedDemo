using _02350AdvancedDemo.Model;
using _02350AdvancedDemo.UndoRedo;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

namespace _02350AdvancedDemo.ViewModel
{
    public abstract class ShapeViewModel : BaseViewModel
    {
        public ICommand RemoveCommand { get; }

        public Shape Shape { get; set; }

        public int Number { get { return Shape.Number; } set { Shape.Number = value; OnPropertyChanged(); } }
        public double X { get { return Shape.X; } set { Shape.X = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanvasCenterX)); } }
        public double Y { get { return Shape.Y; } set { Shape.Y = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanvasCenterY)); } }
        public double Width { get { return Shape.Width; } set { Shape.Width = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanvasCenterX)); OnPropertyChanged(nameof(CenterX)); } }
        public double Height { get { return Shape.Height; } set { Shape.Height = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanvasCenterY)); OnPropertyChanged(nameof(CenterY)); } }
        public List<string> Data { get { return Shape.Data; } set { Shape.Data = value; } }
        public double CanvasCenterX { get { return X + Width / 2; } set { X = value - Width / 2; OnPropertyChanged(nameof(X)); } }
        public double CanvasCenterY { get { return Y + Height / 2; } set { Y = value - Height / 2; OnPropertyChanged(nameof(Y)); } }
        public double CenterX => Width / 2;
        public double CenterY => Height / 2;
        private bool isSelected;
        public bool IsSelected { get { return isSelected; } set { isSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(SelectedColor)); } }
        public Brush SelectedColor => IsSelected ? Brushes.Red : Brushes.Yellow;
        private bool isMoveSelected;
        public bool IsMoveSelected { get { return isMoveSelected; } set { isMoveSelected = value; OnPropertyChanged(); OnPropertyChanged(nameof(BackgroundColor)); } }
        public Brush BackgroundColor => IsMoveSelected ? Brushes.SkyBlue : Brushes.Navy;

        public ShapeViewModel(Shape _shape) : base()
        {
            Shape = _shape;

            RemoveCommand = new RelayCommand(Remove);
        }

        private void Remove()
        {
            undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, new List<ShapeViewModel>() { this }));
        }

        public override string ToString() => Number.ToString();
    }
}
