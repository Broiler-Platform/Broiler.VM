// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           79
// Human-reviewed:   0/11
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The instruction set of format version 2, at the <c>broiler.javascript.wide</c> surface.
/// </summary>
/// <remarks>
/// <para>
/// Version 1's set is arithmetic over three primitive types in one frame. This one is the set a
/// program with objects, functions, closures, exceptions and a standard library needs, and it is a
/// separate format version rather than an extension because every one of those needs a section
/// version 1 does not frame - a function table, an environment model and exception regions with a
/// scope depth in them.
/// </para>
/// <para>
/// The invariant version 1 established is kept: <b>one opcode byte followed by a fixed operand
/// width</b>, so an instruction boundary is computable from the opcode alone and the verifier can
/// build the boundary set without executing anything. Where version 1 wrote jump displacements
/// relative to the following instruction, this version writes <b>absolute code offsets</b>: all
/// functions share one code section, and an absolute target is checkable against the declared
/// range of the function that contains the jump without the verifier having to reconstruct where
/// the jump came from.
/// </para>
/// <para>
/// <b>The set is deliberately not complete JavaScript.</b> There is no generator, no
/// <c>await</c>, no class, no spread, no destructuring and no <c>with</c>; each is a construct the
/// manifest refuses at the front end rather than an opcode the executor would have to answer for.
/// What is here is what a program that runs has to have.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C7B0B2
// Broiler-Human:        PENDING
public enum JsOpcode : byte
{
    /// <summary>Does nothing. Never emitted; a hole in a patched stream would be a defect.</summary>
    Nop = 0x00,

    // ---- literals ------------------------------------------------------------------------------

    /// <summary>Push <c>undefined</c>.</summary>
    LoadUndefined = 0x01,

    /// <summary>Push <c>null</c>.</summary>
    LoadNull = 0x02,

    /// <summary>Push <c>true</c>.</summary>
    LoadTrue = 0x03,

    /// <summary>Push <c>false</c>.</summary>
    LoadFalse = 0x04,

    /// <summary>Push constant pool entry <c>u16</c>.</summary>
    LoadConstant = 0x05,

    /// <summary>Push the frame's <c>this</c> binding.</summary>
    LoadThis = 0x06,

    /// <summary>Push a fresh <c>arguments</c> object built from the frame's actual arguments.</summary>
    NewArguments = 0x07,

    // ---- bindings ------------------------------------------------------------------------------

    /// <summary>Push environment slot <c>u16</c>, <c>u8</c> scopes up. Throws on an uninitialised slot.</summary>
    LoadScoped = 0x10,

    /// <summary>Pop into environment slot <c>u16</c>, <c>u8</c> scopes up. Throws on an uninitialised slot.</summary>
    StoreScoped = 0x11,

    /// <summary>Pop into environment slot <c>u16</c>, <c>u8</c> scopes up, initialising it.</summary>
    InitialiseScoped = 0x12,

    /// <summary>Push the global property named by constant <c>u16</c>. Throws when it is absent.</summary>
    LoadGlobal = 0x13,

    /// <summary>Pop into the global property named by constant <c>u16</c>, creating it if absent.</summary>
    StoreGlobal = 0x14,

    /// <summary>Push <c>undefined</c> for an absent global rather than throwing. Backs <c>typeof</c>.</summary>
    LoadGlobalOrUndefined = 0x15,

    /// <summary>Push a fresh environment of <c>u16</c> slots whose parent is the current one.</summary>
    PushScope = 0x16,

    /// <summary>Discard the current environment and continue in its parent.</summary>
    PopScope = 0x17,

    /// <summary>Replace the current environment with a copy of it, for a per-iteration binding.</summary>
    CopyScope = 0x18,

    /// <summary>Define the global named by constant <c>u16</c> as <c>undefined</c> when absent.</summary>
    DeclareGlobal = 0x19,

    // ---- objects and properties ------------------------------------------------------------------

    /// <summary>Push a fresh ordinary object with the realm's <c>Object.prototype</c>.</summary>
    NewObject = 0x20,

    /// <summary>Pop <c>u16</c> values and push an Array holding them in order.</summary>
    NewArray = 0x21,

    /// <summary>Pop a base; push its property named by constant <c>u16</c>.</summary>
    GetProperty = 0x22,

    /// <summary>Pop a value and a base; set the named property and push the value back.</summary>
    SetProperty = 0x23,

    /// <summary>Pop a key and a base; push the property.</summary>
    GetIndex = 0x24,

    /// <summary>Pop a value, a key and a base; set the property and push the value back.</summary>
    SetIndex = 0x25,

    /// <summary>Pop a value; define constant <c>u16</c> on the object beneath it, which stays.</summary>
    DefineField = 0x26,

    /// <summary>Pop a value and a key; define it on the object beneath them, which stays.</summary>
    DefineIndexed = 0x27,

    /// <summary>Pop a base; push whether deleting constant <c>u16</c> from it succeeded.</summary>
    DeleteProperty = 0x28,

    /// <summary>Pop a key and a base; push whether the deletion succeeded.</summary>
    DeleteIndex = 0x29,

    /// <summary>Pop a function; define it as the getter for constant <c>u16</c> on the object beneath.</summary>
    DefineGetter = 0x2A,

    /// <summary>Pop a function; define it as the setter for constant <c>u16</c> on the object beneath.</summary>
    DefineSetter = 0x2B,

    // ---- functions and calls ---------------------------------------------------------------------

    /// <summary>Push a function object over code unit <c>u16</c>, closing over the current environment.</summary>
    Closure = 0x30,

    /// <summary>Pop <c>u8</c> arguments, a receiver and a callee; push the result.</summary>
    Call = 0x31,

    /// <summary>Pop <c>u8</c> arguments and a constructor; push the constructed object.</summary>
    Construct = 0x32,

    /// <summary>Pop one value and return it from the current frame.</summary>
    Return = 0x33,

    /// <summary>Return <c>undefined</c> from the current frame.</summary>
    ReturnUndefined = 0x34,

    // ---- operators ---------------------------------------------------------------------------------

    /// <summary>The <c>+</c> operator, which concatenates when either operand is a String.</summary>
    Add = 0x40,

    /// <summary>The <c>-</c> operator.</summary>
    Subtract = 0x41,

    /// <summary>The <c>*</c> operator.</summary>
    Multiply = 0x42,

    /// <summary>The <c>/</c> operator.</summary>
    Divide = 0x43,

    /// <summary>The <c>%</c> operator, whose sign follows the dividend.</summary>
    Remainder = 0x44,

    /// <summary>The <c>**</c> operator.</summary>
    Exponent = 0x45,

    /// <summary>Unary <c>-</c>.</summary>
    Negate = 0x46,

    /// <summary>Unary <c>+</c>, which is <c>ToNumber</c> and not a no-op.</summary>
    ToNumber = 0x47,

    /// <summary>Unary <c>!</c>.</summary>
    Not = 0x48,

    /// <summary>Unary <c>~</c>.</summary>
    BitwiseNot = 0x49,

    /// <summary>The <c>&lt;</c> comparison.</summary>
    LessThan = 0x4A,

    /// <summary>The <c>&lt;=</c> comparison.</summary>
    LessThanOrEqual = 0x4B,

    /// <summary>The <c>&gt;</c> comparison.</summary>
    GreaterThan = 0x4C,

    /// <summary>The <c>&gt;=</c> comparison.</summary>
    GreaterThanOrEqual = 0x4D,

    /// <summary>The <c>===</c> comparison.</summary>
    StrictEquals = 0x4E,

    /// <summary>The <c>!==</c> comparison.</summary>
    StrictNotEquals = 0x4F,

    /// <summary>The <c>==</c> comparison.</summary>
    LooseEquals = 0x50,

    /// <summary>The <c>!=</c> comparison.</summary>
    LooseNotEquals = 0x51,

    /// <summary>The <c>|</c> operator, over <c>ToInt32</c>.</summary>
    BitwiseOr = 0x52,

    /// <summary>The <c>&amp;</c> operator, over <c>ToInt32</c>.</summary>
    BitwiseAnd = 0x53,

    /// <summary>The <c>^</c> operator, over <c>ToInt32</c>.</summary>
    BitwiseXor = 0x54,

    /// <summary>The <c>&lt;&lt;</c> operator.</summary>
    ShiftLeft = 0x55,

    /// <summary>The <c>&gt;&gt;</c> operator.</summary>
    ShiftRight = 0x56,

    /// <summary>The <c>&gt;&gt;&gt;</c> operator, over <c>ToUint32</c>.</summary>
    ShiftRightUnsigned = 0x57,

    /// <summary>Pop one; push the <c>typeof</c> string for it.</summary>
    TypeOf = 0x58,

    /// <summary>The <c>instanceof</c> operator.</summary>
    InstanceOf = 0x59,

    /// <summary>The <c>in</c> operator.</summary>
    In = 0x5A,

    /// <summary>Pop one and push <c>undefined</c>. The <c>void</c> operator.</summary>
    Void = 0x5B,

    // ---- control flow ---------------------------------------------------------------------------------

    /// <summary>Continue at absolute code offset <c>u32</c>.</summary>
    Jump = 0x60,

    /// <summary>Pop one; jump to <c>u32</c> when <c>ToBoolean</c> of it is false.</summary>
    JumpIfFalse = 0x61,

    /// <summary>Pop one; jump to <c>u32</c> when <c>ToBoolean</c> of it is true.</summary>
    JumpIfTrue = 0x62,

    /// <summary>Pop one and throw it as a JavaScript exception.</summary>
    Throw = 0x63,

    /// <summary>Pop an object; push an enumerator over its <c>for…in</c> property names.</summary>
    ForInStart = 0x64,

    /// <summary>
    /// Pop an enumerator; push its next name, or jump to <c>u32</c> when it is exhausted.
    /// </summary>
    /// <remarks>
    /// <b>It pops rather than peeks, and the enumerator lives in a scope slot between turns.</b>
    /// Keeping it on the operand stack across the loop body was the obvious encoding and it is
    /// wrong: a <c>break</c> out of the loop would leave it behind, so the break target is reached
    /// at two different heights, and a <c>return</c> from inside the body would reach
    /// <see cref="Return"/> with two values on the stack. Both are refused by the verifier, which
    /// is the good outcome - but the code they refuse is code a correct program produced.
    /// </remarks>
    ForInNext = 0x65,

    // ---- stack ------------------------------------------------------------------------------------------

    /// <summary>Pop one and discard it.</summary>
    Pop = 0x70,

    /// <summary>Push a second copy of the top of the stack.</summary>
    Duplicate = 0x71,

    /// <summary>Push copies of the top two values, in order.</summary>
    DuplicateTwo = 0x72,

    /// <summary>Exchange the top two values.</summary>
    Swap = 0x73,

    /// <summary>Push a copy of the value <c>u8</c> places below the top.</summary>
    Pick = 0x74,
}

/// <summary>The operand shape that follows an opcode byte.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A52162
// Broiler-Human:        PENDING
public enum JsOperandShape : byte
{
    /// <summary>No operand.</summary>
    None = 0,

    /// <summary>One unsigned byte.</summary>
    U8 = 1,

    /// <summary>One little-endian unsigned 16-bit integer.</summary>
    U16 = 2,

    /// <summary>One little-endian unsigned 32-bit code offset.</summary>
    U32 = 3,

    /// <summary>An unsigned byte followed by a little-endian unsigned 16-bit integer.</summary>
    U8U16 = 4,
}

/// <summary>
/// What the verifier, the encoder and the executor must all agree about a version-2 opcode.
/// </summary>
/// <remarks>
/// One table, in the format assembly, for the same reason version 1 has one: the executor and the
/// lowering must agree about the bytecode and neither may depend on the other. A stack effect that
/// varies with an operand - a call's argument count, an array literal's element count - is reported
/// through <see cref="TryDescribe"/> rather than through a pair of constants, because a constant
/// that is right for one operand and wrong for another is worse than no constant.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=32AB60
// Broiler-Human:        PENDING
public static class JsOpcodes
{
    /// <summary>Every opcode format version 2 defines, in ascending numeric order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=374CD6
    // Broiler-Human:        PENDING
    public static readonly JsOpcode[] All =
    [
        JsOpcode.Nop,
        JsOpcode.LoadUndefined, JsOpcode.LoadNull, JsOpcode.LoadTrue, JsOpcode.LoadFalse,
        JsOpcode.LoadConstant, JsOpcode.LoadThis, JsOpcode.NewArguments,
        JsOpcode.LoadScoped, JsOpcode.StoreScoped, JsOpcode.InitialiseScoped,
        JsOpcode.LoadGlobal, JsOpcode.StoreGlobal, JsOpcode.LoadGlobalOrUndefined,
        JsOpcode.PushScope, JsOpcode.PopScope, JsOpcode.CopyScope, JsOpcode.DeclareGlobal,
        JsOpcode.NewObject, JsOpcode.NewArray,
        JsOpcode.GetProperty, JsOpcode.SetProperty, JsOpcode.GetIndex, JsOpcode.SetIndex,
        JsOpcode.DefineField, JsOpcode.DefineIndexed,
        JsOpcode.DeleteProperty, JsOpcode.DeleteIndex,
        JsOpcode.DefineGetter, JsOpcode.DefineSetter,
        JsOpcode.Closure, JsOpcode.Call, JsOpcode.Construct,
        JsOpcode.Return, JsOpcode.ReturnUndefined,
        JsOpcode.Add, JsOpcode.Subtract, JsOpcode.Multiply, JsOpcode.Divide,
        JsOpcode.Remainder, JsOpcode.Exponent, JsOpcode.Negate, JsOpcode.ToNumber,
        JsOpcode.Not, JsOpcode.BitwiseNot,
        JsOpcode.LessThan, JsOpcode.LessThanOrEqual, JsOpcode.GreaterThan,
        JsOpcode.GreaterThanOrEqual, JsOpcode.StrictEquals, JsOpcode.StrictNotEquals,
        JsOpcode.LooseEquals, JsOpcode.LooseNotEquals,
        JsOpcode.BitwiseOr, JsOpcode.BitwiseAnd, JsOpcode.BitwiseXor,
        JsOpcode.ShiftLeft, JsOpcode.ShiftRight, JsOpcode.ShiftRightUnsigned,
        JsOpcode.TypeOf, JsOpcode.InstanceOf, JsOpcode.In, JsOpcode.Void,
        JsOpcode.Jump, JsOpcode.JumpIfFalse, JsOpcode.JumpIfTrue, JsOpcode.Throw,
        JsOpcode.ForInStart, JsOpcode.ForInNext,
        JsOpcode.Pop, JsOpcode.Duplicate, JsOpcode.DuplicateTwo, JsOpcode.Swap, JsOpcode.Pick,
    ];

    /// <summary>Whether <paramref name="value"/> is an opcode format version 2 defines.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=827F19
    // Broiler-Human:        PENDING
    public static bool IsDefined(byte value) => Shape((JsOpcode)value) is not null;

    /// <summary>How many operand bytes follow <paramref name="opcode"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B480F7
    // Broiler-Human:        PENDING
    public static int OperandWidth(JsOpcode opcode) => Shape(opcode) switch
    {
        JsOperandShape.U8 => 1,
        JsOperandShape.U16 => 2,
        JsOperandShape.U32 => 4,
        JsOperandShape.U8U16 => 3,
        _ => 0,
    };

    /// <summary>The whole width of one instruction, opcode byte included.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9DEEA7
    // Broiler-Human:        PENDING
    public static int InstructionWidth(JsOpcode opcode) => 1 + OperandWidth(opcode);

    /// <summary>Whether this opcode ends a basic block by transferring control unconditionally.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5989AC
    // Broiler-Human:        PENDING
    public static bool IsTerminal(JsOpcode opcode) => opcode switch
    {
        JsOpcode.Jump or JsOpcode.Return or JsOpcode.ReturnUndefined or JsOpcode.Throw => true,
        _ => false,
    };

    /// <summary>Whether this opcode's <c>u32</c> operand is a code offset the verifier must check.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=16996C
    // Broiler-Human:        PENDING
    public static bool HasCodeTarget(JsOpcode opcode) => opcode switch
    {
        JsOpcode.Jump or JsOpcode.JumpIfFalse or JsOpcode.JumpIfTrue or JsOpcode.ForInNext => true,
        _ => false,
    };

    /// <summary>
    /// The operand shape of <paramref name="opcode"/>, or <see langword="null"/> when this format
    /// version does not define it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9F6EA7
    // Broiler-Human:        PENDING
    public static JsOperandShape? Shape(JsOpcode opcode) => opcode switch
    {
        JsOpcode.Nop or
        JsOpcode.LoadUndefined or JsOpcode.LoadNull or JsOpcode.LoadTrue or JsOpcode.LoadFalse or
        JsOpcode.LoadThis or JsOpcode.NewArguments or
        JsOpcode.PopScope or
        JsOpcode.NewObject or
        JsOpcode.GetIndex or JsOpcode.SetIndex or JsOpcode.DefineIndexed or JsOpcode.DeleteIndex or
        JsOpcode.Return or JsOpcode.ReturnUndefined or
        JsOpcode.Add or JsOpcode.Subtract or JsOpcode.Multiply or JsOpcode.Divide or
        JsOpcode.Remainder or JsOpcode.Exponent or JsOpcode.Negate or JsOpcode.ToNumber or
        JsOpcode.Not or JsOpcode.BitwiseNot or
        JsOpcode.LessThan or JsOpcode.LessThanOrEqual or JsOpcode.GreaterThan or
        JsOpcode.GreaterThanOrEqual or JsOpcode.StrictEquals or JsOpcode.StrictNotEquals or
        JsOpcode.LooseEquals or JsOpcode.LooseNotEquals or
        JsOpcode.BitwiseOr or JsOpcode.BitwiseAnd or JsOpcode.BitwiseXor or
        JsOpcode.ShiftLeft or JsOpcode.ShiftRight or JsOpcode.ShiftRightUnsigned or
        JsOpcode.TypeOf or JsOpcode.InstanceOf or JsOpcode.In or JsOpcode.Void or
        JsOpcode.Throw or JsOpcode.ForInStart or
        JsOpcode.Pop or JsOpcode.Duplicate or JsOpcode.DuplicateTwo or JsOpcode.Swap
            => JsOperandShape.None,

        JsOpcode.Call or JsOpcode.Construct or JsOpcode.Pick => JsOperandShape.U8,

        JsOpcode.LoadConstant or
        JsOpcode.LoadGlobal or JsOpcode.StoreGlobal or JsOpcode.LoadGlobalOrUndefined or
        JsOpcode.PushScope or JsOpcode.CopyScope or JsOpcode.DeclareGlobal or
        JsOpcode.NewArray or
        JsOpcode.GetProperty or JsOpcode.SetProperty or JsOpcode.DefineField or
        JsOpcode.DeleteProperty or JsOpcode.DefineGetter or JsOpcode.DefineSetter or
        JsOpcode.Closure
            => JsOperandShape.U16,

        JsOpcode.Jump or JsOpcode.JumpIfFalse or JsOpcode.JumpIfTrue or JsOpcode.ForInNext
            => JsOperandShape.U32,

        JsOpcode.LoadScoped or JsOpcode.StoreScoped or JsOpcode.InitialiseScoped
            => JsOperandShape.U8U16,

        _ => null,
    };

    /// <summary>
    /// The stack effect of one instruction, given the operand actually encoded with it.
    /// </summary>
    /// <remarks>
    /// <paramref name="pops"/> and <paramref name="pushes"/> are the counts the executor performs,
    /// and the verifier's abstract height is computed from them alone. A false answer means the
    /// opcode is not one this format version defines - not that its effect is unknown.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=FCBDB3
    // Broiler-Human:        PENDING
    public static bool TryDescribe(JsOpcode opcode, uint operand, out int pops, out int pushes)
    {
        pops = 0;
        pushes = 0;

        switch (opcode)
        {
            case JsOpcode.Nop:
            case JsOpcode.PopScope:
            case JsOpcode.PushScope:
            case JsOpcode.CopyScope:
            case JsOpcode.DeclareGlobal:
                return true;

            case JsOpcode.LoadUndefined:
            case JsOpcode.LoadNull:
            case JsOpcode.LoadTrue:
            case JsOpcode.LoadFalse:
            case JsOpcode.LoadConstant:
            case JsOpcode.LoadThis:
            case JsOpcode.NewArguments:
            case JsOpcode.LoadScoped:
            case JsOpcode.LoadGlobal:
            case JsOpcode.LoadGlobalOrUndefined:
            case JsOpcode.NewObject:
            case JsOpcode.Closure:
            case JsOpcode.Duplicate:
            case JsOpcode.Pick:
                pushes = 1;
                return true;

            case JsOpcode.DuplicateTwo:
                pushes = 2;
                return true;

            case JsOpcode.StoreScoped:
            case JsOpcode.InitialiseScoped:
            case JsOpcode.StoreGlobal:
            case JsOpcode.Pop:
            case JsOpcode.Throw:
            case JsOpcode.Return:
            case JsOpcode.JumpIfFalse:
            case JsOpcode.JumpIfTrue:
                pops = 1;
                return true;

            case JsOpcode.Jump:
            case JsOpcode.ReturnUndefined:
                return true;

            case JsOpcode.NewArray:
                pops = checked((int)operand);
                pushes = 1;
                return true;

            case JsOpcode.GetProperty:
            case JsOpcode.DeleteProperty:
            case JsOpcode.Negate:
            case JsOpcode.ToNumber:
            case JsOpcode.Not:
            case JsOpcode.BitwiseNot:
            case JsOpcode.TypeOf:
            case JsOpcode.Void:
            case JsOpcode.ForInStart:
                pops = 1;
                pushes = 1;
                return true;

            case JsOpcode.GetIndex:
            case JsOpcode.DeleteIndex:
            case JsOpcode.SetProperty:
            case JsOpcode.Add:
            case JsOpcode.Subtract:
            case JsOpcode.Multiply:
            case JsOpcode.Divide:
            case JsOpcode.Remainder:
            case JsOpcode.Exponent:
            case JsOpcode.LessThan:
            case JsOpcode.LessThanOrEqual:
            case JsOpcode.GreaterThan:
            case JsOpcode.GreaterThanOrEqual:
            case JsOpcode.StrictEquals:
            case JsOpcode.StrictNotEquals:
            case JsOpcode.LooseEquals:
            case JsOpcode.LooseNotEquals:
            case JsOpcode.BitwiseOr:
            case JsOpcode.BitwiseAnd:
            case JsOpcode.BitwiseXor:
            case JsOpcode.ShiftLeft:
            case JsOpcode.ShiftRight:
            case JsOpcode.ShiftRightUnsigned:
            case JsOpcode.InstanceOf:
            case JsOpcode.In:
                pops = 2;
                pushes = 1;
                return true;

            case JsOpcode.SetIndex:
                pops = 3;
                pushes = 1;
                return true;

            // The object stays. A literal is a run of definitions over one object, and popping it
            // between them would mean re-pushing it, which doubles the code for no gain.
            case JsOpcode.DefineField:
            case JsOpcode.DefineGetter:
            case JsOpcode.DefineSetter:
                pops = 1;
                return true;

            case JsOpcode.DefineIndexed:
                pops = 2;
                return true;

            // Callee, receiver and the arguments go; one result comes back.
            case JsOpcode.Call:
                pops = checked((int)operand) + 2;
                pushes = 1;
                return true;

            case JsOpcode.Construct:
                pops = checked((int)operand) + 1;
                pushes = 1;
                return true;

            // The FALLTHROUGH effect: the enumerator goes and a name arrives. On the taken
            // branch nothing arrives, so the target's height is one below this instruction's - a
            // rule the verifier applies at the target rather than here, because one instruction
            // with two effects is exactly what a stack-height check exists to pin down.
            case JsOpcode.ForInNext:
                pops = 1;
                pushes = 1;
                return true;

            case JsOpcode.Swap:
                pops = 2;
                pushes = 2;
                return true;

            default:
                return false;
        }
    }
}
