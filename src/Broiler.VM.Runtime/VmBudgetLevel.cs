// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           2
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Medium
// Resource impact:  1/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// One level of the meter chain: the ceilings declared at one scope and what has been consumed
/// against them.
/// </summary>
/// <remarks>
/// It holds no lock of its own. Charging has to be atomic across several levels at once - the
/// tie-break rule requires knowing whether an outer level would also have refused - so the lock
/// lives on the chain that owns the levels, and the levels themselves are plain state.
/// </remarks>
internal sealed class VmBudgetLevel
{
    private readonly ulong[] ceilings;
    private readonly ulong[] consumed = new ulong[VmBudgetDimensions.Count];

    internal VmBudgetLevel(VmBudgetScope scope, ulong[] ceilings)
    {
        Scope = scope;
        this.ceilings = ceilings;
    }

    internal VmBudgetScope Scope { get; }

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=0F4B8C
    // Broiler-Human: PENDING
    internal ulong Ceiling(VmBudgetDimension dimension) => ceilings[(int)dimension];

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=0; Fingerprint=27DA38
    // Broiler-Human: PENDING
    internal ulong Consumed(VmBudgetDimension dimension) => consumed[(int)dimension];

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=E1E12C
    // Broiler-Human: PENDING
    internal ulong Remaining(VmBudgetDimension dimension)
    {
        var ceiling = ceilings[(int)dimension];
        var used = consumed[(int)dimension];
        return used >= ceiling ? 0 : ceiling - used;
    }

    /// <summary>Whether <paramref name="amount"/> would fit. It does not commit anything.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=784BD0
    // Broiler-Human: PENDING
    internal bool Admits(VmBudgetDimension dimension, ulong amount) =>
        amount <= Remaining(dimension);

    /// <summary>Commits a charge that <see cref="Admits"/> has already cleared.</summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=DFD2E3
    // Broiler-Human: PENDING
    internal void Commit(VmBudgetDimension dimension, ulong amount) =>
        consumed[(int)dimension] += amount;

    /// <summary>
    /// Gives back part of a live measure. Only a ceiling-class dimension releases: an allowance
    /// never refunds, and a released allowance would be a refund wearing another name.
    /// </summary>
    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=1C8C47
    // Broiler-Human: PENDING
    internal void Release(VmBudgetDimension dimension, ulong amount)
    {
        if (VmBudgetDimensions.ClassOf(dimension) is not VmBudgetClass.Ceiling)
        {
            return;
        }

        var used = consumed[(int)dimension];
        consumed[(int)dimension] = amount >= used ? 0 : used - amount;
    }

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=BC1A25
    // Broiler-Human: PENDING
    internal VmBudgetSnapshot Snapshot() =>
        new(Scope, (ulong[])consumed.Clone(), (ulong[])ceilings.Clone());

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Low; Resources=1; Fingerprint=E0F700
    // Broiler-Human: PENDING
    internal ulong[] CeilingsCopy() => (ulong[])ceilings.Clone();

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=6BDF22
    // Broiler-Human: PENDING
    internal VmLimitVector AsRemainingVector()
    {
        var remaining = new ulong[VmBudgetDimensions.Count];

        for (var index = 0; index < remaining.Length; index++)
        {
            remaining[index] = Remaining((VmBudgetDimension)index);
        }

        return VmLimitVector.TryCreate(remaining, out var vector) ? vector : default;
    }

    // Broiler-AI:    Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=1; Fingerprint=AB5CDD
    // Broiler-Human: PENDING
    internal VmLimitVector AsCeilingVector() =>
        VmLimitVector.TryCreate(ceilings, out var vector) ? vector : default;
}
