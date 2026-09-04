# Broiler.VM.HyperV

Creates **pico virtual machines** on the Windows Hypervisor Platform and runs raw x86-64 machine
code in them.

A pico VM is one hardware partition, one virtual processor, a slab of RAM, and nothing else — no
firmware, no bootloader, no interrupt table, no operating system. It starts with its instruction
pointer aimed at bytes you loaded, in the processor mode you asked for, and it runs until it
executes `hlt`, touches something you did not answer, or you cancel it.

```csharp
using PicoVm vm = new();                                    // 4 MiB, 64-bit long mode
vm.LoadCode([0xB8, 0x2A, 0x00, 0x00, 0x00, 0xF4]);          // mov eax, 42 ; hlt
VmExit exit = vm.Run();

Console.WriteLine(exit.Reason);   // Halted
Console.WriteLine(vm.Cpu.Rax);    // 42
```

That is real code on the real processor, isolated by the same hardware virtualization Hyper-V
uses — not an interpreter, and not a simulator.

## Independence

This component shares a name prefix with `Broiler.VM` and none of its code. `Broiler.VM` executes
bytecode in software; this one hands machine code to the silicon. It references **no** other
Broiler project and **no** NuGet package: the library binds `WinHvPlatform.dll` and
`WinHvEmulation.dll` directly and depends on nothing else. Its `Directory.Build.props`
deliberately does not chain to a parent, so a checkout evaluates the same wherever it sits.

## Requirements

| | |
|---|---|
| OS | 64-bit Windows 10 1803 or later, or Windows 11 |
| Feature | **Windows Hypervisor Platform** (separate from Hyper-V itself) |
| Runtime | .NET 10, x64 process |
| Privileges | None — partitions are created as a normal user |
| IDE | Visual Studio 2026, or any editor plus `dotnet build`. VS 2022 and earlier resolve the .NET 9 SDK and cannot target `net10.0`, so every project in the solution fails to load there — the same is true of the other Broiler components. |

Enable the feature from an elevated PowerShell prompt and reboot:

```powershell
Enable-WindowsOptionalFeature -Online -FeatureName HypervisorPlatform -All
```

`HyperVPlatform.IsAvailable` answers whether the machine can host a guest, and
`HyperVPlatform.UnavailableReason` says why not, with the command above in the message. Nothing
throws just because you asked.

## Guest modes

`PicoVmOptions.Mode` picks how the processor is set up before the first instruction. Each mode is
reached by writing the architectural registers directly, so there is no boot sequence to execute
and the guest's first instruction is the first instruction of your program.

| Mode | What the guest sees |
|---|---|
| `Real16` | 16-bit real mode. `CS` is based at the code address, so the program sees itself at offset 0 like a `.COM` image. `DS`, `ES` and `SS` are based at 0, so a data address is a guest physical address. |
| `Protected32` | 32-bit protected mode, flat segments, paging off. A linear address is a guest physical address. |
| `Long64` | 64-bit long mode, flat segments, and an identity map built in guest RAM with 2 MiB pages covering the low 1 GiB. A virtual address is a guest physical address. |

Default memory map in long mode:

```
0x00000000  +--------------------------------+
            |  (unused)                      |
0x00001000  |  PML4, PDPT, PD  (reserved)    |  <- PicoVm.ReservedRange
0x00004000  |  (unused)                      |
0x00010000  |  your code, then your data     |  <- PicoVm.CodeAddress
            |                                |
0x00400000  +--------------------------------+  <- initial RSP, top of 4 MiB
```

The low 64 KiB belongs to the machine; put guest data at `0x10000` and above. `LoadCode` refuses
to write over the reserved page table, and `ReservedRange` tells you where it is.

## Devices

A guest reaches the host in exactly two ways, and both are serviced by the Hyper-V instruction
emulator, so string and repeated forms work like they do on hardware.

**I/O ports** — `in` and `out`:

```csharp
vm.Io.MapWriter(0xE9, b => Console.Write((char)b));            // a byte sink
vm.Io.Map(0x100, new DelegatePortDevice(read: (p, w) => 0x5A)); // a source
vm.Io.Map(0xE9, new ConsolePortDevice());                       // bytes straight to stdout
```

**Memory mapped devices** — ordinary loads and stores at addresses with no RAM behind them:

```csharp
vm.Mmio.Map(new DelegateMemoryMappedDevice(
    baseAddress: 0x2000_0000,
    length: 0x1000,
    read: (offset, destination) => destination.Fill(0xAB),
    write: (offset, source) => Log(source)));
```

Unclaimed ports read back all-ones and swallow writes, the way a PC behaves for an absent device.
Unbacked memory stops the run instead, because a guest reading unmapped memory is almost always a
bug rather than a device. Both defaults are switchable: `StopOnUnhandledPort` and
`StopOnUnmappedMemory`.

## How a run ends

`Run` returns a `VmExit` rather than throwing, because "the guest crashed" is a result, not an
error:

| `VmExitReason` | Meaning |
|---|---|
| `Halted` | The guest executed `hlt`. `Rip` is the address **after** it. |
| `MemoryFault` | It touched an address with neither RAM nor a device. `FaultAddress` and `FaultAccess` say where and how. |
| `UnhandledPort` | It touched an unclaimed port and `StopOnUnhandledPort` is on. |
| `UnrecoverableException` | It triple faulted. With no IDT loaded, this is what a bad instruction or wild jump looks like. |
| `Canceled` | The host stopped it, through a token or a `Run(TimeSpan)` budget. |
| `ExitLimitReached` | It serviced `ExitLimit` exits without halting. |
| `EmulationFailed`, `InvalidRegisterValue`, `UnsupportedFeature`, `Unexpected` | Rarer, all with a message that says what happened. |

Nothing is retired when a run stops on `UnhandledPort` or `MemoryFault`, so you can attach the
missing device and call `Run` again — the guest resumes at the same instruction:

```csharp
VmExit exit = vm.Run();                       // UnhandledPort, port 0x99
vm.Io.Map(0x99, new DelegatePortDevice(read: (_, _) => 0x42));
exit = vm.Run();                              // Halted
```

Guests are not trusted to terminate. `Run(TimeSpan)` cancels from another thread, so an endless
loop costs you the budget and nothing more:

```csharp
vm.LoadCode([0xEB, 0xFE]);                    // jmp $
VmExit exit = vm.Run(TimeSpan.FromMilliseconds(250));  // Canceled
```

## Platform limits worth knowing

- **One partition per process may own guest memory.** Windows names a partition's memory block
  after the owning process, so the second live `PicoVm` in a process fails to map RAM with
  `0xC0370008`. Dispose the first, or host the second machine in its own process.
  `HyperVException` says exactly this rather than showing you the raw code.
- **One virtual processor per machine.** Pico VMs are single processor by design.
- **No interrupts.** There is no IDT and `IF` is clear, so an exception in the guest is a triple
  fault, reported as `UnrecoverableException`.
- **Dispose your machines.** A partition holds pinned host pages until it is deleted.

## The CLI

`samples/Broiler.VM.HyperV.Cli` builds `broiler-picovm`, which is both a demonstration and a way
to run a file of machine code:

```bash
dotnet run --project samples/Broiler.VM.HyperV.Cli -- info
dotnet run --project samples/Broiler.VM.HyperV.Cli -- list
dotnet run --project samples/Broiler.VM.HyperV.Cli -- run sum64 --dump
dotnet run --project samples/Broiler.VM.HyperV.Cli -- selftest
dotnet run --project samples/Broiler.VM.HyperV.Cli -- run --file prog.bin --mode long64 --memory 8M
```

`selftest` runs every sample and checks its result:

```
pass  add16     Real16      ax = 0x2345
pass  hello16   Real16      the guest wrote 21 byte(s) to port 0xE9
pass  sum64     Long64      rax = 5050
pass  mul32     Protected32 [0x3000] = 42
pass  mmio64    Long64      the device stored 0xC0FFEE and the guest read back rcx=0x5AFED00D5AFED00D
pass  fault64   Long64      MemoryFault while trying to read 0x10000000
```

Each sample carries the assembly listing its bytes came from; `run <sample>` prints it before
executing.

## Building and testing

```bash
dotnet build Broiler.VM.HyperV.slnx
dotnet test tests/Broiler.VM.HyperV.Tests/Broiler.VM.HyperV.Tests.csproj
```

The suite runs real machine code on the real hypervisor. On a machine without the Windows
Hypervisor Platform the hardware tests skip themselves and the configuration tests still run, so
the suite is green either way rather than red for the wrong reason.

## Further reading

- [docs/architecture.md](docs/architecture.md) — what happens between `new PicoVm()` and the
  guest's first instruction, and how an exit is serviced.
- [docs/machine-code.md](docs/machine-code.md) — writing the code a pico VM runs, with the
  encodings the samples use and the traps each mode sets.

## License

Apache-2.0. See [LICENSE](LICENSE).
