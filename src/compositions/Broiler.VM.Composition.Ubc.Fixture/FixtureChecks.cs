using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Ubc;
using Com.Example.Ledger;
using Com.Example.Tally;
using System.Collections.Immutable;
using System.Globalization;

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
    internal static (string, bool, string) EveryStepKind()
    {
        var expected = new (string Program, string Prefix, string Step)[]
        {
            ("sum", "completed", "Completed and Instantiated"),
            ("suspend", "suspended", "Suspended"),
            ("divide-by-zero", "faulted Trap", "Faulted"),
            ("bad-request", "ProfileFault ProfileContractViolation", "ContractViolation"),
        };
        var missing = new List<string>();

        foreach (var (name, prefix, step) in expected)
        {
            using var runtime = FixtureHost.Runtime();
            var program = TallyPrograms.Named(name);
            var transcript = FixtureHost.Transcript(runtime, program.Artifact.AsSpan(), program.Entry);

            if (!transcript.StartsWith(prefix, StringComparison.Ordinal))
            {
                missing.Add($"{step} from {name}: '{transcript}'");
            }
        }

        return missing.Count == 0
            ? ("every-step-kind", true, "all five executor steps reached")
            : ("every-step-kind", false, string.Join("; ", missing));
    }

    /// <summary>
    /// A loop with no exit, given fuel it cannot spend before its token is cancelled, ends cancelled -
    /// which it can only do if the loop polled within the family's declared bound, because the core
    /// answers a poll-bound breach as a profile fault instead.
    /// </summary>
    internal static (string, bool, string) CancellationWithinTheBound()
    {
        using var runtime = FixtureHost.Runtime(limits =>
        {
            limits[(int)VmBudgetDimension.Fuel] = 100_000_000;
            limits[(int)VmBudgetDimension.WallClock] = 60_000;
        });
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
        var program = TallyPrograms.Named("spin");
        var transcript = FixtureHost.Transcript(runtime, program.Artifact.AsSpan(), program.Entry, cancellation.Token);

        return string.Equals(transcript, "cancelled", StringComparison.Ordinal)
            ? ("cancellation-within-bound", true, $"cancelled, with the loop polling at the declared bound of {TallyProfile.Declaration.MaxUnchargedWork.ToString(CultureInfo.InvariantCulture)}")
            : ("cancellation-within-bound", false, transcript);
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
    internal static (string, bool, string) HostileNeighbour()
    {
        var countdown = TallyPrograms.Named("countdown");
        var catalog = FixtureHost.Catalog(FixtureHost.Tally, LedgerProfile.Descriptor);
        var limits = FixtureHost.Vector(TallyProfile.Defaults());

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

        var ledgerDepth = LedgerProfile.Descriptor.ProfileHardMaxima[VmBudgetDimension.CallDepth];
        var ledgerDefault = LedgerProfile.Descriptor.LimitDefaults[VmBudgetDimension.CallDepth];

        // The host adopts profile defaults: the tightest in the catalog per dimension, which for call
        // depth is the ledger's, so the countdown is refused naming the dimension.
        using (var adopted = FixtureHost.Create(catalog, FixtureHost.AdoptDefaults(), withProvider: false))
        {
            var transcript = FixtureHost.Transcript(adopted, countdown.Artifact.AsSpan(), countdown.Entry);

            if (!string.Equals(transcript, "exhausted CallDepth", StringComparison.Ordinal))
            {
                return ("hostile-neighbour", false, $"with adopted defaults the neighbour's default did not reach the family: '{transcript}'");
            }
        }

        return ("hostile-neighbour", true,
            $"the ledger's call-depth maximum of {ledgerDepth.ToString(CultureInfo.InvariantCulture)} did not reach a countdown twenty frames deep; its default of {ledgerDefault.ToString(CultureInfo.InvariantCulture)}, once adopted, refused it");
    }
}
