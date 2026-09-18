// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           4
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;
using Broiler.VM;

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F05F5A
// Broiler-Human:        PENDING
public static class MachineCodeProfile
{
    /// <summary>This profile's identity: broiler.machinecode.</summary>
    public static VmProfileId Id { get; } = VmProfileId.Parse("broiler.machinecode");

    /// <summary>The x86-64 machine code manifest.</summary>
    public static VmFeatureManifestId X64Manifest { get; } =
        VmFeatureManifestId.Parse("broiler.machinecode.x86-64");

    /// <summary>The ARM64 machine code manifest.</summary>
    public static VmFeatureManifestId Arm64Manifest { get; } =
        VmFeatureManifestId.Parse("broiler.machinecode.arm64");

    /// <summary>This profile's descriptor: the one static accessor the contract asks for.</summary>
    public static VmProfileDescriptor Descriptor { get; } = Build();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D5315E
    // Broiler-Human:        PENDING
    private static VmProfileDescriptor Build()
    {
        VmDiagnosticsIdentity.TryCreate(Id, "broiler.machinecode.diagnostics", out var diagnostics);

        return new VmProfileDescriptor(
            profileId: Id,
            displayName: "Broiler MachineCode",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: ImmutableArray.Create(X64Manifest, Arm64Manifest),
            verifier: new MachineCodeVerifier(Id),
            executorFactory: static environment => new MachineCodeExecutor(Id, environment),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 65_536,
            abandonBudget: 0,
            limitDefaults: Defaults(),
            profileHardMaxima: Maxima(),
            budgetDeclarationMatrix: Matrix(),
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(3000, 3099),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("broiler.machinecode.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity(
                "Broiler.VM.Profile.MachineCode", "0.1.0-preview.1", "broiler.machinecode"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 65_536,
            chargingGranularity: 1);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=00D0CA
    // Broiler-Human:        PENDING
    private static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 50_000_000;
        values[(int)VmBudgetDimension.WallClock] = 10_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_000_000;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 1_024;
        values[(int)VmBudgetDimension.ArtifactBytes] = 32L * 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 64;
        values[(int)VmBudgetDimension.DeclaredCount] = 4_194_304;
        values[(int)VmBudgetDimension.StructuralDepth] = 256;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 4;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 4_096;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.LiveRuntimes] = 64;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=162AEB
    // Broiler-Human:        PENDING
    private static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 1_099_511_627_776;
        values[(int)VmBudgetDimension.WallClock] = 3_600_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 4_294_967_296;
        values[(int)VmBudgetDimension.HostCalls] = 4_294_967_295;
        values[(int)VmBudgetDimension.VerifierWork] = 1_099_511_627_776;
        values[(int)VmBudgetDimension.LiveBytes] = 4_294_967_296;
        values[(int)VmBudgetDimension.CallDepth] = 8_192;
        values[(int)VmBudgetDimension.ArtifactBytes] = 536_870_912;
        values[(int)VmBudgetDimension.SectionCount] = 1_024;
        values[(int)VmBudgetDimension.DeclaredCount] = 4_294_967_295;
        values[(int)VmBudgetDimension.StructuralDepth] = 4_096;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 64;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 16_777_216;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 536_870_912;
        values[(int)VmBudgetDimension.LiveRuntimes] = 4_096;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=18E0B0
    // Broiler-Human:        PENDING
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];
        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.NotApplicable;
        }

        rows[(int)VmBudgetDimension.ArtifactBytes] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.SectionCount] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.DeclaredCount] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.StructuralDepth] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.AllocatedBytes] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.VerifierWork] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.Fuel] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.CallDepth] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.LiveBytes] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.WallClock] = VmBudgetApplicability.Charged;

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}
