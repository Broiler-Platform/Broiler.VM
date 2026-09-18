// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           3
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C7E841
// Broiler-Human:        PENDING
public sealed class MachineCodeReadAdapter : IVmBoundedAllocationMeter
{
    private readonly IVmVerificationContext context;
    private readonly System.Threading.CancellationToken cancellationToken;

    public MachineCodeReadAdapter(IVmVerificationContext context, System.Threading.CancellationToken cancellationToken)
    {
        this.context = context;
        this.cancellationToken = cancellationToken;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1029AC
    // Broiler-Human:        PENDING
    public bool TryReserve(ulong byteCount) =>
        context.Meter.TryCharge(VmBudgetDimension.AllocatedBytes, byteCount);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E46A49
    // Broiler-Human:        PENDING
    public void Release(ulong byteCount) =>
        context.Meter.ReportReleased(VmBudgetDimension.AllocatedBytes, byteCount);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=A0E8D7
    // Broiler-Human:        PENDING
    public bool TryChargeWork(ulong workUnits) =>
        context.Meter.TryCharge(VmBudgetDimension.VerifierWork, workUnits);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=37191E
    // Broiler-Human:        PENDING
    public bool Poll()
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        return context.Meter.Poll();
    }
}
