namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The exemption predicate, case by case, on shapes chosen to separate the cases from each other.
/// </summary>
/// <remarks>
/// Each case is asserted twice: a shape it must exempt, and a shape one letter away from it that
/// it must NOT. A predicate that only ever says "exempt" would satisfy half of these and is the
/// exact failure that matters, because an over-exempted unit disappears from the register without
/// anyone deciding that it should.
/// </remarks>
public sealed class AssuranceScannerTests
{
    private const string Probe = """
        namespace Probe;

        public sealed class Shapes
        {
            private readonly int stored;
            private readonly int read;
            private int mutable;

            // Case 2: assigns a parameter to the field that corresponds to it, and nothing else.
            public Shapes(int stored)
            {
                this.stored = stored;
            }

            // Case 2 must NOT cover a constructor that computes.
            public Shapes(int left, int right)
            {
                this.stored = left * right + 1;
            }


            // Case 1: an auto-property.
            public int Auto { get; init; }

            // Case 1: an accessor that only returns, and one that only assigns, the field that
            // CORRESPONDS to the property.
            public int Mutable
            {
                get { return this.mutable; }
                set { this.mutable = value; }
            }

            // Case 1 must NOT cover an accessor that decides.
            public int Guarded
            {
                get { return this.mutable; }
                set { this.mutable = value < 0 ? 0 : value; }
            }

            // Case 1 must NOT cover a property that publishes some OTHER field. This is the
            // permuted-constructor decision written as a property, and it was exempt.
            public int Published
            {
                get { return this.read; }
            }

            // Case 1 again, and not case 3: an expression-bodied PROPERTY over its own field is
            // the getter half of "only returns a field", and is answered by the property case
            // first.
            public int Stored => this.stored;

            // Case 1 must NOT cover the same shape pointed at another field.
            public int Redirected => this.read;

            // Case 3: a single member access, from a member whose name corresponds to it.
            public int Read() => this.read;

            // Case 3 must NOT cover a member access that does not correspond, whatever kind of
            // member carries it.
            public int Fetched() => this.read;

            // Case 3: a constant.
            public int Constant => 7;

            // Case 3: a delegation to another member of the same type, forwarding its own
            // parameter and nothing else.
            public int Delegated(int value) => Helper(value);

            // Case 3 must NOT cover a delegation that SUPPLIES a value. The literal and the enum
            // member are the policy in this component: they are the width a reader accepts and the
            // meter a charge lands in.
            public int Fixed() => Helper(64);

            public int Routed() => Charge(Scale.Wide, this.stored);

            // Case 3 must NOT cover a delegation whose argument is a field either: the member is
            // choosing the value, not passing one on.
            public int Captured() => Helper(this.stored);

            // Case 3: throw new.
            public int Refused() => throw new System.NotSupportedException();

            // Case 3 must NOT cover an expression body that computes.
            public int Computed() => this.stored * 2 + this.mutable;

            // Relevant: a loop is never trivial.
            public int Counted(int[] values)
            {
                var total = 0;

                foreach (var value in values)
                {
                    total += value;
                }

                return total;
            }

            private static int Helper(int value) => value;

            private static int Charge(Scale scale, int amount) => amount;
        }

        public enum Scale
        {
            Narrow,
            Wide,
        }

        public readonly struct Ceilings
        {
            // Case 2: every parameter reaches the member that corresponds to it, once.
            public Ceilings(ulong maxSectionCount, ulong maxDeclaredCount)
            {
                MaxSectionCount = maxSectionCount;
                MaxDeclaredCount = maxDeclaredCount;
            }

            public ulong MaxSectionCount { get; }

            public ulong MaxDeclaredCount { get; }
        }

        public readonly struct Permuted
        {
            // Case 2 must NOT cover a permutation. Both right-hand sides are parameters of this
            // constructor, which is all the predicate used to ask, and each value lands in the
            // member that belongs to the other one.
            public Permuted(ulong maxSectionCount, ulong maxDeclaredCount)
            {
                MaxSectionCount = maxDeclaredCount;
                MaxDeclaredCount = maxSectionCount;
            }

            public ulong MaxSectionCount { get; }

            public ulong MaxDeclaredCount { get; }
        }

        public sealed class Doubled
        {
            private readonly int value;

            // Case 2 must NOT cover a constructor that assigns one parameter twice. Both members
            // correspond to it, and the second assignment is a decision the signature does not
            // make - some other parameter, or some other member, is going without.
            public Doubled(int value)
            {
                this.value = value;
                Value = value;
            }

            public int Value { get; }

            public int Stored => this.value;
        }

        public sealed class Constants
        {
            // An initialized constant and an initialized static readonly are code units.
            public const int MaximumEntries = 64;

            public static readonly int[] Widths = [1, 2, 4];

            // A field that is neither const nor static readonly declares storage. It IS a unit -
            // every field declaration is - and the predicate answers case 7 for it.
            private readonly int used = 3;

            private int mutable;

            public int Used => used;

            public int Mutable
            {
                get { return mutable; }
                set { mutable = value; }
            }
        }

        public readonly struct Pair : System.IEquatable<Pair>
        {
            public Pair(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }

            public int Y { get; }

            // Case 5 must NOT cover an Equals that decides equality itself.
            public bool Equals(Pair other) => X == other.X && Y == other.Y;

            // Case 5: type-test and hand off.
            public override bool Equals(object? obj) => obj is Pair other && Equals(other);

            // Case 5: hand off to a framework combiner.
            public override int GetHashCode() => System.HashCode.Combine(X, Y);

            // Case 5: an operator that only delegates.
            public static bool operator ==(Pair left, Pair right) => left.Equals(right);

            // Case 5 must NOT cover a negation. `!left.Equals(right)` is the OPPOSITE decision,
            // and while `!` was on the whitelist every operator != in the component was exempt.
            public static bool operator !=(Pair left, Pair right) => !left.Equals(right);

            // Case 5 must NOT cover an operator that computes.
            public static Pair operator +(Pair left, Pair right) => new(left.X + right.X, left.Y + right.Y);
        }

        public sealed record Carried
        {
            // Case 4: the compiler supplies this, not the source.
            public int Value { get; init; }

            // Case 4 must NOT cover a record member that has a body.
            public int Doubled()
            {
                var result = Value;
                return result + result;
            }
        }

        internal static class AssemblyMarker
        {
            // Case 6: nothing inside a marker type is an implementation.
            public static string Describe(int count)
            {
                var text = string.Empty;

                for (var index = 0; index < count; index++)
                {
                    text += index;
                }

                return text;
            }
        }
        """;

    private static AssuranceExemption Exemption(string member) =>
        AssuranceProbe.Unit(Probe, member).Exemption;

    /// <summary>
    /// Case 1, and the correspondence that is the whole of it.
    /// </summary>
    /// <remarks>
    /// The predicate used to accept ANY member access in a property body, which is the
    /// permuted-constructor defect one screen further down the same file: case 2 was narrowed to
    /// require that each parameter reach the member corresponding to it, and case 1 went on
    /// exempting <c>MaxSectionCount =&gt; _maxDeclaredCount</c>. Publishing the declared-count
    /// ceiling from the section-count property has the same effect on every bounded read as
    /// exchanging the two constructor assignments, and it carried no annotation.
    /// </remarks>
    [Fact]
    public void Case_1_Covers_An_Auto_Property_And_An_Accessor_Over_Its_Own_Field()
    {
        Assert.Equal(AssuranceExemption.TrivialPropertyOrAccessor, Exemption("Shapes.Auto"));
        Assert.Equal(AssuranceExemption.TrivialPropertyOrAccessor, Exemption("Shapes.Mutable"));
        Assert.Equal(AssuranceExemption.TrivialPropertyOrAccessor, Exemption("Shapes.Stored"));

        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Guarded"));
        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Published"));
        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Redirected"));
    }

    /// <summary>
    /// Case 2, and the correspondence that is the whole of it.
    /// </summary>
    /// <remarks>
    /// The predicate used to ask only that each right-hand side was SOME parameter of the
    /// constructor, so every permutation of the assignments was equally exempt. Swapping two lines
    /// in the constructor of <c>VmReadBounds</c> re-points the section-count ceiling at the
    /// declared-count ceiling for every bounded read of untrusted input, and the unit carried no
    /// annotation, so no fingerprint moved and no rule had anything to report.
    /// </remarks>
    [Fact]
    public void Case_2_Covers_A_Constructor_That_Assigns_Each_Parameter_To_Its_Own_Member()
    {
        Assert.Equal(AssuranceExemption.ParameterAssigningConstructor, Exemption("Shapes.Shapes(int)"));
        Assert.Equal(
            AssuranceExemption.ParameterAssigningConstructor,
            Exemption("Ceilings.Ceilings(ulong, ulong)"));

        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Shapes(int, int)"));
        Assert.Equal(AssuranceExemption.None, Exemption("Permuted.Permuted(ulong, ulong)"));
        Assert.Equal(AssuranceExemption.None, Exemption("Doubled.Doubled(int)"));
    }

    /// <summary>
    /// Case 3, and the argument rule that is the whole of its delegation half.
    /// </summary>
    /// <remarks>
    /// A delegation that forwards its own parameters decides nothing, because the caller chose
    /// them. A delegation that SUPPLIES a value is making the decision the value encodes, and in
    /// this component that value is the policy: <c>TryReadVarUInt64Core(maxBits: 64, ...)</c> is
    /// the width of an LEB128 reader over untrusted bytes, and
    /// <c>TryCharge(VmBudgetDimension.AllocatedBytes, ...)</c> is the meter a bounded allocation is
    /// charged against. Both were exempt, so neither carried a fingerprint, so neither edit was
    /// visible to any rule.
    /// </remarks>
    [Fact]
    public void Case_3_Covers_A_Member_Access_A_Forwarding_Delegation_A_Constant_And_A_Throw()
    {
        Assert.Equal(AssuranceExemption.TrivialExpressionBodiedMember, Exemption("Shapes.Read()"));
        Assert.Equal(AssuranceExemption.TrivialExpressionBodiedMember, Exemption("Shapes.Constant"));
        Assert.Equal(AssuranceExemption.TrivialExpressionBodiedMember, Exemption("Shapes.Delegated(int)"));
        Assert.Equal(AssuranceExemption.TrivialExpressionBodiedMember, Exemption("Shapes.Refused()"));

        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Computed()"));
        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Counted(int[])"));
        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Fixed()"));
        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Routed()"));
        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Captured()"));

        // The member-access half of this case requires the same correspondence case 1 requires,
        // and for the same reason: without it, a property case 1 had just refused was readmitted
        // here, because a property has an arrow body like anything else.
        Assert.Equal(AssuranceExemption.None, Exemption("Shapes.Fetched()"));
    }

    /// <summary>
    /// Every field declaration is a code unit, and only a <c>const</c> or <c>static readonly</c>
    /// one that states a value is RELEVANT.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The scanner's own doc comment used to justify excluding every field with "they declare no
    /// implementation for a human to certify", which is false for an initialized one. The two that
    /// cost this component most were both budgets - a catalog cap ADR 0002 freezes at 64, and a
    /// default unwind allowance - and each could be multiplied with nothing to notice, while the
    /// method that read the constant kept asserting the fingerprint of the reviewed version.
    /// </para>
    /// <para>
    /// The next revision excluded the plain fields, which was the same mistake one step smaller: a
    /// field's TYPE is the width of the arithmetic every annotated body performs on it, and
    /// changing <c>ulong</c> to <c>uint</c> moves no annotated body's tokens. They are units now
    /// and the predicate answers case 7 for them, so they need no annotation and the manifest
    /// records their fingerprints. Being a unit and being relevant are different questions.
    /// </para>
    /// </remarks>
    [Fact]
    public void Every_Field_Is_A_Unit_And_Only_A_Fixed_Value_Is_Relevant()
    {
        Assert.Equal(AssuranceExemption.None, Exemption("Constants.MaximumEntries"));
        Assert.Equal(AssuranceExemption.None, Exemption("Constants.Widths"));

        Assert.Equal(AssuranceExemption.FieldDeclaringStorage, Exemption("Constants.used"));
        Assert.Equal(AssuranceExemption.FieldDeclaringStorage, Exemption("Constants.mutable"));

        Assert.Equal(
            new[]
            {
                "Constants.MaximumEntries",
                "Constants.Mutable",
                "Constants.Used",
                "Constants.Widths",
                "Constants.mutable",
                "Constants.used",
            },
            AssuranceProbe.Scan(Probe)
                .Select(static unit => unit.Name)
                .Where(static name => name.StartsWith("Probe.Constants.", StringComparison.Ordinal))
                .Select(static name => name["Probe.".Length..])
                .OrderBy(static name => name, StringComparer.Ordinal));
    }

    [Fact]
    public void Case_4_Covers_A_Record_Member_The_Compiler_Supplies_And_Not_One_With_A_Body()
    {
        Assert.Equal(AssuranceExemption.CompilerSuppliedRecordOrEnumMember, Exemption("Carried.Value"));

        Assert.Equal(AssuranceExemption.None, Exemption("Carried.Doubled()"));
    }

    [Fact]
    public void Case_5_Covers_An_Override_Or_Operator_That_Only_Delegates()
    {
        Assert.Equal(AssuranceExemption.DelegatingOverrideOrOperator, Exemption("Pair.Equals(object?)"));
        Assert.Equal(AssuranceExemption.DelegatingOverrideOrOperator, Exemption("Pair.GetHashCode()"));
        Assert.Equal(AssuranceExemption.DelegatingOverrideOrOperator, Exemption("Pair.operator ==(Pair, Pair)"));

        // The qualifier is the case. An Equals that compares the fields itself is a decision about
        // equality, and a decision is what the system exists to put in front of a human.
        Assert.Equal(AssuranceExemption.None, Exemption("Pair.Equals(Pair)"));
        Assert.Equal(AssuranceExemption.None, Exemption("Pair.operator +(Pair, Pair)"));

        // And a negation is not a delegation. `!left.Equals(right)` is the opposite answer to the
        // one Equals gives, which is the entire content of an inequality operator; while `!` was
        // whitelisted, every operator != in the component was exempt and dropping the `!` moved
        // nothing.
        Assert.Equal(AssuranceExemption.None, Exemption("Pair.operator !=(Pair, Pair)"));
    }

    [Fact]
    public void Case_6_Covers_Everything_Inside_An_AssemblyMarker()
    {
        Assert.Equal(AssuranceExemption.InsideAssemblyMarker, Exemption("AssemblyMarker.Describe(int)"));
    }

    [Fact]
    public void The_Escape_Hatch_Exempts_One_Unit_And_Requires_A_Reason()
    {
        const string Hatched = """
            namespace Probe;

            public sealed class Hatched
            {
                // A reason carries no semicolon: `;` separates fields on the AI line, so a reason
                // containing one is a parse failure rather than a longer reason.
                // Broiler-AI:           EXEMPT=Generated marshalling shim - reviewed at the generator
                // Broiler-Human:        PENDING
                public int Shim(int[] values)
                {
                    var total = 0;

                    foreach (var value in values)
                    {
                        total += value;
                    }

                    return total;
                }
            }
            """;

        var unit = AssuranceProbe.Unit(Hatched, "Hatched.Shim(int[])");

        Assert.Equal(AssuranceExemption.DeclaredInSource, unit.Exemption);
        Assert.Equal(AssuranceReviewState.Exempt, unit.State);
        Assert.Empty(unit.Annotation!.VocabularyProblems());

        const string Reasonless = """
            namespace Probe;

            public sealed class Hatched
            {
                // Broiler-AI:           EXEMPT=
                // Broiler-Human:        PENDING
                public int Shim(int value) { return value + 1; }
            }
            """;

        Assert.NotEmpty(AssuranceProbe.Unit(Reasonless, "Hatched.Shim(int)").Annotation!.VocabularyProblems());
    }

    /// <summary>
    /// The predicate classifies over the real checkout, and the covered set is the set of product
    /// projects ON DISK.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The second half used to be a tautology. It compared <c>CoveredAssemblies</c> against the
    /// distinct assemblies of the scanned units - and the scanned units are produced by filtering
    /// the project list THROUGH <c>CoveredAssemblies</c>, so the two sides were the same list
    /// arriving by two routes and the assertion could not fail. Deleting a name from
    /// <c>CoveredAssemblies</c> would have removed a whole product assembly from every rule in
    /// group J with this test green: no unannotated unit to report, no header to regenerate, no
    /// manifest entry to miss.
    /// </para>
    /// <para>
    /// So the expected set is read off disk instead: every <c>*.csproj</c> under <c>src/</c> that
    /// is not under <c>src/tests/</c> is a product project, and its assembly must be covered. That
    /// is an independent source - a fourth product project appearing in the tree fails here until
    /// someone decides whether it is covered, which is the decision this assertion exists to force.
    /// </para>
    /// </remarks>
    [Fact]
    public void The_Predicate_Answers_Both_Ways_Over_The_Real_Product_Tree()
    {
        // A predicate that exempted everything, or nothing, would satisfy every case above only by
        // accident of the probe. Over the checkout it must do both, or it is not classifying.
        Assert.NotEmpty(AssuranceScanner.Units.Where(static unit => unit.IsRelevant));
        Assert.NotEmpty(AssuranceScanner.Units.Where(static unit => unit.IsExempt));

        // The product projects on disk, found without going through the covered list.
        var onDisk = Directory
            .EnumerateFiles(Path.Combine(ComponentGraph.Root, "src"), "*.csproj", SearchOption.AllDirectories)
            .Select(static path => Path.GetRelativePath(ComponentGraph.Root, path).Replace('\\', '/'))
            .Where(static path => !path.StartsWith("src/tests/", StringComparison.Ordinal))

            // A composition root is not a product project - ADR 0001 revision 1 puts it in its own
            // partition - so it is not assurance-covered either. The decision this assertion
            // exists to force was made when the directory was authorised: what a composition root
            // contains is a host's own wiring, published and run rather than shipped as a package,
            // and annotating it would claim a review obligation over code no consumer receives.
            .Where(static path => !path.StartsWith("src/compositions/", StringComparison.Ordinal))
            .Select(static path => Path.GetFileNameWithoutExtension(path))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        // Six product projects at JS-0: the three the core packs, and the three project shells
        // the JavaScript profile family adds. The literal is the point of this assertion - a
        // seventh product project appearing in the tree fails here until someone decides whether
        // it is covered, and that decision is what the number records having been made.
        Assert.Equal(6, onDisk.Length);

        // The covered list is exactly those projects...
        Assert.Equal(
            onDisk,
            AssuranceSources.CoveredAssemblies.OrderBy(static name => name, StringComparer.Ordinal));

        // ...and the scan reached every one of them, so the source set is not merely declared.
        Assert.Equal(
            onDisk,
            AssuranceScanner.Units
                .Select(static unit => unit.File.Assembly)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static name => name, StringComparer.Ordinal));
    }

    /// <summary>
    /// The predicate, on the units where getting it wrong would cost the most: the allocation
    /// guard and the one member of <c>VmReadBounds</c> that decides something.
    /// </summary>
    /// <remarks>
    /// These are the first units the component annotated, and they are held here rather than left
    /// to rule J1's aggregate. J1 asserts that every RELEVANT unit is annotated; if the predicate
    /// wrongly exempted one of these, J1 would stay green and say nothing - which is exclusion
    /// EX-62 in the concrete. Naming them keeps the two shapes that matter most under a check
    /// that does not depend on the predicate agreeing with itself.
    /// </remarks>
    [Fact]
    public void The_Units_That_Touch_Untrusted_Input_Are_Relevant_And_Annotated()
    {
        var allocator = Units("src/Broiler.VM.Binary/VmBoundedAllocator.cs");

        foreach (var name in new[] { "TryAllocate<T>", "TryAllocateExact<T>" })
        {
            var unit = allocator.Single(unit => unit.Name.Contains(name, StringComparison.Ordinal));

            Assert.True(unit.IsRelevant, $"{unit.Name} should be relevant.");
            Assert.NotNull(unit.Annotation);
            Assert.Equal(AssuranceReviewState.HumanPending, unit.State);
        }

        var bounds = Units("src/Broiler.VM.Binary/VmReadBounds.cs");

        // The three units of VmReadBounds that decide something. Equals compares the four ceilings
        // itself rather than handing off, so case 5 does not reach it; operator != negates the
        // answer Equals gives, which is the opposite decision and not a delegation to it; and the
        // TYPE DECLARATION HEADER is a unit in its own right, because a primary constructor is
        // declared in one and this is the type whose four ceilings round one permuted.
        var deciding = new[]
            {
                "Equals(VmReadBounds)",
                "operator !=(VmReadBounds, VmReadBounds)",
                "Broiler.VM.VmReadBounds",
            }
            .Select(name => bounds.Single(unit => unit.Name.EndsWith(name, StringComparison.Ordinal)))
            .ToArray();

        Assert.All(deciding, static unit =>
        {
            Assert.True(unit.IsRelevant, $"{unit.Name} should be relevant.");
            Assert.NotNull(unit.Annotation);
            Assert.Equal(AssuranceReviewState.HumanPending, unit.State);
        });

        // Everything else in that file is exempt by the predicate, so the annotations on it leave
        // nothing unaccounted for. What is exempt is not unwatched: the manifest carries a
        // fingerprint for every one of them, which is rule J7.
        Assert.Empty(bounds
            .Where(unit => !deciding.Contains(unit))
            .Where(static unit => unit.IsRelevant)
            .Select(static unit => unit.Where));

        Assert.Equal(
            bounds.Count,
            AssuranceManifest.Entries(bounds).Count);

        // The post-generation unit set, so this reads the same in both modes: the gate
        // separately asserts that the tree on disk is what the generator would write.
        static IReadOnlyList<AssuranceUnit> Units(string path) =>
            AssuranceGenerator.Current.Units.Where(unit =>
                string.Equals(unit.File.RelativePath, path, StringComparison.Ordinal)).ToArray();
    }
}
