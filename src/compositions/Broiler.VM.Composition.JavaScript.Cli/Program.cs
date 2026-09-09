using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// The end-user host: point it at a JavaScript file and it compiles, verifies and runs it.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is the <c>narrow-runtime-compiler</c> composition and the first root here that earns
/// the label.</b> Section 15 of the profile roadmap defines it as a composition carrying the
/// tokenizer, the static semantics and the lowering for a named restricted surface;
/// <c>docs/compositions.md</c> recorded that no root held it, because the slice-compiler root
/// lowers a programmatic builder rather than source. This one is handed a path by a person.
/// </para>
/// <para>
/// <b>It is not advertised and it is not packable</b>, and the reason is worth stating where a
/// reader will meet it: a tool advertised as a JavaScript host has to be able to run JavaScript,
/// and running a great deal of it is still not the claim advertising would make. Nothing here is
/// reviewed, nothing is accepted, and <c>docs/compositions.md</c> section 1's advertised set stays
/// empty. <i>(Corrected 2026-09-08. This paragraph read "<c>broiler.javascript.slice</c> admits no
/// function, no object, no string value and no property access. Pointed at real-world JavaScript
/// this host refuses almost every file - by name, with the construct named, which is the useful
/// part - and a support claim over that would be untruthful." That was true the day it was written
/// and has been false since 2026-09-04, when <c>WideHost</c> landed: <see cref="Form"/> below
/// selects <c>broiler.javascript.wide</c> by default and keeps the slice behind <c>--slice</c>,
/// and <c>--help</c> says so. Pointed at the pinned Octane checkout, <c>--check</c> at the
/// defaults compiles and verifies fifteen of its twenty-one files, and every one of the six it
/// refuses meets the declared <c>2103:NestingTooDeep</c> ceiling rather than a missing construct -
/// run it and read the codes rather than taking the count from here. The old text is quoted rather
/// than deleted because a root whose own record understates what it does is the same defect as one
/// that overstates it, and this component treats both as a stop condition.)</i>
/// </para>
/// <para>
/// <b>It carries no test tooling.</b> No census, no corpus producer, no conformance harness, no
/// fuzz mutator, no soak. Every sibling root carries some of that because rules A11 and A12 leave
/// it nowhere else; this root's closure is the one with nothing to explain away, which is what
/// makes it readable against the label.
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
            Console.Error.WriteLine($"broiler-js: unhandled {failure.GetType().Name}: {failure.Message}");
            return ExitCodes.HostDefect;
        }
    }

    private static int Dispatch(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help", StringComparer.Ordinal))
        {
            Usage();
            return args.Length == 0 ? ExitCodes.Usage : ExitCodes.Ok;
        }

        if (args.Contains("--version", StringComparer.Ordinal))
        {
            Console.WriteLine(
                $"broiler-js {JavaScriptProfile.Id} manifest {JavaScriptProfile.WideManifest} format " +
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion.ToString(
                    System.Globalization.CultureInfo.InvariantCulture));

            // WHAT THE MANIFEST IS DEFINED AGAINST, on the line a person asks for when they ask
            // what this is. A manifest name is not a conformance claim and neither is an edition
            // name - but "which JavaScript" is the first question a host's version output should
            // be able to answer, and until this line existed the honest answer was that nobody had
            // written it down. It names where the archive is or says it is missing, because a
            // version string that implied a fully taken pin would be the overclaim this whole
            // record is against - `JavaScriptLanguageEdition.Describe` decides which, so the line
            // cannot go stale in either direction. (Corrected 2026-09-08. This read "It names the
            // MISSING archive too", which described the day it was written and not this checkout:
            // the pin was archived on 2026-09-03 and the line has printed "archived at
            // src/Broiler.VM.Profile.JavaScript/docs/specification/ecma-262-es2026-spec.html"
            // since. A comment understating what its own line prints is the same defect as one
            // overstating it, so the superseded reading is quoted rather than deleted.)
            Console.WriteLine("edition " + JavaScriptLanguageEdition.Describe());

            return ExitCodes.Ok;
        }

        if (args.Contains("--closure", StringComparer.Ordinal))
        {
            return Closure();
        }

        // THE HOST-SURFACE LANE, and it takes no file. What it runs is written into the checks
        // themselves, because each one is a pair - a guest program and the lines that program must
        // print - and separating the two into a corpus would put the answer somewhere a reader
        // comparing them has to go and find.
        if (args.Contains("--host-surface", StringComparer.Ordinal))
        {
            var failures = HostSurfaceChecks.Run();

            Console.WriteLine(
                failures == 0
                    ? "host-surface: every check passed"
                    : "host-surface: " + Host.Number(failures) + " check(s) failed");

            return failures == 0 ? ExitCodes.Ok : ExitCodes.Faulted;
        }

        var module = args.Contains("--module", StringComparer.Ordinal);
        var checkOnly = args.Contains("--check", StringComparer.Ordinal);
        var all = args.Contains("--all", StringComparer.Ordinal);
        var quiet = args.Contains("--quiet", StringComparer.Ordinal);
        var slice = args.Contains("--slice", StringComparer.Ordinal);
        var forceStrict = args.Contains("--strict", StringComparer.Ordinal);
        var sweep = args.Contains("--sweep", StringComparer.Ordinal);

        if (!Form(args, out var request, out var formComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + formComplaint);
            return ExitCodes.Usage;
        }

        if (!Fuel(args, out var fuel, out var fuelComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + fuelComplaint);
            return ExitCodes.Usage;
        }

        if (!Wall(args, out var wall, out var wallComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + wallComplaint);
            return ExitCodes.Usage;
        }

        if (!Depth(args, out var depth, out var depthComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + depthComplaint);
            return ExitCodes.Usage;
        }

        if (!CallDepth(args, out var callDepth, out var callDepthComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + callDepthComplaint);
            return ExitCodes.Usage;
        }

        if (!LiveBytes(args, out var liveBytes, out var liveBytesComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + liveBytesComplaint);
            return ExitCodes.Usage;
        }

        var paths = new List<string>();

        for (var index = 0; index < args.Length; index++)
        {
            if (string.Equals(args[index], "--fuel", StringComparison.Ordinal) ||
                string.Equals(args[index], "--native", StringComparison.Ordinal) ||
                string.Equals(args[index], "--wall", StringComparison.Ordinal) ||
                string.Equals(args[index], "--max-depth", StringComparison.Ordinal) ||
                string.Equals(args[index], "--call-depth", StringComparison.Ordinal) ||
                string.Equals(args[index], "--live-bytes", StringComparison.Ordinal))
            {
                index++;
                continue;
            }

            if (args[index].StartsWith("--", StringComparison.Ordinal))
            {
                if (!Known.Contains(args[index], StringComparer.Ordinal))
                {
                    Console.Error.WriteLine($"broiler-js: `{args[index]}` is not an option this host has");
                    return ExitCodes.Usage;
                }

                continue;
            }

            paths.Add(args[index]);
        }

        if (paths.Count == 0)
        {
            Console.Error.WriteLine("broiler-js: no file or directory was named");
            return ExitCodes.Usage;
        }

        if (!Host.Composes(out var why))
        {
            Console.Error.WriteLine($"broiler-js: the profile did not compose: {why}");
            return ExitCodes.HostDefect;
        }

        var files = SourceFiles.Expand(paths, out var missing);

        foreach (var absent in missing)
        {
            Console.Error.WriteLine($"broiler-js: no file or directory at `{absent}`");
        }

        if (files.Count == 0)
        {
            Console.Error.WriteLine("broiler-js: nothing to run");
            return missing.Count == 0 ? ExitCodes.Usage : ExitCodes.Unreadable;
        }

        // SEVERAL NAMED FILES ARE ONE PROGRAM IN ONE REALM, unless a sweep was asked for. That
        // is what a shell does, and it is what both target workloads need: a benchmark harness and
        // its benchmark, or a conformance harness and its test, are separate SCRIPTS that share a
        // global object. A directory is different - a sweep over a tree wants a fresh realm per
        // file, or one file's globals would decide the next one's result.
        var oneRealm = !slice && !sweep && paths.TrueForAll(File.Exists) && files.Count > 1;

        if (oneRealm)
        {
            var read = new List<SourceFile>(files.Count);

            foreach (var path in files)
            {
                read.Add(SourceFiles.Read(path));
            }

            var joined = WideHost.Run(
                read,
                module || SourceFiles.IsModulePath(files[^1]),
                checkOnly,
                forceStrict,
                fuel,
                wall,
                depth,
                callDepth,
                liveBytes,
                request);
            Report(string.Join(' ', files), joined, single: true, all, quiet);
            return ExitCodes.For(joined.Status);
        }

        return Run(
            files, module, checkOnly, all, quiet, fuel, wall, depth, callDepth, liveBytes,
            missing.Count, slice, forceStrict, request);
    }

    /// <summary>Reads the feature manifest and output form the arguments ask for.</summary>
    /// <remarks>
    /// <para>
    /// <b><c>--numeric</c> COMPILES A SMALLER LANGUAGE AND NOT A FASTER ONE.</b> It names the
    /// <c>broiler.javascript.numeric</c> manifest, which admits Number values, the arithmetic,
    /// comparison, bitwise and unary operators, bindings of numbers, the ordinary statements, and
    /// functions of numbers called by name - and refuses everything else by name, at compile time.
    /// A program that uses a String, an object or a closure is refused rather than compiled
    /// differently.
    /// </para>
    /// <para>
    /// <b><c>--native</c> NAMES A BACKEND, AND WHAT DIFFERS BETWEEN THE THREE IS WHETHER ANYTHING
    /// RUNS WHAT THEY EMIT.</b> <i>(Revised 2026-09-07. This paragraph read "EVERY BACKEND THIS
    /// BUILD CARRIES REFUSES" and "No instruction of any architecture is encoded anywhere in this
    /// component". Both were true when written and both are now false; they are quoted rather than
    /// deleted because a host whose own help text understates what it does is the failure this
    /// component treats as a stop condition.)</i> Both <c>x86-64</c> conventions emit machine code,
    /// and the arming path takes exactly one of them: <c>JsNativeExecution.HostArchitecture</c>
    /// answers with the convention this process's own platform uses - Windows x64 on Windows,
    /// System V x64 elsewhere - so a program compiled under that one answers what the same source
    /// answers through the interpreter, and an artifact emitted for the other convention verifies
    /// and then refuses to instantiate by name. <c>arm64-aapcs64</c> emits and is never armed
    /// anywhere - it is <b>emitting-only</b>, so an artifact carrying its bytes verifies, refuses to
    /// instantiate, and is checked by comparing bytes against a retained expectation rather than by
    /// being run. <i>(Corrected 2026-09-08. The two sentences above read "Both <c>x86-64</c>
    /// conventions emit machine code that this host also arms and executes, and a program compiled
    /// under either answers what the same source answers through the interpreter." That overstates
    /// by one convention in exactly the direction the revision above was correcting understatement:
    /// on this win-x64 checkout <c>--numeric --native x86-64-sysv</c> compiles and verifies and
    /// then answers <c>UnsatisfiedHostAssumption</c>, which is the declared behaviour of a form
    /// emitted for somewhere else and not a defect.)</i>
    /// </para>
    /// <para>
    /// <b>It implies nothing about speed and this host makes no claim about any.</b> That sentence
    /// survives the revision above unchanged and matters more now than it did: a form that runs is
    /// exactly the kind of thing a reader will assume a number about, and this component has
    /// retained no measurement of either form, under no predeclared rule, in no baseline register.
    /// </para>
    /// </remarks>
    private static bool Form(string[] args, out JsCompileRequest request, out string complaint)
    {
        var manifest = args.Contains("--numeric", StringComparer.Ordinal)
            ? JsFeatureManifest.Numeric
            : JsFeatureManifest.Wide;

        request = new JsCompileRequest(manifest);
        complaint = string.Empty;

        for (var index = 0; index < args.Length; index++)
        {
            if (!string.Equals(args[index], "--native", StringComparison.Ordinal))
            {
                continue;
            }

            if (index + 1 >= args.Length)
            {
                complaint =
                    "--native wants the name of a backend; this build names " +
                    string.Join(", ", JsNativeBackends.Names);

                return false;
            }

            request = new JsCompileRequest(manifest, JsOutputForm.Native, args[index + 1]);
            return true;
        }

        return true;
    }

    /// <summary>Runs every named file and reports the worst answer any of them gave.</summary>
    private static int Run(
        IReadOnlyList<string> files,
        bool module,
        bool checkOnly,
        bool all,
        bool quiet,
        ulong? fuel,
        ulong? wall,
        int? depth,
        ulong? callDepth,
        ulong? liveBytes,
        int missing,
        bool slice,
        bool forceStrict,
        JsCompileRequest request)
    {
        // ONE FILE AND MANY FILES ARE REPORTED DIFFERENTLY, on purpose. Asked to run one program a
        // host should print what the program produced and nothing else, so its output can be piped.
        // Asked to run a tree it should print a row per file and a summary, because the useful
        // answer over a corpus is the distribution and not the last value.
        var single = files.Count == 1;
        var counts = new Dictionary<RunStatus, int>();
        var worst = missing == 0 ? RunStatus.Completed : RunStatus.Unreadable;

        foreach (var path in files)
        {
            var source = SourceFiles.Read(path);

            // A `.mjs` FILE IS A MODULE WHETHER OR NOT THE OPTION WAS PASSED, and a sweep over a
            // tree is where that matters: a directory holding both is one run, and the goal each
            // file is read under has to come from the file rather than from one flag covering all
            // of them.
            var asModule = module || SourceFiles.IsModulePath(path);

            var result = slice
                ? Host.Run(source, asModule, checkOnly, fuel, depth)
                : WideHost.Run(
                    [source], asModule, checkOnly, forceStrict, fuel, wall, depth, callDepth,
                    liveBytes, request);

            counts[result.Status] = counts.TryGetValue(result.Status, out var seen) ? seen + 1 : 1;

            if (ExitCodes.Rank(result.Status) > ExitCodes.Rank(worst))
            {
                worst = result.Status;
            }

            Report(path, result, single, all, quiet);
        }

        if (!single)
        {
            Summarise(files.Count, counts);
        }

        return ExitCodes.For(worst);
    }

    /// <summary>Prints what one file did.</summary>
    private static void Report(string path, RunResult result, bool single, bool all, bool quiet)
    {
        var shown = path.Replace('\\', '/');

        if (single)
        {
            if (result.Status == RunStatus.Completed && !quiet && result.Value.Length != 0)
            {
                Console.WriteLine(result.Value);
            }

            if (result.Status != RunStatus.Completed)
            {
                Console.Error.WriteLine($"broiler-js: {shown}: {result.Detail}");

                // EVERY REFUSAL RATHER THAN THE FIRST, when asked. The validation stage walks into
                // each construct it excludes instead of stopping at the first, so a file outside
                // this manifest has one diagnostic per occurrence - and a reader deciding what the
                // manifest would have to grow by needs all of them, not the earliest.
                if (all)
                {
                    foreach (var line in result.Diagnostics.Skip(1))
                    {
                        Console.Error.WriteLine($"           {line}");
                    }
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
            _ => "DEFECT  ",
        };

        var note = result.Status == RunStatus.Completed
            ? result.Value
            : result.Detail;

        Console.WriteLine($"{status} {shown}" + (note.Length == 0 ? string.Empty : "  " + note));

        if (all && result.Diagnostics.Count > 1)
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
        Console.WriteLine($"# {Host.Number(files)} files");

        foreach (var status in Enum.GetValues<RunStatus>())
        {
            if (counts.TryGetValue(status, out var count))
            {
                Console.WriteLine($"# {status}: {Host.Number(count)}");
            }
        }
    }

    /// <summary>Reads the instruction allowance, or says why the argument is not one.</summary>
    private static bool Depth(string[] args, out int? depth, out string complaint)
    {
        depth = null;
        complaint = string.Empty;
        var at = Array.IndexOf(args, "--max-depth");

        if (at < 0)
        {
            return true;
        }

        if (at == args.Length - 1)
        {
            complaint = "--max-depth needs a number of levels";
            return false;
        }

        if (!int.TryParse(args[at + 1], out var stated) ||
            stated < 1 ||
            stated > SliceParseOptions.MaximumSupportedNestingDepth)
        {
            complaint =
                $"`{args[at + 1]}` is not a depth between 1 and " +
                Host.Number(SliceParseOptions.MaximumSupportedNestingDepth);

            return false;
        }

        depth = stated;
        return true;
    }

    /// <summary>Reads the wall-clock allowance, or says why the argument is not one.</summary>
    private static bool Wall(string[] args, out ulong? wall, out string complaint)
    {
        wall = null;
        complaint = string.Empty;
        var at = Array.IndexOf(args, "--wall");

        if (at < 0)
        {
            return true;
        }

        if (at == args.Length - 1)
        {
            complaint = "--wall needs a number of milliseconds";
            return false;
        }

        if (!ulong.TryParse(args[at + 1], out var stated) || stated == 0)
        {
            complaint = $"`{args[at + 1]}` is not a positive number of milliseconds";
            return false;
        }

        wall = stated;
        return true;
    }

    /// <summary>Reads the instruction allowance, or says why the argument is not one.</summary>
    /// <summary>
    /// Reads <c>--call-depth</c>, the one ceiling a person measuring this interpreter has to move.
    /// </summary>
    /// <remarks>
    /// <b>It exists so that the call-depth ceiling can be a MEASUREMENT rather than an estimate.</b>
    /// Roadmap section 8 says the bound is measured and not chosen, and a bound nobody can raise
    /// from outside can only be measured by editing the profile — which measures whatever the
    /// editor happened to build. With this, `eng/measure-frame-cost.py` bisects the ceiling against
    /// the real binary on the real stack and reports the depth at which the answer stops being a
    /// named exhaustion.
    /// </remarks>
    private static bool CallDepth(string[] args, out ulong? callDepth, out string complaint)
    {
        callDepth = null;
        complaint = string.Empty;
        var at = Array.IndexOf(args, "--call-depth");

        if (at < 0)
        {
            return true;
        }

        if (at == args.Length - 1)
        {
            complaint = "--call-depth needs a number of frames";
            return false;
        }

        if (!ulong.TryParse(args[at + 1], out var stated) || stated == 0)
        {
            complaint = $"`{args[at + 1]}` is not a positive frame count";
            return false;
        }

        callDepth = stated;
        return true;
    }

    /// <summary>Reads <c>--live-bytes</c>, the allowance a workload's own working set needs.</summary>
    /// <remarks>
    /// <b>The other three ceilings a person meets are settable and this one was not, which made it
    /// the ceiling that decided what could be run.</b> A benchmark that allocates more than the
    /// profile's default holds is not a program this host may not run — it is a program run under
    /// an allowance nobody chose, and the difference shows up as a named exhaustion after a
    /// benchmark has already printed its score. Sizing a budget is the embedder's decision in every
    /// other dimension; there is no reason for memory to be the one an operator has to rebuild the
    /// profile to move. The profile's hard maximum still bounds it, so this widens what a caller
    /// may ask for and not what the profile permits.
    /// </remarks>
    private static bool LiveBytes(string[] args, out ulong? liveBytes, out string complaint)
    {
        liveBytes = null;
        complaint = string.Empty;
        var at = Array.IndexOf(args, "--live-bytes");

        if (at < 0)
        {
            return true;
        }

        if (at == args.Length - 1)
        {
            complaint = "--live-bytes needs a number of bytes";
            return false;
        }

        if (!ulong.TryParse(args[at + 1], out var stated) || stated == 0)
        {
            complaint = $"`{args[at + 1]}` is not a positive byte count";
            return false;
        }

        liveBytes = stated;
        return true;
    }

    private static bool Fuel(string[] args, out ulong? fuel, out string complaint)
    {
        fuel = null;
        complaint = string.Empty;
        var at = Array.IndexOf(args, "--fuel");

        if (at < 0)
        {
            return true;
        }

        if (at == args.Length - 1)
        {
            complaint = "--fuel needs a number of instructions";
            return false;
        }

        if (!ulong.TryParse(args[at + 1], out var stated) || stated == 0)
        {
            complaint = $"`{args[at + 1]}` is not a positive instruction count";
            return false;
        }

        fuel = stated;
        return true;
    }

    /// <summary>The options this host has, so an unknown one is refused rather than ignored.</summary>
    /// <remarks>
    /// A host that ignored an option it did not recognise would run under rules the caller did not
    /// ask for and report success. That is the same mistake the conformance harness's metadata
    /// reader makes a refusal, for the same reason.
    /// </remarks>
    private static readonly string[] Known =
    [
        "--module", "--check", "--all", "--quiet", "--fuel", "--max-depth", "--closure",
        "--slice", "--strict", "--sweep", "--wall", "--call-depth", "--live-bytes", "--help",
        "--version", "--numeric", "--native", "--host-surface",
    ];

    /// <summary>The closure this image actually has, read off its own loaded assemblies.</summary>
    /// <remarks>
    /// Every root here has this mode and rule K4 reads its output. It is what makes the
    /// <c>narrow-runtime-compiler</c> label checkable rather than asserted: the lowering has to be
    /// in this list and no test assembly may be.
    /// </remarks>
    private static int Closure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.JavaScript.Cli");

        // THE LABEL, AND THIS IS THE ONE ROOT ENTITLED TO IT. Its siblings print
        // `narrow-runtime-compiler-shaped`, because what they lower is a programmatic builder or a
        // fixture tree they also wrote. This root is handed a path by a person, so the source
        // surface the label names is a real one.
        Console.WriteLine("label narrow-runtime-compiler");
        Console.WriteLine("carries-lowering yes");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                JavaScriptProfile.Id,
                JavaScriptProfile.Descriptor.PackageIdentity.PackageId,
                JavaScriptProfile.Descriptor.DescriptorRevision,
                JavaScriptProfile.Descriptor.HostCapabilityDescriptors.Length));
        // EVERY MANIFEST THE DESCRIPTOR ACCEPTS, not the one this root happens to prefer. A
        // closure claim naming one while the image admits three would be read as the image
        // refusing the others, which is the opposite of true.
        Console.WriteLine(
            string.Join(
                ' ',
                "manifest",
                JavaScriptProfile.SliceManifest,
                JavaScriptProfile.WideManifest,
                JavaScriptProfile.NumericManifest));
        Console.WriteLine(
            string.Join(
                ' ',
                "format-versions",
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Min,
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Max));

        // The lowering, named from the assembly that actually carries it rather than declared. The
        // label above is a claim about this line: an execution-only image cannot print it, because
        // the type would not be there to ask.
        Console.WriteLine(
            string.Join(' ', "lowering", typeof(SliceSourceCompiler).Assembly.GetName().Name));

        return ExitCodes.Ok;
    }

    private static void Usage()
    {
        Console.WriteLine("broiler-js - run JavaScript on the Broiler.VM JavaScript profile");
        Console.WriteLine();
        Console.WriteLine("  broiler-js [options] <path>...");
        Console.WriteLine();
        Console.WriteLine("  <path>      a .js or .mjs file to run, or a directory swept for both");
        Console.WriteLine();
        Console.WriteLine("Several named files are run as separate scripts sharing ONE realm - which");
        Console.WriteLine("is what a benchmark harness and its benchmark, or a conformance harness and");
        Console.WriteLine("its test, need. They run in ORDINAL ORDER BY PATH and not in the order you");
        Console.WriteLine("named them, so two runs of the same set report the same rows in the same");
        Console.WriteLine("order and two transcripts are comparable; a harness that has to precede its");
        Console.WriteLine("benchmark has to be named so that it sorts first. A directory is swept");
        Console.WriteLine("instead, one realm per file, so one file's globals cannot decide the next");
        Console.WriteLine("one's result.");
        Console.WriteLine();
        Console.WriteLine("  --module    read each file under the module goal rather than the script goal");
        Console.WriteLine("              (a .mjs file is read as a module without it)");
        Console.WriteLine("  --strict    compile every script as strict-mode code");
        Console.WriteLine("  --sweep     run each named file in a realm of its own rather than sharing one");
        Console.WriteLine("  --slice     use the narrow broiler.javascript.slice surface instead");
        Console.WriteLine("  --numeric   use the broiler.javascript.numeric surface instead");
        Console.WriteLine("  --native <backend>");
        Console.WriteLine("              emit machine code beside the bytecode with the named backend.");
        Console.WriteLine($"              This build names three - {string.Join(", ", JsNativeBackends.Names)} -");
        Console.WriteLine("              and they are not equal. Both x86-64 conventions ENCODE REAL");
        Console.WriteLine("              INSTRUCTIONS, and the arming path takes exactly the one this");
        Console.WriteLine("              process's own platform uses: Windows x64 on Windows, System V");
        Console.WriteLine("              x64 elsewhere. A program compiled under that one is armed and");
        Console.WriteLine("              executed here and answers what the same source answers through");
        Console.WriteLine("              the interpreter; an artifact emitted for the other convention");
        Console.WriteLine("              verifies and then refuses to instantiate BY NAME rather than");
        Console.WriteLine("              falling back to the bytecode beside it. arm64-aapcs64 is");
        Console.WriteLine("              EMITTING-ONLY everywhere - nothing here arms an arm64 page - so");
        Console.WriteLine("              its bytes are compared against a retained expectation rather");
        Console.WriteLine("              than run, and it admits a narrower set of instructions than");
        Console.WriteLine("              either x86-64 backend and refuses the rest at compile time.");
        Console.WriteLine("              None of this implies anything about speed: no measurement of");
        Console.WriteLine("              any output form is retained anywhere in this repository.");
        Console.WriteLine("              (Corrected 2026-09-08. This entry read \"Every backend this");
        Console.WriteLine("              build carries refuses: no instruction of any architecture is");
        Console.WriteLine("              encoded here, so what this option produces is the backend");
        Console.WriteLine("              saying so.\" That was true when written, went false when the");
        Console.WriteLine("              encoders landed, and was corrected in this file's own --native");
        Console.WriteLine("              remarks on 2026-09-07 without ever reaching the text a user");
        Console.WriteLine("              sees. Understating a host is the same defect as overstating");
        Console.WriteLine("              one, so it is quoted here rather than quietly dropped.)");
        Console.WriteLine("  --check     compile and verify only; do not run");
        Console.WriteLine("  --all       report every refusal in a file rather than the first");
        Console.WriteLine("  --quiet     do not print the completion value");
        Console.WriteLine("  --fuel <n>  the instruction allowance per run; the profile's default otherwise");
        Console.WriteLine("  --wall <ms> the wall-clock allowance per run; the profile's 10,000 ms otherwise");
        Console.WriteLine("  --live-bytes <n> the live-memory allowance per run; the profile's default otherwise");
        Console.WriteLine("  --call-depth <n> the call-depth allowance per run, in frames; the profile's default otherwise");
        Console.WriteLine("  --max-depth <n>");
        Console.WriteLine("              the nesting depth the parser admits, 1 to 512; the parse options'");
        Console.WriteLine("              64 otherwise. ONE file of the Octane benchmark - earley-boyer -");
        Console.WriteLine("              nests deeper than 64 and is refused at the default, which is a");
        Console.WriteLine("              ceiling this build declares and not a statement about the");
        Console.WriteLine("              language. FIVE more - box2d, gbemu-part2, mandreel, pdfjs and");
        Console.WriteLine("              typescript-compiler - are refused at the default under the same");
        Console.WriteLine("              2103:NestingTooDeep code by a SECOND ceiling this option does");
        Console.WriteLine("              not reach: a tree deeper than the 10,000 levels the compiler");
        Console.WriteLine("              walks, which no command line moves. (Corrected 2026-09-08. This");
        Console.WriteLine("              entry read \"Two files of the Octane benchmark nest deeper than");
        Console.WriteLine("              64 and are refused at the default\"; --check over the pinned");
        Console.WriteLine("              checkout refuses six of its twenty-one files and only one of the");
        Console.WriteLine("              six meets the 64-level bound.)");
        Console.WriteLine("  --closure   print this composition's closure claim and exit");
        Console.WriteLine("  --version   print the profile and manifest identity");
        Console.WriteLine();
        Console.WriteLine("Exit codes: 0 completed, 1 threw, 2 usage, 3 source refused,");
        Console.WriteLine("            4 artifact refused, 5 allowance spent, 6 unreadable, 7 host defect.");
        Console.WriteLine("Over several files ONE code wins, and it is not the numerically largest:");
        Console.WriteLine("the order is by WHOSE FAULT the answer is. Highest first, it runs");
        Console.WriteLine("7, 4, 6, 5, 1, 3, 0 - host defect, then artifact refused, then unreadable,");
        Console.WriteLine("then allowance spent, then threw, then source refused, then completed. 4");
        Console.WriteLine("and 7 lead because both name a defect in this host rather than in the");
        Console.WriteLine("input, so a corpus sweep turning up one lowering defect among thousands of");
        Console.WriteLine("ordinary refusals reports the defect rather than averaging it away.");
        Console.WriteLine("(Corrected 2026-09-08. This read \"Over several files the worst code wins,");
        Console.WriteLine("and 4 and 7 outrank the rest because both name a defect in this host rather");
        Console.WriteLine("than in the input.\" The second clause was and is true; the first reads as");
        Console.WriteLine("the largest number and that is not what the ranking does. Two sweeps run on");
        Console.WriteLine("2026-09-08 settled it: an artifact refused beside a file that does not exist");
        Console.WriteLine("exits 4 and not 6, and a source refused beside a program that threw exits 1");
        Console.WriteLine("and not 3. The ranking is deliberate and was not changed; the sentence");
        Console.WriteLine("describing it was wrong.)");
        Console.WriteLine();
        Console.WriteLine("ONE CASE EXITS DIFFERENTLY HERE THAN IT DOES FROM THE SIBLING HOST, and a");
        Console.WriteLine("reader scripting either should learn it from whichever one they opened. An");
        Console.WriteLine("artifact emitted for a calling convention this image cannot arm - --numeric");
        Console.WriteLine("--native x86-64-sysv on a Windows process, or --native arm64-aapcs64");
        Console.WriteLine("anywhere - compiles, verifies, and then will not instantiate, and THIS host");
        Console.WriteLine("reports it as 4 while `broiler`, the polyglot host in");
        Console.WriteLine("src/compositions/Broiler.VM.Composition.PolyglotCli, reports 2 for the same");
        Console.WriteLine("file. That host has an eighth status this one does not - `Unroutable`, for a");
        Console.WriteLine("file it cannot route - and sends this case there, on the argument that a");
        Console.WriteLine("machine's architecture is not this component's own lowering defect. The");
        Console.WriteLine("argument is right and the mapping is deliberate on both sides; what was");
        Console.WriteLine("missing was anybody writing the difference down. Note in passing that the");
        Console.WriteLine("paragraph above glosses 4 as naming a defect in this host, which this one");
        Console.WriteLine("case is not - the code is wider here than its own gloss, because this host");
        Console.WriteLine("has no narrower code to put it under. A script driving both must accept 4 OR");
        Console.WriteLine("2 for it.");
        Console.WriteLine("(Recorded 2026-09-08. The difference was undocumented in both help texts");
        Console.WriteLine("until it was run: the same file, on this win-x64 checkout, under");
        Console.WriteLine("`broiler-js --numeric --native x86-64-sysv f.js` and `broiler run --numeric");
        Console.WriteLine("--native x86-64-sysv f.js`, exits 4 and 2. Neither mapping was changed to");
        Console.WriteLine("close the gap; the reciprocal note is in that host's own EXIT CODES");
        Console.WriteLine("paragraph, and the reasoning it cites lives in its JavaScriptLane.cs.)");
        Console.WriteLine();
        Console.WriteLine($"This host runs the feature manifest {JavaScriptProfile.WideManifest} by");
        Console.WriteLine("default: objects, arrays, strings, functions, closures, prototypes,");
        Console.WriteLine("classes with super and new.target, symbols, typed arrays, keyed");
        Console.WriteLine("collections, promises, regular expressions, template literals,");
        Console.WriteLine("destructuring, spread, for-of, generators, exceptions, for-in, switch,");
        Console.WriteLine("labels, async functions and await, async generators and for await, the");
        Console.WriteLine("whole of the class body - fields, private names, static blocks and");
        Console.WriteLine("generator members - Proxy, `with`, modules under --module or a .mjs name,");
        Console.WriteLine("and a standard library.");
        Console.WriteLine();
        Console.WriteLine("(Corrected 2026-09-08, and this is the FIRST of the three corrections this");
        Console.WriteLine("section took that day. The paragraph above, together with the measurement");
        Console.WriteLine("sentence further down, read \"It admits no async function, module, class");
        Console.WriteLine("field, private name, class static block, generator member of a class body,");
        Console.WriteLine("Proxy or BigInt. What has been measured against a suite is a handful of its");
        Console.WriteLine("subtrees, which measures those subtrees.\" Seven of the eight named");
        Console.WriteLine("constructs run to completion on the default manifest - point this host at");
        Console.WriteLine("each and read the answer - and the suite has been run whole since bundle");
        Console.WriteLine("jsw-10-001 was collected on 2026-09-05. A host whose own help text");
        Console.WriteLine("understates what it does is the same defect as one that overstates it, so");
        Console.WriteLine("the superseded reading is quoted rather than deleted.)");
        Console.WriteLine();
        Console.WriteLine("BIGINT IS ABSENT IN THREE PLACES AND ALL THREE ANSWER IF YOU ASK THEM.");
        Console.WriteLine("The `BigInt` global is not bound, so `typeof BigInt` answers `undefined`,");
        Console.WriteLine("and `BigInt64Array` and `BigUint64Array` are absent beside it. AND A");
        Console.WriteLine("BIGINT LITERAL IS REFUSED BY NAME AT COMPILE TIME: `1n` answers");
        Console.WriteLine("2104:ConstructOutsideManifest, `a BigInt literal is not admitted by the");
        Console.WriteLine("declared feature manifest`, under every manifest this host selects. Point");
        Console.WriteLine("this host at it and read the code rather than taking it from here.");
        Console.WriteLine();
        Console.WriteLine("(Recorded 2026-09-08, and this note exists because the sentence it");
        Console.WriteLine("replaces became true by accident hours after it was written. Earlier the");
        Console.WriteLine("same day this paragraph was corrected to read \"It admits no BigInt, and");
        Console.WriteLine("BigInt is the only construct this paragraph denies.\" THAT WAS FALSE ON");
        Console.WriteLine("THE DAY IT WAS WRITTEN. The wide front end ADMITTED a BigInt literal and");
        Console.WriteLine("evaluated it as a Number: `typeof 1n` answered \"number\", `1n === 1` was");
        Console.WriteLine("`true`, and `9007199254740993n` answered 9007199254740992 - a silently");
        Console.WriteLine("wrong integer, which --numeric --native x86-64-win64 then emitted machine");
        Console.WriteLine("code returning. Only --slice refused it. It is true now because the wide");
        Console.WriteLine("parser was fixed later that day to notice the `n` suffix the tokenizer had");
        Console.WriteLine("always left on the token, and NOT because anybody had checked the claim.");
        Console.WriteLine("The profile's own record had said so all along: see section 4.2, \"The");
        Console.WriteLine("refusal that was lost\", of");
        Console.WriteLine("src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md, which calls it");
        Console.WriteLine("the one finding in that document breaking a property this profile HAS");
        Console.WriteLine("rather than missing one it never had. The denial a reader meets from today");
        Console.WriteLine("is ENFORCED BY A REFUSAL rather than assumed, which is the whole of what");
        Console.WriteLine("this note is for. The word ONLY is not carried forward, because it was");
        Console.WriteLine("never true either: a decorator is refused under the same 2104 code, and an");
        Console.WriteLine("`accessor` class element and a `using` declaration are refused as syntax");
        Console.WriteLine("errors. BigInt is the absence this paragraph spells out, not the only one");
        Console.WriteLine("the manifest has.)");
        Console.WriteLine();
        Console.WriteLine("What has been measured against a suite is the whole of test262 at the");
        Console.WriteLine("pinned revision, retained in bundle jsw-10-001 - which measures that suite");
        Console.WriteLine("and says nothing about any other. THAT BUNDLE'S FIGURES DESCRIBE A RUN");
        Console.WriteLine("TAKEN BEFORE THE PARSER FIX ABOVE AND DO NOT DESCRIBE THIS BUILD. A");
        Console.WriteLine("whole-suite figure for this build is being re-taken and IS NOT STATED");
        Console.WriteLine("HERE; run eng/run-test262.py and read your own. Nothing here is reviewed,");
        Console.WriteLine("accepted or supported, and this host is advertised as nothing.");
        Console.WriteLine();
        Console.WriteLine("(Corrected 2026-09-08. The sentence above read that bundle jsw-10-001's");
        Console.WriteLine("\"wide run names NO unsupported family at all\". Restoring the BigInt");
        Console.WriteLine("refusal moves every variant reaching a BigInt literal out of the passing");
        Console.WriteLine("and failing columns and into the unsupported one, so the wide run's");
        Console.WriteLine("unsupported column is no longer empty and its passing total is SMALLER.");
        Console.WriteLine("Over test/language/literals/bigint alone the answer moved from 118");
        Console.WriteLine("variants - pass 101, fail 17, unsupported 0 - to 118 variants - pass 58,");
        Console.WriteLine("fail 0, unsupported 60: forty-three that passed by accident and seventeen");
        Console.WriteLine("that failed silently now meet a refusal naming the construct. A smaller");
        Console.WriteLine("pass total that is honest beats a larger one that is not, which is section");
        Console.WriteLine("1 of roadmap.parity.md and the rule that makes this a gain and not a");
        Console.WriteLine("regression.)");
        Console.WriteLine();
        Console.WriteLine($"--slice selects {JavaScriptProfile.SliceManifest} instead, which admits");
        Console.WriteLine("numbers, booleans, undefined, local bindings, the operators format");
        Console.WriteLine("version 1 has opcodes for and structured control flow, and nothing else.");
        Console.WriteLine();
        Console.WriteLine($"--numeric selects {JavaScriptProfile.NumericManifest}, which admits");
        Console.WriteLine("Number values, the arithmetic, comparison, bitwise and unary operators,");
        Console.WriteLine("let/const/var bindings of numbers, if/while/for/do and blocks, function");
        Console.WriteLine("declarations of numbers called by name, and return. It admits no object,");
        Console.WriteLine("string, closure, property access, exception or dynamic construct, and");
        Console.WriteLine("refuses each of them by name at compile time. It exists because a form");
        Console.WriteLine("other than bytecode has to be a whole-artifact form, and this is the");
        Console.WriteLine("language every program of which can be emitted in whole. It is smaller");
        Console.WriteLine("than JavaScript, and no claim is made that it is faster: no measurement");
        Console.WriteLine("of either output form is retained anywhere in this repository, under no");
        Console.WriteLine("predeclared rule and in no baseline register. (Corrected 2026-09-08. This");
        Console.WriteLine("sentence read \"It is smaller than JavaScript and it is not faster:");
        Console.WriteLine("nothing here emits machine code.\" Its final clause has been false since");
        Console.WriteLine("the encoders landed: --numeric --native emits machine code, and on this");
        Console.WriteLine("platform's own calling convention this host arms and executes it.)");
    }
}
