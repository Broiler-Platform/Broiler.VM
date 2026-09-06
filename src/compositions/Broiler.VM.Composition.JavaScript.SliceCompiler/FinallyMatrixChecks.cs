// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Which exits run a <c>finally</c>, which one replaces what it was leaving with, and what a
/// failure raised on the host's side of the boundary does to both.
/// </summary>
/// <remarks>
/// <para>
/// <b>JS-5's gate asks for a retained matrix and names its rows.</b> A nested-handler and
/// <c>finally</c> matrix, in both directions across the boundary, covering <c>return</c>,
/// <c>break</c>, <c>continue</c>, a language throw and a host exception, with return and throw
/// replacement by <c>finally</c> covered, the host exception surfacing as a host failure and a
/// language throw as a typed payload behind a profile fault. Nothing retained showed any row of it.
/// </para>
/// <para>
/// <b>Every row below is a program rather than an assertion about the interpreter</b>, which is
/// what makes the matrix answerable by a reader: each case prints the order things happened in and
/// the check compares that order, so a <c>finally</c> that ran at the wrong moment is a different
/// string rather than a passing test.
/// </para>
/// </remarks>
internal static class FinallyMatrixChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://finally";

    /// <summary>Runs every row of the matrix.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run() =>
    [
        Language("a return runs the finally it leaves through", ReturnRow, "enter,finally,1"),
        Language("a break runs the finally it leaves through", BreakRow, "enter,finally,after"),
        Language("a continue runs the finally it leaves through", ContinueRow, "0,f0,1,f1,done"),
        Language("a throw runs the finally it leaves through", ThrowRow, "enter,finally,caught"),
        Language("a finally that returns replaces the return", ReplacesReturnRow, "outer"),
        Language("a finally that returns replaces the throw", ReplacesThrowRow, "replaced"),
        Language("nested handlers unwind innermost first", NestedRow, "inner,outer,caught"),
        AnUncaughtThrowIsATypedPayload(),
        AHostFailureCrossesTheProfilesFrames(),
    ];

    private const string ReturnRow =
        "var log = [];" +
        "function f() { try { log.push('enter'); return 1; } finally { log.push('finally'); } }" +
        "log.push(String(f())); log.join(',');";

    private const string BreakRow =
        "var log = [];" +
        "for (var i = 0; i < 3; i++) { try { log.push('enter'); break; } finally { log.push('finally'); } }" +
        "log.push('after'); log.join(',');";

    private const string ContinueRow =
        "var log = [];" +
        "for (var i = 0; i < 2; i++) { try { log.push(String(i)); continue; } finally { log.push('f' + i); } }" +
        "log.push('done'); log.join(',');";

    private const string ThrowRow =
        "var log = [];" +
        "try { try { log.push('enter'); throw new Error('x'); } finally { log.push('finally'); } }" +
        "catch (e) { log.push('caught'); } log.join(',');";

    private const string ReplacesReturnRow =
        "function f() { try { return 'inner'; } finally { return 'outer'; } } f();";

    private const string ReplacesThrowRow =
        "function f() { try { throw new Error('x'); } finally { return 'replaced'; } } f();";

    private const string NestedRow =
        "var log = [];" +
        "try {" +
        "  try {" +
        "    try { throw new Error('x'); } finally { log.push('inner'); }" +
        "  } finally { log.push('outer'); }" +
        "} catch (e) { log.push('caught'); }" +
        "log.join(',');";

    /// <summary>One language row: run the program and compare what it recorded.</summary>
    private static (string, bool, string) Language(string name, string source, string expected)
    {
        using var runtime = Runtime([]);

        if (runtime is null)
        {
            return (name, false, "the runtime refused creation");
        }

        if (!TryRun(runtime, source, out var answer, out var outcome, out var reason))
        {
            return (name, false, $"the program answered {outcome}/{reason}");
        }

        return (
            name,
            string.Equals(answer, expected, System.StringComparison.Ordinal),
            $"expected `{expected}` and the program recorded `{answer}`");
    }

    /// <summary>
    /// A throw nothing catches reaches the host as a typed payload behind a fault, not as a core
    /// reason.
    /// </summary>
    private static (string, bool, string) AnUncaughtThrowIsATypedPayload()
    {
        const string Name = "an uncaught throw is a typed payload behind a fault";

        using var runtime = Runtime([]);

        if (runtime is null)
        {
            return (Name, false, "the runtime refused creation");
        }

        var raised = Run(
            runtime,
            "var log = [];" +
            "try { throw new TypeError('from the guest'); } finally { log.push('finally'); }",
            out var result);

        _ = raised;

        if (!JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
        {
            return (Name, false, $"the invocation answered {result.Outcome}/{result.Reason} with no payload");
        }

        return (
            Name,
            result.Outcome is VmOutcome.ProfileFault &&
            string.Equals(uncaught.ErrorName, "TypeError", System.StringComparison.Ordinal),
            $"the invocation answered {result.Outcome}/{result.Reason} carrying a " +
            $"{uncaught.ErrorName}: {uncaught.Message}");
    }

    /// <summary>
    /// A capability that throws while the guest is inside a <c>finally</c>-bearing frame.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the row that crosses the boundary in the other direction</b>, and what it records
    /// is what the contract says rather than what a reader might assume. The write capability
    /// declares <c>TerminateOperation</c>, which the core defines as <i>the operation ends with the
    /// host failure, whatever the profile does next</i> — so the answer is a host failure naming
    /// the capability, and the guest's own bookkeeping about whether its <c>finally</c> ran is
    /// reported beside it rather than asserted, because the contract does not promise it either
    /// way.
    /// </para>
    /// </remarks>
    private static (string, bool, string) AHostFailureCrossesTheProfilesFrames()
    {
        const string Name = "a host failure crosses the profile's frames and names the capability";

        var reached = 0;
        using var runtime = Runtime(
            [
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.WriteCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        System.Threading.Interlocked.Increment(ref reached);
                        throw new System.InvalidOperationException("the host's own defect");
                    }),
            ]);

        if (runtime is null)
        {
            return (Name, false, "the runtime refused creation");
        }

        Run(
            runtime,
            "globalThis.ran = 'no';" +
            "try { print('reaching the host'); } finally { globalThis.ran = 'yes'; }" +
            "'completed';",
            out var result);

        return (
            Name,
            result.Outcome is VmOutcome.HostFailure &&
            result.Diagnostics.CapabilityId.Equals(JavaScriptProfile.WriteCapability.CapabilityId) &&
            reached == 1,
            $"the handler was reached {reached} time(s) and the invocation answered " +
            $"{result.Outcome}/{result.Reason} naming `{result.Diagnostics.CapabilityId}`");
    }

    /// <summary>Compiles and runs one program, reporting the completion value.</summary>
    private static bool TryRun(
        VmRuntime runtime, string source, out string answer, out VmOutcome outcome, out VmReason reason)
    {
        answer = string.Empty;
        var ran = Run(runtime, source, out var result);
        outcome = result.Outcome;
        reason = result.Reason;

        if (!ran || !JavaScriptProfile.TryGetWideCompletion(in result, out var completion))
        {
            return false;
        }

        answer = completion.Value;
        return true;
    }

    /// <summary>Compiles, verifies, instantiates and invokes, returning the whole result.</summary>
    private static bool Run(VmRuntime runtime, string source, out VmInvocationResult result)
    {
        result = default;
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
            return false;
        }

        var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return false;
        }

        using (instance)
        {
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

            result = instance.Invoke(in request, System.Threading.CancellationToken.None);
            return result.Outcome is VmOutcome.Normal;
        }
    }

    /// <summary>A runtime over this profile with these registrations.</summary>
    private static VmRuntime? Runtime(ImmutableArray<VmCapabilityRegistration> capabilities)
    {
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
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
                capabilities: capabilities));

        return created.TryGetRuntime(out var runtime) ? runtime : null;
    }
}
