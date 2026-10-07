// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   1
// Annotated:        1/1
// Exempt:           6
// Human-reviewed:   0/1
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       1
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Abstractions;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// What a composition decides about the JavaScript profile it builds, in full, for
/// <see cref="JavaScriptProfile.DescriptorComposing"/> (JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>Every choice the narrower doors make one at a time, and the one only this door takes: the
/// internationalization data.</b> Each property is fixed before a realm exists, which is why it is a
/// door's argument rather than something a runtime could change.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B32F8A
// Broiler-Human:        PENDING
public sealed class JsComposition
{
    /// <summary>
    /// The optional surfaces admitted, exactly; or nothing for every surface this composition can
    /// build, which includes <see cref="Format.JsSurfaces.Intl"/> only when <see cref="IntlData"/> is set.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A188C0
    // Broiler-Human:        PENDING
    public System.Collections.Generic.IReadOnlyList<VmFeatureManifestId>? Surfaces { get; init; }

    /// <summary>The realm embedder, as <see cref="JavaScriptProfile.DescriptorHostingRealms"/> takes one, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EB2FB0
    // Broiler-Human:        PENDING
    public IJsHostSurface? HostSurface { get; init; }

    /// <summary>The native emitter a native payload is re-emitted with, as <see cref="JavaScriptProfile.DescriptorReEmittingWith"/> takes one, or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0931B2
    // Broiler-Human:        PENDING
    public Format.IJsNativeEmitter? NativeEmitter { get; init; }

    /// <summary>Whether value-form instances run under handle-stress, as <see cref="JavaScriptProfile.DescriptorUnderHandleStress"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=ADFC37
    // Broiler-Human:        PENDING
    public bool HandleStress { get; init; }

    /// <summary>Whether host drains sweep finalization registries, as <see cref="JavaScriptProfile.DescriptorSweepingFinalization"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A06406
    // Broiler-Human:        PENDING
    public bool SweepsFinalization { get; init; }

    /// <summary>
    /// The internationalization data, which <see cref="Format.JsSurfaces.Intl"/> is built from; a
    /// composition admitting that surface must set it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0406B2
    // Broiler-Human:        PENDING
    public Format.IJsIntlData? IntlData { get; init; }
}
