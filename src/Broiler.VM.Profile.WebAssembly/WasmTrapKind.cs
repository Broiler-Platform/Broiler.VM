// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           14
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  1/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The closed list of traps a WebAssembly guest can reach, as the specification names them.
/// </summary>
/// <remarks>
/// <para>
/// <b>A TRAP IS A VALUE AND NEVER AN EXCEPTION.</b> Every member here leaves the interpreter as a
/// return code threaded through the dispatch loop, is carried out of the executor inside
/// <see cref="WebAssemblyTrap"/>, and reaches the caller behind the core's profile-fault outcome.
/// Nothing derived from a CLR exception type crosses the boundary, and no member of this list is
/// raised by throwing.
/// </para>
/// <para>
/// <b>The list is the specification's and not this build's, so it carries a member this build does
/// not raise.</b> <see cref="UndefinedElement"/> is the name an earlier revision of the
/// specification gave the out-of-bounds indirect call that this build reports as
/// <see cref="OutOfBoundsTableAccess"/>. No path here raises it. It is listed because the list is
/// closed by the specification rather than by what one implementation happens to reach, and
/// removing it would make a later build that distinguishes the two cases look like a widening.
/// </para>
/// <para>
/// <b>Exhaustion is not on this list and never becomes a member of it.</b> Running out of call
/// depth, fuel, wall clock or allocated bytes is a resource exhaustion naming a dimension and a
/// scope; the specification's own conformance suite asserts the two families separately, and a
/// build that reported exhaustion as a trap would pass a trap assertion for the wrong reason.
/// </para>
/// <para>
/// The three members the reference-type, garbage-collection and exception manifests would add - a
/// null reference dereference, an out-of-bounds array access and a failed cast - are absent because
/// those manifests are not minted. A trap kind arrives with the surface that can raise it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=FD59E7
// Broiler-Falsified-If: a member here is raised by throwing, or a resource exhaustion is reported as one of these
// Broiler-Human:        PENDING
public enum WasmTrapKind
{
    /// <summary>The <c>unreachable</c> instruction was executed.</summary>
    Unreachable = 1,

    /// <summary>An integer division or remainder had a zero divisor.</summary>
    IntegerDivideByZero = 2,

    /// <summary>
    /// A signed division overflowed, or a float-to-integer conversion was out of range.
    /// </summary>
    IntegerOverflow = 3,

    /// <summary>A float-to-integer conversion was handed a NaN.</summary>
    InvalidConversionToInteger = 4,

    /// <summary>A load or store addressed bytes outside the memory's current size.</summary>
    OutOfBoundsMemoryAccess = 5,

    /// <summary>A table access addressed an entry outside the table's current size.</summary>
    OutOfBoundsTableAccess = 6,

    /// <summary>
    /// The earlier specification revision's name for an out-of-bounds indirect call, which this
    /// build reports as <see cref="OutOfBoundsTableAccess"/> and therefore never raises.
    /// </summary>
    UndefinedElement = 7,

    /// <summary>An indirect call reached a function whose signature was not the declared one.</summary>
    IndirectCallTypeMismatch = 8,

    /// <summary>An indirect call reached a table entry holding a null reference.</summary>
    UninitializedElement = 9,
}

/// <summary>
/// What one run of the interpreter ended as, kept apart from the trap kind so that the trap list
/// stays exactly the specification's.
/// </summary>
/// <remarks>
/// The list has no member for a successful trap, because a trap is not a status: a run that trapped
/// answers <see cref="Trapped"/> and the trap kind travels beside it. Adding a <c>None</c> member to
/// <see cref="WasmTrapKind"/> to carry that would have widened a closed specification list with an
/// implementation's bookkeeping.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5F82C8
// Broiler-Human:        PENDING
internal enum WasmRunStatus
{
    /// <summary>The entry function returned, and its results are on the operand stack.</summary>
    Completed = 0,

    /// <summary>A trap was raised, and the interpreter carries which one and where.</summary>
    Trapped = 1,

    /// <summary>
    /// A charge was refused. The meter has latched the refusal, so the core rewrites the step
    /// whatever this executor answers with.
    /// </summary>
    Exhausted = 2,

    /// <summary>A poll observed cancellation.</summary>
    Cancelled = 3,

    /// <summary>
    /// The interpreter disagreed with what validation proved. This is a defect in this assembly and
    /// is reported as one rather than as anything an artifact did.
    /// </summary>
    Defect = 4,
}
