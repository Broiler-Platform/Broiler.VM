using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Source witnesses for the metadata rules that need an assembly to read: B3, I2 and the admission
/// probe's negative controls. Each is compiled in memory against this runtime's framework, and
/// optionally against stub assemblies compiled the same way, so that the witness can name a
/// foreign assembly without one being on disk or in the graph.
/// </summary>
/// <remarks>
/// The same approach as NativeBaselineRuleTests' X2 witness, made shareable because group I has
/// three rules that need it. Nothing compiled here is loaded or run: <see cref="AssemblyFacts"/>
/// reads the image's tables, as it reads every shipping assembly's.
/// </remarks>
internal static class WitnessAssemblies
{
    /// <summary>The witnesses directory of this project.</summary>
    internal static string Directory { get; } = Path.Combine(
        ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses");

    /// <summary>Compiles source text into an assembly image named <paramref name="assemblyName"/>.</summary>
    /// <param name="assemblyName">The assembly's name, which is what an AssemblyRef to it records.</param>
    /// <param name="source">The source text.</param>
    /// <param name="references">Further images to compile against, beyond the framework.</param>
    internal static byte[] Compile(string assemblyName, string source, params byte[][] references)
    {
        var framework = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var frameworkReferences = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(reference => string.Equals(
                Path.GetDirectoryName(reference), framework, StringComparison.OrdinalIgnoreCase))
            .Select(static reference => (MetadataReference)MetadataReference.CreateFromFile(reference));

        var compilation = CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            frameworkReferences.Concat(references.Select(static image => MetadataReference.CreateFromImage(image))),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        using var stream = new MemoryStream();
        var emitted = compilation.Emit(stream);

        Assert.True(
            emitted.Success,
            $"{assemblyName} did not compile, so it witnesses nothing: " +
            string.Join("; ", emitted.Diagnostics.Where(static diagnostic =>
                diagnostic.Severity == DiagnosticSeverity.Error)));

        return stream.ToArray();
    }

    /// <summary>Compiles a stored source witness, <c>witnesses/&lt;file&gt;</c>, and reads its tables.</summary>
    internal static AssemblyFacts Read(string file, string assemblyName, params byte[][] references) =>
        AssemblyFacts.ReadImage(
            assemblyName,
            Compile(assemblyName, File.ReadAllText(Path.Combine(Directory, file)), references));
}
