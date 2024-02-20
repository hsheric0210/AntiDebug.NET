using System.Runtime.InteropServices;
using System;

namespace AntiDebugLib.Native
{
    internal static partial class NativeDefs
    {
        public const uint STATUS_INFO_LENGTH_MISMATCH = 0xC0000004;

        [StructLayout(LayoutKind.Sequential)]
        public struct CONTEXT
        {
            public uint P1Home;
            public uint P2Home;
            public uint P3Home;
            public uint P4Home;
            public uint P5Home;
            public uint P6Home;
            public long ContextFlags;
            public uint Dr0;
            public uint Dr1;
            public uint Dr2;
            public uint Dr3;
            public uint Dr4;
            public uint Dr5;
            public uint Dr6;
            public uint Dr7;
        }

        public struct PROCESS_MITIGATION_BINARY_SIGNATURE_POLICY
        {
            public uint MicrosoftSignedOnly;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct SYSTEM_CODEINTEGRITY_INFORMATION
        {
            [FieldOffset(0)]
            public ulong Length;

            [FieldOffset(4)]
            public uint CodeIntegrityOptions;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_BASIC_INFORMATION
        {
            internal IntPtr Reserved1;
            internal IntPtr PebBaseAddress;
            internal IntPtr Reserved2_0;
            internal IntPtr Reserved2_1;
            internal IntPtr UniqueProcessId;
            internal IntPtr InheritedFromUniqueProcessId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_KERNEL_DEBUGGER_INFORMATION
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool KernelDebuggerEnabled;

            [MarshalAs(UnmanagedType.U1)]
            public bool KernelDebuggerNotPresent;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_TYPE_INFORMATION
        {
            public UNICODE_STRING TypeName;
            public uint TotalNumberOfHandles;
            public uint TotalNumberOfObjects;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_ALL_INFORMATION
        {
            public uint NumberOfObjects;
            public OBJECT_TYPE_INFORMATION[] ObjectTypeInformation;
        }

        // https://github.com/rrbranco/blackhat2012/blob/master/Csrc/fcall_examples/fcall_examples/defs.h
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct DEBUG_BUFFER
        {
            public IntPtr SectionHandle;
            public IntPtr SectionBase;
            public IntPtr RemoteSectionBase;
            public uint SectionBaseDelta;
            public IntPtr EventPairHandle;
            public fixed uint Unknown[2];
            public IntPtr RemoteThreadHandle;
            public uint InfoClassMask;
            public uint SizeOfInfo;
            public uint AllocatedSize;
            public uint SectionSize;
            public IntPtr ModuleInformation;
            public IntPtr BackTraceInformation;
            public IntPtr HeapInformation;
            public IntPtr LockInformation;
            public IntPtr Reserved1;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr Reserved4;
            public IntPtr Reserved5;
            public IntPtr Reserved6;
            public IntPtr Reserved7;
            public IntPtr Reserved8;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RTL_PROCESS_HEAPS
        {
            public uint NumberOfHeaps;
            public RTL_HEAP_INFORMATION[] Heaps;
        }

        /// <summary>
        /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/NtDll.h#L101
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RTL_HEAP_INFORMATION
        {
            public IntPtr BaseAddress;
            public uint Flags;
            public ushort EntryOverhead;
            public ushort CreatorBackTraceIndex;
            public uint BytesAllocated;
            public uint BytesCommitted;
            public uint NumberOfTags;
            public uint NumberOfEntries;
            public uint NumberOfPseudoTags;
            public uint PseudoTagGranularity;
            public fixed uint Reserved[5];
            public IntPtr Tags;
            public IntPtr Entries;
        }
    }
}
