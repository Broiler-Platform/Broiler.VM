// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Xunit;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N16: two format versions are read by ONE verifier, and each is paired with its own manifest.
/// </summary>
/// <remarks>
/// <para>
/// <b>The gates make a second verifier a stop condition, in those words: "two verifiers that must
/// agree are a security defect with a schedule."</b> Adding a format version is exactly the change
/// that invites one - the second version's structure has nothing in common with the first's beyond
/// the framing, and a second class implementing the contract is the obvious way to write it. This
/// rule holds the profile to one.
/// </para>
/// <para>
/// <b>The second half is the pairing.</b> A format version and a feature manifest are independent
/// identities, and a payload that named one version's manifest under the other's version would be
/// read by a pass whose refusals are written against a surface it does not have. Both directions
/// are retained as corpus entries, and this rule asserts the binding between the two passes that
/// makes those entries mean what they say.
/// </para>
/// <para>
/// <b>It reads source and the retained corpus manifest, and loads nothing.</b> Rule A11 forbids a
/// project reference from here to the profile, and the profile's assemblies are otherwise read
/// through a metadata-only context that cannot run a static constructor - so a descriptor's values
/// are not reachable at run time from this project at all. What is reachable is the one file that
/// constructs it, which is where the decision this rule is about is written down.
/// </para>
/// </remarks>
public sealed class N16WideSurfaceRuleTests
{
    private const string DescriptorPath =
        "src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs";

    private const string FirstPassPath =
        "src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs";

    private const string SecondPassPath =
        "src/Broiler.VM.Profile.JavaScript/JsVerifier.cs";

    /// <summary>The descriptor declares every manifest and a range that spans both versions.</summary>
    /// <remarks>
    /// <b>TWO FORMAT VERSIONS AND THREE MANIFESTS, WHICH IS NOT A CONTRADICTION AND IS WORTH
    /// SAYING WHY.</b> The rule this test holds is that a format version is bound to the feature
    /// manifest it is defined against, so that a payload announcing one version's manifest under
    /// the other's version is refused rather than read by a pass written for a surface it does not
    /// have. Version 2 is defined against <c>broiler.javascript.wide</c> and against
    /// <c>broiler.javascript.numeric</c>, which is a NARROWING of it: the same sections, the same
    /// instruction encoding, the same verifier, and a smaller language behind it. A narrowing needs
    /// no format version of its own, and minting one would have minted the second verifier this
    /// rule exists to keep out.
    /// </remarks>
    [Fact]
    public void N16_Two_Versions_And_Three_Manifests_Are_Declared_Together()
    {
        var text = AssuranceSources.File(DescriptorPath).Text;

        Assert.Contains(
            "VmFeatureManifestId.Parse(\"broiler.javascript.slice\")",
            text,
            System.StringComparison.Ordinal);

        Assert.Contains(
            "VmFeatureManifestId.Parse(\"broiler.javascript.wide\")",
            text,
            System.StringComparison.Ordinal);

        Assert.Contains(
            "VmFeatureManifestId.Parse(Format.JsNumericManifest.ManifestId)",
            text,
            System.StringComparison.Ordinal);

        Assert.Contains(
            "ImmutableArray.Create(SliceManifest, WideManifest, NumericManifest)",
            text,
            System.StringComparison.Ordinal);

        // AND THE OPTIONAL SURFACES ARE ADDED TO THAT SET RATHER THAN BAKED INTO IT. An artifact
        // still names one of the three manifests in its header; a surface is something it declares
        // beside that, and a composition admitting none is a composition whose accepted set is
        // exactly the three above. A literal that listed the surfaces here would make every
        // composition admit them - including the native surface, which is the one whose whole
        // purpose is that a composition can refuse it.
        Assert.Contains(
            "accepted = accepted.Add(VmFeatureManifestId.Parse(surface))",
            text,
            System.StringComparison.Ordinal);

        // The range's upper bound is the version-2 constant rather than a literal, so a third
        // version cannot arrive without this line moving with it.
        Assert.Contains(
            "JavaScriptFormat.MinimumFormatVersion, Format.JsFormat.FormatVersion",
            text,
            System.StringComparison.Ordinal);
    }

    /// <summary>
    /// Exactly one type of the profile assembly declares the verifier contract.
    /// </summary>
    [Fact]
    public void N16_The_Profile_Declares_Exactly_One_Verifier()
    {
        var declaring = AssuranceSources.Files
            .Where(static file => file.Assembly == ArchitectureRules.JavaScriptProfileAssembly)
            .SelectMany(static file => file.Tree.GetRoot()
                .DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .Where(static type => type.BaseList is not null &&
                    type.BaseList.Types.Any(static baseType =>
                        baseType.Type.ToString() == "IVmProfileVerifier"))
                .Select(type => file.RelativePath + ": " + type.Identifier.Text))
            .OrderBy(static found => found, System.StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[] { FirstPassPath + ": JavaScriptVerifier" }, declaring);

        // Non-vacuous: the second pass exists and is NOT a second implementation of the contract -
        // it is a class the first one calls. A rule that only counted implementations would pass
        // just as well over an assembly with no second pass at all.
        Assert.Contains(
            AssuranceSources.Files,
            file => file.RelativePath == SecondPassPath);
    }

    /// <summary>
    /// The version-2 pass is reached from the one verifier, and from the descriptor rather than
    /// from the payload.
    /// </summary>
    /// <remarks>
    /// What this asserts is an ORDERING that a passing verification cannot show: the choice of pass
    /// is made from the descriptor the caller supplied, before the payload is opened. A verifier
    /// that sniffed the payload to decide how to read the payload would be deciding from the thing
    /// it has not yet checked.
    /// </remarks>
    [Fact]
    public void N16_The_Second_Pass_Is_Reached_From_The_First_And_Not_From_The_Payload()
    {
        var verify = AssuranceSources.File(FirstPassPath).Tree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(static method =>
                method.Identifier.Text == "Verify" && method.Body is not null);

        var body = verify.Body!.Statements;

        var dispatch = body
            .OfType<IfStatementSyntax>()
            .FirstOrDefault(static statement =>
                statement.Condition.ToString()
                    .Contains("descriptor.FormatVersion", System.StringComparison.Ordinal) &&
                statement.ToString()
                    .Contains("JsVerifier.Verify", System.StringComparison.Ordinal));

        Assert.True(
            dispatch is not null,
            "the one verifier does not dispatch to the version-2 pass from the descriptor");

        var before = body.TakeWhile(statement => statement != dispatch).ToArray();

        Assert.DoesNotContain(
            before,
            statement => statement.ToString()
                .Contains("payload", System.StringComparison.Ordinal));

        Assert.DoesNotContain(
            before,
            statement => statement.ToString()
                .Contains("VmBoundedReader", System.StringComparison.Ordinal));
    }

    /// <summary>Each pass accepts one manifest, and the two are different.</summary>
    /// <remarks>
    /// If the two ever named the same manifest, a payload of either version would satisfy either
    /// pass and the pairing would be a convention rather than a check. The retained corpus holds
    /// one entry per direction of the mismatch, and this is the binding that makes those two
    /// entries about the pairing rather than about the bytes.
    /// </remarks>
    [Fact]
    public void N16_The_Two_Passes_Accept_Two_Different_Manifests()
    {
        var second = AssuranceSources.File(SecondPassPath).Text;

        Assert.Contains("JsFormat.ManifestId", second, System.StringComparison.Ordinal);
        Assert.DoesNotContain("SliceManifest", second, System.StringComparison.Ordinal);

        // The construction still names the manifest the FIRST pass accepts, and it now also names
        // the optional surfaces the composition admitted - which the second pass refuses an
        // artifact for declaring outside. Both are arguments of the one verifier object, because a
        // second object would be a second answer to the same question.
        //
        // A THIRD ARGUMENT JOINED THEM AND IT IS AN EMITTER, AND THE PREFIX IS MATCHED RATHER THAN
        // THE WHOLE CALL SO THAT THIS ROW STAYS ABOUT WHAT IT IS ABOUT. What this clause pins is
        // that the two manifests and the admitted surfaces reach ONE verifier object; whether that
        // object also holds the emitter a composition may re-emit a native payload with is a
        // different question, asked where re-emission is.
        Assert.Contains(
            "new JavaScriptVerifier(Id, SliceManifest, admittedSurfaces",
            AssuranceSources.File(DescriptorPath).Text,
            System.StringComparison.Ordinal);

        // The retained corpus carries both directions of the mismatch, each recording the code the
        // registry publishes for it.
        var manifest = File.ReadAllLines(
            Path.Combine(ComponentGraph.Root, "src", "tests", "corpus", "js-1", "corpus.manifest"));

        Assert.Contains(
            manifest,
            line => line.StartsWith("wide-a-version-1-artifact-announced-as-version-2|", System.StringComparison.Ordinal) &&
                line.Contains("|1003|", System.StringComparison.Ordinal));

        Assert.Contains(
            manifest,
            line => line.StartsWith("wide-a-slice-manifest-announced-as-the-wide-one|", System.StringComparison.Ordinal) &&
                line.Contains("|1006|", System.StringComparison.Ordinal));
    }
}
