// An async generator as a module's DEFAULT export, which stayed refused by name after the family
// itself had been admitted everywhere else - a gap in `export default` rather than in the family.
import steps from "./an-async-generator.mjs";

steps().next().then(function (step) {
  print("first " + step.value + " " + step.done);
});

typeof steps;
