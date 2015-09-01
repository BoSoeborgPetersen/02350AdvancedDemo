using _02350AdvancedDemo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _02350AdvancedDemo.Serialization
{
    public class SerializerXML
    {
        public static SerializerXML Instance { get; } = new SerializerXML();

        private SerializerXML() { }

        public void Serialize(Diagram diagram, string path)
        {
            using(FileStream stream = File.Create(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Diagram));
                serializer.Serialize(stream, diagram);
            }
        }

        public Diagram Deserialize(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Diagram));
                Diagram diagram = serializer.Deserialize(stream) as Diagram;

                // Reconstruct object graph.
                diagram.Lines.ForEach(x => x.From = diagram.Shapes.Single(y => y.Number == x.FromNumber));
                diagram.Lines.ForEach(x => x.To = diagram.Shapes.Single(y => y.Number == x.ToNumber));

                return diagram;
            }
        }
    }
}
