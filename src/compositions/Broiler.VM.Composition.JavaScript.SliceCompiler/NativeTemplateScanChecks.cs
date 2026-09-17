// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The two directions of the template-closure scan: everything the backends emit is accepted, and
/// what is accepted is what a backend could have emitted.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE FIRST DIRECTION IS THE ONE THAT KEEPS THE BUILD HONEST AND IT IS THE ONE A CHECK CAN
/// RUN.</b> A table that drifted from the encoders would turn the scan into theatre: a payload
/// every backend of this build emits would be refused, and the refusal would arrive in a
/// composition nobody runs these checks in. So the rows below compile real numeric programs with
/// x86-64 under both calling conventions and with arm64, read the emitted bytes back OUT OF THE
/// ARTIFACT, and require the scan to accept every one of them. A template deleted from an encoder,
/// a register a backend started using, an immediate that grew - each of them fails a row here
/// rather than an artifact in the field.
/// </para>
/// <para>
/// <b>THE SECOND DIRECTION IS A PROPERTY OF THE TABLE AND NOT OF A RUN, AND THE ARGUMENT IS STATED
/// WHERE THE TABLE IS.</b> The scanner accepts a byte only where it belongs to an instantiation of
/// a template the table enumerates with every field inside the closed set that field admits, so the
/// accepted set is exactly the set those templates generate; no run can establish that, because it
/// is a statement about every byte sequence rather than about the ones a check happened to write.
/// What the rows below add is the negative half a reader can check: byte sequences that are legal
/// x86-64 or legal A64, that no backend of this build emits, and that the scan refuses BY NAME.
/// </para>
/// <para>
/// <b>The four-zero-byte row is the one this whole lane exists for.</b> A corpus entry carrying
/// four zero bytes as an x86-64 payload verified, was armed, was jumped into and killed the process
/// with an access violation. Four zero bytes are the first row below.
/// </para>
/// </remarks>
internal static class NativeTemplateScanChecks
{
    /// <summary>The name of the row this whole lane was written around.</summary>
    /// <remarks>
    /// <b>It was a constant because the diagnostic registry's row for the scan's refusal pointed at
    /// it, and as of 2026-09-08 no registry row points at any check of this file.</b> Its summary
    /// read "the name the diagnostic registry's row for the scan's refusal points at", and it was
    /// true for one revision of that registry: code 1625 was classified `check` - a reachability
    /// kind meaning "a named check of a producer composition reaches it and no entry of the
    /// retained corpus does" - and rule N7 required its case to be a string literal of this file.
    /// Five retained corpus entries reaching 1625 then landed, the row was promoted to `corpus` and
    /// the kind was withdrawn, so the constant is kept for the name it spells and NOT for a binding
    /// it no longer carries. The row below still runs, and its refusal is still the one this lane
    /// exists for.
    /// </remarks>
    internal const string FourZeroBytes = "a-native-payload-of-four-zero-bytes";

    /// <summary>Runs both directions.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run()
    {
        var windows = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var systemV = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var arm64 = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        var rows = new System.Collections.Generic.List<(string, bool, string)>
        {
            Closure(JsNativeBackends.X64Windows, JsNativeArchitecture.X64Windows, 16, windows),
            Closure(JsNativeBackends.X64SystemV, JsNativeArchitecture.X64SystemV, 16, systemV),
            Closure(JsNativeBackends.Arm64, JsNativeArchitecture.Arm64, 4, arm64),
            // ONE TEMPLATE IS OUT OF REACH OF EVERY PROGRAM THE MANIFEST ADMITS, and it is named
            // here rather than quietly tolerated. `movq xmm0, rax` is emitted for one bytecode
            // instruction - a return of `undefined` - and no admitted source reaches it: the
            // numeric manifest refuses a function body that can fall off its end, and a program
            // body returns its completion slot with an ordinary return whatever that slot holds.
            // The template stays in the table because the backend has one and would emit it for an
            // artifact carrying that instruction, which a producer other than this front end can
            // write; what it does not have is a program in this lane that reaches it.
            Coverage(
                JsNativeBackends.X64Windows,
                JsNativeArchitecture.X64Windows,
                windows,
                "movq xmm0, rax"),
            Coverage(
                JsNativeBackends.X64SystemV,
                JsNativeArchitecture.X64SystemV,
                systemV,
                "movq xmm0, rax"),
            Coverage(JsNativeBackends.Arm64, JsNativeArchitecture.Arm64, arm64),
            TheTableAgreesWithTheConventionTable(),
        };

        rows.AddRange(Refusals());
        rows.Add(APayloadDeclaredForTheOtherConvention());
        rows.AddRange(Mutations());
        rows.AddRange(Verification());
        rows.AddRange(Baseline());
        return rows;
    }

    // ---- the baseline block partition's drift guards -------------------------------------------------

    /// <summary>
    /// The table <see cref="JsBaselineBlocks.StopsAfter"/> reads is the predicates it is built from, at
    /// every byte value, observed through the method because the table is private.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A DEFINED BYTE IS JUDGED THREE WAYS.</b> As the instruction just run, with the next instruction
    /// a <c>Nop</c> inside the unit, the step stops exactly when <see cref="JsBaselineBlocks.EndsBlock"/>
    /// holds; with the program counter one short of or one past the instruction's width it always stops,
    /// which is what shows the width bits; and as the next instruction after a <c>Nop</c> it stops exactly
    /// when <see cref="JsBaselineBlocks.RunsAlone"/> holds.
    /// </para>
    /// <para>
    /// <b>AN UNDEFINED BYTE STOPS THE STEP ON EITHER SIDE OF THE BOUNDARY, and that is all of its entry a
    /// caller can observe.</b> Its width is zero in the table, and the ends bit decides before the width
    /// is read, so no program counter can show it and the row does not claim to.
    /// </para>
    /// <para>
    /// <b>The unit's end is tested before the next byte is read</b>, and the last clause shows it: a
    /// one-byte code with the program counter at its end answers rather than reading past the array.
    /// </para>
    /// </remarks>
    private static (string, bool, string) TheBaselinePartitionTableIsThePredicates()
    {
        const string Name = "the baseline partition table is the predicates";
        var failures = new System.Collections.Generic.List<string>();
        var defined = 0;
        var undefined = 0;

        for (var value = 0; value < 256; value++)
        {
            var current = new byte[16];
            current[0] = (byte)value;
            var next = new byte[16];
            next[1] = (byte)value;

            if (!JsOpcodes.IsDefined((byte)value))
            {
                undefined++;

                for (var pc = 0; pc < current.Length; pc++)
                {
                    if (!JsBaselineBlocks.StopsAfter(current, 0, pc, current.Length))
                    {
                        failures.Add("the undefined byte " + value + " did not stop a step after it at pc " + pc);
                        break;
                    }
                }

                if (!JsBaselineBlocks.StopsAfter(next, 0, 1, next.Length))
                {
                    failures.Add("the undefined byte " + value + " did not stop a step before it");
                }

                continue;
            }

            defined++;
            var opcode = (JsOpcode)value;
            var width = JsOpcodes.InstructionWidth(opcode);

            if (JsBaselineBlocks.StopsAfter(current, 0, width, current.Length) != JsBaselineBlocks.EndsBlock(opcode))
            {
                failures.Add("`" + opcode + "` stops a step after it where EndsBlock answers " + JsBaselineBlocks.EndsBlock(opcode));
            }

            if (!JsBaselineBlocks.StopsAfter(current, 0, width - 1, current.Length) ||
                !JsBaselineBlocks.StopsAfter(current, 0, width + 1, current.Length))
            {
                failures.Add("`" + opcode + "` does not stop a step at a pc other than its width " + width);
            }

            if (JsBaselineBlocks.StopsAfter(next, 0, 1, next.Length) != JsBaselineBlocks.RunsAlone(opcode))
            {
                failures.Add("`" + opcode + "` stops a step before it where RunsAlone answers " + JsBaselineBlocks.RunsAlone(opcode));
            }
        }

        if (defined != JsOpcodes.All.Length)
        {
            failures.Add(defined + " bytes are defined and the opcode table lists " + JsOpcodes.All.Length);
        }

        try
        {
            if (!JsBaselineBlocks.StopsAfter([(byte)JsOpcode.Nop], 0, 1, 1))
            {
                failures.Add("a step at its unit's end did not stop");
            }
        }
        catch (System.IndexOutOfRangeException)
        {
            failures.Add("a step at its unit's end read the byte past it");
        }

        return (
            Name,
            failures.Count == 0,
            failures.Count == 0
                ? defined + " defined bytes agree with the width, EndsBlock and RunsAlone on both sides of a boundary; " +
                  undefined + " undefined bytes stop a step on both sides; the unit's end is tested before the byte past it"
                : string.Join("; ", failures));
    }

    /// <summary>
    /// The class of every opcode in the baseline partition, as the audit of each opcode's arm read it,
    /// by arm and for any operand the verifier admits.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>T</b> is a transfer: terminal, or carrying a code target. <b>C</b> is the call class. <b>E</b> is an
    /// arm, or a helper it calls unconditionally, that reaches <c>JsEngine.Call</c> behind no test but the
    /// iterator record's done flag, a nullish <c>return</c> method, a missing <c>Symbol.iterator</c> (which
    /// throws) or a class with no static elements. <b>R</b> reaches guest or embedder code only behind the
    /// object test of <c>ToPrimitive</c> or <c>Render</c>, the getter or setter test of <c>Lookup</c>,
    /// <c>GetSymbol</c>, <c>SetProperty</c>, <c>SetSymbol</c>, <c>SetWithReceiver</c>, <c>SetSuper</c>,
    /// <c>ReadPrivate</c> or <c>WritePrivate</c>, the dispatch to <c>JsProxy</c> or <c>JsHostObject</c>, or
    /// <c>InstanceOf</c>'s test for a <c>Symbol.hasInstance</c> method - so <c>Add</c> over an object whose
    /// <c>valueOf</c> is a plain method is R, behind the first. <b>P</b> reaches none. An opcode can be in T
    /// and in one of the others.
    /// </para>
    /// <para>
    /// <b>The row asserts T, C and E against the predicates and holds R and P to nothing but being
    /// named.</b> Whether an arm is R or P decides no block; it is here so that a new opcode cannot join
    /// the partition without someone reading its arm and writing its class.
    /// </para>
    /// </remarks>
    private static readonly (JsOpcode Opcode, string Class)[] BaselineClasses =
    [
        (JsOpcode.Nop, "P"),
        (JsOpcode.LoadUndefined, "P"),
        (JsOpcode.LoadNull, "P"),
        (JsOpcode.LoadTrue, "P"),
        (JsOpcode.LoadFalse, "P"),
        (JsOpcode.LoadConstant, "P"),
        (JsOpcode.LoadThis, "P"),
        (JsOpcode.NewArguments, "P"),
        (JsOpcode.LoadNewTarget, "P"),
        (JsOpcode.LoadArgument, "P"),
        (JsOpcode.RestArguments, "P"),
        (JsOpcode.LoadScoped, "P"),
        (JsOpcode.StoreScoped, "P"),
        (JsOpcode.InitialiseScoped, "P"),
        (JsOpcode.LoadGlobal, "R"),
        (JsOpcode.StoreGlobal, "R"),
        (JsOpcode.LoadGlobalOrUndefined, "R"),
        (JsOpcode.PushScope, "P"),
        (JsOpcode.PopScope, "P"),
        (JsOpcode.CopyScope, "P"),
        (JsOpcode.DeclareGlobal, "P"),
        (JsOpcode.PushObjectScope, "P"),
        (JsOpcode.ResolveName, "R"),
        (JsOpcode.NewObject, "P"),
        (JsOpcode.NewArray, "P"),
        (JsOpcode.GetProperty, "R"),
        (JsOpcode.SetProperty, "R"),
        (JsOpcode.GetIndex, "R"),
        (JsOpcode.SetIndex, "R"),
        (JsOpcode.DefineField, "R"),
        (JsOpcode.DefineIndexed, "R"),
        (JsOpcode.DeleteProperty, "R"),
        (JsOpcode.DeleteIndex, "R"),
        (JsOpcode.DefineGetter, "R"),
        (JsOpcode.DefineSetter, "R"),
        (JsOpcode.DefineMethod, "R"),
        (JsOpcode.LoadSuperProperty, "R"),
        (JsOpcode.StoreSuperProperty, "R"),
        (JsOpcode.ArrayAppend, "P"),
        (JsOpcode.Closure, "P"),
        (JsOpcode.Call, "C"),
        (JsOpcode.Construct, "C"),
        (JsOpcode.Return, "T P"),
        (JsOpcode.ReturnUndefined, "T P"),
        (JsOpcode.CallEval, "C"),
        (JsOpcode.SuperCall, "C"),
        (JsOpcode.SuperCallForwarded, "C"),
        (JsOpcode.NewClass, "R"),
        (JsOpcode.ArrayHoles, "P"),
        (JsOpcode.SpreadArray, "E"),
        (JsOpcode.SpreadObject, "R"),
        (JsOpcode.CallSpread, "C"),
        (JsOpcode.ConstructSpread, "C"),
        (JsOpcode.SuperCallSpread, "C"),
        (JsOpcode.SetPrototypeLiteral, "R"),
        (JsOpcode.Add, "R"),
        (JsOpcode.Subtract, "R"),
        (JsOpcode.Multiply, "R"),
        (JsOpcode.Divide, "R"),
        (JsOpcode.Remainder, "R"),
        (JsOpcode.Exponent, "R"),
        (JsOpcode.Negate, "R"),
        (JsOpcode.ToNumber, "R"),
        (JsOpcode.Not, "P"),
        (JsOpcode.BitwiseNot, "R"),
        (JsOpcode.LessThan, "R"),
        (JsOpcode.LessThanOrEqual, "R"),
        (JsOpcode.GreaterThan, "R"),
        (JsOpcode.GreaterThanOrEqual, "R"),
        (JsOpcode.StrictEquals, "P"),
        (JsOpcode.StrictNotEquals, "P"),
        (JsOpcode.LooseEquals, "R"),
        (JsOpcode.LooseNotEquals, "R"),
        (JsOpcode.BitwiseOr, "R"),
        (JsOpcode.BitwiseAnd, "R"),
        (JsOpcode.BitwiseXor, "R"),
        (JsOpcode.ShiftLeft, "R"),
        (JsOpcode.ShiftRight, "R"),
        (JsOpcode.ShiftRightUnsigned, "R"),
        (JsOpcode.TypeOf, "P"),
        (JsOpcode.InstanceOf, "R"),
        (JsOpcode.In, "R"),
        (JsOpcode.Void, "P"),
        (JsOpcode.RequireCoercible, "P"),
        (JsOpcode.Jump, "T P"),
        (JsOpcode.JumpIfFalse, "T P"),
        (JsOpcode.JumpIfTrue, "T P"),
        (JsOpcode.Throw, "T R"),
        (JsOpcode.ForInStart, "R"),
        (JsOpcode.ForInNext, "T P"),
        (JsOpcode.IterateStart, "E"),
        (JsOpcode.IterateNext, "T E"),
        (JsOpcode.IterateRest, "E"),
        (JsOpcode.IterateClose, "E"),
        (JsOpcode.Yield, "P"),
        (JsOpcode.YieldDelegate, "E"),
        (JsOpcode.Await, "P"),
        (JsOpcode.LoadImport, "P"),
        (JsOpcode.ThrowImmutable, "P"),
        (JsOpcode.IterateStartAsync, "E"),
        (JsOpcode.IterateNextAsync, "E"),
        (JsOpcode.IterateAwaitStep, "T R"),
        (JsOpcode.IterateCloseAsync, "T E"),
        (JsOpcode.IterateCloseCheck, "P"),
        (JsOpcode.DefineClassElement, "P"),
        (JsOpcode.NewPrivateName, "P"),
        (JsOpcode.LoadPrivate, "R"),
        (JsOpcode.StorePrivate, "R"),
        (JsOpcode.HasPrivate, "P"),
        (JsOpcode.RunStaticElements, "E"),
        (JsOpcode.Pop, "P"),
        (JsOpcode.Duplicate, "P"),
        (JsOpcode.DuplicateTwo, "P"),
        (JsOpcode.Swap, "P"),
        (JsOpcode.Pick, "P"),
        (JsOpcode.DeclareGlobalLet, "P"),
        (JsOpcode.DeclareGlobalConst, "P"),
        (JsOpcode.InitialiseGlobalLexical, "P"),
        (JsOpcode.DeleteGlobalBinding, "P"),
        (JsOpcode.EnterBody, "P"),
        (JsOpcode.ImportCall, "C"),
        (JsOpcode.ImportMeta, "P"),
    ];

    /// <summary>
    /// Every opcode the format defines has a class in the baseline partition, and the partition's
    /// predicates are exactly what those classes say.
    /// </summary>
    /// <remarks>
    /// <b>A NEW OPCODE WITHOUT A CLASS FAILS THIS ROW</b>, and so does a class list that names an opcode
    /// twice or one the format does not define. <see cref="JsBaselineBlocks.RunsAlone"/> must hold exactly
    /// for C and E, and <see cref="JsBaselineBlocks.EndsBlock"/> exactly for T, C and E - so moving an
    /// opcode into or out of the run-alone set is an edit to both the method and this list.
    /// </remarks>
    private static (string, bool, string) EveryOpcodeIsClassifiedForTheBaselinePartition()
    {
        const string Name = "every opcode is classified for the baseline partition";
        var failures = new System.Collections.Generic.List<string>();
        var named = new System.Collections.Generic.HashSet<JsOpcode>();
        var alone = new System.Collections.Generic.List<string>();

        foreach (var (opcode, kind) in BaselineClasses)
        {
            var classes = kind.Split(' ');

            if (!named.Add(opcode))
            {
                failures.Add("`" + opcode + "` is classified twice");
            }

            if (!JsOpcodes.IsDefined((byte)opcode))
            {
                failures.Add("`" + opcode + "` is classified and the format does not define it");
                continue;
            }

            foreach (var single in classes)
            {
                if (single is not ("T" or "C" or "E" or "R" or "P"))
                {
                    failures.Add("`" + opcode + "` has the class `" + single + "`, which the partition does not have");
                }
            }

            var transfer = System.Array.IndexOf(classes, "T") >= 0;
            var runsAlone = System.Array.IndexOf(classes, "C") >= 0 || System.Array.IndexOf(classes, "E") >= 0;

            if (transfer != (JsOpcodes.IsTerminal(opcode) || JsOpcodes.HasCodeTarget(opcode)))
            {
                failures.Add("`" + opcode + "` is classified " + (transfer ? "" : "not ") + "a transfer and the opcode table says otherwise");
            }

            if (runsAlone != JsBaselineBlocks.RunsAlone(opcode))
            {
                failures.Add("`" + opcode + "` is classified " + kind + " and RunsAlone answers " + JsBaselineBlocks.RunsAlone(opcode));
            }

            if ((transfer || runsAlone) != JsBaselineBlocks.EndsBlock(opcode))
            {
                failures.Add("`" + opcode + "` is classified " + kind + " and EndsBlock answers " + JsBaselineBlocks.EndsBlock(opcode));
            }

            if (runsAlone)
            {
                alone.Add(opcode.ToString());
            }
        }

        foreach (var opcode in JsOpcodes.All)
        {
            if (!named.Contains(opcode))
            {
                failures.Add("`" + opcode + "` has no class in the baseline partition");
            }
        }

        return (
            Name,
            failures.Count == 0,
            failures.Count == 0
                ? named.Count + " opcodes classified; " + alone.Count + " run alone (" + string.Join(", ", alone) + ")"
                : string.Join("; ", failures));
    }

    /// <summary>
    /// For every unit of every wide program, <see cref="JsBaselineBlocks.TryPlan"/> answers what a naive walk
    /// over the same predicates gives, and <see cref="JsBaselineBlocks.Layout"/> is exactly
    /// <see cref="JsBaselineBlocks.LayoutLength"/> entries long, resolves every branch to the instruction it
    /// names and dispatches every landing to its own head.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>WHAT IT CATCHES IS BOOKKEEPING, AND NOTHING ELSE.</b> The walk here groups each program's handler
    /// offsets by reading every region for every unit, and recomputes the landings, the heads, and each
    /// head's last instruction, target, successor and tail one head at a time from the same predicates and
    /// the same four landing sources the plan uses. A grouping that gave a unit another unit's offsets, a
    /// plan that lost a head in its third walk, gave a block its neighbour's tail or filled its landings
    /// out of order fails it. A wrong stop rule, or a landing source both of them lack, passes it: that the
    /// landing sources are complete and that the stop rule is the right one are shown only by programs
    /// that run.
    /// </para>
    /// <para>
    /// <b>The layout half is its own books too.</b> Its length is the one <see cref="JsBaselineBlocks.LayoutLength"/>
    /// counted from the plan; every branch resolves to an index; a branch taken on an equal compare lands on
    /// the call of the head whose program counter it compared; a tail's other exits go to the dispatch; the
    /// dispatch compares exactly the landings; the calls pass exactly the heads, in order, each through
    /// eight times its opcode; and after each call stand exactly the entries its block's target and tail
    /// dictate, comparing that block's own target and successor, with a fall-through tail followed by the
    /// call that passes the successor. A layout that compared a block's target where its successor belongs,
    /// or a neighbour's successor, fails it.
    /// </para>
    /// <para>
    /// <b>THE DISPATCH IS RUN, NOT READ.</b> Each entry's compare and branch is followed for a negative
    /// answer, for every offset from one before the unit to its end, and for the largest answer there is:
    /// a negative answer must leave, a landing must reach the call that passes it, and every other answer
    /// must reach the defect. An above-branch that named the wrong subtree fails it, whatever the tree's
    /// shape. Whether the dispatch is the tree the emitter writes, and whether those templates are the bytes
    /// the emitter writes, is not asked here.
    /// </para>
    /// </remarks>
    private static (string, bool, string) ThePlanKeepsItsOwnBooks()
    {
        const string Name = "the plan keeps its own books";
        var units = 0;
        var heads = 0;
        var entries = 0;
        var answers = 0;

        foreach (var program in WidePrograms)
        {
            var compiled = CompileWide(program, JsNativeBackends.X64Windows);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return (Name, false, program.Name + ": " + Refusal(compiled));
            }

            if (!NativeLifecycle.TryReadImage(compiled.Artifact, out var image, out var refusal))
            {
                return (Name, false, program.Name + ": " + refusal);
            }

            var grouped = JsBaselineBlocks.GroupHandlerOffsets(image);
            var why = GroupingBooks(image, grouped);

            if (why is not null)
            {
                return (Name, false, program.Name + ": " + why);
            }

            for (var unit = 0; unit < image.Functions.Length; unit++)
            {
                if (!JsBaselineBlocks.TryPlan(image, unit, grouped.Of(unit), out var plan, out refusal))
                {
                    return (Name, false, program.Name + ", unit " + unit + ": the plan refused - " + refusal);
                }

                why = PlanBooks(image, unit, plan);
                var laid = 0;
                var run = 0;

                if (why is null)
                {
                    why = LayoutBooks(plan, out laid);
                }

                if (why is null)
                {
                    why = DispatchBooks(plan, JsBaselineBlocks.Layout(plan), out run);
                }

                if (why is not null)
                {
                    return (Name, false, program.Name + ", unit " + unit + ": " + why);
                }

                units++;
                heads += plan.Blocks.Length;
                entries += laid;
                answers += run;
            }
        }

        return (
            Name,
            true,
            units + " units of " + WidePrograms.Length + " programs planned from handler offsets grouped once per program: " +
                heads + " heads, " + entries + " layout entries, each agreeing with a walk, as long as its grammar counts, " +
                "resolving every branch and laying out its own block's tail; " + answers +
                " answers run through the dispatches, each landing reaching its own head and every other answer the defect or the epilogue");
    }

    /// <summary>Why a program's grouped handler offsets differ from a walk over every region for every unit, or <see langword="null"/>.</summary>
    private static string? GroupingBooks(JsNativeProgramImage image, JsBaselineHandlerOffsets grouped)
    {
        if (grouped.Units != image.Functions.Length)
        {
            return "the handler offsets are grouped for " + grouped.Units + " units and the image has " + image.Functions.Length;
        }

        for (var unit = 0; unit < image.Functions.Length; unit++)
        {
            var walked = new System.Collections.Generic.List<uint>();

            foreach (var region in image.Regions)
            {
                if (region.FunctionIndex == (uint)unit)
                {
                    walked.Add(region.HandlerOffset);
                }
            }

            if (!System.MemoryExtensions.SequenceEqual(grouped.Of(unit), new System.ReadOnlySpan<uint>(walked.ToArray())))
            {
                return "unit " + unit + "'s grouped handler offsets are " + string.Join(",", grouped.Of(unit).ToArray()) +
                    " where the regions give " + string.Join(",", walked);
            }
        }

        if (!grouped.Of(-1).IsEmpty || !grouped.Of(image.Functions.Length).IsEmpty)
        {
            return "the grouping answers handler offsets for a unit the image does not have";
        }

        return null;
    }

    /// <summary>Why a unit's plan differs from a naive walk over the same predicates, or <see langword="null"/>.</summary>
    private static string? PlanBooks(JsNativeProgramImage image, int unit, JsBaselineUnitPlan plan)
    {
        var code = image.Code;
        var row = image.Functions[unit];
        var first = (int)row.CodeOffset;
        var end = first + (int)row.CodeLength;

        if (plan.UnitIndex != unit || plan.First != first || plan.End != end)
        {
            return "the plan is of [" + plan.First + ", " + plan.End + ") unit " + plan.UnitIndex;
        }

        var starts = new System.Collections.Generic.List<int>();
        var landings = new System.Collections.Generic.SortedSet<int> { first };

        for (var at = first; at < end; at += JsOpcodes.InstructionWidth((JsOpcode)code[at]))
        {
            starts.Add(at);

            switch ((JsOpcode)code[at])
            {
                case JsOpcode.Yield or JsOpcode.Await or JsOpcode.EnterBody:
                    landings.Add(at + JsOpcodes.InstructionWidth((JsOpcode)code[at]));
                    break;

                case JsOpcode.YieldDelegate:
                    landings.Add(at);
                    break;
            }
        }

        foreach (var region in image.Regions)
        {
            if (region.FunctionIndex == (uint)unit)
            {
                landings.Add((int)region.HandlerOffset);
            }
        }

        if (!System.Linq.Enumerable.SequenceEqual(landings, plan.Landings.ToArray()))
        {
            return "the plan's landings are " + string.Join(",", plan.Landings.ToArray()) + " where a walk gives " + string.Join(",", landings);
        }

        var headSet = new System.Collections.Generic.SortedSet<int>(landings);

        foreach (var at in starts)
        {
            var opcode = (JsOpcode)code[at];
            var following = at + JsOpcodes.InstructionWidth(opcode);

            if (JsOpcodes.HasCodeTarget(opcode))
            {
                headSet.Add((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(System.MemoryExtensions.AsSpan(code, at + 1)));
            }

            if (JsBaselineBlocks.RunsAlone(opcode))
            {
                headSet.Add(at);
            }

            if (JsBaselineBlocks.EndsBlock(opcode) && following < end)
            {
                headSet.Add(following);
            }
        }

        var ordered = System.Linq.Enumerable.ToList(headSet);

        if (ordered.Count != plan.Blocks.Length)
        {
            return "the plan has " + plan.Blocks.Length + " heads where a walk finds " + ordered.Count;
        }

        for (var index = 0; index < ordered.Count; index++)
        {
            var head = ordered[index];
            var last = head;

            while (true)
            {
                var following = last + JsOpcodes.InstructionWidth((JsOpcode)code[last]);

                if (JsBaselineBlocks.EndsBlock((JsOpcode)code[last]) ||
                    following >= end ||
                    JsBaselineBlocks.RunsAlone((JsOpcode)code[following]))
                {
                    break;
                }

                last = following;
            }

            var ending = (JsOpcode)code[last];
            var successor = last + JsOpcodes.InstructionWidth(ending);
            var hasTarget = JsOpcodes.HasCodeTarget(ending);
            var next = index + 1 < ordered.Count ? ordered[index + 1] : end;

            var tail = JsOpcodes.IsTerminal(ending) || successor == end
                ? JsBaselineTail.Leave
                : successor == next ? JsBaselineTail.FallThrough : JsBaselineTail.Branch;

            var walked = new JsBaselineBlock(
                head,
                (JsOpcode)code[head],
                last,
                hasTarget,
                hasTarget ? (int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(System.MemoryExtensions.AsSpan(code, last + 1)) : JsBaselineBlock.NoTarget,
                successor,
                tail);

            if (plan.Blocks[index] != walked)
            {
                return "the plan's block " + index + " is " + plan.Blocks[index] + " where a walk gives " + walked;
            }
        }

        return null;
    }

    /// <summary>Why a unit's layout does not resolve as its plan says, or <see langword="null"/>.</summary>
    private static string? LayoutBooks(JsBaselineUnitPlan plan, out int laid)
    {
        var layout = JsBaselineBlocks.Layout(plan);
        laid = layout.Length;

        if (JsBaselineBlocks.LayoutLength(plan) != layout.Length)
        {
            return "the layout has " + layout.Length + " entries where LayoutLength counts " + JsBaselineBlocks.LayoutLength(plan);
        }

        if (layout.Length < 4 ||
            layout[0].Template != JsBaselineTemplate.TestEax ||
            layout[1].Template != JsBaselineTemplate.JsLeave ||
            layout[1].Target != JsBaselineInstruction.Leave)
        {
            return "the layout does not open with the dispatch's sign test";
        }

        var compared = new System.Collections.Generic.List<int>();
        var calls = new System.Collections.Generic.List<(int Pc, int Slot)>();

        for (var index = 0; index < layout.Length; index++)
        {
            var entry = layout[index];
            var branches = entry.Template is JsBaselineTemplate.JsLeave or JsBaselineTemplate.Je or
                JsBaselineTemplate.Jne or JsBaselineTemplate.Ja or JsBaselineTemplate.Jmp;

            if (!branches)
            {
                if (entry.Target != JsBaselineInstruction.None)
                {
                    return "entry " + index + " (" + entry.Template + ") names a target and does not branch";
                }

                if (entry.Template == JsBaselineTemplate.CmpEaxPc && entry.Role == JsBaselineRole.Dispatch)
                {
                    compared.Add(entry.Operand);
                }

                if (entry.Template == JsBaselineTemplate.CallSlot)
                {
                    if (index < 2 || layout[index - 1].Template != JsBaselineTemplate.MovArg1Pc ||
                        layout[index - 2].Template != JsBaselineTemplate.MovArg0R14)
                    {
                        return "the call at entry " + index + " is not preceded by the two moves";
                    }

                    calls.Add((layout[index - 1].Operand, entry.Operand));
                }

                continue;
            }

            if (entry.Target != JsBaselineInstruction.Leave && (entry.Target < 0 || entry.Target >= layout.Length))
            {
                return "entry " + index + " (" + entry.Template + ") resolves to " + entry.Target;
            }

            if (entry.Template == JsBaselineTemplate.Je)
            {
                var target = entry.Target;

                if (index == 0 || layout[index - 1].Template != JsBaselineTemplate.CmpEaxPc ||
                    target < 0 || target + 1 >= layout.Length ||
                    layout[target].Template != JsBaselineTemplate.MovArg0R14 ||
                    layout[target + 1].Operand != layout[index - 1].Operand)
                {
                    return "the equal branch at entry " + index + " does not land on the call of the head it compared";
                }
            }

            if ((entry.Template == JsBaselineTemplate.Jne ||
                    (entry.Template == JsBaselineTemplate.Jmp && entry.Role == JsBaselineRole.Tail)) &&
                entry.Target != 0)
            {
                return "the tail exit at entry " + index + " goes to " + entry.Target + " and not to the dispatch";
            }

            if (entry.Template == JsBaselineTemplate.Jmp && entry.Role == JsBaselineRole.Dispatch &&
                entry.Target != JsBaselineInstruction.Leave &&
                layout[entry.Target].Template != JsBaselineTemplate.MovStatus)
            {
                return "the dispatch's jump at entry " + index + " goes neither to the defect nor out of the unit";
            }
        }

        compared.Sort();

        if (!System.Linq.Enumerable.SequenceEqual(compared, plan.Landings.ToArray()))
        {
            return "the dispatch compares " + string.Join(",", compared) + " and the landings are " + string.Join(",", plan.Landings.ToArray());
        }

        if (calls.Count != plan.Blocks.Length)
        {
            return "the layout calls " + calls.Count + " times for " + plan.Blocks.Length + " heads";
        }

        for (var index = 0; index < calls.Count; index++)
        {
            if (calls[index] != (plan.Blocks[index].Head, (int)plan.Blocks[index].HeadOpcode * 8))
            {
                return "call " + index + " passes " + calls[index] + " for the head " + plan.Blocks[index].Head;
            }
        }

        // THE TAILS, BLOCK BY BLOCK: between a head's call and the next head's first move stand exactly
        // the entries its block's target and tail dictate, each comparing the block's own target or
        // successor, and a fall-through tail is followed by the call that passes the successor it compared.
        var callAt = new System.Collections.Generic.List<int>();

        for (var index = 0; index < layout.Length; index++)
        {
            if (layout[index].Template == JsBaselineTemplate.CallSlot)
            {
                callAt.Add(index);
            }
        }

        for (var index = 0; index < plan.Blocks.Length; index++)
        {
            var block = plan.Blocks[index];
            var dictated = new System.Collections.Generic.List<(JsBaselineTemplate Template, int Pc)>();

            if (block.HasTarget)
            {
                dictated.Add((JsBaselineTemplate.CmpEaxPc, block.Target));
                dictated.Add((JsBaselineTemplate.Je, block.Target));
            }

            switch (block.Tail)
            {
                case JsBaselineTail.Leave:
                    dictated.Add((JsBaselineTemplate.Jmp, -1));
                    break;

                case JsBaselineTail.FallThrough:
                    dictated.Add((JsBaselineTemplate.CmpEaxPc, block.Following));
                    dictated.Add((JsBaselineTemplate.Jne, -1));
                    break;

                default:
                    dictated.Add((JsBaselineTemplate.CmpEaxPc, block.Following));
                    dictated.Add((JsBaselineTemplate.Je, block.Following));
                    dictated.Add((JsBaselineTemplate.Jmp, -1));
                    break;
            }

            var from = callAt[index] + 1;
            var to = index + 1 < callAt.Count ? callAt[index + 1] - 2 : layout.Length;

            if (to - from != dictated.Count)
            {
                return "the tail of the head " + block.Head + " has " + (to - from) + " entries where its block dictates " + dictated.Count;
            }

            for (var step = 0; step < dictated.Count; step++)
            {
                var entry = layout[from + step];
                var (template, pc) = dictated[step];

                if (entry.Template != template || entry.Role != JsBaselineRole.Tail || entry.Head != block.Head)
                {
                    return "entry " + (from + step) + " of the head " + block.Head + "'s tail is " + entry.Template + " (" + entry.Role + ") where its block dictates " + template;
                }

                var answered = template switch
                {
                    JsBaselineTemplate.CmpEaxPc => entry.Operand,
                    JsBaselineTemplate.Je => layout[entry.Target + 1].Operand,
                    _ => entry.Target == 0 ? -1 : int.MinValue,
                };

                if (answered != pc)
                {
                    return "entry " + (from + step) + " of the head " + block.Head + "'s tail names " + answered + " where its block dictates " + pc + " (-1 is the dispatch)";
                }
            }

            if (block.Tail == JsBaselineTail.FallThrough &&
                (index + 1 >= callAt.Count || layout[callAt[index + 1] - 1].Operand != block.Following))
            {
                return "the head " + block.Head + " falls through into a call that does not pass its successor " + block.Following;
            }
        }

        return null;
    }

    /// <summary>What running a layout's dispatch answered: it left the unit.</summary>
    private const int DispatchLeft = -1;

    /// <summary>What running a layout's dispatch answered: it reached the defect block.</summary>
    private const int DispatchDefect = -2;

    /// <summary>What running a layout's dispatch answered: it reached no entry a dispatch may end at.</summary>
    private const int DispatchLost = int.MinValue;

    /// <summary>Why a unit's dispatch, run, does not send each answer where the landings say, or <see langword="null"/>.</summary>
    private static string? DispatchBooks(JsBaselineUnitPlan plan, JsBaselineInstruction[] layout, out int run)
    {
        run = 0;
        var landings = new System.Collections.Generic.HashSet<int>(plan.Landings.ToArray());

        foreach (var answer in new[] { int.MinValue, -1 })
        {
            run++;
            var reached = RunDispatch(layout, answer);

            if (reached != DispatchLeft)
            {
                return "the answer " + answer + " reaches " + Reached(reached) + " where a negative answer leaves the unit";
            }
        }

        for (var answer = System.Math.Max(0, plan.First - 1); answer <= plan.End; answer++)
        {
            run++;
            var reached = RunDispatch(layout, answer);
            var expected = landings.Contains(answer) ? answer : DispatchDefect;

            if (reached != expected)
            {
                return "the answer " + answer + " reaches " + Reached(reached) + " where the landings send it to " + Reached(expected);
            }
        }

        run++;
        var largest = RunDispatch(layout, int.MaxValue);

        if (largest != DispatchDefect)
        {
            return "the answer " + int.MaxValue + " reaches " + Reached(largest) + " where the landings send it to the defect";
        }

        return null;
    }

    /// <summary>Follows a layout's dispatch for one answer, from its first entry, compare by compare.</summary>
    private static int RunDispatch(JsBaselineInstruction[] layout, int answer)
    {
        var at = 0;
        var sign = false;
        var equal = false;
        var above = false;

        for (var steps = 0; steps <= layout.Length; steps++)
        {
            if (at == JsBaselineInstruction.Leave)
            {
                return DispatchLeft;
            }

            if (at < 0 || at >= layout.Length)
            {
                return DispatchLost;
            }

            var entry = layout[at];

            switch (entry.Template)
            {
                case JsBaselineTemplate.TestEax:
                    sign = answer < 0;
                    at++;
                    break;

                case JsBaselineTemplate.JsLeave:
                    at = sign ? entry.Target : at + 1;
                    break;

                case JsBaselineTemplate.CmpEaxPc:
                    equal = answer == entry.Operand;
                    above = (uint)answer > (uint)entry.Operand;
                    at++;
                    break;

                case JsBaselineTemplate.Je:
                    at = equal ? entry.Target : at + 1;
                    break;

                case JsBaselineTemplate.Jne:
                    at = equal ? at + 1 : entry.Target;
                    break;

                case JsBaselineTemplate.Ja:
                    at = above ? entry.Target : at + 1;
                    break;

                case JsBaselineTemplate.Jmp:
                    at = entry.Target;
                    break;

                case JsBaselineTemplate.MovStatus when entry.Operand == (int)JsBaselineStatus.Defect:
                    return DispatchDefect;

                case JsBaselineTemplate.MovArg0R14 when at + 1 < layout.Length &&
                    layout[at + 1].Template == JsBaselineTemplate.MovArg1Pc:
                    return layout[at + 1].Operand;

                default:
                    return DispatchLost;
            }
        }

        return DispatchLost;
    }

    /// <summary>Names what running a dispatch reached.</summary>
    private static string Reached(int reached) => reached switch
    {
        DispatchLeft => "the epilogue",
        DispatchDefect => "the defect",
        DispatchLost => "no entry a dispatch may end at",
        _ => "the call passing " + reached,
    };

    // ---- the baseline form over the wide manifest ----------------------------------------------

    /// <summary>
    /// The wide programs the baseline rows compile, one family of instruction and landing each.
    /// </summary>
    /// <remarks>
    /// <b>EACH ONE REACHES A KIND OF LANDING OR TAIL THE OTHERS DO NOT.</b> Objects, strings and
    /// closures reach the plain fall-through tail; the class reaches <c>super</c> and private
    /// names; try, catch and finally reach region handler landings and a break through a finally;
    /// the generator reaches a landing per <c>yield</c> - more than a leaf of the compare tree
    /// holds, so an unsigned split is emitted - and resumes by <c>return()</c> and <c>throw()</c>;
    /// <c>yield*</c> reaches the delegating landing; the async programs reach <c>await</c> landings
    /// and the asynchronous iteration instructions that carry code targets; the parameter program
    /// reaches <c>EnterBody</c>; the module reaches an import; and the last reaches a switch, a
    /// labelled continue and <c>with</c>. A program with <c>Library</c> set compiles as a two-module
    /// graph whose main module imports <c>lib</c>.
    /// </remarks>
    private static readonly (string Name, string Source, string? Library)[] WidePrograms =
    [
        ("an object literal and a property read", "var o = { a: 1, b: { c: 2 } }; o.b.c + o.a;", null),
        ("string concatenation", "var s = 'a'; for (var i = 0; i < 3; i++) { s = s + i; } s + '!';", null),
        ("a closure counter", "function counter() { var n = 0; return function () { n = n + 1; return n; }; } var c = counter(); c(); c();", null),
        ("a class with super and private members", "class A { #x = 1; get x() { return this.#x; } m() { return 2; } } class B extends A { #y = 3; constructor() { super(); } m() { return super.m() + this.#y + this.x; } } new B().m();", null),
        ("try, catch and finally with a break through the finally", "var log = ''; for (var i = 0; i < 3; i++) { try { try { if (i === 1) { break; } throw new Error('e' + i); } catch (e) { log += e.message; } finally { log += 'f'; } } finally { log += 'g'; } } log;", null),
        ("a generator resumed by return() and throw() past a leaf of the tree", "function* g() { try { yield 1; yield 2; yield 3; yield 4; yield 5; yield 6; yield 7; yield 8; yield 9; yield 10; } finally { yield 11; } } var a = g(); a.next(); a.return(5); var b = g(); b.next(); try { b.throw(new Error('x')); } catch (e) { } a.next().done;", null),
        ("yield* over a generator and over an array", "function* inner() { yield 1; yield 2; } function* outer() { yield* inner(); yield* [3, 4]; } var t = 0; for (var v of outer()) { t += v; } t;", null),
        ("async and await with a rejected await", "async function f() { try { await Promise.reject(new Error('no')); } catch (e) { return e.message; } } var r; f().then(function (v) { r = v; }); r;", null),
        ("an async generator with for await and yield* into an async iterator", "async function* ag() { yield 1; yield* (async function* () { yield 2; })(); } async function run() { var t = 0; for await (var v of ag()) { t += v; } return t; } run();", null),
        ("for-in", "var o = { a: 1, b: 2 }; var k = ''; for (var p in o) { k += p; } k;", null),
        ("for-of with an early exit", "var t = 0; for (var v of [1, 2, 3, 4]) { if (v > 2) { break; } t += v; } t;", null),
        ("destructuring parameters with defaults", "function f({ a = 1, b } = {}, [c, d = 4] = []) { return a + (b || 0) + (c || 0) + d; } f() + f({ b: 2 }, [3]);", null),
        ("a module with an import", "import { add, n } from 'lib'; export const r = add(n, 2);", "export function add(a, b) { return a + b; } export let n = 1;"),
        ("a switch, a labelled continue and with", "var t = 0; outer: for (var i = 0; i < 3; i++) { for (var j = 0; j < 3; j++) { if (j === 1) { continue outer; } switch (i) { case 0: t += 1; break; case 1: t += 10; break; default: t += 100; } } } var o = { x: 5 }; with (o) { t += x; } t;", null),
    ];

    /// <summary>
    /// The program the golden rows retain the bytes of: two units, a property read, a branch and a
    /// throw.
    /// </summary>
    private const string GoldenSource = "let o={a:1}; function f(x){ if (x) { return o.a; } throw 1; } f(1);";

    /// <summary>Every row about the baseline form's templates, its scan clauses and its emitter.</summary>
    /// <remarks>
    /// <para>
    /// <b>NOTHING HERE RUNS EMITTED CODE, AND BOTH CONVENTIONS ARE CHECKED ON EVERY HOST.</b> The
    /// emitter is a pure function of the image, so a System V emission is as available on Windows as
    /// a Windows one is; what these rows establish is that every emission scans clean against its own
    /// table, that the table is exactly what the emitter writes, that the frame-shape clauses refuse
    /// what they are written against, and that the verifier re-emits the same bytes. Whether the
    /// handlers those bytes call answer what the interpreter answers is the business of the rows that
    /// execute, beside the engine.
    /// </para>
    /// <para>
    /// <b>The numeric rows above are not moved by any of this, and the last rows here say so
    /// explicitly</b> for the three payloads whose tables a wide artifact could have reached.
    /// </para>
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Baseline()
    {
        var windows = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var systemV = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        var rows = new System.Collections.Generic.List<(string, bool, string)>
        {
            BaselineClosure(JsX64Abi.Windows, windows),
            BaselineClosure(JsX64Abi.SystemV, systemV),
            BaselineCoverage(JsX64Abi.Windows, windows),
            BaselineCoverage(JsX64Abi.SystemV, systemV),
            TheBaselineTablesWriteNothingAndCallOnlyTheHandlerTable(),
            TheBaselineConstantsAreTheConventionTables(),
            BaselineGolden(JsX64Abi.Windows, GoldenWindows),
            BaselineGolden(JsX64Abi.SystemV, GoldenSystemV),
            TheTwoConventionsEmitOneTemplateSequence(),
            BaselineReEmission(JsX64Abi.Windows),
            BaselineReEmission(JsX64Abi.SystemV),
            AReEmittingVerifierRefusesAHandlerSwappedInThePayload(),
            TheArm64BackendRefusesTheWideManifest(),
            TheBaselinePartitionTableIsThePredicates(),
            EveryOpcodeIsClassifiedForTheBaselinePartition(),
            ThePlanKeepsItsOwnBooks(),
        };

        rows.AddRange(BaselineRefusals());
        rows.AddRange(UnchangedAnswers());
        return rows;
    }

    /// <summary>Compiles one wide program to the baseline form for one backend.</summary>
    private static JsCompilation CompileWide(
        (string Name, string Source, string? Library) program, string backend)
    {
        var request = new JsCompileRequest(JsFeatureManifest.Wide, JsOutputForm.Native, backend);

        return program.Library is null
            ? JsCompiler.Compile(
                [new JsScriptUnit("baseline.js", program.Source, SliceParseOptions.Script)],
                [],
                request)
            : JsCompiler.Compile(
                [],
                [
                    new JsModuleUnit("lib", program.Library, SliceParseOptions.Module),
                    new JsModuleUnit(
                        "main",
                        program.Source,
                        SliceParseOptions.Module,
                        [new JsResolvedRequest("lib", "lib")]),
                ],
                request);
    }

    /// <summary>Why a compilation was refused, as one line.</summary>
    private static string Refusal(JsCompilation compiled) =>
        compiled.Diagnostics.Count == 0
            ? "refused with no diagnostic"
            : string.Join("; ", compiled.Diagnostics);

    /// <summary>
    /// Every wide program emits under the named convention and its image scans clean against the
    /// baseline table.
    /// </summary>
    /// <remarks>
    /// <b>A REFUSED PROGRAM FAILS THIS ROW, which is the difference from the numeric closure
    /// rows.</b> Those backends compile a closed subset and refuse the rest by name; the baseline
    /// form has one lowering per class of instruction and no type facts, so the only refusal a
    /// verified wide program can meet is the size ceiling, and none of these is near it.
    /// </remarks>
    private static (string, bool, string) BaselineClosure(
        JsX64Abi abi, System.Collections.Generic.HashSet<string> instantiated)
    {
        var name = "everything `" + abi.Name + "` emits for the wide manifest, the baseline scan accepts";
        var bytes = 0;
        var units = 0;

        foreach (var program in WidePrograms)
        {
            var compiled = CompileWide(program, abi.Name);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return (name, false, program.Name + ": " + Refusal(compiled));
            }

            if (!NativeLifecycle.TryReadEmitted(
                compiled.Artifact, out var code, out var symbols, out var refusal))
            {
                return (name, false, program.Name + ": " + refusal);
            }

            var result = JsNativeScan.Scan(
                abi.Architecture, JsNativeTier.Baseline, code, symbols, 16, instantiated);

            if (!result.Accepted)
            {
                return (
                    name,
                    false,
                    program.Name + ": the scan refused " + code.Length + " emitted bytes with " +
                    result.Outcome + " at offset " + result.Offset + " - " + result.Reason +
                    "; the bytes there read " + Around(code, result.Offset));
            }

            bytes += code.Length;
            units += symbols.Length;
        }

        return (
            name,
            true,
            "all " + WidePrograms.Length + " programs emitted and scanned: " + units +
                " code units, " + bytes + " bytes");
    }

    /// <summary>Every template of the baseline table is instantiated by one of the wide programs.</summary>
    private static (string, bool, string) BaselineCoverage(
        JsX64Abi abi, System.Collections.Generic.HashSet<string> instantiated)
    {
        var name = "every baseline template `" + abi.Name + "` has is reached by one of the wide programs";
        var table = JsNativeTemplates.For(abi.Architecture, JsNativeTier.Baseline);
        var uncovered = new System.Collections.Generic.List<string>();

        foreach (var template in table)
        {
            if (!instantiated.Contains(template.Text))
            {
                uncovered.Add(template.Text);
            }
        }

        return (
            name,
            uncovered.Count == 0 && table.Length == 21,
            uncovered.Count == 0
                ? instantiated.Count + " of " + table.Length + " templates instantiated, none out of reach"
                : "unreached: " + string.Join(", ", uncovered));
    }

    /// <summary>
    /// The baseline tables carry no call to a unit entry, no memory destination, and exactly one
    /// indirect transfer: the call through the handler table - judged from each template's bytes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>CLAUSE S4 IS A PROPERTY OF THE TABLE AND THIS ROW PINS IT.</b> The argument that every
    /// indirect call lands in the handler table rests on nothing in the table writing memory - so
    /// the frame's first word is still the table base the runtime stored - and on the one indirect
    /// transfer indexing that table by a field kind that admits only defined opcodes. A template added
    /// with a memory destination, or a second indirect form, turns this row red before any scan
    /// accepts a payload that uses it.
    /// </para>
    /// <para>
    /// <b>A TEMPLATE'S NAME IS NOT EVIDENCE, SO THE ROW DECODES WHAT THE SCAN COMPARES.</b> The scan
    /// matches bytes under a mask and never reads a name; a template named <c>mov eax, arg1d</c> whose
    /// bytes are <c>49 89 1E</c> writes through R14 all the same. So each template's fixed bytes are
    /// decoded as exactly one x86-64 instruction, with every prefix, opcode, ModRM and SIB bit fixed by
    /// the mask, and the decoding is conservative: an opcode it has no rule for is a failure, not a
    /// pass. The controls at the end are byte sequences under misleading names that the judgement must
    /// refuse, so a judgement weakened into accepting them turns the row red as well.
    /// </para>
    /// </remarks>
    private static (string, bool, string) TheBaselineTablesWriteNothingAndCallOnlyTheHandlerTable()
    {
        const string Name =
            "the baseline tables write no memory and transfer indirectly only through the handler table";

        var failures = new System.Collections.Generic.List<string>();
        var judged = 0;

        foreach (var architecture in new[] { JsNativeArchitecture.X64Windows, JsNativeArchitecture.X64SystemV })
        {
            var calls = 0;

            foreach (var template in JsNativeTemplates.For(architecture, JsNativeTier.Baseline))
            {
                judged++;
                var refusal = WhyABaselineTemplateIsRefused(template, out var isHandlerCall);

                if (refusal is not null)
                {
                    failures.Add(architecture + ": `" + template.Text + "` " + refusal);
                }

                if (isHandlerCall)
                {
                    calls++;
                }
            }

            if (calls != 1)
            {
                failures.Add(architecture + ": " + calls + " handler-table calls where one is expected");
            }
        }

        var slot = new JsNativeTemplateField(JsNativeFieldKind.HelperSlot, 16, 32, true, 1, false);
        var pc = new JsNativeTemplateField(JsNativeFieldKind.BytecodePc, 8, 32, true, 1, false);
        var controls = new[]
        {
            // mov [r14], rbx - the finding's own example, under a prologue template's name.
            new JsNativeTemplate("mov eax, arg1d", [0x49, 0x89, 0x1E], false),

            // mov qword [rbx+0], imm32.
            new JsNativeTemplate("add rsp, frame", [0x48, 0xC7, 0x83, 0, 0, 0, 0, 0, 0, 0, 0], false),

            // sete byte [rax].
            new JsNativeTemplate("test eax, eax", [0x0F, 0x94, 0x00], false),

            // pop qword [r14].
            new JsNativeTemplate("pop rbx", [0x41, 0x8F, 0x06], false),

            // call rax.
            new JsNativeTemplate("call [rbx+slot]", [0xFF, 0xD0], false),

            // call qword [rdx+disp32], with the displacement a helper-slot field.
            new JsNativeTemplate("call [rbx+slot]", [0xFF, 0x92, 0, 0, 0, 0], false, slot),

            // jmp qword [rbx+disp32], with the displacement a helper-slot field.
            new JsNativeTemplate("call [rbx+slot]", [0xFF, 0xA3, 0, 0, 0, 0], false, slot),

            // call rel32.
            new JsNativeTemplate("jmp rel32", [0xE8, 0, 0, 0, 0], false),

            // cmp eax, imm32 followed by mov [rax], ebx inside the same template.
            new JsNativeTemplate("cmp eax, pc", [0x3D, 0, 0, 0, 0, 0x89, 0x18], false, pc),

            // An opcode byte a field covers, so the mask compares nothing there.
            new JsNativeTemplate(
                "ret",
                [0xC3, 0, 0, 0],
                true,
                new JsNativeTemplateField(JsNativeFieldKind.BytecodePc, 0, 32, true, 1, false)),

            // imul eax, eax - legal, harmless, and an opcode the judgement has no rule for.
            new JsNativeTemplate("push rbx", [0x0F, 0xAF, 0xC0], false),
        };

        var refused = 0;

        foreach (var control in controls)
        {
            if (WhyABaselineTemplateIsRefused(control, out var called) is not null && !called)
            {
                refused++;
            }
            else
            {
                failures.Add(
                    "control " + System.Convert.ToHexString(control.Fixed) + " named `" + control.Text +
                    "` was not refused by its bytes");
            }
        }

        return (
            Name,
            failures.Count == 0,
            failures.Count == 0
                ? judged + " templates judged by their bytes: one handler-table call per table (FF 93 with a helper-slot " +
                  "disp32), no memory destination, no other transfer through memory or a register, no direct call, " +
                  "no unit-entry branch; " + refused + " of " + controls.Length + " misnamed byte controls refused"
                : string.Join("; ", failures));
    }

    /// <summary>
    /// Why the bytes of one baseline template break clause S4, or <see langword="null"/> when they
    /// are exactly one instruction that writes no memory and transfers control only as the handler
    /// call or a direct branch does.
    /// </summary>
    /// <remarks>
    /// <b>EVERY OPCODE HERE HAS A RULE, AND AN OPCODE WITHOUT ONE IS A REFUSAL.</b> The rules cover
    /// what the baseline form can plausibly grow into - pushes and pops, register moves and loads,
    /// arithmetic and compares with an immediate, conditional sets, direct branches, the two indirect
    /// groups of <c>FF</c> - so a new template either decodes under one of them and is judged, or
    /// fails here and makes its author extend the judgement deliberately. A ModRM <c>mod</c> of three
    /// names a register and anything else names memory; for an opcode whose r/m operand is its
    /// destination, that is a memory write.
    /// </remarks>
    private static string? WhyABaselineTemplateIsRefused(JsNativeTemplate template, out bool isHandlerCall)
    {
        isHandlerCall = false;
        var bytes = template.Fixed;
        var mask = template.Mask;

        foreach (var field in template.Fields)
        {
            if (field.Kind == JsNativeFieldKind.UnitEntryBranch)
            {
                return "branches to a unit entry";
            }
        }

        bool IsFixed(int index) => index < bytes.Length && mask[index] == 0xFF;

        var at = 0;
        var wide = false;

        if (IsFixed(0) && (bytes[0] & 0xF0) == 0x40)
        {
            wide = (bytes[0] & 0x08) != 0;
            at = 1;
        }

        if (!IsFixed(at))
        {
            return "leaves its opcode to a field or ends before one";
        }

        var opcode = bytes[at++];
        var second = -1;
        var hasModRm = false;
        var writesRm = false;
        var immediate = 0;

        switch (opcode)
        {
            case >= 0x50 and <= 0x5F:
            case 0x90:
            case 0xC3:
                break;
            case >= 0xB8 and <= 0xBF:
                immediate = wide ? 8 : 4;
                break;
            case 0x3D:
            case 0xE9:
                immediate = 4;
                break;
            case 0xE8:
                return "is a direct call, which reaches something other than the handler table";
            case 0x84:
            case 0x85:
            case 0x8A:
            case 0x8B:
            case 0xFF:
                hasModRm = true;
                break;
            case 0x88:
            case 0x89:
            case 0x8F:
                hasModRm = true;
                writesRm = true;
                break;
            case 0x80:
            case 0x83:
            case 0xC6:
                hasModRm = true;
                writesRm = true;
                immediate = 1;
                break;
            case 0x81:
            case 0xC7:
                hasModRm = true;
                writesRm = true;
                immediate = 4;
                break;
            case 0x0F:
                if (!IsFixed(at))
                {
                    return "leaves its second opcode byte to a field or ends before one";
                }

                second = bytes[at++];

                if (second is >= 0x80 and <= 0x8F)
                {
                    immediate = 4;
                }
                else if (second is >= 0x90 and <= 0x9F)
                {
                    hasModRm = true;
                    writesRm = true;
                }
                else
                {
                    return "carries the opcode 0F " + second.ToString("X2", System.Globalization.CultureInfo.InvariantCulture) +
                           ", which this judgement has no rule for";
                }

                break;
            default:
                return "carries the opcode " + opcode.ToString("X2", System.Globalization.CultureInfo.InvariantCulture) +
                       ", which this judgement has no rule for";
        }

        var memory = false;

        if (hasModRm)
        {
            if (!IsFixed(at))
            {
                return "leaves its ModRM byte to a field or ends before one";
            }

            var modRm = bytes[at++];
            var mod = modRm >> 6;
            var reg = (modRm >> 3) & 7;
            var rm = modRm & 7;
            memory = mod != 3;

            if (memory && rm == 4)
            {
                if (!IsFixed(at))
                {
                    return "leaves its SIB byte to a field or ends before one";
                }

                if (mod == 0 && (bytes[at] & 7) == 5)
                {
                    at += 4;
                }

                at++;
            }

            at += mod switch
            {
                1 => 1,
                2 => 4,
                0 when rm == 5 => 4,
                _ => 0,
            };

            if (opcode is 0x80 or 0x81 or 0x83 && reg == 7)
            {
                writesRm = false;
            }

            if (opcode is 0x8F or 0xC6 or 0xC7 && reg != 0)
            {
                return "carries the undefined extension /" + reg + " of its opcode";
            }

            if (opcode == 0xFF)
            {
                switch (reg)
                {
                    case 0:
                    case 1:
                        writesRm = true;
                        break;
                    case >= 2 and <= 5:
                        if (template.Length != 6 || bytes[0] != 0xFF || modRm != 0x93 || template.Fields.Length != 1 ||
                            template.Fields[0].Kind != JsNativeFieldKind.HelperSlot ||
                            template.Fields[0].BitOffset != 16 || template.Fields[0].BitWidth != 32)
                        {
                            return "transfers control through memory or a register other than as call qword [rbx+disp32] " +
                                   "with the displacement a helper-slot field";
                        }

                        isHandlerCall = true;
                        break;
                    default:
                        return "carries the extension /" + reg + " of FF, which this judgement has no rule for";
                }
            }
        }

        if (writesRm && memory)
        {
            return "has a memory destination";
        }

        if (at + immediate != template.Length)
        {
            isHandlerCall = false;
            return "is " + template.Length + " bytes where the one instruction it opens with is " + (at + immediate);
        }

        return null;
    }

    /// <summary>
    /// The baseline form's reservation and argument registers agree across the format, the template
    /// table and the convention table.
    /// </summary>
    private static (string, bool, string) TheBaselineConstantsAreTheConventionTables()
    {
        const string Name = "the baseline reservation and argument registers are the convention table's";
        var failures = new System.Collections.Generic.List<string>();

        foreach (var abi in JsX64Abi.Rows)
        {
            var table = JsNativeTemplates.For(abi.Architecture, JsNativeTier.Baseline);
            var expected = abi.Architecture == JsNativeArchitecture.X64Windows ? JsX64Register.Rdx : JsX64Register.Rsi;

            if (abi.BaselineFrameBytes != JsBaselineAbi.FrameBytes(abi.Architecture))
            {
                failures.Add(abi.Name + ": the convention reserves " + abi.BaselineFrameBytes + " and the format " + JsBaselineAbi.FrameBytes(abi.Architecture));
            }

            if (abi.SecondArgumentRegister != expected)
            {
                failures.Add(abi.Name + ": the second argument register is " + abi.SecondArgumentRegister);
            }

            if (table[2].Fixed[3] != abi.BaselineFrameBytes || table[17].Fixed[3] != abi.BaselineFrameBytes)
            {
                failures.Add(abi.Name + ": the table's reservation is not the convention's");
            }

            if (table[3].Fixed[2] != (byte)(0xC0 | (((int)abi.FramePointerRegister & 7) << 3) | 6) ||
                table[14].Fixed[2] != (byte)(0xC0 | (6 << 3) | ((int)abi.FramePointerRegister & 7)))
            {
                failures.Add(abi.Name + ": the table moves the frame through a register other than " + abi.FramePointerRegister);
            }

            if (table[5].Fixed[1] != (byte)(0xC0 | (((int)abi.SecondArgumentRegister & 7) << 3)) ||
                table[15].Fixed[0] != (byte)(0xB8 + ((int)abi.SecondArgumentRegister & 7)))
            {
                failures.Add(abi.Name + ": the table passes the program counter through a register other than " + abi.SecondArgumentRegister);
            }
        }

        return (
            Name,
            failures.Count == 0,
            failures.Count == 0
                ? "both conventions agree about the reservation, the frame register and the program-counter register"
                : string.Join("; ", failures));
    }

    /// <summary>The Windows x64 bytes the golden program is retained as.</summary>
    private const string GoldenWindows =
        "5341564883EC284989CE498B1E89D085C00F88BD0100003D000000000F840F000000E900000000B8FDFFFFFFE9A30100" +
        "004C89F1BA00000000FF93080000003D010000000F85C5FFFFFF4C89F1BA01000000FF93900000003D050000000F85AC" +
        "FFFFFF4C89F1BA05000000FF93C80000003D080000000F8593FFFFFF4C89F1BA08000000FF93800100003D0B0000000F" +
        "857AFFFFFF4C89F1BA0B000000FF93A00000003D0E0000000F8561FFFFFF4C89F1BA0E000000FF93F80300003D110000" +
        "000F8548FFFFFF4C89F1BA11000000FF93000100003D120000000F852FFFFFFF4C89F1BA12000000FF93280000003D15" +
        "0000000F8516FFFFFF4C89F1BA15000000FF93300100003D180000000F85FDFEFFFF4C89F1BA18000000FF9308040000" +
        "3D1B0000000F85E4FEFFFF4C89F1BA1B000000FF93980000003D1E0000000F85CBFEFFFF4C89F1BA1E000000FF930800" +
        "00003D1F0000000F85B2FEFFFF4C89F1BA1F000000FF93280000003D220000000F8599FEFFFF4C89F1BA22000000FF93" +
        "880100003D240000000F8580FEFFFF4C89F1BA24000000FF93900000003D280000000F8567FEFFFF4C89F1BA28000000" +
        "FF93800000003D2C0000000F854EFEFFFF4C89F1BA2C000000FF9398010000E93BFEFFFF4883C428415E5BC390909090" +
        "5341564883EC284989CE498B1E89D085C00F88DB0000003D2D0000000F840F000000E900000000B8FDFFFFFFE9C10000" +
        "004C89F1BA2D000000FF93800000003D310000000F85C5FFFFFF4C89F1BA31000000FF93080300003D3D0000000F8450" +
        "0000003D360000000F85A1FFFFFF4C89F1BA36000000FF93980000003D390000000F8588FFFFFF4C89F1BA39000000FF" +
        "93100100003D3C0000000F856FFFFFFF4C89F1BA3C000000FF9398010000E95CFFFFFF4C89F1BA3D000000FF93280000" +
        "003D400000000F8543FFFFFF4C89F1BA40000000FF9318030000E930FFFFFF4C89F1BA41000000FF93A0010000E91DFF" +
        "FFFF4883C428415E5BC3";

    /// <summary>The System V bytes the golden program is retained as.</summary>
    private const string GoldenSystemV =
        "5341564883EC084989FE498B1E89F085C00F88BD0100003D000000000F840F000000E900000000B8FDFFFFFFE9A30100" +
        "004C89F7BE00000000FF93080000003D010000000F85C5FFFFFF4C89F7BE01000000FF93900000003D050000000F85AC" +
        "FFFFFF4C89F7BE05000000FF93C80000003D080000000F8593FFFFFF4C89F7BE08000000FF93800100003D0B0000000F" +
        "857AFFFFFF4C89F7BE0B000000FF93A00000003D0E0000000F8561FFFFFF4C89F7BE0E000000FF93F80300003D110000" +
        "000F8548FFFFFF4C89F7BE11000000FF93000100003D120000000F852FFFFFFF4C89F7BE12000000FF93280000003D15" +
        "0000000F8516FFFFFF4C89F7BE15000000FF93300100003D180000000F85FDFEFFFF4C89F7BE18000000FF9308040000" +
        "3D1B0000000F85E4FEFFFF4C89F7BE1B000000FF93980000003D1E0000000F85CBFEFFFF4C89F7BE1E000000FF930800" +
        "00003D1F0000000F85B2FEFFFF4C89F7BE1F000000FF93280000003D220000000F8599FEFFFF4C89F7BE22000000FF93" +
        "880100003D240000000F8580FEFFFF4C89F7BE24000000FF93900000003D280000000F8567FEFFFF4C89F7BE28000000" +
        "FF93800000003D2C0000000F854EFEFFFF4C89F7BE2C000000FF9398010000E93BFEFFFF4883C408415E5BC390909090" +
        "5341564883EC084989FE498B1E89F085C00F88DB0000003D2D0000000F840F000000E900000000B8FDFFFFFFE9C10000" +
        "004C89F7BE2D000000FF93800000003D310000000F85C5FFFFFF4C89F7BE31000000FF93080300003D3D0000000F8450" +
        "0000003D360000000F85A1FFFFFF4C89F7BE36000000FF93980000003D390000000F8588FFFFFF4C89F7BE39000000FF" +
        "93100100003D3C0000000F856FFFFFFF4C89F7BE3C000000FF9398010000E95CFFFFFF4C89F7BE3D000000FF93280000" +
        "003D400000000F8543FFFFFF4C89F7BE40000000FF9318030000E930FFFFFF4C89F7BE41000000FF93A0010000E91DFF" +
        "FFFF4883C408415E5BC3";

    /// <summary>One program's baseline emission, compared with the bytes retained for it.</summary>
    /// <remarks>
    /// <b>THE RETAINED BYTES WERE DECODED BY HAND AGAINST THE TEMPLATE LIST THE FORM WAS SPECIFIED
    /// WITH, NOT AGAINST THE TABLE.</b> The closure rows prove the emitter and the table agree with
    /// each other; this row pins both to the written specification - the prologue, the dispatch, the
    /// one-landing leaf, the defect block, each instruction's call and tail, and the epilogue - so
    /// that an encoder and a table that drifted together still move a row.
    /// </remarks>
    private static (string, bool, string) BaselineGolden(JsX64Abi abi, string expected)
    {
        var name = "`" + abi.Name + "` emits the retained baseline bytes for a property read, a branch and a throw";
        var compiled = CompileWide(("golden", GoldenSource, null), abi.Name);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (name, false, Refusal(compiled));
        }

        if (!NativeLifecycle.TryReadEmitted(compiled.Artifact, out var code, out var symbols, out var refusal))
        {
            return (name, false, refusal);
        }

        var written = System.Convert.ToHexString(code);

        return string.Equals(written, expected, System.StringComparison.Ordinal)
            ? (name, true, code.Length + " bytes, " + symbols.Length + " symbols")
            : (name, false, "the emitted bytes are not the retained ones; the emission reads " + written);
    }

    /// <summary>
    /// One program's two emissions decode, each against its own table, to one sequence of templates
    /// with one set of non-register operands.
    /// </summary>
    /// <remarks>
    /// <b>THE CONVENTIONS DIFFER IN REGISTERS AND A RESERVATION, AND IN NOTHING THE PROGRAM
    /// DECIDES.</b> Every template has the same length under both, so the branch displacements, the
    /// program counters, the handler slots and the symbol offsets must all agree; a difference in any
    /// of them is an emitter that consulted the convention for something other than the registers.
    /// </remarks>
    private static (string, bool, string) TheTwoConventionsEmitOneTemplateSequence()
    {
        const string Name = "the two conventions' baseline emissions are one template sequence";
        var compared = 0;

        foreach (var program in WidePrograms)
        {
            var left = CompileWide(program, JsNativeBackends.X64Windows);
            var right = CompileWide(program, JsNativeBackends.X64SystemV);

            if (left.Artifact is null || right.Artifact is null ||
                !NativeLifecycle.TryReadEmitted(left.Artifact, out var windowsCode, out var windowsSymbols, out _) ||
                !NativeLifecycle.TryReadEmitted(right.Artifact, out var systemVCode, out var systemVSymbols, out _))
            {
                return (Name, false, program.Name + ": did not emit under both conventions");
            }

            if (!TryDecode(JsNativeArchitecture.X64Windows, windowsCode, out var windows, out var why) ||
                !TryDecode(JsNativeArchitecture.X64SystemV, systemVCode, out var systemV, out why))
            {
                return (Name, false, program.Name + ": " + why);
            }

            if (!System.Linq.Enumerable.SequenceEqual(windows, systemV) ||
                !System.Linq.Enumerable.SequenceEqual(windowsSymbols, systemVSymbols))
            {
                var at = 0;

                while (at < windows.Count && at < systemV.Count && windows[at] == systemV[at])
                {
                    at++;
                }

                return (
                    Name,
                    false,
                    program.Name + ": the sequences part at instantiation " + at + " (" +
                    (at < windows.Count ? windows[at] : "end") + " against " +
                    (at < systemV.Count ? systemV[at] : "end") + ")");
            }

            compared += windows.Count;
        }

        return (Name, true, compared + " instantiations compared across " + WidePrograms.Length + " programs");
    }

    /// <summary>Decodes a baseline image linearly into template names with their operand values.</summary>
    private static bool TryDecode(
        JsNativeArchitecture architecture,
        byte[] code,
        out System.Collections.Generic.List<string> sequence,
        out string detail)
    {
        sequence = [];
        var table = JsNativeTemplates.For(architecture, JsNativeTier.Baseline);
        var at = 0;
        var afterReturn = false;

        while (at < code.Length)
        {
            if (afterReturn && code[at] == JsNativeTemplates.X64PaddingByte)
            {
                at++;
                continue;
            }

            JsNativeTemplate? found = null;

            foreach (var template in table)
            {
                if (at + template.Length > code.Length)
                {
                    continue;
                }

                var matches = true;

                for (var index = 0; index < template.Length && matches; index++)
                {
                    matches = (code[at + index] & template.Mask[index]) == template.Fixed[index];
                }

                if (matches)
                {
                    found = template;
                    break;
                }
            }

            if (found is null)
            {
                detail = architecture + ": no baseline template matches at " + at;
                return false;
            }

            var text = new System.Text.StringBuilder(found.Text);

            foreach (var field in found.Fields)
            {
                text.Append(' ').Append(
                    System.BitConverter.ToInt32(code, at + (field.BitOffset / 8))
                        .ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            sequence.Add(text.ToString());
            afterReturn = found.IsReturn;
            at += found.Length;
        }

        detail = string.Empty;
        return true;
    }

    /// <summary>
    /// Every wide program's artifact verifies under a descriptor that re-emits with the backend that
    /// wrote it.
    /// </summary>
    /// <remarks>
    /// <b>THE VERIFIER BUILDS ITS OWN IMAGE - CODE, FUNCTION ROWS, REGIONS AND TIER - FROM WHAT IT
    /// READ, AND REQUIRES THE SAME BYTES.</b> An emitter that read anything the artifact does not
    /// carry, or iterated something in an order the artifact does not fix, would fail here for a
    /// correct artifact; so would a verifier that chose the tier from anything but the manifest.
    /// </remarks>
    private static (string, bool, string) BaselineReEmission(JsX64Abi abi)
    {
        var name = "every wide baseline artifact `" + abi.Name + "` writes re-emits byte for byte at verification";
        var artifacts = new System.Collections.Generic.List<(string, byte[])>();

        foreach (var program in WidePrograms)
        {
            var compiled = CompileWide(program, abi.Name);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return (name, false, program.Name + ": " + Refusal(compiled));
            }

            artifacts.Add((program.Name, compiled.Artifact));
        }

        var answers = VerifyAll(
            JavaScriptProfile.DescriptorReEmittingWith(new JsX64Backend(abi), EverySurface()),
            artifacts);

        foreach (var (label, accepted, detail) in answers)
        {
            if (!accepted)
            {
                return (name, false, label + ": " + detail);
            }
        }

        return (name, answers.Count == WidePrograms.Length, answers.Count + " artifacts verified with re-emission");
    }

    /// <summary>
    /// A payload whose one handler call was moved to another defined opcode's slot passes the scan
    /// and fails re-emission.
    /// </summary>
    /// <remarks>
    /// <b>THIS IS THE GAP THE SCAN CANNOT CLOSE, SHOWN TO BE CLOSED BY THE LAYER THAT CAN.</b> The
    /// slot is admitted - it is eight times a defined opcode - so an execution-only image accepts the
    /// payload and relies on the handler's own opcode check at run time; an image with the backend
    /// re-emits and refuses it at verification.
    /// </remarks>
    private static (string, bool, string) AReEmittingVerifierRefusesAHandlerSwappedInThePayload()
    {
        const string Name = "a re-emitting verifier refuses a baseline payload whose handler call was swapped";
        var compiled = CompileWide(WidePrograms[0], JsNativeBackends.X64Windows);

        if (compiled.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(compiled.Artifact, out var code, out _, out _))
        {
            return (Name, false, "the program did not emit: " + Refusal(compiled));
        }

        var artifact = (byte[])compiled.Artifact.Clone();
        var blob = System.MemoryExtensions.IndexOf(
            System.MemoryExtensions.AsSpan(artifact),
            System.MemoryExtensions.AsSpan(code, 0, System.Math.Min(64, code.Length)));

        var swapped = -1;

        for (var at = 0; blob >= 0 && at + 6 <= code.Length; at++)
        {
            if (code[at] != 0xFF || code[at + 1] != 0x93)
            {
                continue;
            }

            var slot = System.BitConverter.ToInt32(code, at + 2);
            var replacement = slot == (int)JsOpcode.LoadNull * 8 ? (int)JsOpcode.LoadTrue * 8 : (int)JsOpcode.LoadNull * 8;
            System.BitConverter.TryWriteBytes(System.MemoryExtensions.AsSpan(artifact, blob + at + 2), replacement);
            swapped = at;
            break;
        }

        if (swapped < 0)
        {
            return (Name, false, "the emission carries no handler call to swap");
        }

        var admitting = VerifyAll(JavaScriptProfile.Descriptor, [("admitting", artifact)])[0];

        var reEmitting = VerifyAll(
            JavaScriptProfile.DescriptorReEmittingWith(new JsX64Backend(JsX64Abi.Windows), EverySurface()),
            [("re-emitting", artifact)])[0];

        return (
            Name,
            admitting.Accepted && !reEmitting.Accepted &&
                reEmitting.Detail.EndsWith(" code " + (int)JavaScriptDiagnosticCode.MalformedNativeSection, System.StringComparison.Ordinal),
            "the call at " + swapped + " swapped: without an emitter " +
                (admitting.Accepted ? "verified" : "refused (" + admitting.Detail + ")") +
                "; with one " + (reEmitting.Accepted ? "VERIFIED" : "refused, " + reEmitting.Detail));
    }

    /// <summary>The arm64 backend refuses the wide manifest by name rather than emitting anything.</summary>
    private static (string, bool, string) TheArm64BackendRefusesTheWideManifest()
    {
        const string Name = "the arm64 backend refuses the wide manifest's native form by name";
        var compiled = CompileWide(WidePrograms[0], JsNativeBackends.Arm64);
        var refusal = Refusal(compiled);

        return (
            Name,
            !compiled.Succeeded && refusal.Contains("no arm64 emitter", System.StringComparison.Ordinal),
            compiled.Succeeded ? "the arm64 backend emitted a wide artifact" : refusal);
    }

    /// <summary>Every surface this build implements, as the descriptor doors take them.</summary>
    private static VmFeatureManifestId[] EverySurface()
    {
        var surfaces = new VmFeatureManifestId[JsSurfaces.All.Length];

        for (var index = 0; index < surfaces.Length; index++)
        {
            surfaces[index] = VmFeatureManifestId.Parse(JsSurfaces.All[index]);
        }

        return surfaces;
    }

    /// <summary>Verifies wide artifacts under one descriptor, one runtime for all of them.</summary>
    private static System.Collections.Generic.List<(string Label, bool Accepted, string Detail)> VerifyAll(
        VmProfileDescriptor profile,
        System.Collections.Generic.List<(string Label, byte[] Artifact)> artifacts)
    {
        var answers = new System.Collections.Generic.List<(string, bool, string)>();
        var catalog = VmCatalog.CreateBuilder().Add(profile).Build();
        var ceilings = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(catalog, new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,

            // THE MODULE SURFACE ASKS FOR A RESOLVER BEFORE IT READS A GRAPH, and these rows verify a
            // module artifact; the graph was bundled by key, so confirming every request is the rule.
            capabilities:
            [
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.ResolveCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        return VmHostCallOutcome.Completed;
                    }),
            ]));

        if (!created.TryGetRuntime(out var runtime))
        {
            foreach (var (label, _) in artifacts)
            {
                answers.Add((label, false, "the runtime refused creation: " + created.Outcome));
            }

            return answers;
        }

        using (runtime)
        {
            foreach (var (label, artifact) in artifacts)
            {
                var descriptor = new VmArtifactDescriptor(
                    JavaScriptProfile.Id,
                    JsFormat.FormatVersion,
                    JavaScriptProfile.WideManifest,
                    default,
                    VmCallerIdentity.FromCanonicalIdentity("com.example.broiler.slice-compiler"));

                var verified = runtime.Verify(in descriptor, artifact, System.Threading.CancellationToken.None);

                answers.Add((
                    label,
                    verified.TryGetArtifact(out _),
                    verified.Outcome + "/" + verified.Reason + " code " + verified.Diagnostics.ProfileDiagnosticCode));
            }
        }

        return answers;
    }

    /// <summary>
    /// Hand-built baseline payloads, each legal machine code, each refused by the clause it names.
    /// </summary>
    /// <remarks>
    /// <b>THE FRAME-SHAPE ROWS ARE WHOLE UNITS WITH ONE THING WRONG</b>: a prologue and an epilogue
    /// that are both correct, and the one instruction or branch between or around them that breaks a
    /// clause. The control beside them branches to the epilogue's first instruction - the landing
    /// every real exit uses - and is accepted, so the refusals are not satisfied by a scan that
    /// refuses every branch toward the epilogue.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> BaselineRefusals()
    {
        byte[] prologue = [0x53, 0x41, 0x56, 0x48, 0x83, 0xEC, 0x28, 0x49, 0x89, 0xCE, 0x49, 0x8B, 0x1E, 0x89, 0xD0];
        byte[] epilogue = [0x48, 0x83, 0xC4, 0x28, 0x41, 0x5E, 0x5B, 0xC3];

        var undefinedByte = 0x0B;

        while (JsOpcodes.IsDefined((byte)undefinedByte))
        {
            undefinedByte++;
        }

        byte[] Unit(params byte[] body) => [.. prologue, .. body, .. epilogue];

        byte[] Call(int slot) =>
        [
            0x4C, 0x89, 0xF1,
            0xBA, 0x00, 0x00, 0x00, 0x00,
            0xFF, 0x93, (byte)slot, (byte)(slot >> 8), 0x00, 0x00,
        ];

        var rows = new System.Collections.Generic.List<(string, bool, string)>
        {
            RefusedBaseline("`call rax` in a baseline unit", JsNativeTier.Baseline, Unit(0xFF, 0xD0), JsNativeScanOutcome.NoTemplate),
            RefusedBaseline(
                "a handler call through the slot of undefined byte 0x" + undefinedByte.ToString("X2", System.Globalization.CultureInfo.InvariantCulture),
                JsNativeTier.Baseline,
                Unit(Call(undefinedByte * 8)),
                JsNativeScanOutcome.OperandOutOfRange),
            RefusedBaseline("a handler call between two slots", JsNativeTier.Baseline, Unit(Call(0x84)), JsNativeScanOutcome.OperandOutOfRange),
            RefusedBaseline("`mov eax, 2`, a status no unit materialises", JsNativeTier.Baseline, Unit(0xB8, 0x02, 0x00, 0x00, 0x00), JsNativeScanOutcome.OperandOutOfRange),
            RefusedBaseline(
                "a baseline unit with no prologue",
                JsNativeTier.Baseline,
                [.. Call((int)JsOpcode.LoadUndefined * 8), .. Call((int)JsOpcode.Pop * 8), .. Call((int)JsOpcode.ReturnUndefined * 8), 0xB8, 0xFD, 0xFF, 0xFF, 0xFF, .. epilogue],
                JsNativeScanOutcome.FrameSequenceMalformed),
            RefusedBaseline("`push rbx` in the middle of a baseline unit", JsNativeTier.Baseline, Unit(0x53), JsNativeScanOutcome.FrameSequenceMalformed),
            RefusedBaseline("a second `ret` in a baseline unit", JsNativeTier.Baseline, Unit(0xC3), JsNativeScanOutcome.FrameSequenceMalformed),
            RefusedBaseline("a `jmp` to the epilogue's `pop rbx`", JsNativeTier.Baseline, Unit(0xE9, 0x06, 0x00, 0x00, 0x00), JsNativeScanOutcome.BranchIntoFrameSequence),
            RefusedBaseline("a `jmp` back into the prologue", JsNativeTier.Baseline, Unit(0xE9, 0xEF, 0xFF, 0xFF, 0xFF), JsNativeScanOutcome.BranchIntoFrameSequence),
        };

        // THE CONTROL: the same jump, to the epilogue's first instruction.
        var control = JsNativeScan.Scan(
            JsNativeArchitecture.X64Windows, JsNativeTier.Baseline, Unit(0xE9, 0x00, 0x00, 0x00, 0x00), [new JsNativeSymbolRow(0, 0)], 16);

        rows.Add((
            "the baseline scan accepts a `jmp` to the epilogue's first instruction",
            control.Accepted,
            control.Accepted ? "accepted" : control.Outcome + " at " + control.Offset + ": " + control.Reason));

        // A NUMERIC UNIT UNDER WIDE, AND A BASELINE UNIT UNDER NUMERIC: each table refuses the
        // other's emission at its first instruction the two do not share.
        var numeric = JsCompiler.Compile(
            [new JsScriptUnit("numeric.js", "1 + 2;", SliceParseOptions.Script)],
            [],
            new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Native, JsNativeBackends.X64Windows));

        var wide = CompileWide(WidePrograms[0], JsNativeBackends.X64Windows);

        if (numeric.Artifact is null || wide.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(numeric.Artifact, out var numericCode, out var numericSymbols, out _) ||
            !NativeLifecycle.TryReadEmitted(wide.Artifact, out var wideCode, out var wideSymbols, out _))
        {
            rows.Add(("the two tiers refuse each other's emissions", false, "a program did not emit"));
            return rows;
        }

        rows.Add(Crossed("a numeric emission judged as baseline", numericCode, numericSymbols, JsNativeTier.Baseline));
        rows.Add(Crossed("a baseline emission judged as numeric", wideCode, wideSymbols, JsNativeTier.Numeric));
        return rows;
    }

    /// <summary>Requires the scan to refuse one Windows x64 payload under one tier with one outcome.</summary>
    private static (string, bool, string) RefusedBaseline(
        string name, JsNativeTier tier, byte[] code, JsNativeScanOutcome expected)
    {
        var label = "the " + tier + " scan refuses " + name;
        var result = JsNativeScan.Scan(JsNativeArchitecture.X64Windows, tier, code, [new JsNativeSymbolRow(0, 0)], 16);

        return (
            label,
            result.Outcome == expected,
            result.Outcome == expected
                ? result.Outcome + " at offset " + result.Offset + ": " + result.Reason
                : "expected " + expected + " and the scan answered " + result.Outcome + " at " + result.Offset + ": " + result.Reason);
    }

    /// <summary>Requires a real emission scanned under the other tier to be refused as no template.</summary>
    private static (string, bool, string) Crossed(
        string name, byte[] code, JsNativeSymbolRow[] symbols, JsNativeTier tier)
    {
        var label = "the template scan refuses " + name;
        var result = JsNativeScan.Scan(JsNativeArchitecture.X64Windows, tier, code, symbols, 16);

        return (
            label,
            result.Outcome == JsNativeScanOutcome.NoTemplate,
            result.Outcome + " at offset " + result.Offset + ": " + result.Reason);
    }

    /// <summary>The answers a wide artifact could already reach, stated as unchanged.</summary>
    /// <remarks>
    /// <b>A WIDE ARTIFACT'S PAYLOAD WAS JUDGED AGAINST THE NUMERIC TABLES UNTIL THE BASELINE TIER
    /// EXISTED, AND THREE RETAINED ANSWERS DEPEND ON WHAT THAT JUDGEMENT SAID.</b> Four zero bytes
    /// declared as x86-64 are still no template at their first byte; the arm64 table is the one a
    /// wide arm64 payload is still judged against, so four zero bytes are still refused there and a
    /// single <c>ret</c> is still accepted. The verification rows above run the same three through
    /// the whole verifier, whose tier is now read off the manifest.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> UnchangedAnswers()
    {
        var zeros = JsNativeScan.Scan(
            JsNativeArchitecture.X64Windows, JsNativeTier.Baseline, [0x00, 0x00, 0x00, 0x00], [new JsNativeSymbolRow(0, 0)], 16);

        var armZeros = JsNativeScan.Scan(
            JsNativeArchitecture.Arm64, JsNativeTier.Baseline, [0x00, 0x00, 0x00, 0x00], [new JsNativeSymbolRow(0, 0)], 4);

        var armReturn = JsNativeScan.Scan(
            JsNativeArchitecture.Arm64, JsNativeTier.Baseline, [0xC0, 0x03, 0x5F, 0xD6], [new JsNativeSymbolRow(0, 0)], 4);

        return
        [
            (
                "unchanged: an x86-64 payload of four zero bytes under the wide manifest is no template at its first byte",
                zeros.Outcome == JsNativeScanOutcome.NoTemplate && zeros.Offset == 0,
                zeros.Outcome + " at offset " + zeros.Offset),
            (
                "unchanged: an arm64 payload of four zero bytes under the wide manifest is no template",
                armZeros.Outcome == JsNativeScanOutcome.NoTemplate && armZeros.Offset == 0,
                armZeros.Outcome + " at offset " + armZeros.Offset),
            (
                "unchanged: a one-instruction arm64 payload under the wide manifest is accepted",
                armReturn.Accepted,
                armReturn.Accepted ? "accepted against the arm64 table" : armReturn.Outcome + ": " + armReturn.Reason),
        ];
    }

    // ---- everything the backends emit, the scanner accepts ---------------------------------

    /// <summary>
    /// The numeric programs the closure rows compile: one shape each rather than one sample each.
    /// </summary>
    /// <remarks>
    /// <b>EACH ONE REACHES A FAMILY OF TEMPLATES NO OTHER ONE DOES.</b> Straight-line arithmetic
    /// reaches the prologue, the slab loads and stores and the four arithmetic instructions; the
    /// block scope reaches the local slots; the conditional reaches a forward branch and a join;
    /// the loop reaches a BACKWARD branch and the fuel charge that pays for it; the float mix
    /// reaches the constant slab; the negation reaches the one sixty-four-bit pattern a unit
    /// exclusive-ors with; the comparisons reach every condition code both encoders spell; and the
    /// two function programs reach the direct call, the argument staging and the propagate epilogue
    /// - which the arm64 backend refuses, so those two rows count for x86-64 alone.
    /// </remarks>
    private static readonly (string Name, string Source)[] Programs =
    [
        ("straight-line arithmetic", "1 + 2 * 3 - 4 / 5;"),
        ("a block scope and repeated local reads", "{ let a = 3; let b = 4; a * a + b * b; }"),
        ("a forward branch and a join", "{ let a = 1; if (a < 2) { a = a * 10; } else { a = a - 1; } a; }"),
        ("a backward branch and a loop", "{ let t = 0; let i = 0; while (i < 10) { t = t + i * i; i = i + 1; } t; }"),
        ("nested loops", "{ let t = 0; let i = 0; while (i < 4) { let j = 0; while (j < 4) { t = t + 1; j = j + 1; } i = i + 1; } t; }"),
        ("a float mix and a constant pool", "{ let x = 1.5; let y = 2.25; x * y - x / y + 0.125; }"),
        ("a negation and a unary not", "{ let a = 1; let b = 2; if (!(a >= b)) { a = -a + b; } a; }"),
        ("every comparison", "{ let a = 1; let b = 2; let n = 0; if (a < b) { n = n + 1; } if (a <= b) { n = n + 1; } if (a > b) { n = n + 1; } if (a >= b) { n = n + 1; } if (a === b) { n = n + 1; } if (a !== b) { n = n + 1; } n; }"),
        ("a declared function and a direct call", "function f(a) { return a * 2; }\nfunction g(a, b) { return f(a) + b; }\ng(3, 4);"),
        ("a recursive function", "function fact(n) { if (n < 2) { return 1; } return n * fact(n - 1); }\nfact(5);"),
        ("a realm binding read and written", "let g = 1; g = g + 2; g;"),
        ("a function body that can complete with no value", "function f(a) { if (a > 0) { return a; } } f(1) + 0;"),
        ("the four relational comparisons", "{ let a = 1; let b = 2; let n = 0; if (a < b) { n = n + 1; } if (a <= b) { n = n + 1; } if (a > b) { n = n + 1; } if (a >= b) { n = n + 1; } n; }"),
        ("a program body whose completion is undefined", "{ let a = 1; }"),
    ];

    /// <summary>
    /// Compiles every program this backend admits, reads the emitted bytes out of the artifact and
    /// requires the scan to accept all of them.
    /// </summary>
    /// <remarks>
    /// <b>A REFUSED PROGRAM IS NOT A FAILING ROW AND IT IS NOT A SILENT PASS EITHER.</b> Each
    /// backend compiles a closed subset of the numeric manifest and refuses the rest by name - arm64
    /// refuses every program that declares a function, because a realm binding is an object graph a
    /// frame of doubles cannot hold - so a refusal is counted and reported rather than judged. What
    /// IS judged is that the count of scanned images is not zero: a row that scanned nothing would
    /// be a green tick for a table nobody exercised.
    /// </remarks>
    private static (string, bool, string) Closure(
        string backend,
        JsNativeArchitecture architecture,
        uint alignment,
        System.Collections.Generic.HashSet<string> instantiated)
    {
        var name = "everything `" + backend + "` emits, the template scan accepts";
        var scanned = 0;
        var refused = 0;
        var bytes = 0;

        foreach (var (label, source) in Programs)
        {
            var compiled = JsCompiler.Compile(
                [new JsScriptUnit("closure.js", source, SliceParseOptions.Script)],
                [],
                new JsCompileRequest(JsFeatureManifest.Numeric, JsOutputForm.Native, backend));

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                refused++;
                continue;
            }

            if (!NativeLifecycle.TryReadEmitted(
                compiled.Artifact, out var code, out var symbols, out var refusal))
            {
                return (name, false, label + ": " + refusal);
            }

            var result = JsNativeScan.Scan(architecture, code, symbols, alignment, instantiated);

            if (!result.Accepted)
            {
                return (
                    name,
                    false,
                    label + ": the scan refused " + code.Length + " emitted bytes with " +
                    result.Outcome + " at offset " + result.Offset + " - " + result.Reason +
                    "; the bytes there read " + Around(code, result.Offset));
            }

            scanned++;
            bytes += code.Length;
        }

        return (
            name,
            scanned > 0,
            scanned == 0
                ? "every one of the " + Programs.Length + " programs was refused by this backend, " +
                    "so this row scanned nothing and establishes nothing"
                : scanned + " of " + Programs.Length + " programs emitted and scanned, " + bytes +
                    " bytes in all, " + refused + " refused by the backend before any byte was " +
                    "emitted");
    }

    /// <summary>
    /// Every template of this architecture's table is instantiated by at least one of the programs.
    /// </summary>
    /// <remarks>
    /// <b>THE CLOSURE ROW IS ONLY AS STRONG AS THE PROGRAMS IT COMPILES, AND THIS IS THE ROW THAT
    /// SAYS SO.</b> A template no program instantiates is a template the encoders could have
    /// contradicted with every row above still green: the direction "everything the backends emit,
    /// the scanner accepts" is a statement about emissions, and an emission nobody produced proves
    /// nothing about the entry that would have decoded it. So this row names the uncovered
    /// templates rather than reporting a number, and it passes only where the list is empty or
    /// every name on it is one the backends cannot reach for a reason stated below.
    /// </remarks>
    private static (string, bool, string) Coverage(
        string backend,
        JsNativeArchitecture architecture,
        System.Collections.Generic.HashSet<string> instantiated,
        params string[] outOfReach)
    {
        var name = "every template `" + backend + "` has is reached by one of these programs";
        var uncovered = new System.Collections.Generic.List<string>();
        var table = JsNativeTemplates.For(architecture);

        foreach (var template in table)
        {
            if (!instantiated.Contains(template.Text) &&
                System.Array.IndexOf(outOfReach, template.Text) < 0)
            {
                uncovered.Add(template.Text);
            }
        }

        var excepted = outOfReach.Length == 0
            ? string.Empty
            : ", with " + outOfReach.Length + " named out of reach of this manifest (" +
                string.Join(", ", outOfReach) + ")";

        return (
            name,
            uncovered.Count == 0,
            uncovered.Count == 0
                ? instantiated.Count + " of " + table.Length + " templates instantiated" + excepted
                : instantiated.Count + " of " + table.Length +
                    " templates instantiated; unreached and unaccounted for: " +
                    string.Join(", ", uncovered));
    }

    /// <summary>The sixteen bytes around an offset, so a failing row names what it met.</summary>
    /// <remarks>
    /// <b>A row that says only "no template matched" sends its reader back to the encoder with
    /// nothing to look for.</b> The bytes are what a reader decodes by hand against the table, and
    /// printing them is the difference between a failure that can be acted on and one that can only
    /// be reproduced.
    /// </remarks>
    private static string Around(byte[] code, uint offset)
    {
        var from = (int)System.Math.Max(0, (long)offset - 4);
        var to = (int)System.Math.Min(code.Length, offset + 12);
        var text = new System.Text.StringBuilder();

        for (var index = from; index < to; index++)
        {
            text.Append(code[index].ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
        }

        return text.ToString() + " (from " + from + ")";
    }

    /// <summary>
    /// The three constants the table restates from the calling-convention table agree with it.
    /// </summary>
    /// <remarks>
    /// <b>THE TABLE CANNOT READ THE CONVENTION TABLE AND THAT IS WHY THIS ROW EXISTS.</b> The
    /// templates live in the format assembly, which references nothing, and the convention table
    /// lives beside the backends; a verifier that had to reference the lowering to know where a
    /// frame pointer is spilled would be a verifier an execution-only image could not carry. So the
    /// three numbers are written twice, and this row - which is in the one place that can see both -
    /// is what stops the two copies parting company.
    /// </remarks>
    private static (string, bool, string) TheTableAgreesWithTheConventionTable()
    {
        const string Name = "the template table's frame constants are the convention table's";
        var failures = new System.Collections.Generic.List<string>();

        foreach (var abi in JsX64Abi.Rows)
        {
            if (JsNativeTemplates.X64FrameBytes(abi.Architecture) != abi.FrameBytes)
            {
                failures.Add(
                    abi.Name + ": the table reserves " +
                    JsNativeTemplates.X64FrameBytes(abi.Architecture) +
                    " bytes and the convention reserves " + abi.FrameBytes);
            }

            if (JsNativeTemplates.X64FramePointerSlot(abi.Architecture) != abi.FramePointerSlot)
            {
                failures.Add(
                    abi.Name + ": the table spills at " +
                    JsNativeTemplates.X64FramePointerSlot(abi.Architecture) +
                    " and the convention spills at " + abi.FramePointerSlot);
            }

            if (JsNativeTemplates.X64FramePointerRegister(abi.Architecture) !=
                (int)abi.FramePointerRegister)
            {
                failures.Add(
                    abi.Name + ": the table names register " +
                    JsNativeTemplates.X64FramePointerRegister(abi.Architecture) +
                    " and the convention names " + (int)abi.FramePointerRegister);
            }
        }

        return (
            Name,
            failures.Count == 0,
            failures.Count == 0
                ? "both conventions agree about the frame size, the spill slot and the register " +
                    "the frame pointer arrives in"
                : string.Join("; ", failures));
    }

    // ---- what the scanner refuses ------------------------------------------------------------

    /// <summary>
    /// Byte sequences that are legal machine code, that no backend of this build emits, and that
    /// the scan refuses by name.
    /// </summary>
    /// <remarks>
    /// <b>EVERY ROW HERE IS A SEQUENCE A PROCESSOR WOULD HAVE EXECUTED.</b> That is the point: the
    /// architecture check asks which machine the bytes are for and not whether they are code, so
    /// each of these would have passed every structural clause the verifier had before the scan
    /// existed - and the first of them did, was armed, and killed a process.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Refusals() =>
    [
        // FOUR ZERO BYTES DECLARED AS x86-64. They decode as `add [rax], al` twice, which faults on
        // a null base and stores through any other. This is the payload JSC-208 records.
        Refused(
            FourZeroBytes,
            JsNativeArchitecture.X64Windows,
            [0x00, 0x00, 0x00, 0x00],
            JsNativeScanOutcome.NoTemplate),

        // FOUR ZERO BYTES DECLARED AS arm64, which decode as a permanently undefined instruction.
        Refused(
            "an arm64 payload of four zero bytes",
            JsNativeArchitecture.Arm64,
            [0x00, 0x00, 0x00, 0x00],
            JsNativeScanOutcome.NoTemplate),

        // AN INDIRECT CALL THROUGH A REGISTER. The encoder can spell it and the backends emit none:
        // every call in an emitted artifact is a direct branch to a code unit of the same artifact,
        // which is what keeps a value a guest computed from ever becoming an address. The table has
        // no template for it, so a payload carrying one is refused.
        Refused(
            "an indirect call through a register",
            JsNativeArchitecture.X64Windows,
            [0xFF, 0xD0, 0xC3],
            JsNativeScanOutcome.NoTemplate),

        // A SYSCALL, which is two bytes and reaches the operating system.
        Refused(
            "a syscall instruction",
            JsNativeArchitecture.X64Windows,
            [0x0F, 0x05, 0xC3],
            JsNativeScanOutcome.NoTemplate),

        // A FRAME DISPLACEMENT THAT NAMES NO FIELD. `mov rax, [r11+33]` is a legal encoding of an
        // instruction the backend emits at five other displacements, and 33 reads the second half
        // of one field and the first half of another.
        Refused(
            "a frame displacement between two declared fields",
            JsNativeArchitecture.X64Windows,
            [0x49, 0x8B, 0x83, 0x21, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.OperandOutOfRange),

        // A SLAB DISPLACEMENT OFF THE EIGHT-BYTE GRID. `mov rdx, [rbx+4]` reads four bytes of one
        // double and four of the next.
        Refused(
            "a slab displacement off the eight-byte grid",
            JsNativeArchitecture.X64Windows,
            [0x48, 0x8B, 0x93, 0x04, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.OperandOutOfRange),

        // AN IMMEDIATE NO UNIT MATERIALISES. `mov rax, 1` is the bail-out code this profile does not
        // have, and one is exactly the value the refused design would have used.
        Refused(
            "a materialised immediate no unit answers with",
            JsNativeArchitecture.X64Windows,
            [0x48, 0xB8, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.OperandOutOfRange),

        // A UNIT THAT DOES NOT END IN A RETURN, so control would run off its last instruction into
        // whatever the allocator left after it.
        Refused(
            "a unit that runs off its own end",
            JsNativeArchitecture.X64Windows,
            [0x53, 0x5B],
            JsNativeScanOutcome.UnitDoesNotEndInAReturn),

        // PADDING THAT IS NOT PADDING: a no-op run after the return with something else inside it.
        Refused(
            "a byte hidden in a unit's alignment padding",
            JsNativeArchitecture.X64Windows,
            [0xC3, 0x90, 0x00],
            JsNativeScanOutcome.PaddingNotAlignment),

        // A BRANCH OUT OF THE UNIT. `jmp .+16` from a one-unit payload lands past the blob.
        Refused(
            "a branch that leaves the unit",
            JsNativeArchitecture.X64Windows,
            [0xE9, 0x10, 0x00, 0x00, 0x00, 0xC3],
            JsNativeScanOutcome.BranchLeavesUnit),

        // A BRANCH INTO THE MIDDLE OF AN INSTRUCTION. The displacement is legal, the target is
        // inside the unit, and it lands on the second byte of a ten-byte move.
        Refused(
            "a branch into the middle of an instruction",
            JsNativeArchitecture.X64Windows,
            [
                0xE9, 0x01, 0x00, 0x00, 0x00,
                0x48, 0xB8, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF8, 0x7F,
                0xC3,
            ],
            JsNativeScanOutcome.BranchIntoInstruction),
    ];

    /// <summary>Requires the scan to refuse one payload with one named outcome.</summary>
    private static (string, bool, string) Refused(
        string name, JsNativeArchitecture architecture, byte[] code, JsNativeScanOutcome expected)
    {
        var label = "the template scan refuses " + name;

        var result = JsNativeScan.Scan(
            architecture, code, [new JsNativeSymbolRow(0, 0)], alignment: 16);

        if (result.Accepted)
        {
            return (label, false, "the scan accepted " + code.Length + " bytes it should refuse");
        }

        return (
            label,
            result.Outcome == expected,
            result.Outcome == expected
                ? result.Outcome + " at offset " + result.Offset + ": " + result.Reason
                : "expected " + expected + " and the scan answered " + result.Outcome + " at " +
                    result.Offset + ": " + result.Reason);
    }

    // ---- and the mutations of a real emission --------------------------------------------------

    /// <summary>
    /// A real emitted image, mutated one byte at a time, is refused where the mutation matters.
    /// </summary>
    /// <remarks>
    /// <b>A HAND-WRITTEN REFUSAL ROW PROVES THE CLAUSE AND A MUTATED IMAGE PROVES THE SCAN REACHES
    /// IT.</b> The rows above are byte strings somebody wrote; these start from bytes a backend of
    /// this build actually emitted for a real program and change one thing, which is the shape a
    /// corrupted or forged payload has. What they cannot do is tell a wrong instruction from a right
    /// one: a mutation that turns one admitted template into another admitted template is accepted,
    /// and that is the scan's limit rather than this row's.
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Mutations()
    {
        var rows = new System.Collections.Generic.List<(string, bool, string)>();

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit(
                "mutate.js",
                "{ let t = 0; let i = 0; while (i < 10) { t = t + i; i = i + 1; } t; }",
                SliceParseOptions.Script)],
            [],
            new JsCompileRequest(
                JsFeatureManifest.Numeric, JsOutputForm.Native, JsNativeBackends.X64Windows));

        var code = System.Array.Empty<byte>();
        var symbols = System.Array.Empty<JsNativeSymbolRow>();
        var refusal = "the loop program was refused by the front end or the backend";

        if (!compiled.Succeeded || compiled.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(compiled.Artifact, out code, out symbols, out refusal))
        {
            rows.Add((
                "a real emitted image can be mutated",
                false,
                "the loop program did not compile to machine code: " + refusal));

            return rows;
        }

        rows.Add((
            "the unmutated image is accepted",
            JsNativeScan.Scan(JsNativeArchitecture.X64Windows, code, symbols, 16).Accepted,
            code.Length + " bytes emitted for one code unit"));

        // THE LAST BYTE OF A UNIT IS ITS RETURN, and an image one byte shorter is a unit that runs
        // off its own end.
        var truncated = code[..^1];

        rows.Add(Mutated(
            "an image with its last return removed",
            truncated,
            symbols,
            JsNativeScanOutcome.UnitDoesNotEndInAReturn));

        // ONE BIT OF ONE BRANCH DISPLACEMENT. The scan walks the image looking for the first branch
        // it can shift by one byte, which turns a target that is an instruction into a target that
        // is inside one.
        var shifted = (byte[])code.Clone();
        var moved = -1;

        for (var at = 0; at + 5 < shifted.Length; at++)
        {
            if (shifted[at] != 0xE9)
            {
                continue;
            }

            shifted[at + 1]++;
            moved = at;
            break;
        }

        rows.Add(moved < 0
            ? ("an image with one branch displacement moved", false, "the image carries no `jmp rel32`")
            : Mutated(
                "an image with one branch displacement moved",
                shifted,
                symbols,
                JsNativeScanOutcome.BranchIntoInstruction));

        return rows;
    }

    /// <summary>
    /// An image emitted for one calling convention and declared as the other is refused.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE TWO CONVENTIONS DIFFER IN THREE CONSTANTS AND THOSE THREE CONSTANTS ARE IN THE FIXED
    /// BYTES OF THE TABLE, so a mislabelled payload is a payload whose prologue matches no
    /// template.</b> A System V unit spills its frame pointer with <c>mov [rsp+0], rdi</c> and a
    /// Windows one with <c>mov [rsp+32], rcx</c>; neither is an instantiation of the other's table.
    /// The bytes are perfectly good machine code and the architecture value is one this build
    /// names, which is exactly the shape of payload the framing layer cannot see through.
    /// </para>
    /// <para>
    /// <b>WHAT THIS IS NOT is the defect the core retains as a fixture.</b> That one was a CALLER
    /// using the wrong convention to call code that was correct - a property of a call site and not
    /// of an artifact - and no scan of a payload can see it. What this row catches is the artifact
    /// that says it was built for a machine it was not built for.
    /// </para>
    /// </remarks>
    private static (string, bool, string) APayloadDeclaredForTheOtherConvention()
    {
        const string Name =
            "the template scan refuses an image emitted for the other calling convention";

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("convention.js", "1 + 2;", SliceParseOptions.Script)],
            [],
            new JsCompileRequest(
                JsFeatureManifest.Numeric, JsOutputForm.Native, JsNativeBackends.X64SystemV));

        var code = System.Array.Empty<byte>();
        var symbols = System.Array.Empty<JsNativeSymbolRow>();
        var refusal = "the front end or the backend refused the program";

        if (!compiled.Succeeded || compiled.Artifact is null ||
            !NativeLifecycle.TryReadEmitted(compiled.Artifact, out code, out symbols, out refusal))
        {
            return (Name, false, "the program did not compile for System V: " + refusal);
        }

        var honest = JsNativeScan.Scan(JsNativeArchitecture.X64SystemV, code, symbols, 16);
        var mislabelled = JsNativeScan.Scan(JsNativeArchitecture.X64Windows, code, symbols, 16);

        return (
            Name,
            honest.Accepted && mislabelled.Outcome == JsNativeScanOutcome.NoTemplate,
            !honest.Accepted
                ? "the System V image was refused by its OWN table: " + honest.Outcome + " at " +
                    honest.Offset
                : "the same " + code.Length + " bytes: accepted as System V, and as Windows " +
                    mislabelled.Outcome + " at offset " + mislabelled.Offset);
    }

    /// <summary>Requires a mutated image to be refused with a named outcome.</summary>
    private static (string, bool, string) Mutated(
        string name, byte[] code, JsNativeSymbolRow[] symbols, JsNativeScanOutcome expected)
    {
        var label = "the template scan refuses " + name;
        var result = JsNativeScan.Scan(JsNativeArchitecture.X64Windows, code, symbols, 16);

        return (
            label,
            result.Outcome == expected,
            result.Outcome == expected
                ? result.Outcome + " at offset " + result.Offset + ": " + result.Reason
                : "expected " + expected + " and the scan answered " + result.Outcome + " at " +
                    result.Offset + ": " + result.Reason);
    }

    // ---- and the same payload through the whole verifier ---------------------------------------

    /// <summary>
    /// The four zero bytes, in a whole artifact, refused by the verifier before anything is armed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE ROWS ABOVE CALL THE SCAN AND THESE CALL THE VERIFIER, and the difference is the whole
    /// reason for running both.</b> A scan that refuses a byte string proves the table; it proves
    /// nothing about whether the verifier reaches the scan, which arm it reaches it from, or what a
    /// composition is told when it does. These build an artifact of the shape the retained corpus
    /// carries - a native surface, an emitted-code section, a symbol for its one code unit - and
    /// hand it to a runtime built from the ORDINARY descriptor, which carries no emitter. That is
    /// the execution-only position exactly: no re-emission is possible, the framing is impeccable,
    /// and the payload is not code.
    /// </para>
    /// <para>
    /// <b>NOTHING HERE IS INSTANTIATED, and the accepting row names an architecture no host
    /// arms.</b> Verification is where the claim is; instantiating a hand-written payload is how
    /// the accident this lane exists for happened, so the control carries an A64 return that this
    /// build refuses to instantiate on every machine it runs on, and it stops at the verifier in
    /// any case.
    /// </para>
    /// </remarks>
    private static System.Collections.Generic.List<(string, bool, string)> Verification() =>
    [
        // THE PAYLOAD JSC-208 RECORDS, DECLARED FOR THE ARCHITECTURE THAT ARMED IT.
        Verified(
            "an x86-64 payload of four zero bytes is refused at verification",
            [0x00, 0x00, 0x00, 0x00],
            JsNativeArchitecture.X64Windows,
            refused: true),

        // THE SAME FOUR BYTES DECLARED FOR arm64, so the refusal is a fact about the bytes rather
        // than about the calling convention the host happens to have.
        Verified(
            "an arm64 payload of four zero bytes is refused at verification",
            [0x00, 0x00, 0x00, 0x00],
            JsNativeArchitecture.Arm64,
            refused: true),

        // AND THE CONTROL. Two refusals are satisfied by a verifier that refuses every artifact
        // carrying these sections at all, which is the easiest wrong implementation to write: this
        // one carries a single A64 return, which is one template instantiation, and it verifies.
        Verified(
            "a payload that is one instruction still verifies",
            [0xC0, 0x03, 0x5F, 0xD6],
            JsNativeArchitecture.Arm64,
            refused: false),
    ];

    /// <summary>Verifies one hand-built artifact and requires the answer this row expects.</summary>
    private static (string, bool, string) Verified(
        string name, byte[] payload, JsNativeArchitecture architecture, bool refused)
    {
        var artifact = Artifact(payload, architecture);
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        var ceilings = System.Collections.Immutable.ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(catalog, new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: []));

        if (!created.TryGetRuntime(out var runtime))
        {
            return (name, false, "the runtime refused creation: " + created.Outcome);
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity("com.example.broiler.slice-compiler"));

            var verified = runtime.Verify(
                in descriptor, artifact, System.Threading.CancellationToken.None);

            var code = verified.Diagnostics.ProfileDiagnosticCode;
            var accepted = verified.TryGetArtifact(out _);

            if (!refused)
            {
                return (
                    name,
                    accepted,
                    accepted
                        ? "verified, and NOTHING INSTANTIATED IT: the row is about the verifier's " +
                            "answer, and the architecture it names is one no host of this build arms"
                        : "the verifier refused it: " + verified.Outcome + "/" + verified.Reason +
                            " code " + code);
            }

            return (
                name,
                !accepted && code == (int)JavaScriptDiagnosticCode.NativePayloadNotTemplateClosed,
                accepted
                    ? "THE VERIFIER ACCEPTED FOUR BYTES THAT ARE NOT CODE"
                    : verified.Outcome + "/" + verified.Reason + " code " + code);
        }
    }

    /// <summary>
    /// A version-2 artifact that is well formed in every respect except the payload it is handed.
    /// </summary>
    /// <remarks>
    /// <b>It is built here rather than borrowed from the retained corpus, because the corpus is a
    /// retained artefact and a check is not.</b> The shape is the corpus's own - one code unit, one
    /// constant, one entry point, the native surface declared, an emitted-code section and a symbol
    /// row for the unit - so what these rows exercise is the arrangement a real native artifact has
    /// rather than a special case built to be refused.
    /// </remarks>
    private static byte[] Artifact(byte[] payload, JsNativeArchitecture architecture)
    {
        byte[] body = [(byte)JsOpcode.LoadConstant, 0x00, 0x00, (byte)JsOpcode.Return];

        JavaScriptArtifactWriter.Section[] sections =
        [
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 4)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants([JsArtifactWriter.NumberConstant(1)])),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, body),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([("main", 0u)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
                JsArtifactWriter.Positions([(0u, 1u, 1u)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
                JsArtifactWriter.Functions(
                [
                    new JsFunctionRow(
                        0, 0, 1, 16, 0, (uint)body.Length,
                        (uint)JsFormat.FunctionFlags.ProgramBody),
                ])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces,
                JsArtifactWriter.Surfaces([JsSurfaces.Native])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeCode,
                JsArtifactWriter.NativeCode(
                    (uint)architecture,
                    backendSemanticVersion: 0,
                    codeAlignment: 1,
                    payload,
                    declaredByteLength: null)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.NativeSymbols,
                JsArtifactWriter.NativeSymbols([new JsNativeSymbolRow(0, 0)])),
        ];

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections);
    }
}
