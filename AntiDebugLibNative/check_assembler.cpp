#include "pch.h"
#include "check_assembler.h"
#include "hwbrk.h"
#include "indirect_calls.h"
#include <intrin.h>

#ifdef _WIN64
extern "C" {
    void __int2d_64();
    void __icebp_64();
    void __popf_trap_64();
    void __div0_64();
}
#endif

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L5
#pragma region INT3 (__debugbreak())

bool check_assembler_int3()
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

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L24
#pragma region INT3 (long)

bool asm_int3_long_bDebugged = false;

static int int3_long_seh(UINT code, PEXCEPTION_POINTERS ep)
{
    asm_int3_long_bDebugged = code != EXCEPTION_BREAKPOINT;
    return EXCEPTION_EXECUTE_HANDLER;
}

bool check_assembler_int3long()
{
    __try
    {
#if _WIN64
        // Let's run some shellcode
        BYTE asmcode[2] = { 0xCD, 0x03 };
        PVOID buffer = VirtualAlloc(nullptr, 2, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
        if (!buffer) return false;
        RtlMoveMemory(buffer, asmcode, 2);
        DWORD oldProtect;
        if (i_VirtualProtect(buffer, 2, PAGE_EXECUTE_READWRITE, &oldProtect))
            ((void(*)())buffer)();
        VirtualFree(buffer, 0, MEM_RELEASE);
#else
        __asm {
            __emit(0xCD);
            __emit(0x03);
        }
#endif
    }
    __except (int3_long_seh(GetExceptionCode(), GetExceptionInformation()))
    {
        return asm_int3_long_bDebugged;
    }

    return false;
}

#pragma endregion

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L43
#pragma region INT 2D

bool check_assembler_int2d()
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

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L59
#pragma region ICEPB (INT 1)

bool check_assembler_icebp()
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

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L73
// HackOvert/AntiDBG :: https://github.com/HackOvert/AntiDBG/blob/75de1f3d8e7d7488ff2e07244e3abda699d0528b/antidbg/AntiDBG.cpp#L553
#pragma region Stack Segment Register (MOVSS)

bool check_assembler_stack_segment_register()
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

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L130
#pragma region Instruction Counting

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

bool check_assembler_instruction_counting()
{
    bool bDebugged = false;
#ifndef _WIN64 // My lack of knowledge on Assembly language
    PVOID hVeh = nullptr;
    HANDLE hThread = nullptr;
    HANDLE m_hHwBps[4] = { 0, 0, 0, 0 };

    __try
    {
        hVeh = i_AddVectoredExceptionHandler(TRUE, instruction_counting_veh);
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

        i_ResumeThread(hThread);
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
            i_RemoveVectoredExceptionHandler(hVeh);
    }
#endif

    return bDebugged;
}

#pragma endregion

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L179
#pragma region Trap and Flag

bool check_assembler_popf_and_trap()
{
    __try
    {
#ifdef _WIN64
        __popf_trap_64();
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

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L201
#pragma region Instruction Prefixes

bool check_assembler_instruction_prefixes()
{
    __try
    {
#ifndef _WIN64
        __asm
        {
            // 0xF3 0x64 disassembles as PREFIX REP:
            __asm __emit 0xF3
            __asm __emit 0x64

            // One byte ICEBP
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

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Assembler.cpp#L236
#pragma region Debug Register Manipulation

static LONG debug_registers_manipulation_seh(PEXCEPTION_POINTERS pExceptionInfo)
{
    bool bDebugged = false;
    if (pExceptionInfo->ContextRecord->Dr0 != 0 || pExceptionInfo->ContextRecord->Dr1 != 0 || pExceptionInfo->ContextRecord->Dr2 != 0 || pExceptionInfo->ContextRecord->Dr3 != 0)
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

    return bDebugged ? EXCEPTION_CONTINUE_EXECUTION : EXCEPTION_EXECUTE_HANDLER;
}

bool check_assembler_debug_registers_modification()
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
