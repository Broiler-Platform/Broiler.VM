using System.Text.Json;
using System.Text.Json.Serialization;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The frozen project graph, read from graph.manifest.json.
/// </summary>
/// <remarks>
/// Rule A7 compares the resolved checkout against this file rather than against a table
/// hand-copied into a test, so that the graph the ADR publishes and the graph the test asserts
/// are the same artefact. Roadmap section 14's dependency-architecture row asks for exactly this
/// drift check, and section 15 gate 2 restates it as "the generated dependency closure matches
/// VM-0" - a release gate that VM-0 does not claim met, only prepares for.
/// </remarks>
internal static class GraphManifest
{
    private static readonly Manifest Loaded = Load();

    internal static IReadOnlyList<Project> Projects => Loaded.Projects;

    internal static int CoreContractVersion => Loaded.CoreContractVersion;

    /// <summary>The declared edges as (from, to) assembly-name pairs.</summary>
    internal static IReadOnlyList<Edge> Edges { get; } =
        Loaded.Projects
            .SelectMany(static project => project.ProjectReferences
                .Select(target => new Edge(project.AssemblyName, target)))
            .OrderBy(static edge => edge.From, StringComparer.Ordinal)
            .ThenBy(static edge => edge.To, StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// The declared package edges as (project, package) pairs: every PackageReference a project
    /// file states, which the manifest records per project since SJL-2.5 so that a package edge is
    /// part of the frozen graph as a project edge is.
    /// </summary>
    internal static IReadOnlyList<Edge> PackageEdges { get; } =
        Loaded.Projects
            .SelectMany(static project => project.PackageReferences!
                .Select(package => new Edge(project.AssemblyName, package)))
            .OrderBy(static edge => edge.From, StringComparer.Ordinal)
            .ThenBy(static edge => edge.To, StringComparer.Ordinal)
            .ToArray();

    internal static string Path { get; } = System.IO.Path.Combine(
        ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "graph.manifest.json");

    private static Manifest Load()
    {
        var path = System.IO.Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "graph.manifest.json");

        var json = File.ReadAllText(path);

        var manifest = JsonSerializer.Deserialize<Manifest>(json, Options)
            ?? throw new InvalidOperationException($"{path} deserialized to null.");

        var silent = manifest.Projects.Where(static project => project.PackageReferences is null).Select(static project => project.AssemblyName).ToArray();

        if (silent.Length > 0)
        {
            throw new InvalidOperationException(
                $"{path}: every project states its package edges, an empty list for none; these do not: {string.Join(", ", silent)}");
        }

        return manifest;
    }

    private static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    internal sealed record Edge(string From, string To);

    internal sealed class Manifest
    {
        [JsonPropertyName("coreContractVersion")]
        public int CoreContractVersion { get; init; }

        [JsonPropertyName("projects")]
        public IReadOnlyList<Project> Projects { get; init; } = [];
    }

    internal sealed class Project
    {
        [JsonPropertyName("path")]
        public string Path { get; init; } = string.Empty;

        [JsonPropertyName("assemblyName")]
        public string AssemblyName { get; init; } = string.Empty;

        [JsonPropertyName("rootNamespace")]
        public string? RootNamespace { get; init; }

        [JsonPropertyName("packageId")]
        public string? PackageId { get; init; }

        [JsonPropertyName("isPackable")]
        public bool IsPackable { get; init; }

        [JsonPropertyName("isTestProject")]
        public bool IsTestProject { get; init; }

        [JsonPropertyName("projectReferences")]
        public IReadOnlyList<string> ProjectReferences { get; init; } = [];

        /// <summary>
        /// The packages the project references, by identifier. Required on every entry since
        /// SJL-2.5 - null rather than absent is what <see cref="Load"/> refuses, so an entry
        /// written before the field existed cannot pass as "no package edges".
        /// </summary>
        [JsonPropertyName("packageReferences")]
        public IReadOnlyList<string>? PackageReferences { get; init; }
    }
}
