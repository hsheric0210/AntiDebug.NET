using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeDefs;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/OtherChecks.cs#L52
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtQuerySystemInformation_SystemKernelDebuggerInformation.cpp
    /// </item>
    /// </list>
    /// </summary>
    internal class KernelDebugger : CheckBase
    {
        public override string Name => "Kernel debugger";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private const uint SystemKernelDebuggerInformation = 0x23;

        public override CheckResult CheckPassive()
        {
            var KernelDebugInfo = new SYSTEM_KERNEL_DEBUGGER_INFORMATION
            {
                KernelDebuggerEnabled = false,
                KernelDebuggerNotPresent = true
            };

            var status = NtQuerySystemInformation_KernelDebuggerInfo(SystemKernelDebuggerInformation, ref KernelDebugInfo, (uint)Marshal.SizeOf(KernelDebugInfo), out var returnLength);
            if (!NT_SUCCESS(status))
            {
                Logger.Warning("Unable to query SystemKernelDebuggerInformation system information. NtQuerySystemInformation returned NTSTATUS {status}.", status);
                NtError("NtQuerySystemInformation", status);
            }
            var expectedReturnLength = (uint)Marshal.SizeOf(KernelDebugInfo);
            if (returnLength != expectedReturnLength)
            {
                Logger.Warning("Return length mismatched. Expected {expected}, got {actual}.", expectedReturnLength, returnLength);
                return DebuggerDetected(new { Expected = expectedReturnLength, Actual = returnLength });
            }

            Logger.Debug("KernelDebuggerEnabled = {enabled}, KernelDebuggerNotPresent = {notpresent}", KernelDebugInfo.KernelDebuggerEnabled, KernelDebugInfo.KernelDebuggerNotPresent);
            return MakeResult(KernelDebugInfo.KernelDebuggerEnabled || !KernelDebugInfo.KernelDebuggerNotPresent);
        }
    }
}
