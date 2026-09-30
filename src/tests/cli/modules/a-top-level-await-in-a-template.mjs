// A REGRESSION FIXTURE FOR A TOP-LEVEL `await` INSIDE A TEMPLATE SUBSTITUTION.
//
// A substitution is parsed by a parser of its own, handed the module's `await` context. It recorded
// an `await` at the module's top level in its own flag, and nothing carried that flag out, so a module
// whose only suspension stood in a substitution was lowered as a unit that may not await and refused
// with "`await` is only admitted inside an async function" - a reason untrue of a module
// *(corrected: JSC-233)*. Each row is an ANSWER, taken from the comparison engine.

const plain = `${await Promise.resolve("plain")}`;
const tagged = ((strings, value) => strings.raw[0] + value)`tag-${await "ged"}`;
const nested = `outer-${`inner-${await Promise.resolve(3)}`}`;

print(plain + " " + tagged + " " + nested);
