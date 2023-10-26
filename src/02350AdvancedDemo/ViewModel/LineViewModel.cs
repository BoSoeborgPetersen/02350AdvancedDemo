namespace _02350AdvancedDemo.ViewModel;

public class LineViewModel : BaseViewModel
{
    private ShapeViewModel from;
    private ShapeViewModel to;
    public Line Line { get; set; }
    public ShapeViewModel From { get { return from; } set { from = value; Line.FromNumber = value?.Number ?? 0; OnPropertyChanged(); } }
    public ShapeViewModel To { get { return to; } set { to = value; Line.ToNumber = value?.Number ?? 0; OnPropertyChanged(); } }
    public string Label { get { return Line.Label; } set { Line.Label = value; OnPropertyChanged(); } }
    public DoubleCollection DashLength => Line is DashLine ? new DoubleCollection() { 2 } : new DoubleCollection() { 1, 0 };

    public LineViewModel(Line _line) : base()
    {
        Line = _line;
    }
}
