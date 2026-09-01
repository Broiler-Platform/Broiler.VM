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
    string Position,
    string Dimension,
    string Scope,
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
/// Nine entries carry a mode other than <c>default</c>, and they are the nine that cannot be
/// produced by bytes alone: a resource exhaustion needs a runtime with a tight ceiling, a
/// cancellation needs a token that is already cancelled, and an unsupported profile needs a
/// descriptor naming a profile the catalog does not hold. The bytes for all nine are the same
/// well-formed artifact, which is the point - what differs is the host, not the program.
/// </para>
/// <para>
/// <b>Seven of the nine are exhaustions, one per dimension a verification of this profile can
/// exhaust</b>, and each records the dimension and the scope its answer named. An exhaustion
/// carries no diagnostic code, so the diagnostic column is zero on all seven and the pair is the
/// only thing that distinguishes them; a corpus that recorded the category alone would be
/// satisfied by a verifier that refused the right artifact for the wrong reason.
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
        Invalid(
            "wrong-magic",
            WrongMagic(),
            "MalformedEncoding",
            JavaScriptDiagnosticCodes.WrongMagic,
            // The read stage: no frame has been entered, so the section index is -1 and the offset
            // is an offset into the artifact.
            "-1:0:0:0"),
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

        Invalid(
            "a-manifest-identity-longer-than-the-format-admits",
            LongManifestId(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ManifestIdTooLong),
        Invalid(
            "an-overlong-variable-length-integer",
            OverlongFormatVersion(),
            "MalformedEncoding",
            JavaScriptDiagnosticCodes.MalformedEncoding),

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
        Invalid(
            "a-declared-maximum-above-the-formats-own-ceiling",
            OperandStackAboveTheCeiling(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.DeclaredMaximumTooLarge),

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
        Invalid(
            "more-constants-than-the-limits-section-admits",
            MoreConstantsThanDeclared(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ConstantCountExceedsDeclaredMaximum),
        Invalid(
            "a-constant-count-far-beyond-what-the-artifact-carries",
            AHostileConstantCount(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ConstantCountExceedsDeclaredMaximum),

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
        Invalid(
            "an-unknown-opcode",
            Code([0xFF]),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.UnknownOpcode,
            // The link stage: the code section is the third frame, the offset is into IT, and the
            // artifact carries no position table - so both coordinates are the reserved zero.
            "2:0:0:0"),
        Invalid("a-code-section-of-no-length", EmptyCode(), "InconsistentStructure", JavaScriptDiagnosticCodes.EmptyCode),
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
            JavaScriptDiagnosticCodes.MalformedPositionRow,
            // The row that is refused is the row that covers the offset, so this one reports its
            // own line and column.
            "2:1:1:1"),
        Invalid(
            "an-entry-point-name-of-no-length",
            NamelessEntry(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEntryName),
        Invalid(
            "an-entry-point-reached-with-operands-on-the-stack",
            EntryReachedWithOperands(),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.EntryStackNotEmpty),

        // ---- the position encoding ------------------------------------------------------------------
        //
        // The one entry whose point is the POSITION rather than the code. Its refusal is at a code
        // offset the second of two table rows covers, so an encoding that reported the first row,
        // reported no row, or reported the offset against the artifact rather than against the code
        // section would each produce a different manifest row.
        Invalid(
            "a-refusal-covered-by-the-second-position-row",
            RefusalUnderASecondPositionRow(),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.UnknownOpcode,
            "2:3:7:5"),

        // ---- the seven exhaustions, one per dimension a verification can exhaust ------------------------
        //
        // A CEILING is compared and answers CeilingReached; an ALLOWANCE is charged and answers
        // AllowanceExhausted. The two reasons are the core's, not this profile's, and writing the
        // wrong one here is a row that fails rather than a row that quietly agrees.
        //
        // The four ceilings are the bounded reader's, compared inside the verification, and each
        // answers at Artifact scope - the scope the verification itself runs under. The artifact-
        // bytes row is the exception among them: the core compares the payload length against the
        // same effective ceiling one call BEFORE the verifier is entered, so this row records the
        // core's answer and the reader's own artifact-bytes arm is defensive. The ordering
        // assertions reach that arm, by calling the verifier directly with bounds of their own.
        Exhausted("an-artifact-larger-than-this-host-admits", "tight-artifact-bytes",
            "CeilingReached", "ArtifactBytes", "Artifact"),
        Exhausted("a-section-count-ceiling-this-host-declined", "tight-sections",
            "CeilingReached", "SectionCount", "Artifact"),
        Exhausted("a-declared-count-ceiling-this-host-declined", "tight-declared-count",
            "CeilingReached", "DeclaredCount", "Artifact"),
        Exhausted("a-structural-depth-ceiling-this-host-declined", "tight-structural-depth",
            "CeilingReached", "StructuralDepth", "Artifact"),

        // The three allowances are charged through the meter, and the meter reports the LEVEL that
        // refused rather than the scope the verifier can attribute unaided. All three are declared
        // at runtime creation here, so all three answer Runtime - which is why the scope is a
        // column and not a constant: the same profile answers at two scopes depending on which
        // budget ran out.
        Exhausted("an-allocation-this-host-declined", "tight-allocated-bytes",
            "AllowanceExhausted", "AllocatedBytes", "Runtime"),
        Exhausted("a-verifier-work-allowance-this-host-spent", "tight-verifier-work",
            "AllowanceExhausted", "VerifierWork", "Runtime"),
        Exhausted("a-wall-clock-allowance-already-spent", "tight-wall-clock",
            "AllowanceExhausted", "WallClock", "Runtime"),

        // ---- the two that need a host and are not exhaustions -------------------------------------------
        new CorpusEntry(
            "a-token-that-was-already-cancelled",
            "cancelled",
            "Cancellation",
            "Cancelled",
            0,
            "-",
            Unpinned,
            Unnamed,
            Unnamed,
            SliceLowering.Addition()),
        new CorpusEntry(
            "a-profile-the-catalog-does-not-hold",
            "foreign-profile",
            "UnsupportedProfile",
            "ProfileNotInCatalog",
            0,
            "-",
            Unpinned,
            Unnamed,
            Unnamed,
            SliceLowering.Addition()),
    ];

    private static CorpusEntry Ok(string name, byte[] bytes, string completion) =>
        new(name, "default", "Normal", "NormalCompleted", 0, completion, Unpinned, Unnamed, Unnamed, bytes);

    private static CorpusEntry Invalid(
        string name, byte[] bytes, string reason, int code, string position = Unpinned) =>
        new(name, "default", "InvalidArtifact", reason, code, "-", position, Unnamed, Unnamed, bytes);

    /// <summary>
    /// One exhaustion entry: a well-formed program, a host that declined it on one dimension, and
    /// the pair the answer must name.
    /// </summary>
    /// <remarks>
    /// The bytes are the same well-formed program every time, and deliberately so. What separates
    /// these seven rows is the host each is presented to, so a row that failed because its bytes
    /// were malformed would be proving something about the artifact writer instead.
    /// </remarks>
    private static CorpusEntry Exhausted(
        string name, string mode, string reason, string dimension, string scope) =>
        new(name, mode, "ResourceExhaustion", reason, 0, "-", Unpinned, dimension, scope,
            SliceLowering.Addition());

    /// <summary>
    /// The position column of a row that does not pin one.
    /// </summary>
    /// <remarks>
    /// <b>Deliberately not "every row states its position".</b> A position is an offset into bytes
    /// this file assembles, so writing one on every row would mean hand-computing sixty offsets
    /// that no reader could check and that any change to the writer would silently invalidate - and
    /// a producer that asked the verifier for them would be recording the answer it is meant to be
    /// testing. What the rows below pin instead is the ENCODING: one artifact-relative position,
    /// two code-relative ones, and one whose line and column come from the second row of a
    /// two-row table. Each of the four fails differently if the encoding moves, and a row that
    /// pins nothing says so.
    /// </remarks>
    private const string Unpinned = "-";

    /// <summary>
    /// The dimension and scope columns of a row whose answer exhausted nothing.
    /// </summary>
    /// <remarks>
    /// Written on every row that is not an exhaustion, rather than left blank, because the replay
    /// reads what the answer carried and the two fields are present on every diagnostics record: a
    /// row that left them empty would be compared against whichever member of each enumeration
    /// happens to be first.
    /// </remarks>
    private const string Unnamed = "-";

    // ---- the byte-level variants -------------------------------------------------------------------

    private static byte[] WrongMagic()
    {
        var bytes = SliceLowering.Addition();
        bytes[0] = (byte)'X';
        return bytes;
    }

    private static byte[] LongManifestId() =>
        JavaScriptArtifactWriter.Write(
            new string('m', (int)JavaScriptFormat.MaximumManifestIdBytes + 1), StandardSections());

    /// <summary>The format version re-encoded overlong, which is a value that already had a form.</summary>
    /// <remarks>
    /// The four magic bytes are followed by the format version as a variable-length integer, and
    /// version 1 is one byte. <c>0x81 0x00</c> is the same value in two bytes with a redundant zero
    /// continuation, which the bounded reader refuses rather than accepting and truncating - so
    /// this is the entry that pins the reader's malformed-encoding status onto this profile's own
    /// code, as opposed to the header check that refuses a version this build does not define.
    /// </remarks>
    private static byte[] OverlongFormatVersion()
    {
        var bytes = SliceLowering.Addition();
        var spliced = new byte[bytes.Length + 1];

        bytes[..4].CopyTo(spliced, 0);
        spliced[4] = 0x81;
        spliced[5] = 0x00;
        bytes[5..].CopyTo(spliced, 6);

        return spliced;
    }

    private static byte[] OperandStackAboveTheCeiling()
    {
        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Limits,
                    JavaScriptArtifactWriter.Limits(JavaScriptFormat.CeilingOperandStack + 1, 1, 1, 1)),
                standard[1], standard[2], standard[3],
            ]);
    }

    /// <summary>
    /// A pool declaring sixty thousand entries and carrying none, in an artifact of a few dozen
    /// bytes.
    /// </summary>
    /// <remarks>
    /// <b>The answer is the same as its smaller neighbour's and the point is not the answer.</b>
    /// <c>more-constants-than-the-limits-section-admits</c> declares two where one is admitted,
    /// which a verifier that sized the array before comparing would survive - it would have
    /// allocated thirty-two bytes and then refused. This one would have allocated close to a
    /// megabyte from an artifact that carries nothing to fill it, which is what makes the ordering
    /// check sharp rather than arithmetic. The count is below the format's own ceiling on purpose,
    /// so what refuses it is the limits section's declaration and not a structural bound.
    /// </remarks>
    private static byte[] AHostileConstantCount()
    {
        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Limits, JavaScriptArtifactWriter.Limits(16, 1, 1, 1)),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Constants,
                    JavaScriptArtifactWriter.Constants([], declaredCount: 60_000)),
                standard[2], standard[3],
            ]);
    }

    /// <summary>Two pool entries under a limits section that admits one.</summary>
    private static byte[] MoreConstantsThanDeclared()
    {
        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Limits, JavaScriptArtifactWriter.Limits(16, 1, 1, 1)),
                new JavaScriptArtifactWriter.Section(
                    JavaScriptFormat.SectionKind.Constants,
                    JavaScriptArtifactWriter.Constants(
                    [
                        JavaScriptArtifactWriter.NumberConstant(1),
                        JavaScriptArtifactWriter.NumberConstant(2),
                    ])),
                standard[2], standard[3],
            ]);
    }

    private static byte[] EmptyCode()
    {
        var standard = StandardSections();

        return JavaScriptArtifactWriter.Write(
            Manifest,
            [
                standard[0], standard[1],
                new JavaScriptArtifactWriter.Section(JavaScriptFormat.SectionKind.Code, []),
                standard[3],
            ]);
    }

    private static byte[] NamelessEntry() => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            Limits(),
            StandardSections()[1],
            StandardSections()[2],
            Entries([(string.Empty, 0u)]),
        ]);

    /// <summary>
    /// Two entry points, the second of which the first falls into with a value on the stack.
    /// </summary>
    /// <remarks>
    /// A program is entered with an empty operand stack, so the edge from the first entry's
    /// <c>LoadConstant</c> into the second entry is the violation - and it is the EDGE that is
    /// refused, not the join it would otherwise cause, so the answer does not depend on which of
    /// the two arrivals the verifier's worklist happens to pop second.
    /// </remarks>
    private static byte[] EntryReachedWithOperands() => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            Limits(),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Constants,
                JavaScriptArtifactWriter.Constants([JavaScriptArtifactWriter.NumberConstant(1)])),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Code,
                [
                    (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
                    (byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00,
                    (byte)JavaScriptOpcode.Return,
                ]),
            Entries([(SliceLowering.MainEntry, 0u), ("second", 3u)]),
        ]);

    /// <summary>
    /// An unknown opcode at an offset the SECOND of two position-table rows covers.
    /// </summary>
    /// <remarks>
    /// The refusal is ordinary; the row it produces is the point. The code section is the third
    /// frame, the bad byte is at code offset 3, and the row covering offset 3 says line 7 column 5
    /// - so the manifest records <c>2:3:7:5</c> and an encoding that reported the artifact-relative
    /// offset, the first row, or no row at all would each write a different one.
    /// </remarks>
    private static byte[] RefusalUnderASecondPositionRow() => JavaScriptArtifactWriter.Write(
        Manifest,
        [
            Limits(),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Constants,
                JavaScriptArtifactWriter.Constants([JavaScriptArtifactWriter.NumberConstant(1)])),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Code,
                [(byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00, 0xFF]),
            Entries([(SliceLowering.MainEntry, 0u)]),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Positions,
                JavaScriptArtifactWriter.Positions([(0u, 1u, 1u), (3u, 7u, 5u)])),
        ]);

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
    internal const int ManifestIdTooLong = 1005;
    internal const int UnsupportedFeatureManifest = 1004;
    internal const int UnknownSectionKind = 1101;
    internal const int SectionOrder = 1102;
    internal const int MissingSection = 1103;
    internal const int TrailingBytes = 1104;
    internal const int SectionLengthMismatch = 1105;
    internal const int DeclaredMaximumTooLarge = 1201;
    internal const int DeclaredFrameCount = 1202;
    internal const int UnknownConstantTag = 1301;
    internal const int MalformedBooleanConstant = 1302;
    internal const int ConstantCountExceedsDeclaredMaximum = 1303;
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
    internal const int EmptyCode = 1410;
    internal const int UnreachableCode = 1411;
    internal const int ReturnStackNotExactlyOne = 1412;
    internal const int NoEntryPoint = 1501;
    internal const int DuplicateEntryPoint = 1502;
    internal const int EntryOffsetNotAnInstructionBoundary = 1503;
    internal const int MalformedEntryName = 1504;
    internal const int EntryStackNotEmpty = 1505;
    internal const int MalformedPositionRow = 1506;
    internal const int ExceptionRegionOutsideManifest = 1507;
    internal const int SuspensionTargetOutsideManifest = 1508;
    internal const int Truncated = 1901;
    internal const int MalformedEncoding = 1902;
}
