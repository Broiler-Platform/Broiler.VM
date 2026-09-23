// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// `using` AND `await using` AT A MODULE'S TOP LEVEL (JSeal F21-F22, JSD-0034).
//
// The module's own resources are disposed when its body ends, before its evaluation settles. The
// asynchronous one is awaited first, so a reaction queued by the body runs before the last disposer
// does. Case 2 is printed by the synchronous disposer, which runs last.
var log = [];
using a = { [Symbol.dispose]() { log.push("a"); print("2 " + log.join()); } };
await using b = { async [Symbol.asyncDispose]() { log.push("b"); } };
await using c = null;
log.push("body");
Promise.resolve().then(function () { log.push("tick"); });
print("1 " + log.join());
