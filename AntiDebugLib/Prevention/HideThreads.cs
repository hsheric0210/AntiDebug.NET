using System;
using System.Diagnostics;

using static AntiDebugLib.Native.Kernel32;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Prevention
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// MinigamesAntiCheat :: https://github.com/AdvDebug/MinegamesAntiCheat/blob/60bc0894981cb531b8de4a085876e3503e9f79f0/MinegamesAntiCheat/MinegamesAntiCheat/AntiDebugging.cs#L91
    /// </item>
    /// <item>
    /// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/NtSetInformationThread_ThreadHideFromDebugger.cpp
    /// </item>
    /// </list>
    /// </summary>
    public class HideThreads : PreventionBase
    {
        public override string Name => "Hide threads from debugger";


        private const uint THREAD_SET_INFORMATION = 0x0020;

        private const uint ThreadHideFromDebugger = 0x11; // THREADINFOCLASS

        public override PreventionResult PreventActive()
        {
            var count = 0;
            foreach (ProcessThread thread in Process.GetCurrentProcess().Threads)
            {
                using (var handle = OpenThread(THREAD_SET_INFORMATION, false, thread.Id))
                {
                    var ntstatus = NtSetInformationThread(handle, ThreadHideFromDebugger, IntPtr.Zero, 0);
                    if (ntstatus == 0)
                        count++;
                    else
                        Logger.Error("Failed to hide thread {tid}. NTSTATUS {ntstatus}.", thread.Id, ntstatus);
                }
            }

            return Applied(new { Count = count });
        }
    }
}
