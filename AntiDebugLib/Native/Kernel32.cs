using System.Runtime.InteropServices;
using System;
using System.Text;
using static AntiDebugLib.Native.AntiDebugLibNative;
using static AntiDebugLib.Native.NativeDefs;
using AntiDebugLib.Utils;
using Microsoft.Win32.SafeHandles;
using StealthModule;

namespace AntiDebugLib.Native
{
    internal static partial class Kernel32
    {
        internal static IntPtr GetCurrentProcess() => new IntPtr(-1);

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DSetHandleInformation(IntPtr hObject, uint dwMask, uint dwFlags);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DCreateMutexA(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DIsDebuggerPresent();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DCheckRemoteDebuggerPresent(IntPtr processHandle, out bool CheckBool);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DWriteProcessMemory(IntPtr ProcHandle, IntPtr BaseAddress, byte[] Buffer, uint size, int NumOfBytes);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate SafeThreadHandle DOpenThread(uint DesiredAccess, bool InheritHandle, int ThreadId);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DGetTickCount();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void DOutputDebugStringA(string Text);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DGetCurrentThread();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DGetThreadContext(IntPtr hThread, ref CONTEXT Context);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate int DQueryFullProcessImageNameA(SafeHandle hProcess, uint Flags, byte[] lpExeName, int[] lpdwSize);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DIsProcessCritical(SafeProcessHandle Handle, ref bool BoolToCheck);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DGetModuleHandleA(string name);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DOpenProcess(uint desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, uint processId);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DCreateFileW([MarshalAs(UnmanagedType.LPWStr)] string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        internal delegate uint DGetModuleFileNameW(IntPtr module, StringBuilder fileName, uint size);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DCloseHandle(IntPtr handle);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DGetFullPathNameW(string lpPathName, uint bufferSize, StringBuilder buffer, IntPtr part);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DVirtualProtect(IntPtr lpAdress, IntPtr dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        internal delegate IntPtr DLoadLibraryW(string lpFileName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true)]
        internal delegate IntPtr DGetProcAddress(IntPtr ModuleHandle, string Function);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true)]
        internal delegate bool DFreeLibrary(IntPtr hModule);

        #endregion

        #region Properties

        internal static DSetHandleInformation SetHandleInformation { get; private set; }

        internal static DCreateMutexA CreateMutexA { get; private set; }

        internal static DIsDebuggerPresent IsDebuggerPresent { get; private set; }

        internal static DCheckRemoteDebuggerPresent CheckRemoteDebuggerPresent { get; private set; }

        internal static DWriteProcessMemory WriteProcessMemory { get; private set; }

        internal static DOpenThread OpenThread { get; private set; }

        internal static DGetTickCount GetTickCount { get; private set; }

        internal static DOutputDebugStringA OutputDebugStringA { get; private set; }

        internal static DGetCurrentThread GetCurrentThread { get; private set; }

        internal static DGetThreadContext GetThreadContext { get; private set; }

        internal static DQueryFullProcessImageNameA QueryFullProcessImageNameA { get; private set; }

        internal static DIsProcessCritical IsProcessCritical { get; private set; }

        internal static DGetModuleHandleA GetModuleHandleA { get; private set; }

        internal static DOpenProcess OpenProcess { get; private set; }

        internal static DCreateFileW CreateFileW { get; private set; }

        internal static DGetModuleFileNameW GetModuleFileNameW { get; private set; }

        internal static DCloseHandle CloseHandle { get; private set; }

        internal static DGetFullPathNameW GetFullPathNameW { get; private set; }

        internal static DVirtualProtect VirtualProtect { get; private set; }

        internal static DLoadLibraryW LoadLibraryW { get; private set; }

        internal static DGetProcAddress GetProcAddress { get; private set; }

        internal static DFreeLibrary FreeLibrary { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var exports = new string[] {
                "SetHandleInformation",
                "CreateMutexA",
                "IsDebuggerPresent",
                "CheckRemoteDebuggerPresent",
                "WriteProcessMemory",
                "OpenThread",
                "GetTickCount",
                "OutputDebugStringA",
                "GetCurrentThread",
                "GetThreadContext",
                "QueryFullProcessImageNameA",
                "IsProcessCritical",
                "GetModuleHandleA",
                "OpenProcess",
                "CreateFileW",
                "GetModuleFileNameW",
                "CloseHandle",
                "GetFullPathNameW",
                "VirtualProtect",
                "LoadLibraryW",
                "GetProcAddress",
                "FreeLibrary",
            };

            var addrs = ExportResolver.ResolveExports("kernel32.dll", exports, throwIfNotFound: true);
            SetHandleInformation = Marshal.GetDelegateForFunctionPointer<DSetHandleInformation>(addrs[0]);
            CreateMutexA = Marshal.GetDelegateForFunctionPointer<DCreateMutexA>(addrs[1]);
            IsDebuggerPresent = Marshal.GetDelegateForFunctionPointer<DIsDebuggerPresent>(addrs[2]);
            CheckRemoteDebuggerPresent = Marshal.GetDelegateForFunctionPointer<DCheckRemoteDebuggerPresent>(addrs[3]);
            WriteProcessMemory = Marshal.GetDelegateForFunctionPointer<DWriteProcessMemory>(addrs[4]);
            OpenThread = Marshal.GetDelegateForFunctionPointer<DOpenThread>(addrs[5]);
            GetTickCount = Marshal.GetDelegateForFunctionPointer<DGetTickCount>(addrs[6]);
            OutputDebugStringA = Marshal.GetDelegateForFunctionPointer<DOutputDebugStringA>(addrs[7]);
            GetCurrentThread = Marshal.GetDelegateForFunctionPointer<DGetCurrentThread>(addrs[8]);
            GetThreadContext = Marshal.GetDelegateForFunctionPointer<DGetThreadContext>(addrs[9]);
            QueryFullProcessImageNameA = Marshal.GetDelegateForFunctionPointer<DQueryFullProcessImageNameA>(addrs[10]);
            IsProcessCritical = Marshal.GetDelegateForFunctionPointer<DIsProcessCritical>(addrs[11]);
            GetModuleHandleA = Marshal.GetDelegateForFunctionPointer<DGetModuleHandleA>(addrs[12]);
            OpenProcess = Marshal.GetDelegateForFunctionPointer<DOpenProcess>(addrs[13]);
            CreateFileW = Marshal.GetDelegateForFunctionPointer<DCreateFileW>(addrs[14]);
            GetModuleFileNameW = Marshal.GetDelegateForFunctionPointer<DGetModuleFileNameW>(addrs[15]);
            CloseHandle = Marshal.GetDelegateForFunctionPointer<DCloseHandle>(addrs[16]);
            GetFullPathNameW = Marshal.GetDelegateForFunctionPointer<DGetFullPathNameW>(addrs[17]);
            VirtualProtect = Marshal.GetDelegateForFunctionPointer<DVirtualProtect>(addrs[18]);
            LoadLibraryW = Marshal.GetDelegateForFunctionPointer<DLoadLibraryW>(addrs[19]);
            GetProcAddress = Marshal.GetDelegateForFunctionPointer<DGetProcAddress>(addrs[20]);
            FreeLibrary = Marshal.GetDelegateForFunctionPointer<DFreeLibrary>(addrs[21]);
        }
    }
}
