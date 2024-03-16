using System;

namespace AntiDebugLib.Check.Memory
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/PageExceptionBreakpointCheck.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class PageExecptionBreakpoint : CheckBase
    {
        public override string Name => "Memory Page Exception breakpoint check"; // todo: native check

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
        {
            throw new NotImplementedException();
        }
    }
}
