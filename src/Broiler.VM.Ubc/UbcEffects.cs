// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           12
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    High
// Criteria:         4/2
// Resource impact:  0/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Immutable;

namespace Broiler.VM.Ubc;

/// <summary>The two forms of the closed language a family row states its stack effect in.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=677E54
// Broiler-Human:        PENDING
public enum UbcEffectForm : byte
{
    /// <summary>Typed pops and typed pushes, each slot listed. The concept calls this form fixed; the identifier is not, because rule X4 confines that name to the JavaScript native template.</summary>
    Listed = 0,

    /// <summary>Fixed typed pops beneath a run of one type whose length the operand gives, then fixed pushes.</summary>
    Counted = 1,
}

/// <summary>
/// A family row's stack effect, stated as data so that the walk computes heights and types from the
/// row and never from a family method that could disagree with the handler.
/// </summary>
/// <remarks>
/// <para>
/// Every list reads bottom to top. A <see cref="UbcEffectForm.Listed"/> effect pops
/// <see cref="Pops"/> and pushes <see cref="Pushes"/>. A <see cref="UbcEffectForm.Counted"/> effect pops
/// <see cref="Pops"/> beneath a run of <c>operand x Multiplier</c> slots of type
/// <see cref="Repeated"/>, which is the shape of an instruction whose arity its operand encodes. A
/// counted effect is admitted only for the one-field shapes <c>U8</c> and <c>U16</c>, whose whole
/// operand is the count.
/// </para>
/// <para>
/// A row whose effect is not expressible in these two forms is not admitted, which is deliberate: the
/// walk, the interpreter and every encoder read one description, and a family that needs another form
/// asks for a universal bytecode format version rather than a callback.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=0; Fingerprint=5F03D0
// Broiler-Falsified-If: an effect can be constructed whose pops or pushes name a slot type outside the closed set
// Broiler-Human:        PENDING
public sealed class UbcEffect
{
    private UbcEffect(
        UbcEffectForm form,
        ImmutableArray<UbcSlotType> pops,
        UbcSlotType repeated,
        byte multiplier,
        ImmutableArray<UbcSlotType> pushes)
    {
        Form = form;
        Pops = pops;
        Repeated = repeated;
        Multiplier = multiplier;
        Pushes = pushes;
    }

    /// <summary>The form of this effect.</summary>
    public UbcEffectForm Form { get; }

    /// <summary>The fixed pops, bottom to top. For a counted effect, the slots beneath the counted run.</summary>
    public ImmutableArray<UbcSlotType> Pops { get; }

    /// <summary>The type of the counted run. Meaningless for a listed effect.</summary>
    public UbcSlotType Repeated { get; }

    /// <summary>How many slots of the run each unit of the operand counts. Zero for a listed effect.</summary>
    public byte Multiplier { get; }

    /// <summary>The pushes, bottom to top.</summary>
    public ImmutableArray<UbcSlotType> Pushes { get; }

    /// <summary>A listed effect: <paramref name="pops"/> then <paramref name="pushes"/>, every slot typed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3CD8E1
    // Broiler-Falsified-If: a list naming an undefined slot type is accepted rather than thrown back at the table's author
    // Broiler-Human:        PENDING
    public static UbcEffect Listed(ImmutableArray<UbcSlotType> pops, ImmutableArray<UbcSlotType> pushes)
    {
        Check(pops, nameof(pops));
        Check(pushes, nameof(pushes));
        return new UbcEffect(UbcEffectForm.Listed, pops, UbcSlotType.V, 0, pushes);
    }

    /// <summary>A counted effect: <paramref name="fixedPops"/> beneath <c>operand x multiplier</c> slots of <paramref name="repeated"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0318CD
    // Broiler-Falsified-If: a zero multiplier or an undefined repeated type is accepted, so a counted row could pop nothing it names
    // Broiler-Human:        PENDING
    public static UbcEffect Counted(
        ImmutableArray<UbcSlotType> fixedPops,
        UbcSlotType repeated,
        byte multiplier,
        ImmutableArray<UbcSlotType> pushes)
    {
        Check(fixedPops, nameof(fixedPops));
        Check(pushes, nameof(pushes));

        if (!UbcSlotTypes.IsDefined((byte)repeated))
        {
            throw new System.ArgumentOutOfRangeException(nameof(repeated), repeated, "not a slot type");
        }

        if (multiplier == 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(multiplier), multiplier, "a counted run counts at least one slot per unit");
        }

        return new UbcEffect(UbcEffectForm.Counted, fixedPops, repeated, multiplier, pushes);
    }

    /// <summary>
    /// The number of slots this effect pops for <paramref name="operand"/>, or false when that number
    /// exceeds the highest height any plane may hold, so no stack could supply it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=63713B
    // Broiler-Falsified-If: an operand large enough to overflow the product yields a small count instead of false
    // Broiler-Human:        PENDING
    public bool TryGetPopCount(ulong operand, out int count)
    {
        count = Pops.Length;

        if (Form == UbcEffectForm.Listed)
        {
            return true;
        }

        if (operand > UbcFormat.MaxHeight)
        {
            return false;
        }

        var run = operand * Multiplier;
        var total = run + (ulong)Pops.Length;

        if (total > UbcFormat.MaxHeight)
        {
            return false;
        }

        count = (int)total;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4F5986
    // Broiler-Human:        PENDING
    private static void Check(ImmutableArray<UbcSlotType> types, string name)
    {
        if (types.IsDefault)
        {
            throw new System.ArgumentException("a slot list must be given, even when empty", name);
        }

        foreach (var type in types)
        {
            if (!UbcSlotTypes.IsDefined((byte)type))
            {
                throw new System.ArgumentOutOfRangeException(name, type, "not a slot type");
            }
        }
    }
}

/// <summary>Whether a row's operand is a code target, and what the taken edge pushes.</summary>
/// <remarks>
/// A target operand is an absolute code offset inside the instruction's own unit, carried in a
/// <c>U32</c> operand. On the taken edge the row pops what its effect pops and pushes
/// <see cref="TakenPushes"/>, which may differ from the fall-through's pushes - the two-effect shape of
/// an iteration step that pushes one slot when it falls through and none when it branches.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EE4175
// Broiler-Human:        PENDING
public sealed class UbcTarget
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=3BF5A2
    // Broiler-Human:        PENDING
    private UbcTarget(bool isCode, ImmutableArray<UbcSlotType> takenPushes, bool distinctTakenEdge)
    {
        IsCode = isCode;
        TakenPushes = takenPushes;
        HasDistinctTakenEdge = distinctTakenEdge;
    }

    /// <summary>The operand is not a code target.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E4390C
    // Broiler-Human:        PENDING
    public static UbcTarget None { get; } = new(false, ImmutableArray<UbcSlotType>.Empty, false);

    /// <summary>True when the operand is a code target.</summary>
    public bool IsCode { get; }

    /// <summary>True when the taken edge pushes <see cref="TakenPushes"/> rather than the row's own pushes.</summary>
    public bool HasDistinctTakenEdge { get; }

    /// <summary>What the taken edge pushes, bottom to top, when <see cref="HasDistinctTakenEdge"/> is true.</summary>
    public ImmutableArray<UbcSlotType> TakenPushes { get; }

    /// <summary>A code target whose taken edge has the row's own effect.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=70D59C
    // Broiler-Human:        PENDING
    public static UbcTarget Code() => new(true, ImmutableArray<UbcSlotType>.Empty, false);

    /// <summary>A code target whose taken edge pushes <paramref name="takenPushes"/> instead of the row's pushes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=887AF3
    // Broiler-Human:        PENDING
    public static UbcTarget Code(ImmutableArray<UbcSlotType> takenPushes)
    {
        if (takenPushes.IsDefault)
        {
            throw new System.ArgumentException("a slot list must be given, even when empty", nameof(takenPushes));
        }

        foreach (var type in takenPushes)
        {
            if (!UbcSlotTypes.IsDefined((byte)type))
            {
                throw new System.ArgumentOutOfRangeException(nameof(takenPushes), type, "not a slot type");
            }
        }

        return new UbcTarget(true, takenPushes, true);
    }
}
