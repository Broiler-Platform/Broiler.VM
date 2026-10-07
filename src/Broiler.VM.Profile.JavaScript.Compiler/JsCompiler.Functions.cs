// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           0
// Human-reviewed:   0/10
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- units ---------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0514C5
    // Broiler-Human:        PENDING
    private int CompileProgram(JsProgramNode program, bool forceStrict)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;
        var outerMethod = insideMethod;
        var outerDerived = insideDerivedConstructor;
        var outerFunction = insideFunction;
        var outerExits = exits;

        insideMethod = false;
        insideDerivedConstructor = false;
        insideFunction = false;
        exits = [];

        strict = program.IsStrict || forceStrict;
        var index = units.Count;
        buffer = new UnitBuffer(0, JsFormat.FunctionFlags.ProgramBody | Strictness());
        units.Add(buffer);
        scope = new Scope(ScopeKind.Program, null);
        blockDepth = 0;

        // SLOT ZERO OF A SCRIPT IS ITS COMPLETION VALUE. A script's value is the value of its last
        // value-producing statement, which is what a person running one file at a prompt expects to
        // see and what the host prints. Keeping it in a slot rather than on the operand stack is
        // what lets every statement leave the stack empty.
        var completion = scope.Declare("#completion", constant: false);

        Emit(JsOpcode.LoadUndefined);
        EmitScoped(JsOpcode.InitialiseScoped, 0, completion);

        var declared = HoistProgram(program.Body);

        // THE ROW IS WRITTEN ONLY FOR A BODY THAT DECLARES SOMETHING: one that declares nothing has
        // no check to run, and the `Function` constructor's assembled source is one of those.
        if (declared.LexicalNames.Length + declared.VarNames.Length +
            declared.FunctionNames.Length + declared.AnnexBNames.Length != 0)
        {
            scriptDeclarations.Add((
                index, declared.LexicalNames, declared.VarNames, declared.FunctionNames, declared.AnnexBNames));
        }

        // A DIRECTIVE IS AN EXPRESSION STATEMENT, AND ITS STRING IS A COMPLETION VALUE. The parser
        // keeps the prologue apart from the body, so without this `eval("'a'")` answered undefined
        // where the language answers `a`. Only the last directive can be the value: the ones before
        // it are overwritten by it, and a later statement that yields a value overwrites it in turn.
        if (program.Directives.Count != 0)
        {
            CompileExpression(program.Directives[^1]);
            EmitScoped(JsOpcode.InitialiseScoped, 0, completion);
        }

        CompileStatements(program.Body, completion);

        EmitScoped(JsOpcode.LoadScoped, 0, completion);
        Emit(JsOpcode.Return);

        buffer.SlotCount = scope.SlotCount;
        buffer = outerBuffer;
        scope = outerScope;
        blockDepth = outerDepth;
        strict = outerStrict;
        insideMethod = outerMethod;
        insideDerivedConstructor = outerDerived;
        insideFunction = outerFunction;
        exits = outerExits;
        return index;
    }

    /// <summary>Lowers one function body into a code unit of its own.</summary>
    /// <param name="function">The body.</param>
    /// <param name="extra">
    /// Flags the CALLER knows and the body does not - that this unit is a class constructor, and
    /// whether it is a derived one. Nothing in a constructor's own text says either.
    /// </param>
    /// <param name="isMethod">
    /// Whether this is a method, which decides two unrelated things: that <c>super</c> resolves
    /// inside it, and that it is <b>not a constructor</b>. <c>new (C.prototype.m)()</c> is a
    /// TypeError in the language, and the flag is what makes it one here.
    /// </param>
    /// <param name="isDerived">Whether this is the constructor of a class with a heritage.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4C96B3
    // Broiler-Human:        PENDING
    private int CompileFunction(
        JsFunctionNode function,
        JsFormat.FunctionFlags extra = JsFormat.FunctionFlags.None,
        bool isMethod = false,
        bool isDerived = false,
        bool isStaticBlock = false,
        bool isFieldInitialiser = false)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;
        var outerMethod = insideMethod;
        var outerDerived = insideDerivedConstructor;
        var outerFunction = insideFunction;
        var outerField = insideFieldInitialiser;
        var outerFieldValueName = fieldValueName;
        fieldValueName = isFieldInitialiser ? function.Name : null;
        var outerExits = exits;
        exits = [];

        // A FIELD INITIALISER IS A METHOD WHOSE `arguments` NAMES NOTHING, and an arrow inside one
        // inherits that as it inherits `super`. Any other function has its own `arguments`, and its
        // own `super` too, so neither reaches past it (JSeal V14).
        if (!function.IsArrow)
        {
            insideFieldInitialiser = isFieldInitialiser;
        }

        // EVERY NESTED BODY CLEARS THIS AND AN ARROW CLEARS IT TOO, which is the one thing that
        // separates it from the three flags below. `this` and `super` reach outward from an arrow;
        // a `return` does not - it returns from the arrow - so an arrow written inside a static
        // block may `return` even though the block may not.
        var outerStaticBlock = insideStaticBlock;
        insideStaticBlock = isStaticBlock;

        // AN ARROW INHERITS EVERY ONE OF THESE AND ANY OTHER FUNCTION RESETS THEM. That single
        // difference is what `super`, `this` and `new.target` mean inside an arrow.
        if (!function.IsArrow)
        {
            insideMethod = isMethod;
            insideDerivedConstructor = isDerived;
            insideFunction = true;
        }

        strict = strict || function.IsStrict;

        var flags = Strictness() | JsFormat.FunctionFlags.Constructible;

        if (function.IsArrow)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Arrow;
        }
        else if (isMethod)
        {
            flags &= ~JsFormat.FunctionFlags.Constructible;
        }

        flags |= extra;

        // THE PARAMETER LIST DECIDES WHO BINDS THE PARAMETERS, and the two answers are not
        // interchangeable. A simple list is copied into slots by the frame, which costs nothing;
        // anything else has to run code - a default is an expression, a rest parameter is an Array,
        // a pattern is a destructuring - so the unit binds its own and declares that it does.
        //
        // A REPEATED NAME STAYS ON THE FIRST PATH, because the list is still simple and a simple
        // sloppy list is the one whose `arguments` object is MAPPED. `function f(a, a) {}` is an
        // ordinary sloppy-mode program every engine runs; it used to take the binding prologue,
        // because two parameters sharing one name declared ONE slot and the frame's copy loop was
        // told to fill two - and the verifier refused the artifact. Each earlier occurrence now
        // gets a slot of its own under a name no source can write (see the declarations below), so
        // the list is still one slot per position, the frame's copy still fills them left to
        // right, the name resolves to the LAST occurrence as the language says it does, and the
        // runtime can map position `i` to slot `i` without being told which names repeat
        // *(corrected: JSeal V05)*.
        var simple = IsSimpleParameterList(function.Parameters);

        if (!simple)
        {
            flags |= JsFormat.FunctionFlags.BindsParameters;
        }

        // THE ARITY A FUNCTION ROW CARRIES IS BOUNDED BY THE CALL CEILING, and the verifier holds
        // every row to it, because for a simple list it is also the count the frame copies a
        // call's arguments into. A list with more than 255 parameters before its first default or
        // rest was lowered anyway, and the verifier refused the artifact this host produced, which
        // is the exit the host reserves for its own defects. It is refused here instead, naming the
        // ceiling, at the function *(corrected: JSC-240)*.
        if (ExpectedArgumentCount(function.Parameters) is > (int)JsFormat.CeilingCallArguments and var arity)
        {
            Refuse(
                function.Span,
                SliceSourceDiagnosticCode.TooManyLocals,
                "a function of this format declares at most 255 parameters before its first default " +
                "or rest parameter - the count the frame copies a call's arguments into - and this " +
                "one declares " + arity.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        // A GENERATOR IS NOT A CONSTRUCTOR, and dropping the bit is what makes `new g()` a type
        // error from the ordinary construction path rather than a special case somewhere in it.
        if (function.IsGenerator)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Generator;
        }

        // AN ASYNC FUNCTION IS NOT A CONSTRUCTOR EITHER, and the arrow bit is left alone. That is
        // the one place the two suspension kinds differ in this method: a generator arrow is not a
        // production of the grammar, and an async arrow is - so the flags are set independently and
        // the verifier's own consistency check admits `Async | Arrow` while refusing
        // `Generator | Arrow`.
        if (function.IsAsync)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Async;
        }

        var index = units.Count;
        var name = function.Name.Length == 0 ? (ushort)0 : (ushort)(InternedName(function.Name) + 1);

        buffer = new UnitBuffer(name, flags)
        {
            ParameterCount = ExpectedArgumentCount(function.Parameters),
        };

        units.Add(buffer);
        scope = new Scope(ScopeKind.Function, outerScope);
        blockDepth = 0;

        // EVERY PARAMETER NAME IS DECLARED BEFORE ANY DEFAULT IS EMITTED, and the slots start
        // empty. That is what makes `function f(a = b, b) {}` the ReferenceError the specification
        // says it is: `b` resolves to a binding that exists and has not been initialised, rather
        // than to a global of the same name that happens to be lying around.
        //
        // A SIMPLE LIST DECLARES ONE SLOT PER POSITION, IN ORDER, and a name that occurs again
        // later in the list gives its earlier position a hidden `#shadowed` slot instead. That is
        // what the frame's copy loop and the mapped `arguments` object both rely on: slot `i` is
        // parameter `i`. An earlier duplicate is unreachable by name - the language resolves the
        // name to the last one - so its hidden slot is written by the copy and by nothing else,
        // and aliasing `arguments[i]` to it is indistinguishable from not mapping that index,
        // which is what `CreateMappedArgumentsObject` does for it.
        for (var position = 0; position < function.Parameters.Count; position++)
        {
            var parameter = function.Parameters[position];

            if (simple && IsRedeclaredLater(function.Parameters, position))
            {
                _ = scope.Declare("#shadowed" + scope.SlotCount, constant: false);
                continue;
            }

            DeclarePatternNames(parameter.Target, constant: false);
        }

        // A PARAMETER THAT BINDS `arguments` IS THE BINDING, AND THE OBJECT IS NOT CREATED AT ALL.
        // `Scope.Declare` answers with the existing slot when the name is already declared, so a
        // function whose formal parameter list contains `arguments` used to have its third or
        // fourth actual overwritten by the arguments object between entry and the first statement -
        // the parameter's value was simply gone. The specification says the same thing from the
        // other end: function declaration instantiation sets `argumentsObjectNeeded` to false when
        // `arguments` is one of the parameter names - and PARAMETER NAMES ARE BOUND NAMES, so
        // `function f({ arguments }) {}` shadows the object exactly as `function f(arguments) {}`
        // does. A `var arguments` or a function declaration of that name is NOT this case: each is
        // initialised after the object is, which is the order the specification asks for and the
        // order the code below already produces *(corrected: JSC-82)*.
        var parameterNames = new System.Collections.Generic.List<string>();

        foreach (var parameter in function.Parameters)
        {
            CollectPatternNames(parameter.Target, parameterNames);
        }

        var shadowedByParameter = false;

        foreach (var parameterName in parameterNames)
        {
            if (string.Equals(parameterName, "arguments", System.StringComparison.Ordinal))
            {
                shadowedByParameter = true;
                break;
            }
        }

        var usesArguments = !function.IsArrow && !shadowedByParameter && UsesArguments(function);

        if (usesArguments)
        {
            buffer.Flags |= JsFormat.FunctionFlags.UsesArguments;
            var slot = scope.Declare("arguments", constant: false);
            Emit(JsOpcode.NewArguments);
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }

        var bodyEval = !strict && MentionsEval(function.Body);

        // THE BODY GETS A VARIABLE ENVIRONMENT OF ITS OWN WHEN ANYTHING COULD TELL (JSeal V15-finish,
        // FunctionDeclarationInstantiation step 28). The specification gives one to every body whose
        // parameter list has expressions, and no other; this lowering pushes it only where the
        // difference is observable - a parameter list that makes a closure, in its own syntax or through a source
        // it evaluates, and a sloppy body whose direct eval may declare a parameter's name, which
        // the specification makes a new binding of the body's environment. Everywhere else the one
        // record keeps the parameters and the body, and a call pays for no second record.
        scope.SeparateBody = !simple && ParametersHaveExpressions(function.Parameters) &&
            (ParametersMakeClosures(function.Parameters) || bodyEval);

        if (!simple)
        {
            // A SLOPPY PARAMETER LIST THAT MAY CALL `eval` DIRECTLY may introduce bindings by name
            // before the body runs, and every later parameter and the whole body search for them
            // (JSeal V15, JSD-0026 step 9). What the parameters' record binds so far - the
            // parameters, and the `arguments` object beside them - is the part of the function its
            // evaluations see, and collide with.
            if (!strict && ParametersMention(function.Parameters, "eval"))
            {
                scope.EvalVariables = true;
            }

            scope.ParameterLimit = scope.SlotCount;
            parameterScopes.Add(scope);
            CompileParameters(function.Parameters);
            parameterScopes.Remove(scope);

            // THE SEAM IS EMITTED FOR A GENERATOR AND FOR NOTHING ELSE, because a generator is the
            // only unit whose parameter list and whose body run at two different times. The
            // language binds the list at the CALL - `EvaluateGeneratorBody` and
            // `EvaluateAsyncGeneratorBody` both perform function declaration instantiation before
            // they create the object - and everything above this instruction is that binding.
            //
            // AN ASYNC FUNCTION IS DELIBERATELY NOT ON THE LIST, and it is the one arm of the three
            // that was already right. Its promise is made BEFORE the binding runs, so a default
            // that throws rejects the promise the call already answered with rather than throwing
            // at the call - which is what this engine's async arm already does by running the body
            // synchronously to its first `await`. Marking the seam there would have bought nothing
            // and cost one dispatch on every async call *(corrected: JSC-159)*.
            if (function.IsGenerator)
            {
                Emit(JsOpcode.EnterBody);
            }
        }

        Scope? parameters = null;

        if (scope.SeparateBody)
        {
            parameters = scope;
            scope = new Scope(ScopeKind.Body, parameters);
            blockDepth++;
            var at = buffer.Code.Count;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((at + 1, scope));
        }

        // A SLOPPY FUNCTION WHOSE OWN CODE MAY CALL `eval` DIRECTLY MAY GAIN BINDINGS BY NAME, and
        // every free name in its body and in the functions nested in it is then searched for one, as
        // a `with` object is searched (JSeal V15, JSD-0026 step 6). The mark is set after the
        // parameter list, whose closures see the parameters' record and not the body's variable
        // environment an evaluation declares into - the body's own record when it has one; a
        // mention of `eval` that is not a direct call costs the search and nothing else. An arrow
        // is its own variable environment and marks itself.
        if (bodyEval)
        {
            scope.EvalVariables = true;
        }

        HoistFunction(function.Body, parameters);

        // EVERYTHING DECLARED FROM HERE ON AT THE FUNCTION'S TOP LEVEL IS LEXICAL - a `let`, a
        // `const`, a `class` - or a temporary nobody can name. The specification gives a sloppy
        // function's top-level lexical declarations a record of their own inside the variable one;
        // this lowering keeps them in the same record and says which half a name is in.
        scope.LexicalFrom = scope.SlotCount;

        CompileDisposing(function.Body, -1);
        Emit(JsOpcode.ReturnUndefined);

        // The unit's own record is the parameters' one; the body's, when it has one, is sized by
        // the instruction that pushes it.
        buffer.SlotCount = (parameters ?? scope).SlotCount;

        if (buffer.SlotCount > MaximumSlots || scope.SlotCount > MaximumSlots)
        {
            Refuse(function.Span, SliceSourceDiagnosticCode.TooManyLocals, "a function declares too many bindings");
        }

        buffer = outerBuffer;
        scope = outerScope;
        blockDepth = outerDepth;
        strict = outerStrict;
        insideMethod = outerMethod;
        insideDerivedConstructor = outerDerived;
        insideFunction = outerFunction;
        insideStaticBlock = outerStaticBlock;
        insideFieldInitialiser = outerField;
        fieldValueName = outerFieldValueName;
        exits = outerExits;
        RecordSource(index, function.SourceStart, function.SourceEnd);
        return index;
    }

    /// <summary>Records the source text a compilation is about to lower, and answers its index.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=CC3D91
    // Broiler-Human:        PENDING
    private int AddSource(string text)
    {
        if (!request.KeepsSourceText)
        {
            return -1;
        }

        sources.Add(text);
        return sources.Count - 1;
    }

    /// <summary>
    /// Records that <paramref name="unit"/> was defined from the span of the current source between
    /// <paramref name="start"/> and <paramref name="end"/> (JSD-0037).
    /// </summary>
    /// <remarks>
    /// A later record for the same unit replaces an earlier one, which is how a class constructor
    /// written as a method gets the whole class's text rather than its own.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B0DAEE
    // Broiler-Human:        PENDING
    private void RecordSource(int unit, int start, int end)
    {
        if (currentSource < 0 || start < 0 || end <= start || end > sources[currentSource].Length)
        {
            return;
        }

        sourceSpans[unit] = (currentSource, start, end);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3FFD44
    // Broiler-Human:        PENDING
    private JsFormat.FunctionFlags Strictness() =>
        strict ? JsFormat.FunctionFlags.Strict : JsFormat.FunctionFlags.None;

    // ---- parameters ----------------------------------------------------------------------------

    /// <summary>Whether every parameter is one name with no initialiser and no <c>...</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8D6266
    // Broiler-Human:        PENDING
    private static bool IsSimpleParameterList(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.IsRest || parameter.Default is not null ||
                parameter.Target is not JsTargetPattern { Target: JsIdentifier })
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Whether a later parameter of a simple list binds the same name as this one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CAAB07
    // Broiler-Human:        PENDING
    private static bool IsRedeclaredLater(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters, int position)
    {
        if (parameters[position].Target is not JsTargetPattern { Target: JsIdentifier earlier })
        {
            return false;
        }

        for (var later = position + 1; later < parameters.Count; later++)
        {
            if (parameters[later].Target is JsTargetPattern { Target: JsIdentifier name } &&
                string.Equals(earlier.Name, name.Name, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// What the function reports as its <c>length</c>: the parameters before the first one that
    /// has a default or is a rest.
    /// </summary>
    /// <remarks>
    /// <b>A pattern with no initialiser COUNTS and a rest parameter never does.</b>
    /// <c>function f({a}, b = 1, c) {}</c> reports 1, not 3 and not 2 - the count stops at the first
    /// default and everything after it, default or not, is invisible to <c>length</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AC49D2
    // Broiler-Human:        PENDING
    private static int ExpectedArgumentCount(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        var count = 0;

        foreach (var parameter in parameters)
        {
            if (parameter.IsRest || parameter.Default is not null)
            {
                break;
            }

            count++;
        }

        return count;
    }

    /// <summary>Emits the prologue that binds a parameter list the frame will not copy.</summary>
    /// <remarks>
    /// <para>
    /// Left to right, because a later default may read an earlier parameter and the specification
    /// says it sees the bound value rather than the argument.
    /// </para>
    /// <para>
    /// <b>ONE DECLARED DIVERGENCE: a repeated name in a list that is not simple is an EARLY ERROR
    /// the language raises and this front end does not.</b> <c>function f(a, a = 1) {}</c> is a
    /// <c>SyntaxError</c> everywhere else and runs here, binding <c>a</c> twice and keeping the
    /// second. The direction is the safe one - a negative test asserting that error scores a
    /// failure rather than a false pass - and the check belongs with the other early errors rather
    /// than bolted onto the lowering.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=ED8285
    // Broiler-Human:        PENDING
    private void CompileParameters(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        for (var at = 0; at < parameters.Count; at++)
        {
            var parameter = parameters[at];
            Position(parameter.Span);

            if (parameter.IsRest)
            {
                // A REST PARAMETER IS AN ARRAY AND NEVER `arguments`. It is dense, it has
                // Array.prototype, and a caller that passed nothing gives it length zero rather
                // than leaving it undefined.
                Emit(JsOpcode.RestArguments, (ushort)at);
            }
            else
            {
                Emit(JsOpcode.LoadArgument, (ushort)at);

                // A DEFAULT RUNS WHEN THE ARGUMENT IS `undefined`, WHICH IS NOT THE SAME AS ABSENT.
                // `f(undefined)` takes the default and `f(null)` does not, so the test is against
                // the value rather than against the argument count.
                ApplyDefault(parameter.Default, InferredFrom(parameter.Target));
            }

            BindPattern(parameter.Target, BindMode.Initialise);
        }
    }
}
