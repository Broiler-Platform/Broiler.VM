// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           2
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The engine's half of <c>Atomics.wait</c>, <c>Atomics.waitAsync</c> and <c>Atomics.notify</c>: whether
/// this agent may block, the blocking wait, and the asynchronous waiters a host drain settles
/// (JSD-0041 section 4).
/// </summary>
/// <remarks>
/// <para>
/// <b>A blocking wait is bounded by the operation, never by the program.</b> It sleeps on its block's
/// gate in slices and, between slices, asks the same two questions every charge asks - cancellation
/// and the meter's poll, which owns the wall clock - so a wait with no timeout ends when the operation's
/// allowance does. It spends no fuel while it sleeps.
/// </para>
/// <para>
/// <b>An asynchronous waiter is settled only at a host drain</b>, as a <c>FinalizationRegistry</c>
/// cleanup is (JSD-0029): a notification marks it, a timeout expires it, and the drain turns either
/// into the settlement of its promise, which queues its reactions as ordinary jobs. A drain whose
/// queue is empty while a waiter of this agent can still be settled - it has a deadline, or its block
/// has crossed to another agent that may notify it - waits for it, bounded as a blocking wait is.
/// </para>
/// <para>
/// <b>A drain also settles what is due between two jobs</b> <i>(corrected: JSC-267)</i>. Settling a
/// waiter is a job the host enqueues, and the edition leaves its order against promise jobs to the
/// host; settling only once the queue was empty let a program whose jobs kept queueing more - the
/// suite's <c>setTimeout</c> stand-in does exactly that - starve every timeout it was waiting for.
/// </para>
/// <para>
/// <b>Deadlines are read on the high-resolution clock</b> <i>(corrected: JSC-267)</i>. They were
/// read on <see cref="System.Environment.TickCount64"/>, whose granularity let a wait end a few
/// milliseconds before its timeout by a finer clock, which is what a program measures it with.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7BBE7E
// Broiler-Falsified-If: a blocking wait runs where the host did not say it may block, or an asynchronous waiter settles outside a host drain
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    /// <summary>
    /// The agent's <c>[[CanBlock]]</c>: whether <c>Atomics.wait</c> may suspend it. A composition's
    /// host surface decides (<see cref="IJsHostAgentPolicy"/>); otherwise it is <see langword="false"/>,
    /// as for an event loop.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4984AD
    // Broiler-Human:        PENDING
    internal bool CanBlock { get; set; }

    /// <summary>The asynchronous waiters this agent made and has not settled, with their blocks.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=978981
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(JsSharedBlock Block, JsWaiter Waiter)> asyncWaiters = [];

    /// <summary>How long a blocking wait sleeps between two polls of the allowance, in milliseconds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1C5049
    // Broiler-Human:        PENDING
    private const int WaitSlice = 20;

    /// <summary>
    /// Asks cancellation and the meter, as a charge's poll does, without spending fuel: what a wait
    /// does between slices.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D25EAF
    // Broiler-Human:        PENDING
    private void PollWhileWaiting()
    {
        if (cancellation.IsCancellationRequested)
        {
            throw new JsAbort(JsAbortKind.Cancelled, "cancellation was requested");
        }

        if (!meter.Poll())
        {
            throw new JsAbort(JsAbortKind.Exhausted, "a budget dimension was reached");
        }
    }

    /// <summary>The deadline <paramref name="timeout"/> milliseconds from now, in clock ticks, or never.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=09C408
    // Broiler-Human:        PENDING
    private static long DeadlineAfter(double timeout)
    {
        var ticks = System.Math.Ceiling(timeout * System.Diagnostics.Stopwatch.Frequency / 1000.0);

        return double.IsPositiveInfinity(ticks) || ticks >= long.MaxValue / 4
            ? long.MaxValue
            : System.Diagnostics.Stopwatch.GetTimestamp() + (long)ticks;
    }

    /// <summary>
    /// How many whole milliseconds remain until <paramref name="deadline"/>, rounded up, so that a
    /// sleep of that length never ends before it; zero or less once it has passed.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BC250A
    // Broiler-Human:        PENDING
    private static long MillisecondsUntil(long deadline)
    {
        var ticks = deadline - System.Diagnostics.Stopwatch.GetTimestamp();

        return ticks <= 0
            ? 0
            : (long)System.Math.Ceiling(ticks * 1000.0 / System.Diagnostics.Stopwatch.Frequency);
    }

    /// <summary>
    /// The blocking half of <c>DoWait</c>: answers <c>"not-equal"</c>, <c>"ok"</c> or <c>"timed-out"</c>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=3C03E7
    // Broiler-Falsified-If: a blocking wait runs in an agent whose host did not say it may block, outlives its operation's allowance, or leaves its waiter in the list
    // Broiler-Human:        PENDING
    internal string WaitBlocking(JsSharedBlock block, int byteIndex, int width, long expected, double timeout)
    {
        var waiter = new JsWaiter(byteIndex, DeadlineAfter(timeout), null);

        lock (block.Gate)
        {
            if (JsAtomicAccess.Load(block.Bytes, byteIndex, width, block.Gate) != expected)
            {
                return "not-equal";
            }

            block.Waiters.Add(waiter);

            try
            {
                while (!waiter.Notified)
                {
                    var remaining = waiter.Deadline == long.MaxValue
                        ? WaitSlice
                        : MillisecondsUntil(waiter.Deadline);

                    if (remaining <= 0)
                    {
                        break;
                    }

                    System.Threading.Monitor.Wait(block.Gate, (int)System.Math.Min(remaining, WaitSlice));

                    if (!waiter.Notified)
                    {
                        PollWhileWaiting();
                    }
                }
            }
            finally
            {
                if (!waiter.Notified)
                {
                    block.Waiters.Remove(waiter);
                }
            }

            return waiter.Notified ? "ok" : "timed-out";
        }
    }

    /// <summary>
    /// The asynchronous half of <c>DoWait</c>: answers the result object, settled now or holding a
    /// promise a notification or the timeout settles at a host drain.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DEFAC9
    // Broiler-Human:        PENDING
    internal JsValue WaitAsync(JsSharedBlock block, int byteIndex, int width, long expected, double timeout)
    {
        var result = new JsObject(Realm.ObjectPrototype);

        lock (block.Gate)
        {
            if (JsAtomicAccess.Load(block.Bytes, byteIndex, width, block.Gate) != expected)
            {
                result.DefineOrdinary("async", JsValue.False);
                result.DefineOrdinary("value", JsValue.String("not-equal"));
                return JsValue.Object(result);
            }

            if (timeout == 0)
            {
                result.DefineOrdinary("async", JsValue.False);
                result.DefineOrdinary("value", JsValue.String("timed-out"));
                return JsValue.Object(result);
            }

            var promise = Realm.NewAsyncPromise();
            var waiter = new JsWaiter(byteIndex, DeadlineAfter(timeout), promise);
            block.Waiters.Add(waiter);
            asyncWaiters.Add((block, waiter));

            result.DefineOrdinary("async", JsValue.True);
            result.DefineOrdinary("value", JsValue.Object(promise));
            return JsValue.Object(result);
        }
    }

    /// <summary>
    /// <c>Atomics.notify</c>'s critical section: removes up to <paramref name="count"/> waiters on
    /// <paramref name="byteIndex"/>, in the order they began waiting, and answers how many.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=6D79B7
    // Broiler-Falsified-If: a notification wakes a waiter on another index or block, or more waiters than it was asked to
    // Broiler-Human:        PENDING
    internal static int Notify(JsSharedBlock block, int byteIndex, double count)
    {
        var woken = 0;

        lock (block.Gate)
        {
            for (var at = 0; at < block.Waiters.Count && woken < count; at++)
            {
                var waiter = block.Waiters[at];

                if (waiter.ByteIndex != byteIndex)
                {
                    continue;
                }

                waiter.Notified = true;
                block.Waiters.RemoveAt(at--);
                woken++;
            }

            if (woken != 0)
            {
                System.Threading.Monitor.PulseAll(block.Gate);
            }
        }

        return woken;
    }

    /// <summary>
    /// Settles this agent's asynchronous waiters that were notified or have timed out, waiting first,
    /// when <paramref name="wait"/> is set and nothing is due, for the earliest deadline or for a
    /// notification from another agent; answers whether any was settled. Only a host drain or step
    /// calls it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=84E69B
    // Broiler-Falsified-If: an asynchronous waiter's promise settles anywhere but at a host drain or step, or a drain waits past its allowance
    // Broiler-Human:        PENDING
    internal bool SettleWaiters(bool wait)
    {
        while (asyncWaiters.Count != 0)
        {
            var settled = false;
            var earliest = long.MaxValue;
            var notifiable = false;
            (JsSharedBlock Block, JsWaiter Waiter)? watched = null;
            var now = System.Diagnostics.Stopwatch.GetTimestamp();

            for (var at = 0; at < asyncWaiters.Count; at++)
            {
                var (block, waiter) = asyncWaiters[at];
                string? outcome = null;

                lock (block.Gate)
                {
                    if (waiter.Notified)
                    {
                        outcome = "ok";
                    }
                    else if (waiter.Deadline <= now)
                    {
                        block.Waiters.Remove(waiter);
                        outcome = "timed-out";
                    }
                }

                if (outcome is null)
                {
                    earliest = System.Math.Min(earliest, waiter.Deadline);

                    if (block.Crossed)
                    {
                        notifiable = true;
                        watched ??= (block, waiter);
                    }

                    continue;
                }

                asyncWaiters.RemoveAt(at--);
                Charge(1);
                Realm.SettleAsyncPromise(this, waiter.Promise!, JsValue.String(outcome), rejected: false);
                settled = true;
            }

            // NOTHING IS DUE, AND NOTHING EVER WILL BE, when no waiter has a deadline and none is on a
            // block another agent holds: in one agent, nothing but this agent could notify it.
            if (settled || !wait || (earliest == long.MaxValue && !notifiable))
            {
                return settled;
            }

            // NOTHING IS DUE AND A DEADLINE OR A NOTIFICATION IS COMING: wait for it in slices,
            // bounded by the operation, and look again.
            var remaining = earliest == long.MaxValue ? WaitSlice : MillisecondsUntil(earliest);

            if (remaining > 0)
            {
                var slice = (int)System.Math.Min(remaining, WaitSlice);

                // A WAITER ANOTHER AGENT MAY NOTIFY IS WATCHED ON ITS BLOCK'S LOCK, which a
                // notification pulses, so the drain wakes when it is notified rather than at the end
                // of the slice; any other waiter is due only at its deadline.
                if (watched is { } pair)
                {
                    lock (pair.Block.Gate)
                    {
                        if (!pair.Waiter.Notified)
                        {
                            System.Threading.Monitor.Wait(pair.Block.Gate, slice);
                        }
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(slice);
                }

                PollWhileWaiting();
            }
        }

        return false;
    }
}
