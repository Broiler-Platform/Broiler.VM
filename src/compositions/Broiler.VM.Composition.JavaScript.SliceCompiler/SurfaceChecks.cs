// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// What a composition's answer to an optional surface actually is, in each configuration it can be
/// in.
/// </summary>
/// <remarks>
/// <para>
/// <b>The point of these checks is a distinction that reads as one behaviour and is two.</b>
/// Roadmap section 6 says a composition declining a feature manifest refuses the artifact at
/// verification, with an invalid-artifact reason the guest never sees, while a composition that
/// admits the manifest and registers no provider refuses at run time with an error the guest may
/// catch. A reader who only ever saw one configuration would have no way to tell that the two are
/// different events with different catchabilities — and bundle JS-4-001's exclusions recorded that
/// difference as UNMET for <c>eval</c>, because the realm simply had no such binding and answered a
/// <c>ReferenceError</c> for both.
/// </para>
/// <para>
/// <b>This root is the legal home for them.</b> Rule A11 forbids a test project from referencing a
/// profile assembly, so behavioural evidence about this profile lives in a composition root's own
/// check list or nowhere — and it has to be a root that carries the LOWERING, because each of these
/// checks is about a program written in JavaScript rather than about hand-assembled bytes. The
/// execution-only image deliberately has no compiler in its closure and could not host them.
/// </para>
/// </remarks>
internal static class SurfaceChecks
{
    /// <summary>Runs every configuration check.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run() =>
    [
        ADecliningCompositionRefusesAtVerification(),
        AnAdmittingCompositionWithNoProviderRefusesAtRunTime(),
        ATypeofDeclaresNothingAndIsAnsweredEitherWay(),
        ADeclaredSurfaceReachesTheRealm(),
    ];

    /// <summary>
    /// A composition admitting no optional surface refuses a program that constructs a typed array,
    /// before it runs.
    /// </summary>
    private static (string, bool, string) ADecliningCompositionRefusesAtVerification()
    {
        const string Name = "a declining composition refuses at verification";
        var artifact = Compile("var a = new Uint8Array(1); a[0];");

        if (artifact is null)
        {
            return (Name, false, "the source did not compile, so the check judged nothing");
        }

        var outcome = Verify(artifact, JavaScriptProfile.DescriptorAdmitting());

        return (
            Name,
            outcome.Outcome == VmOutcome.InvalidArtifact &&
                outcome.Reason == VmReason.UnsupportedFeatureManifest &&
                outcome.Diagnostics.ProfileDiagnosticCode == 1608,
            $"{outcome.Outcome}/{outcome.Reason}/{outcome.Diagnostics.ProfileDiagnosticCode}");
    }

    /// <summary>
    /// A composition admitting the dynamic surface but registering no artifact provider verifies
    /// the same program and refuses it at run time instead.
    /// </summary>
    private static (string, bool, string) AnAdmittingCompositionWithNoProviderRefusesAtRunTime()
    {
        const string Name = "an admitting composition with no provider refuses at run time";
        var artifact = Compile("var e = eval; try { e('1'); 'no refusal'; } catch (x) { x.name; }");

        if (artifact is null)
        {
            return (Name, false, "the source did not compile, so the check judged nothing");
        }

        var answer = RunProgram(artifact, JavaScriptProfile.Descriptor, out var detail);

        // THE GUEST CAUGHT IT, which is the whole difference from the row above. A refusal the
        // guest can catch is a run-time error; a refusal it cannot is an artifact that never
        // became an instance.
        return (Name, answer && string.Equals(detail, "EvalError", System.StringComparison.Ordinal), detail);
    }

    /// <summary>
    /// A <c>typeof</c> of a surface name declares nothing, so the artifact verifies under a
    /// composition that declined the surface — and answers <c>"undefined"</c>.
    /// </summary>
    private static (string, bool, string) ATypeofDeclaresNothingAndIsAnsweredEitherWay()
    {
        const string Name = "a typeof declares no surface and is answered rather than refused";
        var artifact = Compile("typeof Uint8Array;");

        if (artifact is null)
        {
            return (Name, false, "the source did not compile, so the check judged nothing");
        }

        var answer = RunProgram(artifact, JavaScriptProfile.DescriptorAdmitting(), out var detail);

        return (
            Name,
            answer && string.Equals(detail, "undefined", System.StringComparison.Ordinal),
            detail);
    }

    /// <summary>
    /// A composition admitting the binary surface runs the same program the declining one refused.
    /// </summary>
    /// <remarks>
    /// The non-vacuity clause for the first check. A rule that only ever saw a refusal would pass
    /// just as well over a build that refused everything.
    /// </remarks>
    private static (string, bool, string) ADeclaredSurfaceReachesTheRealm()
    {
        const string Name = "an admitting composition runs what the declining one refused";
        var artifact = Compile("var a = new Uint8Array(1); a[0] = 300; String(a[0]);");

        if (artifact is null)
        {
            return (Name, false, "the source did not compile, so the check judged nothing");
        }

        var answer = RunProgram(
            artifact,
            JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.BinaryManifest),
            out var detail);

        return (Name, answer && string.Equals(detail, "44", System.StringComparison.Ordinal), detail);
    }

    /// <summary>Compiles one script, or answers nothing.</summary>
    private static byte[]? Compile(string source)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script)]);

        return compiled.Succeeded ? compiled.Artifact : null;
    }

    /// <summary>Verifies one artifact against a composition's descriptor.</summary>
    private static VmVerificationResult Verify(byte[] artifact, VmProfileDescriptor profile)
    {
        using var runtime = Runtime(profile);

        if (runtime is null)
        {
            return default;
        }

        var descriptor = Descriptor();
        return runtime.Verify(in descriptor, artifact, System.Threading.CancellationToken.None);
    }

    /// <summary>Runs one artifact and reports its completion value, or why it did not run.</summary>
    private static bool RunProgram(byte[] artifact, VmProfileDescriptor profile, out string detail)
    {
        using var runtime = Runtime(profile);

        if (runtime is null)
        {
            detail = "the runtime refused creation";
            return false;
        }

        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            detail = $"verification: {verified.Outcome}/{verified.Reason}";
            return false;
        }

        var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            detail = $"instantiation: {instantiated.Outcome}/{instantiated.Reason}";
            return false;
        }

        using (instance)
        {
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

            var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);

            if (invoked.Outcome != VmOutcome.Normal)
            {
                detail = $"invocation: {invoked.Outcome}/{invoked.Reason}";
                return false;
            }

            if (!JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
            {
                detail = "the completion value was not this profile's";
                return false;
            }

            detail = completion.Value;
            return true;
        }
    }

    /// <summary>The descriptor a caller presents with these bytes.</summary>
    private static VmArtifactDescriptor Descriptor() =>
        new(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

    /// <summary>A runtime over one profile descriptor, with no capability registered at all.</summary>
    /// <remarks>
    /// Registering nothing is what makes these checks about the manifest boundary rather than about
    /// a provider: there is no artifact provider here in any configuration, so the run-time refusal
    /// is the deterministic one a composition expresses a content policy with.
    /// </remarks>
    private static VmRuntime? Runtime(VmProfileDescriptor profile)
    {
        var catalog = VmCatalog.CreateBuilder().Add(profile).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            catalog,
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: ImmutableArray<VmCapabilityRegistration>.Empty));

        return created.TryGetRuntime(out var runtime) ? runtime : null;
    }

    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://surfaces";
}
