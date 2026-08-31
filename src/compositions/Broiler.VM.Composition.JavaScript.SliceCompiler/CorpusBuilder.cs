using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>One retained corpus entry: bytes, and the answer they are expected to produce.</summary>
/// <remarks>
/// The expectation is written down before the replay runs, which is what makes a replay a
/// comparison rather than a recording. An entry whose observed triple differs from what is written
/// here fails the replay; it does not update it.
/// </remarks>
internal sealed record CorpusEntry(
    string Name,
    string Mode,
    string Outcome,
    string Reason,
    int DiagnosticCode,
    string Completion,
    byte[] Bytes);

/// <summary>
/// The retained malformed corpus for <c>broiler.javascript.slice</c>, and the control entries that
/// verify successfully.
/// </summary>
/// <remarks>
/// <para>
/// <b>A corpus in which nothing passes is a corpus that would not notice a verifier that rejects
/// everything</b>, so the control entries are not a courtesy: sixteen of the entries below are
/// well-formed programs with a recorded completion value, and each of those values is a claim
/// about the language rather than about arithmetic.
/// </para>
/// <para>
/// The malformed entries are organised by the stage that must refuse them - header, framing,
/// limits, constants, code, entries - so that a gap in the table is visible as a stage with no
/// entry rather than as a count that looks large enough.
/// </para>
/// <para>
/// Three entries carry a mode other than <c>default</c>, and they are the three that cannot be
/// produced by bytes alone: a resource exhaustion needs a runtime with a tight ceiling, a
/// cancellation needs a token that is already cancelled, and an unsupported profile needs a
/// descriptor naming a profile the catalog does not hold. The bytes for all three are the same
/// well-formed artifact, which is the point - what differs is the host, not the program.
/// </para>
/// </remarks>
internal static class CorpusBuilder
{
    private const string Manifest = SliceLowering.SliceManifestId;

    internal static CorpusEntry[] Build() =>
    [
        // ---- control entries: well-formed programs, each a claim about the language ----------
        Ok("addition", SliceLowering.Addition(), "42"),
        Ok("division-by-zero", SliceLowering.DivisionByZero(), "Infinity"),
        Ok("zero-over-zero", SliceLowering.NotANumber(), "NaN"),
        Ok("negative-zero-division", SliceLowering.NegativeZeroDivision(), "-Infinity"),
        Ok("remainder-takes-the-dividends-sign", SliceLowering.RemainderSign(), "-2"),
        Ok("bitwise-or-goes-through-to-int32", SliceLowering.ToInt32Wraps(), "-2147483648"),
        Ok("unsigned-shift-goes-through-to-uint32", SliceLowering.UnsignedShiftIsUnsigned(), "4294967295"),
        Ok("comparison-produces-a-boolean", SliceLowering.ComparisonProducesABoolean(), "true"),
        Ok("not-a-number-is-not-itself", SliceLowering.NotANumberIsNotItself(), "false"),
        Ok("a-boolean-adds-as-one", SliceLowering.BooleanAddsAsOne(), "2"),
        Ok("strict-equality-compares-kinds", SliceLowering.StrictEqualityComparesKinds(), "false"),
        Ok("an-unassigned-local-is-undefined", SliceLowering.UnassignedLocalIsUndefined(), "undefined"),
        Ok("a-counting-loop", SliceLowering.CountingLoop(10), "55"),
        Ok("a-taken-conditional", SliceLowering.Conditional(true, 1, 2), "1"),
        Ok("an-untaken-conditional", SliceLowering.Conditional(false, 1, 2), "2"),
        Ok("two-entry-points", SliceLowering.TwoEntryPoints(), "1"),

        // ---- the header ----------------------------------------------------------------------
        Invalid("wrong-magic", WrongMagic(), "MalformedEncoding", JavaScriptDiagnosticCodes.WrongMagic),
        Invalid(
            "unsupported-format-version",
            FormatVersion(2),
            "UnsupportedProfileFormatVersion",
            JavaScriptDiagnosticCodes.UnsupportedFormatVersion),
        Invalid(
            "a-manifest-this-build-does-not-accept",
            OtherManifest(),
            "UnsupportedFeatureManifest",
            JavaScriptDiagnosticCodes.UnsupportedFeatureManifest),

        // ---- section framing ------------------------------------------------------------------
        Invalid("trailing-bytes", TrailingBytes(), "InconsistentStructure", JavaScriptDiagnosticCodes.TrailingBytes),
        Invalid("an-unknown-section-kind", UnknownSection(), "UnknownFeature", JavaScriptDiagnosticCodes.UnknownSectionKind),
        Invalid("a-duplicated-section", DuplicateSection(), "InconsistentStructure", JavaScriptDiagnosticCodes.SectionOrder),
        Invalid("sections-out-of-order", OutOfOrder(), "InconsistentStructure", JavaScriptDiagnosticCodes.SectionOrder),
        Invalid("no-code-section", NoCode(), "InconsistentStructure", JavaScriptDiagnosticCodes.MissingSection),
        Invalid(
            "a-section-shorter-than-it-declared",
            SectionLengthMismatch(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.SectionLengthMismatch),
        Invalid("a-truncated-payload", Truncated(), "Truncated", JavaScriptDiagnosticCodes.Truncated),

        // ---- the limits section ----------------------------------------------------------------
        Invalid("more-frames-than-the-slice-has", TwoFrames(), "UnknownFeature", JavaScriptDiagnosticCodes.DeclaredFrameCount),

        // ---- the constant pool ------------------------------------------------------------------
        Invalid(
            "an-interned-name-the-manifest-excludes",
            InternedName(),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.InternedNameOutsideManifest),
        Invalid("an-unknown-constant-tag", UnknownTag(), "UnknownFeature", JavaScriptDiagnosticCodes.UnknownConstantTag),
        Invalid(
            "a-boolean-constant-that-is-neither",
            BadBoolean(),
            "MalformedEncoding",
            JavaScriptDiagnosticCodes.MalformedBooleanConstant),

        // ---- the reserved sections ---------------------------------------------------------------
        Invalid(
            "an-exception-region-the-manifest-excludes",
            Reserved(JavaScriptFormat.SectionKind.ExceptionRegions),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.ExceptionRegionOutsideManifest),
        Invalid(
            "a-suspension-target-the-manifest-excludes",
            Reserved(JavaScriptFormat.SectionKind.SuspensionTargets),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.SuspensionTargetOutsideManifest),

        // ---- the code section ---------------------------------------------------------------------
        Invalid("an-unknown-opcode", Code([0xFF]), "UnknownFeature", JavaScriptDiagnosticCodes.UnknownOpcode),
        Invalid(
            "an-instruction-whose-operand-runs-off-the-end",
            Code([(byte)JavaScriptOpcode.LoadConstant, 0x00]),
            "Truncated",
            JavaScriptDiagnosticCodes.TruncatedInstruction),
        Invalid(
            "a-jump-out-of-range",
            Code([(byte)JavaScriptOpcode.Jump, 0x00, 0x01, 0x00, 0x00]),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.JumpTargetNotAnInstructionBoundary),
        Invalid(
            "a-jump-into-an-operand",
            JumpIntoOperand(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.JumpTargetNotAnInstructionBoundary),
        Invalid(
            "an-operand-stack-underflow",
            Code([(byte)JavaScriptOpcode.Add, (byte)JavaScriptOpcode.Return]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.OperandStackUnderflow),
        Invalid(
            "an-operand-stack-deeper-than-declared",
            StackOverflow(),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.OperandStackOverflow),
        Invalid(
            "two-paths-reaching-one-offset-at-different-heights",
            JoinMismatch(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.InconsistentStackHeightAtJoin),
        Invalid(
            "an-instruction-no-entry-point-reaches",
            Unreachable(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.UnreachableCode),
        Invalid(
            "code-that-falls-off-the-end",
            Code([(byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.FallsOffTheEnd),
        Invalid(
            "a-constant-index-past-the-pool",
            Code([(byte)JavaScriptOpcode.LoadConstant, 0x05, 0x00, (byte)JavaScriptOpcode.Return]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.ConstantIndexOutOfRange),
        Invalid(
            "a-local-index-past-the-frame",
            Code([(byte)JavaScriptOpcode.LoadLocal, 0x05, 0x00, (byte)JavaScriptOpcode.Return]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.LocalIndexOutOfRange),
        Invalid(
            "a-return-that-leaves-more-than-the-completion-value",
            Code(
            [
                (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
                (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
                (byte)JavaScriptOpcode.Return,
            ]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.ReturnStackNotExactlyOne),

        // ---- entries and positions ------------------------------------------------------------------
        Invalid("no-entry-point", NoEntry(), "InconsistentStructure", JavaScriptDiagnosticCodes.NoEntryPoint),
        Invalid("a-duplicated-entry-point", DuplicateEntry(), "InconsistentStructure", JavaScriptDiagnosticCodes.DuplicateEntryPoint),
        Invalid(
            "an-entry-point-inside-an-operand",
            EntryIntoOperand(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.EntryOffsetNotAnInstructionBoundary),
        Invalid(
            "a-position-row-inside-an-operand",
            PositionIntoOperand(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedPositionRow),

        // ---- the three that need a host rather than bytes ----------------------------------------------
        new CorpusEntry(
            "a-section-count-ceiling-this-host-declined",
            "tight-sections",
            "ResourceExhaustion",
            "CeilingReached",
            0,
            "-",
            SliceLowering.Addition()),
        new CorpusEntry(
            "a-token-that-was-already-cancelled",
            "cancelled",
            "Cancellation",
            "Cancelled",
            0,
            "-",
            SliceLowering.Addition()),
        new CorpusEntry(
            "a-profile-the-catalog-does-not-hold",
            "foreign-profile",
            "UnsupportedProfile",
            "ProfileNotInCatalog",
            0,
            "-",
            SliceLowering.Addition()),
    ];

    private static CorpusEntry Ok(string name, byte[] bytes, string completion) =>
        new(name, "default", "Normal", "NormalCompleted", 0, completion, bytes);

    private static CorpusEntry Invalid(string name, byte[] bytes, string reason, int code) =>
        new(name, "default", "InvalidArtifact", reason, code, "-", bytes);

    // ---- the byte-level variants -------------------------------------------------------------------

    private static byte[] WrongMagic()
    {
        var bytes = SliceLowering.Addition();
        bytes[0] = (byte)'X';
        return bytes;
    }

    private static byte[] FormatVersion(uint version) =>
        JavaScriptArtifactWriter.Write(Manifest, StandardSections(), formatVersion: version);

    private static byte[] OtherManifest() =>
        JavaScriptArtifactWriter.Write("broiler.javascript.core", StandardSections());

    private static byte[] TrailingBytes()
    {
        var bytes = SliceLowering.Addition();
        var extended = new byte[bytes.Length + 1];
        bytes.CopyTo(extended, 0);
        return extended;
    }

    private static byte[] Truncated()
    {
        var bytes = SliceLowering.Addition();
        return bytes[..(bytes.Length - 3)];
    }

    private static byte[] UnknownSection()
    {
        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>(StandardSections())
        {
            new((JavaScriptFormat.SectionKind)99, JavaScriptArtifactWriter.Count(0)),
        };

        return JavaScriptArtifactWriter.Write(Manifest, sections.ToArray());
    }

    private static byte[] DuplicateSection()
    {
        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            Limits(),
            Limits(),
        };

        sections.AddRange(StandardSections()[1..]);
        return JavaScriptArtifactWriter.Write(Manifest, sections.ToArray());
    }

    private static byte[] OutOfOrder()
    {
        var standard = StandardSections();
        return JavaScriptArtifactWriter.Write(Manifest, [standard[2], standard[0], standard[1], standard[3]]);
    }

    private static byte[] NoCode()
    {
        var standard = StandardSections();
        return JavaScriptArtifactWriter.Write(Manifest, [standard[0], standard[1], standard[3]]);
    }

    private static byte[] SectionLengthMismatch()
    {
        var body = JavaScriptArtifactWriter.Limits(16, 0, 1, 1);
        var padded = new byte[body.Length + 1];
        body.CopyTo(padded, 0);

        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                new JavaScriptArtifactWriter.Section(JavaScriptFormat.SectionKind.Limits, padded),
                standard[1], standard[2], standard[3],
            ]);
    }

    private static byte[] TwoFrames()
    {
        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Limits, JavaScriptArtifactWriter.Limits(16, 0, 2, 1)),
                standard[1], standard[2], standard[3],
            ]);
    }

    private static byte[] InternedName() => WithConstants(
        [JavaScriptArtifactWriter.NumberConstant(1), JavaScriptArtifactWriter.InternedNameConstant("length")]);

    private static byte[] UnknownTag() => WithConstants(
        [JavaScriptArtifactWriter.NumberConstant(1), [7]]);

    private static byte[] BadBoolean() => WithConstants(
        [JavaScriptArtifactWriter.NumberConstant(1), JavaScriptArtifactWriter.BooleanConstant(2)]);

    private static byte[] WithConstants(byte[][] entries)
    {
        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Limits,
                    JavaScriptArtifactWriter.Limits(16, 0, 1, (uint)entries.Length)),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Constants, JavaScriptArtifactWriter.Constants(entries)),
                standard[2], standard[3],
            ]);
    }

    private static byte[] Reserved(JavaScriptFormat.SectionKind kind)
    {
        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>(StandardSections())
        {
            new(kind, JavaScriptArtifactWriter.Count(1)),
        };

        return JavaScriptArtifactWriter.Write(Manifest, sections.ToArray());
    }

    /// <summary>An artifact whose code section is exactly <paramref name="code"/>.</summary>
    /// <remarks>
    /// The pool holds one Number and the frame holds one local, so a variant can address either
    /// in range or out of it without changing anything else about the artifact.
    /// </remarks>
    private static byte[] Code(byte[] code) => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            Limits(),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Constants,
                JavaScriptArtifactWriter.Constants([JavaScriptArtifactWriter.NumberConstant(1)])),
            new JavaScriptArtifactWriter.Section(JavaScriptFormat.SectionKind.Code, code),
            Entries([(SliceLowering.MainEntry, 0u)]),
        ]);

    private static byte[] JumpIntoOperand() => Code(
    [
        // Jump +1, which lands on the second byte of the following LoadConstant rather than on an
        // instruction boundary.
        (byte)JavaScriptOpcode.Jump, 0x01, 0x00, 0x00, 0x00,
        (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
        (byte)JavaScriptOpcode.Return,
    ]);

    private static byte[] StackOverflow() => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Limits, JavaScriptArtifactWriter.Limits(1, 1, 1, 1)),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Constants,
                JavaScriptArtifactWriter.Constants([JavaScriptArtifactWriter.NumberConstant(1)])),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Code,
                [
                    (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
                    (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
                    (byte)JavaScriptOpcode.Add,
                    (byte)JavaScriptOpcode.Return,
                ]),
            Entries([(SliceLowering.MainEntry, 0u)]),
        ]);

    private static byte[] JoinMismatch() => Code(
    [
        // Push, branch on it (which pops), push again, then land on a Return that the branch
        // reaches at height 0 and the fall-through reaches at height 1.
        (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
        (byte)JavaScriptOpcode.JumpIfFalse, 0x03, 0x00, 0x00, 0x00,
        (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
        (byte)JavaScriptOpcode.Return,
    ]);

    private static byte[] Unreachable() => Code(
    [
        (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
        (byte)JavaScriptOpcode.Return,
        (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
        (byte)JavaScriptOpcode.Return,
    ]);

    private static byte[] NoEntry() => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            Limits(),
            StandardSections()[1],
            StandardSections()[2],
            Entries([]),
        ]);

    private static byte[] DuplicateEntry() => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            Limits(),
            StandardSections()[1],
            StandardSections()[2],
            Entries([(SliceLowering.MainEntry, 0u), (SliceLowering.MainEntry, 0u)]),
        ]);

    private static byte[] EntryIntoOperand() => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            Limits(),
            StandardSections()[1],
            StandardSections()[2],
            Entries([(SliceLowering.MainEntry, 1u)]),
        ]);

    private static byte[] PositionIntoOperand()
    {
        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                standard[0], standard[1], standard[2], standard[3],
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Positions,
                    JavaScriptArtifactWriter.Positions([(1u, 1u, 1u)])),
            ]);
    }

    /// <summary>
    /// The four required sections of a minimal well-formed artifact: one Number constant, a code
    /// section that loads it and returns, and one entry point.
    /// </summary>
    private static JavaScriptArtifactWriter.Section[] StandardSections() =>
    [
        Limits(),
        new JavaScriptArtifactWriter.Section(
            JavaScriptFormat.SectionKind.Constants,
            JavaScriptArtifactWriter.Constants([JavaScriptArtifactWriter.NumberConstant(1)])),
        new JavaScriptArtifactWriter.Section(
            JavaScriptFormat.SectionKind.Code,
            [(byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00, (byte)JavaScriptOpcode.Return]),
        Entries([(SliceLowering.MainEntry, 0u)]),
    ];

    private static JavaScriptArtifactWriter.Section Limits() =>
        new(JavaScriptFormat.SectionKind.Limits, JavaScriptArtifactWriter.Limits(16, 1, 1, 1));

    private static JavaScriptArtifactWriter.Section Entries((string Name, uint Offset)[] entries) =>
        new(JavaScriptFormat.SectionKind.Entries, JavaScriptArtifactWriter.Entries(entries));
}

/// <summary>
/// The diagnostic codes this corpus pins, restated here rather than referenced.
/// </summary>
/// <remarks>
/// <b>The duplication is deliberate and it is the point.</b> A corpus that read its expected codes
/// from the profile it is testing would agree with the profile by construction: renaming a
/// constant's value would move both sides and nothing would go red. These are the numbers a
/// reviewer reads in the published registry, written out where a change to either half has to be
/// made twice - which is what makes the second write a review rather than a rebuild. JS-3a
/// publishes the registry and binds both halves to it.
/// </remarks>
internal static class JavaScriptDiagnosticCodes
{
    internal const int WrongMagic = 1001;
    internal const int UnsupportedFormatVersion = 1002;
    internal const int UnsupportedFeatureManifest = 1004;
    internal const int UnknownSectionKind = 1101;
    internal const int SectionOrder = 1102;
    internal const int MissingSection = 1103;
    internal const int TrailingBytes = 1104;
    internal const int SectionLengthMismatch = 1105;
    internal const int DeclaredFrameCount = 1202;
    internal const int UnknownConstantTag = 1301;
    internal const int MalformedBooleanConstant = 1302;
    internal const int InternedNameOutsideManifest = 1304;
    internal const int UnknownOpcode = 1401;
    internal const int TruncatedInstruction = 1402;
    internal const int JumpTargetNotAnInstructionBoundary = 1403;
    internal const int InconsistentStackHeightAtJoin = 1404;
    internal const int OperandStackUnderflow = 1405;
    internal const int OperandStackOverflow = 1406;
    internal const int ConstantIndexOutOfRange = 1407;
    internal const int LocalIndexOutOfRange = 1408;
    internal const int FallsOffTheEnd = 1409;
    internal const int UnreachableCode = 1411;
    internal const int ReturnStackNotExactlyOne = 1412;
    internal const int NoEntryPoint = 1501;
    internal const int DuplicateEntryPoint = 1502;
    internal const int EntryOffsetNotAnInstructionBoundary = 1503;
    internal const int MalformedPositionRow = 1506;
    internal const int ExceptionRegionOutsideManifest = 1507;
    internal const int SuspensionTargetOutsideManifest = 1508;
    internal const int Truncated = 1901;
}
