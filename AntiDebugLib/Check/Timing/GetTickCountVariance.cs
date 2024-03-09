using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.Timing
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L224
    /// </item>
    /// </list>
    /// </summary>
    public class GetTickCountVariance : CheckBase
    {
        public override string Name => "GetTickCount delta too large";

        public override CheckReliability Reliability => CheckReliability.Bad;

        public override CheckResult CheckActive()
        {
            var start = GetTickCount();
            var delta = GetTickCount() - start;
            Logger.Debug("Time delta between simultaneous GetTickCount call: {delta}", delta);
            if (delta > 0x10)
                return DebuggerDetected(new { Delta = delta });

            return DebuggerNotDetected();
        }
    }
}
