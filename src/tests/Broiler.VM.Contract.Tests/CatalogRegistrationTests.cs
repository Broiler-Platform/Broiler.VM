using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// G1, G2, G3 and G4 of the VM-1 gate: deterministic registration, duplicate and confusable
/// rejection, unknown-profile and unsupported-version failures, and catalog-order independence.
/// </summary>
public sealed class CatalogRegistrationTests
{
    [Fact]
    public void A_Descriptor_Registers_And_Is_Retrievable_By_Exact_Identity()
    {
        var catalog = FixtureComposition.AlphaCatalog();

        Assert.Equal(1, catalog.Count);
        Assert.True(catalog.TryGetEntry(FixtureVmProfile.Id, out var entry));
        Assert.Equal(FixtureVmProfile.Id, entry.ProfileId);
        Assert.Equal("Fixture Alpha", entry.DisplayName);
    }

    [Fact]
    public void Lookup_Is_Ordinal_And_Never_Folded()
    {
        // Folding is the uniqueness rule and only the uniqueness rule. Applying it to a lookup
        // would let two spellings select one entry, and the identity a handle recorded would then
        // depend on which spelling arrived first.
        var catalog = FixtureComposition.AlphaCatalog();

        Assert.True(VmProfileId.TryParse("broiler.vm.fixture.alpha", out var lowered));
        Assert.False(catalog.TryGetEntry(lowered, out _));
    }

    [Fact]
    public void A_Duplicate_Identity_Is_Rejected_At_Add()
    {
        var builder = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor);

        var failure = Assert.Throws<VmCatalogValidationException>(
            () => builder.Add(FixtureVmProfile.Descriptor));

        Assert.Equal(VmCatalogValidationReason.DuplicateProfileId, failure.Reason);
        Assert.True(failure.HasOffendingProfileId);
        Assert.Equal(FixtureVmProfile.Id, failure.OffendingProfileId);
    }

    [Fact]
    public void A_Confusable_Identity_Is_Rejected_Under_The_Ascii_Fold()
    {
        var confusable = FixtureDescriptorFactory.Create(
            VmProfileId.Parse("Broiler.VM.Fixture.ALPHA"),
            VmFeatureManifestId.Parse("Broiler.VM.Fixture.ALPHA.Base"),
            "Confusable",
            "Broiler.VM.Fixtures",
            FixtureVmProfileVariant.Conforming,
            1);

        var builder = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor);

        var failure = Assert.Throws<VmCatalogValidationException>(() => builder.Add(confusable));

        Assert.Equal(VmCatalogValidationReason.ProfileIdAliasCollision, failure.Reason);
    }

    [Fact]
    public void A_Rejected_Add_Leaves_The_Builder_Usable()
    {
        var builder = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor);

        Assert.Throws<VmCatalogValidationException>(() => builder.Add(FixtureVmProfile.Descriptor));

        // The rejected entry did not land, and the builder is still Building rather than poisoned.
        var catalog = builder.Add(SecondFixtureVmProfile.Descriptor).Build();

        Assert.Equal(2, catalog.Count);
    }

    [Fact]
    public void The_Builder_Is_Single_Use()
    {
        var builder = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor);
        builder.Build();

        var failure = Assert.Throws<VmCatalogValidationException>(() => builder.Build());
        Assert.Equal(VmCatalogValidationReason.BuilderConsumed, failure.Reason);

        var second = Assert.Throws<VmCatalogValidationException>(
            () => builder.Add(SecondFixtureVmProfile.Descriptor));
        Assert.Equal(VmCatalogValidationReason.BuilderConsumed, second.Reason);
    }

    [Fact]
    public void Catalog_Construction_Throws_Rather_Than_Returning_A_Result()
    {
        // Composition is the one place the core throws. A catalog is authored from trusted
        // compile-time data, so a defect in it is a wiring bug that must be loud, and no
        // VmCatalogResult type exists for it to be quietly checked through.
        Assert.Empty(
            typeof(VmCatalog).Assembly
                .GetExportedTypes()
                .Where(type => type.Name.Contains("CatalogResult", StringComparison.Ordinal)));
    }

    [Fact]
    public void An_Empty_Catalog_Is_Legal_And_Hosts_Nothing()
    {
        var catalog = VmCatalog.CreateBuilder().Build();

        Assert.Equal(0, catalog.Count);
        Assert.Equal(0, catalog.GetListing().Count);
    }

    [Fact]
    public void Declaration_Order_Has_No_Observable_Effect()
    {
        var forwards = FixtureComposition.Catalog(FixtureVmProfile.Descriptor, SecondFixtureVmProfile.Descriptor);
        var backwards = FixtureComposition.Catalog(SecondFixtureVmProfile.Descriptor, FixtureVmProfile.Descriptor);

        Assert.Equal(forwards.Count, backwards.Count);
        Assert.Equal(forwards.Identity, backwards.Identity);

        for (var index = 0; index < forwards.Count; index++)
        {
            Assert.Equal(forwards.GetListing()[index].ProfileId, backwards.GetListing()[index].ProfileId);
        }
    }

    [Fact]
    public void The_Canonical_Encoding_Is_Byte_Identical_Across_Permutations()
    {
        // Order independence is asserted at the byte level rather than promised. The encoding is
        // the identity oracle, so two catalogs built from the same descriptors in any order encode
        // to exactly the same bytes.
        var forwards = FixtureComposition.Catalog(FixtureVmProfile.Descriptor, SecondFixtureVmProfile.Descriptor);
        var backwards = FixtureComposition.Catalog(SecondFixtureVmProfile.Descriptor, FixtureVmProfile.Descriptor);

        var left = new byte[forwards.Identity.EncodedLength];
        var right = new byte[backwards.Identity.EncodedLength];

        forwards.Identity.CopyEncodingTo(left);
        backwards.Identity.CopyEncodingTo(right);

        Assert.Equal(left, right);
    }

    [Fact]
    public void The_Listing_Is_The_Only_Route_To_Catalog_Contents()
    {
        // A result that names an unsupported profile carries the requested identity and never the
        // catalog. The split is enforced by type: no result and no diagnostics member is typed as a
        // listing, so there is no verbose mode that could turn disclosure back on.
        var resultTypes = typeof(VmVerificationResult).Assembly
            .GetExportedTypes()
            .Where(type => type.Name.EndsWith("Result", StringComparison.Ordinal));

        foreach (var type in resultTypes)
        {
            foreach (var member in type.GetProperties())
            {
                Assert.DoesNotContain("Catalog", member.PropertyType.Name, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void A_Verifier_Whose_Identity_Disagrees_With_Its_Descriptor_Is_Rejected()
    {
        var mismatched = new VmProfileDescriptor(
            profileId: FixtureVmProfile.Id,
            displayName: "Mismatched",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: [FixtureVmProfile.Manifest],
            verifier: new FixtureVmVerifier(SecondFixtureVmProfile.Id, 1),
            executorFactory: environment => throw new InvalidOperationException(),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 1024,
            abandonBudget: 1000,
            limitDefaults: FixtureDescriptorFactory.Defaults(),
            profileHardMaxima: FixtureDescriptorFactory.Maxima(),
            budgetDeclarationMatrix: FixtureDescriptorFactory.Matrix(false),
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(1, 99),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("x"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: Diagnostics(FixtureVmProfile.Id),
            packageIdentity: new VmPackageIdentity("Broiler.VM.Fixtures", "0.1.0", "tests"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 1024,
            chargingGranularity: 1);

        var failure = Assert.Throws<VmCatalogValidationException>(
            () => VmCatalog.CreateBuilder().Add(mismatched));

        Assert.Equal(VmCatalogValidationReason.VerifierIdentityMismatch, failure.Reason);
    }

    [Fact]
    public void A_Reserved_Identity_Requires_A_Reserved_Package()
    {
        var squatting = FixtureDescriptorFactory.Create(
            FixtureVmProfile.Id,
            FixtureVmProfile.Manifest,
            "Squatting",
            "Contoso.Something",
            FixtureVmProfileVariant.Conforming,
            1);

        var failure = Assert.Throws<VmCatalogValidationException>(
            () => VmCatalog.CreateBuilder().Add(squatting));

        Assert.Equal(VmCatalogValidationReason.ProfileIdReservedNamespace, failure.Reason);
    }

    [Fact]
    public void A_Manifest_Outside_Its_Own_Profile_Namespace_Is_Rejected()
    {
        var foreign = FixtureDescriptorFactory.Create(
            FixtureVmProfile.Id,
            SecondFixtureVmProfile.Manifest,
            "Foreign manifest",
            "Broiler.VM.Fixtures",
            FixtureVmProfileVariant.Conforming,
            1);

        var failure = Assert.Throws<VmCatalogValidationException>(
            () => VmCatalog.CreateBuilder().Add(foreign));

        Assert.Equal(VmCatalogValidationReason.FeatureManifestIdOutOfNamespace, failure.Reason);
    }

    [Fact]
    public void Adding_A_Second_Profile_Requires_No_Core_Change()
    {
        // The obligation is that a second profile is added by naming it, and nothing in the core
        // learns about it. The evidence is that the same catalog, runtime and execution path serve
        // both, with two descriptors instead of one.
        var catalog = FixtureComposition.Catalog(FixtureVmProfile.Descriptor, SecondFixtureVmProfile.Descriptor);
        using var runtime = FixtureComposition.Runtime(catalog);

        var alpha = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(7));

        var betaDescriptor = FixtureComposition.Descriptor(
            SecondFixtureVmProfile.Id, SecondFixtureVmProfile.Manifest);

        var beta = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(9), betaDescriptor);

        Assert.Equal(FixtureVmProfile.Id, alpha.Identity.ProfileId);
        Assert.Equal(SecondFixtureVmProfile.Id, beta.Identity.ProfileId);
    }

    private static VmDiagnosticsIdentity Diagnostics(VmProfileId profileId)
    {
        VmDiagnosticsIdentity.TryCreate(profileId, profileId + ".diagnostics", out var identity);
        return identity;
    }
}
