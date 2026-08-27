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

    internal static string Path { get; } = System.IO.Path.Combine(
        ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "graph.manifest.json");

    private static Manifest Load()
    {
        var path = System.IO.Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "graph.manifest.json");

        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<Manifest>(json, Options)
            ?? throw new InvalidOperationException($"{path} deserialized to null.");
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
    }
}
