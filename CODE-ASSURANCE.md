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
| Files scanned | 165 |
| Files carrying an annotation | 165 |
| Code units | 6395 |
| Relevant | 3524 |
| Exempt by predicate | 2871 |
| Annotated | 3524 of 3524 (100%) |
| Human reviewed | 0 of 3524 (0%) |
| Unverified | 3524 |

## Review states

| State | Count |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 3524 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 2871 |

## IP risk

| Value | Units |
|---|---:|
| None | 1126 |
| Low | 3234 |
| Medium | 89 |
| High | 0 |
| Unknown | 0 |
| *not annotated* | 0 |

## Security risk

| Value | Units |
|---|---:|
| None | 6 |
| Low | 659 |
| Medium | 3200 |
| High | 490 |
| Critical | 94 |
| *not annotated* | 0 |

## Resource impact

| Metric | Value |
|---|---:|
| Maximum | 9 / 10 |
| Average over annotated units | 1.9 / 10 |
| Units scored | 3524 |

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
- `Broiler.VM.Profile.JavaScript.Compiler.CompilationStack` in `src/Broiler.VM.Profile.JavaScript.Compiler/CompilationStack.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.CompilationStack.Run<T>(System.Func<T>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/CompilationStack.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.ToArray()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.Branch(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.BranchIf(JsArm64Condition, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.TryFix(out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.Scaled(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.PairOffset(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.StorePairPreIndex(int, int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.LoadPairPostIndex(int, int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.Return()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.SetIfCondition(int, JsArm64Condition)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.LoadDouble(int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.StoreDouble(int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.CompareDouble(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Frame` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Frame.StackFrameBytes` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Frame.UndefinedBits` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.TryEmit(JsAssembledProgram, out JsNativeEmission, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.EmitUnit(JsArm64Assembler, JsAssembledProgram, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Prologue(JsArm64Assembler)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Epilogue(JsArm64Assembler)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Charge(JsArm64Assembler, int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.EmitInstruction(JsArm64Assembler, JsAssembledProgram, JsArm64Walk, int, int[], int, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Binary(JsArm64Assembler, int, JsArm64Operation)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Compare(JsArm64Assembler, int, bool, JsArm64Condition)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Falsy(JsArm64Assembler, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.JumpIfTrue(JsArm64Assembler, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk.LocalSlots` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk.TryTrace(out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk.Step(int, int[], bool[], int, System.Collections.Generic.Queue<(int At, int[] Scopes, bool[] Slots, int Height)>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.SweepAgainstTheNumericManifest(JsAssembledProgram)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexB(System.Collections.Generic.IReadOnlyList<JsStatement>, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexBStatement(JsStatement, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexBBlock(System.Collections.Generic.IReadOnlyList<JsStatement>, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitAnnexBAlias(string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ProtectSomething(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileTemplate(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitToString(JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitTemplateStrings(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileChain(JsChainExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Shadowable(string, out int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Statement(JsStatement, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Function(JsFunctionDeclaration, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Expression(JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Call(JsCallExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.ParseArrowParameters()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.Deepen(SliceSourceSpan, out JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader.ScanSubstitution()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.Windows` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.SystemV` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.Host` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.FrameBytes` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.FramePointerSlot` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.TryBind(int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.TryFinish(out byte[], out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.ModRmMemory(int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Ret()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.AddMemory32Immediate32(JsX64Register, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.SubMemory32Immediate32(JsX64Register, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Jump(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.JumpIf(JsX64Condition, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Call(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.CallRegister(JsX64Register)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.MovsdLoad(int, JsX64Register, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Ucomisd(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Frame` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.SemanticVersion` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.TryEmit(JsNativeProgramImage, out byte[], out JsNativeSymbolRow[], out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.TryReadBindings(JsNativeProgramImage, out System.Collections.Generic.Dictionary<int, int>, out System.Collections.Generic.HashSet<int>, out System.Collections.Generic.HashSet<int>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitUnit(JsX64Assembler, JsNativeProgramImage, int, int[], System.Collections.Generic.Dictionary<int, int>, System.Collections.Generic.HashSet<int>, System.Collections.Generic.HashSet<int>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitPrologue(JsX64Assembler, JsX64UnitPlan, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitFuelCharge(JsX64Assembler, JsX64UnitPlan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitEpilogues(JsX64Assembler, JsX64UnitPlan, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitInstruction(JsX64Assembler, JsNativeProgramImage, JsX64Walk, JsX64UnitPlan, JsFunctionRow, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitLoadGlobal(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitStoreGlobal(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, int, JsOpcode, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitArithmetic(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, JsOpcode, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitNot(JsX64Assembler, JsX64UnitPlan, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitComparison(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, JsOpcode, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitBranch(JsX64Assembler, JsX64UnitPlan, int, int, int, int, JsOpcode)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitCall(JsX64Assembler, JsNativeProgramImage, JsX64UnitPlan, JsX64Value[], int, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64ValueKind` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk.TryResolveSlot(int, int, int, out int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk.TryTrace(out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk.Step(int, JsX64Value[], int[], System.Collections.Generic.Stack<(int, JsX64Value[], int[])>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Take(System.Collections.Generic.IEnumerable<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Walk(SliceNode, System.Collections.Generic.Dictionary<SliceConstructKind, int>, System.Collections.Generic.HashSet<SliceConstructKind>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructExpression` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructStatement` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceManifest` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceManifest.Admits(SliceConstructKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.MaximumSupportedNestingDepth` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.MaximumTreeDepth` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` - Security=High, human line PENDING
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Deepen(SliceSourceSpan, out SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Enter()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.StatementEndsAfterCurrent()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceCompilation` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Compile(string, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.CompileOnTheDeclaredStack(string, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, human line PENDING
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.RefusedModules` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, human line PENDING
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
- `Broiler.VM.Profile.JavaScript.Format.JsNativeValues` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeValues.UninitialisedBits` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeAbiProbe` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeAbiProbeLayout` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeFrame` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeFrame.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.Scan(JsNativeArchitecture, byte[], JsNativeSymbolRow[], uint, System.Collections.Generic.ICollection<string>?)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.Admits(JsNativeFieldKind, long)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptReadAdapter` in `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptInstance` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptContinuation` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Instantiate(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Invoke(IVmInstanceState, in VmInvocationRequest, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Run(JavaScriptInstance, int)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.TurnEntryPoint` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.HostSurfaceBindingIndex` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.SourceProviderCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.ResolveCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.HostSurfaceCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorAdmitting(params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorReEmittingWith(Format.IJsNativeEmitter, params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorHostingRealms(IJsHostSurface, params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Build(ImmutableArray<string>, Format.IJsNativeEmitter?, IJsHostSurface?)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Defaults()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Matrix()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.ToInt32()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.ToUint32()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.StrictlyEquals(JavaScriptValue)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.LessThan(JavaScriptValue, JavaScriptValue)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptProgram` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.surfaces` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.JavaScriptVerifier(VmProfileId, VmFeatureManifestId, System.Collections.Immutable.ImmutableArray<string>, Format.IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.reEmitter` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, human line PENDING
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
- `Broiler.VM.Profile.JavaScript.JsEngine.StepOneJob(out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.DropPendingJobs()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Loader` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluate(JsValue[], bool, Format.JsFormat.FunctionFlags)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.EndHostStep()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.MaximumCallDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.reportingDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.StackBackstopReached()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeText(int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeHostCrossing(ulong)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ToNumberFromText(string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.instanced` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.RunModuleGraph(JsProgram, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluated(JsProgram, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Instantiate(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Step(JsProgram, JsModuleInstance[], System.Collections.Generic.List<int>, int, System.Action<JsEngine, JsValue, bool>?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Confirm(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.DynamicImport(JsProgram, string, JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ImportedModule(JsProgram, string, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.BindParameters(JsProgram, JsCodeUnit, JsScriptFunction, JsFrame, JsValue, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeGenerator(JsValue, JsResumeMode, JsValue, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeAsync(JsAsyncCall, JsResumeMode, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.EnqueueAsyncGenerator(JsValue, JsResumeMode, JsValue, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeAsyncGenerator(JsAsyncGenerator, JsResumeMode, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.Delegate(JsFrame, JsValue[], ref int, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.DelegateAsync(JsFrame, JsValue[], ref int, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ResolveName(System.Collections.Generic.List<JsEnvironment>, int, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeComparison(JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsPause` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsContinuation` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsInstance.Environment` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.RunHostTurn(VmProfileId, JsInstance, IJsHostSurface)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.InstallHostSurface(JsEngine, IJsHostSurface)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.StepEntryPoint` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.StepJobs(VmProfileId, JsInstance, int)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.Resume(VmProfileId, IVmInstanceState, IVmProfileContinuation)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.Unwind(IVmProfileContinuation)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOnGuestStack(System.Action)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOnGuestStack(JsInstance, uint?)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOneJobOnGuestStack(JsInstance)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsEnvironment` in `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsFrame` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsForcedReturn` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsGenerator` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsAsyncCall` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsAsyncGenerator` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostObject` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostObject.TryGetOwnProperty(string, out JsProperty)` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostObject.SetOwnProperty(string, JsProperty)` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.EndStep()` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.DefineIndex(JsHostValue, uint, JsHostValue, JsHostPropertyFlags)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Invoke(JsHostValue, JsHostValue, System.ReadOnlySpan<JsHostValue>)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.EnqueueJob(System.Action)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.DrainJobs(int)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Wrap(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Unwrap(JsHostValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Installing` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.BindConstructor(JsHostFunction)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Bind(JsHostFunction)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Enter(ulong)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Latch(JsAbort)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostRef` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsHostTerminatedException` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.IJsHostSurface` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.IJsHostExotic` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeAbiObservation` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeAbi` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeAbi.Run(System.ReadOnlySpan<byte>, uint, uint, int, int, double[], long, int)` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.OperandSlabSlots` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.FuelPerInvocation` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.JsNativeInstance(JsProgram, JsNativePage, IVmExecutionEnvironment, double[], double[], double[], long[])` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Page` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Operands` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Bindings` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Fuel` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.TryCreate(JsProgram, IVmExecutionEnvironment)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Dispose()` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.CarriesEmittedCode(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.HostArchitecture` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Instantiate(JsProgram, IVmExecutionEnvironment)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Invoke(VmProfileId, JsNativeInstance, in VmInvocationRequest)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.TryFindSymbol(JsProgram, uint, out uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Render(double)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.TypeOf(double)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.ProtReadWrite` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.ProtReadExecute` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapPrivateAnonymous` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapFailed` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapUnix(nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.ArmUnix(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.ReleaseUnix(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.Map(void*, nuint, int, int, int, nint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.Protect(void*, nuint, int)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.Unmap(void*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.MemCommitAndReserve` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.MemRelease` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.PageReadWrite` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.PageExecuteRead` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapWindows(nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.ArmWindows(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.ReleaseWindows(byte*)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.VirtualAlloc(void*, nuint, uint, uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.VirtualProtect(void*, nuint, uint, uint*)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.VirtualFree(void*, nuint, uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.FlushInstructionCache(void*, void*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.GetCurrentProcess()` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePageState` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.address` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.length` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.state` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.JsNativePage(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.State` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.TryMap(System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.Arm()` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.Entry(uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.At(uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsNativePage.Dispose()` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.GetAsyncIterator(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.AsyncGenerator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.EvalIntrinsic` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.IsEvalIntrinsic(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.SetupDynamic()` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.FromSource(JsEngine, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.PromiseSchedule(JsEngine, JsPromiseReaction, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsRealm.AwaitOn(JsEngine, JsValue, System.Action<JsEngine, JsValue, bool>)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadManifest(in VmArtifactDescriptor, ref VmBoundedReader, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadNativeCode(ref VmBoundedReader, ulong, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadNativeSymbols(ref VmBoundedReader, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkNative(Sections, JsCodeUnit[], IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReEmit(Sections, byte[], JsNativeSymbolRow[], IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkModules(Sections, JsCodeUnit[], IVmVerificationContext, JavaScriptReadAdapter, out JsModuleRecord[], out JsBinding[])` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.WasmDecoder(System.ReadOnlySpan<byte>, in VmReadBounds, WasmReadAdapter, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecode(out WasmModule?, out VmVerifierOutcome)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadPreamble()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadOneSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryAdmitSectionPosition(WasmSectionId, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeTypeSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeImportSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeTableSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeMemorySection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeExportSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeElementSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeCodeSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeDataSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryCheckSectionAgreement()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadValueTypeVector(out WasmValueType[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadValueType(out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadLimits(out WasmLimits)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadName(out byte[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadConstantExpression(out WasmConstantExpression)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadLocals(out WasmValueType[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadCount(out uint)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryAllocateValues<TElement>(uint, out TElement[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryAllocateReferences<TElement>(uint, out TElement[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryParse(System.ReadOnlySpan<byte>, System.Span<WasmValue>, System.Span<WasmValueType>, out int, out int, out int, out WebAssemblyEntryPointProblem)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryReadCount(System.ReadOnlySpan<byte>, ref int, out uint)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryReadLiteral(WasmValueType, System.ReadOnlySpan<byte>, out WasmValue)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryReadInteger(System.ReadOnlySpan<byte>, out ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmFormat` in `src/Broiler.VM.Profile.WebAssembly/WasmFormat.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmFormat.OrderRankOf(WasmSectionId)` in `src/Broiler.VM.Profile.WebAssembly/WasmFormat.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmLabel` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmFrame` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmPacing` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmPacing.TryReserve(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmPacing.TryCharge(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmPacing.Observe(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Call(int, System.ReadOnlySpan<WasmValue>, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.TryPushFrame(int, out WasmRunStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.PopFrame(WasmFrame)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Run()` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Branch(WasmFrame, int, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.MemoryAccess(byte, System.ReadOnlySpan<byte>, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.ReadU32(System.ReadOnlySpan<byte>, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.ReadS64(System.ReadOnlySpan<byte>, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.TryNumeric(byte, out WasmTrapKind?)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Integer(byte, ref WasmTrapKind?)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Float(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.FloatComparison(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Convert(byte, ref WasmTrapKind?)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmLeb128` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadVarU32(ref VmBoundedReader, out uint, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadVarS32(ref VmBoundedReader, out int, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadBoundedCount(ref VmBoundedReader, WasmReadAdapter, out uint, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadUnsigned(ref VmBoundedReader, int, int, out ulong, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadSigned(ref VmBoundedReader, int, int, out long, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.ProfileMaximumPages` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.TryLoad(ulong, int, out ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.TryStore(ulong, int, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.TryInitialise(ulong, System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.Grow(uint, IVmMeter)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmJumpTarget` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmFunctionBody` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmFunctionBody.TrySeal(int, int, WasmJumpTarget[])` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmFunctionBody.TryFindJumpTarget(int, out WasmJumpTarget)` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmModule` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmModule.ExecutionBoundsComputed` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmModule.TryCopyExportName(int, System.Span<byte>, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmName` in `src/Broiler.VM.Profile.WebAssembly/WasmName.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmName.IsWellFormed(System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.WebAssembly/WasmName.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmName.Follows(System.ReadOnlySpan<byte>, int, int, byte, byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmName.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.Ceilings` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.WasmReadAdapter(IVmMeter, VmLimitVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.TryChargeDeclaredCount(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.TryChargeStructuralDepth(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.ReleaseStructuralDepth(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmTableInstance` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmTableInstance.TryRead(uint, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmTableInstance.TryInitialise(ulong, System.ReadOnlySpan<uint>)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmStore` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmStore.TryAllocate(WasmModule, IVmMeter, out WasmStore?, out bool)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmStore.ReleasePartial(WasmMemoryInstance?[], WasmTableInstance?[], IVmMeter)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmStore.ProfileMaximumTableEntries` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmStore.Release(IVmMeter)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmInstance` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmContinuation` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmTrapKind` in `src/Broiler.VM.Profile.WebAssembly/WasmTrapKind.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmFuncType` in `src/Broiler.VM.Profile.WebAssembly/WasmTypes.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmControlFrame` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.BottomType` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.InstructionsBetweenPolls` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.WasmValidator(WasmModule, WasmReadAdapter, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidate(out VmVerifierOutcome)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateModule()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateFunctionTypes()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateStartFunction()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateExports()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateGlobals()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateElementSegments()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateDataSegments()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryConstantExpressionType(WasmConstantExpression, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateBody(int)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.OpenReaderOver(System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TrySealBody(WasmFunctionBody)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryStep()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.RefuseOpcode(byte, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryOpenBlock(byte, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryElse(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryEnd()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryBranch(bool)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryBranchTable()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReturn()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryCall()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryCallIndirect()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryLocal(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryGlobal(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryMemoryQuery(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryMemoryAccess(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryNumeric(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryNumericSignature(byte, out WasmValueType, out int, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TrySelect()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PushOperand(WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperand(WebAssemblyDiagnosticCode, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperand(WasmValueType, WebAssemblyDiagnosticCode, WebAssemblyDiagnosticCode, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperands(WasmTypeVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperandSpan(System.ReadOnlySpan<WasmValueType>)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopThenPush(WasmTypeVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PushControl(byte, WasmTypeVector, WasmTypeVector, int, int)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopControl(out WasmControlFrame)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.LabelTypes(in WasmControlFrame)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.MarkUnreachable()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReadBlockType(out WasmTypeVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReadReservedZero()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReadCount(out uint)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReserveScratch()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.ReleaseScratch()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryGrow<TElement>(ref TElement[], ref ulong, int, ulong, VmBudgetDimension)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryAppendJump(int, int, int, int, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryPoll()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValue` in `src/Broiler.VM.Profile.WebAssembly/WasmValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValue.FromF32(float)` in `src/Broiler.VM.Profile.WebAssembly/WasmValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmValue.FromF64(double)` in `src/Broiler.VM.Profile.WebAssembly/WasmValue.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmRefusal` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmRefusal.FromReader(VmBoundedReadStatus, VmSourcePosition)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WasmRefusal.FromVarInt(WasmVarIntStatus, VmBoundedReadStatus, VmSourcePosition)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Instantiate(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Invoke(IVmInstanceState, in VmInvocationRequest, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Unwind(IVmProfileContinuation, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.InstantiateCore(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.InvokeCore(IVmInstanceState, in VmInvocationRequest)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Faulted(WasmTrapKind, int, int)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyValue` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyPayloads.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyTrap` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyPayloads.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.MaxUnchargedWork` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.Build()` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.Defaults()` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.Matrix()` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier.PollGranularity` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=High, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=Critical, human line PENDING
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier.VerifyCore(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=Critical, human line PENDING
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
| Units carrying a criterion | 633 |
| Units required to carry one | 584 |
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
| TrivialPropertyOrAccessor | 829 |
| ParameterAssigningConstructor | 95 |
| TrivialExpressionBodiedMember | 42 |
| CompilerSuppliedRecordOrEnumMember | 6 |
| DelegatingOverrideOrOperator | 104 |
| InsideAssemblyMarker | 0 |
| FieldDeclaringStorage | 519 |
| EnumMemberOfADeclaredVocabulary | 1276 |
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
6395 of them, exempt and relevant alike - with the fingerprint of its declaration.
This manifest is a change-detection record, not a review. A unit listed there is watched, not reviewed:
the entry records what the declaration's tokens hashed to when the generator last ran, and
nothing else. Exempt units still need no annotation and carry none, and no human line in
this component has moved off `PENDING`. What the manifest adds is that a unit the exemption
predicate treats as trivial is no longer invisible: a semantic change to one moves a value
in a generated file the gate compares byte for byte. Rule J7 holds the manifest to the tree.

Beside the units it lists **every covered file** - 165 of them - with a
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
