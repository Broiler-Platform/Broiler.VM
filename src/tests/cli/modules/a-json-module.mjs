// A JSON module is the document, parsed, as the one `default` export: `__proto__` is an own key
// and not the prototype, every importer sees the same object, and the dynamic form under the same
// type answers that object too. An untyped import of the same file would be a different module.
import data from "./data.json" with { type: "json" };
import { default as again } from "./data.json" with { type: "json" };

const own = Object.getOwnPropertyNames(data).join(",");
const proto = Object.getPrototypeOf(data) === Object.prototype;
const list = data.list
  .map((v) => (Object.is(v, -0) ? "-0" : typeof v === "string" ? v.charCodeAt(0).toString(16) : String(v)))
  .join(" ");

const viaDynamic = await import("./data.json", { with: { type: "json" } });

print(`${own} ${proto} ${data.__proto__} ${list} ${data.nested.deep.er.length} ${data === again} ${viaDynamic.default === data} ${Object.keys(viaDynamic).join()}`);
