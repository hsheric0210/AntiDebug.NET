using System.Runtime.InteropServices;

using static AntiDebugLib.Native.AntiDebugLibNative;
using static AntiDebugLib.Native.NativeDefs;
using System;

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

        private bool Check(_HEAP heap)
        {
            Logger.Debug("Heap Flags: {flags:X}, ForceFlags: {forceFlags:X}", heap.Flags, heap.ForceFlags);
            return (heap.Flags & ~HEAP_GROWABLE) != 0 || heap.ForceFlags != 0;
        }

        public override bool CheckActive()
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess) // WOW64
            {
                var wow64heap = GetPeb() + 0x1030;
                Logger.Debug("WOW64 _HEAP is located at {address:X}.", wow64heap.ToInt64());

                if (Check(Marshal.PtrToStructure<_HEAP>(wow64heap)))
                    return true;
            }

            var heapAddress = _PEB.ParsePeb().ProcessHeap;
            Logger.Debug("_HEAP address is located at {address:X}.", heapAddress.ToInt64());
            return Check(Marshal.PtrToStructure<_HEAP>(heapAddress));
        }
    }
}
