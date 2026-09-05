"""The refusal audit: every syntactic position each construct family can be written in.

WHY THIS EXISTS AND WHY IT IS NOT THE ACCEPTANCE TABLE. `src/tests/cli/expected.txt` pins one
answer per source and is the regression gate; this asks a different question, over a matrix rather
than a list. Bundle JS-4-001 section 4 recorded what it is protecting against: a construct refused
in the position somebody remembered and reaching an UNEXPECTED-TOKEN diagnostic in a position
nobody did. A conformance runner scores the first as `unsupported` and the second as a failure, so
the two look identical from the outside and are not.

IT ASKS BOTH DIRECTIONS, WHICH IS THE HALF A LIST CANNOT. A family this manifest declines must
answer `2104:ConstructOutsideManifest` naming ITS OWN construct in every position it can appear in.
A family the manifest admits must answer `2104` in NONE of them - which is what catches a construct
admitted in the position its author tested and still refused one nesting level down.

Run it after admitting a family, with the host already built:

    dotnet build Broiler.VM.slnx -c Release
    python3 eng/audit-refusals.py
"""

import argparse
import io
import os
import subprocess
import sys
import tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
BINARY = os.path.join(
    "src", "compositions", "Broiler.VM.Composition.JavaScript.Cli", "bin", "Release", "net10.0",
    "Broiler.VM.Composition.JavaScript.Cli")

REFUSAL = "2104:ConstructOutsideManifest"

# (name, source, expectation)
#   "admitted"             the host must not answer 2104 anywhere in its output
#   "refused:<construct>"  the host must answer 2104 naming exactly that construct
CASES = [
    # ---- class fields, in every position a field can be written ------------------------------
    ("field bare", "class C { x; }", "admitted"),
    ("field with an initialiser", "class C { x = 1; }", "admitted"),
    ("field static", "class C { static x = 1; }", "admitted"),
    ("field with a computed key", "class C { ['k'] = 1; }", "admitted"),
    ("field with a string key", "class C { 'a b' = 1; }", "admitted"),
    ("field with a numeric key", "class C { 0 = 1; }", "admitted"),
    ("field named get", "class C { get = 1; }", "admitted"),
    ("field named set", "class C { set = 1; }", "admitted"),
    ("field named static", "class C { static = 1; }", "admitted"),
    ("field named async", "class C { async = 1; }", "admitted"),
    ("field in a class expression", "var C = class { x = 1; };", "admitted"),
    ("field in a nested class", "class C { m() { return class { x = 1; }; } }", "admitted"),
    ("field in a parameter default", "function f(C = class { x = 1; }) { return C; }", "admitted"),
    ("field initialiser holding a class", "class C { x = class { y = 1; }; }", "admitted"),
    ("field initialiser holding an arrow", "class C { x = () => this; }", "admitted"),
    ("field initialiser holding a generator",
     "class C { x = function* () { yield 1; }; }", "admitted"),

    # ---- class static blocks ------------------------------------------------------------------
    ("static block", "class C { static { 1; } }", "admitted"),
    ("static block, empty", "class C { static {} }", "admitted"),
    ("static block in a class expression", "var C = class { static { 1; } };", "admitted"),
    ("static block twice", "class C { static { 1; } static { 2; } }", "admitted"),
    ("static block holding a class", "class C { static { class D { x = 1; } } }", "admitted"),
    ("static block after a field", "class C { static a = 1; static { this.b = 2; } }", "admitted"),

    # ---- private names, declared and used ------------------------------------------------------
    ("private field", "class C { #x; }", "admitted"),
    ("private field with an initialiser", "class C { #x = 1; }", "admitted"),
    ("private static field", "class C { static #x = 1; }", "admitted"),
    ("private method", "class C { #m() { return 1; } }", "admitted"),
    ("private static method", "class C { static #m() { return 1; } }", "admitted"),
    ("private generator method", "class C { *#m() { yield 1; } }", "admitted"),
    ("private async method", "class C { async #m() { return 1; } }", "admitted"),
    ("private getter", "class C { get #a() { return 1; } }", "admitted"),
    ("private setter", "class C { set #a(v) {} }", "admitted"),
    ("private read through this", "class C { #x = 1; m() { return this.#x; } }", "admitted"),
    ("private read through an argument", "class C { #x = 1; m(o) { return o.#x; } }", "admitted"),
    ("private read through an optional chain",
     "class C { #x = 1; m(o) { return o?.#x; } }", "admitted"),
    ("private write", "class C { #x = 1; m() { this.#x = 2; } }", "admitted"),
    ("private compound write", "class C { #x = 1; m() { this.#x += 2; } }", "admitted"),
    ("private update", "class C { #x = 1; m() { return this.#x++; } }", "admitted"),
    ("private call", "class C { #m() {} run() { return this.#m(); } }", "admitted"),
    ("private brand check", "class C { #x; static h(o) { return #x in o; } }", "admitted"),
    ("private brand check in a conditional",
     "class C { #x; static h(o) { return #x in o ? 1 : 2; } }", "admitted"),
    ("private object-destructuring target",
     "class C { #x; m(o) { ({ a: o.#x } = { a: 1 }); } }", "admitted"),
    ("private array-destructuring target", "class C { #x; m(o) { [o.#x] = [1]; } }", "admitted"),
    ("private name in a field initialiser", "class C { #x = 1; y = this.#x; }", "admitted"),
    ("private name in a static block",
     "class C { static #x = 1; static { this.y = C.#x; } }", "admitted"),
    ("private name in a nested arrow",
     "class C { #x = 1; m() { return () => this.#x; } }", "admitted"),
    ("private name in a nested class body",
     "class C { #x = 1; m() { class D { #y = 2; } return D; } }", "admitted"),
    ("private name in an anonymous class",
     "var C = class { #x = 1; m() { return this.#x; } };", "admitted"),

    # ---- a generator member of a class body ----------------------------------------------------
    ("class generator method", "class C { *m() { yield 1; } }", "admitted"),
    ("class static generator method", "class C { static *m() { yield 1; } }", "admitted"),
    ("class generator method with a computed key", "class C { *['k']() { yield 1; } }", "admitted"),
    ("class generator method in a class expression",
     "var C = class { *m() { yield 1; } };", "admitted"),
    ("class generator method with super",
     "class B {} class D extends B { *m() { yield 1; } }", "admitted"),

    # ---- what stays refused, in every position it can appear in --------------------------------
    ("async generator function declaration",
     "async function* g() { yield 1; }", "refused:an async generator function"),
    ("async generator function expression",
     "var g = async function* () { yield 1; };", "refused:an async generator function"),
    ("async generator method of a class body",
     "class C { async *m() { yield 1; } }", "refused:an async generator method"),
    ("async generator static method of a class body",
     "class C { static async *m() { yield 1; } }", "refused:an async generator method"),
    ("async generator private method of a class body",
     "class C { async *#m() { yield 1; } }", "refused:an async generator method"),
    ("async generator method of an object literal",
     "var o = { async *m() { yield 1; } };", "refused:an async generator method"),
    ("async generator method of a class expression",
     "var C = class { async *m() { yield 1; } };", "refused:an async generator method"),
    ("async generator method with a computed key",
     "class C { async *['k']() { yield 1; } }", "refused:an async generator method"),
    ("async generator in a class field initialiser",
     "class C { x = async function* () { yield 1; }; }", "refused:an async generator function"),
    ("async generator in a class static block",
     "class C { static { async function* g() { yield 1; } } }",
     "refused:an async generator function"),
    ("for await in an async function",
     "async function f(xs) { for await (var x of xs) { x; } }", "refused:`for await`"),
    ("for await with a lexical head",
     "async function f(xs) { for await (let x of xs) { x; } }", "refused:`for await`"),
    ("for await in an async method of a class body",
     "class C { async m(xs) { for await (var x of xs) { x; } } }", "refused:`for await`"),
    ("for await in an async arrow",
     "var f = async (xs) => { for await (var x of xs) { x; } };", "refused:`for await`"),
    ("decorator on a class", "@dec class C {}", "refused:a decorator"),
    ("decorator on a class member", "class C { @dec m() {} }", "refused:a decorator"),
]


def answer(binary, source, directory):
    """What the host says about one source, both streams together."""
    path = os.path.join(directory, "case.js")
    io.open(path, "w", encoding="utf-8", newline="\n").write(source + "\n")
    done = subprocess.run([binary, path], capture_output=True, text=True)
    return (done.stdout + done.stderr).strip()


def judge(name, source, expected, given):
    """The complaint this case earns, or None."""
    if expected == "admitted":
        if REFUSAL in given:
            return f"{name}: admitted, and the host refused it against the manifest"

        return None

    construct = expected.split(":", 1)[1]

    if REFUSAL not in given:
        return f"{name}: must be refused as `{construct}` and was not refused at all"

    if construct + " is not admitted" not in given:
        return f"{name}: refused, and the diagnostic does not name `{construct}`"

    return None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--binary", default=os.path.join(ROOT, BINARY))
    parser.add_argument("--verbose", action="store_true")
    options = parser.parse_args()

    if not os.path.exists(options.binary):
        raise SystemExit(
            f"{options.binary} is not there: build the solution in Release first")

    complaints = []

    with tempfile.TemporaryDirectory(prefix="broiler-audit-") as directory:
        for name, source, expected in CASES:
            given = answer(options.binary, source, directory)
            complaint = judge(name, source, expected, given)

            if complaint is not None:
                complaints.append(complaint)
                print(f"FAIL {complaint}")
                print(f"     source  {source}")
                print(f"     answer  {given.splitlines()[0] if given else '(nothing)'}")
            elif options.verbose:
                print(f"ok   {name}")

    admitted = sum(1 for case in CASES if case[2] == "admitted")
    print(
        f"# {len(CASES)} positions audited: {admitted} that must not be refused, "
        f"{len(CASES) - admitted} that must be refused by name")

    if complaints:
        print(f"# {len(complaints)} of them answered wrongly")
        return 1

    print("# every position answered as declared")
    return 0


if __name__ == "__main__":
    sys.exit(main())
