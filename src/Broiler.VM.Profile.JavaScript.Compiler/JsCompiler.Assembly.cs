// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          None
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- assembly ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=241F76
    // Broiler-Human:        PENDING
    private byte[] Assemble()
    {
        // THE MODULE ROWS ARE BUILT BEFORE THE POOL IS SNAPSHOTTED, and the order is not
        // cosmetic. A row names its module's key and every specifier it requests as CONSTANTS, so
        // building the rows interns names; doing it after the constant section had been encoded
        // left every one of those names past the end of the pool the artifact carries, and the
        // verifier refused the first module row of every module artifact this host produced.
        var moduleRows = built.Count == 0 ? [] : ModuleRows();

        // THE SOURCE TEXT IS INTERNED HERE TOO, before the pool is encoded, so its constants follow
        // every constant an instruction names and an artifact that drops it differs from one that
        // keeps it in the section and the constants appended after the code's own (JSD-0037).
        var sourceRows = new List<(uint FunctionIndex, uint TextConstant, uint Start, uint Length)>();

        foreach (var (unit, span) in Enumerable.OrderBy(sourceSpans, static pair => pair.Key))
        {
            sourceRows.Add(((uint)unit, StringConstant(sources[span.Source]), (uint)span.Start, (uint)(span.End - span.Start)));
        }

        // THE EVAL SCOPE MAP INTERNS NAMES TOO, for the same reason and with the same consequence:
        // it is built here, before the constant section is encoded, or the names it spells would
        // lie past the end of the pool the artifact carries.
        var evalShapes = new List<JsEvalScopeRow>();
        var evalShapeRows = new Dictionary<Scope, int>(ReferenceEqualityComparer.Instance);
        var evalParameterRows = new Dictionary<Scope, int>(ReferenceEqualityComparer.Instance);
        var evalSiteScopes = new List<(int Unit, int Offset, int Shape, int Depth, JsFormat.EvalRequestFlags Flags)>();

        for (var index = 0; index < units.Count; index++)
        {
            foreach (var site in units[index].EvalSites)
            {
                evalSiteScopes.Add((
                    index,
                    site.Offset,
                    EvalShapeRow(site.Scope, evalShapes, evalShapeRows, evalParameterRows, site.Parameters),
                    site.Depth,
                    site.Flags));
            }
        }

        var evalDeclarationRows = new JsEvalDeclarationRow[evalDeclarations.Count];

        for (var index = 0; index < evalDeclarations.Count; index++)
        {
            var (Unit, Flags, Refusal, VarNames, LexicalNames, FunctionNames, AnnexBNames, PrivateNames) = evalDeclarations[index];

            evalDeclarationRows[index] = new JsEvalDeclarationRow(
                (uint)Unit,
                Flags,
                Refusal,
                Array.ConvertAll(VarNames, name => (uint)InternedName(name)),
                Array.ConvertAll(LexicalNames, name => (uint)InternedName(name)),
                Array.ConvertAll(FunctionNames, name => (uint)InternedName(name)),
                Array.ConvertAll(AnnexBNames, name => (uint)InternedName(name)),
                Array.ConvertAll(PrivateNames, name => (uint)InternedName(name)));
        }

        // Each name is already in the pool - the prologue's own declaring instructions interned it -
        // and the rows are spelled here, before the pool is written, as the eval rows are.
        var scriptRows = new JsScriptDeclarationRow[scriptDeclarations.Count];

        for (var index = 0; index < scriptRows.Length; index++)
        {
            var (Unit, LexicalNames, VarNames, FunctionNames, AnnexBNames) = scriptDeclarations[index];

            scriptRows[index] = new JsScriptDeclarationRow(
                (uint)Unit,
                Array.ConvertAll(LexicalNames, name => (uint)InternedName(name)),
                Array.ConvertAll(VarNames, name => (uint)InternedName(name)),
                Array.ConvertAll(FunctionNames, name => (uint)InternedName(name)),
                Array.ConvertAll(AnnexBNames, name => (uint)InternedName(name)));
        }

        // Spelled before the pool is written, as the declaration rows are.
        var referrerRows = scriptReferrers.ConvertAll(placed => ((uint)placed.Unit, (uint)InternedName(placed.Referrer))).ToArray();

        var code = new List<byte>();
        var rows = new List<JsFunctionRow>();
        var regions = new List<JsExceptionRegionRow>();
        var positions = new List<(uint Offset, uint Line, uint Column)>();
        var bases = new uint[units.Count];
        var maximumStack = 1u;
        var maximumSlots = 1u;

        for (var index = 0; index < units.Count; index++)
        {
            bases[index] = (uint)code.Count;
            var unit = units[index];
            unit.FinishScopes();

            foreach (var site in unit.BranchSites)
            {
                var target = (uint)(
                    unit.Code[site] |
                    (unit.Code[site + 1] << 8) |
                    (unit.Code[site + 2] << 16) |
                    (unit.Code[site + 3] << 24)) + bases[index];

                for (var shift = 0; shift < 32; shift += 8)
                {
                    unit.Code[site + (shift / 8)] = (byte)((target >> shift) & 0xFF);
                }
            }

            code.AddRange(unit.Code);
            maximumStack = Math.Max(maximumStack, (uint)unit.MaximumStack);
            maximumSlots = Math.Max(maximumSlots, (uint)unit.SlotCount);

            rows.Add(new JsFunctionRow(
                unit.NameConstant,
                (uint)unit.ParameterCount,
                (uint)unit.SlotCount,
                (uint)unit.MaximumStack,
                bases[index],
                (uint)unit.Code.Count,
                (uint)unit.Flags));
        }

        for (var index = 0; index < units.Count; index++)
        {
            var unit = units[index];

            foreach (var region in unit.Regions)
            {
                regions.Add(new JsExceptionRegionRow(
                    (uint)index,
                    region.TryStart + bases[index],
                    region.TryEnd + bases[index],
                    region.Handler + bases[index],
                    region.ScopeDepth,
                    region.StackHeight,
                    region.Kind));
            }

            foreach (var (offset, line, column) in unit.Positions)
            {
                positions.Add((offset + bases[index], line, column));
            }
        }

        positions.Sort(static (left, right) => left.Offset.CompareTo(right.Offset));

        var evalSites = new JsEvalSiteRow[evalSiteScopes.Count];

        for (var index = 0; index < evalSites.Length; index++)
        {
            var (unit, offset, shape, depth, flags) = evalSiteScopes[index];

            evalSites[index] = new JsEvalSiteRow(
                (uint)unit, (uint)offset + bases[unit], (uint)shape, (uint)depth, flags);
        }

        var assembled = new JsAssembledProgram(ManifestId(), [.. code], rows, regions, constants, maximumStack, maximumSlots);

        if (request.Manifest == JsFeatureManifest.Numeric)
        {
            SweepAgainstTheNumericManifest(assembled);
        }

        JsNativeEmission? emission = null;

        if (request.Form is JsOutputForm.Native or JsOutputForm.Value or JsOutputForm.ValueFlat && diagnostics.Count == 0)
        {
            emission = Emit(assembled);
        }

        if (diagnostics.Count != 0)
        {
            return [];
        }

        var sections = new List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(
                    maximumStack, maximumSlots, (uint)units.Count, (uint)constants.Count)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants([.. constants])),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, [.. code]),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([.. entries])),
        };

        if (regions.Count != 0)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ExceptionRegions,
                JsArtifactWriter.ExceptionRegions([.. regions])));
        }

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
            JsArtifactWriter.Positions([.. positions])));

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
            JsArtifactWriter.Functions([.. rows])));

        // THE SURFACES SECTION IS WRITTEN ONLY WHEN THERE IS ONE, and its absence is what every
        // artifact written before the kind existed says: this program reaches no optional surface.
        // An empty section would say the same thing in more bytes, and would make the difference
        // between "declares none" and "declares nothing" a difference a reader has to look for.
        // THE MODULE SURFACE IS DECLARED WHERE THE RECORDS ARE WRITTEN, and not by a global read.
        // It is the one optional surface no name puts a program inside: what makes a program a
        // module is that it carries module records, so this is where it says so.
        if (built.Count != 0)
        {
            surfaces.Add(JsSurfaces.Modules);
        }

        // THE NATIVE SURFACE IS DECLARED WHERE THE EMITTED BYTES ARE WRITTEN, and for the same
        // reason the module surface is declared where the records are: no name a program can write
        // puts it inside this surface. What puts an artifact inside it is that it carries machine
        // code, and this is the one place that is known.
        if (emission is not null)
        {
            surfaces.Add(JsSurfaces.Native);
        }

        // THE EVAL SCOPE MAP IS DECLARED WITH THE DYNAMIC SURFACE, which is the only surface that
        // ever reads it: a site's map is read by a direct `eval` and an eval-code unit's row by the
        // evaluation that runs it. An artifact with neither writes no section, which is what every
        // artifact written before the kind existed says.
        var carriesEvalScopes = evalSites.Length != 0 || evalDeclarationRows.Length != 0;

        if (carriesEvalScopes)
        {
            surfaces.Add(JsSurfaces.Dynamic);
        }

        if (surfaces.Count != 0)
        {
            var declared = new string[surfaces.Count];
            surfaces.CopyTo(declared);

            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces,
                JsArtifactWriter.Surfaces(declared)));
        }

        if (built.Count != 0)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Modules,
                JsArtifactWriter.Modules(moduleRows)));
        }

        if (emission is not null)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeCode,
                JsArtifactWriter.NativeCode(
                    JsNativeCodeHeader.Pack(emission.Architecture, emission.ValueForm, emission.ResidentBindings),
                    emission.BackendSemanticVersion,
                    emission.CodeAlignment,
                    emission.Code)));

            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeSymbols,
                JsArtifactWriter.NativeSymbols(emission.Symbols)));
        }

        if (carriesEvalScopes)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.EvalScopes,
                JsArtifactWriter.EvalScopes([.. evalShapes], evalSites, evalDeclarationRows)));
        }

        // THE SCRIPT DECLARATIONS ARE WRITTEN BESIDE EVERY MANIFEST, because every manifest has
        // scripts and every script's global instantiation checks (JSeal V15-host).
        if (scriptRows.Length != 0)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ScriptDeclarations,
                JsArtifactWriter.ScriptDeclarations(scriptRows)));
        }

        // THE SCRIPT REFERRERS ARE WRITTEN BESIDE EVERY MANIFEST too, and only for a script a host
        // placed: an artifact whose scripts were compiled with no referrer says nothing new.
        if (referrerRows.Length != 0)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ScriptReferrers,
                JsArtifactWriter.ScriptReferrers(referrerRows)));
        }

        // THE SOURCE TEXT IS WRITTEN BESIDE EVERY MANIFEST, for every function the compilation
        // kept a span for (JSD-0037).
        if (sourceRows.Count != 0)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.SourceText,
                JsArtifactWriter.SourceText([.. sourceRows])));
        }

        return JsArtifactWriter.Write(ManifestId(), [.. sections]);
    }

    /// <summary>The feature-manifest identity this compilation's artifact names.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7493AC
    // Broiler-Human:        PENDING
    private string ManifestId() =>
        request.Manifest == JsFeatureManifest.Numeric
            ? JsNumericManifest.ManifestId
            : JsFormat.ManifestId;

    /// <summary>
    /// Checks what was actually emitted against the numeric manifest's closed instruction set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>IT IS A CHECK OF THE FRONT END AGAINST ITSELF AND NOT A CHECK OF THE SOURCE.</b> The
    /// admission pass has already refused every construct the manifest excludes, so a program that
    /// reaches here is a program whose source the manifest admits. This sweep answers the other
    /// question - whether the LOWERING wrote only what the manifest says a lowering under it may
    /// write - and the two can part company without either being obviously wrong: a construct the
    /// admission pass never thought to name, or a lowering that grew an instruction for something
    /// it used to spell another way, would each produce an artifact naming a manifest whose closed
    /// instruction set it does not respect.
    /// </para>
    /// <para>
    /// <b>The consequence of leaving it out would be an untrue artifact rather than a crash</b>,
    /// which is the reason it is here and not in a test. An artifact naming this manifest is a
    /// promise to anything that reads it - a backend most of all - that its instructions come from
    /// one list. A promise nothing checks is a promise that is true until it is not.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=375AB9
    // Broiler-Falsified-If: an artifact naming the numeric manifest is produced carrying an instruction that manifest does not admit
    // Broiler-Human:        PENDING
    private void SweepAgainstTheNumericManifest(JsAssembledProgram assembled)
    {
        for (var index = 0; index < assembled.Functions.Count; index++)
        {
            var row = assembled.Functions[index];

            if (!JsNumericManifest.AdmitsFlags((JsFormat.FunctionFlags)row.Flags))
            {
                Refuse(
                    default,
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    "the lowering wrote a code unit whose flags the declared feature manifest " +
                    "does not admit");

                return;
            }

            var at = (int)row.CodeOffset;
            var end = at + (int)row.CodeLength;

            while (at < end)
            {
                var opcode = (JsOpcode)assembled.Code[at];

                if (!JsOpcodes.IsDefined(assembled.Code[at]))
                {
                    Refuse(
                        default,
                        SliceSourceDiagnosticCode.ConstructOutsideManifest,
                        "the lowering wrote an instruction this build does not define");

                    return;
                }

                if (!JsNumericManifest.Admits(opcode))
                {
                    Refuse(
                        default,
                        SliceSourceDiagnosticCode.ConstructOutsideManifest,
                        "the lowering wrote the instruction `" + opcode +
                        "`, which the declared feature manifest does not admit");

                    return;
                }

                at += JsOpcodes.InstructionWidth(opcode);
            }
        }

        if (assembled.ExceptionRegions.Count != 0)
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "the lowering wrote an exception region, which the declared feature manifest " +
                "does not admit");
        }
    }

    /// <summary>Asks the named backend for machine code, or records why there is none.</summary>
    /// <remarks>
    /// <para>
    /// <b>BOTH MANIFESTS HAVE A NATIVE FORM, AND EVERY UNIT OF AN ARTIFACT HAS THE ONE FORM.</b>
    /// Under the numeric manifest a backend computes: values are doubles in slabs and the emitted
    /// code does the arithmetic. Under the wide manifest a backend emits the baseline form: each block
    /// of instructions is one call into the interpreter's own dispatch and the control flow between
    /// blocks is emitted, so no construct the wide lowering writes is
    /// outside it. Either way the backend answers for the whole artifact or refuses it, which is
    /// why there is still no mixed form for this method to be asked for.
    /// </para>
    /// <para>
    /// <b>THREE REFUSALS AND THEY NAME THREE DIFFERENT MISTAKES.</b> Asking for machine code under
    /// a manifest with no native form is asking for something this profile does not define;
    /// naming a backend this build does not carry is naming something that does not exist; and a
    /// backend that will not emit has its own sentence to say - an arm64 backend asked for the
    /// wide manifest, or a baseline emission past the format's native-code ceiling. An author told
    /// the wrong one of the three looks in the wrong place, which is the whole reason a refusal
    /// names a construct rather than a stage.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6F05A5
    // Broiler-Human:        PENDING
    private JsNativeEmission? Emit(JsAssembledProgram assembled)
    {
        if (request.Manifest != JsFeatureManifest.Numeric && request.Manifest != JsFeatureManifest.Wide)
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "the native output form is admitted by the `" + JsNumericManifest.ManifestId +
                "` and `" + JsFormat.ManifestId + "` feature manifests, and every unit of an " +
                "artifact has the one form");

            return null;
        }

        // THE VALUE FORM IS THE WIDE MANIFEST'S ALONE (JSD-0035 section 1), and asking for it under the
        // numeric manifest is asking for something this profile does not define there.
        if (request.Form is JsOutputForm.Value or JsOutputForm.ValueFlat && request.Manifest != JsFeatureManifest.Wide)
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "the value output form is admitted by the `" + JsFormat.ManifestId + "` feature " +
                "manifest alone");

            return null;
        }

        if (!JsNativeBackends.TryFind(request.Backend, out var backend))
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "this build carries no backend named `" + request.Backend + "`; it names " +
                string.Join(", ", JsNativeBackends.Names));

            return null;
        }

        if (!backend.TryEmit(
                assembled with
                {
                    ValueForm = request.Form is JsOutputForm.Value or JsOutputForm.ValueFlat,
                    ResidentBindings = request.Form != JsOutputForm.ValueFlat,
                },
                out var emission,
                out var refusal))
        {
            Refuse(default, SliceSourceDiagnosticCode.ConstructOutsideManifest, refusal);
            return null;
        }

        return emission;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=79FFCF
    // Broiler-Human:        PENDING
    private JsModuleRow[] ModuleRows()
    {
        var rows = new JsModuleRow[built.Count];

        for (var index = 0; index < built.Count; index++)
        {
            var build = built[index];
            var specifiers = new uint[build.Requests.Count];
            var requests = new uint[build.Requests.Count];

            for (var request = 0; request < requests.Length; request++)
            {
                specifiers[request] = InternedName(build.Requests[request]);
                requests[request] = InternedName(build.RequestKeys[request]);
            }

            rows[index] = new JsModuleRow(
                InternedName(build.Key),
                (uint)build.BodyUnit,
                (uint)build.InitialiserUnit,
                specifiers,
                requests,
                [.. build.ImportRows],
                [.. build.LocalExports],
                [.. build.IndirectExports],
                [.. build.StarExports]);
        }

        return rows;
    }
}
