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

        // ---- control entries lowered FROM SOURCE ---------------------------------------------
        //
        // Every entry above is bytecode a human wrote directly, so each is a claim about the
        // verifier and the executor and about nothing else. These are compiled from JavaScript
        // text by the front end, so each is additionally a claim about the tokenizer, the parser,
        // the one validation stage and the lowering - and they are in the SAME corpus, judged by
        // the SAME replay, because the answer to `10 - 3 - 2` does not become a different kind of
        // fact for having been written in JavaScript.
        //
        // The source is retained beside the bytes. A reader who cannot see the program cannot
        // check the claim, and a corpus of opaque blobs is a corpus that is trusted rather than
        // read.
        .. CompiledFromSource(),

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
        Pinned(
            "an-unknown-opcode",
            UnknownOpcodeInCode(),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.UnknownOpcode,
            // The link stage: the code section is the third frame, the offset is into IT, and the
            // artifact carries no position table - so both coordinates are the reserved zero.
            // Hand-computed, and kept as the answer the derivation has to reproduce.
            handComputed: "2:0:0:0"),
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
        Pinned(
            "a-position-row-inside-an-operand",
            PositionIntoOperand(),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedPositionRow,
            // The row that is refused is the row that covers the offset, so this one reports its
            // own line and column. Hand-computed against the format document, and kept as the
            // answer the derivation has to reproduce.
            handComputed: "2:1:1:1"),
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
        Pinned(
            "a-refusal-covered-by-the-second-position-row",
            RefusalUnderASecondPositionRow(),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.UnknownOpcode,
            handComputed: "2:3:7:5"),

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

        // ---- format version 2: the structural refusals the wide surface adds -----------------
        //
        // They live in WideCorpus.cs because they are a different format, and they are SPLICED in
        // here rather than kept in a corpus of their own because the retained corpus is one
        // manifest with one integrity check over it. The mode column is what says which format a
        // row's bytes are.
        ..WideCorpus.Build(),

        // ---- the module goal: a graph rather than a table --------------------------------------
        //
        // Spliced in for the reason the version-2 rows are: one manifest, one integrity check. Two
        // of these rows vary the COMPOSITION rather than the bytes, which their mode column says.
        ..ModuleCorpus.Build(),
    ];

    private static CorpusEntry Ok(string name, byte[] bytes, string completion) =>
        new(name, "default", "Normal", "NormalCompleted", 0, completion, Unpinned, Unnamed, Unnamed, bytes);

    /// <summary>
    /// One entry per accepted source, its bytes produced by the front end.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The producer refuses to write a corpus it could not compile.</b> A source that fails to
    /// compile here is not skipped and not recorded as a refusal: it throws, because the entry
    /// list is a claim that these programs are inside the manifest and a corpus quietly missing
    /// three of them would still replay green.
    /// </para>
    /// <para>
    /// The expected completion is the string written beside the source in
    /// <c>SliceSourcePrograms</c>, which is a human's answer about JavaScript. Nothing here asks
    /// the executor what it thinks: the replay does that, in another image, and the comparison is
    /// only worth something while the two answers have separate authors.
    /// </para>
    /// </remarks>
    private static CorpusEntry[] CompiledFromSource()
    {
        var accepted = SliceSourcePrograms.Accepted;
        var entries = new CorpusEntry[accepted.Length];

        for (var at = 0; at < accepted.Length; at++)
        {
            var program = accepted[at];
            var compiled = SliceSourceCompiler.Compile(program.Source);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                var why = compiled.Diagnostics.Count > 0
                    ? compiled.Diagnostics[0].ToString()
                    : "no artifact and no diagnostic";

                throw new InvalidOperationException(
                    $"the corpus entry `{program.Name}` did not compile: {why}");
            }

            entries[at] = Ok(program.Name, compiled.Artifact, program.Completion);
        }

        return entries;
    }

    private static CorpusEntry Invalid(
        string name, byte[] bytes, string reason, int code, string position = Unpinned) =>
        new(name, "default", "InvalidArtifact", reason, code, "-", position, Unnamed, Unnamed, bytes);

    /// <summary>
    /// An invalid entry whose position is DERIVED, held against the hand-computed answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The producer refuses to write a corpus whose two answers disagree.</b> The derivation
    /// reads the section list and the position table this builder assembled; the hand-computed
    /// string was written by a human against the format document. Neither is taken as the
    /// authority over the other - they are compared, and a difference stops the write.
    /// </para>
    /// <para>
    /// Keeping the literal is the point. A derivation that replaced it would be a second
    /// implementation with nothing to check it, and the reason bundle JS-3A-001 gave for not
    /// pinning more rows - that a hand-computed offset is a number no reader can check - is only
    /// answered if the derivation reproduces the numbers a reader already checked.
    /// </para>
    /// </remarks>
    private static CorpusEntry Pinned(
        string name,
        (byte[] Bytes, string Pin) built,
        string reason,
        int code,
        string handComputed)
    {
        if (!string.Equals(built.Pin, handComputed, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"The corpus entry {name} derives the position {built.Pin} from what the builder " +
                $"wrote and its hand-computed position is {handComputed}. One of the two is " +
                "wrong and this producer will not write a corpus until they agree.");
        }

        return Invalid(name, built.Bytes, reason, code, built.Pin);
    }

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
    /// The position a refusal should report, DERIVED from what this builder wrote.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Bundle JS-3A-001 recorded that only four rows pin a position, and gave two reasons that
    /// are both true and not exhaustive.</b> Hand-computing an offset into bytes the producer
    /// assembles gives a number no reader can check; asking the verifier for it records the answer
    /// under test. This is the third way: the producer computes the position from ITS OWN
    /// construction - the section it put the defect in, the offset it put it at, and the position
    /// table it wrote - and the replay compares that against what the verifier computed from what
    /// it READ.
    /// </para>
    /// <para>
    /// <b>Two derivations of one fact, and they can disagree.</b> That is the shape rule K2 uses
    /// for a composition's profile set and rule J8 uses for the report's own figures, and it is
    /// what makes this a comparison rather than a recording. What it does NOT catch is a shared
    /// misunderstanding of the encoding: if both sides read the covering row wrongly in the same
    /// way, both agree. The four hand-computed pins stay exactly as they are for that reason -
    /// they were written by a human against the format document, and the first thing this
    /// derivation had to do was reproduce them.
    /// </para>
    /// </remarks>
    private static string PinnedAt(
        IReadOnlyList<JavaScriptArtifactWriter.Section> sections,
        JavaScriptFormat.SectionKind kind,
        uint byteOffset,
        IReadOnlyList<(uint Offset, uint Line, uint Column)> positions)
    {
        var sectionIndex = -1;

        for (var index = 0; index < sections.Count; index++)
        {
            if (sections[index].Kind == kind)
            {
                sectionIndex = index;
                break;
            }
        }

        if (sectionIndex < 0)
        {
            throw new InvalidOperationException(
                $"This artifact carries no {kind} section, so a refusal cannot be pinned inside one.");
        }

        // The covering row is the LAST row at or before the offset, which is the encoding JSD-0009
        // records. A refusal before the first row, or in an artifact with no table, reports the
        // reserved zero pair rather than guessing.
        uint line = 0;
        uint column = 0;

        foreach (var row in positions)
        {
            if (row.Offset <= byteOffset)
            {
                line = row.Line;
                column = row.Column;
            }
        }

        return string.Join(':', sectionIndex, byteOffset, line, column);
    }

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
    /// <summary>
    /// The entry whose point is the POSITION rather than the code, with its pin derived.
    /// </summary>
    /// <remarks>
    /// The unknown opcode's offset is FOUND in the code bytes rather than restated beside them,
    /// so moving the byte moves the pin and no third place has to be remembered.
    /// </remarks>
    private static (byte[] Bytes, string Pin) RefusalUnderASecondPositionRow()
    {
        (uint Offset, uint Line, uint Column)[] positions = [(0u, 1u, 1u), (3u, 7u, 5u)];
        byte[] code = [(byte)JavaScriptOpcode.LoadConstant, 0x00, 0x00, 0xFF];

        JavaScriptArtifactWriter.Section[] sections =
        [
            Limits(),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Constants,
                JavaScriptArtifactWriter.Constants([JavaScriptArtifactWriter.NumberConstant(1)])),
            new JavaScriptArtifactWriter.Section(JavaScriptFormat.SectionKind.Code, code),
            Entries([(SliceLowering.MainEntry, 0u)]),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Positions,
                JavaScriptArtifactWriter.Positions(positions)),
        ];

        return (
            JavaScriptArtifactWriter.Write(Manifest, sections),
            PinnedAt(
                sections,
                JavaScriptFormat.SectionKind.Code,
                (uint)Array.IndexOf(code, (byte)0xFF),
                positions));
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
    /// <summary>
    /// A code section carrying one unknown opcode, with its pin derived.
    /// </summary>
    /// <remarks>
    /// The offset is FOUND in the bytes rather than stated beside them, and the empty position
    /// table is passed explicitly: an artifact with no table reports the reserved zero pair, which
    /// JSD-0009 says is what "not known" means and is not the same as a table the derivation
    /// forgot to look at.
    /// </remarks>
    private static (byte[] Bytes, string Pin) UnknownOpcodeInCode()
    {
        byte[] code = [0xFF];

        JavaScriptArtifactWriter.Section[] sections =
        [
            Limits(),
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Constants,
                JavaScriptArtifactWriter.Constants([JavaScriptArtifactWriter.NumberConstant(1)])),
            new JavaScriptArtifactWriter.Section(JavaScriptFormat.SectionKind.Code, code),
            Entries([(SliceLowering.MainEntry, 0u)]),
        ];

        return (
            JavaScriptArtifactWriter.Write(Manifest, sections),
            PinnedAt(
                sections,
                JavaScriptFormat.SectionKind.Code,
                (uint)Array.IndexOf(code, (byte)0xFF),
                []));
    }

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

    /// <summary>A position row pointing into an operand, with its pin derived.</summary>
    /// <remarks>
    /// The refused row is the row that covers the offset, so this artifact reports its own line
    /// and column - which is why the offset the pin is taken at is the row's own offset.
    /// </remarks>
    private static (byte[] Bytes, string Pin) PositionIntoOperand()
    {
        (uint Offset, uint Line, uint Column)[] positions = [(1u, 1u, 1u)];
        var standard = StandardSections();

        JavaScriptArtifactWriter.Section[] sections =
        [
            standard[0], standard[1], standard[2], standard[3],
            new JavaScriptArtifactWriter.Section(
                JavaScriptFormat.SectionKind.Positions,
                JavaScriptArtifactWriter.Positions(positions)),
        ];

        return (
            JavaScriptArtifactWriter.Write(Manifest, sections),
            PinnedAt(
                sections,
                JavaScriptFormat.SectionKind.Code,
                positions[0].Offset,
                positions));
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
    /// <summary>The artifact descriptor's format version disagrees with the payload's.</summary>
    internal const int DescriptorFormatVersionMismatch = 1003;

    /// <summary>The artifact descriptor's manifest disagrees with the payload's.</summary>
    internal const int DescriptorManifestMismatch = 1006;

    /// <summary>A code-unit row states a figure format version 2 cannot represent.</summary>
    internal const int MalformedFunctionRow = 1601;

    /// <summary>An index names a code unit the functions section does not declare.</summary>
    internal const int FunctionIndexOutOfRange = 1602;

    /// <summary>A code unit's declared range is not the next disjoint run of the code section.</summary>
    internal const int CodeUnitRangeInvalid = 1603;

    /// <summary>A scope depth is past what the encoding or the structure allows.</summary>
    internal const int ScopeDepthOutOfRange = 1604;

    /// <summary>An exception region states a range, a handler or a kind this format refuses.</summary>
    internal const int MalformedExceptionRegion = 1605;

    /// <summary>Module records under a manifest that does not admit them.</summary>
    internal const int ModuleSectionOutsideManifest = 1613;

    /// <summary>A module record states an index, a count or a slot this format refuses.</summary>
    internal const int MalformedModuleRow = 1614;

    /// <summary>A module requests a key no module of the artifact carries.</summary>
    internal const int ModuleRequestUnresolved = 1615;

    /// <summary>An import or a re-export names an export the exporting module does not have.</summary>
    internal const int ModuleExportNotFound = 1616;

    /// <summary>Two star re-exports supply one name from different bindings.</summary>
    internal const int ModuleExportAmbiguous = 1617;

    /// <summary>Resolving an export re-entered the module and name it started from.</summary>
    internal const int ModuleExportCircular = 1618;

    /// <summary>The composition registered no module resolver and so declined the surface.</summary>
    internal const int ModuleResolverAbsent = 1619;

    /// <summary>The artifact names the module manifest and declares no module records.</summary>
    internal const int ModuleSectionMissing = 1620;

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
