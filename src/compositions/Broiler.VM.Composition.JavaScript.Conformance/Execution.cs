using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>What one case actually did, beside what its test declared.</summary>
/// <param name="Status">The verdict this case is counted as.</param>
/// <param name="Completion">How the program under test settled.</param>
/// <param name="Answer">
/// What happened, written in the same spelling a test declares an expectation in.
/// </param>
/// <param name="Detail">A sentence a reader can act on. Empty on a pass.</param>
/// <remarks>
/// <b><paramref name="Answer"/> is the whole answer and not a verdict.</b> A harness that recorded
/// only pass or fail could not tell an engine that refused a program for the right reason from one
/// that refused it for the wrong one, and both would be scored the same. Writing the observation
/// in the declaration's own vocabulary is what makes the two comparable at all.
/// </remarks>
internal sealed record Observation(
    ConformanceStatus Status,
    CompletionKind Completion,
    string Answer,
    string Detail);

/// <summary>
/// The engine under test, composed once and asked one question per case.
/// </summary>
/// <remarks>
/// <para>
/// This is the composition: one profile, arriving through its own static accessor, with no name
/// looked up, no directory scanned and no assembly loaded. The harness's closure report means
/// something only because of that.
/// </para>
/// <para>
/// <b>The runtime states a fuel ceiling rather than adopting the profile's default.</b> A
/// conformance run has to be able to say that a program did not finish, and it has to say it the
/// same way on a fast machine and a slow one. Fuel is charged one unit per instruction, so a
/// ceiling on it bounds a runaway program in a number of instructions rather than in seconds; the
/// wall clock would make the same test pass or fail by how busy the machine was, which is the one
/// thing a floor must never be sensitive to.
/// </para>
/// <para>
/// <b>EVERY CASE GETS A RUNTIME OF ITS OWN, and the first run of this harness is why.</b> A fuel
/// allowance is spent over a runtime's whole life rather than reset per invocation, so one
/// composed runtime for a whole shard means the first program that does not terminate spends the
/// allowance and every case after it is reported as a timeout. That run reported thirty-four
/// timeouts and nothing else - and it would have been indistinguishable from an engine that had
/// stopped working, which is exactly the reading a conformance total must never be able to
/// produce. Isolation is not an optimisation to be traded away here: a case's verdict has to be a
/// property of that case.
/// </para>
/// </remarks>
internal sealed class Execution : IDisposable
{
    /// <summary>
    /// The instruction allowance one case gets.
    /// </summary>
    /// <remarks>
    /// Generous against what a test in this manifest can legitimately need - the longest retained
    /// program counts to ten - and small enough that a program which never terminates is answered
    /// in well under a second. It is stated here rather than taken from the profile's own default
    /// of fifty million because the default is sized for a host with no opinion, and a harness
    /// that waits fifty million instructions to learn that a loop does not end is a harness nobody
    /// runs.
    /// </remarks>
    // Broiler-Falsified-If: a program that does not terminate is reported as anything but a
    // TimedOut result whose completion kind is NeverSettled.
    internal const ulong FuelCeiling = 2_000_000;

    private Execution()
    {
    }

    /// <summary>Composes the engine once to prove it composes, or says why it does not.</summary>
    /// <remarks>
    /// The runtime this builds is discarded. Its purpose is to fail early and loudly on a host that
    /// cannot compose the profile at all, rather than to be reused: reuse is what the remark on
    /// this type refuses.
    /// </remarks>
    internal static Execution? Create(out string failure)
    {
        var created = VmRuntime.Create(Catalog(), Options());

        if (created.TryGetRuntime(out var runtime))
        {
            runtime.Dispose();
            failure = string.Empty;
            return new Execution();
        }

        failure = $"{created.Outcome}/{created.Reason}";
        return null;
    }

    /// <summary>The catalog: one profile, arriving through its own static accessor.</summary>
    private static VmCatalog Catalog() => VmCatalog.CreateBuilder()
        .Add(JavaScriptProfile.Descriptor)
        .Build();

    /// <summary>Runs one test and compares what happened with what it declared.</summary>
    internal Observation Run(ConformanceTest test)
    {
        var created = VmRuntime.Create(Catalog(), Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return new Observation(
                ConformanceStatus.Failed,
                CompletionKind.NeverSettled,
                "no-runtime",
                $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            return Run(runtime, test);
        }
    }

    private static Observation Run(VmRuntime runtime, ConformanceTest test)
    {
        if (test.RequiredHost.Length != 0)
        {
            return new Observation(
                ConformanceStatus.Skipped,
                CompletionKind.NeverSettled,
                "skipped",
                $"needs the `{test.RequiredHost}` host, which the sibling root that owns it builds");
        }

        var bytes = test.Bytes;

        if (test.Mode != HostMode.Raw)
        {
            var options = test.Mode == HostMode.Module
                ? SliceParseOptions.Module
                : SliceParseOptions.Script;

            var compiled = SliceSourceCompiler.Compile(test.Source, options);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                var first = compiled.Diagnostics.Count == 0
                    ? "NoDiagnostic"
                    : compiled.Diagnostics[0].Code.ToString();

                return Compare(
                    test,
                    new ConformanceExpectation(ExpectationKind.RefusedBySource, first),
                    Unsettled,
                    compiled.Diagnostics.Count == 0
                        ? "the front end refused and named no diagnostic"
                        : compiled.Diagnostics[0].ToString());
            }

            bytes = compiled.Artifact;
        }

        if (bytes is null)
        {
            return new Observation(
                ConformanceStatus.Failed,
                CompletionKind.NeverSettled,
                "no-bytes",
                "the test carried neither source nor artifact bytes");
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            1,
            JavaScriptProfile.SliceManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("js-conformance://suite"));

        var verified = runtime.Verify(in descriptor, bytes, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return Exhausted(verified.Outcome, test)
                ?? Compare(
                    test,
                    new ConformanceExpectation(
                        ExpectationKind.RefusedByVerifier,
                        DiagnosticName(verified.Diagnostics.ProfileDiagnosticCode)),
                    Unsettled,
                    $"{verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return Exhausted(instantiated.Outcome, test)
                ?? Compare(
                    test,
                    new ConformanceExpectation(
                        ExpectationKind.RefusedByVerifier,
                        DiagnosticName(instantiated.Diagnostics.ProfileDiagnosticCode)),
                    Unsettled,
                    $"{instantiated.Outcome}/{instantiated.Reason}");
        }

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        var exhausted = Exhausted(result.Outcome, test);

        if (exhausted is not null)
        {
            return exhausted;
        }

        if (JavaScriptProfile.TryGetCompletion(in result, out var completion))
        {
            return Compare(
                test,
                new ConformanceExpectation(
                    ExpectationKind.Completion,
                    completion.Value.ToDiagnosticString()),
                Settled,
                string.Empty);
        }

        if (JavaScriptProfile.TryGetFault(in result, out var fault))
        {
            return Compare(
                test,
                new ConformanceExpectation(ExpectationKind.Fault, fault.Kind.ToString()),
                [CompletionProtocol.FailurePrefix + fault.Kind],
                fault.Message);
        }

        return new Observation(
            ConformanceStatus.Failed,
            CompletionKind.NeverSettled,
            "no-answer",
            $"the invocation answered {result.Outcome}/{result.Reason} and carried no payload");
    }

    /// <summary>
    /// The one outcome that is a timeout rather than an answer: the allowance ran out.
    /// </summary>
    /// <remarks>
    /// <b>It is never compared against the declaration.</b> A test cannot declare that it exhausts
    /// a budget, because the budget is the harness's and not the test's - so a case that ran out of
    /// fuel is reported as what it is, whatever the test hoped for. Folding it into the comparison
    /// would let a test declaring an exhaustion pass by never terminating.
    /// </remarks>
    private static Observation? Exhausted(VmOutcome outcome, ConformanceTest test) =>
        outcome == VmOutcome.ResourceExhaustion
            ? new Observation(
                ConformanceStatus.TimedOut,
                CompletionKind.NeverSettled,
                "exhausted",
                $"`{test.Path}` spent the {FuelCeiling.ToString(CultureInfo.InvariantCulture)}-unit " +
                "allowance without settling")
            : null;

    /// <summary>Scores one answer against one declaration.</summary>
    /// <remarks>
    /// The completion kind is read from the markers the run emitted rather than passed in, so the
    /// protocol classifier is on the product path and not only in the harness's own tests. A
    /// classifier nothing calls is a classifier that can be wrong for a year.
    /// </remarks>
    private static Observation Compare(
        ConformanceTest test,
        ConformanceExpectation answer,
        IReadOnlyList<string> markers,
        string detail)
    {
        var (completion, why) = CompletionProtocol.Classify(markers);
        var full = detail.Length == 0 ? why : detail;

        if (answer == test.Expectation)
        {
            return new Observation(ConformanceStatus.Passed, completion, answer.ToString(), string.Empty);
        }

        return new Observation(
            ConformanceStatus.Failed,
            completion,
            answer.ToString(),
            $"declared `{test.Expectation}` and answered `{answer}`" +
                (full.Length == 0 ? string.Empty : ": " + full));
    }

    /// <summary>The markers a refused or exhausted run emits: none, because it never settled.</summary>
    private static readonly string[] Unsettled = [];

    /// <summary>The markers a run that produced a completion value emits.</summary>
    private static readonly string[] Settled = [CompletionProtocol.Completed];

    /// <summary>The member name of a core-result diagnostic code, or the number where it has none.</summary>
    /// <remarks>
    /// The name and not the number, for the reason roadmap section 14 gives about a negative test's
    /// error type: a refusal is matched on what it is. A code outside the vocabulary renders as its
    /// number rather than throwing, because a harness that fell over on an unrecognised code would
    /// destroy the run that was about to report it.
    /// </remarks>
    internal static string DiagnosticName(int code)
    {
        var value = (JavaScriptDiagnosticCode)code;

        return Enum.IsDefined(value)
            ? value.ToString()
            : code.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// The runtime options: profile defaults everywhere but the one dimension this harness owns.
    /// </summary>
    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, FuelCeiling),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: ImmutableArray<VmCapabilityRegistration>.Empty);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Nothing to release: this type holds no runtime between cases, which is the whole of the
    /// isolation property. It stays disposable so the call sites read the same as their siblings'
    /// and so a future resource acquired here has an obvious home.
    /// </remarks>
    public void Dispose() => GC.SuppressFinalize(this);
}
