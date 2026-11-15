// CONTROL OF BUNDLE sjl-13-002 PART D, RULE I5 (a scratch copy, never merged): the deleted type JsMemberExpression
// declared again, with its member NameSpan, as revision 2026-11-13 of the rule pins the control.
namespace Broiler.VM.Profile.JavaScript.Compiler;

internal sealed record JsMemberExpression(SliceSourceSpan Span, string Name)
{
    public SliceSourceSpan NameSpan { get; init; }
}
