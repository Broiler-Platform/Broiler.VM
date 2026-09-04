// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Broiler.VM.HyperV.Interop;

namespace Broiler.VM.HyperV;

/// <summary>The CPU vendor the hypervisor reports for the host processor.</summary>
public enum ProcessorVendor
{
    /// <summary>The vendor could not be determined.</summary>
    Unknown = -1,

    /// <summary>AMD.</summary>
    Amd = 0,

    /// <summary>Intel.</summary>
    Intel = 1,

    /// <summary>Hygon.</summary>
    Hygon = 2,

    /// <summary>ARM - reported by the ARM64 flavour of the API, which this component does not target.</summary>
    Arm = 0x10,
}

/// <summary>What the host hypervisor reports about itself.</summary>
/// <param name="Vendor">The host CPU vendor.</param>
/// <param name="CacheLineFlushSize">The CLFLUSH line size, in bytes.</param>
/// <param name="PhysicalAddressWidthBits">
/// The width of a guest physical address in bits, or <see langword="null"/> on Windows builds
/// that do not report the capability.
/// </param>
/// <param name="Features">The raw WHV_CAPABILITY_FEATURES bit field.</param>
/// <param name="ExtendedVmExits">The raw WHV_EXTENDED_VM_EXITS bit field: the exits a partition may opt into.</param>
public readonly record struct HyperVPlatformInfo(
    ProcessorVendor Vendor,
    int CacheLineFlushSize,
    int? PhysicalAddressWidthBits,
    ulong Features,
    ulong ExtendedVmExits);

/// <summary>
/// Probes the Windows Hypervisor Platform once per process and reports whether pico VMs can
/// run here.
/// </summary>
/// <remarks>
/// The platform needs three things: 64-bit Windows, a running hypervisor, and the "Windows
/// Hypervisor Platform" optional feature, which is what installs the WinHvPlatform.dll API
/// surface. Hyper-V being installed is not by itself enough - the WHP feature is separate.
/// </remarks>
[SupportedOSPlatform("windows")]
public static class HyperVPlatform
{
    private static readonly Lazy<(bool Available, string? Reason)> Probe = new(RunProbe);

    /// <summary>
    /// Gets a value indicating whether this machine can host pico VMs.
    /// </summary>
    public static bool IsAvailable => Probe.Value.Available;

    /// <summary>
    /// Gets a human readable explanation of why <see cref="IsAvailable"/> is
    /// <see langword="false"/>, or <see langword="null"/> when the platform is available.
    /// </summary>
    public static string? UnavailableReason => Probe.Value.Reason;

    /// <summary>Throws when the platform is unavailable, with an actionable message.</summary>
    /// <exception cref="PlatformNotSupportedException">The platform is unavailable.</exception>
    public static void EnsureAvailable()
    {
        if (!IsAvailable)
        {
            throw new PlatformNotSupportedException(UnavailableReason);
        }
    }

    /// <summary>Reads what the hypervisor reports about the host.</summary>
    /// <returns>The host capability summary.</returns>
    /// <exception cref="PlatformNotSupportedException">The platform is unavailable.</exception>
    /// <exception cref="HyperVException">A capability query failed.</exception>
    public static unsafe HyperVPlatformInfo GetInfo()
    {
        EnsureAvailable();

        ushort vendor = 0;
        _ = TryGetCapability(WhvCapabilityCode.ProcessorVendor, &vendor, sizeof(ushort));

        byte clFlush = 0;
        _ = TryGetCapability(WhvCapabilityCode.ProcessorClFlushSize, &clFlush, sizeof(byte));

        uint addressWidth = 0;
        bool hasWidth = TryGetCapability(WhvCapabilityCode.PhysicalAddressWidth, &addressWidth, sizeof(uint));

        ulong features = 0;
        _ = TryGetCapability(WhvCapabilityCode.Features, &features, sizeof(ulong));

        ulong extendedExits = 0;
        _ = TryGetCapability(WhvCapabilityCode.ExtendedVmExits, &extendedExits, sizeof(ulong));

        return new HyperVPlatformInfo(
            Enum.IsDefined((WhvProcessorVendor)vendor) ? (ProcessorVendor)vendor : ProcessorVendor.Unknown,
            clFlush,
            hasWidth ? (int)addressWidth : null,
            features,
            extendedExits);
    }

    private static unsafe bool TryGetCapability(WhvCapabilityCode code, void* buffer, int size)
    {
        uint written = 0;
        int hr = WhvNative.WHvGetCapability(code, buffer, (uint)size, &written);
        return hr >= 0 && written >= size;
    }

    private static unsafe (bool Available, string? Reason) RunProbe()
    {
        if (!OperatingSystem.IsWindows())
        {
            return (false, "The Windows Hypervisor Platform is only available on Windows.");
        }

        if (RuntimeInformation.ProcessArchitecture != Architecture.X64)
        {
            return (false, string.Format(
                CultureInfo.InvariantCulture,
                "Broiler.VM.HyperV targets x64 guests; this process is {0}.",
                RuntimeInformation.ProcessArchitecture));
        }

        int present = 0;
        uint written = 0;
        int hr;

        try
        {
            hr = WhvNative.WHvGetCapability(
                WhvCapabilityCode.HypervisorPresent,
                &present,
                sizeof(int),
                &written);
        }
        catch (DllNotFoundException)
        {
            return (false, FeatureMissingMessage("WinHvPlatform.dll is not present"));
        }
        catch (EntryPointNotFoundException)
        {
            return (false, FeatureMissingMessage("WinHvPlatform.dll does not export WHvGetCapability"));
        }

        if (hr < 0)
        {
            return (false, string.Format(
                CultureInfo.InvariantCulture,
                "WHvGetCapability(HypervisorPresent) failed with HRESULT 0x{0:X8}.",
                hr));
        }

        if (present == 0)
        {
            return (false, FeatureMissingMessage("no hypervisor is running on this host"));
        }

        return (true, null);
    }

    private static string FeatureMissingMessage(string detail) => string.Format(
        CultureInfo.InvariantCulture,
        "The Windows Hypervisor Platform is not usable here ({0}). Enable it from an elevated "
            + "PowerShell prompt with: Enable-WindowsOptionalFeature -Online -FeatureName "
            + "HypervisorPlatform -All, then reboot. Note that a nested guest also needs "
            + "nested virtualization exposed by its own host.",
        detail);
}
