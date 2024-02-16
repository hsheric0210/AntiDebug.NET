using System.Diagnostics;
using System.Runtime.InteropServices;
using static AntiDebugLib.NativeCalls;

namespace AntiDebugLib.Check.DebugFlags
{
    /// <summary>
    /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_DebugFlags.cpp#L149
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
