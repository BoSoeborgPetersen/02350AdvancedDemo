namespace _02350AdvancedDemo.Data;

public class Repository // TODO: Maybe add database
{
    public (List<ShapeViewModel>, List<LineViewModel>) Read()
    {
        List<ShapeViewModel> shapes = [
            new CircleViewModel() { Position = new(30, 40), Size = new(80, 80), Data = ["text1", "text2", "text3"] },
            new SquareViewModel() { Position = new(140, 230), Size = new(200, 100), Data = ["text1", "text2", "text3"] }
            ];

        List<LineViewModel> lines = [new LineViewModel() { From = shapes[0], To = shapes[1], Label = "Line Text" }];

        return (shapes, lines);
    }
}
