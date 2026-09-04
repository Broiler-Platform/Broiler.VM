// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using Xunit;

namespace Broiler.VM.HyperV.Tests;

/// <summary>
/// The platform probe has to give a straight answer on any machine, including one that cannot
/// host a guest at all.
/// </summary>
public sealed class HyperVPlatformTests
{
    [Fact]
    public void AvailabilityAndReasonAgree()
    {
        if (HyperVPlatform.IsAvailable)
        {
            Assert.Null(HyperVPlatform.UnavailableReason);
        }
        else
        {
            Assert.False(string.IsNullOrWhiteSpace(HyperVPlatform.UnavailableReason));
        }
    }

    [Fact]
    public void ProbingIsStable()
    {
        bool first = HyperVPlatform.IsAvailable;
        Assert.Equal(first, HyperVPlatform.IsAvailable);
    }

    [Fact]
    public void AnUnavailablePlatformExplainsItself()
    {
        Assert.SkipWhen(HyperVPlatform.IsAvailable, "the platform is available here");

        PlatformNotSupportedException error = Assert.Throws<PlatformNotSupportedException>(HyperVPlatform.EnsureAvailable);
        Assert.Contains("Hypervisor Platform", error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void TheHostReportsItsProcessor()
    {
        Assert.SkipUnless(HyperVPlatform.IsAvailable, Hardware.Reason);

        HyperVPlatformInfo info = HyperVPlatform.GetInfo();

        Assert.NotEqual(ProcessorVendor.Unknown, info.Vendor);
        Assert.InRange(info.PhysicalAddressWidthBits ?? 48, 32, 64);
    }
}
