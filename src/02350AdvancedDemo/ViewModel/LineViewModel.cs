namespace _02350AdvancedDemo.ViewModel;

public partial class LineViewModel : ObservableObject
{
    [ObservableProperty]
    ShapeViewModel from;
    [ObservableProperty]
    ShapeViewModel to;
    [ObservableProperty]
    string label;
    public virtual DoubleCollection DashLength => [1, 0];
}
