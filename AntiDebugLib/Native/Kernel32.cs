using System.Runtime.InteropServices;
using System;
using System.Text;
using static AntiDebugLib.Native.Kernel32.Delegates;
using static AntiDebugLib.Native.NativeDefs;
using AntiDebugLib.Utils;
using Microsoft.Win32.SafeHandles;
using StealthModule;

namespace AntiDebugLib.Native
{
    internal static partial class Kernel32
    {
        internal static IntPtr GetCurrentProcess() => new IntPtr(-1);

        internal static class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool SetHandleInformation(IntPtr hObject, uint dwMask, uint dwFlags);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate IntPtr CreateMutexA(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool IsDebuggerPresent();

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool CheckRemoteDebuggerPresent(IntPtr processHandle, out bool CheckBool);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool WriteProcessMemory(IntPtr ProcHandle, IntPtr BaseAddress, byte[] Buffer, uint size, int NumOfBytes);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate SafeThreadHandle OpenThread(uint DesiredAccess, bool InheritHandle, int ThreadId);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate uint GetTickCount();

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate void OutputDebugStringA(string Text);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate IntPtr GetCurrentThread();

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool GetThreadContext(IntPtr hThread, ref CONTEXT Context);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate int QueryFullProcessImageNameA(SafeHandle hProcess, uint Flags, byte[] lpExeName, int[] lpdwSize);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool IsProcessCritical(SafeProcessHandle Handle, ref bool BoolToCheck);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate IntPtr GetModuleHandleA(string name);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate IntPtr OpenProcess(uint desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, uint processId);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate IntPtr CreateFileW([MarshalAs(UnmanagedType.LPWStr)] string fileName, uint desiredAccess, uint shareMode, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

            [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
            internal delegate uint GetModuleFileNameW(IntPtr module, StringBuilder fileName, uint size);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool CloseHandle(IntPtr handle);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate uint GetFullPathNameW(string lpPathName, uint bufferSize, StringBuilder buffer, IntPtr part);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate bool VirtualProtect(IntPtr lpAdress, IntPtr dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

            [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
            internal delegate IntPtr LoadLibraryW(string lpFileName);

            [UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true)]
            internal delegate IntPtr GetProcAddress(IntPtr ModuleHandle, string Function);

            [UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true)]
            internal delegate bool FreeLibrary(IntPtr hModule);
        }

        #region Properties

        internal static SetHandleInformation SetHandleInformation { get; private set; }

        internal static CreateMutexA CreateMutexA { get; private set; }

        internal static IsDebuggerPresent IsDebuggerPresent { get; private set; }

        internal static CheckRemoteDebuggerPresent CheckRemoteDebuggerPresent { get; private set; }

        internal static WriteProcessMemory WriteProcessMemory { get; private set; }

        internal static OpenThread OpenThread { get; private set; }

        internal static GetTickCount GetTickCount { get; private set; }

        internal static OutputDebugStringA OutputDebugStringA { get; private set; }

        internal static GetCurrentThread GetCurrentThread { get; private set; }

        internal static GetThreadContext GetThreadContext { get; private set; }

        internal static QueryFullProcessImageNameA QueryFullProcessImageNameA { get; private set; }

        internal static IsProcessCritical IsProcessCritical { get; private set; }

        internal static GetModuleHandleA GetModuleHandleA { get; private set; }

        internal static OpenProcess OpenProcess { get; private set; }

        internal static CreateFileW CreateFileW { get; private set; }

        internal static GetModuleFileNameW GetModuleFileNameW { get; private set; }

        internal static CloseHandle CloseHandle { get; private set; }

        internal static GetFullPathNameW GetFullPathNameW { get; private set; }

        internal static VirtualProtect VirtualProtect { get; private set; }

        internal static LoadLibraryW LoadLibraryW { get; private set; }

        internal static GetProcAddress GetProcAddress { get; private set; }

        internal static FreeLibrary FreeLibrary { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var resolver = new ExportResolver("kernel32.dll");
            resolver.CacheAllExports();
            SetHandleInformation = resolver.GetExport<SetHandleInformation>("SetHandleInformation");
            CreateMutexA = resolver.GetExport<CreateMutexA>("CreateMutexA");
            IsDebuggerPresent = resolver.GetExport<IsDebuggerPresent>("IsDebuggerPresent");
            CheckRemoteDebuggerPresent = resolver.GetExport<CheckRemoteDebuggerPresent>("CheckRemoteDebuggerPresent");
            WriteProcessMemory = resolver.GetExport<WriteProcessMemory>("WriteProcessMemory");
            OpenThread = resolver.GetExport<OpenThread>("OpenThread");
            GetTickCount = resolver.GetExport<GetTickCount>("GetTickCount");
            OutputDebugStringA = resolver.GetExport<OutputDebugStringA>("OutputDebugStringA");
            GetCurrentThread = resolver.GetExport<GetCurrentThread>("GetCurrentThread");
            GetThreadContext = resolver.GetExport<GetThreadContext>("GetThreadContext");
            QueryFullProcessImageNameA = resolver.GetExport<QueryFullProcessImageNameA>("QueryFullProcessImageNameA");
            IsProcessCritical = resolver.GetExport<IsProcessCritical>("IsProcessCritical");
            GetModuleHandleA = resolver.GetExport<GetModuleHandleA>("GetModuleHandleA");
            OpenProcess = resolver.GetExport<OpenProcess>("OpenProcess");
            CreateFileW = resolver.GetExport<CreateFileW>("CreateFileW");
            GetModuleFileNameW = resolver.GetExport<GetModuleFileNameW>("GetModuleFileNameW");
            CloseHandle = resolver.GetExport<CloseHandle>("CloseHandle");
            GetFullPathNameW = resolver.GetExport<GetFullPathNameW>("GetFullPathNameW");
            VirtualProtect = resolver.GetExport<VirtualProtect>("VirtualProtect");
            LoadLibraryW = resolver.GetExport<LoadLibraryW>("LoadLibraryW");
            GetProcAddress = resolver.GetExport<GetProcAddress>("GetProcAddress");
            FreeLibrary = resolver.GetExport<FreeLibrary>("FreeLibrary");
        }
    }
}
