// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           0
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       13
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
/// <b><see cref="Archived"/> was false for as long as it was true that nobody had archived the
/// document, and a run said so on every line it printed.</b> All three of section 24's actions -
/// retrieve, hash, archive - are done as of 2026-09-03, and the document is in this repository at
/// <see cref="ArchivedAt"/> with the digest below as the check. A field is used rather than a
/// paragraph because the state had to be able to change, and this is the change: a run that keeps
/// printing a provisionality that has passed is as wrong as one that never printed it.
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
    /// Whether the document has been archived, which is the third of the three actions roadmap
    /// section 24 asks for. True since 2026-09-03.
    /// </summary>
    /// <remarks>
    /// <b>Archiving is what makes the digest checkable without a network.</b> A pin whose document
    /// lives only at a URL depends on somebody else's uptime and somebody else's history; the
    /// archived copy is verifiable in a checkout that has neither.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=607149
    // Broiler-Human:        PENDING
    public const bool Archived = true;

    /// <summary>Where the archived document is retained, relative to the repository root.</summary>
    /// <remarks>
    /// <b>Named here rather than left to a reader to find</b>, because a run that says a document
    /// is archived and does not say where has moved the search rather than ended it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BB3644
    // Broiler-Human:        PENDING
    public const string ArchivedAt =
        "src/Broiler.VM.Profile.JavaScript/docs/specification/ecma-262-es2026-spec.html";

    /// <summary>The pin on one line, in the shape a run prints it.</summary>
    /// <remarks>
    /// <b>It names the archive's state, and did so in both states.</b> While nothing was archived
    /// it said so, because a run printing an edition and not the missing archive implies more than
    /// it has; now it names the retained path, because a run saying a document is archived and not
    /// saying where has moved the search rather than ended it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4D1AF4
    // Broiler-Human:        PENDING
    public static string Describe() =>
        $"{Standard} {Year} ({Source}@{Revision}, {Document} sha256 {DocumentDigest})" +
        (Archived ? " - archived at " + ArchivedAt : " - retrieved and hashed, NOT archived");
}
