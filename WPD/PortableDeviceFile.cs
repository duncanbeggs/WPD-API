using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPD
{
    //file has both a name and an ID
    public class PortableDeviceFile : PortableDeviceObject
    {
        public PortableDeviceFile(string id, string name) : base(id, name)
        {
            //no code needed. most important stuff is in the PortableDeviceObject from which this inherits
        }
    }
}
