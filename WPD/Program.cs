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

            //loop through all devices connected
            foreach (var device in collection)
            {
                device.Connect();//first we need to connect
                Console.WriteLine("Device Name: " + device.FriendlyName);

                var folder = device.GetContents();//getting COM exception thrown

                foreach(var item in folder.Files)
                {

                    Console.WriteLine(item.Name);
                    DisplayObject(item, device);
                    if ( item is PortableDeviceFile )
                    {
                        Console.WriteLine("Copying file " + item.Name);
                        device.DownloadFile((PortableDeviceFile)item, @"C:\Users\Q12710\Documents");
                    }
                }
                device.Disconnect();
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to end.");
            Console.ReadKey();
        }

        public static void DisplayObject(PortableDeviceObject portableDeviceObject, PortableDevice device)
        {
            Console.WriteLine(portableDeviceObject.Name);
            if (portableDeviceObject is PortableDeviceFolder)
            {
                Console.WriteLine("Folder");
                DisplayFolderContents((PortableDeviceFolder)portableDeviceObject, device);
            }

        }

        //allows for recursion through folder structure
        public static void DisplayFolderContents(PortableDeviceFolder folder, PortableDevice device)
        {
            foreach(var item in folder.Files)
            {
                Console.WriteLine(item.Id);
                if (item is PortableDeviceFolder)
                {
                    DisplayFolderContents((PortableDeviceFolder)item, device);
                }

                if (item is PortableDeviceFile && item.Id.Contains(".mp4"))
                {
                    Console.WriteLine("Copying video file...");
                    device.DownloadFile((PortableDeviceFile)item, @"D:\Users\Duncan\Videos\Hitachi Videos");
                }
            }
        }
    }
}
