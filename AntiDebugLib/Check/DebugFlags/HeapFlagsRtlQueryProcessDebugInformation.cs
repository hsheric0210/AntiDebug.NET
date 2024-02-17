using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeStructs;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Blackhat 2012 :: https://github.com/rrbranco/blackhat2012/blob/025cd065fc3144c0609e6ccd5414e30750e5cb6b/Csrc/fcall_examples/fcall_examples/fcall_examples.cpp#L242
    /// </item>
    /// </list>
    /// </summary>
    public class HeapFlagsRtlQueryProcessDebugInformation : CheckBase
    {
        public override string Name => "Heap Flags: RtlQueryProcessDebugInformation";

        public override CheckReliability Reliability => CheckReliability.Okay;

        private const uint HEAP_GROWABLE = 0x00000002;

        public override bool CheckPassive()
        {
            var buffer = RtlCreateQueryDebugBuffer(0, false);
            if (RtlQueryProcessHeapInformation(buffer) != 0)
                return false;

            var debug = Marshal.PtrToStructure<DEBUG_BUFFER>(buffer);
            var heapFlags = Marshal.PtrToStructure<DEBUG_HEAP_INFORMATION>(debug.HeapInformation + 1).Flags;
            RtlDestroyQueryDebugBuffer(buffer);

            return (debug.RemoteSectionBase.ToInt64() & ~HEAP_GROWABLE) != 0 || (heapFlags & ~HEAP_GROWABLE) != 90;
        }
    }
}
