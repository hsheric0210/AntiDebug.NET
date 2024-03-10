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

    DLLEXPORT ULONGLONG __stdcall /*<c_entrypoint>*/L3Hf7WkobFgc1U68E/*</c_entrypoint>*/(DWORD checkType)
    {
#pragma comment(linker, "/EXPORT:" __FUNCTION__ "=" __FUNCDNAME__)
        switch (checkType)
        {
            case 0:
                return (ULONGLONG)check_assembler_int3();
            case 1:
                return (ULONGLONG)check_assembler_int3long();
            case 2:
                return (ULONGLONG)check_assembler_int2d();
            case 3:
                return (ULONGLONG)check_assembler_icebp();
            case 4:
                return (ULONGLONG)check_assembler_stack_segment_register();
            case 5:
                return (ULONGLONG)check_assembler_instruction_counting();
            case 6:
                return (ULONGLONG)check_assembler_popf_and_trap();
            case 7:
                return (ULONGLONG)check_assembler_instruction_prefixes();
            case 8:
                return (ULONGLONG)check_assembler_debug_registers_modification();
            case 9:
                return (ULONGLONG)check_exception_seh();
            case 10:
                return (ULONGLONG)check_exception_unhandledexceptionfilter();
            case 11:
                return (ULONGLONG)check_exception_raiseexception();
            case 12:
                return (ULONGLONG)check_exception_veh();
            case 13:
                return (ULONGLONG)check_exception_trapflag();
            case 14:
                return (ULONGLONG)check_timing_rdtsc_diff_locky();
            case 15:
                return (ULONGLONG)check_timing_rdtsc_diff_vmexit();
            case 16:
                return (ULONGLONG)check_memory_ntqueryvirtualmemory();
            case 17:
                return (ULONGLONG)check_memory_antistepover_direct();
            case 18:
                return (ULONGLONG)check_memory_antistepover_readfile();
            case 19:
                return (ULONGLONG)check_memory_antistepover_writeprocessmemory();
            default:
                return (ULONGLONG)-1;
        }
    }
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    return TRUE;
}

