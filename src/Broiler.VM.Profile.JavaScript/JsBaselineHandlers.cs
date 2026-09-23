// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   201
// Annotated:        201/201
// Exempt:           1
// Human-reviewed:   0/201
// IP risk:          Low
// Security risk:    Critical
// Criteria:         202/202
// Resource impact:  4/10 max
// Unverified:       201
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The baseline form's handler table: one entry point per opcode byte, each running one step of the
/// dispatch loop from the instruction at the offset it is handed.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE TABLE IS INDEXED BY THE OPCODE BYTE ITSELF, SO THERE IS NO SLOT NUMBERING TO DRIFT.</b> An
/// emitted call reads the slot at eight times the opcode, and the slot holds the entry point built for
/// that opcode; a byte no opcode takes holds an entry point that answers a defect and touches nothing.
/// The static constructor fills the table and then checks it - every defined opcode has an entry
/// point of its own and every other byte has the refusing one - and a table that fails the check is
/// never published, which leaves every baseline instantiation refused rather than any of them wrong.
/// </para>
/// <para>
/// <b>THE TABLE IS UNMANAGED MEMORY THAT LIVES FOR THE PROCESS.</b> Emitted code reaches it through
/// the frame's first field and holds nothing else, so it must not move and must not be collected; it
/// is allocated once, written once here, and never freed. It holds addresses of entry points and no
/// reference the collector traces.
/// </para>
/// <para>
/// <b>AN ENTRY POINT WHOSE OPCODE RUNS ALONE IS A PER-OPCODE INSTANTIATION OF THE INTERPRETER'S OWN LOOP,
/// AND EVERY OTHER ENTRY POINT IS THE BLOCK INSTANTIATION.</b> Which opcodes run alone is
/// <see cref="JsBaselineBlocks.RunsAlone"/>'s answer and no list here. A run-alone step's type names the
/// opcode as a constant, so the compiled step keeps one arm of the switch and a guest call nested under
/// it sits under a small frame. Every other entry point runs <see cref="JsStepBlock"/>, which reads each
/// opcode from the code and runs from the head it is handed until the partition ends the block; it holds
/// every arm, and it is never on the stack under a call the instructions that run alone make. Setting
/// <see cref="PerOpcodeSteps"/> to false routes the run-alone entry points through the block
/// instantiation too, which at a run-alone head runs exactly that instruction, with the same checks and
/// the same semantics.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=20AD4A
// Broiler-Falsified-If: an emitted call through this table reaches code other than one step of the dispatch loop or the refusing entry point
// Broiler-Human:        PENDING
internal static unsafe class JsBaselineHandlers
{
    /// <summary>
    /// Whether each run-alone entry point runs its own per-opcode step or the shared block step; every
    /// other entry point always runs the shared block step.
    /// </summary>
    /// <remarks>
    /// <b>A flip is not a like-for-like fallback for stack depth.</b> With it false, a call made by an
    /// instruction that runs alone nests the block instantiation's frame, which holds every arm, on every
    /// level of a recursion through it, so what the call-depth margin was measured on no longer holds.
    /// What it cannot switch is block granularity: that changes the bytes a unit is emitted as.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=6B18D3
    // Broiler-Falsified-If: the two settings produce a different JavaScript answer for any program
    // Broiler-Human:        PENDING
    internal const bool PerOpcodeSteps = true;

    /// <summary>The base address of the handler table, or zero if the table failed its check.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=FD6363
    // Broiler-Falsified-If: this is non-zero while a defined opcode's slot holds anything but that opcode's own entry point, or an undefined byte's slot holds anything but the refusing one
    // Broiler-Human:        PENDING
    internal static readonly nint Table;

    /// <summary>Fills the table, checks it, and publishes it.</summary>
    /// <remarks>
    /// <b>A static constructor and not a module initializer</b>, so the table is built the first time
    /// a baseline instance asks for it and never in a process that runs only bytecode.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=63B4DD
    // Broiler-Falsified-If: the published table maps a defined opcode byte to an entry point built for another opcode, or an undefined byte to anything but the refusing entry point
    // Broiler-Human:        PENDING
    static JsBaselineHandlers()
    {
        var undefined = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Undefined;
        var slots = new nint[JsBaselineAbi.HandlerSlots];
        System.Array.Fill(slots, undefined);

        slots[(int)JsOpcode.Nop] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Nop;
        slots[(int)JsOpcode.LoadUndefined] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadUndefined;
        slots[(int)JsOpcode.LoadNull] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadNull;
        slots[(int)JsOpcode.LoadTrue] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadTrue;
        slots[(int)JsOpcode.LoadFalse] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadFalse;
        slots[(int)JsOpcode.LoadConstant] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadConstant;
        slots[(int)JsOpcode.LoadThis] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadThis;
        slots[(int)JsOpcode.NewArguments] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&NewArguments;
        slots[(int)JsOpcode.LoadNewTarget] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadNewTarget;
        slots[(int)JsOpcode.LoadArgument] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadArgument;
        slots[(int)JsOpcode.RestArguments] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&RestArguments;
        slots[(int)JsOpcode.LoadScoped] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadScoped;
        slots[(int)JsOpcode.StoreScoped] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StoreScoped;
        slots[(int)JsOpcode.InitialiseScoped] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&InitialiseScoped;
        slots[(int)JsOpcode.LoadGlobal] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadGlobal;
        slots[(int)JsOpcode.StoreGlobal] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StoreGlobal;
        slots[(int)JsOpcode.LoadGlobalOrUndefined] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadGlobalOrUndefined;
        slots[(int)JsOpcode.PushScope] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&PushScope;
        slots[(int)JsOpcode.PopScope] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&PopScope;
        slots[(int)JsOpcode.CopyScope] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&CopyScope;
        slots[(int)JsOpcode.DeclareGlobal] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DeclareGlobal;
        slots[(int)JsOpcode.PushObjectScope] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&PushObjectScope;
        slots[(int)JsOpcode.ResolveName] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ResolveName;
        slots[(int)JsOpcode.NewObject] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&NewObject;
        slots[(int)JsOpcode.NewArray] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&NewArray;
        slots[(int)JsOpcode.GetProperty] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&GetProperty;
        slots[(int)JsOpcode.SetProperty] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SetProperty;
        slots[(int)JsOpcode.GetIndex] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&GetIndex;
        slots[(int)JsOpcode.SetIndex] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SetIndex;
        slots[(int)JsOpcode.DefineField] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DefineField;
        slots[(int)JsOpcode.DefineIndexed] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DefineIndexed;
        slots[(int)JsOpcode.DeleteProperty] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DeleteProperty;
        slots[(int)JsOpcode.DeleteIndex] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DeleteIndex;
        slots[(int)JsOpcode.DefineGetter] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DefineGetter;
        slots[(int)JsOpcode.DefineSetter] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DefineSetter;
        slots[(int)JsOpcode.DefineMethod] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DefineMethod;
        slots[(int)JsOpcode.LoadSuperProperty] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadSuperProperty;
        slots[(int)JsOpcode.StoreSuperProperty] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StoreSuperProperty;
        slots[(int)JsOpcode.ArrayAppend] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ArrayAppend;
        slots[(int)JsOpcode.Closure] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Closure;
        slots[(int)JsOpcode.Call] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Call;
        slots[(int)JsOpcode.Construct] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Construct;
        slots[(int)JsOpcode.Return] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Return;
        slots[(int)JsOpcode.ReturnUndefined] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ReturnUndefined;
        slots[(int)JsOpcode.CallEval] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&CallEval;
        slots[(int)JsOpcode.SuperCall] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SuperCall;
        slots[(int)JsOpcode.SuperCallForwarded] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SuperCallForwarded;
        slots[(int)JsOpcode.NewClass] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&NewClass;
        slots[(int)JsOpcode.ArrayHoles] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ArrayHoles;
        slots[(int)JsOpcode.SpreadArray] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SpreadArray;
        slots[(int)JsOpcode.SpreadObject] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SpreadObject;
        slots[(int)JsOpcode.CallSpread] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&CallSpread;
        slots[(int)JsOpcode.ConstructSpread] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ConstructSpread;
        slots[(int)JsOpcode.SuperCallSpread] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SuperCallSpread;
        slots[(int)JsOpcode.SetPrototypeLiteral] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&SetPrototypeLiteral;
        slots[(int)JsOpcode.Add] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Add;
        slots[(int)JsOpcode.Subtract] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Subtract;
        slots[(int)JsOpcode.Multiply] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Multiply;
        slots[(int)JsOpcode.Divide] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Divide;
        slots[(int)JsOpcode.Remainder] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Remainder;
        slots[(int)JsOpcode.Exponent] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Exponent;
        slots[(int)JsOpcode.Negate] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Negate;
        slots[(int)JsOpcode.ToNumber] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ToNumber;
        slots[(int)JsOpcode.Not] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Not;
        slots[(int)JsOpcode.BitwiseNot] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&BitwiseNot;
        slots[(int)JsOpcode.LessThan] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LessThan;
        slots[(int)JsOpcode.LessThanOrEqual] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LessThanOrEqual;
        slots[(int)JsOpcode.GreaterThan] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&GreaterThan;
        slots[(int)JsOpcode.GreaterThanOrEqual] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&GreaterThanOrEqual;
        slots[(int)JsOpcode.StrictEquals] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StrictEquals;
        slots[(int)JsOpcode.StrictNotEquals] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StrictNotEquals;
        slots[(int)JsOpcode.LooseEquals] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LooseEquals;
        slots[(int)JsOpcode.LooseNotEquals] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LooseNotEquals;
        slots[(int)JsOpcode.BitwiseOr] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&BitwiseOr;
        slots[(int)JsOpcode.BitwiseAnd] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&BitwiseAnd;
        slots[(int)JsOpcode.BitwiseXor] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&BitwiseXor;
        slots[(int)JsOpcode.ShiftLeft] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ShiftLeft;
        slots[(int)JsOpcode.ShiftRight] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ShiftRight;
        slots[(int)JsOpcode.ShiftRightUnsigned] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ShiftRightUnsigned;
        slots[(int)JsOpcode.TypeOf] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&TypeOf;
        slots[(int)JsOpcode.InstanceOf] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&InstanceOf;
        slots[(int)JsOpcode.In] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&In;
        slots[(int)JsOpcode.Void] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Void;
        slots[(int)JsOpcode.RequireCoercible] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&RequireCoercible;
        slots[(int)JsOpcode.ToPropertyKey] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ToPropertyKey;
        slots[(int)JsOpcode.GetTemplateObject] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&GetTemplateObject;
        slots[(int)JsOpcode.Jump] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Jump;
        slots[(int)JsOpcode.JumpIfFalse] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&JumpIfFalse;
        slots[(int)JsOpcode.JumpIfTrue] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&JumpIfTrue;
        slots[(int)JsOpcode.Throw] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Throw;
        slots[(int)JsOpcode.ForInStart] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ForInStart;
        slots[(int)JsOpcode.ForInNext] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ForInNext;
        slots[(int)JsOpcode.IterateStart] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateStart;
        slots[(int)JsOpcode.IterateNext] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateNext;
        slots[(int)JsOpcode.IterateRest] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateRest;
        slots[(int)JsOpcode.IterateClose] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateClose;
        slots[(int)JsOpcode.Yield] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Yield;
        slots[(int)JsOpcode.YieldDelegate] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&YieldDelegate;
        slots[(int)JsOpcode.Await] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Await;
        slots[(int)JsOpcode.LoadImport] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadImport;
        slots[(int)JsOpcode.ThrowImmutable] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ThrowImmutable;
        slots[(int)JsOpcode.DefineClassElement] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DefineClassElement;
        slots[(int)JsOpcode.Pop] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Pop;
        slots[(int)JsOpcode.Duplicate] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Duplicate;
        slots[(int)JsOpcode.DuplicateTwo] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DuplicateTwo;
        slots[(int)JsOpcode.Swap] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Swap;
        slots[(int)JsOpcode.Pick] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Pick;
        slots[(int)JsOpcode.NewPrivateName] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&NewPrivateName;
        slots[(int)JsOpcode.LoadPrivate] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadPrivate;
        slots[(int)JsOpcode.StorePrivate] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StorePrivate;
        slots[(int)JsOpcode.HasPrivate] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&HasPrivate;
        slots[(int)JsOpcode.RunStaticElements] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&RunStaticElements;
        slots[(int)JsOpcode.IterateStartAsync] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateStartAsync;
        slots[(int)JsOpcode.IterateNextAsync] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateNextAsync;
        slots[(int)JsOpcode.IterateAwaitStep] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateAwaitStep;
        slots[(int)JsOpcode.IterateCloseAsync] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateCloseAsync;
        slots[(int)JsOpcode.IterateCloseCheck] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&IterateCloseCheck;
        slots[(int)JsOpcode.DeclareGlobalLet] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DeclareGlobalLet;
        slots[(int)JsOpcode.DeclareGlobalConst] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DeclareGlobalConst;
        slots[(int)JsOpcode.InitialiseGlobalLexical] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&InitialiseGlobalLexical;
        slots[(int)JsOpcode.DeleteGlobalBinding] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DeleteGlobalBinding;
        slots[(int)JsOpcode.EnterBody] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&EnterBody;
        slots[(int)JsOpcode.ImportCall] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ImportCall;
        slots[(int)JsOpcode.ImportMeta] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ImportMeta;

        slots[(int)JsOpcode.CallEvalSpread] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&CallEvalSpread;
        slots[(int)JsOpcode.LoadEvalName] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadEvalName;
        slots[(int)JsOpcode.LoadEvalNameOrUndefined] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadEvalNameOrUndefined;
        slots[(int)JsOpcode.StoreEvalName] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StoreEvalName;
        slots[(int)JsOpcode.LoadEvalNameWithBase] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&LoadEvalNameWithBase;
        slots[(int)JsOpcode.DeleteEvalName] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DeleteEvalName;
        slots[(int)JsOpcode.WithBaseObject] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&WithBaseObject;
        slots[(int)JsOpcode.StoreEvalVariable] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&StoreEvalVariable;

        slots[(int)JsOpcode.DisposeScope] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DisposeScope;
        slots[(int)JsOpcode.DisposeAdd] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DisposeAdd;
        slots[(int)JsOpcode.DisposeFold] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DisposeFold;
        slots[(int)JsOpcode.DisposeStep] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DisposeStep;
        slots[(int)JsOpcode.DisposeEnd] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&DisposeEnd;

        slots[(int)JsOpcode.ToNumeric] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&ToNumeric;
        slots[(int)JsOpcode.Increment] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Increment;
        slots[(int)JsOpcode.Decrement] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Decrement;
        if (!Sound(slots, undefined))
        {
            Table = 0;
            return;
        }

        var table = (nint*)System.Runtime.InteropServices.NativeMemory.AllocZeroed(
            (nuint)(JsBaselineAbi.HandlerSlots * sizeof(nint)));

        for (var index = 0; index < slots.Length; index++)
        {
            table[index] = slots[index];
        }

        Table = (nint)table;
    }

    /// <summary>Whether every defined opcode has its own entry point and every other byte the refusing one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=2; Fingerprint=131DD4
    // Broiler-Falsified-If: this answers true for a table in which two defined opcodes share an entry point, a defined opcode has the refusing one, or an undefined byte has any other
    // Broiler-Human:        PENDING
    private static bool Sound(nint[] slots, nint undefined)
    {
        if (slots.Length != JsBaselineAbi.HandlerSlots || undefined == 0)
        {
            return false;
        }

        var seen = new System.Collections.Generic.HashSet<nint>();

        for (var index = 0; index < slots.Length; index++)
        {
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

    /// <summary>The entry point of every byte no opcode takes: it answers a defect and touches nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=DFD818
    // Broiler-Falsified-If: this reads or writes any state, or answers anything but the defect status
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Undefined(JsBaselineFrame* frame, int pc) => (int)JsBaselineStatus.Defect;

    /// <summary>The entry point for <see cref="JsOpcode.Nop"/> (0x00): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=5A6B2F
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Nop at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Nop(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Nop);

    /// <summary>The entry point for <see cref="JsOpcode.LoadUndefined"/> (0x01): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A2C00C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadUndefined at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadUndefined(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.LoadNull"/> (0x02): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=8B9C06
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadNull at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadNull(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadNull);

    /// <summary>The entry point for <see cref="JsOpcode.LoadTrue"/> (0x03): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=2F4548
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadTrue at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadTrue(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadTrue);

    /// <summary>The entry point for <see cref="JsOpcode.LoadFalse"/> (0x04): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=2931C8
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadFalse at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadFalse(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadFalse);

    /// <summary>The entry point for <see cref="JsOpcode.LoadConstant"/> (0x05): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=248FC9
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadConstant at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadConstant(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadConstant);

    /// <summary>The entry point for <see cref="JsOpcode.LoadThis"/> (0x06): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=449067
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadThis at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadThis(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadThis);

    /// <summary>The entry point for <see cref="JsOpcode.NewArguments"/> (0x07): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=C8A81C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one NewArguments at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewArguments(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.NewArguments);

    /// <summary>The entry point for <see cref="JsOpcode.LoadNewTarget"/> (0x08): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BBD78E
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadNewTarget at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadNewTarget(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadNewTarget);

    /// <summary>The entry point for <see cref="JsOpcode.LoadArgument"/> (0x09): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=6F7F6A
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadArgument at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadArgument(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadArgument);

    /// <summary>The entry point for <see cref="JsOpcode.RestArguments"/> (0x0A): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=428012
    // Broiler-Falsified-If: this runs anything other than a block step starting at one RestArguments at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RestArguments(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.RestArguments);

    /// <summary>The entry point for <see cref="JsOpcode.LoadScoped"/> (0x10): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=1615E7
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadScoped at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadScoped(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadScoped);

    /// <summary>The entry point for <see cref="JsOpcode.StoreScoped"/> (0x11): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=FDC027
    // Broiler-Falsified-If: this runs anything other than a block step starting at one StoreScoped at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreScoped(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StoreScoped);

    /// <summary>The entry point for <see cref="JsOpcode.InitialiseScoped"/> (0x12): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4B8647
    // Broiler-Falsified-If: this runs anything other than a block step starting at one InitialiseScoped at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InitialiseScoped(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.InitialiseScoped);

    /// <summary>The entry point for <see cref="JsOpcode.LoadGlobal"/> (0x13): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=C3B020
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadGlobal at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadGlobal(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.StoreGlobal"/> (0x14): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=B243F9
    // Broiler-Falsified-If: this runs anything other than a block step starting at one StoreGlobal at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreGlobal(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StoreGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.LoadGlobalOrUndefined"/> (0x15): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D6C0AD
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadGlobalOrUndefined at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadGlobalOrUndefined(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadGlobalOrUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.PushScope"/> (0x16): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=8F8213
    // Broiler-Falsified-If: this runs anything other than a block step starting at one PushScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PushScope(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.PushScope);

    /// <summary>The entry point for <see cref="JsOpcode.PopScope"/> (0x17): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=00838C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one PopScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PopScope(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.PopScope);

    /// <summary>The entry point for <see cref="JsOpcode.CopyScope"/> (0x18): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=CDAF46
    // Broiler-Falsified-If: this runs anything other than a block step starting at one CopyScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CopyScope(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.CopyScope);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobal"/> (0x19): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=07D66A
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DeclareGlobal at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobal(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DeclareGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.PushObjectScope"/> (0x1A): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=08ABD6
    // Broiler-Falsified-If: this runs anything other than a block step starting at one PushObjectScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PushObjectScope(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.PushObjectScope);

    /// <summary>The entry point for <see cref="JsOpcode.ResolveName"/> (0x1B): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=07F9F9
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ResolveName at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ResolveName(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ResolveName);

    /// <summary>The entry point for <see cref="JsOpcode.NewObject"/> (0x20): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=DBC04D
    // Broiler-Falsified-If: this runs anything other than a block step starting at one NewObject at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewObject(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.NewObject);

    /// <summary>The entry point for <see cref="JsOpcode.NewArray"/> (0x21): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=040661
    // Broiler-Falsified-If: this runs anything other than a block step starting at one NewArray at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewArray(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.NewArray);

    /// <summary>The entry point for <see cref="JsOpcode.GetProperty"/> (0x22): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=272DF4
    // Broiler-Falsified-If: this runs anything other than a block step starting at one GetProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetProperty(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.GetProperty);

    /// <summary>The entry point for <see cref="JsOpcode.SetProperty"/> (0x23): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=963AD0
    // Broiler-Falsified-If: this runs anything other than a block step starting at one SetProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetProperty(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SetProperty);

    /// <summary>The entry point for <see cref="JsOpcode.GetIndex"/> (0x24): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=696E20
    // Broiler-Falsified-If: this runs anything other than a block step starting at one GetIndex at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetIndex(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.GetIndex);

    /// <summary>The entry point for <see cref="JsOpcode.SetIndex"/> (0x25): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=97167E
    // Broiler-Falsified-If: this runs anything other than a block step starting at one SetIndex at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetIndex(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SetIndex);

    /// <summary>The entry point for <see cref="JsOpcode.DefineField"/> (0x26): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=45AEC3
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DefineField at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineField(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DefineField);

    /// <summary>The entry point for <see cref="JsOpcode.DefineIndexed"/> (0x27): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D354DA
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DefineIndexed at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineIndexed(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DefineIndexed);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteProperty"/> (0x28): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=9D3D19
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DeleteProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteProperty(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DeleteProperty);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteIndex"/> (0x29): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=9536FC
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DeleteIndex at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteIndex(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DeleteIndex);

    /// <summary>The entry point for <see cref="JsOpcode.DefineGetter"/> (0x2A): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A4B9D5
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DefineGetter at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineGetter(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DefineGetter);

    /// <summary>The entry point for <see cref="JsOpcode.DefineSetter"/> (0x2B): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=AD625A
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DefineSetter at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineSetter(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DefineSetter);

    /// <summary>The entry point for <see cref="JsOpcode.DefineMethod"/> (0x2C): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=31CC88
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DefineMethod at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineMethod(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DefineMethod);

    /// <summary>The entry point for <see cref="JsOpcode.LoadSuperProperty"/> (0x2D): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E6F89C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadSuperProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadSuperProperty(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadSuperProperty);

    /// <summary>The entry point for <see cref="JsOpcode.StoreSuperProperty"/> (0x2E): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=6F2DF2
    // Broiler-Falsified-If: this runs anything other than a block step starting at one StoreSuperProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreSuperProperty(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StoreSuperProperty);

    /// <summary>The entry point for <see cref="JsOpcode.ArrayAppend"/> (0x2F): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=ACA39C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ArrayAppend at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ArrayAppend(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ArrayAppend);

    /// <summary>The entry point for <see cref="JsOpcode.Closure"/> (0x30): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=F5AF61
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Closure at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Closure(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Closure);

    /// <summary>The entry point for <see cref="JsOpcode.Call"/> (0x31), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=8F4FE3
    // Broiler-Falsified-If: this runs any instruction other than one Call at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Call(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCall>(frame, pc, JsOpcode.Call)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Call);

    /// <summary>The entry point for <see cref="JsOpcode.Construct"/> (0x32), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=8CCB64
    // Broiler-Falsified-If: this runs any instruction other than one Construct at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Construct(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepConstruct>(frame, pc, JsOpcode.Construct)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Construct);

    /// <summary>The entry point for <see cref="JsOpcode.Return"/> (0x33): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=EB1C70
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Return at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Return(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Return);

    /// <summary>The entry point for <see cref="JsOpcode.ReturnUndefined"/> (0x34): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D3C995
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ReturnUndefined at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ReturnUndefined(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ReturnUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.CallEval"/> (0x35), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=98714F
    // Broiler-Falsified-If: this runs any instruction other than one CallEval at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallEval(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCallEval>(frame, pc, JsOpcode.CallEval)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.CallEval);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCall"/> (0x36), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=B5B6F0
    // Broiler-Falsified-If: this runs any instruction other than one SuperCall at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCall(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSuperCall>(frame, pc, JsOpcode.SuperCall)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SuperCall);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCallForwarded"/> (0x37), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=766C71
    // Broiler-Falsified-If: this runs any instruction other than one SuperCallForwarded at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCallForwarded(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSuperCallForwarded>(frame, pc, JsOpcode.SuperCallForwarded)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SuperCallForwarded);

    /// <summary>The entry point for <see cref="JsOpcode.NewClass"/> (0x38): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E18E52
    // Broiler-Falsified-If: this runs anything other than a block step starting at one NewClass at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewClass(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.NewClass);

    /// <summary>The entry point for <see cref="JsOpcode.ArrayHoles"/> (0x39): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=9F10E0
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ArrayHoles at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ArrayHoles(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ArrayHoles);

    /// <summary>The entry point for <see cref="JsOpcode.SpreadArray"/> (0x3A), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=1A3C48
    // Broiler-Falsified-If: this runs any instruction other than one SpreadArray at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SpreadArray(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSpreadArray>(frame, pc, JsOpcode.SpreadArray)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SpreadArray);

    /// <summary>The entry point for <see cref="JsOpcode.SpreadObject"/> (0x3B): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D1B99A
    // Broiler-Falsified-If: this runs anything other than a block step starting at one SpreadObject at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SpreadObject(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SpreadObject);

    /// <summary>The entry point for <see cref="JsOpcode.CallSpread"/> (0x3C), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4C237E
    // Broiler-Falsified-If: this runs any instruction other than one CallSpread at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallSpread(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCallSpread>(frame, pc, JsOpcode.CallSpread)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.CallSpread);

    /// <summary>The entry point for <see cref="JsOpcode.ConstructSpread"/> (0x3D), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A121AF
    // Broiler-Falsified-If: this runs any instruction other than one ConstructSpread at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ConstructSpread(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepConstructSpread>(frame, pc, JsOpcode.ConstructSpread)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ConstructSpread);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCallSpread"/> (0x3E), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=574BF0
    // Broiler-Falsified-If: this runs any instruction other than one SuperCallSpread at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCallSpread(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSuperCallSpread>(frame, pc, JsOpcode.SuperCallSpread)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SuperCallSpread);

    /// <summary>The entry point for <see cref="JsOpcode.SetPrototypeLiteral"/> (0x3F): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=AFD8FD
    // Broiler-Falsified-If: this runs anything other than a block step starting at one SetPrototypeLiteral at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetPrototypeLiteral(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.SetPrototypeLiteral);

    /// <summary>The entry point for <see cref="JsOpcode.Add"/> (0x40): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A8D14B
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Add at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Add(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Add);

    /// <summary>The entry point for <see cref="JsOpcode.Subtract"/> (0x41): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE0031
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Subtract at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Subtract(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Subtract);

    /// <summary>The entry point for <see cref="JsOpcode.Multiply"/> (0x42): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E0CA13
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Multiply at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Multiply(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Multiply);

    /// <summary>The entry point for <see cref="JsOpcode.Divide"/> (0x43): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=FA6A92
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Divide at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Divide(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Divide);

    /// <summary>The entry point for <see cref="JsOpcode.Remainder"/> (0x44): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4B3DED
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Remainder at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Remainder(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Remainder);

    /// <summary>The entry point for <see cref="JsOpcode.Exponent"/> (0x45): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=AE7094
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Exponent at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Exponent(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Exponent);

    /// <summary>The entry point for <see cref="JsOpcode.Negate"/> (0x46): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=EAEF1C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Negate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Negate(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Negate);

    /// <summary>The entry point for <see cref="JsOpcode.ToNumber"/> (0x47): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=F97078
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ToNumber at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ToNumber(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ToNumber);

    /// <summary>The entry point for <see cref="JsOpcode.Not"/> (0x48): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4F9718
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Not at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Not(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Not);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseNot"/> (0x49): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=71BEDB
    // Broiler-Falsified-If: this runs anything other than a block step starting at one BitwiseNot at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseNot(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.BitwiseNot);

    /// <summary>The entry point for <see cref="JsOpcode.LessThan"/> (0x4A): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A7AB60
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LessThan at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LessThan(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LessThan);

    /// <summary>The entry point for <see cref="JsOpcode.LessThanOrEqual"/> (0x4B): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=04D328
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LessThanOrEqual at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LessThanOrEqual(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LessThanOrEqual);

    /// <summary>The entry point for <see cref="JsOpcode.GreaterThan"/> (0x4C): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=08A3C4
    // Broiler-Falsified-If: this runs anything other than a block step starting at one GreaterThan at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GreaterThan(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.GreaterThan);

    /// <summary>The entry point for <see cref="JsOpcode.GreaterThanOrEqual"/> (0x4D): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=1AE0FB
    // Broiler-Falsified-If: this runs anything other than a block step starting at one GreaterThanOrEqual at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GreaterThanOrEqual(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.GreaterThanOrEqual);

    /// <summary>The entry point for <see cref="JsOpcode.StrictEquals"/> (0x4E): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=6B05E8
    // Broiler-Falsified-If: this runs anything other than a block step starting at one StrictEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StrictEquals(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StrictEquals);

    /// <summary>The entry point for <see cref="JsOpcode.StrictNotEquals"/> (0x4F): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=AB3870
    // Broiler-Falsified-If: this runs anything other than a block step starting at one StrictNotEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StrictNotEquals(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StrictNotEquals);

    /// <summary>The entry point for <see cref="JsOpcode.LooseEquals"/> (0x50): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=5B4EA1
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LooseEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LooseEquals(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LooseEquals);

    /// <summary>The entry point for <see cref="JsOpcode.LooseNotEquals"/> (0x51): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=F7564B
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LooseNotEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LooseNotEquals(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LooseNotEquals);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseOr"/> (0x52): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=7D4EC8
    // Broiler-Falsified-If: this runs anything other than a block step starting at one BitwiseOr at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseOr(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.BitwiseOr);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseAnd"/> (0x53): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=0CD898
    // Broiler-Falsified-If: this runs anything other than a block step starting at one BitwiseAnd at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseAnd(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.BitwiseAnd);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseXor"/> (0x54): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E196D4
    // Broiler-Falsified-If: this runs anything other than a block step starting at one BitwiseXor at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseXor(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.BitwiseXor);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftLeft"/> (0x55): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=F8A8FE
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ShiftLeft at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftLeft(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ShiftLeft);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftRight"/> (0x56): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=5BAD97
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ShiftRight at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftRight(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ShiftRight);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftRightUnsigned"/> (0x57): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=F3C276
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ShiftRightUnsigned at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftRightUnsigned(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ShiftRightUnsigned);

    /// <summary>The entry point for <see cref="JsOpcode.TypeOf"/> (0x58): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E772FA
    // Broiler-Falsified-If: this runs anything other than a block step starting at one TypeOf at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int TypeOf(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.TypeOf);

    /// <summary>The entry point for <see cref="JsOpcode.InstanceOf"/> (0x59): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=0F49FC
    // Broiler-Falsified-If: this runs anything other than a block step starting at one InstanceOf at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InstanceOf(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.InstanceOf);

    /// <summary>The entry point for <see cref="JsOpcode.In"/> (0x5A): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4065E6
    // Broiler-Falsified-If: this runs anything other than a block step starting at one In at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int In(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.In);

    /// <summary>The entry point for <see cref="JsOpcode.Void"/> (0x5B): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=C6A2E3
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Void at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Void(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Void);

    /// <summary>The entry point for <see cref="JsOpcode.RequireCoercible"/> (0x5C): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D09ED2
    // Broiler-Falsified-If: this runs anything other than a block step starting at one RequireCoercible at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RequireCoercible(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.RequireCoercible);

    /// <summary>The entry point for <see cref="JsOpcode.ToPropertyKey"/> (0x5D): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=64B161
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ToPropertyKey at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ToPropertyKey(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ToPropertyKey);

    /// <summary>The entry point for <see cref="JsOpcode.GetTemplateObject"/> (0x5E): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=680DFE
    // Broiler-Falsified-If: this runs anything other than a block step starting at one GetTemplateObject at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetTemplateObject(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.GetTemplateObject);

    /// <summary>The entry point for <see cref="JsOpcode.Jump"/> (0x60): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E36AB7
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Jump at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Jump(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Jump);

    /// <summary>The entry point for <see cref="JsOpcode.JumpIfFalse"/> (0x61): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=72F3C2
    // Broiler-Falsified-If: this runs anything other than a block step starting at one JumpIfFalse at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int JumpIfFalse(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.JumpIfFalse);

    /// <summary>The entry point for <see cref="JsOpcode.JumpIfTrue"/> (0x62): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=0EEE7F
    // Broiler-Falsified-If: this runs anything other than a block step starting at one JumpIfTrue at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int JumpIfTrue(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.JumpIfTrue);

    /// <summary>The entry point for <see cref="JsOpcode.Throw"/> (0x63): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=7BE2F7
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Throw at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Throw(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Throw);

    /// <summary>The entry point for <see cref="JsOpcode.ForInStart"/> (0x64): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BDDE7C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ForInStart at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ForInStart(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ForInStart);

    /// <summary>The entry point for <see cref="JsOpcode.ForInNext"/> (0x65): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D6C4A4
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ForInNext at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ForInNext(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ForInNext);

    /// <summary>The entry point for <see cref="JsOpcode.IterateStart"/> (0x66), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A585E6
    // Broiler-Falsified-If: this runs any instruction other than one IterateStart at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateStart(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateStart>(frame, pc, JsOpcode.IterateStart)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateStart);

    /// <summary>The entry point for <see cref="JsOpcode.IterateNext"/> (0x67), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=EECFBB
    // Broiler-Falsified-If: this runs any instruction other than one IterateNext at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateNext(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateNext>(frame, pc, JsOpcode.IterateNext)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateNext);

    /// <summary>The entry point for <see cref="JsOpcode.IterateRest"/> (0x68), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=3CA5FC
    // Broiler-Falsified-If: this runs any instruction other than one IterateRest at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateRest(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateRest>(frame, pc, JsOpcode.IterateRest)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateRest);

    /// <summary>The entry point for <see cref="JsOpcode.IterateClose"/> (0x69), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=9668EC
    // Broiler-Falsified-If: this runs any instruction other than one IterateClose at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateClose(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateClose>(frame, pc, JsOpcode.IterateClose)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateClose);

    /// <summary>The entry point for <see cref="JsOpcode.Yield"/> (0x6A): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=F4A9AF
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Yield at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Yield(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Yield);

    /// <summary>The entry point for <see cref="JsOpcode.YieldDelegate"/> (0x6B), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=526D32
    // Broiler-Falsified-If: this runs any instruction other than one YieldDelegate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int YieldDelegate(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepYieldDelegate>(frame, pc, JsOpcode.YieldDelegate)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.YieldDelegate);

    /// <summary>The entry point for <see cref="JsOpcode.Await"/> (0x6C): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=994FB9
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Await at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Await(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Await);

    /// <summary>The entry point for <see cref="JsOpcode.LoadImport"/> (0x6D): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=9A9071
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadImport at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadImport(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadImport);

    /// <summary>The entry point for <see cref="JsOpcode.ThrowImmutable"/> (0x6E): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=2CCCD5
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ThrowImmutable at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ThrowImmutable(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ThrowImmutable);

    /// <summary>The entry point for <see cref="JsOpcode.DefineClassElement"/> (0x6F): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=94C9EB
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DefineClassElement at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineClassElement(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DefineClassElement);

    /// <summary>The entry point for <see cref="JsOpcode.Pop"/> (0x70): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=286CE5
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Pop at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Pop(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Pop);

    /// <summary>The entry point for <see cref="JsOpcode.Duplicate"/> (0x71): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=0BE055
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Duplicate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Duplicate(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Duplicate);

    /// <summary>The entry point for <see cref="JsOpcode.DuplicateTwo"/> (0x72): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=305D59
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DuplicateTwo at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DuplicateTwo(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DuplicateTwo);

    /// <summary>The entry point for <see cref="JsOpcode.Swap"/> (0x73): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=CFE42C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Swap at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Swap(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Swap);

    /// <summary>The entry point for <see cref="JsOpcode.Pick"/> (0x74): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=9C2483
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Pick at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Pick(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Pick);

    /// <summary>The entry point for <see cref="JsOpcode.NewPrivateName"/> (0x75): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A004F8
    // Broiler-Falsified-If: this runs anything other than a block step starting at one NewPrivateName at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewPrivateName(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.NewPrivateName);

    /// <summary>The entry point for <see cref="JsOpcode.LoadPrivate"/> (0x76): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=7F2E6A
    // Broiler-Falsified-If: this runs anything other than a block step starting at one LoadPrivate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadPrivate(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadPrivate);

    /// <summary>The entry point for <see cref="JsOpcode.StorePrivate"/> (0x77): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=6D6450
    // Broiler-Falsified-If: this runs anything other than a block step starting at one StorePrivate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StorePrivate(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StorePrivate);

    /// <summary>The entry point for <see cref="JsOpcode.HasPrivate"/> (0x78): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=12FE21
    // Broiler-Falsified-If: this runs anything other than a block step starting at one HasPrivate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int HasPrivate(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.HasPrivate);

    /// <summary>The entry point for <see cref="JsOpcode.RunStaticElements"/> (0x79), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E3D87E
    // Broiler-Falsified-If: this runs any instruction other than one RunStaticElements at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RunStaticElements(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepRunStaticElements>(frame, pc, JsOpcode.RunStaticElements)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.RunStaticElements);

    /// <summary>The entry point for <see cref="JsOpcode.IterateStartAsync"/> (0x7A), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4FC94A
    // Broiler-Falsified-If: this runs any instruction other than one IterateStartAsync at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateStartAsync(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateStartAsync>(frame, pc, JsOpcode.IterateStartAsync)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateStartAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateNextAsync"/> (0x7B), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=AE57F8
    // Broiler-Falsified-If: this runs any instruction other than one IterateNextAsync at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateNextAsync(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateNextAsync>(frame, pc, JsOpcode.IterateNextAsync)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateNextAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateAwaitStep"/> (0x7C): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D00330
    // Broiler-Falsified-If: this runs anything other than a block step starting at one IterateAwaitStep at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateAwaitStep(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateAwaitStep);

    /// <summary>The entry point for <see cref="JsOpcode.IterateCloseAsync"/> (0x7D), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=048BF9
    // Broiler-Falsified-If: this runs any instruction other than one IterateCloseAsync at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateCloseAsync(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateCloseAsync>(frame, pc, JsOpcode.IterateCloseAsync)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateCloseAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateCloseCheck"/> (0x7E): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=AD4CEC
    // Broiler-Falsified-If: this runs anything other than a block step starting at one IterateCloseCheck at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateCloseCheck(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.IterateCloseCheck);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobalLet"/> (0x7F): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=3E7F80
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DeclareGlobalLet at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobalLet(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DeclareGlobalLet);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobalConst"/> (0x80): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=505007
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DeclareGlobalConst at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobalConst(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DeclareGlobalConst);

    /// <summary>The entry point for <see cref="JsOpcode.InitialiseGlobalLexical"/> (0x81): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=654457
    // Broiler-Falsified-If: this runs anything other than a block step starting at one InitialiseGlobalLexical at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InitialiseGlobalLexical(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.InitialiseGlobalLexical);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteGlobalBinding"/> (0x82): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=713F2C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DeleteGlobalBinding at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteGlobalBinding(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DeleteGlobalBinding);

    /// <summary>The entry point for <see cref="JsOpcode.EnterBody"/> (0x83): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=D249CF
    // Broiler-Falsified-If: this runs anything other than a block step starting at one EnterBody at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int EnterBody(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.EnterBody);

    /// <summary>The entry point for <see cref="JsOpcode.ImportCall"/> (0x85), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=8B3573
    // Broiler-Falsified-If: this runs any instruction other than one ImportCall at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ImportCall(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepImportCall>(frame, pc, JsOpcode.ImportCall)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ImportCall);

    /// <summary>The entry point for <see cref="JsOpcode.ImportMeta"/> (0x86): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BC6690
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ImportMeta at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ImportMeta(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ImportMeta);

    /// <summary>The entry point for <see cref="JsOpcode.CallEvalSpread"/> (0x90), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=949682
    // Broiler-Falsified-If: this runs any instruction other than one CallEvalSpread at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallEvalSpread(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCallEvalSpread>(frame, pc, JsOpcode.CallEvalSpread)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.CallEvalSpread);

    /// <summary>The entry point for <see cref="JsOpcode.LoadEvalName"/> (0x91), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=1D4439
    // Broiler-Falsified-If: this runs any instruction other than one LoadEvalName at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadEvalName(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadEvalName>(frame, pc, JsOpcode.LoadEvalName)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadEvalName);

    /// <summary>The entry point for <see cref="JsOpcode.LoadEvalNameOrUndefined"/> (0x92), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A0FC49
    // Broiler-Falsified-If: this runs any instruction other than one LoadEvalNameOrUndefined at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadEvalNameOrUndefined(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadEvalNameOrUndefined>(frame, pc, JsOpcode.LoadEvalNameOrUndefined)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadEvalNameOrUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.StoreEvalName"/> (0x93), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=856FEB
    // Broiler-Falsified-If: this runs any instruction other than one StoreEvalName at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreEvalName(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStoreEvalName>(frame, pc, JsOpcode.StoreEvalName)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StoreEvalName);

    /// <summary>The entry point for <see cref="JsOpcode.LoadEvalNameWithBase"/> (0x94), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=64C206
    // Broiler-Falsified-If: this runs any instruction other than one LoadEvalNameWithBase at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadEvalNameWithBase(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadEvalNameWithBase>(frame, pc, JsOpcode.LoadEvalNameWithBase)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.LoadEvalNameWithBase);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteEvalName"/> (0x95), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=3CA5A8
    // Broiler-Falsified-If: this runs any instruction other than one DeleteEvalName at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteEvalName(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDeleteEvalName>(frame, pc, JsOpcode.DeleteEvalName)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DeleteEvalName);

    /// <summary>The entry point for <see cref="JsOpcode.WithBaseObject"/> (0x9A): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=C6F53B
    // Broiler-Falsified-If: this runs anything other than a block step starting at one WithBaseObject at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int WithBaseObject(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.WithBaseObject);

    /// <summary>The entry point for <see cref="JsOpcode.StoreEvalVariable"/> (0x9B), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=247561
    // Broiler-Falsified-If: this runs any instruction other than one StoreEvalVariable at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreEvalVariable(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStoreEvalVariable>(frame, pc, JsOpcode.StoreEvalVariable)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.StoreEvalVariable);
    /// <summary>The entry point for <see cref="JsOpcode.DisposeScope"/> (0xA0): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A60F4D
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DisposeScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeScope(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DisposeScope);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeAdd"/> (0xA1): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=3411DD
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DisposeAdd at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeAdd(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DisposeAdd);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeFold"/> (0xA2): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F89672
    // Broiler-Falsified-If: this runs anything other than a block step starting at one DisposeFold at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeFold(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DisposeFold);

    /// <summary>The entry point for <see cref="JsOpcode.ToNumeric"/> (0xB0): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=EC258B
    // Broiler-Falsified-If: this runs anything other than a block step starting at one ToNumeric at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ToNumeric(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.ToNumeric);

    /// <summary>The entry point for <see cref="JsOpcode.Increment"/> (0xB1): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=12C42C
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Increment at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Increment(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Increment);

    /// <summary>The entry point for <see cref="JsOpcode.Decrement"/> (0xB2): a block step starting at one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=57F69F
    // Broiler-Falsified-If: this runs anything other than a block step starting at one Decrement at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Decrement(JsBaselineFrame* frame, int pc) =>
        JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Decrement);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeStep"/> (0xA3), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=098985
    // Broiler-Falsified-If: this runs any instruction other than one DisposeStep at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeStep(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDisposeStep>(frame, pc, JsOpcode.DisposeStep)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DisposeStep);

    /// <summary>The entry point for <see cref="JsOpcode.DisposeEnd"/> (0xA4), which runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2C6414
    // Broiler-Falsified-If: this runs any instruction other than one DisposeEnd at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DisposeEnd(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDisposeEnd>(frame, pc, JsOpcode.DisposeEnd)
            : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.DisposeEnd);

    /// <summary>The step that runs one <see cref="JsOpcode.Call"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=821976
    // Broiler-Falsified-If: this step names an opcode other than Call
    // Broiler-Human:        PENDING
    internal readonly struct StepCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Call"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AEA809
        // Broiler-Falsified-If: this answers any opcode other than Call
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Call;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Construct"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=D2D494
    // Broiler-Falsified-If: this step names an opcode other than Construct
    // Broiler-Human:        PENDING
    internal readonly struct StepConstruct : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Construct"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=AB9410
        // Broiler-Falsified-If: this answers any opcode other than Construct
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Construct;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.CallEval"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=5E042F
    // Broiler-Falsified-If: this step names an opcode other than CallEval
    // Broiler-Human:        PENDING
    internal readonly struct StepCallEval : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallEval"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=D5D4C4
        // Broiler-Falsified-If: this answers any opcode other than CallEval
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CallEval;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SuperCall"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=D2FC1C
    // Broiler-Falsified-If: this step names an opcode other than SuperCall
    // Broiler-Human:        PENDING
    internal readonly struct StepSuperCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCall"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=716B4E
        // Broiler-Falsified-If: this answers any opcode other than SuperCall
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCall;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SuperCallForwarded"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=82DB6C
    // Broiler-Falsified-If: this step names an opcode other than SuperCallForwarded
    // Broiler-Human:        PENDING
    internal readonly struct StepSuperCallForwarded : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCallForwarded"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=6D1190
        // Broiler-Falsified-If: this answers any opcode other than SuperCallForwarded
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCallForwarded;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SpreadArray"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=0307E1
    // Broiler-Falsified-If: this step names an opcode other than SpreadArray
    // Broiler-Human:        PENDING
    internal readonly struct StepSpreadArray : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SpreadArray"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=10C694
        // Broiler-Falsified-If: this answers any opcode other than SpreadArray
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SpreadArray;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.CallSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=A5D0B7
    // Broiler-Falsified-If: this step names an opcode other than CallSpread
    // Broiler-Human:        PENDING
    internal readonly struct StepCallSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=B10BF3
        // Broiler-Falsified-If: this answers any opcode other than CallSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CallSpread;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ConstructSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=41B0BC
    // Broiler-Falsified-If: this step names an opcode other than ConstructSpread
    // Broiler-Human:        PENDING
    internal readonly struct StepConstructSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ConstructSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=A344AA
        // Broiler-Falsified-If: this answers any opcode other than ConstructSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ConstructSpread;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SuperCallSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=CBE67D
    // Broiler-Falsified-If: this step names an opcode other than SuperCallSpread
    // Broiler-Human:        PENDING
    internal readonly struct StepSuperCallSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCallSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=935410
        // Broiler-Falsified-If: this answers any opcode other than SuperCallSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCallSpread;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateStart"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=74E822
    // Broiler-Falsified-If: this step names an opcode other than IterateStart
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateStart : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateStart"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=0C1F4F
        // Broiler-Falsified-If: this answers any opcode other than IterateStart
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateStart;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateNext"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=02BE85
    // Broiler-Falsified-If: this step names an opcode other than IterateNext
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateNext : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateNext"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E8268A
        // Broiler-Falsified-If: this answers any opcode other than IterateNext
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateNext;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateRest"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=28105F
    // Broiler-Falsified-If: this step names an opcode other than IterateRest
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateRest : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateRest"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=D63D37
        // Broiler-Falsified-If: this answers any opcode other than IterateRest
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateRest;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateClose"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=167664
    // Broiler-Falsified-If: this step names an opcode other than IterateClose
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateClose : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateClose"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=BF8374
        // Broiler-Falsified-If: this answers any opcode other than IterateClose
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateClose;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.YieldDelegate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=96D383
    // Broiler-Falsified-If: this step names an opcode other than YieldDelegate
    // Broiler-Human:        PENDING
    internal readonly struct StepYieldDelegate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.YieldDelegate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=BB6777
        // Broiler-Falsified-If: this answers any opcode other than YieldDelegate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.YieldDelegate;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.RunStaticElements"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=4AD870
    // Broiler-Falsified-If: this step names an opcode other than RunStaticElements
    // Broiler-Human:        PENDING
    internal readonly struct StepRunStaticElements : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.RunStaticElements"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=276AB0
        // Broiler-Falsified-If: this answers any opcode other than RunStaticElements
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.RunStaticElements;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateStartAsync"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=B0B5E3
    // Broiler-Falsified-If: this step names an opcode other than IterateStartAsync
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateStartAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateStartAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=4D20AC
        // Broiler-Falsified-If: this answers any opcode other than IterateStartAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateStartAsync;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateNextAsync"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=319122
    // Broiler-Falsified-If: this step names an opcode other than IterateNextAsync
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateNextAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateNextAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=021DE0
        // Broiler-Falsified-If: this answers any opcode other than IterateNextAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateNextAsync;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateCloseAsync"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=664AB9
    // Broiler-Falsified-If: this step names an opcode other than IterateCloseAsync
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateCloseAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateCloseAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=3D3E70
        // Broiler-Falsified-If: this answers any opcode other than IterateCloseAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateCloseAsync;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ImportCall"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=ECF250
    // Broiler-Falsified-If: this step names an opcode other than ImportCall
    // Broiler-Human:        PENDING
    internal readonly struct StepImportCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ImportCall"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=33B0A7
        // Broiler-Falsified-If: this answers any opcode other than ImportCall
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ImportCall;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.CallEvalSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=DAB9CA
    // Broiler-Falsified-If: this step names an opcode other than CallEvalSpread
    // Broiler-Human:        PENDING
    internal readonly struct StepCallEvalSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallEvalSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=1FA07A
        // Broiler-Falsified-If: this answers any opcode other than CallEvalSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CallEvalSpread;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadEvalName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=556454
    // Broiler-Falsified-If: this step names an opcode other than LoadEvalName
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadEvalName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadEvalName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=98D74C
        // Broiler-Falsified-If: this answers any opcode other than LoadEvalName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadEvalName;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadEvalNameOrUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E42664
    // Broiler-Falsified-If: this step names an opcode other than LoadEvalNameOrUndefined
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadEvalNameOrUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadEvalNameOrUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=407A2F
        // Broiler-Falsified-If: this answers any opcode other than LoadEvalNameOrUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadEvalNameOrUndefined;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StoreEvalName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E03E43
    // Broiler-Falsified-If: this step names an opcode other than StoreEvalName
    // Broiler-Human:        PENDING
    internal readonly struct StepStoreEvalName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreEvalName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=8F0D6F
        // Broiler-Falsified-If: this answers any opcode other than StoreEvalName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreEvalName;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadEvalNameWithBase"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=7DD129
    // Broiler-Falsified-If: this step names an opcode other than LoadEvalNameWithBase
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadEvalNameWithBase : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadEvalNameWithBase"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=697B36
        // Broiler-Falsified-If: this answers any opcode other than LoadEvalNameWithBase
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadEvalNameWithBase;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DeleteEvalName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=21A508
    // Broiler-Falsified-If: this step names an opcode other than DeleteEvalName
    // Broiler-Human:        PENDING
    internal readonly struct StepDeleteEvalName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteEvalName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=1EF902
        // Broiler-Falsified-If: this answers any opcode other than DeleteEvalName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteEvalName;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StoreEvalVariable"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=ADB718
    // Broiler-Falsified-If: this step names an opcode other than StoreEvalVariable
    // Broiler-Human:        PENDING
    internal readonly struct StepStoreEvalVariable : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreEvalVariable"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=CF3F82
        // Broiler-Falsified-If: this answers any opcode other than StoreEvalVariable
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreEvalVariable;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DisposeStep"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=CB0B02
    // Broiler-Falsified-If: this step names an opcode other than DisposeStep
    // Broiler-Human:        PENDING
    internal readonly struct StepDisposeStep : IJsExecutionMode
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

    /// <summary>The step that runs one <see cref="JsOpcode.DisposeEnd"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=FAC970
    // Broiler-Falsified-If: this step names an opcode other than DisposeEnd
    // Broiler-Human:        PENDING
    internal readonly struct StepDisposeEnd : IJsExecutionMode
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
}
