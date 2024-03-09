using System.Runtime.InteropServices;
using System;

using static AntiDebugLib.Native.NtDll.Delegates;
using StealthModule;
using System.IO;

namespace AntiDebugLib.Native
{
    internal static partial class NtDll
    {
        private static MemoryModule mappedNtdll;

        #region Properties

        internal static NtSetInformationThread NtSetInformationThread { get; private set; }

        internal static NtQueryInformationProcess_uint NtQueryInformationProcess_uint { get; private set; }

        internal static NtQueryInformationProcess_IntPtr NtQueryInformationProcess_IntPtr { get; private set; }

        internal static NtQueryInformationProcess_ProcessBasicInfo NtQueryInformationProcess_ProcessBasicInfo { get; private set; }

        internal static NtQuerySystemInformation_CodeIntegrityInfo NtQuerySystemInformation_CodeIntegrityInfo { get; private set; }

        internal static NtQuerySystemInformation_KernelDebuggerInfo NtQuerySystemInformation_KernelDebuggerInfo { get; private set; }

        internal static CsrGetProcessId CsrGetProcessId { get; private set; }

        internal static NtQueryObject_ref NtQueryObject_ref { get; private set; }

        internal static NtQueryObject_IntPtr NtQueryObject_IntPtr { get; private set; }

        #endregion

        internal static void InitNativesUnhooked()
        {
            var ntdllBytes = File.ReadAllBytes(Path.Combine(Environment.SystemDirectory, "ntdll.dll"));
            mappedNtdll = new MemoryModule(ntdllBytes);
            var resolver = mappedNtdll.Exports;
            resolver.CacheAllExports();
            NtSetInformationThread = resolver.GetExport<NtSetInformationThread>("NtSetInformationThread");
            NtQueryInformationProcess_uint = resolver.GetExport<NtQueryInformationProcess_uint>("NtQueryInformationProcess");
            NtQueryInformationProcess_IntPtr = resolver.GetExport<NtQueryInformationProcess_IntPtr>("NtQueryInformationProcess");
            NtQueryInformationProcess_ProcessBasicInfo = resolver.GetExport<NtQueryInformationProcess_ProcessBasicInfo>("NtQueryInformationProcess");
            NtQuerySystemInformation_CodeIntegrityInfo = resolver.GetExport<NtQuerySystemInformation_CodeIntegrityInfo>("NtQuerySystemInformation");
            NtQuerySystemInformation_KernelDebuggerInfo = resolver.GetExport<NtQuerySystemInformation_KernelDebuggerInfo>("NtQuerySystemInformation");
            CsrGetProcessId = resolver.GetExport<CsrGetProcessId>("CsrGetProcessId");
            NtQueryObject_ref = resolver.GetExport<NtQueryObject_ref>("NtQueryObject");
            NtQueryObject_IntPtr = resolver.GetExport<NtQueryObject_IntPtr>("NtQueryObject");
        }
    }
}
