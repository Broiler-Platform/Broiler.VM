// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// What two runtimes in one process can see of each other, and what a handle they share carries.
/// </summary>
/// <remarks>
/// <para>
/// <b>JS-4's gate asks for these in a shape this profile cannot produce, and the difference is the
/// point rather than an excuse.</b> The gate asks that two runtimes minting properties under the
/// same key text observe neither the other's storage nor its shape nor its key identity, <i>in a
/// test that fails when the key table is made process-wide again</i> — a falsifier written against
/// the seed's storage, where an interned key table and a shape-transition table are process-wide
/// structures a copy would have brought with it. **That copy has not happened**, JS-2 is blocked,
/// and what is here instead was written in this checkout: a property store is a dictionary owned by
/// one object, and there is no key table, no shape table and no feedback anywhere to be
/// process-wide. So the falsifier has nothing to switch on.
/// </para>
/// <para>
/// <b>What is asserted instead is the property the falsifier was protecting</b>, from the outside,
/// through the ordinary surface: two runtimes mint the same names in one process and neither sees
/// the other's values, and a handle one of them verified is read by both with no synchronisation
/// and answers the same thing every time. Those hold whatever the storage is made of, which is
/// what makes them worth running against an implementation the gate did not anticipate.
/// </para>
/// <para>
/// <b>The residual is stated rather than closed.</b> These are behavioural observations and not the
/// structural scan the gate names: they would not catch a per-instance structure that is reachable
/// from a handle and never observed by these programs. What bounds that residual is the
/// construction — a handle carries an immutable program and nothing else, and no cache, shape table
/// or warmed structure exists to be reachable — and a construction is not a scan. It is recorded
/// here so a reader is not left to infer it from a passing check.
/// </para>
/// <para>
/// <b>This root is the legal home for them</b>, for the reason
/// <see cref="SurfaceChecks"/> gives: rule A11 forbids a test project to reference a profile
/// assembly, and each of these is about a program written in JavaScript rather than about
/// hand-assembled bytes, so it needs the root that carries the lowering.
/// </para>
/// </remarks>
internal static class IsolationChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://isolation";

    /// <summary>How many times each thread reads the shared handle.</summary>
    /// <remarks>
    /// Enough that two threads overlap on an ordinary machine and small enough that the check costs
    /// a fraction of a second. It is a race the check tries to lose rather than a measurement.
    /// </remarks>
    private const int ConcurrentReads = 64;

    /// <summary>Runs every isolation check.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run() =>
    [
        TwoRuntimesMintOneNameAndObserveNothingOfEachOther(),
        OneShareableHandleIsInstantiatedByTwoRuntimes(),
        TwoRuntimesReadOneHandleConcurrently(),
        AHandleCarriesNothingTheRunsThatUsedItChanged(),
    ];

    /// <summary>
    /// Two runtimes in one process mint the same property names and read back only their own.
    /// </summary>
    /// <remarks>
    /// The names are the same TEXT in both, which is the whole question: if anything about a key
    /// were shared between runtimes — an interned table, a shape identity, a slot index — the two
    /// programs would be writing into one place. They are also written on the global object rather
    /// than on a local one, because a realm's global is the longest-lived object either runtime has
    /// and is where a process-wide structure would show first.
    /// </remarks>
    private static (string, bool, string) TwoRuntimesMintOneNameAndObserveNothingOfEachOther()
    {
        const string Name = "two runtimes mint one name and observe nothing of each other";

        // Each writes the same three names with a value of its own, then reports what it can see:
        // its own value, and whether the name the OTHER one alone defines is visible here.
        const string Program =
            "globalThis.shared = VALUE;" +
            "globalThis['also shared'] = VALUE + 1;" +
            "globalThis['x' + VALUE] = VALUE;" +
            "String(globalThis.shared) + '/' + String(globalThis['also shared']) + '/' +" +
            "String(typeof globalThis.x100) + '/' + String(typeof globalThis.x200);";

        var first = Compile(Program.Replace("VALUE", "100", System.StringComparison.Ordinal));
        var second = Compile(Program.Replace("VALUE", "200", System.StringComparison.Ordinal));

        if (first is null || second is null)
        {
            return (Name, false, "the programs did not compile, so the check judged nothing");
        }

        // BOTH RUNTIMES ARE ALIVE AT ONCE, which is what makes this about two runtimes rather than
        // about two runs. A structure shared between them would be shared while both hold it.
        using var runtimeA = Runtime();
        using var runtimeB = Runtime();

        if (runtimeA is null || runtimeB is null)
        {
            return (Name, false, "a runtime refused creation");
        }

        if (!TryRun(runtimeA, first, out var answerA, out var whyA))
        {
            return (Name, false, "the first runtime: " + whyA);
        }

        if (!TryRun(runtimeB, second, out var answerB, out var whyB))
        {
            return (Name, false, "the second runtime: " + whyB);
        }

        // 100/101/number/undefined from the first: it sees its own two values, the name it minted,
        // and NOT the one only the second mints. The second is the mirror image.
        var held =
            string.Equals(answerA, "100/101/number/undefined", System.StringComparison.Ordinal) &&
            string.Equals(answerB, "200/201/undefined/number", System.StringComparison.Ordinal);

        return (
            Name,
            held,
            $"the first answered {answerA} and the second {answerB}; each read back its own value " +
            "under the same key text and neither could see the name the other alone minted");
    }

    /// <summary>
    /// One handle, verified once, instantiated by the runtime that verified it and by a second one.
    /// </summary>
    /// <remarks>
    /// The profile declares its representation <c>Shareable</c>, and this is what that declaration
    /// means from outside: a second runtime may instantiate the same handle, and the two instances
    /// are separate realms. A profile whose handle carried instance state would either refuse here
    /// or leak between the two.
    /// </remarks>
    private static (string, bool, string) OneShareableHandleIsInstantiatedByTwoRuntimes()
    {
        const string Name = "one shareable handle is instantiated by two runtimes";

        // It mutates the realm before answering, so an instance that shared anything with the other
        // would answer differently the second time.
        var artifact = Compile(
            "globalThis.count = (globalThis.count || 0) + 1; String(globalThis.count);");

        if (artifact is null)
        {
            return (Name, false, "the program did not compile, so the check judged nothing");
        }

        using var runtimeA = Runtime();
        using var runtimeB = Runtime();

        if (runtimeA is null || runtimeB is null)
        {
            return (Name, false, "a runtime refused creation");
        }

        var descriptor = Descriptor();
        var verified = runtimeA.Verify(
            in descriptor, artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            return (Name, false, $"verification: {verified.Outcome}/{verified.Reason}");
        }

        if (!TryRun(runtimeA, handle, out var answerA, out var whyA))
        {
            return (Name, false, "the verifying runtime: " + whyA);
        }

        if (!TryRun(runtimeB, handle, out var answerB, out var whyB))
        {
            return (Name, false, "the second runtime: " + whyB);
        }

        var held =
            string.Equals(answerA, "1", System.StringComparison.Ordinal) &&
            string.Equals(answerB, "1", System.StringComparison.Ordinal);

        return (
            Name,
            held,
            $"the verifying runtime answered {answerA} and a second runtime instantiating the same " +
            $"handle answered {answerB}; each counted from zero, so neither realm was the other's");
    }

    /// <summary>
    /// Two threads read one handle at once with nothing between them, and every answer agrees.
    /// </summary>
    /// <remarks>
    /// <b>No synchronisation is the point.</b> The contract says everything reachable from a
    /// verified state is immutable once verification returns and safe for unsynchronised concurrent
    /// readers; a handle that lazily materialised anything would be a data race here rather than a
    /// wrong answer, so the check runs many short instantiations rather than one long one, which is
    /// where a race between a reader and a writer would fall.
    /// </remarks>
    private static (string, bool, string) TwoRuntimesReadOneHandleConcurrently()
    {
        const string Name = "two runtimes read one handle concurrently with no synchronisation";

        var artifact = Compile(
            "var total = 0; for (var i = 0; i < 32; i++) { total = total + i; } String(total);");

        if (artifact is null)
        {
            return (Name, false, "the program did not compile, so the check judged nothing");
        }

        using var owner = Runtime();

        if (owner is null)
        {
            return (Name, false, "a runtime refused creation");
        }

        var descriptor = Descriptor();
        var verified = owner.Verify(in descriptor, artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            return (Name, false, $"verification: {verified.Outcome}/{verified.Reason}");
        }

        var answers = new string?[2];
        var failures = new string?[2];

        void Read(int slot)
        {
            using var runtime = Runtime();

            if (runtime is null)
            {
                failures[slot] = "a runtime refused creation";
                return;
            }

            for (var index = 0; index < ConcurrentReads; index++)
            {
                if (!TryRun(runtime, handle, out var answer, out var why))
                {
                    failures[slot] = why;
                    return;
                }

                if (answers[slot] is { } seen && !string.Equals(seen, answer, System.StringComparison.Ordinal))
                {
                    failures[slot] = $"read {index} answered {answer} where an earlier one answered {seen}";
                    return;
                }

                answers[slot] = answer;
            }
        }

        var second = new System.Threading.Thread(() => Read(1)) { IsBackground = true };
        second.Start();
        Read(0);
        second.Join();

        if (failures[0] is { } firstFailure)
        {
            return (Name, false, "the first reader: " + firstFailure);
        }

        if (failures[1] is { } secondFailure)
        {
            return (Name, false, "the second reader: " + secondFailure);
        }

        var held =
            string.Equals(answers[0], "496", System.StringComparison.Ordinal) &&
            string.Equals(answers[1], "496", System.StringComparison.Ordinal);

        return (
            Name,
            held,
            $"{ConcurrentReads} reads on each of two threads, every one answering {answers[0]}, " +
            "with no lock between them and no reader taken from the other's runtime");
    }

    /// <summary>
    /// A handle answers a third runtime exactly what it answered the first, after two runs have
    /// changed everything their own realms hold.
    /// </summary>
    /// <remarks>
    /// This is the behavioural half of the gate's structural scan, and it is weaker in a way worth
    /// naming: it would not see a per-instance structure that is reachable from the handle and that
    /// these programs never observe. What it does see is the case that matters in practice — a
    /// handle that accumulated anything from the instances made out of it — and it sees it after
    /// the realms have been mutated rather than before.
    /// </remarks>
    private static (string, bool, string) AHandleCarriesNothingTheRunsThatUsedItChanged()
    {
        const string Name = "a handle carries nothing the runs that used it changed";

        // It writes a large object into its own realm, so an instance that wrote through to
        // anything the handle owns would be writing a lot of it.
        var artifact = Compile(
            "globalThis.store = globalThis.store || {};" +
            "for (var i = 0; i < 256; i++) { globalThis.store['k' + i] = i; }" +
            "String(Object.keys(globalThis.store).length);");

        if (artifact is null)
        {
            return (Name, false, "the program did not compile, so the check judged nothing");
        }

        using var owner = Runtime();

        if (owner is null)
        {
            return (Name, false, "a runtime refused creation");
        }

        var descriptor = Descriptor();
        var verified = owner.Verify(in descriptor, artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            return (Name, false, $"verification: {verified.Outcome}/{verified.Reason}");
        }

        var seen = new System.Collections.Generic.List<string>();

        for (var index = 0; index < 3; index++)
        {
            using var runtime = Runtime();

            if (runtime is null)
            {
                return (Name, false, "a runtime refused creation");
            }

            if (!TryRun(runtime, handle, out var answer, out var why))
            {
                return (Name, false, $"run {index}: {why}");
            }

            seen.Add(answer);
        }

        var held = seen.TrueForAll(
            static answer => string.Equals(answer, "256", System.StringComparison.Ordinal));

        return (
            Name,
            held,
            $"three runtimes over one handle answered {string.Join(", ", seen)}; each built its own " +
            "store from nothing. This is behavioural and not the structural scan the gate names: " +
            "a per-instance structure these programs never observe would not be caught here, and " +
            "what bounds that is the construction rather than this check");
    }

    /// <summary>Compiles one script, or answers nothing.</summary>
    private static byte[]? Compile(string source)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script)]);

        return compiled.Succeeded ? compiled.Artifact : null;
    }

    /// <summary>Verifies these bytes in this runtime and runs them, or says what refused.</summary>
    private static bool TryRun(VmRuntime runtime, byte[] artifact, out string answer, out string why)
    {
        var descriptor = Descriptor();
        var verified = runtime.Verify(
            in descriptor, artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            answer = string.Empty;
            why = $"verification: {verified.Outcome}/{verified.Reason}";
            return false;
        }

        return TryRun(runtime, handle, out answer, out why);
    }

    /// <summary>Instantiates this handle in this runtime and runs it, or says what refused.</summary>
    private static bool TryRun(
        VmRuntime runtime, VmVerifiedArtifact handle, out string answer, out string why)
    {
        answer = string.Empty;
        var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            why = $"instantiation: {instantiated.Outcome}/{instantiated.Reason}";
            return false;
        }

        using (instance)
        {
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

            var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);

            if (!JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
            {
                why = $"invocation: {invoked.Outcome}/{invoked.Reason}";
                return false;
            }

            answer = completion.Value;
            why = string.Empty;
            return true;
        }
    }

    /// <summary>The descriptor a caller presents with these bytes.</summary>
    private static VmArtifactDescriptor Descriptor() =>
        new(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

    /// <summary>A runtime over this profile, with nothing registered.</summary>
    private static VmRuntime? Runtime()
    {
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            catalog,
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: ImmutableArray<VmCapabilityRegistration>.Empty));

        return created.TryGetRuntime(out var runtime) ? runtime : null;
    }
}
