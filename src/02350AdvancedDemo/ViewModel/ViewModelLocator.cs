using CommonServiceLocator;
using System;
using Unity;
using Unity.ServiceLocation;

namespace _02350AdvancedDemo.ViewModel
{
    public class ViewModelLocator
    {

        public MainViewModel Main => new();
    }
}
