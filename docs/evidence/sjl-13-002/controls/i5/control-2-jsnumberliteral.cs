// CONTROL OF BUNDLE sjl-13-002 PART D, RULE I5 (a scratch copy, never merged): the deleted record JsNumberLiteral,
// whose members are all positional, declared again, the second control revision 2026-11-13 of the rule names.
namespace Broiler.VM.Profile.JavaScript.Compiler;

internal sealed record JsNumberLiteral(SliceSourceSpan Span, double Value, bool IsLegacyOctal);
