// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The retained malformed entries of format version 2, one per structural refusal that version
/// adds.
/// </summary>
/// <remarks>
/// <para>
/// <b>One entry per code, and the count is the point.</b> Version 2 adds a function table, an
/// environment model, exception regions, the optional surfaces and the one unit kind that may
/// suspend; each of those is a place where an artifact can be structurally wrong in a way version 1
/// has no vocabulary for, and the published registry binds each new code to a named entry here. A
/// verifier that refused every version-2 artifact would satisfy none of them, because they are
/// distinguished by the code and not by the refusal.
/// </para>
/// <para>
/// <b>Every entry is bytes, not a host.</b> They carry the replay mode <c>wide</c>, and the only
/// thing that mode changes is the descriptor the caller presents - the format version and the
/// manifest - because a version-2 payload announced as version 1 is refused for the mismatch
/// before the version-2 pass reads a section, which would test the wrong thing.
/// </para>
/// </remarks>
internal static class WideCorpus
{
    /// <summary>The replay mode a version-2 entry is presented under.</summary>
    internal const string Mode = "wide";

    /// <summary>
    /// The replay mode for a composition that admits no optional surface.
    /// </summary>
    /// <remarks>
    /// <b>It is a different HOST, not different bytes.</b> The entry that carries it is a
    /// well-formed artifact declaring the binary surface, and the only reason it is refused is that
    /// the composition replaying it registered a descriptor admitting none. That is the property
    /// roadmap section 6 describes and which no entry recorded until now: a composition declining a
    /// manifest refuses the artifact at verification, with an invalid-artifact reason, rather than
    /// letting it run and answering a run-time error the guest could catch.
    /// </remarks>
    internal const string DecliningMode = "wide-declining";

    /// <summary>Every version-2 entry, in the order the registry publishes their codes.</summary>
    internal static CorpusEntry[] Build() =>
    [
        Entry(
            "wide-a-function-row-the-format-cannot-represent",
            Artifact(parameterCount: 2, scopeSlots: 1),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedFunctionRow),
        Entry(
            "wide-an-entry-point-naming-no-code-unit",
            Artifact(entryUnit: 3),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.FunctionIndexOutOfRange),
        Entry(
            "wide-code-units-that-do-not-tile-the-code-section",
            Artifact(codeOffset: 1),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.CodeUnitRangeInvalid),
        Entry(
            "wide-a-scope-popped-past-the-frame",
            Artifact(code: [(byte)JsOpcode.PopScope, (byte)JsOpcode.ReturnUndefined]),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ScopeDepthOutOfRange),
        Entry(
            "wide-an-exception-handler-outside-its-code-unit",
            Artifact(regionHandlerOutsideUnit: true),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedExceptionRegion),

        // ---- three rows about the optional surfaces an artifact may declare --------------------
        //
        // A surface made of globals cannot be refused by refusing a construct, because reading
        // `Uint8Array` is byte for byte reading a name. The artifact declares what it reaches, and
        // these are the three ways that declaration can be wrong: said twice, naming something
        // nobody wrote, and naming something this composition declined. The last is the only one
        // whose answer depends on the host, and it is the one the manifest boundary is FOR.
        Entry(
            "wide-a-surface-declared-twice",
            Artifact(surfaces: [JsSurfaces.Binary, JsSurfaces.Binary]),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.DuplicateSurface),
        Entry(
            "wide-a-surface-this-build-does-not-implement",
            Artifact(surfaces: ["broiler.javascript.telepathy"]),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.UnknownSurface),
        new CorpusEntry(
            "wide-a-surface-the-composition-declined",
            DecliningMode,
            "InvalidArtifact",
            "UnsupportedFeatureManifest",
            JavaScriptDiagnosticCodes.SurfaceOutsideComposition,
            "-",
            "-",
            "-",
            "-",
            Artifact(surfaces: [JsSurfaces.Binary])),

        // ---- four rows about the two unit kinds that may suspend --------------------------------
        //
        // A suspendable invocation's frame is put on the heap by the EXECUTOR, from the unit's own
        // flag, before any of its code runs. Each of these is a way an artifact can ask for a
        // suspension the executor has not allocated a frame for, and each is refused by the
        // verifier rather than met by a null frame in the middle of the dispatch loop: two put a
        // suspension in a unit that is not the kind that can hold one, and two declare a unit that
        // is that kind AND one of the things it cannot also be.
        //
        // TWO OPCODES AND TWO FLAGS RATHER THAN ONE OF EACH, because the two drivers are different:
        // a `Yield` is resumed by the guest calling `next` and an `Await` by the job queue, so a
        // unit carrying the wrong bit would be handed to a driver with no way to reach it again.
        // The codes are separate for the same reason - an author told the wrong bit is missing
        // looks in the wrong place.
        Entry(
            "wide-a-suspension-outside-a-generator",
            Artifact(code: [
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.Yield,
                (byte)JsOpcode.Return,
            ]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.YieldOutsideGenerator),
        Entry(
            "wide-a-generator-that-is-also-the-program-body",
            Artifact(
                flags: JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Generator),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.GeneratorFlagsInconsistent),
        Entry(
            "wide-an-await-outside-an-async-function",
            Artifact(code: [
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.Await,
                (byte)JsOpcode.Return,
            ]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.AwaitOutsideAsync),
        // THIS ROW USED TO PAIR `Async` WITH `Generator`, AND THAT PAIRING IS NOW THE ASYNC
        // GENERATOR. The combination names a third driver rather than a contradiction, so the row
        // pairs `Async` with the flag that still contradicts it: a unit cannot be both an async
        // function and the program body, because the program body is entered by the host and there
        // is no promise for it to answer with.
        Entry(
            "wide-an-async-function-that-is-also-the-program-body",
            Artifact(
                flags: JsFormat.FunctionFlags.Async | JsFormat.FunctionFlags.ProgramBody),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.AsyncFlagsInconsistent),

        // ---- one row about the `for await` head, which is five instructions and one flag --------
        //
        // FOUR OF THE FIVE WOULD RUN PERFECTLY WELL IN AN ORDINARY FUNCTION - each is a call on an
        // iterator - and the answer would be a promise nobody ever resolved rather than an error
        // anybody could diagnose. Only the `Await` between them carries the async flag's own check,
        // so the sequence is refused against the same flag: a `for await` head belongs to a body
        // that may await, and this is where the FORMAT says so rather than the lowering.
        Entry(
            "wide-an-async-iteration-step-outside-an-async-function",
            Artifact(code: [
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.IterateStartAsync,
                (byte)JsOpcode.Return,
            ]),
            "SemanticValidationFailed",
            JavaScriptDiagnosticCodes.AsyncIterationOutsideAsync),

        // ---- one row about the class body's own operand bits ------------------------------------
        //
        // EVERY BIT IN THIS OPERAND IS DEFINED AND THE COMBINATION IS NOT, which is why it is not
        // the unknown-opcode answer the other operand checks give. A static block that is not
        // static has no `this` to run against, and the executor would have had to pick an arm for
        // an encoding nothing agreed on. The stack is left valid on purpose: the entry has to reach
        // the operand check rather than being refused for a height the lowering would never write.
        Entry(
            "wide-a-class-element-whose-flags-contradict",
            Artifact(code: [
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.DefineClassElement, JsOpcodes.ElementIsBlock,
                (byte)JsOpcode.Pop,
                (byte)JsOpcode.Return,
            ]),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.ClassElementFlagsInconsistent),

        // ---- one row about the instruction that names a surface without being a name ------------
        //
        // A DYNAMIC IMPORT IS THE ONLY OPTIONAL SURFACE AN ARTIFACT CAN REACH WITHOUT READING A
        // NAME OR CARRYING A SECTION. `eval` is a global, so a program inside the dynamic surface
        // is one that reads a name and the declaration follows from the read; a module graph is a
        // section, so an artifact inside the module surface is one that carries records. An
        // `ImportCall` is neither - it is an instruction - and an artifact could therefore have
        // written one while declaring nothing at all, and bought a host round trip with an opcode.
        // This is that artifact, and the code says which of the two declarations it is missing.
        Entry(
            "wide-an-import-call-with-no-surface-declared",
            Artifact(code: [
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.ImportCall, 0x00, 0x00,
                (byte)JsOpcode.Return,
            ]),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.ImportCallOutsideManifest),

        // ---- four rows about the sections that carry emitted machine code ----------------------
        //
        // A NATIVE PAYLOAD IS THE ONE PAYLOAD WHOSE PRESENCE IS A REQUEST TO MAKE MEMORY
        // EXECUTABLE, so the first of the three is the artifact that carries the bytes and never
        // asks: the surfaces section is where a composition's answer about executable memory is
        // read, and an artifact declaring nothing has skipped the question rather than been
        // refused it. The second is the framing refusal, and one entry stands for the whole family
        // because one code does: the verifier decodes no instruction of any architecture, so every
        // question it can answer about these sections is a question about framing. This one
        // declares an emitted length that disagrees with the bytes the section actually carries.
        //
        // THE THIRD VERIFIES, AND IT IS THE ONE THAT MAKES THE OTHER TWO WORTH HAVING. Two
        // malformed entries are satisfied by a verifier that refuses every artifact carrying these
        // sections at all, which would be the easiest wrong implementation to write. This one is a
        // well-formed program declaring the native surface and carrying a symbol for its one code
        // unit, and its completion value is what such a verifier could never produce. THE BYTES IT
        // CARRIES ARE NOT MACHINE CODE AND NOTHING RUNS THEM: this build maps no page and arms
        // none, the ordinary bytecode is what the executor runs, and the entry is a claim about
        // framing rather than about any instruction set.
        //
        // THE FOURTH IS THE SAME BYTES UNDER A DIFFERENT HOST, and it is the one the surface exists
        // for: a composition that admits no optional surface refuses the artifact where the
        // surfaces are read, before the emitted sections are reached at all. That is what declining
        // executable memory looks like from the outside, and it is the same answer the declined
        // binary surface gets - which is the point, because a refusal that needed a mechanism of
        // its own would be a refusal a new surface could forget to have.
        Entry(
            "wide-a-native-payload-no-surface-declares",
            Artifact(nativeCode: [0x00, 0x00, 0x00, 0x00], nativeSymbolOffset: 0),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.NativeSectionOutsideManifest),
        Entry(
            "wide-a-native-code-section-that-disagrees-with-itself",
            Artifact(
                surfaces: [JsSurfaces.Native],
                nativeCode: [0x00, 0x00, 0x00, 0x00],
                nativeSymbolOffset: 0,
                nativeDeclaredLength: 3),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedNativeSection),
        new CorpusEntry(
            "wide-the-native-surface-a-composition-declined",
            DecliningMode,
            "InvalidArtifact",
            "UnsupportedFeatureManifest",
            JavaScriptDiagnosticCodes.SurfaceOutsideComposition,
            "-",
            "-",
            "-",
            "-",
            Artifact(
                surfaces: [JsSurfaces.Native],
                nativeCode: [0x00, 0x00, 0x00, 0x00],
                nativeSymbolOffset: 0)),
        // THIS ROW PINS THE REFUSAL AND NOT A RUN, AND THE REASON IS THE WHOLE OF WHAT A RETAINED
        // CORPUS IS FOR. It read `Ok(..., "1")` until 2026-09-08 - a native payload that verified,
        // instantiated, ran and completed with `1` - and the payload it carried was four zero bytes
        // declared as `X64Windows`. Two things were wrong with that and each is worse than the
        // other on a different machine.
        //
        // ON A HOST WHOSE CONVENTION MATCHED, THE FOUR ZERO BYTES WERE ARMED AND JUMPED INTO, and
        // the process died of an access violation inside the emitted frame. A corpus is the one
        // place in this component whose entire subject is input nobody should trust, so a corpus
        // entry that hands arbitrary bytes to an armed page is the shape of defect this file exists
        // to catch rather than to contain.
        //
        // ON EVERY OTHER HOST IT WAS REFUSED, so the row's expected answer was a property of the
        // machine that generated the corpus rather than of the artifact. A retained row pins ONE
        // answer; a row that answers differently on `win-x64` and on `linux-x64` pins nothing, and
        // the continuous-integration lane found it on the first Linux run.
        //
        // WHAT REPLACES IT IS A PAYLOAD NO HOST WILL EVER ARM. `JsNativeExecution.HostArchitecture`
        // answers `X64Windows`, `X64SystemV` or `None` and never `Arm64`, so an `arm64` payload is
        // refused at instantiation on every machine this component runs on - deterministically, and
        // before a page is mapped. That makes the row host-independent AND makes the four bytes
        // unreachable, and it pins the more valuable of the two properties: that an artifact
        // emitted for another architecture is REFUSED rather than quietly run through the bytecode
        // sitting beside it in the same artifact, which is the guard on this profile's no-fallback
        // rule and the falsification criterion `Instantiate` carries.
        //
        // The bytes are a real A64 instruction - `ret`, 0xD65F03C0 little-endian - rather than
        // zeros, because a corpus entry declaring an architecture should carry something that
        // architecture could execute even when nothing here will.
        //
        // WHAT THIS ROW NO LONGER COVERS is a native payload that verifies, arms and RUNS. That
        // belongs to `NativeAbiChecks`, which selects `JsX64Abi.Host` and compiles through the
        // backend, so it exercises the run path with real emitted code on whichever convention the
        // machine actually has - which is where a host-dependent property should have been all
        // along.
        new CorpusEntry(
            "wide-a-native-payload-for-an-architecture-no-host-arms",
            Mode,
            "ProfileFault",
            "UnsatisfiedHostAssumption",
            0,
            "-",
            "-",
            "-",
            "-",
            Artifact(
                surfaces: [JsSurfaces.Native],
                nativeCode: [0xC0, 0x03, 0x5F, 0xD6],
                nativeSymbolOffset: 0,
                nativeArchitecture: JsNativeArchitecture.Arm64)),

        // ---- six rows about the template-closure scan, which is the layer this root HAS --------
        //
        // THIS ROOT CARRIES NO BACKEND, SO RE-EMISSION EQUALITY IS NOT AVAILABLE TO IT AND THE
        // TEMPLATE-CLOSURE SCAN IS THE WHOLE OF WHAT IT CAN CHECK ABOUT AN EMITTED PAYLOAD. That
        // is the execution-only position exactly, it is the position JSB-5 describes, and until
        // 2026-09-08 nothing whatever stood in it: the architecture value was the only thing
        // between a malformed payload and an armed page, and it asks which machine the bytes are
        // for rather than whether they are code. These rows are what that layer answers, pinned by
        // the BYTE the scan named as well as by the code, because every one of them refuses with
        // 1625 and the offset is the only column that tells them apart.
        //
        // FIVE OF THE SIX DECLARE arm64 AND THAT IS A RULE OF THIS FILE RATHER THAN A PREFERENCE.
        // A corpus entry is replayed by a root that ARMS what it verifies and that also draws
        // mutants from these same bytes, so a retained x86-64 payload is a payload some mutation of
        // which this machine will jump into. `JsNativeExecution.HostArchitecture` answers
        // `X64Windows`, `X64SystemV` or `None` and never `Arm64`, so an arm64 payload is refused at
        // instantiation on every machine this component runs on - which makes these rows
        // host-independent AND makes their bytes unreachable, both of which the 2026-09-08
        // correction above already had to learn once.
        //
        // THE ONE EXCEPTION IS THE INCIDENT ITSELF. `JSC-208` records four zero bytes carried as an
        // x86-64 payload, verified, armed, jumped into, and an access violation in
        // `JsNativeExecution.Invoke`; the architecture is half of what that row records and an
        // arm64 twin would be a different row. It is safe to retain under the architecture it names
        // because four zero bytes match no template of either table - they are the byte sequence
        // the scan was written for - and because no near neighbour of them is closed either.
        //
        // WHAT THESE ROWS THEREFORE DO NOT COVER, stated as a rule and not as a plan: no clause
        // that needs an x86-64 payload to reach it is pinned here. A branch landing INSIDE an
        // instruction is the clearest of them - every A64 instruction is four bytes and every A64
        // branch displacement is a word count, so an arm64 branch cannot land anywhere but on an
        // instruction boundary - and a slab displacement past the format's own ceiling is another,
        // because the A64 load's immediate is twelve unsigned bits scaled by eight and cannot
        // reach the ceiling at all. Both are pinned by `NativeTemplateScanChecks` in the
        // slice-compiler root's `--checks` lane, which scans in process and arms nothing.
        Scan(
            "wide-a-native-payload-of-four-zero-bytes",
            [0x00, 0x00, 0x00, 0x00],
            JsNativeArchitecture.X64Windows,
            offset: 0),

        // The prologue with its epilogue cut off: two words that are both templates, and a unit
        // that would run off its last instruction into whatever the allocator put after it.
        Scan(
            "wide-an-emitted-unit-that-ends-without-a-return",
            [0xFD, 0x7B, 0xBC, 0xA9, 0xFD, 0x03, 0x00, 0x91],
            JsNativeArchitecture.Arm64,
            offset: 7),

        // `b #256` and then `ret`. The displacement is a SIGNED WORD count and not a byte one, so
        // the sixty-four in the word is two hundred and fifty-six bytes past a unit that is eight
        // bytes long - which is the arithmetic a scan that read bytes would get wrong by a factor
        // of four and still call in range.
        Scan(
            "wide-a-branch-that-leaves-its-code-unit",
            [0x40, 0x00, 0x00, 0x14, 0xC0, 0x03, 0x5F, 0xD6],
            JsNativeArchitecture.Arm64,
            offset: 0),

        // `movz w0, #1` and then `ret`. THE INSTRUCTION IS ONE THE ENCODER CAN SPELL AND THE VALUE
        // IS ONE NO BACKEND ASKS FOR: `JsNativeReturn` is zero, two and three, and one is
        // deliberately absent because there is no bail-out to an interpreter for it to mean. A
        // check that judged the field by its WIDTH would admit every one of the sixty-five thousand
        // halfwords this form can carry.
        Scan(
            "wide-a-return-code-no-backend-materialises",
            [0x20, 0x00, 0x80, 0x52, 0xC0, 0x03, 0x5F, 0xD6],
            JsNativeArchitecture.Arm64,
            offset: 0),

        // A `ret` and then a word of zeros. The unit is closed as far as its return and the blob is
        // not, which is the shape a payload gets when something was appended to an emission - and
        // the reason the scan requires every byte of the blob to belong to an instantiation rather
        // than requiring the entry point to decode.
        Scan(
            "wide-a-word-after-an-emitted-units-return",
            [0xC0, 0x03, 0x5F, 0xD6, 0x00, 0x00, 0x00, 0x00],
            JsNativeArchitecture.Arm64,
            offset: 4),

        // ---- and the control, without which the five above are satisfied by refusing everything -
        //
        // NINE WORDS THAT ARE ALL TEMPLATES, AND THE ROW RECORDS THAT VERIFICATION PASSED. The
        // entry is refused at INSTANTIATION for the architecture it names, which is the only reason
        // it is safe to retain, and that refusal is itself the evidence the scan admitted it: a
        // payload the scan refused would have answered `InvalidArtifact/InconsistentStructure/1625`
        // at verification and never reached the arm.
        //
        // IT IS NOT THE SAME CLAIM AS THE SINGLE `ret` ABOVE, which is why both are kept. That one
        // is closed trivially - one instantiation, no field, and the first instruction is also the
        // last - and this one reaches the parts of the scan a one-word payload cannot: the frame
        // read the prologue does, a scaled slab displacement the extraction has to descale, the
        // return code the admissible-set arm judges, the stack teardown, and the requirement that
        // the last instruction be a return with eight instructions in front of it.
        //
        // NOTHING HERE CLAIMS THE INSTRUCTIONS ARE THE RIGHT ONES. This is a prologue and an
        // epilogue with a load and a store between them and it computes nothing; a well-formed
        // sequence of the wrong templates is well formed, and re-emission equality is the only
        // layer that reaches the generator. What the row pins is that the layer an execution-only
        // image has does not refuse an emission.
        new CorpusEntry(
            "wide-an-emitted-payload-every-word-of-which-is-a-template",
            Mode,
            "ProfileFault",
            "UnsatisfiedHostAssumption",
            0,
            "-",
            "-",
            "-",
            "-",
            Artifact(
                surfaces: [JsSurfaces.Native],
                nativeCode: [
                    0xFD, 0x7B, 0xBC, 0xA9,  // stp x29, x30, [sp, #-64]!
                    0xFD, 0x03, 0x00, 0x91,  // mov x29, sp
                    0xF6, 0x03, 0x00, 0xAA,  // mov x22, x0
                    0xD3, 0x02, 0x40, 0xF9,  // ldr x19, [x22, #0]
                    0x60, 0x02, 0x40, 0xFD,  // ldr d0, [x19, #0]
                    0x60, 0x02, 0x00, 0xFD,  // str d0, [x19, #0]
                    0x00, 0x00, 0x80, 0x52,  // movz w0, #0
                    0xFD, 0x7B, 0xC4, 0xA8,  // ldp x29, x30, [sp], #64
                    0xC0, 0x03, 0x5F, 0xD6,  // ret
                ],
                nativeSymbolOffset: 0,
                nativeArchitecture: JsNativeArchitecture.Arm64)),

        // ---- two rows that were unreachable while one version was registered ------------------
        //
        // Both are the CALLER mislabelling the bytes, and neither could happen while the profile
        // registered one format version and accepted one manifest: the core screens the descriptor
        // before the profile is called, so anything that got through named the only answer there
        // was. Registering a second of each is what makes them observable, and an entry is what
        // stops the registry carrying two rows whose justification has quietly expired.
        Entry(
            "wide-a-version-1-artifact-announced-as-version-2",
            Broiler.VM.Profile.JavaScript.Compiler.SliceLowering.Constant(1),
            "DescriptorMismatch",
            JavaScriptDiagnosticCodes.DescriptorFormatVersionMismatch),
        Entry(
            "wide-a-slice-manifest-announced-as-the-wide-one",
            Artifact(manifest: "broiler.javascript.slice"),
            "DescriptorMismatch",
            JavaScriptDiagnosticCodes.DescriptorManifestMismatch),

        // ---- and one that verifies, instantiates and runs --------------------------------------
        //
        // Roadmap section 7's corpus discipline makes a corpus in which nothing verifies a release
        // blocker, and it is right: five malformed entries are satisfied by a verifier that refuses
        // everything of this format version. This one is a whole version-2 program - a closure
        // called through a property of an object - and its completion value is what a verifier that
        // refused the format could never produce.
        Ok(
            "wide-a-method-called-through-a-property",
            WideProgram(),
            "7"),
    ];

    /// <summary>A version-2 program that returns a Number, compiled from source.</summary>
    /// <remarks>
    /// Compiled rather than hand-assembled, because the point of the entry is that the whole path -
    /// tokenizer, parser, lowering, verifier, executor - answers with the value the language says.
    /// A hand-assembled artifact would prove the last two and say nothing about the first three.
    /// </remarks>
    private static byte[] WideProgram()
    {
        var compiled = Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Compile(
            "function Point(x) { this.x = x; }\n" +
            "Point.prototype.twice = function () { return this.x * 2; };\n" +
            "new Point(3).twice() + 1;\n",
            Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.Script);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new System.InvalidOperationException(
                "the retained version-2 control did not compile: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return compiled.Artifact;
    }

    /// <summary>A well-formed version-2 entry with a recorded completion value.</summary>
    private static CorpusEntry Ok(string name, byte[] bytes, string completion) =>
        new(name, Mode, "Normal", "NormalCompleted", 0, completion, "-", "-", "-", bytes);

    /// <summary>One malformed version-2 entry.</summary>
    private static CorpusEntry Entry(string name, byte[] bytes, string reason, int code) =>
        new(name, Mode, "InvalidArtifact", reason, code, "-", "-", "-", "-", bytes);

    /// <summary>
    /// One entry whose emitted payload the template-closure scan refuses, pinned by the byte the
    /// scan named.
    /// </summary>
    /// <remarks>
    /// <b>THE POSITION IS THE ONLY COLUMN THAT TELLS THESE ROWS APART AND IT IS NOT DECORATION.</b>
    /// Every clause of the scan answers with one code, so a set of rows recording the triple alone
    /// would be satisfied by a verifier that refused every emitted payload for the first reason it
    /// could think of - which is precisely the wrong implementation a corpus of refusals invites.
    /// The offset the verifier reports is the scan's own blob-relative offset, so a row that
    /// records it is a claim about WHICH byte was found wrong and not merely that one was.
    /// </remarks>
    private static CorpusEntry Scan(
        string name, byte[] code, JsNativeArchitecture architecture, uint offset) =>
        new(
            name,
            Mode,
            "InvalidArtifact",
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.NativePayloadNotTemplateClosed,
            "-",
            "-1:" + offset.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":0:0",
            "-",
            "-",
            Artifact(
                surfaces: [JsSurfaces.Native],
                nativeCode: code,
                nativeSymbolOffset: 0,
                nativeArchitecture: architecture));

    /// <summary>
    /// A version-2 artifact that is well formed except where a parameter says otherwise.
    /// </summary>
    /// <remarks>
    /// One builder with one deviation per call, so that what an entry is about is the argument it
    /// passes and everything else is the same artifact. A separate hand-written byte string per
    /// entry would leave a reader comparing five things to find the one that differs.
    /// </remarks>
    private static byte[] Artifact(
        uint parameterCount = 0,
        uint scopeSlots = 1,
        uint entryUnit = 0,
        uint codeOffset = 0,
        byte[]? code = null,
        bool regionHandlerOutsideUnit = false,
        string? manifest = null,
        string[]? surfaces = null,
        JsFormat.FunctionFlags flags = JsFormat.FunctionFlags.ProgramBody,
        byte[]? nativeCode = null,
        uint? nativeSymbolOffset = null,
        uint? nativeDeclaredLength = null,
        JsNativeArchitecture nativeArchitecture = JsNativeArchitecture.Arm64)
    {
        var body = code ?? [(byte)JsOpcode.LoadConstant, 0x00, 0x00, (byte)JsOpcode.Return];

        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 4)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants([JsArtifactWriter.NumberConstant(1)])),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, body),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([("main", entryUnit)])),
        };

        if (regionHandlerOutsideUnit)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ExceptionRegions,
                JsArtifactWriter.ExceptionRegions(
                [
                    new JsExceptionRegionRow(
                        0, 0, (uint)body.Length, (uint)body.Length + 8, 0, 0,
                        JsFormat.HandlerKind.Catch),
                ])));
        }

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
            JsArtifactWriter.Positions([(0, 1, 1)])));

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
            JsArtifactWriter.Functions(
            [
                new JsFunctionRow(
                    0,
                    parameterCount,
                    scopeSlots,
                    16,
                    codeOffset,
                    (uint)body.Length,
                    (uint)flags),
            ])));

        if (surfaces is not null)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces,
                JsArtifactWriter.Surfaces(surfaces)));
        }

        if (nativeCode is not null)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeCode,
                JsArtifactWriter.NativeCode(
                    (uint)nativeArchitecture,
                    backendSemanticVersion: 0,
                    codeAlignment: 1,
                    nativeCode,
                    nativeDeclaredLength)));
        }

        if (nativeSymbolOffset is { } offset)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeSymbols,
                JsArtifactWriter.NativeSymbols([new JsNativeSymbolRow(0, offset)])));
        }

        return JsArtifactWriter.Write(manifest ?? JsFormat.ManifestId, sections.ToArray());
    }
}
