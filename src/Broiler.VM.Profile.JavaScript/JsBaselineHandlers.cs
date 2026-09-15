// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The baseline form's handler table: one entry point per opcode byte, each running exactly one
/// instruction of the dispatch loop.
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
/// <b>Each entry point is a per-opcode instantiation of the interpreter's own loop.</b> The step type
/// names the opcode as a constant, so the compiled step keeps one arm of the switch; setting
/// <see cref="PerOpcodeSteps"/> to false routes every entry point through the single instantiation
/// that reads its opcode from the code, with the same checks and the same semantics.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=TBF
// Broiler-Falsified-If: an emitted call through this table reaches code other than one step of the dispatch loop or the refusing entry point
// Broiler-Human:        PENDING
internal static unsafe class JsBaselineHandlers
{
    /// <summary>Whether each entry point runs its own per-opcode step or the shared one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: the two settings produce a different JavaScript answer for any program
    // Broiler-Human:        PENDING
    internal const bool PerOpcodeSteps = true;

    /// <summary>The base address of the handler table, or zero if the table failed its check.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this is non-zero while a defined opcode's slot holds anything but that opcode's own entry point, or an undefined byte's slot holds anything but the refusing one
    // Broiler-Human:        PENDING
    internal static readonly nint Table;

    /// <summary>Fills the table, checks it, and publishes it.</summary>
    /// <remarks>
    /// <b>A static constructor and not a module initializer</b>, so the table is built the first time
    /// a baseline instance asks for it and never in a process that runs only bytecode.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=2; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this reads or writes any state, or answers anything but the defect status
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Undefined(JsBaselineFrame* frame, int pc) => (int)JsBaselineStatus.Defect;

    /// <summary>The entry point for <see cref="JsOpcode.Nop"/> (0x00).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Nop at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Nop(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNop>(frame, pc, JsOpcode.Nop)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Nop);

    /// <summary>The entry point for <see cref="JsOpcode.LoadUndefined"/> (0x01).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadUndefined at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadUndefined(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadUndefined>(frame, pc, JsOpcode.LoadUndefined)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.LoadNull"/> (0x02).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadNull at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadNull(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadNull>(frame, pc, JsOpcode.LoadNull)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadNull);

    /// <summary>The entry point for <see cref="JsOpcode.LoadTrue"/> (0x03).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadTrue at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadTrue(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadTrue>(frame, pc, JsOpcode.LoadTrue)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadTrue);

    /// <summary>The entry point for <see cref="JsOpcode.LoadFalse"/> (0x04).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadFalse at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadFalse(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadFalse>(frame, pc, JsOpcode.LoadFalse)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadFalse);

    /// <summary>The entry point for <see cref="JsOpcode.LoadConstant"/> (0x05).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadConstant at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadConstant(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadConstant>(frame, pc, JsOpcode.LoadConstant)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadConstant);

    /// <summary>The entry point for <see cref="JsOpcode.LoadThis"/> (0x06).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadThis at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadThis(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadThis>(frame, pc, JsOpcode.LoadThis)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadThis);

    /// <summary>The entry point for <see cref="JsOpcode.NewArguments"/> (0x07).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one NewArguments at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewArguments(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNewArguments>(frame, pc, JsOpcode.NewArguments)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.NewArguments);

    /// <summary>The entry point for <see cref="JsOpcode.LoadNewTarget"/> (0x08).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadNewTarget at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadNewTarget(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadNewTarget>(frame, pc, JsOpcode.LoadNewTarget)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadNewTarget);

    /// <summary>The entry point for <see cref="JsOpcode.LoadArgument"/> (0x09).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadArgument at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadArgument(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadArgument>(frame, pc, JsOpcode.LoadArgument)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadArgument);

    /// <summary>The entry point for <see cref="JsOpcode.RestArguments"/> (0x0A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one RestArguments at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RestArguments(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepRestArguments>(frame, pc, JsOpcode.RestArguments)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.RestArguments);

    /// <summary>The entry point for <see cref="JsOpcode.LoadScoped"/> (0x10).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadScoped at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadScoped(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadScoped>(frame, pc, JsOpcode.LoadScoped)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadScoped);

    /// <summary>The entry point for <see cref="JsOpcode.StoreScoped"/> (0x11).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one StoreScoped at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreScoped(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStoreScoped>(frame, pc, JsOpcode.StoreScoped)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.StoreScoped);

    /// <summary>The entry point for <see cref="JsOpcode.InitialiseScoped"/> (0x12).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one InitialiseScoped at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InitialiseScoped(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepInitialiseScoped>(frame, pc, JsOpcode.InitialiseScoped)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.InitialiseScoped);

    /// <summary>The entry point for <see cref="JsOpcode.LoadGlobal"/> (0x13).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadGlobal at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadGlobal(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadGlobal>(frame, pc, JsOpcode.LoadGlobal)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.StoreGlobal"/> (0x14).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one StoreGlobal at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreGlobal(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStoreGlobal>(frame, pc, JsOpcode.StoreGlobal)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.StoreGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.LoadGlobalOrUndefined"/> (0x15).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadGlobalOrUndefined at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadGlobalOrUndefined(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadGlobalOrUndefined>(frame, pc, JsOpcode.LoadGlobalOrUndefined)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadGlobalOrUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.PushScope"/> (0x16).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one PushScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PushScope(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepPushScope>(frame, pc, JsOpcode.PushScope)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.PushScope);

    /// <summary>The entry point for <see cref="JsOpcode.PopScope"/> (0x17).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one PopScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PopScope(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepPopScope>(frame, pc, JsOpcode.PopScope)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.PopScope);

    /// <summary>The entry point for <see cref="JsOpcode.CopyScope"/> (0x18).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one CopyScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CopyScope(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCopyScope>(frame, pc, JsOpcode.CopyScope)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.CopyScope);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobal"/> (0x19).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DeclareGlobal at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobal(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDeclareGlobal>(frame, pc, JsOpcode.DeclareGlobal)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DeclareGlobal);

    /// <summary>The entry point for <see cref="JsOpcode.PushObjectScope"/> (0x1A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one PushObjectScope at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int PushObjectScope(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepPushObjectScope>(frame, pc, JsOpcode.PushObjectScope)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.PushObjectScope);

    /// <summary>The entry point for <see cref="JsOpcode.ResolveName"/> (0x1B).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ResolveName at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ResolveName(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepResolveName>(frame, pc, JsOpcode.ResolveName)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ResolveName);

    /// <summary>The entry point for <see cref="JsOpcode.NewObject"/> (0x20).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one NewObject at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewObject(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNewObject>(frame, pc, JsOpcode.NewObject)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.NewObject);

    /// <summary>The entry point for <see cref="JsOpcode.NewArray"/> (0x21).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one NewArray at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewArray(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNewArray>(frame, pc, JsOpcode.NewArray)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.NewArray);

    /// <summary>The entry point for <see cref="JsOpcode.GetProperty"/> (0x22).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one GetProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetProperty(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepGetProperty>(frame, pc, JsOpcode.GetProperty)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.GetProperty);

    /// <summary>The entry point for <see cref="JsOpcode.SetProperty"/> (0x23).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SetProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetProperty(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSetProperty>(frame, pc, JsOpcode.SetProperty)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SetProperty);

    /// <summary>The entry point for <see cref="JsOpcode.GetIndex"/> (0x24).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one GetIndex at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GetIndex(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepGetIndex>(frame, pc, JsOpcode.GetIndex)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.GetIndex);

    /// <summary>The entry point for <see cref="JsOpcode.SetIndex"/> (0x25).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SetIndex at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetIndex(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSetIndex>(frame, pc, JsOpcode.SetIndex)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SetIndex);

    /// <summary>The entry point for <see cref="JsOpcode.DefineField"/> (0x26).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DefineField at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineField(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDefineField>(frame, pc, JsOpcode.DefineField)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DefineField);

    /// <summary>The entry point for <see cref="JsOpcode.DefineIndexed"/> (0x27).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DefineIndexed at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineIndexed(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDefineIndexed>(frame, pc, JsOpcode.DefineIndexed)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DefineIndexed);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteProperty"/> (0x28).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DeleteProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteProperty(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDeleteProperty>(frame, pc, JsOpcode.DeleteProperty)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DeleteProperty);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteIndex"/> (0x29).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DeleteIndex at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteIndex(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDeleteIndex>(frame, pc, JsOpcode.DeleteIndex)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DeleteIndex);

    /// <summary>The entry point for <see cref="JsOpcode.DefineGetter"/> (0x2A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DefineGetter at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineGetter(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDefineGetter>(frame, pc, JsOpcode.DefineGetter)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DefineGetter);

    /// <summary>The entry point for <see cref="JsOpcode.DefineSetter"/> (0x2B).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DefineSetter at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineSetter(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDefineSetter>(frame, pc, JsOpcode.DefineSetter)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DefineSetter);

    /// <summary>The entry point for <see cref="JsOpcode.DefineMethod"/> (0x2C).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DefineMethod at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineMethod(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDefineMethod>(frame, pc, JsOpcode.DefineMethod)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DefineMethod);

    /// <summary>The entry point for <see cref="JsOpcode.LoadSuperProperty"/> (0x2D).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadSuperProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadSuperProperty(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadSuperProperty>(frame, pc, JsOpcode.LoadSuperProperty)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadSuperProperty);

    /// <summary>The entry point for <see cref="JsOpcode.StoreSuperProperty"/> (0x2E).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one StoreSuperProperty at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StoreSuperProperty(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStoreSuperProperty>(frame, pc, JsOpcode.StoreSuperProperty)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.StoreSuperProperty);

    /// <summary>The entry point for <see cref="JsOpcode.ArrayAppend"/> (0x2F).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ArrayAppend at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ArrayAppend(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepArrayAppend>(frame, pc, JsOpcode.ArrayAppend)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ArrayAppend);

    /// <summary>The entry point for <see cref="JsOpcode.Closure"/> (0x30).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Closure at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Closure(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepClosure>(frame, pc, JsOpcode.Closure)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Closure);

    /// <summary>The entry point for <see cref="JsOpcode.Call"/> (0x31).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Call at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Call(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCall>(frame, pc, JsOpcode.Call)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Call);

    /// <summary>The entry point for <see cref="JsOpcode.Construct"/> (0x32).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Construct at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Construct(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepConstruct>(frame, pc, JsOpcode.Construct)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Construct);

    /// <summary>The entry point for <see cref="JsOpcode.Return"/> (0x33).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Return at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Return(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepReturn>(frame, pc, JsOpcode.Return)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Return);

    /// <summary>The entry point for <see cref="JsOpcode.ReturnUndefined"/> (0x34).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ReturnUndefined at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ReturnUndefined(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepReturnUndefined>(frame, pc, JsOpcode.ReturnUndefined)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ReturnUndefined);

    /// <summary>The entry point for <see cref="JsOpcode.CallEval"/> (0x35).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one CallEval at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallEval(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCallEval>(frame, pc, JsOpcode.CallEval)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.CallEval);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCall"/> (0x36).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SuperCall at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCall(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSuperCall>(frame, pc, JsOpcode.SuperCall)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SuperCall);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCallForwarded"/> (0x37).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SuperCallForwarded at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCallForwarded(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSuperCallForwarded>(frame, pc, JsOpcode.SuperCallForwarded)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SuperCallForwarded);

    /// <summary>The entry point for <see cref="JsOpcode.NewClass"/> (0x38).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one NewClass at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewClass(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNewClass>(frame, pc, JsOpcode.NewClass)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.NewClass);

    /// <summary>The entry point for <see cref="JsOpcode.ArrayHoles"/> (0x39).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ArrayHoles at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ArrayHoles(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepArrayHoles>(frame, pc, JsOpcode.ArrayHoles)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ArrayHoles);

    /// <summary>The entry point for <see cref="JsOpcode.SpreadArray"/> (0x3A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SpreadArray at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SpreadArray(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSpreadArray>(frame, pc, JsOpcode.SpreadArray)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SpreadArray);

    /// <summary>The entry point for <see cref="JsOpcode.SpreadObject"/> (0x3B).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SpreadObject at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SpreadObject(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSpreadObject>(frame, pc, JsOpcode.SpreadObject)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SpreadObject);

    /// <summary>The entry point for <see cref="JsOpcode.CallSpread"/> (0x3C).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one CallSpread at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int CallSpread(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepCallSpread>(frame, pc, JsOpcode.CallSpread)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.CallSpread);

    /// <summary>The entry point for <see cref="JsOpcode.ConstructSpread"/> (0x3D).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ConstructSpread at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ConstructSpread(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepConstructSpread>(frame, pc, JsOpcode.ConstructSpread)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ConstructSpread);

    /// <summary>The entry point for <see cref="JsOpcode.SuperCallSpread"/> (0x3E).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SuperCallSpread at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SuperCallSpread(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSuperCallSpread>(frame, pc, JsOpcode.SuperCallSpread)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SuperCallSpread);

    /// <summary>The entry point for <see cref="JsOpcode.SetPrototypeLiteral"/> (0x3F).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one SetPrototypeLiteral at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int SetPrototypeLiteral(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSetPrototypeLiteral>(frame, pc, JsOpcode.SetPrototypeLiteral)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.SetPrototypeLiteral);

    /// <summary>The entry point for <see cref="JsOpcode.Add"/> (0x40).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Add at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Add(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepAdd>(frame, pc, JsOpcode.Add)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Add);

    /// <summary>The entry point for <see cref="JsOpcode.Subtract"/> (0x41).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Subtract at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Subtract(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSubtract>(frame, pc, JsOpcode.Subtract)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Subtract);

    /// <summary>The entry point for <see cref="JsOpcode.Multiply"/> (0x42).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Multiply at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Multiply(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepMultiply>(frame, pc, JsOpcode.Multiply)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Multiply);

    /// <summary>The entry point for <see cref="JsOpcode.Divide"/> (0x43).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Divide at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Divide(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDivide>(frame, pc, JsOpcode.Divide)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Divide);

    /// <summary>The entry point for <see cref="JsOpcode.Remainder"/> (0x44).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Remainder at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Remainder(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepRemainder>(frame, pc, JsOpcode.Remainder)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Remainder);

    /// <summary>The entry point for <see cref="JsOpcode.Exponent"/> (0x45).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Exponent at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Exponent(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepExponent>(frame, pc, JsOpcode.Exponent)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Exponent);

    /// <summary>The entry point for <see cref="JsOpcode.Negate"/> (0x46).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Negate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Negate(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNegate>(frame, pc, JsOpcode.Negate)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Negate);

    /// <summary>The entry point for <see cref="JsOpcode.ToNumber"/> (0x47).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ToNumber at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ToNumber(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepToNumber>(frame, pc, JsOpcode.ToNumber)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ToNumber);

    /// <summary>The entry point for <see cref="JsOpcode.Not"/> (0x48).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Not at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Not(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNot>(frame, pc, JsOpcode.Not)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Not);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseNot"/> (0x49).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one BitwiseNot at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseNot(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepBitwiseNot>(frame, pc, JsOpcode.BitwiseNot)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.BitwiseNot);

    /// <summary>The entry point for <see cref="JsOpcode.LessThan"/> (0x4A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LessThan at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LessThan(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLessThan>(frame, pc, JsOpcode.LessThan)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LessThan);

    /// <summary>The entry point for <see cref="JsOpcode.LessThanOrEqual"/> (0x4B).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LessThanOrEqual at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LessThanOrEqual(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLessThanOrEqual>(frame, pc, JsOpcode.LessThanOrEqual)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LessThanOrEqual);

    /// <summary>The entry point for <see cref="JsOpcode.GreaterThan"/> (0x4C).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one GreaterThan at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GreaterThan(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepGreaterThan>(frame, pc, JsOpcode.GreaterThan)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.GreaterThan);

    /// <summary>The entry point for <see cref="JsOpcode.GreaterThanOrEqual"/> (0x4D).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one GreaterThanOrEqual at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int GreaterThanOrEqual(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepGreaterThanOrEqual>(frame, pc, JsOpcode.GreaterThanOrEqual)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.GreaterThanOrEqual);

    /// <summary>The entry point for <see cref="JsOpcode.StrictEquals"/> (0x4E).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one StrictEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StrictEquals(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStrictEquals>(frame, pc, JsOpcode.StrictEquals)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.StrictEquals);

    /// <summary>The entry point for <see cref="JsOpcode.StrictNotEquals"/> (0x4F).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one StrictNotEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StrictNotEquals(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStrictNotEquals>(frame, pc, JsOpcode.StrictNotEquals)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.StrictNotEquals);

    /// <summary>The entry point for <see cref="JsOpcode.LooseEquals"/> (0x50).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LooseEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LooseEquals(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLooseEquals>(frame, pc, JsOpcode.LooseEquals)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LooseEquals);

    /// <summary>The entry point for <see cref="JsOpcode.LooseNotEquals"/> (0x51).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LooseNotEquals at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LooseNotEquals(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLooseNotEquals>(frame, pc, JsOpcode.LooseNotEquals)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LooseNotEquals);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseOr"/> (0x52).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one BitwiseOr at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseOr(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepBitwiseOr>(frame, pc, JsOpcode.BitwiseOr)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.BitwiseOr);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseAnd"/> (0x53).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one BitwiseAnd at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseAnd(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepBitwiseAnd>(frame, pc, JsOpcode.BitwiseAnd)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.BitwiseAnd);

    /// <summary>The entry point for <see cref="JsOpcode.BitwiseXor"/> (0x54).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one BitwiseXor at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int BitwiseXor(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepBitwiseXor>(frame, pc, JsOpcode.BitwiseXor)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.BitwiseXor);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftLeft"/> (0x55).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ShiftLeft at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftLeft(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepShiftLeft>(frame, pc, JsOpcode.ShiftLeft)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ShiftLeft);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftRight"/> (0x56).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ShiftRight at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftRight(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepShiftRight>(frame, pc, JsOpcode.ShiftRight)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ShiftRight);

    /// <summary>The entry point for <see cref="JsOpcode.ShiftRightUnsigned"/> (0x57).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ShiftRightUnsigned at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ShiftRightUnsigned(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepShiftRightUnsigned>(frame, pc, JsOpcode.ShiftRightUnsigned)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ShiftRightUnsigned);

    /// <summary>The entry point for <see cref="JsOpcode.TypeOf"/> (0x58).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one TypeOf at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int TypeOf(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepTypeOf>(frame, pc, JsOpcode.TypeOf)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.TypeOf);

    /// <summary>The entry point for <see cref="JsOpcode.InstanceOf"/> (0x59).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one InstanceOf at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InstanceOf(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepInstanceOf>(frame, pc, JsOpcode.InstanceOf)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.InstanceOf);

    /// <summary>The entry point for <see cref="JsOpcode.In"/> (0x5A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one In at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int In(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIn>(frame, pc, JsOpcode.In)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.In);

    /// <summary>The entry point for <see cref="JsOpcode.Void"/> (0x5B).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Void at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Void(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepVoid>(frame, pc, JsOpcode.Void)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Void);

    /// <summary>The entry point for <see cref="JsOpcode.RequireCoercible"/> (0x5C).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one RequireCoercible at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RequireCoercible(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepRequireCoercible>(frame, pc, JsOpcode.RequireCoercible)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.RequireCoercible);

    /// <summary>The entry point for <see cref="JsOpcode.Jump"/> (0x60).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Jump at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Jump(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepJump>(frame, pc, JsOpcode.Jump)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Jump);

    /// <summary>The entry point for <see cref="JsOpcode.JumpIfFalse"/> (0x61).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one JumpIfFalse at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int JumpIfFalse(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepJumpIfFalse>(frame, pc, JsOpcode.JumpIfFalse)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.JumpIfFalse);

    /// <summary>The entry point for <see cref="JsOpcode.JumpIfTrue"/> (0x62).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one JumpIfTrue at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int JumpIfTrue(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepJumpIfTrue>(frame, pc, JsOpcode.JumpIfTrue)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.JumpIfTrue);

    /// <summary>The entry point for <see cref="JsOpcode.Throw"/> (0x63).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Throw at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Throw(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepThrow>(frame, pc, JsOpcode.Throw)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Throw);

    /// <summary>The entry point for <see cref="JsOpcode.ForInStart"/> (0x64).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ForInStart at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ForInStart(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepForInStart>(frame, pc, JsOpcode.ForInStart)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ForInStart);

    /// <summary>The entry point for <see cref="JsOpcode.ForInNext"/> (0x65).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ForInNext at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ForInNext(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepForInNext>(frame, pc, JsOpcode.ForInNext)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ForInNext);

    /// <summary>The entry point for <see cref="JsOpcode.IterateStart"/> (0x66).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateStart at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateStart(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateStart>(frame, pc, JsOpcode.IterateStart)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateStart);

    /// <summary>The entry point for <see cref="JsOpcode.IterateNext"/> (0x67).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateNext at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateNext(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateNext>(frame, pc, JsOpcode.IterateNext)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateNext);

    /// <summary>The entry point for <see cref="JsOpcode.IterateRest"/> (0x68).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateRest at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateRest(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateRest>(frame, pc, JsOpcode.IterateRest)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateRest);

    /// <summary>The entry point for <see cref="JsOpcode.IterateClose"/> (0x69).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateClose at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateClose(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateClose>(frame, pc, JsOpcode.IterateClose)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateClose);

    /// <summary>The entry point for <see cref="JsOpcode.Yield"/> (0x6A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Yield at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Yield(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepYield>(frame, pc, JsOpcode.Yield)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Yield);

    /// <summary>The entry point for <see cref="JsOpcode.YieldDelegate"/> (0x6B).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one YieldDelegate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int YieldDelegate(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepYieldDelegate>(frame, pc, JsOpcode.YieldDelegate)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.YieldDelegate);

    /// <summary>The entry point for <see cref="JsOpcode.Await"/> (0x6C).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Await at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Await(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepAwait>(frame, pc, JsOpcode.Await)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Await);

    /// <summary>The entry point for <see cref="JsOpcode.LoadImport"/> (0x6D).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadImport at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadImport(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadImport>(frame, pc, JsOpcode.LoadImport)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadImport);

    /// <summary>The entry point for <see cref="JsOpcode.ThrowImmutable"/> (0x6E).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ThrowImmutable at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ThrowImmutable(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepThrowImmutable>(frame, pc, JsOpcode.ThrowImmutable)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ThrowImmutable);

    /// <summary>The entry point for <see cref="JsOpcode.DefineClassElement"/> (0x6F).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DefineClassElement at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DefineClassElement(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDefineClassElement>(frame, pc, JsOpcode.DefineClassElement)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DefineClassElement);

    /// <summary>The entry point for <see cref="JsOpcode.Pop"/> (0x70).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Pop at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Pop(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepPop>(frame, pc, JsOpcode.Pop)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Pop);

    /// <summary>The entry point for <see cref="JsOpcode.Duplicate"/> (0x71).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Duplicate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Duplicate(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDuplicate>(frame, pc, JsOpcode.Duplicate)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Duplicate);

    /// <summary>The entry point for <see cref="JsOpcode.DuplicateTwo"/> (0x72).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DuplicateTwo at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DuplicateTwo(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDuplicateTwo>(frame, pc, JsOpcode.DuplicateTwo)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DuplicateTwo);

    /// <summary>The entry point for <see cref="JsOpcode.Swap"/> (0x73).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Swap at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Swap(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepSwap>(frame, pc, JsOpcode.Swap)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Swap);

    /// <summary>The entry point for <see cref="JsOpcode.Pick"/> (0x74).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one Pick at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int Pick(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepPick>(frame, pc, JsOpcode.Pick)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.Pick);

    /// <summary>The entry point for <see cref="JsOpcode.NewPrivateName"/> (0x75).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one NewPrivateName at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int NewPrivateName(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepNewPrivateName>(frame, pc, JsOpcode.NewPrivateName)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.NewPrivateName);

    /// <summary>The entry point for <see cref="JsOpcode.LoadPrivate"/> (0x76).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one LoadPrivate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int LoadPrivate(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepLoadPrivate>(frame, pc, JsOpcode.LoadPrivate)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.LoadPrivate);

    /// <summary>The entry point for <see cref="JsOpcode.StorePrivate"/> (0x77).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one StorePrivate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int StorePrivate(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepStorePrivate>(frame, pc, JsOpcode.StorePrivate)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.StorePrivate);

    /// <summary>The entry point for <see cref="JsOpcode.HasPrivate"/> (0x78).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one HasPrivate at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int HasPrivate(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepHasPrivate>(frame, pc, JsOpcode.HasPrivate)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.HasPrivate);

    /// <summary>The entry point for <see cref="JsOpcode.RunStaticElements"/> (0x79).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one RunStaticElements at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int RunStaticElements(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepRunStaticElements>(frame, pc, JsOpcode.RunStaticElements)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.RunStaticElements);

    /// <summary>The entry point for <see cref="JsOpcode.IterateStartAsync"/> (0x7A).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateStartAsync at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateStartAsync(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateStartAsync>(frame, pc, JsOpcode.IterateStartAsync)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateStartAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateNextAsync"/> (0x7B).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateNextAsync at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateNextAsync(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateNextAsync>(frame, pc, JsOpcode.IterateNextAsync)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateNextAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateAwaitStep"/> (0x7C).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateAwaitStep at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateAwaitStep(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateAwaitStep>(frame, pc, JsOpcode.IterateAwaitStep)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateAwaitStep);

    /// <summary>The entry point for <see cref="JsOpcode.IterateCloseAsync"/> (0x7D).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateCloseAsync at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateCloseAsync(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateCloseAsync>(frame, pc, JsOpcode.IterateCloseAsync)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateCloseAsync);

    /// <summary>The entry point for <see cref="JsOpcode.IterateCloseCheck"/> (0x7E).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one IterateCloseCheck at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int IterateCloseCheck(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepIterateCloseCheck>(frame, pc, JsOpcode.IterateCloseCheck)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.IterateCloseCheck);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobalLet"/> (0x7F).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DeclareGlobalLet at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobalLet(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDeclareGlobalLet>(frame, pc, JsOpcode.DeclareGlobalLet)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DeclareGlobalLet);

    /// <summary>The entry point for <see cref="JsOpcode.DeclareGlobalConst"/> (0x80).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DeclareGlobalConst at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeclareGlobalConst(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDeclareGlobalConst>(frame, pc, JsOpcode.DeclareGlobalConst)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DeclareGlobalConst);

    /// <summary>The entry point for <see cref="JsOpcode.InitialiseGlobalLexical"/> (0x81).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one InitialiseGlobalLexical at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int InitialiseGlobalLexical(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepInitialiseGlobalLexical>(frame, pc, JsOpcode.InitialiseGlobalLexical)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.InitialiseGlobalLexical);

    /// <summary>The entry point for <see cref="JsOpcode.DeleteGlobalBinding"/> (0x82).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one DeleteGlobalBinding at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int DeleteGlobalBinding(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepDeleteGlobalBinding>(frame, pc, JsOpcode.DeleteGlobalBinding)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.DeleteGlobalBinding);

    /// <summary>The entry point for <see cref="JsOpcode.EnterBody"/> (0x83).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one EnterBody at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int EnterBody(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepEnterBody>(frame, pc, JsOpcode.EnterBody)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.EnterBody);

    /// <summary>The entry point for <see cref="JsOpcode.ImportCall"/> (0x85).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ImportCall at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ImportCall(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepImportCall>(frame, pc, JsOpcode.ImportCall)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ImportCall);

    /// <summary>The entry point for <see cref="JsOpcode.ImportMeta"/> (0x86).</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=TBF
    // Broiler-Falsified-If: this runs any instruction other than one ImportMeta at the offset the managed side expects
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.UnmanagedCallersOnly]
    private static int ImportMeta(JsBaselineFrame* frame, int pc) =>
        PerOpcodeSteps
            ? JsNativeActivation.Step<StepImportMeta>(frame, pc, JsOpcode.ImportMeta)
            : JsNativeActivation.Step<JsStepAny>(frame, pc, JsOpcode.ImportMeta);

    /// <summary>The step that runs one <see cref="JsOpcode.Nop"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Nop
    // Broiler-Human:        PENDING
    internal readonly struct StepNop : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Nop"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Nop
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Nop;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadUndefined
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadUndefined;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadNull"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadNull
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadNull : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadNull"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadNull
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadNull;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadTrue"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadTrue
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadTrue : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadTrue"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadTrue
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadTrue;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadFalse"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadFalse
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadFalse : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadFalse"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadFalse
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadFalse;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadConstant"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadConstant
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadConstant : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadConstant"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadConstant
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadConstant;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadThis"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadThis
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadThis : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadThis"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadThis
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadThis;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.NewArguments"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than NewArguments
    // Broiler-Human:        PENDING
    internal readonly struct StepNewArguments : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewArguments"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than NewArguments
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewArguments;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadNewTarget"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadNewTarget
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadNewTarget : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadNewTarget"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadNewTarget
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadNewTarget;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadArgument"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadArgument
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadArgument : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadArgument"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadArgument
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadArgument;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.RestArguments"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than RestArguments
    // Broiler-Human:        PENDING
    internal readonly struct StepRestArguments : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.RestArguments"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than RestArguments
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.RestArguments;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadScoped"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadScoped
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadScoped : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadScoped"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadScoped
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadScoped;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StoreScoped"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than StoreScoped
    // Broiler-Human:        PENDING
    internal readonly struct StepStoreScoped : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreScoped"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than StoreScoped
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreScoped;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.InitialiseScoped"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than InitialiseScoped
    // Broiler-Human:        PENDING
    internal readonly struct StepInitialiseScoped : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.InitialiseScoped"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than InitialiseScoped
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.InitialiseScoped;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadGlobal"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadGlobal
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadGlobal : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadGlobal"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadGlobal
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadGlobal;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StoreGlobal"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than StoreGlobal
    // Broiler-Human:        PENDING
    internal readonly struct StepStoreGlobal : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreGlobal"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than StoreGlobal
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreGlobal;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadGlobalOrUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadGlobalOrUndefined
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadGlobalOrUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadGlobalOrUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadGlobalOrUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadGlobalOrUndefined;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.PushScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than PushScope
    // Broiler-Human:        PENDING
    internal readonly struct StepPushScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.PushScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than PushScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.PushScope;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.PopScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than PopScope
    // Broiler-Human:        PENDING
    internal readonly struct StepPopScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.PopScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than PopScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.PopScope;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.CopyScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than CopyScope
    // Broiler-Human:        PENDING
    internal readonly struct StepCopyScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CopyScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than CopyScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.CopyScope;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DeclareGlobal"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DeclareGlobal
    // Broiler-Human:        PENDING
    internal readonly struct StepDeclareGlobal : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeclareGlobal"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DeclareGlobal
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeclareGlobal;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.PushObjectScope"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than PushObjectScope
    // Broiler-Human:        PENDING
    internal readonly struct StepPushObjectScope : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.PushObjectScope"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than PushObjectScope
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.PushObjectScope;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ResolveName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ResolveName
    // Broiler-Human:        PENDING
    internal readonly struct StepResolveName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ResolveName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ResolveName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ResolveName;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.NewObject"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than NewObject
    // Broiler-Human:        PENDING
    internal readonly struct StepNewObject : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewObject"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than NewObject
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewObject;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.NewArray"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than NewArray
    // Broiler-Human:        PENDING
    internal readonly struct StepNewArray : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewArray"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than NewArray
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewArray;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.GetProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than GetProperty
    // Broiler-Human:        PENDING
    internal readonly struct StepGetProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GetProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than GetProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GetProperty;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SetProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SetProperty
    // Broiler-Human:        PENDING
    internal readonly struct StepSetProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SetProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than SetProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SetProperty;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.GetIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than GetIndex
    // Broiler-Human:        PENDING
    internal readonly struct StepGetIndex : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GetIndex"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than GetIndex
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GetIndex;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SetIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SetIndex
    // Broiler-Human:        PENDING
    internal readonly struct StepSetIndex : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SetIndex"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than SetIndex
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SetIndex;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DefineField"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DefineField
    // Broiler-Human:        PENDING
    internal readonly struct StepDefineField : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineField"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DefineField
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineField;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DefineIndexed"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DefineIndexed
    // Broiler-Human:        PENDING
    internal readonly struct StepDefineIndexed : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineIndexed"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DefineIndexed
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineIndexed;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DeleteProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DeleteProperty
    // Broiler-Human:        PENDING
    internal readonly struct StepDeleteProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DeleteProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteProperty;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DeleteIndex"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DeleteIndex
    // Broiler-Human:        PENDING
    internal readonly struct StepDeleteIndex : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteIndex"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DeleteIndex
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteIndex;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DefineGetter"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DefineGetter
    // Broiler-Human:        PENDING
    internal readonly struct StepDefineGetter : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineGetter"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DefineGetter
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineGetter;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DefineSetter"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DefineSetter
    // Broiler-Human:        PENDING
    internal readonly struct StepDefineSetter : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineSetter"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DefineSetter
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineSetter;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DefineMethod"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DefineMethod
    // Broiler-Human:        PENDING
    internal readonly struct StepDefineMethod : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineMethod"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DefineMethod
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineMethod;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadSuperProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadSuperProperty
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadSuperProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadSuperProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadSuperProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadSuperProperty;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StoreSuperProperty"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than StoreSuperProperty
    // Broiler-Human:        PENDING
    internal readonly struct StepStoreSuperProperty : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StoreSuperProperty"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than StoreSuperProperty
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StoreSuperProperty;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ArrayAppend"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ArrayAppend
    // Broiler-Human:        PENDING
    internal readonly struct StepArrayAppend : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ArrayAppend"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ArrayAppend
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ArrayAppend;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Closure"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Closure
    // Broiler-Human:        PENDING
    internal readonly struct StepClosure : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Closure"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Closure
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Closure;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Call"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Call
    // Broiler-Human:        PENDING
    internal readonly struct StepCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Call"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Construct
    // Broiler-Human:        PENDING
    internal readonly struct StepConstruct : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Construct"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Construct
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Construct;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Return"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Return
    // Broiler-Human:        PENDING
    internal readonly struct StepReturn : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Return"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Return
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Return;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ReturnUndefined"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ReturnUndefined
    // Broiler-Human:        PENDING
    internal readonly struct StepReturnUndefined : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ReturnUndefined"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ReturnUndefined
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ReturnUndefined;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.CallEval"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than CallEval
    // Broiler-Human:        PENDING
    internal readonly struct StepCallEval : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallEval"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SuperCall
    // Broiler-Human:        PENDING
    internal readonly struct StepSuperCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCall"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SuperCallForwarded
    // Broiler-Human:        PENDING
    internal readonly struct StepSuperCallForwarded : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCallForwarded"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than SuperCallForwarded
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCallForwarded;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.NewClass"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than NewClass
    // Broiler-Human:        PENDING
    internal readonly struct StepNewClass : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewClass"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than NewClass
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewClass;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ArrayHoles"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ArrayHoles
    // Broiler-Human:        PENDING
    internal readonly struct StepArrayHoles : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ArrayHoles"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ArrayHoles
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ArrayHoles;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SpreadArray"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SpreadArray
    // Broiler-Human:        PENDING
    internal readonly struct StepSpreadArray : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SpreadArray"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than SpreadArray
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SpreadArray;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SpreadObject"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SpreadObject
    // Broiler-Human:        PENDING
    internal readonly struct StepSpreadObject : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SpreadObject"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than SpreadObject
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SpreadObject;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.CallSpread"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than CallSpread
    // Broiler-Human:        PENDING
    internal readonly struct StepCallSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.CallSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ConstructSpread
    // Broiler-Human:        PENDING
    internal readonly struct StepConstructSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ConstructSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SuperCallSpread
    // Broiler-Human:        PENDING
    internal readonly struct StepSuperCallSpread : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SuperCallSpread"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than SuperCallSpread
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SuperCallSpread;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.SetPrototypeLiteral"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than SetPrototypeLiteral
    // Broiler-Human:        PENDING
    internal readonly struct StepSetPrototypeLiteral : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.SetPrototypeLiteral"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than SetPrototypeLiteral
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.SetPrototypeLiteral;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Add"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Add
    // Broiler-Human:        PENDING
    internal readonly struct StepAdd : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Add"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Add
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Add;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Subtract"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Subtract
    // Broiler-Human:        PENDING
    internal readonly struct StepSubtract : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Subtract"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Subtract
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Subtract;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Multiply"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Multiply
    // Broiler-Human:        PENDING
    internal readonly struct StepMultiply : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Multiply"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Multiply
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Multiply;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Divide"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Divide
    // Broiler-Human:        PENDING
    internal readonly struct StepDivide : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Divide"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Divide
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Divide;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Remainder"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Remainder
    // Broiler-Human:        PENDING
    internal readonly struct StepRemainder : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Remainder"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Remainder
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Remainder;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Exponent"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Exponent
    // Broiler-Human:        PENDING
    internal readonly struct StepExponent : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Exponent"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Exponent
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Exponent;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Negate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Negate
    // Broiler-Human:        PENDING
    internal readonly struct StepNegate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Negate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Negate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Negate;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ToNumber"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ToNumber
    // Broiler-Human:        PENDING
    internal readonly struct StepToNumber : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ToNumber"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ToNumber
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ToNumber;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Not"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Not
    // Broiler-Human:        PENDING
    internal readonly struct StepNot : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Not"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Not
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Not;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.BitwiseNot"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than BitwiseNot
    // Broiler-Human:        PENDING
    internal readonly struct StepBitwiseNot : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseNot"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than BitwiseNot
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseNot;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LessThan"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LessThan
    // Broiler-Human:        PENDING
    internal readonly struct StepLessThan : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LessThan"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LessThan
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LessThan;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LessThanOrEqual"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LessThanOrEqual
    // Broiler-Human:        PENDING
    internal readonly struct StepLessThanOrEqual : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LessThanOrEqual"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LessThanOrEqual
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LessThanOrEqual;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.GreaterThan"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than GreaterThan
    // Broiler-Human:        PENDING
    internal readonly struct StepGreaterThan : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GreaterThan"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than GreaterThan
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GreaterThan;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.GreaterThanOrEqual"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than GreaterThanOrEqual
    // Broiler-Human:        PENDING
    internal readonly struct StepGreaterThanOrEqual : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.GreaterThanOrEqual"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than GreaterThanOrEqual
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.GreaterThanOrEqual;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StrictEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than StrictEquals
    // Broiler-Human:        PENDING
    internal readonly struct StepStrictEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StrictEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than StrictEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StrictEquals;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StrictNotEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than StrictNotEquals
    // Broiler-Human:        PENDING
    internal readonly struct StepStrictNotEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StrictNotEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than StrictNotEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StrictNotEquals;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LooseEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LooseEquals
    // Broiler-Human:        PENDING
    internal readonly struct StepLooseEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LooseEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LooseEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LooseEquals;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LooseNotEquals"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LooseNotEquals
    // Broiler-Human:        PENDING
    internal readonly struct StepLooseNotEquals : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LooseNotEquals"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LooseNotEquals
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LooseNotEquals;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.BitwiseOr"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than BitwiseOr
    // Broiler-Human:        PENDING
    internal readonly struct StepBitwiseOr : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseOr"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than BitwiseOr
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseOr;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.BitwiseAnd"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than BitwiseAnd
    // Broiler-Human:        PENDING
    internal readonly struct StepBitwiseAnd : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseAnd"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than BitwiseAnd
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseAnd;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.BitwiseXor"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than BitwiseXor
    // Broiler-Human:        PENDING
    internal readonly struct StepBitwiseXor : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.BitwiseXor"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than BitwiseXor
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.BitwiseXor;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ShiftLeft"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ShiftLeft
    // Broiler-Human:        PENDING
    internal readonly struct StepShiftLeft : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ShiftLeft"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ShiftLeft
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ShiftLeft;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ShiftRight"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ShiftRight
    // Broiler-Human:        PENDING
    internal readonly struct StepShiftRight : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ShiftRight"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ShiftRight
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ShiftRight;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ShiftRightUnsigned"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ShiftRightUnsigned
    // Broiler-Human:        PENDING
    internal readonly struct StepShiftRightUnsigned : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ShiftRightUnsigned"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ShiftRightUnsigned
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ShiftRightUnsigned;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.TypeOf"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than TypeOf
    // Broiler-Human:        PENDING
    internal readonly struct StepTypeOf : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.TypeOf"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than TypeOf
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.TypeOf;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.InstanceOf"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than InstanceOf
    // Broiler-Human:        PENDING
    internal readonly struct StepInstanceOf : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.InstanceOf"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than InstanceOf
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.InstanceOf;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.In"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than In
    // Broiler-Human:        PENDING
    internal readonly struct StepIn : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.In"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than In
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.In;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Void"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Void
    // Broiler-Human:        PENDING
    internal readonly struct StepVoid : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Void"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Void
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Void;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.RequireCoercible"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than RequireCoercible
    // Broiler-Human:        PENDING
    internal readonly struct StepRequireCoercible : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.RequireCoercible"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than RequireCoercible
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.RequireCoercible;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Jump"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Jump
    // Broiler-Human:        PENDING
    internal readonly struct StepJump : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Jump"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Jump
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Jump;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.JumpIfFalse"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than JumpIfFalse
    // Broiler-Human:        PENDING
    internal readonly struct StepJumpIfFalse : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.JumpIfFalse"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than JumpIfFalse
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.JumpIfFalse;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.JumpIfTrue"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than JumpIfTrue
    // Broiler-Human:        PENDING
    internal readonly struct StepJumpIfTrue : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.JumpIfTrue"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than JumpIfTrue
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.JumpIfTrue;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Throw"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Throw
    // Broiler-Human:        PENDING
    internal readonly struct StepThrow : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Throw"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Throw
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Throw;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ForInStart"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ForInStart
    // Broiler-Human:        PENDING
    internal readonly struct StepForInStart : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ForInStart"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ForInStart
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ForInStart;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ForInNext"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ForInNext
    // Broiler-Human:        PENDING
    internal readonly struct StepForInNext : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ForInNext"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ForInNext
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ForInNext;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateStart"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateStart
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateStart : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateStart"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateNext
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateNext : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateNext"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateRest
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateRest : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateRest"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateClose
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateClose : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateClose"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than IterateClose
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateClose;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Yield"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Yield
    // Broiler-Human:        PENDING
    internal readonly struct StepYield : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Yield"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Yield
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Yield;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.YieldDelegate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than YieldDelegate
    // Broiler-Human:        PENDING
    internal readonly struct StepYieldDelegate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.YieldDelegate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than YieldDelegate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.YieldDelegate;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Await"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Await
    // Broiler-Human:        PENDING
    internal readonly struct StepAwait : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Await"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Await
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Await;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadImport"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadImport
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadImport : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadImport"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadImport
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadImport;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ThrowImmutable"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ThrowImmutable
    // Broiler-Human:        PENDING
    internal readonly struct StepThrowImmutable : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ThrowImmutable"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ThrowImmutable
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ThrowImmutable;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DefineClassElement"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DefineClassElement
    // Broiler-Human:        PENDING
    internal readonly struct StepDefineClassElement : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DefineClassElement"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DefineClassElement
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DefineClassElement;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Pop"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Pop
    // Broiler-Human:        PENDING
    internal readonly struct StepPop : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Pop"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Pop
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Pop;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Duplicate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Duplicate
    // Broiler-Human:        PENDING
    internal readonly struct StepDuplicate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Duplicate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Duplicate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Duplicate;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DuplicateTwo"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DuplicateTwo
    // Broiler-Human:        PENDING
    internal readonly struct StepDuplicateTwo : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DuplicateTwo"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DuplicateTwo
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DuplicateTwo;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Swap"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Swap
    // Broiler-Human:        PENDING
    internal readonly struct StepSwap : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Swap"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Swap
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Swap;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.Pick"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than Pick
    // Broiler-Human:        PENDING
    internal readonly struct StepPick : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.Pick"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than Pick
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.Pick;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.NewPrivateName"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than NewPrivateName
    // Broiler-Human:        PENDING
    internal readonly struct StepNewPrivateName : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.NewPrivateName"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than NewPrivateName
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.NewPrivateName;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.LoadPrivate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than LoadPrivate
    // Broiler-Human:        PENDING
    internal readonly struct StepLoadPrivate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.LoadPrivate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than LoadPrivate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.LoadPrivate;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.StorePrivate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than StorePrivate
    // Broiler-Human:        PENDING
    internal readonly struct StepStorePrivate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.StorePrivate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than StorePrivate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.StorePrivate;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.HasPrivate"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than HasPrivate
    // Broiler-Human:        PENDING
    internal readonly struct StepHasPrivate : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.HasPrivate"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than HasPrivate
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.HasPrivate;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.RunStaticElements"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than RunStaticElements
    // Broiler-Human:        PENDING
    internal readonly struct StepRunStaticElements : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.RunStaticElements"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateStartAsync
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateStartAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateStartAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateNextAsync
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateNextAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateNextAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than IterateNextAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateNextAsync;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateAwaitStep"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateAwaitStep
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateAwaitStep : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateAwaitStep"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than IterateAwaitStep
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateAwaitStep;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateCloseAsync"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateCloseAsync
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateCloseAsync : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateCloseAsync"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than IterateCloseAsync
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateCloseAsync;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.IterateCloseCheck"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than IterateCloseCheck
    // Broiler-Human:        PENDING
    internal readonly struct StepIterateCloseCheck : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.IterateCloseCheck"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than IterateCloseCheck
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.IterateCloseCheck;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DeclareGlobalLet"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DeclareGlobalLet
    // Broiler-Human:        PENDING
    internal readonly struct StepDeclareGlobalLet : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeclareGlobalLet"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DeclareGlobalLet
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeclareGlobalLet;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DeclareGlobalConst"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DeclareGlobalConst
    // Broiler-Human:        PENDING
    internal readonly struct StepDeclareGlobalConst : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeclareGlobalConst"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DeclareGlobalConst
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeclareGlobalConst;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.InitialiseGlobalLexical"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than InitialiseGlobalLexical
    // Broiler-Human:        PENDING
    internal readonly struct StepInitialiseGlobalLexical : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.InitialiseGlobalLexical"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than InitialiseGlobalLexical
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.InitialiseGlobalLexical;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.DeleteGlobalBinding"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than DeleteGlobalBinding
    // Broiler-Human:        PENDING
    internal readonly struct StepDeleteGlobalBinding : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.DeleteGlobalBinding"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than DeleteGlobalBinding
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.DeleteGlobalBinding;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.EnterBody"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than EnterBody
    // Broiler-Human:        PENDING
    internal readonly struct StepEnterBody : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.EnterBody"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than EnterBody
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.EnterBody;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ImportCall"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ImportCall
    // Broiler-Human:        PENDING
    internal readonly struct StepImportCall : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ImportCall"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ImportCall
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ImportCall;
        }
    }

    /// <summary>The step that runs one <see cref="JsOpcode.ImportMeta"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this step names an opcode other than ImportMeta
    // Broiler-Human:        PENDING
    internal readonly struct StepImportMeta : IJsExecutionMode
    {
        /// <summary>Always <see cref="JsOpcode.ImportMeta"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=TBF
        // Broiler-Falsified-If: this answers any opcode other than ImportMeta
        // Broiler-Human:        PENDING
        public static JsOpcode Opcode
        {
            [System.Runtime.CompilerServices.MethodImpl(
                System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            get => JsOpcode.ImportMeta;
        }
    }
}
