using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The fingerprint's two obligations, each asserted against real edits rather than described.
/// </summary>
/// <remarks>
/// <para>
/// The policy asks for both halves at once: "any semantically relevant source modification must
/// invalidate the previous review", and "avoid invalidating reviews merely because
/// <c>dotnet format</c> changed whitespace". A hash meets one of those by accident; meeting both
/// is the design, so both are witnessed here by one probe method edited five ways.
/// </para>
/// <para>
/// Every variant below is the SAME method with ONE thing changed, so a failure names the change
/// rather than the file. The normalized token stream is asserted alongside the hash in the
/// reformatting case, because "the hash did not move" is a much weaker statement than "the thing
/// the hash is taken over did not move", and only the second explains why.
/// </para>
/// </remarks>
public sealed class AssuranceFingerprintTests
{
    private const string Member = "Admit";

    /// <summary>The unedited probe. Every other constant is this one with a single difference.</summary>
    private const string Original = """
        namespace Probe;

        public static class Gate
        {
            public static bool Admit(uint declared, uint bound, out int taken)
            {
                // The count is checked before anything is spent on it.
                var request = declared;
                var ceiling = bound;

                if (request > ceiling)
                {
                    taken = 0;
                    return false;
                }

                taken = (int)request;
                return true;
            }
        }
        """;

    /// <summary>`declared` becomes `count`, at the declaration and at its one use.</summary>
    private const string RenamedParameter = """
        namespace Probe;

        public static class Gate
        {
            public static bool Admit(uint count, uint bound, out int taken)
            {
                // The count is checked before anything is spent on it.
                var request = count;
                var ceiling = bound;

                if (request > ceiling)
                {
                    taken = 0;
                    return false;
                }

                taken = (int)request;
                return true;
            }
        }
        """;

    /// <summary>The refusal path now yields 1 rather than 0.</summary>
    private const string ChangedLiteral = """
        namespace Probe;

        public static class Gate
        {
            public static bool Admit(uint declared, uint bound, out int taken)
            {
                // The count is checked before anything is spent on it.
                var request = declared;
                var ceiling = bound;

                if (request > ceiling)
                {
                    taken = 1;
                    return false;
                }

                taken = (int)request;
                return true;
            }
        }
        """;

    /// <summary>The two independent locals are declared in the other order.</summary>
    private const string ReorderedStatements = """
        namespace Probe;

        public static class Gate
        {
            public static bool Admit(uint declared, uint bound, out int taken)
            {
                // The count is checked before anything is spent on it.
                var ceiling = bound;
                var request = declared;

                if (request > ceiling)
                {
                    taken = 0;
                    return false;
                }

                taken = (int)request;
                return true;
            }
        }
        """;

    /// <summary>A parameter's type widens. Nothing else moves.</summary>
    private const string ChangedType = """
        namespace Probe;

        public static class Gate
        {
            public static bool Admit(uint declared, ulong bound, out int taken)
            {
                // The count is checked before anything is spent on it.
                var request = declared;
                var ceiling = bound;

                if (request > ceiling)
                {
                    taken = 0;
                    return false;
                }

                taken = (int)request;
                return true;
            }
        }
        """;

    /// <summary>What a formatter does: braces, indentation and blank lines, and nothing else.</summary>
    private const string Reformatted = """
        namespace Probe;

        public static class Gate
        {
                public static bool Admit(uint declared, uint bound, out int taken) {
                        // The count is checked before anything is spent on it.
                        var request = declared;

                        var ceiling = bound;
                        if (request > ceiling) { taken = 0; return false; }
                        taken = (int)request;

                        return true;
                }
        }
        """;

    /// <summary>The comment says something else, and the code says exactly the same thing.</summary>
    private const string EditedComment = """
        namespace Probe;

        public static class Gate
        {
            /// <summary>Admits a declared count, or refuses it.</summary>
            // Broiler-AI:    Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=TBF
            // Broiler-Human: PENDING
            public static bool Admit(uint declared, uint bound, out int taken)
            {
                // Rewritten wholesale, and a second line added under it, because a comment is
                // not what a reviewer certifies.
                var request = declared;
                var ceiling = bound;

                if (request > ceiling)
                {
                    taken = 0;
                    return false;
                }

                taken = (int)request;
                return true;
            }
        }
        """;

    [Fact]
    public void A_Semantically_Relevant_Edit_Changes_The_Fingerprint()
    {
        var original = AssuranceProbe.Fingerprint(Original, Member);

        Assert.Equal(AssuranceFingerprint.Width, original.Length);

        var moved = new (string What, string Source)[]
        {
            ("a renamed parameter", RenamedParameter),
            ("a changed literal", ChangedLiteral),
            ("reordered statements", ReorderedStatements),
            ("a changed parameter type", ChangedType),
        };

        var unmoved = moved
            .Where(edit => string.Equals(
                AssuranceProbe.Fingerprint(edit.Source, Member), original, StringComparison.Ordinal))
            .Select(edit => $"{edit.What} left the fingerprint at {original}")
            .ToArray();

        Assert.Empty(unmoved);

        // The four edited fingerprints are also distinct from each other, so the assertion above
        // cannot be passed by a hash that merely reacts to the WORD "declared" appearing.
        var distinct = moved
            .Select(edit => AssuranceProbe.Fingerprint(edit.Source, Member))
            .Append(original)
            .Distinct(StringComparer.Ordinal);

        Assert.Equal(moved.Length + 1, distinct.Count());
    }

    [Fact]
    public void Reformatting_And_Editing_A_Comment_Leave_The_Fingerprint_Alone()
    {
        var original = AssuranceProbe.Fingerprint(Original, Member);

        // The token stream is asserted first. "The hash did not move" is compatible with the hash
        // reading nothing at all; "the normalized form did not move" says why it did not.
        Assert.Equal(
            AssuranceProbe.TokenStream(Original, Member),
            AssuranceProbe.TokenStream(Reformatted, Member));

        Assert.Equal(original, AssuranceProbe.Fingerprint(Reformatted, Member));

        // The edited-comment probe also carries the annotation block itself, which sits in the
        // declaration's leading trivia. A fingerprint that included its own annotation could never
        // be filled: writing the value would change the value.
        Assert.Equal(
            AssuranceProbe.TokenStream(Original, Member),
            AssuranceProbe.TokenStream(EditedComment, Member));

        Assert.Equal(original, AssuranceProbe.Fingerprint(EditedComment, Member));
    }

    [Fact]
    public void The_Normalized_Form_Carries_No_Trivia_And_Every_Token()
    {
        var stream = AssuranceProbe.TokenStream(Original, Member);

        Assert.DoesNotContain("//", stream, StringComparison.Ordinal);
        Assert.DoesNotContain("\n", stream, StringComparison.Ordinal);
        Assert.DoesNotContain("  ", stream, StringComparison.Ordinal);

        // Signature, parameter types, modifiers and body are all in it: the policy's list of what
        // a fingerprint may include, asserted rather than asserted-in-a-comment.
        foreach (var token in new[] { "public", "static", "bool", "Admit", "uint", "out", "int", "return", "true" })
        {
            Assert.Contains(token, stream, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void A_Fingerprint_Is_Six_Uppercase_Hex_Characters()
    {
        foreach (var source in new[] { Original, RenamedParameter, ChangedLiteral, ReorderedStatements })
        {
            var fingerprint = AssuranceProbe.Fingerprint(source, Member);

            Assert.True(
                AssuranceFingerprint.IsWellFormed(fingerprint),
                $"'{fingerprint}' is not six uppercase hex characters.");
        }

        Assert.False(AssuranceFingerprint.IsWellFormed("7a91c2"));
        Assert.False(AssuranceFingerprint.IsWellFormed("7A91C"));
        Assert.False(AssuranceFingerprint.IsWellFormed(AssuranceFingerprint.ToBeFilled));
    }

    /// <summary>
    /// The mechanism that excludes trivia is <c>SyntaxToken.Text</c>, and this is the test that
    /// says so.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The chain used to open with <c>WithoutTrivia()</c>, and the class doc, the component's
    /// assurance specification and the register row all named that call as the thing that keeps
    /// <c>dotnet format</c> from invalidating a review. It was dead: the stream is built from
    /// <c>token.Text</c>, which is the token's own characters and never the trivia around them, so
    /// the call could be deleted with every fingerprint unchanged and nothing red. A decoy is worse
    /// than a redundancy - a later change to <c>token.ToFullString()</c> would have made it
    /// load-bearing, and nothing in the suite would have noticed it had been removed in between.
    /// </para>
    /// <para>
    /// So the call is gone and the real mechanism is pinned here rather than described. The
    /// declaration below is rebuilt with a comment and newlines attached to its own tokens, which
    /// is the state <c>WithoutTrivia()</c> used to clear, and the stream must be unchanged. Under
    /// <c>ToFullString()</c> this fails and names the comment it found.
    /// </para>
    /// </remarks>
    [Fact]
    public void The_Exclusion_Of_Trivia_Is_Token_Text()
    {
        var bare = AssuranceProbe.Named(Original, Member).Declaration;

        var trivial = bare.ReplaceTokens(
            bare.DescendantTokens(),
            static (token, _) => token
                .WithLeadingTrivia(SyntaxFactory.Comment("/* a formatter put this here */"))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed));

        // The node really does carry the trivia now, or the assertion under it proves nothing.
        Assert.Contains("a formatter put this here", trivial.ToFullString(), StringComparison.Ordinal);

        Assert.Equal(
            AssuranceFingerprint.TokenStream(bare),
            AssuranceFingerprint.TokenStream(trivial));

        Assert.Equal(AssuranceFingerprint.Of(bare), AssuranceFingerprint.Of(trivial));
        Assert.DoesNotContain(
            "formatter", AssuranceFingerprint.TokenStream(trivial), StringComparison.Ordinal);
    }

    [Fact]
    public void The_Fingerprint_Is_Stable_Across_Runs()
    {
        // A hash seeded per process would pass every test above and be useless: a review recorded
        // yesterday would read as stale today. SHA-256 over UTF-8 is not, and this pins the exact
        // value so a change of algorithm has to be a deliberate edit to this line.
        Assert.Equal(
            AssuranceProbe.Fingerprint(Original, Member),
            AssuranceProbe.Fingerprint(Original, Member));

        // SHA-256 of the seven-token stream "public void Nothing ( ) { }", first six hex digits.
        Assert.Equal("AF9E13", Fingerprint("public void Nothing() { }"));

        static string Fingerprint(string member) =>
            AssuranceProbe.Fingerprint($"namespace Probe;\n\npublic class Probe\n{{\n    {member}\n}}\n", "Nothing()");
    }

    /// <summary>
    /// A property declaration's fingerprint includes its INITIALIZER.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>public static VmControlResult Accepted { get; } = new(VmControlOutcome.Accepted,
    /// VmReason.None);</c> is an auto-property, so the exemption predicate answers case 1 and it
    /// carries no annotation - but the initializer states a value the type hands every caller, and
    /// changing <c>VmReason.None</c> to <c>VmReason.ObjectDisposed</c> changes what ships.
    /// </para>
    /// <para>
    /// The fingerprint has to move for that edit, or the manifest entry rule J7 keeps for the unit
    /// would be the same before and after and the only record of the change would be silent. It
    /// does move, because the initializer's tokens are the declaration's tokens; this pins that,
    /// because the alternative - fingerprinting only the accessor list - would look identical on
    /// every other property in the tree.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Property_Initializer_Is_Part_Of_The_Declaration_It_Fingerprints()
    {
        const string Shipped = """
            namespace Probe;

            public sealed class Results
            {
                public static Verdict Accepted { get; } = new Verdict(Outcome.Accepted, Reason.None);
            }
            """;

        const string Changed = """
            namespace Probe;

            public sealed class Results
            {
                public static Verdict Accepted { get; } = new Verdict(Outcome.Accepted, Reason.ObjectDisposed);
            }
            """;

        // The unit is exempt, which is the point: nothing but the manifest records it.
        Assert.True(AssuranceProbe.Named(Shipped, "Accepted").IsExempt);

        Assert.Contains(
            "Reason . None",
            AssuranceProbe.TokenStream(Shipped, "Accepted"),
            StringComparison.Ordinal);

        Assert.NotEqual(
            AssuranceProbe.Fingerprint(Shipped, "Accepted"),
            AssuranceProbe.Fingerprint(Changed, "Accepted"));

        // ...and the two manifest entries differ in the fingerprint and in nothing else, so the
        // edit is one line of a diff rather than nothing at all.
        Assert.NotEqual(
            Entry(Shipped).Fingerprint,
            Entry(Changed).Fingerprint);

        Assert.Equal(Entry(Shipped).Name, Entry(Changed).Name);
        Assert.Equal(Entry(Shipped).Exemption, Entry(Changed).Exemption);

        static AssuranceManifestEntry Entry(string source) => AssuranceManifest
            .Entries(AssuranceProbe.Scan(source))
            .Single(static entry => entry.Name.EndsWith(".Accepted", StringComparison.Ordinal));
    }
}
