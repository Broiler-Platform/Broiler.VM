namespace Broiler.VM.Fixtures;

/// <summary>
/// The fixture profile's own projection accessors: how a consumer gets a typed payload back out of
/// a profile-neutral result.
/// </summary>
/// <remarks>
/// <para>
/// This is the shape the contract specifies for payload projection - a static accessor shipped by
/// the profile's own package - and it is why the result types live in the contracts assembly rather
/// than the runtime: a profile package must be able to name them, and a profile package references
/// exactly the contracts and the bounded-reading assemblies, never the runtime.
/// </para>
/// <para>
/// Each accessor checks the full payload identity before casting. Checking only the CLR type would
/// accept a payload minted by a different profile that happened to use the same class, which is the
/// confusion the identity exists to prevent.
/// </para>
/// </remarks>
public static class FixtureVmProfileResults
{
    /// <summary>The value a normal invocation produced.</summary>
    public static bool TryGetValue(in VmInvocationResult result, out FixtureValue value)
    {
        value = null!;

        if (!result.IsSuccess || !OwnsValue(result.PayloadIdentity))
        {
            return false;
        }

        return result.TryGetPayload(out value);
    }

    /// <summary>The value a resumed operation produced.</summary>
    public static bool TryGetValue(in VmResumeResult result, out FixtureValue value)
    {
        value = null!;

        if (!result.IsSuccess || !OwnsValue(result.PayloadIdentity))
        {
            return false;
        }

        return result.TryGetPayload(out value);
    }

    /// <summary>The language-defined fault an invocation produced.</summary>
    public static bool TryGetFault(in VmInvocationResult result, out FixtureFault fault)
    {
        fault = null!;

        if (result.Outcome is not VmOutcome.ProfileFault || !OwnsFault(result.PayloadIdentity))
        {
            return false;
        }

        return result.TryGetPayload(out fault);
    }

    /// <summary>What the profile exposed about a suspended operation.</summary>
    public static bool TryGetSuspensionProjection(
        in VmInvocationResult result,
        out FixtureSuspensionProjection projection)
    {
        projection = null!;

        if (!result.IsSuspended || !OwnsProjection(result.PayloadIdentity))
        {
            return false;
        }

        return result.TryGetPayload(out projection);
    }

    private static bool OwnsValue(VmPayloadIdentity identity) =>
        IsFixtureProfile(identity.ProfileId) &&
        identity.PayloadKindId == FixtureKinds.Value(identity.ProfileId);

    private static bool OwnsFault(VmPayloadIdentity identity) =>
        IsFixtureProfile(identity.ProfileId) &&
        identity.PayloadKindId == FixtureKinds.Fault(identity.ProfileId);

    private static bool OwnsProjection(VmPayloadIdentity identity) =>
        IsFixtureProfile(identity.ProfileId) &&
        identity.PayloadKindId == FixtureKinds.SuspensionProjection(identity.ProfileId);

    private static bool IsFixtureProfile(VmProfileId profileId) =>
        profileId.Equals(FixtureVmProfile.Id) || profileId.Equals(SecondFixtureVmProfile.Id);
}
