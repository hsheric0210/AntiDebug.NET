// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "GetProcAddressSilent.h"
#include "assembler_chk.h"
#include "exception_chk.h"
#include "memory_chk.h"
#include "timing_chk.h"

#define DLLEXPORT __declspec(dllexport)
_IMAGE_OPTIONAL_HEADER
extern "C" {
    // Feel free to rename this function, but I'd recommend to use 'RenameNativeFunctions.ps1'.
    // (It will automatically find markers and rename function names in both C++ and C# side)

    DLLEXPORT ULONGLONG WINAPI /*<c_entrypoint>*/AcmStartupObject/*</c_entrypoint>*/()
    {
        ULONGLONG value = 0ULL;
        UINT16 i = 0;

#ifndef _WIN64
        mem_code_checksum_init();
#endif

        if (asm_int3()) value |= 1ULL << 0;
        if (asm_int3_long()) value |= 1ULL << 1;
        if (asm_int2d()) value |= 1ULL << 2;
        if (asm_ice()) value |= 1ULL << 3;
        if (asm_stack_segment_register()) value |= 1ULL << 4;
        if (asm_instruction_counting()) value |= 1ULL << 5;
        if (asm_popf_and_trap()) value |= 1ULL << 6;
        if (asm_instruction_prefixes()) value |= 1ULL << 7;
        if (asm_debug_registers_modification()) value |= 1ULL << 8;

        if (exc_unhandled_seh()) value |= 1ULL << 9;
        if (exc_raiseexception()) value |= 1ULL << 10;
        if (exc_veh()) value |= 1ULL << 11;

        if (mem_ntqueryvirtualmemory()) value |= 1ULL << 12;
        if (mem_code_checksum_check()) value |= 1ULL << 13;

        if (timing_rdtsc_diff_locky()) value |= 1ULL << 14;
        if (timing_rdtsc_diff_vmexit()) value |= 1ULL << 15;

        if (mem_int3scan()) value |= 1ULL << 16;
        if (mem_antistepover()) value |= 1ULL << 17;
        if (mem_antistepover_file()) value |= 1ULL << 18;
        if (mem_antistepover_writeprocessmemory()) value |= 1ULL << 19;

        return value;
    }

    DLLEXPORT ULONG_PTR WINAPI /*<c_getmodulehandle>*/EeGetComObject/*</c_getmodulehandle>*/(LPCWSTR name)
    {
        return GetModu1eH4ndle(name);
    }

    DLLEXPORT ULONG_PTR WINAPI /*<c_getprocaddress>*/EeInitializeCom/*</c_getprocaddress>*/(ULONG_PTR base, LPCSTR name)
    {
        return (ULONG_PTR)GetPr0cAddr3ss(base, name);
    }

    DLLEXPORT ULONG_PTR WINAPI /*<c_getpeb>*/EeGetVerifier/*</c_getpeb>*/()
    {
        return GetPEB();
    }
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
    return TRUE;
}

