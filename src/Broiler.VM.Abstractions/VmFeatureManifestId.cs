namespace Broiler.VM;

/// <summary>
/// The identity of one feature manifest: the exact language surface accepted by one version of a
/// VM profile.
/// </summary>
/// <remarks>
/// <para>
/// The core fixes identity, cardinality, ordering, comparison and immutability of manifests. It
/// never stores, reads, enumerates, parses or compares manifest <em>content</em>: there is no
/// feature list, no feature flag, no capability bit vector and no member that could hold one.
/// A profile that needs finer granularity than a set of opaque IDs mints more IDs; it does not
/// ask the core for structure, because a structured set of named feature flags would make every
/// new language feature a core data change.
/// </para>
/// <para>
/// The grammar is <see cref="VmProfileId"/>'s with two additions fixed by ADR 0002: a manifest ID
/// must begin with its own profile's ID followed by a dot and at least one further label, and its
/// bounds are 256 characters and at most 12 labels. The namespacing makes manifest IDs globally
/// unique by construction, so no cross-entry uniqueness rule is needed and a manifest can never be
/// claimed by a profile that does not own it.
/// </para>
/// <para>
/// <see cref="StartsWithProfileNamespace"/> is the one structural operation permitted on a
/// manifest ID, ever, and it runs only at catalog construction. Every other use is opaque ordinal
/// equality: no wildcard, no prefix match, no range, no <c>any</c> token, no ordering relation and
/// no notion that one manifest supersedes or implies another. Implication between language
/// surfaces is a language claim the core cannot evaluate.
/// </para>
/// </remarks>
public readonly struct VmFeatureManifestId
    : System.IEquatable<VmFeatureManifestId>, System.IComparable<VmFeatureManifestId>
{
    /// <summary>The most labels a well-formed manifest ID may have.</summary>
    public const int MaximumLabelCount = 12;

    /// <summary>The most characters a well-formed manifest ID may have.</summary>
    public const int MaximumLength = 256;

    /// <summary>
    /// The fewest labels a well-formed manifest ID may have. A manifest ID carries its profile's
    /// two-label minimum plus at least one further label of its own.
    /// </summary>
    public const int MinimumLabelCount = 3;

    private readonly string? text;

    private VmFeatureManifestId(string text) => this.text = text;

    /// <summary>True when this is <see langword="default"/>.</summary>
    public bool IsEmpty => text is null;

    /// <summary>The number of characters in the ID, or zero when empty.</summary>
    public int Length => text?.Length ?? 0;

    /// <summary>The ID as a span, without allocating.</summary>
    public System.ReadOnlySpan<char> AsSpan() => System.MemoryExtensions.AsSpan(text);

    /// <summary>Parses <paramref name="candidate"/>, returning <see langword="false"/> when it does not satisfy the grammar.</summary>
    public static bool TryParse(System.ReadOnlySpan<char> candidate, out VmFeatureManifestId id)
    {
        id = default;

        if (!VmProfileId.TryValidateGrammar(
                candidate,
                MinimumLabelCount,
                MaximumLabelCount,
                VmProfileId.MinimumLength,
                MaximumLength,
                out _))
        {
            return false;
        }

        id = new VmFeatureManifestId(candidate.ToString());
        return true;
    }

    /// <summary>Parses <paramref name="candidate"/> or throws.</summary>
    /// <exception cref="System.ArgumentException">The candidate does not satisfy the grammar.</exception>
    public static VmFeatureManifestId Parse(System.ReadOnlySpan<char> candidate)
    {
        if (!TryParse(candidate, out var id))
        {
            throw new System.ArgumentException(
                "The value is not a well-formed feature-manifest ID: it must satisfy the profile-ID " +
                "grammar with at least three labels, at most twelve labels and at most 256 characters.",
                nameof(candidate));
        }

        return id;
    }

    /// <summary>
    /// The single structural operation: whether this ID lies under <paramref name="profileId"/>,
    /// which is the ID followed by a dot and at least one further label.
    /// </summary>
    /// <remarks>
    /// The comparison is ordinal, not folded. ADR 0002's blanket rule is that only <em>uniqueness</em>
    /// folds ASCII case; this is a namespace-ownership check on the matching side of that split, so
    /// a manifest declared under <c>Broiler.VM.Fixture.Alpha</c> does not belong to a profile
    /// spelled <c>broiler.vm.fixture.alpha</c>.
    /// </remarks>
    public bool StartsWithProfileNamespace(VmProfileId profileId)
    {
        if (text is null || profileId.IsEmpty)
        {
            return false;
        }

        var owner = profileId.AsSpan();
        var self = System.MemoryExtensions.AsSpan(text);

        // Strictly longer, because the manifest must add at least one label of its own: an ID
        // equal to its profile's ID is the profile, not a manifest under it.
        if (self.Length <= owner.Length + 1)
        {
            return false;
        }

        return System.MemoryExtensions.SequenceEqual(self[..owner.Length], owner) &&
            self[owner.Length] == '.';
    }

    /// <summary>Ordinal, case-sensitive, exact equality. There is no other comparison.</summary>
    public bool Equals(VmFeatureManifestId other) =>
        string.Equals(text, other.text, System.StringComparison.Ordinal);

    /// <summary>
    /// Ordinal ordering, used to normalize a declared manifest set into ascending order at catalog
    /// construction so that declaration order is not retained and has no observable effect.
    /// </summary>
    public int CompareTo(VmFeatureManifestId other) =>
        string.CompareOrdinal(text ?? string.Empty, other.text ?? string.Empty);

    /// <summary>The ID verbatim.</summary>
    public override string ToString() => text ?? string.Empty;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmFeatureManifestId other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        text is null ? 0 : string.GetHashCode(text, System.StringComparison.Ordinal);

    /// <summary>Ordinal equality.</summary>
    public static bool operator ==(VmFeatureManifestId left, VmFeatureManifestId right) => left.Equals(right);

    /// <summary>Ordinal inequality.</summary>
    public static bool operator !=(VmFeatureManifestId left, VmFeatureManifestId right) => !left.Equals(right);
}
