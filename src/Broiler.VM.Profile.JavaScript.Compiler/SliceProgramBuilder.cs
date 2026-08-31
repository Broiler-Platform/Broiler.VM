// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           11
// Human-reviewed:   0/24
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>A forward or backward branch target, resolved when it is marked.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A3EF78
// Broiler-Human:        PENDING
public sealed class SliceLabel
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=6D9567
    // Broiler-Human:        PENDING
    internal SliceLabel()
    {
    }

    /// <summary>Where the label was marked, or -1 while it is still unresolved.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=8A0B10
    // Broiler-Human:        PENDING
    internal int Offset { get; set; } = -1;

    /// <summary>The displacement fields waiting to be patched once the offset is known.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=9F112C
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<int> Pending { get; } = [];
}

/// <summary>
/// The hand-written lowering for <c>broiler.javascript.slice</c>: a constant pool, a local frame,
/// an instruction buffer and label patching.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is a sibling of the profile and never a part of it.</b> Nothing here references the
/// profile assembly, and the profile references nothing here - the two meet at the format, which
/// is why an execution-only composition contains a format, a verifier and an interpreter and no
/// lowering at all. That is a property of the reference set rather than of a build switch, and
/// rule N1 asserts it.
/// </para>
/// <para>
/// <b>It is hand-written and scheduled for deletion.</b> Milestone JS-3b writes the real
/// source-to-bytecode lowering over a tokenizer and a validated tree; this one exists so that JS-1
/// can close the whole contract loop against about two thousand readable lines rather than against
/// a copied engine. JS-4's exit gate carries the deletion as a clause with a named owner, because
/// a second lowering that outlived its milestone is a second lowering.
/// </para>
/// <para>
/// <b>It emits no invalid artifact by accident and refuses to hide one.</b> Constants are interned
/// so a program that mentions <c>1</c> twice carries it once; labels are patched exactly once and
/// an unmarked label is an error at build time rather than a displacement of zero. What it does
/// NOT do is validate: the verifier is the boundary, and a lowering that checked what the verifier
/// checks would be a second verifier with a schedule attached.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=450AA4
// Broiler-Human:        PENDING
public sealed class SliceProgramBuilder
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2FFCE8
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<byte> code = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BFC91C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<byte[]> constants = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=4F87A1
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<string> constantKeys = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=8EA7BD
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(string Name, uint Offset)> entries = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=87DC9B
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(uint Offset, uint Line, uint Column)> positions = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=F50074
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceLabel> labels = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=F3308C
    // Broiler-Human:        PENDING
    private int localCount;

    /// <summary>Where the next instruction will be emitted.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=6D5481
    // Broiler-Human:        PENDING
    public int Offset => code.Count;

    /// <summary>How many local slots the frame declares.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=315267
    // Broiler-Human:        PENDING
    public int LocalCount => localCount;

    /// <summary>Declares one more local slot and answers its index.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E252B1
    // Broiler-Human:        PENDING
    public int DeclareLocal() => localCount++;

    /// <summary>Interns a Number constant and answers its pool index.</summary>
    /// <remarks>
    /// The key includes the exact bits rather than the value, so <c>+0</c> and <c>-0</c> are two
    /// entries. They are two values in the language - <c>1/-0</c> is <c>-Infinity</c> - and
    /// interning them together would make a program that distinguished them impossible to write.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=646B54
    // Broiler-Human:        PENDING
    public ushort Number(double value) =>
        Intern(
            "n:" + System.BitConverter.DoubleToInt64Bits(value).ToString(
                System.Globalization.CultureInfo.InvariantCulture),
            JavaScriptArtifactWriter.NumberConstant(value));

    /// <summary>Interns a Boolean constant and answers its pool index.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=422A0D
    // Broiler-Human:        PENDING
    public ushort Boolean(bool value) =>
        Intern("b:" + (value ? "1" : "0"), JavaScriptArtifactWriter.BooleanConstant(value ? (byte)1 : (byte)0));

    /// <summary>Interns the <c>undefined</c> constant and answers its pool index.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=20141E
    // Broiler-Human:        PENDING
    public ushort Undefined() => Intern("u", JavaScriptArtifactWriter.UndefinedConstant());

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C59433
    // Broiler-Human:        PENDING
    private ushort Intern(string key, byte[] encoded)
    {
        var existing = constantKeys.IndexOf(key);

        if (existing >= 0)
        {
            return (ushort)existing;
        }

        constantKeys.Add(key);
        constants.Add(encoded);
        return (ushort)(constants.Count - 1);
    }

    /// <summary>Emits one operand-free instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=4252D3
    // Broiler-Human:        PENDING
    public SliceProgramBuilder Emit(JavaScriptOpcode opcode)
    {
        JavaScriptArtifactWriter.Emit(code, opcode);
        return this;
    }

    /// <summary>Emits one instruction carrying a slot index.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=31A0B0
    // Broiler-Human:        PENDING
    public SliceProgramBuilder Emit(JavaScriptOpcode opcode, ushort index)
    {
        JavaScriptArtifactWriter.Emit(code, opcode, index);
        return this;
    }

    /// <summary>Emits a constant load.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=7B3477
    // Broiler-Human:        PENDING
    public SliceProgramBuilder LoadNumber(double value) => Emit(JavaScriptOpcode.LoadConstant, Number(value));

    /// <summary>Emits a Boolean load.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=0E3742
    // Broiler-Human:        PENDING
    public SliceProgramBuilder LoadBoolean(bool value) => Emit(JavaScriptOpcode.LoadConstant, Boolean(value));

    /// <summary>Emits an <c>undefined</c> load.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=0ACB82
    // Broiler-Human:        PENDING
    public SliceProgramBuilder LoadUndefined() => Emit(JavaScriptOpcode.LoadConstant, Undefined());

    /// <summary>Emits a local read.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=46DD06
    // Broiler-Human:        PENDING
    public SliceProgramBuilder LoadLocal(int slot) => Emit(JavaScriptOpcode.LoadLocal, (ushort)slot);

    /// <summary>Emits a local write.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2193A1
    // Broiler-Human:        PENDING
    public SliceProgramBuilder StoreLocal(int slot) => Emit(JavaScriptOpcode.StoreLocal, (ushort)slot);

    /// <summary>Creates an unresolved label.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=5FE79B
    // Broiler-Human:        PENDING
    public SliceLabel DefineLabel()
    {
        var label = new SliceLabel();
        labels.Add(label);
        return label;
    }

    /// <summary>Resolves <paramref name="label"/> to the current offset and patches every branch to it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6873C9
    // Broiler-Human:        PENDING
    public SliceProgramBuilder MarkLabel(SliceLabel label)
    {
        if (label.Offset >= 0)
        {
            throw new System.InvalidOperationException("the label is already marked");
        }

        label.Offset = code.Count;

        foreach (var at in label.Pending)
        {
            JavaScriptArtifactWriter.PatchJump(code, at, label.Offset);
        }

        label.Pending.Clear();
        return this;
    }

    /// <summary>Emits a branch to <paramref name="label"/>, patching it now or when it is marked.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=98D036
    // Broiler-Human:        PENDING
    public SliceProgramBuilder Branch(JavaScriptOpcode opcode, SliceLabel label)
    {
        var at = JavaScriptArtifactWriter.EmitJump(code, opcode);

        if (label.Offset >= 0)
        {
            JavaScriptArtifactWriter.PatchJump(code, at, label.Offset);
        }
        else
        {
            label.Pending.Add(at);
        }

        return this;
    }

    /// <summary>Declares an entry point starting at the current offset.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=3B093C
    // Broiler-Human:        PENDING
    public SliceProgramBuilder Entry(string name)
    {
        entries.Add((name, (uint)code.Count));
        return this;
    }

    /// <summary>Records one canonical position row for the current offset.</summary>
    /// <remarks>
    /// The table is canonical against bytecode offsets rather than against any later
    /// specialization, so a stack trace and a breakpoint name a stable thing. At this milestone
    /// nothing is lowered from source, so a row's line and column are whatever the caller states -
    /// which is why the profile's position encoding leaves the two profile-owned coordinates at
    /// zero until JS-3a decides what they carry.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=87E74F
    // Broiler-Human:        PENDING
    public SliceProgramBuilder Position(uint line, uint column)
    {
        positions.Add(((uint)code.Count, line, column));
        return this;
    }

    /// <summary>
    /// Frames the whole artifact: the four required sections, the two reserved ones, and the
    /// position table.
    /// </summary>
    /// <remarks>
    /// The declared operand-stack maximum is a parameter rather than something this builder
    /// computes, and that is deliberate. Computing it here would make the artifact's declaration
    /// agree with the verifier's own walk by construction, and the case that matters - a program
    /// declaring less headroom than it uses - could then not be written at all.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3B8928
    // Broiler-Human:        PENDING
    public byte[] ToArtifact(string manifestId, uint declaredOperandStack)
    {
        foreach (var label in labels)
        {
            if (label.Offset < 0)
            {
                throw new System.InvalidOperationException(
                    "a label was branched to and never marked; the displacement would be a lie");
            }
        }

        return JavaScriptArtifactWriter.Write(
            manifestId,
            [
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Limits,
                    JavaScriptArtifactWriter.Limits(
                        declaredOperandStack, (uint)localCount, 1, (uint)constants.Count)),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Constants,
                    JavaScriptArtifactWriter.Constants(constants.ToArray())),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Code, code.ToArray()),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Entries,
                    JavaScriptArtifactWriter.Entries(entries.ToArray())),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.ExceptionRegions, JavaScriptArtifactWriter.Count(0)),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.SuspensionTargets, JavaScriptArtifactWriter.Count(0)),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Positions,
                    JavaScriptArtifactWriter.Positions(positions.ToArray())),
            ]);
    }

    /// <summary>The raw code bytes, for a caller that frames the artifact itself.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=439CA9
    // Broiler-Human:        PENDING
    public byte[] CodeBytes() => code.ToArray();

    /// <summary>The encoded constant-pool entries, for a caller that frames the artifact itself.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=86AB83
    // Broiler-Human:        PENDING
    public byte[][] ConstantEntries() => constants.ToArray();

    /// <summary>The declared entry points, for a caller that frames the artifact itself.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A4507B
    // Broiler-Human:        PENDING
    public (string Name, uint Offset)[] EntryPoints() => entries.ToArray();
}
