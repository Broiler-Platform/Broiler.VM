// An import binding is immutable, and the failure is at RUN TIME rather than at parse.
import { counter } from "./lib.mjs";

try {
  counter = 1;
} catch (error) {
  error.name;
}
