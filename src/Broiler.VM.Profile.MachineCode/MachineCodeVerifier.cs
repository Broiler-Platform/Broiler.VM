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
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Broiler.VM;

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A812F6
// Broiler-Human:        PENDING
public sealed class MachineCodeVerifier : IVmProfileVerifier
{
    public VmProfileId ProfileId { get; }
    public int BuiltAgainstCoreContractVersion => 1;
    public int AuthoredCoreContractVersion => 1;
    public int VerifierSemanticVersion => 1;

    public MachineCodeVerifier(VmProfileId profileId)
    {
        ProfileId = profileId;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=834CF8
    // Broiler-Human:        PENDING
    private static VmSourcePosition At(ulong offset) => new(-1, offset, 0, 0);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A704A5
    // Broiler-Human:        PENDING
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        CancellationToken cancellationToken)
    {
        if (payload.Length < 16)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.Truncated, 1001, At((ulong)payload.Length));
        }

        if (!payload[..4].SequenceEqual(MachineCodeFormat.Magic))
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, 1002, At(0));
        }

        var version = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(4, 4));
        if (version != MachineCodeFormat.FormatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnsupportedProfileFormatVersion, 1003, At(4));
        }

        var archVal = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(8, 4));
        if (archVal is < 1 or > 3)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, 1004, At(8));
        }

        var architecture = (VmNativeArchitecture)archVal;

        // Check manifest agreement
        var manifestStr = descriptor.FeatureManifestId.ToString();
        if (manifestStr.EndsWith("x86_64", StringComparison.Ordinal) &&
            architecture != VmNativeArchitecture.X64Windows &&
            architecture != VmNativeArchitecture.X64SystemV)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnsupportedFeatureManifest, 1005, At(8));
        }

        if (manifestStr.EndsWith("arm64", StringComparison.Ordinal) &&
            architecture != VmNativeArchitecture.Arm64)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnsupportedFeatureManifest, 1006, At(8));
        }

        var sectionCount = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(12, 4));
        var offset = 16;

        var limits = new MachineCodeLimits(1024, 256);
        var constants = Array.Empty<double>();
        var code = Array.Empty<byte>();
        var symbols = Array.Empty<MachineCodeSymbol>();
        var entries = Array.Empty<MachineCodeEntry>();

        var readAdapter = new MachineCodeReadAdapter(context, cancellationToken);

        for (var s = 0; s < sectionCount; s++)
        {
            if (cancellationToken.IsCancellationRequested || !readAdapter.Poll())
            {
                return VmVerifierOutcome.Cancellation();
            }

            if (offset + 8 > payload.Length)
            {
                return VmVerifierOutcome.InvalidArtifact(
                    VmReason.Truncated, 1007, At((ulong)offset));
            }

            var kind = (MachineCodeFormat.SectionKind)BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(offset, 4));
            var length = BinaryPrimitives.ReadUInt32LittleEndian(payload.Slice(offset + 4, 4));
            offset += 8;

            if (offset + (int)length > payload.Length)
            {
                return VmVerifierOutcome.InvalidArtifact(
                    VmReason.Truncated, 1008, At((ulong)offset));
            }

            var sectionData = payload.Slice(offset, (int)length);
            offset += (int)length;

            if (!readAdapter.TryChargeWork(length))
            {
                return VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
            }

            switch (kind)
            {
                case MachineCodeFormat.SectionKind.Limits:
                    if (sectionData.Length < 8)
                    {
                        return VmVerifierOutcome.InvalidArtifact(
                            VmReason.InconsistentStructure, 1009, At((ulong)offset));
                    }
                    var maxOp = BinaryPrimitives.ReadUInt32LittleEndian(sectionData[..4]);
                    var maxScope = BinaryPrimitives.ReadUInt32LittleEndian(sectionData.Slice(4, 4));
                    limits = new MachineCodeLimits(maxOp, maxScope);
                    break;

                case MachineCodeFormat.SectionKind.Constants:
                    if (sectionData.Length < 4)
                    {
                        return VmVerifierOutcome.InvalidArtifact(
                            VmReason.InconsistentStructure, 1010, At((ulong)offset));
                    }
                    var cCount = BinaryPrimitives.ReadUInt32LittleEndian(sectionData[..4]);
                    if (sectionData.Length < 4 + (int)cCount * 8)
                    {
                        return VmVerifierOutcome.InvalidArtifact(
                            VmReason.InconsistentStructure, 1011, At((ulong)offset));
                    }
                    if (!readAdapter.TryReserve((ulong)cCount * 8))
                    {
                        return VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
                    }
                    constants = new double[cCount];
                    for (var i = 0; i < cCount; i++)
                    {
                        constants[i] = BinaryPrimitives.ReadDoubleLittleEndian(sectionData.Slice(4 + i * 8, 8));
                    }
                    break;

                case MachineCodeFormat.SectionKind.NativeCode:
                    if (!readAdapter.TryReserve((ulong)sectionData.Length))
                    {
                        return VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
                    }
                    code = sectionData.ToArray();
                    break;

                case MachineCodeFormat.SectionKind.NativeSymbols:
                    if (sectionData.Length < 4)
                    {
                        return VmVerifierOutcome.InvalidArtifact(
                            VmReason.InconsistentStructure, 1012, At((ulong)offset));
                    }
                    var symCount = BinaryPrimitives.ReadUInt32LittleEndian(sectionData[..4]);
                    if (sectionData.Length < 4 + (int)symCount * 12)
                    {
                        return VmVerifierOutcome.InvalidArtifact(
                            VmReason.InconsistentStructure, 1013, At((ulong)offset));
                    }
                    symbols = new MachineCodeSymbol[symCount];
                    for (var i = 0; i < symCount; i++)
                    {
                        var fnIdx = BinaryPrimitives.ReadUInt32LittleEndian(sectionData.Slice(4 + i * 12, 4));
                        var symOff = BinaryPrimitives.ReadUInt32LittleEndian(sectionData.Slice(8 + i * 12, 4));
                        var symLen = BinaryPrimitives.ReadUInt32LittleEndian(sectionData.Slice(12 + i * 12, 4));
                        symbols[i] = new MachineCodeSymbol(fnIdx, symOff, symLen);
                    }
                    break;

                case MachineCodeFormat.SectionKind.Entries:
                    if (sectionData.Length < 4)
                    {
                        return VmVerifierOutcome.InvalidArtifact(
                            VmReason.InconsistentStructure, 1014, At((ulong)offset));
                    }
                    var entryCount = BinaryPrimitives.ReadUInt32LittleEndian(sectionData[..4]);
                    var entryList = new List<MachineCodeEntry>((int)entryCount);
                    var entryOff = 4;
                    for (var i = 0; i < entryCount; i++)
                    {
                        if (entryOff + 8 > sectionData.Length)
                        {
                            return VmVerifierOutcome.InvalidArtifact(
                                VmReason.InconsistentStructure, 1015, At((ulong)offset));
                        }
                        var fnIdx = BinaryPrimitives.ReadUInt32LittleEndian(sectionData.Slice(entryOff, 4));
                        var nameLen = (int)BinaryPrimitives.ReadUInt32LittleEndian(sectionData.Slice(entryOff + 4, 4));
                        entryOff += 8;

                        if (entryOff + nameLen > sectionData.Length)
                        {
                            return VmVerifierOutcome.InvalidArtifact(
                                VmReason.InconsistentStructure, 1016, At((ulong)offset));
                        }
                        var name = Encoding.UTF8.GetString(sectionData.Slice(entryOff, nameLen));
                        entryOff += nameLen;
                        entryList.Add(new MachineCodeEntry(name, fnIdx));
                    }
                    entries = entryList.ToArray();
                    break;
            }
        }

        // Validate symbol offsets lie inside code
        for (var i = 0; i < symbols.Length; i++)
        {
            if (symbols[i].Offset + symbols[i].Length > (uint)code.Length)
            {
                return VmVerifierOutcome.InvalidArtifact(
                    VmReason.InconsistentStructure, 1017, At(0));
            }
        }

        var program = new MachineCodeProgram(architecture, code, symbols, entries, constants, limits);
        return VmVerifierOutcome.Verified(program, VmArtifactSharing.Shareable);
    }
}
