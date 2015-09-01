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
    [XmlInclude(typeof(DashLine))]
    [ImplementPropertyChanged]
    public class Line
    {
        [XmlIgnore]
        public Shape From { get; set; }
        [XmlIgnore]
        public Shape To { get; set; }

        [XmlIgnore]
        public virtual DoubleCollection DashLength => new DoubleCollection() { 1,0 };

        public string Label { get; set; }

        // For Serialization.
        private int fromNumber;
        public int FromNumber { get { return From?.Number ?? fromNumber; } set { fromNumber = value; } }
        private int toNumber;
        public int ToNumber { get { return To?.Number ?? toNumber; } set { toNumber = value; } }
    }
}
