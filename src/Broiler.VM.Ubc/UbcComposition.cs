// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           46
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    High
// Criteria:         8/8
// Resource impact:  1/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>
/// What a family hands the universal bytecode at composition: its identity, one instruction table per
/// feature manifest it admits, its verifier hook, and the two universal bytecode contract integers.
/// </summary>
/// <remarks>
/// <para>
/// The handler set, the plane type, the payload factories and the frame codec are the family's
/// <see cref="IUbcFamily"/> struct, carried by the type argument of
/// <see cref="UbcFamilyRegistration{TFamily}"/>, so that a loop generic over it specialises per family.
/// This base exists so that code which does not need the type argument can hold a registration.
/// </para>
/// <para>
/// <b>The two integers.</b> <see cref="AuthoredUbcContractVersion"/> is what the family's author wrote
/// for, and is a required argument. <see cref="BuiltAgainstUbcContractVersion"/> is what the family's
/// compiler saw: its parameter defaults to <see cref="UbcContract.Version"/>, a constant, so the value
/// is fixed into the family's assembly at the family's compilation, and a family compiled against
/// another version of this assembly carries that version's number. <see cref="UbcDescriptors.Build"/>
/// compares both with this assembly's.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=47FF0D
// Broiler-Falsified-If: a registration holds two tables for one manifest, or a table of another family's identity
// Broiler-Human:        PENDING
public abstract class UbcFamilyRegistration
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BBC790
    // Broiler-Falsified-If: a table whose family identity is not the registration's, or a second table for one manifest, is accepted
    // Broiler-Human:        PENDING
    private protected UbcFamilyRegistration(
        string identity,
        System.Collections.Generic.IEnumerable<UbcInstructionTable> tables,
        IUbcFamilyVerifier verifier,
        int authoredUbcContractVersion,
        int builtAgainstUbcContractVersion)
    {
        System.ArgumentNullException.ThrowIfNull(identity);
        System.ArgumentNullException.ThrowIfNull(tables);
        System.ArgumentNullException.ThrowIfNull(verifier);

        var ordered = System.Linq.Enumerable.ToArray(tables);

        if (ordered.Length == 0)
        {
            throw new System.ArgumentException("a family registers at least one table", nameof(tables));
        }

        System.Array.Sort(ordered, static (left, right) => string.CompareOrdinal(left.Manifest.ToString(), right.Manifest.ToString()));

        for (var index = 0; index < ordered.Length; index++)
        {
            if (ordered[index] is null || !string.Equals(ordered[index].FamilyIdentity, identity, System.StringComparison.Ordinal))
            {
                throw new System.ArgumentException($"every table belongs to the family '{identity}'", nameof(tables));
            }

            if (index > 0 && ordered[index].Manifest == ordered[index - 1].Manifest)
            {
                throw new System.ArgumentException($"the manifest '{ordered[index].Manifest}' selects two tables", nameof(tables));
            }
        }

        Identity = identity;
        Tables = ImmutableArray.Create(ordered);
        Verifier = verifier;
        AuthoredUbcContractVersion = authoredUbcContractVersion;
        BuiltAgainstUbcContractVersion = builtAgainstUbcContractVersion;
    }

    /// <summary>The family's identity: the identity its Families row carries.</summary>
    public string Identity { get; }

    /// <summary>One table per feature manifest, in ascending ordinal order of the manifest.</summary>
    public ImmutableArray<UbcInstructionTable> Tables { get; }

    /// <summary>The family's verifier hook.</summary>
    public IUbcFamilyVerifier Verifier { get; }

    /// <summary>The universal bytecode contract version the family's author wrote for.</summary>
    public int AuthoredUbcContractVersion { get; }

    /// <summary>The universal bytecode contract version the family was compiled against.</summary>
    public int BuiltAgainstUbcContractVersion { get; }

    /// <summary>The table <paramref name="manifest"/> selects, or false when the family registers none for it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F951CA
    // Broiler-Human:        PENDING
    public bool TryGetTable(VmFeatureManifestId manifest, out UbcInstructionTable table)
    {
        foreach (var candidate in Tables)
        {
            if (candidate.Manifest == manifest)
            {
                table = candidate;
                return true;
            }
        }

        table = null!;
        return false;
    }

    /// <summary>The family's executor for one runtime, made by the composed form's factory with the family's type argument.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=56FAF0
    // Broiler-Human:        PENDING
    internal abstract IVmProfileExecutor CreateExecutor(
        IUbcExecutorFactory factory,
        UbcFamilyDeclaration declaration,
        IVmExecutionEnvironment environment);
}

/// <summary>
/// The descriptor rows a family declares itself - rows 1 to 3 and 8 to 30 - carried into the
/// descriptor verbatim. Rows 4 to 7 are the universal bytecode's and are not here.
/// </summary>
/// <remarks>
/// The constructor takes the rows under the names and in the order of
/// <see cref="VmProfileDescriptor"/>'s, and computes nothing. As there,
/// <paramref name="builtAgainstCoreContractVersion"/> defaults to the core constant at the family's
/// compilation, and <paramref name="authoredCoreContractVersion"/> is required.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=C3A964
// Broiler-Human:        PENDING
public sealed class UbcFamilyDeclaration
{
    /// <summary>Declares every row a family owns.</summary>
    public UbcFamilyDeclaration(
        VmProfileId profileId,
        string displayName,
        int descriptorRevision,
        VmArtifactRepresentationKind artifactRepresentationKind,
        VmArtifactLifetimeKind artifactLifetimeKind,
        bool supportsConcurrentVerification,
        VmThreadAffinity threadAffinity,
        ulong cancellationPollBound,
        ulong abandonBudget,
        VmLimitVector limitDefaults,
        VmLimitVector profileHardMaxima,
        VmBudgetDeclarationMatrix budgetDeclarationMatrix,
        ImmutableArray<VmCapabilityImport> hostCapabilityDescriptors,
        VmGuestLoadDeclaration guestInitiatedLoads,
        VmDeclaration asynchronousInstantiation,
        VmDeclaration externalSuspension,
        VmPayloadKindIdRange payloadKindIdRange,
        int authoredCoreContractVersion,
        VmConformanceManifestId conformanceManifestId,
        int conformanceManifestVersion,
        VmDiagnosticsIdentity diagnosticsIdentity,
        VmPackageIdentity packageIdentity,
        VmFaultRecovery faultRecovery,
        uint maxUnchargedWork,
        uint chargingGranularity,
        VmArtifactSharing artifactSharing = VmArtifactSharing.RuntimeScoped,
        int builtAgainstCoreContractVersion = VmCoreContract.Version)
    {
        ProfileId = profileId;
        DisplayName = displayName;
        DescriptorRevision = descriptorRevision;
        ArtifactRepresentationKind = artifactRepresentationKind;
        ArtifactLifetimeKind = artifactLifetimeKind;
        SupportsConcurrentVerification = supportsConcurrentVerification;
        ThreadAffinity = threadAffinity;
        CancellationPollBound = cancellationPollBound;
        AbandonBudget = abandonBudget;
        LimitDefaults = limitDefaults;
        ProfileHardMaxima = profileHardMaxima;
        BudgetDeclarationMatrix = budgetDeclarationMatrix;
        HostCapabilityDescriptors = hostCapabilityDescriptors;
        GuestInitiatedLoads = guestInitiatedLoads;
        AsynchronousInstantiation = asynchronousInstantiation;
        ExternalSuspension = externalSuspension;
        PayloadKindIdRange = payloadKindIdRange;
        AuthoredCoreContractVersion = authoredCoreContractVersion;
        ConformanceManifestId = conformanceManifestId;
        ConformanceManifestVersion = conformanceManifestVersion;
        DiagnosticsIdentity = diagnosticsIdentity;
        PackageIdentity = packageIdentity;
        FaultRecovery = faultRecovery;
        MaxUnchargedWork = maxUnchargedWork;
        ChargingGranularity = chargingGranularity;
        ArtifactSharing = artifactSharing;
        BuiltAgainstCoreContractVersion = builtAgainstCoreContractVersion;
    }

    /// <summary>Row 1.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>Row 2.</summary>
    public string DisplayName { get; }

    /// <summary>Row 3.</summary>
    public int DescriptorRevision { get; }

    /// <summary>Row 8.</summary>
    public VmArtifactRepresentationKind ArtifactRepresentationKind { get; }

    /// <summary>Row 9.</summary>
    public VmArtifactLifetimeKind ArtifactLifetimeKind { get; }

    /// <summary>Row 10.</summary>
    public bool SupportsConcurrentVerification { get; }

    /// <summary>Row 11.</summary>
    public VmThreadAffinity ThreadAffinity { get; }

    /// <summary>Row 12.</summary>
    public ulong CancellationPollBound { get; }

    /// <summary>Row 13.</summary>
    public ulong AbandonBudget { get; }

    /// <summary>Row 14.</summary>
    public VmLimitVector LimitDefaults { get; }

    /// <summary>Row 15.</summary>
    public VmLimitVector ProfileHardMaxima { get; }

    /// <summary>Row 16.</summary>
    public VmBudgetDeclarationMatrix BudgetDeclarationMatrix { get; }

    /// <summary>Row 17.</summary>
    public ImmutableArray<VmCapabilityImport> HostCapabilityDescriptors { get; }

    /// <summary>Row 18.</summary>
    public VmGuestLoadDeclaration GuestInitiatedLoads { get; }

    /// <summary>Row 19.</summary>
    public VmDeclaration AsynchronousInstantiation { get; }

    /// <summary>Row 20.</summary>
    public VmDeclaration ExternalSuspension { get; }

    /// <summary>Row 21.</summary>
    public VmPayloadKindIdRange PayloadKindIdRange { get; }

    /// <summary>Row 23.</summary>
    public int AuthoredCoreContractVersion { get; }

    /// <summary>Row 24.</summary>
    public VmConformanceManifestId ConformanceManifestId { get; }

    /// <summary>Row 24, its version.</summary>
    public int ConformanceManifestVersion { get; }

    /// <summary>Row 25.</summary>
    public VmDiagnosticsIdentity DiagnosticsIdentity { get; }

    /// <summary>Row 26.</summary>
    public VmPackageIdentity PackageIdentity { get; }

    /// <summary>Row 28.</summary>
    public VmFaultRecovery FaultRecovery { get; }

    /// <summary>Row 29.</summary>
    public uint MaxUnchargedWork { get; }

    /// <summary>Row 30.</summary>
    public uint ChargingGranularity { get; }

    /// <summary>Row 27.</summary>
    public VmArtifactSharing ArtifactSharing { get; }

    /// <summary>Row 22.</summary>
    public int BuiltAgainstCoreContractVersion { get; }
}

/// <summary>
/// Makes a form's executor for one family: implemented once per emitter, generic over the family so
/// the executor it returns is specialised for it.
/// </summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=A9D5B1
// Broiler-Falsified-If: an executor is made for a family by any path but this member, or with a type argument other than the registration's
// Broiler-Human:        PENDING
public interface IUbcExecutorFactory
{
    /// <summary>The executor of <paramref name="family"/> for the runtime <paramref name="environment"/> serves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=BB6186
    // Broiler-Falsified-If: an executor is returned that runs a program with a family other than the type argument's
    // Broiler-Human:        PENDING
    IVmProfileExecutor Create<TFamily>(
        UbcFamilyRegistration<TFamily> family,
        UbcFamilyDeclaration declaration,
        IVmExecutionEnvironment environment)
        where TFamily : struct, IUbcFamily;
}

/// <summary>One form an image composes: its identity, its semantic version and its executor factory.</summary>
/// <remarks>
/// At this contract version the only form an image can compose is <see cref="UbcFormat.BytecodeForm"/>.
/// A native form carries an Emission section, and admitting one needs a form verifier over emitted bytes
/// and the emitting half of a native emitter - the mechanism the extraction record, ADR 0013, did not
/// admit - so <see cref="UbcEmitterSet.Create"/> refuses any other identity, and the walk refuses an
/// artifact naming one as a form the image does not compose.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=478E3B
// Broiler-Human:        PENDING
public sealed class UbcForm
{
    /// <summary>A form.</summary>
    public UbcForm(string identity, int semanticVersion, IUbcExecutorFactory executorFactory)
    {
        Identity = identity;
        SemanticVersion = semanticVersion;
        ExecutorFactory = executorFactory;
    }

    /// <summary>The form identity an artifact's header names.</summary>
    public string Identity { get; }

    /// <summary>The emitter's semantic version.</summary>
    public int SemanticVersion { get; }

    /// <summary>The factory of the form's executor.</summary>
    public IUbcExecutorFactory ExecutorFactory { get; }
}

/// <summary>The forms one image composes for one family: at least one, each identity once.</summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=CD3796
// Broiler-Falsified-If: a set is created empty, with one identity twice, or with a form this contract version cannot verify
// Broiler-Human:        PENDING
public sealed class UbcEmitterSet
{
    private UbcEmitterSet(ImmutableArray<UbcForm> forms) => Forms = forms;

    /// <summary>The forms, in the order given.</summary>
    public ImmutableArray<UbcForm> Forms { get; }

    /// <summary>
    /// A set of <paramref name="forms"/>. Throws <see cref="UbcCompositionException"/> when the set is
    /// empty, names one identity twice, or names a form this contract version does not admit.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=843C4C
    // Broiler-Falsified-If: a native form identity is admitted into a set
    // Broiler-Human:        PENDING
    public static UbcEmitterSet Create(params UbcForm[] forms)
    {
        if (forms is null || forms.Length == 0)
        {
            throw new UbcCompositionException(UbcCompositionFault.NoForm, "an image composes at least one form");
        }

        for (var index = 0; index < forms.Length; index++)
        {
            var form = forms[index] ?? throw new UbcCompositionException(UbcCompositionFault.NoForm, "a form is missing");

            if (form.ExecutorFactory is null)
            {
                throw new UbcCompositionException(UbcCompositionFault.NoForm, $"the form '{form.Identity}' has no executor factory");
            }

            if (!string.Equals(form.Identity, UbcFormat.BytecodeForm, System.StringComparison.Ordinal))
            {
                throw new UbcCompositionException(
                    UbcCompositionFault.FormNotAdmitted,
                    $"the form '{form.Identity}' carries emitted code, and no form verifier for emitted code exists at universal bytecode contract version {UbcContract.Version}");
            }

            for (var earlier = 0; earlier < index; earlier++)
            {
                if (string.Equals(forms[earlier].Identity, form.Identity, System.StringComparison.Ordinal))
                {
                    throw new UbcCompositionException(UbcCompositionFault.DuplicateForm, $"the form '{form.Identity}' is composed twice");
                }
            }
        }

        return new UbcEmitterSet(ImmutableArray.Create(forms));
    }

    /// <summary>The form named <paramref name="identity"/>, or false when the set does not compose it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=38AC90
    // Broiler-Human:        PENDING
    public bool TryGetForm(string identity, out UbcForm form)
    {
        foreach (var candidate in Forms)
        {
            if (string.Equals(candidate.Identity, identity, System.StringComparison.Ordinal))
            {
                form = candidate;
                return true;
            }
        }

        form = null!;
        return false;
    }
}

/// <summary>Why a family could not be made into a descriptor.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=107080
// Broiler-Human:        PENDING
public enum UbcCompositionFault
{
    /// <summary>The family was compiled against, or written for, another universal bytecode contract version.</summary>
    ContractVersionMismatch = 1,

    /// <summary>The declaration's profile identity is not the family's identity.</summary>
    IdentityMismatch = 2,

    /// <summary>The emitter set is empty, or a form in it is incomplete.</summary>
    NoForm = 3,

    /// <summary>The emitter set names one form twice.</summary>
    DuplicateForm = 4,

    /// <summary>The emitter set names a form this contract version does not admit.</summary>
    FormNotAdmitted = 5,

    /// <summary>The family registers more tables than one descriptor may accept manifests.</summary>
    TooManyTables = 6,
}

/// <summary>A family that cannot be composed, thrown while a composition root builds its catalog.</summary>
/// <remarks>
/// Thrown rather than answered because it is a composition error: the image is wrong and no artifact
/// can make it right, which is the same reason the core's catalog throws on an invalid descriptor.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=11A7FF
// Broiler-Human:        PENDING
public sealed class UbcCompositionException : System.Exception
{
    /// <summary>A composition error of <paramref name="fault"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AF0CFC
    // Broiler-Human:        PENDING
    public UbcCompositionException(UbcCompositionFault fault, string message)
        : base(message) => Fault = fault;

    /// <summary>What was wrong.</summary>
    public UbcCompositionFault Fault { get; }
}

/// <summary>
/// The descriptor factory: a family's registration and declaration, and the forms the image composes,
/// made into one core descriptor whose rows 4 to 7 are the universal bytecode's.
/// </summary>
/// <remarks>
/// <para>
/// Row 4 is format version 1 alone. Row 5 is the manifest of every table the family registers. Row 6 is
/// the one verifier, <see cref="UbcVerifier"/>, which runs the walk, the family's hook and the form
/// check. Row 7 makes the composed form's executor, specialised for the family. Every other row is the
/// declaration's, verbatim.
/// </para>
/// <para>
/// At this contract version an image composes one form, so row 7 has no routing to do; the verifier
/// refuses an artifact naming any other form before an executor could be asked to run it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=221939
// Broiler-Falsified-If: a descriptor is built for a family of another universal bytecode contract version, or a row the declaration owns differs from the declaration's
// Broiler-Human:        PENDING
public static class UbcDescriptors
{
    /// <summary>
    /// Builds the descriptor. Throws <see cref="UbcCompositionException"/> when the family was compiled
    /// against or written for another universal bytecode contract version, when the declaration's
    /// identity is not the family's, or when the family registers more tables than a descriptor accepts.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=058D0F
    // Broiler-Falsified-If: a mismatched contract version or identity yields a descriptor rather than an exception
    // Broiler-Human:        PENDING
    public static VmProfileDescriptor Build<TFamily>(
        UbcFamilyRegistration<TFamily> family,
        UbcFamilyDeclaration declaration,
        UbcEmitterSet forms)
        where TFamily : struct, IUbcFamily
    {
        System.ArgumentNullException.ThrowIfNull(family);
        System.ArgumentNullException.ThrowIfNull(declaration);
        System.ArgumentNullException.ThrowIfNull(forms);

        if (family.BuiltAgainstUbcContractVersion != UbcContract.Version || family.AuthoredUbcContractVersion != UbcContract.Version)
        {
            throw new UbcCompositionException(
                UbcCompositionFault.ContractVersionMismatch,
                $"the family '{family.Identity}' was written for universal bytecode contract version {family.AuthoredUbcContractVersion} " +
                $"and compiled against {family.BuiltAgainstUbcContractVersion}; this assembly is version {UbcContract.Version}");
        }

        if (!string.Equals(declaration.ProfileId.ToString(), family.Identity, System.StringComparison.Ordinal))
        {
            throw new UbcCompositionException(
                UbcCompositionFault.IdentityMismatch,
                $"the declaration names the profile '{declaration.ProfileId}' and the family is '{family.Identity}'");
        }

        if (family.Tables.Length > VmProfileDescriptor.MaximumAcceptedFeatureManifests)
        {
            throw new UbcCompositionException(
                UbcCompositionFault.TooManyTables,
                $"a descriptor accepts at most {VmProfileDescriptor.MaximumAcceptedFeatureManifests} manifests");
        }

        var manifests = ImmutableArray.CreateBuilder<VmFeatureManifestId>(family.Tables.Length);

        foreach (var table in family.Tables)
        {
            manifests.Add(table.Manifest);
        }

        var form = forms.Forms[0];
        var verifier = new UbcVerifier(declaration, family, forms);

        return new VmProfileDescriptor(
            profileId: declaration.ProfileId,
            displayName: declaration.DisplayName,
            descriptorRevision: declaration.DescriptorRevision,
            supportedFormatVersions: new VmFormatVersionRange(UbcFormat.FormatVersion, UbcFormat.FormatVersion),
            acceptedFeatureManifests: manifests.MoveToImmutable(),
            verifier: verifier,
            executorFactory: environment => family.CreateExecutor(form.ExecutorFactory, declaration, environment),
            artifactRepresentationKind: declaration.ArtifactRepresentationKind,
            artifactLifetimeKind: declaration.ArtifactLifetimeKind,
            supportsConcurrentVerification: declaration.SupportsConcurrentVerification,
            threadAffinity: declaration.ThreadAffinity,
            cancellationPollBound: declaration.CancellationPollBound,
            abandonBudget: declaration.AbandonBudget,
            limitDefaults: declaration.LimitDefaults,
            profileHardMaxima: declaration.ProfileHardMaxima,
            budgetDeclarationMatrix: declaration.BudgetDeclarationMatrix,
            hostCapabilityDescriptors: declaration.HostCapabilityDescriptors,
            guestInitiatedLoads: declaration.GuestInitiatedLoads,
            asynchronousInstantiation: declaration.AsynchronousInstantiation,
            externalSuspension: declaration.ExternalSuspension,
            payloadKindIdRange: declaration.PayloadKindIdRange,
            authoredCoreContractVersion: declaration.AuthoredCoreContractVersion,
            conformanceManifestId: declaration.ConformanceManifestId,
            conformanceManifestVersion: declaration.ConformanceManifestVersion,
            diagnosticsIdentity: declaration.DiagnosticsIdentity,
            packageIdentity: declaration.PackageIdentity,
            faultRecovery: declaration.FaultRecovery,
            maxUnchargedWork: declaration.MaxUnchargedWork,
            chargingGranularity: declaration.ChargingGranularity,
            artifactSharing: declaration.ArtifactSharing,
            builtAgainstCoreContractVersion: declaration.BuiltAgainstCoreContractVersion);
    }
}
