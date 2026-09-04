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
    /// reported separately.
    /// </remarks>
    Unsupported,

    /// <summary>The runner declined the test: a flag or a phase it does not implement.</summary>
    Skipped,
}

/// <summary>One variant's outcome.</summary>
/// <param name="Path">The suite-relative path of the test.</param>
/// <param name="Variant">Which variant this is: <c>strict</c>, <c>sloppy</c> or <c>raw</c>.</param>
/// <param name="Verdict">What was decided.</param>
/// <param name="Detail">One line a reader can act on.</param>
internal sealed record Test262Outcome(
    string Path, string Variant, Test262Verdict Verdict, string Detail);

/// <summary>
/// Runs a real test262 checkout against <c>broiler.javascript.wide</c>.
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
        string suiteRoot, string relativePath, ulong fuel, ulong wallClock)
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
                RunVariant(suiteRoot, relativePath, text, frontmatter, flags, variant, fuel, wallClock));
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
        ulong fuel,
        ulong wallClock)
    {
        var scripts = new List<JsScriptUnit>();
        var options = SliceParseOptions.Script;

        if (!string.Equals(variant, "raw", StringComparison.Ordinal))
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
        scripts.Add(new JsScriptUnit("test", text, options, strict));

        var compiled = JsCompiler.Compile(scripts);
        var negative = frontmatter.Negative;

        if (!compiled.Succeeded)
        {
            var first = compiled.Diagnostics.Count == 0
                ? null
                : compiled.Diagnostics[0];

            if (first is not null &&
                first.Code == SliceSourceDiagnosticCode.ConstructOutsideManifest)
            {
                return new Test262Outcome(
                    relativePath, variant, Test262Verdict.Unsupported, first.ToString());
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

        var printed = new List<string>();
        var created = VmRuntime.Create(Catalog(), Options(fuel, wallClock, printed));

        if (!created.TryGetRuntime(out var runtime))
        {
            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Skipped,
                $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            return Judge(
                runtime, compiled.Artifact!, scripts, relativePath, variant, negative, flags, printed);
        }
    }

    private static Test262Outcome Judge(
        VmRuntime runtime,
        byte[] artifact,
        List<JsScriptUnit> scripts,
        string relativePath,
        string variant,
        Test262Negative? negative,
        HashSet<string> flags,
        List<string> printed)
    {
        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(in descriptor, artifact, CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            return new Test262Outcome(
                relativePath, variant, Test262Verdict.Failed,
                $"the verifier refused an artifact this runner produced: {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(handle, CancellationToken.None);

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
                return new Test262Outcome(
                    relativePath, variant, Test262Verdict.Skipped,
                    $"the allowance was spent on {result.Diagnostics.ExhaustedDimension}");
            }

            if (JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
            {
                if (!isTest)
                {
                    return new Test262Outcome(
                        relativePath, variant, Test262Verdict.Failed,
                        "a harness file threw: " + uncaught.Message);
                }

                if (negative is not null &&
                    string.Equals(negative.Phase, "runtime", StringComparison.Ordinal))
                {
                    return string.Equals(negative.Type, uncaught.ErrorName, StringComparison.Ordinal)
                        ? new Test262Outcome(
                            relativePath, variant, Test262Verdict.Passed, "threw " + uncaught.ErrorName)
                        : new Test262Outcome(
                            relativePath, variant, Test262Verdict.Failed,
                            "expected " + negative.Type + " and got " + uncaught.Message);
                }

                if (negative is not null)
                {
                    return new Test262Outcome(
                        relativePath, variant, Test262Verdict.Failed,
                        "expected a " + negative.Phase + "-phase " + negative.Type +
                            " and it threw at run time: " + uncaught.Message);
                }

                return new Test262Outcome(
                    relativePath, variant, Test262Verdict.Failed, "uncaught " + uncaught.Message);
            }

            if (!JavaScriptProfile.TryGetWideCompletion(in result, out _))
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

    private static VmCatalog Catalog() => VmCatalog.CreateBuilder()
        .Add(JavaScriptProfile.Descriptor)
        .Build();

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
