// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;

namespace Broiler.VM.HyperV.Devices;

/// <summary>
/// A device that answers guest loads and stores at a range of guest physical addresses that has
/// no RAM behind it.
/// </summary>
/// <remarks>
/// The hypervisor faults the guest out on the first touch of an unmapped page. The host then
/// has the instruction emulator replay the access against this interface, so a device sees the
/// same reads and writes real silicon would, at the width the guest used.
/// </remarks>
public interface IMemoryMappedDevice
{
    /// <summary>The guest physical address the device answers from.</summary>
    ulong BaseAddress { get; }

    /// <summary>The size of the device window in bytes.</summary>
    ulong Length { get; }

    /// <summary>Services a guest load.</summary>
    /// <param name="offset">The offset into the window, in bytes.</param>
    /// <param name="destination">The buffer to fill: 1, 2, 4 or 8 bytes wide.</param>
    void Read(ulong offset, Span<byte> destination);

    /// <summary>Services a guest store.</summary>
    /// <param name="offset">The offset into the window, in bytes.</param>
    /// <param name="source">The bytes the guest wrote: 1, 2, 4 or 8 bytes wide.</param>
    void Write(ulong offset, ReadOnlySpan<byte> source);
}

/// <summary>
/// A memory mapped device built from callbacks.
/// </summary>
public sealed class DelegateMemoryMappedDevice : IMemoryMappedDevice
{
    private readonly Action<ulong, Span<byte>>? read;
    private readonly Action<ulong, ReadOnlySpan<byte>>? write;

    /// <summary>Initializes a new instance of the <see cref="DelegateMemoryMappedDevice"/> class.</summary>
    /// <param name="baseAddress">The guest physical address of the window.</param>
    /// <param name="length">The size of the window in bytes.</param>
    /// <param name="read">Services loads, or <see langword="null"/> to read back zero.</param>
    /// <param name="write">Services stores, or <see langword="null"/> to discard them.</param>
    public DelegateMemoryMappedDevice(
        ulong baseAddress,
        ulong length,
        Action<ulong, Span<byte>>? read = null,
        Action<ulong, ReadOnlySpan<byte>>? write = null)
    {
        ArgumentOutOfRangeException.ThrowIfZero(length);

        BaseAddress = baseAddress;
        Length = length;
        this.read = read;
        this.write = write;
    }

    /// <inheritdoc/>
    public ulong BaseAddress { get; }

    /// <inheritdoc/>
    public ulong Length { get; }

    /// <inheritdoc/>
    public void Read(ulong offset, Span<byte> destination)
    {
        if (read is null)
        {
            destination.Clear();
            return;
        }

        read(offset, destination);
    }

    /// <inheritdoc/>
    public void Write(ulong offset, ReadOnlySpan<byte> source) => write?.Invoke(offset, source);
}
