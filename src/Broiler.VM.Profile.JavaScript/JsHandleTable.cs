// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           15
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    Critical
// Criteria:         13/13
// Resource impact:  3/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// One value-form instance's table of referenced values: a handle word is an index into it and a
/// generation, and an entry holds an ordinary managed reference the collector traces
/// (JSD-0035 section 4).
/// </summary>
/// <remarks>
/// <para>
/// <b>AN ENTRY IS REACHABLE WHEN SOMETHING THE TABLE CAN NAME STILL HOLDS ITS WORD, AND ONLY THEN.</b>
/// The roots are the live words of every frame in the instance's slab, the permanent entries
/// (constants, for the instance's life), and the <i>nursery</i>: every handle answered since the last
/// safepoint, which is how a helper's words stay rooted between being encoded and being stored. A
/// compaction marks from those roots and releases every other entry. Because every word says what it
/// is, the scan cannot mistake a Number for a handle.
/// </para>
/// <para>
/// <b>A RELEASED ENTRY CAN NEVER ANSWER FOR ANOTHER OBJECT.</b> Releasing an entry drops its reference
/// and advances its generation, and every resolution compares the word's generation, index and tag
/// with the entry's. A slot whose generation would wrap back to zero is retired rather than reused, and
/// generation zero is never issued, so a stale word is refused whatever happened to its slot since.
/// </para>
/// <para>
/// <b>ONE LIVE HANDLE PER OBJECT.</b> An object that already has an entry answers that entry's word, so
/// two words naming the same object are equal and the table grows with the number of distinct objects
/// reachable from the slab rather than with the number of encodings.
/// </para>
/// <para>
/// <b>HANDLE-STRESS COMPACTS AT EVERY SAFEPOINT.</b> Under it, a word the scan failed to root is released
/// at the next helper call and refused on its next use, by name, so a rooting mistake is a failing
/// check rather than a wrong answer on a rare run. That is the answer JSD-0035 gives to JSD-0011 Row 2's
/// objection that a rooting bug in a hand-written scheme is not a bug a corpus finds.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=1FF218
// Broiler-Falsified-If: TryResolve answers an object for a word whose entry was released since the word was issued, or a compaction releases an entry some live slab word, permanent entry or nursery handle names
// Broiler-Human:        PENDING
internal sealed class JsHandleTable
{
    /// <summary>The table's size below which an allocation grows it rather than compacting.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A1E5C0
    // Broiler-Human:        PENDING
    internal const int MinimumCompactionSize = 64;

    /// <summary>How many times the entries live after the last compaction a full table must hold before it compacts again.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FBDEE7
    // Broiler-Human:        PENDING
    internal const int GrowthFactor = 2;

    /// <summary>A slot that holds nothing and may be reissued.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=560BED
    // Broiler-Falsified-If: a slot in this state holds a value or is reissued while another slot answers its generation
    // Broiler-Human:        PENDING
    private const byte Free = 0;
    /// <summary>A slot that holds a value a compaction may release.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=008B26
    // Broiler-Falsified-If: a slot in this state is released while a root names it
    // Broiler-Human:        PENDING
    private const byte Live = 1;
    /// <summary>A slot that holds a value for the table's life.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=4F1233
    // Broiler-Falsified-If: a slot in this state is released before the table is
    // Broiler-Human:        PENDING
    private const byte Permanent = 2;
    /// <summary>A slot whose generation would have wrapped, never reissued.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=08FF61
    // Broiler-Falsified-If: a slot in this state is reissued
    // Broiler-Human:        PENDING
    private const byte Retired = 3;

    private readonly IJsWordRoots slab;
    private readonly System.Collections.Generic.Dictionary<object, int> byTarget =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    private readonly System.Collections.Generic.List<int> nursery = [];
    private object?[] targets;
    private ushort[] generations;
    private ushort[] tags;
    private byte[] states;
    private int[] free;
    private int freeCount;
    private int used;
    private int liveAfterCompaction;
    private bool[] marks = [];

    /// <summary>Creates the table of the instance whose slab - one segment, or its chain - is <paramref name="roots"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=622C5F
    // Broiler-Falsified-If: a table is created whose compactions scan any slab other than its own instance's
    // Broiler-Human:        PENDING
    internal JsHandleTable(IJsWordRoots roots, bool stress, int initialCapacity = 16)
    {
        slab = roots;
        Stress = stress;
        var capacity = System.Math.Max(1, initialCapacity);
        targets = new object?[capacity];
        generations = new ushort[capacity];
        tags = new ushort[capacity];
        states = new byte[capacity];
        free = new int[capacity];
    }

    /// <summary>Whether every safepoint compacts.</summary>
    internal bool Stress { get; }

    /// <summary>How many compactions have run.</summary>
    internal int Compactions { get; private set; }

    /// <summary>How many entries hold a value, permanent ones included.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D03337
    // Broiler-Human:        PENDING
    internal int LiveCount => byTarget.Count;

    /// <summary>How many slots have been retired because their generation would have wrapped.</summary>
    internal int RetiredCount { get; private set; }

    /// <summary>The word naming <paramref name="target"/>, allocating an entry for it if it has none.</summary>
    /// <remarks>
    /// The answer joins the nursery whether or not the entry is new: a helper that re-encodes an object
    /// whose only other word it is about to overwrite still holds a rooted word until it stores it.
    /// A permanent request makes the entry permanent for the table's life.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=48DA47
    // Broiler-Falsified-If: two live entries hold the same object, or the word answered is released by a compaction before the next safepoint
    // Broiler-Human:        PENDING
    internal ulong HandleFor(object target, ushort tag, bool permanent = false)
    {
        if (!byTarget.TryGetValue(target, out var slot))
        {
            slot = Allocate();
            targets[slot] = target;
            tags[slot] = tag;
            states[slot] = Live;
            byTarget.Add(target, slot);
        }
        else if (tags[slot] != tag)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "one object was encoded under two handle kinds");
        }

        if (permanent)
        {
            states[slot] = Permanent;
        }
        else if (states[slot] == Live)
        {
            nursery.Add(slot);
        }

        return JsWord.Handle(tag, generations[slot], (uint)slot);
    }

    /// <summary>The object a handle word names, if the word still names one.</summary>
    /// <remarks>
    /// <b>Four checks, and a word that fails any of them answers nothing.</b> The index must be one the
    /// table has issued, the entry must hold a value, and the word's generation and tag must be the
    /// entry's. A refusal is the caller's internal defect to report; it is never another object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=6E1598
    // Broiler-Falsified-If: this answers true for a word whose index, generation or tag differs from a holding entry's, or answers an object other than the one the word was issued for
    // Broiler-Human:        PENDING
    internal bool TryResolve(ulong word, out object? target)
    {
        target = null;

        if (!JsWord.IsHandle(word))
        {
            return false;
        }

        var index = JsWord.HandleIndex(word);

        if (index >= (uint)used ||
            states[index] is not (Live or Permanent) ||
            generations[index] != JsWord.HandleGeneration(word) ||
            tags[index] != JsWord.Tag(word))
        {
            return false;
        }

        target = targets[index];
        return true;
    }

    /// <summary>
    /// The boundary a helper crosses on entry: every word the previous helper answered is stored by now,
    /// so the nursery empties, and under handle-stress the table compacts.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=32CA5F
    // Broiler-Falsified-If: under handle-stress a safepoint returns without a compaction having run
    // Broiler-Human:        PENDING
    internal void Safepoint()
    {
        nursery.Clear();

        if (Stress)
        {
            Compact();
        }
    }

    /// <summary>Marks every entry reachable from the roots and releases every other one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=A9A332
    // Broiler-Falsified-If: an entry named by a live slab word, a nursery handle or a permanent entry is released, or an entry named by none of them survives
    // Broiler-Human:        PENDING
    internal void Compact()
    {
        if (marks.Length < used)
        {
            marks = new bool[targets.Length];
        }

        System.Array.Clear(marks, 0, used);

        foreach (var slot in nursery)
        {
            marks[slot] = true;
        }

        slab.Scan(this);
        var live = 0;

        for (var slot = 0; slot < used; slot++)
        {
            if (states[slot] == Permanent || (states[slot] == Live && marks[slot]))
            {
                live++;
            }
            else if (states[slot] == Live)
            {
                Release(slot);
            }
        }

        liveAfterCompaction = live;
        Compactions++;
    }

    /// <summary>Marks the entry a live slab word names; a live word naming no holding entry is a defect.</summary>
    /// <remarks>
    /// <b>A LIVE WORD THAT NAMES A RELEASED ENTRY IS A ROOTING BUG ALREADY MADE</b>, and the scan says so
    /// rather than skipping it: skipping would leave the word to be refused at its next use, far from the
    /// mistake, or - if its slot had been reissued at the same generation, which retiring before a wrap
    /// prevents - to answer another object.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=0860FA
    // Broiler-Falsified-If: a live handle word whose entry does not hold a value is passed over without a defect
    // Broiler-Human:        PENDING
    internal void MarkRoot(ulong word)
    {
        if (!JsWord.IsHandle(word))
        {
            return;
        }

        if (!TryResolve(word, out _))
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "a live value-form word names a handle the table has released");
        }

        marks[JsWord.HandleIndex(word)] = true;
    }

    /// <summary>A free slot, compacting or growing the table when there is none.</summary>
    /// <remarks>
    /// A full table compacts only once it holds <see cref="GrowthFactor"/> times the entries the last
    /// compaction left live, and never below <see cref="MinimumCompactionSize"/>; otherwise it grows. So
    /// every compaction releases, or is paid for by, a proportional number of allocations.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=767A29
    // Broiler-Falsified-If: a slot is answered that is live, permanent or retired
    // Broiler-Human:        PENDING
    private int Allocate()
    {
        if (freeCount > 0)
        {
            return free[--freeCount];
        }

        if (used == targets.Length)
        {
            if (used >= System.Math.Max(MinimumCompactionSize, GrowthFactor * liveAfterCompaction))
            {
                Compact();

                if (freeCount > 0)
                {
                    return free[--freeCount];
                }
            }

            Grow();
        }

        // A SLOT'S FIRST GENERATION IS ONE, so the all-zero payload is never a word the table issued.
        generations[used] = 1;
        return used++;
    }

    /// <summary>Drops an entry's reference, advances its generation, and frees or retires its slot.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=68B984
    // Broiler-Falsified-If: a released slot keeps its generation, keeps its reference, or is reissued after its generation wrapped to zero
    // Broiler-Human:        PENDING
    private void Release(int slot)
    {
        byTarget.Remove(targets[slot]!);
        targets[slot] = null;
        tags[slot] = 0;
        generations[slot]++;

        if (generations[slot] == 0)
        {
            states[slot] = Retired;
            RetiredCount++;
            return;
        }

        states[slot] = Free;
        free[freeCount++] = slot;
    }

    /// <summary>Doubles every per-slot array.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=0293C4
    // Broiler-Human:        PENDING
    private void Grow()
    {
        var capacity = checked(targets.Length * 2);
        System.Array.Resize(ref targets, capacity);
        System.Array.Resize(ref generations, capacity);
        System.Array.Resize(ref tags, capacity);
        System.Array.Resize(ref states, capacity);
        System.Array.Resize(ref free, capacity);
    }
}
