// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Broiler.VM.HyperV;
using Broiler.VM.HyperV.Devices;

namespace Broiler.VM.HyperV.Cli;

/// <summary>
/// A hand assembled program, the machine it wants, and what the host should find afterwards.
/// </summary>
/// <remarks>
/// The machine code is written out byte by byte on purpose. These samples are the reference for
/// what a pico VM actually executes, so an assembler between the listing and the bytes would only
/// hide the thing being demonstrated.
/// </remarks>
internal sealed class SampleProgram
{
    internal required string Name { get; init; }

    internal required string Summary { get; init; }

    internal required GuestMode Mode { get; init; }

    internal required byte[] Code { get; init; }

    /// <summary>The assembly listing the bytes were produced from.</summary>
    internal required string Listing { get; init; }

    /// <summary>Attaches devices and seeds guest memory before the run.</summary>
    internal Action<PicoVm>? Prepare { get; init; }

    /// <summary>
    /// Checks the machine after the run. Returns null when the program did what it should, or a
    /// description of what went wrong.
    /// </summary>
    internal Func<PicoVm, VmExit, string?>? Verify { get; init; }

    /// <summary>Describes the result for a human.</summary>
    internal Func<PicoVm, VmExit, string>? Report { get; init; }

    internal static IReadOnlyList<SampleProgram> All { get; } =
    [
        Add16(),
        Hello16(),
        Sum64(),
        Multiply32(),
        Mmio64(),
        WildJump(),
    ];

    internal static SampleProgram? Find(string name)
    {
        foreach (SampleProgram program in All)
        {
            if (string.Equals(program.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return program;
            }
        }

        return null;
    }

    /// <summary>16-bit real mode: add two immediates and stop.</summary>
    private static SampleProgram Add16() => new()
    {
        Name = "add16",
        Summary = "real mode 16-bit: 0x1234 + 0x1111 in ax",
        Mode = GuestMode.Real16,
        Listing = """
            b8 34 12    mov ax, 0x1234
            bb 11 11    mov bx, 0x1111
            01 d8       add ax, bx
            f4          hlt
            """,
        Code =
        [
            0xB8, 0x34, 0x12,
            0xBB, 0x11, 0x11,
            0x01, 0xD8,
            0xF4,
        ],
        Verify = static (vm, exit) => exit.IsHalt && (vm.Cpu.Rax & 0xFFFF) == 0x2345
            ? null
            : Format("expected ax=0x2345 after a halt, got ax=0x{0:X4} and {1}", vm.Cpu.Rax & 0xFFFF, exit.Reason),
        Report = static (vm, _) => Format("ax = 0x{0:X4}", vm.Cpu.Rax & 0xFFFF),
    };

    /// <summary>16-bit real mode: walk a host-written string and push it out a port.</summary>
    private static SampleProgram Hello16()
    {
        ConsolePortDevice console = new(capture: true);

        return new SampleProgram
        {
            Name = "hello16",
            Summary = "real mode 16-bit: print a host-written string through port 0xE9",
            Mode = GuestMode.Real16,
            Listing = """
                be 00 20    mov si, 0x2000      ; ds is based at 0, so this is a physical address
                ba e9 00    mov dx, 0x00e9
                ac          lodsb               ; next:
                84 c0       test al, al
                74 03       jz done
                ee          out dx, al
                eb f8       jmp next
                f4          hlt                 ; done:
                """,
            Code =
            [
                0xBE, 0x00, 0x20,
                0xBA, 0xE9, 0x00,
                0xAC,
                0x84, 0xC0,
                0x74, 0x03,
                0xEE,
                0xEB, 0xF8,
                0xF4,
            ],
            Prepare = vm =>
            {
                vm.Io.Map(0xE9, console);
                vm.Memory.Write(0x2000, Encoding.ASCII.GetBytes("hello from a pico VM\n\0"));
            },
            Verify = (_, exit) => exit.IsHalt && console.Text.StartsWith("hello from a pico VM", StringComparison.Ordinal)
                ? null
                : Format("expected the guest to print through port 0xE9, got {0} and {1} byte(s)", exit.Reason, console.Text.Length),
            Report = (_, _) => Format("the guest wrote {0} byte(s) to port 0xE9", console.Text.Length),
        };
    }

    /// <summary>64-bit long mode: a counted loop.</summary>
    private static SampleProgram Sum64() => new()
    {
        Name = "sum64",
        Summary = "long mode 64-bit: sum 1..100 into rax with a counted loop",
        Mode = GuestMode.Long64,
        Listing = """
            31 c0       xor eax, eax
            b9 64 00 00 00  mov ecx, 100
            48 01 c8    add rax, rcx        ; loop:
            ff c9       dec ecx
            75 f9       jnz loop
            f4          hlt
            """,
        Code =
        [
            0x31, 0xC0,
            0xB9, 0x64, 0x00, 0x00, 0x00,
            0x48, 0x01, 0xC8,
            0xFF, 0xC9,
            0x75, 0xF9,
            0xF4,
        ],
        Verify = static (vm, exit) => exit.IsHalt && vm.Cpu.Rax == 5050
            ? null
            : Format("expected rax=5050 after a halt, got rax={0} and {1}", vm.Cpu.Rax, exit.Reason),
        Report = static (vm, _) => Format("rax = {0}", vm.Cpu.Rax),
    };

    /// <summary>32-bit protected mode: multiply and store the answer in guest RAM.</summary>
    private static SampleProgram Multiply32() => new()
    {
        Name = "mul32",
        Summary = "protected mode 32-bit: 7 * 6, stored at guest physical 0x3000",
        Mode = GuestMode.Protected32,
        Listing = """
            b8 07 00 00 00  mov eax, 7
            b9 06 00 00 00  mov ecx, 6
            0f af c1        imul eax, ecx
            a3 00 30 00 00  mov [0x3000], eax
            f4              hlt
            """,
        Code =
        [
            0xB8, 0x07, 0x00, 0x00, 0x00,
            0xB9, 0x06, 0x00, 0x00, 0x00,
            0x0F, 0xAF, 0xC1,
            0xA3, 0x00, 0x30, 0x00, 0x00,
            0xF4,
        ],
        Verify = static (vm, exit) => exit.IsHalt && vm.Memory.ReadUInt32(0x3000) == 42
            ? null
            : Format("expected 42 at 0x3000 after a halt, got {0} and {1}", vm.Memory.ReadUInt32(0x3000), exit.Reason),
        Report = static (vm, _) => Format("[0x3000] = {0}", vm.Memory.ReadUInt32(0x3000)),
    };

    /// <summary>64-bit long mode: talk to a device that has no RAM behind it.</summary>
    private static SampleProgram Mmio64()
    {
        // A one register device at 512 MiB: inside the identity map, far outside the 4 MiB of RAM,
        // so every access faults out to the host and is replayed against these callbacks.
        ulong stored = 0;
        DelegateMemoryMappedDevice device = new(
            baseAddress: 0x2000_0000,
            length: 0x1000,
            read: (offset, destination) =>
            {
                ulong value = offset == 8 ? 0x5AFE_D00D_5AFE_D00D : stored;
                for (int i = 0; i < destination.Length; i++)
                {
                    destination[i] = (byte)(value >> (i * 8));
                }
            },
            write: (offset, source) =>
            {
                ulong value = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    value |= (ulong)source[i] << (i * 8);
                }

                stored = value;
            });

        return new SampleProgram
        {
            Name = "mmio64",
            Summary = "long mode 64-bit: store to and load from a memory mapped device at 0x20000000",
            Mode = GuestMode.Long64,
            Listing = """
                48 c7 c0 ee ff c0 00    mov rax, 0x00c0ffee
                48 c7 c3 00 00 00 20    mov rbx, 0x20000000
                48 89 03                mov [rbx], rax
                48 8b 4b 08             mov rcx, [rbx+8]
                f4                      hlt
                """,
            Code =
            [
                0x48, 0xC7, 0xC0, 0xEE, 0xFF, 0xC0, 0x00,
                0x48, 0xC7, 0xC3, 0x00, 0x00, 0x00, 0x20,
                0x48, 0x89, 0x03,
                0x48, 0x8B, 0x4B, 0x08,
                0xF4,
            ],
            Prepare = vm => vm.Mmio.Map(device),
            Verify = (vm, exit) => exit.IsHalt && stored == 0x00C0FFEE && vm.Cpu.Rcx == 0x5AFE_D00D_5AFE_D00D
                ? null
                : Format(
                    "expected the device to see 0x00c0ffee and the guest to read back 0x5afed00d5afed00d, got 0x{0:X} and rcx=0x{1:X} ({2})",
                    stored,
                    vm.Cpu.Rcx,
                    exit.Reason),
            Report = (vm, _) => Format("the device stored 0x{0:X} and the guest read back rcx=0x{1:X}", stored, vm.Cpu.Rcx),
        };
    }

    /// <summary>What a guest going wrong looks like from the host.</summary>
    private static SampleProgram WildJump() => new()
    {
        Name = "fault64",
        Summary = "long mode 64-bit: jump into unbacked memory and watch the host catch it",
        Mode = GuestMode.Long64,
        Listing = """
            48 c7 c0 00 00 00 10    mov rax, 0x10000000
            ff e0                   jmp rax
            """,
        Code =
        [
            0x48, 0xC7, 0xC0, 0x00, 0x00, 0x00, 0x10,
            0xFF, 0xE0,
        ],
        Verify = static (_, exit) => exit.Reason == VmExitReason.MemoryFault && exit.FaultAddress == 0x1000_0000
            ? null
            : Format("expected a memory fault at 0x10000000, got {0} at 0x{1:X}", exit.Reason, exit.FaultAddress ?? 0),
        Report = static (_, exit) => Format(
            "{0} while trying to {1} 0x{2:X}",
            exit.Reason,
            exit.FaultAccess?.ToString().ToLowerInvariant() ?? "reach",
            exit.FaultAddress ?? 0),
    };

    private static string Format(string format, params object?[] args) =>
        string.Format(CultureInfo.InvariantCulture, format, args);
}
