using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.Model
{
    // This is an abstract base class that is used to define INotifyPropertyChanged functionality used by all Model classes, so they do not have to.
    // The purpose of the INotifyPropertyChanged interface is to inform the View (GUI) that a property of a bound object has changed, so it can update the corresponding graphical representation.
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        // This is the event that is raised when the INotifyPropertyChanged interface is used to let the View (GUI) know that a property of a bound object has changed.
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is used by inheriting classes to raise the INotifyPropertyChanged event.
        // It must be called in all set methods that change the state of model objects, to be sure that the view (GUI) is always updated, when data is changed behind the scenes.
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (propertyName != null && PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
