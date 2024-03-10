using System.Runtime.InteropServices;
using System;

namespace AntiDebugLib.Native
{
    internal static partial class NativeDefs
    {
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
            internal IntPtr ExitStatus;
            internal IntPtr PebBaseAddress;
            internal IntPtr AffinityMask;
            internal IntPtr BasePriority;
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
            public int TotalNumberOfObjects;
            public int TotalNumberOfHandles;
            public int TotalPagedPoolUsage;
            public int TotalNonPagedPoolUsage;
            public int TotalNamePoolUsage;
            public int TotalHandleTableUsage;
            public int HighWaterNumberOfObjects;
            public int HighWaterNumberOfHandles;
            public int HighWaterPagedPoolUsage;
            public int HighWaterNonPagedPoolUsage;
            public int HighWaterNamePoolUsage;
            public int HighWaterHandleTableUsage;
            public int InvalidAttributes;
            public GENERIC_MAPPING GenericMapping;
            public int ValidAccessMask;
            public byte SecurityRequired;
            public byte MaintainHandleCount;
            public int PoolType;
            public int DefaultPagedPoolCharge;
            public int DefaultNonPagedPoolCharge;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OBJECT_ALL_INFORMATION
        {
            public uint NumberOfObjects;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            public OBJECT_TYPE_INFORMATION[] ObjectTypeInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GENERIC_MAPPING
        {
            private int GenericRead;
            private int GenericWrite;
            private int GenericExecute;
            private int GenericAll;
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

        /// <summary>
        /// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/NtDll.h#L164
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct RTL_DEBUG_INFORMATION
        {
            public IntPtr SectionHandle;
            public IntPtr SectionBase;
            public IntPtr RemoteSectionBase;
            public uint SectionBaseDelta;
            public IntPtr EventPairHandle;
            public IntPtr EventPairTarget;
            public IntPtr TargetProcessId;
            public IntPtr TargetThreadHandle;
            public uint Flags;
            public uint OffsetFree;
            public uint CommitSize;
            public uint ViewSize;
            public IntPtr ModuleInformation;
            public IntPtr BackTraceInformation;
            public IntPtr HeapInformation;
            public IntPtr LockInformation;
            public IntPtr SpecificHeap;
            public IntPtr TargetProcessHandle;
            public IntPtr Reserved3;
            public IntPtr Reserved4;
            public IntPtr Reserved5;
            public IntPtr Reserved6;
            public IntPtr Reserved7;
            public IntPtr Reserved8;
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

        public enum MemoryProtection : uint
        {
            EXECUTE = 0x10,
            EXECUTE_READ = 0x20,
            EXECUTE_READWRITE = 0x40,
            EXECUTE_WRITECOPY = 0x80,
            NOACCESS = 0x01,
            READONLY = 0x02,
            READWRITE = 0x04,
            WRITECOPY = 0x08,
            GUARD_Modifierflag = 0x100,
            NOCACHE_Modifierflag = 0x200,
            WRITECOMBINE_Modifierflag = 0x400
        }
    }
}
