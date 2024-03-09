using System.Runtime.InteropServices;
using System;
using System.Text;

using static AntiDebugLib.Native.User32.Delegates;
using static AntiDebugLib.Native.Kernel32;
using StealthModule;

namespace AntiDebugLib.Native
{
    internal static class User32
    {
        internal static class Delegates
        {

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate IntPtr GetForegroundWindow();

            [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
            internal delegate int GetWindowTextLengthW(SafeHandle HWND);

            [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
            internal delegate int GetWindowTextW(IntPtr HWND, StringBuilder WindowText, int nMaxCount);
        }

        #region Properties

        internal static GetForegroundWindow GetForegroundWindow { get; private set; }

        internal static GetWindowTextLengthW GetWindowTextLengthW { get; private set; }

        internal static GetWindowTextW GetWindowTextW { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var user32 = LoadLibraryW("user32.dll"); // user32 is not loaded by default

            var resolver = new ExportResolver(user32);
            resolver.CacheAllExports();
            GetForegroundWindow = resolver.GetExport<GetForegroundWindow>("GetForegroundWindow");
            GetWindowTextLengthW = resolver.GetExport<GetWindowTextLengthW>("GetWindowTextLengthW");
            GetWindowTextW = resolver.GetExport<GetWindowTextW>("GetWindowTextW");
        }
    }
}
