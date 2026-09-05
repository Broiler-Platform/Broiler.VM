// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The optional feature manifests a version-2 artifact may declare beside its own, and the global
/// names that put a program inside one.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why an artifact declares a surface at all.</b> Roadmap section 6 says that a well-formed
/// artifact using a construct outside its declared manifest is rejected <b>at verification</b>, and
/// that this is the difference between a composition declining a manifest and a composition
/// admitting one while registering no provider: the first is an invalid artifact the guest never
/// sees, the second is a run-time refusal the guest may catch. For a construct the front end
/// refuses by name — <c>eval</c>, a module declaration — the artifact's own manifest is enough to
/// carry that. For a surface that is nothing but a set of <b>globals</b> it is not: a program that
/// constructs a <c>Uint8Array</c> is, byte for byte, a program that reads a global, and no section
/// of the artifact says which globals matter. This table is what makes it say so.
/// </para>
/// <para>
/// <b>One table, three readers, and that is the point of putting it here.</b> The lowering reads it
/// to decide what to declare, the verifier reads it to decide what to refuse, and the realm reads
/// it to decide what to install. The format assembly is the one place all three can reach — the
/// profile and the lowering may not reference each other — so a disagreement between them is a
/// disagreement with this file rather than with each other.
/// </para>
/// <para>
/// <b>A <c>typeof</c> deliberately does not declare a surface.</b> The lowering emits a different
/// instruction for a name read that answers <c>undefined</c> rather than throwing, and only the
/// throwing read declares. That is what keeps <c>typeof Uint8Array === "undefined"</c> — the exact
/// shape a machine-generated program uses to find out whether it may go on — a question rather than
/// a refusal.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5C7788
// Broiler-Human:        PENDING
public static class JsSurfaces
{
    /// <summary>
    /// The binary surface: <c>ArrayBuffer</c>, <c>DataView</c> and the typed array constructors.
    /// </summary>
    /// <remarks>
    /// <b><c>SharedArrayBuffer</c> and <c>Atomics</c> are deliberately not in it.</b> They are the
    /// multi-agent surface and they need the agent model; folding them in would let a composition
    /// that wanted an ordinary byte buffer admit cross-agent shared memory by accident.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=CBA141
    // Broiler-Human:        PENDING
    public const string Binary = "broiler.javascript.binary";

    /// <summary>
    /// The dynamic surface: <c>eval</c> and the <c>Function</c> constructor.
    /// </summary>
    /// <remarks>
    /// It is a separate identity for the reason roadmap section 6 gives: a composition that
    /// registers no artifact provider must be able to decline exactly this and say so.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C39C70
    // Broiler-Human:        PENDING
    public const string Dynamic = "broiler.javascript.dynamic";

    /// <summary>
    /// The module surface: module records, live bindings, and the import and export forms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is declared by a SECTION rather than by a global, which is what makes it the odd one
    /// of the three.</b> The binary and dynamic surfaces are sets of globals, and an artifact is
    /// inside one when it reads one of those names; a module reads no name at all - what puts it
    /// inside this surface is that it carries module records. So the lowering declares this one
    /// where it writes those records, and <see cref="TryOwner"/> never answers with it.
    /// </para>
    /// <para>
    /// <b>What a composition is declining when it declines this one is RESOLUTION.</b> Turning a
    /// specifier into the identity of a module is the host's decision - a file path, a URL, a name
    /// in a bundle - and a composition with no answer to it has no business running a module graph.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=TBF
    // Broiler-Human:        PENDING
    public const string Modules = "broiler.javascript.modules";

    /// <summary>Every optional surface this build knows, in ascending ordinal order.</summary>
    /// <remarks>
    /// An artifact declaring a name that is not here is refused as naming a surface this build does
    /// not implement, which is a different failure from naming one the composition declined and
    /// carries a different diagnostic.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=78B347
    // Broiler-Human:        PENDING
    public static readonly string[] All = [Binary, Dynamic, Modules];

    /// <summary>
    /// The global names the binary surface owns, in ascending ordinal order.
    /// </summary>
    /// <remarks>
    /// The nine typed array constructors are the ones a realm with no BigInt can have.
    /// <c>BigInt64Array</c> and <c>BigUint64Array</c> are absent for that reason rather than by
    /// policy, and they are not on this list because a program naming one is naming a global this
    /// surface does not have — which is an absent global and not a declined surface.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E81922
    // Broiler-Human:        PENDING
    public static readonly string[] BinaryGlobals =
    [
        "ArrayBuffer",
        "DataView",
        "Float32Array",
        "Float64Array",
        "Int16Array",
        "Int32Array",
        "Int8Array",
        "Uint16Array",
        "Uint32Array",
        "Uint8Array",
        "Uint8ClampedArray",
    ];

    /// <summary>The global names the dynamic surface owns.</summary>
    /// <remarks>
    /// <para>
    /// <b><c>Function</c> is deliberately NOT on this list, and the asymmetry is the point.</b> The
    /// global <c>Function</c> exists whether or not the surface is admitted, because a realm whose
    /// <c>typeof Function</c> answered <c>"undefined"</c> would be making an untrue statement about
    /// itself — the intrinsic is there and every function's prototype chain ends at its prototype.
    /// What the surface decides is what the <b>constructor</b> does: admitted, it turns a String
    /// into a function; declined, it refuses at run time and says so. A program that reads
    /// <c>Function.prototype</c>, or asks whether something is <c>instanceof Function</c>, is not a
    /// program that wants the dynamic surface, and refusing its artifact would be refusing a name
    /// rather than a capability.
    /// </para>
    /// <para>
    /// <c>eval</c> is different in exactly the way that matters: the global exists for no other
    /// reason. A program that reads it wants to evaluate source, so the name IS the declaration.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B8AACE
    // Broiler-Human:        PENDING
    public static readonly string[] DynamicGlobals = ["eval"];

    /// <summary>
    /// The surface that owns <paramref name="globalName"/>, or <see langword="false"/> when the
    /// name belongs to no optional surface.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=BBFA52
    // Broiler-Human:        PENDING
    public static bool TryOwner(string globalName, out string manifestId)
    {
        foreach (var name in BinaryGlobals)
        {
            if (string.Equals(name, globalName, System.StringComparison.Ordinal))
            {
                manifestId = Binary;
                return true;
            }
        }

        foreach (var name in DynamicGlobals)
        {
            if (string.Equals(name, globalName, System.StringComparison.Ordinal))
            {
                manifestId = Dynamic;
                return true;
            }
        }

        manifestId = string.Empty;
        return false;
    }

    /// <summary>Whether <paramref name="manifestId"/> is an optional surface this build knows.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8D762A
    // Broiler-Human:        PENDING
    public static bool IsKnown(string manifestId)
    {
        foreach (var known in All)
        {
            if (string.Equals(known, manifestId, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
