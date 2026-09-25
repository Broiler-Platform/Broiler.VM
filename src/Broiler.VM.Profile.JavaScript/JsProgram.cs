// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           46
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Critical
// Criteria:         4/4
// Resource impact:  2/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>One code unit of a verified wide-surface program.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0DEC7C
// Broiler-Human:        PENDING
internal sealed class JsCodeUnit
{
    /// <summary>Creates a code unit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=49D34B
    // Broiler-Human:        PENDING
    internal JsCodeUnit(
        string name,
        uint parameterCount,
        uint scopeSlots,
        uint maxOperandStack,
        uint codeOffset,
        uint codeLength,
        Format.JsFormat.FunctionFlags flags)
    {
        Name = name;
        ParameterCount = parameterCount;
        ScopeSlots = scopeSlots;
        MaxOperandStack = maxOperandStack;
        CodeOffset = codeOffset;
        CodeLength = codeLength;
        Flags = flags;
    }

    /// <summary>The unit's name, empty when it is anonymous.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2FD56F
    // Broiler-Human:        PENDING
    internal string Name { get; }

    /// <summary>How many declared parameters the unit has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=995615
    // Broiler-Human:        PENDING
    internal uint ParameterCount { get; }

    /// <summary>How many slots the unit's own environment holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DC8D5D
    // Broiler-Human:        PENDING
    internal uint ScopeSlots { get; }

    /// <summary>The operand-stack height verification computed for this unit.</summary>
    /// <remarks>
    /// It starts as the figure the artifact declared and is REPLACED by the height the abstract
    /// pass computed, which is never larger. The executor sizes its stack from this, so a payload
    /// that declared a generous maximum does not buy a generous allocation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6FCC74
    // Broiler-Human:        PENDING
    internal uint MaxOperandStack { get; set; }

    /// <summary>Where the unit's code starts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B8959E
    // Broiler-Human:        PENDING
    internal uint CodeOffset { get; }

    /// <summary>How long the unit's code is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8AE2C2
    // Broiler-Human:        PENDING
    internal uint CodeLength { get; }

    /// <summary>The unit's flag bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3F7013
    // Broiler-Human:        PENDING
    internal Format.JsFormat.FunctionFlags Flags { get; }

    /// <summary>Whether the unit's code is strict-mode code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5B86DE
    // Broiler-Human:        PENDING
    internal bool IsStrict => (Flags & Format.JsFormat.FunctionFlags.Strict) != 0;

    /// <summary>Whether the unit is an arrow function.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=66E2DF
    // Broiler-Human:        PENDING
    internal bool IsArrow => (Flags & Format.JsFormat.FunctionFlags.Arrow) != 0;

    /// <summary>Whether the frame must materialise an <c>arguments</c> object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AA2DE3
    // Broiler-Human:        PENDING
    internal bool UsesArguments => (Flags & Format.JsFormat.FunctionFlags.UsesArguments) != 0;

    /// <summary>Whether the unit is a class constructor, which no call may reach.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C4661A
    // Broiler-Human:        PENDING
    internal bool IsClassConstructor =>
        (Flags & Format.JsFormat.FunctionFlags.ClassConstructor) != 0;

    /// <summary>Whether the unit's <c>this</c> is created by its <c>super()</c> rather than by its caller.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7FD2F4
    // Broiler-Human:        PENDING
    internal bool IsDerivedConstructor =>
        (Flags & Format.JsFormat.FunctionFlags.DerivedConstructor) != 0;

    /// <summary>Whether the unit's own prologue binds its parameters, so the frame copies none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EF5279
    // Broiler-Human:        PENDING
    internal bool BindsParameters =>
        (Flags & Format.JsFormat.FunctionFlags.BindsParameters) != 0;

    /// <summary>
    /// Whether calling the unit builds a generator object instead of running its code.
    /// </summary>
    /// <remarks>
    /// <b>This bit test is the WHOLE of what the ordinary call path pays for generators
    /// existing.</b> It reads a field the invocation had already loaded, and it decides whether the
    /// frame lives in the interpreter's own locals - which is what every ordinary call still gets,
    /// unchanged - or on the heap where a suspension can leave it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CF19D5
    // Broiler-Human:        PENDING
    internal bool IsGenerator => (Flags & Format.JsFormat.FunctionFlags.Generator) != 0;

    /// <summary>
    /// Whether calling the unit STARTS its code on a heap frame and answers a promise.
    /// </summary>
    /// <remarks>
    /// <b>The verb is what separates this from <see cref="IsGenerator"/>.</b> A generator's call
    /// runs no instruction of its body; an async function's call runs the body straight through to
    /// its first <c>await</c>, on the same native stack the caller is on, and only then returns.
    /// The two share the frame representation and share none of the timing, and a program can see
    /// the difference on its very first line.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=91E0E7
    // Broiler-Human:        PENDING
    internal bool IsAsync => (Flags & Format.JsFormat.FunctionFlags.Async) != 0;
}

/// <summary>One verified exception region.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7DCA8B
// Broiler-Human:        PENDING
internal readonly struct JsRegion(
    uint unit,
    uint tryStart,
    uint tryEnd,
    uint handler,
    uint scopeDepth,
    uint stackHeight,
    Format.JsFormat.HandlerKind kind)
{
    /// <summary>Which code unit the region belongs to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=175148
    // Broiler-Human:        PENDING
    internal uint Unit { get; } = unit;

    /// <summary>The first code offset the region covers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=61A751
    // Broiler-Human:        PENDING
    internal uint TryStart { get; } = tryStart;

    /// <summary>The first code offset after the region.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D123E4
    // Broiler-Human:        PENDING
    internal uint TryEnd { get; } = tryEnd;

    /// <summary>Where control goes when the region catches.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=349401
    // Broiler-Human:        PENDING
    internal uint Handler { get; } = handler;

    /// <summary>How many environments deep the handler runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2DCEC1
    // Broiler-Human:        PENDING
    internal uint ScopeDepth { get; } = scopeDepth;

    /// <summary>The stack height the handler is entered at, before the thrown value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=695B57
    // Broiler-Human:        PENDING
    internal uint StackHeight { get; } = stackHeight;

    /// <summary>Whether the handler catches or rethrows.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4DEDD6
    // Broiler-Human:        PENDING
    internal Format.JsFormat.HandlerKind Kind { get; } = kind;
}

/// <summary>One named entry point, naming a code unit.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=92C350
// Broiler-Human:        PENDING
internal readonly struct JsEntry(string name, uint unit)
{
    /// <summary>The entry-point name a caller passes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C26712
    // Broiler-Human:        PENDING
    internal string Name { get; } = name;

    /// <summary>Which code unit the entry runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=175148
    // Broiler-Human:        PENDING
    internal uint Unit { get; } = unit;
}

/// <summary>
/// A verified format-version-2 program: the immutable half of what an instance runs.
/// </summary>
/// <remarks>
/// Nothing here is per-instance and nothing here is mutable, which is what makes a verified handle
/// shareable between runtimes. Every object a program creates - the realm, the global, every
/// closure - lives on the instance side, so two instances over one handle observe nothing of each
/// other even though they execute the same bytes.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=24F0F9
// Broiler-Human:        PENDING
internal sealed class JsProgram : IVmVerifiedState
{
    /// <summary>Creates a verified program.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=EC37D5
    // Broiler-Human:        PENDING
    internal JsProgram(
        JsValue[] constants,
        string[] names,
        byte[] code,
        JsCodeUnit[] functions,
        JsRegion[] regions,
        JsEntry[] entries,
        int positionRowCount,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces,
        JsModuleRecord[]? modules = null,
        JsBinding[]? importBindings = null,
        string? manifestId = null,
        Format.JsNativeArchitecture nativeArchitecture = Format.JsNativeArchitecture.None,
        uint nativeBackendVersion = 0,
        uint nativeCodeAlignment = 0,
        byte[]? nativeCode = null,
        Format.JsNativeSymbolRow[]? nativeSymbols = null,
        JsEvalMap? evalMap = null,
        System.Collections.Generic.Dictionary<int, JsScriptDeclaration>? scriptDeclarations = null,
        System.Collections.Generic.Dictionary<int, string>? scriptReferrers = null,
        bool nativeValueForm = false,
        Format.JsNativeProgramImage? nativeValueImage = null)
    {
        Constants = constants;
        Names = names;
        Code = code;
        Functions = functions;
        Regions = regions;
        Entries = entries;
        PositionRowCount = positionRowCount;
        AdmittedSurfaces = admittedSurfaces;
        Modules = modules ?? [];
        ImportBindings = importBindings ?? [];
        ModuleOfUnit = MapUnits(Modules, functions.Length);
        ManifestId = manifestId ?? Format.JsFormat.ManifestId;
        NativeArchitecture = nativeArchitecture;
        NativeBackendVersion = nativeBackendVersion;
        NativeCodeAlignment = nativeCodeAlignment;
        NativeCode = nativeCode ?? [];
        NativeSymbols = nativeSymbols ?? [];
        EvalMap = evalMap;
        ScriptDeclarations = scriptDeclarations;
        ScriptReferrers = scriptReferrers;
        NativeValueForm = nativeValueForm;
        NativeValueImage = nativeValueImage;
        valuePlans = nativeValueImage is null ? [] : new Format.JsValueUnitPlan?[functions.Length];
    }

    /// <summary>The value-form plans made so far, one slot per code unit, filled on a unit's first entry.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=02CFED
    // Broiler-Human:        PENDING
    private readonly Format.JsValueUnitPlan?[] valuePlans;

    /// <summary>The handler offsets of the value image, grouped once, on the first plan made.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=30F743
    // Broiler-Human:        PENDING
    private Format.JsBaselineHandlerOffsets? valueHandlerOffsets;

    /// <summary>The image a value-form payload was scanned against, or nothing for any other program.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BC2C12
    // Broiler-Falsified-If: this differs from the image the verifier scanned and re-emitted the value-form payload against
    // Broiler-Human:        PENDING
    internal Format.JsNativeProgramImage? NativeValueImage { get; }

    /// <summary>
    /// The value plan of one code unit - its heights, its region and its residency - made from the image the
    /// payload was scanned against the first time the unit is entered, or nothing where there is none.
    /// </summary>
    /// <remarks>
    /// <b>A PROGRAM IS SHARED, SO A PLAN IS PUBLISHED ONCE AND NEVER REPLACED.</b> Two threads that plan one
    /// unit at once make two equal plans, because the plan is a pure function of the image, and the first
    /// published is the one both read.
    /// </remarks>
    /// <param name="unitIndex">The code unit.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=6F3F18
    // Broiler-Falsified-If: it answers a plan other than the one the template scan held the unit's payload to, or replaces a published plan
    // Broiler-Human:        PENDING
    internal Format.JsValueUnitPlan? ValuePlan(int unitIndex)
    {
        if (NativeValueImage is null || (uint)unitIndex >= (uint)valuePlans.Length)
        {
            return null;
        }

        var published = System.Threading.Volatile.Read(ref valuePlans[unitIndex]);

        if (published is not null)
        {
            return published;
        }

        var offsets = System.Threading.Volatile.Read(ref valueHandlerOffsets);

        if (offsets is null)
        {
            offsets = Format.JsBaselineBlocks.GroupHandlerOffsets(NativeValueImage);
            System.Threading.Interlocked.CompareExchange(ref valueHandlerOffsets, offsets, null);
        }

        if (!Format.JsValueLayout.TryPlan(
                NativeValueImage, unitIndex, offsets.Of(unitIndex), NativeValueImage.ResidentBindings, out var plan, out _))
        {
            return null;
        }

        return System.Threading.Interlocked.CompareExchange(ref valuePlans[unitIndex], plan, null) ?? plan;
    }

    /// <summary>Whether the emitted code is the value form rather than the manifest's own native form.</summary>
    /// <remarks>
    /// <b>It is the emitted-code header's form byte, carried as the verifier read it</b>, so an instance
    /// enters the value form's helpers exactly when the payload was scanned and re-emitted as the value
    /// form (JSD-0035 section 9).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=942D8A
    // Broiler-Falsified-If: this differs from the form byte of the emitted code section the verifier scanned
    // Broiler-Human:        PENDING
    internal bool NativeValueForm { get; }

    /// <summary>The eval scope map the artifact carries, or nothing.</summary>
    /// <remarks>
    /// <b>Its absence is what every artifact written before the section existed says</b>: no
    /// direct-<c>eval</c> site of this program sees anything, so each keeps the explicit refusal it
    /// always had, and no unit of it is eval code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=522B94
    // Broiler-Human:        PENDING
    internal JsEvalMap? EvalMap { get; }

    /// <summary>What each script body declares at the global scope, by unit, or nothing.</summary>
    /// <remarks>
    /// <b>Its absence is what every artifact written before the section existed says</b> (JSeal
    /// V15-host): its scripts keep the lenient instantiation they always had, and a script body with
    /// no row declares nothing a check could refuse.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A1A60F
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<int, JsScriptDeclaration>? ScriptDeclarations { get; }

    /// <summary>The referrer each placed script body carries, by unit, or nothing.</summary>
    /// <remarks>
    /// <b>Its absence is what every artifact written before the section existed says</b> (JSeal
    /// I12-upstream): its scripts are placed nowhere, so eval code and <c>Function</c> bodies they
    /// create offer an embedder the empty referrer they always did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=01008F
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.Dictionary<int, string>? ScriptReferrers { get; }

    /// <summary>The feature manifest the artifact named in its header.</summary>
    /// <remarks>
    /// <b>It is carried because the manifest is the artifact's FORM and the form of a verified
    /// handle never changes.</b> Two manifests are read at this format version - the wide surface
    /// and its numeric narrowing - and which of them an artifact named is a fact about the artifact
    /// that nothing later may re-decide.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3D8852
    // Broiler-Human:        PENDING
    internal string ManifestId { get; }

    /// <summary>The instruction set the emitted code was written for, or none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9EAAF2
    // Broiler-Human:        PENDING
    internal Format.JsNativeArchitecture NativeArchitecture { get; }

    /// <summary>The version of the backend that emitted the code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5A0CC0
    // Broiler-Human:        PENDING
    internal uint NativeBackendVersion { get; }

    /// <summary>The alignment the emitted code was written at.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1C1391
    // Broiler-Human:        PENDING
    internal uint NativeCodeAlignment { get; }

    /// <summary>
    /// The emitted machine code, empty when the artifact carries none.
    /// </summary>
    /// <remarks>
    /// <b>THIS BUILD CARRIES THESE BYTES AND RUNS NONE OF THEM.</b> No page is mapped, no page is
    /// armed and nothing calls into the blob; it is retained because a verified handle must hold
    /// everything its artifact declared, and because re-emission equality - recompiling the
    /// bytecode this handle also carries and comparing - is a claim nobody can check about bytes
    /// that were discarded at verification.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0ADB4A
    // Broiler-Human:        PENDING
    internal byte[] NativeCode { get; }

    /// <summary>Where each code unit's emitted code starts in <see cref="NativeCode"/>.</summary>
    /// <remarks>
    /// It has one row per code unit or none at all. A table naming some units and not others is
    /// refused at verification, because an artifact whose units do not share one form is the
    /// per-unit executor choice this profile's non-goals refuse.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6FC5E6
    // Broiler-Human:        PENDING
    internal Format.JsNativeSymbolRow[] NativeSymbols { get; }

    /// <summary>The armed mapping of <see cref="NativeCode"/>, once an engine of the baseline form mapped it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=F05E06
    // Broiler-Falsified-If: this is written more than once for one program, or holds a mapping that is not armed
    // Broiler-Human:        PENDING
    internal JsNativePage? NativePage;

    /// <summary>The module records, empty when the artifact carries none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=68CF90
    // Broiler-Human:        PENDING
    internal JsModuleRecord[] Modules { get; }

    /// <summary>
    /// Every import entry of the artifact, resolved to the binding it reads.
    /// </summary>
    /// <remarks>
    /// The table is the artifact's rather than each module's, because an import read carries an
    /// index into it and the executor must be able to follow that index without first working out
    /// which module the running code unit belongs to.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2AAC1D
    // Broiler-Human:        PENDING
    internal JsBinding[] ImportBindings { get; }

    /// <summary>Which module each code unit belongs to, or -1 for a unit that is not a module.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=949234
    // Broiler-Human:        PENDING
    internal int[] ModuleOfUnit { get; }

    /// <summary>Indexes the module bodies by code unit, so an invocation can recognise one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B0B137
    // Broiler-Human:        PENDING
    private static int[] MapUnits(JsModuleRecord[] modules, int unitCount)
    {
        var map = new int[unitCount];
        System.Array.Fill(map, -1);

        for (var index = 0; index < modules.Length; index++)
        {
            map[modules[index].BodyUnit] = index;
        }

        return map;
    }

    /// <summary>The optional feature manifests the composition that verified this admits.</summary>
    /// <remarks>
    /// It travels on the verified state rather than being asked for again at execution, because the
    /// composition's answer is fixed the moment it registers a profile descriptor and asking twice
    /// is how two answers happen. The realm reads it to decide which intrinsics exist.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CAB5B1
    // Broiler-Human:        PENDING
    internal System.Collections.Immutable.ImmutableArray<string> AdmittedSurfaces { get; }

    /// <summary>Whether the composition admitted <paramref name="manifestId"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9D7A37
    // Broiler-Human:        PENDING
    internal bool Admits(string manifestId) =>
        !AdmittedSurfaces.IsDefault && AdmittedSurfaces.Contains(manifestId);

    /// <summary>The constant pool, as values.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4D5B5E
    // Broiler-Human:        PENDING
    internal JsValue[] Constants { get; }

    /// <summary>
    /// The constant pool, as strings, for the entries that are names or Strings.
    /// </summary>
    /// <remarks>
    /// A property access reads this rather than <see cref="Constants"/>, so the common path never
    /// asks a value what kind it is before using it as a key. An entry that is not textual holds
    /// the empty string here and the verifier has already refused every instruction that would
    /// read it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2F955F
    // Broiler-Human:        PENDING
    internal string[] Names { get; }

    /// <summary>The code section, holding every unit's code back to back.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0F8DCF
    // Broiler-Human:        PENDING
    internal byte[] Code { get; }

    /// <summary>The code units. Unit zero is the program body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8A0F66
    // Broiler-Human:        PENDING
    internal JsCodeUnit[] Functions { get; }

    /// <summary>The exception regions, innermost first within a unit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2D7445
    // Broiler-Human:        PENDING
    internal JsRegion[] Regions { get; }

    /// <summary>The named entry points.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5988F0
    // Broiler-Human:        PENDING
    internal JsEntry[] Entries { get; }

    /// <summary>How many rows the position table declares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DA6E53
    // Broiler-Human:        PENDING
    internal int PositionRowCount { get; }

    /// <summary>Finds the entry point named <paramref name="name"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C2FDD7
    // Broiler-Human:        PENDING
    internal bool TryFindEntry(string name, out uint unit)
    {
        foreach (var entry in Entries)
        {
            if (string.Equals(entry.Name, name, System.StringComparison.Ordinal))
            {
                unit = entry.Unit;
                return true;
            }
        }

        unit = 0;
        return false;
    }
}

/// <summary>What one script body declares at the global scope (JSeal V15-host).</summary>
/// <remarks>
/// <b>These are the specification's lists for <c>GlobalDeclarationInstantiation</c></b>: the
/// lexically declared names, the <c>var</c> names that are no function's, the functions one per name,
/// and the block-level functions Annex B may hoist. The body's own instructions create the bindings;
/// the executor reads this to run every check before the first of them.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8F4420
// Broiler-Human:        PENDING
internal sealed class JsScriptDeclaration(
    string[] lexicalNames, string[] varNames, string[] functionNames, string[] annexBNames)
{
    /// <summary>The top-level <c>let</c>, <c>const</c> and <c>class</c> names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=20F5C1
    // Broiler-Human:        PENDING
    internal string[] LexicalNames { get; } = lexicalNames;

    /// <summary>The <c>var</c> names, none of which is also a top-level function's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DBEF61
    // Broiler-Human:        PENDING
    internal string[] VarNames { get; } = varNames;

    /// <summary>The top-level function names, one per name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48F7EA
    // Broiler-Human:        PENDING
    internal string[] FunctionNames { get; } = functionNames;

    /// <summary>The block-level functions Annex B may hoist to the global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=380361
    // Broiler-Human:        PENDING
    internal string[] AnnexBNames { get; } = annexBNames;
}
