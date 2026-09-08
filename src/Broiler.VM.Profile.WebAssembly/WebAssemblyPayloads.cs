// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           27
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  2/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>Which of the four numeric types a returned value is.</summary>
/// <remarks>
/// It is a payload's own vocabulary rather than the decoder's <c>WasmValueType</c>, because the
/// decoder's enum carries the format's encoding bytes and two members this surface cannot produce.
/// A caller reading a result should not have to know how the binary format spells a type.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E32E24
// Broiler-Human:        PENDING
public enum WebAssemblyValueKind
{
    /// <summary>A 32-bit integer.</summary>
    I32 = 1,

    /// <summary>A 64-bit integer.</summary>
    I64 = 2,

    /// <summary>A single-precision float.</summary>
    F32 = 3,

    /// <summary>A double-precision float.</summary>
    F64 = 4,
}

/// <summary>One returned value: a kind and the exact bits.</summary>
/// <remarks>
/// The bits are carried rather than a <see cref="float"/> or a <see cref="double"/> field, so a NaN
/// crosses the boundary with the payload it left the guest with. A caller that wants a number reads
/// <see cref="AsSingle"/> or <see cref="AsDouble"/>; a caller that wants to compare a NaN reads
/// <see cref="Bits"/>.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=378F47
// Broiler-Falsified-If: a value crossing this type is quieted, rounded or re-encoded
// Broiler-Human:        PENDING
public readonly struct WebAssemblyValue
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=960F8C
    // Broiler-Human:        PENDING
    internal WebAssemblyValue(WebAssemblyValueKind kind, ulong bits)
    {
        Kind = kind;
        Bits = bits;
    }

    /// <summary>Which of the four numeric types this value is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4FC682
    // Broiler-Human:        PENDING
    public WebAssemblyValueKind Kind { get; }

    /// <summary>The exact bits, zero-extended into sixty-four.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7BFC96
    // Broiler-Human:        PENDING
    public ulong Bits { get; }

    /// <summary>Reads the bits as a 32-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=721837
    // Broiler-Human:        PENDING
    public int AsInt32 => (int)(uint)Bits;

    /// <summary>Reads the bits as a 64-bit integer.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=122BB9
    // Broiler-Human:        PENDING
    public long AsInt64 => (long)Bits;

    /// <summary>Reads the bits as a single-precision float.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D69401
    // Broiler-Human:        PENDING
    public float AsSingle => System.BitConverter.UInt32BitsToSingle((uint)Bits);

    /// <summary>Reads the bits as a double-precision float.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=03158D
    // Broiler-Human:        PENDING
    public double AsDouble => System.BitConverter.UInt64BitsToDouble(Bits);
}

/// <summary>
/// What an entry point returned: several values, because a WebAssembly function may return several.
/// </summary>
/// <remarks>
/// <b>THE RESULT CHANNEL IS THE HALF OF THE INVOCATION CONTRACT THAT IS ALREADY ADEQUATE.</b> The
/// request carries one name and no arguments, which is why this profile encodes its arguments into
/// that name; the result travels as a typed profile payload, which can carry as many values as the
/// function declared. So multi-value returns are expressible on the shipped contract with no
/// amendment, and this type is written for several values from the first commit even though the
/// accepted manifest admits at most one result per function today.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=42DE4C
// Broiler-Human:        PENDING
public sealed class WebAssemblyResults : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7145EF
    // Broiler-Human:        PENDING
    private readonly WebAssemblyValue[] values;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A9C0DA
    // Broiler-Human:        PENDING
    internal WebAssemblyResults(VmProfileId profileId, WebAssemblyValue[] returned)
    {
        Identity = new VmPayloadIdentity(profileId, WebAssemblyProfile.ResultsKindId, 1);
        values = returned;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>How many values the entry point returned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8AB04A
    // Broiler-Human:        PENDING
    public int Count => values.Length;

    /// <summary>Reads one returned value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=868C06
    // Broiler-Human:        PENDING
    public bool TryGetValue(int index, out WebAssemblyValue value)
    {
        value = default;

        if (index < 0 || index >= values.Length)
        {
            return false;
        }

        value = values[index];
        return true;
    }
}

/// <summary>
/// A trap that ended an instantiation or an invocation: which one, its diagnostic code, and where.
/// </summary>
/// <remarks>
/// <b>THE POSITION TRAVELS IN THE PAYLOAD BECAUSE THERE IS NOWHERE ELSE FOR IT.</b> The core's
/// diagnostics record populates its source position on the verification path alone, so a trap's
/// function index and byte offset can only reach a caller here. The position is this profile's own
/// coordinate system: the section identifier of the code section, the byte offset of the trapping
/// instruction inside its own function body, and the function index in the first profile
/// coordinate.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=211660
// Broiler-Falsified-If: a trap reaches a caller as a CLR exception rather than as one of these
// Broiler-Human:        PENDING
public sealed class WebAssemblyTrap : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=937685
    // Broiler-Human:        PENDING
    internal WebAssemblyTrap(
        VmProfileId profileId, WasmTrapKind kind, int diagnosticCode, VmSourcePosition position)
    {
        Identity = new VmPayloadIdentity(profileId, WebAssemblyProfile.TrapKindId, 1);
        Kind = kind;
        DiagnosticCode = diagnosticCode;
        Position = position;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>Which trap, from the specification's closed list.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0F9FEA
    // Broiler-Human:        PENDING
    public WasmTrapKind Kind { get; }

    /// <summary>The stable code from this profile's diagnostic registry.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B7A6DB
    // Broiler-Human:        PENDING
    public int DiagnosticCode { get; }

    /// <summary>Where the trap happened, in this profile's own coordinates.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=614AEA
    // Broiler-Human:        PENDING
    public VmSourcePosition Position { get; }
}

/// <summary>Why an entry point could not be resolved into a call.</summary>
/// <remarks>
/// <b>NONE OF THESE IS A TRAP, AND THAT IS WHY THEY ARE NOT IN THE TRAP LIST.</b> A trap is
/// something a running module did; every member here is something that happened before a single
/// instruction was dispatched, to a request the embedder wrote. Reporting one as a trap would put an
/// embedder's typing mistake into the same family as an out-of-bounds store, and the specification's
/// own suite keeps those families apart.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B7BAD8
// Broiler-Human:        PENDING
public enum WebAssemblyEntryPointProblem
{
    /// <summary>No problem; the entry point resolved.</summary>
    None = 0,

    /// <summary>The entry-point text was empty.</summary>
    Empty = 1,

    /// <summary>A byte count was missing, unterminated, or longer than the text that followed it.</summary>
    MalformedLengthPrefix = 2,

    /// <summary>The declared name length ran past the end of the text.</summary>
    NameTruncated = 3,

    /// <summary>An argument group named a type this encoding does not define.</summary>
    UnknownArgumentType = 4,

    /// <summary>An argument literal was not the spelling its type fixes.</summary>
    MalformedLiteral = 5,

    /// <summary>The entry point carried more arguments than this profile admits.</summary>
    TooManyArguments = 6,

    /// <summary>The module exports no such name.</summary>
    UnknownExport = 7,

    /// <summary>The name is exported, but it names a memory, a table or a global.</summary>
    ExportIsNotAFunction = 8,

    /// <summary>The argument count is not the function's parameter count.</summary>
    ArgumentCountMismatch = 9,

    /// <summary>An argument's type is not the parameter's type.</summary>
    ArgumentTypeMismatch = 10,
}

/// <summary>
/// An entry point that could not be resolved: which problem, and the argument it was found at.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=02BC99
// Broiler-Human:        PENDING
public sealed class WebAssemblyEntryPointFault : IVmProfilePayload
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=055597
    // Broiler-Human:        PENDING
    internal WebAssemblyEntryPointFault(
        VmProfileId profileId, WebAssemblyEntryPointProblem problem, int argumentIndex)
    {
        Identity = new VmPayloadIdentity(profileId, WebAssemblyProfile.EntryPointFaultKindId, 1);
        Problem = problem;
        ArgumentIndex = argumentIndex;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AF660A
    // Broiler-Human:        PENDING
    public VmPayloadIdentity Identity { get; }

    /// <summary>What was wrong with the entry point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=54E823
    // Broiler-Human:        PENDING
    public WebAssemblyEntryPointProblem Problem { get; }

    /// <summary>
    /// Which argument the problem was found at, or minus one when it was not an argument's fault.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3824B3
    // Broiler-Human:        PENDING
    public int ArgumentIndex { get; }
}
