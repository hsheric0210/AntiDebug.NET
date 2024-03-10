using System;
using System.Diagnostics;

using static AntiDebugLib.Native.NativeDefs;
using static AntiDebugLib.Native.NtDll;
using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L126
    /// </item>
    /// <item>
    /// Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection/blob/585e33cbe57aa97725b3f98658944b01f1844562/src/Flags/ProcessDebugPortFlag.cs#L13
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtQueryInformationProcess_ProcessDebugPort.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.D.viii.a. NtQueryInformationProcess ProcessDebugPort
    /// </item>
    /// </list>
    /// </summary>
    public class ProcessDebugPort : CheckBase
    {
        public override string Name => "NtQueryInformationProcess: ProcessDebugPort";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override CheckResult CheckActive()
        {
            const uint ProcessDebugPort = 0x7; // https://ntdoc.m417z.com/processinfoclass
            var size = (uint)(sizeof(uint) * (Environment.Is64BitProcess ? 2 : 1));
            var status = NtQueryInformationProcess_uint(GetCurrentProcess(), ProcessDebugPort, out var port, size, out _);
            if (!NT_SUCCESS(status))
            {
                Logger.Warning("Unable to query ProcessDebugPort process information. NtQueryInformationProcess returned NTSTATUS {status}.", status);
                return NtError("NtQueryInformationProcess", status);
            }

            Logger.Debug("ProcessDebugPort is {value}.", port);
            if (port == 0)
                return DebuggerNotDetected();

            return DebuggerDetected(new { Port = port });
        }
    }
}
