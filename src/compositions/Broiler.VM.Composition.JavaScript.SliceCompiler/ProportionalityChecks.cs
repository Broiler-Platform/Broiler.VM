// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// What each operation family whose cost grows with its input is charged, measured.
/// </summary>
/// <remarks>
/// <para>
/// <b>Roadmap section 8 names seven families and asks each for a declared monotone non-decreasing
/// charging function, a granularity, and a retained fixture with an unsimplified control</b>, and
/// makes the consequence explicit: <i>an operation family without a proportionality fixture does
/// not ship in the increment</i>. JS-5's gate repeats it. Until this file there was no fixture for
/// any of them.
/// </para>
/// <para>
/// <b>The charge is measured from outside, by bisecting the allowance.</b> Nothing in the public
/// surface reports what an invocation spent, and adding a reporter to the profile so that a check
/// could read it would put a measurement channel in the product. So each program is run against a
/// fuel ceiling and the ceiling is bisected: the smallest allowance the program completes under
/// <b>is</b> what it costs. That needs no new surface and cannot disagree with what a host would
/// meet, because it is what a host would meet.
/// </para>
/// <para>
/// <b>Each family has a control, and the control is the same program minus the operation.</b>
/// Building the input is itself proportional to the input — a loop that appends <i>n</i> characters
/// costs <i>n</i> whatever is done with the result — so a fixture reading the candidate alone would
/// report the setup and call it the operation. The difference between the two is what is
/// attributed, which is measurement rule 1 applied to a charge rather than to a duration.
/// </para>
/// <para>
/// <b>What is asserted is the shape and a floor, and the two catch different defects.</b> The shape
/// is that doubling the input at least half again the charge, which is what a flat charge fails —
/// and two families did fail it: string comparison cost sixteen units for any length, and reading a
/// number out of a string cost the same for four digits as for five hundred. The floor is the
/// declared function, which catches a charge that grows but grows below what the work costs. The
/// declared granularity is one, so the floor is the function itself rather than a ceiling over a
/// window.
/// </para>
/// <para>
/// <b>Structured cloning is the seventh family and it does not ship</b>, so it has no fixture here
/// and needs none: there is no <c>structuredClone</c> in this realm, and the rule is that a family
/// without a fixture does not ship rather than that every named family must have one.
/// </para>
/// </remarks>
internal static class ProportionalityChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://proportionality";

    /// <summary>The input magnitudes each family is measured at.</summary>
    /// <remarks>
    /// Doubling, so the shape assertion is a comparison between neighbours rather than a fit. Four
    /// points, because three would let one outlier decide the answer and eight would cost the run
    /// without saying anything the fourth does not.
    /// </remarks>
    private static readonly int[] Magnitudes = [32, 64, 128, 256];

    /// <summary>The largest allowance a bisection considers.</summary>
    private const ulong Ceiling = 8_000_000;

    /// <summary>One family: what it is, what it is declared to cost, and how to run it.</summary>
    private readonly record struct Family(
        string Name,
        string DeclaredFunction,
        System.Func<int, ulong> Floor,
        System.Func<int, string> Candidate,
        System.Func<int, string> Control);

    /// <summary>The families this increment ships, with the function each declares.</summary>
    /// <remarks>
    /// Every floor is stated per program rather than per operation, because that is what a
    /// bisection can see: a family whose fixture performs an operation eight times declares eight
    /// times the per-operation function. The constants are the honest lower bound on the work and
    /// not the measured charge — a floor set to what was measured would pass by construction and
    /// would fail the day an implementation got cheaper without getting wrong.
    /// </remarks>
    private static readonly Family[] Families =
    [
        new(
            "string concatenation",
            "n characters appended, one unit each",
            static n => (ulong)n,
            static n => $"var s='';for(var i=0;i<{n};i++){{s=s+'x';}}s.length;",
            static n => $"var s='';for(var i=0;i<{n};i++){{s='x';}}s.length;"),
        new(
            "string comparison",
            "8 comparisons over min(|a|,|b|) = n characters",
            static n => 8UL * (ulong)n,
            static n => $"var a='x'.repeat({n}),b='x'.repeat({n}-1)+'y';var r=false;" +
                        "for(var i=0;i<8;i++){r=a<b;}r;",
            static n => $"var a='x'.repeat({n}),b='x'.repeat({n}-1)+'y';var r=false;" +
                        "for(var i=0;i<8;i++){r=false;}r;"),
        new(
            "string equality",
            "8 comparisons over min(|a|,|b|) = n characters",
            static n => 8UL * (ulong)n,
            static n => $"var a='x'.repeat({n}),b='x'.repeat({n});var r=false;" +
                        "for(var i=0;i<8;i++){r=a===b;}r;",
            static n => $"var a='x'.repeat({n}),b='x'.repeat({n});var r=false;" +
                        "for(var i=0;i<8;i++){r=false;}r;"),
        new(
            "array copy",
            "n elements copied, one unit each",
            static n => (ulong)n,
            static n => $"var a=[];for(var i=0;i<{n};i++)a.push(i);var b=a.slice();b.length;",
            static n => $"var a=[];for(var i=0;i<{n};i++)a.push(i);var b=a;b.length;"),
        new(
            "array sort",
            "n elements ordered, at least one unit each",
            static n => (ulong)n,
            static n => $"var a=[];for(var i=0;i<{n};i++)a.push(({n}-i)%997);a.sort();a.length;",
            static n => $"var a=[];for(var i=0;i<{n};i++)a.push(({n}-i)%997);a.length;"),
        new(
            "property enumeration",
            "n own keys listed, one unit each",
            static n => (ulong)n,
            static n => $"var o={{}};for(var i=0;i<{n};i++)o['k'+i]=i;Object.keys(o).length;",
            static n => $"var o={{}};for(var i=0;i<{n};i++)o['k'+i]=i;0;"),
        new(
            "regular-expression matching",
            "4 matches over a subject of n characters",
            static n => 4UL * (ulong)n,
            static n => $"var s='a'.repeat({n});var re=/a+b?/;var m=0;" +
                        "for(var i=0;i<4;i++){if(re.exec(s))m++;}m;",
            static n => $"var s='a'.repeat({n});var re=/a+b?/;var m=0;" +
                        "for(var i=0;i<4;i++){m++;}m;"),
        new(
            "numeric conversion of large values",
            "8 conversions over a numeral of n digits",
            static n => 8UL * (ulong)n,
            static n => $"var s='1'.repeat({n});var t=0;for(var i=0;i<8;i++){{t+=Number(s);}}t>0;",
            static n => $"var s='1'.repeat({n});var t=0;for(var i=0;i<8;i++){{t+=1;}}t>0;"),
    ];

    /// <summary>Runs one check per shipped family.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run()
    {
        var checks = new System.Collections.Generic.List<(string, bool, string)>(Families.Length);

        foreach (var family in Families)
        {
            checks.Add(Measure(family));
        }

        return checks;
    }

    /// <summary>Measures one family at every magnitude and judges the series.</summary>
    private static (string, bool, string) Measure(Family family)
    {
        var name = $"{family.Name} is charged proportionally to its input";
        var attributed = new ulong[Magnitudes.Length];

        for (var index = 0; index < Magnitudes.Length; index++)
        {
            var magnitude = Magnitudes[index];
            var candidate = Cost(family.Candidate(magnitude));
            var control = Cost(family.Control(magnitude));

            if (candidate is null || control is null)
            {
                return (name, false, $"at n={magnitude} a program did not complete under {Ceiling}");
            }

            if (candidate.Value < control.Value)
            {
                return (
                    name,
                    false,
                    $"at n={magnitude} the control cost {control.Value} and the candidate " +
                    $"{candidate.Value}, so the operation attributed nothing");
            }

            attributed[index] = candidate.Value - control.Value;
        }

        var complaints = new System.Collections.Generic.List<string>();

        for (var index = 0; index < Magnitudes.Length; index++)
        {
            var floor = family.Floor(Magnitudes[index]);

            if (attributed[index] < floor)
            {
                complaints.Add(
                    $"at n={Magnitudes[index]} the charge {attributed[index]} is below the declared " +
                    $"floor {floor}");
            }

            if (index == 0)
            {
                continue;
            }

            // MONOTONE, AND MORE THAN MONOTONE. A flat charge is monotone; what separates a
            // proportional charge from a flat one is that doubling the input costs materially more,
            // and half again is the weakest statement of that which a linear family always meets.
            if (attributed[index] < attributed[index - 1])
            {
                complaints.Add(
                    $"the charge fell from {attributed[index - 1]} to {attributed[index]} between " +
                    $"n={Magnitudes[index - 1]} and n={Magnitudes[index]}");
            }
            else if (attributed[index] * 2 < attributed[index - 1] * 3)
            {
                complaints.Add(
                    $"doubling n from {Magnitudes[index - 1]} to {Magnitudes[index]} took the charge " +
                    $"only from {attributed[index - 1]} to {attributed[index]}");
            }
        }

        var series = string.Join(
            ", ",
            System.Linq.Enumerable.Select(
                System.Linq.Enumerable.Range(0, Magnitudes.Length),
                index => $"n={Magnitudes[index]}:{attributed[index]}"));

        return (
            name,
            complaints.Count == 0,
            complaints.Count == 0
                ? $"declared {family.DeclaredFunction}, granularity 1; measured against its own " +
                  $"control at {series}"
                : string.Join("; ", complaints) + $" (measured {series})");
    }

    /// <summary>
    /// The smallest fuel allowance this program completes under, or nothing if it needs more than
    /// the bisection considers.
    /// </summary>
    /// <remarks>
    /// The invariant is the ordinary one: <c>low</c> does not complete and <c>high</c> does, so the
    /// loop ends with <c>high</c> the smallest allowance that does. A program is a total function
    /// of its allowance here — nothing it runs depends on a clock, a thread or an address — so the
    /// bisection is over a monotone predicate rather than over a flaky one.
    /// </remarks>
    private static ulong? Cost(string source)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script)]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return null;
        }

        if (!Completes(compiled.Artifact, Ceiling))
        {
            return null;
        }

        ulong low = 0;
        var high = Ceiling;

        while (high - low > 1)
        {
            var middle = low + ((high - low) / 2);

            if (Completes(compiled.Artifact, middle))
            {
                high = middle;
            }
            else
            {
                low = middle;
            }
        }

        return high;
    }

    /// <summary>Whether these bytes run to a normal completion under this allowance.</summary>
    private static bool Completes(byte[] artifact, ulong fuel)
    {
        using var runtime = Runtime(fuel);

        if (runtime is null)
        {
            return false;
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(
            in descriptor, artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            return false;
        }

        var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

        if (!instantiated.TryGetInstance(out var instance))
        {
            return false;
        }

        using (instance)
        {
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

            var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);
            return invoked.Outcome is VmOutcome.Normal;
        }
    }

    /// <summary>A runtime granted exactly this much fuel and the profile's defaults otherwise.</summary>
    /// <remarks>
    /// Verification spends fuel too, and it spends the same amount for every trial over one
    /// program, so it is a constant inside the bisection and cancels between candidate and control
    /// only to the extent the two artifacts are the same size. What it cannot do is make a flat
    /// charge look proportional: it does not grow with <c>n</c> in the way the operation does, and
    /// two families measured flat here before their charging was repaired.
    /// </remarks>
    private static VmRuntime? Runtime(ulong fuel)
    {
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
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
