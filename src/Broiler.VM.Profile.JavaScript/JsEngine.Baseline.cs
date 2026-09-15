// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

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
// Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=5; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=5; Fingerprint=TBF
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
        var page = NativePageOf(program) ??
            throw new JsAbort(JsAbortKind.InternalDefect, "emitted code could not be mapped");

        var act = new JsNativeActivation(
            this, program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
            thisBinding, frame);

        _ = ExecuteCore<JsNativeEntry>(
            program, unitIndex, environment, thisValue, actualArguments, self, newTarget,
            thisBinding, frame, act);

        JsBaselineFrame native;
        native.Handlers = JsBaselineHandlers.Table;
        native.Cookie = act.Cookie;

        var previous = JsNativeActivation.Current;
        JsNativeActivation.Current = act;
        int status;

        try
        {
            var entry = (delegate* unmanaged<JsBaselineFrame*, int, int>)page.At(
                program.NativeSymbols[unitIndex].Offset);

            status = entry(&native, act.Pc);
        }
        finally
        {
            JsNativeActivation.Current = previous;
            System.GC.KeepAlive(page);
            System.GC.KeepAlive(act);
        }

        return status switch
        {
            (int)JsBaselineStatus.Exit => act.Result,
            (int)JsBaselineStatus.Threw => throw act.Pending!,
            _ => throw new JsAbort(
                JsAbortKind.InternalDefect, "emitted code answered status " + status),
        };
    }

    /// <summary>The armed mapping of <paramref name="program"/>'s emitted code, mapped on first use.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE PAGE BELONGS TO THE PROGRAM AND NOT TO THE INSTANCE.</b> Closures, suspended frames,
    /// module graphs and queued jobs all hold the program they run, so a page owned by the program
    /// lives exactly as long as anything could still enter it - and a program a guest loads with
    /// <c>eval</c> or an import gets its own page that goes when it does, instead of one mapping per
    /// load kept for the life of the instance.
    /// </para>
    /// <para>
    /// <b>Two threads may map the same program at once and one of them wins.</b> The winner's page
    /// is published atomically; the loser releases its own and uses the winner's, so a program is
    /// never seen with two pages and a published page is never replaced.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=4; Fingerprint=TBF
    // Broiler-Falsified-If: a page this returns is not armed, or a program's published page is replaced or released while the program is reachable
    // Broiler-Human:        PENDING
    internal JsNativePage? NativePageOf(JsProgram program)
    {
        var published = program.NativePage;

        if (published is not null)
        {
            return published;
        }

        var mapped = JsNativePage.TryMap(program.NativeCode);

        if (mapped is null)
        {
            return null;
        }

        if (!mapped.Arm())
        {
            mapped.Dispose();
            return null;
        }

        var raced = System.Threading.Interlocked.CompareExchange(ref program.NativePage, mapped, null);

        if (raced is not null)
        {
            mapped.Dispose();
            return raced;
        }

        return mapped;
    }

    /// <summary>
    /// Refuses a guest-loaded program whose output form is not this instance's, and maps one that is.
    /// </summary>
    /// <remarks>
    /// <b>A NESTED LOAD IS THE ONE ROUTE BY WHICH A SECOND FORM COULD ENTER A RUNNING INSTANCE</b>,
    /// because the composition's provider compiled it after instantiation checked the first. A
    /// mismatch is this profile's defect or a provider's, never the guest's, so it is an internal
    /// defect rather than a language error. The page is mapped here as well, so a program that
    /// cannot be mapped is refused where it was loaded rather than at its first call.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=TBF
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

        if (nativeForm && NativePageOf(loaded) is null)
        {
            throw new JsAbort(JsAbortKind.InternalDefect, "emitted code could not be mapped");
        }
    }
}
