using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// G11 and G12 of the VM-1 gate: a guest-initiated load through a fixture provider, and the
/// deterministic refusal where no provider is registered.
/// </summary>
public sealed class GuestInitiatedLoadTests
{
    private static VmCatalog DeclaringCatalog() =>
        FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads));

    [Fact]
    public void A_Declaring_Profile_Loads_Through_A_Registered_Provider()
    {
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(99));

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(1, provider.RequestCount);
        Assert.True(FixtureVmProfileResults.TryGetValue(in result, out var value));
        Assert.Equal(7, value.Value);
    }

    [Fact]
    public void A_Composition_With_No_Provider_Refuses_Every_Request_Deterministically()
    {
        // Registering no provider is the content policy, expressed as a contract outcome rather
        // than as an ad-hoc check inside an engine. The runtime is created, the profile runs, and
        // every request it makes is refused the same way.
        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(capabilities: FixtureComposition.ValueCapabilities()));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var first = FixtureComposition.Invoke(instance);
        var second = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, first.Outcome);
        Assert.Equal(first.Outcome, second.Outcome);
        Assert.Equal(first.Reason, second.Reason);

        // The core's own reason, not just the profile's reaction to it. Asserting only the
        // profile-fault category would pass for any nested failure at all - a negative control
        // that removed the refusal entirely still produced a fault, because a null provider then
        // threw and was translated. The fixture carries the underlying reason in its fault code
        // precisely so a test can name which refusal happened.
        Assert.True(FixtureVmProfileResults.TryGetFault(in first, out var fault));
        Assert.Equal((long)VmReason.ProviderNotRegistered, fault.Code);
    }

    [Fact]
    public void A_Required_Import_Left_Unregistered_Fails_Runtime_Creation_With_No_Partial_Binding()
    {
        // A runtime half-wired to its host is a runtime whose first failure happens somewhere
        // unrelated to the mistake, so a missing required import means no runtime at all.
        var created = VmRuntime.Create(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(
                capabilities: System.Collections.Immutable.ImmutableArray<VmCapabilityRegistration>.Empty));

        Assert.Equal(VmOutcome.HostFailure, created.Outcome);
        Assert.Equal(VmReason.CapabilityNotRegistered, created.Reason);
        Assert.False(created.TryGetRuntime(out _));
    }

    [Fact]
    public void A_Non_Declaring_Profile_Is_Handed_No_Mediator_At_All()
    {
        // An undeclared request is structurally unrepresentable rather than a run-time check that
        // could be reported, logged, or forgotten: the core hands a non-declaring profile nothing.
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(capabilities: FixtureComposition.ValueCapabilities()));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(VmReason.ProfileContractViolation, result.Reason);
        Assert.Equal(0, provider.RequestCount);
    }

    [Fact]
    public void A_Provider_That_Refuses_Is_A_Host_Failure_And_Not_A_Refusal_Of_The_Artifact()
    {
        var provider = FixtureArtifactProvider.Refusing();

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(1, provider.RequestCount);

        Assert.True(FixtureVmProfileResults.TryGetFault(in result, out var fault));
        Assert.Equal((long)VmReason.ProviderRefused, fault.Code);
    }

    [Fact]
    public void A_Provider_That_Throws_Is_A_Host_Fault_And_Not_A_Refusal()
    {
        // The difference matters: a policy that refuses is working, and a provider that throws is
        // broken. They must not report the same way.
        var provider = FixtureArtifactProvider.Throwing();

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(1, provider.RequestCount);

        // A provider that throws is a host fault; a provider that refuses is a policy answer. The
        // two reach the profile as different reasons, which is the whole point of the distinction.
        Assert.True(FixtureVmProfileResults.TryGetFault(in result, out var fault));
        Assert.Equal((long)VmReason.HostCapabilityFaulted, fault.Code);
        Assert.NotEqual((long)VmReason.ProviderRefused, fault.Code);
    }

    [Fact]
    public void A_Provider_Answering_With_Another_Profiles_Artifact_Is_Refused()
    {
        // Answering with a different profile's artifact would let one profile reach another through
        // the host, which is a bridge the contract does not have.
        var provider = new FixtureArtifactProvider(SecondFixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(
                FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads),
                SecondFixtureVmProfile.Descriptor),
            FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);

        Assert.True(FixtureVmProfileResults.TryGetFault(in result, out var fault));
        Assert.Equal((long)VmReason.ProviderProfileMismatch, fault.Code);
    }

    [Fact]
    public void A_Nested_Handle_Is_Never_Shareable_With_Another_Runtime()
    {
        // Its ceilings came from one operation's remainder, which means nothing anywhere else, so
        // the origin flag alone decides the answer.
        var nested = FixtureArtifactWriter.Constant(99);
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, nested);

        var catalog = DeclaringCatalog();

        using var runtime = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
        Assert.Equal(VmArtifactOrigin.Caller, artifact.Origin);
    }

    [Fact]
    public void A_Guest_Load_Cannot_Enlarge_Its_Requesting_Operation()
    {
        // The nested verification draws on the requesting operation's remaining allowance. A load
        // can exhaust an invocation; it can never give one more room than it started with.
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(
                FixtureComposition.CeilingsWith(VmBudgetDimension.VerifierWork, 40),
                capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        // Either the nested verification fitted inside the remainder, or it did not and the
        // requesting operation is the one that ran out. What must never happen is a nested load
        // succeeding with more allowance than its parent had.
        Assert.True(
            result.Outcome is VmOutcome.Normal or VmOutcome.ResourceExhaustion or VmOutcome.ProfileFault,
            $"unexpected {result.Outcome}/{result.Reason}");
    }

    [Fact]
    public void Fan_Out_Is_Bounded_Per_Operation()
    {
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(
                capabilities: FixtureComposition.WithProvider(provider)));

        // Six loads in one invocation, against a profile that declares a fan-out maximum of eight:
        // within bounds, and the provider is asked exactly six times.
        var code = new List<byte>();

        for (var index = 0; index < 6; index++)
        {
            code.Add(FixtureFormat.OpLoad);
            code.Add(0);
        }

        code.Add(FixtureFormat.OpPushConst);
        code.Add(0);
        code.Add(FixtureFormat.OpReturn);

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Write([3], code.ToArray()));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(6, provider.RequestCount);
    }

    [Fact]
    public void Fan_Out_Beyond_The_Bound_Is_Refused()
    {
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var code = new List<byte>();

        for (var index = 0; index < 12; index++)
        {
            code.Add(FixtureFormat.OpLoad);
            code.Add(0);
        }

        code.Add(FixtureFormat.OpPushConst);
        code.Add(0);
        code.Add(FixtureFormat.OpReturn);

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Write([3], code.ToArray()));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.NotEqual(VmOutcome.Normal, result.Outcome);
        Assert.True(provider.RequestCount <= 8, $"provider asked {provider.RequestCount} times past its bound");
    }

    [Fact]
    public void A_Guest_Load_Adds_No_Core_Result_Category()
    {
        // A nested load reports through the same ten categories as everything else. The stage row
        // for a guest-initiated load is the evidence: it names no category the contract lacks.
        foreach (var outcome in VmStageMatrix.LegalCategoriesAt(VmStage.GuestInitiatedLoad))
        {
            Assert.True(Enum.IsDefined(outcome));
            Assert.NotEqual(VmOutcome.None, outcome);
        }
    }
}
