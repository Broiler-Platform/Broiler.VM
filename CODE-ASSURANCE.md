# Broiler.VM Code Assurance

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,
`HUMAN_REVIEW.md`, `assurance.manifest.json` and every generated source header from the
product tree.

**Nothing in this component has been reviewed by a human.** This report records that
absence precisely. It is not a claim that the code is reviewed, assured or safe, and the
figures below are the measurement of how far from that claim the component is.

## Summary

| Metric | Value |
|---|---:|
| Files scanned | 113 |
| Files carrying an annotation | 113 |
| Code units | 4064 |
| Relevant | 2197 |
| Exempt by predicate | 1867 |
| Annotated | 2197 of 2197 (100%) |
| Human reviewed | 0 of 2197 (0%) |
| Unverified | 2197 |

## Review states

| State | Count |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 2197 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 1867 |

## IP risk

| Value | Units |
|---|---:|
| None | 736 |
| Low | 1868 |
| Medium | 82 |
| High | 0 |
| Unknown | 0 |
| *not annotated* | 0 |

## Security risk

| Value | Units |
|---|---:|
| None | 5 |
| Low | 625 |
| Medium | 1874 |
| High | 182 |
| Critical | 0 |
| *not annotated* | 0 |

## Resource impact

| Metric | Value |
|---|---:|
| Maximum | 8 / 10 |
| Average over annotated units | 1.8 / 10 |
| Units scored | 2197 |

## High-security review areas

- `Broiler.VM.IVmVerifiedState` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmInstanceState` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmProfileContinuation` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmInvocationRequest` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmProfileVerifier` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmProfileVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmExecutionStepKind` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmExecutorFactory` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.TryParse(System.ReadOnlySpan<char>, out VmProfileId)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.Parse(System.ReadOnlySpan<char>)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.TryValidateGrammar(System.ReadOnlySpan<char>, int, int, int, int, out byte)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.TryValidate(System.ReadOnlySpan<char>, out byte)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.IsAsciiLetter(char)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.IsAsciiAlphanumeric(char)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmBoundedAllocationMeter` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmBoundedAllocationMeter.TryReserve(ulong)` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmBoundedAllocationMeter.TryChargeWork(ulong)` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedAllocator` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedAllocator.TryAllocate<T>(in VmReadBounds, IVmBoundedAllocationMeter, uint, out T[])` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedAllocator.TryAllocateExact<T>(in VmReadBounds, IVmBoundedAllocationMeter, ulong, out T[])` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.VmBoundedReader(System.ReadOnlySpan<byte>, in VmReadBounds, IVmBoundedAllocationMeter)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.VmBoundedReader(System.ReadOnlySpan<byte>, in VmReadBounds, IVmBoundedAllocationMeter, ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.Remaining` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadByte(out byte)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadUInt32LittleEndian(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadUInt64LittleEndian(out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadVarUInt32(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadVarUInt64(out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadDeclaredCount(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadBytes(ulong, out System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryEnterSection(ulong, out VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryExitSection(in VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TrySkipSectionBody(in VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryChargeWork(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryTake(ulong, out System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryConsume(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadVarUInt64Core(int, out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.ChargeWork(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileTemplate(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitToString(JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitTemplateStrings(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileChain(JsChainExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader.ScanSubstitution()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Take(System.Collections.Generic.IEnumerable<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Walk(SliceNode, System.Collections.Generic.Dictionary<SliceConstructKind, int>, System.Collections.Generic.HashSet<SliceConstructKind>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructExpression` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructStatement` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceManifest` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceManifest.Admits(SliceConstructKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.MaximumSupportedNestingDepth` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.MaximumNestingDepth` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseProgram()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseStatement()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseDeclarator()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseFor()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseFunction(SliceSourceSpan, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseClass(SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseMember(bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseAssignment(bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseBinary(int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Combine(SliceSourceSpan, SliceTokenKind, SliceExpression, SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Precedence(SliceTokenKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseUnary()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseCallChain()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParsePrimary()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ArrowFollows()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseArrow(SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ConsumeStatementTerminator()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Enter()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.StatementEndsAfterCurrent()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceCompilation` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Compile(string, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Lower(SliceProgram, SliceBindingTable, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerStatement(SliceStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerIf(SliceIfStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerWhile(SliceWhileStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerDoWhile(SliceDoWhileStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerFor(SliceForStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerExpression(SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerLogical(SliceLogicalExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerConditional(SliceConditionalExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerAssignment(SliceAssignmentExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.OpcodeFor(SliceTokenKind, SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerIdentifierReference(SliceIdentifierReference)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.InDeadZone(SliceIdentifierReference)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Position(SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.Accepted` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.Refused` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.Nested(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceNodeIdentityComparer` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.UseStrictRawForms` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.Scope` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.Validate(SliceProgram)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.HoistVarBindings(System.Collections.Generic.IReadOnlyList<SliceStatement>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VarDeclaratorsWithin(System.Collections.Generic.IReadOnlyList<SliceStatement>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.DeclareLexical(SliceDeclarator, SliceDeclarationKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.CheckVarLexicalIntersection(Scope)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitStatement(SliceStatement)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitConstruct(SliceConstructKind, SliceSourceSpan, System.Collections.Generic.IReadOnlyList<SliceNode>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitExpression(SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitAssignmentTarget(SliceAssignmentExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.Resolve(SliceIdentifierReference)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.Tokenize()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadToken(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadIdentifierEscape(System.Text.StringBuilder, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.AppendScalar(System.Text.StringBuilder, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadUnicodeEscapeValue(out int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadNumericLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadStringLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.RegularExpressionIsAllowedHere()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadRegularExpressionLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadTemplateLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanTemplateBody()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanSubstitution()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanStringBody(char)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanRegularExpressionBody()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.StartsRegularExpression(char)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadPunctuator(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.Punctuators` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.IsDefined(byte)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.OperandWidth(JavaScriptOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.PopCount(JavaScriptOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.PushCount(JavaScriptOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptReadAdapter` in `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptInstance` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptContinuation` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Instantiate(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Invoke(IVmInstanceState, in VmInvocationRequest, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Run(JavaScriptInstance, int)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.SourceProviderCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorAdmitting(params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Build(ImmutableArray<string>)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Defaults()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Matrix()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.ToInt32()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.ToUint32()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.StrictlyEquals(JavaScriptValue)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.LessThan(JavaScriptValue, JavaScriptValue)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProgram` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.surfaces` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadAndCheckManifest(in VmArtifactDescriptor, ref VmBoundedReader)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadSection(ref VmBoundedReader, in VmReadBounds, JavaScriptReadAdapter, ref uint, ref SectionSet, uint, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadLimits(ref VmBoundedReader, ref SectionSet)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadReserved(ref VmBoundedReader, JavaScriptDiagnosticCode)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.Link(ref SectionSet, in VmReadBounds, JavaScriptReadAdapter)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.InvalidInCode(VmReason, JavaScriptDiagnosticCode, ulong, in SectionSet)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.Stopped(System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.FromReader(ref VmBoundedReader, ulong)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsFinalizationRegistryObject` in `src/Broiler.VM.Profile.JavaScript/JsCollections.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.DrainJobs()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Loader` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluate(JsValue[], bool, Format.JsFormat.FunctionFlags)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.MaximumCallDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsInstance.Environment` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOnGuestStack(JsInstance, uint?)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.EvalIntrinsic` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.IsEvalIntrinsic(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.SetupDynamic()` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.FromSource(JsEngine, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.PromiseSchedule(JsEngine, JsPromiseReaction, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, human line PENDING
- `Broiler.VM.VmInstanceImplementation.Dispose(System.TimeSpan)` in `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` - Security=High, human line PENDING
- `Broiler.VM.VmInstanceImplementation.LeaveStep()` in `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` - Security=High, human line PENDING
- `Broiler.VM.VmLimitPrecedence.TryApply(VmBudgetScope, ulong[], VmLimitOverrides, out ulong[], out VmBudgetDimension, out VmReason)` in `src/Broiler.VM.Runtime/VmLimitPrecedence.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime` in `src/Broiler.VM.Runtime/VmRuntime.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken)` in `src/Broiler.VM.Runtime/VmRuntime.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime.VerifyCore(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken, VmDiagnostics, VmArtifactOrigin, VmMeter?)` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime.RunVerifier(VmProfileDescriptor, in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken, VmDiagnostics, VmArtifactOrigin, VmMeter?)` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, human line PENDING

## Falsification criteria

| Metric | Value |
|---|---:|
| Units carrying a criterion | 230 |
| Units required to carry one | 182 |
| Required and missing | 0 |

A `Broiler-Falsified-If:` line states, at the declaration, the observation that would make
the unit wrong. `Security=High` says a unit is risky, which is a set and not a test; the
criterion is the test. It is required where `Security` is `High` or `Critical`, permitted
elsewhere, and rule J10 names every unit that owes one and carries none.

The line is a comment, so it is outside every fingerprint by construction: rewording a
criterion moves no recorded value here, in a file header or in
`assurance.manifest.json`, and invalidates nothing. That is the intended reading - a
criterion is an instruction to whoever reads the unit, not part of what a review is bound to.

This third line is a local extension. The owner's policy defines two lines and not three,
and it is added here because the two cannot carry a falsification criterion at all, and
because the line numbers a separate worksheet cited rotted the moment the annotations moved
the code: an annotation travels with its declaration and a citation does not. Exclusion
EX-74 records that this is an extension to the policy rather than an implementation of it,
and that the owner may reject it.

## Exemption

Exemption is decided by one predicate in `AssuranceScanner.ExemptionFor`, not per unit, so
that the rule is reviewable in one place rather than in several hundred.

| Case | Units |
|---|---:|
| TrivialPropertyOrAccessor | 602 |
| ParameterAssigningConstructor | 81 |
| TrivialExpressionBodiedMember | 28 |
| CompilerSuppliedRecordOrEnumMember | 4 |
| DelegatingOverrideOrOperator | 102 |
| InsideAssemblyMarker | 0 |
| FieldDeclaringStorage | 307 |
| EnumMemberOfADeclaredVocabulary | 743 |
| DeclaredInSource | 0 |

## Per-unit exemptions

| Metric | Value |
|---|---:|
| Per-unit exemptions | 0 |

A per-unit `EXEMPT=<reason>` line exempts one unit by a reason a human wrote, for what the
predicate cannot see. Nothing mechanical checks that the reason is true, that it describes
the unit it sits on, or that it says anything at all, so every use is counted and named
here. `Broiler.VM.Binary` is closed to it entirely: that assembly reads untrusted
input, and a unit there is assessed or it is not shipped. Rule J1 asserts both halves.

No unit in this component states a per-unit exemption.

## Change detection

`assurance.manifest.json` lists **every** code unit in the three product assemblies -
4064 of them, exempt and relevant alike - with the fingerprint of its declaration.
This manifest is a change-detection record, not a review. A unit listed there is watched, not reviewed:
the entry records what the declaration's tokens hashed to when the generator last ran, and
nothing else. Exempt units still need no annotation and carry none, and no human line in
this component has moved off `PENDING`. What the manifest adds is that a unit the exemption
predicate treats as trivial is no longer invisible: a semantic change to one moves a value
in a generated file the gate compares byte for byte. Rule J7 holds the manifest to the tree.

Beside the units it lists **every covered file** - 113 of them - with a
fingerprint over the complete token stream of its compilation unit. A unit entry exists only
for a declaration kind the scanner enumerates, and an enumeration is a whitelist: an
`[assembly: ...]` attribute is a member of nothing and can be in no unit at all.
Nothing in a covered file can change without something moving here, whatever kind of declaration it is. Comments are outside the stream, because a token's
text is its own characters, so the generated header above and the annotation lines below move
no file fingerprint - which is what lets one generation be a fixed point.

## Verification

The generator and the gate are the same code, run as a test in the architecture suite. Two
lanes under `.github/workflows/` compel it rather than leaving it to whoever remembers: the
review lane regenerates every artefact on a pull request and commits what moved, and the
publish lane runs the release mode below and refuses to pack while anything is unresolved.
Exclusion EX-45 still records one RID and one machine for the Native AOT evidence, which no
lane reproduces.

| Mode | Command | Effect |
|---|---|---|
| Generate | `BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release` | Fills every `Fingerprint=TBF`, refreshes a decision the code has outrun into `STALE; Previous=...`, rewrites the generated headers, `HUMAN_REVIEW.md`, `assurance.manifest.json` and this file. |
| Gate | `dotnet test Broiler.VM.slnx -c Release` | Asserts every generated artefact is byte-identical to what the generator would produce. |
| Release | `BROILER_ASSURANCE_RELEASE=1 dotnet test Broiler.VM.slnx -c Release` | The gate, and additionally: no relevant unit left in a state that blocks a release, no annotation this system cannot read, no fingerprint out of date, no unit at the top of the security vocabulary without a criterion. |

The fingerprint is six hex characters - 24 bits - of SHA-256 over the declaration's token
texts, joined by single spaces. Trivia is excluded because a token's text is its own
characters and never the comments or whitespace around it, so `dotnet format` moves no
fingerprint and an annotation is never part of what it describes. The value answers whether a
unit changed since it was reviewed. It is not a collision-free identifier across units and it
is not a cryptographic commitment.
