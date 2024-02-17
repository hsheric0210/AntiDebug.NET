using System.Diagnostics;

namespace AntiDebugLib.Check.DebugFlags
{    /// <summary>
     /// <list type="bullet">
     /// <item>
     /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L243
     /// </item>
     /// </list>
     /// </summary>
    public class DebuggerBreak : CheckBase
    {
        public override string Name => "Debugger.Break";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
        {
            try
            {
                Debugger.Break();
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
