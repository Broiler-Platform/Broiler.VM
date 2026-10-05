using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
#if BROILER_VM_INTL
using Broiler.VM.Profile.JavaScript.Intl;
#endif
using System.Collections.Immutable;

namespace Broiler.VM.Sample.JavaScriptConsumer;

/// <summary>
/// A consumer of the JavaScript profile's packages, reaching them only through a feed (phase F9
/// slice R3, JSD-0059).
/// </summary>
/// <remarks>
/// It composes the profile from its packages, lowers two programs, verifies and runs them, and
/// checks two refusals - one the lowering makes and one the verifier makes - because a consumer
/// that only saw success would not know the refusals reach it intact. Built with the
/// internationalization data, it also composes the profile over that package and runs an
/// <c>Intl</c> program and a <c>Temporal</c> one. It prints what it did and exits non-zero if
/// anything went wrong.
/// </remarks>
internal static class Program
{
    private const string Caller = "broiler-vm-sample://javascript-consumer";

    private static int Main()
    {
        var checks = new List<(string Name, bool Passed, string Detail)>();

        Console.WriteLine($"# broiler-vm-sample-javascript core-contract-version={VmCoreContract.Version}");

        var assemblies = new List<System.Reflection.Assembly> { typeof(VmRuntime).Assembly, typeof(JavaScriptProfile).Assembly, typeof(JsCompiler).Assembly };
#if BROILER_VM_INTL
        assemblies.Add(typeof(JsCldrData).Assembly);
#endif

        foreach (var assembly in assemblies)
        {
            var name = assembly.GetName();
            var informational = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
                .OfType<System.Reflection.AssemblyInformationalVersionAttribute>().FirstOrDefault()?.InformationalVersion ?? "-";
            Console.WriteLine($"# assembly {name.Name} {informational}");
        }

        checks.Add(Answers("one-plus-one", "1 + 1", "2"));
        checks.Add(Answers(
            "a-program", "var a = [3, 1, 2]; a.sort(); a.join() + ':' + JSON.stringify({ x: [1, 'two'] })", "1,2,3:{\"x\":[1,\"two\"]}"));
        checks.Add(LoweringRefuses("a-syntax-error", "1 +"));
        checks.Add(VerifierRefuses("a-truncated-artifact"));
#if BROILER_VM_INTL
        var composing = JavaScriptProfile.DescriptorComposing(new JsComposition { IntlData = JsCldrData.Instance });
        checks.Add(Answers(
            "intl-from-its-package",
            "new Intl.NumberFormat('de-DE').format(1234567.891) + '|' + new Intl.DateTimeFormat('en-US', { timeZone: 'UTC', dateStyle: 'long' }).format(0)",
            "1.234.567,891|January 1, 1970",
            composing));
        checks.Add(Answers(
            "temporal-from-its-package",
            "Temporal.PlainDate.from('2026-10-05').add({ days: 30 }).toString()",
            "2026-11-04",
            composing));
#else
        Console.WriteLine("# built without the internationalization data: the Intl and Temporal checks are not run");
#endif

        var failed = 0;

        foreach (var (name, passed, detail) in checks)
        {
            failed += passed ? 0 : 1;
            Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
        }

        Console.WriteLine(failed == 0
            ? $"broiler-vm-sample-javascript: {checks.Count} checks passed"
            : $"broiler-vm-sample-javascript: {failed} of {checks.Count} checks FAILED");

        return failed == 0 ? 0 : 1;
    }

    private static (string, bool, string) Answers(string name, string source, string expected, VmProfileDescriptor? descriptor = null)
    {
        var bytes = Lower(source, out var refusal);

        if (bytes is null)
        {
            return (name, false, "the lowering refused it: " + refusal);
        }

        var answer = Run(bytes, descriptor);
        return (name, answer == expected, $"answered {answer}");
    }

    private static (string, bool, string) LoweringRefuses(string name, string source)
    {
        var bytes = Lower(source, out var refusal);
        return (name, bytes is null, bytes is null ? "refused: " + refusal : "lowered, where a refusal was expected");
    }

    private static (string, bool, string) VerifierRefuses(string name)
    {
        var bytes = Lower("1 + 1", out _)!;
        var answer = Run(bytes[..(bytes.Length / 2)]);
        return (name, answer.StartsWith("verification refused: InvalidArtifact", StringComparison.Ordinal), answer);
    }

    private static byte[]? Lower(string source, out string refusal)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script, false, Caller)], [], new JsCompileRequest());

        refusal = compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString();
        return compiled.Succeeded ? compiled.Artifact : null;
    }

    private static string Run(byte[] bytes, VmProfileDescriptor? descriptor = null)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            VmCatalog.CreateBuilder().Add(descriptor ?? JavaScriptProfile.Descriptor).Build(),
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: ImmutableArray<VmCapabilityRegistration>.Empty));

        if (!created.TryGetRuntime(out var runtime))
        {
            return $"the runtime refused creation: {created.Outcome}/{created.Reason}";
        }

        using (runtime)
        {
            var artifactDescriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));
            var verified = runtime.Verify(in artifactDescriptor, bytes, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return $"verification refused: {verified.Outcome}/{verified.Reason}";
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    return $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}";
                }

                using (instance)
                {
                    var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));
                    var result = instance.Invoke(in request, CancellationToken.None);

                    return JavaScriptProfile.TryGetWideCompletion(in result, out var completion) ? completion.Value
                        : JavaScriptProfile.TryGetUncaught(in result, out var uncaught) ? "uncaught " + uncaught.Message
                        : $"{result.Outcome}/{result.Reason}";
                }
            }
        }
    }
}
