using _02350AdvancedDemo.Command;
using _02350AdvancedDemo.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _02350AdvancedDemo.ViewModel
{
    // Use the <summary>...</summary> syntax to create XML Comments, used by Intellisence (Java: Content Assist), 
    // and to generate many types of documentation.
    /// <summary>
    /// This ViewModel is bound to the MainWindow View.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // A reference to the Undo/Redo controller.
        private UndoRedoController undoRedoController = UndoRedoController.GetInstance();

        // Keeps track of the state, depending on whether a line is being added or not.
        private bool isAddingLine;
        // Used for saving the shape that a line is drawn from, while it is being drawn.
        private Shape addingLineFrom;
        // Saves the initial point that the mouse has during a move operation.
        private Point moveShapePoint;
        // Used for making the shapes transparent when a new line is being added.
        public double ModeOpacity { get { return isAddingLine ? 0.4 : 1.0; } }

        // The purpose of using an ObservableCollection instead of a List is that it implements the INotifyCollectionChanged interface, 
        //  which is different from the INotifyPropertyChanged interface.
        // By implementing the INotifyCollectionChanged interface, an event is thrown whenever an element is added or removed from the collection.
        // The INotifyCollectionChanged event is then used by the View, 
        //  which update the graphical representation to show the elements that are now in the collection.
        // Also the collection is generic ("<Type>"), which means that it can be defined to hold all kinds of objects (and primitives), 
        //  but at runtime it is optimized for the specific type and can only hold that type.
        public ObservableCollection<Shape> Shapes { get; set; }
        public ObservableCollection<Line> Lines { get; set; }

        // Commands that the UI can be bound to.
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }

        // Commands that the UI can be bound to.
        public ICommand AddShapeCommand { get; private set; }
        public ICommand RemoveShapeCommand { get; private set; }
        public ICommand AddLineCommand { get; private set; }
        public ICommand RemoveLinesCommand { get; private set; }

        // Commands that the UI can be bound to.
        public ICommand MouseDownShapeCommand { get; private set; }
        public ICommand MouseMoveShapeCommand { get; private set; }
        public ICommand MouseUpShapeCommand { get; private set; }

        public MainViewModel()
        {
            // Here the list of Shapes is filled with 2 Nodes. 
            Shapes = new ObservableCollection<Shape>() { 
                // The "new Type() { prop1 = value1, prop2 = value }" syntax is called an Object Initializer, which creates an object and sets its values.

                // This is equivalent to the following:
                // Shape shape1 = new Shape();
                // shape1.X = 30;
                // shape1.Y = 40;
                // shape1.Width = 80;
                // shape1.Height = 80;

                // Also a constructor could be created for the Shape class that takes the parameters (X, Y, Width and Height), 
                //  and the following could be done:
                // new Shape(30, 40, 80, 80);

                new Shape() { X = 30, Y = 40, Width = 80, Height = 80 }, 
                new Shape() { X = 140, Y = 230, Width = 100, Height = 100 } 
            };
            // Here the list of Lines i filled with 1 Line that connects the 2 Shapes in the Shapes collection.
            // ElementAt() is an Extension Method, that like many others can be used on all types of collections.
            // It works just like the "Shapes[0]" syntax would be used for arrays.
            Lines = new ObservableCollection<Line>() { 
                new Line() { From = Shapes.ElementAt(0), To = Shapes.ElementAt(1) } 
            };

            // The commands are given the methods they should use to execute, and find out if they can execute.
            // For these commands the methods are not part of the MainViewModel, but part of the UndoRedoController.
            // Her vidersendes metode kaldne til UndoRedoControlleren.
            UndoCommand = new RelayCommand(undoRedoController.Undo, undoRedoController.CanUndo);
            RedoCommand = new RelayCommand(undoRedoController.Redo, undoRedoController.CanRedo);

            // The commands are given the methods they should use to execute, and find out if they can execute.
            AddShapeCommand = new RelayCommand(AddShape);
            RemoveShapeCommand = new RelayCommand<IList>(RemoveShape, CanRemoveShape);
            AddLineCommand = new RelayCommand(AddLine);
            RemoveLinesCommand = new RelayCommand<IList>(RemoveLines, CanRemoveLines);

            // The commands are given the methods they should use to execute, and find out if they can execute.
            MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
            MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
            MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);
        }

        // Adds a Shape with an AddShapeCommand.
        public void AddShape()
        {
            undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new Shape()));
        }

        // Checks if the chosen Shapes can be removed, which they can if exactly 1 is chosen.
        public bool CanRemoveShape(IList _shapes)
        {
            return _shapes.Count == 1;
        }

        // Removes the chosen Shapes with a RemoveShapesCommand.
        public void RemoveShape(IList _shapes)
        {
            undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, _shapes.Cast<Shape>().ToList()));
        }

        // Starts the procedure to remove a Line, by changing the mode to 'isAddingLine', 
        //  and making the shapes transparent.
        public void AddLine()
        {
            isAddingLine = true;
            RaisePropertyChanged("ModeOpacity");
        }

        // Checks if the chosen Lines can be removed, which they can if at least one is chosen.
        public bool CanRemoveLines(IList _edges)
        {
            return _edges.Count >= 1;
        }
        
        // Removes the chosen Lines with a RemoveLinesCommand.
        public void RemoveLines(IList _lines)
        {
            undoRedoController.AddAndExecute(new RemoveLinesCommand(Lines, _lines.Cast<Line>().ToList()));
        }
        
        // There are two reasons for doing a 'MouseDown' on a Shape, to move it or to draw a line from it.
        // It the state is not 'isAddingEdge', the mouse is captured, to move the Shape.
        // The reason for the capture is to receive mouse move events, even when the mouse is outside the application window.
        public void MouseDownShape(MouseButtonEventArgs e)
        {
            if (!isAddingLine) e.MouseDevice.Target.CaptureMouse();
        }
        
        // This is only used for moving a Shape, and only if the mouse is already captured.
        public void MouseMoveShape(MouseEventArgs e)
        {
            // Checks that the mouse is captured and that a line is not being drawn.
            if (Mouse.Captured != null && !isAddingLine)
            {
                // It is now known that the mouse is captured and here the visual element that the mouse is captured by is retrieved.
                FrameworkElement shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
                // From the shapes visual element, the Shape object which is the DataContext is retrieved.
                Shape shapeModel = (Shape)shapeVisualElement.DataContext;
                // The canvas holding the shapes visual element, is found by searching up the tree of visual elements.
                Canvas canvas = FindParentOfType<Canvas>(shapeVisualElement);
                // The mouse position relative to the canvas is gotten here.
                Point mousePosition = Mouse.GetPosition(canvas);
                // When the shape is moved with the mouse, this method is called many times, for each part of the movement.
                // Therefore to only have 1 Undo/Redo command saved for the whole movement, the initial position is saved, 
                //  during the first part of the movement, so that it together with the final position, 
                //  from when the mouse is released, can become one Undo/Redo command.
                if (moveShapePoint == default(Point)) moveShapePoint = mousePosition;
                // The Shape is moved to the position of the mouse in relation to the canvas.
                // The View (GUI) is then notified by the Shape, that its properties have changed.
                shapeModel.CanvasCenterX = (int)mousePosition.X;
                shapeModel.CanvasCenterY = (int)mousePosition.Y;
            }
        }

        // There are two reasons for doing a 'MouseUp'.
        // Either a Line is being drawn, and the second Shape has just been chosen.
        // Or a Shape is being moved and the move is now done.
        public void MouseUpShape(MouseButtonEventArgs e)
        {
            // Used for adding a Line.
            if (isAddingLine)
            {
                // Because a MouseUp event has happened and a Line is currently being drawn, 
                //  the Shape that the Line is drawn from or to has been selected, and is here retrieved from the event parameters.
                FrameworkElement shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
                // From the shapes visual element, the Shape object which is the DataContext is retrieved.
                Shape shape = (Shape)shapeVisualElement.DataContext;
                // This checks if this is the first Shape chosen during the Line adding operation, 
                //  by looking at the addingLineFrom variable, which is empty when no Shapes have previously been choosen.
                // If this is the first Shape choosen, and if so, the Shape is saved in the AddingLineFrom variable.
                //  Also the Shape is set as selected, to make it look different visually.
                if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
                // If this is not the first Shape choosen, and therefore the second, 
                //  it is checked that the first and second Shape are different.
                else if (addingLineFrom.Number != shape.Number)
                {
                    // Now that it has been established that the Line adding operation has been completed succesfully by the user, 
                    //  a Line is added using an 'AddLineCommand', with a new Line given between the two shapes chosen.
                    undoRedoController.AddAndExecute(new AddLineCommand(Lines, new Line() { From = addingLineFrom, To = shape }));
                    // The property used for visually indicating that a Line is being Drawn is cleared, 
                    //  so the View can return to its original and default apperance.
                    addingLineFrom.IsSelected = false;
                    // The 'isAddingLine' and 'addingLineFrom' variables are cleared, 
                    //  so the MainViewModel is ready for another Line adding operation.
                    isAddingLine = false;
                    addingLineFrom = null;
                    // The property used for visually indicating which Shape has already chosen are choosen is cleared, 
                    //  so the View can return to its original and default apperance.
                    RaisePropertyChanged("ModeOpacity");
                }
            }
            // Used for moving a Shape.
            else
            {
                // Here the visual element that the mouse is captured by is retrieved.
                FrameworkElement shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
                // From the shapes visual element, the Shape object which is the DataContext is retrieved.
                Shape shape = (Shape)shapeVisualElement.DataContext;
                // The canvas holding the shapes visual element, is found by searching up the tree of visual elements.
                Canvas canvas = FindParentOfType<Canvas>(shapeVisualElement);
                // The mouse position relative to the canvas is gotten here.
                Point mousePosition = Mouse.GetPosition(canvas);
                // Now that the Move Shape operation is over, the Shape is moved to the final position (coordinates), 
                //  by using a MoveNodeCommand to move it.
                // The MoveNodeCommand is given the original coordinates and with respect to the Undo/Redo functionality, 
                //  the Shape has only been moved once, with this Command.
                undoRedoController.AddAndExecute(new MoveShapeCommand(shape, (int)moveShapePoint.X, (int)moveShapePoint.Y, (int)mousePosition.X, (int)mousePosition.Y));
                // The original Shape point before the move is cleared, so the MainViewModel is ready for the next move operation.
                moveShapePoint = new Point();
                // The mouse is released, as the move operation is done, so it can be used by other controls.
                e.MouseDevice.Target.ReleaseMouseCapture();
            }
        }

        // Recursive method for finding the parent of a visual element of a certain type, 
        //  by searching up the visual tree of parent elements.
        // The '() ? () : ()' syntax, returns the second part if the first part is true, otherwise it returns the third part.
        private static T FindParentOfType<T>(DependencyObject o)
        {
            dynamic parent = VisualTreeHelper.GetParent(o);
            return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
        }
    }
}