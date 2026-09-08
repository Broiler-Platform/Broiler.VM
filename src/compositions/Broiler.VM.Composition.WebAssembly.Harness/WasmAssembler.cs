namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// A tiny WebAssembly binary assembler: enough of the format to hand the profile real modules.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT LIVES IN A HARNESS ROOT AND MUST.</b> Rule A11 forbids a project outside
/// <c>src/compositions/</c> to reference a profile assembly, so an encoder that produces the bytes a
/// verifier is asked to accept or refuse cannot be a test project. It is also never advertised and
/// never packable, because an encoder must appear in no shipped image.
/// </para>
/// <para>
/// <b>It is an encoder and not a text-format reader.</b> It builds a module out of typed pieces a
/// caller names in C#; it does not read the specification's text format, and calling it an assembler
/// should not be read as the script reader the conformance milestone owes. That reader does not
/// exist.
/// </para>
/// </remarks>
internal sealed class WasmAssembler
{
    internal const byte I32 = 0x7F;
    internal const byte I64 = 0x7E;
    internal const byte F32 = 0x7D;
    internal const byte F64 = 0x7C;

    internal const byte ExportFunction = 0x00;
    internal const byte ExportTable = 0x01;
    internal const byte ExportMemory = 0x02;
    internal const byte ExportGlobal = 0x03;

    private readonly List<byte[]> types = [];
    private readonly List<uint> functions = [];
    private readonly List<byte[]> globals = [];
    private readonly List<byte[]> exports = [];
    private readonly List<byte[]> elements = [];
    private readonly List<byte[]> segments = [];
    private readonly List<byte[]> bodies = [];

    private byte[]? memory;
    private byte[]? table;
    private long start = -1;

    /// <summary>Declares a function type and answers its index.</summary>
    internal uint Type(byte[] parameters, byte[] results)
    {
        var encoded = new List<byte> { 0x60, (byte)parameters.Length };
        encoded.AddRange(parameters);
        encoded.Add((byte)results.Length);
        encoded.AddRange(results);
        types.Add([.. encoded]);
        return (uint)(types.Count - 1);
    }

    /// <summary>Defines a function of a declared type and answers its index.</summary>
    internal uint Function(uint typeIndex, byte[] locals, byte[] code)
    {
        functions.Add(typeIndex);

        var body = new List<byte>();

        if (locals.Length == 0)
        {
            body.Add(0x00);
        }
        else
        {
            body.AddRange(Leb((uint)locals.Length));

            foreach (var local in locals)
            {
                body.Add(0x01);
                body.Add(local);
            }
        }

        body.AddRange(code);
        body.Add(0x0B);

        var framed = new List<byte>();
        framed.AddRange(Leb((uint)body.Count));
        framed.AddRange(body);
        bodies.Add([.. framed]);

        return (uint)(functions.Count - 1);
    }

    /// <summary>Declares the module's one linear memory.</summary>
    internal void Memory(uint minimumPages, uint? maximumPages) =>
        memory = maximumPages is null
            ? [.. new byte[] { 0x00 }.Concat(Leb(minimumPages))]
            : [.. new byte[] { 0x01 }.Concat(Leb(minimumPages)).Concat(Leb(maximumPages.Value))];

    /// <summary>Declares the module's one table of function references.</summary>
    internal void Table(uint entries) =>
        table = [.. new byte[] { 0x70, 0x00 }.Concat(Leb(entries))];

    /// <summary>Declares a global with a constant initialiser, and answers its index.</summary>
    internal uint Global(byte valueType, bool mutable, byte[] initialiser)
    {
        var encoded = new List<byte> { valueType, mutable ? (byte)0x01 : (byte)0x00 };
        encoded.AddRange(initialiser);
        encoded.Add(0x0B);
        globals.Add([.. encoded]);
        return (uint)(globals.Count - 1);
    }

    /// <summary>Exports one entity under a name.</summary>
    internal void Export(string name, byte kind, uint index)
    {
        var utf8 = System.Text.Encoding.UTF8.GetBytes(name);
        var encoded = new List<byte>();
        encoded.AddRange(Leb((uint)utf8.Length));
        encoded.AddRange(utf8);
        encoded.Add(kind);
        encoded.AddRange(Leb(index));
        exports.Add([.. encoded]);
    }

    /// <summary>Adds an active element segment writing function indices at a table offset.</summary>
    internal void Element(int offset, params uint[] functionIndices)
    {
        var encoded = new List<byte> { 0x00 };
        encoded.AddRange(Instruction.I32Const(offset));
        encoded.Add(0x0B);
        encoded.AddRange(Leb((uint)functionIndices.Length));

        foreach (var index in functionIndices)
        {
            encoded.AddRange(Leb(index));
        }

        elements.Add([.. encoded]);
    }

    /// <summary>Adds an active data segment writing bytes at a memory offset.</summary>
    internal void Data(int offset, byte[] contents)
    {
        var encoded = new List<byte> { 0x00 };
        encoded.AddRange(Instruction.I32Const(offset));
        encoded.Add(0x0B);
        encoded.AddRange(Leb((uint)contents.Length));
        encoded.AddRange(contents);
        segments.Add([.. encoded]);
    }

    /// <summary>Names the start function.</summary>
    internal void Start(uint functionIndex) => start = functionIndex;

    /// <summary>Emits the module.</summary>
    internal byte[] Build()
    {
        var module = new List<byte> { 0x00, 0x61, 0x73, 0x6D, 0x01, 0x00, 0x00, 0x00 };

        module.AddRange(Section(1, Vector(types)));
        module.AddRange(Section(3, Vector(functions.Select(Leb).ToList())));

        if (table is not null)
        {
            module.AddRange(Section(4, Vector([table])));
        }

        if (memory is not null)
        {
            module.AddRange(Section(5, Vector([memory])));
        }

        if (globals.Count > 0)
        {
            module.AddRange(Section(6, Vector(globals)));
        }

        if (exports.Count > 0)
        {
            module.AddRange(Section(7, Vector(exports)));
        }

        if (start >= 0)
        {
            module.AddRange(Section(8, Leb((uint)start)));
        }

        if (elements.Count > 0)
        {
            module.AddRange(Section(9, Vector(elements)));
        }

        module.AddRange(Section(10, Vector(bodies)));

        if (segments.Count > 0)
        {
            module.AddRange(Section(11, Vector(segments)));
        }

        return [.. module];
    }

    private static byte[] Vector(IReadOnlyList<byte[]> items)
    {
        var encoded = new List<byte>();
        encoded.AddRange(Leb((uint)items.Count));

        foreach (var item in items)
        {
            encoded.AddRange(item);
        }

        return [.. encoded];
    }

    private static byte[] Section(byte identifier, byte[] body)
    {
        var framed = new List<byte> { identifier };
        framed.AddRange(Leb((uint)body.Length));
        framed.AddRange(body);
        return [.. framed];
    }

    internal static byte[] Leb(uint value)
    {
        var encoded = new List<byte>();

        do
        {
            var current = (byte)(value & 0x7F);
            value >>= 7;
            encoded.Add(value != 0 ? (byte)(current | 0x80) : current);
        }
        while (value != 0);

        return [.. encoded];
    }

    internal static byte[] Sleb(long value)
    {
        var encoded = new List<byte>();
        var more = true;

        while (more)
        {
            var current = (byte)(value & 0x7F);
            value >>= 7;

            if ((value == 0 && (current & 0x40) == 0) || (value == -1 && (current & 0x40) != 0))
            {
                more = false;
            }
            else
            {
                current |= 0x80;
            }

            encoded.Add(current);
        }

        return [.. encoded];
    }
}

/// <summary>
/// The instruction encodings the checks beside this file need, named rather than spelled in hex.
/// </summary>
/// <remarks>
/// Only the instructions the checks use are here. A missing one is not a claim that the profile
/// cannot run it; it is a claim that nothing in this harness has asked it to.
/// </remarks>
internal static class Instruction
{
    internal static byte[] Op(byte opcode) => [opcode];

    internal static byte[] Cat(params byte[][] parts)
    {
        var encoded = new List<byte>();

        foreach (var part in parts)
        {
            encoded.AddRange(part);
        }

        return [.. encoded];
    }

    internal static byte[] I32Const(int value) => Cat([0x41], WasmAssembler.Sleb(value));

    internal static byte[] I64Const(long value) => Cat([0x42], WasmAssembler.Sleb(value));

    internal static byte[] F32Const(float value) =>
        Cat([0x43], BitConverter.GetBytes(BitConverter.SingleToUInt32Bits(value)));

    internal static byte[] F64Const(double value) =>
        Cat([0x44], BitConverter.GetBytes(BitConverter.DoubleToUInt64Bits(value)));

    internal static byte[] LocalGet(uint index) => Cat([0x20], WasmAssembler.Leb(index));

    internal static byte[] LocalSet(uint index) => Cat([0x21], WasmAssembler.Leb(index));

    internal static byte[] LocalTee(uint index) => Cat([0x22], WasmAssembler.Leb(index));

    internal static byte[] GlobalGet(uint index) => Cat([0x23], WasmAssembler.Leb(index));

    internal static byte[] GlobalSet(uint index) => Cat([0x24], WasmAssembler.Leb(index));

    internal static byte[] Block(byte blockType) => [0x02, blockType];

    internal static byte[] Loop(byte blockType) => [0x03, blockType];

    internal static byte[] If(byte blockType) => [0x04, blockType];

    internal static byte[] Else() => [0x05];

    internal static byte[] End() => [0x0B];

    internal static byte[] Br(uint depth) => Cat([0x0C], WasmAssembler.Leb(depth));

    internal static byte[] BrIf(uint depth) => Cat([0x0D], WasmAssembler.Leb(depth));

    internal static byte[] BrTable(uint[] targets, uint fallback)
    {
        var encoded = new List<byte> { 0x0E };
        encoded.AddRange(WasmAssembler.Leb((uint)targets.Length));

        foreach (var target in targets)
        {
            encoded.AddRange(WasmAssembler.Leb(target));
        }

        encoded.AddRange(WasmAssembler.Leb(fallback));
        return [.. encoded];
    }

    internal static byte[] Call(uint function) => Cat([0x10], WasmAssembler.Leb(function));

    internal static byte[] CallIndirect(uint type) =>
        Cat([0x11], WasmAssembler.Leb(type), [0x00]);

    internal static byte[] Load(byte opcode, uint align, uint offset) =>
        Cat([opcode], WasmAssembler.Leb(align), WasmAssembler.Leb(offset));

    internal const byte EmptyBlock = 0x40;

    internal const byte Unreachable = 0x00;
    internal const byte Return = 0x0F;
    internal const byte Drop = 0x1A;
    internal const byte Select = 0x1B;
    internal const byte MemorySize = 0x3F;
    internal const byte MemoryGrow = 0x40;

    internal const byte I32Eqz = 0x45;
    internal const byte I32Eq = 0x46;
    internal const byte I32LtS = 0x48;
    internal const byte I32Add = 0x6A;
    internal const byte I32Sub = 0x6B;
    internal const byte I32Mul = 0x6C;
    internal const byte I32DivS = 0x6D;
    internal const byte I32RemS = 0x6F;
    internal const byte I32And = 0x71;
    internal const byte I32Shl = 0x74;
    internal const byte I32Rotl = 0x77;

    internal const byte I64Add = 0x7C;
    internal const byte I64Mul = 0x7E;
    internal const byte I64ShrU = 0x88;

    internal const byte F32Add = 0x92;
    internal const byte F32Div = 0x95;
    internal const byte F32Min = 0x96;

    internal const byte F64Add = 0xA0;
    internal const byte F64Mul = 0xA2;
    internal const byte F64Sqrt = 0x9F;

    internal const byte I32TruncF64S = 0xAA;
    internal const byte I64ExtendI32S = 0xAC;
    internal const byte F64ConvertI32S = 0xB7;
    internal const byte F32DemoteF64 = 0xB6;
    internal const byte I32ReinterpretF32 = 0xBC;
}
