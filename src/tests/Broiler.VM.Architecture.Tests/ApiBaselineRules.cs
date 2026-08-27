namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group V: the surface rules whose subject VM-1 creates.
/// </summary>
/// <remarks>
/// <para>
/// VM-0's rule E5 fused two claims - a cardinality claim about the graph ("exactly one public
/// type") and a member claim about one type ("whose only members are the two contract-version
/// constants"). Only the first is falsified by VM-1: the product graph now exports a contract
/// surface. E5 is therefore rewritten as V1, an API-baseline rule against the frozen public-name
/// table, and its surviving half is preserved verbatim as V2.
/// </para>
/// <para>
/// Every rule here is a function of what it is pointed at, so it can be aimed at the product graph
/// and at a deliberately violating input. A rule that could only ever be pointed at the real
/// surface would be unfalsifiable, which is the state the register exists to prevent.
/// </para>
/// <para>
/// The letter V is used because A, B, C, D and E are taken by ADR 0001 and ADR 0003, and G, P, S,
/// F, T and R all collide with clause labels inside ADRs 0007, 0011 and 0012.
/// </para>
/// </remarks>
internal static class ApiBaselineRules
{
    /// <summary>
    /// The names ADR 0003's frozen public-name table fixes, plus the names each contract-bearing
    /// record freezes in its own normative text.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the baseline V1 compares the graph against. It is written out rather than derived,
    /// because a baseline derived from the assembly it checks would agree with any change.
    /// </para>
    /// <para>
    /// <c>VmOperation</c> is deliberately absent, and Exclusion EX-41 records why: VM-1 realises
    /// the operation as an internal runtime object addressed publicly through
    /// <see cref="VmOperationControlHandle"/> and <see cref="VmOperationStateSnapshot"/>, so the
    /// frozen name is used but not exported. Listing it here would fail the rule; omitting it
    /// silently would hide a deviation from the frozen table. It is omitted with this note instead.
    /// </para>
    /// </remarks>
    internal static readonly string[] FrozenPublicNames =
    [
        // ADR 0003, the frozen public-name table.
        "VmCoreContract", "VmCatalog", "VmCatalogBuilder", "VmProfileDescriptor",
        "VmCatalogValidationException", "VmRuntime", "VmRuntimeCreationResult", "VmControlResult",
        "VmDiagnostics", "VmCoreDefectException", "VmVerifiedArtifact", "VmVerifiedArtifactState",
        "VmArtifactLifetimeKind", "VmArtifactRepresentationKind", "VmAggregateBudget",
        "VmBudgetScope", "IVmArtifactLoadMediator", "VmGuestLoadBounds", "VmArtifactOrigin",
        "VmOperationControlHandle", "VmSuspension", "VmSuspensionOrigin", "VmOpaqueRef",

        // ADR 0005's result vocabulary, frozen by that record's own normative text.
        "VmOutcome", "VmReason", "IVmOperationResult", "VmVerificationResult",
        "VmInstantiationResult", "VmInvocationResult", "VmSourcePosition", "VmCallerIdentity",
        "VmHostCallOutcome",

        // ADR 0004's lifecycle objects, less VmOperation - see the remarks above.
        "VmInstance", "VmObjectId", "VmThreadAffinity",

        // ADR 0002's identity types.
        "VmProfileId", "VmFeatureManifestId", "VmFormatVersionRange",
    ];

    /// <summary>
    /// The sixteen names ADR 0003 struck, plus the struck bound names of the records that own them.
    /// </summary>
    /// <remarks>
    /// A struck name must appear nowhere. Keeping the list executable rather than in prose is what
    /// stops one reappearing in a later change because nobody remembered it had been rejected.
    /// </remarks>
    internal static readonly string[] RetiredNames =
    [
        "VmSuspensionToken", "VmSuspensionCause", "VmCompositionException",
        "VmProfileIdentityMismatchException", "VmCatalogResult", "VmHandle",
        "VmArtifactOwnershipKind", "DisposeRequested", "RequestResume", "VerificationMode",
        "EffectiveSectionVerificationMode", "ProducedBy", "Allocation", "HostCallCount",
        "LiveRuntimeCount", "WallClockPausesWhileSuspended",
        "MaxDepth", "MaxFanOutPerOperation", "MaxCumulativeNestedBytes",
        "MaxCumulativeNestedVerifierWork",
    ];

    /// <summary>The six unqualified names ADR 0003's terminology rule bans outright.</summary>
    internal static readonly string[] BannedUnqualifiedTypeNames =
    [
        "Profile", "IProfile", "ProfileId", "ProfileDescriptor", "ProfileCatalog", "ProfileFactory",
    ];

    /// <summary>The twelve member names ADR 0007 bans, because each would break monotonicity.</summary>
    internal static readonly string[] BannedBudgetMemberNames =
    [
        "Grant", "Refund", "Reset", "Extend", "Increase", "Raise",
        "TopUp", "Widen", "Reopen", "WithLimits", "Withdraw", "Credit",
    ];

    /// <summary>The shapes core release 1 discharges by absence from the API baseline.</summary>
    internal static readonly string[] ExcludedShapes =
    [
        "Envelope", "Streaming", "Incremental", "FromSource", "LazySection",
    ];

    /// <summary>The three product assemblies as loaded types.</summary>
    internal static IEnumerable<Type> ProductTypes =>
        new[]
        {
            typeof(VmCoreContract).Assembly,
            typeof(VmBoundedReader).Assembly,
            typeof(VmRuntime).Assembly,
        }.SelectMany(static assembly => assembly.GetExportedTypes());

    /// <summary>
    /// V1: every name the frozen table fixes is exported, and every exported type is in the
    /// <c>Broiler.VM</c> namespace.
    /// </summary>
    /// <remarks>
    /// One-directional on membership: the baseline names what the records froze, and a name minted
    /// by VM-1 and owned by no record is not a violation, because a public name introduced by
    /// exactly one record is frozen by that record's own text. Bidirectional on the namespace,
    /// which every record does fix.
    /// </remarks>
    internal static IEnumerable<string> V1(IReadOnlyList<AssemblyFacts> product)
    {
        var exported = product
            .SelectMany(static assembly => assembly.PublicTypeNames)
            .ToArray();

        var simpleNames = exported
            .Select(static name => name[(name.LastIndexOfAny(['.', '+']) + 1)..])
            .ToHashSet(StringComparer.Ordinal);

        var missing = FrozenPublicNames
            .Where(name => !simpleNames.Contains(name))
            .Select(name => $"the frozen name {name} is not exported by the product graph");

        var misplaced = exported
            .Where(static name => !name.StartsWith("Broiler.VM.", StringComparison.Ordinal))
            .Select(name => $"{name} is exported outside namespace Broiler.VM");

        return missing.Concat(misplaced);
    }

    /// <summary>
    /// V2: the contract-version type carries exactly the two constants, and both are literals.
    /// </summary>
    /// <remarks>
    /// This is E5's surviving half. A <c>static readonly</c> would be a field read at run time
    /// rather than a value folded into every consumer, so the constant a support table quotes could
    /// differ from the one a shipped binary carries.
    /// </remarks>
    internal static IEnumerable<string> V2(Type contractType)
    {
        var members = contractType
            .GetMembers(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly)
            .Select(static member => member.Name)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (!members.SequenceEqual(["MinimumSupportedVersion", "Version"], StringComparer.Ordinal))
        {
            yield return $"{contractType.Name} declares " + string.Join(", ", members);
        }

        var nonLiteral = contractType
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(static field => !field.IsLiteral);

        foreach (var field in nonLiteral)
        {
            yield return $"{contractType.Name}.{field.Name} is not a const";
        }
    }

    /// <summary>V3: no exported type or member uses a retired or banned name.</summary>
    internal static IEnumerable<string> V3(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var simpleName = type.Name;

            if (RetiredNames.Contains(simpleName, StringComparer.Ordinal))
            {
                yield return $"the surface exports the retired name {simpleName}";
            }

            if (BannedUnqualifiedTypeNames.Contains(simpleName, StringComparer.Ordinal))
            {
                yield return $"the surface exports the banned unqualified name {simpleName}";
            }

            foreach (var member in MemberNames(type).Where(name =>
                RetiredNames.Contains(name, StringComparer.Ordinal)))
            {
                yield return $"{simpleName} declares the retired member {member}";
            }
        }
    }

    /// <summary>
    /// V4: the descriptor declares the frozen rows and none of the names excluded by construction.
    /// </summary>
    /// <remarks>
    /// Thirty-one properties for thirty rows, because row 24 of the frozen table carries two fields
    /// - the conformance manifest identity and its version. The count is stated as properties
    /// rather than as rows so it can be checked mechanically without the checker having to know
    /// which row is the double.
    /// </remarks>
    internal static IEnumerable<string> V4(Type descriptorType, int expectedProperties = 31)
    {
        string[] excluded =
        [
            "Aliases", "AlternateIds", "LegacyIds", "PreviousIds", "Deprecated", "Priority",
            "Enabled", "Precedence", "OrderingHint", "Features", "LocalizedName", "FilePath",
            "AssemblyName", "TypeName",
        ];

        var properties = descriptorType
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Select(static property => property.Name)
            .ToArray();

        if (properties.Length != expectedProperties)
        {
            yield return $"{descriptorType.Name} declares {properties.Length} properties, not {expectedProperties}";
        }

        foreach (var name in properties.Where(name => excluded.Contains(name, StringComparer.Ordinal)))
        {
            yield return $"{descriptorType.Name} declares the excluded row {name}";
        }
    }

    /// <summary>V5: every reason maps to exactly one category, derivably from its own value.</summary>
    internal static IEnumerable<string> V5(IEnumerable<VmReason> reasons)
    {
        foreach (var reason in reasons)
        {
            if (reason is VmReason.None || VmReasonRegistry.IsControlOnly(reason))
            {
                continue;
            }

            var category = VmReasonRegistry.CategoryOf(reason);

            if (category is VmOutcome.None || !VmReasonRegistry.IsLegal(category, reason))
            {
                yield return $"reason {(int)reason} belongs to no category";
            }
        }
    }

    /// <summary>
    /// V6: the metering surface declares exactly four members, its charge amount is unsigned, and
    /// no member reads a remaining or effective value.
    /// </summary>
    internal static IEnumerable<string> V6(Type meterType)
    {
        var methods = meterType.GetMethods();

        var names = methods
            .Select(static method => method.Name)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (!names.SequenceEqual(["Poll", "ReportReleased", "ReportRetained", "TryCharge"], StringComparer.Ordinal))
        {
            yield return $"{meterType.Name} declares " + string.Join(", ", names);
        }

        foreach (var member in meterType.GetMembers().Where(static member =>
            member.Name.Contains("Remaining", StringComparison.Ordinal) ||
            member.Name.Contains("Effective", StringComparison.Ordinal)))
        {
            yield return $"{meterType.Name}.{member.Name} reads a remaining or effective value";
        }

        var charge = methods.FirstOrDefault(static method => method.Name == "TryCharge");

        if (charge is not null && charge.GetParameters().Any(static parameter =>
            parameter.ParameterType == typeof(long) || parameter.ParameterType == typeof(int)))
        {
            yield return $"{meterType.Name}.TryCharge takes a signed amount";
        }
    }

    /// <summary>V7: no member offers to raise an allowance.</summary>
    internal static IEnumerable<string> V7(IEnumerable<Type> types) =>
        types
            .SelectMany(MemberNames)
            .Where(static name => BannedBudgetMemberNames.Contains(name, StringComparer.Ordinal))
            .Select(static name => $"the surface declares {name}, which would raise an allowance")
            .Distinct(StringComparer.Ordinal);

    /// <summary>V8: no public member returns an awaitable.</summary>
    /// <remarks>
    /// Core contract version 1 admits no asynchronous stage. A profile that must wait suspends and
    /// the host resumes it, so an awaitable on the surface would be a second, undeclared way to
    /// wait.
    /// </remarks>
    internal static IEnumerable<string> V8(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            foreach (var method in type.GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly))
            {
                var returned = method.ReturnType.Name;

                if (returned.StartsWith("Task", StringComparison.Ordinal) ||
                    returned.StartsWith("ValueTask", StringComparison.Ordinal) ||
                    returned.StartsWith("IAsyncEnumerable", StringComparison.Ordinal))
                {
                    yield return $"{type.Name}.{method.Name} returns {returned}";
                }
            }
        }
    }

    /// <summary>V8b: no product assembly reaches a timer, delay or thread-abort API.</summary>
    internal static IEnumerable<string> V8Timers(IReadOnlyList<AssemblyFacts> product)
    {
        string[] forbidden =
        [
            "System.Threading.Timer.Change",
            "System.Threading.Thread.Abort",
            "System.Threading.Tasks.Task.Delay",
        ];

        return product.SelectMany(assembly => assembly.MemberReferences
            .Where(reference => forbidden.Contains(reference, StringComparer.Ordinal))
            .Select(reference => $"{assembly.Name} calls {reference}"));
    }

    /// <summary>V9: exactly one public member constructs a verified artifact.</summary>
    internal static IEnumerable<string> V9(IEnumerable<Type> types)
    {
        var producers = types
            .SelectMany(static type => type.GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.DeclaredOnly))
            .Where(static method => method.ReturnType == typeof(VmVerifiedArtifact))
            .ToArray();

        if (producers.Length != 1 || producers[0].Name != nameof(VmVerifiedArtifact.Create))
        {
            yield return $"the surface has {producers.Length} verified-artifact construction sites";
        }
    }

    /// <summary>V9b: the verification member takes exactly the closed parameter set.</summary>
    internal static IEnumerable<string> V9Signature(System.Reflection.MethodInfo? verify)
    {
        Type[] banned =
        [
            typeof(byte[]), typeof(ArraySegment<byte>), typeof(Memory<byte>),
            typeof(ReadOnlyMemory<byte>), typeof(System.IO.Stream),
        ];

        if (verify is null)
        {
            yield return "no verification member is declared";
            yield break;
        }

        if (verify.GetParameters().Length != 3)
        {
            yield return $"the verification member takes {verify.GetParameters().Length} parameters, not 3";
        }

        foreach (var parameter in verify.GetParameters().Where(parameter =>
            banned.Contains(parameter.ParameterType)))
        {
            yield return $"the verification member accepts {parameter.ParameterType.Name}";
        }
    }

    /// <summary>
    /// V10: no member can express a persisted envelope, streaming or incremental verification,
    /// lazy per-section verification, or in-process producer input.
    /// </summary>
    /// <remarks>
    /// Their invariant 8 discharge is absence from the API baseline, not a returned failure. A type
    /// that existed and threw would be the shape-only stub invariant 8 rejects outright. The stage
    /// enum and the reserved envelope result type are the two exceptions: naming the stage is the
    /// contract admitting it, and neither offers a member by which it can be entered.
    /// </remarks>
    internal static IEnumerable<string> V10(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            if (type.Name is nameof(VmStage) or nameof(VmEnvelopeReadResult))
            {
                continue;
            }

            foreach (var shape in ExcludedShapes.Where(shape =>
                type.Name.Contains(shape, StringComparison.Ordinal)))
            {
                yield return $"{type.Name} names the excluded shape {shape}";
            }

            foreach (var method in type.GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly))
            {
                foreach (var shape in ExcludedShapes.Where(shape =>
                    method.Name.Contains(shape, StringComparison.Ordinal)))
                {
                    yield return $"{type.Name}.{method.Name} names the excluded shape {shape}";
                }
            }
        }
    }

    private static IEnumerable<string> MemberNames(Type type) =>
        type.GetMembers(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.DeclaredOnly)
            .Select(static member => member.Name);
}
