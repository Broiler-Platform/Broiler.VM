namespace Broiler.VM.Composition.PolyglotCli;

/// <summary>What one run did, in the terms a caller can act on.</summary>
/// <param name="Status">Which of this host's answers came back.</param>
/// <param name="Value">The completion value, rendered, or empty where the run produced none.</param>
/// <param name="Detail">One line a reader can act on. Empty on a clean completion.</param>
/// <param name="Diagnostics">
/// Every refusal, in the order the front end produced them. Empty unless a source was refused.
/// </param>
/// <remarks>
/// <b>ONE RESULT TYPE FOR TWO PROFILES, AND IT CARRIES NOTHING EITHER PROFILE OWNS.</b> A status,
/// a rendered value, a line of prose and a list of lines: nothing here is a JavaScript completion
/// or a WebAssembly trap, because a type that named either would make the report of one lane
/// depend on the vocabulary of the other. Each lane renders its own payloads into these four
/// fields, and the reporting code above them cannot tell which lane it is printing.
/// </remarks>
internal sealed record RunResult(
    RunStatus Status,
    string Value,
    string Detail,
    IReadOnlyList<string> Diagnostics);
