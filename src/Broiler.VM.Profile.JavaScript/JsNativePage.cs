// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           3
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// Mapped executable memory holding emitted code for native execution.
/// Delegates mapping and arming to the configured native mapper without declaring platform invokes.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=576F3B
// Broiler-Human:        PENDING
public sealed unsafe class JsNativePage(IDisposable? handle, Func<bool> arm, Func<uint, nint> at, Func<uint, nint> entry) : IDisposable
{
    public static Func<ReadOnlySpan<byte>, JsNativePage?>? Mapper { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7644B6
    // Broiler-Human:        PENDING
    public static JsNativePage? TryMap(ReadOnlySpan<byte> code) => Mapper?.Invoke(code);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4F8A61
    // Broiler-Human:        PENDING
    public bool Arm() => arm();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E8B544
    // Broiler-Human:        PENDING
    public nint At(uint offset) => at(offset);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1715C3
    // Broiler-Human:        PENDING
    public delegate* unmanaged<JsNativeFrame*, int> Entry(uint offset) =>
        (delegate* unmanaged<JsNativeFrame*, int>)entry(offset);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2713F5
    // Broiler-Human:        PENDING
    public void Dispose() => handle?.Dispose();
}
