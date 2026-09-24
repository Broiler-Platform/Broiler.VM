// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           2
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Critical
// Criteria:         9/9
// Resource impact:  0/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The frame context a value-form unit is entered with: the one structure its emitted code and its
/// helpers share (JSD-0035 section 9).
/// </summary>
/// <remarks>
/// <para>
/// <b>NOTHING IN THIS STRUCTURE IS A MANAGED REFERENCE, AND NEITHER IS ANYTHING IT POINTS AT</b>, which
/// is rule X2 extended to the value form. <see cref="Helpers"/> is the address of a table of function
/// pointers in unmanaged memory that lives for the process, and <see cref="Cookie"/> is an integer.
/// Every word an instruction reads or writes lives in the instance's pinned slab, and every object a
/// word names lives in the instance's handle table; a helper reaches both from managed code after the
/// cookie has been compared, and the emitted code holds neither.
/// </para>
/// <para>
/// <b>AT STAGE JSV-1 IT HAS THE BASELINE FRAME'S TWO FIELDS AT THE BASELINE FRAME'S OFFSETS</b>, because
/// the emitted code of that stage is the baseline layout over a partition in which every instruction is
/// a block, and it reads nothing but the table. It is a type of its own so that a value helper can
/// never be handed a baseline frame, and so that the fields JSV-2 adds - the region address the inline
/// templates address words from and the fuel debt they count into - are added to this form alone.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=5A1791
// Broiler-Falsified-If: a field of this structure is, or contains, a reference the collector traces, or its offsets differ from the constants in JsValueAbi
// Broiler-Human:        PENDING
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct JsValueFrame
{
    /// <summary>The base address of the helper table: one function pointer per opcode byte.</summary>
    public nint Helpers;

    /// <summary>The identity of the activation this frame was built for.</summary>
    public long Cookie;
}

/// <summary>The value form's frame layout, as the emitter and the runtime both read it.</summary>
/// <remarks>
/// <b>One statement of each number, in the assembly both halves reference</b>, for the reason
/// <see cref="JsBaselineAbi"/> gives: the emitter is in the lowering, the helpers are in the profile,
/// and neither may reference the other.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D6B895
// Broiler-Falsified-If: a constant here disagrees with the layout the runtime gives JsValueFrame, or with the offsets the value form's emitted code reads
// Broiler-Human:        PENDING
public static class JsValueAbi
{
    /// <summary>The offset of <see cref="JsValueFrame.Helpers"/>.</summary>
    /// <remarks>
    /// <b>It is the baseline frame's table offset</b>, so the prologue both forms share loads the table
    /// register from the same place.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D2E11C
    // Broiler-Falsified-If: the runtime places JsValueFrame.Helpers at any other offset, or it differs from JsBaselineAbi.HandlersOffset
    // Broiler-Human:        PENDING
    public const int HelpersOffset = JsBaselineAbi.HandlersOffset;

    /// <summary>The offset of <see cref="JsValueFrame.Cookie"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=5C496D
    // Broiler-Falsified-If: the runtime places JsValueFrame.Cookie at any other offset
    // Broiler-Human:        PENDING
    public const int CookieOffset = 8;

    /// <summary>The size of <see cref="JsValueFrame"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=168EAF
    // Broiler-Falsified-If: the runtime gives JsValueFrame any other size
    // Broiler-Human:        PENDING
    public const int FrameSize = 16;

    /// <summary>How many slots the helper table has: one per value an opcode byte can take.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A738D5
    // Broiler-Falsified-If: an emitted call can index the helper table at or beyond this many slots
    // Broiler-Human:        PENDING
    public const int HelperSlots = JsBaselineAbi.HandlerSlots;
}

/// <summary>
/// The emitted-code section header's first field: an architecture in its low byte and a form byte above
/// it.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE FORM BYTE IS ZERO FOR THE MANIFEST'S OWN FORM AND NAMES ONLY THE VALUE FORM OTHERWISE.</b>
/// Every artifact written before the value form existed carries zero there, because its architecture
/// field held a value below 256, so each of them reads exactly as it did, and nothing was re-versioned
/// to make room. A reader built before the value form existed refuses a nonzero byte as an
/// architecture it cannot name, which is the right refusal for a form it cannot run. The two upper
/// bytes stay zero, and a reader refuses anything else there.
/// </para>
/// <para>
/// <b>One way to write each fact.</b> The numeric and the baseline forms are only ever written as zero,
/// never as their own tier number, so a payload cannot state the manifest's own form twice over in two
/// spellings that a reader would have to reconcile.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=062001
// Broiler-Falsified-If: a header field Pack writes is read back by TryUnpack as another architecture or another form, a field with a nonzero upper half or a form byte other than zero and the value form's is read at all, or a numeric or baseline header is written with a nonzero form byte
// Broiler-Human:        PENDING
public static class JsNativeCodeHeader
{
    /// <summary>How far the form byte sits above the architecture.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1828CB
    // Broiler-Human:        PENDING
    public const int FormShift = 8;

    /// <summary>The header's first field for an architecture and a form.</summary>
    /// <param name="architecture">The instruction set and convention.</param>
    /// <param name="valueForm">Whether the section is the value form rather than the manifest's own form.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=F5CEC9
    // Broiler-Falsified-If: it writes a form byte other than zero for a section that is not the value form, or other than the value tier's number for one that is
    // Broiler-Human:        PENDING
    public static uint Pack(JsNativeArchitecture architecture, bool valueForm) =>
        ((uint)architecture & 0xFF) | (valueForm ? (uint)JsNativeTier.Value << FormShift : 0u);

    /// <summary>Splits the header's first field, or answers false for one no reader of this build admits.</summary>
    /// <param name="field">The field as the section carries it.</param>
    /// <param name="architecture">Its low byte, when the answer is true.</param>
    /// <param name="valueForm">Whether its form byte names the value form, when the answer is true.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=7714A8
    // Broiler-Falsified-If: it answers true for a field whose upper sixteen bits are not zero or whose form byte is neither zero nor the value tier's number
    // Broiler-Human:        PENDING
    public static bool TryUnpack(uint field, out JsNativeArchitecture architecture, out bool valueForm)
    {
        architecture = (JsNativeArchitecture)(field & 0xFF);
        var form = (field >> FormShift) & 0xFF;
        valueForm = form == (uint)JsNativeTier.Value;
        return (field >> 16) == 0 && (form == 0 || valueForm);
    }
}
