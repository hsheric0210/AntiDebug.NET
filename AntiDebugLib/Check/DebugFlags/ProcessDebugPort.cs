using System;
using System.Diagnostics;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// Use <c>kernel32!NtQueryInformationProcess</c> with <c>PROCESSINFOCLASS.ProcessDebugPort</c> to detect debugger port presence.
    /// https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs#L126
    /// </summary>
    public class ProcessDebugPort : CheckBase
    {
        public override string Name => "NtQueryInformationProcess: ProcessDebugPort";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
        {
            const uint ProcessDebugPort = 0x7; // https://ntdoc.m417z.com/processinfoclass
            var size = (uint)(sizeof(uint) * (Environment.Is64BitProcess ? 2 : 1));
            NtQueryInformationProcess_uint(Process.GetCurrentProcess().SafeHandle, ProcessDebugPort, out var port, size, 0);
            return port != 0;
        }
    }
}
