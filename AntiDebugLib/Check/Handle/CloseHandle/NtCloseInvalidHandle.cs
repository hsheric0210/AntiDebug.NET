using System;

using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.Exploits
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/object-handles.html#closehandle
    /// </item>
    /// <item>
    /// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_HandlesValidation.cpp#L26
    /// </item>
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L76
    /// </item>
    /// </list>
    /// </summary>
    public class NtCloseInvalidHandle : CheckBase
    {
        public override string Name => "Close Handle: NtClose (invalid handle)";

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
