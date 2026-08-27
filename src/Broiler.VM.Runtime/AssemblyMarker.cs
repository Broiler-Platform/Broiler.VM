namespace Broiler.VM.Runtime;

/// <summary>
/// Identifies this assembly to the Broiler.VM architecture tests, which need a compile-time
/// anchor into every project in the graph. It carries no behaviour and never will: when the core runtime, catalog and lifecycle
/// arrives, the tests anchor on those types instead and this marker is deleted.
/// </summary>
/// <remarks>
/// Its existence is the honest shape of milestone VM-0. The graph and the package boundaries are
/// frozen, and every forbidden edge in the VM-0 shell graph is expressed and witnessed; nine of
/// the twenty-eight rules await their subject. Nothing that executes bytecode exists yet.
/// </remarks>
internal sealed class AssemblyMarker
{
}
