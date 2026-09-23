// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module that awaits until the probe releases it.

await new Promise(function (resolve) { globalThis.identitySplitRelease = resolve; });

globalThis.identitySplitLog.push("slow end");
