// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>
/// The one property that makes the module surface declinable: two hosts, one artifact, two answers.
/// </summary>
/// <remarks>
/// <para>
/// <b>Both directions, over the same bytes, in one check.</b> A check that only showed the refusal
/// would pass just as well against a build that refused every module artifact, and one that only
/// showed the acceptance would pass against a build with no decline at all. What roadmap section 6
/// asks for is that the composition decides, so what is asserted is that the SAME payload verifies
/// under a host that registered a resolver and is refused, by name, under one that did not.
/// </para>
/// <para>
/// <b>The refusal is at verification and not at the first import</b>, which is the clause the
/// module manifest is minted for. A run-time <c>ReferenceError</c> would be a composition
/// discovering after instantiation that it could not do what it had already admitted, and bundle
/// JS-4-001 records exactly that difference as the outstanding gap for <c>eval</c>.
/// </para>
/// </remarks>
internal static class ModuleSurfaceChecks
{
    /// <summary>The retained entry both halves of the check are run over.</summary>
    private const string Entry = "modules-a-module-artifact-a-composition-declined";

    /// <summary>Runs the check.</summary>
    internal static (string Name, bool Passed, string Detail)[] Run(string corpus)
    {
        var path = Path.Combine(corpus, Entry + ".bjsb");

        if (!File.Exists(path))
        {
            return
            [
                (
                    "a composition that registers no module resolver refuses a module artifact",
                    false,
                    $"no retained entry at {path}"
                ),
            ];
        }

        var bytes = File.ReadAllBytes(path);
        var descriptor = Hosts.Descriptor(Hosts.ModuleAdmittedMode);

        var admitted = Verify(Hosts.ModuleAdmittedMode, descriptor, bytes);
        var declined = Verify(Hosts.ModuleDeclinedMode, descriptor, bytes);

        var passed =
            admitted.Outcome == VmOutcome.Normal &&
            declined.Outcome == VmOutcome.InvalidArtifact &&
            declined.Reason == VmReason.UnsupportedFeatureManifest &&
            declined.Code == (int)JavaScriptDiagnosticCode.ModuleResolverAbsent;

        return
        [
            (
                "a composition that registers no module resolver refuses a module artifact",
                passed,
                $"registered: {admitted.Outcome}/{admitted.Reason}/{admitted.Code}; " +
                $"not registered: {declined.Outcome}/{declined.Reason}/{declined.Code}"
            ),
        ];
    }

    /// <summary>Verifies the bytes under one host and answers what it said.</summary>
    private static (VmOutcome Outcome, VmReason Reason, int Code) Verify(
        string mode, in VmArtifactDescriptor descriptor, byte[] bytes)
    {
        using var runtime = Hosts.Runtime(mode, out var failure);

        if (runtime is null)
        {
            return (VmOutcome.InvalidState, VmReason.ProfileContractViolation, 0);
        }

        _ = failure;
        var verified = runtime.Verify(in descriptor, bytes, CancellationToken.None);

        return (
            verified.Outcome, verified.Reason, verified.Diagnostics.ProfileDiagnosticCode);
    }
}
