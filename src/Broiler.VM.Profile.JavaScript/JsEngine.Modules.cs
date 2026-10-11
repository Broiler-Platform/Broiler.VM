// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           0
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    High
// Criteria:         8/8
// Resource impact:  5/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>
    /// The instance this realm holds under <paramref name="key"/>, or <see langword="null"/> when it
    /// has linked no module of that key (JSeal I11-upstream).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A80ADE
    // Broiler-Human:        PENDING
    internal JsModuleInstance? FindModuleInstance(string key) => instanced.TryGetValue(key, out var instance) ? instance : null;

    /// <summary>The instances of one artifact's modules, instantiating them if it has not.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=870296
    // Broiler-Human:        PENDING
    private JsModuleInstance[] Graph(JsProgram program) => graphs.TryGetValue(program, out var instances) ? instances : Instantiate(program);

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
    private JsValue RunModuleGraph(JsProgram program, int root) => Evaluated(program, root).Completion;

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=7F2D10
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
                    engine.EnqueueJob(JsValue.Object(engine.Realm.Native("", 0, (inner, thisValue, arguments) => throw new JsThrow(value, inner.Render(value)))), []);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=89D00D
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

            Execute(program, (int)program.Modules[index].InitialiserUnit, instances[index].Environment, JsValue.Undefined, [], null, JsValue.Undefined, null, null, program.Modules[index].Key);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=AC10E7
    // Broiler-Falsified-If: a module body runs twice in one realm, runs before a module it requests has finished, or a module that does not depend on an awaiting module waits for it
    // Broiler-Human:        PENDING
    private JsPromiseObject EvaluateModule(JsModuleInstance module)
    {
        if (module.State is JsModuleState.EvaluatingAsync or JsModuleState.Evaluated && module.CycleRoot is { } root)
        {
            module = root;
        }

        if (module.TopLevelCapability is { } existing)
        {
            return existing;
        }

        var stack = new List<JsModuleInstance>();
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=A87EA4
    // Broiler-Falsified-If: a module body runs before every module it requests has run or started, or a module waiting on async dependencies runs before they finish
    // Broiler-Human:        PENDING
    private bool InnerModuleEvaluation(JsModuleInstance module, List<JsModuleInstance> stack, ref int index, out JsValue failure)
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
                module.DfsAncestorIndex = Math.Min(
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7561E1
    // Broiler-Human:        PENDING
    private bool ExecuteModule(JsModuleInstance module, out JsValue failure)
    {
        failure = JsValue.Undefined;

        try
        {
            module.Completion = Execute(module.Program, (int)module.Program.Modules[module.Index].BodyUnit, module.Environment, JsValue.Undefined, [], null, JsValue.Undefined, null, null, module.Program.Modules[module.Index].Key);
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=737726
    // Broiler-Human:        PENDING
    private void GatherAvailableAncestors(JsModuleInstance module, List<JsModuleInstance> released, HashSet<JsModuleInstance> seen)
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=665B2E
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

        var released = new List<JsModuleInstance>();
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=5D2DE4
    // Broiler-Human:        PENDING
    private JsPromiseObject StartAsyncModule(JsProgram program, JsModuleRecord record, JsModuleInstance instance)
    {
        var body = new JsScriptFunction(Realm, Realm.FunctionPrototype, program, (int)record.BodyUnit, instance.Environment)
        {
            ScriptOrModule = record.Key,
        };

        var frame = new JsFrame(program, (int)record.BodyUnit, instance.Environment, JsValue.Undefined, [], body);

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

                var outcome = capabilities.InvokeBytes(JavaScriptProfile.ResolveBindingIndex, new VmBytes(request), out _);

                if (outcome != VmHostCallOutcome.Completed)
                {
                    ThrowTypeError(
                        "this composition does not resolve '" + record.RequestSpecifiers[index] +
                        "' from '" + record.Key + "' to '" + target.Key + "'");
                }
            }
        }
    }
}
