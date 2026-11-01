# Writes rows-O/probes/o2-collationtest-<kind>.js from Broiler.VM's archived CollationTest files at 83290f9
# (src/tests/cldr/pins/cldr-48.2.0/common/uca/), as the slice compiler's check
# intl/i1/the-root-collation-orders-collationtest-<kind> reads them (IntlChecks.cs ConformanceFile): every
# line that is not empty and not a comment, its code points as \u{...} escapes, compared in order under
# `new Intl.Collator('en', <options>)`. The probe prints how many consecutive pairs compare above 0, and the
# first 25 such pairs by line index. The lines are held in one string literal, each line's length in
# UTF-16 code units in a second (one code unit per line, the length plus 0x20), so the program holds two
# constants: Broiler.VM's constant pool holds at most 65,535. Usage: python3 -I make_collationtest_probes.py <Broiler.VM checkout> <probes dir>
import hashlib, pathlib, sys
root, out = pathlib.Path(sys.argv[1]), pathlib.Path(sys.argv[2])
for kind, name, options in [("non-ignorable", "CollationTest_CLDR_NON_IGNORABLE_SHORT.txt", "{ sensitivity: 'variant' }"),
                            ("shifted", "CollationTest_CLDR_SHIFTED_SHORT.txt", "{ sensitivity: 'variant', ignorePunctuation: true }")]:
    path = root / "src/tests/cldr/pins/cldr-48.2.0/common/uca" / name
    data = path.read_bytes()
    text, lens, count = [], [], 0
    for raw in data.decode("utf-8").splitlines():
        if not raw or raw[0] == "#":
            continue
        cps = [int(p, 16) for p in raw.split()]
        text.append("".join("\\u{%X}" % c for c in cps))
        lens.append("\\u%04X" % (sum(2 if c > 0xFFFF else 1 for c in cps) + 0x20))
        count += 1
    body = ("// vm-fuel: 2000000000000\n"
            f"// O2: {name} (sha256 {hashlib.sha256(data).hexdigest()}), every consecutive pair under {options}.\n"
            f"var c = new Intl.Collator('en', {options});\n"
            "var all = '" + "".join(text) + "';\n"
            "var lens = '" + "".join(lens) + "';\n"
            "var s = [], at = 0;\n"
            "for (var k = 0; k < lens.length; k++) { var n = lens.charCodeAt(k) - 0x20; s.push(all.substring(at, at + n)); at += n; }\n"
            "var bad = 0, first = [];\n"
            "for (var i = 1; i < s.length; i++) { if (c.compare(s[i - 1], s[i]) > 0) { bad++; if (first.length < 25) first.push(i); } }\n"
            "out('o2 pairs ' + (s.length - 1) + ' out-of-order ' + bad);\n"
            "first.forEach(function (i) { out('o2 line ' + i + ': ' + q(s[i - 1]) + ' > ' + q(s[i])); });\n")
    (out / f"o2-collationtest-{kind}.js").write_text(body, encoding="utf-8")
    print(kind, count)
