#include "pch.h"
#include "exception_chk.h"
#include "safe_calls.h"
#include <intrin.h>

#ifdef _WIN64
extern "C" {
    void __div0_64();
}
#endif

#pragma region Unhandled Exception SEH

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Exceptions.cpp#L4
bool exc_unhandled_seh()
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

#pragma region kernel32!RaiseException SEH

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Exceptions.cpp#L18
bool exc_raiseexception()
{
    __try
    {
        safeRaiseException(DBG_CONTROL_C, 0, 0, NULL);
        return true;
    }
    __except (DBG_CONTROL_C == GetExceptionCode()
        ? EXCEPTION_EXECUTE_HANDLER
        : EXCEPTION_CONTINUE_SEARCH)
    {
        return false;
    }
}

#pragma endregion

#pragma region VEH (Vectorized Error Handler)

static LONG CALLBACK my_veh(PEXCEPTION_POINTERS pExceptionInfo)
{
    PCONTEXT ctx = pExceptionInfo->ContextRecord;
    if (ctx->Dr0 != 0 || ctx->Dr1 != 0 || ctx->Dr2 != 0 || ctx->Dr3 != 0)
    {
#ifdef _WIN64
        ctx->Rip += 2;
#else
        ctx->Eip += 2;
#endif
        return EXCEPTION_CONTINUE_EXECUTION;
    }
    return EXCEPTION_EXECUTE_HANDLER;
}

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_Exceptions.cpp#L45
bool exc_veh()
{
    HANDLE hExeptionHandler = NULL;
    bool bDebugged = false;
    __try
    {
        hExeptionHandler = safeAddVectoredExceptionHandler(1, my_veh);

        __try
        {
            __debugbreak();
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

#pragma region Trap flag

// https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/TrapFlag.cpp

static BOOL trap_flag_swallowed_exception = TRUE;

static LONG CALLBACK trap_flag_veh(
    _In_ PEXCEPTION_POINTERS ExceptionInfo
)
{
    trap_flag_swallowed_exception = FALSE;

    if (ExceptionInfo->ExceptionRecord->ExceptionCode == EXCEPTION_SINGLE_STEP)
        return EXCEPTION_CONTINUE_EXECUTION;

    return EXCEPTION_CONTINUE_SEARCH;
}

bool exc_trap_flag()
{
    PVOID Handle = safeAddVectoredExceptionHandler(1, trap_flag_veh);
    trap_flag_swallowed_exception = TRUE;

#ifdef _WIN64
    UINT64 eflags = __readeflags();
#else
    UINT eflags = __readeflags();
#endif

    //  Set the trap flag
    eflags |= 0x100;
    __writeeflags(eflags);

    safeRemoveVectoredExceptionHandler(Handle);
    return trap_flag_swallowed_exception;
}

#pragma endregion
