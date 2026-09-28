using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Profile.WebAssembly;
using Broiler.VM.Ubc;
using System.Collections.Immutable;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Execution;

/// <summary>
/// The WebAssembly profile's execution composition: the family's descriptor over the bytecode
/// emitter, and the translator that turns a module into what the core verifies.
/// </summary>
/// <remarks>
/// <para>
/// <b>What this root demonstrates is the whole loop.</b> The profile it composes reads a
/// WebAssembly binary module, decodes it, validates it and translates it into universal bytecode;
/// the core verifies that artifact under the family's descriptor, which this root builds over the
/// bytecode emitter, and the emitter instantiates it and runs an exported function. The checks below
/// assert that much and no more: that the descriptor is admitted by a catalog, that a valid module
/// translates and its program verifies, that a malformed one is refused with a stable triple, that an
/// invalid one is refused with a triple of its own from the other phase, that a module which verifies
/// instantiates and returns the value its body computes, and that an artifact naming an unaccepted
/// feature manifest is refused by the core before the family is asked anything. The wider surface -
/// every trap, the memory, the tables, the branches - is exercised in the harness root beside this
/// one, because that is where an encoder can live.
/// </para>
/// <para>
/// <b>It is no longer an execution-only image, and its catalog says so.</b> Until the universal
/// bytecode programme's milestone UBC-4 the profile carried no lowering at all, so this image could
/// not turn anything into an artifact and ran the module it was handed as the artifact. It now
/// carries the translator, which lowers a module into a universal bytecode artifact at run time, and
/// the core never sees a module: the catalog prints <c>label none</c> and <c>carries-lowering yes</c>
/// and names the translator and the form.
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
                AnUnacceptedManifestIsRefusedBeforeTheFamilyIsAsked(),
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
                $"{WebAssemblyProfile.Id} revision {Descriptor.DescriptorRevision}");
    }

    /// <summary>
    /// A module with no sections translates, and the core verifies its artifact and holds the
    /// universal bytecode program it read.
    /// </summary>
    private static (string, bool, string) AWellFormedModuleVerifies()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("a-well-formed-module-verifies", false, failure);
        }

        var verified = Verify(runtime, EmptyModule, ArtifactDescriptor(WebAssemblyProfile.SliceManifest));

        if (!verified.TryGetArtifact(out var artifact))
        {
            return ("a-well-formed-module-verifies", false,
                $"{verified.Outcome}/{verified.Reason}/{verified.Code}");
        }

        using (artifact)
        {
            var carries = artifact.TryGetState(out var state) && state is UbcVerifiedProgram;

            return ("a-well-formed-module-verifies", carries,
                $"{verified.Outcome}/{verified.Reason}, universal bytecode program carried: {carries}");
        }
    }

    /// <summary>
    /// A payload whose magic is wrong is refused as a malformed encoding, with this profile's own
    /// code and a byte position. The translator refuses it, in the fields the core refused it in
    /// before the universal bytecode, and the core is never asked.
    /// </summary>
    private static (string, bool, string) AMalformedModuleIsRefusedWithItsOwnTriple()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("a-malformed-module-is-refused", false, failure);
        }

        var verified = Verify(runtime, NotAModule, ArtifactDescriptor(WebAssemblyProfile.SliceManifest));

        var expected =
            verified.Outcome is VmOutcome.InvalidArtifact &&
            verified.Reason is VmReason.MalformedEncoding &&
            verified.Code == (int)WebAssemblyDiagnosticCode.WrongMagic;

        return ("a-malformed-module-is-refused", expected,
            $"{verified.Outcome}/{verified.Reason}/{verified.Code} " +
            $"at offset {verified.Position.ByteOffset}");
    }

    /// <summary>
    /// A module whose sections are all well formed and whose one function body cannot be typed is
    /// refused by the validator, with a code from the validation band and not the decoding one. The
    /// validator runs inside the translator, which answers the refusal as the core answered it.
    /// </summary>
    private static (string, bool, string) AnInvalidModuleIsRefusedByTheOtherPhase()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("an-invalid-module-is-refused-by-validation", false, failure);
        }

        var verified = Verify(runtime, InvalidModule, ArtifactDescriptor(WebAssemblyProfile.SliceManifest));

        var expected =
            verified.Outcome is VmOutcome.InvalidArtifact &&
            verified.Reason is VmReason.SemanticValidationFailed &&
            verified.Code == (int)WebAssemblyDiagnosticCode.OperandStackUnderflow;

        return ("an-invalid-module-is-refused-by-validation", expected,
            $"{verified.Outcome}/{verified.Reason}/{verified.Code} " +
            $"at body offset {verified.Position.ByteOffset}");
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

        var verified = Verify(runtime, AnsweringModule, ArtifactDescriptor(WebAssemblyProfile.SliceManifest));

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
    /// the core, and the family is never asked.
    /// </summary>
    /// <remarks>
    /// <b>What happens first is now the translation.</b> A module is translated before anything is
    /// verified, and the translator reads no manifest - so the empty module translates, and the core
    /// then refuses the artifact descriptor naming <c>broiler.webassembly.core1</c> before the family's
    /// verifier hook sees a byte of the artifact. The line this check prints says both.
    /// </remarks>
    private static (string, bool, string) AnUnacceptedManifestIsRefusedBeforeTheFamilyIsAsked()
    {
        using var runtime = Runtime(out var failure);

        if (runtime is null)
        {
            return ("unaccepted-manifest-is-refused", false, failure);
        }

        var verified = Verify(
            runtime, EmptyModule, ArtifactDescriptor(VmFeatureManifestId.Parse("broiler.webassembly.core1")));

        var expected =
            verified.Stage is Stage.Core &&
            verified.Outcome is VmOutcome.InvalidArtifact &&
            verified.Reason is VmReason.UnsupportedFeatureManifest;

        return ("unaccepted-manifest-is-refused", expected,
            verified.Stage is Stage.Core
                ? $"translated, then {verified.Outcome}/{verified.Reason}"
                : $"not translated: {verified.Outcome}/{verified.Reason}/{verified.Code}");
    }

    /// <summary>
    /// Prints what this composition is, as the composition register documents it.
    /// </summary>
    private static int ReportClosure()
    {
        Console.WriteLine(
            $"# broiler-vm-composition core-contract-version={VmCoreContract.Version} " +
            $"ubc-contract-version={UbcContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.WebAssembly.Execution");

        // THE LABEL THIS IMAGE CARRIED UNTIL THE TRANSLATOR ARRIVED IS NOT PRINTED. It was
        // `execution-only`, and what it meant here was that the profile carried no lowering at all, so
        // nothing in the image could turn anything into an artifact. The translator is a lowering from
        // a module into universal bytecode, it runs in this image, and so the image claims no label.
        Console.WriteLine("label none");
        Console.WriteLine("carries-lowering yes");
        Console.WriteLine("profiles 1");
        Console.WriteLine(
            string.Join(
                ' ',
                "profile",
                WebAssemblyProfile.Id,
                Descriptor.PackageIdentity.PackageId,
                Descriptor.DescriptorRevision,
                Descriptor.HostCapabilityDescriptors.Length));
        Console.WriteLine(string.Join(' ', "manifest", WebAssemblyProfile.SliceManifest));
        Console.WriteLine(
            string.Join(
                ' ',
                "format-versions",
                Descriptor.SupportedFormatVersions.Min,
                Descriptor.SupportedFormatVersions.Max));

        // The lowering, named from what the image holds: the translator's identity and version, and the
        // one form that runs what it writes.
        Console.WriteLine(
            string.Join(
                ' ',
                "translator",
                WasmTranslator.TranslatorIdentity,
                WasmTranslator.TranslatorVersion.ToString(CultureInfo.InvariantCulture)));
        Console.WriteLine(
            $"form {UbcBytecodeEmitter.Form.Identity} " +
            UbcBytecodeEmitter.Form.SemanticVersion.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine("decoder yes");
        Console.WriteLine("validator yes");
        Console.WriteLine("interpreter yes");
        Console.WriteLine("linker no");

        return 0;
    }

    /// <summary>The family's descriptor over the bytecode emitter: the one profile row this root composes.</summary>
    /// <remarks>
    /// A family names no emitter, so the root that composes it builds its descriptor over the form it
    /// chose. This one composes the bytecode emitter and nothing else.
    /// </remarks>
    private static VmProfileDescriptor Descriptor { get; } = UbcDescriptors.Build(
        WebAssemblyProfile.Registration,
        WebAssemblyProfile.Declaration,
        UbcEmitterSet.Create(UbcBytecodeEmitter.Form));

    private static VmRuntime? Runtime(out string failure)
    {
        var catalog = VmCatalog.CreateBuilder()

            // The whole of the composition: one profile, named by its own static accessors and built
            // here over the one form. There is no aggregate profile type to name instead, by design -
            // one would reference every profile assembly and this closure would stop being a
            // single-profile closure.
            .Add(Descriptor)
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

    /// <summary>The artifact descriptor a translation's artifact is verified under.</summary>
    private static VmArtifactDescriptor ArtifactDescriptor(VmFeatureManifestId manifest) =>
        new(WebAssemblyProfile.Id, UbcFormat.FormatVersion, manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("composition-wasm-execution://artifact"));

    /// <summary>Which stage answered a module.</summary>
    private enum Stage
    {
        /// <summary>The translator refused it, and the core was never asked.</summary>
        Translation,

        /// <summary>It translated, and the core answered over its artifact.</summary>
        Core,
    }

    /// <summary>
    /// What verifying one module answered, in the fields the core's refusal of a bare module was
    /// reported in: a translation's refusal where the translator refused it, the core's answer over
    /// the artifact where it translated.
    /// </summary>
    private sealed record Verified(
        Stage Stage,
        VmOutcome Outcome,
        VmReason Reason,
        int Code,
        VmSourcePosition Position,
        VmVerifiedArtifact? Artifact)
    {
        internal bool TryGetArtifact(out VmVerifiedArtifact artifact)
        {
            artifact = Artifact!;
            return Artifact is not null;
        }
    }

    /// <summary>
    /// Translates <paramref name="module"/> under the ceilings the core would verify it under in
    /// <paramref name="runtime"/>, and verifies the artifact under <paramref name="descriptor"/>.
    /// </summary>
    /// <remarks>
    /// The ceilings are the runtime's, read off its budget snapshot, intersected with the profile's
    /// hard maxima and with the request, as the core intersects them for the verification of the
    /// artifact - so the two run under one set.
    /// </remarks>
    private static Verified Verify(VmRuntime runtime, byte[] module, VmArtifactDescriptor descriptor)
    {
        var snapshot = runtime.GetBudgetSnapshot();
        var values = new ulong[VmBudgetDimensions.Count];

        foreach (var dimension in VmBudgetDimensions.All)
        {
            values[(int)dimension] = snapshot.EffectiveCeiling(dimension);
        }

        if (!VmLimitVector.TryCreate(values, out var host))
        {
            throw new InvalidOperationException("the runtime's ceilings do not form a limit vector");
        }

        var ceilings = VmLimitVector.Intersect(
            VmLimitVector.Intersect(host, Descriptor.ProfileHardMaxima),
            descriptor.RequestedLimits.IsEmpty ? VmLimitVector.Unconstrained : descriptor.RequestedLimits);

        var translation = WasmTranslator.Translate(module, ceilings, CancellationToken.None);

        if (!translation.Succeeded)
        {
            return new Verified(
                Stage.Translation, translation.Outcome, translation.Reason, (int)translation.Code,
                translation.Position, null);
        }

        var verified = runtime.Verify(in descriptor, translation.Artifact.AsSpan(), CancellationToken.None);

        return new Verified(
            Stage.Core,
            verified.Outcome,
            verified.Reason,
            verified.Diagnostics.ProfileDiagnosticCode,
            verified.Diagnostics.SourcePosition,
            verified.TryGetArtifact(out var artifact) ? artifact : null);
    }

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
