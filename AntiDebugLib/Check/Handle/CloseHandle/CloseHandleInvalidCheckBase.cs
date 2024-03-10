using System;

namespace AntiDebugLib.Check.Handle.CloseHandle
{
    public abstract class CloseHandleInvalidCheckBase : CheckBase
    {
        // Put here whatever you want
        private static readonly uint[] randomHandleList = new uint[] {
            0x13371337,
            0xDEADBEEF,
            0xDEADDEAD,
            0xDEADF00D,
            0xBADF00D5,
            0xBEAD5B0B,
            0x1337F00D,
            0xBADBEEF0,
        };

        protected IntPtr GetRandomHandle()
            => new IntPtr(unchecked((int)randomHandleList[new Random().Next(randomHandleList.Length)]));
    }
}
