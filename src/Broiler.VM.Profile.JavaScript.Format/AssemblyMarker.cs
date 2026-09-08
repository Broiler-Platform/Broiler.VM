// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           0
// Human-reviewed:   0/1
// IP risk:          None
// Security risk:    None
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       1
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The compile-time anchor into this assembly. It declares no contract and carries no behaviour.
/// </summary>
/// <remarks>
/// <para>
/// JS-0 created project shells and no product code, so this type was minted for the same reason
/// ADR 0001 gave the core's markers at VM-0: an assembly with no type at all contributes no unit
/// to the assurance scan, and a covered assembly that contributes nothing is indistinguishable
/// from one the scan never reached. That premise is spent - this assembly contributes hundreds of
/// annotated units - and the type is kept only because removing it is a graph change nothing here
/// needs.
/// </para>
/// <para>
/// What this assembly holds is the JavaScript profile's bytecode format - magic, format version,
/// section framing, constant pool, code, exception regions and position tables, at format version
/// 1 since JS-1 - and beside it the other things a lowering and an executor must agree on while
/// neither depends on the other: the optional-surface identities, the numeric feature manifest,
/// the native frame ABI and its emitter, and the regular-expression matcher with its own charge
/// callback and ceilings.
/// </para>
/// <para>
/// <i>(Corrected 2026-09-08. The second paragraph read "What this assembly will hold is the
/// JavaScript profile's bytecode format - magic, format version, section framing, constant pool,
/// code, exception regions and position tables - and nothing else. JS-1 defines format version 1
/// in it." The future tense was true at JS-0; the checkout is many milestones past JS-1, the
/// format is at version 1, and "nothing else" was already false of the twelve files beside this
/// one. The first paragraph's premise was corrected on the same date for the same reason. The
/// superseded readings are quoted rather than deleted so the chain stays readable.)</i>
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E1BC8B
// Broiler-Human:        PENDING
internal sealed class AssemblyMarker
{
}
