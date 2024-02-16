# AntiDebug.NET :: Anti-debugging made easy

Various anti-debugging and anti-vm, anti-sandbox techniques are all supported!

To prevent detection and ease obfuscation, all sensitive function calls are handled indirectly and dynamically.
Instead of using direct P/Invoke, they're all retrieved dynamically via hand-made GetProcAddress alternative. (Manually scan IAT to find function, to bypass GetProcAddress hooking)

TODO: Unhook on start to prevent IAT overwrite hooking.

Complicated anti-debug features are implemented using a native DLL. It is loaded 'in-memory' (without leaving file on disk) when it's executed and freed on exit.

## [Techniques](techniques.md)

## TODO(todo.md)

## Related Articles

* The "Ultimate" Anti-Debugging Reference :: http://pferrie.epizy.com/papers/antidebug.pdf
* Check Point Research Anti-Debug Tricks :: https://anti-debug.checkpoint.com/

### Anti-unpacker tricks by Peter Ferrie

* Paper :: https://pferrie.tripod.com/papers/unpackers.pdf

* Part 1 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200812.pdf
* Part 2 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200901.pdf
* Part 3 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200902.pdf
* Part 4 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200903.pdf
* Part 5 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200904.pdf
* Part 6 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200905.pdf
* Part 7 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200906.pdf
* Part 8 :: https://www.virusbulletin.com/uploads/pdf/magazine/2008/200812.pdf

## Related Repositories

* al-khaser :: https://github.com/LordNoteworthy/al-khaser
* ShowStopper :: https://github.com/CheckPointSW/showstopper
* HackOvert/AntiDBG :: https://github.com/HackOvert/AntiDBG
* ThomasThelen/Anti-Debugging :: https://github.com/ThomasThelen/Anti-Debugging
* revsic/AntiDebugging :: https://github.com/revsic/AntiDebugging
* Aegis :: https://github.com/rafael-santiago/aegis
* Anti-Debug-Collection :: https://github.com/MrakDev/Anti-Debug-Collection
* Blackhat 2012 Presentation Samples :: https://github.com/rrbranco/blackhat2012

* MinigamesAntiCheat :: https://github.com/AdvDebug/MinegamesAntiCheat

* Ahora57/RaceCondition :: https://github.com/Ahora57/RaceCondition
* Ahora57/Unabomber :: https://github.com/Ahora57/Unabomber

* AntiCrack-DotNet :: https://github.com/AdvDebug/AntiCrack-DotNet
* NetShield Protector :: https://github.com/AdvDebug/NetShield_Protector
* MindLated :: https://github.com/Sato-Isolated/MindLated

* ScyllaHide :: https://github.com/x64dbg/ScyllaHide
* HyperHide :: https://github.com/Air14/HyperHide
* Unprotect :: https://github.com/fr0gger/unprotect
