#include "pch.h"
#include "timing_chk.h"
#include <intrin.h>

// https://github.com/LordNoteworthy/al-khaser/blob/0f31a3866bafdfa703d2ed1ee1a242ab31bf5ef0/al-khaser/TimingAttacks/timing.cpp#L151
bool timing_rdtsc_diff_locky()
{
    const int retry_count = 50;
    ULONGLONG tsc1;
    ULONGLONG tsc2;
    ULONGLONG tsc3;
    DWORD i = 0;

    for (i = 0; i < retry_count; i++)
    {
        tsc1 = __rdtsc();
        GetProcessHeap();
        tsc2 = __rdtsc();
        CloseHandle(0);
        tsc3 = __rdtsc();

        if ((LODWORD(tsc3) - LODWORD(tsc2)) / (LODWORD(tsc2) - LODWORD(tsc1)) >= 10)
            return FALSE;
    }

    return TRUE;
}

// https://github.com/LordNoteworthy/al-khaser/blob/0f31a3866bafdfa703d2ed1ee1a242ab31bf5ef0/al-khaser/TimingAttacks/timing.cpp#L189
bool timing_rdtsc_diff_vmexit()
{
    const int retry_count = 50;
    ULONGLONG tsc1 = 0;
    ULONGLONG tsc2 = 0;
    ULONGLONG avg = 0;
    INT cpuInfo[4] = {};

    for (INT i = 0; i < retry_count; i++)
    {
        tsc1 = __rdtsc();
        __cpuid(cpuInfo, 0);
        tsc2 = __rdtsc();

        avg += (tsc2 - tsc1);
    }

    avg = avg / retry_count;
    return (avg < 1000 && avg > 0) ? FALSE : TRUE;
}