using System;

namespace AntiDebugLib.Check.System
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/ProcessJob.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class ProcessJob : CheckBase
    {
        public override string Name => "Suspicious job processes"; // todo: native check

        public override CheckReliability Reliability => CheckReliability.Okay;

        public override CheckResult CheckActive()
        {
            throw new NotImplementedException();
        }
    }
}
