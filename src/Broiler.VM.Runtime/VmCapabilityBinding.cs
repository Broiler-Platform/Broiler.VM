// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           12
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// One profile import bound, or deliberately not bound, to one host registration.
/// </summary>
/// <remarks>
/// Binding is by position into an immutable table fixed at runtime creation. The profile addresses
/// slot <em>k</em> and can ask only whether it is bound; it can never enumerate the table, resolve
/// by name, or reach a CLR type or member through it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C634B7
// Broiler-Human:        PENDING
internal sealed class VmCapabilityBinding
{
    internal VmCapabilityBinding(
        VmCapabilityImport import,
        VmHostCapabilityHandler? handler,
        VmHostBytesCapabilityHandler? bytesHandler,
        IVmArtifactProvider? provider)
    {
        Import = import;
        Handler = handler;
        BytesHandler = bytesHandler;
        Provider = provider;
    }

    internal VmCapabilityImport Import { get; }

    internal VmHostCapabilityHandler? Handler { get; }

    internal VmHostBytesCapabilityHandler? BytesHandler { get; }

    internal IVmArtifactProvider? Provider { get; }

    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=0; Fingerprint=337303
    // Broiler-Human:        PENDING
    internal bool IsBound => Handler is not null || BytesHandler is not null || Provider is not null;
}

/// <summary>The bound capability table one executor addresses.</summary>
/// <remarks>
/// <para>
/// The catch boundary here is the whole of exception translation. A capability that throws is
/// caught, and what happens next is what its descriptor declared - terminate the operation, or
/// report an observable fault - not what the exception happened to be. A cancellation and a
/// resource exhaustion raised inside a capability are recognised first, so a host that propagates
/// one is not reported as having faulted.
/// </para>
/// <para>
/// The per-runtime in-capability flag is held for the duration of the call, so a runtime call made
/// from inside a non-reentrant capability is refused rather than deadlocking or recursing.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C2C91D
// Broiler-Human:        PENDING
internal sealed class VmCapabilityInvoker : IVmHostCapabilityInvoker
{
    private readonly VmCapabilityBinding[] bindings;
    private readonly VmRuntime owner;
    private readonly VmMeter meter;

    internal VmCapabilityInvoker(VmCapabilityBinding[] bindings, VmRuntime owner, VmMeter meter)
    {
        this.bindings = bindings;
        this.owner = owner;
        this.meter = meter;
    }

    /// <summary>The outcome the last failing call produced, for the operation to report.</summary>
    internal VmReason LastFailure { get; private set; } = VmReason.None;

    /// <summary>The capability the last failing call named.</summary>
    internal VmCapabilityId LastFailureCapability { get; private set; }

    /// <summary>
    /// Whether the last failure was one the capability declared should end the operation.
    /// </summary>
    /// <remarks>
    /// This is what makes the declared translation mode mean something. Without it both modes reach
    /// the profile identically and the declaration is documentation.
    /// </remarks>
    internal bool TerminatesOperation { get; private set; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=08A7E3
    // Broiler-Human:        PENDING
    public int BindingCount => bindings.Length;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=0; Fingerprint=B14302
    // Broiler-Human:        PENDING
    public bool IsBound(int bindingIndex) =>
        bindingIndex >= 0 && bindingIndex < bindings.Length && bindings[bindingIndex].IsBound;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=2; Fingerprint=7C7101
    // Broiler-Human:        PENDING
    public VmHostCallOutcome Invoke(int bindingIndex, System.ReadOnlySpan<long> arguments, out long result)
    {
        result = 0;

        if (!TryEnter(bindingIndex, out var binding))
        {
            return VmHostCallOutcome.Unavailable;
        }

        var handler = binding.Handler;

        if (handler is null)
        {
            LastFailure = VmReason.CapabilitySignatureMismatch;
            LastFailureCapability = binding.Import.Descriptor.CapabilityId;
            Leave(binding);
            return VmHostCallOutcome.Unavailable;
        }

        try
        {
            return handler(arguments, out result);
        }
        catch (System.OperationCanceledException exception) when (meter.IsOperationCancellation(exception))
        {
            // Recognised before the generic catch so that a host propagating the operation's own
            // cancellation is not reported as having faulted. The filter is what makes that the
            // whole of the exemption: a cancellation carrying any other token, or carrying ours
            // while ours was never cancelled, falls to the generic catch and is translated as the
            // fault it is. Without it such an exception reached neither the stage's cancellation
            // test - which reads the operation's token, not this flag - nor the host-failure path,
            // because nothing here sets TerminatesOperation. It was dropped.
            LastFailure = VmReason.Cancelled;
            LastFailureCapability = binding.Import.Descriptor.CapabilityId;
            return VmHostCallOutcome.Unavailable;
        }
        catch (System.Exception)
        {
            return Translate(binding);
        }
        finally
        {
            Leave(binding);
        }
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=2; Fingerprint=F685D1
    // Broiler-Human:        PENDING
    public VmHostCallOutcome InvokeBytes(int bindingIndex, VmBytes argument, out VmOpaqueRef result)
    {
        result = default;

        if (!TryEnter(bindingIndex, out var binding))
        {
            return VmHostCallOutcome.Unavailable;
        }

        var handler = binding.BytesHandler;

        if (handler is null)
        {
            LastFailure = VmReason.CapabilitySignatureMismatch;
            LastFailureCapability = binding.Import.Descriptor.CapabilityId;
            Leave(binding);
            return VmHostCallOutcome.Unavailable;
        }

        try
        {
            return handler(argument, out result);
        }
        catch (System.OperationCanceledException exception) when (meter.IsOperationCancellation(exception))
        {
            LastFailure = VmReason.Cancelled;
            LastFailureCapability = binding.Import.Descriptor.CapabilityId;
            return VmHostCallOutcome.Unavailable;
        }
        catch (System.Exception)
        {
            return Translate(binding);
        }
        finally
        {
            Leave(binding);
        }
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=0; Fingerprint=C0F568
    // Broiler-Human:        PENDING
    private bool TryEnter(int bindingIndex, out VmCapabilityBinding binding)
    {
        binding = null!;

        if (bindingIndex < 0 || bindingIndex >= bindings.Length)
        {
            LastFailure = VmReason.CapabilityNotRegistered;
            return false;
        }

        binding = bindings[bindingIndex];

        if (!binding.IsBound)
        {
            LastFailure = VmReason.CapabilityNotRegistered;
            LastFailureCapability = binding.Import.Descriptor.CapabilityId;
            TerminatesOperation = true;
            return false;
        }

        if (!meter.TryCharge(VmBudgetDimension.HostCalls, 1))
        {
            LastFailure = VmReason.AllowanceExhausted;
            LastFailureCapability = binding.Import.Descriptor.CapabilityId;
            return false;
        }

        owner.EnterCapability(binding.Import.Descriptor.Reentrancy);
        return true;
    }

    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=0; Fingerprint=FDBEE7
    // Broiler-Human:        PENDING
    private void Leave(VmCapabilityBinding binding) =>
        owner.LeaveCapability(binding.Import.Descriptor.Reentrancy);

    // Broiler-AI:           Origin=AI; Spec=ADR-0011; IP=Low; Security=Medium; Resources=0; Fingerprint=FBCA55
    // Broiler-Human:        PENDING
    private VmHostCallOutcome Translate(VmCapabilityBinding binding)
    {
        LastFailure = VmReason.HostCapabilityFaulted;
        LastFailureCapability = binding.Import.Descriptor.CapabilityId;

        var observable = binding.Import.Descriptor.ExceptionTranslation is VmExceptionTranslation.ObservableFault;

        // TerminateOperation means the operation ends with the host failure, whatever the profile
        // does next. ObservableFault hands the refusal to the profile and lets it decide, and a
        // converted outcome is the profile's own.
        TerminatesOperation = !observable;

        return observable ? VmHostCallOutcome.Refused : VmHostCallOutcome.Unavailable;
    }
}
