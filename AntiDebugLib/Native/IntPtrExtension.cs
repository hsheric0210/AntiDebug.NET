using System;

namespace AntiDebugLib.Native
{
    internal static class IntPtrExtension
    {
        public static string ToHex(this IntPtr ptr)
        {
            if (IntPtr.Size == 4)
                return "0x" + ptr.ToInt32().ToString("X8"); // 32-bit

            return "0x" + ptr.ToInt64().ToString("X16"); // 64-bit
        }

        public static string ToHex(this UIntPtr ptr)
        {
            if (IntPtr.Size == 4)
                return "0x" + ptr.ToUInt32().ToString("X8"); // 32-bit

            return "0x" + ptr.ToUInt64().ToString("X16"); // 64-bit
        }
    }
}
