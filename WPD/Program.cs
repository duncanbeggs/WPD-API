using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPD
{
    class Program
    {
        static void Main(string[] args)
        {
            var collection = new PortableDeviceCollection();

            collection.Refresh();

            foreach (var device in collection)
            {
                device.Connect();//first we need to connect
                Console.WriteLine("Device Name:  " + device.FriendlyName);

                var folder = device.GetContents();//getting COM exception thrown
                foreach(var item in folder.Files)
                {
                    DisplayObject(item);
                }
                device.Disconnect();
            }

            Console.WriteLine();
            Console.WriteLine("Key yo");
            Console.ReadKey();
        }

        public static void DisplayObject(PortableDeviceObject portableDeviceObject)
        {
            Console.WriteLine(portableDeviceObject.Name);
            if (portableDeviceObject is PortableDeviceFolder)
            {
                DisplayFolderContents((PortableDeviceFolder)portableDeviceObject);
            }
        }

        public static void DisplayFolderContents(PortableDeviceFolder folder)
        {
            foreach(var item in folder.Files)
            {
                Console.WriteLine(item.Id);
                if (item is PortableDeviceFolder)
                {
                    DisplayFolderContents((PortableDeviceFolder)item);
                }
            }
        }
    }
}
