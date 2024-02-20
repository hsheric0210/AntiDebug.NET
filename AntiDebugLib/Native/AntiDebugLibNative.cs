using AntiDebugLib.Properties;
using System;
using System.Runtime.InteropServices;

namespace AntiDebugLib.Native
{
    internal static class AntiDebugLibNative
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate ulong DMyEntryPoint();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
        internal delegate IntPtr DMyGetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
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

            return pfnMyGetProcAddress(dllBase, procName);
        }

        internal static IntPtr MyGetModuleHandle(string dllName) => pfnMyGetModuleHandle(dllName);

        internal static IntPtr GetPeb() => pfnMyGetPeb();

        private static DLLFromMemory nativeModule; // Prevent DLL from get garbage collected

        private static string DecorateFunctionName(string name, int paramSize)
        {
            if (Environment.Is64BitProcess)
                return name;

            return '_' + name + '@' + paramSize; // Example: _MyFunctionName@0
        }

        internal static void Init()
        {
            AntiDebug.Logger.Information("Will use {bit}-bit native library.", Environment.Is64BitProcess ? 64 : 32);
            nativeModule = new DLLFromMemory(Environment.Is64BitProcess ? Resources.AntiDebugLibNative_x64 : Resources.AntiDebugLibNative_Win32);
            pfnMyEntryPoint = nativeModule.GetDelegateFromFuncName<DMyEntryPoint>(DecorateFunctionName(/*<cs_entrypoint>*/"AcmStartupObject"/*</cs_entrypoint>*/, 0));
            pfnMyGetProcAddress = nativeModule.GetDelegateFromFuncName<DMyGetProcAddress>(DecorateFunctionName(/*<cs_getprocaddress>*/"EeInitializeCom"/*</cs_getprocaddress>*/, 8));
            pfnMyGetModuleHandle = nativeModule.GetDelegateFromFuncName<DMyGetModuleHandle>(DecorateFunctionName(/*<cs_getmodulehandle>*/"EeGetComObject"/*</cs_getmodulehandle>*/, 4));
            pfnMyGetPeb = nativeModule.GetDelegateFromFuncName<DMyGetPeb>(DecorateFunctionName(/*<cs_getpeb>*/"EeGetVerifier"/*</cs_getpeb>*/, 0));

            // initialize indirect calls
            Kernel32.InitNatives();
            NtDll.InitNatives();
            User32.InitNatives();
        }
    }
}
