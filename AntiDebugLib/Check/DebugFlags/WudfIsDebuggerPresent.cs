using AntiDebugLib.Utils;
using System;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/WUDF_IsDebuggerPresent.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class WudfIsDebuggerPresent : CheckBase
    {
        public override string Name => "WUDF IsDebuggerPresent";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
        {
            throw new NotImplementedException();
        }
    }
}
