using System.Diagnostics;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.AntiDebugLibNative;
using static AntiDebugLib.Native.NativeStructs;
using static AntiDebugLib.Native.Kernel32;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/debug-flags.html#manual-checks-heap-flags
    /// </item>
    /// <item>
    /// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_DebugFlags.cpp#L149
    /// </item>
    /// <item>
    /// al-khaser (Flags) :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/ProcessHeap_Flags.cpp
    /// </item>
    /// <item>
    /// al-khaser (ForceFlags) :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/ProcessHeap_ForceFlags.cpp
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 2. Heap flags
    /// </item>
    /// </list>
    /// </summary>
    public class HeapFlagsPeb : CheckBase
    {
        public override string Name => "Heap Flags: PEB";

        public override CheckReliability Reliability => CheckReliability.Okay;

        private const uint HEAP_GROWABLE = 0x00000002;

        public override bool CheckActive()
        {
            // Don't care if the OS version is lower than Vista

            if (IsWow64Process2(Process.GetCurrentProcess().Handle, out var processMachine, out _))
                Logger.Error("IsWow64Process2 failure. Win32 error {w32err}", Marshal.GetLastWin32Error());

            var isWow64 = processMachine != 0; // IMAGE_FILE_MACHINE_UNKNOWN
            var heapStruct = isWow64 ? Marshal.PtrToStructure<_HEAP>(GetPeb() + 0x1030) : Marshal.PtrToStructure<_HEAP>(_PEB.ParsePeb().ProcessHeap);
            return (heapStruct.Flags & ~HEAP_GROWABLE) != 0 || heapStruct.ForceFlags != 0;
        }
    }
}
