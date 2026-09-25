// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           3
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    Critical
// Criteria:         7/7
// Resource impact:  2/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One value-form instance's slab of words: every value-form frame's region, each opened by a header
/// that states its length and how many of its words are live (JSD-0035 section 3).
/// </summary>
/// <remarks>
/// <para>
/// <b>THE SLAB HOLDS NO MANAGED REFERENCE, SO IT CAN BE PINNED AND HANDED TO EMITTED CODE.</b> A word is
/// a Number, a special constant, a handle - an index and a generation, not an address - or a frame
/// header, so the collector has nothing to trace in it and nothing it could move. Every referenced
/// value is rooted by the handle table instead, and the slab is what decides which of the table's
/// entries are still reachable.
/// </para>
/// <para>
/// <b>THE ARRAY IS LONGER THAN THE CAPACITY IT ADVERTISES</b>, by the format's ceiling on call
/// arguments. A caller writes a call's arguments into the region a callee is about to take before the
/// callee checks that the region fits, and the numeric form's slab lacked this headroom until a fix
/// dated 2026-09-23. This slab carries it from its first line.
/// </para>
/// <para>
/// <b>Frames are pushed and popped in order, and nothing else moves the top.</b> A value-form instance
/// holds a chain of these, <see cref="JsValueStack"/>, and a frame lives inside one of them; at stage
/// JSV-1 only managed code pushes, pops, publishes and scans, because the emitted code reads nothing
/// but its helper table.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=778523
// Broiler-Falsified-If: a live word of some frame is not visited by Scan, or a word past a frame's published live length or past the top is visited
// Broiler-Human:        PENDING
internal sealed class JsValueSlab : IJsWordRoots
{
    /// <summary>How many words the array carries past the capacity it advertises.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=02A8CF
    // Broiler-Falsified-If: a call's argument stores for a region that does not fit can reach past the end of the array
    // Broiler-Human:        PENDING
    internal const int Headroom = (int)JsFormat.CeilingCallArguments;

    /// <summary>Creates a slab that admits frames up to <paramref name="capacity"/> words in all.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7F06F5
    // Broiler-Human:        PENDING
    internal JsValueSlab(int capacity)
    {
        if (capacity is < 1 or > int.MaxValue - Headroom)
        {
            throw new System.ArgumentOutOfRangeException(nameof(capacity));
        }

        Capacity = capacity;
        Words = System.GC.AllocateArray<ulong>(capacity + Headroom, pinned: true);
    }

    /// <summary>The words, pinned for the slab's lifetime.</summary>
    internal ulong[] Words { get; }

    /// <summary>How many words frames may occupy in all, headers included.</summary>
    internal int Capacity { get; }

    /// <summary>The first word no frame occupies.</summary>
    internal int Top { get; private set; }

    /// <summary>Opens a frame of <paramref name="region"/> words with none of them live.</summary>
    /// <remarks>
    /// The region's words are left as they are: whatever an earlier frame wrote there is dead until a
    /// word is stored and published, and <see cref="Scan"/> never reads past the published length.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=408AE9
    // Broiler-Falsified-If: a frame is opened whose header and region together reach past the advertised capacity
    // Broiler-Human:        PENDING
    internal bool TryPushFrame(int region, out int frame)
    {
        frame = Top;

        if (region is < 0 or > JsWord.MaximumFrameLength || (long)Top + 1 + region > Capacity)
        {
            return false;
        }

        Words[frame] = JsWord.Header(region, 0);
        Top = frame + 1 + region;
        return true;
    }

    /// <summary>Publishes how many of a frame's words, from its first, are live.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=276FB2
    // Broiler-Falsified-If: a live length larger than the frame's region is published
    // Broiler-Human:        PENDING
    internal void Publish(int frame, int live)
    {
        var header = HeaderAt(frame);
        Words[frame] = JsWord.Header(JsWord.HeaderRegion(header), live);
    }

    /// <summary>Closes the innermost frame, which must be <paramref name="frame"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=F0AF85
    // Broiler-Falsified-If: a frame other than the innermost one is closed, or the top moves anywhere but to that frame's header
    // Broiler-Human:        PENDING
    internal void PopFrame(int frame)
    {
        var header = HeaderAt(frame);

        if (frame + 1 + JsWord.HeaderRegion(header) != Top)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a value-form frame was closed while a frame inside it was open");
        }

        Top = frame;
    }

    /// <summary>The index of a frame's first region word.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=66FE6A
    // Broiler-Human:        PENDING
    internal static int FirstWord(int frame) => frame + 1;

    /// <summary>The live length a frame's header states.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=120BE6
    // Broiler-Human:        PENDING
    internal int Live(int frame) => JsWord.HeaderLive(HeaderAt(frame));

    /// <summary>The region length a frame's header states.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=53872B
    // Broiler-Human:        PENDING
    internal int Region(int frame) => JsWord.HeaderRegion(HeaderAt(frame));

    /// <summary>Visits every live word of every frame, walking from the base by the headers' lengths.</summary>
    /// <remarks>
    /// <b>A MALFORMED SLAB STOPS THE SCAN, AND IT NEVER GUESSES.</b> A position that should hold a header
    /// and does not, a header stating more live words than its region, or a region running past the top
    /// is an internal defect: a scan that carried on would root some set of words it could not name,
    /// and the table would then free an entry some live word still names.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=3A33CD
    // Broiler-Falsified-If: a scan completes over a slab whose frame chain does not end exactly at the top, or skips a published live word of any frame
    // Broiler-Human:        PENDING
    public void Scan(JsHandleTable table)
    {
        var at = 0;

        while (at < Top)
        {
            var header = Words[at];

            if (!JsWord.IsHeader(header))
            {
                throw new JsAbort(
                    JsAbortKind.InternalDefect, "the value slab holds no frame header where one must be");
            }

            var region = JsWord.HeaderRegion(header);
            var live = JsWord.HeaderLive(header);

            if (live > region || (long)at + 1 + region > Top)
            {
                throw new JsAbort(
                    JsAbortKind.InternalDefect, "a value-form frame header states a length its slab does not hold");
            }

            for (var word = at + 1; word < at + 1 + live; word++)
            {
                table.MarkRoot(Words[word]);
            }

            at += 1 + region;
        }
    }

    /// <summary>The header of <paramref name="frame"/>, or a defect when it holds none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=CC2D9B
    // Broiler-Falsified-If: a word that is not a frame header is answered as one
    // Broiler-Human:        PENDING
    private ulong HeaderAt(int frame)
    {
        if ((uint)frame >= (uint)Top || !JsWord.IsHeader(Words[frame]))
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "a value-form frame names no header");
        }

        return Words[frame];
    }
}
