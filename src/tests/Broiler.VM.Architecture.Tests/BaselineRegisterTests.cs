using System.Globalization;
using System.Text.RegularExpressions;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group L: the rule that binds the baseline register to the run it claims to report.
/// </summary>
/// <remarks>
/// <para>
/// A published performance figure is the easiest claim in this repository to falsify by accident.
/// It is a number in a table; nothing about it looks stale; and unlike a test count it cannot be
/// recomputed by reading the checkout, because the only thing that knows it is a run that has
/// already finished. So the register is held to the log in BOTH directions - a figure nothing
/// measured cannot be published, and a measurement nobody declared cannot appear - and the log it
/// is held to is the one the evidence bundle retains, not one produced on demand.
/// </para>
/// <para>
/// The rule reads document against LOG and not log against checkout. A stale log and a stale
/// document agree with each other, and what covers that is the bundle's own expiry and
/// recertification triggers rather than anything assertable here. That is the same limit group H
/// records for the figures it binds, and it is EX-54.
/// </para>
/// <para>
/// ADR 0001 is the nominal owner: its revision 3 authorises the benchmark host whose output this
/// binds. It names no rule of this group, which exclusion EX-94 records rather than implying a
/// binding that does not exist.
/// </para>
/// </remarks>
public sealed class BaselineRegisterTests
{
    private const string RegisterName = "docs/baselines.md";

    /// <summary>The two lanes the gate asks for, and the marker each begins at in the log.</summary>
    /// <remarks>
    /// The Native AOT lane starts at the marker for RUNNING the native binary rather than at the
    /// one for publishing it, because the publish output sits between them and a publish that
    /// printed a line shaped like a measurement would otherwise be read as one.
    /// </remarks>
    private static readonly (string Lane, string Marker)[] Lanes =
    [
        ("jit", "--- JIT:"),
        ("aot", "--- running the native binary ---"),
    ];

    private static readonly Regex MeasurementLine = new(
        @"^measurement (?<id>[a-z0-9-]+) unit=(?<unit>[a-z]+) .*?per-(?<punit>[a-z]+)-ns=(?<value>-?[0-9.]+) .*?valid=(?<valid>yes|no|bound)",
        RegexOptions.Compiled);

    /// <summary>
    /// A row of the register's measurement table: the identifier, its unit, and one figure per lane.
    /// </summary>
    private sealed record RegisterRow(string Id, string Unit, string Jit, string Aot);

    /// <summary>One measurement as the log reports it, on one lane.</summary>
    private sealed record LoggedMeasurement(string Lane, string Id, string Unit, string Value, string Valid);

    // -------------------------------------------------------------------------------------
    // L1
    // -------------------------------------------------------------------------------------

    /// <summary>Writes what L1 said about this checkout, when asked to.</summary>
    /// <remarks>
    /// The same helper the test calls, over the same inputs. See <see cref="RuleReport"/>.
    /// </remarks>
    [Fact]
    public void RuleMessages_For_Group_L_Are_Written_When_Asked_For()
    {
        RuleReport.Write("L",
        [
            ("L1", () => Violations(RegisterRows(ReadRegister()), LoggedMeasurements(ReadLog()))),
        ]);

        if (RuleReport.Destination is { } destination)
        {
            Assert.True(
                File.Exists(Path.Combine(destination, "L.txt")),
                "a report for group L was asked for and none was written");
        }
    }

    [Fact]
    public void L1_The_Register_Declares_Exactly_What_The_Retained_Log_Measured()
    {
        var register = RegisterRows(ReadRegister());
        var logged = LoggedMeasurements(ReadLog());

        Assert.NotEmpty(register);
        Assert.NotEmpty(logged);

        // Both lanes really are present. Without this the rule would pass a log that ran one lane,
        // and the gate asks for two.
        foreach (var (lane, _) in Lanes)
        {
            Assert.Contains(logged, measurement => string.Equals(measurement.Lane, lane, StringComparison.Ordinal));
        }

        Assert.Empty(Violations(register, logged));
    }

    [Fact]
    public void L1_Rejects_A_Declared_Measurement_The_Log_Never_Ran()
    {
        var violations = Violations(
            RegisterRows(Witness("L1-register-declares-an-unmeasured-row.md.witness")),
            LoggedMeasurements(ReadLog()));

        Assert.Contains(violations, violation =>
            violation.Contains("verify-under-moonlight", StringComparison.Ordinal) &&
            violation.Contains("the log does not carry", StringComparison.Ordinal));
    }

    [Fact]
    public void L1_Rejects_A_Measured_Figure_The_Register_Never_Declared()
    {
        // The register loses a row, the log keeps the measurement. This is the direction a rule
        // written only one way misses, and it is the one that matters most: quietly dropping the
        // row for a figure that got worse is how a register becomes a highlight reel.
        var violations = Violations(
            RegisterRows(Witness("L1-register-omits-a-measured-row.md.witness")),
            LoggedMeasurements(ReadLog()));

        Assert.Contains(violations, violation =>
            violation.Contains("host-call", StringComparison.Ordinal) &&
            violation.Contains("declares no row", StringComparison.Ordinal));
    }

    [Fact]
    public void L1_Rejects_A_Figure_The_Log_Contradicts()
    {
        var violations = Violations(
            RegisterRows(Witness("L1-register-quotes-a-wrong-figure.md.witness")),
            LoggedMeasurements(ReadLog()));

        Assert.Contains(violations, violation =>
            violation.Contains("catalog-lookup", StringComparison.Ordinal) &&
            violation.Contains("jit", StringComparison.Ordinal) &&
            violation.Contains("quotes", StringComparison.Ordinal));
    }

    [Fact]
    public void L1_Rejects_Two_Lanes_Figures_Written_The_Wrong_Way_Round()
    {
        // The likeliest way this table goes wrong: two numbers of the same magnitude, two columns
        // apart. Checking each figure against its OWN lane fails twice here; checking membership in
        // the set of figures for that row would pass.
        var violations = Violations(
            RegisterRows(Witness("L1-register-exchanges-the-two-lanes.md.witness")),
            LoggedMeasurements(ReadLog()));

        Assert.Contains(violations, violation =>
            violation.Contains("diagnostics-capture", StringComparison.Ordinal) &&
            violation.Contains("jit", StringComparison.Ordinal));

        Assert.Contains(violations, violation =>
            violation.Contains("diagnostics-capture", StringComparison.Ordinal) &&
            violation.Contains("aot", StringComparison.Ordinal));
    }

    [Fact]
    public void L1_Rejects_A_Unit_The_Log_Does_Not_Agree_With()
    {
        var violations = Violations(
            RegisterRows(Witness("L1-register-renames-a-unit.md.witness")),
            LoggedMeasurements(ReadLog()));

        Assert.Contains(violations, violation =>
            violation.Contains("meter-per-instruction", StringComparison.Ordinal) &&
            violation.Contains("unit", StringComparison.Ordinal));
    }

    [Fact]
    public void L1_Rejects_A_Lane_The_Harness_Refused_To_Publish()
    {
        // A measurement whose A/A lane exceeded its effect is not a figure. The harness says so and
        // exits non-zero; this stops the register from publishing it anyway.
        var refused = LoggedMeasurements(WitnessLog("L1-log-carries-a-refused-measurement.log.witness"));
        var violations = Violations(RegisterRows(ReadRegister()), refused);

        Assert.Contains(violations, violation =>
            violation.Contains("catalog-lookup", StringComparison.Ordinal) &&
            violation.Contains("refused", StringComparison.Ordinal));
    }

    [Fact]
    public void L1_Holds_Its_Own_Register_Row_To_What_It_Proves()
    {
        var row = RuleRegisterTests.Loaded.Rules.Single(rule => string.Equals(rule.Id, "L1", StringComparison.Ordinal));

        Assert.Equal("Active", row.Status);
        Assert.Equal("0001", row.OwningAdr);
        Assert.Null(row.ActivationMilestone);

        // The statement must claim both directions, because a rule that claimed one and proved one
        // would be honest and a rule that claimed both and proved one is the over-claim this
        // register exists to prevent.
        Assert.Contains("both directions", row.Statement, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("bench.log", row.Evidence, StringComparison.Ordinal);
        Assert.Contains(RegisterName, row.Evidence, StringComparison.Ordinal);
    }

    // -------------------------------------------------------------------------------------
    // The rule itself
    // -------------------------------------------------------------------------------------

    /// <summary>
    /// Every way the register and the log can disagree, named one by one.
    /// </summary>
    /// <remarks>
    /// Each violation names the measurement, the lane and what was wrong with it, so a witness can
    /// assert the CONTENT of the failure it expects. A witness asserted with a bare non-empty check
    /// pins whichever clause happens to fire first, which is how a group loses independent clauses
    /// in one patch with the suite still green.
    /// </remarks>
    private static IReadOnlyList<string> Violations(
        IReadOnlyList<RegisterRow> register,
        IReadOnlyList<LoggedMeasurement> logged)
    {
        var violations = new List<string>();

        foreach (var (lane, _) in Lanes)
        {
            var onLane = logged
                .Where(measurement => string.Equals(measurement.Lane, lane, StringComparison.Ordinal))
                .ToList();

            foreach (var row in register)
            {
                var match = onLane.SingleOrDefault(
                    measurement => string.Equals(measurement.Id, row.Id, StringComparison.Ordinal));

                if (match is null)
                {
                    violations.Add(
                        $"{RegisterName} declares {row.Id} but the log does not carry it on the {lane} lane");
                    continue;
                }

                if (!string.Equals(match.Unit, row.Unit, StringComparison.Ordinal))
                {
                    violations.Add(
                        $"{RegisterName} gives {row.Id} the unit '{row.Unit}' and the {lane} lane " +
                        $"measured it per '{match.Unit}'");
                }

                var quoted = lane is "jit" ? row.Jit : row.Aot;

                if (!SameFigure(quoted, match.Value))
                {
                    violations.Add(
                        $"{RegisterName} quotes {quoted} for {row.Id} on the {lane} lane and the " +
                        $"log records {match.Value}");
                }
            }

            foreach (var measurement in onLane)
            {
                if (!register.Any(row => string.Equals(row.Id, measurement.Id, StringComparison.Ordinal)))
                {
                    violations.Add(
                        $"the {lane} lane measured {measurement.Id} and {RegisterName} declares no row for it");
                }

                // "no" is the harness refusing to publish: the A/A lane exceeded the effect. "bound"
                // is a null result it WAS willing to publish, declared in advance as a measurement
                // whose answer may be that there is nothing to resolve.
                if (string.Equals(measurement.Valid, "no", StringComparison.Ordinal))
                {
                    violations.Add(
                        $"the {lane} lane refused to publish {measurement.Id}: its A/A lane exceeded its effect");
                }
            }
        }

        return violations;
    }

    /// <summary>
    /// Whether a quoted figure and a logged one are the same number.
    /// </summary>
    /// <remarks>
    /// Compared as numbers rather than as strings, so a thousands separator in the document - which
    /// is how a reader wants four-digit nanoseconds written - is not a violation, while a different
    /// number still is. Exact equality, not a tolerance: the register quotes ONE retained run, and
    /// a tolerance would quietly admit a figure from a different one.
    /// </remarks>
    private static bool SameFigure(string quoted, string logged) =>
        double.TryParse(quoted.Replace(",", string.Empty, StringComparison.Ordinal),
            NumberStyles.Float, CultureInfo.InvariantCulture, out var left) &&
        double.TryParse(logged, NumberStyles.Float, CultureInfo.InvariantCulture, out var right) &&
        left.Equals(right);

    // -------------------------------------------------------------------------------------
    // Reading the two sides
    // -------------------------------------------------------------------------------------

    /// <summary>
    /// The measurement table's rows, read out of the register.
    /// </summary>
    /// <remarks>
    /// A row is recognised by its first cell being a backticked identifier the log could carry, not
    /// by its position in the file: the register has several tables and gains more as it is
    /// written, and a rule that counted tables would break every time a section was added. The two
    /// figure columns are the LAST two cells, which is what makes the lane columns positional and
    /// therefore exchangeable - which is exactly the error one of the witnesses exercises.
    /// </remarks>
    private static IReadOnlyList<RegisterRow> RegisterRows(string document)
    {
        var rows = new List<RegisterRow>();

        foreach (var line in document.Split('\n'))
        {
            var trimmed = line.Trim();

            if (!trimmed.StartsWith('|'))
            {
                continue;
            }

            var cells = trimmed
                .Trim('|')
                .Split('|')
                .Select(static cell => cell.Trim())
                .ToArray();

            // Identifier, unit, candidate, control, and one figure per lane.
            if (cells.Length != 6)
            {
                continue;
            }

            var id = cells[0].Trim('`');

            // A hyphenated lower-case token with at least one letter or digit in it. The letter
            // requirement is what keeps a GFM delimiter row out: `---` is hyphens and lower case
            // and would otherwise be read as a measurement named after the line under the header.
            if (!Regex.IsMatch(id, "^[a-z0-9-]+$") ||
                !id.Contains('-', StringComparison.Ordinal) ||
                !id.Any(char.IsLetterOrDigit))
            {
                continue;
            }

            rows.Add(new RegisterRow(id, cells[1], cells[4], cells[5]));
        }

        return rows;
    }

    /// <summary>Every measurement line in the log, tagged with the lane it was printed under.</summary>
    private static IReadOnlyList<LoggedMeasurement> LoggedMeasurements(string log)
    {
        var measurements = new List<LoggedMeasurement>();
        var lane = string.Empty;

        foreach (var line in log.Split('\n'))
        {
            var trimmed = line.Trim();

            foreach (var (name, marker) in Lanes)
            {
                if (trimmed.StartsWith(marker, StringComparison.Ordinal))
                {
                    lane = name;
                }
            }

            var match = MeasurementLine.Match(trimmed);

            if (!match.Success || lane.Length == 0)
            {
                continue;
            }

            // The per-unit group repeats the unit, so a line whose two spellings disagree is a
            // malformed line rather than a measurement, and reading either one alone would hide it.
            Assert.Equal(match.Groups["unit"].Value, match.Groups["punit"].Value);

            measurements.Add(new LoggedMeasurement(
                lane,
                match.Groups["id"].Value,
                match.Groups["unit"].Value,
                match.Groups["value"].Value,
                match.Groups["valid"].Value));
        }

        return measurements;
    }

    private static string ReadRegister() =>
        File.ReadAllText(Path.Combine(ComponentGraph.Root, "docs", "baselines.md"));

    private static string ReadLog()
    {
        var path = Path.Combine(
            ComponentGraph.Root, "docs", "evidence",
            ComponentGraph.CurrentEvidenceDirectory, "bench.log");

        Assert.True(File.Exists(path), $"The current evidence bundle retains no bench.log at {path}.");
        return File.ReadAllText(path);
    }

    private static string Witness(string fileName) => ReadWitness(fileName);

    private static string WitnessLog(string fileName) => ReadWitness(fileName);

    private static string ReadWitness(string fileName)
    {
        var path = Path.Combine(
            ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests",
            "witnesses", "baselines", fileName);

        Assert.True(File.Exists(path), $"Missing witness input {path}.");
        return File.ReadAllText(path);
    }
}
