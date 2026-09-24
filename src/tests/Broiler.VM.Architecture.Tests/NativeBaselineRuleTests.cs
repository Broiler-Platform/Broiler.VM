// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rules X2, X3 and X4: what the baseline native form hands emitted code, where native code may
/// enter managed code and find the activation it runs for, and what each entry point runs once it
/// has.
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
/// <b>X4 is the per-block steps' half.</b> Once a baseline unit calls a handler only at block heads,
/// more properties rest on source no call site shows: each entry point routes by the partition under
/// its own name, the step still checks what it is handed although the template scan now makes those
/// checks unreachable from a verified payload, and the layout and a template's fixed bytes are named
/// only where comparing or emitting is the point.
/// </para>
/// <para>
/// Each rule is asserted twice, as every group here is: the checkout is clean, and the rule rejects
/// a violating input.
/// </para>
/// </remarks>
public sealed partial class NativeBaselineRuleTests
{
    /// <summary>The assembly the baseline frame is declared in.</summary>
    internal const string FrameAssembly = "Broiler.VM.Profile.JavaScript.Format";

    /// <summary>The baseline frame, by its full name.</summary>
    internal const string FrameType = "Broiler.VM.Profile.JavaScript.Format.JsBaselineFrame";

    /// <summary>The value form's frame context, by its full name: rule X2 holds it too (JSD-0035 section 9).</summary>
    internal const string ValueFrameType = "Broiler.VM.Profile.JavaScript.Format.JsValueFrame";

    /// <summary>The one file whose methods native code may call.</summary>
    internal const string HandlerFile = "src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs";

    /// <summary>The value form's helper file: the one other file whose methods native code may call.</summary>
    internal const string ValueHelperFile = "src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs";

    /// <summary>The one file outside the activation that may read or write the thread slot.</summary>
    internal const string EnteringFile = "src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs";

    /// <summary>The file that declares the thread slot and the step that reads it.</summary>
    internal const string ActivationFile = "src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs";

    /// <summary>The file that states the block partition, whose <c>RunsAlone</c> rule X4 routes by.</summary>
    internal const string BlocksFile = "src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs";

    /// <summary>The file that declares the opcodes, whose names are the names a wrapper may carry.</summary>
    internal const string OpcodeFile = "src/Broiler.VM.Profile.JavaScript.Format/JsOpcode.cs";

    /// <summary>The template scan: one of the two places rule X4 lets read the layout and the fixed bytes.</summary>
    internal const string ScanFile = "src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs";

    /// <summary>The file that declares a template and its fixed bytes.</summary>
    internal const string TemplatesFile = "src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs";

    /// <summary>The lowering assembly: the other place rule X4 lets read the layout and the fixed bytes.</summary>
    internal const string LoweringAssembly = "Broiler.VM.Profile.JavaScript.Compiler";

    /// <summary>The mode every entry point runs that does not run a step of its own opcode.</summary>
    internal const string BlockMode = "JsStepBlock";

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

        // ...and the value form's frame context, which the rule holds to the same statement.
        var (valueViolations, valueFields) = InspectFrame(
            context => context.LoadFromAssemblyPath(BuildOutput(FrameAssembly)), ValueFrameType);

        Assert.Contains("Helpers", valueFields);
        Assert.Contains("Cookie", valueFields);
        Assert.Empty(valueViolations);
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

    [Fact]
    public void X4_Every_Wrapper_Routes_By_The_Partition()
    {
        // Non-vacuous before the clean answer is read: the three files clause (a) reads are in the
        // sweep by path, and the clause decided an opcode on each side of the partition by name. A
        // routing rule that read no pattern or found no wrapper would otherwise pass.
        foreach (var path in new[] { HandlerFile, BlocksFile, OpcodeFile })
        {
            Assert.Contains(
                Tree,
                file => string.Equals(file.RelativePath, path, StringComparison.Ordinal));
        }

        var answer = X4Routing(Tree);

        Assert.Contains("Call runs alone", answer.Decided);
        Assert.Contains("Nop takes the block step", answer.Decided);
        Assert.Empty(answer.Violations);
    }

    [Fact]
    public void X4_Step_Keeps_Its_Pc_Bounds_Opcode_And_Cookie_Checks()
    {
        var answer = X4Checks(Tree);

        // Non-vacuous: every one of the four comparisons was found, by name, where the clause looks.
        foreach (var (name, _) in Comparisons)
        {
            Assert.Contains(name, answer.Decided);
        }

        Assert.Empty(answer.Violations);
    }

    [Fact]
    public void X4_Only_The_Scan_And_The_Lowering_Read_The_Layout_And_The_Fixed_Bytes()
    {
        // Non-vacuous: a file of the lowering assembly is in the sweep, and the clause saw both places
        // it allows actually read what it confines - a clause whose allowed places named nothing would
        // pass the same way over a tree in which the members had been renamed. The walk has its own two
        // places, the scan and Layout, and the clause saw each of them name it.
        Assert.Contains(
            Tree,
            static file => string.Equals(file.Assembly, LoweringAssembly, StringComparison.Ordinal));

        var answer = X4Confinement(Tree);

        Assert.Contains("the scan names Fixed", answer.Decided);
        Assert.Contains("the lowering names Layout", answer.Decided);
        Assert.Contains("the scan names Lay", answer.Decided);
        Assert.Contains("JsBaselineBlocks.Layout names Lay", answer.Decided);
        Assert.Empty(answer.Violations);
    }

    [Fact]
    public void X4_Each_Slot_Expected_Opcode_And_Step_Is_The_Wrappers_Own()
    {
        var answer = X4Names(Tree);

        foreach (var decided in new[]
                 {
                     "slot Call", "slot Nop", "expected Call", "expected Nop", "StepCall.Opcode", "the static constructor",
                     "Sound reads the slots",
                 })
        {
            Assert.Contains(decided, answer.Decided);
        }

        Assert.Empty(answer.Violations);
    }

    /// <summary>
    /// A run-alone opcode routed through the block step, and a step of its own for an opcode that does
    /// not run alone, are reported.
    /// </summary>
    [Fact]
    public void X4_A_Call_Routed_Through_The_Block_Step_Is_Reported()
    {
        var handlers = WithWitnessMembers(
            TreeFile(HandlerFile), "JsBaselineHandlers", "X4-a-call-routed-through-the-block-step.cs.witness");

        var violations = X4Routing(Replacing(handlers)).Violations;

        Assert.Contains(violations, static message => message.Contains(
            "(a) Call runs alone, and its wrapper routes Step<JsStepBlock>", StringComparison.Ordinal));

        Assert.Contains(violations, static message => message.Contains(
            "declares StepNop, a step of Nop's own opcode, and Nop does not run alone", StringComparison.Ordinal));

        Assert.Equal(2, violations.Count);

        // ...and the edit is routing alone: every slot, expected opcode and step's answer is still the
        // wrapper's own, so the clause that ties names does not see it and (a) is the only guard.
        Assert.Empty(X4Names(Replacing(handlers)).Violations);
    }

    /// <summary>A step whose opcode comparison was removed is reported, and nothing else in it is.</summary>
    [Fact]
    public void X4_A_Step_That_No_Longer_Checks_The_Opcode_Is_Reported()
    {
        var answer = X4Checks(Replacing(WithWitnessMembers(
            TreeFile(ActivationFile), "JsNativeActivation", "X4-a-step-that-no-longer-checks-the-opcode.cs.witness")));

        Assert.Contains(answer.Violations, static message => message.Contains(
            "(b) Step's condition does not join the opcode check", StringComparison.Ordinal));

        Assert.Single(answer.Violations);
        Assert.Contains("pc check", answer.Decided);
    }

    /// <summary>A step whose comparisons are all present but joined with <c>&amp;&amp;</c> is reported.</summary>
    [Fact]
    public void X4_A_Step_Whose_Checks_Are_Joined_With_And_Is_Reported()
    {
        var answer = X4Checks(Replacing(WithWitnessMembers(
            TreeFile(ActivationFile), "JsNativeActivation", "X4-a-step-whose-checks-are-joined-with-and.cs.witness")));

        foreach (var (name, _) in Comparisons)
        {
            Assert.Contains(answer.Violations, message => message.Contains(
                $"(b) Step's condition does not join the {name}", StringComparison.Ordinal));
        }

        Assert.Equal(4, answer.Violations.Count);
        Assert.Empty(answer.Decided);
    }

    /// <summary>
    /// Each edit of the real <c>Step</c> that leaves a check disabled while its words stay is reported,
    /// by content.
    /// </summary>
    /// <remarks>
    /// The rejecting directions are the real file with one edit made, as for rule X3, so none of them
    /// can go stale. Each is an edit that keeps the compiler and every check row green.
    /// </remarks>
    [Fact]
    public void X4_Edits_That_Leave_A_Check_In_Step_Disabled_Are_Reported()
    {
        var activation = TreeFile(ActivationFile);

        IReadOnlyList<string> Edited(params (string From, string To)[] edits)
        {
            var text = activation.Text;

            foreach (var (from, to) in edits)
            {
                Assert.Contains(from, text, StringComparison.Ordinal);
                text = text.Replace(from, to, StringComparison.Ordinal);
            }

            return X4Checks(Replacing(activation with { Text = text })).Violations;
        }

        static Action<string> Says(string content) => message =>
            Assert.Contains(content, message, StringComparison.Ordinal);

        // An inverted operator.
        Assert.Collection(Edited(("act.Pc != pc", "act.Pc == pc")), Says("does not join the pc check"));

        // A bound that admits the length itself, and one that admits a negative offset.
        Assert.Collection(
            Edited(("(uint)pc >= (uint)act.Code.Length", "(uint)pc > (uint)act.Code.Length")),
            Says("does not join the bounds check"));

        Assert.Collection(
            Edited(("(uint)pc >= (uint)act.Code.Length", "pc >= act.Code.Length")),
            Says("does not join the bounds check"));

        // The cookie compared with itself.
        Assert.Collection(
            Edited(("act.Cookie != frame->Cookie", "act.Cookie != act.Cookie")),
            Says("does not join the cookie check"));

        // The opcode read before the bound is checked, which throws out of Step into emitted code.
        Assert.Collection(
            Edited((
                "(uint)pc >= (uint)act.Code.Length ||\n            act.Code[pc] != (byte)expected)",
                "act.Code[pc] != (byte)expected ||\n            (uint)pc >= (uint)act.Code.Length)")),
            Says("compares the opcode before it checks the bounds"));

        // The whole condition negated: every comparison is still there, and none is a term of the chain.
        Assert.Equal(
            4,
            Edited(
                ("if (act is null ||", "if (!(act is null ||"),
                ("act.Code[pc] != (byte)expected)", "act.Code[pc] != (byte)expected))"))
                .Count(static message => message.Contains("does not join the", StringComparison.Ordinal)));

        // A term that moves the offset before it is compared.
        Assert.Collection(
            Edited(("act.Pc != pc ||", "(pc = act.Pc) < 0 ||\n            act.Pc != pc ||")),
            Says("which assigns, steps or calls"));

        // A refusal that is not a defect.
        Assert.Collection(
            Edited(("return (int)JsBaselineStatus.Defect;", "return (int)JsBaselineStatus.Exit;")),
            Says("does not do exactly `return (int)JsBaselineStatus.Defect;`"));

        // A statement between the check and the step, and one before the check.
        Assert.Collection(
            Edited((
                "        try\n        {\n            _ = act.Engine",
                "        act.Pc = 0;\n\n        try\n        {\n            _ = act.Engine")),
            Says("something runs between Step's check and its first call of ExecuteCore"));

        Assert.Contains(
            Edited(("var act = current;", "var act = current;\n        act ??= current;")),
            static message => message.Contains("Step's second statement is not an if", StringComparison.Ordinal));

        // A lambda, which could run the loop on a path the check does not stand in front of.
        Assert.Collection(
            Edited(("act.Pending = escaped;", "act.Pending = escaped;\n            System.Func<int> again = () => 0;")),
            Says("declares a local function or a lambda"));

        // A second Step beside the real one, which a wrapper could reach instead.
        Assert.Contains(
            X4Checks(Replacing(WithMember(
                activation,
                "internal static int Step(JsBaselineFrame* frame, int pc) => 0;"))).Violations,
            static message => message.Contains("declares 2 methods named Step", StringComparison.Ordinal));
    }

    /// <summary>
    /// A product file outside the scan and the lowering that reads the layout and a template's fixed
    /// bytes is reported; the same file in the lowering is not.
    /// </summary>
    [Fact]
    public void X4_A_Layout_Read_Outside_The_Scan_And_The_Lowering_Is_Reported()
    {
        const string WitnessName = "X4-a-layout-read-outside-the-scan-and-the-lowering.cs.witness";

        var outside = X4Confinement(
            [.. Tree, Witness(
                WitnessName, "src/Broiler.VM.Profile.JavaScript/JsBaselineReEmitter.cs", "Broiler.VM.Profile.JavaScript")])
            .Violations;

        Assert.Contains(outside, static message => message.Contains(
            "(c) src/Broiler.VM.Profile.JavaScript/JsBaselineReEmitter.cs names Layout in Repair", StringComparison.Ordinal));

        Assert.Contains(outside, static message => message.Contains(
            "(c) src/Broiler.VM.Profile.JavaScript/JsBaselineReEmitter.cs names Fixed in Repair", StringComparison.Ordinal));

        Assert.Equal(2, outside.Count);

        // The accepting direction over the same text: in the lowering assembly it is what the clause
        // allows, so the clause is about where the reads are and not about the reads.
        Assert.Empty(X4Confinement(
            [.. Tree, Witness(
                WitnessName, "src/Broiler.VM.Profile.JavaScript.Compiler/JsBaselineReEmitter.cs", LoweringAssembly)])
            .Violations);

        // And a composition is outside the product source the clause sweeps, which is its stated limit.
        Assert.Empty(X4Confinement(
            [.. Tree, Witness(
                WitnessName,
                "src/compositions/Broiler.VM.Composition.JavaScript.ExecutionOnly/JsBaselineReEmitter.cs",
                "Broiler.VM.Composition.JavaScript.ExecutionOnly")])
            .Violations);
    }

    /// <summary>
    /// A file of the format assembly that walks the layout through <c>Lay</c> outside the scan and
    /// <c>Layout</c> is reported, and so is the same file in the lowering; the same text read as the scan
    /// is not.
    /// </summary>
    [Fact]
    public void X4_A_Lay_Call_Outside_The_Scan_And_The_Layout_Is_Reported()
    {
        const string WitnessName = "X4-a-lay-call-outside-the-scan-and-the-layout.cs.witness";

        IReadOnlyList<string> Added(string path, string assembly) =>
            X4Confinement([.. Tree, Witness(WitnessName, path, assembly)]).Violations;

        // Where the walk is visible: the format assembly, beside the layout it makes.
        Assert.Collection(
            Added("src/Broiler.VM.Profile.JavaScript.Format/JsBaselineLayoutCopy.cs", "Broiler.VM.Profile.JavaScript.Format"),
            static message => Assert.Contains(
                "(c) src/Broiler.VM.Profile.JavaScript.Format/JsBaselineLayoutCopy.cs names Lay in CopyTo",
                message,
                StringComparison.Ordinal));

        // The lowering may name Layout and Fixed, and not the walk: the clause allows Lay in fewer places.
        Assert.Collection(
            Added($"src/{LoweringAssembly}/JsBaselineLayoutCopy.cs", LoweringAssembly),
            static message => Assert.Contains(
                $"(c) src/{LoweringAssembly}/JsBaselineLayoutCopy.cs names Lay in CopyTo",
                message,
                StringComparison.Ordinal));

        // The accepting direction over the same text: read as the scan, it is where the clause allows the
        // walk, so the clause is about where the walk is named and not about the walk.
        Assert.Empty(X4Confinement(Replacing(Witness(WitnessName, ScanFile, "Broiler.VM.Profile.JavaScript.Format"))).Violations);
    }

    /// <summary>
    /// A second door to the layout or to the fixed bytes, opened beside the members that declare them,
    /// is reported.
    /// </summary>
    [Fact]
    public void X4_A_Second_Door_To_The_Layout_Or_The_Fixed_Bytes_Is_Reported()
    {
        var blocks = TreeFile(BlocksFile);

        // The layout, called from a member of the type that declares it other than Layout.
        Assert.Collection(
            X4Confinement(Replacing(WithMembers(
                blocks, "JsBaselineBlocks", "public static int Count(JsBaselineUnitPlan plan) => Layout(plan).Length;")))
                .Violations,
            static message => Assert.Contains(
                "JsBaselineBlocks.cs names Layout in Count", message, StringComparison.Ordinal));

        // Lay, the walk Layout is made by, called from a member of the same type other than Layout: the
        // layout's entries handed out under another name, which the clause confines as it does Layout.
        Assert.Collection(
            X4Confinement(Replacing(WithMembers(
                blocks,
                "JsBaselineBlocks",
                "internal static bool Count<TSink>(JsBaselineUnitPlan plan, ref TSink sink) " +
                "where TSink : struct, IJsBaselineLayoutSink => Lay<TSink>(plan, ref sink);")))
                .Violations,
            static message => Assert.Contains(
                "JsBaselineBlocks.cs names Lay in Count", message, StringComparison.Ordinal));

        // A member of the template that hands its fixed bytes out under another name.
        Assert.Collection(
            X4Confinement(Replacing(WithMembers(
                TreeFile(TemplatesFile), "JsNativeTemplate", "public byte[] Encoding => Fixed;")))
                .Violations,
            static message => Assert.Contains(
                "JsNativeTemplates.cs names Fixed in Encoding", message, StringComparison.Ordinal));

        // A static import that lets the layout be named bare, and a property pattern that reads the
        // bytes without a member access.
        var bare = X4Confinement(
            [.. Tree, new NativeMappingRules.SourceUnit(
                "src/Broiler.VM.Profile.JavaScript/JsBaselineCensus.cs",
                "Broiler.VM.Profile.JavaScript",
                "using static Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks;\n" +
                "namespace Broiler.VM.Profile.JavaScript;\n" +
                "internal static class JsBaselineCensus\n{\n" +
                "    internal static int Entries(JsBaselineUnitPlan plan) => Layout(plan).Length;\n" +
                "    internal static bool Empty(JsNativeTemplate template) => template is { Fixed.Length: 0 };\n}\n")])
            .Violations;

        Assert.Contains(bare, static message => message.Contains(
            "JsBaselineCensus.cs names Layout in Entries", StringComparison.Ordinal));

        Assert.Contains(bare, static message => message.Contains(
            "JsBaselineCensus.cs names Fixed in Empty", StringComparison.Ordinal));

        Assert.Equal(2, bare.Count);
    }

    /// <summary>
    /// Exchanged slots, wrappers that expect each other's opcode and a step that answers another opcode
    /// are reported, although routing, the table's self-check, the scan and Step's own check all pass them.
    /// </summary>
    [Fact]
    public void X4_A_Wrapper_That_Expects_Another_Opcode_Is_Reported()
    {
        static string Slot(string opcode, string entry) =>
            $"slots[(int)JsOpcode.{opcode}] = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&{entry};";

        var handlers = TreeFile(HandlerFile);
        var nop = Slot("Nop", "Nop");
        var call = Slot("Call", "Call");

        Assert.Contains(nop, handlers.Text, StringComparison.Ordinal);
        Assert.Contains(call, handlers.Text, StringComparison.Ordinal);

        var exchanged = WithWitnessMembers(
            handlers with
            {
                Text = handlers.Text
                    .Replace(nop, Slot("Nop", "Call"), StringComparison.Ordinal)
                    .Replace(call, Slot("Call", "Nop"), StringComparison.Ordinal),
            },
            "JsBaselineHandlers",
            "X4-a-wrapper-that-expects-another-opcode.cs.witness");

        var violations = X4Names(Replacing(exchanged)).Violations;

        foreach (var expected in new[]
                 {
                     "(d) the slot of Nop holds the address of Call",
                     "(d) the slot of Call holds the address of Nop",
                     "(d) the wrapper Nop passes JsOpcode.Call as the opcode Step<JsStepBlock> expects",
                     "(d) the wrapper Call passes JsOpcode.Nop as the opcode Step<StepCall> expects",
                     "(d) the wrapper Call passes JsOpcode.Nop as the opcode Step<JsStepBlock> expects",
                     "(d) StepCall.Opcode answers JsOpcode.Construct",
                 })
        {
            Assert.Contains(violations, message => message.Contains(expected, StringComparison.Ordinal));
        }

        Assert.Equal(6, violations.Count);

        // ...and the routing clause passes the same table, which is why the names need a clause of
        // their own.
        Assert.Empty(X4Routing(Replacing(exchanged)).Violations);
    }

    /// <summary>A wrapper or a partition clause (a) cannot read is reported rather than passed over.</summary>
    [Fact]
    public void X4_A_Wrapper_Or_A_Partition_It_Cannot_Read_Is_Reported()
    {
        var handlers = TreeFile(HandlerFile);

        IReadOnlyList<string> Routed(params string[] members) =>
            X4Routing(Replacing(WithMembers(handlers, "JsBaselineHandlers", members))).Violations;

        const string Entry = "[System.Runtime.InteropServices.UnmanagedCallersOnly] private static int ";

        // A wrapper that reaches Step through a helper is unreadable, which is the conservative direction.
        Assert.Collection(
            Routed(Entry + "Call(JsBaselineFrame* frame, int pc) => Route<StepCall>(frame, pc, JsOpcode.Call);"),
            static message => Assert.Contains(
                "(a) the wrapper Call is not one expression-bodied call of JsNativeActivation.Step", message, StringComparison.Ordinal));

        // The choice turned round.
        Assert.Collection(
            Routed(Entry + "Call(JsBaselineFrame* frame, int pc) => PerOpcodeSteps ? " +
                "JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Call) : JsNativeActivation.Step<StepCall>(frame, pc, JsOpcode.Call);"),
            static message => Assert.Contains(
                "(a) Call runs alone, and its wrapper routes PerOpcodeSteps ? Step<JsStepBlock> : Step<StepCall>", message, StringComparison.Ordinal));

        // A block opcode given the choice, with a step it should not have.
        Assert.Contains(
            Routed(Entry + "Nop(JsBaselineFrame* frame, int pc) => PerOpcodeSteps ? " +
                "JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Nop) : JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Nop);"),
            static message => message.Contains(
                "(a) Nop does not run alone, and its wrapper routes PerOpcodeSteps ? Step<JsStepBlock> : Step<JsStepBlock>", StringComparison.Ordinal));

        // An entry point named for no opcode.
        Assert.Collection(
            Routed(Entry + "Spare(JsBaselineFrame* frame, int pc) => JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Nop);"),
            static message => Assert.Contains(
                "declares the unmanaged entry point Spare, which is named for no opcode", message, StringComparison.Ordinal));

        // A wrapper removed.
        Assert.Collection(
            X4Routing(Replacing(WithoutMember(handlers, "JsBaselineHandlers", "ImportMeta"))).Violations,
            static message => Assert.Contains(
                "declares no wrapper for 1 of the opcodes JsOpcode declares (ImportMeta)", message, StringComparison.Ordinal));

        // The table made partial, so a wrapper could sit in a file the rule does not parse.
        Assert.Contains(
            X4Routing(Replacing(handlers with
            {
                Text = handlers.Text.Replace(
                    "internal static unsafe class JsBaselineHandlers",
                    "internal static unsafe partial class JsBaselineHandlers",
                    StringComparison.Ordinal),
            })).Violations,
            static message => message.Contains("declares JsBaselineHandlers partial", StringComparison.Ordinal));

        // A partition the clause cannot read: an arm with a guard.
        var blocks = TreeFile(BlocksFile);

        Assert.Contains("JsOpcode.RunStaticElements => true,", blocks.Text, StringComparison.Ordinal);

        Assert.Collection(
            X4Routing(Replacing(blocks with
            {
                Text = blocks.Text.Replace(
                    "JsOpcode.RunStaticElements => true,",
                    "JsOpcode.RunStaticElements when opcode != JsOpcode.Nop => true,",
                    StringComparison.Ordinal),
            })).Violations,
            static message => Assert.Contains(
                "(a) JsBaselineBlocks.RunsAlone is not one switch on its opcode", message, StringComparison.Ordinal));
    }

    /// <summary>
    /// A declaration that a wrapper's or a slot's text binds to in place of what the rule reads is reported,
    /// although every text clauses (a) and (d) compare is unchanged.
    /// </summary>
    [Fact]
    public void X4_A_Name_That_Shadows_What_A_Wrapper_Or_A_Slot_Names_Is_Reported()
    {
        var handlers = TreeFile(HandlerFile);

        // A relay nested in the table under the activation's name: every wrapper still reads as it did.
        var relayed = Replacing(WithWitnessMembers(
            handlers, "JsBaselineHandlers", "X4-a-handler-table-that-shadows-the-activation.cs.witness"));

        Assert.Collection(
            X4Routing(relayed).Violations,
            static message => Assert.Contains(
                $"(a) {HandlerFile} declares a class named JsNativeActivation", message, StringComparison.Ordinal));

        // ...and the names clause reads nothing wrong in it, so the routing clause is its only guard.
        Assert.Empty(X4Names(relayed).Violations);

        const string Check = "        if (!Sound(slots, undefined))\n";
        const string Import = "using Broiler.VM.Profile.JavaScript.Format;\n";

        Assert.Contains(Check, handlers.Text, StringComparison.Ordinal);
        Assert.Contains(Import, handlers.Text, StringComparison.Ordinal);

        // A local function of the constructor, which the slot's `&Call` binds to before the wrapper Call.
        var local = Replacing(handlers with
        {
            Text = handlers.Text.Replace(
                Check,
                "        [System.Runtime.InteropServices.UnmanagedCallersOnly]\n" +
                "        static int Call(JsBaselineFrame* frame, int pc) =>\n" +
                "            JsNativeActivation.Step<JsStepBlock>(frame, pc, JsOpcode.Call);\n\n" + Check,
                StringComparison.Ordinal),
        });

        Assert.Collection(
            X4Routing(local).Violations,
            static message => Assert.Contains(
                $"(a) {HandlerFile} declares the unmanaged entry point Call as a local function", message, StringComparison.Ordinal));

        var named = X4Names(local).Violations;

        Assert.Contains(named, static message => message.Contains(
            $"(d) {HandlerFile} declares a local function named Call beside the wrapper of that name", StringComparison.Ordinal));

        Assert.Contains(named, static message => message.Contains(
            "(d) the static constructor of JsBaselineHandlers has `[System.Runtime.InteropServices.UnmanagedCallersOnly]staticintCall(",
            StringComparison.Ordinal));

        Assert.Equal(2, named.Count);

        // An alias of the block mode at the head of the file, and a type of the table named JsOpcode.
        Assert.Collection(
            X4Routing(Replacing(handlers with
            {
                Text = handlers.Text.Replace(
                    Import, Import + "using JsStepBlock = Broiler.VM.Profile.JavaScript.JsStepRelay;\n", StringComparison.Ordinal),
            })).Violations,
            static message => Assert.Contains(
                $"(a) {HandlerFile} declares a using alias named JsStepBlock", message, StringComparison.Ordinal));

        Assert.Collection(
            X4Names(Replacing(WithMembers(handlers, "JsBaselineHandlers", "private static class JsOpcode { }"))).Violations,
            static message => Assert.Contains(
                $"(d) {HandlerFile} declares a class named JsOpcode", message, StringComparison.Ordinal));

        // A step moved out of the table to a type of its own name elsewhere, which the wrapper's text then
        // binds to, answering another opcode.
        const string Relay = "src/Broiler.VM.Profile.JavaScript/JsStepCall.cs";

        var moved = X4Names(
            [
                .. Replacing(WithoutMember(handlers, "JsBaselineHandlers", "StepCall")),
                new NativeMappingRules.SourceUnit(
                    Relay,
                    "Broiler.VM.Profile.JavaScript",
                    "namespace Broiler.VM.Profile.JavaScript;\n" +
                    "internal readonly struct StepCall : IJsExecutionMode\n{\n" +
                    "    public static JsOpcode Opcode => JsOpcode.Construct;\n}\n"),
            ])
            .Violations;

        Assert.Contains(moved, static message => message.Contains(
            $"(d) the wrapper Call runs Step<StepCall>, which {HandlerFile} does not declare", StringComparison.Ordinal));

        Assert.Contains(moved, static message => message.Contains(
            $"(d) {Relay} declares a struct named StepCall outside JsBaselineHandlers", StringComparison.Ordinal));

        Assert.Equal(2, moved.Count);

        // A type named JsOpcode in the profile's own namespace, which the table's `JsOpcode.X` binds to
        // before the enumeration it imports.
        const string Remap = "src/Broiler.VM.Profile.JavaScript/JsOpcodeRemap.cs";

        Assert.Collection(
            X4Names(
                [
                    .. Tree,
                    new NativeMappingRules.SourceUnit(
                        Remap,
                        "Broiler.VM.Profile.JavaScript",
                        "namespace Broiler.VM.Profile.JavaScript;\ninternal enum JsOpcode : byte\n{\n    Nop = 0x70,\n    Call = 0x00,\n}\n"),
                ])
                .Violations,
            static message => Assert.Contains(
                $"(d) {Remap} declares an enum named JsOpcode", message, StringComparison.Ordinal));
    }

    /// <summary>
    /// A write to a slot or to the published table that is not a slot assignment is reported, although every
    /// slot assignment is as it was and the table's own check passes it.
    /// </summary>
    /// <remarks>
    /// Each is an edit of the real handler table, as for clause (b), so none can go stale.
    /// </remarks>
    [Fact]
    public void X4_A_Write_To_A_Slot_Or_The_Table_Beside_The_Slot_Assignments_Is_Reported()
    {
        var handlers = TreeFile(HandlerFile);

        IReadOnlyList<string> Edited(string from, string to)
        {
            Assert.Contains(from, handlers.Text, StringComparison.Ordinal);
            return X4Names(Replacing(handlers with { Text = handlers.Text.Replace(from, to, StringComparison.Ordinal) })).Violations;
        }

        static Action<string> Says(string content) => message =>
            Assert.Contains(content, message, StringComparison.Ordinal);

        const string Check = "        if (!Sound(slots, undefined))\n";

        // Two slots exchanged by a deconstruction: the entries stay distinct, so the table's check passes it,
        // and Step's opcode check turns every call through either into a defect.
        Assert.Collection(
            Edited(
                Check,
                "        (slots[(int)JsOpcode.Nop], slots[(int)JsOpcode.LoadUndefined]) =\n" +
                "            (slots[(int)JsOpcode.LoadUndefined], slots[(int)JsOpcode.Nop]);\n\n" + Check),
            Says(
                "has `(slots[(int)JsOpcode.Nop],slots[(int)JsOpcode.LoadUndefined])=" +
                "(slots[(int)JsOpcode.LoadUndefined],slots[(int)JsOpcode.Nop]);` among its slot assignments"));

        // A slot moved one byte into its entry point: distinct, non-zero and not the refusing one, so no check
        // at all fails, and a call through it does not fail safe.
        Assert.Collection(
            Edited(Check, "        slots[(int)JsOpcode.Call]++;\n\n" + Check),
            Says("has `slots[(int)JsOpcode.Call]++;` among its slot assignments"));

        // A copy routine over the array.
        Assert.Collection(
            Edited(Check, "        System.Array.Reverse(slots, (int)JsOpcode.Nop, 2);\n\n" + Check),
            Says("has `System.Array.Reverse(slots,(int)JsOpcode.Nop,2);` among its slot assignments"));

        // A write to the table after its check, which nothing reads: the constructor no longer ends as it must.
        var late = Edited(
            "        Table = (nint)table;\n",
            "        table[(int)JsOpcode.Call] = table[(int)JsOpcode.Construct];\n        Table = (nint)table;\n");

        Assert.Contains(late, static message => message.Contains(
            "(d) the static constructor of JsBaselineHandlers does not end `", StringComparison.Ordinal));

        // ...and the check it displaced from the end is read among the slot assignments, which is where the
        // clause now finds it.
        Assert.Contains(late, static message => message.Contains(
            "has `if(!Sound(slots,undefined)){Table=0;return;}` among its slot assignments", StringComparison.Ordinal));

        Assert.Equal(2, late.Count);

        // The check writing the array it is handed, and passing it on.
        const string Seen = "        var seen = new System.Collections.Generic.HashSet<nint>();\n";

        Assert.Collection(
            Edited(Seen, Seen + "        slots[(int)JsOpcode.Call] = slots[(int)JsOpcode.Construct];\n"),
            Says("(d) Sound uses its slots in `slots[(int)JsOpcode.Call]`"));

        Assert.Collection(
            Edited(Seen, Seen + "        System.Array.Reverse(slots);\n"),
            Says("(d) Sound uses its slots in `slots`"));
    }

    /// <summary>
    /// A product use of the layout and of a template's fixed bytes whose names are spelled through unicode
    /// escapes is reported.
    /// </summary>
    [Fact]
    public void X4_A_Layout_Named_Through_An_Escape_Is_Reported()
    {
        const string At = "src/Broiler.VM.Profile.JavaScript/JsBaselineCensus.cs";
        var witness = Witness("X4-a-layout-named-through-an-escape.cs.witness", At, "Broiler.VM.Profile.JavaScript");

        // The witness names neither member as plain text, so a clause that parsed only the files whose text
        // contains the names would never open it.
        Assert.DoesNotMatch(@"\b(?:Layout|Lay|Fixed)\b", witness.Text);

        var violations = X4Confinement([.. Tree, witness]).Violations;

        Assert.Contains(violations, static message => message.Contains(
            $"(c) {At} names Layout in Entries", StringComparison.Ordinal));

        Assert.Contains(violations, static message => message.Contains(
            $"(c) {At} names Fixed in Bytes", StringComparison.Ordinal));

        Assert.Equal(2, violations.Count);
    }

    /// <summary>
    /// The rule reports its own vacuity rather than passing over an input it never found.
    /// </summary>
    [Fact]
    public void X4_Reports_An_Input_In_Which_It_Found_Nothing_To_Quantify_Over()
    {
        var empty = X4([]).ToArray();

        foreach (var path in new[] { HandlerFile, ActivationFile, BlocksFile, OpcodeFile, ScanFile, TemplatesFile })
        {
            Assert.Contains(empty, message => message.Contains(
                $"{path} is not in this input", StringComparison.Ordinal));
        }

        Assert.Contains(empty, static message => message.Contains(
            "no file of the lowering assembly", StringComparison.Ordinal));

        const string Profile = "Broiler.VM.Profile.JavaScript";
        const string Format = "Broiler.VM.Profile.JavaScript.Format";

        var hollow = X4(
            [
                new NativeMappingRules.SourceUnit(
                    HandlerFile, Profile,
                    "namespace Broiler.VM.Profile.JavaScript; internal static unsafe class JsBaselineHandlers { }"),
                new NativeMappingRules.SourceUnit(
                    ActivationFile, Profile,
                    "namespace Broiler.VM.Profile.JavaScript; internal sealed unsafe class JsNativeActivation { }"),
                new NativeMappingRules.SourceUnit(
                    BlocksFile, Format,
                    "namespace Broiler.VM.Profile.JavaScript.Format; public static class JsBaselineBlocks " +
                    "{ public static bool RunsAlone(JsOpcode opcode) => opcode switch { _ => false }; }"),
                new NativeMappingRules.SourceUnit(
                    OpcodeFile, Format,
                    "namespace Broiler.VM.Profile.JavaScript.Format; public enum JsOpcode : byte { Nop = 0x00 }"),
                new NativeMappingRules.SourceUnit(
                    ScanFile, Format,
                    "namespace Broiler.VM.Profile.JavaScript.Format; public static class JsNativeScan { }"),
                new NativeMappingRules.SourceUnit(
                    TemplatesFile, Format,
                    "namespace Broiler.VM.Profile.JavaScript.Format; public sealed class JsNativeTemplate { }"),
                new NativeMappingRules.SourceUnit(
                    "src/Broiler.VM.Profile.JavaScript.Compiler/AssemblyMarker.cs", LoweringAssembly,
                    "namespace Broiler.VM.Profile.JavaScript.Compiler; internal sealed class AssemblyMarker { }"),
            ])
            .ToArray();

        foreach (var expected in new[]
                 {
                     "RunsAlone names no opcode",
                     "declares no unmanaged entry point",
                     "declares no Step",
                     "assigns no element of slots",
                     "declares no JsBaselineBlocks.Layout",
                     "declares no JsBaselineBlocks.Lay",
                     "declares no JsNativeTemplate.Fixed",
                 })
        {
            Assert.Contains(hollow, message => message.Contains(expected, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void X4_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(
            static rule => string.Equals(rule.Id, "X4", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Equal("0001", row.OwningAdr);
        Assert.Contains("JSD-0025", row.Statement, StringComparison.Ordinal);
        Assert.Contains("RunsAlone", row.Statement, StringComparison.Ordinal);
        Assert.Contains(LoweringAssembly, row.Statement, StringComparison.Ordinal);

        // The statement names each comparison clause (b) holds in the words the clause compares, so a
        // reader can see that a rewrite with the same meaning is reported rather than cleared.
        foreach (var (_, text) in Comparisons)
        {
            Assert.Contains(text, row.Statement, StringComparison.Ordinal);
        }

        // Clause (c) confines Lay, and the statement says so with the two places it allows the walk; the
        // row no longer states the limit that it does not.
        Assert.Contains(
            "JsBaselineBlocks.Lay, the walk Layout is made by and the template scan compares a payload through, " +
            "is named only in JsNativeScan.cs and inside Layout",
            row.Statement,
            StringComparison.Ordinal);

        Assert.DoesNotContain("AND NOT LAY", row.NonVacuousWhen, StringComparison.Ordinal);

        // Clause (d) reads the static constructor whole, so the row no longer says that a write beside the
        // slot assignments is the table's self-check to catch; and the names clauses (a) and (d) are read by
        // are held to their declarations, which the statement says.
        Assert.Contains("The static constructor of JsBaselineHandlers is exactly", row.Statement, StringComparison.Ordinal);
        Assert.Contains("declares nothing named JsNativeActivation or JsStepBlock", row.Statement, StringComparison.Ordinal);
        Assert.Contains("every name read by its identifier's value", row.Statement, StringComparison.Ordinal);
        Assert.DoesNotContain("a write through a span, a reference or a copy routine", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("NOT THE TABLE AFTER IT IS PUBLISHED", row.NonVacuousWhen, StringComparison.Ordinal);

        // The row must say what the rule does not decide: it reads source, it holds names to declarations
        // without binding them, it does not pin the fallback constant the design keeps, it does not sweep the
        // compositions, and it does not read the private helpers Lay is written with.
        Assert.Contains("STATED LIMITS", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("DOES NOT BIND THEM", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("helper", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("PerOpcodeSteps", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("composition", row.NonVacuousWhen, StringComparison.Ordinal);
        Assert.Contains("NOT THE HELPERS LAY IS WRITTEN WITH", row.NonVacuousWhen, StringComparison.Ordinal);
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
    internal static IEnumerable<string> X2(Type? frame, string frameType = FrameType)
    {
        if (frame is null)
        {
            yield return
                $"the reader found no type named {frameType}, so this rule judged no frame at all";

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
    [
        .. InspectFrame(static context => context.LoadFromAssemblyPath(BuildOutput(FrameAssembly))).Violations,
        .. InspectFrame(
            static context => context.LoadFromAssemblyPath(BuildOutput(FrameAssembly)), ValueFrameType).Violations,
    ];

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
        var valueHelpers = Named(ValueHelperFile);
        var entering = Named(EnteringFile);
        var activation = Named(ActivationFile);

        foreach (var (path, file) in new[]
                 {
                     (HandlerFile, handler), (ValueHelperFile, valueHelpers), (EnteringFile, entering),
                     (ActivationFile, activation),
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
                !string.Equals(file.RelativePath, ValueHelperFile, StringComparison.Ordinal) &&
                UnmanagedEntry.IsMatch(file.Text))
            {
                yield return
                    $"{file.RelativePath} names UnmanagedCallersOnly, and the two files whose methods " +
                    $"native code may call are {HandlerFile} and {ValueHelperFile}";
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

        if (valueHelpers is not null && !UnmanagedEntry.IsMatch(valueHelpers.Text))
        {
            yield return
                "the value helper file names no UnmanagedCallersOnly, so the place this rule pins for the " +
                "value form is not the place its native code calls";
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
        var valueStepReads = 0;
        var settleReads = 0;

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

            if (string.Equals(member.Name, "StepValue", StringComparison.Ordinal))
            {
                valueStepReads++;
                continue;
            }

            // THE VALUE FORM'S SETTLEMENT IS THE THIRD READER (JSD-0035 stage JSV-2): it charges the debt a
            // debt test carried and runs no instruction, and it makes the value step's checks first.
            if (string.Equals(member.Name, "SettleValue", StringComparison.Ordinal))
            {
                settleReads++;
                continue;
            }

            yield return
                $"{activation.RelativePath} reads the thread slot {name} in {member.Name}, and the three " +
                "readers inside the activation are Step, StepValue and SettleValue, which run nothing unless " +
                "the frame's cookie is the activation's";
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

        if (valueStepReads == 0)
        {
            yield return
                $"StepValue reads no {name}, so the value form's reader this rule allows is not the place " +
                "the slot is read";
        }

        if (settleReads == 0)
        {
            yield return
                $"SettleValue reads no {name}, so the value form's settlement this rule allows is not the " +
                "place the slot is read";
        }
    }

    /// <summary>What one clause of rule X4 reported, and what it decided by name on the way.</summary>
    /// <remarks>
    /// The decisions are what a clean direction asserts before it reads an empty answer, so a clause
    /// that found nothing to decide cannot pass by saying nothing.
    /// </remarks>
    internal sealed record X4Answer(IReadOnlyList<string> Violations, IReadOnlySet<string> Decided);

    /// <summary>The four comparisons clause (b) holds, by name and in the words it compares.</summary>
    internal static readonly (string Name, string Text)[] Comparisons =
    [
        ("cookie check", "act.Cookie != frame->Cookie"),
        ("pc check", "act.Pc != pc"),
        ("bounds check", "(uint)pc >= (uint)act.Code.Length"),
        ("opcode check", "act.Code[pc] != (byte)expected"),
    ];

    /// <summary>X4: every clause, for the group X report.</summary>
    internal static IEnumerable<string> X4(IReadOnlyList<NativeMappingRules.SourceUnit> tree) =>
    [
        .. X4Routing(tree).Violations,
        .. X4Checks(tree).Violations,
        .. X4Confinement(tree).Violations,
        .. X4Names(tree).Violations,
        .. X4Value(tree).Violations,
    ];

    /// <summary>
    /// X4 (a): each entry point named for an opcode routes by <c>JsBaselineBlocks.RunsAlone</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The partition is read from its own source, and the routing is compared with it wrapper by
    /// wrapper.</b> A wrapper whose opcode <c>RunsAlone</c>'s pattern does not name calls
    /// <c>Step&lt;JsStepBlock&gt;</c> alone; one whose opcode it names is exactly
    /// <c>PerOpcodeSteps ? Step&lt;Step{Opcode}&gt; : Step&lt;JsStepBlock&gt;</c>; and the handler file
    /// declares no <c>Step{Opcode}</c> for an opcode that does not run alone.
    /// </para>
    /// <para>
    /// <b>What it cannot read it reports.</b> A pattern that is not an <c>or</c> of opcodes answering
    /// true and ending in <c>_ =&gt; false</c>, a wrapper that is not one call of <c>Step</c> or a choice
    /// between two on <c>PerOpcodeSteps</c> - a wrapper reaching <c>Step</c> through a helper among them -
    /// an opcode with no wrapper, and an entry point named for no opcode are failures rather than clean
    /// results.
    /// </para>
    /// <para>
    /// <b>The names a wrapper is read by are held to the declarations they mean.</b> A wrapper is read as
    /// text, and the same text binds to whatever declaration of its name is nearest: so the handler file may
    /// declare nothing named <c>JsNativeActivation</c> or <c>JsStepBlock</c> - no nested type, member, local
    /// or alias - nor an unmanaged entry point as a local function; and product source may declare no other
    /// type, namespace or alias of either name than the activation class at the top of its file and one
    /// block-mode struct at the top of a namespace of the profile.
    /// </para>
    /// </remarks>
    internal static X4Answer X4Routing(IReadOnlyList<NativeMappingRules.SourceUnit> tree)
    {
        var violations = new List<string>();
        var decided = new HashSet<string>(StringComparer.Ordinal);

        var opcodes = DeclaredOpcodes(TreeFile(tree, OpcodeFile), "a", violations);
        var alone = RunsAloneNames(TreeFile(tree, BlocksFile), violations);
        var handlers = HandlerType(TreeFile(tree, HandlerFile), "a", violations);

        if (opcodes is null || alone is null || handlers is null)
        {
            return new(violations, decided);
        }

        foreach (var name in alone.Where(candidate => !opcodes.Contains(candidate)).Order(StringComparer.Ordinal))
        {
            violations.Add($"(a) JsBaselineBlocks.RunsAlone names {name}, which JsOpcode does not declare");
        }

        var wrappers = Wrappers(handlers);
        var handlerRoot = handlers.SyntaxTree.GetRoot();

        foreach (var stray in handlerRoot.DescendantNodes().OfType<MethodDeclarationSyntax>()
                     .Where(method => IsUnmanagedEntry(method) && method.Parent != handlers))
        {
            violations.Add(
                $"(a) {HandlerFile} declares the unmanaged entry point {stray.Identifier.ValueText} outside " +
                "JsBaselineHandlers' own members, where this rule does not read its route");
        }

        // A NAME THE ROUTE IS READ BY MUST BE THE DECLARATION THE RULE MEANS. A wrapper is read as text, so
        // a type, an alias or a member of the table named JsNativeActivation or JsStepBlock would be what
        // the same text binds to, and an entry point declared as a local function is one a slot's address
        // can name while no wrapper of the table is it.
        foreach (var (declared, declaration) in DeclaredNames(handlerRoot)
                     .Where(static pair => pair.Name is "JsNativeActivation" or BlockMode))
        {
            violations.Add(
                $"(a) {HandlerFile} declares {KindOf(declaration)} named {declared}, so the JsNativeActivation.Step " +
                $"and the {BlockMode} a wrapper names need not be the ones this rule reads");
        }

        foreach (var local in handlerRoot.DescendantNodes().OfType<LocalFunctionStatementSyntax>()
                     .Where(static local => IsUnmanagedEntry(local.AttributeLists)))
        {
            violations.Add(
                $"(a) {HandlerFile} declares the unmanaged entry point {local.Identifier.ValueText} as a local " +
                "function, where a slot's address can name it and this rule does not read its route");
        }

        var profile = TreeFile(tree, HandlerFile)!.Assembly;
        var blockModes = 0;

        foreach (var (file, declared, declaration) in TypeNamesOutsideTheTable(tree)
                     .Where(static found => found.Name is "JsNativeActivation" or BlockMode))
        {
            var topLevel = declaration.Parent is BaseNamespaceDeclarationSyntax or CompilationUnitSyntax;

            if (declared is "JsNativeActivation" && topLevel && declaration is ClassDeclarationSyntax &&
                string.Equals(file.RelativePath, ActivationFile, StringComparison.Ordinal))
            {
                continue;
            }

            if (declared is BlockMode && topLevel && declaration is StructDeclarationSyntax &&
                string.Equals(file.Assembly, profile, StringComparison.Ordinal))
            {
                blockModes++;
                continue;
            }

            violations.Add(
                $"(a) {file.RelativePath} declares {KindOf(declaration)} named {declared}, and the one a wrapper means is " +
                (declared is BlockMode
                    ? $"a struct at the top of a namespace of {profile}"
                    : $"the class at the top of {ActivationFile}"));
        }

        if (blockModes != 1)
        {
            violations.Add(
                $"(a) product source declares {blockModes} structs named {BlockMode} at the top of a namespace of {profile}, " +
                "so the block mode a wrapper names is not one declaration this rule can point at");
        }

        if (wrappers.Length == 0)
        {
            violations.Add("(a) JsBaselineHandlers declares no unmanaged entry point, so this rule found no wrapper to route");
            return new(violations, decided);
        }

        var byName = wrappers
            .GroupBy(static wrapper => wrapper.Identifier.ValueText, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToArray(), StringComparer.Ordinal);

        foreach (var (name, declared) in byName.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (declared.Length > 1)
            {
                violations.Add(
                    $"(a) JsBaselineHandlers declares {declared.Length} unmanaged entry points named {name}, so the " +
                    "one a slot takes need not be the one this rule reads");
            }

            if (string.Equals(name, "Undefined", StringComparison.Ordinal))
            {
                continue;
            }

            if (!opcodes.Contains(name))
            {
                violations.Add(
                    $"(a) JsBaselineHandlers declares the unmanaged entry point {name}, which is named for no opcode " +
                    "JsOpcode declares, so the partition decides its route for nothing");

                continue;
            }

            if (RouteOf(declared[0]) is not { } route)
            {
                violations.Add(
                    $"(a) the wrapper {name} is not one expression-bodied call of JsNativeActivation.Step, nor a " +
                    "choice between two on PerOpcodeSteps, so this rule cannot read where it routes; a wrapper " +
                    "reaching Step through a helper is reported for that reason");

                continue;
            }

            var runsAlone = alone.Contains(name);
            decided.Add(runsAlone ? name + " runs alone" : name + " takes the block step");

            if (runsAlone &&
                !(route.Chosen &&
                  string.Equals(route.Calls[0].Mode, "Step" + name, StringComparison.Ordinal) &&
                  string.Equals(route.Calls[1].Mode, BlockMode, StringComparison.Ordinal)))
            {
                violations.Add(
                    $"(a) {name} runs alone, and its wrapper routes {route.Describe()} where the partition asks " +
                    $"for PerOpcodeSteps ? Step<Step{name}> : Step<{BlockMode}>");
            }

            if (!runsAlone &&
                (route.Chosen || !string.Equals(route.Calls[0].Mode, BlockMode, StringComparison.Ordinal)))
            {
                violations.Add(
                    $"(a) {name} does not run alone, and its wrapper routes {route.Describe()} where the partition " +
                    $"asks for Step<{BlockMode}> alone");
            }
        }

        var unrouted = opcodes.Where(opcode => !byName.ContainsKey(opcode)).ToArray();

        if (unrouted.Length > 0)
        {
            violations.Add(
                $"(a) JsBaselineHandlers declares no wrapper for {unrouted.Length} of the opcodes JsOpcode declares " +
                $"({string.Join(", ", unrouted)}), so the partition routes nothing for them");
        }

        foreach (var step in StepStructs(handlers).Keys.Order(StringComparer.Ordinal))
        {
            var opcode = step["Step".Length..];

            if (opcodes.Contains(opcode) && !alone.Contains(opcode))
            {
                violations.Add(
                    $"(a) {HandlerFile} declares {step}, a step of {opcode}'s own opcode, and {opcode} does not run alone");
            }
        }

        return new(violations, decided);
    }

    /// <summary>
    /// X4 (b): <c>Step</c> refuses with a defect, before it runs anything, unless the frame's cookie, the
    /// offset, its bound and the opcode at it are what the activation expects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The checks are read in the shape that makes them checks, not by the words they contain.</b>
    /// A rule that found the four comparisons by name passes them joined with <c>&amp;&amp;</c>, with an
    /// operator inverted, behind a refusal that is not a defect, or with the offset reassigned before
    /// they read it. So the one <c>Step</c> begins <c>var act = current;</c>, its next statement is an
    /// <c>if</c> with no <c>else</c> that does exactly <c>return (int)JsBaselineStatus.Defect;</c>, the
    /// four comparisons are terms of that condition's top-level <c>||</c> chain, the bound is checked
    /// before the opcode is read, the condition assigns, steps and calls nothing, <c>Step</c> declares
    /// no local function and no lambda, and its first call of <c>ExecuteCore</c>, over its own mode, is
    /// the first thing the statement after the check does.
    /// </para>
    /// <para>
    /// <b>An equivalent rewrite is reported too</b> - operands exchanged, or the chain written as the
    /// negation of a chain joined with <c>&amp;&amp;</c> - which is the conservative direction for a check
    /// nothing else observes.
    /// </para>
    /// </remarks>
    internal static X4Answer X4Checks(IReadOnlyList<NativeMappingRules.SourceUnit> tree)
    {
        var violations = new List<string>();
        var decided = new HashSet<string>(StringComparer.Ordinal);
        var activation = TreeFile(tree, ActivationFile);

        if (activation is null)
        {
            violations.Add($"(b) {ActivationFile} is not in this input, so this rule pins a place it never found");
            return new(violations, decided);
        }

        var steps = ParsedRoot(activation).DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(static type => string.Equals(type.Identifier.ValueText, "JsNativeActivation", StringComparison.Ordinal))
            .SelectMany(static type => type.Members.OfType<MethodDeclarationSyntax>())
            .Where(static method => string.Equals(method.Identifier.ValueText, "Step", StringComparison.Ordinal))
            .ToArray();

        if (steps.Length == 0)
        {
            violations.Add("(b) JsNativeActivation declares no Step, so the checks this rule holds are nowhere");
            return new(violations, decided);
        }

        if (steps.Length > 1)
        {
            violations.Add(
                $"(b) JsNativeActivation declares {steps.Length} methods named Step, so a wrapper need not reach " +
                "the one whose checks this rule reads");
        }

        var step = steps[0];
        var statements = step.Body?.Statements ?? default;

        if (statements.Count == 0 || !string.Equals(Tokens(statements[0]), "varact=current;", StringComparison.Ordinal))
        {
            violations.Add(
                "(b) Step does not begin `var act = current;`, so the activation its checks compare need not be " +
                "the one in the thread slot");
        }

        if (statements.Count < 2 || statements[1] is not IfStatementSyntax check)
        {
            violations.Add(
                "(b) Step's second statement is not an if, so nothing this rule can read refuses before Step " +
                "runs anything");

            return new(violations, decided);
        }

        if (check.Else is not null)
        {
            violations.Add("(b) Step's check has an else, so a failed check does more than refuse");
        }

        var refusal = check.Statement is BlockSyntax block
            ? block.Statements.Count == 1 ? block.Statements[0] : null
            : check.Statement;

        if (refusal is not ReturnStatementSyntax { Expression: { } refused } ||
            !string.Equals(Tokens(refused), "(int)JsBaselineStatus.Defect", StringComparison.Ordinal))
        {
            violations.Add(
                "(b) Step's check does not do exactly `return (int)JsBaselineStatus.Defect;`, so a failed check " +
                "need not refuse with a defect");
        }

        var terms = new List<ExpressionSyntax>();
        TopLevelOr(check.Condition, terms);

        var texts = terms.Select(Tokens).ToList();
        var at = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var (name, text) in Comparisons)
        {
            var index = texts.IndexOf(string.Concat(text.Where(static c => !char.IsWhiteSpace(c))));

            if (index < 0)
            {
                violations.Add(
                    $"(b) Step's condition does not join the {name}, `{text}`, with || at its top level, so a step " +
                    "can run without it");

                continue;
            }

            at[name] = index;
            decided.Add(name);
        }

        if (at.TryGetValue("bounds check", out var bounds) &&
            at.TryGetValue("opcode check", out var opcode) &&
            opcode < bounds)
        {
            violations.Add(
                "(b) Step compares the opcode before it checks the bounds, so an offset past the code throws out " +
                "of Step into emitted code instead of answering a defect");
        }

        foreach (var effect in check.Condition.DescendantNodesAndSelf().Where(IsEffect))
        {
            violations.Add(
                $"(b) Step's condition contains `{effect}`, which assigns, steps or calls, so what its checks " +
                "compare need not be what they read");
        }

        foreach (var function in step.DescendantNodes().Where(static node =>
                     node is LocalFunctionStatementSyntax or AnonymousFunctionExpressionSyntax))
        {
            violations.Add(
                $"(b) Step declares a local function or a lambda, `{function}`, whose body could run the loop " +
                "where the check does not stand before it");
        }

        var core = step.DescendantNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault(static invocation =>
            invocation.Expression switch
            {
                MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText == "ExecuteCore",
                SimpleNameSyntax name => name.Identifier.ValueText == "ExecuteCore",
                _ => false,
            });

        if (core is null)
        {
            violations.Add("(b) Step calls no ExecuteCore, so there is no step for its checks to stand before");
            return new(violations, decided);
        }

        if (statements.Count < 3 || !statements[2].Span.Contains(core.Span) || !RunsFirst(core, statements[2]))
        {
            violations.Add(
                "(b) something runs between Step's check and its first call of ExecuteCore, so the step need not " +
                "start from what the check compared");
        }

        var mode = step.TypeParameterList is { Parameters: [var parameter] } ? parameter.Identifier.ValueText : null;
        var called = core.Expression switch
        {
            MemberAccessExpressionSyntax { Name: GenericNameSyntax generic } => generic,
            GenericNameSyntax generic => generic,
            _ => null,
        };

        if (mode is null || called is not { TypeArgumentList.Arguments: [var argument] } ||
            !string.Equals(Tokens(argument), mode, StringComparison.Ordinal))
        {
            violations.Add(
                "(b) Step's first call of ExecuteCore is not over Step's own mode, so the step that runs need not " +
                "be the one the wrapper chose");
        }

        return new(violations, decided);
    }

    /// <summary>
    /// X4 (c): in the product source, <c>JsBaselineBlocks.Layout</c> and <c>JsNativeTemplate.Fixed</c> are
    /// named only by the scan and by the lowering, and inside the members that declare them; and
    /// <c>JsBaselineBlocks.Lay</c>, the walk <c>Layout</c> is made by, only by the scan and inside
    /// <c>Layout</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The three members are the ones the owner's answers name.</b> <c>Layout</c> answers a unit's whole
    /// layout, and <c>Fixed</c> is the bytes a template fixes; together they are what an emitter needs from
    /// the format assembly. <c>Lay</c> hands out the same entries one at a time, so a member that walks it
    /// is a <c>Layout</c> under another name. Every simple name spelled any of the three, outside comments,
    /// is a use - through a static import, in a property pattern - whatever it resolves to, which is the
    /// conservative direction. Every product file is parsed and the name compared is the identifier's value,
    /// so a name spelled through a unicode escape is read as the name it is.
    /// </para>
    /// <para>
    /// <b>The declaring members are allowed and nothing else beside them</b>: <c>Layout</c>, which walks
    /// <c>Lay</c> and reads its sink's array, and <c>JsNativeTemplate</c>'s constructor and <c>Length</c>.
    /// A new member beside them that hands any of the three out under another name is reported.
    /// </para>
    /// <para>
    /// <b><c>Lay</c> is allowed in fewer places than the other two</b>: the scan, which compares a payload
    /// through it, and <c>Layout</c>, which is made by it, and not the lowering, which reads the layout
    /// through <c>Layout</c>. <c>Lay</c>'s own declaration is not a use of the name, and a call of
    /// <c>Lay</c> inside <c>Lay</c> would be reported.
    /// </para>
    /// </remarks>
    internal static X4Answer X4Confinement(IReadOnlyList<NativeMappingRules.SourceUnit> tree)
    {
        var violations = new List<string>();
        var decided = new HashSet<string>(StringComparer.Ordinal);

        foreach (var path in new[] { ScanFile, BlocksFile, TemplatesFile })
        {
            if (TreeFile(tree, path) is null)
            {
                violations.Add($"(c) {path} is not in this input, so this rule pins a place it never found");
            }
        }

        if (!tree.Any(static file => string.Equals(file.Assembly, LoweringAssembly, StringComparison.Ordinal)))
        {
            violations.Add(
                $"(c) no file of the lowering assembly, {LoweringAssembly}, is in this input, so one of the two " +
                "places this clause allows was never swept");
        }

        foreach (var (path, type, member) in new[]
                 {
                     (BlocksFile, "JsBaselineBlocks", "Layout"),
                     (BlocksFile, "JsBaselineBlocks", "Lay"),
                     (TemplatesFile, "JsNativeTemplate", "Fixed"),
                 })
        {
            if (TreeFile(tree, path) is { } file && !ParsedRoot(file).DescendantNodes().OfType<TypeDeclarationSyntax>()
                    .Where(declaration => string.Equals(declaration.Identifier.ValueText, type, StringComparison.Ordinal))
                    .SelectMany(static declaration => declaration.Members)
                    .Any(declared => string.Equals(MemberKey(declared), member, StringComparison.Ordinal)))
            {
                violations.Add(
                    $"(c) {path} declares no {type}.{member}, so the member this clause confines is not where it looks");
            }
        }

        // EVERY PRODUCT FILE IS PARSED, and none is passed over for not containing the names as text: a name
        // spelled through a unicode escape, or with a formatting character inside it, is the same name to
        // the compiler and to the parsed identifier's value, and a filter over the raw text sees neither.
        foreach (var file in tree.Where(static file =>
                     !file.Assembly.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal)))
        {
            var inScan = string.Equals(file.RelativePath, ScanFile, StringComparison.Ordinal);
            var inLowering = string.Equals(file.Assembly, LoweringAssembly, StringComparison.Ordinal);

            foreach (var name in ParsedRoot(file).DescendantNodes().OfType<SimpleNameSyntax>())
            {
                var text = name.Identifier.ValueText;

                if (text is not ("Layout" or "Lay" or "Fixed"))
                {
                    continue;
                }

                // The walk is allowed in fewer places than the layout and the fixed bytes: not the lowering.
                var walk = text is "Lay";

                if (inScan || (inLowering && !walk))
                {
                    decided.Add((inScan ? "the scan names " : "the lowering names ") + text);
                    continue;
                }

                var member = EnclosingMember(name).Name;
                var owner = name.Ancestors().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault()?.Identifier.ValueText;

                if (DeclaringUses.Contains((file.RelativePath, owner, member, text)))
                {
                    decided.Add($"{owner}.{member} names {text}");
                    continue;
                }

                violations.Add(walk
                    ? $"(c) {file.RelativePath} names Lay in {member}, and JsBaselineBlocks.Lay, the walk the layout " +
                      $"is made by, is named only by the scan, {ScanFile}, and inside JsBaselineBlocks.Layout"
                    : $"(c) {file.RelativePath} names {text} in {member}, and outside the members that declare them " +
                      $"JsBaselineBlocks.Layout and JsNativeTemplate.Fixed are named only by the scan, {ScanFile}, and " +
                      $"by the lowering, {LoweringAssembly}");
            }
        }

        return new(violations, decided);
    }

    /// <summary>
    /// X4 (d): each slot, each wrapper's expected opcode and each step's opcode is the one the wrapper is
    /// named for.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Routing by name is only as good as the names.</b> Clause (a) reads which step a wrapper named
    /// Call runs; nothing there says the slot for Call holds that wrapper, that the wrapper expects Call,
    /// or that its step answers Call. The table's own check sees only distinct entry points, the scan's
    /// slot clause holds the payload and not the table, <c>Step</c> compares the byte with whatever the
    /// wrapper passes, and the dispatch loop takes a per-opcode step's opcode from the step. So each slot
    /// assigned is <c>slots[(int)JsOpcode.X]</c> holding the address of the wrapper <c>X</c>, once per
    /// opcode; each wrapper passes <c>JsOpcode.X</c>; and each <c>StepX</c> a wrapper runs is declared in
    /// the handler file with an <c>Opcode</c> answering <c>JsOpcode.X</c>.
    /// </para>
    /// <para>
    /// <b>The names are held to the declarations they mean.</b> A slot's <c>&amp;X</c> binds to a local or a
    /// local function of the constructor before the wrapper, <c>JsOpcode.X</c> to a type or alias named
    /// <c>JsOpcode</c> nearer than the imported enumeration, and <c>Step&lt;StepX&gt;</c> to a type of that
    /// name wherever the table's own member is missing. So the handler file declares nothing named
    /// <c>JsOpcode</c>, nothing named for an opcode but that opcode's wrapper, and no <c>StepX</c> but as a
    /// member of the table itself; and product source declares no other <c>JsOpcode</c> than the enumeration
    /// and no <c>StepX</c> at all.
    /// </para>
    /// <para>
    /// <b>The static constructor is read whole.</b> A slot assignment is not the only statement that can
    /// write a slot, so the constructor is exactly the refusing entry, the array and its fill, then one slot
    /// assignment per statement, then the check, the copy and the publication; and <c>Sound</c>, the check,
    /// reads the array's length and its elements and does nothing else with it.
    /// </para>
    /// </remarks>
    internal static X4Answer X4Names(IReadOnlyList<NativeMappingRules.SourceUnit> tree)
    {
        var violations = new List<string>();
        var decided = new HashSet<string>(StringComparer.Ordinal);

        var opcodes = DeclaredOpcodes(TreeFile(tree, OpcodeFile), "d", violations);
        var handlers = HandlerType(TreeFile(tree, HandlerFile), "d", violations);

        if (opcodes is null || handlers is null)
        {
            return new(violations, decided);
        }

        // THE STEPS A WRAPPER'S TYPE ARGUMENT MEANS ARE THE TABLE'S OWN MEMBERS, because a nested type is what
        // the name binds to first; one nested deeper, or declared anywhere else, is one the same text could
        // reach instead once the member is gone, so it is reported rather than read.
        var steps = handlers.Members.OfType<BaseTypeDeclarationSyntax>()
            .Where(type => IsStepName(type.Identifier.ValueText, opcodes))
            .GroupBy(static type => type.Identifier.ValueText, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToArray(), StringComparer.Ordinal);

        var handlerRoot = handlers.SyntaxTree.GetRoot();

        // A NAME A SLOT OR A WRAPPER IS READ BY MUST BE THE DECLARATION THE RULE MEANS. The slot's `&X` binds
        // to a local or a local function of the static constructor before the wrapper X, and `JsOpcode.X` to
        // a type or an alias of the table named JsOpcode before the opcode enumeration, so any such
        // declaration is reported: a slot could hold another entry point, or be indexed and expected by
        // another value, with every text this clause compares unchanged.
        foreach (var (declared, declaration) in DeclaredNames(handlerRoot))
        {
            if (declared is "JsOpcode")
            {
                violations.Add(
                    $"(d) {HandlerFile} declares {KindOf(declaration)} named JsOpcode, so the opcode a slot is indexed by " +
                    "and a wrapper expects need not be JsOpcode's");
            }
            else if (opcodes.Contains(declared) &&
                     !(declaration is MethodDeclarationSyntax method && method.Parent == handlers && IsUnmanagedEntry(method)))
            {
                violations.Add(
                    $"(d) {HandlerFile} declares {KindOf(declaration)} named {declared} beside the wrapper of that name, so " +
                    $"the `&{declared}` a slot holds need not be that wrapper");
            }
            else if (IsStepName(declared, opcodes) && declaration.Parent != handlers)
            {
                violations.Add(
                    $"(d) {HandlerFile} declares {KindOf(declaration)} named {declared} other than as a member of " +
                    $"JsBaselineHandlers itself, so the {declared} a wrapper runs need not be the one this rule reads");
            }
        }

        foreach (var (file, declared, declaration) in TypeNamesOutsideTheTable(tree))
        {
            if (declared is "JsOpcode" &&
                !(declaration is EnumDeclarationSyntax { Parent: BaseNamespaceDeclarationSyntax or CompilationUnitSyntax } &&
                  string.Equals(file.RelativePath, OpcodeFile, StringComparison.Ordinal)))
            {
                violations.Add(
                    $"(d) {file.RelativePath} declares {KindOf(declaration)} named JsOpcode, and the one a slot is indexed " +
                    $"by and a wrapper expects is the enumeration at the top of {OpcodeFile}");
            }
            else if (IsStepName(declared, opcodes))
            {
                violations.Add(
                    $"(d) {file.RelativePath} declares {KindOf(declaration)} named {declared} outside JsBaselineHandlers, so " +
                    $"the {declared} a wrapper runs need not be the one this rule reads");
            }
        }

        foreach (var wrapper in Wrappers(handlers))
        {
            var name = wrapper.Identifier.ValueText;

            if (!opcodes.Contains(name))
            {
                continue;
            }

            if (RouteOf(wrapper) is not { } route)
            {
                violations.Add(
                    $"(d) this rule cannot read the opcode the wrapper {name} expects, because it is not one " +
                    "expression-bodied call of JsNativeActivation.Step nor a choice between two on PerOpcodeSteps");

                continue;
            }

            decided.Add("expected " + name);

            foreach (var call in route.Calls)
            {
                if (!string.Equals(call.Expected, "JsOpcode." + name, StringComparison.Ordinal))
                {
                    violations.Add(
                        $"(d) the wrapper {name} passes {call.Expected} as the opcode Step<{call.Mode}> expects, and a " +
                        "wrapper expects the opcode it is named for");
                }

                if (!string.Equals(call.Mode, BlockMode, StringComparison.Ordinal) && !steps.ContainsKey(call.Mode))
                {
                    violations.Add(
                        $"(d) the wrapper {name} runs Step<{call.Mode}>, which {HandlerFile} does not declare, so the " +
                        "opcode that step answers is not read here");
                }
            }
        }

        foreach (var (step, declared) in steps.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            var opcode = step["Step".Length..];

            if (declared.Length > 1)
            {
                violations.Add($"(d) {HandlerFile} declares {declared.Length} types named {step}");
            }

            var answers = OpcodeAnswers(declared[0]);

            if (answers.Count != 1)
            {
                violations.Add(
                    $"(d) {step} does not declare one Opcode that answers one expression, so this rule cannot read " +
                    "the opcode it names");

                continue;
            }

            decided.Add(step + ".Opcode");

            if (!string.Equals(answers[0], "JsOpcode." + opcode, StringComparison.Ordinal))
            {
                violations.Add(
                    $"(d) {step}.Opcode answers {answers[0]}, and a step answers the opcode it is named for");
            }
        }

        var assigned = new Dictionary<string, int>(StringComparer.Ordinal);

        // THE STATIC CONSTRUCTOR IS READ WHOLE, because a slot assignment is not the only statement that can
        // write a slot: an exchange by deconstruction, an increment, a copy routine and a write to the table
        // after its check all leave every assignment this clause reads as it was, and the table's own check
        // passes each of them while the entries stay distinct. So the constructor is exactly the refusing
        // entry, the array and its fill, then one slot assignment per statement, then the check, the copy
        // and the publication - and Sound, the check, only reads the array it is handed.
        var constructors = handlers.Members.OfType<ConstructorDeclarationSyntax>()
            .Where(static constructor => constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
            .ToArray();

        var statements = constructors is [{ Body: { } body }] ? body.Statements : default;

        if (constructors is not [{ Body: not null }])
        {
            violations.Add(
                $"(d) JsBaselineHandlers declares {constructors.Length} static constructors, or one without a block body, " +
                "so this rule cannot read the one place its slots are filled, checked and published");
        }

        var whole = statements.Count >= TableOpening.Length + TableClosing.Length;

        var opens = whole && statements.Take(TableOpening.Length).Select(Tokens)
            .SequenceEqual(TableOpening.Select(Squeezed), StringComparer.Ordinal);

        var closes = whole && statements.Skip(statements.Count - TableClosing.Length).Select(Tokens)
            .SequenceEqual(TableClosing.Select(Squeezed), StringComparer.Ordinal);

        if (constructors is [{ Body: not null }] && !opens)
        {
            violations.Add(
                "(d) the static constructor of JsBaselineHandlers does not begin `" + string.Join(" ", TableOpening) +
                "`, so the slots this rule reads need not be the array the table is filled from");
        }

        if (constructors is [{ Body: not null }] && !closes)
        {
            violations.Add(
                "(d) the static constructor of JsBaselineHandlers does not end `" + string.Join(" ", TableClosing) +
                "`, so what it publishes need not be the slots this rule reads, checked by Sound and copied once");
        }

        var slots = new List<AssignmentExpressionSyntax>();

        foreach (var statement in whole
                     ? statements.Skip(TableOpening.Length).Take(statements.Count - TableOpening.Length - TableClosing.Length)
                     : [])
        {
            if (statement is ExpressionStatementSyntax
                {
                    Expression: AssignmentExpressionSyntax
                    {
                        Left: ElementAccessExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "slots" } },
                    } assignment,
                })
            {
                slots.Add(assignment);
                continue;
            }

            violations.Add(
                $"(d) the static constructor of JsBaselineHandlers has `{Tokens(statement)}` among its slot assignments, " +
                "which is not one slot assigned, so a slot or the table can be written where this rule does not read it");
        }

        if (opens && closes && slots.Count == statements.Count - TableOpening.Length - TableClosing.Length)
        {
            decided.Add("the static constructor");
        }

        var sounds = handlers.Members.OfType<MethodDeclarationSyntax>()
            .Where(static method => string.Equals(method.Identifier.ValueText, "Sound", StringComparison.Ordinal))
            .ToArray();

        if (sounds is not [{ ParameterList.Parameters: [{ Identifier.ValueText: "slots" }, _] } sound])
        {
            violations.Add(
                $"(d) JsBaselineHandlers declares {sounds.Length} methods named Sound, or one whose first of two parameters " +
                "is not slots, so this rule cannot read that the check the table is published after only reads it");
        }
        else
        {
            var reads = true;

            foreach (var use in sound.DescendantNodes().OfType<IdentifierNameSyntax>()
                         .Where(static name => string.Equals(name.Identifier.ValueText, "slots", StringComparison.Ordinal)))
            {
                var read = use.Parent switch
                {
                    MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Length" } length =>
                        length.Expression == use && !IsWrittenThrough(length),
                    ElementAccessExpressionSyntax element => element.Expression == use && !IsWrittenThrough(element),
                    _ => false,
                };

                if (!read)
                {
                    reads = false;

                    violations.Add(
                        $"(d) Sound uses its slots in `{Tokens(use.Parent!)}`, which is not a read of their length or of " +
                        "one of them, so the check the table is published after could change the slots it checks");
                }
            }

            if (reads)
            {
                decided.Add("Sound reads the slots");
            }
        }

        if (slots.Count == 0)
        {
            violations.Add($"(d) {HandlerFile} assigns no element of slots, so this rule found no slot to tie to a wrapper");
        }

        foreach (var assignment in slots)
        {
            var access = (ElementAccessExpressionSyntax)assignment.Left;

            if (!assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) ||
                access.ArgumentList.Arguments is not [var only] ||
                Unparenthesized(only.Expression) is not CastExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax slot,
                } cast ||
                !string.Equals(Tokens(cast.Type), "int", StringComparison.Ordinal) ||
                !string.Equals(Tokens(slot.Expression), "JsOpcode", StringComparison.Ordinal))
            {
                violations.Add(
                    $"(d) {HandlerFile} assigns `{Tokens(assignment)}`, which is not a slot indexed by (int)JsOpcode " +
                    "and assigned once, so this rule cannot tie it to a wrapper");

                continue;
            }

            var opcode = slot.Name.Identifier.ValueText;
            assigned[opcode] = assigned.GetValueOrDefault(opcode) + 1;
            decided.Add("slot " + opcode);

            var target = assignment.Right;

            while (target is CastExpressionSyntax or ParenthesizedExpressionSyntax)
            {
                target = target is CastExpressionSyntax converted
                    ? converted.Expression
                    : ((ParenthesizedExpressionSyntax)target).Expression;
            }

            if (target is not PrefixUnaryExpressionSyntax { Operand: IdentifierNameSyntax entry } address ||
                !address.IsKind(SyntaxKind.AddressOfExpression))
            {
                violations.Add(
                    $"(d) the slot of {opcode} is assigned `{Tokens(assignment.Right)}`, which is not the address of " +
                    "an entry point named in this file");

                continue;
            }

            if (!string.Equals(entry.Identifier.ValueText, opcode, StringComparison.Ordinal))
            {
                violations.Add(
                    $"(d) the slot of {opcode} holds the address of {entry.Identifier.ValueText}, and a slot holds " +
                    "the wrapper named for its own opcode");
            }
        }

        foreach (var (opcode, count) in assigned.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (!opcodes.Contains(opcode))
            {
                violations.Add($"(d) {HandlerFile} assigns a slot for {opcode}, which JsOpcode does not declare");
            }
            else if (count > 1)
            {
                violations.Add($"(d) {HandlerFile} assigns the slot of {opcode} {count} times");
            }
        }

        var unassigned = opcodes.Where(opcode => slots.Count > 0 && !assigned.ContainsKey(opcode)).ToArray();

        if (unassigned.Length > 0)
        {
            violations.Add(
                $"(d) {HandlerFile} assigns no slot for {unassigned.Length} of the opcodes JsOpcode declares " +
                $"({string.Join(", ", unassigned)})");
        }

        return new(violations, decided);
    }

    /// <summary>The members <c>JsOpcode</c> declares, or nothing when they cannot be read.</summary>
    private static IReadOnlySet<string>? DeclaredOpcodes(
        NativeMappingRules.SourceUnit? file, string clause, List<string> violations)
    {
        if (file is null)
        {
            violations.Add($"({clause}) {OpcodeFile} is not in this input, so this rule knows no opcode to decide");
            return null;
        }

        var members = ParsedRoot(file).DescendantNodes().OfType<EnumDeclarationSyntax>()
            .Where(static declaration => string.Equals(declaration.Identifier.ValueText, "JsOpcode", StringComparison.Ordinal))
            .SelectMany(static declaration => declaration.Members)
            .Select(static member => member.Identifier.ValueText)
            .ToHashSet(StringComparer.Ordinal);

        if (members.Count == 0)
        {
            violations.Add($"({clause}) {OpcodeFile} declares no member of JsOpcode, so this rule knows no opcode to decide");
            return null;
        }

        return members;
    }

    /// <summary>The opcodes <c>JsBaselineBlocks.RunsAlone</c>'s pattern names, or nothing when it cannot be read.</summary>
    private static IReadOnlySet<string>? RunsAloneNames(NativeMappingRules.SourceUnit? file, List<string> violations)
    {
        if (file is null)
        {
            violations.Add($"(a) {BlocksFile} is not in this input, so this rule routes by a partition it never found");
            return null;
        }

        var methods = ParsedRoot(file).DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(static type => string.Equals(type.Identifier.ValueText, "JsBaselineBlocks", StringComparison.Ordinal))
            .SelectMany(static type => type.Members.OfType<MethodDeclarationSyntax>())
            .Where(static method => string.Equals(method.Identifier.ValueText, "RunsAlone", StringComparison.Ordinal))
            .ToArray();

        const string Unreadable =
            "(a) JsBaselineBlocks.RunsAlone is not one switch on its opcode whose arms name opcodes joined by `or` " +
            "and answer true, ending in `_ => false`, so this rule cannot read which opcodes run alone";

        if (methods is not [{ ParameterList.Parameters: [var parameter] } method] ||
            Unparenthesized(method.ExpressionBody?.Expression) is not SwitchExpressionSyntax switched ||
            Unparenthesized(switched.GoverningExpression) is not IdentifierNameSyntax governing ||
            !string.Equals(governing.Identifier.ValueText, parameter.Identifier.ValueText, StringComparison.Ordinal))
        {
            violations.Add(Unreadable);
            return null;
        }

        var names = new HashSet<string>(StringComparer.Ordinal);

        for (var index = 0; index < switched.Arms.Count; index++)
        {
            var arm = switched.Arms[index];
            var answer = Tokens(arm.Expression);

            var readable = arm.WhenClause is null && (index == switched.Arms.Count - 1
                ? arm.Pattern is DiscardPatternSyntax && answer == "false"
                : answer == "true" && OpcodesOf(arm.Pattern, names));

            if (!readable)
            {
                violations.Add(Unreadable);
                return null;
            }
        }

        if (names.Count == 0)
        {
            violations.Add("(a) JsBaselineBlocks.RunsAlone names no opcode, so this rule routes nothing by it");
        }

        return names;
    }

    /// <summary>Adds the opcodes an <c>or</c> of constant patterns names, and answers whether that is all it is.</summary>
    private static bool OpcodesOf(PatternSyntax pattern, HashSet<string> names)
    {
        switch (pattern)
        {
            case ParenthesizedPatternSyntax parenthesized:
                return OpcodesOf(parenthesized.Pattern, names);

            case BinaryPatternSyntax binary when binary.IsKind(SyntaxKind.OrPattern):
                return OpcodesOf(binary.Left, names) && OpcodesOf(binary.Right, names);

            case ConstantPatternSyntax { Expression: MemberAccessExpressionSyntax access }
                when string.Equals(Tokens(access.Expression), "JsOpcode", StringComparison.Ordinal):
                names.Add(access.Name.Identifier.ValueText);
                return true;

            default:
                return false;
        }
    }

    /// <summary>The handler table's one declaration, or nothing when there is not exactly one.</summary>
    private static ClassDeclarationSyntax? HandlerType(
        NativeMappingRules.SourceUnit? file, string clause, List<string> violations)
    {
        if (file is null)
        {
            violations.Add($"({clause}) {HandlerFile} is not in this input, so this rule pins a place it never found");
            return null;
        }

        var types = ParsedRoot(file).DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(static type => string.Equals(type.Identifier.ValueText, "JsBaselineHandlers", StringComparison.Ordinal))
            .ToArray();

        if (types.Length != 1)
        {
            violations.Add(
                $"({clause}) {HandlerFile} declares {types.Length} types named JsBaselineHandlers, and this rule reads one table");

            return null;
        }

        if (types[0].Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            violations.Add(
                $"({clause}) {HandlerFile} declares JsBaselineHandlers partial, so a wrapper, a slot or a step could " +
                "sit in a file this rule does not parse");
        }

        return types[0];
    }

    /// <summary>The table's own methods that carry the unmanaged-entry attribute.</summary>
    private static MethodDeclarationSyntax[] Wrappers(ClassDeclarationSyntax handlers) =>
        handlers.Members.OfType<MethodDeclarationSyntax>().Where(IsUnmanagedEntry).ToArray();

    private static bool IsUnmanagedEntry(MethodDeclarationSyntax method) => IsUnmanagedEntry(method.AttributeLists);

    /// <summary>Whether attribute lists carry the unmanaged-entry attribute, its name read by identifier value.</summary>
    private static bool IsUnmanagedEntry(SyntaxList<AttributeListSyntax> lists) =>
        lists.SelectMany(static list => list.Attributes)
            .Any(static attribute => UnmanagedEntry.IsMatch(string.Join(
                ".",
                attribute.Name.DescendantTokens()
                    .Where(static token => token.IsKind(SyntaxKind.IdentifierToken))
                    .Select(static token => token.ValueText))));

    /// <summary>Whether a name is <c>Step</c> followed by the name of a declared opcode.</summary>
    private static bool IsStepName(string name, IReadOnlySet<string> opcodes) =>
        name.Length > "Step".Length &&
        name.StartsWith("Step", StringComparison.Ordinal) &&
        opcodes.Contains(name["Step".Length..]);

    /// <summary>
    /// Every name declared at or below a node, with its declaration: namespaces, types, members, locals,
    /// local functions, parameters, type parameters, pattern and query variables, and using and extern aliases.
    /// </summary>
    /// <remarks>
    /// Each name is its identifier's value, so one spelled through an escape is the name it spells. A label
    /// and a tuple element's name are left out, because neither is what a simple name in an expression or a
    /// type binds to.
    /// </remarks>
    private static IEnumerable<(string Name, SyntaxNode Declaration)> DeclaredNames(SyntaxNode root) =>
        root.DescendantNodesAndSelf().SelectMany(static node => NamesOf(node).Select(name => (name, node)));

    /// <summary>The names one node declares: none, one, or each part of a namespace's dotted name.</summary>
    private static IEnumerable<string> NamesOf(SyntaxNode node)
    {
        if (node is BaseNamespaceDeclarationSyntax space)
        {
            foreach (var part in space.Name.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>())
            {
                yield return part.Identifier.ValueText;
            }

            yield break;
        }

        {
            SyntaxToken? identifier = node switch
            {
                BaseTypeDeclarationSyntax type => type.Identifier,
                DelegateDeclarationSyntax callable => callable.Identifier,
                MethodDeclarationSyntax method => method.Identifier,
                LocalFunctionStatementSyntax local => local.Identifier,
                PropertyDeclarationSyntax property => property.Identifier,
                EventDeclarationSyntax raised => raised.Identifier,
                EnumMemberDeclarationSyntax member => member.Identifier,
                VariableDeclaratorSyntax variable => variable.Identifier,
                ParameterSyntax parameter => parameter.Identifier,
                TypeParameterSyntax typeParameter => typeParameter.Identifier,
                SingleVariableDesignationSyntax designation => designation.Identifier,
                ForEachStatementSyntax loop => loop.Identifier,
                CatchDeclarationSyntax caught => caught.Identifier,
                FromClauseSyntax source => source.Identifier,
                LetClauseSyntax bound => bound.Identifier,
                JoinClauseSyntax joined => joined.Identifier,
                JoinIntoClauseSyntax grouped => grouped.Identifier,
                QueryContinuationSyntax continued => continued.Identifier,
                ExternAliasDirectiveSyntax externAlias => externAlias.Identifier,
                UsingDirectiveSyntax { Alias: { } usingAlias } => usingAlias.Name.Identifier,
                _ => null,
            };

            if (identifier is { ValueText.Length: > 0 } token)
            {
                yield return token.ValueText;
            }
        }
    }

    /// <summary>What kind of declaration a node is, as a message names it.</summary>
    private static string KindOf(SyntaxNode declaration) => declaration switch
    {
        BaseNamespaceDeclarationSyntax => "a namespace",
        UsingDirectiveSyntax => "a using alias",
        ExternAliasDirectiveSyntax => "an extern alias",
        ClassDeclarationSyntax => "a class",
        StructDeclarationSyntax => "a struct",
        RecordDeclarationSyntax => "a record",
        InterfaceDeclarationSyntax => "an interface",
        EnumDeclarationSyntax => "an enum",
        DelegateDeclarationSyntax => "a delegate",
        MethodDeclarationSyntax => "a method",
        LocalFunctionStatementSyntax => "a local function",
        PropertyDeclarationSyntax => "a property",
        EventDeclarationSyntax => "an event",
        EnumMemberDeclarationSyntax => "an enum member",
        VariableDeclaratorSyntax { Parent.Parent: BaseFieldDeclarationSyntax } => "a field",
        VariableDeclaratorSyntax => "a local",
        ParameterSyntax => "a parameter",
        TypeParameterSyntax => "a type parameter",
        _ => "a variable",
    };

    /// <summary>
    /// Every namespace, type and alias product source declares outside the handler file, with its file and
    /// its name - the declarations a name the handler file spells can bind to from elsewhere.
    /// </summary>
    /// <remarks>
    /// Only namespaces, type declarations and compilation units are descended into, because nothing below a
    /// member declares a namespace, a type or an alias.
    /// </remarks>
    private static IEnumerable<(NativeMappingRules.SourceUnit File, string Name, SyntaxNode Declaration)> TypeNamesOutsideTheTable(
        IReadOnlyList<NativeMappingRules.SourceUnit> tree) =>
        tree.Where(static file =>
                !file.Assembly.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal) &&
                !string.Equals(file.RelativePath, HandlerFile, StringComparison.Ordinal))
            .SelectMany(static file => ParsedRoot(file)
                .DescendantNodesAndSelf(static node =>
                    node is CompilationUnitSyntax or BaseNamespaceDeclarationSyntax or TypeDeclarationSyntax)
                .Where(static node => node is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax
                    or BaseNamespaceDeclarationSyntax or UsingDirectiveSyntax { Alias: not null } or ExternAliasDirectiveSyntax)
                .SelectMany(node => NamesOf(node).Select(name => (file, name, node))));

    /// <summary>The statements the handler table's static constructor begins with, in order.</summary>
    private static readonly string[] TableOpening =
    [
        "var undefined = (nint)(delegate* unmanaged<JsBaselineFrame*, int, int>)&Undefined;",
        "var slots = new nint[JsBaselineAbi.HandlerSlots];",
        "System.Array.Fill(slots, undefined);",
    ];

    /// <summary>The statements the handler table's static constructor ends with, after its slot assignments.</summary>
    private static readonly string[] TableClosing =
    [
        "if (!Sound(slots, undefined)) { Table = 0; return; }",
        "var table = (nint*)System.Runtime.InteropServices.NativeMemory.AllocZeroed((nuint)(JsBaselineAbi.HandlerSlots * sizeof(nint)));",
        "for (var index = 0; index < slots.Length; index++) { table[index] = slots[index]; }",
        "Table = (nint)table;",
    ];

    /// <summary>A statement's text with its whitespace removed, as <see cref="Tokens"/> answers it.</summary>
    private static string Squeezed(string text) => string.Concat(text.Where(static c => !char.IsWhiteSpace(c)));

    /// <summary>Every type in the handler file named <c>Step</c> and something, by name.</summary>
    private static Dictionary<string, BaseTypeDeclarationSyntax[]> StepStructs(ClassDeclarationSyntax handlers) =>
        handlers.SyntaxTree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
            .Where(static type => type.Identifier.ValueText.StartsWith("Step", StringComparison.Ordinal) &&
                type.Identifier.ValueText.Length > "Step".Length)
            .GroupBy(static type => type.Identifier.ValueText, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToArray(), StringComparer.Ordinal);

    /// <summary>The expressions a step's <c>Opcode</c> answers, one per way it answers.</summary>
    private static List<string> OpcodeAnswers(BaseTypeDeclarationSyntax step)
    {
        var answers = new List<string>();

        if (step is not TypeDeclarationSyntax type)
        {
            return answers;
        }

        foreach (var property in type.Members.OfType<PropertyDeclarationSyntax>()
                     .Where(static property => string.Equals(property.Identifier.ValueText, "Opcode", StringComparison.Ordinal)))
        {
            if (property.ExpressionBody is { } body)
            {
                answers.Add(Tokens(body.Expression));
                continue;
            }

            foreach (var getter in property.AccessorList?.Accessors
                         .Where(static accessor => accessor.IsKind(SyntaxKind.GetAccessorDeclaration)) ??
                     Enumerable.Empty<AccessorDeclarationSyntax>())
            {
                answers.Add(getter switch
                {
                    { ExpressionBody: { } arrow } => Tokens(arrow.Expression),
                    { Body.Statements: [ReturnStatementSyntax { Expression: { } returned }] } => Tokens(returned),
                    _ => string.Empty,
                });
            }
        }

        return answers;
    }

    /// <summary>A wrapper's route: one call of <c>Step</c>, or a choice between two on <c>PerOpcodeSteps</c>.</summary>
    private sealed record Route(bool Chosen, IReadOnlyList<StepCall> Calls)
    {
        internal string Describe() => Chosen
            ? $"PerOpcodeSteps ? Step<{Calls[0].Mode}> : Step<{Calls[1].Mode}>"
            : $"Step<{Calls[0].Mode}>";
    }

    /// <summary>One call of <c>JsNativeActivation.Step</c>: its mode and the opcode it expects.</summary>
    private sealed record StepCall(string Mode, string Expected);

    private static Route? RouteOf(MethodDeclarationSyntax wrapper) =>
        Unparenthesized(wrapper.ExpressionBody?.Expression) switch
        {
            InvocationExpressionSyntax single when CallOf(single) is { } call => new(false, [call]),

            ConditionalExpressionSyntax choice
                when Unparenthesized(choice.Condition) is IdentifierNameSyntax { Identifier.ValueText: "PerOpcodeSteps" } &&
                    CallOf(Unparenthesized(choice.WhenTrue)) is { } own &&
                    CallOf(Unparenthesized(choice.WhenFalse)) is { } block => new(true, [own, block]),

            _ => null,
        };

    private static StepCall? CallOf(ExpressionSyntax? expression) =>
        expression is InvocationExpressionSyntax
        {
            Expression: MemberAccessExpressionSyntax
            {
                Name: GenericNameSyntax { Identifier.ValueText: "Step", TypeArgumentList.Arguments: [var mode] },
            } access,
            ArgumentList.Arguments: [_, _, var expected],
        } && string.Equals(Tokens(access.Expression), "JsNativeActivation", StringComparison.Ordinal)
            ? new StepCall(Tokens(mode), Tokens(expected.Expression))
            : null;

    /// <summary>The terms of a condition's top-level <c>||</c> chain; a parenthesized chain is one term.</summary>
    private static void TopLevelOr(ExpressionSyntax condition, List<ExpressionSyntax> terms)
    {
        if (condition is BinaryExpressionSyntax binary && binary.IsKind(SyntaxKind.LogicalOrExpression))
        {
            TopLevelOr(binary.Left, terms);
            TopLevelOr(binary.Right, terms);
            return;
        }

        terms.Add(condition);
    }

    /// <summary>Whether a node of a condition assigns, steps, calls or makes a function.</summary>
    private static bool IsEffect(SyntaxNode node) => node switch
    {
        AssignmentExpressionSyntax or InvocationExpressionSyntax or AnonymousFunctionExpressionSyntax
            or ThrowExpressionSyntax => true,
        PrefixUnaryExpressionSyntax prefix => prefix.Kind() is SyntaxKind.PreIncrementExpression
            or SyntaxKind.PreDecrementExpression,
        PostfixUnaryExpressionSyntax postfix => postfix.Kind() is SyntaxKind.PostIncrementExpression
            or SyntaxKind.PostDecrementExpression,
        _ => false,
    };

    /// <summary>
    /// Whether nothing runs before a call inside a statement: every block on the way holds it as its
    /// first statement, and every other node on the way evaluates it first.
    /// </summary>
    private static bool RunsFirst(SyntaxNode call, StatementSyntax statement)
    {
        for (var node = call; node != statement; node = node.Parent!)
        {
            var first = node.Parent switch
            {
                BlockSyntax block => block.Statements.FirstOrDefault() == node,
                TryStatementSyntax attempt => attempt.Block == node,
                ExpressionStatementSyntax or ReturnStatementSyntax or EqualsValueClauseSyntax
                    or LocalDeclarationStatementSyntax => true,
                VariableDeclaratorSyntax => true,
                VariableDeclarationSyntax declaration => declaration.Variables.FirstOrDefault() == node,
                AssignmentExpressionSyntax assignment =>
                    assignment.Right == node && assignment.Left is IdentifierNameSyntax,
                _ => false,
            };

            if (!first)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// The places inside the members that declare them where the confined names may be read: the file,
    /// the type, the member, and the name.
    /// </summary>
    private static readonly HashSet<(string, string?, string, string)> DeclaringUses =
    [
        (BlocksFile, "JsBaselineBlocks", "Layout", "Layout"),
        (BlocksFile, "JsBaselineBlocks", "Layout", "Lay"),
        (TemplatesFile, "JsNativeTemplate", "JsNativeTemplate", "Fixed"),
        (TemplatesFile, "JsNativeTemplate", "Length", "Fixed"),
    ];

    /// <summary>The name a member declaration is known by, for the members X4's inputs are edited through.</summary>
    private static string? MemberKey(MemberDeclarationSyntax member) => member switch
    {
        MethodDeclarationSyntax method => method.Identifier.ValueText,
        BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
        PropertyDeclarationSyntax property => property.Identifier.ValueText,
        _ => null,
    };

    private static ExpressionSyntax? Unparenthesized(ExpressionSyntax? expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesized)
        {
            expression = parenthesized.Expression;
        }

        return expression;
    }

    /// <summary>A node's tokens with every piece of trivia removed, comments and whitespace included.</summary>
    private static string Tokens(SyntaxNode node) =>
        string.Concat(node.DescendantTokens().Select(static token => token.Text));

    /// <summary>A file's parsed root, parsed once per file object.</summary>
    /// <remarks>
    /// Keyed by the object and not by its value, so an edited copy made with <c>with</c> is parsed on its
    /// own; clause (c) and the declaration sweeps read every product file, and each rejecting direction
    /// hands them the same tree with one file replaced.
    /// </remarks>
    private static SyntaxNode ParsedRoot(NativeMappingRules.SourceUnit file) =>
        ParsedRoots.GetValue(file, static unit =>
            ProductTrees.TryGetValue(unit.RelativePath, out var product) && ReferenceEquals(product.Text, unit.Text)
                ? product.Tree.GetRoot()
                : AssuranceSources.Parse(unit.Text, unit.RelativePath).GetRoot());

    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<NativeMappingRules.SourceUnit, SyntaxNode>
        ParsedRoots = new();

    /// <summary>The product files the assurance sweep has already parsed, by path, so the unedited ones are not parsed twice.</summary>
    private static readonly Dictionary<string, AssuranceSourceFile> ProductTrees =
        AssuranceSources.Files.ToDictionary(static file => file.RelativePath, StringComparer.Ordinal);

    private static NativeMappingRules.SourceUnit? TreeFile(IReadOnlyList<NativeMappingRules.SourceUnit> tree, string path) =>
        tree.FirstOrDefault(file => string.Equals(file.RelativePath, path, StringComparison.Ordinal));

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
        Func<MetadataLoadContext, Assembly> load, string frameType = FrameType)
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

        var frame = load(context).GetType(frameType, throwOnError: false);

        return (
            X2(frame, frameType).ToArray(),
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
    private static bool IsWrite(IdentifierNameSyntax identifier) =>
        IsWrittenThrough(identifier.Parent is MemberAccessExpressionSyntax access && access.Name == identifier
            ? access
            : identifier);

    /// <summary>
    /// Whether an expression is assigned to, deconstructed into, stepped, passed by reference, referenced or
    /// has its address taken, walked up as <see cref="IsWrite"/> walks.
    /// </summary>
    private static bool IsWrittenThrough(ExpressionSyntax expression)
    {
        SyntaxNode target = expression;

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

    /// <summary>The swept tree's file at a path.</summary>
    private static NativeMappingRules.SourceUnit TreeFile(string path) =>
        Tree.Single(file => string.Equals(file.RelativePath, path, StringComparison.Ordinal));

    /// <summary>The swept tree with one file put in place of the file at its path.</summary>
    private static IReadOnlyList<NativeMappingRules.SourceUnit> Replacing(NativeMappingRules.SourceUnit edited) =>
        [.. Tree.Select(file => string.Equals(file.RelativePath, edited.RelativePath, StringComparison.Ordinal) ? edited : file)];

    /// <summary>
    /// A real file with a stored witness's members put in place of the members of the same name in one
    /// type, and the witness's other members added to it.
    /// </summary>
    /// <remarks>
    /// Rule X4's witnesses are edits of files a hundred times their size, and a stored copy of the whole
    /// file would go stale at its next edit - the reason X3's rejecting directions edit the real
    /// activation. The witness holds only what the edit makes, and this makes it.
    /// </remarks>
    private static NativeMappingRules.SourceUnit WithWitnessMembers(
        NativeMappingRules.SourceUnit file, string typeName, string witnessFile)
    {
        var path = WitnessPath(witnessFile);

        var members = AssuranceSources.Parse(File.ReadAllText(path), path).GetRoot()
            .DescendantNodes().OfType<TypeDeclarationSyntax>()
            .First(candidate => string.Equals(candidate.Identifier.ValueText, typeName, StringComparison.Ordinal))
            .Members;

        return WithMembers(file, typeName, members);
    }

    /// <summary>A real file with members, written as source, put in place of or beside one type's own.</summary>
    private static NativeMappingRules.SourceUnit WithMembers(
        NativeMappingRules.SourceUnit file, string typeName, params string[] members) =>
        WithMembers(file, typeName, members.Select(static member =>
            SyntaxFactory.ParseMemberDeclaration(member) ??
            throw new InvalidOperationException($"`{member}` does not parse as a member")));

    private static NativeMappingRules.SourceUnit WithMembers(
        NativeMappingRules.SourceUnit file, string typeName, IEnumerable<MemberDeclarationSyntax> members)
    {
        var root = AssuranceSources.Parse(file.Text, file.RelativePath).GetRoot();

        var type = root.DescendantNodes().OfType<TypeDeclarationSyntax>()
            .First(candidate => string.Equals(candidate.Identifier.ValueText, typeName, StringComparison.Ordinal));

        var edited = type;

        foreach (var member in members)
        {
            var key = MemberKey(member);

            var same = key is null ? null : edited.Members.FirstOrDefault(existing =>
                existing.Kind() == member.Kind() && string.Equals(MemberKey(existing), key, StringComparison.Ordinal));

            edited = same is null ? edited.AddMembers(member) : edited.ReplaceNode(same, member);
        }

        return file with { Text = root.ReplaceNode(type, edited).ToFullString() };
    }

    /// <summary>A real file with one type's member of a name removed.</summary>
    private static NativeMappingRules.SourceUnit WithoutMember(
        NativeMappingRules.SourceUnit file, string typeName, string memberName)
    {
        var root = AssuranceSources.Parse(file.Text, file.RelativePath).GetRoot();

        var member = root.DescendantNodes().OfType<TypeDeclarationSyntax>()
            .First(candidate => string.Equals(candidate.Identifier.ValueText, typeName, StringComparison.Ordinal))
            .Members.Single(candidate => string.Equals(MemberKey(candidate), memberName, StringComparison.Ordinal));

        return file with { Text = root.RemoveNode(member, SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString() };
    }

    /// <summary>A value type with no field, for the vacuity direction.</summary>
    private struct EmptyFrame
    {
    }
}
