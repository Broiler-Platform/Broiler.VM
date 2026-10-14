// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Part C's harness of bundle sjl-5-001's rule (decision-rule.md beside this file): every JSON document
// under the directories it is given, compiled alone as a JSON module through the profile's compiler at
// the commit under test, and one line per document - its name, its outcome, the SHA-256 of the artifact's
// bytes and the first diagnostic's code. It decides nothing: the collector runs it twice at each commit
// and compares the outputs. It is a file-based program, run as
//
//   dotnet run -c Release docs/evidence/sjl-5-001/json-modules.cs -- <directory> [<directory> ...]
//
// and the project it names is the compiler of the checkout it sits in. Nothing here is timed.
#:project ../../../src/Broiler.VM.Profile.JavaScript.Compiler/Broiler.VM.Profile.JavaScript.Compiler.csproj

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;

if (args.Length == 0)
{
    Console.Error.WriteLine("usage: json-modules.cs <directory> [<directory> ...]");
    return 2;
}

var output = new StringBuilder();
output.Append("# name|outcome|artifact-sha256|diagnostic\n");

foreach (var directory in args)
{
    if (!Directory.Exists(directory))
    {
        Console.Error.WriteLine($"json-modules: no directory at {directory}");
        return 2;
    }

    var label = Path.GetFileName(Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory)));
    var paths = Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories)
        .Select(path => Path.GetRelativePath(directory, path).Replace('\\', '/'))
        .OrderBy(path => path, StringComparer.Ordinal);

    foreach (var relative in paths)
    {
        // The bytes are read as UTF-8, as the composition roots read a module's text.
        var text = File.ReadAllText(Path.Combine(directory, relative), new UTF8Encoding(false, true));
        var unit = new JsModuleUnit(relative, text, SliceParseOptions.Module) { Type = JsFormat.JsonModuleType };
        var compiled = JsCompiler.Compile([], [unit]);
        var hash = compiled.Artifact is null ? "-" : Convert.ToHexStringLower(SHA256.HashData(compiled.Artifact));
        var code = compiled.Diagnostics.Count == 0 ? "-" : compiled.Diagnostics[0].Code.ToString();

        output.Append(label).Append('/').Append(relative).Append('|')
            .Append(compiled.Succeeded ? "compiled" : "refused").Append('|')
            .Append(hash).Append('|').Append(code).Append('\n');
    }
}

Console.Out.Write(output.ToString());
return 0;
