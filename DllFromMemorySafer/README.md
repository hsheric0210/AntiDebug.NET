# DllFromMemory.Safer.NET :: Safer [DllFromMemory.Net](https://github.com/schellingb/DLLFromMemory-net/)

This project is heavily based on (almost Ctrl-CV'd) from [DllFromMemory.Net](https://github.com/schellingb/DLLFromMemory-net/) project. But, I've added more protection and indirection layer to help further obfuscation and encryption.

## Original Readme

DllFromMemory.Net is a C# library to load a native DLL from memory without the need to allow unsafe code.

By default C# can load external libraries only via files on the filesystem. A common workaround for this problem is to write the DLL into a temporary file first and import it from there. This library can be used to load a DLL completely from memory - without storing on the disk first.

It supports both 32bit and 64bit processes/DLLs, as well as AnyCPU builds.
For AnyCPU, both 32bit and 64bit DLLs must be available in memory (see Sample)

### Contributions

DllFromMemory.Safer.NET is based on DllFromMemory.Net commit 7b1773c8035429e6fb1ab4b8fd0a52d2a4810efc.
https://github.com/schellingb/DLLFromMemory-net

DLLFromMemory.Net is based on Memory Module.net 0.2
Copyright (C) 2012 - 2018 by Andreas Kanzler
https://github.com/Scavanger/MemoryModule.net

Memory Module.net is based on Memory DLL loading code Version 0.0.4
Copyright (C) 2004 - 2015 by Joachim Bauch
https://github.com/fancycode/MemoryModule
