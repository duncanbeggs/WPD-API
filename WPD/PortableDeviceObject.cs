using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPD
{
    //abstract means that this isn't actually implemented or instantiated but provides code to 
    //any classes that derive from this class. The word "abstract" makes intuitive sense in this case.
    public abstract class PortableDeviceObject
    {
        protected PortableDeviceObject(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }
    }
}
