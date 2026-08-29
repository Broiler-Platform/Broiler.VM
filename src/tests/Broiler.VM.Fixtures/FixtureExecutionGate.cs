namespace Broiler.VM.Fixtures;

/// <summary>
/// A rendezvous a test uses to hold a fixture executor inside a step, so that a race can be
/// arranged rather than hoped for.
/// </summary>
/// <remarks>
/// <para>
/// Every concurrency claim VM-4 makes is about what happens while a profile is executing: a
/// disposal that arrives mid-step, a second thread calling into a runtime whose capability is
/// running, an operation resumed on a thread the profile did not start on. A test that started a
/// thread and slept would be asserting that the machine was slow enough, which is the shape of a
/// test that passes on the author's laptop and fails in a lane.
/// </para>
/// <para>
/// It is an ordinary object handed to one descriptor rather than a static, because xunit runs test
/// classes in parallel: a process-global gate would make two independent tests each other's
/// problem. One gate belongs to one composition and is reachable from nothing else.
/// </para>
/// </remarks>
public sealed class FixtureExecutionGate
{
    private readonly System.Threading.ManualResetEventSlim entered = new(false);
    private readonly System.Threading.ManualResetEventSlim released = new(false);
    private int entries;

    /// <summary>Which step kinds this gate holds.</summary>
    public FixtureGatePoint HoldAt { get; set; } = FixtureGatePoint.None;

    /// <summary>How many times a held step has been entered.</summary>
    public int Entries => System.Threading.Volatile.Read(ref entries);

    /// <summary>Blocks until a held step has been entered, or the wait times out.</summary>
    /// <remarks>
    /// The timeout is the test's own deadlock detector. A gate that is never entered means the
    /// execution path under test did not run at all, and reporting that as a timeout is more useful
    /// than hanging the suite.
    /// </remarks>
    public bool WaitForEntry(System.TimeSpan timeout) => entered.Wait(timeout);

    /// <summary>Lets a held step continue.</summary>
    public void Release() => released.Set();

    /// <summary>Called at a gate point by the fixture executor, or by a host handler a test wired.</summary>
    public void Reached(FixtureGatePoint point)
    {
        if ((HoldAt & point) == 0)
        {
            return;
        }

        System.Threading.Interlocked.Increment(ref entries);
        entered.Set();

        // A bounded wait, never an unbounded one. A gate a test forgets to release must fail that
        // test rather than wedge the whole run, and thirty seconds is far longer than any step
        // this suite performs.
        released.Wait(System.TimeSpan.FromSeconds(30));
    }
}

/// <summary>Where a <see cref="FixtureExecutionGate"/> may hold a fixture executor.</summary>
[System.Flags]
public enum FixtureGatePoint
{
    /// <summary>Hold nowhere.</summary>
    None = 0,

    /// <summary>Hold on entry to an invocation, before the first instruction.</summary>
    Invoke = 1,

    /// <summary>Hold on entry to instantiation.</summary>
    Instantiate = 2,

    /// <summary>Hold inside the terminal-unwind entry point.</summary>
    Unwind = 4,

    /// <summary>Hold inside a host capability handler.</summary>
    Capability = 8,
}
