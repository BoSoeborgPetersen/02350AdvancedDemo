namespace _02350AdvancedDemo.Service;

public class StateService(Repository repo)
{
    Type addingLineType;
    public Type AddingLineType { get { return addingLineType; } set { addingLineType = value; WeakReferenceMessenger.Default.Send<IsAddingLineMessage>(); } }
    public bool IsAddingLine => AddingLineType != null;

    public ObservableCollection<ShapeViewModel> Shapes { get; set; } = [];
    public ObservableCollection<LineViewModel> Lines { get; set; } = [];

    public void Init()
    {
        var (shapes, lines) = repo.Read();

        foreach (var shape in shapes) Shapes.Add(shape);
        foreach (var line in lines) Lines.Add(line);
    }
}
