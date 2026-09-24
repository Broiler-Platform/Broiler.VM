// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   427
// Annotated:        427/427
// Exempt:           1
// Human-reviewed:   0/427
// IP risk:          Low
// Security risk:    Critical
// Criteria:         428/428
// Resource impact:  4/10 max
// Unverified:       427
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The value form's helper table: one entry point per opcode byte, each running exactly one instruction
/// of the dispatch loop over the decoding of its input words (JSD-0035 section 5, stage JSV-1).
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS THE ONE OTHER FILE WHOSE METHODS NATIVE CODE MAY CALL</b>, which is rule X3 extended to the
/// value form. The table is indexed by the opcode byte itself, is filled and checked by the static
/// constructor exactly as the baseline table is, lives in unmanaged memory for the process, and holds
/// addresses of entry points and no reference the collector traces. A byte no opcode takes holds an
/// entry point that answers a defect and touches nothing.
/// </para>
/// <para>
/// <b>EVERY ENTRY POINT IS A PER-OPCODE INSTANTIATION OF THE INTERPRETER'S OWN LOOP</b>, because at this
/// stage every instruction is a helper: each step type below names its opcode as a constant, so the
/// compiled step keeps one arm of the switch and its frame is that arm's, and a call nested under any
/// instruction sits under a small frame. The mode types are named <c>Arm{Opcode}</c> rather than the
/// baseline table's <c>Step{Opcode}</c>, so that no name rule X4 holds to the baseline table's own
/// members is declared anywhere else (rule X5 holds these to this table's). The step itself - the checks, the safepoint, the decoding of
/// the instruction's input words into the activation's stack, the arm, and the encoding of what the arm
/// left there - is <see cref="JsNativeActivation.StepValue{TMode}"/>, and this file adds nothing to it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=D64011
// Broiler-Falsified-If: an emitted call through this table reaches code other than one value step of the instruction its slot is named for or the refusing entry point
// Broiler-Human:        PENDING
internal static unsafe class JsValueHelpers
{
    /// <summary>The base address of the helper table, or zero if the table failed its check.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=FD6363
    // Broiler-Falsified-If: this is non-zero while a defined opcode's slot holds anything but that opcode's own entry point, or an undefined byte's slot holds anything but the refusing one
    // Broiler-Human:        PENDING
    internal static readonly nint Table;

    /// <summary>Fills the table, checks it, and publishes it.</summary>
    /// <remarks>
    /// <b>A static constructor and not a module initializer</b>, so the table is built the first time a
    /// value-form instance asks for it and never in a process that runs no value form.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=2BED28
    // Broiler-Falsified-If: the published table maps a defined opcode byte to an entry point built for another opcode, or an undefined byte to anything but the refusing entry point
    // Broiler-Human:        PENDING
    static JsValueHelpers()
    {
        var undefined = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Undefined;
        var slots = new nint[JsValueAbi.HelperSlots];
        System.Array.Fill(slots, undefined);
        slots[(int)JsOpcode.Nop] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Nop;
        slots[(int)JsOpcode.LoadUndefined] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadUndefined;
        slots[(int)JsOpcode.LoadNull] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadNull;
        slots[(int)JsOpcode.LoadTrue] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadTrue;
        slots[(int)JsOpcode.LoadFalse] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadFalse;
        slots[(int)JsOpcode.LoadConstant] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadConstant;
        slots[(int)JsOpcode.LoadThis] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadThis;
        slots[(int)JsOpcode.NewArguments] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&NewArguments;
        slots[(int)JsOpcode.LoadNewTarget] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadNewTarget;
        slots[(int)JsOpcode.LoadArgument] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadArgument;
        slots[(int)JsOpcode.RestArguments] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&RestArguments;
        slots[(int)JsOpcode.LoadScoped] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadScoped;
        slots[(int)JsOpcode.StoreScoped] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StoreScoped;
        slots[(int)JsOpcode.InitialiseScoped] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&InitialiseScoped;
        slots[(int)JsOpcode.LoadGlobal] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadGlobal;
        slots[(int)JsOpcode.StoreGlobal] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StoreGlobal;
        slots[(int)JsOpcode.LoadGlobalOrUndefined] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadGlobalOrUndefined;
        slots[(int)JsOpcode.PushScope] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&PushScope;
        slots[(int)JsOpcode.PopScope] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&PopScope;
        slots[(int)JsOpcode.CopyScope] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&CopyScope;
        slots[(int)JsOpcode.DeclareGlobal] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DeclareGlobal;
        slots[(int)JsOpcode.PushObjectScope] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&PushObjectScope;
        slots[(int)JsOpcode.ResolveName] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ResolveName;
        slots[(int)JsOpcode.NewObject] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&NewObject;
        slots[(int)JsOpcode.NewArray] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&NewArray;
        slots[(int)JsOpcode.GetProperty] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&GetProperty;
        slots[(int)JsOpcode.SetProperty] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SetProperty;
        slots[(int)JsOpcode.GetIndex] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&GetIndex;
        slots[(int)JsOpcode.SetIndex] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SetIndex;
        slots[(int)JsOpcode.DefineField] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DefineField;
        slots[(int)JsOpcode.DefineIndexed] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DefineIndexed;
        slots[(int)JsOpcode.DeleteProperty] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DeleteProperty;
        slots[(int)JsOpcode.DeleteIndex] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DeleteIndex;
        slots[(int)JsOpcode.DefineGetter] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DefineGetter;
        slots[(int)JsOpcode.DefineSetter] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DefineSetter;
        slots[(int)JsOpcode.DefineMethod] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DefineMethod;
        slots[(int)JsOpcode.LoadSuperProperty] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadSuperProperty;
        slots[(int)JsOpcode.StoreSuperProperty] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StoreSuperProperty;
        slots[(int)JsOpcode.ArrayAppend] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ArrayAppend;
        slots[(int)JsOpcode.Closure] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Closure;
        slots[(int)JsOpcode.Call] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Call;
        slots[(int)JsOpcode.Construct] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Construct;
        slots[(int)JsOpcode.Return] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Return;
        slots[(int)JsOpcode.ReturnUndefined] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ReturnUndefined;
        slots[(int)JsOpcode.CallEval] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&CallEval;
        slots[(int)JsOpcode.SuperCall] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SuperCall;
        slots[(int)JsOpcode.SuperCallForwarded] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SuperCallForwarded;
        slots[(int)JsOpcode.NewClass] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&NewClass;
        slots[(int)JsOpcode.ArrayHoles] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ArrayHoles;
        slots[(int)JsOpcode.SpreadArray] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SpreadArray;
        slots[(int)JsOpcode.SpreadObject] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SpreadObject;
        slots[(int)JsOpcode.CallSpread] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&CallSpread;
        slots[(int)JsOpcode.ConstructSpread] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ConstructSpread;
        slots[(int)JsOpcode.SuperCallSpread] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SuperCallSpread;
        slots[(int)JsOpcode.SetPrototypeLiteral] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&SetPrototypeLiteral;
        slots[(int)JsOpcode.Add] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Add;
        slots[(int)JsOpcode.Subtract] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Subtract;
        slots[(int)JsOpcode.Multiply] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Multiply;
        slots[(int)JsOpcode.Divide] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Divide;
        slots[(int)JsOpcode.Remainder] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Remainder;
        slots[(int)JsOpcode.Exponent] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Exponent;
        slots[(int)JsOpcode.Negate] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Negate;
        slots[(int)JsOpcode.ToNumber] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ToNumber;
        slots[(int)JsOpcode.Not] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Not;
        slots[(int)JsOpcode.BitwiseNot] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&BitwiseNot;
        slots[(int)JsOpcode.LessThan] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LessThan;
        slots[(int)JsOpcode.LessThanOrEqual] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LessThanOrEqual;
        slots[(int)JsOpcode.GreaterThan] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&GreaterThan;
        slots[(int)JsOpcode.GreaterThanOrEqual] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&GreaterThanOrEqual;
        slots[(int)JsOpcode.StrictEquals] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StrictEquals;
        slots[(int)JsOpcode.StrictNotEquals] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StrictNotEquals;
        slots[(int)JsOpcode.LooseEquals] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LooseEquals;
        slots[(int)JsOpcode.LooseNotEquals] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LooseNotEquals;
        slots[(int)JsOpcode.BitwiseOr] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&BitwiseOr;
        slots[(int)JsOpcode.BitwiseAnd] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&BitwiseAnd;
        slots[(int)JsOpcode.BitwiseXor] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&BitwiseXor;
        slots[(int)JsOpcode.ShiftLeft] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ShiftLeft;
        slots[(int)JsOpcode.ShiftRight] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ShiftRight;
        slots[(int)JsOpcode.ShiftRightUnsigned] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ShiftRightUnsigned;
        slots[(int)JsOpcode.TypeOf] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&TypeOf;
        slots[(int)JsOpcode.InstanceOf] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&InstanceOf;
        slots[(int)JsOpcode.In] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&In;
        slots[(int)JsOpcode.Void] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Void;
        slots[(int)JsOpcode.RequireCoercible] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&RequireCoercible;
        slots[(int)JsOpcode.ToPropertyKey] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ToPropertyKey;
        slots[(int)JsOpcode.GetTemplateObject] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&GetTemplateObject;
        slots[(int)JsOpcode.Jump] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Jump;
        slots[(int)JsOpcode.JumpIfFalse] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&JumpIfFalse;
        slots[(int)JsOpcode.JumpIfTrue] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&JumpIfTrue;
        slots[(int)JsOpcode.Throw] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Throw;
        slots[(int)JsOpcode.ForInStart] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ForInStart;
        slots[(int)JsOpcode.ForInNext] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ForInNext;
        slots[(int)JsOpcode.IterateStart] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateStart;
        slots[(int)JsOpcode.IterateNext] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateNext;
        slots[(int)JsOpcode.IterateRest] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateRest;
        slots[(int)JsOpcode.IterateClose] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateClose;
        slots[(int)JsOpcode.Yield] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Yield;
        slots[(int)JsOpcode.YieldDelegate] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&YieldDelegate;
        slots[(int)JsOpcode.Await] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Await;
        slots[(int)JsOpcode.LoadImport] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadImport;
        slots[(int)JsOpcode.ThrowImmutable] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ThrowImmutable;
        slots[(int)JsOpcode.DefineClassElement] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DefineClassElement;
        slots[(int)JsOpcode.Pop] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Pop;
        slots[(int)JsOpcode.Duplicate] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Duplicate;
        slots[(int)JsOpcode.DuplicateTwo] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DuplicateTwo;
        slots[(int)JsOpcode.Swap] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Swap;
        slots[(int)JsOpcode.Pick] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Pick;
        slots[(int)JsOpcode.NewPrivateName] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&NewPrivateName;
        slots[(int)JsOpcode.LoadPrivate] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadPrivate;
        slots[(int)JsOpcode.StorePrivate] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StorePrivate;
        slots[(int)JsOpcode.HasPrivate] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&HasPrivate;
        slots[(int)JsOpcode.RunStaticElements] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&RunStaticElements;
        slots[(int)JsOpcode.IterateStartAsync] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateStartAsync;
        slots[(int)JsOpcode.IterateNextAsync] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateNextAsync;
        slots[(int)JsOpcode.IterateAwaitStep] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateAwaitStep;
        slots[(int)JsOpcode.IterateCloseAsync] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateCloseAsync;
        slots[(int)JsOpcode.IterateCloseCheck] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&IterateCloseCheck;
        slots[(int)JsOpcode.DeclareGlobalLet] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DeclareGlobalLet;
        slots[(int)JsOpcode.DeclareGlobalConst] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DeclareGlobalConst;
        slots[(int)JsOpcode.InitialiseGlobalLexical] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&InitialiseGlobalLexical;
        slots[(int)JsOpcode.DeleteGlobalBinding] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DeleteGlobalBinding;
        slots[(int)JsOpcode.EnterBody] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&EnterBody;
        slots[(int)JsOpcode.ImportCall] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ImportCall;
        slots[(int)JsOpcode.ImportMeta] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ImportMeta;
        slots[(int)JsOpcode.CallEvalSpread] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&CallEvalSpread;
        slots[(int)JsOpcode.LoadEvalName] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadEvalName;
        slots[(int)JsOpcode.LoadEvalNameOrUndefined] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadEvalNameOrUndefined;
        slots[(int)JsOpcode.StoreEvalName] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StoreEvalName;
        slots[(int)JsOpcode.LoadEvalNameWithBase] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&LoadEvalNameWithBase;
        slots[(int)JsOpcode.DeleteEvalName] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DeleteEvalName;
        slots[(int)JsOpcode.WithBaseObject] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&WithBaseObject;
        slots[(int)JsOpcode.StoreEvalVariable] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&StoreEvalVariable;
        slots[(int)JsOpcode.DisposeScope] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DisposeScope;
        slots[(int)JsOpcode.DisposeAdd] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DisposeAdd;
        slots[(int)JsOpcode.DisposeFold] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DisposeFold;
        slots[(int)JsOpcode.DisposeStep] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DisposeStep;
        slots[(int)JsOpcode.DisposeEnd] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&DisposeEnd;
        slots[(int)JsOpcode.ToNumeric] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&ToNumeric;
        slots[(int)JsOpcode.Increment] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Increment;
        slots[(int)JsOpcode.Decrement] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Decrement;

        slots[JsValueAbi.SettleSlot] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Settle;
        slots[JsValueAbi.PrepareSlot] = (nint)(delegate* unmanaged<JsValueFrame*, int, JsValueFrame*, int>)&Prepare;
        slots[JsValueAbi.FinishSlot] = (nint)(delegate* unmanaged<JsValueFrame*, int, JsValueFrame*, int, int>)&Finish;

        if (!Sound(slots, undefined))
        {
            Table = 0;
            return;
        }

        var table = (nint*)System.Runtime.InteropServices.NativeMemory.AllocZeroed(
            (nuint)(JsValueAbi.HelperSlots * sizeof(nint)));

        for (var index = 0; index < slots.Length; index++)
        {
            table[index] = slots[index];
        }

        Table = (nint)table;
    }

    /// <summary>
    /// Whether every defined opcode has its own entry point, the settlement, prepare and finish slots their
    /// own three, and every other byte the refusing one.
    /// </summary>
    /// <remarks>
    /// <b>The three fixed slots must be bytes no opcode takes</b>, so a table in which an opcode came to be
    /// defined at one is refused whole rather than handing either entry to the other's callers.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=16E5DF
    // Broiler-Falsified-If: this answers true for a table in which two defined opcodes or two fixed slots share an entry point, a defined opcode has the refusing one or a fixed slot's, a fixed slot is a defined opcode's or holds the refusing entry or nothing, or another undefined byte has anything but the refusing one
    // Broiler-Human:        PENDING
    private static bool Sound(nint[] slots, nint undefined)
    {
        if (slots.Length != JsValueAbi.HelperSlots || undefined == 0)
        {
            return false;
        }

        var seen = new System.Collections.Generic.HashSet<nint>();

        foreach (var fixedSlot in new[] { JsValueAbi.SettleSlot, JsValueAbi.PrepareSlot, JsValueAbi.FinishSlot })
        {
            if (JsOpcodes.IsDefined((byte)fixedSlot) ||
                slots[fixedSlot] == 0 || slots[fixedSlot] == undefined || !seen.Add(slots[fixedSlot]))
            {
                return false;
            }
        }

        for (var index = 0; index < slots.Length; index++)
        {
            if (index is JsValueAbi.SettleSlot or JsValueAbi.PrepareSlot or JsValueAbi.FinishSlot)
            {
                continue;
            }

            if (!JsOpcodes.IsDefined((byte)index))
            {
                if (slots[index] != undefined)
                {
                    return false;
                }

                continue;
            }

            if (slots[index] == 0 || slots[index] == undefined || !seen.Add(slots[index]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// The entry point of the settlement slot: it charges the debt a debt test carried and answers the
    /// offset it was handed (JSD-0035 section 7, clause V5).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=83F643
    // Broiler-Falsified-If: this runs an instruction, or does anything but the checked settlement of JsNativeActivation.SettleValue
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Settle(JsValueFrame* frame, int pc) =>
        JsNativeActivation.SettleValue(frame, pc);

    /// <summary>
    /// The entry point of the prepare slot: a direct call site's <c>Call</c>, run through its own step or
    /// prepared as a direct call (JSD-0035 section 6, stage JSV-3).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=798F00
    // Broiler-Falsified-If: this does anything but the checked preparation of JsNativeActivation.PrepareCall over the Call opcode's own arm
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Prepare(JsValueFrame* frame, int pc, JsValueFrame* callee) =>
        JsNativeActivation.PrepareCall<ArmCall>(frame, pc, callee);

    /// <summary>The entry point of the finish slot: a direct callee's return taken back to its caller (stage JSV-3).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=A4B1AF
    // Broiler-Falsified-If: this does anything but the checked finish of JsNativeActivation.FinishCall
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Finish(JsValueFrame* frame, int pc, JsValueFrame* callee, int status) =>
        JsNativeActivation.FinishCall(frame, pc, callee, status);

    /// <summary>The entry point of every byte no opcode takes: it answers a defect and touches nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=91A42A
    // Broiler-Falsified-If: this reads or writes any state, or answers anything but the defect status
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Undefined(JsValueFrame* frame, int pc) => (int)JsBaselineStatus.Defect;

    /// <summary>The entry point for <see cref="JsOpcode.Nop"/> (0x00): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CDB853
    // Broiler-Falsified-If: this runs anything other than one Nop at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Nop(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNop>(frame, pc, JsOpcode.Nop);

    /// <summary>The entry point for <see cref="JsOpcode.LoadUndefined"/> (0x01): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B00E19
    // Broiler-Falsified-If: this runs anything other than one LoadUndefined at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadUndefined(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadUndefined>(frame, pc, JsOpcode.LoadUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.LoadNull"/> (0x02): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=4904CD
    // Broiler-Falsified-If: this runs anything other than one LoadNull at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadNull(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadNull>(frame, pc, JsOpcode.LoadNull);

    /// <summary>The entry point for <see cref="JsOpcode.LoadTrue"/> (0x03): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=38BA56
    // Broiler-Falsified-If: this runs anything other than one LoadTrue at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadTrue(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadTrue>(frame, pc, JsOpcode.LoadTrue);

    /// <summary>The entry point for <see cref="JsOpcode.LoadFalse"/> (0x04): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=47DC63
    // Broiler-Falsified-If: this runs anything other than one LoadFalse at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadFalse(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadFalse>(frame, pc, JsOpcode.LoadFalse);

    /// <summary>The entry point for <see cref="JsOpcode.LoadConstant"/> (0x05): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=031E57
    // Broiler-Falsified-If: this runs anything other than one LoadConstant at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadConstant(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadConstant>(frame, pc, JsOpcode.LoadConstant);

    /// <summary>The entry point for <see cref="JsOpcode.LoadThis"/> (0x06): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B6B8A3
    // Broiler-Falsified-If: this runs anything other than one LoadThis at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadThis(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadThis>(frame, pc, JsOpcode.LoadThis);

    /// <summary>The entry point for <see cref="JsOpcode.NewArguments"/> (0x07): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=982568
    // Broiler-Falsified-If: this runs anything other than one NewArguments at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewArguments(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNewArguments>(frame, pc, JsOpcode.NewArguments);

    /// <summary>The entry point for <see cref="JsOpcode.LoadNewTarget"/> (0x08): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CE25FB
    // Broiler-Falsified-If: this runs anything other than one LoadNewTarget at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadNewTarget(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadNewTarget>(frame, pc, JsOpcode.LoadNewTarget);

    /// <summary>The entry point for <see cref="JsOpcode.LoadArgument"/> (0x09): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=90B8BA
    // Broiler-Falsified-If: this runs anything other than one LoadArgument at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadArgument(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadArgument>(frame, pc, JsOpcode.LoadArgument);

    /// <summary>The entry point for <see cref="JsOpcode.RestArguments"/> (0x0A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=4EC35C
    // Broiler-Falsified-If: this runs anything other than one RestArguments at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RestArguments(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmRestArguments>(frame, pc, JsOpcode.RestArguments);

    /// <summary>The entry point for <see cref="JsOpcode.LoadScoped"/> (0x10): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E221CE
    // Broiler-Falsified-If: this runs anything other than one LoadScoped at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadScoped(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadScoped>(frame, pc, JsOpcode.LoadScoped);

    /// <summary>The entry point for <see cref="JsOpcode.StoreScoped"/> (0x11): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=88719E
    // Broiler-Falsified-If: this runs anything other than one StoreScoped at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreScoped(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStoreScoped>(frame, pc, JsOpcode.StoreScoped);

    /// <summary>The entry point for <see cref="JsOpcode.InitialiseScoped"/> (0x12): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=186E60
    // Broiler-Falsified-If: this runs anything other than one InitialiseScoped at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InitialiseScoped(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmInitialiseScoped>(frame, pc, JsOpcode.InitialiseScoped);

    /// <summary>The entry point for <see cref="JsOpcode.LoadGlobal"/> (0x13): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=711B90
    // Broiler-Falsified-If: this runs anything other than one LoadGlobal at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadGlobal(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadGlobal>(frame, pc, JsOpcode.LoadGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.StoreGlobal"/> (0x14): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D37A84
    // Broiler-Falsified-If: this runs anything other than one StoreGlobal at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreGlobal(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStoreGlobal>(frame, pc, JsOpcode.StoreGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.LoadGlobalOrUndefined"/> (0x15): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C2AE3F
    // Broiler-Falsified-If: this runs anything other than one LoadGlobalOrUndefined at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadGlobalOrUndefined(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadGlobalOrUndefined>(frame, pc, JsOpcode.LoadGlobalOrUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.PushScope"/> (0x16): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=1022FA
    // Broiler-Falsified-If: this runs anything other than one PushScope at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PushScope(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmPushScope>(frame, pc, JsOpcode.PushScope);

    /// <summary>The entry point for <see cref="JsOpcode.PopScope"/> (0x17): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=04792F
    // Broiler-Falsified-If: this runs anything other than one PopScope at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PopScope(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmPopScope>(frame, pc, JsOpcode.PopScope);

    /// <summary>The entry point for <see cref="JsOpcode.CopyScope"/> (0x18): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=AD07A1
    // Broiler-Falsified-If: this runs anything other than one CopyScope at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CopyScope(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmCopyScope>(frame, pc, JsOpcode.CopyScope);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobal"/> (0x19): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=4F774B
    // Broiler-Falsified-If: this runs anything other than one DeclareGlobal at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobal(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDeclareGlobal>(frame, pc, JsOpcode.DeclareGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.PushObjectScope"/> (0x1A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=861D59
    // Broiler-Falsified-If: this runs anything other than one PushObjectScope at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PushObjectScope(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmPushObjectScope>(frame, pc, JsOpcode.PushObjectScope);

    /// <summary>The entry point for <see cref="JsOpcode.ResolveName"/> (0x1B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F05111
    // Broiler-Falsified-If: this runs anything other than one ResolveName at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ResolveName(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmResolveName>(frame, pc, JsOpcode.ResolveName);

    /// <summary>The entry point for <see cref="JsOpcode.NewObject"/> (0x20): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=1992AB
    // Broiler-Falsified-If: this runs anything other than one NewObject at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewObject(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNewObject>(frame, pc, JsOpcode.NewObject);

    /// <summary>The entry point for <see cref="JsOpcode.NewArray"/> (0x21): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=8CA964
    // Broiler-Falsified-If: this runs anything other than one NewArray at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewArray(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNewArray>(frame, pc, JsOpcode.NewArray);

    /// <summary>The entry point for <see cref="JsOpcode.GetProperty"/> (0x22): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=0A6252
    // Broiler-Falsified-If: this runs anything other than one GetProperty at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetProperty(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmGetProperty>(frame, pc, JsOpcode.GetProperty);

    /// <summary>The entry point for <see cref="JsOpcode.SetProperty"/> (0x23): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=6AD29A
    // Broiler-Falsified-If: this runs anything other than one SetProperty at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetProperty(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSetProperty>(frame, pc, JsOpcode.SetProperty);

    /// <summary>The entry point for <see cref="JsOpcode.GetIndex"/> (0x24): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C28AD4
    // Broiler-Falsified-If: this runs anything other than one GetIndex at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetIndex(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmGetIndex>(frame, pc, JsOpcode.GetIndex);

    /// <summary>The entry point for <see cref="JsOpcode.SetIndex"/> (0x25): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=4C1583
    // Broiler-Falsified-If: this runs anything other than one SetIndex at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetIndex(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSetIndex>(frame, pc, JsOpcode.SetIndex);

    /// <summary>The entry point for <see cref="JsOpcode.DefineField"/> (0x26): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=92A728
    // Broiler-Falsified-If: this runs anything other than one DefineField at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineField(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDefineField>(frame, pc, JsOpcode.DefineField);

    /// <summary>The entry point for <see cref="JsOpcode.DefineIndexed"/> (0x27): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=793E92
    // Broiler-Falsified-If: this runs anything other than one DefineIndexed at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineIndexed(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDefineIndexed>(frame, pc, JsOpcode.DefineIndexed);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteProperty"/> (0x28): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D086DA
    // Broiler-Falsified-If: this runs anything other than one DeleteProperty at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteProperty(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDeleteProperty>(frame, pc, JsOpcode.DeleteProperty);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteIndex"/> (0x29): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=80806E
    // Broiler-Falsified-If: this runs anything other than one DeleteIndex at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteIndex(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDeleteIndex>(frame, pc, JsOpcode.DeleteIndex);

    /// <summary>The entry point for <see cref="JsOpcode.DefineGetter"/> (0x2A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F610DF
    // Broiler-Falsified-If: this runs anything other than one DefineGetter at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineGetter(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDefineGetter>(frame, pc, JsOpcode.DefineGetter);

    /// <summary>The entry point for <see cref="JsOpcode.DefineSetter"/> (0x2B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B1706E
    // Broiler-Falsified-If: this runs anything other than one DefineSetter at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineSetter(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDefineSetter>(frame, pc, JsOpcode.DefineSetter);

    /// <summary>The entry point for <see cref="JsOpcode.DefineMethod"/> (0x2C): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FF497C
    // Broiler-Falsified-If: this runs anything other than one DefineMethod at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineMethod(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDefineMethod>(frame, pc, JsOpcode.DefineMethod);

    /// <summary>The entry point for <see cref="JsOpcode.LoadSuperProperty"/> (0x2D): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FF75AB
    // Broiler-Falsified-If: this runs anything other than one LoadSuperProperty at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadSuperProperty(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadSuperProperty>(frame, pc, JsOpcode.LoadSuperProperty);

    /// <summary>The entry point for <see cref="JsOpcode.StoreSuperProperty"/> (0x2E): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=37FE7A
    // Broiler-Falsified-If: this runs anything other than one StoreSuperProperty at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreSuperProperty(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStoreSuperProperty>(frame, pc, JsOpcode.StoreSuperProperty);

    /// <summary>The entry point for <see cref="JsOpcode.ArrayAppend"/> (0x2F): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=3B0245
    // Broiler-Falsified-If: this runs anything other than one ArrayAppend at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ArrayAppend(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmArrayAppend>(frame, pc, JsOpcode.ArrayAppend);

    /// <summary>The entry point for <see cref="JsOpcode.Closure"/> (0x30): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B09C7D
    // Broiler-Falsified-If: this runs anything other than one Closure at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Closure(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmClosure>(frame, pc, JsOpcode.Closure);

    /// <summary>The entry point for <see cref="JsOpcode.Call"/> (0x31): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FF3AE6
    // Broiler-Falsified-If: this runs anything other than one Call at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Call(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmCall>(frame, pc, JsOpcode.Call);

    /// <summary>The entry point for <see cref="JsOpcode.Construct"/> (0x32): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=338957
    // Broiler-Falsified-If: this runs anything other than one Construct at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Construct(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmConstruct>(frame, pc, JsOpcode.Construct);

    /// <summary>The entry point for <see cref="JsOpcode.Return"/> (0x33): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=55103C
    // Broiler-Falsified-If: this runs anything other than one Return at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Return(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmReturn>(frame, pc, JsOpcode.Return);

    /// <summary>The entry point for <see cref="JsOpcode.ReturnUndefined"/> (0x34): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7C7641
    // Broiler-Falsified-If: this runs anything other than one ReturnUndefined at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ReturnUndefined(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmReturnUndefined>(frame, pc, JsOpcode.ReturnUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.CallEval"/> (0x35): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CB067F
    // Broiler-Falsified-If: this runs anything other than one CallEval at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallEval(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmCallEval>(frame, pc, JsOpcode.CallEval);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCall"/> (0x36): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F46B10
    // Broiler-Falsified-If: this runs anything other than one SuperCall at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCall(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSuperCall>(frame, pc, JsOpcode.SuperCall);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCallForwarded"/> (0x37): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=067599
    // Broiler-Falsified-If: this runs anything other than one SuperCallForwarded at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCallForwarded(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSuperCallForwarded>(frame, pc, JsOpcode.SuperCallForwarded);

    /// <summary>The entry point for <see cref="JsOpcode.NewClass"/> (0x38): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B9D87F
    // Broiler-Falsified-If: this runs anything other than one NewClass at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewClass(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNewClass>(frame, pc, JsOpcode.NewClass);

    /// <summary>The entry point for <see cref="JsOpcode.ArrayHoles"/> (0x39): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=033A04
    // Broiler-Falsified-If: this runs anything other than one ArrayHoles at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ArrayHoles(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmArrayHoles>(frame, pc, JsOpcode.ArrayHoles);

    /// <summary>The entry point for <see cref="JsOpcode.SpreadArray"/> (0x3A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=34D25B
    // Broiler-Falsified-If: this runs anything other than one SpreadArray at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SpreadArray(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSpreadArray>(frame, pc, JsOpcode.SpreadArray);

    /// <summary>The entry point for <see cref="JsOpcode.SpreadObject"/> (0x3B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=4B990D
    // Broiler-Falsified-If: this runs anything other than one SpreadObject at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SpreadObject(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSpreadObject>(frame, pc, JsOpcode.SpreadObject);

    /// <summary>The entry point for <see cref="JsOpcode.CallSpread"/> (0x3C): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B9BE8F
    // Broiler-Falsified-If: this runs anything other than one CallSpread at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallSpread(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmCallSpread>(frame, pc, JsOpcode.CallSpread);

    /// <summary>The entry point for <see cref="JsOpcode.ConstructSpread"/> (0x3D): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A041D3
    // Broiler-Falsified-If: this runs anything other than one ConstructSpread at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ConstructSpread(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmConstructSpread>(frame, pc, JsOpcode.ConstructSpread);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCallSpread"/> (0x3E): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DABE25
    // Broiler-Falsified-If: this runs anything other than one SuperCallSpread at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCallSpread(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSuperCallSpread>(frame, pc, JsOpcode.SuperCallSpread);

    /// <summary>The entry point for <see cref="JsOpcode.SetPrototypeLiteral"/> (0x3F): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2F03D8
    // Broiler-Falsified-If: this runs anything other than one SetPrototypeLiteral at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetPrototypeLiteral(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSetPrototypeLiteral>(frame, pc, JsOpcode.SetPrototypeLiteral);

    /// <summary>The entry point for <see cref="JsOpcode.Add"/> (0x40): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=234207
    // Broiler-Falsified-If: this runs anything other than one Add at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Add(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmAdd>(frame, pc, JsOpcode.Add);

    /// <summary>The entry point for <see cref="JsOpcode.Subtract"/> (0x41): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=046109
    // Broiler-Falsified-If: this runs anything other than one Subtract at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Subtract(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSubtract>(frame, pc, JsOpcode.Subtract);

    /// <summary>The entry point for <see cref="JsOpcode.Multiply"/> (0x42): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=72E6D0
    // Broiler-Falsified-If: this runs anything other than one Multiply at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Multiply(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmMultiply>(frame, pc, JsOpcode.Multiply);

    /// <summary>The entry point for <see cref="JsOpcode.Divide"/> (0x43): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5BA206
    // Broiler-Falsified-If: this runs anything other than one Divide at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Divide(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDivide>(frame, pc, JsOpcode.Divide);

    /// <summary>The entry point for <see cref="JsOpcode.Remainder"/> (0x44): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=480C60
    // Broiler-Falsified-If: this runs anything other than one Remainder at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Remainder(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmRemainder>(frame, pc, JsOpcode.Remainder);

    /// <summary>The entry point for <see cref="JsOpcode.Exponent"/> (0x45): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D97834
    // Broiler-Falsified-If: this runs anything other than one Exponent at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Exponent(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmExponent>(frame, pc, JsOpcode.Exponent);

    /// <summary>The entry point for <see cref="JsOpcode.Negate"/> (0x46): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=71D864
    // Broiler-Falsified-If: this runs anything other than one Negate at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Negate(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNegate>(frame, pc, JsOpcode.Negate);

    /// <summary>The entry point for <see cref="JsOpcode.ToNumber"/> (0x47): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D0051A
    // Broiler-Falsified-If: this runs anything other than one ToNumber at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ToNumber(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmToNumber>(frame, pc, JsOpcode.ToNumber);

    /// <summary>The entry point for <see cref="JsOpcode.Not"/> (0x48): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5F7F50
    // Broiler-Falsified-If: this runs anything other than one Not at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Not(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNot>(frame, pc, JsOpcode.Not);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseNot"/> (0x49): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=1306C3
    // Broiler-Falsified-If: this runs anything other than one BitwiseNot at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseNot(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmBitwiseNot>(frame, pc, JsOpcode.BitwiseNot);

    /// <summary>The entry point for <see cref="JsOpcode.LessThan"/> (0x4A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B9DCBE
    // Broiler-Falsified-If: this runs anything other than one LessThan at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LessThan(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLessThan>(frame, pc, JsOpcode.LessThan);

    /// <summary>The entry point for <see cref="JsOpcode.LessThanOrEqual"/> (0x4B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=008E9A
    // Broiler-Falsified-If: this runs anything other than one LessThanOrEqual at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LessThanOrEqual(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLessThanOrEqual>(frame, pc, JsOpcode.LessThanOrEqual);

    /// <summary>The entry point for <see cref="JsOpcode.GreaterThan"/> (0x4C): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A6A4A2
    // Broiler-Falsified-If: this runs anything other than one GreaterThan at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GreaterThan(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmGreaterThan>(frame, pc, JsOpcode.GreaterThan);

    /// <summary>The entry point for <see cref="JsOpcode.GreaterThanOrEqual"/> (0x4D): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2D471C
    // Broiler-Falsified-If: this runs anything other than one GreaterThanOrEqual at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GreaterThanOrEqual(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmGreaterThanOrEqual>(frame, pc, JsOpcode.GreaterThanOrEqual);

    /// <summary>The entry point for <see cref="JsOpcode.StrictEquals"/> (0x4E): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=EC89ED
    // Broiler-Falsified-If: this runs anything other than one StrictEquals at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StrictEquals(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStrictEquals>(frame, pc, JsOpcode.StrictEquals);

    /// <summary>The entry point for <see cref="JsOpcode.StrictNotEquals"/> (0x4F): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=0E59FD
    // Broiler-Falsified-If: this runs anything other than one StrictNotEquals at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StrictNotEquals(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStrictNotEquals>(frame, pc, JsOpcode.StrictNotEquals);

    /// <summary>The entry point for <see cref="JsOpcode.LooseEquals"/> (0x50): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=21CF78
    // Broiler-Falsified-If: this runs anything other than one LooseEquals at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LooseEquals(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLooseEquals>(frame, pc, JsOpcode.LooseEquals);

    /// <summary>The entry point for <see cref="JsOpcode.LooseNotEquals"/> (0x51): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=45C69B
    // Broiler-Falsified-If: this runs anything other than one LooseNotEquals at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LooseNotEquals(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLooseNotEquals>(frame, pc, JsOpcode.LooseNotEquals);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseOr"/> (0x52): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=19A5A0
    // Broiler-Falsified-If: this runs anything other than one BitwiseOr at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseOr(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmBitwiseOr>(frame, pc, JsOpcode.BitwiseOr);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseAnd"/> (0x53): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2D8C62
    // Broiler-Falsified-If: this runs anything other than one BitwiseAnd at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseAnd(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmBitwiseAnd>(frame, pc, JsOpcode.BitwiseAnd);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseXor"/> (0x54): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=3EE95E
    // Broiler-Falsified-If: this runs anything other than one BitwiseXor at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseXor(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmBitwiseXor>(frame, pc, JsOpcode.BitwiseXor);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftLeft"/> (0x55): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F97573
    // Broiler-Falsified-If: this runs anything other than one ShiftLeft at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftLeft(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmShiftLeft>(frame, pc, JsOpcode.ShiftLeft);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftRight"/> (0x56): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=65B054
    // Broiler-Falsified-If: this runs anything other than one ShiftRight at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftRight(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmShiftRight>(frame, pc, JsOpcode.ShiftRight);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftRightUnsigned"/> (0x57): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CA92BB
    // Broiler-Falsified-If: this runs anything other than one ShiftRightUnsigned at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftRightUnsigned(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmShiftRightUnsigned>(frame, pc, JsOpcode.ShiftRightUnsigned);

    /// <summary>The entry point for <see cref="JsOpcode.TypeOf"/> (0x58): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=01A086
    // Broiler-Falsified-If: this runs anything other than one TypeOf at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int TypeOf(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmTypeOf>(frame, pc, JsOpcode.TypeOf);

    /// <summary>The entry point for <see cref="JsOpcode.InstanceOf"/> (0x59): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=248605
    // Broiler-Falsified-If: this runs anything other than one InstanceOf at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InstanceOf(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmInstanceOf>(frame, pc, JsOpcode.InstanceOf);

    /// <summary>The entry point for <see cref="JsOpcode.In"/> (0x5A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5FF055
    // Broiler-Falsified-If: this runs anything other than one In at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int In(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIn>(frame, pc, JsOpcode.In);

    /// <summary>The entry point for <see cref="JsOpcode.Void"/> (0x5B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=87AD5F
    // Broiler-Falsified-If: this runs anything other than one Void at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Void(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmVoid>(frame, pc, JsOpcode.Void);

    /// <summary>The entry point for <see cref="JsOpcode.RequireCoercible"/> (0x5C): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=3B0113
    // Broiler-Falsified-If: this runs anything other than one RequireCoercible at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RequireCoercible(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmRequireCoercible>(frame, pc, JsOpcode.RequireCoercible);

    /// <summary>The entry point for <see cref="JsOpcode.ToPropertyKey"/> (0x5D): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E13AB6
    // Broiler-Falsified-If: this runs anything other than one ToPropertyKey at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ToPropertyKey(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmToPropertyKey>(frame, pc, JsOpcode.ToPropertyKey);

    /// <summary>The entry point for <see cref="JsOpcode.GetTemplateObject"/> (0x5E): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=1F9053
    // Broiler-Falsified-If: this runs anything other than one GetTemplateObject at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetTemplateObject(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmGetTemplateObject>(frame, pc, JsOpcode.GetTemplateObject);

    /// <summary>The entry point for <see cref="JsOpcode.Jump"/> (0x60): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=270671
    // Broiler-Falsified-If: this runs anything other than one Jump at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Jump(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmJump>(frame, pc, JsOpcode.Jump);

    /// <summary>The entry point for <see cref="JsOpcode.JumpIfFalse"/> (0x61): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C4DE38
    // Broiler-Falsified-If: this runs anything other than one JumpIfFalse at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int JumpIfFalse(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmJumpIfFalse>(frame, pc, JsOpcode.JumpIfFalse);

    /// <summary>The entry point for <see cref="JsOpcode.JumpIfTrue"/> (0x62): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=591FF8
    // Broiler-Falsified-If: this runs anything other than one JumpIfTrue at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int JumpIfTrue(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmJumpIfTrue>(frame, pc, JsOpcode.JumpIfTrue);

    /// <summary>The entry point for <see cref="JsOpcode.Throw"/> (0x63): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5BEC33
    // Broiler-Falsified-If: this runs anything other than one Throw at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Throw(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmThrow>(frame, pc, JsOpcode.Throw);

    /// <summary>The entry point for <see cref="JsOpcode.ForInStart"/> (0x64): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9C9F99
    // Broiler-Falsified-If: this runs anything other than one ForInStart at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ForInStart(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmForInStart>(frame, pc, JsOpcode.ForInStart);

    /// <summary>The entry point for <see cref="JsOpcode.ForInNext"/> (0x65): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=246D7B
    // Broiler-Falsified-If: this runs anything other than one ForInNext at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ForInNext(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmForInNext>(frame, pc, JsOpcode.ForInNext);

    /// <summary>The entry point for <see cref="JsOpcode.IterateStart"/> (0x66): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7E6547
    // Broiler-Falsified-If: this runs anything other than one IterateStart at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateStart(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateStart>(frame, pc, JsOpcode.IterateStart);

    /// <summary>The entry point for <see cref="JsOpcode.IterateNext"/> (0x67): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=47D48A
    // Broiler-Falsified-If: this runs anything other than one IterateNext at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateNext(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateNext>(frame, pc, JsOpcode.IterateNext);

    /// <summary>The entry point for <see cref="JsOpcode.IterateRest"/> (0x68): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=EBF67B
    // Broiler-Falsified-If: this runs anything other than one IterateRest at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateRest(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateRest>(frame, pc, JsOpcode.IterateRest);

    /// <summary>The entry point for <see cref="JsOpcode.IterateClose"/> (0x69): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=D26D42
    // Broiler-Falsified-If: this runs anything other than one IterateClose at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateClose(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateClose>(frame, pc, JsOpcode.IterateClose);

    /// <summary>The entry point for <see cref="JsOpcode.Yield"/> (0x6A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=08D312
    // Broiler-Falsified-If: this runs anything other than one Yield at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Yield(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmYield>(frame, pc, JsOpcode.Yield);

    /// <summary>The entry point for <see cref="JsOpcode.YieldDelegate"/> (0x6B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F8F00E
    // Broiler-Falsified-If: this runs anything other than one YieldDelegate at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int YieldDelegate(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmYieldDelegate>(frame, pc, JsOpcode.YieldDelegate);

    /// <summary>The entry point for <see cref="JsOpcode.Await"/> (0x6C): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=707DD0
    // Broiler-Falsified-If: this runs anything other than one Await at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Await(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmAwait>(frame, pc, JsOpcode.Await);

    /// <summary>The entry point for <see cref="JsOpcode.LoadImport"/> (0x6D): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DE9194
    // Broiler-Falsified-If: this runs anything other than one LoadImport at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadImport(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadImport>(frame, pc, JsOpcode.LoadImport);

    /// <summary>The entry point for <see cref="JsOpcode.ThrowImmutable"/> (0x6E): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=EC4BFE
    // Broiler-Falsified-If: this runs anything other than one ThrowImmutable at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ThrowImmutable(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmThrowImmutable>(frame, pc, JsOpcode.ThrowImmutable);

    /// <summary>The entry point for <see cref="JsOpcode.DefineClassElement"/> (0x6F): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B7008F
    // Broiler-Falsified-If: this runs anything other than one DefineClassElement at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineClassElement(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDefineClassElement>(frame, pc, JsOpcode.DefineClassElement);

    /// <summary>The entry point for <see cref="JsOpcode.Pop"/> (0x70): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=14DC3D
    // Broiler-Falsified-If: this runs anything other than one Pop at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Pop(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmPop>(frame, pc, JsOpcode.Pop);

    /// <summary>The entry point for <see cref="JsOpcode.Duplicate"/> (0x71): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=730659
    // Broiler-Falsified-If: this runs anything other than one Duplicate at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Duplicate(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDuplicate>(frame, pc, JsOpcode.Duplicate);

    /// <summary>The entry point for <see cref="JsOpcode.DuplicateTwo"/> (0x72): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=038CB0
    // Broiler-Falsified-If: this runs anything other than one DuplicateTwo at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DuplicateTwo(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDuplicateTwo>(frame, pc, JsOpcode.DuplicateTwo);

    /// <summary>The entry point for <see cref="JsOpcode.Swap"/> (0x73): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=516106
    // Broiler-Falsified-If: this runs anything other than one Swap at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Swap(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmSwap>(frame, pc, JsOpcode.Swap);

    /// <summary>The entry point for <see cref="JsOpcode.Pick"/> (0x74): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=647F94
    // Broiler-Falsified-If: this runs anything other than one Pick at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Pick(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmPick>(frame, pc, JsOpcode.Pick);

    /// <summary>The entry point for <see cref="JsOpcode.NewPrivateName"/> (0x75): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F77E53
    // Broiler-Falsified-If: this runs anything other than one NewPrivateName at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewPrivateName(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmNewPrivateName>(frame, pc, JsOpcode.NewPrivateName);

    /// <summary>The entry point for <see cref="JsOpcode.LoadPrivate"/> (0x76): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C5D3E1
    // Broiler-Falsified-If: this runs anything other than one LoadPrivate at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadPrivate(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadPrivate>(frame, pc, JsOpcode.LoadPrivate);

    /// <summary>The entry point for <see cref="JsOpcode.StorePrivate"/> (0x77): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DA5D2D
    // Broiler-Falsified-If: this runs anything other than one StorePrivate at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StorePrivate(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStorePrivate>(frame, pc, JsOpcode.StorePrivate);

    /// <summary>The entry point for <see cref="JsOpcode.HasPrivate"/> (0x78): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=6A21D4
    // Broiler-Falsified-If: this runs anything other than one HasPrivate at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int HasPrivate(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmHasPrivate>(frame, pc, JsOpcode.HasPrivate);

    /// <summary>The entry point for <see cref="JsOpcode.RunStaticElements"/> (0x79): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E4808B
    // Broiler-Falsified-If: this runs anything other than one RunStaticElements at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RunStaticElements(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmRunStaticElements>(frame, pc, JsOpcode.RunStaticElements);

    /// <summary>The entry point for <see cref="JsOpcode.IterateStartAsync"/> (0x7A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=45D829
    // Broiler-Falsified-If: this runs anything other than one IterateStartAsync at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateStartAsync(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateStartAsync>(frame, pc, JsOpcode.IterateStartAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateNextAsync"/> (0x7B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=930D4D
    // Broiler-Falsified-If: this runs anything other than one IterateNextAsync at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateNextAsync(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateNextAsync>(frame, pc, JsOpcode.IterateNextAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateAwaitStep"/> (0x7C): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=035850
    // Broiler-Falsified-If: this runs anything other than one IterateAwaitStep at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateAwaitStep(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateAwaitStep>(frame, pc, JsOpcode.IterateAwaitStep);

    /// <summary>The entry point for <see cref="JsOpcode.IterateCloseAsync"/> (0x7D): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C9E9E1
    // Broiler-Falsified-If: this runs anything other than one IterateCloseAsync at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateCloseAsync(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateCloseAsync>(frame, pc, JsOpcode.IterateCloseAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateCloseCheck"/> (0x7E): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B72045
    // Broiler-Falsified-If: this runs anything other than one IterateCloseCheck at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateCloseCheck(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIterateCloseCheck>(frame, pc, JsOpcode.IterateCloseCheck);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobalLet"/> (0x7F): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A8C10D
    // Broiler-Falsified-If: this runs anything other than one DeclareGlobalLet at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobalLet(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDeclareGlobalLet>(frame, pc, JsOpcode.DeclareGlobalLet);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobalConst"/> (0x80): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=05F0F2
    // Broiler-Falsified-If: this runs anything other than one DeclareGlobalConst at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobalConst(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDeclareGlobalConst>(frame, pc, JsOpcode.DeclareGlobalConst);

    /// <summary>The entry point for <see cref="JsOpcode.InitialiseGlobalLexical"/> (0x81): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=21210C
    // Broiler-Falsified-If: this runs anything other than one InitialiseGlobalLexical at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InitialiseGlobalLexical(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmInitialiseGlobalLexical>(frame, pc, JsOpcode.InitialiseGlobalLexical);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteGlobalBinding"/> (0x82): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=54FF86
    // Broiler-Falsified-If: this runs anything other than one DeleteGlobalBinding at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteGlobalBinding(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDeleteGlobalBinding>(frame, pc, JsOpcode.DeleteGlobalBinding);

    /// <summary>The entry point for <see cref="JsOpcode.EnterBody"/> (0x83): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=82546F
    // Broiler-Falsified-If: this runs anything other than one EnterBody at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int EnterBody(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmEnterBody>(frame, pc, JsOpcode.EnterBody);

    /// <summary>The entry point for <see cref="JsOpcode.ImportCall"/> (0x85): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C79CBD
    // Broiler-Falsified-If: this runs anything other than one ImportCall at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ImportCall(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmImportCall>(frame, pc, JsOpcode.ImportCall);

    /// <summary>The entry point for <see cref="JsOpcode.ImportMeta"/> (0x86): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5EEE8F
    // Broiler-Falsified-If: this runs anything other than one ImportMeta at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ImportMeta(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmImportMeta>(frame, pc, JsOpcode.ImportMeta);

    /// <summary>The entry point for <see cref="JsOpcode.CallEvalSpread"/> (0x90): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E83DB8
    // Broiler-Falsified-If: this runs anything other than one CallEvalSpread at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallEvalSpread(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmCallEvalSpread>(frame, pc, JsOpcode.CallEvalSpread);

    /// <summary>The entry point for <see cref="JsOpcode.LoadEvalName"/> (0x91): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=1D2E83
    // Broiler-Falsified-If: this runs anything other than one LoadEvalName at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadEvalName(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadEvalName>(frame, pc, JsOpcode.LoadEvalName);

    /// <summary>The entry point for <see cref="JsOpcode.LoadEvalNameOrUndefined"/> (0x92): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=993E16
    // Broiler-Falsified-If: this runs anything other than one LoadEvalNameOrUndefined at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadEvalNameOrUndefined(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadEvalNameOrUndefined>(frame, pc, JsOpcode.LoadEvalNameOrUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.StoreEvalName"/> (0x93): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9B8EE2
    // Broiler-Falsified-If: this runs anything other than one StoreEvalName at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreEvalName(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStoreEvalName>(frame, pc, JsOpcode.StoreEvalName);

    /// <summary>The entry point for <see cref="JsOpcode.LoadEvalNameWithBase"/> (0x94): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=010F0B
    // Broiler-Falsified-If: this runs anything other than one LoadEvalNameWithBase at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadEvalNameWithBase(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmLoadEvalNameWithBase>(frame, pc, JsOpcode.LoadEvalNameWithBase);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteEvalName"/> (0x95): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BB023C
    // Broiler-Falsified-If: this runs anything other than one DeleteEvalName at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteEvalName(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDeleteEvalName>(frame, pc, JsOpcode.DeleteEvalName);

    /// <summary>The entry point for <see cref="JsOpcode.WithBaseObject"/> (0x9A): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2E9C39
    // Broiler-Falsified-If: this runs anything other than one WithBaseObject at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int WithBaseObject(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmWithBaseObject>(frame, pc, JsOpcode.WithBaseObject);

    /// <summary>The entry point for <see cref="JsOpcode.StoreEvalVariable"/> (0x9B): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B8225B
    // Broiler-Falsified-If: this runs anything other than one StoreEvalVariable at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreEvalVariable(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmStoreEvalVariable>(frame, pc, JsOpcode.StoreEvalVariable);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeScope"/> (0xA0): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=411816
    // Broiler-Falsified-If: this runs anything other than one DisposeScope at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeScope(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDisposeScope>(frame, pc, JsOpcode.DisposeScope);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeAdd"/> (0xA1): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=26C59F
    // Broiler-Falsified-If: this runs anything other than one DisposeAdd at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeAdd(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDisposeAdd>(frame, pc, JsOpcode.DisposeAdd);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeFold"/> (0xA2): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=74383E
    // Broiler-Falsified-If: this runs anything other than one DisposeFold at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeFold(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDisposeFold>(frame, pc, JsOpcode.DisposeFold);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeStep"/> (0xA3): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=48ADC8
    // Broiler-Falsified-If: this runs anything other than one DisposeStep at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeStep(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDisposeStep>(frame, pc, JsOpcode.DisposeStep);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeEnd"/> (0xA4): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F5AD61
    // Broiler-Falsified-If: this runs anything other than one DisposeEnd at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeEnd(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDisposeEnd>(frame, pc, JsOpcode.DisposeEnd);

    /// <summary>The entry point for <see cref="JsOpcode.ToNumeric"/> (0xB0): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=777668
    // Broiler-Falsified-If: this runs anything other than one ToNumeric at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ToNumeric(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmToNumeric>(frame, pc, JsOpcode.ToNumeric);

    /// <summary>The entry point for <see cref="JsOpcode.Increment"/> (0xB1): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FE0117
    // Broiler-Falsified-If: this runs anything other than one Increment at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Increment(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmIncrement>(frame, pc, JsOpcode.Increment);

    /// <summary>The entry point for <see cref="JsOpcode.Decrement"/> (0xB2): one value step of it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=C88297
    // Broiler-Falsified-If: this runs anything other than one Decrement at the offset the managed side expects, through its own step
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Decrement(JsValueFrame* frame, int pc) =>
        JsNativeActivation.StepValue<ArmDecrement>(frame, pc, JsOpcode.Decrement);

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Nop"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EE2A74
    // Broiler-Falsified-If: this mode names an opcode other than Nop
    // Broiler-Human:        PENDING
    internal readonly struct ArmNop : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Nop"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D7D8F6
        // Broiler-Falsified-If: this answers any opcode other than Nop
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Nop;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FA6D6B
    // Broiler-Falsified-If: this mode names an opcode other than LoadUndefined
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E0D46D
        // Broiler-Falsified-If: this answers any opcode other than LoadUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadUndefined;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadNull"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=902CD4
    // Broiler-Falsified-If: this mode names an opcode other than LoadNull
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadNull : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadNull"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8F90FD
        // Broiler-Falsified-If: this answers any opcode other than LoadNull
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadNull;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadTrue"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1B4E9F
    // Broiler-Falsified-If: this mode names an opcode other than LoadTrue
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadTrue : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadTrue"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3D4366
        // Broiler-Falsified-If: this answers any opcode other than LoadTrue
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadTrue;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadFalse"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=38E162
    // Broiler-Falsified-If: this mode names an opcode other than LoadFalse
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadFalse : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadFalse"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=289B9B
        // Broiler-Falsified-If: this answers any opcode other than LoadFalse
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadFalse;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadConstant"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D4298D
    // Broiler-Falsified-If: this mode names an opcode other than LoadConstant
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadConstant : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadConstant"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A1257B
        // Broiler-Falsified-If: this answers any opcode other than LoadConstant
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadConstant;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadThis"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2639BE
    // Broiler-Falsified-If: this mode names an opcode other than LoadThis
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadThis : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadThis"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=93E9E1
        // Broiler-Falsified-If: this answers any opcode other than LoadThis
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadThis;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.NewArguments"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6D576B
    // Broiler-Falsified-If: this mode names an opcode other than NewArguments
    // Broiler-Human:        PENDING
    internal readonly struct ArmNewArguments : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewArguments"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A8B424
        // Broiler-Falsified-If: this answers any opcode other than NewArguments
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewArguments;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadNewTarget"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FFC187
    // Broiler-Falsified-If: this mode names an opcode other than LoadNewTarget
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadNewTarget : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadNewTarget"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D1003C
        // Broiler-Falsified-If: this answers any opcode other than LoadNewTarget
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadNewTarget;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadArgument"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C87458
    // Broiler-Falsified-If: this mode names an opcode other than LoadArgument
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadArgument : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadArgument"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A879E1
        // Broiler-Falsified-If: this answers any opcode other than LoadArgument
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadArgument;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.RestArguments"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E31A74
    // Broiler-Falsified-If: this mode names an opcode other than RestArguments
    // Broiler-Human:        PENDING
    internal readonly struct ArmRestArguments : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.RestArguments"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CC6674
        // Broiler-Falsified-If: this answers any opcode other than RestArguments
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.RestArguments;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadScoped"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1B43C8
    // Broiler-Falsified-If: this mode names an opcode other than LoadScoped
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadScoped : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadScoped"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DA48FC
        // Broiler-Falsified-If: this answers any opcode other than LoadScoped
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadScoped;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StoreScoped"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=06A9E1
    // Broiler-Falsified-If: this mode names an opcode other than StoreScoped
    // Broiler-Human:        PENDING
    internal readonly struct ArmStoreScoped : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreScoped"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3F1707
        // Broiler-Falsified-If: this answers any opcode other than StoreScoped
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreScoped;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.InitialiseScoped"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2267FE
    // Broiler-Falsified-If: this mode names an opcode other than InitialiseScoped
    // Broiler-Human:        PENDING
    internal readonly struct ArmInitialiseScoped : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.InitialiseScoped"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F14691
        // Broiler-Falsified-If: this answers any opcode other than InitialiseScoped
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.InitialiseScoped;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadGlobal"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0C59C5
    // Broiler-Falsified-If: this mode names an opcode other than LoadGlobal
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadGlobal : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadGlobal"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A7AB7B
        // Broiler-Falsified-If: this answers any opcode other than LoadGlobal
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadGlobal;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StoreGlobal"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6069F2
    // Broiler-Falsified-If: this mode names an opcode other than StoreGlobal
    // Broiler-Human:        PENDING
    internal readonly struct ArmStoreGlobal : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreGlobal"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F87126
        // Broiler-Falsified-If: this answers any opcode other than StoreGlobal
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreGlobal;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadGlobalOrUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C81635
    // Broiler-Falsified-If: this mode names an opcode other than LoadGlobalOrUndefined
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadGlobalOrUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadGlobalOrUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=35BA98
        // Broiler-Falsified-If: this answers any opcode other than LoadGlobalOrUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadGlobalOrUndefined;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.PushScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=62441E
    // Broiler-Falsified-If: this mode names an opcode other than PushScope
    // Broiler-Human:        PENDING
    internal readonly struct ArmPushScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.PushScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6F0172
        // Broiler-Falsified-If: this answers any opcode other than PushScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.PushScope;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.PopScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=517D6C
    // Broiler-Falsified-If: this mode names an opcode other than PopScope
    // Broiler-Human:        PENDING
    internal readonly struct ArmPopScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.PopScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=115BB4
        // Broiler-Falsified-If: this answers any opcode other than PopScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.PopScope;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.CopyScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4140DB
    // Broiler-Falsified-If: this mode names an opcode other than CopyScope
    // Broiler-Human:        PENDING
    internal readonly struct ArmCopyScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CopyScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BDDF52
        // Broiler-Falsified-If: this answers any opcode other than CopyScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CopyScope;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DeclareGlobal"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6FC9D1
    // Broiler-Falsified-If: this mode names an opcode other than DeclareGlobal
    // Broiler-Human:        PENDING
    internal readonly struct ArmDeclareGlobal : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeclareGlobal"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8DA33D
        // Broiler-Falsified-If: this answers any opcode other than DeclareGlobal
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeclareGlobal;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.PushObjectScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D5898B
    // Broiler-Falsified-If: this mode names an opcode other than PushObjectScope
    // Broiler-Human:        PENDING
    internal readonly struct ArmPushObjectScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.PushObjectScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2FB524
        // Broiler-Falsified-If: this answers any opcode other than PushObjectScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.PushObjectScope;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ResolveName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=79343F
    // Broiler-Falsified-If: this mode names an opcode other than ResolveName
    // Broiler-Human:        PENDING
    internal readonly struct ArmResolveName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ResolveName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EFF26A
        // Broiler-Falsified-If: this answers any opcode other than ResolveName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ResolveName;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.NewObject"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=218F4B
    // Broiler-Falsified-If: this mode names an opcode other than NewObject
    // Broiler-Human:        PENDING
    internal readonly struct ArmNewObject : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewObject"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A62141
        // Broiler-Falsified-If: this answers any opcode other than NewObject
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewObject;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.NewArray"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D98C06
    // Broiler-Falsified-If: this mode names an opcode other than NewArray
    // Broiler-Human:        PENDING
    internal readonly struct ArmNewArray : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewArray"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CD07BC
        // Broiler-Falsified-If: this answers any opcode other than NewArray
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewArray;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.GetProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F15954
    // Broiler-Falsified-If: this mode names an opcode other than GetProperty
    // Broiler-Human:        PENDING
    internal readonly struct ArmGetProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GetProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5835F4
        // Broiler-Falsified-If: this answers any opcode other than GetProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GetProperty;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SetProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=761E3B
    // Broiler-Falsified-If: this mode names an opcode other than SetProperty
    // Broiler-Human:        PENDING
    internal readonly struct ArmSetProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SetProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F1EBDB
        // Broiler-Falsified-If: this answers any opcode other than SetProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SetProperty;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.GetIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FFA5E9
    // Broiler-Falsified-If: this mode names an opcode other than GetIndex
    // Broiler-Human:        PENDING
    internal readonly struct ArmGetIndex : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GetIndex"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=37F5C2
        // Broiler-Falsified-If: this answers any opcode other than GetIndex
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GetIndex;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SetIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D1E0A0
    // Broiler-Falsified-If: this mode names an opcode other than SetIndex
    // Broiler-Human:        PENDING
    internal readonly struct ArmSetIndex : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SetIndex"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=19F768
        // Broiler-Falsified-If: this answers any opcode other than SetIndex
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SetIndex;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DefineField"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C2B8D8
    // Broiler-Falsified-If: this mode names an opcode other than DefineField
    // Broiler-Human:        PENDING
    internal readonly struct ArmDefineField : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineField"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BC936A
        // Broiler-Falsified-If: this answers any opcode other than DefineField
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineField;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DefineIndexed"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FE311B
    // Broiler-Falsified-If: this mode names an opcode other than DefineIndexed
    // Broiler-Human:        PENDING
    internal readonly struct ArmDefineIndexed : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineIndexed"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=83BF75
        // Broiler-Falsified-If: this answers any opcode other than DefineIndexed
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineIndexed;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DeleteProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B7FA0C
    // Broiler-Falsified-If: this mode names an opcode other than DeleteProperty
    // Broiler-Human:        PENDING
    internal readonly struct ArmDeleteProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CCED5A
        // Broiler-Falsified-If: this answers any opcode other than DeleteProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteProperty;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DeleteIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B5D4B8
    // Broiler-Falsified-If: this mode names an opcode other than DeleteIndex
    // Broiler-Human:        PENDING
    internal readonly struct ArmDeleteIndex : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteIndex"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2C1D9C
        // Broiler-Falsified-If: this answers any opcode other than DeleteIndex
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteIndex;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DefineGetter"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C15C85
    // Broiler-Falsified-If: this mode names an opcode other than DefineGetter
    // Broiler-Human:        PENDING
    internal readonly struct ArmDefineGetter : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineGetter"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2316AF
        // Broiler-Falsified-If: this answers any opcode other than DefineGetter
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineGetter;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DefineSetter"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B0359A
    // Broiler-Falsified-If: this mode names an opcode other than DefineSetter
    // Broiler-Human:        PENDING
    internal readonly struct ArmDefineSetter : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineSetter"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E0C41F
        // Broiler-Falsified-If: this answers any opcode other than DefineSetter
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineSetter;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DefineMethod"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=04784B
    // Broiler-Falsified-If: this mode names an opcode other than DefineMethod
    // Broiler-Human:        PENDING
    internal readonly struct ArmDefineMethod : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineMethod"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=00E07A
        // Broiler-Falsified-If: this answers any opcode other than DefineMethod
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineMethod;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadSuperProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DC6EEE
    // Broiler-Falsified-If: this mode names an opcode other than LoadSuperProperty
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadSuperProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadSuperProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3430FE
        // Broiler-Falsified-If: this answers any opcode other than LoadSuperProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadSuperProperty;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StoreSuperProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B1D68B
    // Broiler-Falsified-If: this mode names an opcode other than StoreSuperProperty
    // Broiler-Human:        PENDING
    internal readonly struct ArmStoreSuperProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreSuperProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9AB412
        // Broiler-Falsified-If: this answers any opcode other than StoreSuperProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreSuperProperty;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ArrayAppend"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=857B55
    // Broiler-Falsified-If: this mode names an opcode other than ArrayAppend
    // Broiler-Human:        PENDING
    internal readonly struct ArmArrayAppend : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ArrayAppend"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D01310
        // Broiler-Falsified-If: this answers any opcode other than ArrayAppend
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ArrayAppend;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Closure"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3C8014
    // Broiler-Falsified-If: this mode names an opcode other than Closure
    // Broiler-Human:        PENDING
    internal readonly struct ArmClosure : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Closure"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B9B08E
        // Broiler-Falsified-If: this answers any opcode other than Closure
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Closure;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Call"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D1D7BE
    // Broiler-Falsified-If: this mode names an opcode other than Call
    // Broiler-Human:        PENDING
    internal readonly struct ArmCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Call"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AEA809
        // Broiler-Falsified-If: this answers any opcode other than Call
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Call;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Construct"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=67FA34
    // Broiler-Falsified-If: this mode names an opcode other than Construct
    // Broiler-Human:        PENDING
    internal readonly struct ArmConstruct : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Construct"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AB9410
        // Broiler-Falsified-If: this answers any opcode other than Construct
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Construct;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Return"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F7DDAF
    // Broiler-Falsified-If: this mode names an opcode other than Return
    // Broiler-Human:        PENDING
    internal readonly struct ArmReturn : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Return"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EF4A69
        // Broiler-Falsified-If: this answers any opcode other than Return
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Return;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ReturnUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F3794E
    // Broiler-Falsified-If: this mode names an opcode other than ReturnUndefined
    // Broiler-Human:        PENDING
    internal readonly struct ArmReturnUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ReturnUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7D9B6E
        // Broiler-Falsified-If: this answers any opcode other than ReturnUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ReturnUndefined;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.CallEval"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A69582
    // Broiler-Falsified-If: this mode names an opcode other than CallEval
    // Broiler-Human:        PENDING
    internal readonly struct ArmCallEval : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallEval"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D5D4C4
        // Broiler-Falsified-If: this answers any opcode other than CallEval
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CallEval;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SuperCall"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4449B4
    // Broiler-Falsified-If: this mode names an opcode other than SuperCall
    // Broiler-Human:        PENDING
    internal readonly struct ArmSuperCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCall"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=716B4E
        // Broiler-Falsified-If: this answers any opcode other than SuperCall
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCall;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SuperCallForwarded"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4B4F91
    // Broiler-Falsified-If: this mode names an opcode other than SuperCallForwarded
    // Broiler-Human:        PENDING
    internal readonly struct ArmSuperCallForwarded : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCallForwarded"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6D1190
        // Broiler-Falsified-If: this answers any opcode other than SuperCallForwarded
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCallForwarded;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.NewClass"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B05163
    // Broiler-Falsified-If: this mode names an opcode other than NewClass
    // Broiler-Human:        PENDING
    internal readonly struct ArmNewClass : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewClass"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=254D0F
        // Broiler-Falsified-If: this answers any opcode other than NewClass
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewClass;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ArrayHoles"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8D4218
    // Broiler-Falsified-If: this mode names an opcode other than ArrayHoles
    // Broiler-Human:        PENDING
    internal readonly struct ArmArrayHoles : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ArrayHoles"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A0D86F
        // Broiler-Falsified-If: this answers any opcode other than ArrayHoles
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ArrayHoles;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SpreadArray"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8E0559
    // Broiler-Falsified-If: this mode names an opcode other than SpreadArray
    // Broiler-Human:        PENDING
    internal readonly struct ArmSpreadArray : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SpreadArray"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=10C694
        // Broiler-Falsified-If: this answers any opcode other than SpreadArray
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SpreadArray;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SpreadObject"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6BF9F0
    // Broiler-Falsified-If: this mode names an opcode other than SpreadObject
    // Broiler-Human:        PENDING
    internal readonly struct ArmSpreadObject : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SpreadObject"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=90CFF0
        // Broiler-Falsified-If: this answers any opcode other than SpreadObject
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SpreadObject;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.CallSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=319A52
    // Broiler-Falsified-If: this mode names an opcode other than CallSpread
    // Broiler-Human:        PENDING
    internal readonly struct ArmCallSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B10BF3
        // Broiler-Falsified-If: this answers any opcode other than CallSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CallSpread;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ConstructSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=69BDA6
    // Broiler-Falsified-If: this mode names an opcode other than ConstructSpread
    // Broiler-Human:        PENDING
    internal readonly struct ArmConstructSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ConstructSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A344AA
        // Broiler-Falsified-If: this answers any opcode other than ConstructSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ConstructSpread;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SuperCallSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=000FCD
    // Broiler-Falsified-If: this mode names an opcode other than SuperCallSpread
    // Broiler-Human:        PENDING
    internal readonly struct ArmSuperCallSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCallSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=935410
        // Broiler-Falsified-If: this answers any opcode other than SuperCallSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCallSpread;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.SetPrototypeLiteral"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DE8A44
    // Broiler-Falsified-If: this mode names an opcode other than SetPrototypeLiteral
    // Broiler-Human:        PENDING
    internal readonly struct ArmSetPrototypeLiteral : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SetPrototypeLiteral"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F076A9
        // Broiler-Falsified-If: this answers any opcode other than SetPrototypeLiteral
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SetPrototypeLiteral;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Add"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0F9A51
    // Broiler-Falsified-If: this mode names an opcode other than Add
    // Broiler-Human:        PENDING
    internal readonly struct ArmAdd : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Add"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5AF05B
        // Broiler-Falsified-If: this answers any opcode other than Add
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Add;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Subtract"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5C6988
    // Broiler-Falsified-If: this mode names an opcode other than Subtract
    // Broiler-Human:        PENDING
    internal readonly struct ArmSubtract : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Subtract"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5768D1
        // Broiler-Falsified-If: this answers any opcode other than Subtract
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Subtract;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Multiply"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BC373B
    // Broiler-Falsified-If: this mode names an opcode other than Multiply
    // Broiler-Human:        PENDING
    internal readonly struct ArmMultiply : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Multiply"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E0007F
        // Broiler-Falsified-If: this answers any opcode other than Multiply
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Multiply;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Divide"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2AA909
    // Broiler-Falsified-If: this mode names an opcode other than Divide
    // Broiler-Human:        PENDING
    internal readonly struct ArmDivide : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Divide"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=84FD41
        // Broiler-Falsified-If: this answers any opcode other than Divide
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Divide;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Remainder"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3482F4
    // Broiler-Falsified-If: this mode names an opcode other than Remainder
    // Broiler-Human:        PENDING
    internal readonly struct ArmRemainder : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Remainder"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=129811
        // Broiler-Falsified-If: this answers any opcode other than Remainder
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Remainder;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Exponent"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=10A908
    // Broiler-Falsified-If: this mode names an opcode other than Exponent
    // Broiler-Human:        PENDING
    internal readonly struct ArmExponent : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Exponent"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=914BD7
        // Broiler-Falsified-If: this answers any opcode other than Exponent
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Exponent;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Negate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7210AB
    // Broiler-Falsified-If: this mode names an opcode other than Negate
    // Broiler-Human:        PENDING
    internal readonly struct ArmNegate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Negate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C79B7B
        // Broiler-Falsified-If: this answers any opcode other than Negate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Negate;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ToNumber"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=83BA2E
    // Broiler-Falsified-If: this mode names an opcode other than ToNumber
    // Broiler-Human:        PENDING
    internal readonly struct ArmToNumber : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ToNumber"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C36BA6
        // Broiler-Falsified-If: this answers any opcode other than ToNumber
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ToNumber;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Not"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0ADF30
    // Broiler-Falsified-If: this mode names an opcode other than Not
    // Broiler-Human:        PENDING
    internal readonly struct ArmNot : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Not"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=75DF62
        // Broiler-Falsified-If: this answers any opcode other than Not
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Not;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.BitwiseNot"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9A46F3
    // Broiler-Falsified-If: this mode names an opcode other than BitwiseNot
    // Broiler-Human:        PENDING
    internal readonly struct ArmBitwiseNot : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseNot"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F715CA
        // Broiler-Falsified-If: this answers any opcode other than BitwiseNot
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseNot;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LessThan"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FDB8F9
    // Broiler-Falsified-If: this mode names an opcode other than LessThan
    // Broiler-Human:        PENDING
    internal readonly struct ArmLessThan : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LessThan"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EEF32A
        // Broiler-Falsified-If: this answers any opcode other than LessThan
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LessThan;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LessThanOrEqual"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9FBAF0
    // Broiler-Falsified-If: this mode names an opcode other than LessThanOrEqual
    // Broiler-Human:        PENDING
    internal readonly struct ArmLessThanOrEqual : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LessThanOrEqual"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F41DF2
        // Broiler-Falsified-If: this answers any opcode other than LessThanOrEqual
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LessThanOrEqual;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.GreaterThan"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A68416
    // Broiler-Falsified-If: this mode names an opcode other than GreaterThan
    // Broiler-Human:        PENDING
    internal readonly struct ArmGreaterThan : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GreaterThan"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E44031
        // Broiler-Falsified-If: this answers any opcode other than GreaterThan
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GreaterThan;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.GreaterThanOrEqual"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=991D86
    // Broiler-Falsified-If: this mode names an opcode other than GreaterThanOrEqual
    // Broiler-Human:        PENDING
    internal readonly struct ArmGreaterThanOrEqual : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GreaterThanOrEqual"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6DF1AF
        // Broiler-Falsified-If: this answers any opcode other than GreaterThanOrEqual
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GreaterThanOrEqual;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StrictEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AB39EF
    // Broiler-Falsified-If: this mode names an opcode other than StrictEquals
    // Broiler-Human:        PENDING
    internal readonly struct ArmStrictEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StrictEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A8BF70
        // Broiler-Falsified-If: this answers any opcode other than StrictEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StrictEquals;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StrictNotEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8A7F32
    // Broiler-Falsified-If: this mode names an opcode other than StrictNotEquals
    // Broiler-Human:        PENDING
    internal readonly struct ArmStrictNotEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StrictNotEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1F0424
        // Broiler-Falsified-If: this answers any opcode other than StrictNotEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StrictNotEquals;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LooseEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1C2C46
    // Broiler-Falsified-If: this mode names an opcode other than LooseEquals
    // Broiler-Human:        PENDING
    internal readonly struct ArmLooseEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LooseEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4B4670
        // Broiler-Falsified-If: this answers any opcode other than LooseEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LooseEquals;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LooseNotEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=717825
    // Broiler-Falsified-If: this mode names an opcode other than LooseNotEquals
    // Broiler-Human:        PENDING
    internal readonly struct ArmLooseNotEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LooseNotEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4EE2CC
        // Broiler-Falsified-If: this answers any opcode other than LooseNotEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LooseNotEquals;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.BitwiseOr"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2A9347
    // Broiler-Falsified-If: this mode names an opcode other than BitwiseOr
    // Broiler-Human:        PENDING
    internal readonly struct ArmBitwiseOr : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseOr"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=28A30B
        // Broiler-Falsified-If: this answers any opcode other than BitwiseOr
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseOr;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.BitwiseAnd"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6453A1
    // Broiler-Falsified-If: this mode names an opcode other than BitwiseAnd
    // Broiler-Human:        PENDING
    internal readonly struct ArmBitwiseAnd : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseAnd"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D7BEA3
        // Broiler-Falsified-If: this answers any opcode other than BitwiseAnd
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseAnd;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.BitwiseXor"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B01B87
    // Broiler-Falsified-If: this mode names an opcode other than BitwiseXor
    // Broiler-Human:        PENDING
    internal readonly struct ArmBitwiseXor : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseXor"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B60E97
        // Broiler-Falsified-If: this answers any opcode other than BitwiseXor
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseXor;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ShiftLeft"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B23F8D
    // Broiler-Falsified-If: this mode names an opcode other than ShiftLeft
    // Broiler-Human:        PENDING
    internal readonly struct ArmShiftLeft : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ShiftLeft"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0FC726
        // Broiler-Falsified-If: this answers any opcode other than ShiftLeft
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ShiftLeft;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ShiftRight"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CEFB22
    // Broiler-Falsified-If: this mode names an opcode other than ShiftRight
    // Broiler-Human:        PENDING
    internal readonly struct ArmShiftRight : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ShiftRight"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=58841A
        // Broiler-Falsified-If: this answers any opcode other than ShiftRight
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ShiftRight;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ShiftRightUnsigned"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AF9597
    // Broiler-Falsified-If: this mode names an opcode other than ShiftRightUnsigned
    // Broiler-Human:        PENDING
    internal readonly struct ArmShiftRightUnsigned : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ShiftRightUnsigned"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A421C0
        // Broiler-Falsified-If: this answers any opcode other than ShiftRightUnsigned
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ShiftRightUnsigned;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.TypeOf"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9D6D91
    // Broiler-Falsified-If: this mode names an opcode other than TypeOf
    // Broiler-Human:        PENDING
    internal readonly struct ArmTypeOf : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.TypeOf"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EA8853
        // Broiler-Falsified-If: this answers any opcode other than TypeOf
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.TypeOf;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.InstanceOf"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=882998
    // Broiler-Falsified-If: this mode names an opcode other than InstanceOf
    // Broiler-Human:        PENDING
    internal readonly struct ArmInstanceOf : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.InstanceOf"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7A23DA
        // Broiler-Falsified-If: this answers any opcode other than InstanceOf
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.InstanceOf;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.In"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=58E843
    // Broiler-Falsified-If: this mode names an opcode other than In
    // Broiler-Human:        PENDING
    internal readonly struct ArmIn : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.In"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2A1104
        // Broiler-Falsified-If: this answers any opcode other than In
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.In;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Void"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F86808
    // Broiler-Falsified-If: this mode names an opcode other than Void
    // Broiler-Human:        PENDING
    internal readonly struct ArmVoid : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Void"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=172D6A
        // Broiler-Falsified-If: this answers any opcode other than Void
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Void;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.RequireCoercible"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BD7E84
    // Broiler-Falsified-If: this mode names an opcode other than RequireCoercible
    // Broiler-Human:        PENDING
    internal readonly struct ArmRequireCoercible : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.RequireCoercible"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=DD736E
        // Broiler-Falsified-If: this answers any opcode other than RequireCoercible
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.RequireCoercible;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ToPropertyKey"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0C60CA
    // Broiler-Falsified-If: this mode names an opcode other than ToPropertyKey
    // Broiler-Human:        PENDING
    internal readonly struct ArmToPropertyKey : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ToPropertyKey"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E6C46D
        // Broiler-Falsified-If: this answers any opcode other than ToPropertyKey
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ToPropertyKey;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.GetTemplateObject"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=824432
    // Broiler-Falsified-If: this mode names an opcode other than GetTemplateObject
    // Broiler-Human:        PENDING
    internal readonly struct ArmGetTemplateObject : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GetTemplateObject"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=448C7D
        // Broiler-Falsified-If: this answers any opcode other than GetTemplateObject
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GetTemplateObject;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Jump"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1A3FEF
    // Broiler-Falsified-If: this mode names an opcode other than Jump
    // Broiler-Human:        PENDING
    internal readonly struct ArmJump : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Jump"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FDAF9E
        // Broiler-Falsified-If: this answers any opcode other than Jump
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Jump;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.JumpIfFalse"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=775656
    // Broiler-Falsified-If: this mode names an opcode other than JumpIfFalse
    // Broiler-Human:        PENDING
    internal readonly struct ArmJumpIfFalse : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.JumpIfFalse"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5DB205
        // Broiler-Falsified-If: this answers any opcode other than JumpIfFalse
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.JumpIfFalse;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.JumpIfTrue"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9157F5
    // Broiler-Falsified-If: this mode names an opcode other than JumpIfTrue
    // Broiler-Human:        PENDING
    internal readonly struct ArmJumpIfTrue : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.JumpIfTrue"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5D29ED
        // Broiler-Falsified-If: this answers any opcode other than JumpIfTrue
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.JumpIfTrue;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Throw"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2D7568
    // Broiler-Falsified-If: this mode names an opcode other than Throw
    // Broiler-Human:        PENDING
    internal readonly struct ArmThrow : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Throw"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BAACFC
        // Broiler-Falsified-If: this answers any opcode other than Throw
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Throw;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ForInStart"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8FB7C6
    // Broiler-Falsified-If: this mode names an opcode other than ForInStart
    // Broiler-Human:        PENDING
    internal readonly struct ArmForInStart : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ForInStart"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6C92A5
        // Broiler-Falsified-If: this answers any opcode other than ForInStart
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ForInStart;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ForInNext"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1F0B61
    // Broiler-Falsified-If: this mode names an opcode other than ForInNext
    // Broiler-Human:        PENDING
    internal readonly struct ArmForInNext : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ForInNext"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=40210D
        // Broiler-Falsified-If: this answers any opcode other than ForInNext
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ForInNext;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateStart"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=119212
    // Broiler-Falsified-If: this mode names an opcode other than IterateStart
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateStart : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateStart"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0C1F4F
        // Broiler-Falsified-If: this answers any opcode other than IterateStart
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateStart;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateNext"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=56A32E
    // Broiler-Falsified-If: this mode names an opcode other than IterateNext
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateNext : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateNext"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E8268A
        // Broiler-Falsified-If: this answers any opcode other than IterateNext
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateNext;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateRest"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3F2315
    // Broiler-Falsified-If: this mode names an opcode other than IterateRest
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateRest : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateRest"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D63D37
        // Broiler-Falsified-If: this answers any opcode other than IterateRest
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateRest;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateClose"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=95888B
    // Broiler-Falsified-If: this mode names an opcode other than IterateClose
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateClose : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateClose"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BF8374
        // Broiler-Falsified-If: this answers any opcode other than IterateClose
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateClose;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Yield"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7B7F34
    // Broiler-Falsified-If: this mode names an opcode other than Yield
    // Broiler-Human:        PENDING
    internal readonly struct ArmYield : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Yield"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0E5713
        // Broiler-Falsified-If: this answers any opcode other than Yield
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Yield;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.YieldDelegate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=85183A
    // Broiler-Falsified-If: this mode names an opcode other than YieldDelegate
    // Broiler-Human:        PENDING
    internal readonly struct ArmYieldDelegate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.YieldDelegate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BB6777
        // Broiler-Falsified-If: this answers any opcode other than YieldDelegate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.YieldDelegate;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Await"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A49AA3
    // Broiler-Falsified-If: this mode names an opcode other than Await
    // Broiler-Human:        PENDING
    internal readonly struct ArmAwait : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Await"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2F7951
        // Broiler-Falsified-If: this answers any opcode other than Await
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Await;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadImport"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6D0504
    // Broiler-Falsified-If: this mode names an opcode other than LoadImport
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadImport : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadImport"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=957D8D
        // Broiler-Falsified-If: this answers any opcode other than LoadImport
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadImport;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ThrowImmutable"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8FBEAC
    // Broiler-Falsified-If: this mode names an opcode other than ThrowImmutable
    // Broiler-Human:        PENDING
    internal readonly struct ArmThrowImmutable : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ThrowImmutable"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C977D7
        // Broiler-Falsified-If: this answers any opcode other than ThrowImmutable
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ThrowImmutable;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DefineClassElement"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4F173E
    // Broiler-Falsified-If: this mode names an opcode other than DefineClassElement
    // Broiler-Human:        PENDING
    internal readonly struct ArmDefineClassElement : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineClassElement"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=56A2D2
        // Broiler-Falsified-If: this answers any opcode other than DefineClassElement
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineClassElement;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Pop"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=EE5544
    // Broiler-Falsified-If: this mode names an opcode other than Pop
    // Broiler-Human:        PENDING
    internal readonly struct ArmPop : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Pop"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A61E2D
        // Broiler-Falsified-If: this answers any opcode other than Pop
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Pop;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Duplicate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C37345
    // Broiler-Falsified-If: this mode names an opcode other than Duplicate
    // Broiler-Human:        PENDING
    internal readonly struct ArmDuplicate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Duplicate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3E8968
        // Broiler-Falsified-If: this answers any opcode other than Duplicate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Duplicate;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DuplicateTwo"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5025B9
    // Broiler-Falsified-If: this mode names an opcode other than DuplicateTwo
    // Broiler-Human:        PENDING
    internal readonly struct ArmDuplicateTwo : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DuplicateTwo"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=435C7A
        // Broiler-Falsified-If: this answers any opcode other than DuplicateTwo
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DuplicateTwo;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Swap"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=406123
    // Broiler-Falsified-If: this mode names an opcode other than Swap
    // Broiler-Human:        PENDING
    internal readonly struct ArmSwap : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Swap"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E79E80
        // Broiler-Falsified-If: this answers any opcode other than Swap
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Swap;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Pick"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=913279
    // Broiler-Falsified-If: this mode names an opcode other than Pick
    // Broiler-Human:        PENDING
    internal readonly struct ArmPick : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Pick"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A6A5C6
        // Broiler-Falsified-If: this answers any opcode other than Pick
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Pick;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.NewPrivateName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CF0B2C
    // Broiler-Falsified-If: this mode names an opcode other than NewPrivateName
    // Broiler-Human:        PENDING
    internal readonly struct ArmNewPrivateName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewPrivateName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4B324A
        // Broiler-Falsified-If: this answers any opcode other than NewPrivateName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewPrivateName;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadPrivate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E6151A
    // Broiler-Falsified-If: this mode names an opcode other than LoadPrivate
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadPrivate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadPrivate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E3F8D0
        // Broiler-Falsified-If: this answers any opcode other than LoadPrivate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadPrivate;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StorePrivate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6315E4
    // Broiler-Falsified-If: this mode names an opcode other than StorePrivate
    // Broiler-Human:        PENDING
    internal readonly struct ArmStorePrivate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StorePrivate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E161C3
        // Broiler-Falsified-If: this answers any opcode other than StorePrivate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StorePrivate;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.HasPrivate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1158D1
    // Broiler-Falsified-If: this mode names an opcode other than HasPrivate
    // Broiler-Human:        PENDING
    internal readonly struct ArmHasPrivate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.HasPrivate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=468A35
        // Broiler-Falsified-If: this answers any opcode other than HasPrivate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.HasPrivate;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.RunStaticElements"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=70289D
    // Broiler-Falsified-If: this mode names an opcode other than RunStaticElements
    // Broiler-Human:        PENDING
    internal readonly struct ArmRunStaticElements : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.RunStaticElements"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=276AB0
        // Broiler-Falsified-If: this answers any opcode other than RunStaticElements
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.RunStaticElements;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateStartAsync"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D94B0A
    // Broiler-Falsified-If: this mode names an opcode other than IterateStartAsync
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateStartAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateStartAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4D20AC
        // Broiler-Falsified-If: this answers any opcode other than IterateStartAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateStartAsync;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateNextAsync"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=9EDB63
    // Broiler-Falsified-If: this mode names an opcode other than IterateNextAsync
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateNextAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateNextAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=021DE0
        // Broiler-Falsified-If: this answers any opcode other than IterateNextAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateNextAsync;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateAwaitStep"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=489DEF
    // Broiler-Falsified-If: this mode names an opcode other than IterateAwaitStep
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateAwaitStep : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateAwaitStep"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=988400
        // Broiler-Falsified-If: this answers any opcode other than IterateAwaitStep
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateAwaitStep;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateCloseAsync"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8734A8
    // Broiler-Falsified-If: this mode names an opcode other than IterateCloseAsync
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateCloseAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateCloseAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3D3E70
        // Broiler-Falsified-If: this answers any opcode other than IterateCloseAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateCloseAsync;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.IterateCloseCheck"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=E0A0CC
    // Broiler-Falsified-If: this mode names an opcode other than IterateCloseCheck
    // Broiler-Human:        PENDING
    internal readonly struct ArmIterateCloseCheck : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateCloseCheck"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=940D59
        // Broiler-Falsified-If: this answers any opcode other than IterateCloseCheck
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateCloseCheck;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DeclareGlobalLet"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8CC6A1
    // Broiler-Falsified-If: this mode names an opcode other than DeclareGlobalLet
    // Broiler-Human:        PENDING
    internal readonly struct ArmDeclareGlobalLet : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeclareGlobalLet"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FFF17F
        // Broiler-Falsified-If: this answers any opcode other than DeclareGlobalLet
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeclareGlobalLet;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DeclareGlobalConst"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A2EC49
    // Broiler-Falsified-If: this mode names an opcode other than DeclareGlobalConst
    // Broiler-Human:        PENDING
    internal readonly struct ArmDeclareGlobalConst : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeclareGlobalConst"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C6B233
        // Broiler-Falsified-If: this answers any opcode other than DeclareGlobalConst
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeclareGlobalConst;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.InitialiseGlobalLexical"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CE3114
    // Broiler-Falsified-If: this mode names an opcode other than InitialiseGlobalLexical
    // Broiler-Human:        PENDING
    internal readonly struct ArmInitialiseGlobalLexical : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.InitialiseGlobalLexical"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2F78D2
        // Broiler-Falsified-If: this answers any opcode other than InitialiseGlobalLexical
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.InitialiseGlobalLexical;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DeleteGlobalBinding"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=4D296D
    // Broiler-Falsified-If: this mode names an opcode other than DeleteGlobalBinding
    // Broiler-Human:        PENDING
    internal readonly struct ArmDeleteGlobalBinding : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteGlobalBinding"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BFB32C
        // Broiler-Falsified-If: this answers any opcode other than DeleteGlobalBinding
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteGlobalBinding;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.EnterBody"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5D36F6
    // Broiler-Falsified-If: this mode names an opcode other than EnterBody
    // Broiler-Human:        PENDING
    internal readonly struct ArmEnterBody : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.EnterBody"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BD1929
        // Broiler-Falsified-If: this answers any opcode other than EnterBody
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.EnterBody;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ImportCall"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=ED7F43
    // Broiler-Falsified-If: this mode names an opcode other than ImportCall
    // Broiler-Human:        PENDING
    internal readonly struct ArmImportCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ImportCall"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=33B0A7
        // Broiler-Falsified-If: this answers any opcode other than ImportCall
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ImportCall;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ImportMeta"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D72C5E
    // Broiler-Falsified-If: this mode names an opcode other than ImportMeta
    // Broiler-Human:        PENDING
    internal readonly struct ArmImportMeta : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ImportMeta"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2803F5
        // Broiler-Falsified-If: this answers any opcode other than ImportMeta
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ImportMeta;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.CallEvalSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B149F0
    // Broiler-Falsified-If: this mode names an opcode other than CallEvalSpread
    // Broiler-Human:        PENDING
    internal readonly struct ArmCallEvalSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallEvalSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1FA07A
        // Broiler-Falsified-If: this answers any opcode other than CallEvalSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CallEvalSpread;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadEvalName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=53DAD0
    // Broiler-Falsified-If: this mode names an opcode other than LoadEvalName
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadEvalName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadEvalName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=98D74C
        // Broiler-Falsified-If: this answers any opcode other than LoadEvalName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadEvalName;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadEvalNameOrUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2AFD77
    // Broiler-Falsified-If: this mode names an opcode other than LoadEvalNameOrUndefined
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadEvalNameOrUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadEvalNameOrUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=407A2F
        // Broiler-Falsified-If: this answers any opcode other than LoadEvalNameOrUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadEvalNameOrUndefined;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StoreEvalName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B76858
    // Broiler-Falsified-If: this mode names an opcode other than StoreEvalName
    // Broiler-Human:        PENDING
    internal readonly struct ArmStoreEvalName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreEvalName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8F0D6F
        // Broiler-Falsified-If: this answers any opcode other than StoreEvalName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreEvalName;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.LoadEvalNameWithBase"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5B722F
    // Broiler-Falsified-If: this mode names an opcode other than LoadEvalNameWithBase
    // Broiler-Human:        PENDING
    internal readonly struct ArmLoadEvalNameWithBase : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadEvalNameWithBase"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=697B36
        // Broiler-Falsified-If: this answers any opcode other than LoadEvalNameWithBase
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadEvalNameWithBase;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DeleteEvalName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D3D865
    // Broiler-Falsified-If: this mode names an opcode other than DeleteEvalName
    // Broiler-Human:        PENDING
    internal readonly struct ArmDeleteEvalName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteEvalName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1EF902
        // Broiler-Falsified-If: this answers any opcode other than DeleteEvalName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteEvalName;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.WithBaseObject"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=47CE85
    // Broiler-Falsified-If: this mode names an opcode other than WithBaseObject
    // Broiler-Human:        PENDING
    internal readonly struct ArmWithBaseObject : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.WithBaseObject"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=ED1AB4
        // Broiler-Falsified-If: this answers any opcode other than WithBaseObject
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.WithBaseObject;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.StoreEvalVariable"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=908837
    // Broiler-Falsified-If: this mode names an opcode other than StoreEvalVariable
    // Broiler-Human:        PENDING
    internal readonly struct ArmStoreEvalVariable : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreEvalVariable"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CF3F82
        // Broiler-Falsified-If: this answers any opcode other than StoreEvalVariable
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreEvalVariable;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DisposeScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=750866
    // Broiler-Falsified-If: this mode names an opcode other than DisposeScope
    // Broiler-Human:        PENDING
    internal readonly struct ArmDisposeScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DisposeScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=72A086
        // Broiler-Falsified-If: this answers any opcode other than DisposeScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DisposeScope;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DisposeAdd"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=85EA4F
    // Broiler-Falsified-If: this mode names an opcode other than DisposeAdd
    // Broiler-Human:        PENDING
    internal readonly struct ArmDisposeAdd : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DisposeAdd"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5DE6AA
        // Broiler-Falsified-If: this answers any opcode other than DisposeAdd
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DisposeAdd;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DisposeFold"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=401FE0
    // Broiler-Falsified-If: this mode names an opcode other than DisposeFold
    // Broiler-Human:        PENDING
    internal readonly struct ArmDisposeFold : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DisposeFold"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=12D9B0
        // Broiler-Falsified-If: this answers any opcode other than DisposeFold
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DisposeFold;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DisposeStep"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2C51D1
    // Broiler-Falsified-If: this mode names an opcode other than DisposeStep
    // Broiler-Human:        PENDING
    internal readonly struct ArmDisposeStep : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DisposeStep"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2ACA17
        // Broiler-Falsified-If: this answers any opcode other than DisposeStep
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DisposeStep;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.DisposeEnd"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6EAB71
    // Broiler-Falsified-If: this mode names an opcode other than DisposeEnd
    // Broiler-Human:        PENDING
    internal readonly struct ArmDisposeEnd : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DisposeEnd"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3C4E84
        // Broiler-Falsified-If: this answers any opcode other than DisposeEnd
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DisposeEnd;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.ToNumeric"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FD9D65
    // Broiler-Falsified-If: this mode names an opcode other than ToNumeric
    // Broiler-Human:        PENDING
    internal readonly struct ArmToNumeric : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ToNumeric"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D773B4
        // Broiler-Falsified-If: this answers any opcode other than ToNumeric
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ToNumeric;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Increment"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0E339A
    // Broiler-Falsified-If: this mode names an opcode other than Increment
    // Broiler-Human:        PENDING
    internal readonly struct ArmIncrement : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Increment"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2E6F4F
        // Broiler-Falsified-If: this answers any opcode other than Increment
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Increment;
        }
    }

    /// <summary>The one-instruction mode that runs the arm of <see cref="JsOpcode.Decrement"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2E3696
    // Broiler-Falsified-If: this mode names an opcode other than Decrement
    // Broiler-Human:        PENDING
    internal readonly struct ArmDecrement : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Decrement"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=92C7FE
        // Broiler-Falsified-If: this answers any opcode other than Decrement
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Decrement;
        }
    }
}
