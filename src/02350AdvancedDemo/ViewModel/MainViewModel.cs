using _02350AdvancedDemo.Model;
using _02350AdvancedDemo.Serialization;
using _02350AdvancedDemo.UndoRedo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
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
        
        private bool isAddingLine;
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

        public ICommand AddShapeCommand { get; }
        public ICommand RemoveShapeCommand { get; }
        public ICommand AddLineCommand { get; }
        public ICommand RemoveLinesCommand { get; }
        
        public ICommand MouseDownShapeCommand { get; }
        public ICommand MouseMoveShapeCommand { get; }
        public ICommand MouseUpShapeCommand { get; }

        public MainViewModel()
        {
            Shapes = new ObservableCollection<Shape>() { 
                new Shape() { X = 30, Y = 40, Width = 50, Height = 50 }, 
                new Shape() { X = 140, Y = 230, Width = 200, Height = 200 } 
            };

            Lines = new ObservableCollection<Line>() { 
                new Line() { From = Shapes[0], To = Shapes[1] } 
            };

            ToggleSidePanelVisibilityCommand = new RelayCommand(ToggleSidePanelVisibility);

            NewDiagramCommand = new RelayCommand(NewDiagram);
            OpenDiagramCommand = new RelayCommand(OpenDiagram);
            SaveDiagramCommand = new RelayCommand(SaveDiagram);

            UndoCommand = new RelayCommand<string>(undoRedoController.Undo, undoRedoController.CanUndo);
            RedoCommand = new RelayCommand<string>(undoRedoController.Redo, undoRedoController.CanRedo);

            AddShapeCommand = new RelayCommand(AddShape);
            RemoveShapeCommand = new RelayCommand<IList>(RemoveShape, CanRemoveShape);
            AddLineCommand = new RelayCommand(AddLine);
            RemoveLinesCommand = new RelayCommand<IList>(RemoveLines, CanRemoveLines);
            
            MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
            MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
            MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);
        }

        public void ToggleSidePanelVisibility()
        {
            SidePanelVisibility = SidePanelVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            RaisePropertyChanged(() => SidePanelVisibility);
            SidePanelVisibilitySymbol = SidePanelVisibilitySymbol == "<" ? ">" : "<";
            RaisePropertyChanged(() => SidePanelVisibilitySymbol);
        }

        public void NewDiagram()
        {
            if(MessageBox.Show("Are you sure (bla bla)?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Shapes = new ObservableCollection<Shape>();
                RaisePropertyChanged(() => Shapes);
                Lines = new ObservableCollection<Line>();
                RaisePropertyChanged(() => Lines);
            }
        }

        public void OpenDiagram()
        {
            OpenFileDialog loadDialog = new OpenFileDialog() { Title = "Open Diagram", Filter = "XML Document (.xml)|*.xml", DefaultExt = "xml", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CheckFileExists = true };
            if (loadDialog.ShowDialog() == true)
            {
                string path = loadDialog.FileName;
                Diagram diagram = SerializerXML.Instance.Deserialize(path);
                // Reconstruct object graph.
                diagram.Lines.ForEach(x => x.From = diagram.Shapes.Single(y => y.Number == x.FromNumber));
                diagram.Lines.ForEach(x => x.To = diagram.Shapes.Single(y => y.Number == x.ToNumber));

                Shapes = new ObservableCollection<Shape>(diagram.Shapes);
                RaisePropertyChanged(() => Shapes);
                Lines = new ObservableCollection<Line>(diagram.Lines);
                RaisePropertyChanged(() => Lines);
            }
        }

        public void SaveDiagram()
        {
            SaveFileDialog saveDialog = new SaveFileDialog() { Title = "Save Diagram", Filter = "XML Document (.xml)|*.xml", DefaultExt = "xml", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
            if (saveDialog.ShowDialog() == true)
            {
                string path = saveDialog.FileName;
                Diagram diagram = new Diagram() { Shapes = Shapes.ToList(), Lines = Lines.ToList() };
                SerializerXML.Instance.Serialize(diagram, path);
            }
        }
        
        public void AddShape()
        {
            undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new Shape()));
        }

        public bool CanRemoveShape(IList _shapes) => _shapes.Count == 1;

        public void RemoveShape(IList _shapes)
        {
            undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, _shapes.Cast<Shape>().ToList()));
        }

        public void AddLine()
        {
            isAddingLine = true;
            RaisePropertyChanged("ModeOpacity");
        }

        public bool CanRemoveLines(IList _edges) => _edges.Count >= 1;

        public void RemoveLines(IList _lines)
        {
            undoRedoController.AddAndExecute(new RemoveLinesCommand(Lines, _lines.Cast<Line>().ToList()));
        }
        
        public void MouseDownShape(MouseButtonEventArgs e)
        {
            if (!isAddingLine) e.MouseDevice.Target.CaptureMouse();
        }
        
        public void MouseMoveShape(MouseEventArgs e)
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

        public void MouseUpShape(MouseButtonEventArgs e)
        {
            if (isAddingLine)
            {
                FrameworkElement shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
                Shape shape = (Shape)shapeVisualElement.DataContext;
                if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
                else if (addingLineFrom.Number != shape.Number)
                {
                    undoRedoController.AddAndExecute(new AddLineCommand(Lines, new Line() { From = addingLineFrom, To = shape }));
                    addingLineFrom.IsSelected = false;
                    isAddingLine = false;
                    addingLineFrom = null;
                    RaisePropertyChanged("ModeOpacity");
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