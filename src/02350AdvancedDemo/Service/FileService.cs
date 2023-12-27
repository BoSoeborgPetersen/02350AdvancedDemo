namespace _02350AdvancedDemo.Service;

public static class FileService
{
    public static void Load(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        string path = Dialogs.ShowOpen();
        if (path != null)
        {
            var diagram = JsonSerializer.Deserialize<Diagram>(File.ReadAllText(path));
            var (loadedShapes, loadedLines) = MappingService.Unmap(diagram);

            shapes.Clear();
            foreach (var shape in loadedShapes) shapes.Add(shape);
            lines.Clear();
            foreach (var line in loadedLines) lines.Add(line);
        }
    }

    public static void Save(ObservableCollection<ShapeViewModel> shapes, ObservableCollection<LineViewModel> lines)
    {
        string path = Dialogs.ShowSave();
        if (path != null)
        {
            Diagram diagram = MappingService.Map(shapes, lines);
            File.WriteAllText(path, JsonSerializer.Serialize(diagram));
        }
    }
}
