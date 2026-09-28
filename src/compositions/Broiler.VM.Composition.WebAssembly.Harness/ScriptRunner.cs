using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Collections.Immutable;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>What a refusal says of the module: that it did not decode, that it decoded and is invalid, or neither.</summary>
/// <remarks>
/// <para>
/// <b>The core's reasons carry the category, and the pass that found it does not decide it.</b>
/// <c>SemanticValidationFailed</c> is a module that decoded and failed validation. <c>Truncated</c>,
/// <c>MalformedEncoding</c>, <c>UnknownFormatVersion</c> and <c>InconsistentStructure</c> are one that
/// did not decode. This profile's validator reads the function bodies, so a malformation inside one is
/// found by the validator: it carries a validation-band code and the reason <c>MalformedEncoding</c>,
/// and it says the module is malformed.
/// </para>
/// <para>
/// <b>A refusal says neither</b> when it is not an invalid artifact, when its code is not one of the
/// profile's module codes (2000 up to the reserved 2900 band), or when its reason is
/// <c>UnknownFeature</c>. A resource exhaustion is a host declining to spend, a code from the core or the
/// universal walk is not the profile judging the module, and an unadmitted feature is the profile
/// declining a construct before it judged whether the module is malformed or invalid.
/// </para>
/// </remarks>
internal enum ScriptJudgement
{
    None,
    Malformed,
    Invalid,
}

/// <summary>What verifying a script's module answered: an artifact, or the refusal's fields.</summary>
internal sealed class ScriptModule
{
    private ScriptModule(VmVerifiedArtifact? artifact, string refusal, ScriptJudgement judgement)
    {
        Artifact = artifact;
        Refusal = refusal;
        Judgement = judgement;
    }

    /// <summary>The verified artifact, or null when the module was refused.</summary>
    internal VmVerifiedArtifact? Artifact { get; }

    /// <summary>The refusal as its answer line prints it, or empty when the module was admitted.</summary>
    internal string Refusal { get; }

    /// <summary>What the refusal says of the module, or none.</summary>
    internal ScriptJudgement Judgement { get; }

    internal static ScriptModule Admitted(VmVerifiedArtifact artifact) => new(artifact, string.Empty, ScriptJudgement.None);

    internal static ScriptModule Refused(
        VmOutcome outcome, VmReason reason, int code, ulong offset, VmBudgetDimension dimension, VmBudgetScope scope) =>
        new(null,
            $"refused {outcome}/{reason}/{code.ToString(CultureInfo.InvariantCulture)}@{offset.ToString(CultureInfo.InvariantCulture)}" +
            (outcome is VmOutcome.ResourceExhaustion ? $"/{dimension}/{scope}" : string.Empty),
            JudgementOf(outcome, reason, code));

    private static ScriptJudgement JudgementOf(VmOutcome outcome, VmReason reason, int code) =>
        outcome is not VmOutcome.InvalidArtifact || code is < 2000 or >= 2900 ? ScriptJudgement.None
        : reason switch
        {
            VmReason.SemanticValidationFailed => ScriptJudgement.Invalid,
            VmReason.Truncated or VmReason.MalformedEncoding or VmReason.UnknownFormatVersion or VmReason.InconsistentStructure =>
                ScriptJudgement.Malformed,
            _ => ScriptJudgement.None,
        };
}

/// <summary>What the specification expects of a command, as the verdict reads it.</summary>
internal enum ScriptVerdict
{
    Pass,
    Fail,
    Excluded,
}

/// <summary>One command's line: where it is, what it is, what the profile answered and the verdict.</summary>
internal sealed record ScriptCommand(string File, int Ordinal, int Line, string Command, string Answer, ScriptVerdict Verdict)
{
    /// <summary>The command's identity: its file and its ordinal among that file's commands.</summary>
    internal string Id => $"{File}:{Ordinal.ToString(CultureInfo.InvariantCulture)}";

    public override string ToString() =>
        $"{Id} L{Line.ToString(CultureInfo.InvariantCulture)} {Command} | {Answer} | {Verdict.ToString().ToLowerInvariant()}";
}

/// <summary>
/// Runs one script's commands through the core, in a runtime of the script's own.
/// </summary>
/// <remarks>
/// <para>
/// <b>An answer is what the profile said; a verdict is whether the specification agrees.</b> Bundle
/// UBC-4-005's comparison is over answers, so that two runs agreeing on the same wrong answer still
/// agree. Its floor is over verdicts. Both are printed on one line per command.
/// </para>
/// <para>
/// <b>What a script can say that this profile cannot hear is answered as such, never skipped.</b>
/// Examples: a <c>get</c> of an exported global, which the invocation surface does not reach; a
/// command on a module that has no instance; a module the reader cannot encode. Each has an answer,
/// and the verdict fails it. A command left out would make a smaller total look like a better one.
/// </para>
/// <para>
/// <b>A module that is not current is disposed when the next module replaces it</b>, unless it has a
/// name or was registered. Its memory then does not stay charged against the next module's ceilings
/// for the rest of the script.
/// </para>
/// </remarks>
internal sealed class ScriptRunner : IDisposable
{
    private readonly string file;
    private readonly VmRuntime runtime;
    private readonly Dictionary<string, Loaded> named = new(StringComparer.Ordinal);
    private readonly List<Loaded> loaded = [];
    private Loaded? current;
    private int ordinal;

    private ScriptRunner(string file, VmRuntime runtime)
    {
        this.file = file;
        this.runtime = runtime;
    }

    /// <summary>Creates the runtime a script runs in: the harness root's options over the adapter's catalog.</summary>
    internal static VmRuntime? CreateRuntime(out string failure)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            ScriptVerification.Catalog(),
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: ImmutableArray<VmCapabilityRegistration>.Empty));

        failure = created.TryGetRuntime(out var runtime)
            ? string.Empty
            : $"runtime creation {created.Outcome}/{created.Reason}";

        return runtime;
    }

    /// <summary>Runs every command of one script, in a runtime of its own, and answers one line per command.</summary>
    internal static List<ScriptCommand> Run(string file, byte[] source)
    {
        var results = new List<ScriptCommand>();
        List<SExpr> commands;

        try
        {
            commands = ScriptText.Read(source);
        }
        catch (ScriptReadException failure)
        {
            results.Add(new ScriptCommand(file, 0, 0, "script", $"reader-refused {failure.Message}", ScriptVerdict.Fail));
            return results;
        }

        var runtime = CreateRuntime(out var creation) ??
            throw new InvalidOperationException($"{file}: {creation}");

        using var runner = new ScriptRunner(file, runtime);

        foreach (var command in commands)
        {
            if (command.Head is "register")
            {
                runner.Register(command);
                runner.ordinal++;
                continue;
            }

            runner.ordinal++;
            results.Add(runner.Command(command));
        }

        return results;
    }

    public void Dispose()
    {
        foreach (var module in loaded)
        {
            module.Instance?.Dispose();
        }

        runtime.Dispose();
    }

    private ScriptCommand Command(SExpr command)
    {
        var head = command.Head ?? "(unknown)";

        try
        {
            return head switch
            {
                "module" => TopLevelModule(command),
                "invoke" or "get" => TopLevelAction(command, head),
                "assert_return" => AssertReturn(command),
                "assert_return_canonical_nan" or "assert_return_arithmetic_nan" => AssertNan(command, head),
                "assert_trap" => AssertTrap(command),
                "assert_exhaustion" => AssertExhaustion(command),
                "assert_malformed" or "assert_invalid" or "assert_unlinkable" => AssertModule(command, head),
                _ => new ScriptCommand(file, ordinal, command.Line, head, "reader-refused unknown command", ScriptVerdict.Fail),
            };
        }
        catch (ScriptReadException failure)
        {
            return new ScriptCommand(file, ordinal, command.Line, head, $"reader-refused {failure.Message}", ScriptVerdict.Fail);
        }
    }

    private ScriptCommand TopLevelAction(SExpr command, string head)
    {
        var action = Action(command);
        return new ScriptCommand(file, ordinal, command.Line, head, action.Text, action.Pass ? ScriptVerdict.Pass : ScriptVerdict.Fail);
    }

    // =============================================================================================
    // Modules
    // =============================================================================================

    private ScriptCommand TopLevelModule(SExpr command)
    {
        var name = command.Items!.Count > 1 && !command.Items[1].IsList && command.Items[1].Kind is AtomKind.Id
            ? command.Items[1].Text
            : null;

        var module = Define(command);

        if (current is not null && current.Name is null && !current.Registered)
        {
            current.Instance?.Dispose();
            loaded.Remove(current);
        }

        current = module;

        if (name is not null)
        {
            named[name] = module;
            module.Name = name;
        }

        return new ScriptCommand(file, ordinal, command.Line, "module", module.Answer,
            module.Instance is not null ? ScriptVerdict.Pass : ScriptVerdict.Fail);
    }

    /// <summary>Encodes, verifies and instantiates a module command's module.</summary>
    private Loaded Define(SExpr command)
    {
        byte[] bytes;

        try
        {
            bytes = Bytes(command);
        }
        catch (ScriptReadException failure)
        {
            return Keep(new Loaded(null, $"reader-refused {failure.Message}"));
        }

        var verified = ScriptVerification.Verify(runtime, bytes, Id);

        if (verified.Artifact is not { } artifact)
        {
            return Keep(new Loaded(null, verified.Refusal) { Judgement = verified.Judgement });
        }

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (instantiated.TryGetInstance(out var instance))
            {
                return Keep(new Loaded(instance, "instance"));
            }

            var carried = WebAssemblyProfile.TryGetTrap(in instantiated, out var trap);

            return Keep(new Loaded(null,
                $"instantiation {instantiated.Outcome}/{instantiated.Reason}" +
                (carried ? $" trap={trap.Kind}" : string.Empty) +
                Exhaustion(instantiated.Outcome, instantiated.Diagnostics)));
        }
    }

    private Loaded Keep(Loaded module)
    {
        loaded.Add(module);
        return module;
    }

    private static byte[] Bytes(SExpr command)
    {
        var items = command.Items!;
        var at = 1;

        if (at < items.Count && !items[at].IsList && items[at].Kind is AtomKind.Id)
        {
            at++;
        }

        if (at < items.Count && items[at].IsWord("binary"))
        {
            return ModuleWriter.Strings(items, at + 1, command);
        }

        if (at < items.Count && items[at].IsWord("quote"))
        {
            throw new ScriptReadException("a module quote outside assert_malformed", command.Line);
        }

        return TextModule.Encode(command);
    }

    private void Register(SExpr command)
    {
        var target = command.Items!.Count > 2 ? Target(command.Items[2]) : current;

        if (target is not null)
        {
            target.Registered = true;
        }
    }

    private Loaded? Target(SExpr? id) =>
        id is { IsList: false, Kind: AtomKind.Id } ? named.GetValueOrDefault(id.Text) : current;

    private string Id => $"{file}:{ordinal.ToString(CultureInfo.InvariantCulture)}";

    // =============================================================================================
    // Actions
    // =============================================================================================

    /// <summary>What an action answered: its line, and the values when it returned normally.</summary>
    private sealed record ActionAnswer(string Text, IReadOnlyList<WebAssemblyValue>? Values, WasmTrapKind? Trap, VmOutcome Outcome, VmBudgetDimension Dimension)
    {
        internal bool Pass => Values is not null;
    }

    private ActionAnswer Action(SExpr action)
    {
        var items = action.Items!;
        var at = 1;
        var target = current;

        if (at < items.Count && !items[at].IsList && items[at].Kind is AtomKind.Id)
        {
            target = named.GetValueOrDefault(items[at].Text);
            at++;
        }

        if (action.Head is "get")
        {
            return new ActionAnswer("no-surface get", null, null, VmOutcome.Normal, default);
        }

        if (action.Head is not "invoke")
        {
            throw new ScriptReadException($"unknown action {action.Head}", action.Line);
        }

        var name = at < items.Count && items[at].Kind is AtomKind.String && !items[at].IsList
            ? items[at].Bytes!
            : throw new ScriptReadException("an invoke without an export name", action.Line);

        at++;

        var entry = new List<byte>();
        entry.AddRange(Encoding.ASCII.GetBytes(name.Length.ToString(CultureInfo.InvariantCulture) + ":"));
        entry.AddRange(name);

        for (; at < items.Count; at++)
        {
            entry.AddRange(Encoding.ASCII.GetBytes(Argument(items[at])));
        }

        if (target?.Instance is not { } instance)
        {
            return new ActionAnswer("no-instance", null, null, VmOutcome.Normal, default);
        }

        var entryBytes = entry.ToArray();
        VmInvocationRequest request;

        try
        {
            request = new VmInvocationRequest(new VmUtf8Text(entryBytes));
        }
        catch (ArgumentException failure)
        {
            throw new ScriptReadException($"an entry point the invocation surface cannot carry: {failure.Message}", action.Line);
        }
        var answered = instance.Invoke(in request, CancellationToken.None);

        if (WebAssemblyProfile.TryGetTrap(in answered, out var trap))
        {
            return new ActionAnswer($"trap {trap.Kind}", null, trap.Kind, answered.Outcome, default);
        }

        if (answered.Outcome is VmOutcome.Normal && WebAssemblyProfile.TryGetResults(in answered, out var results))
        {
            var values = new List<WebAssemblyValue>();

            for (var index = 0; index < results.Count; index++)
            {
                if (results.TryGetValue(index, out var value))
                {
                    values.Add(value);
                }
            }

            return new ActionAnswer("values" + string.Concat(values.Select(static v => " " + Value(v.Kind, v.Bits))), values, null, VmOutcome.Normal, default);
        }

        if (WebAssemblyProfile.TryGetEntryPointFault(in answered, out var fault))
        {
            return new ActionAnswer($"fault {fault.Problem}", null, null, answered.Outcome, default);
        }

        return new ActionAnswer(
            $"{answered.Outcome}/{answered.Reason}" + Exhaustion(answered.Outcome, answered.Diagnostics),
            null, null, answered.Outcome, answered.Diagnostics.ExhaustedDimension);
    }

    /// <summary>A constant argument in the profile's entry-point grammar: its type, its literal's length and the literal.</summary>
    private static string Argument(SExpr constant)
    {
        var (kind, bits) = Constant(constant);

        var literal = kind switch
        {
            WebAssemblyValueKind.I32 => ((int)(uint)bits).ToString(CultureInfo.InvariantCulture),
            WebAssemblyValueKind.I64 => ((long)bits).ToString(CultureInfo.InvariantCulture),
            WebAssemblyValueKind.F32 => ((uint)bits).ToString("X8", CultureInfo.InvariantCulture),
            _ => bits.ToString("X16", CultureInfo.InvariantCulture),
        };

        var type = kind.ToString().ToLowerInvariant();
        return $"{type}:{literal.Length.ToString(CultureInfo.InvariantCulture)}:{literal}";
    }

    private static (WebAssemblyValueKind Kind, ulong Bits) Constant(SExpr constant)
    {
        if (!constant.IsList || constant.Items!.Count != 2 || constant.Items[1].IsList)
        {
            throw new ScriptReadException("a malformed constant", constant.Line);
        }

        var literal = constant.Items[1];

        return constant.Head switch
        {
            "i32.const" => (WebAssemblyValueKind.I32, ScriptNumbers.I32(literal.Text, literal.Line)),
            "i64.const" => (WebAssemblyValueKind.I64, ScriptNumbers.I64(literal.Text, literal.Line)),
            "f32.const" => (WebAssemblyValueKind.F32, ScriptNumbers.F32(literal.Text, literal.Line)),
            "f64.const" => (WebAssemblyValueKind.F64, ScriptNumbers.F64(literal.Text, literal.Line)),
            _ => throw new ScriptReadException($"an unknown constant {constant.Head}", constant.Line),
        };
    }

    private static string Value(WebAssemblyValueKind kind, ulong bits) => kind switch
    {
        WebAssemblyValueKind.I32 or WebAssemblyValueKind.F32 =>
            $"{kind.ToString().ToLowerInvariant()}:{((uint)bits).ToString("X8", CultureInfo.InvariantCulture)}",
        _ => $"{kind.ToString().ToLowerInvariant()}:{bits.ToString("X16", CultureInfo.InvariantCulture)}",
    };

    private static string Exhaustion(VmOutcome outcome, VmDiagnostics diagnostics) =>
        outcome is VmOutcome.ResourceExhaustion ? $"/{diagnostics.ExhaustedDimension}/{diagnostics.ExhaustedScope}" : string.Empty;

    // =============================================================================================
    // Assertions
    // =============================================================================================

    private ScriptCommand AssertReturn(SExpr command)
    {
        var items = command.Items!;
        var action = Action(Required(command, 1));
        var expected = items.Skip(2).Select(Constant).ToList();

        var pass = action.Values is { } values &&
            values.Count == expected.Count &&
            values.Zip(expected).All(static pair => pair.First.Kind == pair.Second.Kind && pair.First.Bits == pair.Second.Bits);

        return new ScriptCommand(file, ordinal, command.Line, "assert_return", action.Text, pass ? ScriptVerdict.Pass : ScriptVerdict.Fail);
    }

    private ScriptCommand AssertNan(SExpr command, string head)
    {
        var action = Action(Required(command, 1));
        var canonical = head is "assert_return_canonical_nan";

        var pass = action.Values is [var value] && value.Kind switch
        {
            WebAssemblyValueKind.F32 => canonical
                ? ((uint)value.Bits & 0x7FFF_FFFFu) == 0x7FC0_0000u
                : ((uint)value.Bits & 0x7FC0_0000u) == 0x7FC0_0000u,
            WebAssemblyValueKind.F64 => canonical
                ? (value.Bits & 0x7FFF_FFFF_FFFF_FFFFul) == 0x7FF8_0000_0000_0000ul
                : (value.Bits & 0x7FF8_0000_0000_0000ul) == 0x7FF8_0000_0000_0000ul,
            _ => false,
        };

        return new ScriptCommand(file, ordinal, command.Line, head, action.Text, pass ? ScriptVerdict.Pass : ScriptVerdict.Fail);
    }

    private ScriptCommand AssertTrap(SExpr command)
    {
        var subject = Required(command, 1);
        var message = Message(command);

        if (subject.Head is "module")
        {
            var module = Define(subject);
            Discard(module);
            var trapped = module.Answer.StartsWith("instantiation ", StringComparison.Ordinal) &&
                TrapMatches(module.Answer, message);

            return new ScriptCommand(file, ordinal, command.Line, "assert_trap", module.Answer, trapped ? ScriptVerdict.Pass : ScriptVerdict.Fail);
        }

        var action = Action(subject);
        var pass = action.Trap is { } kind && Matches(kind, message);
        return new ScriptCommand(file, ordinal, command.Line, "assert_trap", action.Text, pass ? ScriptVerdict.Pass : ScriptVerdict.Fail);
    }

    private ScriptCommand AssertExhaustion(SExpr command)
    {
        var action = Action(Required(command, 1));
        var pass = action.Outcome is VmOutcome.ResourceExhaustion && action.Dimension is VmBudgetDimension.CallDepth;
        return new ScriptCommand(file, ordinal, command.Line, "assert_exhaustion", action.Text, pass ? ScriptVerdict.Pass : ScriptVerdict.Fail);
    }

    /// <summary>
    /// A module assertion: malformed, invalid or unlinkable, each scored by what the refusal says.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A refusal passes only the assertion its category answers</b> (see <see cref="ScriptJudgement"/>).
    /// A malformed module has to be refused as one that did not decode, an invalid one as one that
    /// decoded and failed validation. An unlinkable one has to verify and then fail to instantiate. A
    /// module that is both malformed and invalid is malformed, so a profile that validated before it
    /// finished decoding fails it. A resource exhaustion, a feature this profile does not admit, and a
    /// verification refusal of a module the script calls unlinkable pass none of them: the profile
    /// declined the module before it answered what the script asks.
    /// </para>
    /// <para>
    /// <i>(Corrected 2026-09-28. The malformed and invalid assertions passed on any refusal, and the
    /// unlinkable one on any module without an instance. So a module this profile refused because it
    /// imports passed as unlinkable, and one it refused as the other category, or for want of a
    /// feature, passed as malformed or invalid. Bundles UBC-4-005 and UBC-4-006 scored their runs that way, and
    /// their comparisons were over answers, which this does not change.)</i>
    /// </para>
    /// </remarks>
    private ScriptCommand AssertModule(SExpr command, string head)
    {
        var subject = Required(command, 1);

        // Class (t) of bundle UBC-4-005's rule: a quoted module is text the profile never receives.
        if (head is "assert_malformed" && subject.Items!.Any(static item => item.IsWord("quote")))
        {
            return new ScriptCommand(file, ordinal, command.Line, head, "excluded (t)", ScriptVerdict.Excluded);
        }

        var module = Define(subject);
        Discard(module);

        var pass = head switch
        {
            "assert_unlinkable" => module.Instance is null && module.Answer.StartsWith("instantiation ", StringComparison.Ordinal),
            "assert_malformed" => module.Judgement is ScriptJudgement.Malformed,
            _ => module.Judgement is ScriptJudgement.Invalid,
        };

        return new ScriptCommand(file, ordinal, command.Line, head, module.Answer, pass ? ScriptVerdict.Pass : ScriptVerdict.Fail);
    }

    /// <summary>An assertion's module is never current: one that instantiated anyway is disposed at once.</summary>
    private void Discard(Loaded module)
    {
        module.Instance?.Dispose();
        loaded.Remove(module);
    }

    private static SExpr Required(SExpr command, int at) =>
        at < command.Items!.Count ? command.Items[at] : throw new ScriptReadException($"{command.Head} is missing a part", command.Line);

    private static string Message(SExpr command) =>
        command.Items![^1] is { IsList: false, Kind: AtomKind.String } message
            ? message.Text
            : throw new ScriptReadException($"{command.Head} without a message", command.Line);

    private static bool TrapMatches(string answer, string message)
    {
        var at = answer.IndexOf(" trap=", StringComparison.Ordinal);

        if (at < 0)
        {
            return false;
        }

        var name = answer[(at + " trap=".Length)..];
        var end = name.IndexOf('/', StringComparison.Ordinal);
        name = end < 0 ? name : name[..end];
        return Enum.TryParse<WasmTrapKind>(name, out var kind) && Matches(kind, message);
    }

    /// <summary>
    /// Whether a trap's kind answers a message: one of the kind's message and the expected message is a
    /// prefix of the other, as the reference interpreter compares them.
    /// </summary>
    private static bool Matches(WasmTrapKind kind, string message)
    {
        var canonical = kind switch
        {
            WasmTrapKind.Unreachable => "unreachable",
            WasmTrapKind.IntegerDivideByZero => "integer divide by zero",
            WasmTrapKind.IntegerOverflow => "integer overflow",
            WasmTrapKind.InvalidConversionToInteger => "invalid conversion to integer",
            WasmTrapKind.OutOfBoundsMemoryAccess => "out of bounds memory access",
            WasmTrapKind.OutOfBoundsTableAccess or WasmTrapKind.UndefinedElement => "undefined element",
            WasmTrapKind.IndirectCallTypeMismatch => "indirect call type mismatch",
            WasmTrapKind.UninitializedElement => "uninitialized element",
            _ => null,
        };

        return canonical is not null &&
            (canonical.StartsWith(message, StringComparison.Ordinal) || message.StartsWith(canonical, StringComparison.Ordinal));
    }

    /// <summary>A module a script defined: its instance if it has one, and its answer.</summary>
    private sealed class Loaded(VmInstance? instance, string answer)
    {
        internal VmInstance? Instance { get; } = instance;

        internal string Answer { get; } = answer;

        /// <summary>What verification's refusal said of the module, or none.</summary>
        internal ScriptJudgement Judgement { get; init; }

        internal string? Name { get; set; }

        internal bool Registered { get; set; }
    }
}
