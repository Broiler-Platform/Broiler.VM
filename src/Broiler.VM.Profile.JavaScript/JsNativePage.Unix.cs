// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           1
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Critical
// Criteria:         11/11
// Resource impact:  4/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <content>
/// The Unix half of the one place that makes memory executable.
/// </content>
/// <remarks>
/// <b>THE ANONYMOUS-MAPPING FLAG IS LINUX'S AND IS NOT PORTABLE, AND THAT IS STATED RATHER THAN
/// PAPERED OVER.</b> The value written below is the one Linux uses; Apple's differs, and a mapping
/// request that passed Linux's value there would fail rather than misbehave. The support table
/// names the systems on which an artifact of this form has been published AND RUN, and Linux is the
/// only Unix among them - so this half is honest about being a Linux half, which is a different
/// thing from being a broken portable one.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=1954A2
// Broiler-Falsified-If: this half is reached on a system it was not written for
// Broiler-Human:        PENDING
internal sealed unsafe partial class JsNativePage
{
    /// <summary>Readable and writable, and NOT executable: what a mapping starts as.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=A3E0E6
    // Broiler-Falsified-If: this value admits an execute
    // Broiler-Human:        PENDING
    private const int ProtReadWrite = 0x1 | 0x2;

    /// <summary>
    /// Readable and executable, and NOT writable: what a mapping becomes and stays.
    /// </summary>
    /// <remarks>
    /// <b>THESE TWO ARE THE WHOLE CLOSED SET, and the combination that names both write and execute
    /// permission is not among them and is not written anywhere in this repository.</b> Each is used
    /// exactly once, at one call site, and neither is reachable through a variable a later edit
    /// could widen.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=4E4A17
    // Broiler-Falsified-If: a protection value naming both write and execute permission appears in any source file this component compiles, or outside the stored witness inputs rule X1's negative controls are made of
    // Broiler-Human:        PENDING
    private const int ProtReadExecute = 0x1 | 0x4;

    /// <summary>A private anonymous mapping, as Linux spells it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=913A18
    // Broiler-Falsified-If: this names a shared or a file-backed mapping
    // Broiler-Human:        PENDING
    private const int MapPrivateAnonymous = 0x02 | 0x20;

    /// <summary>What <c>mmap</c> answers when it failed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=BAFAB4
    // Broiler-Falsified-If: this differs from the value the platform answers a failed mapping with, so a failure is read as an address
    // Broiler-Human:        PENDING
    private static readonly void* MapFailed = (void*)(-1);

    /// <summary>Maps a writable, non-executable region.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=31576D
    // Broiler-Falsified-If: this passes any protection other than the readable-and-writable one
    // Broiler-Human:        PENDING
    private static byte* MapUnix(nuint bytes)
    {
        var mapped = Map(null, bytes, ProtReadWrite, MapPrivateAnonymous, -1, 0);
        return mapped == MapFailed ? null : (byte*)mapped;
    }

    /// <summary>Makes the region readable and executable, and no longer writable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=7C06EF
    // Broiler-Falsified-If: this passes any protection that admits a write
    // Broiler-Human:        PENDING
    private static bool ArmUnix(byte* at, nuint bytes) =>
        Protect(at, bytes, ProtReadExecute) == 0;

    /// <summary>Unmaps the region.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=2252AF
    // Broiler-Falsified-If: this unmaps an address or a length the mapping does not own
    // Broiler-Human:        PENDING
    private static void ReleaseUnix(byte* at, nuint bytes) => Unmap(at, bytes);

    /// <summary>Creates a mapping.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=491B84
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport("libc", EntryPoint = "mmap", SetLastError = true)]
    private static partial void* Map(
        void* address, nuint length, int protect, int flags, int descriptor, nint offset);

    /// <summary>Changes a mapping's protection.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=70F3E8
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport(
        "libc", EntryPoint = "mprotect", SetLastError = true)]
    private static partial int Protect(void* address, nuint length, int protect);

    /// <summary>Removes a mapping.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=5B2FE6
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport(
        "libc", EntryPoint = "munmap", SetLastError = true)]
    private static partial int Unmap(void* address, nuint length);
}
