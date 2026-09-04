// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Broiler.VM.HyperV.Interop;

/// <summary>
/// The Windows Hypervisor Platform entry points this component uses.
/// </summary>
/// <remarks>
/// WinHvPlatform.dll and WinHvEmulation.dll live in System32 and are only present when the
/// "Windows Hypervisor Platform" optional feature is enabled, so every call site has to be
/// prepared for <see cref="DllNotFoundException"/> and <see cref="EntryPointNotFoundException"/>.
/// <see cref="HyperVPlatform"/> does that probing once.
/// </remarks>
[SupportedOSPlatform("windows")]
internal static unsafe partial class WhvNative
{
    private const string PlatformLibrary = "WinHvPlatform.dll";
    private const string EmulationLibrary = "WinHvEmulation.dll";

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvGetCapability")]
    internal static partial int WHvGetCapability(
        WhvCapabilityCode capabilityCode,
        void* capabilityBuffer,
        uint capabilityBufferSizeInBytes,
        uint* writtenSizeInBytes);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvCreatePartition")]
    internal static partial int WHvCreatePartition(nint* partition);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvSetupPartition")]
    internal static partial int WHvSetupPartition(nint partition);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvDeletePartition")]
    internal static partial int WHvDeletePartition(nint partition);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvSetPartitionProperty")]
    internal static partial int WHvSetPartitionProperty(
        nint partition,
        WhvPartitionPropertyCode propertyCode,
        void* propertyBuffer,
        uint propertyBufferSizeInBytes);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvGetPartitionProperty")]
    internal static partial int WHvGetPartitionProperty(
        nint partition,
        WhvPartitionPropertyCode propertyCode,
        void* propertyBuffer,
        uint propertyBufferSizeInBytes,
        uint* writtenSizeInBytes);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvMapGpaRange")]
    internal static partial int WHvMapGpaRange(
        nint partition,
        void* sourceAddress,
        ulong guestAddress,
        ulong sizeInBytes,
        WhvMapGpaRangeFlags flags);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvUnmapGpaRange")]
    internal static partial int WHvUnmapGpaRange(nint partition, ulong guestAddress, ulong sizeInBytes);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvTranslateGva")]
    internal static partial int WHvTranslateGva(
        nint partition,
        uint vpIndex,
        ulong gva,
        WhvTranslateGvaFlags translateFlags,
        WhvTranslateGvaResultCode* translationResult,
        ulong* gpa);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvCreateVirtualProcessor")]
    internal static partial int WHvCreateVirtualProcessor(nint partition, uint vpIndex, uint flags);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvDeleteVirtualProcessor")]
    internal static partial int WHvDeleteVirtualProcessor(nint partition, uint vpIndex);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvRunVirtualProcessor")]
    internal static partial int WHvRunVirtualProcessor(
        nint partition,
        uint vpIndex,
        void* exitContext,
        uint exitContextSizeInBytes);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvCancelRunVirtualProcessor")]
    internal static partial int WHvCancelRunVirtualProcessor(nint partition, uint vpIndex, uint flags);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvGetVirtualProcessorRegisters")]
    internal static partial int WHvGetVirtualProcessorRegisters(
        nint partition,
        uint vpIndex,
        WhvRegisterName* registerNames,
        uint registerCount,
        WhvRegisterValue* registerValues);

    [LibraryImport(PlatformLibrary, EntryPoint = "WHvSetVirtualProcessorRegisters")]
    internal static partial int WHvSetVirtualProcessorRegisters(
        nint partition,
        uint vpIndex,
        WhvRegisterName* registerNames,
        uint registerCount,
        WhvRegisterValue* registerValues);

    [LibraryImport(EmulationLibrary, EntryPoint = "WHvEmulatorCreateEmulator")]
    internal static partial int WHvEmulatorCreateEmulator(WhvEmulatorCallbacks* callbacks, nint* emulator);

    [LibraryImport(EmulationLibrary, EntryPoint = "WHvEmulatorDestroyEmulator")]
    internal static partial int WHvEmulatorDestroyEmulator(nint emulator);

    [LibraryImport(EmulationLibrary, EntryPoint = "WHvEmulatorTryIoEmulation")]
    internal static partial int WHvEmulatorTryIoEmulation(
        nint emulator,
        void* context,
        WhvVpExitContext* vpContext,
        WhvIoPortAccessContext* ioInstructionContext,
        WhvEmulatorStatus* emulatorReturnStatus);

    [LibraryImport(EmulationLibrary, EntryPoint = "WHvEmulatorTryMmioEmulation")]
    internal static partial int WHvEmulatorTryMmioEmulation(
        nint emulator,
        void* context,
        WhvVpExitContext* vpContext,
        WhvMemoryAccessContext* mmioInstructionContext,
        WhvEmulatorStatus* emulatorReturnStatus);
}
