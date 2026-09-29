using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Profile.WebAssembly;
using Broiler.VM.Ubc;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The harness root: where this profile's corpus encoder, its retained corpus, the replay that
/// holds it and the differential lane that scores the execution live, and where they must live.
/// </summary>
/// <remarks>
/// <para>
/// <b>Everything here has to name the profile assembly, and rule A11 forbids a project outside
/// src/compositions/ to name one.</b> An encoder that produces the bytes a verifier is asked to
/// accept or refuse is therefore a composition root and never a test project. That is forced rather
/// than chosen, and it is the reason a corpus store sits beside a `Main`.
/// </para>
/// <para>
/// <b>It is never advertised.</b> A corpus encoder and a differential lane must appear in no package
/// and in no advertised composition's closure - not in "no published closure", which this root's
/// own publish would falsify.
/// </para>
/// <para>
/// <b>Four lanes and what each one is worth.</b> The canonical module is decoded, instantiated and
/// invoked on every run, so a regression in the padded-length reader that made this profile write
/// its own LEB128 layer fails before anything else runs. The RETAINED CORPUS is replayed from disk
/// against recorded hashes and recorded answers - see <see cref="CorpusStore"/> for what a derived
/// row claims and what a recorded one does not. The EXECUTION CHECKS drive real modules through the
/// whole core lifecycle. The DIFFERENTIAL LANE scores the interpreter against answers derived
/// somewhere other than the interpreter, which is the only kind of answer worth comparing against.
/// <i>(Noted 2026-09-29: three lanes have joined these four, and each runs on every run. The HOOK
/// CHECKS alter a translator-written artifact and expect the family hook's code (<see cref="HookChecks"/>).
/// The OUTCOME CHECKS hold the verifier outcomes and positions no corpus entry can hold
/// (<see cref="OutcomeChecks"/>). The PROPORTIONALITY CHECKS measure what <c>memory.grow</c> is
/// charged against its declared function, in runtimes of their own (<see cref="ProportionalityChecks"/>).)</i>
/// </para>
/// <para>
/// <b>Every module goes through the translator first.</b> Since the universal bytecode programme's
/// milestone UBC-4 the core verifies universal bytecode and never a bare module: every lane hands a
/// module to <see cref="WasmTranslator"/> through <see cref="ModuleVerification"/>, and the core
/// verifies the artifact the translation answers under the family's descriptor, which this root
/// builds over the bytecode emitter. A module the translator refuses is reported in the fields the
/// core reported it in, so every lane prints what it printed before the translator existed wherever
/// the answer did not change.
/// </para>
/// <para>
/// <b>Three lanes run only when asked for, and none is part of the corpus replay, the execution
/// checks or the differential lane.</b> <c>--determinism</c> translates every corpus module, and
/// every module the execution and differential lanes built, twice and compares the artifacts byte for
/// byte. <c>--primitives &lt;file&gt;</c> is obligation E2 of the universal bytecode: every numeric
/// primitive row of the family's table, the profile's reference arms against the primitive table
/// over the retained primitive input corpus, with the twelve float comparisons reported apart as the
/// negative control. <c>--regions &lt;file&gt;</c> is the same obligation for the region rows - the
/// loads, the stores and <c>memory.size</c> - the profile's memory arms against the table's region
/// primitive over the retained region input corpus, the regions compared after every store. No
/// lane's header opens with a name the headers of those three lanes open with, and a run without the
/// flags prints none of them.
/// </para>
/// <para>
/// <b><c>--spec &lt;directory&gt;</c> is a lane of its own, and it runs alone.</b> It reads the
/// specification's core test scripts from the directory, checks them against the pin <c>--expect</c>
/// names, and runs every command through the core in a runtime per script. It prints one line per
/// command with the profile's answer and the specification's verdict. <see cref="SpecSuite"/> says
/// what the lane is and <see cref="ScriptRunner"/> how a command is answered. It is the population A of
/// bundle UBC-4-005's decision rule.
/// </para>
/// <para>
/// <b>What is still NOT here.</b> There is no reader for the specification's text format and so no
/// way to run the published conformance assertions; every module in this root was written by hand
/// by the same person who wrote its expected answer, and a corpus like that cannot find a refusal
/// nobody thought of. There is no replay under a second publish mode, because this root publishes
/// one. And no corpus row records an execution answer - every row stops at verification.
/// <i>(Corrected 2026-09-28: the first sentence no longer holds. The <c>--spec</c> lane is that reader,
/// over the core test scripts pinned at <c>src/tests/wasm/spec</c>. What this root's own modules can and
/// cannot find is unchanged. The second no longer holds either, and had not since bundles
/// <c>ubc-4-003</c> and <c>ubc-4-004</c> ran this root in three publish modes. Both sentences are kept as
/// what this paragraph said.)</i>
/// </para>
/// </remarks>
internal static class Program
{
    /// <summary>
    /// Where the retained corpus lives, relative to the repository root.
    /// </summary>
    /// <remarks>
    /// Named here and passed to every caller rather than defaulted inside each lane. A default that
    /// each lane computed for itself would let the writer and the replay disagree about which
    /// directory they were talking about, and the disagreement would look like a corpus that always
    /// passes.
    /// </remarks>
    internal const string RetainedCorpusPath = "src/tests/wasm/corpus";

    private static int Main(string[] args)
    {
        var verbose = args.Contains("--verbose", StringComparer.Ordinal);

        try
        {
            if (args.Contains("--closure", StringComparer.Ordinal))
            {
                return ReportClosure();
            }

            // The specification's scripts are a lane of their own, in runtimes of their own.
            if (args.Contains("--spec", StringComparer.Ordinal))
            {
                return SpecSuite.Run(args);
            }

            using var runtime = Runtime(out var creationFailure);

            if (runtime is null)
            {
                Console.WriteLine($"broiler-wasm-harness: {creationFailure}");
                return 1;
            }

            // WRITING IS A MODE OF ITS OWN AND NEVER A SIDE EFFECT OF A RUN. A lane that rewrote the
            // corpus while checking it would agree with itself on every run, which is the one thing
            // a retained corpus may not do; regenerating has to be something a person asked for and
            // a reviewer can see in a diff.
            var writeTo = Argument(args, "--write-corpus");

            if (writeTo is not null)
            {
                return CorpusReplay.Write(runtime, writeTo);
            }

            var corpus = Argument(args, "--corpus");
            var primitives = Argument(args, "--primitives");
            var regions = Argument(args, "--regions");
            var determinism = args.Contains("--determinism", StringComparer.Ordinal);
            var failed = ReportTheDecodedModule(runtime);

            if (corpus is not null)
            {
                failed += CorpusReplay.Report(runtime, corpus, verbose);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(
                    "# retained corpus: not replayed - no --corpus <directory> was given, and " +
                    $"the retained one is at {RetainedCorpusPath}");
                Console.WriteLine();
            }

            // THE LANES THAT RE-READ WHAT THE OTHERS BUILT ARE ASKED FOR, never run by default. The
            // modules the execution and differential lanes build are written down only when the
            // determinism lane will translate them again.
            ModuleVerification.Recorded = determinism ? [] : null;

            failed += ExecutionChecks.Report(runtime, verbose);
            failed += HookChecks.Report(runtime, verbose);
            failed += OutcomeChecks.Report(runtime, verbose);
            failed += DifferentialChecks.Report(runtime, verbose);
            failed += ProportionalityChecks.Report(verbose);

            var built = ModuleVerification.Recorded;
            ModuleVerification.Recorded = null;

            if (built is not null)
            {
                Console.WriteLine();
                failed += Determinism.Report(runtime, built, verbose);
            }

            // A --primitives WITH NO FILE AFTER IT STILL RUNS THE LANE, which then fails: asking for
            // a lane and getting a passing run that never printed it would read as the lane passing.
            if (primitives is not null || args.Contains("--primitives", StringComparer.Ordinal))
            {
                Console.WriteLine();
                failed += PrimitiveDifferential.Report(primitives, verbose);
            }

            // The region rows' half of the same obligation, under the same rule for a flag with no file.
            if (regions is not null || args.Contains("--regions", StringComparer.Ordinal))
            {
                Console.WriteLine();
                failed += RegionDifferential.Report(regions, verbose);
            }

            Console.WriteLine(
                failed == 0
                    ? $"broiler-wasm-harness: every check passed, core contract version {VmCoreContract.Version}"
                    : $"broiler-wasm-harness: {failed} checks FAILED");

            return failed == 0 ? 0 : 1;
        }
        catch (Exception failure)
        {
            Console.WriteLine(
                $"broiler-wasm-harness: unhandled {failure.GetType().Name}: {failure.Message}");

            return 2;
        }
    }

    /// <summary>The value after a named argument, or nothing where it was not given.</summary>
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

    /// <summary>
    /// Decodes the hand-written module and prints what the decoder found in it.
    /// </summary>
    /// <remarks>
    /// The module carries one padded section length on purpose. It is the encoding a single-pass
    /// compiler emits, the core's canonical reader rejects it, and this profile's own reader must
    /// accept it - so it is in the one module every run decodes rather than in an entry somebody
    /// could skip.
    /// </remarks>
    private static int ReportTheDecodedModule(VmRuntime runtime)
    {
        var bytes = CorpusStore.CanonicalModule();
        var verified = ModuleVerification.Verify(
            runtime, bytes, "composition-wasm-harness://artifact", "canonical module");

        Console.WriteLine($"# module: {bytes.Length} bytes, hand-encoded in this root");
        Console.WriteLine($"verification {verified.Outcome}/{verified.Reason}");

        if (!verified.TryGetArtifact(out var artifact))
        {
            Console.WriteLine(
                $"FAIL the canonical module did not verify: " +
                $"code {verified.Code} at " +
                $"section {verified.Position.SectionIndex} " +
                $"offset {verified.Position.ByteOffset}");

            return 1;
        }

        using (artifact)
        {
            // WHAT THE CORE HOLDS IS THE TRANSLATION'S PROGRAM, and what this lane describes is the
            // module the translation decoded and validated on the way to it.
            if (!artifact.TryGetState(out var state) || state is not UbcVerifiedProgram ||
                verified.Module is not { } module)
            {
                Console.WriteLine(
                    "FAIL the verified artifact does not carry a universal bytecode program " +
                    "translated from a decoded module");

                return 1;
            }

            Console.WriteLine($"types                  {module.TypeCount}");
            Console.WriteLine($"functions              {module.FunctionCount}");
            Console.WriteLine($"tables                 {module.TableCount}");
            Console.WriteLine($"memories               {module.MemoryCount}");
            Console.WriteLine($"globals                {module.GlobalCount}");
            Console.WriteLine($"exports                {module.ExportCount}");
            Console.WriteLine($"element segments       {module.ElementSegmentCount}");
            Console.WriteLine($"data segments          {module.DataSegmentCount}");
            Console.WriteLine($"custom sections        {module.CustomSectionCount} (skipped)");
            Console.WriteLine($"start function         {module.StartFunctionIndex}");
            Console.WriteLine($"declares data count    {module.DeclaresDataCount}");
            Console.WriteLine($"total code bytes       {module.TotalCodeBytes}");
            Console.WriteLine(
                $"execution bounds       {(module.ExecutionBoundsComputed ? "computed by validation" : "NOT computed - validation did not run")}");

            if (!module.ExecutionBoundsComputed)
            {
                Console.WriteLine("FAIL a verified module carries bodies validation never sealed");
                return 1;
            }

            for (var index = 0; index < module.ExportCount; index++)
            {
                if (!module.TryDescribeExport(index, out var kind, out var entity, out var nameLength))
                {
                    continue;
                }

                var name = new byte[nameLength];
                module.TryCopyExportName(index, name, out _);
                Console.WriteLine(
                    $"  export[{index}]          \"{Encoding.UTF8.GetString(name)}\" {kind} #{entity}");
            }

            for (var index = 0; index < module.FunctionCount; index++)
            {
                if (module.TryDescribeFunctionBody(index, out var locals, out var codeLength))
                {
                    Console.WriteLine($"  body[{index}]            {locals} locals, {codeLength} code bytes");
                }
            }

            // THE MODULE IS RUN, NOT JUST DESCRIBED. Instantiating it allocates its memory and
            // table, evaluates its global initialiser and applies its element and data segments;
            // invoking its export runs the bytecode emitter over the translation of the body
            // printed above.
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            Console.WriteLine(
                $"instantiation          {instantiated.Outcome}/{instantiated.Reason}");

            if (!instantiated.TryGetInstance(out var instance))
            {
                Console.WriteLine("FAIL the canonical module did not instantiate");
                return 1;
            }

            var utf8 = Encoding.UTF8.GetBytes("3:addi32:1:2i32:1:3");
            var request = new VmInvocationRequest(new VmUtf8Text(utf8));
            var answered = instance.Invoke(in request, CancellationToken.None);

            if (!WebAssemblyProfile.TryGetResults(in answered, out var returned) ||
                returned.Count != 1 ||
                !returned.TryGetValue(0, out var value) ||
                value.AsInt32 != 5)
            {
                Console.WriteLine(
                    $"FAIL invoking \"add\" answered {answered.Outcome}/{answered.Reason}");

                return 1;
            }

            Console.WriteLine(
                $"invocation             {answered.Outcome}/{answered.Reason}, " +
                $"add(2, 3) = {value.AsInt32}");
        }

        Console.WriteLine();
        return 0;
    }

    private static int ReportClosure()
    {
        var descriptor = ModuleVerification.Descriptor;

        Console.WriteLine(
            $"# broiler-vm-composition core-contract-version={VmCoreContract.Version} " +
            $"ubc-contract-version={UbcContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.WebAssembly.Harness");
        Console.WriteLine("label none");
        Console.WriteLine("advertised no");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                WebAssemblyProfile.Id,
                descriptor.PackageIdentity.PackageId,
                descriptor.DescriptorRevision,
                descriptor.HostCapabilityDescriptors.Length));
        Console.WriteLine(string.Join(' ', "manifest", WebAssemblyProfile.SliceManifest));

        // What turns a module into what the core verifies, and the one form that runs it.
        Console.WriteLine(
            string.Join(
                ' ',
                "translator",
                WasmTranslator.TranslatorIdentity,
                WasmTranslator.TranslatorVersion.ToString(CultureInfo.InvariantCulture)));
        Console.WriteLine(
            $"form {UbcBytecodeEmitter.Form.Identity} " +
            UbcBytecodeEmitter.Form.SemanticVersion.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine($"corpus-entries {CorpusStore.Entries().Count}");
        Console.WriteLine($"corpus-retained yes {RetainedCorpusPath}");
        Console.WriteLine("interpreter yes");
        Console.WriteLine("linker no");

        return 0;
    }

    internal static VmRuntime? Runtime(out string failure)
    {
        var catalog = VmCatalog.CreateBuilder()
            .Add(ModuleVerification.Descriptor)
            .Build();

        var created = VmRuntime.Create(catalog, Options());

        if (created.TryGetRuntime(out var runtime))
        {
            failure = string.Empty;
            return runtime;
        }

        failure = $"runtime creation {created.Outcome}/{created.Reason}";
        return null;
    }

    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
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
