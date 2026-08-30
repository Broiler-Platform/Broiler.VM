namespace Com.Example.Ledger;

/// <summary>
/// The ledger profile's artifact format: a header, a framed section of named accounts, and a framed
/// section of postings against them.
/// </summary>
/// <remarks>
/// <para>
/// It is a record format rather than a program format, which is the reason this second consumer
/// profile exists. The calculator reads a flat token stream and never enters a section; this one is
/// framed throughout, so between the two of them a consumer profile exercises both halves of the
/// bounded-reading surface - declared counts and flat windows on one side, section frames and
/// structural depth on the other.
/// </para>
/// <para>
/// Nothing here is a core concept. The core neither knows nor could learn that an account has a
/// name; it carries bytes to this profile's verifier and a typed payload back.
/// </para>
/// </remarks>
public static class LedgerFormat
{
    /// <summary>The four magic bytes every ledger artifact starts with.</summary>
    public static System.ReadOnlySpan<byte> Magic => "LDGR"u8;

    /// <summary>The only profile-format version this verifier accepts.</summary>
    public const uint FormatVersion = 1;

    /// <summary>The longest an account name may be, in UTF-8 bytes.</summary>
    public const int MaximumNameLength = 32;

    /// <summary>The most accounts one ledger may declare.</summary>
    public const int MaximumAccountCount = 1024;

    /// <summary>The most postings one ledger may declare.</summary>
    public const int MaximumPostingCount = 65_536;
}
