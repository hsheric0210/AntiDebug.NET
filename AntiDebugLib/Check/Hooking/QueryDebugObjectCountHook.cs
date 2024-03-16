using System;

namespace AntiDebugLib.Check.Hooking
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtQueryObject_ObjectTypeInformation.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class QueryDebugObjectCountHook : CheckBase
    {
        public override string Name => "NtQueryObject is hooked";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
        {
            throw new NotImplementedException();
        }
    }
}
