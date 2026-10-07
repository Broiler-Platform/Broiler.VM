// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   24
// Annotated:        24/24
// Exempt:           17
// Human-reviewed:   0/24
// IP risk:          Low
// Security risk:    Critical
// Criteria:         7/7
// Resource impact:  4/10 max
// Unverified:       24
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Abstractions;
using System.Collections.Immutable;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The WebAssembly translator: decodes and validates a WebAssembly binary module exactly as this
/// profile's bare-module verifier did until milestone UBC-4 retired it, and lowers a module that
/// passes into a universal bytecode artifact of the WebAssembly family.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS THE FRONT HALF OF VERIFICATION, MOVED OUT OF THE CORE.</b> A composition root calls it
/// before the core verifies anything, and hands the core the artifact it answers. Everything a
/// module can be refused for before milestone UBC-4 it is refused for here, by the same decoder and
/// the same validator in the same order, so a module the bare-module verifier refused is refused
/// with the same outcome, reason, diagnostic code, budget dimension, scope and position, and a
/// module it admitted translates - unless the universal bytecode's format cannot hold it, which is
/// refused with this profile's translation codes, or its artifact is larger than the artifact-bytes
/// ceiling the core would verify that artifact under, which is refused as the core would refuse it.
/// What the universal bytecode then verifies is the translation's output, which no module can choose
/// byte by byte.
/// </para>
/// <para>
/// <b>It runs under a meter of its own, and that meter is the core's verification meter as far as a
/// decoder or a validator can tell.</b> It holds the ceilings it is handed, per dimension, with the
/// core's two release classes: a ceiling-class dimension such as the structural depth gives back
/// what is released, and an allowance such as verifier work or allocated bytes never does. A charge
/// the ceilings cannot cover is refused and latched, as the core latches one, and a translation that
/// stopped on it reports the dimension that refused at runtime scope - the scope a fresh verification
/// meter names, because the core asks its runtime level before the verification's own and a runtime
/// whose ceilings are these refuses first. A bound the decoder or the validator compares itself - a
/// count above the declared-count ceiling, a nesting one level too deep - is not a latched refusal
/// and passes through unchanged, dimension, scope and position. A poll observes the cancellation
/// token, so a cancelled translation answers a cancellation whichever read noticed it.
/// </para>
/// <para>
/// <b>What moved with it, and so no longer holds for this work.</b> Decoding and validation are
/// charged to this meter and to nothing else: they no longer reach the runtime-level accounting a
/// verification's meter commits to, the wall clock the core's poll accrues, or the core's rule of one
/// verification at a time for a profile that declares no concurrent verification. The core still
/// applies all three to the verification of the artifact. The lowering that follows is charged to no
/// meter; it polls the token, its code is held to the artifact-bytes ceiling as it grows, and what it
/// builds grows with the instructions, pushes, branches and frames the validator admitted, never
/// with a product of two of them.
/// </para>
/// <para>
/// <b>It never throws.</b> A cancellation that escapes answers a cancellation, and any other escape
/// answers the reserved defect code <see cref="WebAssemblyDiagnosticCode.VerifierDefect"/> at the
/// position outside every section, as the bare-module verifier answered one: a defect in this
/// assembly is a refusal a reader can recognise, never an exception in a host.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=20A4EC
// Broiler-Falsified-If: a module the bare-module verifier refused translates, or is refused with another outcome, reason, code, dimension, scope or position; a module it admitted is refused other than for a bound the universal bytecode format sets or an artifact above the artifact-bytes ceiling; or any input makes a member throw
// Broiler-Human:        PENDING
public static class WasmTranslator
{
    /// <summary>The identity a translation writes into the artifact's header as the translator that wrote it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1C02CE
    // Broiler-Human:        PENDING
    public const string TranslatorIdentity = "broiler.webassembly.translator";

    /// <summary>The translator's semantic version, written into the artifact's header beside its identity.</summary>
    /// <remarks>It moves whenever the bytes a module translates to change.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=B397CA
    // Broiler-Human:        PENDING
    public const uint TranslatorVersion = 1;

    /// <summary>
    /// The ceilings a translation runs under when its caller states none: the family declaration's
    /// defaults, which carry every dimension verification reads.
    /// </summary>
    /// <remarks>
    /// A host adopting this profile's defaults verifies under exactly these. A composition root with
    /// ceilings of its own passes those instead, intersected with the profile's hard maxima as the core
    /// intersects them, so the translation and the verification of its artifact run under one set.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=00589C
    // Broiler-Human:        PENDING
    public static VmLimitVector VerificationDefaults => WebAssemblyProfile.Declaration.LimitDefaults;

    /// <summary>
    /// Decodes, validates and lowers <paramref name="module"/> under <paramref name="ceilings"/>, and
    /// answers the artifact or the refusal.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The order is the bare-module verification's: a cancellation already requested, then the
    /// artifact-bytes ceiling the core compares before a verifier sees a byte, then the decoder over
    /// the whole module, then the validator over what the decoder produced, then the lowering. An empty
    /// <paramref name="ceilings"/> vector, which reads zero in every dimension, is taken as
    /// <see cref="VerificationDefaults"/>.
    /// </para>
    /// <para>
    /// Two translations of one module under one set of ceilings answer the same bytes.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=58A078
    // Broiler-Falsified-If: an exception escapes, a cancellation is answered as anything but a cancellation, or an escape other than a cancellation is answered as anything but the reserved defect code
    // Broiler-Human:        PENDING
    public static WasmTranslation Translate(
        System.ReadOnlySpan<byte> module,
        VmLimitVector ceilings,
        System.Threading.CancellationToken cancellationToken)
    {
        try
        {
            return TranslateCore(module, ceilings.IsEmpty ? VerificationDefaults : ceilings, cancellationToken);
        }
        catch (System.OperationCanceledException)
        {
            return WasmTranslation.Cancelled();
        }
        catch (System.Exception)
        {
            // EVERY OTHER ESCAPE IS A DEFECT IN THIS ASSEMBLY, and it is answered as the bare-module
            // verifier answered one, because a caller cannot tell an exception from a hostile module.
            return WasmTranslation.Defect();
        }
    }

    /// <summary>The translation's sequence, in the bare-module verification's order.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Critical; Resources=4; Fingerprint=83EE6C
    // Broiler-Falsified-If: a byte of the module is read before the artifact-bytes ceiling is compared, validation runs over a module the decoder refused, or lowering runs over a module validation refused
    // Broiler-Human:        PENDING
    private static WasmTranslation TranslateCore(
        System.ReadOnlySpan<byte> module,
        VmLimitVector ceilings,
        System.Threading.CancellationToken cancellationToken)
    {
        // The core observes a cancellation before it examines any input, and so does this.
        if (cancellationToken.IsCancellationRequested)
        {
            return WasmTranslation.Cancelled();
        }

        // The core compares the payload with the artifact-bytes ceiling before its verifier is asked,
        // and answers the breach at artifact scope.
        if ((ulong)module.Length > ceilings[VmBudgetDimension.ArtifactBytes])
        {
            return WasmTranslation.Exhausted(
                VmReason.CeilingReached, VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact);
        }

        var meter = new WasmTranslationMeter(ceilings, WebAssemblyProfile.MaxUnchargedWork, cancellationToken);
        var adapter = new WasmReadAdapter(meter, ceilings);
        var decoder = new WasmDecoder(module, in adapter.Ceilings, adapter, WebAssemblyProfile.MaxUnchargedWork);

        if (!decoder.TryDecode(out var decoded, out var refusal))
        {
            return Refused(refusal, meter);
        }

        // Decoding completed over the whole module before validation began, as it does in the
        // bare-module verification, so a module both malformed and invalid answers malformed.
        var validator = new WasmValidator(decoded!, adapter, WebAssemblyProfile.MaxUnchargedWork);

        if (!validator.TryValidate(out var invalid))
        {
            return Refused(invalid, meter);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return WasmTranslation.Cancelled();
        }

        return WasmLowering.Lower(decoded!, ceilings[VmBudgetDimension.ArtifactBytes], cancellationToken);
    }

    /// <summary>
    /// A decoder's or a validator's refusal, answered in the fields the core answered it in: a
    /// latched exhaustion names what the meter latched, and a cancellation the meter observed is a
    /// cancellation.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=5D34C4
    // Broiler-Falsified-If: an exhaustion the meter latched is reported with the reader's own attribution, an exhaustion it did not latch is reported with the meter's, or an observed cancellation is answered as an exhaustion
    // Broiler-Human:        PENDING
    private static WasmTranslation Refused(VmVerifierOutcome outcome, WasmTranslationMeter meter)
    {
        switch (outcome.Category)
        {
            case VmOutcome.InvalidArtifact:
                return WasmTranslation.Invalid(
                    outcome.Reason, (WebAssemblyDiagnosticCode)outcome.ProfileDiagnosticCode, outcome.Position);

            case VmOutcome.ResourceExhaustion:
                // A bounded reader folds a refused charge and a refused poll into one status, so the
                // meter, which knows which of the two happened, decides: the core's rule.
                if (meter.CancellationObserved)
                {
                    return WasmTranslation.Cancelled();
                }

                return meter.ExhaustionObserved
                    ? WasmTranslation.Exhausted(outcome.Reason, meter.FailedDimension, WasmTranslationMeter.LatchedScope)
                    : WasmTranslation.Exhausted(outcome.Reason, outcome.ExhaustedDimension, outcome.ExhaustedScope);

            case VmOutcome.Cancellation:
                return WasmTranslation.Cancelled();

            default:
                return WasmTranslation.Defect();
        }
    }
}

/// <summary>
/// What a translation answered: a universal bytecode artifact and the module it was lowered from, or
/// the refusal in the fields the core's verification of the bare module answered in.
/// </summary>
/// <remarks>
/// <para>
/// <b>A refusal carries exactly what the core reported for the same module before milestone
/// UBC-4:</b> the outcome, the reason, this profile's diagnostic code where the outcome carries one,
/// the budget dimension and scope where the outcome is an exhaustion, and the position. A composition
/// root prints them where it printed the core's, so an answer that did not change prints the same
/// line.
/// </para>
/// <para>
/// It is immutable: the artifact is an immutable array, and the module is the verified-state object
/// the validator sealed, which nothing changes after validation returns.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=1; Fingerprint=E0E782
// Broiler-Human:        PENDING
public sealed class WasmTranslation
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A3A08B
    // Broiler-Human:        PENDING
    private WasmTranslation(
        ImmutableArray<byte> artifact,
        WasmModule? module,
        VmOutcome outcome,
        VmReason reason,
        WebAssemblyDiagnosticCode code,
        VmBudgetDimension dimension,
        VmBudgetScope scope,
        VmSourcePosition position)
    {
        Artifact = artifact.IsDefault ? ImmutableArray<byte>.Empty : artifact;
        Module = module;
        Outcome = outcome;
        Reason = reason;
        Code = code;
        Dimension = dimension;
        Scope = scope;
        Position = position;
    }

    /// <summary>Whether the module translated: <see cref="Artifact"/> and <see cref="Module"/> are then present.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7B8D75
    // Broiler-Human:        PENDING
    public bool Succeeded => Outcome is VmOutcome.Normal;

    /// <summary>The universal bytecode artifact, or empty when the module was refused.</summary>
    public ImmutableArray<byte> Artifact { get; }

    /// <summary>The decoded and validated module the artifact was lowered from, or null when it was refused.</summary>
    public WasmModule? Module { get; }

    /// <summary>
    /// <see cref="VmOutcome.Normal"/> when the module translated; otherwise
    /// <see cref="VmOutcome.InvalidArtifact"/>, <see cref="VmOutcome.ResourceExhaustion"/> or
    /// <see cref="VmOutcome.Cancellation"/>.
    /// </summary>
    public VmOutcome Outcome { get; }

    /// <summary>The reason accompanying <see cref="Outcome"/>.</summary>
    public VmReason Reason { get; }

    /// <summary>This profile's diagnostic code for an invalid module, and zero, which names no member, for every other answer.</summary>
    public WebAssemblyDiagnosticCode Code { get; }

    /// <summary>The budget dimension an exhaustion named; meaningful only when <see cref="Outcome"/> is an exhaustion.</summary>
    public VmBudgetDimension Dimension { get; }

    /// <summary>The scope that refused; meaningful only when <see cref="Outcome"/> is an exhaustion.</summary>
    public VmBudgetScope Scope { get; }

    /// <summary>Where in the module an invalid module was found wanting; the default position for every other answer.</summary>
    public VmSourcePosition Position { get; }

    /// <summary>A translation that succeeded.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=52A525
    // Broiler-Human:        PENDING
    internal static WasmTranslation Translated(ImmutableArray<byte> artifact, WasmModule module) =>
        new(artifact, module, VmOutcome.Normal, VmReason.NormalCompleted, 0, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, default);

    /// <summary>An invalid module: a reason, a code and a position.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7D3E4B
    // Broiler-Human:        PENDING
    internal static WasmTranslation Invalid(VmReason reason, WebAssemblyDiagnosticCode code, VmSourcePosition position) =>
        new(default, null, VmOutcome.InvalidArtifact, reason, code, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, position);

    /// <summary>An exhaustion: a reason, the dimension and the scope that refused.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6D8D39
    // Broiler-Human:        PENDING
    internal static WasmTranslation Exhausted(VmReason reason, VmBudgetDimension dimension, VmBudgetScope scope) =>
        new(default, null, VmOutcome.ResourceExhaustion, reason, 0, dimension, scope, default);

    /// <summary>A cancellation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1AA696
    // Broiler-Human:        PENDING
    internal static WasmTranslation Cancelled() =>
        new(default, null, VmOutcome.Cancellation, VmReason.Cancelled, 0, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, default);

    /// <summary>A defect in this assembly, answered as the bare-module verifier answered one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CBFB9D
    // Broiler-Human:        PENDING
    internal static WasmTranslation Defect() =>
        Invalid(
            VmReason.InconsistentStructure,
            WebAssemblyDiagnosticCode.VerifierDefect,
            new VmSourcePosition(sectionIndex: -1, byteOffset: 0, profileCoordinate0: -1, profileCoordinate1: -1));
}

/// <summary>
/// The meter a translation's decoder and validator charge: the ceilings it was handed, per dimension,
/// with the core's release classes, its poll bound and its latches, and nothing above it.
/// </summary>
/// <remarks>
/// <para>
/// <b>Each member answers as the core's verification meter answers.</b> A charge of an undefined
/// dimension is refused without a latch; a charge of nothing is admitted; a charge the remaining
/// ceiling cannot cover is refused and latched, naming its dimension; an admitted charge of verifier
/// work or fuel counts toward the poll bound. A poll refuses a cancelled token first, latching the
/// cancellation, then work above the bound since the last poll, and otherwise resets the count. A
/// retention the ceiling cannot hold is latched and not committed. A release gives back a
/// ceiling-class dimension's consumption, never more than was consumed, and is nothing for an
/// allowance.
/// </para>
/// <para>
/// <b>It has one level.</b> The core's verification meter asks its runtime level, then the
/// verification's own; this one holds the ceilings the translation was handed and nothing else, so a
/// refusal it latches is reported at <see cref="LatchedScope"/>, the scope the core's runtime level
/// names when its ceilings are these.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=1; Fingerprint=EC23D4
// Broiler-Falsified-If: a charge beyond a ceiling is admitted, an allowance is refunded by a release, a ceiling-class release frees more than was consumed, or a cancelled token lets a poll pass
// Broiler-Human:        PENDING
internal sealed class WasmTranslationMeter : IVmMeter
{
    /// <summary>The scope a refusal this meter latched is reported at.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=83528C
    // Broiler-Human:        PENDING
    internal const VmBudgetScope LatchedScope = VmBudgetScope.Runtime;

    private readonly ulong[] ceilings = new ulong[VmBudgetDimensions.Count];
    private readonly ulong[] consumed = new ulong[VmBudgetDimensions.Count];
    private readonly ulong pollBound;
    private readonly System.Threading.CancellationToken cancellationToken;
    private ulong sinceLastPoll;

    /// <summary>A meter holding <paramref name="limits"/>, bounding the work between polls by <paramref name="bound"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BD90DD
    // Broiler-Human:        PENDING
    internal WasmTranslationMeter(VmLimitVector limits, ulong bound, System.Threading.CancellationToken token)
    {
        limits.CopyTo(ceilings);
        pollBound = bound;
        cancellationToken = token;
    }

    /// <summary>Whether a poll saw the cancellation token set.</summary>
    internal bool CancellationObserved { get; private set; }

    /// <summary>Whether a charge or a retention was refused and latched.</summary>
    internal bool ExhaustionObserved { get; private set; }

    /// <summary>Whether a poll found more work since the last one than the bound admits.</summary>
    internal bool PollBoundExceeded { get; private set; }

    /// <summary>The dimension the last latched refusal named.</summary>
    internal VmBudgetDimension FailedDimension { get; private set; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=0; Fingerprint=15E58B
    // Broiler-Falsified-If: a charge the remaining ceiling cannot cover is admitted or committed, or a refused charge leaves no latch naming its dimension
    // Broiler-Human:        PENDING
    public bool TryCharge(VmBudgetDimension dimension, ulong amount)
    {
        if (!VmBudgetDimensions.IsDefined(dimension))
        {
            return false;
        }

        if (amount == 0)
        {
            return true;
        }

        if (!Admits(dimension, amount))
        {
            return Refuse(dimension);
        }

        consumed[(int)dimension] += amount;

        if (dimension is VmBudgetDimension.VerifierWork or VmBudgetDimension.Fuel)
        {
            sinceLastPoll += amount;
        }

        return true;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=High; Resources=0; Fingerprint=70BF59
    // Broiler-Falsified-If: a poll passes while the token is cancelled, or while more work than the bound was charged since the last poll
    // Broiler-Human:        PENDING
    public bool Poll()
    {
        if (cancellationToken.IsCancellationRequested)
        {
            CancellationObserved = true;
            return false;
        }

        if (pollBound > 0 && sinceLastPoll > pollBound)
        {
            PollBoundExceeded = true;
            return false;
        }

        sinceLastPoll = 0;
        return true;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=543C0F
    // Broiler-Human:        PENDING
    public void ReportRetained(VmBudgetDimension dimension, ulong amount)
    {
        if (amount == 0 || !VmBudgetDimensions.IsDefined(dimension))
        {
            return;
        }

        if (!Admits(dimension, amount))
        {
            Refuse(dimension);
            return;
        }

        consumed[(int)dimension] += amount;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0007; IP=Low; Security=Medium; Resources=0; Fingerprint=8B9068
    // Broiler-Human:        PENDING
    public void ReportReleased(VmBudgetDimension dimension, ulong amount)
    {
        if (!VmBudgetDimensions.IsDefined(dimension) || VmBudgetDimensions.ClassOf(dimension) is not VmBudgetClass.Ceiling)
        {
            return;
        }

        var used = consumed[(int)dimension];
        consumed[(int)dimension] = amount >= used ? 0 : used - amount;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=37C008
    // Broiler-Human:        PENDING
    private bool Admits(VmBudgetDimension dimension, ulong amount)
    {
        var ceiling = ceilings[(int)dimension];
        var used = consumed[(int)dimension];
        return used < ceiling && amount <= ceiling - used;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=342272
    // Broiler-Human:        PENDING
    private bool Refuse(VmBudgetDimension dimension)
    {
        FailedDimension = dimension;
        ExhaustionObserved = true;
        return false;
    }
}
