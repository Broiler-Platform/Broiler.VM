using Broiler.VM;

namespace Com.Example.Ledger;

/// <summary>One account, as a fixed-size record into the book's single name blob.</summary>
/// <remarks>
/// A record rather than an object, and a slice rather than a string, so the whole account table is
/// one <see cref="VmBoundedAllocator"/> call of a size the artifact declared and the reader bounded.
/// A per-account object would put the allocation count under the artifact's control, which is the
/// thing an artifact is least entitled to decide.
/// </remarks>
public readonly struct LedgerAccountRecord
{
    internal LedgerAccountRecord(int nameOffset, int nameLength, long openingBalance)
    {
        NameOffset = nameOffset;
        NameLength = nameLength;
        OpeningBalance = openingBalance;
    }

    /// <summary>Where this account's name starts in the book's name blob.</summary>
    public int NameOffset { get; }

    /// <summary>How long the name is, in UTF-8 bytes.</summary>
    public int NameLength { get; }

    /// <summary>What the account opened at.</summary>
    public long OpeningBalance { get; }
}

/// <summary>One posting: a delta against one account.</summary>
public readonly struct LedgerPosting
{
    internal LedgerPosting(int accountIndex, long delta)
    {
        AccountIndex = accountIndex;
        Delta = delta;
    }

    /// <summary>Which account it moves, as an index verification already proved in range.</summary>
    public int AccountIndex { get; }

    /// <summary>By how much.</summary>
    public long Delta { get; }
}

/// <summary>The ledger profile's immutable decoded artifact.</summary>
/// <remarks>
/// Everything reachable from it is immutable once verification returns, which is what makes the
/// handle safe for unsynchronised concurrent readers in two runtimes at once. The account names are
/// in ascending byte order, checked at verification, so a lookup at execution is a binary search
/// over data proved sorted rather than a scan trusting that it is.
/// </remarks>
public sealed class LedgerBook : IVmVerifiedState
{
    private readonly byte[] names;
    private readonly LedgerAccountRecord[] accounts;
    private readonly LedgerPosting[] postings;

    internal LedgerBook(byte[] names, LedgerAccountRecord[] accounts, LedgerPosting[] postings)
    {
        this.names = names;
        this.accounts = accounts;
        this.postings = postings;
    }

    /// <summary>How many accounts the book declares.</summary>
    public int AccountCount => accounts.Length;

    /// <summary>How many postings it declares.</summary>
    public int PostingCount => postings.Length;

    /// <summary>The name of account <paramref name="index"/>, as the UTF-8 bytes the artifact carried.</summary>
    public System.ReadOnlySpan<byte> NameOf(int index)
    {
        var account = accounts[index];
        return System.MemoryExtensions.AsSpan(names, account.NameOffset, account.NameLength);
    }

    /// <summary>What account <paramref name="index"/> opened at.</summary>
    public long OpeningBalanceOf(int index) => accounts[index].OpeningBalance;

    internal LedgerPosting PostingAt(int index) => postings[index];

    /// <summary>
    /// Finds the account named <paramref name="name"/>, or returns <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// A binary search, admissible only because verification proved the names strictly ascending.
    /// That proof is why a hostile artifact cannot turn every lookup into a linear scan of a
    /// thousand equal names.
    /// </remarks>
    internal bool TryFind(System.ReadOnlySpan<byte> name, out int index)
    {
        var low = 0;
        var high = accounts.Length - 1;

        while (low <= high)
        {
            var middle = low + ((high - low) / 2);
            var order = System.MemoryExtensions.SequenceCompareTo(NameOf(middle), name);

            if (order == 0)
            {
                index = middle;
                return true;
            }

            if (order < 0)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        index = -1;
        return false;
    }
}

/// <summary>
/// The ledger profile's verifier: a framed decoder over its own format, driven through the core's
/// bounded reader.
/// </summary>
/// <remarks>
/// <para>
/// Like the calculator's, it is written against the public source contract alone and reaches nothing
/// in <c>Broiler.VM.Runtime</c>. Unlike the calculator's, it frames: each of the two regions is
/// entered as a section whose declared length must be consumed exactly, so an artifact that lies
/// about a region's size is refused at the frame rather than absorbed by the next read. Between the
/// two consumer profiles, both halves of the bounded-reading surface are exercised by something
/// outside this repository's own fixtures.
/// </para>
/// <para>
/// It also proves an ordering property - names strictly ascending - that the executor then relies
/// on. That is the shape of the boundary: what execution assumes, verification establishes.
/// </para>
/// </remarks>
public sealed class LedgerVerifier : IVmProfileVerifier
{
    /// <summary>Creates the verifier for <paramref name="profileId"/>.</summary>
    public LedgerVerifier(VmProfileId profileId) => ProfileId = profileId;

    /// <inheritdoc/>
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    public int BuiltAgainstCoreContractVersion => VmCoreContract.Version;

    /// <inheritdoc/>
    public int AuthoredCoreContractVersion => 1;

    /// <inheritdoc/>
    public int VerifierSemanticVersion => 1;

    /// <inheritdoc/>
    public VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken)
    {
        var meter = new LedgerReadAdapter(context.Meter);
        var bounds = LedgerReadAdapter.ToReadBounds(context.Ceilings.VerificationCeilings);
        var reader = new VmBoundedReader(payload, in bounds, meter);

        if (!reader.TryReadBytes(4, out var magic))
        {
            return Failed(ref reader, 0);
        }

        if (!System.MemoryExtensions.SequenceEqual(magic, LedgerFormat.Magic))
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.MalformedEncoding, DiagnosticCodes.WrongMagic, At(0));
        }

        if (!reader.TryReadVarUInt32(out var formatVersion))
        {
            return Failed(ref reader, reader.Position);
        }

        if (formatVersion != LedgerFormat.FormatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.UnknownFormatVersion, DiagnosticCodes.UnknownFormatVersion, At(reader.Position));
        }

        // The descriptor and the payload must agree. A caller that mislabels bytes gets this
        // profile's deterministic validation failure rather than a search for a decoder that
        // accepts them.
        if (descriptor.FormatVersion != formatVersion)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.DescriptorMismatch, DiagnosticCodes.DescriptorMismatch, At(reader.Position));
        }

        var failure = ReadAccounts(ref reader, meter, in bounds, out var names, out var accounts);

        if (failure.HasValue)
        {
            return failure.Value;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return VmVerifierOutcome.Cancellation();
        }

        failure = ReadPostings(ref reader, meter, in bounds, accounts.Length, out var postings);

        if (failure.HasValue)
        {
            return failure.Value;
        }

        if (reader.Remaining != 0)
        {
            // Trailing bytes are a structural error and not something to ignore. Ignoring them
            // would let one artifact carry content this verifier never looked at.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, DiagnosticCodes.TrailingBytes, At(reader.Position));
        }

        return VmVerifierOutcome.Verified(
            new LedgerBook(names, accounts, postings), VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// Reads the account section: one contiguous name blob, then one fixed-size record per account.
    /// </summary>
    /// <remarks>
    /// Two allocations for any number of accounts, both of a size the artifact declared before the
    /// reader would admit it. The alternative - an object per account - would let the artifact
    /// choose how many allocations verification performs, which is a resource decision an artifact
    /// does not get to make.
    /// </remarks>
    private static VmVerifierOutcome? ReadAccounts(
        ref VmBoundedReader reader,
        LedgerReadAdapter meter,
        in VmReadBounds bounds,
        out byte[] names,
        out LedgerAccountRecord[] accounts)
    {
        names = System.Array.Empty<byte>();
        accounts = System.Array.Empty<LedgerAccountRecord>();

        if (!reader.TryReadVarUInt64(out var sectionLength))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!reader.TryEnterSection(sectionLength, out var frame))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!reader.TryReadDeclaredCount(out var accountCount))
        {
            return Failed(ref reader, reader.Position);
        }

        if (accountCount > LedgerFormat.MaximumAccountCount)
        {
            // The profile's own structural ceiling, distinct from the host's declared-count ceiling
            // the reader already applied. A ledger of a million accounts is not a resource question
            // for this profile; it is not a ledger this profile reads at all.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.SemanticValidationFailed, DiagnosticCodes.TooManyAccounts, At(reader.Position));
        }

        if (!reader.TryReadDeclaredCount(out var totalNameBytes))
        {
            return Failed(ref reader, reader.Position);
        }

        if (totalNameBytes > accountCount * (ulong)LedgerFormat.MaximumNameLength)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, DiagnosticCodes.NameBlobTooLong, At(reader.Position));
        }

        if (!reader.TryReadBytes(totalNameBytes, out var blob))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!VmBoundedAllocator.TryAllocate<byte>(in bounds, meter, totalNameBytes, out var nameBuffer))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        blob.CopyTo(nameBuffer);

        if (!VmBoundedAllocator.TryAllocate<LedgerAccountRecord>(
                in bounds, meter, accountCount, out var records))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        var offset = 0;

        for (var index = 0; index < accountCount; index++)
        {
            if (!reader.TryReadDeclaredCount(out var nameLength))
            {
                return Failed(ref reader, reader.Position);
            }

            if (nameLength == 0 ||
                nameLength > LedgerFormat.MaximumNameLength ||
                nameLength > totalNameBytes - (ulong)offset)
            {
                return VmVerifierOutcome.InvalidArtifact(
                    VmReason.SemanticValidationFailed, DiagnosticCodes.BadName, At(reader.Position));
            }

            if (index > 0 &&
                System.MemoryExtensions.SequenceCompareTo(
                    System.MemoryExtensions.AsSpan(nameBuffer, records[index - 1].NameOffset, records[index - 1].NameLength),
                    System.MemoryExtensions.AsSpan(nameBuffer, offset, (int)nameLength)) >= 0)
            {
                // Strictly ascending, so a duplicate name is unrepresentable rather than merely
                // discouraged, and the executor's binary search has the property it needs.
                return VmVerifierOutcome.InvalidArtifact(
                    VmReason.InconsistentStructure, DiagnosticCodes.NamesOutOfOrder, At(reader.Position));
            }

            if (!reader.TryReadVarUInt64(out var encoded))
            {
                return Failed(ref reader, reader.Position);
            }

            records[index] = new LedgerAccountRecord(offset, (int)nameLength, Decode(encoded));
            offset += (int)nameLength;
        }

        if ((ulong)offset != totalNameBytes)
        {
            // The blob and the records disagree about how much text there is. Accepting the
            // shorter reading would leave bytes in the artifact this verifier never attributed to
            // anything.
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, DiagnosticCodes.NameBlobUnconsumed, At(reader.Position));
        }

        if (!reader.TryExitSection(in frame))
        {
            // The section declared a length the accounts did not fill, or overran it. Either way
            // the artifact and this verifier disagree about where the postings begin.
            return Failed(ref reader, reader.Position);
        }

        if (!reader.TryChargeWork(accountCount))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
        }

        names = nameBuffer;
        accounts = records;
        return null;
    }

    private static VmVerifierOutcome? ReadPostings(
        ref VmBoundedReader reader,
        LedgerReadAdapter meter,
        in VmReadBounds bounds,
        int accountCount,
        out LedgerPosting[] postings)
    {
        postings = System.Array.Empty<LedgerPosting>();

        if (!reader.TryReadVarUInt64(out var sectionLength))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!reader.TryEnterSection(sectionLength, out var frame))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!reader.TryReadDeclaredCount(out var postingCount))
        {
            return Failed(ref reader, reader.Position);
        }

        if (postingCount > LedgerFormat.MaximumPostingCount)
        {
            return VmVerifierOutcome.InvalidArtifact(
                VmReason.SemanticValidationFailed, DiagnosticCodes.TooManyPostings, At(reader.Position));
        }

        if (!VmBoundedAllocator.TryAllocate<LedgerPosting>(in bounds, meter, postingCount, out var buffer))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);
        }

        for (var index = 0; index < postingCount; index++)
        {
            if (!reader.TryReadVarUInt32(out var accountIndex))
            {
                return Failed(ref reader, reader.Position);
            }

            if (accountIndex >= (uint)accountCount)
            {
                // An index into an account that does not exist. Refusing it here rather than at
                // execution is what lets the executor index the table without a bounds decision of
                // its own on every posting.
                return VmVerifierOutcome.InvalidArtifact(
                    VmReason.SemanticValidationFailed, DiagnosticCodes.UnknownAccountIndex, At(reader.Position));
            }

            if (!reader.TryReadVarUInt64(out var encoded))
            {
                return Failed(ref reader, reader.Position);
            }

            buffer[index] = new LedgerPosting((int)accountIndex, Decode(encoded));
        }

        if (!reader.TryExitSection(in frame))
        {
            return Failed(ref reader, reader.Position);
        }

        if (!reader.TryChargeWork(postingCount))
        {
            return VmVerifierOutcome.ResourceExhaustion(
                VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact);
        }

        postings = buffer;
        return null;
    }

    /// <summary>Undoes the zigzag encoding a signed amount is written in.</summary>
    private static long Decode(ulong encoded) => (long)(encoded >> 1) ^ -(long)(encoded & 1);

    /// <summary>Maps the reader's mechanism status onto the answers a verifier may give.</summary>
    /// <remarks>
    /// The same shape the calculator writes, and deliberately not shared with it. Two profiles that
    /// shared this mapping would be sharing semantics through a common assembly, which is the
    /// arrangement the whole boundary exists to prevent; the price of not sharing it is these few
    /// lines, once per profile.
    /// </remarks>
    private static VmVerifierOutcome Failed(ref VmBoundedReader reader, ulong position) =>
        reader.Status switch
        {
            VmBoundedReadStatus.Truncated =>
                VmVerifierOutcome.InvalidArtifact(VmReason.Truncated, DiagnosticCodes.Truncated, At(position)),

            VmBoundedReadStatus.MalformedEncoding =>
                VmVerifierOutcome.InvalidArtifact(VmReason.MalformedEncoding, DiagnosticCodes.MalformedEncoding, At(position)),

            VmBoundedReadStatus.DeclaredCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.DeclaredCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.SectionCountExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.SectionCount, VmBudgetScope.Artifact),

            VmBoundedReadStatus.StructuralDepthExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.StructuralDepth, VmBudgetScope.Artifact),

            VmBoundedReadStatus.ArtifactBytesExceeded =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.ArtifactBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.AllocationRefused =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact),

            VmBoundedReadStatus.WorkBudgetExhausted =>
                VmVerifierOutcome.ResourceExhaustion(VmBudgetDimension.VerifierWork, VmBudgetScope.Artifact),

            _ => VmVerifierOutcome.InvalidArtifact(
                VmReason.InconsistentStructure, DiagnosticCodes.ReaderStopped, At(position)),
        };

    private static VmSourcePosition At(ulong offset) => new(-1, offset, 0, 0);

    /// <summary>This profile's own stable diagnostic codes. The core attaches no meaning to them.</summary>
    internal static class DiagnosticCodes
    {
        internal const int WrongMagic = 8001;
        internal const int UnknownFormatVersion = 8002;
        internal const int DescriptorMismatch = 8003;
        internal const int TrailingBytes = 8004;
        internal const int TooManyAccounts = 8005;
        internal const int TooManyPostings = 8006;
        internal const int BadName = 8007;
        internal const int NamesOutOfOrder = 8008;
        internal const int UnknownAccountIndex = 8009;
        internal const int NameBlobTooLong = 8010;
        internal const int NameBlobUnconsumed = 8011;
        internal const int Truncated = 8101;
        internal const int MalformedEncoding = 8102;
        internal const int ReaderStopped = 8103;
    }
}

/// <summary>
/// The projection between the contract's metering surface and the bounded reader's, written once
/// more because <c>Broiler.VM.Binary</c> names no contract vocabulary and the party holding both is
/// the one that performs the projection.
/// </summary>
public sealed class LedgerReadAdapter : IVmBoundedAllocationMeter
{
    private readonly IVmMeter meter;

    /// <summary>Wraps the contract meter the core supplied.</summary>
    public LedgerReadAdapter(IVmMeter contractMeter) => meter = contractMeter;

    /// <summary>Projects the four artifact-shaped ceilings out of an effective limit vector.</summary>
    public static VmReadBounds ToReadBounds(VmLimitVector limits) =>
        new(
            limits[VmBudgetDimension.ArtifactBytes],
            limits[VmBudgetDimension.SectionCount],
            limits[VmBudgetDimension.DeclaredCount],
            limits[VmBudgetDimension.StructuralDepth]);

    /// <inheritdoc/>
    public bool TryReserve(ulong byteCount) =>
        meter.TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    public void Release(ulong byteCount) =>
        meter.ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    /// <inheritdoc/>
    public bool TryChargeWork(ulong workUnits) =>
        meter.TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    /// <inheritdoc/>
    public bool Poll() => meter.Poll();
}
