using System.Runtime.InteropServices;
using System;

namespace AntiDebugLib
{
    internal static partial class NativeCalls
    {
        /// <summary>
        /// https://www.geoffchappell.com/studies/windows/km/ntoskrnl/inc/api/pebteb/peb/index.htm
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct _PEB
        {
            public byte InheritedAddressSpace;
            public byte ReadImageFileExecOptions;
            public byte BeingDebugged;
            public byte SpareBool;
            public IntPtr Mutant;
            public IntPtr ImageBaseAddress;
            public IntPtr Ldr;
            public IntPtr ProcessParameters;
            public IntPtr SubSystemData;
            public IntPtr ProcessHeap;
            public IntPtr FastPebLock;
            public IntPtr AtlThunkSListPtr; // FastPebLockRoutine
            public IntPtr IFEOKey; // FastPebUnlockRoutine
            public uint CrossProcessFlags; // EnvironmentUpdateCount
            public IntPtr KernelCallbackTable;
            public uint SystemReserved;
            public uint AtlThunkSListPtr32;
            public IntPtr ApiSetMap;
            public uint TlsExpansionCounter;
            public IntPtr TlsBitmap;
            public fixed uint TlsBitmapBits[2];
            public IntPtr ReadOnlySharedMemoryBase;
            public IntPtr SharedData;
            public IntPtr ReadOnlyStaticServerData;
            public IntPtr AnsiCodePageData;
            public IntPtr OemCodePageData;
            public IntPtr UnicodeCaseTableData;
            public uint NumberOfProcessors;
            public uint NtGlobalFlag;

            public static _PEB ParsePeb() => Marshal.PtrToStructure<_PEB>(GetPeb());
        }

        /// <summary>
        /// Below Vista: https://systemroot.gitee.io/pages/apiexplorer/d5/d5/struct__HEAP.html#o2
        /// Vista or Later: https://www.nirsoft.net/kernel_struct/vista/HEAP.html
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct _HEAP
        {
            public _HEAP_ENTRY Entry;
            public uint SegmentSignature;
            public uint SegmentFlags;
            public _LIST_ENTRY SegmentListEntry;
            public IntPtr Heap;
            public IntPtr BaseAddress;
            public uint NumberOfPages;
            public IntPtr FirstEntry;
            public IntPtr LastValidEntry;
            public uint NumberOfUnCommittedPages;
            public uint NumberOfUnCommittedRanges;
            public ushort SegmentAllocatorBackTraceIndex;
            public ushort Reserved;
            public _LIST_ENTRY UCRSegmentList;
            public uint Flags;
            public uint ForceFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct _HEAP_ENTRY
        {
            public ushort Size;
            public ushort Flags;
            public ushort SmallTagIndex;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct _LIST_ENTRY
        {
            public IntPtr Flink; // Forward Link
            public IntPtr Blink; // Backward Link
        }
    }
}
