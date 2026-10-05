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

namespace Broiler.VM.Profile.JavaScript;

/// <summary>A Temporal.Duration (s7.4): its ten fields, float64-representable integers of one sign.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9D1D24
// Broiler-Human:        PENDING
internal sealed class JsDurationObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6BFE67
    // Broiler-Human:        PENDING
    internal JsDurationObject(JsObject prototype, double[] fields)
        : base(prototype) => Fields = fields;

    /// <summary>[[Years]] to [[Nanoseconds]], in Table 21's order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1D4969
    // Broiler-Human:        PENDING
    internal double[] Fields { get; }
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=C473F5
    // Broiler-Human:        PENDING
    internal JsPlainDateObject(JsObject prototype, JsIsoDate date, string calendar)
        : base(prototype)
    {
        Date = date;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5B923A
    // Broiler-Human:        PENDING
    internal JsIsoDate Date { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}

/// <summary>A Temporal.PlainTime (s4.4).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=38D05D
// Broiler-Human:        PENDING
internal sealed class JsPlainTimeObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5B1E77
    // Broiler-Human:        PENDING
    internal JsPlainTimeObject(JsObject prototype, JsTimeRecord time)
        : base(prototype) => Time = time with { Days = 0 };

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0E9A44
    // Broiler-Human:        PENDING
    internal JsTimeRecord Time { get; }
}

/// <summary>A Temporal.PlainDateTime (s5.4).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BD630F
// Broiler-Human:        PENDING
internal sealed class JsPlainDateTimeObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CD50A1
    // Broiler-Human:        PENDING
    internal JsPlainDateTimeObject(JsObject prototype, JsIsoDateTime dateTime, string calendar)
        : base(prototype)
    {
        DateTime = dateTime with { Time = dateTime.Time with { Days = 0 } };
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A837E7
    // Broiler-Human:        PENDING
    internal JsIsoDateTime DateTime { get; }

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ECFD6F
    // Broiler-Human:        PENDING
    internal JsPlainYearMonthObject(JsObject prototype, JsIsoDate date, string calendar)
        : base(prototype)
    {
        Date = date;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5B923A
    // Broiler-Human:        PENDING
    internal JsIsoDate Date { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}

/// <summary>A Temporal.PlainMonthDay (s10.4): the date in the calendar's reference year.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=DD5F04
// Broiler-Human:        PENDING
internal sealed class JsPlainMonthDayObject : JsObject
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A810F2
    // Broiler-Human:        PENDING
    internal JsPlainMonthDayObject(JsObject prototype, JsIsoDate date, string calendar)
        : base(prototype)
    {
        Date = date;
        Calendar = calendar;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5B923A
    // Broiler-Human:        PENDING
    internal JsIsoDate Date { get; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=3C89E4
    // Broiler-Human:        PENDING
    internal string Calendar { get; }
}
