using System.Formats.Tar;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The shared conformance vectors' pin, <c>src/tests/vectors/pins/vectors.pin</c> (programme
/// milestone SJL-1): the archive it names is there, hashes to the pin's digest, and extracts to the
/// files and the content digest the pin names.
/// </summary>
/// <remarks>
/// <para>
/// <b>The conformance root checks the same three things before it runs a vector</b>, so a run on a
/// drifted archive cannot be retained. This test is the other half: it fails the architecture suite
/// on every run, whether or not anybody runs the vectors, the way rule N15 holds test262's archive to
/// its pin.
/// </para>
/// <para>
/// <b>The content digest is recomputed from the archive's members here</b>, by the same definition
/// the harness and every retained suite pin use - SHA-256 over the sorted lines
/// <c>&lt;path&gt; LF &lt;sha256&gt; LF</c>, paths below <c>conformance/vectors/</c> - and not
/// by calling the harness, which a test project may not reference (rule A11).
/// </para>
/// </remarks>
public sealed class SharedVectorsPinTests
{
    private const string PinPath = "src/tests/vectors/pins/vectors.pin";

    private const string Home = "conformance/vectors/";

    [Fact]
    public void The_Archive_Is_The_One_The_Pin_Names()
    {
        var pin = Pin();

        // Non-vacuous: every value compared below is read from the pin, so a pin that stopped
        // declaring one would make its comparison free rather than false.
        foreach (var key in new[] { "suite", "upstream", "revision", "archive-sha256", "content-sha256", "files", "archived", "archived-at" })
        {
            Assert.True(pin.ContainsKey(key), $"{PinPath} declares no `{key}`");
        }

        Assert.Equal("broiler-js-vectors", pin["suite"]);
        Assert.Equal("yes", pin["archived"]);
        Assert.Matches("^[0-9a-f]{40}$", pin["revision"]);
        Assert.Contains(pin["revision"], pin["archived-at"], StringComparison.Ordinal);

        var bytes = File.ReadAllBytes(Path.Combine(ComponentGraph.Root, pin["archived-at"].Replace('/', Path.DirectorySeparatorChar)));
        Assert.Equal(pin["archive-sha256"], Convert.ToHexStringLower(SHA256.HashData(bytes)));

        var members = Members(bytes);
        Assert.Equal(int.Parse(pin["files"], System.Globalization.CultureInfo.InvariantCulture), members.Count);
        Assert.Equal(pin["content-sha256"], ContentDigest(members));

        // The corpus's own files are there, and its negative controls are not: those stay with the
        // home's validator.
        Assert.Contains("prelude.js", members.Keys);
        Assert.Contains("SCHEMA.md", members.Keys);
        Assert.Contains(members.Keys, path => path.StartsWith("numbers/", StringComparison.Ordinal) && path.EndsWith(".vectors", StringComparison.Ordinal));
        Assert.DoesNotContain(members.Keys, path => path.StartsWith("controls/", StringComparison.Ordinal));
    }

    [Fact]
    public void One_Changed_Byte_Moves_The_Content_Digest()
    {
        var pin = Pin();
        var members = Members(File.ReadAllBytes(Path.Combine(ComponentGraph.Root, pin["archived-at"].Replace('/', Path.DirectorySeparatorChar))));
        var changed = new SortedDictionary<string, string>(members, StringComparer.Ordinal)
        {
            ["prelude.js"] = new string('0', 64),
        };

        Assert.NotEqual(pin["content-sha256"], ContentDigest(changed));
    }

    /// <summary>The archive's regular files by their path below the corpus's home, with their SHA-256.</summary>
    private static SortedDictionary<string, string> Members(byte[] archive)
    {
        var members = new SortedDictionary<string, string>(StringComparer.Ordinal);
        using var gzip = new GZipStream(new MemoryStream(archive, writable: false), CompressionMode.Decompress);
        using var reader = new TarReader(gzip);

        while (reader.GetNextEntry(copyData: true) is { } entry)
        {
            if (entry.EntryType is TarEntryType.Directory or TarEntryType.GlobalExtendedAttributes)
            {
                continue;
            }

            Assert.True(entry.EntryType is TarEntryType.RegularFile or TarEntryType.V7RegularFile, $"{entry.Name} is not a regular file");
            Assert.StartsWith(Home, entry.Name, StringComparison.Ordinal);
            using var content = new MemoryStream();
            entry.DataStream!.CopyTo(content);
            members.Add(entry.Name[Home.Length..], Convert.ToHexStringLower(SHA256.HashData(content.ToArray())));
        }

        return members;
    }

    private static string ContentDigest(SortedDictionary<string, string> members)
    {
        var text = new StringBuilder();

        foreach (var (path, digest) in members)
        {
            text.Append(path).Append('\n').Append(digest).Append('\n');
        }

        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));
    }

    private static Dictionary<string, string> Pin()
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var raw in File.ReadAllLines(Path.Combine(ComponentGraph.Root, PinPath)))
        {
            var line = raw.Trim();

            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            var space = line.IndexOf(' ', StringComparison.Ordinal);
            values.Add(line[..space], line[(space + 1)..].Trim());
        }

        return values;
    }
}
