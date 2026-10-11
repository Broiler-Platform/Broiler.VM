// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  5/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=5; Fingerprint=6B487A
    // Broiler-Falsified-If: a program whose form differs from the engine's reaches ExecuteCore or emitted code
    // Broiler-Human:        PENDING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private JsValue Execute(JsProgram program, int unitIndex, JsEnvironment? environment, JsValue thisValue, JsValue[] actualArguments, JsScriptFunction? self,
        JsValue newTarget, JsCell? thisBinding, JsFrame? frame, string? referrer = null, bool tailCalls = false)
    {
        if ((program.NativeCode.Length != 0) != nativeForm || program.NativeValueForm != valueForm)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "a program of the other output form reached this engine");
        }

        // THE FRAME'S SCRIPT OR MODULE, SET FOR ITS WHOLE LIFE AND RESTORED WHEN IT ENDS, however it
        // ends (JSD-0024 section 20): the one a caller states, else the function's own.
        var outerReferrer = activeReferrer;
        activeReferrer = referrer ?? self?.ScriptOrModule ?? string.Empty;

        // THE FRAME'S SITE, which an error made while it runs reads its place from (JSD-0038).
        PushSite(program, unitIndex, self, activeReferrer, newTarget.Type != JsType.Undefined && !program.Functions[unitIndex].IsArrow);

        try
        {
            return nativeForm
                ? valueForm
                    ? RunValue(
                        program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
                        thisBinding, frame)
                    : RunNative(
                        program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
                        thisBinding, frame)
                : ExecuteCore<JsInterpreted>(
                    program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
                    thisBinding, frame, null, tailCalls);
        }
        finally
        {
            PopSite();
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=83AA8A
    // Broiler-Falsified-If: an instantiation over a per-opcode step mode runs more or fewer than one charged instruction per call, the block instantiation stops anywhere but at the first boundary after its first instruction at which JsBaselineBlocks.StopsAfter holds, or the interpreted instantiation behaves differently from the loop before it was made generic
    // Broiler-Human:        PENDING
    internal JsValue ExecuteCore<TMode>(JsProgram program, int unitIndex, JsEnvironment? environment, JsValue thisValue, JsValue[] actualArguments, JsScriptFunction? self,
        JsValue newTarget, JsCell? thisBinding, JsFrame? frame, JsNativeActivation? act, bool tailCalls = false) where TMode : struct, IJsExecutionMode
    {
        var unit = program.Functions[unitIndex];
        var code = program.Code;
        var constants = program.Constants;
        var names = program.Names;
        JsValue[] stack;
        List<JsEnvironment> scopes;
        int sp;
        int pc;
        var abrupt = false;

        if (typeof(TMode) == typeof(JsInterpreted) || typeof(TMode) == typeof(JsNativeEntry))
        {
            stack = frame is null ? new JsValue[unit.MaxOperandStack + 1] : frame.Stack;

            scopes = frame is null
                ? new List<JsEnvironment>(4) { environment! }
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

        // WHETHER THIS FRAME MAY HAND A TAIL CALL TO THE LOOP THAT ENTERED IT (JSC-256): the
        // interpreter's strict call frame, entered by `Invoke` as a call. See `InTailPosition`.
        var tailing = typeof(TMode) == typeof(JsInterpreted) && tailCalls && strict &&
            frame is null && newTarget.Type == JsType.Undefined &&
            !unit.IsGenerator && !unit.IsAsync;

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

        // THE FRAME'S SITE, which every instruction run here writes itself into (JSD-0038). The
        // interpreter's is the one `Execute` took just before entering it; a step of emitted code
        // runs for the innermost frame, whose site is the innermost one too.
        var site = TopSite;

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

                    site.Pc = current;

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

                        // AN OBJECT ENVIRONMENT RECORD'S BINDING, read and written as the record
                        // does it rather than as a property access: the property is asked for again,
                        // and one a getter or `Symbol.unscopables` removed since the search is a
                        // ReferenceError in strict code (GetBindingValue, SetMutableBinding).
                        case JsOpcode.GetObjectBinding:
                            {
                                var name = names[U16(code, pc)];
                                var holder = stack[sp - 1].AsObject();

                                stack[sp - 1] = HasProperty(holder, name)
                                    ? GetProperty(stack[sp - 1], name)
                                    : strict
                                        ? ThrowReferenceError(name + " is not defined")
                                        : JsValue.Undefined;

                                pc += 3;
                                break;
                            }

                        case JsOpcode.SetObjectBinding:
                            {
                                var name = names[U16(code, pc)];
                                var value = stack[--sp];
                                var target = stack[--sp];

                                // THE PROPERTY IS ASKED FOR IN SLOPPY CODE TOO, because `stillExists` is
                                // step 1 of SetMutableBinding whatever the mode, and a proxy sees the
                                // question. Until 2026-10-04 sloppy code skipped it (JSC-258).
                                if (!HasProperty(target.AsObject(), name) && strict)
                                {
                                    ThrowReferenceError(name + " is not defined");
                                }

                                SetProperty(target, name, value, strict);
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
                                var values = new List<JsValue>();
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

                                // `ToObject` OF THE BASE, AS THE OPERATOR SAYS: a nullish base is a
                                // `TypeError` and a primitive is asked through its wrapper, so
                                // `delete "abc".length` is the refusal a String's own non-configurable
                                // `length` gives and not a `true` nobody checked (JSC-236).
                                var went = ToObject(target).DeleteOwnProperty(key);

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

                                // The base is converted before the key, as `delete` orders the two, so
                                // `delete undefined[k]` throws without running `k`'s `toString`.
                                var holder = ToObject(target);

                                // THE KEY IS CONVERTED ONCE, and the refusal names the converted key:
                                // converting it again for the message ran its `toString` a second time
                                // in strict code until 2026-10-04 (JSC-256).
                                var property = ToPropertyKeyValue(key);

                                var removed = property.IsSymbol
                                    ? holder.DeleteOwnSymbol(property.AsSymbol())
                                    : holder.DeleteOwnProperty(property.AsString());

                                if (!removed && strict)
                                {
                                    ThrowTypeError(
                                        "Cannot delete property '" +
                                        (property.IsSymbol
                                            ? property.AsSymbol().Rendered
                                            : property.AsString()) +
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
                                var key = ToPropertyKeyValue(stack[--sp]);
                                var host = stack[sp - 1].AsObject();

                                if (key.IsSymbol)
                                {
                                    DefineSymbolMember(host, key.AsSymbol(), member, code[pc + 1]);
                                }
                                else
                                {
                                    DefineMember(host, key.AsString(), member, code[pc + 1]);
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

                        // THE READ HALF OF A `super[k]` THAT IS READ AND THEN WRITTEN, in the order
                        // LoadSuperProperty keeps - this binding, base, key - and leaving the key it
                        // converted under the value. `super[k] += v`, `super[k]++` and
                        // `super[k] ||= v` then hand StoreSuperProperty a String or a Symbol, so the
                        // conversion `GetValue` performs runs once and the write uses its answer, as
                        // the reference keeps it (JSC-236).
                        case JsOpcode.LoadSuperPropertyKeepKey:
                            {
                                var receiver = thisBinding is null
                                    ? thisValue
                                    : ThisBinding(thisBinding);

                                var start = SuperBase(active);
                                var key = ToPropertyKeyValue(stack[sp - 1]);
                                stack[sp - 1] = key;

                                stack[sp++] = key.IsSymbol
                                    ? GetSymbolWithReceiver(start, key.AsSymbol(), receiver)
                                    : Lookup(start, key.AsString(), receiver);

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
                                var passed = argc == 0 ? [] : new JsValue[argc];

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
                                var arguments = argc == 0 ? [] : new JsValue[argc];

                                for (var at = argc - 1; at >= 0; at--)
                                {
                                    arguments[at] = stack[--sp];
                                }

                                var receiver = stack[--sp];
                                var callee = stack[--sp];

                                if (tailing && InTailPosition(program, unitIndex, current, pc + 2, callee))
                                {
                                    tail = (callee.AsObject() as JsScriptFunction, receiver, arguments);
                                    return default;
                                }

                                stack[sp++] = Call(callee, receiver, arguments);
                                pc += 2;
                                break;
                            }

                        case JsOpcode.CallEval:
                            {
                                var argc = code[pc + 1];
                                var arguments = argc == 0 ? [] : new JsValue[argc];

                                for (var at = argc - 1; at >= 0; at--)
                                {
                                    arguments[at] = stack[--sp];
                                }

                                var receiver = stack[--sp];
                                var callee = stack[--sp];

                                // THE SPELLING SAYS DIRECT; THE VALUE DECIDES WHETHER IT IS. A program
                                // may assign to the global `eval`, and a call to whatever it now holds
                                // is an ordinary call however it is written - in tail position too.
                                if (tailing && !Realm.IsEvalIntrinsic(callee) &&
                                    InTailPosition(program, unitIndex, current, pc + 2, callee))
                                {
                                    tail = (callee.AsObject() as JsScriptFunction, receiver, arguments);
                                    return default;
                                }

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
                                var arguments = argc == 0 ? [] : new JsValue[argc];

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

                                if (tailing && InTailPosition(program, unitIndex, current, pc + 1, callee))
                                {
                                    tail = (callee.AsObject() as JsScriptFunction, receiver, spread);
                                    return default;
                                }

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

                                var tested = ToPropertyKeyValue(left);

                                stack[sp++] = JsValue.Boolean(
                                    tested.IsSymbol
                                        ? HasSymbol(right.AsObject(), tested.AsSymbol())
                                        : HasProperty(right.AsObject(), tested.AsString()));

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

                        // A CALL WRITTEN AS AN ASSIGNMENT TARGET IN NON-STRICT CODE (JSP-7, JSC-239):
                        // the call has run, and the write it asks for has no reference to go
                        // through, which Annex B makes a ReferenceError here rather than an early
                        // error.
                        case JsOpcode.ThrowReferenceError:
                            ThrowReferenceError("this reference cannot be assigned to or deleted");
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

                        case JsOpcode.GetTemplateObjectWide:
                            {
                                var raw = ArgumentsOf(stack[--sp]);
                                var cooked = ArgumentsOf(stack[sp - 1]);
                                var strings = TemplateObject(
                                    program,
                                    pc,
                                    new ReadOnlySpan<JsValue>(cooked),
                                    new ReadOnlySpan<JsValue>(raw));

                                stack[sp - 1] = JsValue.Object(strings);
                                pc++;
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
                Land(scopes, stack, ref sp, ref pc, region, thrown.Value);

                // A VALUE STEP READS WHERE A LANDING WROTE, which can be below the instruction's own
                // inputs; the comparison folds, so the interpreter's instantiation carries no write.
                if (typeof(TMode) != typeof(JsInterpreted))
                {
                    act!.Landed = true;
                }
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

                if (typeof(TMode) != typeof(JsInterpreted))
                {
                    act!.Landed = true;
                }
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C54D2D
    // Broiler-Human:        PENDING
    private static ushort U16(byte[] code, int at) => (ushort)(code[at + 1] | (code[at + 2] << 8));

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=3B2766
    // Broiler-Human:        PENDING
    private static uint U32(byte[] code, int at) => (uint)(
        code[at + 1] | (code[at + 2] << 8) | (code[at + 3] << 16) | (code[at + 4] << 24));
}
