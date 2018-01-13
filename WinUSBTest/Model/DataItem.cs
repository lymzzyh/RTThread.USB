using RTThread.USB;
using RTThread.USB.WinUSB;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace WinUSBTest.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CmdMessage
    {
        public byte MessageType;
        public uint Length;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] File;
    }
    public class DataItem
    {
        private WinUSB winUSB;
        private bool isDeviceConnect;
        private USBDevice currentDevice;

        public bool IsDeviceConnect { get => isDeviceConnect; set => isDeviceConnect = value; }
        public USBDevice CurrentDevice { get => currentDevice; set => currentDevice = value; }

        public DataItem()
        {
            winUSB = new WinUSB();
            currentDevice = null;
            RefreshDeviceList();
        }
        public void RefreshDeviceList()
        {
            List<USBDevice> deviceList = SetupUSB.GetDeviceByGuid(new Guid("{6860DC3C-C05F-4807-8807-1CA861CC1D66}"));
            if (deviceList.Count > 0)
            {
                currentDevice = deviceList[0];
                isDeviceConnect = true;
            }
            else
            {
                isDeviceConnect = false;
                currentDevice = null;
            }
        }
        public uint GetFileSizeFromDevice(string filename)
        {

            USBSetupPacket packet = new USBSetupPacket();

            CmdMessage message = new CmdMessage
            {
                MessageType = 0x02,
                File = new byte[128]
            };

            byte[] data = Encoding.ASCII.GetBytes(filename);
            Array.Copy(data, message.File, data.Length);
            message.Length = 4;
            byte[] setupdata = winUSB.StructToBytes(message, Marshal.SizeOf(message), 5 + filename.Length);
            packet.RequestType = 0x21;
            packet.Request = 0x0A;
            packet.Value = 0x00;
            packet.Index = winUSB.UsbInterfaceDesc.iInterface;
            packet.Length = (ushort)(setupdata.Length);
            if (!isDeviceConnect)
            {
                return 0;
            }
            if (!winUSB.Open(currentDevice))
            {
                return 0;
            }
            winUSB.ControlTransForm(packet, setupdata);
            byte pipID = 0;
            foreach (USBPipeInformation info in winUSB.PipList)
            {
                if (info.PipeType == USBPipeType.UsbdPipeTypeBulk && (info.PipeId & 0x80) == 0x80)
                {
                    pipID = info.PipeId;
                    break;
                }
            }
            if (pipID != 0)
            {
                byte[] readbuffer = winUSB.Read(pipID, (int)message.Length);
                if (readbuffer.Length == 0) return 0;
                uint size = (uint)(readbuffer[3] << 24 | readbuffer[2] << 16 | readbuffer[1] << 8 | readbuffer[0]);
                winUSB.Close();
                return size;
            }
            winUSB.Close();
            return 0;
        }

        public byte[] GetFileFromDevice(string filename)
        {

            USBSetupPacket packet = new USBSetupPacket();
            CmdMessage message = new CmdMessage
            {
                MessageType = 0x00,
                File = new byte[128]
            };
            byte[] data = Encoding.ASCII.GetBytes(filename);
            Array.Copy(data, message.File, data.Length);
            message.Length = GetFileSizeFromDevice(filename);
            if (message.Length == 0)
            {
                winUSB.Close();
                return new byte[0];
            }
            byte[] setupdata = winUSB.StructToBytes(message, Marshal.SizeOf(message), 5 + filename.Length);
            packet.RequestType = 0x21;
            packet.Request = 0x0A;
            packet.Value = 0x00;
            packet.Index = winUSB.UsbInterfaceDesc.iInterface;
            packet.Length = (ushort)(setupdata.Length);
            if (!isDeviceConnect)
            {
                return new byte[0];
            }
            if (!winUSB.Open(currentDevice))
            {
                return new byte[0];
            }
            winUSB.ControlTransForm(packet, setupdata);
            byte pipID = 0;
            foreach (USBPipeInformation info in winUSB.PipList)
            {
                if (info.PipeType == USBPipeType.UsbdPipeTypeBulk && (info.PipeId & 0x80) == 0x80)
                {
                    pipID = info.PipeId;
                    break;
                }
            }
            if (pipID != 0)
            {
                byte[] readbuffer = winUSB.Read(pipID, (int)message.Length);
                winUSB.Close();
                return readbuffer;
            }
            winUSB.Close();
            return new byte[0];
        }
        public void WriteFileToDevice(string filename, byte[] data)
        {
            if (!isDeviceConnect)
            {
                return;
            }
            if (!winUSB.Open(currentDevice))
            {
                return;
            }
            USBSetupPacket packet = new USBSetupPacket();
            CmdMessage message = new CmdMessage
            {
                MessageType = 0x01,
                File = new byte[128]
            };
            byte[] _data = Encoding.ASCII.GetBytes(filename);
            Array.Copy(_data, message.File, _data.Length);
            message.Length = (uint)data.Length;
            byte[] setupdata = winUSB.StructToBytes(message, Marshal.SizeOf(message), 5 + filename.Length);
            packet.RequestType = 0x21;
            packet.Request = 0x0A;
            packet.Value = 0x00;
            packet.Index = winUSB.UsbInterfaceDesc.iInterface;
            packet.Length = (ushort)(setupdata.Length);
            winUSB.ControlTransForm(packet, setupdata);
            if (message.Length == 0)
            {
                winUSB.Close();
                return;
            }
            byte pipID = 0;
            foreach (USBPipeInformation info in winUSB.PipList)
            {
                if (info.PipeType == USBPipeType.UsbdPipeTypeBulk && (info.PipeId & 0x80) == 0x00)
                {
                    pipID = info.PipeId;
                    break;
                }
            }
            if (pipID != 0)
            {
                winUSB.Write(pipID, data);
            }
            winUSB.Close();
        }
    }
}