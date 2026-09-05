using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The claims about the front end that no artifact can carry.
/// </summary>
/// <remarks>
/// <para>
/// <b>They live here because a refused source has no bytes.</b> The retained corpus is a set of
/// artifacts and a replay compares what a runtime answered about each; a source the front end
/// refused produced no artifact, so there is nothing for a replay to read and the execution-only
/// image - which carries no front end at all - could not judge one if there were. Every claim
/// below is about source, so every claim below belongs to the composition whose closure carries a
/// compiler.
/// </para>
/// <para>
/// <b>What is NOT here is anything about execution.</b> The compiled programs go into the retained
/// corpus and are judged by the execution-only root's replay, unchanged - which keeps the rule
/// that the producer writes bytes and expectations and judges neither.
/// </para>
/// </remarks>
internal static class SourceFrontEndChecks
{
    internal static (string Name, bool Passed, string Detail)[] Run() =>
    [
        EverySourceTheManifestAdmitsCompiles(),
        EverySourceTheManifestExcludesIsRefusedByName(),
        EveryModuleEarlyErrorIsRefusedByName(),
        TheLoweringIsDeterministic(),
        TwoGoalsInOneProcessDoNotReachEachOther(),
        DeepNestingIsRefusedRatherThanSurvived(),
        AnEarlyErrorNeverProducesBytes(),
        TheSourceFuzzGuidanceLoopIsWired(),
    ];

    /// <summary>
    /// The source session's guidance loop keeps a new answer and refuses a repeat.
    /// </summary>
    /// <remarks>
    /// <b>Here as well as at the start of every session, so a publish that never fuzzes still
    /// carries it.</b> Whether a session's seed set grows is a fact about the corpus it started
    /// from as much as about the mutator, so growth cannot be what fails; the mechanism can, and
    /// this is where a publish asserts it.
    /// </remarks>
    private static (string, bool, string) TheSourceFuzzGuidanceLoopIsWired()
    {
        var (wired, detail) = SourceFuzzing.GuidanceLoopIsWired();
        return ("the source fuzz guidance loop keeps a new answer and refuses a repeat", wired, detail);
    }

    /// <summary>
    /// Every module-goal early error is refused by the wide front end, with its own code.
    /// </summary>
    /// <remarks>
    /// A separate check from the one below rather than an extension of it, because the compiler
    /// differs: these sources are put to the wide front end, which is the only one with a module
    /// goal, and the two lists would otherwise be one list whose rows go to two compilers.
    /// </remarks>
    private static (string, bool, string) EveryModuleEarlyErrorIsRefusedByName()
    {
        var failed = new List<string>();

        foreach (var program in SliceSourcePrograms.RefusedModules)
        {
            var compiled = program.Options.Goal == SliceGoal.Module
                ? JsCompiler.Compile(
                    [],
                    [new JsModuleUnit("retained", program.Source, program.Options)])
                : JsCompiler.Compile(program.Source, program.Options);

            if (compiled.Succeeded)
            {
                failed.Add($"{program.Name}: compiled, and should not have");
                continue;
            }

            if (!compiled.Diagnostics.Any(diagnostic => diagnostic.Code == program.Code))
            {
                failed.Add(
                    $"{program.Name}: expected {program.Code} and got " +
                    string.Join(", ", compiled.Diagnostics.Select(diagnostic => diagnostic.Code)));
            }
        }

        return (
            "every module-goal early error is refused with its own code",
            failed.Count == 0,
            failed.Count == 0
                ? $"{SliceSourcePrograms.RefusedModules.Length} sources, each refused by name"
                : string.Join("; ", failed));
    }

    /// <summary>Every accepted source compiles to an artifact.</summary>
    /// <remarks>
    /// It asserts compilation and not the answer. What each program runs to is recorded in the
    /// corpus manifest and judged by the replay, which is a different image on a different run -
    /// a producer that checked its own output's answers would be the verifier-with-a-schedule this
    /// component keeps refusing to build.
    /// </remarks>
    private static (string, bool, string) EverySourceTheManifestAdmitsCompiles()
    {
        var failed = new List<string>();

        foreach (var program in SliceSourcePrograms.Accepted)
        {
            var compiled = SliceSourceCompiler.Compile(program.Source);

            if (!compiled.Succeeded)
            {
                failed.Add(
                    $"{program.Name}: {(compiled.Diagnostics.Count > 0 ? compiled.Diagnostics[0].ToString() : "no artifact and no diagnostic")}");
            }
        }

        return (
            "every source the manifest admits compiles",
            failed.Count == 0,
            failed.Count == 0
                ? $"{SliceSourcePrograms.Accepted.Length} sources compiled"
                : string.Join("; ", failed));
    }

    /// <summary>
    /// Every refused source is refused, <b>with the code recorded beside it</b>.
    /// </summary>
    /// <remarks>
    /// The code and not merely the refusal. A front end that answered every bad program with one
    /// code would pass a check that only asked whether it refused, and would tell a caller
    /// nothing about which of two dozen different things went wrong.
    /// </remarks>
    private static (string, bool, string) EverySourceTheManifestExcludesIsRefusedByName()
    {
        var failed = new List<string>();

        foreach (var program in SliceSourcePrograms.Refused)
        {
            var compiled = SliceSourceCompiler.Compile(program.Source, program.Options);

            if (compiled.Succeeded)
            {
                failed.Add($"{program.Name}: compiled, and should not have");
                continue;
            }

            var codes = compiled.Diagnostics.Select(d => d.Code).ToArray();

            if (Array.IndexOf(codes, program.Code) < 0)
            {
                failed.Add(
                    $"{program.Name}: wanted {program.Code}, got " +
                    string.Join(",", codes.Select(c => c.ToString())));
            }
        }

        return (
            "every source the manifest excludes is refused by name",
            failed.Count == 0,
            failed.Count == 0
                ? $"{SliceSourcePrograms.Refused.Length} sources refused with the recorded code"
                : string.Join("; ", failed));
    }

    /// <summary>The same source, twice, is the same bytes.</summary>
    /// <remarks>
    /// Two fresh compilations rather than one compilation compared with itself, because the
    /// failure this catches is a compiler that carries state between runs - an interning table
    /// keyed by a hash seed, an iteration over a dictionary, a slot allocated from a static
    /// counter. A single compilation could not exhibit any of them.
    /// </remarks>
    private static (string, bool, string) TheLoweringIsDeterministic()
    {
        var differed = new List<string>();

        foreach (var program in SliceSourcePrograms.Accepted)
        {
            var first = SliceSourceCompiler.Compile(program.Source);
            var second = SliceSourceCompiler.Compile(program.Source);

            if (first.Artifact is null || second.Artifact is null ||
                !first.Artifact.AsSpan().SequenceEqual(second.Artifact))
            {
                differed.Add(program.Name);
            }
        }

        return (
            "the lowering is deterministic",
            differed.Count == 0,
            differed.Count == 0
                ? $"{SliceSourcePrograms.Accepted.Length} sources compiled twice to identical bytes"
                : "differed: " + string.Join(", ", differed));
    }

    /// <summary>
    /// Two parses with different goals run concurrently in one process, each goal-appropriate.
    /// </summary>
    /// <remarks>
    /// <b>This is roadmap section 9's parse-options gate and it fails when the options are
    /// replaced by a shared static.</b> The source is one a script accepts and a module refuses: a
    /// legacy octal literal is legal in sloppy code and is an early error in strict code, and a
    /// module is strict whatever its prologue says. Run the two goals on many threads at once and
    /// a front end that kept the goal anywhere but on the call stack answers one of them with the
    /// other's rule.
    /// </remarks>
    private static (string, bool, string) TwoGoalsInOneProcessDoNotReachEachOther()
    {
        const string LegacyOctal = "0123";
        const int Rounds = 200;

        var wrong = 0;

        Parallel.For(0, Rounds, round =>
        {
            var asScript = SliceSourceCompiler.Compile(LegacyOctal, SliceParseOptions.Script);
            var asModule = SliceSourceCompiler.Compile(LegacyOctal, SliceParseOptions.Module);

            // The script accepts it; the module refuses it as a legacy octal in strict code. A
            // shared static would make one of the two answer the other's way, on some rounds and
            // not others, which is why this runs many rounds rather than one.
            if (!asScript.Succeeded || asScript.IsStrict)
            {
                Interlocked.Increment(ref wrong);
            }

            if (asModule.Succeeded || !asModule.IsStrict ||
                asModule.Diagnostics.All(d => d.Code != SliceSourceDiagnosticCode.LegacyOctalInStrictCode))
            {
                Interlocked.Increment(ref wrong);
            }

            _ = round;
        });

        return (
            "two goals in one process do not reach each other",
            wrong == 0,
            wrong == 0
                ? $"{Rounds} concurrent script-and-module pairs, each answered by its own goal"
                : $"{wrong} of {Rounds * 2} parses answered under the other goal's rule");
    }

    /// <summary>
    /// A source nested past the bound is refused, and a source nested to the bound is not.
    /// </summary>
    /// <remarks>
    /// <b>Both halves, because only the pair says the bound is a bound.</b> A front end that
    /// refused everything would pass the first half; one that refused nothing would pass the
    /// second. The over-deep case is far past any plausible stack, so a run that reaches the
    /// assertion at all is a run in which the bound - and not the operating system - answered.
    /// </remarks>
    private static (string, bool, string) DeepNestingIsRefusedRatherThanSurvived()
    {
        var options = new SliceParseOptions(SliceGoal.Script, allowTopLevelAwait: false, maximumNestingDepth: 64);

        var atTheBound = SliceSourceCompiler.Compile(SliceSourcePrograms.Nested(20), options);
        var pastTheBound = SliceSourceCompiler.Compile(SliceSourcePrograms.Nested(100_000), options);

        var refusedForDepth =
            !pastTheBound.Succeeded &&
            pastTheBound.Diagnostics.Any(d => d.Code == SliceSourceDiagnosticCode.NestingTooDeep);

        return (
            "deep nesting is refused rather than survived",
            atTheBound.Succeeded && refusedForDepth,
            atTheBound.Succeeded
                ? refusedForDepth
                    ? "20 levels compiled; 100,000 levels refused as NestingTooDeep, with no stack overflow"
                    : "100,000 levels did not produce NestingTooDeep"
                : "20 levels, inside the bound, did not compile");
    }

    /// <summary>
    /// <b>The boundary itself: a source with an early error produces no bytes at all.</b>
    /// </summary>
    /// <remarks>
    /// This is the check that roadmap section 9's open question has been answered rather than
    /// left to convention. The verifier does not re-derive early errors from artifact bytes,
    /// because there are no artifact bytes to re-derive them from: a refused compilation carries a
    /// null artifact, and a result that carried both would be a front end emitting something it
    /// had already refused. Fusing the phases - lowering the bad program and letting the verifier
    /// catch it - would make this check fail.
    /// </remarks>
    private static (string, bool, string) AnEarlyErrorNeverProducesBytes()
    {
        var leaked = new List<string>();

        foreach (var program in SliceSourcePrograms.Refused)
        {
            var compiled = SliceSourceCompiler.Compile(program.Source, program.Options);

            if (compiled.Artifact is not null && compiled.Diagnostics.Count > 0)
            {
                leaked.Add(program.Name);
            }
        }

        return (
            "an early error never produces bytes",
            leaked.Count == 0,
            leaked.Count == 0
                ? $"{SliceSourcePrograms.Refused.Length} refusals, none carrying an artifact"
                : "carried both an artifact and a diagnostic: " + string.Join(", ", leaked));
    }
}
