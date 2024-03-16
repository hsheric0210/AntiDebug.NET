using System;

namespace AntiDebugLib.Check.Hooking
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtSetInformationThread_ThreadHideFromDebugger.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class NtSetInformationThreadHook : CheckBase
    {
        public override string Name => "NtSetInformationThread hooking check";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
        {
            // check if NtSetInformationThread is hooked by calling it with bogus parameters
            throw new NotImplementedException();
        }
    }
}
