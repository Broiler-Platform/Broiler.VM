using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The precedence algorithm's three input layers: what the artifact requests, what the host states
/// at instantiation, and what it states at invocation.
/// </summary>
/// <remarks>
/// <para>
/// The oracle is computed here, from the profile descriptor and the runtime specification, and never
/// read back off a handle. Comparing the handle with itself would agree with any change.
/// </para>
/// <para>
/// The two monotonicity properties are the ones the whole layer exists to make true. M1: each scope
/// is no looser than the scope enclosing it, at the moment it is opened. M2: a live meter never
/// increases, except where a ceiling measure falls because the thing it measures fell.
/// </para>
/// </remarks>
public sealed class LimitPrecedenceTests
{
    [Fact]
    public void A_Ceiling_Is_Clamped_By_Every_Profile_In_The_Catalog_Not_Only_The_Selected_One()
    {
        // PINS A DIVERGENCE RATHER THAN RATIFYING IT. ADR 0007's ordered algorithm gives P1 a
        // closed "Inputs it may read" column - the host's explicit value and the two markers - and
        // places the ProfileMax intersection at P2, against the artifact's OWN profile. The
        // implementation clamps at P1 to the tightest maximum across EVERY descriptor in the
        // catalog. On a catalog of unlike profiles the two produce different numbers, and this test
        // records which number is produced today.
        //
        // Under the record as written the answer below would be 256, the host's own ceiling, because
        // Alpha's maximum of 1024 does not bind. Under the implementation it is 64, because Beta - a profile
        // this artifact has nothing to do with - declared a tighter one. That is not a hypothetical:
        // the VM-3 bundle records a ledger artifact refused with ResourceExhaustion naming
        // SectionCount "in a verifier that had done nothing wrong", for exactly this reason.
        //
        // Exclusion EX-104 item 2 carries the open question of which of the two is wrong. Until it
        // is ruled, this test is the only executable statement of the disagreement: whoever rules
        // changes it deliberately rather than rediscovering the divergence a third time.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.Descriptor,
            FixtureDescriptorFactory.Create(
                SecondFixtureVmProfile.Id,
                SecondFixtureVmProfile.Manifest,
                "Fixture Beta",
                "Broiler.VM.Fixtures",
                FixtureVmProfileVariant.Conforming,
                2,
                profileHardMaxima: FixtureDescriptorFactory.MaximaWith(VmBudgetDimension.SectionCount, 64)));

        using var runtime = FixtureComposition.Runtime(
            catalog,
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.SectionCount, 256)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        var effective = artifact.Identity.EffectiveCeilings.VerificationCeilings[VmBudgetDimension.SectionCount];

        Assert.Equal(64ul, effective);
        Assert.NotEqual(256ul, effective);
    }

    [Fact]
    public void An_Omitted_Artifact_Request_Removes_Nothing_And_Adds_Nothing()
    {
        // The first half of P2: a descriptor that requests no limits inherits the intersection of
        // the host ceiling and the profile maximum, and records no clamp.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        foreach (var dimension in VmBudgetDimensions.All)
        {
            Assert.Equal(
                Expected(dimension, request: ulong.MaxValue),
                artifact.Identity.EffectiveCeilings.VerificationCeilings[dimension]);
        }

        Assert.Empty(artifact.ClampedLimitRequests);
    }

    [Fact]
    public void An_Artifact_Request_Below_The_Intersection_Tightens_And_Is_Not_Clamped()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var tightened = Expected(VmBudgetDimension.Fuel, request: ulong.MaxValue) / 2;
        var artifact = VerifyRequesting(runtime, VmBudgetDimension.Fuel, tightened);

        Assert.Equal(tightened, artifact.Identity.EffectiveCeilings.VerificationCeilings[VmBudgetDimension.Fuel]);
        Assert.Empty(artifact.ClampedLimitRequests);
    }

    [Fact]
    public void An_Artifact_Request_Above_The_Intersection_Is_Clamped_And_Recorded_Not_Rejected()
    {
        // The asymmetry with a host override, and the reason for it: rejecting an over-asking
        // artifact would turn a request into a requirement, so the same safe bytes would fail on a
        // tighter host even though nothing in them needs the larger limit.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var intersection = Expected(VmBudgetDimension.Fuel, request: ulong.MaxValue);
        var asked = intersection + 1_000;

        var artifact = VerifyRequesting(runtime, VmBudgetDimension.Fuel, asked);

        Assert.Equal(intersection, artifact.Identity.EffectiveCeilings.VerificationCeilings[VmBudgetDimension.Fuel]);

        var clamp = Assert.Single(artifact.ClampedLimitRequests);

        Assert.Equal(VmBudgetDimension.Fuel, clamp.Dimension);
        Assert.Equal(asked, clamp.Requested);
        Assert.Equal(intersection, clamp.Effective);
    }

    [Fact]
    public void An_Omitted_Instance_Override_Inherits_The_Materialized_Policy()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        var result = runtime.Instantiate(artifact, VmLimitOverrides.None, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.TryGetInstance(out var instance));

        using (instance)
        {
            Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
        }
    }

    [Fact]
    public void An_Instance_Override_May_Tighten()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        var inherited = artifact.Identity.EffectiveCeilings.InstantiationCeilings[VmBudgetDimension.Fuel];

        var result = runtime.Instantiate(
            artifact, VmLimitOverrides.Of(VmBudgetDimension.Fuel, inherited / 2), CancellationToken.None);

        Assert.True(result.IsSuccess, $"{result.Outcome}/{result.Reason}");
        Assert.True(result.TryGetInstance(out var instance));
        instance.Dispose();
    }

    [Fact]
    public void An_Instance_Override_That_Would_Raise_Is_Refused_And_Is_Not_An_Exhaustion()
    {
        // BudgetRaiseRefused is emphatically not ResourceExhaustion: nothing was exhausted, and
        // misreporting a composition defect as exhaustion is the same diagnostic error that keeps
        // an unsupported profile separate from an invalid artifact.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        var inherited = artifact.Identity.EffectiveCeilings.InstantiationCeilings[VmBudgetDimension.Fuel];

        var result = runtime.Instantiate(
            artifact, VmLimitOverrides.Of(VmBudgetDimension.Fuel, inherited + 1), CancellationToken.None);

        Assert.Equal(VmOutcome.HostFailure, result.Outcome);
        Assert.Equal(VmReason.BudgetRaiseRefused, result.Reason);
        Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
        Assert.False(result.TryGetInstance(out _));
    }

    [Fact]
    public void A_Refused_Override_Applies_None_Of_Its_Other_Entries()
    {
        // Applying the tightenings and then refusing the raise would leave an instance running
        // under a policy no layer ever computed. The whole set is refused or the whole set applies.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        var ceilings = artifact.Identity.EffectiveCeilings.InstantiationCeilings;

        var overrides = VmLimitOverrides.Create(
        [
            new VmLimitOverride(VmBudgetDimension.Fuel, ceilings[VmBudgetDimension.Fuel] / 2),
            new VmLimitOverride(VmBudgetDimension.HostCalls, ceilings[VmBudgetDimension.HostCalls] + 1),
        ]);

        var refused = runtime.Instantiate(artifact, overrides, CancellationToken.None);

        Assert.Equal(VmReason.BudgetRaiseRefused, refused.Reason);
        Assert.Equal(VmBudgetDimension.HostCalls, refused.Diagnostics.ExhaustedDimension);

        // The proof that nothing was applied: instantiating again with nothing stated succeeds and
        // runs, which it could not do against a half-applied policy left behind by the refusal.
        var second = runtime.Instantiate(artifact, VmLimitOverrides.None, CancellationToken.None);

        Assert.True(second.IsSuccess);
        Assert.True(second.TryGetInstance(out var instance));

        using (instance)
        {
            Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
        }
    }

    [Fact]
    public void An_Override_Naming_A_Dimension_The_Scope_Does_Not_Admit_Is_Refused()
    {
        // ArtifactBytes is declarable at the runtime and the artifact and nowhere else. An instance
        // override on it is a host mistake, and it reports as one rather than being ignored.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        var result = runtime.Instantiate(
            artifact, VmLimitOverrides.Of(VmBudgetDimension.ArtifactBytes, 16), CancellationToken.None);

        Assert.Equal(VmOutcome.HostFailure, result.Outcome);
        Assert.Equal(VmReason.BudgetDimensionNotDeclarableAtScope, result.Reason);
        Assert.Equal(VmBudgetDimension.ArtifactBytes, result.Diagnostics.ExhaustedDimension);
    }

    [Fact]
    public void An_Override_Naming_One_Dimension_Twice_Is_Refused()
    {
        // Two values for one dimension is a set with no meaning. Taking the last, or the tighter,
        // would be the core choosing on the host's behalf which of two instructions it meant.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        var ceiling = artifact.Identity.EffectiveCeilings.InstantiationCeilings[VmBudgetDimension.Fuel];

        var overrides = VmLimitOverrides.Create(
        [
            new VmLimitOverride(VmBudgetDimension.Fuel, ceiling / 2),
            new VmLimitOverride(VmBudgetDimension.Fuel, ceiling / 4),
        ]);

        var result = runtime.Instantiate(artifact, overrides, CancellationToken.None);

        Assert.Equal(VmOutcome.HostFailure, result.Outcome);
        Assert.Equal(VmReason.BudgetDimensionNotDeclarableAtScope, result.Reason);
    }

    [Fact]
    public void An_Invocation_Override_Only_Tightens_And_Exhausts_The_Operation_It_Bounds()
    {
        // P4 end to end: the operation that would have completed under the inherited policy runs
        // out under the stated one, and the stated one could not have been larger.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8.ToArray()));

        var tightened = instance.Invoke(
            in request, VmLimitOverrides.Of(VmBudgetDimension.Fuel, 1), CancellationToken.None);

        Assert.Equal(VmOutcome.ResourceExhaustion, tightened.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, tightened.Diagnostics.ExhaustedDimension);
    }

    [Fact]
    public void An_Invocation_Override_That_Would_Raise_Is_Refused_Before_The_Operation_Exists()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var inherited = artifact.Identity.EffectiveCeilings.InstantiationCeilings[VmBudgetDimension.Fuel];
        var request = new VmInvocationRequest(new VmUtf8Text("main"u8.ToArray()));

        var result = instance.Invoke(
            in request, VmLimitOverrides.Of(VmBudgetDimension.Fuel, inherited + 1), CancellationToken.None);

        Assert.Equal(VmOutcome.HostFailure, result.Outcome);
        Assert.Equal(VmReason.BudgetRaiseRefused, result.Reason);

        // The instance is untouched: the refusal happened before an operation was created, so the
        // next invocation runs normally rather than finding the instance faulted or executing.
        Assert.Equal(VmInstanceState.Live, instance.State);
        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
    }

    [Fact]
    public void Raising_A_Ceiling_Requires_A_Newly_Verified_Handle_With_A_Different_Identity()
    {
        // There is no re-policy, re-bind, widen or clone-with-larger-limits operation, so the only
        // way to a higher ceiling is to verify again under a runtime that permits it - and because
        // the effective ceilings are part of identity, the two handles cannot be confused.
        var payload = FixtureArtifactWriter.Sum(20, 22);
        var wide = Expected(VmBudgetDimension.Fuel, request: ulong.MaxValue);

        using var tightRuntime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, wide / 4)));

        using var wideRuntime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var tight = FixtureComposition.Verify(tightRuntime, payload);
        var loose = FixtureComposition.Verify(wideRuntime, payload);

        Assert.NotEqual(
            tight.Identity.EffectiveCeilings.VerificationCeilings[VmBudgetDimension.Fuel],
            loose.Identity.EffectiveCeilings.VerificationCeilings[VmBudgetDimension.Fuel]);

        Assert.NotEqual(tight.Identity, loose.Identity);
        Assert.Equal(VmReason.SharedHandleCeilingMismatch, tight.Identity.FirstMismatch(loose.Identity));

        // And the tight handle is not admitted by the wider runtime, so a host cannot reach the
        // larger ceiling by moving a handle instead of verifying again.
        var crossed = wideRuntime.Instantiate(tight, CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidState, crossed.Outcome);
        Assert.Equal(VmReason.SharedHandleCeilingMismatch, crossed.Reason);
    }

    [Fact]
    public void M1_Every_Scope_Is_No_Looser_Than_The_One_Enclosing_It_Over_Randomized_Layers()
    {
        // A property over randomized layer values rather than one worked example. The seed is
        // fixed, so a counterexample is reproduced by rerunning rather than by luck.
        var random = new DeterministicSequence(0x5EED_10);

        for (var round = 0; round < 64; round++)
        {
            var runtimeCeilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();
            var chosen = new ulong[VmBudgetDimensions.Count];

            foreach (var dimension in VmBudgetDimensions.All)
            {
                if (dimension is VmBudgetDimension.LiveRuntimes)
                {
                    runtimeCeilings.Add(VmCeilingSpec.AdoptParentRemaining(dimension));
                    chosen[(int)dimension] = ulong.MaxValue;
                    continue;
                }

                // Never below the profile's declared default, so every round still verifies: the
                // property under test is the ordering between layers, not whether a starved runtime
                // can read an artifact.
                var floor = FixtureDescriptorFactory.Defaults()[dimension];
                var ceiling = FixtureDescriptorFactory.Maxima()[dimension];
                var value = floor + random.Next(ceiling - floor + 1);

                chosen[(int)dimension] = value;
                runtimeCeilings.Add(VmCeilingSpec.Value(dimension, value));
            }

            var request = new ulong[VmBudgetDimensions.Count];

            foreach (var dimension in VmBudgetDimensions.All)
            {
                // Half the rounds ask for more than they can have, so the clamp is exercised as
                // often as the tightening is.
                request[(int)dimension] = random.Next(2) == 0
                    ? ulong.MaxValue
                    : Saturate(FixtureDescriptorFactory.Defaults()[dimension], random.Next(4_096));
            }

            VmLimitVector.TryCreate(request, out var requested);

            using var runtime = FixtureComposition.Runtime(
                FixtureComposition.AlphaCatalog(),
                FixtureComposition.Options(runtimeCeilings.ToImmutable()));

            var descriptor = new VmArtifactDescriptor(
                FixtureVmProfile.Id,
                FixtureFormat.FormatVersion,
                FixtureVmProfile.Manifest,
                requested,
                VmCallerIdentity.None);

            var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22), descriptor);
            var handle = artifact.Identity.EffectiveCeilings.VerificationCeilings;

            foreach (var dimension in VmBudgetDimensions.All)
            {
                var runtimeCeiling = Math.Min(
                    chosen[(int)dimension], FixtureDescriptorFactory.Maxima()[dimension]);

                var expected = Math.Min(runtimeCeiling, request[(int)dimension]);

                Assert.True(
                    handle[dimension] == expected,
                    $"round {round}, {dimension}: handle {handle[dimension]}, intersection {expected}");

                Assert.True(
                    handle[dimension] <= runtimeCeiling,
                    $"round {round}, {dimension}: the handle is looser than the runtime that made it");
            }

            // The clamp record is exactly the dimensions where the artifact asked for something and
            // was given less. A dimension left at TOP asked for nothing, so it is not a clamp.
            var clamped = artifact.ClampedLimitRequests.Select(static entry => entry.Dimension).ToHashSet();

            foreach (var dimension in VmBudgetDimensions.All)
            {
                var runtimeCeiling = Math.Min(
                    chosen[(int)dimension], FixtureDescriptorFactory.Maxima()[dimension]);

                var asked = request[(int)dimension];
                var expected = asked != ulong.MaxValue && asked > runtimeCeiling;

                Assert.True(
                    expected == clamped.Contains(dimension),
                    $"round {round}, {dimension}: asked {asked}, ceiling {runtimeCeiling}, " +
                    $"clamp {(clamped.Contains(dimension) ? "recorded" : "absent")}");
            }
        }
    }

    [Fact]
    public void M2_No_Live_Meter_Increases_Over_A_Sequence_Of_Operations()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(20, 22));

        var previous = Remaining(runtime);

        for (var round = 0; round < 8; round++)
        {
            using var instance = FixtureComposition.Instantiate(runtime, artifact);

            Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);

            var current = Remaining(runtime);

            foreach (var dimension in VmBudgetDimensions.All)
            {
                if (VmBudgetDimensions.ClassOf(dimension) is not VmBudgetClass.Allowance)
                {
                    // A ceiling measure legitimately falls and rises: it measures something live,
                    // and disposal gives it back. M2 exempts exactly that case.
                    continue;
                }

                Assert.True(
                    current[(int)dimension] <= previous[(int)dimension],
                    $"round {round}, {dimension}: the allowance went up, which is a refund by another name");
            }

            previous = current;
        }
    }

    private static ulong[] Remaining(VmRuntime runtime)
    {
        var snapshot = runtime.GetBudgetSnapshot();
        var remaining = new ulong[VmBudgetDimensions.Count];

        foreach (var dimension in VmBudgetDimensions.All)
        {
            remaining[(int)dimension] = snapshot.Remaining(dimension);
        }

        return remaining;
    }

    /// <summary>The intersection, computed from the descriptor and the runtime specification.</summary>
    private static ulong Expected(VmBudgetDimension dimension, ulong request)
    {
        var maximum = FixtureDescriptorFactory.Maxima()[dimension];

        var runtimeCeiling = dimension is VmBudgetDimension.LiveRuntimes
            ? Math.Min(ulong.MaxValue, maximum)
            : Math.Min(FixtureDescriptorFactory.Defaults()[dimension], maximum);

        return Math.Min(Math.Min(runtimeCeiling, maximum), request);
    }

    private static VmVerifiedArtifact VerifyRequesting(
        VmRuntime runtime,
        VmBudgetDimension dimension,
        ulong value)
    {
        var values = new ulong[VmBudgetDimensions.Count];
        Array.Fill(values, ulong.MaxValue);
        values[(int)dimension] = value;

        VmLimitVector.TryCreate(values, out var requested);

        var descriptor = new VmArtifactDescriptor(
            FixtureVmProfile.Id,
            FixtureFormat.FormatVersion,
            FixtureVmProfile.Manifest,
            requested,
            VmCallerIdentity.None);

        return FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1), descriptor);
    }

    private static ulong Saturate(ulong value, ulong addition) =>
        value > ulong.MaxValue - addition ? ulong.MaxValue : value + addition;

    /// <summary>
    /// A seeded sequence, so a property round that fails is reproduced by rerunning it.
    /// </summary>
    /// <remarks>
    /// Deliberately not <c>System.Random</c>: its sequence is a framework implementation detail,
    /// so a counterexample found on one runtime version would not reproduce on another, and a
    /// property test whose failures do not reproduce is a flake generator.
    /// </remarks>
    private sealed class DeterministicSequence
    {
        private ulong state;

        internal DeterministicSequence(ulong seed) => state = seed == 0 ? 1 : seed;

        internal ulong Next(ulong exclusiveBound)
        {
            state ^= state >> 12;
            state ^= state << 25;
            state ^= state >> 27;

            return exclusiveBound == 0 ? 0 : (state * 0x2545F4914F6CDD1D) % exclusiveBound;
        }
    }
}
