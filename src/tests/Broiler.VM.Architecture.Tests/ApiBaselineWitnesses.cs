namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The deliberately violating inputs for group V.
/// </summary>
/// <remarks>
/// <para>
/// The register's criterion is that every rule has a witness: a real violating input it is shown
/// rejecting. A rule that has only ever been pointed at a clean checkout has demonstrated that the
/// checkout is clean, which is not the same claim.
/// </para>
/// <para>
/// They live in the test assembly for the same reason <c>DynamicLoadingWitness</c> does: each one
/// is exactly what the rule forbids, so it cannot be built into the product graph without breaking
/// the rule it exists to witness.
/// </para>
/// </remarks>
public static class ApiBaselineWitnesses
{
    /// <summary>Witness for V2: a contract-version type with a third member and a non-literal field.</summary>
    public static class WrongContractVersionType
    {
        /// <summary>A field that is not a literal, so a consumer reads it rather than folding it.</summary>
        public static readonly int Version = 1;

        /// <summary>The second constant, correctly spelled.</summary>
        public const int MinimumSupportedVersion = 1;

        /// <summary>A third member, which the frozen record permits nowhere.</summary>
        public const string History = "1";
    }

    /// <summary>Witness for V3: a type carrying one of the struck names.</summary>
    public static class RetiredNameWitness
    {
        /// <summary>A struck name. Its presence here is what makes V3 falsifiable.</summary>
        public sealed class VmHandle
        {
        }
    }

    /// <summary>Witness for V4: a descriptor-shaped type carrying a row excluded by construction.</summary>
    public sealed class DescriptorWithExcludedRows
    {
        /// <summary>An alias set. Core contract version 1 admits no alias mechanism at all.</summary>
        public string[] Aliases { get; } = [];

        /// <summary>A priority. A composition root is a package, never a run-time ordering hint.</summary>
        public int Priority { get; }

        /// <summary>A type name, which is the seed of reflection-based composition.</summary>
        public string TypeName { get; } = string.Empty;
    }

    /// <summary>Witness for V6: a metering surface that lets a profile read what it has left.</summary>
    public interface IMeterThatReadsRemaining
    {
        /// <summary>A remaining reader, which is what the four-member rule exists to exclude.</summary>
        long RemainingFuel { get; }

        /// <summary>A signed charge, which makes a refund expressible.</summary>
        bool TryCharge(VmBudgetDimension dimension, long amount);
    }

    /// <summary>Witness for V7: a member that offers to raise an allowance.</summary>
    public sealed class BudgetThatCanBeRaised
    {
        /// <summary>Granting more allowance is the monotonicity violation the rule names.</summary>
        public void Grant(ulong amount) => _ = amount;

        /// <summary>So is refunding it.</summary>
        public void Refund(ulong amount) => _ = amount;
    }

    /// <summary>Witness for V8: a public member returning an awaitable.</summary>
    public sealed class AwaitableSurface
    {
        /// <summary>A second, undeclared way to wait, which contract version 1 does not admit.</summary>
        public System.Threading.Tasks.Task<int> VerifyAsync() =>
            System.Threading.Tasks.Task.FromResult(0);
    }

    /// <summary>Witness for V9: a second construction site for a verified artifact.</summary>
    public static class SecondConstructionSite
    {
        /// <summary>
        /// A member that mints a handle outside the one construction site. Two of them is how the
        /// one-verifier property is lost quietly.
        /// </summary>
        public static VmVerifiedArtifact Mint() => null!;

        /// <summary>And a third, so the count is unambiguous.</summary>
        public static VmVerifiedArtifact MintAgain() => null!;
    }

    /// <summary>Witness for V10: a member naming an excluded shape.</summary>
    public sealed class VmEnvelopeReaderWitness
    {
        /// <summary>
        /// A member by which the reserved envelope stage could be entered. Its absence from the
        /// real surface is the invariant 8 discharge; its presence here is what proves the rule
        /// would notice.
        /// </summary>
        public int ReadEnvelope() => 0;

        /// <summary>A streaming verification entry point, which would be an amendment.</summary>
        public int VerifyIncremental() => 0;
    }

    /// <summary>
    /// Witness for B6: a type that actually names a fixture type, so the test assembly emits an
    /// assembly reference to <c>Broiler.VM.Fixtures</c>.
    /// </summary>
    /// <remarks>
    /// A project reference alone emits nothing into the AssemblyRef table: the compiler records
    /// only what is used. Without this, B6's scanner would be pointed at an assembly that does not
    /// in fact reference a test-built one, and the rule would report a false clean.
    /// </remarks>
    public static class FixtureReferenceWitness
    {
        /// <summary>The fixture profile's identity, named here purely to create the reference.</summary>
        public static VmProfileId FixtureProfileId => Broiler.VM.Fixtures.FixtureVmProfile.Id;
    }

    /// <summary>
    /// Witness for V11: a diagnostics record carrying a message, which is how a host secret gets in.
    /// </summary>
    /// <remarks>
    /// A message field is the shape that always looks reasonable. The exception handler that fills
    /// it is somewhere else, is written later, and is the one line between an operation's failure
    /// and a connection string in a caller's log.
    /// </remarks>
    public readonly struct DiagnosticsWithAMessage
    {
        /// <summary>The stage, as the real record carries it.</summary>
        public VmStage Stage { get; init; }

        /// <summary>The free text this rule exists to forbid.</summary>
        public string Message { get; init; }
    }

    /// <summary>
    /// Witness for V11: a diagnostics record whose group carries text one level down.
    /// </summary>
    /// <remarks>
    /// The second shape, and the one a check that only looked at the record's own members would
    /// miss: the field is a struct of the contract's own namespace, and the string is inside it.
    /// </remarks>
    public readonly struct DiagnosticsWithATextBearingGroup
    {
        /// <summary>A group that looks like every other group and is not.</summary>
        public HostDetail Detail { get; init; }

        /// <summary>A group carrying free text.</summary>
        public readonly struct HostDetail
        {
            private readonly string text;

            /// <summary>Creates the group.</summary>
            public HostDetail(string value) => text = value;

            /// <summary>What the host said.</summary>
            public override string ToString() => text;
        }
    }

    /// <summary>
    /// Witness for V12: a profile-facing surface that hands back a CLR type.
    /// </summary>
    /// <remarks>
    /// One member is enough. A capability table that can answer "what type is behind binding zero"
    /// is a reflection surface with an index, and everything the index was supposed to prevent
    /// follows from that one answer.
    /// </remarks>
    public interface ICapabilityTableThatLeaksTheClr
    {
        /// <summary>How many slots there are, which is the legitimate half.</summary>
        int BindingCount { get; }

        /// <summary>The type behind a slot, which is the half that must not exist.</summary>
        Type TypeOf(int bindingIndex);

        /// <summary>And an untyped answer, which is the same leak with fewer steps.</summary>
        object Resolve(int bindingIndex);
    }
}
