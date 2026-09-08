using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.WebAssembly.Execution;

/// <summary>
/// The execution-only composition for the WebAssembly profile: a descriptor, a decoder, and no
/// compiler anywhere in the image.
/// </summary>
/// <remarks>
/// <para>
/// <b>What this root demonstrates is the whole loop.</b> The profile it composes reads a
/// WebAssembly binary module, decodes it, validates it, instantiates it and runs an exported
/// function. The checks below assert that much and no more: that the descriptor is admitted by a
/// catalog, that a valid module verifies, that a malformed one is refused with a stable triple, that
/// an invalid one is refused with a triple of its own from the other phase, that a module which
/// verifies instantiates and returns the value its body computes, and that an artifact naming an
/// unaccepted feature manifest is refused by the core before this profile is asked anything. The
/// wider surface - every trap, the memory, the tables, the branches - is exercised in the harness
/// root beside this one, because that is where an encoder can live.
/// </para>
/// <para>
/// The checks live in a composition root rather than in a test project because rule A11 forbids a
/// test project to reference a profile assembly - a rule this component's own graph depends on - so
/// a reader checks behaviour in this program's output rather than in the suite.
/// </para>
/// </remarks>
internal static class Program
{
    /// <summary>
    /// The eight bytes every WebAssembly module begins with, which are also a complete module.
    /// </summary>
    /// <remarks>
    /// A module with no sections at all is a legal encoding: the magic, the version, and nothing
    /// else. It is the smallest thing this root can hand the decoder that ought to succeed, and it
    /// needs no encoder to produce.
    /// </remarks>
    private static readonly byte[] EmptyModule =
        [0x00, 0x61, 0x73, 0x6D, 0x01, 0x00, 0x00, 0x00];

    /// <summary>The same eight bytes with one letter of the magic changed.</summary>
    private static readonly byte[] NotAModule =
        [0x00, 0x61, 0x73, 0x6E, 0x01, 0x00, 0x00, 0x00];

    /// <summary>
    /// A module that decodes and does not validate: one function whose body adds two operands that
    /// are not there.
    /// </summary>
    /// <remarks>
    /// It is the pair to <see cref="NotAModule"/> and it exists to show the two phases answering
    /// separately. Every section of it is well formed, so the decoder accepts all of it and the
    /// refusal comes from the validator - which is what a diagnostic code above the 2700 band says.
    /// </remarks>
    private static readonly byte[] InvalidModule =
    [
        0x00, 0x61, 0x73, 0x6D, 0x01, 0x00, 0x00, 0x00,
        0x01, 0x04, 0x01, 0x60, 0x00, 0x00,
        0x03, 0x02, 0x01, 0x00,
        0x0A, 0x05, 0x01, 0x03, 0x00, 0x6A, 0x0B,
    ];

    /// <summary>
    /// A module exporting one function of no parameters that answers forty-two.
    /// </summary>
    /// <remarks>
    /// It is hand-encoded here rather than produced by an encoder, because this root deliberately
    /// carries no encoder: a corpus encoder belongs in the harness root and must appear in no
    /// advertised composition's closure.
    /// </remarks>
    private static readonly byte[] AnsweringModule =
    [
        0x00, 0x61, 0x73, 0x6D, 0x01, 0x00, 0x00, 0x00,
        0x01, 0x05, 0x01, 0x60, 0x00, 0x01, 0x7F,
        0x03, 0x02, 0x01, 0x00,
        0x07, 0x0A, 0x01, 0x06, 0x61, 0x6E, 0x73, 0x77, 0x65, 0x72, 0x00, 0x00,
        0x0A, 0x06, 0x01, 0x04, 0x00, 0x41, 0x2A, 0x0B,
    ];

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
                TheCatalogAdmitsTheDescriptor(),
                AWellFormedModuleVerifies(),
                AMalformedModuleIsRefusedWithItsOwnTriple(),
                AnInvalidModuleIsRefusedByTheOtherPhase(),
                AVerifiedModuleInstantiatesAndRuns(),
                AnUnacceptedManifestIsRefusedBeforeTheProfileIsAsked(),
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
                    ? $"broiler-wasm-execution: {checks.Count} checks passed, core contract version {VmCoreContract.Version}"
                    : $"broiler-wasm-execution: {failed} of {checks.Count} checks FAILED");

            return failed == 0 ? 0 : 1;
        }
        catch (Exception failure)
        {
            Console.WriteLine(
                $"broiler-wasm-execution: unhandled {failure.GetType().Name}: {failure.Message}");

            return 2;
        }
    }

    /// <summary>The descriptor is complete enough for a catalog to accept it.</summary>
    private static (string, bool, string) TheCatalogAdmitsTheDescriptor()
    {
        using var runtime = Runtime(out var failure);

        return runtime is null
            ? ("catalog-admits-the-descriptor", false, failure)
            : ("catalog-admits-the-descriptor", true,
                $"{WebAssemblyProfile.Id} revision {WebAssemblyProfile.Descriptor.DescriptorRevision}");
    }

    /// <summary>Verification decodes a module with no sections and answers normally.</summary>
    private static (string, bool, string) AWellFormedModuleVerifies()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("a-well-formed-module-verifies", false, failure);
        }

        var descriptor = Descriptor(WebAssemblyProfile.SliceManifest);
        var verified = runtime.Verify(in descriptor, EmptyModule, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("a-well-formed-module-verifies", false,
                $"{verified.Outcome}/{verified.Reason}/{verified.Diagnostics.ProfileDiagnosticCode}");
        }

        using (artifact)
        {
            var carries = artifact.TryGetState(out var state) && state is WasmModule;

            return ("a-well-formed-module-verifies", carries,
                $"{verified.Outcome}/{verified.Reason}, decoded module carried: {carries}");
        }
    }

    /// <summary>
    /// A payload whose magic is wrong is refused as a malformed encoding, with this profile's own
    /// code and a byte position.
    /// </summary>
    private static (string, bool, string) AMalformedModuleIsRefusedWithItsOwnTriple()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("a-malformed-module-is-refused", false, failure);
        }

        var descriptor = Descriptor(WebAssemblyProfile.SliceManifest);
        var verified = runtime.Verify(in descriptor, NotAModule, CancellationToken.None);

        var expected =
            verified.Outcome is VmOutcome.InvalidArtifact &&
            verified.Reason is VmReason.MalformedEncoding &&
            verified.Diagnostics.ProfileDiagnosticCode == (int)WebAssemblyDiagnosticCode.WrongMagic;

        return ("a-malformed-module-is-refused", expected,
            $"{verified.Outcome}/{verified.Reason}/{verified.Diagnostics.ProfileDiagnosticCode} " +
            $"at offset {verified.Diagnostics.SourcePosition.ByteOffset}");
    }

    /// <summary>
    /// A module whose sections are all well formed and whose one function body cannot be typed is
    /// refused by the validator, with a code from the validation band and not the decoding one.
    /// </summary>
    private static (string, bool, string) AnInvalidModuleIsRefusedByTheOtherPhase()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("an-invalid-module-is-refused-by-validation", false, failure);
        }

        var descriptor = Descriptor(WebAssemblyProfile.SliceManifest);
        var verified = runtime.Verify(in descriptor, InvalidModule, CancellationToken.None);

        var expected =
            verified.Outcome is VmOutcome.InvalidArtifact &&
            verified.Reason is VmReason.SemanticValidationFailed &&
            verified.Diagnostics.ProfileDiagnosticCode ==
                (int)WebAssemblyDiagnosticCode.OperandStackUnderflow;

        return ("an-invalid-module-is-refused-by-validation", expected,
            $"{verified.Outcome}/{verified.Reason}/{verified.Diagnostics.ProfileDiagnosticCode} " +
            $"at body offset {verified.Diagnostics.SourcePosition.ByteOffset}");
    }

    /// <summary>
    /// A module that verified instantiates, and its exported function returns what its body
    /// computes.
    /// </summary>
    /// <remarks>
    /// This is the check that keeps the previous ones honest. A verifier that answered yes without
    /// anything downstream ever running the module would be grading its own homework, and the value
    /// this check reads back is the only thing that says otherwise.
    /// </remarks>
    private static (string, bool, string) AVerifiedModuleInstantiatesAndRuns()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("a-verified-module-instantiates-and-runs", false, failure);
        }

        var descriptor = Descriptor(WebAssemblyProfile.SliceManifest);
        var verified = runtime.Verify(in descriptor, AnsweringModule, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("a-verified-module-instantiates-and-runs", false,
                $"verification {verified.Outcome}/{verified.Reason}");
        }

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return ("a-verified-module-instantiates-and-runs", false,
                    $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
            }

            var request = new VmInvocationRequest(new VmUtf8Text("6:answer"u8));
            var completed = instance.Invoke(in request, CancellationToken.None);

            if (!WebAssemblyProfile.TryGetResults(in completed, out var results) ||
                results.Count != 1 ||
                !results.TryGetValue(0, out var value) ||
                value.AsInt32 != 42)
            {
                return ("a-verified-module-instantiates-and-runs", false,
                    $"invocation {completed.Outcome}/{completed.Reason}");
            }

            return ("a-verified-module-instantiates-and-runs", true,
                $"{completed.Outcome}/{completed.Reason} returned {value.AsInt32}");
        }
    }

    /// <summary>
    /// A manifest inside this profile's namespace that the descriptor does not accept is refused by
    /// the core, and the profile is never asked.
    /// </summary>
    private static (string, bool, string) AnUnacceptedManifestIsRefusedBeforeTheProfileIsAsked()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("unaccepted-manifest-is-refused", false, failure);
        }

        var descriptor = Descriptor(VmFeatureManifestId.Parse("broiler.webassembly.core1"));
        var verified = runtime.Verify(in descriptor, EmptyModule, CancellationToken.None);

        var expected =
            verified.Outcome is VmOutcome.InvalidArtifact &&
            verified.Reason is VmReason.UnsupportedFeatureManifest;

        return ("unaccepted-manifest-is-refused", expected, $"{verified.Outcome}/{verified.Reason}");
    }

    /// <summary>
    /// Prints what this composition is, as the composition register documents it.
    /// </summary>
    private static int ReportClosure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.WebAssembly.Execution");
        Console.WriteLine("label execution-only");
        Console.WriteLine("carries-lowering no");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                WebAssemblyProfile.Id,
                WebAssemblyProfile.Descriptor.PackageIdentity.PackageId,
                WebAssemblyProfile.Descriptor.DescriptorRevision,
                WebAssemblyProfile.Descriptor.HostCapabilityDescriptors.Length));
        Console.WriteLine(string.Join(' ', "manifest", WebAssemblyProfile.SliceManifest));
        Console.WriteLine(
            string.Join(
                ' ',
                "format-versions",
                WebAssemblyProfile.Descriptor.SupportedFormatVersions.Min,
                WebAssemblyProfile.Descriptor.SupportedFormatVersions.Max));
        Console.WriteLine("decoder yes");
        Console.WriteLine("validator yes");
        Console.WriteLine("interpreter yes");
        Console.WriteLine("linker no");

        return 0;
    }

    private static VmRuntime? Runtime(out string failure)
    {
        var catalog = VmCatalog.CreateBuilder()

            // The whole of the composition: one profile, named by its own static accessor. There is
            // no aggregate profile type to name instead, by design - one would reference every
            // profile assembly and this closure would stop being a single-profile closure.
            .Add(WebAssemblyProfile.Descriptor)
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

    private static VmArtifactDescriptor Descriptor(VmFeatureManifestId manifest) =>
        new(WebAssemblyProfile.Id, 1, manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("composition-wasm-execution://artifact"));

    private static VmRuntimeCreationOptions Options()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: ImmutableArray<VmCapabilityRegistration>.Empty);
    }
}
