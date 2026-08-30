using Broiler.VM;

namespace Com.Example.Ledger;

/// <summary>
/// The one host capability this profile imports, declared as a shape and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// There is no handler here, and the omission is structural rather than stylistic: a registration
/// type lives in <c>Broiler.VM.Runtime</c>, which a profile assembly does not reference and could
/// not reference without giving up the closure this milestone exists to demonstrate. Declaring what
/// shape a capability must have is the profile's job; supplying one is the composition root's. A
/// profile therefore cannot wire itself to a host even by accident.
/// </para>
/// <para>
/// The import is <see cref="VmCapabilityImportKind.Optional"/>. A required import would make a
/// composition that declines to supply a stamping service fail to construct, which would turn a
/// host's policy decision into a configuration error; optional means the same composition runs and
/// this profile takes its unstamped branch. Both branches are reachable, and the two composition
/// roots take one each.
/// </para>
/// </remarks>
public static class LedgerCapabilities
{
    /// <summary>
    /// The stamping capability: given a balance, the host answers with a stamp of its own choosing.
    /// </summary>
    /// <remarks>
    /// Under the documentation-reserved domain, like the profile ID. The core's reserved-namespace
    /// rule is about the first label folding to <c>broiler</c>, and this claims nothing reserved.
    /// </remarks>
    public static VmCapabilityId StampId { get; } = VmCapabilityId.Parse("com.example.ledger.stamp");

    /// <summary>The signature the stamping capability declares.</summary>
    public static VmCapabilitySignatureId StampSignature { get; } =
        VmCapabilitySignatureId.FromCanonicalDescription("(i64)->i64");

    /// <summary>The binding slot the stamping capability occupies: the profile's only one.</summary>
    public const int StampBinding = 0;

    /// <summary>The stamping capability's shape.</summary>
    /// <remarks>
    /// <see cref="VmCapabilityReentrancy.NonReentrant"/> because a stamping service that called back
    /// into the runtime evaluating a ledger would be re-entering an operation mid-flight, and
    /// <see cref="VmExceptionTranslation.TerminateOperation"/> because a host whose stamp throws has
    /// failed, and reporting that to the guest as a fault it could catch and retry would let a guest
    /// drive a broken host in a loop.
    /// </remarks>
    public static VmHostCapabilityDescriptor Stamp { get; } = new(
        StampId,
        version: 1,
        StampSignature,
        VmCapabilityKind.Value,
        VmCapabilityReentrancy.NonReentrant,
        VmCapabilityThreadAffinity.CallerThread,
        VmExceptionTranslation.TerminateOperation);
}
