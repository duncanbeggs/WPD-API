using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PortableDeviceApiLib;//provides access to the type PortableDeviceClass
using PortableDeviceTypesLib;//provides the PortableDeviceValuesClass();
using System.Threading.Tasks;

namespace WPD
{
    //Think of this class as a wrapper for each WPD-compatible device. We pass in the
    //device's ID when creating a new instance of this class.
    public class PortableDevice
    {
        private bool _isConnected;
        private readonly PortableDeviceClass _device;

        public string DeviceId { get; set; }


       

        public PortableDevice(string deviceId)
        {
            //when an object of this class (PortableDevice) is instantiated it creates an instance of the PortableDeviceClass type
            this._device = new PortableDeviceClass();
            this.DeviceId = deviceId;
        }

        public void Connect()
        {
            if (this._isConnected) { return; }

            //the PortableDeviceValuesClass lets you specify information about the client application
            var clientInfo = (PortableDeviceApiLib.IPortableDeviceValues) new PortableDeviceValuesClass();//DOUBLE-CHECK: make sure that the PortableDeviceApiLib here
            this._device.Open(this.DeviceId, clientInfo);//passing in clientinfo is necessary though so far we aren't sending anything
            this._isConnected = true;
        }

        public void Disconnect()
        {
            if (!this._isConnected) { return; }

            this._device.Close();
            this._isConnected = false;
        }

        //property (accessor) to get/set a friendly name (a name that isn't a bunch of incomprehensible characters.
        public string FriendlyName
        {
            get
            {
                if (!this._isConnected)
                {
                    throw new InvalidOperationException("Not Connected to Device");
                }

                //Retrieve the properties of the device
                IPortableDeviceContent content;
                IPortableDeviceProperties properties;
                this._device.Content(out content);
                content.Properties(out properties);

                //Retrieve the values for the properties
                PortableDeviceApiLib.IPortableDeviceValues propertyValues;//DOUBLE-CHECK: make sure that the PortableDeviceApiLib here is correct
                properties.GetValues("DEVICE", null, out propertyValues);

                //Identify the property to retrieve
                var property = new PortableDeviceApiLib._tagpropertykey();//DOUBLE-CHECK: make sure that the PortableDeviceApiLib here is correct
                property.fmtid = new Guid(0x26D4979A, 0xE643, 0x4626, 0x9E,
                    0x2B, 0x73, 0x6D, 0xC0, 0xC9, 0x2F, 0xDC);//hmm a bunch of addresses?

                property.pid = 12;

                //retrieve the friendly name
                string propertyValue;
                propertyValues.GetStringValue(ref property, out propertyValue);

                return propertyValue;
            }
        }

        //public method
        public PortableDeviceFolder GetContents()
        {
            var root = new PortableDeviceFolder("DEVICE", "DEVICE");//

            IPortableDeviceContent content;
            this._device.Content(out content);
            EnumerateContents(ref content, root);

            return root;
        }

        //this private method is used by the public method GetContents()
        //it enumerates the contents of the current folder (parent) and includes the necessary
        //recursion to parse the contents of any sub-folders. When the GetContents(...) method completes
        //an entire tree structure (root var) has been built that represents the contents of the device.
        private static void EnumerateContents(ref IPortableDeviceContent content, PortableDeviceFolder parent)
        {
            //Get the properties of the object
            IPortableDeviceProperties properties;
            content.Properties(out properties);

            //enumerate the items contained by the current object
            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, parent.Id, null, out objectIds);

            uint fetched = 0;
            do
            {
                string objectId;

                objectIds.Next(1, out objectId, ref fetched);
                if (fetched > 0)
                {
                    var currentObject = WrapObject(properties, objectId);//references custom private method WrapObject( )

                    parent.Files.Add(currentObject);

                    if (currentObject is PortableDeviceFolder)
                    {
                        EnumerateContents(ref content, (PortableDeviceFolder)currentObject);
                    }

                }
            } while (fetched > 0);
        }

        //The WrapObject( ) method creates an instance of the PortableDeviceFolder or PortableDeviceFile class types depending on the
        //type of the object. For each folder or file it extracts the name and type type (folder or file)
        private static PortableDeviceObject WrapObject(IPortableDeviceProperties properties, string objectId)
        {
            PortableDeviceApiLib.IPortableDeviceKeyCollection keys;//DOUBLE-CHECK: make sure that the PortableDeviceApiLib here is correct, not the other lib
            properties.GetSupportedProperties(objectId, out keys);
            
            PortableDeviceApiLib.IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            //Get the name of the object
            string name;
            var property = new PortableDeviceApiLib._tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);//??
            property.pid = 4;
            values.GetStringValue(property, out name);

            //Get the type of the object
            Guid contentType;
            property = new PortableDeviceApiLib._tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 7;
            values.GetGuidValue(property, out contentType);

            //Guid - globally unique identifier 
            var folderType = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C, 0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
            var functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98, 0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);

            if (contentType == folderType || contentType == functionalType)
            {
                return new PortableDeviceFolder(objectId, name);
            }

            return new PortableDeviceFile(objectId, name);
        }
    }
}
