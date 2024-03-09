using System.Runtime.InteropServices;
using System;
using System.Text;

using static AntiDebugLib.Native.AntiDebugLibNative;
using static AntiDebugLib.Native.Kernel32;
using StealthModule;

namespace AntiDebugLib.Native
{
    internal static class User32
    {
        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr DGetForegroundWindow();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate int DGetWindowTextLengthA(SafeHandle HWND);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate int DGetWindowTextA(IntPtr HWND, StringBuilder WindowText, int nMaxCount);

        #endregion

        #region Properties

        internal static DGetForegroundWindow GetForegroundWindow { get; private set; }

        internal static DGetWindowTextLengthA GetWindowTextLengthA { get; private set; }

        internal static DGetWindowTextA GetWindowTextA { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var user32 = LoadLibraryW("user32.dll"); // user32 is not loaded by default

            var resolver = new ExportResolver(user32);
            resolver.CacheAllExports();
            GetForegroundWindow = resolver.GetExport<DGetForegroundWindow>("GetForegroundWindow");
            GetWindowTextLengthA = resolver.GetExport<DGetWindowTextLengthA>("GetWindowTextLengthA");
            GetWindowTextA = resolver.GetExport<DGetWindowTextA>("GetWindowTextA");
        }
    }
}
