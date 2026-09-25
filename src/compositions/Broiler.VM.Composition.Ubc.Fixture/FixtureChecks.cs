using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Ubc;
using Com.Example.Ledger;
using Com.Example.Tally;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.Ubc.Fixture;

/// <summary>The contract checks of the programme's UBC-2.7, each answering a name, a verdict and a detail.</summary>
internal static class FixtureChecks
{
    /// <summary>
    /// The descriptor's rows 1 to 3 and 8 to 30 are the declaration's and rows 4 to 7 the universal
    /// bytecode's, and the catalog admits it.
    /// </summary>
    internal static (string, bool, string) DescriptorRows()
    {
        var d = FixtureHost.Tally;
        var t = TallyProfile.Declaration;
        var differing = new List<string>();

        void Row(string name, bool same)
        {
            if (!same)
            {
                differing.Add(name);
            }
        }

        Row("1 profile", d.ProfileId == t.ProfileId);
        Row("2 display name", string.Equals(d.DisplayName, t.DisplayName, StringComparison.Ordinal));
        Row("3 revision", d.DescriptorRevision == t.DescriptorRevision);
        Row("4 format versions", d.SupportedFormatVersions.Equals(new VmFormatVersionRange(UbcFormat.FormatVersion, UbcFormat.FormatVersion)));
        Row("5 manifests", d.AcceptedFeatureManifests.Length == 1 && d.AcceptedFeatureManifests[0] == TallyProfile.Manifest);
        Row("6 verifier", d.Verifier is UbcVerifier);
        Row("7 executor factory", d.ExecutorFactory is not null);
        Row("8 representation", d.ArtifactRepresentationKind == t.ArtifactRepresentationKind);
        Row("9 lifetime", d.ArtifactLifetimeKind == t.ArtifactLifetimeKind);
        Row("10 concurrent verification", d.SupportsConcurrentVerification == t.SupportsConcurrentVerification);
        Row("11 affinity", d.ThreadAffinity == t.ThreadAffinity);
        Row("12 poll bound", d.CancellationPollBound == t.CancellationPollBound);
        Row("13 abandon budget", d.AbandonBudget == t.AbandonBudget);
        Row("14 defaults", d.LimitDefaults.Equals(t.LimitDefaults));
        Row("15 maxima", d.ProfileHardMaxima.Equals(t.ProfileHardMaxima));
        Row("16 matrix", d.BudgetDeclarationMatrix.Equals(t.BudgetDeclarationMatrix));
        Row("17 imports", d.HostCapabilityDescriptors.SequenceEqual(t.HostCapabilityDescriptors));
        Row("18 guest loads", ReferenceEquals(d.GuestInitiatedLoads, t.GuestInitiatedLoads));
        Row("19 asynchronous instantiation", d.AsynchronousInstantiation == t.AsynchronousInstantiation);
        Row("20 external suspension", d.ExternalSuspension == t.ExternalSuspension);
        Row("21 payload kinds", d.PayloadKindIdRange.Equals(t.PayloadKindIdRange));
        Row("22 built against", d.BuiltAgainstCoreContractVersion == t.BuiltAgainstCoreContractVersion);
        Row("23 authored", d.AuthoredCoreContractVersion == t.AuthoredCoreContractVersion);
        Row("24 conformance", d.ConformanceManifestId.Equals(t.ConformanceManifestId) && d.ConformanceManifestVersion == t.ConformanceManifestVersion);
        Row("25 diagnostics", d.DiagnosticsIdentity.Equals(t.DiagnosticsIdentity));
        Row("26 package", d.PackageIdentity.Equals(t.PackageIdentity));
        Row("27 sharing", d.ArtifactSharing == t.ArtifactSharing);
        Row("28 fault recovery", d.FaultRecovery == t.FaultRecovery);
        Row("29 uncharged work", d.MaxUnchargedWork == t.MaxUnchargedWork);
        Row("30 granularity", d.ChargingGranularity == t.ChargingGranularity);

        _ = FixtureHost.Catalog(FixtureHost.Tally);

        return differing.Count == 0
            ? ("descriptor-rows", true, "rows 1-3 and 8-30 are the declaration's, rows 4-7 the universal bytecode's, and the catalog admits the descriptor")
            : ("descriptor-rows", false, "rows differ: " + string.Join(", ", differing));
    }

    /// <summary>A family compiled against another universal bytecode contract version is refused while the catalog is built.</summary>
    internal static (string, bool, string) ContractVersionRefused()
    {
        var elsewhere = new UbcFamilyRegistration<TallyFamily>(
            TallyTable.Identity, [TallyTable.Table], new TallyVerifier(), authoredUbcContractVersion: 1, builtAgainstUbcContractVersion: 2);

        try
        {
            _ = FixtureHost.Catalog(UbcDescriptors.Build(elsewhere, TallyProfile.Declaration, UbcEmitterSet.Create(UbcBytecodeEmitter.Form)));
            return ("contract-version-refused", false, "a family built against version 2 was composed");
        }
        catch (UbcCompositionException refusal) when (refusal.Fault == UbcCompositionFault.ContractVersionMismatch)
        {
            return ("contract-version-refused", true, refusal.Message);
        }
    }

    /// <summary>Every program of the fixed list answers its expected transcript.</summary>
    internal static (string, bool, string) ProgramCorpus()
    {
        var failures = new List<string>();

        foreach (var program in TallyPrograms.All)
        {
            using var runtime = FixtureHost.Runtime();
            var transcript = FixtureHost.Transcript(runtime, program.Artifact.AsSpan(), program.Entry);

            if (!string.Equals(transcript, program.Expected, StringComparison.Ordinal))
            {
                failures.Add($"{program.Name}: expected '{program.Expected}', got '{transcript}'");
            }
        }

        return failures.Count == 0
            ? ("program-corpus", true, $"{TallyPrograms.All.Length} programs answered their transcripts")
            : ("program-corpus", false, string.Join("; ", failures));
    }

    /// <summary>
    /// Every step an executor may answer is reached from the fixture family: completed, instantiated,
    /// suspended, faulted, and a contract violation - the last kept apart from the guest's faults.
    /// </summary>
    /// <remarks>
    /// The core answers an exception an executor throws exactly as it answers a contract violation the
    /// executor returns, so the transcript alone cannot tell the two apart. The check reads the steps
    /// the executor itself answered through a probing form, requires that no call threw, and is watched
    /// failing over an executor told to throw where it should refuse.
    /// </remarks>
    internal static (string, bool, string) EveryStepKind()
    {
        var expected = new (string Program, string Prefix, VmExecutionStepKind[] Steps)[]
        {
            ("sum", "completed", [VmExecutionStepKind.Instantiated, VmExecutionStepKind.Completed]),
            ("suspend", "suspended", [VmExecutionStepKind.Instantiated, VmExecutionStepKind.Suspended, VmExecutionStepKind.Completed]),
            ("divide-by-zero", "faulted Trap", [VmExecutionStepKind.Instantiated, VmExecutionStepKind.Faulted]),
            ("bad-request", "ProfileFault ProfileContractViolation", [VmExecutionStepKind.Instantiated, VmExecutionStepKind.ContractViolation]),
        };
        var missing = new List<string>();

        foreach (var (name, prefix, steps) in expected)
        {
            var probe = new FixtureProbe();
            using var runtime = FixtureHost.Runtime(descriptor: probe.Descriptor());
            var program = TallyPrograms.Named(name);
            var transcript = FixtureHost.Transcript(runtime, program.Artifact.AsSpan(), program.Entry);

            if (!transcript.StartsWith(prefix, StringComparison.Ordinal) || probe.Threw || !probe.Steps.SequenceEqual(steps))
            {
                missing.Add($"{name}: '{transcript}', steps [{string.Join(' ', probe.Steps)}]{(probe.Threw ? ", the executor threw" : string.Empty)}");
            }
        }

        // The control: an executor that crashes where it should refuse gives the same transcript, and
        // the check must see through it.
        var crashing = new FixtureProbe { ThrowOnInvoke = true };

        using (var runtime = FixtureHost.Runtime(descriptor: crashing.Descriptor()))
        {
            var program = TallyPrograms.Named("bad-request");
            var transcript = FixtureHost.Transcript(runtime, program.Artifact.AsSpan(), program.Entry);

            if (!crashing.Threw || !string.Equals(transcript, program.Expected, StringComparison.Ordinal))
            {
                missing.Add($"the crashing control was not told apart: '{transcript}', threw {crashing.Threw}");
            }
        }

        return missing.Count == 0
            ? ("every-step-kind", true, "all five executor steps reached and read from the executor itself, and a crashing executor told apart from a refusing one")
            : ("every-step-kind", false, string.Join("; ", missing));
    }

    /// <summary>
    /// A loop with no exit, whose token is cancelled while it runs, stops within the family's declared
    /// bound of fuel after the cancellation, having seen the cancellation at a poll.
    /// </summary>
    /// <remarks>
    /// The core ranks a cancellation above a poll-bound breach, so a cancelled answer alone would come
    /// from a loop that never polled as readily as from one that did. The check measures the fuel the
    /// executor charged after the token was cancelled, through a probing form, and requires a poll to
    /// have answered the cancellation; it is watched failing over a loop whose polls are swallowed.
    /// </remarks>
    internal static (string, bool, string) CancellationWithinTheBound()
    {
        var bound = (ulong)TallyProfile.Declaration.MaxUnchargedWork;
        var spin = TallyPrograms.Named("spin");

        (string Transcript, FixtureProbe Probe) Run(bool swallow, CancellationTokenSource cancellation, ulong fuel, bool cancelAtInvoke)
        {
            var probe = new FixtureProbe { Watched = cancellation.Token, SwallowPolls = swallow, CancelAtInvoke = cancelAtInvoke ? cancellation : null };
            using var runtime = FixtureHost.Runtime(
                limits =>
                {
                    limits[(int)VmBudgetDimension.Fuel] = fuel;
                    limits[(int)VmBudgetDimension.WallClock] = 60_000;
                },
                descriptor: probe.Descriptor());
            return (FixtureHost.Transcript(runtime, spin.Artifact.AsSpan(), spin.Entry, cancellation.Token), probe);
        }

        bool Holds(string transcript, FixtureProbe probe) =>
            string.Equals(transcript, "cancelled", StringComparison.Ordinal) && probe.CancellationObserved && probe.FuelAfterCancellation <= bound;

        using var timed = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        var (answer, measured) = Run(swallow: false, timed, 100_000_000, cancelAtInvoke: false);

        if (!Holds(answer, measured))
        {
            return ("cancellation-within-bound", false,
                $"'{answer}', {measured.FuelAfterCancellation.ToString(CultureInfo.InvariantCulture)} fuel after the cancellation against a bound of {bound.ToString(CultureInfo.InvariantCulture)}, observed at a poll {measured.CancellationObserved}");
        }

        // The control: a loop that never polls, its token cancelled as the invocation starts, is still
        // answered as cancelled once its fuel runs out, and the check must not pass it.
        using var cancelled = new CancellationTokenSource();
        var (control, swallowed) = Run(swallow: true, cancelled, 1_000_000, cancelAtInvoke: true);

        if (!string.Equals(control, "cancelled", StringComparison.Ordinal) || Holds(control, swallowed))
        {
            return ("cancellation-within-bound", false, $"the control that never polls was not answered cancelled and failed by the measure: '{control}'");
        }

        return ("cancellation-within-bound", true,
            $"cancelled, {measured.FuelAfterCancellation.ToString(CultureInfo.InvariantCulture)} fuel after the cancellation against a declared bound of {bound.ToString(CultureInfo.InvariantCulture)}, seen at a poll; a loop that never polls fails the same measure ('{control}', {swallowed.FuelAfterCancellation.ToString(CultureInfo.InvariantCulture)} fuel after it)");
    }

    /// <summary>An artifact naming a form this image does not compose is refused at verification, by name.</summary>
    internal static (string, bool, string) FormNotComposed()
    {
        var bytes = FixtureCorpus.WithForm(TallyPrograms.Named("sum").Artifact.AsSpan(), "x86-64");
        using var runtime = FixtureHost.Runtime();
        var transcript = FixtureHost.Transcript(runtime, bytes, "main");
        var expected = $"refused InvalidArtifact UnknownFeature {(int)UbcDiagnosticCode.FormNotComposed}";

        return string.Equals(transcript, expected, StringComparison.Ordinal)
            ? ("form-not-composed", true, transcript)
            : ("form-not-composed", false, $"expected '{expected}', got '{transcript}'");
    }

    /// <summary>
    /// The fuel-parity twins answer one verdict at every fuel ceiling: each charges the same fuel at
    /// the same points, whether its addition is executed from the primitive table or by a handler.
    /// </summary>
    internal static (string, bool, string) FuelParity()
    {
        var inline = TallyPrograms.Named("twin-inline");
        var dynamic = TallyPrograms.Named("twin-dynamic");
        var first = -1;

        for (var fuel = 1; fuel <= 400; fuel++)
        {
            var ceiling = (ulong)fuel;
            using var one = FixtureHost.Runtime(limits => limits[(int)VmBudgetDimension.Fuel] = ceiling);
            using var two = FixtureHost.Runtime(limits => limits[(int)VmBudgetDimension.Fuel] = ceiling);
            var a = FixtureHost.Transcript(one, inline.Artifact.AsSpan(), inline.Entry);
            var b = FixtureHost.Transcript(two, dynamic.Artifact.AsSpan(), dynamic.Entry);

            if (!string.Equals(a, b, StringComparison.Ordinal))
            {
                return ("fuel-parity", false, $"at a fuel ceiling of {fuel.ToString(CultureInfo.InvariantCulture)} the twins disagree: '{a}' against '{b}'");
            }

            if (first < 0 && a.StartsWith("completed", StringComparison.Ordinal))
            {
                first = fuel;
            }
        }

        return first > 1
            ? ("fuel-parity", true, $"one verdict at every ceiling from 1 to 400; both first complete at {first.ToString(CultureInfo.InvariantCulture)}")
            : ("fuel-parity", false, "the twins never both exhausted and completed across the ceilings tried");
    }

    /// <summary>
    /// The two-profile hostile-neighbour check: composed beside the ledger, the fixture family is not
    /// reached by the ledger's hard maxima, and is reached by its defaults when the host adopts them.
    /// </summary>
    /// <remarks>
    /// The defaults half runs a call two frames deep: more than the ledger's default call depth and no
    /// more than its hard maximum, so it is refused only if the ceiling in force is the default and not
    /// the maximum, and the check first shows that it completes under the maximum.
    /// </remarks>
    internal static (string, bool, string) HostileNeighbour()
    {
        var countdown = TallyPrograms.Named("countdown");
        var shallow = TallyPrograms.Named("call-request");
        var catalog = FixtureHost.Catalog(FixtureHost.Tally, LedgerProfile.Descriptor);
        var limits = FixtureHost.Vector(TallyProfile.Defaults());
        var ledgerDepth = LedgerProfile.Descriptor.ProfileHardMaxima[VmBudgetDimension.CallDepth];
        var ledgerDefault = LedgerProfile.Descriptor.LimitDefaults[VmBudgetDimension.CallDepth];

        // The programs must separate the two values, or the check proves nothing.
        if (!(ledgerDefault < 2 && 2 <= ledgerDepth && ledgerDepth < 22))
        {
            return ("hostile-neighbour", false, "the ledger's call-depth default and maximum no longer separate a two-frame call from a countdown twenty-two frames deep");
        }

        // The ledger's own maximum call depth is below what the countdown needs; the host states the
        // family's depth explicitly, and the neighbour's maximum must not reach the family's artifacts.
        using (var stated = FixtureHost.Create(catalog, FixtureHost.Explicit(limits), withProvider: false))
        {
            var transcript = FixtureHost.Transcript(stated, countdown.Artifact.AsSpan(), countdown.Entry);

            if (!string.Equals(transcript, countdown.Expected, StringComparison.Ordinal))
            {
                return ("hostile-neighbour", false, $"with stated ceilings the neighbour's maxima reached the family: '{transcript}'");
            }
        }

        // The two-frame call fits under the neighbour's maximum.
        var atMaximum = (ulong[])limits.Clone();
        atMaximum[(int)VmBudgetDimension.CallDepth] = ledgerDepth;

        using (var maximum = FixtureHost.Create(catalog, FixtureHost.Explicit(atMaximum), withProvider: false))
        {
            var transcript = FixtureHost.Transcript(maximum, shallow.Artifact.AsSpan(), shallow.Entry);

            if (!string.Equals(transcript, shallow.Expected, StringComparison.Ordinal))
            {
                return ("hostile-neighbour", false, $"a two-frame call did not complete under the neighbour's maximum: '{transcript}'");
            }
        }

        // The host adopts profile defaults: the tightest in the catalog per dimension, which for call
        // depth is the ledger's, so the two-frame call is refused naming the dimension.
        using (var adopted = FixtureHost.Create(catalog, FixtureHost.AdoptDefaults(), withProvider: false))
        {
            var transcript = FixtureHost.Transcript(adopted, shallow.Artifact.AsSpan(), shallow.Entry);

            if (!string.Equals(transcript, "exhausted CallDepth", StringComparison.Ordinal))
            {
                return ("hostile-neighbour", false, $"with adopted defaults the neighbour's default did not reach the family: '{transcript}'");
            }
        }

        return ("hostile-neighbour", true,
            $"the ledger's call-depth maximum of {ledgerDepth.ToString(CultureInfo.InvariantCulture)} did not reach a countdown twenty-two frames deep and admits a two-frame call; its default of {ledgerDefault.ToString(CultureInfo.InvariantCulture)}, once adopted, refused that call");
    }

    /// <summary>
    /// A frame's entry is charged its unit's frame fuel: calls into a unit that declares many locals are
    /// refused under a fuel ceiling their call rows alone would fit in many times over.
    /// </summary>
    internal static (string, bool, string) FrameFuelCharged()
    {
        var wide = TallyPrograms.Named("wide-frame");
        var ceiling = (ulong)TallyPrograms.WideFrameLocals / UbcUnitCode.LocalsPerFrameFuel * TallyPrograms.WideFrameCalls;

        using var generous = FixtureHost.Runtime();
        var completed = FixtureHost.Transcript(generous, wide.Artifact.AsSpan(), wide.Entry);

        using var tight = FixtureHost.Runtime(limits => limits[(int)VmBudgetDimension.Fuel] = ceiling);
        var refused = FixtureHost.Transcript(tight, wide.Artifact.AsSpan(), wide.Entry);

        return string.Equals(completed, wide.Expected, StringComparison.Ordinal) && string.Equals(refused, "exhausted Fuel", StringComparison.Ordinal)
            ? ("frame-fuel", true, $"completes under the family's fuel default, and is refused under {ceiling.ToString(CultureInfo.InvariantCulture)}, what its frames alone cost")
            : ("frame-fuel", false, $"under the default '{completed}', under {ceiling.ToString(CultureInfo.InvariantCulture)} '{refused}'");
    }

    /// <summary>
    /// A parked operation holds no call depth: beside one suspended two frames deep, and after one whose
    /// resumption was cancelled at its first poll, a sibling needing the rest of a small ceiling runs.
    /// </summary>
    internal static (string, bool, string) ParkedOperationHoldsNoDepth()
    {
        var suspending = TallyPrograms.Named("suspend-in-callee");
        var sibling = TallyPrograms.Named("call-request");
        using var runtime = FixtureHost.Runtime(limits => limits[(int)VmBudgetDimension.CallDepth] = 3);
        var descriptor = FixtureHost.Descriptor();
        var main = Encoding.UTF8.GetBytes("main");
        var failures = new List<string>();

        VmInstance Instance(TallyProgram program)
        {
            if (!runtime.Verify(in descriptor, program.Artifact.AsSpan(), default).TryGetArtifact(out var handle) ||
                !runtime.Instantiate(handle, default).TryGetInstance(out var instance))
            {
                throw new InvalidOperationException($"{program.Name} did not verify and instantiate");
            }

            return instance;
        }

        string Sibling()
        {
            using var instance = Instance(sibling);
            var request = new VmInvocationRequest(new VmUtf8Text(main));
            var result = instance.Invoke(in request, default);
            return result.Outcome == VmOutcome.Normal ? "ran" : $"{result.Outcome} {result.Diagnostics.ExhaustedDimension}";
        }

        using (var parked = Instance(suspending))
        {
            var request = new VmInvocationRequest(new VmUtf8Text(main));
            var suspended = parked.Invoke(in request, default);

            if (!suspended.TryGetSuspension(out var suspension))
            {
                return ("parked-holds-no-depth", false, $"the program did not suspend: {suspended.Outcome}");
            }

            if (Sibling() is var beside && beside != "ran")
            {
                failures.Add($"beside a parked operation the sibling answered {beside}");
            }

            if (runtime.Resume(suspension).Outcome != VmOutcome.Normal)
            {
                failures.Add("the parked operation did not resume to its end");
            }
        }

        using (var host = new CancellationTokenSource())
        using (var parked = Instance(suspending))
        {
            var request = new VmInvocationRequest(new VmUtf8Text(main));
            var suspended = parked.Invoke(in request, host.Token);

            if (suspended.TryGetSuspension(out var suspension))
            {
                host.Cancel();
                var resumed = runtime.Resume(suspension);

                if (resumed.Outcome != VmOutcome.Cancellation)
                {
                    failures.Add($"a resumption of a cancelled operation answered {resumed.Outcome}");
                }

                if (Sibling() is var after && after != "ran")
                {
                    failures.Add($"after a resumption ended at its first poll the sibling answered {after}");
                }
            }
            else
            {
                failures.Add($"the second program did not suspend: {suspended.Outcome}");
            }
        }

        return failures.Count == 0
            ? ("parked-holds-no-depth", true, "under a call depth of 3, a two-frame sibling ran beside an operation parked two frames deep, and after one whose resumption was cancelled at its first poll")
            : ("parked-holds-no-depth", false, string.Join("; ", failures));
    }
}
