using System.Runtime.InteropServices;
using System;

using static AntiDebugLib.Native.NtDll.Delegates;
using static AntiDebugLib.Native.NativeDefs;
using AntiDebugLib.Utils;
using StealthModule;

namespace AntiDebugLib.Native
{
    internal static partial class NtDll
    {
        internal static class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtClose(IntPtr Handle);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtSetInformationThread(SafeThreadHandle ThreadHandle, uint ThreadInformationClass, IntPtr ThreadInformation, int ThreadInformationLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtQueryInformationProcess_uint(IntPtr hProcess, uint ProcessInfoClass, out uint ProcessInfo, uint nSize, uint ReturnLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtQueryInformationProcess_IntPtr(IntPtr hProcess, uint ProcessInfoClass, out IntPtr ProcessInfo, uint nSize, uint ReturnLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtQueryInformationProcess_ProcessBasicInfo(IntPtr hProcess, uint ProcessInfoClass, ref PROCESS_BASIC_INFORMATION ProcessInfo, uint nSize, uint ReturnLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtQuerySystemInformation_CodeIntegrityInfo(uint SystemInformationClass, ref SYSTEM_CODEINTEGRITY_INFORMATION SystemInformation, uint SystemInformationLength, out uint ReturnLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtQuerySystemInformation_KernelDebuggerInfo(uint SystemInformationClass, ref SYSTEM_KERNEL_DEBUGGER_INFORMATION SystemInformation, uint SystemInformationLength, out uint ReturnLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate uint CsrGetProcessId();

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtQueryObject_ref(IntPtr Handle, uint ObjectInformationClass, ref uint ObjectInformation, uint ObjectInformationLength, out uint ReturnLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS NtQueryObject_IntPtr(IntPtr Handle, uint ObjectInformationClass, IntPtr ObjectInformation, uint ObjectInformationLength, out uint ReturnLength);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate IntPtr RtlCreateQueryDebugBuffer(uint size, [MarshalAs(UnmanagedType.Bool)] bool eventPair);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS RtlQueryProcessHeapInformation(IntPtr debugBuffer);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS RtlQueryProcessDebugInformation(uint processId, uint DebugInfoClassMask, IntPtr debugBuffer);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate NTSTATUS RtlDestroyQueryDebugBuffer(IntPtr debugBuffer);
        }

        #region Properties

        internal static NtClose NtClose { get; private set; }

        internal static RtlCreateQueryDebugBuffer RtlCreateQueryDebugBuffer { get; private set; }

        internal static RtlDestroyQueryDebugBuffer RtlDestroyQueryDebugBuffer { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var resolver = new ExportResolver("ntdll.dll");
            resolver.CacheAllExports();
            NtClose = resolver.GetExport<NtClose>("NtClose");
            RtlCreateQueryDebugBuffer = resolver.GetExport<RtlCreateQueryDebugBuffer>("RtlCreateQueryDebugBuffer");
            RtlDestroyQueryDebugBuffer = resolver.GetExport<RtlDestroyQueryDebugBuffer>("RtlDestroyQueryDebugBuffer");
        }
    }
}
