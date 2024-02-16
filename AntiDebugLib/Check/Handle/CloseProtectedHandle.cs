using AntiDebugLib.Utils;
using System;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.Exploits
{
    /// <summary>
    /// Tries to close protected (IntPtr_FLAG_PROTECT_FROM_CLOSE) handle with NtClose.
    /// Then, the NtClose function will just return FALSE on genuine executing environment.
    /// But it will be likely to raise errors on some kind of debuggers.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L89
    /// </summary>
    public class CloseProtectedHandle : CheckBase
    {
        public override string Name => "NtClose(protected-handle)";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
        {
            var random = new Random();
            var hMutex = CreateMutexA(IntPtr.Zero, false, StringUtils.RandomString(random.Next(15, 256), random));
            const uint IntPtr_FLAG_PROTECT_FROM_CLOSE = 0x00000002u;
            SetHandleInformation(hMutex, IntPtr_FLAG_PROTECT_FROM_CLOSE, IntPtr_FLAG_PROTECT_FROM_CLOSE);
            try
            {
                NtClose(hMutex);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
