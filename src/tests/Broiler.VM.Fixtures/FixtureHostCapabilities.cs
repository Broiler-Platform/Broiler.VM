namespace Broiler.VM.Fixtures;

/// <summary>
/// The host capabilities the fixture profiles import, and the handlers a test registers for them.
/// </summary>
/// <remarks>
/// The set deliberately includes capabilities that misbehave. A throwing handler, a refusing one and
/// an unregistered optional import are how the exception-translation modes, the policy-refusal path
/// and the optional-binding path become demonstrable rather than declared.
/// </remarks>
public static class FixtureHostCapabilities
{
    /// <summary>A capability that doubles its argument.</summary>
    public static VmCapabilityId DoubleId { get; } = VmCapabilityId.Parse("Broiler.VM.Fixture.Double");

    /// <summary>A capability whose handler throws.</summary>
    public static VmCapabilityId ThrowingId { get; } = VmCapabilityId.Parse("Broiler.VM.Fixture.Throwing");

    /// <summary>A capability whose handler refuses as a matter of policy.</summary>
    public static VmCapabilityId RefusingId { get; } = VmCapabilityId.Parse("Broiler.VM.Fixture.Refusing");

    /// <summary>An optional capability a composition may leave unregistered.</summary>
    public static VmCapabilityId OptionalId { get; } = VmCapabilityId.Parse("Broiler.VM.Fixture.Optional");

    /// <summary>The artifact-provider capability.</summary>
    public static VmCapabilityId ProviderId { get; } = VmCapabilityId.Parse("Broiler.VM.Fixture.Provider");

    /// <summary>The signature every integer-valued fixture capability declares.</summary>
    public static VmCapabilitySignatureId IntegerSignature { get; } =
        VmCapabilitySignatureId.FromCanonicalDescription("(i64)->i64");

    /// <summary>The signature the provider declares.</summary>
    public static VmCapabilitySignatureId ProviderSignature { get; } =
        VmCapabilitySignatureId.FromCanonicalDescription("(bytes)->artifact");

    /// <summary>Binding index zero: the doubling capability.</summary>
    public const int DoubleBinding = 0;

    /// <summary>Binding index one: the throwing capability.</summary>
    public const int ThrowingBinding = 1;

    /// <summary>Binding index two: the refusing capability.</summary>
    public const int RefusingBinding = 2;

    /// <summary>Binding index three: the optional capability.</summary>
    public const int OptionalBinding = 3;

    /// <summary>The doubling capability's shape.</summary>
    public static VmHostCapabilityDescriptor Double { get; } = new(
        DoubleId, 1, IntegerSignature, VmCapabilityKind.Value,
        VmCapabilityReentrancy.NonReentrant, VmCapabilityThreadAffinity.CallerThread,
        VmExceptionTranslation.TerminateOperation);

    /// <summary>The throwing capability's shape, which asks for its fault to be observable.</summary>
    public static VmHostCapabilityDescriptor Throwing { get; } = new(
        ThrowingId, 1, IntegerSignature, VmCapabilityKind.Value,
        VmCapabilityReentrancy.NonReentrant, VmCapabilityThreadAffinity.CallerThread,
        VmExceptionTranslation.ObservableFault);

    /// <summary>The refusing capability's shape.</summary>
    public static VmHostCapabilityDescriptor Refusing { get; } = new(
        RefusingId, 1, IntegerSignature, VmCapabilityKind.Value,
        VmCapabilityReentrancy.NonReentrant, VmCapabilityThreadAffinity.CallerThread,
        VmExceptionTranslation.TerminateOperation);

    /// <summary>The optional capability's shape.</summary>
    public static VmHostCapabilityDescriptor Optional { get; } = new(
        OptionalId, 1, IntegerSignature, VmCapabilityKind.Value,
        VmCapabilityReentrancy.NonReentrant, VmCapabilityThreadAffinity.CallerThread,
        VmExceptionTranslation.TerminateOperation);

    /// <summary>The provider capability's shape.</summary>
    public static VmHostCapabilityDescriptor Provider { get; } = new(
        ProviderId, 1, ProviderSignature, VmCapabilityKind.ArtifactProvider,
        VmCapabilityReentrancy.NonReentrant, VmCapabilityThreadAffinity.CallerThread,
        VmExceptionTranslation.TerminateOperation);

    /// <summary>What a fixture profile of the given variant declares it imports.</summary>
    public static System.Collections.Immutable.ImmutableArray<VmCapabilityImport> ImportsFor(
        FixtureVmProfileVariant variant)
    {
        var declaresGuestLoads = variant
            is FixtureVmProfileVariant.DeclaresGuestLoads
            or FixtureVmProfileVariant.MisconvertingNestedOutcome;

        var builder = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmCapabilityImport>();

        builder.Add(new VmCapabilityImport(Double, VmCapabilityImportKind.Required));
        builder.Add(new VmCapabilityImport(Throwing, VmCapabilityImportKind.Required));
        builder.Add(new VmCapabilityImport(Refusing, VmCapabilityImportKind.Required));
        builder.Add(new VmCapabilityImport(Optional, VmCapabilityImportKind.Optional));

        if (declaresGuestLoads)
        {
            // Optional, deliberately. A profile that CAN request code must still run in a
            // composition that forbids it - that is exactly the content-policy case, where the host
            // registers no provider and every request is refused. A required import would turn a
            // policy decision into a composition failure and make the refusal path unreachable.
            builder.Add(new VmCapabilityImport(Provider, VmCapabilityImportKind.Optional));
        }

        return builder.ToImmutable();
    }

    /// <summary>
    /// The doubling capability's handler.
    /// </summary>
    /// <remarks>
    /// The handlers are exposed as plain methods rather than as ready-made registrations, because a
    /// registration type lives in <c>Broiler.VM.Runtime</c> and a profile package never references
    /// the runtime. Binding a capability is the composition root's job; declaring what shape it
    /// needs is the profile's. The split is not a nuisance here - it is the rule that keeps a
    /// profile from being able to wire itself to a host.
    /// </remarks>
    public static VmHostCallOutcome DoubleHandler(System.ReadOnlySpan<long> arguments, out long result)
    {
        result = arguments.Length > 0 ? arguments[0] * 2 : 0;
        return VmHostCallOutcome.Completed;
    }

    /// <summary>A handler that always throws, so exception translation has something to translate.</summary>
    public static VmHostCallOutcome ThrowingHandler(System.ReadOnlySpan<long> arguments, out long result)
    {
        result = 0;
        throw new System.InvalidOperationException("The fixture throwing capability always throws.");
    }

    /// <summary>
    /// A handler that declines as a matter of policy.
    /// </summary>
    /// <remarks>
    /// A policy refusal is a returned value, not a thrown exception, so refusing costs no throw on a
    /// path a guest can drive as often as it likes.
    /// </remarks>
    public static VmHostCallOutcome RefusingHandler(System.ReadOnlySpan<long> arguments, out long result)
    {
        result = 0;
        return VmHostCallOutcome.Refused;
    }
}

/// <summary>The test-only artifact provider that answers a fixture guest-initiated load.</summary>
/// <remarks>
/// Never referenced by a product package, and no product composition registers one. The core ships
/// no provider, so every guest-initiated load in a Broiler-advertised composition is refused - and
/// registering none is itself the content policy.
/// </remarks>
public sealed class FixtureArtifactProvider : IVmArtifactProvider
{
    private readonly byte[] answer;
    private readonly VmArtifactProviderAnswerKind kind;
    private readonly bool throws;
    private readonly VmProfileId answerProfileId;

    /// <summary>Creates a provider that answers with <paramref name="answer"/>.</summary>
    public FixtureArtifactProvider(VmProfileId answerProfileId, byte[] answer)
    {
        this.answerProfileId = answerProfileId;
        this.answer = answer;
        kind = VmArtifactProviderAnswerKind.Provided;
    }

    private FixtureArtifactProvider(VmArtifactProviderAnswerKind kind, bool throws)
    {
        this.kind = kind;
        this.throws = throws;
        answer = System.Array.Empty<byte>();
    }

    /// <summary>A provider that always declines as a matter of policy.</summary>
    public static FixtureArtifactProvider Refusing() => new(VmArtifactProviderAnswerKind.Refused, throws: false);

    /// <summary>A provider that has nothing matching any request.</summary>
    public static FixtureArtifactProvider NotFound() => new(VmArtifactProviderAnswerKind.NotFound, throws: false);

    /// <summary>A provider that throws, which is a host fault and not a refusal.</summary>
    public static FixtureArtifactProvider Throwing() => new(VmArtifactProviderAnswerKind.Provided, throws: true);

    /// <summary>How many requests this provider has been asked to answer.</summary>
    public int RequestCount { get; private set; }

    /// <inheritdoc/>
    public VmCapabilityId CapabilityId => FixtureHostCapabilities.ProviderId;

    /// <inheritdoc/>
    public int Version => 1;

    /// <inheritdoc/>
    public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request)
    {
        RequestCount++;

        if (throws)
        {
            throw new System.InvalidOperationException("The fixture throwing provider always throws.");
        }

        switch (kind)
        {
            case VmArtifactProviderAnswerKind.Refused:
                return VmArtifactProviderAnswer.Refused(VmReason.ProviderRefused);

            case VmArtifactProviderAnswerKind.NotFound:
                return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
        }

        var descriptor = new VmArtifactDescriptor(
            answerProfileId,
            FixtureFormat.FormatVersion,
            answerProfileId.Equals(FixtureVmProfile.Id) ? FixtureVmProfile.Manifest : SecondFixtureVmProfile.Manifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("fixture://provider"));

        return VmArtifactProviderAnswer.Provided(in descriptor, answer);
    }
}
