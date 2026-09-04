# Architecture

What happens between `new PicoVm()` and the guest's first instruction, and what happens each time
the guest stops.

## The layers

```
PicoVm                run loop, exit dispatch, emulator callbacks
  GuestMemory           VirtualAlloc + WHvMapGpaRange, host-side read/write helpers
  VirtualProcessor      register reads and writes
  IoBus / MmioBus       which device answers which port or address
  GuestBootstrap        the register state that puts the processor in a mode
Interop                 P/Invoke onto WinHvPlatform.dll and WinHvEmulation.dll
```

The interop layer is a transcription of `WinHvPlatformDefs.h` and `WinHvEmulation.h`. Structures
carry the offsets and sizes the headers assert (`WHV_RUN_VP_EXIT_CONTEXT` is 224 bytes on AMD64,
its union starts at offset 48), so the projections are explicit-layout rather than hopeful.

## Creating a machine

1. `WHvCreatePartition` — an empty partition handle.
2. `WHvSetPartitionProperty(ProcessorCount, 1)` — properties are only settable before setup.
3. `WHvSetupPartition` — the partition becomes real. Nothing can be configured after this.
4. `VirtualAlloc(MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE)` then `WHvMapGpaRange` — guest RAM.
   The pages must be committed private memory because the hypervisor pins them, which is why the
   managed heap cannot be used.
5. `WHvCreateVirtualProcessor` — one processor, index 0.
6. `GuestBootstrap.Apply` — the mode.

The order matters in both directions. On teardown the mappings are removed before the partition
is deleted, and the host pages are released last.

## Reaching a mode without firmware

A new virtual processor arrives in the x86 reset state: 16-bit real mode, `CS` based at
`0xFFFF0000`, `RIP` at `0xFFF0` — the address a physical machine finds its firmware at. A pico VM
has no firmware, so rather than executing a boot sequence it writes the end state directly.

The hypervisor exposes the *hidden* part of each segment register — base, limit and access rights
— so descriptors can be installed without a GDT existing anywhere in guest memory. The attribute
words are the familiar VMX access-rights encodings:

| Segment | Attributes | Meaning |
|---|---|---|
| Real mode code | `0x009B` | present, code, execute/read, 16-bit, byte granular |
| Real mode data | `0x0093` | present, data, read/write, 16-bit, byte granular |
| 32-bit code | `0xC09B` | as above, D/B set, 4 KiB granular |
| 64-bit code | `0xA09B` | as above, L set instead of D/B |
| Flat data | `0xC093` | present, data, read/write, 4 KiB granular |

Long mode additionally needs paging on before the first instruction, which means a page table has
to exist in guest RAM before the guest runs. `WriteIdentityPageTable` writes three pages — a PML4
whose first entry points at a PDPT, whose first entry points at a page directory of 512 large
pages — mapping the low 1 GiB of virtual addresses onto the same physical addresses. Addresses
above RAM stay mapped on purpose: that is what lets a memory mapped device sit at an address with
no RAM behind it and still be reachable by an ordinary `mov`.

`CR0`, `CR3`, `CR4` and `EFER` are then written in a **single** `WHvSetVirtualProcessorRegisters`
call. The hardware validates the combination of `CR0.PG`, `CR4.PAE` and `EFER.LME` when the guest
is entered, not as each register lands, but writing them together means the processor never holds
a half-applied mode even transiently.

### Register values must be 16-byte aligned

`WHV_REGISTER_VALUE` is declared `DECLSPEC_ALIGN(16)` — it holds 128-bit XMM values — and the API
reads arrays of it with aligned SSE moves. A managed `WhvRegisterValue[]` only guarantees 8-byte
alignment, which faults inside `WinHvPlatform.dll` about half the time. Batched register access
therefore allocates its buffer with `NativeMemory.AlignedAlloc(size, 16)`. This is not a
workaround; it is the contract the header states.

## The run loop

```
WHvRunVirtualProcessor
    |
    +-- X64Halt              -> return Halted
    +-- X64IoPortAccess      -> WHvEmulatorTryIoEmulation   -> IoBus   -> continue
    +-- MemoryAccess         -> WHvEmulatorTryMmioEmulation -> RAM or MmioBus -> continue
    +-- Canceled             -> return Canceled
    +-- UnrecoverableException, InvalidVpRegisterValue, ... -> return the matching reason
```

Device accesses are replayed by the Hyper-V instruction emulator rather than decoded here. The
emulator advances `RIP`, updates registers, and handles the string and repeated forms, which a
hand-rolled port handler would get wrong the first time a guest used `rep outsb`. It reaches back
into the host through five callbacks:

| Callback | What it does here |
|---|---|
| `IoPort` | Looks the port up in `IoBus`. |
| `Memory` | Resolves the address in guest RAM first — the emulator replays *every* operand of the faulting instruction, including the ones in ordinary RAM — then falls back to `MmioBus`. |
| `GetVirtualProcessorRegisters`, `SetVirtualProcessorRegisters` | Forward straight to the partition. |
| `TranslateGvaPage` | Forwards to `WHvTranslateGva`. |

The callbacks are `[UnmanagedCallersOnly]` static methods, so there is no delegate to keep alive
and the component stays AOT-compatible. The instance is recovered from the context pointer the
emulator passes back, a `GCHandle` deliberately allocated **weak**: a strong handle stored in the
object's own field would root the machine forever and leave a forgotten `Dispose` unrecoverable.
`GC.KeepAlive` around the native calls holds the machine alive for exactly as long as a callback
could arrive.

Nothing may unwind through native code, so every callback catches everything and returns `E_FAIL`.
The emulator turns that into a failed status, which the run loop turns into an exit — an
unclaimed port or an unbacked address if that is what happened, `EmulationFailed` otherwise.

### Failing without retiring

When a callback returns `E_FAIL` the emulator changes no guest state, so the guest is still
sitting on the instruction that faulted. That is what makes `UnhandledPort` and `MemoryFault`
resumable: attach the device that was missing and call `Run` again.

### Stopping a guest that will not stop

`WHvCancelRunVirtualProcessor` is safe to call from another thread, so cancellation is a plain
`CancellationToken` registration, and `Run(TimeSpan)` is that plus `CancelAfter`. A guest spinning
in `jmp $` produces no exits at all, so a wall-clock budget is the only thing that can end it.

## What is deliberately absent

- **More than one processor.** The partition is created with one, and the run loop is written for
  one. Multiple processors need a thread per processor and a story for inter-processor
  interrupts; neither belongs in a pico VM.
- **An APIC, a PIT, an IDT.** No interrupts means no interrupt controller, and an exception in the
  guest is a triple fault the host reports rather than something the guest could handle.
- **CPUID and MSR interception.** The partition takes the hypervisor's default answers. Both are
  opt-in partition properties that could be added without disturbing anything else here.
- **Snapshots and migration.** `WHvGetVirtualProcessorState` exists; nothing here needs it yet.
