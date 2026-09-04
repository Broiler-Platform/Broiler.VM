// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using Broiler.VM.HyperV.Boot;
using Broiler.VM.HyperV.Devices;
using Broiler.VM.HyperV.Interop;

namespace Broiler.VM.HyperV;

/// <summary>
/// A pico virtual machine: one hardware partition, one virtual processor, a slab of RAM, and
/// whatever devices the host chooses to put behind ports and unmapped addresses.
/// </summary>
/// <remarks>
/// <para>
/// The machine has no firmware, no interrupt table and no operating system. It is created in the
/// processor mode the caller asks for with its instruction pointer aimed at loaded machine code,
/// and it runs until that code executes HLT, touches something the host does not answer, or the
/// host cancels it. That makes it a way to run a few bytes of x86-64 on real silicon under
/// hardware isolation, with the same setup cost as calling a function.
/// </para>
/// <para>
/// A machine is single threaded: create it, load code, run it and dispose it on one thread.
/// <see cref="Run(CancellationToken)"/> may be interrupted from another thread through its
/// cancellation token, which is the only member safe to reach concurrently.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using PicoVm vm = new();
/// vm.Io.MapWriter(0xE9, b => Console.Write((char)b));
/// vm.LoadCode([0xB0, 0x21, 0xE6, 0xE9, 0xF4]); // mov al, '!' ; out 0xE9, al ; hlt
/// VmExit exit = vm.Run();
/// </code>
/// </example>
[SupportedOSPlatform("windows")]
public sealed unsafe class PicoVm : IDisposable
{
    private const uint VpIndex = 0;
    private const int EFail = unchecked((int)0x80004005);

    private readonly PicoVmOptions options;
    private readonly GCHandle self;
    private readonly nint partition;
    private readonly GuestMemory memory;
    private readonly VirtualProcessor cpu;
    private readonly IoBus io;
    private readonly MmioBus mmio;

    private nint emulator;
    private bool vpCreated;
    private bool disposed;

    private ushort? unclaimedPort;
    private ulong? unclaimedMemory;
    private MemoryAccessKind unclaimedMemoryAccess;

    /// <summary>Creates a pico VM with the default configuration: 4 MiB of RAM in long mode.</summary>
    public PicoVm()
        : this(new PicoVmOptions())
    {
    }

    /// <summary>Creates a pico VM.</summary>
    /// <param name="options">
    /// The machine configuration. A snapshot is taken, so later edits to the object do not
    /// affect this machine.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    /// <exception cref="ArgumentException">The configuration cannot describe a runnable machine.</exception>
    /// <exception cref="PlatformNotSupportedException">The Windows Hypervisor Platform is unavailable.</exception>
    /// <exception cref="HyperVException">The hypervisor refused to create the partition.</exception>
    public PicoVm(PicoVmOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        HyperVPlatform.EnsureAvailable();

        options.Validate();
        this.options = options.Clone();

        nint handle;
        HyperVException.ThrowIfFailed(WhvNative.WHvCreatePartition(&handle), "WHvCreatePartition");
        partition = handle;

        try
        {
            uint processorCount = 1;
            HyperVException.ThrowIfFailed(
                WhvNative.WHvSetPartitionProperty(
                    partition,
                    WhvPartitionPropertyCode.ProcessorCount,
                    &processorCount,
                    sizeof(uint)),
                "WHvSetPartitionProperty(ProcessorCount)");

            HyperVException.ThrowIfFailed(WhvNative.WHvSetupPartition(partition), "WHvSetupPartition");

            memory = new GuestMemory(partition);
            _ = memory.Map(0, this.options.MemorySize, this.options.MemoryAccess);

            HyperVException.ThrowIfFailed(
                WhvNative.WHvCreateVirtualProcessor(partition, VpIndex, 0),
                "WHvCreateVirtualProcessor");
            vpCreated = true;

            cpu = new VirtualProcessor(partition, VpIndex);
            io = new IoBus();
            mmio = new MmioBus(memory);

            GuestBootstrap.Apply(cpu, memory, this.options);

            // Weak on purpose: a strong handle would root the machine in its own field, so a
            // caller who forgot to dispose would leak the partition with no finaliser able to
            // reclaim it. The emulator only calls back while Run is on the stack, and the
            // GC.KeepAlive calls around those native calls hold the machine alive across them.
            self = GCHandle.Alloc(this, GCHandleType.Weak);
        }
        catch
        {
            Cleanup();
            throw;
        }
    }

    /// <summary>Frees the partition if the caller forgot to.</summary>
    ~PicoVm() => Cleanup();

    /// <summary>The configuration this machine was built from.</summary>
    public PicoVmOptions Options => options;

    /// <summary>The guest physical address space.</summary>
    public GuestMemory Memory => memory;

    /// <summary>The machine's single virtual processor.</summary>
    public VirtualProcessor Cpu => cpu;

    /// <summary>The guest's I/O port space.</summary>
    public IoBus Io => io;

    /// <summary>The guest's memory mapped devices.</summary>
    public MmioBus Mmio => mmio;

    /// <summary>The guest physical address <see cref="LoadCode(ReadOnlySpan{byte})"/> writes to.</summary>
    public ulong CodeAddress => options.EffectiveCodeAddress;

    /// <summary>
    /// The guest physical range the machine keeps for itself, or <see langword="null"/> when it
    /// keeps none.
    /// </summary>
    /// <remarks>
    /// In long mode this is the three pages of identity-mapping page table. The guest reads them
    /// on every memory access, so a host that scribbles data over them takes the machine down at
    /// the next instruction. The defaults keep the whole low 64 KiB clear of loaded code for
    /// exactly this reason.
    /// </remarks>
    public (ulong Address, ulong Size)? ReservedRange => options.Mode == GuestMode.Long64
        ? (options.PageTableAddress, 3 * PicoVmOptions.PageSize)
        : null;

    /// <summary>How the last run ended, or <see langword="null"/> if the machine has not run yet.</summary>
    public VmExit? LastExit { get; private set; }

    /// <summary>
    /// Runs machine code in a throwaway pico VM and reports how it ended.
    /// </summary>
    /// <param name="code">The machine code to run.</param>
    /// <param name="options">The machine configuration, or <see langword="null"/> for the defaults.</param>
    /// <param name="configure">
    /// Called after the machine is built and the code is loaded, to attach devices or seed
    /// registers before the run starts.
    /// </param>
    /// <returns>How the run ended, and the guest registers at that point.</returns>
    public static (VmExit Exit, GuestState State) Execute(
        ReadOnlySpan<byte> code,
        PicoVmOptions? options = null,
        Action<PicoVm>? configure = null)
    {
        using PicoVm vm = new(options ?? new PicoVmOptions());
        vm.LoadCode(code);
        configure?.Invoke(vm);
        VmExit exit = vm.Run();
        return (exit, vm.Cpu.Capture());
    }

    /// <summary>Writes machine code at <see cref="CodeAddress"/>, where execution starts.</summary>
    /// <param name="code">The machine code to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">The code does not fit in guest RAM.</exception>
    public void LoadCode(ReadOnlySpan<byte> code) => LoadCode(CodeAddress, code);

    /// <summary>Writes machine code at a guest physical address.</summary>
    /// <param name="guestAddress">Where to write it.</param>
    /// <param name="code">The machine code to load.</param>
    /// <exception cref="ArgumentOutOfRangeException">The code does not fit in guest RAM.</exception>
    /// <exception cref="ArgumentException">The code would overwrite <see cref="ReservedRange"/>.</exception>
    public void LoadCode(ulong guestAddress, ReadOnlySpan<byte> code)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (ReservedRange is { } reserved
            && guestAddress < reserved.Address + reserved.Size
            && reserved.Address < guestAddress + (ulong)code.Length)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Loading {0} byte(s) at 0x{1:X} would overwrite the page table the machine reserved at 0x{2:X}-0x{3:X}.",
                    code.Length,
                    guestAddress,
                    reserved.Address,
                    reserved.Address + reserved.Size),
                nameof(guestAddress));
        }

        memory.Write(guestAddress, code);
    }

    /// <summary>
    /// Puts the processor back in its starting state: the entry point, stack pointer and mode
    /// registers the machine was created with. Guest memory is left alone, so the same loaded
    /// code can be run again.
    /// </summary>
    public void Reset()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        GuestBootstrap.Apply(cpu, memory, options);
        LastExit = null;
    }

    /// <summary>
    /// Runs the guest until it halts, faults, or the run is cancelled.
    /// </summary>
    /// <param name="cancellationToken">Stops the guest, wherever it has got to.</param>
    /// <returns>How the run ended.</returns>
    /// <exception cref="HyperVException">The hypervisor failed to run the processor.</exception>
    public VmExit Run(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        using CancellationTokenRegistration registration = cancellationToken.UnsafeRegister(
            static state => ((PicoVm)state!).RequestCancel(),
            this);

        int exits = 0;

        while (true)
        {
            WhvRunVpExitContext exit = default;

            HyperVException.ThrowIfFailed(
                WhvNative.WHvRunVirtualProcessor(partition, VpIndex, &exit, (uint)sizeof(WhvRunVpExitContext)),
                "WHvRunVirtualProcessor");

            exits++;

            VmExit? outcome = Dispatch(ref exit, exits);
            if (outcome is not null)
            {
                LastExit = outcome;
                return outcome;
            }

            if (options.ExitLimit > 0 && exits >= options.ExitLimit)
            {
                LastExit = Exit(
                    VmExitReason.ExitLimitReached,
                    ref exit,
                    exits,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "the run serviced its limit of {0} exits without the guest halting",
                        options.ExitLimit));
                return LastExit;
            }
        }
    }

    /// <summary>Runs the guest with a wall-clock budget.</summary>
    /// <param name="timeout">How long the guest may run before it is cancelled.</param>
    /// <param name="cancellationToken">Also stops the guest.</param>
    /// <returns>How the run ended.</returns>
    public VmExit Run(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        VmExit exit = Run(cts.Token);

        if (exit.Reason == VmExitReason.Canceled && !cancellationToken.IsCancellationRequested)
        {
            exit = new VmExit(
                VmExitReason.Canceled,
                exit.Rip,
                exit.Rflags,
                exit.InstructionLength,
                exit.ExitCount,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "the guest was still running after its {0:0.###} ms budget",
                    timeout.TotalMilliseconds));
            LastExit = exit;
        }

        return exit;
    }

    /// <summary>Asks a running guest to stop. Safe to call from any thread.</summary>
    public void RequestCancel()
    {
        if (disposed)
        {
            return;
        }

        _ = WhvNative.WHvCancelRunVirtualProcessor(partition, VpIndex, 0);
    }

    /// <summary>Tears down the partition and releases the guest's memory.</summary>
    public void Dispose()
    {
        Cleanup();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Services one exit. Returns null when the guest should keep running, otherwise the outcome
    /// to report.
    /// </summary>
    private VmExit? Dispatch(ref WhvRunVpExitContext exit, int exits) => exit.ExitReason switch
    {
        WhvRunVpExitReason.X64Halt => Exit(VmExitReason.Halted, ref exit, exits, "the guest executed hlt"),
        WhvRunVpExitReason.X64IoPortAccess => ServicePortAccess(ref exit, exits),
        WhvRunVpExitReason.MemoryAccess => ServiceMemoryAccess(ref exit, exits),
        WhvRunVpExitReason.Canceled => Exit(VmExitReason.Canceled, ref exit, exits, "the host cancelled the run"),
        WhvRunVpExitReason.UnrecoverableException => Exit(
            VmExitReason.UnrecoverableException,
            ref exit,
            exits,
            "the guest raised a fault it cannot recover from - with no interrupt table loaded this is "
                + "what a bad instruction, a wild jump or a stack overflow looks like"),
        WhvRunVpExitReason.InvalidVpRegisterValue => Exit(
            VmExitReason.InvalidRegisterValue,
            ref exit,
            exits,
            "the processor was left in a register state the hardware refuses to enter"),
        WhvRunVpExitReason.UnsupportedFeature => Exit(
            VmExitReason.UnsupportedFeature,
            ref exit,
            exits,
            "the guest used a processor feature this partition does not provide"),
        _ => Exit(
            VmExitReason.Unexpected,
            ref exit,
            exits,
            string.Format(CultureInfo.InvariantCulture, "unhandled exit reason {0}", exit.ExitReason)),
    };

    private VmExit? ServicePortAccess(ref WhvRunVpExitContext exit, int exits)
    {
        unclaimedPort = null;
        WhvEmulatorStatus status = default;

        int hr;
        fixed (WhvRunVpExitContext* context = &exit)
        {
            hr = WhvNative.WHvEmulatorTryIoEmulation(
                EnsureEmulator(),
                (void*)GCHandle.ToIntPtr(self),
                &context->VpContext,
                &context->IoPortAccess,
                &status);
        }

        GC.KeepAlive(this);
        HyperVException.ThrowIfFailed(hr, "WHvEmulatorTryIoEmulation");

        if (status.EmulationSuccessful)
        {
            return null;
        }

        if (unclaimedPort is { } port)
        {
            return Exit(
                VmExitReason.UnhandledPort,
                ref exit,
                exits,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "the guest touched I/O port 0x{0:X4}, which has no device behind it",
                    port),
                port: port);
        }

        return Exit(
            VmExitReason.EmulationFailed,
            ref exit,
            exits,
            string.Format(
                CultureInfo.InvariantCulture,
                "the instruction emulator could not replay the port access (status 0x{0:X8})",
                status.AsUInt32));
    }

    private VmExit? ServiceMemoryAccess(ref WhvRunVpExitContext exit, int exits)
    {
        unclaimedMemory = null;
        WhvEmulatorStatus status = default;

        int hr;
        fixed (WhvRunVpExitContext* context = &exit)
        {
            hr = WhvNative.WHvEmulatorTryMmioEmulation(
                EnsureEmulator(),
                (void*)GCHandle.ToIntPtr(self),
                &context->VpContext,
                &context->MemoryAccess,
                &status);
        }

        GC.KeepAlive(this);
        HyperVException.ThrowIfFailed(hr, "WHvEmulatorTryMmioEmulation");

        if (status.EmulationSuccessful)
        {
            return null;
        }

        if (unclaimedMemory is { } gpa)
        {
            return Exit(
                VmExitReason.MemoryFault,
                ref exit,
                exits,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "the guest tried to {0} guest physical address 0x{1:X}, which has neither RAM nor a device behind it",
                    unclaimedMemoryAccess.ToString().ToLowerInvariant(),
                    gpa),
                faultAddress: gpa,
                faultAccess: unclaimedMemoryAccess);
        }

        return Exit(
            VmExitReason.EmulationFailed,
            ref exit,
            exits,
            string.Format(
                CultureInfo.InvariantCulture,
                "the instruction emulator could not replay the access to 0x{0:X} (status 0x{1:X8})",
                exit.MemoryAccess.Gpa,
                status.AsUInt32));
    }

    private static VmExit Exit(
        VmExitReason reason,
        ref WhvRunVpExitContext exit,
        int exits,
        string message,
        ulong? faultAddress = null,
        MemoryAccessKind? faultAccess = null,
        ushort? port = null) => new(
            reason,
            exit.VpContext.Rip,
            exit.VpContext.Rflags,
            exit.VpContext.InstructionLength,
            exits,
            message,
            faultAddress,
            faultAccess,
            port);

    private nint EnsureEmulator()
    {
        if (emulator != 0)
        {
            return emulator;
        }

        WhvEmulatorCallbacks callbacks = new()
        {
            Size = (uint)sizeof(WhvEmulatorCallbacks),
            IoPort = &OnIoPort,
            Memory = &OnMemory,
            GetRegisters = &OnGetRegisters,
            SetRegisters = &OnSetRegisters,
            TranslateGvaPage = &OnTranslateGvaPage,
        };

        nint handle;
        HyperVException.ThrowIfFailed(
            WhvNative.WHvEmulatorCreateEmulator(&callbacks, &handle),
            "WHvEmulatorCreateEmulator");

        emulator = handle;
        return emulator;
    }

    private void Cleanup()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        if (emulator != 0)
        {
            _ = WhvNative.WHvEmulatorDestroyEmulator(emulator);
            emulator = 0;
        }

        // Unmap before the partition goes away, then release the host pages behind guest RAM.
        memory?.Dispose();

        if (vpCreated)
        {
            _ = WhvNative.WHvDeleteVirtualProcessor(partition, VpIndex);
            vpCreated = false;
        }

        if (partition != 0)
        {
            _ = WhvNative.WHvDeletePartition(partition);
        }

        if (self.IsAllocated)
        {
            self.Free();
        }
    }

    private int HandleIoPort(WhvEmulatorIoAccessInfo* access)
    {
        int size = access->AccessSize;
        IPortDevice? device = io.Find(access->Port);

        if (device is null)
        {
            if (options.StopOnUnhandledPort)
            {
                unclaimedPort = access->Port;
                return EFail;
            }

            if (access->Direction == 0)
            {
                // An absent device leaves the bus floating, which a PC reads back as all ones.
                access->Data = Masks.AllOnes(size);
            }

            return 0;
        }

        if (access->Direction == 0)
        {
            access->Data = Masks.Truncate(device.Read(access->Port, size), size);
        }
        else
        {
            device.Write(access->Port, size, Masks.Truncate(access->Data, size));
        }

        return 0;
    }

    private int HandleMemory(WhvEmulatorMemoryAccessInfo* access)
    {
        ulong gpa = access->GpaAddress;
        int size = access->AccessSize;
        bool writing = access->Direction != 0;

        // The emulator replays every operand of the faulting instruction through this callback,
        // including the ones that live in ordinary RAM, so RAM is checked first.
        MemoryRegion? region = memory.FindRegion(gpa);
        if (region is not null && region.Contains(gpa, (ulong)size))
        {
            GuestMemoryAccess needed = writing ? GuestMemoryAccess.Write : GuestMemoryAccess.Read;
            if ((region.Access & needed) == 0)
            {
                unclaimedMemory = gpa;
                unclaimedMemoryAccess = writing ? MemoryAccessKind.Write : MemoryAccessKind.Read;
                return EFail;
            }

            Span<byte> ram = memory.Slice(gpa, size);
            Span<byte> data = new(access->Data, size);

            if (writing)
            {
                data.CopyTo(ram);
            }
            else
            {
                ram.CopyTo(data);
            }

            return 0;
        }

        IMemoryMappedDevice? device = mmio.Find(gpa, (ulong)size);
        if (device is null)
        {
            if (options.StopOnUnmappedMemory)
            {
                unclaimedMemory = gpa;
                unclaimedMemoryAccess = writing ? MemoryAccessKind.Write : MemoryAccessKind.Read;
                return EFail;
            }

            if (!writing)
            {
                new Span<byte>(access->Data, size).Clear();
            }

            return 0;
        }

        ulong offset = gpa - device.BaseAddress;

        if (writing)
        {
            device.Write(offset, new ReadOnlySpan<byte>(access->Data, size));
        }
        else
        {
            device.Read(offset, new Span<byte>(access->Data, size));
        }

        return 0;
    }

    private int HandleGetRegisters(WhvRegisterName* names, uint count, WhvRegisterValue* values) =>
        WhvNative.WHvGetVirtualProcessorRegisters(partition, VpIndex, names, count, values);

    private int HandleSetRegisters(WhvRegisterName* names, uint count, WhvRegisterValue* values) =>
        WhvNative.WHvSetVirtualProcessorRegisters(partition, VpIndex, names, count, values);

    private int HandleTranslateGvaPage(
        ulong gva,
        WhvTranslateGvaFlags flags,
        WhvTranslateGvaResultCode* resultCode,
        ulong* gpa) =>
        WhvNative.WHvTranslateGva(partition, VpIndex, gva, flags, resultCode, gpa);

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int OnIoPort(void* context, WhvEmulatorIoAccessInfo* access)
    {
        try
        {
            return FromContext(context).HandleIoPort(access);
        }
        catch (Exception)
        {
            // Nothing may unwind through the native emulator; the failed status turns into an
            // EmulationFailed exit instead.
            return EFail;
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int OnMemory(void* context, WhvEmulatorMemoryAccessInfo* access)
    {
        try
        {
            return FromContext(context).HandleMemory(access);
        }
        catch (Exception)
        {
            return EFail;
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int OnGetRegisters(void* context, WhvRegisterName* names, uint count, WhvRegisterValue* values)
    {
        try
        {
            return FromContext(context).HandleGetRegisters(names, count, values);
        }
        catch (Exception)
        {
            return EFail;
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int OnSetRegisters(void* context, WhvRegisterName* names, uint count, WhvRegisterValue* values)
    {
        try
        {
            return FromContext(context).HandleSetRegisters(names, count, values);
        }
        catch (Exception)
        {
            return EFail;
        }
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int OnTranslateGvaPage(
        void* context,
        ulong gva,
        WhvTranslateGvaFlags flags,
        WhvTranslateGvaResultCode* resultCode,
        ulong* gpa)
    {
        try
        {
            return FromContext(context).HandleTranslateGvaPage(gva, flags, resultCode, gpa);
        }
        catch (Exception)
        {
            return EFail;
        }
    }

    private static PicoVm FromContext(void* context) =>
        (PicoVm)GCHandle.FromIntPtr((nint)context).Target!;
}
