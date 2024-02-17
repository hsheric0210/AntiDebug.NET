using AntiDebugLib.Native;
using Microsoft.Win32.SafeHandles;
using System;

namespace AntiDebugLib.Utils
{
    public sealed class SafeThreadHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal readonly static SafeThreadHandle InvalidHandle = new SafeThreadHandle(IntPtr.Zero);

        internal SafeThreadHandle() : base(ownsHandle: true)
        {
        }

        internal SafeThreadHandle(IntPtr handle) : base(ownsHandle: true) => SetHandle(handle);

        public SafeThreadHandle(IntPtr existingHandle, bool ownsHandle) : base(ownsHandle) => SetHandle(existingHandle);

        protected override bool ReleaseHandle() => Kernel32.CloseHandle(handle);
    }
}
