// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.ExecutionOnly;

/// <summary>
/// The two ways a composition can fail to run a module, over one artifact, in one check.
/// </summary>
/// <remarks>
/// <para>
/// <b>Three hosts, one payload, three answers.</b> A check that only showed a refusal would pass
/// just as well against a build that refused every module artifact, and one that only showed the
/// acceptance would pass against a build with no decline at all. So the SAME bytes are put to a
/// composition that admits the module surface and registers a resolver, to one that declines the
/// surface, and to one that admits it and registers nothing - and each has to answer differently.
/// </para>
/// <para>
/// <b>The two refusals are not the same event and the codes say which is which.</b> Declining the
/// surface is <c>SurfaceOutsideComposition</c>, which every optional surface answers with;
/// admitting it and registering no resolver is <c>ModuleResolverAbsent</c>, which only this one
/// can. Both are at verification rather than at the first import, which is the property roadmap
/// section 6 distinguishes from a run-time refusal the guest may catch.
/// </para>
/// </remarks>
internal static class ModuleSurfaceChecks
{
    /// <summary>The retained entry every part of the check is run over.</summary>
    private const string Entry = "modules-a-module-artifact-with-no-resolver";

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
        var declined = Verify("wide-declining", descriptor, bytes);
        var unresolved = Verify(Hosts.ModuleUnresolvedMode, descriptor, bytes);

        var passed =
            admitted.Outcome == VmOutcome.Normal &&
            declined.Outcome == VmOutcome.InvalidArtifact &&
            declined.Reason == VmReason.UnsupportedFeatureManifest &&
            declined.Code == (int)JavaScriptDiagnosticCode.SurfaceOutsideComposition &&
            unresolved.Outcome == VmOutcome.InvalidArtifact &&
            unresolved.Reason == VmReason.UnsupportedFeatureManifest &&
            unresolved.Code == (int)JavaScriptDiagnosticCode.ModuleResolverAbsent;

        return
        [
            (
                "a composition declining modules, and one with no resolver, each refuse by name",
                passed,
                $"admitted: {admitted.Outcome}/{admitted.Reason}/{admitted.Code}; " +
                $"surface declined: {declined.Outcome}/{declined.Reason}/{declined.Code}; " +
                $"no resolver: {unresolved.Outcome}/{unresolved.Reason}/{unresolved.Code}"
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
