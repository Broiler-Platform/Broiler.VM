using System.Text;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The replayer the universal bytecode's fixture composition runs in every publish mode, run here over
/// the retained corpus so that what the composition prints is a table this suite has already held to
/// the recorded answers.
/// </summary>
public sealed class UbcCorpusReplayTests
{
    [Fact]
    public void The_Replayer_Answers_Every_Entry_As_Recorded()
    {
        var output = new StringWriter(new StringBuilder());
        var failures = UbcCorpusReplay.Replay(UbcCorpusStore.Directory(CorpusRunner.Root), output);
        var lines = output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);

        Assert.True(failures == 0, output.ToString());
        Assert.Equal(UbcCorpusStore.Read(CorpusRunner.Root).Count + 1, lines.Length);
        Assert.All(lines[..^1], line => Assert.StartsWith("ok  ", line, StringComparison.Ordinal));
    }

    [Fact]
    public void The_Replayer_Reports_A_Mutated_Entry()
    {
        var copy = Directory.CreateTempSubdirectory("ubc-corpus-replay-");

        try
        {
            var source = UbcCorpusStore.Directory(CorpusRunner.Root);

            foreach (var file in Directory.GetFiles(source))
            {
                File.Copy(file, Path.Combine(copy.FullName, Path.GetFileName(file)));
            }

            var victim = UbcCorpusStore.Read(CorpusRunner.Root)[0];
            var path = Path.Combine(copy.FullName, victim.File);
            var bytes = File.ReadAllBytes(path);
            bytes[^1] ^= 0xFF;
            File.WriteAllBytes(path, bytes);

            var output = new StringWriter(new StringBuilder());

            Assert.Equal(1, UbcCorpusReplay.Replay(copy.FullName, output));
            Assert.Contains($"FAIL {victim.Id} MUTATED", output.ToString(), StringComparison.Ordinal);
        }
        finally
        {
            copy.Delete(recursive: true);
        }
    }
}
