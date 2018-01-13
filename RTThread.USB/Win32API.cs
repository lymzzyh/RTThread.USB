using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RTThread.USB
{
    internal class Win32API
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {

            /// DWORD->unsigned int
            public uint dbcc_size;

            /// DWORD->unsigned int
            public uint dbcc_devicetype;

            /// DWORD->unsigned int
            public uint dbcc_reserved;

            /// GUID->_GUID
            public Guid dbcc_classguid;

            /// TCHAR[1]
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string dbcc_name;
        }
        public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            /// DWORD->unsigned int
            public uint dbch_size;

            /// DWORD->unsigned int
            public uint dbch_devicetype;

            /// DWORD->unsigned int
            public uint dbch_reserved;
        }
        /// DBT_CONFIGCHANGECANCELED -> 0x0019
        public const int DBT_CONFIGCHANGECANCELED = 25;

        /// DBT_CONFIGCHANGED -> 0x0018
        public const int DBT_CONFIGCHANGED = 24;

        /// DBT_CUSTOMEVENT -> 0x8006
        public const int DBT_CUSTOMEVENT = 32774;

        /// DBT_DEVICEARRIVAL -> 0x8000
        public const int DBT_DEVICEARRIVAL = 32768;

        /// DBT_DEVICEQUERYREMOVE -> 0x8001
        public const int DBT_DEVICEQUERYREMOVE = 32769;

        /// DBT_DEVICEQUERYREMOVEFAILED -> 0x8002
        public const int DBT_DEVICEQUERYREMOVEFAILED = 32770;

        /// DBT_DEVICEREMOVECOMPLETE -> 0x8004
        public const int DBT_DEVICEREMOVECOMPLETE = 32772;

        /// DBT_DEVICEREMOVEPENDING -> 0x8003
        public const int DBT_DEVICEREMOVEPENDING = 32771;

        /// DBT_DEVICETYPESPECIFIC -> 0x8005
        public const int DBT_DEVICETYPESPECIFIC = 32773;

        /// DBT_DEVNODES_CHANGED -> 0x0007
        public const int DBT_DEVNODES_CHANGED = 7;

        /// DBT_QUERYCHANGECONFIG -> 0x0017
        public const int DBT_QUERYCHANGECONFIG = 23;

        /// DBT_USERDEFINED -> 0xFFFF
        public const int DBT_USERDEFINED = 65535;


        public const int WM_DEVICECHANGE = 537;

        /// FILE_SHARE_READ -> 0x00000001
        public const int FILE_SHARE_READ = 1;

        /// FILE_SHARE_WRITE -> 0x00000002
        public const int FILE_SHARE_WRITE = 2;

        /// FILE_SHARE_DELETE -> 0x00000004
        public const int FILE_SHARE_DELETE = 4;

        public const int MAX_PATH = 260;
        /// <summary>
        /// File attributes and flags for the file. 
        /// </summary>
        /// 
        public static class FLAGSANDATTRIBUTES
        {
            public const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
            public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
            public const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
            public const uint FILE_FLAG_RANDOM_ACCESS = 0x10000000;
            public const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;
            public const uint FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;
            public const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
            public const uint FILE_FLAG_POSIX_SEMANTICS = 0x01000000;
            public const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
            public const uint FILE_FLAG_OPEN_NO_RECALL = 0x00100000;
            public const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;
        }
        /// <summary>
        /// Action to take on files that exist, and which action to take when files do not exist. 
        /// </summary>
        public static class CREATIONDISPOSITION
        {
            public const uint CREATE_NEW = 1;
            public const uint CREATE_ALWAYS = 2;
            public const uint OPEN_EXISTING = 3;
            public const uint OPEN_ALWAYS = 4;
            public const uint TRUNCATE_EXISTING = 5;
        }
        /// <summary>
        /// Type of access to the object. 
        ///</summary>
        public static class DESIREDACCESS
        {
            public const uint GENERIC_READ = 0x80000000;
            public const uint GENERIC_WRITE = 0x40000000;
            public const uint GENERIC_EXECUTE = 0x20000000;
            public const uint GENERIC_ALL = 0x10000000;
        }

        private static class NativeMethods
        {
            /// <summary>
            /// This function creates, opens, or truncates a file, COM port, device, service, or console. 
            /// </summary>
            /// <param name="fileName">a null-terminated string that specifies the name of the object</param>
            /// <param name="desiredAccess">Type of access to the object</param>
            /// <param name="shareMode">Share mode for object</param>
            /// <param name="securityAttributes">Ignored; set to NULL</param>
            /// <param name="creationDisposition">Action to take on files that exist, and which action to take when files do not exist</param>
            /// <param name="flagsAndAttributes">File attributes and flags for the file</param>
            /// <param name="templateFile">Ignored</param>
            /// <returns>An open handle to the specified file indicates success</returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            static public extern IntPtr CreateFile(
            string FileName,                // 文件名
            uint DesiredAccess,             // 访问模式
            uint ShareMode,                 // 共享模式
            IntPtr SecurityAttributes,        // 安全属性
            uint CreationDisposition,       // 如何创建
            uint FlagsAndAttributes,        // 文件属性
            IntPtr hTemplateFile               // 模板文件的句柄
            );

            /// Return Type: BOOL->boolean
            ///hObject: HANDLE->void*
            [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr hObject);

            /// Return Type: DWORD->unsigned int
            [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
            public static extern uint GetLastError();

            /// Return Type: HDEVNOTIFY->PVOID->void*
            ///hRecipient: HANDLE->void*
            ///NotificationFilter: LPVOID->void*
            ///Flags: DWORD->unsigned int
            [DllImport("user32.dll", EntryPoint = "RegisterDeviceNotification")]
            public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);
        }
        public static IntPtr CreateDeviceFile(string device)
        {
            //return new IntPtr ();
            return NativeMethods.CreateFile(device, DESIREDACCESS.GENERIC_READ | DESIREDACCESS.GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, CREATIONDISPOSITION.OPEN_EXISTING, 0x00000080 | FLAGSANDATTRIBUTES.FILE_FLAG_OVERLAPPED, IntPtr.Zero);
        }
        public static bool CloseHandle(IntPtr hObject)
        {
            return NativeMethods.CloseHandle(hObject);
        }
        public static uint GetLastError()
        {
            return NativeMethods.GetLastError();
        }
        public static IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags)
        {
            return NativeMethods.RegisterDeviceNotification(hRecipient, NotificationFilter, Flags);
        }
    }
}
