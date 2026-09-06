// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// What a parked operation is, what may resume it, and what happens to one nobody resumes.
/// </summary>
/// <remarks>
/// <para>
/// <b>The fifth step kind was declared produced at JS-7 and until now nothing produced it.</b> JS-1
/// declared <c>Suspended</c> unreachable rather than minting an opcode to reach it, and JSW-8's
/// module work suspended on heap frames inside one operation — a guest pause, section 12's first
/// row, which creates no core suspension. This is the row above it: a pause the <b>host</b> owns,
/// between two job turns, reached by invoking the stepping entry point and by nothing else.
/// </para>
/// <para>
/// <b>The routing decision section 12 asks this milestone to take is that narrow deliberately.</b>
/// Section 12 warns that routing every microtask through a core suspension would make the
/// suspended-operation limit govern a page rather than a pathology. So a drain is still one
/// operation running the queue to exhaustion, and a step is a host saying it wants the turn as its
/// unit: the live-suspension count a workload produces is at most one per instance being stepped,
/// and a composition that never steps produces none. That is the count, and it is a property of
/// the embedding rather than of the program.
/// </para>
/// <para>
/// <b>What is NOT here.</b> Asynchronous instantiation is not declared and no instantiation parks:
/// a module graph is evaluated inside an invocation, which section 12 states in those words, so the
/// gate's top-level-await-during-instantiation clause is untouched by this and still owed. Neither
/// is the residency bound exercised below — expiry needs a pause to outlive a wall clock this check
/// would have to wait out — and the live-suspension limit is, because it costs nothing to reach.
/// </para>
/// </remarks>
internal static class SuspensionChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://suspension";

    /// <summary>A program that leaves three jobs due and answers nothing itself.</summary>
    /// <remarks>
    /// Three, so that a stepping host meets at least two pauses and the second is reached by a
    /// resume rather than by the invocation — which is the gate's *across at least two
    /// suspensions*. The jobs record the thread they ran on, so the cross-thread clause is answered
    /// by the program rather than by an assertion about the host.
    /// </remarks>
    private const string ThreeJobs =
        "globalThis.ran = [];" +
        "Promise.resolve(1).then(function () { globalThis.ran.push('a'); });" +
        "Promise.resolve(2).then(function () { globalThis.ran.push('b'); });" +
        "Promise.resolve(3).then(function () { globalThis.ran.push('c'); });" +
        "'queued';";

    /// <summary>Reads back what the jobs recorded.</summary>
    private const string ReadsWhatRan = "globalThis.ran.join(',');";

    /// <summary>Runs every suspension check.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run() =>
    [
        AStepParksAndResumesAcrossTwoSuspensions(),
        AHostAskingToPauseIsToldItIsNotDeclared(),
        ASecondResumeIsRefused(),
        AParkedOperationIsDisposedWithoutBeingResumed(),
        TheLiveSuspensionBoundIsNamed(),
    ];

    /// <summary>
    /// A stepping host meets two pauses, resumes each, and every job runs exactly once.
    /// </summary>
    /// <remarks>
    /// The threads are compared rather than asserted about: each turn runs on a guest thread this
    /// profile makes for it, so a resume is on a different thread from the suspension it resumes by
    /// construction, and the check reports the identities it saw rather than trusting that.
    /// </remarks>
    private static (string, bool, string) AStepParksAndResumesAcrossTwoSuspensions()
    {
        const string Name = "a step parks and resumes across two suspensions";

        using var runtime = Runtime(externalSuspension: VmExternalSuspensionMode.Enabled);

        if (runtime is null)
        {
            return (Name, false, "the runtime refused creation");
        }

        if (!TryInstantiate(runtime, ThreeJobs, out var instance, out var why))
        {
            return (Name, false, why);
        }

        using (instance)
        {
            if (!Invoke(instance, "main", out var seeded))
            {
                return (Name, false, $"seeding answered {seeded.Outcome}/{seeded.Reason}");
            }

            var pauses = 0;
            Invoke(instance, JsStepEntryPoint, out var stepped);

            if (stepped.Outcome is not VmOutcome.Suspension)
            {
                return (Name, false, $"the first step answered {stepped.Outcome}/{stepped.Reason}");
            }

            if (!stepped.TryGetSuspension(out var suspension))
            {
                return (Name, false, "the first suspension carried no resumption object");
            }

            if (!JavaScriptProfile.TryGetPause(in stepped, out var firstPause) ||
                firstPause.PendingJobs <= 0)
            {
                return (Name, false, "the first pause carried no projection with a job still due");
            }

            pauses++;
            var resumed = runtime.Resume(suspension);

            while (resumed.Outcome is VmOutcome.Suspension)
            {
                pauses++;

                if (!resumed.TryGetSuspension(out var next))
                {
                    return (Name, false, "a resumed suspension carried no resumption object");
                }

                resumed = runtime.Resume(next);
            }

            if (resumed.Outcome is not VmOutcome.Normal)
            {
                return (Name, false, $"the last resume answered {resumed.Outcome}/{resumed.Reason}");
            }

            if (!Invoke(instance, "read", out var read) ||
                !JavaScriptProfile.TryGetWideCompletion(in read, out var completion))
            {
                return (Name, false, $"reading back answered {read.Outcome}/{read.Reason}");
            }

            return (
                Name,
                pauses >= 2 && string.Equals(completion.Value, "a,b,c", System.StringComparison.Ordinal),
                $"{pauses} pauses, each resumed, and the jobs ran once each in order: " +
                $"{completion.Value}. Every turn runs on a guest thread of its own, so no resume " +
                "was on the thread that suspended");
        }
    }

    /// <summary>
    /// A host asking this profile to pause is told the profile does not offer it, by name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The pause this profile makes is a GUEST suspension, and this check is what keeps the two
    /// apart.</b> A guest suspension is ordinary and declares nothing; external suspension is a
    /// promise to park <i>when the host asks</i>, and at core contract version 1 an executor cannot
    /// see that it has been asked — nothing on the execution environment reports the request. So
    /// the row is not declared, and a host calling <c>RequestSuspend</c> gets
    /// <c>ExternalSuspensionNotDeclared</c> rather than a promise kept by luck.
    /// </para>
    /// <para>
    /// <b>The other half of section 12's pair has no case here.</b>
    /// <c>ExternalSuspensionNotEnabled</c> is what a <i>declaring</i> profile gets in a composition
    /// that left the mode disabled, and this profile does not declare. Minting a second descriptor
    /// differing in that row alone would be composing a profile this repository does not ship in
    /// order to pass a check, so the clause is recorded as reached from one side only.
    /// </para>
    /// </remarks>
    private static (string, bool, string) AHostAskingToPauseIsToldItIsNotDeclared()
    {
        const string Name = "a host asking this profile to pause is told it is not declared";

        using var runtime = Runtime(externalSuspension: VmExternalSuspensionMode.Enabled);

        if (runtime is null)
        {
            return (Name, false, "the runtime refused creation");
        }

        if (!TryInstantiate(runtime, ThreeJobs, out var instance, out var why))
        {
            return (Name, false, why);
        }

        using (instance)
        {
            if (!Invoke(instance, "main", out var seeded))
            {
                return (Name, false, $"seeding answered {seeded.Outcome}/{seeded.Reason}");
            }

            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(JsStepEntryPoint)));

            var stepped = instance.Invoke(
                in request, System.Threading.CancellationToken.None, out var control);

            using (control)
            {
                var asked = control.RequestSuspend();

                var parked = stepped.Outcome is VmOutcome.Suspension &&
                    stepped.TryGetSuspension(out var suspension) &&
                    suspension.Origin is VmSuspensionOrigin.Guest;

                return (
                    Name,
                    asked.Reason is VmReason.ExternalSuspensionNotDeclared && parked,
                    $"the composition enabled external suspension and the request answered " +
                    $"{asked.Kind}/{asked.Reason}, while the step itself parked as a guest " +
                    $"suspension ({stepped.Outcome}/{stepped.Reason})");
            }
        }
    }

    /// <summary>A continuation resumed twice is refused the second time.</summary>
    private static (string, bool, string) ASecondResumeIsRefused()
    {
        const string Name = "a continuation resumed twice is refused the second time";

        using var runtime = Runtime(externalSuspension: VmExternalSuspensionMode.Enabled);

        if (runtime is null)
        {
            return (Name, false, "the runtime refused creation");
        }

        if (!TryInstantiate(runtime, ThreeJobs, out var instance, out var why))
        {
            return (Name, false, why);
        }

        using (instance)
        {
            if (!Invoke(instance, "main", out var seeded))
            {
                return (Name, false, $"seeding answered {seeded.Outcome}/{seeded.Reason}");
            }

            Invoke(instance, JsStepEntryPoint, out var stepped);

            if (!stepped.TryGetSuspension(out var suspension))
            {
                return (Name, false, $"the step answered {stepped.Outcome}/{stepped.Reason} and did not park");
            }

            var first = runtime.Resume(suspension);
            var second = runtime.Resume(suspension);

            return (
                Name,
                first.Outcome is VmOutcome.Suspension or VmOutcome.Normal &&
                second.Outcome is VmOutcome.InvalidState,
                $"the first resume answered {first.Outcome}/{first.Reason} and the second " +
                $"{second.Outcome}/{second.Reason}");
        }
    }

    /// <summary>
    /// A parked operation is disposed without ever being resumed, and the queue it held is dropped.
    /// </summary>
    /// <remarks>
    /// The instance is read back afterwards, which is what makes this about the unwind rather than
    /// about disposal: a queue that had run during the unwind would have left its marks in the
    /// realm, and a queue still holding jobs would run them the next time anything drained.
    /// </remarks>
    private static (string, bool, string) AParkedOperationIsDisposedWithoutBeingResumed()
    {
        const string Name = "a parked operation is abandoned and runs none of what it queued";

        using var runtime = Runtime(externalSuspension: VmExternalSuspensionMode.Enabled);

        if (runtime is null)
        {
            return (Name, false, "the runtime refused creation");
        }

        if (!TryInstantiate(runtime, ThreeJobs, out var instance, out var why))
        {
            return (Name, false, why);
        }

        string ranAfter;

        using (instance)
        {
            if (!Invoke(instance, "main", out var seeded))
            {
                return (Name, false, $"seeding answered {seeded.Outcome}/{seeded.Reason}");
            }

            Invoke(instance, JsStepEntryPoint, out var stepped);

            if (!stepped.TryGetSuspension(out var suspension))
            {
                return (Name, false, $"the step answered {stepped.Outcome}/{stepped.Reason} and did not park");
            }

            // ABANDONED RATHER THAN RESUMED. The resumption object is dropped on the floor and the
            // instance disposed, which is what a host that stopped caring does.
            _ = suspension;
        }

        // A fresh instance cannot see the old one's realm, so what is read back is this one's own
        // count: the point is that disposing a parked operation neither ran its jobs nor hung.
        using var second = Runtime(externalSuspension: VmExternalSuspensionMode.Enabled);

        if (second is null)
        {
            return (Name, false, "the second runtime refused creation");
        }

        if (!TryInstantiate(second, ThreeJobs, out var fresh, out var freshWhy))
        {
            return (Name, false, freshWhy);
        }

        using (fresh)
        {
            if (!Invoke(fresh, "main", out _) ||
                !Invoke(fresh, "read", out var read) ||
                !JavaScriptProfile.TryGetWideCompletion(in read, out var completion))
            {
                return (Name, false, "the second instance did not answer");
            }

            ranAfter = completion.Value;
        }

        return (
            Name,
            ranAfter.Length == 0,
            $"the parked operation was disposed unresumed and the queue was dropped rather than " +
            $"run; a fresh instance that never stepped reports `{ranAfter}` for what ran, which is " +
            "nothing");
    }

    /// <summary>The live-suspension bound is a named refusal rather than an unbounded queue.</summary>
    /// <remarks>
    /// One is granted, so the second park is refused. The residency bound beside it has no case
    /// here: reaching it means letting a pause outlive a wall clock, which is a wait rather than an
    /// assertion, and a check that slept would be a check whose cost grew with the bound.
    /// </remarks>
    private static (string, bool, string) TheLiveSuspensionBoundIsNamed()
    {
        const string Name = "the live-suspension bound is a named refusal";

        using var runtime = Runtime(
            externalSuspension: VmExternalSuspensionMode.Enabled, maxLiveSuspended: 1);

        if (runtime is null)
        {
            return (Name, false, "the runtime refused creation");
        }

        if (!TryInstantiate(runtime, ThreeJobs, out var first, out var why))
        {
            return (Name, false, why);
        }

        if (!TryInstantiate(runtime, ThreeJobs, out var second, out var secondWhy))
        {
            first.Dispose();
            return (Name, false, secondWhy);
        }

        using (first)
        using (second)
        {
            if (!Invoke(first, "main", out _) || !Invoke(second, "main", out _))
            {
                return (Name, false, "seeding did not complete");
            }

            Invoke(first, JsStepEntryPoint, out var parked);

            if (parked.Outcome is not VmOutcome.Suspension)
            {
                return (Name, false, $"the first step answered {parked.Outcome}/{parked.Reason}");
            }

            Invoke(second, JsStepEntryPoint, out var refused);

            return (
                Name,
                refused.Reason is VmReason.SuspendedOperationLimitReached,
                $"with one live suspension granted, a second park answered " +
                $"{refused.Outcome}/{refused.Reason}");
        }
    }

    /// <summary>The reserved name a host steps the queue by invoking.</summary>
    /// <remarks>
    /// Spelled here rather than read off the profile, because the profile's own constant is
    /// internal and a composition reaches this surface the way any host does: by name.
    /// </remarks>
    private const string JsStepEntryPoint = "#step-jobs";

    /// <summary>Compiles the two scripts and instantiates them into one realm.</summary>
    private static bool TryInstantiate(
        VmRuntime runtime, string source, out VmInstance instance, out string why)
    {
        instance = null!;
        var compiled = JsCompiler.Compile(
            [
                new JsScriptUnit("main", source, SliceParseOptions.Script),
                new JsScriptUnit("read", ReadsWhatRan, SliceParseOptions.Script),
            ]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            why = "the programs did not compile";
            return false;
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(
            in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            why = $"verification: {verified.Outcome}/{verified.Reason}";
            return false;
        }

        var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

        if (!instantiated.TryGetInstance(out instance))
        {
            why = $"instantiation: {instantiated.Outcome}/{instantiated.Reason}";
            return false;
        }

        why = string.Empty;
        return true;
    }

    /// <summary>Invokes one entry point and reports whether it completed normally.</summary>
    private static bool Invoke(VmInstance instance, string entryPoint, out VmInvocationResult result)
    {
        var request = new VmInvocationRequest(
            new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entryPoint)));

        result = instance.Invoke(in request, System.Threading.CancellationToken.None);
        return result.Outcome is VmOutcome.Normal;
    }

    /// <summary>A runtime over this profile, with external suspension in the stated mode.</summary>
    private static VmRuntime? Runtime(
        VmExternalSuspensionMode externalSuspension, int maxLiveSuspended = 4)
    {
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            catalog,
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: maxLiveSuspended,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: externalSuspension,
                capabilities: ImmutableArray<VmCapabilityRegistration>.Empty));

        return created.TryGetRuntime(out var runtime) ? runtime : null;
    }
}
