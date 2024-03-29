﻿using AntiDebugLib.Native;
using System;

using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.Handle
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
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/SeDebugPrivilege.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.B.i. OpenProcess
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

        public override CheckResult CheckPassive()
        {
            try
            {
                var handle = Kernel32.OpenProcess(PROCESS_ALL_ACCESS, false, CsrGetProcessId());
                if (handle != IntPtr.Zero)
                {
                    Logger.Debug("CSRSS successfully opened. Handle {handle:X}.", handle.ToHex());
                    Kernel32.CloseHandle(handle);
                    return DebuggerDetected(new { Handle = handle });
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Error calling CsrGetProcessId and OpenProcess. It is likely to something is blocking the call.");
                return DebuggerDetected();
            }

            return DebuggerNotDetected();
        }
    }
}
