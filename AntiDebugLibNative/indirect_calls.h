#pragma once
#include "pch.h"
#include "GetProcAddressSilent.h"

typedef FARPROC(WINAPI *TGetProcAddress)(
    _In_ HMODULE hModule,
    _In_ LPCSTR lpProcName
    );

typedef BOOL(WINAPI *TGetThreadContext)(
    _In_ HANDLE hThread,
    _Inout_ LPCONTEXT lpContext
    );

typedef BOOL(WINAPI *TSetThreadContext)(
    _In_ HANDLE hThread,
    _In_ const CONTEXT *lpContext
    );

typedef DWORD(WINAPI *TGetCurrentProcessId)();

typedef DWORD(WINAPI *TSuspendThread)(
    _In_ HANDLE hThread
    );

typedef DWORD(WINAPI *TResumeThread)(
    _In_ HANDLE hThread
    );

typedef HANDLE(WINAPI *TGetCurrentThread)();

typedef DWORD(WINAPI *TGetCurrentThreadId)();

typedef HANDLE(WINAPI *TOpenThread)(
    _In_ DWORD dwDesiredAccess,
    _In_ BOOL  bInheritHandle,
    _In_ DWORD dwThreadId
    );

typedef PVOID(WINAPI *TAddVectoredExceptionHandler)(
    _In_ ULONG First,
    _In_ PVECTORED_EXCEPTION_HANDLER Handler
    );

typedef ULONG(WINAPI *TRemoveVectoredExceptionHandler)(
    _In_ PVOID Handle
    );

typedef void(WINAPI *TRaiseException)(
    _In_ DWORD           dwExceptionCode,
    _In_ DWORD           dwExceptionFlags,
    _In_ DWORD           nNumberOfArguments,
    _In_ const ULONG_PTR *lpArguments
    );

typedef BOOL(WINAPI *TVirtualProtect)(
    _In_  LPVOID lpAddress,
    _In_  SIZE_T dwSize,
    _In_  DWORD  flNewProtect,
    _Out_ PDWORD lpflOldProtect
    );

typedef BOOL(WINAPI *TWriteProcessMemory)(
    _In_  HANDLE  hProcess,
    _In_  LPVOID  lpBaseAddress,
    _In_  LPCVOID lpBuffer,
    _In_  SIZE_T  nSize,
    _Out_ SIZE_T *lpNumberOfBytesWritten
    );

// kernel32
FARPROC i_GetProcAddress(HMODULE hModule, LPCSTR  lpProcName);
BOOL i_GetThreadContext(HANDLE hThread, LPCONTEXT lpContext);
BOOL i_SetThreadContext(HANDLE hThread, const CONTEXT *lpContext);
DWORD i_GetCurrentProcessId();
DWORD i_SuspendThread(HANDLE hThread);
DWORD i_ResumeThread(HANDLE hThread);
HANDLE i_GetCurrentThread();
DWORD i_GetCurrentThreadId();
HANDLE i_OpenThread(DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwThreadId);
PVOID i_AddVectoredExceptionHandler(ULONG First, PVECTORED_EXCEPTION_HANDLER Handler);
ULONG i_RemoveVectoredExceptionHandler(PVOID Handler);
void i_RaiseException(DWORD dwExceptionCode, DWORD dwExceptionFlags, DWORD nNumberOfArguments, const ULONG_PTR *lpArguments);
DWORD i_VirtualProtect(LPVOID lpAddress, SIZE_T dwSize, DWORD flNewProtect, PDWORD lpflOldProtect);
BOOL i_WriteProcessMemory(HANDLE hProcess, LPVOID lpBaseAddress, LPCVOID lpBuffer, SIZE_T nSize, SIZE_T *lpNumberOfBytesWritten);
