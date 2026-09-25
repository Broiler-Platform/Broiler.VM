// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   62
// Annotated:        62/62
// Exempt:           43
// Human-reviewed:   0/62
// IP risk:          Low
// Security risk:    Critical
// Criteria:         40/35
// Resource impact:  6/10 max
// Unverified:       62
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>
/// The one verifier of every family's descriptor: the container's framing, the structural layer, the
/// code walk over the typed abstract stack, the family's hook, and the form check, in that order, a
/// refusal in an earlier stage never reached past.
/// </summary>
/// <remarks>
/// <para>
/// Built only by <see cref="UbcDescriptors.Build"/>, from the family's declaration, its registration and
/// the forms the image composes. Its semantic version is the universal bytecode's walk version: a
/// change to a family's table or hook is the family's descriptor revision to announce.
/// </para>
/// <para>
/// <b>No member throws on any artifact.</b> Every refusal is one universal diagnostic code with the one
/// core reason <c>docs/ubc/diagnostics/registry.txt</c> gives it, or a family code from the hook, or an
/// exhaustion naming one budget dimension at artifact scope. The work it performs is charged to
/// verifier work before it is done, in pieces no larger than the family's uncharged-work bound with a
/// poll after each, and what it keeps is charged to allocated bytes before it is allocated.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=6; Fingerprint=D5418D
// Broiler-Falsified-If: an artifact whose walk would leave a stack unbalanced, a target off a boundary, a join unequal or a height above its declaration is admitted, or a work charge is performed after the work it pays for
// Broiler-Human:        PENDING
public sealed class UbcVerifier : IVmProfileVerifier
{
    /// <summary>The walk's version: it moves when the walk admits or refuses anything it did not before.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=83433C
    // Broiler-Human:        PENDING
    public const int WalkVersion = 1;

    private readonly UbcFamilyDeclaration declaration;
    private readonly UbcFamilyRegistration family;
    private readonly UbcEmitterSet forms;

    internal UbcVerifier(UbcFamilyDeclaration declaration, UbcFamilyRegistration family, UbcEmitterSet forms)
    {
        this.declaration = declaration;
        this.family = family;
        this.forms = forms;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8180FF
    // Broiler-Human:        PENDING
    public VmProfileId ProfileId => declaration.ProfileId;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=91327C
    // Broiler-Human:        PENDING
    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3577B9
    // Broiler-Human:        PENDING
    public int AuthoredCoreContractVersion => declaration.AuthoredCoreContractVersion;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=219D10
    // Broiler-Human:        PENDING
    public int VerifierSemanticVersion => WalkVersion;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=6; Fingerprint=6FF557
    // Broiler-Falsified-If: a refusal of the reader or the walk is answered as anything but its one outcome, or a stop for cancellation is answered as an exhaustion
    // Broiler-Human:        PENDING
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken)
    {
        var meter = context.Meter;
        var bounds = UbcReadAdapter.ToReadBounds(context.Ceilings.VerificationCeilings);
        var granularity = declaration.MaxUnchargedWork == 0 ? 65536UL : declaration.MaxUnchargedWork;

        // A verification can share its meter with an executing operation - a guest load's nested
        // verification is charged to the operation that asked - so it neither inherits that
        // operation's unpolled work nor leaves its own behind: it polls before its first read and
        // again before it answers.
        if (!meter.Poll())
        {
            return Answer(UbcRefusal.Exhausted(VmBudgetDimension.VerifierWork), cancellationToken);
        }

        if (!UbcArtifactReader.TryRead(payload, in bounds, new UbcReadAdapter(meter), granularity, descriptor.FormatVersion, out var artifact, out var refusal))
        {
            meter.Poll();
            return Answer(refusal, cancellationToken);
        }

        var walk = new UbcWalk(family, forms, artifact!, descriptor.ProfileId, descriptor.FeatureManifestId, meter, granularity);
        var admitted = walk.Run(out var program);
        var settled = meter.Poll();

        if (!admitted)
        {
            return Answer(walk.Refusal, cancellationToken);
        }

        return settled
            ? VmVerifierOutcome.Verified(program!, declaration.ArtifactSharing)
            : Answer(UbcRefusal.Exhausted(VmBudgetDimension.VerifierWork), cancellationToken);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=88F12E
    // Broiler-Falsified-If: a verifier-work stop while the token is cancelled is answered as an exhaustion
    // Broiler-Human:        PENDING
    private static VmVerifierOutcome Answer(UbcRefusal refusal, System.Threading.CancellationToken cancellationToken) =>
        refusal.Kind == UbcRefusalKind.Exhausted && refusal.Dimension == VmBudgetDimension.VerifierWork && cancellationToken.IsCancellationRequested
            ? VmVerifierOutcome.Cancellation()
            : refusal.ToOutcome();
}

/// <summary>One slot of the walk's typed abstract stack: a persistent list, shared between the states that agree below it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CBDAFA
// Broiler-Human:        PENDING
internal sealed class UbcStackNode
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1C57FC
    // Broiler-Human:        PENDING
    internal UbcStackNode(UbcSlotType type, UbcStackNode? below)
    {
        Type = type;
        Below = below;
        Height = (below?.Height ?? 0) + 1;
        Words = (below?.Words ?? 0) + (type == UbcSlotType.V ? 0 : 1);
        Values = (below?.Values ?? 0) + (type == UbcSlotType.V ? 1 : 0);
    }

    internal UbcSlotType Type { get; }

    internal UbcStackNode? Below { get; }

    internal int Height { get; }

    internal int Words { get; }

    internal int Values { get; }
}

/// <summary>
/// One run of the walk over one artifact: the structural layer, every unit's code walk, the family hook
/// and the form check, holding the refusal it stopped at.
/// </summary>
/// <remarks>
/// <para>
/// <b>The code walk</b> decodes a unit's instructions linearly from its first byte, which fixes its
/// boundaries; then types the stack from the unit's entry, an empty operand stack, following every
/// successor. The first state to reach an instruction is its state, and every later arrival must equal
/// it, so each instruction is processed exactly once.
/// </para>
/// <para>
/// <b>What it charges</b>, each before the work is done: one unit per row, instruction or target for
/// every pass it makes over them, one per slot it pushes, pops, takes, walks past or compares, twice the
/// comparisons an introspective sort can make for every sort, and every binary search at the depth of
/// what it searches - the unit's instructions for a code target, a jump-table target and a region's
/// start, end and handler, the longest unit's instructions for a position, the unit's local runs for a
/// local, and the unit's landings for every landing it requires.
/// </para>
/// <para>
/// <b>Regions</b> are an edge from every covered instruction to the handler. The first covered
/// instruction processed fixes the region's entry prefix - the bottom slots of its stack, exactly the
/// region's entry heights of words and values - and the handler is reached with that prefix and the
/// region kind's landing pushes. After the fixpoint every covered instruction's prefix is compared with
/// it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=6; Fingerprint=E1BDC7
// Broiler-Falsified-If: a stage runs after an earlier stage refused, an instruction is admitted without its effect applied to the typed stack, or work is performed without being charged first
// Broiler-Human:        PENDING
internal sealed class UbcWalk
{
    // Estimates of what the walk keeps per decoded instruction, per stack slot, per unit and per array
    // beyond its elements - the array's header and length, and the padding its last element can leave -
    // stated once so every allocation is charged alike. They are not measurements.
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B3E392
    // Broiler-Human:        PENDING
    private const ulong InstructionBytes = 128;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DCEC44
    // Broiler-Human:        PENDING
    private const ulong NodeBytes = 48;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=88D771
    // Broiler-Human:        PENDING
    private const ulong UnitBytes = 128;
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F024B2
    // Broiler-Human:        PENDING
    private const ulong ArrayBytes = 32;

    private readonly UbcFamilyRegistration family;
    private readonly UbcEmitterSet forms;
    private readonly UbcArtifact artifact;
    private readonly VmProfileId descriptorProfile;
    private readonly VmFeatureManifestId descriptorManifest;
    private readonly IVmMeter meter;
    private readonly ulong granularity;
    private ulong uncharged;
    private UbcInstructionTable table = null!;
    private byte slot;
    private object? familyState;

    internal UbcWalk(
        UbcFamilyRegistration family,
        UbcEmitterSet forms,
        UbcArtifact artifact,
        VmProfileId descriptorProfile,
        VmFeatureManifestId descriptorManifest,
        IVmMeter meter,
        ulong granularity)
    {
        this.family = family;
        this.forms = forms;
        this.artifact = artifact;
        this.descriptorProfile = descriptorProfile;
        this.descriptorManifest = descriptorManifest;
        this.meter = meter;
        this.granularity = granularity;
    }

    /// <summary>The refusal the walk stopped at, when <see cref="Run"/> answered false.</summary>
    internal UbcRefusal Refusal { get; private set; }

    /// <summary>Runs every stage in order; answers the verified program, or false with <see cref="Refusal"/> set.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=163230
    // Broiler-Falsified-If: the stages run in another order than the concept's, or a program is answered after any stage refused
    // Broiler-Human:        PENDING
    internal bool Run(out UbcVerifiedProgram? program)
    {
        program = null;

        // The reader polled at its own granularity and its work since the last poll is not known here,
        // so the walk's count starts from a poll of its own.
        if (!Poll())
        {
            return false;
        }

        if (!CheckHeader() || !CheckFamilies() || !CheckTypesAndUnits() || !CheckJumpTables() || !CheckRegionRows())
        {
            return false;
        }

        // At most one FamilyData section passed the Families check: the one of the one declared slot.
        var familyData = System.ReadOnlyMemory<byte>.Empty;

        foreach (var data in artifact.FamilyData)
        {
            familyData = data.Body.AsMemory();
        }

        if (!Hook(family.Verifier.Begin(new UbcHookArtifact(artifact, slot, table, familyData, new HookMeter(this)), out familyState), UbcRefusal.InSection(slot == 0 ? UbcSectionKind.Families : (UbcSectionKind)UbcFormat.FamilyDataKind(slot), 0)))
        {
            return false;
        }

        var units = artifact.Units;

        if (!Reserve((ulong)units.Length * UnitBytes))
        {
            return false;
        }

        var tablesByUnit = GroupByUnit(artifact.JumpTables.Length, index => artifact.JumpTables[index].Unit);
        var regionsByUnit = GroupByUnit(artifact.Regions.Length, index => artifact.Regions[index].Unit);

        if (tablesByUnit is null || regionsByUnit is null)
        {
            return false;
        }

        // Every table's resolved row, one reference each, is kept by the program: reserved before the
        // array exists, and handed to the program as it is rather than copied.
        if (!Reserve(ArrayBytes + ((ulong)artifact.JumpTables.Length * 8)))
        {
            return false;
        }

        var decoded = new UbcUnitCode[units.Length];
        var jumpTargets = new ImmutableArray<int>[artifact.JumpTables.Length];

        // The longest unit is read as each unit is walked, which that unit's walk has paid for, so the
        // position check needs no pass of its own over the units.
        var longest = 0;

        for (var unit = 0; unit < units.Length; unit++)
        {
            var code = WalkUnit(unit, tablesByUnit[unit], regionsByUnit[unit], jumpTargets);

            if (code is null)
            {
                return false;
            }

            decoded[unit] = code;
            longest = System.Math.Max(longest, code.Instructions.Length);
        }

        if (!CheckEntries(out var entryOrder) || !CheckPositions(decoded, longest))
        {
            return false;
        }

        if (!Hook(family.Verifier.End(familyState), UbcRefusal.InHeader(0)))
        {
            return false;
        }

        // The form layer. At this contract version an image composes the bytecode form alone, and the
        // header's form was checked against the composed set before anything else; the one thing left
        // to refuse is emitted code the bytecode form has no verifier for.
        if (artifact.HasSection(UbcSectionKind.Emission))
        {
            return Invalid(UbcDiagnosticCode.EmissionUnexpected, VmReason.InconsistentStructure, UbcRefusal.InSection(UbcSectionKind.Emission, 0));
        }

        program = new UbcVerifiedProgram(
            artifact,
            table,
            slot,
            familyState,
            System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(decoded),
            System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(jumpTargets),
            entryOrder);
        return true;
    }

    // ---- the structural layer ----------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=C8E773
    // Broiler-Falsified-If: a header naming another profile, another manifest or a form the image does not compose passes
    // Broiler-Human:        PENDING
    private bool CheckHeader()
    {
        var header = artifact.Header;

        if (!string.Equals(header.ProfileIdentity, descriptorProfile.ToString(), System.StringComparison.Ordinal) ||
            !string.Equals(header.ProfileIdentity, family.Identity, System.StringComparison.Ordinal))
        {
            return Invalid(UbcDiagnosticCode.DescriptorProfileMismatch, VmReason.DescriptorMismatch, UbcRefusal.InHeader(0));
        }

        if (!string.Equals(header.ManifestIdentity, descriptorManifest.ToString(), System.StringComparison.Ordinal))
        {
            return Invalid(UbcDiagnosticCode.DescriptorManifestMismatch, VmReason.DescriptorMismatch, UbcRefusal.InHeader(0));
        }

        if (!family.TryGetTable(descriptorManifest, out table))
        {
            return Invalid(UbcDiagnosticCode.ManifestNotComposed, VmReason.UnsupportedFeatureManifest, UbcRefusal.InHeader(0));
        }

        if (!forms.TryGetForm(header.FormIdentity, out _))
        {
            return Invalid(UbcDiagnosticCode.FormNotComposed, VmReason.UnknownFeature, UbcRefusal.InHeader(0));
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=F059FA
    // Broiler-Falsified-If: an artifact declaring a family the image does not compose, a family twice, or FamilyData for an undeclared slot passes
    // Broiler-Human:        PENDING
    private bool CheckFamilies()
    {
        var families = artifact.Families;

        for (var index = 0; index < families.Length; index++)
        {
            var row = families[index];

            if (!string.Equals(row.Identity, family.Identity, System.StringComparison.Ordinal))
            {
                return Invalid(UbcDiagnosticCode.FamilyNotComposed, VmReason.UnknownFeature, UbcRefusal.InRow(UbcSectionKind.Families, index));
            }

            if (index > 0)
            {
                return Invalid(UbcDiagnosticCode.DuplicateFamily, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Families, index));
            }

            if (!string.Equals(row.Manifest, artifact.Header.ManifestIdentity, System.StringComparison.Ordinal))
            {
                return Invalid(UbcDiagnosticCode.FamilyManifestMismatch, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Families, index));
            }

            if (row.TableVersion != table.TableVersion)
            {
                return Invalid(UbcDiagnosticCode.FamilyTableVersionMismatch, VmReason.UnknownFeature, UbcRefusal.InRow(UbcSectionKind.Families, index));
            }

            slot = row.Slot;
        }

        foreach (var data in artifact.FamilyData)
        {
            if (data.Slot != slot || slot == 0)
            {
                return Invalid(UbcDiagnosticCode.FamilyDataForUndeclaredSlot, VmReason.InconsistentStructure, UbcRefusal.InSection((UbcSectionKind)UbcFormat.FamilyDataKind(data.Slot), 0));
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B5AE0C
    // Broiler-Falsified-If: a unit naming a missing signature or an undeclared family, setting a reserved flag, declaring more locals or height than the format admits, or leaving a gap in the code is admitted
    // Broiler-Human:        PENDING
    private bool CheckTypesAndUnits()
    {
        var units = artifact.Units;
        var expected = 0UL;

        if (!Work((ulong)units.Length))
        {
            return false;
        }

        for (var index = 0; index < units.Length; index++)
        {
            var unit = units[index];

            if (unit.TypeIndex >= (uint)artifact.Types.Length)
            {
                return Invalid(UbcDiagnosticCode.TypeIndexOutOfRange, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, index));
            }

            if (unit.FamilySlot != 0 && unit.FamilySlot != slot)
            {
                return Invalid(UbcDiagnosticCode.UnitFamilyUndeclared, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, index));
            }

            const ushort reserved = 0x00FC;
            var flags = (ushort)unit.Flags;

            if ((flags & reserved) != 0 || (unit.FamilySlot == 0 && (flags & (ushort)UbcUnitFlags.FamilyDefined) != 0))
            {
                return Invalid(UbcDiagnosticCode.ReservedUnitFlag, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, index));
            }

            var locals = (ulong)artifact.Types[(int)unit.TypeIndex].Parameters.Length;

            if (!Work((ulong)unit.Locals.Length))
            {
                return false;
            }

            foreach (var run in unit.Locals)
            {
                locals += run.Count;
            }

            if (locals > UbcFormat.MaxLocals || unit.MaxWordHeight > UbcFormat.MaxHeight || unit.MaxValueHeight > UbcFormat.MaxHeight)
            {
                return Invalid(UbcDiagnosticCode.UnitDeclarationTooLarge, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, index));
            }

            if (unit.CodeLength == 0 || unit.CodeOffset != expected)
            {
                return Invalid(UbcDiagnosticCode.CodeNotTiled, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, index));
            }

            expected += unit.CodeLength;

            if (!Work((ulong)unit.Landings.Length))
            {
                return false;
            }

            for (var landing = 1; landing < unit.Landings.Length; landing++)
            {
                if (unit.Landings[landing] <= unit.Landings[landing - 1])
                {
                    return Invalid(UbcDiagnosticCode.LandingsInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, index));
                }
            }
        }

        if (expected != (ulong)artifact.Code.Length)
        {
            return Invalid(UbcDiagnosticCode.CodeNotTiled, VmReason.InconsistentStructure, UbcRefusal.InSection(UbcSectionKind.Code, 0));
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=F2B8DD
    // Broiler-Falsified-If: a jump table of a missing unit, with no target, or beyond the count an operand can name is admitted
    // Broiler-Human:        PENDING
    private bool CheckJumpTables()
    {
        var tables = artifact.JumpTables;

        if (tables.Length > UbcFormat.MaxJumpTables)
        {
            return Invalid(UbcDiagnosticCode.JumpTableMalformed, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.JumpTables, UbcFormat.MaxJumpTables));
        }

        if (!Work((ulong)tables.Length))
        {
            return false;
        }

        for (var index = 0; index < tables.Length; index++)
        {
            if (tables[index].Unit >= (uint)artifact.Units.Length || tables[index].Targets.IsEmpty)
            {
                return Invalid(UbcDiagnosticCode.JumpTableMalformed, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.JumpTables, index));
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=4D34DD
    // Broiler-Falsified-If: a region of a missing unit, with an empty or out-of-unit range, of a unit without a family, or of a kind the table does not define is admitted
    // Broiler-Human:        PENDING
    private bool CheckRegionRows()
    {
        var regions = artifact.Regions;

        if (!Work((ulong)regions.Length))
        {
            return false;
        }

        for (var index = 0; index < regions.Length; index++)
        {
            var region = regions[index];

            if (region.Unit >= (uint)artifact.Units.Length)
            {
                return Invalid(UbcDiagnosticCode.RegionRangeInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Regions, index));
            }

            var unit = artifact.Units[(int)region.Unit];
            var end = (ulong)unit.CodeOffset + unit.CodeLength;

            if (region.Start >= region.End || region.Start < unit.CodeOffset || region.End > end ||
                region.Handler < unit.CodeOffset || region.Handler >= end)
            {
                return Invalid(UbcDiagnosticCode.RegionRangeInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Regions, index));
            }

            if (unit.FamilySlot == 0 || !table.TryGetRegionKind(region.Kind, out _))
            {
                return Invalid(UbcDiagnosticCode.RegionKindUndefined, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Regions, index));
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=03DE23
    // Broiler-Falsified-If: two entries with one name, or an entry of a missing unit or of a unit not flagged as an entry, pass
    // Broiler-Human:        PENDING
    private bool CheckEntries(out ImmutableArray<int> order)
    {
        order = default;
        var entries = artifact.Entries;

        // The unit check and the longest-name pass below, one unit each per entry.
        if (!Work(2 * (ulong)entries.Length))
        {
            return false;
        }

        for (var index = 0; index < entries.Length; index++)
        {
            var unit = entries[index].Unit;

            if (unit >= (uint)artifact.Units.Length || (artifact.Units[(int)unit].Flags & UbcUnitFlags.Entry) == 0)
            {
                return Invalid(UbcDiagnosticCode.EntryUnitInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Entries, index));
            }
        }

        // Sorting compares names byte by byte: charged at twice the comparisons an introspective sort
        // can make, each at the longest name's length in eight-byte steps. The pass that numbers the rows
        // before it and the pass that compares neighbours after it are charged with it, before any runs.
        var longest = 0;

        foreach (var entry in entries)
        {
            longest = System.Math.Max(longest, entry.Name.Length);
        }

        var levels = Depth(entries.Length);
        var steps = 1 + ((ulong)longest / 8);

        if (!Reserve(ArrayBytes + ((ulong)entries.Length * 4)) ||
            !Work((ulong)entries.Length + (2 * (ulong)entries.Length * levels * steps) + ((ulong)entries.Length * steps)))
        {
            return false;
        }

        var sorted = new int[entries.Length];

        for (var index = 0; index < sorted.Length; index++)
        {
            sorted[index] = index;
        }

        System.Array.Sort(sorted, (left, right) =>
        {
            var order = System.MemoryExtensions.SequenceCompareTo(entries[left].Name.AsSpan(), entries[right].Name.AsSpan());
            return order != 0 ? order : left.CompareTo(right);
        });

        for (var index = 1; index < sorted.Length; index++)
        {
            if (System.MemoryExtensions.SequenceEqual(entries[sorted[index]].Name.AsSpan(), entries[sorted[index - 1]].Name.AsSpan()))
            {
                return Invalid(UbcDiagnosticCode.EntryNameInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Entries, System.Math.Max(sorted[index], sorted[index - 1])));
            }
        }

        // The order the program keeps is the array reserved above, handed over rather than copied.
        order = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(sorted);
        return true;
    }

    /// <summary>Every position names a unit and one of its boundaries; <paramref name="longest"/> is the most instructions any unit has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3586E6
    // Broiler-Falsified-If: a position of a missing unit, or at an offset that is not one of its unit's boundaries, passes, or a search runs before it is charged
    // Broiler-Human:        PENDING
    private bool CheckPositions(UbcUnitCode[] units, int longest)
    {
        var positions = artifact.Positions;

        // Each row is one binary search over its unit's boundaries, charged at the longest unit's depth
        // before any runs.
        if (!Work((ulong)positions.Length * Depth(longest)))
        {
            return false;
        }

        for (var index = 0; index < positions.Length; index++)
        {
            var position = positions[index];

            if (position.Unit >= (uint)units.Length)
            {
                return Invalid(UbcDiagnosticCode.PositionInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Positions, index));
            }

            if (units[(int)position.Unit].IndexOf(position.Offset) < 0)
            {
                return Invalid(UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Positions, index));
            }
        }

        return true;
    }

    // ---- the code walk -----------------------------------------------------------------------

    /// <summary>Decodes, walks and hooks one unit; answers its decoded code, or null with the refusal set.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=37237D
    // Broiler-Falsified-If: a unit is answered while an instruction in it is unreachable, a landing is missing or extra, or a region's prefix differs between two covered instructions
    // Broiler-Human:        PENDING
    private UbcUnitCode? WalkUnit(int unitIndex, int[] tableIndices, int[] regionIndices, ImmutableArray<int>[] jumpTargets)
    {
        var unit = artifact.Units[unitIndex];
        var signature = artifact.Types[(int)unit.TypeIndex];

        if (!Decode(unitIndex, unit, out var decoded))
        {
            return null;
        }

        var count = decoded.Length;

        var searches = Depth(count);

        foreach (var tableIndex in tableIndices)
        {
            var targets = artifact.JumpTables[tableIndex].Targets;

            // A binary search per target, and the resolved row kept with its array's own bytes: both
            // paid for before either exists.
            if (!Reserve(ArrayBytes + ((ulong)targets.Length * 4)) || !Work((ulong)targets.Length * searches))
            {
                return null;
            }

            var resolved = new int[targets.Length];

            for (var index = 0; index < targets.Length; index++)
            {
                resolved[index] = IndexOf(decoded, targets[index]);

                if (resolved[index] < 0)
                {
                    return Fail(UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.JumpTables, tableIndex));
                }
            }

            jumpTargets[tableIndex] = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(resolved);
        }

        if (!ResolveRegions(unitIndex, unit, regionIndices, decoded, out var regions, out var innermost, out var parents))
        {
            return null;
        }

        // The unit's local table - its signature's parameters and its own runs - and its signature's
        // counts are built per unit, so they are charged per unit: a signature many units share is
        // paid for by each of them.
        var parameters = (ulong)signature.Parameters.Length;
        var runs = (ulong)unit.Locals.Length;

        if (!Work((2 * parameters) + runs + (ulong)signature.Results.Length) || !Reserve((parameters * 4) + (runs * 16)))
        {
            return null;
        }

        var layout = new LocalLayout(signature, unit);

        // The walk's per-instruction state and the decoded instructions, under one reservation made
        // before either exists; InstructionBytes covers both, and each region's two slots are added.
        // The instructions are handed to the unit as they are, not copied.
        if (!Reserve(((ulong)count * InstructionBytes) + ((ulong)regions.Length * 16)))
        {
            return null;
        }

        var state = new WalkState(count, regions.Length);
        var instructions = new UbcInstruction[count];

        if (!Arrive(state, 0, null, unitIndex, unit, decoded))
        {
            return null;
        }

        while (state.Pending.Count > 0)
        {
            var index = state.Pending.Pop();

            if (!EnterRegions(state, index, innermost, parents, regions, unitIndex, unit, decoded) ||
                !Step(state, index, unitIndex, unit, signature, layout, decoded, instructions, jumpTargets))
            {
                return null;
            }
        }

        if (!Work((ulong)count))
        {
            return null;
        }

        for (var index = 0; index < count; index++)
        {
            if (!state.Reached[index])
            {
                return Fail(UbcDiagnosticCode.UnreachableInstruction, VmReason.InconsistentStructure, UbcRefusal.InCode(unitIndex, decoded[index].Pc));
            }
        }

        if (!CheckRegionPrefixes(state, regions, unitIndex, decoded) || !CheckLandings(unitIndex, unit, decoded, regions))
        {
            return null;
        }

        // The hook pass walks every instruction once more: paid for before it runs. What the hook does
        // at an instruction it charges itself, through the meter it is handed.
        if (!Work((ulong)count))
        {
            return null;
        }

        for (var index = 0; index < count; index++)
        {
            var at = decoded[index];

            if (at.Row is null)
            {
                continue;
            }

            if (!Poll())
            {
                return null;
            }

            var hook = new UbcHookInstruction(unitIndex, at.Pc, at.Opcode, at.Operand, at.Row, state.States[index]);

            if (!Hook(family.Verifier.CheckInstruction(familyState, in hook), UbcRefusal.InCode(unitIndex, at.Pc)))
            {
                return null;
            }
        }

        var layoutCounts = layout.Counts(signature);

        return new UbcUnitCode(
            unitIndex,
            unit,
            signature,
            layout.WordLocals,
            layout.ValueLocals,
            layoutCounts.ParameterWords,
            layoutCounts.ParameterValues,
            layoutCounts.ResultWords,
            layoutCounts.ResultValues,
            System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(instructions),
            regions);
    }

    /// <summary>One instruction as the linear decode found it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8B71BC
    // Broiler-Human:        PENDING
    private readonly struct Raw(uint pc, byte opcode, UbcCommonRow? common, UbcInstructionRow? row, ulong operand)
    {
        internal uint Pc { get; } = pc;

        internal byte Opcode { get; } = opcode;

        internal UbcCommonRow? Common { get; } = common;

        internal UbcInstructionRow? Row { get; } = row;

        internal ulong Operand { get; } = operand;
    }

    /// <summary>Decodes a unit linearly, which fixes its boundaries: every opcode defined, every prefix the unit's own, every operand inside the unit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=0A6973
    // Broiler-Falsified-If: an undefined opcode, the reserved prefixes, another slot's prefix or an operand running past the unit is decoded as an instruction
    // Broiler-Human:        PENDING
    private bool Decode(int unitIndex, UbcUnit unit, out Raw[] decoded)
    {
        decoded = System.Array.Empty<Raw>();
        var code = artifact.Code.AsSpan((int)unit.CodeOffset, (int)unit.CodeLength);
        var count = 0;

        // Two passes over the bytes, the first counting so the array is reserved before it exists.
        for (var pass = 0; pass < 2; pass++)
        {
            var at = 0;
            var index = 0;

            while (at < code.Length)
            {
                if (!Work(1))
                {
                    return false;
                }

                var pc = unit.CodeOffset + (uint)at;
                var opcode = code[at];
                UbcCommonRow? common = null;
                UbcInstructionRow? row = null;
                UbcOperandShape shape;
                var head = 1;

                if (opcode < UbcFormat.FamilyPrefixBase)
                {
                    if (!UbcOpcodes.TryDescribe(opcode, out var described))
                    {
                        return Invalid(UbcDiagnosticCode.UnknownOpcode, VmReason.UnknownFeature, UbcRefusal.InCode(unitIndex, pc));
                    }

                    common = described;
                    shape = described.Shape;
                }
                else
                {
                    if (opcode == UbcFormat.FamilyPrefixBase || opcode == UbcFormat.ExtendedPrefix)
                    {
                        return Invalid(UbcDiagnosticCode.ReservedPrefix, VmReason.UnknownFeature, UbcRefusal.InCode(unitIndex, pc));
                    }

                    if (unit.FamilySlot == 0 || opcode - UbcFormat.FamilyPrefixBase != unit.FamilySlot)
                    {
                        return Invalid(UbcDiagnosticCode.ForeignFamilyInstruction, VmReason.InconsistentStructure, UbcRefusal.InCode(unitIndex, pc));
                    }

                    if (at + 1 >= code.Length)
                    {
                        return Invalid(UbcDiagnosticCode.TruncatedInstruction, VmReason.Truncated, UbcRefusal.InCode(unitIndex, pc));
                    }

                    opcode = code[at + 1];

                    if (!table.TryGetRow(opcode, out var familyRow))
                    {
                        return Invalid(UbcDiagnosticCode.UnknownOpcode, VmReason.UnknownFeature, UbcRefusal.InCode(unitIndex, pc));
                    }

                    row = familyRow;
                    shape = familyRow.Shape;
                    head = 2;
                }

                var width = UbcOperandShapes.Width(shape);

                if (at + head + width > code.Length)
                {
                    return Invalid(UbcDiagnosticCode.TruncatedInstruction, VmReason.Truncated, UbcRefusal.InCode(unitIndex, pc));
                }

                if (pass == 1)
                {
                    decoded[index] = new Raw(pc, opcode, common, row, UbcOperandShapes.Read(code.Slice(at + head, width)));
                }

                index++;
                at += head + width;
            }

            if (pass == 0)
            {
                count = index;

                if (!Reserve((ulong)count * InstructionBytes))
                {
                    return false;
                }

                decoded = new Raw[count];
            }
        }

        return true;
    }

    /// <summary>
    /// Resolves the unit's regions to instruction indices, checks that they nest with every region
    /// before any region enclosing it, and answers each instruction's innermost region and each region's
    /// enclosing one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=D42F2E
    // Broiler-Falsified-If: two regions that overlap without nesting, or an enclosing region listed before one it encloses, are admitted, or a search runs before it is charged
    // Broiler-Human:        PENDING
    private bool ResolveRegions(
        int unitIndex,
        UbcUnit unit,
        int[] regionIndices,
        Raw[] decoded,
        out ImmutableArray<UbcDecodedRegion> resolved,
        out int[] innermost,
        out int[] parents)
    {
        resolved = ImmutableArray<UbcDecodedRegion>.Empty;
        innermost = System.Array.Empty<int>();
        parents = System.Array.Empty<int>();
        var count = regionIndices.Length;

        // Each region's three searches - start, end and handler - at the depth of the instructions; the
        // sort below at two comparisons per level of the regions, with the pass that numbers them; and
        // the nesting pass over the instructions and the regions: all paid for before any runs.
        var levels = Depth(count);
        var searches = Depth(decoded.Length);

        if (!Reserve(((ulong)count * 48) + ((ulong)decoded.Length * 4)) ||
            !Work(((ulong)count * 3 * searches) + ((ulong)count * levels * 2) + (2 * (ulong)count) + (ulong)decoded.Length))
        {
            return false;
        }

        var regions = new UbcDecodedRegion[count];
        var unitEnd = unit.CodeOffset + unit.CodeLength;

        for (var local = 0; local < count; local++)
        {
            var row = artifact.Regions[regionIndices[local]];
            var start = IndexOf(decoded, row.Start);
            var end = row.End == unitEnd ? decoded.Length : IndexOf(decoded, row.End);
            var handler = IndexOf(decoded, row.Handler);

            if (start < 0 || end < 0 || handler < 0)
            {
                return Invalid(UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Regions, regionIndices[local]));
            }

            if (row.WordEntryHeight > UbcFormat.MaxHeight || row.ValueEntryHeight > UbcFormat.MaxHeight)
            {
                return Invalid(UbcDiagnosticCode.RegionEntryInconsistent, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Regions, regionIndices[local]));
            }

            regions[local] = new UbcDecodedRegion(start, end, handler, (int)row.WordEntryHeight, (int)row.ValueEntryHeight, row.Kind);
        }

        // Outer before inner: by start ascending, then end descending, then the later row first, since
        // of two regions with one range the earlier row is the inner one.
        var order = new int[count];

        for (var index = 0; index < count; index++)
        {
            order[index] = index;
        }

        System.Array.Sort(order, (left, right) =>
        {
            var byStart = regions[left].Start.CompareTo(regions[right].Start);

            if (byStart != 0)
            {
                return byStart;
            }

            var byEnd = regions[right].End.CompareTo(regions[left].End);
            return byEnd != 0 ? byEnd : right.CompareTo(left);
        });

        parents = new int[count];
        innermost = new int[decoded.Length];
        var open = new System.Collections.Generic.Stack<int>();
        var next = 0;

        for (var instruction = 0; instruction < decoded.Length; instruction++)
        {
            while (open.Count > 0 && regions[open.Peek()].End <= instruction)
            {
                open.Pop();
            }

            while (next < count && regions[order[next]].Start == instruction)
            {
                var candidate = order[next++];

                while (open.Count > 0 && regions[open.Peek()].End <= regions[candidate].Start)
                {
                    open.Pop();
                }

                if (open.Count > 0)
                {
                    var outer = open.Peek();

                    if (regions[candidate].End > regions[outer].End || candidate > outer)
                    {
                        return Invalid(UbcDiagnosticCode.RegionNesting, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Regions, regionIndices[candidate]));
                    }

                    parents[candidate] = outer;
                }
                else
                {
                    parents[candidate] = -1;
                }

                open.Push(candidate);
            }

            innermost[instruction] = open.Count > 0 ? open.Peek() : -1;
        }

        resolved = ImmutableArray.Create(regions);
        return true;
    }

    /// <summary>What the walk keeps for one unit while it types it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EE5E60
    // Broiler-Human:        PENDING
    private sealed class WalkState
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=776EBF
        // Broiler-Human:        PENDING
        internal WalkState(int instructions, int regions)
        {
            States = new UbcStackNode?[instructions];
            Reached = new bool[instructions];
            Prefixes = new UbcStackNode?[regions];
            PrefixSet = new bool[regions];
        }

        internal UbcStackNode?[] States { get; }

        internal bool[] Reached { get; }

        internal UbcStackNode?[] Prefixes { get; }

        internal bool[] PrefixSet { get; }

        internal System.Collections.Generic.Stack<int> Pending { get; } = new();
    }

    /// <summary>An arrival at an instruction with a state: the first fixes it, every later one must equal it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=8BCDD6
    // Broiler-Falsified-If: a second arrival with a different typed stack is accepted, or an arrival above the unit's declared heights is admitted
    // Broiler-Human:        PENDING
    private bool Arrive(WalkState state, int index, UbcStackNode? stack, int unitIndex, UbcUnit unit, Raw[] decoded)
    {
        if (state.Reached[index])
        {
            return Same(state.States[index], stack)
                || Or(UbcDiagnosticCode.JoinMismatch, UbcRefusal.InCode(unitIndex, decoded[index].Pc));
        }

        if ((stack?.Words ?? 0) > unit.MaxWordHeight || (stack?.Values ?? 0) > unit.MaxValueHeight)
        {
            return Invalid(UbcDiagnosticCode.HeightAboveDeclared, VmReason.InconsistentStructure, UbcRefusal.InCode(unitIndex, decoded[index].Pc));
        }

        state.Reached[index] = true;
        state.States[index] = stack;
        state.Pending.Push(index);
        return true;
    }

    /// <summary>
    /// For every region covering the instruction whose prefix is not yet fixed, fixes it from the
    /// instruction's state and reaches the handler. Enclosing regions are fixed with the innermost, so
    /// the chain stops at the first region already fixed.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=4AFD75
    // Broiler-Falsified-If: a handler is reached with a state other than the region's prefix and its kind's landing pushes
    // Broiler-Human:        PENDING
    private bool EnterRegions(WalkState state, int index, int[] innermost, int[] parents, ImmutableArray<UbcDecodedRegion> regions, int unitIndex, UbcUnit unit, Raw[] decoded)
    {
        if (innermost.Length == 0)
        {
            return true;
        }

        for (var region = innermost[index]; region >= 0 && !state.PrefixSet[region]; region = parents[region])
        {
            if (!Prefix(state.States[index], regions[region], out var prefix))
            {
                return Or(UbcDiagnosticCode.RegionEntryInconsistent, UbcRefusal.InCode(unitIndex, decoded[index].Pc));
            }

            state.PrefixSet[region] = true;
            state.Prefixes[region] = prefix;
            table.TryGetRegionKind(regions[region].Kind, out var kind);

            var handlerAt = UbcRefusal.InCode(unitIndex, decoded[regions[region].Handler].Pc);

            if (!Push(prefix, kind.LandingPushes, unit, handlerAt, out var landed) || !Arrive(state, regions[region].Handler, landed, unitIndex, unit, decoded))
            {
                return false;
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=53792D
    // Broiler-Falsified-If: a covered instruction whose bottom slots differ from its region's prefix passes
    // Broiler-Human:        PENDING
    private bool CheckRegionPrefixes(WalkState state, ImmutableArray<UbcDecodedRegion> regions, int unitIndex, Raw[] decoded)
    {
        for (var region = 0; region < regions.Length; region++)
        {
            for (var index = regions[region].Start; index < regions[region].End; index++)
            {
                if (!Prefix(state.States[index], regions[region], out var prefix) || !Same(prefix, state.Prefixes[region]))
                {
                    return Or(UbcDiagnosticCode.RegionEntryInconsistent, UbcRefusal.InCode(unitIndex, decoded[index].Pc));
                }
            }
        }

        return true;
    }

    /// <summary>
    /// The bottom slots of <paramref name="stack"/> the region's entry heights name, when they are
    /// exactly that many words and values; charged for the slots walked past.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B7F7A2
    // Broiler-Falsified-If: a prefix is answered whose words or values differ from the region's entry heights
    // Broiler-Human:        PENDING
    private bool Prefix(UbcStackNode? stack, UbcDecodedRegion region, out UbcStackNode? prefix)
    {
        prefix = null;
        var length = region.WordEntryHeight + region.ValueEntryHeight;
        var height = stack?.Height ?? 0;

        if (length > height || !Work((ulong)(height - length) + 1))
        {
            return false;
        }

        var node = stack;

        for (var step = 0; step < height - length; step++)
        {
            node = node!.Below;
        }

        if ((node?.Words ?? 0) != region.WordEntryHeight || (node?.Values ?? 0) != region.ValueEntryHeight)
        {
            return false;
        }

        prefix = node;
        return true;
    }

    /// <summary>
    /// The unit's landings are exactly its resume points - the instruction after every suspending row
    /// that falls through - and its regions' handlers, ascending.
    /// </summary>
    /// <remarks>
    /// The unit check refused landings that are not strictly ascending, so each offset the unit requires is
    /// found by a binary search over its landings and marked; the landings match when every requirement
    /// is found and every landing is marked.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6DABBD
    // Broiler-Falsified-If: a unit whose landings omit a resume point or a handler, or name any other offset, passes, or a search runs before it is charged
    // Broiler-Human:        PENDING
    private bool CheckLandings(int unitIndex, UbcUnit unit, Raw[] decoded, ImmutableArray<UbcDecodedRegion> regions)
    {
        var landings = unit.Landings;

        // The marks, one per landing, reserved before they exist; the passes over the instructions, the
        // handlers and the marks paid for before they run.
        if (!Reserve(ArrayBytes + (ulong)landings.Length) ||
            !Work((ulong)decoded.Length + (ulong)regions.Length + (ulong)landings.Length))
        {
            return false;
        }

        var marked = new bool[landings.Length];
        var depth = Depth(landings.Length);

        for (var index = 0; index < decoded.Length; index++)
        {
            if (decoded[index].Row is { Kind: UbcInstructionKind.Suspend, IsTerminal: false } && index + 1 < decoded.Length &&
                !Require(landings, marked, depth, decoded[index + 1].Pc, unitIndex))
            {
                return false;
            }
        }

        foreach (var region in regions)
        {
            if (!Require(landings, marked, depth, decoded[region.Handler].Pc, unitIndex))
            {
                return false;
            }
        }

        foreach (var mark in marked)
        {
            if (!mark)
            {
                return Invalid(UbcDiagnosticCode.LandingsInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, unitIndex));
            }
        }

        return true;
    }

    /// <summary>Marks the landing at <paramref name="pc"/>, refusing the unit when it lists none there; the search is charged at <paramref name="depth"/> before it runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A88DBE
    // Broiler-Falsified-If: an offset the landings do not list is marked, or the search runs before it is charged
    // Broiler-Human:        PENDING
    private bool Require(ImmutableArray<uint> landings, bool[] marked, ulong depth, uint pc, int unitIndex)
    {
        if (!Work(depth))
        {
            return false;
        }

        var at = System.MemoryExtensions.BinarySearch(landings.AsSpan(), pc);

        if (at < 0)
        {
            return Invalid(UbcDiagnosticCode.LandingsInvalid, VmReason.InconsistentStructure, UbcRefusal.InRow(UbcSectionKind.Units, unitIndex));
        }

        marked[at] = true;
        return true;
    }

    /// <summary>Applies one instruction to the typed stack, records what the walk proved, and reaches its successors.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=2; Fingerprint=4A7F1C
    // Broiler-Falsified-If: an instruction reaches a successor with a stack other than its effect applied, or is recorded with counts that differ from that effect
    // Broiler-Human:        PENDING
    private bool Step(
        WalkState state,
        int index,
        int unitIndex,
        UbcUnit unit,
        UbcSignature signature,
        LocalLayout layout,
        Raw[] decoded,
        UbcInstruction[] instructions,
        ImmutableArray<int>[] jumpTargets)
    {
        var raw = decoded[index];
        var before = state.States[index];
        var at = UbcRefusal.InCode(unitIndex, raw.Pc);
        var next = index + 1 < decoded.Length ? index + 1 : -1;

        if (!Work(1))
        {
            return false;
        }

        if (raw.Row is { } row)
        {
            return StepFamily(state, index, unitIndex, unit, decoded, instructions, row, before, at, next);
        }

        var common = raw.Common!;
        var operand = raw.Operand;
        UbcStackNode? after;
        int target = -1, resolved = -1;
        var plane = UbcPlane.Word;
        int wordPops = 0, valuePops = 0, wordPushes = 0, valuePushes = 0;
        var falls = !common.IsTerminal;

        switch (common.Effect)
        {
            case UbcCommonEffect.Listed:
            {
                if (!Pop(before, common.Pops, at, out var rest) || !Push(rest, common.Pushes, unit, at, out after))
                {
                    return false;
                }

                Count(common.Pops, out wordPops, out valuePops);
                Count(common.Pushes, out wordPushes, out valuePushes);

                if (common.HasCodeTarget && !Target(decoded, operand, at, out target))
                {
                    return false;
                }

                break;
            }

            case UbcCommonEffect.Trap:
            {
                var trapSlot = UbcOperandShapes.First(common.Shape, operand);
                var trapCode = (ushort)UbcOperandShapes.Second(common.Shape, operand);
                var defined = trapSlot == 0 ? trapCode == 0 : trapSlot == slot && slot != 0 && table.DefinesTrap(trapCode);

                if (!defined)
                {
                    return Invalid(UbcDiagnosticCode.TrapUndefined, VmReason.InconsistentStructure, at);
                }

                after = before;
                break;
            }

            case UbcCommonEffect.Jump:
                if (!Target(decoded, operand, at, out target))
                {
                    return false;
                }

                after = before;
                break;

            case UbcCommonEffect.JumpTable:
            {
                if (!Pop(before, common.Pops, at, out after))
                {
                    return false;
                }

                Count(common.Pops, out wordPops, out valuePops);

                if (operand >= (ulong)artifact.JumpTables.Length || artifact.JumpTables[(int)operand].Unit != (uint)unitIndex)
                {
                    return Invalid(UbcDiagnosticCode.JumpTableInvalid, VmReason.InconsistentStructure, at);
                }

                resolved = (int)operand;

                if (!Work((ulong)jumpTargets[resolved].Length))
                {
                    return false;
                }

                foreach (var successor in jumpTargets[resolved])
                {
                    if (!Arrive(state, successor, after, unitIndex, unit, decoded))
                    {
                        return false;
                    }
                }

                break;
            }

            case UbcCommonEffect.Return:
                if (!Pop(before, signature.Results, at, out after))
                {
                    return false;
                }

                Count(signature.Results, out wordPops, out valuePops);
                break;

            case UbcCommonEffect.Call:
            {
                if (operand >= (ulong)artifact.Units.Length)
                {
                    return Invalid(UbcDiagnosticCode.CallTargetOutOfRange, VmReason.InconsistentStructure, at);
                }

                var callee = artifact.Types[(int)artifact.Units[(int)operand].TypeIndex];

                if (!Pop(before, callee.Parameters, at, out var rest) || !Push(rest, callee.Results, unit, at, out after))
                {
                    return false;
                }

                resolved = (int)operand;
                Count(callee.Parameters, out wordPops, out valuePops);
                Count(callee.Results, out wordPushes, out valuePushes);
                break;
            }

            case UbcCommonEffect.Drop:
            case UbcCommonEffect.Dup:
            case UbcCommonEffect.Dup2:
            case UbcCommonEffect.Swap:
            {
                var depth = common.Effect == UbcCommonEffect.Dup2 || common.Effect == UbcCommonEffect.Swap ? 2 : 1;

                if (!Take(before, depth, at, out var top, out var rest))
                {
                    return false;
                }

                wordPops = before!.Words - (rest?.Words ?? 0);
                valuePops = before.Values - (rest?.Values ?? 0);

                switch (common.Effect)
                {
                    case UbcCommonEffect.Drop:
                        after = rest;
                        break;

                    case UbcCommonEffect.Swap:
                        if (!Push(rest, ImmutableArray.Create(top[1], top[0]), unit, at, out after))
                        {
                            return false;
                        }

                        wordPushes = wordPops;
                        valuePushes = valuePops;
                        break;

                    default:
                        if (!Push(before, top, unit, at, out after))
                        {
                            return false;
                        }

                        wordPushes = 2 * wordPops;
                        valuePushes = 2 * valuePops;
                        break;
                }

                break;
            }

            case UbcCommonEffect.Pick:
            {
                if (operand >= (ulong)(before?.Height ?? 0) || !Work(operand + 1))
                {
                    return Or(UbcDiagnosticCode.StackUnderflow, at);
                }

                var top = before!;
                var node = top;

                for (var step = 0UL; step < operand; step++)
                {
                    node = node.Below!;
                }

                plane = UbcSlotTypes.PlaneOf(node.Type);
                resolved = plane == UbcPlane.Word ? top.Words - node.Words : top.Values - node.Values;

                if (!Push(before, ImmutableArray.Create(node.Type), unit, at, out after))
                {
                    return false;
                }

                wordPushes = plane == UbcPlane.Word ? 1 : 0;
                valuePushes = 1 - wordPushes;
                break;
            }

            case UbcCommonEffect.Select:
            {
                if (!Pop(before, ImmutableArray.Create(UbcSlotType.I32), at, out var candidates) ||
                    !Take(candidates, 2, at, out var pair, out var rest))
                {
                    return false;
                }

                if (pair[0] != pair[1])
                {
                    return Invalid(UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, at);
                }

                var word = pair[0] != UbcSlotType.V;
                wordPops = 1 + (word ? 2 : 0);
                valuePops = word ? 0 : 2;
                wordPushes = word ? 1 : 0;
                valuePushes = word ? 0 : 1;

                if (!Push(rest, ImmutableArray.Create(pair[0]), unit, at, out after))
                {
                    return false;
                }

                break;
            }

            case UbcCommonEffect.Squash:
            {
                var discard = (int)UbcOperandShapes.First(common.Shape, operand);
                var keep = (int)UbcOperandShapes.Second(common.Shape, operand);

                if (!Take(before, keep, at, out var kept, out var middle) || !Take(middle, discard, at, out _, out var rest))
                {
                    return false;
                }

                wordPushes = (before?.Words ?? 0) - (middle?.Words ?? 0);
                valuePushes = (before?.Values ?? 0) - (middle?.Values ?? 0);
                wordPops = (before?.Words ?? 0) - (rest?.Words ?? 0);
                valuePops = (before?.Values ?? 0) - (rest?.Values ?? 0);

                if (!Push(rest, kept, unit, at, out after))
                {
                    return false;
                }

                break;
            }

            case UbcCommonEffect.LocalGet:
            case UbcCommonEffect.LocalSet:
            case UbcCommonEffect.LocalTee:
            {
                // The local's run is found by a binary search over the unit's runs, charged at their
                // depth before it runs.
                if (!Work(Depth(unit.Locals.Length)))
                {
                    return false;
                }

                if (!layout.TryGet(operand, out var type, out resolved))
                {
                    return Invalid(UbcDiagnosticCode.LocalIndexOutOfRange, VmReason.InconsistentStructure, at);
                }

                plane = UbcSlotTypes.PlaneOf(type);
                var one = ImmutableArray.Create(type);
                var isWord = plane == UbcPlane.Word ? 1 : 0;

                if (common.Effect == UbcCommonEffect.LocalGet)
                {
                    if (!Push(before, one, unit, at, out after))
                    {
                        return false;
                    }

                    wordPushes = isWord;
                    valuePushes = 1 - isWord;
                }
                else
                {
                    if (!Pop(before, one, at, out var rest))
                    {
                        return false;
                    }

                    wordPops = isWord;
                    valuePops = 1 - isWord;
                    after = rest;

                    if (common.Effect == UbcCommonEffect.LocalTee)
                    {
                        after = before;
                        wordPushes = wordPops;
                        valuePushes = valuePops;
                    }
                }

                break;
            }

            default:
                return Invalid(UbcDiagnosticCode.VerifierDefect, VmReason.InconsistentStructure, at);
        }

        if (target >= 0 && !Arrive(state, target, after, unitIndex, unit, decoded))
        {
            return false;
        }

        if (falls)
        {
            if (next < 0)
            {
                return Invalid(UbcDiagnosticCode.FallsOffTheEnd, VmReason.InconsistentStructure, at);
            }

            if (!Arrive(state, next, after, unitIndex, unit, decoded))
            {
                return false;
            }
        }

        instructions[index] = new UbcInstruction(
            raw.Pc, raw.Opcode, null, operand, falls ? next : -1, target, resolved, plane,
            before?.Words ?? 0, before?.Values ?? 0, wordPops, valuePops, wordPushes, valuePushes, 0, 0);
        return true;
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=1; Fingerprint=08DCB4
    // Broiler-Falsified-If: a family row is applied with other pops than its effect names for its operand, a suspending row passes outside a suspendable unit, or a branch's taken edge carries the fall-through's pushes when the row names its own
    // Broiler-Human:        PENDING
    private bool StepFamily(
        WalkState state,
        int index,
        int unitIndex,
        UbcUnit unit,
        Raw[] decoded,
        UbcInstruction[] instructions,
        UbcInstructionRow row,
        UbcStackNode? before,
        VmSourcePosition at,
        int next)
    {
        var raw = decoded[index];
        var effect = row.Effect;

        if (row.Kind == UbcInstructionKind.Suspend && (unit.Flags & UbcUnitFlags.Suspendable) == 0)
        {
            return Invalid(UbcDiagnosticCode.SuspendOutsideSuspendableUnit, VmReason.InconsistentStructure, at);
        }

        if (!effect.TryGetPopCount(raw.Operand, out var popCount))
        {
            return Invalid(UbcDiagnosticCode.CountedOperandTooLarge, VmReason.InconsistentStructure, at);
        }

        UbcStackNode? rest;

        if (effect.Form == UbcEffectForm.Listed)
        {
            if (!Pop(before, effect.Pops, at, out rest))
            {
                return false;
            }
        }
        else
        {
            var run = popCount - effect.Pops.Length;

            if (!Repeat(before, effect.Repeated, run, at, out var beneath) || !Pop(beneath, effect.Pops, at, out rest))
            {
                return false;
            }
        }

        var wordPops = (before?.Words ?? 0) - (rest?.Words ?? 0);
        var valuePops = (before?.Values ?? 0) - (rest?.Values ?? 0);
        Count(effect.Pushes, out var wordPushes, out var valuePushes);

        if (!Push(rest, effect.Pushes, unit, at, out var after))
        {
            return false;
        }

        var target = -1;
        int takenWords = 0, takenValues = 0;

        if (row.Target.IsCode)
        {
            if (!Target(decoded, raw.Operand, at, out target))
            {
                return false;
            }

            var taken = row.Target.HasDistinctTakenEdge ? row.Target.TakenPushes : effect.Pushes;
            Count(taken, out takenWords, out takenValues);

            if (!Push(rest, taken, unit, at, out var landed) || !Arrive(state, target, landed, unitIndex, unit, decoded))
            {
                return false;
            }
        }

        if (!row.IsTerminal)
        {
            if (next < 0)
            {
                return Invalid(UbcDiagnosticCode.FallsOffTheEnd, VmReason.InconsistentStructure, at);
            }

            if (!Arrive(state, next, after, unitIndex, unit, decoded))
            {
                return false;
            }
        }

        instructions[index] = new UbcInstruction(
            raw.Pc, raw.Opcode, row, raw.Operand, row.IsTerminal ? -1 : next, target, -1, UbcPlane.Word,
            before?.Words ?? 0, before?.Values ?? 0, wordPops, valuePops, wordPushes, valuePushes, takenWords, takenValues);
        return true;
    }

    // ---- the typed stack -----------------------------------------------------------------------

    /// <summary>Pops <paramref name="types"/>, the last the top, checking each; charged per slot.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=8BD449
    // Broiler-Falsified-If: a slot of another type than the list names is popped, or a pop past the bottom answers true
    // Broiler-Human:        PENDING
    private bool Pop(UbcStackNode? stack, ImmutableArray<UbcSlotType> types, VmSourcePosition at, out UbcStackNode? rest)
    {
        rest = stack;

        if (types.Length > (stack?.Height ?? 0))
        {
            return Invalid(UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, at);
        }

        if (!Work((ulong)types.Length))
        {
            return false;
        }

        for (var index = types.Length - 1; index >= 0; index--)
        {
            if (rest!.Type != types[index])
            {
                return Invalid(UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, at);
            }

            rest = rest.Below;
        }

        return true;
    }

    /// <summary>Pops a run of <paramref name="count"/> slots of one type.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=FE337A
    // Broiler-Falsified-If: a run holding a slot of another type is popped
    // Broiler-Human:        PENDING
    private bool Repeat(UbcStackNode? stack, UbcSlotType type, int count, VmSourcePosition at, out UbcStackNode? rest)
    {
        rest = stack;

        if (count > (stack?.Height ?? 0))
        {
            return Invalid(UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, at);
        }

        if (!Work((ulong)count))
        {
            return false;
        }

        for (var index = 0; index < count; index++)
        {
            if (rest!.Type != type)
            {
                return Invalid(UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, at);
            }

            rest = rest.Below;
        }

        return true;
    }

    /// <summary>Takes the top <paramref name="count"/> slots whatever their types, answering them bottom to top.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=C8FF1A
    // Broiler-Falsified-If: the slots answered are not the stack's top slots bottom to top, or a take past the bottom answers true
    // Broiler-Human:        PENDING
    private bool Take(UbcStackNode? stack, int count, VmSourcePosition at, out ImmutableArray<UbcSlotType> taken, out UbcStackNode? rest)
    {
        taken = ImmutableArray<UbcSlotType>.Empty;
        rest = stack;

        if (count > (stack?.Height ?? 0))
        {
            return Invalid(UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, at);
        }

        if (!Work((ulong)count) || !Reserve((ulong)count))
        {
            return false;
        }

        var types = new UbcSlotType[count];

        for (var index = count - 1; index >= 0; index--)
        {
            types[index] = rest!.Type;
            rest = rest.Below;
        }

        taken = System.Runtime.InteropServices.ImmutableCollectionsMarshal.AsImmutableArray(types);
        return true;
    }

    /// <summary>
    /// Pushes <paramref name="types"/>, the last the top, refusing first when either plane would rise
    /// above the unit's declared height, so a signature with many results cannot spend memory to be
    /// refused; each slot is charged as work and as retained bytes before it is built.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=2257D0
    // Broiler-Falsified-If: a slot is created before its work and bytes are charged, or a push past a declared height builds a stack
    // Broiler-Human:        PENDING
    private bool Push(UbcStackNode? stack, ImmutableArray<UbcSlotType> types, UbcUnit unit, VmSourcePosition at, out UbcStackNode? after)
    {
        after = stack;

        if (types.Length == 0)
        {
            return true;
        }

        Count(types, out var words, out var values);

        if ((long)(stack?.Words ?? 0) + words > unit.MaxWordHeight || (long)(stack?.Values ?? 0) + values > unit.MaxValueHeight)
        {
            return Invalid(UbcDiagnosticCode.HeightAboveDeclared, VmReason.InconsistentStructure, at);
        }

        if (!Work((ulong)types.Length) || !Reserve((ulong)types.Length * NodeBytes))
        {
            return false;
        }

        foreach (var type in types)
        {
            after = new UbcStackNode(type, after);
        }

        return true;
    }

    /// <summary>Two stacks are equal slot for slot; shared tails compare at once. Charged for the slots compared.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=0D6F45
    // Broiler-Falsified-If: two stacks differing in any slot's type or in height compare equal
    // Broiler-Human:        PENDING
    private bool Same(UbcStackNode? left, UbcStackNode? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if ((left?.Height ?? 0) != (right?.Height ?? 0))
        {
            return false;
        }

        if (!Work((ulong)(left?.Height ?? 0)))
        {
            return false;
        }

        while (!ReferenceEquals(left, right))
        {
            if (left is null || right is null || left.Type != right.Type)
            {
                return false;
            }

            left = left.Below;
            right = right.Below;
        }

        return true;
    }

    /// <summary>Resolves a code target to its instruction; the binary search is charged at the depth of the unit's instructions before it runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=60AF70
    // Broiler-Falsified-If: a target outside the unit or between two boundaries resolves to an instruction, or the search runs before it is charged
    // Broiler-Human:        PENDING
    private bool Target(Raw[] decoded, ulong operand, VmSourcePosition at, out int target)
    {
        target = -1;

        if (!Work(Depth(decoded.Length)))
        {
            return false;
        }

        target = operand > uint.MaxValue ? -1 : IndexOf(decoded, (uint)operand);

        return target >= 0 || Invalid(UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, at);
    }

    /// <summary>The probes a binary search over <paramref name="length"/> sorted items makes at most, and the levels of a sort of them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1D9A9B
    // Broiler-Human:        PENDING
    private static ulong Depth(int length) => (ulong)System.Numerics.BitOperations.Log2((uint)length + 1) + 1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=9B8B49
    // Broiler-Human:        PENDING
    private static void Count(ImmutableArray<UbcSlotType> types, out int words, out int values)
    {
        words = 0;
        values = 0;

        foreach (var type in types)
        {
            if (type == UbcSlotType.V)
            {
                values++;
            }
            else
            {
                words++;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0A22CA
    // Broiler-Human:        PENDING
    private static int IndexOf(Raw[] decoded, uint pc)
    {
        var low = 0;
        var high = decoded.Length - 1;

        while (low <= high)
        {
            var middle = low + ((high - low) / 2);

            if (decoded[middle].Pc == pc)
            {
                return middle;
            }

            if (decoded[middle].Pc < pc)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return -1;
    }

    /// <summary>The rows of a section grouped by the unit each names, in section order within a unit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48D1D1
    // Broiler-Falsified-If: a row is placed under a unit it does not name, or two rows of one unit change order
    // Broiler-Human:        PENDING
    private int[][]? GroupByUnit(int count, System.Func<int, uint> unitOf)
    {
        // Every caller's rows had their unit checked against the Units section already.
        var units = artifact.Units.Length;

        if (!Reserve(((ulong)units * 40) + ((ulong)count * 4)) || !Work((ulong)units + (2 * (ulong)count)))
        {
            return null;
        }

        var sizes = new int[units];

        for (var index = 0; index < count; index++)
        {
            sizes[unitOf(index)]++;
        }

        var groups = new int[units][];

        for (var unit = 0; unit < units; unit++)
        {
            groups[unit] = sizes[unit] == 0 ? System.Array.Empty<int>() : new int[sizes[unit]];
            sizes[unit] = 0;
        }

        for (var index = 0; index < count; index++)
        {
            var unit = unitOf(index);
            groups[unit][sizes[unit]++] = index;
        }

        return groups;
    }

    // ---- the local table -----------------------------------------------------------------------

    /// <summary>Where each local of a unit lives: its type and its index within its plane.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=2C752C
    // Broiler-Falsified-If: a local is answered with a type or a plane index other than its signature's or its run's
    // Broiler-Human:        PENDING
    private sealed class LocalLayout
    {
        private readonly ImmutableArray<UbcSlotType> parameters;
        private readonly int[] parameterIndex;
        private readonly ImmutableArray<UbcLocalRun> runs;
        private readonly long[] runStart;
        private readonly int[] runWordsBefore;
        private readonly int[] runValuesBefore;
        private readonly long total;

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8D469A
        // Broiler-Falsified-If: a local is placed at a plane index another local of the same plane already has
        // Broiler-Human:        PENDING
        internal LocalLayout(UbcSignature signature, UbcUnit unit)
        {
            parameters = signature.Parameters;
            parameterIndex = new int[parameters.Length];
            var words = 0;
            var values = 0;

            for (var index = 0; index < parameters.Length; index++)
            {
                parameterIndex[index] = parameters[index] == UbcSlotType.V ? values++ : words++;
            }

            runs = unit.Locals;
            runStart = new long[runs.Length];
            runWordsBefore = new int[runs.Length];
            runValuesBefore = new int[runs.Length];
            long start = parameters.Length;

            for (var index = 0; index < runs.Length; index++)
            {
                runStart[index] = start;
                runWordsBefore[index] = words;
                runValuesBefore[index] = values;
                start += runs[index].Count;

                if (runs[index].Type == UbcSlotType.V)
                {
                    values += (int)runs[index].Count;
                }
                else
                {
                    words += (int)runs[index].Count;
                }
            }

            total = start;
            WordLocals = words;
            ValueLocals = values;
        }

        internal int WordLocals { get; }

        internal int ValueLocals { get; }

        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=F52618
        // Broiler-Falsified-If: an index at or past the local count answers a type
        // Broiler-Human:        PENDING
        internal bool TryGet(ulong local, out UbcSlotType type, out int planeIndex)
        {
            type = UbcSlotType.I32;
            planeIndex = -1;

            if (local >= (ulong)total)
            {
                return false;
            }

            if (local < (ulong)parameters.Length)
            {
                type = parameters[(int)local];
                planeIndex = parameterIndex[(int)local];
                return true;
            }

            var low = 0;
            var high = runs.Length - 1;

            while (low < high)
            {
                var middle = low + ((high - low + 1) / 2);

                if (runStart[middle] <= (long)local)
                {
                    low = middle;
                }
                else
                {
                    high = middle - 1;
                }
            }

            // Runs of length zero share a start with the run after them; the search lands on the last
            // run starting at or before the local, which is the one holding it.
            type = runs[low].Type;
            var offset = (int)((long)local - runStart[low]);
            planeIndex = (type == UbcSlotType.V ? runValuesBefore[low] : runWordsBefore[low]) + offset;
            return true;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=995E5C
        // Broiler-Human:        PENDING
        internal (int ParameterWords, int ParameterValues, int ResultWords, int ResultValues) Counts(UbcSignature signature)
        {
            Count(signature.Parameters, out var parameterWords, out var parameterValues);
            Count(signature.Results, out var resultWords, out var resultValues);
            return (parameterWords, parameterValues, resultWords, resultValues);
        }
    }

    // ---- the family hook -----------------------------------------------------------------------

    /// <summary>Turns a hook's answer into the walk's; a hook that answers with the universal range or with no invalid-artifact reason is a defect.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=44F6D0
    // Broiler-Falsified-If: a refusal of the hook is answered as an admission, or a hook code in the universal range reaches the outcome as the hook's
    // Broiler-Human:        PENDING
    private bool Hook(UbcHookAnswer answer, VmSourcePosition at)
    {
        if (Refused())
        {
            // The hook's meter refused a charge and the hook admitted anyway: the stop stands.
            return false;
        }

        if (!answer.Refused)
        {
            return true;
        }

        if (answer.Exhausted is { } dimension)
        {
            Refusal = UbcRefusal.Exhausted(dimension);
            return false;
        }

        var universal = answer.FamilyCode is >= 3000 and <= 3999;
        var invalidReason = (int)answer.Reason is > (int)VmReason.InvalidArtifactUnspecified and < (int)VmReason.InvalidStateUnspecified;

        if (universal || !invalidReason)
        {
            return Invalid(UbcDiagnosticCode.VerifierDefect, VmReason.InconsistentStructure, at);
        }

        Refusal = UbcRefusal.InvalidByFamily(answer.FamilyCode, answer.Reason, at);
        return false;
    }

    /// <summary>
    /// The meter the hook is handed: its work - verifier work and fuel, the two dimensions the core counts
    /// toward the poll bound - is counted with the walk's, so one bound governs both.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=922DD1
    // Broiler-Falsified-If: a charge of work the hook makes reaches the meter without passing the walk's count of work since the last poll
    // Broiler-Human:        PENDING
    private sealed class HookMeter(UbcWalk walk) : IVmMeter
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B96125
        // Broiler-Falsified-If: verifier work or fuel the hook charges escapes the walk's count of work since the last poll, or its refusal names another dimension
        // Broiler-Human:        PENDING
        public bool TryCharge(VmBudgetDimension dimension, ulong amount) =>
            dimension is VmBudgetDimension.VerifierWork or VmBudgetDimension.Fuel
                ? walk.Work(amount, dimension)
                : walk.meter.TryCharge(dimension, amount);

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3E81D7
        // Broiler-Human:        PENDING
        public bool Poll() => walk.Poll();

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FDDC7E
        // Broiler-Human:        PENDING
        public void ReportRetained(VmBudgetDimension dimension, ulong amount) => walk.meter.ReportRetained(dimension, amount);

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A38EE4
        // Broiler-Human:        PENDING
        public void ReportReleased(VmBudgetDimension dimension, ulong amount) => walk.meter.ReportReleased(dimension, amount);
    }

    // ---- charging --------------------------------------------------------------------------------

    /// <summary>
    /// Charges <paramref name="units"/> of work to <paramref name="dimension"/> before it is done - verifier
    /// work, or fuel a family hook charges, both of which the core counts toward the poll bound - in
    /// pieces that never let the work since the last poll pass the bound, polling whenever it reaches it.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=0; Fingerprint=392571
    // Broiler-Falsified-If: work since the last poll can exceed the family's uncharged-work bound, or a refused charge is answered as success or as an exhaustion of another dimension
    // Broiler-Human:        PENDING
    private bool Work(ulong units, VmBudgetDimension dimension = VmBudgetDimension.VerifierWork)
    {
        while (units > 0)
        {
            var piece = System.Math.Min(units, granularity - uncharged);

            if (!meter.TryCharge(dimension, piece))
            {
                Refusal = UbcRefusal.Exhausted(dimension);
                return false;
            }

            uncharged += piece;
            units -= piece;

            if (uncharged == granularity && !Poll())
            {
                return false;
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4CED9D
    // Broiler-Falsified-If: a false poll is answered as anything but a verifier-work stop
    // Broiler-Human:        PENDING
    private bool Poll()
    {
        uncharged = 0;

        if (!meter.Poll())
        {
            Refusal = UbcRefusal.Exhausted(VmBudgetDimension.VerifierWork);
            return false;
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=6BCE88
    // Broiler-Falsified-If: an allocation the walk keeps is made before its bytes are charged
    // Broiler-Human:        PENDING
    private bool Reserve(ulong bytes)
    {
        if (bytes == 0 || meter.TryCharge(VmBudgetDimension.AllocatedBytes, bytes))
        {
            return true;
        }

        Refusal = UbcRefusal.Exhausted(VmBudgetDimension.AllocatedBytes);
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4653BA
    // Broiler-Human:        PENDING
    private bool Refused() => Refusal.Kind == UbcRefusalKind.Exhausted || Refusal.Code != 0;

    /// <summary>
    /// Refuses with <paramref name="code"/> unless a charge inside the check that failed has already
    /// stopped the walk, whose refusal then stands.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F23F01
    // Broiler-Falsified-If: an exhaustion met inside a comparison is answered as an invalid artifact
    // Broiler-Human:        PENDING
    private bool Or(UbcDiagnosticCode code, VmSourcePosition position) =>
        !Refused() && Invalid(code, VmReason.InconsistentStructure, position);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2F8DC8
    // Broiler-Human:        PENDING
    private bool Invalid(UbcDiagnosticCode code, VmReason reason, VmSourcePosition position)
    {
        Refusal = UbcRefusal.Invalid(code, reason, position);
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3449ED
    // Broiler-Human:        PENDING
    private UbcUnitCode? Fail(UbcDiagnosticCode code, VmReason reason, VmSourcePosition position)
    {
        Refusal = UbcRefusal.Invalid(code, reason, position);
        return null;
    }
}
