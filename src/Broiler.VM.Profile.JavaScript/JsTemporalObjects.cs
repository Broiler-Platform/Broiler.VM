// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           14
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

using BigInteger = System.Numerics.BigInteger;
using Broiler.ECMAScript.Temporal;
using Broiler.ECMAScript.Time;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>A Temporal.Duration (s7.4): its ten fields, float64-representable integers of one sign, as the shared library's record.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9D1D24
// Broiler-Human:        PENDING
internal sealed class JsDurationObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=132FAD
    // Broiler-Human:        PENDING
    internal JsDurationObject(JsObject prototype, DurationFields fields)
        : base(prototype) => Fields = fields;

    /// <summary>[[Years]] to [[Nanoseconds]].</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=D35EF1
    // Broiler-Human:        PENDING
    internal DurationFields Fields { get; }
}

/// <summary>A Temporal.Instant (s8.4).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=899814
// Broiler-Human:        PENDING
internal sealed class JsInstantObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BCFF92
    // Broiler-Human:        PENDING
    internal JsInstantObject(JsObject prototype, BigInteger epochNanoseconds)
        : base(prototype) => EpochNanoseconds = epochNanoseconds;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9C8259
    // Broiler-Human:        PENDING
    internal BigInteger EpochNanoseconds { get; }
}

/// <summary>A Temporal.PlainDate (s3.4).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BE636A
// Broiler-Human:        PENDING
internal sealed class JsPlainDateObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A5682F
    // Broiler-Human:        PENDING
    internal JsPlainDateObject(JsObject prototype, IsoDate date, string calendar)
        : base(prototype)
    {
        Date = date;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3EDEA2
    // Broiler-Human:        PENDING
    internal IsoDate Date { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}

/// <summary>A Temporal.PlainTime (s4.4).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=38D05D
// Broiler-Human:        PENDING
internal sealed class JsPlainTimeObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=55608C
    // Broiler-Human:        PENDING
    internal JsPlainTimeObject(JsObject prototype, IsoTime time)
        : base(prototype) => Time = time;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8FDC10
    // Broiler-Human:        PENDING
    internal IsoTime Time { get; }
}

/// <summary>A Temporal.PlainDateTime (s5.4).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BD630F
// Broiler-Human:        PENDING
internal sealed class JsPlainDateTimeObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=622857
    // Broiler-Human:        PENDING
    internal JsPlainDateTimeObject(JsObject prototype, IsoDateTime dateTime, string calendar)
        : base(prototype)
    {
        DateTime = dateTime;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=42C2A0
    // Broiler-Human:        PENDING
    internal IsoDateTime DateTime { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}

/// <summary>A Temporal.ZonedDateTime (s6.4).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=902552
// Broiler-Human:        PENDING
internal sealed class JsZonedDateTimeObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7AF851
    // Broiler-Human:        PENDING
    internal JsZonedDateTimeObject(JsObject prototype, BigInteger epochNanoseconds, string timeZone, string calendar)
        : base(prototype)
    {
        EpochNanoseconds = epochNanoseconds;
        TimeZone = timeZone;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9C8259
    // Broiler-Human:        PENDING
    internal BigInteger EpochNanoseconds { get; }

    /// <summary>An available time zone identifier: an IANA name in its database's case, or a normalized offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9A6046
    // Broiler-Human:        PENDING
    internal string TimeZone { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}

/// <summary>A Temporal.PlainYearMonth (s9.4): the first day of the month, or the calendar's reference day.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=985EA8
// Broiler-Human:        PENDING
internal sealed class JsPlainYearMonthObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F52B79
    // Broiler-Human:        PENDING
    internal JsPlainYearMonthObject(JsObject prototype, IsoDate date, string calendar)
        : base(prototype)
    {
        Date = date;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3EDEA2
    // Broiler-Human:        PENDING
    internal IsoDate Date { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}

/// <summary>A Temporal.PlainMonthDay (s10.4): the date in the calendar's reference year.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DD5F04
// Broiler-Human:        PENDING
internal sealed class JsPlainMonthDayObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=724D93
    // Broiler-Human:        PENDING
    internal JsPlainMonthDayObject(JsObject prototype, IsoDate date, string calendar)
        : base(prototype)
    {
        Date = date;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3EDEA2
    // Broiler-Human:        PENDING
    internal IsoDate Date { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}
