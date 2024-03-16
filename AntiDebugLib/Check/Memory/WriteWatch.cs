using System;

namespace AntiDebugLib.Check.Memory
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/WriteWatch.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class WriteWatch : CheckBase
    {
        public override string Name => "Memory WriteWatch";

        public override CheckReliability Reliability => CheckReliability.Great;

        public override CheckResult CheckActive()
        {
            throw new NotImplementedException();
        }
    }
}
