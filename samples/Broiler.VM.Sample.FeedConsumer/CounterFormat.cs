using Broiler.VM;
using System.Collections.Generic;

namespace Broiler.VM.Sample.FeedConsumer;

/// <summary>
/// The counter format, and the writer that produces one.
/// </summary>
/// <remarks>
/// Four bytes of magic, a format version, a starting value and a step count. It is about as small
/// as a format can be while still having a version and a declared count in it, and both of those
/// are there because they are the two things a verifier must handle carefully: a version it does
/// not know, and a count an attacker chose.
/// </remarks>
internal static class CounterFormat
{
    /// <summary>The magic, which is checked before anything else is read.</summary>
    internal static ReadOnlySpan<byte> Magic => "BVMC"u8;

    internal const uint FormatVersion = 1;

    /// <summary>The profile's own diagnostic codes, which are its to define.</summary>
    internal const int NotThisFormat = 9001;

    internal const int UnknownVersion = 9002;

    internal const int DescriptorDisagrees = 9003;

    internal const int TrailingBytes = 9004;

    /// <summary>Writes an artifact that starts at <paramref name="start"/> and adds one <paramref name="steps"/> times.</summary>
    internal static byte[] Write(long start, uint steps)
    {
        var bytes = new List<byte>();

        bytes.AddRange(Magic.ToArray());
        WriteVarUInt64(bytes, FormatVersion);
        WriteVarUInt64(bytes, unchecked((ulong)start));
        WriteVarUInt64(bytes, steps);

        return bytes.ToArray();
    }

    /// <summary>
    /// LEB128, canonical form, matching what the core's bounded reader accepts.
    /// </summary>
    /// <remarks>
    /// The reader rejects over-long encodings, so a writer that padded a value would produce bytes
    /// its own verifier refuses. That is the intended direction of the asymmetry: the reader is
    /// the authority and the writer conforms to it, rather than the format being whatever the
    /// writer happens to emit.
    /// </remarks>
    private static void WriteVarUInt64(List<byte> bytes, ulong value)
    {
        while (true)
        {
            var septet = (byte)(value & 0x7F);
            value >>= 7;

            if (value == 0)
            {
                bytes.Add(septet);
                return;
            }

            bytes.Add((byte)(septet | 0x80));
        }
    }
}

/// <summary>What a counter artifact decodes to, and what an instance of one holds.</summary>
/// <remarks>
/// One type for both, which a profile is free to do: the verified state is immutable and shareable
/// across runtimes, and this profile's instance state adds nothing to it, so there is nothing to
/// separate. A profile whose instances held mutable memory would need two types, and the core's
/// two interfaces are what say so.
/// </remarks>
internal sealed class CounterState : IVmVerifiedState, IVmInstanceState
{
    internal CounterState(long start, uint steps)
    {
        Start = start;
        Steps = steps;
    }

    internal long Start { get; }

    internal uint Steps { get; }
}

/// <summary>The typed payload a completed invocation carries back.</summary>
/// <remarks>
/// The core never looks inside it. It travels through the profile-neutral result envelope as an
/// <see cref="IVmProfilePayload"/> and comes back out through this profile's own projection, which
/// checks the payload identity before it casts - so a payload minted by another profile that
/// happened to use the same CLR type is refused rather than reinterpreted.
/// </remarks>
internal sealed class CounterValue : IVmProfilePayload
{
    internal CounterValue(long value) => Value = value;

    internal long Value { get; }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; } =
        new(CounterProfile.Id, CounterProfile.ValueKindId, payloadSchemaVersion: 1);
}
