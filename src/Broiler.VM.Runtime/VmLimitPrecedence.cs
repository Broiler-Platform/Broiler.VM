// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The two layers of the precedence algorithm that read a value stated about one artifact or one
/// operation: the artifact request at verification, and the host override at instantiation and
/// invocation.
/// </summary>
/// <remarks>
/// <para>
/// The two layers are here together because they are the same computation with opposite failure
/// behaviour, and separating them is what made the asymmetry easy to get wrong. Both take a stated
/// value and an inherited bound. An <em>artifact</em> request that asks for more is clamped to the
/// bound and recorded, because it is stated from outside the trust boundary and rejecting it would
/// turn a request into a requirement. A <em>host</em> override that asks for more is refused
/// outright, because it is stated from inside the boundary and clamping it would discard an
/// instruction from trusted code.
/// </para>
/// <para>
/// Neither layer can raise anything. That is structural rather than checked: the clamp takes a
/// minimum, and the override is refused before a single dimension is written, so a set containing
/// one raising entry applies none of its other entries either. A partially applied set would leave
/// an operation running under a policy no layer ever computed.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=0FE8E6
// Broiler-Human:        PENDING
internal static class VmLimitPrecedence
{
    /// <summary>
    /// P2: the artifact-requested limits the host and profile intersection tightened, in the frozen
    /// dimension order.
    /// </summary>
    /// <remarks>
    /// Read from the immutable descriptor and never from the payload, which is why this can run
    /// before the first payload byte is examined. A descriptor stating no limits produces no clamps
    /// rather than fifteen clamps against TOP.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=2; Fingerprint=6EB4E9
    // Broiler-Human:        PENDING
    internal static System.Collections.Immutable.ImmutableArray<VmLimitClamp> Clamps(
        VmLimitVector bound,
        VmLimitVector requested)
    {
        if (requested.IsEmpty)
        {
            return System.Collections.Immutable.ImmutableArray<VmLimitClamp>.Empty;
        }

        System.Collections.Immutable.ImmutableArray<VmLimitClamp>.Builder? builder = null;

        foreach (var dimension in VmBudgetDimensions.All)
        {
            // TOP is how a vector spells "this dimension says nothing", and a dimension that says
            // nothing was not clamped - it was never a request. Reporting one would fill the record
            // with fourteen clamps for every descriptor that tightened a single dimension, which is
            // the same as reporting none.
            if (requested.IsUnconstrained(dimension))
            {
                continue;
            }

            var asked = requested[dimension];

            if (asked <= bound[dimension])
            {
                continue;
            }

            builder ??= System.Collections.Immutable.ImmutableArray.CreateBuilder<VmLimitClamp>();
            builder.Add(new VmLimitClamp(dimension, asked, bound[dimension]));
        }

        return builder is null
            ? System.Collections.Immutable.ImmutableArray<VmLimitClamp>.Empty
            : builder.ToImmutable();
    }

    /// <summary>
    /// P3 and P4: applies a host override set to the inherited ceilings of one scope, or refuses
    /// the whole set.
    /// </summary>
    /// <remarks>
    /// The clauses run in a fixed order so a set breaking two of them always reports the same one:
    /// an unknown dimension, then a dimension repeated, then a dimension the scope table does not
    /// admit here, then a value that would raise the inherited bound.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=3; Fingerprint=C06726
    // Broiler-Falsified-If: a refused set leaves one dimension of the caller's inherited array changed
    // Broiler-Human:        PENDING
    internal static bool TryApply(
        VmBudgetScope scope,
        ulong[] inherited,
        VmLimitOverrides overrides,
        out ulong[] resolved,
        out VmBudgetDimension offending,
        out VmReason failure)
    {
        resolved = inherited;
        offending = VmBudgetDimension.Fuel;
        failure = VmReason.None;

        if (overrides.IsEmpty)
        {
            // The omitted case, and the common one: inherit the materialized policy unchanged.
            return true;
        }

        var stated = new bool[VmBudgetDimensions.Count];
        var candidate = (ulong[])inherited.Clone();

        for (var index = 0; index < overrides.Count; index++)
        {
            var entry = overrides[index];
            offending = entry.Dimension;

            if (!VmBudgetDimensions.IsDefined(entry.Dimension))
            {
                failure = VmReason.BudgetDimensionUnresolved;
                return false;
            }

            if (stated[(int)entry.Dimension])
            {
                failure = VmReason.BudgetDimensionNotDeclarableAtScope;
                return false;
            }

            if (!VmBudgetDimensions.IsDeclarableAt(entry.Dimension, scope))
            {
                failure = VmReason.BudgetDimensionNotDeclarableAtScope;
                return false;
            }

            // Tighten-only, checked against what this scope inherited rather than against the
            // runtime ceiling: an override may be no looser than the value it replaces, and the
            // value it replaces is already the intersection of every layer above it.
            if (entry.Value > candidate[(int)entry.Dimension])
            {
                failure = VmReason.BudgetRaiseRefused;
                return false;
            }

            candidate[(int)entry.Dimension] = entry.Value;
            stated[(int)entry.Dimension] = true;
        }

        offending = VmBudgetDimension.Fuel;
        resolved = candidate;
        return true;
    }
}
