using Broiler.VM;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>One row of the retained corpus manifest, as this root reads it back.</summary>
internal sealed record ReplayEntry(
    string Name,
    string Sha256,
    string Mode,
    string Outcome,
    string Reason,
    int DiagnosticCode,
    string Completion,
    string Position,
    string Dimension,
    string Scope);

/// <summary>What one replayed entry actually did.</summary>
internal sealed record ReplayObservation(
    string Name,
    string Outcome,
    string Reason,
    int DiagnosticCode,
    string Completion,
    string Position,
    string Dimension,
    string Scope,
    string HashStatus);

/// <summary>
/// The retained-corpus replay: read the bytes, re-hash them, verify them, and compare the observed
/// triple against what the manifest recorded.
/// </summary>
/// <remarks>
/// <para>
/// <b>The hash is re-computed rather than trusted.</b> A corpus whose bytes changed without its
/// manifest changing is a failure and not a quiet drift, which is the whole reason a hash is
/// recorded beside every entry.
/// </para>
/// <para>
/// <b>Nine entries are replayed under a host rather than under bytes.</b> A resource exhaustion
/// needs a runtime with a tight ceiling and there is one per dimension a verification can exhaust,
/// a cancellation needs a token that is already cancelled, and an unsupported profile needs a
/// descriptor naming a profile the catalog does not hold. Their bytes are the same well-formed
/// artifact as a control entry's, so what the row proves is a property of the host and not of the
/// program.
/// </para>
/// <para>
/// <b>An exhaustion row observes a dimension and a scope, and every other row observes neither.</b>
/// An exhaustion answer carries no diagnostic code - the column is zero on all nine - so the pair
/// is the only thing that identifies which refusal happened, and a row that recorded the category
/// alone would be satisfied by a verifier that exhausted the wrong budget.
/// </para>
/// </remarks>
internal static class CorpusReplay
{
    /// <summary>Parses the manifest a producer wrote.</summary>
    /// <remarks>
    /// Hand-parsed rather than deserialized. The producer formats and this reads, and the two
    /// halves are deliberately independent code: a shared serializer that round-trips itself would
    /// agree with itself whatever either half meant.
    /// </remarks>
    internal static ReplayEntry[] ReadManifest(string path)
    {
        var entries = new List<ReplayEntry>();

        foreach (var line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var parts = line.Split('|');

            if (parts.Length != 10)
            {
                throw new InvalidOperationException($"corpus manifest row has {parts.Length} columns: {line}");
            }

            entries.Add(new ReplayEntry(
                parts[0],
                parts[1],
                parts[2],
                parts[3],
                parts[4],
                int.Parse(parts[5], System.Globalization.CultureInfo.InvariantCulture),
                parts[6],
                parts[7],
                parts[8],
                parts[9]));
        }

        return entries.ToArray();
    }

    /// <summary>Replays every entry and answers what each one did.</summary>
    internal static ReplayObservation[] Replay(string directory, ReplayEntry[] entries)
    {
        var observations = new ReplayObservation[entries.Length];

        for (var index = 0; index < entries.Length; index++)
        {
            observations[index] = ReplayOne(directory, entries[index]);
        }

        return observations;
    }

    private static ReplayObservation ReplayOne(string directory, ReplayEntry entry)
    {
        var bytes = File.ReadAllBytes(Path.Combine(directory, entry.Name + ".bjsb"));
        var hash = Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes));
        var hashStatus = string.Equals(hash, entry.Sha256, StringComparison.Ordinal) ? "match" : "MISMATCH";

        using var runtime = Hosts.Runtime(entry.Mode, out var failure);

        if (runtime is null)
        {
            return new ReplayObservation(
                entry.Name, "HostFailure", failure, 0, "-", "-", "-", "-", hashStatus);
        }

        var descriptor = Hosts.Descriptor(entry.Mode);
        using var cancellation = new CancellationTokenSource();

        if (string.Equals(entry.Mode, "cancelled", StringComparison.Ordinal))
        {
            cancellation.Cancel();
        }

        var verified = runtime.Verify(in descriptor, bytes, cancellation.Token);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return new ReplayObservation(
                entry.Name,
                verified.Outcome.ToString(),
                verified.Reason.ToString(),
                verified.Diagnostics.ProfileDiagnosticCode,
                "-",
                Position(entry, in verified),
                Dimension(in verified),
                Scope(in verified),
                hashStatus);
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return new ReplayObservation(
                entry.Name,
                instantiated.Outcome.ToString(),
                instantiated.Reason.ToString(),
                instantiated.Diagnostics.ProfileDiagnosticCode,
                "-",
                "-",
                "-",
                "-",
                hashStatus);
        }

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        // TWO SURFACES, TWO PAYLOAD KINDS, ONE COLUMN. A version-2 instance answers with the
        // wide surface's own completion payload, and a replay that only knew the version-1 one
        // would record every passing version-2 entry as having produced nothing.
        var completion = JavaScriptProfile.TryGetCompletion(in result, out var value)
            ? value.Value.ToDiagnosticString()
            : JavaScriptProfile.TryGetWideCompletion(in result, out var wide)
                ? (wide.TypeOf == "undefined" ? "undefined" : wide.Value)
                : JavaScriptProfile.TryGetFault(in result, out var fault)
                    ? "fault:" + fault.Kind
                    : JavaScriptProfile.TryGetUncaught(in result, out var uncaught)
                        ? "uncaught:" + uncaught.ErrorName
                        : "-";

        return new ReplayObservation(
            entry.Name,
            verified.Outcome.ToString(),
            verified.Reason.ToString(),
            verified.Diagnostics.ProfileDiagnosticCode,
            completion,
            Position(entry, in verified),
            Dimension(in verified),
            Scope(in verified),
            hashStatus);
    }

    /// <summary>The dimension a verification exhausted, and <c>-</c> where it exhausted none.</summary>
    /// <remarks>
    /// <para>
    /// <b>Read only where the outcome is an exhaustion.</b> The two fields are present on every
    /// diagnostics record and carry the first member of each enumeration where nothing was
    /// exhausted, so formatting them unconditionally would write <c>Fuel</c> and <c>Aggregate</c>
    /// beside sixty entries that exhausted nothing - a value that looks like an observation, is
    /// not one, and would be compared against a manifest column somebody had to fill in with it.
    /// </para>
    /// <para>
    /// This is the whole reason the pair is recorded at all: an exhaustion answer carries no
    /// diagnostic code, so <c>ResourceExhaustion/CeilingReached/0</c> is the same triple for a
    /// section-count ceiling and a structural-depth one, and the corpus could not tell a verifier
    /// that refuses the right artifact for the wrong reason from one that does not.
    /// </para>
    /// </remarks>
    private static string Dimension(in VmVerificationResult verified) =>
        verified.Outcome == VmOutcome.ResourceExhaustion
            ? verified.Diagnostics.ExhaustedDimension.ToString()
            : "-";

    /// <summary>The scope that refused, and <c>-</c> where nothing did.</summary>
    /// <remarks>
    /// The scope is not decoration beside the dimension. The reader's four ceilings are compared
    /// inside a verification and answer at <c>Artifact</c>; the three allowances are charged
    /// through the meter, which reports the level that actually refused - so the same profile
    /// answers at two different scopes depending on which budget ran out, and a row recording the
    /// dimension alone would hide that.
    /// </remarks>
    private static string Scope(in VmVerificationResult verified) =>
        verified.Outcome == VmOutcome.ResourceExhaustion
            ? verified.Diagnostics.ExhaustedScope.ToString()
            : "-";

    /// <summary>
    /// The four fields of the position a verification answered with, as the manifest writes them.
    /// </summary>
    /// <remarks>
    /// A row that pins no position observes none: the alternative - formatting every row's four
    /// fields and comparing them - would turn every entry into a claim about byte offsets this
    /// corpus does not make, and the first change to the artifact writer would fail sixty rows for
    /// a reason none of them is about.
    /// </remarks>
    private static string Position(ReplayEntry entry, in VmVerificationResult verified)
    {
        if (string.Equals(entry.Position, "-", StringComparison.Ordinal))
        {
            return "-";
        }

        var position = verified.Diagnostics.SourcePosition;

        return string.Join(
            ':',
            position.SectionIndex,
            position.ByteOffset,
            position.ProfileCoordinate0,
            position.ProfileCoordinate1);
    }

    /// <summary>Whether an observation is what its manifest row recorded.</summary>
    internal static bool Agrees(ReplayEntry expected, ReplayObservation observed) =>
        observed.HashStatus == "match" &&
        string.Equals(expected.Outcome, observed.Outcome, StringComparison.Ordinal) &&
        string.Equals(expected.Reason, observed.Reason, StringComparison.Ordinal) &&
        expected.DiagnosticCode == observed.DiagnosticCode &&
        string.Equals(expected.Completion, observed.Completion, StringComparison.Ordinal) &&
        string.Equals(expected.Position, observed.Position, StringComparison.Ordinal) &&
        string.Equals(expected.Dimension, observed.Dimension, StringComparison.Ordinal) &&
        string.Equals(expected.Scope, observed.Scope, StringComparison.Ordinal);
}
