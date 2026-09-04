// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Broiler.VM.HyperV;
using Broiler.VM.HyperV.Devices;

namespace Broiler.VM.HyperV.Cli;

/// <summary>
/// A command line front end for pico VMs: report what the host can do, run the sample programs,
/// or run a raw binary of x86-64 machine code.
/// </summary>
internal static class Program
{
    private const int Ok = 0;
    private const int Failed = 1;
    private const int Usage = 2;
    private const int Unavailable = 3;

    private static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return Usage;
        }

        try
        {
            return args[0].ToLowerInvariant() switch
            {
                "info" => Info(),
                "list" => List(),
                "run" => Run(args),
                "selftest" => SelfTest(),
                "help" or "--help" or "-h" => PrintUsage(),
                _ => Fail(string.Format(CultureInfo.InvariantCulture, "unknown command '{0}'", args[0])),
            };
        }
        catch (PlatformNotSupportedException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return Unavailable;
        }
        catch (Exception ex) when (ex is HyperVException or ArgumentException or IOException)
        {
            Console.Error.WriteLine(ex.Message);
            return Failed;
        }
    }

    private static int PrintUsage()
    {
        Console.WriteLine("""
            broiler-picovm - create pico VMs on the Windows Hypervisor Platform and run machine code in them

              broiler-picovm info                     what the host hypervisor reports
              broiler-picovm list                     the built-in sample programs
              broiler-picovm run <sample>             run a sample by name
              broiler-picovm run --file <path> [...]  run a file of raw machine code
              broiler-picovm selftest                 run every sample and check its result

            options for run --file:
              --mode real16|prot32|long64   processor mode to start in (default long64)
              --memory <bytes|4M|512K>      guest RAM (default 4M)
              --at <hex>                    guest physical address to load and start at
              --console-port <hex>          port whose bytes are written to stdout (default e9)
              --timeout <ms>                wall-clock budget for the run (default 5000)
              --dump                        print the register file when the run ends
            """);

        return Usage;
    }

    private static int Info()
    {
        Console.WriteLine("Windows Hypervisor Platform");
        Console.WriteLine("  available            : {0}", HyperVPlatform.IsAvailable);

        if (!HyperVPlatform.IsAvailable)
        {
            Console.WriteLine("  reason               : {0}", HyperVPlatform.UnavailableReason);
            return Unavailable;
        }

        HyperVPlatformInfo info = HyperVPlatform.GetInfo();
        Console.WriteLine("  processor vendor     : {0}", info.Vendor);
        Console.WriteLine("  clflush line size    : {0} bytes", info.CacheLineFlushSize);
        Console.WriteLine(
            "  physical address bits: {0}",
            info.PhysicalAddressWidthBits?.ToString(CultureInfo.InvariantCulture) ?? "not reported");
        Console.WriteLine("  feature bits         : 0x{0:X16}", info.Features);
        Console.WriteLine("  extended exit bits   : 0x{0:X16}", info.ExtendedVmExits);
        return Ok;
    }

    private static int List()
    {
        foreach (SampleProgram program in SampleProgram.All)
        {
            Console.WriteLine("{0,-9} {1,-11} {2}", program.Name, program.Mode, program.Summary);
        }

        return Ok;
    }

    private static int Run(string[] args)
    {
        if (args.Length < 2)
        {
            return Fail("run needs a sample name or --file <path>");
        }

        return args[1] == "--file" ? RunFile(args) : RunSample(args[1], dump: HasFlag(args, "--dump"));
    }

    private static int RunSample(string name, bool dump)
    {
        SampleProgram? program = SampleProgram.Find(name);
        if (program is null)
        {
            return Fail(string.Format(
                CultureInfo.InvariantCulture,
                "no sample called '{0}'; try broiler-picovm list",
                name));
        }

        Console.WriteLine("{0}: {1}", program.Name, program.Summary);
        Console.WriteLine();
        Console.WriteLine(Indent(program.Listing));
        Console.WriteLine();

        using PicoVm vm = new(new PicoVmOptions { Mode = program.Mode, Name = program.Name });
        vm.LoadCode(program.Code);
        program.Prepare?.Invoke(vm);

        long start = Stopwatch.GetTimestamp();
        VmExit exit = vm.Run(TimeSpan.FromSeconds(5));
        TimeSpan elapsed = Stopwatch.GetElapsedTime(start);

        Console.WriteLine("exit   : {0}", exit);
        Console.WriteLine("elapsed: {0:0.000} ms", elapsed.TotalMilliseconds);

        if (program.Report is not null)
        {
            Console.WriteLine("result : {0}", program.Report(vm, exit));
        }

        if (dump)
        {
            Console.WriteLine();
            Console.Write(vm.Cpu.Capture().Format());
        }

        string? problem = program.Verify?.Invoke(vm, exit);
        if (problem is not null)
        {
            Console.Error.WriteLine("FAILED : {0}", problem);
            return Failed;
        }

        return Ok;
    }

    private static int RunFile(string[] args)
    {
        string? path = ValueOf(args, "--file");
        if (path is null)
        {
            return Fail("--file needs a path");
        }

        if (!File.Exists(path))
        {
            return Fail(string.Format(CultureInfo.InvariantCulture, "no such file: {0}", path));
        }

        byte[] code = File.ReadAllBytes(path);

        PicoVmOptions options = new()
        {
            Name = Path.GetFileName(path),
            Mode = ParseMode(ValueOf(args, "--mode") ?? "long64"),
            MemorySize = ParseSize(ValueOf(args, "--memory") ?? "4M"),
        };

        if (ValueOf(args, "--at") is { } at)
        {
            options.CodeAddress = ParseHex(at);
        }

        ushort consolePort = (ushort)(ValueOf(args, "--console-port") is { } port ? ParseHex(port) : 0xE9);
        int timeout = ValueOf(args, "--timeout") is { } budget
            ? int.Parse(budget, CultureInfo.InvariantCulture)
            : 5000;

        using PicoVm vm = new(options);
        vm.LoadCode(code);
        vm.Io.Map(consolePort, new ConsolePortDevice());

        Console.WriteLine(
            "loaded {0} byte(s) at 0x{1:X} in {2}, {3} bytes of RAM, console on port 0x{4:X2}",
            code.Length,
            vm.CodeAddress,
            options.Mode,
            options.MemorySize,
            consolePort);

        long start = Stopwatch.GetTimestamp();
        VmExit exit = vm.Run(TimeSpan.FromMilliseconds(timeout));
        TimeSpan elapsed = Stopwatch.GetElapsedTime(start);

        Console.WriteLine();
        Console.WriteLine("exit   : {0}", exit);
        Console.WriteLine("elapsed: {0:0.000} ms", elapsed.TotalMilliseconds);

        if (HasFlag(args, "--dump"))
        {
            Console.WriteLine();
            Console.Write(vm.Cpu.Capture().Format());
        }

        return exit.IsHalt ? Ok : Failed;
    }

    private static int SelfTest()
    {
        int failures = 0;

        foreach (SampleProgram program in SampleProgram.All)
        {
            using PicoVm vm = new(new PicoVmOptions { Mode = program.Mode, Name = program.Name });
            vm.LoadCode(program.Code);
            program.Prepare?.Invoke(vm);

            VmExit exit = vm.Run(TimeSpan.FromSeconds(5));
            string? problem = program.Verify?.Invoke(vm, exit);

            if (problem is null)
            {
                Console.WriteLine(
                    "pass  {0,-9} {1,-11} {2}",
                    program.Name,
                    program.Mode,
                    program.Report?.Invoke(vm, exit) ?? exit.Reason.ToString());
            }
            else
            {
                failures++;
                Console.WriteLine("FAIL  {0,-9} {1,-11} {2}", program.Name, program.Mode, problem);
            }
        }

        Console.WriteLine();
        Console.WriteLine(
            "{0} sample(s), {1} failure(s)",
            SampleProgram.All.Count,
            failures);

        return failures == 0 ? Ok : Failed;
    }

    private static GuestMode ParseMode(string value) => value.ToLowerInvariant() switch
    {
        "real16" or "real" or "16" => GuestMode.Real16,
        "prot32" or "protected32" or "32" => GuestMode.Protected32,
        "long64" or "long" or "64" => GuestMode.Long64,
        _ => throw new ArgumentException(
            string.Format(CultureInfo.InvariantCulture, "unknown mode '{0}'", value),
            nameof(value)),
    };

    private static ulong ParseSize(string value)
    {
        char suffix = value[^1];
        ReadOnlySpan<char> digits = value.AsSpan(0, value.Length - 1);

        return char.ToLowerInvariant(suffix) switch
        {
            'k' => ulong.Parse(digits, CultureInfo.InvariantCulture) * 1024,
            'm' => ulong.Parse(digits, CultureInfo.InvariantCulture) * 1024 * 1024,
            _ => ulong.Parse(value, CultureInfo.InvariantCulture),
        };
    }

    private static ulong ParseHex(string value) => Convert.ToUInt64(
        value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? value[2..] : value,
        16);

    private static string? ValueOf(string[] args, string name)
    {
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (string.Equals(args[i], name, StringComparison.Ordinal))
            {
                return args[i + 1];
            }
        }

        return null;
    }

    private static bool HasFlag(string[] args, string name) => Array.IndexOf(args, name) >= 0;

    private static string Indent(string text) => "    " + text.ReplaceLineEndings("\n    ");

    private static int Fail(string message)
    {
        Console.Error.WriteLine(message);
        return Usage;
    }
}
