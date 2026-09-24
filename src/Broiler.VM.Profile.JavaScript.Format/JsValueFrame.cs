// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   22
// Annotated:        22/22
// Exempt:           6
// Human-reviewed:   0/22
// IP risk:          Low
// Security risk:    Critical
// Criteria:         20/20
// Resource impact:  0/10 max
// Unverified:       22
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
/// pointers in unmanaged memory that lives for the process, <see cref="Region"/> is the address of the
/// activation's first word in a pinned slab, and <see cref="Cookie"/> and <see cref="Debt"/> are
/// integers. Every object a word names lives in the instance's handle table, which a helper reaches
/// from managed code after the cookie has been compared; the emitted code holds none of it.
/// </para>
/// <para>
/// <b>STAGE JSV-2 ADDS THE TWO FIELDS ITS INLINE TEMPLATES READ.</b> The region address is where an
/// inline template addresses words from: the activation's arguments, then its resident bindings, then
/// its operand stack (<see cref="JsValueLayout"/> says which word is which). The debt is how many pure
/// instructions ran since the last settlement; the emitted code keeps a running count in a register,
/// stores it here before every call, and the helper that is called charges it to the meter and zeroes
/// it (JSD-0035 section 7). The first two fields keep the offsets JSV-1 gave them.
/// </para>
/// <para>
/// <b>STAGE JSV-3 ADDS THE TWO FIELDS A DIRECT CALL READS</b> (JSD-0035 section 6). A unit's frame on the
/// machine stack holds one context of its own for the callee of a direct call, filled by the call-prepare
/// helper: the callee's frame fields, <see cref="Entry"/>, the address of the callee unit's emitted entry in
/// the same payload, and <see cref="EntryPc"/>, the offset it is entered at. The emitted code writes none of
/// the context but its debt word, which is how a helper-filled entry address is the only one a direct call
/// can go to.
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

    /// <summary>The address of the activation's first word, inside a pinned slab.</summary>
    public nint Region;

    /// <summary>How many pure instructions ran and have not yet been charged to the meter.</summary>
    public long Debt;

    /// <summary>
    /// For the context a direct call is made with: the address of the callee unit's emitted entry, which only
    /// the call-prepare helper writes; nothing in any other context.
    /// </summary>
    public nint Entry;

    /// <summary>For the context a direct call is made with: the offset the callee is entered at.</summary>
    public long EntryPc;
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

    /// <summary>The offset of <see cref="JsValueFrame.Region"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=604F4C
    // Broiler-Falsified-If: the runtime places JsValueFrame.Region at any other offset
    // Broiler-Human:        PENDING
    public const int RegionOffset = 16;

    /// <summary>The offset of <see cref="JsValueFrame.Debt"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=21C096
    // Broiler-Falsified-If: the runtime places JsValueFrame.Debt at any other offset
    // Broiler-Human:        PENDING
    public const int DebtOffset = 24;

    /// <summary>The size of <see cref="JsValueFrame"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D34DD6
    // Broiler-Falsified-If: the runtime gives JsValueFrame any other size
    // Broiler-Human:        PENDING
    public const int FrameSize = 48;

    /// <summary>The offset of <see cref="JsValueFrame.Entry"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=232D6F
    // Broiler-Falsified-If: the runtime places JsValueFrame.Entry at any other offset
    // Broiler-Human:        PENDING
    public const int EntryOffset = 32;

    /// <summary>The offset of <see cref="JsValueFrame.EntryPc"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=0828B6
    // Broiler-Falsified-If: the runtime places JsValueFrame.EntryPc at any other offset
    // Broiler-Human:        PENDING
    public const int EntryPcOffset = 40;

    /// <summary>How many slots the helper table has: one per value an opcode byte can take.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A738D5
    // Broiler-Falsified-If: an emitted call can index the helper table at or beyond this many slots
    // Broiler-Human:        PENDING
    public const int HelperSlots = JsBaselineAbi.HandlerSlots;

    /// <summary>The helper-table slot of the settlement helper a debt test calls.</summary>
    /// <remarks>
    /// <b>The last slot, which no opcode byte this format defines takes</b>, so a settlement can never be
    /// mistaken for an instruction's helper and an opcode added later cannot land on it unnoticed: the
    /// table's own check refuses a defined opcode at this slot.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=32CE45
    // Broiler-Falsified-If: an opcode this format defines has this byte, or an emitted settlement calls any other slot
    // Broiler-Human:        PENDING
    public const int SettleSlot = HelperSlots - 1;

    /// <summary>The helper-table slot of the call-prepare helper a direct call site calls first.</summary>
    /// <remarks>
    /// <b>The slot below the settlement's, which no opcode byte takes either</b>, for the settlement's reason.
    /// The helper runs the call's instruction through the interpreter's own arm, or prepares a direct call
    /// and answers <see cref="DirectCall"/> (JSD-0035 section 6, stage JSV-3).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=96F399
    // Broiler-Falsified-If: an opcode this format defines has this byte, or an emitted direct call site calls any other slot first
    // Broiler-Human:        PENDING
    public const int PrepareSlot = HelperSlots - 2;

    /// <summary>The helper-table slot of the call-finish helper a direct call site calls after the callee returns.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=19C3FB
    // Broiler-Falsified-If: an opcode this format defines has this byte, or an emitted direct call is followed by a call of any other slot
    // Broiler-Human:        PENDING
    public const int FinishSlot = HelperSlots - 3;

    /// <summary>What the call-prepare helper answers when the call is to be made directly.</summary>
    /// <remarks>
    /// <b>Negative, and none of <see cref="JsBaselineStatus"/>'s</b>, so it cannot be read as an offset to
    /// resume at or as a way to leave: the call site compares for it before its tail reads the answer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=EAD516
    // Broiler-Falsified-If: this equals a JsBaselineStatus or can be an instruction offset
    // Broiler-Human:        PENDING
    public const int DirectCall = -4;

    /// <summary>What a unit answers when an inline return left its value in the region's first word.</summary>
    /// <remarks>
    /// <b>The callee returns its value in its region's first word and a status in the return register</b>
    /// (JSD-0035 section 6): the word is the region's own, so nothing but a word crosses, and the debt the
    /// return ran up is in the context's debt word, for whoever reads the answer to charge before it goes on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B914F5
    // Broiler-Falsified-If: this equals a JsBaselineStatus or DirectCall, or a unit answers it with any word but its returned value in its region's first word
    // Broiler-Human:        PENDING
    public const int Returned = -5;

    /// <summary>Where in a unit's machine-stack frame the context of its direct callees lives.</summary>
    /// <remarks>
    /// <b>Above Windows x64's shadow space and at the stack pointer under System V</b>: a helper the unit calls
    /// may use the shadow space as its home area, so the context sits past it, and nothing System V calls
    /// writes above the stack pointer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=3500E3
    // Broiler-Falsified-If: the context overlaps the shadow space, the saved registers or the return address, or reaches past the frame the prologue reserves
    // Broiler-Human:        PENDING
    public static int CalleeOffset(JsNativeArchitecture architecture) =>
        architecture == JsNativeArchitecture.X64Windows ? 32 : 0;

    /// <summary>How many bytes a unit's prologue subtracts from the stack pointer after its four pushes.</summary>
    /// <remarks>
    /// <b>The baseline form's reservation and one callee context.</b> The baseline's forty bytes on Windows
    /// x64 and eight under System V bring the stack, eight past a sixteen-byte boundary after four pushes,
    /// to one, with the shadow space Windows asks for; the context is a multiple of sixteen, so the unit
    /// still calls every helper and every direct callee sixteen-aligned (stage JSV-3).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=90A8DF
    // Broiler-Falsified-If: a value-form unit calls a helper or a direct callee with the stack pointer not sixteen-aligned, with less shadow space than the convention requires, or with a callee context that does not fit the frame
    // Broiler-Human:        PENDING
    public static int FrameBytes(JsNativeArchitecture architecture) =>
        JsBaselineAbi.FrameBytes(architecture) + FrameSize;
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
/// <b>THE VALUE FORM'S BYTE ALSO SAYS WHETHER ANY BINDING MAY BE RESIDENT.</b> The value tier's number
/// alone is the value form as the residency analysis decides it; the same number with
/// <see cref="FlatBit"/> is the value form with every binding left in its managed environment, the
/// control JSD-0035's risks name for the analysis. The two emit different bytes for one program, so
/// the verifier has to re-plan a payload the way it was planned, and the byte is where that is said.
/// </para>
/// <para>
/// <b>One way to write each fact.</b> The numeric and the baseline forms are only ever written as zero,
/// never as their own tier number, so a payload cannot state the manifest's own form twice over in two
/// spellings that a reader would have to reconcile.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=062001
// Broiler-Falsified-If: a header field Pack writes is read back by TryUnpack as another architecture, another form or another residency, a field with a nonzero upper half or a form byte other than zero and the value form's two is read at all, or a numeric or baseline header is written with a nonzero form byte
// Broiler-Human:        PENDING
public static class JsNativeCodeHeader
{
    /// <summary>How far the form byte sits above the architecture.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1828CB
    // Broiler-Human:        PENDING
    public const int FormShift = 8;

    /// <summary>The bit of the form byte that withholds residency from every binding of a value-form payload.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=77B59B
    // Broiler-Human:        PENDING
    public const uint FlatBit = 0x10;

    /// <summary>The header's first field for an architecture and a form.</summary>
    /// <param name="architecture">The instruction set and convention.</param>
    /// <param name="valueForm">Whether the section is the value form rather than the manifest's own form.</param>
    /// <param name="residentBindings">
    /// For the value form, whether its bindings may be resident; ignored for any other form.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=66AAF0
    // Broiler-Falsified-If: it writes a form byte other than zero for a section that is not the value form, or other than the value tier's number, with FlatBit exactly when residency is withheld, for one that is
    // Broiler-Human:        PENDING
    public static uint Pack(JsNativeArchitecture architecture, bool valueForm, bool residentBindings = true) =>
        ((uint)architecture & 0xFF) |
        (valueForm ? ((uint)JsNativeTier.Value | (residentBindings ? 0u : FlatBit)) << FormShift : 0u);

    /// <summary>Splits the header's first field, or answers false for one no reader of this build admits.</summary>
    /// <param name="field">The field as the section carries it.</param>
    /// <param name="architecture">Its low byte, when the answer is true.</param>
    /// <param name="valueForm">Whether its form byte names the value form, when the answer is true.</param>
    /// <param name="residentBindings">
    /// Whether a value-form payload's bindings may be resident, when the answer is true; true for every
    /// other form, which has no bindings of its own to place.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=F3FE38
    // Broiler-Falsified-If: it answers true for a field whose upper sixteen bits are not zero or whose form byte is neither zero, the value tier's number, nor that number with FlatBit
    // Broiler-Human:        PENDING
    public static bool TryUnpack(
        uint field, out JsNativeArchitecture architecture, out bool valueForm, out bool residentBindings)
    {
        architecture = (JsNativeArchitecture)(field & 0xFF);
        var form = (field >> FormShift) & 0xFF;
        valueForm = (form & ~FlatBit) == (uint)JsNativeTier.Value;
        residentBindings = !valueForm || (form & FlatBit) == 0;
        return (field >> 16) == 0 && (form == 0 || valueForm);
    }
}
