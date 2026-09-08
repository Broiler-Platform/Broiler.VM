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
