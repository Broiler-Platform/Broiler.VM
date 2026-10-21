// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The wide lowering's binder pass (SJL-12.6 item 1, JSD-0080 decision D11): the answers it reads
/// off the tree where the lowering used to read them off its own records.
/// </summary>
/// <remarks>
/// <para>
/// <b>An answer read off the tree agrees with the record only while the record holds what the tree
/// says.</b> The binder decides which block-level functions Annex B admits in a function from the
/// syntax - the parameter list's names, <c>arguments</c> and the body's own lexical names - where
/// the lowering used to take the names already declared in the function's record when it hoisted.
/// The two are the same set because nothing a source can spell is declared in that record before the
/// hoisting but the parameters and <c>arguments</c>: a named function expression's own name lives in a
/// record of its own outside the function, and every other early declaration is a temporary whose
/// name begins with <c>#</c>.
/// </para>
/// <para>
/// <b>These rows are what notices a change that breaks that</b>, by what the programs answer rather
/// than by which instructions they compile to. A block function named after a parameter is declined,
/// so the parameter keeps its value; a block function named after the function expression around it
/// is admitted, so its <c>var</c>-scoped binding shadows the expression's own name and holds
/// <c>undefined</c> until the declaration is evaluated. Moving the expression's name into the
/// function's record - or declaring any other name there before the hoisting - fails the second row,
/// whichever way the binder then answers.
/// </para>
/// </remarks>
internal static class BinderChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://binder";

    /// <summary>Runs every row.</summary>
    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run()
    {
        yield return Language(
            "annex b declines a block function a parameter of its function names",
            "function f(a) { { function a() { return 2; } } return a; } f(1);",
            "1");

        yield return Language(
            "annex b declines a block function a destructured parameter names, wherever it stands",
            "function f({ a }, b = 0) { if (b === 0) { switch (b) { default: function a() {} } } return a; }" +
            " f({ a: 'kept' });",
            "kept");

        yield return Language(
            "annex b admits a block function named after the function expression around it",
            "(function g() { var before = typeof g; { function g() {} } return before + ':' + typeof g; })();",
            "undefined:function");
    }

    /// <summary>One row: compile and run the program, and compare its completion value.</summary>
    private static (string, bool, string) Language(string name, string source, string expected)
    {
        using var runtime = Runtime();

        if (runtime is null)
        {
            return (name, false, "the runtime refused creation");
        }

        if (!TryRun(runtime, source, out var answer))
        {
            return (name, false, answer);
        }

        return (
            name,
            string.Equals(answer, expected, System.StringComparison.Ordinal),
            $"expected `{expected}` and the program answered `{answer}`");
    }

    /// <summary>Compiles, verifies, instantiates and invokes one script, answering its completion or why not.</summary>
    private static bool TryRun(VmRuntime runtime, string source, out string answer)
    {
        var compiled = JsCompiler.Compile([new JsScriptUnit("main", source, SliceParseOptions.Script)]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            answer = "the source was refused: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString());

            return false;
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            answer = $"verification: {verified.Outcome}/{verified.Reason}";
            return false;
        }

        using (handle)
        {
            var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                answer = $"instantiation: {instantiated.Outcome}/{instantiated.Reason}";
                return false;
            }

            using (instance)
            {
                var request = new VmInvocationRequest(
                    new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

                var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);

                if (invoked.Outcome is not VmOutcome.Normal ||
                    !JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
                {
                    answer = $"the run answered {invoked.Outcome}/{invoked.Reason}";
                    return false;
                }

                answer = completion.Value;
                return true;
            }
        }
    }

    /// <summary>A runtime over this profile with no capability registered.</summary>
    private static VmRuntime? Runtime()
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
                capabilities: []));

        return created.TryGetRuntime(out var runtime) ? runtime : null;
    }
}
