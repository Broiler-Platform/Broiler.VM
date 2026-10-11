// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   27
// Annotated:        27/27
// Exempt:           28
// Human-reviewed:   0/27
// IP risk:          Low
// Security risk:    High
// Criteria:         15/15
// Resource impact:  5/10 max
// Unverified:       27
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The wide-surface engine: its realms, the abstract operations over them, and the dispatch loop.
/// </summary>
/// <remarks>
/// <para>
/// <b>One class holds the operations and the loop because they call each other in both
/// directions.</b> <c>ToPrimitive</c> calls <c>valueOf</c>, which may be a bytecode function, which
/// runs on the loop, which calls <c>ToPrimitive</c>. Splitting them would mean an interface between
/// two halves of one thing, and the interface would be a delegate field on each side.
/// </para>
/// <para>
/// <b>Fuel is charged per instruction and per call.</b> Not per second: two runs of the same
/// program on two machines stop at the same instruction. A built-in that does bounded work charges
/// once; a built-in whose work is proportional to an argument - sorting, joining, matching - charges
/// proportionally, so a program cannot buy unbounded work with one instruction.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A41ED2
    // Broiler-Human:        PENDING
    private const int FuelPerInstruction = 1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D9AC66
    // Broiler-Human:        PENDING
    private readonly IVmMeter meter;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=454C09
    // Broiler-Human:        PENDING
    private readonly CancellationToken cancellation;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2CF56D
    // Broiler-Human:        PENDING
    private readonly ImmutableArray<string> surfaces;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=329437
    // Broiler-Human:        PENDING
    private int depth;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2AB034
    // Broiler-Human:        PENDING
    private ulong sinceLastPoll;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2E76EA
    // Broiler-Human:        PENDING
    private readonly IVmHostCapabilityInvoker? capabilities;

    /// <summary>Whether every program this engine runs carries the baseline native form.</summary>
    /// <remarks>
    /// <b>ONE FORM PER INSTANCE, FIXED WHEN THE INSTANCE IS BUILT.</b> The instance's own artifact
    /// decided it, and a program a guest loads later must carry the same form or the load is a
    /// defect: an engine that ran one program interpreted and the next one emitted would be choosing
    /// an execution form per program, which is the per-unit choice this profile's non-goals refuse.
    /// It is checked three times - at instantiation, at every nested load, and on every entry into
    /// the dispatch loop - because each check is one comparison and each catches a different route.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=05B362
    // Broiler-Falsified-If: an engine built for one form runs a program of the other form
    // Broiler-Human:        PENDING
    private readonly bool nativeForm;

    /// <summary>Whether the native form every program of this engine carries is the value form.</summary>
    /// <remarks>
    /// <b>Fixed with <see cref="nativeForm"/> and checked where it is</b>: the value form is a native form
    /// of its own (JSD-0035 section 1), so a baseline program reaching a value-form engine, or the reverse,
    /// is the same defect as a bytecode program reaching a native one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=921708
    // Broiler-Falsified-If: an engine built for the value form runs a baseline program's emitted code, or the reverse
    // Broiler-Human:        PENDING
    private readonly bool valueForm;

    /// <summary>Creates an engine over a fresh realm.</summary>
    /// <remarks>
    /// <b>A value-form engine owns its instance's words and handles from the start</b>: one value stack
    /// and one handle table rooted by it, the table under handle-stress when the composition asked for it
    /// (JSD-0035 sections 3 and 4). Every other engine allocates neither.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4316D1
    // Broiler-Human:        PENDING
    internal JsEngine(
        IVmMeter contractMeter,
        CancellationToken token,
        IVmHostCapabilityInvoker? invoker = null,
        ImmutableArray<string> admittedSurfaces = default,
        bool nativeForm = false,
        bool valueForm = false,
        bool handleStress = false,
        bool sweepsFinalization = false,
        JsIntlTables? intl = null)
    {
        meter = contractMeter;
        Intl = intl;
        cancellation = token;
        capabilities = invoker;
        this.nativeForm = nativeForm || valueForm;
        this.valueForm = valueForm;
        SweepsFinalization = sweepsFinalization;

        if (valueForm)
        {
            ValueStack = new JsValueStack();
            ValueHandles = new JsHandleTable(ValueStack, handleStress);
        }

        // THE SURFACE SET IS ASSIGNED BEFORE THE REALM IS BUILT AND NOT AFTER, because the realm's
        // constructor is what decides which intrinsics exist. A realm handed the set afterwards
        // would have to be able to grow a global, and a realm that can grow one is a realm whose
        // contents depend on when you looked.
        surfaces = admittedSurfaces.IsDefault
            ? []
            : admittedSurfaces;

        FirstRealm = Realm = new JsRealm(this);
    }

    /// <summary>
    /// The jobs that have fallen due and not yet run: the microtask queue, in enqueue order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a field of the engine and therefore of the realm, not of an invocation.</b> A
    /// promise resolved during one script whose reaction has not run yet is still owed when the
    /// next script starts, which is exactly what a host running several scripts in one realm needs;
    /// an invocation-scoped queue would silently drop it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=418701
    // Broiler-Human:        PENDING
    private readonly Queue<(JsValue Callable, JsValue[] Arguments, string Referrer)> jobs = new();

    /// <summary>Whether any job is waiting.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2DE9B8
    // Broiler-Human:        PENDING
    internal bool HasPendingJobs => jobs.Count != 0;

    /// <summary>How many jobs are due and have not run.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5235ED
    // Broiler-Human:        PENDING
    internal int PendingJobCount => jobs.Count;

    /// <summary>Whether the composition admitted the optional surface <paramref name="manifestId"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=486D64
    // Broiler-Human:        PENDING
    internal bool Admits(string manifestId) => surfaces.Contains(manifestId);

    /// <summary>
    /// The internationalization tables the composition handed over, or nothing; a realm builds
    /// <c>Intl</c> only where the surface is admitted and these are present (JSD-0043).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=806EEF
    // Broiler-Human:        PENDING
    internal JsIntlTables? Intl { get; }

    /// <summary>
    /// The mediator this invocation may ask for further executable bytes through, or nothing.
    /// </summary>
    /// <remarks>
    /// <b>It is set per invocation and cleared after it, because that is the contract.</b> A
    /// mediator is valid only for the dynamic extent of the invocation that supplied it, and a
    /// profile that retained one and used it later would be naming a mediator the core reports as
    /// out of scope. Holding it on the engine rather than threading it through every frame is the
    /// only concession, and the clearing is what keeps it honest.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=67C7AE
    // Broiler-Falsified-If: a mediator is used outside the invocation that supplied it
    // Broiler-Human:        PENDING
    internal IVmArtifactLoadMediator? Loader { get; set; }

    /// <summary>
    /// Writes one line of text to whatever the composition registered, or nowhere.
    /// </summary>
    /// <remarks>
    /// The two sinks are deliberately different things. <see cref="Output"/> is an in-process hook
    /// a test host sets to capture what a program printed; the capability is the host boundary a
    /// real composition registers. A program that prints reaches both when both exist and neither
    /// when neither does, and in no case does it reach a console this profile opened itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=9D27C2
    // Broiler-Human:        PENDING
    internal void Write(string text)
    {
        Output?.Invoke(text);

        if (capabilities is null ||
            capabilities.BindingCount <= JavaScriptProfile.WriteBindingIndex ||
            !capabilities.IsBound(JavaScriptProfile.WriteBindingIndex))
        {
            return;
        }

        if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the host-call allowance is spent");
        }

        var bytes = Encoding.UTF8.GetBytes(text);

        capabilities.InvokeBytes(
            JavaScriptProfile.WriteBindingIndex, new VmBytes(bytes), out _);
    }

    /// <summary>The embedder's view of this engine's realm, built on first use.</summary>
    /// <remarks>
    /// <b>Lazily, because most realms never have one.</b> The seam costs two weak tables and a
    /// handful of fields, and a composition that registered no host surface should pay for none of
    /// it - so the absence of an embedder is the absence of an object rather than an object with
    /// nothing in it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AEEA62
    // Broiler-Human:        PENDING
    internal JsHostRealm HostRealm => hostRealm ??= new JsHostRealm(this, FirstRealm, null);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FF6FDD
    // Broiler-Human:        PENDING
    private JsHostRealm? hostRealm;

    /// <summary>Opens the host surface's window, where one was ever asked for.</summary>
    /// <remarks>
    /// It is a no-op for an engine with no embedder, which is what keeps the bracket free on every
    /// realm that never had a host object in it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F15254
    // Broiler-Human:        PENDING
    internal void BeginHostStep() => hostRealm?.BeginStep();

    /// <summary>Closes the window, answering an abort host code caught and discarded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=39C37F
    // Broiler-Falsified-If: an abort latched at the seam is dropped when the step ends
    // Broiler-Human:        PENDING
    internal JsAbort? EndHostStep() => hostRealm?.EndStep();


    /// <summary>Whatever the host wired to <c>print</c>, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C44B64
    // Broiler-Human:        PENDING
    internal Action<string>? Output { get; set; }

    /// <summary>
    /// The deepest the call stack may go before the operation ends as a resource exhaustion.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the ORDINARY answer for a recursing program, and the budget ceiling is the
    /// tighter bound a host may impose.</b> The two were the other way round until 2026-09-04, and
    /// the reversal is a correction rather than a preference *(JSC-96)*: while the ceiling answered
    /// first, a stack overflow was a resource exhaustion no guest could see, so
    /// <c>try { recurse(); } catch (e) { }</c> — which a recursive descent probing its own depth, a
    /// benchmark sizing a workload and a conformance case asserting the error's type all write —
    /// never ran its own guard. <c>Maximum call stack size exceeded</c> is a catchable exception in
    /// every engine and it is one here.
    /// </para>
    /// <para>
    /// <b>It is a counted number, and the runtime's stack probe sits in front of it rather than
    /// beside it.</b> The probe answers whether there is room to do anything at all; when it says
    /// no, the operation ENDS, because building and dispatching an error object from there is what
    /// terminated the process at 3,000 frames *(JSC-85)*. The counted bound is reached with the
    /// probe still satisfied, so throwing from it is safe. Folding the two into one condition —
    /// which is what this was — gave the unsafe case's answer to the safe one.
    /// </para>
    /// <para>
    /// <b>The figure is MEASURED and not chosen</b>: <c>eng/measure-frame-cost.py</c> bisects the
    /// published binary against a recursion with no base case and finds that this interpreter
    /// survives 17,963 JavaScript calls on the sixty-four-megabyte stack
    /// <see cref="JsExecution"/> declares — 3,736 bytes of native stack per call, and the same
    /// figure whether the JavaScript frame is narrow or wide, because the operand stack and the
    /// environment are heap objects rather than stack ones. A guest `throw` unwinds from the same
    /// depth, which it did not before the executor caught by FILTER rather than by
    /// catch-and-rethrow *(JSC-97)*. This bound is set at 6,000, under a third of the measurement,
    /// with the margin for a call shape costing more than the measured one and for the frames the
    /// refusal's own error object needs.
    /// </para>
    /// <para>
    /// <b>BOTH HALVES OF THAT MEASUREMENT MOVE WHEN THE INSTRUCTION SET DOES, and re-measuring is
    /// not optional.</b> The per-call cost is the executor's own frame, which a switch sizes for
    /// the widest live set across all of its arms, so every bundle that adds cases to the dispatch
    /// loop grows it. Admitting spread, destructuring and <c>for … of</c> alongside classes grew it
    /// from 1,936 bytes to 3,158 - which put 6,000 frames past what the sixteen megabytes then
    /// declared held, so a runaway recursion terminated the process instead of throwing. That is
    /// JSC-85 exactly, reached by arithmetic rather than by a code change, and it is why the stack
    /// was re-measured and re-declared rather than this bound quietly lowered. Admitting the
    /// generator family grew it again, from 3,158 bytes to 3,463 - the two suspension arms and the
    /// heap frame they read - and the sixty-four megabytes still hold 19,377 calls, so the bound is
    /// still under a third of the capacity and the stack did not have to move. Admitting the ASYNC
    /// family and <c>with</c> grew it once more, from 3,463 bytes to 3,736 - one more suspension
    /// arm, the two locals the async driver carries across its own try, and the scope-chain walk a
    /// dynamic name resolution holds - and the capacity fell from 19,377 calls to 17,963, which is
    /// still more than twice the ceiling a host may be granted. The two families were measured
    /// apart before they were measured together, at 18,277 and 19,288 calls, and NEITHER of those
    /// figures describes the build that ships both: a per-frame cost is a property of the whole
    /// dispatch loop, so it is measured on the tree that has everything in it. The measurement
    /// was re-taken each time anyway, because a bound that is safe by arithmetic nobody re-did is a
    /// bound nobody knows is safe. Admitting the CLASS BODY - fields, static blocks, private names
    /// and a generator member - added six arms and grew it once more, from 3,736 bytes to
    /// <b>4,073</b>, and the capacity fell from 17,963 calls to <b>16,478</b>: 2.75 times this
    /// bound and 2.01 times the ceiling a host may be granted, so nothing had to move
    /// <i>(JSC-126)</i>.
    /// </para>
    /// <para>
    /// <b>AND THE TIME AFTER THAT, SOMETHING DID HAVE TO MOVE.</b> Asynchronous iteration adds five
    /// dispatch arms - the four steps of a <c>for await</c> head and the check its close owes - and
    /// grew the executor's frame from 4,073 bytes to <b>4,551</b>. On the sixty-four megabytes
    /// <see cref="JsExecution"/> then declared that is <b>14,737</b> calls, which is 1.80 times the
    /// ceiling a host may be granted: BELOW the factor of two the previous measurement already
    /// called the narrowest it had been, so the ordering this bound exists to guarantee stopped
    /// being guaranteed. The stack was raised to ninety-six megabytes and re-measured at
    /// <b>22,122</b> calls, which is 3.69 times this bound and 2.70 times that ceiling
    /// <i>(JSC-139)</i>. Raising the stack rather than lowering this bound is the same choice
    /// JSC-85 made and for the same reason: this bound is about what a program may do, and the
    /// stack is about what the machine can hold. The stack moved once more on 2026-09-17, to two
    /// hundred and eight megabytes, because a route table wider than the plain call every figure
    /// above was taken on found the same ordering failing on routes those measurements never
    /// reached - and this bound did not move, for the reason it has never moved <i>(JSC-226)</i>.
    /// </para>
    /// <para>
    /// <b>An <c>await</c>'s resumption does NOT stack, which is the one thing about this family
    /// that could have made the figure misleading.</b> A <c>yield*</c> chain holds one interpreter
    /// frame per level because each resumption is nested inside the last; an async chain does not,
    /// because every resumption starts from the job queue with the previous frame already returned.
    /// So an async function awaiting for ever spends <c>Fuel</c> and never the stack, and it is the
    /// SYNCHRONOUS part of an async body - a call before the first <c>await</c> - that this bound
    /// governs, exactly as it governs an ordinary call.
    /// </para>
    /// <para>
    /// <b>What a host can still do is narrow it.</b> The <c>CallDepth</c> budget is charged on every
    /// call and its exhaustion is an abort the guest cannot catch, which is what roadmap section 8
    /// asks for in those words. A host that wants a program refused at a hundred frames sets the
    /// ceiling there and gets it. What a host cannot do is widen past this bound, because this bound
    /// is about the native stack rather than about policy.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=321EE5
    // Broiler-Falsified-If: a program recursing past this bound terminates the process rather than throwing a catchable RangeError
    // Broiler-Human:        PENDING
    internal int MaximumCallDepth { get; set; } = 6000;

    /// <summary>Whether this engine is in the middle of reporting a call-depth refusal.</summary>
    /// <remarks>
    /// It exists so the refusal can allocate. Every other reading of it would be a reason to delete
    /// it, and the one that matters is in <see cref="Call"/>: without it the bound refuses the
    /// frames its own error object needs.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DA5D1D
    // Broiler-Falsified-If: this stays set after the refusal has been thrown, so a later recursion is unbounded
    // Broiler-Human:        PENDING
    private bool reportingDepth;

    // ---- metering ------------------------------------------------------------------------------

    /// <summary>
    /// How much work this engine performs between two polls.
    /// </summary>
    /// <remarks>
    /// It is HALF the profile's declared cancellation poll bound, and the halving is what makes the
    /// declaration true rather than nearly true: a charge is added before the poll is considered,
    /// so the most work that can accumulate between two polls is one window plus the charge that
    /// crossed it. A single charge larger than a window is split, because a built-in charging
    /// proportionally to a megabyte-long string would otherwise breach the bound in one call - which
    /// is what the RegExp benchmark did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=733ADC
    // Broiler-Human:        PENDING
    private const ulong PollWindow = 16_384;

    /// <summary>
    /// <see cref="Charge"/> as a delegate, made once, which is how the BigInt operations charge fuel
    /// - and so meet the cancellation poll - between the steps of a long operation.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DA6294
    // Broiler-Human:        PENDING
    internal Action<ulong> ChargeFuel => chargeFuel ??= Charge;

    /// <summary>The delegate <see cref="ChargeFuel"/> made.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B83AE1
    // Broiler-Human:        PENDING
    private Action<ulong>? chargeFuel;

    /// <summary>What a call is charged for itself, over the instruction that makes it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=176597
    // Broiler-Human:        PENDING
    private const ulong CallCharge = 4;

    /// <summary>
    /// What a value-form direct call charges before it enters its callee, in one charge: the pure instructions
    /// before it, the <c>Call</c> instruction and the call itself.
    /// </summary>
    /// <remarks>
    /// <b>One charge where the interpreter makes three, and the same verdict at every ceiling</b>: nothing
    /// between the three is observable, and a charge is all-or-nothing, so an allowance that cannot take the
    /// sum ends the operation before anything the interpreter would have run past its own exhaustion is seen
    /// (JSD-0035 section 7).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5DBA5E
    // Broiler-Falsified-If: a direct call is charged other than the instruction's unit and the call's four over its debt
    // Broiler-Human:        PENDING
    internal const long DirectCallCharge = FuelPerInstruction + (long)CallCharge;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=305714
    // Broiler-Human:        PENDING
    internal string Describe(JsValue value) => value.Type switch
    {
        JsType.Undefined => "undefined",
        JsType.Null => "null",
        JsType.String => "\"" + value.AsString() + "\"",
        JsType.Number => JsNumberFormat.ToJsString(value.AsNumber()),
        JsType.Boolean => value.AsBoolean() ? "true" : "false",

        // A SYMBOL NEEDS ITS OWN ARM AND THE DEFAULT WAS NOT ONE. This described anything that was
        // not one of the five above by asking it whether it was callable, which reads the value as
        // an OBJECT - and a Symbol is not one. So `Symbol()()` - a call of a Symbol, which every
        // engine answers with a TypeError - reached this while building that very TypeError's
        // message, failed the cast, and ended the whole invocation as a contract violation: an
        // internal fault, uncatchable, in place of the language's own error.
        JsType.Symbol => value.AsSymbol().Rendered,

        // Described the way a literal spells it, and charged the way `ToString` is.
        JsType.BigInt => BigIntText(value.AsBigInt()) + "n",
        _ => value.IsObject && value.AsObject().IsCallable ? "function" : "object",
    };

    /// <summary>Runs a program's entry point and answers what it completed with.</summary>
    /// <remarks>
    /// <b>The frame's referrer is the script's own</b> - the one its artifact placed it at, or none -
    /// unless <paramref name="referrer"/> states another: a program the <c>Function</c> constructor
    /// compiled is dynamic code, and runs as its caller's (JSD-0024 section 20).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=694F2B
    // Broiler-Human:        PENDING
    internal JsValue RunEntry(JsProgram program, uint unit, string? referrer = null)
    {
        if (program.ModuleOfUnit[(int)unit] is var moduleIndex and >= 0)
        {
            return RunModuleGraph(program, moduleIndex);
        }

        // A SCRIPT'S GLOBAL DECLARATIONS ARE CHECKED BEFORE ITS FIRST INSTRUCTION (JSeal V15-host):
        // every conflict and definability check of `GlobalDeclarationInstantiation` runs here, over
        // the row the artifact carries for the body, so a script that fails one creates nothing.
        if (program.ScriptDeclarations is { } scripts &&
            scripts.TryGetValue((int)unit, out var declared))
        {
            InstantiateGlobalDeclarations(declared);
        }

        var code = program.Functions[(int)unit];
        var environment = new JsEnvironment((int)code.ScopeSlots, null);

        return Execute(
            program,
            (int)unit,
            environment,
            JsValue.Object(Realm.GlobalObject),
            Array.Empty<JsValue>(),
            null,
            JsValue.Undefined,
            null,
            null,
            referrer ??
                (program.ScriptReferrers is { } placed && placed.TryGetValue((int)unit, out var own)
                    ? own
                    : string.Empty));
    }

    // ---- modules -------------------------------------------------------------------------------

    /// <summary>
    /// The referrer of the running execution context: what <c>GetActiveScriptOrModule()</c> answers,
    /// as the string a host resolves against (JSeal I12-upstream, JSD-0024 section 20).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every frame sets it on entry and restores it on exit</b> (<see cref="Execute"/>): a
    /// function's frame to the function's <c>[[ScriptOrModule]]</c>, a script body's to the referrer
    /// its artifact placed it at, a module's initialiser and body to the module's key, and eval code
    /// and a <c>Function</c> body to the referrer of the code that evaluated them. A built-in -
    /// <c>eval</c>, the <c>Function</c> constructor, a host function - enters no frame, so while one
    /// runs this is still its caller's, which is the specification's "topmost execution context
    /// whose ScriptOrModule is not null". A job runs with the referrer that was active when it was
    /// enqueued (<see cref="CallJob"/>, as HostEnqueuePromiseJob requires). With no guest frame and
    /// no job on the stack at all it is empty, which is the language's null referrer.
    /// </para>
    /// <para>
    /// <b>What reads it</b>: an <c>import()</c> whose code carries no referrer of its own (eval code,
    /// a <c>Function</c> body), a function being created (its <c>[[ScriptOrModule]]</c>), and eval
    /// code or a <c>Function</c> body being entered (their frame's referrer).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6825FB
    // Broiler-Human:        PENDING
    private string activeReferrer = string.Empty;

    /// <summary>
    /// The module instances of this realm, one array per artifact, created when its first module is
    /// entered.
    /// </summary>
    /// <remarks>
    /// <b>One realm may hold the modules of several artifacts, and a dynamic import is why.</b> A
    /// specifier the artifact carries no module for is put to the mediator, which answers with a
    /// SEPARATE verified artifact carrying its own records — so the instances of a realm are no
    /// longer indexable by a single array, and an import read has to reach the instances of the
    /// artifact the reading code unit belongs to. Keying on the program is what makes an instance
    /// index mean the same thing in both.
    /// <para>
    /// The key is the verified program's IDENTITY, which is what the default comparer gives here: a
    /// program is a reference type that defines no equality of its own, and two artifacts with
    /// identical bytes are still two artifacts, verified separately and holding separate records.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=83514C
    // Broiler-Human:        PENDING
    private readonly Dictionary<JsProgram, JsModuleInstance[]> graphs = [];

    /// <summary>
    /// Every module this realm has instantiated, by the key its artifact resolved it to.
    /// </summary>
    /// <remarks>
    /// <b>THIS IS WHAT MAKES A MODULE ONE MODULE, and the language is emphatic about it.</b>
    /// <c>import('./m')</c> twice answers the same namespace object, and so does
    /// <c>import('./m')</c> in a program that also wrote <c>import x from './m'</c> — one module
    /// record, one environment, one evaluation. Without a registry across artifacts, a specifier
    /// answered by the mediator would have built a second instance of a module the realm already
    /// had, with its own slots, and a program could then observe two values of one exported
    /// <c>let</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=832748
    // Broiler-Falsified-If: two instances of one module key exist in one realm
    // Broiler-Human:        PENDING
    private readonly Dictionary<string, JsModuleInstance> instanced = new(StringComparer.Ordinal);

    /// <summary>
    /// The realm's template registry: one strings object per tagged-template site, by program and
    /// instruction offset.
    /// </summary>
    /// <remarks>
    /// <b>Weak in the program</b>, so the sites of an evaluated program that nothing can run again
    /// do not keep their strings objects alive; a guest that still holds one keeps its own.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=52E2BF
    // Broiler-Human:        PENDING
    private readonly ConditionalWeakTable<JsProgram, Dictionary<int, JsObject>> templates = [];

    /// <summary>
    /// The realm's <c>IncrementModuleAsyncEvaluationCount</c>: the last
    /// <see cref="JsModuleInstance.AsyncEvaluationOrder"/> handed out.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3305A0
    // Broiler-Human:        PENDING
    private long moduleAsyncEvaluationCount;

    /// <summary>
    /// How many module evaluations are under way on the native stack: an <c>Evaluate</c> walk, or
    /// the bodies one async completion releases.
    /// </summary>
    /// <remarks>
    /// <b>The specification never lets two evaluations overlap</b> (<c>Evaluate</c> step 1), and a
    /// body that reaches back into the host, or asks for an evaluation by some other route while
    /// one is running, would make them overlap here; such a request is deferred to a job while this
    /// is not zero, which is where the language's own route puts it (JSeal I11-async).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=42921A
    // Broiler-Human:        PENDING
    private int moduleEvaluationDepth;

    /// <summary>The entry point a module artifact names its root module by.</summary>
    /// <remarks>
    /// It is a constant of the FORMAT rather than of the compiler that happens to produce one: a
    /// mediator's answer is bytes this profile did not write, and the name it is read back by has
    /// to be a rule both sides can be held to.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F84B52
    // Broiler-Human:        PENDING
    private const string ModuleEntryName = "module";

    /// <summary>The running calls of sloppy plain functions, innermost last.</summary>
    /// <remarks>
    /// <para>
    /// <b>Only sloppy plain functions are recorded, and the depth says what is between them.</b>
    /// Every call - of a built-in, a strict function, a generator's resumption - takes one count of
    /// the depth, and direct eval code takes none. So the call that made a recorded one is the
    /// recorded one directly beneath it exactly when that one runs one count shallower; anything
    /// else between them - a built-in such as <c>Array.prototype.map</c> or <c>Reflect.apply</c>, a
    /// strict function, a generator, an async function - leaves a gap, and the answer is
    /// <c>null</c>, which is what the forbidden-extension rule and every engine's reading ask for.
    /// Eval code is looked through because it takes no count, which is also what they ask.
    /// </para>
    /// <para>
    /// <b>It costs a sloppy plain call one append and one removal</b>, and nothing else pays.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=0A03E0
    // Broiler-Falsified-If: an entry stays after its call has ended, or the order of the entries is not the order of the calls
    // Broiler-Human:        PENDING
    private readonly List<LegacyFrame> legacyFrames = [];

    /// <summary>A tail call a frame has handed to the loop that entered it, or none.</summary>
    /// <remarks>
    /// It is set by the frame's last act and read by <see cref="Invoke"/> before anything else runs,
    /// so no guest code can observe or overwrite it in between.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4339E9
    // Broiler-Falsified-If: a tail call is left pending when the frame that made it has been answered for, or is made by any loop but the one that entered that frame
    // Broiler-Human:        PENDING
    private (JsScriptFunction? Callee, JsValue Receiver, JsValue[] Arguments) tail;

    /// <summary>The delegation is at its own yield, and a resumption is a request to forward.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6D9F45
    // Broiler-Human:        PENDING
    private const int DelegateStepFresh = 0;

    /// <summary>Awaiting what the inner <c>next</c> or <c>throw</c> answered.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6DFAAB
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingStep = 1;

    /// <summary>Awaiting what the inner <c>return</c> answered.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EBD61E
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingReturn = 2;

    /// <summary>Awaiting the close an inner iterator with no <c>throw</c> is given first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D1C9B
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingClose = 3;

    /// <summary>Awaiting the resumption's own value, when the inner iterator has no <c>return</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D5FDD
    // Broiler-Human:        PENDING
    private const int DelegateAwaitingReceived = 4;

    /// <summary>Renders a thrown value for a host that has to describe it in one line.</summary>
    /// <remarks>
    /// <b>A Symbol renders as its descriptive string and never through <c>ToString</c></b>, which
    /// throws for one. Every re-throw - an awaited rejection, a generator resumed with
    /// <c>throw</c> - renders the value it carries, so a rendering that threw replaced the guest's
    /// value with a <c>TypeError</c> of its own: <c>await Promise.reject(Symbol())</c> was caught as
    /// "Cannot convert a Symbol value to a string" until 2026-10-04 (JSC-255). An object's
    /// <c>name</c> and <c>message</c> are still read here, getters included, which the language does
    /// not do on a throw and which that record names as not repaired.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3D177E
    // Broiler-Human:        PENDING
    internal string Render(JsValue value)
    {
        if (!value.IsObject)
        {
            return RenderPart(value);
        }

        var name = GetProperty(value, "name");
        var message = GetProperty(value, "message");

        if (!name.IsNullish || !message.IsNullish)
        {
            var head = name.IsNullish ? "Error" : RenderPart(name);
            var tail = message.IsNullish ? string.Empty : RenderPart(message);
            return tail.Length == 0 ? head : head + ": " + tail;
        }

        return ToStringValue(value);

        string RenderPart(JsValue part) =>
            part.IsString
                ? part.AsString()
                : part.Type == JsType.Symbol ? part.AsSymbol().Rendered : ToStringValue(part);
    }
}

/// <summary>How much of the dispatch loop one call of <see cref="JsEngine.ExecuteCore"/> runs.</summary>
/// <remarks>
/// <para>
/// <b>A MODE IS A VALUE TYPE SO THAT EACH ONE GETS ITS OWN COMPILED LOOP.</b> The runtime compiles a
/// generic method separately for every value-type argument, and a comparison of the argument's type
/// token against a mode's is a constant in that compilation - so the interpreter's instantiation
/// carries no trace of the native form's branches, and no inlining decision is needed to remove them.
/// </para>
/// <para>
/// <b>The opcode is a static member so a per-opcode mode can name its instruction as a constant.</b>
/// A step built for one opcode switches over a constant, which lets the compiler keep only that arm;
/// the modes that read the opcode from the code - the interpreter and the block step - answer
/// <c>default</c> and never have it asked, and the entry runs no instruction to ask it for.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=7F0A56
// Broiler-Falsified-If: a mode's opcode is read by the dispatch loop for a mode that reads its opcode from the code
// Broiler-Human:        PENDING
internal interface IJsExecutionMode
{
    /// <summary>The instruction a per-opcode step runs; <c>default</c> for every other mode.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=A70DF0
    // Broiler-Falsified-If: a per-opcode step answers an opcode other than the one its handler was installed for
    // Broiler-Human:        PENDING
    static abstract JsOpcode Opcode { get; }
}

/// <summary>The interpreter: the whole loop, from entry to return or suspension.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=7881B9
// Broiler-Falsified-If: the loop instantiated over this mode stops before a return, a suspension or an escaping exception
// Broiler-Human:        PENDING
internal readonly struct JsInterpreted : IJsExecutionMode
{
    /// <summary>Never asked: the interpreter reads each opcode from the code.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AA45B6
    // Broiler-Human:        PENDING
    public static JsOpcode Opcode => default;
}

/// <summary>The baseline form's entry: the prologue, an abrupt resumption's raise and landing, and nothing more.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=9020C2
// Broiler-Falsified-If: the loop instantiated over this mode charges for or runs an instruction
// Broiler-Human:        PENDING
internal readonly struct JsNativeEntry : IJsExecutionMode
{
    /// <summary>Never asked: the entry runs no instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AA45B6
    // Broiler-Human:        PENDING
    public static JsOpcode Opcode => default;
}

/// <summary>A block of instructions read from the code: from a head until the baseline partition stops the step.</summary>
/// <remarks>
/// <para>
/// <b>IT RUNS THE INTERPRETER'S OWN LOOP AND ONLY MOVES THE BOUNDARY.</b> Every instruction is charged,
/// dispatched, filtered and landed exactly as the interpreted instantiation does it; the step hands back
/// to emitted code where <see cref="JsBaselineBlocks.StopsAfter"/> holds, which is the same function the
/// emitter's tails are laid out from. Entered at an instruction that runs alone it runs exactly that
/// instruction, since a block ends after one, and that is what lets a run-alone handler run here when
/// <see cref="JsBaselineHandlers.PerOpcodeSteps"/> is false.
/// </para>
/// <para>
/// <b>Its frame holds every arm</b>, so a guest call nested under an instruction it runs sits under a frame
/// the size of the interpreter's, not a per-opcode one. The instructions that nest a call on their common
/// path run alone, so a call one of them makes sits under this frame only while per-opcode steps are off;
/// a call an accessor, a Proxy trap or a coercion makes from inside a block sits under it always.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=0FD013
// Broiler-Falsified-If: the loop instantiated over this mode stops at its first boundary, runs an instruction after a later boundary at which JsBaselineBlocks.StopsAfter holds, or stops at a later boundary at which it does not
// Broiler-Human:        PENDING
internal readonly struct JsStepBlock : IJsExecutionMode
{
    /// <summary>Never asked: a block step reads each opcode from the code.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AA45B6
    // Broiler-Human:        PENDING
    public static JsOpcode Opcode => default;
}
