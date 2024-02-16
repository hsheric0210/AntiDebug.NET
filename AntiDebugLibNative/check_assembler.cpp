#include "pch.h"
#include "assembler_chk.h"
#include "hwbrk.h"
#include "safe_calls.h"
#include <intrin.h>

#ifdef _WIN64
extern "C" {
    void __int2d_64();
    void __icebp_64();
    void __popf_trap();
    void __div0_64();
}
#endif

#pragma region INT3 (__debugbreak())

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L5
bool asm_int3()
{
    __try
    {
        // https://learn.microsoft.com/ko-kr/cpp/intrinsics/debugbreak
        // https://stackoverflow.com/a/3634151
        __debugbreak();
        return true;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        return false;
    }
}

#pragma endregion

#pragma region INT3 (long)

#ifndef _WIN64
bool asm_int3_long_bDebugged = false;

static int int3_long_seh(UINT code, PEXCEPTION_POINTERS ep)
{
    asm_int3_long_bDebugged = code != EXCEPTION_BREAKPOINT;
    return EXCEPTION_EXECUTE_HANDLER;
}

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L24
bool asm_int3_long()
{
    __try
    {
        // should we just allocate this to the memory and directly CALL it?
        // VirtualAlloc -> Write -> convert to function pointer -> execute -> VirtualFree
        __asm {
            __emit(0xCD);
            __emit(0x03);
        }
    }
    __except (int3_long_seh(GetExceptionCode(), GetExceptionInformation()))
    {
        return asm_int3_long_bDebugged;
    }
}
#else
// not available on x64 due to my lack of knowledge on Assembly
bool asm_int3_long()
{
    return false;
}
#endif

#pragma endregion

#pragma region INT 2D

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L43
bool asm_int2d()
{
    __try
    {
#ifdef _WIN64
        __int2d_64();
#else
        __asm {
            xor eax, eax;
            int 0x2d;
            nop;
        }
#endif
        return true;
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        return false;
    }
}

#pragma endregion

#pragma region ICEPB (INT 1)

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L59
bool asm_ice()
{
    __try
    {
#ifdef _WIN64
        __icebp_64();
#else
        __asm __emit 0xF1;
        return true;
#endif
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        // ignored
    }

    return false;
}

#pragma endregion

#pragma region Stack Segment Register

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L73
bool asm_stack_segment_register()
{
    bool bTraced = false;

#ifndef _WIN64 // My lack of knowledge on Assembly language
    __asm
    {
        push ss
        pop ss
        pushf
        test byte ptr[esp + 1], 1
        jz movss_not_being_debugged
    }

    bTraced = true;

movss_not_being_debugged:
    // restore stack
    __asm popf;
#endif

    return bTraced;
}

#pragma endregion

#pragma region Instruction Counting

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L130

#ifndef _WIN64 // My lack of knowledge on Assembly language
__declspec(naked) DWORD WINAPI instruction_counting_proc(LPVOID lpThreadParameter)
{
    __asm
    {
        xor eax, eax
        nop
        nop
        nop
        nop
        cmp al, 4
        jne being_debugged
    }

    ExitThread(FALSE);

being_debugged:
    ExitThread(TRUE);
}

static LONG WINAPI instruction_counting_veh(PEXCEPTION_POINTERS pExceptionInfo)
{
    if (pExceptionInfo->ExceptionRecord->ExceptionCode == EXCEPTION_SINGLE_STEP)
    {
        pExceptionInfo->ContextRecord->Eax += 1;
        pExceptionInfo->ContextRecord->Eip += 1;
        return EXCEPTION_CONTINUE_EXECUTION;
    }
    return EXCEPTION_CONTINUE_SEARCH;
}
#endif

bool asm_instruction_counting()
{
    bool bDebugged = false;
#ifndef _WIN64 // My lack of knowledge on Assembly language
    PVOID hVeh = nullptr;
    HANDLE hThread = nullptr;
    HANDLE m_hHwBps[4] = { 0, 0, 0, 0 };

    __try
    {
        hVeh = safeAddVectoredExceptionHandler(TRUE, instruction_counting_veh);
        if (!hVeh)
            __leave;

        hThread = CreateThread(0, 0, instruction_counting_proc, NULL, CREATE_SUSPENDED, 0);
        if (!hThread)
            __leave;

        PVOID m_pThreadAddr = &instruction_counting_proc;
        if (*(PBYTE)m_pThreadAddr == 0xE9)
            m_pThreadAddr = (PVOID)((DWORD)m_pThreadAddr + 5 + *(PDWORD)((PBYTE)m_pThreadAddr + 1));

        for (size_t i = 0; i < 4; i++)
            m_hHwBps[i] = SetHardwareBreakpoint(hThread, HWBRK_TYPE_CODE, HWBRK_SIZE_1, (PVOID)((DWORD)m_pThreadAddr + 2 + i));

        safeResumeThread(hThread);
        WaitForSingleObject(hThread, INFINITE);

        DWORD dwThreadExitCode;
        if (TRUE == GetExitCodeThread(hThread, &dwThreadExitCode))
            bDebugged = (TRUE == dwThreadExitCode);
    }
    __finally
    {
        if (hThread)
            CloseHandle(hThread);

        for (int i = 0; i < 4; i++)
        {
            if (m_hHwBps[i])
                RemoveHardwareBreakpoint(m_hHwBps[i]);
        }

        if (hVeh)
            safeRemoveVectoredExceptionHandler(hVeh);
    }
#endif

    return bDebugged;
}

#pragma endregion

#pragma region Trap and Flag

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L179
bool asm_popf_and_trap()
{
    __try
    {
#ifdef _WIN64
        __popf_trap();
#else
        __asm
        {
            pushfd
            mov dword ptr[esp], 0x100
            popfd
            nop
        }
        return true;
#endif
    }
    __except (GetExceptionCode() == EXCEPTION_SINGLE_STEP ? EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_EXECUTION)
    {
        // ignored
    }

    return false;
}

#pragma endregion

#pragma region Instruction Prefixes

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L201
bool asm_instruction_prefixes()
{
    __try
    {
#ifndef _WIN64
        __asm
        {
            // 0xF3 0x64 disassembles as PREFIX REP:
            __asm __emit 0xF3
            __asm __emit 0x64

            // One byte INT 1
            __asm __emit 0xF1
        }
        return true;
#endif
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        // ignored
    }

    return false;
}

#pragma endregion

#pragma region Debug Register Manipulation

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L236

static LONG debug_registers_manipulation_seh(PEXCEPTION_POINTERS pExceptionInfo)
{
    bool bDebugged = false;
    if (pExceptionInfo->ContextRecord->Dr0 != 0 || pExceptionInfo->ContextRecord->Dr1 != 0 ||
        pExceptionInfo->ContextRecord->Dr2 != 0 || pExceptionInfo->ContextRecord->Dr3 != 0)
        bDebugged = true;

#ifdef _WIN64
    pExceptionInfo->ContextRecord->Rip += 2;
#else
    pExceptionInfo->ContextRecord->Eip += 2;
#endif
    pExceptionInfo->ContextRecord->Dr0 = 0;
    pExceptionInfo->ContextRecord->Dr1 = 0;
    pExceptionInfo->ContextRecord->Dr2 = 0;
    pExceptionInfo->ContextRecord->Dr3 = 0;

    return bDebugged
        ? EXCEPTION_CONTINUE_EXECUTION
        : EXCEPTION_EXECUTE_HANDLER;
}

bool asm_debug_registers_modification()
{
    __try
    {
        // Raise DIV/0
#ifdef _WIN64
        __div0_64();
#else
        __asm
        {
            xor eax, eax
            div eax
        }
#endif

        return true;
    }
    __except (debug_registers_manipulation_seh(GetExceptionInformation()))
    {
        return false;
    }
}

#pragma endregion

#pragma region MOVSS

// https://github.com/HackOvert/AntiDBG/blob/75de1f3d8e7d7488ff2e07244e3abda699d0528b/antidbg/AntiDBG.cpp#L553
bool asm_movss()
{
#ifndef _WIN64 // Only works on x86
    BOOL found = FALSE;

    _asm
    {
        push ss;
        pop ss;
        pushfd;
        test byte ptr[esp + 1], 1;
        jne fnd;
        jmp end;
fnd:
        mov found, 1;
end:
        nop;
    }

    return found;
#endif
    return false;
}

#pragma endregion
