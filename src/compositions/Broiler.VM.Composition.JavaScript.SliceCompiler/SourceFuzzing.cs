using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Mutates JavaScript source text, drawing from a seed set and perturbing what it draws.
/// </summary>
/// <remarks>
/// <para>
/// <b>Text and not bytes, which is the whole reason this is a second mutator rather than the
/// first one pointed somewhere else.</b> The artifact mutator flips bits in a binary whose framing
/// it is trying to break; a source mutator that did the same would spend its session producing
/// invalid UTF-8 and would exercise one arm of the tokenizer forever. What is wanted here is text
/// that is nearly a program: the operations below cut, duplicate and splice at line and token
/// boundaries, and the one that inserts inserts from the language's own vocabulary.
/// </para>
/// <para>
/// <b>xorshift64*, written out rather than taken from a library</b>, for the reason the sibling
/// mutator gives: a session is reproduced by naming its seed, and a sequence that is the
/// framework's is a sequence that can change under this component without a line of it moving.
/// </para>
/// </remarks>
internal sealed class SourceMutator
{
    /// <summary>
    /// Fragments drawn from the language this front end reads, not from an alphabet.
    /// </summary>
    /// <remarks>
    /// <b>Chosen so a mutant stays nearly-a-program.</b> Random characters reach the tokenizer's
    /// "this begins no token" arm and stop; these reach the parser and the validation stage, which
    /// is where the interesting answers are. The set deliberately mixes constructs the manifest
    /// admits with constructs it declines, because refusing the second kind BY NAME is half of
    /// what the front end is for.
    /// </remarks>
    private static readonly string[] Fragments =
    [
        "var ", "let ", "const ", "function ", "return ", "if (", "else ", "while (", "do ",
        "for (", "break", "continue", "typeof ", "new ", "class ", "=>", "...", "?.", "??",
        "{", "}", "(", ")", "[", "]", ";", ",", "=", "===", "!==", "+", "-", "*", "/", "%",
        "\"use strict\";", "#!", "0x", "1e", ".5", "'", "\"", "`", "//", "/*", "*/", "\\u",
    ];

    private ulong state;

    internal SourceMutator(ulong seed) => state = seed == 0 ? 0x9E3779B97F4A7C15 : seed;

    /// <summary>One mutant, drawn from the pool and perturbed.</summary>
    internal string Next(IReadOnlyList<string> corpus)
    {
        var input = corpus[(int)(NextValue() % (ulong)corpus.Count)];

        return (NextValue() % 10) switch
        {
            0 => ReplaceCharacter(input),
            1 => DeleteCharacter(input),
            2 => InsertFragment(input),
            3 => Truncate(input),
            4 => DuplicateLine(input),
            5 => DeleteLine(input),
            6 => Splice(input, corpus),
            7 => UnbalanceABracket(input),
            8 => Nest(input),
            _ => RepeatFragment(input),
        };
    }

    /// <summary>xorshift64*, so the sequence is this file's and not a library's.</summary>
    private ulong NextValue()
    {
        state ^= state >> 12;
        state ^= state << 25;
        state ^= state >> 27;
        return state * 0x2545F4914F6CDD1D;
    }

    private int Index(int length) => length == 0 ? 0 : (int)(NextValue() % (ulong)length);

    private string Fragment() => Fragments[(int)(NextValue() % (ulong)Fragments.Length)];

    private string ReplaceCharacter(string input)
    {
        if (input.Length == 0)
        {
            return Fragment();
        }

        var at = Index(input.Length);
        var fragment = Fragment();
        return input[..at] + fragment[Index(fragment.Length)] + input[(at + 1)..];
    }

    private string DeleteCharacter(string input) =>
        input.Length == 0 ? input : input.Remove(Index(input.Length), 1);

    private string InsertFragment(string input) =>
        input.Insert(input.Length == 0 ? 0 : Index(input.Length + 1), Fragment());

    private string Truncate(string input) =>
        input.Length == 0 ? input : input[..Index(input.Length)];

    private string DuplicateLine(string input)
    {
        var lines = Lines(input);
        var at = Index(lines.Count);
        lines.Insert(at, lines[at]);
        return string.Join('\n', lines);
    }

    private string DeleteLine(string input)
    {
        var lines = Lines(input);

        if (lines.Count <= 1)
        {
            return input;
        }

        lines.RemoveAt(Index(lines.Count));
        return string.Join('\n', lines);
    }

    /// <summary>Half of one seed and half of another, joined at a line boundary.</summary>
    private string Splice(string input, IReadOnlyList<string> corpus)
    {
        var other = corpus[(int)(NextValue() % (ulong)corpus.Count)];
        var head = Lines(input);
        var tail = Lines(other);

        return string.Join(
            '\n',
            head.Take(Index(head.Count) + 1).Concat(tail.Skip(Index(tail.Count))));
    }

    /// <summary>
    /// Removes one closing bracket, which the balanced operations above cannot produce.
    /// </summary>
    /// <remarks>
    /// Written as its own operation because it is the mutation most likely to reach the parser's
    /// recovery arms, and the line and splice operations tend to cut whole balanced regions.
    /// </remarks>
    private string UnbalanceABracket(string input)
    {
        var closers = new List<int>();

        for (var index = 0; index < input.Length; index++)
        {
            if (input[index] is '}' or ')' or ']')
            {
                closers.Add(index);
            }
        }

        return closers.Count == 0 ? input : input.Remove(closers[Index(closers.Count)], 1);
    }

    /// <summary>
    /// Wraps the input in blocks, which is the one shape that reaches the nesting bound.
    /// </summary>
    /// <remarks>
    /// Bounded at four times the front end's own default depth: enough that the bound is reached
    /// and refused rather than survived, and not so much that a session spends its time building
    /// strings. The front end's answer to this is an explicit refusal, so a mutant that reaches it
    /// is a signal like any other.
    /// </remarks>
    private string Nest(string input)
    {
        var depth = 1 + (int)(NextValue() % (SliceParseOptions.DefaultMaximumNestingDepth * 4));
        return new string('{', depth) + input + new string('}', depth);
    }

    private string RepeatFragment(string input)
    {
        var fragment = Fragment();
        var times = 1 + (int)(NextValue() % 64);
        var builder = new StringBuilder(input);

        for (var repeat = 0; repeat < times; repeat++)
        {
            builder.Append(fragment);
        }

        return builder.ToString();
    }

    private static List<string> Lines(string input) =>
        [.. input.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n')];
}

/// <summary>The seeds a source session draws from, and the answers it has already reached.</summary>
/// <remarks>
/// The same shape as the artifact session's pool and for the same reasons: bounded so that a long
/// session's draw distribution is not a function of how many answers it happened to find, and
/// primed from the retained corpus so that "new" means new against what is retained rather than
/// new since the last iteration.
/// </remarks>
internal sealed class SourceSeedPool
{
    private readonly List<string> sources;
    private readonly HashSet<string> answers = new(StringComparer.Ordinal);
    private readonly int ceiling;

    internal SourceSeedPool(IEnumerable<string> corpus, int poolCeiling)
    {
        sources = [.. corpus];
        ceiling = poolCeiling;
    }

    /// <summary>The seeds, as the mutator draws from them.</summary>
    internal IReadOnlyList<string> Sources => sources;

    /// <summary>How many distinct answers this pool has been told about.</summary>
    internal int Answers => answers.Count;

    /// <summary>How many the seed corpus alone reached, fixed when priming ended.</summary>
    internal int Baseline { get; private set; }

    /// <summary>How many mutants were offered. A session offers every one it draws.</summary>
    internal int Considered { get; private set; }

    /// <summary>How many were kept as further seeds.</summary>
    internal int Kept { get; private set; }

    /// <summary>Records an answer the seed corpus reaches, keeping nothing.</summary>
    internal void Prime(string coverage)
    {
        answers.Add(coverage);
        Baseline = answers.Count;
    }

    /// <summary>Offers one mutant and its answer. Keeps it if the answer is new.</summary>
    internal bool Consider(string source, string coverage)
    {
        Considered++;

        if (!answers.Add(coverage))
        {
            return false;
        }

        if (sources.Count < ceiling)
        {
            sources.Add(source);
            Kept++;
            return true;
        }

        return false;
    }
}

/// <summary>What one source mutant reached, as this session can observe it.</summary>
/// <param name="Refused">Whether the front end refused the source.</param>
/// <param name="DiagnosticCode">The embedder-seam code it refused with, or empty.</param>
/// <param name="Verified">Whether an artifact it produced was accepted by the verifier.</param>
/// <param name="VerifierReason">The core's reason where the verifier refused, or empty.</param>
/// <param name="EscapedTypeName">The type of an exception that escaped, or empty.</param>
internal sealed record SourceObservation(
    bool Refused,
    string DiagnosticCode,
    bool Verified,
    string VerifierReason,
    string EscapedTypeName);

/// <summary>
/// Roadmap section 7's second discipline over its second surface: the source tokenizer and parser.
/// </summary>
/// <remarks>
/// <para>
/// <b>This surface was named as unfuzzed for as long as it did not exist, and then for a while
/// after it did.</b> Section 7 asks for fuzzing over four surfaces - the verifier, the source
/// tokenizer and parser, the regular-expression matcher, and the executor over adversarial
/// artifacts. The sibling session in the execution-only root covers the first and the fourth and
/// says in its own words that the other two "are surfaces this profile has not written yet". That
/// stopped being true of the source surface at JS-3b, and the ledger's JS-3b row has carried
/// <i>the front end is fuzzed by nothing</i> as a named gap since. This closes it.
/// </para>
/// <para>
/// <b>The session's central assertion is not "it did not crash".</b> It is that
/// <b>a source this front end compiles produces an artifact this profile's own verifier
/// accepts</b>. The two stages check disjoint things by design - a refused source has no bytes,
/// and the verifier reads bytes whatever produced them - and the seam between them is exactly
/// where a lowering can emit something structurally wrong without any test noticing, because the
/// front end's tests end at "it compiled" and the verifier's begin at bytes somebody handed it.
/// </para>
/// <para>
/// <b>That is not a hypothetical seam.</b> Three defects of exactly that shape were repaired on
/// 2026-09-03, each found by pointing a real conformance suite at the host rather than by any
/// check here: a loop whose body always breaks emitted a continuation nothing reached, a block
/// continued to be lowered after a statement control could not pass, and a loop nothing could
/// leave left a program tail no execution reached - the first two refused as
/// <c>UnreachableCode</c> and the third, once repaired, as
/// <c>JumpTargetNotAnInstructionBoundary</c>. Every one of them is a source that compiled and did
/// not verify. A session asserting this invariant would have found all three without a suite.
/// </para>
/// <para>
/// <b>Answer-guided, with the same bound the sibling states.</b> A mutant's signal is the answer
/// this profile publishes about it - the seam diagnostic of a refusal, or the fact that it
/// compiled and verified. Two mutants that take different paths to one answer are one signal, so
/// this is not edge coverage and a defect on a path that answers like its neighbour is invisible
/// to the guidance. <see cref="Broiler.VM.Profile.JavaScript.Compiler.SliceSourceDiagnosticCode"/>
/// is as fine as the signal gets.
/// </para>
/// </remarks>
internal static class SourceFuzzing
{
    /// <summary>How many sources the pool may hold before it stops growing.</summary>
    /// <remarks>
    /// The sibling's ceiling and for the sibling's reason. The seam vocabulary is 22 codes, so a
    /// pool that reaches this has kept far more distinct answers than the vocabulary has members
    /// and is drawing from findings rather than from seeds.
    /// </remarks>
    private const int PoolCeiling = 512;

    /// <summary>How large a seed the mutator may draw from.</summary>
    /// <remarks>
    /// <b>PRIMING AND DRAWING ARE DIFFERENT QUESTIONS, and the first draft asked them as one.</b>
    /// Every entry of the retained corpus is primed, so the session knows which answers the corpus
    /// reaches; two of those entries are a program of 65,536 declarations and one of 65,536
    /// distinct constants, three-quarters of a megabyte each. A pool that also DREW from them spent
    /// about one iteration in thirty splicing, nesting and recompiling three-quarters of a
    /// megabyte, which took a 25,000-iteration session from two seconds to more than ten minutes
    /// and bought nothing: the answers those two reach are already primed, and a mutant of one is
    /// overwhelmingly the same answer again. Every other retained source is under a kilobyte, so
    /// this bound excludes exactly those two and is stated rather than tuned.
    /// </remarks>
    private const int LargestDrawableSeed = 64 * 1024;

    /// <summary>
    /// The published answer about one source, which is what the guidance keys on.
    /// </summary>
    /// <remarks>
    /// <b>A verifier refusal is not in this vocabulary, deliberately.</b> Every other outcome here
    /// is an answer this profile is entitled to give about a source; bytes this lowering produced
    /// and this verifier refused are a defect in this component, and folding them into the
    /// guidance would make the session explore them rather than report them.
    /// </remarks>
    internal static string Coverage(SourceObservation observation) =>
        observation.EscapedTypeName.Length != 0
            ? "escaped:" + observation.EscapedTypeName
            : observation.Refused
                ? "refused:" + observation.DiagnosticCode
                : "compiled-and-verified";

    /// <summary>Whether an observation is a finding rather than a signal.</summary>
    /// <remarks>
    /// Two shapes and they are different failures. An escaped exception is a front end that did
    /// not answer at all - roadmap section 9 requires a source to be refused or lowered, never to
    /// throw past its caller. A compiled artifact the verifier refuses is the seam defect this
    /// session exists to find.
    /// </remarks>
    internal static string WhyItIsAFinding(SourceObservation observation)
    {
        if (observation.EscapedTypeName.Length != 0)
        {
            return $"the front end threw {observation.EscapedTypeName} instead of answering";
        }

        if (!observation.Refused && !observation.Verified)
        {
            return
                "the front end compiled this source and the verifier refused the bytes: " +
                observation.VerifierReason;
        }

        return string.Empty;
    }

    /// <summary>
    /// Whether the guidance loop keeps a new answer and refuses a repeat.
    /// </summary>
    /// <remarks>
    /// Run at the start of every session before it reports any guidance figure, and again as a
    /// named check of this composition, so a publish that never fuzzes still carries it. The
    /// sibling's argument applies unchanged: whether a session's seed set grows is a fact about
    /// the corpus it started from, so growth cannot be what fails, and this can.
    /// </remarks>
    internal static (bool Wired, string Detail) GuidanceLoopIsWired()
    {
        var pool = new SourceSeedPool(["1;"], PoolCeiling);
        pool.Prime("probe:already-reached");

        var kept = pool.Consider("2;", "probe:new");
        var repeated = pool.Consider("3;", "probe:new");
        var primed = pool.Consider("4;", "probe:already-reached");

        if (!kept)
        {
            return (false, "an answer nothing had reached was not kept");
        }

        if (repeated || primed)
        {
            return (false, "an answer already reached was kept as a further seed");
        }

        if (pool.Considered != 3)
        {
            return (false, $"three mutants were offered and the pool counted {pool.Considered}");
        }

        return (true, $"{pool.Kept} of {pool.Considered} offers kept, baseline {pool.Baseline}");
    }

    /// <summary>Compiles one source and, where it compiled, verifies what came out.</summary>
    /// <remarks>
    /// <b>Every escape is caught here and nowhere else.</b> A session that let one propagate would
    /// stop at the first defect it found, which is the opposite of what a session is for; catching
    /// it makes the mutant a finding and lets the run continue. <see cref="OutOfMemoryException"/>
    /// is deliberately not caught: it says nothing about the front end.
    /// </remarks>
    internal static SourceObservation Observe(string source, SliceParseOptions options)
    {
        try
        {
            var compiled = SliceSourceCompiler.Compile(source, options);

            if (!compiled.Succeeded)
            {
                return new SourceObservation(
                    Refused: true,
                    DiagnosticCode: compiled.Diagnostics.Count == 0
                        ? "(no diagnostic)"
                        : compiled.Diagnostics[0].Code.ToString(),
                    Verified: false,
                    VerifierReason: string.Empty,
                    EscapedTypeName: string.Empty);
            }

            return Verify(compiled.Artifact!);
        }
        catch (Exception failure) when (failure is not OutOfMemoryException)
        {
            return new SourceObservation(
                Refused: false,
                DiagnosticCode: string.Empty,
                Verified: false,
                VerifierReason: string.Empty,
                EscapedTypeName: failure.GetType().Name);
        }
    }

    /// <summary>Asks this profile's own verifier what it makes of the bytes.</summary>
    private static SourceObservation Verify(byte[] artifact)
    {
        var created = VmRuntime.Create(Catalog(), Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return new SourceObservation(
                false, string.Empty, false, "the runtime refused creation", string.Empty);
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                1,
                JavaScriptProfile.SliceManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity("broiler.vm.composition.javascript.slicecompiler"));

            var verified = runtime.Verify(in descriptor, artifact, CancellationToken.None);

            if (verified.TryGetArtifact(out _))
            {
                return new SourceObservation(false, string.Empty, true, string.Empty, string.Empty);
            }

            // AN EXHAUSTION AT VERIFICATION IS NOT AN ARTIFACT REFUSAL, and reading it as one
            // would make a session report a defect every time a mutant grew past a ceiling. The
            // end-user host had that exact confusion and it was corrected there; this is the same
            // distinction, made once rather than rediscovered.
            if (verified.Outcome == VmOutcome.ResourceExhaustion)
            {
                return new SourceObservation(
                    false, string.Empty, true, string.Empty, string.Empty);
            }

            return new SourceObservation(
                false,
                string.Empty,
                false,
                $"{verified.Diagnostics.ProfileDiagnosticCode} ({verified.Outcome}/{verified.Reason})",
                string.Empty);
        }
    }

    /// <summary>Runs one session over a seed directory of source files.</summary>
    internal static int Run(string directory, ulong seed, int iterations)
    {
        var corpus = Seeds();

        if (corpus.Count == 0)
        {
            Console.WriteLine($"broiler-js-slice-compiler: no seed sources under {directory}");
            return 2;
        }

        Console.WriteLine(
            $"broiler-js-source-fuzz: seed {seed}, {iterations} iterations, {corpus.Count} seed " +
            $"sources of which {corpus.Count(static source => source.Length <= LargestDrawableSeed)} " +
            "are drawn from, surface: the source tokenizer, parser and lowering");

        var (wired, detail) = GuidanceLoopIsWired();

        if (!wired)
        {
            Console.WriteLine("broiler-js-source-fuzz: the guidance loop is not wired: " + detail);
            return 5;
        }

        var mutator = new SourceMutator(seed);
        var pool = new SourceSeedPool(
            corpus.Where(static source => source.Length <= LargestDrawableSeed), PoolCeiling);

        var histogram = new Dictionary<string, int>(StringComparer.Ordinal);

        // BOTH GOALS, because they are two front ends as far as the validation stage is concerned
        // and a session over one would leave the other's early errors unexplored.
        SliceParseOptions[] goals = [SliceParseOptions.Script, SliceParseOptions.Module];

        foreach (var source in corpus)
        {
            foreach (var options in goals)
            {
                pool.Prime(Coverage(Observe(source, options)));
            }
        }

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            var mutant = mutator.Next(pool.Sources);
            var options = goals[iteration % goals.Length];
            var observation = Observe(mutant, options);
            var coverage = Coverage(observation);

            histogram[coverage] = histogram.GetValueOrDefault(coverage) + 1;
            pool.Consider(mutant, coverage);

            var finding = WhyItIsAFinding(observation);

            if (finding.Length != 0)
            {
                return Report(directory, seed, iteration, mutant, finding);
            }
        }

        Console.WriteLine(
            $"broiler-js-source-fuzz: {iterations} mutants, {pool.Answers} distinct answers " +
            $"({pool.Baseline} reached by the seed corpus alone), {pool.Kept} kept as further seeds");

        foreach (var (answer, count) in histogram.OrderByDescending(entry => entry.Value))
        {
            Console.WriteLine(
                $"  {count.ToString(CultureInfo.InvariantCulture),8}  {answer}");
        }

        Console.WriteLine("broiler-js-source-fuzz: no finding");
        return 0;
    }

    /// <summary>Retains a finding beside the corpus and names it.</summary>
    /// <remarks>
    /// <b>The input is written out, because a session that reported a defect and discarded the
    /// text that produced it would be asking somebody to find it again.</b> A retained finding is
    /// what becomes a named corpus entry once the defect it names is repaired.
    /// </remarks>
    private static int Report(string directory, ulong seed, int iteration, string mutant, string why)
    {
        Console.WriteLine(
            $"broiler-js-source-fuzz: FINDING at iteration {iteration} of seed {seed}: {why}");

        var findings = Path.Combine(directory, "..", "js-1-source-fuzz-findings");
        Directory.CreateDirectory(findings);

        var path = Path.Combine(findings, $"source-fuzz-seed{seed}-iteration{iteration}.js");
        File.WriteAllText(path, mutant);
        Console.WriteLine($"broiler-js-source-fuzz: retained {path}");
        return 1;
    }

    /// <summary>
    /// The seed set: the retained source corpus, as the checks in this composition read it.
    /// </summary>
    /// <remarks>
    /// <b>The corpus and not the files beside its manifest, and the difference is not
    /// cosmetic.</b> The first draft of this session enumerated `*.js` under the corpus directory,
    /// which is 55 of the corpus's 57 entries: two are retained as `generated` because they are a
    /// program of 65,536 declarations and one of 65,536 distinct constants, and a file of either
    /// is not a reviewable thing. Those two are the only sources that reach `TooManyLocals` and
    /// `TooManyConstants`, so a session seeded from the directory could not prime them and eight
    /// sessions over 200,000 mutants reached 21 of the 24 seam codes rather than 23.
    /// </remarks>
    private static IReadOnlyList<string> Seeds() =>
    [
        .. SliceSourcePrograms.Accepted.Select(static program => program.Source),
        .. SliceSourcePrograms.Refused.Select(static program => program.Source),
    ];

    private static VmCatalog Catalog() => VmCatalog.CreateBuilder()
        .Add(JavaScriptProfile.Descriptor)
        .Build();

    /// <summary>
    /// The runtime a session verifies under, at this profile's own declared defaults.
    /// </summary>
    /// <remarks>
    /// <b>Defaults rather than tight ceilings, because a verifier refusal is a finding here.</b>
    /// The sibling session runs its artifacts under deliberately stingy vectors to reach the
    /// budget arms, which is right when an exhaustion is a signal; in this session an artifact
    /// that fails to verify is a defect in the lowering, and a ceiling this session chose could
    /// manufacture one that says nothing about the front end.
    /// </remarks>
    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
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
}
