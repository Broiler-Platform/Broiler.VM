// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rules X2 and X3: what the baseline native form hands emitted code, and where native code may
/// enter managed code and find the activation it runs for.
/// </summary>
/// <remarks>
/// <para>
/// <b>They are in group X because they are the rooting half of the property X1 is the mapping half
/// of.</b> Rule X1 pins where memory is made executable. Once it is, two further things decide
/// whether emitted code can reach a managed object the collector does not know it holds: what the
/// frame it is handed contains, and which managed code it can call. Decision JSD-0025 argues both,
/// and neither argument is visible at any call site.
/// </para>
/// <para>
/// Each rule is asserted twice, as every group here is: the checkout is clean, and the rule rejects
/// a violating input.
/// </para>
/// </remarks>
public sealed class NativeBaselineRuleTests
{
    /// <summary>The assembly the baseline frame is declared in.</summary>
    internal const string FrameAssembly = "Broiler.VM.Profile.JavaScript.Format";

    /// <summary>The baseline frame, by its full name.</summary>
    internal const string FrameType = "Broiler.VM.Profile.JavaScript.Format.JsBaselineFrame";

    /// <summary>The one file whose methods native code may call.</summary>
    internal const string HandlerFile = "src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs";

    /// <summary>The one file outside the activation that may read or write the thread slot.</summary>
    internal const string EnteringFile = "src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs";

    /// <summary>The file that declares the thread slot and the step that reads it.</summary>
    internal const string ActivationFile = "src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs";

    /// <summary>The shipping source tree rule X1 sweeps, read once.</summary>
    private static readonly IReadOnlyList<NativeMappingRules.SourceUnit> Tree =
        NativeMappingRules.Tree();

    [Fact]
    public void X2_the_baseline_frame_holds_no_reference()
    {
        // Non-vacuous before the clean answer is read: the Format assembly's build output was
        // found, the frame is in it, it is a value type, and the reader decided both of its fields
        // by name. A rule that passes by finding nothing would otherwise pass on an unbuilt tree.
        var (violations, fields) = InspectFrame(
            context => context.LoadFromAssemblyPath(BuildOutput(FrameAssembly)));

        Assert.Contains("Handlers", fields);
        Assert.Contains("Cookie", fields);
        Assert.Empty(violations);
    }

    /// <summary>
    /// A frame that carries a reference is reported, whether the field is one or a value type
    /// holds one.
    /// </summary>
    /// <remarks>
    /// The witness is source, compiled here, and read by the same reader that cleared the built
    /// assembly. A type compiled into this test assembly would do the same job for a metadata rule,
    /// as it does for B5c; it is not done that way because the witness has to BE the frame, at the
    /// frame's full name, and a second type of that name in this assembly would be a type every
    /// other reader of this assembly could mistake for the real one.
    /// </remarks>
    [Fact]
    public void X2_A_Baseline_Frame_Field_Holding_A_Reference_Is_Reported()
    {
        var compiled = CompileWitness("X2-a-baseline-frame-field-holding-a-reference.cs.witness");

        var (violations, fields) = InspectFrame(context => context.LoadFromByteArray(compiled));

        Assert.Contains(violations, message => message.Contains(
            "declares Activation, which holds a reference at Activation", StringComparison.Ordinal));

        Assert.Contains(violations, message => message.Contains(
            "declares Resume, which holds a reference at Resume.Pending", StringComparison.Ordinal));

        // ...and the accepting direction inside the same witness: the real frame's two fields are
        // an address and an integer, and a rule that reported them would be a rule about structs.
        Assert.Contains("Handlers", fields);
        Assert.DoesNotContain(violations, static message => message.Contains(
            "declares Handlers", StringComparison.Ordinal));
        Assert.DoesNotContain(violations, static message => message.Contains(
            "declares Cookie", StringComparison.Ordinal));
        Assert.Equal(2, violations.Count);
    }

    /// <summary>The rule reports a frame it could not judge rather than passing over it.</summary>
    [Fact]
    public void X2_Reports_A_Frame_It_Could_Not_Judge()
    {
        Assert.Contains(
            X2(null),
            static message => message.Contains("no type named", StringComparison.Ordinal));

        Assert.Contains(
            X2(typeof(NativeBaselineRuleTests)),
            static message => message.Contains("is not a value type", StringComparison.Ordinal));

        Assert.Contains(
            X2(typeof(EmptyFrame)),
            static message => message.Contains("declares no field", StringComparison.Ordinal));
    }

    [Fact]
    public void X2_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            static rule => string.Equals(rule.Id, "X2", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Equal("0001", row.OwningAdr);
        Assert.Contains("JSD-0025", row.Statement, StringComparison.Ordinal);

        // The row must say what the rule does not decide. A pointer is not a reference and passes,
        // which is what lets the table address sit in the frame - and it also means an address of a
        // managed object stored as an integer passes, so the row names what holds that shut instead.
        Assert.Contains("pointer", row.NonVacuousWhen, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ADDRESSES", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("build output", row.NonVacuousWhen, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void X3_Native_Code_Enters_And_Finds_Its_Activation_Only_Where_The_Record_Argues()
    {
        // Non-vacuous before the clean answer is read, the way X1 is: every file the rule names is
        // in the sweep by path, a composition root's source is in it, and the sweep is over a real
        // tree rather than whatever a broken path expression returned.
        foreach (var path in new[] { HandlerFile, EnteringFile, ActivationFile })
        {
            Assert.Contains(
                Tree,
                file => string.Equals(file.RelativePath, path, StringComparison.Ordinal));
        }

        Assert.Contains(
            Tree,
            static file => file.Assembly.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal));

        Assert.True(Tree.Count > 100);

        // The sweep is the shipping tree, and a vendored component is not in it. The row states that
        // limit by directory, and this is what keeps the limit the one the row states.
        Assert.DoesNotContain(Tree, static file => ComponentGraph.VendoredComponents.Any(directory =>
            file.RelativePath.StartsWith(directory, StringComparison.Ordinal)));

        Assert.Empty(X3(Tree));
    }

    /// <summary>
    /// An unmanaged entry outside the handler file is reported, and so is its use of the slot.
    /// </summary>
    [Fact]
    public void X3_An_Unmanaged_Entry_Outside_The_Handler_File_Is_Reported()
    {
        var violations = X3(
            [.. Tree, Witness(
                "X3-an-unmanaged-entry-outside-the-handler-file.cs.witness",
                "src/Broiler.VM.Profile.JavaScript/JsNativeHostCallbacks.cs",
                "Broiler.VM.Profile.JavaScript")])
            .ToArray();

        Assert.Contains(violations, static message => message.Contains(
            "JsNativeHostCallbacks.cs names UnmanagedCallersOnly", StringComparison.Ordinal));

        Assert.Contains(violations, static message => message.Contains(
            "JsNativeHostCallbacks.cs names JsNativeActivation.Current", StringComparison.Ordinal));

        Assert.Equal(2, violations.Length);
    }

    /// <summary>
    /// Inside the activation, a second writer or a second reader of the slot's field is reported.
    /// </summary>
    /// <remarks>
    /// The rejecting directions are the real file with one member added, asserted by the content of
    /// the violation each produces. A stored copy of the activation would go stale at its next edit
    /// and be repaired by copying it again - the reason the group H coverage clauses mutate rather
    /// than store.
    /// </remarks>
    [Fact]
    public void X3_A_Second_Writer_Or_Reader_Of_The_Slot_Is_Reported()
    {
        var activation = Tree.Single(static file =>
            string.Equals(file.RelativePath, ActivationFile, StringComparison.Ordinal));

        var writer = SlotViolations(WithMember(
            activation, "internal static void Park(JsNativeActivation parked) => current = parked;"));

        Assert.Contains(writer, static message => message.Contains(
            "writes the thread slot current in Park", StringComparison.Ordinal));

        var reader = SlotViolations(WithMember(
            activation, "internal static bool Busy => current is not null;"));

        Assert.Contains(reader, static message => message.Contains(
            "reads the thread slot current in Busy", StringComparison.Ordinal));

        var property = SlotViolations(WithMember(
            activation, "internal static bool Entered => Current is not null;"));

        Assert.Contains(property, static message => message.Contains(
            "names Current in Entered", StringComparison.Ordinal));

        var partial = SlotViolations(activation with
        {
            Text = activation.Text.Replace(
                "internal sealed unsafe class JsNativeActivation",
                "internal sealed unsafe partial class JsNativeActivation",
                StringComparison.Ordinal),
        });

        Assert.Contains(partial, static message => message.Contains(
            "is partial", StringComparison.Ordinal));

        // A qualifier the parser spells differently is still the type.
        var qualified = SlotViolations(WithMember(
            activation,
            "internal static void Park(JsNativeActivation parked) => " +
            "global::Broiler.VM.Profile.JavaScript.JsNativeActivation.current = parked;"));

        Assert.Contains(qualified, static message => message.Contains(
            "writes the thread slot current in Park", StringComparison.Ordinal));

        // A write inside Step is not the read Step is allowed, however the target is wrapped.
        foreach (var write in new[] { "(current) = null;", "(current, _) = (null, 0);" })
        {
            var stepWrite = SlotViolations(activation with
            {
                Text = activation.Text.Replace(
                    "var act = current;",
                    "var act = current;\n        " + write,
                    StringComparison.Ordinal),
            });

            Assert.Contains(stepWrite, static message => message.Contains(
                "writes the thread slot current in Step", StringComparison.Ordinal));
        }
    }

    /// <summary>
    /// A second type declared beside the activation, in its own file, cannot name the slot unseen.
    /// </summary>
    [Fact]
    public void X3_A_Type_Beside_The_Activation_Naming_The_Slot_Is_Reported()
    {
        var activation = Tree.Single(static file =>
            string.Equals(file.RelativePath, ActivationFile, StringComparison.Ordinal));

        var sibling = SlotViolations(activation with
        {
            Text = activation.Text +
                "\ninternal static class JsStepHelper\n{\n" +
                "    internal static JsNativeActivation? Peek() => JsNativeActivation.Current;\n}\n",
        }).ToArray();

        Assert.Contains(sibling, static message => message.Contains(
            "names JsNativeActivation.Current outside JsNativeActivation", StringComparison.Ordinal));

        // ...and the real file, which declares nothing beside the activation, is not reported.
        Assert.DoesNotContain(SlotViolations(activation), static message => message.Contains(
            "outside JsNativeActivation", StringComparison.Ordinal));
    }

    /// <summary>An alias of the activation, anywhere in the tree, is reported.</summary>
    [Fact]
    public void X3_An_Alias_Of_The_Activation_Is_Reported()
    {
        var violations = X3(
            [.. Tree, new NativeMappingRules.SourceUnit(
                "src/Broiler.VM.Profile.JavaScript/JsSlotPeek.cs",
                "Broiler.VM.Profile.JavaScript",
                "using Slot = Broiler.VM.Profile.JavaScript.JsNativeActivation;\n" +
                "namespace Broiler.VM.Profile.JavaScript;\n" +
                "internal static class JsSlotPeek { internal static bool Busy => Slot.Current is not null; }\n")])
            .ToArray();

        Assert.Contains(violations, static message => message.Contains(
            "JsSlotPeek.cs aliases JsNativeActivation", StringComparison.Ordinal));
        Assert.Single(violations);
    }

    /// <summary>
    /// The rule reports its own vacuity rather than passing over an input it never found.
    /// </summary>
    [Fact]
    public void X3_Reports_An_Input_In_Which_It_Found_Nothing_To_Quantify_Over()
    {
        var empty = X3([]).ToArray();

        Assert.Contains(empty, static message => message.Contains(
            $"{HandlerFile} is not in this input", StringComparison.Ordinal));
        Assert.Contains(empty, static message => message.Contains(
            $"{EnteringFile} is not in this input", StringComparison.Ordinal));
        Assert.Contains(empty, static message => message.Contains(
            $"{ActivationFile} is not in this input", StringComparison.Ordinal));

        var hollow = X3(
            [
                new NativeMappingRules.SourceUnit(
                    HandlerFile, "Broiler.VM.Profile.JavaScript",
                    "namespace Broiler.VM.Profile.JavaScript; internal static class JsBaselineHandlers { }"),
                new NativeMappingRules.SourceUnit(
                    EnteringFile, "Broiler.VM.Profile.JavaScript",
                    "namespace Broiler.VM.Profile.JavaScript; internal sealed partial class JsEngine { }"),
                new NativeMappingRules.SourceUnit(
                    ActivationFile, "Broiler.VM.Profile.JavaScript",
                    "namespace Broiler.VM.Profile.JavaScript; internal sealed class JsNativeActivation { }"),
            ])
            .ToArray();

        Assert.Contains(hollow, static message => message.Contains(
            "the handler file names no UnmanagedCallersOnly", StringComparison.Ordinal));
        Assert.Contains(hollow, static message => message.Contains(
            "writes no JsNativeActivation.Current", StringComparison.Ordinal));
        Assert.Contains(hollow, static message => message.Contains(
            "declares no thread-static field", StringComparison.Ordinal));
    }

    [Fact]
    public void X3_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            static rule => string.Equals(rule.Id, "X3", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Equal("0001", row.OwningAdr);
        Assert.Contains("JSD-0025", row.Statement, StringComparison.Ordinal);
        Assert.Contains("N20", row.Statement, StringComparison.Ordinal);

        // The statement is scoped to the tree the rule sweeps, and the row names every component that
        // sweep leaves out - one of them declares native callbacks of its own, so an unscoped
        // statement would claim a property the repository does not have.
        Assert.Contains("shipping source tree", row.Statement, StringComparison.Ordinal);

        foreach (var directory in ComponentGraph.VendoredComponents)
        {
            Assert.Contains(directory, row.NonVacuousWhen, StringComparison.Ordinal);
        }

        // The design named a third clause - no write through a frame pointer outside two files -
        // and the rule does not decide it. The row has to say so, and say what holds it instead,
        // or a reader takes the design's three clauses for three checks.
        Assert.Contains("DROPPED AND NOT CLAIMED", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("S4", row.NonVacuousWhen, StringComparison.Ordinal);
    }

    /// <summary>
    /// X2: the baseline frame declares no field whose type is, or contains, a reference.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every field the type declares, static ones included, and every instance field of every
    /// value type below them.</b> A static field is not in the layout emitted code is handed, and it
    /// is checked anyway: the statement says "declares no field", forbidding one costs nothing, and
    /// a rule narrower than its statement is the defect this register exists against.
    /// </para>
    /// <para>
    /// <b>A pointer, a function pointer, a primitive and an enum pass, and so does nothing
    /// else.</b> The first two are what let the table address sit here, and they are the stated limit
    /// of this rule: an integer or a pointer is not traced, so what it ADDRESSES is not this rule's
    /// question. A managed pointer is a reference, a generic parameter is decided as one because
    /// nothing bounds what it is instantiated with, and a value type is walked field by field.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> X2(Type? frame)
    {
        if (frame is null)
        {
            yield return
                $"the reader found no type named {FrameType}, so this rule judged no frame at all";

            yield break;
        }

        if (!frame.IsValueType)
        {
            yield return
                $"{frame.FullName} is not a value type, and a frame handed to emitted code by address " +
                "has to be one";

            yield break;
        }

        var fields = frame.GetFields(AllDeclared);

        if (fields.Length == 0)
        {
            yield return $"{frame.FullName} declares no field, so this rule judged no layout";
        }

        foreach (var field in fields)
        {
            if (ReferenceIn(field.FieldType, field.Name, []) is { } found)
            {
                yield return
                    $"{frame.FullName} declares {field.Name}, which holds a reference at {found.Path} " +
                    $"({found.Type}), and a frame emitted code is handed is one the collector does not scan";
            }
        }
    }

    /// <summary>What X2 says about the built Format assembly, for the group X report.</summary>
    internal static IEnumerable<string> X2Report() =>
        InspectFrame(static context => context.LoadFromAssemblyPath(BuildOutput(FrameAssembly))).Violations;

    /// <summary>
    /// X3: native code enters managed code only through the handler file, and the activation slot
    /// is written and read only where decision JSD-0025 argues it is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Text across the tree, syntax inside the activation.</b> Whether a file names the attribute
    /// or the slot is a question a whole-word scan answers for every file, comments included, which
    /// is the conservative direction. Which member of the activation touches the slot's private
    /// field is not, because the field's name is an ordinary word the dispatch loop uses for a local;
    /// the one file that can reach the field is parsed, and the rule insists the type is not partial,
    /// which is what makes that one file the whole of where the field can be reached.
    /// </para>
    /// <para>
    /// <b>The rule reports its own vacuity for every place it pins.</b> A named file missing from
    /// the input, a handler file with no entry in it, an entering file that never sets the slot, and
    /// an activation whose step no longer reads it are failures rather than clean results.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> X3(IReadOnlyList<NativeMappingRules.SourceUnit> tree)
    {
        NativeMappingRules.SourceUnit? Named(string path) => tree.FirstOrDefault(file =>
            string.Equals(file.RelativePath, path, StringComparison.Ordinal));

        var handler = Named(HandlerFile);
        var entering = Named(EnteringFile);
        var activation = Named(ActivationFile);

        foreach (var (path, file) in new[]
                 {
                     (HandlerFile, handler), (EnteringFile, entering), (ActivationFile, activation),
                 })
        {
            if (file is null)
            {
                yield return
                    $"{path} is not in this input, so this rule pins a place it never found";
            }
        }

        foreach (var file in tree)
        {
            if (!string.Equals(file.RelativePath, HandlerFile, StringComparison.Ordinal) &&
                UnmanagedEntry.IsMatch(file.Text))
            {
                yield return
                    $"{file.RelativePath} names UnmanagedCallersOnly, and the one file whose methods " +
                    $"native code may call is {HandlerFile}";
            }

            if (!string.Equals(file.RelativePath, EnteringFile, StringComparison.Ordinal) &&
                !string.Equals(file.RelativePath, ActivationFile, StringComparison.Ordinal) &&
                SlotNamed.IsMatch(file.Text))
            {
                yield return
                    $"{file.RelativePath} names JsNativeActivation.Current, and outside the activation " +
                    $"the one file that may set or read the thread slot is {EnteringFile}";
            }

            if (StaticImport.IsMatch(file.Text))
            {
                yield return
                    $"{file.RelativePath} imports JsNativeActivation's static members, which lets it " +
                    "name the thread slot without naming the type";
            }

            if (TypeAlias.IsMatch(file.Text))
            {
                yield return
                    $"{file.RelativePath} aliases JsNativeActivation, which lets it name the thread slot " +
                    "under a name this rule does not read";
            }
        }

        if (handler is not null && !UnmanagedEntry.IsMatch(handler.Text))
        {
            yield return
                "the handler file names no UnmanagedCallersOnly, so the one place this rule pins is " +
                "not the place native code calls";
        }

        if (entering is not null && !SlotWritten.IsMatch(entering.Text))
        {
            yield return
                $"{EnteringFile} writes no JsNativeActivation.Current, so the one writer this rule " +
                "allows is not the place the slot is set";
        }

        if (activation is not null)
        {
            foreach (var message in SlotViolations(activation))
            {
                yield return message;
            }
        }
    }

    /// <summary>
    /// Inside the activation: the slot is one private thread-static field of a type that is not
    /// partial, written only by <c>Current</c>'s setter and read only by its getter and by
    /// <c>Step</c>, and the property is not named inside its own type.
    /// </summary>
    internal static IEnumerable<string> SlotViolations(NativeMappingRules.SourceUnit activation)
    {
        var root = AssuranceSources.Parse(activation.Text, activation.RelativePath).GetRoot();

        var type = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(static type =>
            string.Equals(type.Identifier.ValueText, "JsNativeActivation", StringComparison.Ordinal));

        if (type is null)
        {
            yield return $"{activation.RelativePath} declares no JsNativeActivation";
            yield break;
        }

        if (root.DescendantNodes().OfType<ClassDeclarationSyntax>().Count(static other =>
                string.Equals(other.Identifier.ValueText, "JsNativeActivation", StringComparison.Ordinal)) > 1)
        {
            yield return
                $"{activation.RelativePath} declares more than one type named JsNativeActivation, so the " +
                "one this rule parses need not be the one that holds the slot";
        }

        // The rest of the file is read the way every other file is. The tree scan leaves this file
        // out because its class names the slot by design, so without this a second type declared
        // beside the activation - a step helper, say - could name the slot and nothing would see it.
        var outside = activation.Text[..type.Span.Start] + "\n" + activation.Text[type.Span.End..];

        if (SlotNamed.IsMatch(outside))
        {
            yield return
                $"{activation.RelativePath} names JsNativeActivation.Current outside JsNativeActivation, " +
                $"and outside the activation the one file that may set or read the thread slot is {EnteringFile}";
        }

        if (type.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            yield return
                $"{activation.RelativePath} declares JsNativeActivation, which is partial, so its " +
                "private thread slot is reachable from a file this rule does not parse";
        }

        var slots = type.Members
            .OfType<FieldDeclarationSyntax>()
            .Where(static field => field.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                field.AttributeLists.SelectMany(static list => list.Attributes).Any(static attribute =>
                    ThreadStaticNames.Contains(attribute.Name.ToString(), StringComparer.Ordinal)))
            .SelectMany(static field => field.Declaration.Variables.Select(variable => (field, variable)))
            .ToArray();

        if (slots.Length == 0)
        {
            yield return
                $"{activation.RelativePath} declares no thread-static field, so the slot this rule " +
                "governs is not where the rule looks for it";

            yield break;
        }

        if (slots.Length > 1)
        {
            yield return
                $"{activation.RelativePath} declares {slots.Length} thread-static fields, and the " +
                "activation has one slot";
        }

        var (declaration, slot) = slots[0];
        var name = slot.Identifier.ValueText;

        if (declaration.Modifiers.Any(static modifier => modifier.Kind() is SyntaxKind.PublicKeyword
                or SyntaxKind.InternalKeyword or SyntaxKind.ProtectedKeyword))
        {
            yield return
                $"{activation.RelativePath} declares the thread slot {name} wider than private, so a " +
                "file this rule does not parse can reach it";
        }

        var writes = 0;
        var stepReads = 0;

        foreach (var identifier in type.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            if (!IsOwnMember(identifier))
            {
                continue;
            }

            var text = identifier.Identifier.ValueText;
            var member = EnclosingMember(identifier);

            if (string.Equals(text, "Current", StringComparison.Ordinal))
            {
                yield return
                    $"{activation.RelativePath} names Current in {member.Name}, and inside the " +
                    "activation the slot is reached through its field";

                continue;
            }

            if (!string.Equals(text, name, StringComparison.Ordinal))
            {
                continue;
            }

            if (IsWrite(identifier))
            {
                if (member is { Name: "Current", Accessor: "set" })
                {
                    writes++;
                    continue;
                }

                yield return
                    $"{activation.RelativePath} writes the thread slot {name} in {member.Name}, and the " +
                    "one writer inside the activation is Current's setter";

                continue;
            }

            if (member is { Name: "Current", Accessor: "get" })
            {
                continue;
            }

            if (string.Equals(member.Name, "Step", StringComparison.Ordinal))
            {
                stepReads++;
                continue;
            }

            yield return
                $"{activation.RelativePath} reads the thread slot {name} in {member.Name}, and the one " +
                "reader inside the activation is Step, which runs nothing unless the frame's cookie is " +
                "the activation's";
        }

        if (writes == 0)
        {
            yield return
                $"no accessor of Current writes {name}, so the slot this rule governs is not the one " +
                "the entering frame sets";
        }

        if (stepReads == 0)
        {
            yield return
                $"Step reads no {name}, so the one reader this rule allows is not the place the slot " +
                "is read";
        }
    }

    /// <summary>The attribute that makes a method callable from native code, either spelling.</summary>
    private static readonly Regex UnmanagedEntry =
        new(@"\bUnmanagedCallersOnly(?:Attribute)?\b", RegexOptions.Compiled);

    /// <summary>The slot, named through its type.</summary>
    private static readonly Regex SlotNamed =
        new(@"\bJsNativeActivation\s*\.\s*Current\b", RegexOptions.Compiled);

    /// <summary>The slot, assigned through its type.</summary>
    private static readonly Regex SlotWritten =
        new(@"\bJsNativeActivation\s*\.\s*Current\s*(?:\?\?)?=(?!=)", RegexOptions.Compiled);

    /// <summary>A static import of the activation, which would let a file name the slot bare.</summary>
    private static readonly Regex StaticImport =
        new(@"\busing\s+static\s+(?:global::)?[\w.]*\bJsNativeActivation\s*;", RegexOptions.Compiled);

    /// <summary>An alias of the activation, which would let a file name the slot under another name.</summary>
    private static readonly Regex TypeAlias =
        new(@"\busing\s+\w+\s*=\s*(?:global::)?[\w.]*\bJsNativeActivation\s*;", RegexOptions.Compiled);

    private static readonly string[] ThreadStaticNames =
    [
        "ThreadStatic", "ThreadStaticAttribute", "System.ThreadStatic", "System.ThreadStaticAttribute",
    ];

    private const BindingFlags AllDeclared =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
        BindingFlags.DeclaredOnly;

    private const BindingFlags InstanceDeclared =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    /// <summary>Where a type holds a reference, if it holds one, as a dotted field path.</summary>
    private static (string Path, string Type)? ReferenceIn(Type type, string path, HashSet<Type> walking)
    {
        if (type.IsPointer || type.IsFunctionPointer || type.IsPrimitive || type.IsEnum)
        {
            return null;
        }

        if (type.IsByRef || !type.IsValueType)
        {
            return (path, type.FullName ?? type.Name);
        }

        if (!walking.Add(type))
        {
            return null;
        }

        foreach (var field in type.GetFields(InstanceDeclared))
        {
            if (ReferenceIn(field.FieldType, path + "." + field.Name, walking) is { } found)
            {
                return found;
            }
        }

        return null;
    }

    /// <summary>
    /// Runs X2 over the frame as one assembly declares it, inside a context that runs nothing.
    /// </summary>
    /// <remarks>
    /// The context is <see cref="MetadataLoadContext"/> for the reason <see cref="ProfileApiSurface"/>
    /// gives: loading an assembly runs its module initializers, and a rule about what a type holds
    /// has no business executing the type. The answers are materialised before the context goes.
    /// </remarks>
    private static (IReadOnlyList<string> Violations, IReadOnlyList<string> Fields) InspectFrame(
        Func<MetadataLoadContext, Assembly> load)
    {
        var resolverPaths = new List<string>();
        var built = BuildOutput(FrameAssembly);

        if (File.Exists(built))
        {
            resolverPaths.Add(built);
        }

        resolverPaths.AddRange(Directory.EnumerateFiles(
            Path.GetDirectoryName(typeof(object).Assembly.Location)!, "*.dll"));
        resolverPaths.AddRange(Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll"));

        using var context = new MetadataLoadContext(
            new PathAssemblyResolver(resolverPaths.Distinct(StringComparer.OrdinalIgnoreCase)));

        var frame = load(context).GetType(FrameType, throwOnError: false);

        return (
            X2(frame).ToArray(),
            frame is null ? [] : frame.GetFields(AllDeclared).Select(static field => field.Name).ToArray());
    }

    /// <summary>Where one of the profile family's assemblies was built by this run's configuration.</summary>
    private static string BuildOutput(string assemblyName) => Path.Combine(
        ComponentGraph.Root, "src", assemblyName, "bin", ProfileApiSurface.Configuration,
        ProfileApiSurface.TargetFramework, assemblyName + ".dll");

    /// <summary>A stored source witness, compiled in memory against this runtime's framework.</summary>
    private static byte[] CompileWitness(string fileName)
    {
        var path = WitnessPath(fileName);
        var framework = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Where(reference => string.Equals(
                Path.GetDirectoryName(reference), framework, StringComparison.OrdinalIgnoreCase))
            .Select(static reference => MetadataReference.CreateFromFile(reference));

        var compilation = CSharpCompilation.Create(
            Path.GetFileNameWithoutExtension(fileName).Replace('-', '_').Replace('.', '_'),
            [AssuranceSources.Parse(File.ReadAllText(path), path)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        using var stream = new MemoryStream();
        var emitted = compilation.Emit(stream);

        Assert.True(
            emitted.Success,
            $"{fileName} did not compile, so it witnesses nothing: " +
            string.Join("; ", emitted.Diagnostics.Where(static diagnostic =>
                diagnostic.Severity == DiagnosticSeverity.Error)));

        return stream.ToArray();
    }

    /// <summary>Whether an identifier names a member of the activation itself rather than of something else.</summary>
    /// <remarks>
    /// The qualifier is compared with its whitespace and any <c>global::</c> removed, and any
    /// qualifier ending in the type's simple name is the type: the slot's field is private and the
    /// property static, so no receiver but the type can name either, and every spelling of the type
    /// is one the rule has to count. An alias of the type is the one spelling this cannot see, and
    /// rule X3 reports every alias of it across the tree instead.
    /// </remarks>
    private static bool IsOwnMember(IdentifierNameSyntax identifier)
    {
        if (identifier.Parent is not MemberAccessExpressionSyntax access || access.Name != identifier)
        {
            return true;
        }

        var qualifier = string.Concat(access.Expression.ToString().Where(static c => !char.IsWhiteSpace(c)));

        if (qualifier.StartsWith("global::", StringComparison.Ordinal))
        {
            qualifier = qualifier["global::".Length..];
        }

        return qualifier is "JsNativeActivation" ||
            qualifier.EndsWith(".JsNativeActivation", StringComparison.Ordinal);
    }

    /// <summary>Whether an identifier is assigned to, deconstructed into, stepped, or passed by reference.</summary>
    /// <remarks>
    /// The target is walked up through parentheses and through the tuples of a deconstruction to the
    /// place that decides it, so <c>(current) = x</c> and <c>(current, _) = (x, 0)</c> are writes and
    /// not reads - a read is what the step is allowed, so a write mistaken for one would pass there.
    /// </remarks>
    private static bool IsWrite(IdentifierNameSyntax identifier)
    {
        SyntaxNode target = identifier.Parent is MemberAccessExpressionSyntax access && access.Name == identifier
            ? access
            : identifier;

        while (true)
        {
            switch (target.Parent)
            {
                case ParenthesizedExpressionSyntax parenthesized:
                    target = parenthesized;
                    continue;

                case ArgumentSyntax argument when argument.Parent is TupleExpressionSyntax tuple:
                    if (!argument.RefKindKeyword.IsKind(SyntaxKind.None))
                    {
                        return true;
                    }

                    target = tuple;
                    continue;

                case AssignmentExpressionSyntax assignment:
                    return assignment.Left == target;

                case ArgumentSyntax argument:
                    return !argument.RefKindKeyword.IsKind(SyntaxKind.None);

                case RefExpressionSyntax:
                    return true;

                case PrefixUnaryExpressionSyntax prefix:
                    return prefix.Kind() is SyntaxKind.PreIncrementExpression or SyntaxKind.PreDecrementExpression
                        or SyntaxKind.AddressOfExpression;

                case PostfixUnaryExpressionSyntax postfix:
                    return postfix.Kind() is SyntaxKind.PostIncrementExpression or SyntaxKind.PostDecrementExpression;

                case ForEachVariableStatementSyntax loop:
                    return loop.Variable == target;

                default:
                    return false;
            }
        }
    }

    /// <summary>The member an identifier sits in, and the accessor when it is in a property.</summary>
    private static (string Name, string? Accessor) EnclosingMember(SyntaxNode node)
    {
        foreach (var ancestor in node.Ancestors())
        {
            switch (ancestor)
            {
                case AccessorDeclarationSyntax accessor
                    when accessor.Parent?.Parent is PropertyDeclarationSyntax owner:
                    return (owner.Identifier.ValueText, accessor.Keyword.ValueText);

                case PropertyDeclarationSyntax property:
                    return (property.Identifier.ValueText, "get");

                case MethodDeclarationSyntax method:
                    return (method.Identifier.ValueText, null);

                case ConstructorDeclarationSyntax constructor:
                    return (constructor.Identifier.ValueText, null);

                case BaseFieldDeclarationSyntax field:
                    return (string.Join(", ", field.Declaration.Variables.Select(static v => v.Identifier.ValueText)), null);

                case MemberDeclarationSyntax other:
                    return (other.Kind().ToString(), null);
            }
        }

        return ("the file", null);
    }

    /// <summary>The activation's real text with one member added before its closing brace.</summary>
    private static NativeMappingRules.SourceUnit WithMember(NativeMappingRules.SourceUnit file, string member)
    {
        var closing = file.Text.LastIndexOf('}');

        return file with { Text = file.Text[..closing] + "    " + member + "\n" + file.Text[closing..] };
    }

    /// <summary>A stored witness input, read as though it were the file it stands in for.</summary>
    private static NativeMappingRules.SourceUnit Witness(
        string fileName, string relativePath, string assembly) =>
        new(relativePath, assembly, File.ReadAllText(WitnessPath(fileName)));

    private static string WitnessPath(string fileName) => Path.Combine(
        ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", fileName);

    /// <summary>A value type with no field, for the vacuity direction.</summary>
    private struct EmptyFrame
    {
    }
}
