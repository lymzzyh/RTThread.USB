using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static RTThread.USB.WinUSB.WinUSBAPI;
using static RTThread.USB.Win32API;

namespace RTThread.USB.WinUSB
{
    public class USBInterfaceDescriptor
    {
        public byte bLength;
        public byte bDescriptorType;
        public byte bInterfaceNumber;
        public byte bAlternateSetting;
        public byte bNumEndpoints;
        public byte bInterfaceClass;
        public byte bInterfaceSubClass;
        public byte bInterfaceProtocol;
        public byte iInterface;
    }
    public enum USBPipeType
    {
        UsbdPipeTypeControl = 0,
        UsbdPipeTypeIsochronous = 1,
        UsbdPipeTypeBulk = 2,
        UsbdPipeTypeInterrupt = 3
    }
    public class USBPipeInformation
    {
        public USBPipeType PipeType;
        public byte PipeId;
        public ushort MaximumPacketSize;
        public byte Interval;
        public bool IsIn => ((PipeId & 0x80) == 0x80) ? true : false;
    }
    public struct USBSetupPacket
    {
        public byte RequestType;
        public byte Request;
        public ushort Value;
        public ushort Index;
        public ushort Length;
    }
    public class WinUSB
    {
        private IntPtr deviceHandle;
        private IntPtr winUSBHandle;
        private bool isOpen;
        private USBInterfaceDescriptor usbInterfaceDesc;
        private List<USBPipeInformation> pipList;
        public bool IsOpen { get => isOpen; }
        public USBInterfaceDescriptor UsbInterfaceDesc { get => usbInterfaceDesc; }
        public List<USBPipeInformation> PipList { get => pipList; }

        public WinUSB()
        {
            pipList = new List<USBPipeInformation>();
            usbInterfaceDesc = new USBInterfaceDescriptor();
        }

        public bool Open(USBDevice device)
        {
            deviceHandle = CreateDeviceFile(device.Path);
            if (deviceHandle.ToInt32() == -1)
            {
                return false;
            }
            if (WinUsb_Initialize(deviceHandle, ref winUSBHandle) == false)
            {
                CloseHandle(deviceHandle);
                throw new Exception("Error Code:" + GetLastError().ToString());
            }
            isOpen = true;
            RefreshDeviceInformation();
            return isOpen;
        }
        private void RefreshDeviceInformation()
        {
            pipList.RemoveAll(x => true);
            if (!isOpen)
            {
                return;
            }
            usbInterfaceDesc = WinUsb_QueryInterfaceSettings(winUSBHandle, 0);
            if (usbInterfaceDesc == null)
                return;
            for (byte index = 0; index < usbInterfaceDesc.bNumEndpoints + 1; index++)
            {
                USBPipeInformation pip = WinUsb_QueryPipe(winUSBHandle, 0, index);
                if (pip != null)
                    pipList.Add(pip);
            }
        }
        public void Close()
        {
            if (!isOpen)
                return;
            WinUsb_Free(winUSBHandle);
            CloseHandle(deviceHandle);
            isOpen = false;
        }
        public byte[] StructToBytes(object structObj, int structSize, int copySize)
        {
            byte[] bytes = new byte[copySize];
            IntPtr structPtr = Marshal.AllocHGlobal(structSize);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, copySize);
            Marshal.FreeHGlobal(structPtr);
            return bytes;

        }
        private static IntPtr BytesToIntptr(byte[] bytes)
        {
            int size = bytes.Length;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, buffer, size);
            return buffer;
        }
        unsafe public uint ControlTransForm(USBSetupPacket packet, byte[] Buffer)
        {
            uint cbSent = 0;
            if (!isOpen) return cbSent;
            IntPtr buffer = BytesToIntptr(Buffer);
            WinUsb_ControlTransfer(winUSBHandle, packet, buffer, (uint)Buffer.Length, ref cbSent, null);
            Marshal.FreeHGlobal(buffer);
            return cbSent;

        }
        unsafe public uint ControlTransForm(USBSetupPacket packet)
        {
            uint cbSent = 0;
            if (!isOpen) return cbSent;
            WinUsb_ControlTransfer(winUSBHandle, packet, IntPtr.Zero, 0, ref cbSent, null);
            return cbSent;
        }
        unsafe public uint Write(byte pipID, byte[] Buffer)
        {
            uint cbSent = 0;
            if (!isOpen) return cbSent;
            IntPtr buffer = BytesToIntptr(Buffer);
            WinUsb_WritePipe(winUSBHandle, pipID, buffer, (uint)Buffer.Length, ref cbSent, null);
            Marshal.FreeHGlobal(buffer);
            return cbSent;
        }
        unsafe public byte[] Read(byte pipID, int size)
        {
            if (!isOpen) return new byte[0];
            IntPtr readBuffer = Marshal.AllocHGlobal(size);
            uint read = 0;
            WinUsb_ReadPipe(winUSBHandle, pipID, readBuffer, (uint)size, ref read, null);
            byte[] result = new byte[read];
            Marshal.Copy(readBuffer, result, 0, result.Length);
            Marshal.FreeHGlobal(readBuffer);
            return result;
        }
        public bool ResetPipe(byte pipID)
        {
            if (!isOpen) return false;
            return WinUsb_ResetPipe(winUSBHandle, pipID);
        }
        public bool AbortPipe(byte pipID)
        {
            if (!isOpen) return false;
            return WinUsb_AbortPipe(winUSBHandle, pipID);
        }
    }
}
