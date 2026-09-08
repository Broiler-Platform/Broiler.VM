using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The rules about making memory executable: which assembly may map a page, where the calls that
/// do it are written, and which composition is allowed to contain them.
/// </summary>
/// <remarks>
/// <para>
/// <b>These rules exist because rule B5 published a prohibition it did not check.</b> B5 forbids
/// <c>System.Reflection.Emit</c>, <c>System.Runtime.Loader</c> and the reflection-invocation
/// members, and none of those reaches a hand-written machine-code path. A composition that mapped
/// a page executable and jumped into it passed every automated gate this component had, while
/// tripping a stop condition the component published. That was a hole in the enforcement before
/// any backend existed; it stopped being hypothetical when an arming path was written.
/// </para>
/// <para>
/// <b>Three rules rather than one, because the question is asked in three places and each answer
/// is checkable on its own.</b> Rule B5c reads compiled metadata and asks which assemblies declare
/// a platform invoke into a mapping or protection entry point. Rule X1 reads source and asks where
/// those calls are written and what protection they pass. Rule K5 reads the composition register
/// and asks whether the image that contains them says so. A component with only the first would
/// know an assembly can map memory and not where; with only the second, a rule defeated by a
/// generated file; with only the third, a declaration nothing holds to the tree.
/// </para>
/// <para>
/// <b>THE ENTRY POINT IS THE SUBJECT AND THE MEMBER NAME IS NOT.</b> A managed member is named in
/// the MemberRef table and a rule can read it there; <c>VirtualProtect</c> and <c>mprotect</c> are
/// named in no member reference at all, because they are rows in the calling assembly's own
/// ImplMap. That is the reading B5 could never have made and it is why B5c is a separate row.
/// </para>
/// </remarks>
internal static class NativeMappingRules
{
    /// <summary>
    /// The one assembly of this component that is allowed to declare a mapping or protection
    /// platform invoke.
    /// </summary>
    /// <remarks>
    /// Named here rather than derived, and it is a decision rather than an observation: a rule
    /// that took "whichever assembly happens to have one" as its allowed set would be satisfied by
    /// every tree, including the one this rule exists to reject.
    /// </remarks>
    internal const string ArmingAssembly = "Broiler.VM.Profile.JavaScript";

    /// <summary>
    /// The files that make up the arming path: one type in three parts.
    /// </summary>
    /// <remarks>
    /// <b>Three files and not one, and rule J6 is the reason.</b> A covered product source may
    /// carry no preprocessor directive, so the platform split cannot be a <c>#if</c>; it is two
    /// partial halves and a run-time test, both compiled into every image. The rule treats the
    /// three as one place because they are one type, and it says so rather than letting a reader
    /// discover that "one place" means three paths.
    /// </remarks>
    internal static readonly string[] ArmingPath =
    [
        "src/Broiler.VM.Profile.JavaScript/JsNativePage.cs",
        "src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs",
        "src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs",
    ];

    /// <summary>
    /// Every entry point by which a process reserves a region, changes a region's protection,
    /// releases one, or publishes freshly written bytes to the instruction stream.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Both platforms and the spellings either of them accepts, because a rule that knew only the
    /// one this component happens to call would be defeated by an author who reached for the
    /// neighbour. The <c>Nt*</c> pair is here because the documented Win32 calls are wrappers over
    /// it and a caller can skip them; <c>pthread_jit_write_protect_np</c>,
    /// <c>sys_icache_invalidate</c> and <c>__clear_cache</c> are here because they are how the
    /// same thing is done on a system this component has never published to, and a rule that
    /// waited for that port to be written would be added after the code it governs.
    /// </para>
    /// <para>
    /// A trailing <c>A</c> or <c>W</c> is stripped before the comparison, because the ANSI and
    /// wide spellings of one Win32 export are one entry point.
    /// </para>
    /// </remarks>
    internal static readonly string[] MappingEntryPoints =
    [
        "CreateFileMapping",
        "FlushInstructionCache",
        "MapViewOfFile",
        "MapViewOfFile3",
        "NtAllocateVirtualMemory",
        "NtProtectVirtualMemory",
        "VirtualAlloc",
        "VirtualAllocEx",
        "VirtualFree",
        "VirtualProtect",
        "VirtualProtectEx",
        "__clear_cache",
        "memfd_create",
        "mmap",
        "mmap64",
        "mprotect",
        "munmap",
        "pthread_jit_write_protect_np",
        "shm_open",
        "sys_icache_invalidate",
    ];

    /// <summary>
    /// The entry points the arming path declares, and the whole of what it is allowed to declare.
    /// </summary>
    /// <remarks>
    /// A closed set rather than a licence for the assembly. An arming path that acquired
    /// <c>pthread_jit_write_protect_np</c> would be an arming path with a second mechanism in it,
    /// and the point of naming one assembly is that what it does can be read in one sitting.
    /// </remarks>
    internal static readonly string[] ArmingPathEntryPoints =
    [
        "FlushInstructionCache",
        "VirtualAlloc",
        "VirtualFree",
        "VirtualProtect",
        "mmap",
        "mprotect",
        "munmap",
    ];

    /// <summary>The architectures the composition register's native-execution column may name.</summary>
    /// <remarks>
    /// <c>none</c> is one of them and it is the claim of incapability the register describes it as:
    /// a composition declaring it must be unable to map artifact bytes executable, which is a
    /// property of what its image contains rather than of which call sites a reader followed.
    /// </remarks>
    internal static readonly string[] Architectures = ["none", "x86-64", "x86-32", "arm64"];

    /// <summary>
    /// The Win32 page protections that admit a write and an execute at the same time.
    /// </summary>
    /// <remarks>
    /// <c>PAGE_EXECUTE_READWRITE</c> and <c>PAGE_EXECUTE_WRITECOPY</c>. Both are here because a
    /// rule that named only the first would be satisfied by the second, which does the same thing
    /// through a copy-on-write mapping.
    /// </remarks>
    internal static readonly int[] WindowsWriteAndExecute = [0x40, 0x80];

    /// <summary>The Unix protection bits that name a write and an execute.</summary>
    internal const int UnixWriteAndExecute = 0x2 | 0x4;

    /// <summary>The parameter name a protection argument is passed to.</summary>
    /// <remarks>
    /// The arming path's imports all spell it this way, and the rule declares the spelling rather
    /// than counting arguments: an argument index would be one edit away from checking the wrong
    /// operand of the wrong import, and it would say nothing a reader could verify by looking at
    /// the declaration. A rename does not weaken the rule silently - the vacuity clause reports
    /// that no protection position was found at all.
    /// </remarks>
    internal const string ProtectionParameter = "protect";

    /// <summary>
    /// B5c: the mapping and protection platform invokes of one assembly, each one a violation
    /// unless it is the arming path declaring an entry point the arming path declares.
    /// </summary>
    /// <remarks>
    /// Two clauses, and they fail differently. An assembly other than the arming one declaring any
    /// of these has acquired the capability outside the place this component put it. The arming
    /// assembly declaring an entry point outside its own closed set has grown a second mechanism
    /// inside the place - which is the same defect wearing the right assembly name.
    /// </remarks>
    internal static IEnumerable<string> B5c(AssemblyFacts assembly)
    {
        foreach (var invoke in assembly.PlatformInvokes)
        {
            var entryPoint = Normalize(EntryPointOf(invoke));

            if (!MappingEntryPoints.Contains(entryPoint, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.Equals(assembly.Name, ArmingAssembly, StringComparison.Ordinal))
            {
                yield return
                    $"{assembly.Name} declares the platform invoke {invoke}, and the one assembly " +
                    $"that may map or protect memory is {ArmingAssembly}";

                continue;
            }

            if (!ArmingPathEntryPoints.Contains(entryPoint, StringComparer.OrdinalIgnoreCase))
            {
                yield return
                    $"{assembly.Name} declares the platform invoke {invoke}, which is outside the " +
                    "closed set the arming path declares";
            }
        }
    }

    /// <summary>
    /// X1: the arming path is the one place in the tree that names a mapping or protection API,
    /// and no protection it passes admits a write and an execute at once.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Clause four quantifies over the arming path and not over every hexadecimal literal in
    /// the tree, and that is the strongest scope it can honestly have.</b> The value
    /// <c>0x40</c> is a page protection in a file that can pass one to an operating system and is
    /// an opcode byte, a REX prefix and a continuation-bit mask everywhere else; a rule that
    /// forbade the literal would be forbidding three instruction encoders. Clause one is what
    /// makes the narrower scope complete: outside the arming path no file names an API that has a
    /// protection argument, so outside the arming path a protection value cannot reach one.
    /// </para>
    /// <para>
    /// <b>The rule reports its own vacuity twice.</b> An input in which no arming file appears, and
    /// an arming path that names no mapping API, are both failures rather than clean results - the
    /// shape a source rule takes when a file has been renamed out from under it.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> X1(IReadOnlyList<SourceUnit> tree)
    {
        var arming = tree
            .Where(static file => ArmingPath.Contains(file.RelativePath, StringComparer.Ordinal))
            .ToArray();

        foreach (var file in tree.Except(arming))
        {
            foreach (var entryPoint in MappingEntryPoints.Where(entryPoint => Names(file.Text, entryPoint)))
            {
                yield return
                    $"{file.RelativePath} names {entryPoint}, and the one place in this component " +
                    $"that maps or protects memory is {string.Join(", ", ArmingPath)}";
            }
        }

        if (arming.Length == 0)
        {
            yield return
                "no file of the arming path is in this input, so this rule quantified over a " +
                "capability it never found";

            yield break;
        }

        if (!arming.Any(file => MappingEntryPoints.Any(entryPoint => Names(file.Text, entryPoint))))
        {
            yield return
                "the arming path names no mapping or protection API, so the one place this rule " +
                "pins is not the place that maps memory";
        }

        var constants = Constants(arming);
        var positions = ProtectionPositions(arming);
        var found = 0;

        foreach (var (file, call, method) in ProtectionArguments(arming, positions))
        {
            found++;

            if (call is not IdentifierNameSyntax named)
            {
                yield return
                    $"{file.RelativePath} passes {call} as {method}'s protection argument, and a " +
                    "protection is passed as a named constant of the arming path or not at all";

                continue;
            }

            var name = named.Identifier.ValueText;

            if (!constants.TryGetValue(name, out var constant))
            {
                yield return
                    $"{file.RelativePath} passes {name} as {method}'s protection argument, and no " +
                    "constant of that name is declared in the arming path";

                continue;
            }

            if (WriteAndExecute(constant))
            {
                yield return
                    $"{constant.File} declares {name} as a protection that admits a write and an " +
                    $"execute at once, and {file.RelativePath} passes it to {method}";
            }
        }

        if (found == 0)
        {
            yield return
                $"no argument in the arming path reaches a parameter named `{ProtectionParameter}`, " +
                "so this rule checked no protection at all";
        }
    }

    /// <summary>
    /// K5: a composition's image and its native-execution cell say the same thing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is what turns the register's native-execution column from a claim a reader checks
    /// into a rule that fails a build.</b> The core's closing stop condition was narrowed from "a
    /// product closure reaches dynamic code" to "reaches UNDECLARED dynamic code", and an absolute
    /// prohibition needs no allowlist while a declaration needs one. The register is where the
    /// declaration lives and this is the rule that reads it.
    /// </para>
    /// <para>
    /// <b>Both directions, and the second is not decoration.</b> A row declaring <c>none</c> over
    /// an image that contains an arming path is a permission nobody granted. A row naming an
    /// architecture over an image that contains no arming path at all is a permission nobody can
    /// exercise, which reads to anyone auditing the table as a capability this composition has -
    /// and a register that overstates is the failure this component treats as a stop condition.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> K5(
        CompositionRules.Row row, IReadOnlyList<string> assembliesThatMapMemory)
    {
        var declared = row.NativeExecution
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(static value => value.Trim('`', ' '))
            .ToArray();

        if (declared.Length == 0)
        {
            yield return
                $"{row.Composition} declares nothing in the native-execution column, and a row " +
                "with no cell there declares neither a permission nor an incapability";

            yield break;
        }

        foreach (var value in declared.Where(value =>
                     !Architectures.Contains(value, StringComparer.Ordinal)))
        {
            yield return
                $"{row.Composition} declares native execution '{value}', which is not one of " +
                $"[{string.Join(", ", Architectures)}]";
        }

        var armsNothing = declared.Contains("none", StringComparer.Ordinal);

        if (armsNothing && declared.Length > 1)
        {
            yield return
                $"{row.Composition} declares none beside an architecture, and the two cannot both " +
                "be true of one image";
        }

        var mapping = Closure(row)
            .Where(name => assembliesThatMapMemory.Contains(name, StringComparer.Ordinal))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (armsNothing && mapping.Length > 0)
        {
            yield return
                $"{row.Composition} declares none and links [{string.Join(", ", mapping)}], which " +
                "maps memory executable. `none` is a claim of incapability rather than of " +
                "restraint, so the row states a permission nobody granted";
        }

        if (!armsNothing && mapping.Length == 0)
        {
            yield return
                $"{row.Composition} declares [{string.Join(", ", declared)}] and links no assembly " +
                "that can map memory executable, so the row states a permission nothing in the " +
                "image can exercise";
        }
    }

    /// <summary>Every assembly one composition's image contains, as its register row declares it.</summary>
    /// <remarks>
    /// The same set rule K4 holds a published closure report to, derived here rather than read off
    /// a bundle: K4 has already established that the two agree, and deriving it keeps this rule
    /// answerable for a composition whose evidence bundle belongs to another milestone series.
    /// </remarks>
    internal static IEnumerable<string> Closure(CompositionRules.Row row) =>
        CompositionRules.CoreAssemblies
            .Append(row.Composition)
            .Concat(row.ProfileAssemblies)
            .Concat(row.Siblings);

    /// <summary>
    /// Every assembly of this checkout that can map memory executable, read two ways.
    /// </summary>
    /// <remarks>
    /// <b>Metadata and source, unioned, because neither reading is complete on its own.</b> The
    /// metadata reading is the authoritative one - it sees a platform invoke whichever spelling
    /// produced it - and it can only see an assembly that was built, which the mobile head is not
    /// on a machine without the workload it needs. The source reading covers every project in the
    /// checkout whether or not it was built. An assembly the two disagree about is in the set,
    /// because the question is what an image can do and not which reader noticed.
    /// </remarks>
    internal static IReadOnlyList<string> AssembliesThatMapMemory(
        IReadOnlyList<AssemblyFacts> assemblies, IReadOnlyList<SourceUnit> tree) =>
        assemblies
            .Where(static assembly => assembly.PlatformInvokes.Any(invoke =>
                MappingEntryPoints.Contains(
                    Normalize(EntryPointOf(invoke)), StringComparer.OrdinalIgnoreCase)))
            .Select(static assembly => assembly.Name)
            .Concat(tree
                .Where(static file => MappingEntryPoints.Any(entryPoint => Names(file.Text, entryPoint)))
                .Select(static file => file.Assembly))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// The source of every project that ships: the product assemblies the assurance system already
    /// parses, and the composition roots it does not.
    /// </summary>
    /// <remarks>
    /// A composition root's own source is in scope because a root is published and run. The two
    /// xunit projects are out of scope for the same reason they are out of the metadata sweep:
    /// this suite reads its own witnesses, and a rule that forbade a test naming
    /// <c>VirtualProtect</c> would forbid the rule that forbids it.
    /// </remarks>
    internal static IReadOnlyList<SourceUnit> Tree()
    {
        var units = AssuranceSources.Files
            .Select(static file => new SourceUnit(file.RelativePath, file.Assembly, file.Text))
            .ToList();

        foreach (var project in ComponentGraph.Projects.Where(static project => project.IsComposition))
        {
            var directory = Path.GetDirectoryName(project.Path)!;

            foreach (var path in Directory
                         .EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories)
                         .Where(path => !IsBuildOutput(directory, path)))
            {
                units.Add(new SourceUnit(
                    Path.GetRelativePath(ComponentGraph.Root, path).Replace('\\', '/'),
                    project.AssemblyName,
                    File.ReadAllText(path)));
            }
        }

        return units
            .OrderBy(static unit => unit.RelativePath, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>One source file, as much of it as these rules read.</summary>
    internal sealed record SourceUnit(string RelativePath, string Assembly, string Text);

    /// <summary>A constant declared in the arming path, with the file that declares it.</summary>
    internal sealed record ArmingConstant(string File, long? Value);

    /// <summary>The entry point half of a "module!entryPoint" description.</summary>
    private static string EntryPointOf(string platformInvoke)
    {
        var separator = platformInvoke.IndexOf('!', StringComparison.Ordinal);

        return separator < 0 ? platformInvoke : platformInvoke[(separator + 1)..];
    }

    /// <summary>An entry point with the Win32 ANSI or wide suffix removed, where that names one.</summary>
    private static string Normalize(string entryPoint)
    {
        if (entryPoint.Length < 2 || (entryPoint[^1] != 'A' && entryPoint[^1] != 'W'))
        {
            return entryPoint;
        }

        var trimmed = entryPoint[..^1];

        return MappingEntryPoints.Contains(trimmed, StringComparer.OrdinalIgnoreCase)
            ? trimmed
            : entryPoint;
    }

    /// <summary>Whether a source text names an identifier, as a whole word.</summary>
    /// <remarks>
    /// Whole words rather than substrings, so a rule about <c>mmap</c> does not fire on a member
    /// whose name happens to contain it, and case-sensitively, because these are the exports'
    /// own spellings.
    /// </remarks>
    private static bool Names(string text, string identifier) =>
        Word(identifier).IsMatch(text);

    private static readonly Dictionary<string, Regex> Words = [];

    private static Regex Word(string identifier)
    {
        lock (Words)
        {
            if (!Words.TryGetValue(identifier, out var word))
            {
                word = new Regex(@"\b" + Regex.Escape(identifier) + @"\b", RegexOptions.Compiled);
                Words[identifier] = word;
            }

            return word;
        }
    }

    /// <summary>Whether a protection constant admits a write and an execute at the same time.</summary>
    /// <remarks>
    /// Read under the encoding of the half that declares it, and under both where the shared file
    /// declares it. Win32 names whole protections and Unix names bits, so one comparison cannot
    /// serve for both: <c>0x40</c> is a write-and-execute protection on Windows and is nothing at
    /// all under the Unix bits, while <c>0x6</c> is a write-and-execute protection on Unix and is
    /// not a Win32 protection constant.
    /// </remarks>
    private static bool WriteAndExecute(ArmingConstant constant)
    {
        if (constant.Value is not { } value)
        {
            return false;
        }

        var windows = !constant.File.EndsWith(".Unix.cs", StringComparison.Ordinal);
        var unix = !constant.File.EndsWith(".Windows.cs", StringComparison.Ordinal);

        return (windows && WindowsWriteAndExecute.Contains((int)value)) ||
            (unix && (value & UnixWriteAndExecute) == UnixWriteAndExecute);
    }

    /// <summary>Every constant field the arming path declares, with the value it evaluates to.</summary>
    private static Dictionary<string, ArmingConstant> Constants(IReadOnlyList<SourceUnit> arming)
    {
        var declarations = new Dictionary<string, (string File, ExpressionSyntax? Initializer)>(
            StringComparer.Ordinal);

        foreach (var file in arming)
        {
            foreach (var field in Parse(file).DescendantNodes().OfType<FieldDeclarationSyntax>()
                         .Where(static field => field.Modifiers.Any(SyntaxKind.ConstKeyword)))
            {
                foreach (var declarator in field.Declaration.Variables)
                {
                    declarations[declarator.Identifier.ValueText] =
                        (file.RelativePath, declarator.Initializer?.Value);
                }
            }
        }

        return declarations.ToDictionary(
            static entry => entry.Key,
            entry => new ArmingConstant(
                entry.Value.File, Evaluate(entry.Value.Initializer, declarations)),
            StringComparer.Ordinal);
    }

    /// <summary>
    /// A constant expression's value, where this evaluator can decide it.
    /// </summary>
    /// <remarks>
    /// Literals, parentheses, a bitwise or, and a reference to another constant of the arming path.
    /// That is the whole grammar the protection constants are written in, and an expression outside
    /// it evaluates to nothing rather than to a guess - which makes the rule silent about it, and
    /// is why the clause that requires a protection argument to NAME one of these constants is the
    /// clause doing the work.
    /// </remarks>
    private static long? Evaluate(
        ExpressionSyntax? expression,
        IReadOnlyDictionary<string, (string File, ExpressionSyntax? Initializer)> declarations,
        int depth = 0)
    {
        if (expression is null || depth > 8)
        {
            return null;
        }

        switch (expression)
        {
            case ParenthesizedExpressionSyntax parenthesized:
                return Evaluate(parenthesized.Expression, declarations, depth + 1);

            case LiteralExpressionSyntax literal
                when literal.Token.Value is int or uint or long or ulong or short or ushort or byte:
                return Convert.ToInt64(literal.Token.Value, System.Globalization.CultureInfo.InvariantCulture);

            case BinaryExpressionSyntax binary when binary.IsKind(SyntaxKind.BitwiseOrExpression):
                var left = Evaluate(binary.Left, declarations, depth + 1);
                var right = Evaluate(binary.Right, declarations, depth + 1);

                return left is { } l && right is { } r ? l | r : null;

            case IdentifierNameSyntax name
                when declarations.TryGetValue(name.Identifier.ValueText, out var referenced):
                return Evaluate(referenced.Initializer, declarations, depth + 1);

            default:
                return null;
        }
    }

    /// <summary>
    /// Each method the arming path declares that takes a protection, and which argument it is.
    /// </summary>
    private static Dictionary<string, int> ProtectionPositions(IReadOnlyList<SourceUnit> arming)
    {
        var positions = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var file in arming)
        {
            foreach (var method in Parse(file).DescendantNodes().OfType<MethodDeclarationSyntax>())
            {
                var parameters = method.ParameterList.Parameters;

                for (var index = 0; index < parameters.Count; index++)
                {
                    if (string.Equals(
                            parameters[index].Identifier.ValueText,
                            ProtectionParameter,
                            StringComparison.Ordinal))
                    {
                        positions[method.Identifier.ValueText] = index;
                    }
                }
            }
        }

        return positions;
    }

    /// <summary>Every argument the arming path passes into a protection position.</summary>
    private static IEnumerable<(SourceUnit File, ExpressionSyntax Argument, string Method)>
        ProtectionArguments(IReadOnlyList<SourceUnit> arming, IReadOnlyDictionary<string, int> positions)
    {
        foreach (var file in arming)
        {
            foreach (var call in Parse(file).DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var name = call.Expression switch
                {
                    IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
                    MemberAccessExpressionSyntax member => member.Name.Identifier.ValueText,
                    _ => null,
                };

                if (name is null ||
                    !positions.TryGetValue(name, out var index) ||
                    index >= call.ArgumentList.Arguments.Count)
                {
                    continue;
                }

                yield return (file, call.ArgumentList.Arguments[index].Expression, name);
            }
        }
    }

    private static readonly Dictionary<string, SyntaxNode> Parsed = [];

    /// <summary>One parse per file, under the options every other reader of this tree uses.</summary>
    private static SyntaxNode Parse(SourceUnit file)
    {
        lock (Parsed)
        {
            // Keyed on the text and not on the path, because a witness stands in for an arming
            // file at that file's own path and the two must not share a parse.
            if (!Parsed.TryGetValue(file.Text, out var root))
            {
                root = AssuranceSources.Parse(file.Text, file.RelativePath).GetRoot();
                Parsed[file.Text] = root;
            }

            return root;
        }
    }

    private static bool IsBuildOutput(string projectDirectory, string path)
    {
        var relative = Path.GetRelativePath(projectDirectory, path).Replace('\\', '/');
        var first = relative.Split('/')[0];

        return string.Equals(first, "bin", StringComparison.Ordinal) ||
            string.Equals(first, "obj", StringComparison.Ordinal);
    }
}
