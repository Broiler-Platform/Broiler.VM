// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           0
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    Critical
// Criteria:         13/13
// Resource impact:  4/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <content>
/// The Windows half of the one place that makes memory executable.
/// </content>
/// <remarks>
/// <b>IT IS A SEPARATE FILE AND NOT A PREPROCESSOR BRANCH, because rule J6 forbids a preprocessor
/// directive in any covered source file of this product.</b> Both halves are compiled into every
/// image and the choice between them is a run-time test; the alternative - selecting a file by an
/// MSBuild condition - would make the published closure differ per target and would multiply the
/// closure reports a composition rule compares against an exact allowed set.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=1954A2
// Broiler-Falsified-If: this half is reached on a system it was not written for
// Broiler-Human:        PENDING
internal sealed unsafe partial class JsNativePage
{
    /// <summary>Reserve and commit, in one call.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=724619
    // Broiler-Falsified-If: this names anything but reserving and committing, or it carries a protection bit
    // Broiler-Human:        PENDING
    private const uint MemCommitAndReserve = 0x3000;

    /// <summary>Release the whole reservation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=95117F
    // Broiler-Falsified-If: this names a free that leaves the reservation standing
    // Broiler-Human:        PENDING
    private const uint MemRelease = 0x8000;

    /// <summary>Readable and writable, and NOT executable: what a mapping starts as.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=9DE8C2
    // Broiler-Falsified-If: this value admits an execute
    // Broiler-Human:        PENDING
    private const uint PageReadWrite = 0x04;

    /// <summary>
    /// Readable and executable, and NOT writable: what a mapping becomes and stays.
    /// </summary>
    /// <remarks>
    /// <b>THESE TWO ARE THE WHOLE CLOSED SET, and the value that means readable, writable AND
    /// executable is not among them and is not written anywhere in this repository.</b> A protection
    /// argument that came from a variable, a parameter or a table would be an argument some later
    /// edit could widen; there are two named constants, each used once, and no third.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=056695
    // Broiler-Falsified-If: a protection value naming both write and execute permission appears in any source file this component compiles, or outside the stored witness inputs rule X1's negative controls are made of
    // Broiler-Human:        PENDING
    private const uint PageExecuteRead = 0x20;

    /// <summary>Maps a writable, non-executable region.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=5C64B1
    // Broiler-Falsified-If: this passes any protection other than the readable-and-writable one
    // Broiler-Human:        PENDING
    private static byte* MapWindows(nuint bytes) =>
        (byte*)VirtualAlloc(null, bytes, MemCommitAndReserve, PageReadWrite);

    /// <summary>Makes the region readable and executable, and no longer writable.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=10433E
    // Broiler-Falsified-If: this passes any protection that admits a write
    // Broiler-Human:        PENDING
    private static bool ArmWindows(byte* at, nuint bytes)
    {
        uint previous;

        if (!VirtualProtect(at, bytes, PageExecuteRead, &previous))
        {
            return false;
        }

        // The platform's documented way to publish freshly written code. On x86-64 the instruction
        // cache is coherent with stores in hardware and this is not architecturally required; it is
        // called because it is the supported sequence and it costs one call per artifact.
        FlushInstructionCache(GetCurrentProcess(), at, bytes);
        return true;
    }

    /// <summary>Releases the whole reservation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=7E36F0
    // Broiler-Falsified-If: this releases an address the mapping does not own, or releases one twice
    // Broiler-Human:        PENDING
    private static void ReleaseWindows(byte* at) => VirtualFree(at, 0, MemRelease);

    /// <summary>Reserves and commits a region with the protection given.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=634738
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial void* VirtualAlloc(void* address, nuint size, uint type, uint protect);

    /// <summary>Changes a committed region's protection.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=3BA5CB
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport("kernel32.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(
        System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static partial bool VirtualProtect(
        void* address, nuint size, uint protect, uint* previous);

    /// <summary>Releases a reservation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=65CC8A
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport("kernel32.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(
        System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static partial bool VirtualFree(void* address, nuint size, uint type);

    /// <summary>Publishes freshly written code to the instruction stream.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=030701
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport("kernel32.dll", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(
        System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static partial bool FlushInstructionCache(void* process, void* address, nuint size);

    /// <summary>A pseudo-handle for the calling process.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=4; Fingerprint=FCD535
    // Broiler-Falsified-If: this signature differs from the one the platform exports
    // Broiler-Human:        PENDING
    [System.Runtime.InteropServices.LibraryImport("kernel32.dll")]
    private static partial void* GetCurrentProcess();
}
