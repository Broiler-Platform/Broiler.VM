namespace Broiler.VM.Runtime;

/// <summary>
/// Identifies this assembly to the Broiler.VM architecture tests, which need a compile-time
/// anchor into every project in the graph. It carries no behaviour and never will: when the core runtime, catalog and lifecycle
/// arrives, the tests anchor on those types instead and this marker is deleted.
/// </summary>
/// <remarks>
/// Its existence is the honest shape of milestone VM-0. The graph, the package boundaries and
/// the forbidden edges are frozen and proven; nothing that executes bytecode exists yet.
/// </remarks>
internal sealed class AssemblyMarker
{
}
