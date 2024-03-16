using System;

namespace AntiDebugLib.Check.Hooking.ModuleBounds
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/ModuleBoundsHookCheck.cpp
    /// </item>
    /// </list>
    /// </summary>
    internal abstract class ModuleBoundsCheck : CheckBase
    {
        protected abstract string DllName { get; }

        protected abstract string[] ProcNames { get; }

        protected abstract byte[] BadOpCodes { get; }

        public override CheckReliability Reliability => CheckReliability.Okay;

        public override CheckResult CheckPassive()
        {
            throw new NotImplementedException();
        }
    }
}
