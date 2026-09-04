// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.IO;
using System.Text;
using System.Threading;
using Broiler.VM.HyperV.Devices;
using Xunit;

namespace Broiler.VM.HyperV.Tests;

/// <summary>
/// End to end tests: real machine code on the real hypervisor. Every test here skips when the
/// Windows Hypervisor Platform is not available, which is the normal state of a machine that is
/// itself a guest without nested virtualization.
/// </summary>
public sealed class PicoVmTests
{
    public PicoVmTests() => Assert.SkipUnless(HyperVPlatform.IsAvailable, Hardware.Reason);

    [Fact]
    public void RealModeAddsTwoImmediates()
    {
        // mov ax, 0x1234 ; mov bx, 0x1111 ; add ax, bx ; hlt
        byte[] code = [0xB8, 0x34, 0x12, 0xBB, 0x11, 0x11, 0x01, 0xD8, 0xF4];

        using PicoVm vm = new(new PicoVmOptions { Mode = GuestMode.Real16 });
        vm.LoadCode(code);
        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal(0x2345UL, vm.Cpu.Rax & 0xFFFF);
    }

    [Fact]
    public void ProtectedModeStoresToGuestRam()
    {
        // mov eax, 7 ; mov ecx, 6 ; imul eax, ecx ; mov [0x3000], eax ; hlt
        byte[] code =
        [
            0xB8, 0x07, 0x00, 0x00, 0x00,
            0xB9, 0x06, 0x00, 0x00, 0x00,
            0x0F, 0xAF, 0xC1,
            0xA3, 0x00, 0x30, 0x00, 0x00,
            0xF4,
        ];

        using PicoVm vm = new(new PicoVmOptions { Mode = GuestMode.Protected32 });
        vm.LoadCode(code);
        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal(42u, vm.Memory.ReadUInt32(0x3000));
    }

    [Fact]
    public void LongModeRunsACountedLoop()
    {
        // xor eax, eax ; mov ecx, 100 ; add rax, rcx ; dec ecx ; jnz ; hlt
        byte[] code =
        [
            0x31, 0xC0,
            0xB9, 0x64, 0x00, 0x00, 0x00,
            0x48, 0x01, 0xC8,
            0xFF, 0xC9,
            0x75, 0xF9,
            0xF4,
        ];

        using PicoVm vm = new();
        vm.LoadCode(code);
        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal(5050UL, vm.Cpu.Rax);

        // The halt exit reports the address after the instruction, not of it.
        Assert.Equal(vm.CodeAddress + (ulong)code.Length, exit.Rip);
        Assert.Equal(1, exit.ExitCount);
    }

    [Fact]
    public void LongModeStartsWithPagingOnAndFlatSegments()
    {
        using PicoVm vm = new();
        GuestState state = vm.Cpu.Capture();

        Assert.Equal(0x80000031UL, state.Cr0);  // PG | NE | ET | PE
        Assert.Equal(0x620UL, state.Cr4);       // PAE | OSFXSR | OSXMMEXCPT
        Assert.Equal(0x500UL, state.Efer);      // LMA | LME
        Assert.Equal(vm.Options.PageTableAddress, state.Cr3);
        Assert.Equal(vm.CodeAddress, state.Rip);
        Assert.Equal(vm.Options.MemorySize, state.Rsp);
    }

    [Fact]
    public void GuestWritesReachThePortDevice()
    {
        ConsolePortDevice console = new(TextWriter.Null, capture: true);

        // mov rsi, 0x8000 ; mov dx, 0xE9 ; lodsb ; test al,al ; jz done ; out dx,al ; jmp next ; hlt
        // The string sits at 0x8000, clear of the identity page table the machine reserves at 0x1000.
        byte[] code =
        [
            0x48, 0xC7, 0xC6, 0x00, 0x80, 0x00, 0x00,
            0x66, 0xBA, 0xE9, 0x00,
            0xAC,
            0x84, 0xC0,
            0x74, 0x03,
            0xEE,
            0xEB, 0xF8,
            0xF4,
        ];

        using PicoVm vm = new();
        vm.LoadCode(code);
        vm.Io.Map(0xE9, console);
        vm.Memory.Write(0x8000, Encoding.ASCII.GetBytes("pico\0"));

        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal("pico", console.Text);
        Assert.Equal(5, exit.ExitCount); // four bytes out, then the halt
    }

    [Fact]
    public void GuestReadsFromThePortDevice()
    {
        // mov dx, 0x100 ; in al, dx ; hlt
        byte[] code = [0x66, 0xBA, 0x00, 0x01, 0xEC, 0xF4];

        using PicoVm vm = new();
        vm.LoadCode(code);
        vm.Io.Map(0x100, new DelegatePortDevice(read: (_, _) => 0x5A));

        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal(0x5AUL, vm.Cpu.Rax & 0xFF);
    }

    [Fact]
    public void GuestReachesAMemoryMappedDevice()
    {
        ulong stored = 0;
        DelegateMemoryMappedDevice device = new(
            0x2000_0000,
            0x1000,
            read: (_, destination) => destination.Fill(0xAB),
            write: (_, source) =>
            {
                stored = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    stored |= (ulong)source[i] << (i * 8);
                }
            });

        // mov rax, 0xc0ffee ; mov rbx, 0x20000000 ; mov [rbx], rax ; mov rcx, [rbx+8] ; hlt
        byte[] code =
        [
            0x48, 0xC7, 0xC0, 0xEE, 0xFF, 0xC0, 0x00,
            0x48, 0xC7, 0xC3, 0x00, 0x00, 0x00, 0x20,
            0x48, 0x89, 0x03,
            0x48, 0x8B, 0x4B, 0x08,
            0xF4,
        ];

        using PicoVm vm = new();
        vm.LoadCode(code);
        vm.Mmio.Map(device);

        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal(0x00C0FFEEUL, stored);
        Assert.Equal(0xABABABABABABABABUL, vm.Cpu.Rcx);
    }

    [Fact]
    public void UnbackedMemoryStopsTheRun()
    {
        // mov rax, 0x10000000 ; jmp rax
        byte[] code = [0x48, 0xC7, 0xC0, 0x00, 0x00, 0x00, 0x10, 0xFF, 0xE0];

        using PicoVm vm = new();
        vm.LoadCode(code);

        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.MemoryFault, exit.Reason);
        Assert.Equal(0x1000_0000UL, exit.FaultAddress);
        Assert.Equal(MemoryAccessKind.Read, exit.FaultAccess);
    }

    [Fact]
    public void UnclaimedPortsCanFloatOrStopTheRun()
    {
        // in al, 0x99 ; hlt
        byte[] code = [0xE4, 0x99, 0xF4];

        using (PicoVm floating = new())
        {
            floating.LoadCode(code);
            VmExit exit = floating.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

            Assert.Equal(VmExitReason.Halted, exit.Reason);
            Assert.Equal(0xFFUL, floating.Cpu.Rax & 0xFF);
        }

        using PicoVm strict = new(new PicoVmOptions { StopOnUnhandledPort = true });
        strict.LoadCode(code);
        VmExit stopped = strict.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.UnhandledPort, stopped.Reason);
        Assert.Equal((ushort?)0x99, stopped.Port);
    }

    [Fact]
    public void AStoppedRunResumesWhereItLeftOff()
    {
        // in al, 0x99 ; hlt
        byte[] code = [0xE4, 0x99, 0xF4];

        using PicoVm vm = new(new PicoVmOptions { StopOnUnhandledPort = true });
        vm.LoadCode(code);

        Assert.Equal(VmExitReason.UnhandledPort, vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken).Reason);

        // Nothing was retired, so plugging the hole and running again completes the instruction.
        vm.Io.Map(0x99, new DelegatePortDevice(read: (_, _) => 0x42));
        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal(0x42UL, vm.Cpu.Rax & 0xFF);
    }

    [Fact]
    public void AnEndlessGuestIsStoppedByItsBudget()
    {
        byte[] code = [0xEB, 0xFE]; // jmp $

        using PicoVm vm = new();
        vm.LoadCode(code);

        VmExit exit = vm.Run(TimeSpan.FromMilliseconds(250), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.Canceled, exit.Reason);
        Assert.Contains("budget", exit.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ACancellationTokenStopsTheGuest()
    {
        byte[] code = [0xEB, 0xFE]; // jmp $

        using PicoVm vm = new();
        vm.LoadCode(code);

        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMilliseconds(250));

        Assert.Equal(VmExitReason.Canceled, vm.Run(cts.Token).Reason);
    }

    [Fact]
    public void TheExitLimitStopsAGuestThatNeverHalts()
    {
        // mov dx, 0xE9 ; out dx, al ; jmp back
        byte[] code = [0x66, 0xBA, 0xE9, 0x00, 0xEE, 0xEB, 0xFD];

        using PicoVm vm = new(new PicoVmOptions { ExitLimit = 4 });
        vm.LoadCode(code);
        vm.Io.MapWriter(0xE9, static _ => { });

        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.ExitLimitReached, exit.Reason);
        Assert.Equal(4, exit.ExitCount);
    }

    [Fact]
    public void ResetRunsTheSameCodeAgain()
    {
        // mov eax, 1 ; inc eax ; hlt
        byte[] code = [0xB8, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xC0, 0xF4];

        using PicoVm vm = new();
        vm.LoadCode(code);

        Assert.Equal(VmExitReason.Halted, vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken).Reason);
        Assert.Equal(2UL, vm.Cpu.Rax);

        vm.Cpu.Rax = 0xDEAD;
        vm.Reset();

        Assert.Equal(VmExitReason.Halted, vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken).Reason);
        Assert.Equal(2UL, vm.Cpu.Rax);
    }

    [Fact]
    public void MachinesAreIndependent()
    {
        // mov eax, imm32 ; hlt
        byte[] first = [0xB8, 0x01, 0x00, 0x00, 0x00, 0xF4];
        byte[] second = [0xB8, 0x02, 0x00, 0x00, 0x00, 0xF4];

        ulong a;
        ulong b;

        using (PicoVm one = new())
        {
            one.LoadCode(first);
            _ = one.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
            a = one.Cpu.Rax;
        }

        using (PicoVm two = new())
        {
            two.LoadCode(second);
            _ = two.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
            b = two.Cpu.Rax;
        }

        Assert.Equal(1UL, a);
        Assert.Equal(2UL, b);
    }

    [Fact]
    public void ASecondLiveMachineIsRefusedWithAnActionableMessage()
    {
        using PicoVm first = new();

        // Windows names a partition's memory block after the owning process, so the second
        // partition in a process cannot map guest RAM. If a future build lifts that, this test
        // sees no exception and the message check simply drops out.
        Exception? error = Record.Exception(() => new PicoVm().Dispose());

        if (error is not null)
        {
            HyperVException refused = Assert.IsType<HyperVException>(error);
            Assert.Contains("one partition per process", refused.Message, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void CodeCannotOverwriteTheReservedPageTable()
    {
        using PicoVm vm = new();

        Assert.NotNull(vm.ReservedRange);
        (ulong address, ulong size) = vm.ReservedRange!.Value;
        Assert.Equal(vm.Options.PageTableAddress, address);
        Assert.Equal(3 * PicoVmOptions.PageSize, size);

        Assert.Throws<ArgumentException>(() => vm.LoadCode(address, new byte[16]));
    }

    [Fact]
    public void RealModeMachinesReserveNothing()
    {
        using PicoVm vm = new(new PicoVmOptions { Mode = GuestMode.Real16 });

        Assert.Null(vm.ReservedRange);
    }

    [Fact]
    public void GuestMemoryIsSharedWithTheHost()
    {
        using PicoVm vm = new();

        vm.Memory.WriteUInt64(0x5000, 0x0123456789ABCDEF);
        Assert.Equal(0x0123456789ABCDEFUL, vm.Memory.ReadUInt64(0x5000));
        Assert.Equal(0xEF, vm.Memory.ReadUInt8(0x5000));
        Assert.Equal(0xCDEF, vm.Memory.ReadUInt16(0x5000));

        // mov rax, [0x5000] via rbx, then store it back one page up
        byte[] code =
        [
            0x48, 0xC7, 0xC3, 0x00, 0x50, 0x00, 0x00,   // mov rbx, 0x5000
            0x48, 0x8B, 0x03,                           // mov rax, [rbx]
            0x48, 0x89, 0x83, 0x00, 0x10, 0x00, 0x00,   // mov [rbx+0x1000], rax
            0xF4,
        ];

        vm.LoadCode(code);
        Assert.Equal(VmExitReason.Halted, vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken).Reason);
        Assert.Equal(0x0123456789ABCDEFUL, vm.Memory.ReadUInt64(0x6000));
    }

    [Fact]
    public void ReadOnlyRegionsRejectGuestWrites()
    {
        using PicoVm vm = new(new PicoVmOptions { MemorySize = 2 * 1024 * 1024 });
        _ = vm.Memory.Map(0x30_0000, 4096, GuestMemoryAccess.Read);

        // mov rbx, 0x300000 ; mov qword [rbx], rax ; hlt
        byte[] code =
        [
            0x48, 0xC7, 0xC3, 0x00, 0x00, 0x30, 0x00,
            0x48, 0x89, 0x03,
            0xF4,
        ];

        vm.LoadCode(code);
        VmExit exit = vm.Run(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        Assert.Equal(VmExitReason.MemoryFault, exit.Reason);
        Assert.Equal(0x30_0000UL, exit.FaultAddress);
        Assert.Equal(MemoryAccessKind.Write, exit.FaultAccess);
    }

    [Fact]
    public void ExecuteRunsAThrowawayMachine()
    {
        byte[] code = [0xB8, 0x07, 0x00, 0x00, 0x00, 0xF4]; // mov eax, 7 ; hlt

        (VmExit exit, GuestState state) = PicoVm.Execute(code);

        Assert.Equal(VmExitReason.Halted, exit.Reason);
        Assert.Equal(7UL, state.Rax);
    }

    [Fact]
    public void ADisposedMachineRefusesToRun()
    {
        PicoVm vm = new();
        vm.Dispose();

        Assert.Throws<ObjectDisposedException>(() => vm.Run(TestContext.Current.CancellationToken));
        vm.Dispose(); // idempotent
    }

    [Fact]
    public void DeviceWindowsCannotOverlapRam()
    {
        using PicoVm vm = new();

        Assert.Throws<InvalidOperationException>(
            () => vm.Mmio.Map(new DelegateMemoryMappedDevice(0x1000, 0x1000)));
    }

    [Fact]
    public void PortsCannotBeClaimedTwice()
    {
        using PicoVm vm = new();
        vm.Io.MapWriter(0xE9, static _ => { });

        Assert.Throws<InvalidOperationException>(() => vm.Io.MapWriter(0xE9, static _ => { }));
    }
}
