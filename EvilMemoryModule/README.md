# EvilMemoryModule.NET :: Load modules in-memory and invoke

This project is heavily based on (almost Ctrl-CV'd) from [DllFromMemory.Net](https://github.com/schellingb/DLLFromMemory-net/) project and [DInvoke](https://github.com/TheWover/DInvoke) project.

I've added more protection and indirection layer to help further obfuscation and encryption.

No single use of P/Invoke and `nameof()` keyword. Anything in this repository is fully obfuscator-compatible!

Also feel free to edit AssemblyInfo.cs to change file name, description, etc.

## Contributions

DllFromMemory.Safer.NET is based on DllFromMemory.Net commit 7b1773c8035429e6fb1ab4b8fd0a52d2a4810efc.
https://github.com/schellingb/DLLFromMemory-net

DLLFromMemory.Net is based on Memory Module.net 0.2
Copyright (C) 2012 - 2018 by Andreas Kanzler
https://github.com/Scavanger/MemoryModule.net

Memory Module.net is based on Memory DLL loading code Version 0.0.4
Copyright (C) 2004 - 2015 by Joachim Bauch
https://github.com/fancycode/MemoryModule

## Repositories

* [DllFromMemory.Net](https://github.com/schellingb/DLLFromMemory-net)
* [DInvoke](https://github.com/TheWover/DInvoke)
