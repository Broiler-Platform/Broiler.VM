// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   198
// Annotated:        198/198
// Exempt:           32
// Human-reviewed:   0/198
// IP risk:          Low
// Security risk:    High
// Criteria:         73/73
// Resource impact:  7/10 max
// Unverified:       198
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The wide-surface engine: one realm, the abstract operations over it, and the dispatch loop.
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

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F04339
    // Broiler-Human:        PENDING
    private readonly System.Threading.CancellationToken cancellation;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=60EEF1
    // Broiler-Human:        PENDING
    private readonly System.Collections.Immutable.ImmutableArray<string> surfaces;

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

    /// <summary>Creates an engine over a fresh realm.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=67C431
    // Broiler-Human:        PENDING
    internal JsEngine(
        IVmMeter contractMeter,
        System.Threading.CancellationToken token,
        IVmHostCapabilityInvoker? invoker = null,
        System.Collections.Immutable.ImmutableArray<string> admittedSurfaces = default,
        bool nativeForm = false)
    {
        meter = contractMeter;
        cancellation = token;
        capabilities = invoker;
        this.nativeForm = nativeForm;

        // THE SURFACE SET IS ASSIGNED BEFORE THE REALM IS BUILT AND NOT AFTER, because the realm's
        // constructor is what decides which intrinsics exist. A realm handed the set afterwards
        // would have to be able to grow a global, and a realm that can grow one is a realm whose
        // contents depend on when you looked.
        surfaces = admittedSurfaces.IsDefault
            ? System.Collections.Immutable.ImmutableArray<string>.Empty
            : admittedSurfaces;

        Realm = new JsRealm(this);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=703251
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Queue<(JsValue Callable, JsValue[] Arguments, string Referrer)> jobs = new();

    /// <summary>Adds one job to the queue.</summary>
    /// <remarks>
    /// <para>
    /// <b>Enqueueing is charged.</b> A program that enqueues without bound is a program that has
    /// bought unbounded future work with a bounded present, and the charge is what makes the queue
    /// a thing the allowance covers rather than a hole beside it.
    /// </para>
    /// <para>
    /// <b>The job keeps the script or module that was active when it was enqueued</b> (JSD-0024
    /// section 20.3): HostEnqueuePromiseJob requires it to be the active one again when the job
    /// runs, so eval code or a <c>Function</c> a job's built-in callable evaluates resolves its
    /// <c>import()</c> against the code that queued the job, not against nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A1A336
    // Broiler-Human:        PENDING
    internal void EnqueueJob(JsValue callable, JsValue[] arguments)
    {
        Charge(1);
        Retain(64);
        jobs.Enqueue((callable, arguments, activeReferrer));
    }

    /// <summary>Whether any job is waiting.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2DE9B8
    // Broiler-Human:        PENDING
    internal bool HasPendingJobs => jobs.Count != 0;

    /// <summary>How many jobs are due and have not run.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5235ED
    // Broiler-Human:        PENDING
    internal int PendingJobCount => jobs.Count;

    /// <summary>
    /// Runs every job that is due, and every job those enqueue, until none is left.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The host decides when this happens and this profile never decides for it.</b> An
    /// embedding that runs one script and stops, one that runs several in one realm, and one that
    /// interleaves them with its own work all want different drain points, and a queue drained
    /// implicitly at a point nobody stated is a behaviour no embedder can reason about. The host
    /// asks by invoking the reserved entry point named in <see cref="JsExecution"/>.
    /// </para>
    /// <para>
    /// <b>A job that never settles is a resource exhaustion and not a hang.</b> A job may enqueue
    /// another job, so this loop is not bounded by the queue's length at entry; what bounds it is
    /// the allowance, charged per job here and per instruction inside each one. A program whose
    /// jobs enqueue jobs for ever spends its fuel and the operation ends naming <c>Fuel</c> —
    /// which is the same answer a program that loops for ever gets, and deliberately so.
    /// </para>
    /// <para>
    /// <b>A job that throws does not stop the drain.</b> The language says an unhandled rejection
    /// is the host's business and that the queue continues; the thrown value is carried out to the
    /// host through the return value rather than being swallowed, and the remaining jobs still run.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=4B5EB1
    // Broiler-Falsified-If: a job runs at a point the host did not ask for, or an endless queue is a hang rather than an exhaustion
    // Broiler-Human:        PENDING
    internal JsValue DrainJobs()
    {
        var first = JsValue.Undefined;
        var faulted = false;

        while (jobs.Count != 0)
        {
            Charge(1);
            var (callable, arguments, referrer) = jobs.Dequeue();

            try
            {
                _ = CallJob(callable, arguments, referrer);
            }
            catch (JsThrow thrown)
            {
                if (!faulted)
                {
                    faulted = true;
                    first = thrown.Value;
                }
            }
        }

        if (faulted)
        {
            throw new JsThrow(first, Render(first));
        }

        return JsValue.Undefined;
    }

    /// <summary>
    /// Runs at most one due job and says whether the queue still holds anything.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the same queue and the same jobs as <see cref="DrainJobs"/>; what differs is who
    /// decides when the next one runs.</b> A drain runs the queue to exhaustion inside one
    /// operation, which is what a host that wants a script settled asks for. A host that wants an
    /// event loop of its own — one that can interleave its work with the guest's, or stop between
    /// turns and never resume — needs the turn to be the unit, and a queue drained to exhaustion
    /// cannot give it one.
    /// </para>
    /// <para>
    /// <b>A job that throws does not stop the stepping</b>, exactly as it does not stop a drain: the
    /// value is carried out and the queue keeps its remaining jobs, so a host stepping through a
    /// program sees the same sequence of faults, one at a time, that a drain would have folded into
    /// the first.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=FB280D
    // Broiler-Falsified-If: more than one job runs in a step, or a step reports a queue state the queue does not have
    // Broiler-Human:        PENDING
    internal bool StepOneJob(out JsValue thrown)
    {
        thrown = JsValue.Undefined;

        if (jobs.Count == 0)
        {
            return false;
        }

        Charge(1);
        var (callable, arguments, referrer) = jobs.Dequeue();

        try
        {
            _ = CallJob(callable, arguments, referrer);
        }
        catch (JsThrow raised)
        {
            thrown = raised.Value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calls one dequeued job with the script or module that queued it as the active one.
    /// </summary>
    /// <remarks>
    /// A job whose callable is a guest function enters a frame that sets its own referrer anyway; a
    /// built-in one - <c>eval</c>, the <c>Function</c> constructor, a host function - enters none,
    /// and this is what it then sees. The previous referrer is restored however the job ends.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B91E1A
    // Broiler-Human:        PENDING
    private JsValue CallJob(JsValue callable, JsValue[] arguments, string referrer)
    {
        var outerReferrer = activeReferrer;
        activeReferrer = referrer;

        try
        {
            return Call(callable, JsValue.Undefined, arguments);
        }
        finally
        {
            activeReferrer = outerReferrer;
        }
    }

    /// <summary>
    /// Drops every queued job without running any of it.
    /// </summary>
    /// <remarks>
    /// The terminal unwind's whole of the work, and it deliberately runs <b>no guest code</b>:
    /// roadmap section 12 says the unwind must run nothing able to request a load or to suspend,
    /// and the only way to promise that about a queue of guest callables is not to call them.
    /// Whatever they would have done is not done, which is what abandoning an operation means.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=459593
    // Broiler-Falsified-If: a queued job runs during an unwind
    // Broiler-Human:        PENDING
    internal int DropPendingJobs()
    {
        var dropped = jobs.Count;
        jobs.Clear();
        return dropped;
    }

    /// <summary>Whether the composition admitted the optional surface <paramref name="manifestId"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=486D64
    // Broiler-Human:        PENDING
    internal bool Admits(string manifestId) => surfaces.Contains(manifestId);

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
    /// Evaluates a String as a program, through the one route a guest may obtain code by.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing here compiles anything.</b> The source becomes the opaque request payload of a
    /// guest-initiated load, the composition's registered artifact provider answers it with bytes,
    /// and the core verifies those bytes into their own immutable handle before a byte of them
    /// runs — under the requesting operation's remaining allowance, at a nesting depth the core
    /// counts. That is roadmap section 11's design and it is what keeps a compiler inside a
    /// composition's declared closure instead of inside this profile.
    /// </para>
    /// <para>
    /// <b>Two refusals a reader will meet and must not confuse.</b> A composition that DECLINES
    /// <c>broiler.javascript.dynamic</c> never gets here at all: its artifacts naming the surface
    /// were refused at verification, as an invalid artifact the guest never sees. A composition
    /// that admits the surface and registers NO provider gets here and is refused at run time, as
    /// an error the guest may catch. Section 6 draws exactly that distinction and this method is
    /// where the second half of it happens.
    /// </para>
    /// <para>
    /// <b>The direct form reaches this method only where it means the same thing as the indirect
    /// one.</b> A direct <c>eval</c> evaluates in the CALLER's scope, and the artifact this method
    /// asks for is compiled without any knowledge of the frame that asked for it, so its free names
    /// reach the global object. At the top level of a script with nothing between the call and the
    /// body's entry record that is exactly right, because the caller's scope IS the global scope. A
    /// site whose caller's artifact carries a scope-map row is evaluated against the caller's own
    /// records by <see cref="EvaluateAtSite"/> instead (JSeal V14, JSD-0026); a direct call that
    /// arrives here from anywhere else - a function unit whose site has no row - is refused by name
    /// rather than answered with a global's value. That refusal is the published exclusion, not a
    /// defect to be discovered later.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=F0977B
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator
    // Broiler-Human:        PENDING
    internal JsValue Evaluate(JsValue[] arguments, bool direct, Format.JsFormat.FunctionFlags callerFlags)
    {
        var source = arguments.Length == 0 ? JsValue.Undefined : arguments[0];

        // `eval` of anything that is not a String answers it unchanged. It is the one coercion the
        // language deliberately does not do, so that `eval(someObject)` is a value and not a
        // program.
        if (!source.IsString)
        {
            return source;
        }

        if (direct && (callerFlags & Format.JsFormat.FunctionFlags.ProgramBody) == 0)
        {
            throw Error(
                "EvalError",
                "a direct eval at this call site is not admitted: its artifact describes no scope " +
                "for it - code whose scope reaches a module, a function body whose parameter list " +
                "both calls eval and creates closures, or an artifact written without an eval " +
                "scope map - so evaluated source cannot see the " +
                "calling frame's bindings. An indirect eval - (0, eval)(source) - evaluates in " +
                "the global scope and is admitted");
        }

        if (Loader is null)
        {
            throw Error(
                "EvalError",
                "this composition registered no artifact provider, so no source can become code");
        }

        var text = source.AsString();
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);

        // Proportional to the source, because a guest that could buy an unbounded compilation with
        // one instruction would have found the hole every budget dimension exists to close.
        Charge(1 + (ulong)bytes.Length);

        // A SOURCE THAT BEGINS WITH A CONTROL CHARACTER BEGINS NO PROGRAM, and it is answered here
        // rather than sent. U+0000 to U+0008 and U+000E to U+001F are neither white space nor a line
        // terminator and begin no token, so every front end refuses such a source - and the first two
        // of them are the marks a module request and an eval request begin with, so sending one would
        // ask the provider a different question than the guest did (JSeal V14). The answer is the
        // one the provider's refusal would have become.
        if (text.Length != 0 && (text[0] <= '\u0008' || text[0] is >= '\u000E' and <= '\u001F'))
        {
            return ThrowSyntaxError("the evaluated source is not a program this profile admits");
        }

        // The request carries the profile's identity, a nesting depth of one, and the source. The
        // core fills in and enforces everything else - the operation the work is charged to, the
        // remaining allowance, the real depth - which is why a profile may state a nesting depth
        // here without that being a way to relax one.
        var request = new VmArtifactRequest(
            JavaScriptProfile.Id,
            default,
            default,
            1,
            default,
            cancellation,
            new VmBytes(bytes));

        var loaded = Loader.RequestLoad(in request);

        // A PROVIDER REFUSAL IS A `SyntaxError` AND NOT AN `EvalError`, and the two are different
        // answers to different questions. `ProviderRefused` is what the mediator reports when the
        // provider it asked answered "this is not a program I will supply" - which, for the only
        // providers this profile's compositions register, means the front end refused the SOURCE.
        // The language says `eval` of source that is not a program throws a `SyntaxError`, and
        // programs test for it: a conformance case that asserts `assert.throws(SyntaxError, ...)`
        // over an evaluated string is checking the language and not this host's plumbing, and an
        // `EvalError` there failed a case whose subject this host answers correctly. Every OTHER
        // way a load can fail - no provider registered, a budget exhausted, the mediator out of
        // scope, a foreign artifact - is this host's own plumbing and keeps the `EvalError` it had.
        if (loaded.Reason == VmReason.ProviderRefused)
        {
            return ThrowSyntaxError("the evaluated source is not a program this profile admits");
        }

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            throw Error(
                "EvalError",
                "the artifact provider did not supply a program: " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram evaluated)
        {
            throw Error("EvalError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(evaluated);

        if (!evaluated.TryFindEntry("main", out var unit))
        {
            throw Error("EvalError", "the evaluated program declares no entry point");
        }

        // IT RUNS IN THIS REALM AND NOT IN A NEW ONE. The handle is a separate verified artifact -
        // its own constants, its own code, its own function table - but the global object it
        // reaches is this engine's, which is what makes `eval("var f = function () {}")` define
        // something the calling program can afterwards call. AND IT RUNS AS ITS CALLER'S SCRIPT OR
        // MODULE: this is dynamic code, whatever referrer a provider compiled it with (JSD-0024
        // section 20).
        return RunEntry(evaluated, unit, activeReferrer);
    }

    /// <summary>
    /// Performs a direct <c>eval</c> from one call site, or refuses it by name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A direct evaluation without a site row is admitted only where it means what a global
    /// evaluation means</b>, and three places that used to be admitted are not (JSD-0026 step 1). A
    /// module's top level has bindings of its own - its declarations are slots and its imports are
    /// indirections - so evaluating there globally answered a <c>ReferenceError</c> for a name the
    /// module declares; since JSeal V15-module the lowering writes a row for every module site, so
    /// only an artifact without one meets that refusal. A script's body with a block, a <c>for</c>-<c>let</c> head, a
    /// <c>switch</c>, a <c>catch</c> or a <c>with</c> record around the call site has names the
    /// global scope does not, and the frame's current record is not the one the body was entered
    /// with exactly when one of those lies between: a block that declares nothing pushes no
    /// record and binds nothing a global evaluation could miss. Each of these used to run the
    /// source against the global scope and answer with the wrong binding's value.
    /// </para>
    /// <para>
    /// <b>A value that is not a String is answered unchanged before anything is refused</b>,
    /// because that is what <c>eval</c> does with one wherever it is called from: there is no
    /// program to evaluate, so there is nothing about the calling scope to get wrong.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=C613DA
    // Broiler-Falsified-If: a direct eval with no site row at a module's top level, or in a script body whose current record is not its entry record, evaluates the source instead of throwing an EvalError
    // Broiler-Human:        PENDING
    private JsValue EvaluateDirect(
        JsProgram program,
        int unitIndex,
        int site,
        System.Collections.Generic.List<JsEnvironment> scopes,
        JsValue[] arguments,
        JsValue thisValue,
        JsCell? thisBinding,
        JsValue newTarget,
        JsScriptFunction? active)
    {
        if (arguments.Length == 0 || !arguments[0].IsString)
        {
            return arguments.Length == 0 ? JsValue.Undefined : arguments[0];
        }

        // A SITE THE CALLER'S MAP DESCRIBES IS EVALUATED AGAINST THE CALLER'S RECORDS, and every
        // other site keeps the answer it had: the global path at a script body's top level, and the
        // explicit refusal everywhere else (JSeal V14, JSD-0026 step 5).
        if (program.EvalMap is { } map && map.TryFindSite((uint)site, out var described))
        {
            return EvaluateAtSite(
                program, described, scopes[^1], arguments[0].AsString(),
                thisValue, thisBinding, newTarget, active);
        }

        var unit = program.Functions[unitIndex];

        if ((unit.Flags & Format.JsFormat.FunctionFlags.ProgramBody) != 0)
        {
            if (program.ModuleOfUnit[unitIndex] >= 0)
            {
                throw Error(
                    "EvalError",
                    "a direct eval at a module's top level is not admitted: the module's own " +
                    "bindings and imports are not visible to evaluated source. An indirect " +
                    "eval - (0, eval)(source) - evaluates in the global scope and is admitted");
            }

            if (scopes.Count != 1)
            {
                throw Error(
                    "EvalError",
                    "a direct eval inside a block, loop head, switch, catch clause or with " +
                    "statement that has bindings of its own is not admitted: evaluated source " +
                    "cannot see them. An indirect eval - (0, eval)(source) - evaluates in the " +
                    "global scope and is admitted");
            }

            // A SCRIPT'S OWN TOP LEVEL WITH NOTHING BETWEEN THE CALL AND THE BODY'S ENTRY RECORD is
            // the global scope and nothing else, so the evaluation is global eval code: its lexical
            // declarations stay its own, it inherits the script's strictness, and its `var`s and
            // functions become configurable globals after the global checks (JSeal V15, JSD-0026
            // step 8). It answers with the caller's `this`, which at a script's top level is the
            // global one.
            return EvaluateGlobal(
                arguments[0].AsString(),
                unit.IsStrict ? Format.JsFormat.EvalRequestFlags.Strict : Format.JsFormat.EvalRequestFlags.None,
                thisValue);
        }

        return Evaluate(arguments, direct: true, unit.Flags);
    }

    /// <summary>
    /// Evaluates a String as global eval code: an indirect <c>eval</c>, or a direct one at a script's
    /// top level with no record around the call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is eval code and not a script</b> (JSeal V15, JSD-0026 step 8), which is four differences
    /// a script lowering could not give: the program's <c>let</c>, <c>const</c> and <c>class</c>
    /// declarations are its own and never the realm's lexical half; a strict program's <c>var</c>s
    /// and functions are its own too; a sloppy program's become properties of the global object that
    /// are configurable, so <c>delete</c> removes them; and before any is created the global checks run
    /// - a <c>var</c> colliding with a global lexical declaration is a <c>SyntaxError</c> and a
    /// function the global object cannot take is a <c>TypeError</c>, with nothing created either way.
    /// </para>
    /// <para>
    /// <b>The route is the direct one's</b>: the source and the request flags - strict when a strict
    /// script asked directly, never for an indirect call - go to the one provider as an eval request,
    /// the answer is bound to it, and the boundary record it is entered with has no parent and a view
    /// of the global scope alone.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=C997CA
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator, or a global evaluation's lexical declaration becomes a binding of the realm
    // Broiler-Human:        PENDING
    internal JsValue EvaluateGlobal(string text, Format.JsFormat.EvalRequestFlags flags, JsValue thisValue)
    {
        var (evaluated, unit, declaration) = LoadEvalCode(flags, text);
        var view = JsEvalView.Global();

        // NO CLASS ENCLOSES A GLOBAL EVALUATION, so a private name it uses and does not declare is
        // the SyntaxError `AllPrivateIdentifiersValid` makes it (JSeal V15-finish).
        if (declaration.PrivateNames.Length != 0)
        {
            ThrowSyntaxError(
                "the evaluated source names the private name " + declaration.PrivateNames[0][1..] +
                ", which no class encloses");
        }

        if (declaration.Introduces)
        {
            InstantiateEvalDeclarations(view, null, declaration);
        }

        var code = evaluated.Functions[(int)unit];
        var boundary = new JsEnvironment((int)code.ScopeSlots, null, view);

        // EVAL CODE RUNS AS ITS CALLER'S SCRIPT OR MODULE (PerformEval, JSD-0024 section 20).
        return Execute(
            evaluated,
            (int)unit,
            boundary,
            thisValue,
            System.Array.Empty<JsValue>(),
            null,
            JsValue.Undefined,
            null,
            null,
            activeReferrer);
    }

    /// <summary>
    /// Evaluates a String as a SCRIPT an embedder is running in this realm, and answers its completion
    /// value (JSeal V15-host, JSD-0024 section 15).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a script and not eval code</b>, which is everything the global eval path is not: its
    /// <c>let</c>, <c>const</c> and <c>class</c> declarations are bindings of the realm's global lexical
    /// environment that later scripts see, its <c>var</c>s and functions are non-configurable
    /// properties of the global object, and <c>GlobalDeclarationInstantiation</c>'s checks run before
    /// its first instruction - a lexical declaration colliding with an earlier script's lexical one or
    /// with a non-configurable global property is a <c>SyntaxError</c>, a <c>var</c> or function
    /// colliding with a global lexical one likewise, and a function or <c>var</c> the global object
    /// cannot take a <c>TypeError</c>, with nothing created either way.
    /// </para>
    /// <para>
    /// <b>The route is every guest-initiated load's, with the one mark no guest can write</b>: the
    /// source, its name and the strictness the embedder asked for go to the provider as a script
    /// request, the core verifies the answer under this operation's allowance, and the answer is
    /// bound to the request. Whether a guest may evaluate source is the provider's policy for the
    /// OTHER marks; a guest <c>eval</c> inside the script is still an eval request.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=1248A2
    // Broiler-Falsified-If: a host script's source becomes executable bytes without passing through the mediator, or an answer that is not a script compiled under the requested strictness runs
    // Broiler-Human:        PENDING
    internal JsValue EvaluateScript(string text, string sourceName, bool strict)
    {
        if (Loader is null)
        {
            throw Error(
                "EvalError",
                "this composition registered no artifact provider, so no source can become code");
        }

        var payload = Format.JsFormat.ScriptRequest(
            strict ? Format.JsFormat.ScriptRequestFlags.Strict : Format.JsFormat.ScriptRequestFlags.None,
            sourceName,
            text);

        // Proportional to the request, exactly as every other evaluation is charged.
        Charge(1 + (ulong)payload.Length);

        var request = new VmArtifactRequest(
            JavaScriptProfile.Id,
            default,
            default,
            1,
            default,
            cancellation,
            new VmBytes(payload));

        var loaded = Loader.RequestLoad(in request);

        // A REFUSAL IS THE SCRIPT'S `SyntaxError`, named by the source name the embedder gave: the
        // provider holds the diagnostic and its position, and the reason vocabulary cannot carry it.
        if (loaded.Reason == VmReason.ProviderRefused)
        {
            ThrowSyntaxError(
                (sourceName.Length == 0 ? "the host script" : "the host script " + sourceName) +
                " is not a program this profile admits");
        }

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            throw Error(
                "EvalError",
                "the artifact provider did not supply a program: " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram evaluated)
        {
            throw Error("EvalError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(evaluated);

        if (!evaluated.TryFindEntry("main", out var unit) ||
            (evaluated.Functions[(int)unit].Flags &
                (Format.JsFormat.FunctionFlags.ProgramBody | Format.JsFormat.FunctionFlags.EvalCode)) !=
                Format.JsFormat.FunctionFlags.ProgramBody ||
            evaluated.ModuleOfUnit[(int)unit] >= 0 ||
            (strict && !evaluated.Functions[(int)unit].IsStrict))
        {
            throw Error(
                "EvalError",
                "the artifact provider answered for another goal: a host script is answered by a " +
                "script compiled under the strictness its request asked for");
        }

        // IT RUNS IN THIS REALM AND NOT IN A NEW ONE, through the one entry every script takes.
        return RunEntry(evaluated, unit);
    }

    /// <summary>
    /// Asks the provider for eval code compiled under <paramref name="flags"/>, and binds its answer to
    /// the request or refuses it by name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The route is the one every guest-initiated load takes, with a marked payload</b> (JSD-0026
    /// sections 5 and 8): the source and the request flags go to the one provider the composition
    /// registered, which is asked once per evaluation and may refuse, and the core verifies what it
    /// answers before a byte of it runs, under this operation's allowance and at a depth it counts.
    /// </para>
    /// <para>
    /// <b>The answer is bound to the request.</b> It must be a program whose entry named <c>eval</c> is
    /// eval code compiled under the flags that were asked for, and anything else - a provider that
    /// compiled the payload as a script, or for another site - is refused with an <c>EvalError</c>
    /// naming a provider that answered for another goal. A program an earlier lowering marked as
    /// one this build does not run - the refusal of sloppy declarations JSeal V14 wrote, or of a
    /// reference to the caller's <c>super</c> or private names V14 and V15 wrote - is refused by
    /// name before its first instruction.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=541E1E
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator, or an answer that is not eval code compiled under the requested flags is returned
    // Broiler-Human:        PENDING
    private (JsProgram Program, uint Unit, JsEvalDeclaration Declaration) LoadEvalCode(
        Format.JsFormat.EvalRequestFlags flags, string text)
    {
        if (Loader is null)
        {
            throw Error(
                "EvalError",
                "this composition registered no artifact provider, so no source can become code");
        }

        var payload = Format.JsFormat.EvalRequest(flags, text);

        // Proportional to the source, exactly as every other evaluation is charged.
        Charge(1 + (ulong)(payload.Length - 2));

        var request = new VmArtifactRequest(
            JavaScriptProfile.Id,
            default,
            default,
            1,
            default,
            cancellation,
            new VmBytes(payload));

        var loaded = Loader.RequestLoad(in request);

        if (loaded.Reason == VmReason.ProviderRefused)
        {
            ThrowSyntaxError("the evaluated source is not a program this profile admits");
        }

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            throw Error(
                "EvalError",
                "the artifact provider did not supply a program: " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram evaluated)
        {
            throw Error("EvalError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(evaluated);

        if (!evaluated.TryFindEntry("eval", out var unit) ||
            (evaluated.Functions[(int)unit].Flags & Format.JsFormat.FunctionFlags.EvalCode) == 0 ||
            evaluated.EvalMap is not { } answered ||
            !answered.TryFindDeclaration((int)unit, out var declaration) ||
            declaration.Flags != flags)
        {
            throw Error(
                "EvalError",
                "the artifact provider answered for another goal: an eval is answered by eval " +
                "code compiled under the flags its call asked for");
        }

        switch (declaration.Refusal)
        {
            case Format.JsFormat.EvalRefusal.VarDeclarations:
                throw Error(
                    "EvalError",
                    "the evaluated program carries the refusal of sloppy var and function " +
                    "declarations an earlier build wrote; this build instantiates them itself");

            case Format.JsFormat.EvalRefusal.SuperReference:
                throw Error(
                    "EvalError",
                    "a direct eval whose source refers to the calling method's super is not admitted");

            case Format.JsFormat.EvalRefusal.PrivateName:
                throw Error(
                    "EvalError",
                    "a direct eval whose source names a private name of the calling class is not " +
                    "admitted");
        }

        return (evaluated, unit, declaration);
    }

    /// <summary>
    /// Performs the part of <c>EvalDeclarationInstantiation</c> that reaches outside a sloppy
    /// evaluation: every check first, then the caller's new bindings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing is created until every check has passed</b> (JSeal V15, JSD-0026 step 7). The walk
    /// goes from the site's innermost record to the variable environment through the caller's
    /// verified map, one row and one record at a time: a lexical binding of a declared name in any
    /// record on the way - a block's, a <c>for</c>-<c>let</c> head's, a <c>switch</c>'s, an enclosing
    /// evaluation's own, or a function's top-level <c>let</c>, <c>const</c> or <c>class</c> - is a
    /// <c>SyntaxError</c>. A <c>with</c> record is passed, because it holds no declarations, and so is
    /// a catch clause's parameter, whatever its form: the pinned ES2026 text exempts the record of a
    /// <c>Catch</c> clause (Annex B.3.4, normative-optional and supported here) and says nothing about
    /// the parameter's shape. A declared name the function record already binds as a <c>var</c>, a
    /// parameter, a function or <c>arguments</c> is that binding; everything else becomes a binding
    /// of the function's eval-variables set. At the global environment a declared name that is a
    /// global lexical declaration is a <c>SyntaxError</c> and a function or <c>var</c> the global
    /// object cannot take a <c>TypeError</c>, exactly as <c>CanDeclareGlobalFunction</c> and
    /// <c>CanDeclareGlobalVar</c> answer; what passes becomes a configurable property.
    /// </para>
    /// <para>
    /// <b>An Annex B block function is hoisted per evaluation</b>, when no lexical binding of its name
    /// lies between the call and the variable environment - a catch clause's parameter again
    /// excepted - and, at the global environment, when the name is no global lexical declaration and
    /// the global object could take a <c>var</c> of it. A hoisted name gets a binding holding
    /// <c>undefined</c> unless it is already declared, and only a hoisted name is written when its
    /// declaration is evaluated.
    /// </para>
    /// <para>
    /// <b>The order of creation is the specification's</b>: the Annex B names, then the functions -
    /// whose bindings are made here and written by the program's first instructions, which create the
    /// function objects - then the <c>var</c>s, so a global evaluation that declares both shows the
    /// function's property before the <c>var</c>'s. Each record walked and each name checked is
    /// charged, so no loop here is unmetered.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=053159
    // Broiler-Falsified-If: a binding is created in the caller's scope or on the global object before a later check of the same evaluation throws, or a declared name is made a binding past a lexical binding of the same name that is not a catch parameter
    // Broiler-Human:        PENDING
    private void InstantiateEvalDeclarations(
        JsEvalView view, JsEnvironment? callerRecord, JsEvalDeclaration declaration)
    {
        var names = declaration.VarNames.Length + declaration.FunctionNames.Length;
        var annexB = declaration.AnnexBNames;
        var blocked = annexB.Length == 0 ? null : new bool[annexB.Length];
        JsEnvironment? variableRecord = null;
        JsEvalShape? variableShape = null;

        if (view.Program is { } program)
        {
            var map = program.EvalMap;
            var row = view.Site!.Scope;
            var record = callerRecord;

            while (true)
            {
                Charge(1 + (ulong)names + (ulong)annexB.Length);

                if (record is null || map is null || (uint)row >= (uint)map.Shapes.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval view named no record");
                }

                var shape = map.Shapes[row];

                if (shape.Kind != Format.JsFormat.EvalScopeKind.With)
                {
                    RequireNoLexical(shape, declaration.VarNames);
                    RequireNoLexical(shape, declaration.FunctionNames);

                    for (var at = 0; at < annexB.Length; at++)
                    {
                        if (shape.BindsLexically(annexB[at]))
                        {
                            blocked![at] = true;
                        }
                    }
                }

                if (shape.Kind is Format.JsFormat.EvalScopeKind.Function or
                    Format.JsFormat.EvalScopeKind.FunctionBody)
                {
                    if (record.Binding is not null)
                    {
                        throw new JsAbort(
                            JsAbortKind.InternalDefect, "an eval view's function row named an object record");
                    }


                    variableRecord = record;
                    variableShape = shape;
                    break;
                }

                if (shape.Kind == Format.JsFormat.EvalScopeKind.Program)
                {
                    break;
                }

                if (shape.Kind == Format.JsFormat.EvalScopeKind.Eval)
                {
                    // AN ENCLOSING EVALUATION IS SLOPPY, or this one could not be: its variable
                    // environment is its own caller's, so the walk goes on through the view its
                    // boundary was entered with - to the global one when that evaluation was global.
                    if (record.EvalView is not { } outer)
                    {
                        throw new JsAbort(
                            JsAbortKind.InternalDefect, "an eval view's eval row named no boundary");
                    }

                    if (outer.Program is null)
                    {
                        break;
                    }

                    map = outer.Program.EvalMap;
                    row = outer.Site!.Scope;
                    record = record.Parent;
                    continue;
                }

                row = shape.Parent;
                record = record.Parent;
            }
        }

        var global = variableRecord is null;

        if (global)
        {
            Charge(1 + (ulong)names);
            RequireNoGlobalLexical(declaration.VarNames);
            RequireNoGlobalLexical(declaration.FunctionNames);

            // The specification visits the functions last to first; which one is named first changes
            // only the message of the TypeError, and the order kept here is that one.
            for (var at = declaration.FunctionNames.Length - 1; at >= 0; at--)
            {
                if (!CanDeclareGlobalFunction(declaration.FunctionNames[at]))
                {
                    ThrowTypeError(
                        "the global object cannot take a function named " + declaration.FunctionNames[at]);
                }
            }

            foreach (var name in declaration.VarNames)
            {
                if (!CanDeclareGlobalVar(name))
                {
                    ThrowTypeError("the global object cannot take a var named " + name);
                }
            }
        }

        var declared = new System.Collections.Generic.HashSet<string>(
            declaration.FunctionNames, System.StringComparer.Ordinal);

        for (var at = 0; at < annexB.Length; at++)
        {
            var name = annexB[at];
            Charge(1);

            if (blocked![at])
            {
                continue;
            }

            if (global &&
                ((Realm.HasLexicals && Realm.TryLexical(name, out _)) || !CanDeclareGlobalVar(name)))
            {
                continue;
            }

            if (!declared.Contains(name) &&
                System.Array.IndexOf(declaration.VarNames, name) < 0)
            {
                if (global)
                {
                    CreateGlobalVarBinding(name);
                }
                else
                {
                    IntroduceEvalVariable(variableRecord!, variableShape!, name);
                }
            }

            declared.Add(name);
        }

        foreach (var name in declaration.FunctionNames)
        {
            Charge(1);

            if (global)
            {
                CreateGlobalFunctionBinding(name);
            }
            else
            {
                IntroduceEvalVariable(variableRecord!, variableShape!, name);
            }
        }

        foreach (var name in declaration.VarNames)
        {
            Charge(1);

            if (global)
            {
                CreateGlobalVarBinding(name);
            }
            else
            {
                IntroduceEvalVariable(variableRecord!, variableShape!, name);
            }
        }

        view.VariableRecord = variableRecord;
        view.VariableShape = variableShape;
        view.Declared = declared;
    }

    /// <summary>
    /// Performs the checks of <c>GlobalDeclarationInstantiation</c> for one script body, and the
    /// definition half of its function bindings, before the body's first instruction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every check runs before anything is created</b> (JSeal V15-host), in the pinned ES2026
    /// order: a lexically declared name that is already a global lexical declaration, or a
    /// non-configurable own property of the global object (<c>HasRestrictedGlobalProperty</c>), is a
    /// <c>SyntaxError</c>; a <c>var</c> or function name that is a global lexical declaration is a
    /// <c>SyntaxError</c>; a function the global object cannot take (<c>CanDeclareGlobalFunction</c>,
    /// last to first) and then a <c>var</c> it cannot take (<c>CanDeclareGlobalVar</c>) is a
    /// <c>TypeError</c>. The edition has no <c>[[VarNames]]</c> list, so a <c>let</c> over a
    /// configurable global - an eval-introduced <c>var</c>, say - is admitted.
    /// </para>
    /// <para>
    /// <b>What passes is created by the body's own instructions</b>, which follow here unchanged, with
    /// one step taken first: each function name gets the property <c>CreateGlobalFunctionBinding</c>
    /// defines - writable, enumerable and not configurable, replacing an absent or configurable one
    /// (an accessor included) and leaving a non-configurable one's attributes alone - so the body's
    /// write stores the function into the right property. The global lexical bindings the body then
    /// declares can no longer meet an existing one.
    /// </para>
    /// <para>
    /// <b>An Annex B candidate this lowering cannot skip is refused by name.</b> The specification
    /// hoists a block-level function only when its name is no global lexical declaration and the
    /// global object could take a <c>var</c> of it, and skips it otherwise; the body's instructions
    /// here create the alias and write it unconditionally, which would write an earlier script's
    /// lexical binding or grow a non-extensible global. So such a script is an <c>EvalError</c>
    /// before anything is created, rather than a wrong answer. Every name checked is charged.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=C0B3E2
    // Broiler-Falsified-If: a binding is created on the global object or in the global lexical environment before a later check of the same script throws, or a lexical declaration is admitted over an existing global lexical declaration or a non-configurable global property
    // Broiler-Human:        PENDING
    private void InstantiateGlobalDeclarations(JsScriptDeclaration declaration)
    {
        var global = Realm.GlobalObject;

        Charge(
            1 + (ulong)declaration.LexicalNames.Length + (ulong)declaration.VarNames.Length +
            (ulong)declaration.FunctionNames.Length + (ulong)declaration.AnnexBNames.Length);

        foreach (var name in declaration.LexicalNames)
        {
            if (Realm.HasLexicals && Realm.TryLexical(name, out _))
            {
                ThrowSyntaxError("a script cannot declare " + name + ": it is already a global lexical declaration");
            }

            if (global.TryGetOwnProperty(name, out var existing) && !existing.Configurable)
            {
                ThrowSyntaxError(
                    "a script cannot declare " + name + " lexically: it is a non-configurable property of the global object");
            }
        }

        RequireNoScriptLexical(declaration.VarNames);
        RequireNoScriptLexical(declaration.FunctionNames);

        for (var at = declaration.FunctionNames.Length - 1; at >= 0; at--)
        {
            if (!CanDeclareGlobalFunction(declaration.FunctionNames[at]))
            {
                ThrowTypeError("the global object cannot take a function named " + declaration.FunctionNames[at]);
            }
        }

        foreach (var name in declaration.VarNames)
        {
            if (!CanDeclareGlobalVar(name))
            {
                ThrowTypeError("the global object cannot take a var named " + name);
            }
        }

        foreach (var name in declaration.AnnexBNames)
        {
            if (System.Array.IndexOf(declaration.FunctionNames, name) >= 0 ||
                System.Array.IndexOf(declaration.VarNames, name) >= 0)
            {
                continue;
            }

            if ((Realm.HasLexicals && Realm.TryLexical(name, out _)) || !CanDeclareGlobalVar(name))
            {
                throw Error(
                    "EvalError",
                    "a script whose block-level function " + name + " Annex B would not hoist - a " +
                    "global lexical declaration of that name exists, or the global object cannot " +
                    "take it - is not admitted: this lowering writes the alias unconditionally");
            }
        }

        foreach (var name in declaration.FunctionNames)
        {
            if (!global.TryGetOwnProperty(name, out var existing) || existing.Configurable)
            {
                global.SetOwnProperty(
                    name,
                    JsProperty.Data(
                        JsValue.Undefined, JsPropertyAttributes.Writable | JsPropertyAttributes.Enumerable));
            }
        }
    }

    /// <summary>Throws the conflict <c>SyntaxError</c> when a global lexical declaration has one of a script's <c>var</c>-scoped <paramref name="names"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=395810
    // Broiler-Human:        PENDING
    private void RequireNoScriptLexical(string[] names)
    {
        if (!Realm.HasLexicals)
        {
            return;
        }

        foreach (var name in names)
        {
            if (Realm.TryLexical(name, out _))
            {
                ThrowSyntaxError("a script cannot declare var " + name + ": it is a global lexical declaration");
            }
        }
    }

    /// <summary>Throws the conflict <c>SyntaxError</c> when a row binds one of <paramref name="names"/> lexically.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=6B1830
    // Broiler-Human:        PENDING
    private void RequireNoLexical(JsEvalShape shape, string[] names)
    {
        foreach (var name in names)
        {
            if (shape.BindsLexically(name))
            {
                ThrowSyntaxError(
                    "a direct eval cannot declare var " + name +
                    ": a lexical declaration, or a parameter the evaluation's parameter list binds, of the same " +
                    "name lies between the call and its variable environment");
            }
        }
    }

    /// <summary>Throws the conflict <c>SyntaxError</c> when a global lexical declaration has one of <paramref name="names"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EB55E4
    // Broiler-Human:        PENDING
    private void RequireNoGlobalLexical(string[] names)
    {
        if (!Realm.HasLexicals)
        {
            return;
        }

        foreach (var name in names)
        {
            if (Realm.TryLexical(name, out _))
            {
                ThrowSyntaxError(
                    "an eval cannot declare var " + name + ": it is a global lexical declaration");
            }
        }
    }

    /// <summary>The specification's <c>CanDeclareGlobalFunction</c>, over this realm's global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C0336A
    // Broiler-Human:        PENDING
    private bool CanDeclareGlobalFunction(string name)
    {
        var global = Realm.GlobalObject;

        if (!global.TryGetOwnProperty(name, out var existing))
        {
            return global.Extensible;
        }

        return existing.Configurable || (!existing.IsAccessor && existing.Writable && existing.Enumerable);
    }

    /// <summary>The specification's <c>CanDeclareGlobalVar</c>, over this realm's global object.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F88789
    // Broiler-Human:        PENDING
    private bool CanDeclareGlobalVar(string name) =>
        Realm.GlobalObject.HasOwnProperty(name) || Realm.GlobalObject.Extensible;

    /// <summary>
    /// The specification's <c>CreateGlobalVarBinding</c> with a deletable binding: a configurable
    /// property holding <c>undefined</c>, unless the global object already has one of the name.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7BBBC9
    // Broiler-Human:        PENDING
    private void CreateGlobalVarBinding(string name)
    {
        var global = Realm.GlobalObject;

        if (!global.HasOwnProperty(name) && global.Extensible)
        {
            global.SetOwnProperty(name, JsProperty.Data(JsValue.Undefined, JsPropertyAttributes.Default));
        }
    }

    /// <summary>
    /// The definition half of the specification's <c>CreateGlobalFunctionBinding</c> with a deletable
    /// binding; the program's first instructions write the function object into it.
    /// </summary>
    /// <remarks>
    /// An absent or configurable property is replaced by a writable, enumerable, configurable data
    /// property - an accessor included, which is what "if a binding already exists, it is replaced"
    /// means; a non-configurable one, which <see cref="CanDeclareGlobalFunction"/> admitted only as a
    /// writable, enumerable data property, keeps its attributes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=E28910
    // Broiler-Human:        PENDING
    private void CreateGlobalFunctionBinding(string name)
    {
        var global = Realm.GlobalObject;

        if (!global.TryGetOwnProperty(name, out var existing) || existing.Configurable)
        {
            global.SetOwnProperty(name, JsProperty.Data(JsValue.Undefined, JsPropertyAttributes.Default));
        }
    }

    /// <summary>
    /// Gives a function's variable environment a binding of <paramref name="name"/> holding
    /// <c>undefined</c>, unless it has one: a slot the function declares, or an earlier evaluation's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=01E0CE
    // Broiler-Human:        PENDING
    private void IntroduceEvalVariable(JsEnvironment record, JsEvalShape shape, string name)
    {
        if (shape.Binds(name))
        {
            return;
        }

        var introduced = record.EvalVariables;

        if (introduced is null)
        {
            introduced = new JsEvalVariables();
            Retain(64);
            record.EvalVariables = introduced;
        }

        if (!introduced.HasOwnProperty(name))
        {
            // A BINDING NUMBER THE GUEST CONTROLS is retained like a collection entry, and it is
            // bounded by the source bytes the evaluation already paid for (JSD-0026 section 10).
            Retain(96);
            introduced.SetOwnProperty(name, JsProperty.Data(JsValue.Undefined, JsPropertyAttributes.Default));
        }
    }

    /// <summary>
    /// Writes one name of the variable environment an evaluation's boundary was entered with: the
    /// write <see cref="Format.JsOpcode.StoreEvalVariable"/> performs.
    /// </summary>
    /// <remarks>
    /// A name the evaluation's instantiation did not declare - an Annex B alias the caller's bindings
    /// kept from hoisting - is not written. At a function's environment the write lands in the slot the
    /// function declares or in its eval-variables set, recreating a binding a <c>delete</c> removed, as
    /// <c>SetMutableBinding</c> does for a sloppy caller; at the global environment it is the global
    /// environment's <c>SetMutableBinding</c>: a global lexical binding when one has the name, and a
    /// sloppy <c>Set</c> on the global object otherwise.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=19B50E
    // Broiler-Falsified-If: the write reaches a record other than the evaluation's variable environment, or writes a name its instantiation did not declare
    // Broiler-Human:        PENDING
    private void WriteEvalVariable(JsEnvironment boundary, string name, JsValue value)
    {
        var view = boundary.EvalView!;

        if (view.Declared is not { } declared || !declared.Contains(name))
        {
            return;
        }

        if (view.VariableRecord is { } record)
        {
            if (view.VariableShape!.TryFind(name, out var slot, out _, out _))
            {
                if (slot >= record.Slots.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval scope map named no slot");
                }

                record.Slots[slot] = value;
                return;
            }

            var introduced = record.EvalVariables;

            if (introduced is null)
            {
                introduced = new JsEvalVariables();
                Retain(64);
                record.EvalVariables = introduced;
            }

            if (!introduced.HasOwnProperty(name))
            {
                Retain(96);
            }

            introduced.SetOwnProperty(name, JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
        {
            if (!bound.Initialised)
            {
                ThrowReferenceError("cannot access " + name + " before its declaration");
            }

            if (bound.Mutable)
            {
                bound.Value = value;
            }

            return;
        }

        SetProperty(JsValue.Object(Realm.GlobalObject), name, value, false);
    }

    /// <summary>
    /// Evaluates a String as direct <c>eval</c> code against one site's caller: its records, its
    /// <c>this</c>, its <c>new.target</c> and its strictness.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The route is the one every guest-initiated load takes, with a marked payload</b> (JSD-0026
    /// sections 5 and 8): the source and the site's request flags go to the one provider the
    /// composition registered, which is asked once per evaluation and may refuse, and the core
    /// verifies what it answers before a byte of it runs, under this operation's allowance and at a
    /// depth it counts. Compile permission is therefore the provider's answer here exactly as it is
    /// for an indirect <c>eval</c>; the scope map grants nothing.
    /// </para>
    /// <para>
    /// <b>The answer is bound to the request.</b> It must be a program whose entry named <c>eval</c>
    /// is eval code compiled under the flags the site asked for, and anything else - a provider that
    /// compiled the payload as a script, or for another site - is refused with an <c>EvalError</c>
    /// naming a provider that answered for another goal. A program the provider compiled and this
    /// build does not run - a sloppy <c>var</c> or function declaration, a reference to the caller's
    /// <c>super</c> or to a private name - is refused by name before its first instruction.
    /// </para>
    /// <para>
    /// <b>The boundary record is new for every evaluation and its parent is the caller's current
    /// record</b>, so two activations of one caller, and two evaluations in one activation, never
    /// share the evaluated program's own bindings, while every write the program makes to one of
    /// its caller's is a write to the caller's own slot - which every closure over that record sees.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=9109EA
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator, or an answer that is not eval code compiled under the site's flags runs
    // Broiler-Human:        PENDING
    private JsValue EvaluateAtSite(
        JsProgram caller,
        JsEvalSite site,
        JsEnvironment record,
        string text,
        JsValue thisValue,
        JsCell? thisBinding,
        JsValue newTarget,
        JsScriptFunction? active)
    {
        var (evaluated, unit, declaration) = LoadEvalCode(site.Flags, text);
        var view = new JsEvalView(caller, site);
        var code = evaluated.Functions[(int)unit];
        var boundary = new JsEnvironment((int)code.ScopeSlots, record, view);

        // EVERY PRIVATE NAME THE PROGRAM USES AND DOES NOT DECLARE IS ONE A CLASS AROUND THE CALL
        // DECLARES, or the evaluation is the SyntaxError `AllPrivateIdentifiersValid` makes it -
        // before anything is instantiated (JSeal V15-finish). The names are found through the same
        // verified map the program's own `LoadEvalName` reads them through, by a walk that reads
        // class slots alone - an early error runs no guest code and asks no `with` object.
        foreach (var name in declaration.PrivateNames)
        {
            if (!EvalPrivateNameDeclared(boundary, name))
            {
                ThrowSyntaxError(
                    "the evaluated source names the private name " + name[1..] +
                    ", which no class enclosing the call declares");
            }
        }

        // A SLOPPY EVALUATION'S `var` AND FUNCTION DECLARATIONS ARE ITS CALLER'S, checked and made
        // before its first instruction (JSeal V15, JSD-0026 steps 6-7).
        if (declaration.Introduces)
        {
            InstantiateEvalDeclarations(view, record, declaration);
        }

        // EVAL CODE RUNS AS ITS CALLER'S SCRIPT OR MODULE (PerformEval, JSD-0024 section 20),
        // which is the calling frame's and not necessarily the active function's.
        return Execute(
            evaluated,
            (int)unit,
            boundary,
            thisValue,
            System.Array.Empty<JsValue>(),
            active,
            newTarget,
            thisBinding,
            null,
            activeReferrer);
    }

    /// <summary>The eval boundary record <paramref name="hops"/> records out from the current one.</summary>
    /// <remarks>
    /// An eval name instruction that reaches a record with no view was not emitted by the lowering
    /// that wrote the verified program - the verifier admits these instructions only in eval code -
    /// so the answer is an internal defect and never a lookup somewhere else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=BFC809
    // Broiler-Falsified-If: it answers a record that was not entered as an eval boundary
    // Broiler-Human:        PENDING
    private static JsEnvironment EvalBoundary(
        System.Collections.Generic.List<JsEnvironment> scopes, int hops)
    {
        var boundary = scopes[^1].Ancestor(hops);

        if (boundary?.EvalView is null)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "an eval name instruction reached no eval boundary");
        }

        return boundary;
    }

    /// <summary>Where one of the caller's names lives, as an eval view resolved it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=99F935
    // Broiler-Human:        PENDING
    private readonly struct JsEvalBinding(
        JsEnvironment? record,
        int slot,
        bool immutable,
        JsObject? holder,
        bool functionName = false,
        JsProgram? importer = null)
    {
        /// <summary>The declarative record the name is a slot of, or null.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=240892
        // Broiler-Human:        PENDING
        internal JsEnvironment? Record { get; } = record;

        /// <summary>The slot, when <see cref="Record"/> is set.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=897E94
        // Broiler-Human:        PENDING
        internal int Slot { get; } = slot;

        /// <summary>Whether a write to the slot is a <c>TypeError</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=936BE6
        // Broiler-Human:        PENDING
        internal bool Immutable { get; } = immutable;

        /// <summary>
        /// Whether the slot is a named function expression's own name, which sloppy code's write
        /// leaves alone rather than throwing (VM-FIX-D).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C09ECA
        // Broiler-Human:        PENDING
        internal bool FunctionName { get; } = functionName;

        /// <summary>The <c>with</c> object that has the name, or null.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3A0FD4
        // Broiler-Human:        PENDING
        internal JsObject? Object { get; } = holder;

        /// <summary>
        /// The program whose import table <see cref="Slot"/> indexes, when the name is one of a
        /// module's imports (JSeal V15-module), or null.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3D2B2D
        // Broiler-Human:        PENDING
        internal JsProgram? Importer { get; } = importer;
    }

    /// <summary>
    /// Resolves one of the caller's names outward from an eval boundary, through the view it was
    /// entered with.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The map and the chain are walked in step, one row and one record at a time</b>, starting at
    /// the site's innermost row and the record the evaluation was entered under. A declarative row
    /// answers with its slot of the record, a <c>with</c> row asks its record's object -
    /// <c>HasProperty</c>, then <c>Symbol.unscopables</c>, as <see cref="ResolveName"/> does - and a
    /// root ends the walk: a program row in the realm's global scope, which the caller answers with
    /// a binding of neither kind, and an eval row, after its own names, by continuing through the
    /// view ITS boundary record was entered with. Nothing is cached.
    /// </para>
    /// <para>
    /// <b>A disagreement between the map and the chain is an internal defect</b>: a row the map does
    /// not have, a <c>with</c> row over a record with no object, a slot the record does not have. The
    /// verifier admitted the map against the code that pushes the records, so a disagreement means
    /// one of them was not what the verifier read, and the walk stops rather than reading elsewhere.
    /// </para>
    /// <para>
    /// <b>Each record walked is charged one unit</b>, as <see cref="ResolveName"/> charges, and the
    /// walk is bounded by the chain, which the scope-depth ceiling bounds per unit.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=7216D6
    // Broiler-Falsified-If: a name is answered from a slot the view's site row chain does not name, or from a record past a declarative binding of the same name
    // Broiler-Human:        PENDING
    private JsEvalBinding ResolveEvalName(JsEnvironment boundary, string name)
    {
        var view = boundary.EvalView!;

        // A GLOBAL EVALUATION SEES THE GLOBAL SCOPE AND NOTHING ELSE (JSeal V15).
        if (view.Program is null)
        {
            Charge(1);
            return default;
        }

        var record = boundary.Parent;
        var map = view.Program.EvalMap;
        var row = view.Site!.Scope;

        while (true)
        {
            Charge(1);

            if (record is null || map is null || (uint)row >= (uint)map.Shapes.Length)
            {
                throw new JsAbort(JsAbortKind.InternalDefect, "an eval view named no record");
            }

            var shape = map.Shapes[row];

            if (shape.Kind == Format.JsFormat.EvalScopeKind.With)
            {
                if (record.Binding is not { } holder)
                {
                    throw new JsAbort(
                        JsAbortKind.InternalDefect, "an eval view's with row named a declarative record");
                }

                if (HasProperty(holder, name) && !Unscopable(holder, name))
                {
                    return new JsEvalBinding(null, 0, false, holder);
                }
            }
            else if (shape.TryFind(name, out var slot, out var immutable, out var functionName))
            {
                if (record.Binding is not null || slot >= record.Slots.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval scope map named no slot");
                }

                return new JsEvalBinding(record, slot, immutable, null, functionName);
            }
            else if (shape.TryFindImport(name, out var entry))
            {
                // A MODULE'S IMPORT IS READ THROUGH THE CALLER'S IMPORT TABLE, whose entry the
                // verifier bounded (JSeal V15-module); the record is the module's own and holds
                // no slot for it.
                if (record.Binding is not null)
                {
                    throw new JsAbort(
                        JsAbortKind.InternalDefect, "an eval view's module row named an object record");
                }

                return new JsEvalBinding(null, entry, true, null, false, view.Program);
            }
            else if (shape.Kind is (Format.JsFormat.EvalScopeKind.Function or
                    Format.JsFormat.EvalScopeKind.FunctionBody) &&
                record.EvalVariables is { } introduced &&
                introduced.HasOwnProperty(name))
            {
                // A NAME AN EVALUATION INTRODUCED INTO THE FUNCTION is found after the function's own
                // slots and before anything outside it, which is where its variable environment is
                // (JSeal V15).
                return new JsEvalBinding(null, 0, false, introduced);
            }

            // A MODULE'S RECORD ENDS ITS CHAIN AS A SCRIPT BODY'S DOES, in the realm's global scope
            // (JSeal V15-module).
            if (shape.Kind is Format.JsFormat.EvalScopeKind.Program or Format.JsFormat.EvalScopeKind.Module)
            {
                return default;
            }

            if (shape.Kind == Format.JsFormat.EvalScopeKind.Eval)
            {
                if (record.EvalView is not { } outer)
                {
                    throw new JsAbort(
                        JsAbortKind.InternalDefect, "an eval view's eval row named no boundary");
                }

                if (outer.Program is null)
                {
                    return default;
                }

                view = outer;
                map = outer.Program.EvalMap;
                row = outer.Site!.Scope;
                record = record.Parent;
                continue;
            }

            row = shape.Parent;
            record = record.Parent;
        }
    }

    /// <summary>
    /// Whether a class around an eval boundary declares a private name, as the map's declarative
    /// rows answer it.
    /// </summary>
    /// <remarks>
    /// The walk is <see cref="ResolveEvalName"/>'s, row by row and record by record, but a
    /// <c>with</c> row is passed over without asking its object and a function's evaluation-introduced
    /// names are not consulted: only a class declares a private name, and the check this answers is
    /// an early error, which runs no guest code and never shows guest code the profile's internal
    /// <c>##</c> spelling (JSeal V15-finish).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=01D083
    // Broiler-Falsified-If: it asks a with object or runs guest code, or answers true for a name no declarative row on the site's row chain names
    // Broiler-Human:        PENDING
    private bool EvalPrivateNameDeclared(JsEnvironment boundary, string name)
    {
        var view = boundary.EvalView!;

        if (view.Program is null)
        {
            Charge(1);
            return false;
        }

        var record = boundary.Parent;
        var map = view.Program.EvalMap;
        var row = view.Site!.Scope;

        while (true)
        {
            Charge(1);

            if (record is null || map is null || (uint)row >= (uint)map.Shapes.Length)
            {
                throw new JsAbort(JsAbortKind.InternalDefect, "an eval view named no record");
            }

            var shape = map.Shapes[row];

            if (shape.Kind != Format.JsFormat.EvalScopeKind.With &&
                shape.TryFind(name, out var slot, out _, out _))
            {
                if (record.Binding is not null || slot >= record.Slots.Length)
                {
                    throw new JsAbort(JsAbortKind.InternalDefect, "an eval scope map named no slot");
                }

                return true;
            }

            if (shape.Kind is Format.JsFormat.EvalScopeKind.Program or Format.JsFormat.EvalScopeKind.Module)
            {
                return false;
            }

            if (shape.Kind == Format.JsFormat.EvalScopeKind.Eval)
            {
                if (record.EvalView is not { } outer)
                {
                    throw new JsAbort(
                        JsAbortKind.InternalDefect, "an eval view's eval row named no boundary");
                }

                if (outer.Program is null)
                {
                    return false;
                }

                view = outer;
                map = outer.Program.EvalMap;
                row = outer.Site!.Scope;
                record = record.Parent;
                continue;
            }

            row = shape.Parent;
            record = record.Parent;
        }
    }

    /// <summary>Reads a resolved binding, or the realm's global scope when it resolved to neither kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=940B72
    // Broiler-Human:        PENDING
    private JsValue ReadEvalBinding(JsEvalBinding binding, string name, bool orUndefined)
    {
        if (binding.Record is { } record)
        {
            // A BINDING IN ITS DEAD ZONE THROWS FOR `typeof` TOO, which is the one respect in which
            // `typeof` of a declared name differs from `typeof` of one nobody declared.
            if (record.Slots[binding.Slot].IsEmpty)
            {
                ThrowReferenceError("Cannot access a binding before initialisation");
            }

            return record.Slots[binding.Slot];
        }

        // AN IMPORT IS READ WHERE IT LIVES, every time: a live binding, in its dead zone until the
        // exporting module initialises it (JSeal V15-module).
        if (binding.Importer is { } importer)
        {
            return JsModuleNamespace.Read(importer.ImportBindings[binding.Slot], Graph(importer), this);
        }

        if (binding.Object is { } holder)
        {
            return GetProperty(JsValue.Object(holder), name);
        }

        if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
        {
            return ReadLexical(name, bound);
        }

        if (!HasProperty(Realm.GlobalObject, name))
        {
            return orUndefined ? JsValue.Undefined : ThrowReferenceError(name + " is not defined");
        }

        return GetProperty(JsValue.Object(Realm.GlobalObject), name);
    }

    /// <summary>Reads one of the caller's names from eval code.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6D2647
    // Broiler-Human:        PENDING
    private JsValue ReadEvalName(JsEnvironment boundary, string name, bool orUndefined) =>
        ReadEvalBinding(ResolveEvalName(boundary, name), name, orUndefined);

    /// <summary>Writes one of the caller's names from eval code.</summary>
    /// <remarks>
    /// <b>The rules are the caller's own instructions' rules</b>: a slot in its dead zone is a
    /// <c>ReferenceError</c> and an immutable one a <c>TypeError</c>, as <c>StoreScoped</c> and
    /// <c>ThrowImmutable</c> answer; a <c>with</c> object's property is set on the object; and a name
    /// nothing binds is <c>StoreGlobal</c>'s question, asked with the evaluated program's strictness
    /// - so strict eval code cannot create a global by assignment and sloppy eval code can.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=290144
    // Broiler-Falsified-If: a write through the map reaches a slot the map does not name, or succeeds on an immutable or uninitialised binding
    // Broiler-Human:        PENDING
    private void WriteEvalName(JsEnvironment boundary, string name, JsValue value, bool strict)
    {
        var binding = ResolveEvalName(boundary, name);

        if (binding.Record is { } record)
        {
            if (record.Slots[binding.Slot].IsEmpty)
            {
                ThrowReferenceError("Cannot access a binding before initialisation");
            }

            if (binding.Immutable)
            {
                // A NAMED FUNCTION EXPRESSION'S OWN NAME IS IMMUTABLE AND NOT STRICT: sloppy eval
                // code's `g = 1` is ignored, as the lowering ignores the same store outside eval,
                // and strict eval code's throws. Until VM-FIX-D's review the slot was written.
                if (binding.FunctionName && !strict)
                {
                    return;
                }

                throw Error("TypeError", "assignment to constant variable " + name);
            }

            record.Slots[binding.Slot] = value;
            return;
        }

        // AN IMPORT IS AN IMMUTABLE BINDING, and assigning to one is the TypeError it is outside
        // eval code (JSeal V15-module).
        if (binding.Importer is not null)
        {
            throw Error("TypeError", "assignment to constant variable " + name);
        }

        if (binding.Object is { } holder)
        {
            SetProperty(JsValue.Object(holder), name, value, strict);
            return;
        }

        if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
        {
            if (!bound.Initialised)
            {
                ThrowReferenceError("cannot access " + name + " before its declaration");
            }

            if (!bound.Mutable)
            {
                throw Error("TypeError", "assignment to constant variable " + name);
            }

            bound.Value = value;
            return;
        }

        if (strict && !HasProperty(Realm.GlobalObject, name))
        {
            ThrowReferenceError(name + " is not defined");
        }

        SetProperty(JsValue.Object(Realm.GlobalObject), name, value, strict);
    }

    /// <summary>Answers <c>delete</c> of one of the caller's names from eval code.</summary>
    /// <remarks>
    /// A declarative binding is never deletable and answers <c>false</c>; a <c>with</c> object's
    /// property is deleted from the object, with <c>DeleteProperty</c>'s strict rule; a global
    /// answers what <c>DeleteGlobalBinding</c> answers; and a name nothing binds answers
    /// <c>true</c>, because there was nothing to keep.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BA5477
    // Broiler-Human:        PENDING
    private JsValue DeleteEvalName(JsEnvironment boundary, string name, bool strict)
    {
        var binding = ResolveEvalName(boundary, name);

        if (binding.Record is not null || binding.Importer is not null)
        {
            return JsValue.False;
        }

        if (binding.Object is { } holder)
        {
            var went = holder.DeleteOwnProperty(name);

            if (!went && strict)
            {
                ThrowTypeError("Cannot delete property '" + name + "'");
            }

            return JsValue.Boolean(went);
        }

        return JsValue.Boolean(
            (!Realm.HasLexicals || !Realm.TryLexical(name, out _)) &&
            (!Realm.GlobalObject.HasOwnProperty(name) || Realm.GlobalObject.DeleteOwnProperty(name)));
    }

    /// <summary>
    /// Writes one line of text to whatever the composition registered, or nowhere.
    /// </summary>
    /// <remarks>
    /// The two sinks are deliberately different things. <see cref="Output"/> is an in-process hook
    /// a test host sets to capture what a program printed; the capability is the host boundary a
    /// real composition registers. A program that prints reaches both when both exist and neither
    /// when neither does, and in no case does it reach a console this profile opened itself.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5B318E
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

        var bytes = System.Text.Encoding.UTF8.GetBytes(text);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=53FBA5
    // Broiler-Human:        PENDING
    internal JsHostRealm HostRealm => hostRealm ??= new JsHostRealm(this);

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

    /// <summary>The realm this engine runs in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B48D28
    // Broiler-Human:        PENDING
    internal JsRealm Realm { get; }

    /// <summary>Whatever the host wired to <c>print</c>, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EED672
    // Broiler-Human:        PENDING
    internal System.Action<string>? Output { get; set; }

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
    /// Ends the operation when the runtime's stack probe refuses, naming the dimension it refused
    /// on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The refusal is real and it was reported as nothing.</b> A call whose stack probe says no
    /// has nowhere left to build an error object, so it ends the operation rather than throwing a
    /// <c>RangeError</c> — that half is <see cref="MaximumCallDepth"/>'s remarks and is right. What
    /// it did NOT do was say what ran out: the abort became
    /// <c>ProfileFault</c>/<c>AllowanceExhausted</c>, which names no dimension and no scope, and
    /// release gate 4 asks that a call-stack overflow be <b>reported as a resource exhaustion
    /// naming a dimension</b> and not be fatal. Half of that held — nothing terminated — and the
    /// naming half did not.
    /// </para>
    /// <para>
    /// <b>The meter is how a profile names a dimension, because a profile may not mint a core
    /// outcome.</b> Charging a quantity of <c>CallDepth</c> no level can admit is the truthful
    /// statement of what happened — this interpreter cannot take another frame — and it is the
    /// only statement of it the contract has. The core's own precedence then reports
    /// <c>ResourceExhaustion</c>/<c>CeilingReached</c> carrying <c>CallDepth</c> and the scope that
    /// refused, ahead of whatever step this abort produces. Nothing is committed by a refused
    /// charge, so the meter is left exactly as it was.
    /// </para>
    /// <para>
    /// <b>What it costs a reader is worth stating.</b> The dimension named is the one the guest was
    /// spending and not the machine resource that ran out, so an operator who raises
    /// <c>CallDepth</c> after meeting this gets no further — the stack, not the ceiling, is what
    /// refused. That is the same trade the counted bound makes in the other direction, and the
    /// alternative was an answer that named nothing at all.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=94E6AA
    // Broiler-Falsified-If: the stack backstop produces a result naming no dimension, or a refused charge commits anything
    // Broiler-Human:        PENDING
    private JsAbort StackBackstopReached()
    {
        // The whole range, so that no ceiling a host could grant admits it and the outermost level
        // that refuses is the one reported.
        _ = meter.TryCharge(VmBudgetDimension.CallDepth, ulong.MaxValue);

        return new JsAbort(JsAbortKind.Exhausted, "the call-depth backstop was reached");
    }

    /// <summary>
    /// Charges for work whose size is a number of characters, never less than one unit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The comparison and conversion families charged a flat amount, and a flat charge over an
    /// input the guest chooses is a budget that bounds nothing.</b> Roadmap section 8 names
    /// <i>string concatenation and comparison</i> and <i>numeric conversion of large values</i>
    /// among the families whose cost grows with their input, and asks each to declare a monotone
    /// non-decreasing charging function. Concatenation had one and comparison did not: comparing
    /// two strings of any length cost sixteen units, measured, and so did reading a number out of a
    /// string of any length.
    /// </para>
    /// <para>
    /// <b>The unit is a character and the declared function is the work's own bound.</b> An ordinal
    /// comparison stops at the first difference and therefore cannot read past the shorter operand,
    /// so its function is <c>min(|a|, |b|) + 1</c>; reading a number out of a string trims and scans
    /// the whole of it, so its function is <c>|s| + 1</c>. Both are monotone non-decreasing in the
    /// magnitude of the input, and the declared granularity is one, so the charge is the function
    /// itself rather than a ceiling over a window.
    /// </para>
    /// <para>
    /// The <c>+ 1</c> is not rounding: a comparison of two empty strings is still a comparison, and
    /// a family whose charge can be zero is a family a program can perform without limit.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=626201
    // Broiler-Falsified-If: a text operation's charge does not grow with the input the guest controls
    // Broiler-Human:        PENDING
    internal void ChargeText(int units) => Charge(units <= 0 ? 1UL : (ulong)units + 1UL);

    /// <summary>Charges one crossing of the host surface: a host call, and its fuel.</summary>
    /// <remarks>
    /// <para>
    /// <b>It charges <c>HostCalls</c> as well as <c>Fuel</c>, and that is what makes the host
    /// surface cost what the capability channel costs.</b> A crossing does not go through
    /// <c>IVmHostCapabilityInvoker</c> - a host object is an ordinary object in this realm and
    /// calling its method is an ordinary call - so the boundary charge
    /// <c>VmCapabilityBinding.TryEnter</c> applies is not applied for us. Left uncharged, an
    /// embedder could move an unbounded amount of work across the seam and pay for none of it,
    /// which would make a host-call ceiling a ceiling on the wrong thing.
    /// </para>
    /// <para>
    /// The fuel is proportional to what the crossing carries - an argument count, a text length -
    /// because a flat charge over a guest-controlled quantity is a charge a guest can dilute to
    /// nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=62A22B
    // Broiler-Falsified-If: a crossing of the host surface completes without charging HostCalls
    // Broiler-Human:        PENDING
    internal void ChargeHostCrossing(ulong units)
    {
        if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the host-call allowance is spent");
        }

        Charge(units);
    }

    /// <summary>Charges fuel, aborting when the allowance is spent.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4AFB70
    // Broiler-Human:        PENDING
    internal void Charge(ulong units)
    {
        while (units > PollWindow)
        {
            ChargeOnce(PollWindow);
            units -= PollWindow;
        }

        ChargeOnce(units);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8CD7C8
    // Broiler-Human:        PENDING
    private void ChargeOnce(ulong units)
    {
        if (!meter.TryCharge(VmBudgetDimension.Fuel, units))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the instruction allowance is spent");
        }

        sinceLastPoll += units;

        if (sinceLastPoll < PollWindow)
        {
            return;
        }

        sinceLastPoll = 0;

        if (cancellation.IsCancellationRequested)
        {
            throw new JsAbort(JsAbortKind.Cancelled, "cancellation was requested");
        }

        if (!meter.Poll())
        {
            throw new JsAbort(JsAbortKind.Exhausted, "a budget dimension was reached");
        }
    }

    /// <summary>Reports bytes an allocation retained, so LiveBytes stays a ceiling.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=2C6F27
    // Broiler-Human:        PENDING
    internal void Retain(ulong bytes) =>
        meter.ReportRetained(VmBudgetDimension.LiveBytes, bytes);

    /// <summary>
    /// Retains bytes only if every level admits them, aborting BEFORE the allocation otherwise.
    /// </summary>
    /// <remarks>
    /// <b><see cref="Retain"/> reports after the fact, and a host that is handed a value cannot be
    /// told one operation later that it should not have been.</b> A retention report returns
    /// nothing and its refusal is latched for the next charge or poll, which is right for a guest
    /// allocation whose result the same operation is about to fail anyway. An embedder building a
    /// buffer through the host surface would already hold the value by then, so this gates on
    /// <c>TryCharge</c> - which is the meter's own statement of how a caller that must observe a
    /// ceiling refusal does it - and the refusal is decided before a byte is allocated.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=13932D
    // Broiler-Falsified-If: a retention the LiveBytes ceiling refuses returns normally or leaves the allocation to happen
    // Broiler-Human:        PENDING
    internal void RetainOrAbort(ulong bytes)
    {
        if (!meter.TryCharge(VmBudgetDimension.LiveBytes, bytes))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the live-bytes ceiling does not admit the allocation");
        }
    }

    // ---- throwing ------------------------------------------------------------------------------

    /// <summary>Throws a <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D6FB0C
    // Broiler-Human:        PENDING
    internal JsValue ThrowTypeError(string message) => throw Error("TypeError", message);

    /// <summary>Throws a <c>RangeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=92484D
    // Broiler-Human:        PENDING
    internal JsValue ThrowRangeError(string message) => throw Error("RangeError", message);

    /// <summary>Throws a <c>ReferenceError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=97EC2F
    // Broiler-Human:        PENDING
    internal JsValue ThrowReferenceError(string message) => throw Error("ReferenceError", message);

    /// <summary>Throws a <c>SyntaxError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=173F3B
    // Broiler-Human:        PENDING
    internal JsValue ThrowSyntaxError(string message) => throw Error("SyntaxError", message);

    /// <summary>Builds a throw carrying a fresh Error of the named intrinsic kind.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=66C1DC
    // Broiler-Human:        PENDING
    internal JsThrow Error(string kind, string message)
    {
        var error = Realm.CreateError(kind, message);
        return new JsThrow(error, kind + ": " + message);
    }

    // ---- conversions ---------------------------------------------------------------------------

    /// <summary>The abstract operation <c>ToPrimitive</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B7C265
    // Broiler-Human:        PENDING
    internal JsValue ToPrimitive(JsValue value, string hint)
    {
        if (!value.IsObject)
        {
            return value;
        }

        // `Symbol.toPrimitive` COMES FIRST AND ANSWERS FOR THE WHOLE OPERATION. It is what makes a
        // Symbol refuse to become a String, and what lets a Date distinguish the three hints; a
        // conversion that consulted `valueOf` first would have already produced an answer before the
        // object could say it has none.
        if (TryGetSymbolMethod(value, Realm.ToPrimitiveSymbol, out var exotic))
        {
            var answered = Call(exotic, value, [JsValue.String(hint)]);

            if (!answered.IsObject)
            {
                return answered;
            }

            return ThrowTypeError("Cannot convert object to primitive value");
        }

        return OrdinaryToPrimitive(value, hint);
    }

    /// <summary>
    /// The abstract operation <c>OrdinaryToPrimitive</c>: the two methods, in the hint's order.
    /// </summary>
    /// <remarks>
    /// It is separate from <see cref="ToPrimitive"/> because an exotic
    /// <c>Symbol.toPrimitive</c> can need it: a Date's answers the <c>"default"</c> hint by asking
    /// for the <c>"string"</c> ordering, and an implementation that recursed into
    /// <see cref="ToPrimitive"/> to get it would find its own exotic method again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5A1F89
    // Broiler-Human:        PENDING
    internal JsValue OrdinaryToPrimitive(JsValue value, string hint)
    {
        var order = string.Equals(hint, "string", System.StringComparison.Ordinal)
            ? new[] { "toString", "valueOf" }
            : ["valueOf", "toString"];

        foreach (var name in order)
        {
            var method = GetProperty(value, name);

            if (method.IsObject && method.AsObject().IsCallable)
            {
                var result = Call(method, value, System.Array.Empty<JsValue>());

                if (!result.IsObject)
                {
                    return result;
                }
            }
        }

        return ThrowTypeError("Cannot convert object to primitive value");
    }

    /// <summary>The abstract operation <c>ToNumber</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B3645E
    // Broiler-Human:        PENDING
    internal double ToNumber(JsValue value) => value.Type switch
    {
        JsType.Number => value.AsNumber(),
        JsType.Boolean => value.AsBoolean() ? 1 : 0,
        JsType.Undefined => double.NaN,
        JsType.Null => 0,
        JsType.String => ToNumberFromText(value.AsString()),

        // A SYMBOL HAS TO BE REFUSED HERE AND NOT LEFT TO THE ARM BELOW. `ToPrimitive` of a
        // primitive is that primitive, so a Symbol reaching the recursive arm converts to itself
        // for ever: the process dies of a stack overflow, which is the one failure this profile
        // may never produce. `ToString` already refuses a Symbol by name; this is the same refusal
        // on the other conversion, and the reason is the same - a Symbol is a key nobody can
        // forge, and a key that silently became a number would be forgeable by arithmetic.
        JsType.Symbol => ThrowTypeError("Cannot convert a Symbol value to a number").AsNumber(),

        // A BIGINT IS REFUSED BY `ToNumber` ITSELF, which is the specification's own answer and not
        // a placeholder: `ToNumber(1n)` is a TypeError. The operations that may turn a BigInt into
        // a Number - `Number(x)` alone - do so through `ToNumeric` and `JsBigInt.ToNumber`, and the
        // operators and comparisons that take either type convert with `ToNumeric` (JSeal B03-B05).
        // What stops here is unary `+`, `Math`, `isNaN` and every other operation that asks for a
        // Number, none of which may produce one from a BigInt. Leaving it to the arm below would
        // loop exactly as a Symbol did, because a BigInt is its own primitive.
        JsType.BigInt => ThrowTypeError("Cannot convert a BigInt value to a number").AsNumber(),
        _ => ToNumber(ToPrimitive(value, "number")),
    };

    /// <summary>Reads a number out of text, charging for the text it reads.</summary>
    /// <remarks>
    /// The conversion trims and scans the whole string, so its cost is the string's own length —
    /// which the guest chooses, and which was charged nothing until this existed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=07B791
    // Broiler-Falsified-If: a longer numeric string is read for the same charge as a shorter one
    // Broiler-Human:        PENDING
    private double ToNumberFromText(string text)
    {
        ChargeText(text.Length);
        return JsNumberFormat.ToNumber(text);
    }

    /// <summary>
    /// The abstract operation <c>ToNumeric</c>: a Number or a BigInt, from one conversion.
    /// </summary>
    /// <remarks>
    /// An object is converted to a primitive once, with the <c>"number"</c> hint, and a BigInt it
    /// answers is kept; anything else goes on to <see cref="ToNumber"/>, which is where a Symbol is
    /// refused. A value of either numeric type is returned as it is, so a Number operand costs one
    /// test more than it did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=5C1B6B
    // Broiler-Falsified-If: an object operand is converted to a primitive twice, or a BigInt reaches ToNumber through this operation
    // Broiler-Human:        PENDING
    internal JsValue ToNumeric(JsValue value)
    {
        if (value.Type is JsType.Number or JsType.BigInt)
        {
            return value;
        }

        var primitive = value.IsObject ? ToPrimitive(value, "number") : value;
        return primitive.IsBigInt ? primitive : JsValue.Number(ToNumber(primitive));
    }

    /// <summary>The abstract operation <c>ToString</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C35FB5
    // Broiler-Human:        PENDING
    internal string ToStringValue(JsValue value) => value.Type switch
    {
        JsType.String => value.AsString(),
        JsType.Number => JsNumberFormat.ToJsString(value.AsNumber()),
        JsType.Boolean => value.AsBoolean() ? "true" : "false",
        JsType.Undefined => "undefined",
        JsType.Null => "null",

        // A SYMBOL DOES NOT COERCE, AND THAT IS THE WHOLE POINT OF THE TYPE. Every other primitive
        // has a String it turns into, so a Symbol that also had one would be usable everywhere a
        // String is - which is exactly what a key nobody can forge must not be. `String(symbol)`
        // and `symbol.toString()` are the explicit forms the language nonetheless provides, and
        // they go through the Symbol intrinsic rather than through here.
        JsType.Symbol => ThrowTypeError("Cannot convert a Symbol value to a string").AsString(),

        // A BIGINT HAS A STRING, AND IT IS EXACT: `BigInt::toString(x, 10)`, with no `n`. The
        // conversion is charged on the digits it can produce before it produces them.
        JsType.BigInt => BigIntText(value.AsBigInt()),
        _ => ToStringValue(ToPrimitive(value, "string")),
    };

    /// <summary>
    /// <c>BigInt::toString(x, 10)</c>, charged on the digits it can produce and on the square of the
    /// value's width, step by step (JSeal B03).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E0D910
    // Broiler-Falsified-If: a wider BigInt is converted to text for the same charge as a narrower one
    // Broiler-Human:        PENDING
    private string BigIntText(JsBigInt value) => value.ToDecimalString(ChargeFuel);

    /// <summary>
    /// <see cref="Charge"/> as a delegate, made once, which is how the BigInt operations charge fuel
    /// - and so meet the cancellation poll - between the steps of a long operation.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=718B75
    // Broiler-Human:        PENDING
    internal System.Action<ulong> ChargeFuel => chargeFuel ??= Charge;

    /// <summary>The delegate <see cref="ChargeFuel"/> made.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5990A0
    // Broiler-Human:        PENDING
    private System.Action<ulong>? chargeFuel;

    /// <summary>
    /// The abstract operation <c>ToBigInt</c>: a BigInt from a Boolean, a BigInt or a String, after
    /// one <c>ToPrimitive</c> with the <c>"number"</c> hint; anything else is refused as the
    /// specification refuses it.
    /// </summary>
    /// <remarks>
    /// <b>A Number is a TypeError here, not a conversion</b>: <c>BigInt.asIntN(8, 1)</c> must not
    /// quietly accept a value that might have lost its low bits on the way in. The one caller that
    /// takes a Number, the <c>BigInt</c> function, tests for it before calling this.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=66E934
    // Broiler-Falsified-If: a Number, undefined, null or a Symbol converts to a BigInt, or a String outside StringIntegerLiteral answers anything but a SyntaxError
    // Broiler-Human:        PENDING
    internal JsBigInt ToBigInt(JsValue value) => PrimitiveToBigInt(ToPrimitive(value, "number"));

    /// <summary><see cref="ToBigInt"/> after its <c>ToPrimitive</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=29E42B
    // Broiler-Falsified-If: a Number, undefined, null or a Symbol converts to a BigInt
    // Broiler-Human:        PENDING
    internal JsBigInt PrimitiveToBigInt(JsValue primitive)
    {
        switch (primitive.Type)
        {
            case JsType.BigInt:
                return primitive.AsBigInt();

            case JsType.Boolean:
                return primitive.AsBoolean() ? JsBigInt.One : JsBigInt.Zero;

            case JsType.String:
                return StringToBigIntOrThrow(primitive.AsString());

            case JsType.Number:
                throw Error(
                    "TypeError",
                    "Cannot convert " + JsNumberFormat.ToJsString(primitive.AsNumber()) + " to a BigInt");

            case JsType.Symbol:
                throw Error("TypeError", "Cannot convert a Symbol value to a BigInt");

            default:
                throw Error(
                    "TypeError",
                    "Cannot convert " + (primitive.Type == JsType.Null ? "null" : "undefined") + " to a BigInt");
        }
    }

    /// <summary>
    /// <c>StringToBigInt</c> for a caller that needs a value: a SyntaxError for text outside the
    /// grammar, and the ceiling's RangeError for a value too wide for the realm.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=60A446
    // Broiler-Falsified-If: a text outside StringIntegerLiteral answers a value, or a value past the ceiling is answered
    // Broiler-Human:        PENDING
    internal JsBigInt StringToBigIntOrThrow(string text)
    {
        if (!JsBigInt.TryParseStringInteger(text, ChargeFuel, out var parsed, out _))
        {
            throw Error(
                "SyntaxError",
                "Cannot convert " + (text.Length > 64 ? text[..64] + "..." : text) + " to a BigInt");
        }

        return parsed ?? BigIntResult(null).AsBigInt();
    }

    /// <summary>
    /// The abstract operation <c>NumberToBigInt</c>: the exact integer an integral Number is, and a
    /// RangeError for a fraction, a NaN or an infinity.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=9A4F6D
    // Broiler-Falsified-If: a Number that is not an integer converts to a BigInt, or an integral one converts to any integer but its own
    // Broiler-Human:        PENDING
    internal JsBigInt NumberToBigInt(double number)
    {
        if (!double.IsFinite(number) || System.Math.Floor(number) != number)
        {
            throw Error(
                "RangeError",
                "The number " + JsNumberFormat.ToJsString(number) +
                " cannot be converted to a BigInt because it is not an integer");
        }

        return JsBigInt.FromIntegralNumber(number);
    }

    /// <summary>
    /// The conversion every write into a typed-array element makes first: <c>ToBigInt</c> for a
    /// BigInt kind and <c>ToNumber</c> for every other, as the specification's
    /// <c>TypedArraySetElement</c> and the built-ins that follow it choose by the view's
    /// <c>[[ContentType]]</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The answer is a value the element write stores without converting again</b>, and it is
    /// always of the view's content type: a Number for a Number kind, a BigInt for a BigInt kind.
    /// Assigning <c>1</c> to a <c>BigInt64Array</c> element is therefore the <c>TypeError</c>
    /// <c>ToBigInt</c> owes a Number, and assigning <c>1n</c> to a <c>Float64Array</c> element the
    /// one <c>ToNumber</c> owes a BigInt; neither is silently converted the other way.
    /// </para>
    /// <para>
    /// <b>A BigInt wider than 64 bits is narrowed here, once, and charged for it</b>, because the
    /// narrowing reads every word of the value; the narrowed value writes the same eight bytes the
    /// wide one would, so a <c>fill</c> that stores it a million times masks it once.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=AB31B4
    // Broiler-Falsified-If: a BigInt kind accepts a Number, a Number kind accepts a BigInt, or either converts with the other content type's operation
    // Broiler-Human:        PENDING
    internal JsValue ToElementValue(JsElementKind kind, JsValue value)
    {
        if (!JsElements.HoldsBigInts(kind))
        {
            return JsValue.Number(ToNumber(value));
        }

        var integer = ToBigInt(value);

        if (integer.Bits <= 64)
        {
            return JsValue.BigInt(integer);
        }

        Charge(JsBigInt.LinearCost(integer.Words));
        return JsValue.BigInt(JsElements.Narrow(kind, integer));
    }

    /// <summary>
    /// The Number a BigInt stands for, <c>F(R(x))</c>: the nearest one, ties to even, charged on
    /// its width. <c>Number(x)</c> is the one operation that asks for it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=1D0F36
    // Broiler-Falsified-If: a BigInt converts to a Number other than the nearest one with ties to even
    // Broiler-Human:        PENDING
    internal double BigIntToNumber(JsBigInt value) => value.ToNumber(ChargeFuel);

    /// <summary>The abstract operation <c>ToObject</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=46C66A
    // Broiler-Human:        PENDING
    internal JsObject ToObject(JsValue value) => value.Type switch
    {
        JsType.Object => value.AsObject(),
        JsType.String => Realm.WrapString(value.AsString()),
        JsType.Number => new JsPrimitiveWrapper(Realm.NumberPrototype, "Number", value),
        JsType.Boolean => new JsPrimitiveWrapper(Realm.BooleanPrototype, "Boolean", value),
        JsType.Symbol => new JsPrimitiveWrapper(Realm.SymbolPrototype, "Symbol", value),

        // A BIGINT WRAPPER IS AN ORDINARY OBJECT WITH A [[BigIntData]] SLOT (JSeal B05): its
        // `Object.prototype.toString` tag comes from `BigInt.prototype[Symbol.toStringTag]` and not
        // from a class name, so the class is `Object`, as the specification's builtinTag is.
        JsType.BigInt => new JsPrimitiveWrapper(BigIntPrototypeOrRefuse(), "Object", value),
        _ => (JsObject)ThrowTypeError("Cannot convert undefined or null to object").AsObject(),
    };

    /// <summary>The abstract operation <c>ToInt32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1BA637
    // Broiler-Human:        PENDING
    internal int ToInt32(JsValue value) => JsValue.ToInt32(ToNumber(value));

    /// <summary>The abstract operation <c>ToUint32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DBC89F
    // Broiler-Human:        PENDING
    internal uint ToUint32(JsValue value) => JsValue.ToUint32(ToNumber(value));

    /// <summary>The abstract operation <c>ToIntegerOrInfinity</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F80693
    // Broiler-Human:        PENDING
    internal double ToInteger(JsValue value) => JsValue.ToInteger(ToNumber(value));

    /// <summary>The abstract operation <c>ToPropertyKey</c>, over the string keys this surface has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7A65ED
    // Broiler-Human:        PENDING
    internal string ToPropertyKey(JsValue value) =>
        value.Type == JsType.String ? value.AsString() : ToStringValue(value);

    /// <summary>
    /// The abstract operation <c>ToPropertyKey</c> over both kinds of key, answering a String or a
    /// Symbol value.
    /// </summary>
    /// <remarks>
    /// <b>A Symbol survives, and so does a Symbol an object converts to.</b> <see cref="ToPropertyKey"/>
    /// answers a string and therefore refuses both, which is right only where the caller has already
    /// taken the Symbol path. This one is for a caller that has not: it runs <c>ToPrimitive</c> with
    /// the string hint exactly once, keeps a Symbol it produces, and converts anything else with
    /// <c>ToString</c>, which for a primitive runs no guest code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=AD4C8B
    // Broiler-Human:        PENDING
    internal JsValue ToPropertyKeyValue(JsValue value)
    {
        if (value.IsSymbol || value.Type == JsType.String)
        {
            return value;
        }

        var primitive = ToPrimitive(value, "string");
        return primitive.IsSymbol ? primitive : JsValue.String(ToStringValue(primitive));
    }

    /// <summary>
    /// The one Symbol-keyed lookup the engine performs on its own behalf, for a well-known Symbol.
    /// </summary>
    /// <remarks>
    /// Every protocol the language expresses through a well-known Symbol — iteration, primitive
    /// coercion, instance testing — reads a method off a value and calls it. This is that read: it
    /// answers nothing when the property is absent or nullish, and a <c>TypeError</c> when it is
    /// present and not callable, which is what every one of those protocols says to do.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B7FC6
    // Broiler-Human:        PENDING
    internal bool TryGetSymbolMethod(JsValue value, JsSymbol key, out JsValue method)
    {
        var found = GetSymbol(value, key);

        if (found.IsNullish)
        {
            method = JsValue.Undefined;
            return false;
        }

        if (!found.IsObject || !found.AsObject().IsCallable)
        {
            ThrowTypeError("a Symbol-keyed protocol member is not a function");
        }

        method = found;
        return true;
    }

    // ---- properties ----------------------------------------------------------------------------

    /// <summary>The prototype a primitive's property lookup starts from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4659EB
    // Broiler-Human:        PENDING
    private JsObject? PrototypeFor(JsValue value) => value.Type switch
    {
        JsType.String => Realm.StringPrototype,
        JsType.Number => Realm.NumberPrototype,
        JsType.Boolean => Realm.BooleanPrototype,
        JsType.Symbol => Realm.SymbolPrototype,

        JsType.BigInt => BigIntPrototypeOrRefuse(),
        _ => null,
    };

    /// <summary>
    /// <c>BigInt.prototype</c>, which a realm has whenever a BigInt can reach it.
    /// </summary>
    /// <remarks>
    /// <b>A composition that declines the BigInt surface has no BigInt values</b>: every artifact
    /// holding a BigInt constant or naming the <c>BigInt</c> global is refused at its verification,
    /// and no other source makes one. So this refusal is unreachable by a program; it exists so that
    /// a value that arrived some way nobody foresaw is refused by name rather than read as an object
    /// with no chain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=31B5DF
    // Broiler-Human:        PENDING
    private JsObject BigIntPrototypeOrRefuse() =>
        Realm.BigIntPrototype ??
        throw Error("TypeError", "this composition declined the BigInt surface, so BigInt.prototype does not exist");

    /// <summary>Reads a property off any value, walking the prototype chain.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=62779B
    // Broiler-Human:        PENDING
    internal JsValue GetProperty(JsValue baseValue, string key)
    {
        if (baseValue.IsNullish)
        {
            return ThrowTypeError(
                "Cannot read properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (reading '" + key + "')");
        }

        if (baseValue.IsString)
        {
            var text = baseValue.AsString();

            if (string.Equals(key, "length", System.StringComparison.Ordinal))
            {
                return JsValue.Number(text.Length);
            }

            if (JsObject.IsArrayIndex(key, out var at))
            {
                return at < text.Length
                    ? JsValue.String(text[(int)at].ToString())
                    : JsValue.Undefined;
            }
        }

        var start = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);

        // AN INTEGER-INDEXED EXOTIC OBJECT DOES NOT INHERIT ITS INDICES, and that is a property of
        // [[Get]] rather than of [[GetOwnProperty]] — so it cannot be expressed by an override on
        // the object and has to be expressed here. Without it a realm in which somebody wrote
        // `Object.prototype[9] = 42` would answer 42 for `new Int32Array(3)[9]`, where the language
        // says `undefined`: the index is out of the view, and out of the view is the end of the
        // search rather than the start of a walk.
        //
        // The same holds for a numeric key that is not an index - "-0", "1.5", "-1": it names no
        // element, so the answer is `undefined` without a walk (JsTypedArray.IsNumericKey).
        if (start is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            return view.TryGetOwnProperty(key, out var element) ? element.Value : JsValue.Undefined;
        }

        return start is null ? JsValue.Undefined : Lookup(start, key, baseValue);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A1708E
    // Broiler-Human:        PENDING
    private JsValue Lookup(JsObject start, string key, JsValue receiver)
    {
        var current = start;

        while (current is not null)
        {
            // A PROXY SWALLOWS THE REST OF THE WALK RATHER THAN ANSWERING ONE LINK OF IT. `[[Get]]`
            // on a proxy is a whole operation - the `get` trap decides everything, including
            // whether a prototype is consulted at all - so continuing the loop past it would run
            // the trap for the own property and then walk the TARGET's chain behind its back. The
            // test is inside the loop because a proxy is just as likely to be somebody's
            // prototype as it is to be the object a program named.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyGet(JsValue.String(key), receiver);
            }

            // A TYPED ARRAY PART-WAY UP THE CHAIN ENDS THE WALK FOR A NUMERIC KEY, for the reason
            // it does at the start: its [[Get]] answers the element or `undefined` and never asks
            // its own prototype.
            if (current is JsTypedArray passedView && JsTypedArray.IsNumericKey(key))
            {
                return passedView.TryGetOwnProperty(key, out var element)
                    ? element.Value
                    : JsValue.Undefined;
            }

            if (current.TryGetOwnProperty(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), receiver, System.Array.Empty<JsValue>());
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes a property on any value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=BEFA3A
    // Broiler-Human:        PENDING
    internal void SetProperty(JsValue baseValue, string key, JsValue value, bool strict)
    {
        if (baseValue.IsNullish)
        {
            ThrowTypeError(
                "Cannot set properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (setting '" + key + "')");

            return;
        }

        var current = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);
        var target = baseValue.AsObjectOrNull();

        // AN ARRAY'S `length` IS THE ONE PROPERTY WHOSE VALUE IS CHECKED BEFORE IT IS STORED, and
        // the check has to be here because it can THROW: `a.length = -1` is a RangeError in every
        // engine, and the object model has no engine to raise one with. What reaches the object is
        // the coerced number, so `a.length = "2"` sets two rather than nothing.
        // A `length` that is already closed refuses the write before anything is converted:
        // `OrdinarySet` stops at the non-writable own property and never reaches `ArraySetLength`,
        // so a `valueOf` on the assigned value does not run.
        if (target is JsArray sized && string.Equals(key, "length", System.StringComparison.Ordinal))
        {
            if (!sized.LengthWritable)
            {
                if (strict)
                {
                    ThrowTypeError("Cannot assign to read only property 'length' of object '[object Array]'");
                }

                return;
            }

            value = JsValue.Number(ArrayLengthOrRefuse(value));
        }

        // A WRITE PAST A CLOSED LENGTH IS A REFUSAL, and in strict code a refusal is a TypeError.
        // The object model drops the write silently because it cannot know the mode, so the mode's
        // half of the answer belongs here.
        if (target is JsArray fixedLength && !fixedLength.LengthWritable &&
            JsObject.IsArrayIndex(key, out var past) && past >= fixedLength.Length)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        // THE ELEMENT CONVERSION IS THE ENGINE'S BECAUSE IT CAN RUN `valueOf`. The object model
        // stores an element without an engine to hand, so it can convert a primitive exactly and
        // nothing else; the language says a write to an integer-indexed element is `ToNumber` of
        // whatever was assigned, and `ToNumber` of an object is a call. Doing it here is also what
        // makes the write silently discarded when the index is out of the view or the buffer is
        // detached — after the conversion has happened, which is the order the specification asks
        // for and is observable through a `valueOf` with a side effect.
        if (target is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            // A numeric key that is not an index is converted and then discarded the same way, and
            // it never walks the chain: no inherited setter may see it. The conversion is the one
            // the view's content type names: ToBigInt for a BigInt kind (JSeal B07).
            var element = ToElementValue(view.Kind, value);

            if (JsObject.IsArrayIndex(key, out var at))
            {
                _ = view.TryWriteAt((int)at, element);
            }

            return;
        }

        // A NAMESPACE REFUSES EVERY WRITE, and in strict code a refused write is a `TypeError`.
        // The walk below would find an export's own property, see it is writable - which it is,
        // and which a program can read off the descriptor - and let the assignment land in a copy
        // that no longer tracks the module's binding. Module code is always strict, so this is a
        // throw wherever an import is in scope; the sloppy branch is here for a namespace that
        // reached a script through a host.
        if (target is JsModuleNamespace)
        {
            if (strict)
            {
                ThrowTypeError(
                    "Cannot assign to '" + key + "' of a module namespace object");
            }

            return;
        }

        while (current is not null)
        {
            // A PROXY ANSWERS `[[Set]]` WHOLE, and the refusal it reports is turned into the
            // language's own two answers here: `false` is silent in sloppy code and a TypeError in
            // strict code, which is the rule every other refusal on this path already follows.
            if (current is JsProxy proxy)
            {
                if (!proxy.ProxySet(JsValue.String(key), value, baseValue) && strict)
                {
                    ThrowTypeError("Cannot assign to read only property '" + key + "'");
                }

                return;
            }

            // A TYPED ARRAY PART-WAY UP THE CHAIN, with a numeric key it holds no element for,
            // takes the write and discards it without converting the value: its [[Set]] answers
            // true for a receiver other than itself, and no setter behind it is reached. An element
            // it does hold is an ordinary writable data property from here on.
            if (current is JsTypedArray passedView && JsTypedArray.IsNumericKey(key) &&
                !passedView.TryGetOwnProperty(key, out _))
            {
                return;
            }

            if (current.TryGetOwnProperty(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError("Cannot set property " + key + " which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(property.Setter), baseValue, [value]);
                    return;
                }

                if (!property.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to read only property '" + key + "'");
                    }

                    return;
                }

                if (ReferenceEquals(current, target))
                {
                    property.Value = value;
                    target.SetOwnProperty(key, property);
                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        if (target is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create property '" + key + "' on a primitive");
            }

            return;
        }

        if (!target.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>An Array length, or the <c>RangeError</c> the language owes for anything else.</summary>
    /// <remarks>
    /// <b>The test is that the number survives the round trip</b>, which is the specification's own
    /// wording and is why <c>-1</c>, <c>1.5</c> and <c>NaN</c> are all refused while <c>"2"</c> is
    /// accepted: each of the three has a <c>ToUint32</c> that differs from its <c>ToNumber</c>, and
    /// the string does not.
    /// <para>
    /// <b>The value is converted twice, as <c>ArraySetLength</c> says</b>: once by
    /// <c>ToUint32</c> and once by <c>ToNumber</c>. An object's <c>valueOf</c> therefore runs twice
    /// and the two answers are compared, so one that answers differently the second time is a
    /// <c>RangeError</c> rather than whichever length the first call chose.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3406AB
    // Broiler-Human:        PENDING
    internal uint ArrayLengthOrRefuse(JsValue value)
    {
        var index = JsValue.ToUint32(ToNumber(value));
        var number = ToNumber(value);

        if (index != number)
        {
            throw Error("RangeError", "Invalid array length");
        }

        return index;
    }

    /// <summary>Defines an own data property under a key that is not known until it is evaluated.</summary>
    /// <remarks>
    /// <b>A computed member of an object literal DEFINES and does not assign</b>, and the two differ
    /// wherever the chain has an opinion: <c>{ [k]: v }</c> with <c>k</c> of <c>"__proto__"</c> makes
    /// an own property called <c>__proto__</c>, where an assignment would have found the accessor on
    /// <c>Object.prototype</c> and moved the object's prototype instead. The same difference shows
    /// against any setter, and against a read-only property inherited from a frozen prototype.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=339011
    // Broiler-Human:        PENDING
    private void DefineByKey(JsObject host, JsValue key, JsValue value)
    {
        if (key.IsSymbol)
        {
            host.SetOwnSymbol(key.AsSymbol(), JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        host.SetOwnProperty(ToPropertyKey(key), JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    // ---- the reflective forms of the two above -------------------------------------------------

    /// <summary>Reads a property off one object's chain with any accessor bound to another value.</summary>
    /// <remarks>
    /// <b>The chain that is walked and the <c>this</c> a getter sees are the same thing in every
    /// ordinary read</b>, because the base of the reference is both, and separating them is the
    /// entire reason <c>Reflect.get</c> takes a third argument. A program can run a getter it found
    /// on one object against an object that does not have it, which is how a class hierarchy reads
    /// an inherited accessor without inheriting.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B99E24
    // Broiler-Human:        PENDING
    internal JsValue GetWithReceiver(JsObject target, string key, JsValue receiver)
    {
        if (target is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            return view.TryGetOwnProperty(key, out var element) ? element.Value : JsValue.Undefined;
        }

        return Lookup(target, key, receiver);
    }

    /// <summary>The same read for a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B5B79
    // Broiler-Human:        PENDING
    internal JsValue GetSymbolWithReceiver(JsObject target, JsSymbol key, JsValue receiver)
    {
        var current = target;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WALK OBEYS, and for the same reason.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyGet(JsValue.Symbol(key), receiver);
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), receiver, System.Array.Empty<JsValue>());
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes through one object's chain, lands the write on another, and answers whether
    /// it took.</summary>
    /// <remarks>
    /// <para>
    /// <b>The answer is the point, and it is not the same question as "what does the property read
    /// back as".</b> A setter that discards what it was handed still took the write — the language
    /// says <c>[[Set]]</c> is true whenever a setter ran — and a read-back would call it a refusal.
    /// So this walks the chain itself rather than storing and looking.
    /// </para>
    /// <para>
    /// <b>The walk is over the target and the store is on the receiver</b>, which is what makes a
    /// data property found on a prototype shadow rather than overwrite. The two coincide for every
    /// call that does not name a receiver, which is nearly all of them.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D0728E
    // Broiler-Human:        PENDING
    internal bool SetWithReceiver(JsObject target, string key, JsValue value, JsValue receiver)
    {
        // A NUMERIC KEY ON A TYPED ARRAY IS THE INTEGER-INDEXED [[Set]]. When the view is its own
        // receiver the value is converted and written, or discarded if the key names no element;
        // for any other receiver a key that names no element is a success with no conversion,
        // and only a valid element goes on to the ordinary walk, which lands it on the receiver.
        if (target is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            if (receiver.IsObject && ReferenceEquals(receiver.AsObject(), view))
            {
                var element = ToElementValue(view.Kind, value);

                if (JsObject.IsArrayIndex(key, out var at))
                {
                    _ = view.TryWriteAt((int)at, element);
                }

                return true;
            }

            if (!view.TryGetOwnProperty(key, out _))
            {
                return true;
            }
        }

        // A NAMESPACE REFUSES EVERY WRITE, INCLUDING ONE TO A NAME IT DOES NOT EXPORT. Its export
        // properties read back as writable - the language says so, and a program can see it in the
        // descriptor - so the walk below would find a writable data property and let the write
        // land, silently turning a live binding into a copy. The refusal is unconditional and is
        // the object's, not the property's, which is why it is decided before the walk begins.
        if (target is JsModuleNamespace)
        {
            return false;
        }

        var current = target;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE, with the boolean this member already answers in.
            if (current is JsProxy proxy)
            {
                return proxy.ProxySet(JsValue.String(key), value, receiver);
            }

            // THE SAME RULE FOR A TYPED ARRAY PART-WAY UP THE CHAIN as for one at its start.
            if (current is JsTypedArray passedView && !ReferenceEquals(current, target) &&
                JsTypedArray.IsNumericKey(key))
            {
                if (receiver.IsObject && ReferenceEquals(receiver.AsObject(), passedView))
                {
                    var element = ToElementValue(passedView.Kind, value);

                    if (JsObject.IsArrayIndex(key, out var passedAt))
                    {
                        _ = passedView.TryWriteAt((int)passedAt, element);
                    }

                    return true;
                }

                if (!passedView.TryGetOwnProperty(key, out _))
                {
                    return true;
                }
            }

            if (current.TryGetOwnProperty(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        return false;
                    }

                    Call(JsValue.Object(property.Setter), receiver, [value]);
                    return true;
                }

                if (!property.Writable)
                {
                    return false;
                }

                break;
            }

            current = current.Prototype;
        }

        return LandOnReceiver(receiver, JsValue.String(key), value);
    }

    /// <summary>The same write for a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C15A9A
    // Broiler-Human:        PENDING
    internal bool SetSymbolWithReceiver(JsObject target, JsSymbol key, JsValue value, JsValue receiver)
    {
        var current = target;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE, with the boolean this member already answers in.
            if (current is JsProxy proxy)
            {
                return proxy.ProxySet(JsValue.Symbol(key), value, receiver);
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        return false;
                    }

                    Call(JsValue.Object(property.Setter), receiver, [value]);
                    return true;
                }

                if (!property.Writable)
                {
                    return false;
                }

                break;
            }

            current = current.Prototype;
        }

        return LandOnReceiver(receiver, JsValue.Symbol(key), value);
    }

    /// <summary>Where a reflective write ends up: an own property of the receiver, or a refusal.</summary>
    /// <remarks>
    /// <para>
    /// <b>The receiver is asked through its own internal methods and is handed exactly what the
    /// specification hands it.</b> <c>OrdinarySetWithOwnDescriptor</c> reads the receiver's own
    /// property with <c>[[GetOwnProperty]]</c>; an existing writable data property is updated with
    /// <c>[[DefineOwnProperty]](P, { [[Value]]: V })</c> - the value and nothing else, so a Proxy
    /// receiver's <c>defineProperty</c> trap sees a one-field descriptor and an Array receiver's
    /// <c>length</c> goes through the Array's own definition - and an absent one is created by
    /// <c>CreateDataProperty</c>. Restating the whole stored descriptor, as this used to, showed a
    /// trap fields the language never passes and threw where a trap's <c>false</c> is the answer.
    /// </para>
    /// <para>
    /// Both key kinds come here, so a Symbol-keyed write cannot drift from a String-keyed one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=E7D724
    // Broiler-Human:        PENDING
    private bool LandOnReceiver(JsValue receiver, JsValue key, JsValue value)
    {
        if (!receiver.IsObject)
        {
            return false;
        }

        var holder = receiver.AsObject();

        var held = key.IsSymbol
            ? holder.TryGetOwnSymbol(key.AsSymbol(), out var existing)
            : holder.TryGetOwnProperty(key.AsString(), out existing);

        var fields = new JsRealm.ObjectDescriptorFields { HasValue = true, Value = value };

        if (held)
        {
            if (existing.IsAccessor || !existing.Writable)
            {
                return false;
            }
        }
        else
        {
            fields.HasWritable = true;
            fields.Writable = true;
            fields.HasEnumerable = true;
            fields.Enumerable = true;
            fields.HasConfigurable = true;
            fields.Configurable = true;
        }

        return JsRealm.ObjectDefineOwn(this, holder, key, fields);
    }

    // ---- classes -------------------------------------------------------------------------------

    /// <summary>Reads a <c>this</c> that a derived constructor may not have bound yet.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7893CB
    // Broiler-Human:        PENDING
    private JsValue ThisBinding(JsCell binding)
    {
        if (binding.Value.IsEmpty)
        {
            ThrowReferenceError(
                "Must call super constructor in derived class before accessing 'this' or " +
                "returning from derived constructor");
        }

        return binding.Value;
    }

    /// <summary>
    /// Defines one method-shaped member, and gives it the home object that makes its <c>super</c>
    /// resolve.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The member's name is set here and not where the closure was made</b>, because a computed
    /// key is not known until now: <c>class C { [k]() { } }</c> has to report <c>k</c>'s value as
    /// the method's name, and an accessor reports <c>"get x"</c> rather than <c>"x"</c>. The code
    /// unit carries whatever the source spelled, which is right for the common case and empty for
    /// the computed one.
    /// </para>
    /// <para>
    /// A getter and a setter for one key are one property, so defining either keeps whichever half
    /// is already there - but only when what is already there is an accessor. A data property of
    /// the same name is replaced outright, which is what redeclaring it in a class body means.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=720A94
    // Broiler-Human:        PENDING
    private void DefineMember(JsObject host, string key, JsValue member, byte flags)
    {
        // DEFINING A MEMBER IS DefinePropertyOrThrow AND NOT AN UNCHECKED WRITE, and the one key
        // that can already be there and refuse is a class's own `prototype`: it is not
        // configurable, so `class C { static ['prototype']() { } }` is a TypeError rather than a
        // class whose `prototype` is a method. Every other key a member can name - `name`,
        // `length`, `constructor`, anything an object literal writes - is configurable, so this
        // refuses nothing a program is entitled to do.
        if (host.TryGetOwnProperty(key, out var standing) && !standing.Configurable)
        {
            ThrowTypeError("Cannot redefine property: " + key);
        }

        var getter = (flags & JsOpcodes.MemberIsGetter) != 0;
        var setter = (flags & JsOpcodes.MemberIsSetter) != 0;

        var attributes = JsPropertyAttributes.Configurable |
            ((flags & JsOpcodes.MemberIsEnumerable) != 0
                ? JsPropertyAttributes.Enumerable
                : JsPropertyAttributes.None);

        if (member.IsObject && member.AsObject() is JsScriptFunction bodied)
        {
            bodied.HomeObject = host;
            var label = getter ? "get " + key : setter ? "set " + key : key;
            bodied.FunctionName = label;

            bodied.SetOwnProperty(
                "name",
                JsProperty.Data(JsValue.String(label), JsPropertyAttributes.Configurable));
        }

        if (!getter && !setter)
        {
            host.SetOwnProperty(
                key, JsProperty.Data(member, attributes | JsPropertyAttributes.Writable));

            return;
        }

        host.TryGetOwnProperty(key, out var existing);
        var accessor = member.AsObjectOrNull();

        host.SetOwnProperty(
            key,
            JsProperty.Accessor(
                getter ? accessor : existing.IsAccessor ? existing.Getter : null,
                setter ? accessor : existing.IsAccessor ? existing.Setter : null,
                attributes));
    }

    /// <summary>Defines one member under a Symbol key, which a computed member may name.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is a second method rather than a widened one for the same reason the reads are two
    /// walks</b>: the storage is a separate table, a String key and a Symbol key can never collide,
    /// and there is nothing for the two to agree about beyond the flags — which is why the flag
    /// decoding is the only text repeated here.
    /// </para>
    /// <para>
    /// <b>The function's name is the description in brackets</b>, which is what the language says
    /// and what makes <c>C.prototype[Symbol.iterator].name</c> answer
    /// <c>"[Symbol.iterator]"</c>. A Symbol with no description names an empty function, because
    /// there is nothing to put between the brackets.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EB5634
    // Broiler-Human:        PENDING
    private void DefineSymbolMember(JsObject host, JsSymbol key, JsValue member, byte flags)
    {
        if (host.TryGetOwnSymbol(key, out var standing) && !standing.Configurable)
        {
            ThrowTypeError("Cannot redefine property: " + key.Rendered);
        }

        var getter = (flags & JsOpcodes.MemberIsGetter) != 0;
        var setter = (flags & JsOpcodes.MemberIsSetter) != 0;

        var attributes = JsPropertyAttributes.Configurable |
            ((flags & JsOpcodes.MemberIsEnumerable) != 0
                ? JsPropertyAttributes.Enumerable
                : JsPropertyAttributes.None);

        if (member.IsObject && member.AsObject() is JsScriptFunction bodied)
        {
            bodied.HomeObject = host;
            var described = key.Described ? "[" + key.Description + "]" : string.Empty;
            var label = getter ? "get " + described : setter ? "set " + described : described;
            bodied.FunctionName = label;

            bodied.SetOwnProperty(
                "name",
                JsProperty.Data(JsValue.String(label), JsPropertyAttributes.Configurable));
        }

        if (!getter && !setter)
        {
            host.SetOwnSymbol(
                key, JsProperty.Data(member, attributes | JsPropertyAttributes.Writable));

            return;
        }

        host.TryGetOwnSymbol(key, out var existing);
        var accessor = member.AsObjectOrNull();

        host.SetOwnSymbol(
            key,
            JsProperty.Accessor(
                getter ? accessor : existing.IsAccessor ? existing.Getter : null,
                setter ? accessor : existing.IsAccessor ? existing.Setter : null,
                attributes));
    }

    /// <summary>Builds the object graph a class definition is.</summary>
    /// <remarks>
    /// <b>The heritage is validated before anything is built</b>, so a bad <c>extends</c> leaves no
    /// half-made class behind: a superclass that is neither <c>null</c> nor a constructor, or one
    /// whose <c>prototype</c> is a primitive, is a TypeError at the definition rather than a
    /// surprise at the first <c>new</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=42386B
    // Broiler-Human:        PENDING
    private JsValue BuildClass(JsValue constructor, bool derived, JsValue heritage)
    {
        Charge(8);
        var target = (JsScriptFunction)constructor.AsObject();
        JsObject? inherited = Realm.ObjectPrototype;
        JsObject? constructorParent = Realm.FunctionPrototype;

        if (derived && heritage.Type != JsType.Null)
        {
            if (!heritage.IsObject || !heritage.AsObject().IsConstructor)
            {
                return ThrowTypeError(
                    "Class extends value " + Describe(heritage) +
                    " is not a constructor or null");
            }

            var parentPrototype = GetProperty(heritage, "prototype");

            if (!parentPrototype.IsObject && parentPrototype.Type != JsType.Null)
            {
                return ThrowTypeError(
                    "Class extends value does not have valid prototype property " +
                    Describe(parentPrototype));
            }

            inherited = parentPrototype.AsObjectOrNull();
            constructorParent = heritage.AsObject();
        }
        else if (derived)
        {
            // `extends null` GIVES THE PROTOTYPE NO PROTOTYPE and leaves the constructor an
            // ordinary function object. The class is still DERIVED, which is why constructing one
            // fails: its `super()` has `Function.prototype` above it and that is not a constructor.
            inherited = null;
        }

        var prototype = new JsObject(inherited);

        prototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(
                constructor,
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        target.Prototype = constructorParent;
        target.HomeObject = prototype;

        target.SetOwnProperty(
            "prototype",
            JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));

        return constructor;
    }

    /// <summary>Records one class element on the constructor for later application.</summary>
    /// <remarks>
    /// <para>
    /// <b>The home object is given here and not when the element is applied</b>, because it is a
    /// property of the FUNCTION and the function is created once. A field initialiser's
    /// <c>super.x</c> reads through the class prototype for every instance, not through whatever
    /// object the initialiser happened to run against.
    /// </para>
    /// <para>
    /// <b>A second half of an accessor merges rather than appending</b>, and it merges only into an
    /// element of the same name in the same list. <c>get #a</c> and <c>set #a</c> declare one
    /// private name; two records would install two elements under one name and the second would
    /// hide the first.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=23C0ED
    // Broiler-Human:        PENDING
    private void RecordClassElement(
        JsScriptFunction target, JsObject prototype, JsValue key, JsValue body, byte flags)
    {
        Charge(4);
        var isStatic = (flags & JsOpcodes.ElementIsStatic) != 0;

        if (body.IsObject && body.AsObject() is JsScriptFunction bodied)
        {
            bodied.HomeObject = isStatic ? target : prototype;
        }

        var list = isStatic
            ? target.StaticElements ??= []
            : target.InstanceElements ??= [];

        if ((flags & JsOpcodes.ElementIsSetter) != 0)
        {
            foreach (var standing in list)
            {
                if (standing.Key.IsSymbol && key.IsSymbol &&
                    ReferenceEquals(standing.Key.AsSymbol(), key.AsSymbol()))
                {
                    standing.Setter = body;
                    standing.Flags |= JsOpcodes.ElementIsSetter;
                    return;
                }
            }

            list.Add(
                new JsClassElement { Key = key, Body = JsValue.Undefined, Setter = body, Flags = flags });

            return;
        }

        if ((flags & JsOpcodes.ElementIsGetter) != 0)
        {
            foreach (var standing in list)
            {
                if (standing.Key.IsSymbol && key.IsSymbol &&
                    ReferenceEquals(standing.Key.AsSymbol(), key.AsSymbol()))
                {
                    standing.Body = body;
                    standing.Flags |= JsOpcodes.ElementIsGetter;
                    return;
                }
            }
        }

        list.Add(new JsClassElement { Key = key, Body = body, Setter = JsValue.Undefined, Flags = flags });
    }

    /// <summary>Runs the elements a class body recorded against the constructor itself.</summary>
    /// <remarks>
    /// <b>The private methods go on in a pass of their own, before anything runs.</b> A static
    /// field's initialiser and a static block may both call a private static method, and the class
    /// body is entitled to write the method after them - <c>class C { static a = C.#m(); static
    /// #m() { return 1 } }</c> is an ordinary program. One pass in source order would have made
    /// that a <c>TypeError</c> about a method the class does have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3723CE
    // Broiler-Human:        PENDING
    private void RunStaticElements(JsScriptFunction target)
    {
        if (target.StaticElements is not { } elements)
        {
            return;
        }

        ApplyClassElements(target, elements, methods: true);
        ApplyClassElements(target, elements, methods: false);
    }

    /// <summary>Gives one new instance the elements its class recorded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C27CBF
    // Broiler-Human:        PENDING
    private void InitialiseInstanceElements(JsObject instance, JsScriptFunction constructor)
    {
        if (constructor.InstanceElements is not { } elements)
        {
            return;
        }

        ApplyClassElements(instance, elements, methods: true);
        ApplyClassElements(instance, elements, methods: false);
    }

    /// <summary>Applies one pass of a recorded element list to one object.</summary>
    /// <remarks>
    /// <b>A field's initialiser is CALLED and a method is not</b>, which is the whole of what the
    /// method bit decides here. The receiver of the call is the object being given the element, so
    /// <c>class C { x = this.y }</c> reads the instance and <c>class C { static x = this.name }</c>
    /// reads the constructor - one rule, two objects, decided by which list the element was in.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7AB1A4
    // Broiler-Human:        PENDING
    private void ApplyClassElements(
        JsObject target,
        System.Collections.Generic.List<JsClassElement> elements,
        bool methods)
    {
        var receiver = JsValue.Object(target);

        foreach (var element in elements)
        {
            var isMethod = (element.Flags & JsOpcodes.ElementIsMethod) != 0;

            if (isMethod != methods)
            {
                continue;
            }

            Charge(4);

            if (isMethod)
            {
                target.SetPrivate(element.Key.AsSymbol(), PrivateElementOf(element));
                continue;
            }

            if ((element.Flags & JsOpcodes.ElementIsBlock) != 0)
            {
                Call(element.Body, receiver, System.Array.Empty<JsValue>());
                continue;
            }

            var value = element.Body.Type == JsType.Undefined
                ? JsValue.Undefined
                : Call(element.Body, receiver, System.Array.Empty<JsValue>());

            if ((element.Flags & JsOpcodes.ElementIsPrivate) != 0)
            {
                var name = element.Key.AsSymbol();

                // A FIELD DECLARED TWICE ON ONE OBJECT IS A TypeError AND NOT A SECOND WRITE. It is
                // reachable without a duplicate in the source: `class C { #x = 1 }` whose
                // constructor returns an object it has already constructed would install `#x` on it
                // twice, and the language says the second attempt fails.
                if (target.HasPrivate(name))
                {
                    ThrowTypeError(
                        "Cannot initialize " + name.Description + " twice on the same object");
                }

                target.SetPrivate(
                    name, JsProperty.Data(value, JsPropertyAttributes.Writable));

                continue;
            }

            if (element.Key.IsSymbol)
            {
                DefineOwnSymbolDataProperty(target, element.Key.AsSymbol(), value);
                continue;
            }

            // A FIELD IS CreateDataPropertyOrThrow AND NOT AN ASSIGNMENT, which is what makes a
            // field shadow an inherited SETTER of the same name rather than calling it. An
            // assignment would have run the setter and defined nothing.
            DefineOwnDataProperty(target, ToPropertyKey(element.Key), value);
        }
    }

    /// <summary>Defines one own data property, refusing where the object already refuses.</summary>
    /// <remarks>
    /// <b>It is <c>CreateDataPropertyOrThrow</c> and not <c>SetOwnProperty</c>.</b> A field lands
    /// on an instance the class body has never seen frozen, so the refusal is rare - but it is
    /// reachable: a derived constructor may return a frozen object, and a class whose fields then
    /// fail to define must say so rather than produce an instance missing them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=26D8A1
    // Broiler-Human:        PENDING
    private void DefineOwnDataProperty(JsObject target, string key, JsValue value)
    {
        // A PROXY IS ASKED ONLY ITS defineProperty TRAP. CreateDataPropertyOrThrow is one
        // [[DefineOwnProperty]] call, so reading the standing property or the extensibility first
        // would run getOwnPropertyDescriptor and isExtensible traps the language never calls. The
        // proxy's own define refuses with a TypeError when the trap does.
        if (target is JsProxy)
        {
            target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        // A TYPED ARRAY'S NUMERIC KEY IS ITS OWN [[DefineOwnProperty]], which converts the value
        // with the engine - ToNumber, or ToBigInt for a BigInt kind - and refuses a key that names
        // no element. Storing it through SetOwnProperty would convert without the engine, which
        // cannot run a `valueOf` and cannot refuse a Number bound for a BigInt element.
        // (JSeal B07, 2026-09-22.)
        if (target is JsTypedArray && JsTypedArray.IsNumericKey(key))
        {
            var fields = new JsRealm.ObjectDescriptorFields
            {
                HasValue = true,
                Value = value,
                HasWritable = true,
                Writable = true,
                HasEnumerable = true,
                Enumerable = true,
                HasConfigurable = true,
                Configurable = true,
            };

            if (!JsRealm.ObjectDefineOwn(this, target, JsValue.String(key), fields))
            {
                ThrowTypeError("Cannot define property " + key + " on a typed array");
            }

            return;
        }

        if (target.TryGetOwnProperty(key, out var standing)
            ? !standing.Configurable
            : !target.Extensible)
        {
            ThrowTypeError("Cannot define property " + key + ", object is not extensible");
        }

        target.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>
    /// <see cref="DefineOwnDataProperty"/> for a Symbol key, with the same refusal.
    /// </summary>
    /// <remarks>
    /// A class field named by a computed Symbol is <c>CreateDataPropertyOrThrow</c> exactly as a
    /// String-named one is, so an instance a derived constructor froze refuses it the same way
    /// rather than gaining a Symbol-keyed property it could not otherwise be given.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=CEB03F
    // Broiler-Human:        PENDING
    private void DefineOwnSymbolDataProperty(JsObject target, JsSymbol key, JsValue value)
    {
        // A PROXY IS ASKED ONLY ITS defineProperty TRAP, for the reason the String twin gives.
        if (target is JsProxy)
        {
            target.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
            return;
        }

        if (target.TryGetOwnSymbol(key, out var standing)
            ? !standing.Configurable
            : !target.Extensible)
        {
            ThrowTypeError("Cannot define property " + key.Rendered + ", object is not extensible");
        }

        target.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>The private element one recorded method or accessor installs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=65257B
    // Broiler-Human:        PENDING
    private static JsProperty PrivateElementOf(JsClassElement element)
    {
        var accessor = element.Flags & (JsOpcodes.ElementIsGetter | JsOpcodes.ElementIsSetter);

        if (accessor == 0)
        {
            // NOT WRITABLE, which is what makes `this.#m = 1` a TypeError rather than a
            // replacement of the class's own method on one instance.
            return JsProperty.Data(element.Body, JsPropertyAttributes.None);
        }

        return JsProperty.Accessor(
            element.Body.AsObjectOrNull(),
            element.Setter.AsObjectOrNull(),
            JsPropertyAttributes.None);
    }

    /// <summary>Reads one private element, or says why there is none to read.</summary>
    /// <remarks>
    /// <b>The refusal names the private name and not the object</b>, because the object is usually
    /// the answer's subject rather than its cause: a method extracted from a class and called
    /// against something else meets this, and so does a brand check written as a read. Both are
    /// the same fact - this object was not constructed by the class that minted this name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1C20CD
    // Broiler-Human:        PENDING
    private JsValue ReadPrivate(JsValue host, JsSymbol name)
    {
        if (!host.IsObject || !host.AsObject().TryGetPrivate(name, out var element))
        {
            return ThrowTypeError(
                "Cannot read private member " + name.Description +
                " from an object whose class did not declare it");
        }

        if (!element.IsAccessor)
        {
            return element.Value;
        }

        // A WRITE-ONLY PRIVATE ACCESSOR IS A TypeError WHEN READ, and not `undefined`. `set #a` on
        // its own declares a name with no getter, and the language refuses the read rather than
        // answering the absence the way a property with no getter does.
        return element.Getter is null
            ? ThrowTypeError("'" + name.Description + "' was defined without a getter")
            : Call(JsValue.Object(element.Getter), host, System.Array.Empty<JsValue>());
    }

    /// <summary>Writes one private element, or says why it cannot be written.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D2D724
    // Broiler-Human:        PENDING
    private void WritePrivate(JsValue host, JsSymbol name, JsValue value)
    {
        if (!host.IsObject || !host.AsObject().TryGetPrivate(name, out var element))
        {
            ThrowTypeError(
                "Cannot write private member " + name.Description +
                " to an object whose class did not declare it");

            return;
        }

        if (element.IsAccessor)
        {
            if (element.Setter is null)
            {
                ThrowTypeError("'" + name.Description + "' was defined without a setter");
                return;
            }

            Call(JsValue.Object(element.Setter), host, [value]);
            return;
        }

        if (!element.Writable)
        {
            ThrowTypeError("Cannot write to private method " + name.Description);
            return;
        }

        host.AsObject().SetPrivate(name, JsProperty.Data(value, JsPropertyAttributes.Writable));
    }

    /// <summary>Runs a <c>super()</c>: constructs the superclass and binds the result as <c>this</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3A0C9B
    // Broiler-Human:        PENDING
    private JsValue SuperConstruct(
        JsScriptFunction? active, JsCell? binding, JsValue newTarget, JsValue[] arguments)
    {
        if (active is null || binding is null)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a super call reached a frame with no this binding");
        }

        var parent = active.Prototype;

        if (parent is null || !parent.IsConstructor)
        {
            return ThrowTypeError(
                "Super constructor of " + active.FunctionName + " is not a constructor");
        }

        var constructed = Construct(JsValue.Object(parent), arguments, newTarget);

        // CALLING `super()` TWICE IS AN ERROR AND NOT A SECOND BINDING, and the check happens
        // AFTER the superclass has run rather than before. That order is the specification's and
        // it is observable: a second `super()` still constructs the superclass, with whatever the
        // superclass constructor does, and only the attempt to bind the result fails. Checking
        // first would have been the obvious encoding and it makes the superclass's side effects
        // disappear.
        if (!binding.Value.IsEmpty)
        {
            return ThrowReferenceError("Super constructor may only be called once");
        }

        binding.Value = constructed;

        // THE DERIVED CLASS'S OWN FIELDS GO ON HERE AND NOT IN ITS CONSTRUCTOR'S PROLOGUE. The
        // object did not exist until this instant - the BASE constructor made it - so this is the
        // first point at which a derived class has something to install its fields on, and it is
        // also why a field initialiser may read `this` while the constructor's first line may not.
        if (constructed.IsObject)
        {
            InitialiseInstanceElements(constructed.AsObject(), active);
        }

        return constructed;
    }

    /// <summary>The object a <c>super</c> lookup starts from.</summary>
    /// <remarks>
    /// <para>
    /// It is the home object's PROTOTYPE, and the receiver of the lookup is <c>this</c> - the pair
    /// that makes <c>super.m()</c> reach the parent's <c>m</c> and run it against the instance.
    /// Starting at the receiver's prototype instead would find the method itself and recur
    /// forever, which is the defect this design exists to make unrepresentable.
    /// </para>
    /// <para>
    /// <b>A home object with no prototype is a TypeError and not an <c>undefined</c>.</b> The
    /// specification requires the base to be object-coercible before it reads anything through it,
    /// so <c>super.x</c> inside a method of an object whose prototype is <c>null</c> fails the way
    /// <c>null.x</c> fails rather than quietly answering nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=25892B
    // Broiler-Human:        PENDING
    private JsObject SuperBase(JsScriptFunction? active)
    {
        var home = active?.HomeObject;

        if (home is null)
        {
            ThrowTypeError("'super' keyword unexpected here");
        }

        if (home!.Prototype is null)
        {
            ThrowTypeError("Cannot read properties of null (reading a 'super' property)");
        }

        return home.Prototype!;
    }

    /// <summary>Writes a property through the active method's home object.</summary>
    /// <remarks>
    /// The chain the write consults starts above the home object and the write itself lands on
    /// <c>this</c>: an inherited setter runs with <c>this</c> as its receiver, an inherited
    /// non-writable data property refuses the write, and anything else creates or replaces an own
    /// property of the instance rather than touching the prototype it was found on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A8CBB6
    // Broiler-Human:        PENDING
    private void SetSuper(
        JsObject start, JsValue receiver, string key, JsValue value, bool strict)
    {
        var current = start;

        while (current is not null)
        {
            if (current.TryGetOwnProperty(key, out var found))
            {
                if (found.IsAccessor)
                {
                    if (found.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError(
                                "Cannot set property " + key + " which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(found.Setter), receiver, [value]);
                    return;
                }

                if (!found.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to read only property '" + key + "'");
                    }

                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        var instance = receiver.AsObjectOrNull();

        if (instance is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create property '" + key + "' on a primitive");
            }

            return;
        }

        if (instance.TryGetOwnProperty(key, out var own) && !own.IsAccessor)
        {
            if (!own.Writable)
            {
                if (strict)
                {
                    ThrowTypeError("Cannot assign to read only property '" + key + "'");
                }

                return;
            }

            own.Value = value;
            instance.SetOwnProperty(key, own);
            return;
        }

        if (!instance.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add property " + key + ", object is not extensible");
            }

            return;
        }

        instance.SetOwnProperty(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>The <c>in</c> operator's lookup: does any object in the chain have the key.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=856713
    // Broiler-Human:        PENDING
    internal bool HasProperty(JsObject start, string key)
    {
        // THE SAME EXOTIC RULE [[Get]] OBEYS, AND FOR THE SAME REASON. An integer-indexed object
        // does not inherit its indices, so `9 in new Int32Array(3)` is false whatever anybody put
        // on `Object.prototype`. Expressing it here rather than on the object is forced: the walk
        // is the engine's, not the object's.
        if (start is JsTypedArray view && JsTypedArray.IsNumericKey(key))
        {
            return view.TryGetOwnProperty(key, out _);
        }

        // A NAMESPACE ANSWERS THIS FROM ITS EXPORT SET AND NEVER READS THE BINDING. The walk below
        // asks each object for the property, and a namespace answers that by reading through to the
        // module's slot - which throws for an export whose module has not run yet. `'x' in ns` is
        // true for such a name and `ns.x` is a `ReferenceError`; going through the walk would make
        // both the same throw. The Symbol table is still the base's, so `@@toStringTag in ns` is
        // answered below by the ordinary path.
        if (start is JsModuleNamespace names)
        {
            return names.Exports(key);
        }

        var current = start;

        while (current is not null)
        {
            // A PROXY ANSWERS `in` WHOLE. The `has` trap is asked about the chain as well as the
            // object, so a walk that continued past it would ask the target the question the trap
            // has already answered.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyHas(JsValue.String(key));
            }

            // A typed array part-way up the chain answers a numeric key itself, and ends the walk.
            if (current is JsTypedArray passedView && JsTypedArray.IsNumericKey(key))
            {
                return passedView.TryGetOwnProperty(key, out _);
            }

            if (current.TryGetOwnProperty(key, out _))
            {
                return true;
            }

            current = current.Prototype;
        }

        return false;
    }

    // ---- calling -------------------------------------------------------------------------------

    /// <summary>Calls <paramref name="callee"/>, whatever kind of callable it is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=40A67B
    // Broiler-Human:        PENDING
    internal JsValue Call(JsValue callee, JsValue thisValue, JsValue[] arguments)
    {
        if (!callee.IsObject || !callee.AsObject().IsCallable)
        {
            return ThrowTypeError(Describe(callee) + " is not a function");
        }

        Charge(4);

        // THE TWO BACKSTOPS ARE DIFFERENT ANSWERS TO DIFFERENT QUESTIONS, and folding them into one
        // condition - which is what this was - cost the language its own error.
        //
        // The runtime's own stack probe answers "is there room to do ANYTHING here". When it says
        // no there is no safe action left: constructing an error object and dispatching it needs
        // stack the program has already spent, which is the process termination
        // [JSC-85](roadmap.corrections.md#jsc-85) recorded. So that case ends the operation.
        //
        // The counted bound answers a different question - "has this interpreter recursed further
        // than it will promise" - and it is reached with the stack probe still satisfied, so a
        // `RangeError` can be built and thrown. It MUST be thrown rather than aborted, because
        // `Maximum call stack size exceeded` is a catchable exception in every engine and real
        // programs catch it: a recursive descent that probes its own depth, a benchmark that sizes
        // a workload, a conformance case that asserts the error's type. An abort there is a
        // resource exhaustion no guest can see, and the guard the program wrote never runs.
        if (!System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            throw StackBackstopReached();
        }

        if (depth >= MaximumCallDepth && !reportingDepth)
        {
            // THE REPORT NEEDS FRAMES OF ITS OWN, and refusing them is what turned this bound into
            // a process termination *(JSC-85)*. Building the `RangeError` runs `CreateError`, which
            // constructs an object, which re-enters here at a depth already past the bound and
            // throws again — a recursion with no base case, inside the code that exists to refuse
            // one. The flag is the base case. It is cleared as the exception unwinds, so the guest's
            // own `catch` runs with the bound back in force.
            reportingDepth = true;

            try
            {
                ThrowRangeError("Maximum call stack size exceeded");
            }
            finally
            {
                reportingDepth = false;
            }
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;

        try
        {
            switch (callee.AsObject())
            {
                // THE PROXY CASE IS FIRST BECAUSE IT IS NOT A FUNCTION. It has no bytecode and no
                // delegate; what it has is an `apply` trap, or a target to forward to. Reaching it
                // through this switch rather than at the call site is what makes every route into a
                // call - a call expression, `Function.prototype.call`, a comparator handed to
                // `sort`, an iterator's `next` - trap alike.
                case JsProxy proxy:
                    return proxy.ProxyCall(thisValue, arguments);

                case JsNativeFunction native:
                    return native.Call(this, thisValue, arguments);

                case JsBoundFunction bound:
                    return Call(
                        JsValue.Object(bound.Target),
                        bound.BoundThis,
                        Concat(bound.BoundArguments, arguments));

                // A CLASS IS NOT CALLABLE AND THE REFUSAL BELONGS HERE. Every route into a
                // function - a call site, `Function.prototype.call`, a comparison function handed
                // to `sort` - arrives at this switch, and a guard inside the constructor's own
                // code would answer for none of them because the frame is never entered.
                case JsScriptFunction script when script.IsClassConstructor:
                    return ThrowTypeError(
                        "Class constructor " + script.FunctionName +
                        " cannot be invoked without 'new'");

                case JsScriptFunction script:
                    return Invoke(script, thisValue, arguments, JsValue.Undefined, null);

                default:
                    return ThrowTypeError("value is not a function");
            }
        }
        finally
        {
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    /// <summary>Constructs with <paramref name="callee"/>, which is also the <c>new.target</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B91950
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsValue callee, JsValue[] arguments) =>
        Construct(callee, arguments, callee);

    /// <summary>Constructs with <paramref name="callee"/> on behalf of <paramref name="newTarget"/>.</summary>
    /// <remarks>
    /// <para>
    /// <b>The instance is made from <c>new.target</c>'s prototype and not from the callee's</b>,
    /// and the two differ exactly when a derived class calls up: <c>new C()</c> on a three-deep
    /// chain runs <c>A</c>'s constructor with <c>new.target</c> still <c>C</c>, so the object it
    /// creates is a <c>C</c>. Reading the callee's own prototype would have made every instance of
    /// a subclass an instance of its base.
    /// </para>
    /// <para>
    /// <b>A derived constructor is entered with NO instance at all.</b> Its <c>this</c> is
    /// whatever its <c>super()</c> eventually returns, so what is passed down is an empty box the
    /// <c>super()</c> fills; a frame that read it before then gets the <c>ReferenceError</c> the
    /// language promises rather than a half-built object.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=78D681
    // Broiler-Human:        PENDING
    internal JsValue Construct(JsValue callee, JsValue[] arguments, JsValue newTarget)
    {
        if (!callee.IsObject || !callee.AsObject().IsConstructor)
        {
            return ThrowTypeError(Describe(callee) + " is not a constructor");
        }

        Charge(8);
        var target = callee.AsObject();

        // THE `new.target` GOES THROUGH UNCHANGED AND IS NOT REPLACED BY THE PROXY. A `construct`
        // trap is handed whatever the construction named, which is what lets a proxied base class
        // build an instance of a derived one; substituting the proxy would make every such instance
        // an instance of the proxy's own target.
        if (target is JsProxy proxied)
        {
            return proxied.ProxyConstruct(arguments, newTarget);
        }

        if (target is JsNativeFunction native)
        {
            // THE NEW TARGET IS HANDED OVER RATHER THAN DROPPED. Every built-in in the profile
            // ignores the receiver slot on this path and goes on ignoring it; what changes is that
            // a body written by an embedder can now answer `new.target`, which is a question a
            // constructor is entitled to ask and had no way to.
            var made = native.Construct(this, arguments, newTarget);

            // A BUILT-IN REACHED THROUGH `super()` MUST STILL MAKE AN INSTANCE OF THE DERIVED
            // CLASS. `class Failure extends Error { }` is the case that matters: the built-in
            // builds the object, and it builds it against its own prototype because that is all a
            // C# body is given - so without this the instance would be an Error and not a Failure,
            // and `catch (e) { e instanceof Failure }` would be false for an object the program
            // just threw. The re-pointing is skipped when the built-in is what `new` named, which
            // is every ordinary construction, and when the built-in read `new.target` itself
            // (JsNativeFunction.BuildsFromNewTarget), because a second read is a second getter call.
            if (made.IsObject && !native.BuildsFromNewTarget &&
                !ReferenceEquals(newTarget.AsObjectOrNull(), target))
            {
                var wanted = GetProperty(newTarget, "prototype");

                if (wanted.IsObject)
                {
                    made.AsObject().Prototype = wanted.AsObject();
                }
            }

            return made;
        }

        if (target is JsBoundFunction bound)
        {
            // A BOUND FUNCTION'S `new.target` FOLLOWS THROUGH TO ITS TARGET when the bound function
            // is the one being constructed, and is left alone otherwise - which is what makes
            // `new (D.bind(null))()` produce a `D`.
            return Construct(
                JsValue.Object(bound.Target),
                Concat(bound.BoundArguments, arguments),
                ReferenceEquals(newTarget.AsObjectOrNull(), bound)
                    ? JsValue.Object(bound.Target)
                    : newTarget);
        }

        var script = (JsScriptFunction)target;
        var derived = script.IsDerivedConstructor;
        JsObject? instance = null;

        if (!derived)
        {
            var prototype = GetProperty(newTarget, "prototype");

            instance = new JsObject(
                prototype.IsObject ? prototype.AsObject() : Realm.ObjectPrototype);

            // THE FIELDS GO ON BEFORE THE BODY RUNS AND NOT AFTER IT, which is what makes
            // `class C { x = 1; constructor() { this.x += 1 } }` produce a `2`. The specification
            // puts this at the top of a BASE constructor's body evaluation; a derived one gets its
            // own fields when `super()` returns, because until then it has no object to give them
            // to.
            InitialiseInstanceElements(instance, script);
        }

        var binding = new JsCell
        {
            Value = derived ? JsValue.Empty : JsValue.Object(instance!),
        };

        // THE TWO BACKSTOPS ARE DIFFERENT ANSWERS TO DIFFERENT QUESTIONS, and folding them into one
        // condition - which is what this was - cost the language its own error.
        //
        // The runtime's own stack probe answers "is there room to do ANYTHING here". When it says
        // no there is no safe action left: constructing an error object and dispatching it needs
        // stack the program has already spent, which is the process termination
        // [JSC-85](roadmap.corrections.md#jsc-85) recorded. So that case ends the operation.
        //
        // The counted bound answers a different question - "has this interpreter recursed further
        // than it will promise" - and it is reached with the stack probe still satisfied, so a
        // `RangeError` can be built and thrown. It MUST be thrown rather than aborted, because
        // `Maximum call stack size exceeded` is a catchable exception in every engine and real
        // programs catch it: a recursive descent that probes its own depth, a benchmark that sizes
        // a workload, a conformance case that asserts the error's type. An abort there is a
        // resource exhaustion no guest can see, and the guard the program wrote never runs.
        if (!System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            throw StackBackstopReached();
        }

        if (depth >= MaximumCallDepth && !reportingDepth)
        {
            // THE REPORT NEEDS FRAMES OF ITS OWN, and refusing them is what turned this bound into
            // a process termination *(JSC-85)*. Building the `RangeError` runs `CreateError`, which
            // constructs an object, which re-enters here at a depth already past the bound and
            // throws again — a recursion with no base case, inside the code that exists to refuse
            // one. The flag is the base case. It is cleared as the exception unwinds, so the guest's
            // own `catch` runs with the bound back in force.
            reportingDepth = true;

            try
            {
                ThrowRangeError("Maximum call stack size exceeded");
            }
            finally
            {
                reportingDepth = false;
            }
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;

        try
        {
            var returned = Invoke(script, binding.Value, arguments, newTarget, binding);

            // A CONSTRUCTOR THAT RETURNS AN OBJECT RETURNS THAT OBJECT, and one that returns
            // anything else returns the instance. Getting this backwards makes every factory
            // written as a constructor produce the wrong thing.
            if (returned.IsObject)
            {
                return returned;
            }

            if (!derived)
            {
                return JsValue.Object(instance!);
            }

            // A DERIVED CONSTRUCTOR IS HELD TO MORE THAN A BASE ONE. Returning a primitive other
            // than `undefined` is a TypeError rather than being ignored, and falling off the end
            // without having called `super()` is a ReferenceError rather than producing nothing -
            // which is the check that makes the whole temporal dead zone worth having.
            if (returned.Type != JsType.Undefined)
            {
                return ThrowTypeError(
                    "Derived constructors may only return object or undefined");
            }

            if (binding.Value.IsEmpty)
            {
                return ThrowReferenceError(
                    "Must call super constructor in derived class before accessing 'this' or " +
                    "returning from derived constructor");
            }

            return binding.Value;
        }
        finally
        {
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=609082
    // Broiler-Human:        PENDING
    private static JsValue[] Concat(JsValue[] first, JsValue[] second)
    {
        if (first.Length == 0)
        {
            return second;
        }

        var joined = new JsValue[first.Length + second.Length];
        System.Array.Copy(first, joined, first.Length);
        System.Array.Copy(second, 0, joined, first.Length, second.Length);
        return joined;
    }

    /// <summary>Reads a global lexical binding, or refuses because it is in its dead zone.</summary>
    /// <remarks>
    /// <b>The dead zone is a state and the refusal is what makes it observable.</b> A binding that
    /// has been declared and not yet initialised holds nothing, and the language says a read of one
    /// is a <c>ReferenceError</c> naming it - which is the whole difference between a script-level
    /// <c>let</c> and the <c>var</c> whose read before the declaration answers <c>undefined</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=630988
    // Broiler-Human:        PENDING
    private JsValue ReadLexical(string name, JsLexicalBinding binding)
    {
        if (!binding.Initialised)
        {
            ThrowReferenceError("cannot access " + name + " before its declaration");
        }

        return binding.Value;
    }

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

    // ---- the iteration protocol ----------------------------------------------------------------

    /// <summary>
    /// The abstract operation <c>GetIterator</c>: the guest's own protocol, driven from here.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing here shortcuts the protocol for a value this engine happens to recognise.</b>
    /// Spreading an Array calls <c>Array.prototype[Symbol.iterator]</c> and then calls the
    /// <c>next</c> that answered, every time - because a guest may replace either, and a program
    /// that does is entitled to see its own function run. A fast path over the dense elements would
    /// be faster and would answer the wrong thing for exactly the programs that would notice.
    /// </para>
    /// <para>
    /// <b>It reads <c>Symbol.iterator</c> and not a <c>length</c>.</b> An array-like and an
    /// iterable are different things, and a construct that fell back to indices when the Symbol was
    /// absent would iterate a plain object that happened to have a <c>length</c> — which the
    /// language refuses, loudly, and for good reason.
    /// </para>
    /// <para>
    /// <b>One helper for every construct that iterates</b> — spread, <c>for … of</c>, array
    /// destructuring, and any built-in taking an iterable. Each of those is the same three steps
    /// with a different thing done to the value, and writing them separately is how three of them
    /// end up agreeing and the fourth does not.
    /// </para>
    /// <para>
    /// <c>next</c> is read ONCE, here, and the record keeps it. The specification reads it at
    /// <c>GetIterator</c> and calls that same function at every step.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8BAE7A
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetIterator(JsValue iterable)
    {
        Charge(4);

        if (iterable.IsNullish)
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        if (!TryGetSymbolMethod(iterable, Realm.IteratorSymbol, out var method))
        {
            ThrowTypeError(Describe(iterable) + " is not iterable");
        }

        return GetIteratorFromMethod(iterable, method);
    }

    /// <summary>
    /// The specification's <c>GetIteratorFromMethod</c>: an iterator record from a
    /// <c>Symbol.iterator</c> method the caller has already read.
    /// </summary>
    /// <remarks>
    /// <b>It exists for the callers that have to read the method BEFORE deciding to iterate.</b>
    /// A typed array constructor asks <c>GetMethod(argument, @@iterator)</c> to choose between the
    /// iteration protocol and the array-like reading; reading the method a second time through
    /// <see cref="GetIterator"/> would run a getter twice that the language runs once.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D2ED70
    // Broiler-Human:        PENDING
    internal JsIteratorRecord GetIteratorFromMethod(JsValue iterable, JsValue method)
    {
        var iterator = Call(method, iterable, System.Array.Empty<JsValue>());

        if (!iterator.IsObject)
        {
            ThrowTypeError("The result of the iterator method is not an object");
        }

        return new JsIteratorRecord(iterator, GetProperty(iterator, "next"));
    }

    /// <summary>
    /// One step of <c>IteratorStep</c>: answers the next value, or that the iterator is finished.
    /// </summary>
    /// <remarks>
    /// <b>A record whose <c>next</c> threw is marked done before the exception leaves.</b> The
    /// specification does not close an iterator whose <c>next</c> failed - it has no reason to
    /// believe the object is in a state that can answer <c>return</c> - and the flag is what carries
    /// that decision to the <c>IterateClose</c> the lowering emits unconditionally.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=780D55
    // Broiler-Human:        PENDING
    internal bool TryIterateNext(JsIteratorRecord record, out JsValue value) =>
        TryIterateNext(record, System.Array.Empty<JsValue>(), out value, out _);

    /// <summary>
    /// The same step, with the argument <c>next</c> is called with and the value a DONE step
    /// carried.
    /// </summary>
    /// <remarks>
    /// <b>Both extras exist for <c>yield*</c> and for nothing else, which is why they are an
    /// overload rather than a second walk over the protocol.</b> A delegation forwards what
    /// <c>gen.next(v)</c> sent to the inner iterator's own <c>next</c>, and what <c>yield*</c>
    /// EVALUATES TO is the value of the step that reported done - which every other construct
    /// discards, and which a helper that only answered "finished" could not give back. The
    /// argument list is passed through rather than built here so that the ordinary overload calls
    /// <c>next</c> with NO arguments, exactly as <c>IteratorNext</c> does when it has no value to
    /// send: an iterator written in the guest can see the difference in its own
    /// <c>arguments.length</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=91005A
    // Broiler-Human:        PENDING
    internal bool TryIterateNext(
        JsIteratorRecord record,
        JsValue[] sent,
        out JsValue value,
        out JsValue completed,
        bool wantsCompleted = false)
    {
        value = JsValue.Undefined;
        completed = JsValue.Undefined;

        if (record.Done)
        {
            return false;
        }

        Charge(2);
        JsValue result;

        try
        {
            result = Call(record.Next, record.Iterator, sent);
        }
        catch (JsThrow)
        {
            record.Done = true;
            throw;
        }

        if (!result.IsObject)
        {
            record.Done = true;
            ThrowTypeError("Iterator result " + Describe(result) + " is not an object");
        }

        // THE `value` OF A DONE RESULT IS READ ONLY BY A CALLER THAT WANTS IT, which is one: the
        // `yield*` delegation, whose own value is the inner iterator's return value. Reading it for
        // everybody is observable through a getter - the pinned suite's set-like iterators count
        // exactly these reads - and a `for … of` that read it would be asking a question the
        // language does not ask.
        //
        // A THROWING `done` OR `value` GETTER MARKS THE RECORD DONE AS A THROWING `next` DOES.
        // `IteratorComplete` and `IteratorValue` are part of the step, and a step that failed
        // leaves the iterator unclosed: a `for … of` that went on to call `return` would run guest
        // code the language never runs there.
        try
        {
            if (GetProperty(result, "done").ToBooleanValue())
            {
                record.Done = true;

                if (wantsCompleted)
                {
                    completed = GetProperty(result, "value");
                }

                return false;
            }

            value = GetProperty(result, "value");
        }
        catch (JsThrow)
        {
            record.Done = true;
            throw;
        }

        return true;
    }

    /// <summary>
    /// <c>IteratorClose</c> under a normal or a <c>break</c>-shaped completion.
    /// </summary>
    /// <remarks>
    /// Errors from <c>return</c> propagate here, and a <c>return</c> answering a non-object is
    /// itself a <c>TypeError</c> - both of which a guest can observe and both of which a quiet close
    /// would swallow.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=48D728
    // Broiler-Human:        PENDING
    internal void CloseIterator(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;
        var method = GetProperty(record.Iterator, "return");

        if (method.IsNullish)
        {
            return;
        }

        if (!method.IsObject || !method.AsObject().IsCallable)
        {
            ThrowTypeError("The iterator's return is not a function");
        }

        var result = Call(method, record.Iterator, System.Array.Empty<JsValue>());

        if (!result.IsObject)
        {
            ThrowTypeError("The iterator's return answered " + Describe(result) + " and not an object");
        }
    }

    /// <summary>
    /// <c>IteratorClose</c> under a throw completion, which discards whatever <c>return</c> does.
    /// </summary>
    /// <remarks>
    /// <b>The exception already in flight is the one the program is owed.</b> A <c>for … of</c>
    /// body that throws still has to give the iterator its <c>return</c>, but replacing the body's
    /// exception with one the clean-up raised would report the second failure and lose the first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7F0280
    // Broiler-Human:        PENDING
    internal void CloseIteratorQuietly(JsIteratorRecord record)
    {
        if (record.Done)
        {
            return;
        }

        record.Done = true;

        try
        {
            var method = GetProperty(record.Iterator, "return");

            if (method.IsObject && method.AsObject().IsCallable)
            {
                Call(method, record.Iterator, System.Array.Empty<JsValue>());
            }
        }
        catch (JsThrow)
        {
            // Deliberately swallowed: see the remark.
        }
    }

    /// <summary>Drains an iterable into a list, closing nothing because it ran to completion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=F6C82A
    // Broiler-Human:        PENDING
    internal void IterateInto(JsValue iterable, System.Collections.Generic.List<JsValue> into)
    {
        var record = GetIterator(iterable);

        while (TryIterateNext(record, out var element))
        {
            into.Add(element);
        }
    }

    /// <summary>Drains a record that is already open, which is what a rest element takes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=AA5907
    // Broiler-Human:        PENDING
    internal JsArray DrainIterator(JsIteratorRecord record)
    {
        var rest = Realm.NewArray();

        while (TryIterateNext(record, out var element))
        {
            rest.Push(element);
        }

        return rest;
    }

    /// <summary>
    /// The abstract operation <c>CopyDataProperties</c>, which is what object spread is.
    /// </summary>
    /// <remarks>
    /// <b>Own and enumerable, in the order the source yields them, and through the ordinary read
    /// path.</b> Reading through the property path is what makes a getter on the source run once
    /// and contribute its value, which is what the language says and what copying descriptors would
    /// not do. A <c>null</c> or <c>undefined</c> source contributes nothing rather than throwing -
    /// <c>{...null}</c> is an empty object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=ADD0F8
    // Broiler-Human:        PENDING
    internal void CopyDataProperties(JsObject target, JsValue source)
    {
        if (source.IsNullish)
        {
            return;
        }

        var from = ToObject(source);

        foreach (var key in from.OwnPropertyNames())
        {
            Charge(1);

            if (!from.TryGetOwnProperty(key, out var property) || !property.Enumerable)
            {
                continue;
            }

            target.SetOwnProperty(
                key,
                JsProperty.Data(GetProperty(source, key), JsPropertyAttributes.Default));
        }
    }

    /// <summary>Runs a program's entry point and answers what it completed with.</summary>
    /// <remarks>
    /// <b>The frame's referrer is the script's own</b> - the one its artifact placed it at, or none -
    /// unless <paramref name="referrer"/> states another: a program the <c>Function</c> constructor
    /// compiled is dynamic code, and runs as its caller's (JSD-0024 section 20).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D406D6
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
            System.Array.Empty<JsValue>(),
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7465D6
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsProgram, JsModuleInstance[]> graphs =
        new();

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=9CFED6
    // Broiler-Falsified-If: two instances of one module key exist in one realm
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, JsModuleInstance> instanced =
        new(System.StringComparer.Ordinal);

    /// <summary>
    /// The instance this realm holds under <paramref name="key"/>, or <see langword="null"/> when it
    /// has linked no module of that key (JSeal I11-upstream).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A80ADE
    // Broiler-Human:        PENDING
    internal JsModuleInstance? FindModuleInstance(string key) =>
        instanced.TryGetValue(key, out var instance) ? instance : null;

    /// <summary>The instances of one artifact's modules, instantiating them if it has not.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=870296
    // Broiler-Human:        PENDING
    private JsModuleInstance[] Graph(JsProgram program) =>
        graphs.TryGetValue(program, out var instances) ? instances : Instantiate(program);

    /// <summary>
    /// Links and evaluates the graph rooted at one module, and answers what its body completed with.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Three phases, and the order is the specification's rather than a convenience.</b> Every
    /// module's environment is created, then every module's declarations are initialised, then the
    /// bodies are evaluated in the order a depth-first walk from the root leaves them. Collapsing
    /// the second phase into the third is the change that breaks a legal cyclic program: the module
    /// that runs first calls a function of the one that has not, and that function has to already
    /// exist.
    /// </para>
    /// <para>
    /// <b>Linking happens once per realm and evaluation happens once per module.</b> A second
    /// invocation of the root entry point answers what the first completed with rather than running
    /// the graph again, which is what the language says of a module that is already evaluated.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=6350BB
    // Broiler-Falsified-If: a module body runs twice in one realm, or a module body runs before every module's declarations are initialised
    // Broiler-Human:        PENDING
    private JsValue RunModuleGraph(JsProgram program, int root) =>
        Evaluated(program, root).Completion;

    /// <summary>
    /// The realm's template registry: one strings object per tagged-template site, by program and
    /// instruction offset.
    /// </summary>
    /// <remarks>
    /// <b>Weak in the program</b>, so the sites of an evaluated program that nothing can run again
    /// do not keep their strings objects alive; a guest that still holds one keeps its own.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AB410D
    // Broiler-Human:        PENDING
    private readonly System.Runtime.CompilerServices.ConditionalWeakTable<
        JsProgram, System.Collections.Generic.Dictionary<int, JsObject>> templates = new();

    /// <summary>
    /// Answers the template object of the site at <paramref name="site"/>, building it the first
    /// time the site is evaluated.
    /// </summary>
    /// <remarks>
    /// <b>GetTemplateObject as the specification writes it</b>: a cooked Array and a raw Array of
    /// the site's chunks, <c>raw</c> defined on the first with every attribute off, and both frozen.
    /// Nothing here reads a global, so a guest that replaced <c>Object.freeze</c> or
    /// <c>Object.defineProperty</c> sees no call and cannot change the object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CEE75C
    // Broiler-Falsified-If: two evaluations of one site answer different objects, two sites answer one object, or the answer is observably not frozen
    // Broiler-Human:        PENDING
    private JsObject TemplateObject(JsProgram program, int site, JsValue[] stack, int at, int count)
    {
        var registry = templates.GetOrCreateValue(program);

        if (registry.TryGetValue(site, out var existing))
        {
            return existing;
        }

        var cooked = new JsArray(Realm.ArrayPrototype);
        var raw = new JsArray(Realm.ArrayPrototype);

        for (var index = 0; index < count; index++)
        {
            Charge(1);
            cooked.Push(stack[at + index]);
            raw.Push(stack[at + count + index]);
        }

        JsRealm.ObjectSetIntegrity(this, raw, freeze: true);
        cooked.SetOwnProperty("raw", JsProperty.Data(JsValue.Object(raw), JsPropertyAttributes.None));
        JsRealm.ObjectSetIntegrity(this, cooked, freeze: true);
        registry[site] = cooked;
        return cooked;
    }

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

    /// <summary>
    /// Evaluates the graph rooted at one module for the artifact's entry point, and answers its
    /// instance.
    /// </summary>
    /// <remarks>
    /// <b>The entry point has nobody holding its promise, so a failure has to be raised</b>: one
    /// found while the walk is still synchronous - or recorded by an earlier evaluation - is thrown
    /// here, as it always was, and one that arrives later, when an async module of the graph
    /// rejects, is raised from a job of its own, which the host's drain reports like any other job
    /// that threw (JSeal I11-async). Throwing it inside the promise reaction instead would only
    /// have rejected a promise nobody can see.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=CE0B69
    // Broiler-Falsified-If: a module body runs twice in one realm, or the entry graph's failure is reported nowhere
    // Broiler-Human:        PENDING
    private JsModuleInstance Evaluated(JsProgram program, int root)
    {
        var instance = Graph(program)[root];
        var promise = EvaluateModule(instance);

        if (promise.State == JsPromiseState.Rejected)
        {
            throw new JsThrow(promise.Result, Render(promise.Result));
        }

        if (promise.State == JsPromiseState.Pending)
        {
            Realm.ReactOn(this, promise, static (engine, value, threw) =>
            {
                if (threw)
                {
                    engine.EnqueueJob(
                        JsValue.Object(engine.Realm.Native(
                            "",
                            0,
                            (inner, thisValue, arguments) => throw new JsThrow(value, inner.Render(value)))),
                        System.Array.Empty<JsValue>());
                }
            });
        }

        return instance;
    }

    /// <summary>Creates every module of one artifact and initialises its declarations.</summary>
    /// <remarks>
    /// <b>A module the realm already holds under its key is ADOPTED rather than rebuilt.</b> An
    /// artifact answered by the mediator normally carries the module that was asked for and every
    /// module that one requests, and some of those are ordinarily already instanced — the artifact
    /// the importer is written in carried them. Building fresh ones would give the realm two
    /// environments for one module; taking the ones it has is what makes the second artifact a
    /// second VIEW of a graph rather than a second graph.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=6D69E9
    // Broiler-Falsified-If: a module body runs before every module of its artifact has its declarations initialised
    // Broiler-Human:        PENDING
    private JsModuleInstance[] Instantiate(JsProgram program)
    {
        // THE COMPOSITION IS ASKED BEFORE ANYTHING IS REGISTERED. A refused resolution used to be
        // raised after the fresh instances were already in the realm's registry, uninitialised, so
        // a later import of the same key adopted an instance whose declarations had never been put
        // in place. Asking first leaves a refused graph unlinked and invisible, and a later load
        // links it again from the start.
        Confirm(program);

        var instances = new JsModuleInstance[program.Modules.Length];
        var fresh = new bool[instances.Length];

        for (var index = 0; index < instances.Length; index++)
        {
            var record = program.Modules[index];

            if (instanced.TryGetValue(record.Key, out var existing))
            {
                instances[index] = existing;
                continue;
            }

            var slots = (int)program.Functions[(int)record.BodyUnit].ScopeSlots;
            instances[index] = new JsModuleInstance(new JsEnvironment(slots, null), program, index);
            instanced[record.Key] = instances[index];
            fresh[index] = true;
        }

        for (var index = 0; index < instances.Length; index++)
        {
            if (fresh[index])
            {
                instances[index].Namespace =
                    new JsModuleNamespace(program.Modules[index], instances, this);
            }
        }

        graphs[program] = instances;

        for (var index = 0; index < instances.Length; index++)
        {
            if (!fresh[index])
            {
                continue;
            }

            Charge(FuelPerInstruction);

            Execute(
                program,
                (int)program.Modules[index].InitialiserUnit,
                instances[index].Environment,
                JsValue.Undefined,
                System.Array.Empty<JsValue>(),
                null,
                JsValue.Undefined,
                null,
                null,
                program.Modules[index].Key);

            instances[index].State = JsModuleState.Initialised;
        }

        return instances;
    }

    /// <summary>
    /// The specification's <c>Evaluate</c>: evaluates the graph rooted at one module and answers
    /// the promise its component settles when it has finished.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The pinned ES2026 algorithm, step for step</b> (JSeal I11-async): <c>Evaluate</c>,
    /// <c>InnerModuleEvaluation</c>, <c>ExecuteAsyncModule</c>, <c>GatherAvailableAncestors</c> and
    /// <c>AsyncModuleExecutionFulfilled</c>/<c>Rejected</c>, over the instance's own fields. A
    /// module with a top-level <c>await</c> does not hold up the walk: its body is started, the
    /// walk goes on to its siblings, and the modules that depend on it are counted as waiting
    /// (<c>[[PendingAsyncDependencies]]</c>) and run, in the order the walk first reached them,
    /// when the last thing they wait on finishes. The walk this replaces evaluated async siblings
    /// one after another, so a sibling that did not depend on an awaiting module waited for it.
    /// </para>
    /// <para>
    /// <b>A second evaluation of a module that is under way answers the same promise</b>, its
    /// component root's <c>[[TopLevelCapability]]</c>, or a new one that the root's completion
    /// settles; there is no second walk to coordinate with. An evaluation that failed stays failed
    /// (<c>[[EvaluationError]]</c>): every later request answers the identical value, and no body
    /// runs again.
    /// </para>
    /// <para>
    /// <b>Fuel, not the guest, bounds the work</b>: every module the walk visits, every parent it
    /// records and every ancestor a completion gathers is charged; every wait is a promise
    /// reaction, so nothing here blocks.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=BC6F73
    // Broiler-Falsified-If: a module body runs twice in one realm, runs before a module it requests has finished, or a module that does not depend on an awaiting module waits for it
    // Broiler-Human:        PENDING
    private JsPromiseObject EvaluateModule(JsModuleInstance module)
    {
        if (module.State is JsModuleState.EvaluatingAsync or JsModuleState.Evaluated &&
            module.CycleRoot is { } root)
        {
            module = root;
        }

        if (module.TopLevelCapability is { } existing)
        {
            return existing;
        }

        var stack = new System.Collections.Generic.List<JsModuleInstance>();
        var capability = Realm.NewHostPromise(this);
        module.TopLevelCapability = capability;

        bool completed;
        JsValue failure;
        moduleEvaluationDepth++;

        try
        {
            var index = 0;
            completed = InnerModuleEvaluation(module, stack, ref index, out failure);
        }
        finally
        {
            moduleEvaluationDepth--;
        }

        if (!completed)
        {
            // EVERY MODULE STILL ON THE WALK IS FINISHED WITH THE ERROR: the thrower, what depends
            // on it, and the members of its cycle, whose bodies may already have run.
            foreach (var member in stack)
            {
                Charge(FuelPerInstruction);
                member.State = JsModuleState.Evaluated;
                member.EvaluationError = failure;
            }

            Realm.SettleAsyncPromise(this, capability, failure, rejected: true);
        }
        else if (module.State == JsModuleState.Evaluated)
        {
            Realm.SettleAsyncPromise(this, capability, JsValue.Undefined, rejected: false);
        }

        return capability;
    }

    /// <summary>
    /// The specification's <c>InnerModuleEvaluation</c>: visits one module and what it requests,
    /// and runs or starts its body; answers false with what was thrown when the walk must stop.
    /// </summary>
    /// <remarks>
    /// <b>The depth-first indices find each strongly connected component</b>, whose root becomes
    /// every member's <see cref="JsModuleInstance.CycleRoot"/> when the component is complete; a
    /// module that depends on a finished component waits on that root, never on the member.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=91CD77
    // Broiler-Falsified-If: a module body runs before every module it requests has run or started, or a module waiting on async dependencies runs before they finish
    // Broiler-Human:        PENDING
    private bool InnerModuleEvaluation(
        JsModuleInstance module,
        System.Collections.Generic.List<JsModuleInstance> stack,
        ref int index,
        out JsValue failure)
    {
        Charge(FuelPerInstruction);
        failure = JsValue.Undefined;

        if (module.State is JsModuleState.EvaluatingAsync or JsModuleState.Evaluated)
        {
            if (module.EvaluationError is { } error)
            {
                failure = error;
                return false;
            }

            return true;
        }

        if (module.State == JsModuleState.Evaluating)
        {
            return true;
        }

        if (module.State != JsModuleState.Initialised)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "a module was evaluated before it was linked");
        }

        module.State = JsModuleState.Evaluating;
        var moduleIndex = index;
        module.DfsIndex = index;
        module.DfsAncestorIndex = index;
        module.PendingAsyncDependencies = 0;
        index++;
        stack.Add(module);

        var graph = Graph(module.Program);

        foreach (var request in module.Program.Modules[module.Index].Requests)
        {
            var required = graph[request];

            if (!InnerModuleEvaluation(required, stack, ref index, out failure))
            {
                return false;
            }

            if (required.State == JsModuleState.Evaluating)
            {
                module.DfsAncestorIndex = System.Math.Min(
                    module.DfsAncestorIndex, required.DfsAncestorIndex);
            }
            else
            {
                required = required.CycleRoot ?? required;

                if (required.EvaluationError is { } error)
                {
                    failure = error;
                    return false;
                }
            }

            if (required.AsyncEvaluationOrder > 0)
            {
                Charge(FuelPerInstruction);
                module.PendingAsyncDependencies++;
                (required.AsyncParentModules ??= []).Add(module);
            }
        }

        if (module.PendingAsyncDependencies > 0 || module.HasTla)
        {
            module.AsyncEvaluationOrder = ++moduleAsyncEvaluationCount;

            if (module.PendingAsyncDependencies == 0)
            {
                ExecuteAsyncModule(module);
            }
        }
        else if (!ExecuteModule(module, out failure))
        {
            return false;
        }

        if (module.DfsAncestorIndex == moduleIndex)
        {
            // THE COMPONENT IS COMPLETE, and this module is its root: every member still on the
            // stack above it is finished, or awaiting, with it.
            while (true)
            {
                Charge(FuelPerInstruction);
                var member = stack[^1];
                stack.RemoveAt(stack.Count - 1);
                member.State = member.AsyncEvaluationOrder == 0
                    ? JsModuleState.Evaluated
                    : JsModuleState.EvaluatingAsync;
                member.CycleRoot = module;

                if (ReferenceEquals(member, module))
                {
                    break;
                }
            }
        }

        return true;
    }

    /// <summary>Runs one synchronous module body, and answers false with what it threw.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=341144
    // Broiler-Human:        PENDING
    private bool ExecuteModule(JsModuleInstance module, out JsValue failure)
    {
        failure = JsValue.Undefined;

        try
        {
            module.Completion = Execute(
                module.Program,
                (int)module.Program.Modules[module.Index].BodyUnit,
                module.Environment,
                JsValue.Undefined,
                System.Array.Empty<JsValue>(),
                null,
                JsValue.Undefined,
                null,
                null,
                module.Program.Modules[module.Index].Key);

            return true;
        }
        catch (JsThrow thrown)
        {
            failure = thrown.Value;
            return false;
        }
    }

    /// <summary>
    /// The specification's <c>ExecuteAsyncModule</c>: starts a body with a top-level <c>await</c>
    /// and hands its completion to the module's async bookkeeping, from a job.
    /// </summary>
    /// <remarks>
    /// <b>A body that throws before its first <c>await</c> has rejected its promise, and that is
    /// all it has done</b>: the failure reaches the module, and every module waiting on it, when the
    /// rejection's reaction runs, exactly as a later throw does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=44A4F9
    // Broiler-Human:        PENDING
    private void ExecuteAsyncModule(JsModuleInstance module)
    {
        var promise = StartAsyncModule(module.Program, module.Program.Modules[module.Index], module);

        Realm.ReactOn(this, promise, (engine, value, threw) =>
        {
            if (threw)
            {
                engine.AsyncModuleExecutionRejected(module, value);
                return;
            }

            engine.AsyncModuleExecutionFulfilled(module);
        });
    }

    /// <summary>
    /// The specification's <c>GatherAvailableAncestors</c>: collects the modules that were waiting
    /// on <paramref name="module"/> alone, and the synchronous ones above them that this completion
    /// releases too.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=57E12A
    // Broiler-Human:        PENDING
    private void GatherAvailableAncestors(
        JsModuleInstance module,
        System.Collections.Generic.List<JsModuleInstance> released,
        System.Collections.Generic.HashSet<JsModuleInstance> seen)
    {
        if (module.AsyncParentModules is not { } parents)
        {
            return;
        }

        foreach (var parent in parents)
        {
            Charge(FuelPerInstruction);

            if (seen.Contains(parent) || (parent.CycleRoot ?? parent).EvaluationError is not null)
            {
                continue;
            }

            if (parent.State != JsModuleState.EvaluatingAsync || parent.PendingAsyncDependencies <= 0)
            {
                throw new JsAbort(
                    JsAbortKind.InternalDefect, "a module waiting on an async dependency was not awaiting");
            }

            parent.PendingAsyncDependencies--;

            if (parent.PendingAsyncDependencies == 0)
            {
                released.Add(parent);
                seen.Add(parent);

                if (!parent.HasTla)
                {
                    GatherAvailableAncestors(parent, released, seen);
                }
            }
        }
    }

    /// <summary>
    /// The specification's <c>AsyncModuleExecutionFulfilled</c>: finishes a module whose async
    /// evaluation completed, and runs or starts what that releases, in walk order.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=42DB0E
    // Broiler-Falsified-If: the modules one completion releases run in an order other than the one the walk reached them in, or a module runs while something it waits on has not finished
    // Broiler-Human:        PENDING
    private void AsyncModuleExecutionFulfilled(JsModuleInstance module)
    {
        if (module.State == JsModuleState.Evaluated)
        {
            return;
        }

        module.AsyncEvaluationOrder = JsModuleInstance.AsyncEvaluationDone;
        module.State = JsModuleState.Evaluated;

        if (module.TopLevelCapability is { } capability)
        {
            Realm.SettleAsyncPromise(this, capability, JsValue.Undefined, rejected: false);
        }

        var released = new System.Collections.Generic.List<JsModuleInstance>();
        GatherAvailableAncestors(module, released, []);
        module.AsyncParentModules = null;
        released.Sort(static (left, right) => left.AsyncEvaluationOrder.CompareTo(right.AsyncEvaluationOrder));
        moduleEvaluationDepth++;

        try
        {
            foreach (var next in released)
            {
                Charge(FuelPerInstruction);

                if (next.State == JsModuleState.Evaluated)
                {
                    continue;
                }

                if (next.HasTla)
                {
                    ExecuteAsyncModule(next);
                }
                else if (!ExecuteModule(next, out var failure))
                {
                    AsyncModuleExecutionRejected(next, failure);
                }
                else
                {
                    next.AsyncEvaluationOrder = JsModuleInstance.AsyncEvaluationDone;
                    next.State = JsModuleState.Evaluated;
                    next.AsyncParentModules = null;

                    if (next.TopLevelCapability is { } settled)
                    {
                        Realm.SettleAsyncPromise(this, settled, JsValue.Undefined, rejected: false);
                    }
                }
            }
        }
        finally
        {
            moduleEvaluationDepth--;
        }
    }

    /// <summary>
    /// The specification's <c>AsyncModuleExecutionRejected</c>: fails a module whose async
    /// evaluation threw, and every module waiting on it, with the same value.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=140497
    // Broiler-Falsified-If: a module that waits on one whose async evaluation threw later runs, or a module not waiting on it is failed
    // Broiler-Human:        PENDING
    private void AsyncModuleExecutionRejected(JsModuleInstance module, JsValue error)
    {
        Charge(FuelPerInstruction);

        if (module.State == JsModuleState.Evaluated)
        {
            return;
        }

        module.EvaluationError = error;
        module.State = JsModuleState.Evaluated;
        module.AsyncEvaluationOrder = JsModuleInstance.AsyncEvaluationDone;

        if (module.TopLevelCapability is { } capability)
        {
            Realm.SettleAsyncPromise(this, capability, error, rejected: true);
        }

        var parents = module.AsyncParentModules;
        module.AsyncParentModules = null;

        if (parents is not null)
        {
            foreach (var parent in parents)
            {
                AsyncModuleExecutionRejected(parent, error);
            }
        }
    }

    /// <summary>Enters one module body as an async frame and answers the promise it settles.</summary>
    /// <remarks>
    /// The frame needs a function to name the unit it is running, and a module body is nobody's
    /// closure - so one is made over the module's own environment. It is never called and never
    /// reaches the guest; what it carries is the unit index and the scope chain the frame would
    /// otherwise have to be told twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A5152E
    // Broiler-Human:        PENDING
    private JsPromiseObject StartAsyncModule(
        JsProgram program, JsModuleRecord record, JsModuleInstance instance)
    {
        var body = new JsScriptFunction(
            Realm.FunctionPrototype, program, (int)record.BodyUnit, instance.Environment)
        {
            ScriptOrModule = record.Key,
        };

        var frame = new JsFrame(
            program,
            (int)record.BodyUnit,
            instance.Environment,
            JsValue.Undefined,
            System.Array.Empty<JsValue>(),
            body);

        Charge((frame.FrameBytes / 64) + 4);
        var call = new JsAsyncCall(frame, Realm.NewAsyncPromise()) { DiscardsCompletion = true };
        ResumeAsync(call, JsResumeMode.Next, JsValue.Undefined);
        return call.Promise;
    }

    /// <summary>
    /// Puts every module request to the composition, and refuses a graph it resolves differently.
    /// </summary>
    /// <remarks>
    /// <b>The profile never derives a key and this is why it does not have to.</b> The artifact
    /// states what its producer resolved each specifier to; this asks the composition whether that
    /// is its own answer, one request at a time, and a <c>Refused</c> ends the run by name. So a
    /// graph bundled under one host's resolution rules cannot be evaluated under another's, and no
    /// filesystem, URL scheme or search path is known to this component at all.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=4B22CD
    // Broiler-Falsified-If: a module request is honoured without the composition being asked, or a refusal is treated as an answer
    // Broiler-Human:        PENDING
    private void Confirm(JsProgram program)
    {
        if (capabilities is null ||
            capabilities.BindingCount <= JavaScriptProfile.ResolveBindingIndex ||
            !capabilities.IsBound(JavaScriptProfile.ResolveBindingIndex))
        {
            ThrowTypeError("this composition provides no module resolver");
            return;
        }

        foreach (var record in program.Modules)
        {
            for (var index = 0; index < record.Requests.Length; index++)
            {
                Charge(FuelPerInstruction);
                var target = program.Modules[record.Requests[index]];

                // THE FORMAT'S OWN TEXT ENCODING AND NOT THE PLATFORM'S. A module specifier is a
                // JavaScript String, so it may hold an unpaired surrogate that UTF-8 cannot carry -
                // and the platform's encoder answers that by THROWING, which turned a conformance
                // case about a lone surrogate in an export name into a harness crash. The format
                // already defines an encoding for exactly this and the request uses it.
                var request = JsFormat.EncodeText(
                    record.Key + "\0" + record.RequestSpecifiers[index] + "\0" + target.Key);

                if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
                {
                    throw new JsAbort(JsAbortKind.Exhausted, "the host-call allowance is spent");
                }

                var outcome = capabilities.InvokeBytes(
                    JavaScriptProfile.ResolveBindingIndex, new VmBytes(request), out _);

                if (outcome != VmHostCallOutcome.Completed)
                {
                    ThrowTypeError(
                        "this composition does not resolve '" + record.RequestSpecifiers[index] +
                        "' from '" + record.Key + "' to '" + target.Key + "'");
                }
            }
        }
    }

    // ---- the dynamic import ---------------------------------------------------------------------

    /// <summary>
    /// Answers the promise of the module <paramref name="specifier"/> names from
    /// <paramref name="referrer"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A DYNAMIC IMPORT IS TWO ROUTES AND THE ARTIFACT'S OWN MODULES ARE THE FAST ONE.</b> A
    /// specifier the referring module already requested statically names a module this artifact
    /// carries, whose key the composition confirmed at link and whose instance this realm may
    /// already hold — so that call is answered with no host round trip at all, and it is answered
    /// with the SAME instance the static import got, which is the identity the language requires.
    /// Every other specifier is a value nobody resolved before the bytes were written, so it goes
    /// to the mediator: the referrer and the specifier become the request payload, the composition's
    /// provider answers with a verified artifact carrying the module the pair names, and this realm
    /// links and evaluates that artifact's graph beside its own.
    /// </para>
    /// <para>
    /// <b>Neither route compiles anything here, which is the point of having the second one.</b>
    /// <c>import(x + y)</c> is a specifier no compiler saw; answering it means somebody has to turn
    /// a specifier into bytes at run time, and in this profile that somebody is never this
    /// component. It is the same door <c>eval</c> goes through, distinguished by the payload's
    /// first byte — see <see cref="JsFormat.ModuleRequestMark"/> — and it is gated by the same
    /// surface, which is why an artifact containing a dynamic import declares
    /// <c>broiler.javascript.dynamic</c> whether or not any particular call reaches the mediator.
    /// </para>
    /// <para>
    /// <b>Three refusals a reader will meet and must not confuse.</b> A composition that DECLINED
    /// the dynamic surface never gets here: its artifact was refused at verification, as an invalid
    /// artifact the guest never sees. A composition that admits the surface and registers NO
    /// provider gets here and is refused at run time, by a rejection the guest may catch. And a
    /// composition whose provider cannot answer for this specifier is refused at run time too, by a
    /// different rejection naming the specifier. That is the same three-way distinction
    /// <see cref="Evaluate"/> draws for <c>eval</c>, and this joins it rather than inventing a
    /// fourth shape.
    /// </para>
    /// <para>
    /// <b>Nothing here throws.</b> Every failure — a specifier that will not coerce, an attribute
    /// nothing honours, a module nothing can find, a module whose body threw — settles the promise
    /// as a rejection, because a production that answers a promise and sometimes throws is one no
    /// <c>catch</c> can be written against.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=880ABC
    // Broiler-Falsified-If: this throws instead of rejecting, or a specifier reaches bytes without passing through the mediator or the artifact's own records
    // Broiler-Human:        PENDING
    internal JsValue DynamicImport(
        JsProgram program, string referrer, JsValue specifierValue, JsValue optionsValue)
    {
        var promise = Realm.NewAsyncPromise();
        Charge(FuelPerInstruction);

        try
        {
            // THE SPECIFIER IS COERCED BEFORE THE OPTIONS ARE READ, which is the specification's
            // order and is observable: `import({toString(){throw a}}, {get with(){throw b}})`
            // rejects with `a`.
            var specifier = ToStringValue(specifierValue);
            RequireHonourableAttributes(optionsValue);

            if (!TryOwnRequest(program, referrer, specifier, out var found))
            {
                // A SPECIFIER NOBODY RESOLVED BEFORE THE BYTES WERE WRITTEN MAY BE ANSWERED LATER.
                // An embedder that loads its modules asynchronously takes the request here and
                // completes it from a turn of its own; until then the promise is simply pending,
                // and nothing on this path waits for it (JSD-0024 section 15).
                if (hostRealm is { ModuleLoader: not null } seam &&
                    seam.OfferModuleRequest(referrer, specifier, promise))
                {
                    return JsValue.Object(promise);
                }

                found = MediatedModule(referrer, specifier);
            }

            EvaluateInto(found.Program, found.Index, promise, settleWithNamespace: true);
        }
        catch (JsThrow thrown)
        {
            Realm.SettleAsyncPromise(this, promise, thrown.Value, rejected: true);
        }

        return JsValue.Object(promise);
    }

    /// <summary>
    /// Evaluates the graph rooted at one module and settles <paramref name="promise"/> when it is
    /// finished: with the module's namespace for an import, with <c>undefined</c> for a host.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The promise is settled when the graph is finished and not when it is started</b>, from
    /// the reaction to the evaluation's own promise, so every caller has one channel for every
    /// failure and a namespace is never handed out before its module has run.
    /// </para>
    /// <para>
    /// <b>An import links and evaluates from a job</b>, as <c>ContinueDynamicImport</c> does once
    /// its load promise settles (JSeal I11-async): the evaluation never begins inside the body that
    /// asked for it, so an import of a module that is still on the walk waits for it rather than
    /// meeting it half evaluated. A host's evaluation starts at once, unless it too arrives while an
    /// evaluation is under way.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=50B9DF
    // Broiler-Falsified-If: the promise settles before every module of the graph has finished, or a failure escapes as a throw
    // Broiler-Human:        PENDING
    internal void EvaluateInto(
        JsProgram program, int root, JsPromiseObject promise, bool settleWithNamespace)
    {
        if (settleWithNamespace || moduleEvaluationDepth != 0)
        {
            var ready = Realm.NewHostPromise(this);
            Realm.SettleAsyncPromise(this, ready, JsValue.Undefined, rejected: false);

            Realm.ReactOn(
                this,
                ready,
                (engine, value, threw) => engine.LinkAndEvaluate(program, root, promise, settleWithNamespace));

            return;
        }

        LinkAndEvaluate(program, root, promise, settleWithNamespace);
    }

    /// <summary>
    /// Links the graph rooted at one module, evaluates it, and settles <paramref name="promise"/>
    /// from the evaluation's reaction.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=F6359B
    // Broiler-Falsified-If: the promise settles other than through the job queue, or a failure escapes as a throw
    // Broiler-Human:        PENDING
    private void LinkAndEvaluate(
        JsProgram program, int root, JsPromiseObject promise, bool settleWithNamespace)
    {
        try
        {
            var instances = Graph(program);
            var evaluation = EvaluateModule(instances[root]);

            // A HOST'S EVALUATION THAT FINISHED ON THE WALK IS SETTLED AT ONCE, as JSD-0024 section
            // 15 has always settled it; its reactions still run from the queue. An import waits for
            // the evaluation's reaction, which is `ContinueDynamicImport`'s own step.
            if (!settleWithNamespace && evaluation.State != JsPromiseState.Pending)
            {
                var rejected = evaluation.State == JsPromiseState.Rejected;
                Realm.SettleAsyncPromise(
                    this, promise, rejected ? evaluation.Result : JsValue.Undefined, rejected);

                return;
            }

            Realm.ReactOn(
                this,
                evaluation,
                (engine, value, threw) => engine.Realm.SettleAsyncPromise(
                    engine,
                    promise,
                    threw
                        ? value
                        : settleWithNamespace
                            ? JsValue.Object(instances[root].Namespace!)
                            : JsValue.Undefined,
                    threw));
        }
        catch (JsThrow thrown)
        {
            Realm.SettleAsyncPromise(this, promise, thrown.Value, rejected: true);
        }
    }

    /// <summary>
    /// Loads and links the graph a host names, evaluating nothing, and answers its root.
    /// </summary>
    /// <remarks>
    /// <b>The same door a dynamic import goes through, and no other.</b> The request is a module
    /// request to the composition's artifact provider, verified by the core under this operation's
    /// allowance, and answered only by an artifact whose entry is a module graph - so a provider
    /// that compiled the text as a classic script is refused by name. The graph is instantiated
    /// here, so its namespace exists and its declarations are initialised, but no body runs until
    /// a host asks for an evaluation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=85EF5C
    // Broiler-Falsified-If: a module body runs before a host asks for an evaluation, or a module request is answered without the mediator
    // Broiler-Human:        PENDING
    internal (JsProgram Program, int Index) LinkHostModule(string referrer, string specifier)
    {
        var found = MediatedModule(referrer, specifier);
        _ = Graph(found.Program);
        return found;
    }

    /// <summary>
    /// Completes an import an embedder deferred: loads through the mediator now, and settles the
    /// import's promise when the graph is finished.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=DD5620
    // Broiler-Falsified-If: a deferred import settles other than through the job queue, or a failure escapes as a throw
    // Broiler-Human:        PENDING
    internal void CompleteImport(string referrer, string specifier, JsPromiseObject promise)
    {
        try
        {
            var found = MediatedModule(referrer, specifier);
            EvaluateInto(found.Program, found.Index, promise, settleWithNamespace: true);
        }
        catch (JsThrow thrown)
        {
            Realm.SettleAsyncPromise(this, promise, thrown.Value, rejected: true);
        }
    }

    /// <summary>The key the composition resolved one module of one artifact to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=709ABF
    // Broiler-Human:        PENDING
    internal static string ModuleKey(JsProgram program, int index) => program.Modules[index].Key;

    /// <summary>The namespace of one module of one artifact, which linking has already built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6DBC15
    // Broiler-Human:        PENDING
    internal JsValue ModuleNamespace(JsProgram program, int index) =>
        JsValue.Object(Graph(program)[index].Namespace!);

    /// <summary>
    /// Reads a dynamic import's second argument, and declines every attribute it carries.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The shape is checked in full before the attributes are declined</b>, and that order is
    /// what a conformance suite grades: <c>import('./m', null)</c> and
    /// <c>import('./m', {with: 1})</c> and <c>import('./m', {with: {type: 1}})</c> are three
    /// different programs and the language rejects each of them for its own reason, none of which
    /// is "this host has no loader for that type". A host that declined the whole argument on sight
    /// would answer all three the same way and would be right about none.
    /// </para>
    /// <para>
    /// <b>And then every attribute is declined, because no composition of this profile has a loader
    /// for one.</b> An attribute says what KIND of thing the specifier names — a JSON document, a
    /// text file — and answering that is the loader's job; the static form of the same clause is
    /// declined by the front end, where a static import is loaded, and this is the same refusal at
    /// the moment a dynamic import is loaded.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D9090F
    // Broiler-Human:        PENDING
    private void RequireHonourableAttributes(JsValue options)
    {
        if (options.Type == JsType.Undefined)
        {
            return;
        }

        if (!options.IsObject)
        {
            ThrowTypeError("the second argument of import() is an object or undefined");
        }

        var attributes = GetProperty(options, "with");

        if (attributes.Type == JsType.Undefined)
        {
            return;
        }

        if (!attributes.IsObject)
        {
            ThrowTypeError("the `with` property of an import options object is an object");
        }

        var carrier = attributes.AsObject();
        var declined = string.Empty;

        foreach (var key in carrier.OwnPropertyNames())
        {
            Charge(1);

            if (!carrier.TryGetOwnProperty(key, out var property) || !property.Enumerable)
            {
                continue;
            }

            if (!GetProperty(attributes, key).IsString)
            {
                ThrowTypeError("the import attribute `" + key + "` is not a string");
            }

            if (declined.Length == 0)
            {
                declined = key;
            }
        }

        if (declined.Length != 0)
        {
            ThrowTypeError(
                "no composition of this profile can honour the import attribute `" + declined +
                "`, so the module this call names cannot be loaded");
        }
    }

    /// <summary>
    /// Finds the module one specifier names from one referrer in the referring artifact's own
    /// request table, answering false when the referrer never requested it statically.
    /// </summary>
    /// <remarks>
    /// <b>A module this realm already holds is never loaded a second time</b>, and the two routes
    /// meet at that rule rather than each keeping their own answer. This fast path finds it through
    /// the referring module's own request table; the mediator's answer
    /// (<see cref="MediatedModule"/>) is matched against the keys this realm has already instanced,
    /// so an artifact that arrives carrying a module the realm has is a second VIEW of that module
    /// and not a second copy of it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=8970EB
    // Broiler-Falsified-If: one module key is evaluated twice in one realm
    // Broiler-Human:        PENDING
    private bool TryOwnRequest(
        JsProgram program, string referrer, string specifier, out (JsProgram Program, int Index) found)
    {
        foreach (var record in program.Modules)
        {
            if (!string.Equals(record.Key, referrer, System.StringComparison.Ordinal))
            {
                continue;
            }

            for (var index = 0; index < record.RequestSpecifiers.Length; index++)
            {
                Charge(1);

                if (string.Equals(
                    record.RequestSpecifiers[index], specifier, System.StringComparison.Ordinal))
                {
                    found = (program, record.Requests[index]);
                    return true;
                }
            }

            break;
        }

        found = default;
        return false;
    }

    /// <summary>
    /// Asks the composition's artifact provider for the module one specifier names from one
    /// referrer, and answers the verified graph's root.
    /// </summary>
    /// <remarks>
    /// The answer must be a module graph: an artifact whose <c>module</c> entry is a module body.
    /// Anything else - a classic script compiled from the same text, a program with no such entry -
    /// is refused, so a module payload never runs under the script goal.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=006C6A
    // Broiler-Falsified-If: a module request is answered by an artifact whose root is not a module body
    // Broiler-Human:        PENDING
    private (JsProgram Program, int Index) MediatedModule(string referrer, string specifier)
    {
        if (Loader is null)
        {
            ThrowTypeError(
                "this composition registered no artifact provider, so '" + specifier +
                "' cannot be loaded");
        }

        var payload = JsFormat.ModuleRequest(referrer, specifier);
        Charge(1 + (ulong)payload.Length);

        var request = new VmArtifactRequest(
            JavaScriptProfile.Id,
            default,
            default,
            1,
            default,
            cancellation,
            new VmBytes(payload));

        var loaded = Loader!.RequestLoad(in request);

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            // A MODULE THAT WILL NOT PARSE OR WILL NOT LINK IS A `SyntaxError`, AND THE REST IS A
            // `TypeError`, and the two answer different questions. `ProviderRefused` is what the
            // mediator reports when the provider's front end refused the module's SOURCE;
            // `InvalidArtifact` is what the core reports when the graph the provider supplied did
            // not link - a name two star re-exports supply, an import of a name nothing exports, a
            // resolution that walks a cycle. The language calls all of those the module's own
            // syntax, and the suite tests for it directly: a hundred and thirty variants assert
            // `SyntaxError` on the rejection of a dynamic import, and a `TypeError` there failed
            // cases whose subject this host answers correctly. Everything else - no such module, no
            // provider, a surface the composition declined, an allowance spent - is this host's own
            // plumbing rather than the module's text, and keeps the `TypeError` it had.
            var linkage = loaded.Reason == VmReason.ProviderRefused ||
                (loaded.Outcome == VmOutcome.InvalidArtifact &&
                    loaded.Reason is not VmReason.UnsupportedFeatureManifest
                        and not VmReason.UnknownFeature);

            throw Error(
                linkage ? "SyntaxError" : "TypeError",
                "the module '" + specifier + "' could not be loaded from '" + referrer + "': " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram loadedProgram)
        {
            throw Error("TypeError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(loadedProgram);

        if (!loadedProgram.TryFindEntry(ModuleEntryName, out var unit) ||
            loadedProgram.ModuleOfUnit[(int)unit] is var root and < 0)
        {
            ThrowTypeError(
                "the artifact answered for '" + specifier + "' carries no module graph");
        }

        return (loadedProgram, loadedProgram.ModuleOfUnit[(int)unit]);
    }

    /// <summary>The entry point a module artifact names its root module by.</summary>
    /// <remarks>
    /// It is a constant of the FORMAT rather than of the compiler that happens to produce one: a
    /// mediator's answer is bytes this profile did not write, and the name it is read back by has
    /// to be a rule both sides can be held to.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F84B52
    // Broiler-Human:        PENDING
    private const string ModuleEntryName = "module";

    /// <summary>Enters one bytecode function's frame.</summary>
    /// <param name="function">The closure being entered.</param>
    /// <param name="thisValue">The receiver the call site supplied.</param>
    /// <param name="arguments">The actual arguments.</param>
    /// <param name="newTarget">
    /// The constructor a <c>new</c> named, or <c>undefined</c> for an ordinary call.
    /// </param>
    /// <param name="binding">
    /// The box a construction holds its <c>this</c> in, or <see langword="null"/> for a call.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B03C11
    // Broiler-Human:        PENDING
    private JsValue Invoke(
        JsScriptFunction function,
        JsValue thisValue,
        JsValue[] arguments,
        JsValue newTarget,
        JsCell? binding)
    {
        var program = function.Program;
        var unit = program.Functions[function.Unit];
        var environment = new JsEnvironment((int)unit.ScopeSlots, function.Environment);

        // A UNIT THAT BINDS ITS OWN PARAMETERS GETS NO COPY AT ALL, and the slots stay EMPTY. That
        // is not an optimisation: a default that reads a later parameter has to find a binding in
        // its temporal dead zone, and filling the slots with `undefined` here would turn that
        // ReferenceError into a silent `undefined`. For a simple parameter list `ParameterCount` is
        // both the arity and the copy count and this is the whole of parameter binding.
        if (!unit.BindsParameters)
        {
            var count = System.Math.Min(arguments.Length, (int)unit.ParameterCount);

            for (var at = 0; at < count; at++)
            {
                environment.Slots[at] = arguments[at];
            }

            for (var at = count; at < unit.ParameterCount; at++)
            {
                environment.Slots[at] = JsValue.Undefined;
            }
        }

        var receiver = unit.IsArrow
            ? function.LexicalThis
            : unit.IsStrict
                ? thisValue
                : thisValue.IsNullish
                    ? JsValue.Object(Realm.GlobalObject)
                    : thisValue.IsObject
                        ? thisValue
                        : JsValue.Object(ToObject(thisValue));

        // CALLING AN ASYNC GENERATOR FUNCTION RUNS NONE OF ITS BODY EITHER, and it is tested BEFORE
        // the two arms below because it carries both of their bits. What it answers is an async
        // generator object rather than a promise: a call of one starts nothing, and the first
        // `next` is what puts the body on the interpreter's stack - which is the generator's half
        // of the pair. Everything asynchronous about it is on the other side of that `next`.
        //
        // ITS PARAMETER LIST IS BOUND AT THE CALL LIKE A PLAIN GENERATOR'S AND NOT LIKE AN ASYNC
        // FUNCTION'S. `EvaluateAsyncGeneratorBody` performs `FunctionDeclarationInstantiation` with
        // a `?` and then creates the object, so a default that throws throws SYNCHRONOUSLY and
        // there is no promise for it to reject - which is the opposite of the async arm below,
        // where the promise is made first and a failing default settles it *(corrected: JSC-159)*.
        if (unit.IsGenerator && unit.IsAsync)
        {
            var body = new JsFrame(program, function.Unit, environment, receiver, arguments, function);

            Charge((body.FrameBytes / 64) + 4);
            BindParameters(program, unit, function, body, receiver, arguments);
            return JsValue.Object(Realm.CreateAsyncGenerator(function, body));
        }

        // CALLING A GENERATOR FUNCTION RUNS NONE OF ITS BODY. The environment above is built and
        // the parameters are bound - both are observable, and both happen at the call - and then
        // the frame is put on the heap and handed back inside a generator object instead of being
        // interpreted. One bit test is what an ordinary call pays for that.
        if (unit.IsGenerator)
        {
            var frame = new JsFrame(program, function.Unit, environment, receiver, arguments, function);

            // THE FRAME IS CHARGED IN PROPORTION TO ITS SIZE, which is the rule this engine already
            // applies to a built-in whose work is proportional to an argument: one instruction may
            // not buy unbounded work. A generator over a unit verified to need a deep operand stack
            // costs more to build than one over a shallow unit, and a program that builds a million
            // of them has spent a million times that.
            Charge((frame.FrameBytes / 64) + 4);
            BindParameters(program, unit, function, frame, receiver, arguments);
            return JsValue.Object(Realm.CreateGenerator(function, frame));
        }

        // CALLING AN ASYNC FUNCTION RUNS ITS BODY, HERE, NOW, ON THIS NATIVE STACK. That is the
        // whole difference from the arm above and a program can see it on its first line:
        // `async function f(){ print(1); await 0; } f(); print(2)` prints 1 before 2, because the
        // body runs to its first `await` before the call returns. What the call returns is the
        // promise, which is already made and still pending unless the body finished without
        // awaiting at all.
        if (unit.IsAsync)
        {
            var frame = new JsFrame(program, function.Unit, environment, receiver, arguments, function)
            {
                // AN ASYNC ARROW TAKES ITS `new.target` AND ITS `this` BOX FROM WHERE IT WAS
                // WRITTEN, exactly as the ordinary path below does - but it has to take them onto
                // the FRAME, because the job that resumes this frame knows nothing about the call
                // site and cannot supply them a second time.
                NewTarget = unit.IsArrow ? function.LexicalNewTarget : newTarget,
                ThisBinding = unit.IsArrow ? function.LexicalThisBinding : binding,
            };

            Charge((frame.FrameBytes / 64) + 4);
            var call = new JsAsyncCall(frame, Realm.NewAsyncPromise());
            ResumeAsync(call, JsResumeMode.Next, JsValue.Undefined);
            return JsValue.Object(call.Promise);
        }

        // AN ARROW TAKES ALL THREE FROM WHERE IT WAS WRITTEN. It has no `this`, no `new.target`
        // and no `super` of its own, so what the call site supplies for any of them is discarded
        // here rather than being allowed to reach the frame.
        return Execute(
            program,
            function.Unit,
            environment,
            receiver,
            arguments,
            function,
            unit.IsArrow ? function.LexicalNewTarget : newTarget,
            unit.IsArrow ? function.LexicalThisBinding : binding,
            null);
    }

    /// <summary>
    /// Runs a generator's parameter-binding prologue, at the call, and leaves the frame at the
    /// first instruction of its body.
    /// </summary>
    /// <param name="program">The program the unit lives in.</param>
    /// <param name="unit">The unit being entered.</param>
    /// <param name="function">The closure the call named.</param>
    /// <param name="frame">The heap frame the generator object will hold.</param>
    /// <param name="receiver">The receiver the body will see as <c>this</c>.</param>
    /// <param name="arguments">The actual arguments the prologue binds.</param>
    /// <remarks>
    /// <para>
    /// <b>Binding a parameter list is not part of a generator's body and never was, and the
    /// difference is visible on the first line of a program.</b> The specification runs
    /// <c>FunctionDeclarationInstantiation</c> before it creates the generator object, so
    /// <c>function* g([x = boom()]) {}; g([undefined])</c> throws where it is written - there is no
    /// generator to resume and no <c>next</c> for the failure to wait for. This engine ran the
    /// binding as the first instructions of the body instead, which for a generator meant the first
    /// <c>next</c>, and every destructuring parameter of every generator template in the
    /// conformance suite reported its error one call too late *(corrected: JSC-159)*.
    /// </para>
    /// <para>
    /// <b>Only a unit that BINDS its own parameters pays for this, and that is the same bit the
    /// frame already tests.</b> A simple parameter list is copied into slots by the caller above
    /// with no code at all, so there is nothing here to run and the entry is skipped: a generator
    /// over a simple list costs exactly what it cost before.
    /// </para>
    /// <para>
    /// <b>It is the ORDINARY dispatch loop over the ordinary frame, stopped by an instruction.</b>
    /// The alternative was a second interpreter, or a code unit of its own for the prologue - which
    /// would have needed a second set of slots, a second operand stack and a second set of
    /// exception regions to hold the same scope. What this needs instead is one flag on the frame
    /// and one seam in the stream, and the verifier checks the seam exactly as it checks a
    /// <c>yield</c>.
    /// </para>
    /// <para>
    /// <b>A prologue that ends anywhere but at the seam is an artifact this checkout cannot
    /// produce</b>, and it is refused rather than handed on: the frame would otherwise reach the
    /// generator object pointing at a suspension it has not started, which the first resumption
    /// would re-enter at an operand height the abstract pass never computed for it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E1EDFC
    // Broiler-Falsified-If: a generator over a non-simple parameter list reports a binding failure at its first resumption rather than at its call
    // Broiler-Human:        PENDING
    private void BindParameters(
        JsProgram program,
        JsCodeUnit unit,
        JsScriptFunction function,
        JsFrame frame,
        JsValue receiver,
        JsValue[] arguments)
    {
        if (!unit.BindsParameters)
        {
            return;
        }

        frame.BindingParameters = true;

        // A GENERATOR BODY IS NEITHER AN ARROW NOR A CONSTRUCTOR - the verifier refuses both
        // pairings - so the two the resumption paths pass as literals are passed as literals here.
        _ = Execute(
            program,
            frame.UnitIndex,
            null,
            receiver,
            arguments,
            function,
            JsValue.Undefined,
            null,
            frame);

        if (frame.BindingParameters)
        {
            frame.BindingParameters = false;

            throw new JsAbort(
                JsAbortKind.InternalDefect,
                "a parameter-binding prologue did not reach the body it belongs to");
        }

        // THE FRAME IS NOT SUSPENDED IN THE SENSE THE DRIVERS MEAN. It stopped, and the state it
        // stopped in is `suspendedStart`: nothing has been yielded, `Started` is still false, and
        // the first resumption is the first entry into the body rather than a re-entry into an
        // instruction. Clearing the flag here keeps the two resumption paths reading it as they
        // already do.
        frame.Suspended = false;
    }

    // ---- generators ----------------------------------------------------------------------------

    /// <summary>
    /// The one entry every resumption goes through: <c>next</c>, <c>return</c> and <c>throw</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The state is decided before the frame is touched.</b> Four states and three methods make
    /// twelve cases, and eleven of them answer without running a single instruction: a completed
    /// generator answers or rethrows, a generator that has not started swallows a <c>return</c> and
    /// rethrows a <c>throw</c> without ever entering its body, and one that is already on the
    /// interpreter's stack is a <c>TypeError</c>. The twelfth is the resumption.
    /// </para>
    /// <para>
    /// <b>A resumption is charged like a call, because it IS one - a second interpreter frame on
    /// the same native stack.</b> Fuel covers the re-entry, so driving a generator a million steps
    /// cannot buy a million frame switches for nothing; and the CALL-DEPTH dimension covers the
    /// frame, which is what makes a <c>yield*</c> chain thousands deep end in a named exhaustion
    /// rather than in a stack overflow. It is charged here and not left to the <c>next</c> call
    /// that reached this method, because that call's frame returns as soon as the generator
    /// suspends and this one does not: a delegation chain holds one of each per level, so counting
    /// only the call would say a chain is half as deep as it is - and the measured difference is
    /// the difference between an answer and a terminated process.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=984439
    // Broiler-Falsified-If: a generator resumed while its own body is running re-enters that body, or a completed generator runs any instruction
    // Broiler-Human:        PENDING
    internal JsValue ResumeGenerator(JsValue receiver, JsResumeMode mode, JsValue sent, string method)
    {
        if (receiver.AsObjectOrNull() is not JsGenerator generator)
        {
            return ThrowTypeError(
                "Generator.prototype." + method + " called on a value that is not a generator");
        }

        if (generator.State == JsGeneratorState.Executing)
        {
            return ThrowTypeError("Generator is already running");
        }

        if (generator.State == JsGeneratorState.Completed || generator.Frame is null)
        {
            return mode switch
            {
                JsResumeMode.Throw => throw new JsThrow(sent, Render(sent)),
                JsResumeMode.Return => JsValue.Object(Realm.IteratorResult(sent, done: true)),
                _ => JsValue.Object(Realm.IteratorResult(JsValue.Undefined, done: true)),
            };
        }

        // A GENERATOR THAT HAS NOT STARTED HAS NO `try` TO RUN, so an abrupt resumption completes
        // it where it stands. Resuming into the body first and then unwinding would run the
        // parameter bindings' side effects a second time, which nothing in the language asks for.
        if (generator.State == JsGeneratorState.SuspendedStart && mode != JsResumeMode.Next)
        {
            CompleteGenerator(generator);

            return mode == JsResumeMode.Throw
                ? throw new JsThrow(sent, Render(sent))
                : JsValue.Object(Realm.IteratorResult(sent, done: true));
        }

        Charge(4);

        // THE DEPTH IS TAKEN BEFORE THE STATE MOVES, so a resumption refused for depth leaves the
        // generator suspended and resumable rather than half-entered. A generator that could not
        // be resumed because the stack was full has not run any of its body, and completing it
        // would be a stronger claim than what happened.
        if (depth >= MaximumCallDepth ||
            !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            return ThrowRangeError("Maximum call stack size exceeded");
        }

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;
        var frame = generator.Frame;
        frame.ResumeMode = mode;
        frame.ResumeValue = sent;
        frame.Suspended = false;
        generator.State = JsGeneratorState.Executing;

        try
        {
            var completed = Execute(
                frame.Program,
                frame.UnitIndex,
                null,
                frame.ThisValue,
                frame.Arguments,
                frame.Function,

                // A GENERATOR BODY IS NEITHER AN ARROW NOR A CONSTRUCTOR - the verifier refuses
                // both pairings - so its `new.target` is `undefined` and it has no `this` box to
                // read through. Passing the two explicitly rather than threading them through the
                // frame keeps the suspended state to what an instruction boundary actually needs.
                JsValue.Undefined,
                null,
                frame);

            if (frame.Suspended)
            {
                generator.State = JsGeneratorState.SuspendedYield;
                frame.Started = true;
                return JsValue.Object(Realm.IteratorResult(completed, done: false));
            }

            CompleteGenerator(generator);
            return JsValue.Object(Realm.IteratorResult(completed, done: true));
        }
        catch (JsReturnSignal forced)
        {
            // THE RETURN THE `finally` BLOCKS DID NOT OVERRIDE. It reaches here having run every
            // enclosing finaliser on the way out, which is the whole reason it travels as an
            // exception rather than as a returned flag.
            return JsValue.Object(Realm.IteratorResult(forced.Value, done: true));
        }
        finally
        {
            // ANY OTHER WAY OUT OF THE BODY COMPLETES THE GENERATOR - a throw the body did not
            // catch, an allowance spent mid-instruction, a stack the runtime could not grow. The
            // test is on the STATE rather than on the exception type, because a catch clause per
            // type is a list that a new type is added to by forgetting: the one that got away
            // would leave a generator reading `already running` for the rest of the program, and
            // every later resumption of it would be a TypeError with no cause a reader could find.
            if (generator.State == JsGeneratorState.Executing)
            {
                CompleteGenerator(generator);
            }

            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }
    }

    /// <summary>
    /// Retires a generator: no frame, no state to resume, and the operand stack let go of.
    /// </summary>
    /// <remarks>
    /// <b>Dropping the frame reference is the point and not the tidiness.</b> A completed generator
    /// object may stay reachable for the rest of the program - somebody kept the variable - and
    /// without this it would keep its operand-stack array, its scope chain and everything those
    /// reach alive with it. Clearing the field is what makes exhausting a generator release what it
    /// was holding, at the instant it is exhausted rather than at the collector's convenience.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5D1433
    // Broiler-Human:        PENDING
    private static void CompleteGenerator(JsGenerator generator)
    {
        generator.Frame = null;
        generator.State = JsGeneratorState.Completed;
    }

    // ---- async functions -----------------------------------------------------------------------

    /// <summary>
    /// Runs one async call's body until it suspends, returns or throws, and settles its promise
    /// when it does either of the last two.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the WHOLE driver, and it is called from exactly two places</b>: the call that
    /// started the function, and the promise reaction an <c>await</c> registered. There is no
    /// third, which is what makes "the body runs at the call and then only on a job" a property a
    /// reader can check by looking at the callers rather than by tracing the queue.
    /// </para>
    /// <para>
    /// <b>A resumption is charged like a call, because it IS one.</b> Fuel covers the re-entry so
    /// that a program awaiting in a loop cannot buy frame switches for nothing, and the CALL-DEPTH
    /// dimension covers the frame so that an async function awaiting another async function
    /// thousands deep ends in a named exhaustion rather than a stack overflow. Both are the
    /// argument <see cref="ResumeGenerator"/> already makes, and it applies here more strongly: a
    /// job that resumes a frame which awaits again enqueues another job, so a program that awaits
    /// for ever spends its allowance on a queue that never empties and the drain ends naming
    /// <c>Fuel</c> — never as a hang.
    /// </para>
    /// <para>
    /// <b>The exhaustion path settles nothing, deliberately.</b> When the allowance is spent
    /// mid-body the abort travels out to the host as a contract violation, and rejecting the
    /// promise on the way would be manufacturing a guest-visible outcome for an operation the host
    /// is being told did not complete. The frame is dropped, so nothing is retained by a call that
    /// can never run again.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=04C185
    // Broiler-Falsified-If: an async call whose body is already on the interpreter's stack is resumed again, or a program that awaits without end is a hang rather than an exhaustion
    // Broiler-Human:        PENDING
    private void ResumeAsync(JsAsyncCall call, JsResumeMode mode, JsValue carried)
    {
        // A CALL THAT HAS NO FRAME HAS ALREADY SETTLED, and a second resumption of it is silently
        // dropped rather than answered. It is reachable only through a thenable whose `then` calls
        // its callback twice, which the promise machinery's own latch already stops - this is the
        // second lock on the same door, because the cost of being wrong here is two interpreters
        // walking one operand stack rather than a diagnosable error.
        if (call.Frame is null || call.Running)
        {
            return;
        }

        // A RESUMPTION REFUSED FOR DEPTH REJECTS, WHERE A GENERATOR'S WOULD MERELY REFUSE. Nothing
        // will ever come back to ask again: the reaction that carried this resumption has already
        // run, so a call left suspended here is a promise pending for ever, which is the hang the
        // whole metering model exists to prevent. It is taken before any state moves, so the
        // rejection is the only thing that happened.
        if (depth >= MaximumCallDepth ||
            !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
        {
            call.Frame = null;
            Realm.SettleAsyncPromise(
                this,
                call.Promise,
                Error("RangeError", "Maximum call stack size exceeded").Value,
                rejected: true);

            return;
        }

        Charge(4);

        if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
        {
            throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
        }

        depth++;
        call.Running = true;
        var body = call.Frame;
        body.ResumeMode = mode;
        body.ResumeValue = carried;
        body.Suspended = false;

        // WHAT THE BODY DID, DECIDED INSIDE THE FRAME AND ACTED ON OUTSIDE IT. Settling a promise
        // and registering an await both run guest code - a reaction handler, a `then` getter - and
        // running that while this frame still counts against the call-depth dimension would charge
        // a continuation for a frame that has already finished. Recording the outcome in two locals
        // and acting after the `finally` is what keeps the accounting honest, and it is also what
        // stops a thenable that resumes synchronously from meeting its own frame still marked
        // running.
        var outcome = JsValue.Undefined;
        var settled = false;
        var rejected = false;

        try
        {
            var completed = Execute(
                body.Program,
                body.UnitIndex,
                null,
                body.ThisValue,
                body.Arguments,
                body.Function,
                body.NewTarget,
                body.ThisBinding,
                body);

            outcome = completed;

            if (body.Suspended)
            {
                body.Started = true;
            }
            else
            {
                settled = true;
                call.Frame = null;
            }
        }
        catch (JsThrow thrown)
        {
            // THE BODY'S OWN `catch` AND `finally` HAVE ALREADY RUN by the time this is reached:
            // an abrupt resumption is raised at the suspension point inside the dispatch loop's
            // try, so the unit's exception regions saw it first. What arrives here is what the body
            // did not handle, and rejecting with it is the whole of `async` error propagation.
            outcome = thrown.Value;
            settled = true;
            rejected = true;
            call.Frame = null;
        }
        catch
        {
            // AN ALLOWANCE SPENT MID-BODY SETTLES NOTHING, deliberately. The abort travels out to
            // the host as a contract violation, and manufacturing a rejection on the way would be
            // giving the guest an outcome for an operation the host is being told did not complete.
            // The frame is dropped, so a call that can never run again retains nothing.
            call.Frame = null;
            throw;
        }
        finally
        {
            call.Running = false;
            depth--;
            meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
        }

        if (settled)
        {
            Realm.SettleAsyncPromise(
                this,
                call.Promise,
                call.DiscardsCompletion && !rejected ? JsValue.Undefined : outcome,
                rejected);

            return;
        }

        // THE AWAIT IS PERFORMED HERE AND NOT IN THE OPCODE, for the reason the `Yield` case hands
        // its value out rather than acting on it: the opcode's whole job is to leave the frame in a
        // resumable state, and everything that depends on WHO resumes belongs to the driver.
        Realm.AwaitOn(
            this,
            outcome,
            (engine, value, threw) => engine.ResumeAsync(
                call, threw ? JsResumeMode.Throw : JsResumeMode.Next, value));
    }

    // ---- async generators ----------------------------------------------------------------------
    //
    // THE THIRD DRIVER, AND IT IS NOT THE OTHER TWO STACKED. A generator is pulled by its caller and
    // answers at once; an async function is pushed by the job queue and answers a promise nobody
    // asked twice for. An async generator is pulled AND answers later, which is a combination
    // neither of the two above has a place to put: between the pull and the answer, another pull can
    // arrive. The queue below is where those go, and everything else in this section exists to keep
    // it answered in order.

    /// <summary>
    /// The one entry every <c>next</c>, <c>return</c> and <c>throw</c> on an async generator goes
    /// through: it answers a promise, and it may start nothing at all.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every path answers a PROMISE, including the paths that are errors.</b> A receiver that is
    /// not an async generator is a rejected promise where the synchronous family throws, because
    /// the method's whole contract is that it answers something with a <c>then</c> - and a caller
    /// writing <c>gen.next().catch(f)</c> would otherwise also need a <c>try</c> around the call.
    /// </para>
    /// <para>
    /// <b>The three methods differ in what a generator that is not suspended does with them, and
    /// that is the only place they differ.</b> <c>next</c> on a completed generator answers a done
    /// step without queueing anything; <c>throw</c> on one that has not started completes it and
    /// rejects, running none of its body, for the reason the synchronous family does not enter a
    /// body that has no <c>try</c> in it yet; <c>return</c> queues even on a completed generator,
    /// because its value is AWAITED before it is answered and the awaiting has to happen somewhere.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=CB6E30
    // Broiler-Falsified-If: a call of `next`, `return` or `throw` on an async generator answers anything but a promise, or two calls made before the first settles are answered out of order
    // Broiler-Human:        PENDING
    internal JsValue EnqueueAsyncGenerator(
        JsValue receiver, JsResumeMode mode, JsValue sent, string method)
    {
        if (receiver.AsObjectOrNull() is not JsAsyncGenerator generator)
        {
            return Realm.RejectedPromise(
                this,
                "AsyncGenerator.prototype." + method +
                " called on a value that is not an async generator");
        }

        Charge(4);
        var promise = Realm.NewAsyncPromise();

        // A `throw` INTO A GENERATOR THAT HAS NOT STARTED COMPLETES IT WHERE IT STANDS. There is no
        // `try` in a body none of whose instructions have run, so entering it to unwind would run
        // the parameter bindings' side effects a second time for nothing.
        if (mode == JsResumeMode.Throw && generator.State == JsAsyncGeneratorState.SuspendedStart)
        {
            generator.Frame = null;
            generator.State = JsAsyncGeneratorState.Completed;
        }

        var state = generator.State;

        if (mode == JsResumeMode.Next && state == JsAsyncGeneratorState.Completed)
        {
            return Realm.FulfilledPromise(
                this, JsValue.Object(Realm.IteratorResult(JsValue.Undefined, done: true)));
        }

        if (mode == JsResumeMode.Throw && state == JsAsyncGeneratorState.Completed)
        {
            return Realm.RejectWith(this, sent);
        }

        generator.Queue.Add(new JsAsyncGeneratorRequest(mode, sent, promise));

        switch (state)
        {
            case JsAsyncGeneratorState.SuspendedStart when mode == JsResumeMode.Return:
            case JsAsyncGeneratorState.Completed:

                // THE BODY IS NOT ENTERED AND THE VALUE IS STILL AWAITED. `agen().return(p)` where
                // `p` is a promise answers `{ value: <what p resolved to>, done: true }`, which is
                // the one thing a `return` does that a `next` on the same generator does not - and
                // it is why this request is queued rather than answered here.
                generator.Frame = null;
                generator.State = JsAsyncGeneratorState.DrainingQueue;
                AwaitAsyncGeneratorReturn(generator);
                break;

            case JsAsyncGeneratorState.SuspendedStart:
                generator.State = JsAsyncGeneratorState.Executing;
                ResumeAsyncGenerator(generator, mode, sent);
                break;

            case JsAsyncGeneratorState.SuspendedYield:
                ResumeAsyncGeneratorAtYield(generator, mode, sent);
                break;

            default:

                // EXECUTING OR DRAINING: the request is in the queue and something already running
                // will reach it. Doing anything else here is what would re-enter a running frame.
                break;
        }

        return JsValue.Object(promise);
    }

    /// <summary>
    /// Resumes a generator suspended at a <c>yield</c>, awaiting a <c>return</c>'s value first.
    /// </summary>
    /// <remarks>
    /// <b>It is <c>AsyncGeneratorUnwrapYieldResumption</c>, and the await it performs is invisible
    /// in the source.</b> <c>gen.return(p)</c> while the body sits at a <c>yield</c> waits for
    /// <c>p</c> before the body's own <c>finally</c> blocks run, so a finaliser that observes the
    /// world sees it after <c>p</c> settled rather than before. A <c>p</c> that REJECTS turns the
    /// return into a throw at the same suspension point, which is why the callback below chooses
    /// between two modes rather than always raising a return.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A0B79D
    // Broiler-Human:        PENDING
    private void ResumeAsyncGeneratorAtYield(
        JsAsyncGenerator generator, JsResumeMode mode, JsValue sent)
    {
        generator.State = JsAsyncGeneratorState.Executing;

        if (mode != JsResumeMode.Return)
        {
            ResumeAsyncGenerator(generator, mode, sent);
            return;
        }

        Realm.AwaitOn(
            this,
            sent,
            (engine, value, threw) => engine.ResumeAsyncGenerator(
                generator, threw ? JsResumeMode.Throw : JsResumeMode.Return, value));
    }

    /// <summary>
    /// Runs one async generator's body until it yields, awaits, returns or throws, and answers the
    /// requests it settles on the way.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a LOOP and not a recursion, and the difference is a process termination.</b> A
    /// <c>yield</c> answers the request at the front of the queue and then looks at the queue
    /// again: if another request is already waiting the specification says execution continues
    /// WITHOUT suspending, so the body is resumed straight away. Writing that as a call back into
    /// this method cost one native frame per queued request, and a program that calls <c>next</c> a
    /// hundred thousand times before awaiting any of the promises would have ended in a stack
    /// overflow rather than in an answer - on a stack that holds a few thousand ordinary calls.
    /// </para>
    /// <para>
    /// <b>The two suspensions are told apart by the frame and never by the value.</b>
    /// <see cref="JsFrame.Suspension"/> is written by the instruction that left the loop, so an
    /// <c>await</c> whose operand happens to be an iteration step and a <c>yield</c> whose operand
    /// happens to be a promise are not confusable. Guessing from the value is the defect this field
    /// exists to make unwritable.
    /// </para>
    /// <para>
    /// <b>A resumption is charged like a call, because it IS one</b>, exactly as
    /// <see cref="ResumeGenerator"/> and <see cref="ResumeAsync"/> argue: fuel for the re-entry so
    /// that driving a generator cannot buy frame switches for nothing, and the call-depth dimension
    /// for the frame so that an async generator awaiting another one thousands deep ends in a named
    /// exhaustion rather than a stack overflow.
    /// </para>
    /// <para>
    /// <b>A depth refusal REJECTS where a synchronous generator's would merely refuse</b>, for the
    /// reason <see cref="ResumeAsync"/> gives: nothing will come back to ask again, so a request
    /// left unanswered is a promise pending for ever.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=30F939
    // Broiler-Falsified-If: an async generator whose body is on the interpreter's stack is resumed again, or an `await` inside an async generator body settles a request the way a `yield` does
    // Broiler-Human:        PENDING
    private void ResumeAsyncGenerator(
        JsAsyncGenerator generator, JsResumeMode mode, JsValue carried)
    {
        while (true)
        {
            if (generator.Frame is null || generator.Running)
            {
                return;
            }

            if (depth >= MaximumCallDepth ||
                !System.Runtime.CompilerServices.RuntimeHelpers.TryEnsureSufficientExecutionStack())
            {
                generator.Frame = null;
                generator.State = JsAsyncGeneratorState.DrainingQueue;

                Realm.CompleteAsyncGeneratorStep(
                    this,
                    generator,
                    Error("RangeError", "Maximum call stack size exceeded").Value,
                    done: true,
                    rejected: true);

                DrainAsyncGeneratorQueue(generator);
                return;
            }

            Charge(4);

            if (!meter.TryCharge(VmBudgetDimension.CallDepth, 1))
            {
                throw new JsAbort(JsAbortKind.Exhausted, "the call-depth ceiling was reached");
            }

            depth++;
            generator.Running = true;
            var body = generator.Frame;
            body.ResumeMode = mode;
            body.ResumeValue = carried;
            body.Suspended = false;
            body.Suspension = JsSuspension.None;

            // WHAT THE BODY DID, DECIDED INSIDE THE FRAME AND ACTED ON OUTSIDE IT, for the reason
            // `ResumeAsync` records: settling a promise runs guest code, and running it while this
            // frame still counts against the call-depth dimension would charge a continuation for a
            // frame that has already finished.
            var outcome = JsValue.Undefined;
            var suspension = JsSuspension.None;
            var finished = false;
            var rejected = false;

            try
            {
                outcome = Execute(
                    body.Program,
                    body.UnitIndex,
                    null,
                    body.ThisValue,
                    body.Arguments,
                    body.Function,

                    // AN ASYNC GENERATOR BODY IS NEITHER AN ARROW NOR A CONSTRUCTOR - the verifier
                    // refuses `Generator | Arrow` and `Async | Constructible` - so its `new.target`
                    // is `undefined` and it has no `this` box to read through.
                    JsValue.Undefined,
                    null,
                    body);

                if (body.Suspended)
                {
                    body.Started = true;
                    suspension = body.Suspension;
                }
                else
                {
                    finished = true;
                    generator.Frame = null;
                }
            }
            catch (JsReturnSignal forced)
            {
                // THE RETURN THE `finally` BLOCKS DID NOT OVERRIDE, arriving here having run every
                // enclosing finaliser - and it completes the generator NORMALLY, with the value it
                // carries. `return` is not an error, and rejecting for one would turn
                // `gen.return(1)` into a rejection every consumer would have to catch.
                outcome = forced.Value;
                finished = true;
                generator.Frame = null;
            }
            catch (JsThrow thrown)
            {
                outcome = thrown.Value;
                finished = true;
                rejected = true;
                generator.Frame = null;
            }
            catch
            {
                // AN ALLOWANCE SPENT MID-BODY SETTLES NOTHING, deliberately, exactly as it does for
                // an async call: the abort travels out to the host as a contract violation, and
                // manufacturing a rejection on the way would hand the guest an outcome for an
                // operation the host is being told did not complete.
                generator.Frame = null;
                throw;
            }
            finally
            {
                generator.Running = false;
                depth--;
                meter.ReportReleased(VmBudgetDimension.CallDepth, 1);
            }

            if (finished)
            {
                generator.State = JsAsyncGeneratorState.DrainingQueue;
                Realm.CompleteAsyncGeneratorStep(this, generator, outcome, true, rejected);
                DrainAsyncGeneratorQueue(generator);
                return;
            }

            if (suspension == JsSuspension.Await)
            {
                // THE STATE STAYS `Executing` ACROSS THE AWAIT, and that is what makes a `next`
                // arriving mid-await a queued request rather than a second entry into the body.
                Realm.AwaitOn(
                    this,
                    outcome,
                    (engine, value, threw) => engine.ResumeAsyncGenerator(
                        generator, threw ? JsResumeMode.Throw : JsResumeMode.Next, value));

                return;
            }

            Realm.CompleteAsyncGeneratorStep(this, generator, outcome, false, false);

            if (generator.Queue.Count == 0)
            {
                generator.State = JsAsyncGeneratorState.SuspendedYield;
                return;
            }

            var waiting = generator.Queue[0];

            // A QUEUED `return` IS THE ONE CONTINUATION THAT CANNOT STAY IN THIS LOOP, because its
            // value has to be awaited before the body is re-entered. Everything else carries on
            // round, which is what "execution continues without suspending" means.
            if (waiting.Mode == JsResumeMode.Return)
            {
                ResumeAsyncGeneratorAtYield(generator, waiting.Mode, waiting.Value);
                return;
            }

            mode = waiting.Mode;
            carried = waiting.Value;
        }
    }

    /// <summary>
    /// Answers the requests an async generator's body will never reach, oldest first.
    /// </summary>
    /// <remarks>
    /// <b>It stops at the first <c>return</c> rather than answering it here</b>, because a
    /// <c>return</c>'s value is awaited and the rest of the queue has to wait behind it: answering
    /// the requests after it first would deliver them out of order. What resumes the drain is the
    /// job <see cref="AwaitAsyncGeneratorReturn"/> registers.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1D9150
    // Broiler-Human:        PENDING
    private void DrainAsyncGeneratorQueue(JsAsyncGenerator generator)
    {
        while (generator.Queue.Count != 0)
        {
            var request = generator.Queue[0];

            if (request.Mode == JsResumeMode.Return)
            {
                AwaitAsyncGeneratorReturn(generator);
                return;
            }

            // A `next` AFTER THE BODY FINISHED IS A DONE STEP CARRYING `undefined`, and NOT the
            // value the body completed with: that value was the answer to the request that was at
            // the front when the body finished, and handing it to every later `next` would repeat
            // a return value the language returns once.
            Realm.CompleteAsyncGeneratorStep(
                this,
                generator,
                request.Mode == JsResumeMode.Throw ? request.Value : JsValue.Undefined,
                done: true,
                rejected: request.Mode == JsResumeMode.Throw);
        }

        generator.State = JsAsyncGeneratorState.Completed;
    }

    /// <summary>Awaits the value a queued <c>return</c> carries, then answers it and drains on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C28516
    // Broiler-Human:        PENDING
    private void AwaitAsyncGeneratorReturn(JsAsyncGenerator generator)
    {
        if (generator.Queue.Count == 0)
        {
            generator.State = JsAsyncGeneratorState.Completed;
            return;
        }

        var request = generator.Queue[0];

        try
        {
            Realm.AwaitOn(
                this,
                request.Value,
                (engine, value, threw) =>
                {
                    engine.Realm.CompleteAsyncGeneratorStep(engine, generator, value, true, threw);
                    engine.DrainAsyncGeneratorQueue(generator);
                });
        }
        catch (JsThrow thrown)
        {
            // RESOLVING THE VALUE READ GUEST CODE AND IT THREW - a `constructor` getter on a
            // thenable is enough. The request is answered with that failure rather than left
            // pending, and the drain goes on: a queue that stopped here would be a set of promises
            // nothing could ever settle.
            Realm.CompleteAsyncGeneratorStep(this, generator, thrown.Value, true, rejected: true);
            DrainAsyncGeneratorQueue(generator);
        }
    }

    // ---- the loop ------------------------------------------------------------------------------

    /// <summary>
    /// Runs one activation of a code unit in whichever form this instance's programs carry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>EVERY ENTRY INTO A CODE UNIT COMES THROUGH HERE, AND THE FORM IS ASSERTED ON EACH ONE.</b>
    /// The eight drivers - an entry, a module body, an ordinary call, the parameter prologue and the
    /// three resumptions - call this method and nothing below it, so a program of the other form can
    /// reach neither the interpreter nor emitted code without passing the one comparison. The
    /// comparison is the third of three: instantiation and every nested load already refused a
    /// mismatch, and an answer here means one of those two was bypassed, which is a defect in this
    /// profile rather than anything a guest did.
    /// </para>
    /// <para>
    /// <b>It is inlined into its callers and the interpreter pays one test for it.</b> The bytecode
    /// arm is the interpreter's own loop instantiated over <see cref="JsInterpreted"/>, whose every
    /// native-only branch the importer removes; the native arm is a call that is never inlined.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=28C3E3
    // Broiler-Falsified-If: a program whose form differs from the engine's reaches ExecuteCore or emitted code
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private JsValue Execute(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame,
        string? referrer = null)
    {
        if ((program.NativeCode.Length != 0) != nativeForm)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a program of the other output form reached this engine");
        }

        // THE FRAME'S SCRIPT OR MODULE, SET FOR ITS WHOLE LIFE AND RESTORED WHEN IT ENDS, however it
        // ends (JSD-0024 section 20): the one a caller states, else the function's own.
        var outerReferrer = activeReferrer;
        activeReferrer = referrer ?? self?.ScriptOrModule ?? string.Empty;

        try
        {
            return nativeForm
                ? RunNative(
                    program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
                    thisBinding, frame)
                : ExecuteCore<JsInterpreted>(
                    program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
                    thisBinding, frame, null);
        }
        finally
        {
            activeReferrer = outerReferrer;
        }
    }

    /// <summary>
    /// The dispatch loop, over an ordinary frame or over a generator's heap-allocated one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A <see langword="null"/> <paramref name="frame"/> is the ordinary path and it is
    /// unchanged.</b> The operand stack, the scope chain, the height and the instruction pointer
    /// are locals exactly as they were; the frame is read once, at entry, and never looked at again
    /// unless the unit actually suspends. What an ordinary call now pays is that one test and the
    /// argument that carries it.
    /// </para>
    /// <para>
    /// <b>An abrupt resumption is raised at the top of the try, not at the suspension point.</b>
    /// <c>gen.throw</c> and <c>gen.return</c> re-enter at the instruction the frame suspended at
    /// and must be seen by whatever exception region encloses it - so the raise happens inside the
    /// same try the dispatch loop runs in, with <c>current</c> already set to that instruction. The
    /// existing region search then runs the same <c>catch</c> and <c>finally</c> blocks it would
    /// have run for a throw from the instruction itself, and no unwinding is reimplemented.
    /// </para>
    /// <para>
    /// <b>ONE BODY, SPECIALISED OVER A MODE, AND THE MODE ONLY DECIDES HOW MUCH OF IT RUNS.</b>
    /// <see cref="JsInterpreted"/> is the loop as it always was: every test of the mode below is a
    /// comparison of two type tokens the importer folds, so that instantiation carries none of them.
    /// <see cref="JsNativeEntry"/> runs the prologue - including an abrupt resumption's raise and
    /// landing - and stops at the first instruction without charging for it. A per-opcode mode runs
    /// exactly one charged instruction and stops at the next; <see cref="JsStepBlock"/> runs charged
    /// instructions until <see cref="JsBaselineBlocks.StopsAfter"/> holds, which is what one call from
    /// emitted code into the handler table asks for. Stopping is the one statement at the top of the
    /// inner loop, which every way back to an instruction passes through: a <c>break</c> out of an
    /// arm, a caught throw and a caught forced return all land there. So a step runs the same arm
    /// text, the same charge, the same filter and the same landing as the interpreter, because it is
    /// the interpreter; the block mode only moves the boundary at which it hands back to emitted code.
    /// </para>
    /// <para>
    /// <b>A step reads its state from the activation and writes it back only at the boundary.</b>
    /// The operand stack and the scope list are the activation's own objects, shared exactly as a
    /// generator's frame shares them today, so an arm mutates them in place; the height and the
    /// instruction pointer are integers and are handed back when the step stops.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=31159E
    // Broiler-Falsified-If: an instantiation over a per-opcode step mode runs more or fewer than one charged instruction per call, the block instantiation stops anywhere but at the first boundary after its first instruction at which JsBaselineBlocks.StopsAfter holds, or the interpreted instantiation behaves differently from the loop before it was made generic
    // Broiler-Human:        PENDING
    internal JsValue ExecuteCore<TMode>(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame,
        JsNativeActivation? act)
        where TMode : struct, IJsExecutionMode
    {
        var unit = program.Functions[unitIndex];
        var code = program.Code;
        var constants = program.Constants;
        var names = program.Names;
        JsValue[] stack;
        System.Collections.Generic.List<JsEnvironment> scopes;
        int sp;
        int pc;
        var abrupt = false;

        if (typeof(TMode) == typeof(JsInterpreted) || typeof(TMode) == typeof(JsNativeEntry))
        {
            stack = frame is null ? new JsValue[unit.MaxOperandStack + 1] : frame.Stack;

            scopes = frame is null
                ? new System.Collections.Generic.List<JsEnvironment>(4) { environment! }
                : frame.Scopes;

            sp = frame is null ? 0 : frame.Sp;
            pc = frame is null ? (int)unit.CodeOffset : frame.Pc;

            // A DELEGATION RESUMES INSIDE ITS OWN OPCODE, whatever mode it resumes in: `return` and
            // `throw` arriving mid-`yield*` are forwarded to the inner iterator rather than raised
            // here, so only a plain `yield` reaches either of the two arms below.
            abrupt = frame is { Started: true, Delegating: false } &&
                frame.ResumeMode != JsResumeMode.Next;

            // THE NORMAL RESUMPTION IS FINISHED HERE AND NOT IN THE OPCODE. The instruction that
            // suspended has already run its pop; what re-entry owes it is the push of the sent value
            // and the step past it, and doing that here keeps the `Yield` case a straight-line
            // suspend.
            //
            // THE STEP IS THE WIDTH OF THE INSTRUCTION ACTUALLY AT THE POINTER, not of `Yield`. The
            // two suspensions this arm serves - `Yield` and `Await` - happen to be one byte each
            // today, so naming one of them worked; it would have gone on working right up until a
            // suspension with an operand was added, and then it would have resumed one byte into an
            // instruction rather than after it. Reading the byte costs nothing and cannot be wrong.
            if (frame is { Started: true, Delegating: false } && !abrupt)
            {
                stack[sp++] = frame.ResumeValue;
                pc += JsOpcodes.InstructionWidth((JsOpcode)code[pc]);
            }

            // THE ENTRY HANDS THE ACTIVATION THE OBJECTS IT BUILT OR BORROWED, so every step after
            // it mutates the same stack and the same scope list the interpreter would have.
            if (typeof(TMode) == typeof(JsNativeEntry))
            {
                act!.Stack = stack;
                act.Scopes = scopes;
            }
        }
        else
        {
            stack = act!.Stack;
            scopes = act.Scopes;
            sp = act.Sp;
            pc = act.Pc;
        }

        var strict = unit.IsStrict;
        var current = pc;
        JsRegion region = default;

        // THE FUNCTION `super` BELONGS TO IS NOT ALWAYS THE ONE RUNNING. An arrow has no `super`
        // of its own and reaches the enclosing method's, so both halves of `super` - the home
        // object a property starts from and the superclass a call constructs - are read from here
        // rather than from `self`.
        var active = self is null ? null : unit.IsArrow ? self.LexicalActiveFunction : self;

        // AN ENTRY RUNS NO INSTRUCTION, A PER-OPCODE STEP RUNS ONE, AND A BLOCK STEP RUNS UNTIL THE
        // PARTITION STOPS IT. The flag starts set for the entry, so the first boundary it reaches
        // stops it; a step clears the boundary once and runs its first instruction, so the stop rule
        // is first asked about an instruction that has run, never about the value `current` starts
        // with.
        var stepped = typeof(TMode) == typeof(JsNativeEntry);

        while (true)
        {
            try
            {
                if (abrupt)
                {
                    abrupt = false;
                    current = pc;
                    var carried = frame!.ResumeValue;

                    if (frame.ResumeMode == JsResumeMode.Throw)
                    {
                        throw new JsThrow(carried, Render(carried));
                    }

                    throw new JsReturnSignal(carried);
                }

                while (true)
                {
                    // THE STEP BOUNDARY. Every route back to an instruction reaches this line - an
                    // arm's `break`, a landed throw and a landed forced return - so a step stops here
                    // with the pointer the interpreter would run next: an entry at once, a per-opcode
                    // step after its one instruction, and a block step where the baseline partition
                    // says its block ends, which is the function the emitted unit's tails are laid out
                    // from.
                    if (typeof(TMode) != typeof(JsInterpreted))
                    {
                        // THE UNIT'S END IS READ WHERE IT IS ASKED FOR, AND BY A BLOCK STEP ALONE.
                        // The mode comparison in front of it folds, so no other instantiation carries
                        // the read; holding it in a local instead would put it, and the two field
                        // reads behind it, in every instantiation's prologue, including the
                        // interpreter's.
                        if (stepped &&
                            (typeof(TMode) != typeof(JsStepBlock) ||
                                JsBaselineBlocks.StopsAfter(
                                    code, current, pc, (int)(unit.CodeOffset + unit.CodeLength))))
                        {
                            act!.Sp = sp;
                            act.Pc = pc;
                            return default;
                        }

                        stepped = true;
                    }

                    current = pc;
                    Charge(FuelPerInstruction);

                    var opcode = typeof(TMode) == typeof(JsInterpreted) || typeof(TMode) == typeof(JsStepBlock)
                        ? (JsOpcode)code[pc]
                        : TMode.Opcode;

                    switch (opcode)
                    {
                        case JsOpcode.Nop:
                            pc++;
                            break;

                        case JsOpcode.LoadUndefined:
                            stack[sp++] = JsValue.Undefined;
                            pc++;
                            break;

                        case JsOpcode.LoadNull:
                            stack[sp++] = JsValue.Null;
                            pc++;
                            break;

                        case JsOpcode.LoadTrue:
                            stack[sp++] = JsValue.True;
                            pc++;
                            break;

                        case JsOpcode.LoadFalse:
                            stack[sp++] = JsValue.False;
                            pc++;
                            break;

                        case JsOpcode.LoadConstant:
                            stack[sp++] = constants[U16(code, pc)];
                            pc += 3;
                            break;

                        case JsOpcode.LoadThis:
                            // A FRAME WITH A BINDING READS THE BINDING AND NOT THE VALUE IT WAS
                            // ENTERED WITH, because a derived constructor's `this` arrives part
                            // way through the frame and an arrow inside one has to see it when it
                            // does. Every other frame has no binding and reads what it was given.
                            stack[sp++] = thisBinding is null
                                ? thisValue
                                : ThisBinding(thisBinding);

                            pc++;
                            break;

                        case JsOpcode.NewArguments:
                            // A UNIT THAT DOES NOT BIND ITS OWN PARAMETERS HAS A SIMPLE LIST, and
                            // its parameters are slots `0..ParameterCount-1` of the record this
                            // frame entered with - which is what the frame's copy loop filled. A
                            // sloppy one of those is the one unit whose `arguments` is MAPPED onto
                            // them; every other unit gets the unmapped object *(corrected: JSeal
                            // V05)*.
                            stack[sp++] = JsValue.Object(
                                Realm.CreateArguments(
                                    actualArguments,
                                    self,
                                    strict,
                                    strict || unit.BindsParameters ? null : scopes[0],
                                    (int)unit.ParameterCount));

                            pc++;
                            break;

                        case JsOpcode.LoadNewTarget:

                            // `default` IS `undefined` HERE AND THAT IS DELIBERATE. An ordinary
                            // call passes nothing, and the value kind a `JsValue` defaults to is
                            // the uninitialised-binding marker, which no expression may produce -
                            // so the read normalises it rather than letting the marker escape onto
                            // the operand stack.
                            stack[sp++] = newTarget.IsEmpty ? JsValue.Undefined : newTarget;
                            pc++;
                            break;

                        case JsOpcode.LoadArgument:
                        {
                            var at = U16(code, pc);

                            stack[sp++] = at < actualArguments.Length
                                ? actualArguments[at]
                                : JsValue.Undefined;

                            pc += 3;
                            break;
                        }

                        case JsOpcode.RestArguments:
                        {
                            var from = U16(code, pc);
                            var rest = Realm.NewArray();

                            for (var at = from; at < actualArguments.Length; at++)
                            {
                                Charge(1);
                                rest.Push(actualArguments[at]);
                            }

                            stack[sp++] = JsValue.Object(rest);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.LoadScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped read named no slot");
                            }

                            if (slot.Slots[U16(code, pc + 1)].IsEmpty)
                            {
                                ThrowReferenceError("Cannot access a binding before initialisation");
                            }

                            stack[sp++] = slot.Slots[U16(code, pc + 1)];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.StoreScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped write named no slot");
                            }

                            var index = U16(code, pc + 1);

                            if (slot.Slots[index].IsEmpty)
                            {
                                ThrowReferenceError("Cannot access a binding before initialisation");
                            }

                            slot.Slots[index] = stack[--sp];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.InitialiseScoped:
                        {
                            var slot = Slot(scopes, code[pc + 1], U16(code, pc + 1), out var found);

                            if (!found)
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect, "a scoped initialiser named no slot");
                            }

                            slot.Slots[U16(code, pc + 1)] = stack[--sp];
                            pc += 4;
                            break;
                        }

                        case JsOpcode.LoadGlobal:
                        {
                            var name = names[U16(code, pc)];

                            // THE DECLARATIVE HALF IS ASKED FIRST, which is the order the global
                            // environment record has and the reason `let Array = 1` at script level
                            // shadows the intrinsic rather than replacing it.
                            if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
                            {
                                stack[sp++] = ReadLexical(name, bound);
                                pc += 3;
                                break;
                            }

                            if (!HasProperty(Realm.GlobalObject, name))
                            {
                                ThrowReferenceError(name + " is not defined");
                            }

                            stack[sp++] = GetProperty(JsValue.Object(Realm.GlobalObject), name);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.LoadGlobalOrUndefined:
                        {
                            var name = names[U16(code, pc)];

                            // `typeof x` ANSWERS FOR A NAME NOBODY DECLARED AND THROWS FOR ONE IN
                            // ITS DEAD ZONE, which is the one place the two halves answer
                            // differently for the same shape of question: the property half has no
                            // dead zone to be in, and this half does.
                            if (Realm.HasLexicals && Realm.TryLexical(name, out var bound))
                            {
                                stack[sp++] = ReadLexical(name, bound);
                                pc += 3;
                                break;
                            }

                            stack[sp++] = HasProperty(Realm.GlobalObject, name)
                                ? GetProperty(JsValue.Object(Realm.GlobalObject), name)
                                : JsValue.Undefined;

                            pc += 3;
                            break;
                        }

                        case JsOpcode.StoreGlobal:
                        {
                            var target = names[U16(code, pc)];

                            if (Realm.HasLexicals && Realm.TryLexical(target, out var bound))
                            {
                                var written = stack[--sp];

                                if (!bound.Initialised)
                                {
                                    ThrowReferenceError(
                                        "cannot access " + target + " before its declaration");
                                }

                                // AN ASSIGNMENT TO A `const` IS A `TypeError` WHEREVER IT IS
                                // WRITTEN, and the check is on the binding rather than on the
                                // store: a function closed over the name, a later script, and a
                                // `with` body that did not shadow it all arrive here.
                                if (!bound.Mutable)
                                {
                                    throw Error(
                                        "TypeError", "assignment to constant variable " + target);
                                }

                                bound.Value = written;
                                pc += 3;
                                break;
                            }

                            // STRICT CODE MAY NOT CREATE A GLOBAL BY ASSIGNING TO A NAME NOBODY
                            // DECLARED, and that is the whole of what `"use strict"` buys a reader
                            // of an unfamiliar program. Sloppy code creates the property, which is
                            // what the arm below does; strict code gets the `ReferenceError` the
                            // language gives, because the alternative — a silent global — is the
                            // defect strict mode exists to make impossible.
                            if (strict && !HasProperty(Realm.GlobalObject, target))
                            {
                                sp--;
                                ThrowReferenceError(target + " is not defined");
                            }

                            SetProperty(
                                JsValue.Object(Realm.GlobalObject),
                                target,
                                stack[--sp],
                                strict);

                            pc += 3;
                            break;
                        }

                        case JsOpcode.DeleteGlobalBinding:
                        {
                            var name = names[U16(code, pc)];

                            // A LEXICAL BINDING IS NOT A PROPERTY AND IS NOT DELETABLE, and a name
                            // neither half carries was never there to keep - which the language
                            // answers `true` for, not with the `ReferenceError` a READ of the same
                            // name would give.
                            stack[sp++] = JsValue.Boolean(
                                (!Realm.HasLexicals || !Realm.TryLexical(name, out _)) &&
                                (!Realm.GlobalObject.HasOwnProperty(name) ||
                                    Realm.GlobalObject.DeleteOwnProperty(name)));

                            pc += 3;
                            break;
                        }

                        case JsOpcode.DeclareGlobalLet:
                            Realm.DeclareLexical(names[U16(code, pc)], mutable: true);
                            pc += 3;
                            break;

                        case JsOpcode.DeclareGlobalConst:
                            Realm.DeclareLexical(names[U16(code, pc)], mutable: false);
                            pc += 3;
                            break;

                        case JsOpcode.InitialiseGlobalLexical:
                        {
                            var name = names[U16(code, pc)];

                            if (!Realm.TryLexical(name, out var bound))
                            {
                                throw new JsAbort(
                                    JsAbortKind.InternalDefect,
                                    "a lexical initialiser named no binding");
                            }

                            bound.Value = stack[--sp];
                            bound.Initialised = true;
                            pc += 3;
                            break;
                        }

                        case JsOpcode.DeclareGlobal:
                        {
                            var name = names[U16(code, pc)];

                            if (!Realm.GlobalObject.HasOwnProperty(name))
                            {
                                Realm.GlobalObject.SetOwnProperty(
                                    name,
                                    JsProperty.Data(
                                        JsValue.Undefined,
                                        JsPropertyAttributes.Writable | JsPropertyAttributes.Enumerable));
                            }

                            pc += 3;
                            break;
                        }

                        case JsOpcode.PushScope:
                            scopes.Add(new JsEnvironment(U16(code, pc), scopes[^1]));
                            pc += 3;
                            break;

                        case JsOpcode.PushObjectScope:
                            // The coercion is charged like the allocation it usually is: a String or
                            // a Number operand builds a wrapper object here, and `with (null)` is
                            // the TypeError `ToObject` already throws.
                            Charge(4);
                            scopes.Add(new JsEnvironment(ToObject(stack[--sp]), scopes[^1]));
                            pc++;
                            break;

                        case JsOpcode.ResolveName:
                            stack[sp++] = ResolveName(scopes, code[pc + 1], names[U16(code, pc + 1)]);
                            pc += 4;
                            break;

                        // EVAL CODE'S FREE NAMES, RESOLVED THROUGH THE VIEW ITS BOUNDARY RECORD WAS
                        // ENTERED WITH (JSD-0026 section 5). The operand's high half counts the
                        // records to the boundary and its low half names the name; everything past
                        // the boundary is the caller's, described by the caller's verified map.
                        case JsOpcode.LoadEvalName:
                            stack[sp++] = ReadEvalName(
                                EvalBoundary(scopes, code[pc + 1]), names[U16(code, pc + 1)], false);

                            pc += 4;
                            break;

                        case JsOpcode.LoadEvalNameOrUndefined:
                            stack[sp++] = ReadEvalName(
                                EvalBoundary(scopes, code[pc + 1]), names[U16(code, pc + 1)], true);

                            pc += 4;
                            break;

                        case JsOpcode.StoreEvalName:
                            WriteEvalName(
                                EvalBoundary(scopes, code[pc + 1]),
                                names[U16(code, pc + 1)],
                                stack[--sp],
                                strict);

                            pc += 4;
                            break;

                        case JsOpcode.LoadEvalNameWithBase:
                        {
                            var name = names[U16(code, pc + 1)];
                            var binding = ResolveEvalName(EvalBoundary(scopes, code[pc + 1]), name);

                            if (binding.Object is { } holder)
                            {
                                stack[sp++] = GetProperty(JsValue.Object(holder), name);
                                stack[sp++] = holder is JsEvalVariables
                                    ? JsValue.Undefined
                                    : JsValue.Object(holder);
                            }
                            else
                            {
                                stack[sp++] = ReadEvalBinding(binding, name, false);
                                stack[sp++] = JsValue.Undefined;
                            }

                            pc += 4;
                            break;
                        }

                        case JsOpcode.DeleteEvalName:
                            stack[sp++] = DeleteEvalName(
                                EvalBoundary(scopes, code[pc + 1]), names[U16(code, pc + 1)], strict);

                            pc += 4;
                            break;

                        // A SLOPPY EVALUATION'S FUNCTION DECLARATIONS AND ANNEX B ALIASES ARE WRITTEN
                        // TO ITS VARIABLE ENVIRONMENT DIRECTLY (JSeal V15), past any `with` object or
                        // catch parameter of the same name between the call and it.
                        case JsOpcode.StoreEvalVariable:
                            WriteEvalVariable(
                                EvalBoundary(scopes, code[pc + 1]), names[U16(code, pc + 1)], stack[--sp]);

                            pc += 4;
                            break;

                        // THE RECEIVER OF A CALL THROUGH A NAME A SEARCH ANSWERED: the object for a
                        // `with` record, `undefined` for a function's eval variables, which no guest
                        // code may hold (JSeal V15).
                        case JsOpcode.WithBaseObject:
                            if (stack[sp - 1].IsObject && stack[sp - 1].AsObject() is JsEvalVariables)
                            {
                                stack[sp - 1] = JsValue.Undefined;
                            }

                            pc++;
                            break;

                        case JsOpcode.PopScope:
                            scopes.RemoveAt(scopes.Count - 1);
                            pc++;
                            break;

                        case JsOpcode.CopyScope:
                            scopes[^1] = scopes[^1].Copy(U16(code, pc));
                            pc += 3;
                            break;

                        case JsOpcode.NewObject:
                            stack[sp++] = JsValue.Object(new JsObject(Realm.ObjectPrototype));
                            pc++;
                            break;

                        case JsOpcode.NewArray:
                        {
                            var count = U16(code, pc);
                            var array = new JsArray(Realm.ArrayPrototype);

                            for (var at = 0; at < count; at++)
                            {
                                array.Push(stack[sp - count + at]);
                            }

                            sp -= count;
                            stack[sp++] = JsValue.Object(array);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.GetProperty:
                        {
                            var target = stack[--sp];
                            stack[sp++] = GetProperty(target, names[U16(code, pc)]);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.SetProperty:
                        {
                            var value = stack[--sp];
                            var target = stack[--sp];
                            SetProperty(target, names[U16(code, pc)], value, strict);
                            stack[sp++] = value;
                            pc += 3;
                            break;
                        }

                        case JsOpcode.GetIndex:
                        {
                            var key = stack[--sp];
                            var target = stack[--sp];
                            stack[sp++] = GetIndexed(target, key);
                            pc++;
                            break;
                        }

                        case JsOpcode.SetIndex:
                        {
                            var value = stack[--sp];
                            var key = stack[--sp];
                            var target = stack[--sp];
                            SetIndexed(target, key, value, strict);
                            stack[sp++] = value;
                            pc++;
                            break;
                        }

                        case JsOpcode.DefineField:
                            stack[sp - 2].AsObject().SetOwnProperty(
                                names[U16(code, pc)],
                                JsProperty.Data(stack[sp - 1], JsPropertyAttributes.Default));

                            sp--;
                            pc += 3;
                            break;

                        case JsOpcode.DefineIndexed:
                        {
                            var value = stack[--sp];
                            var key = stack[--sp];
                            DefineByKey(stack[sp - 1].AsObject(), key, value);
                            pc++;
                            break;
                        }

                        case JsOpcode.SetPrototypeLiteral:
                        {
                            var wanted = stack[--sp];

                            // A VALUE THAT IS NEITHER AN OBJECT NOR `null` IS IGNORED rather than
                            // refused: `{ __proto__: 5 }` is an ordinary object, and the member is
                            // dropped. No cycle is reachable here and no refusal is possible - the
                            // object is one this instruction sequence just built, so it is
                            // extensible and nothing else holds a reference through which it could
                            // appear in the chain being installed.
                            if (wanted.IsObject)
                            {
                                stack[sp - 1].AsObject().Prototype = wanted.AsObject();
                            }
                            else if (wanted.Type == JsType.Null)
                            {
                                stack[sp - 1].AsObject().Prototype = null;
                            }

                            pc++;
                            break;
                        }

                        case JsOpcode.DefineGetter:
                        case JsOpcode.DefineSetter:
                        {
                            var accessor = stack[--sp].AsObject();
                            var host = stack[sp - 1].AsObject();
                            var key = names[U16(code, pc)];
                            host.TryGetOwnProperty(key, out var existing);

                            host.SetOwnProperty(
                                key,
                                JsProperty.Accessor(
                                    opcode == JsOpcode.DefineGetter ? accessor : existing.Getter,
                                    opcode == JsOpcode.DefineSetter ? accessor : existing.Setter,
                                    JsPropertyAttributes.Enumerable | JsPropertyAttributes.Configurable));

                            pc += 3;
                            break;
                        }

                        case JsOpcode.ArrayAppend:
                        {
                            var element = stack[--sp];
                            var array = (JsArray)stack[sp - 1].AsObject();
                            Charge(1);
                            array.SetIndex(array.Length, element);
                            pc++;
                            break;
                        }

                        case JsOpcode.ArrayHoles:
                        {
                            var array = (JsArray)stack[sp - 1].AsObject();
                            array.SetLength(array.Length + U16(code, pc));
                            pc += 3;
                            break;
                        }

                        case JsOpcode.SpreadArray:
                        {
                            var source = stack[--sp];
                            var array = (JsArray)stack[sp - 1].AsObject();
                            var values = new System.Collections.Generic.List<JsValue>();
                            IterateInto(source, values);

                            foreach (var element in values)
                            {
                                Charge(1);
                                array.SetIndex(array.Length, element);
                            }

                            pc++;
                            break;
                        }

                        case JsOpcode.SpreadObject:
                        {
                            var source = stack[--sp];
                            CopyDataProperties(stack[sp - 1].AsObject(), source);
                            pc++;
                            break;
                        }

                        case JsOpcode.DeleteProperty:
                        {
                            var target = stack[--sp];
                            var key = names[U16(code, pc)];
                            var went = !target.IsObject || target.AsObject().DeleteOwnProperty(key);

                            // A REFUSED DELETE ANSWERS `false` IN SLOPPY CODE AND THROWS IN STRICT
                            // CODE, and the pair is the same rule the assignment above follows: an
                            // operation the object refused is reported as a value where a program
                            // may not have asked, and as an exception where it said it wanted to
                            // know. A `false` nobody reads is how a frozen property gets treated as
                            // deleted by the code after it.
                            if (!went && strict)
                            {
                                ThrowTypeError("Cannot delete property '" + key + "'");
                            }

                            stack[sp++] = JsValue.Boolean(went);
                            pc += 3;
                            break;
                        }

                        // A COMPUTED DELETE OBEYS THE SAME STRICT RULE THE STATIC ONE DOES, and it
                        // did not: `delete o.frozen` threw in strict code and `delete o["frozen"]`
                        // answered `false` and went on. The two are one operator written two ways,
                        // so a program that reached a refused delete through a computed key was
                        // told nothing and carried on as though the property were gone.
                        case JsOpcode.DeleteIndex:
                        {
                            var key = stack[--sp];
                            var target = stack[--sp];

                            var removed = !target.IsObject ||
                                (key.IsSymbol
                                    ? target.AsObject().DeleteOwnSymbol(key.AsSymbol())
                                    : target.AsObject().DeleteOwnProperty(ToPropertyKey(key)));

                            if (!removed && strict)
                            {
                                ThrowTypeError(
                                    "Cannot delete property '" +
                                    (key.IsSymbol
                                        ? "Symbol(" + key.AsSymbol().Description + ")"
                                        : ToPropertyKey(key)) +
                                    "'");
                            }

                            stack[sp++] = JsValue.Boolean(removed);
                            pc++;
                            break;
                        }

                        case JsOpcode.Closure:
                            stack[sp++] = JsValue.Object(
                                Realm.CreateClosure(
                                    program,
                                    U16(code, pc),
                                    scopes[^1],
                                    thisValue,
                                    thisBinding,
                                    newTarget,
                                    active,
                                    activeReferrer));

                            pc += 3;
                            break;

                        // A COMPUTED KEY MAY BE A SYMBOL, and this is the instruction most likely
                        // to meet one: `class C { [Symbol.iterator]() {} }` is how a guest joins
                        // the iteration protocol, and converting the key to a String first threw
                        // the TypeError that conversion owes rather than defining the member.
                        case JsOpcode.DefineMethod:
                        {
                            var member = stack[--sp];
                            var key = stack[--sp];
                            var host = stack[sp - 1].AsObject();

                            if (key.IsSymbol)
                            {
                                DefineSymbolMember(host, key.AsSymbol(), member, code[pc + 1]);
                            }
                            else
                            {
                                DefineMember(host, ToPropertyKey(key), member, code[pc + 1]);
                            }

                            pc += 2;
                            break;
                        }

                        // THE ORDER OF THE THREE STEPS IS OBSERVABLE AND IS THE SPECIFICATION'S:
                        // the this binding is read, then the base is taken from the home object,
                        // and only then is the key converted. A key whose `toString` re-points the
                        // home object's prototype still reads through the prototype the reference
                        // was made against, and a derived constructor that has not called
                        // `super()` fails before the conversion runs at all.
                        case JsOpcode.LoadSuperProperty:
                        {
                            var receiver = thisBinding is null
                                ? thisValue
                                : ThisBinding(thisBinding);

                            // A SUPER KEY IS A PROPERTY KEY AND NOT A STRING: `super[Symbol.replace]`
                            // is how a subclass of a built-in reaches the parent's protocol method.
                            var start = SuperBase(active);
                            var key = ToPropertyKeyValue(stack[--sp]);

                            stack[sp++] = key.IsSymbol
                                ? GetSymbolWithReceiver(start, key.AsSymbol(), receiver)
                                : Lookup(start, key.AsString(), receiver);

                            pc++;
                            break;
                        }

                        case JsOpcode.StoreSuperProperty:
                        {
                            var value = stack[--sp];

                            var receiver = thisBinding is null
                                ? thisValue
                                : ThisBinding(thisBinding);

                            var start = SuperBase(active);
                            var key = ToPropertyKeyValue(stack[--sp]);

                            if (!key.IsSymbol)
                            {
                                SetSuper(start, receiver, key.AsString(), value, strict);
                            }
                            else if (!SetSymbolWithReceiver(start, key.AsSymbol(), value, receiver) &&
                                strict)
                            {
                                ThrowTypeError("Cannot assign to a read only Symbol-keyed property");
                            }

                            stack[sp++] = value;
                            pc++;
                            break;
                        }

                        case JsOpcode.NewClass:
                        {
                            var derived = (code[pc + 1] & JsOpcodes.ClassIsDerived) != 0;
                            var constructor = stack[--sp];
                            var heritage = derived ? stack[--sp] : JsValue.Undefined;
                            stack[sp++] = BuildClass(constructor, derived, heritage);
                            pc += 2;
                            break;
                        }

                        // THE PAIR UNDER THE KEY IS READ AND NOT POPPED, which is what lets a whole
                        // class body run over the one constructor-and-prototype pair the lowering
                        // loaded once. Which of the two is the home object is the static bit's
                        // answer and nothing else's.
                        case JsOpcode.DefineClassElement:
                        {
                            var elementFlags = code[pc + 1];
                            var body = stack[--sp];
                            var key = stack[--sp];

                            RecordClassElement(
                                (JsScriptFunction)stack[sp - 2].AsObject(),
                                stack[sp - 1].AsObject(),
                                key,
                                body,
                                elementFlags);

                            pc += 2;
                            break;
                        }

                        case JsOpcode.RunStaticElements:
                            RunStaticElements((JsScriptFunction)stack[sp - 1].AsObject());
                            pc++;
                            break;

                        case JsOpcode.NewPrivateName:
                            Charge(2);

                            stack[sp++] = JsValue.Symbol(
                                new JsSymbol(program.Constants[U16(code, pc)].AsString(), described: true)
                                {
                                    IsPrivateName = true,
                                });

                            pc += 3;
                            break;

                        case JsOpcode.LoadPrivate:
                        {
                            var name = stack[--sp].AsSymbol();
                            var host = stack[--sp];
                            stack[sp++] = ReadPrivate(host, name);
                            pc++;
                            break;
                        }

                        case JsOpcode.StorePrivate:
                        {
                            var written = stack[--sp];
                            var name = stack[--sp].AsSymbol();
                            var host = stack[--sp];
                            WritePrivate(host, name, written);
                            stack[sp++] = written;
                            pc++;
                            break;
                        }

                        // A NON-OBJECT IS A TypeError AND NOT A `false`. The form exists to ask a
                        // question `o.#x` would have thrown for, so answering `false` for a number
                        // reads like the right answer - and it is the ORDINARY `in` operator's
                        // answer that this follows instead: `"x" in 5` throws, and the private form
                        // is the same operator with a name the grammar spells differently. The
                        // opposite reading was written first and the comparison engine refused it.
                        case JsOpcode.HasPrivate:
                        {
                            var name = stack[--sp].AsSymbol();
                            var host = stack[--sp];

                            if (!host.IsObject)
                            {
                                ThrowTypeError(
                                    "Cannot use 'in' operator to search for '" + name.Description +
                                    "' in " + Describe(host));
                            }

                            stack[sp++] = JsValue.Boolean(host.AsObject().HasPrivate(name));
                            pc++;
                            break;
                        }

                        case JsOpcode.SuperCall:
                        {
                            var argc = code[pc + 1];
                            var passed = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                passed[at] = stack[--sp];
                            }

                            stack[sp++] = SuperConstruct(active, thisBinding, newTarget, passed);
                            pc += 2;
                            break;
                        }

                        // THE ARGUMENT ARRAY CARRIES THE COUNT, and everything else this needs -
                        // the superclass, the `new.target`, the `this` slot it binds - comes from
                        // the frame exactly as SuperCall's does.
                        case JsOpcode.SuperCallSpread:
                            stack[sp - 1] = SuperConstruct(
                                active, thisBinding, newTarget, ArgumentsOf(stack[sp - 1]));

                            pc++;
                            break;

                        case JsOpcode.SuperCallForwarded:
                            stack[sp++] = SuperConstruct(
                                active, thisBinding, newTarget, actualArguments);

                            pc++;
                            break;

                        case JsOpcode.Call:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var receiver = stack[--sp];
                            var callee = stack[--sp];
                            stack[sp++] = Call(callee, receiver, arguments);
                            pc += 2;
                            break;
                        }

                        case JsOpcode.CallEval:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var receiver = stack[--sp];
                            var callee = stack[--sp];

                            // THE SPELLING SAYS DIRECT; THE VALUE DECIDES WHETHER IT IS. A program
                            // may assign to the global `eval`, and a call to whatever it now holds
                            // is an ordinary call however it is written.
                            stack[sp++] = Realm.IsEvalIntrinsic(callee)
                                ? EvaluateDirect(
                                    program, unitIndex, current, scopes, arguments,
                                    thisValue, thisBinding, newTarget, active)
                                : Call(callee, receiver, arguments);

                            pc += 2;
                            break;
                        }

                        // THE SPREAD SPELLING OF THE SAME CALL, AND THE SAME TWO ANSWERS. The
                        // arguments arrive as one Array, exactly as `CallSpread`'s do; what the
                        // callee's identity decides is the same thing it decides for `CallEval`.
                        case JsOpcode.CallEvalSpread:
                        {
                            var spread = ArgumentsOf(stack[--sp]);
                            var receiver = stack[--sp];
                            var callee = stack[--sp];

                            stack[sp++] = Realm.IsEvalIntrinsic(callee)
                                ? EvaluateDirect(
                                    program, unitIndex, current, scopes, spread,
                                    thisValue, thisBinding, newTarget, active)
                                : Call(callee, receiver, spread);

                            pc++;
                            break;
                        }

                        case JsOpcode.Construct:
                        {
                            var argc = code[pc + 1];
                            var arguments = argc == 0 ? System.Array.Empty<JsValue>() : new JsValue[argc];

                            for (var at = argc - 1; at >= 0; at--)
                            {
                                arguments[at] = stack[--sp];
                            }

                            var callee = stack[--sp];
                            stack[sp++] = Construct(callee, arguments);
                            pc += 2;
                            break;
                        }

                        case JsOpcode.CallSpread:
                        {
                            var spread = ArgumentsOf(stack[--sp]);
                            var receiver = stack[--sp];
                            var callee = stack[--sp];
                            stack[sp++] = Call(callee, receiver, spread);
                            pc++;
                            break;
                        }

                        case JsOpcode.ConstructSpread:
                        {
                            var spread = ArgumentsOf(stack[--sp]);
                            var callee = stack[--sp];
                            stack[sp++] = Construct(callee, spread);
                            pc++;
                            break;
                        }

                        case JsOpcode.Return:
                            return typeof(TMode) == typeof(JsInterpreted)
                                ? stack[--sp]
                                : act!.Exit(stack[--sp]);

                        case JsOpcode.ReturnUndefined:
                            return typeof(TMode) == typeof(JsInterpreted)
                                ? JsValue.Undefined
                                : act!.Exit(JsValue.Undefined);

                        case JsOpcode.Add:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = Add(left, right);
                            pc++;
                            break;
                        }

                        case JsOpcode.Subtract:
                            Binary(stack, ref sp, opcode, static (a, b) => a - b, this);
                            pc++;
                            break;

                        case JsOpcode.Multiply:
                            Binary(stack, ref sp, opcode, static (a, b) => a * b, this);
                            pc++;
                            break;

                        case JsOpcode.Divide:
                            Binary(stack, ref sp, opcode, static (a, b) => a / b, this);
                            pc++;
                            break;

                        case JsOpcode.Remainder:
                            Binary(stack, ref sp, opcode, static (a, b) => a % b, this);
                            pc++;
                            break;

                        case JsOpcode.Exponent:
                            Binary(stack, ref sp, opcode, static (a, b) => JsRealm.MathPower(a, b), this);
                            pc++;
                            break;

                        case JsOpcode.Negate:
                        {
                            var numeric = ToNumeric(stack[sp - 1]);
                            stack[sp - 1] = numeric.IsNumber
                                ? JsValue.Number(-numeric.AsNumber())
                                : BigIntResult(JsBigInt.Negate(numeric.AsBigInt(), ChargeFuel));

                            pc++;
                            break;
                        }

                        case JsOpcode.ToNumber:
                            stack[sp - 1] = JsValue.Number(ToNumber(stack[sp - 1]));
                            pc++;
                            break;

                        // THE UPDATE EXPRESSIONS (JSeal B05): one conversion that keeps a BigInt,
                        // then one step of the operand's own type. A Number takes exactly the
                        // arithmetic `x + 1` and `x - 1` always took.
                        case JsOpcode.ToNumeric:
                            stack[sp - 1] = ToNumeric(stack[sp - 1]);
                            pc++;
                            break;

                        case JsOpcode.Increment:
                        case JsOpcode.Decrement:
                            stack[sp - 1] = NumericStep(stack[sp - 1], opcode == JsOpcode.Increment);
                            pc++;
                            break;

                        case JsOpcode.Not:
                            stack[sp - 1] = JsValue.Boolean(!stack[sp - 1].ToBooleanValue());
                            pc++;
                            break;

                        case JsOpcode.BitwiseNot:
                        {
                            var numeric = ToNumeric(stack[sp - 1]);
                            stack[sp - 1] = numeric.IsNumber
                                ? JsValue.Number(~JsValue.ToInt32(numeric.AsNumber()))
                                : BigIntResult(JsBigInt.BitwiseNot(numeric.AsBigInt(), ChargeFuel));

                            pc++;
                            break;
                        }

                        case JsOpcode.LessThan:
                        case JsOpcode.LessThanOrEqual:
                        case JsOpcode.GreaterThan:
                        case JsOpcode.GreaterThanOrEqual:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(Relational(opcode, left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.StrictEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            ChargeComparison(left, right);
                            stack[sp++] = JsValue.Boolean(left.StrictlyEquals(right));
                            pc++;
                            break;
                        }

                        case JsOpcode.StrictNotEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            ChargeComparison(left, right);
                            stack[sp++] = JsValue.Boolean(!left.StrictlyEquals(right));
                            pc++;
                            break;
                        }

                        case JsOpcode.LooseEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(LooselyEquals(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.LooseNotEquals:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(!LooselyEquals(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.BitwiseOr:
                        case JsOpcode.BitwiseAnd:
                        case JsOpcode.BitwiseXor:
                        case JsOpcode.ShiftLeft:
                        case JsOpcode.ShiftRight:
                        case JsOpcode.ShiftRightUnsigned:
                        {
                            var rightValue = stack[--sp];
                            var leftValue = stack[--sp];
                            stack[sp++] = Bitwise(opcode, leftValue, rightValue);
                            pc++;
                            break;
                        }

                        case JsOpcode.TypeOf:
                            stack[sp - 1] = JsValue.String(stack[sp - 1].TypeOf());
                            pc++;
                            break;

                        case JsOpcode.InstanceOf:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];
                            stack[sp++] = JsValue.Boolean(InstanceOf(left, right));
                            pc++;
                            break;
                        }

                        case JsOpcode.In:
                        {
                            var right = stack[--sp];
                            var left = stack[--sp];

                            if (!right.IsObject)
                            {
                                ThrowTypeError("Cannot use 'in' operator to search for a key");
                            }

                            stack[sp++] = JsValue.Boolean(
                                left.IsSymbol
                                    ? HasSymbol(right.AsObject(), left.AsSymbol())
                                    : HasProperty(right.AsObject(), ToPropertyKey(left)));

                            pc++;
                            break;
                        }

                        case JsOpcode.Void:
                            stack[sp - 1] = JsValue.Undefined;
                            pc++;
                            break;

                        case JsOpcode.RequireCoercible:
                        {
                            var subject = stack[sp - 1];

                            if (subject.IsNullish)
                            {
                                var named = names[U16(code, pc)];

                                ThrowTypeError(
                                    named.Length == 0
                                        ? "Cannot destructure " + Describe(subject)
                                        : "Cannot destructure property '" + named + "' of " +
                                            Describe(subject));
                            }

                            pc += 3;
                            break;
                        }

                        // ONLY AN OBJECT KEY IS CONVERTED HERE, because only an object can run code
                        // on its way to becoming a key; the read and the write after this see the
                        // String or Symbol it produced and convert nothing observable. The base is
                        // refused first, as `GetValue` refuses it before converting the key.
                        case JsOpcode.ToPropertyKey:
                        {
                            var key = stack[sp - 1];

                            if (key.IsObject)
                            {
                                var target = stack[sp - 2];

                                if (target.IsNullish)
                                {
                                    ThrowTypeError(
                                        "Cannot read properties of " +
                                            (target.Type == JsType.Null ? "null" : "undefined"));
                                }

                                stack[sp - 1] = ToPropertyKeyValue(key);
                            }

                            pc++;
                            break;
                        }

                        case JsOpcode.Jump:
                            pc = (int)U32(code, pc);
                            break;

                        case JsOpcode.JumpIfFalse:
                            pc = !stack[--sp].ToBooleanValue() ? (int)U32(code, pc) : pc + 5;
                            break;

                        case JsOpcode.JumpIfTrue:
                            pc = stack[--sp].ToBooleanValue() ? (int)U32(code, pc) : pc + 5;
                            break;

                        case JsOpcode.Throw:
                        {
                            var thrown = stack[--sp];

                            // THE ONE VALUE THIS INSTRUCTION DOES NOT THROW. A `finally` that a
                            // forced return passed through re-raises what it parked, using this
                            // instruction because the lowering has no other; a forced return
                            // parked here comes back out as a forced return, so an outer `catch`
                            // still never sees it.
                            if (thrown.AsObjectOrNull() is JsForcedReturn forced)
                            {
                                throw new JsReturnSignal(forced.Value);
                            }

                            throw new JsThrow(thrown, Render(thrown));
                        }

                        case JsOpcode.ForInStart:
                        {
                            var target = stack[--sp];
                            stack[sp++] = JsValue.Object(Realm.CreateEnumerator(this, target));
                            pc++;
                            break;
                        }

                        case JsOpcode.ForInNext:
                        {
                            var enumerator = (JsEnumerator)stack[--sp].AsObject();

                            if (enumerator.TryNext(out var key))
                            {
                                stack[sp++] = JsValue.String(key);
                                pc += 5;
                            }
                            else
                            {
                                pc = (int)U32(code, pc);
                            }

                            break;
                        }

                        case JsOpcode.IterateStart:
                            stack[sp - 1] = JsValue.Object(GetIterator(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.IterateNext:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (TryIterateNext(record, out var element))
                            {
                                stack[sp++] = element;
                                pc += 5;
                            }
                            else
                            {
                                pc = (int)U32(code, pc);
                            }

                            break;
                        }

                        case JsOpcode.IterateRest:
                            stack[sp - 1] = JsValue.Object(
                                DrainIterator((JsIteratorRecord)stack[sp - 1].AsObject()));

                            pc++;
                            break;

                        case JsOpcode.IterateClose:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (code[pc + 1] == 0)
                            {
                                CloseIterator(record);
                            }
                            else
                            {
                                CloseIteratorQuietly(record);
                            }

                            pc += 2;
                            break;
                        }

                        case JsOpcode.Yield:
                        {
                            // THE WHOLE OF SUSPENDING. The yielded value leaves on the return, the
                            // height and the pointer stay behind in the frame, and the pointer is
                            // left AT this instruction rather than after it - so a resumption that
                            // arrives abruptly raises its throw or its return at a point the
                            // enclosing exception regions actually cover. Nothing clears the
                            // delegation flag here because nothing can have set it: a suspended
                            // `yield*` always resumes at its own instruction, never at this one.
                            var yielded = stack[--sp];
                            frame!.Sp = sp;
                            frame.Pc = pc;
                            frame.Suspended = true;

                            // THE KIND IS RECORDED FOR THE ONE DRIVER THAT HAS TO ASK. A generator
                            // and an async function each suspend one way, so neither reads this;
                            // an async generator's body suspends both ways into one frame, and the
                            // two mean opposite things to whoever receives the value.
                            frame.Suspension = JsSuspension.Yield;
                            return typeof(TMode) == typeof(JsInterpreted) ? yielded : act!.Exit(yielded);
                        }

                        // AN IMPORT READ GOES TO THE EXPORTING ENVIRONMENT EVERY TIME. Nothing is
                        // cached here and nothing may be: a live binding is one whose later value
                        // is seen, so the only correct read is the one that happens now.
                        case JsOpcode.LoadImport:
                            stack[sp++] = JsModuleNamespace.Read(
                                program.ImportBindings[U16(code, pc)], Graph(program), this);

                            pc += 3;
                            break;

                        case JsOpcode.ThrowImmutable:
                            ThrowTypeError(
                                "Assignment to constant variable '" + names[U16(code, pc)] + "'");

                            break;

                        // THE SEAM BETWEEN THE PARAMETER LIST AND THE BODY, WHICH ONLY ONE OF THE
                        // TWO ENTRIES STOPS AT. A generator whose unit binds its own parameters is
                        // entered here by the CALL, which has just run the defaults, the rest
                        // parameter and the patterns; it leaves the frame pointing at the first
                        // instruction of the body and hands it to the generator object, which is
                        // the specification's `suspendedStart`. Every other run - the first `next`,
                        // an ordinary call of a unit that is not a generator at all - walks
                        // straight through for one dispatch.
                        case JsOpcode.EnterBody:
                            if (frame is { BindingParameters: true })
                            {
                                // THE POINTER IS LEFT PAST THIS INSTRUCTION AND NOT AT IT, which is
                                // the opposite of what `Yield` does and for the opposite reason:
                                // a yield is RE-ENTERED so that an abrupt resumption raises inside
                                // the regions covering it, and this is not re-entered at all. The
                                // resumption that follows is the first entry into the body, and
                                // `Started` stays false so that it neither pushes a sent value nor
                                // raises one - a `return` or a `throw` arriving before it completes
                                // the generator without running any of the body, which is what the
                                // language says about a generator that has not started.
                                frame.BindingParameters = false;
                                frame.Sp = sp;
                                frame.Pc = pc + 1;
                                frame.Suspended = true;

                                return typeof(TMode) == typeof(JsInterpreted)
                                    ? JsValue.Undefined
                                    : act!.Exit(JsValue.Undefined);
                            }

                            pc++;
                            break;
                        // A DYNAMIC IMPORT LEAVES A PROMISE WHERE IT TOOK TWO VALUES, AND NEVER
                        // THROWS. Everything that can go wrong past this point - a specifier that
                        // will not coerce, an attribute nothing honours, a module nothing can find,
                        // a module whose body threw - reaches the guest as a rejection, because
                        // that is the one thing the language guarantees about this production.
                        case JsOpcode.ImportCall:
                        {
                            var options = stack[--sp];
                            var specifier = stack[--sp];

                            // CODE COMPILED WITH NO REFERRER - eval code, a Function body - resolves
                            // against the running script or module (JSD-0024 section 20).
                            var written = names[U16(code, pc)];

                            stack[sp++] = DynamicImport(
                                program, written.Length == 0 ? activeReferrer : written, specifier, options);

                            pc += 3;
                            break;
                        }

                        // AND `import.meta` LEAVES THE ONE OBJECT THE MODULE HAS, built the first
                        // time it is asked for. Building it in the module's initialiser instead
                        // would have made an object for every module of every graph, whether or not
                        // a line of the module ever mentions it.
                        case JsOpcode.ImportMeta:
                        {
                            var instance = Graph(program)[U16(code, pc)];

                            if (instance.Meta is null)
                            {
                                Charge(FuelPerInstruction);
                                instance.Meta = new JsObject(prototype: null);
                            }

                            stack[sp++] = JsValue.Object(instance.Meta);
                            pc += 3;
                            break;
                        }

                        case JsOpcode.Await:
                        {
                            // THE SAME SUSPEND AS `Yield`, AND THE VALUE MEANS SOMETHING ELSE ON
                            // THE WAY OUT. What leaves on the return is what is being awaited
                            // rather than what is being yielded, and the driver that receives it
                            // resolves it and registers this frame's continuation. The pointer is
                            // left AT this instruction for the same reason: a rejected await
                            // re-enters abruptly and has to raise where the body's own exception
                            // regions cover it.
                            var awaited = stack[--sp];
                            frame!.Sp = sp;
                            frame.Pc = pc;
                            frame.Suspended = true;
                            frame.Suspension = JsSuspension.Await;
                            return typeof(TMode) == typeof(JsInterpreted) ? awaited : act!.Exit(awaited);
                        }

                        case JsOpcode.YieldDelegate:
                        {
                            // TWO DELEGATION LOOPS AND ONE INSTRUCTION, chosen by the unit's own
                            // flag rather than by anything on the stack. A synchronous delegation
                            // runs between two yields inside one entry into this loop; an
                            // asynchronous one leaves after every inner call and comes back at this
                            // same instruction, so it has five re-entry points where the
                            // synchronous one has one. Which of the two a body gets is fixed when
                            // it is verified and cannot change at run time.
                            var step = unit.IsAsync
                                ? DelegateAsync(frame!, stack, ref sp, pc)
                                : Delegate(frame!, stack, ref sp, pc);

                            if (frame!.Suspended)
                            {
                                return typeof(TMode) == typeof(JsInterpreted) ? step : act!.Exit(step);
                            }

                            stack[sp++] = step;
                            pc++;
                            break;
                        }

                        case JsOpcode.IterateStartAsync:
                            stack[sp - 1] = JsValue.Object(Realm.GetAsyncIterator(stack[sp - 1]));
                            pc++;
                            break;

                        case JsOpcode.IterateNextAsync:
                        {
                            // WHAT IS PUSHED IS UNAWAITED AND IS NOT A STEP. An async iterator's
                            // `next` answers a promise, and the instruction after this one is the
                            // `Await` that resolves it - so nothing here reads `done` or `value`,
                            // because neither exists yet.
                            var record = (JsIteratorRecord)stack[sp - 1].AsObject();
                            Charge(2);

                            // THE RECORD IS MARKED DONE FOR THE LENGTH OF THE STEP, and unmarked by
                            // `IterateAwaitStep` when the step turns out to have a value. A head
                            // step that fails - the call throwing, the promise rejecting, the
                            // answer not being an object - owes the iterator NO `return`: the
                            // specification propagates all of those with `?` and closes only for an
                            // abrupt binding or body. The loop's handler closes unconditionally, so
                            // the flag is what tells it which of the two happened, exactly as the
                            // synchronous `TryIterateNext` uses it.
                            record.Done = true;
                            stack[sp++] = Call(record.Next, record.Iterator, []);
                            pc++;
                            break;
                        }

                        case JsOpcode.IterateAwaitStep:
                        {
                            var step = stack[--sp];
                            var record = (JsIteratorRecord)stack[--sp].AsObject();

                            if (!step.IsObject)
                            {
                                ThrowTypeError(
                                    "Iterator result " + Describe(step) + " is not an object");
                            }

                            if (GetProperty(step, "done").ToBooleanValue())
                            {
                                pc = (int)U32(code, pc);
                                break;
                            }

                            // THE RECORD COMES BACK OFF DONE HERE, and only here. Everything from
                            // the call of `next` to this point is a head step the specification
                            // propagates without closing; from this point to the next call it is
                            // the loop body, whose abrupt exits owe the iterator its `return`.
                            record.Done = false;
                            stack[sp++] = GetProperty(step, "value");
                            pc += 5;
                            break;
                        }

                        case JsOpcode.IterateCloseAsync:
                        {
                            var record = (JsIteratorRecord)stack[--sp].AsObject();
                            var method = record.Done
                                ? JsValue.Undefined
                                : GetProperty(record.Iterator, "return");

                            // AN ITERATOR THAT IS DONE OR HAS NO `return` IS NOT AWAITED AT ALL,
                            // and the branch is what keeps that promise-free. `AsyncIteratorClose`
                            // returns its completion the moment it finds no `return` to call, so a
                            // lowering that awaited unconditionally would have spent a turn of the
                            // job queue on every `break` out of a loop over an Array.
                            if (method.IsNullish)
                            {
                                record.Done = true;
                                pc = (int)U32(code, pc);
                                break;
                            }

                            if (!method.IsObject || !method.AsObject().IsCallable)
                            {
                                ThrowTypeError("The iterator's return is not a function");
                            }

                            record.Done = true;
                            stack[sp++] = Call(method, record.Iterator, []);
                            pc += 5;
                            break;
                        }

                        case JsOpcode.IterateCloseCheck:
                        {
                            var answered = stack[--sp];

                            if (!answered.IsObject)
                            {
                                ThrowTypeError(
                                    "The iterator's return answered " + Describe(answered) +
                                    " and not an object");
                            }

                            pc++;
                            break;
                        }

                        // ---- resource scopes (JSD-0034) -----------------------------------------
                        //
                        // THE SCOPE VALUE CARRIES THE WHOLE OF DISPOSAL'S STATE, so these five arms
                        // hold nothing between them: the entries, how far the unwinding has got,
                        // the completion so far and the two await flags all live on the object the
                        // lowering keeps in a hidden slot. That is what lets an async disposal leave
                        // this loop at every `Await` and come back to the next step.

                        case JsOpcode.DisposeScope:
                            Charge(1);
                            stack[sp++] = JsValue.Object(new JsDisposeScope());
                            pc++;
                            break;

                        case JsOpcode.DisposeAdd:
                        {
                            var disposal = JsDisposeScope.From(stack[--sp]);
                            Realm.DisposeScopeAdd(this, disposal, stack[sp - 1], code[pc + 1] != 0);
                            pc += 2;
                            break;
                        }

                        case JsOpcode.DisposeFold:
                        {
                            var disposal = JsDisposeScope.From(stack[--sp]);
                            Realm.DisposeScopeFold(this, disposal, stack[--sp]);
                            pc++;
                            break;
                        }

                        case JsOpcode.DisposeStep:
                        {
                            var disposal = JsDisposeScope.From(stack[--sp]);

                            if (Realm.DisposeScopeStep(this, disposal, out var awaited))
                            {
                                stack[sp++] = awaited;
                                pc += 5;
                            }
                            else
                            {
                                pc = (int)U32(code, pc);
                            }

                            break;
                        }

                        case JsOpcode.DisposeEnd:
                        {
                            var disposal = JsDisposeScope.From(stack[--sp]);
                            var settled = Realm.DisposeScopeEnd(this, disposal, code[pc + 1] != 0);

                            if (code[pc + 1] != 0)
                            {
                                stack[sp++] = settled;
                            }

                            pc += 2;
                            break;
                        }

                        case JsOpcode.Pop:
                            sp--;
                            pc++;
                            break;

                        case JsOpcode.Duplicate:
                            stack[sp] = stack[sp - 1];
                            sp++;
                            pc++;
                            break;

                        case JsOpcode.DuplicateTwo:
                            stack[sp] = stack[sp - 2];
                            stack[sp + 1] = stack[sp - 1];
                            sp += 2;
                            pc++;
                            break;

                        case JsOpcode.Swap:
                        {
                            (stack[sp - 1], stack[sp - 2]) = (stack[sp - 2], stack[sp - 1]);
                            pc++;
                            break;
                        }

                        case JsOpcode.Pick:
                            stack[sp] = stack[sp - 1 - code[pc + 1]];
                            sp++;
                            pc += 2;
                            break;

                        case JsOpcode.GetTemplateObject:
                        {
                            var count = code[pc + 1];
                            var strings = TemplateObject(program, pc, stack, sp - (2 * count), count);
                            sp -= 2 * count;
                            stack[sp++] = JsValue.Object(strings);
                            pc += 2;
                            break;
                        }

                        default:
                            throw new JsAbort(
                                JsAbortKind.InternalDefect, "a verified opcode had no case here");
                    }
                }
            }
            // A FILTER AND NOT A CATCH-AND-RETHROW, and the difference is a process termination.
            //
            // A frame with a `catch` that rethrows is entered during the SECOND pass: the runtime
            // runs the handler as a funclet above the current stack, and the rethrow starts a fresh
            // dispatch from there. A throw crossing a thousand of these accumulated a thousand
            // funclets and their dispatchers, and the process died - on a stack that holds eight
            // thousand ordinary calls. A guest `throw` from any depth past about five hundred was
            // fatal, whether or not the guest had a `catch` waiting for it.
            //
            // A FILTER runs in the FIRST pass, without unwinding and without a funclet per frame:
            // a frame with no region for this instruction answers false and is passed over, and
            // exactly one dispatch reaches the frame that has one. `TryFindHandler` is a pure
            // search, which is what a filter has to be.
            catch (JsThrow thrown) when (TryFindHandler(program, unitIndex, current, out region))
            {
                while (scopes.Count > region.ScopeDepth + 1)
                {
                    scopes.RemoveAt(scopes.Count - 1);
                }

                sp = (int)region.StackHeight;
                stack[sp++] = thrown.Value;
                pc = (int)region.Handler;
            }
            catch (JsReturnSignal forced)
            {
                // A FORCED RETURN RUNS EVERY `finally` AND NO `catch`. The search skips catch
                // regions rather than taking the innermost region of either kind, which is the one
                // place the two completions differ; taking the innermost would let
                // `try { yield } catch (e) {}` swallow a `gen.return()` as though somebody had
                // thrown, and the generator would carry on running instead of ending.
                if (!TryFindFinally(program, unitIndex, current, out var finaliser))
                {
                    throw;
                }

                while (scopes.Count > finaliser.ScopeDepth + 1)
                {
                    scopes.RemoveAt(scopes.Count - 1);
                }

                sp = (int)finaliser.StackHeight;
                stack[sp++] = JsValue.Object(new JsForcedReturn(forced.Value));
                pc = (int)finaliser.Handler;
            }
        }
    }

    /// <summary>
    /// The innermost <c>finally</c> region of <paramref name="unit"/> covering <paramref name="pc"/>.
    /// </summary>
    /// <remarks>
    /// The regions of one unit are recorded innermost-first by the lowering, so the first match in
    /// order is the innermost - the same property <see cref="TryFindHandler"/> relies on. What
    /// differs is only that a <c>catch</c> region is passed over rather than taken.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C8F9E6
    // Broiler-Human:        PENDING
    private static bool TryFindFinally(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit &&
                candidate.Kind == JsFormat.HandlerKind.Finally &&
                pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
    }

    /// <summary>
    /// One turn of a <c>yield*</c>: step the inner iterator, or forward an abrupt resumption to it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the specification's delegation loop, entered afresh at every resumption.</b> The
    /// iterator record lives in the frame, so the loop's whole state between two resumptions is
    /// "which record" and "still delegating" - and re-entering at the same instruction is what lets
    /// a <c>return</c> or a <c>throw</c> that arrives mid-delegation be handed to the inner
    /// iterator rather than raised in the outer body.
    /// </para>
    /// <para>
    /// <b>The source is acquired through <see cref="GetIterator"/> and stepped through
    /// <see cref="TryIterateNext(JsIteratorRecord, JsValue[], out JsValue, out JsValue)"/>, which
    /// is what makes <c>yield*</c> WORK OVER ANYTHING.</b> The
    /// operand is read for its <c>Symbol.iterator</c> exactly as a spread or a <c>for … of</c>
    /// reads it, so an Array, a String, a Map, a Set, another generator and a plain object that
    /// defines the Symbol itself are all delegated over by one path. Recognising the realm's own
    /// iterables by TYPE instead - which is what a realm without a Symbol has to do - refused every
    /// object a guest made iterable, and refused it as "not iterable", which is a claim about the
    /// program rather than about the engine.
    /// </para>
    /// <para>
    /// <b>The two missing-method cases are where an engine is most often wrong.</b> An inner
    /// iterator with no <c>return</c> - which is every Array and String iterator - does not swallow
    /// the outer <c>return</c>: the outer generator returns, running its own finalisers. An inner
    /// iterator with no <c>throw</c> is closed first and then the delegation raises a
    /// <c>TypeError</c>, so a program that throws into one is told what is missing rather than
    /// silently getting its own exception back.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E53DC7
    // Broiler-Falsified-If: a `return` or a `throw` that arrives while a `yield*` is suspended is not offered to the inner iterator first
    // Broiler-Human:        PENDING
    private JsValue Delegate(JsFrame frame, JsValue[] stack, ref int sp, int pc)
    {
        if (!frame.Delegating)
        {
            frame.Delegate = GetIterator(stack[--sp]);
            frame.ResumeMode = JsResumeMode.Next;
            frame.ResumeValue = JsValue.Undefined;
        }

        // THE FLAG IS CLEARED ON THE WAY IN AND SET AGAIN ONLY BY AN ACTUAL SUSPENSION, so every
        // other way out of this method ends the delegation - including the ways that leave by
        // throwing. Clearing it only on the paths that return normally left it set when an inner
        // `throw` method threw and the outer body caught: the next resumption would then re-enter
        // a delegation that no longer existed, at an instruction that was no longer a `yield*`.
        frame.Delegating = false;
        var record = frame.Delegate!;
        var mode = frame.ResumeMode;
        var sent = frame.ResumeValue;
        frame.ResumeMode = JsResumeMode.Next;
        frame.ResumeValue = JsValue.Undefined;
        JsValue step;

        switch (mode)
        {
            case JsResumeMode.Throw:
            {
                var thrower = GetProperty(record.Iterator, "throw");

                if (!thrower.IsObject || !thrower.AsObject().IsCallable)
                {
                    // CLOSED UNDER A NORMAL COMPLETION AND NOT A QUIET ONE, which is what the
                    // specification asks for here: an error the inner `return` raises while it is
                    // being cleaned up is the error the program is owed, and only if it raises
                    // none does the missing `throw` become the TypeError below.
                    CloseIterator(record);
                    return ThrowTypeError("The iterator does not provide a 'throw' method.");
                }

                step = Call(thrower, record.Iterator, [sent]);
                break;
            }

            case JsResumeMode.Return:
            {
                var returner = GetProperty(record.Iterator, "return");

                // AN INNER ITERATOR WITH NO `return` DOES NOT SWALLOW THE OUTER ONE. Every Array
                // and String iterator is in this case, so it is the common one rather than the
                // exotic one: the outer generator returns, and its own finalisers run on the way.
                if (!returner.IsObject || !returner.AsObject().IsCallable)
                {
                    record.Done = true;
                    throw new JsReturnSignal(sent);
                }

                step = Call(returner, record.Iterator, [sent]);

                if (!step.IsObject)
                {
                    return ThrowTypeError("iterator result is not an object");
                }

                if (GetProperty(step, "done").ToBooleanValue())
                {
                    record.Done = true;
                    frame.Delegate = null;
                    throw new JsReturnSignal(GetProperty(step, "value"));
                }

                break;
            }

            default:
            {
                // THE ONE PATH THAT IS THE ORDINARY PROTOCOL, so it is the ordinary helper: the
                // record's `next` is the function read once at acquisition, the sent value is
                // forwarded as its argument, and the inner iterator's own COMPLETION VALUE is what
                // `yield*` evaluates to - the half of delegation a loop written by hand forgets.
                if (!TryIterateNext(record, [sent], out var element, out var completed, wantsCompleted: true))
                {
                    frame.Delegate = null;
                    return completed;
                }

                frame.Delegating = true;
                frame.Sp = sp;
                frame.Pc = pc;
                frame.Suspended = true;
                return element;
            }
        }

        if (!step.IsObject)
        {
            return ThrowTypeError("iterator result is not an object");
        }

        if (GetProperty(step, "done").ToBooleanValue())
        {
            record.Done = true;
            frame.Delegate = null;
            return GetProperty(step, "value");
        }

        frame.Delegating = true;
        frame.Sp = sp;
        frame.Pc = pc;
        frame.Suspended = true;
        frame.Suspension = JsSuspension.Yield;
        return GetProperty(step, "value");
    }

    /// <summary>
    /// One turn of an ASYNC <c>yield*</c>: the same delegation loop, with every inner step awaited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a separate method from <see cref="Delegate"/> and not a flag through it, because
    /// the two differ in the one thing a method body cannot parameterise: WHERE IT MAY STOP.</b>
    /// A synchronous delegation runs from one <c>yield</c> to the next inside a single entry into
    /// the dispatch loop; an asynchronous one leaves the loop after every inner call, waits for a
    /// promise, and comes back at the same instruction. Threading a boolean through the
    /// synchronous version would have meant a suspension point in the middle of each of its four
    /// branches, and the branch that forgot one would have carried on synchronously with a promise
    /// in its hand.
    /// </para>
    /// <para>
    /// <b>Five re-entry points and one instruction, which is what
    /// <see cref="JsFrame.DelegateStage"/> exists to distinguish.</b> Stage zero is the ordinary
    /// one - the inner value has been yielded out and the caller has answered - and the other four
    /// are awaits: of what <c>next</c> or <c>throw</c> answered, of what <c>return</c> answered, of
    /// the close performed when the inner iterator has no <c>throw</c>, and of the RESUMPTION's own
    /// value when it has no <c>return</c>. That last one is the await a reader does not expect and
    /// the language performs twice: once on the way in, at the yield, and again here.
    /// </para>
    /// <para>
    /// <b>An await that REJECTS ends the delegation and raises in the outer body.</b> That is the
    /// difference between a rejection arriving here and a <c>throw</c> arriving at the yield: the
    /// second is a request from the consumer, which the inner iterator is offered first; the first
    /// is the inner iterator's own step having failed, and there is nothing left to offer it to.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=197F48
    // Broiler-Falsified-If: an inner step of an async `yield*` reaches the outer body unawaited, or a `return` or a `throw` that arrives while one is suspended is not offered to the inner iterator first
    // Broiler-Human:        PENDING
    private JsValue DelegateAsync(JsFrame frame, JsValue[] stack, ref int sp, int pc)
    {
        if (!frame.Delegating)
        {
            frame.Delegate = Realm.GetAsyncIterator(stack[--sp]);
            frame.ResumeMode = JsResumeMode.Next;
            frame.ResumeValue = JsValue.Undefined;
            frame.DelegateStage = DelegateStepFresh;
        }

        // THE FLAG IS CLEARED ON THE WAY IN AND SET AGAIN ONLY BY AN ACTUAL SUSPENSION, for the
        // reason `Delegate` records: every other way out of this method ends the delegation,
        // including the ways that leave by throwing.
        frame.Delegating = false;
        var record = frame.Delegate!;
        var stage = frame.DelegateStage;
        var mode = frame.ResumeMode;
        var carried = frame.ResumeValue;
        frame.ResumeMode = JsResumeMode.Next;
        frame.ResumeValue = JsValue.Undefined;
        frame.DelegateStage = DelegateStepFresh;
        JsValue step;

        if (stage != DelegateStepFresh)
        {
            if (mode == JsResumeMode.Throw)
            {
                record.Done = true;
                frame.Delegate = null;
                throw new JsThrow(carried, Render(carried));
            }

            switch (stage)
            {
                case DelegateAwaitingClose:

                    // THE CLOSE HAPPENED AND ITS ANSWER IS CHECKED, and then the protocol
                    // violation is reported anyway. The inner iterator was given its chance to
                    // clean up because the `throw` it is about to be told it does not implement
                    // ends the delegation; an error the clean-up raised has already left above.
                    frame.Delegate = null;

                    return carried.IsObject
                        ? ThrowTypeError("The iterator does not provide a 'throw' method.")
                        : ThrowTypeError("iterator result is not an object");

                case DelegateAwaitingReceived:
                    frame.Delegate = null;
                    throw new JsReturnSignal(carried);

                default:
                    step = carried;
                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case JsResumeMode.Throw:
                {
                    var thrower = GetProperty(record.Iterator, "throw");

                    if (thrower.IsNullish)
                    {
                        var closer = GetProperty(record.Iterator, "return");

                        if (closer.IsNullish)
                        {
                            record.Done = true;
                            frame.Delegate = null;
                            return ThrowTypeError("The iterator does not provide a 'throw' method.");
                        }

                        if (!closer.IsObject || !closer.AsObject().IsCallable)
                        {
                            frame.Delegate = null;
                            return ThrowTypeError("The iterator's return is not a function");
                        }

                        record.Done = true;

                        return SuspendDelegation(
                            frame,
                            Call(closer, record.Iterator, []),
                            sp,
                            pc,
                            DelegateAwaitingClose);
                    }

                    if (!thrower.IsObject || !thrower.AsObject().IsCallable)
                    {
                        frame.Delegate = null;
                        return ThrowTypeError("The iterator's throw is not a function");
                    }

                    return SuspendDelegation(
                        frame,
                        Call(thrower, record.Iterator, [carried]),
                        sp,
                        pc,
                        DelegateAwaitingStep);
                }

                case JsResumeMode.Return:
                {
                    var returner = GetProperty(record.Iterator, "return");

                    // AN INNER ITERATOR WITH NO `return` DOES NOT SWALLOW THE OUTER ONE, and the
                    // outer one's value is AWAITED A SECOND TIME on the way out. It was awaited
                    // once at the yield, by the unwrapping every async resumption goes through,
                    // and the language awaits it again here - which a program counting turns of
                    // the job queue can see, and which is why it is a suspension rather than a
                    // return raised from this line.
                    if (returner.IsNullish)
                    {
                        record.Done = true;

                        return SuspendDelegation(
                            frame, carried, sp, pc, DelegateAwaitingReceived);
                    }

                    if (!returner.IsObject || !returner.AsObject().IsCallable)
                    {
                        frame.Delegate = null;
                        return ThrowTypeError("The iterator's return is not a function");
                    }

                    return SuspendDelegation(
                        frame,
                        Call(returner, record.Iterator, [carried]),
                        sp,
                        pc,
                        DelegateAwaitingReturn);
                }

                default:
                    Charge(2);

                    // THE RECORD IS MARKED DONE FOR THE LENGTH OF THE STEP, exactly as the
                    // `for await` head marks it: an inner step that fails owes the inner iterator
                    // no `return`, and the mark is what stops one being sent.
                    record.Done = true;

                    return SuspendDelegation(
                        frame,
                        Call(record.Next, record.Iterator, [carried]),
                        sp,
                        pc,
                        DelegateAwaitingStep);
            }
        }

        if (!step.IsObject)
        {
            frame.Delegate = null;
            return ThrowTypeError("iterator result is not an object");
        }

        if (GetProperty(step, "done").ToBooleanValue())
        {
            record.Done = true;
            frame.Delegate = null;
            var completed = GetProperty(step, "value");

            // A DELEGATION THAT ENDED INSIDE A `return` IS STILL RETURNING. The inner iterator
            // answered a done step, so the value it carries is what the OUTER generator completes
            // with - not what the `yield*` evaluates to, because nothing is going to evaluate it.
            if (stage == DelegateAwaitingReturn)
            {
                throw new JsReturnSignal(completed);
            }

            return completed;
        }

        record.Done = false;

        return SuspendDelegation(
            frame, GetProperty(step, "value"), sp, pc, DelegateStepFresh);
    }

    /// <summary>Suspends the frame inside a delegation, at the stage it will re-enter at.</summary>
    /// <remarks>
    /// Stage zero is the yield of an inner value and every other stage is an await, which is the one
    /// place the <see cref="JsSuspension"/> the driver reads is decided for a delegation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D2339C
    // Broiler-Human:        PENDING
    private static JsValue SuspendDelegation(
        JsFrame frame, JsValue value, int sp, int pc, int stage)
    {
        frame.Delegating = true;
        frame.DelegateStage = stage;
        frame.Sp = sp;
        frame.Pc = pc;
        frame.Suspended = true;

        frame.Suspension = stage == DelegateStepFresh
            ? JsSuspension.Yield
            : JsSuspension.Await;

        return value;
    }

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

    /// <summary>Unpacks the argument Array a spread call built into the array a call takes.</summary>
    /// <remarks>
    /// The Array was built by this lowering and nothing else can reach it, so the dense elements
    /// are read directly. It is the one place in the iteration work where that is honest: the
    /// protocol already ran, when the spread appended.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=73F0A8
    // Broiler-Human:        PENDING
    private JsValue[] ArgumentsOf(JsValue packed)
    {
        var array = (JsArray)packed.AsObject();
        var count = (int)array.Length;

        if (count == 0)
        {
            return System.Array.Empty<JsValue>();
        }

        Charge((ulong)count);
        var arguments = new JsValue[count];

        for (var at = 0; at < count; at++)
        {
            var element = at < array.DenseCount ? array.DenseAt(at) : JsValue.Undefined;
            arguments[at] = element.IsEmpty ? JsValue.Undefined : element;
        }

        return arguments;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EAA946
    // Broiler-Human:        PENDING
    private static bool TryFindHandler(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit && pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
    }

    /// <summary>
    /// The object environment record that binds <paramref name="name"/> within the innermost
    /// <paramref name="limit"/> records, or <c>undefined</c> when none of them does.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS IS THE ONLY LOOKUP BY NAME IN THE SCOPE CHAIN, AND IT CAN REACH NOTHING BUT AN
    /// OBJECT A <c>with</c> PUT THERE.</b> A declarative record is a <c>JsValue</c> array with no
    /// names in it, so this walk has nothing to compare a name against and skips it; a name the
    /// objects do not have therefore falls through to the <c>(depth, slot)</c> address the lowering
    /// computed from the language's own scope rules, and to nothing else. That is what stops a
    /// <c>with</c> body reaching an enclosing function's binding by naming it.
    /// </para>
    /// <para>
    /// <b><paramref name="limit"/> is the lowering's, and it is why an outer <c>with</c> cannot
    /// shadow an inner declaration.</b> It counts the records between the reference and the binding
    /// the lowering resolved, so a record past that binding is never asked. Walking the whole chain
    /// would make <c>with (a) { function f() { var x; with (b) { x } } }</c> read <c>a.x</c>.
    /// </para>
    /// <para>
    /// <b>Nothing here is cached, and that is the behaviour rather than a missing optimisation.</b>
    /// Each mention of a name inside a <c>with</c> body runs this walk again, so an object that
    /// gains the property between two reads is read from on the second and not on the first, and an
    /// object that loses it goes back to the enclosing binding. An implementation that remembered
    /// where a name resolved last time would answer the first reading for both.
    /// </para>
    /// <para>
    /// <b><c>Symbol.unscopables</c> is consulted per record and per name</b>, which is where the
    /// specification consults it - inside <c>HasBinding</c> - and is why a name listed truthily
    /// there does not stop the walk. It is a property read on the guest's object and can therefore
    /// run a getter, which is charged like any other call this walk makes.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=E98FC8
    // Broiler-Falsified-If: this walk answers with anything but an object a `PushObjectScope` placed on the chain
    // Broiler-Human:        PENDING
    private JsValue ResolveName(
        System.Collections.Generic.List<JsEnvironment> scopes, int limit, string name)
    {
        var current = scopes[^1];

        for (var step = 0; step < limit && current is not null; step++)
        {
            Charge(1);

            if (current.Binding is { } bound && HasProperty(bound, name) && !Unscopable(bound, name))
            {
                return JsValue.Object(bound);
            }

            // A FUNCTION'S EVAL VARIABLES ARE ASKED AS A `with` OBJECT IS, without its
            // `Symbol.unscopables`: an own-property test of an object nobody else holds (JSeal V15).
            if (current.EvalVariables is { } introduced && introduced.HasOwnProperty(name))
            {
                return JsValue.Object(introduced);
            }

            current = current.Parent;
        }

        return JsValue.Undefined;
    }

    /// <summary>Whether <paramref name="bound"/> hides <paramref name="name"/> from a <c>with</c>.</summary>
    /// <remarks>
    /// <b>The blocklist is read off the object every time, prototype chain included.</b> It is what
    /// lets an Array put <c>values</c>, <c>keys</c> and <c>flat</c> on
    /// <c>Array.prototype[Symbol.unscopables]</c> so that <c>with (someArray) { values }</c> reaches
    /// an outer <c>values</c> rather than the method - which is the whole reason the Symbol was
    /// added to the language, and is a compatibility rule rather than a nicety.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=47A445
    // Broiler-Human:        PENDING
    private bool Unscopable(JsObject bound, string name)
    {
        var blocked = GetSymbol(JsValue.Object(bound), Realm.UnscopablesSymbol);
        return blocked.IsObject && GetProperty(blocked, name).ToBooleanValue();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=6F7A88
    // Broiler-Human:        PENDING
    private static JsEnvironment Slot(
        System.Collections.Generic.List<JsEnvironment> scopes, int depth, int index, out bool found)
    {
        var current = scopes[^1];

        for (var step = 0; step < depth; step++)
        {
            if (current.Parent is null)
            {
                found = false;
                return current;
            }

            current = current.Parent;
        }

        found = index < current.Slots.Length;
        return current;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B5B178
    // Broiler-Human:        PENDING
    private static void Binary(
        JsValue[] stack,
        ref int sp,
        JsOpcode opcode,
        System.Func<double, double, double> operation,
        JsEngine engine)
    {
        // BOTH OPERANDS LEAVE THE STACK BEFORE EITHER IS CONVERTED, AND THE LEFT IS CONVERTED FIRST.
        // ToNumeric can run guest code (valueOf, toString, Symbol.toPrimitive), so the order is
        // observable, and a throw from the left operand's conversion must leave the right one
        // unconverted. Two Numbers take the operation they always took; anything else is BigInt
        // arithmetic or the TypeError for mixing the two (JSeal B03).
        var rightValue = stack[--sp];
        var leftValue = stack[--sp];
        var left = engine.ToNumeric(leftValue);
        var right = engine.ToNumeric(rightValue);

        stack[sp++] = left.IsNumber && right.IsNumber
            ? JsValue.Number(operation(left.AsNumber(), right.AsNumber()))
            : engine.BigIntBinary(opcode, left, right);
    }

    /// <summary>The <c>+</c> operator, which is concatenation when either side is a String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=454988
    // Broiler-Human:        PENDING
    internal JsValue Add(JsValue left, JsValue right)
    {
        var primitiveLeft = ToPrimitive(left, "default");
        var primitiveRight = ToPrimitive(right, "default");

        if (primitiveLeft.IsString || primitiveRight.IsString)
        {
            return JsValue.String(ToStringValue(primitiveLeft) + ToStringValue(primitiveRight));
        }

        var numericLeft = ToNumeric(primitiveLeft);
        var numericRight = ToNumeric(primitiveRight);

        return numericLeft.IsNumber && numericRight.IsNumber
            ? JsValue.Number(numericLeft.AsNumber() + numericRight.AsNumber())
            : BigIntBinary(JsOpcode.Add, numericLeft, numericRight);
    }

    /// <summary>
    /// The six bitwise and shift operators: <c>ToNumeric</c> on both operands, left first, then the
    /// 32-bit Number operation or the BigInt one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1760EF
    // Broiler-Human:        PENDING
    internal JsValue Bitwise(JsOpcode opcode, JsValue leftValue, JsValue rightValue)
    {
        var left = ToNumeric(leftValue);
        var right = ToNumeric(rightValue);

        if (!left.IsNumber || !right.IsNumber)
        {
            return BigIntBinary(opcode, left, right);
        }

        var a = left.AsNumber();
        var b = right.AsNumber();

        return opcode switch
        {
            JsOpcode.BitwiseOr => JsValue.Number(JsValue.ToInt32(a) | JsValue.ToInt32(b)),
            JsOpcode.BitwiseAnd => JsValue.Number(JsValue.ToInt32(a) & JsValue.ToInt32(b)),
            JsOpcode.BitwiseXor => JsValue.Number(JsValue.ToInt32(a) ^ JsValue.ToInt32(b)),
            JsOpcode.ShiftLeft => JsValue.Number(JsValue.ToInt32(a) << (int)(JsValue.ToUint32(b) & 31)),
            JsOpcode.ShiftRight => JsValue.Number(JsValue.ToInt32(a) >> (int)(JsValue.ToUint32(b) & 31)),
            _ => JsValue.Number(JsValue.ToUint32(a) >> (int)(JsValue.ToUint32(b) & 31)),
        };
    }

    /// <summary>
    /// A binary numeric operator over two values <c>ToNumeric</c> has already produced, at least
    /// one of them a BigInt: the TypeError for mixing, or the BigInt operation (JSeal B03-B04).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>No operand is ever read as a Number here.</b> Mixing is refused before anything is
    /// computed, as the specification's <c>ApplyStringOrNumericBinaryOperator</c> refuses it, and
    /// <c>&gt;&gt;&gt;</c> is refused on two BigInts because <c>BigInt::unsignedRightShift</c> is a
    /// TypeError - a BigInt has no fixed width to fill with zeros.
    /// </para>
    /// <para>
    /// The errors the specification gives the arithmetic itself are raised here, before the
    /// operation runs: a zero divisor for <c>/</c> and <c>%</c> and a negative exponent for
    /// <c>**</c> are RangeErrors. A result wider than the realm's ceiling is a RangeError too
    /// (<see cref="BigIntResult"/>); the operations charge fuel before each step.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=346910
    // Broiler-Falsified-If: a Number operand is mixed into a BigInt result, a BigInt is read as a Number, a zero divisor or negative exponent answers anything but a RangeError, or >>> answers a value
    // Broiler-Human:        PENDING
    internal JsValue BigIntBinary(JsOpcode opcode, JsValue left, JsValue right)
    {
        if (!left.IsBigInt || !right.IsBigInt)
        {
            return ThrowTypeError("Cannot mix BigInt and other types, use explicit conversions");
        }

        var a = left.AsBigInt();
        var b = right.AsBigInt();

        switch (opcode)
        {
            case JsOpcode.Divide or JsOpcode.Remainder when b.IsZero:
                return ThrowRangeError("Division by zero");

            case JsOpcode.Exponent when b.Value.Sign < 0:
                return ThrowRangeError("Exponent must be non-negative");

            case JsOpcode.ShiftRightUnsigned:
                return ThrowTypeError("BigInts have no unsigned right shift, use >> instead");
        }

        return BigIntResult(opcode switch
        {
            JsOpcode.Add => JsBigInt.Add(a, b, ChargeFuel),
            JsOpcode.Subtract => JsBigInt.Subtract(a, b, ChargeFuel),
            JsOpcode.Multiply => JsBigInt.Multiply(a, b, ChargeFuel),
            JsOpcode.Divide => JsBigInt.Divide(a, b, ChargeFuel),
            JsOpcode.Remainder => JsBigInt.Remainder(a, b, ChargeFuel),
            JsOpcode.Exponent => JsBigInt.Power(a, b, ChargeFuel),
            JsOpcode.BitwiseAnd => JsBigInt.And(a, b, ChargeFuel),
            JsOpcode.BitwiseOr => JsBigInt.Or(a, b, ChargeFuel),
            JsOpcode.BitwiseXor => JsBigInt.Xor(a, b, ChargeFuel),
            JsOpcode.ShiftLeft => JsBigInt.ShiftLeft(a, b, ChargeFuel),
            JsOpcode.ShiftRight => JsBigInt.ShiftRight(a, b, ChargeFuel),
            _ => throw new System.InvalidOperationException("not a binary numeric operator: " + opcode),
        });
    }

    /// <summary>
    /// One step of an update expression: <c>x + 1</c> or <c>x - 1</c> for a Number, <c>x + 1n</c>
    /// or <c>x - 1n</c> for a BigInt, after a <c>ToNumeric</c> of anything else.
    /// </summary>
    /// <remarks>
    /// The specification's update is <c>Number::add(oldValue, 1)</c> or <c>BigInt::add(oldValue,
    /// 1n)</c> by the operand's type (and the same with subtract); mixing cannot arise, because the
    /// one is always of the operand's own type. The BigInt step is charged and bounded exactly as
    /// the <c>+</c> and <c>-</c> operators are.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=27D8C7
    // Broiler-Falsified-If: a BigInt operand is stepped through a double or answers a Number, or a Number operand answers anything but x + 1 or x - 1
    // Broiler-Human:        PENDING
    internal JsValue NumericStep(JsValue value, bool increment)
    {
        var numeric = ToNumeric(value);

        if (numeric.IsNumber)
        {
            return JsValue.Number(increment ? numeric.AsNumber() + 1 : numeric.AsNumber() - 1);
        }

        return BigIntResult(
            increment
                ? JsBigInt.Add(numeric.AsBigInt(), JsBigInt.One, ChargeFuel)
                : JsBigInt.Subtract(numeric.AsBigInt(), JsBigInt.One, ChargeFuel));
    }

    /// <summary>
    /// A computed BigInt as a value, or the RangeError for one wider than the realm's ceiling.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=FAB1EA
    // Broiler-Falsified-If: an operation whose result is past JsBigInt.MaximumBits answers a value or escapes as anything but a RangeError
    // Broiler-Human:        PENDING
    internal JsValue BigIntResult(JsBigInt? result) =>
        result is null
            ? ThrowRangeError(
                "Maximum BigInt size exceeded: the result would be wider than " +
                JsBigInt.MaximumBits.ToString(System.Globalization.CultureInfo.InvariantCulture) + " bits")
            : JsValue.BigInt(result);

    /// <summary>The four relational operators, through one abstract comparison.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A7E258
    // Broiler-Human:        PENDING
    internal bool Relational(JsOpcode opcode, JsValue left, JsValue right)
    {
        // THE ORDER OF EVALUATION IS THE SPECIFICATION'S: all four operators convert the LEFT
        // operand first. `>` and `>=` swap the operands of IsLessThan but pass LeftFirst = false,
        // which converts that call's second argument - the source's left operand - first. It is
        // observable through a valueOf with a side effect, and it is the kind of thing only a
        // conformance suite ever notices.
        var first = ToPrimitive(left, "number");
        var second = ToPrimitive(right, "number");

        if (first.IsString && second.IsString)
        {
            var firstText = first.AsString();
            var secondText = second.AsString();

            // An ordinal comparison stops at the first difference, so the shorter operand bounds
            // the work whatever the longer one holds.
            ChargeText(System.Math.Min(firstText.Length, secondText.Length));

            var order = string.CompareOrdinal(firstText, secondText);

            return opcode switch
            {
                JsOpcode.LessThan => order < 0,
                JsOpcode.LessThanOrEqual => order <= 0,
                JsOpcode.GreaterThan => order > 0,
                _ => order >= 0,
            };
        }

        // A BIGINT ON EITHER SIDE IS COMPARED EXACTLY (JSeal B05), never through a double:
        // `9007199254740993n > 9007199254740992` is true, which no rounding of the left operand
        // could answer. An undefined comparison - a NaN, or a String that is not an integer - is
        // false for all four operators, as the Number path's IEEE comparison already makes it.
        if (first.IsBigInt || second.IsBigInt)
        {
            return BigIntOrder(first, second) is { } order && opcode switch
            {
                JsOpcode.LessThan => order < 0,
                JsOpcode.LessThanOrEqual => order <= 0,
                JsOpcode.GreaterThan => order > 0,
                _ => order >= 0,
            };
        }

        var a = ToNumber(first);
        var b = ToNumber(second);

        return opcode switch
        {
            JsOpcode.LessThan => a < b,
            JsOpcode.LessThanOrEqual => a <= b,
            JsOpcode.GreaterThan => a > b,
            _ => a >= b,
        };
    }

    /// <summary>
    /// The order of two primitives at least one of which is a BigInt and which are not both
    /// Strings: negative, zero or positive, or <see langword="null"/> where the specification's
    /// <c>IsLessThan</c> answers <c>undefined</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A String beside a BigInt is read with <c>StringToBigInt</c>, not <c>ToNumber</c></b>, so
    /// <c>'9007199254740993' &gt; 9007199254740992n</c> is true; a String outside the grammar is
    /// undefined, and one spelling a value wider than the realm's ceiling is ordered by its sign,
    /// which is exact because no value in the realm is that wide.
    /// </para>
    /// <para>
    /// Otherwise both sides take <c>ToNumeric</c>, which refuses a Symbol, and a Number beside a
    /// BigInt is compared by <see cref="JsBigInt.CompareToNumber"/>: a NaN is undefined and an
    /// infinity is beyond every BigInt.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=309DC2
    // Broiler-Falsified-If: a BigInt and a Number or String are ordered other than by their mathematical values, or either is read through a double
    // Broiler-Human:        PENDING
    private int? BigIntOrder(JsValue first, JsValue second)
    {
        if (first.IsBigInt && second.IsString)
        {
            if (!JsBigInt.TryParseStringInteger(second.AsString(), ChargeFuel, out var parsed, out var wide))
            {
                return null;
            }

            return parsed is null ? -wide : CompareBigInts(first.AsBigInt(), parsed);
        }

        if (first.IsString && second.IsBigInt)
        {
            if (!JsBigInt.TryParseStringInteger(first.AsString(), ChargeFuel, out var parsed, out var wide))
            {
                return null;
            }

            return parsed is null ? wide : CompareBigInts(parsed, second.AsBigInt());
        }

        var a = ToNumeric(first);
        var b = ToNumeric(second);

        if (a.IsBigInt && b.IsBigInt)
        {
            return CompareBigInts(a.AsBigInt(), b.AsBigInt());
        }

        return a.IsBigInt ? OrderAgainstNumber(a.AsBigInt(), b.AsNumber()) : -OrderAgainstNumber(b.AsBigInt(), a.AsNumber());
    }

    /// <summary>The order of two BigInts, charged on the narrower one's words.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=B271B5
    // Broiler-Human:        PENDING
    private int CompareBigInts(JsBigInt left, JsBigInt right)
    {
        ChargeText(System.Math.Min(left.Words, right.Words));
        return left.Value.CompareTo(right.Value);
    }

    /// <summary>
    /// The order of a BigInt against a Number, or <see langword="null"/> for a NaN; an infinity is
    /// beyond every BigInt.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=5CB7FA
    // Broiler-Falsified-If: a BigInt is ordered against a Number other than by their mathematical values
    // Broiler-Human:        PENDING
    private int? OrderAgainstNumber(JsBigInt value, double number) =>
        double.IsNaN(number) ? null
        : double.IsPositiveInfinity(number) ? -1
        : double.IsNegativeInfinity(number) ? 1
        : value.CompareToNumber(number, ChargeFuel);

    /// <summary>
    /// Charges an equality comparison for the text it may have to read.
    /// </summary>
    /// <remarks>
    /// Only the string-against-string case can read anything: every other pair of the same type
    /// compares a word or a reference, and two different types answer without looking. Equality
    /// answers immediately when the lengths differ, so like the relational comparison beside it the
    /// shorter operand bounds the work.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=8F73E3
    // Broiler-Falsified-If: comparing two long equal strings costs what comparing two short ones costs
    // Broiler-Human:        PENDING
    private void ChargeComparison(JsValue left, JsValue right)
    {
        if (left.IsString && right.IsString)
        {
            ChargeText(System.Math.Min(left.AsString().Length, right.AsString().Length));
        }

        // Two BigInts compare word by word, and like two Strings the shorter bounds the work.
        if (left.IsBigInt && right.IsBigInt)
        {
            ChargeText(System.Math.Min(left.AsBigInt().Words, right.AsBigInt().Words));
        }
    }

    /// <summary>The abstract equality comparison, <c>==</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8DCC17
    // Broiler-Human:        PENDING
    internal bool LooselyEquals(JsValue left, JsValue right)
    {
        if (left.Type == right.Type)
        {
            ChargeComparison(left, right);
            return left.StrictlyEquals(right);
        }

        if (left.IsNullish && right.IsNullish)
        {
            return true;
        }

        // AN [[IsHTMLDDA]] OBJECT IS LOOSELY EQUAL TO BOTH NULLISH VALUES (Annex B.3.6.2), and it is
        // the only object that is.
        if (left.IsNullish || right.IsNullish)
        {
            return (left.IsObject && left.AsObject().IsHtmlDda) || (right.IsObject && right.AsObject().IsHtmlDda);
        }

        if (left.Type == JsType.Number && right.Type == JsType.String)
        {
            return left.AsNumber() == ToNumber(right);
        }

        if (left.Type == JsType.String && right.Type == JsType.Number)
        {
            return ToNumber(left) == right.AsNumber();
        }

        // A BIGINT BESIDE A STRING READS THE STRING WITH `StringToBigInt` (JSeal B05): `1n == '1'`
        // is true, `1n == '1.0'` is false because the text is not an integer, and a text spelling
        // a value wider than the realm's ceiling equals nothing the realm can hold.
        if (left.IsBigInt && right.Type == JsType.String)
        {
            return BigIntEqualsText(left.AsBigInt(), right.AsString());
        }

        if (left.Type == JsType.String && right.IsBigInt)
        {
            return BigIntEqualsText(right.AsBigInt(), left.AsString());
        }

        if (left.Type == JsType.Boolean)
        {
            return LooselyEquals(JsValue.Number(left.AsBoolean() ? 1 : 0), right);
        }

        if (right.Type == JsType.Boolean)
        {
            return LooselyEquals(left, JsValue.Number(right.AsBoolean() ? 1 : 0));
        }

        // AN OBJECT BESIDE ANY OF THE FOUR PRIMITIVES THE SPECIFICATION LISTS IS CONVERTED ONCE:
        // String, Number, BigInt and Symbol. The Symbol arm was missing, so `sym == Object(sym)`
        // answered false (Test262 `equals/coerce-symbol-to-prim-return-prim.js`).
        if (left.IsObject && right.Type is JsType.Number or JsType.String or JsType.BigInt or JsType.Symbol)
        {
            return LooselyEquals(ToPrimitive(left, "default"), right);
        }

        if (right.IsObject && left.Type is JsType.Number or JsType.String or JsType.BigInt or JsType.Symbol)
        {
            return LooselyEquals(left, ToPrimitive(right, "default"));
        }

        // A BIGINT BESIDE A NUMBER IS EQUAL ONLY TO THE SAME INTEGER, compared exactly: a NaN and
        // an infinity equal no BigInt, and `9007199254740993n == 9007199254740992` is false.
        if (left.IsBigInt && right.IsNumber)
        {
            return OrderAgainstNumber(left.AsBigInt(), right.AsNumber()) == 0;
        }

        if (left.IsNumber && right.IsBigInt)
        {
            return OrderAgainstNumber(right.AsBigInt(), left.AsNumber()) == 0;
        }

        return false;
    }

    /// <summary>Whether a String spells exactly this BigInt under <c>StringToBigInt</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=783A53
    // Broiler-Falsified-If: a String that is not an integer, or spells another integer, is answered equal
    // Broiler-Human:        PENDING
    private bool BigIntEqualsText(JsBigInt value, string text) =>
        JsBigInt.TryParseStringInteger(text, ChargeFuel, out var parsed, out _) &&
        parsed is not null &&
        CompareBigInts(value, parsed) == 0;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D3DBFB
    // Broiler-Human:        PENDING
    private bool InstanceOf(JsValue left, JsValue right)
    {
        // `Symbol.hasInstance` COMES FIRST AND IS CONSULTED ON ANY OBJECT, callable or not. That
        // ordering is what lets a plain object answer `instanceof` at all, and checking callability
        // before it would refuse the one case the Symbol exists for.
        if (right.IsObject && TryGetSymbolMethod(right, Realm.HasInstanceSymbol, out var custom))
        {
            return Call(custom, right, [left]).ToBooleanValue();
        }

        if (!right.IsObject || !right.AsObject().IsCallable)
        {
            ThrowTypeError("Right-hand side of 'instanceof' is not callable");
        }

        if (right.AsObject() is JsBoundFunction bound)
        {
            return InstanceOf(left, JsValue.Object(bound.Target));
        }

        if (!left.IsObject)
        {
            return false;
        }

        var prototype = GetProperty(right, "prototype");

        if (!prototype.IsObject)
        {
            ThrowTypeError("Function has non-object prototype in instanceof");
        }

        var target = prototype.AsObject();
        var walk = left.AsObject().Prototype;

        while (walk is not null)
        {
            if (ReferenceEquals(walk, target))
            {
                return true;
            }

            walk = walk.Prototype;
        }

        return false;
    }

    /// <summary>Reads an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=383EFA
    // Broiler-Human:        PENDING
    internal JsValue GetIndexed(JsValue target, JsValue key)
    {
        // THE BASE IS CHECKED BEFORE THE KEY IS CONVERTED, and the order is observable. The
        // language builds a Reference from `base[expr]` WITHOUT converting the key, and converts it
        // where the reference is read - so `null[{ toString(){ throw x; } }]` is the `TypeError`
        // about the base and never the exception the conversion would have raised. A key that needs
        // no user code is still named in the message, because naming it costs nothing and runs
        // nothing.
        if (target.IsNullish && !key.IsSymbol && key.Type is not JsType.String and not JsType.Number)
        {
            ThrowTypeError(
                "Cannot read properties of " +
                    (target.Type == JsType.Null ? "null" : "undefined"));
        }

        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < array.DenseCount)
            {
                var element = array.DenseAt(at);

                // A HOLE IS NOT AN ANSWER. It may be a hole, or it may be a slot the array vacated
                // when the element was given attributes it could not carry, in which case the value
                // is in the ordinary map and the general path finds it.
                if (!element.IsEmpty)
                {
                    return element;
                }
            }
        }

        if (target.IsString && key.Type == JsType.Number)
        {
            var text = target.AsString();
            var number = key.AsNumber();
            var at = (int)number;

            if (at == number && at >= 0 && at < text.Length)
            {
                return JsValue.String(text[at].ToString());
            }
        }

        if (key.IsSymbol)
        {
            return GetSymbol(target, key.AsSymbol());
        }

        return GetProperty(target, ToPropertyKey(key));
    }

    /// <summary>Reads a Symbol-keyed property, walking the prototype chain the same way.</summary>
    /// <remarks>
    /// It is a second walk rather than a widened one because the key type differs all the way down:
    /// the storage is a separate table, and a String key and a Symbol key can never collide, so
    /// there is nothing for the two walks to agree about.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DCEDE5
    // Broiler-Human:        PENDING
    internal JsValue GetSymbol(JsValue baseValue, JsSymbol key)
    {
        if (baseValue.IsNullish)
        {
            return ThrowTypeError(
                "Cannot read properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (reading a Symbol-keyed property)");
        }

        var current = baseValue.IsObject ? baseValue.AsObject() : PrototypeFor(baseValue);

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WALK OBEYS, and for the same reason.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyGet(JsValue.Symbol(key), baseValue);
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (!property.IsAccessor)
                {
                    return property.Value;
                }

                return property.Getter is null
                    ? JsValue.Undefined
                    : Call(JsValue.Object(property.Getter), baseValue, System.Array.Empty<JsValue>());
            }

            current = current.Prototype;
        }

        return JsValue.Undefined;
    }

    /// <summary>Writes a Symbol-keyed property.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=E3C0FB
    // Broiler-Human:        PENDING
    internal void SetSymbol(JsValue baseValue, JsSymbol key, JsValue value, bool strict)
    {
        if (baseValue.IsNullish)
        {
            ThrowTypeError(
                "Cannot set properties of " + (baseValue.Type == JsType.Null ? "null" : "undefined") +
                " (setting a Symbol-keyed property)");

            return;
        }

        // A PRIMITIVE BASE WALKS ITS WRAPPER'S PROTOTYPE, as the String write does: a Symbol-keyed
        // setter inherited from `Number.prototype` runs with the primitive as `this`. Only a write
        // that would have to create or change an own property on the primitive is refused.
        var target = baseValue.AsObjectOrNull();
        var current = target ?? PrototypeFor(baseValue);

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WRITE OBEYS, refusal and mode included.
            if (current is JsProxy proxy)
            {
                if (!proxy.ProxySet(JsValue.Symbol(key), value, baseValue) && strict)
                {
                    ThrowTypeError("Cannot assign to a read only Symbol-keyed property");
                }

                return;
            }

            if (current.TryGetOwnSymbol(key, out var property))
            {
                if (property.IsAccessor)
                {
                    if (property.Setter is null)
                    {
                        if (strict)
                        {
                            ThrowTypeError("Cannot set a Symbol-keyed property which has only a getter");
                        }

                        return;
                    }

                    Call(JsValue.Object(property.Setter), baseValue, [value]);
                    return;
                }

                if (!property.Writable)
                {
                    if (strict)
                    {
                        ThrowTypeError("Cannot assign to a read only Symbol-keyed property");
                    }

                    return;
                }

                // AN OWN WRITABLE PROPERTY KEEPS ITS ATTRIBUTES AND TAKES ONLY THE VALUE. Writing
                // the default attribute set here, as this once did, made a sealed property
                // configurable again and a non-enumerable one enumerable on the first assignment.
                if (ReferenceEquals(current, target))
                {
                    property.Value = value;
                    target.SetOwnSymbol(key, property);
                    return;
                }

                break;
            }

            current = current.Prototype;
        }

        if (target is null)
        {
            if (strict)
            {
                ThrowTypeError("Cannot create a Symbol-keyed property on a primitive");
            }

            return;
        }

        // THE SAME EXTENSIBILITY DECISION THE STRING WRITE MAKES. A Symbol key is a new own
        // property like any other, so a frozen, sealed or merely non-extensible object refuses it:
        // silently in sloppy code and as a TypeError in strict code.
        if (!target.Extensible)
        {
            if (strict)
            {
                ThrowTypeError("Cannot add a Symbol-keyed property, object is not extensible");
            }

            return;
        }

        target.SetOwnSymbol(key, JsProperty.Data(value, JsPropertyAttributes.Default));
    }

    /// <summary>Whether a Symbol-keyed property is reachable from <paramref name="start"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=49964F
    // Broiler-Human:        PENDING
    internal bool HasSymbol(JsObject start, JsSymbol key)
    {
        var current = start;

        while (current is not null)
        {
            // THE SAME WHOLE-OPERATION RULE THE STRING WALK OBEYS, and for the same reason.
            if (current is JsProxy proxy)
            {
                return proxy.ProxyHas(JsValue.Symbol(key));
            }

            if (current.TryGetOwnSymbol(key, out _))
            {
                return true;
            }

            current = current.Prototype;
        }

        return false;
    }

    /// <summary>Writes an indexed property, with the fast path an Array element deserves.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DAF660
    // Broiler-Human:        PENDING
    internal void SetIndexed(JsValue target, JsValue key, JsValue value, bool strict)
    {
        // The same ordering the read half performs, for the same reason: a write through a
        // reference whose base is nullish is a `TypeError` about the base, decided before anything
        // the key's conversion could run.
        if (target.IsNullish && !key.IsSymbol && key.Type is not JsType.String and not JsType.Number)
        {
            ThrowTypeError(
                "Cannot set properties of " +
                    (target.Type == JsType.Null ? "null" : "undefined"));
        }

        if (target.IsObject && target.AsObject() is JsArray array && key.Type == JsType.Number)
        {
            var number = key.AsNumber();
            var at = (int)number;

            // THE FAST PATH IS FOR AN ELEMENT THAT IS STILL AN ELEMENT. A dense slot the array
            // vacated is a property with attributes living in the ordinary map, and writing the
            // slot would step straight over a `writable: false` that somebody asked for - which is
            // what made a frozen array assignable while reporting itself frozen. Appending is a
            // fast path too, and only while the array is extensible.
            if (at == number && at >= 0)
            {
                // A CLOSED LENGTH REFUSES BEFORE EITHER FAST PATH IS TAKEN, and in strict code the
                // refusal is a TypeError. The object model drops such a write silently because it
                // cannot know the mode; this is where the mode is known.
                if (!array.LengthWritable && at >= array.Length)
                {
                    if (strict)
                    {
                        ThrowTypeError(
                            "Cannot add property " + at.ToString(
                                System.Globalization.CultureInfo.InvariantCulture) +
                            ", object is not extensible");
                    }

                    return;
                }

                if (at < array.DenseCount)
                {
                    if (!array.DenseAt(at).IsEmpty)
                    {
                        array.SetIndex((uint)at, value);
                        return;
                    }
                }
                else if (array.Extensible && at == array.Length &&
                         !ChainMayAnswerIndex(array.Prototype, at))
                {
                    array.SetIndex((uint)at, value);
                    return;
                }
            }
        }

        if (key.IsSymbol)
        {
            SetSymbol(target, key.AsSymbol(), value, strict);
            return;
        }

        SetProperty(target, ToPropertyKey(key), value, strict);
    }

    /// <summary>
    /// Whether anything on a prototype chain could answer a write to index <paramref name="at"/>
    /// before it lands on the Array that starts the chain.
    /// </summary>
    /// <remarks>
    /// <b>An append is <c>OrdinarySet</c>, and <c>OrdinarySet</c> walks the chain.</b> An inherited
    /// setter at the index runs instead of the append, and an inherited read-only property refuses
    /// it; the append fast path stepped over both, so <c>Array.prototype</c> defining a setter at
    /// <c>"0"</c> never saw <c>[].push(1)</c>. A Proxy or a typed array on the chain answers the
    /// write itself and is always sent to the full walk; any other prototype is asked only whether
    /// it holds the key, and the ordinary chain - <c>Array.prototype</c> then
    /// <c>Object.prototype</c> - holds none.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=35F4F3
    // Broiler-Human:        PENDING
    private static bool ChainMayAnswerIndex(JsObject? prototype, int at)
    {
        string? key = null;

        for (var current = prototype; current is not null; current = current.Prototype)
        {
            if (current is JsProxy or JsTypedArray)
            {
                return true;
            }

            key ??= JsNumberFormat.ToUintString((uint)at);

            if (current.TryGetOwnProperty(key, out _))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The specification's <c>CreateListFromArrayLike</c> narrowed to the two kinds of property key.
    /// </summary>
    /// <remarks>
    /// <b>An array-like and not an iterable</b>, which is the same choice <c>Reflect.apply</c> makes
    /// and for the same reason: the specification says so, and a handler returning a Set would be
    /// returning something with no <c>length</c>. <b>The element check is not politeness either</b> —
    /// every caller of this treats what comes back as an own key, and a Number in the list would
    /// become a key nothing on the target could ever match, so an <c>ownKeys</c> trap that answered
    /// <c>[0]</c> would silently report a property called neither <c>0</c> nor anything else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DE8552
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.List<JsValue> ProxyKeyList(JsValue list)
    {
        if (!list.IsObject)
        {
            ThrowTypeError("the 'ownKeys' trap answered a value that is not an object");
        }

        var length = JsValue.ToInteger(ToNumber(GetProperty(list, "length")));

        if (length > JsRealm.ReflectArgumentCeiling)
        {
            ThrowRangeError("the 'ownKeys' trap answered a list longer than this profile admits");
        }

        var collected = new System.Collections.Generic.List<JsValue>();

        for (var at = 0; at < length; at++)
        {
            Charge(1);
            var element = GetIndexed(list, JsValue.Number(at));

            if (!element.IsSymbol && element.Type != JsType.String)
            {
                ThrowTypeError("the 'ownKeys' trap answered a key that is neither a String nor a Symbol");
            }

            collected.Add(element);
        }

        return collected;
    }

    /// <summary>Renders a thrown value for a host that has to describe it in one line.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1C82D0
    // Broiler-Human:        PENDING
    internal string Render(JsValue value)
    {
        if (!value.IsObject)
        {
            return value.IsString ? value.AsString() : ToStringValue(value);
        }

        var name = GetProperty(value, "name");
        var message = GetProperty(value, "message");

        if (!name.IsNullish || !message.IsNullish)
        {
            var head = name.IsNullish ? "Error" : ToStringValue(name);
            var tail = message.IsNullish ? string.Empty : ToStringValue(message);
            return tail.Length == 0 ? head : head + ": " + tail;
        }

        return ToStringValue(value);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C54D2D
    // Broiler-Human:        PENDING
    private static ushort U16(byte[] code, int at) => (ushort)(code[at + 1] | (code[at + 2] << 8));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B2766
    // Broiler-Human:        PENDING
    private static uint U32(byte[] code, int at) => (uint)(
        code[at + 1] | (code[at + 2] << 8) | (code[at + 3] << 16) | (code[at + 4] << 24));
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
