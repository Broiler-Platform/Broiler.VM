// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           3
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    Low
// Criteria:         1/0
// Resource impact:  1/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.ECMAScript.Intl;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The composition's Intl data as the shared Intl library reads it: this family's
/// <see cref="IJsIntlData"/> answered as <c>Broiler.ECMAScript.Intl</c>'s <see cref="IIntlDataSet"/>
/// (JSD-0082, decision D8; milestone SJL-9.4).
/// </summary>
/// <remarks>
/// <para>
/// <b>Internal, so that no shared type enters the profile's public surface.</b>
/// <see cref="IJsIntlData"/>, <see cref="JsIntlTable"/> and <c>JsComposition.IntlData</c> stay this
/// family's public contract, which rule I6 and the API baseline hold, and the library's interface is
/// met here, inside the profile, by an adapter no embedder sees.
/// </para>
/// <para>
/// <b>The two enumerations share their numbers.</b> <see cref="IntlTable"/> keeps
/// <see cref="JsIntlTable"/>'s values, with 7, 23 and 24 retired in both, and appends values only for
/// tables this family's data never carries. A value <see cref="JsIntlTable"/> does not define is
/// such a table, and is answered empty, as decision D8 asks of an engine for a table it lacks;
/// an embedder's <see cref="IJsIntlData"/> is never asked for a number its contract does not name.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=JSD-0082 D8; IP=Low; Security=Low; Resources=1; Fingerprint=31C281
// Broiler-Human:        PENDING
internal sealed class JsIntlDataSet : IIntlDataSet
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=503B2D
    // Broiler-Human:        PENDING
    private readonly IJsIntlData data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=203A47
    // Broiler-Human:        PENDING
    internal JsIntlDataSet(IJsIntlData data) => this.data = data;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E80C9F
    // Broiler-Human:        PENDING
    public string CldrVersion => data.CldrVersion;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=JSD-0082 D8; IP=Low; Security=Low; Resources=1; Fingerprint=D6C23A
    // Broiler-Falsified-If: a table JsIntlTable names is answered with other bytes than the composition's data hands over, or a number it does not name reaches the composition's data
    // Broiler-Human:        PENDING
    public ReadOnlySpan<byte> Table(IntlTable table) =>
        table != IntlTable.None && Enum.IsDefined((JsIntlTable)(int)table) ? data.Table((JsIntlTable)(int)table) : default;
}
