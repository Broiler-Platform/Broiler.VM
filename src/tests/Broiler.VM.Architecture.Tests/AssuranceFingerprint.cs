using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;

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
/// spelled is a change to the source, and the conservative answer is the right one.
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
    /// The normalized form the fingerprint is taken over. Exposed because a hash nobody can
    /// inspect is a hash nobody can argue with: a test that shows which edits move the
    /// fingerprint has to be able to show what moved underneath it.
    /// </summary>
    internal static string TokenStream(SyntaxNode declaration) =>
        string.Join(
            " ",
            declaration
                .DescendantTokens()
                .Select(static token => token.Text));

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
