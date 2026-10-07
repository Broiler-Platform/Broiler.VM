using Broiler.VM.Abstractions;
using Broiler.VM.Profile.WebAssembly;
using Broiler.VM.Ubc;
using System.Collections.Immutable;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The family hook's own refusals, each reached by an artifact the translator would not write.
/// </summary>
/// <remarks>
/// <para>
/// <b>WHY THESE ARE NOT CORPUS ENTRIES.</b> Every retained corpus entry is a WebAssembly module, and every
/// module goes through the translator, which writes module definitions the hook admits: a definitions
/// section it did not write cannot come out of it. The hook refuses such a section all the same,
/// because the core verifies a universal bytecode artifact whoever wrote it. So its refusals are
/// reached here, by taking the artifact the translator wrote for a small module, altering one thing in
/// it, and handing the bytes to the core's verification.
/// </para>
/// <para>
/// <b>The answer each check expects is written down beside it, before the core is asked</b>, as a
/// derived corpus row's is. The alteration is the check's whole content: the module definitions this
/// file writes are written from the layout <c>WasmFamilyData</c> documents, by a writer of this file's
/// own, so a change to the profile's codec that the hook's reader followed and this writer did not is a
/// failure here rather than two copies agreeing. Three controls rewrite an artifact without altering it,
/// and each must still verify: a failing control means the rewriting, not the hook, is wrong.
/// </para>
/// <para>
/// The published registry names each check as the case that reaches its code, and rule W3 holds the
/// name, the code and the reason written here to the registry's row.
/// </para>
/// </remarks>
internal static class HookChecks
{
    private const string Caller = "urn:broiler:wasm-harness/hook";

    /// <summary>One check: the module translated, how its artifact is altered, and the answer it must get.</summary>
    private sealed record HookCase(
        string Name,
        byte[] Module,
        Func<UbcArtifact, UbcArtifact> Alter,
        VmOutcome Outcome,
        VmReason Reason,
        WebAssemblyDiagnosticCode Code);

    /// <summary>Runs every check, prints each failure (and each pass when verbose), and answers the failures.</summary>
    internal static int Report(VmRuntime runtime, bool verbose)
    {
        var cases = Cases();
        var failed = 0;

        Console.WriteLine($"# hook: {cases.Count} checks");

        foreach (var check in cases)
        {
            var (passed, detail) = Run(runtime, check);

            if (!passed)
            {
                failed++;
            }

            if (verbose || !passed)
            {
                Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {check.Name}: {detail}");
            }
        }

        Console.WriteLine($"# hook: {cases.Count - failed} of {cases.Count} checks passed");
        return failed;
    }

    private static (bool Passed, string Detail) Run(VmRuntime runtime, HookCase check)
    {
        var descriptor = ModuleVerification.ArtifactDescriptor(WebAssemblyProfile.SliceManifest, Caller);
        var translation = WasmTranslator.Translate(
            check.Module, ModuleVerification.Ceilings(runtime, in descriptor), CancellationToken.None);

        if (!translation.Succeeded)
        {
            return (false, $"the module did not translate: {translation.Outcome}/{translation.Reason}/{(int)translation.Code}");
        }

        byte[] altered;

        try
        {
            altered = UbcArtifactWriter.Write(check.Alter(Read(translation.Artifact.AsSpan())));
        }
        catch (InvalidOperationException failure)
        {
            return (false, failure.Message);
        }

        var verified = runtime.Verify(in descriptor, altered, CancellationToken.None);

        if (verified.TryGetArtifact(out var admitted))
        {
            admitted.Dispose();
        }

        var code = verified.Diagnostics.ProfileDiagnosticCode;
        var passed = verified.Outcome == check.Outcome && verified.Reason == check.Reason && code == (int)check.Code;

        return (passed,
            $"{verified.Outcome}/{verified.Reason}/{code.ToString(CultureInfo.InvariantCulture)}" +
            (passed ? string.Empty : $", expected {check.Outcome}/{check.Reason}/{((int)check.Code).ToString(CultureInfo.InvariantCulture)}"));
    }

    private static List<HookCase> Cases()
    {
        var plain = CorpusStore.FunctionModule([], [], [], exportSection: ExportF);

        // Two i32.div_s, each an instruction that may trap and so carries a Positions row.
        var twoDivisions = CorpusStore.FunctionModule(
            [], [CorpusStore.I32], [0x41, 0x01, 0x41, 0x01, 0x6D, 0x41, 0x01, 0x6D], exportSection: ExportF);

        // One immutable i32 global, read by the one function.
        var readsAGlobal = CorpusStore.FunctionModule(
            [], [CorpusStore.I32], [0x23, 0x00],
            globalSection: [0x01, CorpusStore.I32, 0x00, 0x41, 0x00, 0x0B],
            exportSection: ExportF);

        // Two functions of type () -> (), the first exported as "f" and the second not exported.
        byte[] twoFunctions =
        [
            .. CorpusStore.Preamble,
            .. CorpusStore.Section(1, [0x01, 0x60, 0x00, 0x00]),
            .. CorpusStore.Section(3, [0x02, 0x00, 0x00]),
            .. CorpusStore.Section(7, ExportF),
            .. CorpusStore.Section(10, [0x02, 0x02, 0x00, 0x0B, 0x02, 0x00, 0x0B]),
        ];

        return
        [
            // The controls: each base module's artifact, read and written back unaltered, verifies.
            new("hook-control-a-rewritten-artifact-verifies", plain, static artifact => artifact,
                VmOutcome.Normal, VmReason.NormalCompleted, 0),
            new("hook-control-a-rewritten-artifact-with-positions-verifies", twoDivisions, static artifact => artifact,
                VmOutcome.Normal, VmReason.NormalCompleted, 0),
            new("hook-control-a-rewritten-artifact-with-a-global-verifies", readsAGlobal, static artifact => artifact,
                VmOutcome.Normal, VmReason.NormalCompleted, 0),

            // The definitions as the hook reads them, before it compares them with anything else.
            new("hook-module-definitions-missing", plain, static artifact => WithFamilyData(artifact, null),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.ModuleDefinitionsMissing),
            new("hook-module-definitions-ending-after-the-version", plain,
                static artifact => WithFamilyData(artifact, new Layout().U32(1).Bytes),
                VmOutcome.InvalidArtifact, VmReason.Truncated, WebAssemblyDiagnosticCode.ModuleDefinitionsTruncated),
            new("hook-module-definitions-with-a-byte-after-the-start-field", plain,
                static artifact => WithFamilyData(artifact, Nothing(new Layout()).U8(0).Bytes),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.ModuleDefinitionsTrailingBytes),
            new("hook-module-definitions-of-layout-version-two", plain,
                static artifact => WithFamilyData(artifact, new Layout().U32(2).Bytes),
                VmOutcome.InvalidArtifact, VmReason.UnknownFormatVersion, WebAssemblyDiagnosticCode.ModuleDefinitionsVersionUnsupported),
            new("hook-module-definitions-memory-presence-byte-of-two", plain,
                static artifact => WithFamilyData(artifact, new Layout().U32(1).U8(2).Bytes),
                VmOutcome.InvalidArtifact, VmReason.MalformedEncoding, WebAssemblyDiagnosticCode.ModuleDefinitionsMalformedPresence),
            new("hook-memory-limits-minimum-above-maximum", plain,
                static artifact => WithFamilyData(artifact, new Layout().U32(1).U8(1).U32(2).U8(1).U32(1).Bytes),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.LimitsMinimumAboveMaximum),
            new("hook-memory-minimum-above-the-formats-page-maximum", plain,
                static artifact => WithFamilyData(artifact, new Layout().U32(1).U8(1).U32(65_537).U8(0).Bytes),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.MemoryPagesAboveFormatMaximum),
            new("hook-thirty-two-bit-global-with-initial-bits-above-its-width", plain,
                static artifact => WithFamilyData(artifact, new Layout().U32(1).U8(0).U8(0).U32(1).U8(0).U8(0).U64(0x1_0000_0000).Bytes),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.GlobalInitialValueOutOfRange),

            // The definitions read, and held to the rest of the artifact.
            new("hook-a-unit-declaring-a-local-of-value-slots", plain, static artifact => WithValueLocal(artifact),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.ValueSlotNotAdmitted),
            new("hook-an-export-naming-a-unit-that-is-not-an-entry", twoFunctions,
                static artifact => WithFamilyData(artifact, Exporting(new Layout(), unit: 1).Bytes),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.ExportedFunctionNotAnEntry),
            new("hook-positions-rows-out-of-order", twoDivisions, static artifact => WithPositionsSwapped(artifact),
                VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, WebAssemblyDiagnosticCode.PositionsNotOrdered),

            // And at the one instruction that reads a global.
            new("hook-a-global-read-typed-other-than-its-global", readsAGlobal,
                static artifact => WithFamilyData(artifact, Exporting(new Layout().U32(1).U8(0).U8(0).U32(1).U8(1).U8(0).U64(0), unit: 0).Bytes),
                VmOutcome.InvalidArtifact, VmReason.SemanticValidationFailed, WebAssemblyDiagnosticCode.GlobalRowTypeMismatch),
        ];
    }

    /// <summary>The export section entry naming function 0 "f": one export, name length one, 'f', kind function, index zero.</summary>
    private static readonly byte[] ExportF = [0x01, 0x01, 0x66, 0x00, 0x00];

    /// <summary>Definitions declaring nothing: version one, no memory, no table, no globals, elements, data or exports, no start.</summary>
    private static Layout Nothing(Layout layout) =>
        layout.U32(1).U8(0).U8(0).U32(0).U32(0).U32(0).U32(0).U32(0xFFFF_FFFF);

    /// <summary>The rest of the definitions after the globals: no elements or data, export "f" of <paramref name="unit"/>, no start.</summary>
    private static Layout Exporting(Layout layout, uint unit)
    {
        if (layout.Length == 0)
        {
            // Nothing written yet: version one, no memory, no table, no globals.
            layout.U32(1).U8(0).U8(0).U32(0);
        }

        return layout.U32(0).U32(0).U32(1).U32(1).U8(0x66).U8(0).U32(unit).U32(0xFFFF_FFFF);
    }

    private static UbcArtifact Read(ReadOnlySpan<byte> artifact)
    {
        var bounds = new VmReadBounds((ulong)artifact.Length, 64, 1 << 20, 16);

        if (!UbcArtifactReader.TryRead(artifact, in bounds, new Unmetered(), 4096, UbcFormat.FormatVersion, out var decoded, out var refusal))
        {
            throw new InvalidOperationException(
                $"the translator's own artifact did not read: {refusal.Code.ToString(CultureInfo.InvariantCulture)}");
        }

        return decoded!;
    }

    /// <summary>The artifact with its one FamilyData section's body replaced, or with no FamilyData section when null.</summary>
    private static UbcArtifact WithFamilyData(UbcArtifact a, byte[]? body)
    {
        if (a.FamilyData.Length != 1)
        {
            throw new InvalidOperationException($"the translator's artifact carries {a.FamilyData.Length} FamilyData sections rather than one");
        }

        var slot = a.FamilyData[0].Slot;
        var data = body is null
            ? ImmutableArray<UbcFamilyData>.Empty
            : [new UbcFamilyData(slot, [.. body])];

        // The sections the translator wrote stay present, but for the FamilyData one when it is removed.
        var present = body is null ? a.PresentSections & ~(1u << UbcFormat.FamilyDataKind(slot)) : a.PresentSections;

        return new UbcArtifact(
            a.Header, a.Families, a.Types, a.Units, a.Code, a.JumpTables, a.Regions, a.Entries, a.Positions, data, a.Emission, present);
    }

    /// <summary>The artifact with one run of one value-slot local added to its first unit.</summary>
    private static UbcArtifact WithValueLocal(UbcArtifact a)
    {
        var first = a.Units[0];
        var unit = new UbcUnit(
            first.TypeIndex, first.FamilySlot, [.. first.Locals, new UbcLocalRun(1, UbcSlotType.V)],
            first.MaxWordHeight, first.MaxValueHeight, first.CodeOffset, first.CodeLength, first.Flags, first.Landings);

        return new UbcArtifact(
            a.Header, a.Families, a.Types, a.Units.SetItem(0, unit), a.Code, a.JumpTables, a.Regions, a.Entries, a.Positions,
            a.FamilyData, a.Emission, a.PresentSections);
    }

    /// <summary>The artifact with its first two Positions rows exchanged.</summary>
    private static UbcArtifact WithPositionsSwapped(UbcArtifact a)
    {
        if (a.Positions.Length < 2)
        {
            throw new InvalidOperationException($"the translator's artifact carries {a.Positions.Length} Positions rows rather than two or more");
        }

        var positions = a.Positions.SetItem(0, a.Positions[1]).SetItem(1, a.Positions[0]);

        return new UbcArtifact(
            a.Header, a.Families, a.Types, a.Units, a.Code, a.JumpTables, a.Regions, a.Entries, positions,
            a.FamilyData, a.Emission, a.PresentSections);
    }

    /// <summary>Writes the module definitions' fixed-width, little-endian fields, as WasmFamilyData documents them.</summary>
    private sealed class Layout
    {
        private readonly List<byte> bytes = [];

        internal int Length => bytes.Count;

        internal byte[] Bytes => [.. bytes];

        internal Layout U8(byte value)
        {
            bytes.Add(value);
            return this;
        }

        internal Layout U32(uint value)
        {
            for (var shift = 0; shift < 32; shift += 8)
            {
                bytes.Add((byte)(value >> shift));
            }

            return this;
        }

        internal Layout U64(ulong value)
        {
            for (var shift = 0; shift < 64; shift += 8)
            {
                bytes.Add((byte)(value >> shift));
            }

            return this;
        }
    }

    /// <summary>A meter that admits every charge: the harness reads the translator's artifact, which is not untrusted here.</summary>
    private sealed class Unmetered : IVmBoundedAllocationMeter
    {
        public bool TryReserve(ulong byteCount) => true;

        public void Release(ulong byteCount)
        {
        }

        public bool TryChargeWork(ulong workUnits) => true;

        public bool Poll() => true;
    }
}
