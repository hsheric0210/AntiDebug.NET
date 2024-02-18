#pragma once

#define STATUS_INFO_LENGTH_MISMATCH 0xC0000004

typedef union _PSAPI_WORKING_SET_BLOCK
{
    ULONG Flags;
    struct
    {
        ULONG Protection : 5;
        ULONG ShareCount : 3;
        ULONG Shared : 1;
        ULONG Reserved : 3;
        ULONG VirtualPage : 20;
    };
} PSAPI_WORKING_SET_BLOCK, *PPSAPI_WORKING_SET_BLOCK;

typedef struct _MEMORY_WORKING_SET_LIST
{
    ULONG NumberOfPages;
    PSAPI_WORKING_SET_BLOCK WorkingSetList[1];
} MEMORY_WORKING_SET_LIST, *PMEMORY_WORKING_SET_LIST;

typedef enum _MEMORY_INFORMATION_CLASS
{
    MemoryBasicInformation,
    MemoryWorkingSetList,
} MEMORY_INFORMATION_CLASS;

typedef NTSTATUS(WINAPI *TNtQueryVirtualMemory)(
    HANDLE                   ProcessHandle,
    PVOID                    BaseAddress,
    MEMORY_INFORMATION_CLASS MemoryInformationClass,
    PVOID                    MemoryInformation,
    SIZE_T                   MemoryInformationLength,
    PSIZE_T                  ReturnLength
    );

bool check_memory_antistepover_direct();
bool check_memory_antistepover_readfile();
bool check_memory_antistepover_writeprocessmemory();
bool check_memory_ntqueryvirtualmemory();
bool check_memory_pageguard();
