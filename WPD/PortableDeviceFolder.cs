using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPD
{
    //the PortableDeviceObject is a class from which we inherit a bunch of code that is common to the various different objects
    //folder has both a name and an ID
    public class PortableDeviceFolder : PortableDeviceObject
    {
        public PortableDeviceFolder(string id, string name) : base(id, name)
        {
            this.Files = new List<PortableDeviceObject>();// a list of files/folders(?)
        }

        public IList<PortableDeviceObject> Files { get; set; } 
    }
}
