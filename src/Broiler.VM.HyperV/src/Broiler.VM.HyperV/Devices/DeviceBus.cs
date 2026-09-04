// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.Versioning;

namespace Broiler.VM.HyperV.Devices;

/// <summary>
/// The guest's I/O port space: which device, if any, answers each of the 65536 ports.
/// </summary>
public sealed class IoBus
{
    private readonly Dictionary<ushort, IPortDevice> byPort = new();

    /// <summary>The ports that currently have a device behind them.</summary>
    public IReadOnlyCollection<ushort> MappedPorts => byPort.Keys;

    /// <summary>Puts a device behind a single port.</summary>
    /// <param name="port">The port number.</param>
    /// <param name="device">The device to answer with.</param>
    /// <exception cref="InvalidOperationException">The port already has a device.</exception>
    public void Map(ushort port, IPortDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (!byPort.TryAdd(port, device))
        {
            throw new InvalidOperationException(string.Format(
                CultureInfo.InvariantCulture,
                "Port 0x{0:X4} already has a device.",
                port));
        }
    }

    /// <summary>Puts one device behind a contiguous range of ports.</summary>
    /// <param name="firstPort">The first port in the range.</param>
    /// <param name="portCount">How many ports the device claims.</param>
    /// <param name="device">The device to answer with.</param>
    public void Map(ushort firstPort, int portCount, IPortDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(portCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(firstPort + portCount, 0x10000);

        for (int i = 0; i < portCount; i++)
        {
            Map((ushort)(firstPort + i), device);
        }
    }

    /// <summary>
    /// Puts a byte sink behind a port, the shape almost every pico guest wants: a single
    /// <c>out dx, al</c> that the host observes.
    /// </summary>
    /// <param name="port">The port number.</param>
    /// <param name="onWrite">Called with each byte the guest writes.</param>
    public void MapWriter(ushort port, Action<byte> onWrite)
    {
        ArgumentNullException.ThrowIfNull(onWrite);

        Map(port, new DelegatePortDevice(write: (_, size, value) =>
        {
            for (int i = 0; i < size; i++)
            {
                onWrite((byte)((value >> (i * 8)) & 0xFF));
            }
        }));
    }

    /// <summary>Tests whether a port has a device.</summary>
    /// <param name="port">The port number.</param>
    /// <returns><see langword="true"/> when a device is mapped.</returns>
    public bool IsMapped(ushort port) => byPort.ContainsKey(port);

    /// <summary>Finds the device behind a port.</summary>
    /// <param name="port">The port number.</param>
    /// <returns>The device, or <see langword="null"/> when the port is unclaimed.</returns>
    public IPortDevice? Find(ushort port) => byPort.GetValueOrDefault(port);
}

/// <summary>
/// The memory mapped devices in a guest's physical address space, at addresses where there is
/// no RAM.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class MmioBus
{
    private readonly List<IMemoryMappedDevice> devices = new();
    private readonly GuestMemory memory;

    internal MmioBus(GuestMemory memory) => this.memory = memory;

    /// <summary>The mapped devices, in the order they were added.</summary>
    public IReadOnlyList<IMemoryMappedDevice> Devices => new ReadOnlyCollection<IMemoryMappedDevice>(devices);

    /// <summary>Adds a device to the guest's physical address space.</summary>
    /// <param name="device">The device, which carries its own base address and length.</param>
    /// <exception cref="InvalidOperationException">
    /// The window overlaps guest RAM or another device. A device window has to sit at addresses
    /// with no RAM behind them, otherwise the guest would reach the RAM and never fault out.
    /// </exception>
    public void Map(IMemoryMappedDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);

        if (device.Length == 0)
        {
            throw new ArgumentException("A device window cannot be empty.", nameof(device));
        }

        ulong end = device.BaseAddress + device.Length;

        foreach (MemoryRegion region in memory.Regions)
        {
            if (device.BaseAddress < region.EndAddress && region.GuestAddress < end)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.InvariantCulture,
                    "The device window 0x{0:X}-0x{1:X} overlaps guest RAM at 0x{2:X}-0x{3:X}.",
                    device.BaseAddress,
                    end,
                    region.GuestAddress,
                    region.EndAddress));
            }
        }

        foreach (IMemoryMappedDevice existing in devices)
        {
            if (device.BaseAddress < existing.BaseAddress + existing.Length && existing.BaseAddress < end)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.InvariantCulture,
                    "The device window 0x{0:X}-0x{1:X} overlaps the device already at 0x{2:X}.",
                    device.BaseAddress,
                    end,
                    existing.BaseAddress));
            }
        }

        devices.Add(device);
    }

    /// <summary>Finds the device covering a guest physical range.</summary>
    /// <param name="guestAddress">The first address of the access.</param>
    /// <param name="length">The width of the access in bytes.</param>
    /// <returns>The device, or <see langword="null"/> when nothing covers the whole range.</returns>
    public IMemoryMappedDevice? Find(ulong guestAddress, ulong length)
    {
        foreach (IMemoryMappedDevice device in devices)
        {
            if (guestAddress >= device.BaseAddress && guestAddress + length <= device.BaseAddress + device.Length)
            {
                return device;
            }
        }

        return null;
    }
}
