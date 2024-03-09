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
            var resolver = new ExportResolver("kernel32.dll");
            resolver.CacheAllExports();
            SetHandleInformation = resolver.GetExport<DSetHandleInformation>("SetHandleInformation");
            CreateMutexA = resolver.GetExport<DCreateMutexA>("CreateMutexA");
            IsDebuggerPresent = resolver.GetExport<DIsDebuggerPresent>("IsDebuggerPresent");
            CheckRemoteDebuggerPresent = resolver.GetExport<DCheckRemoteDebuggerPresent>("CheckRemoteDebuggerPresent");
            WriteProcessMemory = resolver.GetExport<DWriteProcessMemory>("WriteProcessMemory");
            OpenThread = resolver.GetExport<DOpenThread>("OpenThread");
            GetTickCount = resolver.GetExport<DGetTickCount>("GetTickCount");
            OutputDebugStringA = resolver.GetExport<DOutputDebugStringA>("OutputDebugStringA");
            GetCurrentThread = resolver.GetExport<DGetCurrentThread>("GetCurrentThread");
            GetThreadContext = resolver.GetExport<DGetThreadContext>("GetThreadContext");
            QueryFullProcessImageNameA = resolver.GetExport<DQueryFullProcessImageNameA>("QueryFullProcessImageNameA");
            IsProcessCritical = resolver.GetExport<DIsProcessCritical>("IsProcessCritical");
            GetModuleHandleA = resolver.GetExport<DGetModuleHandleA>("GetModuleHandleA");
            OpenProcess = resolver.GetExport<DOpenProcess>("OpenProcess");
            CreateFileW = resolver.GetExport<DCreateFileW>("CreateFileW");
            GetModuleFileNameW = resolver.GetExport<DGetModuleFileNameW>("GetModuleFileNameW");
            CloseHandle = resolver.GetExport<DCloseHandle>("CloseHandle");
            GetFullPathNameW = resolver.GetExport<DGetFullPathNameW>("GetFullPathNameW");
            VirtualProtect = resolver.GetExport<DVirtualProtect>("VirtualProtect");
            LoadLibraryW = resolver.GetExport<DLoadLibraryW>("LoadLibraryW");
            GetProcAddress = resolver.GetExport<DGetProcAddress>("GetProcAddress");
            FreeLibrary = resolver.GetExport<DFreeLibrary>("FreeLibrary");
        }
    }
}
