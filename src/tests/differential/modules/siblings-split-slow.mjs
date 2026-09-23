// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The slow module the split cycle waits on.

await new Promise(function (resolve) { globalThis.siblingsSplitRelease = resolve; });

globalThis.siblingsLog.push("slow end");
