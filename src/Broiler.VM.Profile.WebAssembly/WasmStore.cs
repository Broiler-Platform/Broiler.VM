// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   18
// Annotated:        18/18
// Exempt:           9
// Human-reviewed:   0/18
// IP risk:          Low
// Security risk:    Critical
// Criteria:         10/10
// Resource impact:  4/10 max
// Unverified:       18
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// One table: a vector of function indices, where a slot holding
/// <see cref="NullFunctionReference"/> is the null reference.
/// </summary>
/// <remarks>
/// A funcref is held as an index rather than as a reference because this build admits no reference
/// value on an operand stack: the only instruction that reads a table is <c>call_indirect</c>, which
/// wants a function to call. The moment reference types open, this becomes a reference array and
/// the index form stops being enough - which is why the type is here rather than being a bare
/// <c>int[]</c> the interpreter indexes.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=6C840E
// Broiler-Falsified-If: a slot is read without its index being compared against the entry count first
// Broiler-Human:        PENDING
internal sealed class WasmTableInstance
{
    /// <summary>The value a slot holds until an element segment writes a function into it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0C3E2A
    // Broiler-Human:        PENDING
    internal const int NullFunctionReference = -1;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=877C3D
    // Broiler-Human:        PENDING
    private readonly int[] entries;

    /// <summary>Takes an already-charged and already-nulled entry vector.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F6D7CC
    // Broiler-Human:        PENDING
    internal WasmTableInstance(int[] initial) => entries = initial;

    /// <summary>How many entries the table holds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A47191
    // Broiler-Human:        PENDING
    internal int EntryCount => entries.Length;

    /// <summary>Reads one entry, or refuses because the index is outside the table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=895FB1
    // Broiler-Falsified-If: an index at or past the entry count is read rather than refused
    // Broiler-Human:        PENDING
    internal bool TryRead(uint index, out int functionIndex)
    {
        functionIndex = NullFunctionReference;

        if (index >= (uint)entries.Length)
        {
            return false;
        }

        functionIndex = entries[(int)index];
        return true;
    }

    /// <summary>
    /// Writes an element segment's entries in, after checking the whole segment fits.
    /// </summary>
    /// <remarks>
    /// Atomic per segment for the same reason a data segment is: the range is checked whole before
    /// one entry is written, and a segment already applied stays applied when a later one is
    /// refused.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=C8FAE1
    // Broiler-Falsified-If: a prefix of a segment that does not fit is written
    // Broiler-Human:        PENDING
    internal bool TryInitialise(ulong offset, System.ReadOnlySpan<uint> functionIndices)
    {
        if (offset + (ulong)functionIndices.Length > (ulong)entries.Length)
        {
            return false;
        }

        for (var index = 0; index < functionIndices.Length; index++)
        {
            entries[(int)offset + index] = (int)functionIndices[index];
        }

        return true;
    }
}

/// <summary>
/// The store one instance runs against: its memories, its tables and its globals.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE STORE IS ALLOCATED PER INSTANCE HERE, AND THAT DECIDES NOTHING ABOUT THE STORE
/// READING.</b> The roadmap's section 11 keeps two readings live - an artifact that is a container
/// of several modules with a link plan, and a store held once per runtime that instances open into
/// - and rejects the third by name. This build chooses neither, because it cannot exercise the
/// difference: no module here declares an import, so no instance can ever see another's export and
/// the question of which store an instance lands in has one answer whichever reading is later taken.
/// A build that shipped a linker would be taking that decision by writing one; this one does not
/// ship a linker.
/// </para>
/// <para>
/// Everything here is mutable and none of it is reachable from a verified handle. The module is
/// immutable and shared; the store is per instance and private to it, which is what lets two
/// runtimes read one shareable handle with no synchronisation at all.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=F2E105
// Broiler-Falsified-If: anything here becomes reachable from an artifact's state once verification has returned
// Broiler-Human:        PENDING
internal sealed class WasmStore
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=590565
    // Broiler-Human:        PENDING
    private readonly WasmMemoryInstance[] memories;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=879040
    // Broiler-Human:        PENDING
    private readonly WasmTableInstance[] tables;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=04FB6E
    // Broiler-Human:        PENDING
    private readonly WasmValue[] globals;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8E5323
    // Broiler-Human:        PENDING
    private WasmStore(WasmMemoryInstance[] allocatedMemories, WasmTableInstance[] allocatedTables, WasmValue[] allocatedGlobals)
    {
        memories = allocatedMemories;
        tables = allocatedTables;
        globals = allocatedGlobals;
    }

    /// <summary>The memories, of which this format version admits at most one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5D0C4F
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmMemoryInstance> Memories =>
        System.MemoryExtensions.AsSpan(memories);

    /// <summary>The tables, of which this format version admits at most one.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0B8CF6
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmTableInstance> Tables =>
        System.MemoryExtensions.AsSpan(tables);

    /// <summary>The globals, indexed as the module indexes them.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=82FEFA
    // Broiler-Human:        PENDING
    internal WasmValue[] Globals => globals;

    /// <summary>
    /// Allocates the store a module needs, charging every byte before it exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A memory is allocated at its declared minimum and a table at its declared minimum, both
    /// zeroed - a memory to zero bytes, a table to null references. Each allocation is charged
    /// against the allowance and then reported as retained, because a store's bytes live for as long
    /// as the instance does.
    /// </para>
    /// <para>
    /// A declared minimum above this profile's own page ceiling is refused here rather than at the
    /// first growth, and it is refused as an allocation this profile will not make rather than as a
    /// budget exhaustion, because no budget was consulted.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=9C0CD5
    // Broiler-Falsified-If: an array is allocated before its charge returns true, or a declared minimum above the profile ceiling is allocated
    // Broiler-Human:        PENDING
    internal static bool TryAllocate(
        WasmModule module, IVmMeter meter, out WasmStore? store, out bool refusedByProfileCeiling)
    {
        store = null;
        refusedByProfileCeiling = false;

        var memoryCount = module.MemoryCount;
        var allocatedMemories = memoryCount == 0
            ? []
            : new WasmMemoryInstance[memoryCount];

        for (var index = 0; index < memoryCount; index++)
        {
            var declared = module.Memories[index].Limits;
            var maximum = declared.HasMaximum ? declared.Maximum : WasmTypeGrammar.MaximumMemoryPages;

            if (declared.Minimum > WasmMemoryInstance.ProfileMaximumPages)
            {
                refusedByProfileCeiling = true;
                ReleasePartial(allocatedMemories, allocatedTables: [], meter);
                return false;
            }

            var initialBytes = (ulong)declared.Minimum * WasmMemoryInstance.PageBytes;

            if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, initialBytes))
            {
                ReleasePartial(allocatedMemories, allocatedTables: [], meter);
                return false;
            }

            allocatedMemories[index] = new WasmMemoryInstance(new byte[(int)initialBytes], maximum);
            meter.ReportRetained(VmBudgetDimension.LiveBytes, initialBytes);
        }

        var tableCount = module.TableCount;
        var allocatedTables = tableCount == 0 ? [] : new WasmTableInstance[tableCount];

        for (var index = 0; index < tableCount; index++)
        {
            var declared = module.Tables[index].Limits;

            if (declared.Minimum > ProfileMaximumTableEntries)
            {
                refusedByProfileCeiling = true;
                ReleasePartial(allocatedMemories, allocatedTables, meter);
                return false;
            }

            var entryBytes = (ulong)declared.Minimum * sizeof(int);

            if (!meter.TryCharge(VmBudgetDimension.AllocatedBytes, entryBytes))
            {
                ReleasePartial(allocatedMemories, allocatedTables, meter);
                return false;
            }

            var entries = new int[(int)declared.Minimum];
            System.Array.Fill(entries, WasmTableInstance.NullFunctionReference);
            allocatedTables[index] = new WasmTableInstance(entries);
            meter.ReportRetained(VmBudgetDimension.LiveBytes, entryBytes);
        }

        var globalCount = module.GlobalCount;
        var allocatedGlobals = globalCount == 0 ? [] : new WasmValue[globalCount];

        store = new WasmStore(allocatedMemories, allocatedTables, allocatedGlobals);
        return true;
    }

    /// <summary>
    /// Reports released whatever an abandoned allocation had already retained.
    /// </summary>
    /// <remarks>
    /// <b>A REFUSED ALLOCATION MUST NOT LEAVE RETAINED BYTES BEHIND IT.</b> Retention is a ceiling
    /// and a ceiling is not per-operation, so bytes reported retained by a store that was never
    /// published would go on counting against every later instance of the same runtime. The array
    /// slots that were never filled are null and are skipped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EC74DD
    // Broiler-Falsified-If: a refused allocation leaves any byte reported retained and never released
    // Broiler-Human:        PENDING
    private static void ReleasePartial(
        WasmMemoryInstance?[] allocatedMemories, WasmTableInstance?[] allocatedTables, IVmMeter meter)
    {
        for (var index = 0; index < allocatedMemories.Length; index++)
        {
            allocatedMemories[index]?.Release(meter);
        }

        for (var index = 0; index < allocatedTables.Length; index++)
        {
            if (allocatedTables[index] is { } table)
            {
                meter.ReportReleased(
                    VmBudgetDimension.LiveBytes, (ulong)table.EntryCount * sizeof(int));
            }
        }
    }

    /// <summary>
    /// The largest table this profile will allocate, declared here for the same reason the page
    /// ceiling is declared on the memory.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0F97A2
    // Broiler-Falsified-If: this bound is derived from a limit vector rather than declared here
    // Broiler-Human:        PENDING
    internal const uint ProfileMaximumTableEntries = 1_048_576;

    /// <summary>Reports every byte this store retained as released.</summary>
    /// <remarks>
    /// It runs no guest code. There is nothing to finalise in WebAssembly at any manifest this build
    /// admits, so releasing a store is dropping arrays and telling the meter their bytes are gone.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8C390E
    // Broiler-Falsified-If: any guest instruction is dispatched on this path
    // Broiler-Human:        PENDING
    internal void Release(IVmMeter meter)
    {
        for (var index = 0; index < memories.Length; index++)
        {
            memories[index].Release(meter);
        }

        for (var index = 0; index < tables.Length; index++)
        {
            meter.ReportReleased(
                VmBudgetDimension.LiveBytes, (ulong)tables[index].EntryCount * sizeof(int));
        }
    }
}

/// <summary>The instance state a successful instantiation publishes: one module and one store.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=95661E
// Broiler-Falsified-If: an instance is published while any segment or the start function refused
// Broiler-Human:        PENDING
public sealed class WasmInstance : IVmInstanceState
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F6B2CF
    // Broiler-Human:        PENDING
    internal WasmInstance(WasmModule instantiated, WasmStore allocated)
    {
        Module = instantiated;
        Store = allocated;
    }

    /// <summary>The verified module this instance runs.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7529E1
    // Broiler-Human:        PENDING
    internal WasmModule Module { get; }

    /// <summary>The store this instance owns.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D1EA74
    // Broiler-Human:        PENDING
    internal WasmStore Store { get; }

    /// <summary>How many times an entry point has been invoked on this instance.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D7D16B
    // Broiler-Human:        PENDING
    public int InvocationCount { get; internal set; }
}

/// <summary>
/// The continuation type the contract requires this profile to have and which nothing here
/// produces.
/// </summary>
/// <remarks>
/// <b>NOTHING IN THIS BUILD PARKS A FRAME.</b> At every manifest this profile accepts, execution
/// runs to completion or to a trap: there is no instruction that suspends, the descriptor declares
/// neither asynchronous instantiation nor external suspension, and no path in this assembly returns
/// a suspended step. The type exists because the contract names it and because
/// <see cref="WebAssemblyExecutor.Unwind"/> must have something to recognise if a later surface ever
/// mints one - and because a store abandoned mid-step is exactly what an unwind would have to
/// release. A reader should take the presence of this type as the shape of a future obligation and
/// not as evidence that this build suspends.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=56FA69
// Broiler-Falsified-If: any path in this assembly constructs one of these and hands it to the core
// Broiler-Human:        PENDING
public sealed class WasmContinuation : IVmProfileContinuation
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=80C6B8
    // Broiler-Human:        PENDING
    internal WasmContinuation(WasmStore parked) => Store = parked;

    /// <summary>The store the parked operation was running against.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D1EA74
    // Broiler-Human:        PENDING
    internal WasmStore Store { get; }
}
