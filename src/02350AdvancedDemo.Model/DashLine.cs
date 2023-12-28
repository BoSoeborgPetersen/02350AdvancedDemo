namespace _02350AdvancedDemo.Model;

public record DashLine(int FromNumber, int ToNumber, string Label) : Line(FromNumber, ToNumber, Label);
