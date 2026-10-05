// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           11
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Intl.Segmenter</c> (ECMA-402 s19; JSD-0050): a String's grapheme clusters, words or sentences by
/// UAX #29's default rules, as Segments objects and their iterators.
/// </summary>
/// <remarks>
/// <b>A Segments object finds its boundaries once.</b> The first <c>containing</c> or iteration of a
/// Segments object runs <see cref="JsSegmentation"/> over its whole string and keeps the boundaries;
/// FindBoundary is then a binary search, and every iterator of the object shares them.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary><c>%Intl.Segmenter.prototype%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B3F475
    // Broiler-Human:        PENDING
    internal JsObject? SegmenterPrototype { get; private set; }

    /// <summary><c>%Intl.Segmenter%</c>, or nothing where the surface is not built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=33F850
    // Broiler-Human:        PENDING
    internal JsNativeFunction? SegmenterConstructor { get; private set; }

    /// <summary><c>%IntlSegmentsPrototype%</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5CAC70
    // Broiler-Human:        PENDING
    internal JsObject? SegmentsPrototype { get; private set; }

    /// <summary><c>%IntlSegmentIteratorPrototype%</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DF92D6
    // Broiler-Human:        PENDING
    internal JsObject? SegmentIteratorPrototype { get; private set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7D0597
    // Broiler-Human:        PENDING
    private void SetupSegmenter(JsObject intl)
    {
        var prototype = new JsObject(ObjectPrototype);
        SegmenterPrototype = prototype;

        var constructor = new JsNativeFunction(
            this,
            FunctionPrototype,
            "Segmenter",
            0,
            static (engine, thisValue, arguments) =>
                throw engine.Error("TypeError", "Constructor Intl.Segmenter requires 'new'"),
            static (engine, newTarget, arguments) => NewSegmenter(engine, newTarget, arguments));

        constructor.BuildsFromNewTarget = true;
        SegmenterConstructor = constructor;

        constructor.SetOwnProperty("prototype", JsProperty.Data(JsValue.Object(prototype), JsPropertyAttributes.None));
        prototype.SetOwnProperty(
            "constructor",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        Method(constructor, "supportedLocalesOf", 1, static (engine, thisValue, arguments) =>
        {
            var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
            var supported = FilterLocales(engine, AvailableLocales(engine), requested, Argument(arguments, 1));
            return JsValue.Object(engine.Realm.NewArray(supported.ConvertAll(static locale => JsValue.String(locale))));
        });

        Method(prototype, "resolvedOptions", 0, static (engine, thisValue, arguments) =>
        {
            var segmenter = SegmenterOfThis(engine, thisValue, "resolvedOptions");
            var options = new JsObject(engine.Realm.ObjectPrototype);
            options.DefineOrdinary("locale", JsValue.String(segmenter.Locale));
            options.DefineOrdinary("granularity", JsValue.String(segmenter.Granularity));
            return JsValue.Object(options);
        });

        Method(prototype, "segment", 1, static (engine, thisValue, arguments) =>
        {
            var segmenter = SegmenterOfThis(engine, thisValue, "segment");
            var text = engine.ToStringValue(Argument(arguments, 0));
            return JsValue.Object(new JsSegmentsObject(engine.Realm.SegmentsPrototype!, segmenter, text));
        });

        prototype.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Intl.Segmenter"), JsPropertyAttributes.Configurable));

        // %INTLSEGMENTSPROTOTYPE%: containing and [Symbol.iterator], and no tag.
        var segments = new JsObject(ObjectPrototype);
        SegmentsPrototype = segments;

        Method(segments, "containing", 1, static (engine, thisValue, arguments) =>
        {
            var target = thisValue.AsObjectOrNull() as JsSegmentsObject ??
                throw engine.Error("TypeError", "%Segments.prototype%.containing requires that 'this' be a Segments object");
            var n = engine.ToInteger(Argument(arguments, 0));

            if (n < 0 || n >= target.Text.Length)
            {
                return JsValue.Undefined;
            }

            var boundaries = target.Boundaries(engine);
            var index = (int)n;
            var position = System.Array.BinarySearch(boundaries, index);
            var start = position >= 0 ? boundaries[position] : boundaries[~position - 1];
            var end = position >= 0 ? boundaries[position + 1] : boundaries[~position];
            return JsValue.Object(SegmentData(engine, target, start, end));
        });

        segments.SetOwnSymbol(
            IteratorSymbol,
            JsProperty.Data(
                JsValue.Object(Native("[Symbol.iterator]", 0, static (engine, thisValue, arguments) =>
                {
                    var target = thisValue.AsObjectOrNull() as JsSegmentsObject ??
                        throw engine.Error("TypeError", "%Segments.prototype%[Symbol.iterator] requires that 'this' be a Segments object");
                    return JsValue.Object(new JsSegmentIteratorObject(engine.Realm.SegmentIteratorPrototype!, target));
                })),
                JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));

        // %INTLSEGMENTITERATORPROTOTYPE%: on %Iterator.prototype%, with next and its tag.
        var iterator = new JsObject(IteratorPrototype);
        SegmentIteratorPrototype = iterator;

        Method(iterator, "next", 0, static (engine, thisValue, arguments) =>
        {
            var step = thisValue.AsObjectOrNull() as JsSegmentIteratorObject ??
                throw engine.Error("TypeError", "%SegmentIterator.prototype%.next requires that 'this' be a Segment Iterator");
            var target = step.Segments;

            if (step.Position >= target.Text.Length)
            {
                return JsValue.Object(engine.Realm.IteratorResult(JsValue.Undefined, true));
            }

            var boundaries = target.Boundaries(engine);
            var start = step.Position;
            var end = boundaries[System.Array.BinarySearch(boundaries, start) + 1];
            step.Position = end;
            return JsValue.Object(engine.Realm.IteratorResult(JsValue.Object(SegmentData(engine, target, start, end)), false));
        });

        iterator.SetOwnSymbol(
            ToStringTagSymbol,
            JsProperty.Data(JsValue.String("Segmenter String Iterator"), JsPropertyAttributes.Configurable));

        intl.SetOwnProperty(
            "Segmenter",
            JsProperty.Data(JsValue.Object(constructor), JsPropertyAttributes.Writable | JsPropertyAttributes.Configurable));
    }

    /// <summary>The segmenter <paramref name="thisValue"/> is, or the <c>TypeError</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=525714
    // Broiler-Human:        PENDING
    private static JsSegmenterObject SegmenterOfThis(JsEngine engine, JsValue thisValue, string member) =>
        thisValue.AsObjectOrNull() as JsSegmenterObject ??
        throw engine.Error("TypeError", "Intl.Segmenter.prototype." + member + " requires that 'this' be an Intl.Segmenter");

    /// <summary>ECMA-402's <c>Intl.Segmenter</c> constructor (s19.1.1): the options read in its order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6BCD04
    // Broiler-Human:        PENDING
    internal static JsValue NewSegmenter(JsEngine engine, JsValue newTarget, JsValue[] arguments)
    {
        var prototype = engine.PrototypeFromConstructor(newTarget, engine.Realm.SegmenterPrototype!);

        // RESOLVEOPTIONS: the locales, the options through GetOptionsObject, the matcher, no relevant keys.
        var requested = CanonicalizeLocaleList(engine, Argument(arguments, 0));
        var options = JsValue.Object(Base64Options(engine, Argument(arguments, 1)));
        _ = GetStringOption(engine, options, "localeMatcher", ["lookup", "best fit"], "best fit");

        var resolved = ResolveLocale(
            engine,
            requested,
            new System.Collections.Generic.Dictionary<string, string?>(System.StringComparer.Ordinal),
            [],
            static (locale, key) => [null]);

        var granularity = GetStringOption(engine, options, "granularity", ["grapheme", "word", "sentence"], "grapheme")!;
        return JsValue.Object(new JsSegmenterObject(prototype, resolved.Locale, granularity));
    }

    /// <summary>ECMA-402's CreateSegmentDataObject (s19.7.1): segment, index, input, and isWordLike for words.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=94DC7A
    // Broiler-Human:        PENDING
    private static JsObject SegmentData(JsEngine engine, JsSegmentsObject segments, int start, int end)
    {
        engine.Charge(1);
        var result = new JsObject(engine.Realm.ObjectPrototype);
        result.DefineOrdinary("segment", JsValue.String(segments.Text[start..end]));
        result.DefineOrdinary("index", JsValue.Number(start));
        result.DefineOrdinary("input", JsValue.String(segments.Text));

        if (segments.Segmenter.Granularity == "word")
        {
            result.DefineOrdinary("isWordLike", JsValue.Boolean(JsSegmentation.IsWordLike(engine.Intl!.Breaks, segments.Text, start, end)));
        }

        return result;
    }
}

/// <summary>An <c>Intl.Segmenter</c>: its locale and granularity.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3CA4AA
// Broiler-Human:        PENDING
internal sealed class JsSegmenterObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=68CD06
    // Broiler-Human:        PENDING
    internal JsSegmenterObject(JsObject prototype, string locale, string granularity)
        : base(prototype)
    {
        Locale = locale;
        Granularity = granularity;
    }

    /// <summary>The resolved locale.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=04656B
    // Broiler-Human:        PENDING
    internal string Locale { get; }

    /// <summary><c>grapheme</c>, <c>word</c> or <c>sentence</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CD605E
    // Broiler-Human:        PENDING
    internal string Granularity { get; }
}

/// <summary>A Segments object: a segmenter, a string, and the string's boundaries once found.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=11FD5E
// Broiler-Human:        PENDING
internal sealed class JsSegmentsObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=501D73
    // Broiler-Human:        PENDING
    private int[]? boundaries;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=49E539
    // Broiler-Human:        PENDING
    internal JsSegmentsObject(JsObject prototype, JsSegmenterObject segmenter, string text)
        : base(prototype)
    {
        Segmenter = segmenter;
        Text = text;
    }

    /// <summary>[[SegmentsSegmenter]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A34C6C
    // Broiler-Human:        PENDING
    internal JsSegmenterObject Segmenter { get; }

    /// <summary>[[SegmentsString]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=37F0D2
    // Broiler-Human:        PENDING
    internal string Text { get; }

    /// <summary>The code unit indices that begin a segment, then the string's length, found the first time they are asked for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3293F0
    // Broiler-Human:        PENDING
    internal int[] Boundaries(JsEngine engine) =>
        boundaries ??= JsSegmentation.Boundaries(engine, engine.Intl!.Breaks, Text, Segmenter.Granularity);
}

/// <summary>A Segment Iterator: its Segments object and the index of its next segment.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3D1AE0
// Broiler-Human:        PENDING
internal sealed class JsSegmentIteratorObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=40A83B
    // Broiler-Human:        PENDING
    internal JsSegmentIteratorObject(JsObject prototype, JsSegmentsObject segments)
        : base(prototype) => Segments = segments;

    /// <summary>The Segments object iterated, whose segmenter and string are [[IteratingSegmenter]] and [[IteratedString]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9DCF9C
    // Broiler-Human:        PENDING
    internal JsSegmentsObject Segments { get; }

    /// <summary>[[IteratedStringNextSegmentCodeUnitIndex]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0EBB62
    // Broiler-Human:        PENDING
    internal int Position { get; set; }
}
