// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           12
// Human-reviewed:   0/14
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The JavaScript profile's bytecode format: magic, format version, section framing, and the
/// vocabularies a section body is written in.
/// </summary>
/// <remarks>
/// <para>
/// Format version 1 is defined with the first feature manifest and grows with the interpreter. It
/// is deliberately not an enumeration of a whole-language opcode set: an opcode set designed
/// before the value model is a set that will be redesigned after it.
/// </para>
/// <para>
/// What version 1 carries from the first byte is everything whose retrofit is a format-version
/// break rather than an addition. Framed sections with a declared count, a constant pool with a
/// tag per entry, a code section with fixed instruction boundaries, exception regions with
/// explicit nesting and a finally continuation target, <b>suspension and resume targets reserved
/// before any generator exists</b>, a canonical position table, and declared maxima for the
/// operand stack, locals, frames and constants. The slice manifest admits none of the exception
/// or suspension machinery and the verifier refuses an artifact that uses it - but the SECTIONS
/// are framed, parsed and validated from version 1, because adding a section kind to a frozen
/// format is what a version break is for.
/// </para>
/// <para>
/// A declared maximum in this format is <b>declared for checking and never used to size an
/// allocation before the bound comparison</b>. That is the difference between a bound and a hope,
/// and it is a property of how the verifier reads this format rather than of the format itself.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=F6149F
// Broiler-Human:        PENDING
public static class JavaScriptFormat
{
    /// <summary>The four magic bytes every artifact of this profile starts with.</summary>
    /// <remarks>
    /// Not a version. The format version follows it as a variable-length integer, so a reader that
    /// matched on the magic alone would be matching on the family rather than on the encoding.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=94D7A4
    // Broiler-Human:        PENDING
    public static System.ReadOnlySpan<byte> Magic => "BJSB"u8;

    /// <summary>The only format version this build defines.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F181CE
    // Broiler-Human:        PENDING
    public const uint FormatVersion = 1;

    /// <summary>The lowest format version the profile descriptor accepts.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=473842
    // Broiler-Human:        PENDING
    public const uint MinimumFormatVersion = 1;

    /// <summary>The highest format version the profile descriptor accepts.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=801732
    // Broiler-Human:        PENDING
    public const uint MaximumFormatVersion = 1;

    /// <summary>The longest a feature-manifest identity may be, in UTF-8 bytes.</summary>
    /// <remarks>
    /// A bound rather than a convention, because the identity is read out of untrusted bytes
    /// before anything else about the artifact is known. The core's own identity grammar is far
    /// shorter than this; the slack is so a later manifest name is not a format break.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=44464C
    // Broiler-Human:        PENDING
    public const uint MaximumManifestIdBytes = 128;

    /// <summary>The longest an entry-point name may be, in UTF-8 bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=BC9B82
    // Broiler-Human:        PENDING
    public const uint MaximumEntryNameBytes = 256;

    /// <summary>
    /// The section kinds, which must appear in strictly ascending order and at most once each.
    /// </summary>
    /// <remarks>
    /// Ascending and unique is a structural rule the verifier enforces rather than a convention a
    /// writer follows. It makes a duplicated section unrepresentable rather than
    /// last-one-wins, and it means a reader never has to hold two candidate bodies for one kind.
    /// An unknown kind is refused outright: a format that skipped what it did not recognise would
    /// let one artifact carry content its verifier never looked at.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=028C66
    // Broiler-Human:        PENDING
    public enum SectionKind : uint
    {
        /// <summary>Declared maxima: operand stack, locals, frames, constants.</summary>
        Limits = 1,

        /// <summary>The constant pool.</summary>
        Constants = 2,

        /// <summary>The instruction stream.</summary>
        Code = 3,

        /// <summary>The named entry points and their code offsets.</summary>
        Entries = 4,

        /// <summary>Exception regions. Framed from version 1; admitted by no manifest yet.</summary>
        ExceptionRegions = 5,

        /// <summary>Suspension and resume targets. Framed from version 1; admitted by no manifest yet.</summary>
        SuspensionTargets = 6,

        /// <summary>The canonical bytecode-offset to source-position table.</summary>
        Positions = 7,
    }

    /// <summary>The constant-pool entry tags.</summary>
    /// <remarks>
    /// <see cref="InternedName"/> is reserved from version 1 and admitted by no manifest yet. The
    /// slice has no property access and no strings, so an artifact carrying one is refused as a
    /// construct outside its declared manifest - at verification, by its own diagnostic code, and
    /// not at first execution.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8A0ADC
    // Broiler-Human:        PENDING
    public enum ConstantTag : byte
    {
        /// <summary>The one value <c>undefined</c>. No payload.</summary>
        Undefined = 1,

        /// <summary>A Boolean. One payload byte, which must be 0 or 1.</summary>
        Boolean = 2,

        /// <summary>A Number. Eight payload bytes, IEEE 754 binary64, little-endian.</summary>
        Number = 3,

        /// <summary>
        /// A property name, interned once per program at load time rather than at each use.
        /// Reserved: no manifest admits it.
        /// </summary>
        InternedName = 4,
    }

    /// <summary>
    /// The profile's own ceiling on each declared maximum, checked before the maximum is believed.
    /// </summary>
    /// <remarks>
    /// These are not budget dimensions and they are not the descriptor's limit vector. They are
    /// this format's own structural ceilings, and they exist so that a declared maximum is
    /// compared against something before it reaches a comparison that would otherwise be
    /// unbounded. A breach of one of them is an INVALID ARTIFACT and not a resource exhaustion:
    /// the host declined nothing, the artifact declared something this format does not represent.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F2AF0C
    // Broiler-Human:        PENDING
    public const uint CeilingOperandStack = 1024;

    /// <summary>The most locals one frame may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=DCA423
    // Broiler-Human:        PENDING
    public const uint CeilingLocals = 65_536;

    /// <summary>The most frames the slice may declare, which is one - it has no functions.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E75BE7
    // Broiler-Human:        PENDING
    public const uint CeilingFrames = 1;

    /// <summary>The most constants a pool may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7EF4ED
    // Broiler-Human:        PENDING
    public const uint CeilingConstants = 65_536;

    /// <summary>The most entry points one artifact may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=4D3618
    // Broiler-Human:        PENDING
    public const uint CeilingEntries = 256;

    /// <summary>The most position-table rows one artifact may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=514841
    // Broiler-Human:        PENDING
    public const uint CeilingPositions = 1_048_576;
}
