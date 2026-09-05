// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           5
// Human-reviewed:   0/5
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The realm's global LEXICAL environment: the bindings a script-level <c>let</c>, <c>const</c> or
/// <c>class</c> makes, which are not properties of the global object.
/// </summary>
/// <remarks>
/// <para>
/// <b>The global environment record has two halves and only one of them is the global object.</b>
/// A <c>var</c> and a function declaration create a PROPERTY: <c>globalThis</c> shows it, a read
/// before the declaration answers <c>undefined</c>, and the property is what a later script sees. A
/// <c>let</c>, a <c>const</c> and a <c>class</c> create a BINDING in the declarative half beside
/// that object, and every one of those three sentences is false of it — which is why this is a
/// table of its own and not a set of properties with a flag.
/// </para>
/// <para>
/// <b>It belongs to the realm because the bindings outlive the script that declared them.</b> The
/// alternative, a slot in the script's own frame, is what this profile does for every other
/// lexical binding and it cannot work here: a conformance run evaluates its harness files as
/// separate scripts in one realm, several of them publish a helper with <c>const</c>, and a binding
/// in the declaring frame would be gone before the test that reads it ran.
/// </para>
/// <para>
/// <b>A declaration that meets an existing binding replaces it rather than refusing.</b> The
/// language makes a re-declaration a <c>SyntaxError</c> raised before the script runs, and refusing
/// here would be the same answer at the same moment — except for one caller it would be wrong for:
/// evaluated source reaches this through the same program lowering a script does, and the language
/// gives eval code a lexical environment of its own that is discarded afterwards, so a second
/// <c>(0, eval)("let x = 1")</c> is a program and not an error. Replacing is right for that caller
/// and lenient for the other; refusing is right for one and wrong for the other, and the
/// divergence this leaves is recorded rather than hidden.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>The bindings, by name, in no order a program can observe.</summary>
    /// <remarks>
    /// Allocated once and usually empty: a program with no script-level lexical declaration pays
    /// one <c>Count</c> comparison per global reference and nothing else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=408357
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, JsLexicalBinding> lexicals = [];

    /// <summary>Whether any script-level lexical binding exists at all.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7539CD
    // Broiler-Human:        PENDING
    internal bool HasLexicals => lexicals.Count != 0;

    /// <summary>Creates or replaces an uninitialised binding.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=308072
    // Broiler-Human:        PENDING
    internal void DeclareLexical(string name, bool mutable) =>
        lexicals[name] = new JsLexicalBinding(mutable);

    /// <summary>The binding <paramref name="name"/> stands for, if this half has one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=B22AF3
    // Broiler-Human:        PENDING
    internal bool TryLexical(string name, out JsLexicalBinding binding) =>
        lexicals.TryGetValue(name, out binding!);
}

/// <summary>One binding of the global lexical environment.</summary>
/// <remarks>
/// <b>The uninitialised state is the temporal dead zone and it is a state rather than a value.</b> A
/// sentinel value would have been reachable: <c>undefined</c> is what an uninitialised <c>let</c>
/// takes when its declaration has no initialiser, so a binding holding it is initialised and one
/// that has never been reached is not, and no value tells the two apart.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=AC224D
// Broiler-Human:        PENDING
internal sealed class JsLexicalBinding
{
    /// <summary>Creates the binding a declaration makes, before its initialiser has run.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=FBD27E
    // Broiler-Human:        PENDING
    internal JsLexicalBinding(bool mutable) => Mutable = mutable;

    /// <summary>Whether an assignment to this binding is admitted at all.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A37B05
    // Broiler-Human:        PENDING
    internal bool Mutable { get; }

    /// <summary>Whether the declaration this binding belongs to has been reached.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=2C1C18
    // Broiler-Human:        PENDING
    internal bool Initialised { get; set; }

    /// <summary>What the binding holds once it has been initialised.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1A2922
    // Broiler-Human:        PENDING
    internal JsValue Value { get; set; }
}
