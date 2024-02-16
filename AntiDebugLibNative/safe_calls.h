#pragma once
#include "pch.h"
#include "GetProcAddressSilent.h"

typedef FARPROC(WINAPI *TGetProcAddress)(
    IN HMODULE hModule,
    IN LPCSTR lpProcName
    );

typedef BOOL(WINAPI *TGetThreadContext)(
    IN HANDLE hThread,
    IN OUT LPCONTEXT lpContext
    );

typedef BOOL(WINAPI *TSetThreadContext)(
    IN HANDLE hThread,
    IN const CONTEXT *lpContext
    );

typedef DWORD(WINAPI *TGetCurrentProcessId)();

typedef DWORD(WINAPI *TSuspendThread)(
    IN HANDLE hThread
    );

typedef DWORD(WINAPI *TResumeThread)(
    IN HANDLE hThread
    );

typedef HANDLE(WINAPI *TGetCurrentThread)();

typedef DWORD(WINAPI *TGetCurrentThreadId)();

typedef HANDLE(WINAPI *TOpenThread)(
    IN DWORD dwDesiredAccess,
    IN BOOL  bInheritHandle,
    IN DWORD dwThreadId
    );

typedef PVOID(WINAPI *TAddVectoredExceptionHandler)(
    ULONG First,
    PVECTORED_EXCEPTION_HANDLER Handler
    );

typedef ULONG(WINAPI *TRemoveVectoredExceptionHandler)(
    IN PVOID Handle
    );

typedef void(WINAPI *TRaiseException)(
    IN DWORD           dwExceptionCode,
    IN DWORD           dwExceptionFlags,
    IN DWORD           nNumberOfArguments,
    IN const ULONG_PTR *lpArguments
    );

FARPROC safeGetProcAddress(HMODULE hModule, LPCSTR  lpProcName);
BOOL safeGetThreadContext(HANDLE hThread, LPCONTEXT lpContext);
BOOL safeSetThreadContext(HANDLE hThread, const CONTEXT *lpContext);
DWORD safeGetCurrentProcessId();
DWORD safeSuspendThread(HANDLE hThread);
DWORD safeResumeThread(HANDLE hThread);
HANDLE safeGetCurrentThread();
DWORD safeGetCurrentThreadId();
HANDLE safeOpenThread(DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwThreadId);
PVOID safeAddVectoredExceptionHandler(ULONG First, PVECTORED_EXCEPTION_HANDLER Handler);
ULONG safeRemoveVectoredExceptionHandler(PVOID Handler);
void safeRaiseException(DWORD dwExceptionCode, DWORD dwExceptionFlags, DWORD nNumberOfArguments, const ULONG_PTR *lpArguments);
