// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>
/// The <c>broiler.webassembly</c> lane: a module's bytes, verified, instantiated and invoked.
/// </summary>
/// <remarks>
/// <para>
/// <b>NOTHING IS COMPILED IN THIS LANE, AND THAT IS A PROPERTY OF THE PROFILE RATHER THAN A CHOICE
/// THIS ROOT MADE.</b> The WebAssembly profile has no lowering anywhere in this repository: its
/// payload is a module produced by an external toolchain and read verbatim, so the file a person
/// names IS the artifact. The JavaScript lane beside it lowers source into an artifact and then
/// verifies it; this one verifies what it was handed. That asymmetry is why <c>compile</c> answers
/// a refusal for a <c>.wasm</c> file instead of copying it: a host that pretended to compile a
/// module would be claiming a lowering nothing here has.
/// </para>
/// <para>
/// <b>The entry-point encoding is the profile's own and this lane spells it in one place.</b> An
/// entry point is a length-prefixed name followed by one length-prefixed literal per argument, and
/// the harness root spells the same encoding for its own checks. Spelling it twice is what a
/// second host of the same profile costs; spelling it twice DIFFERENTLY would be a defect, so this
/// file states the shape it writes and the profile answers <c>UnknownExport</c> or a malformed
/// entry point when a host gets it wrong.
/// </para>
/// <para>
/// <b>What this lane does NOT do, named rather than left to be discovered.</b> It links nothing: a
/// module that declares an import is refused by the profile's validator, because imports are not
/// built. It reads no text format, so a <c>.wat</c> file is not a file this host has any answer
/// for. And it prints no memory: a module's linear memory is its own, and a host that dumped it
/// would be inventing an interface the profile does not publish.
/// </para>
/// </remarks>
internal static class WebAssemblyLane
{
    /// <summary>The identity this lane presents to the verifier.</summary>
    private const string Caller = "broiler-cli://webassembly";

    /// <summary>The export name this lane invokes when a caller names none and it exists.</summary>
    /// <remarks>
    /// Two names in preference order, and both are conventions rather than anything the format
    /// says. <c>_start</c> is what a WASI-targeting toolchain emits for a command module and
    /// <c>main</c> is what a person writing a module by hand tends to export; a module exporting
    /// neither, and more than one function, gets its exports listed rather than a guess.
    /// </remarks>
    private static readonly string[] ConventionalEntries = ["main", "_start"];

    /// <summary>Verifies, instantiates and invokes one module.</summary>
    internal static RunResult Run(
        InputFile file,
        bool checkOnly,
        string? entry,
        IReadOnlyList<string> arguments,
        Composition.Allowances allowances)
    {
        if (file.Problem.Length != 0)
        {
            return new RunResult(file.Status, string.Empty, file.Problem, []);
        }

        var created = VmRuntime.Create(
            Composition.Catalog(), Composition.Options(allowances, javascript: false, provider: null));

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
            return Run(runtime, file, checkOnly, entry, arguments);
        }
    }

    private static RunResult Run(
        VmRuntime runtime,
        InputFile file,
        bool checkOnly,
        string? entry,
        IReadOnlyList<string> arguments)
    {
        var descriptor = new VmArtifactDescriptor(
            WebAssemblyProfile.Id,
            1,
            WebAssemblyProfile.SliceManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(in descriptor, file.Bytes, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            if (verified.Outcome == VmOutcome.ResourceExhaustion)
            {
                return new RunResult(
                    RunStatus.Exhausted,
                    string.Empty,
                    $"verifying the module spent the allowance: {verified.Reason}",
                    []);
            }

            // A MODULE THIS HOST DID NOT PRODUCE IS A REFUSED SOURCE AND NOT A REFUSED ARTIFACT,
            // which is the one place the two lanes disagree about how to report the same core
            // answer. In the JavaScript lane a refused artifact accuses this component, because
            // this component's own lowering wrote the bytes. Here nothing in this image wrote
            // them: the file came from an external toolchain, so a refusal is a statement about
            // the input, and reporting it under the code that means "defect here" would send a
            // reader looking for a bug in a compiler this profile does not have.
            return new RunResult(
                RunStatus.RefusedSource,
                string.Empty,
                $"the module was refused: code {verified.Diagnostics.ProfileDiagnosticCode} " +
                $"({verified.Outcome}/{verified.Reason}) at section " +
                verified.Diagnostics.SourcePosition.SectionIndex.ToString(CultureInfo.InvariantCulture) +
                " offset " +
                verified.Diagnostics.SourcePosition.ByteOffset.ToString(CultureInfo.InvariantCulture),
                []);
        }

        using (artifact)
        {
            if (!artifact.TryGetState(out var state) || state is not WasmModule module)
            {
                return new RunResult(
                    RunStatus.HostDefect,
                    string.Empty,
                    "a verified artifact carried no decoded module, which is a defect here",
                    []);
            }

            var exported = FunctionExports(module);

            if (checkOnly)
            {
                return new RunResult(
                    RunStatus.Completed,
                    string.Empty,
                    string.Empty,
                    [.. exported.Select(static name => "export " + name)]);
            }

            var chosen = Choose(entry, exported, module, out var complaint);

            if (complaint.Length != 0)
            {
                return new RunResult(RunStatus.Unroutable, string.Empty, complaint, []);
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                if (instantiated.Outcome == VmOutcome.ResourceExhaustion)
                {
                    return new RunResult(
                        RunStatus.Exhausted,
                        string.Empty,
                        $"instantiating the module spent the allowance: {instantiated.Reason}",
                        []);
                }

                if (WebAssemblyProfile.TryGetTrap(in instantiated, out var startTrap))
                {
                    // A START FUNCTION THAT TRAPS IS THE PROGRAM FAULTING AND NOT THE MODULE BEING
                    // REFUSED. It ran; it trapped; the module was well-formed enough to run.
                    return new RunResult(
                        RunStatus.Faulted,
                        string.Empty,
                        $"the start function trapped: {startTrap.Kind} ({startTrap.DiagnosticCode})",
                        []);
                }

                return new RunResult(
                    RunStatus.RefusedArtifact,
                    string.Empty,
                    "the module verified and would not instantiate: " +
                    $"({instantiated.Outcome}/{instantiated.Reason})",
                    []);
            }

            // NO EXPORT TO CALL IS NOT A FAILURE WHEN A START FUNCTION RAN. The module's own
            // entry point is the start section, instantiation is where it runs, and it has run by
            // the time this line is reached.
            if (chosen.Length == 0)
            {
                return new RunResult(
                    RunStatus.Completed,
                    string.Empty,
                    string.Empty,
                    ["the start function ran during instantiation and the module exports no function"]);
            }

            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(EntryPoint(chosen, arguments))));

            var answered = instance.Invoke(in request, CancellationToken.None);

            return Report(answered, chosen, exported);
        }
    }

    private static RunResult Report(
        in VmInvocationResult answered, string entry, IReadOnlyList<string> exported)
    {
        if (answered.Outcome == VmOutcome.ResourceExhaustion)
        {
            return new RunResult(
                RunStatus.Exhausted,
                string.Empty,
                $"`{entry}` did not settle within its allowance: {answered.Reason} on " +
                answered.Diagnostics.ExhaustedDimension.ToString(),
                []);
        }

        if (WebAssemblyProfile.TryGetTrap(in answered, out var trap))
        {
            return new RunResult(
                RunStatus.Faulted,
                string.Empty,
                $"`{entry}` trapped: {trap.Kind} ({trap.DiagnosticCode})",
                []);
        }

        if (WebAssemblyProfile.TryGetEntryPointFault(in answered, out var fault))
        {
            return new RunResult(
                RunStatus.Unroutable,
                string.Empty,
                $"`{entry}` is not an entry point this module answers: {fault.Problem}. " +
                (exported.Count == 0
                    ? "The module exports no function."
                    : "It exports " + string.Join(", ", exported) + "."),
                []);
        }

        if (!WebAssemblyProfile.TryGetResults(in answered, out var results))
        {
            return new RunResult(
                RunStatus.HostDefect,
                string.Empty,
                $"the invocation answered {answered.Outcome}/{answered.Reason} and carried no payload",
                []);
        }

        var rendered = new List<string>(results.Count);

        for (var index = 0; index < results.Count; index++)
        {
            if (results.TryGetValue(index, out var value))
            {
                rendered.Add(Describe(value));
            }
        }

        return new RunResult(RunStatus.Completed, string.Join(' ', rendered), string.Empty, []);
    }

    /// <summary>Renders one result the way a person reading a terminal wants to see it.</summary>
    /// <remarks>
    /// <b>A float is printed round-trippable and NOT rounded.</b> A host that printed
    /// <c>0.1 + 0.2</c> as <c>0.3</c> would be reporting a number the module did not produce,
    /// which is the whole reason the harness root compares bit patterns rather than text.
    /// </remarks>
    private static string Describe(WebAssemblyValue value) => value.Kind switch
    {
        WebAssemblyValueKind.I32 => value.AsInt32.ToString(CultureInfo.InvariantCulture),
        WebAssemblyValueKind.I64 => value.AsInt64.ToString(CultureInfo.InvariantCulture),
        WebAssemblyValueKind.F32 => value.AsSingle.ToString("R", CultureInfo.InvariantCulture),
        _ => value.AsDouble.ToString("R", CultureInfo.InvariantCulture),
    };

    /// <summary>Every function this module exports, in the order the module publishes them.</summary>
    private static IReadOnlyList<string> FunctionExports(WasmModule module)
    {
        var names = new List<string>(module.ExportCount);

        for (var index = 0; index < module.ExportCount; index++)
        {
            if (!module.TryDescribeExport(index, out var kind, out _, out var nameLength) ||
                kind != WasmExportKind.Function)
            {
                continue;
            }

            var buffer = new byte[nameLength];

            if (module.TryCopyExportName(index, buffer, out var written) && written == nameLength)
            {
                names.Add(System.Text.Encoding.UTF8.GetString(buffer));
            }
        }

        return names;
    }

    /// <summary>
    /// Decides which export to invoke, or says why the caller has to.
    /// </summary>
    /// <remarks>
    /// <b>A GUESS AMONG SEVERAL IS NOT MADE, AND THAT IS THE POINT OF THIS METHOD.</b> A module
    /// exporting one function has an unambiguous entry point and a module exporting `main` names
    /// one by convention; a module exporting six names none, and picking the first would run
    /// whichever function the toolchain happened to emit first. The caller is told what the module
    /// exports and asked to say which, because a host that guessed would sometimes be right and
    /// would never say which case it was in.
    /// </remarks>
    private static string Choose(
        string? asked, IReadOnlyList<string> exported, WasmModule module, out string complaint)
    {
        complaint = string.Empty;

        if (asked is { Length: > 0 })
        {
            return asked;
        }

        foreach (var conventional in ConventionalEntries)
        {
            if (exported.Contains(conventional, StringComparer.Ordinal))
            {
                return conventional;
            }
        }

        if (exported.Count == 1)
        {
            return exported[0];
        }

        // A start function is the module's own entry point, and a module that has one and exports
        // nothing callable has already done everything it was going to do by the time it is
        // instantiated.
        if (exported.Count == 0)
        {
            return string.Empty;
        }

        complaint =
            $"exports {exported.Count.ToString(CultureInfo.InvariantCulture)} functions and none " +
            "named `main` or `_start`, so this host declines to guess which one you meant. Name " +
            "one with --invoke: " + string.Join(", ", exported) +
            (module.StartFunctionIndex >= 0
                ? ". Its start function has already run."
                : string.Empty);

        return string.Empty;
    }

    /// <summary>
    /// The entry-point string the profile reads: a length-prefixed name, then the arguments.
    /// </summary>
    /// <remarks>
    /// The length is in BYTES of UTF-8 and not in characters, which is the distinction a name
    /// outside ASCII makes visible and a host spelling it by hand gets wrong.
    /// </remarks>
    private static string EntryPoint(string name, IReadOnlyList<string> arguments) =>
        string.Concat(
            System.Text.Encoding.UTF8.GetByteCount(name).ToString(CultureInfo.InvariantCulture),
            ":",
            name,
            string.Concat(arguments));

    /// <summary>
    /// Reads one <c>--arg</c> as the profile's argument encoding, or says why it is not one.
    /// </summary>
    /// <remarks>
    /// <b>A float is taken as a DECIMAL and written as a bit pattern.</b> The encoding the profile
    /// reads is hexadecimal bits, because a decimal literal has a rounding question in it and an
    /// entry point is not the place to answer one; a person typing a command line has decimals, so
    /// the conversion happens here, once, and the exact bits it produced are what the argument
    /// carries.
    /// </remarks>
    internal static bool TryReadArgument(string argument, out string encoded, out string complaint)
    {
        encoded = string.Empty;
        complaint = string.Empty;

        var split = argument.IndexOf(':', StringComparison.Ordinal);

        if (split <= 0 || split == argument.Length - 1)
        {
            complaint =
                $"`{argument}` is not an argument; write i32:<n>, i64:<n>, f32:<x> or f64:<x>";

            return false;
        }

        var type = argument[..split];
        var literal = argument[(split + 1)..];

        switch (type)
        {
            case "i32" when int.TryParse(literal, CultureInfo.InvariantCulture, out var i32):
                encoded = Encode("i32", i32.ToString(CultureInfo.InvariantCulture));
                return true;

            case "i64" when long.TryParse(literal, CultureInfo.InvariantCulture, out var i64):
                encoded = Encode("i64", i64.ToString(CultureInfo.InvariantCulture));
                return true;

            case "f32" when float.TryParse(literal, CultureInfo.InvariantCulture, out var f32):
                encoded = Encode(
                    "f32",
                    System.BitConverter.SingleToUInt32Bits(f32).ToString("X8", CultureInfo.InvariantCulture));

                return true;

            case "f64" when double.TryParse(literal, CultureInfo.InvariantCulture, out var f64):
                encoded = Encode(
                    "f64",
                    System.BitConverter.DoubleToUInt64Bits(f64).ToString("X16", CultureInfo.InvariantCulture));

                return true;

            default:
                complaint =
                    $"`{argument}` is not an argument this host can encode; the types are i32, " +
                    "i64, f32 and f64, and the literal has to parse as one";

                return false;
        }
    }

    private static string Encode(string type, string literal) =>
        $"{type}:{literal.Length.ToString(CultureInfo.InvariantCulture)}:{literal}";
}
