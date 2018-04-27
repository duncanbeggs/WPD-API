using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortableDeviceApiLib;
using PortableDeviceTypesLib;

namespace WPD
{
    //this class represents a simple collection which keeps track of PortableDevice instances. 
    //When an instance of the PortableDeviceCollection class is created its constructor creates an 
    //instance of the PortableDeviceManager type which can be found in the PortableDeviceApi 1.0 Type lib
    public class PortableDeviceCollection : Collection<PortableDevice>
    {
        //found in portable device api 1.0 type library
        private readonly PortableDeviceManager _deviceManager;

        //contructor
        public PortableDeviceCollection()
        {
            this._deviceManager = new PortableDeviceManager();
        }

        //Builds a collection of WPD-compatible devices
        public void Refresh()
        {
            this._deviceManager.RefreshDeviceList();

            //determine how many WPD devices are connected
            var deviceIds = new string[1];
            uint count = 1;
            this._deviceManager.GetDevices(ref deviceIds[0], ref count);

            //Retrieve the device id for each connected device
            deviceIds = new string[count];
            try
            {
                this._deviceManager.GetDevices(ref deviceIds[0], ref count);
            } catch (System.IndexOutOfRangeException e1)
            {
                Console.WriteLine("Error: " + e1 + " \nHint: Maybe no devices are connected?");
            }
            foreach (var deviceId in deviceIds)
            {
                Add(new PortableDevice(deviceId));
            }
        }
    }
}
