using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Immutable;

namespace Com.Example.Tally;

/// <summary>The fixture family's one value type: a tally of one field.</summary>
/// <remarks>
/// Immutable, so a slot of the value plane can be copied by reference and a captured suspension shares
/// values with the plane it was captured from without either seeing the other change.
/// </remarks>
public sealed class TallyValue
{
    /// <summary>A tally of <paramref name="amount"/>.</summary>
    public TallyValue(long amount) => Amount = amount;

    /// <summary>The tally's one field.</summary>
    public long Amount { get; }

    /// <summary>The amount a slot holds, reading an empty slot as zero.</summary>
    public static long AmountOf(TallyValue? value) => value?.Amount ?? 0;
}

/// <summary>The fixture family's value plane: an array of tallies reached by index.</summary>
public sealed class TallyPlane : IUbcValuePlane
{
    private TallyValue?[] slots;

    /// <summary>A plane with room for <paramref name="capacity"/> tallies.</summary>
    public TallyPlane(int capacity) => slots = new TallyValue?[capacity];

    /// <inheritdoc/>
    public int Capacity => slots.Length;

    /// <inheritdoc/>
    public int ValueBytes => System.IntPtr.Size;

    /// <summary>The tally at <paramref name="index"/>, or null for an empty slot.</summary>
    public TallyValue? this[int index]
    {
        get => slots[index];
        set => slots[index] = value;
    }

    /// <inheritdoc/>
    public void Resize(int capacity)
    {
        if (capacity > slots.Length)
        {
            System.Array.Resize(ref slots, capacity);
        }
    }

    /// <inheritdoc/>
    public void Copy(int from, int to) => slots[to] = slots[from];

    /// <inheritdoc/>
    public void Clear(int start, int count) => System.Array.Clear(slots, start, count);
}

/// <summary>What a Tally instance keeps: the environment it may load guest programs through.</summary>
public sealed class TallyInstanceState
{
    internal TallyInstanceState(IVmExecutionEnvironment environment, UbcVerifiedProgram program)
    {
        Environment = environment;
        Program = program;
    }

    internal IVmExecutionEnvironment Environment { get; }

    internal UbcVerifiedProgram Program { get; }

    /// <summary>How many guest programs this instance has loaded.</summary>
    public int Loads { get; internal set; }

    /// <summary>The reason the last guest load was answered with, which a load-refused fault carries.</summary>
    public VmReason LastLoad { get; internal set; }
}

/// <summary>
/// The fixture family's handlers, value plane, payloads and frame codec: a struct whose static members
/// the bytecode emitter's loop reaches with no interface dispatch.
/// </summary>
/// <remarks>
/// <para>
/// Every row of <see cref="TallyTable"/> is handled here, the primitive rows included: an emitter may
/// execute a primitive itself and must answer what this handler answers, and the primitive input corpus
/// records the handler's answer for every input it holds.
/// </para>
/// <para>
/// A handler reads its inputs at the activation's argument bases and writes its outputs there; it never
/// moves a top and never pushes a frame.
/// </para>
/// </remarks>
public struct TallyFamily : IUbcFamily
{
    /// <inheritdoc/>
    public static UbcStatus Handle(ref UbcActivation activation, byte familyOpcode, ulong operand)
    {
        var plane = (TallyPlane)activation.Values;
        var words = activation.Words;
        var word = activation.WordArgs;
        var value = activation.ValueArgs;

        switch (familyOpcode)
        {
            case TallyTable.Const:
                plane[value] = new TallyValue((int)(uint)operand);
                return UbcStatus.Next;

            case TallyTable.Add:
            {
                var left = TallyValue.AmountOf(plane[value]);
                var right = TallyValue.AmountOf(plane[value + 1]);
                var sum = left + right;

                if (((left ^ sum) & (right ^ sum)) < 0)
                {
                    return UbcStatus.Trap(TallyTable.TrapOverflow);
                }

                plane[value] = new TallyValue(sum);
                return UbcStatus.Next;
            }

            case TallyTable.Count:
                words[word] = (ulong)TallyValue.AmountOf(plane[value]);
                return UbcStatus.Next;

            case TallyTable.AddWords:
            case TallyTable.MulWords:
            case TallyTable.DivWords:
            case TallyTable.IsZero:
            {
                // The handler's answer for a primitive row is the primitive table's own reference
                // implementation: an emitter executing the row inline must agree with it bit for bit.
                TallyTable.Table.TryGetRow(familyOpcode, out var row);
                var result = UbcPrimitives.Evaluate(row.Primitive!.Value, words[word], row.Effect.Pops.Length > 1 ? words[word + 1] : 0, TallyTable.Table.CanonicaliseNaN);

                if (result.Trap != UbcTrapCode.None)
                {
                    foreach (var mapping in row.Traps)
                    {
                        if (mapping.Universal == result.Trap)
                        {
                            return UbcStatus.Trap(mapping.FamilyCode);
                        }
                    }

                    return UbcStatus.Defect;
                }

                words[word] = result.Bits;
                return UbcStatus.Next;
            }

            case TallyTable.Throw:
                activation.Pending = plane[value] ?? new TallyValue(0);
                return UbcStatus.Threw;

            case TallyTable.IfPositive:
                return TallyValue.AmountOf(plane[value]) > 0 ? UbcStatus.Taken : UbcStatus.Next;

            case TallyTable.CallUnit:
                activation.CallRequest = new UbcCallRequest((int)(uint)operand);
                return UbcStatus.Request;

            case TallyTable.Yield:
                activation.Pending = plane[value] ?? new TallyValue(0);
                return UbcStatus.Suspend;

            case TallyTable.Load:
                return Load(ref activation, (int)operand);

            default:
                return UbcStatus.Defect;
        }
    }

    /// <inheritdoc/>
    public static void OnLand(ref UbcActivation activation, byte regionKind)
    {
        var plane = (TallyPlane)activation.Values;
        plane[activation.ValueArgs] = activation.Pending as TallyValue ?? new TallyValue(0);
    }

    /// <inheritdoc/>
    public static void OnResume(ref UbcActivation activation, object reason)
    {
        var plane = (TallyPlane)activation.Values;
        plane[activation.ValueArgs] = reason as TallyValue ?? new TallyValue(0);
    }

    /// <inheritdoc/>
    public static IUbcValuePlane CreateValuePlane(int capacity) => new TallyPlane(capacity);

    /// <inheritdoc/>
    public static object CreateInstance(UbcInstanceContext context) =>
        new TallyInstanceState(context.Environment, context.Program);

    /// <inheritdoc/>
    public static bool BindParameters(ref UbcActivation activation, System.ReadOnlySpan<byte> entryName)
    {
        // Every entry's parameters start at zero: a word parameter as the word zero, a value parameter
        // as a tally of nothing. The program list's entries take none, so this is for completeness.
        var unit = activation.Program.Units[activation.Unit];
        var plane = (TallyPlane)activation.Values;
        System.Array.Clear(activation.Words, activation.WordBase, unit.ParameterWords);

        for (var index = 0; index < unit.ParameterValues; index++)
        {
            plane[activation.ValueBase + index] = new TallyValue(0);
        }

        return true;
    }

    /// <inheritdoc/>
    public static IVmProfilePayload? Completion(ref UbcActivation activation)
    {
        var unit = activation.Program.Units[activation.Unit];
        var plane = (TallyPlane)activation.Values;
        var words = ImmutableArray.CreateBuilder<long>(unit.ResultWords);
        var values = ImmutableArray.CreateBuilder<long>(unit.ResultValues);

        for (var index = 0; index < unit.ResultWords; index++)
        {
            words.Add((long)activation.Words[activation.WordArgs + index]);
        }

        for (var index = 0; index < unit.ResultValues; index++)
        {
            values.Add(TallyValue.AmountOf(plane[activation.ValueArgs + index]));
        }

        return new TallyResult(words.MoveToImmutable(), values.MoveToImmutable());
    }

    /// <inheritdoc/>
    public static IVmProfilePayload Fault(ref UbcActivation activation, byte familySlot, ushort code) =>
        new TallyFault(
            familySlot == 0 && code == 0 ? TallyFaultKind.Unreachable : TallyFaultKind.Trap,
            code,
            0,
            code == TallyTable.TrapLoadRefused && activation.InstanceState is TallyInstanceState state ? state.LastLoad : VmReason.None);

    /// <inheritdoc/>
    public static IVmProfilePayload? Uncaught(ref UbcActivation activation) =>
        new TallyFault(TallyFaultKind.Uncaught, 0, TallyValue.AmountOf(activation.Pending as TallyValue));

    /// <inheritdoc/>
    public static IVmProfilePayload? SuspendProjection(ref UbcActivation activation) =>
        new TallySuspension(TallyValue.AmountOf(activation.Pending as TallyValue));

    /// <inheritdoc/>
    public static IVmProfilePayload? EntryRefused(object instanceState, System.ReadOnlySpan<byte> entryName) =>
        new TallyFault(TallyFaultKind.NoSuchEntry, 0, 0);

    /// <inheritdoc/>
    public static object CaptureValues(IUbcValuePlane plane, int start, int count)
    {
        var tallies = (TallyPlane)plane;
        var captured = new TallyValue?[count];

        for (var index = 0; index < count; index++)
        {
            captured[index] = tallies[start + index];
        }

        return captured;
    }

    /// <inheritdoc/>
    public static void RestoreValues(IUbcValuePlane plane, int start, object captured)
    {
        var tallies = (TallyPlane)plane;
        var saved = (TallyValue?[])captured;

        for (var index = 0; index < saved.Length; index++)
        {
            tallies[start + index] = saved[index];
        }
    }

    /// <summary>
    /// Loads the guest program the operand names through the composition's provider, and asks the
    /// emitter to enter its <c>main</c> with the row's input: the same family, the same emitter, the
    /// program verified by the same descriptor's verifier because the core verifies what the mediator
    /// hands back.
    /// </summary>
    private static UbcStatus Load(ref UbcActivation activation, int name)
    {
        var state = (TallyInstanceState)activation.InstanceState!;

        if (!state.Environment.TryGetArtifactLoadMediator(out var mediator))
        {
            return UbcStatus.Trap(TallyTable.TrapLoadRefused);
        }

        var payload = System.Text.Encoding.ASCII.GetBytes(TallyPrograms.GuestName(name));
        var request = new VmArtifactRequest(TallyProfile.Id, default, default, 1, default, default, new VmBytes(payload));
        var loaded = mediator.RequestLoad(in request);
        state.Loads++;
        state.LastLoad = loaded.Reason;

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact) ||
            !artifact.TryGetState(out var verified) || verified is not UbcVerifiedProgram program ||
            !program.TryGetEntry("main"u8, out var unit))
        {
            return UbcStatus.Trap(TallyTable.TrapLoadRefused);
        }

        activation.CallRequest = new UbcCallRequest(program, unit);
        return UbcStatus.Request;
    }
}
