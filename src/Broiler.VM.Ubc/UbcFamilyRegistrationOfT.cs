// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Ubc;

/// <summary>A family's registration, carrying the family's <see cref="IUbcFamily"/> struct as its type argument.</summary>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=0; Fingerprint=1F1E41
// Broiler-Human:        PENDING
public sealed class UbcFamilyRegistration<TFamily> : UbcFamilyRegistration
    where TFamily : struct, IUbcFamily
{
    /// <summary>
    /// Registers a family. Leave <paramref name="builtAgainstUbcContractVersion"/> to its default: it is a
    /// fact about the family's compilation, not a claim its author makes.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D08C68
    // Broiler-Human:        PENDING
    public UbcFamilyRegistration(
        string identity,
        System.Collections.Generic.IEnumerable<UbcInstructionTable> tables,
        IUbcFamilyVerifier verifier,
        int authoredUbcContractVersion,
        int builtAgainstUbcContractVersion = UbcContract.Version)
        : base(identity, tables, verifier, authoredUbcContractVersion, builtAgainstUbcContractVersion)
    {
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FBFDA7
    // Broiler-Human:        PENDING
    internal override IVmProfileExecutor CreateExecutor(
        IUbcExecutorFactory factory,
        UbcFamilyDeclaration declaration,
        IVmExecutionEnvironment environment) =>
        factory.Create(this, declaration, environment);
}
