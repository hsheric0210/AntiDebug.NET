#pragma once

#define LODWORD(_qw) ((DWORD)(_qw))

bool timing_rdtsc_diff_locky();
bool timing_rdtsc_diff_vmexit();
