// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The host's <c>setTimeout</c> for a test's main realm: a timer queue that waits without spinning.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why the host defines it at all.</b> The suite's <c>atomicsHelper.js</c> installs its own
/// <c>setTimeout</c> only where the host has none, and that stand-in is a promise chain that re-queues
/// itself until <c>Date.now()</c> passes the deadline. Every turn allocates, and this profile charges
/// every allocation of an operation to its live bytes, so what such a wait costs was its wall time
/// multiplied by however fast the machine ran it. <c>getReportAsync</c> waits on it for a second
/// whenever a report is not yet there, and whether it is there depends on how quickly another agent
/// ran: <c>Atomics/waitAsync/no-spurious-wakeup-on-add.js</c> exhausted <c>LiveBytes</c> in about one
/// run in six and passed in the rest (JSC-288). A host timer that waits instead of spinning makes the
/// cost of a timeout independent of the clock it waits on.
/// </para>
/// <para>
/// <b>An event loop's order, kept with one pump job.</b> Timers are held here, ordered by deadline and
/// then by the order they were set, and one host job - the pump - serves them. While guest jobs are
/// queued the pump steps behind them, so every microtask runs before any timer, as on an event loop.
/// While the queue is idle and the next timer is not yet due, the pump sleeps at most
/// <see cref="Slice"/> and steps behind the queue again, which hands the profile's drain the turn
/// between two jobs where it settles an <c>Atomics.waitAsync</c> deadline that falls due first
/// (JSD-0041, JSC-267). When a timer is due, its callback runs with the arguments it was given.
/// </para>
/// <para>
/// <b>What it still costs, and why that is bounded.</b> Each step of the pump is one host job, so a
/// timer's wait allocates once per slice rather than once per spin: a few hundred bytes a millisecond
/// against a live-bytes ceiling of tens of megabytes, which the wall clock ends long before it is
/// reached. That is the property the fix is for: no verdict decided on <c>LiveBytes</c> by how long
/// another thread took.
/// </para>
/// <para>
/// <b>It is the conformance harness's, not the profile's.</b> The realm the profile builds publishes no
/// <c>setTimeout</c> (its globals list says so), and no other composition defines one; INTERPRETING.md
/// leaves timers to the host, and the suite's helper is written to defer to one.
/// </para>
/// </remarks>
internal sealed class Test262Timers
{
    /// <summary>The longest the pump sleeps before handing the drain a turn, in milliseconds.</summary>
    internal const int Slice = 1;

    private static readonly System.Diagnostics.Stopwatch Clock = System.Diagnostics.Stopwatch.StartNew();

    private readonly List<(double Due, long Order, JsHostValue Callback, JsHostValue[] Arguments)> timers = [];

    private readonly CancellationToken stopping;

    private long order;

    private bool pumpQueued;

    private Test262Timers(CancellationToken stopping) => this.stopping = stopping;

    /// <summary>Defines <c>setTimeout</c> on <paramref name="realm"/>'s global object, with a queue of its own.</summary>
    internal static void Install(JsHostRealm realm, CancellationToken stopping)
    {
        var timers = new Test262Timers(stopping);

        realm.DefineValue(
            realm.Global,
            "setTimeout",
            realm.NewMethod("setTimeout", timers.SetTimeout, 2),
            JsHostPropertyFlags.Writable | JsHostPropertyFlags.Configurable);
    }

    /// <summary><c>setTimeout(callback, delay, ...arguments)</c>: queue the call, answer <c>undefined</c>.</summary>
    /// <remarks>
    /// A delay that is not a non-negative number is zero, as an event loop clamps it. A callback that is
    /// not callable is the <c>TypeError</c> the invocation raises when the timer is due.
    /// </remarks>
    private JsHostValue SetTimeout(JsHostRealm realm, JsHostValue thisValue, ReadOnlySpan<JsHostValue> arguments)
    {
        var callback = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;
        var delay = arguments.Length > 1 ? realm.ToNumber(arguments[1]) : 0;

        if (double.IsNaN(delay) || delay < 0)
        {
            delay = 0;
        }

        timers.Add((Clock.Elapsed.TotalMilliseconds + delay, order++, callback, arguments.Length > 2 ? arguments[2..].ToArray() : []));
        Queue(realm);
        return JsHostValue.Undefined;
    }

    private void Queue(JsHostRealm realm)
    {
        if (pumpQueued || timers.Count == 0 || stopping.IsCancellationRequested)
        {
            return;
        }

        pumpQueued = true;
        realm.EnqueueJob(() => Pump(realm));
    }

    /// <summary>One step of the pump: yield to queued jobs, wait one slice, or run the timer that is due.</summary>
    private void Pump(JsHostRealm realm)
    {
        pumpQueued = false;

        if (timers.Count == 0 || stopping.IsCancellationRequested)
        {
            return;
        }

        if (realm.HasPendingJobs)
        {
            Queue(realm);
            return;
        }

        var next = 0;

        for (var index = 1; index < timers.Count; index++)
        {
            if (timers[index].Due < timers[next].Due ||
                timers[index].Due == timers[next].Due && timers[index].Order < timers[next].Order)
            {
                next = index;
            }
        }

        var wait = timers[next].Due - Clock.Elapsed.TotalMilliseconds;

        if (wait > 0)
        {
            stopping.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(Math.Min(wait, Slice)));
            Queue(realm);
            return;
        }

        var (_, _, callback, callArguments) = timers[next];
        timers.RemoveAt(next);
        Queue(realm);
        realm.Invoke(callback, JsHostValue.Undefined, callArguments);
    }
}
