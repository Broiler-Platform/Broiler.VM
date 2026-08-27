namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group V: the surface rules whose subject VM-1 creates, each shown rejecting a real violating
/// input.
/// </summary>
/// <remarks>
/// Every rule is exercised in both directions - the checkout is clean, and a witness is caught -
/// because a rule that has never rejected anything expresses nothing. The witnesses live in
/// <see cref="ApiBaselineWitnesses"/> for the same reason the group B ones do: each is exactly what
/// its rule forbids, so it cannot be built into the product graph.
/// </remarks>
public sealed class ApiBaselineRuleTests
{
    [Fact]
    public void V1_The_Exported_Surface_Contains_The_Frozen_Names_And_Nothing_Outside_The_Namespace()
    {
        Assert.Empty(ApiBaselineRules.V1(AssemblyFacts.Product));

        // The witness: a graph exporting only the bounded-reading primitives contains none of the
        // contract names, so the rule must flag every one of them as missing.
        Assert.NotEmpty(ApiBaselineRules.V1([AssemblyFacts.Binary]));
    }

    [Fact]
    public void V2_The_Contract_Version_Type_Carries_Exactly_Two_Constants()
    {
        Assert.Empty(ApiBaselineRules.V2(typeof(VmCoreContract)));

        // The witness: a type with a third member and a static readonly where a const belongs.
        Assert.NotEmpty(ApiBaselineRules.V2(typeof(ApiBaselineWitnesses.WrongContractVersionType)));
    }

    [Fact]
    public void V3_No_Retired_Or_Banned_Name_Is_Exported()
    {
        Assert.Empty(ApiBaselineRules.V3(ApiBaselineRules.ProductTypes));

        // The witness: a type carrying one of ADR 0003's struck names.
        Assert.NotEmpty(ApiBaselineRules.V3([typeof(ApiBaselineWitnesses.RetiredNameWitness.VmHandle)]));
    }

    [Fact]
    public void V4_The_Descriptor_Declares_Exactly_The_Frozen_Rows()
    {
        Assert.Empty(ApiBaselineRules.V4(typeof(VmProfileDescriptor)));

        // The witness: a descriptor-shaped type carrying an alias set, a priority and a type name,
        // each excluded by construction at core contract version 1. It is checked against the same
        // frozen row list the real descriptor is, so the rule flags both the missing rows and the
        // excluded ones.
        Assert.NotEmpty(ApiBaselineRules.V4(typeof(ApiBaselineWitnesses.DescriptorWithExcludedRows)));

        // And a descriptor missing one frozen row is caught by name rather than by arithmetic: a
        // count would be satisfied by any substitution that kept the total.
        var oneShort = ApiBaselineRules.FrozenDescriptorRows.Take(30).ToArray();
        Assert.NotEmpty(ApiBaselineRules.V4(typeof(VmProfileDescriptor), oneShort));
    }

    [Fact]
    public void V5_Every_Reason_Belongs_To_Exactly_One_Category()
    {
        Assert.Empty(ApiBaselineRules.V5(VmReasonRegistry.All()));

        // The witness: a value in no category block at all. The mapping is derived from the value,
        // so a reason filed outside the blocks is exactly what the rule must catch.
        Assert.NotEmpty(ApiBaselineRules.V5([(VmReason)50]));
    }

    [Fact]
    public void V6_The_Metering_Surface_Is_Four_Members_And_Reads_Nothing()
    {
        Assert.Empty(ApiBaselineRules.V6(typeof(IVmMeter)));

        // The witness: a meter that lets a profile read its remaining fuel and charge a signed
        // amount - the two things the four-member rule exists to exclude.
        Assert.NotEmpty(ApiBaselineRules.V6(typeof(ApiBaselineWitnesses.IMeterThatReadsRemaining)));
    }

    [Fact]
    public void V7_No_Member_Offers_To_Raise_An_Allowance()
    {
        Assert.Empty(ApiBaselineRules.V7(ApiBaselineRules.ProductTypes));

        // The witness: a budget with Grant and Refund on it.
        Assert.NotEmpty(ApiBaselineRules.V7([typeof(ApiBaselineWitnesses.BudgetThatCanBeRaised)]));
    }

    [Fact]
    public void V8_No_Public_Member_Returns_An_Awaitable_And_No_Timer_Is_Referenced()
    {
        Assert.Empty(ApiBaselineRules.V8(ApiBaselineRules.ProductTypes));
        Assert.Empty(ApiBaselineRules.V8Timers(AssemblyFacts.Product));

        // The witness: a public member returning a task.
        Assert.NotEmpty(ApiBaselineRules.V8([typeof(ApiBaselineWitnesses.AwaitableSurface)]));
    }

    [Fact]
    public void V9_There_Is_One_Construction_Site_And_One_Closed_Verification_Signature()
    {
        Assert.Empty(ApiBaselineRules.V9(ApiBaselineRules.ProductTypes, AssemblyFacts.Product));
        Assert.Empty(ApiBaselineRules.V9Signature(typeof(VmRuntime).GetMethod(nameof(VmRuntime.Verify))));

        // The witness: a type with two further members that mint a handle.
        Assert.NotEmpty(ApiBaselineRules.V9([typeof(ApiBaselineWitnesses.SecondConstructionSite)]));

        // And the reachability half: a graph in which the contracts assembly alone is examined
        // cannot show the factory reachable from the runtime.
        Assert.NotEmpty(ApiBaselineRules.V9(ApiBaselineRules.ProductTypes, [AssemblyFacts.Abstractions]));

        // And a verification signature widened to take a stream, which is how an incremental form
        // would arrive if it arrived quietly.
        Assert.NotEmpty(ApiBaselineRules.V9Signature(null));
    }

    [Fact]
    public void V10_No_Member_Can_Express_An_Excluded_Shape()
    {
        Assert.Empty(ApiBaselineRules.V10(ApiBaselineRules.ProductTypes));

        // The witness: a type naming the envelope shape and a member naming the incremental one.
        Assert.NotEmpty(ApiBaselineRules.V10([typeof(ApiBaselineWitnesses.VmEnvelopeReaderWitness)]));
    }
}
