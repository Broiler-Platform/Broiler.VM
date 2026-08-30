using Broiler.VM;
using Com.Example.Calculator;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.Calculator;

/// <summary>
/// The single-profile composition: one application-local profile, named directly, composed by typed
/// registration, published and run.
/// </summary>
/// <remarks>
/// <para>
/// Everything VM-3 claims is visible in this file's references. The profile arrives through a static
/// accessor on its own type; no name is looked up, no directory is scanned, no assembly is loaded,
/// and nothing here or in the profile calls into reflection. The composition is exactly what is
/// written down, which is what makes a closure report over a publish of this project mean anything.
/// </para>
/// <para>
/// It also registers a host capability the composed profile does not import, and that is
/// deliberate: registering a capability never implies a provider. The calculator imports nothing, so
/// it reaches nothing, and the capability sits in the runtime unreachable from the guest. A host
/// that had to curate its registrations per profile would be doing the core's containment work by
/// hand.
/// </para>
/// </remarks>
internal static class Program
{
    /// <summary>The capability this root registers and the composed profile cannot reach.</summary>
    private static readonly VmCapabilityId UnreachableCapability =
        VmCapabilityId.Parse("com.example.host.unreachable");

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
                Arithmetic(),
                LanguageFaultStaysALanguageFault(),
                UnknownEntryPointIsTheProfilesBusiness(),
                RefusedBeforeExecution(),
                RegisteringACapabilityImpliesNoProvider(),
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
                    ? $"broiler-vm-composition-calculator: {checks.Count} checks passed, core contract version {VmCoreContract.Version}"
                    : $"broiler-vm-composition-calculator: {failed} of {checks.Count} checks FAILED");

            return failed == 0 ? 0 : 1;
        }
        catch (Exception failure)
        {
            Console.WriteLine(
                $"broiler-vm-composition-calculator: unhandled {failure.GetType().Name}: {failure.Message}");

            return 2;
        }
    }

    /// <summary>The composed profile verifies, instantiates and evaluates an artifact.</summary>
    private static (string, bool, string) Arithmetic()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("arithmetic", false, failure);
        }

        var descriptor = Descriptor();
        var verified = runtime.Verify(
            in descriptor, CalculatorArtifactWriter.Sum(20, 22), CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("arithmetic", false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return ("arithmetic", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
        }

        var request = new VmInvocationRequest(new VmUtf8Text("evaluate"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        if (!CalculatorProfile.TryGetAnswer(in result, out var answer) || answer.Value != 42)
        {
            return ("arithmetic", false, $"invocation {result.Outcome}/{result.Reason}");
        }

        return ("arithmetic", true, $"evaluated to {answer.Value}");
    }

    /// <summary>
    /// Division by zero reaches the caller as this profile's own typed fault, behind the
    /// profile-neutral fault category.
    /// </summary>
    /// <remarks>
    /// The core has no case for division, and acquires none by hosting a profile that does. What it
    /// carries is a payload whose identity it checked and whose contents it never read.
    /// </remarks>
    private static (string, bool, string) LanguageFaultStaysALanguageFault()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("language-fault", false, failure);
        }

        var descriptor = Descriptor();
        var verified = runtime.Verify(
            in descriptor, CalculatorArtifactWriter.Quotient(1, 0), CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("language-fault", false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return ("language-fault", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
        }

        var request = new VmInvocationRequest(new VmUtf8Text("evaluate"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        if (result.Outcome is not VmOutcome.ProfileFault ||
            !CalculatorProfile.TryGetFault(in result, out var fault))
        {
            return ("language-fault", false, $"expected a fault, got {result.Outcome}/{result.Reason}");
        }

        return ("language-fault", true, $"{result.Outcome}/{result.Reason}: {fault.Description}");
    }

    /// <summary>An entry point the profile does not know is the profile's answer, not the core's.</summary>
    private static (string, bool, string) UnknownEntryPointIsTheProfilesBusiness()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("unknown-entry-point", false, failure);
        }

        var descriptor = Descriptor();
        var verified = runtime.Verify(
            in descriptor, CalculatorArtifactWriter.Constant(1), CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("unknown-entry-point", false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return ("unknown-entry-point", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
        }

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        return result.Outcome is VmOutcome.ProfileFault
            ? ("unknown-entry-point", true, $"{result.Outcome}/{result.Reason}")
            : ("unknown-entry-point", false, $"expected a fault, got {result.Outcome}/{result.Reason}");
    }

    /// <summary>
    /// An artifact whose stack would exceed the profile's own maximum is refused at verification.
    /// </summary>
    /// <remarks>
    /// Which is what makes the executor's fixed-size stack safe: the size comes from a number
    /// verification computed, never from one the payload chose.
    /// </remarks>
    private static (string, bool, string) RefusedBeforeExecution()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("refused-before-execution", false, failure);
        }

        var descriptor = Descriptor();
        var result = runtime.Verify(
            in descriptor,
            CalculatorArtifactWriter.DeepStack(CalculatorFormat.MaximumStackDepth + 1),
            CancellationToken.None);

        if (result.TryGetArtifact(out var artifact))
        {
            artifact.Dispose();
            return ("refused-before-execution", false, "an over-deep program verified");
        }

        return result.Outcome is VmOutcome.InvalidArtifact
            ? ("refused-before-execution", true, $"{result.Outcome}/{result.Reason}")
            : ("refused-before-execution", false, $"expected InvalidArtifact, got {result.Outcome}/{result.Reason}");
    }

    /// <summary>
    /// The runtime carries a registered capability the composed profile never declared, and the
    /// profile still reaches nothing.
    /// </summary>
    /// <remarks>
    /// The binding table a profile sees has exactly as many slots as it declared imports, which for
    /// this profile is none. Registration is the host's offer; a declared import is what accepts
    /// one, and there is no third path.
    /// </remarks>
    private static (string, bool, string) RegisteringACapabilityImpliesNoProvider()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("registration-implies-no-provider", false, failure);
        }

        if (CalculatorProfile.Descriptor.HostCapabilityDescriptors.Length != 0)
        {
            return ("registration-implies-no-provider", false, "the calculator declared an import");
        }

        var descriptor = Descriptor();
        var verified = runtime.Verify(
            in descriptor, CalculatorArtifactWriter.Product(6, 7), CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("registration-implies-no-provider", false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return ("registration-implies-no-provider", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
        }

        var request = new VmInvocationRequest(new VmUtf8Text("evaluate"u8));
        var result = instance.Invoke(in request, CancellationToken.None);

        return CalculatorProfile.TryGetAnswer(in result, out var answer) && answer.Value == 42
            ? ("registration-implies-no-provider", true, "one capability registered, zero bindings offered")
            : ("registration-implies-no-provider", false, $"invocation {result.Outcome}/{result.Reason}");
    }

    /// <summary>
    /// Prints what this composition is, as the composition register documents it: the profiles it
    /// names, their identities, and the assemblies its own closure is expected to contain.
    /// </summary>
    /// <remarks>
    /// The authoritative closure is the published output, which the evidence bundle lists from the
    /// file system. This is the composition's own statement of what it declared, so a report that
    /// disagrees with the publish has two independent halves to compare rather than one.
    /// </remarks>
    private static int ReportClosure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.Calculator");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                CalculatorProfile.Id,
                CalculatorProfile.Descriptor.PackageIdentity.PackageId,
                CalculatorProfile.Descriptor.DescriptorRevision,
                CalculatorProfile.Descriptor.HostCapabilityDescriptors.Length));

        return 0;
    }

    private static VmRuntime? Runtime(out string failure)
    {
        var catalog = VmCatalog.CreateBuilder()

            // The whole of the composition: one profile, named by its own static accessor. There
            // is no aggregate profile type to name instead, by design - one would reference every
            // profile assembly and this closure would stop being a single-profile closure.
            .Add(CalculatorProfile.Descriptor)
            .Build();

        var created = VmRuntime.Create(catalog, Options());

        if (created.TryGetRuntime(out var runtime))
        {
            failure = string.Empty;
            return runtime;
        }

        failure = $"runtime creation {created.Outcome}/{created.Reason}";
        return null;
    }

    private static VmArtifactDescriptor Descriptor() =>
        new(CalculatorProfile.Id, CalculatorFormat.FormatVersion, CalculatorProfile.Manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("composition-calculator://artifact"));

    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        capabilities.Add(VmCapabilityRegistration.Value(
            new VmHostCapabilityDescriptor(
                UnreachableCapability,
                version: 1,
                VmCapabilitySignatureId.FromCanonicalDescription("(i64)->i64"),
                VmCapabilityKind.Value,
                VmCapabilityReentrancy.NonReentrant,
                VmCapabilityThreadAffinity.CallerThread,
                VmExceptionTranslation.TerminateOperation),
            Unreachable));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }

    /// <summary>A handler no composed profile can reach, because none of them imports it.</summary>
    private static VmHostCallOutcome Unreachable(ReadOnlySpan<long> arguments, out long result)
    {
        result = 0;
        return VmHostCallOutcome.Refused;
    }
}
