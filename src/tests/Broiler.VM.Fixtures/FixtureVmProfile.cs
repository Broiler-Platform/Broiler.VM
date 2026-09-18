namespace Broiler.VM.Fixtures;

/// <summary>
/// The deliberately non-conforming fixture profiles VM-1 needs to demonstrate its refusal paths.
/// </summary>
/// <remarks>
/// Every refusal the core declares needs something that actually triggers it. A profile that only
/// ever behaves correctly proves that the happy path works and nothing else, and a rule that has
/// never rejected anything expresses nothing.
/// </remarks>
public enum FixtureVmProfileVariant
{
    /// <summary>Behaves correctly in every respect.</summary>
    Conforming = 0,

    /// <summary>Never charges fuel, so its declared matrix and its behaviour disagree.</summary>
    NonCharging = 1,

    /// <summary>Charges far more than it declares, to drive exhaustion.</summary>
    OverRunning = 2,

    /// <summary>Charges proportionally to variable work rather than one nominal unit per step.</summary>
    Proportionality = 3,

    /// <summary>Performs work between polls beyond its declared bound.</summary>
    PollBoundBreaker = 4,

    /// <summary>Declares no terminal unwind, so an abandoned continuation is simply dropped.</summary>
    NoUnwindEntryPoint = 5,

    /// <summary>Declares guest-initiated loads.</summary>
    DeclaresGuestLoads = 6,

    /// <summary>Declares no guest-initiated loads, so it is handed no mediator at all.</summary>
    DeclaresNoGuestLoads = 7,

    /// <summary>Declares asynchronous instantiation and parks during it.</summary>
    DeclaresAsynchronousInstantiation = 8,

    /// <summary>Suspends during instantiation without declaring that it may.</summary>
    SuspendsDuringInstantiation = 9,

    /// <summary>Declares external suspension.</summary>
    DeclaresExternalSuspension = 10,

    /// <summary>Swallows a terminal nested outcome instead of converting it.</summary>
    MisconvertingNestedOutcome = 11,

    /// <summary>Produces an executor that reports a different profile identity.</summary>
    IdentityMismatchedExecutor = 12,

    /// <summary>Its verifier throws instead of answering.</summary>
    ThrowingVerifier = 13,

    /// <summary>Its verifier answers Normal without producing verified state.</summary>
    StatelessVerifier = 14,

    /// <summary>It accepts several feature manifests, declared out of order.</summary>
    MultiManifest = 15,

    /// <summary>
    /// Conforming, except that it polls on a window rather than after every charge.
    /// </summary>
    /// <remarks>
    /// Every other executor in this repository - the fixture's conforming variant, the calculator,
    /// the ledger and the slice executor - polls immediately after each charge, so almost no charge
    /// ever falls between two polls. A profile that charges in a window between polls is the
    /// ordinary case for a real engine, and nothing exercised it. This variant is the fixture
    /// analogue of a bytecode engine's charge window: it declares a bound of sixty-four work units
    /// and polls once the same number of per-instruction charges have accumulated.
    /// </remarks>
    WindowedPolling = 16,

    /// <summary>
    /// Declares guest-initiated loads, and polls on a window rather than after every charge.
    /// </summary>
    /// <remarks>
    /// A load requested by a variant that polls after every charge is requested by a meter that has
    /// just committed everything it charged, so the remainder handed to the nested verification is
    /// the same whether or not the reader settles first, and nothing about that reader is
    /// exercised. This variant is what puts charges between the last poll and the request. It keeps
    /// the conforming bound rather than the window as its declared latency, so the work a nested
    /// verification charges on the requesting meter still fits between two of the requester's own
    /// polls: polling more often than declared is what a profile is allowed to do.
    /// </remarks>
    WindowedGuestLoads = 17,
}

/// <summary>
/// The primary fixture profile: a bytecode stack machine used to prove the core contract.
/// </summary>
/// <remarks>
/// <para>
/// Its descriptor is exposed through a static accessor on its own type, which is the composition
/// shape the contract requires. There is deliberately no aggregate type naming several profiles: one
/// would reference every profile assembly and defeat the exact-closure gates a composition report
/// depends on.
/// </para>
/// <para>
/// It is test-only and never referenced by a product package. Four independent mechanisms hold that
/// - its path, the literal packability element in its project file, the graph rules at project and
/// assembly level, and the artefact check on a pack - so the containment does not rest on anyone
/// remembering it.
/// </para>
/// </remarks>
public static class FixtureVmProfile
{
    /// <summary>This profile's identity, under the reserved fixture namespace.</summary>
    public static VmProfileId Id { get; } = VmProfileId.Parse("Broiler.VM.Fixture.Alpha");

    /// <summary>Its one accepted feature manifest.</summary>
    public static VmFeatureManifestId Manifest { get; } =
        VmFeatureManifestId.Parse("Broiler.VM.Fixture.Alpha.Base");

    /// <summary>The conforming descriptor, which is what a composition root normally names.</summary>
    public static VmProfileDescriptor Descriptor { get; } = DescriptorFor(FixtureVmProfileVariant.Conforming);

    /// <summary>The descriptor for one deliberately shaped variant.</summary>
    public static VmProfileDescriptor DescriptorFor(FixtureVmProfileVariant variant) =>
        DescriptorFor(variant, orderRecorder: null);

    /// <summary>The descriptor for one variant, with a read-order recorder attached to its verifier.</summary>
    public static VmProfileDescriptor DescriptorFor(
        FixtureVmProfileVariant variant,
        FixtureReadOrderRecorder? orderRecorder) =>
        FixtureDescriptorFactory.Create(
            Id, Manifest, "Fixture Alpha", "Broiler.VM.Fixtures", variant, 1, orderRecorder);

    /// <summary>
    /// The descriptor for one variant, with an execution gate a test uses to hold the executor
    /// inside a step.
    /// </summary>
    /// <remarks>
    /// The gate belongs to the descriptor rather than to the profile type, so two tests running in
    /// parallel hold two different executors and neither can see the other's rendezvous.
    /// </remarks>
    public static VmProfileDescriptor DescriptorFor(
        FixtureVmProfileVariant variant,
        FixtureExecutionGate gate,
        VmThreadAffinity affinity = VmThreadAffinity.Agile) =>
        FixtureDescriptorFactory.Create(
            Id, Manifest, "Fixture Alpha", "Broiler.VM.Fixtures", variant, 1, null, gate, affinity);

    /// <summary>
    /// The descriptor for one variant, handing a test the execution environment and the executor
    /// the core built from it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The environment is the only way to a profile's own meter from outside a step, and a test
    /// that wants to charge the way a capability handler does - or to prove that charging from the
    /// wrong context is refused - needs the same object the executor was handed. It is handed over
    /// at construction rather than exposed as a property, because an executor is built once per
    /// profile per runtime and a property would have to be found before it could be read.
    /// </para>
    /// <para>
    /// The executor comes with it for the same reason: its per-run read-outs, such as the ceiling a
    /// guest-initiated load was verified under, belong to the executor instance the core created,
    /// and nothing else in the composition hands it back.
    /// </para>
    /// </remarks>
    public static VmProfileDescriptor DescriptorFor(
        FixtureVmProfileVariant variant,
        System.Action<IVmExecutionEnvironment>? environmentObserver,
        System.Action<FixtureVmExecutor>? executorObserver = null,
        FixtureExecutionGate? gate = null) =>
        FixtureDescriptorFactory.Create(
            Id, Manifest, "Fixture Alpha", "Broiler.VM.Fixtures", variant, 1, null, gate,
            environmentObserver: environmentObserver,
            executorObserver: executorObserver);

    /// <summary>The descriptor for one variant under a declared thread affinity.</summary>
    public static VmProfileDescriptor DescriptorFor(
        FixtureVmProfileVariant variant,
        VmThreadAffinity affinity) =>
        FixtureDescriptorFactory.Create(
            Id, Manifest, "Fixture Alpha", "Broiler.VM.Fixtures", variant, 1, null, null, affinity);

    /// <summary>
    /// The descriptor for one variant whose verifier runs <paramref name="onFirstPayloadRead"/>
    /// once, after its first payload byte and before the rest of the artifact.
    /// </summary>
    /// <remarks>
    /// The only way to stand inside a verification. Every other cancellation a test can arrange is
    /// already requested when <c>Verify</c> is entered, and the core answers those before the
    /// profile is called at all - so a test that wants the case where a token is cancelled *while*
    /// a verifier is reading has to be handed the moment, exactly as a concurrency test is handed
    /// one by an execution gate. It belongs to a descriptor rather than to the profile type so two
    /// tests in parallel cannot see each other's hook.
    /// </remarks>
    public static VmProfileDescriptor DescriptorForVerifierHook(
        FixtureVmProfileVariant variant,
        System.Action onFirstPayloadRead) =>
        FixtureDescriptorFactory.Create(
            Id, Manifest, "Fixture Alpha", "Broiler.VM.Fixtures", variant, 1,
            onFirstPayloadRead: onFirstPayloadRead);
}

/// <summary>
/// A second, distinct fixture profile with its own value model and its own payload kind range.
/// </summary>
/// <remarks>
/// Two profiles are needed rather than one. A foreign payload, a foreign opaque reference and a
/// two-profile composition are all conditions that need a second profile to exist at all, and the
/// obligation that adding a profile requires no core change cannot be shown with one.
/// </remarks>
public static class SecondFixtureVmProfile
{
    /// <summary>This profile's identity.</summary>
    public static VmProfileId Id { get; } = VmProfileId.Parse("Broiler.VM.Fixture.Beta");

    /// <summary>Its one accepted feature manifest.</summary>
    public static VmFeatureManifestId Manifest { get; } =
        VmFeatureManifestId.Parse("Broiler.VM.Fixture.Beta.Base");

    /// <summary>The conforming descriptor.</summary>
    public static VmProfileDescriptor Descriptor { get; } = DescriptorFor(FixtureVmProfileVariant.Conforming);

    /// <summary>The descriptor for one deliberately shaped variant.</summary>
    public static VmProfileDescriptor DescriptorFor(FixtureVmProfileVariant variant) =>
        FixtureDescriptorFactory.Create(Id, Manifest, "Fixture Beta", "Broiler.VM.Fixtures", variant, 2, null);
}

/// <summary>Builds fixture descriptors, filling every one of the thirty required rows.</summary>
/// <remarks>
/// Every row is supplied explicitly. That is the point of a full-arity constructor with no fluent
/// builder: forgetting a field is a compile error rather than a run-time surprise, and the cost of
/// that is exactly this one place where all thirty are written down.
/// </remarks>
public static class FixtureDescriptorFactory
{
    /// <summary>The payload kind range the first fixture profile owns.</summary>
    public static VmPayloadKindIdRange AlphaPayloadKinds { get; } = new(1, 99);

    /// <summary>The payload kind range the second fixture profile owns, deliberately disjoint.</summary>
    public static VmPayloadKindIdRange BetaPayloadKinds { get; } = new(100, 199);

    /// <summary>Creates a descriptor for one fixture profile and variant.</summary>
    public static VmProfileDescriptor Create(
        VmProfileId profileId,
        VmFeatureManifestId manifest,
        string displayName,
        string packageId,
        FixtureVmProfileVariant variant,
        int ordinal,
        FixtureReadOrderRecorder? orderRecorder = null,
        FixtureExecutionGate? gate = null,
        VmThreadAffinity affinity = VmThreadAffinity.Agile,
        VmLimitVector? profileHardMaxima = null,
        System.Action? onFirstPayloadRead = null,
        System.Action<IVmExecutionEnvironment>? environmentObserver = null,
        System.Action<FixtureVmExecutor>? executorObserver = null)
    {
        VmDiagnosticsIdentity.TryCreate(profileId, profileId + ".diagnostics", out var diagnostics);

        var declaresGuestLoads = variant
            is FixtureVmProfileVariant.DeclaresGuestLoads
            or FixtureVmProfileVariant.MisconvertingNestedOutcome
            or FixtureVmProfileVariant.WindowedGuestLoads;

        var guestLoads = declaresGuestLoads
            ? VmGuestLoadDeclaration.Declared(
                minimumProviderCapabilityVersion: 1,
                profileHardMaxima: new VmGuestLoadBounds(4, 8, 64 * 1024, 1_000_000),
                verifierWorkToFuelRate: 1)
            : VmGuestLoadDeclaration.NotDeclared;

        var asynchronous = variant is FixtureVmProfileVariant.DeclaresAsynchronousInstantiation
            ? VmDeclaration.Declared
            : VmDeclaration.NotDeclared;

        var external = variant is FixtureVmProfileVariant.DeclaresExternalSuspension
            ? VmDeclaration.Declared
            : VmDeclaration.NotDeclared;

        var executorIdentity = variant is FixtureVmProfileVariant.IdentityMismatchedExecutor
            ? VmProfileId.Parse("Broiler.VM.Fixture.Impostor")
            : profileId;

        // Declared deliberately out of ascending order, so that normalization is demonstrable
        // rather than accidental.
        var manifests = variant is FixtureVmProfileVariant.MultiManifest
            ? System.Collections.Immutable.ImmutableArray.Create(
                VmFeatureManifestId.Parse(profileId + ".Zulu"),
                VmFeatureManifestId.Parse(profileId + ".Alfa"),
                manifest)
            : System.Collections.Immutable.ImmutableArray.Create(manifest);

        return new VmProfileDescriptor(
            profileId: profileId,
            displayName: displayName,
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: manifests,
            verifier: new FixtureVmVerifier(
                profileId, semanticVersion: 1, variant: variant, orderRecorder: orderRecorder,
                onFirstPayloadRead: onFirstPayloadRead, pollBound: PollBound(variant)),
            executorFactory: environment =>
            {
                environmentObserver?.Invoke(environment);

                var executor = new FixtureVmExecutor(
                    executorIdentity, environment, variant, chargingGranularity: 1, gate);

                executorObserver?.Invoke(executor);

                return executor;
            },
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: affinity,
            cancellationPollBound: PollBound(variant),
            abandonBudget: 1000,
            limitDefaults: Defaults(),
            profileHardMaxima: profileHardMaxima ?? Maxima(),
            budgetDeclarationMatrix: Matrix(declaresGuestLoads),
            hostCapabilityDescriptors: FixtureHostCapabilities.ImportsFor(variant),
            guestInitiatedLoads: guestLoads,
            asynchronousInstantiation: asynchronous,
            externalSuspension: external,
            payloadKindIdRange: ordinal == 1 ? AlphaPayloadKinds : BetaPayloadKinds,
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create(profileId + ".conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity(packageId, "0.1.0-preview.1", "broiler-vm-core-tests"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: (uint)PollBound(variant),
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    /// <summary>The work a variant promises it will not exceed between two polls.</summary>
    /// <remarks>
    /// The descriptor states the bound twice - once as the declared cancellation latency and once
    /// as the uncharged-work ceiling the core meters against - and they are one number here. A
    /// variant whose two rows disagreed would be exercising the disagreement rather than the bound.
    /// <see cref="FixtureVmProfileVariant.WindowedPolling"/> polls exactly at its declared bound,
    /// so its window and the core's counter are the same size and start in phase.
    /// </remarks>
    private static ulong PollBound(FixtureVmProfileVariant variant) => variant switch
    {
        FixtureVmProfileVariant.PollBoundBreaker => 32UL,
        FixtureVmProfileVariant.WindowedPolling => FixtureVmExecutor.PollWindow,
        _ => 1024UL,
    };

    /// <summary>The profile's bounded defaults. No member encodes "unbounded" or "unset".</summary>
    public static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 1_000_000;
        values[(int)VmBudgetDimension.WallClock] = 30_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 8 * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 10_000;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 8;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 64 * 1024;
        values[(int)VmBudgetDimension.VerifierWork] = 1_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 8 * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 256;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 4;
        values[(int)VmBudgetDimension.ArtifactBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 64;
        values[(int)VmBudgetDimension.DeclaredCount] = 65_536;
        values[(int)VmBudgetDimension.StructuralDepth] = 16;
        values[(int)VmBudgetDimension.LiveRuntimes] = 64;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>The profile's hard maxima, which a host may tighten and never loosen.</summary>
    /// <summary>The hard maxima with one dimension deliberately tightened.</summary>
    /// <remarks>
    /// For pinning what a catalog does with unlike profiles. A maximum is a statement about a
    /// profile's neighbours as much as about itself, and a fixture that cannot vary one cannot
    /// demonstrate that.
    /// </remarks>
    public static VmLimitVector MaximaWith(VmBudgetDimension dimension, ulong value)
    {
        var values = new ulong[VmBudgetDimensions.Count];

        foreach (var current in VmBudgetDimensions.All)
        {
            values[(int)current] = current == dimension ? value : Maxima()[current];
        }

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    public static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 100_000_000;
        values[(int)VmBudgetDimension.WallClock] = 300_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_000_000;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 64;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 4096;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 16;
        values[(int)VmBudgetDimension.ArtifactBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 1024;
        values[(int)VmBudgetDimension.DeclaredCount] = 1_048_576;
        values[(int)VmBudgetDimension.StructuralDepth] = 64;
        values[(int)VmBudgetDimension.LiveRuntimes] = ulong.MaxValue;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>The profile's declaration of which dimensions it charges.</summary>
    public static VmBudgetDeclarationMatrix Matrix(bool declaresGuestLoads)
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        if (!declaresGuestLoads)
        {
            // A profile that does not declare guest loads must also declare the four nested-load
            // dimensions inapplicable, so that the two declarations cannot disagree.
            rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
            rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
            rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;
        }

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}
