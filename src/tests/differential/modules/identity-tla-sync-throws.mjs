// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module with a top-level await that throws BEFORE its first await, so its evaluation promise is
// rejected by the time its body returns.

globalThis.identityTlaSyncRuns = (globalThis.identityTlaSyncRuns || 0) + 1;

if (globalThis.identityTlaSyncRuns > 0) {
  throw new SyntaxError("early");
}

await null;
