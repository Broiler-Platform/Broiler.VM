namespace Broiler.VM;

/// <summary>
/// The host-facing view of one catalog entry.
/// </summary>
/// <remarks>
/// Exactly the fields a host may see: identity, support ranges, both contract integers, the three
/// capability declarations, conformance and diagnostics identity, and the package ID. It carries no
/// verifier, no factory, no limits and no capability descriptors, because a listing is for support
/// tables and diagnostics, not for reaching into a profile.
/// </remarks>
public readonly struct VmProfileCatalogEntry
{
    internal VmProfileCatalogEntry(VmProfileDescriptor descriptor)
    {
        ProfileId = descriptor.ProfileId;
        DisplayName = descriptor.DisplayName;
        SupportedFormatVersions = descriptor.SupportedFormatVersions;
        AcceptedFeatureManifests = descriptor.AcceptedFeatureManifests;
        BuiltAgainstCoreContractVersion = descriptor.BuiltAgainstCoreContractVersion;
        AuthoredCoreContractVersion = descriptor.AuthoredCoreContractVersion;
        GuestInitiatedLoads = descriptor.GuestInitiatedLoads.Kind;
        AsynchronousInstantiation = descriptor.AsynchronousInstantiation;
        ExternalSuspension = descriptor.ExternalSuspension;
        ConformanceManifestId = descriptor.ConformanceManifestId;
        ConformanceManifestVersion = descriptor.ConformanceManifestVersion;
        DiagnosticsIdentity = descriptor.DiagnosticsIdentity;
        PackageId = descriptor.PackageIdentity.PackageId;
    }

    /// <summary>The profile's identity.</summary>
    public VmProfileId ProfileId { get; }

    /// <summary>Its display name, echoed and never compared.</summary>
    public string DisplayName { get; }

    /// <summary>The profile-format versions it accepts.</summary>
    public VmFormatVersionRange SupportedFormatVersions { get; }

    /// <summary>The feature manifests it accepts, in normalized order.</summary>
    public System.Collections.Immutable.ImmutableArray<VmFeatureManifestId> AcceptedFeatureManifests { get; }

    /// <summary>The contract version it was compiled against.</summary>
    public int BuiltAgainstCoreContractVersion { get; }

    /// <summary>The contract version its author wrote for.</summary>
    public int AuthoredCoreContractVersion { get; }

    /// <summary>Whether it may request code while executing.</summary>
    public VmDeclaration GuestInitiatedLoads { get; }

    /// <summary>Whether its instantiation may suspend.</summary>
    public VmDeclaration AsynchronousInstantiation { get; }

    /// <summary>Whether the host may suspend it from outside.</summary>
    public VmDeclaration ExternalSuspension { get; }

    /// <summary>Its conformance corpus identity.</summary>
    public VmConformanceManifestId ConformanceManifestId { get; }

    /// <summary>That corpus's version.</summary>
    public int ConformanceManifestVersion { get; }

    /// <summary>Its diagnostics token.</summary>
    public VmDiagnosticsIdentity DiagnosticsIdentity { get; }

    /// <summary>The package it ships in.</summary>
    public string PackageId { get; }
}

/// <summary>The host-facing enumeration of a catalog's contents, always in normalized order.</summary>
/// <remarks>
/// Reachable only from <see cref="VmCatalog.GetListing"/>. No result type and no diagnostics member
/// is typed as this or as any collection of catalog entries, so the disclosure split - a host may
/// see what a composition contains, a guest may not - is enforced by type rather than by a flag,
/// and there is no verbose-diagnostics option that could turn it back on.
/// </remarks>
public readonly struct VmProfileCatalogListing
{
    internal VmProfileCatalogListing(System.Collections.Immutable.ImmutableArray<VmProfileCatalogEntry> entries) =>
        Entries = entries;

    /// <summary>How many entries the catalog holds.</summary>
    public int Count => Entries.IsDefault ? 0 : Entries.Length;

    /// <summary>The entries, in ascending ordinal identity order.</summary>
    public System.Collections.Immutable.ImmutableArray<VmProfileCatalogEntry> Entries { get; }

    /// <summary>One entry, by position in the normalized order.</summary>
    public VmProfileCatalogEntry this[int index] => Entries[index];
}

/// <summary>
/// The canonical encoding of a catalog: its identity oracle.
/// </summary>
/// <remarks>
/// <para>
/// The encoding walks entries in ascending ordinal identity order and emits, per entry and in
/// descriptor-row order, each string as its bytes preceded by a four-byte little-endian length, each
/// integer as four little-endian bytes, each declaration as one byte, and each set preceded by a
/// four-byte little-endian count - with no field names, separators, whitespace or culture anywhere.
/// </para>
/// <para>
/// It exists so that "declaration order has no observable effect" is a byte-level, testable property
/// rather than a promise: two catalogs built from the same descriptors in different orders encode
/// to identical bytes. Rows that are tunable policy, or that are inert metadata, are excluded, so a
/// host retuning a limit does not appear to have changed what the catalog <em>is</em>.
/// </para>
/// </remarks>
public sealed class VmCatalogIdentity : System.IEquatable<VmCatalogIdentity>
{
    private readonly byte[] encoding;

    internal VmCatalogIdentity(byte[] encoding) => this.encoding = encoding;

    /// <summary>How many bytes the canonical encoding occupies.</summary>
    public int EncodedLength => encoding.Length;

    /// <summary>Copies the canonical encoding out.</summary>
    public void CopyEncodingTo(System.Span<byte> destination) =>
        System.MemoryExtensions.AsSpan(encoding).CopyTo(destination);

    /// <inheritdoc/>
    public bool Equals(VmCatalogIdentity? other) =>
        other is not null &&
        System.MemoryExtensions.SequenceEqual(
            System.MemoryExtensions.AsSpan(encoding),
            System.MemoryExtensions.AsSpan(other.encoding));

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as VmCatalogIdentity);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        // FNV-1a over the canonical bytes. It is a bucketing hash, not an authentication tag, and
        // the encoding itself stays the identity: two catalogs are equal when their bytes are.
        var hash = 2166136261u;

        foreach (var value in encoding)
        {
            hash = (hash ^ value) * 16777619u;
        }

        return unchecked((int)hash);
    }
}

/// <summary>
/// The mutable, single-use builder a composition root uses to author a catalog.
/// </summary>
/// <remarks>
/// <para>
/// Single-use by design. A builder that could be built twice would let a composition root hand two
/// catalogs to two runtimes believing they were the same, and the second <c>Build</c> would be the
/// only place that was ever true. Any call on a consumed builder throws.
/// </para>
/// <para>
/// Single-descriptor rules are enforced eagerly at <see cref="Add"/> so the exception's stack names
/// the offending registration call. Cross-entry rules run at <see cref="Add"/> against the entries
/// accepted so far and are re-validated at <see cref="Build"/>, which additionally enforces the
/// set-level rules.
/// </para>
/// </remarks>
public sealed class VmCatalogBuilder
{
    /// <summary>The most entries one catalog may hold.</summary>
    public const int MaximumEntries = 64;

    private readonly System.Collections.Generic.List<VmProfileDescriptor> entries = new();
    private bool consumed;

    internal VmCatalogBuilder()
    {
    }

    /// <summary>
    /// Registers one profile by its descriptor. Fluent, returning the same builder, so a
    /// composition root reads as the list of profiles it contains.
    /// </summary>
    /// <exception cref="VmCatalogValidationException">The descriptor is not admissible.</exception>
    public VmCatalogBuilder Add(VmProfileDescriptor descriptor)
    {
        if (consumed)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.BuilderConsumed, entries.Count, nameof(Add));
        }

        if (descriptor is null)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.DescriptorMissing, entries.Count, nameof(descriptor));
        }

        if (entries.Count >= MaximumEntries)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.CatalogTooLarge, descriptor.ProfileId, nameof(MaximumEntries));
        }

        VmDescriptorValidation.ValidateSingle(descriptor, entries.Count);
        VmDescriptorValidation.ValidateAgainstAccepted(descriptor, entries);

        entries.Add(descriptor);
        return this;
    }

    /// <summary>Builds the immutable catalog and consumes the builder.</summary>
    /// <exception cref="VmCatalogValidationException">A set-level rule is violated.</exception>
    public VmCatalog Build()
    {
        if (consumed)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.BuilderConsumed, entries.Count, nameof(Build));
        }

        // Re-validated at Build as well as at Add. The two runs are not redundant: Add sees a
        // partial set, and a rule that depends on the whole set can only be decided here.
        for (var index = 0; index < entries.Count; index++)
        {
            VmDescriptorValidation.ValidateSingle(entries[index], index);
        }

        for (var index = 0; index < entries.Count; index++)
        {
            VmDescriptorValidation.ValidateAgainstAccepted(entries[index], entries.GetRange(0, index));
        }

        consumed = true;

        var ordered = entries.ToArray();
        System.Array.Sort(ordered, static (left, right) => left.ProfileId.CompareTo(right.ProfileId));

        return new VmCatalog(ordered);
    }
}

/// <summary>
/// The immutable, free-threaded set of VM profiles one composition contains.
/// </summary>
/// <remarks>
/// <para>
/// Exactly one state, no terminal state, not disposable. It outlives every runtime built from it
/// and is owned by nobody, which is what lets several runtimes share one without a lifetime
/// question between them.
/// </para>
/// <para>
/// It has <strong>no</strong> mutating member - no add, remove, clear, replace or overwriting
/// try-add, no priority and no enabled flag - and no indexer, index-based accessor, or
/// first/default/primary/only entry concept, and it does not behave differently when it holds one
/// entry. A single-profile composition and a two-profile composition differ in what they contain
/// and in nothing else.
/// </para>
/// <para>
/// An empty catalog is legal, and every verification against it returns
/// <see cref="VmOutcome.UnsupportedProfile"/> - which is the truthful answer for a composition that
/// hosts nothing, and a much better diagnostic than a corrupt-file report.
/// </para>
/// </remarks>
public sealed class VmCatalog
{
    private readonly VmProfileDescriptor[] entries;
    private readonly VmProfileCatalogListing listing;

    internal VmCatalog(VmProfileDescriptor[] orderedEntries)
    {
        entries = orderedEntries;

        var built = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmProfileCatalogEntry>(orderedEntries.Length);

        foreach (var descriptor in orderedEntries)
        {
            built.Add(new VmProfileCatalogEntry(descriptor));
        }

        listing = new VmProfileCatalogListing(built.ToImmutable());
        Identity = new VmCatalogIdentity(VmCanonicalCatalogEncoding.Encode(orderedEntries));
    }

    /// <summary>Starts a new builder.</summary>
    public static VmCatalogBuilder CreateBuilder() => new();

    /// <summary>How many profiles this catalog contains.</summary>
    public int Count => entries.Length;

    /// <summary>The canonical encoding of this catalog, used as its identity.</summary>
    public VmCatalogIdentity Identity { get; }

    /// <summary>
    /// Looks up one profile by exact ordinal identity. There is no folded lookup: folding is the
    /// uniqueness rule, and applying it here would let two spellings select one entry.
    /// </summary>
    public bool TryGetEntry(VmProfileId profileId, out VmProfileCatalogEntry entry)
    {
        for (var index = 0; index < entries.Length; index++)
        {
            if (entries[index].ProfileId.Equals(profileId))
            {
                entry = listing.Entries[index];
                return true;
            }
        }

        entry = default;
        return false;
    }

    /// <summary>
    /// The host-facing listing. This is the only route to a catalog's contents: a result that names
    /// an unsupported profile carries the requested ID and never the catalog.
    /// </summary>
    public VmProfileCatalogListing GetListing() => listing;

    internal bool TryGetDescriptor(VmProfileId profileId, out VmProfileDescriptor descriptor)
    {
        for (var index = 0; index < entries.Length; index++)
        {
            if (entries[index].ProfileId.Equals(profileId))
            {
                descriptor = entries[index];
                return true;
            }
        }

        descriptor = null!;
        return false;
    }

    internal System.Collections.Generic.IReadOnlyList<VmProfileDescriptor> Descriptors => entries;
}
