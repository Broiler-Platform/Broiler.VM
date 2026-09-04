// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Format;
using System.Collections.Immutable;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The feature manifest one <c>--test262</c> run is taken under, and the optional surfaces the
/// composition taking it admits.
/// </summary>
/// <remarks>
/// <para>
/// <b>A transcript that does not say what it was run under is not evidence.</b> The workload
/// roadmap's JSW-10 asks for a whole-suite run <b>per manifest</b>, so the manifest stops being an
/// implicit property of the code that happens to be compiled in and becomes a stated input of the
/// run - printed at the top of the transcript, written into the report, and compared across the
/// shards a merge is handed. Two runs under two manifests are two runs, and a merge that added them
/// would publish a figure describing neither.
/// </para>
/// <para>
/// <b>Two manifests and a set of optional surfaces, which are three different questions.</b> The
/// manifest an artifact NAMES is <c>broiler.javascript.slice</c> or
/// <c>broiler.javascript.wide</c>, and the two are lowered by two different front ends into two
/// different format versions. The optional surfaces - <c>broiler.javascript.binary</c> and
/// <c>broiler.javascript.dynamic</c> - are declared BESIDE the wide manifest by an artifact that
/// reaches them, and a composition declines one by building a descriptor that does not admit it.
/// Those identities exist for exactly that, so a run has to be able to take them away.
/// </para>
/// <para>
/// <b>The catalog is built once per run and not once per case.</b> Building a descriptor composes a
/// verifier, and a whole-suite run asks the question a hundred thousand times; the alternative -
/// calling <see cref="JavaScriptProfile.DescriptorAdmitting"/> inside the per-variant path, which
/// is where the wide runner's catalog used to be built - spends that composition on every variant
/// for an answer that cannot change inside one run.
/// </para>
/// </remarks>
internal sealed class Test262Manifest
{
    /// <summary>The manifest a run is taken under when nobody says otherwise.</summary>
    /// <remarks>
    /// <b>The wide manifest, because it is the one that admits every optional surface</b>, and a
    /// run that silently defaulted to the narrow one would report an <c>unsupported</c> column that
    /// says more about the default than about the engine.
    /// </remarks>
    internal const string Default = "broiler.javascript.wide";

    private Test262Manifest(
        VmFeatureManifestId id,
        uint formatVersion,
        bool loadsHarness,
        ImmutableArray<string> admitted,
        ImmutableArray<string> declined)
    {
        Id = id;
        FormatVersion = formatVersion;
        LoadsHarness = loadsHarness;
        Admitted = admitted;
        Declined = declined;

        var surfaces = new VmFeatureManifestId[admitted.Length];

        for (var index = 0; index < admitted.Length; index++)
        {
            surfaces[index] = VmFeatureManifestId.Parse(admitted[index]);
        }

        Catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorAdmitting(surfaces))
            .Build();
    }

    /// <summary>The manifest an artifact of this run names in its header.</summary>
    internal VmFeatureManifestId Id { get; }

    /// <summary>The format version that manifest is defined against.</summary>
    internal uint FormatVersion { get; }

    /// <summary>
    /// Whether the suite's harness files are loaded into the realm before the test.
    /// </summary>
    /// <remarks>
    /// <b>The wide manifest loads them and the slice cannot, and that is a property of the manifest
    /// rather than a choice this runner makes.</b> <c>broiler.javascript.slice</c> admits no call,
    /// so <c>assert.js</c> is refused by its own front end before it could be installed; loading it
    /// anyway would report the suite's own harness as an engine failure once per test.
    /// </remarks>
    internal bool LoadsHarness { get; }

    /// <summary>The optional surfaces this run's composition admits, in ordinal order.</summary>
    internal ImmutableArray<string> Admitted { get; }

    /// <summary>The optional surfaces this run's composition declines, in ordinal order.</summary>
    internal ImmutableArray<string> Declined { get; }

    /// <summary>The catalog this run verifies and executes against.</summary>
    internal VmCatalog Catalog { get; }

    /// <summary>Whether this run is taken under the wide manifest.</summary>
    internal bool IsWide => Id == JavaScriptProfile.WideManifest;

    /// <summary>
    /// The family a refusal for a declined surface is counted under.
    /// </summary>
    /// <remarks>
    /// <b>It names the set this run declined rather than the surface the artifact declared, because
    /// the verifier's answer does not carry the second one.</b>
    /// <see cref="JavaScriptDiagnosticCode.SurfaceOutsideComposition"/> says that some declared
    /// surface was not admitted; which one is in the artifact's surface section. The alternative
    /// would have been to decode that section here, which is re-implementing the verifier inside the
    /// harness that is meant to be reading its answers - and a second decoder that disagreed with
    /// the first would be a defect in the instrument. A run declining one surface therefore names it
    /// exactly, and a run declining two says so.
    /// </remarks>
    internal string DeclinedFamily =>
        "an artifact declaring " + string.Join(" or ", Declined) + ", which this composition declines";

    /// <summary>
    /// Reads the manifest and the declined surfaces off a command line, or says why it cannot.
    /// </summary>
    /// <remarks>
    /// <b>An unknown name is refused rather than defaulted.</b> A run asked for a manifest this
    /// build does not have would otherwise be taken under the default and reported under the name
    /// nobody implemented, which is the one way a per-manifest run can lie about which manifest it
    /// was.
    /// </remarks>
    internal static bool TryParse(
        string? named,
        IReadOnlyList<string> declined,
        out Test262Manifest manifest,
        out string failure)
    {
        manifest = null!;
        var name = named ?? Default;

        foreach (var surface in declined)
        {
            if (!JsSurfaces.IsKnown(surface))
            {
                failure =
                    $"`{surface}` is not an optional surface this build implements; it knows " +
                    string.Join(" and ", JsSurfaces.All);

                return false;
            }
        }

        var admitted = ImmutableArray.CreateBuilder<string>();

        foreach (var surface in JsSurfaces.All)
        {
            if (!declined.Contains(surface, StringComparer.Ordinal))
            {
                admitted.Add(surface);
            }
        }

        var declinedInOrder = ImmutableArray.CreateBuilder<string>();

        foreach (var surface in JsSurfaces.All)
        {
            if (declined.Contains(surface, StringComparer.Ordinal))
            {
                declinedInOrder.Add(surface);
            }
        }

        if (string.Equals(name, JavaScriptProfile.WideManifest.ToString(), StringComparison.Ordinal))
        {
            manifest = new Test262Manifest(
                JavaScriptProfile.WideManifest,
                JsFormat.FormatVersion,
                loadsHarness: true,
                admitted.ToImmutable(),
                declinedInOrder.ToImmutable());

            failure = string.Empty;
            return true;
        }

        if (string.Equals(name, JavaScriptProfile.SliceManifest.ToString(), StringComparison.Ordinal))
        {
            manifest = new Test262Manifest(
                JavaScriptProfile.SliceManifest,
                JavaScriptFormat.MinimumFormatVersion,
                loadsHarness: false,
                admitted.ToImmutable(),
                declinedInOrder.ToImmutable());

            failure = string.Empty;
            return true;
        }

        failure =
            $"`{name}` is not a manifest this build runs a suite under; it runs " +
            $"{JavaScriptProfile.SliceManifest} and {JavaScriptProfile.WideManifest}";

        return false;
    }

    /// <summary>
    /// Rebuilds a manifest from what a report recorded, so a merge reads one rather than trusting a
    /// name.
    /// </summary>
    internal static Test262Manifest Recorded(
        string id,
        uint formatVersion,
        bool loadsHarness,
        IReadOnlyList<string> admitted,
        IReadOnlyList<string> declined) =>
        new(
            VmFeatureManifestId.Parse(id),
            formatVersion,
            loadsHarness,
            [.. admitted],
            [.. declined]);

    /// <summary>The lines a run prints before it scores anything, and a report writes.</summary>
    /// <remarks>
    /// <b>Every one of them is an input the totals depend on.</b> The manifest decides which front
    /// end lowered the source, the format version decides which artifact shape the verifier admits,
    /// the harness line decides whether the suite's assertion library was in the realm, and the
    /// admitted set decides what a program reaching a typed array meets. A reader handed four
    /// verdicts and none of these is being handed a number with no question attached to it.
    /// </remarks>
    internal string Describe() =>
        Describe(Id.ToString(), FormatVersion, LoadsHarness, Admitted, Declined);

    /// <summary>The same line, written from what a report recorded rather than from a manifest.</summary>
    /// <remarks>
    /// <b>One formatter and two callers, because a merged report and the run that produced it must
    /// describe the composition in the same words.</b> Two renderings of one sentence is how a
    /// transcript and the report beside it come to disagree about which surfaces were admitted.
    /// </remarks>
    internal static string Describe(
        string id,
        uint formatVersion,
        bool loadsHarness,
        IReadOnlyList<string> admitted,
        IReadOnlyList<string> declined) =>
        "manifest " + id +
        " at format version " + formatVersion.ToString(CultureInfo.InvariantCulture) +
        "; harness " + (loadsHarness ? "loaded" : "not loaded") +
        "; admitted surfaces " + (admitted.Count == 0 ? "(none)" : string.Join(", ", admitted)) +
        "; declined " + (declined.Count == 0 ? "(none)" : string.Join(", ", declined));
}
