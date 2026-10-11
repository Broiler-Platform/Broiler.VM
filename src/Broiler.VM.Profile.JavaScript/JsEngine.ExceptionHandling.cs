// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Critical
// Criteria:         2/2
// Resource impact:  5/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>
    /// The innermost <c>finally</c> region of <paramref name="unit"/> covering <paramref name="pc"/>.
    /// </summary>
    /// <remarks>
    /// The regions of one unit are recorded innermost-first by the lowering, so the first match in
    /// order is the innermost - the same property <see cref="TryFindHandler"/> relies on. What
    /// differs is only that a <c>catch</c> region is passed over rather than taken.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=C8F9E6
    // Broiler-Human:        PENDING
    private static bool TryFindFinally(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit &&
                candidate.Kind == JsFormat.HandlerKind.Finally &&
                pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
    }

    /// <summary>A throw's landing in a region: the scopes the region is outside of dropped, the stack cut to its height and the value pushed there, and the region's handler next.</summary>
    /// <remarks>
    /// <b>ONE TEXT FOR THE TWO PLACES A THROW LANDS</b>: the dispatch loop's catch, where an instruction's own
    /// exception meets its unit's region, and the value form's status chain, where an exception a direct call
    /// answered meets the caller's region at the call without being thrown again (JSD-0035 section 8, stage
    /// JSV-4).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C47FDB
    // Broiler-Falsified-If: a landing keeps a scope deeper than its region's, leaves the stack at any height but the region's plus the value, or resumes anywhere but the region's handler
    // Broiler-Human:        PENDING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Land(List<JsEnvironment> scopes, JsValue[] stack, ref int sp, ref int pc, JsRegion region, JsValue value)
    {
        while (scopes.Count > region.ScopeDepth + 1)
        {
            scopes.RemoveAt(scopes.Count - 1);
        }

        sp = (int)region.StackHeight;
        stack[sp++] = value;
        pc = (int)region.Handler;
    }

    /// <summary>
    /// Lands an exception at a value-form activation's instruction where the interpreter's filter would land
    /// it, with no throw: the status chain's landing (JSD-0035 section 8, stage JSV-4).
    /// </summary>
    /// <remarks>
    /// <b>THE SAME SEARCH AND THE SAME LANDING THE DISPATCH LOOP MAKES</b>: a guest exception only, the unit's
    /// innermost region covering the instruction, and <see cref="Land"/>. An exception it answers false for
    /// lands nowhere in the unit, and goes on as a status.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=7547F6
    // Broiler-Falsified-If: it lands an exception the interpreter's filter would not land at the same instruction, or lands one elsewhere than the interpreter would
    // Broiler-Human:        PENDING
    internal static bool TryLand(JsNativeActivation act, int pc, Exception raised)
    {
        if (raised is not JsThrow thrown || !TryFindHandler(act.Program, act.UnitIndex, pc, out var region))
        {
            return false;
        }

        var sp = act.Sp;
        var landing = pc;
        Land(act.Scopes, act.Stack, ref sp, ref landing, region, thrown.Value);
        act.Sp = sp;
        act.Pc = landing;
        act.Landed = true;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=EAA946
    // Broiler-Human:        PENDING
    private static bool TryFindHandler(JsProgram program, int unit, int pc, out JsRegion region)
    {
        foreach (var candidate in program.Regions)
        {
            if (candidate.Unit == (uint)unit && pc >= candidate.TryStart && pc < candidate.TryEnd)
            {
                region = candidate;
                return true;
            }
        }

        region = default;
        return false;
    }
}
