// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.IO;
using System.Text;

namespace Broiler.VM.HyperV.Devices;

/// <summary>
/// A device on the guest's I/O port space, driven by the guest's IN and OUT instructions.
/// </summary>
public interface IPortDevice
{
    /// <summary>Services an IN from the guest.</summary>
    /// <param name="port">The port being read.</param>
    /// <param name="size">The width of the access in bytes: 1, 2 or 4.</param>
    /// <returns>The value to hand the guest, in the low <paramref name="size"/> bytes.</returns>
    uint Read(ushort port, int size);

    /// <summary>Services an OUT from the guest.</summary>
    /// <param name="port">The port being written.</param>
    /// <param name="size">The width of the access in bytes: 1, 2 or 4.</param>
    /// <param name="value">The value the guest wrote, in the low <paramref name="size"/> bytes.</param>
    void Write(ushort port, int size, uint value);
}

/// <summary>
/// A port device built from callbacks, for the common case where a whole class would be noise.
/// </summary>
public sealed class DelegatePortDevice : IPortDevice
{
    private readonly Func<ushort, int, uint>? read;
    private readonly Action<ushort, int, uint>? write;

    /// <summary>Initializes a new instance of the <see cref="DelegatePortDevice"/> class.</summary>
    /// <param name="read">Services IN, or <see langword="null"/> to read back all-ones.</param>
    /// <param name="write">Services OUT, or <see langword="null"/> to discard writes.</param>
    public DelegatePortDevice(Func<ushort, int, uint>? read = null, Action<ushort, int, uint>? write = null)
    {
        this.read = read;
        this.write = write;
    }

    /// <inheritdoc/>
    public uint Read(ushort port, int size) =>
        read is null ? Masks.AllOnes(size) : read(port, size);

    /// <inheritdoc/>
    public void Write(ushort port, int size, uint value) => write?.Invoke(port, size, value);
}

/// <summary>
/// A write-only character device: every byte the guest writes is appended to a text writer.
/// </summary>
/// <remarks>
/// This is the pico equivalent of a serial console. Point a guest at it with a one byte OUT -
/// <c>out 0xE9, al</c> is the conventional debug port on a PC - and its output shows up on the
/// host without the guest needing a driver, an interrupt or a single mapped device page.
/// </remarks>
public sealed class ConsolePortDevice : IPortDevice
{
    private readonly TextWriter writer;
    private readonly StringBuilder captured = new();
    private readonly bool capture;

    /// <summary>Initializes a new instance of the <see cref="ConsolePortDevice"/> class.</summary>
    /// <param name="writer">Where characters go. Defaults to <see cref="Console.Out"/>.</param>
    /// <param name="capture">
    /// When <see langword="true"/>, everything written is also kept in <see cref="Text"/>.
    /// </param>
    public ConsolePortDevice(TextWriter? writer = null, bool capture = false)
    {
        this.writer = writer ?? Console.Out;
        this.capture = capture;
    }

    /// <summary>Everything the guest has written so far, when capturing was requested.</summary>
    public string Text => captured.ToString();

    /// <summary>Reads back zero: the console has nothing to say.</summary>
    /// <param name="port">The port being read.</param>
    /// <param name="size">The width of the access in bytes.</param>
    /// <returns>Zero.</returns>
    public uint Read(ushort port, int size) => 0;

    /// <inheritdoc/>
    public void Write(ushort port, int size, uint value)
    {
        for (int i = 0; i < size; i++)
        {
            char c = (char)((value >> (i * 8)) & 0xFF);
            writer.Write(c);

            if (capture)
            {
                _ = captured.Append(c);
            }
        }
    }
}

/// <summary>Width helpers shared by the device implementations.</summary>
internal static class Masks
{
    /// <summary>The all-ones value an absent device reads back at the given width.</summary>
    internal static uint AllOnes(int size) => size >= 4 ? uint.MaxValue : (1u << (size * 8)) - 1;

    /// <summary>Truncates a value to the given width.</summary>
    internal static uint Truncate(uint value, int size) => size >= 4 ? value : value & ((1u << (size * 8)) - 1);
}
