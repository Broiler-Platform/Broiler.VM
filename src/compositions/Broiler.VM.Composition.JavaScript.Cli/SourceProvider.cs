// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// This composition's answer to a guest-initiated load: compile the source it was handed.
/// </summary>
/// <remarks>
/// <para>
/// <b>The compiler is HERE and not in the profile, and that is the whole design.</b> A profile that
/// could turn a String into bytes on its own would be a profile with a compiler inside its Native
/// AOT closure whether or not the composition wanted one, and no registration could take it away.
/// Registering this provider is the permission; a sibling root that registers nothing composes the
/// same profile, runs the same programs, and answers every <c>eval</c> with a refusal the guest may
/// catch. That refusal is the content policy, expressed as a contract outcome.
/// </para>
/// <para>
/// <b>It compiles at the same manifest and the same format version as the outer program</b>, and
/// declares it in the descriptor it answers with. The core verifies those bytes into their own
/// immutable handle before any of them runs — under the requesting operation's remaining allowance
/// and at a nesting depth the core counts — so nothing here is trusted because it came from inside
/// the image.
/// </para>
/// <para>
/// <b>A source refusal is a <c>Refused</c> answer and not an exception.</b> A provider that threw
/// would be a broken host, and the core would translate it as a host fault; a provider that
/// declines a program the front end will not admit is a working host saying so. The distinction is
/// the one the artifact-provider contract draws in its own remark, applied to a compiler.
/// </para>
/// </remarks>
internal sealed class SourceProvider : IVmArtifactProvider
{
    /// <summary>The identity this provider is registered under.</summary>
    public VmCapabilityId CapabilityId => JavaScriptProfile.SourceProviderCapability.CapabilityId;

    /// <summary>Its exact version.</summary>
    public int Version => JavaScriptProfile.SourceProviderCapability.Version;

    /// <summary>How many requests this provider has answered, for a host that wants to say.</summary>
    internal int RequestCount { get; private set; }

    /// <summary>Answers one request by compiling its payload.</summary>
    public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request)
    {
        RequestCount++;

        if (request.RequestingProfileId != JavaScriptProfile.Id)
        {
            // A provider may only answer with an artifact of the requesting profile, so a request
            // from another one is not this provider's to answer at all.
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
            // HOST. The guest asked for something outside the manifest and is told so; the
            // diagnostic itself does not cross the boundary, because a provider answers with an
            // artifact or a reason and the reason vocabulary is the core's.
            return VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("broiler-js-cli://source-provider"));

        return VmArtifactProviderAnswer.Provided(in descriptor, compiled.Artifact);
    }
}
