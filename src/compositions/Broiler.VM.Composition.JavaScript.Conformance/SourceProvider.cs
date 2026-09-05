// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// This harness's answer to a guest-initiated load: compile the source it was handed.
/// </summary>
/// <remarks>
/// <para>
/// <b>A harness that registered no provider was measuring a composition nobody ships.</b> The suite
/// reaches <c>eval</c> in hundreds of cases — not to test <c>eval</c>, but because it is how a test
/// builds a program whose early error it wants to observe. Without a provider every one of those
/// answered <c>HostFailure/ProviderNotRegistered</c>, which the harness scored as a FAILURE of the
/// engine. It was a fact about the harness's own wiring.
/// </para>
/// <para>
/// <b>It is the same shape as the end-user host's, and deliberately not shared code.</b> A
/// composition root's wiring is its own; two roots that register the same capability are two
/// decisions that happen to agree, and a helper library holding the decision for both would make
/// the agreement structural. What they share is the profile, which is the thing that is supposed to
/// be shared.
/// </para>
/// <para>
/// <b>What it does not do is decide the manifest.</b> The descriptor it answers with names the
/// manifest and the format version the requesting program was verified at, so a slice-mode run
/// cannot be handed wide-mode bytes through a door the guest opened.
/// </para>
/// </remarks>
internal sealed class SourceProvider(VmFeatureManifestId manifest, uint formatVersion)
    : IVmArtifactProvider
{
    /// <summary>The identity this provider is registered under.</summary>
    public VmCapabilityId CapabilityId => JavaScriptProfile.SourceProviderCapability.CapabilityId;

    /// <summary>Its exact version.</summary>
    public int Version => JavaScriptProfile.SourceProviderCapability.Version;

    /// <summary>Answers one request by compiling its payload.</summary>
    public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request)
    {
        if (request.RequestingProfileId != JavaScriptProfile.Id)
        {
            return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
        }

        // TWO QUESTIONS THROUGH ONE DOOR, AND THE PAYLOAD SAYS WHICH. `eval` asks for the program a
        // String is; a dynamic `import()` asks for the module a specifier names from a referrer,
        // and marks its payload so. The suite reaches the second in a thousand variants, most of
        // them scripts, and every one of them with a specifier no compilation of the test file
        // could have resolved in advance.
        if (JsFormat.TryReadModuleRequest(
            request.RequestPayload.Span, out var referrer, out var specifier))
        {
            return Module(referrer, specifier);
        }

        string source;

        try
        {
            source = System.Text.Encoding.UTF8.GetString(request.RequestPayload.Span);
        }
        catch (System.ArgumentException)
        {
            return VmArtifactProviderAnswer.Refused(VmReason.MalformedEncoding);
        }

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script)]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            // THE FRONT END REFUSED THE SOURCE, WHICH IS A POLICY ANSWER AND NOT A FAILURE OF THIS
            // HOST, and for this harness it is also the answer a negative test is asking for: a
            // program `eval` cannot compile is one the manifest does not admit.
            return VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
        }

        return Answered(compiled.Artifact);
    }

    /// <summary>Answers a request for the module one specifier names from one referrer.</summary>
    /// <remarks>
    /// <b>A specifier that resolves to nothing is NOT FOUND and a graph that will not compile is
    /// REFUSED</b>, and a whole family of the suite asks for the first of those on purpose - a
    /// dynamic import of a file that is not there is a rejected promise and not a harness failure.
    /// </remarks>
    private VmArtifactProviderAnswer Module(string referrer, string specifier)
    {
        var graph = Test262Modules.LoadFor(referrer, specifier);

        if (graph.Failure.Length != 0)
        {
            return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
        }

        var compiled = JsCompiler.Compile([], graph.Modules);

        return compiled.Succeeded && compiled.Artifact is not null
            ? Answered(compiled.Artifact)
            : VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
    }

    /// <summary>Wraps compiled bytes in the descriptor the requesting program was verified at.</summary>
    private VmArtifactProviderAnswer Answered(byte[] artifact)
    {
        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            formatVersion,
            manifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("broiler-js-conformance://source-provider"));

        return VmArtifactProviderAnswer.Provided(in descriptor, artifact);
    }
}
