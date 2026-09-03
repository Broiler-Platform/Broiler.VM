namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>What happened to one file.</summary>
/// <remarks>
/// <b>Six answers because this host can answer in six places</b> - it cannot read the file, the
/// front end refuses the source, the verifier refuses the artifact the front end produced, the
/// program exhausts its allowance, the program throws, or it completes. A host reporting only
/// success and failure would put a defect in its own lowering and a program using a construct
/// this manifest excludes in the same bucket, and those are the two most different outcomes here.
/// </remarks>
internal enum RunStatus
{
    /// <summary>The program ran and produced a completion value.</summary>
    Completed,

    /// <summary>The front end refused the source. No artifact was produced.</summary>
    RefusedSource,

    /// <summary>The program ran and a JavaScript error escaped it.</summary>
    Faulted,

    /// <summary>The program spent its instruction allowance without settling.</summary>
    Exhausted,

    /// <summary>The verifier refused the artifact the front end produced.</summary>
    RefusedArtifact,

    /// <summary>The file could not be read as JavaScript source at all.</summary>
    Unreadable,

    /// <summary>This host did something wrong.</summary>
    HostDefect,
}

/// <summary>The exit codes this host uses, and what a caller may conclude from each.</summary>
/// <remarks>
/// <para>
/// <b>They are a contract, not a convenience.</b> A host whose only codes are zero and one makes
/// "your program has a syntax error" indistinguishable from "this host is broken", and a script
/// driving it over a corpus cannot tell the two apart. Every code below names one thing.
/// </para>
/// <para>
/// <b>Over several files the WORST code wins, and the order is by whose fault it is rather than
/// by severity to the user.</b> <see cref="RefusedArtifact"/> outranks <see cref="Unreadable"/>
/// even though an unreadable file sounds worse: an artifact this host's own lowering produced and
/// its own verifier then refused is a defect in this component, and a defect must not be reported
/// under a code that reads as a property of the input.
/// </para>
/// </remarks>
internal static class ExitCodes
{
    /// <summary>Every file ran and completed.</summary>
    internal const int Ok = 0;

    /// <summary>A program threw and nothing caught it.</summary>
    internal const int Faulted = 1;

    /// <summary>The command line is not one this host understands.</summary>
    internal const int Usage = 2;

    /// <summary>A source was refused before it became an artifact.</summary>
    internal const int RefusedSource = 3;

    /// <summary>An artifact was refused by the verifier. <b>This one is a defect here.</b></summary>
    internal const int RefusedArtifact = 4;

    /// <summary>A program spent its instruction allowance.</summary>
    internal const int Exhausted = 5;

    /// <summary>A named file could not be read as source.</summary>
    internal const int Unreadable = 6;

    /// <summary>This host did something wrong. <b>Also a defect here.</b></summary>
    internal const int HostDefect = 7;

    /// <summary>The code one status reports.</summary>
    internal static int For(RunStatus status) => status switch
    {
        RunStatus.Completed => Ok,
        RunStatus.RefusedSource => RefusedSource,
        RunStatus.Faulted => Faulted,
        RunStatus.Exhausted => Exhausted,
        RunStatus.RefusedArtifact => RefusedArtifact,
        RunStatus.Unreadable => Unreadable,
        _ => HostDefect,
    };

    /// <summary>
    /// How much a status dominates when several files ran. Higher wins.
    /// </summary>
    /// <remarks>
    /// The two this component is answerable for are highest, so a corpus sweep that turns up one
    /// lowering defect among thousands of ordinary refusals reports the defect rather than
    /// averaging it away.
    /// </remarks>
    internal static int Rank(RunStatus status) => status switch
    {
        RunStatus.Completed => 0,
        RunStatus.RefusedSource => 1,
        RunStatus.Faulted => 2,
        RunStatus.Exhausted => 3,
        RunStatus.Unreadable => 4,
        RunStatus.RefusedArtifact => 5,
        _ => 6,
    };
}
