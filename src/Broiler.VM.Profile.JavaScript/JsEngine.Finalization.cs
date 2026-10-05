// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           4
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The host-drained sweep that lets a <c>FinalizationRegistry</c>'s cleanup callbacks arrive
/// (JSD-0029 D03-a, phase F4).
/// </summary>
/// <remarks>
/// <para>
/// <b>The collector's state is read at two points only, both entry points a host invokes from
/// outside guest code</b>: once when <c>#drain-jobs</c> begins, and once before each
/// <c>#step-jobs</c> turn's job (<see cref="JsExecution"/>). A sweep marks every registration whose
/// target the collector has taken and queues one ordinary job per registry that has marked
/// registrations; that job removes each marked registration and then calls the registry's callback
/// with its held value, on the guest stack, under the allowance, like any other job. Nothing runs from
/// a CLR finalizer, nothing sweeps inside <c>JsHostRealm.DrainJobs</c> or a script, and a guest cannot
/// make a sweep happen.
/// </para>
/// <para>
/// <b>It is off unless the composition turned it on</b>, through
/// <see cref="JavaScriptProfile.DescriptorSweepingFinalization"/>, before the realm existed. Off, a
/// registry is the inert one it always was: no registry is tracked and no sweep reads anything.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=7BBE7E
// Broiler-Falsified-If: a cleanup callback runs outside a job a host-requested sweep queued, the collector is read anywhere but at a #drain-jobs or #step-jobs sweep, or any guest code runs from a CLR finalizer
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    /// <summary>Whether this engine's composition turned the sweep on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=377109
    // Broiler-Human:        PENDING
    internal bool SweepsFinalization { get; private init; }

    /// <summary>
    /// What a sweep asks about each registration's target. The production answer is "the weak
    /// reference no longer resolves"; an internal check substitutes a scripted one (JSD-0029 section 6).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C8BB23
    // Broiler-Human:        PENDING
    internal IJsFinalizationEligibility Eligibility { get; set; } = JsCollectedEligibility.Instance;

    /// <summary>Every registry this realm made while sweeping, in creation order, held weakly.</summary>
    /// <remarks>
    /// A registry nothing reaches drops out, and its callbacks never run, which the specification
    /// allows (JSD-0029 section 4.3).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BFE766
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<System.WeakReference<JsFinalizationRegistryObject>> registries = [];

    /// <summary>Starts tracking a registry the realm just made, when this engine sweeps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DC2A08
    // Broiler-Human:        PENDING
    internal void TrackRegistry(JsFinalizationRegistryObject registry)
    {
        if (SweepsFinalization)
        {
            registries.Add(new System.WeakReference<JsFinalizationRegistryObject>(registry));
        }
    }

    /// <summary>
    /// The sweep: marks every registration whose target is collected, and queues one cleanup job for
    /// each registry that holds marked registrations and has none queued.
    /// </summary>
    /// <remarks>
    /// <b>It is metered</b>: one unit per registry and one per registration it inspects. A charge
    /// that fails leaves every registration marked so far marked, so a partial sweep loses nothing and
    /// repeats nothing (JSD-0029 section 4.5).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B4A4C5
    // Broiler-Falsified-If: a sweep runs anywhere but at the start of a #drain-jobs or before a #step-jobs turn, or queues a second cleanup job for a registry that has one queued
    // Broiler-Human:        PENDING
    internal void SweepFinalization()
    {
        if (!SweepsFinalization)
        {
            return;
        }

        for (var at = 0; at < registries.Count; at++)
        {
            if (!registries[at].TryGetTarget(out var registry))
            {
                registries.RemoveAt(at--);
                continue;
            }

            Charge(1);

            if (registry.Mark(this, Eligibility) && !registry.CleanupQueued)
            {
                registry.CleanupQueued = true;
                EnqueueJob(JsValue.Object(Realm.FinalizationCleanupJob), [JsValue.Object(registry)]);
            }
        }
    }

    /// <summary>
    /// Forgets that any registry has a cleanup job queued, after the queue itself was dropped, so the
    /// next sweep queues one again for the registrations that stay marked.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F95ABF
    // Broiler-Human:        PENDING
    private void ForgetQueuedCleanups()
    {
        foreach (var entry in registries)
        {
            if (entry.TryGetTarget(out var registry))
            {
                registry.CleanupQueued = false;
            }
        }
    }
}

/// <summary>
/// Whether a registration's target has been collected, as a sweep asks it (JSD-0029 section 6).
/// </summary>
/// <remarks>
/// <b>Internal, and not public surface.</b> The production answer reads the weak reference; a check
/// that must not depend on when the collector runs substitutes a scripted answer that marks targets
/// it names, by identity, whether or not they are alive - which changes nothing the model promises,
/// because a callback only ever receives the held value.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=37B675
// Broiler-Human:        PENDING
internal interface IJsFinalizationEligibility
{
    /// <summary>Whether the target <paramref name="target"/> refers to counts as collected now.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D360F8
    // Broiler-Human:        PENDING
    bool IsCollected(System.WeakReference<object> target);
}

/// <summary>The production eligibility: the weak reference no longer resolves.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=06319E
// Broiler-Human:        PENDING
internal sealed class JsCollectedEligibility : IJsFinalizationEligibility
{
    /// <summary>The one instance.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=13BBF1
    // Broiler-Human:        PENDING
    internal static readonly JsCollectedEligibility Instance = new();

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=782095
    // Broiler-Human:        PENDING
    public bool IsCollected(System.WeakReference<object> target) => !target.TryGetTarget(out _);
}

/// <summary>
/// A scripted eligibility for internal checks: the targets a check names are collected, by identity,
/// and every other target is asked of the collector as in production.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=09DBA5
// Broiler-Human:        PENDING
internal sealed class JsScriptedEligibility : IJsFinalizationEligibility
{
    /// <summary>The targets the check marked, held weakly so naming one keeps nothing alive.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=010A67
    // Broiler-Human:        PENDING
    private readonly System.Runtime.CompilerServices.ConditionalWeakTable<object, object> marked = new();

    /// <summary>Marks <paramref name="target"/> as collected from the next sweep on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=1D9B22
    // Broiler-Human:        PENDING
    internal void Mark(object target) => marked.AddOrUpdate(target, target);

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=28AC3D
    // Broiler-Human:        PENDING
    public bool IsCollected(System.WeakReference<object> target) =>
        !target.TryGetTarget(out var value) || marked.TryGetValue(value, out _);
}
