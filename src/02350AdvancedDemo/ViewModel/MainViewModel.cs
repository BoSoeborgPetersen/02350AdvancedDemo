using _02350AdvancedDemo.Model;
using _02350AdvancedDemo.Serialization;
using _02350AdvancedDemo.UndoRedo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _02350AdvancedDemo.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private UndoRedoController undoRedoController = UndoRedoController.GetInstance();
        private DialogViewModel dialogVM = new DialogViewModel();
        
        private bool isAddingLine;
        private Type addingLineType;
        private Shape addingLineFrom;
        private Point moveShapePoint;
        public double ModeOpacity => isAddingLine ? 0.4 : 1.0;
        public Visibility SidePanelVisibility { get; set; } = Visibility.Visible;
        public string SidePanelVisibilitySymbol { get; set; } = "<";

        public ObservableCollection<Shape> Shapes { get; set; }
        public ObservableCollection<Line> Lines { get; set; }

        public ICommand ToggleSidePanelVisibilityCommand { get; set; }

        public ICommand NewDiagramCommand { get; }
        public ICommand OpenDiagramCommand { get; }
        public ICommand SaveDiagramCommand { get; }

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand AddCircleCommand { get; }
        public ICommand AddSquareCommand { get; }
        public ICommand RemoveShapeCommand { get; }
        public ICommand AddLineCommand { get; }
        public ICommand AddDashLineCommand { get; }
        public ICommand RemoveLinesCommand { get; }
        
        public ICommand MouseDownShapeCommand { get; }
        public ICommand MouseMoveShapeCommand { get; }
        public ICommand MouseUpShapeCommand { get; }

        public MainViewModel()
        {
            Shapes = new ObservableCollection<Shape>() {
                new Circle() { X = 30, Y = 40, Width = 80, Height = 80, Data = new List<string> { "text1", "text2", "text3" } },
                new Square() { X = 140, Y = 230, Width = 200, Height = 100, Data = new List<string> { "text1", "text2", "text3" } }
            };

            Lines = new ObservableCollection<Line>() { 
                new Line() { From = Shapes[0], To = Shapes[1], Label = "Line Text" } 
            };

            ToggleSidePanelVisibilityCommand = new RelayCommand(ToggleSidePanelVisibility);

            NewDiagramCommand = new RelayCommand(NewDiagram);
            OpenDiagramCommand = new RelayCommand(OpenDiagram);
            SaveDiagramCommand = new RelayCommand(SaveDiagram);

            UndoCommand = new RelayCommand<string>(undoRedoController.Undo, undoRedoController.CanUndo);
            RedoCommand = new RelayCommand<string>(undoRedoController.Redo, undoRedoController.CanRedo);

            AddCircleCommand = new RelayCommand(AddCircle);
            AddSquareCommand = new RelayCommand(AddSquare);
            RemoveShapeCommand = new RelayCommand<IList>(RemoveShape, CanRemoveShape);
            AddLineCommand = new RelayCommand(AddLine);
            AddDashLineCommand = new RelayCommand(AddDashLine);
            RemoveLinesCommand = new RelayCommand<IList>(RemoveLines, CanRemoveLines);
            
            MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
            MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
            MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);
        }

        private void ToggleSidePanelVisibility()
        {
            SidePanelVisibility = SidePanelVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            RaisePropertyChanged(() => SidePanelVisibility);
            SidePanelVisibilitySymbol = SidePanelVisibilitySymbol == "<" ? ">" : "<";
            RaisePropertyChanged(() => SidePanelVisibilitySymbol);
        }

        private void NewDiagram()
        {
            if(dialogVM.ShowNew())
            {
                Shapes = new ObservableCollection<Shape>();
                RaisePropertyChanged(() => Shapes);
                Lines = new ObservableCollection<Line>();
                RaisePropertyChanged(() => Lines);
            }
        }

        private void OpenDiagram()
        {
            string path = dialogVM.ShowOpen();
            if (path != null)
            {
                Diagram diagram = SerializerXML.Instance.Deserialize(path);

                Shapes = new ObservableCollection<Shape>(diagram.Shapes);
                RaisePropertyChanged(() => Shapes);
                Lines = new ObservableCollection<Line>(diagram.Lines);
                RaisePropertyChanged(() => Lines);
            }
        }

        private void SaveDiagram()
        {
            string path = dialogVM.ShowSave();
            if (path != null)
            {
                Diagram diagram = new Diagram() { Shapes = Shapes.ToList(), Lines = Lines.ToList() };
                SerializerXML.Instance.Serialize(diagram, path);
            }
        }

        private void AddCircle()
        {
            undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new Circle()));
        }

        private void AddSquare()
        {
            undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new Square()));
        }

        private bool CanRemoveShape(IList _shapes) => _shapes.Count == 1;

        private void RemoveShape(IList _shapes)
        {
            undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, _shapes.Cast<Shape>().ToList()));
        }

        private void AddLine()
        {
            isAddingLine = true;
            addingLineType = typeof(Line);
            RaisePropertyChanged(() => ModeOpacity);
        }

        private void AddDashLine()
        {
            isAddingLine = true;
            addingLineType = typeof(DashLine);
            RaisePropertyChanged(() => ModeOpacity);
        }

        private bool CanRemoveLines(IList _edges) => _edges.Count >= 1;

        private void RemoveLines(IList _lines)
        {
            undoRedoController.AddAndExecute(new RemoveLinesCommand(Lines, _lines.Cast<Line>().ToList()));
        }

        private void MouseDownShape(MouseButtonEventArgs e)
        {
            if (!isAddingLine) e.MouseDevice.Target.CaptureMouse();
        }

        private void MouseMoveShape(MouseEventArgs e)
        {
            if (Mouse.Captured != null && !isAddingLine)
            {
                FrameworkElement shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
                Shape shapeModel = (Shape)shapeVisualElement.DataContext;
                Canvas canvas = FindParentOfType<Canvas>(shapeVisualElement);
                Point mousePosition = Mouse.GetPosition(canvas);
                if (moveShapePoint == default(Point)) moveShapePoint = mousePosition;
                shapeModel.CanvasCenterX = (int)mousePosition.X;
                shapeModel.CanvasCenterY = (int)mousePosition.Y;
            }
        }

        private void MouseUpShape(MouseButtonEventArgs e)
        {
            if (isAddingLine)
            {
                FrameworkElement shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
                Shape shape = (Shape)shapeVisualElement.DataContext;
                if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
                else if (addingLineFrom.Number != shape.Number)
                {
                    Line lineToAdd = addingLineType == typeof(Line) ? new Line() { From = addingLineFrom, To = shape } :
                        addingLineType == typeof(DashLine) ? new DashLine() { From = addingLineFrom, To = shape } : null;
                    undoRedoController.AddAndExecute(new AddLineCommand(Lines, lineToAdd));
                    addingLineFrom.IsSelected = false;
                    isAddingLine = false;
                    addingLineType = null;
                    addingLineFrom = null;
                    RaisePropertyChanged(() => ModeOpacity);
                }
            }
            else
            {
                FrameworkElement shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
                Shape shape = (Shape)shapeVisualElement.DataContext;
                Canvas canvas = FindParentOfType<Canvas>(shapeVisualElement);
                Point mousePosition = Mouse.GetPosition(canvas);
                undoRedoController.AddAndExecute(new MoveShapeCommand(shape, (int)moveShapePoint.X, (int)moveShapePoint.Y, (int)mousePosition.X, (int)mousePosition.Y));
                moveShapePoint = new Point();
                e.MouseDevice.Target.ReleaseMouseCapture();
            }
        }

        private static T FindParentOfType<T>(DependencyObject o)
        {
            dynamic parent = VisualTreeHelper.GetParent(o);
            return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
        }
    }
}