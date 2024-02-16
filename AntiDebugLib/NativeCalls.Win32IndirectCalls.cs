using System.Runtime.InteropServices;
using System;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace AntiDebugLib
{
    internal static partial class NativeCalls
    {
        #region Delegates: Kernel32

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
        internal delegate IntPtr DOpenThread(uint DesiredAccess, bool InheritHandle, int ThreadId);

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

        #region Delegates: User32

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DGetForegroundWindow();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate int DGetWindowTextLengthA(SafeHandle HWND);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate int DGetWindowTextA(IntPtr HWND, StringBuilder WindowText, int nMaxCount);

        #endregion

        #region Delegates: ntdll

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate bool DNtClose(IntPtr Handle);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DNtSetInformationThread(SafeHandle ThreadHandle, uint ThreadInformationClass, IntPtr ThreadInformation, int ThreadInformationLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DNtQueryInformationProcess_uint(SafeHandle hProcess, uint ProcessInfoClass, out uint ProcessInfo, uint nSize, uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DNtQueryInformationProcess_IntPtr(SafeHandle hProcess, uint ProcessInfoClass, out IntPtr ProcessInfo, uint nSize, uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DNtQueryInformationProcess_ProcessBasicInfo(SafeHandle hProcess, uint ProcessInfoClass, ref PROCESS_BASIC_INFORMATION ProcessInfo, uint nSize, uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DNtQuerySystemInformation_CodeIntegrityInfo(uint SystemInformationClass, ref SYSTEM_CODEINTEGRITY_INFORMATION SystemInformation, uint SystemInformationLength, out uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DNtQuerySystemInformation_KernelDebuggerInfo(uint SystemInformationClass, ref SYSTEM_KERNEL_DEBUGGER_INFORMATION SystemInformation, uint SystemInformationLength, out uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DCsrGetProcessId();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DNtQueryObject(IntPtr Handle, uint ObjectInformationClass, IntPtr ObjectInformation, uint ObjectInformationLength, out uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DRtlCreateQueryDebugBuffer(uint size, [MarshalAs(UnmanagedType.Bool)] bool eventPair);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate ulong DRtlQueryProcessHeapInformation(IntPtr debugBuffer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate ulong DRtlQueryProcessDebugInformation(uint processId, uint DebugInfoClassMask, IntPtr debugBuffer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate ulong DRtlDestroyQueryDebugBuffer(IntPtr debugBuffer);

        #endregion

        #region Properties: Kernel32

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

        #region Properties: User32

        internal static DGetForegroundWindow GetForegroundWindow { get; private set; }

        internal static DGetWindowTextLengthA GetWindowTextLengthA { get; private set; }

        internal static DGetWindowTextA GetWindowTextA { get; private set; }

        #endregion

        #region Properties: ntdll

        internal static DNtClose NtClose { get; private set; }

        internal static DNtSetInformationThread NtSetInformationThread { get; private set; }

        internal static DNtQueryInformationProcess_uint NtQueryInformationProcess_uint { get; private set; }

        internal static DNtQueryInformationProcess_IntPtr NtQueryInformationProcess_IntPtr { get; private set; }

        internal static DNtQueryInformationProcess_ProcessBasicInfo NtQueryInformationProcess_ProcessBasicInfo { get; private set; }

        internal static DNtQuerySystemInformation_CodeIntegrityInfo NtQuerySystemInformation_CodeIntegrityInfo { get; private set; }

        internal static DNtQuerySystemInformation_KernelDebuggerInfo NtQuerySystemInformation_KernelDebuggerInfo { get; private set; }

        internal static DCsrGetProcessId CsrGetProcessId { get; private set; }

        internal static DNtQueryObject NtQueryObject { get; private set; }

        internal static DRtlCreateQueryDebugBuffer RtlCreateQueryDebugBuffer { get; private set; }

        internal static DRtlQueryProcessHeapInformation RtlQueryProcessHeapInformation { get; private set; }

        internal static DRtlQueryProcessDebugInformation RtlQueryProcessDebugInformation { get; private set; }

        internal static DRtlDestroyQueryDebugBuffer RtlDestroyQueryDebugBuffer { get; private set; }

        #endregion

        private static void InitIndirectCalls()
        {
            #region Init: Kernel32

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

            #endregion

            #region Init: User32

            var user32 = LoadLibrary("user32.dll"); // user32 is not loaded by default

            GetForegroundWindow = Marshal.GetDelegateForFunctionPointer<DGetForegroundWindow>(MyGetProcAddress(user32, "GetForegroundWindow"));
            GetWindowTextLengthA = Marshal.GetDelegateForFunctionPointer<DGetWindowTextLengthA>(MyGetProcAddress(user32, "GetWindowTextLengthA"));
            GetWindowTextA = Marshal.GetDelegateForFunctionPointer<DGetWindowTextA>(MyGetProcAddress(user32, "GetWindowTextA"));

            #endregion

            #region Init: ntdll

            var ntdll = MyGetModuleHandle("ntdll.dll");

            NtClose = Marshal.GetDelegateForFunctionPointer<DNtClose>(MyGetProcAddress(ntdll, "NtClose"));
            NtSetInformationThread = Marshal.GetDelegateForFunctionPointer<DNtSetInformationThread>(MyGetProcAddress(ntdll, "NtSetInformationThread"));
            NtQueryInformationProcess_uint = Marshal.GetDelegateForFunctionPointer<DNtQueryInformationProcess_uint>(MyGetProcAddress(ntdll, "NtQueryInformationProcess"));
            NtQueryInformationProcess_IntPtr = Marshal.GetDelegateForFunctionPointer<DNtQueryInformationProcess_IntPtr>(MyGetProcAddress(ntdll, "NtQueryInformationProcess"));
            NtQueryInformationProcess_ProcessBasicInfo = Marshal.GetDelegateForFunctionPointer<DNtQueryInformationProcess_ProcessBasicInfo>(MyGetProcAddress(ntdll, "NtQueryInformationProcess"));
            NtQuerySystemInformation_CodeIntegrityInfo = Marshal.GetDelegateForFunctionPointer<DNtQuerySystemInformation_CodeIntegrityInfo>(MyGetProcAddress(ntdll, "NtQuerySystemInformation"));
            NtQuerySystemInformation_KernelDebuggerInfo = Marshal.GetDelegateForFunctionPointer<DNtQuerySystemInformation_KernelDebuggerInfo>(MyGetProcAddress(ntdll, "NtQuerySystemInformation"));
            CsrGetProcessId = Marshal.GetDelegateForFunctionPointer<DCsrGetProcessId>(MyGetProcAddress(ntdll, "CsrGetProcessId"));
            NtQueryObject = Marshal.GetDelegateForFunctionPointer<DNtQueryObject>(MyGetProcAddress(ntdll, "NtQueryObject"));
            RtlCreateQueryDebugBuffer = Marshal.GetDelegateForFunctionPointer<DRtlCreateQueryDebugBuffer>(MyGetProcAddress(ntdll, "RtlCreateQueryDebugBuffer"));
            RtlQueryProcessHeapInformation = Marshal.GetDelegateForFunctionPointer<DRtlQueryProcessHeapInformation>(MyGetProcAddress(ntdll, "RtlQueryProcessHeapInformation"));
            RtlQueryProcessDebugInformation = Marshal.GetDelegateForFunctionPointer<DRtlQueryProcessDebugInformation>(MyGetProcAddress(ntdll, "RtlQueryProcessDebugInformation"));
            RtlDestroyQueryDebugBuffer = Marshal.GetDelegateForFunctionPointer<DRtlDestroyQueryDebugBuffer>(MyGetProcAddress(ntdll, "RtlDestroyQueryDebugBuffer"));

            #endregion
        }
    }
}
