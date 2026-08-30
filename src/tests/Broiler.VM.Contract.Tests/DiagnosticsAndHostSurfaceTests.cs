using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// What a diagnostics record says, and what a host can reach through the capability boundary.
/// </summary>
/// <remarks>
/// The gate asks for two things in one breath: diagnostics that identify profile, version and
/// artifact locations, and diagnostics that leak no host secret. They pull in opposite directions,
/// so both halves are asserted here - the identifying half against real failures at every stage,
/// and the leaking half against a host that tries to put a secret into one.
/// </remarks>
public sealed class DiagnosticsAndHostSurfaceTests
{
    private const string Secret = "postgres://user:hunter2@db.internal:5432/ledger";

    /// <summary>
    /// A failed verification identifies the profile, the format version, the manifest, the
    /// verifier's semantic version and where in the bytes it stopped.
    /// </summary>
    [Fact]
    public void A_Failed_Verification_Identifies_Profile_Version_And_Position()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        // Truncated after the magic: a failure the profile's own verifier reports, with a position.
        var descriptor = FixtureComposition.Descriptor();
        var result = runtime.Verify(in descriptor, [0x42, 0x52, 0x4F, 0x49], CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidArtifact, result.Outcome);

        var diagnostics = result.Diagnostics;

        Assert.Equal(FixtureVmProfile.Id, diagnostics.ProfileId);
        Assert.Equal(FixtureFormat.FormatVersion, diagnostics.ProfileFormatVersion);
        Assert.Equal(FixtureVmProfile.Manifest, diagnostics.FeatureManifestId);
        Assert.True(diagnostics.VerifierSemanticVersion > 0, "no verifier semantic version was recorded");
        Assert.Equal(VmStage.Verification, diagnostics.Stage);
        Assert.False(diagnostics.RuntimeId.IsEmpty, "no runtime identity was recorded");

        // The position is the profile's own, and it is what a caller needs to say WHERE the
        // artifact was wrong rather than only that it was.
        Assert.True(
            diagnostics.SourcePosition.ByteOffset > 0 || diagnostics.ProfileDiagnosticCode != 0,
            "the failure carried neither a position nor a profile diagnostic code");

        // And the caller's own identity, carried verbatim because the caller supplied it.
        Assert.Equal("test://artifact", diagnostics.CallerIdentity.ToString());
    }

    /// <summary>
    /// A host capability that throws a secret produces a host failure that names the capability and
    /// carries none of the secret.
    /// </summary>
    /// <remarks>
    /// The record has nowhere to put a message, which rule V11 asserts structurally. This is the
    /// behavioural half of the same claim: the exception really is thrown, the operation really does
    /// fail, and what reaches the caller is an identity and a category.
    /// </remarks>
    [Fact]
    public void A_Throwing_Capability_Leaks_Nothing_Of_What_It_Threw()
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.Descriptor),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Throwing)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(21, FixtureHostCapabilities.DoubleBinding));

        var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.HostFailure, result.Outcome);

        var diagnostics = result.Diagnostics;

        // It names WHICH capability failed, which is what a host needs to find its own defect.
        Assert.Equal(FixtureHostCapabilities.DoubleId, diagnostics.CapabilityId);
        Assert.Equal(1, diagnostics.CapabilityVersion);

        // And nothing anywhere in the record is the secret. Every text-bearing member is checked by
        // name rather than by a search over some rendering, because a rendering that omitted a
        // field would make this pass by not looking.
        foreach (var text in new[]
        {
            diagnostics.CallerIdentity.ToString(),
            diagnostics.ProfileId.ToString(),
            diagnostics.FeatureManifestId.ToString(),
            diagnostics.CapabilityId.ToString(),
        })
        {
            Assert.DoesNotContain("hunter2", text, StringComparison.Ordinal);
            Assert.DoesNotContain("db.internal", text, StringComparison.Ordinal);
        }

        instance.Dispose();
        artifact.Dispose();

        static VmHostCallOutcome Throwing(ReadOnlySpan<long> arguments, out long result) =>
            throw new InvalidOperationException(Secret);
    }

    /// <summary>
    /// A host that supplies no caller identity has none recorded.
    /// </summary>
    /// <remarks>
    /// The one text-bearing member whose content is the host's own choice is the one the host can
    /// decline to fill. That is the whole of what a host has to do to keep its own strings out of a
    /// caller's log.
    /// </remarks>
    [Fact]
    public void A_Host_That_Supplies_No_Caller_Identity_Has_None_Recorded()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var anonymous = new VmArtifactDescriptor(
            FixtureVmProfile.Id,
            FixtureFormat.FormatVersion,
            FixtureVmProfile.Manifest,
            default,
            VmCallerIdentity.None);

        var result = runtime.Verify(in anonymous, [0x00], CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidArtifact, result.Outcome);
        Assert.True(result.Diagnostics.CallerIdentity.IsEmpty, "an identity nobody supplied was recorded");
        Assert.False(result.Diagnostics.CallerIdentity.IsCallerSupplied);
    }

    /// <summary>
    /// An invalid-state failure says which object, in which state, and which call was attempted.
    /// </summary>
    [Fact]
    public void An_Invalid_State_Failure_Names_The_Object_The_State_And_The_Call()
    {
        var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));

        runtime.Dispose();

        var result = runtime.Instantiate(artifact, CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidState, result.Outcome);
        Assert.Equal(VmReason.ObjectDisposed, result.Reason);
        Assert.Equal(VmObjectKind.Runtime, result.Diagnostics.ObjectKind);
        Assert.Equal(VmAttemptedCall.Instantiate, result.Diagnostics.AttemptedCall);
        Assert.Equal(VmInitiator.Caller, result.Diagnostics.Initiator);

        artifact.Dispose();
    }

    /// <summary>
    /// A resource failure names the dimension and the scope that ran out.
    /// </summary>
    [Fact]
    public void A_Resource_Failure_Names_The_Dimension_And_The_Scope()
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(ceilings: FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 64)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(10_000));
        var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
        Assert.True(
            result.Diagnostics.ExhaustedScope is VmBudgetScope.Invocation or VmBudgetScope.Instance
                or VmBudgetScope.Runtime,
            $"the exhaustion named scope {result.Diagnostics.ExhaustedScope}");

        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// The capability table answers whether a slot is bound and nothing else about it.
    /// </summary>
    /// <remarks>
    /// The behavioural half of rule V12. A profile holds an <see cref="IVmHostCapabilityInvoker"/>
    /// and can ask for a binding count, ask whether index k is bound, and invoke k. It cannot
    /// enumerate the registered set, resolve a capability by name, or learn what is on the other
    /// side - so an unregistered optional import is indistinguishable from one the host chose not
    /// to offer, which is exactly what keeps a capability table from becoming a directory.
    /// </remarks>
    [Fact]
    public void A_Profile_Can_Ask_Only_Whether_A_Slot_Is_Bound()
    {
        var members = typeof(IVmHostCapabilityInvoker)
            .GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Select(static member => member.Name)
            .Where(static name => !name.StartsWith("get_", StringComparison.Ordinal))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["BindingCount", "Invoke", "InvokeBytes", "IsBound"], members);
    }

    /// <summary>
    /// The opaque-reference identity reasons are unreachable at core contract version 1, and this
    /// asserts the unreachability rather than claiming the check works.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>ForeignOpaqueRef</c> and <c>StaleOpaqueRef</c> are declared reasons that nothing can
    /// produce, and the cause is structural rather than an omission. The core mints no opaque
    /// reference and consumes none: a host handler produces one, the core hands it to the profile
    /// that asked, and no member of contract version 1 ever takes one back. There is no moment at
    /// which the core could compare a reference against anything.
    /// </para>
    /// <para>
    /// So the honest test is the one that fails the day that stops being true: no public member
    /// accepts a <c>VmOpaqueRef</c>. When one does, this test fails, and whoever added it has to
    /// implement the check the two reasons were reserved for.
    /// </para>
    /// </remarks>
    [Fact]
    public void No_Public_Member_Consumes_An_Opaque_Reference()
    {
        var members = typeof(VmRuntime).Assembly.GetExportedTypes()
            .Concat(typeof(VmCoreContract).Assembly.GetExportedTypes())
            .Where(static type => type != typeof(VmOpaqueRef))
            .SelectMany(static type => type.GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.DeclaredOnly))
            .SelectMany(static method => method.GetParameters().Select(parameter => (method, parameter)))
            .Where(static entry =>
                entry.parameter.ParameterType == typeof(VmOpaqueRef) ||
                entry.parameter.ParameterType == typeof(VmOpaqueRef).MakeByRefType())
            .ToArray();

        // Nothing takes one as an input. An input is the only shape that would let a profile - or a
        // host - present a reference back and expect the core to say something about it.
        var consuming = members
            .Where(static entry => !entry.parameter.IsOut)
            .Select(static entry => $"{entry.method.DeclaringType?.Name}.{entry.method.Name}")
            .ToArray();

        Assert.Empty(consuming);

        // What does exist is the one direction that carries a reference OUT of a host handler and
        // through the core to the profile that asked. The core is a courier there, not a checker:
        // it has nothing to compare a reference against, because it minted none and holds no
        // registry of them.
        var producing = members
            .Where(static entry => entry.parameter.IsOut)
            .Select(static entry => $"{entry.method.DeclaringType?.Name}.{entry.method.Name}")
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        // The delegate's compiler-generated asynchronous pair is listed beside its Invoke, because
        // a delegate type declares all three. None of them is a core member and none is reachable
        // from a profile; they are named rather than filtered so that the list stays a complete
        // answer to "where can a reference cross this boundary" rather than a curated one.
        string[] expected =
        [
            "IVmHostCapabilityInvoker.InvokeBytes",
            "VmHostBytesCapabilityHandler.BeginInvoke",
            "VmHostBytesCapabilityHandler.EndInvoke",
            "VmHostBytesCapabilityHandler.Invoke",
        ];

        Assert.Equal(expected.OrderBy(static name => name, StringComparer.Ordinal), producing);
    }
}
