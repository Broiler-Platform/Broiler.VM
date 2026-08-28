namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The release gate: everything that must be true of the review record before a package leaves this
/// component.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why this is separate from the ordinary gate.</b> A bare <c>dotnet test</c> asserts that the
/// generated artefacts are what the generator would write. That is a check on the RECORD and it is
/// deliberately silent about what the record says: this component is built while nothing in it has
/// been read, which ledger update rule 8 permits and which the ordinary gate must therefore accept.
/// Publishing is the irreversible act, and there the record's CONTENT is the thing under test.
/// </para>
/// <para>
/// <b>What it asserts, and why each clause is here.</b> The five clauses are the five ways the
/// record can be green and worthless.
/// </para>
/// <list type="number">
/// <item>
/// A generated artefact on disk is not what the generator would write, so the record a reader is
/// about to trust describes a tree that is not this one.
/// </item>
/// <item>
/// An annotation does not parse, or states a value outside its vocabulary, or is attached to no
/// declaration. This is the review SYNTAX: a line nothing can read is a decision nothing recorded.
/// </item>
/// <item>
/// A recorded fingerprint is not the fingerprint of the declaration it sits on, or is still the
/// placeholder. The state machine already turns that into <c>STALE</c> or <c>AI_ASSESSED</c>, and
/// this states it as its own fact so a publish failure names the arithmetic rather than the state.
/// </item>
/// <item>
/// A unit the assessment puts at the top of the security vocabulary carries no falsification
/// criterion, so nothing at the declaration says what would make it wrong.
/// </item>
/// <item>
/// A relevant unit is in a state that blocks a release - anything but <c>VERIFIED</c>. This is the
/// clause the other four exist to make meaningful: they establish that the states can be believed.
/// </item>
/// </list>
/// <para>
/// <b>Why it is armed by an environment variable.</b> Every relevant unit in this component is
/// <c>HUMAN_PENDING</c> today, so a gate that ran unconditionally would fail the suite on a tree
/// that is exactly as the owner ruled it may be. The variable is set by the publish lane and by
/// nothing else, so the gate is armed precisely where the irreversible act happens. The FUNCTION is
/// not conditional on anything: rule J11 drives witness inputs through <see cref="Blockers"/> in
/// both directions whatever the environment says, so the gate is under test on every run and only
/// its application to this tree is deferred.
/// </para>
/// </remarks>
internal static class AssuranceRelease
{
    /// <summary>Set to <c>1</c> to make the suite assert the release gate over this checkout.</summary>
    internal const string GateVariable = "BROILER_ASSURANCE_RELEASE";

    internal static bool GateRequested =>
        string.Equals(Environment.GetEnvironmentVariable(GateVariable), "1", StringComparison.Ordinal);

    /// <summary>
    /// Every reason this tree may not be published, most structural first. Empty means the record
    /// says what a publish needs it to say - which is not the same as the component being correct.
    /// </summary>
    internal static IReadOnlyList<string> Blockers(AssurancePlan plan)
    {
        var blockers = new List<string>();

        blockers.AddRange(AssuranceGenerator.StaleArtefacts(plan.Artefacts));
        blockers.AddRange(SyntaxBlockers(plan));
        blockers.AddRange(FingerprintBlockers(plan.Units));
        blockers.AddRange(AssuranceScanner.MissingFalsificationCriteria(plan.Units));
        blockers.AddRange(UnresolvedBlockers(plan.Units));

        return blockers;
    }

    /// <summary>
    /// Every annotation this system cannot read, and every assurance comment attached to nothing.
    /// </summary>
    internal static IEnumerable<string> SyntaxBlockers(AssurancePlan plan)
    {
        var byFile = plan.Units
            .GroupBy(static unit => unit.File.RelativePath, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToArray(), StringComparer.Ordinal);

        foreach (var file in plan.Files)
        {
            var units = byFile.TryGetValue(file.RelativePath, out var found) ? found : [];

            foreach (var orphan in AssuranceScanner.OrphanAnnotations(file, units))
            {
                yield return orphan;
            }
        }

        foreach (var unit in plan.Units.Where(static unit => unit.Annotation is not null))
        {
            foreach (var problem in unit.Annotation!.VocabularyProblems())
            {
                yield return $"{unit.Where}: {problem}";
            }
        }
    }

    /// <summary>
    /// Every unit whose annotation records a fingerprint that is not the one its declaration
    /// hashes to now.
    /// </summary>
    /// <remarks>
    /// An <c>EXEMPT=</c> line records no fingerprint and is not asked for one. A <c>STALE</c> line
    /// is reported by the clause below rather than here: its AI-line fingerprint is current by
    /// construction after a generation, and what is out of date is the decision, not the arithmetic.
    /// </remarks>
    internal static IEnumerable<string> FingerprintBlockers(IEnumerable<AssuranceUnit> units)
    {
        foreach (var unit in units)
        {
            if (unit.Annotation is not { ExemptReason: null } annotation)
            {
                continue;
            }

            var recorded = annotation.RecordedFingerprint;

            if (recorded is null)
            {
                yield return $"{unit.Where} records no Fingerprint field";

                continue;
            }

            if (string.Equals(recorded, AssuranceFingerprint.ToBeFilled, StringComparison.Ordinal))
            {
                yield return
                    $"{unit.Where} still records Fingerprint={AssuranceFingerprint.ToBeFilled}, so the " +
                    "generator has not run over it";

                continue;
            }

            if (!string.Equals(recorded, unit.Fingerprint, StringComparison.Ordinal))
            {
                yield return
                    $"{unit.Where} records Fingerprint={recorded} and the declaration hashes to " +
                    $"{unit.Fingerprint}";
            }
        }
    }

    /// <summary>
    /// Every relevant unit in a state that blocks a release, named with the state and the human
    /// line that produced it.
    /// </summary>
    /// <remarks>
    /// By name and not by count. A publish refused with "689 units are unresolved" tells whoever
    /// reads it nothing they can act on, and the whole point of the per-unit record is that the
    /// answer is a list of declarations.
    /// </remarks>
    internal static IEnumerable<string> UnresolvedBlockers(IEnumerable<AssuranceUnit> units) => units
        .Where(static unit => unit.IsRelevant && AssuranceStateMachine.BlocksRelease(unit.State))
        .Select(static unit =>
            $"{unit.Where} is {AssuranceStateMachine.Name(unit.State)} and its human line reads " +
            $"'{AssuranceHumanReview.HumanLine(unit)}'");
}
