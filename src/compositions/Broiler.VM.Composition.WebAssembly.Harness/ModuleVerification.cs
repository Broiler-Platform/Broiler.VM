using Broiler.VM.Abstractions;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Profile.WebAssembly;
using Broiler.VM.Ubc;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>Which stage answered a module: the translator, or the core over the translator's artifact.</summary>
internal enum ModuleStage
{
    /// <summary>The translator refused the module, so the core was never asked.</summary>
    Translation,

    /// <summary>The module translated, and the answer is the core's over the artifact.</summary>
    Core,
}

/// <summary>
/// What verifying one WebAssembly module answered, in the fields every lane of this root prints.
/// </summary>
/// <remarks>
/// <para>
/// <b>One answer, whichever stage gave it.</b> A module the translator refuses is refused in the
/// fields the core refused it in before the universal bytecode - outcome, reason, diagnostic code,
/// budget dimension, scope and position - so a lane prints a translation's refusal exactly where and
/// how it printed the core's. A module that translates is answered by the core's verification of the
/// artifact, and only then can there be an artifact to instantiate.
/// </para>
/// <para>
/// <b>The stage is recorded and not printed.</b> The corpus replay reads it, because a core refusal of
/// a module the translator admitted is a defect of the translator or of the family's verifier hook and
/// never an answer a corpus row may record; the other lanes print what they always printed.
/// </para>
/// </remarks>
internal sealed class VerifiedModule
{
    private readonly VmVerifiedArtifact? artifact;

    private VerifiedModule(
        ModuleStage stage,
        VmOutcome outcome,
        VmReason reason,
        int code,
        VmBudgetDimension dimension,
        VmBudgetScope scope,
        VmSourcePosition position,
        WasmModule? module,
        VmVerifiedArtifact? artifact)
    {
        Stage = stage;
        Outcome = outcome;
        Reason = reason;
        Code = code;
        Dimension = dimension;
        Scope = scope;
        Position = position;
        Module = module;
        this.artifact = artifact;
    }

    /// <summary>Which stage answered.</summary>
    internal ModuleStage Stage { get; }

    /// <summary>The outcome, as the core would have reported it for the bare module.</summary>
    internal VmOutcome Outcome { get; }

    /// <summary>The reason beside <see cref="Outcome"/>.</summary>
    internal VmReason Reason { get; }

    /// <summary>The profile diagnostic code, where the answer carries one.</summary>
    internal int Code { get; }

    /// <summary>The dimension an exhaustion named.</summary>
    internal VmBudgetDimension Dimension { get; }

    /// <summary>The scope an exhaustion was refused at.</summary>
    internal VmBudgetScope Scope { get; }

    /// <summary>Where in the module a refusal was found.</summary>
    internal VmSourcePosition Position { get; }

    /// <summary>The decoded and validated module, whenever the translation succeeded.</summary>
    internal WasmModule? Module { get; }

    /// <summary>The verified artifact, when the core admitted the translation's output.</summary>
    internal bool TryGetArtifact(out VmVerifiedArtifact verified)
    {
        verified = artifact!;
        return artifact is not null;
    }

    /// <summary>The translator's refusal, in the fields the core's refusal was reported in.</summary>
    internal static VerifiedModule Refused(WasmTranslation translation) =>
        new(
            ModuleStage.Translation,
            translation.Outcome,
            translation.Reason,
            (int)translation.Code,
            translation.Dimension,
            translation.Scope,
            translation.Position,
            translation.Module,
            null);

    /// <summary>The core's answer over the translation's artifact.</summary>
    internal static VerifiedModule Answered(WasmTranslation translation, in VmVerificationResult verified) =>
        new(
            ModuleStage.Core,
            verified.Outcome,
            verified.Reason,
            verified.Diagnostics.ProfileDiagnosticCode,
            verified.Diagnostics.ExhaustedDimension,
            verified.Diagnostics.ExhaustedScope,
            verified.Diagnostics.SourcePosition,
            translation.Module,
            verified.TryGetArtifact(out var admitted) ? admitted : null);
}

/// <summary>
/// The one door every lane of this root verifies a WebAssembly module through: translate it, then
/// verify the artifact under the family's descriptor.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE CORE VERIFIES UNIVERSAL BYTECODE AND NEVER A MODULE.</b> The profile's descriptor is built
/// here, as every root composing the family builds it, over the one form this root composes - the
/// bytecode emitter - because a family names no emitter and the form is the root's choice. A module
/// is handed to the translator first, under the ceilings the core would verify it under, and what
/// reaches the core is the translator's artifact, named by the universal bytecode's format version.
/// </para>
/// <para>
/// <b>The ceilings are the core's own, read off the runtime rather than assumed.</b> The core verifies
/// under the runtime's ceilings intersected with the profile's hard maxima and with what the artifact
/// descriptor requests; this reads the runtime's ceilings from its budget snapshot and intersects them
/// the same way, so the translation and the verification of its artifact run under one set.
/// </para>
/// </remarks>
internal static class ModuleVerification
{
    /// <summary>The family's descriptor over the bytecode emitter: the one profile row this root composes.</summary>
    internal static VmProfileDescriptor Descriptor { get; } = UbcDescriptors.Build(
        WebAssemblyProfile.Registration,
        WebAssemblyProfile.Declaration,
        UbcEmitterSet.Create(UbcBytecodeEmitter.Form));

    /// <summary>
    /// Where every module verified is written down when a lane that re-reads them asked for it, with the
    /// lane that verified it - the last segment of its caller identity - and the label of the check
    /// that built it; null records nothing.
    /// </summary>
    internal static List<(string Lane, string Label, byte[] Module)>? Recorded { get; set; }

    /// <summary>The artifact descriptor a translation's artifact is verified under.</summary>
    internal static VmArtifactDescriptor ArtifactDescriptor(VmFeatureManifestId manifest, string caller) =>
        new(WebAssemblyProfile.Id, UbcFormat.FormatVersion, manifest, default,
            VmCallerIdentity.FromCanonicalIdentity(caller));

    /// <summary>
    /// The ceilings the core verifies an artifact named by <paramref name="descriptor"/> under in
    /// <paramref name="runtime"/>: the runtime's ceilings, the profile's hard maxima and the request.
    /// </summary>
    internal static VmLimitVector Ceilings(VmRuntime runtime, in VmArtifactDescriptor descriptor)
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

        var bound = VmLimitVector.Intersect(host, Descriptor.ProfileHardMaxima);

        return VmLimitVector.Intersect(
            bound,
            descriptor.RequestedLimits.IsEmpty ? VmLimitVector.Unconstrained : descriptor.RequestedLimits);
    }

    /// <summary>Translates <paramref name="module"/> and verifies its artifact under the slice manifest.</summary>
    internal static VerifiedModule Verify(VmRuntime runtime, byte[] module, string caller, string label)
    {
        Recorded?.Add((caller[(caller.LastIndexOf('/') + 1)..], label, module));

        var descriptor = ArtifactDescriptor(WebAssemblyProfile.SliceManifest, caller);
        var translation = WasmTranslator.Translate(module, Ceilings(runtime, in descriptor), CancellationToken.None);

        if (!translation.Succeeded)
        {
            return VerifiedModule.Refused(translation);
        }

        var verified = runtime.Verify(in descriptor, translation.Artifact.AsSpan(), CancellationToken.None);
        return VerifiedModule.Answered(translation, in verified);
    }
}
