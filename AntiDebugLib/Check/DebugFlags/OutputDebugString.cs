using AntiDebugLib.Utils;
using System;
using System.Runtime.InteropServices;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <c>kernel32!OutputDebugStringA</c> will <c>SetLastError()</c> to a nonzero value if there's no debugger attached.
    /// Only works on some old Windows versions. But I'd like to place it here.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L230
    /// </summary>
    public class OutputDebugString : CheckBase
    {
        public override string Name => "OutputDebugString";

        public override CheckReliability Reliability => CheckReliability.Okay;

        public override bool CheckActive()
        {
            var random = new Random();
            OutputDebugStringA(StringUtils.RandomString(random.Next(512), random));
            return Marshal.GetLastWin32Error() == 0;
        }
    }
}
