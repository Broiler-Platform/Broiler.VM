// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  7/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>Adds one job to the queue.</summary>
    /// <remarks>
    /// <para>
    /// <b>Enqueueing is charged.</b> A program that enqueues without bound is a program that has
    /// bought unbounded future work with a bounded present, and the charge is what makes the queue
    /// a thing the allowance covers rather than a hole beside it.
    /// </para>
    /// <para>
    /// <b>The job keeps the script or module that was active when it was enqueued</b> (JSD-0024
    /// section 20.3): HostEnqueuePromiseJob requires it to be the active one again when the job
    /// runs, so eval code or a <c>Function</c> a job's built-in callable evaluates resolves its
    /// <c>import()</c> against the code that queued the job, not against nothing.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=A1A336
    // Broiler-Human:        PENDING
    internal void EnqueueJob(JsValue callable, JsValue[] arguments)
    {
        Charge(1);
        Retain(64);
        jobs.Enqueue((callable, arguments, activeReferrer));
    }

    /// <summary>
    /// Runs every job that is due, and every job those enqueue, until none is left.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The host decides when this happens and this profile never decides for it.</b> An
    /// embedding that runs one script and stops, one that runs several in one realm, and one that
    /// interleaves them with its own work all want different drain points, and a queue drained
    /// implicitly at a point nobody stated is a behaviour no embedder can reason about. The host
    /// asks by invoking the reserved entry point named in <see cref="JsExecution"/>.
    /// </para>
    /// <para>
    /// <b>A job that never settles is a resource exhaustion and not a hang.</b> A job may enqueue
    /// another job, so this loop is not bounded by the queue's length at entry; what bounds it is
    /// the allowance, charged per job here and per instruction inside each one. A program whose
    /// jobs enqueue jobs for ever spends its fuel and the operation ends naming <c>Fuel</c> —
    /// which is the same answer a program that loops for ever gets, and deliberately so.
    /// </para>
    /// <para>
    /// <b>A job that throws does not stop the drain.</b> The language says an unhandled rejection
    /// is the host's business and that the queue continues; the thrown value is carried out to the
    /// host through the return value rather than being swallowed, and the remaining jobs still run.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=21DD26
    // Broiler-Falsified-If: a job runs at a point the host did not ask for, or an endless queue is a hang rather than an exhaustion
    // Broiler-Human:        PENDING
    internal JsValue DrainJobs()
    {
        var first = JsValue.Undefined;
        var faulted = false;

        while (jobs.Count != 0)
        {
            Charge(1);
            var (callable, arguments, referrer) = jobs.Dequeue();

            try
            {
                _ = CallJob(callable, arguments, referrer);
            }
            catch (JsThrow thrown)
            {
                if (!faulted)
                {
                    faulted = true;
                    first = thrown.Value;
                }
            }

            // A WAITER THAT IS DUE IS SETTLED BETWEEN TWO JOBS, as a job the host enqueued: a queue
            // that keeps refilling itself must not starve the timeouts it waits on (JSC-267).
            if (asyncWaiters.Count != 0)
            {
                _ = SettleWaiters(wait: false);
            }
        }

        if (faulted)
        {
            throw new JsThrow(first, Render(first));
        }

        return JsValue.Undefined;
    }

    /// <summary>
    /// Runs at most one due job and says whether the queue still holds anything.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the same queue and the same jobs as <see cref="DrainJobs"/>; what differs is who
    /// decides when the next one runs.</b> A drain runs the queue to exhaustion inside one
    /// operation, which is what a host that wants a script settled asks for. A host that wants an
    /// event loop of its own — one that can interleave its work with the guest's, or stop between
    /// turns and never resume — needs the turn to be the unit, and a queue drained to exhaustion
    /// cannot give it one.
    /// </para>
    /// <para>
    /// <b>A job that throws does not stop the stepping</b>, exactly as it does not stop a drain: the
    /// value is carried out and the queue keeps its remaining jobs, so a host stepping through a
    /// program sees the same sequence of faults, one at a time, that a drain would have folded into
    /// the first.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=FB280D
    // Broiler-Falsified-If: more than one job runs in a step, or a step reports a queue state the queue does not have
    // Broiler-Human:        PENDING
    internal bool StepOneJob(out JsValue thrown)
    {
        thrown = JsValue.Undefined;

        if (jobs.Count == 0)
        {
            return false;
        }

        Charge(1);
        var (callable, arguments, referrer) = jobs.Dequeue();

        try
        {
            _ = CallJob(callable, arguments, referrer);
        }
        catch (JsThrow raised)
        {
            thrown = raised.Value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Calls one dequeued job with the script or module that queued it as the active one.
    /// </summary>
    /// <remarks>
    /// A job whose callable is a guest function enters a frame that sets its own referrer anyway; a
    /// built-in one - <c>eval</c>, the <c>Function</c> constructor, a host function - enters none,
    /// and this is what it then sees. The previous referrer is restored however the job ends.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B91E1A
    // Broiler-Human:        PENDING
    private JsValue CallJob(JsValue callable, JsValue[] arguments, string referrer)
    {
        var outerReferrer = activeReferrer;
        activeReferrer = referrer;

        try
        {
            return Call(callable, JsValue.Undefined, arguments);
        }
        finally
        {
            activeReferrer = outerReferrer;
        }
    }

    /// <summary>
    /// Drops every queued job without running any of it.
    /// </summary>
    /// <remarks>
    /// The terminal unwind's whole of the work, and it deliberately runs <b>no guest code</b>:
    /// roadmap section 12 says the unwind must run nothing able to request a load or to suspend,
    /// and the only way to promise that about a queue of guest callables is not to call them.
    /// Whatever they would have done is not done, which is what abandoning an operation means.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=59F781
    // Broiler-Falsified-If: a queued job runs during an unwind
    // Broiler-Human:        PENDING
    internal int DropPendingJobs()
    {
        var dropped = jobs.Count;
        jobs.Clear();

        // A DROPPED CLEANUP JOB LEAVES ITS REGISTRATIONS MARKED, and the next sweep - if the instance
        // is ever drained again - queues a fresh one for them (JSD-0029 section 4.5).
        ForgetQueuedCleanups();
        return dropped;
    }
}
