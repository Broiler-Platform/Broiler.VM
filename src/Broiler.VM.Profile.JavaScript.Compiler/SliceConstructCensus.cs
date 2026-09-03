// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   7
// Annotated:        7/7
// Exempt:           0
// Human-reviewed:   0/7
// IP risk:          None
// Security risk:    High
// Criteria:         3/3
// Resource impact:  2/10 max
// Unverified:       7
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>What one census run found in one body of JavaScript.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3D1168
// Broiler-Human:        PENDING
public sealed record SliceCensus(
    int FilesRead,
    int FilesParsed,
    int FilesCompiled,
    int FilesThatFaulted,
    System.Collections.Generic.IReadOnlyDictionary<SliceConstructKind, int> Occurrences,
    System.Collections.Generic.IReadOnlyDictionary<SliceConstructKind, int> Files,
    System.Collections.Generic.IReadOnlyDictionary<SliceSourceDiagnosticCode, int> ParseFailures);

/// <summary>
/// Counts what real JavaScript is made of, so a manifest decision is a measurement.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because the roadmap's remaining scope is decided by guesses otherwise.</b>
/// JS-4 buys an object model, JS-5 an executor with calls, JS-6 a standard library, and the order
/// and the size of each is currently argued from what a JavaScript engine is usually assumed to
/// need. A parser that can read the language can instead answer what a named body of code
/// actually contains, ranked, and the ranking is a fact a plan can be held to.
/// </para>
/// <para>
/// <b>It counts two things per construct, and the second is the one that ranks honestly.</b>
/// Occurrences say how often a construct appears, which one enormous generated file can dominate.
/// File counts say how many files could not be compiled without it, which is what "this manifest
/// needs it" means. A tool reporting only the first would rank a minified bundle's comma operators
/// above every object literal in the corpus.
/// </para>
/// <para>
/// <b>It reads nothing but the text it is given.</b> No suite metadata, no harness protocol, no
/// network: this is a census of source, and the conformance oracle roadmap section 14 specifies is
/// a different instrument with a pinned revision, a self-check and a ratchet, none of which this
/// has or claims.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=C0354E
// Broiler-Falsified-If: a construct present in a source is not counted, or a count includes a construct the source does not contain
// Broiler-Human:        PENDING
public static class SliceConstructCensus
{
    /// <summary>The options a census parses under.</summary>
    /// <remarks>
    /// The largest bound the parser can honour, because a census wants to read the file rather
    /// than to enforce the slice's own conservative default - and a file the bound refuses is
    /// reported as a parse failure rather than silently counted as empty.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C01EB6
    // Broiler-Human:        PENDING
    public static SliceParseOptions Options => new(
        SliceGoal.Script,
        allowTopLevelAwait: true,
        SliceParseOptions.MaximumSupportedNestingDepth);

    /// <summary>Reads every source in <paramref name="sources"/> and counts what they contain.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=B3C49F
    // Broiler-Falsified-If: a source that parses contributes no counts, or a source that does not parse is counted as containing nothing
    // Broiler-Human:        PENDING
    public static SliceCensus Take(System.Collections.Generic.IEnumerable<string> sources)
    {
        System.ArgumentNullException.ThrowIfNull(sources);

        var occurrences = new System.Collections.Generic.Dictionary<SliceConstructKind, int>();
        var files = new System.Collections.Generic.Dictionary<SliceConstructKind, int>();
        var failures = new System.Collections.Generic.Dictionary<SliceSourceDiagnosticCode, int>();
        var read = 0;
        var parsed = 0;
        var compiled = 0;
        var faulted = 0;

        foreach (var source in sources)
        {
            read++;

            try
            {
                var tokenizer = new SliceTokenizer(source);
                var tokens = tokenizer.Tokenize();

                if (tokenizer.Diagnostics.Count > 0)
                {
                    Bump(failures, tokenizer.Diagnostics[0].Code);
                    continue;
                }

                var parser = new SliceParser(tokens, Options);
                var program = parser.ParseProgram();

                if (parser.Diagnostics.Count > 0)
                {
                    Bump(failures, parser.Diagnostics[0].Code);
                    continue;
                }

                parsed++;

                var here = new System.Collections.Generic.HashSet<SliceConstructKind>();
                Walk(program, occurrences, here);

                foreach (var kind in here)
                {
                    Bump(files, kind);
                }

                // A file with no construct outside the manifest is a file this profile could compile
                // today, which is the census's headline number and is usually zero.
                if (here.Count == 0)
                {
                    compiled++;
                }
            }
            catch (System.Exception)
            {
                // A FAULT IS COUNTED AND NEVER SWALLOWED. The front end's contract is that it
                // refuses rather than throws, so an exception escaping it is a defect in the front
                // end and not a property of the source - but a census that died on the first one
                // measured nothing at all, which is how a lone surrogate in one test262 file cost
                // a whole run. It is counted here and reported beside the totals so the number is
                // visible; it is not turned into a parse failure, because those are two different
                // things and only one of them is a bug in this component.
                faulted++;
            }
        }

        return new SliceCensus(read, parsed, compiled, faulted, occurrences, files, failures);
    }

    /// <summary>Walks a whole tree, counting every construct node it meets.</summary>
    /// <remarks>
    /// The walk is over the tree rather than over the validator's diagnostics, deliberately. The
    /// validator stops reporting after the first tokenizing or parse failure and its subject is
    /// one program's admissibility; a census wants every occurrence in a file that parsed, which
    /// is a different question over the same nodes.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=3DAB0E
    // Broiler-Falsified-If: a node reachable in the tree is not visited
    // Broiler-Human:        PENDING
    private static void Walk(
        SliceNode node,
        System.Collections.Generic.Dictionary<SliceConstructKind, int> occurrences,
        System.Collections.Generic.HashSet<SliceConstructKind> here)
    {
        switch (node)
        {
            case SliceProgram program:
                foreach (var statement in program.Body)
                {
                    Walk(statement, occurrences, here);
                }

                break;

            case SliceConstructStatement construct:
                Record(construct.Kind, occurrences, here);

                foreach (var child in construct.Children)
                {
                    Walk(child, occurrences, here);
                }

                break;

            case SliceConstructExpression construct:
                Record(construct.Kind, occurrences, here);

                foreach (var child in construct.Children)
                {
                    Walk(child, occurrences, here);
                }

                break;

            case SliceVariableStatement declaration:
                foreach (var declarator in declaration.Declarators)
                {
                    Walk(declarator, occurrences, here);
                }

                break;

            case SliceDeclarator declarator:
                if (declarator.Initialiser is not null)
                {
                    Walk(declarator.Initialiser, occurrences, here);
                }

                break;

            case SliceExpressionStatement statement:
                Walk(statement.Expression, occurrences, here);
                break;

            case SliceBlockStatement block:
                foreach (var statement in block.Body)
                {
                    Walk(statement, occurrences, here);
                }

                break;

            case SliceIfStatement branch:
                Walk(branch.Test, occurrences, here);
                Walk(branch.Consequent, occurrences, here);

                if (branch.Alternate is not null)
                {
                    Walk(branch.Alternate, occurrences, here);
                }

                break;

            case SliceWhileStatement loop:
                Walk(loop.Test, occurrences, here);
                Walk(loop.Body, occurrences, here);
                break;

            case SliceDoWhileStatement loop:
                Walk(loop.Body, occurrences, here);
                Walk(loop.Test, occurrences, here);
                break;

            case SliceForStatement loop:
                if (loop.Initialiser is not null)
                {
                    Walk(loop.Initialiser, occurrences, here);
                }

                if (loop.Test is not null)
                {
                    Walk(loop.Test, occurrences, here);
                }

                if (loop.Update is not null)
                {
                    Walk(loop.Update, occurrences, here);
                }

                Walk(loop.Body, occurrences, here);
                break;

            case SliceUnaryExpression unary:
                Walk(unary.Operand, occurrences, here);
                break;

            case SliceBinaryExpression binary:
                Walk(binary.Left, occurrences, here);
                Walk(binary.Right, occurrences, here);
                break;

            case SliceLogicalExpression logical:
                Walk(logical.Left, occurrences, here);
                Walk(logical.Right, occurrences, here);
                break;

            case SliceConditionalExpression conditional:
                Walk(conditional.Test, occurrences, here);
                Walk(conditional.WhenTrue, occurrences, here);
                Walk(conditional.WhenFalse, occurrences, here);
                break;

            case SliceAssignmentExpression assignment:
                Walk(assignment.Target, occurrences, here);
                Walk(assignment.Value, occurrences, here);
                break;

            default:
                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BF56B4
    // Broiler-Human:        PENDING
    private static void Record(
        SliceConstructKind kind,
        System.Collections.Generic.Dictionary<SliceConstructKind, int> occurrences,
        System.Collections.Generic.HashSet<SliceConstructKind> here)
    {
        Bump(occurrences, kind);
        here.Add(kind);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=4F09C6
    // Broiler-Human:        PENDING
    private static void Bump<T>(System.Collections.Generic.Dictionary<T, int> into, T key)
        where T : notnull =>
        into[key] = into.TryGetValue(key, out var count) ? count + 1 : 1;
}
