namespace Com.Example.Ledger;

/// <summary>
/// Builds ledger artifacts.
/// </summary>
/// <remarks>
/// <para>
/// An encoder belongs beside the decoder that reads it, in the profile's own assembly. Like the
/// calculator's, it uses no Broiler type at all: writing an artifact is not a core operation, and
/// the core sees the bytes for the first time when someone asks it to verify them.
/// </para>
/// <para>
/// It sorts the account names, because the format requires them strictly ascending and a writer that
/// left that to its caller would make every well-formed artifact an accident. Writing an unsorted
/// one is still possible - through <see cref="WriteExactly"/> - which is how the ordering refusal
/// gets tested.
/// </para>
/// </remarks>
public static class LedgerArtifactWriter
{
    /// <summary>An artifact with the given accounts and no postings.</summary>
    public static byte[] Opening(params (string Name, long Balance)[] accounts) =>
        Write(accounts, System.Array.Empty<(int, long)>());

    /// <summary>An artifact with accounts and postings against them, sorted into format order.</summary>
    /// <remarks>
    /// The postings are re-indexed onto the sorted account order, so a caller writes the account it
    /// means rather than an index into an ordering the writer chose.
    /// </remarks>
    public static byte[] Write(
        (string Name, long Balance)[] accounts,
        (int AccountIndex, long Delta)[] postings)
    {
        var order = new int[accounts.Length];

        for (var index = 0; index < order.Length; index++)
        {
            order[index] = index;
        }

        System.Array.Sort(
            order,
            (left, right) => string.CompareOrdinal(accounts[left].Name, accounts[right].Name));

        var position = new int[accounts.Length];
        var sorted = new (string Name, long Balance)[accounts.Length];

        for (var index = 0; index < order.Length; index++)
        {
            position[order[index]] = index;
            sorted[index] = accounts[order[index]];
        }

        var moved = new (int AccountIndex, long Delta)[postings.Length];

        for (var index = 0; index < postings.Length; index++)
        {
            var posting = postings[index];
            moved[index] = (position[posting.AccountIndex], posting.Delta);
        }

        return WriteExactly(sorted, moved);
    }

    /// <summary>
    /// Writes an artifact from accounts and postings exactly as given, checking neither.
    /// </summary>
    /// <remarks>
    /// Deliberately unchecked, so a test can write bytes the verifier must refuse: names out of
    /// order, a name of nothing, an index into an account that does not exist. A writer that could
    /// only produce valid artifacts would make every rejection path unreachable.
    /// </remarks>
    public static byte[] WriteExactly(
        (string Name, long Balance)[] accounts,
        (int AccountIndex, long Delta)[] postings)
    {
        var names = new byte[accounts.Length][];
        var totalNameBytes = 0;

        for (var index = 0; index < accounts.Length; index++)
        {
            names[index] = System.Text.Encoding.UTF8.GetBytes(accounts[index].Name);
            totalNameBytes += names[index].Length;
        }

        var accountBody = new System.Collections.Generic.List<byte>(64 + (accounts.Length * 8) + totalNameBytes);
        WriteVarUInt(accountBody, (ulong)accounts.Length);
        WriteVarUInt(accountBody, (ulong)totalNameBytes);

        for (var index = 0; index < accounts.Length; index++)
        {
            accountBody.AddRange(names[index]);
        }

        for (var index = 0; index < accounts.Length; index++)
        {
            WriteVarUInt(accountBody, (ulong)names[index].Length);
            WriteVarUInt(accountBody, Encode(accounts[index].Balance));
        }

        var postingBody = new System.Collections.Generic.List<byte>(16 + (postings.Length * 8));
        WriteVarUInt(postingBody, (ulong)postings.Length);

        for (var index = 0; index < postings.Length; index++)
        {
            WriteVarUInt(postingBody, (ulong)postings[index].AccountIndex);
            WriteVarUInt(postingBody, Encode(postings[index].Delta));
        }

        var buffer = new System.Collections.Generic.List<byte>(
            16 + accountBody.Count + postingBody.Count);

        buffer.AddRange(LedgerFormat.Magic.ToArray());
        WriteVarUInt(buffer, LedgerFormat.FormatVersion);

        // Each region is length-framed, and the verifier enters it as a section that must be
        // consumed exactly. A region whose declared length and content disagree is refused at the
        // frame rather than absorbed by the next read.
        WriteVarUInt(buffer, (ulong)accountBody.Count);
        buffer.AddRange(accountBody);
        WriteVarUInt(buffer, (ulong)postingBody.Count);
        buffer.AddRange(postingBody);

        return buffer.ToArray();
    }

    /// <summary>Zigzags a signed amount, so a small negative costs one byte rather than ten.</summary>
    public static ulong Encode(long value) => (ulong)((value << 1) ^ (value >> 63));

    private static void WriteVarUInt(System.Collections.Generic.List<byte> buffer, ulong value)
    {
        while (value >= 0x80)
        {
            buffer.Add((byte)(value | 0x80));
            value >>= 7;
        }

        buffer.Add((byte)value);
    }
}
