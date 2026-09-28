using Broiler.VM;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The script reader's one adapter: the catalog its runtimes are built over, and how a module is verified.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS THE ONE FILE OF THE READER THAT DIFFERS BETWEEN THE TWO COMMITS BUNDLE UBC-4-005 COMPARES.</b>
/// Milestone UBC-4 changed how this root verifies a module. The retired path handed the module's bytes to
/// the core under the profile's own descriptor. This one translates the module to universal bytecode
/// first, and the core verifies the translation. Every other file of the reader is the same at both
/// commits, and the bundle retains both versions of this one.
/// </para>
/// <para>
/// <b>Both versions answer in the same fields</b>: the outcome, the reason, the diagnostic code, the byte
/// offset, and for an exhaustion the dimension and the scope. That makes an answer comparable whichever
/// path gave it.
/// </para>
/// </remarks>
internal static class ScriptVerification
{
    /// <summary>The identity every script module is verified under.</summary>
    internal const string Caller = "composition-wasm-harness://spec";

    /// <summary>The catalog a script's runtime is built over.</summary>
    internal static VmCatalog Catalog() =>
        VmCatalog.CreateBuilder().Add(ModuleVerification.Descriptor).Build();

    /// <summary>Verifies <paramref name="module"/>: translation first, then the core over the translation.</summary>
    internal static ScriptModule Verify(VmRuntime runtime, byte[] module, string label)
    {
        var verified = ModuleVerification.Verify(runtime, module, Caller, label);

        return verified.TryGetArtifact(out var artifact)
            ? ScriptModule.Admitted(artifact)
            : ScriptModule.Refused(
                verified.Outcome,
                verified.Reason,
                verified.Code,
                verified.Position.ByteOffset,
                verified.Dimension,
                verified.Scope);
    }
}
