// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// No product assembly of the JavaScript profile reaches the host platform's regular-expression
/// engine.
/// </summary>
/// <remarks>
/// <para>
/// <b>This rule exists because the matcher was a translation before it was an implementation, and
/// a translation looks exactly like an implementation from outside.</b> The realm's
/// <c>RegExp</c> was built on <c>System.Text.RegularExpressions</c> until the workload programme
/// replaced it: the two dialects agree on most patterns and disagree on the ones a guest is most
/// likely to write, so the Octane <c>regexp</c> benchmark ran, printed a score, and failed its own
/// checksum — which the ledger records as an approximation that neither waits nor excludes, and as
/// a surface answering wrongly instead of refusing.
/// </para>
/// <para>
/// <b>What the workload roadmap's JSW-4 asks for is not that the matcher exists but that the
/// closure contains no call site constructing one from the platform, ASSERTED BY A METADATA TEST.</b>
/// The matcher landing is a state of the tree that a later change can undo without anybody
/// noticing: a single <c>using System.Text.RegularExpressions;</c> in a String method would restore
/// exactly the dialect the programme spent a stage removing, and every test in this repository
/// would stay green. This is the assertion that clause names.
/// </para>
/// <para>
/// <b>It is over source text rather than over the compiled assemblies, and the reason is rule
/// A11.</b> A test project may not reference a profile assembly, so this cannot load one and read
/// its type references; what it can read is the source the assemblies are built from, which is the
/// same source the assurance scanner already parses for every other rule over this tree.
/// <b>The test half of the repository is deliberately out of scope</b>: the architecture suite
/// itself parses annotations with a platform regular expression, and a rule that forbade that
/// would be forbidding the tool rather than the product.
/// </para>
/// </remarks>
public sealed class N18PlatformMatcherRuleTests
{
    /// <summary>The profile's three product assemblies, which are the ones this rule is about.</summary>
    /// <remarks>
    /// The core's three are covered by the assurance scanner too and are not this rule's subject: a
    /// binary reader has no dialect to be wrong about, and JSW-4's clause is the profile's.
    /// </remarks>
    private static readonly string[] ProfileAssemblies =
    [
        "Broiler.VM.Profile.JavaScript",
        "Broiler.VM.Profile.JavaScript.Compiler",
        "Broiler.VM.Profile.JavaScript.Format",
    ];

    /// <summary>The spellings that reach the platform engine, whichever way a file names it.</summary>
    /// <remarks>
    /// Three rather than one, because a namespace import, a fully qualified name and a bare type
    /// name are three ways to write the same call and a rule that knew only the first would be
    /// defeated by a <c>using</c> at the top of the file.
    /// </remarks>
    private static readonly string[] Spellings =
    [
        "System.Text.RegularExpressions",
        "using System.Text.RegularExpressions;",
        "new Regex(",
    ];

    /// <summary>The profile's product source names the platform matcher nowhere.</summary>
    [Fact]
    public void N18_No_Product_Source_Of_This_Profile_Constructs_A_Platform_Regular_Expression()
    {
        var covered = Covered();

        // Non-vacuous in both of the ways this rule can be empty for the wrong reason: it passes by
        // finding nothing, so a renamed assembly or a moved path would be its cleanest-looking
        // result. All three assemblies have to be represented - the matcher is in the format one,
        // the literal that reaches it is compiled by the compiler one, and the realm that
        // constructs a `RegExp` is in the profile one - and the file the matcher lives in has to be
        // among them by name.
        Assert.Equal(
            ProfileAssemblies.Order(StringComparer.Ordinal),
            covered.Select(static file => file.Assembly).Distinct().Order(StringComparer.Ordinal));

        Assert.True(covered.Count > 60);

        Assert.Contains(
            covered,
            file => string.Equals(
                file.RelativePath,
                "src/Broiler.VM.Profile.JavaScript.Format/JsRegExpMatcher.cs",
                StringComparison.Ordinal));

        Assert.Empty(Violations(covered));
    }

    /// <summary>The rejecting direction, decided by an input rather than by a comment.</summary>
    /// <remarks>
    /// The witness is a source file that constructs a platform matcher in the shape a String method
    /// would. It is held under <c>witnesses/register</c> rather than in a product project, because
    /// a file that would make this rule fail cannot live where the rule looks.
    /// </remarks>
    [Fact]
    public void N18_A_Source_That_Reaches_The_Platform_Engine_Is_Reported()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root,
            "src/tests/Broiler.VM.Architecture.Tests/witnesses/register",
            "N18-a-product-source-that-constructs-a-platform-matcher.cs.witness"));

        var reported = Spellings.Where(spelling =>
            witness.Contains(spelling, StringComparison.Ordinal)).ToArray();

        // All three spellings, because the witness writes all three: a rule that caught only the
        // one a reader thought of first would report this file and miss the next one.
        Assert.Equal(3, reported.Length);
    }

    /// <summary>The profile's covered product sources.</summary>
    private static IReadOnlyList<AssuranceSourceFile> Covered() =>
        AssuranceSources.Files
            .Where(static file => ProfileAssemblies.Contains(file.Assembly, StringComparer.Ordinal))
            .ToArray();

    /// <summary>Each file that names the platform engine, and which spelling it used.</summary>
    private static IEnumerable<string> Violations(IEnumerable<AssuranceSourceFile> covered) =>
        from file in covered
        from spelling in Spellings
        where file.Text.Contains(spelling, StringComparison.Ordinal)
        select $"{file.RelativePath} names `{spelling}`: the profile owns its matcher, and a call " +
            "site reaching the platform's engine restores the dialect JSW-4 removed";
}
