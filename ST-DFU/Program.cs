using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System;
using System.Linq;
using nanoFramework.Tools.StDfu;
using System.Text;

namespace Examples
{
    internal class ReadWrite
    {
        //DFU Device in DFU mode
        private const int ProductId = 0xDF11; 
        private const int VendorId = 0x0483;

        private static Dfu dfu = new Dfu(VendorId, ProductId);

        public static void Main(string[] args)
        {
            using (var context = new UsbContext())
            {
                context.SetDebugLevel(LogLevel.Info);

                //Get a list of all connected devices
                var usbDeviceCollection = context.List();

                //Narrow down the device by vendor and pid
                var selectedDevice = usbDeviceCollection.FirstOrDefault(d => d.ProductId == ProductId && d.VendorId == VendorId);

                if (selectedDevice != null)
                {
                    Console.WriteLine("Found USB device in DFU mode...");

                    //Open the device
                    selectedDevice.TryOpen();

                    //Get the first config number of the interface
                    selectedDevice.ClaimInterface(selectedDevice.Configs[0].Interfaces[0].Number);

                    Console.WriteLine(GetDeviceInfo(selectedDevice));

                    dfu.SetUsb(selectedDevice);

                    //do DFU stuff!!!
                    try
                    {
                        dfu.ProgramFirmware(@"F:\nanobooter-nanoclr.dfu");
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine($"Failed!!! - {e}");
                    }
                    

                }
                else
                {
                    Console.WriteLine("No USB device found!");
                }
            }
        }

        private static string GetDeviceInfo(IUsbDevice device)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Model: " + device.Info.Device + "\n");
            //sb.Append("ID: " + device.Info.DeviceId + " (0x" + Integer.toHexString(device.getDeviceId()) + ")" + "\n");
            sb.Append("Class: " + device.Info.DeviceClass + "\n");
            sb.Append("Subclass: " + device.Info.DeviceSubClass + "\n");
            sb.Append("Protocol: " + device.Info.DeviceProtocol + "\n");
            sb.Append("Vendor ID " + device.Info.VendorId + "\n"); //+ " (0x" + Integer.toHexString(device.getVendorId()) + ")" + "\n");
            sb.Append("Product ID: " + device.Info.ProductId + "\n"); //+ " (0x" + Integer.toHexString(device.getProductId()) + ")" + "\n");
            //sb.Append("Device Ver: 0x" + Integer.toHexString(mDeviceVersion) + "\n");
            //sb.Append("Interface count: " + device.getInterfaceCount() + "\n");

            //for (int i = 0; i < device.in.getInterfaceCount(); i++)
            //{
            //    UsbInterface usbInterface = device.getInterface(i);
            //    sb.Append("Interface: " + usbInterface.toString() + "\n");
            //    sb.Append("Endpoint Count: " + usbInterface.getEndpointCount() + "\n");

            //    for (int j = 0; j < usbInterface.getEndpointCount(); j++)
            //    {
            //        UsbEndpoint ep = usbInterface.getEndpoint(j);
            //        sb.Append("Endpoint: " + ep.toString() + "\n");
            //    }
            //}

            return sb.ToString();
        }
    }
}