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
/// <para>
/// <b>Nor does it decide the output form: it compiles with the run's own request.</b> An instance
/// has one form, and the engine refuses a guest-loaded program of the other form as a defect, so
/// a provider that compiled every <c>eval</c> to bytecode would turn every native variant reaching
/// <c>eval</c> into an internal defect that no bytecode run could show.
/// </para>
/// </remarks>
internal sealed class SourceProvider(VmFeatureManifestId manifest, uint formatVersion, JsCompileRequest compileRequest)
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

        // A THIRD QUESTION, MARKED THE SAME WAY: a direct `eval` asks for its String compiled as
        // eval code for one call site, under the flags byte its request carries (source-provider
        // version 2, JSeal V14). Any other leading control byte is a vocabulary this host does not
        // speak, and compiling it as source would be answering a question nobody asked.
        if (!JsCompiler.TryReadProgramRequest(request.RequestPayload.Span, out var script))
        {
            return VmArtifactProviderAnswer.Refused(VmReason.MalformedEncoding);
        }

        JsScriptUnit[] scripts = [script];
        var compiled = JsCompiler.Compile(scripts, [], compileRequest);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            // THE FRONT END REFUSED THE SOURCE, WHICH IS A POLICY ANSWER AND NOT A FAILURE OF THIS
            // HOST, and for this harness it is also the answer a negative test is asking for: a
            // program `eval` cannot compile is one the manifest does not admit.
            return Unanswered(scripts, []);
        }

        return Answered(compiled.Artifact);
    }

    /// <summary>The answer for an input the run's own request would not compile.</summary>
    /// <remarks>
    /// <para>
    /// <b>A native compilation can fail for a reason that is not the source's, and the answer must
    /// not say otherwise.</b> The baseline form refuses an artifact whose emitted code would exceed
    /// the format's native-code ceiling, and that refusal is a limit of the form. Answering it as
    /// <c>Refused(SemanticValidationFailed)</c> would tell the guest its program was not admitted,
    /// which a negative test would score as the early error it expects - a pass the bytecode run
    /// could not have earned.
    /// </para>
    /// <para>
    /// <b>So the same input is compiled again in bytecode, and only that answer decides.</b> If
    /// bytecode admits it, the failure was the emitter's and the answer is <c>NotFound</c>: there is
    /// no artifact of this run's form for it. If bytecode refuses it too, the source was refused and
    /// the answer is the one a bytecode run gives. A bytecode run never reaches the second
    /// compilation, so its answers are unchanged.
    /// </para>
    /// </remarks>
    private VmArtifactProviderAnswer Unanswered(
        IReadOnlyList<JsScriptUnit> scripts, IReadOnlyList<JsModuleUnit> modules)
    {
        if (compileRequest.Form is JsOutputForm.Native or JsOutputForm.Value)
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

        var compiled = JsCompiler.Compile([], graph.Modules, compileRequest);

        return compiled.Succeeded && compiled.Artifact is not null
            ? Answered(compiled.Artifact)
            : Unanswered([], graph.Modules);
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
