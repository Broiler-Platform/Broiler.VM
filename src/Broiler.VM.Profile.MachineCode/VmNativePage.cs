// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           10
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=24103E
// Broiler-Human:        PENDING
internal enum VmNativePageState
{
    /// <summary>Mapped readable and writable, and not executable.</summary>
    Writable = 0,

    /// <summary>Mapped readable and executable, and not writable.</summary>
    Armed = 1,

    /// <summary>Unmapped. Nothing may be asked of it.</summary>
    Released = 2,
}

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DD89CC
// Broiler-Human:        PENDING
internal sealed unsafe partial class VmNativePage : System.IDisposable
{
    private byte* address;
    private readonly nuint length;
    private VmNativePageState state;
    private readonly VmNativeMapping mapping;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1715A5
    // Broiler-Human:        PENDING
    private VmNativePage(VmNativeMapping owned, byte* mapped, nuint bytes)
    {
        mapping = owned;
        address = mapped;
        length = bytes;
        state = VmNativePageState.Writable;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2CBD03
    // Broiler-Human:        PENDING
    private sealed unsafe class VmNativeMapping : System.Runtime.InteropServices.SafeHandle
    {
        private readonly nuint bytes;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=459880
        // Broiler-Human:        PENDING
        internal VmNativeMapping(byte* mapped, nuint length)
            : base(System.IntPtr.Zero, ownsHandle: true)
        {
            bytes = length;
            SetHandle((nint)mapped);
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7887B9
        // Broiler-Human:        PENDING
        public override bool IsInvalid => handle == System.IntPtr.Zero;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=975095
        // Broiler-Human:        PENDING
        protected override bool ReleaseHandle()
        {
            var released = (byte*)handle;

            if (System.OperatingSystem.IsWindows())
            {
                ReleaseWindows(released);
            }
            else
            {
                ReleaseUnix(released, bytes);
            }

            System.GC.RemoveMemoryPressure((long)bytes);
            return true;
        }
    }

    internal VmNativePageState State => state;

    internal nuint Length => length;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=C2E188
    // Broiler-Human:        PENDING
    internal static VmNativePage? TryMap(System.ReadOnlySpan<byte> code)
    {
        if (code.Length == 0)
        {
            return null;
        }

        var bytes = (nuint)code.Length;

        var mapped = System.OperatingSystem.IsWindows()
            ? MapWindows(bytes)
            : MapUnix(bytes);

        if (mapped is null)
        {
            return null;
        }

        var owned = new VmNativeMapping(mapped, bytes);
        System.GC.AddMemoryPressure((long)bytes);

        var page = new VmNativePage(owned, mapped, bytes);
        code.CopyTo(new System.Span<byte>(mapped, code.Length));
        return page;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7ECD5D
    // Broiler-Human:        PENDING
    internal bool Arm()
    {
        if (state != VmNativePageState.Writable)
        {
            throw new System.InvalidOperationException(
                "a mapping can be armed only once, and only while it is writable");
        }

        var armed = System.OperatingSystem.IsWindows()
            ? ArmWindows(address, length)
            : ArmUnix(address, length);

        if (!armed)
        {
            return false;
        }

        state = VmNativePageState.Armed;
        return true;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A9EFE4
    // Broiler-Human:        PENDING
    internal delegate* unmanaged<VmNativeFrame*, int> Entry(uint offset)
    {
        if (state != VmNativePageState.Armed)
        {
            throw new System.InvalidOperationException(
                "an entry point exists only for an armed mapping");
        }

        if (offset >= length)
        {
            throw new System.ArgumentOutOfRangeException(nameof(offset));
        }

        return (delegate* unmanaged<VmNativeFrame*, int>)(address + offset);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=304FFB
    // Broiler-Human:        PENDING
    internal nint At(uint offset)
    {
        if (state != VmNativePageState.Armed)
        {
            throw new System.InvalidOperationException(
                "an address inside a mapping is handed out only once it is armed");
        }

        if (offset >= length)
        {
            throw new System.ArgumentOutOfRangeException(nameof(offset));
        }

        return (nint)(address + offset);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6B229D
    // Broiler-Human:        PENDING
    public void Dispose()
    {
        if (state == VmNativePageState.Released || address is null)
        {
            return;
        }

        state = VmNativePageState.Released;
        address = null;
        mapping.Dispose();
    }
}
