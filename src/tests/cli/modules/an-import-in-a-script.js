// `import` in source presented as a SCRIPT is a syntax error, and not a construct the manifest
// excludes: the manifest admits the declaration and this goal does not.
import { anything } from "./lib.mjs";
anything;
