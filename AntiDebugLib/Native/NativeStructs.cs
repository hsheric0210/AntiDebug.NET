using System.Runtime.InteropServices;
using System;

namespace AntiDebugLib.Native
{
    internal static partial class NativeStructs
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
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct DEBUG_HEAP_INFORMATION
        {
            public uint Base; // 0×00
            public uint Flags; // 0×04
            public ushort Granularity; // 0×08
            public ushort Unknown; // 0x0A
            public uint Allocated; // 0x0C
            public uint Committed; // 0×10
            public uint TagCount; // 0×14
            public uint BlockCount; // 0×18
            public fixed uint Reserved[7]; // 0x1C
            public IntPtr Tags; // 0×38
            public IntPtr Blocks; // 0x3C
        }
    }
}
