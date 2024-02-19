using AntiDebugLib.Properties;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AntiDebugLib.Native
{
    internal static class AntiDebugLibNative
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate ulong DMyEntryPoint();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DMyGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DMyGetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string moduleName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
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

        private static string FixFunctionName(string name, int paramSize)
        {
            if (Environment.Is64BitProcess)
                return name;

            return '_' + name + '@' + paramSize; // Example: _MyFunctionName@0
        }

        internal static void Init()
        {
            var mem = new DLLFromMemory(Environment.Is64BitProcess ? Resources.AntiDebugLibNative_x64 : Resources.AntiDebugLibNative_Win32);
            pfnMyEntryPoint = mem.GetDelegateFromFuncName<DMyEntryPoint>(FixFunctionName(/*<cs_entrypoint>*/"AcmStartupObject"/*</cs_entrypoint>*/, 0));
            pfnMyGetProcAddress = mem.GetDelegateFromFuncName<DMyGetProcAddress>(FixFunctionName(/*<cs_getmodulehandle>*/"EeGetComObject"/*</cs_getmodulehandle>*/, 4));
            pfnMyGetModuleHandle = mem.GetDelegateFromFuncName<DMyGetModuleHandle>(FixFunctionName(/*<cs_getprocaddress>*/"EeInitializeCom"/*</cs_getprocaddress>*/, 8));
            pfnMyGetPeb = mem.GetDelegateFromFuncName<DMyGetPeb>(FixFunctionName(/*<cs_getpeb>*/"EeGetVerifier"/*</cs_getpeb>*/, 0));

            Thread.Sleep(600000);

            // initialize indirect calls
            Kernel32.InitNatives();
            NtDll.InitNatives();
            User32.InitNatives();
        }
    }
}
