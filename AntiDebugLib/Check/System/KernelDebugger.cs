using System.Runtime.InteropServices;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// Check if the kernel debugger is present by calling <c>ntdll!NtQuerySystemInformation</c> with <c>SystemKernelDebuggerInformation</c>.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/OtherChecks.cs#L52
    /// </summary>
    internal class KernelDebugger : CheckBase
    {
        public override string Name => "Kernel debugger";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private const uint SystemKernelDebuggerInformation = 0x23;

        public override bool CheckPassive()
        {
            var KernelDebugInfo = new SYSTEM_KERNEL_DEBUGGER_INFORMATION
            {
                KernelDebuggerEnabled = false,
                KernelDebuggerNotPresent = true
            };

            NtQuerySystemInformation_KernelDebuggerInfo(SystemKernelDebuggerInformation, ref KernelDebugInfo, (uint)Marshal.SizeOf(KernelDebugInfo), out var returnLength);

            return returnLength == (uint)Marshal.SizeOf(KernelDebugInfo)
                && (KernelDebugInfo.KernelDebuggerEnabled || !KernelDebugInfo.KernelDebuggerNotPresent);
        }
    }
}
