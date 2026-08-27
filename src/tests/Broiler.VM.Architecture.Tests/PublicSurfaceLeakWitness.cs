namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The witness input for rule B4, compiled into the test assembly so that the same public-surface
/// scanner that clears Broiler.VM.Abstractions can be shown flagging a real leak.
/// </summary>
/// <remarks>
/// The leak B4 exists to stop is a product assembly whose public signature drags in a type its
/// consumers did not ask for. Here the offending type is xunit's, which no product assembly may
/// name; in VM-1 the same rule stops a profile type or a host type reaching the neutral contract
/// surface. If B4 ever stops flagging this class, B4 has stopped working, and
/// <see cref="AssemblyMetadataRuleTests"/> fails rather than passing quietly.
/// </remarks>
public sealed class PublicSurfaceLeakWitness
{
    public Xunit.Abstractions.ITestOutputHelper? Output { get; init; }
}
