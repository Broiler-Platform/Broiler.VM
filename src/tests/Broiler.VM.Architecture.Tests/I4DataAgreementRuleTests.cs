using HomeCalendarPin = Broiler.ECMAScript.Generation.Time.CalendarPin;
using OwnerPin = Broiler.Unicode.Generation.UnicodePin;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule I4, data agreement: the pin this repository's embedded Unicode tables are generated from
/// equals the pin of the area's home, Broiler.Unicode, by hash, and every Unicode data version an
/// admitted library declares equals that pin's; and, since SJL-8.5, the tzdb, calendar and CLDR time
/// zone key pins the embedded time zone and calendar tables are generated from equal the time data
/// home's, and every time data version the home's generator declares equals the version they state.
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
/// <para>
/// <b>The time data's clauses (added 2026-10-17, at SJL-8.5, JSD-0076).</b> The tzdb and calendar pins
/// and CLDR's time zone keys moved to the Broiler.JS repository, whose generator package carries no
/// pin file, so the home's pins are constants of the rule, each with the commit it was read at
/// (<see cref="AdmissionRules.TimeHomeCommit"/>): the home's tzdb and calendar pins are byte-identical
/// copies of this repository's and are held by length and SHA-256, and the home's CLDR pin names one of
/// this repository's 114 CLDR files, which is held by its entry. The versions are the four data
/// versions the generator declares in <c>TimeDataVersions</c> and the two its pin readers declare, read
/// from the admitted package's metadata and held to the release this repository's tzdb pin states, the
/// CLDR release its CLDR pin states, and the crate versions its pinned crates' <c>Cargo.toml</c> state.
/// The CLDR pin's other 113 files are rule N27's, and remain this repository's alone until SJL-9.
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

    [Fact]
    public void I4_The_Time_Pins_Equal_The_Homes_And_Every_Declared_Time_Version_The_Pins()
    {
        var tzdb = File.ReadAllBytes(Path.Combine(ComponentGraph.Root, TimeTableEmbedding.TzdbPinPath));
        var calendars = File.ReadAllBytes(Path.Combine(ComponentGraph.Root, TimeTableEmbedding.CalendarPinPath));
        var cldr = CldrPin.Load();

        // Not vacuous: the two pins are read whole, and the CLDR pin names the time zone keys among its
        // 114 files.
        Assert.Equal(AdmissionRules.TimeHomeTzdbPin.Bytes, tzdb.LongLength);
        Assert.Equal(AdmissionRules.TimeHomeCalendarPin.Bytes, calendars.LongLength);
        Assert.Contains(cldr.Entries, static entry => entry.Name == AdmissionRules.TimeHomeCldrTimeZoneKeys.Name);
        Assert.Empty(AdmissionRules.I4TimePins(tzdb, calendars, CldrEntries(cldr)));

        // The versions: the generator's four TimeDataVersions constants and its two pin readers'
        // PinnedVersion constants, and nothing else of an admitted package, each held to its pin.
        var pinned = PinnedTimeVersions();
        Assert.Equal(
            new Dictionary<string, string>(StringComparer.Ordinal) { ["tzdb"] = "2026e", ["cldr"] = "48.2.0", ["icu_calendar"] = "2.3.0", ["calendrical_calculations"] = "0.2.4" },
            pinned);

        var declared = AdmissionRules.AdmittedTimeVersions();
        Assert.Equal(
            AdmissionRules.TimeVersionFields.Keys.Order(StringComparer.Ordinal),
            declared.Select(static d => d.Field).Order(StringComparer.Ordinal));
        Assert.All(declared, static d => Assert.Equal("Broiler.ECMAScript.Generation", d.Assembly));
        Assert.Empty(AdmissionRules.I4TimeVersions(pinned, declared));

        // The two pin readers' PinnedVersion are time versions, and the Unicode clause does not read them.
        Assert.DoesNotContain(AdmissionRules.AdmittedUnicodeVersions(), static d =>
            d.Field == "Broiler.ECMAScript.Generation.Time.CldrPin.PinnedVersion"
            || d.Field == "Broiler.ECMAScript.Generation.Time.TzdbPin.PinnedVersion"
            || d.Field == "Broiler.ECMAScript.Generation.Intl.IntlCldrPin.PinnedVersion");
        Assert.Contains(AdmissionRules.AdmittedUnicodeVersions(), static d =>
            d.Field == "Broiler.ECMAScript.Generation.Intl.IntlUcdPin.PinnedVersion" && d.Value == "17.0.0");
    }

    [Fact]
    public void I4_A_Time_Pin_Differing_From_The_Homes_And_A_Generator_Declaring_Another_Release_Are_Reported()
    {
        var tzdb = File.ReadAllBytes(Path.Combine(ComponentGraph.Root, TimeTableEmbedding.TzdbPinPath));
        var calendars = File.ReadAllBytes(Path.Combine(ComponentGraph.Root, TimeTableEmbedding.CalendarPinPath));
        var cldr = CldrEntries(CldrPin.Load()).ToArray();

        // The tzdb pin with one digit of one member's hash changed: that pin alone is reported.
        var differing = File.ReadAllBytes(Witness("I4-a-tzdb-pin-differing-from-the-home-by-one-hash.pin.witness"));
        var single = Assert.Single(AdmissionRules.I4TimePins(differing, calendars, cldr));
        Assert.StartsWith("src/tests/tzdb/pins/tzdb.pin is 2775 bytes hashing to ", single, StringComparison.Ordinal);
        Assert.Contains("the time data home's copy is 2775 bytes hashing to ef9c7443115a802db68895b29ad6c584ddec666280120b61e38061fcf5215f6f at Broiler.JS 2d37a38a", single, StringComparison.Ordinal);

        // The same of the calendar pin, made in memory from the real one, and of the CLDR pin's entry.
        var changed = (byte[])calendars.Clone();
        var at = Array.IndexOf(changed, (byte)'a', changed.Length / 2);
        changed[at] = (byte)'b';
        Assert.Contains("src/tests/calendars/pins/calendars.pin is 3687 bytes hashing to ", Assert.Single(AdmissionRules.I4TimePins(tzdb, changed, cldr)), StringComparison.Ordinal);

        var keys = AdmissionRules.TimeHomeCldrTimeZoneKeys;
        var forged = cldr.Select(entry => entry.Name == keys.Name ? entry with { Sha256 = "0" + entry.Sha256[1..] } : entry).ToArray();
        Assert.Contains($"src/tests/cldr/pins/cldr.pin names {keys.Name} as 59702 bytes hashing to 0", Assert.Single(AdmissionRules.I4TimePins(tzdb, calendars, forged)), StringComparison.Ordinal);

        // A generator declaring the tzdb release as 2026d: that declaration alone is reported, not the
        // calendar versions that agree, nor a field of another type holding the same text.
        var library = WitnessAssemblies.Compile(
            "Broiler.ECMAScript.Generation",
            File.ReadAllText(Witness("I4-a-generator-declaring-another-tzdb-release.cs.witness")));
        var declared = AdmissionRules.DeclaredTimeVersions("Broiler.ECMAScript.Generation", library);

        Assert.Equal(3, declared.Count);
        Assert.Equal(
            ["Broiler.ECMAScript.Generation declares Broiler.ECMAScript.Generation.Time.TimeDataVersions.TzdbRelease = \"2026d\", and this repository's tzdb pin states 2026e (rule I4)"],
            AdmissionRules.I4TimeVersions(PinnedTimeVersions(), declared));
    }

    /// <summary>The files this repository's CLDR pin names, as I4's CLDR clause reads them.</summary>
    private static IEnumerable<(string Name, long Bytes, string Sha256)> CldrEntries(CldrPin pin) =>
        pin.Entries.Select(static entry => (entry.Name, entry.Bytes, entry.Sha256));

    /// <summary>
    /// The time data versions this repository's pins state: the tzdb release its tzdb pin names, the CLDR
    /// release its CLDR pin names, and each calendar crate's version as the pinned crate's own
    /// <c>Cargo.toml</c> states it, read by the home's pin reader after the crates are verified.
    /// </summary>
    private static IReadOnlyDictionary<string, string> PinnedTimeVersions()
    {
        var calendarPin = TimeTableEmbedding.LoadCalendarPin();
        var members = TimeTableEmbedding.ReadCalendarArchive(calendarPin);
        var versions = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["tzdb"] = TimeTableEmbedding.LoadTzdbPin().Version,
            ["cldr"] = CldrPin.Load().Version,
        };

        foreach (var manifest in members.Keys.Where(static name => name.EndsWith("/Cargo.toml", StringComparison.Ordinal)))
        {
            var (crate, version) = HomeCalendarPin.PackageOf(HomeCalendarPin.Decode(members[manifest]));
            versions[crate] = version;
        }

        return versions;
    }
}
