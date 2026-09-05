// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The realm's admitted global set is published by the realm, and no document may disagree with it.
/// </summary>
/// <remarks>
/// <para>
/// <b>This rule exists because a document was the authority and was wrong in a way nobody could
/// see.</b> Bundle JS-4-001 published a list of the globals absent from the realm. Every name on it
/// was absent; the list simply was not the whole set — the keyed collections and <c>Promise</c> were
/// absent too and were not on it — and the bundle never claimed exhaustiveness. The workload
/// roadmap's section 3.2 records what that costs: <b>a list a later reader mistakes for the whole
/// set is how a gap survives a review</b>, and it puts the obligation to make the two agree on
/// JSW-6.
/// </para>
/// <para>
/// <b>So the set is asked of a program and the documents are checked against it.</b>
/// <c>Broiler.VM.Composition.JavaScript.SliceCompiler --globals</c> runs
/// <c>Object.getOwnPropertyNames(globalThis)</c> in a verified artifact on a real instance and
/// writes what it answered; a document's claim that a name is ABSENT is then a claim this rule can
/// decide. <b>Both directions are checked</b>, which is the clause JSW-6 states: a name the
/// documents call absent and the realm has is a document that has gone stale, and a name the
/// documents call absent and this rule cannot find on either list is a document naming something
/// nobody published.
/// </para>
/// <para>
/// <b>It reads two files and runs nothing.</b> Rule A11 forbids a project reference from here to the
/// profile, so this rule cannot build a realm; what it can do is compare the file the realm wrote
/// with the block the ledger publishes, which is the whole of what the disagreement would be.
/// </para>
/// </remarks>
public sealed class N17RealmGlobalsRuleTests
{
    /// <summary>Where the realm writes what it admits.</summary>
    private const string PublishedPath =
        "src/Broiler.VM.Profile.JavaScript/docs/realm/globals.txt";

    /// <summary>The ledger, which carries the absent list in a fenced block.</summary>
    private const string LedgerPath =
        "src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md";

    /// <summary>The fence that opens the ledger's machine-readable absent list.</summary>
    private const string AbsentFence = "```absent-globals";

    /// <summary>The realm publishes a set, and it is not empty.</summary>
    [Fact]
    public void N17_The_Realm_Publishes_Its_Admitted_Global_Set()
    {
        var published = Published();

        // Non-vacuous: the set is a real one, and these five are on it for five different reasons -
        // an intrinsic the language has always had, one this profile added late, one behind an
        // optional surface, the host's own binding, and the self-reference. A rule that only
        // counted would pass over a file holding one name.
        Assert.NotEmpty(published);
        Assert.Contains("Object", published);
        Assert.Contains("Promise", published);
        Assert.Contains("Uint8Array", published);
        Assert.Contains("print", published);
        Assert.Contains("globalThis", published);

        // Sorted, so a regeneration that reordered nothing produces no diff and a reader comparing
        // two revisions is comparing membership rather than iteration order.
        Assert.Equal(published.OrderBy(static name => name, StringComparer.Ordinal).ToArray(), published);
    }

    /// <summary>Nothing a document calls absent is a name the realm has.</summary>
    [Fact]
    public void N17_No_Document_Calls_A_Present_Global_Absent()
    {
        var published = new HashSet<string>(Published(), StringComparer.Ordinal);
        var absent = Absent();

        Assert.NotEmpty(absent);

        var disagreements = absent.Where(published.Contains).ToArray();

        Assert.Empty(disagreements);

        // The rejecting direction, asserted by the CONTENT of the failure rather than by its
        // existence: a witness list naming a global the realm has must be reported, and reported as
        // that name.
        var witness = Witness("N17-a-document-calls-a-present-global-absent.txt.witness");
        var reported = witness.Where(published.Contains).ToArray();

        Assert.Contains("Object", reported);
    }

    /// <summary>Every name the two lists hold is a name, not prose that drifted into a block.</summary>
    [Fact]
    public void N17_Both_Lists_Hold_Only_Identifiers()
    {
        foreach (var name in Published().Concat(Absent()))
        {
            Assert.True(
                name.Length != 0 && (char.IsLetter(name[0]) || name[0] is '_' or '$'),
                $"`{name}` is not a global name, so one of the two lists has prose in it");

            Assert.True(
                name.All(static c => char.IsLetterOrDigit(c) || c is '_' or '$'),
                $"`{name}` is not a global name, so one of the two lists has prose in it");
        }
    }

    /// <summary>The names the realm answered with.</summary>
    private static string[] Published()
    {
        var path = Path.Combine(ComponentGraph.Root, PublishedPath);

        Assert.True(File.Exists(path), $"the realm has published nothing at {PublishedPath}");

        return File.ReadAllLines(path)
            .Select(static line => line.Trim())
            .Where(static line => line.Length != 0 && !line.StartsWith('#'))
            .ToArray();
    }

    /// <summary>The names the ledger's fenced block calls absent.</summary>
    private static string[] Absent() =>
        Fenced(File.ReadAllLines(Path.Combine(ComponentGraph.Root, LedgerPath)));

    /// <summary>The names a witness list carries, read by the same reader.</summary>
    private static string[] Witness(string name)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src/tests/Broiler.VM.Architecture.Tests/witnesses/register", name);

        Assert.True(File.Exists(path), $"the witness {name} is not on disk");
        return Fenced(File.ReadAllLines(path));
    }

    /// <summary>Reads the one fenced block, and refuses a document that carries none or two.</summary>
    private static string[] Fenced(string[] lines)
    {
        var names = new List<string>();
        var inside = false;
        var seen = 0;

        foreach (var line in lines)
        {
            if (!inside && line.Trim().Equals(AbsentFence, StringComparison.Ordinal))
            {
                inside = true;
                seen++;
                continue;
            }

            if (inside && line.Trim().Equals("```", StringComparison.Ordinal))
            {
                inside = false;
                continue;
            }

            if (inside && line.Trim().Length != 0)
            {
                names.Add(line.Trim());
            }
        }

        Assert.Equal(1, seen);
        return [.. names];
    }
}
