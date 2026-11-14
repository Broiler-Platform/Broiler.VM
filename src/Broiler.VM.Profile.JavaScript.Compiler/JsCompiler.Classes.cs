// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=89AE54
    // Broiler-Human:        PENDING
    private void CompileClassDeclaration(ClassDeclaration declaration)
    {
        var name = NameOf(declaration.Class);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5EA1E1
    // Broiler-Human:        PENDING
    private void CompileClass(ClassNode node, string inferredName)
    {
        var outer = scope;
        var own = NameOf(node);
        var named = own.Length != 0;
        var privates = binder.PrivateNames(node);
        var heritage = node.Heritage is not null;

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
                scope.Declare(own, constant: true);
            }
        }

        if (heritage)
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

        var name = named ? own : inferredName;
        var written = JsTreeFacts.FindConstructor(node, unitParse.BigInts);
        var constructor = written?.Function;

        var flags = JsFormat.FunctionFlags.ClassConstructor | JsFormat.FunctionFlags.Constructible |
            (heritage ? JsFormat.FunctionFlags.DerivedConstructor : JsFormat.FunctionFlags.None);

        var unit = constructor is null
            ? CompileImplicitConstructor(JsTreeFacts.At(node), name, heritage, flags)
            : CompileFunction(
                constructor,
                JsTreeFacts.At(written!),
                name,
                flags,
                isMethod: true,
                isDerived: heritage);

        // A CLASS'S CONSTRUCTOR RENDERS AS THE WHOLE CLASS, whether it was written or implied.
        RecordSource(unit, node.Span.Offset, JsTreeFacts.EndOf(node));

        Emit(JsOpcode.Closure, (ushort)unit);
        Emit(JsOpcode.NewClass, (byte)(heritage ? JsOpcodes.ClassIsDerived : 0));

        var defines = node.Elements.Count != 0 &&
            (constructor is null || node.Elements.Count != 1);

        if (defines)
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetProperty, InternedName("prototype"));
        }

        var statics = false;

        foreach (var member in node.Elements)
        {
            if (ReferenceEquals(member, written))
            {
                continue;
            }

            Position(JsTreeFacts.At(member));

            if (member is not MethodDefinition { Key: not PrivateIdentifier } method)
            {
                statics |= member is StaticBlock or FieldDefinition { IsStatic: true } or MethodDefinition { IsStatic: true };
                CompileClassElement(member);
                continue;
            }

            if (method.IsStatic)
            {
                Emit(JsOpcode.Pick, (byte)1);
            }

            var computed = JsTreeFacts.KeyExpression(method.Key);
            var key = computed is null ? KeyOf(method.Key) : string.Empty;

            if (computed is null)
            {
                Emit(JsOpcode.LoadConstant, StringConstant(key));
            }
            else
            {
                CompileExpression(computed);
            }

            Emit(JsOpcode.Closure, (ushort)CompileFunction(method.Function, JsTreeFacts.At(method), key, isMethod: true));
            Emit(JsOpcode.DefineMethod, MemberOperand(method.MethodKind, enumerable: false));

            if (method.IsStatic)
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
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(own));
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=217BAA
    // Broiler-Human:        PENDING
    private void CompileClassElement(SyntaxNode member)
    {
        var (isStatic, written) = member switch
        {
            MethodDefinition method => (method.IsStatic, method.Key),
            FieldDefinition field => (field.IsStatic, field.Key),
            _ => (true, null),
        };

        var isPrivate = written is PrivateIdentifier;
        var flags = (byte)(
            (isStatic ? JsOpcodes.ElementIsStatic : 0) |
            (isPrivate ? JsOpcodes.ElementIsPrivate : 0));

        if (member is StaticBlock block)
        {
            Emit(JsOpcode.LoadUndefined);

            Emit(
                JsOpcode.Closure,
                (ushort)CompileFunction(
                    block, JsTreeFacts.At(block), string.Empty, [], block.Body, null,
                    isArrow: false, isGenerator: false, isAsync: false, block.Span.Offset, JsTreeFacts.EndOf(block),
                    isMethod: true, isStaticBlock: true));

            Emit(JsOpcode.DefineClassElement, (byte)(flags | JsOpcodes.ElementIsBlock));
            return;
        }

        var computed = JsTreeFacts.KeyExpression(written!);
        var key = computed is null ? KeyOf(written!) : string.Empty;

        if (isPrivate)
        {
            EmitPrivateName(JsTreeFacts.At(member), key);
        }
        else if (computed is null)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(key));
        }
        else
        {
            CompileExpression(computed);
        }

        if (member is MethodDefinition element)
        {
            flags |= element.MethodKind switch
            {
                MethodKind.Get => (byte)(JsOpcodes.ElementIsMethod | JsOpcodes.ElementIsGetter),
                MethodKind.Set => (byte)(JsOpcodes.ElementIsMethod | JsOpcodes.ElementIsSetter),
                _ => JsOpcodes.ElementIsMethod,
            };

            Emit(JsOpcode.Closure, (ushort)CompileFunction(element.Function, JsTreeFacts.At(element), key, isMethod: true));
            Emit(JsOpcode.DefineClassElement, flags);
            return;
        }

        var value = ((FieldDefinition)member).Value;

        // A FIELD WITH NO INITIALISER IS `undefined` AND NOT AN ABSENT FIELD. `class C { x }`
        // defines `x` on every instance, so the field is recorded with no initialiser rather than
        // not recorded - and the executor tells the two apart by what is pushed here.
        if (value is null)
        {
            Emit(JsOpcode.LoadUndefined);
        }
        else
        {
            if (computed is not null && !isPrivate && NamedAtRunTime(value))
            {
                flags |= JsOpcodes.ElementIsNamedValue;
            }

            // A FIELD'S INITIALISER IS A FUNCTION RETURNING ITS VALUE AT THE FIELD, with no source of its
            // own, as the wide parser's tree held it.
            Emit(
                JsOpcode.Closure,
                (ushort)CompileFunction(
                    member, JsTreeFacts.At(member), key, [], [], value,
                    isArrow: false, isGenerator: false, isAsync: false, -1, -1,
                    isMethod: true, isFieldInitialiser: true));
        }

        Emit(JsOpcode.DefineClassElement, flags);
    }

    /// <summary>Pushes the private name a class body in scope declared under this spelling.</summary>
    /// <remarks>
    /// <b>It resolves through the ordinary scope chain, so the INNERMOST class that declares the
    /// spelling wins</b> - which is what the specification's PrivateEnvironment says, and what makes
    /// a nested class able to declare its own <c>#x</c> without disturbing the outer one's. A
    /// spelling no enclosing class declared is an early error and not a run-time absence, because
    /// there is no object a name nobody minted could be found on - the binder's to refuse, before
    /// anything is lowered (SJL-12.6 item 2).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=575843
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
        if (evalRoot is not null && (evalFlags & JsFormat.EvalRequestFlags.InClassBody) != 0)
        {
            if (TryEvalHops(out var evalHops))
            {
                var spelled = PrivateSlot(name);

                if (!evalPrivateNames.Contains(spelled))
                {
                    evalPrivateNames.Add(spelled);
                }

                EmitEvalName(JsOpcode.LoadEvalName, evalHops, spelled);
                return;
            }

            // PAST THE SCOPE-DEPTH CEILING THE READ HAS NO HOP COUNT, and the ceiling's refusal,
            // which stays the lowering's, has already refused the unit. The name then cannot be
            // read through the caller at all, and the lowering refuses it as it always did, after
            // the ceiling's 2306 (2103 until registry revision 19): a consequence of the ceiling, not
            // a language early error, so it is not the binder's to report and the refused unit's list
            // stays the one it was. Its code is 2217, a private name no enclosing class declares,
            // though past the ceiling the name may be one the caller's class declares; only this
            // defensive site reaches 2217 that way. *(Revised 2026-11-08: it read "after the
            // ceiling's 2103: a consequence of the ceiling".)*
            Refuse(
                span,
                SliceSourceDiagnosticCode.UndeclaredPrivateName,
                "`" + name + "` is not declared by any class this expression is inside of");

            Emit(JsOpcode.LoadUndefined);
            return;
        }

        throw BinderAdmitted(span, "the private name `" + name + "`, which no class around it declares");
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A12D3C
    // Broiler-Human:        PENDING
    private static byte MemberOperand(MethodKind kind, bool enumerable) => (byte)(
        (kind switch
        {
            MethodKind.Get => JsOpcodes.MemberIsGetter,
            MethodKind.Set => JsOpcodes.MemberIsSetter,
            _ => 0,
        }) | (enumerable ? JsOpcodes.MemberIsEnumerable : 0));
}
