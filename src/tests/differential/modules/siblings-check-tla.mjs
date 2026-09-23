// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async sibling that sets a flag only after its first await.

globalThis.siblingsCheck = false;
await 0;
globalThis.siblingsCheck = true;
