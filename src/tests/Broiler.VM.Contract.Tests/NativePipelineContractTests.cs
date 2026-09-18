using System.Text;
using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

public sealed class NativePipelineContractTests
{
    private sealed class StubNativeCompiler : IVmNativeCompiler
    {
        public VmProfileId InputProfileId { get; init; } = VmProfileId.Parse("test.input");
        public VmProfileId OutputProfileId { get; init; } = VmProfileId.Parse("broiler.machinecode");
        public bool SupportsManifest { get; set; } = true;
        public bool ShouldSucceed { get; set; } = true;
        public string RefusalMessage { get; set; } = "compilation refused";
        public byte[] OutputBytes { get; set; } = [(byte)'B', (byte)'M', (byte)'C', 0];

        public bool CanCompile(VmFeatureManifestId manifestId) => SupportsManifest;

        public bool TryCompile(
            in VmArtifactDescriptor inputDescriptor,
            ReadOnlyMemory<byte> inputBytecode,
            string targetArchitecture,
            out VmArtifactDescriptor outputDescriptor,
            out byte[] outputMachineCode,
            out string refusal)
        {
            if (!ShouldSucceed)
            {
                outputDescriptor = default;
                outputMachineCode = [];
                refusal = RefusalMessage;
                return false;
            }

            outputDescriptor = new VmArtifactDescriptor(
                OutputProfileId,
                1,
                VmFeatureManifestId.Parse("broiler.machinecode.x86-64"),
                inputDescriptor.RequestedLimits,
                inputDescriptor.CallerIdentity);

            outputMachineCode = OutputBytes;
            refusal = string.Empty;
            return true;
        }
    }

    [Fact]
    public void CompileToMachineCode_Throws_When_Compiler_Is_Null()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var descriptor = new VmArtifactDescriptor(
            VmProfileId.Parse("test.input"), 1, VmFeatureManifestId.Parse("test.input.manifest"), default, VmCallerIdentity.None);

        Assert.Throws<ArgumentNullException>(() =>
            runtime.CompileToMachineCode(null!, in descriptor, new byte[16], "x86-64"));
    }

    [Fact]
    public void CompileToMachineCode_Fails_When_Compiler_Refuses_Manifest()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var compiler = new StubNativeCompiler { SupportsManifest = false };
        var descriptor = new VmArtifactDescriptor(
            VmProfileId.Parse("test.input"), 1, VmFeatureManifestId.Parse("test.unsupported.manifest"), default, VmCallerIdentity.None);

        var result = runtime.CompileToMachineCode(compiler, in descriptor, new byte[16], "x86-64");

        Assert.False(result.Succeeded);
        Assert.Null(result.MachineCodeArtifact);
        Assert.Contains("cannot compile manifest", result.Refusal, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompileToMachineCode_Fails_When_Lowering_Returns_False()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var compiler = new StubNativeCompiler
        {
            SupportsManifest = true,
            ShouldSucceed = false,
            RefusalMessage = "unsupported opcode in input bytecode",
        };

        var descriptor = new VmArtifactDescriptor(
            VmProfileId.Parse("test.input"), 1, VmFeatureManifestId.Parse("test.input.manifest"), default, VmCallerIdentity.None);

        var result = runtime.CompileToMachineCode(compiler, in descriptor, new byte[16], "x86-64");

        Assert.False(result.Succeeded);
        Assert.Null(result.MachineCodeArtifact);
        Assert.Equal("unsupported opcode in input bytecode", result.Refusal);
    }

    [Fact]
    public void CompileToMachineCode_Succeeds_And_Emits_MachineCode_Artifact()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var expectedBytes = Encoding.UTF8.GetBytes("BMC\0PAYLOAD");
        var compiler = new StubNativeCompiler
        {
            SupportsManifest = true,
            ShouldSucceed = true,
            OutputBytes = expectedBytes,
        };

        var caller = VmCallerIdentity.FromCanonicalIdentity("test://pipeline");
        var descriptor = new VmArtifactDescriptor(
            VmProfileId.Parse("test.input"), 1, VmFeatureManifestId.Parse("test.input.manifest"), default, caller);

        var result = runtime.CompileToMachineCode(compiler, in descriptor, new byte[16], "x86-64");

        Assert.True(result.Succeeded);
        Assert.Null(result.Refusal);
        Assert.NotNull(result.MachineCodeArtifact);
        Assert.Equal(expectedBytes, result.MachineCodeArtifact);
        Assert.Equal("broiler.machinecode", result.Descriptor.ProfileId.ToString());
        Assert.Equal("broiler.machinecode.x86-64", result.Descriptor.FeatureManifestId.ToString());
        Assert.Equal(caller, result.Descriptor.CallerIdentity);
    }
}
