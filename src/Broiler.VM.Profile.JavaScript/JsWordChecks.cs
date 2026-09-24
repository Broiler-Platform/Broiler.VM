// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   39
// Annotated:        39/39
// Exempt:           10
// Human-reviewed:   0/39
// IP risk:          Low
// Security risk:    Critical
// Criteria:         22/22
// Resource impact:  3/10 max
// Unverified:       39
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// Stage JSV-0's exit gate: the value word, its codec, the handle table and the slab scan, checked
/// over every kind, every NaN class and a model-checked fuzz target (JSD-0035 section 10).
/// </summary>
/// <remarks>
/// <para>
/// <b>NOTHING IN THE PRODUCT CALLS THIS, AND IT IS PUBLIC ONLY SO THAT A CHECKS LANE CAN.</b> The types
/// it drives are internal, and this profile grants no assembly its internals; <see cref="JsNativeAbi"/>
/// is the precedent for a public entry that exists for checks alone. No execution path reaches a value
/// word at this stage, so everything here runs over tables and slabs the checks build themselves.
/// </para>
/// <para>
/// <b>TWO ROWS ARE NEGATIVE CONTROLS, AND THEY ARE WHAT MAKE THE REST WORTH READING.</b> One stores a
/// word without publishing it and shows that handle-stress refuses it at the next safepoint while the
/// ordinary table lets it decode; the other stores a released handle in a live word and shows the scan
/// stops on it. A stress mode nobody watched catch a rooting mistake would be a stress mode nobody
/// should trust.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=49B052
// Broiler-Human:        PENDING
public static class JsWordChecks
{
    /// <summary>Every JSV-0 check, including three fixed-seed fuzz runs, as named rows.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A33212
    // Broiler-Human:        PENDING
    public static (string Name, bool Passed, string Detail)[] Run() =>
    [
        Row("every-tag-prefix-has-exactly-one-class", EveryTagPrefixHasExactlyOneClass),
        Row("the-hardware-nans-are-numbers", TheHardwareNaNsAreNumbers),
        Row("every-nan-payload-encodes-as-itself-or-the-canonical-nan", EveryNaNPayloadEncodesCanonically),
        Row("every-kind-round-trips", EveryKindRoundTrips),
        Row("one-object-has-one-word", OneObjectHasOneWord),
        Row("words-that-name-no-value-are-refused", WordsThatNameNoValueAreRefused),
        Row("generation-zero-is-never-issued-and-a-wrapping-slot-is-retired", AWrappingSlotIsRetired),
        Row("permanent-and-nursery-entries-survive-a-compaction", PermanentAndNurseryEntriesSurvive),
        Row("an-allocation-compaction-keeps-the-helpers-own-words", AnAllocationCompactionKeepsTheNursery),
        Row("only-published-words-are-roots", OnlyPublishedWordsAreRoots),
        Row("handle-stress-refuses-an-unrooted-word", HandleStressRefusesAnUnrootedWord),
        Row("a-live-word-naming-a-released-handle-stops-the-scan", AReleasedLiveWordStopsTheScan),
        Row("a-malformed-frame-chain-stops-the-scan", AMalformedFrameChainStopsTheScan),
        Row("frames-close-in-order-and-refuse-to-overflow", FramesCloseInOrderAndRefuseToOverflow),
        Row("the-value-stack-opens-a-segment-and-scans-every-one", TheValueStackOpensASegmentAndScansEveryOne),
        Row("the-value-stack-closes-frames-in-call-order", TheValueStackClosesFramesInCallOrder),
        Row("every-helper-window-is-the-verifiers-count", EveryHelperWindowIsTheVerifiersCount),
        Fuzz(1, 20_000, stress: false),
        Fuzz(2, 20_000, stress: true),
        Fuzz(3, 20_000, stress: true),
    ];

    /// <summary>
    /// A model-checked fuzz run: random frames, stores, publications, safepoints and compactions over a
    /// slab and a table, against a shadow of every live value and of every word that fell out of use.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>TWO INVARIANTS ARE CHECKED AT EVERY STEP THAT READS.</b> A live word decodes to exactly the
    /// value the shadow holds for its slot. A word that fell out of use either still decodes to the
    /// very object it was issued for - its object may be live elsewhere, and one object has one word -
    /// or is refused; it never decodes to another object.
    /// </para>
    /// <para>
    /// <b>At the end every frame is closed and the table is compacted, and it must be empty</b>: an entry
    /// that survives with nothing rooting it is a leak the scan would carry for the instance's life.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=628167
    // Broiler-Falsified-If: a run passes in which a live word decoded to a value other than its shadow's, a retired word decoded to a different object, or an entry survived the final compaction
    // Broiler-Human:        PENDING
    public static (string Name, bool Passed, string Detail) Fuzz(int seed, int iterations, bool stress)
    {
        var name = "value-word/fuzz/seed-" + seed + (stress ? "/handle-stress" : string.Empty);

        try
        {
            var model = new FuzzModel(seed, stress);
            var failure = model.Run(iterations);
            return (name, failure is null, failure ?? model.Describe());
        }
        catch (JsAbort abort)
        {
            return (name, false, "defect: " + abort.Message);
        }
    }

    /// <summary>Runs one named check and turns a thrown defect into a failing row.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=62AA0A
    // Broiler-Human:        PENDING
    private static (string Name, bool Passed, string Detail) Row(string name, System.Func<string?> check)
    {
        try
        {
            var failure = check();
            return ("value-word/" + name, failure is null, failure ?? "holds");
        }
        catch (JsAbort abort)
        {
            return ("value-word/" + name, false, "unexpected defect: " + abort.Message);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0B402D
    // Broiler-Falsified-If: this passes while some top sixteen bits classify as two classes, as none, or as a class other than JSD-0035's table gives
    // Broiler-Human:        PENDING
    private static string? EveryTagPrefixHasExactlyOneClass()
    {
        ulong[] lows = [0, 1, 0x1234_5678_9ABCUL, JsWord.PayloadMask];

        for (uint prefix = 0; prefix <= 0xFFFF; prefix++)
        {
            foreach (var low in lows)
            {
                var word = ((ulong)prefix << JsWord.TagShift) | low;
                var tag = JsWord.Tag(word);
                var number = JsWord.IsNumber(word);
                var special = tag == JsWord.SpecialTag;
                var handle = JsWord.IsHandle(word);
                var header = JsWord.IsHeader(word);
                var reserved = tag == JsWord.ReservedTag;
                var classes = (number ? 1 : 0) + (special ? 1 : 0) + (handle ? 1 : 0) + (header ? 1 : 0) +
                    (reserved ? 1 : 0);

                var expected = prefix switch
                {
                    < JsWord.FirstTag => number,
                    JsWord.SpecialTag => special,
                    >= JsWord.StringTag and <= JsWord.BigIntTag => handle,
                    JsWord.HeaderTag => header,
                    _ => reserved,
                };

                if (classes != 1 || !expected)
                {
                    return $"prefix 0x{prefix:X4} with low bits 0x{low:X} has {classes} classes or the wrong one";
                }
            }
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=733DE9
    // Broiler-Falsified-If: this passes while a NaN the hardware computes is classified as a tagged word
    // Broiler-Human:        PENDING
    private static string? TheHardwareNaNsAreNumbers()
    {
        // COMPUTED AT RUN TIME rather than written as constants, so that what is checked is what the
        // arithmetic unit produces and not what the compiler folded.
        var zero = System.BitConverter.Int64BitsToDouble(System.Environment.ProcessorCount > int.MaxValue ? 1 : 0);
        var infinity = 1 / zero;
        double[] computed = [zero / zero, -(zero / zero), infinity - infinity, zero * infinity, System.Math.Sqrt(-1 + zero)];
        ulong[] constants = [0xFFF8_0000_0000_0000UL, JsWord.CanonicalNaN];

        foreach (var value in computed)
        {
            var bits = (ulong)System.BitConverter.DoubleToInt64Bits(value);

            if (!double.IsNaN(value) || !JsWord.IsNumber(bits))
            {
                return $"a computed NaN has bits 0x{bits:X16}, which is not a Number word";
            }
        }

        foreach (var bits in constants)
        {
            if (!JsWord.IsNumber(bits))
            {
                return $"the default NaN 0x{bits:X16} is not a Number word";
            }
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=16554C
    // Broiler-Falsified-If: this passes while some NaN that could reach a tag, including one whose bits carry a tag, encodes as anything but the canonical NaN, another NaN encodes as anything but itself, a word either answers reaches a tag under a sign flip or quieting, or a word decodes as a non-NaN or as other bits
    // Broiler-Human:        PENDING
    private static string? EveryNaNPayloadEncodesCanonically()
    {
        const ulong SignBit = 0x8000_0000_0000_0000UL;
        const ulong QuietBit = 0x0008_0000_0000_0000UL;

        var table = new JsHandleTable(new JsValueSlab(8), stress: false);
        var random = new System.Random(0x5EED);
        var payloads = new System.Collections.Generic.List<ulong>
        {
            1, 0x11, 0x12, 0xFFFF_FFFFUL, 0x8000_0000_0000UL, JsWord.PayloadMask,
        };

        for (var extra = 0; extra < 64; extra++)
        {
            payloads.Add((ulong)random.NextInt64() & JsWord.PayloadMask);
        }

        var cases = 0;

        // EVERY TOP-SIXTEEN PATTERN A NaN CAN HAVE: both signs, the quiet bit set or clear, and every
        // value of the mantissa's next three bits - which is where the tags live on the negative side.
        for (uint sign = 0; sign <= 1; sign++)
        {
            for (uint nibble = 0; nibble <= 0xF; nibble++)
            {
                var prefix = (sign << 15) | 0x7FF0u | nibble;

                foreach (var payload in payloads)
                {
                    var bits = ((ulong)prefix << JsWord.TagShift) | payload;
                    var value = System.BitConverter.Int64BitsToDouble((long)bits);

                    if (!double.IsNaN(value))
                    {
                        continue;
                    }

                    var word = JsWordCodec.Encode(JsValue.Number(value), table);
                    var expected = (bits & JsWord.NaNTagReach) != 0 ? JsWord.CanonicalNaN : bits;

                    if (word != expected)
                    {
                        return $"the NaN 0x{bits:X16} encodes as 0x{word:X16} and not as 0x{expected:X16}";
                    }

                    // WHAT EMITTED CODE CAN DO TO A NaN WORD - flip its sign, quiet it, or both - keeps it a
                    // Number, so no inline template can turn a NaN into a tag.
                    foreach (var moved in new[] { word, word ^ SignBit, word | QuietBit, (word ^ SignBit) | QuietBit })
                    {
                        if (!JsWord.IsNumber(moved))
                        {
                            return $"the NaN word 0x{word:X16} reaches the tag 0x{moved:X16}";
                        }
                    }

                    if (!JsWordCodec.TryDecode(word, table, out var back) || !back.IsNumber ||
                        !double.IsNaN(back.AsNumber()) ||
                        (ulong)System.BitConverter.DoubleToInt64Bits(back.AsNumber()) != word)
                    {
                        return $"the NaN word 0x{word:X16} does not decode to a NaN Number of its own bits";
                    }

                    cases++;
                }
            }
        }

        return cases < 32 * 60 ? $"only {cases} NaN cases were exercised" : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7E78F8
    // Broiler-Falsified-If: this passes while a value of some kind decodes to another kind, another reference or other Number bits
    // Broiler-Human:        PENDING
    private static string? EveryKindRoundTrips()
    {
        var table = new JsHandleTable(new JsValueSlab(8), stress: false);
        double[] numbers =
        [
            0.0, -0.0, 1, -1, 0.5, double.MaxValue, double.MinValue, double.Epsilon, -double.Epsilon,
            double.PositiveInfinity, double.NegativeInfinity, 9007199254740992, -9007199254740994,
            1e-310, 4.9406564584124654E-324, double.NaN,
        ];

        var values = new System.Collections.Generic.List<JsValue>
        {
            JsValue.Empty, JsValue.Undefined, JsValue.Null, JsValue.True, JsValue.False,
            JsValue.String(string.Empty), JsValue.String(new string('x', 3)),
            JsValue.Object(new JsObject(null)), JsValue.Symbol(new JsSymbol("tag", true)),
            JsValue.BigInt(new JsBigInt(new System.Numerics.BigInteger(-12345))),
        };

        foreach (var number in numbers)
        {
            values.Add(JsValue.Number(number));
        }

        foreach (var value in values)
        {
            var word = JsWordCodec.Encode(value, table);

            if (!JsWordCodec.TryDecode(word, table, out var back) || !Same(value, back))
            {
                return $"a {value.Type} did not survive the round trip through 0x{word:X16}";
            }
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=174F1F
    // Broiler-Falsified-If: this passes while one object is answered two words, or two objects one
    // Broiler-Human:        PENDING
    private static string? OneObjectHasOneWord()
    {
        var table = new JsHandleTable(new JsValueSlab(8), stress: false);
        var one = new JsObject(null);
        var other = new JsObject(null);
        var first = JsWordCodec.Encode(JsValue.Object(one), table);
        var again = JsWordCodec.Encode(JsValue.Object(one), table);
        var second = JsWordCodec.Encode(JsValue.Object(other), table);

        // TWO STRINGS WITH THE SAME TEXT ARE TWO WORDS. The table is keyed by reference, so a word's
        // equality is identity; string equality is by content and stays a helper's to answer.
        var text = JsWordCodec.Encode(JsValue.String(new string('a', 2)), table);
        var sameText = JsWordCodec.Encode(JsValue.String(new string('a', 2)), table);

        return first != again ? "one object was answered two words"
            : first == second ? "two objects were answered one word"
            : text == sameText ? "two string instances were answered one word"
            : table.LiveCount != 4 ? $"the table holds {table.LiveCount} entries rather than 4"
            : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=9A83D0
    // Broiler-Falsified-If: this passes while a header, a reserved word, an undefined special, a handle with a foreign index, generation or tag, or a released handle decodes to a value
    // Broiler-Human:        PENDING
    private static string? WordsThatNameNoValueAreRefused()
    {
        var table = new JsHandleTable(new JsValueSlab(8), stress: false);
        var text = JsWordCodec.Encode(JsValue.String(new string('s', 1)), table);
        var released = JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table);
        var index = JsWord.HandleIndex(text);
        var generation = JsWord.HandleGeneration(text);

        // Keep the string rooted as permanent, and let the object go.
        _ = table.HandleFor(JsWordCodec.Decode(text, table).AsString(), JsWord.StringTag, permanent: true);
        table.Safepoint();
        table.Compact();

        (string Label, ulong Word)[] refused =
        [
            ("a frame header", JsWord.Header(3, 1)),
            ("a reserved word", (ulong)JsWord.ReservedTag << JsWord.TagShift),
            ("special payload 5", JsWord.Undefined | 5),
            ("the largest special payload", JsWord.Undefined | JsWord.PayloadMask),
            ("an index past the table", JsWord.Handle(JsWord.StringTag, generation, 1000)),
            ("another generation", JsWord.Handle(JsWord.StringTag, (ushort)(generation + 1), index)),
            ("generation zero", JsWord.Handle(JsWord.StringTag, 0, index)),
            ("another kind's tag", JsWord.Handle(JsWord.ObjectTag, generation, index)),
            ("a released entry", released),
        ];

        foreach (var (label, word) in refused)
        {
            if (JsWordCodec.TryDecode(word, table, out _))
            {
                return $"{label} (0x{word:X16}) decoded to a value";
            }
        }

        return JsWordCodec.TryDecode(text, table, out _) ? null : "the rooted string no longer decodes";
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=5234F5
    // Broiler-Falsified-If: this passes while generation zero is issued, a slot is reissued after its generation wrapped, or a word from an earlier generation of a reissued slot decodes
    // Broiler-Human:        PENDING
    private static string? AWrappingSlotIsRetired()
    {
        var table = new JsHandleTable(new JsValueSlab(8), stress: false, initialCapacity: 1);
        ulong earliest = 0;
        var slotsSeen = new System.Collections.Generic.HashSet<uint>();

        for (var round = 0; round < 0x10000 + 2; round++)
        {
            var word = JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table);

            if (JsWord.HandleGeneration(word) == 0)
            {
                return $"generation zero was issued in round {round}";
            }

            if (round == 0)
            {
                earliest = word;
            }
            else if (JsWordCodec.TryDecode(earliest, table, out _))
            {
                return $"the first round's word still decodes in round {round}";
            }

            slotsSeen.Add(JsWord.HandleIndex(word));
            table.Safepoint();
            table.Compact();
        }

        return table.RetiredCount != 1 ? $"{table.RetiredCount} slots were retired rather than 1"
            : slotsSeen.Count != 2 ? $"{slotsSeen.Count} slots were used rather than 2"
            : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=71804B
    // Broiler-Falsified-If: this passes while a permanent entry or a handle answered since the last safepoint is released, or a nursery handle survives once the safepoint has passed
    // Broiler-Human:        PENDING
    private static string? PermanentAndNurseryEntriesSurvive()
    {
        var table = new JsHandleTable(new JsValueSlab(8), stress: false);
        var constant = table.HandleFor(new string('c', 1), JsWord.StringTag, permanent: true);
        var held = JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table);
        table.Compact();

        if (!JsWordCodec.TryDecode(constant, table, out _) || !JsWordCodec.TryDecode(held, table, out _))
        {
            return "a permanent entry or a nursery handle did not survive a compaction";
        }

        table.Safepoint();
        table.Compact();

        return !JsWordCodec.TryDecode(constant, table, out _) ? "the permanent entry was released"
            : JsWordCodec.TryDecode(held, table, out _) ? "a handle nothing roots survived its safepoint"
            : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=BC0C53
    // Broiler-Falsified-If: this passes while a compaction an allocation triggers releases a handle answered since the last safepoint
    // Broiler-Human:        PENDING
    private static string? AnAllocationCompactionKeepsTheNursery()
    {
        var table = new JsHandleTable(new JsValueSlab(8), stress: false, initialCapacity: 1);

        // The first helper's words: never stored, so garbage once its safepoint passes.
        for (var index = 0; index < JsHandleTable.MinimumCompactionSize; index++)
        {
            _ = JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table);
        }

        table.Safepoint();
        var compactions = table.Compactions;
        var mine = new System.Collections.Generic.List<ulong>();

        // The second helper allocates until the table has to compact to find room.
        for (var index = 0; index < 2 * JsHandleTable.MinimumCompactionSize; index++)
        {
            mine.Add(JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table));
        }

        if (table.Compactions == compactions)
        {
            return "no allocation triggered a compaction, so nothing was checked";
        }

        foreach (var word in mine)
        {
            if (!JsWordCodec.TryDecode(word, table, out _))
            {
                return $"a word the running helper holds (0x{word:X16}) was released by its own allocation";
            }
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=60AB8E
    // Broiler-Falsified-If: this passes while a published live word's entry is released, or a word past the published length or past the top roots its entry
    // Broiler-Human:        PENDING
    private static string? OnlyPublishedWordsAreRoots()
    {
        var slab = new JsValueSlab(16);
        var table = new JsHandleTable(slab, stress: false);

        if (!slab.TryPushFrame(4, out var frame))
        {
            return "a four-word frame did not fit a sixteen-word slab";
        }

        var first = JsValueSlab.FirstWord(frame);
        ulong[] words =
        [
            JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table),
            JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table),
            JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table),
            JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table),
        ];

        slab.Words[first] = words[0];
        slab.Words[first + 1] = words[1];
        slab.Words[first + 2] = words[2];
        slab.Words[slab.Top + 1] = words[3];
        slab.Publish(frame, 2);
        table.Safepoint();
        table.Compact();

        return !JsWordCodec.TryDecode(words[0], table, out _) || !JsWordCodec.TryDecode(words[1], table, out _)
            ? "a published live word's entry was released"
            : JsWordCodec.TryDecode(words[2], table, out _) ? "a word past the published length rooted its entry"
            : JsWordCodec.TryDecode(words[3], table, out _) ? "a word past the top rooted its entry"
            : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=190D9D
    // Broiler-Falsified-If: this passes while handle-stress lets a stored but unpublished word decode after its safepoint
    // Broiler-Human:        PENDING
    private static string? HandleStressRefusesAnUnrootedWord()
    {
        // THE SAME MISTAKE TWICE: a helper stores a word into its frame and forgets to publish it.
        var latent = Unpublished(stress: false);
        var caught = Unpublished(stress: true);

        return !latent ? "the control failed: without handle-stress the unpublished word should still decode"
            : caught ? "handle-stress let an unpublished word decode after its safepoint"
            : null;

        static bool Unpublished(bool stress)
        {
            var slab = new JsValueSlab(8);
            var table = new JsHandleTable(slab, stress);
            _ = slab.TryPushFrame(2, out var frame);
            var first = JsValueSlab.FirstWord(frame);
            slab.Words[first] = JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table);
            slab.Publish(frame, 1);
            slab.Words[first + 1] = JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table);
            table.Safepoint();
            return JsWordCodec.TryDecode(slab.Words[first + 1], table, out _);
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=0B4BDC
    // Broiler-Falsified-If: this passes while a compaction completes over a live word that names a released handle
    // Broiler-Human:        PENDING
    private static string? AReleasedLiveWordStopsTheScan()
    {
        var slab = new JsValueSlab(8);
        var table = new JsHandleTable(slab, stress: false);
        var stale = JsWordCodec.Encode(JsValue.Object(new JsObject(null)), table);
        table.Safepoint();
        table.Compact();
        _ = slab.TryPushFrame(1, out var frame);
        slab.Words[JsValueSlab.FirstWord(frame)] = stale;
        slab.Publish(frame, 1);

        try
        {
            table.Compact();
            return "the scan passed over a live word naming a released handle";
        }
        catch (JsAbort abort) when (abort.Kind == JsAbortKind.InternalDefect)
        {
            return null;
        }
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=6EF373
    // Broiler-Falsified-If: this passes while a scan completes over a frame chain with a missing header or a header stating more live words than its region
    // Broiler-Human:        PENDING
    private static string? AMalformedFrameChainStopsTheScan()
    {
        ulong[] headers =
        [
            JsWord.FromNumber(3),
            ((ulong)JsWord.HeaderTag << JsWord.TagShift) | (2UL << 24) | 3,
            JsWord.Header(JsWord.MaximumFrameLength, 0),
        ];

        foreach (var header in headers)
        {
            var slab = new JsValueSlab(8);
            var table = new JsHandleTable(slab, stress: false);
            _ = slab.TryPushFrame(2, out var frame);
            slab.Words[frame] = header;

            try
            {
                table.Compact();
                return $"the scan completed over the header 0x{header:X16}";
            }
            catch (JsAbort abort) when (abort.Kind == JsAbortKind.InternalDefect)
            {
            }
        }

        return null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=286879
    // Broiler-Falsified-If: this passes while an outer frame closes over an open inner one, or a frame past the advertised capacity opens
    // Broiler-Human:        PENDING
    private static string? FramesCloseInOrderAndRefuseToOverflow()
    {
        var slab = new JsValueSlab(10);

        if (slab.Words.Length != 10 + JsValueSlab.Headroom)
        {
            return $"the slab carries {slab.Words.Length - 10} words of headroom";
        }

        if (!slab.TryPushFrame(3, out var outer) || !slab.TryPushFrame(5, out var inner))
        {
            return "two frames of ten words did not fit a ten-word slab";
        }

        if (slab.TryPushFrame(0, out _))
        {
            return "a frame opened past the advertised capacity";
        }

        try
        {
            slab.PopFrame(outer);
            return "an outer frame closed while an inner one was open";
        }
        catch (JsAbort abort) when (abort.Kind == JsAbortKind.InternalDefect)
        {
        }

        slab.PopFrame(inner);
        slab.PopFrame(outer);
        return slab.Top == 0 ? null : $"the top is {slab.Top} after every frame closed";
    }

    /// <summary>
    /// A region too large for the first segment opens a second, a word in each is rooted by one scan, and
    /// closing the second frame returns the chain to the first (JSV-1).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=048F07
    // Broiler-Falsified-If: this passes while a frame spans two segments, a compaction releases a handle a published word in the second segment names, or the chain does not return to its first segment
    // Broiler-Human:        PENDING
    private static string? TheValueStackOpensASegmentAndScansEveryOne()
    {
        var stack = new JsValueStack();
        var table = new JsHandleTable(stack, stress: false);

        if (!stack.TryPush(JsValueStack.SegmentWords - 8, out var first, out var outer))
        {
            return "the first region did not fit its segment";
        }

        if (!stack.TryPush(64, out var second, out var inner) || ReferenceEquals(first, second) || stack.SegmentCount != 2)
        {
            return "a region past the first segment's end did not open a second segment";
        }

        var kept = new JsObject(null);
        var alsoKept = new JsObject(null);
        first.Words[JsValueSlab.FirstWord(outer)] = JsWordCodec.Encode(JsValue.Object(kept), table);
        first.Publish(outer, 1);
        second.Words[JsValueSlab.FirstWord(inner)] = JsWordCodec.Encode(JsValue.Object(alsoKept), table);
        second.Publish(inner, 1);
        table.Safepoint();
        table.Compact();

        if (!JsWordCodec.TryDecode(first.Words[JsValueSlab.FirstWord(outer)], table, out var one) ||
            !ReferenceEquals(one.AsObject(), kept) ||
            !JsWordCodec.TryDecode(second.Words[JsValueSlab.FirstWord(inner)], table, out var two) ||
            !ReferenceEquals(two.AsObject(), alsoKept))
        {
            return "a compaction released a word published in one of the two segments";
        }

        stack.Pop(second, inner);

        if (!stack.TryPush(4, out var again, out var reopened) || !ReferenceEquals(again, first))
        {
            return "closing the second segment's only frame did not return the chain to the first";
        }

        stack.Pop(again, reopened);
        stack.Pop(first, outer);
        return null;
    }

    /// <summary>A frame closed while a frame after it is open is a defect, across segments as within one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=095AB5
    // Broiler-Falsified-If: this passes while an outer frame, in its own segment or an earlier one, closes over an open frame
    // Broiler-Human:        PENDING
    private static string? TheValueStackClosesFramesInCallOrder()
    {
        var stack = new JsValueStack();

        if (!stack.TryPush(JsValueStack.SegmentWords - 8, out var first, out var outer) ||
            !stack.TryPush(64, out _, out _))
        {
            return "the two frames did not open";
        }

        try
        {
            stack.Pop(first, outer);
            return "a frame in the first segment closed while one in the second was open";
        }
        catch (JsAbort abort) when (abort.Kind == JsAbortKind.InternalDefect)
        {
            return null;
        }
    }

    /// <summary>
    /// For every defined opcode and a spread of operands, the pop count a value helper encodes from is the
    /// verifier's, and its read window covers it (JSV-1).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=3FEEF1
    // Broiler-Falsified-If: this passes while a helper's pop count differs from TryDescribe's for some instruction, or its read depth is below that count
    // Broiler-Human:        PENDING
    private static string? EveryHelperWindowIsTheVerifiersCount()
    {
        var code = new byte[8];

        for (var value = 0; value < 256; value++)
        {
            if (!JsOpcodes.IsDefined((byte)value))
            {
                continue;
            }

            var opcode = (JsOpcode)value;

            foreach (var operand in new uint[] { 0, 1, 7, 200 })
            {
                System.Array.Clear(code);
                code[0] = (byte)value;
                System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(System.MemoryExtensions.AsSpan(code, 1), operand);

                var width = JsOpcodes.OperandWidth(opcode);
                var encoded = width switch
                {
                    0 => 0u,
                    1 => operand & 0xFF,
                    2 => operand & 0xFFFF,
                    _ => operand,
                };

                if (JsOpcodes.Shape(opcode) == JsOperandShape.U8U16)
                {
                    // The depth is the first byte and the slot the next two, so these bytes read as
                    // depth `operand & 0xFF` and slot `operand >> 8`.
                    encoded = ((operand & 0xFF) << 16) | ((operand >> 8) & 0xFFFF);
                }

                if (!JsOpcodes.TryDescribe(opcode, encoded, out var pops, out _))
                {
                    return $"{opcode} is defined and TryDescribe does not describe it";
                }

                var helperPops = JsValueWindows.Pops(code, 0);
                var reads = JsValueWindows.ReadDepth(code, 0);

                if (helperPops != pops || reads < pops)
                {
                    return $"{opcode} with operand {encoded}: the helper pops {helperPops} and reads {reads}, and the verifier counts {pops} pops";
                }
            }
        }

        return null;
    }

    /// <summary>Whether two values are the same value: kind, reference, and Number bits unless both are NaN.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=20F176
    // Broiler-Human:        PENDING
    private static bool Same(in JsValue left, in JsValue right)
    {
        if (left.Type != right.Type)
        {
            return false;
        }

        return left.Type switch
        {
            JsType.Number => double.IsNaN(left.AsNumber())
                ? double.IsNaN(right.AsNumber())
                : System.BitConverter.DoubleToInt64Bits(left.AsNumber()) ==
                    System.BitConverter.DoubleToInt64Bits(right.AsNumber()),
            JsType.Boolean => left.AsBoolean() == right.AsBoolean(),
            JsType.String => ReferenceEquals(left.AsString(), right.AsString()),
            JsType.Object => ReferenceEquals(left.AsObject(), right.AsObject()),
            JsType.Symbol => ReferenceEquals(left.AsSymbol(), right.AsSymbol()),
            JsType.BigInt => ReferenceEquals(left.AsBigInt(), right.AsBigInt()),
            _ => true,
        };
    }

    /// <summary>The object a referenced value holds, or null for a value that holds none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1C9609
    // Broiler-Human:        PENDING
    private static object? Target(in JsValue value) => value.Type switch
    {
        JsType.String => value.AsString(),
        JsType.Object => value.AsObject(),
        JsType.Symbol => value.AsSymbol(),
        JsType.BigInt => value.AsBigInt(),
        _ => null,
    };

    /// <summary>The fuzz target's state: the slab and table under test and a shadow of what they must hold.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A69F70
    // Broiler-Falsified-If: the model's shadow of a live slot differs from the value last stored there
    // Broiler-Human:        PENDING
    private sealed class FuzzModel
    {
        /// <summary>The fuzz slab's capacity, deep enough for a few hundred frames.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A847AC
        // Broiler-Human:        PENDING
        private const int SlabWords = 4096;
        /// <summary>How many fallen-out-of-use words the model keeps to re-check.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=938468
        // Broiler-Human:        PENDING
        private const int DroppedLimit = 8192;

        private readonly System.Random random;
        private readonly JsValueSlab slab = new(SlabWords);
        private readonly JsHandleTable table;
        private readonly JsValue[] pool;
        private readonly System.Collections.Generic.List<(int Frame, System.Collections.Generic.List<JsValue> Values)> frames = [];
        private readonly System.Collections.Generic.List<(ulong Word, object Target)> dropped = [];
        private int liveChecks;
        private int staleDecoded;
        private int staleRefused;
        private int deepest;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=14CD86
        // Broiler-Human:        PENDING
        internal FuzzModel(int seed, bool stress)
        {
            random = new System.Random(seed);
            table = new JsHandleTable(slab, stress, initialCapacity: 4);
            var values = new System.Collections.Generic.List<JsValue>();

            for (var index = 0; index < 12; index++)
            {
                values.Add(JsValue.String(new string((char)('a' + index), index + 1)));
                values.Add(JsValue.Object(new JsObject(null)));
            }

            for (var index = 0; index < 6; index++)
            {
                values.Add(JsValue.Symbol(new JsSymbol("s" + index, true)));
                values.Add(JsValue.BigInt(new JsBigInt(new System.Numerics.BigInteger(index * 7919))));
            }

            pool = [.. values];
        }

        /// <summary>Runs the operations and answers the first broken invariant, or null.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=E24C01
        // Broiler-Falsified-If: an operation breaks an invariant and the run still answers null
        // Broiler-Human:        PENDING
        internal string? Run(int iterations)
        {
            for (var step = 0; step < iterations; step++)
            {
                var failure = random.Next(20) switch
                {
                    < 4 => Push(),
                    < 6 => Pop(),
                    < 10 => Overwrite(),
                    < 12 => Republish(),
                    < 14 => Safepoint(),
                    14 => Compact(),
                    < 18 => CheckLive(random.Next(frames.Count + 1)),
                    _ => CheckDropped(),
                };

                if (failure is not null)
                {
                    return $"step {step}: {failure}";
                }
            }

            for (var frame = 0; frame < frames.Count; frame++)
            {
                if (CheckLive(frame) is { } failure)
                {
                    return "final check: " + failure;
                }
            }

            while (frames.Count > 0)
            {
                _ = Pop();
            }

            table.Safepoint();
            table.Compact();

            return table.LiveCount == 0 ? null : $"{table.LiveCount} entries survived with nothing rooting them";
        }

        /// <summary>What the run exercised, for the passing row's detail.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AA21C7
        // Broiler-Human:        PENDING
        internal string Describe() =>
            $"{table.Compactions} compactions, {liveChecks} live words checked, {staleDecoded} stale words " +
            $"still naming their own object, {staleRefused} refused, deepest {deepest} frames, table empty at the end";

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D02D13
        // Broiler-Human:        PENDING
        private string? Push()
        {
            var region = random.Next(0, 24);

            if (!slab.TryPushFrame(region, out var frame))
            {
                return null;
            }

            var values = new System.Collections.Generic.List<JsValue>();
            var live = random.Next(0, region + 1);

            for (var index = 0; index < live; index++)
            {
                var value = NextValue();
                slab.Words[JsValueSlab.FirstWord(frame) + index] = JsWordCodec.Encode(value, table);
                values.Add(value);
            }

            slab.Publish(frame, live);
            frames.Add((frame, values));
            deepest = System.Math.Max(deepest, frames.Count);
            return null;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=864D56
        // Broiler-Human:        PENDING
        private string? Pop()
        {
            if (frames.Count == 0)
            {
                return null;
            }

            var (frame, values) = frames[^1];

            for (var index = 0; index < values.Count; index++)
            {
                Drop(JsValueSlab.FirstWord(frame) + index, values[index]);
            }

            slab.PopFrame(frame);
            frames.RemoveAt(frames.Count - 1);
            return null;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AE9ED9
        // Broiler-Human:        PENDING
        private string? Overwrite()
        {
            if (frames.Count == 0)
            {
                return null;
            }

            var (frame, values) = frames[random.Next(frames.Count)];

            if (values.Count == 0)
            {
                return null;
            }

            var index = random.Next(values.Count);
            Drop(JsValueSlab.FirstWord(frame) + index, values[index]);
            var value = NextValue();
            slab.Words[JsValueSlab.FirstWord(frame) + index] = JsWordCodec.Encode(value, table);
            values[index] = value;
            return null;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=754E2C
        // Broiler-Human:        PENDING
        private string? Republish()
        {
            if (frames.Count == 0)
            {
                return null;
            }

            var (frame, values) = frames[random.Next(frames.Count)];
            var live = random.Next(0, slab.Region(frame) + 1);

            // A GROWING FRAME STORES BEFORE IT PUBLISHES, as a helper must: a published word is scanned,
            // and a word that is not yet stored would be whatever the slab held there before.
            while (values.Count < live)
            {
                var value = NextValue();
                slab.Words[JsValueSlab.FirstWord(frame) + values.Count] = JsWordCodec.Encode(value, table);
                values.Add(value);
            }

            while (values.Count > live)
            {
                Drop(JsValueSlab.FirstWord(frame) + values.Count - 1, values[^1]);
                values.RemoveAt(values.Count - 1);
            }

            slab.Publish(frame, live);
            return null;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2DA65E
        // Broiler-Human:        PENDING
        private string? Safepoint()
        {
            table.Safepoint();

            // UNDER HANDLE-STRESS EVERY LIVE WORD IS CHECKED AFTER EVERY SAFEPOINT, because that is the
            // moment a rooting mistake turns into a released entry.
            if (table.Stress)
            {
                for (var frame = 0; frame < frames.Count; frame++)
                {
                    if (CheckLive(frame) is { } failure)
                    {
                        return failure;
                    }
                }
            }

            return null;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0EF149
        // Broiler-Human:        PENDING
        private string? Compact()
        {
            table.Compact();
            return null;
        }

        /// <summary>Checks every live word of one frame against the shadow; an out-of-range frame checks nothing.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=04318B
        // Broiler-Falsified-If: a live word that does not decode to its shadow value is passed
        // Broiler-Human:        PENDING
        private string? CheckLive(int which)
        {
            if (which >= frames.Count)
            {
                return null;
            }

            var (frame, values) = frames[which];

            if (slab.Live(frame) != values.Count)
            {
                return $"frame {which} publishes {slab.Live(frame)} live words and holds {values.Count}";
            }

            for (var index = 0; index < values.Count; index++)
            {
                var word = slab.Words[JsValueSlab.FirstWord(frame) + index];
                liveChecks++;

                if (!JsWordCodec.TryDecode(word, table, out var back) || !Same(values[index], back))
                {
                    return $"frame {which} word {index} (0x{word:X16}) no longer decodes to its {values[index].Type}";
                }
            }

            return null;
        }

        /// <summary>Checks one word that fell out of use: its own object, or refused, and never another.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=DCE26B
        // Broiler-Falsified-If: a word that fell out of use decodes to an object other than the one it was issued for and the check passes
        // Broiler-Human:        PENDING
        private string? CheckDropped()
        {
            if (dropped.Count == 0)
            {
                return null;
            }

            var (word, target) = dropped[random.Next(dropped.Count)];

            if (!JsWordCodec.TryDecode(word, table, out var back))
            {
                staleRefused++;
                return null;
            }

            staleDecoded++;
            return ReferenceEquals(Target(back), target)
                ? null
                : $"a word that fell out of use (0x{word:X16}) decoded to another object";
        }

        /// <summary>Records a slot's word as fallen out of use when it names an object.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C065EB
        // Broiler-Human:        PENDING
        private void Drop(int at, in JsValue value)
        {
            if (Target(value) is not { } target)
            {
                return;
            }

            if (dropped.Count >= DroppedLimit)
            {
                dropped.RemoveRange(0, DroppedLimit / 2);
            }

            dropped.Add((slab.Words[at], target));
        }

        /// <summary>
        /// A random value: Numbers with hostile NaNs among them, the specials, the pool's objects, and fresh
        /// objects that nothing else will ever hold.
        /// </summary>
        /// <remarks>
        /// The fresh ones are what make slots turn over: a pool object stays reachable from somewhere for
        /// most of a run, so without them a released-and-reissued slot - the case where a stale word must
        /// be refused rather than answer its successor - would be rare.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8F9D71
        // Broiler-Human:        PENDING
        private JsValue NextValue() => random.Next(24) switch
        {
            20 or 21 => JsValue.Object(new JsObject(null)),
            22 => JsValue.String(new string('f', 1 + random.Next(4))),
            23 => JsValue.Symbol(new JsSymbol("fresh", true)),
            < 3 => JsValue.Number(random.Next(-1000, 1000)),
            < 5 => JsValue.Number(System.BitConverter.Int64BitsToDouble(random.NextInt64())),
            5 => JsValue.Number(System.BitConverter.Int64BitsToDouble(
                (long)(((ulong)random.Next(JsWord.FirstTag, 0x10000) << JsWord.TagShift) |
                    ((ulong)random.NextInt64() & JsWord.PayloadMask)))),
            6 => JsValue.Number(-0.0),
            7 => JsValue.Undefined,
            8 => JsValue.Null,
            9 => JsValue.Boolean(random.Next(2) == 0),
            10 => JsValue.Empty,
            _ => pool[random.Next(pool.Length)],
        };
    }
}
