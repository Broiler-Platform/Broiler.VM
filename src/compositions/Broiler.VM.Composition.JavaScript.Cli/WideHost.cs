// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// The host for <c>broiler.javascript.wide</c>: several scripts, one realm, one artifact.
/// </summary>
/// <remarks>
/// <para>
/// <b>Naming several files runs them as several scripts in ONE realm, in order.</b> That is what a
/// shell does, and it is what both of this milestone's target workloads need: the Octane harness is
/// <c>base.js</c> plus a benchmark plus a runner, and a conformance test is
/// <c>assert.js</c> plus <c>sta.js</c> plus the test. Concatenating them would be a different
/// program - it would change <c>this</c> inside a constructor and change what a directive prologue
/// means - so the artifact carries one code unit and one entry point per file and this host invokes
/// them in order against one instance.
/// </para>
/// <para>
/// <b>The first script that throws stops the run.</b> A shell does the same; continuing would run
/// the rest of a program whose setup failed and report whatever came out.
/// </para>
/// </remarks>
internal static class WideHost
{
    /// <summary>The identity this host presents to the verifier.</summary>
    private const string Caller = "broiler-js-cli://wide";

    /// <summary>Compiles, verifies and runs a list of scripts in one realm.</summary>
    internal static RunResult Run(
        IReadOnlyList<SourceFile> files,
        bool module,
        bool checkOnly,
        bool forceStrict,
        ulong? fuel,
        ulong? wallClock,
        int? maximumDepth)
    {
        foreach (var file in files)
        {
            if (file.Unreadable.Length != 0)
            {
                return new RunResult(RunStatus.Unreadable, string.Empty, file.Unreadable, []);
            }
        }

        var goal = module ? SliceGoal.Module : SliceGoal.Script;

        var options = maximumDepth is { } depth
            ? new SliceParseOptions(goal, allowTopLevelAwait: false, depth)
            : module ? SliceParseOptions.Module : SliceParseOptions.Script;

        var scripts = new List<JsScriptUnit>(files.Count);

        for (var index = 0; index < files.Count; index++)
        {
            scripts.Add(new JsScriptUnit(
                "script" + index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                files[index].Text,
                options,
                forceStrict));
        }

        var compiled = JsCompiler.Compile(scripts);

        if (!compiled.Succeeded || compiled.Artifact is null)
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

        var created = VmRuntime.Create(Catalog(), Options(fuel, wallClock));

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
            return Run(runtime, compiled.Artifact, scripts.Count, checkOnly);
        }
    }

    private static RunResult Run(VmRuntime runtime, byte[] artifact, int count, bool checkOnly)
    {
        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
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
                    $"verifying the artifact spent the allowance: {verified.Reason} on {verified.Diagnostics.ExhaustedDimension}/{verified.Diagnostics.ExhaustedScope}",
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

            return new RunResult(
                RunStatus.RefusedArtifact,
                string.Empty,
                "the artifact verified and would not instantiate: " +
                    $"({instantiated.Outcome}/{instantiated.Reason})",
                []);
        }

        var value = string.Empty;

        for (var index = 0; index < count; index++)
        {
            var name = "script" + index.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(name)));

            var result = instance.Invoke(in request, CancellationToken.None);

            if (result.Outcome == VmOutcome.ResourceExhaustion)
            {
                return new RunResult(
                    RunStatus.Exhausted,
                    string.Empty,
                    $"the program did not settle within its allowance: {result.Reason} on {result.Diagnostics.ExhaustedDimension}",
                    []);
            }

            if (JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
            {
                return new RunResult(
                    RunStatus.Faulted,
                    string.Empty,
                    uncaught.ErrorName.Length == 0
                        ? "uncaught " + uncaught.Message
                        : "uncaught " + uncaught.Message,
                    []);
            }

            if (JavaScriptProfile.TryGetWideCompletion(in result, out var completion))
            {
                // THE PROGRAM'S VALUE IS THE LAST SCRIPT'S, AND `undefined` IS A VALUE. This read
                // `TypeOf == "undefined" ? string.Empty : completion.Value` until 2026-09-04, which
                // was wrong twice over. The same file printed `undefined` under `--slice` and
                // nothing here, so what the host printed depended on which manifest lowered it -
                // and a program a person would call identical answered differently. And carrying a
                // value forward meant an earlier script's 42 outlived a later script's `undefined`,
                // so the printed value was not any script's completion but the last interesting
                // one, which is not a rule anybody could state.
                value = completion.Value;
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

    /// <summary>The catalog: one profile, arriving through its own static accessor.</summary>
    private static VmCatalog Catalog() => VmCatalog.CreateBuilder()
        .Add(JavaScriptProfile.Descriptor)
        .Build();

    /// <summary>
    /// The runtime this host creates, with the one capability the wide surface imports registered.
    /// </summary>
    /// <remarks>
    /// <c>print</c> reaching standard output is this composition's decision and nobody else's. A
    /// sibling root that registers nothing composes the same profile and runs the same programs,
    /// and their <c>print</c> reaches nowhere - which is the difference registration is supposed to
    /// make.
    /// </remarks>
    private static VmRuntimeCreationOptions Options(ulong? fuel, ulong? wallClock)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel when fuel is { } stated => VmCeilingSpec.Value(dimension, stated),
                VmBudgetDimension.WallClock when wallClock is { } budget =>
                    VmCeilingSpec.Value(dimension, budget),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        capabilities.Add(VmCapabilityRegistration.Value(
            JavaScriptProfile.WriteCapability,
            Write));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }

    /// <summary>The one host capability this root registers: write a line to standard output.</summary>
    private static VmHostCallOutcome Write(VmBytes argument, out VmOpaqueRef result)
    {
        result = default;
        Console.Out.WriteLine(System.Text.Encoding.UTF8.GetString(argument.Span));
        return VmHostCallOutcome.Completed;
    }
}
