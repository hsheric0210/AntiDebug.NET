#include "pch.h"
#include "memory_chk.h"
#include "skCrypter.h"
#include "safe_calls.h"
#include "GetProcAddressSilent.h"
#include <intrin.h>

#pragma region INT3 scan 

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_MemoryChecks.cpp#L16

bool mem_int3scan()
{
    return int3scan_scan_byte(0xCC, _ReturnAddress(), 1); // 0xCC = INT (Interrupt)
}

bool int3scan_scan_byte(BYTE cByte, PVOID pMemory, SIZE_T nMemorySize)
{
    auto pBytes = (PBYTE)pMemory;
    for (SIZE_T i = 0; ; i++)
    {
        if (((nMemorySize > 0) && (i >= nMemorySize)) // bound check
            || ((nMemorySize == 0) && (pBytes[i] == 0xC3))) // RETN
            break;

        if (pBytes[i] == cByte && pBytes[i - 1] != cByte && pBytes[i + 1] != cByte)
        {
            return true;
        }
    }
    return false;
}

#pragma endregion

#pragma region Anti Step-over (then directly overwrite it with NOP)

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_MemoryChecks.cpp#L45

bool mem_antistepover()
{
    PVOID pRetAddress = _ReturnAddress();
    bool bBpFound = *(PBYTE)pRetAddress == 0xCC; // 0xCC = INT (Interrupt)
    if (bBpFound)
    {
        DWORD dwOldProtect;
        if (VirtualProtect(pRetAddress, 1, PAGE_EXECUTE_READWRITE, &dwOldProtect))
        {
            *(PBYTE)pRetAddress = 0x90; // Replace with 0x90 (NOP)
            VirtualProtect(pRetAddress, 1, dwOldProtect, &dwOldProtect);
        }
    }
    return bBpFound;
}

#pragma endregion

#pragma region Anti Step-over (then overwrite with the original Op with ReadFile())

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_MemoryChecks.cpp#L66

bool mem_antistepover_file()
{
    PVOID pRetAddress = _ReturnAddress();
    bool bBpFound = *(PBYTE)pRetAddress == 0xCC; // 0xCC = INT (Interrupt)
    if (bBpFound)
    {
        DWORD dwOldProtect, dwRead;
        if (VirtualProtect(pRetAddress, 1, PAGE_EXECUTE_READWRITE, &dwOldProtect))
        {
            CHAR szFilePath[MAX_PATH];
            if (GetModuleFileNameA(nullptr, szFilePath, MAX_PATH))
            {
                HANDLE hFile = CreateFileA(szFilePath, GENERIC_READ, FILE_SHARE_READ, nullptr, OPEN_EXISTING, 0, nullptr);
                if (INVALID_HANDLE_VALUE != hFile)
                    ReadFile(hFile, pRetAddress, 1, &dwRead, nullptr);
            }
            VirtualProtect(pRetAddress, 1, dwOldProtect, &dwOldProtect);
        }
    }
    return bBpFound;
}

#pragma endregion

#pragma region Anti Step-over (then overwrite with NOP using WriteProcessMemory())

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_MemoryChecks.cpp#L104

const BYTE mem_antistepover_writeprocessmrmory_buffer[] = { 0x90 }; // NOP

bool mem_antistepover_writeprocessmemory()
{
    PVOID pRetAddress = _ReturnAddress();
    bool bBpFound = *(PBYTE)pRetAddress == 0xCC; // 0xCC = INT (Interrupt)
    if (bBpFound)
    {
        DWORD dwOldProtect;
        if (VirtualProtect(pRetAddress, 1, PAGE_EXECUTE_READWRITE, &dwOldProtect))
        {
            WriteProcessMemory(GetCurrentProcess(), pRetAddress, mem_antistepover_writeprocessmrmory_buffer, 1, nullptr);
            VirtualProtect(pRetAddress, 1, dwOldProtect, &dwOldProtect);
        }
    }
    return bBpFound;
}

#pragma endregion

#pragma region NtQueryVirtualMemory 

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_MemoryChecks.cpp#L202
// https://www.virusbulletin.com/virusbulletin/2012/12/journey-sirefef-packer-research-case-study
// https://waliedassar1.rssing.com/chan-33272685/all_p2.html

bool mem_ntqueryvirtualmemory()
{
#ifndef _WIN64
    auto pfnNtQueryVirtualMemory = (TNtQueryVirtualMemory)GetPr0cAddr3ss(GetModu1eH4ndle(skCrypt(L"ntdll.dll")), skCrypt("NtQueryVirtualMemory"));

    NTSTATUS status;
    PBYTE pMem = nullptr;
    DWORD dwMemSize = 0;

    do
    {
        dwMemSize += 0x1000;
        pMem = (PBYTE)_malloca(dwMemSize);
        if (!pMem)
            return false;

        memset(pMem, 0, dwMemSize);
        status = pfnNtQueryVirtualMemory(GetCurrentProcess(), NULL, MemoryWorkingSetList, pMem, dwMemSize, NULL);
    } while (status == STATUS_INFO_LENGTH_MISMATCH);

    PMEMORY_WORKING_SET_LIST pWorkingSet = (PMEMORY_WORKING_SET_LIST)pMem;
    for (ULONG i = 0; i < pWorkingSet->NumberOfPages; i++)
    {
        DWORD dwAddr = pWorkingSet->WorkingSetList[i].VirtualPage << 0x0C;
        DWORD dwEIP = 0;
        __asm
        {
            push eax
            call $ + 5
            pop eax
            mov dwEIP, eax
            pop eax
        }

        if (dwAddr == (dwEIP & 0xFFFFF000))
            return (pWorkingSet->WorkingSetList[i].Shared == 0) || (pWorkingSet->WorkingSetList[i].ShareCount == 0);
    }
#endif // _WIN64
    return false;
}

#pragma endregion

#pragma region Code Checksum Test

// https://github.com/CheckPointSW/showstopper/blob/4e6b8dbef35724d7eb987f61cf72dff7a6abfe49/src/not_suspicious/Technique_MemoryChecks.cpp#L411

#ifndef _WIN64
static __declspec(naked) int code_checksum_test()
{
    __asm
    {
        push edx
        mov edx, 0
        mov eax, 10
        mov ecx, 2
        div ecx
        pop edx
        ret
    }
}

size_t calculate_function_size(PVOID pFunc)
{
    PBYTE pMem = (PBYTE)pFunc;
    size_t nFuncSize = 0;
    do
    {
        ++nFuncSize;
    } while (*(pMem++) != 0xC3);
    return nFuncSize;
}

unsigned bit_reverse(unsigned x)
{
    x = ((x & 0x55555555) << 1) | ((x >> 1) & 0x55555555);
    x = ((x & 0x33333333) << 2) | ((x >> 2) & 0x33333333);
    x = ((x & 0x0F0F0F0F) << 4) | ((x >> 4) & 0x0F0F0F0F);
    x = (x << 24) | ((x & 0xFF00) << 8) | ((x >> 8) & 0xFF00) | (x >> 24);
    return x;
}

unsigned int crc32(unsigned char *message, int size)
{
    unsigned int byte, crc = 0xFFFFFFFF;
    for (int i = 0; i < size; i++)
    {
        byte = message[i];
        byte = bit_reverse(byte);
        for (int j = 0; j <= 7; j++)
        {
            if ((int)(crc ^ byte) < 0)
                crc = (crc << 1) ^ 0x04C11DB7;
            else
                crc = crc << 1;
            byte = byte << 1;
        }
    }
    return bit_reverse(~crc);
}

PVOID mem_code_checksum_func_addr;
size_t mem_code_checksum_func_size;
UINT32 mem_code_checksum_checksum;

void mem_code_checksum_init()
{
    code_checksum_test();
    mem_code_checksum_func_addr = &code_checksum_test;
    mem_code_checksum_func_size = calculate_function_size(mem_code_checksum_func_addr);
    mem_code_checksum_checksum = crc32((PBYTE)mem_code_checksum_func_addr, mem_code_checksum_func_size);
    DWORD Checksum = crc32((PBYTE)mem_code_checksum_func_addr, mem_code_checksum_func_size);
}

bool mem_code_checksum_check()
{
    return crc32((PBYTE)mem_code_checksum_func_addr, mem_code_checksum_func_size) != mem_code_checksum_checksum;
}
#else
bool mem_code_checksum_check()
{
    return false;
}
#endif

#pragma endregion

#pragma region Page Guard Violation

// https://github.com/LordNoteworthy/al-khaser/blob/master/al-khaser/AntiDebug/MemoryBreakpoints_PageGuard.cpp
bool mem_pageguard()
{
    SYSTEM_INFO SystemInfo = { 0 };
    DWORD OldProtect = 0;
    PVOID pAllocation = NULL;

    GetSystemInfo(&SystemInfo);

    pAllocation = VirtualAlloc(NULL, SystemInfo.dwPageSize, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
    if (pAllocation == NULL)
        return FALSE;

    RtlFillMemory(pAllocation, 1, 0xC3);

    if (VirtualProtect(pAllocation, SystemInfo.dwPageSize, PAGE_EXECUTE_READWRITE | PAGE_GUARD, &OldProtect) == 0)
        return FALSE;

    __try
    {
        ((void(*)())pAllocation)(); // Exception or execution, which shall it be :D?
    }
    __except (GetExceptionCode() == STATUS_GUARD_PAGE_VIOLATION ? EXCEPTION_EXECUTE_HANDLER : EXCEPTION_CONTINUE_SEARCH)
    {
        VirtualFree(pAllocation, 0, MEM_RELEASE);
        return FALSE;
    }

    VirtualFree(pAllocation, 0, MEM_RELEASE);
    return TRUE;
}

#pragma endregion
