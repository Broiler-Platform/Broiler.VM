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
/// and <c>broiler.javascript.slice</c> admits no function, no object, no string value and no
/// property access. Pointed at real-world JavaScript this host refuses almost every file - by
/// name, with the construct named, which is the useful part - and a support claim over that would
/// be untruthful.
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
            // written it down. It names the missing archive too, because a version string that
            // implied a fully taken pin would be the overclaim this whole record is against.
            Console.WriteLine("edition " + JavaScriptLanguageEdition.Describe());

            return ExitCodes.Ok;
        }

        if (args.Contains("--closure", StringComparer.Ordinal))
        {
            return Closure();
        }

        var module = args.Contains("--module", StringComparer.Ordinal);
        var checkOnly = args.Contains("--check", StringComparer.Ordinal);
        var all = args.Contains("--all", StringComparer.Ordinal);
        var quiet = args.Contains("--quiet", StringComparer.Ordinal);
        var slice = args.Contains("--slice", StringComparer.Ordinal);
        var forceStrict = args.Contains("--strict", StringComparer.Ordinal);
        var sweep = args.Contains("--sweep", StringComparer.Ordinal);

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

        var paths = new List<string>();

        for (var index = 0; index < args.Length; index++)
        {
            if (string.Equals(args[index], "--fuel", StringComparison.Ordinal) ||
                string.Equals(args[index], "--wall", StringComparison.Ordinal) ||
                string.Equals(args[index], "--max-depth", StringComparison.Ordinal))
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
                depth);
            Report(string.Join(' ', files), joined, single: true, all, quiet);
            return ExitCodes.For(joined.Status);
        }

        return Run(files, module, checkOnly, all, quiet, fuel, wall, depth, missing.Count, slice, forceStrict);
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
        int missing,
        bool slice,
        bool forceStrict)
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
                : WideHost.Run([source], asModule, checkOnly, forceStrict, fuel, wall, depth);

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
        "--slice", "--strict", "--sweep", "--wall", "--help", "--version",
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
        // closure claim naming one while the image admits two would be read as the image
        // refusing the other, which is the opposite of true.
        Console.WriteLine(
            string.Join(
                ' ',
                "manifest",
                JavaScriptProfile.SliceManifest,
                JavaScriptProfile.WideManifest));
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
        Console.WriteLine("  <path>      a .js file to run, or a directory swept for .js files");
        Console.WriteLine();
        Console.WriteLine("Several named files are run as separate scripts sharing ONE realm, in the");
        Console.WriteLine("order given - which is what a benchmark harness and its benchmark, or a");
        Console.WriteLine("conformance harness and its test, need. A directory is swept instead, one");
        Console.WriteLine("realm per file, so one file's globals cannot decide the next one's result.");
        Console.WriteLine();
        Console.WriteLine("  --module    read each file under the module goal rather than the script goal");
        Console.WriteLine("              (a .mjs file is read as a module without it)");
        Console.WriteLine("  --strict    compile every script as strict-mode code");
        Console.WriteLine("  --sweep     run each named file in a realm of its own rather than sharing one");
        Console.WriteLine("  --slice     use the narrow broiler.javascript.slice surface instead");
        Console.WriteLine("  --check     compile and verify only; do not run");
        Console.WriteLine("  --all       report every refusal in a file rather than the first");
        Console.WriteLine("  --quiet     do not print the completion value");
        Console.WriteLine("  --fuel <n>  the instruction allowance per run; the profile's default otherwise");
        Console.WriteLine("  --wall <ms> the wall-clock allowance per run; the profile's 10,000 ms otherwise");
        Console.WriteLine("  --max-depth <n>");
        Console.WriteLine("              the nesting depth the parser admits; the parse options' 64 otherwise.");
        Console.WriteLine("              Two files of the Octane benchmark nest deeper than 64 and are refused");
        Console.WriteLine("              at the default - which is a ceiling this build declares and not a");
        Console.WriteLine("              statement about the language.");
        Console.WriteLine("  --closure   print this composition's closure claim and exit");
        Console.WriteLine("  --version   print the profile and manifest identity");
        Console.WriteLine();
        Console.WriteLine("Exit codes: 0 completed, 1 threw, 2 usage, 3 source refused,");
        Console.WriteLine("            4 artifact refused, 5 allowance spent, 6 unreadable, 7 host defect.");
        Console.WriteLine("Over several files the worst code wins, and 4 and 7 outrank the rest");
        Console.WriteLine("because both name a defect in this host rather than in the input.");
        Console.WriteLine();
        Console.WriteLine($"This host runs the feature manifest {JavaScriptProfile.WideManifest} by");
        Console.WriteLine("default: objects, arrays, strings, functions, closures, prototypes,");
        Console.WriteLine("exceptions, for-in, switch, labels and a standard library. It admits no");
        Console.WriteLine("class, generator, async function, module, destructuring, spread, template");
        Console.WriteLine("literal, for-of, Proxy, Symbol, BigInt or typed array, and no eval or");
        Console.WriteLine("Function constructor. What has been measured against a conformance suite");
        Console.WriteLine("is a handful of its subtrees, which measures those subtrees. Nothing here");
        Console.WriteLine("is reviewed, accepted or supported, and this host is advertised as");
        Console.WriteLine("nothing.");
        Console.WriteLine();
        Console.WriteLine($"--slice selects {JavaScriptProfile.SliceManifest} instead, which admits");
        Console.WriteLine("numbers, booleans, undefined, local bindings, the operators format");
        Console.WriteLine("version 1 has opcodes for and structured control flow, and nothing else.");
    }
}
