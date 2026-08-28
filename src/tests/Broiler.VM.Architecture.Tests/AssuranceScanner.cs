using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Which of the six exemption cases, if any, covers a declaration.
/// </summary>
/// <remarks>
/// The names are the cases as the component's assurance specification states them, so that a
/// reader can hold the code against the document one line at a time.
/// </remarks>
internal enum AssuranceExemption
{
    /// <summary>Not exempt. The unit is relevant and must carry an annotation.</summary>
    None = 0,

    /// <summary>Case 1: an auto-property, or an accessor whose body only returns or assigns a field.</summary>
    TrivialPropertyOrAccessor,

    /// <summary>Case 2: a constructor whose body only assigns parameters to fields or properties.</summary>
    ParameterAssigningConstructor,

    /// <summary>
    /// Case 3: an expression-bodied member whose body is a single member access, a single
    /// delegation to another member of the same type, a constant, or <c>throw new ...</c>.
    /// </summary>
    TrivialExpressionBodiedMember,

    /// <summary>Case 4: a compiler-generated member of a <c>record</c> or an <c>enum</c>.</summary>
    CompilerSuppliedRecordOrEnumMember,

    /// <summary>Case 5: <c>ToString</c>, <c>GetHashCode</c>, <c>Equals</c>, or an operator that only delegates.</summary>
    DelegatingOverrideOrOperator,

    /// <summary>Case 6: declared inside <c>AssemblyMarker</c>.</summary>
    InsideAssemblyMarker,

    /// <summary>
    /// The per-unit escape hatch: <c>// Broiler-AI: EXEMPT=&lt;reason&gt;</c>, for what the
    /// predicate cannot see. It is not one of the six and is deliberately named apart from them.
    /// </summary>
    DeclaredInSource,
}

/// <summary>One declaration in the product graph, with everything the policy asks about it.</summary>
internal sealed class AssuranceUnit
{
    internal required AssuranceSourceFile File { get; init; }

    internal required MemberDeclarationSyntax Declaration { get; init; }

    /// <summary>Namespace-qualified type and member, for a report a human can navigate by.</summary>
    internal required string Name { get; init; }

    /// <summary>One-based line of the declaration's first token.</summary>
    internal required int Line { get; init; }

    internal required AssuranceExemption Exemption { get; init; }

    internal AssuranceAnnotation? Annotation { get; init; }

    internal string Fingerprint => this.fingerprint ??= AssuranceFingerprint.Of(Declaration);

    private string? fingerprint;

    internal bool IsExempt => Exemption != AssuranceExemption.None;

    internal bool IsRelevant => !IsExempt;

    internal AssuranceReviewState State =>
        AssuranceStateMachine.Resolve(Annotation, IsExempt, Fingerprint);

    internal string Where => $"{File.RelativePath}({Line}): {Name}";
}

/// <summary>
/// Enumerates every declaration in the three product assemblies and classifies each as RELEVANT
/// or EXEMPT.
/// </summary>
/// <remarks>
/// <para>
/// <b>What counts as a declaration.</b> Every member that can carry executable code: methods,
/// constructors, destructors, operators, conversion operators, properties, indexers and events -
/// and every <c>const</c> or <c>static readonly</c> field that carries an initializer. Enum
/// members, delegate declarations, plain instance fields and the type declarations themselves are
/// not code units and are not enumerated. A bodiless declaration IS enumerated: an interface
/// member, an abstract member or an <c>extern</c> member has a signature, and in a contract
/// assembly the signature is precisely what a reviewer certifies.
/// </para>
/// <para>
/// <b>Why an initialized constant is a unit.</b> An earlier revision excluded every field with the
/// reason that fields "declare no implementation for a human to certify". That is false for an
/// initialized one, and the two constants it cost were both budgets: <c>MaximumEntries = 64</c>,
/// which ADR 0002 freezes, and <c>DefaultUnwindBudget = 1_000_000</c>. Either could be multiplied
/// by sixteen with no annotation to move and no fingerprint to invalidate, while the method that
/// READS the constant went on asserting the fingerprint of the version that was assessed. A field
/// initializer can also hold an arbitrary lambda - a whole admission algorithm with a loop in it -
/// and carry no review state at all. A plain instance field with no initializer stays out: it
/// declares storage and decides nothing.
/// </para>
/// <para>
/// <b>Why a predicate and not a list.</b> The policy asks for exemption rules that are
/// machine-checkable. Several hundred hand-written <c>EXEMPT</c> lines would be unreviewable,
/// and each one would be a place to hide a unit that should have been assessed. One predicate is
/// a page a reviewer can actually read, and it is the artefact under review here: if it is wrong,
/// it is wrong visibly and in one place.
/// </para>
/// <para>
/// <b>Bias.</b> Every borderline shape is answered RELEVANT. Over-including costs an annotation;
/// over-exempting loses a unit silently, which is the failure this system exists to prevent.
/// </para>
/// </remarks>
internal static class AssuranceScanner
{
    /// <summary>Every unit in the product graph, in file and then source order.</summary>
    internal static IReadOnlyList<AssuranceUnit> Units { get; } = AssuranceSources.Files
        .SelectMany(Scan)
        .ToArray();

    /// <summary>Scans one file. Used by the corpus above and, on synthesized text, by the tests.</summary>
    internal static IReadOnlyList<AssuranceUnit> Scan(AssuranceSourceFile file)
    {
        var text = new AssuranceText(file.Text);
        var units = new List<AssuranceUnit>();

        foreach (var declaration in file.Tree.GetRoot()
                     .DescendantNodes()
                     .OfType<MemberDeclarationSyntax>()
                     .Where(IsCodeUnit))
        {
            var annotation = AnnotationOn(declaration, text);

            units.Add(new AssuranceUnit
            {
                File = file,
                Declaration = declaration,
                Name = NameOf(declaration),
                Line = file.Tree.GetLineSpan(declaration.Span).StartLinePosition.Line + 1,
                Exemption = ExemptionFor(declaration, annotation),
                Annotation = annotation,
            });
        }

        return units;
    }

    /// <summary>The member kinds that can carry executable code, and so can be reviewed.</summary>
    internal static bool IsCodeUnit(MemberDeclarationSyntax member) => member switch
    {
        MethodDeclarationSyntax or
        ConstructorDeclarationSyntax or
        DestructorDeclarationSyntax or
        OperatorDeclarationSyntax or
        ConversionOperatorDeclarationSyntax or
        PropertyDeclarationSyntax or
        IndexerDeclarationSyntax or
        EventDeclarationSyntax => true,
        FieldDeclarationSyntax field => IsFixedValue(field),
        _ => false,
    };

    /// <summary>
    /// A <c>const</c> or <c>static readonly</c> field declaration that states a value.
    /// </summary>
    /// <remarks>
    /// The value is the reviewable thing, so the declaration has to state one: a
    /// <c>static readonly</c> assigned in a static constructor is reviewed there, and a field with
    /// no initializer at all decides nothing. The whole declaration is the unit, so a fingerprint
    /// covers the modifiers, the type, every declarator and every initializer expression.
    /// </remarks>
    internal static bool IsFixedValue(FieldDeclarationSyntax field)
    {
        var isConstant = field.Modifiers.Any(SyntaxKind.ConstKeyword);
        var isStaticReadOnly = field.Modifiers.Any(SyntaxKind.StaticKeyword) &&
            field.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);

        return (isConstant || isStaticReadOnly) &&
            field.Declaration.Variables.Any(static variable => variable.Initializer is not null);
    }

    // =============================================================================================
    // THE PREDICATE. This is the reviewable artefact of the whole exemption scheme.
    // =============================================================================================

    /// <summary>
    /// Decides whether a declaration is exempt from carrying its own review block, and under which
    /// of the six cases. <see cref="AssuranceExemption.None"/> means RELEVANT.
    /// </summary>
    /// <remarks>
    /// The cases are tried in the order they are written, and the first that matches is the answer.
    /// The order carries no policy - the cases are close to disjoint - but it is fixed so that the
    /// reported reason for a unit is stable across runs and diffs.
    /// </remarks>
    internal static AssuranceExemption ExemptionFor(
        MemberDeclarationSyntax declaration,
        AssuranceAnnotation? annotation)
    {
        // The escape hatch comes first: a reason a human wrote down outranks a predicate that
        // could not see what they saw. It is not one of the six.
        if (annotation?.ExemptReason is not null)
        {
            return AssuranceExemption.DeclaredInSource;
        }

        // CASE 6 - inside AssemblyMarker. A marker type exists so an assembly can be named from a
        // test; nothing in it is an implementation. Checked first because it is a property of
        // where the member lives rather than of what it says.
        if (ContainingTypes(declaration).Any(static type =>
                string.Equals(type.Identifier.ValueText, "AssemblyMarker", StringComparison.Ordinal)))
        {
            return AssuranceExemption.InsideAssemblyMarker;
        }

        // CASE 4 - a compiler-generated member of a record or an enum. In a syntax tree the
        // generated members are not there to be seen, so what this recognises is the shape that
        // produces them: a member of a record or an enum for which the source supplies no
        // implementation at all, the compiler's copy constructor, Equals, GetHashCode,
        // Deconstruct and printing members included.
        if (ContainingTypes(declaration).FirstOrDefault() is RecordDeclarationSyntax or EnumDeclarationSyntax &&
            !SuppliesAnImplementation(declaration))
        {
            return AssuranceExemption.CompilerSuppliedRecordOrEnumMember;
        }

        // CASE 1 - an auto-property, or an accessor whose body only returns or assigns a field.
        if (declaration is BasePropertyDeclarationSyntax property && IsTrivialProperty(property))
        {
            return AssuranceExemption.TrivialPropertyOrAccessor;
        }

        // CASE 2 - a constructor whose body only assigns parameters to fields or properties.
        if (declaration is ConstructorDeclarationSyntax constructor && AssignsParametersOnly(constructor))
        {
            return AssuranceExemption.ParameterAssigningConstructor;
        }

        // CASE 3 - an expression-bodied member whose body is a single member access, a single
        // delegation to another member of the same type, a constant, or `throw new ...`.
        if (ArrowBody(declaration) is { } arrow &&
            (IsSingleMemberAccess(arrow) ||
             IsDelegationToOwnMember(arrow, declaration) ||
             IsConstant(arrow) ||
             IsThrowNew(arrow)))
        {
            return AssuranceExemption.TrivialExpressionBodiedMember;
        }

        // CASE 5 - ToString, GetHashCode, Equals, or an operator, that ONLY delegates. The
        // qualifier is the whole case: an Equals that compares four fields itself is a decision
        // about equality and is relevant; an Equals that type-tests and hands off is not.
        if (IsOverrideOrOperator(declaration) && OnlyDelegates(declaration))
        {
            return AssuranceExemption.DelegatingOverrideOrOperator;
        }

        return AssuranceExemption.None;
    }

    // ---- Case 1 ---------------------------------------------------------------------------------

    private static bool IsTrivialProperty(BasePropertyDeclarationSyntax property)
    {
        // `public ulong MaxArtifactBytes => _bytes;` - a member access, nothing more.
        if (property is PropertyDeclarationSyntax { ExpressionBody: { } arrow })
        {
            return IsSingleMemberAccess(arrow.Expression);
        }

        if (property.AccessorList is null)
        {
            return false;
        }

        return property.AccessorList.Accessors.All(static accessor =>
        {
            // An accessor with no body at all is compiler-supplied: this is an auto-property.
            if (accessor.Body is null && accessor.ExpressionBody is null)
            {
                return true;
            }

            if (accessor.ExpressionBody is { } arrow)
            {
                return IsSingleMemberAccess(arrow.Expression) || IsFieldAssignmentFromValue(arrow.Expression);
            }

            return accessor.Body!.Statements is [var only] && only switch
            {
                ReturnStatementSyntax { Expression: { } returned } => IsSingleMemberAccess(returned),
                ExpressionStatementSyntax statement => IsFieldAssignmentFromValue(statement.Expression),
                _ => false,
            };
        });
    }

    /// <summary>`_field = value;` - the setter half of "only returns or assigns a field".</summary>
    private static bool IsFieldAssignmentFromValue(ExpressionSyntax expression) =>
        expression is AssignmentExpressionSyntax assignment &&
        assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) &&
        IsSingleMemberAccess(assignment.Left) &&
        assignment.Right is IdentifierNameSyntax { Identifier.ValueText: "value" };

    // ---- Case 2 ---------------------------------------------------------------------------------

    /// <summary>
    /// A constructor whose body assigns each of its parameters, at most once, to the member that
    /// CORRESPONDS to it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The correspondence is the whole case, and an earlier revision left it out: it asked only
    /// that the right-hand side be SOME parameter of the constructor, which makes every
    /// permutation of the assignments equally exempt. Swapping two lines in the
    /// <c>VmReadBounds</c> constructor re-points the section-count ceiling at the declared-count
    /// ceiling for every bounded read of untrusted input, and the predicate called it trivial - so
    /// the unit carried no annotation, no fingerprint moved, and no rule had anything to report.
    /// A constructor that permutes its parameters is making a decision, and a decision is what
    /// this system exists to put in front of a human.
    /// </para>
    /// <para>
    /// Names are compared case-insensitively after a leading underscore is dropped, which is the
    /// whole of the convention this component uses: <c>maxSectionCount</c> corresponds to
    /// <c>MaxSectionCount</c>, to <c>this.maxSectionCount</c> and to <c>_maxSectionCount</c>, and
    /// to nothing else. Anything the convention does not cover is answered RELEVANT.
    /// </para>
    /// <para>
    /// Each parameter may be assigned at most once, so a body that stores one parameter into two
    /// fields and drops another is relevant: which field ends up holding which value is exactly
    /// the fact this case would otherwise stop anyone from checking.
    /// </para>
    /// </remarks>
    private static bool AssignsParametersOnly(ConstructorDeclarationSyntax constructor)
    {
        // A `: this(...)` or `: base(...)` hop runs code this predicate is not looking at, so a
        // constructor that chains is relevant. An argument-free hop runs nothing worth reviewing.
        if (constructor.Initializer is { ArgumentList.Arguments.Count: > 0 })
        {
            return false;
        }

        IReadOnlyList<ExpressionSyntax>? assignments;

        if (constructor.ExpressionBody is { } arrow)
        {
            assignments = [arrow.Expression];
        }
        else if (constructor.Body is null)
        {
            return false;
        }
        else
        {
            assignments = Statements(constructor.Body);
        }

        if (assignments is null)
        {
            return false;
        }

        var assigned = new HashSet<string>(StringComparer.Ordinal);

        foreach (var expression in assignments)
        {
            if (expression is not AssignmentExpressionSyntax assignment ||
                !assignment.IsKind(SyntaxKind.SimpleAssignmentExpression) ||
                !IsSingleMemberAccess(assignment.Left) ||
                assignment.Right is not IdentifierNameSyntax right)
            {
                return false;
            }

            var parameter = constructor.ParameterList.Parameters.FirstOrDefault(parameter =>
                string.Equals(
                    parameter.Identifier.ValueText, right.Identifier.ValueText, StringComparison.Ordinal));

            if (parameter is null ||
                !Corresponds(AssignedMemberName(assignment.Left), parameter.Identifier.ValueText) ||
                !assigned.Add(parameter.Identifier.ValueText))
            {
                return false;
            }
        }

        return true;

        static IReadOnlyList<ExpressionSyntax>? Statements(BlockSyntax body)
        {
            var expressions = new List<ExpressionSyntax>();

            foreach (var statement in body.Statements)
            {
                if (statement is not ExpressionStatementSyntax expression)
                {
                    return null;
                }

                expressions.Add(expression.Expression);
            }

            return expressions;
        }
    }

    /// <summary>The simple name being assigned: the last identifier of `X`, `this.X` or `A.B.X`.</summary>
    private static string AssignedMemberName(ExpressionSyntax left) => Unwrap(left) switch
    {
        IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
        MemberAccessExpressionSyntax { Name: IdentifierNameSyntax name } => name.Identifier.ValueText,
        _ => string.Empty,
    };

    /// <summary>
    /// True when a member name and a parameter name are the same name under this component's
    /// convention: a leading underscore is decoration, and the leading capital is a casing rule.
    /// </summary>
    private static bool Corresponds(string member, string parameter) =>
        member.Length > 0 &&
        parameter.Length > 0 &&
        string.Equals(
            member.TrimStart('_'),
            parameter.TrimStart('_'),
            StringComparison.OrdinalIgnoreCase);

    // ---- Case 3 ---------------------------------------------------------------------------------

    /// <summary>A name, a `this`, or a dotted chain of them. No call, no operator, no argument.</summary>
    private static bool IsSingleMemberAccess(ExpressionSyntax expression) => Unwrap(expression) switch
    {
        IdentifierNameSyntax => true,
        ThisExpressionSyntax => true,
        MemberAccessExpressionSyntax member =>
            IsSingleMemberAccess(member.Expression) && member.Name is IdentifierNameSyntax,
        _ => false,
    };

    /// <summary>
    /// `Helper(x)`, `this.Helper(x)` or `OwnType.Helper(x)`, where every argument is a parameter of
    /// the delegating member or <c>this</c>. A call to something else - another type's static, a
    /// framework helper - is not this case, and neither is a call that supplies a value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The argument rule is the case.</b> An earlier revision accepted any argument that was a
    /// name or a constant, on the reading that a delegation passing constants is trivial. In this
    /// component the constant IS the policy. <c>TryReadVarUInt64</c> is
    /// <c>TryReadVarUInt64Core(maxBits: 64, out value)</c>: the literal is the width of the LEB128
    /// reader over untrusted bytes, and changing it to 32 changes what the parser accepts. The four
    /// <c>IVmBoundedAllocationMeter</c> members on <c>VmMeter</c> are
    /// <c>TryCharge(VmBudgetDimension.AllocatedBytes, ...)</c> and its siblings: the enum member is
    /// the meter the charge lands in, and re-routing it stops a ceiling being enforced. Both were
    /// exempt, so both carried no annotation and therefore no fingerprint, and both edits were
    /// invisible to every rule in the group.
    /// </para>
    /// <para>
    /// What survives is the shape that genuinely decides nothing: a member that forwards its own
    /// arguments unchanged. A parameter of the delegating member carries no decision, because the
    /// caller chose it; anything the member SUPPLIES - a literal, a named constant, an enum member,
    /// a field, a computed expression - is a decision the member is making, and it is reviewed.
    /// </para>
    /// </remarks>
    private static bool IsDelegationToOwnMember(ExpressionSyntax expression, SyntaxNode declaration)
    {
        if (Unwrap(expression) is not InvocationExpressionSyntax invocation)
        {
            return false;
        }

        if (!invocation.ArgumentList.Arguments.All(argument =>
                IsForwardedParameter(argument.Expression, declaration)))
        {
            return false;
        }

        var ownType = ContainingTypes(declaration).FirstOrDefault()?.Identifier.ValueText;

        return invocation.Expression switch
        {
            IdentifierNameSyntax or GenericNameSyntax => true,
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } => true,
            MemberAccessExpressionSyntax { Expression: IdentifierNameSyntax qualifier } =>
                ownType is not null &&
                string.Equals(qualifier.Identifier.ValueText, ownType, StringComparison.Ordinal),
            _ => false,
        };
    }

    /// <summary>
    /// True for an argument the delegating member did not choose: one of its own parameters, or
    /// <c>this</c>. Everything else - a literal, an enum member, a field, an expression - is a
    /// value the member supplies, and supplying a value is a decision.
    /// </summary>
    private static bool IsForwardedParameter(ExpressionSyntax expression, SyntaxNode declaration)
    {
        var unwrapped = Unwrap(expression);

        if (unwrapped is ThisExpressionSyntax)
        {
            return true;
        }

        if (unwrapped is not IdentifierNameSyntax identifier)
        {
            return false;
        }

        var parameters = declaration switch
        {
            BaseMethodDeclarationSyntax method => method.ParameterList.Parameters,
            IndexerDeclarationSyntax indexer => indexer.ParameterList.Parameters,
            // A property or event accessor has one implicit parameter and it is called `value`.
            _ => default,
        };

        if (parameters.Count == 0)
        {
            return declaration is BasePropertyDeclarationSyntax &&
                string.Equals(identifier.Identifier.ValueText, "value", StringComparison.Ordinal);
        }

        return parameters.Any(parameter => string.Equals(
            parameter.Identifier.ValueText, identifier.Identifier.ValueText, StringComparison.Ordinal));
    }

    private static bool IsConstant(ExpressionSyntax expression) => Unwrap(expression) switch
    {
        LiteralExpressionSyntax => true,
        PrefixUnaryExpressionSyntax unary when unary.IsKind(SyntaxKind.UnaryMinusExpression) ||
            unary.IsKind(SyntaxKind.UnaryPlusExpression) => IsConstant(unary.Operand),
        InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" } } => true,
        DefaultExpressionSyntax => true,
        _ => false,
    };

    private static bool IsThrowNew(ExpressionSyntax expression) =>
        Unwrap(expression) is ThrowExpressionSyntax { Expression: BaseObjectCreationExpressionSyntax };

    // ---- Case 5 ---------------------------------------------------------------------------------

    private static bool IsOverrideOrOperator(MemberDeclarationSyntax declaration) => declaration switch
    {
        MethodDeclarationSyntax method => method.Identifier.ValueText is "ToString" or "GetHashCode" or "Equals",
        OperatorDeclarationSyntax => true,
        ConversionOperatorDeclarationSyntax => true,
        _ => false,
    };

    /// <summary>
    /// True when the whole implementation is one expression built only from calls, names,
    /// literals, type tests and the two short-circuit operators - and contains at least one call.
    /// </summary>
    /// <remarks>
    /// A whitelist rather than a pattern list, because the question is what the expression may NOT
    /// contain. `MaxArtifactBytes == other.MaxArtifactBytes &amp;&amp; ...` is four comparisons and
    /// no call: `==` is not on the list and there is no invocation, so it fails twice over and the
    /// unit stays relevant, which is the answer that matters.
    /// </remarks>
    private static bool OnlyDelegates(MemberDeclarationSyntax declaration)
    {
        var expression = ArrowBody(declaration) ?? SingleReturnedExpression(declaration);

        if (expression is null)
        {
            return false;
        }

        var permitted = expression.DescendantNodesAndSelf().All(static node => node switch
        {
            InvocationExpressionSyntax => true,
            MemberAccessExpressionSyntax => true,
            ArgumentListSyntax or ArgumentSyntax => true,
            ParenthesizedExpressionSyntax => true,
            ThisExpressionSyntax or BaseExpressionSyntax => true,
            LiteralExpressionSyntax => true,
            IsPatternExpressionSyntax => true,
            DeclarationPatternSyntax or SingleVariableDesignationSyntax => true,
            BinaryExpressionSyntax binary =>
                binary.IsKind(SyntaxKind.LogicalAndExpression) || binary.IsKind(SyntaxKind.LogicalOrExpression),
            PrefixUnaryExpressionSyntax unary => unary.IsKind(SyntaxKind.LogicalNotExpression),
            TypeSyntax => true,
            TypeArgumentListSyntax => true,
            _ => false,
        });

        return permitted && expression.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>().Any();
    }

    // ---- Shared shape helpers -------------------------------------------------------------------

    private static ExpressionSyntax? ArrowBody(MemberDeclarationSyntax declaration) => declaration switch
    {
        MethodDeclarationSyntax method => method.ExpressionBody?.Expression,
        ConstructorDeclarationSyntax constructor => constructor.ExpressionBody?.Expression,
        DestructorDeclarationSyntax destructor => destructor.ExpressionBody?.Expression,
        OperatorDeclarationSyntax @operator => @operator.ExpressionBody?.Expression,
        ConversionOperatorDeclarationSyntax conversion => conversion.ExpressionBody?.Expression,
        PropertyDeclarationSyntax property => property.ExpressionBody?.Expression,
        IndexerDeclarationSyntax indexer => indexer.ExpressionBody?.Expression,
        _ => null,
    };

    private static ExpressionSyntax? SingleReturnedExpression(MemberDeclarationSyntax declaration) =>
        declaration switch
        {
            MethodDeclarationSyntax { Body.Statements: [ReturnStatementSyntax { Expression: { } only }] } => only,
            OperatorDeclarationSyntax { Body.Statements: [ReturnStatementSyntax { Expression: { } only }] } => only,
            ConversionOperatorDeclarationSyntax
            { Body.Statements: [ReturnStatementSyntax { Expression: { } only }] } => only,
            _ => null,
        };

    /// <summary>True when the source, rather than the compiler, says what this member does.</summary>
    private static bool SuppliesAnImplementation(MemberDeclarationSyntax declaration)
    {
        if (ArrowBody(declaration) is not null)
        {
            return true;
        }

        // An initializer is an implementation the source supplies, so a constant declared inside a
        // record is not one of the members the compiler writes and case 4 does not reach it.
        if (declaration is FieldDeclarationSyntax field)
        {
            return field.Declaration.Variables.Any(static variable => variable.Initializer is not null);
        }

        if (declaration is BaseMethodDeclarationSyntax { Body: not null })
        {
            return true;
        }

        return declaration is BasePropertyDeclarationSyntax { AccessorList: { } accessors } &&
            accessors.Accessors.Any(static accessor =>
                accessor.Body is not null || accessor.ExpressionBody is not null);
    }

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression) =>
        expression is ParenthesizedExpressionSyntax parenthesized
            ? Unwrap(parenthesized.Expression)
            : expression;

    private static IEnumerable<BaseTypeDeclarationSyntax> ContainingTypes(SyntaxNode node) =>
        node.Ancestors().OfType<BaseTypeDeclarationSyntax>();

    // ---- Naming and annotation lookup -----------------------------------------------------------

    private static string NameOf(MemberDeclarationSyntax declaration)
    {
        var owner = string.Join(
            ".",
            ContainingTypes(declaration).Reverse().Select(static type => type.Identifier.ValueText));

        var space = declaration.Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .Select(static @namespace => @namespace.Name.ToString())
            .FirstOrDefault();

        var member = declaration switch
        {
            MethodDeclarationSyntax method => method.Identifier.ValueText + Generics(method.TypeParameterList),
            ConstructorDeclarationSyntax constructor => constructor.Identifier.ValueText,
            DestructorDeclarationSyntax destructor => "~" + destructor.Identifier.ValueText,
            OperatorDeclarationSyntax @operator => "operator " + @operator.OperatorToken.ValueText,
            ConversionOperatorDeclarationSyntax conversion => "operator " + conversion.Type,
            PropertyDeclarationSyntax property => property.Identifier.ValueText,
            IndexerDeclarationSyntax => "this[]",
            EventDeclarationSyntax @event => @event.Identifier.ValueText,
            FieldDeclarationSyntax field => string.Join(
                ", ",
                field.Declaration.Variables.Select(static variable => variable.Identifier.ValueText)),
            _ => "<member>",
        };

        var qualified = string.IsNullOrEmpty(space) ? owner : $"{space}.{owner}";

        return $"{qualified}.{member}{Parameters(declaration)}";

        static string Generics(TypeParameterListSyntax? parameters) => parameters is null
            ? string.Empty
            : "<" + string.Join(",", parameters.Parameters.Select(static p => p.Identifier.ValueText)) + ">";

        // Parameter modifiers are part of the name because they are part of the signature: two
        // overloads differing only by `out` are two units, and a name that dropped the keyword
        // would make one of them unreachable from a report.
        static string Parameters(MemberDeclarationSyntax declaration) =>
            declaration is BaseMethodDeclarationSyntax method
                ? "(" + string.Join(
                    ", ",
                    method.ParameterList.Parameters.Select(static parameter => string.Concat(
                        parameter.Modifiers.Select(static modifier => modifier.ValueText + " ")) +
                        (parameter.Type?.ToString() ?? "?"))) + ")"
                : string.Empty;
    }

    /// <summary>
    /// The two-line block in this declaration's leading trivia, or null when it carries none.
    /// </summary>
    /// <remarks>
    /// The block is looked for in the leading trivia rather than by scanning lines, so a comment
    /// that merely mentions <c>Broiler-AI:</c> somewhere else in the file is not mistaken for one
    /// - and so a block that has drifted away from any declaration is left for
    /// <see cref="OrphanAnnotations"/> to report rather than being silently attached to whatever
    /// follows it.
    /// </remarks>
    private static AssuranceAnnotation? AnnotationOn(MemberDeclarationSyntax declaration, AssuranceText text)
    {
        foreach (var trivia in declaration.GetLeadingTrivia())
        {
            if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) ||
                !trivia.ToString().TrimStart().StartsWith(AssuranceAnnotation.AiMarker, StringComparison.Ordinal))
            {
                continue;
            }

            var line = declaration.SyntaxTree.GetLineSpan(trivia.Span).StartLinePosition.Line;

            return AssuranceAnnotation.TryParse(text, line, out _);
        }

        return null;
    }

    /// <summary>
    /// Every assurance comment line in a file that is not part of a block attached to a
    /// declaration, with the reason it did not parse where one is available.
    /// </summary>
    internal static IEnumerable<string> OrphanAnnotations(
        AssuranceSourceFile file,
        IEnumerable<AssuranceUnit> units)
    {
        var text = new AssuranceText(file.Text);

        var attached = units
            .Where(static unit => unit.Annotation is not null)
            .SelectMany(static unit => new[] { unit.Annotation!.AiLine, unit.Annotation!.HumanLine })
            .ToHashSet();

        for (var line = 0; line < text.Count; line++)
        {
            var trimmed = text[line].TrimStart();

            var isAssuranceLine =
                trimmed.StartsWith(AssuranceAnnotation.AiMarker, StringComparison.Ordinal) ||
                trimmed.StartsWith(AssuranceAnnotation.HumanMarker, StringComparison.Ordinal);

            if (!isAssuranceLine || attached.Contains(line))
            {
                continue;
            }

            AssuranceAnnotation.TryParse(text, line, out var problem);

            yield return $"{file.RelativePath}({line + 1}): " +
                (problem ?? "an assurance comment that is attached to no declaration");
        }
    }
}
