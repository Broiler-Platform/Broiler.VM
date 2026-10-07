// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           0
// Human-reviewed:   0/18
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=85D476
    // Broiler-Human:        PENDING
    private void Emit(JsOpcode opcode)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode);
        buffer.Track(opcode, 0);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E6E1D2
    // Broiler-Human:        PENDING
    private void Emit(JsOpcode opcode, ushort operand)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode, operand);
        buffer.Track(opcode, operand);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FB6C8B
    // Broiler-Human:        PENDING
    private void Emit(JsOpcode opcode, byte operand)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode, operand);
        buffer.Track(opcode, operand);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=70C504
    // Broiler-Human:        PENDING
    private void EmitScoped(JsOpcode opcode, byte hops, int slot)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode, hops, (ushort)slot);
        buffer.Track(opcode, 0);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9CA5E1
    // Broiler-Human:        PENDING
    private void Branch(JsOpcode opcode, Label label)
    {
        var site = JsArtifactWriter.EmitBranch(buffer.Code, opcode);
        buffer.BranchSites.Add(site);
        label.Sites.Add(site);
        buffer.Track(opcode, 0);

        if (label.Offset >= 0)
        {
            JsArtifactWriter.PatchBranch(buffer.Code, site, (uint)label.Offset);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D48A65
    // Broiler-Human:        PENDING
    private Label NewLabel()
    {
        var label = new Label();
        buffer.Labels.Add(label);
        return label;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D41CF2
    // Broiler-Human:        PENDING
    private void Mark(Label label)
    {
        label.Offset = buffer.Code.Count;

        foreach (var site in label.Sites)
        {
            JsArtifactWriter.PatchBranch(buffer.Code, site, (uint)label.Offset);
        }
    }

    /// <summary>Places what follows at <paramref name="span"/> when it names a place at all.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7E9920
    // Broiler-Human:        PENDING
    private void PositionAt(SliceSourceSpan span)
    {
        if (span.Line > 0)
        {
            Position(span);
        }
    }

    /// <summary>Places a call: at the method's name for a call of a named member, else at its start.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=239DD2
    // Broiler-Human:        PENDING
    private void PositionCall(JsCallExpression call) =>
        Position(call.Callee is JsMemberExpression { NameSpan.Line: > 0 } member ? member.NameSpan : call.Span);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FFE885
    // Broiler-Human:        PENDING
    private void Position(SliceSourceSpan span)
    {
        if (buffer.LastLine == span.Line && buffer.LastColumn == span.Column)
        {
            return;
        }

        buffer.LastLine = span.Line;
        buffer.LastColumn = span.Column;

        buffer.Positions.Add(
            ((uint)buffer.Code.Count, (uint)System.Math.Max(1, span.Line), (uint)System.Math.Max(1, span.Column)));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9BC56B
    // Broiler-Human:        PENDING
    private ushort NumberConstant(double value)
    {
        var key = "n" + value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        return Intern(key, JsArtifactWriter.NumberConstant(value));
    }

    /// <summary>
    /// A BigInt constant: its canonical encoding, interned by its decimal spelling so that
    /// <c>0x10n</c> and <c>16n</c> share one entry.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0147F8
    // Broiler-Human:        PENDING
    private ushort BigIntConstant(System.Numerics.BigInteger value)
    {
        var key = "b" + value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var magnitude = System.Numerics.BigInteger.Abs(value).ToByteArray(isUnsigned: true, isBigEndian: false);

        // ZERO IS THE EMPTY MAGNITUDE, which is the one spelling the verifier admits for it; the
        // runtime's own encoding of zero is one zero byte.
        if (value.IsZero)
        {
            magnitude = [];
        }

        return Intern(key, JsArtifactWriter.BigIntConstant(value.Sign < 0, magnitude));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=82BF59
    // Broiler-Human:        PENDING
    private ushort StringConstant(string value) =>
        Intern("s" + value, JsArtifactWriter.StringConstant(value));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=64AB04
    // Broiler-Human:        PENDING
    private ushort InternedName(string value) =>
        Intern("i" + value, JsArtifactWriter.InternedNameConstant(value));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A9AA1D
    // Broiler-Human:        PENDING
    private ushort Intern(string key, byte[] encoded)
    {
        if (constantIndex.TryGetValue(key, out var found))
        {
            return found;
        }

        if (constants.Count >= 65535)
        {
            // THE REFUSAL NAMES THE CEILING AND WHERE IT WAS MET, once. The pool is one per
            // artifact and an index is two bytes, so the ceiling is a property of the format and
            // not of the program; the position is the construct being compiled when the first
            // constant past it was asked for. Until 2026-09-29 this said only "the constant pool
            // is full", at 0:0, on every constant past the ceiling.
            if (!constantPoolFull)
            {
                constantPoolFull = true;
                Refuse(
                    new SliceSourceSpan(System.Math.Max(0, buffer.LastLine), System.Math.Max(0, buffer.LastColumn)),
                    SliceSourceDiagnosticCode.TooManyConstants,
                    "the constant pool is full: an artifact of this format holds at most 65,535 distinct " +
                    "constants - numbers, strings, BigInts and names together - and this one needs more");
            }

            return 0;
        }

        var index = (ushort)constants.Count;
        constants.Add(encoded);
        constantIndex[key] = index;
        return index;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A5C9D
    // Broiler-Human:        PENDING
    private static JsOpcode BinaryOpcode(SliceTokenKind kind) => kind switch
    {
        SliceTokenKind.Plus => JsOpcode.Add,
        SliceTokenKind.Minus => JsOpcode.Subtract,
        SliceTokenKind.Star => JsOpcode.Multiply,
        SliceTokenKind.Slash => JsOpcode.Divide,
        SliceTokenKind.Percent => JsOpcode.Remainder,
        SliceTokenKind.StarStar => JsOpcode.Exponent,
        SliceTokenKind.LessThan => JsOpcode.LessThan,
        SliceTokenKind.LessThanEquals => JsOpcode.LessThanOrEqual,
        SliceTokenKind.GreaterThan => JsOpcode.GreaterThan,
        SliceTokenKind.GreaterThanEquals => JsOpcode.GreaterThanOrEqual,
        SliceTokenKind.EqualsEqualsEquals => JsOpcode.StrictEquals,
        SliceTokenKind.BangEqualsEquals => JsOpcode.StrictNotEquals,
        SliceTokenKind.EqualsEquals => JsOpcode.LooseEquals,
        SliceTokenKind.BangEquals => JsOpcode.LooseNotEquals,
        SliceTokenKind.Bar => JsOpcode.BitwiseOr,
        SliceTokenKind.Ampersand => JsOpcode.BitwiseAnd,
        SliceTokenKind.Caret => JsOpcode.BitwiseXor,
        SliceTokenKind.LessThanLessThan => JsOpcode.ShiftLeft,
        SliceTokenKind.GreaterThanGreaterThan => JsOpcode.ShiftRight,
        SliceTokenKind.GreaterThanGreaterThanGreaterThan => JsOpcode.ShiftRightUnsigned,
        SliceTokenKind.Instanceof => JsOpcode.InstanceOf,
        SliceTokenKind.In => JsOpcode.In,
        _ => JsOpcode.Add,
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3032B3
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceSpan span, SliceSourceDiagnosticCode code, string message)
    {
        if (diagnostics.Count >= 64)
        {
            return;
        }

        diagnostics.Add(new SliceSourceDiagnostic(code, message, span.Line, span.Column));
    }
}
