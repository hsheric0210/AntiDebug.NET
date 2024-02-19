/*
 * DLLFromMemory.Net
 *
 * Load a native DLL from memory without the need to allow unsafe code
 *
 * Copyright (C) 2018 - 2019 by Bernhard Schelling
 *
 * Based on Memory Module.net 0.2
 * Copyright (C) 2012 - 2018 by Andreas Kanzler (andi_kanzler(at)gmx.de)
 * https://github.com/Scavanger/MemoryModule.net
 *
 * Based on Memory DLL loading code Version 0.0.4
 * Copyright (C) 2004 - 2015 by Joachim Bauch (mail(at)joachim-bauch.de)
 * https://github.com/fancycode/MemoryModule
 *
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 2.0 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is MemoryModule.c
 *
 * The Initial Developer of the Original Code is Joachim Bauch.
 *
 * Portions created by Joachim Bauch are Copyright (C) 2004 - 2015
 * Joachim Bauch. All Rights Reserved.
 *
 * Portions created by Andreas Kanzler are Copyright (C) 2012 - 2018
 * Andreas Kanzler. All Rights Reserved.
 *
 * Portions created by Bernhard Schelling are Copyright (C) 2018 - 2019
 * Bernhard Schelling. All Rights Reserved.
 *
 */

using System;
using System.Runtime.InteropServices;

public partial class DLLFromMemory
{
    static T PtrRead<T>(IntPtr ptr) => (T)Marshal.PtrToStructure(ptr, typeof(T));
    static void PtrWrite<T>(IntPtr ptr, T val) => Marshal.StructureToPtr(val, ptr, false);
    static IntPtr PtrAdd(IntPtr p, int v) => (IntPtr)(p.ToInt64() + v);
    static IntPtr PtrAdd(IntPtr p, uint v) => IntPtr.Size == 8 ? (IntPtr)(p.ToInt64() + unchecked(v)) : (IntPtr)(p.ToInt32() + unchecked((int)v));
    static IntPtr PtrAdd(IntPtr p, IntPtr v) => IntPtr.Size == 8 ? (IntPtr)(p.ToInt64() + v.ToInt64()) : (IntPtr)(p.ToInt32() + v.ToInt32());
    static IntPtr PtrAdd(IntPtr p, UIntPtr v) => IntPtr.Size == 8 ? (IntPtr)(p.ToInt64() + unchecked((long)v.ToUInt64())) : (IntPtr)(p.ToInt32() + unchecked((int)v.ToUInt32()));
    static IntPtr PtrSub(IntPtr p, IntPtr v) => IntPtr.Size == 8 ? (IntPtr)(p.ToInt64() - v.ToInt64()) : (IntPtr)(p.ToInt32() - v.ToInt32());
    static IntPtr PtrBitOr(IntPtr p, UIntPtr v) => IntPtr.Size == 8 ? (IntPtr)unchecked((long)(unchecked((ulong)p.ToInt64()) | v.ToUInt64())) : (IntPtr)unchecked((int)(unchecked((uint)p.ToInt32()) | v.ToUInt32()));
    static IntPtr PtrAlignDown(IntPtr p, UIntPtr align) => (IntPtr)unchecked((long)(unchecked((ulong)p.ToInt64()) & ~(align.ToUInt64() - 1)));
    static bool PtrIsInvalidHandle(IntPtr h) => h == IntPtr.Zero || h == (IntPtr.Size == 8 ? (IntPtr)(long)-1 : (IntPtr)(-1));
    static bool PtrSpanBoundary(IntPtr p, uint Size, int BoundaryBits) => unchecked((ulong)p.ToInt64()) >> BoundaryBits < (unchecked((ulong)p.ToInt64()) + Size) >> BoundaryBits;
}