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

        public override CheckResult CheckActive()
        {
            var ctx = new CONTEXT { ContextFlags = CONTEXT_DEBUG_REGISTERS };

            if (!GetThreadContext(GetCurrentThread(), ref ctx))
            {
                Logger.Warning("Unable to get the current thread context. GetThreadContext returned win32 error {error}.", Marshal.GetLastWin32Error());
                return Win32Error("GetThreadContext");
            }

            // https://en.wikipedia.org/wiki/X86_debug_register
            // DR4=DR6, DR5=DR7
            Logger.Debug("Current thread debug register values: DR0={dr0:X} DR1={dr1:X} DR2={dr2:X} DR3={dr3:X} DR6={dr6:X} DR7={dr7:X}", ctx.Dr0, ctx.Dr1, ctx.Dr2, ctx.Dr3, ctx.Dr6, ctx.Dr7);
            if (ctx.Dr0 != 0x00 || ctx.Dr1 != 0x00 || ctx.Dr2 != 0x00 || ctx.Dr3 != 0x00 || ctx.Dr6 != 0x00 || ctx.Dr7 != 0x00)
                return DebuggerDetected(new { DR0 = ctx.Dr0, DR1 = ctx.Dr1, DR2 = ctx.Dr2, DR3 = ctx.Dr3, DR6 = ctx.Dr6, DR7 = ctx.Dr7 });

            return DebuggerNotDetected();
        }
    }
}
