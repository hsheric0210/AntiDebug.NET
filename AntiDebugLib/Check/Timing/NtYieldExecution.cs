using System;

namespace AntiDebugLib.Check.Timing
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtYieldExecution.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class NtYieldExecution : CheckBase
    {
        public override string Name => "NtYieldExecution (SwitchToThread)";

        public override CheckReliability Reliability => CheckReliability.Bad;

        public override CheckResult CheckActive()
        {
            throw new NotImplementedException();
        }
    }
}
