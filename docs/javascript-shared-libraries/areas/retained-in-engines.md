# What stays in each engine, and why

**Status:** design. This document lists the responsibilities the programme deliberately does **not**
share, so that a reader does not mistake an absence from the catalogue for an oversight. Each row
gives the reason, and the condition under which the row would be reconsidered.

The feasibility report's own words set the boundary: "The value model, object model and execution
backend should remain separate initially. This is the strongest limitation on how far
straightforward sharing can go."

---

## 1. Execution machinery

| Responsibility | Broiler.VM | Broiler.JS | Why it stays | Reconsidered when |
|---|---|---|---|---|
| Value representation | Internal tagged `JsValue` struct (a type byte, a double and an object reference) | Public abstract `JSValue` class hierarchy, with factory delegates, dynamic-object integration and run-time callbacks | A shared value interface would change allocation, dispatch, identity, exceptions and performance in both engines. It would make no built-in reusable by itself. Broiler.VM's JSD-0011 rejects "Adopting the seed's boxed value hierarchy". | Never, by this programme. Decision [SJL-D-18](../decisions.md#sjl-d-18) records it as **not planned**. |
| Object storage, shapes, property maps | Its own | Its own (`Storage`) | Engine-specific optimisation strategies | As above |
| Frames, dispatch, suspension, generators, async | Its interpreter and native forms | Its expression-tree compiler and runtime | The execution backend is what distinguishes the engines | As above |
| Compiler back ends and emitted forms | Bytecode, the universal bytecode programme, native forms | IL through expression trees; the portable bytecode island | Back-end decisions | As above |
| Verification, resource budgets, cancellation, continuations | Its own (core contract) | None | Broiler.VM's core contract and its rules | (none) |
| Realm and agent lifecycle, host integration, capability policy | Its composition model | Its bootstrap profiles and registries | Host policy | (none) |

## 2. Built-in object implementations

These stay. The algorithms underneath them move.

| Responsibility | Why it stays |
|---|---|
| `Array`, `Promise`, `Proxy`, `Reflect`, collections, iterators, typed arrays and `ArrayBuffer`, `Atomics`, `WeakRef` and `FinalizationRegistry`, `ShadowRealm` | They are object-model code. Their observable behaviour is property access, ordering and identity. Sharing them would require a shared object model. |
| The `JSON.parse` and `JSON.stringify` drivers | Their steps can be observed (reviver, `toJSON`, replacer, proxies, getters), and they build engine objects. Only the lexical kernels are shared ([text.md](text.md)). |
| The `RegExp` object, `lastIndex`, the `Symbol` protocols (`match`, `matchAll`, `replace`, `search`, `split`), species | Object protocols. Only the engine underneath is shared ([regexp.md](regexp.md)). |
| `split` and `replace` drivers, `GetSubstitution`'s evaluation | Named-capture `Get` is observable. Only template parsing is shared. |
| Every Intl and Temporal object, its prototype and its option reading | Option reading is observable and specification-ordered. The algorithms below take resolved records. |
| `Error` objects, stack text | Engine-specific (Broiler.VM's JSD-0038 defines its own) |
| `Function.prototype.toString` source text | Depends on each engine's source retention (Broiler.VM's source-text section) |
| String search (`indexOf`, `includes`, `startsWith`, `endsWith`) | The algorithm is the ordinal base-class-library call. Broiler.JS's culture-sensitive calls are a Broiler.JS correction, not a shared library. |
| `Math.random` | State and seeding are policy |

## 3. Policy that each engine supplies through a provider

| Policy | Broiler.VM | Broiler.JS | Decision |
|---|---|---|---|
| `Date`'s local time zone | UTC | The host's `TimeZoneInfo.Local` | [SJL-D-19](../decisions.md#sjl-d-19) |
| Clock and its resolution | The host clock, at millisecond resolution for `Temporal.Now` | The host clock, at 100-nanosecond resolution | SJL-D-19 |
| Default locale | Fixed `en-US` | The host culture | SJL-D-19 |
| Available locales | Its data | The host's cultures, optionally | [SJL-D-22](../decisions.md#sjl-d-22) |
| BigInt width ceiling | Its constant | Unbounded, under [SJL-D-15](../decisions.md#sjl-d-15) (a) | SJL-D-15 |
| String length ceiling, JSON depth, regular-expression frames and trail | Its constants | Its own choices | Passed as limits |
| Metering | Fuel, cancellation and the wall clock, through its charge | None, or its own | [SJL-D-10](../decisions.md#sjl-d-10) |

## 4. Broiler.JS's non-standard extensions

These stay in Broiler.JS under decision [SJL-D-17](../decisions.md#sjl-d-17):

- the `Decimal` type, `m` literals and `typeof` `"decimal"`;
- `Number.prototype.clz`;
- `String.prototype.contains`;
- the .NET format-string extensions of `Date` and `Number` `toLocaleString`;
- the SpiderMonkey-style `N/N/N` date heuristics, and the culture-dependent legacy `Date.parse`;
- the legacy `RegExp` statics;
- CLR interop for `DateTime` and `DateTimeOffset`.

## 5. Responsibilities only one engine has

These are not duplication. Some could be adopted by the other engine later, through the shared
library, and that adoption is future work, not this programme's. Each area's verdict records them as
**adoptions**, under extraction-gate condition X1, wherever an area moves them anyway.

**Broiler.VM only:**

- metering and limits throughout;
- deterministic host policy;
- data optional at deployment time;
- pins and generators with byte-for-byte rules;
- full UAX #29 word and sentence segmentation;
- UCA collation;
- a `localeCompare` without Intl;
- exact property names;
- Pattern_Syntax-correct identifiers;
- compile-time regular-expression early errors from the run-time parser;
- the Intl reference datasets and the Temporal polyfill dataset;
- canonical Float16 NaN, and explicit little-endian typed arrays;
- divide-and-conquer BigInt text;
- the CLDR metazone history, golden zones and partial-location zone names;
- the ICU-model pattern generator and interval formats;
- `DisplayNames` data;
- the `fractional` duration style.

**Broiler.JS only:**

- ES2025 duplicate named groups (through its .NET translator; SJL-11 keeps them);
- host-wide locale coverage through ICU;
- 100-nanosecond `Temporal.Now`;
- backzone history before 1970, and the `Factory` zone, until SJL-8 drops both under
  [SJL-D-20](../decisions.md#sjl-d-20), each as a listed correction;
- compact-notation data for a few more locales;
- the CLR interop surface;
- the non-standard extensions of section 4.

**Neither engine:** dictionary-based or locale-tailored segmentation.
