// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   53
// Annotated:        53/53
// Exempt:           26
// Human-reviewed:   0/53
// IP risk:          Low
// Security risk:    High
// Criteria:         13/13
// Resource impact:  2/10 max
// Unverified:       53
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The sixteen general-purpose registers, by their architectural numbers.</summary>
/// <remarks>
/// <b>The numbers ARE the encoding and that is why they are written here rather than mapped.</b>
/// A register's number is its three low bits in a ModRM or SIB field and its fourth bit in a REX
/// prefix, so a table that renumbered them would be a table every encoding had to be translated
/// through - and a translation is a place a wrong number can hide. These are the architecture's own
/// numbers and nothing converts them.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C1ADDE
// Broiler-Human:        PENDING
public enum JsX64Register
{
    /// <summary>RAX, and the register a return code is answered in under both conventions.</summary>
    Rax = 0,

    /// <summary>RCX, which carries the first integer argument under the Windows x64 convention.</summary>
    Rcx = 1,

    /// <summary>RDX.</summary>
    Rdx = 2,

    /// <summary>RBX, callee-saved under both conventions.</summary>
    Rbx = 3,

    /// <summary>RSP, the stack pointer.</summary>
    Rsp = 4,

    /// <summary>RBP, callee-saved under both conventions.</summary>
    Rbp = 5,

    /// <summary>RSI, callee-saved under Windows x64 and VOLATILE under System V.</summary>
    Rsi = 6,

    /// <summary>
    /// RDI, which carries the first integer argument under System V and is callee-saved under
    /// Windows x64.
    /// </summary>
    Rdi = 7,

    /// <summary>R8.</summary>
    R8 = 8,

    /// <summary>R9.</summary>
    R9 = 9,

    /// <summary>R10, volatile under both conventions.</summary>
    R10 = 10,

    /// <summary>R11, volatile under both conventions.</summary>
    R11 = 11,

    /// <summary>R12, callee-saved under both conventions.</summary>
    R12 = 12,

    /// <summary>R13, callee-saved under both conventions.</summary>
    R13 = 13,

    /// <summary>R14, callee-saved under both conventions.</summary>
    R14 = 14,

    /// <summary>R15, callee-saved under both conventions.</summary>
    R15 = 15,
}

/// <summary>The condition codes, as the low nibble every conditional opcode carries.</summary>
/// <remarks>
/// <b>ONE NIBBLE SERVES THE JUMPS AND THE SETS, which is why this is an enum and not two.</b> A
/// conditional jump is <c>0F 8x</c> and a conditional set is <c>0F 9x</c> over the same
/// <c>x</c>, so a code named once cannot be spelled two different ways in the two places it is
/// used.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8CB5FC
// Broiler-Human:        PENDING
public enum JsX64Condition
{
    /// <summary>Equal: ZF set. After <c>ucomisd</c> an unordered comparison also sets ZF.</summary>
    Equal = 0x4,

    /// <summary>Not equal: ZF clear.</summary>
    NotEqual = 0x5,

    /// <summary>Above: CF clear and ZF clear.</summary>
    Above = 0x7,

    /// <summary>Above or equal: CF clear.</summary>
    AboveOrEqual = 0x3,

    /// <summary>Parity: PF set, which after <c>ucomisd</c> means at least one operand was NaN.</summary>
    Parity = 0xA,

    /// <summary>No parity: PF clear, which after <c>ucomisd</c> means neither operand was NaN.</summary>
    NoParity = 0xB,

    /// <summary>Signed less than: SF differs from OF.</summary>
    Less = 0xC,
}

/// <summary>
/// An encoder of x86-64 instructions over a growable byte buffer, with labels and rel32 branches.
/// </summary>
/// <remarks>
/// <para>
/// <b>EVERY BRANCH IS A REL32 AND THERE IS NO RELAXATION PASS, AND THAT IS THE MOST IMPORTANT
/// SENTENCE IN THIS FILE.</b> The short forms exist, they are two bytes shorter, and this encoder
/// will not write one. A one-byte displacement is correct until a later edit pushes its target past
/// a hundred and twenty-seven bytes, at which point the patch silently truncates and the branch
/// lands in the middle of an instruction - a miscompile a verifier cannot see, a test cannot
/// reliably provoke and a reader cannot spot. A relaxation pass that shortened branches and then
/// re-measured would be the same defect with a schedule attached: every pass that moves code
/// invalidates the displacements the previous pass computed, and the defect is in the interaction
/// and not in either pass. So: one form, one width, one pass, no cleverness.
/// </para>
/// <para>
/// <b>Nothing here decides anything.</b> This type selects no instruction, allocates no register
/// and knows no calling convention; it writes exactly the encoding it is asked for. That is what
/// makes it checkable against a published encoding table byte for byte, and it is why the parts
/// that DO decide - the convention table, the lowering of one bytecode instruction to a sequence -
/// live in the backend beside it rather than in here.
/// </para>
/// <para>
/// <b>A label is bound at most once and every use of it is patched when it is.</b> A label that
/// nothing ever bound is a defect in the backend rather than in the program being compiled, so it
/// is reported by <see cref="TryFinish"/> as a refusal, and nothing hands out bytes that carry an
/// unpatched displacement.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=9E1CB2
// Broiler-Falsified-If: a branch this encoder writes carries fewer than four bytes of displacement, or a byte sequence it emits differs from the architecture manual's encoding for the instruction it names
// Broiler-Human:        PENDING
public sealed class JsX64Assembler
{
    /// <summary>The bytes emitted so far.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FC887B
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<byte> bytes = [];

    /// <summary>Where each label is bound, or minus one while it is not.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=69D485
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<int> bound = [];

    /// <summary>The displacement sites waiting for each label, by label.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C6BD02
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<System.Collections.Generic.List<int>> pending = [];

    /// <summary>How many bytes have been emitted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=94D198
    // Broiler-Human:        PENDING
    public int Position => bytes.Count;

    /// <summary>Reserves a label, unbound.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5B6D6D
    // Broiler-Human:        PENDING
    public int Label()
    {
        bound.Add(-1);
        pending.Add([]);
        return bound.Count - 1;
    }

    /// <summary>Binds <paramref name="label"/> to the current position and patches its uses.</summary>
    /// <remarks>
    /// <b>The patch happens here rather than at the end, and the difference matters only for the
    /// error it makes impossible.</b> A pass that patched everything at the end would have to hold
    /// every site until then, and a site whose label was bound twice would be patched twice with
    /// the second answer winning silently. Binding patches and clears, so a second bind of one
    /// label is a defect this method can see.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CE5E20
    // Broiler-Falsified-If: a patched displacement is not the distance from the end of the branch instruction to the bind point
    // Broiler-Human:        PENDING
    public bool TryBind(int label, out string refusal)
    {
        if (bound[label] >= 0)
        {
            refusal = "the emitter bound one label twice, which is a defect in the emitter";
            return false;
        }

        var at = bytes.Count;
        bound[label] = at;

        foreach (var site in pending[label])
        {
            // A REL32 IS MEASURED FROM THE END OF THE BRANCH INSTRUCTION, which is four bytes past
            // the site the displacement occupies. Measuring from the start is the classic
            // off-by-the-instruction-length error, and it produces code that runs and lands in the
            // wrong place rather than code that faults.
            Patch(site, at - (site + 4));
        }

        pending[label].Clear();
        refusal = string.Empty;
        return true;
    }

    /// <summary>The emitted bytes, or a refusal because some label was never bound.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=DFEB4F
    // Broiler-Falsified-If: bytes are handed out while any branch site still holds an unpatched displacement
    // Broiler-Human:        PENDING
    public bool TryFinish(out byte[] code, out string refusal)
    {
        for (var index = 0; index < bound.Count; index++)
        {
            if (bound[index] < 0 && pending[index].Count != 0)
            {
                code = [];
                refusal =
                    "the emitter branched to a label it never bound, which is a defect in the " +
                    "emitter rather than in the program being compiled";

                return false;
            }
        }

        code = bytes.ToArray();
        refusal = string.Empty;
        return true;
    }

    /// <summary>Writes one byte.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AEC670
    // Broiler-Human:        PENDING
    public void Emit(byte value) => bytes.Add(value);

    /// <summary>Writes four bytes, little-endian.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=49B030
    // Broiler-Human:        PENDING
    public void EmitInt32(int value)
    {
        bytes.Add((byte)value);
        bytes.Add((byte)(value >> 8));
        bytes.Add((byte)(value >> 16));
        bytes.Add((byte)(value >> 24));
    }

    /// <summary>Writes eight bytes, little-endian.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D6BF35
    // Broiler-Human:        PENDING
    public void EmitInt64(long value)
    {
        for (var shift = 0; shift < 64; shift += 8)
        {
            bytes.Add((byte)(value >> shift));
        }
    }

    /// <summary>Pads to a multiple of <paramref name="alignment"/> with one-byte no-ops.</summary>
    /// <remarks>
    /// <b>It pads with <c>0x90</c> and not with zero.</b> A run of zero bytes decodes as
    /// <c>add [rax], al</c>, which faults on a null base and stores through any other; a reader who
    /// lands in the padding by mistake should meet an instruction that does nothing, because
    /// padding that a mistake turns into a store is padding that hides the mistake.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EFF408
    // Broiler-Human:        PENDING
    public void AlignTo(int alignment)
    {
        while ((bytes.Count % alignment) != 0)
        {
            bytes.Add(0x90);
        }
    }

    // ---- prefixes and operand encoding ---------------------------------------------------------

    /// <summary>Writes a REX prefix, or nothing when none of its bits are needed.</summary>
    /// <remarks>
    /// <b>A bare <c>0x40</c> REX prefix is legal and this encoder does not write one</b>, because
    /// an unnecessary prefix is a byte two encoders of the same instruction can disagree about and
    /// re-emission equality compares bytes. The prefix is written when the operand size is
    /// sixty-four bits or when any of the three register fields reaches the upper eight.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DDB01E
    // Broiler-Human:        PENDING
    private void Rex(bool wide, int reg, int index, int baseRegister)
    {
        var prefix = 0x40 |
            (wide ? 8 : 0) |
            (((reg >> 3) & 1) << 2) |
            (((index >> 3) & 1) << 1) |
            ((baseRegister >> 3) & 1);

        if (prefix != 0x40)
        {
            bytes.Add((byte)prefix);
        }
    }

    /// <summary>Writes a ModRM byte for two registers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6D9363
    // Broiler-Human:        PENDING
    private void ModRmRegister(int reg, int rm) =>
        bytes.Add((byte)(0xC0 | ((reg & 7) << 3) | (rm & 7)));

    /// <summary>
    /// Writes a ModRM byte, a SIB byte where the base demands one, and a 32-bit displacement.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE DISPLACEMENT IS ALWAYS THIRTY-TWO BITS, for the same reason every branch is.</b> An
    /// eight-bit displacement is three bytes shorter and correct until a frame grows past a hundred
    /// and twenty-seven bytes, and the failure is a load from the wrong slot rather than a fault.
    /// One width, always, and the cost is bytes nobody counts.
    /// </para>
    /// <para>
    /// <b>RSP and R12 cannot be a base without a SIB byte, and that is an encoding fact rather
    /// than a choice.</b> The value <c>100</c> in the r/m field means "a SIB byte follows"; a
    /// register whose low three bits are <c>100</c> therefore cannot name itself there, and the
    /// SIB byte <c>0x24</c> is the one that says "base RSP or R12, no index".
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=51DD3D
    // Broiler-Falsified-If: a memory operand based on RSP or R12 is encoded without the SIB byte the architecture requires
    // Broiler-Human:        PENDING
    private void ModRmMemory(int reg, int baseRegister, int displacement)
    {
        bytes.Add((byte)(0x80 | ((reg & 7) << 3) | (baseRegister & 7)));

        if ((baseRegister & 7) == 4)
        {
            bytes.Add(0x24);
        }

        EmitInt32(displacement);
    }

    /// <summary>Overwrites four bytes at <paramref name="site"/> with <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A7CB5C
    // Broiler-Human:        PENDING
    private void Patch(int site, int value)
    {
        bytes[site] = (byte)value;
        bytes[site + 1] = (byte)(value >> 8);
        bytes[site + 2] = (byte)(value >> 16);
        bytes[site + 3] = (byte)(value >> 24);
    }

    // ---- integer instructions ------------------------------------------------------------------

    /// <summary><c>push r64</c>: <c>50+r</c>, with a REX prefix for the upper eight.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8FFFB0
    // Broiler-Human:        PENDING
    public void Push(JsX64Register register)
    {
        Rex(false, 0, 0, (int)register);
        bytes.Add((byte)(0x50 + ((int)register & 7)));
    }

    /// <summary><c>pop r64</c>: <c>58+r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C52A12
    // Broiler-Human:        PENDING
    public void Pop(JsX64Register register)
    {
        Rex(false, 0, 0, (int)register);
        bytes.Add((byte)(0x58 + ((int)register & 7)));
    }

    /// <summary><c>ret</c>: <c>C3</c>, and there is deliberately no immediate form.</summary>
    /// <remarks>
    /// <b>NEITHER x86-64 CONVENTION HAS A CALLEE-POPS FORM, WHICH IS WHY THE <c>ret 8</c> DEFECT
    /// CLASS THE CORE ROADMAP RETAINS AS A FIXTURE CANNOT ARISE IN THIS BACKEND.</b> That anecdote
    /// is about a callee-pops convention, where the callee removes the caller's arguments and a
    /// wrong count moves the stack pointer a few bytes per call until a process that had been
    /// running for hours dies somewhere unrelated to the defect. Both x86-64 conventions are
    /// caller-pops: the caller adjusts its own stack and the callee returns with a bare
    /// <c>C3</c>, so there is no count for a callee to get wrong. The fixture is honoured anyway,
    /// by the test beside this backend that asserts the stack pointer is unchanged across every
    /// emitted entry point - because "the convention makes it impossible" is a claim, and a claim
    /// with no test is exactly how the original one was made.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=3376B8
    // Broiler-Falsified-If: an emitted return removes any argument bytes from the stack
    // Broiler-Human:        PENDING
    public void Ret() => bytes.Add(0xC3);

    /// <summary><c>mov r64, r64</c>: <c>REX.W 89 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8FC9AE
    // Broiler-Human:        PENDING
    public void MovRegisterRegister(JsX64Register destination, JsX64Register source)
    {
        Rex(true, (int)source, 0, (int)destination);
        bytes.Add(0x89);
        ModRmRegister((int)source, (int)destination);
    }

    /// <summary><c>mov r64, imm64</c>: <c>REX.W B8+r</c> and eight bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B3D046
    // Broiler-Human:        PENDING
    public void MovRegisterImmediate64(JsX64Register destination, long value)
    {
        Rex(true, 0, 0, (int)destination);
        bytes.Add((byte)(0xB8 + ((int)destination & 7)));
        EmitInt64(value);
    }

    /// <summary><c>mov r64, [base+disp32]</c>: <c>REX.W 8B /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8087EC
    // Broiler-Human:        PENDING
    public void MovRegisterMemory(
        JsX64Register destination, JsX64Register baseRegister, int displacement)
    {
        Rex(true, (int)destination, 0, (int)baseRegister);
        bytes.Add(0x8B);
        ModRmMemory((int)destination, (int)baseRegister, displacement);
    }

    /// <summary><c>mov [base+disp32], r64</c>: <c>REX.W 89 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EC8E9C
    // Broiler-Human:        PENDING
    public void MovMemoryRegister(
        JsX64Register baseRegister, int displacement, JsX64Register source)
    {
        Rex(true, (int)source, 0, (int)baseRegister);
        bytes.Add(0x89);
        ModRmMemory((int)source, (int)baseRegister, displacement);
    }

    /// <summary><c>lea r64, [base+disp32]</c>: <c>REX.W 8D /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=22B1EB
    // Broiler-Human:        PENDING
    public void LeaRegisterMemory(
        JsX64Register destination, JsX64Register baseRegister, int displacement)
    {
        Rex(true, (int)destination, 0, (int)baseRegister);
        bytes.Add(0x8D);
        ModRmMemory((int)destination, (int)baseRegister, displacement);
    }

    /// <summary><c>add r64, imm32</c>: <c>REX.W 81 /0 id</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=847C44
    // Broiler-Human:        PENDING
    public void AddRegisterImmediate32(JsX64Register destination, int value)
    {
        Rex(true, 0, 0, (int)destination);
        bytes.Add(0x81);
        ModRmRegister(0, (int)destination);
        EmitInt32(value);
    }

    /// <summary><c>sub r64, imm32</c>: <c>REX.W 81 /5 id</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=762E3A
    // Broiler-Human:        PENDING
    public void SubRegisterImmediate32(JsX64Register destination, int value)
    {
        Rex(true, 0, 0, (int)destination);
        bytes.Add(0x81);
        ModRmRegister(5, (int)destination);
        EmitInt32(value);
    }

    /// <summary><c>xor r64, r64</c>: <c>REX.W 31 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=959858
    // Broiler-Human:        PENDING
    public void XorRegisterRegister(JsX64Register destination, JsX64Register source)
    {
        Rex(true, (int)source, 0, (int)destination);
        bytes.Add(0x31);
        ModRmRegister((int)source, (int)destination);
    }

    /// <summary><c>add dword [base+disp32], imm32</c>: <c>81 /0 id</c>, thirty-two bits wide.</summary>
    /// <remarks>
    /// <b>It is a thirty-two-bit operation on purpose, because the field it edits is an
    /// <c>int</c>.</b> The frame's remaining-operand-slot count is four bytes; a sixty-four-bit add
    /// would write over the pointer that follows it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7539CE
    // Broiler-Falsified-If: this instruction writes more than four bytes of the frame
    // Broiler-Human:        PENDING
    public void AddMemory32Immediate32(JsX64Register baseRegister, int displacement, int value)
    {
        Rex(false, 0, 0, (int)baseRegister);
        bytes.Add(0x81);
        ModRmMemory(0, (int)baseRegister, displacement);
        EmitInt32(value);
    }

    /// <summary><c>sub dword [base+disp32], imm32</c>: <c>81 /5 id</c>, thirty-two bits wide.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=F4ECE1
    // Broiler-Falsified-If: this instruction writes more than four bytes of the frame
    // Broiler-Human:        PENDING
    public void SubMemory32Immediate32(JsX64Register baseRegister, int displacement, int value)
    {
        Rex(false, 0, 0, (int)baseRegister);
        bytes.Add(0x81);
        ModRmMemory(5, (int)baseRegister, displacement);
        EmitInt32(value);
    }

    /// <summary><c>dec qword [base+disp32]</c>: <c>REX.W FF /1</c>.</summary>
    /// <remarks>
    /// <b>The one-byte <c>dec</c> forms do not exist in 64-bit mode - their opcodes were taken for
    /// the REX prefixes</b> - so this is the group-five form and there is no shorter one to be
    /// tempted by.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=249209
    // Broiler-Human:        PENDING
    public void DecrementMemory64(JsX64Register baseRegister, int displacement)
    {
        Rex(true, 0, 0, (int)baseRegister);
        bytes.Add(0xFF);
        ModRmMemory(1, (int)baseRegister, displacement);
    }

    /// <summary><c>cmp r64, r64</c>: <c>REX.W 39 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CBC6C3
    // Broiler-Human:        PENDING
    public void CmpRegisterRegister(JsX64Register left, JsX64Register right)
    {
        Rex(true, (int)right, 0, (int)left);
        bytes.Add(0x39);
        ModRmRegister((int)right, (int)left);
    }

    /// <summary><c>test r32, r32</c>: <c>85 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8437BB
    // Broiler-Human:        PENDING
    public void TestRegister32(JsX64Register left, JsX64Register right)
    {
        Rex(false, (int)right, 0, (int)left);
        bytes.Add(0x85);
        ModRmRegister((int)right, (int)left);
    }

    /// <summary><c>setcc r8</c>: <c>0F 90+cc /0</c>, over the low byte of a register.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B8EB13
    // Broiler-Human:        PENDING
    public void SetCondition(JsX64Condition condition, JsX64Register destination)
    {
        Rex(false, 0, 0, (int)destination);
        bytes.Add(0x0F);
        bytes.Add((byte)(0x90 + (int)condition));
        ModRmRegister(0, (int)destination);
    }

    /// <summary><c>and r8, r8</c>: <c>20 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=964D22
    // Broiler-Human:        PENDING
    public void And8(JsX64Register destination, JsX64Register source)
    {
        Rex(false, (int)source, 0, (int)destination);
        bytes.Add(0x20);
        ModRmRegister((int)source, (int)destination);
    }

    /// <summary><c>or r8, r8</c>: <c>08 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=994CEE
    // Broiler-Human:        PENDING
    public void Or8(JsX64Register destination, JsX64Register source)
    {
        Rex(false, (int)source, 0, (int)destination);
        bytes.Add(0x08);
        ModRmRegister((int)source, (int)destination);
    }

    /// <summary><c>movzx r32, r8</c>: <c>0F B6 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2B95DB
    // Broiler-Human:        PENDING
    public void MovZeroExtend8To32(JsX64Register destination, JsX64Register source)
    {
        Rex(false, (int)destination, 0, (int)source);
        bytes.Add(0x0F);
        bytes.Add(0xB6);
        ModRmRegister((int)destination, (int)source);
    }

    // ---- branches, every one of them a rel32 ---------------------------------------------------

    /// <summary><c>jmp rel32</c>: <c>E9 id</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=E457CA
    // Broiler-Falsified-If: this emits the two-byte EB form
    // Broiler-Human:        PENDING
    public void Jump(int label)
    {
        bytes.Add(0xE9);
        Displacement(label);
    }

    /// <summary><c>jcc rel32</c>: <c>0F 80+cc id</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=212DC1
    // Broiler-Falsified-If: this emits the two-byte 7x form
    // Broiler-Human:        PENDING
    public void JumpIf(JsX64Condition condition, int label)
    {
        bytes.Add(0x0F);
        bytes.Add((byte)(0x80 + (int)condition));
        Displacement(label);
    }

    /// <summary><c>call rel32</c>: <c>E8 id</c>.</summary>
    /// <remarks>
    /// <b>A DIRECT CALL AND NEVER AN INDIRECT ONE.</b> Every call this backend emits is to a code
    /// unit of the same artifact, at a displacement fixed when the artifact was compiled; there is
    /// no table of pointers to be relocated at load, no register holding a target, and no path by
    /// which a value a guest program computed becomes an address. That is what makes the emitted
    /// blob position-independent, byte-identical on re-emission, and unable to transfer control
    /// anywhere the emitter did not write.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=767753
    // Broiler-Falsified-If: a call whose target is not a code unit of the same artifact is emitted
    // Broiler-Human:        PENDING
    public void Call(int label)
    {
        bytes.Add(0xE8);
        Displacement(label);
    }

    /// <summary><c>call r64</c>: <c>FF /2</c>, an indirect call through a register.</summary>
    /// <remarks>
    /// <b>THE BACKEND EMITS NONE OF THESE AND THIS EXISTS FOR THE TRAMPOLINE THAT CHECKS IT.</b>
    /// Every call in an emitted artifact is a direct <c>call rel32</c> to a code unit of the same
    /// artifact, which is what keeps the blob position-independent and keeps a value a guest
    /// computed from ever becoming an address. A trampoline is a different thing: it calls an
    /// address it was handed, precisely so that it can stand between a caller and an emitted entry
    /// point and watch what the calling convention actually did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=4A18FF
    // Broiler-Falsified-If: the backend emits one of these
    // Broiler-Human:        PENDING
    public void CallRegister(JsX64Register target)
    {
        Rex(false, 0, 0, (int)target);
        bytes.Add(0xFF);
        ModRmRegister(2, (int)target);
    }

    /// <summary>Writes a four-byte displacement, patched now or recorded for the bind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E3C9A4
    // Broiler-Human:        PENDING
    private void Displacement(int label)
    {
        var site = bytes.Count;
        EmitInt32(0);

        if (bound[label] >= 0)
        {
            Patch(site, bound[label] - (site + 4));
            return;
        }

        pending[label].Add(site);
    }

    // ---- SSE2 doubles --------------------------------------------------------------------------

    /// <summary><c>movsd xmm, [base+disp32]</c>: <c>F2 REX 0F 10 /r</c>.</summary>
    /// <remarks>
    /// <b>The <c>F2</c> is a mandatory prefix and it comes BEFORE the REX prefix.</b> A REX written
    /// ahead of it would be a prefix of the wrong instruction, and the processor would decode the
    /// result as something else entirely rather than refusing it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=07711B
    // Broiler-Falsified-If: a REX prefix is written ahead of a mandatory SSE prefix
    // Broiler-Human:        PENDING
    public void MovsdLoad(int destination, JsX64Register baseRegister, int displacement)
    {
        bytes.Add(0xF2);
        Rex(false, destination, 0, (int)baseRegister);
        bytes.Add(0x0F);
        bytes.Add(0x10);
        ModRmMemory(destination, (int)baseRegister, displacement);
    }

    /// <summary><c>movsd [base+disp32], xmm</c>: <c>F2 REX 0F 11 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=04C121
    // Broiler-Human:        PENDING
    public void MovsdStore(JsX64Register baseRegister, int displacement, int source)
    {
        bytes.Add(0xF2);
        Rex(false, source, 0, (int)baseRegister);
        bytes.Add(0x0F);
        bytes.Add(0x11);
        ModRmMemory(source, (int)baseRegister, displacement);
    }

    /// <summary><c>movsd xmm, xmm</c>: <c>F2 0F 10 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CD3E15
    // Broiler-Human:        PENDING
    public void MovsdRegister(int destination, int source)
    {
        bytes.Add(0xF2);
        Rex(false, destination, 0, source);
        bytes.Add(0x0F);
        bytes.Add(0x10);
        ModRmRegister(destination, source);
    }

    /// <summary><c>addsd xmm, xmm</c>: <c>F2 0F 58 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=153D77
    // Broiler-Human:        PENDING
    public void Addsd(int destination, int source) => Sse2Double(0x58, destination, source);

    /// <summary><c>subsd xmm, xmm</c>: <c>F2 0F 5C /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=941616
    // Broiler-Human:        PENDING
    public void Subsd(int destination, int source) => Sse2Double(0x5C, destination, source);

    /// <summary><c>mulsd xmm, xmm</c>: <c>F2 0F 59 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5D7EA2
    // Broiler-Human:        PENDING
    public void Mulsd(int destination, int source) => Sse2Double(0x59, destination, source);

    /// <summary><c>divsd xmm, xmm</c>: <c>F2 0F 5E /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=02CE1A
    // Broiler-Human:        PENDING
    public void Divsd(int destination, int source) => Sse2Double(0x5E, destination, source);

    /// <summary>The shared shape of the two-operand <c>F2 0F xx</c> scalar-double instructions.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=021E80
    // Broiler-Human:        PENDING
    private void Sse2Double(byte opcode, int destination, int source)
    {
        bytes.Add(0xF2);
        Rex(false, destination, 0, source);
        bytes.Add(0x0F);
        bytes.Add(opcode);
        ModRmRegister(destination, source);
    }

    /// <summary><c>ucomisd xmm, xmm</c>: <c>66 0F 2E /r</c>.</summary>
    /// <remarks>
    /// <b>It sets the parity flag when either operand is NaN, and every comparison this backend
    /// emits reads that bit.</b> JavaScript says every relational comparison with a NaN operand is
    /// false and that <c>NaN === NaN</c> is false, and the only way to honour both is to test the
    /// parity flag rather than to trust the zero and carry flags alone.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D1BB8D
    // Broiler-Falsified-If: a comparison this backend emits answers true for an unordered pair
    // Broiler-Human:        PENDING
    public void Ucomisd(int left, int right)
    {
        bytes.Add(0x66);
        Rex(false, left, 0, right);
        bytes.Add(0x0F);
        bytes.Add(0x2E);
        ModRmRegister(left, right);
    }

    /// <summary><c>xorpd xmm, xmm</c>: <c>66 0F 57 /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=89190B
    // Broiler-Human:        PENDING
    public void Xorpd(int destination, int source)
    {
        bytes.Add(0x66);
        Rex(false, destination, 0, source);
        bytes.Add(0x0F);
        bytes.Add(0x57);
        ModRmRegister(destination, source);
    }

    /// <summary><c>cvtsi2sd xmm, r64</c>: <c>F2 REX.W 0F 2A /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C2DAFD
    // Broiler-Human:        PENDING
    public void Cvtsi2sd(int destination, JsX64Register source)
    {
        bytes.Add(0xF2);
        Rex(true, destination, 0, (int)source);
        bytes.Add(0x0F);
        bytes.Add(0x2A);
        ModRmRegister(destination, (int)source);
    }

    /// <summary><c>movq xmm, r64</c>: <c>66 REX.W 0F 6E /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=949B0F
    // Broiler-Human:        PENDING
    public void MovqToXmm(int destination, JsX64Register source)
    {
        bytes.Add(0x66);
        Rex(true, destination, 0, (int)source);
        bytes.Add(0x0F);
        bytes.Add(0x6E);
        ModRmRegister(destination, (int)source);
    }

    /// <summary><c>movq r64, xmm</c>: <c>66 REX.W 0F 7E /r</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D1B8E6
    // Broiler-Human:        PENDING
    public void MovqFromXmm(JsX64Register destination, int source)
    {
        bytes.Add(0x66);
        Rex(true, source, 0, (int)destination);
        bytes.Add(0x0F);
        bytes.Add(0x7E);
        ModRmRegister(source, (int)destination);
    }
}
