using System;
using System.Runtime.InteropServices;

namespace AntiDebugLib
{
    internal static partial class NativeCalls
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate ulong DMyEntryPoint();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate IntPtr DMyGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate IntPtr DMyGetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string moduleName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate IntPtr DMyGetPeb();

        private static DMyEntryPoint pfnMyEntryPoint;
        private static DMyGetProcAddress pfnMyGetProcAddress;
        private static DMyGetModuleHandle pfnMyGetModuleHandle;
        private static DMyGetPeb pfnMyGetPeb;

        internal static ulong DoNativeChecks() => pfnMyEntryPoint();

        internal static IntPtr MyGetProcAddress(IntPtr dllBase, string procName)
        {
            if (dllBase == IntPtr.Zero)
                throw new ArgumentException("dllBase is zero", nameof(dllBase));

            var pfn = pfnMyGetProcAddress(dllBase, procName);
            if (pfn == IntPtr.Zero)
                throw new EntryPointNotFoundException("Procedure " + procName + " is not found.");

            return pfn;
        }

        internal static IntPtr MyGetModuleHandle(string dllName)
        {
            var pfn = pfnMyGetModuleHandle(dllName);
            if (pfn == IntPtr.Zero)
                throw new EntryPointNotFoundException("DLL " + dllName + " is not loaded.");

            return pfn;
        }

        internal static IntPtr GetPeb() => pfnMyGetPeb();

        internal static void Init(string libPath)
        {
            var dll = LoadLibrary(libPath);
            pfnMyEntryPoint = Marshal.GetDelegateForFunctionPointer<DMyEntryPoint>(GetProcAddress(dll, /*<cs_entrypoint>*/"AcmStartupObject"/*</cs_entrypoint>*/));
            pfnMyGetProcAddress = Marshal.GetDelegateForFunctionPointer<DMyGetProcAddress>(GetProcAddress(dll, /*<cs_getmodulehandle>*/"EeGetComObject"/*</cs_getmodulehandle>*/));
            pfnMyGetModuleHandle = Marshal.GetDelegateForFunctionPointer<DMyGetModuleHandle>(GetProcAddress(dll, /*<cs_getprocaddress>*/"EeInitializeCom"/*</cs_getprocaddress>*/));
            pfnMyGetPeb = Marshal.GetDelegateForFunctionPointer<DMyGetPeb>(GetProcAddress(dll, /*<cs_getpeb>*/"EeGetVerifier"/*</cs_getpeb>*/));

            InitIndirectCalls();
        }
    }
}
