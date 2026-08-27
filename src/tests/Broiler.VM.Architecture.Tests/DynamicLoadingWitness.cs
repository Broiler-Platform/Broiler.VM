namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The witness input for rule B5, compiled into the test assembly so that the same metadata
/// scanner that clears the product graph can be shown flagging a real violation.
/// </summary>
/// <remarks>
/// Invariant 2 forbids exactly this: resolving a profile by name at run time instead of
/// referencing its factory directly. The method is never called; its presence in the
/// MemberRef table is the entire point. If B5 ever stops flagging this type, B5 has stopped
/// working, and <see cref="AssemblyMetadataRuleTests"/> fails rather than passing quietly.
/// </remarks>
internal static class DynamicLoadingWitness
{
    internal static object? ResolveProfileByName(string typeName)
    {
        var type = System.Type.GetType(typeName, throwOnError: false);

        return type is null ? null : System.Activator.CreateInstance(type);
    }
}
