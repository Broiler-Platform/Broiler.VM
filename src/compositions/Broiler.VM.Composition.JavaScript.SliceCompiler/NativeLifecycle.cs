using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Drives one numeric program all the way through the core lifecycle, in whichever output form it
/// is asked for.
/// </summary>
/// <remarks>
/// <para>
/// <b>ONE FRONT END, TWO EXITS, AND THE SAME LIFECYCLE FOR BOTH.</b> The same source text is
/// lowered by the same tokenizer, parser and lowering; the only difference between the two calls is
/// which output form the compilation was asked for. Both artifacts are then verified by the same
/// verifier, instantiated by the same runtime and invoked through the same entry point, so a
/// difference in what they answer is a difference in the FORM and cannot be a difference in the
/// path.
/// </para>
/// <para>
/// <b>THE NATIVE FORM IS VERIFIED WITH A RE-EMITTING DESCRIPTOR AND THE BYTECODE FORM IS NOT,
/// because only one of them has anything to re-emit.</b> A composition that carries a lowering can
/// recompile the bytecode an artifact carries and require the emitted bytes to match, which is the
/// only sense in which machine code is verifiable; an execution-only composition cannot, and what
/// it trusts instead is provenance. This root carries a compiler, so it takes the stronger door.
/// </para>
/// </remarks>
internal static class NativeLifecycle
{
    /// <summary>The identity this composition verifies under.</summary>
    private const string Caller = "com.example.broiler.slice-compiler";

    /// <summary>Compiles, verifies, instantiates and invokes, and says what came back.</summary>
    internal static string Run(string text, JsOutputForm form, string backend)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("script0", text, SliceParseOptions.Script)],
            [],
            new JsCompileRequest(JsFeatureManifest.Numeric, form, backend));

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return "refused by the front end: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString());
        }

        var descriptor = form == JsOutputForm.Native && JsNativeBackends.TryFind(backend, out var found)
            ? JavaScriptProfile.DescriptorReEmittingWith(
                (IJsNativeEmitter)found, JavaScriptProfile.NativeManifest)
            : JavaScriptProfile.Descriptor;

        var catalog = VmCatalog.CreateBuilder().Add(descriptor).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(catalog, new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: []));

        if (!created.TryGetRuntime(out var runtime))
        {
            return "the runtime refused creation: " + created.Outcome + "/" + created.Reason;
        }

        using (runtime)
        {
            var artifactDescriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.NumericManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(
                in artifactDescriptor, compiled.Artifact, CancellationToken.None);

            if (!verified.TryGetArtifact(out var handle))
            {
                return "the verifier refused: " + verified.Outcome + "/" + verified.Reason +
                    " code " + verified.Diagnostics.ProfileDiagnosticCode;
            }

            var instantiated = runtime.Instantiate(handle, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return "instantiation refused: " + instantiated.Outcome + "/" + instantiated.Reason;
            }

            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("script0")));

            var result = instance.Invoke(in request, CancellationToken.None);

            if (JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
            {
                return "uncaught " + uncaught.Message;
            }

            return JavaScriptProfile.TryGetWideCompletion(in result, out var completion)
                ? completion.Value + " (" + completion.TypeOf + ")"
                : result.Outcome + "/" + result.Reason;
        }
    }

    /// <summary>What one wide program answered, in every respect a host can observe.</summary>
    /// <param name="Completion">The completion value of the entry invocation, rendered, or empty.</param>
    /// <param name="TypeOf">What <c>typeof</c> answered for it, or empty.</param>
    /// <param name="ErrorName">The constructor name of an uncaught throw, or empty.</param>
    /// <param name="ErrorMessage">The rendered uncaught throw, or empty.</param>
    /// <param name="Printed">Every line <c>print</c> wrote, in order, joined by line feeds.</param>
    /// <param name="Outcome">
    /// Where the run ended: <c>completed</c>, <c>uncaught</c>, <c>exhausted:</c> and a dimension, or the
    /// outcome and reason of whichever stage refused.
    /// </param>
    /// <param name="Fuel">The runtime's fuel consumption when the run ended.</param>
    /// <param name="Collections">How many collections the print callback forced.</param>
    internal sealed record WideAnswer(
        string Completion,
        string TypeOf,
        string ErrorName,
        string ErrorMessage,
        string Printed,
        string Outcome,
        ulong Fuel,
        int Collections)
    {
        /// <summary>Whether the program ran to its end without spending its allowance or being refused.</summary>
        internal bool Completed =>
            string.Equals(Outcome, "completed", StringComparison.Ordinal) ||
            string.Equals(Outcome, "uncaught", StringComparison.Ordinal);

        /// <summary>
        /// The answer as one line, with or without its fuel figure.
        /// </summary>
        /// <remarks>
        /// <b>THE PRINTED LINES ARE HASHED RATHER THAN QUOTED</b>, because a program that prints
        /// thousands of lines would otherwise make a failing row unreadable; their count stays in the
        /// clear so a reader can see which side printed more.
        /// </remarks>
        internal string Render(bool withFuel)
        {
            var lines = Printed.Length == 0 ? 0 : Printed.Split('\n').Length;
            var hash = Convert.ToHexStringLower(
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(Printed)))[..12];

            return Outcome + " value=" + Completion + " (" + TypeOf + ")" +
                (ErrorName.Length == 0 && ErrorMessage.Length == 0 ? string.Empty : " threw " + ErrorName + ": " + ErrorMessage) +
                " printed=" + lines + "#" + hash +
                (withFuel ? " fuel=" + Fuel : string.Empty);
        }
    }

    /// <summary>
    /// Compiles one source under the wide manifest in the form asked for, and runs it through the whole
    /// core lifecycle: verify, instantiate, invoke the entry, drain the job queue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE NATIVE FORM IS VERIFIED BY RE-EMISSION AND THE BYTECODE FORM BY THE ADMITTING DOOR</b>,
    /// for the reason <see cref="Run"/> gives: this root carries the backend, so the stronger
    /// verification is the one it takes. Every surface this build implements is admitted, because a
    /// program that reaches <c>eval</c> or a module graph declares the surface and this row is about
    /// the form rather than about which surfaces a composition declined.
    /// </para>
    /// <para>
    /// <b>THE HOST IS THE ONE A CONFORMANCE RUN BUILDS</b>: a <c>print</c> that records, a source
    /// provider that compiles with the run's own request - so an <c>eval</c> in a native instance is
    /// answered in the native form, which the engine requires - and a resolver that confirms every
    /// module request, because the graphs compiled here were bundled by key.
    /// </para>
    /// </remarks>
    internal static WideAnswer RunWide(
        string text,
        JsOutputForm form,
        string backend,
        ulong fuel,
        string? library = null,
        Action? everyThousandPrints = null)
    {
        var compiled = CompileWide(text, form, backend, library);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Refusal(
                "refused by the front end: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return RunWideArtifact(
            compiled.Artifact,
            form,
            backend,
            fuel,
            reEmit: form == JsOutputForm.Native,
            module: library is not null,
            everyThousandPrints);
    }

    /// <summary>Compiles one wide source, or a two-module graph whose main module imports <c>lib</c>.</summary>
    internal static JsCompilation CompileWide(
        string text, JsOutputForm form, string backend, string? library = null)
    {
        var request = new JsCompileRequest(
            JsFeatureManifest.Wide, form, form == JsOutputForm.Native ? backend : string.Empty);

        return library is null
            ? JsCompiler.Compile([new JsScriptUnit("script0", text, SliceParseOptions.Script)], [], request)
            : JsCompiler.Compile(
                [],
                [
                    new JsModuleUnit("lib", library, SliceParseOptions.Module),
                    new JsModuleUnit("main", text, SliceParseOptions.Module, [new JsResolvedRequest("lib", "lib")]),
                ],
                request);
    }

    /// <summary>Runs an already-compiled wide artifact through the lifecycle <see cref="RunWide"/> describes.</summary>
    /// <remarks>
    /// <b>SEPARATE SO THAT A ROW CAN HAND IT BYTES NO COMPILATION WROTE</b> - a payload whose handler
    /// call was moved to another slot, handed to the admitting door so that what refuses it is the
    /// template scan every image runs and not a re-emission only some images can make.
    /// </remarks>
    internal static WideAnswer RunWideArtifact(
        byte[] artifact,
        JsOutputForm form,
        string backend,
        ulong fuel,
        bool reEmit,
        bool module,
        Action? everyThousandPrints = null)
    {
        JsX64Abi? abi = null;

        foreach (var row in JsX64Abi.Rows)
        {
            if (string.Equals(row.Name, backend, StringComparison.Ordinal))
            {
                abi = row;
            }
        }

        if (form == JsOutputForm.Native && reEmit && abi is null)
        {
            return Refusal("no x86-64 backend is named " + backend);
        }

        var descriptor = form == JsOutputForm.Native && reEmit
            ? JavaScriptProfile.DescriptorReEmittingWith(new JsX64Backend(abi!), EverySurface())
            : JavaScriptProfile.DescriptorAdmitting(EverySurface());

        var request = new JsCompileRequest(
            JsFeatureManifest.Wide, form, form == JsOutputForm.Native ? backend : string.Empty);

        var printed = new List<string>();
        var collections = 0;
        var catalog = VmCatalog.CreateBuilder().Add(descriptor).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                VmBudgetDimension.WallClock => VmCeilingSpec.Value(dimension, WideWallClockMilliseconds),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        var created = VmRuntime.Create(catalog, new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities:
            [
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.WriteCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        printed.Add(System.Text.Encoding.UTF8.GetString(argument.Span));

                        // THE COLLECTION IS FORCED FROM INSIDE THE GUEST'S OWN CALL, which is the
                        // point: a print is a host call made by a handler, so every emitted frame of
                        // every activation between the entry and this print is on the stack while
                        // the collector walks it and moves what it likes.
                        if (everyThousandPrints is not null && printed.Count % 1000 == 0)
                        {
                            everyThousandPrints();
                            collections++;
                        }

                        return VmHostCallOutcome.Completed;
                    }),
                VmCapabilityRegistration.ArtifactProvider(
                    JavaScriptProfile.SourceProviderCapability,
                    new WideSourceProvider(request)),
                VmCapabilityRegistration.Value(
                    JavaScriptProfile.ResolveCapability,
                    (VmBytes argument, out VmOpaqueRef result) =>
                    {
                        result = default;
                        return VmHostCallOutcome.Completed;
                    }),
            ]));

        if (!created.TryGetRuntime(out var runtime))
        {
            return Refusal("the runtime refused creation: " + created.Outcome + "/" + created.Reason);
        }

        using (runtime)
        {
            WideAnswer Ended(string outcome, string value = "", string typeOf = "", string name = "", string message = "") =>
                new(
                    value,
                    typeOf,
                    name,
                    message,
                    string.Join('\n', printed),
                    outcome,
                    runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel),
                    collections);

            var artifactDescriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in artifactDescriptor, artifact, CancellationToken.None);

            if (!verified.TryGetArtifact(out var handle))
            {
                return Ended(
                    "the verifier refused: " + verified.Outcome + "/" + verified.Reason + " code " +
                    verified.Diagnostics.ProfileDiagnosticCode);
            }

            var instantiated = runtime.Instantiate(handle, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return Ended(Stage("instantiation", instantiated.Outcome, instantiated.Reason, instantiated.Diagnostics));
            }

            var entry = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(module ? JsCompiler.ModuleEntry : "script0")));

            var result = instance.Invoke(in entry, CancellationToken.None);

            if (JavaScriptProfile.TryGetUncaught(in result, out var uncaught))
            {
                return Ended("uncaught", name: uncaught.ErrorName, message: uncaught.Message);
            }

            if (!JavaScriptProfile.TryGetWideCompletion(in result, out var completion))
            {
                return Ended(Stage("invocation", result.Outcome, result.Reason, result.Diagnostics));
            }

            var drain = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(JavaScriptProfile.DrainEntryPoint)));

            var drained = instance.Invoke(in drain, CancellationToken.None);

            if (JavaScriptProfile.TryGetUncaught(in drained, out var jobThrew))
            {
                return Ended(
                    "uncaught", completion.Value, completion.TypeOf, "job " + jobThrew.ErrorName, jobThrew.Message);
            }

            return drained.Outcome == VmOutcome.Normal
                ? Ended("completed", completion.Value, completion.TypeOf)
                : Ended(
                    Stage("draining", drained.Outcome, drained.Reason, drained.Diagnostics),
                    completion.Value,
                    completion.TypeOf);
        }
    }

    /// <summary>
    /// The wall-clock ceiling a wide run is taken under: the deterministic lane's, so a slower form is
    /// never the reason two answers differ.
    /// </summary>
    private const ulong WideWallClockMilliseconds = 60_000;

    /// <summary>An answer for a run that never reached a runtime.</summary>
    private static WideAnswer Refusal(string why) => new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, why, 0, 0);

    /// <summary>Where a stage that did not complete ended, as the answer's outcome.</summary>
    private static string Stage(string stage, VmOutcome outcome, VmReason reason, VmDiagnostics diagnostics) =>
        outcome == VmOutcome.ResourceExhaustion
            ? "exhausted:" + diagnostics.ExhaustedDimension + " " + stage
            : stage + " " + outcome + "/" + reason;

    /// <summary>Every surface this build implements, as the descriptor doors take them.</summary>
    private static VmFeatureManifestId[] EverySurface()
    {
        var surfaces = new VmFeatureManifestId[JsSurfaces.All.Length];

        for (var index = 0; index < surfaces.Length; index++)
        {
            surfaces[index] = VmFeatureManifestId.Parse(JsSurfaces.All[index]);
        }

        return surfaces;
    }

    /// <summary>The source provider a wide run registers: it compiles an <c>eval</c> with the run's own request.</summary>
    /// <remarks>
    /// <b>THE FORM IS THE RUN'S AND NOT THE PROVIDER'S.</b> An instance has one form and the engine
    /// refuses a guest-loaded program of the other as a defect, so a provider that compiled every
    /// <c>eval</c> to bytecode would turn the native half of every row that reaches <c>eval</c> into an
    /// internal defect the bytecode half could never show. Dynamic imports are not answered: no row
    /// here reaches one.
    /// </remarks>
    private sealed class WideSourceProvider(JsCompileRequest request) : IVmArtifactProvider
    {
        /// <summary>The identity this provider is registered under.</summary>
        public VmCapabilityId CapabilityId => JavaScriptProfile.SourceProviderCapability.CapabilityId;

        /// <summary>Its exact version.</summary>
        public int Version => JavaScriptProfile.SourceProviderCapability.Version;

        /// <summary>Answers one request by compiling its payload as a script.</summary>
        public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest artifactRequest)
        {
            if (artifactRequest.RequestingProfileId != JavaScriptProfile.Id ||
                JsFormat.TryReadModuleRequest(artifactRequest.RequestPayload.Span, out _, out _))
            {
                return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
            }

            string source;

            try
            {
                source = System.Text.Encoding.UTF8.GetString(artifactRequest.RequestPayload.Span);
            }
            catch (ArgumentException)
            {
                return VmArtifactProviderAnswer.Refused(VmReason.MalformedEncoding);
            }

            var compiled = JsCompiler.Compile(
                [new JsScriptUnit("main", source, SliceParseOptions.Script)], [], request);

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
            }

            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            return VmArtifactProviderAnswer.Provided(in descriptor, compiled.Artifact);
        }
    }

    /// <summary>
    /// Reads the program a wide artifact's baseline form was emitted from back out of the artifact: its
    /// code, its function rows and its exception regions, as a baseline image.
    /// </summary>
    /// <remarks>
    /// <b>IT READS THE ARTIFACT FOR THE REASON <see cref="TryReadEmitted"/> DOES</b>: a row about the program
    /// a payload was emitted from should hold what an artifact carries, not a projection the compiler never
    /// wrote. The constant pool is not read, because the baseline form reads no constant, so the image's
    /// constant arrays are empty; its operand-stack maximum is the deepest a function row declares.
    /// </remarks>
    internal static bool TryReadImage(byte[] artifact, out JsNativeProgramImage image, out string refusal)
    {
        image = null!;

        var at = 4;
        ReadVarUInt(artifact, ref at);
        var manifest = ReadVarUInt(artifact, ref at);
        at += (int)manifest;
        var sections = ReadVarUInt(artifact, ref at);
        byte[]? code = null;
        JsFunctionRow[]? rows = null;
        JsExceptionRegionRow[] regions = [];
        var stack = 0u;

        for (var index = 0u; index < sections; index++)
        {
            var kind = ReadVarUInt(artifact, ref at);
            var length = (int)ReadVarUInt(artifact, ref at);
            var cursor = at;
            at += length;

            if (kind == (uint)JsFormat.SectionKind.Code)
            {
                code = artifact[cursor..at];
            }
            else if (kind == (uint)JsFormat.SectionKind.Functions)
            {
                rows = new JsFunctionRow[ReadVarUInt(artifact, ref cursor)];

                for (var row = 0; row < rows.Length; row++)
                {
                    rows[row] = new JsFunctionRow(
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor));

                    stack = System.Math.Max(stack, rows[row].MaxOperandStack);
                }
            }
            else if (kind == (uint)JsFormat.SectionKind.ExceptionRegions)
            {
                regions = new JsExceptionRegionRow[ReadVarUInt(artifact, ref cursor)];

                for (var row = 0; row < regions.Length; row++)
                {
                    regions[row] = new JsExceptionRegionRow(
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        ReadVarUInt(artifact, ref cursor),
                        (JsFormat.HandlerKind)artifact[cursor++]);
                }
            }
        }

        if (code is null || rows is null || rows.Length == 0)
        {
            refusal = "the artifact carries no code section or no function table";
            return false;
        }

        image = new JsNativeProgramImage(code, rows, [], [], stack)
        {
            Tier = JsNativeTier.Baseline,
            Regions = regions,
        };

        refusal = string.Empty;
        return true;
    }

    /// <summary>
    /// Compiles one numeric source to machine code and hands back the emitted bytes and their
    /// symbol table, read out of the artifact the compilation produced.
    /// </summary>
    /// <remarks>
    /// <b>THE BYTES ARE READ OUT OF THE ARTIFACT RATHER THAN TAKEN FROM THE BACKEND, and that is
    /// what makes the row honest.</b> What a check should call into is what an artifact actually
    /// carries: bytes taken straight from the emitter would be a check of the emitter against
    /// itself with the whole write-and-read path removed - which is exactly the path a truncation
    /// or an off-by-one in the framing would live in.
    /// </remarks>
    internal static bool TryReadEmitted(
        byte[] artifact, out byte[] code, out JsNativeSymbolRow[] symbols, out string refusal)
    {
        code = [];
        symbols = [];
        refusal = string.Empty;

        var at = 4;
        ReadVarUInt(artifact, ref at);
        var manifest = ReadVarUInt(artifact, ref at);
        at += (int)manifest;
        var sections = ReadVarUInt(artifact, ref at);

        for (var index = 0u; index < sections; index++)
        {
            var kind = ReadVarUInt(artifact, ref at);
            var length = (int)ReadVarUInt(artifact, ref at);
            var body = at;
            at = body + length;

            if (kind == (uint)JsFormat.SectionKind.NativeCode)
            {
                code = artifact[(body + 16)..(body + length)];
                continue;
            }

            if (kind != (uint)JsFormat.SectionKind.NativeSymbols)
            {
                continue;
            }

            var count = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(
                artifact.AsSpan(body));

            symbols = new JsNativeSymbolRow[count];

            for (var row = 0u; row < count; row++)
            {
                var cursor = body + 4 + ((int)row * 8);

                symbols[row] = new JsNativeSymbolRow(
                    System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(
                        artifact.AsSpan(cursor)),
                    System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(
                        artifact.AsSpan(cursor + 4)));
            }
        }

        if (code.Length == 0 || symbols.Length == 0)
        {
            refusal = "the artifact carries no emitted code section or no symbol table";
            return false;
        }

        return true;
    }

    /// <summary>Reads one variable-length unsigned integer.</summary>
    private static uint ReadVarUInt(byte[] bytes, ref int at)
    {
        var value = 0u;
        var shift = 0;

        while (true)
        {
            var b = bytes[at++];
            value |= (uint)(b & 0x7F) << shift;

            if ((b & 0x80) == 0)
            {
                return value;
            }

            shift += 7;
        }
    }
}
