#!/usr/bin/env python3
"""E7 parity rule, applied by hand per form.

The sorted result rows of base and credit are identical, except on rows in that
form's predeclared WallClock list, where the credit row may be Passed or an
exhaustion of another allowance.  Every other difference fails, including a
credit row exhausted on WallClock that is not in the list, and a listed row that
became Failed.
"""
import sys

PREDECLARED = {
    ("test/staging/sm/regress/regress-1507322-deep-weakmap.js", "sloppy"),
    ("test/staging/sm/regress/regress-1507322-deep-weakmap.js", "strict"),
}


def rows(path):
    out = {}
    with open(path, "r", encoding="utf-8", newline="") as fh:
        for line in fh:
            if not line.startswith("result|"):
                continue
            line = line.rstrip("\r\n")
            f = line.split("|")
            out[(f[1], f[2])] = (line, f[3], f[5])
    return out


def main(base_path, credit_path, form):
    base = rows(base_path)
    credit = rows(credit_path)
    print(f"form {form}")
    print(f"base rows {len(base)}  credit rows {len(credit)}")

    only_base = sorted(set(base) - set(credit))
    only_credit = sorted(set(credit) - set(base))
    for k in only_base:
        print(f"MISSING-IN-CREDIT {k[0]} [{k[1]}]")
    for k in only_credit:
        print(f"MISSING-IN-BASE {k[0]} [{k[1]}]")

    identical = 0
    admitted = []
    failures = []
    for k in sorted(set(base) & set(credit)):
        bline, bverdict, bdim = base[k]
        cline, cverdict, cdim = credit[k]
        if bline == cline:
            identical += 1
            continue
        if k in PREDECLARED:
            if cverdict == "Passed" or (cverdict == "Exhausted" and cdim != "WallClock"):
                admitted.append((k, bverdict, bdim, cverdict, cdim))
                continue
            failures.append((k, bverdict, bdim, cverdict, cdim, "listed row not Passed nor another allowance"))
        else:
            failures.append((k, bverdict, bdim, cverdict, cdim, "not in the predeclared list"))

    # a credit WallClock row that is not in the list, even if base also had one
    stray = []
    for k, (cline, cverdict, cdim) in credit.items():
        if cdim == "WallClock" and k not in PREDECLARED:
            stray.append(k)

    print(f"rows identical {identical}")
    print(f"rows differing, admitted by the list {len(admitted)}")
    for k, bv, bd, cv, cd in admitted:
        print(f"  ADMITTED {k[0]} [{k[1]}] base {bv}/{bd or '-'} -> credit {cv}/{cd or '-'}")
    print(f"rows differing, failing the rule {len(failures) + len(only_base) + len(only_credit)}")
    for k, bv, bd, cv, cd, why in failures:
        print(f"  FAIL {k[0]} [{k[1]}] base {bv}/{bd or '-'} -> credit {cv}/{cd or '-'} ({why})")
    print(f"credit WallClock rows outside the list {len(stray)}")
    for k in sorted(stray):
        print(f"  FAIL-STRAY-WALLCLOCK {k[0]} [{k[1]}]")

    verdict = "E7 PASSES" if not failures and not only_base and not only_credit and not stray else "E7 FAILS"
    print(verdict)
    return 0 if verdict == "E7 PASSES" else 1


if __name__ == "__main__":
    sys.exit(main(sys.argv[1], sys.argv[2], sys.argv[3]))
