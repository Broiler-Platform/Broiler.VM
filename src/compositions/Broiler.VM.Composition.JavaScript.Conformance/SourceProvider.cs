// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

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

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            formatVersion,
            manifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("broiler-js-conformance://source-provider"));

        return VmArtifactProviderAnswer.Provided(in descriptor, compiled.Artifact);
    }
}
