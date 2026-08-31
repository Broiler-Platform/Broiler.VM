using Broiler.VM;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>
/// The mutation engine: a total function of its seed and the corpus it starts from.
/// </summary>
/// <remarks>
/// <para>
/// <b>Deterministic on purpose, and it is the property a finding is worth anything without.</b>
/// There is no wall-clock budget, no thread count and no unseeded randomness anywhere, so a
/// session is reproduced by naming its seed and its seed corpus. A fuzzer whose sessions cannot be
/// replayed produces anecdotes.
/// </para>
/// <para>
/// This is the profile family's own code and not the core's. The core has a mutator of the same
/// shape in its fixtures assembly, and rule A12 forbids a composition root to reference that
/// assembly - which is the right answer rather than an inconvenience: a composition root that
/// pulled in a test-only assembly would be publishing a closure nobody would believe.
/// </para>
/// </remarks>
internal sealed class ArtifactMutator
{
    /// <summary>Byte values that sit on a boundary something is likely to compare against.</summary>
    private static readonly byte[] Interesting =
        [0x00, 0x01, 0x7F, 0x80, 0x81, 0xFE, 0xFF, 0x10, 0x20, 0x50, 0x60, 0x70];

    private ulong state;

    internal ArtifactMutator(ulong seed) => state = seed == 0 ? 0x9E3779B97F4A7C15 : seed;

    /// <summary>One mutant, drawn from the corpus and perturbed.</summary>
    internal byte[] Next(IReadOnlyList<byte[]> corpus)
    {
        var input = corpus[(int)(NextValue() % (ulong)corpus.Count)];

        return (NextValue() % 10) switch
        {
            0 => FlipBit(input),
            1 => SetByte(input),
            2 => SetInterestingByte(input),
            3 => Truncate(input),
            4 => Extend(input),
            5 => DuplicateChunk(input),
            6 => DeleteChunk(input),
            7 => Splice(input, corpus),
            8 => PerturbOperand(input),
            _ => RewriteLength(input),
        };
    }

    /// <summary>xorshift64*, written out so the sequence is this file's and not a library's.</summary>
    private ulong NextValue()
    {
        state ^= state >> 12;
        state ^= state << 25;
        state ^= state >> 27;
        return state * 0x2545F4914F6CDD1D;
    }

    private int Index(int length) => length == 0 ? 0 : (int)(NextValue() % (ulong)length);

    private byte[] FlipBit(byte[] input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        var copy = (byte[])input.Clone();
        var at = Index(copy.Length);
        copy[at] ^= (byte)(1 << (int)(NextValue() % 8));
        return copy;
    }

    private byte[] SetByte(byte[] input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        var copy = (byte[])input.Clone();
        copy[Index(copy.Length)] = (byte)(NextValue() % 256);
        return copy;
    }

    private byte[] SetInterestingByte(byte[] input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        var copy = (byte[])input.Clone();
        copy[Index(copy.Length)] = Interesting[(int)(NextValue() % (ulong)Interesting.Length)];
        return copy;
    }

    private byte[] Truncate(byte[] input) =>
        input.Length <= 1 ? input : input[..(1 + Index(input.Length - 1))];

    private byte[] Extend(byte[] input)
    {
        var added = 1 + (int)(NextValue() % 16);
        var copy = new byte[input.Length + added];
        input.CopyTo(copy, 0);

        for (var index = input.Length; index < copy.Length; index++)
        {
            copy[index] = (byte)(NextValue() % 256);
        }

        return copy;
    }

    private byte[] DuplicateChunk(byte[] input)
    {
        if (input.Length < 2)
        {
            return input;
        }

        var start = Index(input.Length);
        var length = 1 + Index(System.Math.Min(16, input.Length - start));
        var copy = new byte[input.Length + length];

        input.AsSpan(0, start + length).CopyTo(copy);
        input.AsSpan(start, length).CopyTo(copy.AsSpan(start + length));
        input.AsSpan(start + length).CopyTo(copy.AsSpan(start + (length * 2)));

        return copy;
    }

    private byte[] DeleteChunk(byte[] input)
    {
        if (input.Length < 2)
        {
            return input;
        }

        var start = Index(input.Length - 1);
        var length = 1 + Index(System.Math.Min(16, input.Length - start - 1));
        var copy = new byte[input.Length - length];

        input.AsSpan(0, start).CopyTo(copy);
        input.AsSpan(start + length).CopyTo(copy.AsSpan(start));

        return copy;
    }

    private byte[] Splice(byte[] input, IReadOnlyList<byte[]> corpus)
    {
        var other = corpus[(int)(NextValue() % (ulong)corpus.Count)];

        if (input.Length == 0 || other.Length == 0)
        {
            return input;
        }

        var cut = Index(input.Length);
        var take = Index(other.Length);
        var copy = new byte[cut + (other.Length - take)];

        input.AsSpan(0, cut).CopyTo(copy);
        other.AsSpan(take).CopyTo(copy.AsSpan(cut));

        return copy;
    }

    /// <summary>
    /// Bumps the operand of an instruction that carries an index, leaving the opcode alone.
    /// </summary>
    /// <remarks>
    /// <b>Added because the undirected mutations could not reach the defect this fuzzer exists to
    /// catch.</b> Removing the verifier's constant-index check leaves an artifact that verifies
    /// and then indexes past the pool in the executor - and twenty-five thousand undirected
    /// iterations did not find it. To trigger it a mutant must still verify, which only about
    /// three in a hundred do, AND must carry a non-zero index in the two operand bytes of a
    /// specific opcode; a random byte poke satisfies both about never.
    ///
    /// So this one looks for an opcode byte that takes a <c>u16</c> index and writes a small
    /// non-zero value into the byte after it. It is structure-aware in the weakest possible sense
    /// - it matches a byte value and does not parse anything - and it is what turned a fuzzer that
    /// found nothing because it could not into one that finds this class in seconds.
    /// </remarks>
    private byte[] PerturbOperand(byte[] input)
    {
        if (input.Length < 3)
        {
            return input;
        }

        var copy = (byte[])input.Clone();
        var start = Index(copy.Length - 2);

        for (var offset = 0; offset < copy.Length - 2; offset++)
        {
            var at = (start + offset) % (copy.Length - 2);

            if (copy[at] is not (0x10 or 0x11 or 0x12))
            {
                continue;
            }

            copy[at + 1] = (byte)(1 + (NextValue() % 8));
            return copy;
        }

        return SetInterestingByte(input);
    }

    /// <summary>
    /// Rewrites one variable-length integer in place, which is where a length or a count lives.
    /// </summary>
    /// <remarks>
    /// The undirected mutations reach a length field eventually and mostly turn it into a
    /// different small number. This one writes the values a bound is compared against - a maximum,
    /// a maximum plus one, a value whose continuation bits run off the end - because a
    /// declared-count defect is not found by drifting towards it.
    /// </remarks>
    private byte[] RewriteLength(byte[] input)
    {
        if (input.Length < 2)
        {
            return input;
        }

        var copy = (byte[])input.Clone();
        var at = Index(copy.Length - 1);

        switch (NextValue() % 4)
        {
            case 0:
                copy[at] = 0xFF;
                copy[at + 1] = 0xFF;
                break;

            case 1:
                copy[at] = 0x80;
                copy[at + 1] = 0x80;
                break;

            case 2:
                copy[at] = 0x81;
                copy[at + 1] = 0x00;
                break;

            default:
                copy[at] = (byte)(NextValue() % 128);
                break;
        }

        return copy;
    }
}

/// <summary>What one verification of one mutant did.</summary>
internal sealed record FuzzObservation(
    VmOutcome Outcome,
    VmReason Reason,
    int DiagnosticCode,
    bool ProducedHandle,
    bool Escaped,
    string EscapedTypeName,
    ulong AllocatedBytes,
    string ExecutionStep);

/// <summary>
/// Roadmap section 7's second discipline, over the two of its four surfaces that exist.
/// </summary>
/// <remarks>
/// <para>
/// The section asks for coverage-guided fuzzing over four surfaces: the verifier, the source
/// tokenizer and parser, the regular-expression matcher, and the executor over
/// verified-but-adversarial artifacts. **Two of the four exist at this milestone** - the verifier
/// and the executor - and the other two are surfaces this profile has not written yet. What is
/// here covers the two that exist and says so; JS-9 owns the rest and the session budgets.
/// </para>
/// <para>
/// <b>It finds nothing by itself.</b> What it produces when it does find something is the valuable
/// part: a minimized input, retained, which becomes a named corpus entry. A counterexample closed
/// by an allow-list entry is not closed.
/// </para>
/// </remarks>
internal static class Fuzzing
{
    /// <summary>How many bytes of allocation one mutant byte may authorise.</summary>
    /// <remarks>
    /// The same bound the ordering checks hold the retained corpus to, applied to every mutant.
    /// This is the part a hand-written corpus cannot do: sixty entries check the ordering on sixty
    /// shapes, and a session checks it on every shape the mutator reaches.
    /// </remarks>
    private const ulong AllocationBytesPerArtifactByte = 64;

    internal static int Run(string directory, ulong seed, int iterations, bool verbose)
    {
        // Seeded from the MANIFEST and not from a glob over the directory. A session is claimed to
        // be a total function of its seed and its seed corpus, and a glob makes that false the
        // first time a finding is retained: the next session picks the finding up as a
        // sixty-first seed and answers differently for the same seed. Found by running this
        // twice.
        var corpus = CorpusReplay
            .ReadManifest(Path.Combine(directory, "corpus.manifest"))
            .Select(entry => File.ReadAllBytes(Path.Combine(directory, entry.Name + ".bjsb")))
            .ToArray();

        if (corpus.Length == 0)
        {
            Console.WriteLine($"broiler-js-execution-only: no seed artifacts in {directory}");
            return 2;
        }

        Console.WriteLine(
            $"broiler-js-fuzz: seed {seed}, {iterations} iterations, {corpus.Length} seed artifacts, " +
            "surfaces: verifier and executor");

        var mutator = new ArtifactMutator(seed);
        var histogram = new Dictionary<string, int>(StringComparer.Ordinal);
        var verified = 0;

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            var input = mutator.Next(corpus);

            // Every fourth iteration runs under a HOST THAT DECLINES. With unconstrained ceilings
            // a resource exhaustion is unreachable, so three of the five outcomes the invariants
            // admit would never be produced and the clauses about them would be quantifiers over
            // nothing. The tight vector is not a measurement and is not tuned: it is small enough
            // that most artifacts breach something and large enough that some do not.
            var observation = Observe(input, tight: iteration % 4 == 3);
            // A verified mutant's key carries what the EXECUTOR did with it. The verifier's answer
            // for an admitted artifact is always Normal, so a histogram keyed on that alone would
            // report the whole executor surface as one bucket - and the executor is half of what
            // this session claims to cover.
            var key = observation.Escaped
                ? "Escaped:" + observation.EscapedTypeName
                : observation.Outcome == VmOutcome.Normal
                    ? "Normal/" + observation.ExecutionStep
                    : observation.Outcome.ToString();

            histogram[key] = histogram.GetValueOrDefault(key) + 1;
            verified += observation.Outcome == VmOutcome.Normal ? 1 : 0;

            var violations = Violations(observation, input.Length);

            if (violations.Count == 0)
            {
                continue;
            }

            Console.WriteLine($"broiler-js-fuzz: FINDING at iteration {iteration} of seed {seed}");

            foreach (var violation in violations)
            {
                Console.WriteLine("  " + violation);
            }

            // Beside the corpus and not IN it, for the same reason the seeds come from the
            // manifest: a retained corpus is a set of entries with recorded answers, and a file
            // dropped into it is neither.
            var findings = Path.Combine(directory, "..", "js-1-fuzz-findings");
            Directory.CreateDirectory(findings);

            var minimized = Minimize(input);
            var path = Path.Combine(findings, $"fuzz-finding-seed{seed}-iteration{iteration}.bjsb");
            File.WriteAllBytes(path, minimized);

            // Length AND non-zero bytes, because for a length-framed format the minimizer mostly
            // blanks rather than shortens - and a line reporting only the length would say
            // "minimized from 100 to 100 bytes" about a pass that emptied most of the artifact.
            Console.WriteLine(
                $"  minimized from {input.Length} bytes " +
                $"({input.Count(static value => value != 0)} non-zero) to {minimized.Length} bytes " +
                $"({minimized.Count(static value => value != 0)} non-zero)");
            Console.WriteLine($"  retained as {path}");
            Console.WriteLine(
                "  Close it with a NAMED CORPUS ENTRY and a fix, never with an allow-list entry.");

            return 1;
        }

        Console.WriteLine($"broiler-js-fuzz: no counterexample in {iterations} iterations.");

        foreach (var pair in histogram.OrderBy(static entry => entry.Key, StringComparer.Ordinal))
        {
            Console.WriteLine($"  {pair.Key}: {pair.Value}");
        }

        // A session whose mutants all answered the same way exercised one path, and reporting it as
        // success would let a broken mutator read as twenty thousand clean iterations. The same
        // argument applies to the executor surface: if nothing verified, nothing was executed, and
        // half of what this session claims to cover was never reached.
        if (histogram.Count < 2)
        {
            Console.WriteLine(
                "broiler-js-fuzz: every iteration answered the same way. The session exercised one " +
                "path and proves nothing.");

            return 3;
        }

        if (verified == 0)
        {
            Console.WriteLine(
                "broiler-js-fuzz: no mutant verified, so the executor surface was never reached. " +
                "The session covers the verifier alone and may not be read as covering both.");

            return 4;
        }

        Console.WriteLine(
            $"broiler-js-fuzz: {verified} mutants verified and were instantiated and invoked, so " +
            "both surfaces were reached.");

        return 0;
    }

    /// <summary>Replays one retained input, which is how a finding is reproduced.</summary>
    internal static int Replay(string path)
    {
        var input = File.ReadAllBytes(path);
        var observation = Observe(input);
        var violations = Violations(observation, input.Length);

        Console.WriteLine(
            $"{path}: {input.Length} bytes, {observation.Outcome}/{observation.Reason}/" +
            $"{observation.DiagnosticCode}, step {observation.ExecutionStep}, " +
            $"{observation.AllocatedBytes} allocated bytes");

        foreach (var violation in violations)
        {
            Console.WriteLine("  " + violation);
        }

        return violations.Count == 0 ? 0 : 1;
    }

    /// <summary>
    /// The invariants a mutant may not break, whatever it is.
    /// </summary>
    /// <remarks>
    /// Nothing here is about the mutant being well formed. Every one of these holds for arbitrary
    /// bytes, which is what makes them invariants rather than expectations.
    /// </remarks>
    private static List<string> Violations(FuzzObservation observation, int inputLength)
    {
        var violations = new List<string>();

        if (observation.Escaped)
        {
            violations.Add("an exception escaped: " + observation.EscapedTypeName);
            return violations;
        }

        var admitted = observation.Outcome
            is VmOutcome.Normal
            or VmOutcome.InvalidArtifact
            or VmOutcome.ResourceExhaustion
            or VmOutcome.Cancellation
            or VmOutcome.UnsupportedProfile;

        if (!admitted)
        {
            violations.Add($"verification answered {observation.Outcome}, which it may not produce");
        }

        if (observation.ProducedHandle != (observation.Outcome == VmOutcome.Normal))
        {
            violations.Add(
                $"answered {observation.Outcome} and " +
                (observation.ProducedHandle ? "produced a handle" : "produced no handle"));
        }

        if (observation.Outcome == VmOutcome.InvalidArtifact && observation.DiagnosticCode == 0)
        {
            violations.Add("an invalid artifact carries no diagnostic code");
        }

        // The executor surface. A verified artifact is one the verifier promised the executor can
        // run, so a contract violation or an untyped fault out of it is a broken promise rather
        // than a bad program - and it is the class this fuzzer exists to find, because the
        // artifacts that provoke it are the ones nobody wrote.
        if (observation.ExecutionStep is "fault:UNTYPED")
        {
            violations.Add(
                "a verified artifact faulted with no typed payload, so something threw out of the " +
                "executor and the core reported it");
        }

        if (observation.ExecutionStep.StartsWith("invoke:ContractViolation", StringComparison.Ordinal))
        {
            violations.Add("a verified artifact produced a contract violation when invoked");
        }

        var permitted = (ulong)inputLength * AllocationBytesPerArtifactByte;

        if (observation.AllocatedBytes > permitted)
        {
            violations.Add(
                $"charged {observation.AllocatedBytes} allocated bytes from {inputLength} input " +
                $"bytes, past {permitted}");
        }

        return violations;
    }

    /// <summary>Verifies one input, and executes it if it verified.</summary>
    private static FuzzObservation Observe(byte[] input, bool tight = false)
    {
        var context = new RecordingContext(tight ? TightCeilings : VmLimitVector.Unconstrained);
        var descriptor = Hosts.Descriptor("default");

        try
        {
            var outcome = JavaScriptProfile.Descriptor.Verifier.Verify(
                in descriptor, input, context, CancellationToken.None);

            var step = outcome.Category == VmOutcome.Normal ? Execute(input) : "-";

            return new FuzzObservation(
                outcome.Category,
                outcome.Reason,
                outcome.ProfileDiagnosticCode,
                outcome.State is not null,
                Escaped: false,
                EscapedTypeName: string.Empty,
                context.Recorder.Total(VmBudgetDimension.AllocatedBytes),
                step);
        }
        catch (Exception failure)
        {
            return new FuzzObservation(
                VmOutcome.Normal, VmReason.None, 0, false, true, failure.GetType().FullName!,
                context.Recorder.Total(VmBudgetDimension.AllocatedBytes), "-");
        }
    }

    /// <summary>
    /// The fourth surface: the executor over an artifact that verified and is still adversarial.
    /// </summary>
    /// <remarks>
    /// A mutant that verifies is by construction a valid artifact, and it is also one nobody
    /// wrote. Running it is the only way to find an executor that trusts something the verifier
    /// does not actually promise.
    /// </remarks>
    private static string Execute(byte[] input)
    {
        using var runtime = Hosts.Runtime("default", out _);

        if (runtime is null)
        {
            return "host-failure";
        }

        var descriptor = Hosts.Descriptor("default");
        var verified = runtime.Verify(in descriptor, input, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return "not-admitted-by-the-runtime";
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return "instantiation:" + instantiated.Reason;
        }

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        // The outcome alone does not say enough, and this is the distinction the whole executor
        // surface turns on. A guest program is ALLOWED to fault: an entry-point name nothing is
        // bound to is a ReferenceError, and a mutated name produces one constantly. What is not
        // allowed is a fault the profile did not author - the core catching an exception out of
        // the executor and reporting it as one - because that is the verifier having admitted
        // something the executor could not run. The two are told apart by whether this profile's
        // own typed payload came back with it.
        if (result.Outcome != VmOutcome.ProfileFault)
        {
            return "invoke:" + result.Outcome;
        }

        return JavaScriptProfile.TryGetFault(in result, out var fault)
            ? "fault:" + fault.Kind
            : "fault:UNTYPED";
    }

    /// <summary>
    /// Shrinks a counterexample while it stays one.
    /// </summary>
    /// <remarks>
    /// Chunks first and then single bytes, and every candidate is kept only if it still violates
    /// something. A minimizer that accepted a candidate breaking a DIFFERENT invariant would hand
    /// back an input that reproduces a finding nobody has seen.
    /// </remarks>
    private static byte[] Minimize(byte[] input)
    {
        var best = input;
        var signature = Signature(best);

        for (var chunk = 64; chunk >= 1; chunk /= 2)
        {
            var progressed = true;

            while (progressed)
            {
                progressed = false;

                for (var at = 0; at + chunk <= best.Length; at++)
                {
                    var candidate = new byte[best.Length - chunk];
                    best.AsSpan(0, at).CopyTo(candidate);
                    best.AsSpan(at + chunk).CopyTo(candidate.AsSpan(at));

                    if (!string.Equals(Signature(candidate), signature, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    best = candidate;
                    progressed = true;
                    break;
                }
            }
        }

        // Then a pass that BLANKS bytes instead of removing them. Deletion rarely shrinks an
        // artifact of a length-framed format - every removal moves a section boundary and the
        // finding goes with it - so on its own the minimizer hands back what it was given.
        // Blanking keeps every length intact and empties whatever the finding does not depend on,
        // which is what makes the retained bytes readable by the person who has to close it.
        for (var at = 0; at < best.Length; at++)
        {
            if (best[at] == 0)
            {
                continue;
            }

            var candidate = (byte[])best.Clone();
            candidate[at] = 0;

            if (string.Equals(Signature(candidate), signature, StringComparison.Ordinal))
            {
                best = candidate;
            }
        }

        return best;
    }

    /// <summary>
    /// A host that declines: small ceilings on the four dimensions a verification can breach.
    /// </summary>
    /// <remarks>
    /// Stated rather than derived, and deliberately not the profile's declared defaults: what this
    /// exists to reach is the resource path, and a vector copied from the defaults would only
    /// reach it for artifacts the defaults already refuse.
    /// </remarks>
    private static VmLimitVector TightCeilings { get; } = Tighten();

    private static VmLimitVector Tighten()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        Array.Fill(values, ulong.MaxValue);

        values[(int)VmBudgetDimension.ArtifactBytes] = 96;
        values[(int)VmBudgetDimension.AllocatedBytes] = 512;
        values[(int)VmBudgetDimension.DeclaredCount] = 8;
        values[(int)VmBudgetDimension.SectionCount] = 4;

        return VmLimitVector.TryCreate(values, out var vector)
            ? vector
            : throw new InvalidOperationException("the frozen dimension count moved");
    }

    /// <summary>What a candidate must keep doing to count as the same finding.</summary>
    private static string Signature(byte[] candidate)
    {
        var observation = Observe(candidate);
        var violations = Violations(observation, candidate.Length);

        return violations.Count == 0 ? string.Empty : string.Join("; ", violations);
    }
}
