// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           0
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// Resolves a runtime's fifteen ceilings from the host's specification, the profiles in the
/// catalog, and the parent budget.
/// </summary>
/// <remarks>
/// <para>
/// Resource authority is trusted and monotonic: at runtime creation the host supplies explicit
/// ceilings or explicitly adopts bounded profile defaults, and <strong>omission never means
/// unbounded</strong>. A dimension with no entry fails runtime creation rather than acquiring a
/// value nobody chose.
/// </para>
/// <para>
/// A per-runtime ceiling may never exceed the parent's remaining allowance, which is what stops
/// creating more runtimes from multiplying a host maximum. The live-runtime dimension is the one
/// exception in the other direction: it is meaningful only against a parent, so its only legal
/// runtime-scope entry is to adopt the parent's remaining, and with no parent that resolves to TOP.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A5C61C
// Broiler-Human:        PENDING
internal static class VmCeilingResolution
{
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=3; Fingerprint=21E56B
    // Broiler-Human:        PENDING
    internal static bool TryResolve(
        VmCatalog catalog,
        VmRuntimeCreationOptions options,
        out ulong[] ceilings,
        out VmReason failure)
    {
        ceilings = new ulong[VmBudgetDimensions.Count];
        failure = VmReason.None;

        var seen = new bool[VmBudgetDimensions.Count];
        var specs = options.Ceilings.IsDefault
            ? System.Collections.Immutable.ImmutableArray<VmCeilingSpec>.Empty
            : options.Ceilings;

        foreach (var spec in specs)
        {
            if (!VmBudgetDimensions.IsDefined(spec.Dimension) || seen[(int)spec.Dimension])
            {
                failure = VmReason.BudgetDimensionUnresolved;
                return false;
            }

            if (spec.Dimension is VmBudgetDimension.LiveRuntimes &&
                spec.Source is not VmCeilingSource.AdoptParentRemaining)
            {
                failure = VmReason.BudgetDimensionNotRuntimeScoped;
                return false;
            }

            if (!TryResolveOne(catalog, options, spec, out var value, out failure))
            {
                return false;
            }

            ceilings[(int)spec.Dimension] = value;
            seen[(int)spec.Dimension] = true;
        }

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (!seen[(int)dimension])
            {
                failure = VmReason.BudgetDimensionUnresolved;
                return false;
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=3; Fingerprint=6BDCA3
    // Broiler-Human:        PENDING
    private static bool TryResolveOne(
        VmCatalog catalog,
        VmRuntimeCreationOptions options,
        VmCeilingSpec spec,
        out ulong value,
        out VmReason failure)
    {
        failure = VmReason.None;

        switch (spec.Source)
        {
            case VmCeilingSource.Explicit:
                value = spec.ExplicitValue;
                break;

            case VmCeilingSource.AdoptProfileDefault:
                // The tightest default in the catalog, and unlike the maximum above this one is
                // catalog-wide on purpose. A maximum has a correct owner at P2 - the profile the
                // artifact names - so P1 need not guess at one. A default has no owner at all here:
                // the host declined to state a number and no profile is selected yet, so something
                // must be chosen before there is anything to choose it from. The most conservative
                // answer is the only safe one, and it costs nothing, because verification
                // re-intersects with the selected profile's own maxima afterwards.
                value = TightestProfileDefault(catalog, spec.Dimension);
                break;

            default:
                // With no parent there is nothing to adopt. TOP is the only honest answer for the
                // live-runtime dimension - an unparented runtime has no sibling to count against -
                // and every other dimension must have been given a number.
                if (options.AggregateBudget is null)
                {
                    if (spec.Dimension is VmBudgetDimension.LiveRuntimes)
                    {
                        value = ulong.MaxValue;
                        break;
                    }

                    failure = VmReason.BudgetDimensionUnresolved;
                    value = 0;
                    return false;
                }

                // A dimension the parent does not meter has no remainder to adopt. Resolving it to
                // zero silently would give the runtime a ceiling of nothing and make every later
                // charge fail for a reason that names the wrong thing.
                if (!VmBudgetDimensions.CarriesAggregateScope(spec.Dimension))
                {
                    failure = VmReason.BudgetDimensionUnresolved;
                    value = 0;
                    return false;
                }

                value = options.AggregateBudget.RemainingFor(spec.Dimension);
                break;
        }

        // No profile hard maximum is applied here, and that is the whole of the correction ruled on
        // 2026-08-31. ADR 0007 puts ProfileMax in P2, against the profile the ARTIFACT names, and
        // gives P1 a closed input list that does not include a descriptor. This step used to clamp
        // to the tightest maximum across every descriptor in the catalog, which meant one profile's
        // declaration silently constrained another's: a ledger artifact was refused naming
        // SectionCount because a calculator in the same catalog framed one section, in a verifier
        // that had done nothing wrong.
        //
        // Dropping it removes no bound. Verification computes
        // Intersect(hostCeilings, profile.ProfileHardMaxima) with the SELECTED profile, so a profile
        // still cannot be granted more than its own maximum; the P1 clamp only ever added
        // cross-profile coupling on top of that, in a graph whose rules are otherwise at pains to
        // keep profiles from touching each other.

        if (options.AggregateBudget is not null &&
            VmBudgetDimensions.CarriesAggregateScope(spec.Dimension) &&
            spec.Dimension is not VmBudgetDimension.LiveRuntimes)
        {
            var remaining = options.AggregateBudget.RemainingFor(spec.Dimension);

            if (value > remaining)
            {
                failure = VmReason.ExceedsParentRemaining;
                return false;
            }
        }

        return true;
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=3; Fingerprint=64F4F2
    // Broiler-Human:        PENDING
    private static ulong TightestProfileDefault(VmCatalog catalog, VmBudgetDimension dimension)
    {
        var tightest = ulong.MaxValue;

        foreach (var descriptor in catalog.Descriptors)
        {
            var declared = descriptor.LimitDefaults[dimension];

            if (declared < tightest)
            {
                tightest = declared;
            }
        }

        // An empty catalog has no default to adopt. Zero is the safe answer: a runtime over a
        // catalog that hosts nothing can run nothing, and every verification against it is an
        // unsupported profile anyway.
        return catalog.Count == 0 ? 0 : tightest;
    }

}
