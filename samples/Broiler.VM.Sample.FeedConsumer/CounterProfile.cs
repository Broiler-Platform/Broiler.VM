using Broiler.VM;
using System.Collections.Immutable;

namespace Broiler.VM.Sample.FeedConsumer;

/// <summary>
/// A whole profile, written against the three packages and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// It counts. An artifact says where to start and how many times to add one, and the executor does
/// exactly that, charging a unit of Fuel for each step. There is no cleverness in it on purpose:
/// what this sample demonstrates is the SHAPE of the source-level profile contract from outside
/// the repository, and a profile with interesting semantics would put its own semantics in front
/// of that.
/// </para>
/// <para>
/// The thirty rows below are the whole of what a profile declares. Every one of them is a promise
/// the core will hold this profile to - what format versions it accepts, what it charges, what it
/// may do and may not - and the descriptor is refused at catalog construction if they do not hang
/// together. A profile author's first hour with Broiler.VM is spent here, so this file is written
/// to be read rather than to be short.
/// </para>
/// </remarks>
internal static class CounterProfile
{
    /// <summary>
    /// This profile's identity.
    /// </summary>
    /// <remarks>
    /// A reverse-DNS name the sample's author controls. It is emphatically not a <c>broiler.</c>
    /// name: that prefix is reserved for profiles shipped by a Broiler package, the catalog checks
    /// it against the declared package identity, and a consumer profile claiming it would be
    /// claiming provenance it does not have.
    /// </remarks>
    internal static VmProfileId Id { get; } = VmProfileId.Parse("com.example.counter");

    internal static VmFeatureManifestId Manifest { get; } =
        VmFeatureManifestId.Parse("com.example.counter.base");

    /// <summary>
    /// The kind identifier this profile stamps on the value it returns.
    /// </summary>
    /// <remarks>
    /// Inside the range the descriptor declares, and checked against it: a payload whose kind is
    /// outside the profile's own declared range is a contract breach the core can see, which is
    /// what stops one profile minting identifiers in another's space.
    /// </remarks>
    internal const int ValueKindId = 900;

    internal static VmProfileDescriptor Descriptor { get; } = Describe();

    private static VmProfileDescriptor Describe()
    {
        if (!VmDiagnosticsIdentity.TryCreate(Id, "com.example.counter.diagnostics", out var diagnostics))
        {
            throw new InvalidOperationException("the counter profile's diagnostics identity is not valid");
        }

        return new VmProfileDescriptor(
            profileId: Id,
            displayName: "Example Counter",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(
                CounterFormat.FormatVersion, CounterFormat.FormatVersion),
            acceptedFeatureManifests: ImmutableArray.Create(Manifest),
            verifier: new CounterVerifier(),
            executorFactory: environment => new CounterExecutor(environment),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 256,
            abandonBudget: 0,
            limitDefaults: Defaults(),
            profileHardMaxima: Maxima(),
            budgetDeclarationMatrix: Matrix(),

            // This profile imports nothing from the host. A profile that needs no capability is a
            // first-class case and the honest declaration for a counter.
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(900, 999),
            authoredCoreContractVersion: VmCoreContract.Version,
            conformanceManifestId: VmConformanceManifestId.Create("com.example.counter.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity(
                "Broiler.VM.Sample.FeedConsumer", "1.0.0", "example-application"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 256,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// What this profile asks for when a host states nothing.
    /// </summary>
    /// <remarks>
    /// No dimension may be left unset and none may be unbounded: the vector is refused otherwise.
    /// That is the rule a profile author is most likely to be surprised by and it is the one worth
    /// being surprised by early - "unlimited" is not expressible, so a profile cannot ship a
    /// default that quietly removes a bound on the host's behalf.
    /// </remarks>
    private static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];

        values[(int)VmBudgetDimension.Fuel] = 1_000_000;
        values[(int)VmBudgetDimension.WallClock] = 30_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 1;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 1;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000;
        values[(int)VmBudgetDimension.LiveBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 1;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 1;
        values[(int)VmBudgetDimension.ArtifactBytes] = 64 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 4;
        values[(int)VmBudgetDimension.DeclaredCount] = 1024;
        values[(int)VmBudgetDimension.StructuralDepth] = 4;
        values[(int)VmBudgetDimension.LiveRuntimes] = 8;

        if (!VmLimitVector.TryCreate(values, out var vector))
        {
            throw new InvalidOperationException("the counter profile's defaults are not a valid limit vector");
        }

        return vector;
    }

    /// <summary>
    /// The most this profile will ever accept, whatever a host asks for.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A host may tighten these and may not loosen them. Setting them close to the defaults is the
    /// conservative choice for a profile this small, and it costs nothing: <b>a maximum binds this
    /// profile's own artifacts and nobody else's</b>, because it is applied at verification against
    /// the profile the artifact names.
    /// </para>
    /// <para>
    /// The declaration that reaches a neighbour is the <c>Defaults</c> vector above. A host that
    /// adopts profile defaults rather than stating numbers gets the tightest default in the
    /// catalog, per dimension, because at runtime creation no profile has been selected and there
    /// is no other safe answer. This profile is composed alone, so nothing here is felt by anyone
    /// else - but a profile written for a mixed catalog should declare a maximum for what it would
    /// tolerate being granted and a default for what it actually needs, and should know that a
    /// stingy default on a dimension it never uses is the one that strangles a neighbour.
    /// </para>
    /// <para>
    /// <b>Corrected 2026-08-31.</b> This remark used to say the maxima were CATALOG-WIDE in
    /// effect - that the tightest maximum for a dimension capped every profile in the catalog, so
    /// a small maximum decided on a neighbour's behalf. That clamp was an implementation defect
    /// rather than a property of the contract; ADR 0007 puts <c>ProfileMax</c> at P2, against the
    /// profile an artifact names, and it has been removed. <c>docs/compositions.md</c> section 5
    /// is the authority.
    /// </para>
    /// </remarks>
    private static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];

        values[(int)VmBudgetDimension.Fuel] = 100_000_000;
        values[(int)VmBudgetDimension.WallClock] = 300_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 1;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 1;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 1;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 1;
        values[(int)VmBudgetDimension.ArtifactBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 16;
        values[(int)VmBudgetDimension.DeclaredCount] = 65_536;
        values[(int)VmBudgetDimension.StructuralDepth] = 16;
        values[(int)VmBudgetDimension.LiveRuntimes] = ulong.MaxValue;

        if (!VmLimitVector.TryCreate(values, out var vector))
        {
            throw new InvalidOperationException("the counter profile's maxima are not a valid limit vector");
        }

        return vector;
    }

    /// <summary>
    /// Which dimensions this profile actually charges, and which it cannot.
    /// </summary>
    /// <remarks>
    /// Declaring a dimension charged and never charging it is a lie a host cannot detect; declaring
    /// it inapplicable is a promise the core can hold. This profile makes no host call, requests no
    /// nested load and never recurses, so those dimensions are inapplicable rather than merely
    /// unused - and a host reading the matrix learns that tightening them would change nothing.
    /// </remarks>
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        rows[(int)VmBudgetDimension.HostCalls] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.CallDepth] = VmBudgetApplicability.NotApplicable;

        if (!VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix))
        {
            throw new InvalidOperationException("the counter profile's declaration matrix is not valid");
        }

        return matrix;
    }
}
