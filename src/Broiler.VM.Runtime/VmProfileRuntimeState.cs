namespace Broiler.VM;

/// <summary>
/// One profile's per-runtime state: its binding table, its provider, and its lazily created
/// executor.
/// </summary>
/// <remarks>
/// The executor is created at first instantiation rather than at runtime creation, so a composition
/// that links two profiles and only ever runs one never pays for the other's executor. The factory
/// is still rooted by a direct reference, which is what trimming and Native AOT need; laziness
/// changes when it runs, not whether it is reachable.
/// </remarks>
internal sealed class VmProfileRuntimeState
{
    private readonly object gate = new();
    private IVmProfileExecutor? executor;

    internal VmProfileRuntimeState(VmProfileDescriptor descriptor)
    {
        Descriptor = descriptor;
        Bindings = System.Array.Empty<VmCapabilityBinding>();
        Scope = new VmExecutionScope();
    }

    /// <summary>
    /// The per-profile execution scope. It is what makes one per-runtime executor able to charge
    /// whichever operation is currently running through it.
    /// </summary>
    internal VmExecutionScope Scope { get; }

    /// <summary>The mediator, created once for a declaring profile and reused for every step.</summary>
    internal VmArtifactLoadMediator? Mediator { get; set; }

    internal VmProfileDescriptor Descriptor { get; }

    internal VmCapabilityBinding[] Bindings { get; private set; }

    internal IVmArtifactProvider? Provider { get; private set; }

    internal System.Collections.Immutable.ImmutableArray<VmHostCapabilityDescriptor> BoundShapes { get; private set; }
        = System.Collections.Immutable.ImmutableArray<VmHostCapabilityDescriptor>.Empty;

    internal System.Collections.Immutable.ImmutableArray<VmHostSignatureAssumption> Assumptions { get; private set; }
        = System.Collections.Immutable.ImmutableArray<VmHostSignatureAssumption>.Empty;

    /// <summary>
    /// Binds this profile's declared imports against the host's registrations, once.
    /// </summary>
    /// <remarks>
    /// <strong>No partial binding.</strong> A required import the composition did not register
    /// fails runtime creation, and no capability is bound at all - a runtime half-wired to its host
    /// is a runtime whose first failure happens somewhere unrelated to the mistake.
    /// </remarks>
    internal bool TryBind(
        System.Collections.Immutable.ImmutableArray<VmCapabilityRegistration> registrations,
        out VmReason failure,
        out VmCapabilityId capability)
    {
        failure = VmReason.None;
        capability = default;

        var imports = Descriptor.HostCapabilityDescriptors.IsDefault
            ? System.Collections.Immutable.ImmutableArray<VmCapabilityImport>.Empty
            : Descriptor.HostCapabilityDescriptors;

        var bindings = new VmCapabilityBinding[imports.Length];
        var shapes = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmHostCapabilityDescriptor>();
        var assumptions = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmHostSignatureAssumption>();
        IVmArtifactProvider? provider = null;

        for (var index = 0; index < imports.Length; index++)
        {
            var import = imports[index];
            var match = Find(registrations, import.Descriptor.CapabilityId, import.Descriptor.Version);

            if (match is null)
            {
                if (import.ImportKind is VmCapabilityImportKind.Required)
                {
                    failure = VmReason.CapabilityNotRegistered;
                    capability = import.Descriptor.CapabilityId;
                    return false;
                }

                bindings[index] = new VmCapabilityBinding(import, null, null, null);
                assumptions.Add(Assume(import, optionalBound: false));
                continue;
            }

            var registration = match.Value;

            // A mismatch is refused here, at binding, and never at first call: a signature that
            // only disagrees when someone happens to invoke it is a defect with a schedule.
            if (!registration.Descriptor.SignatureId.Equals(import.Descriptor.SignatureId) ||
                registration.Descriptor.Kind != import.Descriptor.Kind ||
                registration.Descriptor.Reentrancy != import.Descriptor.Reentrancy ||
                registration.Descriptor.ExceptionTranslation != import.Descriptor.ExceptionTranslation)
            {
                failure = VmReason.CapabilitySignatureMismatch;
                capability = import.Descriptor.CapabilityId;
                return false;
            }

            bindings[index] = new VmCapabilityBinding(
                import, registration.Handler, registration.BytesHandler, registration.Provider);

            // Keyed on the declared kind, which is the same field the at-most-one-provider guard
            // counts by. Keying on the presence of a provider object instead let the two disagree.
            if (registration.Kind is VmCapabilityKind.ArtifactProvider && registration.Provider is not null)
            {
                provider = registration.Provider;
            }

            shapes.Add(registration.Descriptor);
            assumptions.Add(Assume(import, optionalBound: true));
        }

        Bindings = bindings;
        Provider = provider;
        BoundShapes = shapes.ToImmutable();
        Assumptions = assumptions.ToImmutable();
        return true;
    }

    internal bool TryGetExecutor(IVmExecutionEnvironment environment, out IVmProfileExecutor created, out VmReason failure)
    {
        failure = VmReason.None;

        lock (gate)
        {
            if (executor is not null)
            {
                created = executor;
                return true;
            }
        }

        var produced = Descriptor.ExecutorFactory(environment);

        if (produced is null)
        {
            created = null!;
            failure = VmReason.ProfileContractViolation;
            return false;
        }

        // The executor's declared identity is checked when it is created, and a mismatch is
        // returned rather than thrown: it is a defect in a profile observed at run time, not a
        // composition error the caller could have prevented.
        if (!produced.ProfileId.Equals(Descriptor.ProfileId))
        {
            created = null!;
            failure = VmReason.ExecutorIdentityMismatch;
            return false;
        }

        lock (gate)
        {
            executor ??= produced;
            created = executor;
        }

        return true;
    }

    private VmHostSignatureAssumption Assume(VmCapabilityImport import, bool optionalBound) =>
        new(
            import.Descriptor.CapabilityId,
            import.Descriptor.Version,
            import.Descriptor.SignatureId,
            import.Descriptor.Kind,
            import.Descriptor.Reentrancy,
            import.Descriptor.ExceptionTranslation,
            import.ImportKind is VmCapabilityImportKind.Optional && optionalBound);

    private static VmCapabilityRegistration? Find(
        System.Collections.Immutable.ImmutableArray<VmCapabilityRegistration> registrations,
        VmCapabilityId capabilityId,
        int version)
    {
        if (registrations.IsDefault)
        {
            return null;
        }

        foreach (var registration in registrations)
        {
            if (registration.Descriptor.CapabilityId.Equals(capabilityId) &&
                registration.Descriptor.Version == version)
            {
                return registration;
            }
        }

        return null;
    }
}
