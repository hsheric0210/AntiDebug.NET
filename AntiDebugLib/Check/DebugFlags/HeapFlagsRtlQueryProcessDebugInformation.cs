using System.Diagnostics;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeStructs;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Blackhat 2012 :: https://github.com/rrbranco/blackhat2012/blob/025cd065fc3144c0609e6ccd5414e30750e5cb6b/Csrc/fcall_examples/fcall_examples/fcall_examples.cpp#L217
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.D.xii. RtlQueryProcessDebugInformation
    /// </item>
    /// </list>
    /// </summary>
    public class HeapFlagsRtlQueryProcessDebugInformation : CheckBase
    {
        public override string Name => "Heap Flags: RtlQueryProcessDebugInformation";

        public override CheckReliability Reliability => CheckReliability.Okay;

        private const uint PDI_HEAPS = 0x04;
        private const uint PDI_HEAP_BLOCKS = 0x10;

        private const uint HEAP_GROWABLE = 0x00000002;

        public override bool CheckPassive()
        {
            var buffer = RtlCreateQueryDebugBuffer(0, false);
            if (RtlQueryProcessDebugInformation((uint)Process.GetCurrentProcess().Id, PDI_HEAPS | PDI_HEAP_BLOCKS, buffer) != 0)
                return false;

            var debug = Marshal.PtrToStructure<DEBUG_BUFFER>(buffer);
            var heapFlags = Marshal.PtrToStructure<DEBUG_HEAP_INFORMATION>(debug.HeapInformation + 1).Flags;
            RtlDestroyQueryDebugBuffer(buffer);

            return (heapFlags & ~HEAP_GROWABLE) != 0;
        }
    }
}
