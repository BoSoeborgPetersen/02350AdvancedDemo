namespace _02350AdvancedDemo.Model;

[JsonDerivedType(typeof(DashLine))]
public record Line(int FromNumber, int ToNumber, string Label);
