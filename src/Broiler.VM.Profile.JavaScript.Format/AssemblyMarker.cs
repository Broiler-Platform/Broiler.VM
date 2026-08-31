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
/// JS-0 creates project shells and no product code, so this type exists for the same reason
/// ADR 0001 gave the core's markers at VM-0: an assembly with no type at all contributes no unit
/// to the assurance scan, and a covered assembly that contributes nothing is indistinguishable
/// from one the scan never reached.
/// </para>
/// <para>
/// What this assembly will hold is the JavaScript profile's bytecode format - magic, format
/// version, section framing, constant pool, code, exception regions and position tables - and
/// nothing else. JS-1 defines format version 1 in it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E1BC8B
// Broiler-Human:        PENDING
internal sealed class AssemblyMarker
{
}
