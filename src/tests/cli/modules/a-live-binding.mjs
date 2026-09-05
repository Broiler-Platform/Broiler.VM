// The four import forms this manifest admits, in one file.
import answer, { counter, bump } from "./lib.mjs";
import * as everything from "./lib.mjs";

bump();
bump();

answer + counter + everything.counter;
