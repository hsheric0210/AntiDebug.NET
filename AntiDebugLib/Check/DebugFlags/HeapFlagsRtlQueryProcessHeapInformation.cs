using System;
using System.Runtime.InteropServices;

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

        public override CheckResult CheckActive()
        {
            var buffer = IntPtr.Zero;

            try
            {
                buffer = RtlCreateQueryDebugBuffer(0, false);
                if (buffer == IntPtr.Zero)
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

                var debug = Marshal.PtrToStructure<DEBUG_BUFFER>(buffer);
                var heapFlags = Marshal.PtrToStructure<RTL_HEAP_INFORMATION>(new IntPtr(buffer.ToInt64() + debug.HeapInformation.ToInt64() + 8)).Flags; // 8: RTL_PROCESS_HEAPS.NumberOfHeaps

                Logger.Debug("Heap Flags: {flags:X}", heapFlags);
                if ((heapFlags & ~HEAP_GROWABLE) == 0)
                    return DebuggerNotDetected();

                return DebuggerDetected(new { Flags = heapFlags });
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
