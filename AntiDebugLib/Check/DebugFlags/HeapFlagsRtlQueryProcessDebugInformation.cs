using StealthModule;
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

        public override CheckResult CheckActive()
        {
            var buffer = Pointer.Zero;

            try
            {
                buffer = RtlCreateQueryDebugBuffer(0, false);
                if (buffer == Pointer.Zero)
                {
                    Logger.Warning("Unable to allocate debug buffer.");
                    return Error(new { Function = "RtlCreateQueryDebugBuffer" });
                }

                var status = RtlQueryProcessDebugInformation((uint)Process.GetCurrentProcess().Id, PDI_HEAPS | PDI_HEAP_BLOCKS, buffer);
                if (!NT_SUCCESS(status))
                {
                    Logger.Warning("Unable to query process debug information. RtlQueryProcessDebugInformation returned NTSTATUS {status}.", status);
                    return NtError("RtlQueryProcessDebugInformation", status);
                }

                var heapInformationOffset = Pointer.Is64Bit ? 0x70 : 0x38; // I found this address BY MYSELF (by comparing the memory dump and address values)
                var heapInformation = (Pointer)Marshal.ReadIntPtr(buffer + heapInformationOffset);

                var flagsOffset = Pointer.Size * 2; // Skip two ptrs
                var heapFlagsAddress = heapInformation + flagsOffset;
                var heapFlags = Marshal.ReadInt32(heapFlagsAddress);

                Logger.Debug("Heap Flags: {flags:X}", heapFlags);
                if ((heapFlags & ~HEAP_GROWABLE) == 0)
                    return DebuggerNotDetected();

                return DebuggerDetected(new { Flags = heapFlags });
            }
            finally
            {
                if (buffer != Pointer.Zero)
                {
                    var status = RtlDestroyQueryDebugBuffer(buffer);
                    if (!NT_SUCCESS(status))
                        Logger.Warning("Unable to destroy debug buffer. RtlDestroyQueryDebugBuffer returned NTSTATUS {status}.", status);
                }
            }
        }
    }
}
