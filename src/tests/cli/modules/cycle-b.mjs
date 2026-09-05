import { fromA } from "./cycle-a.mjs";

export function fromB() {
  return fromA() * 10;
}
