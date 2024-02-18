using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeStructs;
using static AntiDebugLib.Native.NtDll;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// <list type="bullet">
    /// <item>
    /// Blackhat 2012 :: 
    /// </item>
    /// <item>
    /// The "Ultimate" Anti-Debugging Reference (by Peter Ferrie) :: 7.D.x. RtlQueryProcessHeapInformation
    /// </item>
    /// </list>
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
