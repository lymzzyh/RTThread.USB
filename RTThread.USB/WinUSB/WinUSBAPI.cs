using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace RTThread.USB.WinUSB
{
    internal class WinUSBAPI
    {
        public enum DeviceSpeend
        {
            FullSpeed = 0x01,
            HighSpeed = 0x03,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_INTERFACE_DESCRIPTOR
        {

            /// UCHAR->unsigned char
            public byte bLength;

            /// UCHAR->unsigned char
            public byte bDescriptorType;

            /// UCHAR->unsigned char
            public byte bInterfaceNumber;

            /// UCHAR->unsigned char
            public byte bAlternateSetting;

            /// UCHAR->unsigned char
            public byte bNumEndpoints;

            /// UCHAR->unsigned char
            public byte bInterfaceClass;

            /// UCHAR->unsigned char
            public byte bInterfaceSubClass;

            /// UCHAR->unsigned char
            public byte bInterfaceProtocol;

            /// UCHAR->unsigned char
            public byte iInterface;
        }

        public enum USBD_PIPE_TYPE
        {

            /// UsbdPipeTypeControl -> 0
            UsbdPipeTypeControl = 0,

            /// UsbdPipeTypeIsochronous -> 1
            UsbdPipeTypeIsochronous = 1,

            /// UsbdPipeTypeBulk -> 2
            UsbdPipeTypeBulk = 2,

            /// UsbdPipeTypeInterrupt -> 3
            UsbdPipeTypeInterrupt = 3,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINUSB_PIPE_INFORMATION
        {

            /// USBD_PIPE_TYPE->_USBD_PIPE_TYPE
            public USBD_PIPE_TYPE PipeType;

            /// UCHAR->unsigned char
            public byte PipeId;

            /// USHORT->unsigned short
            public ushort MaximumPacketSize;

            /// UCHAR->unsigned char
            public byte Interval;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WINUSB_SETUP_PACKET
        {

            /// UCHAR->unsigned char
            public byte RequestType;

            /// UCHAR->unsigned char
            public byte Request;

            /// USHORT->unsigned short
            public ushort Value;

            /// USHORT->unsigned short
            public ushort Index;

            /// USHORT->unsigned short
            public ushort Length;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_CONFIGURATION_DESCRIPTOR
        {

            /// UCHAR->unsigned char
            public byte bLength;

            /// UCHAR->unsigned char
            public byte bDescriptorType;

            /// USHORT->unsigned short
            public ushort wTotalLength;

            /// UCHAR->unsigned char
            public byte bNumInterfaces;

            /// UCHAR->unsigned char
            public byte bConfigurationValue;

            /// UCHAR->unsigned char
            public byte iConfiguration;

            /// UCHAR->unsigned char
            public byte bmAttributes;

            /// UCHAR->unsigned char
            public byte MaxPower;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct USB_COMMON_DESCRIPTOR
        {

            /// UCHAR->unsigned char
            public byte bLength;

            /// UCHAR->unsigned char
            public byte bDescriptorType;
        }

        private static class NativeMethods
        {
            /// Return Type: BOOL->boolean
            ///DeviceHandle: HANDLE->void*
            ///InterfaceHandle: PWINUSB_INTERFACE_HANDLE->void**
            [DllImport("winusb.dll", EntryPoint = "WinUsb_Initialize")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_Initialize(IntPtr DeviceHandle, ref IntPtr InterfaceHandle);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_Free")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_Free(IntPtr InterfaceHandle);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///AssociatedInterfaceIndex: UCHAR->unsigned char
            ///AssociatedInterfaceHandle: PWINUSB_INTERFACE_HANDLE->void**
            [DllImport("winusb.dll", EntryPoint = "WinUsb_GetAssociatedInterface")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_GetAssociatedInterface(IntPtr InterfaceHandle, byte AssociatedInterfaceIndex, ref IntPtr AssociatedInterfaceHandle);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///DescriptorType: UCHAR->unsigned char
            ///Index: UCHAR->unsigned char
            ///LanguageID: USHORT->unsigned short
            ///Buffer: PUCHAR->unsigned char*
            ///BufferLength: ULONG->unsigned int
            ///LengthTransferred: PULONG->unsigned int*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_GetDescriptor")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///AlternateInterfaceNumber: UCHAR->unsigned char
            ///UsbAltInterfaceDescriptor: PUSB_INTERFACE_DESCRIPTOR->_USB_INTERFACE_DESCRIPTOR*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_QueryInterfaceSettings")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_QueryInterfaceSettings(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, ref USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///InformationType: ULONG->unsigned int
            ///BufferLength: PULONG->unsigned int*
            ///Buffer: PVOID->void*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_QueryDeviceInformation")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_QueryDeviceInformation(IntPtr InterfaceHandle, uint InformationType, ref uint BufferLength, IntPtr Buffer);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///SettingNumber: UCHAR->unsigned char
            [DllImport("winusb.dll", EntryPoint = "WinUsb_SetCurrentAlternateSetting")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_SetCurrentAlternateSetting(IntPtr InterfaceHandle, byte SettingNumber);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///SettingNumber: PUCHAR->unsigned char*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_GetCurrentAlternateSetting")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_GetCurrentAlternateSetting(IntPtr InterfaceHandle, IntPtr SettingNumber);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///AlternateInterfaceNumber: UCHAR->unsigned char
            ///PipeIndex: UCHAR->unsigned char
            ///PipeInformation: PWINUSB_PIPE_INFORMATION->_WINUSB_PIPE_INFORMATION*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_QueryPipe")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_QueryPipe(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, byte PipeIndex, ref WINUSB_PIPE_INFORMATION PipeInformation);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PipeID: UCHAR->unsigned char
            ///PolicyType: ULONG->unsigned int
            ///ValueLength: ULONG->unsigned int
            ///Value: PVOID->void*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_SetPipePolicy")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_SetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, uint ValueLength, IntPtr Value);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PipeID: UCHAR->unsigned char
            ///PolicyType: ULONG->unsigned int
            ///ValueLength: PULONG->unsigned int*
            ///Value: PVOID->void*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_GetPipePolicy")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_GetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, ref uint ValueLength, IntPtr Value);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PipeID: UCHAR->unsigned char
            ///Buffer: PUCHAR->unsigned char*
            ///BufferLength: ULONG->unsigned int
            ///LengthTransferred: PULONG->unsigned int*
            ///Overlapped: LPOVERLAPPED->_OVERLAPPED*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_ReadPipe")]
            [return: MarshalAs(UnmanagedType.Bool)]
            unsafe public static extern bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred, NativeOverlapped* Overlapped);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PipeID: UCHAR->unsigned char
            ///Buffer: PUCHAR->unsigned char*
            ///BufferLength: ULONG->unsigned int
            ///LengthTransferred: PULONG->unsigned int*
            ///Overlapped: LPOVERLAPPED->_OVERLAPPED*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_WritePipe")]
            [return: MarshalAs(UnmanagedType.Bool)]
            unsafe public static extern bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred, NativeOverlapped* Overlapped);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///SetupPacket: WINUSB_SETUP_PACKET->_WINUSB_SETUP_PACKET
            ///Buffer: PUCHAR->unsigned char*
            ///BufferLength: ULONG->unsigned int
            ///LengthTransferred: PULONG->unsigned int*
            ///Overlapped: LPOVERLAPPED->_OVERLAPPED*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_ControlTransfer")]
            [return: MarshalAs(UnmanagedType.Bool)]
            unsafe public static extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred, NativeOverlapped* Overlapped);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PipeID: UCHAR->unsigned char
            [DllImport("winusb.dll", EntryPoint = "WinUsb_ResetPipe")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_ResetPipe(IntPtr InterfaceHandle, byte PipeID);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PipeID: UCHAR->unsigned char
            [DllImport("winusb.dll", EntryPoint = "WinUsb_AbortPipe")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_AbortPipe(IntPtr InterfaceHandle, byte PipeID);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PipeID: UCHAR->unsigned char
            [DllImport("winusb.dll", EntryPoint = "WinUsb_FlushPipe")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_FlushPipe(IntPtr InterfaceHandle, byte PipeID);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PolicyType: ULONG->unsigned int
            ///ValueLength: ULONG->unsigned int
            ///Value: PVOID->void*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_SetPowerPolicy")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_SetPowerPolicy(IntPtr InterfaceHandle, uint PolicyType, uint ValueLength, IntPtr Value);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///PolicyType: ULONG->unsigned int
            ///ValueLength: PULONG->unsigned int*
            ///Value: PVOID->void*
            [DllImport("winusb.dll", EntryPoint = "WinUsb_GetPowerPolicy")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool WinUsb_GetPowerPolicy(IntPtr InterfaceHandle, uint PolicyType, ref uint ValueLength, IntPtr Value);


            /// Return Type: BOOL->boolean
            ///InterfaceHandle: WINUSB_INTERFACE_HANDLE->void*
            ///lpOverlapped: LPOVERLAPPED->_OVERLAPPED*
            ///lpNumberOfBytesTransferred: LPDWORD->unsigned int*
            ///bWait: BOOL->boolean
            [DllImport("winusb.dll", EntryPoint = "WinUsb_GetOverlappedResult")]
            [return: MarshalAs(UnmanagedType.Bool)]
            unsafe public static extern bool WinUsb_GetOverlappedResult(IntPtr InterfaceHandle, NativeOverlapped* lpOverlapped, ref uint lpNumberOfBytesTransferred, [MarshalAs(UnmanagedType.Bool)] bool bWait);


            /// Return Type: PUSB_INTERFACE_DESCRIPTOR->_USB_INTERFACE_DESCRIPTOR*
            ///ConfigurationDescriptor: PUSB_CONFIGURATION_DESCRIPTOR->_USB_CONFIGURATION_DESCRIPTOR*
            ///StartPosition: PVOID->void*
            ///InterfaceNumber: LONG->int
            ///AlternateSetting: LONG->int
            ///InterfaceClass: LONG->int
            ///InterfaceSubClass: LONG->int
            ///InterfaceProtocol: LONG->int
            [DllImport("winusb.dll", EntryPoint = "WinUsb_ParseConfigurationDescriptor")]
            public static extern IntPtr WinUsb_ParseConfigurationDescriptor(ref USB_CONFIGURATION_DESCRIPTOR ConfigurationDescriptor, IntPtr StartPosition, int InterfaceNumber, int AlternateSetting, int InterfaceClass, int InterfaceSubClass, int InterfaceProtocol);


            /// Return Type: PUSB_COMMON_DESCRIPTOR->_USB_COMMON_DESCRIPTOR*
            ///DescriptorBuffer: PVOID->void*
            ///TotalLength: ULONG->unsigned int
            ///StartPosition: PVOID->void*
            ///DescriptorType: LONG->int
            [DllImport("winusb.dll", EntryPoint = "WinUsb_ParseDescriptors")]
            public static extern IntPtr WinUsb_ParseDescriptors(IntPtr DescriptorBuffer, uint TotalLength, IntPtr StartPosition, int DescriptorType);


        }

        public static bool WinUsb_Initialize(IntPtr DeviceHandle, ref IntPtr InterfaceHandle)
        {
            return NativeMethods.WinUsb_Initialize(DeviceHandle, ref InterfaceHandle);
        }
        public static bool WinUsb_Free(IntPtr InterfaceHandle)
        {
            return NativeMethods.WinUsb_Free(InterfaceHandle);
        }
        public static bool WinUsb_GetAssociatedInterface(IntPtr InterfaceHandle, byte AssociatedInterfaceIndex, ref IntPtr AssociatedInterfaceHandle)
        {
            return NativeMethods.WinUsb_GetAssociatedInterface(InterfaceHandle, AssociatedInterfaceIndex, ref AssociatedInterfaceHandle);
        }
        public static bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred)
        {
            return NativeMethods.WinUsb_GetDescriptor(InterfaceHandle, DescriptorType, Index, LanguageID, Buffer, BufferLength, ref LengthTransferred);
        }
        public static USBInterfaceDescriptor WinUsb_QueryInterfaceSettings(IntPtr InterfaceHandle, byte AlternateInterfaceNumber)
        {
            USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor = new USB_INTERFACE_DESCRIPTOR();
            if(NativeMethods.WinUsb_QueryInterfaceSettings(InterfaceHandle, AlternateInterfaceNumber, ref UsbAltInterfaceDescriptor) == false)
            {
                return null;
            }
            return new USBInterfaceDescriptor()
            {
                bLength = UsbAltInterfaceDescriptor.bLength,
                bDescriptorType = UsbAltInterfaceDescriptor.bDescriptorType,
                bInterfaceNumber = UsbAltInterfaceDescriptor.bInterfaceNumber,
                bAlternateSetting = UsbAltInterfaceDescriptor.bAlternateSetting,
                bNumEndpoints = UsbAltInterfaceDescriptor.bNumEndpoints,
                bInterfaceClass = UsbAltInterfaceDescriptor.bInterfaceClass,
                bInterfaceSubClass = UsbAltInterfaceDescriptor.bInterfaceSubClass,
                bInterfaceProtocol = UsbAltInterfaceDescriptor.bInterfaceProtocol,
                iInterface = UsbAltInterfaceDescriptor.iInterface
            };
        }
        public static bool WinUsb_QueryDeviceInformation(IntPtr InterfaceHandle, uint InformationType, ref uint BufferLength, IntPtr Buffer)
        {
            return NativeMethods.WinUsb_QueryDeviceInformation(InterfaceHandle, InformationType, ref BufferLength, Buffer);
        }
        public static bool WinUsb_SetCurrentAlternateSetting(IntPtr InterfaceHandle, byte SettingNumber)
        {
            return NativeMethods.WinUsb_SetCurrentAlternateSetting(InterfaceHandle, SettingNumber);
        }
        public static USBPipeInformation WinUsb_QueryPipe(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, byte PipeIndex)
        {
            WINUSB_PIPE_INFORMATION PipeInformation = new WINUSB_PIPE_INFORMATION();
            if(NativeMethods.WinUsb_QueryPipe(InterfaceHandle, AlternateInterfaceNumber, PipeIndex, ref PipeInformation) == false)
            {
                return null;
            }
            return new USBPipeInformation()
            {
                PipeType = (USBPipeType)PipeInformation.PipeType,
                PipeId = PipeInformation.PipeId,
                MaximumPacketSize = PipeInformation.MaximumPacketSize,
                Interval = PipeInformation.Interval
            };
        }
        public static bool WinUsb_SetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, uint ValueLength, IntPtr Value)
        {
            return NativeMethods.WinUsb_SetPipePolicy(InterfaceHandle, PipeID, PolicyType, ValueLength, Value);
        }
        public static bool WinUsb_GetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, ref uint ValueLength, IntPtr Value)
        {
            return NativeMethods.WinUsb_GetPipePolicy(InterfaceHandle, PipeID, PolicyType, ref ValueLength, Value);
        }
        unsafe public static bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred, NativeOverlapped* Overlapped)
        {
            return NativeMethods.WinUsb_ReadPipe(InterfaceHandle, PipeID, Buffer, BufferLength, ref LengthTransferred, Overlapped);
        }
        unsafe public static bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred, NativeOverlapped* Overlapped)
        {
            return NativeMethods.WinUsb_WritePipe(InterfaceHandle, PipeID, Buffer, BufferLength, ref LengthTransferred, Overlapped);
        }
        unsafe public static bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, USBSetupPacket SetupPacket, IntPtr Buffer, uint BufferLength, ref uint LengthTransferred, NativeOverlapped* Overlapped)
        {
            WINUSB_SETUP_PACKET packet = new WINUSB_SETUP_PACKET()
            {
                Index = SetupPacket.Index,
                Length = SetupPacket.Length,
                Request = SetupPacket.Request,
                RequestType = SetupPacket.RequestType,
                Value = SetupPacket.Value
            };
            return NativeMethods.WinUsb_ControlTransfer(InterfaceHandle, packet, Buffer, BufferLength, ref LengthTransferred, Overlapped);
        }
        public static bool WinUsb_ResetPipe(IntPtr InterfaceHandle, byte PipeID)
        {
            return NativeMethods.WinUsb_ResetPipe(InterfaceHandle, PipeID);
        }
        public static bool WinUsb_AbortPipe(IntPtr InterfaceHandle, byte PipeID)
        {
            return NativeMethods.WinUsb_AbortPipe(InterfaceHandle, PipeID);
        }
        public static bool WinUsb_FlushPipe(IntPtr InterfaceHandle, byte PipeID)
        {
            return NativeMethods.WinUsb_FlushPipe(InterfaceHandle, PipeID);
        }
        public static bool WinUsb_SetPowerPolicy(IntPtr InterfaceHandle, uint PolicyType, uint ValueLength, IntPtr Value)
        {
            return NativeMethods.WinUsb_SetPowerPolicy(InterfaceHandle, PolicyType, ValueLength, Value);
        }
        public static bool WinUsb_GetPowerPolicy(IntPtr InterfaceHandle, uint PolicyType, ref uint ValueLength, IntPtr Value)
        {
            return NativeMethods.WinUsb_GetPowerPolicy(InterfaceHandle, PolicyType, ref ValueLength, Value);
        }
        unsafe public static bool WinUsb_GetOverlappedResult(IntPtr InterfaceHandle, NativeOverlapped* lpOverlapped, ref uint lpNumberOfBytesTransferred, bool bWait)
        {
            return NativeMethods.WinUsb_GetOverlappedResult(InterfaceHandle, lpOverlapped, ref lpNumberOfBytesTransferred, bWait);
        }
        public static IntPtr WinUsb_ParseConfigurationDescriptor(ref USB_CONFIGURATION_DESCRIPTOR ConfigurationDescriptor, IntPtr StartPosition, int InterfaceNumber, int AlternateSetting, int InterfaceClass, int InterfaceSubClass, int InterfaceProtocol)
        {
            return NativeMethods.WinUsb_ParseConfigurationDescriptor(ref ConfigurationDescriptor, StartPosition, InterfaceNumber, AlternateSetting, InterfaceClass, InterfaceSubClass, InterfaceProtocol);
        }
        public static IntPtr WinUsb_ParseDescriptors(IntPtr DescriptorBuffer, uint TotalLength, IntPtr StartPosition, int DescriptorType)
        {
            return NativeMethods.WinUsb_ParseDescriptors(DescriptorBuffer, TotalLength, StartPosition, DescriptorType);
        }
    }
}
