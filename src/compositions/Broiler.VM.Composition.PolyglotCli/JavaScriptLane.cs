// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>
/// The <c>broiler.javascript</c> lane: several scripts, one realm, one artifact.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS LANE IS COPIED FROM
/// <c>Broiler.VM.Composition.JavaScript.Cli/WideHost.cs</c>, and it is copied rather than
/// referenced for the reason rules A11 and A12 impose.</b> A library holding it would be a project
/// outside <c>src/compositions/</c> naming a profile assembly, which A11 forbids; a composition
/// root referencing another composition root is not the three-core-plus-profiles reference set A12
/// admits. So a second end-user host of the same profile duplicates the loop, and saying so here
/// is what keeps a reader from taking a duplicate for an independent second opinion: it is the
/// same loop, and where the two diverge the divergence is a defect in one of them.
/// </para>
/// <para>
/// <b>What is NOT copied is the catalog and the ceilings.</b> Those come from
/// <see cref="Composition"/>, which composes two profiles into one catalog, and they are the whole
/// difference between this root and the one this file came from.
/// </para>
/// <para>
/// <b>Naming several files runs them as several scripts in ONE realm, in order.</b> That is what a
/// shell does, and it is what both of this repository's target JavaScript workloads need: the
/// Octane harness is <c>base.js</c> plus a benchmark plus a runner, and a conformance test is
/// <c>assert.js</c> plus <c>sta.js</c> plus the test. Concatenating them would be a different
/// program - it would change <c>this</c> inside a constructor and change what a directive prologue
/// means - so the artifact carries one code unit and one entry point per file and this lane
/// invokes them in order against one instance.
/// </para>
/// </remarks>
internal static class JavaScriptLane
{
    /// <summary>The identity this lane presents to the verifier.</summary>
    private const string Caller = "broiler-cli://javascript";

    /// <summary>Compiles, verifies and runs a list of scripts in one realm.</summary>
    internal static RunResult Run(
        IReadOnlyList<InputFile> files,
        bool module,
        bool checkOnly,
        bool forceStrict,
        int? maximumDepth,
        Composition.Allowances allowances,
        JsCompileRequest request)
    {
        foreach (var file in files)
        {
            if (file.Problem.Length != 0)
            {
                return new RunResult(file.Status, string.Empty, file.Problem, []);
            }
        }

        var goal = module ? SliceGoal.Module : SliceGoal.Script;

        var options = maximumDepth is { } depth
            ? new SliceParseOptions(goal, allowTopLevelAwait: false, depth)
            : module ? SliceParseOptions.Module : SliceParseOptions.Script;

        var scripts = new List<JsScriptUnit>(files.Count);
        var modules = new List<JsModuleUnit>();

        for (var index = 0; index < files.Count; index++)
        {
            // THE LAST FILE IS THE ONE THAT MAY BE A MODULE, and the ones before it are its realm.
            // A module in the middle of the list would be a graph whose evaluation order the
            // argument order decided, which is not an order anybody could state.
            if (module && index == files.Count - 1)
            {
                var loaded = ModuleGraph.Load(files[index].Path);

                if (loaded.Failure.Length != 0)
                {
                    return new RunResult(RunStatus.Unreadable, string.Empty, loaded.Failure, []);
                }

                modules.AddRange(loaded.Modules);
                continue;
            }

            // THE SCRIPT'S OWN PATH IS ITS REFERRER, which is what makes `import('./m.mjs')` in a
            // script mean the same thing it means in a module beside it.
            scripts.Add(new JsScriptUnit(
                "script" + index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                files[index].Text,
                options,
                forceStrict,
                Path.GetFullPath(files[index].Path).Replace('\\', '/')));
        }

        var compiled = JsCompiler.Compile(scripts, modules, request);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Refused(compiled);
        }

        // THE BACKEND IS PART OF THE CATALOG WHEN A NATIVE FORM WAS ASKED FOR. The emitted bytes
        // are re-emitted by the verifier and compared, which is the only sense in which machine
        // code is verifiable at all; a descriptor without the emitter would check the framing and
        // then be trusting whoever produced the artifact, which in this image is this image.
        //
        // THE CAST IS `as` AND NOT A CAST, AND THE REASON IS THE `emitting-only` LABEL ITSELF.
        // A backend that emits and is never armed does not implement the re-emitting interface at
        // all - `arm64-aapcs64` resolves to an encoder and to nothing a verifier can call back
        // into - so the emitting-only property is in the type system rather than only in the
        // support table. Writing `(IJsNativeEmitter)found` here threw an InvalidCastException the
        // first time this host was pointed at that backend, which reported a real property of the
        // build under this component's own defect code. The null answer is the correct one: the
        // ordinary descriptor admits the artifact's framing, and the image then declines to arm it.
        var emitter = request.Form == JsOutputForm.Native &&
            JsNativeBackends.TryFind(request.Backend, out var backend)
                ? backend as Broiler.VM.Profile.JavaScript.Format.IJsNativeEmitter
                : null;

        var created = VmRuntime.Create(
            Composition.Catalog(emitter),
            Composition.Options(allowances, javascript: true, new SourceProvider()));

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
            return Run(runtime, compiled.Artifact, scripts.Count, modules.Count != 0, checkOnly, request.Manifest);
        }
    }

    /// <summary>Compiles a list of scripts and hands back the artifact bytes, or why not.</summary>
    /// <remarks>
    /// <b>This is what <c>broiler compile</c> is, and it stops one step earlier than
    /// the run loop.</b> The artifact is not verified here: verification is charged against a
    /// runtime's allowance and belongs to the run, and a host that verified at compile time would
    /// report a resource exhaustion for an artifact it was only asked to write to a file.
    /// </remarks>
    internal static RunResult Compile(
        IReadOnlyList<InputFile> files,
        bool module,
        bool forceStrict,
        int? maximumDepth,
        JsCompileRequest request,
        out byte[] artifact)
    {
        artifact = [];

        foreach (var file in files)
        {
            if (file.Problem.Length != 0)
            {
                return new RunResult(file.Status, string.Empty, file.Problem, []);
            }
        }

        var goal = module ? SliceGoal.Module : SliceGoal.Script;

        var options = maximumDepth is { } depth
            ? new SliceParseOptions(goal, allowTopLevelAwait: false, depth)
            : module ? SliceParseOptions.Module : SliceParseOptions.Script;

        var scripts = new List<JsScriptUnit>(files.Count);
        var modules = new List<JsModuleUnit>();

        for (var index = 0; index < files.Count; index++)
        {
            if (module && index == files.Count - 1)
            {
                var loaded = ModuleGraph.Load(files[index].Path);

                if (loaded.Failure.Length != 0)
                {
                    return new RunResult(RunStatus.Unreadable, string.Empty, loaded.Failure, []);
                }

                modules.AddRange(loaded.Modules);
                continue;
            }

            scripts.Add(new JsScriptUnit(
                "script" + index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                files[index].Text,
                options,
                forceStrict,
                Path.GetFullPath(files[index].Path).Replace('\\', '/')));
        }

        var compiled = JsCompiler.Compile(scripts, modules, request);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Refused(compiled);
        }

        artifact = compiled.Artifact;
        return new RunResult(RunStatus.Completed, string.Empty, string.Empty, []);
    }

    private static RunResult Refused(JsCompilation compiled)
    {
        var lines = new string[compiled.Diagnostics.Count];

        for (var index = 0; index < lines.Length; index++)
        {
            lines[index] = compiled.Diagnostics[index].ToString();
        }

        return new RunResult(
            RunStatus.RefusedSource,
            string.Empty,
            lines.Length == 0
                ? "the front end refused the source and named no diagnostic, which is a defect here"
                : lines[0],
            lines);
    }

    private static RunResult Run(
        VmRuntime runtime,
        byte[] artifact,
        int scripts,
        bool hasModules,
        bool checkOnly,
        JsFeatureManifest manifest)
    {
        var count = scripts + (hasModules ? 1 : 0);

        // THE DESCRIPTOR NAMES THE MANIFEST THE ARTIFACT NAMES, and the verifier compares the two
        // rather than trusting either. A host that always said `wide` would be mislabelling a
        // numeric artifact, which the verifier answers as the caller's mistake - correctly.
        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            manifest == JsFeatureManifest.Numeric
                ? JavaScriptProfile.NumericManifest
                : JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(in descriptor, artifact, CancellationToken.None);

        if (!verified.TryGetArtifact(out var verifiedArtifact))
        {
            if (verified.Outcome == VmOutcome.ResourceExhaustion)
            {
                return new RunResult(
                    RunStatus.Exhausted,
                    string.Empty,
                    "verifying the artifact spent the allowance: " +
                    $"{verified.Reason} on {verified.Diagnostics.ExhaustedDimension}/{verified.Diagnostics.ExhaustedScope}",
                    []);
            }

            return new RunResult(
                RunStatus.RefusedArtifact,
                string.Empty,
                "the verifier refused an artifact this host produced: " +
                    $"{verified.Diagnostics.ProfileDiagnosticCode} " +
                    $"({verified.Outcome}/{verified.Reason}) at byte " +
                    verified.Diagnostics.SourcePosition.ByteOffset.ToString(
                        System.Globalization.CultureInfo.InvariantCulture),
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

            // AN EMITTED ARTIFACT THIS MACHINE CANNOT ARM IS NOT A DEFECT IN THIS HOST, AND
            // REPORTING IT AS ONE WOULD BE THE OVERCLAIM THIS COMPONENT TREATS AS A STOP
            // CONDITION. The arming path admits two calling conventions of one architecture and
            // answers `None` for every other process architecture, so an artifact emitted for
            // System V on a Windows process, or for arm64 anywhere, verifies and then refuses to
            // instantiate by name. That is the declared behaviour of an emitting-only form and of
            // a form emitted for somewhere else - the caller asked for a backend this machine does
            // not arm - so it reports under the code that means "you asked for something I do not
            // do" and not under the one that accuses this component's own lowering.
            if (instantiated.Reason == VmReason.UnsatisfiedHostAssumption && manifest == JsFeatureManifest.Numeric)
            {
                return new RunResult(
                    RunStatus.Unroutable,
                    string.Empty,
                    "the artifact verified and this image will not arm it: " +
                    $"({instantiated.Outcome}/{instantiated.Reason}). The arming path answers with " +
                    "the one x86-64 calling convention this process's own platform uses - Windows " +
                    "x64 or System V x64 - and refuses an artifact emitted for the other one or " +
                    "for any other architecture; arm64-aapcs64 is emitting-only everywhere.",
                    []);
            }

            return new RunResult(
                RunStatus.RefusedArtifact,
                string.Empty,
                "the artifact verified and would not instantiate: " +
                    $"({instantiated.Outcome}/{instantiated.Reason})",
                []);
        }

        var value = string.Empty;

        for (var index = 0; index <= count; index++)
        {
            // THE LAST TIME ROUND IS THE JOB QUEUE, AND THIS HOST STATES THAT AS ITS DRAIN POINT.
            // A queue drained at a point nobody stated is a behaviour no embedder can reason about,
            // so the profile never chooses; this host chooses after the last script, which is what
            // a shell does. A MODULE GRAPH IS ONE MORE INVOCATION AND IT COMES BEFORE THE DRAIN.
            var name = index == count
                ? JavaScriptProfile.DrainEntryPoint
                : index == scripts && hasModules
                    ? JsCompiler.ModuleEntry
                    : "script" + index.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(name)));

            var result = instance.Invoke(in request, CancellationToken.None);

            if (result.Outcome == VmOutcome.ResourceExhaustion)
            {
                return new RunResult(
                    RunStatus.Exhausted,
                    string.Empty,
                    "the program did not settle within its allowance: " +
                    $"{result.Reason} on {result.Diagnostics.ExhaustedDimension}",
                    []);
            }

            if (JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
            {
                return new RunResult(RunStatus.Faulted, string.Empty, "uncaught " + uncaught.Message, []);
            }

            if (JavaScriptProfile.TryGetWideCompletion(in result, out var completion))
            {
                // THE DRAIN IS NOT A SCRIPT AND ITS COMPLETION IS NOT THE PROGRAM'S. It always
                // answers `undefined`, and letting that overwrite the last script's value would
                // make every program print `undefined`.
                if (index != count)
                {
                    value = completion.Value;
                }

                continue;
            }

            return new RunResult(
                RunStatus.HostDefect,
                string.Empty,
                $"the invocation answered {result.Outcome}/{result.Reason} and carried no payload",
                []);
        }

        return new RunResult(RunStatus.Completed, value, string.Empty, []);
    }
}
