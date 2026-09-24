// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <content>
/// Rule X4's clause (e): the value form's helper table, which native code calls one helper per
/// instruction through, routes each helper to the one-instruction step of its own opcode, and that step
/// keeps the checks the baseline step keeps (JSD-0035 section 5, stage JSV-1).
/// </content>
public sealed partial class NativeBaselineRuleTests
{
    /// <summary>The value helper table's type.</summary>
    internal const string ValueTable = "JsValueHelpers";

    /// <summary>The prefix of the value table's one-instruction modes, one per opcode.</summary>
    internal const string ArmPrefix = "Arm";

    /// <summary>
    /// The terms clause (e) holds <c>StepValue</c>'s check to: the baseline step's cookie, bounds and opcode
    /// checks, the region and the plan.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A property and not a field, because it reads <see cref="Comparisons"/> from the class's other file
    /// and the order two files' static initialisers run in is not one either file states.
    /// </para>
    /// <para>
    /// The baseline's pc check is not among them (JSD-0035 stage JSV-2): inline templates run instructions
    /// no managed code sees, so the offset a value helper is handed is held instead to the plan - its
    /// operand height there must be one the walk reached, which <see cref="ReachedCheck"/> reads as the
    /// statement after the check, because the plan's lookup is a call and the condition has none.
    /// </para>
    /// </remarks>
    internal static (string Name, string Text)[] ValueComparisons =>
    [
        .. Comparisons.Where(static comparison => !string.Equals(comparison.Name, "pc check", StringComparison.Ordinal)),
        ("region check", "act.Segment is null"),
        ("plan check", "act.Plan is null"),
    ];

    /// <summary>The statement after <c>StepValue</c>'s check: the offset is one the plan's walk reached.</summary>
    internal const string ReachedCheck = "if (act.Plan.HeightAt(pc) < 0) { return (int)JsBaselineStatus.Defect; }";

    /// <summary>The one helper the value table holds that is named for no opcode: the debt settlement.</summary>
    internal const string SettleEntry = "Settle";

    /// <summary>The settlement's body, exactly.</summary>
    internal const string SettleBody = "JsNativeActivation.SettleValue(frame, pc)";

    /// <summary>The settlement's one slot assignment, exactly.</summary>
    internal const string SettleAssignment =
        "slots[JsValueAbi.SettleSlot] = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Settle;";

    /// <summary>
    /// The helpers the value table holds that are named for no opcode, each with its body and its one slot
    /// assignment, exactly: the debt settlement (stage JSV-2) and a direct call's prepare and finish helpers
    /// (stage JSV-3).
    /// </summary>
    internal static readonly (string Name, string Body, string Assignment)[] FixedEntries =
    [
        (SettleEntry, SettleBody, SettleAssignment),
        (
            "Prepare",
            "JsNativeActivation.PrepareCall<ArmCall>(frame, pc, callee)",
            "slots[JsValueAbi.PrepareSlot] = (nint)(delegate* unmanaged<JsValueFrame*, int, JsValueFrame*, int>)&Prepare;"),
        (
            "Finish",
            "JsNativeActivation.FinishCall(frame, pc, callee, status)",
            "slots[JsValueAbi.FinishSlot] = (nint)(delegate* unmanaged<JsValueFrame*, int, JsValueFrame*, int, int>)&Finish;"),
    ];

    [Fact]
    public void X4_Each_Value_Helper_Runs_Its_Own_Arm_Through_A_Checked_Step()
    {
        // Non-vacuous before the clean answer is read: the value helper file is in the sweep, and the
        // clause decided a helper, an arm, a slot and every one of StepValue's comparisons by name.
        Assert.Contains(
            Tree,
            static file => string.Equals(file.RelativePath, ValueHelperFile, StringComparison.Ordinal));

        var answer = X4Value(Tree);

        foreach (var decided in new[]
                 {
                     "helper Call", "helper Nop", "ArmCall.Opcode", "slot Call", "the static constructor",
                     "Sound reads the slots", "decode before the arm", "encode after the arm",
                     "helper " + SettleEntry, "slot " + SettleEntry, "StepValue reached check",
                     "helper Prepare", "slot Prepare", "helper Finish", "slot Finish",
                 })
        {
            Assert.Contains(decided, answer.Decided);
        }

        foreach (var (name, _) in ValueComparisons)
        {
            Assert.Contains("StepValue " + name, answer.Decided);
        }

        Assert.Empty(answer.Violations);
    }

    /// <summary>
    /// A helper that runs another opcode's arm, an arm that answers another opcode, and a StepValue that
    /// no longer checks the region are each reported.
    /// </summary>
    [Fact]
    public void X4_A_Value_Helper_That_Runs_Another_Arm_Is_Reported()
    {
        var helpers = WithWitnessMembers(
            TreeFile(ValueHelperFile), ValueTable, "X4-a-value-helper-that-runs-another-arm.cs.witness");

        var violations = X4Value(Replacing(helpers)).Violations;

        Assert.Contains(violations, static message => message.Contains(
            "(e) the helper Nop is `JsNativeActivation.StepValue<ArmCall>(frame,pc,JsOpcode.Nop)`",
            StringComparison.Ordinal));

        Assert.Contains(violations, static message => message.Contains(
            "(e) ArmConstruct.Opcode answers JsOpcode.Call", StringComparison.Ordinal));

        var activation = TreeFile(ActivationFile);

        var unchecked_ = X4Value(Replacing(activation with
        {
            Text = activation.Text.Replace(
                "act.Code[pc] != (byte)expected ||\n            act.Segment is null ||",
                "act.Code[pc] != (byte)expected ||",
                StringComparison.Ordinal),
        })).Violations;

        Assert.Contains(unchecked_, static message => message.Contains(
            "(e) StepValue's condition does not join the region check", StringComparison.Ordinal));

        var unreached = X4Value(Replacing(activation with
        {
            Text = activation.Text.Replace(
                "if (act.Plan.HeightAt(pc) < 0)\n        {\n            return (int)JsBaselineStatus.Defect;\n        }\n\n        try\n        {\n            var debt = frame->Debt;\n            frame->Debt = 0;\n            act.Engine.ChargeDebt(debt);\n\n            var pops",
                "try\n        {\n            var debt = frame->Debt;\n            frame->Debt = 0;\n            act.Engine.ChargeDebt(debt);\n\n            var pops",
                StringComparison.Ordinal),
        })).Violations;

        Assert.Contains(unreached, static message => message.Contains(
            "(e) StepValue's third statement is not", StringComparison.Ordinal));

        var helperText = helpers.Text;
        var unsettled = X4Value(Replacing(helpers with
        {
            Text = helperText.Replace(
                "JsNativeActivation.SettleValue(frame, pc);", "JsNativeActivation.StepValue<ArmNop>(frame, pc, JsOpcode.Nop);",
                StringComparison.Ordinal),
        })).Violations;

        Assert.Contains(unsettled, static message => message.Contains(
            "(e) the value table's Settle is", StringComparison.Ordinal));

        // A PREPARE HELPER THAT RUNS ANOTHER ARM, and a finish slot assigned twice, are each reported (stage
        // JSV-3).
        var misprepared = X4Value(Replacing(helpers with
        {
            Text = helperText.Replace(
                "JsNativeActivation.PrepareCall<ArmCall>(frame, pc, callee);",
                "JsNativeActivation.PrepareCall<ArmConstruct>(frame, pc, callee);",
                StringComparison.Ordinal),
        })).Violations;

        Assert.Contains(misprepared, static message => message.Contains(
            "(e) the value table's Prepare is", StringComparison.Ordinal));

        var finishSlot =
            "slots[JsValueAbi.FinishSlot] = (nint)(delegate* unmanaged<JsValueFrame*, int, JsValueFrame*, int, int>)&Finish;";

        var twice = X4Value(Replacing(helpers with
        {
            Text = helperText.Replace(finishSlot, finishSlot + "\n        " + finishSlot, StringComparison.Ordinal),
        })).Violations;

        Assert.Contains(twice, static message => message.Contains(
            "(e) the value table makes 2 assignments", StringComparison.Ordinal));
    }

    /// <summary>
    /// X4 (e): the value helper table and the step it calls, read from source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Every opcode's helper runs its own arm.</b> The table declares one unmanaged entry point named
    /// <c>Undefined</c>, one named <c>Settle</c> that is exactly <c>JsNativeActivation.SettleValue(frame, pc)</c>
    /// and is assigned once to <c>JsValueAbi.SettleSlot</c> (stage JSV-2's debt settlement), a
    /// <c>Prepare</c> and a <c>Finish</c> exactly as <see cref="FixedEntries"/> states them and each assigned
    /// once to its own fixed slot (stage JSV-3's direct call), and one per opcode <c>JsOpcode</c> declares,
    /// each exactly
    /// <c>JsNativeActivation.StepValue&lt;Arm{Opcode}&gt;(frame, pc, JsOpcode.{Opcode})</c>, and each
    /// <c>Arm{Opcode}</c> is a member of the table whose <c>Opcode</c> answers <c>JsOpcode.{Opcode}</c>; no
    /// other product source declares a type named <c>Arm{Opcode}</c>, and the table declares nothing named
    /// <c>JsNativeActivation</c> or <c>JsOpcode</c>.
    /// </para>
    /// <para>
    /// <b>The table is filled, checked and published as the baseline table is</b>: its static constructor
    /// is the refusing entry, the array and its fill, one slot assignment per statement holding the address
    /// of the helper named for its opcode, and the check, the copy and the publication; and <c>Sound</c>
    /// only reads the slots.
    /// </para>
    /// <para>
    /// <b>StepValue keeps Step's checks, the pc check held to the plan.</b> It begins
    /// <c>var act = current;</c>, its next statement is an if with no else doing exactly
    /// <c>return (int)JsBaselineStatus.Defect;</c>, whose top-level <c>||</c> chain holds the cookie, bounds
    /// and opcode comparisons Step's does, the bound before the opcode, the region check and the plan check;
    /// the condition has no effect; the statement after it refuses an offset the plan's walk never reached;
    /// it declares no local function or lambda; its call of
    /// ExecuteCore is over its own mode; and the input words are decoded before that call and the outputs
    /// encoded after it.
    /// </para>
    /// </remarks>
    internal static X4Answer X4Value(IReadOnlyList<NativeMappingRules.SourceUnit> tree)
    {
        var violations = new List<string>();
        var decided = new HashSet<string>(StringComparer.Ordinal);

        var opcodes = DeclaredOpcodes(TreeFile(tree, OpcodeFile), "e", violations);
        var file = TreeFile(tree, ValueHelperFile);

        if (file is null)
        {
            violations.Add($"(e) {ValueHelperFile} is not in this input, so this rule pins a place it never found");
        }

        ValueStep(tree, violations, decided);

        if (opcodes is null || file is null)
        {
            return new(violations, decided);
        }

        var root = ParsedRoot(file);

        var tables = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(static type => string.Equals(type.Identifier.ValueText, ValueTable, StringComparison.Ordinal))
            .ToArray();

        if (tables is not [var table])
        {
            violations.Add($"(e) {ValueHelperFile} declares {tables.Length} types named {ValueTable}, and this rule reads one table");
            return new(violations, decided);
        }

        if (table.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            violations.Add($"(e) {ValueTable} is partial, so a helper, a slot or an arm could sit in a file this rule does not parse");
        }

        foreach (var (declared, declaration) in DeclaredNames(root)
                     .Where(static pair => pair.Name is "JsNativeActivation" or "JsOpcode"))
        {
            violations.Add(
                $"(e) {ValueHelperFile} declares {KindOf(declaration)} named {declared}, so the step a helper calls or " +
                "the opcode it passes need not be the one this rule reads");
        }

        foreach (var stray in root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                     .Where(method => IsUnmanagedEntry(method) && method.Parent != table))
        {
            violations.Add($"(e) {ValueHelperFile} declares the unmanaged entry point {stray.Identifier.ValueText} outside {ValueTable}");
        }

        foreach (var local in root.DescendantNodes().OfType<LocalFunctionStatementSyntax>()
                     .Where(static local => IsUnmanagedEntry(local.AttributeLists)))
        {
            violations.Add($"(e) {ValueHelperFile} declares the unmanaged entry point {local.Identifier.ValueText} as a local function");
        }

        // ---- the helpers --------------------------------------------------------------------------------
        var helpers = Wrappers(table)
            .GroupBy(static helper => helper.Identifier.ValueText, StringComparer.Ordinal)
            .ToDictionary(static group => group.Key, static group => group.ToArray(), StringComparer.Ordinal);

        foreach (var (name, declared) in helpers.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (declared.Length > 1)
            {
                violations.Add($"(e) {ValueTable} declares {declared.Length} unmanaged entry points named {name}");
            }

            var body = declared[0].ExpressionBody?.Expression is { } expression ? Tokens(expression) : null;

            if (string.Equals(name, "Undefined", StringComparison.Ordinal))
            {
                if (!string.Equals(body, "(int)JsBaselineStatus.Defect", StringComparison.Ordinal))
                {
                    violations.Add("(e) the value table's Undefined does not answer exactly (int)JsBaselineStatus.Defect");
                }

                continue;
            }

            if (FixedEntries.FirstOrDefault(entry => string.Equals(entry.Name, name, StringComparison.Ordinal)) is { Name: not null } fixedEntry &&
                !opcodes.Contains(name))
            {
                if (!string.Equals(body, Squeezed(fixedEntry.Body), StringComparison.Ordinal))
                {
                    violations.Add($"(e) the value table's {name} is `{body ?? "not expression-bodied"}`, and it is exactly `{fixedEntry.Body}`");
                    continue;
                }

                decided.Add("helper " + name);
                continue;
            }

            if (!opcodes.Contains(name))
            {
                violations.Add($"(e) {ValueTable} declares the unmanaged entry point {name}, which is named for no opcode");
                continue;
            }

            var expected = $"JsNativeActivation.StepValue<{ArmPrefix}{name}>(frame,pc,JsOpcode.{name})";

            if (!string.Equals(body, expected, StringComparison.Ordinal))
            {
                violations.Add(
                    $"(e) the helper {name} is `{body ?? "not expression-bodied"}`, and a helper is exactly `{expected}`");

                continue;
            }

            decided.Add("helper " + name);
        }

        var missing = opcodes.Where(opcode => !helpers.ContainsKey(opcode)).Order(StringComparer.Ordinal).ToArray();

        if (missing.Length > 0)
        {
            violations.Add($"(e) {ValueTable} declares no helper for {string.Join(", ", missing)}");
        }

        // ---- the arms -----------------------------------------------------------------------------------
        foreach (var arm in table.Members.OfType<BaseTypeDeclarationSyntax>()
                     .Where(type => IsArmName(type.Identifier.ValueText, opcodes)))
        {
            var name = arm.Identifier.ValueText;
            var opcode = name[ArmPrefix.Length..];
            var answers = OpcodeAnswers(arm);

            if (answers is not [var answer])
            {
                violations.Add($"(e) {name} does not declare one Opcode that answers one expression");
                continue;
            }

            if (!string.Equals(answer, "JsOpcode." + opcode, StringComparison.Ordinal))
            {
                violations.Add($"(e) {name}.Opcode answers {answer}, and an arm answers the opcode it is named for");
                continue;
            }

            decided.Add(name + ".Opcode");
        }

        foreach (var opcode in opcodes.Where(opcode => helpers.ContainsKey(opcode) &&
                     !table.Members.OfType<BaseTypeDeclarationSyntax>()
                         .Any(type => string.Equals(type.Identifier.ValueText, ArmPrefix + opcode, StringComparison.Ordinal))))
        {
            violations.Add($"(e) {ValueTable} declares no {ArmPrefix}{opcode}, so the arm its helper runs is not read here");
        }

        foreach (var (other, declared, declaration) in TypeNamesOutside(tree, ValueHelperFile)
                     .Where(found => IsArmName(found.Name, opcodes)))
        {
            violations.Add(
                $"(e) {other.RelativePath} declares {KindOf(declaration)} named {declared} outside {ValueTable}, so the " +
                "arm a helper runs need not be the one this rule reads");
        }

        // ---- the table ----------------------------------------------------------------------------------
        ValueTableShape(table, opcodes, violations, decided);
        return new(violations, decided);
    }

    /// <summary>Clause (e)'s reading of StepValue, in the activation's own file.</summary>
    private static void ValueStep(
        IReadOnlyList<NativeMappingRules.SourceUnit> tree, List<string> violations, HashSet<string> decided)
    {
        var activation = TreeFile(tree, ActivationFile);

        if (activation is null)
        {
            violations.Add($"(e) {ActivationFile} is not in this input, so StepValue is nowhere this rule looks");
            return;
        }

        var steps = ParsedRoot(activation).DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(static type => string.Equals(type.Identifier.ValueText, "JsNativeActivation", StringComparison.Ordinal))
            .SelectMany(static type => type.Members.OfType<MethodDeclarationSyntax>())
            .Where(static method => string.Equals(method.Identifier.ValueText, "StepValue", StringComparison.Ordinal))
            .ToArray();

        if (steps is not [var step])
        {
            violations.Add($"(e) JsNativeActivation declares {steps.Length} methods named StepValue, and this rule reads one");
            return;
        }

        var statements = step.Body?.Statements ?? default;

        if (statements.Count == 0 || !string.Equals(Tokens(statements[0]), "varact=current;", StringComparison.Ordinal))
        {
            violations.Add("(e) StepValue does not begin `var act = current;`");
        }

        if (statements.Count < 3 || statements[1] is not IfStatementSyntax { Else: null } check)
        {
            violations.Add("(e) StepValue's second statement is not an if with no else, so nothing refuses before it runs");
            return;
        }

        var refusal = check.Statement is BlockSyntax { Statements: [var only] } ? only : check.Statement;

        if (refusal is not ReturnStatementSyntax { Expression: { } refused } ||
            !string.Equals(Tokens(refused), "(int)JsBaselineStatus.Defect", StringComparison.Ordinal))
        {
            violations.Add("(e) StepValue's check does not do exactly `return (int)JsBaselineStatus.Defect;`");
        }

        var terms = new List<ExpressionSyntax>();
        TopLevelOr(check.Condition, terms);
        var texts = terms.Select(Tokens).ToList();
        var at = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var (name, text) in ValueComparisons)
        {
            var index = texts.IndexOf(string.Concat(text.Where(static c => !char.IsWhiteSpace(c))));

            if (index < 0)
            {
                violations.Add($"(e) StepValue's condition does not join the {name}, `{text}`, with || at its top level");
                continue;
            }

            at[name] = index;
            decided.Add("StepValue " + name);
        }

        if (at.TryGetValue("bounds check", out var bounds) && at.TryGetValue("opcode check", out var opcode) && opcode < bounds)
        {
            violations.Add("(e) StepValue compares the opcode before it checks the bounds");
        }

        foreach (var effect in check.Condition.DescendantNodesAndSelf().Where(IsEffect))
        {
            violations.Add($"(e) StepValue's condition contains `{effect}`, which assigns, steps or calls");
        }

        if (!string.Equals(Tokens(statements[2]), Squeezed(ReachedCheck), StringComparison.Ordinal))
        {
            violations.Add($"(e) StepValue's third statement is not `{ReachedCheck}`, so an offset the plan never reached could run");
        }
        else
        {
            decided.Add("StepValue reached check");
        }

        foreach (var function in step.DescendantNodes().Where(static node =>
                     node is LocalFunctionStatementSyntax or AnonymousFunctionExpressionSyntax))
        {
            violations.Add($"(e) StepValue declares a local function or a lambda, `{function}`");
        }

        var calls = step.DescendantNodes().OfType<InvocationExpressionSyntax>().ToArray();
        var core = Array.FindIndex(calls, static call => CalledName(call) == "ExecuteCore");
        var enter = Array.FindIndex(calls, static call => Tokens(call.Expression) == "JsValueWindows.Enter");
        var leave = Array.FindIndex(calls, static call => Tokens(call.Expression) == "JsValueWindows.Leave");

        if (core < 0)
        {
            violations.Add("(e) StepValue calls no ExecuteCore, so there is no arm for its checks to stand before");
            return;
        }

        var mode = step.TypeParameterList is { Parameters: [var parameter] } ? parameter.Identifier.ValueText : null;

        if (mode is null ||
            calls[core].Expression is not MemberAccessExpressionSyntax { Name: GenericNameSyntax { TypeArgumentList.Arguments: [var argument] } } ||
            !string.Equals(Tokens(argument), mode, StringComparison.Ordinal))
        {
            violations.Add("(e) StepValue's call of ExecuteCore is not over StepValue's own mode");
        }

        if (enter < 0 || enter > core)
        {
            violations.Add("(e) StepValue does not decode the instruction's words before the arm runs");
        }
        else
        {
            decided.Add("decode before the arm");
        }

        if (leave < 0 || leave < core)
        {
            violations.Add("(e) StepValue does not encode what the arm left after it runs");
        }
        else
        {
            decided.Add("encode after the arm");
        }
    }

    /// <summary>Clause (e)'s reading of the table's static constructor and its check.</summary>
    private static void ValueTableShape(
        ClassDeclarationSyntax table, IReadOnlySet<string> opcodes, List<string> violations, HashSet<string> decided)
    {
        var constructors = table.Members.OfType<ConstructorDeclarationSyntax>()
            .Where(static constructor => constructor.Modifiers.Any(SyntaxKind.StaticKeyword))
            .ToArray();

        if (constructors is not [{ Body: { } body }])
        {
            violations.Add($"(e) {ValueTable} declares {constructors.Length} static constructors, or one without a block body");
            return;
        }

        var statements = body.Statements;
        var whole = statements.Count >= ValueTableOpening.Length + ValueTableClosing.Length;

        var opens = whole && statements.Take(ValueTableOpening.Length).Select(Tokens)
            .SequenceEqual(ValueTableOpening.Select(Squeezed), StringComparer.Ordinal);

        var closes = whole && statements.Skip(statements.Count - ValueTableClosing.Length).Select(Tokens)
            .SequenceEqual(ValueTableClosing.Select(Squeezed), StringComparer.Ordinal);

        if (!opens)
        {
            violations.Add($"(e) the static constructor of {ValueTable} does not begin `{string.Join(" ", ValueTableOpening)}`");
        }

        if (!closes)
        {
            violations.Add($"(e) the static constructor of {ValueTable} does not end `{string.Join(" ", ValueTableClosing)}`");
        }

        var assigned = new Dictionary<string, int>(StringComparer.Ordinal);
        var fixedAssigned = FixedEntries.ToDictionary(static entry => entry.Name, static _ => 0, StringComparer.Ordinal);

        foreach (var statement in whole
                     ? statements.Skip(ValueTableOpening.Length).Take(statements.Count - ValueTableOpening.Length - ValueTableClosing.Length)
                     : [])
        {
            if (FixedEntries.FirstOrDefault(entry =>
                    string.Equals(Tokens(statement), Squeezed(entry.Assignment), StringComparison.Ordinal)) is { Name: { } fixedName })
            {
                fixedAssigned[fixedName]++;
                continue;
            }

            if (statement is not ExpressionStatementSyntax
                {
                    Expression: AssignmentExpressionSyntax
                    {
                        Left: ElementAccessExpressionSyntax
                        {
                            Expression: IdentifierNameSyntax { Identifier.ValueText: "slots" },
                            ArgumentList.Arguments: [var index],
                        },
                    } assignment,
                } ||
                !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) ||
                Unparenthesized(index.Expression) is not CastExpressionSyntax { Expression: MemberAccessExpressionSyntax slot } cast ||
                !string.Equals(Tokens(cast.Type), "int", StringComparison.Ordinal) ||
                !string.Equals(Tokens(slot.Expression), "JsOpcode", StringComparison.Ordinal))
            {
                violations.Add(
                    $"(e) the static constructor of {ValueTable} has `{Tokens(statement)}` among its slot assignments, " +
                    "which is not one slot indexed by (int)JsOpcode and assigned once");

                continue;
            }

            var opcode = slot.Name.Identifier.ValueText;
            assigned[opcode] = assigned.GetValueOrDefault(opcode) + 1;

            var target = assignment.Right;

            while (target is CastExpressionSyntax or ParenthesizedExpressionSyntax)
            {
                target = target is CastExpressionSyntax converted
                    ? converted.Expression
                    : ((ParenthesizedExpressionSyntax)target).Expression;
            }

            if (target is not PrefixUnaryExpressionSyntax { Operand: IdentifierNameSyntax entry } address ||
                !address.IsKind(SyntaxKind.AddressOfExpression) ||
                !string.Equals(entry.Identifier.ValueText, opcode, StringComparison.Ordinal))
            {
                violations.Add($"(e) the value table's slot of {opcode} does not hold the address of the helper named {opcode}");
                continue;
            }

            decided.Add("slot " + opcode);
        }

        foreach (var (opcode, count) in assigned.Where(static pair => pair.Value > 1))
        {
            violations.Add($"(e) the value table assigns the slot of {opcode} {count} times");
        }

        var unassigned = opcodes.Where(opcode => !assigned.ContainsKey(opcode)).Order(StringComparer.Ordinal).ToArray();

        if (unassigned.Length > 0)
        {
            violations.Add($"(e) the value table assigns no slot for {string.Join(", ", unassigned)}");
        }

        foreach (var (fixedName, _, assignment) in FixedEntries)
        {
            if (fixedAssigned[fixedName] != 1)
            {
                violations.Add($"(e) the value table makes {fixedAssigned[fixedName]} assignments `{assignment}`, and it makes one");
            }
            else
            {
                decided.Add("slot " + fixedName);
            }
        }

        if (opens && closes && assigned.Count > 0)
        {
            decided.Add("the static constructor");
        }

        var sounds = table.Members.OfType<MethodDeclarationSyntax>()
            .Where(static method => string.Equals(method.Identifier.ValueText, "Sound", StringComparison.Ordinal))
            .ToArray();

        if (sounds is not [{ ParameterList.Parameters: [{ Identifier.ValueText: "slots" }, _] } sound])
        {
            violations.Add($"(e) {ValueTable} declares {sounds.Length} methods named Sound, or one whose first of two parameters is not slots");
            return;
        }

        var reads = sound.DescendantNodes().OfType<IdentifierNameSyntax>()
            .Where(static name => string.Equals(name.Identifier.ValueText, "slots", StringComparison.Ordinal))
            .All(static use => use.Parent switch
            {
                MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Length" } length =>
                    length.Expression == use && !IsWrittenThrough(length),
                ElementAccessExpressionSyntax element => element.Expression == use && !IsWrittenThrough(element),
                _ => false,
            });

        if (!reads)
        {
            violations.Add("(e) the value table's Sound uses its slots other than by reading their length or one of them");
        }
        else
        {
            decided.Add("Sound reads the slots");
        }
    }

    /// <summary>Whether a name is the arm prefix followed by a declared opcode.</summary>
    private static bool IsArmName(string name, IReadOnlySet<string> opcodes) =>
        name.Length > ArmPrefix.Length &&
        name.StartsWith(ArmPrefix, StringComparison.Ordinal) &&
        opcodes.Contains(name[ArmPrefix.Length..]);

    /// <summary>The simple name an invocation calls, generic or not.</summary>
    private static string? CalledName(InvocationExpressionSyntax call) => call.Expression switch
    {
        MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
        SimpleNameSyntax name => name.Identifier.ValueText,
        _ => null,
    };

    /// <summary>Every namespace, type and alias product source declares outside one file.</summary>
    private static IEnumerable<(NativeMappingRules.SourceUnit File, string Name, SyntaxNode Declaration)> TypeNamesOutside(
        IReadOnlyList<NativeMappingRules.SourceUnit> tree, string path) =>
        tree.Where(file =>
                !file.Assembly.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal) &&
                !string.Equals(file.RelativePath, path, StringComparison.Ordinal))
            .SelectMany(static file => ParsedRoot(file)
                .DescendantNodesAndSelf(static node =>
                    node is CompilationUnitSyntax or BaseNamespaceDeclarationSyntax or TypeDeclarationSyntax)
                .Where(static node => node is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax
                    or BaseNamespaceDeclarationSyntax or UsingDirectiveSyntax { Alias: not null } or ExternAliasDirectiveSyntax)
                .SelectMany(node => NamesOf(node).Select(name => (file, name, node))));

    /// <summary>The statements the value table's static constructor begins with, in order.</summary>
    private static readonly string[] ValueTableOpening =
    [
        "var undefined = (nint)(delegate* unmanaged<JsValueFrame*, int, int>)&Undefined;",
        "var slots = new nint[JsValueAbi.HelperSlots];",
        "System.Array.Fill(slots, undefined);",
    ];

    /// <summary>The statements the value table's static constructor ends with, after its slot assignments.</summary>
    private static readonly string[] ValueTableClosing =
    [
        "if (!Sound(slots, undefined)) { Table = 0; return; }",
        "var table = (nint*)System.Runtime.InteropServices.NativeMemory.AllocZeroed((nuint)(JsValueAbi.HelperSlots * sizeof(nint)));",
        "for (var index = 0; index < slots.Length; index++) { table[index] = slots[index]; }",
        "Table = (nint)table;",
    ];
}
