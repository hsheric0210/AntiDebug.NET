using System.Runtime.InteropServices;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_DebugFlags.cpp#L149
    /// </summary>
    public class HeapFlagsRtlQueryProcessHeapInformation : CheckBase
    {
        public override string Name => "Heap Flags: RtlQueryProcessHeapInformation";

        public override CheckReliability Reliability => CheckReliability.Okay;

        private const uint HEAP_GROWABLE = 0x00000002;

        public override bool CheckActive()
        {
            var buffer = RtlCreateQueryDebugBuffer(0, false);
            if (RtlQueryProcessHeapInformation(buffer) != 0)
                return false;

            var debug = Marshal.PtrToStructure<DEBUG_BUFFER>(buffer);
            var heapDebug = Marshal.PtrToStructure<DEBUG_HEAP_INFORMATION>(debug.HeapInformation);
            return (heapDebug.Flags & ~HEAP_GROWABLE) != 0;
        }
    }
}
