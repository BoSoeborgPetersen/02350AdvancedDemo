namespace _02350AdvancedDemo.ViewModel;

public static class Extensions
{
    public static int ParseOr(this string s, int alternative) => int.TryParse(s, out var i) ? i : alternative;
}
