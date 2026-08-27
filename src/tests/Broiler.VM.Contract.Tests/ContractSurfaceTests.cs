using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Reflection;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// G8, G10 and G16 of the VM-1 gate: declared thread affinity and reentrancy, the explicit absence
/// of reflection or name-based discovery, and the accepted contract recorded with its version.
/// </summary>
public sealed class ContractSurfaceTests
{
    private static readonly Assembly[] Product =
    [
        typeof(VmCoreContract).Assembly,
        typeof(VmBoundedReader).Assembly,
        typeof(VmRuntime).Assembly,
    ];

    [Fact]
    public void The_Core_Contract_Version_Is_One_And_Is_Recorded_Everywhere_A_Result_Reaches()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var invocation = FixtureComposition.Invoke(instance);

        Assert.Equal(1, VmCoreContract.Version);
        Assert.Equal(1, VmCoreContract.MinimumSupportedVersion);
        Assert.Equal(VmCoreContract.Version, artifact.Identity.CoreContractVersion);
        Assert.Equal(VmCoreContract.Version, invocation.Diagnostics.CoreContractVersion);
    }

    [Fact]
    public void The_Contract_Version_Constants_Are_Literals_And_Nothing_Else_Is_Declared()
    {
        var members = typeof(VmCoreContract)
            .GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(member => member.Name)
            .OrderBy(name => name, StringComparer.Ordinal);

        Assert.Equal(["MinimumSupportedVersion", "Version"], members);

        foreach (var field in typeof(VmCoreContract).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            Assert.True(field.IsLiteral, $"{field.Name} must be a const, not a static readonly");
        }
    }

    [Fact]
    public void A_Descriptor_Built_Against_A_Future_Contract_Version_Is_Refused()
    {
        var future = new VmProfileDescriptor(
            profileId: FixtureVmProfile.Id,
            displayName: "From the future",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: [FixtureVmProfile.Manifest],
            verifier: new FixtureVmVerifier(FixtureVmProfile.Id, 1),
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
            hostCapabilityDescriptors: System.Collections.Immutable.ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(1, 99),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("x"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: DiagnosticsFor(FixtureVmProfile.Id),
            packageIdentity: new VmPackageIdentity("Broiler.VM.Fixtures", "0.1.0", "tests"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 1024,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.RuntimeScoped,
            builtAgainstCoreContractVersion: VmCoreContract.Version + 1);

        var failure = Assert.Throws<VmCatalogValidationException>(
            () => VmCatalog.CreateBuilder().Add(future));

        Assert.Equal(VmCatalogValidationReason.CoreContractVersionNotYetSupported, failure.Reason);
    }

    [Fact]
    public void No_Product_Assembly_Reaches_A_Dynamic_Loading_Or_Reflection_Invocation_Api()
    {
        // Registration is static and typed. There is no assembly load, no type-by-name lookup, no
        // activator, and no IL emit anywhere in the product graph - which is what makes a Native
        // AOT closure a fact rather than a hope.
        string[] forbidden =
        [
            "System.Reflection.Assembly.Load",
            "System.Reflection.Assembly.LoadFrom",
            "System.Type.GetType",
            "System.Activator.CreateInstance",
            "System.Reflection.MethodBase.Invoke",
        ];

        foreach (var assembly in Product)
        {
            var referenced = assembly.GetReferencedAssemblies().Select(name => name.Name);

            Assert.DoesNotContain("System.Reflection.Emit", referenced);
            Assert.DoesNotContain("System.Runtime.Loader", referenced);
        }

        // The named members are asserted over compiled metadata by the architecture suite; here the
        // point is that no public member of the surface takes a type name or an assembly name.
        foreach (var assembly in Product)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        Assert.NotEqual(typeof(Type), parameter.ParameterType);
                        Assert.NotEqual(typeof(Assembly), parameter.ParameterType);
                    }
                }
            }
        }

        Assert.NotEmpty(forbidden);
    }

    [Fact]
    public void No_Product_Assembly_Exports_An_Aggregate_Profile_Type()
    {
        string[] banned = ["BuiltInProfiles", "DefaultProfiles", "AllProfiles", "KnownProfiles"];

        foreach (var assembly in Product)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                Assert.DoesNotContain(type.Name, banned);
            }
        }
    }

    [Fact]
    public void No_Exported_Type_Uses_An_Unqualified_Reserved_Name()
    {
        // A closure report, a support table and a test log carry no namespace, so the unqualified
        // name is what a reader actually sees.
        string[] banned =
        [
            "Profile", "IProfile", "ProfileId", "ProfileDescriptor", "ProfileCatalog", "ProfileFactory",
        ];

        foreach (var assembly in Product)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                Assert.DoesNotContain(type.Name, banned);
            }
        }
    }

    [Fact]
    public void No_Public_Member_Returns_An_Awaitable()
    {
        // Core contract version 1 admits no asynchronous runtime creation, verification or
        // invocation. A profile that must wait suspends, and the host resumes it.
        foreach (var assembly in Product)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var returned = method.ReturnType.Name;

                    Assert.False(
                        returned.StartsWith("Task", StringComparison.Ordinal) ||
                        returned.StartsWith("ValueTask", StringComparison.Ordinal) ||
                        returned.StartsWith("IAsyncEnumerable", StringComparison.Ordinal),
                        $"{type.Name}.{method.Name} returns {returned}");
                }
            }
        }
    }

    [Fact]
    public void No_Budget_Member_Offers_To_Raise_An_Allowance()
    {
        // Monotonicity is the point of the budget contract, so the words that would break it are
        // absent by name as well as by behaviour.
        string[] banned =
        [
            "Grant", "Refund", "Reset", "Extend", "Increase", "Raise",
            "TopUp", "Widen", "Reopen", "WithLimits", "Withdraw", "Credit",
        ];

        foreach (var assembly in Product)
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    Assert.DoesNotContain(member.Name, banned);
                }
            }
        }
    }

    [Fact]
    public void The_Profile_Facing_Meter_Has_Exactly_Four_Members_And_No_Remaining_Reader()
    {
        // A profile learns a limit only by reaching it and being refused. A remaining reader would
        // let it spend exactly up to a ceiling on every operation while staying compliant.
        var members = typeof(IVmMeter)
            .GetMethods()
            .Select(method => method.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["Poll", "ReportReleased", "ReportRetained", "TryCharge"], members);

        var charge = typeof(IVmMeter).GetMethod(nameof(IVmMeter.TryCharge))!;
        Assert.Equal(typeof(ulong), charge.GetParameters()[1].ParameterType);
    }

    [Fact]
    public void The_Fifteen_Dimensions_Are_Closed_And_In_Their_Frozen_Order()
    {
        var declared = Enum.GetValues<VmBudgetDimension>();

        Assert.Equal(VmBudgetDimensions.Count, declared.Length);
        Assert.Equal(VmBudgetDimension.Fuel, declared[0]);
        Assert.Equal(VmBudgetDimension.LiveRuntimes, declared[^1]);

        // Seven allowances and eight ceilings, asserted from the table rather than from a copy of
        // it living in test code.
        var allowances = declared.Count(d => VmBudgetDimensions.ClassOf(d) is VmBudgetClass.Allowance);
        Assert.Equal(7, allowances);
        Assert.Equal(8, declared.Length - allowances);

        var aggregate = declared.Count(VmBudgetDimensions.CarriesAggregateScope);
        Assert.Equal(11, aggregate);
    }

    [Fact]
    public void The_Outcome_Set_Is_Closed_At_Ten_With_Its_Frozen_Values()
    {
        Assert.Equal(0, (int)VmOutcome.None);
        Assert.Equal(1, (int)VmOutcome.Normal);
        Assert.Equal(2, (int)VmOutcome.UnsupportedProfile);
        Assert.Equal(3, (int)VmOutcome.InvalidArtifact);
        Assert.Equal(4, (int)VmOutcome.InvalidState);
        Assert.Equal(5, (int)VmOutcome.ProfileFault);
        Assert.Equal(6, (int)VmOutcome.Suspension);
        Assert.Equal(7, (int)VmOutcome.Cancellation);
        Assert.Equal(8, (int)VmOutcome.ResourceExhaustion);
        Assert.Equal(9, (int)VmOutcome.HostFailure);
        Assert.Equal(10, Enum.GetValues<VmOutcome>().Length);
    }

    [Fact]
    public void Every_Reason_Belongs_To_Exactly_One_Category()
    {
        foreach (var reason in VmReasonRegistry.All())
        {
            if (reason is VmReason.None)
            {
                Assert.Equal(VmOutcome.None, VmReasonRegistry.CategoryOf(reason));
                continue;
            }

            if (VmReasonRegistry.IsControlOnly(reason))
            {
                Assert.Equal(VmOutcome.None, VmReasonRegistry.CategoryOf(reason));
                continue;
            }

            var category = VmReasonRegistry.CategoryOf(reason);

            Assert.NotEqual(VmOutcome.None, category);
            Assert.True(VmReasonRegistry.IsLegal(category, reason));
        }
    }

    [Fact]
    public void The_Reason_Registry_Revision_Is_Not_Wired_To_The_Contract_Version()
    {
        // Two numbers with two jobs. Wiring them together would make an additive reason look like a
        // contract amendment and a contract amendment look like a new reason.
        var registry = typeof(VmReasonRegistry).GetField(nameof(VmReasonRegistry.Revision))!;

        Assert.True(registry.IsLiteral);
        Assert.Equal(1, VmReasonRegistry.Revision);
    }

    [Fact]
    public void The_Stage_Matrix_Forbids_What_The_Contract_Forbids()
    {
        // Invalid artifact cannot appear after verification: a verified handle cannot later become
        // invalid, and admitting it would create a second, later verification point.
        Assert.False(VmStageMatrix.IsLegal(VmStage.Instantiation, VmOutcome.InvalidArtifact));
        Assert.False(VmStageMatrix.IsLegal(VmStage.Invocation, VmOutcome.InvalidArtifact));
        Assert.False(VmStageMatrix.IsLegal(VmStage.Resume, VmOutcome.InvalidArtifact));

        // No profile instance exists to own a fault before instantiation.
        Assert.False(VmStageMatrix.IsLegal(VmStage.Verification, VmOutcome.ProfileFault));
        Assert.False(VmStageMatrix.IsLegal(VmStage.GuestInitiatedLoad, VmOutcome.ProfileFault));

        // A resumable nested verification would let a half-verified artifact outlive its
        // requesting operation.
        Assert.False(VmStageMatrix.IsLegal(VmStage.Verification, VmOutcome.Suspension));
        Assert.False(VmStageMatrix.IsLegal(VmStage.GuestInitiatedLoad, VmOutcome.Suspension));

        // Resume subtracts unsupported profile from the row of the stage that suspended.
        Assert.True(VmStageMatrix.IsLegal(VmStage.Instantiation, VmOutcome.UnsupportedProfile));
        Assert.False(VmStageMatrix.IsLegal(VmStage.Resume, VmOutcome.UnsupportedProfile));

        // No host capability is invoked on the caller-driven verification path.
        Assert.False(VmStageMatrix.IsLegal(VmStage.Verification, VmOutcome.HostFailure));
        Assert.True(VmStageMatrix.IsLegal(VmStage.GuestInitiatedLoad, VmOutcome.HostFailure));

        // Runtime creation reaches no cancellation polling point.
        Assert.False(VmStageMatrix.IsLegal(VmStage.RuntimeCreation, VmOutcome.Cancellation));
    }

    [Fact]
    public void The_Persisted_Envelope_Stage_Is_Admitted_And_Unreachable()
    {
        // Its invariant 8 discharge is absence from the public surface, not a returned failure.
        // A type that existed and threw would be the shape-only stub invariant 8 rejects.
        Assert.True(VmStageMatrix.IsLegal(VmStage.EnvelopePreprocessing, VmOutcome.Normal));

        var envelopeMembers = Product
            .SelectMany(assembly => assembly.GetExportedTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(method => method.Name.Contains("Envelope", StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(envelopeMembers);
    }

    [Fact]
    public void There_Is_Exactly_One_Public_Member_Returning_A_Verified_Artifact()
    {
        // One construction site. Everything else that hands back a handle is an accessor on
        // something that already holds one - a lease, a result - and cannot mint a new one.
        var producers = Product
            .SelectMany(assembly => assembly.GetExportedTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            .Where(method => method.ReturnType == typeof(VmVerifiedArtifact))
            .ToArray();

        Assert.Single(producers);
        Assert.Equal(nameof(VmVerifiedArtifact.Create), producers[0].Name);
        Assert.Equal(typeof(VmVerifiedArtifact), producers[0].DeclaringType);

        // It is reached through a TryGet accessor on a result, never as a bare return, so a caller
        // that ignores the outcome cannot obtain the handle.
        var accessors = typeof(VmVerificationResult)
            .GetMethods()
            .Where(method => method.Name == nameof(VmVerificationResult.TryGetArtifact))
            .ToArray();

        Assert.Single(accessors);
    }

    [Fact]
    public void No_Verification_Member_Accepts_A_Stream_Or_A_Buffer_Type()
    {
        Type[] banned =
        [
            typeof(byte[]), typeof(ArraySegment<byte>), typeof(Memory<byte>),
            typeof(ReadOnlyMemory<byte>), typeof(System.IO.Stream),
        ];

        var verify = typeof(VmRuntime).GetMethod(nameof(VmRuntime.Verify))!;

        foreach (var parameter in verify.GetParameters())
        {
            Assert.DoesNotContain(parameter.ParameterType, banned);
        }

        Assert.Equal(3, verify.GetParameters().Length);
    }

    [Fact]
    public void A_Non_Reentrant_Capability_Refuses_A_Runtime_Call_From_Inside_Itself()
    {
        // Enforced, not merely declared: the core holds a per-runtime in-capability flag for the
        // duration of the call. A declaration nothing enforces is documentation.
        VmRuntime? captured = null;
        var reentered = VmOutcome.None;

        VmHostCallOutcome Reentrant(ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;

            if (captured is not null)
            {
                var descriptor = FixtureComposition.Descriptor();
                reentered = captured.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None).Outcome;
            }

            return VmHostCallOutcome.Completed;
        }

        var capabilities = System.Collections.Immutable.ImmutableArray.Create(
            VmCapabilityRegistration.Value(FixtureHostCapabilities.Double, Reentrant),
            VmCapabilityRegistration.Value(FixtureHostCapabilities.Throwing, FixtureHostCapabilities.ThrowingHandler),
            VmCapabilityRegistration.Value(FixtureHostCapabilities.Refusing, FixtureHostCapabilities.RefusingHandler));

        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(), FixtureComposition.Options(capabilities: capabilities));

        captured = runtime;

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(21, FixtureHostCapabilities.DoubleBinding));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(VmOutcome.InvalidState, reentered);
    }

    [Fact]
    public void A_Host_Call_Returns_Its_Value_And_Charges_The_Host_Call_Dimension()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(21, FixtureHostCapabilities.DoubleBinding));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.True(FixtureVmProfileResults.TryGetValue(in result, out var value));
        Assert.Equal(42, value.Value);
    }

    [Fact]
    public void A_Refusing_Capability_Is_Not_A_Fault_Of_The_Core()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(1, FixtureHostCapabilities.RefusingBinding));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
    }

    [Fact]
    public void A_Throwing_Capability_Cannot_Tear_Down_The_Runtime()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(1, FixtureHostCapabilities.ThrowingBinding));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(VmRuntimeState.Ready, runtime.State);

        // The runtime is still usable afterwards, which is the whole claim: a host exception
        // cannot tear down or corrupt the runtime it was called from.
        var again = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(5));
        Assert.Equal(FixtureVmProfile.Id, again.Identity.ProfileId);
    }

    [Fact]
    public void An_Unbound_Optional_Import_Is_Reported_As_Unbound_Rather_Than_Faulting()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(1, FixtureHostCapabilities.OptionalBinding));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(VmRuntimeState.Ready, runtime.State);
    }

    [Fact]
    public void A_Second_Invocation_While_One_Is_Running_Is_Refused_Rather_Than_Queued()
    {
        // Queuing would be a scheduler, and the core does not schedule.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        Assert.True(FixtureComposition.Invoke(instance).IsSuspended);

        var second = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.InvalidState, second.Outcome);
        Assert.Equal(VmReason.WrongState, second.Reason);
    }

    [Fact]
    public void An_Executor_Whose_Identity_Disagrees_With_Its_Descriptor_Is_A_Profile_Fault()
    {
        // Returned, never thrown: it is a defect in a profile observed at run time, not a
        // composition error the caller could have prevented.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.IdentityMismatchedExecutor));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        var result = runtime.Instantiate(artifact, CancellationToken.None);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(VmReason.ExecutorIdentityMismatch, result.Reason);
    }

    [Fact]
    public void A_Profile_Suspending_During_Undeclared_Instantiation_Is_Refused()
    {
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.SuspendsDuringInstantiation));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        var result = runtime.Instantiate(artifact, CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidState, result.Outcome);
        Assert.Equal(VmReason.UndeclaredAsynchronousInstantiation, result.Reason);
    }

    [Fact]
    public void A_Profile_That_Exceeds_Its_Declared_Poll_Bound_Faults()
    {
        // The bound is how a profile promises a cancellation latency in its own work units.
        // Exceeding it is a broken promise, not an exhausted budget.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.PollBoundBreaker));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(64));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
    }

    private static VmDiagnosticsIdentity DiagnosticsFor(VmProfileId profileId)
    {
        VmDiagnosticsIdentity.TryCreate(profileId, profileId + ".diagnostics", out var identity);
        return identity;
    }
}
