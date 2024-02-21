// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "GetProcAddressSilent.h"
#include "check_assembler.h"
#include "check_exception.h"
#include "check_memory.h"
#include "check_timing.h"

#define DLLEXPORT __declspec(dllexport)

extern "C" {
    // Feel free to rename this function, but I'd recommend to use 'RenameNativeFunctions.ps1'.
    // (It will automatically find markers and rename function names in both C++ and C# side)

    DLLEXPORT ULONGLONG WINAPI /*<c_entrypoint>*/AD43568293496/*</c_entrypoint>*/()
    {
        ULONGLONG value = 0ULL;
        UINT16 i = 0;


        if (check_assembler_int3()) value |= 1ULL << 0;
        if (check_assembler_int3long()) value |= 1ULL << 1;
        if (check_assembler_int2d()) value |= 1ULL << 2;
        if (check_assembler_icebp()) value |= 1ULL << 3;
        if (check_assembler_stack_segment_register()) value |= 1ULL << 4;
        if (check_assembler_instruction_counting()) value |= 1ULL << 5;
        if (check_assembler_popf_and_trap()) value |= 1ULL << 6;
        if (check_assembler_instruction_prefixes()) value |= 1ULL << 7;
        if (check_assembler_debug_registers_modification()) value |= 1ULL << 8;

        if (check_exception_seh()) value |= 1ULL << 9;
        if (check_exception_unhandledexceptionfilter()) value |= 1ULL << 10;
        if (check_exception_raiseexception()) value |= 1ULL << 11;
        if (check_exception_veh()) value |= 1ULL << 12;
        if (check_exception_trapflag()) value |= 1ULL << 13;

        if (check_timing_rdtsc_diff_locky()) value |= 1ULL << 14;
        if (check_timing_rdtsc_diff_vmexit()) value |= 1ULL << 15;

        if (check_memory_ntqueryvirtualmemory()) value |= 1ULL << 16;
        if (check_memory_antistepover_direct()) value |= 1ULL << 17;
        if (check_memory_antistepover_readfile()) value |= 1ULL << 18;
        if (check_memory_antistepover_writeprocessmemory()) value |= 1ULL << 19;

        return value;
    }

    DLLEXPORT ULONG_PTR WINAPI /*<c_getpeb>*/AD4567348905025/*</c_getpeb>*/()
    {
        return GetPEB();
    }
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    return TRUE;
}

