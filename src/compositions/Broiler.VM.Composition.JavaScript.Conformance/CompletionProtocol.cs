namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// How a run's settlement is read: from the markers it emitted, and from nothing else.
/// </summary>
/// <remarks>
/// <para>
/// <b>A result is evidence only when a marker is there.</b> That is the whole of the protocol and
/// it is the property roadmap section 14 asks for: a harness that scored by whether the engine
/// returned would count a program that never finished, a program that finished twice, and a
/// program that finished once as the same thing. Reading the markers instead makes the three
/// distinguishable, and the distinction survives into every total because the kind is recorded on
/// every result rather than on the asynchronous ones.
/// </para>
/// <para>
/// <b>Nothing in `broiler.javascript.slice` can emit more or fewer than one marker.</b> The
/// manifest admits no promise, no generator and no asynchronous function, so a synchronous
/// invocation settles exactly once and a refusal settles not at all. Two of the four kinds are
/// therefore reachable from no source this front end accepts - which is a reason to test the
/// classifier against recorded marker sequences, not a reason to leave it unwritten. The day a
/// suspension is admitted, the fixture that produces one is the only thing that has to be
/// written; this file does not change.
/// </para>
/// </remarks>
internal static class CompletionProtocol
{
    /// <summary>The marker a run emits when the program completed.</summary>
    internal const string Completed = "completed";

    /// <summary>The prefix of the marker a run emits when the program reported a failure.</summary>
    internal const string FailurePrefix = "failed:";

    /// <summary>Reads one run's settlement out of the markers it emitted.</summary>
    /// <remarks>
    /// <b>Two settlements are a failure and not a first-one-wins pass.</b> A program that reports
    /// twice ran code after the point it claimed to be finished, so the outcome recorded is not the
    /// outcome reached - which is exactly the property this protocol exists to restore.
    /// </remarks>
    internal static (CompletionKind Kind, string Detail) Classify(IReadOnlyList<string> markers)
    {
        var completions = 0;
        var failures = new List<string>();

        foreach (var raw in markers)
        {
            var marker = raw.Trim();

            if (string.Equals(marker, Completed, StringComparison.Ordinal))
            {
                completions++;
            }
            else if (marker.StartsWith(FailurePrefix, StringComparison.Ordinal))
            {
                failures.Add(marker[FailurePrefix.Length..].Trim());
            }
        }

        var settlements = completions + failures.Count;

        if (settlements == 0)
        {
            return (CompletionKind.NeverSettled, "no completion marker was emitted");
        }

        if (settlements > 1)
        {
            return (
                CompletionKind.CompletedTwice,
                $"the run settled {settlements} times" +
                    (failures.Count == 0 ? string.Empty : $", first reporting `{failures[0]}`"));
        }

        return failures.Count == 1
            ? (CompletionKind.ReportedFailure, failures[0])
            : (CompletionKind.Completed, string.Empty);
    }

    /// <summary>
    /// What a case becomes when something escapes the engine rather than being answered by it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A failure of the case and never a crash of the run.</b> An exception escaping a pass
    /// whose contract is that it refuses is a defect worth reporting loudly, and a measurement tool
    /// that stops at the first one measures nothing: the census that preceded this harness lost a
    /// whole run to a single lone surrogate. The case is scored as a failure carrying the exception
    /// type, the run continues, and the type name is in the report where a reader can group by it.
    /// </para>
    /// <para>
    /// <b>The type name and not the message.</b> A message carries paths and offsets that differ
    /// between machines, so grouping by it produces one group per case; a type name groups the way
    /// a defect actually recurs. The message is kept on the detail, where nothing counts it.
    /// </para>
    /// </remarks>
    internal static Observation Escaped(Exception failure) => new(
        ConformanceStatus.Failed,
        CompletionKind.NeverSettled,
        "escaped " + failure.GetType().Name,
        $"{failure.GetType().Name}: {failure.Message}");
}
