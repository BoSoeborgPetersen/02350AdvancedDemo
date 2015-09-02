using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace _02350AdvancedDemo.Model
{
    [XmlInclude(typeof(Circle))]
    [XmlInclude(typeof(Square))]
    public abstract class Shape
    {
        private static int counter = 0;
        public int Number { get; set; } = ++counter;

        public int X { get; set; } = 200;
        public int Y { get; set; } = 200;
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;

        public List<string> Data { get; set; }

        public override string ToString() => Number.ToString();
    }
}
