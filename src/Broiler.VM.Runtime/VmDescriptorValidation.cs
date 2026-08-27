namespace Broiler.VM;

/// <summary>
/// The admission predicate every descriptor passes before it becomes a catalog entry.
/// </summary>
/// <remarks>
/// <para>
/// Split into two halves deliberately. <see cref="ValidateSingle"/> decides a descriptor on its own
/// terms and runs eagerly at <c>Add</c>, so a wiring defect is reported with the offending
/// registration call on the stack. <see cref="ValidateAgainstAccepted"/> decides it against the
/// entries already accepted, which is the only place a duplicate or a confusable pair can be seen.
/// </para>
/// <para>
/// The contract-version admission rules run in a fixed order, so a descriptor that fails two of them
/// always reports the same one. A descriptor failing several checks that reported whichever one the
/// implementation happened to reach first would make the failure depend on the implementation.
/// </para>
/// </remarks>
internal static class VmDescriptorValidation
{
    internal static void ValidateSingle(VmProfileDescriptor descriptor, int ordinalPosition)
    {
        if (descriptor.ProfileId.IsEmpty)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.ProfileIdMalformed, ordinalPosition, nameof(descriptor.ProfileId));
        }

        ValidateCoreContractAdmission(descriptor);

        if (descriptor.ProfileId.IsReservedNamespace && !StartsWithBroiler(descriptor.PackageIdentity.PackageId))
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.ProfileIdReservedNamespace,
                descriptor.ProfileId,
                nameof(descriptor.PackageIdentity));
        }

        if (!IsWellFormedDisplayName(descriptor.DisplayName))
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.DisplayNameMalformed, descriptor.ProfileId, nameof(descriptor.DisplayName));
        }

        if (descriptor.DescriptorRevision < 1)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.DescriptorRevisionInvalid, descriptor.ProfileId, nameof(descriptor.DescriptorRevision));
        }

        if (!descriptor.SupportedFormatVersions.IsWellFormed)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.FormatVersionRangeInvalid, descriptor.ProfileId, nameof(descriptor.SupportedFormatVersions));
        }

        ValidateFeatureManifests(descriptor);

        if (descriptor.Verifier is null)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.MissingVerifier, descriptor.ProfileId, nameof(descriptor.Verifier));
        }

        if (descriptor.ExecutorFactory is null)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.MissingExecutorFactory, descriptor.ProfileId, nameof(descriptor.ExecutorFactory));
        }

        // The verifier declares its own identity, so a descriptor cannot advertise one profile and
        // hand the core another profile's verifier. Both contract integers are compared, because
        // the descriptor carries both and a mismatch in either is a mismatch.
        if (!descriptor.Verifier.ProfileId.Equals(descriptor.ProfileId) ||
            descriptor.Verifier.BuiltAgainstCoreContractVersion != descriptor.BuiltAgainstCoreContractVersion ||
            descriptor.Verifier.AuthoredCoreContractVersion != descriptor.AuthoredCoreContractVersion)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.VerifierIdentityMismatch, descriptor.ProfileId, nameof(descriptor.Verifier));
        }

        ValidateBudgets(descriptor);
        ValidateGuestLoads(descriptor);
        ValidateCapabilities(descriptor);

        if (!descriptor.PayloadKindIdRange.IsWellFormed)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.PayloadKindIdRangeInvalid, descriptor.ProfileId, nameof(descriptor.PayloadKindIdRange));
        }

        if (descriptor.ConformanceManifestId.IsEmpty)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.ConformanceIdentityMissing, descriptor.ProfileId, nameof(descriptor.ConformanceManifestId));
        }

        if (descriptor.DiagnosticsIdentity.IsEmpty)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.DiagnosticsIdentityMalformed, descriptor.ProfileId, nameof(descriptor.DiagnosticsIdentity));
        }

        if (!descriptor.PackageIdentity.IsComplete)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.PackageIdentityMissing, descriptor.ProfileId, nameof(descriptor.PackageIdentity));
        }

        if (descriptor.MaxUnchargedWork < 1)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.MaxUnchargedWorkInvalid, descriptor.ProfileId, nameof(descriptor.MaxUnchargedWork));
        }

        if (descriptor.ChargingGranularity < 1)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.ChargingGranularityInvalid, descriptor.ProfileId, nameof(descriptor.ChargingGranularity));
        }
    }

    internal static void ValidateAgainstAccepted(
        VmProfileDescriptor descriptor,
        System.Collections.Generic.IReadOnlyList<VmProfileDescriptor> accepted)
    {
        foreach (var existing in accepted)
        {
            if (existing.ProfileId.Equals(descriptor.ProfileId))
            {
                throw new VmCatalogValidationException(
                    VmCatalogValidationReason.DuplicateProfileId, descriptor.ProfileId, nameof(descriptor.ProfileId));
            }

            // The folded comparison is the uniqueness rule, and it is only ever applied here.
            // Catching the confusable pair at composition time is the whole point: at run time the
            // two entries would silently shadow each other.
            if (VmProfileId.EqualsUnderAsciiFold(existing.ProfileId, descriptor.ProfileId))
            {
                throw new VmCatalogValidationException(
                    VmCatalogValidationReason.ProfileIdAliasCollision, descriptor.ProfileId, nameof(descriptor.ProfileId));
            }
        }
    }

    private static void ValidateCoreContractAdmission(VmProfileDescriptor descriptor)
    {
        if (descriptor.BuiltAgainstCoreContractVersion < VmCoreContract.MinimumSupportedVersion)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.CoreContractVersionRetired,
                descriptor.ProfileId,
                nameof(descriptor.BuiltAgainstCoreContractVersion));
        }

        if (descriptor.BuiltAgainstCoreContractVersion > VmCoreContract.Version)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.CoreContractVersionNotYetSupported,
                descriptor.ProfileId,
                nameof(descriptor.BuiltAgainstCoreContractVersion));
        }

        if (descriptor.AuthoredCoreContractVersion < 1)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.CoreContractBuiltAgainstMismatch,
                descriptor.ProfileId,
                nameof(descriptor.AuthoredCoreContractVersion));
        }

        // The author cannot claim to have written for a contract newer than the one the profile
        // was compiled against: that combination means the two numbers came from different builds.
        if (descriptor.AuthoredCoreContractVersion > descriptor.BuiltAgainstCoreContractVersion)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.CoreContractAuthoredExceedsBuiltAgainst,
                descriptor.ProfileId,
                nameof(descriptor.AuthoredCoreContractVersion));
        }
    }

    private static void ValidateFeatureManifests(VmProfileDescriptor descriptor)
    {
        var manifests = descriptor.AcceptedFeatureManifests;

        if (manifests.IsDefaultOrEmpty)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.FeatureManifestSetEmpty, descriptor.ProfileId, nameof(descriptor.AcceptedFeatureManifests));
        }

        if (manifests.Length > VmProfileDescriptor.MaximumAcceptedFeatureManifests)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.FeatureManifestSetTooLarge, descriptor.ProfileId, nameof(descriptor.AcceptedFeatureManifests));
        }

        for (var index = 0; index < manifests.Length; index++)
        {
            if (manifests[index].IsEmpty)
            {
                throw new VmCatalogValidationException(
                    VmCatalogValidationReason.FeatureManifestIdMalformed, descriptor.ProfileId, nameof(descriptor.AcceptedFeatureManifests));
            }

            if (!manifests[index].StartsWithProfileNamespace(descriptor.ProfileId))
            {
                throw new VmCatalogValidationException(
                    VmCatalogValidationReason.FeatureManifestIdOutOfNamespace, descriptor.ProfileId, nameof(descriptor.AcceptedFeatureManifests));
            }

            for (var other = 0; other < index; other++)
            {
                if (manifests[other].Equals(manifests[index]))
                {
                    throw new VmCatalogValidationException(
                        VmCatalogValidationReason.DuplicateFeatureManifestId, descriptor.ProfileId, nameof(descriptor.AcceptedFeatureManifests));
                }
            }
        }
    }

    private static void ValidateBudgets(VmProfileDescriptor descriptor)
    {
        if (!descriptor.BudgetDeclarationMatrix.IsComplete)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.BudgetDeclarationMatrixIncomplete, descriptor.ProfileId, nameof(descriptor.BudgetDeclarationMatrix));
        }

        // Verification always does work, so declaring the verifier-work dimension inapplicable is
        // not a profile choice: it is a claim that cannot be true.
        if (descriptor.BudgetDeclarationMatrix[VmBudgetDimension.VerifierWork] is VmBudgetApplicability.NotApplicable)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.VerifierWorkNotApplicable, descriptor.ProfileId, nameof(descriptor.BudgetDeclarationMatrix));
        }

        if (descriptor.LimitDefaults.IsEmpty || descriptor.ProfileHardMaxima.IsEmpty)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.LimitDefaultsInvalid, descriptor.ProfileId, nameof(descriptor.LimitDefaults));
        }

        // A default that meant "unbounded" would make adopting the profile default the same as
        // declaring no ceiling at all, which invariant 9 forbids: omission never means unbounded.
        if (descriptor.LimitDefaults.HasAnyUnconstrained())
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.LimitDefaultsInvalid, descriptor.ProfileId, nameof(descriptor.LimitDefaults));
        }

        if (!VmLimitVector.IsNoLooserThan(descriptor.LimitDefaults, descriptor.ProfileHardMaxima))
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.ProfileDefaultExceedsProfileMaximum, descriptor.ProfileId, nameof(descriptor.LimitDefaults));
        }
    }

    private static void ValidateGuestLoads(VmProfileDescriptor descriptor)
    {
        var declaration = descriptor.GuestInitiatedLoads;

        if (declaration is null)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.GuestLoadDeclarationIncomplete, descriptor.ProfileId, nameof(descriptor.GuestInitiatedLoads));
        }

        if (declaration.Kind is VmDeclaration.NotDeclared)
        {
            // A non-declaring profile must also declare the four nested-load dimensions
            // inapplicable, so that "declares no guest loads" and "charges no nested-load
            // dimension" cannot disagree.
            if (declaration.IsWellFormed)
            {
                return;
            }

            throw new VmCatalogValidationException(
                VmCatalogValidationReason.GuestLoadDeclarationIncomplete, descriptor.ProfileId, nameof(descriptor.GuestInitiatedLoads));
        }

        if (!declaration.ProfileHardMaxima.IsFinite)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.GuestLoadMaximumUnbounded, descriptor.ProfileId, nameof(descriptor.GuestInitiatedLoads));
        }

        if (declaration.VerifierWorkToFuelRate < 1)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.VerifierWorkToFuelRateInvalid, descriptor.ProfileId, nameof(descriptor.GuestInitiatedLoads));
        }

        if (!declaration.IsWellFormed)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.GuestLoadDeclarationIncomplete, descriptor.ProfileId, nameof(descriptor.GuestInitiatedLoads));
        }

        // Declaring guest loads while declaring the nesting dimension inapplicable would leave the
        // depth bound unenforceable, so the two declarations are checked against each other.
        if (descriptor.BudgetDeclarationMatrix[VmBudgetDimension.NestedLoadDepth] is VmBudgetApplicability.NotApplicable)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.GuestLoadDeclarationIncomplete, descriptor.ProfileId, nameof(descriptor.BudgetDeclarationMatrix));
        }
    }

    private static void ValidateCapabilities(VmProfileDescriptor descriptor)
    {
        var imports = descriptor.HostCapabilityDescriptors;

        if (imports.IsDefault)
        {
            return;
        }

        for (var index = 0; index < imports.Length; index++)
        {
            if (!imports[index].Descriptor.IsWellFormed)
            {
                throw new VmCatalogValidationException(
                    VmCatalogValidationReason.HostCapabilityDescriptorInvalid, descriptor.ProfileId, nameof(descriptor.HostCapabilityDescriptors));
            }

            for (var other = 0; other < index; other++)
            {
                if (imports[other].Descriptor.CapabilityId.Equals(imports[index].Descriptor.CapabilityId))
                {
                    throw new VmCatalogValidationException(
                        VmCatalogValidationReason.DuplicateHostCapabilityId, descriptor.ProfileId, nameof(descriptor.HostCapabilityDescriptors));
                }
            }
        }

        // Importing a host capability at all requires the host-call dimension to be charged; a
        // profile that declared it inapplicable would be importing something it cannot pay for.
        if (imports.Length > 0 &&
            descriptor.BudgetDeclarationMatrix[VmBudgetDimension.HostCalls] is VmBudgetApplicability.NotApplicable)
        {
            throw new VmCatalogValidationException(
                VmCatalogValidationReason.HostCapabilityDescriptorInvalid, descriptor.ProfileId, nameof(descriptor.BudgetDeclarationMatrix));
        }
    }

    private static bool StartsWithBroiler(string packageId)
    {
        const string Prefix = "broiler.";

        if (packageId is null || packageId.Length < Prefix.Length)
        {
            return false;
        }

        for (var index = 0; index < Prefix.Length; index++)
        {
            if (VmProfileId.FoldAscii(packageId[index]) != Prefix[index])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// One to sixty-four UTF-16 units, no NUL, no C0 or C1 control, no line or paragraph separator,
    /// and no leading or trailing space. Non-ASCII is permitted and no normalization, casing or
    /// collation is performed - the field is never compared, so normalizing it would buy nothing and
    /// would introduce a globalization dependency into a component that needs none.
    /// </summary>
    private static bool IsWellFormedDisplayName(string displayName)
    {
        if (string.IsNullOrEmpty(displayName) ||
            displayName.Length > VmProfileDescriptor.MaximumDisplayNameLength)
        {
            return false;
        }

        if (displayName[0] == ' ' || displayName[^1] == ' ')
        {
            return false;
        }

        foreach (var value in displayName)
        {
            var code = (int)value;

            // C0 including NUL, DEL, the C1 block, and the line and paragraph separators. Written
            // as code points rather than as character literals so the rule stays readable in a file
            // the house style keeps to ASCII.
            if (code <= 0x1F || code == 0x7F ||
                (code >= 0x80 && code <= 0x9F) ||
                code == 0x2028 || code == 0x2029)
            {
                return false;
            }
        }

        return true;
    }
}
