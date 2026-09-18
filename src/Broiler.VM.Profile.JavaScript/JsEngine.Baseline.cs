// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           1
// Human-reviewed:   0/3
// IP risk:          None
// Security risk:    Critical
// Criteria:         3/3
// Resource impact:  5/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <content>
/// The engine's half of the baseline native form: entering emitted code, and the page it runs from.
/// </content>
/// <remarks>
/// <b>IT IS A SEPARATE FILE BECAUSE IT IS THE ONE PLACE THE ENGINE LEAVES MANAGED CODE.</b> Everything
/// the form executes is the dispatch loop's own body; what this half adds is the call into the
/// mapped code, the thread slot that call sets, and the mapping itself - the small surface a reader
/// auditing where emitted code is entered has to read, kept apart from the seven thousand lines of
/// semantics it shares.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=5; Fingerprint=7BBE7E
// Broiler-Falsified-If: emitted code is entered from anywhere but this file
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{
    /// <summary>Runs one activation of a unit by entering its emitted code.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE ENTRY RUNS IN MANAGED CODE BEFORE ANY EMITTED FRAME EXISTS.</b> The dispatch loop's own
    /// prologue builds or borrows the stack and the scopes, finishes a normal resumption and raises an
    /// abrupt one - so a <c>throw()</c> or a <c>return()</c> into a suspended body lands in its regions
    /// or propagates out of this method exactly as the interpreter's would, and the emitted code is
    /// entered only at an instruction the managed side chose.
    /// </para>
    /// <para>
    /// <b>THE ACTIVATION IS ROOTED HERE FOR THE WHOLE EMITTED CALL.</b> The frame handed to the emitted
    /// code carries a table address and a cookie and nothing the collector traces; the activation it
    /// names is kept alive by this frame and by the thread slot it sets, and the slot is restored when
    /// the call returns, whether it returned or not, so a nested activation's slot never outlives it.
    /// </para>
    /// <para>
    /// <b>A caught exception is raised again with a plain throw.</b> Capturing and replaying its
    /// dispatch information would append a trace per native level to an exception that crosses many,
    /// and no guest can observe the trace.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=5; Fingerprint=262CBB
    // Broiler-Falsified-If: emitted code runs while its activation or its page is unreachable from a managed root, or this answers a value for a status other than exit
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    private unsafe JsValue RunNative(
        JsProgram program,
        int unitIndex,
        JsEnvironment? environment,
        JsValue thisValue,
        JsValue[] actualArguments,
        JsScriptFunction? self,
        JsValue newTarget,
        JsCell? thisBinding,
        JsFrame? frame)
    {
        var previous = JsNativeActivation.Current;
        JsNativeActivation.Current = null;
        try
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "emitted code must be executed via Broiler.VM machinecode profile");
        }
        finally
        {
            JsNativeActivation.Current = previous;
        }
    }

    internal object? NativePageOf(JsProgram program) => null;

    /// <summary>
    /// Refuses a guest-loaded program whose output form is not this instance's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=EC8414
    // Broiler-Falsified-If: a guest-loaded program of the other form, of another architecture, or of the numeric manifest runs in a baseline instance
    // Broiler-Human:        PENDING
    private void RequireInstanceForm(JsProgram loaded)
    {
        if (JsNativeExecution.CarriesEmittedCode(loaded) != nativeForm ||
            (nativeForm &&
                (loaded.NativeArchitecture != JsNativeExecution.HostArchitecture ||
                    string.Equals(
                        loaded.ManifestId, JsNumericManifest.ManifestId, System.StringComparison.Ordinal))))
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect,
                "a guest-loaded program's output form differs from its instance's form");
        }

        if (nativeForm)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "emitted code must be executed via Broiler.VM machinecode profile");
        }
    }
}
