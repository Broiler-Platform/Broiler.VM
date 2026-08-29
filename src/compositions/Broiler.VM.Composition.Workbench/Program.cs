using Broiler.VM;
using Com.Example.Calculator;
using Com.Example.Ledger;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.Workbench;

/// <summary>
/// The two-profile composition: two application-local profiles from two assemblies, in one catalog,
/// with one host capability bound for one of them.
/// </summary>
/// <remarks>
/// <para>
/// The claim it exists to demonstrate is the one VM-3's gate names last: adding a second profile
/// requires no change to the core runtime or the execution loop. The three product assemblies are
/// byte-identical to the ones the single-profile root links, and this file is the whole of the
/// difference between a composition with one profile and a composition with two.
/// </para>
/// <para>
/// The two profiles share nothing but the contract. Different formats, different value models,
/// different entry-point conventions, disjoint payload kind ranges, different limit defaults, and
/// one imports a host capability while the other imports none. They do not reference each other and
/// could not: neither knows the other exists.
/// </para>
/// </remarks>
internal static class Program
{
    private static int Main(string[] args)
    {
        var verbose = args.Contains("--verbose", StringComparer.Ordinal);

        try
        {
            if (args.Contains("--closure", StringComparer.Ordinal))
            {
                return ReportClosure();
            }

            var checks = new List<(string Name, bool Passed, string Detail)>
            {
                BothProfilesInOneCatalog(),
                EachProfileKeepsItsOwnFormat(),
                EntryPointBytesAreTheProfilesToRead(),
                AnOptionalImportIsBoundWhenTheHostOffersIt(),
                AnOptionalImportIsAbsentWhenItDoesNot(),
                AForeignArtifactIsUnsupportedRatherThanInvalid(),
                LanguageFaultsStayInTheirOwnProfile(),
            };

            var failed = 0;

            foreach (var (name, passed, detail) in checks)
            {
                if (!passed)
                {
                    failed++;
                }

                if (verbose || !passed)
                {
                    Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
                }
            }

            Console.WriteLine(
                failed == 0
                    ? $"broiler-vm-composition-workbench: {checks.Count} checks passed, core contract version {VmCoreContract.Version}"
                    : $"broiler-vm-composition-workbench: {failed} of {checks.Count} checks FAILED");

            return failed == 0 ? 0 : 1;
        }
        catch (Exception failure)
        {
            Console.WriteLine(
                $"broiler-vm-composition-workbench: unhandled {failure.GetType().Name}: {failure.Message}");

            return 2;
        }
    }

    /// <summary>Two profiles, two assemblies, one catalog, one runtime.</summary>
    private static (string, bool, string) BothProfilesInOneCatalog()
    {
        using var runtime = Runtime(stamping: true, out var failure);

        if (runtime is null)
        {
            return ("two-profile-catalog", false, failure);
        }

        return runtime.Catalog.Count == 2
            ? ("two-profile-catalog", true, $"{runtime.Catalog.Count} profiles, {VmCoreContract.Version} core contract version")
            : ("two-profile-catalog", false, $"expected 2 profiles, got {runtime.Catalog.Count}");
    }

    /// <summary>
    /// Each profile verifies its own artifact and refuses the other's, without either knowing the
    /// other exists.
    /// </summary>
    /// <remarks>
    /// The core does not try one decoder and then the next. It routes by the identity the artifact
    /// descriptor declares and hands the bytes to exactly that verifier, which is why a calculator
    /// artifact labelled as a ledger fails deterministically rather than being sniffed.
    /// </remarks>
    private static (string, bool, string) EachProfileKeepsItsOwnFormat()
    {
        using var runtime = Runtime(stamping: true, out var failure);

        if (runtime is null)
        {
            return ("formats-stay-separate", false, failure);
        }

        var calculator = CalculatorDescriptor();
        var ledger = LedgerDescriptor();

        var own = runtime.Verify(in calculator, CalculatorArtifactWriter.Sum(1, 2), CancellationToken.None);

        if (!own.TryGetArtifact(out var artifact))
        {
            return ("formats-stay-separate", false, $"calculator verification {own.Outcome}/{own.Reason}");
        }

        artifact.Dispose();

        var crossed = runtime.Verify(in ledger, CalculatorArtifactWriter.Sum(1, 2), CancellationToken.None);

        if (crossed.TryGetArtifact(out var wrong))
        {
            wrong.Dispose();
            return ("formats-stay-separate", false, "the ledger verifier accepted a calculator artifact");
        }

        return crossed.Outcome is VmOutcome.InvalidArtifact
            ? ("formats-stay-separate", true, $"cross-fed artifact rejected as {crossed.Outcome}/{crossed.Reason}")
            : ("formats-stay-separate", false, $"expected InvalidArtifact, got {crossed.Outcome}/{crossed.Reason}");
    }

    /// <summary>
    /// One profile reads the entry-point bytes as a fixed name and the other as a lookup key, and
    /// the core carries both verbatim.
    /// </summary>
    private static (string, bool, string) EntryPointBytesAreTheProfilesToRead()
    {
        using var runtime = Runtime(stamping: true, out var failure);

        if (runtime is null)
        {
            return ("entry-point-bytes", false, failure);
        }

        if (!TryBalance(runtime, "cash"u8, out var balance, out var detail))
        {
            return ("entry-point-bytes", false, detail);
        }

        if (balance.Balance != 130)
        {
            return ("entry-point-bytes", false, $"expected 130, got {balance.Balance}");
        }

        if (!TryBalance(runtime, "rent"u8, out var other, out detail))
        {
            return ("entry-point-bytes", false, detail);
        }

        return other.Balance == -70
            ? ("entry-point-bytes", true, $"cash={balance.Balance} rent={other.Balance}")
            : ("entry-point-bytes", false, $"expected -70, got {other.Balance}");
    }

    /// <summary>
    /// The ledger's optional import is bound in a runtime whose host registered it, and the profile
    /// takes its stamped branch.
    /// </summary>
    private static (string, bool, string) AnOptionalImportIsBoundWhenTheHostOffersIt()
    {
        using var runtime = Runtime(stamping: true, out var failure);

        if (runtime is null)
        {
            return ("optional-import-bound", false, failure);
        }

        if (!TryBalance(runtime, "cash"u8, out var balance, out var detail))
        {
            return ("optional-import-bound", false, detail);
        }

        return balance.IsStamped && balance.Stamp == balance.Balance + 1
            ? ("optional-import-bound", true, $"balance {balance.Balance} stamped {balance.Stamp}")
            : ("optional-import-bound", false, $"stamped={balance.IsStamped} stamp={balance.Stamp}");
    }

    /// <summary>
    /// The same profile, the same artifact, and a host that registered nothing: the import is
    /// unbound and the profile answers unstamped.
    /// </summary>
    /// <remarks>
    /// Two runtimes over one catalog, differing only in what the host offered. That an optional
    /// import is a per-runtime binding rather than a property of the profile is exactly what makes a
    /// host's policy the host's to set.
    /// </remarks>
    private static (string, bool, string) AnOptionalImportIsAbsentWhenItDoesNot()
    {
        using var runtime = Runtime(stamping: false, out var failure);

        if (runtime is null)
        {
            return ("optional-import-unbound", false, failure);
        }

        if (!TryBalance(runtime, "cash"u8, out var balance, out var detail))
        {
            return ("optional-import-unbound", false, detail);
        }

        return !balance.IsStamped && balance.Stamp == 0 && balance.Balance == 130
            ? ("optional-import-unbound", true, $"balance {balance.Balance} unstamped")
            : ("optional-import-unbound", false, $"stamped={balance.IsStamped} stamp={balance.Stamp}");
    }

    /// <summary>
    /// A profile this composition does not contain is its own outcome, not a corrupt file.
    /// </summary>
    /// <remarks>
    /// The distinction matters to a host: an artifact for a profile you did not compose is a
    /// deployment question, and an artifact that failed verification is a trust question. Collapsing
    /// them would make one look like the other.
    /// </remarks>
    private static (string, bool, string) AForeignArtifactIsUnsupportedRatherThanInvalid()
    {
        using var runtime = Runtime(stamping: true, out var failure);

        if (runtime is null)
        {
            return ("unsupported-profile", false, failure);
        }

        var foreign = new VmArtifactDescriptor(
            VmProfileId.Parse("com.example.absent"),
            1,
            VmFeatureManifestId.Parse("com.example.absent.base"),
            default,
            VmCallerIdentity.FromCanonicalIdentity("composition-workbench://artifact"));

        var result = runtime.Verify(in foreign, CalculatorArtifactWriter.Constant(1), CancellationToken.None);

        return result.Outcome is VmOutcome.UnsupportedProfile
            ? ("unsupported-profile", true, $"{result.Outcome}/{result.Reason}")
            : ("unsupported-profile", false, $"expected UnsupportedProfile, got {result.Outcome}/{result.Reason}");
    }

    /// <summary>
    /// Each profile's faults are its own, and the core acquires a case for neither.
    /// </summary>
    /// <remarks>
    /// Division by zero and an unknown account are the same core category and entirely different
    /// facts. The payload identity is what tells them apart, and each profile's own projection is
    /// what reads it - so a host that composes both gets two typed answers from one envelope shape.
    /// </remarks>
    private static (string, bool, string) LanguageFaultsStayInTheirOwnProfile()
    {
        using var runtime = Runtime(stamping: true, out var failure);

        if (runtime is null)
        {
            return ("faults-stay-separate", false, failure);
        }

        var calculator = CalculatorDescriptor();
        var verified = runtime.Verify(
            in calculator, CalculatorArtifactWriter.Quotient(1, 0), CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("faults-stay-separate", false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return ("faults-stay-separate", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
        }

        var request = new VmInvocationRequest(new VmUtf8Text("evaluate"u8));
        var arithmetic = instance.Invoke(in request, CancellationToken.None);

        if (!CalculatorProfile.TryGetFault(in arithmetic, out var divide))
        {
            return ("faults-stay-separate", false, $"expected a calculator fault, got {arithmetic.Outcome}/{arithmetic.Reason}");
        }

        // The ledger's projection must decline the calculator's payload. Both are faults, both are
        // opaque to the core, and only the profile that minted one can read it.
        if (LedgerProfile.TryGetFault(in arithmetic, out _))
        {
            return ("faults-stay-separate", false, "the ledger projected the calculator's fault");
        }

        if (!TryLedgerFault(runtime, "missing"u8, out var unknown, out var detail))
        {
            return ("faults-stay-separate", false, detail);
        }

        return ("faults-stay-separate", true, $"\"{divide.Description}\" and \"{unknown.Description}\"");
    }

    /// <summary>Prints the composition's own statement of what it declared.</summary>
    private static int ReportClosure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.Workbench");
        Console.WriteLine("profiles 2");

        foreach (var descriptor in new[] { CalculatorProfile.Descriptor, LedgerProfile.Descriptor })
        {
            Console.WriteLine(
                string.Join(
                    ' ',
                    "profile",
                    descriptor.ProfileId,
                    descriptor.PackageIdentity.PackageId,
                    descriptor.DescriptorRevision,
                    descriptor.HostCapabilityDescriptors.Length));
        }

        return 0;
    }

    private static bool TryBalance(
        VmRuntime runtime,
        ReadOnlySpan<byte> account,
        out LedgerBalance balance,
        out string detail)
    {
        var result = Ask(runtime, account, out detail);

        if (detail.Length != 0)
        {
            balance = null!;
            return false;
        }

        if (!LedgerProfile.TryGetBalance(in result, out balance))
        {
            detail = $"invocation {result.Outcome}/{result.Reason}";
            return false;
        }

        return true;
    }

    private static bool TryLedgerFault(
        VmRuntime runtime,
        ReadOnlySpan<byte> account,
        out LedgerFault fault,
        out string detail)
    {
        var result = Ask(runtime, account, out detail);

        if (detail.Length != 0)
        {
            fault = null!;
            return false;
        }

        if (result.Outcome is not VmOutcome.ProfileFault || !LedgerProfile.TryGetFault(in result, out fault))
        {
            fault = null!;
            detail = $"expected a ledger fault, got {result.Outcome}/{result.Reason}";
            return false;
        }

        return true;
    }

    /// <summary>Asks the ledger for one account, naming it in the entry-point bytes.</summary>
    private static VmInvocationResult Ask(VmRuntime runtime, ReadOnlySpan<byte> account, out string detail)
    {
        var descriptor = LedgerDescriptor();
        var verified = runtime.Verify(in descriptor, Book(), CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            detail =
                $"ledger verification {verified.Outcome}/{verified.Reason} " +
                $"{verified.Diagnostics.ExhaustedDimension}/{verified.Diagnostics.ExhaustedScope} " +
                $"code {verified.Diagnostics.ProfileDiagnosticCode}";
            return default;
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            detail = $"ledger instantiation {instantiated.Outcome}/{instantiated.Reason}";
            return default;
        }

        detail = string.Empty;
        var request = new VmInvocationRequest(new VmUtf8Text(account));
        return instance.Invoke(in request, CancellationToken.None);
    }

    /// <summary>One small book: two accounts and three postings against them.</summary>
    private static byte[] Book() =>
        LedgerArtifactWriter.Write(
            [("cash", 100), ("rent", 0)],
            [(0, 50), (1, -70), (0, -20)]);

    private static VmRuntime? Runtime(bool stamping, out string failure)
    {
        var catalog = VmCatalog.CreateBuilder()

            // Two profiles, each named through its own static accessor on its own type. There is
            // no aggregate type listing both, by design: one would reference every profile
            // assembly, and a composition that wanted only the calculator would link the ledger.
            .Add(CalculatorProfile.Descriptor)
            .Add(LedgerProfile.Descriptor)
            .Build();

        var created = VmRuntime.Create(catalog, Options(stamping));

        if (created.TryGetRuntime(out var runtime))
        {
            failure = string.Empty;
            return runtime;
        }

        failure = $"runtime creation {created.Outcome}/{created.Reason}";
        return null;
    }

    private static VmArtifactDescriptor CalculatorDescriptor() =>
        new(CalculatorProfile.Id, CalculatorFormat.FormatVersion, CalculatorProfile.Manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("composition-workbench://artifact"));

    private static VmArtifactDescriptor LedgerDescriptor() =>
        new(LedgerProfile.Id, LedgerFormat.FormatVersion, LedgerProfile.Manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("composition-workbench://artifact"));

    private static VmRuntimeCreationOptions Options(bool stamping)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (dimension is VmBudgetDimension.LiveRuntimes)
            {
                ceilings.Add(VmCeilingSpec.AdoptParentRemaining(dimension));
                continue;
            }

            // Adopting the profile default resolves to the TIGHTEST default in the catalog, which
            // in a mixed catalog is the tightest profile's rather than each profile's own. The
            // calculator needs one section, no nesting and no host call; the ledger frames two
            // sections and imports a stamping capability. Adopting throughout would hand the whole
            // runtime the calculator's numbers and leave the ledger unable to verify its own
            // artifact - a resource refusal caused by which OTHER profile happens to be composed
            // alongside it.
            //
            // So a host composing two unlike profiles states these three itself. That is not a way
            // around the calculator's limits: the effective ceiling for an operation is the
            // intersection of the host's with that profile's own hard maxima, so the calculator is
            // still held to one section, no nesting and no host call. What the explicit entry buys
            // is room for the profile whose maxima actually allow it.
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.HostCalls => VmCeilingSpec.Value(dimension, 1_024),
                VmBudgetDimension.SectionCount => VmCeilingSpec.Value(dimension, 8),
                VmBudgetDimension.StructuralDepth => VmCeilingSpec.Value(dimension, 4),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        if (stamping)
        {
            // The host's whole offer, and the only one it makes. The ledger declared this shape;
            // the calculator declared nothing, so registering it changes nothing for the
            // calculator - a capability reaches a profile through a declared import or not at all.
            capabilities.Add(VmCapabilityRegistration.Value(LedgerCapabilities.Stamp, Stamp));
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }

    /// <summary>
    /// The host's stamping service: it answers with the balance plus one.
    /// </summary>
    /// <remarks>
    /// Deliberately trivial and deliberately deterministic. A stamp that read a clock would make
    /// the composition's own output vary between the JIT, trimmed and Native AOT runs whose
    /// transcripts VM-3 compares, and the comparison is the evidence.
    /// </remarks>
    private static VmHostCallOutcome Stamp(ReadOnlySpan<long> arguments, out long result)
    {
        result = arguments.Length > 0 ? arguments[0] + 1 : 0;
        return VmHostCallOutcome.Completed;
    }
}
