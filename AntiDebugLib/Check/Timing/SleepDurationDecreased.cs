using System;
using System.Threading;

namespace AntiDebugLib.Check.Timing
{
    /// <summary>
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L224
    /// </summary>
    public class SleepDurationDecreased : CheckBase
    {
        public override string Name => "Sleep Ignorance - TickCount delta too short than expected";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
        {
            var prev = Environment.TickCount;
            Thread.Sleep(500);
            return Environment.TickCount - prev < 500L;
        }
    }
}
