using AntiDebugLib.Utils;
using System;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L230
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/OutputDebugStringAPI.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.D.ix. OutputDebugString
    /// </item>
    /// </list>
    /// </summary>
    public class OutputDebugString : CheckBase
    {
        public override string Name => "OutputDebugString";

        public override CheckReliability Reliability => CheckReliability.Okay;

        public override bool CheckActive()
        {
            var random = new Random();
            OutputDebugStringA(StringUtils.RandomString(random.Next(512), random));
            return Marshal.GetLastWin32Error() != 0;
        }
    }
}
