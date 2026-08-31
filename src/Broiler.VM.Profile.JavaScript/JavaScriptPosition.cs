// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           4
// Human-reviewed:   0/6
// IP risk:          None
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  2/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>One row of the canonical position table: a code offset and the source it came from.</summary>
/// <remarks>
/// The table is canonical against bytecode offsets rather than against any later specialization,
/// and the verifier refuses a table whose offsets are not strictly ascending - so the row covering
/// an offset is the last one at or before it, and there is exactly one.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9838A1
// Broiler-Human:        PENDING
internal readonly struct JavaScriptPositionRow
{
    /// <summary>Binds a code offset to a line and a column.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=001D8E
    // Broiler-Human:        PENDING
    internal JavaScriptPositionRow(uint offset, int line, int column)
    {
        Offset = offset;
        Line = line;
        Column = column;
    }

    /// <summary>The offset into the code section this row is canonical for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1184BD
    // Broiler-Human:        PENDING
    internal uint Offset { get; }

    /// <summary>The one-based source line.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=BD1738
    // Broiler-Human:        PENDING
    internal int Line { get; }

    /// <summary>The one-based source column.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CDBAA3
    // Broiler-Human:        PENDING
    internal int Column { get; }
}

/// <summary>
/// This profile's use of the core's four-field position record, decided at JS-3a and published in
/// <c>docs/diagnostics/registry.txt</c>.
/// </summary>
/// <remarks>
/// <para>
/// The core stores the record and never parses, orders, formats or compares it, and two of its
/// four fields are explicitly the profile's own. That is exactly the shape in which two profiles
/// build two incompatible conventions against one struct without either of them ever writing the
/// convention down. Decision JSD-0009 writes this one down, and it has four clauses:
/// </para>
/// <para>
/// <b>The section index is populated and it means what the core says it means.</b> A position
/// inside a framed section body carries that section's ordinal index in the artifact's own section
/// sequence - the index of the frame, not the section KIND, because a kind is not an ordinal and
/// an artifact may omit a section this format defines. A position that is an offset into the
/// artifact's byte stream rather than into a section body carries <c>-1</c>. Every refusal the
/// bounded read produces is of the second kind, because the reader is part-way through the framing
/// when it stops and there is no frame to name.
/// </para>
/// <para>
/// <b>The byte offset is always populated, and what it is an offset INTO is what the section index
/// says.</b> Artifact-relative when the section index is <c>-1</c>, section-body-relative
/// otherwise. Reporting a code-section offset with a section index of <c>-1</c> - which this
/// verifier did before JS-3a - is the conflation this clause exists to forbid: the number was
/// right and the frame it named was wrong, so a consumer resolving it against the artifact would
/// land on an unrelated byte.
/// </para>
/// <para>
/// <b>The two profile-owned coordinates carry a one-based line and a one-based column</b> read out
/// of the canonical position table, and <b>zero in both means the position is not known</b>. Zero
/// is reserved for exactly that: the verifier refuses a table row declaring line or column zero, so
/// an artifact cannot mint an unknown-looking position that a consumer would then trust. An
/// artifact that carries no covering row for an offset reports zero and says so honestly rather
/// than reporting the nearest row it could find.
/// </para>
/// <para>
/// <b>A position is constructed here and nowhere else.</b> Rule N9 holds every
/// <c>VmSourcePosition</c> construction in this assembly to these two factories, because a
/// convention that one call site can bypass is a convention that one call site will bypass.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=908BBE
// Broiler-Human:        PENDING
internal static class JavaScriptPosition
{
    /// <summary>The section index of a position that is an offset into the artifact itself.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6E21DB
    // Broiler-Human:        PENDING
    internal const int OutsideAnySection = -1;

    /// <summary>The coordinate value that means "not known", in both coordinates together.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6A6DD5
    // Broiler-Human:        PENDING
    internal const int Unknown = 0;

    /// <summary>A position that is an offset into the artifact's byte stream.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=3207E1
    // Broiler-Human:        PENDING
    internal static VmSourcePosition InArtifact(ulong byteOffset) =>
        new(OutsideAnySection, byteOffset, Unknown, Unknown);

    /// <summary>
    /// A position inside the code section, carrying the line and column of the row that covers it.
    /// </summary>
    /// <remarks>
    /// The scan is linear over a strictly ascending table and stops at the first row past the
    /// offset. A binary search would be the same answer in fewer steps and is not written here:
    /// this runs once, on the refusal path, after a verification has already failed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=36B11E
    // Broiler-Falsified-If: an offset with no row at or before it reports a line other than zero, or an offset with two candidate rows reports the earlier one
    // Broiler-Human:        PENDING
    internal static VmSourcePosition InCode(
        int codeSectionIndex,
        ulong codeOffset,
        System.Collections.Generic.List<JavaScriptPositionRow> rows)
    {
        var line = Unknown;
        var column = Unknown;

        foreach (var row in rows)
        {
            if (row.Offset > codeOffset)
            {
                break;
            }

            line = row.Line;
            column = row.Column;
        }

        return new(codeSectionIndex, codeOffset, line, column);
    }
}
