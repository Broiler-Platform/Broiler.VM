// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The value form's rows (decision JSD-0035, stage JSV-1): what the artifact records, what the verifier
/// and the scan hold a payload to, and whether a program answers in the value form what it answers in
/// bytecode - with and without handle-stress.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE AGREEMENT ROWS RUN THE SAME PROGRAMS AS THE BASELINE FORM'S</b>, the wide programs and the
/// probes of <see cref="NativeAbiChecks"/>, each once in bytecode and once in the value form, and compare
/// the whole answer: the completion, what was printed, how the run ended, and the fuel it consumed, which
/// the value form charges per instruction exactly where the interpreter does. A program that loads code
/// verifies a larger payload, and verification is charged to fuel, so its figure is reported and not
/// compared, as the baseline rows do.
/// </para>
/// <para>
/// <b>THE STRESS ROWS RUN THEM AGAIN UNDER HANDLE-STRESS</b>, where every helper call compacts the
/// instance's handle table and every decoded word is compared with the interpreter's value, so a rooting or
/// codec mistake on any of them fails the row by name.
/// </para>
/// <para>
/// A machine that is not x86-64 enters no emitted code; its execution rows are reported not run, and the
/// rows about bytes, headers and refusals run everywhere.
/// </para>
/// </remarks>
internal static class ValueFormChecks
{
    /// <summary>The fuel every agreement row is run under.</summary>
    private const ulong Fuel = 50_000_000;

    /// <summary>Runs every value-form row.</summary>
    internal static List<(string Name, bool Passed, string Detail)> Run()
    {
        Program.InitializeNativeMapping();
        var rows = new List<(string, bool, string)>();

        rows.AddRange(TheHeaderFormByte());
        rows.AddRange(TheValueFormIsTheWideManifestsAlone());

        foreach (var abi in JsX64Abi.Rows)
        {
            rows.Add(EveryUnitIsEmittedAndReEmitted(abi));
            rows.AddRange(TheScanHoldsEachFormToItsOwnPartition(abi));
        }

        if (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture !=
            System.Runtime.InteropServices.Architecture.X64)
        {
            rows.Add((
                "not-run/value/execution",
                false,
                "this machine is not x86-64, so no value-form emitted code was entered and nothing the " +
                "agreement and stress rows assert is claimed"));

            return rows;
        }

        rows.AddRange(TheValueFormAnswersAsBytecode(JsX64Abi.Host, handleStress: false));
        rows.AddRange(TheValueFormAnswersAsBytecode(JsX64Abi.Host, handleStress: true));
        return rows;
    }

    /// <summary>The form byte: written for the value form alone, and read back only when it is one this build names.</summary>
    private static List<(string, bool, string)> TheHeaderFormByte()
    {
        var rows = new List<(string, bool, string)>();

        foreach (var architecture in new[] { JsNativeArchitecture.X64SystemV, JsNativeArchitecture.X64Windows, JsNativeArchitecture.Arm64 })
        {
            var plain = JsNativeCodeHeader.Pack(architecture, valueForm: false);
            var value = JsNativeCodeHeader.Pack(architecture, valueForm: true);

            var plainRead = JsNativeCodeHeader.TryUnpack(plain, out var plainArchitecture, out var plainValue);
            var valueRead = JsNativeCodeHeader.TryUnpack(value, out var valueArchitecture, out var valueValue);

            rows.Add((
                "value/header/the-form-byte-round-trips/" + architecture,
                plain == (uint)architecture && plainRead && plainArchitecture == architecture && !plainValue &&
                    valueRead && valueArchitecture == architecture && valueValue && value != plain,
                "the manifest's own form writes " + plain + " and the value form " + value));
        }

        // A FORM BYTE NAMING THE BASELINE TIER OR ANY OTHER, AND ANY BIT ABOVE IT, IS REFUSED: one way to
        // write each fact, and no reader guessing at a field it cannot split.
        foreach (var (name, field) in new[]
                 {
                     ("the-baseline-tier-spelled-out", (uint)JsNativeArchitecture.X64SystemV | (1u << 8)),
                     ("a-form-byte-no-tier-names", (uint)JsNativeArchitecture.X64SystemV | (7u << 8)),
                     ("a-bit-above-the-form-byte", (uint)JsNativeArchitecture.X64SystemV | (1u << 16)),
                 })
        {
            rows.Add((
                "value/header/refuses/" + name,
                !JsNativeCodeHeader.TryUnpack(field, out _, out _),
                "the field " + field + " is " + (JsNativeCodeHeader.TryUnpack(field, out _, out _) ? "read" : "refused")));
        }

        return rows;
    }

    /// <summary>The compiler refuses the value form under the numeric manifest, and the verifier refuses a value byte beside it.</summary>
    private static List<(string, bool, string)> TheValueFormIsTheWideManifestsAlone()
    {
        var rows = new List<(string, bool, string)>();

        var numericValue = JsCompiler.Compile(
            [new JsScriptUnit("numeric.js", "1 + 2;", SliceParseOptions.Script)],
            [],
            new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Value, JsNativeBackends.X64SystemV));

        rows.Add((
            "value/refuses/the-value-form-under-the-numeric-manifest-at-compilation",
            !numericValue.Succeeded && numericValue.Diagnostics.Count > 0,
            numericValue.Diagnostics.Count > 0 ? numericValue.Diagnostics[0].ToString() : "it compiled"));

        // A NUMERIC ARTIFACT WHOSE HEADER IS PATCHED TO NAME THE VALUE FORM: well-framed, and a form its
        // manifest has none of, so the verifier refuses the header before it chooses a table.
        var numeric = JsCompiler.Compile(
            [new JsScriptUnit("numeric.js", "1 + 2;", SliceParseOptions.Script)],
            [],
            new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Native, JsNativeBackends.X64SystemV));

        if (numeric.Artifact is null || !TryPatchFormByte(numeric.Artifact, out var patched))
        {
            rows.Add(("value/refuses/a-value-byte-beside-the-numeric-manifest", false, "the numeric artifact did not emit"));
            return rows;
        }

        var answer = NativeLifecycle.Run(patched, JsOutputForm.Native, JsNativeBackends.X64SystemV);

        rows.Add((
            "value/refuses/a-value-byte-beside-the-numeric-manifest",
            answer.Contains("the verifier refused", StringComparison.Ordinal) &&
                answer.Contains(((int)JavaScriptDiagnosticCode.MalformedNativeSection).ToString(System.Globalization.CultureInfo.InvariantCulture), StringComparison.Ordinal),
            answer));

        return rows;
    }

    /// <summary>Every wide program is emitted in the value form, and a verifier that re-emits admits it byte for byte.</summary>
    private static (string, bool, string) EveryUnitIsEmittedAndReEmitted(JsX64Abi abi)
    {
        var label = "value/re-emission-is-byte-identical/" + abi.Name;
        var answers = new List<string>();

        foreach (var (name, source, library) in NativeAbiChecks.WidePrograms)
        {
            var compiled = NativeLifecycle.CompileWide(source, JsOutputForm.Value, abi.Name, library);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return (label, false, name + " did not compile in the value form");
            }

            var verified = NativeLifecycle.VerifiesWithReEmission(compiled.Artifact, abi, library is not null);

            if (verified.Length != 0)
            {
                return (label, false, name + ": " + verified);
            }

            answers.Add(name);
        }

        return (label, true, answers.Count + " programs verified against their own re-emission");
    }

    /// <summary>A value payload is held to the value partition, and neither form's payload passes as the other's.</summary>
    private static List<(string, bool, string)> TheScanHoldsEachFormToItsOwnPartition(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();
        var source = NativeAbiChecks.WidePrograms[4].Source;
        var value = NativeLifecycle.CompileWide(source, JsOutputForm.Value, abi.Name);
        var baseline = NativeLifecycle.CompileWide(source, JsOutputForm.Native, abi.Name);

        if (value.Artifact is null || baseline.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(value.Artifact, out var valueCode, out var valueSymbols, out _) ||
            !NativeLifecycle.TryReadEmitted(baseline.Artifact, out var baselineCode, out var baselineSymbols, out _) ||
            !NativeLifecycle.TryReadImage(value.Artifact, out var image, out _))
        {
            rows.Add(("value/scan/" + abi.Name, false, "a program did not emit"));
            return rows;
        }

        foreach (var (name, tier, code, symbols, accepted) in new[]
                 {
                     ("a-value-payload-under-the-value-tier", JsNativeTier.Value, valueCode, valueSymbols, true),
                     ("a-value-payload-under-the-baseline-tier", JsNativeTier.Baseline, valueCode, valueSymbols, false),
                     ("a-baseline-payload-under-the-value-tier", JsNativeTier.Value, baselineCode, baselineSymbols, false),
                 })
        {
            var result = JsNativeScan.Scan(abi.Architecture, tier, code, symbols, 16, image with { Tier = tier });

            rows.Add((
                "value/scan/" + name + "/" + abi.Name,
                result.Accepted == accepted,
                result.Outcome + " at " + result.Offset + ": " + result.Reason));
        }

        return rows;
    }

    /// <summary>Every wide program and probe answers in the value form what it answers in bytecode.</summary>
    private static List<(string, bool, string)> TheValueFormAnswersAsBytecode(JsX64Abi abi, bool handleStress)
    {
        var rows = new List<(string, bool, string)>();
        var prefix = handleStress ? "value/under-handle-stress/answers-as-bytecode/" : "value/answers-as-bytecode/";

        var programs = NativeAbiChecks.WidePrograms.Select(static program => (program.Name, program.Source, program.Library, GuestLoads: false))
            .Concat(NativeAbiChecks.Probes.Select(static probe => (probe.Name, probe.Source, Library: (string?)null, probe.GuestLoads)));

        foreach (var (name, source, library, guestLoads) in programs)
        {
            var interpreted = NativeLifecycle.RunWide(source, JsOutputForm.Bytecode, string.Empty, Fuel, library);

            var value = NativeLifecycle.RunWide(
                source, JsOutputForm.Value, abi.Name, Fuel, library, handleStress: handleStress);

            var same =
                string.Equals(interpreted.Outcome, value.Outcome, StringComparison.Ordinal) &&
                string.Equals(interpreted.Completion, value.Completion, StringComparison.Ordinal) &&
                string.Equals(interpreted.TypeOf, value.TypeOf, StringComparison.Ordinal) &&
                string.Equals(interpreted.ErrorName, value.ErrorName, StringComparison.Ordinal) &&
                string.Equals(interpreted.ErrorMessage, value.ErrorMessage, StringComparison.Ordinal) &&
                string.Equals(interpreted.Printed, value.Printed, StringComparison.Ordinal) &&
                (guestLoads || interpreted.Fuel == value.Fuel);

            rows.Add((
                prefix + name,
                same && interpreted.Completed,
                "bytecode " + interpreted.Outcome + " `" + interpreted.Completion + "` fuel " + interpreted.Fuel +
                "; value " + value.Outcome + " `" + value.Completion + "` fuel " + value.Fuel +
                (guestLoads ? " (fuel reported, not compared: the program loads code)" : string.Empty)));
        }

        return rows;
    }

    /// <summary>Sets the value form's byte in an artifact's emitted-code header, in place of a zero one.</summary>
    private static bool TryPatchFormByte(byte[] artifact, out byte[] patched)
    {
        patched = (byte[])artifact.Clone();
        var at = NativeLifecycle.NativeCodeBody(artifact);

        if (at < 0 || patched[at + 1] != 0)
        {
            return false;
        }

        patched[at + 1] = (byte)JsNativeTier.Value;
        return true;
    }
}
