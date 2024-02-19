#include "pch.h"
#include "indirect_calls.h"
#include "skCrypter.h"
#include "GetProcAddressSilent.h"

ULONG_PTR kernel32_addr = NULL;
ULONG_PTR ntdll_addr = NULL;

// kernel32
TGetProcAddress pfnGetProcAddress = nullptr;
TGetThreadContext pfnGetThreadContext = nullptr;
TSetThreadContext pfnSetThreadContext = nullptr;
TGetCurrentProcessId pfnGetCurrentProcessId = nullptr;
TSuspendThread pfnSuspendThread = nullptr;
TResumeThread pfnResumeThread = nullptr;
TGetCurrentThread pfnGetCurrentThread = nullptr;
TGetCurrentThreadId pfnGetCurrentThreadId = nullptr;
TOpenThread  pfnOpenThread = nullptr;
TAddVectoredExceptionHandler  pfnAddVectoredExceptionHandler = nullptr;
TRemoveVectoredExceptionHandler pfnRemoveVectoredExceptionHandler = nullptr;
TRaiseException pfnRaiseException = nullptr;
TVirtualProtect pfnVirtualProtect = nullptr;
TWriteProcessMemory pfnWriteProcessMemory = nullptr;

ULONG_PTR findKernel32()
{
    if (!kernel32_addr)
        kernel32_addr = GetModu1eH4ndle(skCrypt(L"kernel32.dll"));
    return kernel32_addr;
}

ULONG_PTR findNtdll()
{
    if (!ntdll_addr)
        ntdll_addr = GetModu1eH4ndle(skCrypt(L"ntdll.dll"));
    return ntdll_addr;
}

FARPROC i_GetProcAddress(HMODULE hModule, LPCSTR lpProcName)
{
    if (!pfnGetProcAddress)
        pfnGetProcAddress = (TGetProcAddress)GetPr0cAddr3ss(findKernel32(), skCrypt("GetProcAddress"));

    if (pfnGetProcAddress)
        return pfnGetProcAddress(hModule, lpProcName);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return nullptr;
}

BOOL i_GetThreadContext(HANDLE hThread, LPCONTEXT lpContext)
{
    if (!pfnGetThreadContext)
        pfnGetThreadContext = (TGetThreadContext)GetPr0cAddr3ss(findKernel32(), skCrypt("GetThreadContext"));

    if (pfnGetThreadContext)
        return pfnGetThreadContext(hThread, lpContext);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return FALSE;
}

BOOL i_SetThreadContext(HANDLE hThread, const CONTEXT *lpContext)
{
    if (!pfnSetThreadContext)
        pfnSetThreadContext = (TSetThreadContext)GetPr0cAddr3ss(findKernel32(), skCrypt("SetThreadContext"));

    if (pfnSetThreadContext)
        return pfnSetThreadContext(hThread, lpContext);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return FALSE;
}

DWORD i_GetCurrentProcessId()
{
    if (!pfnGetCurrentProcessId)
        pfnGetCurrentProcessId = (TGetCurrentProcessId)GetPr0cAddr3ss(findKernel32(), skCrypt("GetCurrentProcessId"));

    if (pfnGetCurrentProcessId)
        return pfnGetCurrentProcessId();

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return 0;
}

DWORD i_SuspendThread(HANDLE hThread)
{
    if (!pfnSuspendThread)
        pfnSuspendThread = (TSuspendThread)GetPr0cAddr3ss(findKernel32(), skCrypt("SuspendThread"));

    if (pfnSuspendThread)
        return pfnSuspendThread(hThread);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return -1;
}

DWORD i_ResumeThread(HANDLE hThread)
{
    if (!pfnResumeThread)
        pfnResumeThread = (TResumeThread)GetPr0cAddr3ss(findKernel32(), skCrypt("ResumeThread"));

    if (pfnResumeThread)
        return pfnResumeThread(hThread);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return -1;
}

HANDLE i_GetCurrentThread()
{
    if (!pfnGetCurrentThread)
        pfnGetCurrentThread = (TGetCurrentThread)GetPr0cAddr3ss(findKernel32(), skCrypt("GetCurrentThread"));

    if (pfnGetCurrentThread)
        return pfnGetCurrentThread();

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return nullptr;
}

DWORD i_GetCurrentThreadId()
{
    if (!pfnGetCurrentThreadId)
        pfnGetCurrentThreadId = (TGetCurrentThreadId)GetPr0cAddr3ss(findKernel32(), skCrypt("GetCurrentThreadId"));

    if (pfnGetCurrentThreadId)
        return pfnGetCurrentThreadId();

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return 0;
}

HANDLE i_OpenThread(DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwThreadId)
{
    if (!pfnOpenThread)
        pfnOpenThread = (TOpenThread)GetPr0cAddr3ss(findKernel32(), skCrypt("OpenThread"));

    if (pfnOpenThread)
        return pfnOpenThread(dwDesiredAccess, bInheritHandle, dwThreadId);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return nullptr;
}

PVOID i_AddVectoredExceptionHandler(ULONG First, PVECTORED_EXCEPTION_HANDLER Handler)
{
    // This procedure is broken. GetProcAddress and GetProcAddr3ss returns different value.
    if (!pfnAddVectoredExceptionHandler)
        pfnAddVectoredExceptionHandler = (TAddVectoredExceptionHandler)i_GetProcAddress((HMODULE)findKernel32(), skCrypt("AddVectoredExceptionHandler"));

    if (pfnAddVectoredExceptionHandler)
        return pfnAddVectoredExceptionHandler(First, Handler);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return nullptr;
}

ULONG i_RemoveVectoredExceptionHandler(PVOID Handler)
{
    // This procedure is broken. GetProcAddress and GetProcAddr3ss returns different value.
    if (!pfnRemoveVectoredExceptionHandler)
        pfnRemoveVectoredExceptionHandler = (TRemoveVectoredExceptionHandler)i_GetProcAddress((HMODULE)findKernel32(), skCrypt("RemoveVectoredExceptionHandler"));

    if (pfnRemoveVectoredExceptionHandler)
        return pfnRemoveVectoredExceptionHandler(Handler);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return 0;
}

void i_RaiseException(DWORD dwExceptionCode, DWORD dwExceptionFlags, DWORD nNumberOfArguments, const ULONG_PTR *lpArguments)
{
    if (!pfnRaiseException)
        pfnRaiseException = (TRaiseException)GetPr0cAddr3ss(findKernel32(), skCrypt("RaiseException"));

    if (pfnRaiseException)
        return pfnRaiseException(dwExceptionCode, dwExceptionFlags, nNumberOfArguments, lpArguments);
}

DWORD i_VirtualProtect(LPVOID lpAddress, SIZE_T dwSize, DWORD flNewProtect, PDWORD lpflOldProtect)
{
    if (!pfnVirtualProtect)
        pfnVirtualProtect = (TVirtualProtect)GetPr0cAddr3ss(findKernel32(), skCrypt("VirtualProtect"));

    if (pfnVirtualProtect)
        return pfnVirtualProtect(lpAddress, dwSize, flNewProtect, lpflOldProtect);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return 0;
}

BOOL i_WriteProcessMemory(HANDLE hProcess, LPVOID lpBaseAddress, LPCVOID lpBuffer, SIZE_T nSize, SIZE_T *lpNumberOfBytesWritten)
{
    if (!pfnWriteProcessMemory)
        pfnWriteProcessMemory = (TWriteProcessMemory)GetPr0cAddr3ss(findKernel32(), skCrypt("WriteProcessMemory"));

    if (pfnWriteProcessMemory)
        return pfnWriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesWritten);

    SetLastError(STATUS_ENTRYPOINT_NOT_FOUND);
    return FALSE;
}
