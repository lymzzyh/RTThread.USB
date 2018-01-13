using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RTThread.USB
{
    internal static class SetupAPI
    {
        /// SPDRP_DEVICEDESC -> (0x00000000)
        public const int SPDRP_DEVICEDESC = 0;

        /// SPDRP_HARDWAREID -> (0x00000001)
        public const int SPDRP_HARDWAREID = 1;

        /// SPDRP_COMPATIBLEIDS -> (0x00000002)
        public const int SPDRP_COMPATIBLEIDS = 2;

        /// SPDRP_NTDEVICEPATHS -> (0x00000003)
        public const int SPDRP_NTDEVICEPATHS = 3;

        /// SPDRP_SERVICE -> (0x00000004)
        public const int SPDRP_SERVICE = 4;

        /// SPDRP_CONFIGURATION -> (0x00000005)
        public const int SPDRP_CONFIGURATION = 5;

        /// SPDRP_CONFIGURATIONVECTOR -> (0x00000006)
        public const int SPDRP_CONFIGURATIONVECTOR = 6;

        /// SPDRP_CLASS -> (0x00000007)
        public const int SPDRP_CLASS = 7;

        /// SPDRP_CLASSGUID -> (0x00000008)
        public const int SPDRP_CLASSGUID = 8;

        /// SPDRP_DRIVER -> (0x00000009)
        public const int SPDRP_DRIVER = 9;

        /// SPDRP_CONFIGFLAGS -> (0x0000000A)
        public const int SPDRP_CONFIGFLAGS = 10;

        /// SPDRP_MFG -> (0x0000000B)
        public const int SPDRP_MFG = 11;

        /// SPDRP_FRIENDLYNAME -> (0x0000000C)
        public const int SPDRP_FRIENDLYNAME = 12;

        /// SPDRP_LOCATION_INFORMATION -> (0x0000000D)
        public const int SPDRP_LOCATION_INFORMATION = 13;

        /// SPDRP_PHYSICAL_DEVICE_OBJECT_NAME -> (0x0000000E)
        public const int SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 14;

        /// SPDRP_CAPABILITIES -> (0x0000000F)
        public const int SPDRP_CAPABILITIES = 15;

        /// SPDRP_UI_NUMBER -> (0x00000010)
        public const int SPDRP_UI_NUMBER = 16;

        /// SPDRP_UPPERFILTERS -> (0x00000011)
        public const int SPDRP_UPPERFILTERS = 17;

        /// SPDRP_LOWERFILTERS -> (0x00000012)
        public const int SPDRP_LOWERFILTERS = 18;

        /// SPDRP_MAXIMUM_PROPERTY -> (0x00000013)
        public const int SPDRP_MAXIMUM_PROPERTY = 19;

        public enum DIGCF
        {
            DIGCF_DEFAULT = 0x00000001, // only valid with DIGCF_DEVICEINTERFACE                 
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010
        }
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid interfaceClassGuid;
            public int flags;
            public int reserved;
        }
        [StructLayout(LayoutKind.Sequential)]
        public class SP_DEVINFO_DATA
        {
            public int cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
            public Guid classGuid = Guid.Empty; 
            public int devInst = 0; 
            public int reserved = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            internal int cbSize;
            internal short devicePath;
        }
        private static class NativeMethods
        {

            [DllImport("setupapi.dll", SetLastError = true)]
            public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator, IntPtr HwndParent, DIGCF Flags);
            [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

            [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, ref uint requiredSize, SP_DEVINFO_DATA deviceInfoData);
            [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern Boolean SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);
            [DllImport("setupapi.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr lpInfoSet, SP_DEVINFO_DATA DeviceInfoData, UInt32 Property, UInt32 PropertyRegDataType, StringBuilder PropertyBuffer, UInt32 PropertyBufferSize, ref uint RequiredSize);


        }
        public static IntPtr GetClassDevOfHandle(Guid guid)
        {
            return NativeMethods.SetupDiGetClassDevs(ref guid, 0, IntPtr.Zero, DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE);
        }

        public static bool GetEnumDeviceInterfaces(IntPtr InfoSet, ref Guid guid, uint index, ref SP_DEVICE_INTERFACE_DATA interfaceInfo)
        {
            return NativeMethods.SetupDiEnumDeviceInterfaces(InfoSet, IntPtr.Zero, ref guid, index, ref interfaceInfo);
        }

        public static bool GetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, uint deviceInterfaceDetailDataSize, ref uint requiredSize, SP_DEVINFO_DATA deviceInfoData)
        {
            return NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, deviceInterfaceDetailData, deviceInterfaceDetailDataSize, ref requiredSize, deviceInfoData);
        }

        public static void DestroyDeviceInfoList(IntPtr InfoSet)
        {
            NativeMethods.SetupDiDestroyDeviceInfoList(InfoSet);
        }
        public static bool GetDeviceRegistryProperty(IntPtr lpInfoSet, SP_DEVINFO_DATA DeviceInfoData, UInt32 Property, UInt32 PropertyRegDataType, StringBuilder PropertyBuffer, UInt32 PropertyBufferSize, ref uint RequiredSize)
        {
            return NativeMethods.SetupDiGetDeviceRegistryProperty(lpInfoSet, DeviceInfoData, Property, PropertyRegDataType, PropertyBuffer, PropertyBufferSize, ref RequiredSize);
        }
    }
}
