using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The admission probe of milestone SJL-2.9: a scratch, JavaScript-family-shaped project, outside
/// the checkout, that references the probe package rule I1 admits test-only, restores it through
/// this repository's own NuGet.config into an isolated package folder, and builds.
/// </summary>
/// <remarks>
/// <para>
/// It exists so that the admission path is exercised end to end before any product project takes a
/// shared package: the source mapping sends the shared prefix to the committed feed, the lock file
/// records the content hash, rules I1, I2 and I6 read what restored, and each negative control is a
/// change to one of those that a rule must refuse. <b>No product project references the probe
/// package</b>; this project is written to a temporary directory and deleted.
/// </para>
/// <para>
/// The NuGet.config written beside it is the repository's, with the committed feed's relative path
/// made absolute and nothing else changed, so what the probe proves about source mapping is a
/// statement about the configuration the repository restores with.
/// </para>
/// </remarks>
internal sealed class AdmissionProbe : IDisposable
{
    internal const string AssemblyName = "Broiler.VM.Profile.JavaScript.AdmissionProbe";

    private readonly StringBuilder transcript = new();

    private AdmissionProbe(string directory, AdmittedPackage package)
    {
        Directory = directory;
        Package = package;
    }

    internal string Directory { get; }

    internal AdmittedPackage Package { get; }

    internal string ProjectPath => Path.Combine(Directory, "probe", AssemblyName + ".csproj");

    internal string AssetsPath => Path.Combine(Directory, "probe", "obj", "project.assets.json");

    internal string PackagesFolder => Path.Combine(Directory, "packages");

    internal string ConfigPath => Path.Combine(Directory, "NuGet.config");

    /// <summary>Everything the probe ran, with each command's output and exit code.</summary>
    internal string Transcript => transcript.ToString();

    /// <summary>Writes the scratch project for the admitted test-only package, its source and its configuration.</summary>
    internal static AdmissionProbe Create()
    {
        // The entry that names the probe. Until SJL-3.4 it was the probe pre-release, admitted test-only;
        // since, it is the package the profile itself references, so the probe checks what ships.
        var package = AdmissionRules.AllowList.Single(static p => p.Projects.Contains(AssemblyName, StringComparer.Ordinal));
        var directory = System.IO.Directory.CreateTempSubdirectory("broiler-vm-admission-probe-").FullName;
        var probe = new AdmissionProbe(directory, package);

        System.IO.Directory.CreateDirectory(Path.Combine(directory, "probe"));
        File.WriteAllText(probe.ProjectPath, $"""
            <Project Sdk="Microsoft.NET.Sdk">
              <!-- The admission probe (SJL-2.9): JavaScript-family-shaped, outside the checkout, test-only. -->
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <AssemblyName>{AssemblyName}</AssemblyName>
                <RootNamespace>{AssemblyName}</RootNamespace>
                <Nullable>enable</Nullable>
                <ImplicitUsings>disable</ImplicitUsings>
                <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
                <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="{package.Id}" Version="[{package.Version}]" />
              </ItemGroup>
            </Project>
            """);

        // A public surface that uses the contract types inside and names none of them: what rule I6
        // requires of every family assembly that takes a shared package.
        File.WriteAllText(Path.Combine(directory, "probe", "Charge.cs"), $$"""
            using Broiler.ECMAScript;

            namespace {{AssemblyName}};

            public static class Charge
            {
                public static ulong Run(ulong units)
                {
                    ulong charged = 0;
                    WorkCharge meter = value => charged += value;
                    meter(units);
                    return new LengthLimit(16).Admits((long)charged) ? charged : 0;
                }
            }
            """);

        var config = XDocument.Load(Path.Combine(ComponentGraph.Root, "NuGet.config"));
        foreach (var source in config.Descendants("add").Where(static e => e.Parent?.Name.LocalName == "packageSources"))
        {
            var value = (string?)source.Attribute("value");
            if (value is not null && !value.Contains("://", StringComparison.Ordinal))
            {
                source.SetAttributeValue("value", Path.GetFullPath(Path.Combine(ComponentGraph.Root, value)));
            }
        }

        config.Save(probe.ConfigPath);
        return probe;
    }

    /// <summary>The probe's project file, parsed as the rules parse every project file.</summary>
    internal ComponentGraph.ProjectFile Project => ComponentGraph.ParseOutside(ProjectPath);

    /// <summary>What ordinary restore resolved, or null before a restore.</summary>
    internal IReadOnlyList<RestoredPackage>? RestoreMetadata =>
        File.Exists(AssetsPath) ? AdmissionRules.ReadRestoreAssets(File.ReadAllText(AssetsPath)) : null;

    /// <summary>The admitted assembly as it restored into the isolated package folder.</summary>
    internal string RestoredAssembly(string assemblyName) => Path.Combine(
        PackagesFolder, Package.Id.ToLowerInvariant(), Package.Version.ToLowerInvariant(), "lib", "net10.0", assemblyName + ".dll");

    /// <summary>The probe's own built assembly.</summary>
    internal string BuiltAssembly => Path.Combine(Directory, "probe", "bin", "Release", "net10.0", AssemblyName + ".dll");

    internal int Restore() => Run(["restore", ProjectPath, "--configfile", ConfigPath, "--packages", PackagesFolder, "--no-http-cache"]);

    internal int Build() => Run(["build", ProjectPath, "-c", "Release", "--no-restore", "--nologo"]);

    /// <summary>Runs <c>dotnet</c> in the probe's directory, appending the command and its output to the transcript.</summary>
    internal int Run(IReadOnlyList<string> arguments)
    {
        var dotnet = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") is { Length: > 0 } host ? host : "dotnet";
        var start = new ProcessStartInfo(dotnet)
        {
            WorkingDirectory = Directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        start.Environment["NUGET_PACKAGES"] = PackagesFolder;
        start.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        start.Environment["DOTNET_NOLOGO"] = "1";

        using var process = Process.Start(start)!;
        var error = process.StandardError.ReadToEndAsync();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        Note("$ dotnet " + string.Join(' ', arguments).Replace(Directory, "<probe>", StringComparison.Ordinal));
        Note((output + error.Result).Replace(Directory, "<probe>", StringComparison.Ordinal).TrimEnd());
        Note("exit " + process.ExitCode.ToString(System.Globalization.CultureInfo.InvariantCulture));
        Note(string.Empty);
        return process.ExitCode;
    }

    /// <summary>Appends a line to the transcript, with the probe's temporary directory named &lt;probe&gt; however it is spelled.</summary>
    internal void Note(string line) => transcript
        .Append(line
            .Replace(Path.GetRelativePath(ComponentGraph.Root, Directory).Replace('\\', '/'), "<probe>", StringComparison.Ordinal)
            .Replace(Directory, "<probe>", StringComparison.Ordinal))
        .Append('\n');

    public void Dispose()
    {
        try
        {
            System.IO.Directory.Delete(Directory, recursive: true);
        }
        catch (IOException)
        {
            // A build server can hold a file a moment longer; the directory is temporary either way.
        }
    }
}
