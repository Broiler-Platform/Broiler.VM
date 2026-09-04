// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           5
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// A JavaScript <c>throw</c> in flight, carried on the CLR's own exception mechanism.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why a CLR exception and not a completion record.</b> A built-in written in C# can throw, and
/// so can a getter it calls, and so can a comparison function a sort calls. Threading a completion
/// record through every one of those would mean every built-in returning a two-field result and
/// every caller checking it - and the one that forgets is a silent wrong answer rather than a
/// crash. The CLR's mechanism already unwinds C# frames correctly; this type is what it carries.
/// </para>
/// <para>
/// It never escapes the profile. The interpreter catches it at every frame that has a region, and
/// the executor catches whatever is left and turns it into the profile fault the core's result
/// envelope carries.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9D5643
// Broiler-Human:        PENDING
internal sealed class JsThrow : System.Exception
{
    /// <summary>Creates a throw carrying <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=861E48
    // Broiler-Human:        PENDING
    internal JsThrow(JsValue value, string rendered)
        : base(rendered) => Value = value;

    /// <summary>The thrown value, which is any JavaScript value and not necessarily an Error.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CF3706
    // Broiler-Human:        PENDING
    internal JsValue Value { get; }
}

/// <summary>
/// A stop that is not a JavaScript exception: an allowance spent, a cancellation, or a bound this
/// profile enforces and the language has no value for.
/// </summary>
/// <remarks>
/// It is deliberately not catchable by guest code. A program that could <c>catch</c> its own fuel
/// exhaustion could spend the rest of its allowance deciding what to do about it, which is the one
/// thing an allowance exists to prevent.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=634ADF
// Broiler-Human:        PENDING
internal sealed class JsAbort : System.Exception
{
    /// <summary>Creates an abort.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=26C8FA
    // Broiler-Human:        PENDING
    internal JsAbort(JsAbortKind kind, string detail)
        : base(detail) => Kind = kind;

    /// <summary>Which stop this is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DAB379
    // Broiler-Human:        PENDING
    internal JsAbortKind Kind { get; }
}

/// <summary>The stops that are not JavaScript exceptions.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F162B4
// Broiler-Human:        PENDING
internal enum JsAbortKind
{
    /// <summary>A budget dimension was spent.</summary>
    Exhausted = 0,

    /// <summary>Cancellation was observed at a polling point.</summary>
    Cancelled = 1,

    /// <summary>The interpreter reached a state its verifier should have made unreachable.</summary>
    InternalDefect = 2,
}
