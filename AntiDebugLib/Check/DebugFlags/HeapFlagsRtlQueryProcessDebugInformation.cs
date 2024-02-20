using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using static AntiDebugLib.Native.NativeDefs;
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

        public override bool CheckActive()
        {
            var buffer = IntPtr.Zero;

            try
            {
                buffer = RtlCreateQueryDebugBuffer(0, false);
                if (buffer == IntPtr.Zero)
                {
                    Logger.Warning("Unable to allocate debug buffer.");
                    return false;
                }

                var status = RtlQueryProcessDebugInformation((uint)Process.GetCurrentProcess().Id, PDI_HEAPS | PDI_HEAP_BLOCKS, buffer);
                if (!NT_SUCCESS(status))
                {
                    Logger.Warning("Unable to query process debug information. RtlQueryProcessDebugInformation returned NTSTATUS {status}.", status);
                    return false;
                }

                var debug = Marshal.PtrToStructure<DEBUG_BUFFER>(buffer);
                var heapFlags = Marshal.PtrToStructure<RTL_HEAP_INFORMATION>(new IntPtr(buffer.ToInt64() + debug.HeapInformation.ToInt64() + 8)).Flags; // 8: RTL_PROCESS_HEAPS.NumberOfHeaps

                Logger.Debug("Heap Flags: {flags:X}", heapFlags);
                return (heapFlags & ~HEAP_GROWABLE) != 0;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    var status = RtlDestroyQueryDebugBuffer(buffer);
                    if (!NT_SUCCESS(status))
                        Logger.Warning("Unable to destroy debug buffer. RtlDestroyQueryDebugBuffer returned NTSTATUS {status}.", status);
                }
            }
        }
    }
}
