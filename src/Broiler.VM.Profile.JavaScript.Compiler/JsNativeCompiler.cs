// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           2
// Human-reviewed:   0/7
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// Compiles JavaScript bytecode artifacts into Broiler MachineCode profile artifacts.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4899B6
// Broiler-Human:        PENDING
public sealed class JsNativeCompiler : IVmNativeCompiler
{
    /// <summary>The profile ID this compiler lowers from: broiler.javascript.</summary>
    public VmProfileId InputProfileId { get; } = VmProfileId.Parse("broiler.javascript");

    /// <summary>The profile ID this compiler emits: broiler.machinecode.</summary>
    public VmProfileId OutputProfileId { get; } = VmProfileId.Parse("broiler.machinecode");

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F3BEAA
    // Broiler-Human:        PENDING
    public bool CanCompile(VmFeatureManifestId manifestId)
    {
        var val = manifestId.ToString();
        return val == "broiler.javascript" ||
               val == "broiler.javascript.slice" ||
               val == "broiler.javascript.numeric";
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=576F97
    // Broiler-Human:        PENDING
    public bool TryCompile(
        in VmArtifactDescriptor inputDescriptor,
        ReadOnlyMemory<byte> inputBytecode,
        string targetArchitecture,
        out VmArtifactDescriptor outputDescriptor,
        out byte[] outputMachineCode,
        out string refusal)
    {
        outputDescriptor = default;
        outputMachineCode = Array.Empty<byte>();

        var backendName = ResolveBackendName(targetArchitecture);
        if (!JsNativeBackends.TryFind(backendName, out var backend))
        {
            refusal = $"Unsupported native backend for architecture '{targetArchitecture}'.";
            return false;
        }

        if (!TryExtractSections(
            inputBytecode.Span,
            out var maxOperandStack,
            out var maxScopeSlots,
            out var constants,
            out var code,
            out var functions,
            out var regions,
            out var entries,
            out var existingEmission,
            out refusal))
        {
            return false;
        }

        JsNativeEmission emission;
        if (existingEmission is not null && existingEmission.Architecture == backend.Architecture)
        {
            emission = existingEmission;
        }
        else
        {
            var manifestId = inputDescriptor.FeatureManifestId.ToString();
            var assembled = new JsAssembledProgram(
                manifestId,
                code,
                functions,
                regions,
                constants,
                maxOperandStack,
                maxScopeSlots);

            if (!backend.TryEmit(assembled, out emission, out refusal))
            {
                return false;
            }
        }

        // Emit the BMC\0 binary artifact
        outputMachineCode = EmitBmcArtifact(backend.Architecture, maxOperandStack, maxScopeSlots, constants, emission, entries);

        var outputManifest = backend.Architecture == JsNativeArchitecture.Arm64
            ? VmFeatureManifestId.Parse("broiler.machinecode.arm64")
            : VmFeatureManifestId.Parse("broiler.machinecode.x86_64");

        outputDescriptor = new VmArtifactDescriptor(
            OutputProfileId,
            1,
            outputManifest,
            inputDescriptor.RequestedLimits,
            inputDescriptor.CallerIdentity);

        refusal = string.Empty;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AA2F72
    // Broiler-Human:        PENDING
    private static string ResolveBackendName(string targetArchitecture)
    {
        if (string.Equals(targetArchitecture, "arm64", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(targetArchitecture, JsNativeBackends.Arm64, StringComparison.OrdinalIgnoreCase))
        {
            return JsNativeBackends.Arm64;
        }

        if (string.Equals(targetArchitecture, "x86-64-sysv", StringComparison.OrdinalIgnoreCase))
        {
            return JsNativeBackends.X64SystemV;
        }

        if (string.Equals(targetArchitecture, "x86-64-win64", StringComparison.OrdinalIgnoreCase))
        {
            return JsNativeBackends.X64Windows;
        }

        // Default based on host OS
        return OperatingSystem.IsWindows() ? JsNativeBackends.X64Windows : JsNativeBackends.X64SystemV;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=06F867
    // Broiler-Human:        PENDING
    private static bool TryExtractSections(
        ReadOnlySpan<byte> artifact,
        out uint maxOperandStack,
        out uint maxScopeSlots,
        out List<byte[]> constants,
        out byte[] code,
        out List<JsFunctionRow> functions,
        out List<JsExceptionRegionRow> regions,
        out List<(string Name, uint FunctionIndex)> entries,
        out JsNativeEmission? existingEmission,
        out string refusal)
    {
        maxOperandStack = 1024;
        maxScopeSlots = 256;
        constants = [];
        code = [];
        functions = [];
        regions = [];
        entries = [];
        existingEmission = null;

        if (artifact.Length < 4)
        {
            refusal = "Artifact payload too short.";
            return false;
        }

        if (!artifact[..4].SequenceEqual(JavaScriptFormat.Magic))
        {
            refusal = "Invalid JavaScript artifact magic bytes.";
            return false;
        }

        var offset = 4;
        if (!TryReadVarUInt(artifact, ref offset, out var formatVersion))
        {
            refusal = "Could not read format version.";
            return false;
        }

        if (!TryReadVarUInt(artifact, ref offset, out var manifestLen))
        {
            refusal = "Could not read manifest ID length.";
            return false;
        }

        offset += (int)manifestLen;

        if (!TryReadVarUInt(artifact, ref offset, out var sectionCount))
        {
            refusal = "Could not read section count.";
            return false;
        }

        byte[]? nativeCode = null;
        JsNativeSymbolRow[]? nativeSymbols = null;
        JsNativeArchitecture nativeArch = JsNativeArchitecture.None;

        for (var s = 0ul; s < sectionCount; s++)
        {
            if (!TryReadVarUInt(artifact, ref offset, out var kindVal))
            {
                refusal = "Could not read section kind.";
                return false;
            }

            if (!TryReadVarUInt(artifact, ref offset, out var sectionLen))
            {
                refusal = "Could not read section length.";
                return false;
            }

            if (offset + (int)sectionLen > artifact.Length)
            {
                refusal = "Section length exceeds payload.";
                return false;
            }

            var sectionBody = artifact.Slice(offset, (int)sectionLen);
            offset += (int)sectionLen;

            var kind = (JsFormat.SectionKind)kindVal;
            switch (kind)
            {
                case JsFormat.SectionKind.Limits:
                    var limOff = 0;
                    if (TryReadVarUInt(sectionBody, ref limOff, out var opStack) &&
                        TryReadVarUInt(sectionBody, ref limOff, out var scSlots))
                    {
                        maxOperandStack = (uint)opStack;
                        maxScopeSlots = (uint)scSlots;
                    }
                    break;

                case JsFormat.SectionKind.Constants:
                    var constOff = 0;
                    if (TryReadVarUInt(sectionBody, ref constOff, out var constCount))
                    {
                        for (var c = 0ul; c < constCount; c++)
                        {
                            if (constOff >= sectionBody.Length) break;
                            var tag = (JsFormat.ConstantTag)sectionBody[constOff++];
                            var start = constOff - 1;
                            switch (tag)
                            {
                                case JsFormat.ConstantTag.Undefined:
                                case JsFormat.ConstantTag.Null:
                                    constants.Add(sectionBody.Slice(start, 1).ToArray());
                                    break;
                                case JsFormat.ConstantTag.Boolean:
                                    constOff += 1;
                                    constants.Add(sectionBody.Slice(start, 2).ToArray());
                                    break;
                                case JsFormat.ConstantTag.Number:
                                    constOff += 8;
                                    constants.Add(sectionBody.Slice(start, 9).ToArray());
                                    break;
                                case JsFormat.ConstantTag.InternedName:
                                case JsFormat.ConstantTag.String:
                                    if (TryReadVarUInt(sectionBody, ref constOff, out var strLen))
                                    {
                                        constOff += (int)strLen;
                                        constants.Add(sectionBody.Slice(start, constOff - start).ToArray());
                                    }
                                    break;
                            }
                        }
                    }
                    break;

                case JsFormat.SectionKind.Code:
                    code = sectionBody.ToArray();
                    break;

                case JsFormat.SectionKind.Entries:
                    var entryOff = 0;
                    if (TryReadVarUInt(sectionBody, ref entryOff, out var entCount))
                    {
                        for (var e = 0ul; e < entCount; e++)
                        {
                            if (TryReadVarUInt(sectionBody, ref entryOff, out var fnIdx) &&
                                TryReadVarUInt(sectionBody, ref entryOff, out var nameLen))
                            {
                                var name = Encoding.UTF8.GetString(sectionBody.Slice(entryOff, (int)nameLen));
                                entryOff += (int)nameLen;
                                entries.Add((name, (uint)fnIdx));
                            }
                        }
                    }
                    break;

                case JsFormat.SectionKind.Functions:
                    var fnOff = 0;
                    if (TryReadVarUInt(sectionBody, ref fnOff, out var fnCount))
                    {
                        for (var f = 0ul; f < fnCount; f++)
                        {
                            if (TryReadVarUInt(sectionBody, ref fnOff, out var name) &&
                                TryReadVarUInt(sectionBody, ref fnOff, out var par) &&
                                TryReadVarUInt(sectionBody, ref fnOff, out var sc) &&
                                TryReadVarUInt(sectionBody, ref fnOff, out var op) &&
                                TryReadVarUInt(sectionBody, ref fnOff, out var off) &&
                                TryReadVarUInt(sectionBody, ref fnOff, out var len) &&
                                TryReadVarUInt(sectionBody, ref fnOff, out var flg))
                            {
                                functions.Add(new JsFunctionRow(
                                    (uint)name, (uint)par, (uint)sc, (uint)op, (uint)off, (uint)len, (uint)flg));
                            }
                        }
                    }
                    break;

                case JsFormat.SectionKind.ExceptionRegions:
                    var regOff = 0;
                    if (TryReadVarUInt(sectionBody, ref regOff, out var regCount))
                    {
                        for (var r = 0ul; r < regCount; r++)
                        {
                            if (TryReadVarUInt(sectionBody, ref regOff, out var fnIdx) &&
                                TryReadVarUInt(sectionBody, ref regOff, out var start) &&
                                TryReadVarUInt(sectionBody, ref regOff, out var end) &&
                                TryReadVarUInt(sectionBody, ref regOff, out var hOff) &&
                                TryReadVarUInt(sectionBody, ref regOff, out var depth) &&
                                TryReadVarUInt(sectionBody, ref regOff, out var height))
                            {
                                var hKind = (JsFormat.HandlerKind)sectionBody[regOff++];
                                regions.Add(new JsExceptionRegionRow(
                                    (uint)fnIdx, (uint)start, (uint)end, (uint)hOff, (uint)depth, (uint)height, hKind));
                            }
                        }
                    }
                    break;

                case JsFormat.SectionKind.NativeCode:
                    var nOff = 0;
                    if (TryReadVarUInt(sectionBody, ref nOff, out var archVal) &&
                        TryReadVarUInt(sectionBody, ref nOff, out var semVer) &&
                        TryReadVarUInt(sectionBody, ref nOff, out var align) &&
                        TryReadVarUInt(sectionBody, ref nOff, out var codeLen))
                    {
                        nativeArch = (JsNativeArchitecture)archVal;
                        nativeCode = sectionBody.Slice(nOff, (int)codeLen).ToArray();
                    }
                    break;

                case JsFormat.SectionKind.NativeSymbols:
                    var sOff = 0;
                    if (TryReadVarUInt(sectionBody, ref sOff, out var sCount))
                    {
                        var symList = new List<JsNativeSymbolRow>((int)sCount);
                        for (var i = 0ul; i < sCount; i++)
                        {
                            if (TryReadVarUInt(sectionBody, ref sOff, out var sFnIdx) &&
                                TryReadVarUInt(sectionBody, ref sOff, out var sOffVal))
                            {
                                symList.Add(new JsNativeSymbolRow((uint)sFnIdx, (uint)sOffVal));
                            }
                        }
                        nativeSymbols = symList.ToArray();
                    }
                    break;
            }
        }

        if (nativeCode is not null && nativeSymbols is not null)
        {
            existingEmission = new JsNativeEmission(nativeArch, 1, 16, nativeCode, nativeSymbols);
        }

        refusal = string.Empty;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=551B8C
    // Broiler-Human:        PENDING
    private static byte[] EmitBmcArtifact(
        JsNativeArchitecture arch,
        uint maxOperandStack,
        uint maxScopeSlots,
        List<byte[]> encodedConstants,
        JsNativeEmission emission,
        List<(string Name, uint FunctionIndex)> entries)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

        // BMC\0 magic
        writer.Write((byte)'B');
        writer.Write((byte)'M');
        writer.Write((byte)'C');
        writer.Write((byte)'\0');

        writer.Write(1u); // FormatVersion 1
        var vmArch = arch switch
        {
            JsNativeArchitecture.X64Windows => 1u,
            JsNativeArchitecture.X64SystemV => 2u,
            JsNativeArchitecture.Arm64 => 3u,
            _ => 1u,
        };
        writer.Write(vmArch);
        writer.Write(5u); // 5 sections

        // 1. Limits
        writer.Write(1u); // SectionKind.Limits
        writer.Write(8u);
        writer.Write(maxOperandStack);
        writer.Write(maxScopeSlots);

        // 2. Constants (extract doubles)
        var doubleList = new List<double>();
        foreach (var c in encodedConstants)
        {
            if (c.Length == 9 && c[0] == (byte)JsFormat.ConstantTag.Number)
            {
                doubleList.Add(BinaryPrimitives.ReadDoubleLittleEndian(c.AsSpan(1, 8)));
            }
            else
            {
                doubleList.Add(0.0);
            }
        }

        writer.Write(2u); // SectionKind.Constants
        var constBytesLen = 4u + (uint)(doubleList.Count * 8);
        writer.Write(constBytesLen);
        writer.Write((uint)doubleList.Count);
        foreach (var d in doubleList)
        {
            writer.Write(d);
        }

        // 3. NativeCode
        writer.Write(3u); // SectionKind.NativeCode
        writer.Write((uint)emission.Code.Length);
        writer.Write(emission.Code);

        // 4. NativeSymbols
        writer.Write(4u); // SectionKind.NativeSymbols
        var symBytesLen = 4u + (uint)(emission.Symbols.Length * 12);
        writer.Write(symBytesLen);
        writer.Write((uint)emission.Symbols.Length);
        for (var i = 0; i < emission.Symbols.Length; i++)
        {
            var s = emission.Symbols[i];
            var nextOffset = (i + 1 < emission.Symbols.Length) ? emission.Symbols[i + 1].Offset : (uint)emission.Code.Length;
            var len = nextOffset >= s.Offset ? nextOffset - s.Offset : 0u;
            writer.Write(s.FunctionIndex);
            writer.Write(s.Offset);
            writer.Write(len);
        }

        // 5. Entries
        using var entryStream = new MemoryStream();
        using var entryWriter = new BinaryWriter(entryStream, Encoding.UTF8, leaveOpen: true);
        entryWriter.Write((uint)entries.Count);
        foreach (var (name, fnIdx) in entries)
        {
            entryWriter.Write(fnIdx);
            var utf8 = Encoding.UTF8.GetBytes(name);
            entryWriter.Write((uint)utf8.Length);
            entryWriter.Write(utf8);
        }
        entryWriter.Flush();
        var entryBytes = entryStream.ToArray();

        writer.Write(5u); // SectionKind.Entries
        writer.Write((uint)entryBytes.Length);
        writer.Write(entryBytes);

        writer.Flush();
        return stream.ToArray();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=FF412B
    // Broiler-Human:        PENDING
    private static bool TryReadVarUInt(ReadOnlySpan<byte> span, ref int offset, out ulong value)
    {
        value = 0;
        var shift = 0;
        while (offset < span.Length)
        {
            var b = span[offset++];
            value |= (ulong)(b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                return true;
            }
            shift += 7;
            if (shift >= 64)
            {
                return false;
            }
        }
        return false;
    }
}
