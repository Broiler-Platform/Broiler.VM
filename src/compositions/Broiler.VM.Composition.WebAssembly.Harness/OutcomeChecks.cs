using Broiler.VM.Abstractions;
using Broiler.VM.Profile.WebAssembly;
using Broiler.VM.Ubc;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The verifier outcomes WA-1's gate names that the retained corpus cannot hold, and the position an
/// invalid artifact carries, which the corpus manifest does not record.
/// </summary>
/// <remarks>
/// <para>
/// <b>What the corpus already does, and what it cannot.</b> WA-1 asks for each of the five verifier
/// outcomes from a named case, the invalid-artifact case carrying a diagnostic code and a byte position.
/// Three outcomes are retained corpus entries: an accepted module, an invalid artifact with its code, and
/// a resource exhaustion naming its dimension and scope. The other two are not properties of a module's
/// bytes, so no corpus entry can hold them: a cancellation is what a caller's token says, and an
/// unsupported profile is what a descriptor names. They are checks here. And the manifest records a
/// refusal's code and not its position, so the position the published registry documents is checked
/// here too, at three places whose four coordinates the registry's header states.
/// </para>
/// <para>
/// Every answer is written down beside its check before the core or the translator is asked.
/// </para>
/// </remarks>
internal static class OutcomeChecks
{
    private const string Caller = "urn:broiler:wasm-harness/outcomes";

    internal static int Report(VmRuntime runtime, bool verbose)
    {
        List<(string Name, bool Passed, string Detail)> checks =
        [
            // Decoding, between sections: a byte where a section identifier is expected names none. The
            // position is the payload offset of that byte, eight, with no section ordinal, identifier or
            // item yet.
            Positioned(runtime, "outcome-an-invalid-artifact-carries-its-code-and-position-between-sections",
                [.. CorpusStore.Preamble, 0x0E, 0x00],
                VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.UnknownSectionId, new(-1, 8, -1, -1)),

            // Decoding, inside a section: the first function type's tag. The type section is the first
            // framed, ordinal zero, identifier one, and its tag byte is item zero at payload offset eleven.
            Positioned(runtime, "outcome-an-invalid-artifact-carries-its-code-and-position-inside-a-section",
                [.. CorpusStore.Preamble, .. CorpusStore.Section(1, [0x01, 0x61, 0x00, 0x00])],
                VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.MalformedFunctionTypeTag, new(0, 11, 1, 0)),

            // Validation, inside a body: i32.add with nothing on the stack. The validator knows no section
            // ordinal; the offset is within the body's instructions, after the opcode it read; the
            // identifier is the code section's, ten, and the item is function zero.
            Positioned(runtime, "outcome-an-invalid-artifact-carries-its-code-and-position-inside-a-body",
                CorpusStore.FunctionModule([], [], [0x6A]),
                VmReason.SemanticValidationFailed, WebAssemblyDiagnosticCode.OperandStackUnderflow, new(-1, 1, 10, 0)),

            SuspensionRefused(runtime),
            TranslationCancelled(runtime),
            VerificationCancelled(runtime),
            UnsupportedProfile(runtime),
        ];

        var failed = 0;
        Console.WriteLine($"# outcomes: {checks.Count} checks");

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

        Console.WriteLine($"# outcomes: {checks.Count - failed} of {checks.Count} checks passed");
        return failed;
    }

    private static (string, bool, string) Positioned(
        VmRuntime runtime, string name, byte[] module, VmReason reason, WebAssemblyDiagnosticCode code, VmSourcePosition expected)
    {
        var verified = ModuleVerification.Verify(runtime, module, Caller, name);
        var at = verified.Position;
        var passed =
            verified.Outcome == VmOutcome.InvalidArtifact && verified.Reason == reason && verified.Code == (int)code &&
            at.SectionIndex == expected.SectionIndex && at.ByteOffset == expected.ByteOffset &&
            at.ProfileCoordinate0 == expected.ProfileCoordinate0 && at.ProfileCoordinate1 == expected.ProfileCoordinate1;

        return (name, passed,
            $"{verified.Outcome}/{verified.Reason}/{verified.Code.ToString(CultureInfo.InvariantCulture)}@{Describe(at)}" +
            (passed ? string.Empty : $", expected InvalidArtifact/{reason}/{((int)code).ToString(CultureInfo.InvariantCulture)}@{Describe(expected)}"));
    }

    /// <summary>
    /// The execution step <c>Suspended</c> is unreachable from this profile, and the refusal that says so
    /// is tested rather than an instruction minted to reach it: the profile declares no external
    /// suspension, so a request to suspend an operation of its is refused as undeclared.
    /// </summary>
    private static (string, bool, string) SuspensionRefused(VmRuntime runtime)
    {
        const string Name = "step-a-suspension-request-is-refused-as-undeclared";
        var verified = ModuleVerification.Verify(runtime, CorpusStore.FunctionModule([], [], [], exportSection: [0x01, 0x01, 0x66, 0x00, 0x00]), Caller, Name);

        if (!verified.TryGetArtifact(out var artifact))
        {
            return (Name, false, $"verification {verified.Outcome}/{verified.Reason}");
        }

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return (Name, false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
            }

            using (instance)
            {
                var request = new VmInvocationRequest(new VmUtf8Text("1:f"u8.ToArray()));
                var answered = instance.Invoke(in request, CancellationToken.None, out var handle);

                using (handle)
                {
                    var refused = handle.RequestSuspend();
                    var passed = answered.Outcome == VmOutcome.Normal &&
                        refused.Kind == VmControlOutcome.Unsupported &&
                        refused.Reason == VmReason.ExternalSuspensionNotDeclared;

                    return (Name, passed, $"invocation {answered.Outcome}, suspension {refused.Kind}/{refused.Reason}");
                }
            }
        }
    }

    /// <summary>A translation handed a token already cancelled answers a cancellation, not a module.</summary>
    private static (string, bool, string) TranslationCancelled(VmRuntime runtime)
    {
        const string Name = "outcome-a-cancelled-translation-answers-a-cancellation";
        using var source = new CancellationTokenSource();
        source.Cancel();

        var descriptor = ModuleVerification.ArtifactDescriptor(WebAssemblyProfile.SliceManifest, Caller);
        var translation = WasmTranslator.Translate(
            CorpusStore.CanonicalModule(), ModuleVerification.Ceilings(runtime, in descriptor), source.Token);

        return (Name,
            translation.Outcome == VmOutcome.Cancellation && translation.Reason == VmReason.Cancelled,
            $"{translation.Outcome}/{translation.Reason}");
    }

    /// <summary>A verification handed a token already cancelled answers a cancellation, not an artifact.</summary>
    private static (string, bool, string) VerificationCancelled(VmRuntime runtime)
    {
        const string Name = "outcome-a-cancelled-verification-answers-a-cancellation";

        if (!TryTranslate(runtime, out var artifact, out var failure))
        {
            return (Name, false, failure);
        }

        using var source = new CancellationTokenSource();
        source.Cancel();

        var descriptor = ModuleVerification.ArtifactDescriptor(WebAssemblyProfile.SliceManifest, Caller);
        var verified = runtime.Verify(in descriptor, artifact, source.Token);
        Release(in verified);

        return (Name,
            verified.Outcome == VmOutcome.Cancellation && verified.Reason == VmReason.Cancelled,
            $"{verified.Outcome}/{verified.Reason}");
    }

    /// <summary>An artifact whose descriptor names a profile the catalog does not hold is an unsupported profile.</summary>
    private static (string, bool, string) UnsupportedProfile(VmRuntime runtime)
    {
        const string Name = "outcome-a-descriptor-naming-a-profile-the-catalog-lacks-answers-unsupported-profile";

        if (!TryTranslate(runtime, out var artifact, out var failure))
        {
            return (Name, false, failure);
        }

        // A well-formed descriptor of a profile this composition does not hold: its manifest has to sit
        // under the profile's own namespace, or the core refuses the descriptor as malformed first.
        var descriptor = new VmArtifactDescriptor(
            VmProfileId.Parse("broiler.absent"), UbcFormat.FormatVersion, VmFeatureManifestId.Parse("broiler.absent.slice"), default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));
        var verified = runtime.Verify(in descriptor, artifact, CancellationToken.None);
        Release(in verified);

        return (Name,
            verified.Outcome == VmOutcome.UnsupportedProfile && verified.Reason == VmReason.ProfileNotInCatalog,
            $"{verified.Outcome}/{verified.Reason}");
    }

    private static bool TryTranslate(VmRuntime runtime, out byte[] artifact, out string failure)
    {
        var descriptor = ModuleVerification.ArtifactDescriptor(WebAssemblyProfile.SliceManifest, Caller);
        var translation = WasmTranslator.Translate(
            CorpusStore.CanonicalModule(), ModuleVerification.Ceilings(runtime, in descriptor), CancellationToken.None);

        artifact = translation.Succeeded ? [.. translation.Artifact] : [];
        failure = translation.Succeeded ? string.Empty : $"the canonical module did not translate: {translation.Outcome}/{translation.Reason}";
        return translation.Succeeded;
    }

    private static void Release(in VmVerificationResult verified)
    {
        if (verified.TryGetArtifact(out var admitted))
        {
            admitted.Dispose();
        }
    }

    private static string Describe(VmSourcePosition at) =>
        string.Create(CultureInfo.InvariantCulture, $"({at.SectionIndex}, {at.ByteOffset}, {at.ProfileCoordinate0}, {at.ProfileCoordinate1})");
}
