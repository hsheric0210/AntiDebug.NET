#include "pch.h"
#include "check_exception.h"
#include "safe_calls.h"
#include <intrin.h>

#ifdef _WIN64
extern "C" {
    void __div0_64();
    BOOLEAN __unhandled_exception_filter_64();
}
#endif

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Exceptions.cpp#L4
#pragma region Normal SEH (DIV/0)

bool check_exception_seh()
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
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        return false;
    }
}

#pragma endregion

// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/exceptions.html#unhandledexceptionfilter
// Currently disabled because of errors
#pragma region UnhandledExceptionFilter

#ifndef _WIN64
// Return 1 if exception is swallowed (= being debugged)
// Return 0 if exception is not swallowed ('inc eax' will be skipped by handler)+
__declspec(naked) BOOLEAN unhandled_exception_raiser()
{
    __asm
    {
        xor eax, eax
        int 3 // CC
        inc eax // 40
        ret
    }
}
#endif

LONG unhandled_exception_filter(PEXCEPTION_POINTERS pExceptionInfo)
{
    PCONTEXT ctx = pExceptionInfo->ContextRecord;
#ifdef _WIN64
    ctx->Rip += 4; // Skip: 'CC 48 FF C0'
#else
    ctx->Eip += 2; // Skip: 'CC 40'
#endif
    return EXCEPTION_CONTINUE_SEARCH;
}

bool check_exception_unhandledexceptionfilter()
{
    bool bDebugged = true;
    SetUnhandledExceptionFilter((LPTOP_LEVEL_EXCEPTION_FILTER)unhandled_exception_filter);

    return false;

#ifdef _WIN64
    return __unhandled_exception_filter_64();
#else
    return unhandled_exception_raiser();
#endif
}
#pragma endregion


// Checkpoint AntiDebug Research :: https://anti-debug.checkpoint.com/techniques/exceptions.html#raiseexception
// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Exceptions.cpp#L18
#pragma region kernel32!RaiseException SEH

bool check_exception_raiseexception()
{
    __try
    {
        safeRaiseException(DBG_CONTROL_C, 0, 0, nullptr);
        return true;
    }
    __except (DBG_CONTROL_C == GetExceptionCode() ? EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_SEARCH)
    {
        return false;
    }
}

#pragma endregion

// ShowStopper :: https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Exceptions.cpp#L45
#pragma region VEH (Vectorized Error Handler)

static LONG CALLBACK my_veh(PEXCEPTION_POINTERS pExceptionInfo)
{
    PCONTEXT ctx = pExceptionInfo->ContextRecord;
    if (ctx->Dr0 != 0 || ctx->Dr1 != 0 || ctx->Dr2 != 0 || ctx->Dr3 != 0)
    {
        // Skip 'bDebugged = true'
#ifdef _WIN64
        ctx->Rip += 2;
#else
        ctx->Eip += 2;
#endif
        return EXCEPTION_CONTINUE_EXECUTION;
    }
    return EXCEPTION_EXECUTE_HANDLER;
}

bool check_exception_veh()
{
    HANDLE hExeptionHandler = nullptr;
    bool bDebugged = false;
    __try
    {
        hExeptionHandler = safeAddVectoredExceptionHandler(1, my_veh);

        __try
        {
            __debugbreak(); // INT3
            bDebugged = true;
        }
        __except (EXCEPTION_EXECUTE_HANDLER)
        {
            // ignored
        }
    }
    __finally
    {
        if (hExeptionHandler)
            safeRemoveVectoredExceptionHandler(hExeptionHandler);
    }

    return bDebugged;
}

#pragma endregion

// al-khaser :: https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/TrapFlag.cpp
// https://en.wikipedia.org/wiki/FLAGS_register
#pragma region Trap flag

static BOOL trap_flag_swallowed_exception = TRUE;

static LONG CALLBACK trap_flag_veh(_In_ PEXCEPTION_POINTERS ExceptionInfo)
{
    trap_flag_swallowed_exception = FALSE;

    if (ExceptionInfo->ExceptionRecord->ExceptionCode == EXCEPTION_SINGLE_STEP)
        return EXCEPTION_CONTINUE_EXECUTION;

    return EXCEPTION_CONTINUE_SEARCH;
}

bool check_exception_trapflag()
{
    PVOID Handle = safeAddVectoredExceptionHandler(1, trap_flag_veh);
    trap_flag_swallowed_exception = TRUE;

#ifdef _WIN64
    UINT64 eflags = __readeflags();
#else
    UINT eflags = __readeflags();
#endif

    //  Set the trap flag
    eflags |= 0x100; // Trap flag (single step)
    __writeeflags(eflags);

    safeRemoveVectoredExceptionHandler(Handle);
    return trap_flag_swallowed_exception;
}

#pragma endregion
