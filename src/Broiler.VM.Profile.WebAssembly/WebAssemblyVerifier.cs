// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           4
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Critical
// Criteria:         4/4
// Resource impact:  4/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The WebAssembly profile's verifier: the one trust boundary, reached only through the core's one
/// verification entry point.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS ONE VERIFICATION IN TWO PHASES, AND THE ORDER BETWEEN THEM IS OBSERVABLE.</b> Decoding
/// answers whether the payload is a well-formed encoding: its preamble is right, its sections are in
/// the canonical order with no non-custom section twice, every section body consumed exactly its
/// declared length, every variable-length integer was inside its byte budget, every name was
/// well-formed UTF-8, and its function and code counts agree. Validation then answers whether the
/// module means anything: every index addresses something, every instruction byte names an
/// instruction this manifest admits, every operand stack balances, every block nests and closes, and
/// every load and store aligns within its width. Decoding completes over the WHOLE payload before
/// validation begins, so a module that is both malformed and invalid is reported malformed - which
/// is a property of the sequence in <see cref="VerifyCore"/> rather than a convention.
/// </para>
/// <para>
/// <b>A module that verifies here is then instantiated and run</b> by
/// <see cref="WebAssemblyExecutor"/> over <see cref="WasmInterpreter"/>. What validation buys the
/// interpreter is that the two bounds and the jump table it needs are computed and stored on each
/// function body rather than left to be read out of the payload.
/// <i>(Corrected 2026-09-08. This paragraph read "<b>A module that verifies here is still not a
/// module that can be run</b>, because running one needs an interpreter and this assembly has
/// none. What validation buys is that the executor's refusal is now the absence of an interpreter
/// alone". Both halves were true when written and went stale when the interpreter landed in this
/// assembly; the executor refuses nothing on that ground now. The superseded reading is quoted
/// rather than deleted, because a boundary record that revises itself silently is one a reader
/// cannot audit.)</i>
/// </para>
/// <para>
/// <b>Verification is total.</b> It answers; it does not throw. The core deliberately does not
/// catch a verifier's exception - so that a verifier's bug reaches its author as a crash rather
/// than masquerading as a malicious artifact - which means the whole body is wrapped here and any
/// escape becomes a deterministic refusal carrying a reserved diagnostic code. That path is a bug
/// to be fixed and never a rejection route, and the code it carries says so.
/// </para>
/// <para>
/// <b>Two outcome categories appear below and the split is the core's ruling rather than a
/// preference.</b> A malformed encoding is an invalid artifact carrying this profile's diagnostic
/// code and a byte position. A breach of an effective ceiling is a resource exhaustion naming one
/// dimension and one scope, with no diagnostic code at all. Conflating them tells a caller its
/// module is malformed when the truth is that this host declined to spend the memory.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=9EEB68
// Broiler-Falsified-If: any input makes a member of this type throw, or a state it hands back is treated as though a validation pass had run over it
// Broiler-Human:        PENDING
public sealed class WebAssemblyVerifier : IVmProfileVerifier
{
    /// <summary>
    /// How much work the bounded reader accumulates between polls.
    /// </summary>
    /// <remarks>
    /// It is the number the descriptor declares as this profile's uncharged-work bound, and it is
    /// the largest safe value: the meter reports a poll-bound violation on work strictly greater
    /// than the bound, so polling exactly at it is inside the promise. A granularity of one would
    /// be far tighter than anything this profile claims and would take the meter's lock and read a
    /// clock on every byte of every integer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=A489E3
    // Broiler-Falsified-If: it exceeds the uncharged-work bound this profile's descriptor declares
    // Broiler-Human:        PENDING
    public const ulong PollGranularity = 65_536;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DA99B9
    // Broiler-Human:        PENDING
    private readonly VmFeatureManifestId acceptedManifest;

    /// <summary>Creates the verifier for one profile identity and its one accepted manifest.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AFB733
    // Broiler-Human:        PENDING
    public WebAssemblyVerifier(VmProfileId profileId, VmFeatureManifestId manifest)
    {
        ProfileId = profileId;
        acceptedManifest = manifest;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CCA6CF
    // Broiler-Human:        PENDING
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=91327C
    // Broiler-Human:        PENDING
    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8BCF43
    // Broiler-Human:        PENDING
    public int AuthoredCoreContractVersion => 1;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=47E01B
    // Broiler-Human:        PENDING
    public int VerifierSemanticVersion => 1;

    /// <inheritdoc/>
    /// <remarks>
    /// The profile-identity check is the FIRST statement and the reader is not constructed before
    /// it, so an artifact naming a profile this verifier does not host is answered without a
    /// payload byte being examined.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=2B99FE
    // Broiler-Falsified-If: a payload byte is read on a path that answers UnsupportedProfile, or an exception escapes this method
    // Broiler-Human:        PENDING
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken)
    {
        if (descriptor.ProfileId != ProfileId)
        {
            return VmVerifierOutcome.UnsupportedProfile();
        }

        try
        {
            return VerifyCore(in descriptor, payload, context, cancellationToken);
        }
        catch (System.OperationCanceledException)
        {
            // A cancellation is the one escape that is not a defect: the token was signalled while
            // something below was reading it, and the honest answer is the one the caller asked for.
            return VmVerifierOutcome.Cancellation();
        }
        catch (System.Exception)
        {
            // EVERY OTHER ESCAPE IS A DEFECT IN THIS ASSEMBLY AND IS REPORTED AS ONE. It becomes a
            // refusal rather than a crash because the core does not catch a verifier's exception,
            // and a caller cannot tell an exception from a hostile artifact. The reserved code is
            // what tells a reader which of the two this was.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure,
                (int)WebAssemblyDiagnosticCode.VerifierDefect,
                new VmSourcePosition(
                    sectionIndex: -1, byteOffset: 0, profileCoordinate0: -1, profileCoordinate1: -1));
        }
    }

    /// <summary>
    /// The verification sequence, in the order the ordering rules require it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Identity, then the format version, then the feature manifest, then - and this is the clause
    /// that matters - <b>the ceilings are materialized into read bounds BEFORE the first byte is
    /// read</b>. The adapter's own constructor projects them, and every reader either pass builds
    /// is built from that one value, so there is no reader anywhere in this profile that reads a
    /// byte under a bound computed later or computed differently.
    /// </para>
    /// <para>
    /// Then decoding over the whole payload, then validation over the module decoding produced. The
    /// two are separate statements rather than one fused walk, which is what makes a module that is
    /// both malformed and invalid answer malformed.
    /// </para>
    /// <para>
    /// The core has already checked the format version and the manifest before this verifier is
    /// called. They are checked again here because this verifier is a total function of its own
    /// arguments and must answer correctly for a caller that reaches it another way; a check that
    /// is only correct because somebody else ran it first is not a check this boundary can keep.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=20FD2C
    // Broiler-Falsified-If: a payload byte is read before the effective ceilings have been projected into the reader's bounds
    // Broiler-Human:        PENDING
    private VmVerifierOutcome VerifyCore(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken)
    {
        if (descriptor.FormatVersion is < WasmFormat.MinimumFormatVersion
            or > WasmFormat.MaximumFormatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnsupportedProfileFormatVersion,
                (int)WebAssemblyDiagnosticCode.UnsupportedArtifactFormatVersion,
                Outside());
        }

        if (descriptor.FeatureManifestId != acceptedManifest)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnsupportedFeatureManifest,
                (int)WebAssemblyDiagnosticCode.UnacceptedFeatureManifest,
                Outside());
        }

        var adapter = new WasmReadAdapter(context.Meter, context.Ceilings.VerificationCeilings);
        var decoder = new WasmDecoder(payload, in adapter.Ceilings, adapter, PollGranularity);

        if (!decoder.TryDecode(out var module, out var refusal))
        {
            return refusal;
        }

        // DECODING COMPLETED BEFORE VALIDATION BEGAN, AND THE SEQUENCE HERE IS WHERE THAT IS TRUE.
        // The decoder answered over the whole payload before this line was reached, so a module that
        // is both malformed and invalid was already refused above with a decode diagnostic and never
        // arrives here. Fusing the two - validating each function body as its code section was read
        // - would be faster and would answer the wrong one of two true things.
        var validator = new WasmValidator(module!, adapter, PollGranularity);

        if (!validator.TryValidate(out var invalid))
        {
            return invalid;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return VmVerifierOutcome.Cancellation();
        }

        // NO HOST-IMPORT CHECK RUNS HERE, AND THAT IS A PROPERTY OF THE SURFACE RATHER THAN AN
        // OMISSION. The accepted manifest admits no import at all and the decoder refuses a module
        // that declares one, so there is no import whose capability descriptor could be looked for
        // in the registered set. The check arrives with imports.
        return VmVerifierOutcome.Verified(module!, VmArtifactSharing.Shareable);
    }

    /// <summary>A position outside every section, which is what a header-level refusal carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=333224
    // Broiler-Human:        PENDING
    private static VmSourcePosition Outside() =>
        new(sectionIndex: -1, byteOffset: 0, profileCoordinate0: -1, profileCoordinate1: -1);
}
