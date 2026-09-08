namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>What happened to one file.</summary>
/// <remarks>
/// <para>
/// <b>Seven answers because this host can answer in seven places</b> - it cannot read the file, it
/// cannot tell which profile the file belongs to, a front end refuses the source, the verifier
/// refuses the artifact a front end produced, the program exhausts its allowance, the program
/// faults, or it completes. A host reporting only success and failure would put a defect in its
/// own lowering and a program using a construct a manifest excludes in the same bucket, and those
/// are the two most different outcomes here.
/// </para>
/// <para>
/// <b>THIS ENUMERATION AND THE EXIT CODES BELOW ARE COPIED FROM
/// <c>Broiler.VM.Composition.JavaScript.Cli/Vocabulary.cs</c>, DELIBERATELY AND WITH ONE
/// ADDITION.</b> Rules A11 and A12 leave shared code between composition roots nowhere to live: a
/// library holding it would be a project outside <c>src/compositions/</c> that references a
/// profile assembly, which A11 forbids, and a composition root referencing another composition
/// root is not the three-core-plus-profiles reference set A12 admits. Copying is therefore the
/// normal shape here rather than a shortcut, and it is said out loud so that a reader comparing
/// the two files finds a recorded decision rather than a suspected accident. The addition is
/// <see cref="Unroutable"/>: a single-profile host never had to decide which profile a file
/// belongs to, and this one answers that question before any front end sees a byte.
/// </para>
/// </remarks>
internal enum RunStatus
{
    /// <summary>The program ran and produced a completion value.</summary>
    Completed,

    /// <summary>A front end refused the source. No artifact was produced.</summary>
    RefusedSource,

    /// <summary>The program ran and a fault escaped it - a JavaScript error, or a trap.</summary>
    Faulted,

    /// <summary>The program spent its instruction allowance without settling.</summary>
    Exhausted,

    /// <summary>The verifier refused the artifact it was handed.</summary>
    RefusedArtifact,

    /// <summary>The file could not be read at all.</summary>
    Unreadable,

    /// <summary>The file names no profile this host composes, or names one its bytes contradict.</summary>
    Unroutable,

    /// <summary>This host did something wrong.</summary>
    HostDefect,
}

/// <summary>The exit codes this host uses, and what a caller may conclude from each.</summary>
/// <remarks>
/// <para>
/// <b>They are a contract, not a convenience.</b> A host whose only codes are zero and one makes
/// "your program has a syntax error" indistinguishable from "this host is broken", and a script
/// driving it over a corpus cannot tell the two apart. Every code below names one thing, and
/// <c>help</c> prints all of them.
/// </para>
/// <para>
/// <b>Over several files the WORST code wins, and the order is by whose fault it is rather than
/// by severity to the user.</b> <see cref="RefusedArtifact"/> outranks <see cref="Unreadable"/>
/// even though an unreadable file sounds worse: an artifact this host's own lowering produced and
/// its own verifier then refused is a defect in this component, and a defect must not be reported
/// under a code that reads as a property of the input.
/// </para>
/// <para>
/// <b><see cref="Unroutable"/> is a USAGE answer and not an input answer</b>, which is the one
/// ranking decision the single-profile host had no occasion to take. A file this host cannot route
/// is a file the caller named for a host that does not handle it - the same kind of mistake as a
/// misspelled option - so it reports under the code a caller reads as "you asked for something I
/// do not do", and it does not accuse the file of being malformed.
/// </para>
/// </remarks>
internal static class ExitCodes
{
    /// <summary>Every file ran and completed.</summary>
    internal const int Ok = 0;

    /// <summary>A program faulted and nothing caught it.</summary>
    internal const int Faulted = 1;

    /// <summary>The command line is not one this host understands, or names a file it cannot route.</summary>
    internal const int Usage = 2;

    /// <summary>A source was refused before it became an artifact.</summary>
    internal const int RefusedSource = 3;

    /// <summary>An artifact was refused by the verifier. <b>This one is a defect here.</b></summary>
    internal const int RefusedArtifact = 4;

    /// <summary>A program spent its instruction allowance.</summary>
    internal const int Exhausted = 5;

    /// <summary>A named file could not be read.</summary>
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
        RunStatus.Unroutable => Usage,
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
        RunStatus.Unroutable => 4,
        RunStatus.Unreadable => 5,
        RunStatus.RefusedArtifact => 6,
        _ => 7,
    };
}
