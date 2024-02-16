using System;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.Exploits
{
    /// <summary>
    /// Tries to call NtClose with an invalid handle.
    /// Then, the NtClose function will just return FALSE on genuine executing environment.
    /// But it will be likely to raise any errors if some kind of debuggers are attached.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L76
    /// </summary>
    public class CloseInvalidHandle : CheckBase
    {
        public override string Name => "NtClose(invalid-handle)";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
        {
            try
            {
                NtClose((IntPtr)0x13371337L);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
