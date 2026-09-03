using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>What one file did, in the terms a caller can act on.</summary>
/// <param name="Status">Which of this host's six answers came back.</param>
/// <param name="Value">The completion value, rendered, or empty where the program produced none.</param>
/// <param name="Detail">One line a reader can act on. Empty on a clean completion.</param>
/// <param name="Diagnostics">
/// Every refusal, in the order the front end produced them. Empty unless the source was refused.
/// </param>
internal sealed record RunResult(
    RunStatus Status,
    string Value,
    string Detail,
    IReadOnlyList<string> Diagnostics);

/// <summary>
/// The host: source in, a running program out, and one answer about what happened.
/// </summary>
/// <remarks>
/// <para>
/// This is the composition. One profile, arriving through its own static accessor; no name looked
/// up, no directory scanned, no assembly loaded. What makes it the <c>narrow-runtime-compiler</c>
/// image rather than its execution-only sibling is one reference - the lowering - and the closure
/// report is where that shows.
/// </para>
/// <para>
/// <b>Each file gets a runtime of its own.</b> An instruction allowance is spent over a runtime's
/// whole life rather than reset per invocation, so a host that composed once and ran a hundred
/// files would report every file after the first non-terminating one as exhausted. The conformance
/// harness learned that the expensive way and recorded it; this root does not have to learn it
/// again.
/// </para>
/// <para>
/// <b>The allowance is the profile's own declared default unless a caller states one.</b> A host
/// with an opinion about how long a program may run is a host imposing a policy the profile did
/// not declare, and roadmap section 3 records the declared defaults as the profile's. What the
/// default buys is that a program which never terminates ends in a bounded number of instructions
/// rather than never - and it ends the same way on a fast machine and a slow one, because fuel is
/// charged per instruction and not per second.
/// </para>
/// </remarks>
internal static class Host
{
    /// <summary>The entry point the lowering emits, which is what this host invokes.</summary>
    private static readonly byte[] EntryPoint = "main"u8.ToArray();

    /// <summary>The identity this host presents to the verifier.</summary>
    /// <remarks>
    /// A caller identity naming what this is rather than who ran it. It reaches a diagnostic and
    /// nothing else; putting a user or a machine name here would put an environment detail into a
    /// message a transcript retains.
    /// </remarks>
    private const string Caller = "broiler-js-cli://source";

    /// <summary>Composes the profile once to prove it composes, or says why it does not.</summary>
    internal static bool Composes(out string failure)
    {
        var created = VmRuntime.Create(Catalog(), Options(null));

        if (created.TryGetRuntime(out var runtime))
        {
            runtime.Dispose();
            failure = string.Empty;
            return true;
        }

        failure = $"{created.Outcome}/{created.Reason}";
        return false;
    }

    /// <summary>Compiles, verifies and runs one source text.</summary>
    /// <remarks>
    /// <b>The nesting bound is a caller's option because the default refuses real files.</b> Two
    /// files of the Octane benchmark nest deeper than the parse options' default of 64 and are
    /// refused with <c>NestingTooDeep</c> - which is not a manifest refusal and not a statement
    /// about the language: it is a ceiling the specification permits an implementation to have.
    /// The construct census reads at the largest bound the parser supports and therefore reports
    /// those two files as parsed, so the census and this host disagree about them and the census
    /// is the optimistic one. Exposing the bound is what makes the disagreement measurable rather
    /// than a discrepancy between two documents.
    /// </remarks>
    internal static RunResult Run(
        SourceFile file, bool module, bool checkOnly, ulong? fuel, int? maximumDepth)
    {
        if (file.Unreadable.Length != 0)
        {
            return new RunResult(RunStatus.Unreadable, string.Empty, file.Unreadable, []);
        }

        var goal = module ? SliceGoal.Module : SliceGoal.Script;

        var options = maximumDepth is { } depth
            ? new SliceParseOptions(goal, allowTopLevelAwait: false, depth)
            : module ? SliceParseOptions.Module : SliceParseOptions.Script;

        var compiled = SliceSourceCompiler.Compile(file.Text, options);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            var lines = compiled.Diagnostics.Select(one => one.ToString()).ToArray();

            return new RunResult(
                RunStatus.RefusedSource,
                string.Empty,
                lines.Length == 0
                    ? "the front end refused the source and named no diagnostic, which is a defect here"
                    : lines[0],
                lines);
        }

        var created = VmRuntime.Create(Catalog(), Options(fuel));

        if (!created.TryGetRuntime(out var runtime))
        {
            return new RunResult(
                RunStatus.HostDefect,
                string.Empty,
                $"the runtime refused creation: {created.Outcome}/{created.Reason}",
                []);
        }

        using (runtime)
        {
            return Run(runtime, compiled.Artifact, checkOnly);
        }
    }

    private static RunResult Run(VmRuntime runtime, byte[] artifact, bool checkOnly)
    {
        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            1,
            JavaScriptProfile.SliceManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(in descriptor, artifact, CancellationToken.None);

        if (!verified.TryGetArtifact(out var verifiedArtifact))
        {
            // EXHAUSTION AT VERIFICATION IS NOT AN ARTIFACT REFUSAL, and reading it as one was a
            // defect in this reporting that a sweep of a real suite found: sixteen files of
            // test262 are large enough that verifying them spends the allowance, and every one of
            // them was reported under the code that accuses this component. Verification is work,
            // it is charged, and running out of allowance while doing it is the same answer a
            // running program gets.
            if (verified.Outcome == VmOutcome.ResourceExhaustion)
            {
                return new RunResult(
                    RunStatus.Exhausted,
                    string.Empty,
                    $"verifying the artifact spent the allowance: {verified.Reason}",
                    []);
            }

            // AN ARTIFACT THIS HOST'S OWN LOWERING PRODUCED AND ITS OWN VERIFIER REFUSED IS A
            // DEFECT HERE, not a property of the input, and it reports under a code that says so.
            // The profile's diagnostic code is named rather than left to the core's reason, because
            // the reason says the artifact was inconsistent and the code says which rule found it.
            return new RunResult(
                RunStatus.RefusedArtifact,
                string.Empty,
                "the verifier refused an artifact this host produced: " +
                    $"{DiagnosticName(verified.Diagnostics.ProfileDiagnosticCode)} " +
                    $"({verified.Outcome}/{verified.Reason})",
                []);
        }

        if (checkOnly)
        {
            return new RunResult(RunStatus.Completed, string.Empty, string.Empty, []);
        }

        var instantiated = runtime.Instantiate(verifiedArtifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            if (instantiated.Outcome == VmOutcome.ResourceExhaustion)
            {
                return new RunResult(
                    RunStatus.Exhausted,
                    string.Empty,
                    $"instantiating the artifact spent the allowance: {instantiated.Reason}",
                    []);
            }

            return new RunResult(
                RunStatus.RefusedArtifact,
                string.Empty,
                "the artifact verified and would not instantiate: " +
                    $"{DiagnosticName(instantiated.Diagnostics.ProfileDiagnosticCode)} " +
                    $"({instantiated.Outcome}/{instantiated.Reason})",
                []);
        }

        var request = new VmInvocationRequest(new VmUtf8Text(EntryPoint));
        var result = instance.Invoke(in request, CancellationToken.None);

        if (result.Outcome == VmOutcome.ResourceExhaustion)
        {
            return new RunResult(
                RunStatus.Exhausted,
                string.Empty,
                $"the program did not settle within its allowance: {result.Reason}",
                []);
        }

        if (JavaScriptProfile.TryGetCompletion(in result, out var completion))
        {
            return new RunResult(
                RunStatus.Completed, completion.Value.ToDiagnosticString(), string.Empty, []);
        }

        if (JavaScriptProfile.TryGetFault(in result, out var fault))
        {
            return new RunResult(
                RunStatus.Faulted, string.Empty, $"{fault.Kind}: {fault.Message}", []);
        }

        return new RunResult(
            RunStatus.HostDefect,
            string.Empty,
            $"the invocation answered {result.Outcome}/{result.Reason} and carried no payload",
            []);
    }

    /// <summary>The catalog: one profile, arriving through its own static accessor.</summary>
    private static VmCatalog Catalog() => VmCatalog.CreateBuilder()
        .Add(JavaScriptProfile.Descriptor)
        .Build();

    /// <summary>The runtime this host creates for one file.</summary>
    private static VmRuntimeCreationOptions Options(ulong? fuel)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel when fuel is { } stated =>
                    VmCeilingSpec.Value(dimension, stated),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,

            // No artifact provider is registered, so every guest-initiated load is refused
            // deterministically. That is this host's content policy and it is the only one a
            // manifest with no `eval`, no `Function` and no `import()` could have.
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: ImmutableArray<VmCapabilityRegistration>.Empty);
    }

    /// <summary>Renders a number the one way this host renders numbers.</summary>
    internal static string Number(long value) => value.ToString(CultureInfo.InvariantCulture);

    /// <summary>The member name of a profile diagnostic code, or the number where it has none.</summary>
    /// <remarks>
    /// The name and not the number, because a number sends a reader to the registry to find out
    /// what was refused and a name tells them. A code outside the vocabulary renders as its number
    /// rather than throwing: a host that fell over on an unrecognised code would turn a diagnostic
    /// it could not read into a crash.
    /// </remarks>
    private static string DiagnosticName(int code)
    {
        var value = (JavaScriptDiagnosticCode)code;

        // NUMBER AND NAME, in the spelling the source-refusal path already uses. The number is
        // what the published registry is keyed on, so a reader can look the row up; the name is
        // what makes the line readable without doing that. Printing one of the two would have
        // made the two halves of this host's own reporting disagree about their own vocabulary.
        return Enum.IsDefined(value)
            ? Number(code) + ":" + value.ToString()
            : Number(code);
    }
}
