using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Ubc;
using Com.Example.Ledger;
using Com.Example.Tally;
using System.Collections.Immutable;
using System.Globalization;

namespace Broiler.VM.Composition.Ubc.Fixture;

/// <summary>
/// The universal bytecode's demonstration composition: the fixture family over the bytecode emitter,
/// beside the ledger, published and run.
/// </summary>
/// <remarks>
/// <para>
/// Its checks are the programme's UBC-2.7 contract checks, run inside the published image so that
/// every publish mode answers them: the descriptor's rows, the contract-version refusal, the program
/// corpus's transcripts, every executor step read from the executor itself, cancellation within the
/// declared bound, a form the image does not compose refused at verification by name, fuel parity across
/// the twins, the two-profile hostile-neighbour check the roadmap has asked for since VM-3, a frame's
/// fuel charged with it, and a parked operation holding no call depth.
/// </para>
/// <para>
/// <c>--closure</c> prints what the composition declares; <c>--transcripts</c> prints every program's
/// transcript; <c>--write-corpus DIR</c> retains the program corpus and the primitive input corpus;
/// <c>--corpus DIR</c> replays a retained corpus against its hashes and its recorded answers.
/// </para>
/// </remarks>
internal static class Program
{
    private static int Main(string[] args)
    {
        var verbose = args.Contains("--verbose", StringComparer.Ordinal);

        try
        {
            if (args.Contains("--closure", StringComparer.Ordinal))
            {
                return ReportClosure();
            }

            if (args.Contains("--transcripts", StringComparer.Ordinal))
            {
                foreach (var program in TallyPrograms.All)
                {
                    using var runtime = FixtureHost.Runtime();
                    Console.WriteLine($"{program.Name}: {FixtureHost.Transcript(runtime, program.Artifact.AsSpan(), program.Entry).Replace("\n", " | ", StringComparison.Ordinal)}");
                }

                return 0;
            }

            if (Argument(args, "--write-corpus") is { } target)
            {
                return FixtureCorpus.Write(target);
            }

            if (Argument(args, "--corpus") is { } source)
            {
                return FixtureCorpus.Replay(source, verbose);
            }

            if (Argument(args, "--ubc1-corpus") is { } malformed)
            {
                return Broiler.VM.Contract.Tests.UbcCorpusReplay.Replay(malformed, Console.Out) == 0 ? 0 : 1;
            }

            var checks = new List<(string Name, bool Passed, string Detail)>
            {
                FixtureChecks.DescriptorRows(),
                FixtureChecks.ContractVersionRefused(),
                FixtureChecks.ProgramCorpus(),
                FixtureChecks.EveryStepKind(),
                FixtureChecks.CancellationWithinTheBound(),
                FixtureChecks.FormNotComposed(),
                FixtureChecks.FuelParity(),
                FixtureChecks.HostileNeighbour(),
                FixtureChecks.FrameFuelCharged(),
                FixtureChecks.ParkedOperationHoldsNoDepth(),
            };

            var failed = 0;

            foreach (var (name, passed, detail) in checks)
            {
                if (!passed)
                {
                    failed++;
                }

                if (verbose || !passed)
                {
                    Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
                }
            }

            Console.WriteLine(
                failed == 0
                    ? $"broiler-vm-composition-ubc-fixture: {checks.Count} checks passed, core contract version {VmCoreContract.Version}, universal bytecode contract version {UbcContract.Version}"
                    : $"broiler-vm-composition-ubc-fixture: {failed} of {checks.Count} checks FAILED");

            return failed == 0 ? 0 : 1;
        }
        catch (Exception failure)
        {
            Console.WriteLine($"broiler-vm-composition-ubc-fixture: unhandled {failure.GetType().Name}: {failure.Message}");
            return 2;
        }
    }

    private static string? Argument(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }

    /// <summary>What this composition declares, as the composition register documents it.</summary>
    private static int ReportClosure()
    {
        Console.WriteLine($"# broiler-vm-composition core-contract-version={VmCoreContract.Version} ubc-contract-version={UbcContract.Version}");
        Console.WriteLine("composition Broiler.VM.Composition.Ubc.Fixture");
        Console.WriteLine("profiles 2");

        foreach (var descriptor in new[] { FixtureHost.Tally, LedgerProfile.Descriptor })
        {
            Console.WriteLine(string.Join(
                ' ',
                "profile",
                descriptor.ProfileId,
                descriptor.PackageIdentity.PackageId,
                descriptor.DescriptorRevision.ToString(CultureInfo.InvariantCulture),
                descriptor.HostCapabilityDescriptors.Length.ToString(CultureInfo.InvariantCulture)));
        }

        Console.WriteLine($"form {UbcBytecodeEmitter.Form.Identity} {UbcBytecodeEmitter.Form.SemanticVersion.ToString(CultureInfo.InvariantCulture)}");
        return 0;
    }
}
