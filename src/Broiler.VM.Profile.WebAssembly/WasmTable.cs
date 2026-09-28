// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           1
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  2/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// One table: a vector of function indices, where a slot holding
/// <see cref="NullFunctionReference"/> is the null reference.
/// </summary>
/// <remarks>
/// <para>
/// A funcref is held as an index rather than as a reference because this build admits no reference
/// value on an operand stack: the only instruction that reads a table is <c>call_indirect</c>, which
/// wants a function to call. The moment reference types open, this becomes a reference array and
/// the index form stops being enough - which is why the type is here rather than being a bare
/// <c>int[]</c> the family's handler indexes.
/// </para>
/// <para>
/// A function index is a unit index of the translated artifact, because the translator writes one
/// unit per defined function in function-index order. The table is the family's instance state's, and
/// the store it was once part of went with the bare-module executor at milestone UBC-4.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=6C840E
// Broiler-Falsified-If: a slot is read without its index being compared against the entry count first
// Broiler-Human:        PENDING
internal sealed class WasmTableInstance
{
    /// <summary>The value a slot holds until an element segment writes a function into it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0C3E2A
    // Broiler-Human:        PENDING
    internal const int NullFunctionReference = -1;

    /// <summary>
    /// The largest table this profile will allocate, declared here for the same reason the page
    /// ceiling is declared on the memory.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0F97A2
    // Broiler-Falsified-If: this bound is derived from a limit vector rather than declared here
    // Broiler-Human:        PENDING
    internal const uint ProfileMaximumTableEntries = 1_048_576;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=877C3D
    // Broiler-Human:        PENDING
    private readonly int[] entries;

    /// <summary>Takes an already-charged and already-nulled entry vector.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F6D7CC
    // Broiler-Human:        PENDING
    internal WasmTableInstance(int[] initial) => entries = initial;

    /// <summary>How many entries the table holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A47191
    // Broiler-Human:        PENDING
    internal int EntryCount => entries.Length;

    /// <summary>Reads one entry, or refuses because the index is outside the table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=895FB1
    // Broiler-Falsified-If: an index at or past the entry count is read rather than refused
    // Broiler-Human:        PENDING
    internal bool TryRead(uint index, out int functionIndex)
    {
        functionIndex = NullFunctionReference;

        if (index >= (uint)entries.Length)
        {
            return false;
        }

        functionIndex = entries[(int)index];
        return true;
    }

    /// <summary>
    /// Writes an element segment's entries in, after checking the whole segment fits.
    /// </summary>
    /// <remarks>
    /// Atomic per segment for the same reason a data segment is: the range is checked whole before
    /// one entry is written, and a segment already applied stays applied when a later one is
    /// refused.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=C8FAE1
    // Broiler-Falsified-If: a prefix of a segment that does not fit is written
    // Broiler-Human:        PENDING
    internal bool TryInitialise(ulong offset, System.ReadOnlySpan<uint> functionIndices)
    {
        if (offset + (ulong)functionIndices.Length > (ulong)entries.Length)
        {
            return false;
        }

        for (var index = 0; index < functionIndices.Length; index++)
        {
            entries[(int)offset + index] = (int)functionIndices[index];
        }

        return true;
    }
}
