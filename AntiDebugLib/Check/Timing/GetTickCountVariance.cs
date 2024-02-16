using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.Timing
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L224
    /// </summary>
    public class GetTickCountVariance : CheckBase
    {
        public override string Name => "GetTickCount delta too large";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
        {
            var start = GetTickCount();
            return GetTickCount() - start > 0x10;
        }
    }
}
