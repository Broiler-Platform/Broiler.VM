// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           8
// Human-reviewed:   0/4
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// Mapped executable memory holding emitted code for native execution.
/// Delegates mapping and arming to the configured native mapper without declaring platform invokes.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B69A56
// Broiler-Human:        PENDING
public sealed unsafe class JsNativePage : System.IDisposable
{
    private readonly System.IDisposable? handle;
    private readonly System.Func<bool> arm;
    private readonly System.Func<uint, nint> at;
    private readonly System.Func<uint, nint> entry;

    public static System.Func<System.ReadOnlySpan<byte>, JsNativePage?>? Mapper { get; set; }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=ABC23D
    // Broiler-Human:        PENDING
    public static JsNativePage? TryMap(System.ReadOnlySpan<byte> code) => Mapper?.Invoke(code);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9AE630
    // Broiler-Human:        PENDING
    public JsNativePage(
        System.IDisposable? handle,
        System.Func<bool> arm,
        System.Func<uint, nint> at,
        System.Func<uint, nint> entry)
    {
        this.handle = handle;
        this.arm = arm;
        this.at = at;
        this.entry = entry;
    }

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
