// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;
using System.Text;
using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// <c>--vectors &lt;archive&gt; --expect &lt;pin&gt;</c>: this profile's engine-level run of the
/// shared conformance vectors (programme milestone SJL-1).
/// </summary>
/// <remarks>
/// <para>
/// <b>The corpus is Broiler.JS's, and this repository holds it as a pinned archive.</b>
/// <c>src/tests/vectors/pins/vectors.pin</c> names the Broiler.JS commit, the archive's digest and
/// the digest of the files it extracts to. This mode reads the archive itself, in memory, and
/// refuses it before any vector runs unless both digests and the file count are the pin's. Rule N13
/// lists <c>tests/vectors</c> among the suite directories no project file may name, so the corpus
/// reaches this root on a command line and nowhere else.
/// </para>
/// <para>
/// <b>The negative control is <c>--mutate &lt;id&gt;</c></b>: after the pinned archive is verified,
/// the named vector's expected value is changed in the in-memory copy, and the run must report that
/// vector as a difference nothing classifies.
/// </para>
/// <para>
/// <b>The rendering is the corpus's, not this harness's.</b> The corpus's SCHEMA.md section 7 is
/// the one rule Broiler.JS's runner follows too: each vector runs in a fresh realm, the prelude and
/// then the vector's script as two Scripts, and the outcome is written from the host's own view of
/// the value - a Number by its bits, a String by its code units, a BigInt by its integer - never by
/// asking the engine to format it. A thrown object is an error object when it has an
/// [[ErrorData]] slot, which the host surface does not expose, so it is asked of
/// <c>Object.prototype.toString</c>, whose built-in tag is <c>Error</c> for exactly those objects;
/// the function is read before the vector's script runs, so the script cannot replace it.
/// </para>
/// <para>
/// <b>Its realms admit every surface the profile builds, Intl and Temporal included</b>, with the
/// internationalization data the test262 runs hand over. Until corpus revision 6 they admitted every
/// surface but those two, which need the data; the numerics area's rounding vectors run through
/// Temporal. <i>(Added 2026-10-13, at milestone SJL-4.2.)</i>
/// </para>
/// <para>
/// <b>Every vector gets a runtime of its own</b>, for the reason <see cref="Execution"/> gives: a
/// fuel allowance is spent over a runtime's life, so a vector that does not terminate would
/// otherwise be charged to every vector after it. One that spends its allowance is rendered
/// <c>abort:</c>, the engine's result for that vector.
/// </para>
/// </remarks>
internal static class VectorsCommand
{
    /// <summary>The engine name the corpus's per-engine and divergence fields use for this profile.</summary>
    private const string Engine = "broiler-vm";

    private const string Caller = "broiler-js-conformance://vectors";

    /// <summary>The directory of the corpus's home every file of the archive lies under.</summary>
    private const string Home = "conformance/vectors/";

    private const string Separator = " | ";

    internal static int Run(string archive, string[] args, bool verbose)
    {
        var expect = Argument(args, "--expect");

        if (expect is null)
        {
            Console.WriteLine("broiler-js-conformance: --vectors takes --expect <retained pin>; an unpinned corpus is not run");
            return ExitCodes.Usage;
        }

        var pin = RetainedSuitePin.Read(expect, out var complaints);

        if (pin is null)
        {
            foreach (var complaint in complaints)
            {
                Console.WriteLine("broiler-js-conformance: " + complaint);
            }

            return ExitCodes.Failed;
        }

        // THE ARCHIVE IS CHECKED BEFORE ANY VECTOR RUNS, both ways the pin names it: the bytes as
        // retrieved, and every file they extract to with its path.
        var bytes = File.ReadAllBytes(archive);
        var archiveDigest = Suite.Sha256(bytes);

        if (!string.Equals(archiveDigest, pin.ArchiveDigest, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(
                $"broiler-js-conformance: {archive} hashes to `{archiveDigest}` and the retained pin names `{pin.ArchiveDigest}`");
            return ExitCodes.Failed;
        }

        if (!Extract(bytes, out var entries, out var unreadable))
        {
            Console.WriteLine("broiler-js-conformance: " + unreadable);
            return ExitCodes.Failed;
        }

        var files = entries.Select(static entry => new SuiteFile(entry.Key, Suite.Sha256(entry.Value))).ToList();
        var disagreements = pin.Disagrees(Suite.Digest(files), files.Count);

        if (disagreements.Count != 0)
        {
            foreach (var disagreement in disagreements)
            {
                Console.WriteLine("broiler-js-conformance: " + disagreement);
            }

            return ExitCodes.Failed;
        }

        Console.WriteLine("broiler-js-conformance: vectors " + pin.Describe());

        var vectors = new List<Vector>();

        foreach (var file in entries.Keys
                     .Where(static path => path.EndsWith(".vectors", StringComparison.Ordinal))
                     .Order(StringComparer.Ordinal))
        {
            if (!Read(Encoding.UTF8.GetString(entries[file]), file, vectors, out var failure))
            {
                Console.WriteLine("broiler-js-conformance: " + failure);
                return ExitCodes.Failed;
            }
        }

        // THE NEGATIVE CONTROL (SJL-1.6): one vector's expected value is changed in memory, and the
        // run must report that vector as a difference nothing classifies.
        if (Argument(args, "--mutate") is { } mutated)
        {
            var index = vectors.FindIndex(vector => string.Equals(vector.Id, mutated, StringComparison.Ordinal));

            if (index < 0)
            {
                Console.WriteLine($"broiler-js-conformance: --mutate names `{mutated}`, which is no vector of the corpus");
                return ExitCodes.Usage;
            }

            vectors[index] = vectors[index] with { Out = Mutate(vectors[index].Out) };
            Console.WriteLine($"broiler-js-conformance: negative control: {mutated} expects `{vectors[index].Out}`");
        }

        if (!entries.TryGetValue("prelude.js", out var preludeBytes))
        {
            Console.WriteLine("broiler-js-conformance: the archive holds no prelude.js");
            return ExitCodes.Failed;
        }

        var prelude = Encoding.UTF8.GetString(preludeBytes);
        var fuel = Number(args, "--fuel", Test262Command.DefaultFuel);
        var wallClock = Number(args, "--wall", Test262Command.DefaultWallClock);
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", "undefined;", SliceParseOptions.Script, false, Caller)],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            Console.WriteLine("broiler-js-conformance: the vectors' host program does not compile");
            return ExitCodes.HarnessDefect;
        }

        var report = new StringBuilder();
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        var lines = new List<string>();

        foreach (var vector in vectors)
        {
            var rendered = Evaluate(compiled.Artifact, prelude, vector.Script, fuel, wallClock);
            var expected = vector.Class == "impl-defined" && vector.PerEngine is { } own ? own : vector.Out;
            var verdict = Compare(vector, expected, rendered);
            var noLongerHolds = verdict == "match" && vector.Divergence is not null;
            counts[verdict] = counts.GetValueOrDefault(verdict) + 1;

            if (noLongerHolds)
            {
                counts["divergence no longer holds"] = counts.GetValueOrDefault("divergence no longer holds") + 1;
            }

            lines.Add($"{vector.Id}\t{vector.Class}\t{expected}\t{rendered}\t{verdict}{(noLongerHolds ? " (divergence no longer holds)" : string.Empty)}");

            if (verbose && verdict == "difference")
            {
                Console.WriteLine($"difference {vector.Id}: expected {expected}, rendered {rendered}");
            }
        }

        report.Append(CultureInfo.InvariantCulture, $"# engine: {Engine}\n");
        report.Append(CultureInfo.InvariantCulture, $"# corpus: {pin.Upstream}@{pin.Revision} content sha256 {pin.ContentDigest}\n");
        report.Append(CultureInfo.InvariantCulture, $"# vectors: {vectors.Count}\n");

        foreach (var name in (string[])["match", "classified-divergence", "difference", "divergence no longer holds"])
        {
            report.Append(CultureInfo.InvariantCulture, $"# {name}: {counts.GetValueOrDefault(name)}\n");
        }

        report.Append("id\tclass\texpected\trendered\tverdict\n");

        foreach (var line in lines)
        {
            report.Append(line).Append('\n');
        }

        if (Argument(args, "--report") is { } output)
        {
            File.WriteAllText(output, report.ToString(), new UTF8Encoding(false));
        }

        var differences = counts.GetValueOrDefault("difference");

        Console.WriteLine(
            $"broiler-js-conformance: {vectors.Count} vectors: {counts.GetValueOrDefault("match")} match, " +
            $"{counts.GetValueOrDefault("classified-divergence")} classified divergences, {differences} unclassified differences");

        return differences == 0 ? ExitCodes.Ok : ExitCodes.Failed;
    }

    /// <summary>One vector, as much of it as an engine-level run reads.</summary>
    private sealed record Vector(string Id, string Class, string Out, string? PerEngine, string? Divergence, string Script);

    /// <summary>
    /// The archive's regular files, by their path below the corpus's home, as <c>git archive</c> of a
    /// Broiler.JS commit wrote them. An entry outside the home, or of any other kind, refuses it.
    /// </summary>
    private static bool Extract(byte[] bytes, out SortedDictionary<string, byte[]> entries, out string failure)
    {
        entries = new SortedDictionary<string, byte[]>(StringComparer.Ordinal);
        using var gzip = new System.IO.Compression.GZipStream(new MemoryStream(bytes), System.IO.Compression.CompressionMode.Decompress);
        using var tar = new System.Formats.Tar.TarReader(gzip);

        while (tar.GetNextEntry(copyData: true) is { } entry)
        {
            if (entry.EntryType is System.Formats.Tar.TarEntryType.Directory or System.Formats.Tar.TarEntryType.GlobalExtendedAttributes)
            {
                continue;
            }

            var name = Suite.Normalize(entry.Name);

            if (entry.EntryType is not (System.Formats.Tar.TarEntryType.RegularFile or System.Formats.Tar.TarEntryType.V7RegularFile) ||
                !name.StartsWith(Home, StringComparison.Ordinal) || entry.DataStream is null)
            {
                failure = $"the archive's entry `{entry.Name}` is not a file of {Home}";
                return false;
            }

            using var content = new MemoryStream();
            entry.DataStream.CopyTo(content);

            if (!entries.TryAdd(name[Home.Length..], content.ToArray()))
            {
                failure = $"the archive holds `{entry.Name}` twice";
                return false;
            }
        }

        failure = string.Empty;
        return true;
    }

    /// <summary>Reads one vector file under SCHEMA.md section 1. The home's validator checks the rest.</summary>
    private static bool Read(string text, string name, List<Vector> vectors, out string failure)
    {
        var lines = text.Split('\n');
        var schema = false;
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);

        for (var index = 0; index <= lines.Length; index++)
        {
            var line = index < lines.Length ? lines[index] : string.Empty;

            if (line.StartsWith('#'))
            {
                continue;
            }

            if (!schema)
            {
                if (index == lines.Length || !string.Equals(line, "schema: 1", StringComparison.Ordinal))
                {
                    failure = $"{name}:{index + 1}: the first line that is not a comment is not `schema: 1`";
                    return false;
                }

                schema = true;
                continue;
            }

            if (line.Length == 0)
            {
                if (fields.Count != 0)
                {
                    if (!fields.TryGetValue("id", out var id) || !fields.TryGetValue("class", out var @class) ||
                        !fields.TryGetValue("out", out var expected) || !fields.TryGetValue("script", out var script))
                    {
                        failure = $"{name}:{index}: a record lacks id, class, out or script";
                        return false;
                    }

                    vectors.Add(new Vector(
                        id,
                        @class,
                        expected,
                        fields.GetValueOrDefault("per-engine." + Engine),
                        fields.GetValueOrDefault("divergence." + Engine),
                        script));
                    fields.Clear();
                }

                continue;
            }

            var colon = line.IndexOf(": ", StringComparison.Ordinal);

            if (colon <= 0 || !fields.TryAdd(line[..colon], line[(colon + 2)..]))
            {
                failure = $"{name}:{index + 1}: `{line}` is not a key: value line of its record";
                return false;
            }
        }

        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// The schema's section 8. A fault is the runner's and nothing classifies it; an abort is the
    /// engine's, and a divergence field may.
    /// </summary>
    private static string Compare(Vector vector, string expected, string rendered)
    {
        if (Alternatives(expected).Contains(rendered, StringComparer.Ordinal))
        {
            return "match";
        }

        // A divergence field classifies the difference on a divergence vector, and on an
        // impl-defined one whose per-engine expectation this engine does not meet yet.
        return !rendered.StartsWith("fault: ", StringComparison.Ordinal) &&
            vector.Class is "divergence" or "impl-defined" && vector.Divergence is not null
            ? "classified-divergence"
            : "difference";
    }

    /// <summary>
    /// An outcome's alternatives, separated by <c> | </c>. A String may hold that separator, so the
    /// text is split only outside a <c>str:"…"</c> value.
    /// </summary>
    private static List<string> Alternatives(string outcome)
    {
        var alternatives = new List<string>();
        var start = 0;
        var quoted = false;

        for (var index = 0; index < outcome.Length; index++)
        {
            if (quoted)
            {
                if (outcome[index] == '\\')
                {
                    index++;
                }
                else if (outcome[index] == '"')
                {
                    quoted = false;
                }
            }
            else if (outcome[index] == '"')
            {
                quoted = true;
            }
            else if (string.CompareOrdinal(outcome, index, Separator, 0, Separator.Length) == 0)
            {
                alternatives.Add(outcome[start..index]);
                start = index + Separator.Length;
                index += Separator.Length - 1;
            }
        }

        alternatives.Add(outcome[start..]);
        return alternatives;
    }

    /// <summary>A different expected value of the same kind: the last bit of a double, or one more character of a string.</summary>
    private static string Mutate(string outcome)
    {
        const string number = "value: f64:0x";
        const string text = "value: str:\"";

        if (outcome.StartsWith(number, StringComparison.Ordinal) && outcome.Length == number.Length + 16)
        {
            var bits = ulong.Parse(outcome.AsSpan(number.Length), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            return "value: " + RenderNumber(BitConverter.UInt64BitsToDouble(bits ^ 1));
        }

        return outcome.StartsWith(text, StringComparison.Ordinal) && !outcome.Contains(Separator, StringComparison.Ordinal)
            ? outcome[..^1] + "0\""
            : throw new ArgumentException($"no mutation is defined for `{outcome}`");
    }

    /// <summary>Evaluates one vector in a runtime, an instance and a realm of its own.</summary>
    private static string Evaluate(ReadOnlyMemory<byte> artifactBytes, string prelude, string script, ulong fuel, ulong wallClock)
    {
        var surface = new VectorSurface(prelude, script);

        // EVERY SURFACE IS ADMITTED, Intl and Temporal included, so the internationalization data is
        // handed over as the test262 runs hand it (Test262Manifest.Descriptor): the numerics area's
        // rounding vectors run through Temporal (corpus revision 6, milestone SJL-4.2).
        var created = VmRuntime.Create(
            VmCatalog.CreateBuilder().Add(JavaScriptProfile.DescriptorComposing(new JsComposition
            {
                HostSurface = surface,
                IntlData = Broiler.VM.Profile.JavaScript.Intl.JsCldrData.Instance,
            })).Build(),
            Options(fuel, wallClock));

        if (!created.TryGetRuntime(out var runtime))
        {
            return Fault($"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));
            var verified = runtime.Verify(in descriptor, artifactBytes.Span, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return Fault($"verification refused the host program: {verified.Outcome}/{verified.Reason}");
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    return Fault($"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}");
                }

                using (instance)
                {
                    foreach (var entry in (string[])["main", JavaScriptProfile.TurnEntryPoint])
                    {
                        var request = new VmInvocationRequest(new VmUtf8Text(Encoding.UTF8.GetBytes(entry)));
                        var result = instance.Invoke(in request, CancellationToken.None);

                        if (result.Outcome != VmOutcome.Normal)
                        {
                            return surface.Rendered ?? OneLine($"abort: {entry} answered {result.Outcome}/{result.Reason}");
                        }
                    }

                    return surface.Rendered ?? Fault("the turn ran and rendered nothing");
                }
            }
        }
    }

    private static VmRuntimeCreationOptions Options(ulong fuel, ulong wallClock)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                VmBudgetDimension.WallClock => VmCeilingSpec.Value(dimension, wallClock),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities:
            [
                // THE HOST SURFACE IS WHAT RUNS THE VECTORS, and it is installed only where its
                // capability is registered. The handler carries no traffic.
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.HostSurfaceCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        return VmHostCallOutcome.Completed;
                    }),

                // A HOST SCRIPT IS COMPILED BY THE PROVIDER, as every program a realm asks for is.
                VmCapabilityRegistration.ArtifactProvider(
                    JavaScriptProfile.SourceProviderCapability,
                    new SourceProvider(JavaScriptProfile.WideManifest, JsFormat.FormatVersion, new JsCompileRequest())),
            ]);
    }

    /// <summary>The embedder: in its turn it evaluates the prelude, then the vector, and renders the outcome.</summary>
    private sealed class VectorSurface(string prelude, string script) : IJsHostSurface
    {
        internal string? Rendered { get; private set; }

        public void OnRealmCreated(JsHostRealm realm)
        {
        }

        public void OnTurn(JsHostRealm realm)
        {
            if (Rendered is not null)
            {
                return;
            }

            JsHostValue objectToString;

            try
            {
                realm.EvaluateScript(prelude, "prelude.js");
                objectToString = realm.GetProperty(
                    realm.GetProperty(realm.GetProperty(realm.Global, "Object"), "prototype"),
                    "toString");
            }
            catch (Exception failure) when (failure is JsHostThrowException or JsHostTerminatedException)
            {
                Rendered = Fault("the prelude failed: " + failure.GetType().Name);
                return;
            }

            try
            {
                Rendered = "value: " + Render(realm.EvaluateScript(script, "vector.js"));
            }
            catch (JsHostThrowException thrown)
            {
                Rendered = RenderThrown(realm, objectToString, thrown.Thrown);
            }
            catch (JsHostTerminatedException terminated)
            {
                Rendered = OneLine("abort: " + terminated.Message);
                throw;
            }
        }

        private static string RenderThrown(JsHostRealm realm, JsHostValue objectToString, JsHostValue thrown)
        {
            if (thrown.Kind is not (JsHostValueKind.Object or JsHostValueKind.Function or JsHostValueKind.Array))
            {
                return "throw: " + Render(thrown);
            }

            try
            {
                if (!string.Equals(realm.Invoke(objectToString, thrown).AsString(), "[object Error]", StringComparison.Ordinal))
                {
                    return "throw: object";
                }

                return realm.GetProperty(realm.GetProperty(thrown, "constructor"), "name").AsString() is { } name
                    ? "error: " + name
                    : Fault("a thrown error's constructor has no string name");
            }
            catch (JsHostThrowException)
            {
                return Fault("reading a thrown object's tag or constructor name threw");
            }
        }
    }

    /// <summary>A value in the corpus notation (SCHEMA.md section 3), from the host's view of it.</summary>
    private static string Render(JsHostValue value) => value.Kind switch
    {
        JsHostValueKind.Undefined => "undefined",
        JsHostValueKind.Null => "null",
        JsHostValueKind.Boolean => value.AsBoolean() ? "bool:true" : "bool:false",
        JsHostValueKind.Number => RenderNumber(value.AsNumber()),
        JsHostValueKind.String => RenderString(value.AsString()!),
        JsHostValueKind.BigInt => "bigint:" + ((BigInteger)value.AsBigInt()!).ToString(CultureInfo.InvariantCulture),
        JsHostValueKind.Symbol => "symbol",
        JsHostValueKind.Object or JsHostValueKind.Function or JsHostValueKind.Array => "object",
        _ => Fault($"a value of kind {value.Kind} has no rendering"),
    };

    private static string RenderNumber(double value) =>
        "f64:0x" + (double.IsNaN(value) ? 0x7FF8000000000000UL : BitConverter.DoubleToUInt64Bits(value))
            .ToString("X16", CultureInfo.InvariantCulture);

    private static string RenderString(string chars)
    {
        var builder = new StringBuilder(chars.Length + 6).Append("str:\"");

        foreach (var c in chars)
        {
            if (c is '"' or '\\')
            {
                builder.Append('\\').Append(c);
            }
            else if (c is >= ' ' and <= '~')
            {
                builder.Append(c);
            }
            else
            {
                builder.Append('\\').Append('u').Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
            }
        }

        return builder.Append('"').ToString();
    }

    /// <summary>A fault of the runner, on one line: it matches no outcome and is never classified.</summary>
    private static string Fault(string description) => OneLine("fault: " + description);

    private static string OneLine(string text) =>
        text.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');

    private static string? Argument(string[] args, string option)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], option, StringComparison.Ordinal))
            {
                return args[index + 1];
            }
        }

        return null;
    }

    private static ulong Number(string[] args, string option, ulong fallback)
    {
        for (var index = 0; index < args.Length - 1; index++)
        {
            if (string.Equals(args[index], option, StringComparison.Ordinal) &&
                ulong.TryParse(args[index + 1], NumberStyles.None, CultureInfo.InvariantCulture, out var stated) &&
                stated != 0)
            {
                return stated;
            }
        }

        return fallback;
    }
}
