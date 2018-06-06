using System;
using System.IO;
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
            //get the target directory, the destination to copy the video to
            string destPath = getFileDestinationPath();
            Console.WriteLine(destPath);

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
                    DisplayObject(item, device, destPath);
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

        public static string getFileDestinationPath()
        {
            string line;
            try
            {
                StreamReader sr = new StreamReader("destPath.txt");
                line = sr.ReadLine();
                return line;
            } catch
            {
                Console.WriteLine("ERROR: could not find file destPath.txt to grab video destination path");
                return null;
            }
            
        }

        public static void DisplayObject(PortableDeviceObject portableDeviceObject, PortableDevice device, string destPath)
        {
            Console.WriteLine(portableDeviceObject.Name);
            if (portableDeviceObject is PortableDeviceFolder)
            {
                Console.WriteLine("Folder");
                DisplayFolderContents((PortableDeviceFolder)portableDeviceObject, device, destPath);
            }

        }

        //allows for recursion through folder structure
        public static void DisplayFolderContents(PortableDeviceFolder folder, PortableDevice device, string destPath)
        {
            foreach(var item in folder.Files)
            {
                Console.WriteLine(item.Id);
                if (item is PortableDeviceFolder)
                {
                    DisplayFolderContents((PortableDeviceFolder)item, device, destPath);
                }

                if (item is PortableDeviceFile && item.Id.Contains(".mp4"))
                {
                    Console.WriteLine("Copying video file...");
                    device.DownloadFile((PortableDeviceFile)item, destPath);
                }
            }
        }
    }
}
