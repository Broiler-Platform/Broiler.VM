import { value, increment } from "./modules/value.mjs";
increment();
print("1 " + value);
print("2 Grüße");
print("3 " + (this === undefined));
