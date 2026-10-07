// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           0
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  3/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    /// <summary>
    /// Lowers a value whose name the language takes from what it is being bound to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>const C = class { };</c> gives the class the name <c>C</c>, and <c>var f = function () {}</c>
    /// gives the function the name <c>f</c>, neither of which the text contains. It is done here
    /// rather than in the executor because the name is baked into the code unit, and a unit belongs
    /// to exactly one syntactic site - so the name a site infers is the name every closure over that
    /// site has.
    /// </para>
    /// <para>
    /// <b>The closure is emitted here rather than through <see cref="CompileFunctionExpression"/>,
    /// and the difference is a binding.</b> A function expression with a name in its TEXT binds that
    /// name inside its own body: <c>var f = function g () { return g; }</c> can see <c>g</c>. An
    /// inferred name is not that - <c>var f = function () { f = 1; }</c> assigns the outer <c>f</c> -
    /// so the unit is named without the surrounding scope that a written name creates.
    /// </para>
    /// <para>
    /// <b>A name is inferred only where the language infers one.</b> The positions are the ones that
    /// call this: a declarator, an assignment to a name, a member of an object literal, and a default
    /// - for a parameter or inside a pattern. <c>o.p = function () {}</c> is NOT one of them and its
    /// function is anonymous, which is the case a reader is most likely to expect here and be wrong
    /// about.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D0EF9A
    // Broiler-Human:        PENDING
    private void CompileNamedValue(JsExpression value, string inferred)
    {
        if (inferred.Length == 0)
        {
            CompileExpression(value);
            return;
        }

        if (value is JsClassExpression anonymous && anonymous.Class.Name.Length == 0)
        {
            Position(value.Span);
            CompileClass(anonymous.Class, inferred);
            return;
        }

        if (value is JsFunctionExpression function && function.Function.Name.Length == 0)
        {
            Position(value.Span);
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function.Function with { Name = inferred }));
            return;
        }

        CompileExpression(value);
    }

    /// <summary>
    /// Whether <paramref name="value"/> is an anonymous function definition the executor can name
    /// after it is made, from a key known only at run time.
    /// </summary>
    /// <remarks>
    /// <b>A function or an arrow always can</b>: nothing runs between its creation and its naming.
    /// <b>A class can only when it has no static element</b>, because a static element runs while the
    /// class is defined and could read the name before it is given, and a static <c>name</c> member
    /// is the class's name and must not be overwritten. Such a class keeps the empty name, which the
    /// record of this change states (JSP-6, JSC-238).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=53D99F
    // Broiler-Human:        PENDING
    private static bool NamedAtRunTime(JsExpression value)
    {
        if (value is JsFunctionExpression function)
        {
            return function.Function.Name.Length == 0;
        }

        if (value is not JsClassExpression { Class.Name.Length: 0 } anonymous)
        {
            return false;
        }

        foreach (var member in anonymous.Class.Members)
        {
            if (member.IsStatic)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>The name a pattern's leaf infers for an anonymous default, or the empty string.</summary>
    /// <remarks>
    /// <b>Only a leaf that is one NAME infers anything.</b> <c>[a = function () {}]</c> names the
    /// function <c>a</c>; <c>[o.p = function () {}]</c> names nothing, and neither does a leaf that is
    /// itself a pattern - there is no one name for the default to take.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=27021F
    // Broiler-Human:        PENDING
    private static string InferredFrom(JsPattern target) =>
        target is JsTargetPattern { Target: JsIdentifier name } ? name.Name : string.Empty;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=873B5E
    // Broiler-Human:        PENDING
    private void CompileArray(JsArrayLiteral array)
    {
        var dense = true;

        foreach (var element in array.Elements)
        {
            if (element is null or JsSpreadElement)
            {
                dense = false;
                break;
            }
        }

        if (dense && array.Elements.Count <= 1024)
        {
            foreach (var element in array.Elements)
            {
                CompileExpression(element!);
            }

            Emit(JsOpcode.NewArray, (ushort)array.Elements.Count);
            return;
        }

        // EVERY OTHER SHAPE GOES THROUGH THE APPENDING PATH, holes and spreads alike, and the
        // sparse path this replaced was a JSC-81-shaped defect: it set `length` with SetProperty,
        // which pops the Array as well as the value and pushes only the value back, so `[1, , 3]`
        // left NOTHING on the operand stack and the verifier refused the whole artifact. That the
        // verifier caught it is the good outcome; that a plain elision reached it at all is what
        // this repair is for.
        CompileSpreadArray(array.Elements);
    }

    /// <summary>
    /// Builds an Array whose length is not known until it has been built.
    /// </summary>
    /// <remarks>
    /// <b>Every index after a spread is dynamic, so the whole literal switches to appending.</b>
    /// The index of <c>b</c> in <c>[a, ...xs, b]</c> depends on how many values <c>xs</c> yielded,
    /// and holes still have to count: <c>[...xs, , b]</c> leaves one behind. Emitting a constant
    /// index for the elements before the first spread and appending after it would have been
    /// half a lowering with a seam in the middle for no gain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=772EAF
    // Broiler-Human:        PENDING
    private void CompileSpreadArray(
        System.Collections.Generic.IReadOnlyList<JsExpression?> elements)
    {
        Emit(JsOpcode.NewArray, (ushort)0);
        var holes = 0;

        foreach (var element in elements)
        {
            if (element is null)
            {
                holes++;
                continue;
            }

            if (holes != 0)
            {
                Emit(JsOpcode.ArrayHoles, (ushort)holes);
                holes = 0;
            }

            if (element is JsSpreadElement spread)
            {
                CompileExpression(spread.Argument);
                Emit(JsOpcode.SpreadArray);
                continue;
            }

            CompileExpression(element);
            Emit(JsOpcode.ArrayAppend);
        }

        if (holes != 0)
        {
            // TRAILING HOLES ARE STILL LENGTH, and `ArrayHoles` is how they are said. The
            // sparse path this replaced set `length` with `SetProperty`, which pops the Array as
            // well as the value and pushes only the value back, so the literal left NOTHING on the
            // operand stack and the verifier refused the artifact *(corrected: JSC-81)*. One
            // instruction that grows the Array without defining an element has no such seam.
            Emit(JsOpcode.ArrayHoles, (ushort)holes);
        }
    }

    /// <summary>Whether an argument list carries a spread, so the count is not a constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3FEE9
    // Broiler-Human:        PENDING
    private static bool HasSpread(
        System.Collections.Generic.IReadOnlyList<JsExpression> arguments)
    {
        foreach (var argument in arguments)
        {
            if (argument is JsSpreadElement)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether an argument list travels as one Array rather than written out on the operand
    /// stack: when it carries a spread, so the count is not a constant, or when it is longer
    /// than the call instruction's one-byte count can say.
    /// </summary>
    /// <remarks>
    /// The count of 255 is the operand's width, not a limit the language states, and refusing a
    /// longer list as a construct outside the manifest named a reason that was not true
    /// *(corrected: JSC-240)*. The spread instructions take an Array they did not build by
    /// iterating, so a long list written out reaches the callee through the same path a spread
    /// does, with every argument evaluated once and in order.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=601796
    // Broiler-Human:        PENDING
    private static bool CarriesArgumentsInAnArray(
        System.Collections.Generic.IReadOnlyList<JsExpression> arguments) =>
        arguments.Count > 255 || HasSpread(arguments);

    /// <summary>Builds the one Array a spread call or a spread construction passes its arguments in.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5BBE2C
    // Broiler-Human:        PENDING
    private void CompileArgumentArray(
        System.Collections.Generic.IReadOnlyList<JsExpression> arguments)
    {
        Emit(JsOpcode.NewArray, (ushort)0);

        foreach (var argument in arguments)
        {
            if (argument is JsSpreadElement spread)
            {
                CompileExpression(spread.Argument);
                Emit(JsOpcode.SpreadArray);
                continue;
            }

            CompileExpression(argument);
            Emit(JsOpcode.ArrayAppend);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C1C2C5
    // Broiler-Human:        PENDING
    private void CompileObject(JsObjectLiteral literal)
    {
        Emit(JsOpcode.NewObject);

        var prototyped = false;

        foreach (var entry in literal.Entries)
        {
            // A `{ a = 1 }` THAT REACHED THE LOWERING WAS NEVER A PATTERN. The parser could not
            // tell an object literal from the cover grammar of an assignment pattern until the
            // token after the brace, so it marked the shape and let it through; whatever arrives
            // here is the half that no `=` followed, and it is a syntax error.
            if (entry.Cover)
            {
                Refuse(
                    entry.Span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`=` is not an object literal entry outside a destructuring assignment");

                continue;
            }

            if (entry.Kind == JsPropertyKind.Spread)
            {
                CompileExpression(entry.Value);
                Emit(JsOpcode.SpreadObject);
                continue;
            }

            // A METHOD AND A PROPERTY WHOSE VALUE IS A FUNCTION ARE DIFFERENT OBJECTS. `{ m() {} }`
            // makes a method - it has a home object, so `super` inside it resolves, and it is not
            // a constructor - and `{ m: function () {} }` makes an ordinary function. They were
            // one lowering until classes were admitted, which was invisible only because nothing
            // could ask a function for its home object.
            if (entry.IsMethod)
            {
                if (entry.Computed is null)
                {
                    Emit(JsOpcode.LoadConstant, StringConstant(entry.Key));
                }
                else
                {
                    CompileExpression(entry.Computed);
                }

                CompileMethodValue(entry.Value);

                Emit(
                    JsOpcode.DefineMethod,
                    MemberOperand(
                        entry.Kind switch
                        {
                            JsPropertyKind.Get => JsMethodKind.Get,
                            JsPropertyKind.Set => JsMethodKind.Set,
                            _ => JsMethodKind.Method,
                        },
                        enumerable: true));

                continue;
            }

            if (entry.Computed is not null)
            {
                // THE KEY IS CONVERTED BEFORE THE VALUE IS EVALUATED, which is where
                // `ComputedPropertyName` converts it: a key whose `toString` runs is observed before
                // anything the value does, where the definition converted it after (JSP-6, JSC-238).
                CompileExpression(entry.Computed);
                Emit(JsOpcode.ToPropertyKey);
                CompileExpression(entry.Value);

                // AN ANONYMOUS FUNCTION TAKES THE KEY AS ITS NAME, which is known only now, so the
                // definition names it as it names a method - without making it one (JSP-6,
                // JSC-238). Every other value is defined as it was.
                if (NamedAtRunTime(entry.Value))
                {
                    Emit(
                        JsOpcode.DefineMethod,
                        (byte)(JsOpcodes.MemberIsEnumerable | JsOpcodes.MemberIsNamedValue));

                    continue;
                }

                Emit(JsOpcode.DefineIndexed);
                continue;
            }

            // `__proto__: p` IS THE ONE MEMBER THAT IS NOT A MEMBER. The language spells it like a
            // property and gives it a different meaning - it sets the prototype - and the three
            // spellings that do NOT mean that are all excluded above or here: a computed key, a
            // method, and the shorthand. Writing it twice is a syntax error, because a literal that
            // set its prototype twice would have an order nobody could read off the source.
            if (!entry.Shorthand && string.Equals(entry.Key, "__proto__", System.StringComparison.Ordinal))
            {
                if (prototyped)
                {
                    Refuse(
                        entry.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "an object literal may set `__proto__` once");

                    continue;
                }

                prototyped = true;
                CompileExpression(entry.Value);
                Emit(JsOpcode.SetPrototypeLiteral);
                continue;
            }

            CompileNamedValue(entry.Value, entry.Key);
            Emit(JsOpcode.DefineField, InternedName(entry.Key));
        }
    }

    /// <summary>Emits a closure for a member written in method form.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E977A0
    // Broiler-Human:        PENDING
    private void CompileMethodValue(JsExpression value)
    {
        if (value is JsFunctionExpression method)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(method.Function, isMethod: true));
            return;
        }

        CompileExpression(value);
    }

    /// <summary>Lowers a regular-expression literal, refusing a pattern that is not one.</summary>
    /// <remarks>
    /// <para>
    /// <b>A LITERAL'S PATTERN IS CHECKED HERE AND NOT WHERE IT RUNS, because the language makes it
    /// an EARLY error.</b> <c>/(/ </c> is a program that does not parse, in the same way that
    /// <c>var = 1</c> is; a front end that emitted it and let the constructor refuse it at run time
    /// answered the right kind of error at the wrong time, and a program that never reached the
    /// literal - one behind a `false` branch, which is how the pinned suite writes these - was
    /// accepted outright.
    /// </para>
    /// <para>
    /// <b>The check is the matcher's own and not a second opinion.</b> The pattern grammar lives in
    /// the format assembly, which both this front end and the executor read, precisely so that the
    /// two cannot disagree about what a pattern is: a front end with its own idea of the grammar
    /// would either refuse a pattern the executor runs or emit one it cannot.
    /// </para>
    /// <para>
    /// <b>The flags are an early error too</b>, and they were not: <c>/a/x</c> and <c>/a/gg</c>
    /// behind a <c>false</c> branch were accepted, and <c>/a/v</c> - a flag the language has and
    /// this matcher does not - passed the front end and was refused only when the literal ran, as a
    /// SyntaxError from the constructor. Now a flag outside <c>dgimsuyv</c>, a repeated flag, or
    /// <c>u</c> with <c>v</c> is the language's SyntaxError here, and a well-formed <c>v</c> is an
    /// early SyntaxError whose message names the flag as unsupported (JSeal slice JSD-0031-later).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=92D102
    // Broiler-Human:        PENDING
    private void CompileRegExpLiteral(JsRegExpLiteral pattern)
    {
        // A refused literal is still lowered, so the stack shape does not depend on the refusal.
        if (!RefuseRegExpLiteralFlags(pattern))
        {
            try
            {
                _ = JsRegExpMatcher.Compile(
                    pattern.Pattern,
                    pattern.Flags.Contains('i', System.StringComparison.Ordinal),
                    pattern.Flags.Contains('m', System.StringComparison.Ordinal),
                    pattern.Flags.Contains('s', System.StringComparison.Ordinal),
                    pattern.Flags.Contains('u', System.StringComparison.Ordinal),
                    pattern.Flags.Contains('v', System.StringComparison.Ordinal));
            }
            catch (JsRegExpSyntaxError failure)
            {
                Refuse(pattern.Span, SliceSourceDiagnosticCode.UnexpectedToken, failure.Message);
            }
        }

        Emit(JsOpcode.LoadGlobal, InternedName("RegExp"));
        Emit(JsOpcode.LoadConstant, StringConstant(pattern.Pattern));
        Emit(JsOpcode.LoadConstant, StringConstant(pattern.Flags));
        Emit(JsOpcode.Construct, (byte)2);
    }

    /// <summary>
    /// Refuses a regular-expression literal whose flags the language refuses, and answers whether it
    /// did.
    /// </summary>
    /// <remarks>
    /// ES2026 IsValidRegularExpressionLiteral: the flags are drawn from <c>dgimsuyv</c>, none twice,
    /// and not <c>u</c> and <c>v</c> together. Those are SyntaxErrors of the language, and they are
    /// all this refuses. <b>A well-formed <c>v</c> is admitted</b> since phase F2 gave the matcher
    /// the flag's class syntax, set operations and properties of strings; until then it was refused
    /// here as unsupported, under the reading of decision JSD-0031 section 12 (JSC-262).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=51285E
    // Broiler-Human:        PENDING
    private bool RefuseRegExpLiteralFlags(JsRegExpLiteral pattern)
    {
        var flags = pattern.Flags;

        for (var at = 0; at < flags.Length; at++)
        {
            if ("dgimsuyv".IndexOf(flags[at], System.StringComparison.Ordinal) < 0 ||
                flags.IndexOf(flags[at], at + 1) >= 0)
            {
                Refuse(
                    pattern.Span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "Invalid regular expression flags: " + flags);

                return true;
            }
        }

        if (!flags.Contains('v', System.StringComparison.Ordinal))
        {
            return false;
        }

        if (flags.Contains('u', System.StringComparison.Ordinal))
        {
            Refuse(
                pattern.Span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "Invalid regular expression flags: " + flags + " (u and v may not be combined)");

            return true;
        }

        // A WELL-FORMED `v` IS ADMITTED since phase F2 gave the matcher its class syntax; until then
        // it was refused here as unsupported (JSC-262).
        return false;
    }

    // ---- templates -----------------------------------------------------------------------------

    /// <summary>Lowers a template literal to the concatenation it is.</summary>
    /// <remarks>
    /// <para>
    /// <b>A template is not sugar for <c>+</c>, and the difference is the coercion.</b> <c>`${x}`</c>
    /// is <c>ToString(x)</c>, which asks an object for <c>toString</c> FIRST; <c>"" + x</c> is
    /// addition, which asks for <c>valueOf</c> first. For <c>{ valueOf() { return 1 }, toString()
    /// { return "s" } }</c> the two answer differently, and every engine answers <c>"s"</c>. So the
    /// substitution goes through the realm's <c>String</c> rather than through <see
    /// cref="JsOpcode.Add"/>, and only the joining is addition - of two Strings, where addition has
    /// no coercion left to get wrong.
    /// </para>
    /// <para>
    /// <b>Except for a Symbol, which must throw, and <c>String</c> is the one function that does
    /// not.</b> <c>String(symbol)</c> is the language's single explicit Symbol-to-String coercion
    /// and answers <c>"Symbol(x)"</c>; a template must throw a <c>TypeError</c> instead. So the
    /// lowering tests the type first and, for a Symbol, reaches the throw the only way an opcode
    /// set without a <c>ToString</c> instruction can - by adding it to a String, which is exactly
    /// the implicit coercion the type refuses. The added value is never used: that path always
    /// throws, and it falls through to the call only so that the two paths meet the verifier at one
    /// height.
    /// </para>
    /// <para>
    /// <b>The declared cost is that this reads the global <c>String</c>.</b> A program that
    /// replaces it changes what a template produces, which the language does not allow. It is the
    /// same dependency a regular-expression literal already takes on the global <c>RegExp</c>, and
    /// it is taken for the same reason: this instruction set has no opcode for the operation, and a
    /// wrong coercion in every template is a worse answer than a coercion a hostile program can
    /// move.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=22497C
    // Broiler-Falsified-If: a substitution coerces through `valueOf` before `toString`, or a Symbol substitution does not throw
    // Broiler-Human:        PENDING
    private void CompileTemplate(JsTemplateLiteral template)
    {
        Emit(JsOpcode.LoadConstant, StringConstant(template.Cooked[0]!));

        for (var index = 0; index < template.Substitutions.Count; index++)
        {
            EmitToString(template.Substitutions[index]);
            Emit(JsOpcode.Add);

            var tail = template.Cooked[index + 1]!;

            if (tail.Length != 0)
            {
                Emit(JsOpcode.LoadConstant, StringConstant(tail));
                Emit(JsOpcode.Add);
            }
        }
    }

    /// <summary>Pushes <c>ToString</c> of one expression, throwing for a Symbol as the language does.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A802C8
    // Broiler-Falsified-If: the two paths reach the call at different operand-stack heights
    // Broiler-Human:        PENDING
    private void EmitToString(JsExpression value)
    {
        Emit(JsOpcode.LoadGlobal, InternedName("String"));
        Emit(JsOpcode.LoadUndefined);
        CompileExpression(value);

        var ordinary = NewLabel();
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.TypeOf);
        Emit(JsOpcode.LoadConstant, StringConstant("symbol"));
        Emit(JsOpcode.StrictEquals);
        Branch(JsOpcode.JumpIfFalse, ordinary);

        // The Symbol path. `Add` of a String and a Symbol is the implicit coercion the type
        // refuses, so this throws and never arrives below; it leaves one value behind on paper so
        // that the fall-through and the branch agree about the height.
        Emit(JsOpcode.LoadConstant, StringConstant(string.Empty));
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Add);

        Mark(ordinary);
        Emit(JsOpcode.Call, (byte)1);
    }

    /// <summary>Lowers a tagged template to the call it is.</summary>
    /// <remarks>
    /// <para>
    /// The tag is called with the strings object first and every substitution after it, in source
    /// order, and the template is never concatenated at all. The strings object is built by
    /// <see cref="EmitTemplateStrings"/>, which is where the identity rule lives.
    /// </para>
    /// <para>
    /// <b>Past 255 arguments the call takes them in one Array</b>, as a long call written out does,
    /// and the strings object is built from Arrays as well. Until 2026-10-03 a site with more than
    /// 254 substitutions was refused as a construct outside the manifest, which it is not
    /// *(corrected: JSC-240)*.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5BC1D6
    // Broiler-Human:        PENDING
    private void CompileTaggedTemplate(JsTaggedTemplate tagged)
    {
        EmitCallee(tagged.Tag);

        var count = tagged.Quasi.Substitutions.Count + 1;

        if (count > 255)
        {
            Emit(JsOpcode.NewArray, (ushort)0);
            EmitTemplateStrings(tagged.Quasi);
            Emit(JsOpcode.ArrayAppend);

            foreach (var substitution in tagged.Quasi.Substitutions)
            {
                CompileExpression(substitution);
                Emit(JsOpcode.ArrayAppend);
            }

            Emit(JsOpcode.CallSpread);
            return;
        }

        EmitTemplateStrings(tagged.Quasi);

        foreach (var substitution in tagged.Quasi.Substitutions)
        {
            CompileExpression(substitution);
        }

        Emit(JsOpcode.Call, (byte)count);
    }

    /// <summary>
    /// Pushes the strings object of one tagged-template CALL SITE, the same object every time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The identity is the specification's, and it is what a tag is usually written to
    /// exploit.</b> <c>function f() { return tag`x`; } f() === f()</c> compares the strings object
    /// of one site against itself and must answer true, because a tag that caches a compiled
    /// result against the strings object - which is the reason the rule exists - would otherwise
    /// recompile on every call and leak a cache entry each time.
    /// </para>
    /// <para>
    /// <b>The site is the <see cref="JsOpcode.GetTemplateObject"/> instruction itself</b>, and the
    /// realm's registry is the executor's, keyed by the loaded program and the instruction's offset.
    /// Until VM-FIX-D the cache was a property of the global object named by script ordinal, line and
    /// column: a guest could read and overwrite it, it showed up in the global object's own keys, and
    /// two separately evaluated programs with a template at the same position - two indirect
    /// <c>eval</c>s of one source - shared an object the language gives each its own.
    /// </para>
    /// <para>
    /// The chunks go on the stack as constants, cooked first, and the instruction discards them
    /// after the first evaluation. A site with more chunks than the one-byte count can say puts
    /// them in a cooked Array and a raw Array instead, for <see cref="JsOpcode.GetTemplateObjectWide"/>,
    /// which builds the same object from them.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=5927B1
    // Broiler-Falsified-If: two evaluations of one call site produce two strings objects, or the cache is reachable from guest code
    // Broiler-Human:        PENDING
    private void EmitTemplateStrings(JsTemplateLiteral quasi)
    {
        var count = quasi.Cooked.Count;

        if (count > 255)
        {
            Emit(JsOpcode.NewArray, (ushort)0);

            for (var index = 0; index < count; index++)
            {
                if (quasi.Cooked[index] is { } chunk)
                {
                    Emit(JsOpcode.LoadConstant, StringConstant(chunk));
                }
                else
                {
                    Emit(JsOpcode.LoadUndefined);
                }

                Emit(JsOpcode.ArrayAppend);
            }

            Emit(JsOpcode.NewArray, (ushort)0);

            for (var index = 0; index < count; index++)
            {
                Emit(JsOpcode.LoadConstant, StringConstant(quasi.Raw[index]));
                Emit(JsOpcode.ArrayAppend);
            }

            Emit(JsOpcode.GetTemplateObjectWide);
            return;
        }

        for (var index = 0; index < count; index++)
        {
            // A CHUNK AN UNDEFINED ESCAPE SPOILED IS `undefined` in the strings array, and only a
            // tagged template can carry one: the parser refuses it in an untagged template.
            if (quasi.Cooked[index] is { } chunk)
            {
                Emit(JsOpcode.LoadConstant, StringConstant(chunk));
            }
            else
            {
                Emit(JsOpcode.LoadUndefined);
            }
        }

        for (var index = 0; index < count; index++)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(quasi.Raw[index]));
        }

        Emit(JsOpcode.GetTemplateObject, (byte)count);
    }
}
