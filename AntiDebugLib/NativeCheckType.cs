namespace AntiDebugLib
{
    public enum NativeCheckType : uint
    {
        Assembler_Int3 = 0,
        Assembler_Int3Long = 1,
        Assembler_Int2D = 2,
        Assembler_IceBP = 3,
        Assembler_StackSegmentRegister = 4,
        Assembler_InstructionCounting = 5,
        Assembler_PopfAndTrap = 6,
        Assembler_InstructionPrefixes = 7,
        Assembler_DebugRegisterModification = 8,
        Exception_SEH = 9,
        Exception_UnhandledExceptionFilter = 10,
        Exception_RaiseException = 11,
        Exception_VEH = 12,
        Exception_TrapFlag = 13,
        Timing_Rdtsc_Locky = 14,
        Timing_Rdtsc_VmExit = 15,
        Memory_WorkingSetShare = 16,
        Memory_AntiStepOver_Direct = 17,
        Memory_AntiStepOver_ReadFile = 18,
        Memory_AntiStepOver_WriteProcessMemory = 19,
    }
}
