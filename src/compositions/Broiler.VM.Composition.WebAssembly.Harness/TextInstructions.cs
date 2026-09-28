namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>What follows an instruction's keyword in the text format.</summary>
internal enum Immediate
{
    None,
    Block,
    Label,
    LabelTable,
    Func,
    TypeUse,
    Local,
    Global,
    Memory,
    MemoryIndex,
    I32,
    I64,
    F32,
    F64,
}

/// <summary>
/// Encodes one expression - a function body, an initialiser or an offset - in either of the text
/// format's two forms: the plain sequence closed by <c>end</c>, and the folded one.
/// </summary>
/// <remarks>
/// A folded instruction's operands are written before it, and a folded block's body needs no
/// <c>end</c>: both forms reach the same bytes. Labels are resolved against the blocks around the
/// instruction as it is written; a numeric label is written as it stands.
/// </remarks>
internal sealed class BodyWriter
{
    private static readonly Dictionary<string, (byte Opcode, Immediate Immediate, byte Alignment)> Opcodes = Build();

    private readonly ModuleWriter module;
    private readonly List<byte> output;
    private readonly Dictionary<string, uint> locals;
    private readonly List<string?> labels = [];

    internal BodyWriter(ModuleWriter module, List<byte> output, Dictionary<string, uint> locals)
    {
        this.module = module;
        this.output = output;
        this.locals = locals;
    }

    /// <summary>Writes <c>items[from..to)</c>, every one of which is an instruction or part of one.</summary>
    internal void Sequence(List<SExpr> items, int from, int to)
    {
        var at = from;

        while (at < to)
        {
            at = Instruction(items, at, to);
        }
    }

    private int Instruction(List<SExpr> items, int at, int to)
    {
        var item = items[at];

        if (item.IsList)
        {
            Folded(item);
            return at + 1;
        }

        if (item.Kind is not AtomKind.Word)
        {
            throw new ScriptReadException($"an instruction was expected, not {item.Text}", item.Line);
        }

        return item.Text switch
        {
            "block" or "loop" or "if" => PlainBlock(items, at, to),
            "else" or "end" => throw new ScriptReadException($"an unmatched {item.Text}", item.Line),
            _ => Plain(items, at, to),
        };
    }

    /// <summary>A plain <c>block</c>, <c>loop</c> or <c>if</c>, through its <c>end</c>.</summary>
    private int PlainBlock(List<SExpr> items, int at, int to)
    {
        var keyword = items[at];
        var isIf = keyword.Text is "if";
        at++;
        var label = OptionalId(items, ref at, to);
        at = BlockType(keyword, items, at, to);
        labels.Add(label);
        var elseAt = -1;

        while (true)
        {
            if (at >= to)
            {
                throw new ScriptReadException($"{keyword.Text} without end", keyword.Line);
            }

            var current = items[at];

            if (current.IsWord("end"))
            {
                // An else with nothing after it is the same block as none, and the reference
                // encoder writes none.
                if (elseAt >= 0 && output.Count == elseAt + 1)
                {
                    output.RemoveAt(elseAt);
                }

                output.Add(0x0B);
                at++;
                SkipMatchingId(items, ref at, to, label);
                break;
            }

            if (isIf && current.IsWord("else"))
            {
                elseAt = output.Count;
                output.Add(0x05);
                at++;
                SkipMatchingId(items, ref at, to, label);
                continue;
            }

            at = Instruction(items, at, to);
        }

        labels.RemoveAt(labels.Count - 1);
        return at;
    }

    private void Folded(SExpr list)
    {
        var items = list.Items!;
        var head = list.Head ?? throw new ScriptReadException("a folded instruction without a keyword", list.Line);
        var at = 1;

        switch (head)
        {
            case "block" or "loop":
            {
                var label = OptionalId(items, ref at, items.Count);
                at = BlockType(list, items, at, items.Count, head);
                labels.Add(label);
                Sequence(items, at, items.Count);
                labels.RemoveAt(labels.Count - 1);
                output.Add(0x0B);
                return;
            }

            case "if":
            {
                var label = OptionalId(items, ref at, items.Count);
                var typeStart = at;
                at = SkipTypeUse(items, at);

                // The condition's operands are outside the block, so they are written before its label exists.
                var branches = at;

                while (branches < items.Count && items[branches].Head is not ("then" or "else"))
                {
                    branches++;
                }

                Sequence(items, at, branches);
                BlockType(list, items, typeStart, at, head);
                labels.Add(label);

                if (branches < items.Count && items[branches].Head is "then")
                {
                    var then = items[branches].Items!;
                    Sequence(then, 1, then.Count);
                    branches++;
                }

                if (branches < items.Count && items[branches].Head is "else")
                {
                    var otherwise = items[branches].Items!;

                    // An empty else is written as none, as the reference encoder writes it.
                    if (otherwise.Count > 1)
                    {
                        output.Add(0x05);
                        Sequence(otherwise, 1, otherwise.Count);
                    }

                    branches++;
                }

                if (branches != items.Count)
                {
                    throw new ScriptReadException("unexpected content in a folded if", list.Line);
                }

                labels.RemoveAt(labels.Count - 1);
                output.Add(0x0B);
                return;
            }

            default:
            {
                var operation = Lookup(items[0]);
                var immediates = new List<byte>();
                at = Immediates(list, operation.Immediate, operation.Alignment, items, 1, items.Count, immediates);
                Sequence(items, at, items.Count);
                output.Add(operation.Opcode);
                output.AddRange(immediates);
                return;
            }
        }
    }

    private int Plain(List<SExpr> items, int at, int to)
    {
        var operation = Lookup(items[at]);
        var immediates = new List<byte>();
        var next = Immediates(items[at], operation.Immediate, operation.Alignment, items, at + 1, to, immediates);
        output.Add(operation.Opcode);
        output.AddRange(immediates);
        return next;
    }

    /// <summary>Reads an instruction's immediates from <paramref name="at"/> and writes them to <paramref name="into"/>.</summary>
    private int Immediates(SExpr site, Immediate kind, byte alignment, List<SExpr> items, int at, int to, List<byte> into)
    {
        switch (kind)
        {
            case Immediate.None:
                return at;

            case Immediate.Label:
                Leb.U32(into, Label(Atom(items, at, to, site)));
                return at + 1;

            case Immediate.LabelTable:
            {
                var targets = new List<uint>();

                while (at < to && !items[at].IsList && (items[at].Kind is AtomKind.Id || ScriptNumbers.IsUnsigned(items[at].Text)))
                {
                    targets.Add(Label(items[at]));
                    at++;
                }

                if (targets.Count == 0)
                {
                    throw new ScriptReadException("br_table without a label", site.Line);
                }

                Leb.U32(into, (uint)(targets.Count - 1));

                foreach (var target in targets)
                {
                    Leb.U32(into, target);
                }

                return at;
            }

            case Immediate.Func:
                Leb.U32(into, module.FuncIndex(Atom(items, at, to, site)));
                return at + 1;

            case Immediate.TypeUse:
                Leb.U32(into, module.UseAt(site));
                into.Add(0x00);
                return SkipTypeUse(items, at, to);

            case Immediate.Local:
            {
                var atom = Atom(items, at, to, site);
                Leb.U32(into, atom.Kind is AtomKind.Id
                    ? locals.TryGetValue(atom.Text, out var index) ? index : throw new ScriptReadException($"unknown local {atom.Text}", atom.Line)
                    : ScriptNumbers.U32(atom.Text, atom.Line));
                return at + 1;
            }

            case Immediate.Global:
                Leb.U32(into, module.GlobalIndex(Atom(items, at, to, site)));
                return at + 1;

            case Immediate.Memory:
            {
                var offset = 0u;
                var align = 1u << alignment;

                if (at < to && !items[at].IsList && items[at].Text.StartsWith("offset=", StringComparison.Ordinal))
                {
                    offset = ScriptNumbers.U32(items[at].Text["offset=".Length..], items[at].Line);
                    at++;
                }

                if (at < to && !items[at].IsList && items[at].Text.StartsWith("align=", StringComparison.Ordinal))
                {
                    align = ScriptNumbers.U32(items[at].Text["align=".Length..], items[at].Line);
                    at++;
                }

                if (align == 0 || (align & (align - 1)) != 0)
                {
                    throw new ScriptReadException("an alignment that is not a power of two", site.Line);
                }

                Leb.U32(into, (uint)System.Numerics.BitOperations.Log2(align));
                Leb.U32(into, offset);
                return at;
            }

            case Immediate.MemoryIndex:
                into.Add(0x00);
                return at;

            case Immediate.I32:
            {
                var atom = Atom(items, at, to, site);
                Leb.S64(into, (int)ScriptNumbers.I32(atom.Text, atom.Line));
                return at + 1;
            }

            case Immediate.I64:
            {
                var atom = Atom(items, at, to, site);
                Leb.S64(into, (long)ScriptNumbers.I64(atom.Text, atom.Line));
                return at + 1;
            }

            case Immediate.F32:
            {
                var atom = Atom(items, at, to, site);
                into.AddRange(BitConverter.GetBytes(ScriptNumbers.F32(atom.Text, atom.Line)));
                return at + 1;
            }

            case Immediate.F64:
            {
                var atom = Atom(items, at, to, site);
                into.AddRange(BitConverter.GetBytes(ScriptNumbers.F64(atom.Text, atom.Line)));
                return at + 1;
            }

            default:
                throw new ScriptReadException("an unknown immediate kind", site.Line);
        }
    }

    /// <summary>Writes the block's opcode and its type, and answers where its body starts.</summary>
    private int BlockType(SExpr site, List<SExpr> items, int at, int to, string? keyword = null)
    {
        var opcode = (keyword ?? site.Text) switch
        {
            "block" => (byte)0x02,
            "loop" => (byte)0x03,
            _ => (byte)0x04,
        };

        output.Add(opcode);
        var end = SkipTypeUse(items, at, to);

        if (module.HasUse(site))
        {
            Leb.S64(output, module.UseAt(site));
            return end;
        }

        byte? result = null;

        for (var index = at; index < end; index++)
        {
            foreach (var type in items[index].Items!.Skip(1))
            {
                result = ModuleWriter.ValueType(type);
            }
        }

        output.Add(result ?? 0x40);
        return end;
    }

    private static int SkipTypeUse(List<SExpr> items, int at, int to = int.MaxValue)
    {
        while (at < Math.Min(to, items.Count) && items[at].Head is "type" or "param" or "result")
        {
            at++;
        }

        return at;
    }

    private uint Label(SExpr atom)
    {
        if (atom.Kind is not AtomKind.Id)
        {
            return ScriptNumbers.U32(atom.Text, atom.Line);
        }

        for (var index = labels.Count - 1; index >= 0; index--)
        {
            if (string.Equals(labels[index], atom.Text, StringComparison.Ordinal))
            {
                return (uint)(labels.Count - 1 - index);
            }
        }

        throw new ScriptReadException($"unknown label {atom.Text}", atom.Line);
    }

    private static string? OptionalId(List<SExpr> items, ref int at, int to)
    {
        if (at < to && !items[at].IsList && items[at].Kind is AtomKind.Id)
        {
            return items[at++].Text;
        }

        return null;
    }

    private static void SkipMatchingId(List<SExpr> items, ref int at, int to, string? label)
    {
        if (at < to && !items[at].IsList && items[at].Kind is AtomKind.Id)
        {
            if (!string.Equals(items[at].Text, label, StringComparison.Ordinal))
            {
                throw new ScriptReadException($"mismatching label {items[at].Text}", items[at].Line);
            }

            at++;
        }
    }

    private static SExpr Atom(List<SExpr> items, int at, int to, SExpr site) =>
        at < to && !items[at].IsList ? items[at] : throw new ScriptReadException($"{site.Head ?? site.Text} is missing an immediate", site.Line);

    private static (byte Opcode, Immediate Immediate, byte Alignment) Lookup(SExpr keyword) =>
        !keyword.IsList && Opcodes.TryGetValue(keyword.Text, out var operation)
            ? operation
            : throw new ScriptReadException($"unknown operator {keyword.Text}", keyword.Line);

    private static Dictionary<string, (byte, Immediate, byte)> Build()
    {
        var table = new Dictionary<string, (byte, Immediate, byte)>(StringComparer.Ordinal)
        {
            ["unreachable"] = (0x00, Immediate.None, 0),
            ["nop"] = (0x01, Immediate.None, 0),
            ["br"] = (0x0C, Immediate.Label, 0),
            ["br_if"] = (0x0D, Immediate.Label, 0),
            ["br_table"] = (0x0E, Immediate.LabelTable, 0),
            ["return"] = (0x0F, Immediate.None, 0),
            ["call"] = (0x10, Immediate.Func, 0),
            ["call_indirect"] = (0x11, Immediate.TypeUse, 0),
            ["drop"] = (0x1A, Immediate.None, 0),
            ["select"] = (0x1B, Immediate.None, 0),
            ["local.get"] = (0x20, Immediate.Local, 0),
            ["local.set"] = (0x21, Immediate.Local, 0),
            ["local.tee"] = (0x22, Immediate.Local, 0),
            ["global.get"] = (0x23, Immediate.Global, 0),
            ["global.set"] = (0x24, Immediate.Global, 0),
            ["memory.size"] = (0x3F, Immediate.MemoryIndex, 0),
            ["memory.grow"] = (0x40, Immediate.MemoryIndex, 0),
            ["i32.const"] = (0x41, Immediate.I32, 0),
            ["i64.const"] = (0x42, Immediate.I64, 0),
            ["f32.const"] = (0x43, Immediate.F32, 0),
            ["f64.const"] = (0x44, Immediate.F64, 0),
        };

        (string Name, byte Alignment)[] memory =
        [
            ("i32.load", 2), ("i64.load", 3), ("f32.load", 2), ("f64.load", 3),
            ("i32.load8_s", 0), ("i32.load8_u", 0), ("i32.load16_s", 1), ("i32.load16_u", 1),
            ("i64.load8_s", 0), ("i64.load8_u", 0), ("i64.load16_s", 1), ("i64.load16_u", 1),
            ("i64.load32_s", 2), ("i64.load32_u", 2),
            ("i32.store", 2), ("i64.store", 3), ("f32.store", 2), ("f64.store", 3),
            ("i32.store8", 0), ("i32.store16", 1), ("i64.store8", 0), ("i64.store16", 1), ("i64.store32", 2),
        ];

        for (var index = 0; index < memory.Length; index++)
        {
            table[memory[index].Name] = ((byte)(0x28 + index), Immediate.Memory, memory[index].Alignment);
        }

        string[] numeric =
        [
            "i32.eqz", "i32.eq", "i32.ne", "i32.lt_s", "i32.lt_u", "i32.gt_s", "i32.gt_u", "i32.le_s", "i32.le_u", "i32.ge_s", "i32.ge_u",
            "i64.eqz", "i64.eq", "i64.ne", "i64.lt_s", "i64.lt_u", "i64.gt_s", "i64.gt_u", "i64.le_s", "i64.le_u", "i64.ge_s", "i64.ge_u",
            "f32.eq", "f32.ne", "f32.lt", "f32.gt", "f32.le", "f32.ge",
            "f64.eq", "f64.ne", "f64.lt", "f64.gt", "f64.le", "f64.ge",
            "i32.clz", "i32.ctz", "i32.popcnt", "i32.add", "i32.sub", "i32.mul", "i32.div_s", "i32.div_u", "i32.rem_s", "i32.rem_u",
            "i32.and", "i32.or", "i32.xor", "i32.shl", "i32.shr_s", "i32.shr_u", "i32.rotl", "i32.rotr",
            "i64.clz", "i64.ctz", "i64.popcnt", "i64.add", "i64.sub", "i64.mul", "i64.div_s", "i64.div_u", "i64.rem_s", "i64.rem_u",
            "i64.and", "i64.or", "i64.xor", "i64.shl", "i64.shr_s", "i64.shr_u", "i64.rotl", "i64.rotr",
            "f32.abs", "f32.neg", "f32.ceil", "f32.floor", "f32.trunc", "f32.nearest", "f32.sqrt",
            "f32.add", "f32.sub", "f32.mul", "f32.div", "f32.min", "f32.max", "f32.copysign",
            "f64.abs", "f64.neg", "f64.ceil", "f64.floor", "f64.trunc", "f64.nearest", "f64.sqrt",
            "f64.add", "f64.sub", "f64.mul", "f64.div", "f64.min", "f64.max", "f64.copysign",
            "i32.wrap_i64", "i32.trunc_f32_s", "i32.trunc_f32_u", "i32.trunc_f64_s", "i32.trunc_f64_u",
            "i64.extend_i32_s", "i64.extend_i32_u", "i64.trunc_f32_s", "i64.trunc_f32_u", "i64.trunc_f64_s", "i64.trunc_f64_u",
            "f32.convert_i32_s", "f32.convert_i32_u", "f32.convert_i64_s", "f32.convert_i64_u", "f32.demote_f64",
            "f64.convert_i32_s", "f64.convert_i32_u", "f64.convert_i64_s", "f64.convert_i64_u", "f64.promote_f32",
            "i32.reinterpret_f32", "i64.reinterpret_f64", "f32.reinterpret_i32", "f64.reinterpret_i64",
        ];

        for (var index = 0; index < numeric.Length; index++)
        {
            table[numeric[index]] = ((byte)(0x45 + index), Immediate.None, 0);
        }

        return table;
    }
}
