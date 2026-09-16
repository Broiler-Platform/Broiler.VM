"""E3 injected-defect witnesses, as run for bundle VM-5-002 at collection time.

Each witness is a local edit to src/Broiler.VM.Runtime in a worktree outside the repository,
applied by exact text replacement (every replacement must match the stated number of times, or
the witness is refused before anything runs), then the fuel-exactness tests run, then the tree is
restored with `git checkout -- src/Broiler.VM.Runtime`. Nothing here is ever committed.

    python witnesses.py <worktree> <witness> apply
    python witnesses.py <worktree> list
"""
import io
import json
import sys

M = "src/Broiler.VM.Runtime/VmMeter.cs"
R = "src/Broiler.VM.Runtime/VmRuntime.cs"
I = "src/Broiler.VM.Runtime/VmInstanceImplementation.cs"
P = "src/Broiler.VM.Runtime/VmFuelPreAdmissions.cs"
S = "src/Broiler.VM.Runtime/VmExecutionScope.cs"

S3 = (M, "                runtime.FuelPreAdmissions!.Settle(this);\n"
         "                return pollBound > 0 && sinceLastPoll > pollBound;",
         "                return pollBound > 0 && sinceLastPoll > pollBound;", 1)
S10 = (I, "            // As on the resume path: what the step spent from a block is committed before the\n"
          "            // outcome is mapped. The uncharged-work counter the mapping reads is exact jointly with\n"
          "            // the settle that reader takes itself, and the reader's is the one that must stay.\n"
          "            operation.Meter.SettlePreAdmittedFuel();\n",
          "", 1)
S11 = (I, "            // and the read.\n"
          "            operation.Meter.SettlePreAdmittedFuel();\n",
          "            // and the read.\n", 1)
S1_ALL = (M, "                if (!preAdmissions.IsEmpty && !preAdmissions.LeavesRoomFor(this, amount))\n"
             "                {\n"
             "                    contenders = preAdmissions.SettleAll();\n"
             "                }\n",
             "", 1)

# name -> (injected defect, [edits], tests that must fail)
WITNESSES = {
    "W1": ("S9 deleted: the SettleAll in the runtime's budget snapshot",
           [(R, "            runtimeLevel.FuelPreAdmissions!.SettleAll();\n\n"
                "            return runtimeLevel.Snapshot();",
                "            return runtimeLevel.Snapshot();", 1)],
           ["T4"]),
    "W2": ("S3, S10 and S11 deleted together: the whole step-end settle",
           [S3, S10, S11],
           ["T8 row 62 units"]),
    "W3": ("The settle in Poll deleted, and with it the re-admission it guards",
           [(M, "            var held = runtime.FuelPreAdmissions!.Settle(this);\n\n", "", 1),
            (M, "            if (held)\n"
                "            {\n"
                "                runtime.FuelPreAdmissions.PreAdmit(this, 0);\n"
                "            }\n", "", 1)],
           ["T8 row 10, 60, 60"]),
    "W4": ("LeavesRoomFor answers true",
           [(P, "    internal bool LeavesRoomFor(VmMeter meter, ulong amount) =>\n"
                "        amount <= Unreserved(meter.RuntimeLevel) &&\n"
                "        (meter.InstanceLevel is null || amount <= Unreserved(meter.InstanceLevel)) &&\n"
                "        amount <= Unreserved(meter.InvocationLevel);",
                "    internal bool LeavesRoomFor(VmMeter meter, ulong amount) => true;", 1)],
           ["T6"]),
    "W4b": ("S3, S10 and the SettleAll branch of S1 deleted",
            [S3, S10, S1_ALL],
            ["T2b"]),
    "W5": ("S4 deleted: the settle in RemainingSnapshot",
           [(M, "                runtime.FuelPreAdmissions!.Settle(this);\n"
                "                return invocation.AsRemainingVector();",
                "                return invocation.AsRemainingVector();", 1)],
           ["T9"]),
    "W6": ("The MayPreAdmit test in PreAdmit deleted",
           [(P, "        if (!meter.MayPreAdmit || meter.PreAdmissionSizeLocked != 0)",
                "        if (meter.PreAdmissionSizeLocked != 0)", 1)],
           ["T11"]),
    "W7": ("The fast-path head returns false instead of calling TryChargeLocked",
           [(M, "        return TryChargeLocked(dimension, amount);\n    }",
                "        return false;\n    }", 1)],
           ["T1"]),
    "W8": ("VmExecutionScope.Current returns the thread's resolved meter without comparing contexts",
           [(S, "                ReferenceEquals(resolvedScope, this) &&\n"
                "                ReferenceEquals(resolvedContext, context))",
                "                ReferenceEquals(resolvedScope, this))", 1)],
           ["T12", "T7"]),
    "W9": ("S5 deleted: the SettleAll before retention's admission check",
           [(M, "            // Every holder, not just this meter: a retention is admitted against the runtime and\n"
                "            // instance levels, which every operation of this runtime shares, so fuel any of them\n"
                "            // has spent and not yet committed is fuel this admission must already see.\n"
                "            if (dimension is VmBudgetDimension.Fuel)\n"
                "            {\n"
                "                runtime.FuelPreAdmissions!.SettleAll();\n"
                "            }\n\n", "", 1)],
           ["T14 row holders=1", "T14 row holders=2"]),
    "W9b": ("S5 settles only the charging meter instead of every holder",
            [(M, "            // has spent and not yet committed is fuel this admission must already see.\n"
                 "            if (dimension is VmBudgetDimension.Fuel)\n"
                 "            {\n"
                 "                runtime.FuelPreAdmissions!.SettleAll();\n"
                 "            }",
                 "            // has spent and not yet committed is fuel this admission must already see.\n"
                 "            if (dimension is VmBudgetDimension.Fuel)\n"
                 "            {\n"
                 "                runtime.FuelPreAdmissions!.Settle(this);\n"
                 "            }", 1)],
            ["T14 row holders=2"]),
    "W10": ("The dimension test removed from the fast-path head",
            [(M, "        if (dimension == VmBudgetDimension.Fuel && amount != 0)",
                 "        if (amount != 0)", 1)],
            ["T16"]),
    # Not in the design's table: the two arms commit c98011a added tests for, each named by that
    # commit as the defect its test catches.
    "W11": ("An eviction drops its victim's block instead of committing it",
            [(P, "            holders[0]!.CommitPreAdmittedFuelLocked();\n            RemoveAt(0);",
                 "            RemoveAt(0);", 1)],
            ["T17"]),
    "W12": ("A lookup with the flow suppressed is cached under the null context",
            [(S, "            if (context is not null &&\n                ReferenceEquals(resolvedScope, this) &&",
                 "            if (ReferenceEquals(resolvedScope, this) &&", 1),
             (S, "            if (context is not null)\n            {\n                resolvedScope = this;",
                 "            {\n                resolvedScope = this;", 1)],
            ["T18"]),
}


def apply(root, name):
    for path, old, new, expected in WITNESSES[name][1]:
        full = root.rstrip("/") + "/" + path
        raw = io.open(full, "rb").read()
        bom = raw.startswith(b"\xef\xbb\xbf")
        text = raw.decode("utf-8-sig")
        crlf = "\r\n" in text
        if crlf:
            old, new = old.replace("\n", "\r\n"), new.replace("\n", "\r\n")
        found = text.count(old)
        if found != expected:
            raise SystemExit("REFUSED %s: %s matched %d times, expected %d" % (name, path, found, expected))
        text = text.replace(old, new)
        io.open(full, "wb").write((b"\xef\xbb\xbf" if bom else b"") + text.encode("utf-8"))
        print("applied %s to %s" % (name, path))


if __name__ == "__main__":
    if sys.argv[2] == "list":
        print(json.dumps({k: {"defect": v[0], "must_fail": v[2]} for k, v in WITNESSES.items()}, indent=1))
    else:
        apply(sys.argv[1], sys.argv[2])
