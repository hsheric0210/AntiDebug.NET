using System.Runtime.InteropServices;
using System;

using static AntiDebugLib.Native.AntiDebugLibNative;

namespace AntiDebugLib.Native
{
    internal static partial class NativeDefs
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
    }
}
