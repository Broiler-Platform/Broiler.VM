using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// Encodes a module written in the specification's text format, version 1.0, to the binary format.
/// </summary>
/// <remarks>
/// <para>
/// <b>The binary it writes is the one the reference tools write</b>, choice for choice, so that
/// every module it encodes can be compared byte for byte with an independent encoder's. There are
/// three such choices:
/// <list type="bullet">
/// <item>every integer is the shortest encoding of its value;</item>
/// <item>a function type the text implies but never declares is appended after every declared
/// one, in the order the module's fields, and within a function its instructions, first need it;</item>
/// <item>consecutive locals of one type share a declaration.</item>
/// </list>
/// Nothing is written that the text does not say: no name section and no empty section.
/// </para>
/// <para>
/// <b>It encodes; it does not validate.</b> A module the suite calls invalid - a branch to a label
/// that does not exist, a type the stack does not have, a second memory - is written as the text
/// says, so that the profile's validator is what refuses it. Only what the text format itself
/// cannot say is refused here, as a <see cref="ScriptReadException"/>.
/// </para>
/// </remarks>
internal static class TextModule
{
    /// <summary>Encodes the <c>(module ...)</c> list <paramref name="module"/>.</summary>
    internal static byte[] Encode(SExpr module) => new ModuleWriter(module).Encode();
}

/// <summary>A function signature: parameter and result types as their binary bytes.</summary>
internal sealed record FuncSignature(byte[] Params, byte[] Results)
{
    public bool Equals(FuncSignature? other) =>
        other is not null && Params.AsSpan().SequenceEqual(other.Params) && Results.AsSpan().SequenceEqual(other.Results);

    public override int GetHashCode() => HashCode.Combine(Params.Length, Results.Length);
}

/// <summary>One module's fields, collected, resolved and written.</summary>
internal sealed class ModuleWriter
{
    private const byte FuncRef = 0x70;

    private readonly SExpr module;
    private readonly List<FuncSignature> types = [];
    private readonly Dictionary<string, uint> typeIds = new(StringComparer.Ordinal);
    private readonly List<Import> imports = [];
    private readonly List<Func> funcs = [];
    private readonly List<Table> tables = [];
    private readonly List<Memory> memories = [];
    private readonly List<Global> globals = [];
    private readonly List<Export> exports = [];
    private readonly List<ElemSegment> elems = [];
    private readonly List<DataSegment> datas = [];
    private readonly Dictionary<SExpr, uint> resolvedUses = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<string, uint>[] ids = [new(StringComparer.Ordinal), new(StringComparer.Ordinal), new(StringComparer.Ordinal), new(StringComparer.Ordinal)];
    private SExpr? start;

    internal ModuleWriter(SExpr module) => this.module = module;

    private enum Space
    {
        Func = 0,
        Table = 1,
        Memory = 2,
        Global = 3,
    }

    internal byte[] Encode()
    {
        var items = module.Items!;
        var index = 1;

        if (index < items.Count && !items[index].IsList && items[index].Kind is AtomKind.Id)
        {
            index++;
        }

        // Declared types first, wherever they stand, because a use may name one declared later.
        for (var at = index; at < items.Count; at++)
        {
            if (items[at].Head is "type")
            {
                DeclareType(items[at]);
            }
        }

        for (; index < items.Count; index++)
        {
            var field = items[index];

            if (!field.IsList)
            {
                throw new ScriptReadException($"a module field that is not a list: {field.Text}", field.Line);
            }

            switch (field.Head)
            {
                case "type": break;
                case "import": ReadImport(field); break;
                case "func": ReadFunc(field); break;
                case "table": ReadTable(field); break;
                case "memory": ReadMemory(field); break;
                case "global": ReadGlobal(field); break;
                case "export": ReadExport(field); break;
                case "start": start = Required(field, 1); break;
                case "elem": ReadElem(field); break;
                case "data": ReadData(field); break;
                default: throw new ScriptReadException($"unknown module field {field.Head ?? "(list)"}", field.Line);
            }
        }

        AssignIndices(funcs, Space.Func);
        AssignIndices(tables, Space.Table);
        AssignIndices(memories, Space.Memory);
        AssignIndices(globals, Space.Global);
        ResolveTypes();

        return Write();
    }

    // =============================================================================================
    // Reading the fields
    // =============================================================================================

    private void DeclareType(SExpr field)
    {
        var items = field.Items!;
        var at = 1;
        var id = OptionalId(items, ref at);
        var func = Required(field, at);

        if (func.Head is not "func")
        {
            throw new ScriptReadException("a type field that is not a function type", field.Line);
        }

        var use = ReadTypeUse(func.Items!, 1, out var end);

        if (end != func.Items!.Count)
        {
            throw new ScriptReadException("unexpected content in a function type", func.Line);
        }

        if (id is not null)
        {
            typeIds[id] = (uint)types.Count;
        }

        types.Add(use.Signature);
    }

    private void ReadImport(SExpr field)
    {
        var items = field.Items!;
        var moduleName = StringAt(field, 1);
        var name = StringAt(field, 2);
        var description = Required(field, 3);
        var inner = description.Items!;
        var at = 1;
        var id = OptionalId(inner, ref at);
        var source = new ImportSource(moduleName, name);

        if (items.Count != 4)
        {
            throw new ScriptReadException("unexpected content in an import", field.Line);
        }

        switch (description.Head)
        {
            case "func":
                var use = ReadTypeUse(inner, at, out var end);
                Expect(inner, end, description);
                funcs.Add(new Func(id, source, use, [], [], []) { Field = field });
                break;
            case "table":
                tables.Add(ReadTableType(inner, at, description, id, source, []));
                break;
            case "memory":
                memories.Add(new Memory(id, source, ReadLimits(inner, ref at, description), null, []));
                Expect(inner, at, description);
                break;
            case "global":
                var (type, mutable) = ReadGlobalType(inner, ref at, description);
                Expect(inner, at, description);
                globals.Add(new Global(id, source, type, mutable, [], []));
                break;
            default:
                throw new ScriptReadException($"unknown import kind {description.Head}", description.Line);
        }

        imports.Add(new Import(moduleName, name, description.Head!, description.Head switch
        {
            "func" => funcs[^1],
            "table" => tables[^1],
            "memory" => memories[^1],
            _ => globals[^1],
        }));
    }

    private void ReadFunc(SExpr field)
    {
        var items = field.Items!;
        var at = 1;
        var id = OptionalId(items, ref at);
        var inlineExports = InlineExports(items, ref at);
        var source = InlineImport(items, ref at);
        var use = ReadTypeUse(items, at, out at);
        var locals = new List<(string? Id, byte Type)>();

        while (at < items.Count && items[at].Head is "local")
        {
            ReadNamedTypes(items[at], locals);
            at++;
        }

        var func = new Func(id, source, use, locals, items.GetRange(at, items.Count - at), inlineExports) { Field = field };

        if (source is not null)
        {
            if (locals.Count != 0 || func.Body.Count != 0)
            {
                throw new ScriptReadException("an imported function with a body", field.Line);
            }

            imports.Add(new Import(source.Module, source.Name, "func", func));
        }

        funcs.Add(func);
        AddInlineExports(inlineExports, "func", func);
    }

    private void ReadTable(SExpr field)
    {
        var items = field.Items!;
        var at = 1;
        var id = OptionalId(items, ref at);
        var inlineExports = InlineExports(items, ref at);
        var source = InlineImport(items, ref at);

        // The abbreviation: an element type followed by an element list, the table sized to fit.
        if (at < items.Count && !items[at].IsList && items[at].IsWord("funcref") &&
            at + 1 < items.Count && items[at + 1].Head is "elem")
        {
            var elements = items[at + 1].Items!.GetRange(1, items[at + 1].Items!.Count - 1);
            var count = (uint)elements.Count;
            var table = new Table(id, null, count, count, inlineExports);
            tables.Add(table);
            elems.Add(new ElemSegment(null, [SExpr.List(field.Line, [SExpr.Atom(field.Line, AtomKind.Word, "i32.const"), SExpr.Atom(field.Line, AtomKind.Word, "0")])], elements, table));
            AddInlineExports(inlineExports, "table", table);
            Expect(items, at + 2, field);
            return;
        }

        var declared = ReadTableType(items, at, field, id, source, inlineExports);
        tables.Add(declared);

        if (source is not null)
        {
            imports.Add(new Import(source.Module, source.Name, "table", declared));
        }

        AddInlineExports(inlineExports, "table", declared);
    }

    private void ReadMemory(SExpr field)
    {
        var items = field.Items!;
        var at = 1;
        var id = OptionalId(items, ref at);
        var inlineExports = InlineExports(items, ref at);
        var source = InlineImport(items, ref at);

        // The abbreviation: a data list, the memory sized in whole pages to fit it.
        if (at < items.Count && items[at].Head is "data")
        {
            var bytes = Strings(items[at].Items!, 1, items[at]);
            var pages = (uint)((bytes.Length + 65_535) / 65_536);
            var memory = new Memory(id, null, (pages, pages), null, inlineExports);
            memories.Add(memory);
            datas.Add(new DataSegment(null, [SExpr.List(field.Line, [SExpr.Atom(field.Line, AtomKind.Word, "i32.const"), SExpr.Atom(field.Line, AtomKind.Word, "0")])], bytes, memory));
            AddInlineExports(inlineExports, "memory", memory);
            Expect(items, at + 1, field);
            return;
        }

        var limits = ReadLimits(items, ref at, field);
        Expect(items, at, field);
        var declared = new Memory(id, source, limits, null, inlineExports);
        memories.Add(declared);

        if (source is not null)
        {
            imports.Add(new Import(source.Module, source.Name, "memory", declared));
        }

        AddInlineExports(inlineExports, "memory", declared);
    }

    private void ReadGlobal(SExpr field)
    {
        var items = field.Items!;
        var at = 1;
        var id = OptionalId(items, ref at);
        var inlineExports = InlineExports(items, ref at);
        var source = InlineImport(items, ref at);
        var (type, mutable) = ReadGlobalType(items, ref at, field);
        var global = new Global(id, source, type, mutable, items.GetRange(at, items.Count - at), inlineExports);

        if (source is not null)
        {
            if (global.Init.Count != 0)
            {
                throw new ScriptReadException("an imported global with an initialiser", field.Line);
            }

            imports.Add(new Import(source.Module, source.Name, "global", global));
        }

        globals.Add(global);
        AddInlineExports(inlineExports, "global", global);
    }

    private void ReadExport(SExpr field)
    {
        var name = StringAt(field, 1);
        var description = Required(field, 2);
        Expect(field.Items!, 3, field);

        if (description.Head is not ("func" or "table" or "memory" or "global") || description.Items!.Count != 2)
        {
            throw new ScriptReadException("a malformed export description", description.Line);
        }

        exports.Add(new Export(name, description.Head!, null, description.Items[1]));
    }

    private void ReadElem(SExpr field)
    {
        var items = field.Items!;
        var at = 1;
        SExpr? table = null;

        if (at < items.Count && !items[at].IsList)
        {
            table = items[at++];
        }

        var offset = Offset(items, ref at, field);
        elems.Add(new ElemSegment(table, offset, items.GetRange(at, items.Count - at), null));
    }

    private void ReadData(SExpr field)
    {
        var items = field.Items!;
        var at = 1;
        SExpr? memory = null;

        if (at < items.Count && !items[at].IsList && items[at].Kind is not AtomKind.String)
        {
            memory = items[at++];
        }

        var offset = Offset(items, ref at, field);
        datas.Add(new DataSegment(memory, offset, Strings(items, at, field), null));
    }

    private static List<SExpr> Offset(List<SExpr> items, ref int at, SExpr field)
    {
        if (at >= items.Count || !items[at].IsList)
        {
            throw new ScriptReadException("a segment without an offset", field.Line);
        }

        var offset = items[at++];
        return offset.Head is "offset" ? offset.Items!.GetRange(1, offset.Items.Count - 1) : [offset];
    }

    private static Table ReadTableType(List<SExpr> items, int at, SExpr field, string? id, ImportSource? source, List<byte[]> inlineExports)
    {
        var limits = ReadLimits(items, ref at, field);

        if (at >= items.Count || !items[at].IsWord("funcref"))
        {
            throw new ScriptReadException("a table without the element type funcref", field.Line);
        }

        Expect(items, at + 1, field);
        return new Table(id, source, limits.Min, limits.Max, inlineExports);
    }

    private static (uint Min, uint? Max) ReadLimits(List<SExpr> items, ref int at, SExpr field)
    {
        if (at >= items.Count || items[at].IsList || !ScriptNumbers.IsUnsigned(items[at].Text))
        {
            throw new ScriptReadException("limits without a minimum", field.Line);
        }

        var minimum = ScriptNumbers.U32(items[at].Text, items[at].Line);
        at++;
        uint? maximum = null;

        if (at < items.Count && !items[at].IsList && ScriptNumbers.IsUnsigned(items[at].Text))
        {
            maximum = ScriptNumbers.U32(items[at].Text, items[at].Line);
            at++;
        }

        return (minimum, maximum);
    }

    private static (byte Type, bool Mutable) ReadGlobalType(List<SExpr> items, ref int at, SExpr field)
    {
        if (at >= items.Count)
        {
            throw new ScriptReadException("a global without a type", field.Line);
        }

        var type = items[at++];

        if (type.Head is "mut")
        {
            if (type.Items!.Count != 2)
            {
                throw new ScriptReadException("a malformed mutable global type", type.Line);
            }

            return (ValueType(type.Items[1]), true);
        }

        return (ValueType(type), false);
    }

    private static List<byte[]> InlineExports(List<SExpr> items, ref int at)
    {
        var names = new List<byte[]>();

        while (at < items.Count && items[at].Head is "export")
        {
            names.Add(StringAt(items[at], 1));
            Expect(items[at].Items!, 2, items[at]);
            at++;
        }

        return names;
    }

    private static ImportSource? InlineImport(List<SExpr> items, ref int at)
    {
        if (at >= items.Count || items[at].Head is not "import")
        {
            return null;
        }

        var source = new ImportSource(StringAt(items[at], 1), StringAt(items[at], 2));
        Expect(items[at].Items!, 3, items[at]);
        at++;
        return source;
    }

    private void AddInlineExports(List<byte[]> names, string kind, object entity)
    {
        foreach (var name in names)
        {
            exports.Add(new Export(name, kind, entity, null));
        }
    }

    /// <summary>Reads <c>(type x)? (param ...)* (result ...)*</c> from <paramref name="at"/>.</summary>
    private TypeUse ReadTypeUse(List<SExpr> items, int at, out int end)
    {
        SExpr? reference = null;

        if (at < items.Count && items[at].Head is "type")
        {
            reference = Required(items[at], 1);
            Expect(items[at].Items!, 2, items[at]);
            at++;
        }

        var parameters = new List<(string? Id, byte Type)>();
        var results = new List<byte>();
        var inline = false;

        while (at < items.Count && items[at].Head is "param")
        {
            ReadNamedTypes(items[at], parameters);
            inline = true;
            at++;
        }

        while (at < items.Count && items[at].Head is "result")
        {
            foreach (var type in items[at].Items!.Skip(1))
            {
                results.Add(ValueType(type));
            }

            inline = true;
            at++;
        }

        end = at;
        return new TypeUse(reference, parameters, new FuncSignature([.. parameters.Select(static p => p.Type)], [.. results]), inline);
    }

    /// <summary>Reads <c>(param $x t)</c> or <c>(param t*)</c>, and the same for locals.</summary>
    private static void ReadNamedTypes(SExpr list, List<(string? Id, byte Type)> into)
    {
        var items = list.Items!;

        if (items.Count > 1 && !items[1].IsList && items[1].Kind is AtomKind.Id)
        {
            if (items.Count != 3)
            {
                throw new ScriptReadException($"a named {list.Head} with other than one type", list.Line);
            }

            into.Add((items[1].Text, ValueType(items[2])));
            return;
        }

        foreach (var type in items.Skip(1))
        {
            into.Add((null, ValueType(type)));
        }
    }

    internal static byte ValueType(SExpr atom) => atom.IsList ? throw new ScriptReadException("a list where a value type belongs", atom.Line) : atom.Text switch
    {
        "i32" => (byte)0x7F,
        "i64" => (byte)0x7E,
        "f32" => (byte)0x7D,
        "f64" => (byte)0x7C,
        _ => throw new ScriptReadException($"unknown value type {atom.Text}", atom.Line),
    };

    // =============================================================================================
    // Indices and types
    // =============================================================================================

    /// <summary>Numbers a space imports first, then definitions, each in the order they were written.</summary>
    private void AssignIndices<T>(List<T> entities, Space space)
        where T : Entity
    {
        var ordered = entities.Where(static e => e.Source is not null).Concat(entities.Where(static e => e.Source is null)).ToList();
        entities.Clear();
        entities.AddRange(ordered);

        for (var index = 0; index < entities.Count; index++)
        {
            entities[index].Index = (uint)index;

            if (entities[index].Id is { } id && !ids[(int)space].TryAdd(id, (uint)index))
            {
                throw new ScriptReadException($"duplicate identifier {id}", module.Line);
            }
        }
    }

    /// <summary>
    /// Gives every function, and every indirect call and block that needs one, its type index - adding
    /// a type where the text implies one no declaration matches, in the order the fields and their
    /// instructions need it.
    /// </summary>
    private void ResolveTypes()
    {
        var items = module.Items!;

        foreach (var field in items)
        {
            if (field.Head is not ("import" or "func") ||
                funcs.FirstOrDefault(f => ReferenceEquals(f.Field, field)) is not { } func)
            {
                continue;
            }

            func.TypeIndex = Resolve(func.Use);

            if (func.Source is null)
            {
                ResolveBody(func.Body);
            }
        }
    }

    private uint Resolve(TypeUse use)
    {
        if (use.Reference is { } reference)
        {
            return reference.Kind is AtomKind.Id
                ? typeIds.TryGetValue(reference.Text, out var named) ? named : throw new ScriptReadException($"unknown type {reference.Text}", reference.Line)
                : ScriptNumbers.U32(reference.Text, reference.Line);
        }

        var found = types.IndexOf(use.Signature);

        if (found >= 0)
        {
            return (uint)found;
        }

        types.Add(use.Signature);
        return (uint)(types.Count - 1);
    }

    /// <summary>Walks a body in text order for the indirect calls and blocks that need a type index.</summary>
    private void ResolveBody(List<SExpr> items)
    {
        for (var at = 0; at < items.Count; at++)
        {
            var item = items[at];

            if (item.IsList)
            {
                ResolveList(item);
                continue;
            }

            if (item.IsWord("call_indirect"))
            {
                resolvedUses[item] = Resolve(ReadTypeUse(items, at + 1, out _));
            }
            else if (item.IsWord("block") || item.IsWord("loop") || item.IsWord("if"))
            {
                var next = at + 1;

                if (next < items.Count && !items[next].IsList && items[next].Kind is AtomKind.Id)
                {
                    next++;
                }

                ResolveBlockType(item, items, next);
            }
        }
    }

    private void ResolveList(SExpr list)
    {
        var items = list.Items!;

        switch (list.Head)
        {
            case "call_indirect":
                resolvedUses[list] = Resolve(ReadTypeUse(items, 1, out _));
                ResolveBody(items.GetRange(1, items.Count - 1));
                return;
            case "block" or "loop" or "if":
                var next = 1;

                if (next < items.Count && !items[next].IsList && items[next].Kind is AtomKind.Id)
                {
                    next++;
                }

                ResolveBlockType(list, items, next);
                break;
        }

        ResolveBody(items.GetRange(1, items.Count - 1));
    }

    private void ResolveBlockType(SExpr site, List<SExpr> items, int at)
    {
        var use = ReadTypeUse(items, at, out _);

        if (use.Reference is not null || use.Signature.Params.Length != 0 || use.Signature.Results.Length > 1)
        {
            resolvedUses[site] = Resolve(use);
        }
    }

    // =============================================================================================
    // Writing
    // =============================================================================================

    private byte[] Write()
    {
        var output = new List<byte> { 0x00, 0x61, 0x73, 0x6D, 0x01, 0x00, 0x00, 0x00 };

        Section(output, 1, types.Count, body =>
        {
            foreach (var type in types)
            {
                body.Add(0x60);
                Leb.U32(body, (uint)type.Params.Length);
                body.AddRange(type.Params);
                Leb.U32(body, (uint)type.Results.Length);
                body.AddRange(type.Results);
            }
        });

        Section(output, 2, imports.Count, body =>
        {
            foreach (var import in imports)
            {
                Name(body, import.Module);
                Name(body, import.Name);

                switch (import.Entity)
                {
                    case Func func:
                        body.Add(0x00);
                        Leb.U32(body, func.TypeIndex);
                        break;
                    case Table table:
                        body.Add(0x01);
                        body.Add(FuncRef);
                        Limits(body, table.Min, table.Max);
                        break;
                    case Memory memory:
                        body.Add(0x02);
                        Limits(body, memory.Limits.Min, memory.Limits.Max);
                        break;
                    case Global global:
                        body.Add(0x03);
                        body.Add(global.Type);
                        body.Add(global.Mutable ? (byte)1 : (byte)0);
                        break;
                }
            }
        });

        var defined = funcs.Where(static f => f.Source is null).ToList();

        Section(output, 3, defined.Count, body =>
        {
            foreach (var func in defined)
            {
                Leb.U32(body, func.TypeIndex);
            }
        });

        var definedTables = tables.Where(static t => t.Source is null).ToList();

        Section(output, 4, definedTables.Count, body =>
        {
            foreach (var table in definedTables)
            {
                body.Add(FuncRef);
                Limits(body, table.Min, table.Max);
            }
        });

        var definedMemories = memories.Where(static m => m.Source is null).ToList();

        Section(output, 5, definedMemories.Count, body =>
        {
            foreach (var memory in definedMemories)
            {
                Limits(body, memory.Limits.Min, memory.Limits.Max);
            }
        });

        var definedGlobals = globals.Where(static g => g.Source is null).ToList();

        Section(output, 6, definedGlobals.Count, body =>
        {
            foreach (var global in definedGlobals)
            {
                body.Add(global.Type);
                body.Add(global.Mutable ? (byte)1 : (byte)0);
                Expression(body, global.Init, []);
            }
        });

        Section(output, 7, exports.Count, body =>
        {
            foreach (var export in exports)
            {
                Name(body, export.Name);
                body.Add(export.Kind switch { "func" => (byte)0, "table" => (byte)1, "memory" => (byte)2, _ => (byte)3 });
                Leb.U32(body, export.Entity is Entity entity ? entity.Index : Index(export.Reference!, SpaceOf(export.Kind)));
            }
        });

        if (start is not null)
        {
            var body = new List<byte>();
            Leb.U32(body, Index(start, Space.Func));
            output.Add(8);
            Leb.U32(output, (uint)body.Count);
            output.AddRange(body);
        }

        Section(output, 9, elems.Count, body =>
        {
            foreach (var segment in elems)
            {
                Leb.U32(body, segment.Table is { } table ? Index(table, Space.Table) : segment.Host?.Index ?? 0);
                Expression(body, segment.Offset, []);
                Leb.U32(body, (uint)segment.Funcs.Count);

                foreach (var func in segment.Funcs)
                {
                    Leb.U32(body, Index(func, Space.Func));
                }
            }
        });

        Section(output, 10, defined.Count, body =>
        {
            foreach (var func in defined)
            {
                var code = new List<byte>();
                var locals = func.Locals;
                var runs = new List<(uint Count, byte Type)>();

                foreach (var (_, type) in locals)
                {
                    if (runs.Count > 0 && runs[^1].Type == type)
                    {
                        runs[^1] = (runs[^1].Count + 1, type);
                    }
                    else
                    {
                        runs.Add((1, type));
                    }
                }

                Leb.U32(code, (uint)runs.Count);

                foreach (var (count, type) in runs)
                {
                    Leb.U32(code, count);
                    code.Add(type);
                }

                Expression(code, func.Body, LocalIds(func));
                Leb.U32(body, (uint)code.Count);
                body.AddRange(code);
            }
        });

        Section(output, 11, datas.Count, body =>
        {
            foreach (var segment in datas)
            {
                Leb.U32(body, segment.Memory is { } memory ? Index(memory, Space.Memory) : segment.Host?.Index ?? 0);
                Expression(body, segment.Offset, []);
                Leb.U32(body, (uint)segment.Bytes.Length);
                body.AddRange(segment.Bytes);
            }
        });

        return [.. output];
    }

    private Dictionary<string, uint> LocalIds(Func func)
    {
        var map = new Dictionary<string, uint>(StringComparer.Ordinal);
        var index = 0u;
        var parameters = func.Use.Inline ? func.Use.Parameters : [];

        if (!func.Use.Inline && func.TypeIndex < types.Count)
        {
            index = (uint)types[(int)func.TypeIndex].Params.Length;
        }

        foreach (var (id, _) in parameters.Concat(func.Locals))
        {
            if (id is not null && !map.TryAdd(id, index))
            {
                throw new ScriptReadException($"duplicate local {id}", module.Line);
            }

            index++;
        }

        return map;
    }

    private void Expression(List<byte> output, List<SExpr> items, Dictionary<string, uint> locals)
    {
        new BodyWriter(this, output, locals).Sequence(items, 0, items.Count);
        output.Add(0x0B);
    }

    private static void Section(List<byte> output, byte id, int count, Action<List<byte>> write)
    {
        if (count == 0)
        {
            return;
        }

        var body = new List<byte>();
        Leb.U32(body, (uint)count);
        write(body);
        output.Add(id);
        Leb.U32(output, (uint)body.Count);
        output.AddRange(body);
    }

    private static void Limits(List<byte> output, uint min, uint? max)
    {
        output.Add(max is null ? (byte)0 : (byte)1);
        Leb.U32(output, min);

        if (max is { } bound)
        {
            Leb.U32(output, bound);
        }
    }

    private static void Name(List<byte> output, byte[] name)
    {
        Leb.U32(output, (uint)name.Length);
        output.AddRange(name);
    }

    private static Space SpaceOf(string kind) => kind switch
    {
        "func" => Space.Func,
        "table" => Space.Table,
        "memory" => Space.Memory,
        _ => Space.Global,
    };

    internal uint FuncIndex(SExpr atom) => Index(atom, Space.Func);

    internal uint GlobalIndex(SExpr atom) => Index(atom, Space.Global);

    internal uint UseAt(SExpr site) =>
        resolvedUses.TryGetValue(site, out var index) ? index : throw new ScriptReadException("an unresolved type use", site.Line);

    internal bool HasUse(SExpr site) => resolvedUses.ContainsKey(site);

    private uint Index(SExpr atom, Space space)
    {
        if (atom.IsList)
        {
            throw new ScriptReadException("a list where an index belongs", atom.Line);
        }

        if (atom.Kind is AtomKind.Id)
        {
            return ids[(int)space].TryGetValue(atom.Text, out var index)
                ? index
                : throw new ScriptReadException($"unknown {space.ToString().ToLowerInvariant()} {atom.Text}", atom.Line);
        }

        return ScriptNumbers.U32(atom.Text, atom.Line);
    }

    private static SExpr Required(SExpr list, int at) =>
        at < list.Items!.Count ? list.Items[at] : throw new ScriptReadException($"{list.Head} is missing a part", list.Line);

    private static byte[] StringAt(SExpr list, int at)
    {
        var item = Required(list, at);
        return !item.IsList && item.Kind is AtomKind.String ? item.Bytes! : throw new ScriptReadException("a string was expected", item.Line);
    }

    internal static byte[] Strings(List<SExpr> items, int at, SExpr owner)
    {
        var bytes = new List<byte>();

        for (; at < items.Count; at++)
        {
            if (items[at].IsList || items[at].Kind is not AtomKind.String)
            {
                throw new ScriptReadException("a string was expected", owner.Line);
            }

            bytes.AddRange(items[at].Bytes!);
        }

        return [.. bytes];
    }

    private static string? OptionalId(List<SExpr> items, ref int at)
    {
        if (at < items.Count && !items[at].IsList && items[at].Kind is AtomKind.Id)
        {
            return items[at++].Text;
        }

        return null;
    }

    private static void Expect(List<SExpr> items, int end, SExpr owner)
    {
        if (end != items.Count)
        {
            throw new ScriptReadException($"unexpected content in {owner.Head}", owner.Line);
        }
    }

    // =============================================================================================
    // The module's entities
    // =============================================================================================

    internal sealed record ImportSource(byte[] Module, byte[] Name);

    internal sealed record TypeUse(SExpr? Reference, List<(string? Id, byte Type)> Parameters, FuncSignature Signature, bool Inline);

    private sealed record Import(byte[] Module, byte[] Name, string Kind, Entity Entity);

    private sealed record Export(byte[] Name, string Kind, object? Entity, SExpr? Reference);

    private sealed record ElemSegment(SExpr? Table, List<SExpr> Offset, List<SExpr> Funcs, Table? Host);

    private sealed record DataSegment(SExpr? Memory, List<SExpr> Offset, byte[] Bytes, Memory? Host);

    internal abstract class Entity(string? id, ImportSource? source)
    {
        internal string? Id { get; } = id;

        internal ImportSource? Source { get; } = source;

        internal uint Index { get; set; }
    }

    private sealed class Func(string? id, ImportSource? source, TypeUse use, List<(string? Id, byte Type)> locals, List<SExpr> body, List<byte[]> exports)
        : Entity(id, source)
    {
        internal TypeUse Use { get; } = use;

        internal List<(string? Id, byte Type)> Locals { get; } = locals;

        internal List<SExpr> Body { get; } = body;

        internal List<byte[]> Exports { get; } = exports;

        internal uint TypeIndex { get; set; }

        /// <summary>The field the function was written in, which is how its type is resolved in field order.</summary>
        internal SExpr? Field { get; init; }
    }

    private sealed class Table(string? id, ImportSource? source, uint min, uint? max, List<byte[]> exports)
        : Entity(id, source)
    {
        internal uint Min { get; } = min;

        internal uint? Max { get; } = max;

        internal List<byte[]> Exports { get; } = exports;
    }

    private sealed class Memory(string? id, ImportSource? source, (uint Min, uint? Max) limits, byte[]? data, List<byte[]> exports)
        : Entity(id, source)
    {
        internal (uint Min, uint? Max) Limits { get; } = limits;

        internal byte[]? Data { get; } = data;

        internal List<byte[]> Exports { get; } = exports;
    }

    private sealed class Global(string? id, ImportSource? source, byte type, bool mutable, List<SExpr> init, List<byte[]> exports)
        : Entity(id, source)
    {
        internal byte Type { get; } = type;

        internal bool Mutable { get; } = mutable;

        internal List<SExpr> Init { get; } = init;

        internal List<byte[]> Exports { get; } = exports;
    }
}

/// <summary>Minimal LEB128 encodings.</summary>
internal static class Leb
{
    internal static void U32(List<byte> output, uint value)
    {
        do
        {
            var next = (byte)(value & 0x7F);
            value >>= 7;
            output.Add(value == 0 ? next : (byte)(next | 0x80));
        }
        while (value != 0);
    }

    internal static void S64(List<byte> output, long value)
    {
        while (true)
        {
            var next = (byte)(value & 0x7F);
            value >>= 7;

            if ((value == 0 && (next & 0x40) == 0) || (value == -1 && (next & 0x40) != 0))
            {
                output.Add(next);
                return;
            }

            output.Add((byte)(next | 0x80));
        }
    }
}
