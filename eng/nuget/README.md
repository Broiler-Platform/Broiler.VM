# Broiler.VM

Broiler.VM is a Native AOT-compatible host for verified bytecode artifacts. It is a **host for
language profiles, not a language**: the core owns profile selection, bounded loading, the
verification boundary, the execution lifecycle, resource authority and diagnostics, and owns no
opcode set, value representation or language semantics of its own. Profiles are registered by a
direct, typed reference; nothing is discovered by scanning assemblies or loading types by name.

> **Preview.** These packages are an early preview. The public API can change between previews,
> no milestone has been accepted, and no human review has taken place yet. Do not use them for
> anything that needs a stability or security guarantee.

## Packages

All packages are versioned in lockstep and target `net10.0`.

| Package | What it is |
|---|---|
| `Broiler.VM.Abstractions` | Profile-neutral contracts: descriptors, identities, limits, budgets, results and diagnostics |
| `Broiler.VM.Binary` | Bounded binary reading and allocation guards shared by the core and every profile verifier |
| `Broiler.VM.Runtime` | The catalog, the runtime and its lifecycle, resource authority and guest-load mediation |
| `Broiler.VM.Profile.JavaScript` | The JavaScript profile: verifier, executor, object model and standard library |
| `Broiler.VM.Profile.JavaScript.Compiler` | JavaScript source to profile bytecode |
| `Broiler.VM.Profile.JavaScript.Format` | The JavaScript profile's bytecode format: opcodes, encoder and decoder |
| `Broiler.VM.Profile.WebAssembly` | The WebAssembly profile: decoder, validator and interpreter |
| `Broiler.VM.Profile.MachineCode` | The native output profile: native page arming and machine-code execution |

## Getting started

A host needs the runtime, plus the profile it wants to run:

```shell
dotnet add package Broiler.VM.Runtime --prerelease
dotnet add package Broiler.VM.Profile.JavaScript --prerelease
dotnet add package Broiler.VM.Profile.JavaScript.Compiler --prerelease
```

A custom profile needs only `Broiler.VM.Abstractions` and `Broiler.VM.Binary`. The
[feed consumer sample](https://github.com/Broiler-Platform/Broiler.VM/tree/main/samples/Broiler.VM.Sample.FeedConsumer)
writes a small profile against those two packages and runs it through `Broiler.VM.Runtime`,
under JIT, trimming and Native AOT.

## Links

- [Source and documentation](https://github.com/Broiler-Platform/Broiler.VM)
- [Status ledger](https://github.com/Broiler-Platform/Broiler.VM/blob/main/docs/roadmap.status.md): what has and has not been demonstrated
- [Support matrix](https://github.com/Broiler-Platform/Broiler.VM/blob/main/docs/support.md)
- [Issues](https://github.com/Broiler-Platform/Broiler.VM/issues)

Licensed under Apache-2.0. Third-party notices are included in each package.
