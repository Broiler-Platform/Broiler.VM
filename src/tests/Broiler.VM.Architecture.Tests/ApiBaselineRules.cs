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
    /// The thirty rows of the frozen descriptor table, by name.
    /// </summary>
    /// <remarks>
    /// Row 24 carries two fields - the conformance manifest identity and its version - so thirty
    /// rows are thirty-one properties. The names are written out rather than counted: a count is
    /// satisfied by any thirty-one properties at all, including thirty-one wrong ones, which is
    /// exactly the drift a table this long exists to catch.
    /// </remarks>
    internal static readonly string[] FrozenDescriptorRows =
    [
        "ProfileId", "DisplayName", "DescriptorRevision", "SupportedFormatVersions",
        "AcceptedFeatureManifests", "Verifier", "ExecutorFactory", "ArtifactRepresentationKind",
        "ArtifactLifetimeKind", "SupportsConcurrentVerification", "ThreadAffinity",
        "CancellationPollBound", "AbandonBudget", "LimitDefaults", "ProfileHardMaxima",
        "BudgetDeclarationMatrix", "HostCapabilityDescriptors", "GuestInitiatedLoads",
        "AsynchronousInstantiation", "ExternalSuspension", "PayloadKindIdRange",
        "BuiltAgainstCoreContractVersion", "AuthoredCoreContractVersion", "ConformanceManifestId",
        "ConformanceManifestVersion", "DiagnosticsIdentity", "PackageIdentity", "ArtifactSharing",
        "FaultRecovery", "MaxUnchargedWork", "ChargingGranularity",
    ];

    /// <summary>
    /// V4: the descriptor declares exactly the frozen rows, by name, and none of the names excluded
    /// by construction.
    /// </summary>
    internal static IEnumerable<string> V4(Type descriptorType, IReadOnlyList<string>? expectedRows = null)
    {
        string[] excluded =
        [
            "Aliases", "AlternateIds", "LegacyIds", "PreviousIds", "Deprecated", "Priority",
            "Enabled", "Precedence", "OrderingHint", "Features", "LocalizedName", "FilePath",
            "AssemblyName", "TypeName",
        ];

        var rows = expectedRows ?? FrozenDescriptorRows;

        var properties = descriptorType
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Select(static property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var row in rows.Where(row => !properties.Contains(row)))
        {
            yield return $"{descriptorType.Name} is missing the frozen row {row}";
        }

        foreach (var name in properties.Where(name => !rows.Contains(name, StringComparer.Ordinal)))
        {
            yield return $"{descriptorType.Name} declares {name}, which the frozen table does not";
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

    /// <summary>
    /// V9: exactly one public member of the product graph mints a verified artifact, and exactly
    /// one product assembly reaches it.
    /// </summary>
    /// <remarks>
    /// Two halves, because either alone is weaker than the claim. Counting members that return the
    /// handle would pass a graph with one factory and any number of assemblies calling it; reading
    /// the member-reference tables says which assemblies can reach the factory at all. Together
    /// they assert that the handle is minted in one place and reachable from one place, which is
    /// what the one-construction-site rule is for.
    /// </remarks>
    internal static IEnumerable<string> V9(IEnumerable<Type> types, IReadOnlyList<AssemblyFacts>? product = null)
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

        if (product is null)
        {
            yield break;
        }

        const string Factory = "Broiler.VM.VmVerifiedArtifact.Create";

        var callers = product
            .Where(assembly => assembly.MemberReferences.Contains(Factory, StringComparer.Ordinal))
            .Select(static assembly => assembly.Name)
            .ToArray();

        if (callers.Length != 1 || callers[0] != "Broiler.VM.Runtime")
        {
            yield return
                "the verified-artifact factory is reachable from " +
                (callers.Length == 0 ? "no product assembly" : string.Join(", ", callers)) +
                ", not from Broiler.VM.Runtime alone";
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

    /// <summary>
    /// V11: no diagnostics field can carry free text, so no host secret can reach one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// VM-4's gate asks that diagnostics identify profile, version and artifact locations "without
    /// leaking host secrets", which is a claim about what a record CAN hold rather than about what
    /// this implementation happens to put in one. A record with a message field would be one
    /// exception handler away from carrying a connection string, a path or a token, and no test over
    /// today's call sites would notice the day it did.
    /// </para>
    /// <para>
    /// So the shape is asserted instead: every member of the record is an enum, a number, or one of
    /// the validated identity types, and the ONE member that carries text is the caller identity the
    /// caller itself supplied. The core cannot leak what it has nowhere to put.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> V11(Type diagnostics)
    {
        foreach (var property in diagnostics.GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.DeclaredOnly))
        {
            var type = property.PropertyType;

            if (TextBearingIdentities.Contains(type) || type.IsEnum || type.IsPrimitive)
            {
                continue;
            }

            if (type.IsValueType && !CarriesText(type))
            {
                continue;
            }

            yield return $"VmDiagnostics.{property.Name} is a {type.Name}, which can carry free text";
        }
    }

    /// <summary>
    /// The four members of a diagnostics record that carry text, and why each is admitted.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Three are IDENTITIES under a closed grammar: dot-separated ASCII labels, bounded in length,
    /// no control characters, validated at parse. Their values come from the descriptor a
    /// composition wrote, which is the thing the record exists to identify - a diagnostics record
    /// that could not name the profile would fail the other half of the same gate clause.
    /// </para>
    /// <para>
    /// The fourth is the caller identity, whose content is the caller's own. A host that wants
    /// nothing of its own recorded passes nothing, and nothing is then carried. That is a different
    /// admission from the other three and it is worth naming as such: this rule cannot stop a host
    /// putting a secret in its own caller identity. What it stops is the core acquiring somewhere
    /// to put one.
    /// </para>
    /// </remarks>
    internal static readonly Type[] TextBearingIdentities =
    [
        typeof(VmCallerIdentity),
        typeof(VmProfileId),
        typeof(VmFeatureManifestId),
        typeof(VmCapabilityId),
    ];

    /// <summary>
    /// Whether a value type holds a string field of its own.
    /// </summary>
    /// <remarks>
    /// One level, which is what the record's shape needs: every group it carries is a flat struct
    /// of numbers, enums and validated identities, and a group that nested another struct to hide a
    /// string would be caught by the same walk one call down.
    /// </remarks>
    private static bool CarriesText(Type type) =>
        type.GetFields(
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance)
            .Any(static field =>
                field.FieldType == typeof(string) ||
                (field.FieldType.IsValueType &&
                 !field.FieldType.IsPrimitive &&
                 !field.FieldType.IsEnum &&
                 field.FieldType != field.DeclaringType &&
                 CarriesText(field.FieldType)));

    /// <summary>
    /// V12: nothing a profile is handed can reach CLR reflection, a delegate, or an untyped object.
    /// </summary>
    /// <remarks>
    /// The gate's last sentence: host imports cannot reach undeclared CLR surface. What a profile
    /// is handed at execution is an environment, a meter, a capability table and possibly a load
    /// mediator, and every member of all four is asserted here to traffic only in the contract's own
    /// types and primitives. A single member returning <c>object</c> or a <c>Type</c> would turn the
    /// capability table into an ambient platform surface, which is exactly what addressing
    /// capabilities by index rather than by name exists to prevent.
    /// </remarks>
    internal static IEnumerable<string> V12(IEnumerable<Type> profileFacing)
    {
        Type[] banned =
        [
            typeof(object), typeof(Type), typeof(Delegate), typeof(MulticastDelegate),
            typeof(System.Reflection.MemberInfo), typeof(System.Reflection.MethodInfo),
            typeof(System.Reflection.Assembly), typeof(System.Reflection.FieldInfo),
            typeof(System.Reflection.PropertyInfo), typeof(System.Runtime.Loader.AssemblyLoadContext),
            typeof(System.IntPtr), typeof(System.UIntPtr),
        ];

        foreach (var type in profileFacing)
        {
            foreach (var method in type.GetMethods(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.DeclaredOnly))
            {
                foreach (var used in method.GetParameters()
                             .Select(static parameter => parameter.ParameterType)
                             .Append(method.ReturnType))
                {
                    var bare = used.IsByRef ? used.GetElementType()! : used;

                    if (banned.Contains(bare))
                    {
                        yield return $"{type.Name}.{method.Name} traffics in {bare.Name}";
                    }
                }
            }
        }
    }

    /// <summary>The four things a profile is handed while it executes.</summary>
    internal static Type[] ProfileFacingContracts =>
    [
        typeof(IVmExecutionEnvironment),
        typeof(IVmHostCapabilityInvoker),
        typeof(IVmMeter),
        typeof(IVmVerificationContext),
        typeof(IVmArtifactLoadMediator),
    ];

    private static IEnumerable<string> MemberNames(Type type) =>
        type.GetMembers(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.DeclaredOnly)
            .Select(static member => member.Name);
}
