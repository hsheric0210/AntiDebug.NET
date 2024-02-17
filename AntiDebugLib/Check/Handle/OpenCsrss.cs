using System;

using static AntiDebugLib.Native.Kernel32;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.Exploits
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/object-handles.html#openprocess
    /// </item>
    /// <item>
    /// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_HandlesValidation.cpp#L4
    /// </item>
    /// <item>
    /// https://ezbeat.tistory.com/219
    /// </item>
    /// </list>
    /// </summary>
    public class OpenCsrss : CheckBase
    {
        public override string Name => "OpenProcess (try opening csrss)";

        public override CheckReliability Reliability => CheckReliability.Perfect;

        private const uint PROCESS_ALL_ACCESS = 0x000F0000 | 0x00100000 | 0xFFFF; // winnt.h

        public override bool CheckPassive()
        {
            try
            {
                var handle = OpenProcess(PROCESS_ALL_ACCESS, false, CsrGetProcessId());
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error calling CsrGetProcessId and OpenProcess. It is likely to something is blocking the call.");
                return true;
            }

            return false;
        }
    }
}
