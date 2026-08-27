namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group B: the rules decidable only from compiled metadata. B1, B2, B3, B5, B5b, B6 and B7 read
/// the AssemblyRef, MemberRef, TypeRef, TypeDef and CustomAttribute tables directly, because a
/// package can reintroduce an edge that no project file spells out, and because loading an
/// assembly to reflect over it would run the very module initializers rule B5b exists to detect.
/// B4 and the member half of E5 are the scoped exception: they reflect over assemblies this test
/// project already references, because they inspect a public signature that is already loaded
/// rather than trying to prove the absence of dynamic loading. ADR 0001 records that split.
/// </summary>
/// <remarks>
/// Several of these are registered Vacuous at VM-0 in rules.register.json. That is the honest
/// state of a rule whose subject is a shell: Broiler.VM.Runtime emits no assembly reference
/// because it uses no type, so B2, B3 and B6 hold trivially. They are written, wired and running
/// now so that VM-1 inherits them; they are not counted as evidence that the boundary was
/// tested against something real.
/// </remarks>
public sealed class AssemblyMetadataRuleTests
{
    [Fact]
    public void B1_The_Graph_Sinks_Reference_Nothing_Outside_The_Framework()
    {
        Assert.Empty(ArchitectureRules.B1(AssemblyFacts.Abstractions));
        Assert.Empty(ArchitectureRules.B1(AssemblyFacts.Binary));

        // The witness: the test assembly itself references xunit, so the rule must flag it.
        Assert.NotEmpty(ArchitectureRules.B1(AssemblyFacts.TestAssembly));
    }

    [Fact]
    public void B2_The_Runtime_References_Nothing_Outside_Abstractions_And_Binary()
    {
        Assert.Empty(ArchitectureRules.B2(AssemblyFacts.Runtime));
        Assert.NotEmpty(ArchitectureRules.B2(AssemblyFacts.TestAssembly));
    }

    [Fact]
    public void B3_No_Assembly_Names_A_Foreign_Broiler_Assembly()
    {
        foreach (var assembly in AssemblyFacts.Product.Append(AssemblyFacts.Fixtures))
        {
            Assert.Empty(ArchitectureRules.B3(assembly));
        }
    }

    [Fact]
    public void B4_The_Public_Surface_Names_No_Foreign_Type()
    {
        Assert.Empty(ArchitectureRules.B4(typeof(VmCoreContract).Assembly));

        // The witness: this test assembly exports types whose members name xunit types.
        Assert.NotEmpty(ArchitectureRules.B4(typeof(RuleRegisterTests).Assembly));
    }

    [Fact]
    public void B5_No_Assembly_Reaches_A_Dynamic_Loading_Api()
    {
        foreach (var assembly in AssemblyFacts.Product)
        {
            Assert.Empty(ArchitectureRules.B5(assembly));
        }

        // The witness: DynamicLoadingWitness calls Type.GetType and Activator.CreateInstance.
        Assert.NotEmpty(ArchitectureRules.B5(AssemblyFacts.TestAssembly));
    }

    [Fact]
    public void B5b_No_Assembly_Applies_A_Module_Initializer()
    {
        foreach (var assembly in AssemblyFacts.Product)
        {
            Assert.Empty(ArchitectureRules.B5b(assembly));
        }

        // The witness: ModuleInitializerWitness carries the attribute.
        Assert.NotEmpty(ArchitectureRules.B5b(AssemblyFacts.TestAssembly));
    }

    [Fact]
    public void B6_No_Product_Assembly_References_A_Test_Assembly()
    {
        foreach (var assembly in AssemblyFacts.Product)
        {
            Assert.Empty(ArchitectureRules.B6(assembly));
        }
    }

    [Fact]
    public void B7_No_Product_Assembly_Exports_An_Aggregate_Profile_Type()
    {
        foreach (var assembly in AssemblyFacts.Product)
        {
            Assert.Empty(ArchitectureRules.B7(assembly));
        }

        // The witness: a publicly nested type named BuiltInProfiles. Nested, deliberately - a
        // visibility test written for top-level types alone would not see it.
        Assert.NotEmpty(ArchitectureRules.B7(AssemblyFacts.TestAssembly));
    }

    [Fact]
    public void E5_The_Public_Surface_Is_Exactly_The_Core_Contract_Version()
    {
        var exported = AssemblyFacts.Product
            .SelectMany(static assembly => assembly.PublicTypeNames)
            .OrderBy(static name => name, StringComparer.Ordinal);

        Assert.Equal(["Broiler.VM.VmCoreContract"], exported);

        var members = typeof(VmCoreContract)
            .GetMembers(System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Static |
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.DeclaredOnly)
            .Select(static member => member.Name)
            .OrderBy(static name => name, StringComparer.Ordinal);

        Assert.Equal(["MinimumSupportedVersion", "Version"], members);
    }
}
