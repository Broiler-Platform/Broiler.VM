// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>What running one variant of one test decided.</summary>
internal enum Test262Verdict
{
    /// <summary>The test ran and did what it declared it would.</summary>
    Passed,

    /// <summary>The test ran and did not.</summary>
    Failed,

    /// <summary>
    /// The test names something this feature manifest does not admit, so it was not run.
    /// </summary>
    /// <remarks>
    /// <b>This is the verdict that keeps a score honest.</b> A front end that refuses a class
    /// declaration has not found a syntax error - it has found a construct it does not implement,
    /// and counting that as a pass on a test that expects a SyntaxError would turn every
    /// unimplemented feature into a point. Unsupported is neither a pass nor a failure and is
    /// reported separately, with the family the front end named carried beside it.
    /// </remarks>
    Unsupported,

    /// <summary>The variant spent an allowance this run set, without answering.</summary>
    /// <remarks>
    /// <b>Its own verdict, because "we did not wait long enough" is not a failure and not a
    /// skip.</b> A ceiling is a property of the run rather than of the engine: the same test under a
    /// larger allowance may answer, and a run whose failed column silently carried its exhaustions
    /// would be a run whose failures nobody can act on. It is not a skip either - a skipped variant
    /// was never started, and this one was started and did not finish - so a reader can tell "the
    /// ceiling was too low" from "this engine loops" only if the two are counted apart. The
    /// dimension the allowance ran out on is carried on the outcome, because that is the half of the
    /// answer a reader acts on.
    /// </remarks>
    Exhausted,

    /// <summary>The runner declined the test: a flag or a phase it does not implement.</summary>
    Skipped,
}

/// <summary>One variant's outcome.</summary>
/// <param name="Path">The suite-relative path of the test.</param>
/// <param name="Variant">Which variant this is: <c>strict</c>, <c>sloppy</c> or <c>raw</c>.</param>
/// <param name="Verdict">What was decided.</param>
/// <param name="Detail">One line a reader can act on.</param>
/// <param name="Family">
/// The construct family an <see cref="Test262Verdict.Unsupported"/> verdict names, empty otherwise.
/// </param>
/// <param name="Dimension">
/// The budget dimension an <see cref="Test262Verdict.Exhausted"/> verdict spent, empty otherwise.
/// </param>
/// <remarks>
/// <b>The family and the dimension are fields rather than prose inside the detail.</b> Both are
/// aggregated into a table at the end of a run and summed across shards by the merge; recovering
/// them by parsing a sentence would make the table a property of how a message happens to be
/// punctuated.
/// </remarks>
internal sealed record Test262Outcome(
    string Path,
    string Variant,
    Test262Verdict Verdict,
    string Detail,
    string Family = "",
    string Dimension = "");

/// <summary>
/// Runs a real test262 checkout under the feature manifest the run names.
/// </summary>
/// <remarks>
/// <para>
/// <b>The harness files are separate scripts and the realm is shared.</b> INTERPRETING.md requires
/// exactly that, and concatenation is a real defect and not a shortcut: it changes <c>this</c>
/// inside <c>Test262Error</c>, changes what <c>delete</c> does in <c>propertyHelper.js</c>, and
/// changes the directive-prologue semantics that a whole family of tests is about. The wide
/// surface's artifact carries one code unit per script and the instance is the realm, so this
/// runner gets the required shape from the format rather than from a convention it has to keep.
/// </para>
/// <para>
/// <b>A fresh realm per variant.</b> Tests destroy <c>assert</c>, redefine <c>Object.prototype</c>
/// members and freeze intrinsics; a runner that reused a realm would score the next test against
/// the previous one's wreckage. A realm here is an instance, so a fresh one costs a runtime and
/// nothing more.
/// </para>
/// <para>
/// <b>Which manifest is an input and not a constant.</b> JSW-10 asks for a whole-suite run per
/// manifest, so the manifest decides three things at once: which front end lowers the source, which
/// format version the artifact declares, and whether the suite's harness files are loaded at all.
/// <c>broiler.javascript.slice</c> admits no call, so its front end refuses <c>assert.js</c> before
/// it could be installed - which is why loading the harness is the manifest's property rather than
/// this runner's habit.
/// </para>
/// <para>
/// <b>What it does not implement, it says.</b> Module tests, <c>resolution</c>-phase negatives and
/// the <c>CanBlockIsFalse</c> flag are skipped by name rather than run and scored.
/// </para>
/// </remarks>
internal static class Test262Run
{
    /// <summary>The identity this runner presents to the verifier.</summary>
    private const string Caller = "broiler-js-conformance://test262";

    /// <summary>What a passing asynchronous test prints.</summary>
    private const string AsyncComplete = "Test262:AsyncTestComplete";

    /// <summary>What a failing asynchronous test prints.</summary>
    private const string AsyncFailure = "Test262:AsyncTestFailure:";

    /// <summary>Runs one test file, in every variant its flags call for.</summary>
    internal static IReadOnlyList<Test262Outcome> RunOne(
        string suiteRoot,
        string relativePath,
        Test262Manifest manifest,
        ulong fuel,
        ulong wallClock)
    {
        var full = Path.Combine(suiteRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(full))
        {
            return [new Test262Outcome(relativePath, "-", Test262Verdict.Skipped, "no such file")];
        }

        if (relativePath.Contains("_FIXTURE", StringComparison.Ordinal))
        {
            return
            [
                new Test262Outcome(
                    relativePath, "-", Test262Verdict.Skipped,
                    "a fixture is loaded by another test and never run on its own"),
            ];
        }

        var text = ReadUtf8(full);

        if (!Test262Metadata.TryRead(relativePath, text, out var frontmatter, out var failure))
        {
            return [new Test262Outcome(relativePath, "-", Test262Verdict.Skipped, failure)];
        }

        var flags = new HashSet<string>(frontmatter.Flags, StringComparer.Ordinal);

        if (flags.Contains("module"))
        {
            return
            [
                new Test262Outcome(
                    relativePath, "-", Test262Verdict.Skipped,
                    "this manifest admits no module goal"),
            ];
        }

        if (flags.Contains("CanBlockIsFalse"))
        {
            return
            [
                new Test262Outcome(
                    relativePath, "-", Test262Verdict.Skipped,
                    "this agent's [[CanBlock]] is true"),
            ];
        }

        if (frontmatter.Negative is { } negative &&
            string.Equals(negative.Phase, "resolution", StringComparison.Ordinal))
        {
            return
            [
                new Test262Outcome(
                    relativePath, "-", Test262Verdict.Skipped,
                    "a resolution-phase negative needs modules"),
            ];
        }

        var variants = new List<string>(2);

        if (flags.Contains("raw"))
        {
            variants.Add("raw");
        }
        else if (flags.Contains("onlyStrict"))
        {
            variants.Add("strict");
        }
        else if (flags.Contains("noStrict"))
        {
            variants.Add("sloppy");
        }
        else
        {
            variants.Add("strict");
            variants.Add("sloppy");
        }

        var outcomes = new List<Test262Outcome>(variants.Count);

        foreach (var variant in variants)
        {
            outcomes.Add(
                RunVariant(
                    suiteRoot, relativePath, text, frontmatter, flags, variant, manifest, fuel,
                    wallClock));
        }

        return outcomes;
    }

    private static Test262Outcome RunVariant(
        string suiteRoot,
        string relativePath,
        string text,
        Test262Frontmatter frontmatter,
        HashSet<string> flags,
        string variant,
        Test262Manifest manifest,
        ulong fuel,
        ulong wallClock)
    {
        var scripts = new List<JsScriptUnit>();
        var options = SliceParseOptions.Script;

        if (manifest.LoadsHarness && !string.Equals(variant, "raw", StringComparison.Ordinal))
        {
            if (!TryHarness(suiteRoot, "assert.js", scripts, options, out var why) ||
                !TryHarness(suiteRoot, "sta.js", scripts, options, out why))
            {
                return new Test262Outcome(relativePath, variant, Test262Verdict.Skipped, why);
            }

            if (flags.Contains("async") &&
                !TryHarness(suiteRoot, "doneprintHandle.js", scripts, options, out why))
            {
                return new Test262Outcome(relativePath, variant, Test262Verdict.Skipped, why);
            }

            foreach (var include in frontmatter.Includes)
            {
                if (!TryHarness(suiteRoot, include, scripts, options, out why))
                {
                    return new Test262Outcome(relativePath, variant, Test262Verdict.Skipped, why);
                }
            }
        }

        var strict = string.Equals(variant, "strict", StringComparison.Ordinal);
        var negative = frontmatter.Negative;

        byte[] artifact;

        if (manifest.IsWide)
        {
            scripts.Add(new JsScriptUnit("test", text, options, strict));
            var compiled = JsCompiler.Compile(scripts);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return Refused(relativePath, variant, compiled.Diagnostics, negative);
            }

            artifact = compiled.Artifact;
        }
        else
        {
            // THE NARROW FRONT END TAKES ONE SOURCE AND NO HARNESS, and the strict reading is the
            // suite's own rule applied here rather than a flag this compiler has. `--run`'s ingested
            // dialect prepends the same prologue for the same reason: a file that declares neither
            // strictness is DEFINED to be run twice, so prepending is part of what the file means.
            var source = strict ? Test262Adapter.StrictPrologue + text : text;
            var compiled = SliceSourceCompiler.Compile(source, options);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return Refused(relativePath, variant, compiled.Diagnostics, negative);
            }

            scripts.Clear();
            scripts.Add(new JsScriptUnit("main", source, options, strict));
            artifact = compiled.Artifact;
        }

        var printed = new List<string>();
        var created = VmRuntime.Create(manifest.Catalog, Options(fuel, wallClock, printed));

        if (created.Outcome == VmOutcome.ResourceExhaustion)
        {
            return Spent(relativePath, variant, created.Diagnostics, "creating the runtime");
        }

        if (!created.TryGetRuntime(out var runtime))
        {
            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Skipped,
                $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            return Judge(
                runtime, artifact, scripts, relativePath, variant, negative, flags, printed, manifest);
        }
    }

    /// <summary>What a refusal of the source is, which is the question the four verdicts turn on.</summary>
    /// <remarks>
    /// <b>A construct outside the manifest is unsupported and carries the family the front end
    /// named; every other refusal is an answer about the language.</b> That is the whole of the
    /// honesty rule roadmap section 14 states: this profile refuses valid JavaScript on lines a
    /// negative test also expects a refusal on, and scoring the first as though it were the second
    /// would turn every unimplemented construct into a point.
    /// </remarks>
    private static Test262Outcome Refused(
        string relativePath,
        string variant,
        IReadOnlyList<SliceSourceDiagnostic> diagnostics,
        Test262Negative? negative)
    {
        var first = diagnostics.Count == 0 ? null : diagnostics[0];

        if (first is not null && first.Code == SliceSourceDiagnosticCode.ConstructOutsideManifest)
        {
            return new Test262Outcome(
                relativePath,
                variant,
                Test262Verdict.Unsupported,
                first.ToString(),
                Test262Families.Of(first.Message));
        }

        if (negative is not null &&
            string.Equals(negative.Phase, "parse", StringComparison.Ordinal) &&
            string.Equals(negative.Type, "SyntaxError", StringComparison.Ordinal))
        {
            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Passed,
                "refused at parse: " + (first is null ? "no diagnostic" : first.ToString()));
        }

        return new Test262Outcome(
            relativePath, variant, Test262Verdict.Failed,
            "the front end refused the source: " + (first is null ? "no diagnostic" : first.ToString()));
    }

    /// <summary>One variant that ran out of an allowance, with the dimension it ran out of.</summary>
    private static Test262Outcome Spent(
        string relativePath, string variant, VmDiagnostics diagnostics, string where) =>
        new(
            relativePath,
            variant,
            Test262Verdict.Exhausted,
            "the allowance was spent " + where,
            Family: string.Empty,
            Dimension: diagnostics.ExhaustedDimension.ToString());

    private static Test262Outcome Judge(
        VmRuntime runtime,
        byte[] artifact,
        List<JsScriptUnit> scripts,
        string relativePath,
        string variant,
        Test262Negative? negative,
        HashSet<string> flags,
        List<string> printed,
        Test262Manifest manifest)
    {
        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            manifest.FormatVersion,
            manifest.Id,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(in descriptor, artifact, CancellationToken.None);

        if (verified.Outcome == VmOutcome.ResourceExhaustion)
        {
            return Spent(relativePath, variant, verified.Diagnostics, "verifying the artifact");
        }

        if (!verified.TryGetArtifact(out var handle))
        {
            // A COMPOSITION DECLINING A SURFACE IS AN ABSENCE AND NOT A DEFECT, and this is where
            // the two are told apart. Roadmap section 6 puts that refusal at verification, with an
            // invalid-artifact reason, precisely so it is distinguishable from a run-time refusal -
            // and a run that counted it as a failure would report `--decline` as an engine that had
            // broken rather than as the composition doing the job the identity exists for.
            if (verified.Diagnostics.ProfileDiagnosticCode ==
                (int)JavaScriptDiagnosticCode.SurfaceOutsideComposition)
            {
                return new Test262Outcome(
                    relativePath,
                    variant,
                    Test262Verdict.Unsupported,
                    $"the verifier refused an artifact declaring a declined surface: " +
                        $"{verified.Outcome}/{verified.Reason}",
                    manifest.DeclinedFamily);
            }

            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Failed,
                $"the verifier refused an artifact this runner produced: {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(handle, CancellationToken.None);

        if (instantiated.Outcome == VmOutcome.ResourceExhaustion)
        {
            return Spent(relativePath, variant, instantiated.Diagnostics, "instantiating the artifact");
        }

        if (!instantiated.TryGetInstance(out var instance))
        {
            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Failed,
                $"the artifact would not instantiate: {instantiated.Outcome}/{instantiated.Reason}");
        }

        for (var index = 0; index < scripts.Count; index++)
        {
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(scripts[index].Name)));

            var result = instance.Invoke(in request, CancellationToken.None);
            var isTest = index == scripts.Count - 1;

            if (result.Outcome == VmOutcome.ResourceExhaustion)
            {
                return Spent(
                    relativePath,
                    variant,
                    result.Diagnostics,
                    isTest ? "running the test" : "running a harness file");
            }

            if (TryUncaught(in result, manifest, out var errorName, out var message))
            {
                if (!isTest)
                {
                    return new Test262Outcome(
                        relativePath, variant, Test262Verdict.Failed,
                        "a harness file threw: " + message);
                }

                if (negative is not null &&
                    string.Equals(negative.Phase, "runtime", StringComparison.Ordinal))
                {
                    return string.Equals(negative.Type, errorName, StringComparison.Ordinal)
                        ? new Test262Outcome(
                            relativePath, variant, Test262Verdict.Passed, "threw " + errorName)
                        : new Test262Outcome(
                            relativePath, variant, Test262Verdict.Failed,
                            "expected " + negative.Type + " and got " + message);
                }

                if (negative is not null)
                {
                    return new Test262Outcome(
                        relativePath, variant, Test262Verdict.Failed,
                        "expected a " + negative.Phase + "-phase " + negative.Type +
                            " and it threw at run time: " + message);
                }

                return new Test262Outcome(
                    relativePath, variant, Test262Verdict.Failed, "uncaught " + message);
            }

            if (!Completed(in result, manifest))
            {
                return new Test262Outcome(
                    relativePath, variant, Test262Verdict.Failed,
                    $"the invocation answered {result.Outcome}/{result.Reason} and carried no payload");
            }
        }

        if (negative is not null)
        {
            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Failed,
                "expected a " + negative.Phase + "-phase " + negative.Type + " and nothing was thrown");
        }

        if (flags.Contains("async"))
        {
            foreach (var line in printed)
            {
                if (string.Equals(line, AsyncComplete, StringComparison.Ordinal))
                {
                    return new Test262Outcome(
                        relativePath, variant, Test262Verdict.Passed, AsyncComplete);
                }

                if (line.StartsWith(AsyncFailure, StringComparison.Ordinal))
                {
                    return new Test262Outcome(relativePath, variant, Test262Verdict.Failed, line);
                }
            }

            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Failed,
                "an asynchronous test printed no completion, and this profile has no job queue");
        }

        return new Test262Outcome(relativePath, variant, Test262Verdict.Passed, string.Empty);
    }

    /// <summary>The uncaught exception a result carries, in whichever manifest's payload shape.</summary>
    /// <remarks>
    /// <b>Two payload kinds because there are two manifests, and the projection is the profile's
    /// own.</b> A wide-surface fault carries a JavaScript error name; a slice fault carries one of
    /// the profile's fault kinds. Asking for the wrong one would answer "no payload" and report a
    /// thrown test as an invocation that carried nothing.
    /// </remarks>
    private static bool TryUncaught(
        in VmInvocationResult result,
        Test262Manifest manifest,
        out string errorName,
        out string message)
    {
        if (manifest.IsWide)
        {
            if (JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
            {
                errorName = uncaught.ErrorName;
                message = uncaught.Message;
                return true;
            }
        }
        else if (JavaScriptProfile.TryGetFault(in result, out var fault))
        {
            errorName = fault.Kind.ToString();
            message = fault.Kind + ": " + fault.Message;
            return true;
        }

        errorName = string.Empty;
        message = string.Empty;
        return false;
    }

    /// <summary>Whether the invocation produced this manifest's completion payload.</summary>
    private static bool Completed(in VmInvocationResult result, Test262Manifest manifest) =>
        manifest.IsWide
            ? JavaScriptProfile.TryGetWideCompletion(in result, out _)
            : JavaScriptProfile.TryGetCompletion(in result, out _);

    private static bool TryHarness(
        string suiteRoot,
        string name,
        List<JsScriptUnit> scripts,
        SliceParseOptions options,
        out string failure)
    {
        var path = Path.Combine(
            suiteRoot, "harness", name.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(path))
        {
            failure = "no harness file at harness/" + name;
            return false;
        }

        scripts.Add(
            new JsScriptUnit(
                "harness" + scripts.Count.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ReadUtf8(path),
                options,
                ForceStrict: false));

        failure = string.Empty;
        return true;
    }

    /// <summary>Reads a file as UTF-8, with no byte-order mark and no line-ending translation.</summary>
    private static string ReadUtf8(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
        return System.Text.Encoding.UTF8.GetString(bytes, start, bytes.Length - start);
    }

    private static VmRuntimeCreationOptions Options(
        ulong fuel, ulong wallClock, List<string> printed)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                VmBudgetDimension.WallClock => VmCeilingSpec.Value(dimension, wallClock),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        capabilities.Add(VmCapabilityRegistration.Value(
            JavaScriptProfile.WriteCapability,
            (VmBytes argument, out VmOpaqueRef result) =>
            {
                result = default;
                printed.Add(System.Text.Encoding.UTF8.GetString(argument.Span));
                return VmHostCallOutcome.Completed;
            }));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }
}
