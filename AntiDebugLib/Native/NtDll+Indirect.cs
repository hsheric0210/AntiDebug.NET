using System.Runtime.InteropServices;
using System;

using static AntiDebugLib.Native.AntiDebugLibNative;
using static AntiDebugLib.Native.NativeDefs;
using AntiDebugLib.Utils;

namespace AntiDebugLib.Native
{
    internal static class NtDll
    {
        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtClose(IntPtr Handle);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtSetInformationThread(SafeThreadHandle ThreadHandle, uint ThreadInformationClass, IntPtr ThreadInformation, int ThreadInformationLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtQueryInformationProcess_uint(IntPtr hProcess, uint ProcessInfoClass, out uint ProcessInfo, uint nSize, uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtQueryInformationProcess_IntPtr(IntPtr hProcess, uint ProcessInfoClass, out IntPtr ProcessInfo, uint nSize, uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtQueryInformationProcess_ProcessBasicInfo(IntPtr hProcess, uint ProcessInfoClass, ref PROCESS_BASIC_INFORMATION ProcessInfo, uint nSize, uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtQuerySystemInformation_CodeIntegrityInfo(uint SystemInformationClass, ref SYSTEM_CODEINTEGRITY_INFORMATION SystemInformation, uint SystemInformationLength, out uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtQuerySystemInformation_KernelDebuggerInfo(uint SystemInformationClass, ref SYSTEM_KERNEL_DEBUGGER_INFORMATION SystemInformation, uint SystemInformationLength, out uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint DCsrGetProcessId();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtQueryObject_ref(IntPtr Handle, uint ObjectInformationClass, ref uint ObjectInformation, uint ObjectInformationLength, out uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DNtQueryObject_IntPtr(IntPtr Handle, uint ObjectInformationClass, IntPtr ObjectInformation, uint ObjectInformationLength, out uint ReturnLength);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DRtlCreateQueryDebugBuffer(uint size, [MarshalAs(UnmanagedType.Bool)] bool eventPair);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DRtlQueryProcessHeapInformation(IntPtr debugBuffer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DRtlQueryProcessDebugInformation(uint processId, uint DebugInfoClassMask, IntPtr debugBuffer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate NTSTATUS DRtlDestroyQueryDebugBuffer(IntPtr debugBuffer);

        #endregion

        #region Properties

        internal static DNtClose NtClose { get; private set; }

        internal static DNtSetInformationThread NtSetInformationThread { get; private set; }

        internal static DNtQueryInformationProcess_uint NtQueryInformationProcess_uint { get; private set; }

        internal static DNtQueryInformationProcess_IntPtr NtQueryInformationProcess_IntPtr { get; private set; }

        internal static DNtQueryInformationProcess_ProcessBasicInfo NtQueryInformationProcess_ProcessBasicInfo { get; private set; }

        internal static DNtQuerySystemInformation_CodeIntegrityInfo NtQuerySystemInformation_CodeIntegrityInfo { get; private set; }

        internal static DNtQuerySystemInformation_KernelDebuggerInfo NtQuerySystemInformation_KernelDebuggerInfo { get; private set; }

        internal static DCsrGetProcessId CsrGetProcessId { get; private set; }

        internal static DNtQueryObject_ref NtQueryObject_ref { get; private set; }

        internal static DNtQueryObject_IntPtr NtQueryObject_IntPtr { get; private set; }

        internal static DRtlCreateQueryDebugBuffer RtlCreateQueryDebugBuffer { get; private set; }

        internal static DRtlQueryProcessHeapInformation RtlQueryProcessHeapInformation { get; private set; }

        internal static DRtlQueryProcessDebugInformation RtlQueryProcessDebugInformation { get; private set; }

        internal static DRtlDestroyQueryDebugBuffer RtlDestroyQueryDebugBuffer { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var ntdll = MyGetModuleHandle("ntdll.dll");

            NtClose = Marshal.GetDelegateForFunctionPointer<DNtClose>(MyGetProcAddress(ntdll, "NtClose"));
            NtSetInformationThread = Marshal.GetDelegateForFunctionPointer<DNtSetInformationThread>(MyGetProcAddress(ntdll, "NtSetInformationThread"));
            NtQueryInformationProcess_uint = Marshal.GetDelegateForFunctionPointer<DNtQueryInformationProcess_uint>(MyGetProcAddress(ntdll, "NtQueryInformationProcess"));
            NtQueryInformationProcess_IntPtr = Marshal.GetDelegateForFunctionPointer<DNtQueryInformationProcess_IntPtr>(MyGetProcAddress(ntdll, "NtQueryInformationProcess"));
            NtQueryInformationProcess_ProcessBasicInfo = Marshal.GetDelegateForFunctionPointer<DNtQueryInformationProcess_ProcessBasicInfo>(MyGetProcAddress(ntdll, "NtQueryInformationProcess"));
            NtQuerySystemInformation_CodeIntegrityInfo = Marshal.GetDelegateForFunctionPointer<DNtQuerySystemInformation_CodeIntegrityInfo>(MyGetProcAddress(ntdll, "NtQuerySystemInformation"));
            NtQuerySystemInformation_KernelDebuggerInfo = Marshal.GetDelegateForFunctionPointer<DNtQuerySystemInformation_KernelDebuggerInfo>(MyGetProcAddress(ntdll, "NtQuerySystemInformation"));
            CsrGetProcessId = Marshal.GetDelegateForFunctionPointer<DCsrGetProcessId>(MyGetProcAddress(ntdll, "CsrGetProcessId"));
            NtQueryObject_ref = Marshal.GetDelegateForFunctionPointer<DNtQueryObject_ref>(MyGetProcAddress(ntdll, "NtQueryObject"));
            NtQueryObject_IntPtr = Marshal.GetDelegateForFunctionPointer<DNtQueryObject_IntPtr>(MyGetProcAddress(ntdll, "NtQueryObject"));
            RtlCreateQueryDebugBuffer = Marshal.GetDelegateForFunctionPointer<DRtlCreateQueryDebugBuffer>(MyGetProcAddress(ntdll, "RtlCreateQueryDebugBuffer"));
            RtlQueryProcessHeapInformation = Marshal.GetDelegateForFunctionPointer<DRtlQueryProcessHeapInformation>(MyGetProcAddress(ntdll, "RtlQueryProcessHeapInformation"));
            RtlQueryProcessDebugInformation = Marshal.GetDelegateForFunctionPointer<DRtlQueryProcessDebugInformation>(MyGetProcAddress(ntdll, "RtlQueryProcessDebugInformation"));
            RtlDestroyQueryDebugBuffer = Marshal.GetDelegateForFunctionPointer<DRtlDestroyQueryDebugBuffer>(MyGetProcAddress(ntdll, "RtlDestroyQueryDebugBuffer"));
        }
    }
}
