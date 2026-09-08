// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Globalization;

namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>Everything one invocation of a subcommand was asked for.</summary>
/// <remarks>
/// <para>
/// <b>READ ONCE, INTO A RECORD, RATHER THAN SCANNED WHEREVER A VALUE IS WANTED.</b> The host this
/// root's code comes from asks <c>args.Contains("--quiet")</c> at the point of use, which is
/// readable while the option list is short and stops being checkable when a grammar arrives: an
/// option meaningful for one subcommand and not another is a fact nothing holds. Reading the whole
/// command line once means an option is either understood or refused, in one place, before any
/// file is opened.
/// </para>
/// <para>
/// <b>AN OPTION THIS HOST DOES NOT HAVE IS REFUSED AND NEVER IGNORED.</b> A host that ignored an
/// unrecognised option would run under rules the caller did not ask for and then report success,
/// which is the one failure a script driving it cannot detect.
/// </para>
/// </remarks>
internal sealed record Options
{
    /// <summary>The files and directories named, in the order the caller named them.</summary>
    internal IReadOnlyList<string> Paths { get; init; } = [];

    /// <summary>Where <c>compile</c> writes its artifact, or empty where none was named.</summary>
    internal string Output { get; init; } = string.Empty;

    /// <summary>Read every source under the module goal.</summary>
    internal bool Module { get; init; }

    /// <summary>Compile every script as strict-mode code.</summary>
    internal bool Strict { get; init; }

    /// <summary>Give every file a run of its own rather than sharing a realm.</summary>
    internal bool Sweep { get; init; }

    /// <summary>Compile and verify only.</summary>
    internal bool CheckOnly { get; init; }

    /// <summary>Report every refusal rather than the first.</summary>
    internal bool All { get; init; }

    /// <summary>Do not print the completion value.</summary>
    internal bool Quiet { get; init; }

    /// <summary>The WebAssembly export to call, or null where the caller named none.</summary>
    internal string? Invoke { get; init; }

    /// <summary>The arguments to that export, already in the profile's own encoding.</summary>
    internal IReadOnlyList<string> Arguments { get; init; } = [];

    /// <summary>The nesting depth the JavaScript parser admits, or null for its own default.</summary>
    internal int? MaximumDepth { get; init; }

    /// <summary>What the caller asked of each budget dimension.</summary>
    internal Composition.Allowances Allowances { get; init; } = new(null, null, null, null);

    /// <summary>The manifest and output form the JavaScript lowering is asked for.</summary>
    internal JsCompileRequest Request { get; init; } = new();

    /// <summary>Reads a command line, or says why it is not one this host understands.</summary>
    internal static bool TryRead(string[] args, out Options options, out string complaint)
    {
        options = new Options();
        complaint = string.Empty;

        var paths = new List<string>();
        var arguments = new List<string>();
        var output = string.Empty;
        var invoke = (string?)null;
        var module = false;
        var strict = false;
        var sweep = false;
        var check = false;
        var all = false;
        var quiet = false;
        var numeric = false;
        var backend = string.Empty;
        int? depth = null;
        ulong? fuel = null;
        ulong? wall = null;
        ulong? callDepth = null;
        ulong? liveBytes = null;

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];

            switch (argument)
            {
                case "--module":
                    module = true;
                    continue;

                case "--strict":
                    strict = true;
                    continue;

                case "--sweep":
                    sweep = true;
                    continue;

                case "--check":
                    check = true;
                    continue;

                case "--all":
                    all = true;
                    continue;

                case "--quiet":
                    quiet = true;
                    continue;

                case "--numeric":
                    numeric = true;
                    continue;
            }

            if (!argument.StartsWith("--", StringComparison.Ordinal) &&
                !string.Equals(argument, "-o", StringComparison.Ordinal))
            {
                paths.Add(argument);
                continue;
            }

            if (index + 1 >= args.Length)
            {
                complaint = $"{argument} needs a value after it";
                return false;
            }

            var value = args[++index];

            switch (argument)
            {
                case "-o" or "--output":
                    output = value;
                    continue;

                case "--invoke":
                    invoke = value;
                    continue;

                case "--arg":
                    if (!WebAssemblyLane.TryReadArgument(value, out var encoded, out var why))
                    {
                        complaint = why;
                        return false;
                    }

                    arguments.Add(encoded);
                    continue;

                case "--native":
                    if (!JsNativeBackends.Names.Contains(value, StringComparer.Ordinal))
                    {
                        complaint =
                            $"`{value}` is not a backend this build names; they are " +
                            string.Join(", ", JsNativeBackends.Names);

                        return false;
                    }

                    backend = value;
                    continue;

                case "--max-depth":
                    if (!int.TryParse(value, CultureInfo.InvariantCulture, out var levels) ||
                        levels < 1 ||
                        levels > SliceParseOptions.MaximumSupportedNestingDepth)
                    {
                        complaint =
                            $"`{value}` is not a depth between 1 and " +
                            SliceParseOptions.MaximumSupportedNestingDepth.ToString(
                                CultureInfo.InvariantCulture);

                        return false;
                    }

                    depth = levels;
                    continue;

                case "--fuel":
                    if (!Positive(value, out var instructions))
                    {
                        complaint = $"`{value}` is not a positive instruction count";
                        return false;
                    }

                    fuel = instructions;
                    continue;

                case "--wall":
                    if (!Positive(value, out var milliseconds))
                    {
                        complaint = $"`{value}` is not a positive number of milliseconds";
                        return false;
                    }

                    wall = milliseconds;
                    continue;

                case "--call-depth":
                    if (!Positive(value, out var frames))
                    {
                        complaint = $"`{value}` is not a positive frame count";
                        return false;
                    }

                    callDepth = frames;
                    continue;

                case "--live-bytes":
                    if (!Positive(value, out var bytes))
                    {
                        complaint = $"`{value}` is not a positive byte count";
                        return false;
                    }

                    liveBytes = bytes;
                    continue;

                default:
                    complaint = $"`{argument}` is not an option this host has";
                    return false;
            }
        }

        // A NATIVE FORM EXISTS FOR ONE MANIFEST AND THE HOST SAYS SO RATHER THAN LETTING THE FRONT
        // END SAY IT. `--native` without `--numeric` would be lowered and refused with a
        // construct-outside-manifest diagnostic naming whatever the program's first object or
        // string happened to be, which reads as a complaint about the program and is a complaint
        // about the command line. The whole-artifact rule is the reason: there is no per-unit
        // choice and no fallback, so the manifest a native artifact is emitted for is decided
        // before a byte is read.
        if (backend.Length != 0 && !numeric)
        {
            complaint =
                "--native emits machine code for the " + JavaScriptProfile.NumericManifest +
                " surface and there is no native form of any other; pass --numeric with it";

            return false;
        }

        var manifest = numeric ? JsFeatureManifest.Numeric : JsFeatureManifest.Wide;

        options = new Options
        {
            Paths = paths,
            Output = output,
            Module = module,
            Strict = strict,
            Sweep = sweep,
            CheckOnly = check,
            All = all,
            Quiet = quiet,
            Invoke = invoke,
            Arguments = arguments,
            MaximumDepth = depth,
            Allowances = new Composition.Allowances(fuel, wall, callDepth, liveBytes),
            Request = backend.Length == 0
                ? new JsCompileRequest(manifest)
                : new JsCompileRequest(manifest, JsOutputForm.Native, backend),
        };

        return true;
    }

    private static bool Positive(string value, out ulong parsed) =>
        ulong.TryParse(value, CultureInfo.InvariantCulture, out parsed) && parsed != 0;
}
