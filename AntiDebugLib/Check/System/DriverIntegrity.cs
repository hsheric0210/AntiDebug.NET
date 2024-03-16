using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeDefs;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.System
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/OtherChecks.cs#L20
    /// </item>
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/OtherChecks.cs#L36
    /// </item>
    /// </list>
    /// </summary>
    internal class DriverIntegrity : CheckBase
    {
        public override string Name => "Driver integrity options";

        public override CheckReliability Reliability => CheckReliability.Great;

        private const uint SystemCodeIntegrityInformation = 0x67;
        private const uint CODEINTEGRITY_OPTION_ENABLED = 0x01;
        private const uint CODEINTEGRITY_OPTION_TESTSIGN = 0x02;

        public override CheckResult CheckPassive()
        {
            var CodeIntegrityInfo = new SYSTEM_CODEINTEGRITY_INFORMATION { Length = (uint)Marshal.SizeOf(typeof(SYSTEM_CODEINTEGRITY_INFORMATION)) };

            var status = NtQuerySystemInformation_CodeIntegrityInfo(SystemCodeIntegrityInformation, ref CodeIntegrityInfo, (uint)Marshal.SizeOf(CodeIntegrityInfo), out var returnLength);
            if (!NT_SUCCESS(status))
            {
                Logger.Warning("Unable to query SystemCodeIntegrityInformation system information. NtQuerySystemInformation returned NTSTATUS {status}.", status);
                return NtError("NtQuerySystemInformation", status);
            }

            var expectedReturnLength = (uint)Marshal.SizeOf(CodeIntegrityInfo);
            if (returnLength != expectedReturnLength)
            {
                Logger.Warning("Return length mismatched. Expected {expected}, got {actual}.", expectedReturnLength, returnLength);
                return DebuggerDetected(new { Expected = expectedReturnLength, Actual = returnLength });
            }

            Logger.Debug("SYSTEM_CODEINTEGRITY_INFORMATION->CodeIntegrityOptions is {value:X}.", CodeIntegrityInfo.CodeIntegrityOptions);
            if ((CodeIntegrityInfo.CodeIntegrityOptions & CODEINTEGRITY_OPTION_ENABLED) != 0 && (CodeIntegrityInfo.CodeIntegrityOptions & CODEINTEGRITY_OPTION_TESTSIGN) == 0)
                return DebuggerNotDetected();

            return DebuggerDetected(new { Flags = CodeIntegrityInfo.CodeIntegrityOptions });
        }
    }
}
