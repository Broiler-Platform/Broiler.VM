// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           5
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A8E51B
// Broiler-Human:        PENDING
public sealed class VmNativeCompilationResult
{
    private VmNativeCompilationResult(
        bool succeeded,
        VmArtifactDescriptor descriptor,
        byte[]? machineCodeArtifact,
        string? refusal)
    {
        Succeeded = succeeded;
        Descriptor = descriptor;
        MachineCodeArtifact = machineCodeArtifact;
        Refusal = refusal;
    }

    /// <summary>Whether native compilation succeeded.</summary>
    public bool Succeeded { get; }

    /// <summary>The descriptor of the produced machine code artifact.</summary>
    public VmArtifactDescriptor Descriptor { get; }

    /// <summary>The emitted machine code artifact bytes.</summary>
    public byte[]? MachineCodeArtifact { get; }

    /// <summary>The refusal reason if compilation failed.</summary>
    public string? Refusal { get; }

    /// <summary>Mints a successful compilation result.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1039BB
    // Broiler-Human:        PENDING
    public static VmNativeCompilationResult Success(
        in VmArtifactDescriptor descriptor, byte[] machineCodeArtifact) =>
        new(true, descriptor, machineCodeArtifact, null);

    /// <summary>Mints a failed compilation result.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=05ADBF
    // Broiler-Human:        PENDING
    public static VmNativeCompilationResult Failure(string refusal) =>
        new(false, default, null, refusal);
}
