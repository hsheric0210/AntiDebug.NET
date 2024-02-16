using System;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.Exploits
{
    /// <summary>
    /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_HandlesValidation.cpp#L4
    /// https://ezbeat.tistory.com/219
    /// </summary>
    public class OpenCsrss : CheckBase
    {
        public override string Name => "Open csrss.exe";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        public override bool CheckPassive()
        {
            try
            {
                const uint PROCESS_ALL_ACCESS = 0x000F0000 | 0x00100000 | 0xFFFF; // winnt.h
                var handle = OpenProcess(PROCESS_ALL_ACCESS, false, CsrGetProcessId());
                if (handle != IntPtr.Zero)
                {
                    NtClose(handle);
                    return true;
                }
            }
            catch
            {
                Logger.Error("Error calling CsrGetProcessId and OpenProcess. It is likely to something is blocking the call.");
                return true;
            }

            return false;
        }
    }
}
