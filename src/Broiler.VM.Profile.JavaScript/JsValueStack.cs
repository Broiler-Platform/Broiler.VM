// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           3
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Critical
// Criteria:         7/7
// Resource impact:  3/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>Where a handle table's compaction finds the live words that root its entries.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=DD2034
// Broiler-Falsified-If: an implementation's Scan passes over a published live word of some frame it holds
// Broiler-Human:        PENDING
internal interface IJsWordRoots
{
    /// <summary>Marks, in <paramref name="table"/>, every live word of every frame held here.</summary>
    /// <param name="table">The table whose entries the words name.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=6B1144
    // Broiler-Falsified-If: a published live word of a frame held here is not marked
    // Broiler-Human:        PENDING
    void Scan(JsHandleTable table);
}

/// <summary>
/// One value-form instance's words: a chain of pinned slabs, each holding whole frames, pushed and
/// popped in call order (JSD-0035 section 3).
/// </summary>
/// <remarks>
/// <para>
/// <b>A SLAB NEVER MOVES, SO THE CHAIN GROWS BY ADDING ONE RATHER THAN BY COPYING.</b> A frame's region
/// is inside one segment, and a region that does not fit the current segment opens the next, which is
/// allocated when there is none big enough. So a word's address is fixed for its frame's life, which is
/// what lets a later stage hand emitted code a region address, and an instance that never recurses
/// deeply pays for one small segment.
/// </para>
/// <para>
/// <b>ONE SPARE SEGMENT IS KEPT ABOVE THE CURRENT ONE</b>, so a recursion that oscillates across a
/// segment boundary reuses the segment it just left rather than allocating one per crossing; any
/// segment above the spare is dropped when the chain shrinks back past it.
/// </para>
/// <para>
/// <b>THE CHAIN IS BOUNDED, AND A PUSH PAST THE BOUND IS REFUSED RATHER THAN ALLOCATED.</b> The engine
/// answers a refused push with the call-depth backstop, the answer the interpreter gives when the
/// machine stack runs out, so a guest cannot make an instance allocate words without limit; the bound
/// is far above what the call-depth ceiling lets a verified program reach with frames of the operand
/// stack's ceiling.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=671496
// Broiler-Falsified-If: a frame spans two segments, a segment holding a live frame is dropped or reused, a frame is popped out of call order without a defect, or Scan passes over a live word of any open frame
// Broiler-Human:        PENDING
internal sealed class JsValueStack : IJsWordRoots
{
    /// <summary>The words a segment advertises when no region needs more.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9178F2
    // Broiler-Human:        PENDING
    internal const int SegmentWords = 8192;

    /// <summary>The most words every segment together may advertise.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=02C436
    // Broiler-Falsified-If: the chain's segments together advertise more words than this
    // Broiler-Human:        PENDING
    internal const long MaximumWords = 1L << 25;

    private readonly System.Collections.Generic.List<JsValueSlab> segments = [new JsValueSlab(SegmentWords)];
    private int current;
    private long advertised = SegmentWords;

    /// <summary>How many segments the chain holds, the spare included.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=85FDCB
    // Broiler-Human:        PENDING
    internal int SegmentCount => segments.Count;

    /// <summary>Opens a frame of <paramref name="region"/> words, in the current segment or the next.</summary>
    /// <param name="region">How many words the frame's region holds, its header not included.</param>
    /// <param name="segment">The segment the frame is in, when the answer is true.</param>
    /// <param name="frame">The index of the frame's header in that segment, when the answer is true.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=D8C559
    // Broiler-Falsified-If: a frame is opened across two segments, in a segment below one holding an open frame, or past MaximumWords advertised in all
    // Broiler-Human:        PENDING
    internal bool TryPush(int region, out JsValueSlab segment, out int frame)
    {
        segment = segments[current];

        if (segment.TryPushFrame(region, out frame))
        {
            return true;
        }

        if (region is < 0 or > Format.JsWord.MaximumFrameLength)
        {
            return false;
        }

        var next = current + 1;
        var needed = System.Math.Max(SegmentWords, region + 1);

        if (next >= segments.Count || segments[next].Capacity < needed)
        {
            // THE SPARE IS REPLACED WHEN IT IS TOO SMALL, and it is empty whenever this runs, because a
            // segment above the current one holds no open frame.
            if (next < segments.Count)
            {
                advertised -= segments[next].Capacity;
                segments.RemoveRange(next, segments.Count - next);
            }

            if (advertised + needed > MaximumWords)
            {
                return false;
            }

            segments.Add(new JsValueSlab(needed));
            advertised += needed;
        }

        current = next;
        segment = segments[current];
        return segment.TryPushFrame(region, out frame);
    }

    /// <summary>Closes the innermost frame, which must be <paramref name="frame"/> of <paramref name="segment"/>.</summary>
    /// <param name="segment">The segment the frame was opened in.</param>
    /// <param name="frame">The index of its header there.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=E84434
    // Broiler-Falsified-If: a frame of a segment other than the current one is closed without a defect, or the chain keeps more than one empty segment above the current one
    // Broiler-Human:        PENDING
    internal void Pop(JsValueSlab segment, int frame)
    {
        if (!ReferenceEquals(segment, segments[current]))
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a value-form frame was closed while a frame after it was open");
        }

        segment.PopFrame(frame);

        if (segment.Top != 0 || current == 0)
        {
            return;
        }

        current--;

        // Everything above the spare goes, so the chain holds at most one empty segment.
        var keep = current + 2;

        if (segments.Count > keep)
        {
            for (var index = keep; index < segments.Count; index++)
            {
                advertised -= segments[index].Capacity;
            }

            segments.RemoveRange(keep, segments.Count - keep);
        }
    }

    /// <summary>Visits every live word of every open frame, segment by segment from the base.</summary>
    /// <param name="table">The table the words are marked in.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=9B1956
    // Broiler-Falsified-If: a live word of an open frame in any segment up to the current one is not marked, or a segment above the current one is scanned
    // Broiler-Human:        PENDING
    public void Scan(JsHandleTable table)
    {
        for (var index = 0; index <= current; index++)
        {
            segments[index].Scan(table);
        }
    }
}
