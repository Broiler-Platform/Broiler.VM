using System.Globalization;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Rule N33: the JavaScript profile's baseline register and the logs its bundle retains agree in both
/// directions (phase F9 slice R1, decision JSD-0059).
/// </summary>
/// <remarks>
/// <para>
/// Rule L1's shape over the profile's own register: a figure nothing measured cannot be published, a
/// measurement nobody declared cannot appear, each arm's figure is the one that arm logged, and a
/// figure the lane refused to publish - its A/A lane exceeding its effect - may not be quoted at all.
/// The profile's arms are logged one to a file rather than under markers in one log, so the arm is
/// read from the file's name.
/// </para>
/// <para>
/// <b>The same limit as L1.</b> The rule reads document against LOG and not log against checkout: a
/// stale log and a stale document agree with each other, and what covers that is the bundle's
/// recertification triggers rather than anything assertable here.
/// </para>
/// </remarks>
public sealed class JsBaselineRegisterTests
{
    private const string RegisterName = "src/Broiler.VM.Profile.JavaScript/docs/baselines.md";

    private static readonly string[] Arms = ["jit", "aot"];

    private static readonly Regex MeasurementLine = new(
        @"^measurement (?<id>[a-z0-9-]+) unit=(?<unit>[a-z]+) .*?per-(?<punit>[a-z]+)-ns=(?<value>-?[0-9.]+) .*?valid=(?<valid>yes|no)",
        RegexOptions.Compiled);

    private static readonly Regex BundleLink = new(@"\*\*bundle \[(?<id>JS-10-[0-9]{3})\]\(evidence/(?<directory>js-10-[0-9]{3})/README\.md\)\*\*", RegexOptions.Compiled);

    private sealed record RegisterRow(string Id, string Unit, string Jit, string Aot);

    private sealed record LoggedMeasurement(string Arm, string Id, string Unit, string Value, string Valid);

    [Fact]
    public void N33_The_Profile_Register_Declares_Exactly_What_Its_Bundle_Measured()
    {
        var register = RegisterRows(ReadRegister());
        var logged = ReadLogs();

        Assert.Equal(2, register.Count);
        Assert.Equal(4, logged.Count);

        foreach (var arm in Arms)
        {
            Assert.Contains(logged, measurement => measurement.Arm == arm);
        }

        Assert.Empty(Violations(register, logged));
    }

    [Fact]
    public void N33_Rejects_A_Declared_Measurement_No_Log_Carries()
    {
        var violations = Violations(RegisterRows(Witness("N33-register-declares-an-unmeasured-row.md.witness")), ReadLogs());

        Assert.Contains(violations, violation =>
            violation.Contains("instantiate-per-function", StringComparison.Ordinal) &&
            violation.Contains("does not carry", StringComparison.Ordinal));
    }

    [Fact]
    public void N33_Rejects_A_Measured_Figure_The_Register_Never_Declared()
    {
        var violations = Violations(RegisterRows(Witness("N33-register-omits-a-measured-row.md.witness")), ReadLogs());

        Assert.Contains(violations, violation =>
            violation.Contains("cold-start", StringComparison.Ordinal) &&
            violation.Contains("declares no row", StringComparison.Ordinal));
    }

    [Fact]
    public void N33_Rejects_Two_Arms_Figures_Written_The_Wrong_Way_Round()
    {
        var violations = Violations(RegisterRows(Witness("N33-register-exchanges-the-two-arms.md.witness")), ReadLogs());

        Assert.Contains(violations, violation =>
            violation.Contains("verify-throughput", StringComparison.Ordinal) && violation.Contains("jit", StringComparison.Ordinal));
        Assert.Contains(violations, violation =>
            violation.Contains("verify-throughput", StringComparison.Ordinal) && violation.Contains("aot", StringComparison.Ordinal));
    }

    [Fact]
    public void N33_Rejects_A_Figure_The_Lane_Refused_To_Publish()
    {
        var refused = Measurements("aot", Witness("N33-log-carries-a-refused-measurement.log.witness"));
        var violations = Violations(RegisterRows(ReadRegister()), [.. ReadLogs().Where(measurement => measurement.Arm == "jit"), .. refused]);

        Assert.Contains(violations, violation =>
            violation.Contains("cold-start", StringComparison.Ordinal) &&
            violation.Contains("refused", StringComparison.Ordinal));
    }

    [Fact]
    public void N33_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(rule => string.Equals(rule.Id, "N33", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Null(row.ActivationMilestone);
        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(RegisterName, row.Evidence, StringComparison.Ordinal);
    }

    private static List<string> Violations(IReadOnlyList<RegisterRow> register, IReadOnlyList<LoggedMeasurement> logged)
    {
        var violations = new List<string>();

        foreach (var arm in Arms)
        {
            var onArm = logged.Where(measurement => measurement.Arm == arm).ToList();

            foreach (var row in register)
            {
                var match = onArm.SingleOrDefault(measurement => measurement.Id == row.Id);

                if (match is null)
                {
                    violations.Add($"{RegisterName} declares {row.Id} but the {arm} arm's log does not carry it");
                    continue;
                }

                if (match.Unit != row.Unit)
                {
                    violations.Add($"{RegisterName} gives {row.Id} the unit '{row.Unit}' and the {arm} arm measured it per '{match.Unit}'");
                }

                var quoted = arm == "jit" ? row.Jit : row.Aot;

                if (!SameFigure(quoted, match.Value))
                {
                    violations.Add($"{RegisterName} quotes {quoted} for {row.Id} on the {arm} arm and its log records {match.Value}");
                }
            }

            foreach (var measurement in onArm)
            {
                if (!register.Any(row => row.Id == measurement.Id))
                {
                    violations.Add($"the {arm} arm measured {measurement.Id} and {RegisterName} declares no row for it");
                }

                if (measurement.Valid == "no")
                {
                    violations.Add($"the {arm} arm refused to publish {measurement.Id}: its A/A lane exceeded its effect");
                }
            }
        }

        return violations;
    }

    /// <summary>Compared as numbers, so a thousands separator is not a violation and a different number is; exactly, because the register quotes one run.</summary>
    private static bool SameFigure(string quoted, string logged) =>
        double.TryParse(quoted.Replace(",", string.Empty, StringComparison.Ordinal), NumberStyles.Float, CultureInfo.InvariantCulture, out var left) &&
        double.TryParse(logged, NumberStyles.Float, CultureInfo.InvariantCulture, out var right) &&
        left.Equals(right);

    /// <summary>The measurement table's rows: a backticked hyphenated identifier, a unit, two descriptions, and one figure per arm.</summary>
    private static List<RegisterRow> RegisterRows(string document)
    {
        var rows = new List<RegisterRow>();

        foreach (var line in document.Split('\n'))
        {
            var trimmed = line.Trim();

            if (!trimmed.StartsWith('|'))
            {
                continue;
            }

            var cells = trimmed.Trim('|').Split('|').Select(static cell => cell.Trim()).ToArray();

            if (cells.Length != 6 || !cells[0].StartsWith('`'))
            {
                continue;
            }

            var id = cells[0].Trim('`');

            if (Regex.IsMatch(id, "^[a-z0-9]+(-[a-z0-9]+)+$"))
            {
                rows.Add(new RegisterRow(id, cells[1], cells[4], cells[5]));
            }
        }

        return rows;
    }

    private static List<LoggedMeasurement> Measurements(string arm, string log)
    {
        var measurements = new List<LoggedMeasurement>();

        foreach (var line in log.Split('\n'))
        {
            var match = MeasurementLine.Match(line.Trim());

            if (!match.Success)
            {
                continue;
            }

            Assert.Equal(match.Groups["unit"].Value, match.Groups["punit"].Value);
            measurements.Add(new LoggedMeasurement(arm, match.Groups["id"].Value, match.Groups["unit"].Value, match.Groups["value"].Value, match.Groups["valid"].Value));
        }

        return measurements;
    }

    private static string ReadRegister() => File.ReadAllText(Path.Combine(ComponentGraph.Root, RegisterName));

    /// <summary>Both arms' logs, from the bundle the register names.</summary>
    private static List<LoggedMeasurement> ReadLogs()
    {
        var link = BundleLink.Match(ReadRegister());
        Assert.True(link.Success, $"{RegisterName} names no bundle in the form **bundle [JS-10-nnn](evidence/js-10-nnn/README.md)**");

        var directory = Path.Combine(ComponentGraph.Root, "src", "Broiler.VM.Profile.JavaScript", "docs", "evidence", link.Groups["directory"].Value);
        var measurements = new List<LoggedMeasurement>();

        foreach (var arm in Arms)
        {
            var path = Path.Combine(directory, $"measure-{arm}.log");
            Assert.True(File.Exists(path), $"bundle {link.Groups["id"].Value} retains no {Path.GetFileName(path)}");
            measurements.AddRange(Measurements(arm, File.ReadAllText(path)));
        }

        return measurements;
    }

    private static string Witness(string fileName)
    {
        var path = Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "witnesses", "js-baselines", fileName);
        Assert.True(File.Exists(path), $"Missing witness input {path}.");
        return File.ReadAllText(path);
    }
}
