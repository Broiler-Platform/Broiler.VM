// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// What the realm admits, asked of the realm rather than of a document.
/// </summary>
/// <remarks>
/// <para>
/// <b>The point is that a document cannot be the authority on this.</b> Bundle JS-4-001 published a
/// list of absent globals which was true and not exhaustive — the keyed collections and
/// <c>Promise</c> were absent too and were not on it — and a later reader mistaking such a list for
/// the whole set is how a gap survives a review. So the set is published by asking a program, and a
/// rule compares what the program answered against what the documents claim.
/// </para>
/// <para>
/// <b>It is asked in JavaScript, through the ordinary path.</b> A C# accessor reaching into the
/// realm would answer about a field; this runs
/// <c>Object.getOwnPropertyNames(globalThis)</c> in a verified artifact on a real instance, so what
/// it reports is what a guest can see and nothing else. A name the realm defines and a guest cannot
/// reach would be a name this does not list, which is the right answer.
/// </para>
/// <para>
/// <b>Every optional surface is admitted for this.</b> The set is the widest the profile has, and a
/// composition that declines one gets fewer names; publishing the narrow set would make the file a
/// statement about one composition rather than about the profile.
/// </para>
/// </remarks>
internal static class RealmGlobals
{
    /// <summary>The identity this reader presents to the verifier.</summary>
    private const string Caller = "js-slice-compiler://globals";

    /// <summary>The program that asks. It is the whole of what this mode runs.</summary>
    private const string Source =
        "Object.getOwnPropertyNames(globalThis).sort().join('\\n');";

    /// <summary>Reads the realm's admitted global names, or answers why it could not.</summary>
    internal static bool TryRead(out ImmutableArray<string> names, out string failure)
    {
        names = [];
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", Source, SliceParseOptions.Script)]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            failure = "the reader program did not compile";
            return false;
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorAdmitting(
                JavaScriptProfile.BinaryManifest, JavaScriptProfile.DynamicManifest))
            .Build();

        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            catalog,
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: ImmutableArray<VmCapabilityRegistration>.Empty));

        if (!created.TryGetRuntime(out var runtime))
        {
            failure = $"the runtime refused creation: {created.Outcome}/{created.Reason}";
            return false;
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(
                in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (!verified.TryGetArtifact(out var handle))
            {
                failure = $"verification: {verified.Outcome}/{verified.Reason}";
                return false;
            }

            var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                failure = $"instantiation: {instantiated.Outcome}/{instantiated.Reason}";
                return false;
            }

            using (instance)
            {
                var request = new VmInvocationRequest(
                    new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

                var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);

                if (!JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
                {
                    failure = $"invocation: {invoked.Outcome}/{invoked.Reason}";
                    return false;
                }

                names = [.. completion.Value.Split('\n', System.StringSplitOptions.RemoveEmptyEntries)];
                failure = string.Empty;
                return true;
            }
        }
    }

    /// <summary>Prints the set, or writes it to the file a rule reads.</summary>
    internal static int Run(string? writeTo)
    {
        if (!TryRead(out var names, out var failure))
        {
            System.Console.Error.WriteLine("broiler-js-slice-compiler: " + failure);
            return 3;
        }

        var text = new System.Text.StringBuilder();
        text.Append("# broiler.javascript realm globals 1\n");
        text.Append("#\n");
        text.Append("# GENERATED. Regenerate with:\n");
        text.Append("#   dotnet run --project src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler \\\n");
        text.Append("#     -- --globals --write src/Broiler.VM.Profile.JavaScript/docs/realm/globals.txt\n");
        text.Append("#\n");
        text.Append("# It is the answer `Object.getOwnPropertyNames(globalThis)` gave on a realm built by a\n");
        text.Append("# composition admitting every optional surface, sorted. A document's list of what is\n");
        text.Append("# absent is checked against this file rather than against another document.\n");

        foreach (var name in names)
        {
            text.Append(name).Append('\n');
        }

        if (writeTo is null)
        {
            System.Console.Write(text.ToString());
            return 0;
        }

        var path = System.IO.Path.GetFullPath(writeTo);
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);
        System.IO.File.WriteAllText(path, text.ToString());
        System.Console.WriteLine($"broiler-js-slice-compiler: wrote {names.Length} global names to {writeTo}");
        return 0;
    }
}
