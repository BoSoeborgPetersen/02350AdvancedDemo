namespace _02350AdvancedDemo.Serialization;

public static class SerializerXML
{
    public static async void AsyncSerializeToFile(Diagram diagram, string path) => 
        await Task.Run(() => SerializeToFile(diagram, path));

    static void SerializeToFile(Diagram diagram, string path)
    {
        using FileStream stream = File.Create(path);
        XmlSerializer serializer = new(typeof(Diagram));
        serializer.Serialize(stream, diagram);
    }

    public static Task<Diagram> AsyncDeserializeFromFile(string path) => 
        Task.Run(() => DeserializeFromFile(path));

    static Diagram DeserializeFromFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        XmlSerializer serializer = new(typeof(Diagram));
        return serializer.Deserialize(stream) as Diagram;
    }

    public static Task<string> AsyncSerializeToString(Diagram diagram) => 
        Task.Run(() => SerializeToString(diagram));

    static string SerializeToString(Diagram diagram)
    {
        var stringBuilder = new StringBuilder();

        using TextWriter stream = new StringWriter(stringBuilder);
        XmlSerializer serializer = new(typeof(Diagram));
        serializer.Serialize(stream, diagram);

        return stringBuilder.ToString();
    }

    public static Task<Diagram> AsyncDeserializeFromString(string xml) => 
        Task.Run(() => DeserializeFromString(xml));

    static Diagram DeserializeFromString(string xml)
    {
        using TextReader stream = new StringReader(xml);
        XmlSerializer serializer = new(typeof(Diagram));
        return serializer.Deserialize(stream) as Diagram;
    }
}
