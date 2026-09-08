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

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The compile-time anchor into this assembly. It declares no contract and carries no behaviour.
/// </summary>
/// <remarks>
/// <para>
/// This assembly holds the tokenizer, the syntax tree, the static semantics and the lowering for
/// the slice, wide and numeric manifests, together with the x86-64 and arm64 machine-code backends
/// that attach at the finished bytecode. JS-1 wrote the hand lowering for the slice manifest and
/// JS-3b wrote the general one.
/// <i>(Corrected 2026-09-08. This paragraph read "JS-0 creates project shells and no product code.
/// No tokenizer, no syntax tree, no static semantics and no lowering exist here; JS-1 writes a hand
/// lowering for the slice manifest and JS-3b writes the general one." All four absences were true
/// at JS-0 and none has been true since JS-3b; describing this assembly as a shell understates the
/// component that compiles the whole wide surface and emits machine code, which this repository
/// treats as the same defect as overstating one. The superseded reading is quoted rather than
/// deleted so the chain stays readable.)</i>
/// </para>
/// <para>
/// This assembly is a sibling of the profile and never a part of it. A composition that executes
/// precompiled artifacts contains a format, a verifier and an interpreter and no lowering at all,
/// which is a property of the reference set rather than of a build switch.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E1BC8B
// Broiler-Human:        PENDING
internal sealed class AssemblyMarker
{
}
