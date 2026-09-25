using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The composition half of UBC-1.5: the emitter set, the registration and the descriptor factory. The
/// corpus family is made into a descriptor here exactly as the corpus runner makes it, and the
/// descriptor is registered in no catalog - the roadmap registers none at UBC-1.
/// </summary>
public sealed class UbcCompositionTests
{
    [Fact]
    public void The_Emitter_Set_Refuses_A_Native_Form()
    {
        var exception = Assert.Throws<UbcCompositionException>(() =>
            UbcEmitterSet.Create(new UbcForm("x86-64", 1, new UbcCorpusExecutorFactory())));

        Assert.Equal(UbcCompositionFault.FormNotAdmitted, exception.Fault);

        // Alongside the bytecode form too: one native form refuses the set.
        exception = Assert.Throws<UbcCompositionException>(() => UbcEmitterSet.Create(
            new UbcForm(UbcFormat.BytecodeForm, 1, new UbcCorpusExecutorFactory()),
            new UbcForm("arm64", 1, new UbcCorpusExecutorFactory())));

        Assert.Equal(UbcCompositionFault.FormNotAdmitted, exception.Fault);
    }

    [Fact]
    public void The_Emitter_Set_Refuses_An_Empty_Set_A_Missing_Form_And_A_Form_With_No_Factory()
    {
        Assert.Equal(UbcCompositionFault.NoForm, Assert.Throws<UbcCompositionException>(() => UbcEmitterSet.Create()).Fault);
        Assert.Equal(UbcCompositionFault.NoForm, Assert.Throws<UbcCompositionException>(() => UbcEmitterSet.Create(null!)).Fault);
        Assert.Equal(UbcCompositionFault.NoForm, Assert.Throws<UbcCompositionException>(() => UbcEmitterSet.Create([null!])).Fault);
        Assert.Equal(
            UbcCompositionFault.NoForm,
            Assert.Throws<UbcCompositionException>(() => UbcEmitterSet.Create(new UbcForm(UbcFormat.BytecodeForm, 1, null!))).Fault);
    }

    [Fact]
    public void The_Emitter_Set_Refuses_A_Form_Composed_Twice()
    {
        var exception = Assert.Throws<UbcCompositionException>(() => UbcEmitterSet.Create(
            new UbcForm(UbcFormat.BytecodeForm, 1, new UbcCorpusExecutorFactory()),
            new UbcForm(UbcFormat.BytecodeForm, 2, new UbcCorpusExecutorFactory())));

        Assert.Equal(UbcCompositionFault.DuplicateForm, exception.Fault);
    }

    [Fact]
    public void The_Emitter_Set_Of_The_Bytecode_Form_Finds_It_And_Nothing_Else()
    {
        var forms = UbcCorpusFamily.Forms();

        Assert.True(forms.TryGetForm(UbcFormat.BytecodeForm, out var form));
        Assert.Equal(1, form.SemanticVersion);
        Assert.False(forms.TryGetForm("x86-64", out _));
        Assert.False(forms.TryGetForm("Bytecode", out _));
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(0, 1)]
    public void A_Family_Of_Another_Universal_Bytecode_Contract_Version_Is_Refused(int authored, int builtAgainst)
    {
        var registration = UbcCorpusFamily.Registration(authoredUbcContractVersion: authored, builtAgainstUbcContractVersion: builtAgainst);

        var exception = Assert.Throws<UbcCompositionException>(() =>
            UbcDescriptors.Build(registration, UbcCorpusFamily.Declaration(), UbcCorpusFamily.Forms()));

        Assert.Equal(UbcCompositionFault.ContractVersionMismatch, exception.Fault);
    }

    [Fact]
    public void A_Declaration_Of_Another_Identity_Is_Refused()
    {
        var exception = Assert.Throws<UbcCompositionException>(() => UbcDescriptors.Build(
            UbcCorpusFamily.Registration(),
            UbcCorpusFamily.Declaration(VmProfileId.Parse("com.example.other")),
            UbcCorpusFamily.Forms()));

        Assert.Equal(UbcCompositionFault.IdentityMismatch, exception.Fault);
    }

    [Fact]
    public void A_Family_Of_More_Tables_Than_A_Descriptor_Accepts_Manifests_Is_Refused()
    {
        var tables = Enumerable.Range(0, VmProfileDescriptor.MaximumAcceptedFeatureManifests + 1)
            .Select(static index =>
            {
                UbcInstructionTable.TryCreate(
                    UbcCorpusFamily.Identity, 1, VmFeatureManifestId.Parse($"com.example.ubccorpus.m{index:D2}"),
                    [], [], [], [], false, out var table, out _);
                return table!;
            })
            .ToArray();

        var registration = new UbcFamilyRegistration<UbcCorpusHandlers>(UbcCorpusFamily.Identity, tables, new UbcCorpusHook(), UbcContract.Version);

        var exception = Assert.Throws<UbcCompositionException>(() =>
            UbcDescriptors.Build(registration, UbcCorpusFamily.Declaration(), UbcCorpusFamily.Forms()));

        Assert.Equal(UbcCompositionFault.TooManyTables, exception.Fault);
    }

    [Fact]
    public void A_Registration_Refuses_No_Table_A_Foreign_Table_And_Two_Tables_Of_One_Manifest()
    {
        var hook = new UbcCorpusHook();
        var baseTable = UbcCorpusFamily.BaseTable();

        UbcInstructionTable.TryCreate(
            "com.example.stranger", 1, UbcCorpusFamily.BaseManifest, [], [], [], [], false, out var foreign, out _);

        Assert.Throws<ArgumentException>(() => new UbcFamilyRegistration<UbcCorpusHandlers>(UbcCorpusFamily.Identity, [], hook, 1));
        Assert.Throws<ArgumentException>(() => new UbcFamilyRegistration<UbcCorpusHandlers>(UbcCorpusFamily.Identity, [foreign!], hook, 1));
        Assert.Throws<ArgumentException>(() =>
            new UbcFamilyRegistration<UbcCorpusHandlers>(UbcCorpusFamily.Identity, [baseTable, UbcCorpusFamily.BaseTable()], hook, 1));
        Assert.Throws<ArgumentNullException>(() => new UbcFamilyRegistration<UbcCorpusHandlers>(UbcCorpusFamily.Identity, [baseTable], null!, 1));
    }

    [Fact]
    public void A_Registration_Holds_Its_Tables_In_Manifest_Order_And_Selects_By_Manifest()
    {
        var tables = UbcCorpusFamily.Tables();
        var registration = new UbcFamilyRegistration<UbcCorpusHandlers>(
            UbcCorpusFamily.Identity, [tables[1], tables[0]], new UbcCorpusHook(), UbcContract.Version);

        Assert.Equal(new[] { UbcCorpusFamily.BaseManifest, UbcCorpusFamily.WideManifest }, registration.Tables.Select(static table => table.Manifest));
        Assert.True(registration.TryGetTable(UbcCorpusFamily.WideManifest, out var wide));
        Assert.Equal(UbcCorpusFamily.WideTableVersion, wide.TableVersion);
        Assert.False(registration.TryGetTable(VmFeatureManifestId.Parse("com.example.ubccorpus.absent"), out _));
        Assert.Equal(UbcContract.Version, registration.BuiltAgainstUbcContractVersion);
    }

    [Fact]
    public void The_Built_Descriptor_Carries_The_Declaration_Verbatim_In_Every_Row_The_Family_Owns()
    {
        var declaration = UbcCorpusFamily.Declaration();
        var descriptor = UbcDescriptors.Build(UbcCorpusFamily.Registration(), declaration, UbcCorpusFamily.Forms());

        // Rows 1 to 3.
        Assert.Equal(declaration.ProfileId, descriptor.ProfileId);
        Assert.Equal(declaration.DisplayName, descriptor.DisplayName);
        Assert.Equal(declaration.DescriptorRevision, descriptor.DescriptorRevision);

        // Rows 8 to 30.
        Assert.Equal(declaration.ArtifactRepresentationKind, descriptor.ArtifactRepresentationKind);
        Assert.Equal(declaration.ArtifactLifetimeKind, descriptor.ArtifactLifetimeKind);
        Assert.Equal(declaration.SupportsConcurrentVerification, descriptor.SupportsConcurrentVerification);
        Assert.Equal(declaration.ThreadAffinity, descriptor.ThreadAffinity);
        Assert.Equal(declaration.CancellationPollBound, descriptor.CancellationPollBound);
        Assert.Equal(declaration.AbandonBudget, descriptor.AbandonBudget);
        Assert.Equal(declaration.LimitDefaults, descriptor.LimitDefaults);
        Assert.Equal(declaration.ProfileHardMaxima, descriptor.ProfileHardMaxima);
        Assert.Equal(declaration.BudgetDeclarationMatrix, descriptor.BudgetDeclarationMatrix);
        Assert.Equal(declaration.HostCapabilityDescriptors, descriptor.HostCapabilityDescriptors);
        Assert.Equal(declaration.GuestInitiatedLoads, descriptor.GuestInitiatedLoads);
        Assert.Equal(declaration.AsynchronousInstantiation, descriptor.AsynchronousInstantiation);
        Assert.Equal(declaration.ExternalSuspension, descriptor.ExternalSuspension);
        Assert.Equal(declaration.PayloadKindIdRange, descriptor.PayloadKindIdRange);
        Assert.Equal(declaration.BuiltAgainstCoreContractVersion, descriptor.BuiltAgainstCoreContractVersion);
        Assert.Equal(declaration.AuthoredCoreContractVersion, descriptor.AuthoredCoreContractVersion);
        Assert.Equal(declaration.ConformanceManifestId, descriptor.ConformanceManifestId);
        Assert.Equal(declaration.ConformanceManifestVersion, descriptor.ConformanceManifestVersion);
        Assert.Equal(declaration.DiagnosticsIdentity, descriptor.DiagnosticsIdentity);
        Assert.Equal(declaration.PackageIdentity, descriptor.PackageIdentity);
        Assert.Equal(declaration.ArtifactSharing, descriptor.ArtifactSharing);
        Assert.Equal(declaration.FaultRecovery, descriptor.FaultRecovery);
        Assert.Equal(declaration.MaxUnchargedWork, descriptor.MaxUnchargedWork);
        Assert.Equal(declaration.ChargingGranularity, descriptor.ChargingGranularity);
    }

    [Fact]
    public void Rows_Four_To_Seven_Are_The_Universal_Bytecodes()
    {
        var factory = new UbcCorpusExecutorFactory();
        var declaration = UbcCorpusFamily.Declaration();
        var descriptor = UbcDescriptors.Build(UbcCorpusFamily.Registration(), declaration, UbcCorpusFamily.Forms(factory));

        // Row 4: format version 1 alone.
        Assert.Equal(new VmFormatVersionRange(UbcFormat.FormatVersion, UbcFormat.FormatVersion), descriptor.SupportedFormatVersions);

        // Row 5: the manifest of every table the family registers, in ordinal order.
        Assert.Equal(new[] { UbcCorpusFamily.BaseManifest, UbcCorpusFamily.WideManifest }, descriptor.AcceptedFeatureManifests);

        // Row 6: the one verifier, speaking for the family's identity with the walk's version.
        var verifier = Assert.IsType<UbcVerifier>(descriptor.Verifier);
        Assert.Equal(declaration.ProfileId, verifier.ProfileId);
        Assert.Equal(UbcVerifier.WalkVersion, verifier.VerifierSemanticVersion);
        Assert.Equal(VmCoreContract.Version, verifier.BuiltAgainstCoreContractVersion);
        Assert.Equal(declaration.AuthoredCoreContractVersion, verifier.AuthoredCoreContractVersion);

        // Row 7: the composed form's executor factory, reached with the family's own type argument
        // and the declaration. The corpus family's factory records that and refuses, because nothing
        // executes at UBC-1.
        Assert.Throws<NotSupportedException>(() => descriptor.ExecutorFactory(null!));
        Assert.Equal(typeof(UbcCorpusHandlers), factory.RequestedFamily);
        Assert.Same(declaration, factory.RequestedDeclaration);
    }

    [Fact]
    public void The_Corpus_Family_Executes_Nothing()
    {
        var activation = default(UbcActivation);

        Assert.Throws<NotSupportedException>(() => UbcCorpusHandlers.Handle(ref activation, 0, 0));
        Assert.Throws<NotSupportedException>(() => UbcCorpusHandlers.CreateValuePlane(1));
    }
}
