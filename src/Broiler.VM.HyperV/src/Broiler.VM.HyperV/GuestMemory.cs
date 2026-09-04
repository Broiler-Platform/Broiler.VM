// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Broiler.VM.HyperV.Interop;

namespace Broiler.VM.HyperV;

/// <summary>
/// One contiguous range of host memory handed to a partition as guest RAM.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed unsafe class MemoryRegion
{
    private readonly byte* host;

    internal MemoryRegion(ulong guestAddress, ulong size, byte* host, GuestMemoryAccess access)
    {
        GuestAddress = guestAddress;
        Size = size;
        Access = access;
        this.host = host;
    }

    /// <summary>The guest physical address the region starts at.</summary>
    public ulong GuestAddress { get; }

    /// <summary>The size of the region in bytes, always a multiple of the page size.</summary>
    public ulong Size { get; }

    /// <summary>The access the guest has to the region.</summary>
    public GuestMemoryAccess Access { get; }

    /// <summary>The first guest physical address past the region.</summary>
    public ulong EndAddress => GuestAddress + Size;

    internal byte* HostPointer => host;

    /// <summary>Tests whether a guest physical address falls inside the region.</summary>
    /// <param name="guestAddress">The address to test.</param>
    /// <returns><see langword="true"/> when the address is inside the region.</returns>
    public bool Contains(ulong guestAddress) =>
        guestAddress >= GuestAddress && guestAddress < EndAddress;

    /// <summary>Tests whether a whole range falls inside the region.</summary>
    /// <param name="guestAddress">The first address of the range.</param>
    /// <param name="length">The length of the range in bytes.</param>
    /// <returns><see langword="true"/> when the range is entirely inside the region.</returns>
    public bool Contains(ulong guestAddress, ulong length) =>
        guestAddress >= GuestAddress
        && length <= EndAddress - guestAddress
        && guestAddress <= EndAddress;

    /// <summary>
    /// Views the whole region as a span. Regions larger than <see cref="int.MaxValue"/> cannot be
    /// spanned in one piece; use <see cref="GuestMemory.Slice"/> for those.
    /// </summary>
    /// <returns>A span over the region's backing memory.</returns>
    /// <exception cref="InvalidOperationException">The region is too large to span.</exception>
    public Span<byte> AsSpan()
    {
        if (Size > int.MaxValue)
        {
            throw new InvalidOperationException(string.Format(
                CultureInfo.InvariantCulture,
                "The region at 0x{0:X} is {1} bytes, which is more than a single span can address.",
                GuestAddress,
                Size));
        }

        return new Span<byte>(host, (int)Size);
    }
}

/// <summary>
/// The guest physical address space of a pico VM: the regions of host memory the partition can
/// see, plus the read and write helpers a host uses to load code and collect results.
/// </summary>
/// <remarks>
/// Guest RAM is ordinary committed host memory that the hypervisor pins and maps into the
/// partition. Writing to it from the host while the guest is running is allowed and is how the
/// samples hand work to a guest; there is no coherence problem on x86 because both sides see
/// the same physical pages.
/// </remarks>
[SupportedOSPlatform("windows")]
public sealed unsafe class GuestMemory : IDisposable
{
    private readonly nint partition;
    private readonly List<MemoryRegion> regions = new();
    private bool disposed;

    internal GuestMemory(nint partition) => this.partition = partition;

    /// <summary>The mapped regions, in the order they were created.</summary>
    public IReadOnlyList<MemoryRegion> Regions => new ReadOnlyCollection<MemoryRegion>(regions);

    /// <summary>The total number of bytes mapped into the guest.</summary>
    public ulong TotalSize
    {
        get
        {
            ulong total = 0;
            foreach (MemoryRegion region in regions)
            {
                total += region.Size;
            }

            return total;
        }
    }

    /// <summary>
    /// Allocates host memory and maps it into the partition as guest RAM.
    /// </summary>
    /// <param name="guestAddress">The guest physical address to map at. Must be page aligned.</param>
    /// <param name="size">The size in bytes. Must be a non-zero multiple of the page size.</param>
    /// <param name="access">The access the guest is granted.</param>
    /// <returns>The mapped region.</returns>
    /// <exception cref="ArgumentException">The address or size is not page aligned.</exception>
    /// <exception cref="InvalidOperationException">The range overlaps an existing region.</exception>
    /// <exception cref="HyperVException">The hypervisor refused the mapping.</exception>
    public MemoryRegion Map(ulong guestAddress, ulong size, GuestMemoryAccess access = GuestMemoryAccess.ReadWriteExecute)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (size == 0 || size % PicoVmOptions.PageSize != 0)
        {
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, "Size {0} must be a non-zero multiple of {1}.", size, PicoVmOptions.PageSize),
                nameof(size));
        }

        if (guestAddress % PicoVmOptions.PageSize != 0)
        {
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, "Guest address 0x{0:X} must be page aligned.", guestAddress),
                nameof(guestAddress));
        }

        foreach (MemoryRegion existing in regions)
        {
            if (guestAddress < existing.EndAddress && existing.GuestAddress < guestAddress + size)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.InvariantCulture,
                    "The range 0x{0:X}-0x{1:X} overlaps the region already mapped at 0x{2:X}-0x{3:X}.",
                    guestAddress,
                    guestAddress + size,
                    existing.GuestAddress,
                    existing.EndAddress));
            }
        }

        void* host = Kernel32.VirtualAlloc(
            null,
            (nuint)size,
            Kernel32.MemCommit | Kernel32.MemReserve,
            Kernel32.PageReadWrite);

        if (host is null)
        {
            throw new OutOfMemoryException(string.Format(
                CultureInfo.InvariantCulture,
                "VirtualAlloc could not commit {0} bytes of guest RAM (Win32 error {1}).",
                size,
                Marshal.GetLastPInvokeError()));
        }

        int hr = WhvNative.WHvMapGpaRange(partition, host, guestAddress, size, ToMapFlags(access));
        if (hr < 0)
        {
            _ = Kernel32.VirtualFree(host, 0, Kernel32.MemRelease);
            throw HyperVException.Create(hr, "WHvMapGpaRange");
        }

        MemoryRegion region = new(guestAddress, size, (byte*)host, access);
        regions.Add(region);
        return region;
    }

    /// <summary>Finds the region containing a guest physical address.</summary>
    /// <param name="guestAddress">The address to resolve.</param>
    /// <returns>The region, or <see langword="null"/> when the address is unmapped.</returns>
    public MemoryRegion? FindRegion(ulong guestAddress)
    {
        foreach (MemoryRegion region in regions)
        {
            if (region.Contains(guestAddress))
            {
                return region;
            }
        }

        return null;
    }

    /// <summary>Views a range of guest physical memory as a span.</summary>
    /// <param name="guestAddress">The first address of the range.</param>
    /// <param name="length">The length of the range in bytes.</param>
    /// <returns>A span over the guest's own memory - writes through it are visible to the guest.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The range is not entirely mapped.</exception>
    public Span<byte> Slice(ulong guestAddress, int length)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        if (!TryResolve(guestAddress, (ulong)length, out byte* pointer))
        {
            throw new ArgumentOutOfRangeException(
                nameof(guestAddress),
                string.Format(
                    CultureInfo.InvariantCulture,
                    "The range 0x{0:X}+{1} is not mapped into the guest.",
                    guestAddress,
                    length));
        }

        return new Span<byte>(pointer, length);
    }

    /// <summary>Copies bytes into guest memory.</summary>
    /// <param name="guestAddress">The destination guest physical address.</param>
    /// <param name="data">The bytes to copy.</param>
    public void Write(ulong guestAddress, ReadOnlySpan<byte> data) => data.CopyTo(Slice(guestAddress, data.Length));

    /// <summary>Copies bytes out of guest memory.</summary>
    /// <param name="guestAddress">The source guest physical address.</param>
    /// <param name="destination">The buffer to fill.</param>
    public void Read(ulong guestAddress, Span<byte> destination) => Slice(guestAddress, destination.Length).CopyTo(destination);

    /// <summary>Reads a block of guest memory into a new array.</summary>
    /// <param name="guestAddress">The source guest physical address.</param>
    /// <param name="count">The number of bytes to read.</param>
    /// <returns>The bytes read.</returns>
    public byte[] ReadBytes(ulong guestAddress, int count)
    {
        byte[] buffer = new byte[count];
        Read(guestAddress, buffer);
        return buffer;
    }

    /// <summary>Reads one byte of guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <returns>The byte at that address.</returns>
    public byte ReadUInt8(ulong guestAddress) => Slice(guestAddress, 1)[0];

    /// <summary>Writes one byte of guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <param name="value">The value to store.</param>
    public void WriteUInt8(ulong guestAddress, byte value) => Slice(guestAddress, 1)[0] = value;

    /// <summary>Reads a little-endian 16-bit word from guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <returns>The value at that address.</returns>
    public ushort ReadUInt16(ulong guestAddress) => BinaryPrimitives.ReadUInt16LittleEndian(Slice(guestAddress, sizeof(ushort)));

    /// <summary>Writes a little-endian 16-bit word to guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <param name="value">The value to store.</param>
    public void WriteUInt16(ulong guestAddress, ushort value) =>
        BinaryPrimitives.WriteUInt16LittleEndian(Slice(guestAddress, sizeof(ushort)), value);

    /// <summary>Reads a little-endian 32-bit word from guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <returns>The value at that address.</returns>
    public uint ReadUInt32(ulong guestAddress) => BinaryPrimitives.ReadUInt32LittleEndian(Slice(guestAddress, sizeof(uint)));

    /// <summary>Writes a little-endian 32-bit word to guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <param name="value">The value to store.</param>
    public void WriteUInt32(ulong guestAddress, uint value) =>
        BinaryPrimitives.WriteUInt32LittleEndian(Slice(guestAddress, sizeof(uint)), value);

    /// <summary>Reads a little-endian 64-bit word from guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <returns>The value at that address.</returns>
    public ulong ReadUInt64(ulong guestAddress) => BinaryPrimitives.ReadUInt64LittleEndian(Slice(guestAddress, sizeof(ulong)));

    /// <summary>Writes a little-endian 64-bit word to guest memory.</summary>
    /// <param name="guestAddress">The guest physical address.</param>
    /// <param name="value">The value to store.</param>
    public void WriteUInt64(ulong guestAddress, ulong value) =>
        BinaryPrimitives.WriteUInt64LittleEndian(Slice(guestAddress, sizeof(ulong)), value);

    /// <summary>Fills a range of guest memory with a repeated byte.</summary>
    /// <param name="guestAddress">The first address to fill.</param>
    /// <param name="length">The number of bytes to fill.</param>
    /// <param name="value">The byte to write.</param>
    public void Fill(ulong guestAddress, int length, byte value) => Slice(guestAddress, length).Fill(value);

    internal bool TryResolve(ulong guestAddress, ulong length, out byte* pointer)
    {
        foreach (MemoryRegion region in regions)
        {
            if (region.Contains(guestAddress, length))
            {
                pointer = region.HostPointer + (guestAddress - region.GuestAddress);
                return true;
            }
        }

        pointer = null;
        return false;
    }

    /// <summary>Unmaps every region from the partition and releases the host memory backing it.</summary>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        foreach (MemoryRegion region in regions)
        {
            // A best effort: the partition may already be gone, and there is nothing useful to
            // do about a failure here beyond still releasing the host pages.
            _ = WhvNative.WHvUnmapGpaRange(partition, region.GuestAddress, region.Size);
            _ = Kernel32.VirtualFree(region.HostPointer, 0, Kernel32.MemRelease);
        }

        regions.Clear();
    }

    private static WhvMapGpaRangeFlags ToMapFlags(GuestMemoryAccess access)
    {
        WhvMapGpaRangeFlags flags = WhvMapGpaRangeFlags.None;

        if ((access & GuestMemoryAccess.Read) != 0)
        {
            flags |= WhvMapGpaRangeFlags.Read;
        }

        if ((access & GuestMemoryAccess.Write) != 0)
        {
            flags |= WhvMapGpaRangeFlags.Write;
        }

        if ((access & GuestMemoryAccess.Execute) != 0)
        {
            flags |= WhvMapGpaRangeFlags.Execute;
        }

        return flags;
    }
}
