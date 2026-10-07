// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           78
// Human-reviewed:   0/23
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       23
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- supporting types ------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6B7777
    // Broiler-Human:        PENDING
    private enum ScopeKind
    {
        Program,
        Function,
        Block,

        /// <summary>
        /// A module's own environment, which is a scope of SLOTS rather than of globals.
        /// </summary>
        /// <remarks>
        /// It is a kind of its own and not <see cref="Program"/> with a flag, because every place
        /// that asks the question asks it about the top level: a script's top-level declaration is
        /// a property of the global object or a binding of the realm's lexical half, and a module's
        /// is a slot nothing outside the module can name.
        /// </remarks>
        Module,

        /// <summary>
        /// The object environment record a <c>with</c> puts on the chain, which declares nothing.
        /// </summary>
        /// <remarks>
        /// <b>It is in this chain so that the HOP COUNTS stay right.</b> The record exists at run
        /// time and every <c>(depth, slot)</c> pair emitted inside a <c>with</c> body counts it, so
        /// a compile-time chain that left it out would resolve every enclosing name one record too
        /// close. It also declares no name and never can, which is what makes a name resolved
        /// through it a lookup on an object rather than on a scope.
        /// </remarks>
        With,

        /// <summary>
        /// An evaluated program's own record: the eval boundary, whose parent at run time is its
        /// caller's current record.
        /// </summary>
        /// <remarks>
        /// It is the root of the compile-time chain, as <see cref="Program"/> is, and a name that
        /// reaches it unresolved is one of the caller's (JSD-0026 section 5).
        /// </remarks>
        Eval,

        /// <summary>
        /// A function body's own variable environment, pushed inside the parameters' record when
        /// the parameter list has expressions that could tell the two apart (JSeal V15-finish).
        /// </summary>
        /// <remarks>
        /// It holds what a <see cref="Function"/> record holds for a body - its <c>var</c>s,
        /// functions and top-level lexical declarations - and is the variable environment a body's
        /// direct eval declares into; the parameters and <c>arguments</c> stay in the unit's own
        /// record, where the parameter list's closures see them.
        /// </remarks>
        Body,
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B6D523
    // Broiler-Human:        PENDING
    private enum ExitKind
    {
        Loop,
        Switch,
        Label,
        Finally,

        /// <summary>
        /// A statement list that declared a resource: every exit through it disposes its scope.
        /// </summary>
        Dispose,
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=418641
    // Broiler-Human:        PENDING
    private sealed class Label
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A0B10
        // Broiler-Human:        PENDING
        internal int Offset { get; set; } = -1;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BDD300
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<int> Sites { get; } = [];
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D1A5EA
    // Broiler-Human:        PENDING
    private sealed class Exit(ExitKind kind, string label, int depth)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=57323F
        // Broiler-Human:        PENDING
        internal ExitKind Kind { get; } = kind;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FFD2E9
        // Broiler-Human:        PENDING
        internal string Label { get; } = label;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DCD9D5
        // Broiler-Human:        PENDING
        internal int Depth { get; } = depth;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F69F0D
        // Broiler-Human:        PENDING
        internal Label? Break { get; init; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B027D9
        // Broiler-Human:        PENDING
        internal Label? Continue { get; init; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=90737C
        // Broiler-Human:        PENDING
        internal JsBlockStatement? Finaliser { get; init; }

        /// <summary>
        /// The slot holding this loop's iterator record, or -1 when it is not a <c>for … of</c>.
        /// </summary>
        /// <remarks>
        /// <b>Leaving a <c>for … of</c> owes the iterator a <c>return</c>, and the code that leaves
        /// is not written where the loop is.</b> A <c>break</c> three blocks in, a labelled
        /// <c>break</c> out of two loops and a <c>return</c> from anywhere all have to close every
        /// iterator they pass, innermost first, and they find them here rather than by walking the
        /// syntax back up. <c>continue</c> is the exit that must NOT close, which is the reason this
        /// is a slot on the record rather than a fixed instruction at the loop's bottom.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=39F68C
        // Broiler-Human:        PENDING
        internal int IteratorSlot { get; init; } = -1;

        /// <summary>
        /// Whether the iterator in <see cref="IteratorSlot"/> is an ASYNC one.
        /// </summary>
        /// <remarks>
        /// <b>The close is a different sequence and not a different operand.</b> A synchronous
        /// close is one instruction; an asynchronous one calls <c>return</c>, awaits what it
        /// answered and then requires it to be an object — three instructions with a suspension in
        /// the middle — and it skips all three when there is no <c>return</c> to call. Every exit
        /// that owes a close has to know which, and the exit record is the one thing all four of
        /// them already consult.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D82625
        // Broiler-Human:        PENDING
        internal bool IteratorIsAsync { get; init; }

        /// <summary>
        /// Whether this finaliser's body is being emitted right now.
        /// </summary>
        /// <remarks>
        /// <b>A `return` inside a `finally` must not run that same `finally` again.</b> The exit
        /// stack still carries the entry while its body is being emitted - it has to, because the
        /// body is emitted in the middle of compiling the statement that leaves - so without this
        /// flag the compiler emits the finaliser, meets the `return` inside it, and emits the
        /// finaliser again, forever. It is a compile-time loop rather than a wrong program, which
        /// is the only reason it is not worse.
        /// </remarks>
        internal bool Running { get; set; }

        /// <summary>
        /// The slot of <see cref="DisposalScope"/> that holds this list's disposal scope, or -1 when
        /// this is not a <see cref="ExitKind.Dispose"/> exit.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5FBE62
        // Broiler-Human:        PENDING
        internal int DisposalSlot { get; init; } = -1;

        /// <summary>The compile-time scope <see cref="DisposalSlot"/> is a slot of.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7CBD5C
        // Broiler-Human:        PENDING
        internal Scope? DisposalScope { get; init; }

        /// <summary>
        /// Whether the list declared an <c>await using</c>, so that every exit through it awaits.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2DCC47
        // Broiler-Human:        PENDING
        internal bool DisposalIsAsync { get; init; }

        /// <summary>Where the region guarding the list's statements, or a <c>try</c>'s block, begins.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2118F7
        // Broiler-Human:        PENDING
        internal int Guarded { get; init; }

        /// <summary>The handler that disposes the list's scope for a throw or a forced return.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=92FB33
        // Broiler-Human:        PENDING
        internal Label? DisposalHandler { get; init; }

        /// <summary>The first resource declaration of the list, where its disposal is reported.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9F069A
        // Broiler-Human:        PENDING
        internal SliceSourceSpan DisposalSpan { get; init; }

        /// <summary>
        /// The <see cref="regionLevel"/> of the statement this exit leaves when that statement owns
        /// exception regions - a <c>try … finally</c>, a <c>for … of</c> or a resource scope - and
        /// -1 for an exit that owns none.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=383906
        // Broiler-Human:        PENDING
        internal int Level { get; init; } = -1;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=04B3E8
    // Broiler-Human:        PENDING
    private sealed class Scope(ScopeKind kind, Scope? parent)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A869FD
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.Dictionary<string, (int Slot, bool Constant)> names =
            new(System.StringComparer.Ordinal);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A516E8
        // Broiler-Human:        PENDING
        internal ScopeKind Kind { get; } = kind;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A9A20D
        // Broiler-Human:        PENDING
        internal Scope? Parent { get; } = parent;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=069DEC
        // Broiler-Human:        PENDING
        internal int SlotCount { get; private set; }

        /// <summary>Whether this block is a <c>catch</c> clause's, which the eval scope map says.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=611FEA
        // Broiler-Human:        PENDING
        internal bool IsCatch { get; init; }

        /// <summary>
        /// Whether this record is the one a named function expression binds its own name in.
        /// </summary>
        /// <remarks>
        /// That binding is immutable and NOT strict: an assignment to it is ignored in sloppy code
        /// and a <c>TypeError</c> in strict code, which neither a constant nor a mutable slot is.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=45B662
        // Broiler-Human:        PENDING
        internal bool IsFunctionName { get; init; }

        /// <summary>
        /// The first slot of a lexical declaration in a function's or a catch clause's record, whose
        /// earlier slots are its parameters, <c>var</c>s and functions (JSeal V15).
        /// </summary>
        /// <remarks>
        /// Every name of a block's or an evaluation's own record is lexical and every name of a
        /// program's is not, so only those two kinds read it; until it is set nothing in them is.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=872837
        // Broiler-Human:        PENDING
        internal int LexicalFrom { get; set; } = int.MaxValue;

        /// <summary>
        /// The slots of a function body's top-level lexical declarations that were declared before
        /// <see cref="LexicalFrom"/>, when the body hoists them ahead of its function declarations
        /// so its closures can see them (VM-FIX-D with JSeal V15).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8ABF32
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.HashSet<int>? LexicalSlots { get; set; }

        /// <summary>
        /// Whether this is a sloppy function whose own code may call <c>eval</c> directly, and so may
        /// gain bindings by name at run time (JSeal V15, JSD-0026 step 6).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B79FCF
        // Broiler-Human:        PENDING
        internal bool EvalVariables { get; set; }

        /// <summary>
        /// The first slot a function's parameter list did not declare - its parameters and the
        /// <c>arguments</c> object precede it - when the list may call <c>eval</c> (JSeal V15).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8BA04C
        // Broiler-Human:        PENDING
        internal int ParameterLimit { get; set; }

        /// <summary>
        /// Whether this function's body has a variable environment of its own, pushed inside this
        /// record after the parameter list (JSeal V15-finish).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=9784BB
        // Broiler-Human:        PENDING
        internal bool SeparateBody { get; set; }

        /// <summary>Whether the binding in <paramref name="slot"/> is a lexical declaration.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2DACF0
        // Broiler-Human:        PENDING
        internal bool IsLexical(int slot) => Kind switch
        {
            ScopeKind.Block => !IsCatch || slot >= LexicalFrom,
            ScopeKind.Eval => true,
            ScopeKind.Function or ScopeKind.Body =>
                slot >= LexicalFrom || (LexicalSlots?.Contains(slot) ?? false),
            _ => false,
        };

        /// <summary>Every name this record binds, in no particular order.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=001D07
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IEnumerable<string> Names => names.Keys;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DB65E3
        // Broiler-Human:        PENDING
        internal int Declare(string name, bool constant)
        {
            if (names.TryGetValue(name, out var existing))
            {
                return existing.Slot;
            }

            var slot = SlotCount++;
            names[name] = (slot, constant);
            return slot;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1735B6
        // Broiler-Human:        PENDING
        internal bool Has(string name) => names.ContainsKey(name);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9E0F15
        // Broiler-Human:        PENDING
        internal int SlotOf(string name) => names[name].Slot;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=553A6E
        // Broiler-Human:        PENDING
        internal bool TryGet(string name, out int slot, out bool constant)
        {
            if (names.TryGetValue(name, out var found))
            {
                slot = found.Slot;
                constant = found.Constant;
                return true;
            }

            slot = 0;
            constant = false;
            return false;
        }
    }

    /// <summary>What lowering one module has established so far.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5AA680
    // Broiler-Human:        PENDING
    private sealed class ModuleBuild(string key, Scope environment)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=71D82D
        // Broiler-Human:        PENDING
        internal string Key { get; } = key;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FD8308
        // Broiler-Human:        PENDING
        internal Scope Scope { get; } = environment;

        /// <summary>The module specifiers this module requests, in source order.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AE3191
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string> Requests { get; } = [];

        /// <summary>The key each request resolved to, parallel to <see cref="Requests"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=50B8BD
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string> RequestKeys { get; } = [];

        /// <summary>The unit this module was presented as, which carries its resolutions.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CDD8D9
        // Broiler-Human:        PENDING
        internal JsModuleUnit? Unit { get; init; }

        /// <summary>Each imported local name, and its index in the artifact's import table.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F84707
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.Dictionary<string, int> Imports { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>This module's import entries, in the order the artifact writes them.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=90DB02
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsImportEntryRow> ImportRows { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=30B446
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsLocalExportRow> LocalExports { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F68387
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsIndirectExportRow> IndirectExports { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0AD7C7
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<uint> StarExports { get; } = [];

        /// <summary>Every name this module publishes, so a second use of one is an early error.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EC9EB4
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.HashSet<string> ExportNames { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>The function declarations the initialiser gives their closures.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D9E337
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsFunctionNode> Functions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6BDB99
        // Broiler-Human:        PENDING
        internal int InitialiserUnit { get; set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BD9C34
        // Broiler-Human:        PENDING
        internal int BodyUnit { get; set; }
    }
    /// <summary>A region whose range is still being emitted.</summary>
    /// <param name="StackHeight">
    /// The operand height the handler is entered at, under the one value the executor pushes. It is
    /// zero for every region a STATEMENT opens, because a statement boundary is the one place the
    /// operand stack is reliably empty, and it is the height at the pattern for the two regions an
    /// array pattern opens - which is the whole of what lets a region guard an expression.
    /// </param>
    /// <param name="Level">
    /// The <see cref="regionLevel"/> of the statement that owns the region. A region no statement
    /// owns guards one expression or one instruction, never a <c>break</c> or a <c>return</c>, and
    /// keeps the default.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F84FC0
    // Broiler-Human:        PENDING
    private readonly record struct PendingRegion(
        int TryStart,
        Label Handler,
        int ScopeDepth,
        JsFormat.HandlerKind Kind,
        int StackHeight = 0,
        int Level = int.MaxValue);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=075549
    // Broiler-Human:        PENDING
    private sealed class ClosedRegion(
        uint tryStart,
        uint tryEnd,
        Label handler,
        uint scopeDepth,
        JsFormat.HandlerKind kind,
        uint stackHeight)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=61A751
        // Broiler-Human:        PENDING
        internal uint TryStart { get; } = tryStart;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D123E4
        // Broiler-Human:        PENDING
        internal uint TryEnd { get; } = tryEnd;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=707074
        // Broiler-Human:        PENDING
        internal Label HandlerLabel { get; } = handler;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CCD6C7
        // Broiler-Human:        PENDING
        internal uint Handler => (uint)System.Math.Max(0, HandlerLabel.Offset);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2DCEC1
        // Broiler-Human:        PENDING
        internal uint ScopeDepth { get; } = scopeDepth;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1F1D54
        // Broiler-Human:        PENDING
        internal JsFormat.HandlerKind Kind { get; } = kind;

        /// <summary>The operand height the handler is entered at, under the value pushed there.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=695B57
        // Broiler-Human:        PENDING
        internal uint StackHeight { get; } = stackHeight;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3F0624
    // Broiler-Human:        PENDING
    private sealed class UnitBuffer(ushort nameConstant, JsFormat.FunctionFlags flags)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=491BB2
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<byte> Code { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3FBC4A
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<int> BranchSites { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=62A27A
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<Label> Labels { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4380B9
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<(int Site, Scope Scope)> ScopeSites { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6DBCE6
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<PendingRegion> PendingRegions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C4FF50
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<ClosedRegion> Regions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4C27F0
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<(uint Offset, uint Line, uint Column)> Positions { get; } = [];

        /// <summary>The direct-<c>eval</c> sites of this unit that get a map row, at unit-relative offsets.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=775BA1
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<(int Offset, Scope Scope, int Depth, JsFormat.EvalRequestFlags Flags, System.Collections.Generic.HashSet<Scope>? Parameters)> EvalSites { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A31EDA
        // Broiler-Human:        PENDING
        internal ushort NameConstant { get; } = nameConstant;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0E6DFE
        // Broiler-Human:        PENDING
        internal JsFormat.FunctionFlags Flags { get; set; } = flags;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E45E5F
        // Broiler-Human:        PENDING
        internal int ParameterCount { get; init; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=10AC31
        // Broiler-Human:        PENDING
        internal int SlotCount { get; set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BB8115
        // Broiler-Human:        PENDING
        internal int MaximumStack { get; private set; } = 8;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=96DBEF
        // Broiler-Human:        PENDING
        internal int Height { get; private set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E86B38
        // Broiler-Human:        PENDING
        internal int LastLine { get; set; } = -1;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=50E8CA
        // Broiler-Human:        PENDING
        internal int LastColumn { get; set; } = -1;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F88AC0
        // Broiler-Human:        PENDING
        internal void Track(JsOpcode opcode, uint operand)
        {
            if (!JsOpcodes.TryDescribe(opcode, operand, out var pops, out var pushes))
            {
                return;
            }

            Height = System.Math.Max(0, Height - pops + pushes);
            MaximumStack = System.Math.Max(MaximumStack, Height + 24);
        }

        /// <summary>Re-states the height at a join this straight-line pass walked past.</summary>
        /// <remarks>
        /// <para>
        /// <b><see cref="Track"/> follows the code as written, and an optional chain's guard is the
        /// one lowering here whose written order is not its taken order.</b> The guard's
        /// fall-through is the SHORT CIRCUIT - it pops what the chain was holding and pushes one
        /// value - while the path that continues arrives by branch, holding everything the chain
        /// had. So the pass leaves the model one value low for every guard that was holding two,
        /// and a function with more of those than the twenty-four slots of slack absorb declares a
        /// stack it then overflows. The verifier catches it, which is the good outcome and a poor
        /// diagnosis: it names the instruction that overflowed and not the guard that mis-declared.
        /// </para>
        /// <para>
        /// <b>So the guard says what the height really is instead of the slack being widened to
        /// hide it.</b> Widening would buy a bigger number for every function in the artifact to
        /// pay for an error in a few, and would leave the model wrong - which is worse than a
        /// model that is right, because the next lowering to reach for it would inherit the error.
        /// </para>
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F87859
        // Broiler-Human:        PENDING
        internal void Rejoin(int height)
        {
            Height = System.Math.Max(0, height);
            MaximumStack = System.Math.Max(MaximumStack, Height + 24);
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1EA494
        // Broiler-Human:        PENDING
        internal void CloseRegion(int tryStart, int tryEnd)
        {
            for (var index = PendingRegions.Count - 1; index >= 0; index--)
            {
                if (PendingRegions[index].TryStart != tryStart)
                {
                    continue;
                }

                var pending = PendingRegions[index];
                PendingRegions.RemoveAt(index);

                // THE RANGE IS WRITTEN AS THE PIECES NO HOLE OF THIS LEVEL OR A SHALLOWER ONE
                // COVERS, in ascending order and at the position the whole range would have taken,
                // so the executor's first-match search meets them exactly where it met the range.
                // A piece that would protect nothing is dropped rather than written empty.
                //
                // THE HOLES ARE RECORDED IN THE ORDER THEIR ENDS WERE EMITTED, so the ones that can
                // reach into this range are a suffix found by halving: a close costs the holes
                // inside the range and not every hole the unit has, and a unit of many small
                // statements stays linear in its code.
                var cuts = new System.Collections.Generic.List<(int Start, int End)>();
                var low = 0;
                var high = Holes.Count;

                while (low < high)
                {
                    var middle = low + ((high - low) / 2);

                    if (Holes[middle].End <= tryStart)
                    {
                        low = middle + 1;
                    }
                    else
                    {
                        high = middle;
                    }
                }

                for (var at = low; at < Holes.Count; at++)
                {
                    var hole = Holes[at];

                    if (hole.Level <= pending.Level && hole.Start < tryEnd)
                    {
                        cuts.Add((hole.Start, hole.End));
                    }
                }

                cuts.Sort();
                var from = tryStart;

                foreach (var cut in cuts)
                {
                    if (cut.Start > from)
                    {
                        AddPiece(pending, from, cut.Start);
                    }

                    from = System.Math.Max(from, cut.End);
                }

                if (from < tryEnd)
                {
                    AddPiece(pending, from, tryEnd);
                }

                return;
            }
        }

        /// <summary>Writes one piece of a pending region's range as a closed row.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1850D5
        // Broiler-Human:        PENDING
        private void AddPiece(PendingRegion pending, int from, int until)
        {
            Regions.Add(new ClosedRegion(
                (uint)from,
                (uint)until,
                pending.Handler,
                (uint)pending.ScopeDepth,
                pending.Kind,
                (uint)pending.StackHeight));
        }

        /// <summary>
        /// The code ranges that leave a statement, each with the <see cref="regionLevel"/> of the
        /// statement it leaves.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FA36BB
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<(int Start, int End, int Level)> Holes { get; } = [];

        /// <summary>
        /// Records that the code from <paramref name="start"/> to the cursor runs after the
        /// statement at <paramref name="level"/> has completed, so that no region of that
        /// statement, or of one nested in it, covers it when it is closed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>A <c>finally</c> ran twice because its own region covered the copy of it that a
        /// <c>return</c> inlines.</b> The unwinding a <c>break</c>, <c>continue</c> or
        /// <c>return</c> emits - finalisers, iterator closes, resource disposal - is written where
        /// the jump is, inside every protected range around it. What that code throws belongs to
        /// the statements still enclosing it once the ones it leaves have completed: the
        /// language's <c>try</c> runs its finaliser after the block's completion is settled and
        /// its <c>catch</c> never sees a <c>return</c>. So <c>try { return 1; } finally { throw
        /// 2; }</c> re-ran the finaliser from its own handler, and a disposer that threw after a
        /// passed <c>finally</c> re-ran that one.
        /// </para>
        /// <para>
        /// <b>The ranges are split rather than the unwinding moved out of line</b>, because the
        /// inline form is what lets each exit continue to a different target with no completion
        /// record, and a split changes no instruction and no row format: a region becomes several
        /// rows with one handler, which the verifier already seeds and joins as one entry.
        /// </para>
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9DE8CE
        // Broiler-Falsified-If: a region of the statement at the given level, or of one nested in it, is closed with a row covering an instruction between start and the cursor at the call
        // Broiler-Human:        PENDING
        internal void Leave(int start, int level)
        {
            // THE END IS ALWAYS THE CURSOR, which only grows, and that is what keeps the list in the
            // order the search in `CloseRegion` relies on.
            if (start < Code.Count)
            {
                Holes.Add((start, Code.Count, level));
            }
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5BEFB8
        // Broiler-Human:        PENDING
        internal void FinishScopes()
        {
            foreach (var (site, target) in ScopeSites)
            {
                var count = (ushort)target.SlotCount;
                Code[site] = (byte)(count & 0xFF);
                Code[site + 1] = (byte)(count >> 8);
            }
        }
    }
}
