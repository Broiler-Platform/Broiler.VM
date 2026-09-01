using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>One thing charged, polled or reported through the meter, in the order it happened.</summary>
internal readonly record struct MeterEvent(string Kind, VmBudgetDimension Dimension, ulong Amount);

/// <summary>
/// A meter that records the order of everything that goes through it, and refuses what a ceiling
/// says to refuse.
/// </summary>
/// <remarks>
/// <para>
/// The verifier's ANSWER does not show its ordering. An artifact that declares sixty thousand
/// constants and carries none is refused either way; what distinguishes a verifier that compared
/// the count against its bound from one that sized an array first is whether an allocation was
/// charged before the refusal, and the only party that sees that is the meter.
/// </para>
/// <para>
/// This is a composition root's own meter, handed to the verifier through a verification context
/// the root builds. Nothing in the profile knows it is being watched, and nothing in the profile
/// was changed to be watchable - which is what makes the observation worth anything.
/// </para>
/// </remarks>
internal sealed class RecordingMeter : IVmMeter
{
    private readonly List<MeterEvent> events = [];
    private readonly VmLimitVector ceilings;
    private readonly Dictionary<VmBudgetDimension, ulong> charged = [];

    internal RecordingMeter(VmLimitVector ceilings) => this.ceilings = ceilings;

    /// <summary>Everything that went through this meter, in order.</summary>
    internal IReadOnlyList<MeterEvent> Events => events;

    /// <summary>How much of one dimension was charged in total.</summary>
    internal ulong Total(VmBudgetDimension dimension) =>
        charged.TryGetValue(dimension, out var amount) ? amount : 0;

    /// <inheritdoc/>
    public bool TryCharge(VmBudgetDimension dimension, ulong amount)
    {
        var running = Total(dimension) + amount;

        if (running > ceilings[dimension])
        {
            events.Add(new MeterEvent("refuse", dimension, amount));
            return false;
        }

        charged[dimension] = running;
        events.Add(new MeterEvent("charge", dimension, amount));
        return true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// A poll refuses only where the stated wall-clock ceiling is zero, and that is the whole of
    /// the emulation. The core's meter refuses a poll for three causes - a cancelled token, a
    /// spent wall clock, and a profile that went too long between polls - and of the three only
    /// the wall clock is a ceiling this meter is given.
    /// </para>
    /// <para>
    /// <b>Zero rather than a small number, and it is not a clock reading.</b> This meter holds no
    /// clock: a ceiling that had to be compared against elapsed time would make a fuzz session's
    /// answer depend on how busy the machine was, which is exactly the property every session here
    /// is built not to have. A zero ceiling says <em>this host's allowance is already spent</em>,
    /// which is a fact about the host and reaches the same arm of the verifier.
    /// </para>
    /// <para>
    /// <b>Until this arm existed the verifier's poll refusal was unreachable from a root</b>, so
    /// the branch that decides between a cancellation and a wall-clock exhaustion was reached by
    /// no session and no check - a branch whose two answers are the difference between a caller
    /// who changed their mind and a budget that ran out.
    /// </para>
    /// </remarks>
    public bool Poll()
    {
        events.Add(new MeterEvent("poll", VmBudgetDimension.WallClock, 0));

        return ceilings[VmBudgetDimension.WallClock] != 0;
    }

    /// <inheritdoc/>
    public void ReportRetained(VmBudgetDimension dimension, ulong amount) =>
        events.Add(new MeterEvent("retain", dimension, amount));

    /// <inheritdoc/>
    public void ReportReleased(VmBudgetDimension dimension, ulong amount) =>
        events.Add(new MeterEvent("release", dimension, amount));
}

/// <summary>A verification context carrying a recording meter and stated ceilings.</summary>
internal sealed class RecordingContext : IVmVerificationContext
{
    internal RecordingContext(VmLimitVector verificationCeilings)
    {
        Ceilings = new VmEffectiveCeilings(verificationCeilings, VmLimitVector.Unconstrained);
        Recorder = new RecordingMeter(verificationCeilings);
    }

    /// <summary>The meter, as the root sees it.</summary>
    internal RecordingMeter Recorder { get; }

    /// <inheritdoc/>
    public VmEffectiveCeilings Ceilings { get; }

    /// <inheritdoc/>
    public IVmMeter Meter => Recorder;

    /// <inheritdoc/>
    public ImmutableArray<VmHostCapabilityDescriptor> RegisteredCapabilities =>
        ImmutableArray<VmHostCapabilityDescriptor>.Empty;

    /// <inheritdoc/>
    public bool TryGetCapabilityDescriptor(
        VmCapabilityId capabilityId, int version, out VmHostCapabilityDescriptor descriptor)
    {
        descriptor = default;
        return false;
    }
}

/// <summary>
/// Roadmap section 7's third discipline: the ordering is the property, and the answer alone does
/// not show it.
/// </summary>
/// <remarks>
/// <para>
/// The section names three orderings and asks that they be asserted mechanically for every corpus
/// entry, including every failing one. Until JS-3a one ordering was observed - that an unsupported
/// profile is refused without a payload byte being examined - and it is a different one. These are
/// the other three.
/// </para>
/// <para>
/// <b>The non-vacuity check is the load-bearing one.</b> "Nothing was allocated before the
/// refusal" is true of a verifier that never allocates anything at all, so a well-formed artifact
/// is shown to charge <c>AllocatedBytes</c> before any of the negative claims is read as meaning
/// something.
/// </para>
/// </remarks>
internal static class OrderingChecks
{
    /// <summary>
    /// The entries whose refusal is a declared count or length failing its bound, and which must
    /// therefore charge nothing at all.
    /// </summary>
    /// <remarks>
    /// Each of these is refused BEFORE the array its declaration would have sized. Listed by name
    /// rather than derived from the manifest, because the property is about what each artifact was
    /// written to provoke and a derivation from the answer would be reading the outcome back.
    /// </remarks>
    private static readonly string[] SizedByADeclarationThatFailsFirst =
    [
        "a-constant-count-far-beyond-what-the-artifact-carries",
        "more-constants-than-the-limits-section-admits",
        "a-declared-maximum-above-the-formats-own-ceiling",
        "more-frames-than-the-slice-has",
    ];

    /// <summary>
    /// How many bytes of allocation one artifact byte may authorise.
    /// </summary>
    /// <remarks>
    /// The verifier allocates the code section once as bytes, a boundary map and an entry map at
    /// one byte per code byte, a height map at four, and the constant pool at the value struct's
    /// width per entry. Sixty-four is comfortably above what that comes to and far below what a
    /// declaration-sized allocation would reach: the hostile corpus entry declares sixty thousand
    /// constants in an artifact of a few dozen bytes, so a verifier that sized from the
    /// declaration would exceed this by three orders of magnitude. It is a bound on the SHAPE of
    /// the growth, not a measurement, and JS-5 owns measuring anything.
    /// </remarks>
    private const ulong AllocationBytesPerArtifactByte = 64;

    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run(
        string directory, ReplayEntry[] entries)
    {
        yield return CeilingsAreMaterialisedBeforeTheFirstByte(directory, entries);
        yield return AWellFormedArtifactDoesAllocate(directory);
        yield return ADeclarationPastItsBoundSizesNothing(directory, entries);
        yield return AllocationIsProportionalToBytesPresent(directory, entries);
    }

    /// <summary>
    /// A payload larger than the artifact-bytes ceiling is refused with nothing charged at all.
    /// </summary>
    /// <remarks>
    /// If the ceiling were materialised after the first read, the reader would have consumed the
    /// magic, charged work for it, and answered a framing question. Zero events is the observation
    /// that it did not: the ceiling was in hand before the first byte.
    /// </remarks>
    private static (string, bool, string) CeilingsAreMaterialisedBeforeTheFirstByte(
        string directory, ReplayEntry[] entries)
    {
        var bytes = File.ReadAllBytes(Path.Combine(directory, "addition.bjsb"));
        var context = new RecordingContext(Ceilings(VmBudgetDimension.ArtifactBytes, 4));
        var descriptor = Descriptor;

        var outcome = JavaScriptProfile.Descriptor.Verifier.Verify(
            in descriptor, bytes, context, CancellationToken.None);

        var refused = outcome.Category == VmOutcome.ResourceExhaustion &&
            outcome.ExhaustedDimension == VmBudgetDimension.ArtifactBytes;

        return (
            "ceilings-are-materialised-before-the-first-byte-is-read",
            refused && context.Recorder.Events.Count == 0,
            $"{outcome.Category}/{outcome.ExhaustedDimension} over {bytes.Length} bytes against a ceiling " +
            $"of 4, with {context.Recorder.Events.Count} meter events; {entries.Length} corpus " +
            "entries are held to the two checks below");
    }

    /// <summary>
    /// The non-vacuity control: a well-formed artifact charges allocation.
    /// </summary>
    /// <remarks>
    /// Every other check here says something did NOT happen. Without this one they would all pass
    /// over a verifier that allocated nothing, which is the shape a negative claim fails in.
    /// </remarks>
    private static (string, bool, string) AWellFormedArtifactDoesAllocate(string directory)
    {
        var context = Verify(directory, "addition");
        var allocated = context.Recorder.Total(VmBudgetDimension.AllocatedBytes);
        var work = context.Recorder.Total(VmBudgetDimension.VerifierWork);

        return (
            "a-well-formed-artifact-does-charge-for-what-it-allocates",
            allocated > 0 && work > 0,
            $"addition charges {allocated} allocated bytes and {work} work units across " +
            $"{context.Recorder.Events.Count} meter events");
    }

    /// <summary>
    /// An artifact refused on a declared count or length allocates nothing at all.
    /// </summary>
    /// <remarks>
    /// This is "a refusal happens before the allocation it would have authorised" and "a declared
    /// count is compared against its bound before it sizes anything", which are one observation
    /// from two directions. The sharp case is the hostile entry: sixty thousand declared constants
    /// in an artifact of a few dozen bytes, where a verifier that sized first would charge roughly
    /// a megabyte before noticing.
    /// </remarks>
    private static (string, bool, string) ADeclarationPastItsBoundSizesNothing(
        string directory, ReplayEntry[] entries)
    {
        var known = entries.Select(static entry => entry.Name).ToHashSet(StringComparer.Ordinal);
        var failures = new List<string>();
        var checkedNames = new List<string>();

        foreach (var name in SizedByADeclarationThatFailsFirst)
        {
            if (!known.Contains(name))
            {
                failures.Add($"{name}: no corpus entry of that name");
                continue;
            }

            var context = Verify(directory, name);
            var allocated = context.Recorder.Total(VmBudgetDimension.AllocatedBytes);

            if (allocated != 0)
            {
                failures.Add($"{name}: charged {allocated} allocated bytes before refusing");
                continue;
            }

            checkedNames.Add(name);
        }

        return (
            "a-declaration-past-its-bound-sizes-nothing",
            failures.Count == 0,
            failures.Count == 0
                ? $"{checkedNames.Count} entries refused with zero allocated bytes: " +
                    string.Join(", ", checkedNames)
                : string.Join("; ", failures));
    }

    /// <summary>
    /// Across EVERY corpus entry, allocation grows with the bytes present rather than with what
    /// the bytes declare.
    /// </summary>
    /// <remarks>
    /// The per-entry checks above name the artifacts written to provoke the property. This one is
    /// the quantifier the roadmap actually asks for - every entry, including every failing one -
    /// and it is the check that would catch a NEW declaration-sized allocation somewhere no
    /// hand-written case anticipated.
    /// </remarks>
    private static (string, bool, string) AllocationIsProportionalToBytesPresent(
        string directory, ReplayEntry[] entries)
    {
        var failures = new List<string>();
        var worstName = "-";
        double worst = 0;

        foreach (var entry in entries)
        {
            var bytes = File.ReadAllBytes(Path.Combine(directory, entry.Name + ".bjsb"));
            var context = Verify(directory, entry.Name);
            var allocated = context.Recorder.Total(VmBudgetDimension.AllocatedBytes);
            var permitted = (ulong)bytes.Length * AllocationBytesPerArtifactByte;

            if (allocated > permitted)
            {
                failures.Add(
                    $"{entry.Name}: {allocated} allocated bytes from {bytes.Length} artifact bytes");
            }

            var ratio = bytes.Length == 0 ? 0 : (double)allocated / bytes.Length;

            if (ratio > worst)
            {
                worst = ratio;
                worstName = entry.Name;
            }
        }

        return (
            "allocation-is-proportional-to-the-bytes-present-not-to-what-they-declare",
            failures.Count == 0,
            failures.Count == 0
                ? $"{entries.Length} entries under {AllocationBytesPerArtifactByte} allocated " +
                    $"bytes per artifact byte; the highest is {worst:F1} at {worstName}"
                : string.Join("; ", failures));
    }

    private static RecordingContext Verify(string directory, string name)
    {
        var bytes = File.ReadAllBytes(Path.Combine(directory, name + ".bjsb"));
        var context = new RecordingContext(VmLimitVector.Unconstrained);
        var descriptor = Descriptor;

        JavaScriptProfile.Descriptor.Verifier.Verify(
            in descriptor, bytes, context, CancellationToken.None);

        return context;
    }

    private static VmLimitVector Ceilings(VmBudgetDimension dimension, ulong value)
    {
        var values = new ulong[VmBudgetDimensions.Count];
        Array.Fill(values, ulong.MaxValue);
        values[(int)dimension] = value;

        return VmLimitVector.TryCreate(values, out var vector)
            ? vector
            : throw new InvalidOperationException("the frozen dimension count moved");
    }

    private static VmArtifactDescriptor Descriptor { get; } = new(
        JavaScriptProfile.Id,
        1,
        JavaScriptProfile.SliceManifest,
        default,
        VmCallerIdentity.FromCanonicalIdentity("js-execution-only://ordering"));
}
