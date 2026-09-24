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
    /// <summary>
    /// Installs the native page mapper before anything in this process can ask for a native run.
    /// </summary>
    /// <remarks>
    /// A static constructor rather than a line in <see cref="Main"/>, because the type initializer
    /// runs before any member of this type does - including an entry point a future host, a test
    /// harness or a trimmed shim reaches by some other route. An install that lived in
    /// <c>Main</c> alone would be an install that a second entry point silently does without, and
    /// the failure it produces is a refusal by name rather than a crash, which is exactly the
    /// shape that survives a gate.
    /// </remarks>
    static Program()
    {
        InitializeNativeMapping();
    }

    /// <summary>
    /// Fills <see cref="JsNativePage.Mapper"/> with the arming path this image links.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the whole of what makes <c>--native</c> a run rather than a refusal in this
    /// process.</b> The profile assembly declares no platform invoke and maps nothing; it asks the
    /// composition for a page through this hook, and <c>JsExecution</c> refuses to instantiate a
    /// native artifact when the hook answers null - deliberately, at instantiation, so a process
    /// that may not make memory executable says so by name instead of faulting its first call.
    /// With the hook unfilled, <c>--native x86-64-win64</c> on a Windows x64 process answered
    /// <c>ProfileFault/UnsatisfiedHostAssumption</c>, which is the same sentence this host prints
    /// for an artifact emitted for somewhere else - so the refusal a caller was meant to read as
    /// "you asked for a backend this machine does not arm" was also what it got for the one
    /// backend this machine does arm.
    /// </para>
    /// <para>
    /// <b>Copied from the slice-compiler root rather than shared.</b> Four lines of delegate
    /// plumbing in each root that links the arming path is the cost of the rule that keeps
    /// <c>Broiler.VM.Profile.MachineCode</c> out of the profile assembly's own reference set; a
    /// shared helper would have to live somewhere both roots can see, and the only such place is
    /// a product assembly, which is the edge this arrangement exists to avoid.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=TBF
    // Broiler-Human:        PENDING
    internal static unsafe void InitializeNativeMapping()
    {
        JsNativePage.Mapper = static code =>
        {
            var page = Broiler.VM.Profile.MachineCode.VmNativePage.TryMap(code);
            return page is null ? null : new JsNativePage(
                page,
                () => page.Arm(),
                offset => page.At(offset),
                offset => (nint)page.Entry(offset));
        };
    }

    private static int Main(string[] args)
    {
        InitializeNativeMapping();
        WriteUtf8();

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

    /// <summary>
    /// Makes each standard stream that is redirected to a pipe or a file write UTF-8 without a
    /// byte-order mark, whatever code page this process inherited; a stream that is a console
    /// window keeps the runtime's console writer.
    /// </summary>
    /// <remarks>
    /// THE BYTES THIS HOST WRITES ARE PART OF WHAT IT PROMISES, the same way the bytes it reads
    /// are (JSD-0017 section 3). The runtime's default writer encodes with the console's output
    /// code page, so on a machine whose console uses code page 850 a guest's <c>"é"</c> left as a
    /// different byte and <c>"∛"</c> left as <c>?</c> - a different answer on a different machine,
    /// which no retained answer can be compared against. The writers are replaced rather than
    /// <see cref="Console.OutputEncoding"/> being set, because setting it changes the code page of
    /// the console window itself, which outlives this process and is not this host's to change.
    /// Both writers flush on every write so the two streams interleave as they did before.
    /// <para>
    /// <b>A console window is left alone, because it is read by a person rather than compared.</b>
    /// The raw stream under a console writes bytes that the window decodes with its own code page,
    /// so UTF-8 there showed <c>"é"</c> as <c>"├®"</c> on a code page 850 console where the default
    /// writer had shown it correctly. The rule is therefore per stream: redirected streams carry
    /// deterministic UTF-8, and an interactive console keeps the runtime's writer and whatever that
    /// console can display (JSD-0017 section 3).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=TBF
    // Broiler-Falsified-If: a guest's non-ASCII text reaches a redirected stream as bytes other than its UTF-8 encoding
    // Broiler-Human:        PENDING
    private static void WriteUtf8()
    {
        var utf8 = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        if (Console.IsOutputRedirected)
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput(), utf8) { AutoFlush = true });
        }

        if (Console.IsErrorRedirected)
        {
            Console.SetError(new StreamWriter(Console.OpenStandardError(), utf8) { AutoFlush = true });
        }
    }

    private static int Dispatch(string[] args)
    {
        // THE EFFECTIVE CONFIGURATION, BEFORE THIS PROCESS DOES ANYTHING ELSE, and it is not a
        // mode: every other argument is still answered afterwards, so the line can be asked for
        // beside the work rather than instead of it. It goes to STANDARD ERROR because a run whose
        // completion value, closure claim or report is read off standard output has to carry it
        // without disturbing that.
        if (args.Contains("--runtime", StringComparer.Ordinal))
        {
            Runtime();
        }

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

        // HANDLE-STRESS IS A PROPERTY OF THE RUNTIME THIS HOST BUILDS, not of the artifact: it changes
        // nothing but how often a value-form instance's handle table compacts (JSD-0035 section 4).
        var handleStress = args.Contains("--handle-stress", StringComparer.Ordinal);

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

        if (!Bytes(args, "--artifact-bytes", out var artifactBytes, out var artifactComplaint) ||
            !Bytes(args, "--nested-load-bytes", out var nestedLoadBytes, out artifactComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + artifactComplaint);
            return ExitCodes.Usage;
        }

        var loads = new LoadAllowances(artifactBytes, nestedLoadBytes);

        var paths = new List<string>();

        for (var index = 0; index < args.Length; index++)
        {
            if (string.Equals(args[index], "--fuel", StringComparison.Ordinal) ||
                string.Equals(args[index], "--native", StringComparison.Ordinal) ||
                string.Equals(args[index], "--value", StringComparison.Ordinal) ||
                string.Equals(args[index], "--wall", StringComparison.Ordinal) ||
                string.Equals(args[index], "--max-depth", StringComparison.Ordinal) ||
                string.Equals(args[index], "--call-depth", StringComparison.Ordinal) ||
                string.Equals(args[index], "--live-bytes", StringComparison.Ordinal) ||
                string.Equals(args[index], "--artifact-bytes", StringComparison.Ordinal) ||
                string.Equals(args[index], "--nested-load-bytes", StringComparison.Ordinal))
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
                request,
                handleStress,
                loads);
            Report(string.Join(' ', files), joined, single: true, all, quiet);
            return ExitCodes.For(joined.Status);
        }

        return Run(
            files, module, checkOnly, all, quiet, fuel, wall, depth, callDepth, liveBytes,
            missing.Count, slice, forceStrict, request, handleStress, loads);
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
    /// <b><c>--native</c> WITHOUT <c>--numeric</c> ASKS FOR THE BASELINE FORM OVER THE WIDE
    /// SURFACE, AND THIS HOST PASSES IT THROUGH.</b> <i>(Added 2026-09-15.)</i> Every unit of the
    /// wide artifact is emitted as x86-64 machine code whose control flow between blocks of
    /// instructions is emitted and which runs each block by one call into the interpreter's own
    /// dispatch, so a program this host runs in bytecode it also runs in that form. The same
    /// request reaches this host's source provider, because an instance has one form and a
    /// guest-loaded program of the other form is refused as a defect. The arm64 backend emits only
    /// for <c>--numeric</c>.
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
            // `--value` ASKS FOR THE WIDE MANIFEST'S VALUE FORM (JSD-0035), named by backend exactly as
            // `--native` names the baseline form's; the compiler refuses it beside `--numeric`.
            var form = args[index] switch
            {
                "--native" => JsOutputForm.Native,
                "--value" => JsOutputForm.Value,
                _ => JsOutputForm.Bytecode,
            };

            if (form == JsOutputForm.Bytecode)
            {
                continue;
            }

            if (index + 1 >= args.Length)
            {
                complaint =
                    args[index] + " wants the name of a backend; this build names " +
                    string.Join(", ", JsNativeBackends.Names);

                return false;
            }

            request = new JsCompileRequest(manifest, form, args[index + 1]);
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
        JsCompileRequest request,
        bool handleStress,
        LoadAllowances loads)
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
                    liveBytes, request, handleStress, loads);

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

    /// <summary>Reads a byte allowance named by <paramref name="option"/>, when the caller stated one.</summary>
    /// <remarks>
    /// <b>For <c>--artifact-bytes</c> and <c>--nested-load-bytes</c>, the two allowances an artifact's own
    /// size is charged to</b>, for <c>--live-bytes</c>'s reason: a form whose artifacts are larger than
    /// another's - the value form calls a helper per instruction - meets these ceilings on programs the
    /// other form runs, and moving them is the caller's decision to state rather than the profile's to
    /// rebuild. The profile's hard maxima still bound both.
    /// </remarks>
    private static bool Bytes(string[] args, string option, out ulong? bytes, out string complaint)
    {
        bytes = null;
        complaint = string.Empty;
        var at = Array.IndexOf(args, option);

        if (at < 0)
        {
            return true;
        }

        if (at == args.Length - 1 || !ulong.TryParse(args[at + 1], out var stated) || stated == 0)
        {
            complaint = option + " needs a positive number of bytes";
            return false;
        }

        bytes = stated;
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
        "--version", "--numeric", "--native", "--host-surface", "--runtime", "--value",
        "--handle-stress", "--artifact-bytes", "--nested-load-bytes",
    ];

    /// <summary>
    /// Prints the runtime configuration this process actually got, on standard error.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>EFFECTIVE, NOT REQUESTED.</b> Two timed runs are comparable only when both ran under the
    /// same runtime, and the settings that decide that - the garbage collector's mode, tiered
    /// compilation, tiered PGO - are taken from the environment and from the runtime configuration.
    /// Neither is visible in the command line that started the process, so a harness recording the
    /// arguments alone would record what it asked for rather than what it ran under.
    /// </para>
    /// <para>
    /// <b>Tiering is read from BOTH sources, because the runtime takes it from both.</b> An
    /// environment variable does not reach <c>AppContext</c>, so a process started under
    /// <c>DOTNET_TieredCompilation=0</c> answers the switch as absent and a line reading only the
    /// switch would report the default; a <c>runtimeconfig</c> setting does not reach the
    /// environment either. Both are printed under their own names. <b>Neither of them is the tier
    /// state itself</b> - no runtime API reports that - so what these name is what was asked of the
    /// runtime, which is the limit this option has and the one a bundle citing it must state.
    /// </para>
    /// <para>
    /// The pattern is <c>src/tests/Broiler.VM.Bench.Host/Program.cs</c>, which prints the same GC
    /// and runtime-identifier facts at the head of a bench transcript for the same reason.
    /// </para>
    /// <para>
    /// <b><c>native-arming</c> is the one field here whose value is not the machine's.</b>
    /// <i>(Added 2026-09-23.)</i> Every other field reports something the process was started with
    /// and differs between machines, so the acceptance suite reads only their keys. This one
    /// reports whether <see cref="JsNativePage.Mapper"/> was filled, which is a property of what
    /// was composed into this image: <c>installed</c> on every platform and in every publish mode
    /// where <see cref="InitializeNativeMapping"/> ran, and <c>none</c> in an image that links no
    /// arming path or never reaches the one it links. It is here because between 2026-09-18 and
    /// 2026-09-23 this host printed nothing that distinguished those two states, and the way the
    /// difference showed was that every <c>--native</c> run verified its artifact and then refused
    /// to instantiate it. It reports what this process holds and claims nothing about speed, about
    /// which conventions are armed, or about whether any particular artifact will run.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=TBF
    // Broiler-Falsified-If: the line reports a setting other than the one the running process has
    // Broiler-Human:        PENDING
    private static void Runtime() =>
        Console.Error.WriteLine(
            $"runtime-identifier={System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier} " +
            $"process-architecture={System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture} " +
            $"gc-server={System.Runtime.GCSettings.IsServerGC} " +
            $"gc-latency={System.Runtime.GCSettings.LatencyMode} " +
            $"tiered-compilation-env={Asked("TieredCompilation")} " +
            $"tiered-compilation-config={Configured("System.Runtime.TieredCompilation")} " +
            $"tiered-pgo-env={Asked("TieredPGO")} " +
            $"tiered-pgo-config={Configured("System.Runtime.TieredPGO")} " +
            $"dynamic-code={System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeCompiled} " +
            $"native-arming={(JsNativePage.Mapper is null ? "none" : "installed")}");

    /// <summary>What the environment asks of the runtime for one knob, or <c>unset</c>.</summary>
    /// <remarks>
    /// Both prefixes, newest first: the runtime reads <c>DOTNET_</c> and still honours the older
    /// <c>COMPlus_</c>, so a line reading one of them would print <c>unset</c> for a process that
    /// had in fact been configured through the other. An empty value is not a request and reads as
    /// unset, which is how the runtime's own parse treats it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=TBF
    // Broiler-Falsified-If: a variable the runtime reads for this knob is set and this answers `unset`
    // Broiler-Human:        PENDING
    private static string Asked(string knob)
    {
        var current = Environment.GetEnvironmentVariable("DOTNET_" + knob);

        if (!string.IsNullOrEmpty(current))
        {
            return current;
        }

        var legacy = Environment.GetEnvironmentVariable("COMPlus_" + knob);

        return string.IsNullOrEmpty(legacy) ? "unset" : legacy;
    }

    /// <summary>What the runtime configuration states for one switch, or <c>unset</c>.</summary>
    /// <remarks>
    /// <c>AppContext</c> carries what <c>runtimeconfig.json</c>, the host and the publish
    /// properties set, and nothing the environment set. It is the second half of the same question
    /// and not a substitute for the first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=TBF
    // Broiler-Falsified-If: the switch is present in this process's configuration and this answers `unset`
    // Broiler-Human:        PENDING
    private static string Configured(string name) =>
        AppContext.GetData(name) is { } value ? value.ToString() ?? "unset" : "unset";

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
        Console.WriteLine("              Without --numeric it asks for the baseline form over the wide");
        Console.WriteLine("              surface: every unit emitted, each block of instructions one call");
        Console.WriteLine("              into the interpreter's own dispatch, the control flow between");
        Console.WriteLine("              blocks emitted, and eval and import() compiled the same way. The");
        Console.WriteLine("              arm64 backend emits only with --numeric.");
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
        Console.WriteLine("  --value <backend>");
        Console.WriteLine("              emit the wide surface's VALUE FORM (decision JSD-0035) with the");
        Console.WriteLine("              named x86-64 backend: every instruction one call of a helper that");
        Console.WriteLine("              runs the interpreter's own arm over NaN-boxed words in a pinned");
        Console.WriteLine("              slab, the control flow between them emitted, and eval and import()");
        Console.WriteLine("              compiled the same way. Refused with --numeric and by arm64.");
        Console.WriteLine("              It implies nothing about speed either.");
        Console.WriteLine("  --handle-stress");
        Console.WriteLine("              run a value-form program with its handle table compacting at every");
        Console.WriteLine("              helper call and every decoded word compared with the interpreter's");
        Console.WriteLine("              value, so a rooting mistake is an internal defect by name; it");
        Console.WriteLine("              changes nothing for any other form");
        Console.WriteLine("  --check     compile and verify only; do not run");
        Console.WriteLine("  --all       report every refusal in a file rather than the first");
        Console.WriteLine("  --quiet     do not print the completion value");
        Console.WriteLine("  --fuel <n>  the instruction allowance per run; the profile's default otherwise");
        Console.WriteLine("  --wall <ms> the wall-clock allowance per run; the profile's 10,000 ms otherwise");
        Console.WriteLine("  --live-bytes <n> the live-memory allowance per run; the profile's default otherwise");
        Console.WriteLine("  --call-depth <n> the call-depth allowance per run, in frames; the profile's default otherwise");
        Console.WriteLine("  --artifact-bytes <n> the allowance an artifact's size is charged to; the profile's default otherwise");
        Console.WriteLine("  --nested-load-bytes <n> the allowance guest-loaded artifacts' sizes are charged to; the profile's default otherwise");
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
        Console.WriteLine("  --runtime   print the runtime configuration this process ACTUALLY GOT to");
        Console.WriteLine("              standard error, before anything else, and then carry on with");
        Console.WriteLine("              the other options: the runtime identifier and process");
        Console.WriteLine("              architecture, the garbage collector's mode and latency,");
        Console.WriteLine("              tiered compilation and tiered PGO as BOTH the environment and");
        Console.WriteLine("              the runtime configuration state them, and whether dynamic code");
        Console.WriteLine("              is compiled. Tiering is read from both sources because the");
        Console.WriteLine("              runtime takes it from both and neither shows the other's");
        Console.WriteLine("              setting; NEITHER IS THE TIER STATE ITSELF, which no runtime");
        Console.WriteLine("              API reports, so what is printed is what was asked of the");
        Console.WriteLine("              runtime rather than what it did. The line ends with");
        Console.WriteLine("              native-arming, which is the one field on it whose value is");
        Console.WriteLine("              not this machine's: `installed` if this image filled the");
        Console.WriteLine("              profile's page-mapping hook and `none` if it did not, which");
        Console.WriteLine("              is the difference between --native running a program and");
        Console.WriteLine("              refusing every one of them. (Added 2026-09-23, because this");
        Console.WriteLine("              host spent five days printing nothing that told the two");
        Console.WriteLine("              apart.) It says nothing about which conventions are armed.");
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
        Console.WriteLine("BIGINT IS ADMITTED BY THE DEFAULT MANIFEST, THROUGH ITS OWN SURFACE.");
        Console.WriteLine("`1n` is an exact integer, the `BigInt` global and `BigInt.prototype` are");
        Console.WriteLine("bound, and the operators, conversions and comparisons are the language's");
        Console.WriteLine("(JSeal B01-B05, decision JSD-0033). A program holding a BigInt literal or");
        Console.WriteLine("naming `BigInt` declares broiler.javascript.bigint, which a composition");
        Console.WriteLine("may decline. `BigInt64Array`, `BigUint64Array` and the DataView BigInt");
        Console.WriteLine("accessors exist wherever that surface is admitted (JSeal B07-B08); naming");
        Console.WriteLine("either constructor declares it beside broiler.javascript.binary. --numeric");
        Console.WriteLine("still refuses a BigInt literal by name, 2104:ConstructOutsideManifest.");
        Console.WriteLine();
        Console.WriteLine("(Corrected 2026-09-22. This paragraph said the BigInt typed arrays and the");
        Console.WriteLine("DataView BigInt accessors were \"STILL ABSENT (cards B07-B08)\"; they were");
        Console.WriteLine("added by those cards.)");
        Console.WriteLine();
        Console.WriteLine("(Corrected 2026-09-21. This paragraph read \"BIGINT IS ABSENT IN THREE");
        Console.WriteLine("PLACES AND ALL THREE ANSWER IF YOU ASK THEM\": the global unbound and a");
        Console.WriteLine("literal refused by name under every manifest this host selects. Both");
        Console.WriteLine("stopped being true when card B05 admitted the surface; the typed arrays");
        Console.WriteLine("are the one place of the three that still answers `undefined`.)");
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
        Console.WriteLine("(Recorded 2026-09-15. The wide surface has a native form of its own since");
        Console.WriteLine("then - the baseline form, under --native without --numeric - so this");
        Console.WriteLine("manifest is no longer the only language that can be emitted in whole. It");
        Console.WriteLine("remains the one whose instructions are emitted as computation rather than");
        Console.WriteLine("as calls into the interpreter.)");
    }
}
