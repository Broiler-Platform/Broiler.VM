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

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The compile-time anchor into this assembly. It declares no contract and carries no behaviour.
/// </summary>
/// <remarks>
/// <para>
/// This assembly holds the JavaScript profile: the profile descriptor and its host capability
/// descriptors (JavaScriptProfile.cs), the verifier (JavaScriptVerifier.cs), the executor
/// (JavaScriptExecutor.cs), the value model (JsValue.cs) and the standard library carried across
/// JsRealm.cs and its twenty-three partials. Nothing in this component may be described as
/// reviewed, accepted or supported.
/// </para>
/// <para>
/// The fifteen limit defaults and fifteen hard maxima are declared in JavaScriptProfile.cs
/// (Defaults and Maxima) and are used by the descriptor built there. Decision JSD-0004 records
/// why the vector holds the values it holds.
/// </para>
/// <para>
/// <i>(Corrected 2026-09-08. The first paragraph read "JS-0 creates project shells and no product
/// code. This assembly holds no descriptor, no verifier, no executor, no value model and no
/// standard library", and the second read that the limit vector was "recorded in decision JSD-0004
/// rather than here, because JS-0's own delivery order states that this milestone lands no product
/// code ... JS-1 lands them with the descriptor that uses them". Both were true at JS-0 and neither
/// has been true since JS-1; describing this assembly as a shell understates the component that
/// runs test262, which this repository treats as the same defect as overstating one. The superseded
/// readings are quoted rather than deleted so the chain stays readable.)</i>
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=None; Resources=0; Fingerprint=E1BC8B
// Broiler-Human:        PENDING
internal sealed class AssemblyMarker
{
}
