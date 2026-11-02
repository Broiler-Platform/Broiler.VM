# Writes rows-O/probes/o15-orderings.js: Broiler.VM's src/tests/cldr/orderings/orderings.js at 83290f9, byte for
# byte, evaluated by indirect eval as SJL-9.1's dataset probes did (bundle sjl-9-001, probes/*/d-<name>.js), its
# completion value printed. Usage: python3 -I make_orderings_probe.py <Broiler.VM checkout> <probes dir>
import hashlib, json, pathlib, sys
src = (pathlib.Path(sys.argv[1]) / "src/tests/cldr/orderings/orderings.js").read_bytes()
body = (f"// O15: src/tests/cldr/orderings/orderings.js (sha256 {hashlib.sha256(src).hexdigest()}), by indirect eval.\n"
        "var program = " + json.dumps(src.decode("utf-8"), ensure_ascii=True) + ";\n"
        "out((0, eval)(program));\n")
(pathlib.Path(sys.argv[2]) / "o15-orderings.js").write_text(body, encoding="utf-8")
