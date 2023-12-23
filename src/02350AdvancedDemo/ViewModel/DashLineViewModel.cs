namespace _02350AdvancedDemo.ViewModel;

public class DashLineViewModel() : LineViewModel()
{
    public override DoubleCollection DashLength => new() { 2 };
}
