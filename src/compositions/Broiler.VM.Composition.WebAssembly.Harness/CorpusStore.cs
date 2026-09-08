using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The retained corpus: what every entry is, and the encoder that turns each one into bytes.
/// </summary>
/// <remarks>
/// <para>
/// <b>An entry is five things and not three.</b> The bytes, the outcome, the reason and the
/// diagnostic code are the four an embedder can read back; the fifth is a hand-written invariant
/// naming the family the entry belongs to, and it is the half that survives a regeneration. A
/// corpus of triples alone records what this build answered - regenerate it against a broken build
/// and it records the break as the expectation. The invariant is written by a person from the
/// format, is never regenerated, and is what the replay holds a row to when the triple was
/// recorded rather than derived.
/// </para>
/// <para>
/// <b>Two provenances, and the difference is stated in the manifest rather than implied.</b> A
/// <c>derived</c> row's triple was written by hand before the profile was asked, and the writer
/// REFUSES TO EMIT A MANIFEST when the profile disagrees with one - the declaration wins and the
/// run stops. A <c>recorded</c> row's triple came from the profile at the moment the corpus was
/// written, which detects a change and proves no correctness: the sweeps are ninety-nine
/// truncations and ninety-nine inversions of one module and nobody hand-derives a hundred and
/// ninety-eight diagnostic codes without inventing most of them. What holds a recorded row is its
/// invariant, and what a recorded row is worth is stated here rather than left to a reader.
/// </para>
/// <para>
/// <b>What is NOT here.</b> There is no entry produced by a fuzzer, no entry minimised from a
/// counterexample, and no entry drawn from the specification's own test suite - this corpus is
/// hand-written, so it cannot find a refusal nobody thought of. There is no replay under a second
/// publish mode, because this root publishes one. And no row records an execution answer: every
/// entry stops at verification, and the interpreter is scored by the differential lane instead,
/// where the expected values come from somewhere other than the interpreter.
/// </para>
/// <para>
/// <b>It lives in a composition root because rule A11 leaves nowhere else.</b> An encoder that
/// produces the bytes a verifier is asked to accept has to name the profile assembly, and a project
/// outside <c>src/compositions/</c> may not.
/// </para>
/// </remarks>
internal static class CorpusStore
{
    /// <summary>The extension every retained entry carries.</summary>
    internal const string Extension = ".wasm";

    /// <summary>The manifest file the writer emits and the replay reads.</summary>
    internal const string ManifestFileName = "corpus.manifest";

    // =============================================================================================
    // The invariant vocabulary. Six words, each one a claim a person made about a family.
    // =============================================================================================

    /// <summary>The module must verify. A refusal of any kind fails the row.</summary>
    internal const string Accepts = "accepts";

    /// <summary>
    /// The module must be refused by the DECODER, which this profile makes observable by numbering
    /// its decode diagnostics below 2700 and its validation diagnostics above.
    /// </summary>
    internal const string RefusesDecoding = "refuses-decoding";

    /// <summary>The module must decode cleanly and be refused by the VALIDATOR.</summary>
    internal const string RefusesValidation = "refuses-validation";

    /// <summary>The module must be answered with a resource exhaustion naming a dimension.</summary>
    internal const string Exhausts = "exhausts";

    /// <summary>
    /// The module must be refused, and the phase is not pinned. Used where a mutation lands in a
    /// place whose phase is a fact about the module rather than about the family.
    /// </summary>
    internal const string Refuses = "refuses";

    /// <summary>
    /// The module may be accepted or refused, and either answer is sound - but a refusal may not be
    /// the verifier reporting its own defect, and may not be a code no published enumeration holds.
    /// This is the sweeps' invariant, and it is the strongest one that is true of them.
    /// </summary>
    internal const string SoundEitherWay = "sound-either-way";

    // =============================================================================================
    // The entry
    // =============================================================================================

    /// <summary>
    /// One corpus entry: the bytes, the family it belongs to, the invariant it must satisfy, and -
    /// where a person derived it - the triple it must produce.
    /// </summary>
    /// <remarks>
    /// A null <see cref="Outcome"/> is what makes a row <c>recorded</c>. It is a null and not a
    /// sentinel value because a sentinel would be a triple, and a triple nobody wrote is the exact
    /// thing this record exists to keep out of the derived population.
    /// </remarks>
    internal sealed record CorpusEntry(
        string Name,
        string Family,
        byte[] Bytes,
        string Invariant,
        VmOutcome? Outcome = null,
        VmReason? Reason = null,
        WebAssemblyDiagnosticCode Code = 0,
        VmBudgetDimension? Dimension = null,
        VmBudgetScope? Scope = null);

    /// <summary>Every entry, in the order the manifest writes them.</summary>
    internal static List<CorpusEntry> Entries()
    {
        var entries = new List<CorpusEntry>();
        entries.AddRange(Controls());
        entries.AddRange(Preambles());
        entries.AddRange(Varints());
        entries.AddRange(SectionFraming());
        entries.AddRange(Names());
        entries.AddRange(TypesAndLimits());
        entries.AddRange(FeatureRefusals());
        entries.AddRange(ConstantExpressions());
        entries.AddRange(Ceilings());
        entries.AddRange(ValidationControl());
        entries.AddRange(ValidationTypes());
        entries.AddRange(ValidationIndices());
        entries.AddRange(ValidationModule());
        entries.AddRange(TruncationSweep());
        entries.AddRange(InversionSweep());
        return entries;
    }

    // =============================================================================================
    // Controls: the modules that must verify. A corpus in which nothing passes would not notice a
    // verifier that refuses everything, and every refusing family below is only meaningful beside
    // one of these.
    // =============================================================================================

    private static IEnumerable<CorpusEntry> Controls() =>
    [
        new("control-preamble-only", "control", [.. Preamble], Accepts,
            VmOutcome.Normal, VmReason.NormalCompleted),

        // THE PINNED FINDING. The function section's length is written in the padded five-byte form
        // a single-pass compiler emits, which the core's canonical variable-length readers reject
        // and which this profile's own readers must accept. It is the reason this profile wrote its
        // own LEB128 layer at all, and it is retained as bytes so that a regression back onto the
        // core's readers turns a corpus row from a pass into a failure rather than turning up as a
        // report from somebody's toolchain months later.
        new("control-canonical-module-with-a-padded-section-length", "control",
            CanonicalModule(), Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // Two custom sections and nothing else: unordered, repeatable, and read past without a byte
        // of either body being looked at.
        new("control-two-custom-sections", "control",
            [.. Preamble, .. Section(0, [0x01, 0x61, 0xFF]), .. Section(0, [0x01, 0x62, 0xFF])],
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // (func (param i32 i32) (result i32) local.get 0 local.get 1 i32.add)
        new("control-a-function-that-adds-its-two-parameters", "control",
            FunctionModule([I32, I32], [I32], [0x20, 0x00, 0x20, 0x01, 0x6A]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // UNREACHABLE CODE IS TYPE-CHECKED POLYMORPHICALLY AND NOT SKIPPED. After `unreachable` the
        // frame's stack is truncated and the frame is marked unreachable, so `i32.add` pops two
        // operands that are not there, is answered with the bottom type twice, and pushes the i32
        // the function's result type then consumes. A validator that treated those pops as an
        // underflow would refuse this module, and that is the half of the pair this entry catches -
        // the other half is `validation-an-operand-stack-underflow-in-reachable-code`, which is the
        // same instruction in a frame that is not unreachable.
        new("control-unreachable-code-that-could-not-type-if-it-were-reachable", "control",
            FunctionModule([], [I32], [0x00, 0x6A]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // A second unreachable-code case, and a different mechanism: the branch makes the rest of
        // the block unreachable, so `i32.add` pops one operand that is there and one that is not.
        //
        // CORRECTED 2026-09-07. This entry was first written as `(block (br 0) (i64.const 1))` on
        // the belief that unreachable code is unconstrained. It is not: `br` truncates the frame's
        // stack to the height the frame was entered at and marks it unreachable, and `end` still
        // requires the height to be back where it started - so a constant pushed after the branch
        // and never consumed leaves the block one operand tall and is refused with
        // BlockLeavesExtraOperands. The profile said so, this file said otherwise, and the profile
        // was right. What the entry does now is push the constant, consume it together with an
        // operand the polymorphic stack supplies, and drop the result - which is the property that
        // was meant all along.
        new("control-unreachable-code-after-an-unconditional-branch", "control",
            FunctionModule([], [], [0x02, 0x40, 0x0C, 0x00, 0x41, 0x01, 0x6A, 0x1A, 0x0B]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // A third: `return` also marks the frame unreachable, and what follows is polymorphic.
        new("control-unreachable-code-after-a-return", "control",
            FunctionModule([], [I32], [0x41, 0x01, 0x0F, 0x45, 0x45, 0x45]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // (func (block (loop (br 1))))
        new("control-a-block-a-loop-and-a-branch-out-of-both", "control",
            FunctionModule([], [], [0x02, 0x40, 0x03, 0x40, 0x0C, 0x01, 0x0B, 0x0B]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // (func (result i32) i32.const 1 (if (result i32) (then i32.const 10) (else i32.const 20)))
        new("control-a-conditional-with-both-arms-producing-a-result", "control",
            FunctionModule([], [I32], [0x41, 0x01, 0x04, 0x7F, 0x41, 0x0A, 0x05, 0x41, 0x14, 0x0B]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),
    ];

    // =============================================================================================
    // The preamble: the eight bytes before any section exists
    // =============================================================================================

    private static IEnumerable<CorpusEntry> Preambles() =>
    [
        new("preamble-wrong-magic", "preamble",
            [0x00, 0x61, 0x73, 0x6E, 0x01, 0x00, 0x00, 0x00], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.WrongMagic),

        new("preamble-wrong-binary-version", "preamble",
            [0x00, 0x61, 0x73, 0x6D, 0x02, 0x00, 0x00, 0x00], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.UnknownFormatVersion,
            WebAssemblyDiagnosticCode.UnsupportedBinaryVersion),

        new("preamble-truncated", "preamble",
            [0x00, 0x61, 0x73, 0x6D, 0x01], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.Truncated,
            WebAssemblyDiagnosticCode.Truncated),
    ];

    // =============================================================================================
    // Variable-length integers: the layer this profile wrote for itself
    // =============================================================================================

    private static IEnumerable<CorpusEntry> Varints() =>
    [
        // Six bytes where five are the budget: the encoding runs past what a 32-bit integer allows,
        // and the refusal is about the LENGTH of the encoding.
        new("varint-section-length-over-long", "varint",
            [.. Preamble, 0x01, 0x82, 0x80, 0x80, 0x80, 0x80, 0x00], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.IntegerRepresentationTooLong),

        // Five bytes, INSIDE the budget, but the terminal byte sets bits a 32-bit value does not
        // have. It is a different malformation from the one above and carries a different code on
        // purpose: a decoder that answered one code for both would be telling a caller that a
        // truncatable stream and an out-of-range value are the same problem.
        new("varint-section-length-terminal-byte-has-unused-bits-set", "varint",
            [.. Preamble, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.IntegerTooLarge),

        // The same pair one level down, inside a section body rather than in its length: the type
        // count of a type section, over-long.
        new("varint-a-vector-count-over-long", "varint",
            [.. Preamble, .. Section(1, [0x82, 0x80, 0x80, 0x80, 0x80, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.IntegerRepresentationTooLong),

        // A padded count INSIDE a body, in the accepted direction: five bytes encoding one. The
        // entry above and this one differ by whether the padding stays inside the 32-bit budget,
        // which is the whole distinction this profile's reader exists to make.
        new("varint-a-padded-vector-count-inside-a-body-is-accepted", "varint",
            [.. Preamble, .. Section(1, [0x81, 0x80, 0x80, 0x80, 0x00, 0x60, 0x00, 0x00])],
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // A padded SIGNED immediate. The signed reader is a separate function with a separate
        // terminal-byte rule - sign extension rather than zero extension - and a corpus that pinned
        // only the unsigned one would leave half the layer unpinned.
        new("varint-a-padded-signed-constant-immediate-is-accepted", "varint",
            FunctionModule([], [I32], [0x41, 0xC1, 0xFF, 0xFF, 0xFF, 0x7F]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // The signed reader's own over-long case: six bytes for a 32-bit signed immediate.
        new("varint-a-signed-constant-immediate-over-long", "varint",
            FunctionModule([], [I32], [0x41, 0x80, 0x80, 0x80, 0x80, 0x80, 0x00]), RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.IntegerRepresentationTooLong),

        // And its terminal-byte case: five bytes, in budget, whose last byte is neither an all-ones
        // nor an all-zeroes sign extension of bit 31.
        //
        // CORRECTED 2026-09-07. The byte here was 0x02 and 0x02 IS VALID: a fifth byte contributes
        // bits 28 to 34, so its low four bits are ordinary value bits and only bits 4, 5 and 6 have
        // to repeat the sign at bit 31. 0x02 sets bit 29 and encodes 2^29, which the profile
        // accepted and this file called malformed. 0x10 sets bit 32 while bit 31 is clear, which is
        // the sign extension that cannot be one.
        new("varint-a-signed-constant-immediate-with-an-invalid-sign-extension", "varint",
            FunctionModule([], [I32], [0x41, 0x80, 0x80, 0x80, 0x80, 0x10]), RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.IntegerSignExtensionInvalid),
    ];

    // =============================================================================================
    // Section framing: identity, order, repetition and declared length
    // =============================================================================================

    private static IEnumerable<CorpusEntry> SectionFraming() =>
    [
        new("section-unknown-identifier", "section",
            [.. Preamble, 0x0E, 0x00], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.UnknownSectionId),

        new("section-out-of-canonical-order", "section",
            [.. Preamble, .. Section(3, [0x00]), .. Section(1, [0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.SectionOutOfOrder),

        // Adjacent identifiers, which is the pair a decoder comparing the wrong way round would
        // still accept: the entry above skips five identifiers and this one skips none.
        new("section-out-of-order-by-one-identifier", "section",
            [.. Preamble, .. Section(5, [0x00]), .. Section(4, [0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.SectionOutOfOrder),

        new("section-duplicated", "section",
            [.. Preamble, .. Section(1, [0x00]), .. Section(1, [0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.DuplicateSection),

        // A duplicate that is NOT adjacent, so an implementation remembering only the previous
        // identifier would answer SectionOutOfOrder or accept it.
        new("section-duplicated-with-another-section-between", "section",
            [.. Preamble, .. Section(1, [0x00]), .. Section(3, [0x00]), .. Section(1, [0x00])],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.DuplicateSection),

        new("section-body-shorter-than-its-declared-length", "section",
            [.. Preamble, 0x01, 0x08, 0x01, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.SectionLengthMismatch),

        // The other direction: the body is longer than the length declared for it, so the reader
        // finishes the section with bytes of it unread.
        new("section-body-longer-than-its-declared-length", "section",
            [.. Preamble, 0x01, 0x01, 0x01, 0x60, 0x00, 0x00], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.SectionLengthMismatch),

        // A declared length that runs off the end of the artifact.
        new("section-declares-a-length-past-the-end-of-the-artifact", "section",
            [.. Preamble, 0x01, 0x7F, 0x01, 0x60, 0x00, 0x00], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.Truncated,
            WebAssemblyDiagnosticCode.Truncated),
    ];

    // =============================================================================================
    // Names. THE PLATFORM'S OWN DECODER WOULD HAVE ACCEPTED THE FIRST FIVE OF THESE.
    //
    // System.Text.Encoding.UTF8 is constructed with throwOnInvalidBytes false, so
    // Encoding.UTF8.GetString replaces every sequence below with U+FFFD and answers a string - it
    // does not fail. A profile that decoded names that way would accept modules the format calls
    // malformed and would hand two different modules the same name. That is why this profile has
    // its own scalar-value decoder and why these entries exist as bytes.
    // =============================================================================================

    private static IEnumerable<CorpusEntry> Names() =>
    [
        // C0 AF is the overlong two-byte form of '/', which has a one-byte form. UTF8.GetString
        // replaces it; the format calls the name malformed.
        new("name-overlong-two-byte-form", "name",
            [.. Preamble, .. Section(7, [0x01, 0x02, 0xC0, 0xAF, 0x00, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedNameEncoding),

        // ED A0 80 encodes U+D800, which is a surrogate and not a scalar value. UTF8.GetString
        // replaces it.
        new("name-encodes-a-surrogate", "name",
            [.. Preamble, .. Section(7, [0x01, 0x03, 0xED, 0xA0, 0x80, 0x00, 0x00])],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedNameEncoding),

        // F4 90 80 80 names U+110000, one past the last code point there is. UTF8.GetString
        // replaces it.
        new("name-encodes-a-value-above-the-last-code-point", "name",
            [.. Preamble, .. Section(7, [0x01, 0x04, 0xF4, 0x90, 0x80, 0x80, 0x00, 0x00])],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedNameEncoding),

        // A lead byte announcing three bytes with only two present before the name ends. The
        // sequence is cut by the NAME's declared length and not by the artifact's end, so a decoder
        // reading past the name would find the export kind byte and take it for a continuation.
        new("name-truncated-multi-byte-sequence-at-the-names-end", "name",
            [.. Preamble, .. Section(7, [0x01, 0x02, 0xE2, 0x82, 0x00, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedNameEncoding),

        // A continuation byte with no lead byte before it.
        new("name-a-lone-continuation-byte", "name",
            [.. Preamble, .. Section(7, [0x01, 0x01, 0x80, 0x00, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedNameEncoding),

        // F8 is a five-byte lead byte in the obsolete form of the encoding. No revision of UTF-8
        // has admitted it since 2003.
        new("name-a-five-byte-lead-byte", "name",
            [.. Preamble, .. Section(7, [0x01, 0x05, 0xF8, 0x88, 0x80, 0x80, 0x80, 0x00, 0x00])],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedNameEncoding),

        // The control: a name that is four bytes of one astral scalar value. It must be ACCEPTED,
        // because a decoder that refused everything above three bytes would pass every entry above.
        new("name-a-four-byte-scalar-value-is-accepted", "name",
            [.. Preamble,
             .. Section(1, [0x01, 0x60, 0x00, 0x00]),
             .. Section(3, [0x01, 0x00]),
             .. Section(7, [0x01, 0x04, 0xF0, 0x9F, 0x92, 0xA9, 0x00, 0x00]),
             .. Section(10, [0x01, 0x02, 0x00, 0x0B])],
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // A name of no bytes at all, which the format admits: the empty string is a name.
        new("name-of-no-bytes-is-accepted", "name",
            [.. Preamble,
             .. Section(1, [0x01, 0x60, 0x00, 0x00]),
             .. Section(3, [0x01, 0x00]),
             .. Section(7, [0x01, 0x00, 0x00, 0x00]),
             .. Section(10, [0x01, 0x02, 0x00, 0x0B])],
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        new("name-export-kind-byte-names-nothing", "name",
            [.. Preamble, .. Section(7, [0x01, 0x01, 0x61, 0x09, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.UnknownExternalKind),
    ];

    // =============================================================================================
    // Types and limits
    // =============================================================================================

    private static IEnumerable<CorpusEntry> TypesAndLimits() =>
    [
        new("type-function-tag-is-not-0x60", "type",
            [.. Preamble, .. Section(1, [0x01, 0x61, 0x00, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedFunctionTypeTag),

        new("type-value-type-byte-names-nothing", "type",
            [.. Preamble, .. Section(1, [0x01, 0x60, 0x01, 0x55, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.UnknownValueType),

        new("limits-flag-is-neither-form", "limits",
            [.. Preamble, .. Section(5, [0x01, 0x07, 0x01])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedLimitsFlag),

        new("limits-minimum-above-maximum", "limits",
            [.. Preamble, .. Section(5, [0x01, 0x01, 0x05, 0x02])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.LimitsMinimumAboveMaximum),

        // 65,537 pages: one past the 65,536 the format itself caps a memory at, which is a
        // different fact from a host ceiling and gets a decode code rather than an exhaustion.
        new("limits-memory-minimum-above-the-formats-own-maximum", "limits",
            [.. Preamble, .. Section(5, [0x01, 0x00, 0x81, 0x80, 0x04])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.MemoryPagesAboveFormatMaximum),

        new("type-table-element-type-is-not-funcref", "type",
            [.. Preamble, .. Section(4, [0x01, 0x6F, 0x00, 0x01])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.UnknownElementType),

        new("type-global-mutability-flag-is-neither-form", "type",
            [.. Preamble, .. Section(6, [0x01, 0x7F, 0x02, 0x41, 0x00, 0x0B])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.MalformedMutabilityFlag),
    ];

    // =============================================================================================
    // Features this manifest does not admit. Each one is a WELL-FORMED encoding of something a
    // later surface defines, which is a different fact from a byte naming nothing, and the two
    // carry different reasons so that an embedder can tell "your module is broken" from "this build
    // does not do that yet".
    // =============================================================================================

    private static IEnumerable<CorpusEntry> FeatureRefusals() =>
    [
        new("feature-a-vector-value-type", "feature",
            [.. Preamble, .. Section(1, [0x01, 0x60, 0x01, 0x7B, 0x00])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.ValueTypeNotAdmitted),

        new("feature-an-import", "feature",
            [.. Preamble, .. Section(2, [0x01, 0x01, 0x6D, 0x01, 0x66, 0x00, 0x00])],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.ImportNotAdmitted),

        new("feature-two-memories", "feature",
            [.. Preamble, .. Section(5, [0x02, 0x00, 0x01, 0x00, 0x01])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.MultipleMemoriesNotAdmitted),

        new("feature-two-tables", "feature",
            [.. Preamble, .. Section(4, [0x02, 0x70, 0x00, 0x01, 0x70, 0x00, 0x01])],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.MultipleTablesNotAdmitted),

        // 0xFC is the bulk-memory prefix: a real instruction in a later surface, and one no manifest
        // here admits.
        new("feature-a-bulk-memory-prefixed-instruction", "feature",
            FunctionModule([], [], [0xFC]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.OpcodeNotAdmitted),

        // The multi-value block type: a non-negative signed 33-bit immediate naming a type index.
        new("feature-a-block-type-in-the-type-index-form", "feature",
            FunctionModule([], [], [0x02, 0x00, 0x0B]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.BlockTypeNotAdmitted),
    ];

    // =============================================================================================
    // Constant expressions
    // =============================================================================================

    private static IEnumerable<CorpusEntry> ConstantExpressions() =>
    [
        new("constant-expression-is-not-a-constant-instruction", "constant-expression",
            [.. Preamble, .. Section(6, [0x01, 0x7F, 0x00, 0x6A, 0x0B])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.UnknownFeature,
            WebAssemblyDiagnosticCode.UnsupportedConstantExpressionOpcode),

        new("constant-expression-is-not-terminated", "constant-expression",
            [.. Preamble, .. Section(6, [0x01, 0x7F, 0x00, 0x41, 0x00, 0x01])], RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.ConstantExpressionNotTerminated),

        new("constant-expression-of-another-type-than-the-global", "constant-expression",
            FunctionModule([], [], [], globalSection: [0x01, 0x7E, 0x00, 0x41, 0x01, 0x0B]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.ConstantExpressionTypeMismatch),

        // A constant expression may read an imported global and no other, and no manifest here
        // admits an import - so there is no global this expression could legally be reading.
        new("constant-expression-reading-a-global", "constant-expression",
            FunctionModule([], [], [], globalSection: [0x01, 0x7F, 0x00, 0x23, 0x00, 0x0B]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.ConstantExpressionGlobalUnavailable),
    ];

    // =============================================================================================
    // Ceilings. A ceiling answer carries NO diagnostic code - the dimension and the scope are the
    // only things that tell one exhaustion from another, which is the shape rule N11 fixes for the
    // JavaScript profile's corpus and the reason those two columns exist here.
    // =============================================================================================

    private static IEnumerable<CorpusEntry> Ceilings()
    {
        // The declared-count ceiling this profile defaults to is 4,194,304. 0xFFFFFFFF is three
        // orders of magnitude above it and is still a WELL-FORMED 32-bit encoding, so the answer is
        // an exhaustion naming the dimension and not a malformation: the count clears no ceiling,
        // so it never becomes a loop bound or a buffer size, and that ordering - bound before use -
        // is what the entry pins.
        yield return new("ceiling-a-vector-length-above-the-declared-count-ceiling", "ceiling",
            [.. Preamble, 0x01, 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F], Exhausts,
            VmOutcome.ResourceExhaustion, VmReason.CeilingReached, 0,
            VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact);

        // The same ceiling reached from a different section, so the row is not satisfied by one
        // guarded call site.
        yield return new("ceiling-a-code-section-count-above-the-declared-count-ceiling", "ceiling",
            [.. Preamble,
             .. Section(1, [0x01, 0x60, 0x00, 0x00]),
             .. Section(3, [0x01, 0x00]),
             // The code section's identifier, a six-byte body, and a count of 0xFFFFFFFF - written
             // out rather than framed, because the framing helper would have to be handed a body
             // this entry deliberately never supplies.
             0x0A, 0x06, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F, 0x00],
            Exhausts,
            VmOutcome.ResourceExhaustion, VmReason.CeilingReached, 0,
            VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact);

        // Structural depth. This profile defaults to 256 levels and the function's own frame is one
        // of them, so 255 nested blocks sit exactly at the ceiling and 256 are one past it. The two
        // are retained as a pair because a corpus holding only the refusing side would be satisfied
        // by a validator that refused at any depth at all.
        yield return new("ceiling-nesting-at-the-structural-depth-ceiling", "ceiling",
            NestedBlocks(255), Accepts, VmOutcome.Normal, VmReason.NormalCompleted);

        yield return new("ceiling-nesting-one-level-beyond-the-structural-depth-ceiling", "ceiling",
            NestedBlocks(256), Exhausts,
            VmOutcome.ResourceExhaustion, VmReason.CeilingReached, 0,
            VmBudgetDimension.StructuralDepth, VmBudgetScope.Artifact);

        // The section-count ceiling this profile defaults to is 64. Custom sections take no position
        // in the order and may repeat, so they are the only way to reach it without first breaking
        // the order rule - which makes this the entry that separates "too many sections" from
        // "sections in the wrong order".
        yield return new("ceiling-more-sections-than-the-section-count-ceiling", "ceiling",
            ManyCustomSections(65), Exhausts,
            VmOutcome.ResourceExhaustion, VmReason.CeilingReached, 0,
            VmBudgetDimension.SectionCount, VmBudgetScope.Artifact);
    }

    // =============================================================================================
    // Validation: control flow. Everything from here down DECODES CLEANLY and is refused by the
    // validator, which this profile makes observable by numbering validation diagnostics above
    // 2700. A module that is both malformed and invalid appears in a decode family and never here.
    // =============================================================================================

    private static IEnumerable<CorpusEntry> ValidationControl() =>
    [
        // The same instruction as the unreachable-code control, with nothing at all beneath it in a
        // frame that is NOT unreachable.
        new("validation-an-operand-stack-underflow-in-reachable-code", "validation-control",
            FunctionModule([], [], [0x6A]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.OperandStackUnderflow),

        // Only the function's own frame is open, so every depth but zero addresses no label.
        new("validation-a-branch-to-a-label-depth-that-does-not-exist", "validation-control",
            FunctionModule([], [], [0x0C, 0x05]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.BranchDepthOutOfRange),

        // Depth one where exactly one frame is open: the off-by-one neighbour of the entry above,
        // which a bound written with the wrong comparison would still accept.
        new("validation-a-branch-one-depth-past-the-last-open-label", "validation-control",
            FunctionModule([], [], [0x0C, 0x01]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.BranchDepthOutOfRange),

        // (block (result i32)) with nothing inside it: the block declares an i32 and leaves none.
        new("validation-a-block-whose-declared-result-type-is-unmet", "validation-control",
            FunctionModule([], [], [0x02, 0x7F, 0x0B]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.BlockResultTypeUnmet),

        // The opposite failure: the block declares nothing and leaves one operand behind.
        new("validation-a-block-that-leaves-an-operand-above-its-entry-height", "validation-control",
            FunctionModule([], [], [0x02, 0x40, 0x41, 0x01, 0x0B]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.BlockLeavesExtraOperands),

        new("validation-an-else-where-no-if-is-open", "validation-control",
            FunctionModule([], [], [0x02, 0x40, 0x05, 0x0B]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.ElseWithoutIf),

        // A conditional declaring a result and supplying no alternative arm: the arm that does not
        // exist would have had to produce the i32.
        new("validation-a-conditional-with-a-result-and-no-else-arm", "validation-control",
            FunctionModule([], [], [0x41, 0x01, 0x04, 0x7F, 0x41, 0x02, 0x0B]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.IfWithoutElseResultMismatch),

        // The body's own end closes the block, so the function's frame is still open at its last
        // byte.
        new("validation-a-function-body-that-ends-with-a-block-still-open", "validation-control",
            FunctionModule([], [], [0x02, 0x40]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.UnterminatedFunctionBody),

        // The first end closes the function; the byte after it belongs to no frame.
        new("validation-a-body-carrying-bytes-after-the-end-that-closed-it", "validation-control",
            FunctionModule([], [], [0x0B]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.InstructionsAfterFunctionEnd),

        // Two labels of one table with different arities: the inner block carries nothing and the
        // outer carries an i32, so there is no single stack height to branch to.
        new("validation-a-branch-table-whose-labels-carry-different-arities", "validation-control",
            FunctionModule([], [], [0x02, 0x7F, 0x02, 0x40, 0x41, 0x00, 0x0E, 0x01, 0x00, 0x01]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.BranchTableArityMismatch),

        // 0x06 names no instruction in any published surface. It decodes as far as the opcode byte
        // and is refused there, so it is a MALFORMED encoding rather than an invalid module - which
        // is why its reason differs from every other entry in this family.
        new("validation-a-byte-that-names-no-instruction", "validation-control",
            FunctionModule([], [], [0x06]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.UnknownOpcode),

        // memory.size carries a byte the format reserves as zero, and a later revision will give it
        // a meaning. Decoding it as this revision's would be decoding a different module.
        new("validation-a-reserved-immediate-that-is-not-zero", "validation-control",
            FunctionModule([], [], [0x3F, 0x01]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.MalformedEncoding,
            WebAssemblyDiagnosticCode.ReservedImmediateNotZero),
    ];

    // =============================================================================================
    // Validation: types
    // =============================================================================================

    private static IEnumerable<CorpusEntry> ValidationTypes() =>
    [
        // i32.add is handed an i64. The first pop it makes answers a concrete type that is not the
        // one it asked for, which is a mismatch and not an underflow, and the two carry their own
        // codes because a corpus that could not tell them apart would score both with one fixture.
        new("validation-a-type-mismatch-between-an-operand-and-its-instruction", "validation-type",
            FunctionModule([], [], [0x41, 0x01, 0x42, 0x02, 0x6A]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.OperandTypeMismatch),

        // The mismatch in the other operand position, so a checker that only tested the top of the
        // stack would still accept one of the pair.
        new("validation-a-type-mismatch-in-the-lower-operand-position", "validation-type",
            FunctionModule([], [], [0x42, 0x01, 0x41, 0x02, 0x6A]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.OperandTypeMismatch),

        // The function declares an i32 result and its body leaves an i64.
        //
        // CORRECTED 2026-09-07. This row expected OperandTypeMismatch and the profile answers
        // BlockResultTypeUnmet, and the profile is the one being consistent: A FUNCTION'S OWN FRAME
        // IS A BLOCK FRAME whose end types are the function's result types, so a body that leaves
        // the wrong type fails the same comparison a block does and gets the same code. The row is
        // retained with the code corrected rather than deleted, because it is the only entry
        // reaching that comparison through the outermost frame rather than through a nested one.
        new("validation-a-body-leaving-a-value-of-the-wrong-result-type", "validation-type",
            FunctionModule([], [I32], [0x42, 0x01]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.BlockResultTypeUnmet),

        // An i32 load declaring an eight-byte alignment. It is refused HERE and not at the access,
        // because an alignment above the natural alignment of the width is a validation error and
        // not a trap.
        new("validation-a-load-whose-alignment-is-above-its-natural-alignment", "validation-type",
            FunctionModule([], [], [0x41, 0x00, 0x28, 0x03, 0x00], memorySection: [0x01, 0x00, 0x01]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.AlignmentAboveNaturalAlignment),

        // The neighbouring case: an i32 load at exactly its natural alignment, which must be
        // ACCEPTED. Without it a validator refusing every alignment would pass the entry above.
        new("validation-a-load-at-its-natural-alignment-is-accepted", "validation-type",
            FunctionModule(
                [], [], [0x41, 0x00, 0x28, 0x02, 0x00, 0x1A], memorySection: [0x01, 0x00, 0x01]),
            Accepts, VmOutcome.Normal, VmReason.NormalCompleted),

        // A narrow load, whose natural alignment is one byte rather than the width of its result.
        new("validation-a-byte-load-declaring-a-two-byte-alignment", "validation-type",
            FunctionModule([], [], [0x41, 0x00, 0x2C, 0x01, 0x00], memorySection: [0x01, 0x00, 0x01]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.AlignmentAboveNaturalAlignment),
    ];

    // =============================================================================================
    // Validation: indices into the module's own spaces
    // =============================================================================================

    private static IEnumerable<CorpusEntry> ValidationIndices() =>
    [
        new("validation-a-local-index-past-the-parameters-and-locals", "validation-index",
            FunctionModule([], [], [0x20, 0x00]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.LocalIndexOutOfRange),

        new("validation-a-global-index-the-module-does-not-declare", "validation-index",
            FunctionModule([], [], [0x23, 0x00]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.GlobalIndexOutOfRange),

        new("validation-a-write-to-a-global-the-module-declared-immutable", "validation-index",
            FunctionModule(
                [], [], [0x41, 0x01, 0x24, 0x00],
                globalSection: [0x01, 0x7F, 0x00, 0x41, 0x2A, 0x0B]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.GlobalIsImmutable),

        new("validation-a-call-whose-index-addresses-no-function", "validation-index",
            FunctionModule([], [], [0x10, 0x07]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.FunctionIndexOutOfRange),

        new("validation-a-memory-instruction-with-no-memory-declared", "validation-index",
            FunctionModule([], [], [0x3F, 0x00]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.MemoryNotDeclared),

        new("validation-an-indirect-call-with-no-table-declared", "validation-index",
            FunctionModule([], [], [0x41, 0x00, 0x11, 0x00, 0x00]), RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.TableNotDeclared),

        new("validation-an-indirect-call-naming-no-declared-type", "validation-index",
            FunctionModule(
                [], [], [0x41, 0x00, 0x11, 0x07, 0x00], tableSection: [0x01, 0x70, 0x00, 0x01]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.TypeIndexOutOfRange),
    ];

    // =============================================================================================
    // Validation: the module's own cross-section agreements
    // =============================================================================================

    private static IEnumerable<CorpusEntry> ValidationModule() =>
    [
        new("module-a-function-section-without-a-code-section", "validation-module",
            [.. Preamble, .. Section(1, [0x01, 0x60, 0x00, 0x00]), .. Section(3, [0x01, 0x00])],
            RefusesDecoding,
            VmOutcome.InvalidArtifact, VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.FunctionAndCodeCountMismatch),

        new("module-a-function-whose-type-index-addresses-no-type", "validation-module",
            [.. Preamble,
             .. Section(1, [0x01, 0x60, 0x00, 0x00]),
             .. Section(3, [0x01, 0x03]),
             .. Section(10, [0x01, 0x02, 0x00, 0x0B])],
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.FunctionTypeIndexOutOfRange),

        new("module-a-start-section-naming-a-function-that-does-not-exist", "validation-module",
            [.. Preamble,
             .. Section(1, [0x01, 0x60, 0x00, 0x00]),
             .. Section(3, [0x01, 0x00]),
             .. Section(8, [0x05]),
             .. Section(10, [0x01, 0x02, 0x00, 0x0B])],
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.StartFunctionIndexOutOfRange),

        new("module-a-start-function-that-takes-a-parameter", "validation-module",
            [.. Preamble,
             .. Section(1, [0x01, 0x60, 0x01, 0x7F, 0x00]),
             .. Section(3, [0x01, 0x00]),
             .. Section(8, [0x00]),
             .. Section(10, [0x01, 0x02, 0x00, 0x0B])],
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.StartFunctionSignatureInvalid),

        new("module-two-exports-publishing-one-name", "validation-module",
            FunctionModule(
                [], [], [], exportSection: [0x02, 0x01, 0x61, 0x00, 0x00, 0x01, 0x61, 0x00, 0x00]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.DuplicateExportName),

        new("module-an-export-naming-an-index-its-kind-does-not-have", "validation-module",
            FunctionModule([], [], [], exportSection: [0x01, 0x01, 0x61, 0x00, 0x05]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.ExportIndexOutOfRange),

        new("module-an-element-segment-with-no-table-declared", "validation-module",
            FunctionModule([], [], [], elementSection: [0x01, 0x00, 0x41, 0x00, 0x0B, 0x01, 0x00]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.ElementSegmentTableIndexOutOfRange),

        new("module-an-element-segment-writing-an-unknown-function-index", "validation-module",
            FunctionModule(
                [], [], [],
                tableSection: [0x01, 0x70, 0x00, 0x01],
                elementSection: [0x01, 0x00, 0x41, 0x00, 0x0B, 0x01, 0x03]),
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.ElementSegmentFunctionIndexOutOfRange),

        new("module-a-data-segment-with-no-memory-declared", "validation-module",
            [.. Preamble,
             .. Section(1, [0x01, 0x60, 0x00, 0x00]),
             .. Section(3, [0x01, 0x00]),
             .. Section(10, [0x01, 0x02, 0x00, 0x0B]),
             .. Section(11, [0x01, 0x00, 0x41, 0x00, 0x0B, 0x01, 0x41])],
            RefusesValidation,
            VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed,
            WebAssemblyDiagnosticCode.DataSegmentMemoryIndexOutOfRange),
    ];

    // =============================================================================================
    // The sweeps. THESE ROWS ARE RECORDED AND NOT DERIVED, and what that is worth is stated at the
    // top of this file: a recorded triple detects a change between one regeneration and the next
    // and proves nothing about correctness. What holds them is the hand-written invariant beside
    // each family - a truncation must be REFUSED, and an inversion must be answered SOUNDLY,
    // meaning it may be accepted or refused but may never come back as the verifier reporting its
    // own defect and may never carry a code no enumeration publishes.
    // =============================================================================================

    /// <summary>
    /// The offsets of the canonical module at which a strict prefix is itself a whole module.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>CORRECTED 2026-09-07. "Every prefix of a module is refused" is FALSE and the sweep proved
    /// it.</b> The family's invariant was written as <c>refuses</c> for all ninety-nine offsets, and
    /// four of them verified: a prefix that stops exactly on a section boundary is not a truncated
    /// module, it is a SHORTER module, and if the sections it still carries agree with one another
    /// it is a valid one. The four are derived here rather than recorded, because the reasoning is
    /// short and is the whole content of the correction.
    /// </para>
    /// <para>
    /// <b>8</b> is the preamble alone, which is a module declaring nothing. <b>17</b> adds the type
    /// section, and a type nothing uses is admitted. The six boundaries between - 25, 31, 36, 44,
    /// 59 and 68 - all carry the function section without the code section that has to match it, so
    /// each is refused with FunctionAndCodeCountMismatch and none of them is an exception. <b>79</b>
    /// is the first boundary past the code section, so the function count and the body count agree
    /// again and the module is whole. <b>90</b> adds the data segment, which the memory section
    /// already declared room for. The last boundary, 99, is the module itself and is not a strict
    /// prefix.
    /// </para>
    /// </remarks>
    private static readonly int[] TruncationsThatAreWholeModules = [8, 17, 79, 90];

    /// <summary>
    /// Every strict prefix of the canonical module, one entry per offset.
    /// </summary>
    /// <remarks>
    /// <b>The sweep is what catches a reader that answers off the end of a buffer at exactly one
    /// offset</b>, which is the defect a hand-written truncation entry at three chosen offsets does
    /// not find. Ninety-five of the ninety-nine must be refused and four must verify, and both
    /// halves are derived: see the table above for why the four are what they are.
    /// </remarks>
    private static IEnumerable<CorpusEntry> TruncationSweep()
    {
        var module = CanonicalModule();

        for (var length = 0; length < module.Length; length++)
        {
            yield return TruncationsThatAreWholeModules.Contains(length)
                ? new(
                    $"truncation-{length:D4}",
                    "truncation-sweep",
                    module[..length],
                    Accepts,
                    VmOutcome.Normal,
                    VmReason.NormalCompleted)
                : new(
                    $"truncation-{length:D4}",
                    "truncation-sweep",
                    module[..length],
                    Refuses);
        }
    }

    /// <summary>
    /// The canonical module with one byte inverted, one entry per offset.
    /// </summary>
    /// <remarks>
    /// <b>Some of these are ACCEPTED and that is correct.</b> An inversion inside the custom
    /// section's payload, or inside the data segment's contents, produces a different module and not
    /// an invalid one - so the family's invariant cannot be "refuses" without being false. What is
    /// true of every one of them is that the answer is sound: an acceptance or a refusal carrying a
    /// published code, and never the verifier reporting its own defect.
    /// </remarks>
    private static IEnumerable<CorpusEntry> InversionSweep()
    {
        var module = CanonicalModule();

        for (var offset = 0; offset < module.Length; offset++)
        {
            var mutant = (byte[])module.Clone();
            mutant[offset] ^= 0xFF;

            yield return new(
                $"inversion-{offset:D4}",
                "inversion-sweep",
                mutant,
                SoundEitherWay);
        }
    }

    // =============================================================================================
    // The encoder
    // =============================================================================================

    /// <summary>The byte the format encodes an i32 with.</summary>
    internal const byte I32 = 0x7F;

    /// <summary>The eight bytes every module begins with: the magic and binary version 1.</summary>
    internal static readonly byte[] Preamble = [0x00, 0x61, 0x73, 0x6D, 0x01, 0x00, 0x00, 0x00];

    /// <summary>
    /// The module the corpus is built around: one type, one function, one table, one memory, one
    /// global, two exports, one element segment, one data segment, one body, and one custom section
    /// to skip.
    /// </summary>
    /// <remarks>
    /// The function section's length is written in the padded five-byte form rather than the
    /// canonical one-byte form. That is the whole point: the core's variable-length readers reject
    /// it, this profile's accept it, and a regression back onto the core's readers turns this
    /// module - the one both sweeps are cut from - from a pass into a failure at two hundred rows
    /// at once.
    /// </remarks>
    internal static byte[] CanonicalModule()
    {
        var module = new List<byte>();
        module.AddRange(Preamble);

        // (type (func (param i32 i32) (result i32)))
        module.AddRange(Section(1, [0x01, 0x60, 0x02, 0x7F, 0x7F, 0x01, 0x7F]));

        // One function of type 0, with its section length padded to five bytes.
        module.AddRange(PaddedSection(3, [0x01, 0x00]));

        // One funcref table, minimum one element.
        module.AddRange(Section(4, [0x01, 0x70, 0x00, 0x01]));

        // One memory, minimum one page.
        module.AddRange(Section(5, [0x01, 0x00, 0x01]));

        // One mutable i32 global initialised to 42.
        module.AddRange(Section(6, [0x01, 0x7F, 0x01, 0x41, 0x2A, 0x0B]));

        // (export "add" (func 0)) (export "mem" (memory 0))
        module.AddRange(Section(7,
            [0x02, 0x03, 0x61, 0x64, 0x64, 0x00, 0x00, 0x03, 0x6D, 0x65, 0x6D, 0x02, 0x00]));

        // One active element segment writing function 0 at table offset 0.
        module.AddRange(Section(9, [0x01, 0x00, 0x41, 0x00, 0x0B, 0x01, 0x00]));

        // (func (param i32 i32) (result i32) local.get 0 local.get 1 i32.add)
        module.AddRange(Section(10, [0x01, 0x07, 0x00, 0x20, 0x00, 0x20, 0x01, 0x6A, 0x0B]));

        // One active data segment writing "ABC" at memory offset 0.
        module.AddRange(Section(11, [0x01, 0x00, 0x41, 0x00, 0x0B, 0x03, 0x41, 0x42, 0x43]));

        // A custom section, which takes no position in the order and is read past unexamined.
        module.AddRange(Section(0, [0x04, 0x64, 0x65, 0x6D, 0x6F, 0xAA, 0xBB]));

        return [.. module];
    }

    /// <summary>A module whose one function body opens <paramref name="levels"/> nested blocks.</summary>
    private static byte[] NestedBlocks(int levels)
    {
        var code = new List<byte>();

        for (var level = 0; level < levels; level++)
        {
            code.Add(0x02);
            code.Add(0x40);
        }

        for (var level = 0; level < levels; level++)
        {
            code.Add(0x0B);
        }

        return FunctionModule([], [], [.. code]);
    }

    /// <summary>A module carrying <paramref name="count"/> custom sections and nothing else.</summary>
    private static byte[] ManyCustomSections(int count)
    {
        var module = new List<byte>();
        module.AddRange(Preamble);

        for (var index = 0; index < count; index++)
        {
            module.AddRange(Section(0, [0x01, (byte)('a' + (index % 26))]));
        }

        return [.. module];
    }

    /// <summary>
    /// Builds a module with one function type, one function of it, and one body - plus whatever
    /// other sections the entry needs, framed in the canonical order.
    /// </summary>
    /// <remarks>
    /// It exists so that a validation entry is one line of bytes rather than a whole module. Every
    /// section it writes is well formed and in order, so an entry built with it REACHES THE
    /// VALIDATOR: an entry that stopped at the decoder would be recording the wrong phase's answer,
    /// and the invariant column would catch it.
    /// </remarks>
    internal static byte[] FunctionModule(
        byte[] parameters,
        byte[] results,
        byte[] code,
        byte[]? tableSection = null,
        byte[]? memorySection = null,
        byte[]? globalSection = null,
        byte[]? exportSection = null,
        byte[]? elementSection = null)
    {
        var type = new List<byte> { 0x01, 0x60, (byte)parameters.Length };
        type.AddRange(parameters);
        type.Add((byte)results.Length);
        type.AddRange(results);

        var body = new List<byte> { 0x00 };
        body.AddRange(code);
        body.Add(0x0B);

        var bodies = new List<byte> { 0x01 };
        bodies.AddRange(Leb((uint)body.Count));
        bodies.AddRange(body);

        var module = new List<byte>();
        module.AddRange(Preamble);
        module.AddRange(Section(1, [.. type]));
        module.AddRange(Section(3, [0x01, 0x00]));

        if (tableSection is not null)
        {
            module.AddRange(Section(4, tableSection));
        }

        if (memorySection is not null)
        {
            module.AddRange(Section(5, memorySection));
        }

        if (globalSection is not null)
        {
            module.AddRange(Section(6, globalSection));
        }

        if (exportSection is not null)
        {
            module.AddRange(Section(7, exportSection));
        }

        if (elementSection is not null)
        {
            module.AddRange(Section(9, elementSection));
        }

        module.AddRange(Section(10, [.. bodies]));
        return [.. module];
    }

    /// <summary>The canonical unsigned variable-length encoding of one number.</summary>
    internal static byte[] Leb(uint value)
    {
        var encoded = new List<byte>();

        do
        {
            var current = (byte)(value & 0x7F);
            value >>= 7;
            encoded.Add(value != 0 ? (byte)(current | 0x80) : current);
        }
        while (value != 0);

        return [.. encoded];
    }

    /// <summary>Frames a section body with its identifier and its canonical length.</summary>
    internal static byte[] Section(byte identifier, byte[] body)
    {
        var framed = new List<byte> { identifier };
        framed.AddRange(Leb((uint)body.Length));
        framed.AddRange(body);
        return [.. framed];
    }

    /// <summary>
    /// Frames a section body with its identifier and its length written in the padded five-byte
    /// form a single-pass compiler emits.
    /// </summary>
    internal static byte[] PaddedSection(byte identifier, byte[] body)
    {
        var length = (uint)body.Length;

        var framed = new List<byte>
        {
            identifier,
            (byte)((length & 0x7F) | 0x80),
            (byte)(((length >> 7) & 0x7F) | 0x80),
            (byte)(((length >> 14) & 0x7F) | 0x80),
            (byte)(((length >> 21) & 0x7F) | 0x80),
            (byte)((length >> 28) & 0x0F),
        };

        framed.AddRange(body);
        return [.. framed];
    }
}
