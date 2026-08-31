using System.Globalization;
using System.Reflection;
using System.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The complete public surface of the three product assemblies, as text.
/// </summary>
/// <remarks>
/// <para>
/// Group V fixes named properties of the surface - that a frozen name is exported, that a member
/// returns no task, that no member is called <c>Grant</c>. Every one of them is a claim about what
/// must or must not be there, and none of them is a claim about what IS there. A member added
/// tomorrow that breaks no V rule is a public API addition nothing in this repository notices, and
/// a member deleted tomorrow is a breaking change nothing notices either.
/// </para>
/// <para>
/// This is the enumeration those rules are not. It is deliberately exhaustive and deliberately
/// mechanical: every exported type, every public and protected member of it, with a signature
/// written out in full, sorted so the file is a function of the assemblies rather than of the
/// order reflection happened to return things in.
/// </para>
/// <para>
/// <b>Why constants carry their values.</b> <c>VmCoreContract.Version</c> is a literal, and a
/// literal's value is part of the contract in a way a field's type is not: changing 1 to 2 is the
/// amendment ADR 0003 governs, and a baseline that recorded only "there is an int called Version"
/// would not see it happen. The same argument applies to every other constant, so every constant
/// is recorded the same way rather than one being special-cased.
/// </para>
/// <para>
/// <b>What is deliberately included that a reader might not expect.</b> Compiler-generated public
/// members of records - <c>EqualityContract</c>, <c>&lt;Clone&gt;$</c>, <c>PrintMembers</c>, the
/// equality operators - are in the surface because they ARE in the surface: a consumer can call
/// them, and turning a record into a class removes them. Excluding them would make the baseline a
/// record of what this component meant to export rather than of what it exports.
/// </para>
/// </remarks>
internal static class ApiSurface
{
    /// <summary>The three packable assemblies, in dependency order.</summary>
    /// <remarks>
    /// Named rather than discovered. A baseline that enumerated whatever assemblies were in the
    /// test output would silently start covering the fixtures the day someone added a public type
    /// to them, and silently stop covering a product assembly the day one was renamed.
    /// </remarks>
    internal static readonly string[] PackableAssemblies =
    [
        "Broiler.VM.Abstractions",
        "Broiler.VM.Binary",
        "Broiler.VM.Runtime",
    ];

    /// <summary>Describes the public surface of the three product assemblies.</summary>
    internal static IReadOnlyList<string> Describe()
    {
        var lines = new List<string>();

        foreach (var name in PackableAssemblies)
        {
            lines.AddRange(Describe(Assembly.Load(name)));
        }

        return lines;
    }

    /// <summary>
    /// Describes one assembly's public surface, however that assembly was obtained.
    /// </summary>
    /// <remarks>
    /// <b>Two baselines, one describer.</b> The packable three arrive through
    /// <see cref="Assembly.Load(string)"/> because a ProjectReference puts them in the test
    /// output; the JavaScript profile family's three arrive through a
    /// <c>MetadataLoadContext</c> over their build output, because rule A11 forbids the reference
    /// that would put them here and loading them would run their module initializers. Only the
    /// loading differs, and every line of both baselines is written by the code below - so a
    /// reader who can read one file can read the other, and a change to how a member is spelled
    /// moves both.
    /// </remarks>
    internal static IEnumerable<string> Describe(Assembly assembly)
    {
        var name = assembly.GetName().Name!;
        var lines = new List<string>();

        foreach (var type in assembly.GetExportedTypes().OrderBy(Name, StringComparer.Ordinal))
        {
            lines.Add(DescribeType(name, type));
            lines.AddRange(DescribeMembers(name, type));
        }

        return lines;
    }

    private static string DescribeType(string assembly, Type type)
    {
        var builder = new StringBuilder();

        builder.Append("type ").Append(assembly).Append(' ').Append(Name(type)).Append(" : ");
        builder.Append(Kind(type));

        var bases = new List<string>();

        // Compared by NAME rather than against typeof(object). A type described through a
        // MetadataLoadContext has its own System.Object, which is not this runtime's, so the
        // identity comparison was false for every profile type and wrote "System.Object" into one
        // baseline and not the other - two spellings of one describer, which is the thing having
        // one describer was for.
        if (type is { IsClass: true, BaseType: not null } && Name(type.BaseType) != "System.Object")
        {
            bases.Add(Name(type.BaseType));
        }

        // Declared interfaces only. The full interface map includes everything a base type
        // implements, which would make an unrelated change to a base look like a change here.
        var inherited = type.BaseType?.GetInterfaces() ?? [];

        bases.AddRange(type
            .GetInterfaces()
            .Where(candidate => !inherited.Contains(candidate))
            .Select(Name)
            .OrderBy(static text => text, StringComparer.Ordinal));

        if (bases.Count > 0)
        {
            builder.Append(' ').Append(string.Join(", ", bases));
        }

        return builder.ToString();
    }

    private static IEnumerable<string> DescribeMembers(string assembly, Type type)
    {
        const BindingFlags Flags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        var lines = new List<string>();

        foreach (var member in type.GetMembers(Flags))
        {
            var described = Describe(assembly, type, member);

            if (described is not null)
            {
                lines.Add(described);
            }
        }

        lines.Sort(StringComparer.Ordinal);
        return lines;
    }

    private static string? Describe(string assembly, Type type, MemberInfo member)
    {
        var prefix = $"  {assembly} {Name(type)}.";

        switch (member)
        {
            case FieldInfo field when Visible(field.IsPublic, field.IsFamily, field.IsFamilyOrAssembly):
                return prefix + field.Name + " : " + Modifiers(field) + Name(field.FieldType) +
                    (field.IsLiteral ? " = " + Literal(field.GetRawConstantValue()) : string.Empty);

            case PropertyInfo property:
            {
                var accessors = property.GetAccessors(nonPublic: true)
                    .Where(static accessor =>
                        Visible(accessor.IsPublic, accessor.IsFamily, accessor.IsFamilyOrAssembly))
                    .ToArray();

                if (accessors.Length == 0)
                {
                    return null;
                }

                var parameters = property.GetIndexParameters();

                var name = parameters.Length == 0
                    ? property.Name
                    : property.Name + "[" + string.Join(", ", parameters.Select(Signature)) + "]";

                return prefix + name + " : " + Modifiers(accessors[0]) + Name(property.PropertyType) +
                    " { " + string.Join(" ", accessors
                        .Select(static accessor => accessor.Name.Split('_')[0] + ";")
                        .OrderBy(static text => text, StringComparer.Ordinal)) + " }";
            }

            case EventInfo declared when declared.AddMethod is { } add &&
                Visible(add.IsPublic, add.IsFamily, add.IsFamilyOrAssembly):
                return prefix + declared.Name + " : event " + Name(declared.EventHandlerType!);

            case MethodBase method when Visible(method.IsPublic, method.IsFamily, method.IsFamilyOrAssembly):
            {
                // Accessors are described by their property or event, so describing them again
                // would record one member twice and make a get-only property look like a method.
                if (method is MethodInfo { IsSpecialName: true } accessor &&
                    (accessor.Name.StartsWith("get_", StringComparison.Ordinal) ||
                     accessor.Name.StartsWith("set_", StringComparison.Ordinal) ||
                     accessor.Name.StartsWith("add_", StringComparison.Ordinal) ||
                     accessor.Name.StartsWith("remove_", StringComparison.Ordinal)))
                {
                    return null;
                }

                var arguments = string.Join(", ", method.GetParameters().Select(Signature));

                var generics = method.IsGenericMethodDefinition
                    ? "<" + string.Join(", ", method.GetGenericArguments().Select(Name)) + ">"
                    : string.Empty;

                var returns = method is MethodInfo declared ? Name(declared.ReturnType) : "void";

                return prefix + method.Name + generics + "(" + arguments + ") : " +
                    Modifiers(method) + returns;
            }

            default:
                return null;
        }
    }

    /// <summary>
    /// Whether a member is part of the surface a consumer outside this component can reach.
    /// </summary>
    /// <remarks>
    /// Protected members count. A sealed type has none that matter, but an unsealed one's
    /// protected members are as binding a promise as its public ones - a derived type in another
    /// assembly compiles against them - and leaving them out would make sealing or unsealing a
    /// type invisible here. <c>protected internal</c> counts for the same reason; <c>private
    /// protected</c> does not, because no assembly outside this one can reach it.
    /// </remarks>
    private static bool Visible(bool isPublic, bool isFamily, bool isFamilyOrAssembly) =>
        isPublic || isFamily || isFamilyOrAssembly;

    private static string Modifiers(FieldInfo field) =>
        (field.IsLiteral ? "const " : field.IsStatic ? "static " : string.Empty) +
        (field.IsInitOnly ? "readonly " : string.Empty);

    private static string Modifiers(MethodBase method) =>
        (method.IsStatic ? "static " : string.Empty) +
        (method is { IsAbstract: true } ? "abstract " :
            method is { IsVirtual: true, IsFinal: false } ? "virtual " : string.Empty);

    private static string Signature(ParameterInfo parameter)
    {
        var direction = parameter.ParameterType.IsByRef
            ? parameter.IsOut ? "out " : parameter.IsIn ? "in " : "ref "
            : string.Empty;

        return direction + Name(parameter.ParameterType) + " " + (parameter.Name ?? "?");
    }

    /// <summary>
    /// A type's name, written the same way every time.
    /// </summary>
    /// <remarks>
    /// Reflection spells a generic type <c>List`1</c> and its arguments separately, a by-ref type
    /// with a trailing ampersand, and a nested type with a plus. All three are written out here,
    /// because a baseline is read by people and a diff of mangled names is not reviewable.
    /// </remarks>
    private static string Name(Type type)
    {
        if (type.IsByRef)
        {
            return Name(type.GetElementType()!);
        }

        if (type.IsArray)
        {
            return Name(type.GetElementType()!) + "[" + new string(',', type.GetArrayRank() - 1) + "]";
        }

        if (type.IsGenericParameter)
        {
            return type.Name;
        }

        var name = type.IsNested ? Name(type.DeclaringType!) + "." + type.Name : type.FullName ?? type.Name;

        if (!type.IsGenericType)
        {
            return name;
        }

        var tick = name.IndexOf('`', StringComparison.Ordinal);

        if (tick >= 0)
        {
            name = name[..tick];
        }

        return name + "<" + string.Join(", ", type.GetGenericArguments().Select(Name)) + ">";
    }

    private static string Kind(Type type) =>
        type.IsEnum ? "enum" :
        type.IsInterface ? "interface" :
        type.IsValueType ? (IsByRefLike(type) ? "ref struct" : "struct") :
        type.IsAbstract && type.IsSealed ? "static class" :
        type.IsAbstract ? "abstract class" :
        type.IsSealed ? "sealed class" : "class";

    private static bool IsByRefLike(Type type) => type.IsByRefLike;

    private static string Literal(object? value) =>
        value switch
        {
            null => "null",
            string text => "\"" + text + "\"",
            bool flag => flag ? "true" : "false",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? "?",
        };
}
