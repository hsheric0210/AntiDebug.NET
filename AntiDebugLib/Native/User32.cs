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

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate int GetWindowTextLengthA(SafeHandle HWND);

            [UnmanagedFunctionPointer(CallingConvention.Winapi)]
            internal delegate int GetWindowTextA(IntPtr HWND, StringBuilder WindowText, int nMaxCount);
        }

        #region Properties

        internal static GetForegroundWindow GetForegroundWindow { get; private set; }

        internal static GetWindowTextLengthA GetWindowTextLengthA { get; private set; }

        internal static GetWindowTextA GetWindowTextA { get; private set; }

        #endregion

        internal static void InitNatives()
        {
            var user32 = LoadLibraryW("user32.dll"); // user32 is not loaded by default

            var resolver = new ExportResolver(user32);
            resolver.CacheAllExports();
            GetForegroundWindow = resolver.GetExport<GetForegroundWindow>("GetForegroundWindow");
            GetWindowTextLengthA = resolver.GetExport<GetWindowTextLengthA>("GetWindowTextLengthA");
            GetWindowTextA = resolver.GetExport<GetWindowTextA>("GetWindowTextA");
        }
    }
}
