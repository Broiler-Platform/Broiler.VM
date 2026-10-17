// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           3
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using System;

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>The three segmentations <c>Intl.Segmenter</c> offers (ECMA-402 s19).</summary>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s19; IP=Low; Security=Low; Resources=1; Fingerprint=974B87
// Broiler-Human:        PENDING
public enum JsSegmentGranularity
{
    /// <summary>Extended grapheme clusters.</summary>
    Grapheme = 0,

    /// <summary>Words.</summary>
    Word = 1,

    /// <summary>Sentences.</summary>
    Sentence = 2,
}

/// <summary>
/// UAX #29's default text segmentation, which a composition admitting <see cref="JsSurfaces.Intl"/>
/// hands to the profile with its data: the Intl data's <see cref="IJsIntlData"/> implements it too
/// (JSD-0074; milestone SJL-7.4).
/// </summary>
/// <remarks>
/// <para>
/// <b>An interface, because segmentation is optional.</b> Its algorithm is the Unicode area's owner's
/// optional segmentation package, which only the Intl data assembly references, so that a composition
/// that declines the surface carries none of it (JSD-0074's X5, held by rule I3). The profile reaches it
/// here, as it reaches the data through <see cref="IJsIntlData"/>, and references neither the data nor the
/// package. An embedder that supplies data of its own implements this beside it for
/// <c>Intl.Segmenter</c>; the profile refuses a segmentation its data does not offer.
/// </para>
/// <para>
/// <b>The meter is the caller's.</b> Each method charges one unit per code point it classifies or reads,
/// before reading it, through <c>charge</c>, which may throw to stop it (JSD-0074, decision 4, which
/// corrects the profile's own segmenter's row G5).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s19; IP=Low; Security=Medium; Resources=2; Fingerprint=DCF242
// Broiler-Human:        PENDING
public interface IJsTextSegmentation
{
    /// <summary>
    /// The code unit indices at which a segment of <paramref name="granularity"/> begins in
    /// <paramref name="text"/>, with the text's length last: UAX #29's default boundaries, with no
    /// locale tailoring and no dictionary. An empty text has the one boundary 0.
    /// </summary>
    /// <param name="text">The text, read as code points; a lone surrogate is one.</param>
    /// <param name="granularity">The segmentation.</param>
    /// <param name="charge">The caller's meter, or null.</param>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s19; IP=Low; Security=Medium; Resources=2; Fingerprint=00062A
    // Broiler-Human:        PENDING
    int[] Boundaries(ReadOnlySpan<char> text, JsSegmentGranularity granularity, Action<ulong>? charge);

    /// <summary>
    /// Whether a word segment is word-like (ECMA-402's <c>isWordLike</c>): it holds a letter, a number,
    /// Katakana, an ideograph or Hiragana.
    /// </summary>
    /// <param name="segment">One word segment.</param>
    /// <param name="charge">The caller's meter, or null.</param>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s19; IP=Low; Security=Medium; Resources=2; Fingerprint=F5B027
    // Broiler-Human:        PENDING
    bool IsWordLike(ReadOnlySpan<char> segment, Action<ulong>? charge);
}
