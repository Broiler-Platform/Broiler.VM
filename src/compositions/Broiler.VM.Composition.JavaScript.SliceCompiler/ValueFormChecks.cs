// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The value form's rows (decision JSD-0035, stages JSV-1 to JSV-3): what the artifact records, what the
/// verifier and the scan hold a payload to, whether a program answers in the value form what it answers
/// in bytecode - with and without handle-stress, and with every binding non-resident - whether every
/// inline template answers what its arm answers, whether the fuel-parity twins give one verdict at
/// every ceiling, and whether direct calls - every reached <c>Call</c> a direct call site - answer as the
/// interpreter's calls do, up to the counted bound's <c>RangeError</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE AGREEMENT ROWS RUN THE SAME PROGRAMS AS THE BASELINE FORM'S</b>, the wide programs and the
/// probes of <see cref="NativeAbiChecks"/>, each once in bytecode and once in the value form, and compare
/// the whole answer: the completion, what was printed, how the run ended, and the fuel it consumed, which
/// the value form charges for every instruction the interpreter charges for - a helper's per instruction,
/// an inline template's as debt settled before the next helper runs. A program that loads code verifies a
/// larger payload, and verification is charged to fuel, so its figure is reported and not compared, as
/// the baseline rows do.
/// </para>
/// <para>
/// <b>THE STRESS ROWS RUN THEM AGAIN UNDER HANDLE-STRESS</b>, where every helper call compacts the
/// instance's handle table, so a word naming a handle nothing roots fails the row by name; and the flat
/// rows run them in the value form with every binding classed non-resident, the control the residency
/// analysis is held to.
/// </para>
/// <para>
/// <b>THE DIFFERENTIAL ROWS RUN EVERY INLINE TEMPLATE AGAINST ITS ARM</b> over edge values - both zeros,
/// the infinities, NaNs with payloads, the thirty-two-bit boundaries and their neighbours, and every kind
/// that is not a Number, so each guard's slow path runs too - and compare every result's bits. A row
/// counts which inline kinds the programs actually placed, so a template no program reaches fails
/// rather than passing unexercised.
/// </para>
/// <para>
/// <b>THE FUEL-PARITY ROWS RUN EACH TWIN AT EVERY CEILING WHERE THE DEBT COULD TELL</b> - every ceiling
/// from one settlement window below the total to a few above it, and a sweep below that - in bytecode,
/// the value form and its flat control, and compare the verdicts: the outcome kind, the exhausted
/// dimension, the completion and what was printed. The consumed figure at exhaustion is JSD-0035
/// section 7's named divergence and is not compared; the total is.
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

    /// <summary>How many ceilings the parity rows sweep below the window around the total.</summary>
    private const int Sweep = 64;

    /// <summary>
    /// How far below a twin's total the parity rows try every ceiling: the most debt that can be
    /// outstanding at any point, a threshold's worth on a back edge and a run's worth after it, and slack.
    /// </summary>
    private const int Window = JsValueLayout.DebtThreshold + JsValueLayout.StraightLineRun + 64;

    /// <summary>Runs every value-form row.</summary>
    internal static List<(string Name, bool Passed, string Detail)> Run()
    {
        Program.InitializeNativeMapping();
        var rows = new List<(string, bool, string)>();

        rows.AddRange(TheHeaderFormByte());
        rows.AddRange(TheValueFormIsTheWideManifestsAlone());

        foreach (var abi in JsX64Abi.Rows)
        {
            rows.Add(EveryUnitIsEmittedAndReEmitted(abi, JsOutputForm.Value));
            rows.Add(EveryUnitIsEmittedAndReEmitted(abi, JsOutputForm.ValueFlat));
            rows.AddRange(TheScanHoldsEachFormToItsOwnPartition(abi));
            rows.AddRange(TheScanNamesTheClauseAPayloadBreaks(abi));
        }

        rows.Add(TheDifferentialProgramsPlaceEveryInlineKind(JsX64Abi.Host));
        rows.Add(EveryReachedCallIsADirectCallSite(JsX64Abi.Host));

        if (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture !=
            System.Runtime.InteropServices.Architecture.X64)
        {
            rows.Add((
                "not-run/value/execution",
                false,
                "this machine is not x86-64, so no value-form emitted code was entered and nothing the " +
                "agreement, stress, differential and parity rows assert is claimed"));

            return rows;
        }

        rows.Add(AFlippedResidencyBitIsRefused(JsX64Abi.Host));
        rows.AddRange(TheValueFormAnswersAsBytecode(JsX64Abi.Host, JsOutputForm.Value, handleStress: false));
        rows.AddRange(TheValueFormAnswersAsBytecode(JsX64Abi.Host, JsOutputForm.Value, handleStress: true));
        rows.AddRange(TheValueFormAnswersAsBytecode(JsX64Abi.Host, JsOutputForm.ValueFlat, handleStress: false));
        rows.AddRange(TheDirectCallsAnswerAsBytecode(JsX64Abi.Host));
        rows.AddRange(EveryInlineTemplateAnswersAsItsArm(JsX64Abi.Host));
        rows.AddRange(TheTwinsGiveOneVerdictAtEveryCeiling(JsX64Abi.Host));
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
            var flat = JsNativeCodeHeader.Pack(architecture, valueForm: true, residentBindings: false);
            var baselineFlat = JsNativeCodeHeader.Pack(architecture, valueForm: false, residentBindings: false);

            var plainRead = JsNativeCodeHeader.TryUnpack(plain, out var plainArchitecture, out var plainValue, out var plainResident);
            var valueRead = JsNativeCodeHeader.TryUnpack(value, out var valueArchitecture, out var valueValue, out var valueResident);
            var flatRead = JsNativeCodeHeader.TryUnpack(flat, out var flatArchitecture, out var flatValue, out var flatResident);

            rows.Add((
                "value/header/the-form-byte-round-trips/" + architecture,
                plain == (uint)architecture && plainRead && plainArchitecture == architecture && !plainValue && plainResident &&
                    valueRead && valueArchitecture == architecture && valueValue && valueResident && value != plain,
                "the manifest's own form writes " + plain + " and the value form " + value));

            // THE FLAT CONTROL IS THE VALUE FORM WITH ONE BIT MORE, and the bit means nothing beside any
            // other form, so a baseline header is written the same with or without it.
            rows.Add((
                "value/header/the-flat-control-round-trips/" + architecture,
                flatRead && flatArchitecture == architecture && flatValue && !flatResident &&
                    flat == (value | (JsNativeCodeHeader.FlatBit << JsNativeCodeHeader.FormShift)) && baselineFlat == plain,
                "the flat control writes " + flat + ", and a baseline header asked for no residency writes " + baselineFlat));
        }

        // A FORM BYTE NAMING THE BASELINE TIER OR ANY OTHER, AND ANY BIT ABOVE IT, IS REFUSED: one way to
        // write each fact, and no reader guessing at a field it cannot split. The flat bit is refused
        // beside anything but the value tier's number.
        foreach (var (name, field) in new[]
                 {
                     ("the-baseline-tier-spelled-out", (uint)JsNativeArchitecture.X64SystemV | (1u << 8)),
                     ("a-form-byte-no-tier-names", (uint)JsNativeArchitecture.X64SystemV | (7u << 8)),
                     ("a-bit-above-the-form-byte", (uint)JsNativeArchitecture.X64SystemV | (1u << 16)),
                     ("the-flat-bit-alone", (uint)JsNativeArchitecture.X64SystemV | (JsNativeCodeHeader.FlatBit << 8)),
                     ("the-flat-bit-beside-the-baseline-tier", (uint)JsNativeArchitecture.X64SystemV | ((JsNativeCodeHeader.FlatBit | 1u) << 8)),
                 })
        {
            rows.Add((
                "value/header/refuses/" + name,
                !JsNativeCodeHeader.TryUnpack(field, out _, out _, out _),
                "the field " + field + " is " + (JsNativeCodeHeader.TryUnpack(field, out _, out _, out _) ? "read" : "refused")));
        }

        return rows;
    }

    /// <summary>The compiler refuses the value form under the numeric manifest, and the verifier refuses a value byte beside it.</summary>
    private static List<(string, bool, string)> TheValueFormIsTheWideManifestsAlone()
    {
        var rows = new List<(string, bool, string)>();

        foreach (var (form, name) in new[] { (JsOutputForm.Value, "value"), (JsOutputForm.ValueFlat, "flat-value") })
        {
            var numericValue = JsCompiler.Compile(
                [new JsScriptUnit("numeric.js", "1 + 2;", SliceParseOptions.Script)],
                [],
                new JsCompileRequest(JsFeatureManifest.Numeric, form, JsNativeBackends.X64SystemV));

            rows.Add((
                "value/refuses/the-" + name + "-form-under-the-numeric-manifest-at-compilation",
                !numericValue.Succeeded && numericValue.Diagnostics.Count > 0,
                numericValue.Diagnostics.Count > 0 ? numericValue.Diagnostics[0].ToString() : "it compiled"));
        }

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

    /// <summary>Every wide program is emitted in a value form, and a verifier that re-emits admits it byte for byte.</summary>
    private static (string, bool, string) EveryUnitIsEmittedAndReEmitted(JsX64Abi abi, JsOutputForm form)
    {
        var label = (form == JsOutputForm.ValueFlat ? "value-flat" : "value") + "/re-emission-is-byte-identical/" + abi.Name;
        var answers = new List<string>();

        foreach (var (name, source, library) in NativeAbiChecks.WidePrograms)
        {
            var compiled = NativeLifecycle.CompileWide(source, form, abi.Name, library);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return (label, false, name + " did not compile in the " + form + " form");
            }

            var verified = NativeLifecycle.VerifiesWithReEmission(compiled.Artifact, abi, library is not null, form);

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

    /// <summary>A program whose one function keeps bindings resident, guards Numbers and loops.</summary>
    private const string ResidentLoop =
        "function f(n, k) { var s = 0; for (var i = 0; i < n; i++) { s = s + i * 2.5; } return s + k; }\n" +
        "f(10, 1);\n";

    /// <summary>
    /// A payload whose residency is not its plan's, whose guard branches to another instruction's helper,
    /// whose debt test settles elsewhere, or whose inline template carries another word, is refused by the
    /// clause it breaks (JSD-0035 section 9).
    /// </summary>
    private static List<(string, bool, string)> TheScanNamesTheClauseAPayloadBreaks(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();
        var prefix = "value/scan/refuses/";
        var resident = NativeLifecycle.CompileWide(ResidentLoop, JsOutputForm.Value, abi.Name);
        var flat = NativeLifecycle.CompileWide(ResidentLoop, JsOutputForm.ValueFlat, abi.Name);

        if (resident.Artifact is null || flat.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(resident.Artifact, out var code, out var symbols, out _) ||
            !NativeLifecycle.TryReadEmitted(flat.Artifact, out var flatCode, out var flatSymbols, out _) ||
            !NativeLifecycle.TryReadImage(resident.Artifact, out var image, out _) ||
            !NativeLifecycle.TryReadImage(flat.Artifact, out var flatImage, out _))
        {
            rows.Add((prefix + abi.Name, false, "the resident loop did not emit in both value forms"));
            return rows;
        }

        var accepted = JsNativeScan.Scan(abi.Architecture, JsNativeTier.Value, code, symbols, 16, image);
        var flatAccepted = JsNativeScan.Scan(abi.Architecture, JsNativeTier.Value, flatCode, flatSymbols, 16, flatImage);

        rows.Add((
            "value/scan/the-resident-and-the-flat-payloads-are-admitted/" + abi.Name,
            accepted.Accepted && flatAccepted.Accepted && image.ResidentBindings && !flatImage.ResidentBindings &&
                !code.AsSpan().SequenceEqual(flatCode),
            accepted.Outcome + "; " + flatAccepted.Outcome));

        // THE RESIDENCY IS THE FORM BYTE'S, so a payload planned one way and scanned the other is a body that
        // is not its layout.
        foreach (var (name, scanned, scannedSymbols, handed) in new[]
                 {
                     ("a-resident-payload-under-a-flat-plan", code, symbols, image with { ResidentBindings = false }),
                     ("a-flat-payload-under-a-resident-plan", flatCode, flatSymbols, flatImage with { ResidentBindings = true }),
                 })
        {
            var result = JsNativeScan.Scan(abi.Architecture, JsNativeTier.Value, scanned, scannedSymbols, 16, handed);
            rows.Add((prefix + name + "/" + abi.Name, !result.Accepted, result.Outcome + " at " + result.Offset + ": " + result.Reason));
        }

        var table = JsNativeTemplates.For(abi.Architecture, JsNativeTier.Value);
        var grouped = JsBaselineBlocks.GroupHandlerOffsets(image);
        const int Unit = 1;

        if (symbols.Length <= Unit ||
            !JsValueLayout.TryPlan(image, Unit, grouped.Of(Unit), true, out var plan, out var refusal))
        {
            rows.Add((prefix + "mutations/" + abi.Name, false, "the loop's function has no value plan"));
            return rows;
        }

        var layout = JsValueLayout.Layout(plan);
        var offsets = EntryOffsets(table, symbols[Unit].Offset, layout);

        foreach (var (name, role, expected) in new[]
                 {
                     ("a-guard-that-branches-to-another-helper", JsValueRole.Guard, JsNativeScanOutcome.GuardNotItsHelper),
                     ("a-debt-test-that-settles-elsewhere", JsValueRole.Debt, JsNativeScanOutcome.DebtNotSettled),
                 })
        {
            var mutated = Retargeted(code, table, layout, offsets, role);
            var result = mutated is null
                ? default
                : JsNativeScan.Scan(abi.Architecture, JsNativeTier.Value, mutated, symbols, 16, image);

            rows.Add((
                prefix + name + "/" + abi.Name,
                mutated is not null && !result.Accepted && result.Outcome == expected,
                mutated is null
                    ? "the loop's layout has no " + role + " branch to move"
                    : result.Outcome + " at " + result.Offset + ": " + result.Reason));
        }

        // THE PROGRAM BODY CALLS f DIRECTLY, so its layout holds a direct call site (stage JSV-3): one whose
        // prepare call is the finish helper's, and one whose answer skips the finish, are each refused by the
        // call clause.
        if (!JsValueLayout.TryPlan(image, 0, grouped.Of(0), true, out var bodyPlan, out _))
        {
            rows.Add((prefix + "call-mutations/" + abi.Name, false, "the program body has no value plan"));
            return rows;
        }

        var bodyLayout = JsValueLayout.Layout(bodyPlan);
        var bodyOffsets = EntryOffsets(table, symbols[0].Offset, bodyLayout);
        var prepareAt = System.Array.FindIndex(bodyLayout, static entry => entry.Template == JsValueTemplate.CallPrepare);
        byte[]? finishFirst = null;

        if (prepareAt >= 0)
        {
            finishFirst = (byte[])code.Clone();
            TemplateOf(table, JsValueTemplate.CallFinish).Fixed.CopyTo(finishFirst, bodyOffsets[prepareAt]);
        }

        foreach (var (name, mutated) in new[]
                 {
                     ("a-direct-call-site-that-finishes-before-it-prepares", finishFirst),
                     ("a-direct-call-site-whose-answer-skips-the-finish", Retargeted(code, table, bodyLayout, bodyOffsets, JsValueRole.Call)),
                 })
        {
            var result = mutated is null
                ? default
                : JsNativeScan.Scan(abi.Architecture, JsNativeTier.Value, mutated, symbols, 16, image);

            rows.Add((
                prefix + name + "/" + abi.Name,
                mutated is not null && !result.Accepted && result.Outcome == JsNativeScanOutcome.CallNotDirect,
                mutated is null
                    ? "the program body's layout has no direct call site"
                    : result.Outcome + " at " + result.Offset + ": " + result.Reason));
        }

        var word = AnotherWord(code, table, layout, offsets);
        var wordResult = word is null
            ? default
            : JsNativeScan.Scan(abi.Architecture, JsNativeTier.Value, word, symbols, 16, image);

        rows.Add((
            prefix + "an-inline-template-that-materialises-another-word/" + abi.Name,
            word is not null && !wordResult.Accepted && wordResult.Outcome == JsNativeScanOutcome.InlineNotTheInstruction,
            word is null
                ? "the loop's layout has no inline word to change"
                : wordResult.Outcome + " at " + wordResult.Offset + ": " + wordResult.Reason));

        return rows;
    }

    /// <summary>Where each entry of a unit's layout begins, after the unit's prologue.</summary>
    private static int[] EntryOffsets(JsNativeTemplate[] table, uint unitStart, JsValueInstruction[] layout)
    {
        var at = (int)unitStart;

        // The table opens with the prologue's rows in the order they are written, ending at the one that
        // moves the program counter into place.
        var prologue = Array.FindIndex(table, static row => string.Equals(row.Text, "mov eax, arg1d", StringComparison.Ordinal)) + 1;

        for (var index = 0; index < prologue; index++)
        {
            at += table[index].Length;
        }

        var offsets = new int[layout.Length + 1];

        for (var index = 0; index < layout.Length; index++)
        {
            offsets[index] = at;
            at += TemplateOf(table, layout[index].Template).Length;
        }

        offsets[layout.Length] = at;
        return offsets;
    }

    /// <summary>The value table's row a layout entry names.</summary>
    private static JsNativeTemplate TemplateOf(JsNativeTemplate[] table, JsValueTemplate template) =>
        Array.Find(table, row => string.Equals(row.Text, JsValueLayout.TemplateName(template), StringComparison.Ordinal))
        ?? throw new InvalidOperationException("the value table has no row named " + JsValueLayout.TemplateName(template));

    /// <summary>
    /// A copy of a payload whose first branch of a role is moved to the target of another entry's branch
    /// of that role - another instruction's stub - or to the unit's first entry when there is no other.
    /// </summary>
    private static byte[]? Retargeted(
        byte[] code, JsNativeTemplate[] table, JsValueInstruction[] layout, int[] offsets, JsValueRole role)
    {
        var first = Array.FindIndex(layout, entry => entry.Role == role && entry.Target >= 0);

        if (first < 0)
        {
            return null;
        }

        var other = Array.FindIndex(
            layout, entry => entry.Role == role && entry.Target >= 0 && entry.Target != layout[first].Target);

        var destination = offsets[other >= 0 ? layout[other].Target : 0];
        var template = TemplateOf(table, layout[first].Template);
        var field = Array.Find(template.Fields, field => field.Kind == JsNativeFieldKind.UnitLocalBranch);
        var site = offsets[first] + (field.BitOffset / 8);
        var end = offsets[first] + template.Length;

        var mutated = (byte[])code.Clone();
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(mutated.AsSpan(site), destination - end);
        return mutated;
    }

    /// <summary>A copy of a payload whose first inline materialised Number is another Number.</summary>
    private static byte[]? AnotherWord(byte[] code, JsNativeTemplate[] table, JsValueInstruction[] layout, int[] offsets)
    {
        for (var index = 0; index < layout.Length; index++)
        {
            var template = TemplateOf(table, layout[index].Template);
            var at = Array.FindIndex(template.Fields, static field => field.Kind == JsNativeFieldKind.ValueWord);

            if (layout[index].Role != JsValueRole.Inline || at < 0)
            {
                continue;
            }

            var site = offsets[index] + (template.Fields[at].BitOffset / 8);
            var word = System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(code.AsSpan(site));

            // Only a Number's word, which another Number's is: 2.5 and 3.5 differ in their mantissa alone.
            if ((ulong)word >= JsWord.Undefined)
            {
                continue;
            }

            var mutated = (byte[])code.Clone();
            System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(
                mutated.AsSpan(site), BitConverter.DoubleToInt64Bits(BitConverter.Int64BitsToDouble(word) + 1.0));

            return mutated;
        }

        return null;
    }

    /// <summary>
    /// A resident payload whose form byte is flipped to the flat control is refused by the verifier: the
    /// byte says how the payload was planned, and a body planned the other way is not its layout.
    /// </summary>
    private static (string, bool, string) AFlippedResidencyBitIsRefused(JsX64Abi abi)
    {
        const string Name = "value/refuses/a-payload-whose-residency-bit-is-flipped";
        var compiled = NativeLifecycle.CompileWide(ResidentLoop, JsOutputForm.Value, abi.Name);
        var at = compiled.Artifact is null ? -1 : NativeLifecycle.NativeCodeBody(compiled.Artifact);

        if (at < 0 || compiled.Artifact![at + 1] != (byte)JsNativeTier.Value)
        {
            return (Name, false, "the resident loop did not emit a value header");
        }

        var patched = (byte[])compiled.Artifact.Clone();
        patched[at + 1] |= (byte)JsNativeCodeHeader.FlatBit;

        var answer = NativeLifecycle.RunWideArtifact(patched, JsOutputForm.Value, abi.Name, Fuel, reEmit: false, module: false);

        return (
            Name,
            answer.Outcome.StartsWith("the verifier refused", StringComparison.Ordinal),
            answer.Outcome);
    }

    /// <summary>Every wide program and probe answers in a value form what it answers in bytecode.</summary>
    private static List<(string, bool, string)> TheValueFormAnswersAsBytecode(JsX64Abi abi, JsOutputForm form, bool handleStress)
    {
        var rows = new List<(string, bool, string)>();
        var prefix = (form == JsOutputForm.ValueFlat ? "value-flat" : "value") +
            (handleStress ? "/under-handle-stress/answers-as-bytecode/" : "/answers-as-bytecode/");

        var programs = NativeAbiChecks.WidePrograms.Select(static program => (program.Name, program.Source, program.Library, GuestLoads: false))
            .Concat(NativeAbiChecks.Probes.Select(static probe => (probe.Name, probe.Source, Library: (string?)null, probe.GuestLoads)));

        foreach (var (name, source, library, guestLoads) in programs)
        {
            var interpreted = NativeLifecycle.RunWide(source, JsOutputForm.Bytecode, string.Empty, Fuel, library);

            var value = NativeLifecycle.RunWide(
                source, form, abi.Name, Fuel, library, handleStress: handleStress);

            rows.Add((
                prefix + name,
                SameAnswer(interpreted, value) && (guestLoads || interpreted.Fuel == value.Fuel) && interpreted.Completed,
                "bytecode " + interpreted.Outcome + " `" + interpreted.Completion + "` fuel " + interpreted.Fuel +
                "; value " + value.Outcome + " `" + value.Completion + "` fuel " + value.Fuel +
                (guestLoads ? " (fuel reported, not compared: the program loads code)" : string.Empty)));
        }

        return rows;
    }

    /// <summary>Whether two runs gave the same verdict: outcome, completion, error and printed lines.</summary>
    private static bool SameAnswer(NativeLifecycle.WideAnswer left, NativeLifecycle.WideAnswer right) =>
        string.Equals(left.Outcome, right.Outcome, StringComparison.Ordinal) &&
        string.Equals(left.Completion, right.Completion, StringComparison.Ordinal) &&
        string.Equals(left.TypeOf, right.TypeOf, StringComparison.Ordinal) &&
        string.Equals(left.ErrorName, right.ErrorName, StringComparison.Ordinal) &&
        string.Equals(left.ErrorMessage, right.ErrorMessage, StringComparison.Ordinal) &&
        string.Equals(left.Printed, right.Printed, StringComparison.Ordinal);

    // ---- direct calls (stage JSV-3) ----------------------------------------------------------------------

    /// <summary>
    /// Programs whose calls are direct in the value form: recursion, exceptions across calls, receivers,
    /// parameters, closures, the callees a direct call leaves to the helper, and a recursion to the counted
    /// bound.
    /// </summary>
    private static readonly (string Name, string Source)[] DirectCallPrograms =
    [
        ("recursion", """
            function fib(n) { return n < 2 ? n : fib(n - 1) + fib(n - 2); }
            function even(n) { return n === 0 ? true : odd(n - 1); }
            function odd(n) { return n === 0 ? false : even(n - 1); }
            function ack(m, n) { return m === 0 ? n + 1 : n === 0 ? ack(m - 1, 1) : ack(m - 1, ack(m, n - 1)); }
            function sum(a) { var s = 0; for (var i = 0; i < 3000; i++) { s = s + i * a; } return s; }
            [fib(16), even(501), odd(300), ack(2, 3), sum(2), sum(0.5)].join();
            """),
        ("exceptions", """
            var log = [];
            function thrower(x) { if (x % 3 === 2) throw new TypeError("t" + x); return x * 2; }
            function middle(x) { return thrower(x) + 1; }
            function outer(x) { try { return middle(x); } catch (e) { return e.name + e.message; } finally { log.push("f" + x); } }
            var r = [];
            for (var i = 0; i < 9; i++) r.push(outer(i));
            function throwsValue(v) { throw v; }
            function catchValue(v) { try { throwsValue(v); } catch (e) { return typeof e + ":" + String(e); } }
            r.push(catchValue(1), catchValue("s"), catchValue(null), catchValue({ toString: function () { return "o"; } }));
            function rethrow() { try { thrower(2); } catch (e) { throw new RangeError("re:" + e.message); } }
            try { rethrow(); } catch (e) { r.push(e.name + e.message); }
            function deep(n) { if (n === 0) throw new Error("bottom"); return deep(n - 1); }
            try { deep(200); } catch (e) { r.push(e.message); }
            function badRead(n) { return n > 0 ? badRead(n - 1) : null.x; }
            try { badRead(50); } catch (e) { r.push(e.constructor.name); }
            function returnsInFinally() { try { return thrower(1); } finally { log.push("rf"); } }
            r.push(returnsInFinally());
            r.join("|") + " " + log.join(",");
            """),
        ("receivers-and-parameters", """
            var obj = { v: 7, get: function () { return this.v; }, arrow: function () { var f = () => this.v * 2; return f(); } };
            function sloppyThis() { return this === globalThis; }
            function strictThis() { "use strict"; return this; }
            function primitiveThis() { return typeof this; }
            function missing(a, b, c) { return [a, b, c].map(String).join("/"); }
            function extra() { return arguments.length + ":" + arguments[2]; }
            function defaults(a, b = a + 1, c = b * 2) { return a + b + c; }
            function rest(a, ...more) { return a + more.length; }
            function destructure({ x, y = 5 }, [z]) { return x + y + z; }
            function mapped(a) { arguments[0] = 99; return a; }
            function strictUnmapped(a) { "use strict"; arguments[0] = 99; return a; }
            [obj.get(), obj.arrow(), sloppyThis(), String(strictThis()), primitiveThis.call(5),
             missing(1), extra(1, 2, 3, 4), defaults(1), rest(1, 2, 3), destructure({ x: 1 }, [2]),
             mapped(1), strictUnmapped(1)].join();
            """),
        ("closures-and-helper-callees", """
            function counter() { var c = 0; return function () { c += 1; return c; }; }
            var k = counter(); k(); k();
            function* gen() { yield 1; yield 2; }
            async function asy() { return 3; }
            class C { constructor(v) { this.v = v; } m() { return this.v; } static s(x) { return x + 1; } }
            var bound = function (a, b) { return this.q + a + b; }.bind({ q: 1 }, 2);
            var fromEval = eval("(function (x) { return x * 3; })");
            var results = [k(), [...gen()].join(), typeof asy(), new C(4).m(), C.s(1), bound(3), fromEval(5),
                Math.max(1, 9), [3, 1, 2].sort(function (a, b) { return a - b; }).join()];
            try { C(1); } catch (e) { results.push(e.name); }
            try { var notFn = 1; notFn(); } catch (e) { results.push(e.name); }
            function makeObject(i) { return { i: i, s: "x" + i }; }
            var made = [];
            for (var j = 0; j < 50; j++) made.push(makeObject(j).s);
            results.push(made.join(""));
            results.join();
            """),
        ("the-counted-bound", """
            var d = 0;
            function down() { d++; down(); }
            var name;
            try { down(); } catch (e) { name = e.name + ":" + e.message; }
            var d2 = 0, caughtAt = 0;
            function downCatching() { d2++; try { downCatching(); } catch (e) { caughtAt++; } }
            downCatching();
            var d3 = 0;
            function again() { d3++; again(); }
            try { again(); } catch (e) { name += "/" + e.name; }
            [name, d, d2, caughtAt, d3].join();
            """),
    ];

    /// <summary>
    /// Every direct-call program answers in both value forms, and under handle-stress, what it answers in
    /// bytecode, with the same fuel: the direct call charges, checks, binds and lands exactly as the
    /// interpreter's call does, up to the counted bound's <c>RangeError</c>.
    /// </summary>
    private static List<(string, bool, string)> TheDirectCallsAnswerAsBytecode(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();

        foreach (var (name, source) in DirectCallPrograms)
        {
            var interpreted = NativeLifecycle.RunWide(source, JsOutputForm.Bytecode, string.Empty, Fuel);

            foreach (var (label, form, stress) in new[]
                     {
                         ("value", JsOutputForm.Value, false),
                         ("value-flat", JsOutputForm.ValueFlat, false),
                         ("value/under-handle-stress", JsOutputForm.Value, true),
                     })
            {
                var value = NativeLifecycle.RunWide(source, form, abi.Name, Fuel, handleStress: stress);

                // A PROGRAM THAT LOADS CODE IS CHARGED FOR VERIFYING IT, and the value form's payload is larger,
                // so its fuel is reported and not compared, as the agreement rows do for their probes.
                var loads = source.Contains("eval(", StringComparison.Ordinal);

                rows.Add((
                    label + "/direct-calls/answer-as-bytecode/" + name,
                    SameAnswer(interpreted, value) && (loads || interpreted.Fuel == value.Fuel) && interpreted.Completed,
                    "bytecode " + interpreted.Render(withFuel: true) + "; " + label + " " + value.Render(withFuel: true) +
                    (loads ? " (fuel reported, not compared: the program loads code)" : string.Empty)));
            }
        }

        return rows;
    }

    /// <summary>
    /// Every <c>Call</c> the plan's walk reached is laid out as a direct call site, in every unit of every
    /// wide and direct-call program, and none is a plain helper call through the <c>Call</c> slot.
    /// </summary>
    private static (string, bool, string) EveryReachedCallIsADirectCallSite(JsX64Abi abi)
    {
        const string Name = "value/direct-calls/every-reached-call-is-a-direct-call-site";
        var sites = 0;

        var programs = NativeAbiChecks.WidePrograms.Select(static program => (program.Name, program.Source, program.Library))
            .Concat(DirectCallPrograms.Select(static program => (program.Name, program.Source, Library: (string?)null)));

        foreach (var (name, source, library) in programs)
        {
            var compiled = NativeLifecycle.CompileWide(source, JsOutputForm.Value, abi.Name, library);

            if (compiled.Artifact is null || !NativeLifecycle.TryReadImage(compiled.Artifact, out var image, out _))
            {
                return (Name, false, name + " did not compile in the value form");
            }

            var grouped = JsBaselineBlocks.GroupHandlerOffsets(image);

            for (var unit = 0; unit < image.Functions.Length; unit++)
            {
                if (!JsValueLayout.TryPlan(image, unit, grouped.Of(unit), image.ResidentBindings, out var plan, out _))
                {
                    continue;
                }

                var layout = JsValueLayout.Layout(plan);

                for (var pc = plan.First; pc < plan.End; pc++)
                {
                    if (plan.HeightAt(pc) < 0 || image.Code[pc] != (byte)JsOpcode.Call)
                    {
                        continue;
                    }

                    var at = pc;
                    var prepared = layout.Any(entry => entry.Pc == at && entry.Template == JsValueTemplate.CallPrepare);
                    var plain = layout.Any(entry =>
                        entry.Pc == at && entry.Template == JsValueTemplate.CallSlot && entry.Operand == (long)JsOpcode.Call * 8);

                    if (!prepared || plain)
                    {
                        return (Name, false, name + ": the Call at " + pc + " of unit " + unit + " is not a direct call site");
                    }

                    sites++;
                }
            }
        }

        return (Name, sites > 0, sites + " reached Call instructions, each a direct call site");
    }

    // ---- the differential ------------------------------------------------------------------------------

    /// <summary>
    /// What every differential program begins with: the edge values, and a rendering of a result that
    /// shows a Number's bits - the sign of a zero and a NaN's payload included - and any other kind's type
    /// and text.
    /// </summary>
    /// <remarks>
    /// <b>THE EDGE VALUES LEAVE OUT A NaN A WORD DOES NOT CARRY</b>: one setting a bit of
    /// <see cref="JsWord.NaNTagReach"/>, which the codec canonicalises (JSD-0035 section 2). That is the
    /// form's named divergence, which the language admits because it lets an implementation choose a NaN's
    /// bits, and it is not what a template could get wrong; the quiet and signalling NaNs of both signs
    /// with payloads are in, so every template runs over them and every result's bits are compared.
    /// </remarks>
    private const string Prelude = """
        var f64 = new Float64Array(1), u32 = new Uint32Array(f64.buffer);
        function nan(hi, lo) { u32[1] = hi; u32[0] = lo; return f64[0]; }
        function show(r) {
          if (typeof r !== "number") return typeof r + ":" + String(r);
          f64[0] = r;
          return u32[1].toString(16) + "." + u32[0].toString(16);
        }
        var V = [0, -0, 1, -1, 0.5, -0.5, 1.5, -2.5, 3, 31, 32, 33, -31, 2147483647, 2147483648, -2147483648,
          -2147483649, 4294967295, 4294967296, 4294967297, 1e21, -1e21, 1.7976931348623157e308, 5e-324, -5e-324,
          Infinity, -Infinity, NaN, nan(0x7ff80000, 1), nan(0xfff80000, 0), nan(0x7ff00000, 1), nan(0xfff00000, 5),
          9007199254740991, 9007199254740993, undefined, null, true, false, "", "3", "x", {}, [], [2], Symbol("s")];
        function one(f, a) { try { return show(f(a)); } catch (e) { return "throw " + e.name; } }
        function two(f, a, b) { try { return show(f(a, b)); } catch (e) { return "throw " + e.name; } }
        var lines = [];

        """;

    /// <summary>Every operator over every pair of edge values.</summary>
    private const string Binary = """
        var ops = [
          function (a, b) { return a + b; }, function (a, b) { return a - b; }, function (a, b) { return a * b; },
          function (a, b) { return a / b; }, function (a, b) { return a % b; },
          function (a, b) { return a < b; }, function (a, b) { return a <= b; }, function (a, b) { return a > b; },
          function (a, b) { return a >= b; }, function (a, b) { return a == b; }, function (a, b) { return a != b; },
          function (a, b) { return a === b; }, function (a, b) { return a !== b; },
          function (a, b) { return a & b; }, function (a, b) { return a | b; }, function (a, b) { return a ^ b; },
          function (a, b) { return a << b; }, function (a, b) { return a >> b; }, function (a, b) { return a >>> b; },
          function (a, b) { var x = a; x += b; return x; }, function (a, b) { var x = a; x -= b; x *= b; return x; },
          function (a, b) { var x = a; x /= b; x |= 0; return x; }, function (a, b) { var x = a; x <<= b; x >>>= 1; return x; },
          function (a, b) { return (a < b) === (b > a); }, function (a, b) { return a < b ? 1 : 2; },
          function (a, b) { if (a <= b) return 3; return 4; }, function (a, b) { return a - b - (a * b) / (b + a); },
          function (a, b) { return -a * -b; }, function (a, b) { return (a & 7) + (b ^ -1); },
        ];
        for (var k = 0; k < ops.length; k++)
          for (var i = 0; i < V.length; i++)
            for (var j = 0; j < V.length; j++)
              lines.push(k + " " + i + " " + j + " " + two(ops[k], V[i], V[j]));
        lines.join("\n");
        """;

    /// <summary>Every unary operator and every conditional jump over every edge value.</summary>
    private const string Unary = """
        var ops = [
          function (a) { return -a; }, function (a) { return +a; }, function (a) { return ~a; }, function (a) { return !a; },
          function (a) { return void a; }, function (a) { var x = a; x++; return x; }, function (a) { var x = a; x--; return x; },
          function (a) { var x = a; return ++x; }, function (a) { var x = a; return --x; }, function (a) { var x = a; return x++; },
          function (a) { if (a) return 1; return 2; }, function (a) { if (!a) return 1; return 2; }, function (a) { return a ? 1 : 2; },
          function (a) { return a || 7; }, function (a) { return a && 7; }, function (a) { return a ?? 7; },
          function (a) { var n = 0; while (a && n < 3) n++; return n; },
          function (a) { var n = 0; do { n++; } while (!a && n < 3); return n; },
          function (a) { return -(-a); }, function (a) { return ~~a; }, function (a) { return a | 0; }, function (a) { return a >>> 0; },
          function (a) { return typeof a; }, function (a) { return !!a; }, function (a) { return a === a; }, function (a) { return a != a; },
          function (a) { return !(a < 1); }, function (a) { return a == null; }, function (a) { return a === undefined ? 1 : 2; },
        ];
        for (var k = 0; k < ops.length; k++)
          for (var i = 0; i < V.length; i++)
            lines.push(k + " " + i + " " + one(ops[k], V[i]));
        lines.join("\n");
        """;

    /// <summary>
    /// Bindings - resident, captured, in their dead zone, per-iteration - and the stack operations the
    /// compound forms lower to, over every pair of edge values; and a straight line long enough that its
    /// debt is tested inside it.
    /// </summary>
    private static readonly string Bindings = """
        var ops = [
          function (a, b) { var x = a, y = b; x = x + y; y = x * y; return x - y; },
          function (a, b) { let x = a; { let y = b; x = y; } return x; },
          function (a, b) { const c = a; let d = c; d = d + b; return d; },
          function (a, b) { try { t = a; let t; return t; } catch (e) { return e.name; } },
          function (a, b) { try { var r = u; let u = a; return r; } catch (e) { return e.name; } },
          function (a, b) { var o = { v: a }; o.v += b; return o.v; },
          function (a, b) { var arr = [a]; arr[0] *= b; return arr[0]; },
          function (a, b) { var arr = [a, b]; arr[1]++; return arr[1]; },
          function (a, b) { var o = { v: a }; return o.v++; },
          function (a, b) { var o = { v: a }; var k = "v"; o[k] -= b; return o[k]--; },
          function (a, b) { var x, y; x = y = a; return x + y + b; },
          function (a, b) { var c = a; var g = function () { return c; }; return g() + b; },
          function (a, b) { var s = 0; for (var i = 0; i < 3; i++) { s = s + a; } return s * b; },
          function (a, b) { var s = 0; for (let i = 0; i < 3; i++) { s = s + a; } return s - b; },
          function (a, b) { var r = [a, b]; var t = r[0]; r[0] = r[1]; r[1] = t; return r[0] - r[1]; },
          function (a, b) { var x = a; switch (x) { case b: return 1; case 0: return 2; default: return 3; } },
          function (a, b) { for (var q = a; q < b; ) { return q; } return b; },
          function (a, b) { return arguments[0] + arguments.length; },
          function (a, b, c) { return c; },
          function (a, b) { var z = a; z = b; return [z, a, b].length; },
          function (a, b) { var [p, q] = [a, b]; return p - q; },
          function (a, b) { var x = a; x ??= b; return x; },
          function (a = 1, b = -0) { return a - b; },
          function ([a] = [], { b } = { b: 2 }) { return a + b; },
          function (a, b) { var r; ({ ...r } = { p: a, q: b }); return r.p - r.q; },
          function (a, b) { class C { static s() { return a; } m() { return b; } } return C.s() * new C().m(); },
          function (a, b) { var x = a;
        """ + string.Concat(Enumerable.Repeat(" x = x + b;", 300)) + """
         return x; },
        ];
        for (var k = 0; k < ops.length; k++)
          for (var i = 0; i < V.length; i++)
            for (var j = 0; j < V.length; j++)
              lines.push(k + " " + i + " " + j + " " + two(ops[k], V[i], V[j]));
        lines.join("\n");
        """;

    /// <summary>The differential programs, by name.</summary>
    private static readonly (string Name, string Source)[] Differentials =
    [
        ("binary", Prelude + Binary),
        ("unary", Prelude + Unary),
        ("bindings-and-the-stack", Prelude + Bindings),
    ];

    /// <summary>
    /// Every inline kind the layout can place is placed by some differential program, so no template
    /// passes the differential without having run.
    /// </summary>
    private static (string, bool, string) TheDifferentialProgramsPlaceEveryInlineKind(JsX64Abi abi)
    {
        const string Name = "value/differential/every-inline-kind-is-placed";
        var placed = new Dictionary<JsValueInline, SortedSet<string>>();

        foreach (var (name, source) in Differentials)
        {
            var compiled = NativeLifecycle.CompileWide(source, JsOutputForm.Value, abi.Name);

            if (compiled.Artifact is null || !NativeLifecycle.TryReadImage(compiled.Artifact, out var image, out var why))
            {
                return (Name, false, name + " did not compile in the value form");
            }

            var grouped = JsBaselineBlocks.GroupHandlerOffsets(image);

            for (var unit = 0; unit < image.Functions.Length; unit++)
            {
                if (!JsValueLayout.TryPlan(image, unit, grouped.Of(unit), image.ResidentBindings, out var plan, out _))
                {
                    continue;
                }

                for (var pc = plan.First; pc < plan.End; pc++)
                {
                    if (plan.HeightAt(pc) < 0 || plan.InlineAt(pc) == JsValueInline.None)
                    {
                        continue;
                    }

                    var kind = plan.InlineAt(pc);

                    if (!placed.TryGetValue(kind, out var opcodes))
                    {
                        placed[kind] = opcodes = new SortedSet<string>(StringComparer.Ordinal);
                    }

                    opcodes.Add(((JsOpcode)image.Code[pc]).ToString());
                }
            }
        }

        var missing = Enum.GetValues<JsValueInline>()
            .Where(kind => kind != JsValueInline.None && !placed.ContainsKey(kind))
            .ToArray();

        return (
            Name,
            missing.Length == 0,
            (missing.Length == 0 ? string.Empty : "never placed: " + string.Join(", ", missing) + "; ") +
            "placed: " + string.Join(", ", placed.OrderBy(static pair => pair.Key)
                .Select(static pair => pair.Key + " (" + string.Join(" ", pair.Value) + ")")));
    }

    /// <summary>Every differential program answers in the value form, bit for bit and fuel for fuel, what it answers in bytecode.</summary>
    private static List<(string, bool, string)> EveryInlineTemplateAnswersAsItsArm(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();

        foreach (var (name, source) in Differentials)
        {
            var interpreted = NativeLifecycle.RunWide(source, JsOutputForm.Bytecode, string.Empty, Fuel);

            foreach (var form in new[] { JsOutputForm.Value, JsOutputForm.ValueFlat })
            {
                var value = NativeLifecycle.RunWide(source, form, abi.Name, Fuel);
                var same = SameAnswer(interpreted, value) && interpreted.Fuel == value.Fuel;

                rows.Add((
                    (form == JsOutputForm.ValueFlat ? "value-flat" : "value") + "/differential/" + name,
                    same && interpreted.Completed,
                    same
                        ? interpreted.Completion.Split('\n').Length + " results agree bit for bit; fuel " + value.Fuel
                        : FirstDifference(interpreted, value)));
            }
        }

        return rows;
    }

    /// <summary>The first result two differential runs disagree on, or how their runs differ.</summary>
    private static string FirstDifference(NativeLifecycle.WideAnswer interpreted, NativeLifecycle.WideAnswer value)
    {
        if (!string.Equals(interpreted.Completion, value.Completion, StringComparison.Ordinal))
        {
            var left = interpreted.Completion.Split('\n');
            var right = value.Completion.Split('\n');

            for (var index = 0; index < Math.Min(left.Length, right.Length); index++)
            {
                if (!string.Equals(left[index], right[index], StringComparison.Ordinal))
                {
                    return "bytecode `" + left[index] + "`, value `" + right[index] + "`";
                }
            }

            return "bytecode gave " + left.Length + " results and the value form " + right.Length;
        }

        return "bytecode " + interpreted.Render(withFuel: true) + "; value " + value.Render(withFuel: true);
    }

    // ---- fuel parity -----------------------------------------------------------------------------------

    /// <summary>
    /// Each fuel-parity twin completes under the same total in all three forms, and gives the same verdict
    /// in all three at every ceiling within a settlement window of that total and at a sweep below it.
    /// </summary>
    private static List<(string, bool, string)> TheTwinsGiveOneVerdictAtEveryCeiling(JsX64Abi abi)
    {
        var rows = new List<(string, bool, string)>();
        var directory = TwinDirectory();

        if (directory is null)
        {
            rows.Add((
                "value/fuel-parity/the-twins-are-found",
                false,
                "no src/tests/forms/wide above this process's directory or the current one holds the twins"));

            return rows;
        }

        foreach (var path in Directory.GetFiles(directory, "*.small.js").Order(StringComparer.Ordinal))
        {
            var twin = Path.GetFileName(path);
            var source = File.ReadAllText(path);

            var forms = new[] { JsOutputForm.Bytecode, JsOutputForm.Value, JsOutputForm.ValueFlat };
            var artifacts = new byte[forms.Length][];

            for (var index = 0; index < forms.Length; index++)
            {
                var compiled = NativeLifecycle.CompileWide(source, forms[index], abi.Name);
                artifacts[index] = compiled.Artifact ?? [];
            }

            if (artifacts.Any(static artifact => artifact.Length == 0))
            {
                rows.Add(("value/fuel-parity/" + twin, false, "the twin did not compile in all three forms"));
                continue;
            }

            NativeLifecycle.WideAnswer At(int form, ulong ceiling) =>
                NativeLifecycle.RunWideArtifact(artifacts[form], forms[form], abi.Name, ceiling, reEmit: false, module: false);

            var totals = Enumerable.Range(0, forms.Length).Select(form => At(form, Fuel)).ToArray();
            var total = totals[0].Fuel;

            if (!totals.All(answer => answer.Completed && answer.Fuel == total && SameAnswer(totals[0], answer)))
            {
                rows.Add((
                    "value/fuel-parity/" + twin,
                    false,
                    "the totals differ: " + string.Join("; ", totals.Select(static answer => answer.Render(withFuel: true)))));

                continue;
            }

            var ceilings = new SortedSet<ulong>();

            for (var ceiling = total > Window ? total - Window : 1; ceiling <= total + 4; ceiling++)
            {
                ceilings.Add(ceiling);
            }

            for (var step = 1; step < Sweep; step++)
            {
                ceilings.Add(Math.Max(1, total * (ulong)step / Sweep));
            }

            string? disagreement = null;
            var completing = 0;

            foreach (var ceiling in ceilings)
            {
                var bytecode = At(0, ceiling);
                completing += bytecode.Completed ? 1 : 0;

                for (var form = 1; form < forms.Length && disagreement is null; form++)
                {
                    var other = At(form, ceiling);

                    if (!SameAnswer(bytecode, other))
                    {
                        disagreement =
                            "at " + ceiling + " bytecode " + bytecode.Render(withFuel: true) + " and " + forms[form] +
                            " " + other.Render(withFuel: true);
                    }
                }

                if (disagreement is not null)
                {
                    break;
                }
            }

            rows.Add((
                "value/fuel-parity/" + twin,
                disagreement is null && completing == 5,
                disagreement ??
                    "total " + total + " in all three forms; one verdict at each of " + ceilings.Count +
                    " ceilings, every one from " + (total > Window ? total - Window : 1) + " to " + (total + 4) +
                    " and " + (Sweep - 1) + " below"));
        }

        return rows;
    }

    /// <summary>The folder the fuel-parity twins are in, found above this process or the current directory.</summary>
    private static string? TwinDirectory()
    {
        foreach (var start in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
        {
            for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
            {
                var candidate = Path.Combine(directory.FullName, "src", "tests", "forms", "wide");

                if (File.Exists(Path.Combine(candidate, "numeric-loop.small.js")))
                {
                    return candidate;
                }
            }
        }

        return null;
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
