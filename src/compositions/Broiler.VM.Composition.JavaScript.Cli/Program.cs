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
                $"broiler-js {JavaScriptProfile.Id} manifest {JavaScriptProfile.SliceManifest} format 1");

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

        if (!Fuel(args, out var fuel, out var fuelComplaint))
        {
            Console.Error.WriteLine("broiler-js: " + fuelComplaint);
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

        return Run(files, module, checkOnly, all, quiet, fuel, depth, missing.Count);
    }

    /// <summary>Runs every named file and reports the worst answer any of them gave.</summary>
    private static int Run(
        IReadOnlyList<string> files,
        bool module,
        bool checkOnly,
        bool all,
        bool quiet,
        ulong? fuel,
        int? depth,
        int missing)
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
            var result = Host.Run(SourceFiles.Read(path), module, checkOnly, fuel, depth);

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
        "--help", "--version",
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
        Console.WriteLine(string.Join(' ', "manifest", JavaScriptProfile.SliceManifest));
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
        Console.WriteLine("  --module    read each file under the module goal rather than the script goal");
        Console.WriteLine("  --check     compile and verify only; do not run");
        Console.WriteLine("  --all       report every refusal in a file rather than the first");
        Console.WriteLine("  --quiet     do not print the completion value");
        Console.WriteLine("  --fuel <n>  the instruction allowance per file; the profile's default otherwise");
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
        Console.WriteLine($"This host accepts the feature manifest {JavaScriptProfile.SliceManifest},");
        Console.WriteLine("which admits numbers, booleans, undefined, local bindings, the operators");
        Console.WriteLine("this format has opcodes for and structured control flow - and admits no");
        Console.WriteLine("function, no object, no string value and no property access. It is NOT a");
        Console.WriteLine("JavaScript implementation and is advertised as nothing.");
    }
}
