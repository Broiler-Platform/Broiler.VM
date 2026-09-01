using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>
/// The execution-only composition: a format, a verifier, an interpreter and no compiler.
/// </summary>
/// <remarks>
/// <para>
/// It cannot turn source into an artifact however it is invoked, because the lowering is not in
/// its reference set. Everything it runs is precompiled and read as bytes from the retained
/// corpus - including the fixtures its own checks use, which is why it takes a corpus directory
/// rather than assembling artifacts of its own. That is the composition label as a property of the
/// graph rather than as a promise, and the closure report over a publish of this project is where
/// a reader checks it.
/// </para>
/// <para>
/// <b>One profile and no neighbour.</b> The two-profile checks JS-0 carried - a hostile
/// neighbour's maxima, its adopted defaults, a foreign payload - live in the slice-compiler root
/// instead, because composing a second profile here would put it in this closure and this closure
/// is the single-profile claim.
/// </para>
/// <para>
/// The checks below are this milestone's behavioural evidence. They live in a composition root
/// rather than in a test project because rule A11 forbids a test project to reference a profile
/// assembly - a rule this component's own graph depends on - so a reader checks behaviour in this
/// program's output rather than in the suite.
/// </para>
/// </remarks>
internal static class Program
{
    private static int Main(string[] args)
    {
        var verbose = args.Contains("--verbose", StringComparer.Ordinal);

        try
        {
            if (args.Contains("--closure", StringComparer.Ordinal))
            {
                return ReportClosure();
            }

            var replay = Argument(args, "--replay");

            if (replay is not null)
            {
                return Fuzzing.Replay(replay);
            }

            var corpus = Argument(args, "--corpus");

            if (corpus is null)
            {
                Console.WriteLine("broiler-js-execution-only: no --corpus <directory> was given");
                return 2;
            }

            if (args.Contains("--fuzz", StringComparer.Ordinal))
            {
                return Fuzzing.Run(
                    corpus,
                    ulong.Parse(Argument(args, "--seed") ?? "1", System.Globalization.CultureInfo.InvariantCulture),
                    int.Parse(Argument(args, "--iterations") ?? "20000", System.Globalization.CultureInfo.InvariantCulture),
                    verbose);
            }

            var addition = File.ReadAllBytes(Path.Combine(corpus, "addition.bjsb"));

            var checks = new List<(string Name, bool Passed, string Detail)>
            {
                UnsupportedProfileExaminesNoByte(),
                FourExecutionStepKinds(addition),
                SuspensionIsUnreachableHere(),
                OperandStackIsSizedFromVerification(addition),
                TheCallerBufferMayChangeAfterwards(addition),
            };

            checks.AddRange(ReplayChecks(corpus, verbose));
            checks.AddRange(HostLifetimeChecks.Run(corpus));
            checks.AddRange(OrderingChecks.Run(
                corpus, CorpusReplay.ReadManifest(Path.Combine(corpus, "corpus.manifest"))));

            var failed = 0;

            foreach (var (name, passed, detail) in checks)
            {
                if (!passed)
                {
                    failed++;
                }

                if (verbose || !passed)
                {
                    Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
                }
            }

            Console.WriteLine(
                failed == 0
                    ? $"broiler-js-execution-only: {checks.Count} checks passed, core contract version {VmCoreContract.Version}"
                    : $"broiler-js-execution-only: {failed} of {checks.Count} checks FAILED");

            return failed == 0 ? 0 : 1;
        }
        catch (Exception failure)
        {
            Console.WriteLine(
                $"broiler-js-execution-only: unhandled {failure.GetType().Name}: {failure.Message}");

            return 2;
        }
    }

    private static string? Argument(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], name, StringComparison.Ordinal))
            {
                return args[index + 1];
            }
        }

        return null;
    }

    // ---- the corpus ------------------------------------------------------------------------

    /// <summary>
    /// Replays the retained corpus twice and holds every entry to the triple its manifest records.
    /// </summary>
    /// <remarks>
    /// Twice, because a replay that left residue would pass the first time, and the roadmap asks
    /// for the property rather than for one run. The two passes are compared row by row, so a
    /// difference is reported as the entry that moved rather than as a changed total.
    /// </remarks>
    private static IEnumerable<(string, bool, string)> ReplayChecks(string directory, bool verbose)
    {
        var manifestPath = Path.Combine(directory, "corpus.manifest");

        if (!File.Exists(manifestPath))
        {
            yield return ("corpus-replay", false, $"no corpus.manifest in {directory}");
            yield break;
        }

        var entries = CorpusReplay.ReadManifest(manifestPath);
        var first = CorpusReplay.Replay(directory, entries);
        var second = CorpusReplay.Replay(directory, entries);

        var disagreements = new List<string>();

        for (var index = 0; index < entries.Length; index++)
        {
            if (!CorpusReplay.Agrees(entries[index], first[index]))
            {
                // Every compared field is printed, the position and the exhausted pair among
                // them. An earlier revision printed four of the five, so a position regression
                // reported an expected and an observed answer that read identically - a true
                // sentence nobody can act on, which is the one thing a control log may not
                // produce. The dimension and the scope arrived with the same defect and were
                // caught the same way: the control that maps one exhaustion status onto its
                // neighbour moves nothing else, so a message without the pair printed both sides
                // as ResourceExhaustion/CeilingReached/0/-/-.
                disagreements.Add(
                    $"{entries[index].Name}: expected " +
                    $"{entries[index].Outcome}/{entries[index].Reason}/{entries[index].DiagnosticCode}/" +
                    $"{entries[index].Completion}/{entries[index].Position}/" +
                    $"{entries[index].Dimension}/{entries[index].Scope}, observed " +
                    $"{first[index].Outcome}/{first[index].Reason}/{first[index].DiagnosticCode}/" +
                    $"{first[index].Completion}/{first[index].Position}/" +
                    $"{first[index].Dimension}/{first[index].Scope} (hash {first[index].HashStatus})");
            }
            else if (verbose)
            {
                Console.WriteLine(
                    $"     entry {entries[index].Name}: {first[index].Outcome}/{first[index].Reason}/" +
                    $"{first[index].DiagnosticCode}/{first[index].Completion}/{first[index].Position}/" +
                    $"{first[index].Dimension}/{first[index].Scope}");
            }
        }

        yield return disagreements.Count == 0
            ? ("corpus-replay", true, $"{entries.Length} entries replayed to their recorded answers")
            : ("corpus-replay", false, string.Join("; ", disagreements));

        var residue = new List<string>();

        for (var index = 0; index < entries.Length; index++)
        {
            if (first[index] != second[index])
            {
                residue.Add(entries[index].Name);
            }
        }

        yield return residue.Count == 0
            ? ("corpus-replays-twice-with-no-residue", true, "both passes agreed row for row")
            : ("corpus-replays-twice-with-no-residue", false, string.Join(", ", residue));

        // A corpus in which nothing passes would not notice a verifier that rejects everything, so
        // the control entries are counted rather than assumed.
        var controls = entries.Count(entry => string.Equals(entry.Outcome, "Normal", StringComparison.Ordinal));

        yield return controls > 0
            ? ("the-corpus-contains-passing-controls", true, $"{controls} entries verify successfully")
            : ("the-corpus-contains-passing-controls", false, "every entry is a rejection");

        // The five verifier outcomes, each produced by a named entry of this corpus.
        var outcomes = entries
            .Select(entry => entry.Outcome)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        string[] required = ["Cancellation", "InvalidArtifact", "Normal", "ResourceExhaustion", "UnsupportedProfile"];
        var missing = required.Where(name => !outcomes.Contains(name, StringComparer.Ordinal)).ToArray();

        yield return missing.Length == 0
            ? ("five-verifier-outcomes", true, string.Join(", ", outcomes))
            : ("five-verifier-outcomes", false, "not produced: " + string.Join(", ", missing));

        // Every invalid-artifact entry carries a diagnostic code, and every exhaustion entry names
        // a dimension. The split between the two categories is what a corpus entry pins, and a
        // rejection with no code would be a rejection nobody could look up.
        var uncoded = entries
            .Where(entry => string.Equals(entry.Outcome, "InvalidArtifact", StringComparison.Ordinal))
            .Where(entry => entry.DiagnosticCode == 0)
            .Select(entry => entry.Name)
            .ToArray();

        yield return uncoded.Length == 0
            ? ("every-invalid-artifact-carries-a-diagnostic-code", true,
                $"{entries.Count(entry => entry.Outcome == "InvalidArtifact")} entries, each with a code")
            : ("every-invalid-artifact-carries-a-diagnostic-code", false, string.Join(", ", uncoded));

        foreach (var check in ExhaustionChecks(entries))
        {
            yield return check;
        }
    }

    /// <summary>
    /// The exhaustion half of the corpus: one entry per dimension a verification can exhaust, each
    /// naming the dimension and the scope its answer carried.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why this is a check at all.</b> An exhaustion answer carries no diagnostic code, so the
    /// registry's both-directions binding - which is what holds every other refusal in this profile
    /// to a case that provokes it - reaches none of them. Without these two checks the corpus could
    /// lose an exhaustion entry, or grow a host mode nothing exercises, and every other check here
    /// would stay green.
    /// </para>
    /// <para>
    /// <b>What it is not.</b> The dimensions compared here are the ones this composition declares a
    /// tight mode for, so a dimension the verifier can answer and no mode reaches would pass. That
    /// is the architecture rule's half of the binding, read off the verifier's own source, and it
    /// is deliberately not repeated here: this root cannot see the verifier's source, and a second
    /// hand-written list of seven would agree with whichever of the two it was copied from.
    /// </para>
    /// </remarks>
    private static IEnumerable<(string, bool, string)> ExhaustionChecks(ReplayEntry[] entries)
    {
        // Every exhaustion row names a pair, and every row that is not an exhaustion names
        // neither. The second half matters as much as the first: a dimension written beside a
        // normal completion is a claim about a budget nothing ran out of.
        var misnamed = entries
            .Where(entry => string.Equals(entry.Outcome, "ResourceExhaustion", StringComparison.Ordinal)
                ? entry.Dimension == "-" || entry.Scope == "-"
                : entry.Dimension != "-" || entry.Scope != "-")
            .Select(entry => $"{entry.Name}: {entry.Outcome} with {entry.Dimension}/{entry.Scope}")
            .ToArray();

        var exhaustions = entries
            .Where(entry => string.Equals(entry.Outcome, "ResourceExhaustion", StringComparison.Ordinal))
            .ToArray();

        yield return misnamed.Length == 0
            ? ("an-exhaustion-names-a-dimension-and-a-scope-and-nothing-else-does", true,
                $"{exhaustions.Length} exhaustion entries name a pair, {entries.Length - exhaustions.Length} name none")
            : ("an-exhaustion-names-a-dimension-and-a-scope-and-nothing-else-does", false,
                string.Join("; ", misnamed));

        // And one entry per dimension this host can tighten, in both directions: a mode with no
        // entry is a dimension nothing pins, and an entry naming a dimension no mode tightens is a
        // row whose answer this composition cannot have provoked.
        var pinned = exhaustions
            .Select(entry => entry.Dimension)
            .ToHashSet(StringComparer.Ordinal);

        var declared = Hosts.TightModes
            .Select(mode => mode.Dimension.ToString())
            .ToHashSet(StringComparer.Ordinal);

        var unpinned = declared.Except(pinned, StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        var unprovokable = pinned.Except(declared, StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        var duplicated = exhaustions
            .GroupBy(entry => entry.Dimension, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => $"{group.Key} is pinned by {group.Count()} entries")
            .ToArray();

        var complaints = unpinned.Select(dimension => $"{dimension} is tightened by a mode and no entry pins it")
            .Concat(unprovokable.Select(dimension => $"{dimension} is pinned by an entry and no mode tightens it"))
            .Concat(duplicated)
            .ToArray();

        yield return complaints.Length == 0
            ? ("every-dimension-this-host-can-tighten-is-pinned-by-one-entry", true,
                $"{pinned.Count} dimensions, one entry each: {string.Join(", ", pinned.Order(StringComparer.Ordinal))}")
            : ("every-dimension-this-host-can-tighten-is-pinned-by-one-entry", false,
                string.Join("; ", complaints));
    }

    // ---- the contract loop -----------------------------------------------------------------

    /// <summary>
    /// An artifact naming a profile this verifier does not host is refused without a payload byte
    /// being examined.
    /// </summary>
    /// <remarks>
    /// Proved by handing the verifier an EMPTY payload alongside a foreign descriptor. A verifier
    /// that read anything before checking the identity would fail on the empty span with a framing
    /// answer, so the outcome is evidence about the ordering and not only about the answer.
    /// </remarks>
    private static (string, bool, string) UnsupportedProfileExaminesNoByte()
    {
        var descriptor = new VmArtifactDescriptor(
            VmProfileId.Parse("com.example.absent"),
            1,
            VmFeatureManifestId.Parse("com.example.absent.base"),
            default,
            VmCallerIdentity.FromCanonicalIdentity("js-execution-only://foreign"));

        var outcome = JavaScriptProfile.Descriptor.Verifier.Verify(
            in descriptor, ReadOnlySpan<byte>.Empty, new EmptyVerificationContext(), CancellationToken.None);

        return outcome.Category == VmOutcome.UnsupportedProfile
            ? ("unsupported-profile-examines-no-payload-byte", true,
                $"{outcome.Category}/{outcome.Reason} over an empty payload")
            : ("unsupported-profile-examines-no-payload-byte", false,
                $"answered {outcome.Category}/{outcome.Reason}");
    }

    /// <summary>
    /// Four of the five execution-step kinds, each produced by a named path.
    /// </summary>
    /// <remarks>
    /// <c>Instantiated</c> from an instantiation, <c>Completed</c> from an entry point that
    /// returns, <c>Faulted</c> from an entry-point name nothing is bound to - a ReferenceError in
    /// the language - and <c>ContractViolation</c> from a resume this surface can never have
    /// produced a continuation for. The fifth is <see cref="SuspensionIsUnreachableHere"/>.
    /// </remarks>
    private static (string, bool, string) FourExecutionStepKinds(byte[] addition)
    {
        using var runtime = Hosts.Runtime("default", out var failure);

        if (runtime is null)
        {
            return ("execution-step-kinds", false, failure);
        }

        var descriptor = Hosts.Descriptor("default");
        var verified = runtime.Verify(in descriptor, addition, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("execution-step-kinds", false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return ("execution-step-kinds", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
        }

        var completedRequest = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var completed = instance.Invoke(in completedRequest, CancellationToken.None);

        if (!JavaScriptProfile.TryGetCompletion(in completed, out var value) ||
            value.Value.ToDiagnosticString() != "42")
        {
            return ("execution-step-kinds", false, $"invocation {completed.Outcome}/{completed.Reason}");
        }

        var faultedRequest = new VmInvocationRequest(new VmUtf8Text("nowhere"u8));
        var faulted = instance.Invoke(in faultedRequest, CancellationToken.None);

        if (!JavaScriptProfile.TryGetFault(in faulted, out var fault) ||
            fault.Kind != JavaScriptErrorKind.ReferenceError)
        {
            return ("execution-step-kinds", false, $"unknown entry point gave {faulted.Outcome}/{faulted.Reason}");
        }

        // The contract violation, produced directly rather than through the core: this surface
        // never suspends, so a resume can only be a continuation it was never given.
        var executor = JavaScriptProfile.Descriptor.ExecutorFactory(new EmptyExecutionEnvironment());
        var resumed = executor.Resume(instance is null ? null! : new NoInstance(), new NoContinuation(), CancellationToken.None);

        return resumed.Kind == VmExecutionStepKind.ContractViolation
            ? ("execution-step-kinds", true,
                "Instantiated, Completed, Faulted as a ReferenceError, and ContractViolation from a resume")
            : ("execution-step-kinds", false, $"a resume answered {resumed.Kind}");
    }

    /// <summary>
    /// The fifth step kind is unreachable from this surface, and this milestone declares it rather
    /// than minting an out-of-manifest opcode to reach it.
    /// </summary>
    /// <remarks>
    /// The slice has no generator, no async function and no module, so nothing can park. Adding an
    /// opcode that suspended in order to produce the answer would be widening the manifest to
    /// satisfy a gate, which is the shape the roadmap forbids by name. JS-7 produces it.
    /// </remarks>
    private static (string, bool, string) SuspensionIsUnreachableHere()
    {
        var declaresSuspension =
            JavaScriptProfile.Descriptor.ExternalSuspension == VmDeclaration.Declared ||
            JavaScriptProfile.Descriptor.AsynchronousInstantiation == VmDeclaration.Declared;

        return declaresSuspension
            ? ("suspended-is-declared-produced-at-js-7", false,
                "the descriptor declares a pause this surface cannot make")
            : ("suspended-is-declared-produced-at-js-7", true,
                "neither external suspension nor asynchronous instantiation is declared; JS-7 produces the fifth kind");
    }

    /// <summary>
    /// The executor's operand stack is sized from a bound computed at verification and stored on
    /// the verified state, never from a number the payload chose.
    /// </summary>
    /// <remarks>
    /// The artifact declares a maximum of sixteen and uses two. The verified state reports two,
    /// which is the number the executor allocates from; the declared sixteen is checked against it
    /// and does not become it.
    /// </remarks>
    private static (string, bool, string) OperandStackIsSizedFromVerification(byte[] addition)
    {
        using var runtime = Hosts.Runtime("default", out var failure);

        if (runtime is null)
        {
            return ("operand-stack-sized-from-verification", false, failure);
        }

        var descriptor = Hosts.Descriptor("default");
        var verified = runtime.Verify(in descriptor, addition, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact) ||
            !artifact.TryGetState(out var state) ||
            state is not JavaScriptProgram program)
        {
            return ("operand-stack-sized-from-verification", false,
                $"verification {verified.Outcome}/{verified.Reason}");
        }

        return program.MaximumOperandStack == 2
            ? ("operand-stack-sized-from-verification", true,
                $"the artifact declared 16 and verification computed {program.MaximumOperandStack}")
            : ("operand-stack-sized-from-verification", false,
                $"verification computed {program.MaximumOperandStack}, expected 2");
    }

    /// <summary>
    /// Overwriting the caller's buffer after verification returns changes neither the verified
    /// state nor the execution result.
    /// </summary>
    private static (string, bool, string) TheCallerBufferMayChangeAfterwards(byte[] addition)
    {
        using var runtime = Hosts.Runtime("default", out var failure);

        if (runtime is null)
        {
            return ("the-caller-buffer-may-change-afterwards", false, failure);
        }

        var buffer = (byte[])addition.Clone();
        var descriptor = Hosts.Descriptor("default");
        var verified = runtime.Verify(in descriptor, buffer, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("the-caller-buffer-may-change-afterwards", false,
                $"verification {verified.Outcome}/{verified.Reason}");
        }

        Array.Fill(buffer, (byte)0xEE);

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return ("the-caller-buffer-may-change-afterwards", false,
                $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
        }

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        return JavaScriptProfile.TryGetCompletion(in result, out var value) &&
            value.Value.ToDiagnosticString() == "42"
            ? ("the-caller-buffer-may-change-afterwards", true,
                "the buffer was overwritten after verification and the answer did not move")
            : ("the-caller-buffer-may-change-afterwards", false, $"invocation {result.Outcome}/{result.Reason}");
    }

    // ---- the closure report -------------------------------------------------------------------

    /// <summary>Prints what this composition declares itself to be.</summary>
    /// <remarks>
    /// The authoritative closure is the published output, which the evidence bundle lists from the
    /// file system. This is the composition's own statement of what it declared, so a report that
    /// disagrees with the publish has two independent halves to compare rather than one.
    /// </remarks>
    private static int ReportClosure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.JavaScript.ExecutionOnly");
        Console.WriteLine("label execution-only");
        Console.WriteLine("carries-lowering no");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                JavaScriptProfile.Id,
                JavaScriptProfile.Descriptor.PackageIdentity.PackageId,
                JavaScriptProfile.Descriptor.DescriptorRevision,
                JavaScriptProfile.Descriptor.HostCapabilityDescriptors.Length));
        Console.WriteLine(string.Join(' ', "manifest", JavaScriptProfile.SliceManifest));
        Console.WriteLine(
            string.Join(
                ' ',
                "format-versions",
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Min,
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Max));

        return 0;
    }
}

/// <summary>
/// A verification context carrying no capability and generous ceilings, for the one check that
/// calls the verifier directly.
/// </summary>
/// <remarks>
/// The unsupported-profile answer must not depend on a runtime, and constructing one to prove it
/// would make the check answer a different question. The meter refuses nothing and counts nothing:
/// what the check reads is whether the verifier looked at the payload, and a meter that interfered
/// would be a second reason for the answer.
/// </remarks>
internal sealed class EmptyVerificationContext : IVmVerificationContext
{
    /// <inheritdoc/>
    public VmEffectiveCeilings Ceilings { get; } =
        new(VmLimitVector.Unconstrained, VmLimitVector.Unconstrained);

    /// <inheritdoc/>
    public IVmMeter Meter { get; } = new PermissiveMeter();

    /// <inheritdoc/>
    public ImmutableArray<VmHostCapabilityDescriptor> RegisteredCapabilities =>
        ImmutableArray<VmHostCapabilityDescriptor>.Empty;

    /// <inheritdoc/>
    public bool TryGetCapabilityDescriptor(
        VmCapabilityId capabilityId,
        int version,
        out VmHostCapabilityDescriptor descriptor)
    {
        descriptor = default;
        return false;
    }

    private sealed class PermissiveMeter : IVmMeter
    {
        public bool TryCharge(VmBudgetDimension dimension, ulong amount) => true;

        public bool Poll() => true;

        public void ReportRetained(VmBudgetDimension dimension, ulong amount)
        {
        }

        public void ReportReleased(VmBudgetDimension dimension, ulong amount)
        {
        }
    }
}

/// <summary>An execution environment for the one check that calls the executor directly.</summary>
internal sealed class EmptyExecutionEnvironment : IVmExecutionEnvironment
{
    /// <inheritdoc/>
    public VmProfileId ProfileId => JavaScriptProfile.Id;

    /// <inheritdoc/>
    public IVmMeter Meter { get; } = new NeverRefusingMeter();

    /// <inheritdoc/>
    public IVmHostCapabilityInvoker Capabilities { get; } = new NoCapabilities();

    /// <inheritdoc/>
    public bool TryGetArtifactLoadMediator(out IVmArtifactLoadMediator mediator)
    {
        mediator = null!;
        return false;
    }

    private sealed class NeverRefusingMeter : IVmMeter
    {
        public bool TryCharge(VmBudgetDimension dimension, ulong amount) => true;

        public bool Poll() => true;

        public void ReportRetained(VmBudgetDimension dimension, ulong amount)
        {
        }

        public void ReportReleased(VmBudgetDimension dimension, ulong amount)
        {
        }
    }

    private sealed class NoCapabilities : IVmHostCapabilityInvoker
    {
        public int BindingCount => 0;

        public bool IsBound(int bindingIndex) => false;

        public VmHostCallOutcome Invoke(int bindingIndex, ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;
            return VmHostCallOutcome.Refused;
        }

        public VmHostCallOutcome InvokeBytes(int bindingIndex, VmBytes argument, out VmOpaqueRef result)
        {
            result = default;
            return VmHostCallOutcome.Refused;
        }
    }
}

/// <summary>An instance state this profile did not make, for the resume check.</summary>
internal sealed class NoInstance : IVmInstanceState
{
}

/// <summary>A continuation this profile never produced, for the resume check.</summary>
internal sealed class NoContinuation : IVmProfileContinuation
{
}
