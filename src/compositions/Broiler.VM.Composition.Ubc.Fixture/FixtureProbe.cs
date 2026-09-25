using Broiler.VM;
using Broiler.VM.Emitter.Bytecode;
using Broiler.VM.Ubc;
using Com.Example.Tally;

namespace Broiler.VM.Composition.Ubc.Fixture;

/// <summary>
/// A probing form: the bytecode emitter's executor, reached through a factory that watches what the
/// executor answers and what it charges, so a check can hold the executor itself to a property the
/// core's answer alone cannot show.
/// </summary>
/// <remarks>
/// <para>
/// The core ranks a cancellation above a poll-bound breach and answers an exception an executor throws
/// exactly as it answers a contract violation the executor returns. A check that read the core's answer
/// alone would therefore pass for a loop that never polls, and for an executor that crashes where it
/// should refuse. The probe records the executor's own steps and the fuel it charged after the token
/// was cancelled, and it can be told to break the executor the way those checks must catch - to swallow
/// every poll, or to throw from an invocation - so that each check is watched failing too.
/// </para>
/// <para>
/// It is composed only into the runtimes those checks build; the image's own descriptor is the
/// bytecode emitter's form as it stands.
/// </para>
/// </remarks>
internal sealed class FixtureProbe : IUbcExecutorFactory
{
    private readonly List<VmExecutionStepKind> steps = new();

    /// <summary>The token whose cancellation the probe measures from; none measures nothing.</summary>
    internal CancellationToken Watched { get; init; }

    /// <summary>Cancelled as an invocation starts, so that verification and instantiation run uncancelled.</summary>
    internal CancellationTokenSource? CancelAtInvoke { get; init; }

    /// <summary>Answers every poll true without asking the core: a loop that never polls.</summary>
    internal bool SwallowPolls { get; init; }

    /// <summary>Throws from every invocation: an executor that crashes where it should answer.</summary>
    internal bool ThrowOnInvoke { get; init; }

    /// <summary>The kinds of the steps the executor answered, in order.</summary>
    internal IReadOnlyList<VmExecutionStepKind> Steps => steps;

    /// <summary>True when a call into the executor threw.</summary>
    internal bool Threw { get; private set; }

    /// <summary>The fuel the executor charged once <see cref="Watched"/> was cancelled.</summary>
    internal ulong FuelAfterCancellation { get; private set; }

    /// <summary>True when a poll the core answered returned false after <see cref="Watched"/> was cancelled.</summary>
    internal bool CancellationObserved { get; private set; }

    /// <summary>The fixture family's descriptor over this probe as its one form.</summary>
    internal VmProfileDescriptor Descriptor() => UbcDescriptors.Build(
        TallyProfile.Registration,
        TallyProfile.Declaration,
        UbcEmitterSet.Create(new UbcForm(UbcFormat.BytecodeForm, UbcBytecodeEmitter.SemanticVersion, this)));

    /// <inheritdoc/>
    public IVmProfileExecutor Create<TFamily>(
        UbcFamilyRegistration<TFamily> family,
        UbcFamilyDeclaration declaration,
        IVmExecutionEnvironment environment)
        where TFamily : struct, IUbcFamily =>
        new Executor(this, UbcBytecodeEmitter.Form.ExecutorFactory.Create(family, declaration, new Environment(this, environment)));

    private VmExecutionStep Record(Func<VmExecutionStep> call)
    {
        try
        {
            var step = call();
            steps.Add(step.Kind);
            return step;
        }
        catch
        {
            Threw = true;
            throw;
        }
    }

    private sealed class Executor(FixtureProbe probe, IVmProfileExecutor inner) : IVmProfileExecutor
    {
        public VmProfileId ProfileId => inner.ProfileId;

        public VmExecutionStep Instantiate(VmVerifiedArtifact artifact, CancellationToken cancellationToken) =>
            probe.Record(() => inner.Instantiate(artifact, cancellationToken));

        public VmExecutionStep Invoke(IVmInstanceState state, in VmInvocationRequest request, CancellationToken cancellationToken)
        {
            probe.CancelAtInvoke?.Cancel();

            try
            {
                var step = probe.ThrowOnInvoke
                    ? throw new InvalidOperationException("the probe was told to crash the executor")
                    : inner.Invoke(state, in request, cancellationToken);
                probe.steps.Add(step.Kind);
                return step;
            }
            catch
            {
                probe.Threw = true;
                throw;
            }
        }

        public VmExecutionStep Resume(IVmInstanceState state, IVmProfileContinuation continuation, CancellationToken cancellationToken) =>
            probe.Record(() => inner.Resume(state, continuation, cancellationToken));

        public void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance) =>
            inner.Unwind(continuation, effectiveUnwindAllowance);
    }

    private sealed class Environment(FixtureProbe probe, IVmExecutionEnvironment inner) : IVmExecutionEnvironment
    {
        public VmProfileId ProfileId => inner.ProfileId;

        // The environment's meter is the ambient one of the step in progress, so it is read at each use.
        public IVmMeter Meter => new Meter(probe, inner.Meter);

        public IVmHostCapabilityInvoker Capabilities => inner.Capabilities;

        public bool TryGetArtifactLoadMediator(out IVmArtifactLoadMediator mediator) => inner.TryGetArtifactLoadMediator(out mediator);
    }

    private sealed class Meter(FixtureProbe probe, IVmMeter inner) : IVmMeter
    {
        public bool TryCharge(VmBudgetDimension dimension, ulong amount)
        {
            if (dimension == VmBudgetDimension.Fuel && probe.Watched.IsCancellationRequested)
            {
                probe.FuelAfterCancellation += amount;
            }

            return inner.TryCharge(dimension, amount);
        }

        public bool Poll()
        {
            if (probe.SwallowPolls)
            {
                return true;
            }

            var answer = inner.Poll();

            if (!answer && probe.Watched.IsCancellationRequested)
            {
                probe.CancellationObserved = true;
            }

            return answer;
        }

        public void ReportRetained(VmBudgetDimension dimension, ulong amount) => inner.ReportRetained(dimension, amount);

        public void ReportReleased(VmBudgetDimension dimension, ulong amount) => inner.ReportReleased(dimension, amount);
    }
}
