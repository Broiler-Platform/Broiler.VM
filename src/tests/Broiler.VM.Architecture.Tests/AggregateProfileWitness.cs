using System.Runtime.CompilerServices;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The witness input for rule B7, compiled into the test assembly.
/// </summary>
/// <remarks>
/// Roadmap section 3 rejects a type of this shape by name: an aggregate that names several
/// profiles at once would reference every profile assembly and defeat VM-3's exact-closure
/// gates. The type is deliberately nested and public, which is also the case the metadata reader
/// used to miss - nested types carry NestedPublic, never Public, so a visibility test written for
/// top-level types alone would not have seen it.
/// </remarks>
public static class ProfileCatalogWitness
{
    public static class BuiltInProfiles
    {
    }
}

/// <summary>
/// The witness input for rule B5b, compiled into the test assembly.
/// </summary>
/// <remarks>
/// Invariant 2 forbids a module-initializer ordering dependency by name, and the aggregate
/// repository suppresses CA2255 repository-wide precisely because seven legacy assemblies
/// auto-register built-ins this way. Broiler.VM does not chain to that suppression, so the
/// warning is suppressed here, narrowly, on the one method that exists to be caught.
/// </remarks>
internal static class ModuleInitializerWitness
{
#pragma warning disable CA2255 // The attribute is the point: B5b must find it.
    [ModuleInitializer]
    internal static void Initialize()
    {
    }
#pragma warning restore CA2255
}
