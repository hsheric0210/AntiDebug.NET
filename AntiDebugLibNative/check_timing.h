#pragma once

#define LODWORD(_qw) ((DWORD)(_qw))

bool check_timing_rdtsc_diff_locky();
bool check_timing_rdtsc_diff_vmexit();
