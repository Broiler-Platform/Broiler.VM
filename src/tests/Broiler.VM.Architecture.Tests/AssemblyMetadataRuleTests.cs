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

    /// <summary>
    /// B5 over every assembly a published image can contain, which is not where it started.
    /// </summary>
    /// <remarks>
    /// <b>THE SCOPE WIDENED ON 2026-09-07 AND THE OLD ONE WAS THE WRONG THREE.</b> This ran over
    /// the core three while the register row said "the product graph": the three assemblies least
    /// able to reach a dynamic-loading API were the only ones swept, and every profile, lowering
    /// and composition root - the assemblies that could - was outside it. A profile could have
    /// referenced <c>System.Reflection.Emit</c> and this rule would have stayed green while the
    /// register published the prohibition. The sweep is now the graph the row already claimed.
    /// </remarks>
    [Fact]
    public void B5_No_Assembly_Reaches_A_Dynamic_Loading_Api()
    {
        // Non-vacuous before the empty answers are read: the widened sweep has to have found the
        // profile families and the composition roots, or an empty result is a statement about a
        // path expression rather than about the graph.
        Assert.Contains(
            AssemblyFacts.Shipping,
            assembly => assembly.Name.StartsWith("Broiler.VM.Profile.", StringComparison.Ordinal));

        Assert.Contains(
            AssemblyFacts.Shipping,
            assembly => assembly.Name.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal));

        foreach (var assembly in AssemblyFacts.Shipping)
        {
            Assert.Empty(ArchitectureRules.B5(assembly));
        }

        // The witness: DynamicLoadingWitness calls Type.GetType and Activator.CreateInstance.
        Assert.NotEmpty(ArchitectureRules.B5(AssemblyFacts.TestAssembly));
    }

    [Fact]
    public void B5b_No_Assembly_Applies_A_Module_Initializer()
    {
        // The same widening, for the same reason: the row says "the product graph" and a module
        // initializer in a profile is exactly the ordering dependency invariant 2 forbids.
        foreach (var assembly in AssemblyFacts.Shipping)
        {
            Assert.Empty(ArchitectureRules.B5b(assembly));
        }

        // The witness: ModuleInitializerWitness carries the attribute.
        Assert.NotEmpty(ArchitectureRules.B5b(AssemblyFacts.TestAssembly));
    }

    /// <summary>
    /// A shipping assembly neither B5 nor B5b could read is one a plain build does not produce.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Both rows say "every shipping assembly", and this is the clause that makes the sentence
    /// mean every one a plain <c>dotnet test</c> can open.</b> The mobile head needs a workload the
    /// main solution deliberately does not require, so on a machine without it there is no build
    /// output and neither rule can read that assembly's tables. That is a real limit, it is stated
    /// in both register rows from 2026-09-08, and it is asserted here rather than described: every
    /// assembly the sweep could not read is one <c>Broiler.VM.slnx</c> does not list. An assembly
    /// that went unread for any other reason - a head added to the main solution and then not
    /// built, say - fails here instead of quietly narrowing two rules.
    /// </para>
    /// <para>
    /// It stops one step short of rule B5c's version on purpose. B5c can go on to assert that the
    /// unread assembly's SOURCE is in the tree rule X1 sweeps, because X1 reads for the mapping and
    /// protection entry points B5c is about. No source rule reads for a dynamic-loading member or
    /// for <c>ModuleInitializerAttribute</c>, so there is nothing for these two rows to hand the
    /// unread assembly to, and the rows say that rather than implying a cover that does not exist.
    /// </para>
    /// </remarks>
    [Fact]
    public void B5_And_B5b_Name_Every_Shipping_Assembly_They_Could_Not_Read()
    {
        var main = File.ReadAllText(Path.Combine(ComponentGraph.Root, "Broiler.VM.slnx"));

        foreach (var name in AssemblyFacts.Unread)
        {
            var project = ComponentGraph.Projects.Single(candidate =>
                string.Equals(candidate.AssemblyName, name, StringComparison.Ordinal));

            Assert.DoesNotContain(project.RelativePath, main, StringComparison.Ordinal);
        }
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
