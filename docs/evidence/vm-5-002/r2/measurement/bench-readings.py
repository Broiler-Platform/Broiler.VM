"""Section 5.11's readings of the bench host's two pairs in VM-5-002's re-collection.

    python bench-readings.py <e13 dir>

Item 4, both readings, both pairs, on meter-per-instruction: lane total (credit candidate-ns at or
below base candidate-ns plus base aa-ns) and per instruction (credit per-instruction-ns at or below
base per-instruction-ns plus base aa-ns / 256). Item 5, reported without a verdict: host-call credit
per-call-ns against base per-call-ns plus base aa-ns. Every figure is copied from the measurement line
of the named file; the last lines give each run's measurement count and invalid count.
"""
import pathlib
import re
import sys

d = pathlib.Path(sys.argv[1])
PAIRS = [("1, base first", "bench-base.log", "bench-credit.log"),
         ("2, credit first", "bench-base-run2.log", "bench-credit-run2.log")]


def row(path, name):
    text = (d / path).read_text(encoding="utf-8")
    m = re.search(r"^measurement %s (.*)$" % re.escape(name), text, re.M)
    fields = dict(kv.split("=", 1) for kv in m.group(1).split())
    tail = re.search(r"^# elapsed-ms=\S+ measurements=(\d+) invalid=(\d+)", text, re.M)
    return fields, (tail.group(1), tail.group(2)) if tail else ("?", "?")


all_hold = True
for label, base_log, credit_log in PAIRS:
    b, bt = row(base_log, "meter-per-instruction")
    c, ct = row(credit_log, "meter-per-instruction")
    bc, ba, bp = float(b["candidate-ns"]), float(b["aa-ns"]), float(b["per-instruction-ns"])
    cc, cp = float(c["candidate-ns"]), float(c["per-instruction-ns"])
    lane = cc <= bc + ba
    per = cp <= bp + ba / 256
    all_hold &= lane and per
    print("pair %s: meter-per-instruction" % label)
    print("  %s: candidate-ns=%s per-instruction-ns=%s aa-ns=%s valid=%s (measurements=%s invalid=%s)"
          % (base_log, b["candidate-ns"], b["per-instruction-ns"], b["aa-ns"], b["valid"], *bt))
    print("  %s: candidate-ns=%s per-instruction-ns=%s aa-ns=%s valid=%s (measurements=%s invalid=%s)"
          % (credit_log, c["candidate-ns"], c["per-instruction-ns"], c["aa-ns"], c["valid"], *ct))
    print("  item 4, lane total: credit %.1f <= base %.1f + base aa %.1f = %.1f -> %s" % (cc, bc, ba, bc + ba, "holds" if lane else "FAILS"))
    print("  item 4, per instruction: credit %.4f <= base %.4f + base aa %.1f / 256 = %.4f -> %s" % (cp, bp, ba, bp + ba / 256, "holds" if per else "FAILS"))
    hb, _ = row(base_log, "host-call")
    hc, _ = row(credit_log, "host-call")
    hbp, hba, hcp = float(hb["per-call-ns"]), float(hb["aa-ns"]), float(hc["per-call-ns"])
    print("  host-call: base per-call-ns=%s aa-ns=%s candidate-ns=%s; credit per-call-ns=%s aa-ns=%s candidate-ns=%s"
          % (hb["per-call-ns"], hb["aa-ns"], hb["candidate-ns"], hc["per-call-ns"], hc["aa-ns"], hc["candidate-ns"]))
    print("  item 5 reading, reported without a verdict: credit %.4f %s base %.4f + base aa %.1f = %.4f"
          % (hcp, "<=" if hcp <= hbp + hba else ">", hbp, hba, hbp + hba))
print("item 4, both readings in both pairs: %s" % ("hold" if all_hold else "DO NOT all hold"))
