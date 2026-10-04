// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           11
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  3/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The running frames an error's <c>stack</c> is captured from, and the capture (JSD-0038).
/// </summary>
/// <remarks>
/// <para>
/// <b>One site per running activation, reused by depth.</b> <see cref="Execute"/> takes a site for
/// every frame it runs - a call, a construction, a generator's or an async function's resumption, a
/// script or a module body - and a value-form direct call takes one too, because it enters a frame
/// without <see cref="Execute"/>. The array grows to the deepest call the instance has made and is
/// never shrunk; a site whose frame has ended keeps no reference to that frame's program or function.
/// </para>
/// <para>
/// <b>Every instruction the dispatch loop runs writes itself into its frame's site</b>, next to the
/// fuel charge every instruction already pays; that is what lets a frame that called out - through a
/// call, a getter, a <c>valueOf</c>, a proxy trap - be placed at the instruction that called. The
/// emitted forms run their instructions through the same loop, one step or one block at a time, for
/// the innermost frame, so the innermost site is theirs; the one instruction emitted code runs
/// without the loop that can leave the frame, a value-form direct call, places the caller's site
/// itself.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    /// <summary>The most frames a captured stack holds, innermost first.</summary>
    /// <remarks>
    /// <b>It is V8's default <c>Error.stackTraceLimit</c></b>, and it bounds what a capture costs: an
    /// error made at the bottom of a deep recursion records ten frames, not the recursion.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=52809F
    // Broiler-Human:        PENDING
    internal const int StackFrameLimit = 10;

    /// <summary>The running frames' sites, outermost first; the first <see cref="siteCount"/> are live.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D530C9
    // Broiler-Human:        PENDING
    private JsStackSite[] sites = new JsStackSite[16];

    /// <summary>
    /// The function `new` named when a built-in is being constructed through `super()`, whose frames
    /// the next capture skips; nothing otherwise.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E16370
    // Broiler-Human:        PENDING
    private JsObject? stackSkipUntil;

    /// <summary>How many sites are live.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BEE831
    // Broiler-Human:        PENDING
    private int siteCount;

    /// <summary>Takes the site of a frame that is about to run.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C787C8
    // Broiler-Falsified-If: a site is taken without being given back when its frame ends, or a taken site names another frame's program, unit or function
    // Broiler-Human:        PENDING
    internal JsStackSite PushSite(
        JsProgram program,
        int unit,
        JsScriptFunction? function,
        string referrer,
        bool construct)
    {
        if (siteCount == sites.Length)
        {
            System.Array.Resize(ref sites, sites.Length * 2);
        }

        var site = sites[siteCount] ??= new JsStackSite();
        site.Program = program;
        site.Unit = unit;
        site.Function = function;
        site.Referrer = referrer;
        site.Construct = construct;
        site.Pc = (int)program.Functions[unit].CodeOffset;
        siteCount++;
        return site;
    }

    /// <summary>Gives back the innermost site, keeping no reference to the frame that held it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F0DB3E
    // Broiler-Human:        PENDING
    internal void PopSite()
    {
        var site = sites[--siteCount];
        site.Program = null;
        site.Function = null;
        site.Referrer = string.Empty;
    }

    /// <summary>The innermost live site.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2E1FDD
    // Broiler-Human:        PENDING
    internal JsStackSite TopSite => sites[siteCount - 1];

    /// <summary>
    /// The running frames, innermost first and at most <see cref="StackFrameLimit"/> of them, each
    /// named and placed as it stands now.
    /// </summary>
    /// <remarks>
    /// <b>A built-in's own frame is not among them</b>: a built-in runs no unit and has no site, so an
    /// error a built-in raises is placed at the guest instruction that called it. Each frame is
    /// charged one unit of fuel.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=958C4C
    // Broiler-Human:        PENDING
    internal JsStackFrame[] CaptureStack()
    {
        // THE FRAMES A `super()` CHAIN RAN IN ARE SKIPPED, the first time a capture is made after a
        // built-in was constructed through one: every site down to the one running the function
        // `new` named, which is skipped too. A chain whose `new.target` runs in no site - a bound
        // function, a proxy - skips nothing.
        var top = siteCount;

        if (stackSkipUntil is { } skip)
        {
            stackSkipUntil = null;

            for (var at = siteCount - 1; at >= 0; at--)
            {
                if (ReferenceEquals(sites[at].Function, skip))
                {
                    top = at;
                    break;
                }
            }
        }

        var count = System.Math.Min(top, StackFrameLimit);

        if (count == 0)
        {
            return [];
        }

        Charge((ulong)count);
        var frames = new JsStackFrame[count];

        for (var at = 0; at < count; at++)
        {
            var site = sites[top - 1 - at];
            var program = site.Program!;
            var placed = program.TryPositionOf(site.Unit, site.Pc, out var line, out var column);

            frames[at] = new JsStackFrame(
                FrameName(site),
                site.Referrer,
                placed ? line : 0,
                placed ? column : 0,
                site.Construct);
        }

        return frames;
    }

    /// <summary>
    /// The name a frame is shown under: the running function's own <c>name</c> when it is a String
    /// data property, otherwise the unit's name; empty for a script, module or eval body.
    /// </summary>
    /// <remarks>
    /// The own property is read without running anything - an accessor named <c>name</c> is not
    /// called - so the name a program assigned by NamedEvaluation (<c>var f = function () {}</c>) is
    /// shown, and no capture can run guest code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=638A5A
    // Broiler-Human:        PENDING
    private static string FrameName(JsStackSite site)
    {
        if (site.Function is { } function &&
            function.TryGetOwnProperty("name", out var property) &&
            !property.IsAccessor &&
            property.Value.Type == JsType.String)
        {
            return property.Value.AsString();
        }

        return site.Program!.Functions[site.Unit].Name;
    }

    /// <summary>
    /// The text of a captured stack: the error's header, as <c>Error.prototype.toString</c> would
    /// render it now, then one line per frame.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The lines are V8's</b>: <c>    at name (place:line:column)</c>, <c>new name</c> for a
    /// construction, and <c>    at place:line:column</c> for an anonymous frame. A frame whose
    /// referrer is empty is placed at <c>&lt;anonymous&gt;</c>.
    /// </para>
    /// <para>
    /// <b>A header that throws is <c>&lt;error&gt;</c></b>, as in V8: reading a stack does not raise
    /// what a getter on the error's <c>name</c> or <c>message</c> raised.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8E6E4A
    // Broiler-Human:        PENDING
    internal string FormatStack(JsObject error, JsStackFrame[] frames)
    {
        string header;

        try
        {
            header = ErrorHeader(error);
        }
        catch (JsThrow)
        {
            header = "<error>";
        }

        var text = new System.Text.StringBuilder(header);

        foreach (var frame in frames)
        {
            text.Append("\n    at ");
            var place = frame.Referrer.Length == 0 ? "<anonymous>" : frame.Referrer;

            if (frame.Line != 0)
            {
                place = place + ":" +
                    frame.Line.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" +
                    frame.Column.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            var name = frame.Name.Length == 0 && frame.Construct ? "<anonymous>" : frame.Name;

            if (name.Length == 0)
            {
                text.Append(place);
            }
            else
            {
                text.Append(frame.Construct ? "new " : string.Empty).Append(name).Append(" (").Append(place).Append(')');
            }
        }

        Charge((ulong)text.Length);
        return text.ToString();
    }

    /// <summary><c>Error.prototype.toString</c>'s answer for <paramref name="error"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=34C00F
    // Broiler-Human:        PENDING
    internal string ErrorHeader(JsObject error)
    {
        var receiver = JsValue.Object(error);
        var namePart = GetProperty(receiver, "name");
        var messagePart = GetProperty(receiver, "message");
        var name = namePart.Type == JsType.Undefined ? "Error" : ToStringValue(namePart);
        var message = messagePart.Type == JsType.Undefined ? string.Empty : ToStringValue(messagePart);

        if (name.Length == 0)
        {
            return message;
        }

        return message.Length == 0 ? name : name + ": " + message;
    }
}

/// <summary>One running frame, as an error's stack sees it (JSD-0038).</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D61FA0
// Broiler-Human:        PENDING
internal sealed class JsStackSite
{
    /// <summary>The program the frame runs, or nothing while the site is free.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7896B7
    // Broiler-Human:        PENDING
    internal JsProgram? Program;

    /// <summary>The unit the frame runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DAF100
    // Broiler-Human:        PENDING
    internal int Unit;

    /// <summary>The function running, or nothing for a script, module or eval body.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5D40BF
    // Broiler-Human:        PENDING
    internal JsScriptFunction? Function;

    /// <summary>The script or module the frame belongs to, empty when it was placed nowhere.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CA5791
    // Broiler-Human:        PENDING
    internal string Referrer = string.Empty;

    /// <summary>Whether the frame was entered by a construction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9AEDDE
    // Broiler-Human:        PENDING
    internal bool Construct;

    /// <summary>The instruction the interpreter is running in this frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4E6E02
    // Broiler-Human:        PENDING
    internal int Pc;

}

/// <summary>One captured frame of an error's stack: its name, its place and how it was entered.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6B9476
// Broiler-Human:        PENDING
internal readonly record struct JsStackFrame(
    string Name, string Referrer, uint Line, uint Column, bool Construct);

/// <summary>
/// An object with an <c>[[ErrorData]]</c> slot, and the stack captured when it was made (JSD-0038).
/// </summary>
/// <remarks>
/// <b>The stack is held as frames until it is first read</b>, and only then rendered, so an error
/// made and caught for control flow pays for ten small records and no string. Once read or assigned it
/// is a value, and stays the value it was: a later change to <c>message</c> is not reflected, as in
/// V8.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=621462
// Broiler-Human:        PENDING
internal sealed class JsErrorObject : JsObject
{
    /// <summary>Creates an error on <paramref name="prototype"/>, with no stack yet.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8B1649
    // Broiler-Human:        PENDING
    internal JsErrorObject(JsObject? prototype)
        : base(prototype, "Error")
    {
    }

    /// <summary>The frames captured when the error was made, until the stack is first read or assigned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DD01BA
    // Broiler-Human:        PENDING
    internal JsStackFrame[]? Frames { get; set; }

    /// <summary>The stack's value once it was read or assigned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AF846B
    // Broiler-Human:        PENDING
    internal JsValue StackValue { get; set; } = JsValue.Undefined;
}
