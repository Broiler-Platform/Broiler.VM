// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           0
// Human-reviewed:   0/11
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    /// <summary>Binds a class declaration's name after building the class.</summary>
    /// <remarks>
    /// The binding is mutable, which a reader who has just seen the class's own binding declared
    /// constant will want explained: they are two different bindings. <c>class C { }</c> introduces
    /// <c>C</c> in the enclosing scope as an ordinary <c>let</c>, and separately introduces a
    /// constant <c>C</c> that only the class body can see - which is why <c>C = 1</c> after the
    /// declaration is fine and <c>C = 1</c> inside a method is not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=38B06E
    // Broiler-Human:        PENDING
    private void CompileClassDeclaration(JsClassDeclaration declaration)
    {
        var name = declaration.Class.Name;
        CompileClass(declaration.Class, name);

        if (scope.Kind == ScopeKind.Program && blockDepth == 0)
        {
            Emit(
                programLexicals.ContainsKey(name)
                    ? JsOpcode.InitialiseGlobalLexical
                    : JsOpcode.StoreGlobal,
                InternedName(name));

            return;
        }

        var slot = scope.Has(name) ? scope.SlotOf(name) : scope.Declare(name, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
    }

    /// <summary>
    /// Lowers a class, leaving the constructor on the operand stack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The shape is: a scope, the heritage, the constructor, the class, then the members.</b>
    /// The scope holds the class's own name and is pushed FIRST, because every closure the body
    /// creates - the constructor included - has to capture it; a scope pushed after the constructor
    /// closure would leave <c>C</c> unresolvable inside <c>C</c>'s own methods. It is popped at the
    /// end, and popping it destroys nothing: the closures captured the record itself.
    /// </para>
    /// <para>
    /// <b>The class binding is initialised LAST, after every member has been defined</b>, and that
    /// ordering is observable: a computed member key that names the class reads a binding still in
    /// its dead zone and throws, which is what the specification asks for and what a lowering that
    /// initialised it early would answer <c>undefined</c> to.
    /// </para>
    /// <para>
    /// While the members are defined the stack holds the constructor and the prototype, in that
    /// order, so a prototype member needs no reload and a static member is one
    /// <see cref="JsOpcode.Pick"/> away. The alternative - reading <c>C.prototype</c> before each
    /// prototype member - would be a property lookup per member for no gain, and it would go
    /// through a property this instruction set deliberately makes non-writable.
    /// <b>That pair is also what <see cref="JsOpcode.DefineClassElement"/> reads</b>, which is why
    /// a field costs no reload either.
    /// </para>
    /// <para>
    /// <b>A class body has FOUR times in it and not one, and every ordering rule below is one of
    /// them.</b> The private names are minted first, because a method compiled after them has to
    /// capture the slots they live in. Then every key in the body is evaluated, in source order,
    /// including the keys of static fields whose initialisers have not run. Then the class binding
    /// is initialised. Only then do the static initialisers and blocks run, which is what lets
    /// <c>static { C.tag = 1 }</c> name the class and stops
    /// <c>static [C.name] = 1</c> from doing so. A lowering that performed a static field where it
    /// was written would have collapsed the middle two and been wrong about both.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=82D7CD
    // Broiler-Human:        PENDING
    private void CompileClass(JsClassNode node, string inferredName)
    {
        var outer = scope;
        var named = node.Name.Length != 0;
        var privates = PrivateNamesOf(node);

        // THE SCOPE EXISTS FOR THE PRIVATE NAMES TOO AND NOT ONLY FOR THE CLASS'S OWN BINDING. An
        // anonymous class with a `#x` in it has nowhere else to keep the name, and the name has to
        // be somewhere every method of the body captures - which is the same requirement the class
        // binding has and is met by the same record.
        var scoped = named || privates.Count != 0;

        if (scoped)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var site = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((site, scope));

            if (named)
            {
                scope.Declare(node.Name, constant: true);
            }
        }

        if (node.HasHeritage)
        {
            CompileExpression(node.Heritage!);
        }

        // THE PRIVATE NAMES ARE DECLARED AFTER THE HERITAGE IS COMPILED, because the heritage is
        // evaluated in the OUTER private environment (15.7.14 step 8): `class extends (o.#x) {
        // #x }` names an outer `#x` or none, never the class's own. Declaring them first let the
        // heritage resolve them (JSC-253). The class binding stays visible to the heritage, in
        // its dead zone, as the specification has it.
        if (scoped)
        {
            foreach (var privateName in privates)
            {
                var slot = scope.Declare(PrivateSlot(privateName), constant: true);
                Emit(JsOpcode.NewPrivateName, StringConstant(privateName));
                EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
            }
        }

        var name = named ? node.Name : inferredName;
        var constructor = FindConstructor(node);

        var flags = JsFormat.FunctionFlags.ClassConstructor | JsFormat.FunctionFlags.Constructible |
            (node.HasHeritage ? JsFormat.FunctionFlags.DerivedConstructor : JsFormat.FunctionFlags.None);

        var unit = constructor is null
            ? CompileImplicitConstructor(node.Span, name, node.HasHeritage, flags)
            : CompileFunction(
                constructor with { Name = name }, flags, isMethod: true, isDerived: node.HasHeritage);

        // A CLASS'S CONSTRUCTOR RENDERS AS THE WHOLE CLASS, whether it was written or implied.
        RecordSource(unit, node.SourceStart, node.SourceEnd);

        Emit(JsOpcode.Closure, (ushort)unit);
        Emit(JsOpcode.NewClass, (byte)(node.HasHeritage ? JsOpcodes.ClassIsDerived : 0));

        var defines = node.Members.Count != 0 &&
            (constructor is null || node.Members.Count != 1);

        if (defines)
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetProperty, InternedName("prototype"));
        }

        var statics = false;

        foreach (var member in node.Members)
        {
            if (member.Function is not null && ReferenceEquals(member.Function, constructor))
            {
                continue;
            }

            Position(member.Span);

            if (member.Kind is JsMethodKind.Field or JsMethodKind.StaticBlock || member.IsPrivate)
            {
                statics |= member.IsStatic;
                CompileClassElement(member);
                continue;
            }

            if (member.IsStatic)
            {
                Emit(JsOpcode.Pick, (byte)1);
            }

            if (member.Computed is null)
            {
                Emit(JsOpcode.LoadConstant, StringConstant(member.Key));
            }
            else
            {
                CompileExpression(member.Computed);
            }

            Emit(JsOpcode.Closure, (ushort)CompileFunction(member.Function!, isMethod: true));
            Emit(JsOpcode.DefineMethod, MemberOperand(member.Kind, enumerable: false));

            if (member.IsStatic)
            {
                Emit(JsOpcode.Pop);
            }
        }

        if (defines)
        {
            Emit(JsOpcode.Pop);
        }

        if (named)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(node.Name));
        }

        // THE STATIC ELEMENTS RUN AFTER THE BINDING AND BEFORE THE SCOPE GOES, and both halves of
        // that matter. A static block that names the class needs the binding initialised; a static
        // block that reads a private name needs the record the names live in still to be the
        // innermost one, because the block's own closure resolves them by hop count.
        if (statics)
        {
            Emit(JsOpcode.RunStaticElements);
        }

        if (!scoped)
        {
            return;
        }

        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }

    /// <summary>
    /// Lowers one field, private member or static block into a record on the constructor.
    /// </summary>
    /// <remarks>
    /// <b>Every arm leaves the stack exactly as it found it</b>, which is what lets the caller run
    /// a whole class body over the one constructor-and-prototype pair it loaded once. The key and
    /// the initialiser are pushed and consumed by the one instruction, and the pair beneath them is
    /// read rather than popped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E036F1
    // Broiler-Human:        PENDING
    private void CompileClassElement(JsClassMember member)
    {
        var flags = (byte)(
            (member.IsStatic ? JsOpcodes.ElementIsStatic : 0) |
            (member.IsPrivate ? JsOpcodes.ElementIsPrivate : 0));

        if (member.Kind == JsMethodKind.StaticBlock)
        {
            RefuseArguments(member.Function!);
            Emit(JsOpcode.LoadUndefined);

            Emit(
                JsOpcode.Closure,
                (ushort)CompileFunction(member.Function!, isMethod: true, isStaticBlock: true));

            Emit(JsOpcode.DefineClassElement, (byte)(flags | JsOpcodes.ElementIsBlock));
            return;
        }

        if (member.IsPrivate)
        {
            EmitPrivateName(member.Span, member.Key);
        }
        else if (member.Computed is null)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(member.Key));
        }
        else
        {
            CompileExpression(member.Computed);
        }

        if (member.Kind != JsMethodKind.Field)
        {
            flags |= member.Kind switch
            {
                JsMethodKind.Get => (byte)(JsOpcodes.ElementIsMethod | JsOpcodes.ElementIsGetter),
                JsMethodKind.Set => (byte)(JsOpcodes.ElementIsMethod | JsOpcodes.ElementIsSetter),
                _ => JsOpcodes.ElementIsMethod,
            };

            Emit(JsOpcode.Closure, (ushort)CompileFunction(member.Function!, isMethod: true));
            Emit(JsOpcode.DefineClassElement, flags);
            return;
        }

        // A FIELD WITH NO INITIALISER IS `undefined` AND NOT AN ABSENT FIELD. `class C { x }`
        // defines `x` on every instance, so the field is recorded with no initialiser rather than
        // not recorded - and the executor tells the two apart by what is pushed here.
        if (member.Function is null)
        {
            Emit(JsOpcode.LoadUndefined);
        }
        else
        {
            RefuseArguments(member.Function);

            if (member.Computed is not null && !member.IsPrivate &&
                member.Function.Body is [JsReturnStatement { Value: { } value }] &&
                NamedAtRunTime(value))
            {
                flags |= JsOpcodes.ElementIsNamedValue;
            }

            Emit(
                JsOpcode.Closure,
                (ushort)CompileFunction(member.Function, isMethod: true, isFieldInitialiser: true));
        }

        Emit(JsOpcode.DefineClassElement, flags);
    }

    /// <summary>
    /// Refuses an <c>arguments</c> anywhere inside a field initialiser or a static block.
    /// </summary>
    /// <remarks>
    /// <b>Neither has an <c>arguments</c> of its own and neither may borrow one</b>, which is what
    /// makes this an early error rather than a read of the enclosing function's object. Both run
    /// with a <c>this</c> the class body does not have and with no argument list at all, so the
    /// specification makes the WORD a Syntax Error there rather than leaving a program to discover
    /// at run time that the object it named is somebody else's. An arrow inside one is included,
    /// because an arrow has no <c>arguments</c> either and reaches outward for it; an ordinary
    /// nested function is not, because it has one of its own - which is exactly the boundary the
    /// walk this calls already draws for the enclosing function's own materialisation question.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=21BAC5
    // Broiler-Human:        PENDING
    private void RefuseArguments(JsFunctionNode body)
    {
        foreach (var statement in body.Body)
        {
            if (!Walk.Mentions(statement, "arguments"))
            {
                continue;
            }

            Refuse(
                body.Span,
                SliceSourceDiagnosticCode.UnresolvableIdentifier,
                "`arguments` names nothing inside a class field initialiser or a static block");

            return;
        }
    }

    /// <summary>
    /// Every private name one class body declares, in source order and once each.
    /// </summary>
    /// <remarks>
    /// <b><c>get #a</c> and <c>set #a</c> declare ONE name and not two</b>, which is why this
    /// de-duplicates rather than counting members. Two slots would have made the setter write an
    /// element the getter could not see, and the brand check would then answer differently
    /// depending on which half a program happened to ask through.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=14A539
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<string> PrivateNamesOf(JsClassNode node)
    {
        var found = new System.Collections.Generic.List<string>();

        foreach (var member in node.Members)
        {
            if (member.IsPrivate && !found.Contains(member.Key))
            {
                found.Add(member.Key);
            }
        }

        return found;
    }

    /// <summary>Pushes the private name a class body in scope declared under this spelling.</summary>
    /// <remarks>
    /// <b>It resolves through the ordinary scope chain, so the INNERMOST class that declares the
    /// spelling wins</b> - which is what the specification's PrivateEnvironment says, and what makes
    /// a nested class able to declare its own <c>#x</c> without disturbing the outer one's. A
    /// spelling no enclosing class declared is a refusal here and not a run-time absence, because
    /// there is no object a name nobody minted could be found on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9133B7
    // Broiler-Human:        PENDING
    private void EmitPrivateName(SliceSourceSpan span, string name)
    {
        if (TryResolve(PrivateSlot(name), out var hops, out var slot, out _))
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)hops, slot);
            return;
        }

        // A PRIVATE NAME EVAL CODE DOES NOT DECLARE MAY BE ITS CALLER'S CLASS'S (JSeal V15-finish).
        // The caller's map carries its classes' private-name slots, so the name is read through it
        // like any other of the caller's names, and the declaration row lists it: the executor
        // makes the evaluation the SyntaxError the language gives when no class around the call
        // declares it, before anything is instantiated.
        if (evalRoot is not null && (evalFlags & JsFormat.EvalRequestFlags.InClassBody) != 0 &&
            TryEvalHops(out var evalHops))
        {
            var spelled = PrivateSlot(name);

            if (!evalPrivateNames.Contains(spelled))
            {
                evalPrivateNames.Add(spelled);
            }

            EmitEvalName(JsOpcode.LoadEvalName, evalHops, spelled);
            return;
        }

        Refuse(
            span,
            SliceSourceDiagnosticCode.UnresolvableIdentifier,
            "`" + name + "` is not declared by any class this expression is inside of");

        Emit(JsOpcode.LoadUndefined);
    }

    /// <summary>The slot name a private name is kept under.</summary>
    /// <remarks>
    /// <b>The second <c>#</c> is what keeps a private name apart from the lowering's own
    /// temporaries.</b> A compound assignment declares a slot called <c>#target0</c> in the
    /// enclosing FUNCTION scope, which is inside the class scope a private name lives in, so a
    /// class that declared <c>#target0</c> - a perfectly ordinary private name - would have had its
    /// name shadowed by a temporary and every access would have read whatever the last assignment
    /// left there. No private name can begin with a second <c>#</c>, because an identifier cannot,
    /// so this spelling is one no source can collide with.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6DE4AA
    // Broiler-Human:        PENDING
    private static string PrivateSlot(string name) => "#" + name;

    /// <summary>The class body's own <c>constructor</c>, when it wrote one.</summary>
    /// <remarks>
    /// A STATIC member called <c>constructor</c> is not it, and neither is a getter of that name
    /// nor a computed key that happens to evaluate to the string: the specification decides this
    /// syntactically, on a non-static, non-computed method whose property name is
    /// <c>constructor</c>, and so does this.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6BC9DC
    // Broiler-Human:        PENDING
    private static JsFunctionNode? FindConstructor(JsClassNode node)
    {
        foreach (var member in node.Members)
        {
            if (!member.IsStatic &&
                !member.IsPrivate &&
                member.Kind == JsMethodKind.Method &&
                member.Computed is null &&
                string.Equals(member.Key, "constructor", System.StringComparison.Ordinal))
            {
                return member.Function;
            }
        }

        return null;
    }

    /// <summary>Builds the constructor a class body did not write.</summary>
    /// <remarks>
    /// <b>There is no syntax tree behind this unit, and there could not be.</b> A base class's
    /// implicit constructor is <c>constructor() { }</c>, which a tree could express; a derived
    /// class's is <c>constructor(...args) { super(...args); }</c>, and this manifest admits neither
    /// a rest parameter nor a spread argument - both are refused by name. Rather than admit half of
    /// each to write one function nobody typed, the forwarding is an instruction.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=432C40
    // Broiler-Human:        PENDING
    private int CompileImplicitConstructor(
        SliceSourceSpan span, string name, bool derived, JsFormat.FunctionFlags flags)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;

        strict = true;
        var index = units.Count;
        var constant = name.Length == 0 ? (ushort)0 : (ushort)(InternedName(name) + 1);
        buffer = new UnitBuffer(constant, flags | JsFormat.FunctionFlags.Strict);
        units.Add(buffer);
        scope = new Scope(ScopeKind.Function, outerScope);
        blockDepth = 0;
        Position(span);

        if (derived)
        {
            Emit(JsOpcode.SuperCallForwarded);
            Emit(JsOpcode.Pop);
        }

        Emit(JsOpcode.ReturnUndefined);
        buffer.SlotCount = scope.SlotCount;
        buffer = outerBuffer;
        scope = outerScope;
        blockDepth = outerDepth;
        strict = outerStrict;
        return index;
    }

    /// <summary>The <see cref="JsOpcode.DefineMethod"/> operand for one member.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CBC5E3
    // Broiler-Human:        PENDING
    private static byte MemberOperand(JsMethodKind kind, bool enumerable) => (byte)(
        (kind switch
        {
            JsMethodKind.Get => JsOpcodes.MemberIsGetter,
            JsMethodKind.Set => JsOpcodes.MemberIsSetter,
            _ => 0,
        }) | (enumerable ? JsOpcodes.MemberIsEnumerable : 0));
}
