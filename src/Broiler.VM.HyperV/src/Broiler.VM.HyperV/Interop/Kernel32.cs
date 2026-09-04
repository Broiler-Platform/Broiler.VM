// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Broiler.VM.HyperV.Interop;

/// <summary>
/// Virtual memory primitives. Guest RAM has to be page aligned, committed, private memory:
/// WHvMapGpaRange pins the pages and hands them to the hypervisor, which rules out the
/// managed heap and rules out anything the allocator might decommit underneath it.
/// </summary>
[SupportedOSPlatform("windows")]
internal static unsafe partial class Kernel32
{
    internal const uint MemCommit = 0x00001000;
    internal const uint MemReserve = 0x00002000;
    internal const uint MemRelease = 0x00008000;
    internal const uint PageReadWrite = 0x04;

    [LibraryImport("kernel32.dll", EntryPoint = "VirtualAlloc", SetLastError = true)]
    internal static partial void* VirtualAlloc(void* address, nuint size, uint allocationType, uint protect);

    [LibraryImport("kernel32.dll", EntryPoint = "VirtualFree", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool VirtualFree(void* address, nuint size, uint freeType);
}
