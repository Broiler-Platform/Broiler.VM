namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// What one conformance case did, in the four words a total may be built from.
/// </summary>
/// <remarks>
/// There is no fifth value and in particular there is no "error". A case whose engine fell over is
/// <see cref="Failed"/> with a detail saying so, because a run that could report a category
/// outside the totals could report a defect as something other than a failure - which is the one
/// thing roadmap section 14 asks this harness to make impossible.
/// </remarks>
internal enum ConformanceStatus
{
    /// <summary>The case reached the verdict its test declared.</summary>
    Passed,

    /// <summary>The case reached some other verdict. Every way of being wrong lands here.</summary>
    Failed,

    /// <summary>The case was selected and deliberately not executed. Never a pass.</summary>
    Skipped,

    /// <summary>The case did not finish inside its allowance. Never a pass.</summary>
    TimedOut,
}

/// <summary>
/// How a case's completion was observed, on every result and not only on an asynchronous one.
/// </summary>
/// <remarks>
/// <para>
/// The four kinds are roadmap section 14's, and they are a property of the RUN rather than of the
/// test: a harness that scores by exit status alone cannot tell a test that finished from one that
/// never settled, and both come back as passes. Recording the kind beside every status is what
/// makes the two distinguishable in a total.
/// </para>
/// <para>
/// <b>Two of the four are unreachable from `broiler.javascript.slice` and that is stated rather
/// than hidden.</b> This manifest admits no promise, no generator and no asynchronous function, so
/// nothing it can express settles twice or fails to settle at all. They are not dropped: the
/// classifier that reads them is exercised by recorded marker sequences in the harness's own
/// regression suite, and the day the manifest grows a suspension the fixture that produces one is
/// the only thing that has to be written.
/// </para>
/// </remarks>
internal enum CompletionKind
{
    /// <summary>Settled exactly once, reporting completion.</summary>
    Completed,

    /// <summary>Settled exactly once, reporting a failure of its own.</summary>
    ReportedFailure,

    /// <summary>Never settled. A failure, not a pass with a caveat.</summary>
    NeverSettled,

    /// <summary>Settled more than once, so the recorded outcome is not the reached one.</summary>
    CompletedTwice,
}

/// <summary>
/// The three ways this profile can be handed a program, each reporting totals of its own.
/// </summary>
/// <remarks>
/// They are three because the profile has three, not because another suite has three. A script and
/// a module are two parse goals of one lowering; <see cref="Raw"/> is an artifact presented as
/// bytes with no lowering consulted at all, which is the only mode an execution-only image could
/// ever run and is therefore the one whose totals say something about that image.
/// </remarks>
internal enum HostMode
{
    /// <summary>Source lowered under the script goal.</summary>
    Script,

    /// <summary>Source lowered under the module goal.</summary>
    Module,

    /// <summary>Artifact bytes, verified and run with no lowering.</summary>
    Raw,
}

/// <summary>
/// The closed set of ways a run is misconfigured. Each is a failure of the run, never a smaller
/// total.
/// </summary>
/// <remarks>
/// <para>
/// Roadmap section 14 names five and then states a sixth behaviour without naming it - "removing
/// one shard's report must produce incomplete coverage, not a smaller total". A behaviour a run
/// must have is a member this enumeration must carry, so <see cref="IncompleteShardCoverage"/> is
/// here and correction JSC-51 records that this set is the roadmap's five plus that one.
/// </para>
/// <para>
/// <b>What is deliberately NOT a member: a self-check mismatch.</b> The self-check runs before a
/// shard is configured at all, so a mismatch has no run to be a property of; it stops the process
/// on its own exit code. Folding it in here would let a reader believe a run had been configured
/// and had then gone wrong, when in fact nothing ran.
/// </para>
/// </remarks>
internal enum ConfigurationFailure
{
    /// <summary>Shard reports disagree about a field every shard of one run must share.</summary>
    InconsistentShardConfiguration,

    /// <summary>No suite revision was pinned. A branch name is not a pin, and neither is nothing.</summary>
    MissingSuiteRevision,

    /// <summary>A host mode the run declared reported nothing, or one it did not declare reported something.</summary>
    IncompleteHostModeCoverage,

    /// <summary>The selection pipeline admitted no test at all.</summary>
    EmptySelection,

    /// <summary>Tests were selected and none of them ran.</summary>
    NoExecutedTests,

    /// <summary>A merge was handed fewer shard reports than the run declared shards.</summary>
    IncompleteShardCoverage,
}

/// <summary>
/// The process exit codes this harness uses, so a caller can tell the four apart.
/// </summary>
/// <remarks>
/// A conformance failure and a broken harness must not share an exit code: the first is the
/// measurement working and the second is the measurement being unreadable, and a caller that saw
/// one number for both would retry the wrong one.
/// </remarks>
internal static class ExitCodes
{
    /// <summary>Everything asked for happened and nothing failed.</summary>
    internal const int Ok = 0;

    /// <summary>Cases failed, or the run was misconfigured. The measurement is readable.</summary>
    internal const int Failed = 1;

    /// <summary>The harness could not be invoked as asked. Nothing was measured.</summary>
    internal const int Usage = 2;

    /// <summary>
    /// The self-check or the harness's own regression suite disagreed with its declared verdicts.
    /// </summary>
    /// <remarks>
    /// Its own number because it is the one result that says nothing about the engine. A shard
    /// that reports this has measured nothing and its totals must not be merged.
    /// </remarks>
    internal const int HarnessDefect = 3;
}
