using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Ubc;
using Com.Example.Ledger;
using Com.Example.Tally;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.Ubc.Fixture;

/// <summary>
/// The composition: the fixture family's descriptor, built here because the forms are chosen here,
/// and the runtimes the checks run in.
/// </summary>
internal static class FixtureHost
{
    /// <summary>
    /// The fixture family's descriptor over the one form this image composes. Rows 4 to 7 are the
    /// universal bytecode's; every other row is the family's declaration.
    /// </summary>
    internal static VmProfileDescriptor Tally { get; } = UbcDescriptors.Build(
        TallyProfile.Registration,
        TallyProfile.Declaration,
        UbcEmitterSet.Create(UbcBytecodeEmitter.Form));

    /// <summary>
    /// A runtime composing the fixture family alone, with its own defaults as explicit ceilings, over
    /// <paramref name="descriptor"/> when a check brings a probing one and the image's own otherwise;
    /// <paramref name="guest"/> is what the provider answers the first guest name with, the lowering's
    /// guest when none is given.
    /// </summary>
    internal static VmRuntime Runtime(
        System.Action<ulong[]>? adjust = null,
        bool withProvider = true,
        VmProfileDescriptor? descriptor = null,
        ReadOnlyMemory<byte>? guest = null)
    {
        var limits = Vector(TallyProfile.Defaults());
        adjust?.Invoke(limits);
        return Create(Catalog(descriptor ?? Tally), Explicit(limits), withProvider, guest);
    }

    /// <summary>A catalog of the given descriptors, in order.</summary>
    internal static VmCatalog Catalog(params VmProfileDescriptor[] descriptors)
    {
        var builder = VmCatalog.CreateBuilder();

        foreach (var descriptor in descriptors)
        {
            builder.Add(descriptor);
        }

        return builder.Build();
    }

    /// <summary>A runtime over <paramref name="catalog"/> with the given ceilings.</summary>
    internal static VmRuntime Create(VmCatalog catalog, ImmutableArray<VmCeilingSpec> ceilings, bool withProvider, ReadOnlyMemory<byte>? guest = null)
    {
        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        if (withProvider)
        {
            capabilities.Add(VmCapabilityRegistration.ArtifactProvider(
                TallyProfile.ProviderCapability,
                new GuestProvider(guest ?? TallyPrograms.Guest.AsMemory())));
        }

        var options = new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings,
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 4,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());

        var created = VmRuntime.Create(catalog, options);

        return created.TryGetRuntime(out var runtime)
            ? runtime
            : throw new InvalidOperationException($"runtime creation {created.Outcome}/{created.Reason}");
    }

    /// <summary>Every dimension set explicitly from <paramref name="limits"/>, the live-runtime count adopted from the parent.</summary>
    internal static ImmutableArray<VmCeilingSpec> Explicit(ulong[] limits)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.Value(dimension, limits[(int)dimension]));
        }

        return ceilings.ToImmutable();
    }

    /// <summary>Every dimension adopted from the catalog's profile defaults: the tightest per dimension.</summary>
    internal static ImmutableArray<VmCeilingSpec> AdoptDefaults()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        return ceilings.ToImmutable();
    }

    /// <summary>A limit vector's values, one per dimension.</summary>
    internal static ulong[] Vector(VmLimitVector vector)
    {
        var values = new ulong[VmBudgetDimensions.Count];

        foreach (var dimension in VmBudgetDimensions.All)
        {
            values[(int)dimension] = vector[dimension];
        }

        return values;
    }

    /// <summary>The artifact descriptor a Tally artifact is verified under.</summary>
    internal static VmArtifactDescriptor Descriptor() =>
        new(TallyProfile.Id, UbcFormat.FormatVersion, TallyProfile.Manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("composition-ubc-fixture://artifact"));

    /// <summary>
    /// Runs a program to its end and answers its transcript: one line per step, a suspension resumed
    /// at once, so the whole run is one string a check compares and a corpus retains.
    /// </summary>
    internal static string Transcript(VmRuntime runtime, ReadOnlySpan<byte> artifact, string entry, CancellationToken token = default)
    {
        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, artifact, token);

        if (!verified.TryGetArtifact(out var handle))
        {
            return $"refused {verified.Outcome} {verified.Reason} {verified.Diagnostics.ProfileDiagnosticCode.ToString(CultureInfo.InvariantCulture)}";
        }

        var instantiated = runtime.Instantiate(handle, token);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return $"instantiation {instantiated.Outcome} {instantiated.Reason}";
        }

        var request = new VmInvocationRequest(new VmUtf8Text(Encoding.UTF8.GetBytes(entry)));
        var result = instance.Invoke(in request, token);
        var lines = new List<string>();

        if (!result.TryGetSuspension(out var suspension))
        {
            lines.Add(Line(result.Outcome, result.Reason, result.Diagnostics, result.TryGetPayload<IVmProfilePayload>(out var payload) ? payload : null));
            return string.Join('\n', lines);
        }

        lines.Add(result.TryGetPayload<IVmProfilePayload>(out var projection) ? projection.ToString()! : "suspended");

        while (true)
        {
            var resumed = runtime.Resume(suspension);

            if (!resumed.TryGetSuspension(out suspension))
            {
                lines.Add(Line(resumed.Outcome, resumed.Reason, resumed.Diagnostics, resumed.TryGetPayload<IVmProfilePayload>(out var answer) ? answer : null));
                return string.Join('\n', lines);
            }

            lines.Add(resumed.TryGetPayload<IVmProfilePayload>(out var again) ? again.ToString()! : "suspended");
        }
    }

    private static string Line(VmOutcome outcome, VmReason reason, VmDiagnostics diagnostics, IVmProfilePayload? payload) => outcome switch
    {
        VmOutcome.Normal => payload?.ToString() ?? "completed",
        VmOutcome.ProfileFault when payload is not null => payload.ToString()!,
        VmOutcome.ResourceExhaustion => $"exhausted {diagnostics.ExhaustedDimension}",
        VmOutcome.Cancellation => "cancelled",
        _ => $"{outcome} {reason}",
    };

    /// <summary>
    /// The composition's artifact provider for the fixture family's guest loads: <c>guest-1</c> is a
    /// Tally program, <c>guest-2</c> is the ledger's artifact under the ledger's own descriptor - an
    /// answer the core must refuse as a breach, because a provider may answer a profile only with an
    /// artifact of that profile - and anything else is not found.
    /// </summary>
    private sealed class GuestProvider(ReadOnlyMemory<byte> guest) : IVmArtifactProvider
    {
        public VmCapabilityId CapabilityId => TallyProfile.ProviderCapability.CapabilityId;

        public int Version => TallyProfile.ProviderCapability.Version;

        public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request)
        {
            var name = Encoding.ASCII.GetString(request.RequestPayload.Span);

            if (string.Equals(name, TallyPrograms.GuestName(1), StringComparison.Ordinal))
            {
                var descriptor = Descriptor();
                return VmArtifactProviderAnswer.Provided(in descriptor, guest.Span);
            }

            if (string.Equals(name, TallyPrograms.GuestName(2), StringComparison.Ordinal))
            {
                var foreign = new VmArtifactDescriptor(
                    LedgerProfile.Id, 1, LedgerProfile.Manifest, default,
                    VmCallerIdentity.FromCanonicalIdentity("composition-ubc-fixture://foreign"));
                return VmArtifactProviderAnswer.Provided(in foreign, Foreign);
            }

            return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
        }

        private static readonly byte[] Foreign = LedgerArtifactWriter.Opening(("cash", 10));
    }
}
