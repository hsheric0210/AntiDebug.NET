﻿using System.Runtime.InteropServices;

using static AntiDebugLib.Native.AntiDebugLibNative;
using static AntiDebugLib.Native.NativeDefs;
using System;
using AntiDebugLib.Native;

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

        private CheckResult Check(int flags, int forceFlags)
        {
            Logger.Debug("Heap Flags: {flags:X}, ForceFlags: {forceFlags:X}", flags, forceFlags);
            if ((flags & ~HEAP_GROWABLE) != 0 || forceFlags != 0)
                return DebuggerDetected(new { Flags = flags, ForceFlags = forceFlags });

            return DebuggerNotDetected();
        }

        public override CheckResult CheckActive()
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess) // WOW64
            {
                var wow64heap = GetPeb() + 0x1030;
                Logger.Debug("WOW64 _HEAP is located at {address:X}.", wow64heap.ToHex());

                var result = Check(Marshal.ReadInt32(wow64heap + 0x40), Marshal.ReadInt32(wow64heap + 0x44));
                if (result.IsDetected)
                    return result;
            }

            var heapAddress = _PEB.ParsePeb().ProcessHeap;
            Logger.Debug("_HEAP address is located at {address:X}.", heapAddress.ToHex());
            var flagsOffset = Environment.Is64BitProcess ? 0x70 : 0x40;
            var forceFlagsOffset = Environment.Is64BitProcess ? 0x74 : 0x44;
            return Check(Marshal.ReadInt32(heapAddress + flagsOffset), Marshal.ReadInt32(heapAddress + forceFlagsOffset));
        }
    }
}
