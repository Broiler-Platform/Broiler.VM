# Writing the code a pico VM runs

A pico VM executes bytes. This page covers where those bytes come from, what each mode expects of
them, and the traps that catch people the first time.

## Getting bytes

Hand encoding is fine for a few instructions and is what the samples do, with the listing kept
next to the bytes. For anything longer, assemble to a flat binary and load the file:

```bash
nasm -f bin guest.asm -o guest.bin
dotnet run --project samples/Broiler.VM.HyperV.Cli -- run --file guest.bin --mode long64
```

`nasm -f bin` emits exactly the bytes of the section with no headers, which is what
`vm.LoadCode(File.ReadAllBytes(path))` wants. Start the source with `bits 64` (or `bits 16` /
`bits 32`) and `org` set to the address you load at.

There is deliberately no assembler in this component. Assembling is a solved problem with good
tools, and a half assembler would be a worse one.

## The rules every mode shares

**End with `hlt` (`0xF4`).** It is how a program says it is finished. Anything else — running off
the end of your code into zeroed RAM, jumping somewhere unmapped — ends the run as a fault.

**There are no interrupts.** `IF` is clear and no interrupt descriptor table is loaded, so a
divide by zero, an invalid opcode or a page fault becomes a triple fault, reported as
`UnrecoverableException`. There is no `int 0x10` and no BIOS.

**The stack works, and it is yours to overflow.** `RSP` starts at the top of RAM (or `0xFFF0` in
real mode). Nothing guards it; pushing past the bottom of RAM faults.

**`hlt` reports the address after itself.** `VmExit.Rip` for a halt is the byte following the
`0xF4`, which is what you want when you are checking that a program ran to its end.

**Registers other than the ones you set hold reset values.** `RDX` starts holding the processor
signature, exactly as on real hardware. Zero what you rely on.

## Real mode (`GuestMode.Real16`)

`CS` is based at the code address and `RIP` starts at 0, so the program sees itself at offset 0 —
write it like a `.COM` file. `DS`, `ES` and `SS` are based at 0, so **a data address in the
program is a guest physical address**. That asymmetry is deliberate: code is relocatable, data is
addressed the way the host addresses it.

```asm
bits 16
org 0
        mov si, 0x2000          ; a physical address the host wrote to
        mov dx, 0x00e9
next:   lodsb                   ; ds:si, and ds is based at 0
        test al, al
        jz done
        out dx, al
        jmp next
done:   hlt
```

```
be 00 20  ba e9 00  ac  84 c0  74 03  ee  eb f8  f4
```

The code address must be 16-byte aligned — it becomes the `CS` base, and the selector is that
address shifted right by four — and it must be below 1 MiB.

## Protected mode (`GuestMode.Protected32`)

Flat 32-bit segments, paging off, so a linear address is a guest physical address and there is
nothing to reserve:

```asm
bits 32
        mov eax, 7
        mov ecx, 6
        imul eax, ecx
        mov [0x3000], eax       ; straight to guest physical 0x3000
        hlt
```

```
b8 07 00 00 00  b9 06 00 00 00  0f af c1  a3 00 30 00 00  f4
```

## Long mode (`GuestMode.Long64`)

Flat segments and an identity map, so a virtual address is a guest physical address for the low
1 GiB. Above 1 GiB nothing is mapped and a touch is a page fault, not a host exit.

**The low 64 KiB belongs to the machine.** The identity page table sits at `0x1000`–`0x3FFF` by
default; writing data over it takes the guest down at its next memory access, which looks like an
unexplained `UnrecoverableException`. `PicoVm.ReservedRange` tells you the exact range, `LoadCode`
refuses to write across it, and the default code address of `0x10000` keeps you clear of it.
Move the tables with `PicoVmOptions.PageTableAddress` if you want the low pages for something else.

```asm
bits 64
        xor eax, eax
        mov ecx, 100
loop:   add rax, rcx
        dec ecx
        jnz loop
        hlt
```

```
31 c0  b9 64 00 00 00  48 01 c8  ff c9  75 f9  f4
```

Note `ff c9` for `dec ecx`: in 64-bit mode the one-byte `dec` forms are gone, they are REX
prefixes now.

## Talking to the host

**A port write is the cheapest channel there is.** One instruction, no driver, no setup:

```asm
        mov al, '!'
        out 0xe9, al            ; e6 e9
```

**A port read** returns whatever the device hands back, and all-ones when no device is mapped —
the same thing an empty ISA slot does:

```asm
        mov dx, 0x100           ; 66 ba 00 01
        in al, dx               ; ec
```

**Memory mapped devices** are reached with ordinary loads and stores, at an address inside the
identity map but outside RAM. With the 4 MiB default, anything from 4 MiB to 1 GiB works:

```asm
        mov rbx, 0x20000000     ; 48 c7 c3 00 00 00 20
        mov [rbx], rax          ; 48 89 03
        mov rcx, [rbx+8]        ; 48 8b 4b 08
```

The host sees the access at the width the instruction used, which is why the device interface
takes a span rather than a `ulong`.

## Sharing data with the host

Guest RAM is ordinary host memory, mapped. Writing it from the host while the guest runs is
allowed and coherent — both sides are looking at the same physical pages:

```csharp
vm.Memory.Write(0x8000, Encoding.ASCII.GetBytes("hello\0"));
vm.Memory.WriteUInt64(0x9000, 42);
VmExit exit = vm.Run();
ulong answer = vm.Memory.ReadUInt64(0x9000);
```

`vm.Memory.Slice(gpa, length)` hands out a `Span<byte>` over the guest's own memory for anything
the typed helpers do not cover.

## When something goes wrong

| What you see | Usual cause |
|---|---|
| `UnrecoverableException` immediately | The first instruction is not what you think. Check the mode matches the encoding — 16-bit bytes in a 64-bit guest decode into nonsense. |
| `UnrecoverableException` after a while | A fault with no IDT: divide by zero, an invalid opcode, a stack that ran off the bottom of RAM — or data written over the page table. |
| `MemoryFault` at a plausible-looking address | A jump or load went somewhere with no RAM and no device. `FaultAddress` and `FaultAccess` say where and how. |
| `Canceled` | The guest never halted. Usually a loop whose exit condition is wrong. |
| `Halted` but the answer is wrong | The program ran to the end. It is a program bug now, not a machine one — `vm.Cpu.Capture().Format()` prints the register file. |

`broiler-picovm run <sample> --dump` shows what a healthy run looks like for comparison.
