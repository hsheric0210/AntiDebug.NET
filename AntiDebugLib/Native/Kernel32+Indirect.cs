using System.Runtime.InteropServices;
using System;
using System.Text;
using Microsoft.Win32.SafeHandles;

using static AntiDebugLib.Native.AntiDebugLibNative;
using static AntiDebugLib.Native.NativeStructs;
using AntiDebugLib.Utils;

namespace AntiDebugLib.Native
{
    internal static partial class Kernel32
    {
        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DSetHandleInformation(IntPtr hObject, uint dwMask, uint dwFlags);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DCreateMutexA(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DIsDebuggerPresent();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DCheckRemoteDebuggerPresent(SafeProcessHandle processHandle, out bool CheckBool);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DWriteProcessMemory(SafeProcessHandle ProcHandle, IntPtr BaseAddress, byte[] Buffer, uint size, int NumOfBytes);

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

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DGetModuleFileNameW(IntPtr module, StringBuilder fileName, uint size);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DCloseHandle(IntPtr handle);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DIsWow64Process2(IntPtr process, out uint processMachine, out uint nativeMatchine);

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

        internal static DIsWow64Process2 IsWow64Process2 { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var kernel32 = MyGetModuleHandle("kernel32.dll");

            SetHandleInformation = Marshal.GetDelegateForFunctionPointer<DSetHandleInformation>(MyGetProcAddress(kernel32, "SetHandleInformation"));
            CreateMutexA = Marshal.GetDelegateForFunctionPointer<DCreateMutexA>(MyGetProcAddress(kernel32, "CreateMutexA"));
            IsDebuggerPresent = Marshal.GetDelegateForFunctionPointer<DIsDebuggerPresent>(MyGetProcAddress(kernel32, "IsDebuggerPresent"));
            CheckRemoteDebuggerPresent = Marshal.GetDelegateForFunctionPointer<DCheckRemoteDebuggerPresent>(MyGetProcAddress(kernel32, "CheckRemoteDebuggerPresent"));
            WriteProcessMemory = Marshal.GetDelegateForFunctionPointer<DWriteProcessMemory>(MyGetProcAddress(kernel32, "WriteProcessMemory"));
            OpenThread = Marshal.GetDelegateForFunctionPointer<DOpenThread>(MyGetProcAddress(kernel32, "OpenThread"));
            GetTickCount = Marshal.GetDelegateForFunctionPointer<DGetTickCount>(MyGetProcAddress(kernel32, "GetTickCount"));
            OutputDebugStringA = Marshal.GetDelegateForFunctionPointer<DOutputDebugStringA>(MyGetProcAddress(kernel32, "OutputDebugStringA"));
            GetCurrentThread = Marshal.GetDelegateForFunctionPointer<DGetCurrentThread>(MyGetProcAddress(kernel32, "GetCurrentThread"));
            GetThreadContext = Marshal.GetDelegateForFunctionPointer<DGetThreadContext>(MyGetProcAddress(kernel32, "GetThreadContext"));
            QueryFullProcessImageNameA = Marshal.GetDelegateForFunctionPointer<DQueryFullProcessImageNameA>(MyGetProcAddress(kernel32, "QueryFullProcessImageNameA"));
            IsProcessCritical = Marshal.GetDelegateForFunctionPointer<DIsProcessCritical>(MyGetProcAddress(kernel32, "IsProcessCritical"));
            GetModuleHandleA = Marshal.GetDelegateForFunctionPointer<DGetModuleHandleA>(MyGetProcAddress(kernel32, "GetModuleHandleA"));
            OpenProcess = Marshal.GetDelegateForFunctionPointer<DOpenProcess>(MyGetProcAddress(kernel32, "OpenProcess"));
            CreateFileW = Marshal.GetDelegateForFunctionPointer<DCreateFileW>(MyGetProcAddress(kernel32, "CreateFileW"));
            GetModuleFileNameW = Marshal.GetDelegateForFunctionPointer<DGetModuleFileNameW>(MyGetProcAddress(kernel32, "GetModuleFileNameW"));
            CloseHandle = Marshal.GetDelegateForFunctionPointer<DCloseHandle>(MyGetProcAddress(kernel32, "CloseHandle"));
            IsWow64Process2 = Marshal.GetDelegateForFunctionPointer<DIsWow64Process2>(MyGetProcAddress(kernel32, "IsWow64Process2"));
        }
    }
}
