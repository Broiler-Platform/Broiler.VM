# The differential probes

**Owner:** JavaScript profile owner. **Reviewer:** none.

Probes over the surface `broiler.javascript.wide` admits, run through the end-user host by
[`eng/run-differential.py`](../../../eng/run-differential.py). Each probe prints one numbered line
per case; each has a `.expected.txt` beside it holding what this build answered.

## Why these exist, and what they are not

**They are not a conformance claim.** Conformance is the pinned third-party suite's question and
the harness under `src/compositions/Broiler.VM.Composition.JavaScript.Conformance` asks it. These
are a bring-up instrument: a few hundred cases, written from what the language says, run against a
second engine to find out where this one disagrees.

**They exist because a rule over names cannot see a wrong method.** Rule N17 compares the set of
globals this realm publishes against the set any document claims is absent, and it passes. It has
nothing to say about a prototype that is missing six methods, an argument count that is wrong, or a
method that reads the array-like protocol where the language says the iteration protocol — all of
which were true of this realm while N17 was green, and all of which the first run of
`the-general-surface.js` found *(recorded as [JSC-91](../../Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-91))*.

## The two comparisons, and why both are needed

`eng/run-differential.py` always compares against the retained answers. That is a **regression**
check: it needs nothing but the built host, and it asserts that this build answers what the build
that retained the file answered.

Given `--against <engine>` it also runs each probe under that engine. That is the question the
retained file cannot answer — whether the answers are **right**. A retained file agrees with itself
by construction, and a component whose only oracle is its own previous output produces its own
claims about JavaScript rather than conformance, which is what bundle JS-4-001 records of every
fixture written here.

## Declared divergences are data

An answer file may carry `#diverges <case> <reason>` lines. They are authored, not generated, and
`--write` carries them forward.

- A case named there and differing is reported as a **declared divergence**.
- A case not named there and differing is a **finding**, and the driver exits non-zero.
- A case named there and **not** differing is a **stale declaration**, and the driver exits non-zero
  as well — a declaration nobody removed is a claim about the code that has stopped being true.

All three directions have been watched: doctoring a probe so this build answers differently reports
the case against the retained file; removing a `#diverges` line reports the case as undeclared;
adding one for a case that agrees reports it as stale.

## What is here

Each probe is a surface, and the file's name says which. A probe is APPENDED to and never
renumbered: a case number is how a divergence is named, so inserting one would silently move every
declaration that points at another.

| Probe | What it covers |
|---|---|
| `the-general-surface.js` | The realm as a whole: coercion, the globals, the error objects, the `Object` statics, function objects, the Array, and the name a function takes from what it is bound to |
| `the-statement-and-object-surface.js` | Hoisting and closure capture, labels, `switch`, `try` in every combination, property descriptors and own-key order, accessors, `this`, holes, number formatting, strict mode, the class body, and the two members of an object literal that are not property definitions |
| `the-later-library-methods.js` | The keyed collections and their set operations, the views, `JSON`, the promise as a value, and the members the language added after the first pass over each of them |
| `the-json-date-and-regexp-surface.js` | The pattern protocol, the matcher, the instant, and the number at its edges |
| `the-seam-between-generators-and-the-rest.js` | Where the suspension machinery meets everything merged beside it |
| `the-async-family.js` | `async` functions and `await`: what runs synchronously, what a turn is, and how a rejection travels |
| `the-settling-of-promises.js` | The combinators over the receiver, the capability every static builds through, and the turn a reaction runs on |
| `the-with-statement.js` | The object environment record, and the names a `with` body resolves through it |

## Running them

```bash
dotnet build Broiler.VM.slnx -c Release
python3 eng/run-differential.py                                  # against the retained answers
python3 eng/run-differential.py --against /path/to/comparison    # and against a second engine
python3 eng/run-differential.py --write                          # retain what this build answered
```

`--write` is for after a deliberate change. Read the diff it produces before keeping it: a probe
whose answers moved because a repair landed and a probe whose answers moved because something broke
look identical in the file and different in the diff.
