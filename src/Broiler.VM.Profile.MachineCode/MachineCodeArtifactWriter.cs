// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   2
// Annotated:        2/2
// Exempt:           0
// Human-reviewed:   0/2
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       2
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=6AFCB8
// Broiler-Human:        PENDING
public static class MachineCodeArtifactWriter
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F178C2
    // Broiler-Human:        PENDING
    public static byte[] Write(
        VmNativeArchitecture architecture,
        MachineCodeLimits limits,
        double[] constants,
        byte[] code,
        MachineCodeSymbol[] symbols,
        MachineCodeEntry[] entries)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

        // Header
        writer.Write(MachineCodeFormat.Magic);
        writer.Write(MachineCodeFormat.FormatVersion);
        writer.Write((uint)architecture);
        writer.Write(5u); // 5 sections

        // Section 1: Limits
        writer.Write((uint)MachineCodeFormat.SectionKind.Limits);
        writer.Write(8u); // 2 * sizeof(uint)
        writer.Write(limits.MaximumOperandStack);
        writer.Write(limits.MaximumScopeSlots);

        // Section 2: Constants
        writer.Write((uint)MachineCodeFormat.SectionKind.Constants);
        var constantsLen = 4u + (uint)(constants.Length * 8);
        writer.Write(constantsLen);
        writer.Write((uint)constants.Length);
        for (var i = 0; i < constants.Length; i++)
        {
            writer.Write(constants[i]);
        }

        // Section 3: NativeCode
        writer.Write((uint)MachineCodeFormat.SectionKind.NativeCode);
        writer.Write((uint)code.Length);
        writer.Write(code);

        // Section 4: NativeSymbols
        writer.Write((uint)MachineCodeFormat.SectionKind.NativeSymbols);
        var symbolsLen = 4u + (uint)(symbols.Length * 12);
        writer.Write(symbolsLen);
        writer.Write((uint)symbols.Length);
        for (var i = 0; i < symbols.Length; i++)
        {
            writer.Write(symbols[i].FunctionIndex);
            writer.Write(symbols[i].Offset);
            writer.Write(symbols[i].Length);
        }

        // Section 5: Entries
        using var entriesStream = new MemoryStream();
        using var entriesWriter = new BinaryWriter(entriesStream, Encoding.UTF8, leaveOpen: true);
        entriesWriter.Write((uint)entries.Length);
        for (var i = 0; i < entries.Length; i++)
        {
            entriesWriter.Write(entries[i].FunctionIndex);
            var utf8Bytes = Encoding.UTF8.GetBytes(entries[i].Name);
            entriesWriter.Write((uint)utf8Bytes.Length);
            entriesWriter.Write(utf8Bytes);
        }
        entriesWriter.Flush();
        var entriesBytes = entriesStream.ToArray();

        writer.Write((uint)MachineCodeFormat.SectionKind.Entries);
        writer.Write((uint)entriesBytes.Length);
        writer.Write(entriesBytes);

        writer.Flush();
        return stream.ToArray();
    }
}
