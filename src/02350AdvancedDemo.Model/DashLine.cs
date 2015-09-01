using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace _02350AdvancedDemo.Model
{
    public class DashLine : Line
    {
        [XmlIgnore]
        public override DoubleCollection DashLength => new DoubleCollection() { 2 };
    }
}
