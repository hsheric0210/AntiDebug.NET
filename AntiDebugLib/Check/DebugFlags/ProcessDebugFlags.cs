using System.Diagnostics;

using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet/blob/91872f71c5601e4b037b713f31327dfde1662481/AntiCrack-DotNet/AntiDebug.cs
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtQueryInformationProcess_ProcessDebugFlags.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class ProcessDebugFlags : CheckBase
    {
        public override string Name => "NtQueryInformationProcess: ProcessDebugFlags";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckActive()
        {
            const uint ProcessDebugFlags = 0x1F; // https://ntdoc.m417z.com/processinfoclass
            NtQueryInformationProcess_uint(Process.GetCurrentProcess().SafeHandle, ProcessDebugFlags, out var flag, sizeof(uint), 0);
            return flag == 0;
        }
    }
}
