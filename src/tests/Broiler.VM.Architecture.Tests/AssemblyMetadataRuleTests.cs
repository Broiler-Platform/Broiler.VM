namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group B: the rules decidable only from compiled metadata. B1, B2, B3, B5, B5b, B6 and B7 read
/// the AssemblyRef, MemberRef, TypeRef, TypeDef and CustomAttribute tables directly, because a
/// package can reintroduce an edge that no project file spells out, and because loading an
/// assembly to reflect over it would run the very module initializers rule B5b exists to detect.
/// B4 is the scoped exception: it reflects over assemblies this test project already references,
/// because it inspects a public signature that is already loaded rather than trying to prove the
/// absence of dynamic loading. ADR 0001 records that split.
/// </summary>
/// <remarks>
/// <para>
/// At VM-0 several of these were registered Vacuous, because the runtime was a shell that emitted
/// no assembly reference at all and so could not break B2, B3 or B6. VM-1 gives them a real
/// subject: the product graph now has method bodies, assembly references and an exported surface.
/// </para>
/// <para>
/// B3 is the one that did NOT become Active, and the register says why: A1 forbids the outbound
/// project reference, A2 forbids the package-shaped one, and the single-source NuGet.config makes
/// a foreign Broiler package unresolvable, so a violation is unreachable by construction rather
/// than merely absent. Claiming it Active would be claiming the suite had rejected something it
/// cannot construct.
/// </para>
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
        // Registered Vacuous with an activation milestone of VM-3, not Active. The subject exists
        // now - these assemblies name non-framework assemblies for the first time - but no project
        // in the checkout can acquire a foreign Broiler assembly to violate it with, so the rule
        // runs and has rejected nothing. It becomes reachable when composition roots and profile
        // packages exist.
        foreach (var assembly in AssemblyFacts.Product.Append(AssemblyFacts.Fixtures))
        {
            Assert.Empty(ArchitectureRules.B3(assembly));
        }
    }

    [Fact]
    public void B4_The_Public_Surface_Names_No_Foreign_Type()
    {
        // Every product assembly is swept now. At VM-0 only Abstractions exported a type, so
        // sweeping one was the whole graph; at VM-1 the runtime exports a surface too.
        Assert.Empty(ArchitectureRules.B4(typeof(VmCoreContract).Assembly));
        Assert.Empty(ArchitectureRules.B4(typeof(VmBoundedReader).Assembly));
        Assert.Empty(ArchitectureRules.B4(typeof(VmRuntime).Assembly));

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

        // The witness, newly available at VM-1 with no synthesis: the test assembly references
        // Broiler.VM.Fixtures, which is built from src/tests/. Pointing the scanner at it is a
        // real violating input, in the pattern B1 already uses.
        Assert.NotEmpty(ArchitectureRules.B6(AssemblyFacts.TestAssembly));
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

    // Rule E5 is superseded at VM-1. It fused a cardinality claim about the graph with a member
    // claim about one type, and only the first is falsified by the contract surface landing. V1
    // replaces the cardinality half with an API baseline and V2 preserves the member half verbatim;
    // both live in ApiBaselineRuleTests. The register retains E5 as a Deferred row so the
    // supersession is auditable rather than a silently deleted assertion.
}
