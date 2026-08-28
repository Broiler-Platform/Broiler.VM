using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The fingerprint that binds a human decision to a specific version of an implementation:
/// SHA-256 over the declaration's token texts, joined by single spaces, rendered as the first six
/// uppercase hex characters.
/// </summary>
/// <remarks>
/// <para>
/// <b>What is in it.</b> Every token of the declaration node - modifiers, name, type parameters
/// and their constraints, the parameter list with its types, the return type, attributes, and the
/// whole body including any local function. Tokens are emitted as their source text, separated by
/// a single space, so `1_000` and `1000` are different fingerprints: a change to how a literal is
/// spelled is a change to the source, and the conservative answer is the right one. The one
/// exception is a class, struct, interface or record, which contributes its HEADER rather than its
/// body - <see cref="Tokens"/> says why, and an <c>enum</c> is deliberately not in that exception.
/// </para>
/// <para>
/// <b>And a whole-file fingerprint beside them.</b> <see cref="OfFile"/> takes the same hash over
/// the complete token stream of a compilation unit. The per-unit values give the granularity a
/// review needs; the file value gives COMPLETENESS, because the unit enumeration is a whitelist and
/// what a whitelist does not list has no record at all. Both are in
/// <c>assurance.manifest.json</c> and rule J7 holds both to the tree.
/// </para>
/// <para>
/// <b>What is not, and by what mechanism.</b> Every piece of trivia: comments - including the
/// assurance annotation itself, which must not be part of what it describes - and all whitespace.
/// That is what makes the policy's requirement achievable: <c>dotnet format</c> must not
/// invalidate a review.
/// </para>
/// <para>
/// The mechanism is <see cref="SyntaxToken.Text"/>, and naming it matters. An earlier revision
/// called <c>WithoutTrivia()</c> on the declaration first and the class doc, the specification and
/// the register row all pointed at that call as the thing protecting reviews from a formatter. It
/// was doing nothing: <c>Text</c> is the token's own characters and never carries the trivia
/// around it, so the call could be deleted with every fingerprint unchanged and the whole suite
/// green. It was worse than dead - it was a decoy, because a later change from <c>Text</c> to
/// <c>ToFullString()</c> would have made it load-bearing, and nothing would have noticed it had
/// been removed in the meantime. The call is gone and
/// <c>AssuranceFingerprintTests.The_Exclusion_Of_Trivia_Is_Token_Text</c> pins the real mechanism
/// instead: it fingerprints a declaration that carries trivia on its tokens and asserts the stream
/// is the same, which fails under <c>ToFullString()</c>.
/// </para>
/// <para>
/// <b>The limit, stated rather than glossed.</b> Six hex characters are 24 bits. Against an
/// accidental edit that is a one-in-sixteen-million chance of collision per changed unit, which is
/// what the fingerprint is for: it answers <em>did this unit change since it was reviewed</em>.
/// It is not a collision-free identifier across units - a component with a few thousand units
/// expects same-value pairs by the birthday bound - and it is not a cryptographic commitment: a
/// party who can choose the code can find a preimage for a 24-bit prefix by brute force in
/// seconds. Detecting a hostile rewrite that preserves the prefix is the job of the git history
/// and the review event, not of this number.
/// </para>
/// </remarks>
internal static class AssuranceFingerprint
{
    /// <summary>The placeholder a developer writes and the generator replaces. "To Be Filled".</summary>
    internal const string ToBeFilled = "TBF";

    /// <summary>The number of hex characters a fingerprint carries.</summary>
    internal const int Width = 6;

    /// <summary>The fingerprint of a declaration.</summary>
    internal static string Of(SyntaxNode declaration) => OfTokenStream(TokenStream(declaration));

    /// <summary>
    /// The fingerprint of a WHOLE FILE: every token of its compilation unit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why a file needs one of its own.</b> The unit enumeration is a whitelist, and what a
    /// whitelist does not list is in no unit, no fingerprint and no manifest entry. A fourth
    /// adversarial round walked straight through the gap four times - an enum member, a type
    /// declaration header carrying a primary constructor, an
    /// <c>[assembly: InternalsVisibleTo("anything")]</c> that opens every internal type in
    /// <c>Broiler.VM.Runtime</c>, and an event field declaration - each with the suite green and
    /// <c>assurance.manifest.json</c> byte-unchanged. Widening the whitelist answers those four and
    /// not the fifth, because the defect is the whitelist and not its contents.
    /// </para>
    /// <para>
    /// This value answers COMPLETENESS instead: nothing in a covered file can change without
    /// something moving, whatever kind of declaration it is and whether or not the unit enumeration
    /// has a name for it. An assembly-level attribute is not a member of anything, and it is in
    /// here. The per-unit fingerprints stay, because they give the granularity a review needs -
    /// this one says only that the file is not what it was.
    /// </para>
    /// <para>
    /// <b>Comments are excluded by the same mechanism as everywhere else</b>, and this is the one
    /// place it has to be said out loud: the stream is built from <see cref="SyntaxToken.Text"/>,
    /// which is a token's own characters and never the trivia around it, so the generated header
    /// this system writes into every covered file and the two annotation lines above every relevant
    /// unit are not in the stream. Without that a file's fingerprint would depend on the header,
    /// the header would depend on the fingerprint, and no generation would ever be a fixed point.
    /// <c>AssuranceFingerprintTests.A_File_Fingerprint_Excludes_Comments_And_Covers_What_No_Unit_Can</c>
    /// asserts it, in both directions.
    /// </para>
    /// </remarks>
    internal static string OfFile(SyntaxTree tree) => Of(tree.GetRoot());

    /// <summary>
    /// The normalized form the fingerprint is taken over. Exposed because a hash nobody can
    /// inspect is a hash nobody can argue with: a test that shows which edits move the
    /// fingerprint has to be able to show what moved underneath it.
    /// </summary>
    internal static string TokenStream(SyntaxNode declaration) =>
        string.Join(
            " ",
            Tokens(declaration).Select(static token => token.Text));

    /// <summary>
    /// The tokens one unit's fingerprint is taken over: every token of the declaration, except
    /// that a class, struct, interface or record contributes its HEADER only.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why a type contributes its header.</b> The reviewable thing about
    /// <c>internal readonly struct VmReadBounds(ulong maxSectionCount, ulong maxDeclaredCount)</c>
    /// is the declaration line: the modifiers, the base list, and above all the primary
    /// constructor's parameter list, where permuting two parameters re-points every caller's
    /// argument at a different ceiling. That is round one's permuted-ceiling defeat, and it came
    /// back verbatim through a primary constructor because a type declaration was in no unit at
    /// all. The members below the header are units in their own right with fingerprints of their
    /// own, so folding them in here would make one annotation go stale every time any member moved
    /// and would say nothing the member's own fingerprint does not already say.
    /// </para>
    /// <para>
    /// <b>Why an enum does not.</b> An enum's members carry no executable code and are exempt from
    /// carrying annotations of their own - they are entries in one closed vocabulary, and the
    /// vocabulary is what a reviewer certifies. So an <c>enum</c> declaration's fingerprint is the
    /// WHOLE declaration, members and values included, and its one annotation is bound to the
    /// vocabulary rather than to the word <c>enum</c>. Adding, removing, renaming, reordering or
    /// revaluing a member moves it. <see cref="EnumDeclarationSyntax"/> is not a
    /// <see cref="TypeDeclarationSyntax"/>, so the case below reaches exactly the four kinds it
    /// names and the enum falls through to the whole-declaration default.
    /// </para>
    /// </remarks>
    internal static IEnumerable<SyntaxToken> Tokens(SyntaxNode declaration) => declaration switch
    {
        TypeDeclarationSyntax type => HeaderTokens(type),
        _ => declaration.DescendantTokens(),
    };

    /// <summary>
    /// A type declaration's header: everything up to its member list - attributes, modifiers, the
    /// keyword, the identifier, the type parameters, the primary constructor's parameter list, the
    /// base list and the constraint clauses.
    /// </summary>
    /// <remarks>
    /// The cut is the opening brace, which is a CHILD of the declaration and therefore reachable
    /// without knowing which optional parts a given declaration carries. A record with no body -
    /// <c>public sealed record Point(int X, int Y);</c> - has no brace, so the whole declaration is
    /// its header, which is the right answer for it.
    /// </remarks>
    private static IEnumerable<SyntaxToken> HeaderTokens(TypeDeclarationSyntax type) =>
        type.ChildNodesAndTokens()
            .TakeWhile(static child => !child.IsKind(SyntaxKind.OpenBraceToken))
            .SelectMany(static child => child.IsToken
                ? Enumerable.Repeat(child.AsToken(), 1)
                : child.AsNode()!.DescendantTokens());

    private static string OfTokenStream(string tokenStream)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(tokenStream));

        return Convert.ToHexString(digest)[..Width];
    }

    /// <summary>True for a value the generator may write into a Fingerprint field.</summary>
    internal static bool IsWellFormed(string value) =>
        value.Length == Width && value.All(static character =>
            character is >= '0' and <= '9' or >= 'A' and <= 'F');
}
