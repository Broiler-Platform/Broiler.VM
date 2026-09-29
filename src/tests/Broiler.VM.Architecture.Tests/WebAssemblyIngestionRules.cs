namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule W7: the WebAssembly harness root, which holds the script reader, the corpus store and the
/// encoders, reaches no package and no advertised composition, and no other project carries one of
/// its files.
/// </summary>
/// <remarks>
/// <para>
/// <b>What WA-4's gate asks for.</b> "A scan asserts the script reader, the corpus store, the encoder,
/// and every suite file appear in no package and in no advertised composition's closure report",
/// with a negative control that adds a reference to the script reader from the execution root and
/// observes the scan fail. Rule N13 is that scan for the JavaScript conformance harness, and it is
/// written over a harness name, so this rule holds its six clauses over the WebAssembly harness root
/// and the specification's core scripts: the root is referenced by no project, declares no package
/// identity and is literally not packable, has no advertised register row and appears in no
/// advertised closure report, and no project file names the suite directory.
/// </para>
/// <para>
/// <b>The clause N13 does not need, and this root does.</b> The reader, the store and the encoders
/// are source files of the harness root, and a project could compile one of them through a link
/// without referencing the root at all. Rule A3 refuses a link that leaves the component and not one
/// that stays inside it. So every source, content or import item of another project that lands in
/// the harness root's directory is reported, and the files the gate names are asserted to be there,
/// so the clause is not satisfied by a harness that moved them elsewhere.
/// </para>
/// </remarks>
internal static class WebAssemblyIngestionRules
{
    /// <summary>The harness root this rule is about.</summary>
    internal const string Harness = "Broiler.VM.Composition.WebAssembly.Harness";

    /// <summary>The specification's core scripts, which no project file may name.</summary>
    internal static readonly string[] SuiteDirectories = ["tests/wasm/spec"];

    /// <summary>The files of the harness root the gate names, and what each is.</summary>
    internal static readonly (string File, string What)[] IngestionSources =
    [
        ("ScriptText.cs", "the script reader"),
        ("ScriptRunner.cs", "the script runner"),
        ("TextModule.cs", "the text-format encoder"),
        ("TextInstructions.cs", "the text-format encoder's instructions"),
        ("WasmAssembler.cs", "the binary encoder"),
        ("CorpusStore.cs", "the corpus store"),
    ];

    /// <summary>W7 over the checkout's projects, the composition register and the retained closure reports.</summary>
    internal static IEnumerable<string> W7(
        IReadOnlyList<ComponentGraph.ProjectFile> projects,
        IReadOnlyList<CompositionRules.Row> rows,
        IReadOnlyList<(string Composition, CompositionRules.ClosureMode Mode)> closures)
    {
        foreach (var violation in ArchitectureRules.N13(Harness, projects, rows, closures, SuiteDirectories))
        {
            yield return violation;
        }

        var harness = projects.Where(static project => string.Equals(project.AssemblyName, Harness, StringComparison.Ordinal)).ToArray();

        if (harness.Length != 1)
        {
            // N13's own vacuity clause has said so.
            yield break;
        }

        var directory = Path.GetDirectoryName(harness[0].Path)! + Path.DirectorySeparatorChar;

        foreach (var (file, what) in IngestionSources.Where(source => !File.Exists(Path.Combine(directory, source.File))))
        {
            yield return $"the rule names {file} as {what}, and the harness root holds no such file, so the clause over it holds over nothing";
        }

        foreach (var project in projects.Where(project => !ReferenceEquals(project, harness[0])))
        {
            foreach (var item in project.SourceItemPaths.Where(item => item.StartsWith(directory, StringComparison.Ordinal)))
            {
                yield return
                    $"{project.RelativePath} carries {Path.GetRelativePath(ComponentGraph.Root, item).Replace('\\', '/')}, " +
                    $"a file of {Harness}, into its own build";
            }
        }
    }
}
