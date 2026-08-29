namespace Broiler.VM.Fixtures;

/// <summary>
/// A deterministic, seeded, structure-aware mutator over fixture artifact bytes.
/// </summary>
/// <remarks>
/// <para>
/// Deterministic because a fuzz finding that cannot be replayed is an anecdote. Every session is a
/// total function of its seed and its seed corpus, so a failing iteration is reproduced by naming
/// the same two things, and the minimized bytes are retained besides.
/// </para>
/// <para>
/// Structure-aware because a purely random byte source spends nearly all of its budget failing the
/// four-byte magic. The mutators that matter are the ones that keep an artifact plausible enough to
/// reach the parts of the verifier worth attacking: rewrite a declared count, rewrite a section
/// length, splice two artifacts, duplicate a chunk. The blind bit flip is kept as well, because a
/// mutator that only produces shapes its author thought of only finds defects its author thought of.
/// </para>
/// <para>
/// It has no opinion about what verification should answer. Deciding that is
/// <see cref="FixtureFuzzInvariants"/>, and keeping the two apart is what lets the mutator run
/// against any composition.
/// </para>
/// </remarks>
public sealed class FixtureFuzzMutator
{
    private ulong state;

    /// <summary>Creates a mutator from a seed. Two mutators with one seed produce one sequence.</summary>
    public FixtureFuzzMutator(ulong seed) =>
        // Never zero: xorshift is stuck at zero, and a zero seed would silently produce one
        // constant "random" sequence for every iteration of the session.
        state = seed == 0 ? 0x9E3779B97F4A7C15 : seed;

    /// <summary>How many distinct mutators the sequence draws from.</summary>
    public const int MutatorCount = 9;

    /// <summary>Produces one mutant from <paramref name="seedCorpus"/>.</summary>
    public byte[] Next(System.Collections.Generic.IReadOnlyList<byte[]> seedCorpus)
    {
        if (seedCorpus.Count == 0)
        {
            return [];
        }

        var source = seedCorpus[(int)(NextUInt64() % (ulong)seedCorpus.Count)];
        var mutant = (byte[])source.Clone();
        var rounds = 1 + (int)(NextUInt64() % 3);

        for (var round = 0; round < rounds; round++)
        {
            mutant = Apply(mutant, seedCorpus);
        }

        return mutant;
    }

    /// <summary>The next value of the sequence. Exposed so a driver can label an iteration.</summary>
    public ulong NextUInt64()
    {
        // xorshift64*, chosen because it is four lines of arithmetic with no framework dependency
        // and no platform variation. Nothing here needs statistical quality; it needs to be the
        // same sequence on every machine and in every publish mode, which a framework generator
        // does not promise.
        state ^= state >> 12;
        state ^= state << 25;
        state ^= state >> 27;
        return state * 0x2545F4914F6CDD1D;
    }

    private byte[] Apply(byte[] input, System.Collections.Generic.IReadOnlyList<byte[]> seedCorpus)
    {
        var choice = (int)(NextUInt64() % MutatorCount);

        return choice switch
        {
            0 => FlipBit(input),
            1 => SetByte(input),
            2 => Truncate(input),
            3 => Extend(input),
            4 => DuplicateChunk(input),
            5 => DeleteChunk(input),
            6 => Splice(input, seedCorpus),
            7 => RewriteVarInt(input),
            8 => SetInterestingByte(input),
            _ => input,
        };
    }

    private byte[] FlipBit(byte[] input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        var output = (byte[])input.Clone();
        var index = (int)(NextUInt64() % (ulong)output.Length);
        output[index] ^= (byte)(1 << (int)(NextUInt64() % 8));
        return output;
    }

    private byte[] SetByte(byte[] input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        var output = (byte[])input.Clone();
        output[(int)(NextUInt64() % (ulong)output.Length)] = (byte)NextUInt64();
        return output;
    }

    /// <summary>Sets a byte to one of the values a length, a count or a continuation bit lives at.</summary>
    private byte[] SetInterestingByte(byte[] input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        var interesting = new byte[] { 0x00, 0x01, 0x7F, 0x80, 0x81, 0xFE, 0xFF };
        var output = (byte[])input.Clone();
        output[(int)(NextUInt64() % (ulong)output.Length)] = interesting[(int)(NextUInt64() % (ulong)interesting.Length)];
        return output;
    }

    private byte[] Truncate(byte[] input)
    {
        if (input.Length == 0)
        {
            return input;
        }

        var length = (int)(NextUInt64() % (ulong)input.Length);
        var output = new byte[length];
        System.Array.Copy(input, output, length);
        return output;
    }

    private byte[] Extend(byte[] input)
    {
        var added = 1 + (int)(NextUInt64() % 16);
        var output = new byte[input.Length + added];
        System.Array.Copy(input, output, input.Length);

        for (var index = input.Length; index < output.Length; index++)
        {
            output[index] = (byte)NextUInt64();
        }

        return output;
    }

    private byte[] DuplicateChunk(byte[] input)
    {
        if (input.Length < 2)
        {
            return input;
        }

        var start = (int)(NextUInt64() % (ulong)input.Length);
        var length = 1 + (int)(NextUInt64() % (ulong)(input.Length - start));

        var output = new byte[input.Length + length];
        System.Array.Copy(input, 0, output, 0, start + length);
        System.Array.Copy(input, start, output, start + length, length);
        System.Array.Copy(input, start + length, output, start + 2 * length, input.Length - start - length);
        return output;
    }

    private byte[] DeleteChunk(byte[] input)
    {
        if (input.Length < 2)
        {
            return input;
        }

        var start = (int)(NextUInt64() % (ulong)input.Length);
        var length = 1 + (int)(NextUInt64() % (ulong)(input.Length - start));

        var output = new byte[input.Length - length];
        System.Array.Copy(input, 0, output, 0, start);
        System.Array.Copy(input, start + length, output, start, input.Length - start - length);
        return output;
    }

    private byte[] Splice(byte[] input, System.Collections.Generic.IReadOnlyList<byte[]> seedCorpus)
    {
        var other = seedCorpus[(int)(NextUInt64() % (ulong)seedCorpus.Count)];

        if (input.Length == 0 || other.Length == 0)
        {
            return input;
        }

        var head = (int)(NextUInt64() % (ulong)input.Length);
        var tail = (int)(NextUInt64() % (ulong)other.Length);

        var output = new byte[head + (other.Length - tail)];
        System.Array.Copy(input, 0, output, 0, head);
        System.Array.Copy(other, tail, output, head, other.Length - tail);
        return output;
    }

    /// <summary>
    /// Rewrites a variable-length integer in place, keeping its byte width so the framing around it
    /// stays plausible.
    /// </summary>
    /// <remarks>
    /// This is the mutator that reaches the interesting code. A declared count, a section length and
    /// a format version are all variable-length integers, and rewriting one to a hostile value while
    /// leaving the rest of the artifact intact is the shape a blind bit flipper almost never
    /// produces: everything downstream still parses, so the count is what the verifier has to
    /// survive.
    /// </remarks>
    private byte[] RewriteVarInt(byte[] input)
    {
        if (input.Length <= 4)
        {
            return input;
        }

        var output = (byte[])input.Clone();
        var start = 4 + (int)(NextUInt64() % (ulong)(output.Length - 4));

        var width = 0;

        while (start + width < output.Length && width < 10)
        {
            width++;

            if ((output[start + width - 1] & 0x80) == 0)
            {
                break;
            }
        }

        if (width == 0)
        {
            return output;
        }

        var value = NextUInt64() % 0xFFFFFFFF;

        for (var index = 0; index < width; index++)
        {
            var group = (byte)(value & 0x7F);
            value >>= 7;
            output[start + index] = index == width - 1 ? group : (byte)(group | 0x80);
        }

        return output;
    }
}

/// <summary>What one fuzz iteration observed.</summary>
public sealed class FixtureFuzzObservation
{
    /// <summary>Records one iteration.</summary>
    public FixtureFuzzObservation(
        VmOutcome outcome,
        bool producedHandle,
        bool escaped,
        string escapedTypeName,
        ulong reservedBytes,
        ulong allocationCeiling,
        bool policyObserved,
        bool policyPrecededEveryRead,
        bool noReservationPrecededTheFirstRead,
        long elapsedMilliseconds)
    {
        Outcome = outcome;
        ProducedHandle = producedHandle;
        Escaped = escaped;
        EscapedTypeName = escapedTypeName;
        ReservedBytes = reservedBytes;
        AllocationCeiling = allocationCeiling;
        PolicyObserved = policyObserved;
        PolicyPrecededEveryRead = policyPrecededEveryRead;
        NoReservationPrecededTheFirstRead = noReservationPrecededTheFirstRead;
        ElapsedMilliseconds = elapsedMilliseconds;
    }

    /// <summary>The category verification answered with.</summary>
    public VmOutcome Outcome { get; }

    /// <summary>Whether a verified handle came back.</summary>
    public bool ProducedHandle { get; }

    /// <summary>Whether an exception escaped the verification entry point.</summary>
    public bool Escaped { get; }

    /// <summary>The escaping exception's type name, or the empty string.</summary>
    public string EscapedTypeName { get; }

    /// <summary>How many bytes the bounded allocator reserved.</summary>
    public ulong ReservedBytes { get; }

    /// <summary>The allocation ceiling the frozen policy carried.</summary>
    public ulong AllocationCeiling { get; }

    /// <summary>Whether the verifier was entered at all.</summary>
    public bool PolicyObserved { get; }

    /// <summary>Whether the policy was frozen before any byte was read or any allocation reserved.</summary>
    public bool PolicyPrecededEveryRead { get; }

    /// <summary>Whether no allocation was reserved before the first byte was read.</summary>
    public bool NoReservationPrecededTheFirstRead { get; }

    /// <summary>How long the iteration took.</summary>
    public long ElapsedMilliseconds { get; }
}

/// <summary>
/// The properties every input must have, whatever it is. This is what a fuzz session is looking for
/// a counterexample to.
/// </summary>
/// <remarks>
/// They are stated once and used by every driver, so a session run from the fuzz host and a session
/// run inside the behavioural suite are looking for the same thing. A driver that carried its own
/// idea of what counts as a finding would make the two incomparable.
/// </remarks>
public static class FixtureFuzzInvariants
{
    /// <summary>The outcomes the load stage may produce.</summary>
    public static System.Collections.Generic.IReadOnlyList<VmOutcome> LoadStageOutcomes { get; } =
    [
        VmOutcome.Normal,
        VmOutcome.UnsupportedProfile,
        VmOutcome.InvalidArtifact,
        VmOutcome.ResourceExhaustion,
        VmOutcome.Cancellation,
        VmOutcome.InvalidState,
    ];

    /// <summary>Every property <paramref name="observation"/> breaks, named.</summary>
    public static System.Collections.Generic.IReadOnlyList<string> Violations(
        FixtureFuzzObservation observation,
        long millisecondBudget)
    {
        var violations = new System.Collections.Generic.List<string>();

        if (observation.Escaped)
        {
            // An escaping exception is a finding and never a category. Translating one would let a
            // verifier defect masquerade as a malicious artifact, and the corpus could then not tell
            // a null dereference from bytes that were genuinely invalid.
            violations.Add("an exception escaped verification: " + observation.EscapedTypeName);
        }

        var admitted = false;

        foreach (var outcome in LoadStageOutcomes)
        {
            admitted |= outcome == observation.Outcome;
        }

        if (!admitted)
        {
            violations.Add("the load stage answered " + observation.Outcome + ", which it may not produce");
        }

        if (observation.ProducedHandle != (observation.Outcome is VmOutcome.Normal))
        {
            violations.Add(
                "the answer was " + observation.Outcome + " and a handle was " +
                (observation.ProducedHandle ? "produced" : "not produced"));
        }

        if (observation.PolicyObserved && !observation.PolicyPrecededEveryRead)
        {
            violations.Add("a payload byte was read or an allocation reserved before the policy was frozen");
        }

        if (observation.PolicyObserved && !observation.NoReservationPrecededTheFirstRead)
        {
            violations.Add("an allocation was reserved before any payload byte was read");
        }

        if (observation.PolicyObserved && observation.ReservedBytes > observation.AllocationCeiling)
        {
            violations.Add(
                "reserved " + observation.ReservedBytes + " bytes against a ceiling of " +
                observation.AllocationCeiling);
        }

        if (observation.ElapsedMilliseconds > millisecondBudget)
        {
            violations.Add(
                "the iteration took " + observation.ElapsedMilliseconds +
                "ms against a budget of " + millisecondBudget + "ms");
        }

        return violations;
    }
}
