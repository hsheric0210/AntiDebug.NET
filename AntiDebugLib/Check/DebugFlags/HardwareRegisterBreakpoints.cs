using static AntiDebugLib.Native.NativeStructs;
using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.DebugFlags
{    /// <summary>
     /// <list type="bullet">
     /// <item>
     /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L258
     /// </item>
     /// </list>
     /// </summary>
    public class HardwareRegisterBreakpoints : CheckBase
    {
        public override string Name => "Hardware Register Breakpoints";

        public override CheckReliability Reliability => CheckReliability.Great;

        private const long CONTEXT_DEBUG_REGISTERS = 0x00010000L | 0x00000010L;

        public override bool CheckActive()
        {
            var ctx = new CONTEXT
            {
                ContextFlags = CONTEXT_DEBUG_REGISTERS
            };

            return GetThreadContext(GetCurrentThread(), ref ctx)
                && (ctx.Dr1 != 0x00
                    || ctx.Dr2 != 0x00
                    || ctx.Dr3 != 0x00
                    || ctx.Dr4 != 0x00
                    || ctx.Dr5 != 0x00
                    || ctx.Dr6 != 0x00
                    || ctx.Dr7 != 0x00);
        }
    }
}
