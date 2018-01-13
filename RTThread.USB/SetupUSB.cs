using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static RTThread.USB.SetupAPI;

namespace RTThread.USB
{
    public class SetupUSB
    {
        public static List<USBDevice> GetDeviceByGuid(Guid guid)
        {
            List<USBDevice> deviceList = new List<USBDevice>();
            IntPtr info = GetClassDevOfHandle(guid);
            if (info == IntPtr.Zero)
            {
                return deviceList;
            }
            uint index = 0;
            SP_DEVICE_INTERFACE_DATA ifData = new SP_DEVICE_INTERFACE_DATA();
            ifData.cbSize = Marshal.SizeOf(ifData);

            for (index = 0; GetEnumDeviceInterfaces(info, ref guid, index, ref ifData); ++index)
            {
                uint needed = 0;
                GetDeviceInterfaceDetail(info, ref ifData, IntPtr.Zero, 0, ref needed, null);
                IntPtr pDetail = Marshal.AllocHGlobal((int)needed);
                SP_DEVICE_INTERFACE_DETAIL_DATA detail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                detail.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
                Marshal.StructureToPtr(detail, pDetail, false);
                SP_DEVINFO_DATA did = new SP_DEVINFO_DATA();
                USBDevice device = null;
                if (GetDeviceInterfaceDetail(info, ref ifData, pDetail, needed, ref needed, did))
                {
                    device = new USBDevice((Marshal.PtrToStringAuto((IntPtr)((int)pDetail + 4))));
                }
                StringBuilder Product = new StringBuilder();
                uint reqsize = 0;
                if (GetDeviceRegistryProperty(info, did, SPDRP_DEVICEDESC, 0, Product, 253, ref reqsize))
                {
                    device.Name = Product.ToString();
                }
                else
                {
                    device.Name = ("(Unnamed device)");
                }
                Marshal.FreeHGlobal(pDetail);
                deviceList.Add(device);
            }
            DestroyDeviceInfoList(info);

            return deviceList;
        }
    }
}
