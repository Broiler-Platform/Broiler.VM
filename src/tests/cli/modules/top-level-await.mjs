// Top-level `await` in a module, and the ordering it exists to give: this module is evaluated
// after the one it imports has finished awaiting, so `ready` is the awaited value and not the
// one the dependency was initialised with.
import { ready } from "./top-level-await-dep.mjs";

print("importer sees " + ready);

const doubled = await Promise.resolve(21);
print("importer awaited " + (doubled * 2));
