// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           0
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The stack this component compiles on, declared here rather than inherited from whoever called.
/// </summary>
/// <remarks>
/// <para>
/// <b>The profile already took this decision for execution and never took it for compilation.</b>
/// <c>JsExecution</c> runs a guest invocation on a thread whose stack it declares, and states why:
/// otherwise the call-depth ceiling means a different thing on every host, and a stack overflow is
/// the one failure the CLR cannot turn into an exception, so no meter and no ceiling can report it
/// afterwards. Every word of that applies to the walk over a syntax tree, which recurses once per
/// node on a spine as long as the source. It ran on whatever stack the caller happened to have -
/// on Windows, the megabyte a process gives its main thread.
/// </para>
/// <para>
/// <b>What it is not: a fix on its own.</b> Raising a stack moves a cliff rather than removing
/// one. What makes the answer a refusal at every size is
/// <see cref="SliceParseOptions.MaximumTreeDepth"/>; what makes that bound large enough to cost no
/// real program is this stack. The two are one decision and neither half stands alone.
/// </para>
/// <para>
/// <b>A thread per compilation rather than one held open</b>, for the reason the execution side
/// gives: a parked thread is a resource held while doing nothing, and a fresh one costs a fraction
/// of a millisecond against a compile that costs milliseconds at least. It also starts every
/// compilation with the same stack whatever the previous one did, which is what makes the bound's
/// derivation mean the same thing on the second call as on the first.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=2C7737
// Broiler-Falsified-If: a compilation walks a syntax tree on the caller's stack
// Broiler-Human:        PENDING
internal static class CompilationStack
{
    /// <summary>
    /// How much native stack one compilation gets.
    /// </summary>
    /// <remarks>
    /// The same figure the guest gets, and deliberately the same: two numbers would be two things
    /// to re-derive whenever either walk grew a frame, and the two walks are comparable - one
    /// activation per tree node against one per guest call. <c>MaximumTreeDepth</c> is derived
    /// against this figure and cites it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7F3B2D
    // Broiler-Human:        PENDING
    internal const int CompileStackBytes = 16 * 1024 * 1024;

    /// <summary>Runs <paramref name="compile"/> on a thread whose stack this profile declared.</summary>
    /// <remarks>
    /// Whatever the compilation raises is carried back and rethrown here, so a caller sees the
    /// exception it would have seen had the work run on its own thread. Nothing about the result
    /// crosses a thread boundary mutably: the compilation is produced and then read.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=97F713
    // Broiler-Falsified-If: the compilation runs on the calling thread, or an exception it raised does not reach the caller
    // Broiler-Human:        PENDING
    internal static T Run<T>(System.Func<T> compile)
    {
        var completed = default(T)!;
        System.Runtime.ExceptionServices.ExceptionDispatchInfo? raised = null;

        var worker = new System.Threading.Thread(
            () =>
            {
                try
                {
                    completed = compile();
                }
                catch (System.Exception failure)
                {
                    raised = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure);
                }
            },
            CompileStackBytes)
        {
            IsBackground = true,
            Name = "broiler-js-compile",
        };

        worker.Start();
        worker.Join();
        raised?.Throw();
        return completed;
    }
}
