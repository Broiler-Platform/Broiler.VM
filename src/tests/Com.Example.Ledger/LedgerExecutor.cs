using Broiler.VM;

namespace Com.Example.Ledger;

/// <summary>The ledger profile's answer: one account's balance, and the host's stamp on it.</summary>
/// <remarks>
/// The core never names this type, never calls a member on it, and inspects nothing about it except
/// the identity every payload carries. A consumer gets the concrete type back through this profile's
/// own static accessor, which is the projection shape the contract specifies.
/// </remarks>
public sealed class LedgerBalance : IVmProfilePayload
{
    internal LedgerBalance(VmProfileId profileId, string account, long balance, long stamp, bool stamped)
    {
        Identity = new VmPayloadIdentity(profileId, LedgerProfile.BalanceKindId, 1);
        Account = account;
        Balance = balance;
        Stamp = stamp;
        IsStamped = stamped;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>The account the caller named.</summary>
    public string Account { get; }

    /// <summary>Its balance after every posting against it.</summary>
    public long Balance { get; }

    /// <summary>What the host's stamping capability answered, or zero when it was not bound.</summary>
    public long Stamp { get; }

    /// <summary>Whether a stamping capability was bound at all.</summary>
    public bool IsStamped { get; }
}

/// <summary>The ledger profile's language-defined fault.</summary>
/// <remarks>
/// An unknown account and an overflowing balance are facts about this profile's value model, not
/// about the core. They ride behind the profile-neutral fault category as a typed payload, so they
/// reach a caller in full without the core acquiring a case for either.
/// </remarks>
public sealed class LedgerFault : IVmProfilePayload
{
    internal LedgerFault(VmProfileId profileId, string description)
    {
        Identity = new VmPayloadIdentity(profileId, LedgerProfile.FaultKindId, 1);
        Description = description;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>What this profile calls the fault.</summary>
    public string Description { get; }
}

/// <summary>The ledger profile's mutable per-instance state.</summary>
public sealed class LedgerInstance : IVmInstanceState
{
    internal LedgerInstance(LedgerBook book) => Book = book;

    internal LedgerBook Book { get; }

    /// <summary>How many balances this instance has been asked for.</summary>
    public int InvocationCount { get; internal set; }
}

/// <summary>
/// The ledger profile's per-runtime executor.
/// </summary>
/// <remarks>
/// <para>
/// The entry point is the account name, carried as the caller's UTF-8 bytes. That is the second
/// reason this profile exists: the calculator has one fixed entry point and this one has as many as
/// the artifact declares, so between them the contract's promise about entry-point bytes - that the
/// core carries them verbatim and the profile decides what they mean - is exercised both ways.
/// </para>
/// <para>
/// It calls the host's stamping capability when one is bound and takes its other branch when none
/// is. Both branches are reachable in a shipped composition, and the two composition roots take one
/// each.
/// </para>
/// </remarks>
public sealed class LedgerExecutor : IVmProfileExecutor
{
    private readonly IVmExecutionEnvironment environment;

    internal LedgerExecutor(VmProfileId profileId, IVmExecutionEnvironment executionEnvironment)
    {
        ProfileId = profileId;
        environment = executionEnvironment;
    }

    /// <inheritdoc/>
    public VmProfileId ProfileId { get; }

    /// <inheritdoc/>
    public VmExecutionStep Instantiate(
        VmVerifiedArtifact artifact,
        System.Threading.CancellationToken cancellationToken)
    {
        if (!artifact.TryGetState(out var state) || state is not LedgerBook book)
        {
            // A handle this profile did not produce, or one that has been disposed. Either way it
            // is not something to run, and saying so is a contract violation rather than a fault:
            // no guest program caused it.
            return VmExecutionStep.ContractViolation(VmReason.ForeignHandle);
        }

        if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
        {
            return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
        }

        return VmExecutionStep.Instantiated(new LedgerInstance(book), null);
    }

    /// <inheritdoc/>
    public VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken)
    {
        if (state is not LedgerInstance instance)
        {
            return VmExecutionStep.ContractViolation(VmReason.ForeignPayload);
        }

        instance.InvocationCount++;

        var book = instance.Book;

        if (!book.TryFind(request.EntryPoint.Utf8, out var account))
        {
            // Naming an account this book does not have is a language-level mistake, so it is this
            // profile's fault to define. The core neither knows what an account is nor which of
            // them exist.
            return VmExecutionStep.Faulted(new LedgerFault(ProfileId, "no such account"));
        }

        var name = System.Text.Encoding.UTF8.GetString(book.NameOf(account));
        var balance = book.OpeningBalanceOf(account);

        for (var index = 0; index < book.PostingCount; index++)
        {
            if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))
            {
                return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);
            }

            // One poll per posting, well inside the uncharged-work bound this profile's descriptor
            // declares. A profile that declared a bound and did not poll to it would be promising
            // a cancellation latency it does not keep.
            if (!environment.Meter.Poll())
            {
                return VmExecutionStep.ContractViolation(VmReason.Cancelled);
            }

            var posting = book.PostingAt(index);

            if (posting.AccountIndex != account)
            {
                continue;
            }

            // Checked, because a balance that silently wrapped would answer with a number this
            // profile's value model says is wrong. An out-of-range total is a language fault, and
            // reporting it as one is cheaper and more honest than an exception the core would have
            // to translate.
            var moved = balance + posting.Delta;

            if (((balance ^ moved) & (posting.Delta ^ moved)) < 0)
            {
                return VmExecutionStep.Faulted(new LedgerFault(ProfileId, "balance is out of range"));
            }

            balance = moved;
        }

        return Answer(name, balance);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This profile never suspends, so a resume can only be the core handing back a continuation it
    /// was never given. Answering with a contract violation rather than pretending to resume keeps a
    /// defect in the core or in this profile from being reported as a completed operation.
    /// </remarks>
    public VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        System.Threading.CancellationToken cancellationToken) =>
        VmExecutionStep.ContractViolation(VmReason.ProfileContractViolation);

    /// <inheritdoc/>
    /// <remarks>Nothing to unwind: this profile parks nowhere and holds no resource across a step.</remarks>
    public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance)
    {
    }

    /// <summary>
    /// Stamps the balance if the host bound a stamping capability, and answers unstamped if it did
    /// not.
    /// </summary>
    /// <remarks>
    /// <see cref="IVmHostCapabilityInvoker.IsBound"/> is the whole of what this profile may ask
    /// about the capability table: whether slot <em>k</em> is bound. It cannot enumerate the
    /// registered set, resolve a capability by name, or learn anything about what is on the other
    /// side - which is what keeps a capability table from becoming an ambient platform surface.
    /// </remarks>
    private VmExecutionStep Answer(string name, long balance)
    {
        if (!environment.Capabilities.IsBound(LedgerCapabilities.StampBinding))
        {
            return VmExecutionStep.Completed(
                new LedgerBalance(ProfileId, name, balance, stamp: 0, stamped: false));
        }

        System.Span<long> arguments = stackalloc long[1];
        arguments[0] = balance;

        var outcome = environment.Capabilities.Invoke(
            LedgerCapabilities.StampBinding, arguments, out var stamp);

        if (outcome is not VmHostCallOutcome.Completed)
        {
            // A refusal and an unavailability are the host's answers, not failures of this profile.
            // Reporting them as a fault leaves the caller with something it can act on, and leaves
            // the instance recoverable.
            return VmExecutionStep.Faulted(new LedgerFault(ProfileId, "the stamping host declined"));
        }

        return VmExecutionStep.Completed(
            new LedgerBalance(ProfileId, name, balance, stamp, stamped: true));
    }
}
