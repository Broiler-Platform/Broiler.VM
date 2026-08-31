namespace Broiler.VM.Fixtures;

/// <summary>
/// How firmly a corpus entry pins the answer it expects.
/// </summary>
/// <remarks>
/// Two kinds, because two kinds of case exist and collapsing them would weaken both. A case built
/// to exercise one named defect knows exactly which reason it should produce, and pinning that
/// reason is the whole value of the case. A case drawn from a systematic sweep - truncate at every
/// offset, corrupt every byte - has no hand-computed answer that is worth more than the sweep
/// itself; what it can still assert is that the answer lies inside the closed set, that a failure
/// yields no handle, and that the answer has not moved since it was recorded.
/// </remarks>
public enum FixtureCorpusPinning
{
    /// <summary>The outcome, the reason and the profile diagnostic code are all expected exactly.</summary>
    Exact = 0,

    /// <summary>
    /// The answer is expected to lie in the closed set and to be the one the manifest recorded. The
    /// manifest value is an observation under version control, not a hand-computed expectation, and
    /// it is labelled as such wherever it is written.
    /// </summary>
    Recorded = 1,
}

/// <summary>Where a corpus entry came from.</summary>
public enum FixtureCorpusProvenance
{
    /// <summary>Declared in this file, and rewritten from it whenever the corpus is regenerated.</summary>
    Seeded = 0,

    /// <summary>
    /// A minimized regression retained from a fuzz run. The bytes on disk are the artefact; nothing
    /// regenerates them, and the generator never rewrites or removes one.
    /// </summary>
    Minimized = 1,
}

/// <summary>One retained malformed-input case: the bytes, how they are presented, and the answer.</summary>
public sealed class FixtureCorpusEntry
{
    internal FixtureCorpusEntry(
        string id,
        string family,
        byte[] bytes,
        string note,
        VmOutcome outcome,
        VmReason reason,
        int profileDiagnosticCode,
        VmBudgetDimension dimension,
        VmBudgetScope scope,
        bool namesDimension,
        FixtureCorpusPinning pinning,
        uint descriptorFormatVersion,
        ulong artifactBytesRequest)
    {
        Id = id;
        Family = family;
        Bytes = bytes;
        Note = note;
        Outcome = outcome;
        Reason = reason;
        ProfileDiagnosticCode = profileDiagnosticCode;
        Dimension = dimension;
        Scope = scope;
        NamesDimension = namesDimension;
        Pinning = pinning;
        DescriptorFormatVersion = descriptorFormatVersion;
        ArtifactBytesRequest = artifactBytesRequest;
    }

    /// <summary>The stable, file-safe identifier. It is also the file name stem.</summary>
    public string Id { get; }

    /// <summary>Which group of related cases this belongs to.</summary>
    public string Family { get; }

    /// <summary>The artifact bytes exactly as they are presented to verification.</summary>
    public byte[] Bytes { get; }

    /// <summary>What this case is for, in one line.</summary>
    public string Note { get; }

    /// <summary>The expected outcome category.</summary>
    public VmOutcome Outcome { get; }

    /// <summary>The expected reason, for an exactly pinned case.</summary>
    public VmReason Reason { get; }

    /// <summary>The expected profile diagnostic code, for an exactly pinned case.</summary>
    public int ProfileDiagnosticCode { get; }

    /// <summary>The expected exhausted dimension, where the case names one.</summary>
    public VmBudgetDimension Dimension { get; }

    /// <summary>The expected exhausted scope, where the case names one.</summary>
    public VmBudgetScope Scope { get; }

    /// <summary>Whether <see cref="Dimension"/> and <see cref="Scope"/> are part of the expectation.</summary>
    public bool NamesDimension { get; }

    /// <summary>How firmly the answer is pinned.</summary>
    public FixtureCorpusPinning Pinning { get; }

    /// <summary>Where the entry came from.</summary>
    public FixtureCorpusProvenance Provenance => FixtureCorpusProvenance.Seeded;

    /// <summary>The profile-format version the descriptor presenting these bytes declares.</summary>
    public uint DescriptorFormatVersion { get; }

    /// <summary>
    /// An artifact-requested <c>ArtifactBytes</c> tightening, or zero for a descriptor that requests
    /// nothing.
    /// </summary>
    /// <remarks>
    /// This is how the oversized case stays small on disk. An artifact request may only tighten, so
    /// asking for a ceiling below the payload's own length is a legitimate descriptor that must
    /// deterministically refuse its own bytes - and it exercises the same early ceiling check a
    /// megabyte-long file would, without committing a megabyte to the repository.
    /// </remarks>
    public ulong ArtifactBytesRequest { get; }
}

/// <summary>
/// The seeded half of the VM-2 malformed-input corpus: what the bytes are, and what each one must
/// answer.
/// </summary>
/// <remarks>
/// <para>
/// The expectation is declared here, beside the bytes, and never derived from a run. A baseline
/// read back out of the thing it checks agrees with any change, which would leave a corpus that
/// passes whatever the verifier does and proves nothing about what it should do.
/// </para>
/// <para>
/// Three valid artifacts are included deliberately. A corpus in which every entry fails cannot tell
/// a verifier that classifies correctly from one that rejects everything it is handed, and the
/// second passes a failure-only corpus perfectly.
/// </para>
/// </remarks>
public static class FixtureCorpus
{
    /// <summary>The canonical well-formed artifact the sweep families are derived from.</summary>
    /// <remarks>
    /// It is small on purpose: every one of its byte offsets becomes a truncation case and a
    /// corruption case, so its length is the size of two whole families.
    /// </remarks>
    public static byte[] Canonical() => FixtureArtifactWriter.Sum(20, 22);

    /// <summary>Every seeded entry, in a stable order.</summary>
    public static System.Collections.Generic.IReadOnlyList<FixtureCorpusEntry> Entries()
    {
        var entries = new System.Collections.Generic.List<FixtureCorpusEntry>();

        AddControls(entries);
        AddPrefixes(entries);
        AddMagic(entries);
        AddFormatVersion(entries);
        AddSectionFraming(entries);
        AddConstantPool(entries);
        AddCodeSection(entries);
        AddCeilings(entries);
        AddSweeps(entries);

        return entries;
    }

    // ---- the controls ---------------------------------------------------------------------------

    private static void AddControls(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        entries.Add(Normal(
            "control-sum", "control", Canonical(),
            "A well-formed artifact. A corpus in which everything fails cannot detect a verifier that rejects everything."));

        entries.Add(Normal(
            "control-constant", "control", FixtureArtifactWriter.Constant(7),
            "The smallest well-formed artifact that returns a value."));

        entries.Add(Normal(
            "control-empty-code", "control", Build(1, [Section(FixtureFormat.SectionConstants, ConstantBody([1])), Section(FixtureFormat.SectionCode, CodeBody([]))]),
            "Zero instructions is a well-formed program, not a malformed artifact. Rejecting it would be a verifier inventing a rule."));
    }

    // ---- prefixes too short to carry a header ----------------------------------------------------

    private static void AddPrefixes(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        entries.Add(Invalid(
            "prefix-empty", "prefix", [],
            VmReason.Truncated, 2001,
            "Zero bytes. The first read is bounded, so this is a truncation and never an index."));

        entries.Add(Invalid(
            "prefix-one-byte", "prefix", [0x42],
            VmReason.Truncated, 2001,
            "One byte, short of the four-byte magic."));

        entries.Add(Invalid(
            "prefix-magic-only", "prefix", [.. FixtureFormat.Magic],
            VmReason.Truncated, 2001,
            "The magic and nothing after it: the format version read runs off the end."));

        entries.Add(Invalid(
            "prefix-dangling-continuation", "prefix", [.. FixtureFormat.Magic, 0x80],
            VmReason.Truncated, 2001,
            "A variable-length integer whose continuation bit promises a byte the artifact does not contain."));
    }

    // ---- the magic ------------------------------------------------------------------------------

    private static void AddMagic(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        var canonical = Canonical();

        for (var index = 0; index < 4; index++)
        {
            var damaged = (byte[])canonical.Clone();
            damaged[index] ^= 0xFF;

            entries.Add(Invalid(
                $"magic-byte-{index}-inverted", "magic", damaged,
                VmReason.MalformedEncoding, 1001,
                $"Byte {index} of the magic inverted. A wrong magic is the profile's own rejection, not a truncation."));
        }

        var lowered = (byte[])canonical.Clone();

        for (var index = 0; index < 4; index++)
        {
            lowered[index] = (byte)(lowered[index] | 0x20);
        }

        entries.Add(Invalid(
            "magic-lowercased", "magic", lowered,
            VmReason.MalformedEncoding, 1001,
            "The magic in lower case. Matching is over the raw bytes, so it is not a near miss but a miss."));
    }

    // ---- the format version ---------------------------------------------------------------------

    private static void AddFormatVersion(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        foreach (var version in new uint[] { 0, 2, 99, 65_536, uint.MaxValue })
        {
            entries.Add(Invalid(
                $"format-version-{version}", "format-version",
                Build(version, [Section(FixtureFormat.SectionConstants, ConstantBody([1])), Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]))]),
                VmReason.UnknownFormatVersion, 1002,
                $"A payload declaring profile-format version {version}, which this profile does not know."));
        }

        entries.Add(Invalid(
            "format-version-non-canonical", "format-version",
            [.. FixtureFormat.Magic, 0x81, 0x00, 0x02],
            VmReason.MalformedEncoding, 2002,
            "Version 1 written with a redundant continuation group. Two encodings of one value would make a byte-identical artifact check meaningless."));

        entries.Add(Invalid(
            "descriptor-format-version-2", "format-version", Canonical(),
            VmReason.UnsupportedProfileFormatVersion, 0,
            "Well-formed bytes presented under a descriptor naming a profile-format version the catalog entry does not support. The core answers before the verifier is entered.",
            descriptorFormatVersion: 2));
    }

    // ---- section framing --------------------------------------------------------------------------

    private static void AddSectionFraming(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        entries.Add(Invalid(
            "sections-declared-none", "framing",
            Build(FixtureFormat.FormatVersion, []),
            VmReason.InconsistentStructure, 1005,
            "Zero sections declared, so neither a constant pool nor code is present."));

        entries.Add(Invalid(
            "sections-constants-only", "framing",
            Build(FixtureFormat.FormatVersion, [Section(FixtureFormat.SectionConstants, ConstantBody([1]))]),
            VmReason.InconsistentStructure, 1005,
            "A constant pool with no code section."));

        entries.Add(Invalid(
            "sections-code-only", "framing",
            Build(FixtureFormat.FormatVersion, [Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpReturn]))]),
            VmReason.InconsistentStructure, 1005,
            "Code with no constant pool."));

        entries.Add(Invalid(
            "sections-over-declared", "framing",
            WithSectionCount(9),
            VmReason.Truncated, 2001,
            "Nine sections declared and two present: the third section header runs off the end."));

        entries.Add(Exhausted(
            "sections-count-above-declared-count-bound", "framing",
            WithSectionCount(70_000),
            VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact,
            "A section count above the declared-count ceiling is refused before it can drive a loop."));

        entries.Add(Exhausted(
            "sections-beyond-section-ceiling", "framing",
            ManySections(65),
            VmBudgetDimension.SectionCount, VmBudgetScope.Artifact,
            "Sixty-five sections against a sixty-four section ceiling: entering the sixty-fifth is refused. "
            + "A ceiling breach and not a malformed artifact - the sixty-five frames are all well formed, "
            + "and the same bytes verify against a host that permits sixty-five. This entry was pinned as "
            + "invalid-artifact until 2026-08-31 and is the reason that misreading was worth finding: a "
            + "corpus entry recording the wrong category does not fail, it passes."));

        foreach (var kind in new byte[] { 0, 3, 9, 255 })
        {
            entries.Add(Invalid(
                $"section-kind-{kind}", "framing",
                Build(FixtureFormat.FormatVersion, [Section(kind, ConstantBody([1])), Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpReturn]))]),
                VmReason.UnknownFeature, 1004,
                $"A section of unknown kind {kind}. An unknown section is a deterministic rejection, not a skip: skipping would let an artifact carry content nothing looked at."));
        }

        entries.Add(Invalid(
            "section-length-over-by-three", "framing",
            FixtureArtifactWriter.Write([1], [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn], FixtureArtifactWriter.Corruption.SectionLengthMismatch),
            VmReason.MalformedEncoding, 2002,
            "A section declaring three bytes more than its body. Consuming less than declared is as structural an error as consuming more."));

        entries.Add(Invalid(
            "section-length-under-by-one", "framing",
            WithConstantsSectionLengthDelta(-1),
            VmReason.MalformedEncoding, 2002,
            "A section declaring one byte less than its body, so the next section header would start inside it."));

        entries.Add(Invalid(
            "section-length-beyond-the-artifact", "framing",
            WithConstantsSectionLength(1UL << 40),
            VmReason.Truncated, 2001,
            "A section declaring a terabyte inside a twenty-byte artifact. The length is compared with what remains before a frame exists."));

        entries.Add(Invalid(
            "section-length-maximum", "framing",
            WithConstantsSectionLength(ulong.MaxValue),
            VmReason.Truncated, 2001,
            "A section length of 2^64-1. The comparison is against the remainder, so the arithmetic never has to hold the sum."));
    }

    // ---- the constant pool ------------------------------------------------------------------------

    private static void AddConstantPool(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        entries.Add(Exhausted(
            "constants-count-four-billion", "constants",
            FixtureArtifactWriter.Write([1], [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn], FixtureArtifactWriter.Corruption.OverDeclaredCount),
            VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact,
            "Four billion constants declared. The count is refused before anything proportional to it is allocated, which is the ordering the whole guard exists for."));

        entries.Add(Exhausted(
            "constants-count-just-over-the-bound", "constants",
            WithConstantCount(65_537, [1]),
            VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact,
            "One more than the declared-count ceiling. The interesting case is the boundary, not the absurd number."));

        entries.Add(Invalid(
            "constants-count-at-the-bound", "constants",
            WithConstantCount(65_536, [1]),
            VmReason.Truncated, 2001,
            "Exactly at the declared-count ceiling, so the count is admitted and the pool is allocated - and then the artifact runs out of bytes long before it has 65,536 values. Resource-hostile and in bounds at once."));

        entries.Add(Invalid(
            "constants-count-exceeds-body", "constants",
            WithConstantCount(3, [1, 2]),
            VmReason.MalformedEncoding, 2002,
            "Three constants declared and two present, so the third is read out of the next section's header and the frame no longer closes where it said it would."));

        entries.Add(Invalid(
            "constants-non-canonical-varint", "constants",
            FixtureArtifactWriter.Write([1], [FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn], FixtureArtifactWriter.Corruption.NonCanonicalVarInt),
            VmReason.MalformedEncoding, 2002,
            "A constant encoded with a redundant continuation group."));

        entries.Add(Invalid(
            "constants-overlong-varint", "constants",
            WithRawConstantBody(1, [0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x00]),
            VmReason.MalformedEncoding, 2002,
            "A constant whose encoding carries more groups than sixty-four bits can hold."));
    }

    // ---- the code section -------------------------------------------------------------------------

    private static void AddCodeSection(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        entries.Add(Exhausted(
            "code-length-above-the-bound", "code",
            WithCodeLength(70_000, [FixtureFormat.OpReturn]),
            VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact,
            "A code length above the declared-count ceiling."));

        entries.Add(Invalid(
            "code-length-exceeds-body", "code",
            WithCodeLength(64, [FixtureFormat.OpReturn]),
            VmReason.Truncated, 2001,
            "Sixty-four bytes of code declared and one present."));

        entries.Add(Invalid(
            "code-unknown-opcode", "code",
            FixtureArtifactWriter.Write([1], [0xFE]),
            VmReason.SemanticValidationFailed, 1006,
            "An opcode outside the fixture's instruction set. Nothing unverified may be reachable by the executor."));

        entries.Add(Invalid(
            "code-operand-missing", "code",
            FixtureArtifactWriter.Write([1], [FixtureFormat.OpPushConst]),
            VmReason.SemanticValidationFailed, 1006,
            "An instruction whose operand byte is not there."));

        entries.Add(Invalid(
            "code-constant-index-out-of-range", "code",
            FixtureArtifactWriter.Write([1], [FixtureFormat.OpPushConst, 5, FixtureFormat.OpReturn]),
            VmReason.SemanticValidationFailed, 1006,
            "A constant index past the end of a pool of one."));

        entries.Add(Invalid(
            "code-operand-index-at-the-boundary", "code",
            FixtureArtifactWriter.Write([1, 2], [FixtureFormat.OpPushConst, 2, FixtureFormat.OpReturn]),
            VmReason.SemanticValidationFailed, 1006,
            "An index exactly one past a pool of two. Off-by-one is the case an index check is most likely to get wrong."));
    }

    // ---- the ceilings ------------------------------------------------------------------------------

    private static void AddCeilings(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        var canonical = Canonical();

        entries.Add(Exhausted(
            "oversized-against-a-requested-ceiling", "ceiling", canonical,
            VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact,
            "Well-formed bytes presented under a descriptor that tightens the artifact-bytes ceiling below their own length. The refusal happens before the first byte is read.",
            artifactBytesRequest: (ulong)canonical.Length - 1));

        entries.Add(Exhausted(
            "oversized-against-a-zero-ceiling", "ceiling", canonical,
            VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact,
            "The same artifact under a request of zero. A ceiling of nothing admits nothing, which is what a bound of zero has to mean.",
            artifactBytesRequest: 1));
    }

    // ---- the systematic sweeps ----------------------------------------------------------------------

    private static void AddSweeps(System.Collections.Generic.List<FixtureCorpusEntry> entries)
    {
        var canonical = Canonical();

        for (var length = 0; length < canonical.Length; length++)
        {
            var prefix = new byte[length];
            System.Array.Copy(canonical, prefix, length);

            entries.Add(Recorded(
                $"truncated-at-{length:D2}", "truncation-sweep", prefix,
                $"The canonical artifact cut to {length} bytes. No prefix of a valid artifact may verify."));
        }

        for (var index = 0; index < canonical.Length; index++)
        {
            var damaged = (byte[])canonical.Clone();
            damaged[index] = (byte)~damaged[index];

            entries.Add(Recorded(
                $"inverted-byte-{index:D2}", "corruption-sweep", damaged,
                $"Byte {index} of the canonical artifact inverted. Some of these are still valid artifacts, and that is the point: the sweep asserts a closed set of answers, not that everything fails."));
        }
    }

    // ---- entry construction --------------------------------------------------------------------------

    private static FixtureCorpusEntry Normal(string id, string family, byte[] bytes, string note) =>
        new(id, family, bytes, note, VmOutcome.Normal, VmReason.NormalCompleted, 0,
            VmBudgetDimension.Fuel, VmBudgetScope.Artifact, false,
            FixtureCorpusPinning.Exact, FixtureFormat.FormatVersion, 0);

    private static FixtureCorpusEntry Invalid(
        string id,
        string family,
        byte[] bytes,
        VmReason reason,
        int profileDiagnosticCode,
        string note,
        uint descriptorFormatVersion = FixtureFormat.FormatVersion) =>
        new(id, family, bytes, note, VmOutcome.InvalidArtifact, reason, profileDiagnosticCode,
            VmBudgetDimension.Fuel, VmBudgetScope.Artifact, false,
            FixtureCorpusPinning.Exact, descriptorFormatVersion, 0);

    private static FixtureCorpusEntry Exhausted(
        string id,
        string family,
        byte[] bytes,
        VmBudgetDimension dimension,
        VmBudgetScope scope,
        string note,
        ulong artifactBytesRequest = 0) =>
        new(id, family, bytes, note, VmOutcome.ResourceExhaustion, VmReason.None, 0,
            dimension, scope, true,
            FixtureCorpusPinning.Exact, FixtureFormat.FormatVersion, artifactBytesRequest);

    private static FixtureCorpusEntry Recorded(string id, string family, byte[] bytes, string note) =>
        new(id, family, bytes, note, VmOutcome.None, VmReason.None, 0,
            VmBudgetDimension.Fuel, VmBudgetScope.Artifact, false,
            FixtureCorpusPinning.Recorded, FixtureFormat.FormatVersion, 0);

    // ---- byte construction ----------------------------------------------------------------------------

    private static byte[] Section(byte kind, byte[] body)
    {
        var buffer = new System.Collections.Generic.List<byte>(body.Length + 8) { kind };
        WriteVarUInt(buffer, (ulong)body.Length);
        buffer.AddRange(body);
        return buffer.ToArray();
    }

    private static byte[] SectionWithStatedLength(byte kind, byte[] body, ulong statedLength)
    {
        var buffer = new System.Collections.Generic.List<byte>(body.Length + 16) { kind };
        WriteVarUInt(buffer, statedLength);
        buffer.AddRange(body);
        return buffer.ToArray();
    }

    private static byte[] ConstantBody(long[] constants) => RawConstantBody((uint)constants.Length, constants);

    private static byte[] RawConstantBody(uint statedCount, long[] constants)
    {
        var body = new System.Collections.Generic.List<byte>(constants.Length * 2 + 4);
        WriteVarUInt(body, statedCount);

        foreach (var constant in constants)
        {
            WriteVarUInt(body, unchecked((ulong)constant));
        }

        return body.ToArray();
    }

    private static byte[] CodeBody(byte[] code) => RawCodeBody((uint)code.Length, code);

    private static byte[] RawCodeBody(uint statedLength, byte[] code)
    {
        var body = new System.Collections.Generic.List<byte>(code.Length + 4);
        WriteVarUInt(body, statedLength);
        body.AddRange(code);
        return body.ToArray();
    }

    private static byte[] Build(uint formatVersion, byte[][] sections) =>
        Build(formatVersion, (ulong)sections.Length, sections);

    private static byte[] Build(uint formatVersion, ulong statedSectionCount, byte[][] sections)
    {
        var buffer = new System.Collections.Generic.List<byte>(64);
        buffer.AddRange(FixtureFormat.Magic.ToArray());
        WriteVarUInt(buffer, formatVersion);
        WriteVarUInt(buffer, statedSectionCount);

        foreach (var section in sections)
        {
            buffer.AddRange(section);
        }

        return buffer.ToArray();
    }

    private static byte[] WithSectionCount(ulong stated) =>
        Build(
            FixtureFormat.FormatVersion,
            stated,
            [
                Section(FixtureFormat.SectionConstants, ConstantBody([1])),
                Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn])),
            ]);

    private static byte[] ManySections(int count)
    {
        var sections = new byte[count][];

        for (var index = 0; index < count; index++)
        {
            sections[index] = Section(FixtureFormat.SectionConstants, ConstantBody([]));
        }

        return Build(FixtureFormat.FormatVersion, sections);
    }

    private static byte[] WithConstantCount(uint statedCount, long[] present) =>
        Build(
            FixtureFormat.FormatVersion,
            [
                Section(FixtureFormat.SectionConstants, RawConstantBody(statedCount, present)),
                Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpReturn])),
            ]);

    private static byte[] WithRawConstantBody(uint statedCount, byte[] rawValues)
    {
        var body = new System.Collections.Generic.List<byte>(rawValues.Length + 4);
        WriteVarUInt(body, statedCount);
        body.AddRange(rawValues);

        return Build(
            FixtureFormat.FormatVersion,
            [
                Section(FixtureFormat.SectionConstants, body.ToArray()),
                Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpReturn])),
            ]);
    }

    private static byte[] WithCodeLength(uint statedLength, byte[] present) =>
        Build(
            FixtureFormat.FormatVersion,
            [
                Section(FixtureFormat.SectionConstants, ConstantBody([1])),
                Section(FixtureFormat.SectionCode, RawCodeBody(statedLength, present)),
            ]);

    private static byte[] WithConstantsSectionLength(ulong statedLength) =>
        Build(
            FixtureFormat.FormatVersion,
            [
                SectionWithStatedLength(FixtureFormat.SectionConstants, ConstantBody([1]), statedLength),
                Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpReturn])),
            ]);

    private static byte[] WithConstantsSectionLengthDelta(int delta)
    {
        var body = ConstantBody([1]);
        return Build(
            FixtureFormat.FormatVersion,
            [
                SectionWithStatedLength(FixtureFormat.SectionConstants, body, (ulong)(body.Length + delta)),
                Section(FixtureFormat.SectionCode, CodeBody([FixtureFormat.OpReturn])),
            ]);
    }

    private static void WriteVarUInt(System.Collections.Generic.List<byte> buffer, ulong value)
    {
        while (true)
        {
            var group = (byte)(value & 0x7F);
            value >>= 7;

            if (value == 0)
            {
                buffer.Add(group);
                return;
            }

            buffer.Add((byte)(group | 0x80));
        }
    }
}
