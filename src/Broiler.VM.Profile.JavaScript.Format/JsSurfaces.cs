// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           0
// Human-reviewed:   0/12
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       12
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=30036F
    // Broiler-Human:        PENDING
    public const string Modules = "broiler.javascript.modules";

    /// <summary>
    /// The native surface: the artifact carries emitted machine code beside its bytecode.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is declared by a SECTION and never by a global, exactly as <see cref="Modules"/>
    /// is.</b> No name a program can write puts it inside this surface; what puts an artifact
    /// inside it is that it carries a <see cref="JsFormat.SectionKind.NativeCode"/> section, so the
    /// lowering declares it where it writes those bytes and <see cref="TryOwner"/> never answers
    /// with it.
    /// </para>
    /// <para>
    /// <b>What a composition declining this one is declining is EXECUTABLE MEMORY.</b> Running
    /// emitted code means a page this process made executable, which is a decision about the host
    /// and not about the language - and a composition with no answer to it must be able to refuse
    /// the artifact rather than to load one and hope nothing calls into the bytes. Declined, the
    /// artifact is refused where the surfaces are read, at verification, before any instruction of
    /// it is reachable; that is the same answer a declined binary or module surface gets and it is
    /// the answer roadmap section 6 requires.
    /// </para>
    /// <para>
    /// <b>A composition that admits it is not thereby saying it can run the bytes, and the gap
    /// between the two questions is the whole reason this surface is separate from an
    /// architecture.</b> <i>(Revised 2026-09-07. This paragraph read "at this build no composition
    /// can", and continued "Nothing here maps a page, arms one or calls into emitted code, and no
    /// backend encodes an instruction of any architecture", and "refusing a payload for an
    /// architecture an image has no backend for is a SECOND refusal that nothing in this repository
    /// performs yet". Every one of those clauses is now false, and they are quoted rather than
    /// deleted because a surface constant whose documentation understates what the build does is
    /// the failure this record exists against, read in the direction nobody checks.)</i>
    /// </para>
    /// <para>
    /// <b>What is true instead.</b> An arming path exists in exactly one place, three backends
    /// encode - two for <c>x86-64</c> and one for <c>arm64</c> - and the second refusal is
    /// performed: an artifact whose payload names an architecture this host cannot arm is refused
    /// at instantiation rather than run. So the two questions really are two, and an image answers
    /// them separately: this surface asks whether a composition admits an executable payload at
    /// all, and the architecture in the payload asks whether this host is one that can arm it.
    /// <b>The <c>arm64</c> backend is where the difference is visible</b>, because it emits and is
    /// never armed anywhere - it is emitting-only, and an artifact carrying its bytes verifies and
    /// refuses to instantiate.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=756E6F
    // Broiler-Human:        PENDING
    public const string Native = "broiler.javascript.native";

    /// <summary>
    /// The BigInt surface: BigInt values, their literals, and the <c>BigInt</c> global.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It was a gate and is now an advertised surface</b> (decision JSD-0033, section 7; JSeal
    /// card B05). Until B05 the realm had an internal BigInt value kind that no public compilation
    /// could produce, and this name was known and deliberately absent from <see cref="All"/>. B05
    /// completed the language's BigInt surface - conversion, comparison, the global and its
    /// prototype, wrappers and JSON - and admitted it here, so the descriptor admitting every
    /// surface admits it and a composition may still decline it by name.
    /// </para>
    /// <para>
    /// <b>It is declared by a CONSTANT and by a GLOBAL.</b> The lowering declares it where it writes
    /// a <see cref="JsFormat.ConstantTag.BigInt"/> constant, and a program reading the
    /// <c>BigInt</c> global declares it the way a program reading <c>Uint8Array</c> declares the
    /// binary surface (<see cref="BigIntGlobals"/>). A composition that declines it refuses both at
    /// verification and builds no <c>BigInt</c> global, so no BigInt value can arise in its realms.
    /// An update expression declares nothing: it converts with <c>ToNumeric</c> whatever the
    /// program holds, and a Number takes the path it always took.
    /// </para>
    /// <para>
    /// <i>(Amended 2026-09-21, B05. As the gate this paragraph read "No shipped composition admits
    /// it, and it is deliberately not in All"; that stopped being true when the surface was
    /// admitted, and is quoted rather than deleted.)</i>
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=415303
    // Broiler-Human:        PENDING
    public const string BigInt = "broiler.javascript.bigint";

    /// <summary>Every optional surface this build knows, in ascending ordinal order.</summary>
    /// <remarks>
    /// An artifact declaring a name that is not here is refused as naming a surface this build does
    /// not implement, which is a different failure from naming one the composition declined and
    /// carries a different diagnostic.
    /// <i>(Amended 2026-09-21. <see cref="BigInt"/> was for a while known and not here, so that the
    /// descriptor admitting every surface declined it; card B05 admitted it, and it is here.)</i>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=622B1E
    // Broiler-Human:        PENDING
    public static readonly string[] All = [BigInt, Binary, Dynamic, Modules, Native];

    /// <summary>
    /// The global names the binary surface owns, in ascending ordinal order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The twelve typed array constructors. <c>BigInt64Array</c> and <c>BigUint64Array</c> joined
    /// on 2026-09-22 (JSeal B07) and are on <see cref="BigIntGlobals"/> as well, because their
    /// elements are BigInt values: a program naming one declares BOTH surfaces, so a composition
    /// declining either refuses it at verification rather than running it into an absent global.
    /// <see cref="TryOwner"/> answers this surface for them, and the lowering adds the BigInt
    /// surface beside it.
    /// </para>
    /// <para>
    /// <i>(Amended 2026-09-22. The list held the ten Number kinds only, with the note that the two
    /// BigInt constructors were card B07's and "not on this list because a program naming one is
    /// naming a global this surface does not have"; that stopped being true with B07.)</i>
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C3506E
    // Broiler-Human:        PENDING
    public static readonly string[] BinaryGlobals =
    [
        "ArrayBuffer",
        "BigInt64Array",
        "BigUint64Array",
        "DataView",
        "Float16Array",
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

    /// <summary>The global names the BigInt surface owns.</summary>
    /// <remarks>
    /// <para>
    /// A program that reads <c>BigInt</c> wants BigInt values - it can make one from a Number or a
    /// String without writing a literal - so the name is a declaration, as <c>eval</c> is one for
    /// the dynamic surface. A <c>typeof BigInt</c> stays a question, as every <c>typeof</c> does.
    /// </para>
    /// <para>
    /// <c>BigInt64Array</c> and <c>BigUint64Array</c> are here too (since 2026-09-22, JSeal B07):
    /// reading one of their elements makes a BigInt value, so naming either constructor declares
    /// this surface as well as the binary one, on which they are also listed.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=807DA6
    // Broiler-Human:        PENDING
    public static readonly string[] BigIntGlobals = ["BigInt", "BigInt64Array", "BigUint64Array"];

    /// <summary>
    /// The surface that owns <paramref name="globalName"/>, or <see langword="false"/> when the
    /// name belongs to no optional surface.
    /// </summary>
    /// <remarks>
    /// Two names belong to two surfaces - <c>BigInt64Array</c> and <c>BigUint64Array</c>, on both
    /// <see cref="BinaryGlobals"/> and <see cref="BigIntGlobals"/> - and this answers the binary
    /// surface for them, the first list it searches; a caller that records declarations reads
    /// <see cref="BigIntGlobals"/> as well. (Added 2026-09-22, JSeal B07.)
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=338E4D
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

        foreach (var name in BigIntGlobals)
        {
            if (string.Equals(name, globalName, System.StringComparison.Ordinal))
            {
                manifestId = BigInt;
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
