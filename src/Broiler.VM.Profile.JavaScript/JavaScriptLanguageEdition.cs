// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           0
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The language-specification edition this profile's manifests are defined against, pinned by an
/// immutable revision identifier and a digest.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because a manifest defined against a moving document is not defined.</b> The
/// roadmap's ledger has carried "the language-specification edition is not pinned" as an open
/// dependency since JS-0, with the consequence stated in its own words: no manifest may be
/// accepted against an unpinned edition, because a conformance total against a moving document is
/// not a total. What was missing was not an intention - it was a revision somebody had actually
/// retrieved, because recording an edition nobody has retrieved would be a pin in name only.
/// </para>
/// <para>
/// <b>The pin is a commit and not an edition name, and that is the whole point of the field
/// below.</b> "ES2026" names a document; <see cref="Revision"/> names one immutable state of it.
/// The tag is recorded too, as how the commit was found rather than as what is pinned: a tag can
/// be moved and a commit cannot.
/// </para>
/// <para>
/// <b><see cref="Archived"/> is false and says so on every run that prints this.</b> Retrieving,
/// hashing and archiving a third-party document is a human action, and two of those three have
/// been done. The pin is therefore PROVISIONAL in the sense roadmap section 24 defines: it carries
/// a named exclusion in the ledger, with a holder and an unblock condition, until someone archives
/// the document. A field is used rather than a paragraph so that a run states the provisionality
/// rather than a reader having to go and look it up.
/// </para>
/// <para>
/// <b>What a pinned edition does not do.</b> It does not tell the conformance harness which
/// feature flags of a third-party suite name constructs of this edition: flags do not map onto
/// clauses mechanically, so that filter still reads the suite's own proposed-and-standard split
/// and the two authorities can disagree. One disagreement is known and measured -
/// <c>regexp-duplicate-named-groups</c> is in this edition and the pinned checkout still calls it
/// a proposal - and it moves no figure, because no case claiming it was ever scorable here.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D72744
// Broiler-Human:        PENDING
public static class JavaScriptLanguageEdition
{
    /// <summary>The standard, by its ECMA number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7A637C
    // Broiler-Human:        PENDING
    public const string Standard = "ECMA-262";

    /// <summary>Which edition of it, as an ordinal.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A67BA4
    // Broiler-Human:        PENDING
    public const int Edition = 17;

    /// <summary>The year the edition is known by.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C05E14
    // Broiler-Human:        PENDING
    public const string Year = "ES2026";

    /// <summary>Where the pinned revision was retrieved from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=278DCB
    // Broiler-Human:        PENDING
    public const string Source = "tc39/ecma262";

    /// <summary>The tag the revision was found by, which is not what is pinned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B9DF1A
    // Broiler-Human:        PENDING
    public const string Tag = "es2026";

    /// <summary>The immutable revision identifier: the commit the tag resolved to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4E055C
    // Broiler-Human:        PENDING
    public const string Revision = "0248456c758431e4bb8e5d26333ff1865123c9cd";

    /// <summary>The document the digest is over.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=73D3EC
    // Broiler-Human:        PENDING
    public const string Document = "spec.html";

    /// <summary>Its length in bytes, so a truncated retrieval is visible before the digest is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E84184
    // Broiler-Human:        PENDING
    public const int DocumentBytes = 2978793;

    /// <summary>SHA-256 over that document, as retrieved at that revision.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=10F075
    // Broiler-Human:        PENDING
    public const string DocumentDigest =
        "ce7bc30174061fd8d212270b81cf6511661180c1e174f6911d10ced0581527b0";

    /// <summary>
    /// Whether a human has archived the document, which is the third of the three actions roadmap
    /// section 24 asks for and the one nobody has performed.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AE2ABC
    // Broiler-Human:        PENDING
    public const bool Archived = false;

    /// <summary>The pin on one line, in the shape a run prints it.</summary>
    /// <remarks>
    /// It names the provisionality rather than leaving it to a reader who might not look. A run
    /// that printed the edition and not the missing archive would be a run implying more than it
    /// has.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2DE130
    // Broiler-Human:        PENDING
    public static string Describe() =>
        $"{Standard} {Year} ({Source}@{Revision}, {Document} sha256 {DocumentDigest})" +
        (Archived ? string.Empty : " - retrieved and hashed, NOT archived");
}
