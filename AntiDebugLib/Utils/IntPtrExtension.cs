using System;

namespace AntiDebugLib.Utils
{
    internal static class IntPtrExtension
    {
        public static IntPtr Add(this IntPtr a, uint b) => new IntPtr(a.ToInt64() + b);

        public static IntPtr Add(this IntPtr a, ulong b) => new IntPtr(unchecked((long)((ulong)a.ToInt64() + b)));

        public static IntPtr Subtract(this IntPtr a, uint b) => new IntPtr(a.ToInt64() - b);

        public static IntPtr Subtract(this IntPtr a, ulong b) => new IntPtr(unchecked((long)((ulong)a.ToInt64() - b)));
    }
}
