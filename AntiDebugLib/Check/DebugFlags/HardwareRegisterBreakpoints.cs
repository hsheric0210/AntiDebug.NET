using static AntiDebugLib.Native.NativeDefs;
using static AntiDebugLib.Native.Kernel32;
using System.Runtime.InteropServices;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L258
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/HardwareBreakpoints.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 6.A. Hardware breakpoints
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
            var ctx = new CONTEXT { ContextFlags = CONTEXT_DEBUG_REGISTERS };

            if (!GetThreadContext(GetCurrentThread(), ref ctx))
            {
                Logger.Warning("Unable to get the current thread context. GetThreadContext returned win32 error {error}.", Marshal.GetLastWin32Error());
                return false;
            }

            Logger.Debug("Current thread debug register values: DR0={dr0:X} DR1={dr1:X} DR2={dr2:X} DR3={dr3:X} DR4={dr4:X} DR5={dr5:X} DR6={dr6:X} DR7={dr7:X}", ctx.Dr0, ctx.Dr1, ctx.Dr2, ctx.Dr3, ctx.Dr4, ctx.Dr5, ctx.Dr6, ctx.Dr7);
            return ctx.Dr0 != 0x00 || ctx.Dr1 != 0x00 || ctx.Dr2 != 0x00 || ctx.Dr3 != 0x00 || ctx.Dr4 != 0x00 || ctx.Dr5 != 0x00 || ctx.Dr6 != 0x00 || ctx.Dr7 != 0x00;
        }
    }
}
