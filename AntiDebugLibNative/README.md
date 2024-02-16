# MyStealer.AntiDebug.Native

엔트리 포인트 함수는 dllmain.cpp에 정의되어 있으며, 이름을 자유롭게 바꿔도 됩니다.
단, 이름 변경 시 C#단(MyStealer.AntiDebug)에서도 엔트리 포인트 이름을 변경해 주어야 합니다.

반환값은 uint64 비트플래그이며, 각 비트는 실패한 검사들을 의미합니다.

|비트|검사 종류|검사 이름|
|---:|:---:|:---:|
|0|어셈블리|INT3 인터럽트 (__debugbreak)|
|1|어셈블리|INT3 인터럽트 (직접 정의, 32비트 전용)|
|2|어셈블리|INT2D(INT 0x2D)|
|3|어셈블리|ICEBP(0x1F)|
|4|어셈블리|스택 세그먼트 레지스터 검사|
|5|어셈블리|Instruction Counting VEH|
|6|어셈블리|POPF & TRAP|
|7|어셈블리|인스트럭션 접두사 검사|
|8|어셈블리|디버그 레지스터 변조 검사|
|9|에러 핸들링|Unhandled SEH|
|10|에러 핸들링|RaiseException|
|11|에러 핸들링|VEH|
|12|메모리 검사|NtQueryVirtualMemory|
|13|메모리 검사|코드 체크섬(CRC-32) 검사|
|14|타이밍 검사|RDTSC 인스트럭션 명령 실행 시간 비교|
|15|타이밍 검사|RDTSC 인스트럭션 __cpuid 조회 시간 비교|
