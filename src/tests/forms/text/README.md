<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The text area's fuel-parity probes and JSON module documents

These are two populations of bundle [`sjl-5-001`](../../../../docs/evidence/sjl-5-001/decision-rule.md)'s
rule. Milestone SJL-5 of the shared JavaScript libraries programme moves this family's text algorithms -
well-formedness and code points, padding and relative indices, replacement templates, the URI codecs,
the base64 and hex codecs, and JSON's lexical grammar - into `Broiler.ECMAScript.Algorithms`
([JSD-0070](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0070-text-algorithms-move-to-the-shared-algorithms-library.md)),
and the shared members charge fuel through this family's meter. **Nothing here is timed.** No figure from
them is retained anywhere in this repository except in bundle `sjl-5-001`.

Both were written, and committed with the rule, before any code of SJL-5 existed.

## The probes (part B)

**They exist to show that the shared members charge what this family charges today**: each probe's
smallest completing fuel allowance is bisected at the milestone's base commit, and after the cut-over every
probe must complete at that figure and refuse at one less, in both output forms.

| Probe | What it charges |
|---|---|
| `trim-utf16.small.js` | `trim`, `trimStart` and `trimEnd` over every white-space character both engines already agree on; `isWellFormed` and `toWellFormed` over lone and paired surrogates; `codePointAt`, `at` and the string iterator over astral text |
| `pad-index.small.js` | `padStart`, `padEnd` and `repeat` well below the string ceiling; relative indices through String's `slice`, `substring`, `substr` and `at`, Array's `slice`, `at`, `fill`, `copyWithin` and `splice`, and a typed array's `slice`, `subarray`, `fill` and `at` |
| `substitution.small.js` | replacement templates with `$$`, `` $& ``, `` $` ``, `$'`, `$n`, `$nn` and `$<name>` through `replace` and `replaceAll`, with string and regular-expression patterns |
| `uri.small.js` | `encodeURI`, `encodeURIComponent`, `decodeURI` and `decodeURIComponent` over one- to four-octet text, and `escape` and `unescape` |
| `base64-hex.small.js` | `Uint8Array`'s base64 and hex codecs in both alphabets, every last-chunk mode, white space in the input, and `setFromBase64` and `setFromHex` into destinations shorter than the input |
| `json.small.js` | `JSON.parse` over nested documents, escapes and numbers, and `JSON.stringify`'s quoting and its gap from a number and from a string, with a replacer array |

**Every probe's answer is one the cut-over does not change.** SJL-5 corrects three of this family's
results (JSD-0070, X2): `padStart` and `padEnd` past the string ceiling, and `replaceAll`'s conversion of
a replacement when nothing matches. No probe goes near the ceiling or passes a replacement that is not a
string, so neither correction reaches it. No probe raises an error, reads an option that is not one of
the specification's names, or runs a getter, a reviver or `toJSON`. A probe whose answer changed after
the cut-over would be a defect of the probe or of a shared member, and part B reads it as a moved
verdict.

Every probe is deterministic, takes its counts as literals, and ends in an expression statement whose
value is the answer. Each compiles under `broiler.javascript.wide`, the command-line root's manifest,
and gives the same answer in both forms.

## The JSON module documents (part C)

[`json-modules/`](json-modules/) holds the documents part C compiles as JSON modules, each alone, with
the rule's harness, [`json-modules.cs`](../../../../docs/evidence/sjl-5-001/json-modules.cs), at the base
and after; the pinned test262 suite's own JSON fixtures join them. The harness prints each document's
outcome, the SHA-256 of its artifact and its first diagnostic's code, and part C holds every one of them
to the base.

| Documents | What they hold |
|---|---|
| `scalar-*.json` | each kind of top-level value that is not a container |
| `empty-containers.json`, `keys.json` | empty objects and arrays; `__proto__`, the empty key, keys that look like indices, a duplicated key and an escaped one |
| `numbers.json` | zero of both signs, the extremes of the doubles, `1e400` and `1e-400`, an integer past 2^53 and every exponent spelling |
| `escapes.json`, `raw-text.json` | every escape, escaped lone surrogates of both kinds and a pair, and raw text outside ASCII with U+2028 and U+2029 |
| `whitespace.json` | every white-space character JSON admits, between every pair of tokens |
| `nested-*.json` | nesting at the deepest this family admits a JSON module today, 61 levels (correspondence row J8), and one mixed |
| `large.json` | a few hundred objects |
| `refused-*.json` | documents refused today and so after: one level past the edge, the translation's own depth limit, and texts that are not JSON - a trailing comma, single quotes, a leading zero, a raw control character, hexadecimal, a comment, `NaN`, an empty text, two values, an unterminated object, a bad escape, a short `\u` and a JavaScript object literal - so that the refusal's code is compared too |

The documents are declared binary in `.gitattributes`: their bytes are the population, and
`whitespace.json` carries carriage returns.
