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

                uint heapFlags;
                if (Pointer.Is64Bit)
                {
                    var debug = Marshal.PtrToStructure<RTL_DEBUG_INFORMATION>(buffer);
                    var debug2 = Marshal.PtrToStructure<DEBUG_BUFFER>(buffer);
                    Console.WriteLine($"debug buffer @ {buffer} and the heapinfo is point to RDI {(Pointer)debug.HeapInformation} or DB {(Pointer)debug2.HeapInformation}");
                    heapFlags = Marshal.PtrToStructure<RTL_HEAP_INFORMATION>(buffer + debug.HeapInformation + Pointer.Size).Flags; // 8: RTL_PROCESS_HEAPS.NumberOfHeaps
                }
                else
                {
                    var debug = Marshal.PtrToStructure<RTL_DEBUG_INFORMATION>(buffer);
                    var debug2 = Marshal.PtrToStructure<DEBUG_BUFFER>(buffer);
                    Console.WriteLine($"debug buffer @ {buffer} and the heapinfo is point to RDI {(Pointer)debug.HeapInformation} or DB {(Pointer)debug2.HeapInformation}");
                    heapFlags = Marshal.PtrToStructure<RTL_HEAP_INFORMATION>((Pointer)debug.HeapInformation).Flags; // https://evilcodecave.wordpress.com/tag/pdebug_buffer/
                }

                Console.WriteLine($"Flags is {heapFlags:X}");

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
