using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rules N20 and N21: the host surface is never ambient, and every crossing of it is charged.
/// </summary>
/// <remarks>
/// <para>
/// <b>These are the two properties the host surface's whole argument rests on, and neither is
/// visible at a call site.</b> The design's claim is that a composition's registration is still the
/// permission even though the traffic no longer goes through the capability table. That claim is
/// true only while two things stay true of the source, and both are the kind of thing a later
/// change breaks by kindness rather than by carelessness.
/// </para>
/// <para>
/// <b>N20 is the one that would break by convenience.</b> A static or thread-static field holding
/// the surface or a realm reads like an obvious simplification the day somebody needs a realm
/// somewhere the parameter did not reach. What it would actually do is make one composition's
/// embedder reachable from another composition's realm, in a process that hosts two - which is
/// exactly the ambient platform surface the capability table exists to prevent, reintroduced
/// underneath it where no registration can see it.
/// </para>
/// <para>
/// <b>N21 is the one that would break by omission.</b> A new member added to the realm without its
/// entry charge is a member that works, passes every behavioural check, and costs a guest nothing -
/// so the failure is invisible until an embedder moves real traffic across it and a host-call
/// ceiling turns out to bound something other than host calls. The charge is at the top of every
/// member deliberately, so a scan can ask for it, and this rule is what asks.
/// </para>
/// </remarks>
public sealed class HostSurfaceRuleTests
{
    private const string Assembly = "Broiler.VM.Profile.JavaScript";

    /// <summary>A field declaration that is static, in any of the spellings C# allows.</summary>
    private static readonly Regex AmbientField = new(
        @"(\[\s*System\.ThreadStatic\s*\]|\[\s*ThreadStatic\s*\])|(\b(private|internal|public|protected)\s+static\s+(readonly\s+)?[A-Za-z0-9_.<>?]*\b(IJsHostSurface|JsHostRealm)\b)",
        RegexOptions.Compiled);

    /// <summary>A public member of the realm, and whatever its body opens with.</summary>
    /// <remarks>
    /// It matches the declaration and then the first non-blank line of the body, which is where the
    /// charge has to be. A member whose body opens with anything else is what the rule reports.
    /// </remarks>
    private static readonly Regex PublicMember = new(
        @"\n    public (?!static JsHostValue (Missing|Undefined|Null)\b)[A-Za-z0-9_.<>?\[\]]+ (?<name>[A-Za-z0-9_]+)\s*(\([^)]*\)|\{[\r\n\s]*get)[^\{]*\{\s*(?<first>[^\r\n]*)",
        RegexOptions.Compiled);

    private static IEnumerable<AssuranceSourceFile> ProfileFiles =>
        AssuranceSources.Files.Where(static file => file.Assembly == Assembly);

    private static string TextOf(string name) =>
        File.ReadAllText(Path.Combine(
            ComponentGraph.Root,
            ProfileFiles.Single(file => file.RelativePath.EndsWith("/" + name, StringComparison.Ordinal))
                .RelativePath.Replace('/', Path.DirectorySeparatorChar)));

    /// <summary>N20's clauses over this checkout.</summary>
    private static IEnumerable<(string Id, Func<IEnumerable<string>> Run)> AmbientClauses =>
    [
        ("N20.1", NoAmbientHolder),
        ("N20.2", TheSurfaceIsReachedThroughAParameter),
    ];

    /// <summary>N21's clauses over this checkout.</summary>
    private static IEnumerable<(string Id, Func<IEnumerable<string>> Run)> ChargeClauses =>
    [
        ("N21.1", EveryPublicMemberCharges),
        ("N21.2", TheChargeReachesTheHostCallDimension),
    ];

    /// <summary>No source in the profile family parks a surface or a realm in static state.</summary>
    private static IEnumerable<string> NoAmbientHolder()
    {
        var scanned = 0;

        foreach (var file in ProfileFiles)
        {
            var text = File.ReadAllText(Path.Combine(
                ComponentGraph.Root, file.RelativePath.Replace('/', Path.DirectorySeparatorChar)));

            scanned++;

            foreach (Match match in AmbientField.Matches(text))
            {
                if (!match.Value.Contains("IJsHostSurface", StringComparison.Ordinal) &&
                    !match.Value.Contains("JsHostRealm", StringComparison.Ordinal))
                {
                    continue;
                }

                yield return
                    $"{file.RelativePath} holds a host surface or a realm in static state as " +
                    $"`{match.Value.Trim()}`, which would make one composition's embedder reachable " +
                    "from another composition's realm";
            }
        }

        // THE RULE REPORTS ITS OWN VACUITY, because it passes by finding nothing and a scan over an
        // empty file set would pass for the wrong reason.
        if (scanned == 0)
        {
            yield return "N20 scanned no source at all, so it judged nothing";
        }
    }

    /// <summary>A callback receives its realm, rather than resolving one from anywhere.</summary>
    private static IEnumerable<string> TheSurfaceIsReachedThroughAParameter()
    {
        var text = TextOf("JsHostValue.cs");

        if (!text.Contains("JsHostRealm realm, JsHostValue thisValue", StringComparison.Ordinal))
        {
            yield return
                "the host function delegate does not take its realm as a parameter, so a body " +
                "would have to resolve one from somewhere ambient";
        }

        if (!text.Contains("void OnRealmCreated(JsHostRealm realm)", StringComparison.Ordinal))
        {
            yield return
                "the surface is not handed its realm as a parameter, so a composition would have " +
                "to reach one by another route";
        }
    }

    /// <summary>Every public member of the realm opens by entering the charge.</summary>
    private static IEnumerable<string> EveryPublicMemberCharges()
    {
        var text = TextOf("JsHostRealm.cs");
        var matched = 0;

        foreach (Match member in PublicMember.Matches(text))
        {
            matched++;

            var first = member.Groups["first"].Value.Trim();

            if (first.StartsWith("Enter(", StringComparison.Ordinal))
            {
                continue;
            }

            yield return
                $"JsHostRealm.{member.Groups["name"].Value} opens with `{first}` rather than with " +
                "the entry charge, so a crossing through it costs a guest nothing";
        }

        if (matched == 0)
        {
            yield return
                "N21 matched no public member of JsHostRealm, so it judged nothing - which is the " +
                "failure mode a rule that passes by finding nothing has";
        }
    }

    /// <summary>The entry charge reaches the host-call dimension and not fuel alone.</summary>
    /// <remarks>
    /// Fuel alone would be the easy mistake: it is the dimension every other engine operation
    /// charges, and charging it would look right. What it would miss is that a host-call ceiling is
    /// how a composition bounds how often a guest reaches its own code, and a surface that charged
    /// no host call would leave that ceiling bounding the capability table only.
    /// </remarks>
    private static IEnumerable<string> TheChargeReachesTheHostCallDimension()
    {
        var engine = TextOf("JsEngine.cs");
        var realm = TextOf("JsHostRealm.cs");

        if (!engine.Contains("VmBudgetDimension.HostCalls", StringComparison.Ordinal) ||
            !engine.Contains("internal void ChargeHostCrossing(ulong units)", StringComparison.Ordinal))
        {
            yield return
                "the engine declares no host-crossing charge, so the realm's entry cannot reach " +
                "the host-call dimension";
        }

        if (!realm.Contains("engine.ChargeHostCrossing(units)", StringComparison.Ordinal))
        {
            yield return "the realm's entry does not reach the engine's host-crossing charge";
        }
    }

    /// <summary>Writes what N20 said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_N20_Are_Written_When_Asked_For()
    {
        RuleReport.Write("N20", AmbientClauses);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "N20.txt")),
                "a report for N20 was asked for and none was written");
        }
    }

    /// <summary>Writes what N21 said about this checkout, when asked to.</summary>
    [Fact]
    public void RuleMessages_For_N21_Are_Written_When_Asked_For()
    {
        RuleReport.Write("N21", ChargeClauses);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "N21.txt")),
                "a report for N21 was asked for and none was written");
        }
    }

    [Fact]
    public void N20_No_Host_Surface_Or_Realm_Is_Held_In_Static_State()
    {
        foreach (var (id, run) in AmbientClauses)
        {
            Assert.Empty(run().Select(message => $"{id}: {message}"));
        }
    }

    /// <summary>The rejecting direction, run over a witness rather than over a story about one.</summary>
    /// <remarks>
    /// The witness parks the current realm in a thread-static, which is the shape a later
    /// contributor produces the day a callback needs a realm somewhere the parameter did not reach.
    /// The real file passes the predicate the witness fails, which is what makes this a rule rather
    /// than a pair of strings.
    /// </remarks>
    [Fact]
    public void N20_Rejects_An_Ambient_Realm()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
            "N20-an-ambient-host-realm.cs.witness"));

        Assert.Matches(AmbientField, witness);
        Assert.DoesNotMatch(AmbientField, TextOf("JsHostRealm.cs"));
    }

    [Fact]
    public void N21_Every_Crossing_Of_The_Host_Surface_Is_Charged()
    {
        foreach (var (id, run) in ChargeClauses)
        {
            Assert.Empty(run().Select(message => $"{id}: {message}"));
        }
    }

    /// <summary>The rejecting direction: a member that acts before it charges.</summary>
    [Fact]
    public void N21_Rejects_A_Member_That_Acts_Before_It_Charges()
    {
        var witness = File.ReadAllText(Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses",
            "N21-a-crossing-that-charges-nothing.cs.witness"));

        var members = PublicMember.Matches(witness);

        Assert.NotEmpty(members);
        Assert.Contains(
            members.Cast<Match>(),
            static member => !member.Groups["first"].Value.Trim()
                .StartsWith("Enter(", StringComparison.Ordinal));
    }
}
