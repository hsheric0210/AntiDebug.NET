using System;

namespace AntiDebugLib.Check.Handle.CloseHandle
{
    public abstract class CloseHandleInvalidCheckBase : CheckBase
    {
        // Put here whatever you want
        private static readonly long[] randomHandleList = new long[] {
            0x13371337L,
            0xDEADBEEFL,
            0xDEADDEADL,
            0xDEADF00DL,
            0xBADF00D5L,
            0xBEAD5B0BL,
            0x1337F00DL,
            0xC15C0D06F00D5L,
            0xBADBEEF0L,
        };

        protected IntPtr GetRandomHandle()
            => new IntPtr(randomHandleList[new Random().Next(randomHandleList.Length)]);
    }
}
