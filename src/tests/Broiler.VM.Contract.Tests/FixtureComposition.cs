using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The composition helpers the behavioural suite builds runtimes with.
/// </summary>
/// <remarks>
/// <para>
/// Binding capabilities is the composition root's job, not the profile's - a profile package never
/// references <c>Broiler.VM.Runtime</c> and so cannot construct a registration. That split lives
/// here, in the only place that references both.
/// </para>
/// <para>
/// Every ceiling is stated explicitly. There is no "default options" helper that omits one, because
/// omission is precisely what runtime creation refuses, and a test helper that quietly supplied a
/// missing dimension would hide the rule it exists to exercise.
/// </para>
/// </remarks>
internal static class FixtureComposition
{
    internal static VmCatalog Catalog(params VmProfileDescriptor[] descriptors)
    {
        var builder = VmCatalog.CreateBuilder();

        foreach (var descriptor in descriptors)
        {
            builder.Add(descriptor);
        }

        return builder.Build();
    }

    internal static VmCatalog AlphaCatalog(FixtureVmProfileVariant variant = FixtureVmProfileVariant.Conforming) =>
        Catalog(FixtureVmProfile.DescriptorFor(variant));

    /// <summary>Every dimension adopted from the profile's declared defaults.</summary>
    internal static ImmutableArray<VmCeilingSpec> AdoptedCeilings()
    {
        var builder = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            builder.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        return builder.ToImmutable();
    }

    /// <summary>Adopted ceilings with one dimension overridden to a stated value.</summary>
    internal static ImmutableArray<VmCeilingSpec> CeilingsWith(VmBudgetDimension dimension, ulong value)
    {
        var builder = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var current in VmBudgetDimensions.All)
        {
            if (current == dimension)
            {
                builder.Add(VmCeilingSpec.Value(current, value));
                continue;
            }

            builder.Add(current is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(current)
                : VmCeilingSpec.AdoptProfileDefault(current));
        }

        return builder.ToImmutable();
    }

    internal static ImmutableArray<VmCapabilityRegistration> ValueCapabilities()
    {
        var builder = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        builder.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Double, FixtureHostCapabilities.DoubleHandler));
        builder.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Throwing, FixtureHostCapabilities.ThrowingHandler));
        builder.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Refusing, FixtureHostCapabilities.RefusingHandler));

        return builder.ToImmutable();
    }

    internal static ImmutableArray<VmCapabilityRegistration> WithProvider(IVmArtifactProvider provider)
    {
        var builder = ValueCapabilities().ToBuilder();

        builder.Add(VmCapabilityRegistration.ArtifactProvider(
            FixtureHostCapabilities.Provider, provider));

        return builder.ToImmutable();
    }

    internal static VmRuntimeCreationOptions Options(
        ImmutableArray<VmCeilingSpec>? ceilings = null,
        VmAggregateBudget? parent = null,
        VmExternalSuspensionMode externalSuspension = VmExternalSuspensionMode.Disabled,
        ImmutableArray<VmCapabilityRegistration>? capabilities = null,
        int maxLiveSuspendedOperations = 4,
        TimeSpan? maxSuspendedResidency = null) =>
        new(
            aggregateBudget: parent,
            ceilings: ceilings ?? AdoptedCeilings(),
            maxSuspendedResidency: maxSuspendedResidency ?? TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: maxLiveSuspendedOperations,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: externalSuspension,
            capabilities: capabilities ?? ValueCapabilities());

    internal static VmRuntime Runtime(VmCatalog catalog, VmRuntimeCreationOptions? options = null)
    {
        var created = VmRuntime.Create(catalog, options ?? Options());

        Assert.True(created.IsSuccess, $"runtime creation failed: {created.Outcome}/{created.Reason}");
        Assert.True(created.TryGetRuntime(out var runtime));

        return runtime;
    }

    internal static VmArtifactDescriptor Descriptor(
        VmProfileId? profileId = null,
        VmFeatureManifestId? manifest = null,
        uint formatVersion = FixtureFormat.FormatVersion) =>
        new(
            profileId ?? FixtureVmProfile.Id,
            formatVersion,
            manifest ?? FixtureVmProfile.Manifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("test://artifact"));

    internal static VmVerifiedArtifact Verify(VmRuntime runtime, byte[] payload, VmArtifactDescriptor? descriptor = null)
    {
        var used = descriptor ?? Descriptor();
        var result = runtime.Verify(in used, payload, CancellationToken.None);

        Assert.True(result.IsSuccess, $"verification failed: {result.Outcome}/{result.Reason}");
        Assert.True(result.TryGetArtifact(out var artifact));

        return artifact;
    }

    internal static VmInstance Instantiate(VmRuntime runtime, VmVerifiedArtifact artifact)
    {
        var result = runtime.Instantiate(artifact, CancellationToken.None);

        Assert.True(result.IsSuccess, $"instantiation failed: {result.Outcome}/{result.Reason}");
        Assert.True(result.TryGetInstance(out var instance));

        return instance;
    }

    internal static VmInvocationResult Invoke(VmInstance instance, string entryPoint = "main")
    {
        var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entryPoint)));
        return instance.Invoke(in request, CancellationToken.None);
    }

    internal static VmInvocationResult Invoke(
        VmInstance instance,
        out VmOperationControlHandle handle,
        string entryPoint = "main")
    {
        var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entryPoint)));
        return instance.Invoke(in request, CancellationToken.None, out handle);
    }
}
