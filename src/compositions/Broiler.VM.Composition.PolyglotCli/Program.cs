// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.WebAssembly;

namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>
/// The end-user host across both input profiles: point it at a file and it runs the file.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS THE FIRST IMAGE IN THIS REPOSITORY THAT COMPOSES TWO PRODUCT PROFILES, AND THAT IS
/// NOT A SIDE EFFECT OF WRITING A COMMAND-LINE TOOL.</b> <c>docs/roadmap.md</c> section 14 has
/// asked since VM-3 for a catalog test over two profiles, and section 16 records that a
/// composition hosting two profiles does not close a gate until that test exists. Until now the
/// only two-profile composition in the checkout was <c>Broiler.VM.Composition.Workbench</c>, and
/// its two profiles are <c>Com.Example.Calculator</c> and <c>Com.Example.Ledger</c> - fixtures this
/// repository wrote, for this repository, in order to be composed. This root composes
/// <c>broiler.javascript</c> and <c>broiler.webassembly</c>: two product profiles with different
/// payload formats, different feature manifests, different diagnostic vocabularies and different
/// execution models, neither written with the other in mind. What the gate asked to see is that
/// adding the second profile requires no change to the core runtime and no change to the execution
/// loop, and the way this root shows it is that <see cref="Composition.Catalog"/> is two
/// <c>Add</c> calls and nothing else.
/// </para>
/// <para>
/// <b>WHAT THIS ROOT DOES NOT CLOSE, SAID IMMEDIATELY AFTER THE CLAIM AND NOT LATER.</b> Section
/// 14's row is a specific test - a profile composed beside a DELIBERATELY ADVERSE neighbour,
/// showing the neighbour's hard maxima do not reach it while its adopted defaults do - and this
/// root does not write it. Section 16 says in its own words that a second product profile "turns
/// both from a design argument into a thing with two real parties" and that "it does not make
/// either pass earlier"; that sentence covers this root exactly. Nor does it meet VM-3's own exit
/// gate, which asks that a two-profile composition publish and run under trimming AND Native AOT:
/// the trimmed publish was taken and run, and the Native AOT publish was attempted and its native
/// link step failed on the collecting machine, which the retained closure report says in its own
/// header. What changed on 2026-09-07 is that the condition became satisfiable, and a condition
/// that has become satisfiable is not a gate that has been passed.
/// </para>
/// <para>
/// <b>Where this root's code came from, said out loud.</b> The reading of files, the module
/// resolution, the artifact provider, the JavaScript run loop and the exit-code discipline are
/// copied from <c>Broiler.VM.Composition.JavaScript.Cli</c>, which is a real end-user host with an
/// acceptance suite driving its binary. Rules A11 and A12 leave shared code between composition
/// roots nowhere to live: a library holding it would be a project outside <c>src/compositions/</c>
/// naming a profile assembly, which A11 forbids, and a root referencing another root is not the
/// reference set A12 admits. Copying is therefore the normal shape here and every copied file says
/// so at its top.
/// </para>
/// <para>
/// <b>It is a demonstration and it is not advertised.</b> A tool advertised as a polyglot host
/// would be claiming both surfaces, and neither claim is one this checkout can make: the
/// JavaScript surface has no BigInt - the <c>BigInt</c> global is unbound so <c>typeof BigInt</c>
/// answers <c>undefined</c>, and a BigInt literal is refused by name at compile time with
/// <c>2104:ConstructOutsideManifest</c> - and the WebAssembly surface admits no import, no text
/// format, no vector, no GC, no exception, no thread and no memory64. Nothing here has been
/// reviewed by a person, and <c>docs/compositions.md</c> section 1's advertised set stays empty.
/// <i>(Corrected 2026-09-08. The JavaScript clause read "the JavaScript surface admits no async
/// function, no class field, no Proxy and no BigInt". Three of those four are admitted and run
/// here - an async function awaits and settles through the drained job queue, a class body carries
/// fields, private names, static blocks and generator members, and a Proxy's traps fire - and
/// BigInt is the one that is genuinely absent. The superseded reading is quoted rather than
/// deleted because a root that understates what it composes is the same defect as one that
/// overstates it, and the reason this root is not advertised is review and acceptance rather than
/// a short surface.)</i>
/// </para>
/// <para>
/// <b>AND THE BIGINT CLAUSE ABOVE WAS FALSE UNTIL LATER THE SAME DAY, WHICH IS WORTH A SECOND
/// NOTE RATHER THAN A SILENT EDIT.</b> <i>(Recorded 2026-09-08.)</i> When the correction above was
/// written it said BigInt was "the one that is genuinely absent", and the <i>binding</i> was - but
/// the <i>literal</i> was not: the wide front end admitted <c>1n</c> and evaluated it as a Number,
/// so <c>typeof 1n</c> answered <c>"number"</c>, <c>1n === 1</c> was <c>true</c>, and
/// <c>9007199254740993n</c> answered <c>9007199254740992</c>, a silently wrong integer that
/// <c>--numeric --native x86-64-win64</c> emitted machine code returning. Section 4.2 of
/// <c>src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md</c>, "The refusal that was lost",
/// had recorded every one of those readings and named it the one finding there that breaks a
/// property this profile has rather than missing one it never had. <c>JsParser</c> was fixed the
/// same day to notice the <c>n</c> suffix the tokenizer had always left on the token, and the
/// refusal is restored; <b>the value kind is still not implemented</b>, and BigInt arithmetic is a
/// widening nothing has scheduled. So the clause above is true from 2026-09-08 because the parser
/// changed, not because the claim was ever checked, and what a reader gets from today is a
/// refusal naming the construct rather than an absence taken on trust.
/// </para>
/// </remarks>
internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            return Dispatch(args);
        }
        catch (Exception failure)
        {
            // A HOST THAT DIES OF AN UNHANDLED EXCEPTION HAS NO EXIT CODE A CALLER CAN READ. The
            // type and message go to standard error and the code says this component is at fault.
            Console.Error.WriteLine($"broiler: unhandled {failure.GetType().Name}: {failure.Message}");
            return ExitCodes.HostDefect;
        }
    }

    /// <summary>
    /// Reads the subcommand, or decides there is none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A FLAGS-ONLY SURFACE CANNOT CARRY WORKLOADS, WHICH IS WHY THIS HOST HAS A GRAMMAR AND
    /// ITS PREDECESSOR DOES NOT.</b> The JavaScript host has fifteen options and one verb, and
    /// every new thing it learned to do arrived as another <c>--</c> flag: <c>--closure</c>,
    /// <c>--version</c>, <c>--check</c>. That works while there is one job. This host has to run a
    /// file, write an artifact to a named path, report its own closure and describe itself, and
    /// those are four jobs with four different argument shapes - <c>compile</c> alone takes an
    /// output path that <c>run</c> has no meaning for. Encoding four shapes in one flat option list
    /// makes every option's applicability a thing a reader has to know rather than a thing the
    /// grammar says.
    /// </para>
    /// <para>
    /// <b>BARE <c>broiler file.js</c> STILL RUNS THE FILE, and that is a compatibility decision
    /// rather than a convenience.</b> A person who has typed <c>broiler-js x.js</c> types
    /// <c>broiler x.js</c>, and a host that answered "unknown subcommand: x.js" would be teaching
    /// its grammar at the cost of the first thing anybody tries. The synonym is exact: the first
    /// argument is a subcommand only when it is one of the five names below, and a file called
    /// <c>run</c> with no extension is not one of them because this host routes on extension.
    /// </para>
    /// </remarks>
    private static int Dispatch(string[] args)
    {
        if (args.Length == 0)
        {
            Usage();
            return ExitCodes.Usage;
        }

        var verb = args[0];
        var rest = args[1..];

        switch (verb)
        {
            case "help" or "--help" or "-h":
                Usage();
                return ExitCodes.Ok;

            case "version" or "--version":
                return Version();

            case "closure" or "--closure":
                return Closure();

            case "run":
                return Run(rest);

            case "compile":
                return Compile(rest);

            default:
                // NOT A SUBCOMMAND, SO IT IS A PATH - and a mistyped subcommand is caught by the
                // routing rather than here, because a host that guessed "did you mean run?" would
                // have to decide which of `runn`, `ruin` and `./run.js` were guesses.
                return Run(args);
        }
    }

    // =============================================================================================
    // run
    // =============================================================================================

    private static int Run(string[] args)
    {
        if (!Options.TryRead(args, out var options, out var complaint))
        {
            Console.Error.WriteLine("broiler: " + complaint);
            return ExitCodes.Usage;
        }

        if (options.Paths.Count == 0)
        {
            Console.Error.WriteLine("broiler: no file or directory was named");
            return ExitCodes.Usage;
        }

        var files = Inputs.Expand(options.Paths, out var missing);

        foreach (var absent in missing)
        {
            Console.Error.WriteLine($"broiler: no file or directory at `{absent}`");
        }

        if (files.Count == 0)
        {
            Console.Error.WriteLine("broiler: nothing to run");
            return missing.Count == 0 ? ExitCodes.Usage : ExitCodes.Unreadable;
        }

        var read = new List<InputFile>(files.Count);

        foreach (var path in files)
        {
            read.Add(Inputs.Read(path));
        }

        var single = files.Count == 1;
        var counts = new Dictionary<RunStatus, int>();
        var worst = missing.Count == 0 ? RunStatus.Completed : RunStatus.Unreadable;

        foreach (var group in Group(read, options.Sweep))
        {
            var result = group[0].Lane == Lane.WebAssembly || group[0].Problem.Length != 0
                ? RunOne(group[0], options)
                : JavaScriptLane.Run(
                    group,
                    options.Module || Inputs.IsModulePath(group[^1].Path),
                    options.CheckOnly,
                    options.Strict,
                    options.MaximumDepth,
                    options.Allowances,
                    options.Request);

            counts[result.Status] = counts.TryGetValue(result.Status, out var seen) ? seen + 1 : 1;

            if (ExitCodes.Rank(result.Status) > ExitCodes.Rank(worst))
            {
                worst = result.Status;
            }

            Report(string.Join(' ', group.Select(static file => file.Path)), result, single, options);
        }

        if (!single)
        {
            Summarise(files.Count, counts);
        }

        return ExitCodes.For(worst);
    }

    private static RunResult RunOne(InputFile file, Options options) =>
        file.Problem.Length != 0
            ? new RunResult(file.Status, string.Empty, file.Problem, [])
            : file.Lane == Lane.WebAssembly
                ? WebAssemblyLane.Run(
                    file, options.CheckOnly, options.Invoke, options.Arguments, options.Allowances)
                : JavaScriptLane.Run(
                    [file],
                    options.Module || Inputs.IsModulePath(file.Path),
                    options.CheckOnly,
                    options.Strict,
                    options.MaximumDepth,
                    options.Allowances,
                    options.Request);

    /// <summary>
    /// Partitions the files into the runs this host performs, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A MAXIMAL RUN OF ADJACENT JAVASCRIPT FILES IS ONE REALM, AND EVERY OTHER FILE IS ITS
    /// OWN.</b> The one-realm rule is the JavaScript host's and it is what its two target workloads
    /// need - a benchmark harness and its benchmark, or a conformance harness and its test, are
    /// separate scripts that share a global object. A WebAssembly module shares nothing with
    /// anything: it has no global object to share, imports are not built, and two modules in one
    /// process are two instances however they were named.
    /// </para>
    /// <para>
    /// <b>The partition is by ADJACENCY and not by profile, which is the choice worth reading.</b>
    /// Grouping every JavaScript file in the command line into one realm regardless of position
    /// would silently reorder <c>broiler a.js m.wasm b.js</c> - a and b would share a realm that
    /// the module ran in the middle of, and the order a person wrote would not be the order that
    /// happened. Adjacency keeps the command line's order exactly, and a caller who wants two
    /// separated scripts in one realm writes them adjacently.
    /// </para>
    /// <para>
    /// <c>--sweep</c> makes every file its own run, which is what a directory of unrelated files
    /// wants: one file's globals must not decide the next one's result.
    /// </para>
    /// </remarks>
    private static IEnumerable<List<InputFile>> Group(IReadOnlyList<InputFile> files, bool sweep)
    {
        var current = new List<InputFile>();

        foreach (var file in files)
        {
            var joinable =
                !sweep &&
                current.Count != 0 &&
                file.Problem.Length == 0 &&
                current[^1].Problem.Length == 0 &&
                file.Lane == Lane.JavaScript &&
                current[^1].Lane == Lane.JavaScript;

            if (joinable)
            {
                current.Add(file);
                continue;
            }

            if (current.Count != 0)
            {
                yield return current;
            }

            current = [file];
        }

        if (current.Count != 0)
        {
            yield return current;
        }
    }

    private static void Report(string shown, RunResult result, bool single, Options options)
    {
        if (single)
        {
            if (result.Status == RunStatus.Completed && !options.Quiet && result.Value.Length != 0)
            {
                Console.WriteLine(result.Value);
            }

            if (result.Status != RunStatus.Completed)
            {
                Console.Error.WriteLine($"broiler: {shown}: {result.Detail}");

                if (options.All)
                {
                    foreach (var line in result.Diagnostics.Skip(1))
                    {
                        Console.Error.WriteLine($"         {line}");
                    }
                }
            }
            else if (options.All)
            {
                foreach (var line in result.Diagnostics)
                {
                    Console.WriteLine($"# {line}");
                }
            }

            return;
        }

        var status = result.Status switch
        {
            RunStatus.Completed => "ok      ",
            RunStatus.RefusedSource => "refused ",
            RunStatus.Faulted => "threw   ",
            RunStatus.Exhausted => "unbound ",
            RunStatus.RefusedArtifact => "ARTIFACT",
            RunStatus.Unreadable => "unread  ",
            RunStatus.Unroutable => "unrouted",
            _ => "DEFECT  ",
        };

        var note = result.Status == RunStatus.Completed ? result.Value : result.Detail;

        Console.WriteLine($"{status} {shown}" + (note.Length == 0 ? string.Empty : "  " + note));

        if (options.All && result.Diagnostics.Count > 1)
        {
            foreach (var line in result.Diagnostics.Skip(1))
            {
                Console.WriteLine($"         {line}");
            }
        }
    }

    /// <summary>Prints the distribution over a sweep, which is the answer a corpus run wants.</summary>
    private static void Summarise(int files, Dictionary<RunStatus, int> counts)
    {
        Console.WriteLine($"# {Number(files)} files");

        foreach (var status in Enum.GetValues<RunStatus>())
        {
            if (counts.TryGetValue(status, out var count))
            {
                Console.WriteLine($"# {status}: {Number(count)}");
            }
        }
    }

    // =============================================================================================
    // compile
    // =============================================================================================

    /// <summary>
    /// Lowers named JavaScript source into one artifact and writes it to a named path.
    /// </summary>
    /// <remarks>
    /// <b>THIS SUBCOMMAND EXISTS FOR ONE PROFILE AND REFUSES FOR THE OTHER, and the refusal is the
    /// honest answer rather than a gap.</b> The WebAssembly profile has no lowering anywhere in
    /// this repository: a <c>.wasm</c> file already IS the artifact, produced by a toolchain that
    /// is not here. So <c>broiler compile app.wasm -o app.bvm</c> is refused by name, because the
    /// only thing this host could do is copy the file, and a copy presented as a compilation would
    /// be a claim about a lowering nothing in this image has.
    /// </remarks>
    private static int Compile(string[] args)
    {
        if (!Options.TryRead(args, out var options, out var complaint))
        {
            Console.Error.WriteLine("broiler: " + complaint);
            return ExitCodes.Usage;
        }

        if (options.Output.Length == 0)
        {
            Console.Error.WriteLine("broiler: compile needs an output path; write -o <artifact>");
            return ExitCodes.Usage;
        }

        if (options.Paths.Count == 0)
        {
            Console.Error.WriteLine("broiler: no file was named");
            return ExitCodes.Usage;
        }

        var files = Inputs.Expand(options.Paths, out var missing);

        foreach (var absent in missing)
        {
            Console.Error.WriteLine($"broiler: no file or directory at `{absent}`");
        }

        if (files.Count == 0)
        {
            return missing.Count == 0 ? ExitCodes.Usage : ExitCodes.Unreadable;
        }

        var read = new List<InputFile>(files.Count);

        foreach (var path in files)
        {
            var file = Inputs.Read(path);

            if (file.Problem.Length != 0)
            {
                Console.Error.WriteLine($"broiler: {file.Path}: {file.Problem}");
                return ExitCodes.For(file.Status);
            }

            if (file.Lane != Lane.JavaScript)
            {
                Console.Error.WriteLine(
                    $"broiler: {file.Path}: the broiler.webassembly profile carries no lowering in " +
                    "this image, so a `.wasm` file is already an artifact and there is nothing " +
                    "here to compile it from");

                return ExitCodes.Usage;
            }

            read.Add(file);
        }

        var result = JavaScriptLane.Compile(
            read,
            options.Module || Inputs.IsModulePath(read[^1].Path),
            options.Strict,
            options.MaximumDepth,
            options.Request,
            out var artifact);

        if (result.Status != RunStatus.Completed)
        {
            Console.Error.WriteLine($"broiler: {result.Detail}");

            if (options.All)
            {
                foreach (var line in result.Diagnostics.Skip(1))
                {
                    Console.Error.WriteLine($"         {line}");
                }
            }

            return ExitCodes.For(result.Status);
        }

        try
        {
            File.WriteAllBytes(options.Output, artifact);
        }
        catch (IOException failure)
        {
            Console.Error.WriteLine($"broiler: cannot write `{options.Output}`: {failure.Message}");
            return ExitCodes.Unreadable;
        }
        catch (UnauthorizedAccessException failure)
        {
            Console.Error.WriteLine($"broiler: cannot write `{options.Output}`: {failure.Message}");
            return ExitCodes.Unreadable;
        }

        Console.WriteLine(
            $"{options.Output.Replace('\\', '/')} {Number(artifact.Length)} bytes " +
            $"{options.Request.Manifest} " +
            (options.Request.Form == JsOutputForm.Native
                ? "native " + options.Request.Backend
                : "bytecode"));

        return ExitCodes.Ok;
    }

    // =============================================================================================
    // version, closure
    // =============================================================================================

    private static int Version()
    {
        Console.WriteLine(
            $"broiler {JavaScriptProfile.Id} manifest {JavaScriptProfile.WideManifest} format " +
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion.ToString(
                System.Globalization.CultureInfo.InvariantCulture));

        // WHAT THE MANIFEST IS DEFINED AGAINST, on the line a person asks for when they ask what
        // this is. A manifest name is not a conformance claim and neither is an edition name.
        Console.WriteLine("edition " + JavaScriptLanguageEdition.Describe());

        Console.WriteLine(
            $"broiler {WebAssemblyProfile.Id} manifest {WebAssemblyProfile.SliceManifest} format 1");

        Console.WriteLine(
            "core-contract-version " +
            VmCoreContract.Version.ToString(System.Globalization.CultureInfo.InvariantCulture));

        return ExitCodes.Ok;
    }

    /// <summary>The closure this image actually has, read off the assemblies it composes.</summary>
    /// <remarks>
    /// Every root here has this mode and rule K4 reads its output. This root's table is the first
    /// with two profile rows in it that are not both fixtures, which is what makes the two-profile
    /// catalog claim readable rather than asserted.
    /// </remarks>
    private static int Closure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.PolyglotCli");

        // THE LABEL IS `narrow-runtime-compiler` FOR THE JAVASCRIPT HALF AND NOTHING FOR THE
        // OTHER, so this root prints the label it is entitled to and no more. It carries the
        // tokenizer, the static semantics and the lowering for a named restricted source surface,
        // and it is handed source from outside the image by a person naming a file - which is what
        // the label means. The WebAssembly half carries no lowering at all, so no label of that
        // family exists for it to claim.
        Console.WriteLine("label narrow-runtime-compiler");
        Console.WriteLine("carries-lowering yes");
        Console.WriteLine("profiles 2");

        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                JavaScriptProfile.Id,
                JavaScriptProfile.Descriptor.PackageIdentity.PackageId,
                JavaScriptProfile.Descriptor.DescriptorRevision,
                JavaScriptProfile.Descriptor.HostCapabilityDescriptors.Length));

        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                WebAssemblyProfile.Id,
                WebAssemblyProfile.Descriptor.PackageIdentity.PackageId,
                WebAssemblyProfile.Descriptor.DescriptorRevision,
                WebAssemblyProfile.Descriptor.HostCapabilityDescriptors.Length));

        // EVERY MANIFEST EACH DESCRIPTOR ACCEPTS, not the one this root happens to prefer. A
        // closure claim naming one while the image admits three would be read as the image
        // refusing the others, which is the opposite of true.
        Console.WriteLine(
            string.Join(
                ' ',
                "manifest",
                JavaScriptProfile.SliceManifest,
                JavaScriptProfile.WideManifest,
                JavaScriptProfile.NumericManifest,
                WebAssemblyProfile.SliceManifest));

        Console.WriteLine(
            string.Join(
                ' ',
                "format-versions",
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Min,
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Max,
                WebAssemblyProfile.Descriptor.SupportedFormatVersions.Min,
                WebAssemblyProfile.Descriptor.SupportedFormatVersions.Max));

        // The lowering, named from the assembly that actually carries it rather than declared.
        Console.WriteLine(
            string.Join(' ', "lowering", typeof(SliceSourceCompiler).Assembly.GetName().Name));

        // THE BACKEND ROSTER, WITH THE HALF THAT RUNS SEPARATED FROM THE HALF THAT DOES NOT. A
        // closure claim listing three backend names would read as three forms this image can run,
        // and one of them it can only emit.
        Console.WriteLine("native-backends " + string.Join(' ', JsNativeBackends.Names));
        Console.WriteLine(
            "native-executes " + JsNativeBackends.X64SystemV + " " + JsNativeBackends.X64Windows);

        Console.WriteLine("native-emitting-only " + JsNativeBackends.Arm64);

        return ExitCodes.Ok;
    }

    internal static string Number(long value) =>
        value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    // =============================================================================================
    // help
    // =============================================================================================

    private static void Usage()
    {
        Console.WriteLine("broiler - run JavaScript and WebAssembly on the Broiler.VM core");
        Console.WriteLine();
        Console.WriteLine("  broiler run <file>...            run each file on the profile it belongs to");
        Console.WriteLine("  broiler compile <file> -o <out>  lower JavaScript source into one artifact");
        Console.WriteLine("  broiler closure                  print this composition's closure claim");
        Console.WriteLine("  broiler version                  print both profiles' identities");
        Console.WriteLine("  broiler help                     print this");
        Console.WriteLine();
        Console.WriteLine("  broiler <file>...                a synonym for `run`");
        Console.WriteLine();
        Console.WriteLine("ROUTING. A `.js` or `.mjs` file goes to broiler.javascript and a `.wasm` file");
        Console.WriteLine("to broiler.webassembly. The extension decides, and the first four bytes are");
        Console.WriteLine("then checked against it: a `.wasm` file that does not open with 00 61 73 6D is");
        Console.WriteLine("refused rather than decoded, and a `.js` file that does is refused rather than");
        Console.WriteLine("tokenized. A mislabelled file is named as one instead of failing at byte zero.");
        Console.WriteLine();
        Console.WriteLine("Adjacent JavaScript files are run as separate scripts sharing ONE realm, in");
        Console.WriteLine("ORDINAL ORDER BY PATH and not in the order you named them, so two runs of the");
        Console.WriteLine("same set report the same rows in the same order; a harness that has to precede");
        Console.WriteLine("its benchmark has to be named so that it sorts first. Every WebAssembly module");
        Console.WriteLine("is its own instance. A directory is swept recursively for all three");
        Console.WriteLine("extensions, and --sweep gives every file its own run.");
        Console.WriteLine();
        Console.WriteLine("  --module    read each file under the module goal rather than the script goal");
        Console.WriteLine("              (a .mjs file is read as a module without it)");
        Console.WriteLine("  --strict    compile every script as strict-mode code");
        Console.WriteLine("  --sweep     run each file in a realm of its own rather than sharing one");
        Console.WriteLine("  --check     compile and verify only; do not run");
        Console.WriteLine("  --all       report every refusal in a file rather than the first");
        Console.WriteLine("  --quiet     do not print the completion value");
        Console.WriteLine("  --invoke <name>");
        Console.WriteLine("              the export to call in a WebAssembly module. Without it this host");
        Console.WriteLine("              calls `main`, then `_start`, then the module's only exported");
        Console.WriteLine("              function - and where a module exports several and names none of");
        Console.WriteLine("              them, it lists them and declines to guess.");
        Console.WriteLine("  --arg <type>:<literal>");
        Console.WriteLine("              one argument to that export; i32, i64, f32 or f64. Repeatable.");
        Console.WriteLine("  --fuel <n>  the instruction allowance per run");
        Console.WriteLine("  --wall <ms> the wall-clock allowance per run");
        Console.WriteLine("  --call-depth <n>  the call-depth allowance per run, in frames");
        Console.WriteLine("  --live-bytes <n>  the live-memory allowance per run");
        Console.WriteLine("  --max-depth <n>");
        Console.WriteLine("              the nesting depth the JavaScript parser admits, 1 to 512; 64");
        Console.WriteLine("              otherwise. A SECOND ceiling this option does not reach - a tree");
        Console.WriteLine("              deeper than the 10,000 levels the compiler walks - refuses");
        Console.WriteLine("              under the same 2103:NestingTooDeep code and no command line");
        Console.WriteLine("              moves it.");
        Console.WriteLine("  -o <path>   where `compile` writes the artifact");
        Console.WriteLine();
        Console.WriteLine("OUTPUT FORM. These select what the JavaScript lowering emits, and both are");
        Console.WriteLine("whole-artifact choices: there is no per-unit selection, no guard and no");
        Console.WriteLine("fallback to bytecode.");
        Console.WriteLine();
        Console.WriteLine($"  --numeric   compile against {JavaScriptProfile.NumericManifest}, which admits");
        Console.WriteLine("              Number values, the arithmetic, comparison, bitwise and unary");
        Console.WriteLine("              operators, let/const/var bindings of numbers, if/while/for/do and");
        Console.WriteLine("              blocks, function declarations of numbers called by name, and");
        Console.WriteLine("              return. It admits no object, string, closure, property access,");
        Console.WriteLine("              exception or dynamic construct, and refuses each of them BY NAME");
        Console.WriteLine("              at compile time. It is a smaller language and not a faster one.");
        Console.WriteLine("  --native <backend>");
        Console.WriteLine("              emit machine code for the named backend instead of bytecode.");
        Console.WriteLine("              This build names three, and they are not equal:");
        Console.WriteLine();
        Console.WriteLine($"                {JsNativeBackends.X64Windows}   emits, and executes on a Windows x64");
        Console.WriteLine("                                process and nowhere else");
        Console.WriteLine($"                {JsNativeBackends.X64SystemV}    emits, and executes on a System V x64");
        Console.WriteLine("                                process and nowhere else");
        Console.WriteLine($"                {JsNativeBackends.Arm64}   EMITTING-ONLY, and it admits LESS");
        Console.WriteLine("                                than either x86-64 backend: an instruction");
        Console.WriteLine("                                that reaches the realm is refused at compile");
        Console.WriteLine("                                time under 2104, so most programs never");
        Console.WriteLine("                                reach an artifact at all. One that does");
        Console.WriteLine("                                verifies and then refuses to instantiate.");
        Console.WriteLine("                                Nothing here arms an arm64 page.");
        Console.WriteLine();
        Console.WriteLine("              The arming path answers with the ONE x86-64 calling convention");
        Console.WriteLine("              this process's own platform uses - Windows x64 or System V x64,");
        Console.WriteLine("              never both - and refuses an artifact emitted for the other one,");
        Console.WriteLine("              or for any architecture at all, by name rather than running it");
        Console.WriteLine("              or falling back to the bytecode. (Corrected 2026-09-08. These");
        Console.WriteLine("              two lines read \"x86-64-win64 emits AND executes / x86-64-sysv");
        Console.WriteLine("              emits AND executes\" under the qualifier \"An x86-64 artifact is");
        Console.WriteLine("              armed only where the process architecture is x64\", which reads");
        Console.WriteLine("              as though both conventions were armed on one x64 process. Only");
        Console.WriteLine("              the platform's own is; the refusal this host prints for the");
        Console.WriteLine("              other has always said so.)");
        Console.WriteLine("              No measurement of either form is retained anywhere in this");
        Console.WriteLine("              repository, and this host claims no speed of any kind.");
        Console.WriteLine();
        Console.WriteLine("EXIT CODES. 0 completed, 1 faulted or trapped, 2 usage - which includes a file");
        Console.WriteLine("this host cannot route, AND ONE THING THAT IS NOT A ROUTING QUESTION AT ALL,");
        Console.WriteLine("named below - 3 source refused, 4 artifact refused, 5 allowance spent, 6");
        Console.WriteLine("unreadable, 7 host defect.");
        Console.WriteLine();
        Console.WriteLine("Over several runs ONE code wins, and it is not the numerically largest: the");
        Console.WriteLine("order is by WHOSE FAULT the answer is. Highest first, it runs 7, 4, 6, 2, 5,");
        Console.WriteLine("1, 3, 0 - host defect, then artifact refused, then unreadable, then");
        Console.WriteLine("unroutable, then allowance spent, then faulted, then source refused, then");
        Console.WriteLine("completed. 4 and 7 lead because both name a defect in this host rather than");
        Console.WriteLine("in the input, so a sweep turning up one lowering defect among thousands of");
        Console.WriteLine("ordinary refusals reports the defect rather than averaging it away.");
        Console.WriteLine("(Corrected 2026-09-08. This read \"Over several runs the worst code wins, and");
        Console.WriteLine("4 and 7 outrank the rest because both name a defect in this host rather than");
        Console.WriteLine("in the input.\" The second clause was and is true; the first reads as the");
        Console.WriteLine("largest number and that is not what the ranking does. A sweep run on");
        Console.WriteLine("2026-09-08 over a program that spent its fuel and a file this host cannot");
        Console.WriteLine("route reported both in its summary and exited 2, not 5. The ranking is");
        Console.WriteLine("deliberate and was not changed; the sentence describing it was wrong.)");
        Console.WriteLine();
        Console.WriteLine("2 ALSO COVERS AN ARTIFACT THIS IMAGE VERIFIED AND WILL NOT ARM, which the");
        Console.WriteLine("phrase \"a file this host cannot route\" does not describe and never did.");
        Console.WriteLine("`--numeric --native x86-64-sysv` on a Windows process, or `--native");
        Console.WriteLine("arm64-aapcs64` anywhere, compiles and verifies and then meets an arming path");
        Console.WriteLine("that answers for the ONE calling convention this process's own platform uses.");
        Console.WriteLine("The caller asked for a backend this machine does not arm - the same kind of");
        Console.WriteLine("mistake as a misspelled option, and not a fault in the file, in the source, or");
        Console.WriteLine("in this host's lowering - so it reports under `Unroutable`, the eighth status");
        Console.WriteLine("this host has and the JavaScript-only host does not, and exits 2 rather than");
        Console.WriteLine("4. Reporting a machine's architecture as this component's own lowering defect");
        Console.WriteLine("is the overclaim the mapping exists to avoid, and JavaScriptLane.cs argues it");
        Console.WriteLine("at the line that takes the decision.");
        Console.WriteLine();
        Console.WriteLine("`broiler-js`, the JavaScript-only host in");
        Console.WriteLine("src/compositions/Broiler.VM.Composition.JavaScript.Cli, EXITS 4 FOR THE SAME");
        Console.WriteLine("FILE, because it has no eighth status to send it to. Neither mapping is a");
        Console.WriteLine("defect and neither was changed; a script driving both has to accept 2 OR 4 for");
        Console.WriteLine("this one case.");
        Console.WriteLine();
        Console.WriteLine("(Recorded 2026-09-08. The difference was undocumented in both help texts until");
        Console.WriteLine("it was run: the same file on this win-x64 checkout, under `broiler run");
        Console.WriteLine("--numeric --native x86-64-sysv f.js` and `broiler-js --numeric --native");
        Console.WriteLine("x86-64-sysv f.js`, exits 2 and 4. The documentation was fixed and the mapping");
        Console.WriteLine("was not, because the argument behind the mapping is the right one; the");
        Console.WriteLine("reciprocal note is in that host's own exit-code paragraph.)");
        Console.WriteLine();
        Console.WriteLine($"WHAT THIS RUNS. The JavaScript default is {JavaScriptProfile.WideManifest}:");
        Console.WriteLine("objects, arrays, strings, functions, closures, prototypes, classes with super");
        Console.WriteLine("and new.target, symbols, typed arrays, keyed collections, promises, regular");
        Console.WriteLine("expressions, template literals, destructuring, spread, for-of, generators,");
        Console.WriteLine("exceptions, for-in, switch, labels, async functions and await, async");
        Console.WriteLine("generators and for await, the whole of the class body - fields, private names,");
        Console.WriteLine("static blocks and generator members - Proxy, `with`, modules, and a standard");
        Console.WriteLine("library.");
        Console.WriteLine();
        Console.WriteLine("BIGINT IS ABSENT IN THREE PLACES AND ALL THREE ANSWER IF YOU ASK THEM. The");
        Console.WriteLine("`BigInt` global is not bound, so `typeof BigInt` answers `undefined`, and");
        Console.WriteLine("`BigInt64Array` and `BigUint64Array` are absent beside it. AND A BIGINT");
        Console.WriteLine("LITERAL IS REFUSED BY NAME AT COMPILE TIME: `1n` answers");
        Console.WriteLine("2104:ConstructOutsideManifest, `a BigInt literal is not admitted by the");
        Console.WriteLine("declared feature manifest`, under every manifest this host selects. Point this");
        Console.WriteLine("host at it and read the code rather than taking it from here.");
        Console.WriteLine();
        Console.WriteLine("(Corrected 2026-09-08. This paragraph read \"It admits no async function, class");
        Console.WriteLine("field, private name, class static block, generator member of a class body,");
        Console.WriteLine("Proxy or BigInt.\" Every one of those but BigInt runs to completion here - point");
        Console.WriteLine("this host at each and read the answer. The list was copied forward from the");
        Console.WriteLine("JavaScript-only host and went stale with it, and a host whose own help text");
        Console.WriteLine("understates what it does is the same defect as one that overstates it.)");
        Console.WriteLine();
        Console.WriteLine("(Recorded 2026-09-08, LATER THE SAME DAY, because the sentence that replaced");
        Console.WriteLine("the one above became true by accident within hours of being written. That");
        Console.WriteLine("sentence read \"It admits no BigInt, and BigInt is the only construct this");
        Console.WriteLine("paragraph denies\", and it was FALSE WHEN IT WAS WRITTEN: the wide front end");
        Console.WriteLine("ADMITTED a BigInt literal and evaluated it as a Number, so `typeof 1n` answered");
        Console.WriteLine("\"number\", `1n === 1` was `true`, and `9007199254740993n` answered");
        Console.WriteLine("9007199254740992 - a silently wrong integer that `--numeric --native");
        Console.WriteLine("x86-64-win64` emitted machine code returning. It is true now because the wide");
        Console.WriteLine("parser was fixed later that day to notice the `n` suffix the tokenizer had");
        Console.WriteLine("always left on the token, and NOT because anybody had checked the claim. The");
        Console.WriteLine("profile's own record had said so all along: section 4.2, \"The refusal that was");
        Console.WriteLine("lost\", of src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md, which calls");
        Console.WriteLine("it the one finding there that breaks a property this profile HAS rather than");
        Console.WriteLine("missing one it never had. What a reader meets from today is a refusal naming");
        Console.WriteLine("the construct rather than an absence taken on trust - and the VALUE KIND is");
        Console.WriteLine("still not implemented, so this is a restored refusal and not a new feature.");
        Console.WriteLine("The word ONLY is not carried forward, because it was never true either: a");
        Console.WriteLine("decorator is refused under the same 2104 code, and an `accessor` class element");
        Console.WriteLine("and a `using` declaration are refused as syntax errors. BigInt is the absence");
        Console.WriteLine("this paragraph spells out, not the only one the manifest has.)");
        Console.WriteLine();
        Console.WriteLine($"The WebAssembly surface is {WebAssemblyProfile.SliceManifest}: it decodes,");
        Console.WriteLine("validates, instantiates and executes a module - integer and floating-point");
        Console.WriteLine("arithmetic, locals, globals, linear memory, structured control flow, direct and");
        Console.WriteLine("indirect calls, start functions, element and data segments, and traps. It has");
        Console.WriteLine("NO imports and no linking, no text format, no vector instructions, no garbage");
        Console.WriteLine("collection, no exception handling, no threads and no memory64. A module that");
        Console.WriteLine("declares an import is refused rather than linked.");
        Console.WriteLine();
        Console.WriteLine("Nothing here has been reviewed by a person, no composition in this repository");
        Console.WriteLine("is advertised, and this host is advertised as nothing.");
    }
}
