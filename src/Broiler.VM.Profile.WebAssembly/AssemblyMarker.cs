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

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The compile-time anchor into this assembly. It declares no contract and carries no behaviour.
/// </summary>
/// <remarks>
/// <para>
/// WA-0 stood up the boundary, the identity and the assurance floor and landed no product code;
/// four milestones since have landed code into this assembly, and the universal bytecode
/// programme's milestone UBC-4 has replaced its execution path. What it holds beside this type is
/// an identity, a decoder, a validator, a translator of a module into universal bytecode, and the
/// universal bytecode family the translation runs under - its declaration and registration, its
/// instruction table, its verifier hook, its handlers with the store an instance runs against, and
/// the numeric arms the family's reference handler answers with - and nothing in this component may
/// be described as validated, accepted, supported or published.
/// <i>(Corrected 2026-09-08. This paragraph read "What this assembly holds beside this type is an
/// identity, a descriptor, a verifier that refuses every artifact and an executor that refuses
/// every step. There is no decoder, no validator, no value model, no store and no interpreter."
/// Every one of those five absences was true at WA-0 and is false in this checkout, and the
/// executor refuses nothing on that ground; understating the assembly that decodes, validates and
/// runs a guest payload is the same defect as overstating it, which is why the superseded reading
/// is quoted here rather than removed.)</i>
/// <i>(Corrected 2026-09-25. The corrected paragraph read "What it holds beside this type is an
/// identity, a descriptor, a decoder, a validator, a verifier, a value model, a store, an
/// interpreter and an executor". Milestone UBC-4 retired the descriptor, the verifier, the value
/// model, the interpreter and the executor once the composition roots translated first, and the
/// store became the family's instance state.)</i>
/// </para>
/// <para>
/// The nine-row value and frame decision is recorded under docs/decisions/, where decision
/// WAD-0003 says where each row stands now that the value model it was written in is retired. The
/// store reading, the entry-point encoding and the refusable-retention amendment are open, and each
/// is recorded in the roadmap under docs/ rather than here.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E1BC8B
// Broiler-Human:        PENDING
internal sealed class AssemblyMarker
{
}
