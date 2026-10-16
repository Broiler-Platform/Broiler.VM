using OwnerPin = Broiler.Unicode.Generation.UnicodePin;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule I4, data agreement: the pin this repository's embedded Unicode tables are generated from
/// equals the pin of the area's home, Broiler.Unicode, by hash, and every Unicode data version an
/// admitted library declares equals that pin's.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why a rule.</b> Decision SJL-D-6 (c), which JSD-0074 records for the Unicode area, leaves the
/// generator and the pin with the owner and has each engine embed the tables the owner's generator
/// writes from the engine's own pin. Rules N22 and N28 hold the embedded bytes to that generation;
/// nothing else would notice this repository's pin and the owner's drifting apart, after which the
/// owner's algorithms would read tables of another release than the one they were tested against.
/// The governance's row names it as a Broiler.VM pin differing from its home's by one hash, and a
/// library declaring another Unicode version.
/// </para>
/// <para>
/// <b>What it reads.</b> The owner's pin is the one its generator package carries, read through the
/// package's own <c>UnicodePin.Owner()</c>, so the comparison is with the release this repository
/// admits rather than with a checkout. The versions are read from the metadata of every admitted
/// assembly on the committed feed, without loading it. The CLDR, tzdb and calendar pins are their
/// areas' (SJL-8, SJL-9): their homes hold no pin yet, and the rule gains a clause for each when its
/// area moves.
/// </para>
/// </remarks>
public sealed class I4DataAgreementRuleTests
{
    private static string Witness(string name) => Path.Combine(WitnessAssemblies.Directory, name);

    [Fact]
    public void I4_The_Unicode_Pin_Equals_The_Owners_And_Every_Admitted_Version_The_Pin()
    {
        var family = OwnerPin.Parse(File.ReadAllText(Path.Combine(ComponentGraph.Root, UnicodePin.PinPath)));
        var owner = OwnerPin.Owner();

        // Not vacuous: both pins name every file the generator reads, the twenty-three UCD, emoji and
        // licence files and the three property tables, at the version the owner's algorithms read.
        Assert.Equal(26, family.Entries.Count);
        Assert.Equal(26, owner.Entries.Count);
        Assert.Equal(OwnerPin.PinnedVersion, family.Version);
        Assert.Empty(AdmissionRules.I4Pins(family, owner));

        // The versions: every admitted assembly that declares one is read, and the owner's generator
        // is among them.
        var declared = AdmissionRules.AdmittedUnicodeVersions();
        Assert.Contains(declared, static d => d.Assembly == "Broiler.Unicode.Generation" && d.Field == "Broiler.Unicode.Generation.UnicodePin.PinnedVersion");
        Assert.Contains(declared, static d => d.Assembly == "Broiler.ECMAScript.Algorithms");
        Assert.Empty(AdmissionRules.I4Versions(family.Version, declared));
    }

    [Fact]
    public void I4_A_Pin_Differing_By_One_Hash_And_A_Library_Declaring_Another_Version_Are_Reported()
    {
        var owner = OwnerPin.Owner();
        var differing = OwnerPin.Parse(File.ReadAllText(Witness("I4-a-pin-differing-from-the-owner-by-one-hash.pin.witness")));
        var reported = AdmissionRules.I4Pins(differing, owner).ToArray();

        // One file, and only that one.
        var single = Assert.Single(reported);
        Assert.Contains("ucd-17.0.0/CaseFolding.txt is 87539 bytes hashing to ff8d8fefbf123574205085d6714c36149eb946d717a0c585c27f0f4ef58c4180 here", single, StringComparison.Ordinal);

        var library = WitnessAssemblies.Compile(
            "Broiler.Unicode.Algorithms",
            File.ReadAllText(Witness("I4-a-library-declaring-another-unicode-version.cs.witness")));
        var declared = AdmissionRules.DeclaredUnicodeVersions("Broiler.Unicode.Algorithms", library);

        Assert.Equal(
            ["Broiler.Unicode.Algorithms declares Broiler.Unicode.Algorithms.UnicodeAlgorithms.UnicodeVersion = \"16.0.0\", and the pin names 17.0.0 (rule I4)"],
            AdmissionRules.I4Versions("17.0.0", declared));
    }
}
