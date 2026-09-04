// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using Xunit;

namespace Broiler.VM.HyperV.Tests;

/// <summary>
/// Configuration is checked before a partition is created, so these run on any machine whether
/// or not it can host a guest.
/// </summary>
public sealed class OptionValidationTests
{
    [Fact]
    public void DefaultsDescribeALongModeMachine()
    {
        PicoVmOptions options = new();

        Assert.Equal(GuestMode.Long64, options.Mode);
        Assert.Equal(4UL * 1024 * 1024, options.MemorySize);
        Assert.Equal(0x10000UL, options.CodeAddress ?? 0x10000UL);
    }

    [Fact]
    public void MemorySizeMustBePageAligned() =>
        Assert.Throws<ArgumentException>(() => new PicoVm(new PicoVmOptions { MemorySize = (64 * 1024) + 1 }));

    [Fact]
    public void MemorySizeHasAFloor() =>
        Assert.Throws<ArgumentException>(() => new PicoVm(new PicoVmOptions { MemorySize = 4096 }));

    [Fact]
    public void CodeMustLandInsideRam() =>
        Assert.Throws<ArgumentException>(() => new PicoVm(new PicoVmOptions
        {
            MemorySize = 64 * 1024,
            CodeAddress = 0x40000,
        }));

    [Fact]
    public void RealModeCodeMustBeParagraphAligned() =>
        Assert.Throws<ArgumentException>(() => new PicoVm(new PicoVmOptions
        {
            Mode = GuestMode.Real16,
            CodeAddress = 0x1008,
        }));

    [Fact]
    public void LongModeCodeMustNotOverlapThePageTable() =>
        Assert.Throws<ArgumentException>(() => new PicoVm(new PicoVmOptions
        {
            Mode = GuestMode.Long64,
            PageTableAddress = 0x1000,
            CodeAddress = 0x2000,
        }));

    [Fact]
    public void PageTableMustFitInRam() =>
        Assert.Throws<ArgumentException>(() => new PicoVm(new PicoVmOptions
        {
            Mode = GuestMode.Long64,
            MemorySize = 64 * 1024,
            PageTableAddress = 0xF000,
        }));

    [Fact]
    public void OptionsAreSnapshotted()
    {
        Assert.SkipUnless(HyperVPlatform.IsAvailable, Hardware.Reason);

        PicoVmOptions options = new() { Mode = GuestMode.Real16, MemorySize = 128 * 1024 };
        using PicoVm vm = new(options);

        options.MemorySize = 1024 * 1024;

        Assert.Equal(128UL * 1024, vm.Options.MemorySize);
        Assert.Equal(128UL * 1024, vm.Memory.TotalSize);
    }
}

/// <summary>Shared skip reason for the tests that need a hypervisor.</summary>
internal static class Hardware
{
    internal static string Reason =>
        HyperVPlatform.UnavailableReason ?? "the Windows Hypervisor Platform is unavailable";
}
