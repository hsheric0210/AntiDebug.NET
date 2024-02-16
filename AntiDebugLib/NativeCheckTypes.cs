using System;

namespace AntiDebugLib
{
    /// <summary>
    /// AntiDebugLib.Native README.md 참조
    /// </summary>
    [Flags]
    internal enum NativeCheckTypes
    {
        None = 0,
        Int3 = 1 << 0,
        Int3Long = 1 << 1,
        Int2D = 1 << 2,
        IceBp = 1 << 3,
        StackSegmentRegister = 1 << 4,
        InstructionCounting = 1 << 5,
        PopfTrap = 1 << 6,
        InstructionPrefix = 1 << 7,
        DebugRegisterManipulation = 1 << 8,
        UnhandledSeh = 1 << 9,
        RaiseException = 1 << 10,
        Veh = 1 << 11,
        NtQueryVirtualMemory = 1 << 12,
        CodeChecksum = 1 << 13,
        RdtscTimeDelta = 1 << 14,
        RdtscCpuid = 1 << 15,
    }
}
