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
/// <b>It compiles with the same request as the outer program - manifest, output form and
/// backend</b> - and declares the manifest in the descriptor it answers with. The core verifies
/// those bytes into their own immutable handle before any of them runs — under the requesting
/// operation's remaining allowance and at a nesting depth the core counts — so nothing here is
/// trusted because it came from inside the image. The form matters as much as the manifest: an
/// instance has one form, and a guest-loaded program of the other form is refused as a defect.
/// </para>
/// <para>
/// <b>A source refusal is a <c>Refused</c> answer and not an exception.</b> A provider that threw
/// would be a broken host, and the core would translate it as a host fault; a provider that
/// declines a program the front end will not admit is a working host saying so. The distinction is
/// the one the artifact-provider contract draws in its own remark, applied to a compiler.
/// </para>
/// </remarks>
internal sealed class SourceProvider(JsCompileRequest compileRequest) : IVmArtifactProvider
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

        // TWO QUESTIONS THROUGH ONE DOOR, AND THE PAYLOAD SAYS WHICH. `eval` and the `Function`
        // constructor ask for the program a String is; a dynamic `import()` asks for the module a
        // specifier names from a referrer, and marks its payload so. The two are answered by two
        // different compilations of two different goals, and folding them together would have made
        // a module graph out of whatever text a guest happened to pass to `eval`.
        if (Broiler.VM.Profile.JavaScript.Format.JsFormat.TryReadModuleRequest(
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

        JsScriptUnit[] scripts = [new JsScriptUnit("main", source, SliceParseOptions.Script)];
        var compiled = JsCompiler.Compile(scripts, [], compileRequest);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            // THE FRONT END REFUSED THE SOURCE, WHICH IS A POLICY ANSWER AND NOT A FAILURE OF THIS
            // HOST. The guest asked for something outside the manifest and is told so; the
            // diagnostic itself does not cross the boundary, because a provider answers with an
            // artifact or a reason and the reason vocabulary is the core's.
            return Unanswered(scripts, []);
        }

        return Answered(compiled.Artifact);
    }

    /// <summary>Wraps compiled bytes in a descriptor naming the manifest they were compiled under.</summary>
    /// <remarks>
    /// <b>The descriptor names the manifest the request named</b>, because the verifier compares it
    /// with the manifest the artifact's own header names and answers a disagreement as the caller's
    /// mistake - which a provider always saying <c>wide</c> about a numeric artifact would be.
    /// </remarks>
    private VmArtifactProviderAnswer Answered(byte[] artifact)
    {
        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            compileRequest.Manifest == JsFeatureManifest.Numeric
                ? JavaScriptProfile.NumericManifest
                : JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("broiler-js-cli://source-provider"));

        return VmArtifactProviderAnswer.Provided(in descriptor, artifact);
    }

    /// <summary>The answer for an input this provider's request would not compile.</summary>
    /// <remarks>
    /// <b>A native compilation can fail for a reason that is not the source's</b> - the baseline
    /// form refuses an artifact whose emitted code would exceed the format's native-code ceiling -
    /// so the same input is compiled again in bytecode and only that answer decides. Bytecode
    /// admitting it means there is no artifact of this form for it, which is <c>NotFound</c>;
    /// bytecode refusing it too means the source was refused. The conformance harness's provider
    /// answers the same way, so the two roots give a guest the same answer for the same program.
    /// </remarks>
    private VmArtifactProviderAnswer Unanswered(
        IReadOnlyList<JsScriptUnit> scripts, IReadOnlyList<JsModuleUnit> modules)
    {
        if (compileRequest.Form == JsOutputForm.Native)
        {
            var bytecode = JsCompiler.Compile(
                scripts, modules, compileRequest with { Form = JsOutputForm.Bytecode, Backend = string.Empty });

            if (bytecode.Succeeded && bytecode.Artifact is not null)
            {
                return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
            }
        }

        return VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
    }

    /// <summary>Answers a request for the module one specifier names from one referrer.</summary>
    /// <remarks>
    /// <b>A specifier this host cannot resolve is NOT FOUND and a graph it can resolve but not
    /// compile is REFUSED</b>, which are the two answers the core's vocabulary already has for a
    /// provider and which mean what a reader would expect: the first is "there is no such module
    /// here" and the second is "there is, and it is not a program this manifest admits". The guest
    /// sees both as a rejected promise carrying the reason, and the reason is what tells the two
    /// apart.
    /// </remarks>
    private VmArtifactProviderAnswer Module(string referrer, string specifier)
    {
        var graph = ModuleGraph.LoadFor(referrer, specifier);

        if (graph.Failure.Length != 0)
        {
            return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
        }

        var compiled = JsCompiler.Compile([], graph.Modules, compileRequest);

        return compiled.Succeeded && compiled.Artifact is not null
            ? Answered(compiled.Artifact)
            : Unanswered([], graph.Modules);
    }
}
