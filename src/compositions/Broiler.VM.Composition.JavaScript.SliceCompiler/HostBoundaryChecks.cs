// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// When a host capability is refused, and what a capability that fails is reported as.
/// </summary>
/// <remarks>
/// <para>
/// <b>JS-5's gate asks for two things about this boundary and each has its own case below.</b> The
/// first is that a mismatch is refused <i>when the runtime is created and not at first call</i> —
/// a signature that only disagrees when somebody happens to invoke it is a defect with a schedule.
/// The second is that the translation precedence is proved <i>per capability</i>: a cancellation
/// carrying the operation's own token reported as cancellation, an exhausted meter at the moment of
/// the catch as resource exhaustion, and anything else as a host failure naming the capability.
/// </para>
/// <para>
/// <b>Two clauses of that gate are unreachable for this profile, and they are recorded as
/// unreachable rather than left to look unmet.</b> The gate asks that a failed <i>required</i>
/// import leave no partially bound runtime — and this profile declares three imports, every one of
/// them <b>optional</b>: <c>broiler.javascript.write</c>, <c>broiler.javascript.source-provider</c>
/// and <c>broiler.javascript.resolve</c>. There is no required import to fail, so no case can be
/// built, and building one would mean changing the descriptor to make a gate constructible, which
/// is the wrong direction. The same declaration decides the version clause: matching is by
/// identity <i>and</i> version together, so a registration at another version is not a mismatch the
/// core refuses but a capability it never finds, and an optional import that finds nothing is bound
/// to nothing. That is the unbound branch, which has a case of its own below.
/// </para>
/// <para>
/// <b>The exhaustion case is the profile's own charge failing rather than the core's.</b> This
/// profile charges <c>HostCalls</c> itself before it reaches the boundary, so a runtime granted no
/// host calls refuses inside the profile; the core's precedence then reports the exhaustion it
/// observed ahead of anything the step said. The clause asks for the answer and this is the path
/// the answer comes by, which is worth stating because a reader tracing it in the core's binding
/// code would find a second charge that never runs.
/// </para>
/// </remarks>
internal static class HostBoundaryChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://host-boundary";

    /// <summary>A program that reaches the write capability once and then answers.</summary>
    private const string PrintsOnce = "print('reached'); 'done';";

    /// <summary>Runs every host-boundary check.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run() =>
    [
        ASignatureMismatchIsRefusedAtCreation(),
        AKindMismatchIsRefusedAtCreation(),
        AVersionMismatchLeavesAnOptionalImportUnbound(),
        AnUnboundOptionalImportIsExercised(),
        ACancellationCarryingTheOperationsTokenIsACancellation(),
        AnExhaustedMeterAtTheCatchIsAResourceExhaustion(),
        AnyOtherFailureIsAHostFailureNamingTheCapability(),
    ];

    /// <summary>
    /// A registration whose signature differs is refused when the runtime is created.
    /// </summary>
    private static (string, bool, string) ASignatureMismatchIsRefusedAtCreation()
    {
        const string Name = "a signature mismatch is refused when the runtime is created";

        var declared = JavaScriptProfile.WriteCapability;
        var wrong = new VmHostCapabilityDescriptor(
            declared.CapabilityId,
            declared.Version,
            VmCapabilitySignatureId.FromCanonicalDescription("(bytes)->bytes"),
            declared.Kind,
            declared.Reentrancy,
            declared.ThreadAffinity,
            declared.ExceptionTranslation);

        var created = Create([VmCapabilityRegistration.Value(wrong, Accepts)]);

        if (created.TryGetRuntime(out var runtime))
        {
            runtime.Dispose();
            return (Name, false, "the runtime was created over a signature the profile does not import");
        }

        return (
            Name,
            created.Reason is VmReason.CapabilitySignatureMismatch,
            $"creation answered {created.Outcome}/{created.Reason}; the handler was never invoked, " +
            "because there was no runtime to invoke it from");
    }

    /// <summary>A registration whose kind differs is refused when the runtime is created.</summary>
    /// <remarks>
    /// The kind is registered as a value capability under a descriptor declaring the provider kind,
    /// rather than through the provider factory, because that factory refuses the pairing itself —
    /// which is a different guard, in the core's own surface, and is not the one this case is
    /// about.
    /// </remarks>
    private static (string, bool, string) AKindMismatchIsRefusedAtCreation()
    {
        const string Name = "a kind mismatch is refused when the runtime is created";

        var declared = JavaScriptProfile.WriteCapability;
        var wrong = new VmHostCapabilityDescriptor(
            declared.CapabilityId,
            declared.Version,
            declared.SignatureId,
            VmCapabilityKind.ArtifactProvider,
            declared.Reentrancy,
            declared.ThreadAffinity,
            declared.ExceptionTranslation);

        var created = Create([VmCapabilityRegistration.Value(wrong, Accepts)]);

        if (created.TryGetRuntime(out var runtime))
        {
            runtime.Dispose();
            return (Name, false, "the runtime was created over a kind the profile does not import");
        }

        return (
            Name,
            created.Reason is VmReason.CapabilitySignatureMismatch,
            $"creation answered {created.Outcome}/{created.Reason}");
    }

    /// <summary>
    /// A registration at another version is not a mismatch: the optional import finds nothing.
    /// </summary>
    private static (string, bool, string) AVersionMismatchLeavesAnOptionalImportUnbound()
    {
        const string Name = "a version mismatch leaves an optional import unbound";

        var declared = JavaScriptProfile.WriteCapability;
        var other = new VmHostCapabilityDescriptor(
            declared.CapabilityId,
            declared.Version + 1,
            declared.SignatureId,
            declared.Kind,
            declared.Reentrancy,
            declared.ThreadAffinity,
            declared.ExceptionTranslation);

        var reached = 0;
        var created = Create(
            [
                VmCapabilityRegistration.Value(
                    other,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        System.Threading.Interlocked.Increment(ref reached);
                        return VmHostCallOutcome.Completed;
                    }),
            ]);

        if (!created.TryGetRuntime(out var runtime))
        {
            return (Name, false, $"creation answered {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            if (!TryRun(runtime, PrintsOnce, out var answer, out var outcome, out var reason))
            {
                return (Name, false, $"the program answered {outcome}/{reason}");
            }

            return (
                Name,
                reached == 0 && string.Equals(answer, "done", System.StringComparison.Ordinal),
                $"the program completed with {answer} and the handler registered at version " +
                $"{other.Version} was reached {reached} times; the import declares version " +
                $"{declared.Version} and matching is by identity and version together, so this is " +
                "the unbound branch rather than a refusal");
        }
    }

    /// <summary>A composition registering nothing runs the same program, reaching nowhere.</summary>
    private static (string, bool, string) AnUnboundOptionalImportIsExercised()
    {
        const string Name = "the unbound branch of an optional import is exercised";

        var created = Create([]);

        if (!created.TryGetRuntime(out var runtime))
        {
            return (Name, false, $"creation answered {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            if (!TryRun(runtime, PrintsOnce, out var answer, out var outcome, out var reason))
            {
                return (Name, false, $"the program answered {outcome}/{reason}");
            }

            return (
                Name,
                string.Equals(answer, "done", System.StringComparison.Ordinal),
                $"with no capability registered the program printed and completed with {answer}; " +
                "what it printed reached nowhere, which is the composition's answer and not a fault");
        }
    }

    /// <summary>
    /// A capability throwing the operation's own cancellation is reported as a cancellation.
    /// </summary>
    /// <remarks>
    /// The token is the one handed to the invocation, and it is cancelled before the throw, because
    /// the core's test is both halves: the exception carries the operation's token AND the token is
    /// cancelled. An exception carrying a foreign token is the last case below rather than this
    /// one.
    /// </remarks>
    private static (string, bool, string) ACancellationCarryingTheOperationsTokenIsACancellation()
    {
        const string Name = "a capability cancelling with the operation's own token is a cancellation";

        using var source = new System.Threading.CancellationTokenSource();
        var token = source.Token;

        var created = Create(
            [
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.WriteCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        source.Cancel();
                        throw new System.OperationCanceledException(token);
                    }),
            ]);

        if (!created.TryGetRuntime(out var runtime))
        {
            return (Name, false, $"creation answered {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            TryRun(runtime, PrintsOnce, out _, out var outcome, out var reason, token);

            return (
                Name,
                outcome is VmOutcome.Cancellation,
                $"the invocation answered {outcome}/{reason}");
        }
    }

    /// <summary>
    /// A host call with no host-call allowance left is a resource exhaustion naming the dimension.
    /// </summary>
    private static (string, bool, string) AnExhaustedMeterAtTheCatchIsAResourceExhaustion()
    {
        const string Name = "an exhausted meter at the host call is a resource exhaustion";

        var reached = 0;
        var created = Create(
            [
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.WriteCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        System.Threading.Interlocked.Increment(ref reached);
                        return VmHostCallOutcome.Completed;
                    }),
            ],
            hostCalls: 0);

        if (!created.TryGetRuntime(out var runtime))
        {
            return (Name, false, $"creation answered {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            TryRun(runtime, PrintsOnce, out _, out var outcome, out var reason);

            return (
                Name,
                outcome is VmOutcome.ResourceExhaustion && reached == 0,
                $"the invocation answered {outcome}/{reason} and the handler was reached {reached} " +
                "times; the charge that failed is this profile's own, taken before the boundary");
        }
    }

    /// <summary>Any other failure is a host failure naming the capability.</summary>
    private static (string, bool, string) AnyOtherFailureIsAHostFailureNamingTheCapability()
    {
        const string Name = "any other capability failure is a host failure naming the capability";

        var created = Create(
            [
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.WriteCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                        throw new System.InvalidOperationException("the host's own defect")),
            ]);

        if (!created.TryGetRuntime(out var runtime))
        {
            return (Name, false, $"creation answered {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            TryRun(
                runtime, PrintsOnce, out _, out var outcome, out var reason, default, out var capability);

            return (
                Name,
                outcome is VmOutcome.HostFailure &&
                capability.Equals(JavaScriptProfile.WriteCapability.CapabilityId),
                $"the invocation answered {outcome}/{reason} naming `{capability}`");
        }
    }

    /// <summary>A handler that does nothing and says so.</summary>
    private static VmHostCallOutcome Accepts(VmBytes argument, out VmOpaqueRef result)
    {
        result = default;
        return VmHostCallOutcome.Completed;
    }

    /// <summary>Compiles, verifies, instantiates and invokes, reporting what the core answered.</summary>
    private static bool TryRun(
        VmRuntime runtime,
        string source,
        out string answer,
        out VmOutcome outcome,
        out VmReason reason,
        System.Threading.CancellationToken cancellationToken = default) =>
        TryRun(runtime, source, out answer, out outcome, out reason, cancellationToken, out _);

    /// <summary>The same, additionally reporting the capability a host failure named.</summary>
    private static bool TryRun(
        VmRuntime runtime,
        string source,
        out string answer,
        out VmOutcome outcome,
        out VmReason reason,
        System.Threading.CancellationToken cancellationToken,
        out VmCapabilityId capability)
    {
        answer = string.Empty;
        outcome = VmOutcome.None;
        reason = VmReason.None;
        capability = default;

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script)]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return false;
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(
            in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            outcome = verified.Outcome;
            reason = verified.Reason;
            return false;
        }

        var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            outcome = instantiated.Outcome;
            reason = instantiated.Reason;
            return false;
        }

        using (instance)
        {
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

            var invoked = instance.Invoke(in request, cancellationToken);
            outcome = invoked.Outcome;
            reason = invoked.Reason;
            capability = invoked.Diagnostics.CapabilityId;

            if (!JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
            {
                return false;
            }

            answer = completion.Value;
            return true;
        }
    }

    /// <summary>Creates a runtime over this profile with these registrations.</summary>
    private static VmRuntimeCreationResult Create(
        ImmutableArray<VmCapabilityRegistration> capabilities, ulong? hostCalls = null)
    {
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.HostCalls when hostCalls is { } granted =>
                    VmCeilingSpec.Value(dimension, granted),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        return VmRuntime.Create(
            catalog,
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: capabilities));
    }
}
