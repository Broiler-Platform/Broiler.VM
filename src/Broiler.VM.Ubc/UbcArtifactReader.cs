// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           14
// Human-reviewed:   0/24
// IP risk:          Low
// Security risk:    Critical
// Criteria:         21/21
// Resource impact:  6/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>
/// Reads a universal bytecode container into an <see cref="UbcArtifact"/>, refusing anything that is
/// not well framed, under the four read ceilings and the two allowances a verification runs under.
/// </summary>
/// <remarks>
/// <para>
/// <b>Framing only.</b> The reader checks what the bytes are - the magic, the format version, the
/// identity strings, the section kinds and their order, every length, every count against the
/// declared-count ceiling, the slot-type bytes, and that each section consumed exactly its declared
/// length - and decodes every section the universal bytecode owns into rows. Whether those rows agree
/// with each other and with the code is the walk's (<see cref="UbcVerifier"/>).
/// </para>
/// <para>
/// <b>The canonical read order is kept.</b> The bounds and the meter are parameters, so the policy is
/// handed over before anything happens; the first thing read is the magic; and every row array, string
/// and byte run is reserved against the allocated-bytes allowance after the count or length that
/// justifies it has been read and bounded, and before it is allocated. A reservation is never released:
/// the decoded artifact keeps what it reserved.
/// </para>
/// <para>
/// <b>No member throws</b> on any input; a refusal is an answer.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=6; Fingerprint=F628F2
// Broiler-Falsified-If: an input makes the reader throw, allocate before the count that sizes the allocation has passed its bound, or answer an artifact whose sections were not each consumed exactly
// Broiler-Human:        PENDING
public static class UbcArtifactReader
{
    // The bytes a decoded row is reserved at: an estimate of the object's footprint, stated once so
    // that every row is charged alike. It is not a measurement and nothing depends on its exactness;
    // it exists so that a count the declared-count ceiling admits still has to fit the allowance.
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B9C923
    // Broiler-Human:        PENDING
    private const ulong RowBytes = 64;

    /// <summary>
    /// Reads <paramref name="payload"/>. When <paramref name="descriptorFormatVersion"/> is given, the
    /// payload's format version is compared with it before anything else is read.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=6; Fingerprint=77EF0F
    // Broiler-Falsified-If: a payload with trailing bytes, an out-of-order or repeated section, a section not consumed exactly, or a count past its bound yields an artifact
    // Broiler-Human:        PENDING
    public static bool TryRead(
        System.ReadOnlySpan<byte> payload,
        in VmReadBounds bounds,
        IVmBoundedAllocationMeter meter,
        ulong pollGranularity,
        uint? descriptorFormatVersion,
        out UbcArtifact? artifact,
        out UbcRefusal refusal)
    {
        artifact = null;
        var reader = new VmBoundedReader(payload, in bounds, meter, pollGranularity);
        var context = new Context(bounds, meter, pollGranularity);

        if (!reader.TryReadBytes(4, out var magic))
        {
            refusal = FromReader(ref reader, UbcRefusal.InHeader(0));
            return false;
        }

        if (!System.MemoryExtensions.SequenceEqual(magic, UbcFormat.Magic))
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.WrongMagic, VmReason.MalformedEncoding, UbcRefusal.InHeader(0));
            return false;
        }

        if (!reader.TryReadVarUInt32(out var formatVersion))
        {
            refusal = FromReader(ref reader, UbcRefusal.InHeader(reader.Position));
            return false;
        }

        // The descriptor-against-payload comparison comes first: a payload that disagrees with the
        // descriptor it was presented under is refused for that, whatever version it claims.
        if (descriptorFormatVersion is { } expected && expected != formatVersion)
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.DescriptorFormatVersionMismatch, VmReason.DescriptorMismatch, UbcRefusal.InHeader(reader.Position));
            return false;
        }

        if (formatVersion != UbcFormat.FormatVersion)
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.UnsupportedFormatVersion, VmReason.UnsupportedProfileFormatVersion, UbcRefusal.InHeader(reader.Position));
            return false;
        }

        if (!TryReadIdentity(ref reader, context, -1, out var profile, out refusal) ||
            !TryReadIdentity(ref reader, context, -1, out var manifest, out refusal) ||
            !TryReadIdentity(ref reader, context, -1, out var form, out refusal) ||
            !TryReadIdentity(ref reader, context, -1, out var translator, out refusal))
        {
            return false;
        }

        if (!reader.TryReadVarUInt32(out var translatorVersion) || !reader.TryReadDeclaredCount(out var sectionCount))
        {
            refusal = FromReader(ref reader, UbcRefusal.InHeader(reader.Position));
            return false;
        }

        var header = new UbcHeader(formatVersion, profile, manifest, form, translator, translatorVersion);
        var sections = new Sections();
        var previous = 0u;

        for (var index = 0u; index < sectionCount; index++)
        {
            if (!meter.Poll())
            {
                refusal = UbcRefusal.Exhausted(VmBudgetDimension.VerifierWork);
                return false;
            }

            if (!TryReadSection(ref reader, context, ref previous, sections, out refusal))
            {
                return false;
            }
        }

        if (reader.Remaining != 0)
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.TrailingBytes, VmReason.InconsistentStructure, UbcRefusal.InHeader(reader.Position));
            return false;
        }

        if (sections.Types.IsDefault || sections.Units.IsDefault || sections.Code.IsDefault || sections.Entries.IsDefault)
        {
            var missing = sections.Types.IsDefault ? UbcSectionKind.Types
                : sections.Units.IsDefault ? UbcSectionKind.Units
                : sections.Code.IsDefault ? UbcSectionKind.Code
                : UbcSectionKind.Entries;
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.MissingSection, VmReason.InconsistentStructure, UbcRefusal.InSection(missing, reader.Position));
            return false;
        }

        artifact = new UbcArtifact(
            header,
            OrEmpty(sections.Families),
            sections.Types,
            sections.Units,
            sections.Code,
            OrEmpty(sections.JumpTables),
            OrEmpty(sections.Regions),
            sections.Entries,
            OrEmpty(sections.Positions),
            sections.FamilyData.ToImmutable(),
            sections.Emission,
            sections.Present);
        refusal = default;
        return true;
    }

    /// <summary>
    /// The refusal a latched reader status is answered with: ADR 0011's canonical mapping of a bounded
    /// read status onto an outcome.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=High; Resources=0; Fingerprint=06EC41
    // Broiler-Falsified-If: a ceiling status is answered as an invalid artifact, or a status is mapped to another status's dimension
    // Broiler-Human:        PENDING
    internal static UbcRefusal FromReader(ref VmBoundedReader reader, VmSourcePosition position) => reader.Status switch
    {
        VmBoundedReadStatus.Truncated => UbcRefusal.Invalid(UbcDiagnosticCode.Truncated, VmReason.Truncated, position),
        VmBoundedReadStatus.MalformedEncoding => UbcRefusal.Invalid(UbcDiagnosticCode.MalformedEncoding, VmReason.MalformedEncoding, position),
        VmBoundedReadStatus.DeclaredCountExceeded => UbcRefusal.Exhausted(VmBudgetDimension.DeclaredCount),
        VmBoundedReadStatus.SectionCountExceeded => UbcRefusal.Exhausted(VmBudgetDimension.SectionCount),
        VmBoundedReadStatus.StructuralDepthExceeded => UbcRefusal.Exhausted(VmBudgetDimension.StructuralDepth),
        VmBoundedReadStatus.ArtifactBytesExceeded => UbcRefusal.Exhausted(VmBudgetDimension.ArtifactBytes),
        VmBoundedReadStatus.AllocationRefused => UbcRefusal.Exhausted(VmBudgetDimension.AllocatedBytes),
        VmBoundedReadStatus.WorkBudgetExhausted => UbcRefusal.Exhausted(VmBudgetDimension.VerifierWork),
        _ => UbcRefusal.Invalid(UbcDiagnosticCode.ReaderStopped, VmReason.InconsistentStructure, position),
    };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=95F3E4
    // Broiler-Falsified-If: a section kind is admitted before its order and uniqueness are checked, or a body that under-reads or over-reads its declared length is accepted
    // Broiler-Human:        PENDING
    private static bool TryReadSection(
        ref VmBoundedReader reader,
        Context context,
        ref uint previous,
        Sections sections,
        out UbcRefusal refusal)
    {
        var at = reader.Position;

        if (!reader.TryReadVarUInt32(out var kind) || !reader.TryReadVarUInt64(out var length))
        {
            refusal = FromReader(ref reader, UbcRefusal.InHeader(reader.Position));
            return false;
        }

        if (kind is < (uint)UbcSectionKind.Families or > (uint)UbcSectionKind.Emission ||
            kind is > (uint)UbcSectionKind.FamilyDataLast and < (uint)UbcSectionKind.Emission)
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.UnknownSectionKind, VmReason.UnknownFeature, UbcRefusal.InHeader(at));
            return false;
        }

        var sectionKind = (UbcSectionKind)kind;

        if (kind <= previous)
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.SectionOrder, VmReason.InconsistentStructure, UbcRefusal.InSection(sectionKind, at));
            return false;
        }

        previous = kind;
        sections.Present |= 1u << (int)kind;

        if (!reader.TryEnterSection(length, out var frame))
        {
            refusal = FromReader(ref reader, UbcRefusal.InSection(sectionKind, reader.Position));
            return false;
        }

        var read = sectionKind switch
        {
            UbcSectionKind.Families => ReadFamilies(ref reader, context, sections, out refusal),
            UbcSectionKind.Types => ReadTypes(ref reader, context, sections, out refusal),
            UbcSectionKind.Units => ReadUnits(ref reader, context, sections, out refusal),
            UbcSectionKind.Code => ReadCode(ref reader, context, sections, length, out refusal),
            UbcSectionKind.JumpTables => ReadJumpTables(ref reader, context, sections, out refusal),
            UbcSectionKind.Regions => ReadRegions(ref reader, context, sections, out refusal),
            UbcSectionKind.Entries => ReadEntries(ref reader, context, sections, out refusal),
            UbcSectionKind.Positions => ReadPositions(ref reader, context, sections, out refusal),
            UbcSectionKind.Emission => ReadEmission(ref reader, context, sections, out refusal),
            _ => ReadFamilyData(ref reader, context, sections, (byte)(kind - (uint)UbcSectionKind.FamilyDataFirst + 1), length, out refusal),
        };

        if (!read)
        {
            return false;
        }

        if (!reader.IsOk)
        {
            refusal = FromReader(ref reader, UbcRefusal.InSection(sectionKind, reader.Position));
            return false;
        }

        if (!reader.TryExitSection(in frame))
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.SectionLengthMismatch, VmReason.InconsistentStructure, UbcRefusal.InSection(sectionKind, reader.Position));
            return false;
        }

        refusal = default;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D3E5E7
    // Broiler-Falsified-If: a slot outside one to fourteen, or a slot not above its predecessor, is read into a row
    // Broiler-Human:        PENDING
    private static bool ReadFamilies(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.Families;

        if (!TryReadCount(ref reader, context, kind, RowBytes, out var count, out refusal))
        {
            return false;
        }

        var rows = ImmutableArray.CreateBuilder<UbcFamilyEntry>((int)count);
        var previous = 0u;

        for (var index = 0u; index < count; index++)
        {
            var at = reader.Position;

            if (!reader.TryReadVarUInt32(out var slot))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (slot is 0 or > UbcFormat.MaxFamilySlot || slot <= previous)
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.FamilySlotInvalid, VmReason.InconsistentStructure, UbcRefusal.InSection(kind, at));
                return false;
            }

            previous = slot;

            if (!TryReadIdentity(ref reader, context, (int)kind, out var identity, out refusal))
            {
                return false;
            }

            if (!reader.TryReadVarUInt32(out var tableVersion))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (!TryReadIdentity(ref reader, context, (int)kind, out var manifest, out refusal))
            {
                return false;
            }

            rows.Add(new UbcFamilyEntry((byte)slot, identity, tableVersion, manifest));
        }

        sections.Families = rows.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CADD1D
    // Broiler-Falsified-If: a slot-type byte outside the closed set is read into a signature
    // Broiler-Human:        PENDING
    private static bool ReadTypes(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.Types;

        if (!TryReadCount(ref reader, context, kind, RowBytes, out var count, out refusal))
        {
            return false;
        }

        var rows = ImmutableArray.CreateBuilder<UbcSignature>((int)count);

        for (var index = 0u; index < count; index++)
        {
            if (!TryReadSlotTypes(ref reader, context, kind, out var parameters, out refusal) ||
                !TryReadSlotTypes(ref reader, context, kind, out var results, out refusal))
            {
                return false;
            }

            rows.Add(new UbcSignature(parameters, results));
        }

        sections.Types = rows.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7FB1F3
    // Broiler-Falsified-If: a unit row with a family slot above fourteen, flags above sixteen bits, or a local-run type outside the closed set is read
    // Broiler-Human:        PENDING
    private static bool ReadUnits(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.Units;

        if (!TryReadCount(ref reader, context, kind, RowBytes, out var count, out refusal))
        {
            return false;
        }

        var rows = ImmutableArray.CreateBuilder<UbcUnit>((int)count);

        for (var index = 0u; index < count; index++)
        {
            var at = reader.Position;

            if (!reader.TryReadVarUInt32(out var typeIndex) || !reader.TryReadVarUInt32(out var familySlot))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (familySlot > UbcFormat.MaxFamilySlot)
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.UnitFamilyUndeclared, VmReason.InconsistentStructure, UbcRefusal.InSection(kind, at));
                return false;
            }

            if (!TryReadCount(ref reader, context, kind, 8, out var runCount, out refusal))
            {
                return false;
            }

            var runs = ImmutableArray.CreateBuilder<UbcLocalRun>((int)runCount);

            for (var run = 0u; run < runCount; run++)
            {
                if (!reader.TryReadVarUInt32(out var runLength) || !reader.TryReadByte(out var type))
                {
                    refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                    return false;
                }

                if (!UbcSlotTypes.IsDefined(type))
                {
                    refusal = UbcRefusal.Invalid(UbcDiagnosticCode.UnknownSlotType, VmReason.UnknownFeature, UbcRefusal.InSection(kind, reader.Position - 1));
                    return false;
                }

                runs.Add(new UbcLocalRun(runLength, (UbcSlotType)type));
            }

            if (!reader.TryReadVarUInt32(out var maxWords) ||
                !reader.TryReadVarUInt32(out var maxValues) ||
                !reader.TryReadVarUInt32(out var codeOffset) ||
                !reader.TryReadVarUInt32(out var codeLength))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            var flagsAt = reader.Position;

            if (!reader.TryReadVarUInt32(out var flags))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (flags > ushort.MaxValue)
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.ReservedUnitFlag, VmReason.InconsistentStructure, UbcRefusal.InSection(kind, flagsAt));
                return false;
            }

            if (!TryReadOffsets(ref reader, context, kind, out var landings, out refusal))
            {
                return false;
            }

            rows.Add(new UbcUnit(typeIndex, (byte)familySlot, runs.MoveToImmutable(), maxWords, maxValues,
                codeOffset, codeLength, (UbcUnitFlags)flags, landings));
        }

        sections.Units = rows.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=E757D0
    // Broiler-Falsified-If: the code bytes are allocated before their length is reserved, or read in one charge larger than the poll granularity
    // Broiler-Human:        PENDING
    private static bool ReadCode(ref VmBoundedReader reader, Context context, Sections sections, ulong length, out UbcRefusal refusal)
    {
        if (!TryReadRun(ref reader, context, UbcSectionKind.Code, length, out var bytes, out refusal))
        {
            return false;
        }

        sections.Code = bytes;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CDCFF6
    // Broiler-Falsified-If: a jump table row is read whose target count was not bounded before its targets were reserved
    // Broiler-Human:        PENDING
    private static bool ReadJumpTables(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.JumpTables;

        if (!TryReadCount(ref reader, context, kind, RowBytes, out var count, out refusal))
        {
            return false;
        }

        var rows = ImmutableArray.CreateBuilder<UbcJumpTable>((int)count);

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (!TryReadOffsets(ref reader, context, kind, out var targets, out refusal))
            {
                return false;
            }

            rows.Add(new UbcJumpTable(unit, targets));
        }

        sections.JumpTables = rows.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=BFE508
    // Broiler-Falsified-If: a region row is read from bytes past the section's declared length
    // Broiler-Human:        PENDING
    private static bool ReadRegions(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.Regions;

        if (!TryReadCount(ref reader, context, kind, RowBytes, out var count, out refusal))
        {
            return false;
        }

        var rows = ImmutableArray.CreateBuilder<UbcRegion>((int)count);

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit) ||
                !reader.TryReadVarUInt32(out var start) ||
                !reader.TryReadVarUInt32(out var end) ||
                !reader.TryReadVarUInt32(out var handler) ||
                !reader.TryReadVarUInt32(out var words) ||
                !reader.TryReadVarUInt32(out var values) ||
                !reader.TryReadByte(out var regionKind))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            rows.Add(new UbcRegion(unit, start, end, handler, words, values, regionKind));
        }

        sections.Regions = rows.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=3A55CC
    // Broiler-Falsified-If: an entry name longer than the format admits, or not valid UTF-8, is read into a row
    // Broiler-Human:        PENDING
    private static bool ReadEntries(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.Entries;

        if (!TryReadCount(ref reader, context, kind, RowBytes, out var count, out refusal))
        {
            return false;
        }

        var rows = ImmutableArray.CreateBuilder<UbcEntry>((int)count);

        for (var index = 0u; index < count; index++)
        {
            var at = reader.Position;

            if (!reader.TryReadVarUInt32(out var nameLength))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (nameLength is 0 or > UbcFormat.MaxEntryNameBytes)
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.EntryNameInvalid, VmReason.InconsistentStructure, UbcRefusal.InSection(kind, at));
                return false;
            }

            if (!reader.TryReadBytes(nameLength, out var name))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (!System.Text.Unicode.Utf8.IsValid(name))
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.EntryNameInvalid, VmReason.InconsistentStructure, UbcRefusal.InSection(kind, at));
                return false;
            }

            if (!context.TryReserve(nameLength, out refusal))
            {
                return false;
            }

            var copy = ImmutableArray.Create(name.ToArray());

            if (!reader.TryReadVarUInt32(out var unit))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            rows.Add(new UbcEntry(copy, unit));
        }

        sections.Entries = rows.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CF6549
    // Broiler-Falsified-If: a coordinate above the core's signed range is read into a row
    // Broiler-Human:        PENDING
    private static bool ReadPositions(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.Positions;

        if (!TryReadCount(ref reader, context, kind, RowBytes, out var count, out refusal))
        {
            return false;
        }

        var rows = ImmutableArray.CreateBuilder<UbcPosition>((int)count);

        for (var index = 0u; index < count; index++)
        {
            var at = reader.Position;

            if (!reader.TryReadVarUInt32(out var unit) ||
                !reader.TryReadVarUInt32(out var offset) ||
                !reader.TryReadVarUInt32(out var first) ||
                !reader.TryReadVarUInt32(out var second))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (first > int.MaxValue || second > int.MaxValue)
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.PositionInvalid, VmReason.InconsistentStructure, UbcRefusal.InSection(kind, at));
                return false;
            }

            rows.Add(new UbcPosition(unit, offset, (int)first, (int)second));
        }

        sections.Positions = rows.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=5A79FC
    // Broiler-Falsified-If: a family's section body is allocated before its length passes the artifact bound and the allowance
    // Broiler-Human:        PENDING
    private static bool ReadFamilyData(ref VmBoundedReader reader, Context context, Sections sections, byte slot, ulong length, out UbcRefusal refusal)
    {
        var kind = (UbcSectionKind)UbcFormat.FamilyDataKind(slot);

        if (!TryReadRun(ref reader, context, kind, length, out var body, out refusal))
        {
            return false;
        }

        sections.FamilyData.Add(new UbcFamilyData(slot, body));
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=CC904A
    // Broiler-Falsified-If: an emission whose stated code length is not the bytes that follow is read into a section
    // Broiler-Human:        PENDING
    private static bool ReadEmission(ref VmBoundedReader reader, Context context, Sections sections, out UbcRefusal refusal)
    {
        const UbcSectionKind kind = UbcSectionKind.Emission;

        if (!TryReadIdentity(ref reader, context, (int)kind, out var form, out refusal))
        {
            return false;
        }

        if (!reader.TryReadVarUInt32(out var version) ||
            !reader.TryReadVarUInt32(out var alignment) ||
            !reader.TryReadUInt64LittleEndian(out var shapeHash) ||
            !reader.TryReadUInt32LittleEndian(out var codeLength))
        {
            refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
            return false;
        }

        if (!TryReadRun(ref reader, context, kind, codeLength, out var bytes, out refusal))
        {
            return false;
        }

        if (!TryReadCount(ref reader, context, kind, 8, out var symbolCount, out refusal))
        {
            return false;
        }

        var symbols = ImmutableArray.CreateBuilder<UbcSymbol>((int)symbolCount);

        for (var index = 0u; index < symbolCount; index++)
        {
            if (!reader.TryReadVarUInt32(out var unit) || !reader.TryReadVarUInt32(out var offset))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            symbols.Add(new UbcSymbol(unit, offset));
        }

        sections.Emission = new UbcEmission(form, version, alignment, shapeHash, bytes, symbols.MoveToImmutable());
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=299E8D
    // Broiler-Falsified-If: a count is returned, or reserved against, before the declared-count ceiling has passed it
    // Broiler-Human:        PENDING
    private static bool TryReadCount(
        ref VmBoundedReader reader,
        Context context,
        UbcSectionKind kind,
        ulong bytesPerItem,
        out uint count,
        out UbcRefusal refusal)
    {
        if (!reader.TryReadDeclaredCount(out count))
        {
            refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
            return false;
        }

        return context.TryReserve(count * bytesPerItem, out refusal);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=02A77C
    // Broiler-Falsified-If: a slot list is read whose count was not bounded, or a byte outside the closed set of slot types is answered as a type
    // Broiler-Human:        PENDING
    private static bool TryReadSlotTypes(
        ref VmBoundedReader reader,
        Context context,
        UbcSectionKind kind,
        out ImmutableArray<UbcSlotType> types,
        out UbcRefusal refusal)
    {
        types = default;

        if (!TryReadCount(ref reader, context, kind, 1, out var count, out refusal))
        {
            return false;
        }

        var builder = ImmutableArray.CreateBuilder<UbcSlotType>((int)count);

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadByte(out var type))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            if (!UbcSlotTypes.IsDefined(type))
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.UnknownSlotType, VmReason.UnknownFeature, UbcRefusal.InSection(kind, reader.Position - 1));
                return false;
            }

            builder.Add((UbcSlotType)type);
        }

        types = builder.MoveToImmutable();
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=3994A7
    // Broiler-Falsified-If: an offset list is reserved or read before its count passed the declared-count ceiling
    // Broiler-Human:        PENDING
    private static bool TryReadOffsets(
        ref VmBoundedReader reader,
        Context context,
        UbcSectionKind kind,
        out ImmutableArray<uint> offsets,
        out UbcRefusal refusal)
    {
        offsets = default;

        if (!TryReadCount(ref reader, context, kind, 4, out var count, out refusal))
        {
            return false;
        }

        var builder = ImmutableArray.CreateBuilder<uint>((int)count);

        for (var index = 0u; index < count; index++)
        {
            if (!reader.TryReadVarUInt32(out var offset))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            builder.Add(offset);
        }

        offsets = builder.MoveToImmutable();
        return true;
    }

    /// <summary>
    /// Reads an identity string: a length bounded by the format, then that many bytes, each an ASCII
    /// letter, digit, dot, hyphen or underscore.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=67607E
    // Broiler-Falsified-If: an empty identity, one longer than the format admits, or one holding a byte outside its character set is answered as a string
    // Broiler-Human:        PENDING
    private static bool TryReadIdentity(
        ref VmBoundedReader reader,
        Context context,
        int sectionIndex,
        out string identity,
        out UbcRefusal refusal)
    {
        identity = string.Empty;
        var at = reader.Position;
        var position = new VmSourcePosition(sectionIndex, at, -1, -1);

        if (!reader.TryReadVarUInt32(out var length))
        {
            refusal = FromReader(ref reader, position);
            return false;
        }

        if (length is 0 or > UbcFormat.MaxIdentityBytes)
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.MalformedIdentity, VmReason.MalformedEncoding, position);
            return false;
        }

        if (!reader.TryReadBytes(length, out var bytes))
        {
            refusal = FromReader(ref reader, position);
            return false;
        }

        foreach (var value in bytes)
        {
            var admitted = value is (>= (byte)'a' and <= (byte)'z') or (>= (byte)'A' and <= (byte)'Z')
                or (>= (byte)'0' and <= (byte)'9') or (byte)'.' or (byte)'-' or (byte)'_';

            if (!admitted)
            {
                refusal = UbcRefusal.Invalid(UbcDiagnosticCode.MalformedIdentity, VmReason.MalformedEncoding, position);
                return false;
            }
        }

        if (!context.TryReserve(length, out refusal))
        {
            return false;
        }

        identity = System.Text.Encoding.ASCII.GetString(bytes);
        return true;
    }

    /// <summary>
    /// Reads a run of bytes: reserved first, then read in pieces no larger than the poll granularity,
    /// so no single work charge outruns the uncharged-work bound the reader polls against.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=C42454
    // Broiler-Falsified-If: the run is allocated before its length passes the artifact bound and the allowance, or one read charges more work than the granularity
    // Broiler-Human:        PENDING
    private static bool TryReadRun(
        ref VmBoundedReader reader,
        Context context,
        UbcSectionKind kind,
        ulong length,
        out ImmutableArray<byte> bytes,
        out UbcRefusal refusal)
    {
        bytes = ImmutableArray<byte>.Empty;

        if (length > reader.Remaining)
        {
            refusal = UbcRefusal.Invalid(UbcDiagnosticCode.Truncated, VmReason.Truncated, UbcRefusal.InSection(kind, reader.Position));
            return false;
        }

        if (!VmBoundedAllocator.TryAllocateExact<byte>(context.Bounds, context.Meter, length, out var buffer))
        {
            refusal = UbcRefusal.Exhausted(VmBudgetDimension.AllocatedBytes);
            return false;
        }

        var window = context.Granularity is 0 or > 65536 ? 65536UL : context.Granularity;
        var written = 0UL;

        while (written < length)
        {
            var piece = System.Math.Min(window, length - written);

            if (!reader.TryReadBytes(piece, out var chunk))
            {
                refusal = FromReader(ref reader, UbcRefusal.InSection(kind, reader.Position));
                return false;
            }

            chunk.CopyTo(System.MemoryExtensions.AsSpan(buffer, (int)written));
            written += piece;
        }

        bytes = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(buffer);
        refusal = default;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1C2C8E
    // Broiler-Human:        PENDING
    private static ImmutableArray<T> OrEmpty<T>(ImmutableArray<T> array) => array.IsDefault ? ImmutableArray<T>.Empty : array;

    /// <summary>What every section reader shares: the bounds, the meter, and the granularity.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7776E1
    // Broiler-Falsified-If: a reservation larger than the artifact bound reaches the meter, or a refused reservation is answered as anything but an allocation exhaustion
    // Broiler-Human:        PENDING
    private sealed class Context(VmReadBounds bounds, IVmBoundedAllocationMeter meter, ulong granularity)
    {
        internal VmReadBounds Bounds { get; } = bounds;

        internal IVmBoundedAllocationMeter Meter { get; } = meter;

        internal ulong Granularity { get; } = granularity;

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=98778E
        // Broiler-Falsified-If: a byte count past the artifact bound is charged, or a refusal is not answered as an allocation exhaustion
        // Broiler-Human:        PENDING
        internal bool TryReserve(ulong bytes, out UbcRefusal refusal)
        {
            refusal = default;

            if (bytes == 0)
            {
                return true;
            }

            // An artifact can never need more resident bytes than it may itself be long, which is the
            // allocator's own rule; a count whose rows would exceed it is refused before the meter.
            if (bytes > Bounds.MaxArtifactBytes || !Meter.TryReserve(bytes))
            {
                refusal = UbcRefusal.Exhausted(VmBudgetDimension.AllocatedBytes);
                return false;
            }

            return true;
        }
    }

    /// <summary>The sections read so far.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=736D60
    // Broiler-Human:        PENDING
    private sealed class Sections
    {
        internal ImmutableArray<UbcFamilyEntry> Families { get; set; }

        internal ImmutableArray<UbcSignature> Types { get; set; }

        internal ImmutableArray<UbcUnit> Units { get; set; }

        internal ImmutableArray<byte> Code { get; set; }

        internal ImmutableArray<UbcJumpTable> JumpTables { get; set; }

        internal ImmutableArray<UbcRegion> Regions { get; set; }

        internal ImmutableArray<UbcEntry> Entries { get; set; }

        internal ImmutableArray<UbcPosition> Positions { get; set; }

        internal ImmutableArray<UbcFamilyData>.Builder FamilyData { get; } = ImmutableArray.CreateBuilder<UbcFamilyData>();

        internal UbcEmission? Emission { get; set; }

        internal uint Present { get; set; }
    }
}
