// SJL-11.1 correspondence, library level: Broiler.Regex 0.1.0-preview.4 called directly, for the rows
// Broiler.JS's shell masks (its TryRouteToBroiler catches every exception and falls back to .NET).
// Every answer printed is the library's; a string is printed with \uXXXX for anything outside ASCII.
using Broiler.Regex;
using Broiler.Regex.Parsing;

static string Q(string s)
{
    var b = new System.Text.StringBuilder("\"");
    foreach (var c in s) b.Append(c < 0x20 || c > 0x7e || c == '"' || c == '\\' ? $"\\u{(int)c:x4}" : c.ToString());
    return b.Append('"').ToString();
}

static string Try(string pattern, string flags, string input)
{
    try
    {
        var re = new BroilerRegex(pattern, flags);
        var m = re.Match(input);
        return m.Success ? $"construct ok, match @{m.Index} length {m.Length}" : "construct ok, no match";
    }
    catch (Exception e) { return $"construct refuses: {e.GetType().Name}: {e.Message}"; }
}

static string Parse(string pattern, RegexFlags flags)
{
    try { RegexParser.Parse(pattern, flags, out _, out _); return "parse-only accepts"; }
    catch (Exception e) { return $"parse-only refuses: {e.GetType().Name}"; }
}

void Row(string label, string pattern, string flags, string input)
{
    var f = RegexFlagsParser.Parse(flags);
    Console.WriteLine($"{label} | pattern {Q(pattern)} /{flags} on {Q(input)} | {Parse(pattern, f)} | {Try(pattern, flags, input)}");
}

var one = char.ConvertFromUtf32(0x0661);
var bold = char.ConvertFromUtf32(0x1D400);
// \k parse-only (S5): parsing accepts a reference to a group the pattern does not have.
Row("k-parse-only", @"\k<b>(?<a>x)", "u", "x");
Row("k-parse-only", @"\k<b>(?<a>x)", "", "x");
// \0 then a non-ASCII decimal digit (S4): the lookahead is ASCII 0-9 only.
Row("nul-nd", @"\0" + one, "u", "\0" + one);
Row("nul-nd", @"[\0" + one + "]", "u", one);
Row("nul-nd", @"\0" + one, "", "\0" + one);
// Escaped group names (S2) and the duplicate check before decoding (S1). "%u" stands for a backslash
// and u, so that this file holds no escape the C# compiler would decode first.
static string E(string s) => s.Replace("%u", ((char)92).ToString() + "u");
Row("escaped-name", E("(?<%uD835%uDC00>x)"), "", "x");
Row("escaped-name", E("(?<%uD835%uDC00>x)"), "u", "x");
Row("escaped-name", "(?<" + bold + ">x)", "", "x");
Row("escaped-name", E("(?<%u{1D400}>x)"), "", "x");
Row("escaped-name", E("(?<%uD800>x)"), "u", "x");
Row("escaped-duplicate", E("(?<%u0061>x)(?<a>y)"), "", "xy");
Row("escaped-duplicate", E("(?<%u0061>x)(?<a>y)"), "u", "xy");
Row("duplicate-alternatives", @"(?<a>x)|(?<a>y)", "u", "y");
// v-mode class syntax the shell accepted through its .NET fallback (S8): the row's own cases, then
// test262's breaking-change-from-u-to-v-01.js to -28.js in order.
Row("v-syntax", "[a|]", "v", "a");
Row("v-syntax", "[a!!b]", "v", "a");
string[] breaking = ["[(]", "[)]", "[[]", "[{]", "[}]", "[/]", "[-]", "[|]", "[&&]", "[!!]", "[##]", "[$$]", "[%%]", "[**]",
    "[++]", "[,,]", "[..]", "[::]", "[;;]", "[<<]", "[==]", "[>>]", "[??]", "[@@]", "[``]", "[~~]", "[^^^]", "[_^^]"];
for (var i = 0; i < breaking.Length; i++)
    Row($"v-syntax breaking-change-{i + 1:00}", breaking[i], "v", "a");
// A four-digit lead-surrogate escape before a braced escape under u (S10), and a back-reference
// under u across a surrogate pair (M7): the shell routes both to the library, so these are its answers.
static string S(params int[] units) => new string(units.Select(c => (char)c).ToArray());
Row("lead-then-braced", E("%uD83D%u{DC38}+"), "u", S(0xD83D, 0xDC38, 0xDC38));
Row("lead-then-braced", E("%uD83D%u{3042}*"), "u", S(0xD83D));
Row("lead-then-braced", E("[%uD83D%u{3042}]*"), "u", S(0xD83D));
Row("backreference-pair", @"foo(.+)bar\1", "u", "foo" + S(0xD834) + "bar" + S(0xD834, 0xDC00));
Row("backreference-pair", @"foo(.+)bar\1", "", "foo" + S(0xD834) + "bar" + S(0xD834, 0xDC00));
Row("lookbehind-quantified", "(?<=a)*", "", "a");
// Loose property names (S6), corrected in Broiler.JS at SJL-7.5 by its own precheck.
Row("loose-name", @"\p{letter}", "u", "a");
Row("loose-name", @"\p{Script=greek}", "u", "a");
Row("hrkt", @"\p{sc=Hrkt}", "u", "a");
