using StealthModule;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;

using static AntiDebugLib.Native.NativeDefs;
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

        [HandleProcessCorruptedStateExceptions]
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

                var status = RtlQueryProcessHeapInformation(buffer);
                if (!NT_SUCCESS(status))
                {
                    Logger.Warning("Unable to query process heap information. RtlQueryProcessHeapInformation returned NTSTATUS {status}.", status);
                    return NtError("RtlQueryProcessHeapInformation", status);
                }

                var heapInformationOffset = Pointer.Is64Bit ? 0x70 : 0x38; // I found this address BY MYSELF (by comparing the memory dump and address values)
                var heapInformation = (Pointer)Marshal.ReadIntPtr(buffer + heapInformationOffset);

                var flagsOffset = Pointer.Size * 2; // Skip 8 bytes
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
