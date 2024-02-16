#include "pch.h"
#include "safe_calls.h"
#include "skCrypter.h"
#include "GetProcAddressSilent.h"

ULONG_PTR kernel32_addr = NULL;
ULONG_PTR ntdll_addr = NULL;

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

FARPROC safeGetProcAddress(HMODULE hModule, LPCSTR  lpProcName)
{
    if (!pfnGetProcAddress)
        pfnGetProcAddress = (TGetProcAddress)GetPr0cAddr3ss(findKernel32(), skCrypt("GetProcAddress"));

    if (pfnGetProcAddress)
        return pfnGetProcAddress(hModule, lpProcName);

    return nullptr;
}

BOOL safeGetThreadContext(HANDLE hThread, LPCONTEXT lpContext)
{
    if (!pfnGetThreadContext)
        pfnGetThreadContext = (TGetThreadContext)GetPr0cAddr3ss(findKernel32(), skCrypt("GetThreadContext"));

    if (pfnGetThreadContext)
        return pfnGetThreadContext(hThread, lpContext);

    return FALSE;
}

BOOL safeSetThreadContext(HANDLE hThread, const CONTEXT *lpContext)
{
    if (!pfnSetThreadContext)
        pfnSetThreadContext = (TSetThreadContext)GetPr0cAddr3ss(findKernel32(), skCrypt("SetThreadContext"));

    if (pfnSetThreadContext)
        return pfnSetThreadContext(hThread, lpContext);

    return FALSE;
}

DWORD safeGetCurrentProcessId()
{
    if (!pfnGetCurrentProcessId)
        pfnGetCurrentProcessId = (TGetCurrentProcessId)GetPr0cAddr3ss(findKernel32(), skCrypt("GetCurrentProcessId("));

    if (pfnGetCurrentProcessId)
        return pfnGetCurrentProcessId();

    return 0;
}

DWORD safeSuspendThread(HANDLE hThread)
{
    if (!pfnSuspendThread)
        pfnSuspendThread = (TSuspendThread)GetPr0cAddr3ss(findKernel32(), skCrypt("SuspendThread"));

    if (pfnSuspendThread)
        return pfnSuspendThread(hThread);

    return -1;
}

DWORD safeResumeThread(HANDLE hThread)
{
    if (!pfnResumeThread)
        pfnResumeThread = (TResumeThread)GetPr0cAddr3ss(findKernel32(), skCrypt("ResumeThread"));

    if (pfnResumeThread)
        return pfnResumeThread(hThread);

    return -1;
}

HANDLE safeGetCurrentThread()
{
    if (!pfnGetCurrentThread)
        pfnGetCurrentThread = (TGetCurrentThread)GetPr0cAddr3ss(findKernel32(), skCrypt("GetCurrentThread"));

    if (pfnGetCurrentThread)
        return pfnGetCurrentThread();

    return nullptr;
}

DWORD safeGetCurrentThreadId()
{
    if (!pfnGetCurrentThreadId)
        pfnGetCurrentThreadId = (TGetCurrentThreadId)GetPr0cAddr3ss(findKernel32(), skCrypt("GetCurrentThreadId"));

    if (pfnGetCurrentThreadId)
        return pfnGetCurrentThreadId();

    return 0;
}

HANDLE safeOpenThread(DWORD dwDesiredAccess, BOOL bInheritHandle, DWORD dwThreadId)
{
    if (!pfnOpenThread)
        pfnOpenThread = (TOpenThread)GetPr0cAddr3ss(findKernel32(), skCrypt("OpenThread"));

    if (pfnOpenThread)
        return pfnOpenThread(dwDesiredAccess, bInheritHandle, dwThreadId);

    return nullptr;
}

PVOID safeAddVectoredExceptionHandler(ULONG First, PVECTORED_EXCEPTION_HANDLER Handler)
{
    // This procedure is broken. GetProcAddress and GetProcAddr3ss returns different value.
    if (!pfnAddVectoredExceptionHandler)
        pfnAddVectoredExceptionHandler = (TAddVectoredExceptionHandler)safeGetProcAddress((HMODULE)findKernel32(), skCrypt("AddVectoredExceptionHandler"));

    if (pfnAddVectoredExceptionHandler)
        return pfnAddVectoredExceptionHandler(First, Handler);

    return nullptr;
}

ULONG safeRemoveVectoredExceptionHandler(PVOID Handler)
{
    // This procedure is broken. GetProcAddress and GetProcAddr3ss returns different value.
    if (!pfnRemoveVectoredExceptionHandler)
        pfnRemoveVectoredExceptionHandler = (TRemoveVectoredExceptionHandler)safeGetProcAddress((HMODULE)findKernel32(), skCrypt("RemoveVectoredExceptionHandler"));

    if (pfnRemoveVectoredExceptionHandler)
        return pfnRemoveVectoredExceptionHandler(Handler);

    return 0;
}

void safeRaiseException(DWORD dwExceptionCode, DWORD dwExceptionFlags, DWORD nNumberOfArguments, const ULONG_PTR *lpArguments)
{
    if (!pfnRaiseException)
        pfnRaiseException = (TRaiseException)GetPr0cAddr3ss(findKernel32(), skCrypt("RaiseException"));

    if (pfnRaiseException)
        return pfnRaiseException(dwExceptionCode, dwExceptionFlags, nNumberOfArguments, lpArguments);
}
