"""E3 injected-defect witnesses of VM-5-002's re-collection, on the remedy head.

The first collection's witnesses.py restated for the owner-directed remedy: W3 restated for the poll
that counts a held block, W8 and W12 naming the tests that now carry them (T19 and T20), W13 to W25
added with W21b and W21c, and T23 read by row. This is the table the development drives at 965e6ad
and 16e3d6d used (their two scripts joined into one file, with no edit or must-fail entry changed).

Each witness is a local edit to src/Broiler.VM.Runtime in a worktree outside the repository, applied by
exact text replacement (every replacement must match the stated number of times, or the witness is
refused before anything runs). Nothing here is ever committed.

    python witnesses.py <worktree> <witness>
    python witnesses.py <worktree> list | names
    python witnesses.py <worktree> mustfail <witness>
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

POLL_TEST = ("            if (pollBound > 0 && sinceLastPoll + (used - usedAtLastPoll) > pollBound)\n"
             "            {\n"
             "                PollBoundExceeded = true;\n"
             "                return false;\n"
             "            }\n")
POLL_RESET = ("            sinceLastPoll = 0;\n"
              "            usedAtLastPoll = used;\n")

T8_62 = "Poll_Bound_Detection_Counts_Fuel_Charged_Between_Polls(nopCount: 0, before: 0, units: 62"
T8_106060 = "Poll_Bound_Detection_Counts_Fuel_Charged_Between_Polls(nopCount: 0, before: 10, units: 60"
T8_NOPS = "Poll_Bound_Detection_Counts_Fuel_Charged_Between_Polls(nopCount: 10000"
T21 = "A_Capability_That_Changes_Its_Context_Keeps_The_Change_And_Releases_Its_Depth"

# name -> (injected defect, [edits], tests that must fail: substrings of the test display name)
WITNESSES = {
    "W1": ("S9 deleted: the SettleAll in the runtime's budget snapshot",
           [(R, "            runtimeLevel.FuelPreAdmissions!.SettleAll();\n\n"
                "            return runtimeLevel.Snapshot();",
                "            return runtimeLevel.Snapshot();", 1)],
           ["A_Budget_Snapshot_Of_A_Held_Step_Counts_Every_Admitted_Charge"]),
    "W2": ("S3, S10 and S11 deleted together: the whole step-end settle",
           [S3, S10, S11],
           [T8_62]),
    "W3": ("Restated: in Poll, the read of the block and the write of usedAtLastPoll deleted, and the bound tested on sinceLastPoll alone",
           [(M, "            // What the fast path has admitted from this meter's block, read once. With no block held,\n"
                "            // size and remainder are both zero.\n"
                "            var used = preAdmissionSize - (ulong)System.Threading.Volatile.Read(ref preAdmittedFuel);\n\n", "", 1),
            (M, "sinceLastPoll + (used - usedAtLastPoll) > pollBound", "sinceLastPoll > pollBound", 1),
            (M, POLL_RESET, "            sinceLastPoll = 0;\n", 1)],
           [T8_106060]),
    "W4": ("LeavesRoomFor answers true",
           [(P, "    internal bool LeavesRoomFor(VmMeter meter, ulong amount) =>\n"
                "        amount <= Unreserved(meter.RuntimeLevel) &&\n"
                "        (meter.InstanceLevel is null || amount <= Unreserved(meter.InstanceLevel)) &&\n"
                "        amount <= Unreserved(meter.InvocationLevel);",
                "    internal bool LeavesRoomFor(VmMeter meter, ulong amount) => true;", 1)],
           ["Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit"]),
    "W4b": ("S3, S10 and the SettleAll branch of S1 deleted",
            [S3, S10, S1_ALL],
            ["An_Instance_Allowance_Spans_Invocations_To_The_Unit"]),
    "W5": ("S4 deleted: the settle in RemainingSnapshot",
           [(M, "                runtime.FuelPreAdmissions!.Settle(this);\n"
                "                return invocation.AsRemainingVector();",
                "                return invocation.AsRemainingVector();", 1)],
           ["A_Guest_Load_Is_Verified_Under_The_Exact_Remaining_Invocation_Fuel"]),
    "W6": ("The MayPreAdmit test in PreAdmit deleted",
           [(P, "        if (!meter.MayPreAdmit || meter.PreAdmissionSizeLocked != 0)",
                "        if (meter.PreAdmissionSizeLocked != 0)", 1)],
           ["An_Aggregate_Parent_Sees_Every_Charge_Of_A_Held_Step"]),
    "W7": ("The fast-path head returns false instead of calling TryChargeLocked",
           [(M, "        return TryChargeLocked(dimension, amount);\n    }",
                "        return false;\n    }", 1)],
           ["A_Runtime_Fuel_Ceiling_Is_Spent_To_The_Unit_Under_Windowed_Polling"]),
    "W8": ("VmExecutionScope.Current returns the thread's resolved meter without comparing contexts",
           [(S, "                ReferenceEquals(resolvedScope, this) &&\n"
                "                ReferenceEquals(resolvedContext, context))",
                "                ReferenceEquals(resolvedScope, this))", 1)],
           ["Fuel_Charged_Through_The_Environment_Meter_Follows_The_Execution_Context",
            "A_Charge_Made_Under_Another_Operations_Context_Bills_That_Operation"]),
    "W9": ("S5 deleted: the SettleAll before retention's admission check",
           [(M, "            // Every holder, not just this meter: a retention is admitted against the runtime and\n"
                "            // instance levels, which every operation of this runtime shares, so fuel any of them\n"
                "            // has spent and not yet committed is fuel this admission must already see.\n"
                "            if (dimension is VmBudgetDimension.Fuel)\n"
                "            {\n"
                "                runtime.FuelPreAdmissions!.SettleAll();\n"
                "            }\n\n", "", 1)],
           ["A_Fuel_Retention_Is_Admitted_Against_Every_Holders_Charges(holders: 1)",
            "A_Fuel_Retention_Is_Admitted_Against_Every_Holders_Charges(holders: 2)"]),
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
            ["A_Fuel_Retention_Is_Admitted_Against_Every_Holders_Charges(holders: 2)"]),
    "W10": ("The dimension test removed from the fast-path head",
            [(M, "        if (dimension == VmBudgetDimension.Fuel && amount != 0)",
                 "        if (amount != 0)", 1)],
            ["A_Non_Fuel_Charge_Never_Spends_A_Fuel_Block"]),
    "W11": ("An eviction drops its victim's block instead of committing it",
            [(P, "            holders[0]!.CommitPreAdmittedFuelLocked();\n            RemoveAt(0);",
                 "            RemoveAt(0);", 1)],
            ["A_Full_Table_Commits_The_Block_It_Evicts"]),
    "W12": ("A lookup with the flow suppressed is held under the null context",
            [(S, "            if (context is not null &&\n                ReferenceEquals(resolvedScope, this) &&",
                 "            if (ReferenceEquals(resolvedScope, this) &&", 1),
             (S, "            if (context is not null)\n            {\n                resolvedScope = this;",
                 "            {\n                resolvedScope = this;", 1)],
            ["A_Suppressed_Lookup_Is_Not_Answered_From_An_Earlier_One_On_The_Same_Thread"]),
    "W13": ("In CommitPreAdmittedFuelLocked, the whole of what the block admitted is folded instead of what no poll counted",
            [(M, "        sinceLastPoll += uncounted;", "        sinceLastPoll += used;", 1)],
            [T8_NOPS]),
    "W14": ("In Poll, the write of usedAtLastPoll deleted",
            [(M, POLL_RESET, "            sinceLastPoll = 0;\n", 1)],
            [T8_NOPS]),
    "W15": ("In CommitPreAdmittedFuelLocked, the clearing of usedAtLastPoll deleted",
            [(M, "        preAdmissionSize = 0;\n        usedAtLastPoll = 0;\n", "        preAdmissionSize = 0;\n", 1)],
            [T8_NOPS]),
    "W16": ("In LeaveCapability, the entry context put back whenever one was recorded, with no identity test",
            [(R, "            if (entry.Before is not null &&\n"
                 "                ReferenceEquals(System.Threading.ExecutionContext.Capture(), entry.Inside))",
                 "            if (entry.Before is not null)", 1)],
            [T21]),
    "W17": ("In LeaveCapability, the write-back after the restore branch deleted",
            [(R, "            var depth = CapabilityDepth - 1;\n\n"
                 "            // Null, not zero. Storing zero would leave the entry on this thread forever; storing\n"
                 "            // nothing is what actually releases it, and releasing it is the whole point.\n"
                 "            inCapabilityDepth.Value = depth > 0 ? depth : null;\n", "", 1)],
            [T21]),
    "W18": ("In EnterCapability, the entry context captured after the depth write",
            [(R, "        var before = System.Threading.ExecutionContext.Capture();\n\n"
                 "        inCapabilityDepth.Value = CapabilityDepth + 1;\n",
                 "        inCapabilityDepth.Value = CapabilityDepth + 1;\n\n"
                 "        var before = System.Threading.ExecutionContext.Capture();\n", 1)],
            [T21]),
    "W19": ("In LeaveCapability, the return after Restore deleted",
            [(R, "                System.Threading.ExecutionContext.Restore(entry.Before);\n                return;\n",
                 "                System.Threading.ExecutionContext.Restore(entry.Before);\n", 1)],
            ["A_Capability_Called_From_Inside_Another_Leaves_The_Outer_Call_Inside_Its_Boundary"]),
    "W20": ("In LeaveCapability, a depth of zero stored instead of released (control 16's injection)",
            [(R, "            inCapabilityDepth.Value = depth > 0 ? depth : null;",
                 "            inCapabilityDepth.Value = depth;", 1)],
            ["A_Disposed_Runtime_Leaves_No_Per_Thread_State_Behind_When_Its_Capability_Changes_Its_Context"]),
    "W21": ("In Poll, a refused poll records what it counted as counted",
            [(M, "                PollBoundExceeded = true;\n                return false;\n",
                 "                PollBoundExceeded = true;\n                usedAtLastPoll = used;\n                return false;\n", 1)],
            ["A_Poll_After_A_Refused_Poll_Is_Refused_Again"]),
    "W21b": ("In Poll, the reset moved above the bound test",
             [(M, POLL_TEST + "\n" + POLL_RESET, POLL_RESET + "\n" + POLL_TEST, 1)],
             ["A_Poll_After_A_Refused_Poll_Is_Refused_Again", T8_106060]),
}



T23_27 = "A_Poll_After_A_Refused_Poll_Is_Refused_Again(units: 27"
T23_28 = "A_Poll_After_A_Refused_Poll_Is_Refused_Again(units: 28"
T24 = "A_Host_That_Invokes_With_The_Flow_Suppressed_Has_Its_Capability_Calls_Released"
T25 = "A_Charge_Through_Another_Runtimes_Scope_Is_Not_Answered_From_This_Runtimes_Meter"
T26 = "A_Meter_Charged_From_Two_Threads_Is_Polled_On_Exactly_The_Work_Between_Polls"
RESTORE = "A_Capability_That_Changes_Nothing_Returns_Its_Caller_To_The_Context_It_Was_Called_Under"

d, e, _ = WITNESSES["W21"]
WITNESSES["W21"] = (d, e, [T23_27])
d, e, _ = WITNESSES["W21b"]
WITNESSES["W21b"] = (d, e, [T23_27, T23_28, T8_106060])

WITNESSES["W21c"] = (
    "In Poll, a refused poll resets the uncharged-work counter",
    [(M, "                PollBoundExceeded = true;\n                return false;\n",
         "                PollBoundExceeded = true;\n                sinceLastPoll = 0;\n                return false;\n", 1)],
    [T23_28])
WITNESSES["W22"] = (
    "In LeaveCapability, the test of whether the entry recorded a context removed",
    [(R, "            if (entry.Before is not null &&\n"
         "                ReferenceEquals(System.Threading.ExecutionContext.Capture(), entry.Inside))",
         "            if (ReferenceEquals(System.Threading.ExecutionContext.Capture(), entry.Inside))", 1)],
    [T24])
WITNESSES["W23"] = (
    "In VmExecutionScope.Current, the scope comparison removed",
    [(S, "                ReferenceEquals(resolvedScope, this) &&\n", "", 1)],
    [T25])
WITNESSES["W24"] = (
    "In Poll, the block read before the gate is taken",
    [(M, "        lock (gate)\n"
         "        {\n"
         "            // What the fast path has admitted from this meter's block, read once. With no block held,\n"
         "            // size and remainder are both zero.\n"
         "            var used = preAdmissionSize - (ulong)System.Threading.Volatile.Read(ref preAdmittedFuel);\n",
         "        // What the fast path has admitted from this meter's block, read once. With no block held,\n"
         "        // size and remainder are both zero.\n"
         "        var used = preAdmissionSize - (ulong)System.Threading.Volatile.Read(ref preAdmittedFuel);\n\n"
         "        lock (gate)\n"
         "        {\n", 1)],
    [T26])
WITNESSES["W25"] = (
    "In EnterCapability, every entry records nothing, so every return writes the depth back",
    [(R, "        return before is null\n            ? default\n",
         "        return before is null || true\n            ? default\n", 1)],
    [RESTORE])

ORDER = ["W1", "W2", "W3", "W4", "W4b", "W5", "W6", "W7", "W8", "W9", "W9b", "W10", "W11", "W12",
         "W13", "W14", "W15", "W16", "W17", "W18", "W19", "W20", "W21", "W21b", "W21c", "W22", "W23",
         "W24", "W25"]
assert sorted(ORDER) == sorted(WITNESSES.keys())


def apply(root, name):
    for path, old, new, expected in WITNESSES[name][1]:
        full = root.rstrip("/") + "/" + path
        raw = io.open(full, "rb").read()
        bom = raw.startswith(b"\xef\xbb\xbf")
        text = raw.decode("utf-8-sig")
        if "\r\n" in text:
            old, new = old.replace("\n", "\r\n"), new.replace("\n", "\r\n")
        found = text.count(old)
        if found != expected:
            raise SystemExit("REFUSED %s: %s matched %d times, expected %d" % (name, path, found, expected))
        text = text.replace(old, new)
        io.open(full, "wb").write((b"\xef\xbb\xbf" if bom else b"") + text.encode("utf-8"))
        print("applied %s to %s" % (name, path))


if __name__ == "__main__":
    if sys.argv[2] == "list":
        print(json.dumps({k: {"defect": WITNESSES[k][0], "must_fail": WITNESSES[k][2]} for k in ORDER}, indent=1))
    elif sys.argv[2] == "mustfail":
        print("\n".join(WITNESSES[sys.argv[3]][2]))
    elif sys.argv[2] == "names":
        print(" ".join(ORDER))
    else:
        apply(sys.argv[1], sys.argv[2])
